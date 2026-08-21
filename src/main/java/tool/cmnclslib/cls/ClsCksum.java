package tool.cmnclslib.cls;

import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;
import java.security.MessageDigest;
import java.util.Locale;

/**
 * 各種ハッシュおよび POSIX cksum アルゴリズムによるチェックサム計算機能を提供するクラスです。
 */
public class ClsCksum {

    private static final long[] CRC_TABLE = new long[] {
            0x00000000L, 0x04C11DB7L, 0x09823B6EL, 0x0D4326D9L, 0x130476DCL, 0x17C56B6BL,
            0x1A864DB2L, 0x1E475005L, 0x2608EDB8L, 0x22C9F00FL, 0x2F8AD6D6L, 0x2B4BCB61L,
            0x350C9B64L, 0x31CD86D3L, 0x3C8EA00AL, 0x384FBDBDL, 0x4C11DB70L, 0x48D0C6C7L,
            0x4593E01EL, 0x4152FDA9L, 0x5F15ADACL, 0x5BD4B01BL, 0x569796C2L, 0x52568B75L,
            0x6A1936C8L, 0x6ED82B7FL, 0x639B0DA6L, 0x675A1011L, 0x791D4014L, 0x7DDC5DA3L,
            0x709F7B7AL, 0x745E66CDL, 0x9823B6E0L, 0x9CE2AB57L, 0x91A18D8EL, 0x95609039L,
            0x8B27C03CL, 0x8FE6DD8BL, 0x82A5FB52L, 0x8664E6E5L, 0xBE2B5B58L, 0xBAEA46EFL,
            0xB7A96036L, 0xB3687D81L, 0xAD2F2D84L, 0xA9EE3033L, 0xA4AD16EAL, 0xA06C0B5DL,
            0xD4326D90L, 0xD0F37027L, 0xDDB056FEL, 0xD9714B49L, 0xC7361B4CL, 0xC3F706FBL,
            0xCEB42022L, 0xCA753D95L, 0xF23A8028L, 0xF6FB9D9FL, 0xFBB8BB46L, 0xFF79A6F1L,
            0xE13EF6F4L, 0xE5FFEB43L, 0xE8BCCD9AL, 0xEC7DD02DL, 0x34867077L, 0x30476DC0L,
            0x3D044B19L, 0x39C556AEL, 0x278206ABL, 0x23431B1CL, 0x2E003DC5L, 0x2AC12072L,
            0x128E9DCFL, 0x164F8078L, 0x1B0CA6A1L, 0x1FCDBB16L, 0x018AEB13L, 0x054BF6A4L,
            0x0808D07DL, 0x0CC9CDCAL, 0x7897AB07L, 0x7C56B6B0L, 0x71159069L, 0x75D48DDEL,
            0x6B93DDBBL, 0x6F52C06CL, 0x6211E6B5L, 0x66D0FB02L, 0x5E9F46BFL, 0x5A5E5B08L,
            0x571D7DD1L, 0x53DC6066L, 0x4D9B3063L, 0x495A2DD4L, 0x44190B0DL, 0x40D816BAL,
            0xACA5C697L, 0xA864DB20L, 0xA527FDF9L, 0xA1E6E04EL, 0xBFA1B04BL, 0xBB60ADFCL,
            0xB6238B25L, 0xB2E29692L, 0x8AAD2B2FL, 0x8E6C3698L, 0x832F1041L, 0x87EE0DF6L,
            0x99A95DF3L, 0x9D684044L, 0x902B669DL, 0x94EA7B2AL, 0xE0B41DE7L, 0xE4750050L,
            0xE9362689L, 0xEDF73B3EL, 0xF3B06B3BL, 0xF771768CL, 0xFA325055L, 0xFEF34DE2L,
            0xC6BCF05FL, 0xC27DEDE8L, 0xCF3ECB31L, 0xCBFFD686L, 0xD5B88683L, 0xD1799B34L,
            0xDC3ABDEDL, 0xD8FBA05AL, 0x690CE0EEL, 0x6DCDFD59L, 0x608EDB80L, 0x644FC637L,
            0x7A089632L, 0x7EC98B85L, 0x738AAD5CL, 0x774BB0EBL, 0x4F040D56L, 0x4BC510E1L,
            0x46863638L, 0x42472B8FL, 0x5C007B8AL, 0x58C1663DL, 0x558240E4L, 0x51435D53L,
            0x251D3B9EL, 0x21DC2629L, 0x2C9F00F0L, 0x285E1D47L, 0x36194D42L, 0x32D850F5L,
            0x3F9B762CL, 0x3B5A6B9BL, 0x0315D626L, 0x07D4CB91L, 0x0A97ED48L, 0x0E56F0FFL,
            0x1011A0FAL, 0x14D0BD4DL, 0x19939B94L, 0x1D528623L, 0xF12F560EL, 0xF5EE4BB9L,
            0xF8AD6D60L, 0xFC6C70D7L, 0xE22B20D2L, 0xE6EA3D65L, 0xEBA91BBCL, 0xEF68060BL,
            0xD727BBB6L, 0xD3E6A601L, 0xDEA580D8L, 0xDA649D6FL, 0xC423CD6AL, 0xC0E2D0DDL,
            0xCDA1F604L, 0xC960EBB3L, 0xBD3E8D7EL, 0xB9FF90C9L, 0xB4BCB610L, 0xB07DABA7L,
            0xAE3AFBA2L, 0xAAFBE615L, 0xA7B8C0CCL, 0xA379DD7BL, 0x9B3660C6L, 0x9FF77D71L,
            0x92B45BA8L, 0x9675461FL, 0x8832161AL, 0x8CF30BADL, 0x81B02D74L, 0x857130C3L,
            0x5D8A9099L, 0x594B8D2EL, 0x5408ABF7L, 0x50C9B640L, 0x4E8EE645L, 0x4A4FFBF2L,
            0x470CDD2BL, 0x43CDC09CL, 0x7B827D21L, 0x7F436096L, 0x7200464FL, 0x76C15BF8L,
            0x68860BFDL, 0x6C47164AL, 0x61043093L, 0x65C52D24L, 0x119B4BE9L, 0x155A565EL,
            0x18197087L, 0x1CD86D30L, 0x029F3D35L, 0x065E2082L, 0x0B1D065BL, 0x0FDC1BECL,
            0x3793A651L, 0x3352BBE6L, 0x3E119D3FL, 0x3AD08088L, 0x2497D08DL, 0x2056CD3AL,
            0x2D15EBE3L, 0x29D4F654L, 0xC5A92679L, 0xC1683BCEL, 0xCC2B1D17L, 0xC8EA00A0L,
            0xD6AD50A5L, 0xD26C4D12L, 0xDF2F6BCBL, 0xDBEE767CL, 0xE3A1CBC1L, 0xE760D676L,
            0xEA23F0AFL, 0xEEE2ED18L, 0xF0A5BD1DL, 0xF464A0AAL, 0xF9278673L, 0xFDE69BC4L,
            0x89B8FD09L, 0x8D79E0BEL, 0x803AC667L, 0x84FBDBD0L, 0x9ABC8BD5L, 0x9E7D9662L,
            0x933EB0BBL, 0x97FFAD0CL, 0xAFB010B1L, 0xAB710D06L, 0xA6322BDFL, 0xA2F33668L,
            0xBCB4666DL, 0xB8757BDAL, 0xB5365D03L, 0xB1F740B4L
    };

