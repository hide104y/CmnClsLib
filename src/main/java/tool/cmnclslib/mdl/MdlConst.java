package tool.cmnclslib.mdl;

import java.math.BigInteger;

/**
 * システム全体で使用する共通定数定義クラスです。
 */
public final class MdlConst {

    // PATH
    /** ツール基本ディレクトリパス */
    public static final String TOOL_BASE = "C:\\Tool\\Infra";
    /** 設定ファイル基本ディレクトリパス */
    public static final String CONF_BASE = "C:\\Tool\\Infra\\conf";

    // 暗号鍵
    /** デフォルト暗号鍵の別名プレースホルダー */
    public static final String CRYPT_KEY_ALIAS_DEFAULT = "_DEFAULT_ENC_KEY_";
    /** 組み込み暗号鍵の別名プレースホルダー */
    public static final String CRYPT_KEY_ALIAS_BUILTIN = "_BUILTIN_ENC_KEY_";
    /** 環境変数暗号鍵の別名プレースホルダー */
    public static final String CRYPT_KEY_ALIAS_ENV = "_ENV_ENC_KEY_";
    /** 暗号鍵を取得するデフォルトの環境変数名 */
    public static final String CRYPT_KEY_NAME_ENV = "MY_CRYPT_KEY";

    // 暗号化・復号化デフォルト値
    /** デフォルトの暗号鍵サイズ (bit) */
    public static final int CRYPT_KEYSIZE = 128;
    /** デフォルトのブロックサイズ (bit) */
    public static final int CRYPT_BLOCKSIZE = 128;
    /** デフォルトのキー派生イテレーション回数 */
    public static final int CRYPT_ITERATIONCOUNT = 10000;
    /** デフォルトのハッシュアルゴリズム */
    public static final String CRYPT_HASHALGORITHM = "MD5";

    // 暗号鍵：OpenSSL互換確認テスト
    /** サンプル値その１ */
    public static final String CRYPT_KEY = "secret#0";
    /** サンプル値その２ */
    public static final String CRYPT_TEST_KEY = "keyw0rd2022";
    /** サンプル値その３ */
    public static final String CRYPT_TEST_PWD = "UnitTestP@ssW0rd!";
    /** 確認値その１ */
    public static final String ENC_PASS_MD5_S128_B128_C0 = "U2FsdGVkX193cHp5ZnJ4a8AIw1jtXSIhWCXfD2rt+HQKmeQL6XXI52zl5fmj5kwZ";
    /** 確認値その２ */
    public static final String ENC_PASS_SHA1_S128_B128_C10000 = "U2FsdGVkX193dXVha3JnZ/Njr4i5KskyqBZd4XdjHvAyvSIlW5td4AX9+ZBwHvKa";
    /** 確認値その３ */
    public static final String ENC_PASS_SHA256_S128_B128_C10000 = "U2FsdGVkX19ubmdhbnl6awQ4f6fnmYjrunTPph6hEmoQSWZSZFBK5RbmYBNots2g";
    /** 確認値その４ */
    public static final String ENC_PASS_SHA256_S256_B128_C10000 = "U2FsdGVkX19sYmRhaGltahwygQaa5JDqmSwyXLqvjID827P37rX55JT/TPVTw6t5";
    /** 確認値その５ */
    public static final String ENC_PASS_SHA512_S128_B128_C10000 = "U2FsdGVkX19saGFmcHlhax2LEttLP2zntM1f1DxUFlGfYHUViUn4iHucyRWf4HTn";
    /** 確認値その６ */
    public static final String ENC_PASS_SHA512_S256_B128_C10000 = "U2FsdGVkX192bWZzcWhtZCEOP5ZCh1jVPVx4RfzoFGfhH0XWossBjAVptz226Sfg";

    // エラー番号 / ログレベル
    /** デバッグログレベル (-1) */
    public static final int LVL_DEBUG = -1;
    /** 情報ログレベル (0) */
    public static final int LVL_I = 0;
    /** 警告ログレベル (10) */
    public static final int LVL_W = 10;
    /** エラーログレベル (20) */
    public static final int LVL_E = 20;
    /** 致命的エラーログレベル (30) */
    public static final int LVL_F = 30;
    /** ログ出力なし (1000) */
    public static final int LVL_NONE = 1000;

    // 型の範囲
    /** int の最小値 */
    public static final int INT_MIN = Integer.MIN_VALUE;
    /** int の最大値 */
    public static final int INT_MAX = Integer.MAX_VALUE;
    /** 32bit 符号なし整数の最大値 */
    public static final long UINT_MAX = 4294967295L;
    /** long の最小値 */
    public static final long LNG_MIN = Long.MIN_VALUE;
    /** long の最大値 */
    public static final long LNG_MAX = Long.MAX_VALUE;
    /** 64bit 符号なし整数の最大値 */
    public static final BigInteger ULNG_MAX = new BigInteger("18446744073709551615");
    /** double の最小値表現 */
    public static final double DBL_MIN = -1.7e308;
    /** double の最大値表現 */
    public static final double DBL_MAX = 1.7e308;

    // NULL値表現
    /** int 型の null 代替値 */
    public static final int INT_NULL = Integer.MIN_VALUE;
    /** long 型の null 代替値 */
    public static final long LNG_NULL = Long.MIN_VALUE;
    /** double 型の null 代替値 */
    public static final double DBL_NULL = -1.7e308;

    // タイプ
    /** 対象タイプ: すべて */
    public static final int INT_TYPE_ALL = 0;
    /** 対象タイプ: ディレクトリ */
    public static final int INT_TYPE_DIRECTORY = 1;
    /** 対象タイプ: ファイル */
    public static final int INT_TYPE_FILE = 2;

    private MdlConst() {
        // インスタンス化防止
    }
}
