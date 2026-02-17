use std::error::Error;
use std::path::PathBuf;
use std::time::Duration;
use std::fs::File;
use std::io::Write;

use reqwest::Client;

/// `url` is the url of where the file will be
/// downloaded from.
///
/// `out_file` is the path of where the downloaded
/// file will be. It does not need to be a file that
/// actually exists. If it does exist, it will be
/// overwritten.
///
/// Will return a PathBuf of `out_file`
pub async fn download_file(url: &str, out_file: &PathBuf) -> Result<PathBuf, Box<dyn Error>> {
    let client = Client::builder()
        .timeout(Duration::from_secs(270))
        .user_agent("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:145.0) Gecko/20100101 Firefox/145.0")
        .build()?;

    let res = client
        .get(url)
        .send()
        .await?;

    if !res.status().is_success() {
        return Err("status not success".into());
    }

    let body = res
        .bytes()
        .await?;

    let mut file = File::create(&out_file)?;
    file.write_all(&body)?;

    Ok(out_file.to_path_buf())
}