    private String errorMessage = "";
    private long length = 0;
    private int bufferSize = 8192;
    private long checksumValue = 0;

    /**
     * ClsCksum クラスの新しいインスタンスを初期化します。
     */
    public ClsCksum() {
    }

    /**
     * 直近の処理で発生したエラーメッセージを取得します。
     *
     * @return エラーメッセージ文字列。エラーがない場合は空文字列
     */
    public String getErrorMessage() {
        return errorMessage;
    }

    /**
     * 直近の処理で発生したエラーメッセージを設定します。
     *
     * @param errorMessage エラーメッセージ文字列
     */
    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage != null ? errorMessage : "";
    }

    /**
     * 最後に処理したデータのバイト長を取得します。
     *
     * @return 処理されたストリームまたはファイルのバイト数
     */
    public long getLength() {
        return length;
    }

    /**
     * 指定されたファイルパスのファイルからデフォルトのアルゴリズム (cksum) でチェックサムを取得します。
     *
     * @param filePath チェックサムを取得する対象ファイルのフルパス
     * @return 計算されたチェックサム文字列。エラーが発生した場合は空文字列
     */
    public String getChecksum(String filePath) {
        return getChecksum(filePath, "cksum");
    }

    /**
     * 指定されたファイルパスのファイルとアルゴリズムを使用してチェックサムを取得します。
     *
     * @param filePath チェックサムを取得する対象ファイルのフルパス
     * @param algorithm 使用するアルゴリズム ("ADLER32", "MD5", "SHA1", "SHA256", "SHA512", "cksum" 等)
     * @return 計算されたチェックサム文字列。エラーが発生した場合は空文字列
     */
    public String getChecksum(String filePath, String algorithm) {
        String checksum = "";
        this.errorMessage = "";
        try {
            java.nio.file.Path path = java.nio.file.Paths.get(filePath);
            try (InputStream stream = java.nio.file.Files.newInputStream(path)) {
                String algo = algorithm != null ? algorithm.toUpperCase(Locale.ROOT) : "CKSUM";
                switch (algo) {
                    case "ADLER32":
                    case "MD5":
                    case "SHA":
                    case "SHA1":
                    case "SHA-1":
                    case "SHA256":
                    case "SHA512":
                        checksum = getChecksum(stream, algo);
                        break;
                    default:
                        checksum = getChecksum(stream);
                        break;
                }
                this.length = java.nio.file.Files.size(path);
            }
        } catch (Exception ex) {
            this.errorMessage = "[ClsCksum.GetChecksum(" + filePath + "," + algorithm + ")] " + ex.getMessage();
        }
        return checksum;
    }

    /**
     * 指定されたストリームとアルゴリズムを使用してチェックサムを取得します。
     *
     * @param stream チェックサムを取得する読み取り可能な入力ストリーム
     * @param algorithm 使用するアルゴリズム ("ADLER32", "MD5", "SHA1", "SHA256", "SHA512", "cksum" 等)
     * @return 計算されたチェックサム文字列。エラーが発生した場合は空文字列
     */
    public String getChecksum(InputStream stream, String algorithm) {
        String checksum = "";
        this.length = 0;
        try {
            String algo = algorithm != null ? algorithm.toUpperCase(Locale.ROOT) : "CKSUM";
            switch (algo) {
                case "ADLER32":
                    ClsAdler32 adler32 = new ClsAdler32();
                    checksum = adler32.computeChecksum(stream);
                    this.length = adler32.getLength();
                    break;
                case "MD5":
                    checksum = computeDigestHex(stream, "MD5");
                    break;
                case "SHA":
                case "SHA1":
                case "SHA-1":
                    checksum = computeDigestHex(stream, "SHA-1");
                    break;
                case "SHA256":
                    checksum = computeDigestHex(stream, "SHA-256");
                    break;
                case "SHA512":
                    checksum = computeDigestHex(stream, "SHA-512");
                    break;
                default:
                    checksum = getChecksum(stream);
                    break;
            }
        } catch (Exception ex) {
            this.errorMessage = "[ClsCksum.GetChecksum(InputStream," + algorithm + ")] " + ex.getMessage();
        }
        return checksum;
    }

    private static final char[] HEX_CHARS = "0123456789abcdef".toCharArray();

    private String computeDigestHex(InputStream stream, String digestAlgorithm) throws Exception {
        MessageDigest md = MessageDigest.getInstance(digestAlgorithm);
        byte[] buffer = new byte[bufferSize];
        int bytesRead;
        long totalBytes = 0;
        while ((bytesRead = stream.read(buffer, 0, bufferSize)) > 0) {
            md.update(buffer, 0, bytesRead);
            totalBytes += bytesRead;
        }
        this.length = totalBytes;
        byte[] digest = md.digest();
        char[] hexChars = new char[digest.length * 2];
        for (int i = 0; i < digest.length; i++) {
            int v = digest[i] & 0xFF;
            hexChars[i * 2] = HEX_CHARS[v >>> 4];
            hexChars[i * 2 + 1] = HEX_CHARS[v & 0x0F];
        }
        return new String(hexChars);
    }

    /**
     * 指定されたストリームから POSIX cksum (CRC32 互換) アルゴリズムでチェックサムを取得します。
     *
     * @param stream チェックサムを取得する読み取り可能な入力ストリーム
     * @return 計算された POSIX cksum チェックサム数値文字列。エラーが発生した場合は空文字列
     */
    public String getChecksum(InputStream stream) {
        String checksum = "";
        this.checksumValue = 0;
        this.length = 0;
        this.errorMessage = "";

        byte[] buffer = new byte[bufferSize];
        try {
            int bytesRead;
            while ((bytesRead = stream.read(buffer, 0, bufferSize)) > 0) {
                for (int i = 0; i < bytesRead; i++) {
                    updateCrcByte(buffer[i]);
                }
            }
            long checksumLen = this.length;
            for (; checksumLen != 0; checksumLen >>>= 8) {
                int tableIndex = (int) (((this.checksumValue >>> 24) ^ checksumLen) & 0xFF);
                this.checksumValue = ((this.checksumValue << 8) & 0xFFFFFFFFL) ^ CRC_TABLE[tableIndex];
            }
            long finalValue = (~this.checksumValue) & 0xFFFFFFFFL;
            checksum = Long.toString(finalValue);
        } catch (Exception ex) {
            this.errorMessage = "[ClsCksum.GetChecksum(InputStream)] " + ex.getMessage();
        }
        return checksum;
    }

    private void updateCrcByte(byte b) {
        int tableIndex = (int) (((this.checksumValue >>> 24) ^ (b & 0xFF)) & 0xFF);
        this.checksumValue = ((this.checksumValue << 8) & 0xFFFFFFFFL) ^ CRC_TABLE[tableIndex];
        this.length++;
    }
}
