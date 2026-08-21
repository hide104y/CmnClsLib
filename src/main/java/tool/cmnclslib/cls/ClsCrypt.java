package tool.cmnclslib.cls;

import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.SecureRandom;
import java.util.Base64;
import java.util.Locale;
import javax.crypto.Cipher;
import javax.crypto.SecretKeyFactory;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.PBEKeySpec;
import javax.crypto.spec.SecretKeySpec;
import tool.cmnclslib.mdl.MdlConst;

/**
 *  ## ハッシュアルゴリズムとキーサイズの対応
 *  hashAlgorithm  | ハッシュ長 | 鍵導出方式  | 有効な _keySize | _keySizeの推奨値
 * ----------------|------------|-------------|-----------------|------------------
 *  MD5            | 128 bit    | OpenSSL MD5 | 128, 256        | 128 (互換用途)
 *  SHA1           | 160 bit    | PBKDF2      | 128, 192, 256   | 128 または 256
 *  SHA256         | 256 bit    | PBKDF2      | 128, 192, 256   | 128 または 256
 *  SHA512         | 512 bit    | PBKDF2      | 128, 192, 256   | 128 または 256
 *
 *  ## OpenSSL引数説明
 *  引数              | 説明                               | 備考
 * -------------------|------------------------------------|--------------------------------------------------
 *  -aes-128-cbc      | 暗号方式・鍵サイズ・暗号利用モード | keySize=128 に対応（256 のときは -aes-256-cbc）
 *  -base64           | Base64 文字列のデコード            | -a でも同様
 *  -d                | 復号化 (Decrypt)                   | 暗号化時の -e から -d に変更
 *  -pass pass:暗号鍵 | 復号鍵（パスワード）               |
 *  -md sha256        | 鍵導出のハッシュアルゴリズム       | md5 sha1 sha256 sha512
 *  -pbkdf2           | PBKDF2 鍵導出関数の使用            | md5のみPBKDF2ではなくOpenSSL独自のEVP_BytesToKey方式(-iterと-pbkdf2は指定しない)
 *  -iter 繰返回数    | PBKDF2 のストレッチング回数        | sha1 sha256 sha512で指定可
 */

/**
 * AES 暗号化および復号機能を提供するクラスです。
 * OpenSSL 互換の暗号化方式（"Salted__" ヘッダー付き Base64 文字列）に対応しています。
 */
public class ClsCrypt {

    public static final String DEFAULT_HASH_ALGORITHM = MdlConst.CRYPT_HASHALGORITHM;
    public static final int DEFAULT_ITERATION_COUNT = MdlConst.CRYPT_ITERATIONCOUNT;
    public static final int DEFAULT_KEY_SIZE = MdlConst.CRYPT_KEYSIZE;
    public static final int DEFAULT_BLOCK_SIZE = MdlConst.CRYPT_BLOCKSIZE;

    private static final String RANDOM_CHARS = "abcdefghijklmnopqrstuvwxyz";
    private static final SecureRandom SECURE_RANDOM = new SecureRandom();

    private String errorMessage = "";
    private String errorDump = "";
    private String result = "";
    private int keySize = DEFAULT_KEY_SIZE;
    private int blockSize = DEFAULT_BLOCK_SIZE;
    private String hashAlgorithm = DEFAULT_HASH_ALGORITHM;
    private int iterationCount = DEFAULT_ITERATION_COUNT;
    private boolean isVerbose = false;
    private byte[] keyBytesDerived;
    private byte[] ivBytesDerived;
    private String encKeyEnvName = "";

    /**
     * ClsCrypt クラスの新しいインスタンスを初期化します。
     */
    public ClsCrypt() {
    }

    public String getDefaultEncKey() {
        return MdlConst.CRYPT_KEY;
    }

    public String getErrorMessage() {
        return errorMessage;
    }

    public String getErrorDump() {
        return errorDump;
    }

    public String getResult() {
        return result;
    }

    public int getKeySize() {
        return keySize;
    }

    public void setKeySize(int keySize) {
        this.keySize = keySize;
    }

    public int getBlockSize() {
        return blockSize;
    }

    public void setBlockSize(int blockSize) {
        this.blockSize = blockSize;
    }

