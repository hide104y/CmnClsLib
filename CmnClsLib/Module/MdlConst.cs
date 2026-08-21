using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmnClsLib.Module
{
    public class MdlConst
    {
        // PATH
        public const String TOOL_BASE = @"C:\Tool\Infra";
        public const String CONF_BASE = @"C:\Tool\Infra\conf";

        // 暗号鍵
        public const string CRYPT_KEY_ALIAS_DEFAULT = "_DEFAULT_ENC_KEY_";
        public const string CRYPT_KEY_ALIAS_BUILTIN = "_BUILTIN_ENC_KEY_";
        public const string CRYPT_KEY_ALIAS_ENV = "_ENV_ENC_KEY_";
        public const string CRYPT_KEY_NAME_ENV = "MY_CRYPT_KEY";

        // 暗号化・復号化デフォルト値
        public const int CRYPT_KEYSIZE = 128;
        public const int CRYPT_BLOCKSIZE = 128;
        public const int CRYPT_ITERATIONCOUNT = 10000;
        public const string CRYPT_HASHALGORITHM = "MD5";

        // 暗号鍵：OpenSSL互換確認テスト
        public const string CRYPT_KEY = @"secret#0";
        public const string CRYPT_TEST_KEY = @"keyw0rd2022";
        public const string CRYPT_TEST_PWD = @"UnitTestP@ssW0rd!";
        public const string ENC_PASS_MD5_S128_B128_C0 = @"U2FsdGVkX193cHp5ZnJ4a8AIw1jtXSIhWCXfD2rt+HQKmeQL6XXI52zl5fmj5kwZ";
        public const string ENC_PASS_SHA1_S128_B128_C10000 = @"U2FsdGVkX193dXVha3JnZ/Njr4i5KskyqBZd4XdjHvAyvSIlW5td4AX9+ZBwHvKa";
        public const string ENC_PASS_SHA256_S128_B128_C10000 = @"U2FsdGVkX19ubmdhbnl6awQ4f6fnmYjrunTPph6hEmoQSWZSZFBK5RbmYBNots2g";
        public const string ENC_PASS_SHA256_S256_B128_C10000 = @"U2FsdGVkX19sYmRhaGltahwygQaa5JDqmSwyXLqvjID827P37rX55JT/TPVTw6t5";
        public const string ENC_PASS_SHA512_S128_B128_C10000 = @"U2FsdGVkX19saGFmcHlhax2LEttLP2zntM1f1DxUFlGfYHUViUn4iHucyRWf4HTn";
        public const string ENC_PASS_SHA512_S256_B128_C10000 = @"U2FsdGVkX192bWZzcWhtZCEOP5ZCh1jVPVx4RfzoFGfhH0XWossBjAVptz226Sfg";

        // エラー番号
        public const int LVL_DEBUG = -1;        // DEBUG
        public const int LVL_I = 0;             // INFO
        public const int LVL_W = 10;            // WARN
        public const int LVL_E = 20;            // ERROR
        public const int LVL_F = 30;            // FATAL
        public const int LVL_NONE = 1000;       // NONE

        // 型の範囲
        public const int INT_MIN = -2147483648;
        public const int INT_MAX = 2147483647;
        public const uint UINT_MAX = 4294967295;
        public const long LNG_MIN = -9223372036854775808;
        public const long LNG_MAX = 9223372036854775807;
        public const ulong ULNG_MAX = 18446744073709551615;
        public const Double DBL_MIN = -1.7e308;
        public const Double DBL_MAX = 1.7e308;

        // NULL値
        public const int INT_NULL = -2147483648;
        public const long LNG_NULL = -9223372036854775808;
        public const Double DBL_NULL = -1.7e308;

        // タイプ
        public const int INT_TYPE_ALL = 0;
        public const int INT_TYPE_DIRECTORY = 1;
        public const int INT_TYPE_FILE = 2;

    }
}
