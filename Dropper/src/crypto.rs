use std::error::Error;
use std::fs::File;
use std::io::{BufReader, BufWriter, Read, Write};
use std::path::PathBuf;

use base64::prelude::*;
use sha2::{Sha256, Digest};

use chacha20poly1305::{ChaCha20Poly1305, Nonce};
use chacha20poly1305::aead::{Aead, KeyInit};

const TAG_LEN: usize = 16;
const NONCE_LEN: usize = 12;
const SALT_LEN: usize = 20;

const CHUNK_LEN: usize = 1 * 1024 * 1024;

pub fn decrypt_string(data: &str, key: &str) -> Result<String, Box<dyn Error>> {
    let data = BASE64_STANDARD.decode(data)?;
    if data.len() < SALT_LEN + NONCE_LEN + TAG_LEN {
        return Err("invalid data format".into());
    }

    let salt = &data[0..SALT_LEN];
    let nonce = &data[SALT_LEN..SALT_LEN + NONCE_LEN];
    let sealed = &data[SALT_LEN + NONCE_LEN..];

    let key = get_key(key, salt);
    let chacha = ChaCha20Poly1305::new(&key.into());

    let the_nonce = Nonce::from_slice(nonce);

    let decrypted = chacha
        .decrypt(the_nonce, sealed)
        .map_err(|_| "failed to decrypt")?;

    Ok(String::from_utf8(decrypted)?)
}

/// `encrypted_file` is the path of the encrypted file.
///
/// `decrypted_file` is the path of where the decrypted
/// file will be. It does not need to be a file that
/// actually exists. If it does exist, it will be
/// overwritten.
///
/// Will return a PathBuf of `decrypted_file`
pub fn decrypt_file(
    encrypted_file: &PathBuf,
    decrypted_file: &PathBuf,
    key: &str
) -> Result<PathBuf, Box<dyn Error>> {
    let enc_file = File::open(encrypted_file)?;
    let mut reader = BufReader::new(enc_file);

    let mut salt = vec![0u8; SALT_LEN];
    reader.read_exact(&mut salt)?;

    let key = get_key(key, &salt);
    let chacha = ChaCha20Poly1305::new(&key.into());

    let dec_file = File::create(decrypted_file)?;
    let mut writer = BufWriter::new(dec_file);

    loop {
        let mut nonce = vec![0u8; NONCE_LEN];
        let nonce_bytes = reader.read(&mut nonce)?;

        if nonce_bytes == 0 {
            break;
        }

        if nonce_bytes < NONCE_LEN {
            return Err("invalid data format".into());
        }

        let mut sealed = vec![0u8; CHUNK_LEN + TAG_LEN];
        let mut bytes = 0;

        while bytes < CHUNK_LEN + TAG_LEN {
            match reader.read(&mut sealed[bytes..]) {
                Ok(0) => break,
                Ok(n) => bytes += n,
                Err(e) => return Err(e.into()),
            }
        }

        if bytes == 0 {
            return Err("invalid data format".into());
        }

        sealed.truncate(bytes);

        let the_nonce = Nonce::from_slice(&nonce);

        let decrypted = chacha
            .decrypt(the_nonce, sealed.as_ref())
            .map_err(|_| "failed to decrypt")?;

        writer.write_all(&decrypted)?;
    }

    writer.flush()?;

    Ok(decrypted_file.to_path_buf())
}


fn get_key(key: &str, salt: &[u8]) -> [u8; 32] {
    let mut sha256 = Sha256::new();
    sha256.update(key.as_bytes());
    sha256.update(salt);

    sha256
        .finalize()
        .into()
}

#[test]
fn test_string_decryption() {
    use std::process;

    let encryption_key = "uhuAzxyqja0xoicrolZC4";
    let encrypted_string = "ZXYqBIe7V8ovucW7hyswDopqtyyh20MM3d6Xw7neHotF4khfOpqdLGzC5WCx8ZxlkbA24VmbDfU1noNvK2UbjBq/CHfWRY9hxJitlUYTDzlvjjuRYsQl8LgV3ivtQvPXNido";

    let expected_string = "https://https://files.catbox.moe/ergbuh23786sdafz12";

    let decrypted_string = match decrypt_string(encrypted_string, encryption_key) {
        Ok(decrypted) => decrypted,
        Err(error) => {
            println!("failed to decrypt - {}", error);
            process::exit(1);
        }
    };

    assert_eq!(decrypted_string, expected_string);
}