    public int getIterationCount() {
        return iterationCount;
    }

    public void setIterationCount(int iterationCount) {
        this.iterationCount = iterationCount;
    }

    public String getHashAlgorithm() {
        return hashAlgorithm;
    }

    public void setHashAlgorithm(String hashAlgorithm) {
        this.hashAlgorithm = hashAlgorithm != null ? hashAlgorithm.toUpperCase(Locale.ROOT) : DEFAULT_HASH_ALGORITHM;
    }

    public boolean isVerbose() {
        return isVerbose;
    }

    public void setVerbose(boolean verbose) {
        isVerbose = verbose;
    }

    public String getEncKeyEnvName() {
        return encKeyEnvName;
    }

    public void setEncKeyEnvName(String encKeyEnvName) {
        this.encKeyEnvName = encKeyEnvName != null ? encKeyEnvName : "";
    }

    /**
     * 暗号鍵のエイリアス名や環境変数名を評価し、実際の暗号鍵文字列を取得します。
     *
     * @param key 暗号鍵、またはエイリアス名 ("DEFAULT", "ENV", "BUILTIN" 等)
     * @return 評価・解決された実際の暗号鍵文字列
     */
    public String resolveKeyAlias(String key) {
        String envName = (encKeyEnvName != null && !encKeyEnvName.isEmpty()) ? encKeyEnvName : MdlConst.CRYPT_KEY_NAME_ENV;
        String trimmed = key != null ? key.trim() : "";

        if (trimmed.isEmpty() || MdlConst.CRYPT_KEY_ALIAS_DEFAULT.equals(trimmed)) {
            String envVal = System.getenv(envName);
            return (envVal != null && !envVal.isEmpty()) ? envVal : MdlConst.CRYPT_KEY;
        } else if (MdlConst.CRYPT_KEY_ALIAS_ENV.equals(trimmed)) {
            String envVal = System.getenv(envName);
            if (envVal != null && !envVal.isEmpty()) {
                return envVal;
            }
            throw new IllegalStateException("環境変数(" + envName + ")が設定されていません。");
        } else if (MdlConst.CRYPT_KEY_ALIAS_BUILTIN.equals(trimmed)) {
            return MdlConst.CRYPT_KEY;
        }
        return key;
    }

    /**
     * 指定された平文文字列を AES 暗号化します。暗号化結果は result プロパティに格納されます。
     *
     * @param key 共通暗号鍵またはキーのエイリアス名
     * @param plainText 暗号化する平文文字列
     * @return 暗号化処理が成功した場合は true、失敗した場合は false
     */
    public boolean encrypt(String key, String plainText) {
        try {
            String resolvedKey = resolveKeyAlias(key);
            String saltString = generateRandomString(8);
            byte[] saltBytes = saltString.getBytes(StandardCharsets.US_ASCII);
            byte[] keyBytes = resolvedKey.getBytes(StandardCharsets.UTF_8);
            byte[] plainTextBytes = plainText.getBytes(StandardCharsets.UTF_8);
            byte[] prefixBytes = ("Salted__" + saltString).getBytes(StandardCharsets.US_ASCII);

            if (isVerbose) {
                System.out.println("[ClsCrypt.Encrypt()] keySize        = " + keySize);
                System.out.println("[ClsCrypt.Encrypt()] blockSize      = " + blockSize);
                System.out.println("[ClsCrypt.Encrypt()] hashAlgorithm  = " + hashAlgorithm);
                System.out.println("[ClsCrypt.Encrypt()] iterationCount = " + iterationCount);
            }

            boolean keyDerived;
            if ("MD5".equalsIgnoreCase(hashAlgorithm)) {
                keyDerived = deriveOpenSslKey(keyBytes, saltBytes);
            } else {
                keyDerived = deriveKeyPbkdf2(keyBytes, saltBytes);
            }
            if (!keyDerived) {
                return false;
            }

            Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
            SecretKeySpec secretKeySpec = new SecretKeySpec(keyBytesDerived, "AES");
            IvParameterSpec ivParameterSpec = new IvParameterSpec(ivBytesDerived);
            cipher.init(Cipher.ENCRYPT_MODE, secretKeySpec, ivParameterSpec);

            byte[] encryptedBytes = cipher.doFinal(plainTextBytes);

            byte[] combinedBytes = new byte[prefixBytes.length + encryptedBytes.length];
            System.arraycopy(prefixBytes, 0, combinedBytes, 0, prefixBytes.length);
            System.arraycopy(encryptedBytes, 0, combinedBytes, prefixBytes.length, encryptedBytes.length);

            this.result = Base64.getEncoder().encodeToString(combinedBytes);
            return true;
        } catch (Exception ex) {
            this.errorMessage = ex.getMessage();
            this.errorDump = getStackTraceStr(ex);
            return false;
        }
    }

