use std::{fs, io};
use std::path::PathBuf;
use std::error::Error;
use std::fs::File;

use zip::ZipArchive;

/// `zip_file` is the path of the zip file.
///
/// `out_dir` is the path of where the directory
/// where the unzipped items in the zip file
/// will go. It does not have to actually
/// exist.
///
/// Will return a PathBuf of `out_dir`
pub fn unzip_file(zip_file: &PathBuf, out_dir: &PathBuf) -> Result<PathBuf, Box<dyn Error>> {
    let file = File::open(zip_file)?;
    let mut zip = ZipArchive::new(file)?;

    fs::create_dir_all(out_dir)?;

    for i in 0..zip.len() {
        let mut file = zip.by_index(i)?;
        let path = match file.enclosed_name() {
            Some(path) => path,
            None => continue
        };

        let output = out_dir.join(path);

        if file.name().ends_with('/') {
            fs::create_dir_all(&output)?;
        } else {
            if let Some(parent) = output.parent() {
                fs::create_dir_all(parent)?;
            }

            let mut new = File::create(&output)?;
            io::copy(&mut file, &mut new)?;
        }
    }

    Ok(out_dir.to_path_buf())
}

pub fn find_file(
    search_dir: &PathBuf,
    target_extension: &str,
    ignored_files: &[&str]
) -> Result<PathBuf, Box<dyn Error>> {
    let files = fs::read_dir(search_dir)?;

    for file in files {
        let path = file?.path();
        if !path.is_file() {
            continue;
        }

        if let Some(name) = path.file_name() {
            let name = name.to_string_lossy();

            if ignored_files.iter().any(|str| name.contains(str)) {
                continue;
            }
        }

        if let Some(ext) = path.extension() {
            if ext.to_string_lossy().to_lowercase() != target_extension.to_lowercase() {
                continue;
            }

            return Ok(path);
        }
    }

    Err("failed to find zip file".into())
}

pub fn get_lesser_codes(data: &str) -> Vec<u8> {
    let mut codes = Vec::new();

    for char in data.to_lowercase().chars() {
        if !char.is_ascii_alphabetic() {
            continue;
        }

        let code = (char as u8) - ('a' as u8) + 1;
        codes.push(code);
    }

    codes
}