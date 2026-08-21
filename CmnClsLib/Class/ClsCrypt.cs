using System.Security.Cryptography;
using System.Text;
using CmnClsLib.Module;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

//  ## ハッシュアルゴリズムとキーサイズの対応
//  _hashAlgorithm | ハッシュ長 | 鍵導出方式  | 有効な _keySize | _keySizeの推奨値
// ----------------|------------|-------------|-----------------|------------------
//  MD5            | 128 bit    | OpenSSL MD5 | 128, 256        | 128 (互換用途)
//  SHA1           | 160 bit    | PBKDF2      | 128, 192, 256   | 128 または 256
//  SHA256         | 256 bit    | PBKDF2      | 128, 192, 256   | 128 または 256
//  SHA512         | 512 bit    | PBKDF2      | 128, 192, 256   | 128 または 256
// 
//  ## OpenSSL引数説明
//  引数              | 説明                               | 備考
// -------------------|------------------------------------|--------------------------------------------------
//  -aes-128-cbc      | 暗号方式・鍵サイズ・暗号利用モード | _keySize=128 に対応（256 のときは -aes-256-cbc）
//  -base64           | Base64 文字列のデコード            | -a でも同様
//  -d                | 復号化 (Decrypt)                   | 暗号化時の -e から -d に変更
//  -pass pass:暗号鍵 | 復号鍵（パスワード）               |
//  -md sha256        | 鍵導出のハッシュアルゴリズム       | md5 sha1 sha256 sha512
//  -pbkdf2           | PBKDF2 鍵導出関数の使用            | md5のみPBKDF2ではなくOpenSSL独自のEVP_BytesToKey方式(-iterと-pbkdf2は指定しない)
//  -iter 繰返回数    | PBKDF2 のストレッチング回数        | sha1 sha256 sha512で指定可

namespace CmnClsLib.Class
{
    /// <summary>
    /// AES 暗号化および復号機能を提供するクラスです。
    /// OpenSSL 互換の暗号化方式（"Salted__" ヘッダー付き Base64 文字列）に対応しています。
    /// </summary>
    public class ClsCrypt
    {
        public const string DEFAULT_HASH_ALGORITHM = MdlConst.CRYPT_HASHALGORITHM;
        public const int DEFAULT_ITERATION_COUNT = MdlConst.CRYPT_ITERATIONCOUNT;
        public const int DEFAULT_KEY_SIZE = MdlConst.CRYPT_KEYSIZE;
        public const int DEFAULT_BLOCK_SIZE = MdlConst.CRYPT_BLOCKSIZE;

        private string _errorMessage = "";                          // エラーメッセージ
        private string _errorDump = "";                             // エラーダンプ
        private string _result = "";                                // 処理結果
        private int _keySize = DEFAULT_KEY_SIZE;                    // 暗号鍵サイズ（bit）
        private int _blockSize = DEFAULT_BLOCK_SIZE;                // 暗号ブロックサイズ（bit）
        private string _hashAlgorithm = DEFAULT_HASH_ALGORITHM;     // ハッシュアルゴリズム
        private int _iterationCount = DEFAULT_ITERATION_COUNT;      // 繰返回数
        private bool _isVerbose = false;                            // 冗長出力フラグ
        private byte[]? _key;                                       // 秘密鍵
        private byte[]? _iv;                                        // 初期化ベクトル
        private string _encKeyEnvName = "";                         // 暗号鍵格納環境変数名

        private static readonly char[] RandomCharCandidates = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        /// <summary>
        /// <see cref="ClsCrypt"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <example>
        /// <code>
        /// var crypt = new ClsCrypt();
        /// </code>
        /// </example>
        public ClsCrypt()
        {
        }

        public string DefaultEncKey => MdlConst.CRYPT_KEY;
        public string ErrorMessage => _errorMessage;
        public string ErrorDump => _errorDump;
        public string Result => _result;
        public int KeySize { get => _keySize; set => _keySize = value; }
        public int BlockSize { get => _blockSize; set => _blockSize = value; }
        public int IterationCount { get => _iterationCount; set => _iterationCount = value; }
        public string HashAlgorithm { get => _hashAlgorithm; set => _hashAlgorithm = value.ToUpperInvariant(); }
        public bool IsVerbose { get => _isVerbose; set => _isVerbose = value; }
        public string EncKeyEnvName { get => _encKeyEnvName; set => _encKeyEnvName = value; }