    /**
     * Base64 エンコードされた暗号文文字列を AES 復号化します。復号結果は result プロパティに格納されます。
     *
     * @param key 共通暗号鍵またはキーのエイリアス名
     * @param cipherTextBase64 復号対象の Base64 文字列（OpenSSL 互換フォーマット）
     * @return 復号化処理が成功した場合は true、失敗した場合は false
     */
    public boolean decrypt(String key, String cipherTextBase64) {
        try {
            String resolvedKey = resolveKeyAlias(key);
            byte[] keyBytes = resolvedKey.getBytes(StandardCharsets.UTF_8);
            byte[] combinedBytes = Base64.getDecoder().decode(cipherTextBase64);

            byte[] saltBytes = new byte[8];
            System.arraycopy(combinedBytes, 8, saltBytes, 0, 8);

            int encryptedLen = combinedBytes.length - 16;
            byte[] encryptedBytes = new byte[encryptedLen];
            System.arraycopy(combinedBytes, 16, encryptedBytes, 0, encryptedLen);

            if (isVerbose) {
                System.out.println("[ClsCrypt.Decrypt()] keySize        = " + keySize);
                System.out.println("[ClsCrypt.Decrypt()] blockSize      = " + blockSize);
                System.out.println("[ClsCrypt.Decrypt()] hashAlgorithm  = " + hashAlgorithm);
                System.out.println("[ClsCrypt.Decrypt()] iterationCount = " + iterationCount);
            }

            boolean keyDerived;
            if ("MD5".equalsIgnoreCase(hashAlgorithm)) {
                keyDerived = deriveOpenSslKey(keyBytes, saltBytes);
            } else {
                keyDerived = deriveKeyPbkdf2(keyBytes, saltBytes);
            }
            if (!keyDerived) {
                return false;
            }

            Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
            SecretKeySpec secretKeySpec = new SecretKeySpec(keyBytesDerived, "AES");
            IvParameterSpec ivParameterSpec = new IvParameterSpec(ivBytesDerived);
            cipher.init(Cipher.DECRYPT_MODE, secretKeySpec, ivParameterSpec);

            byte[] plainTextBytes = cipher.doFinal(encryptedBytes);
            this.result = new String(plainTextBytes, StandardCharsets.UTF_8);
            return true;
        } catch (Exception ex) {
            this.errorMessage = ex.getMessage();
            this.errorDump = getStackTraceStr(ex);
            return false;
        }
    }

    /**
     * OpenSSL 互換の MD5 方式で秘密鍵および初期化ベクトル (IV) を派生・設定します。
     *
     * @param keyBytes 暗号鍵のバイト配列
     * @param saltBytes SALT値のバイト配列
     * @return 生成が成功した場合は true、失敗した場合は false
     */
    public boolean deriveOpenSslKey(byte[] keyBytes, byte[] saltBytes) {
        try {
            MessageDigest md = MessageDigest.getInstance("MD5");
            byte[] preKey = new byte[keyBytes.length + saltBytes.length];
            System.arraycopy(keyBytes, 0, preKey, 0, keyBytes.length);
            System.arraycopy(saltBytes, 0, preKey, keyBytes.length, saltBytes.length);

            if (keySize == 128) {
                keyBytesDerived = md.digest(preKey);
                md.reset();
                byte[] preIv = new byte[keyBytesDerived.length + preKey.length];
                System.arraycopy(keyBytesDerived, 0, preIv, 0, keyBytesDerived.length);
                System.arraycopy(preKey, 0, preIv, keyBytesDerived.length, preKey.length);
                ivBytesDerived = md.digest(preIv);
            } else {
                byte[] hash1 = md.digest(preKey);
                md.reset();
                byte[] preHash2 = new byte[16 + preKey.length];
                System.arraycopy(hash1, 0, preHash2, 0, 16);
                System.arraycopy(preKey, 0, preHash2, 16, preKey.length);
                byte[] hash2 = md.digest(preHash2);

                keyBytesDerived = new byte[32];
                System.arraycopy(hash1, 0, keyBytesDerived, 0, 16);
                System.arraycopy(hash2, 0, keyBytesDerived, 16, 16);

                md.reset();
                byte[] preIv = new byte[16 + preKey.length];
                System.arraycopy(hash2, 0, preIv, 0, 16);
                System.arraycopy(preKey, 0, preIv, 16, preKey.length);
                ivBytesDerived = md.digest(preIv);
            }
            return true;
        } catch (Exception ex) {
            this.errorMessage = ex.getMessage();
            this.errorDump = getStackTraceStr(ex);
            return false;
        }
    }

    /**
     * OpenSSL 互換の PBKDF2 方式で秘密鍵および初期化ベクトル (IV) を派生・設定します。
     *
     * @param keyBytes 暗号鍵のバイト配列
     * @param saltBytes SALT値のバイト配列
     * @return 生成が成功した場合は true、失敗した場合は false
     */
    public boolean deriveKeyPbkdf2(byte[] keyBytes, byte[] saltBytes) {
        if (this.iterationCount < 1) {
            this.iterationCount = 1;
        }
        int effectiveIteration = this.iterationCount;
        try {
            String pbkdf2Algorithm;
            String upperAlgo = hashAlgorithm != null ? hashAlgorithm.toUpperCase(Locale.ROOT) : "SHA1";
            switch (upperAlgo) {
                case "SHA512":
                    pbkdf2Algorithm = "PBKDF2WithHmacSHA512";
                    break;
                case "SHA256":
                    pbkdf2Algorithm = "PBKDF2WithHmacSHA256";
                    break;
                default:
                    pbkdf2Algorithm = "PBKDF2WithHmacSHA1";
                    break;
            }

            int keyLengthInBytes = keySize / 8;
            int ivLengthInBytes = blockSize / 8;
            int totalBits = (keyLengthInBytes + ivLengthInBytes) * 8;

            char[] passwordChars = new String(keyBytes, StandardCharsets.UTF_8).toCharArray();
            PBEKeySpec spec = new PBEKeySpec(passwordChars, saltBytes, effectiveIteration, totalBits);
            SecretKeyFactory skf = SecretKeyFactory.getInstance(pbkdf2Algorithm);
            byte[] derivedBytes = skf.generateSecret(spec).getEncoded();

            keyBytesDerived = new byte[keyLengthInBytes];
            ivBytesDerived = new byte[ivLengthInBytes];
            System.arraycopy(derivedBytes, 0, keyBytesDerived, 0, keyLengthInBytes);
            System.arraycopy(derivedBytes, keyLengthInBytes, ivBytesDerived, 0, ivLengthInBytes);
            return true;
        } catch (Exception ex) {
            this.errorMessage = ex.getMessage();
            this.errorDump = getStackTraceStr(ex);
            return false;
        }
    }

    /**
     * @deprecated {@link #deriveKeyPbkdf2(byte[], byte[])} を使用してください。
     */
    @Deprecated
    public boolean deriveOpenSslKeyPbkdf2(byte[] keyBytes, byte[] saltBytes) {
        return deriveKeyPbkdf2(keyBytes, saltBytes);
    }

    private String generateRandomString(int length) {
        StringBuilder sb = new StringBuilder(length);
        for (int i = 0; i < length; i++) {
            int idx = SECURE_RANDOM.nextInt(RANDOM_CHARS.length());
            sb.append(RANDOM_CHARS.charAt(idx));
        }
        return sb.toString();
    }

    private String getStackTraceStr(Throwable t) {
        StringBuilder sb = new StringBuilder();
        for (StackTraceElement ste : t.getStackTrace()) {
            sb.append(ste.toString()).append(System.lineSeparator());
        }
        return sb.toString();
    }
}