        /// <summary>
        /// 暗号鍵のエイリアス名を変換します。(非推奨の旧メソッド名)
        /// </summary>
        /// <param name="key">変換対象の暗号鍵またはエイリアス名。</param>
        /// <returns>変換後の実際の暗号鍵文字列。</returns>
        /// <example>
        /// <code>
        /// var crypt = new ClsCrypt();
        /// string resolvedKey = crypt.FixKeyValue("DEFAULT");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ResolveKeyAlias(key)' を使用します。")]
        public string FixKeyValue(string key)
        {
            return ResolveKeyAlias(key);
        }

        /// <summary>
        /// 暗号鍵のエイリアス名や環境変数名を評価し、実際の暗号鍵文字列を取得します。
        /// </summary>
        /// <param name="key">暗号鍵、またはエイリアス名 ("DEFAULT", "ENV", "BUILTIN" 等)。</param>
        /// <returns>評価・解決された実際の暗号鍵文字列。</returns>
        /// <example>
        /// <code>
        /// var crypt = new ClsCrypt();
        /// string key = crypt.ResolveKeyAlias("DEFAULT");
        /// </code>
        /// </example>
        public string ResolveKeyAlias(string key)
        {
            if (string.IsNullOrEmpty(_encKeyEnvName)) _encKeyEnvName = MdlConst.CRYPT_KEY_NAME_ENV;
            return key.Trim() switch
            {
                "" or MdlConst.CRYPT_KEY_ALIAS_DEFAULT => Environment.GetEnvironmentVariable(_encKeyEnvName) is string envVal && !string.IsNullOrEmpty(envVal) ? envVal : MdlConst.CRYPT_KEY,
                MdlConst.CRYPT_KEY_ALIAS_ENV => Environment.GetEnvironmentVariable(_encKeyEnvName) switch
                {
                    string envVal when !string.IsNullOrEmpty(envVal) => envVal,
                    _ => throw new InvalidOperationException($"環境変数({_encKeyEnvName})が設定されていません。")
                },
                MdlConst.CRYPT_KEY_ALIAS_BUILTIN => MdlConst.CRYPT_KEY,
                _ => key
            };
        }

        /// <summary>
        /// 指定された平文文字列を AES 暗号化します。暗号化結果は <see cref="Result"/> プロパティに格納されます。
        /// </summary>
        /// <param name="key">共通暗号鍵またはキーのエイリアス名。</param>
        /// <param name="plainText">暗号化する平文文字列。</param>
        /// <returns>暗号化処理が成功した場合は true。失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// var crypt = new ClsCrypt();
        /// if (crypt.Encrypt("my_secret_key", "パスワード123"))
        /// {
        ///     Console.WriteLine(crypt.Result); // Base64暗号文
        /// }
        /// </code>
        /// </example>
        public bool Encrypt(string key, string plainText)
        {
            try
            {
                key = ResolveKeyAlias(key);
                string saltString = GenerateRandomString(8);
                byte[] saltBytes = Encoding.ASCII.GetBytes(saltString);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] prefixBytes = Encoding.ASCII.GetBytes("Salted__" + saltString);

                if (_isVerbose)
                {
                    Console.WriteLine($"[ClsCrypt.Encrypt()] _keySize        = {_keySize}");
                    Console.WriteLine($"[ClsCrypt.Encrypt()] _blockSize      = {_blockSize}");
                    Console.WriteLine($"[ClsCrypt.Encrypt()] _hashAlgorithm  = {_hashAlgorithm}");
                    Console.WriteLine($"[ClsCrypt.Encrypt()] _iterationCount = {_iterationCount}");
                }

                bool keyDerived = _hashAlgorithm switch
                {
                    "MD5" => DeriveOpenSslKey(keyBytes, saltBytes),
                    _ => DeriveOpenSslKeyPbkdf2(keyBytes, saltBytes)
                };
                if (!keyDerived) return false;

                using Aes aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = _keySize;
                aes.BlockSize = _blockSize;
                if (_key is not null) aes.Key = _key;
                if (_iv is not null) aes.IV = _iv;

                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cs.FlushFinalBlock();
                }
                byte[] encryptedBytes = ms.ToArray();

                byte[] combinedBytes = new byte[prefixBytes.Length + encryptedBytes.Length];
                prefixBytes.CopyTo(combinedBytes, 0);
                encryptedBytes.CopyTo(combinedBytes, prefixBytes.Length);

                _result = Convert.ToBase64String(combinedBytes);
                return true;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                _errorDump = ex.StackTrace ?? "";
                return false;
            }
        }

        /// <summary>
        /// Base64 エンコードされた暗号文文字列を AES 復号化します。復号結果は <see cref="Result"/> プロパティに格納されます。
        /// </summary>
        /// <param name="key">共通暗号鍵またはキーのエイリアス名。</param>
        /// <param name="cipherTextBase64">復号対象の Base64 文字列（OpenSSL 互換フォーマット）。</param>
        /// <returns>復号化処理が成功した場合は true。失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// var crypt = new ClsCrypt();
        /// if (crypt.Decrypt("my_secret_key", cipherTextBase64))
        /// {
        ///     Console.WriteLine(crypt.Result); // 復号された平文文字列
        /// }
        /// </code>
        /// </example>
        public bool Decrypt(string key, string cipherTextBase64)
        {
            try
            {
                key = ResolveKeyAlias(key);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] combinedBytes = Convert.FromBase64String(cipherTextBase64);

                byte[] saltBytes = combinedBytes.AsSpan(8, 8).ToArray();
                byte[] encryptedBytes = combinedBytes.AsSpan(16).ToArray();

                if (_isVerbose)
                {
                    Console.WriteLine($"[ClsCrypt.Decrypt()] _keySize        = {_keySize}");
                    Console.WriteLine($"[ClsCrypt.Decrypt()] _blockSize      = {_blockSize}");
                    Console.WriteLine($"[ClsCrypt.Decrypt()] _hashAlgorithm  = {_hashAlgorithm}");
                    Console.WriteLine($"[ClsCrypt.Decrypt()] _iterationCount = {_iterationCount}");
                }

                bool keyDerived = _hashAlgorithm switch
                {
                    "MD5" => DeriveOpenSslKey(keyBytes, saltBytes),
                    _ => DeriveOpenSslKeyPbkdf2(keyBytes, saltBytes)
                };
                if (!keyDerived) return false;

                using Aes aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = _keySize;
                aes.BlockSize = _blockSize;
                if (_key is not null) aes.Key = _key;
                if (_iv is not null) aes.IV = _iv;

                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                    cs.FlushFinalBlock();
                }
                byte[] plainTextBytes = ms.ToArray();

                _result = Encoding.UTF8.GetString(plainTextBytes);
                return true;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                _errorDump = ex.StackTrace ?? "";
                return false;
            }
        }

        /// <summary>
        /// OpenSSL 互換の MD5 方式で秘密鍵および初期化ベクトル (IV) を生成します。(非推奨の旧メソッド名)
        /// </summary>
        /// <param name="baKey">暗号鍵のバイト配列。</param>
        /// <param name="baSalt">SALT値のバイト配列。</param>
        /// <returns>生成が成功した場合は true。失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// var crypt = new ClsCrypt();
        /// crypt.GetOpenSSLKey(Encoding.UTF8.GetBytes("key"), Encoding.ASCII.GetBytes("12345678"));
        /// </code>
        /// </example>
        [Obsolete("代わりに 'DeriveOpenSslKey(baKey, baSalt)' を使用します。")]
        public bool GetOpenSSLKey(byte[] baKey, byte[] baSalt)
        {
            return DeriveOpenSslKey(baKey, baSalt);
        }

        /// <summary>
        /// OpenSSL 互換の MD5 方式で秘密鍵および初期化ベクトル (IV) を派生・設定します。
        /// </summary>
        /// <param name="keyBytes">暗号鍵のバイト配列。</param>
        /// <param name="saltBytes">SALT値のバイト配列。</param>
        /// <returns>生成が成功した場合は true。失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// var crypt = new ClsCrypt();
        /// bool ok = crypt.DeriveOpenSslKey(Encoding.UTF8.GetBytes("key"), Encoding.ASCII.GetBytes("12345678"));
        /// </code>
        /// </example>
        public bool DeriveOpenSslKey(byte[] keyBytes, byte[] saltBytes)
        {
            try
            {
                Span<byte> preKey = stackalloc byte[keyBytes.Length + saltBytes.Length];
                keyBytes.CopyTo(preKey);
                saltBytes.CopyTo(preKey[keyBytes.Length..]);

                if (_keySize == 128)
                {
                    _key = MD5.HashData(preKey);
                    Span<byte> preIv = stackalloc byte[_key.Length + preKey.Length];
                    _key.CopyTo(preIv);
                    preKey.CopyTo(preIv[_key.Length..]);
                    _iv = MD5.HashData(preIv);
                }
                else
                {
                    byte[] hash1 = MD5.HashData(preKey);
                    Span<byte> preHash2 = stackalloc byte[16 + preKey.Length];
                    hash1.CopyTo(preHash2);
                    preKey.CopyTo(preHash2[16..]);
                    byte[] hash2 = MD5.HashData(preHash2);

                    _key = new byte[32];
                    hash1.CopyTo(_key.AsSpan(0, 16));
                    hash2.CopyTo(_key.AsSpan(16, 16));

                    Span<byte> preIv = stackalloc byte[16 + preKey.Length];
                    hash2.CopyTo(preIv);
                    preKey.CopyTo(preIv[16..]);
                    _iv = MD5.HashData(preIv);
                }
                return true;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                _errorDump = ex.StackTrace ?? "";
                return false;
            }
        }

        /// <summary>
        /// OpenSSL 互換の PBKDF2 方式で秘密鍵および初期化ベクトル (IV) を生成します。(非推奨の旧メソッド名)
        /// </summary>
        /// <param name="baKey">暗号鍵のバイト配列。</param>
        /// <param name="baSalt">SALT値のバイト配列。</param>
        /// <returns>生成が成功した場合は true。失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// var crypt = new ClsCrypt();
        /// crypt.GetOpenSSLKeyPBKDF2(Encoding.UTF8.GetBytes("key"), Encoding.ASCII.GetBytes("12345678"));
        /// </code>
        /// </example>
        [Obsolete("代わりに 'DeriveOpenSslKeyPbkdf2(baKey, baSalt)' を使用します。")]
        public bool GetOpenSSLKeyPBKDF2(byte[] baKey, byte[] baSalt)
        {
            return DeriveOpenSslKeyPbkdf2(baKey, baSalt);
        }

        /// <summary>
        /// OpenSSL 互換の PBKDF2 方式で秘密鍵および初期化ベクトル (IV) を派生・設定します。
        /// </summary>
        /// <param name="keyBytes">暗号鍵のバイト配列。</param>
        /// <param name="saltBytes">SALT値のバイト配列。</param>
        /// <returns>生成が成功した場合は true。失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// var crypt = new ClsCrypt();
        /// bool ok = crypt.DeriveOpenSslKeyPbkdf2(Encoding.UTF8.GetBytes("key"), Encoding.ASCII.GetBytes("12345678"));
        /// </code>
        /// </example>
        public bool DeriveOpenSslKeyPbkdf2(byte[] keyBytes, byte[] saltBytes)
        {
            if (_iterationCount < 1) _iterationCount = 1;
            try
            {
                HashAlgorithmName hashAlgorithmName = _hashAlgorithm switch
                {
                    "SHA512" => HashAlgorithmName.SHA512,
                    "SHA256" => HashAlgorithmName.SHA256,
                    _ => HashAlgorithmName.SHA1
                };

                int keyLengthInBytes = _keySize / 8;
                int ivLengthInBytes = _blockSize / 8;
                Span<byte> derivedBytes = stackalloc byte[keyLengthInBytes + ivLengthInBytes];

                Rfc2898DeriveBytes.Pbkdf2(keyBytes, saltBytes, derivedBytes, _iterationCount, hashAlgorithmName);

                _key = derivedBytes[..keyLengthInBytes].ToArray();
                _iv = derivedBytes[keyLengthInBytes..].ToArray();
                return true;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                _errorDump = ex.StackTrace ?? "";
                return false;
            }
        }

        /// <summary>
        /// 指定された長さのランダムな小文字英字文字列を生成します。
        /// </summary>
        /// <param name="length">生成する文字列の長さ。</param>
        /// <returns>生成されたランダム文字列。</returns>
        /// <example>
        /// <code>
        /// string salt = GenerateRandomString(8);
        /// </code>
        /// </example>
        private string GenerateRandomString(int length)
        {
            return RandomNumberGenerator.GetString(RandomCharCandidates, length);
        }

    }
}

