using System.Text.RegularExpressions;
using CmnClsLib.Module;
using CmnClsLib.Interface;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified
// 2026/08/11 Gemini 3.6 Flash (High) Review & Modified 2回目

namespace CmnClsLib.Class
{
    public class ClsCmmnArgs
    {
        private ICmnLogger _logger;                                         // ログ出力
        private ClsJp1Job _jp1;                                             // JP1ユーティリティ
        private Dictionary<string, string> _namedArgs = [];                 // 引数
        private string _exePath = "";                                       // このEXEのフルパス
        private string _exeDir = "";                                        // このEXEのあるディレクトリパス
        private string _exeBaseName = "";                                   // このEXEのBansename
        private string _machineName = "";                                   // コンピュータ名
        private string _argDefFilePath = "";                                // 引数定義INIファイル
        private string _envPrefix = @"ENV\.";                               // 文字列置換環境変数検索プレフィックス
        private int _verbose = 0;                                           // 冗長レベル
        private int _pid = 0;                                               // このプロセスのPID
        private bool _isUsage = false;                                      // Usage出力フラグ
        private bool _isStackTrace = false;                                 // スタックトレースフラグ
        private bool _isAjsJob = false;                                     // AJSJOBNAMEフラグ
        // 認証
        private string _authDefFilePath = "";                               // 認証情報設定ファイル
        private string _domainName = "";                                    // ドメイン名
        private string _username = "";                                      // ユーザ名
        private string _usernameWithoutDomain = "";                         // ドメイン無しユーザ名
        private string _password = "";                                      // パスワード
        private string _defaultEncKey = MdlConst.CRYPT_KEY_ALIAS_DEFAULT;   // 暗号鍵
        private string _encKey = "";                                        // 暗号鍵
        private string _encKeyEnvName = "";                                 // 暗号鍵格納環境変数名
        private string _argKeyOfUserConf = "def";                           // ユーザー定義ファイル引数キー
        private string _hashAlgorithm = ClsCrypt.DEFAULT_HASH_ALGORITHM;    // ハッシュアルゴリズム
        private int _keySize = ClsCrypt.DEFAULT_KEY_SIZE;                   // 鍵長
        private int _blockSize = ClsCrypt.DEFAULT_BLOCK_SIZE;               // ブロック長
        private int _iterationCount = ClsCrypt.DEFAULT_ITERATION_COUNT;     // 繰返回数
        private bool _isSwitchUser = false;                                 // 偽装認証フラグ
        private bool _isLogon = false;                                      // 偽装認証フラグその２
        private bool _isLogonAlwaysOk = false;                              // 偽装認証エラー無視フラグ
        private bool _isDecodePasswd = false;                               // パスワード複合化フラグ
        private bool _isDecodeKey = false;                                  // 暗号鍵複合化フラグ
        private bool _isDebugAuth = false;                                  // 認証メソッドデバッグフラグ
        private bool _isDefaultEncKey = false;                              // 暗号鍵フラグ
        // NET USE
        private string _netSharePath = "";                                  // ネットワーク共有ディレクトリパス
        private string _driveName = "";                                     // ドライブ名
        private bool _isMount = false;                                      // NET USE接続フラグ
        private bool _isUmount = false;                                     // NET USE切断フラグ
        private List<int> _netUseOkErrNoList = [];                          // Net Useで正常と見なすエラー番号リスト
        // ETC
        private string _host = "";                                          // ホスト名
        private string _errorMessage = "";                                  // エラーメッセージ
        private string _envIdKey = "ENV_ID";                                // 環境変数：環境種別キー
        private string _envId = "";                                         // 環境種別値
        private string _replaceEnvIdKey = "__ENV_ID__";                     // 環境種別置換対象名
        private bool _isForce = false;                                      // 強制実行フラグ
        private bool _isDiff = false;                                       // 更新のみ表示フラグ
        private bool _isGetEnvId = true;                                    // 環境種別環境変数取得フラグ
        private int _diffLevel = 0;                                         // 更新のみ表示フラグレベル
        private int _timeout = 86400;                                       // タイムアウト（秒）
        // リトライ
        private int _retryMax = 0;                                          // リトライ回数
        private int _retrySleep = 5;                                        // リトライ待ち（秒）
        // 文字列分割パターン
        private string _splitPattern = @"[,\/|]";                           // 文字列分割デリミタパターン
        private string _keyValDelimiter = @"[:]";                           // KEY:VALデリミタ文字列
        // フィルター
        private bool _isRegIncBasename = true;                              // 絞込時basenameフラグ
        private bool _isRegExcBasename = true;                              // 除外時basenameフラグ
        private bool _isIncHitRecursive = true;                             // 絞込結果を階層下に適用フラグ
        private bool _isExcHitRecursive = true;                             // 除外結果を階層下に適用フラグ
        private bool _isDirFilterOr = false;                                // 絞込or除外フラグ
        // 引数名リスト：認証
        private List<string> _keyNameOfUsernameList = [];                   // ユーザ名
        private List<string> _keyNameOfPasswordList = [];                   // パスワード
        private List<string> _keyNameOfEncPassList = [];                    // 暗号化パスワード
        private List<string> _keyNameOfEncKeyList = [];                     // 暗号鍵
        private List<string> _keyNameOfEncEncKeyList = [];                  // 暗号化暗号鍵
        private List<string> _keyNameOfEncKeySizeList = [];                 // 鍵長
        // 引数名リスト：フィルター
        private List<string> _incFilesList = [];                            // ファイル絞り込み正規表現ルールリスト
        private List<string> _excFilesList = [];                            // ファイル除外正規表現ルールリスト
        private List<string> _incDirsList = [];                             // ディレクトリ絞り込み正規表現ルールリスト
        private List<string> _excDirsList = [];                             // ディレクトリ除外正規表現ルールリスト
        // 引数名リスト：置換
        private Dictionary<string, string> _replaceDic = [];                // 置換リスト
        private Dictionary<string, string> _shortDic = [];                  // 短縮名リスト
        private Dictionary<string, string> _authDefDic = [];                // 認証ファイルの内容

        /// <summary>
        /// <see cref="ClsCmmnArgs"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="logger">ログ出力用のロガーインスタンス (<see cref="ICmnLogger"/>)</param>
        /// <example>
        /// <code>
        /// ICmnLogger logger = new ClsLogger();
        /// ClsCmmnArgs cmmnArgs = new ClsCmmnArgs(logger);
        /// </code>
        /// </example>
        public ClsCmmnArgs(ICmnLogger logger)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            _logger = logger;
            _jp1 = new(_logger);
            InitializeLists();
        }

        public Dictionary<string, string> NamedArgs { get => _namedArgs; set => _namedArgs = value; }
        public Dictionary<string, string> DicAuthDef { get => _authDefDic; set => _authDefDic = value; }
        public ClsJp1Job Jp1 { get => _jp1; set => _jp1 = value; }
        public string ExeBaseName { get => _exeBaseName; set => _exeBaseName = value; }
        public string ExePath { get => _exePath; set => _exePath = value; }
        public string ExeDir { get => _exeDir; set => _exeDir = value; }
        public string MachineName { get => _machineName; set => _machineName = value; }
        public int Pid { get => _pid; set => _pid = value; }
        public bool IsUsage { get => _isUsage; set => _isUsage = value; }
        public int Verbose { get => _verbose; set => _verbose = value; }
        public bool IsStackTrace { get => _isStackTrace; set => _isStackTrace = value; }
        public bool IsAjsJob { get => _isAjsJob; set => _isAjsJob = value; }
        public string AuthDefFilePath { get => _authDefFilePath; set => _authDefFilePath = value; }
        public string ArgKeyOfUserConf { get => _argKeyOfUserConf; set => _argKeyOfUserConf = value; }
        public string DomainName { get => _domainName; set => _domainName = value; }
        public string Username { get => _username; set => _username = value; }
        public string UsernameWithoutDomain { get => _usernameWithoutDomain; set => _usernameWithoutDomain = value; }
        public string Password { get => _password; set => _password = value; }
        public string EncKey { get => _encKey; set => _encKey = value; }
        public string EncKeyEnvName { get => _encKeyEnvName; set => _encKeyEnvName = value; }
        public string DefaultEncKey { get => _defaultEncKey; set => _defaultEncKey = value; }
        public int KeySize { get => _keySize; set => _keySize = value; }
        public int BlockSize { get => _blockSize; set => _blockSize = value; }
        public string HashAlgorithm { get => _hashAlgorithm; set => _hashAlgorithm = value.ToUpperInvariant(); }
        public int IterationCount { get => _iterationCount; set => _iterationCount = value; }
        public bool IsSwitchUser { get => _isSwitchUser; set => _isSwitchUser = value; }
        public bool IsLogon { get => _isLogon; set => _isLogon = value; }
        public bool IsLogonAlwaysOk { get => _isLogonAlwaysOk; set => _isLogonAlwaysOk = value; }
        public bool IsDecodePasswd { get => _isDecodePasswd; set => _isDecodePasswd = value; }
        public bool IsDecodeKey { get => _isDecodeKey; set => _isDecodeKey = value; }
        public bool IsDefaultEncKey { get => _isDefaultEncKey; set => _isDefaultEncKey = value; }
        public string NetSharePath { get => _netSharePath; set => _netSharePath = value; }
        public string DriveName { get => _driveName; set => _driveName = value; }
        public bool IsMount { get => _isMount; set => _isMount = value; }
        public bool IsUmount { get => _isUmount; set => _isUmount = value; }
        public List<int> NetUseOkErrNoList { get => _netUseOkErrNoList; set => _netUseOkErrNoList = value; }
        public string Host { get => _host; set => _host = value; }
        public string EnvIdKey { get => _envIdKey; set => _envIdKey = value; }
        public string EnvId { get => _envId; set => _envId = value; }
        public string RunEnvKey { get => _replaceEnvIdKey; set => _replaceEnvIdKey = value; }
        public string RunEnv { get => _envId; set => _envId = value; }
        public string ReplaceEnvIdKey { get => _replaceEnvIdKey; set => _replaceEnvIdKey = value; }
        public string ErrorMessage { get => _errorMessage; set => _errorMessage = value; }
        public bool IsForce { get => _isForce; set => _isForce = value; }
        public bool IsDiff { get => _isDiff; set => _isDiff = value; }
        public bool IsGetEnvId { get => _isGetEnvId; set => _isGetEnvId = value; }
        public bool IsDebugAuth { get => _isDebugAuth; set => _isDebugAuth = value; }
        public int DiffLevel { get => _diffLevel; set => _diffLevel = value; }
        public int Timeout { get => _timeout; set => _timeout = value; }
        public int RetryMax { get => _retryMax; set => _retryMax = value; }
        public int RetrySleep { get => _retrySleep; set => _retrySleep = value; }
        public string SplitPattern { get => _splitPattern; set => _splitPattern = value; }
        public string KeyValDelimiter { get => _keyValDelimiter; set => _keyValDelimiter = value; }
        public bool IsRegIncBasename { get => _isRegIncBasename; set => _isRegIncBasename = value; }
        public bool IsRegExcBasename { get => _isRegExcBasename; set => _isRegExcBasename = value; }
        public bool IsIncHitRecursive { get => _isIncHitRecursive; set => _isIncHitRecursive = value; }
        public bool IsExcHitRecursive { get => _isExcHitRecursive; set => _isExcHitRecursive = value; }
        public bool IsDirFilterOr { get => _isDirFilterOr; set => _isDirFilterOr = value; }
        public List<string> KeyNameOfUsernameList { get => _keyNameOfUsernameList; set => _keyNameOfUsernameList = value; }
        public List<string> KeyNameOfPasswordList { get => _keyNameOfPasswordList; set => _keyNameOfPasswordList = value; }
        public List<string> KeyNameOfEncPassList { get => _keyNameOfEncPassList; set => _keyNameOfEncPassList = value; }
        public List<string> KeyNameOfEncKeyList { get => _keyNameOfEncKeyList; set => _keyNameOfEncKeyList = value; }
        public List<string> KeyNameOfEncEncKeyList { get => _keyNameOfEncEncKeyList; set => _keyNameOfEncEncKeyList = value; }
        public List<string> KeyNameOfEncKeySizeList { get => _keyNameOfEncKeySizeList; set => _keyNameOfEncKeySizeList = value; }
        public List<string> IncFilesList { get => _incFilesList; set => _incFilesList = value; }
        public List<string> ExcFilesList { get => _excFilesList; set => _excFilesList = value; }
        public List<string> IncDirsList { get => _incDirsList; set => _incDirsList = value; }
        public List<string> ExcDirsList { get => _excDirsList; set => _excDirsList = value; }
        public Dictionary<string, string> ReplaceDic { get => _replaceDic; set => _replaceDic = value; }
        public Dictionary<string, string> ShortDic { get => _shortDic; set => _shortDic = value; }

        /// <summary>
        /// 各種デフォルト引数名キーリストの初期化および環境情報の取得を行います。
        /// </summary>
        /// <example>
        /// <code>
        /// cmmnArgs.InitializeLists();
        /// </code>
        /// </example>
        public void InitializeLists()
        {
            _keyNameOfUsernameList.AddRange(["username", "user", "u"]);
            _keyNameOfPasswordList.AddRange(["password", "pass", "p"]);
            _keyNameOfEncPassList.AddRange(["encodedpassword", "encpass", "ep"]);
            _keyNameOfEncKeyList.AddRange(["enckey", "key", "k"]);
            _keyNameOfEncEncKeyList.AddRange(["encenckey", "ek"]);
            _keyNameOfEncKeySizeList.AddRange(["keysize", "size", "s"]);
            
            try { _machineName = Environment.MachineName; } catch { }
        }

        /// <summary>
        /// 現在のプロセスのモジュール情報（EXEパス、ディレクトリ、BaseName、PID）を取得します。
        /// </summary>
        /// <returns>モジュール情報の取得に成功した場合は true、失敗した場合は false</returns>
        /// <example>
        /// <code>
        /// bool success = cmmnArgs.GetModuleInfo();
        /// </code>
        /// </example>
        public bool GetModuleInfo()
        {
            try
            {
                using var process = System.Diagnostics.Process.GetCurrentProcess();
                string exePath = process.MainModule?.FileName ?? "";
                return GetModuleInfo(exePath);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 指定された実行ファイルパスからモジュール情報（EXEパス、ディレクトリ、BaseName、PID）を取得します。
        /// </summary>
        /// <param name="exePath">実行ファイルのフルパス</param>
        /// <returns>モジュール情報の取得に成功した場合は true、失敗した場合は false</returns>
        /// <example>
        /// <code>
        /// bool success = cmmnArgs.GetModuleInfo(@"C:\App\myprogram.exe");
        /// </code>
        /// </example>
        public bool GetModuleInfo(string exePath)
        {
            try
            {
                if (!MdlFile.PathExists(exePath))
                {
                    using var process = System.Diagnostics.Process.GetCurrentProcess();
                    exePath = process.MainModule?.FileName ?? "";
                }
                _exeDir = System.IO.Path.GetDirectoryName(exePath) ?? "";
                _exeBaseName = System.IO.Path.GetFileNameWithoutExtension(exePath);
                _pid = Environment.ProcessId;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// コマンドライン引数辞書 (<see cref="NamedArgs"/>) から一般的な共通引数（ヘルプ、ログ設定、環境変数、文字列置換パラメータ等）を解析して保持します。
        /// </summary>
        /// <returns>共通引数の取得・解析が成功した場合は true、それ以外は false</returns>
        /// <example>
        /// <code>
        /// cmmnArgs.NamedArgs = new Dictionary&lt;string, string&gt; { { "v", "3" }, { "force", "" } };
        /// bool ok = cmmnArgs.GetCommonArgs();
        /// </code>
        /// </example>
        public bool GetCommonArgs()
        {
            const string STR_MY_NAME = "[ClsCmmnArgs.GetCommonArgs()]";
            bool isOk = true;
            string tempStr = "";

            // -----------------------------------------------------------------
            // CmmnParams Option ：
            // -----------------------------------------------------------------
            // -arg-def path       ：引数定義INIファイルパス
            if (_namedArgs.TryGetValue("arg-def", out string? argDefVal) && !string.IsNullOrEmpty(argDefVal))
            {
                _argDefFilePath = argDefVal;
                try
                {
                    var dicNamedArg = MdlFile.ReadFileToDictionary(_argDefFilePath);
                    if (dicNamedArg.Count > 0)
                    {
                        foreach (var (key, value) in dicNamedArg)
                        {
                            _namedArgs.TryAdd(key, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLine(MdlConst.LVL_E, $"{STR_MY_NAME}[-arg-def {_argDefFilePath}] EXCEPTION : {ex.Message}");
                }
            }

            // -h|-help|-?         ：Usage表示
            // -h hostname         ：ホスト名
            if (_namedArgs.TryGetValue("h", out string? hVal))
            {
                if (!string.IsNullOrEmpty(hVal))
                {
                    _host = hVal.Trim();
                }
                else
                {
                    _isUsage = true;
                }
            }

            if (_namedArgs.ContainsKey("help") || _namedArgs.ContainsKey("?"))
            {
                _isUsage = true;
            }

            // -force              ：強制実行フラグ
            foreach (string key in new string[] { "force" })
            {
                if (MdlArg.ContainsKey(_namedArgs, key))
                {
                    _isForce = true;
                    break;
                }
            }

            // -----------------------------------------------------------------
            // CmmnParams Output Option：
            // -----------------------------------------------------------------
            // -v |-vv|-brief num  ：冗長表示
            string[] vKeys = ["v", "vv", "vvv", "vvvv", "vvvvv", "vvvvvv", "vvvvvvv", "vvvvvvvv", "vvvvvvvvv", "vvvvvvvvvv", "vvvvvvvvvvv", "vvvvvvvvvvvv"];
            foreach (string key in vKeys)
            {
                if (_namedArgs.TryGetValue(key, out string? vVal))
                {
                    _verbose = key.Length;
                    if (!string.IsNullOrEmpty(vVal))
                    {
                        int tempInt = MdlUtil.ParseInt(vVal, MdlConst.INT_NULL);
                        if (tempInt != MdlConst.INT_NULL) _verbose = tempInt;
                        break;
                    }
                }
            }
            if (_namedArgs.TryGetValue("brief", out string? briefVal))
            {
                _verbose = -1;
                if (!string.IsNullOrEmpty(briefVal))
                {
                    int tempInt = MdlUtil.ParseInt(briefVal, MdlConst.INT_NULL);
                    if (tempInt != MdlConst.INT_NULL) _verbose = -1 * tempInt;
                }
            }

            // -diff               ：差分表示フラグ
            if (_namedArgs.TryGetValue("diff", out string? diffVal))
            {
                _isDiff = true;
                if (!string.IsNullOrEmpty(diffVal))
                {
                    int tempInt = MdlUtil.ParseInt(diffVal, MdlConst.INT_NULL);
                    if (tempInt != MdlConst.INT_NULL) _diffLevel = tempInt;
                }
            }

            // コンソール出力禁止フラグ
            if (_namedArgs.TryGetValue("console", out string? consoleVal) && !string.IsNullOrEmpty(consoleVal))
            {
                var (isConsole, isStdout, isStderr) = consoleVal.ToLowerInvariant() switch
                {
                    "off" => ("false", "false", "false"),
                    "stdout" => ("true", "true", "false"),
                    "stderr" => ("true", "false", "true"),
                    _ => (null, null, null)
                };
                if (isConsole != null)
                {
                    _logger.SetValueByKey(ClsLogger.IS_CONSOLE, isConsole);
                    _logger.SetValueByKey(ClsLogger.IS_STDOUT, isStdout!);
                    _logger.SetValueByKey(ClsLogger.IS_STDERR, isStderr!);
                }
            }

            // -stacktrace         ：例外時スタックトレース表示
            if (_namedArgs.ContainsKey("stacktrace"))
            {
                _isStackTrace = true;
            }

            // -stdenc encode      ：標準出力エンコード
            if (_namedArgs.TryGetValue("stdenc", out string? stdEncVal) && !string.IsNullOrEmpty(stdEncVal))
            {
                _logger.SetValueByKey(ClsLogger.IS_CONSOLE_ENCODING, "true");
                _logger.SetValueByKey(ClsLogger.CONSOLE_ENCODING, stdEncVal.Trim());
            }

            // -----------------------------------------------------------------
            // CmmnParams Env Option：
            // -----------------------------------------------------------------
            // -env-enckey name    ：暗号鍵格納環境変数名
            if (_namedArgs.TryGetValue("env-enckey", out string? envEncKeyVal) && !string.IsNullOrEmpty(envEncKeyVal))
            {
                _encKeyEnvName = envEncKeyVal.Trim();
            }

            // -----------------------------------------------------------------
            // CmmnParams Job Option：
            // -----------------------------------------------------------------
            // 環境変数が存在する場合
            if (_jp1.IsAjsJob)
            {
                _logger.SetValueByKey(ClsLogger.IS_STDERR, "true");
            }

            // -ajsjobname name    ：AJSJOBNAME
            if (_namedArgs.TryGetValue("ajsjobname", out string? ajsJobVal) && !string.IsNullOrEmpty(ajsJobVal))
            {
                _jp1.SetEnvironmentVariable(ajsJobVal);
            }

            // -nojp1              ：AJSJOBNAME参照フラグ
            if (_namedArgs.ContainsKey("nojp1"))
            {
                _jp1.IsAjsJob = false;
            }

            // フラグコピー
            _isAjsJob = _jp1.IsAjsJob;

            // -envajs str         ：AJSJOBNAME検索プレフィックス
            if (_namedArgs.TryGetValue("envajs", out string? envAjsVal) && !string.IsNullOrEmpty(envAjsVal))
            {
                _jp1.Prefix = envAjsVal;
            }

            // -envvar str         ：環境変数検索プレフィックス
            if (_namedArgs.TryGetValue("envvar", out string? envVarVal) && !string.IsNullOrEmpty(envVarVal))
            {
                _envPrefix = envVarVal;
            }

            // -envenvid str       ：環境種別キー環境変数名
            if (_namedArgs.TryGetValue("envenvid", out string? envEnvIdVal) && !string.IsNullOrEmpty(envEnvIdVal))
            {
                _envIdKey = envEnvIdVal;
            }
            if (_isGetEnvId) _envId = Environment.GetEnvironmentVariable(_envIdKey) ?? "";

            // -----------------------------------------------------------------
            // CmmnParams Replace Option：
            // -----------------------------------------------------------------
            // -splitby pattern    ：文字列分割デリミタパターン
            if (_namedArgs.TryGetValue("splitby", out string? splitByVal) && !string.IsNullOrEmpty(splitByVal))
            {
                _splitPattern = splitByVal;
            }

            // -split-kv-by pattern：key[分割デリミタパターン]Val
            if (_namedArgs.TryGetValue("split-kv-by", out string? splitKvVal) && !string.IsNullOrEmpty(splitKvVal))
            {
                _keyValDelimiter = splitKvVal;
            }

            // -replace a:b        ：文字列置換CSVリスト
            if (_namedArgs.TryGetValue("replace", out string? replaceVal) && !string.IsNullOrEmpty(replaceVal))
            {
                foreach (string pair in MdlUtil.ParseCsvToList(null, replaceVal, _splitPattern, _verbose, true))
                {
                    string[] pairParts = Regex.Split(pair, _keyValDelimiter);
                    if (pairParts.Length > 1)
                    {
                        string replaceTo = pairParts[1];
                        if (_isAjsJob && Regex.IsMatch(replaceTo, @"^" + _jp1.Prefix))
                        {
                            replaceTo = _jp1.ConvertStringFromEnvironment(replaceTo);
                        }
                        string envName = MdlUtil.GetRegexTarget(replaceTo, @"^" + _envPrefix + @"(?<TARGET>.+)$");
                        if (!string.IsNullOrEmpty(envName))
                        {
                            string strEnvVal = Environment.GetEnvironmentVariable(envName) ?? "";
                            if (!string.IsNullOrEmpty(envName))
                            {
                                replaceTo = strEnvVal;
                            }
                        }
                        if (_shortDic.Count > 0)
                        {
                            foreach (var (key, val) in _shortDic)
                            {
                                replaceTo = Regex.Replace(replaceTo ?? "", @"^" + key + @"$", val, RegexOptions.IgnoreCase);
                            }
                        }
                        _replaceDic[pairParts[0]] = replaceTo ?? "";
                    }
                }
            }

            // -reservereplace     ：文字列予約語再値置換
            if (_namedArgs.ContainsKey("reservereplace"))
            {
                _shortDic["prod"] = "production";
                _shortDic["stg"] = "staging";
                _shortDic["dev"] = "development";
            }

            // -morereplace b:c    ：文字列再値置換CSVリスト
            if (_namedArgs.TryGetValue("morereplace", out string? moreReplaceVal) && !string.IsNullOrEmpty(moreReplaceVal))
            {
                foreach (string pair in MdlUtil.ParseCsvToList(null, moreReplaceVal, _splitPattern, _verbose, true))
                {
                    string[] pairParts = pair.Split(':');
                    if (pairParts.Length > 1)
                    {
                        _shortDic[pairParts[0]] = pairParts[1];
                    }
                }
            }

            // 環境識別子
            if (!string.IsNullOrEmpty(_envId))
            {
                if (!_replaceDic.ContainsKey(_replaceEnvIdKey)) _replaceDic[_replaceEnvIdKey] = _envId;
            }

            // ホスト名の置換
            if (!string.IsNullOrEmpty(_host))
            {
                _host = ReplaceByDictionary(_host);
            }


            // -----------------------------------------------------------------
            // CmmnParams Log Option：
            // -----------------------------------------------------------------
            // -ldir path          ：ログ出力先ディレクトリパス（日付付ファイル名で出力）
            foreach (string key in new string[] { "ldir", "ldir-n" })
            {
                if (MdlArg.ContainsKey(_namedArgs, key))
                {
                    tempStr = "";
                    switch (key)
                    {
                        // 追加
                        case "ldir":
                            tempStr = GetPathParam("ldir", MdlFile.PATH_IS_DIRECTORY, true);
                            break;
                        // 新規作成
                        case "ldir-n":
                            _logger.SetValueByKey(ClsLogger.IS_APPEND, "false");
                            tempStr = GetPathParam("ldir-n", MdlFile.PATH_IS_DIRECTORY, true);
                            break;
                    }
                    if (!String.IsNullOrEmpty(tempStr))
                    {
                        _logger.SetValueByKey(ClsLogger.IS_FILE, "true");
                        _logger.SetValueByKey(ClsLogger.DIR, tempStr);
                        _logger.SetValueByKey(ClsLogger.PATH, tempStr + "\\" + _exeBaseName + "." + MdlDate.GetFormattedDate("yyyyMMdd.HHmmss") + "." + _pid.ToString() + ".log");
                        break;
                    }
                }
            }

            // -log  path          ：ログ出力ファイルパス（-ldirより優先）
            foreach (string key in new string[] { "log", "log-n" })
            {
                if (MdlArg.ContainsKey(_namedArgs, key))
                {
                    tempStr = "";
                    switch (key)
                    {
                        case "log":
                            tempStr = GetPathParam("log", MdlFile.PATH_IS_FILE, true);
                            break;
                        case "log-n":
                            _logger.SetValueByKey(ClsLogger.IS_APPEND, "false");
                            tempStr = GetPathParam("log-n", MdlFile.PATH_IS_FILE, true);
                            break;
                    }
                    if (!String.IsNullOrEmpty(tempStr))
                    {
                        _logger.SetValueByKey(ClsLogger.DIR, "" + MdlFile.GetDirectoryPath(tempStr));
                        if (MdlFile.CreateDirectory(_logger.GetValueByKey(ClsLogger.DIR, "")) < MdlFile.OK_MKDIR_HANTEI)
                        {
                            _logger.SetValueByKey(ClsLogger.IS_FILE, "true");
                            if (_verbose > 4) WriteLine(MdlConst.LVL_DEBUG, STR_MY_NAME + " -log : " + tempStr);
                            _logger.SetValueByKey(ClsLogger.PATH, MdlDate.ReplaceStringWithDateTime(tempStr.Replace(@"%%", @"%")));
                            break;
                        }
                        else
                        {
                            isOk = false;
                        }
                    }
                }
            }

            // -logenc encode      ：ログファイルエンコード
            foreach (string key in new string[] { "logenc" })
            {
                if (MdlArg.ContainsKey(_namedArgs, key))
                {
                    tempStr = MdlArg.GetValue(_namedArgs, key);
                    if (!String.IsNullOrEmpty(tempStr))
                    {
                        // _logEnc = tempStr.Trim();
                        _logger.SetValueByKey(ClsLogger.FILE_ENCODING, tempStr);
                        break;
                    }
                }
            }

            if (_logger.GetValueByKey(ClsLogger.IS_FILE, false))
            {
                if (_verbose > 4)
                {
                    WriteLine(MdlConst.LVL_DEBUG, STR_MY_NAME + " LogDir : " + _logger.GetValueByKey(ClsLogger.DIR, ""));
                    WriteLine(MdlConst.LVL_DEBUG, STR_MY_NAME + " Path : " + _logger.GetValueByKey(ClsLogger.PATH, ""));
                }
            }

            // -----------------------------------------------------------------
            // CmmnParams Command Option：
            // -----------------------------------------------------------------
            // -retry num          ：リトライ回数
            if (_namedArgs.TryGetValue("retry", out string? retryVal) && !string.IsNullOrEmpty(retryVal))
            {
                int tempInt = MdlUtil.ParseInt(retryVal, MdlConst.INT_NULL);
                if (tempInt != MdlConst.INT_NULL) _retryMax = tempInt;
            }
            if (_retryMax < 0) _retryMax = 0;

            // -sleep sec          ：リトライ間隔（秒）
            if (_namedArgs.TryGetValue("sleep", out string? sleepVal) && !string.IsNullOrEmpty(sleepVal))
            {
                int tempInt = MdlUtil.ParseInt(sleepVal, MdlConst.INT_NULL);
                if (tempInt != MdlConst.INT_NULL) _retrySleep = tempInt;
            }
            if (_retrySleep < 1) _retrySleep = 1;

            // -timeout sec        ：タイムアウト（秒）
            if (_namedArgs.TryGetValue("timeout", out string? timeoutVal) && !string.IsNullOrEmpty(timeoutVal))
            {
                int tempInt = MdlUtil.ParseInt(timeoutVal, MdlConst.INT_NULL);
                if (tempInt != MdlConst.INT_NULL) _timeout = tempInt;
            }
            if (_timeout < 1) _timeout = 86400;

            // -----------------------------------------------------------------
            // CmmnParams Debug Option：
            // -----------------------------------------------------------------
            // -dumpargs           ：引数の表示
            if (_namedArgs.ContainsKey("dumpargs"))
            {
                foreach (var (key, value) in _namedArgs)
                {
                    WriteLine(MdlConst.LVL_DEBUG, $"ARG : -{key} {value}");
                }
            }

            // -dumpreplace        ：置換リストの表示
            if (_namedArgs.ContainsKey("dumpreplace"))
            {
                foreach (var (key, value) in _replaceDic)
                {
                    WriteLine(MdlConst.LVL_DEBUG, $"[REPLACE] KEY = {key} / VAL = {value}");
                }
                foreach (var (key, value) in _shortDic)
                {
                    WriteLine(MdlConst.LVL_DEBUG, $"[MOREREPLACE] KEY = {key} / VAL = {value}");
                }
            }

            // -debug-auth         ：認証DEBUGフラグ
            if (_namedArgs.ContainsKey("debug-auth"))
            {
                _isDebugAuth = true;
            }

            // -hh                 ：CmmnParams Usage
            if (_namedArgs.ContainsKey("hh") || _namedArgs.ContainsKey("??"))
            {
                _isUsage = true;
                GetArgsForAuth();
                ShowUsage();
            }

            // END
            return isOk;
        }

        /// <summary>
        /// 認証情報に関連するコマンドライン引数（ユーザー名、パスワード、暗号鍵、認証ファイル等）を取得および解析します。
        /// </summary>
        /// <returns>認証引数の取得・解析が正常に完了した場合は true、それ以外は false</returns>
        /// <example>
        /// <code>
        /// bool isAuthOk = cmmnArgs.GetArgsForAuth();
        /// </code>
        /// </example>
        public bool GetArgsForAuth()
        {
            bool isOk = true;
            string tempStr = "";
            // アカウント設定ファイルパス指定引数名
            foreach (string key in new string[] { "auth-conf-key" })
            {
                if (MdlArg.ContainsKey(_namedArgs, key))
                {
                    tempStr = MdlArg.GetValue(_namedArgs, key);
                    if (!String.IsNullOrEmpty(tempStr))
                    {
                        _argKeyOfUserConf = tempStr;
                    }
                }
            }
            // アカウント設定ファイルパス
            if (isOk) isOk = GetArgsForUserDefFile();
            // ユーザ名・ドメイン名の取得
            if (isOk) isOk = GetArgsForUser();
            // パスワード・暗号鍵・鍵サイズの取得
            if (isOk) isOk = GetArgsForPasswd();
            // 各種フラグの取得
            if (isOk) isOk = GetArgsForAuthFlag();
            // DEBUGフラグ
            if (_isDebugAuth) ShowDebugAuth();
            return isOk;
        }

        /// <summary>
        /// ユーザー定義の認証設定ファイル引数を取得し、設定ファイルを読み込みます。
        /// </summary>
        /// <returns>設定ファイルの読み込みおよび解析が成功した場合は true、失敗した場合は false</returns>
        /// <example>
        /// <code>
        /// cmmnArgs.ArgKeyOfUserConf = "def";
        /// bool isLoaded = cmmnArgs.GetArgsForUserDefFile();
        /// </code>
        /// </example>
        public bool GetArgsForUserDefFile()
        {
            bool isOk = true;

            string[] keys = [_argKeyOfUserConf, _argKeyOfUserConf + "name"];
            foreach (string key in keys)
            {
                if (_namedArgs.TryGetValue(key, out string? confVal) && !string.IsNullOrEmpty(confVal))
                {
                    if (key.Equals(_argKeyOfUserConf, StringComparison.Ordinal))
                    {
                        _authDefFilePath = confVal;
                    }
                    else if (key.Equals(_argKeyOfUserConf + "name", StringComparison.Ordinal))
                    {
                        _authDefFilePath = $@"{MdlConst.CONF_BASE}\passwd\{confVal}.{_replaceEnvIdKey}.yml";
                    }
                    isOk = ReadUserDefFile(confVal);
                    break;
                }
            }
            if (_isDebugAuth) ShowDebugAuth();

            return isOk;
        }

        /// <summary>
        /// 指定されたパスのユーザー定義（認証情報）設定ファイルを読み込み、ユーザー名・パスワード・暗号鍵等の情報を抽出します。
        /// </summary>
        /// <param name="strPathFFile">読み込む設定ファイルのパス</param>
        /// <returns>読み込み・解析が成功した場合は true、失敗した場合は false</returns>
        /// <example>
        /// <code>
        /// bool isRead = cmmnArgs.ReadUserDefFile(@"C:\Conf\auth.yml");
        /// </code>
        /// </example>
        public bool ReadUserDefFile(string filePath)
        {
            const string STR_MY_NAME = "[ClsCmmnArgs.ReadUserDefFile()]";
            bool isSuccess = true;
            _authDefFilePath = MdlFile.GetAbsolutePath(filePath.Trim());
            _authDefFilePath = ReplaceByDictionary(_authDefFilePath);
            if (MdlFile.PathExists(_authDefFilePath))
            {
                ClsConfigFile configFile = new(_logger);
                _authDefDic.Clear();
                configFile.ConfigDictionary = _authDefDic;
                configFile.Verbose = _verbose;
                configFile.Pattern = "^(?<KEY>[^#:]+):(?<VALUE>.+)$";
                if (configFile.LoadToDictionary(_authDefFilePath) > 0)
                {
                    foreach (string key in new string[] { "username" })
                    {
                        if (_authDefDic.ContainsKey(key) && !string.IsNullOrEmpty(_authDefDic[key]))
                        {
                            _username = _authDefDic[key];
                        }
                    }
                    SplitUserAndDomain();
                    foreach (string key in new string[] { "domain" })
                    {
                        if (_authDefDic.ContainsKey(key) && !string.IsNullOrEmpty(_authDefDic[key]))
                        {
                                _domainName = _authDefDic[key];
                        }
                    }
                    foreach (string key in new string[] { "password" })
                    {
                        _isDecodePasswd = true;
                        _password = _authDefDic[key];
                    }
                    foreach (string key2 in new string[] { "crypto", "encrypted" })
                    {
                        if (_authDefDic.ContainsKey(key2) && "false".Equals(_authDefDic[key2].ToLower())) _isDecodePasswd = false;
                    }
                    foreach (string key in new string[] { "plaintext" })
                    {
                        if (_authDefDic.ContainsKey(key) && !string.IsNullOrEmpty(_authDefDic[key]))
                        {
                            _password = _authDefDic[key];
                        }
                    }
                    foreach (string key in new string[] { "key", "enckey", "secret", "encenckey" })
                    {
                        if (_authDefDic.ContainsKey(key) && !string.IsNullOrEmpty(_authDefDic[key]))
                        {
                            _encKey = _authDefDic[key];
                            switch (key)
                            {
                                case "encenckey":
                                    _isDecodeKey = true;
                                    break;
                                default:
                                    _isDecodeKey = false;
                                    _encKey = _authDefDic[key];
                                    break;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(_encKey))
                    {
                        _isDecodeKey = false;
                        _isDefaultEncKey = true;
                        _encKey = _defaultEncKey;
                    }
                    foreach (string key in new string[] { "keysize" })
                    {
                        if (_authDefDic.TryGetValue(key, out string? val) && !string.IsNullOrEmpty(val))
                        {
                            _keySize = MdlUtil.ParseInt(val, 128);
                        }
                    }
                    foreach (string key in new string[] { "blocksize" })
                    {
                        if (_authDefDic.TryGetValue(key, out string? val) && !string.IsNullOrEmpty(val))
                        {
                            _blockSize = MdlUtil.ParseInt(val, 128);
                        }
                    }
                    foreach (string key in new string[] { "iteration" })
                    {
                        if (_authDefDic.TryGetValue(key, out string? val) && !string.IsNullOrEmpty(val))
                        {
                            _iterationCount = MdlUtil.ParseInt(val, 10000);
                        }
                    }
                    foreach (string key in new string[] { "hashalgo", "HashAlgorithm".ToLower() })
                    {
                        if (_authDefDic.ContainsKey(key) && !string.IsNullOrEmpty(_authDefDic[key]))
                        {
                            _hashAlgorithm = _authDefDic[key].ToUpper();
                        }
                    }
                    foreach (string key in new string[] { "env-enckey", "envenckey", "EncKeyEnvName".ToLower() })
                    {
                        if (_authDefDic.ContainsKey(key) && !string.IsNullOrEmpty(_authDefDic[key]))
                        {
                            _encKeyEnvName = _authDefDic[key];
                        }
                    }
                    if (_authDefDic.TryGetValue("debug-auth", out string? debugVal) && !string.IsNullOrEmpty(debugVal))
                    {
                        _isDebugAuth = debugVal.ToLowerInvariant() switch
                        {
                            "true" or "yes" or "y" => true,
                            _ => false
                        };
                    }
                }
                if ("MD5".Equals(_hashAlgorithm))
                {
                    _iterationCount = 0;
                }
                else
                {
                    if (_iterationCount < 1) _iterationCount = 1;
                }
            }
            else
            {
                WriteLine(MdlConst.LVL_DEBUG, STR_MY_NAME + "INVALID ARGUMENT: -" + _argKeyOfUserConf + " " + _authDefFilePath + " : NO SUCH A FILE");
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 現在設定されているユーザー名文字列から、ドメイン名とドメインなしユーザー名を分離して保持します。
        /// </summary>
        /// <example>
        /// <code>
        /// cmmnArgs.Username = @"DOMAIN\User01";
        /// cmmnArgs.SplitUserAndDomain();
        /// // DomainName -> "DOMAIN", UsernameWithoutDomain -> "User01"
        /// </code>
        /// </example>
        public void SplitUserAndDomain()
        {
            if (string.IsNullOrEmpty(_username))
            {
                _username = @"WORKGROUP\Administrator";
            }
            string[] fields = _username.Split('\\');
            if (fields.Length > 1)
            {
                _domainName = fields[0];
                _usernameWithoutDomain = fields[1];
            }
            else
            {
                if (string.IsNullOrEmpty(_domainName)) _domainName = @"WORKGROUP";
                _usernameWithoutDomain = fields[0];
            }
        }

        /// <summary>
        /// コマンドライン引数からユーザー名、ドメイン名、自ホスト/リモートホスト修飾引数（-u, -domain, -lhn, -rhn）を取得します。
        /// </summary>
        /// <returns>取得処理が成功した場合は true、失敗した場合は false</returns>
        /// <example>
        /// <code>
        /// bool ok = cmmnArgs.GetArgsForUser();
        /// </code>
        /// </example>
        public bool GetArgsForUser()
        {
            bool isOk = true;

            // -u|-user|-username n：ユーザ名
            foreach (string key in _keyNameOfUsernameList)
            {
                if (_namedArgs.TryGetValue(key, out string? uVal) && !string.IsNullOrEmpty(uVal))
                {
                    _username = uVal.Trim().Trim('\'', '\"');
                    break;
                }
            }
            SplitUserAndDomain();

            // ドメイン名
            if (_namedArgs.TryGetValue("domain", out string? domainVal) && !string.IsNullOrEmpty(domainVal))
            {
                _domainName = domainVal.Trim().Trim('\'', '\"');
            }

            // -lhn <name>         ：ユーザ名 => 指定値|自ホスト名\\ユーザ名
            if (_namedArgs.TryGetValue("lhn", out string? lhnVal))
            {
                if (!string.IsNullOrEmpty(lhnVal))
                {
                    _username = $"{lhnVal}\\{_usernameWithoutDomain}";
                    _domainName = lhnVal;
                }
                else
                {
                    _username = $"{_machineName}\\{_usernameWithoutDomain}";
                    _domainName = _machineName;
                }
            }

            // -rhn <name>         ：ユーザ名 => 指定値|接続先ホスト名\\ユーザ名
            if (_namedArgs.TryGetValue("rhn", out string? rhnVal))
            {
                if (!string.IsNullOrEmpty(rhnVal))
                {
                    _username = $"{rhnVal}\\{_usernameWithoutDomain}";
                    _domainName = rhnVal;
                }
                else
                {
                    _username = $"{_host}\\{_usernameWithoutDomain}";
                    _domainName = _host;
                }
            }

            // DEBUG
            if (_isDebugAuth)
            {
                WriteLine(MdlConst.LVL_DEBUG, "[GetArgsForUser] Domainname      : " + _domainName);
                WriteLine(MdlConst.LVL_DEBUG, "[GetArgsForUser] Username        : " + _username);
            }
            return isOk;
        }

        /// <summary>
        /// パスワード、暗号鍵、鍵長、暗号化アルゴリズム等のセキュリティパラメータ引数を取得・解析し、復号処理を実施します。
        /// </summary>
        /// <returns>引数の取得および復号が成功した場合は true、失敗した場合は false</returns>
        /// <example>
        /// <code>
        /// bool isPassOk = cmmnArgs.GetArgsForPasswd();
        /// </code>
        /// </example>
        public bool GetArgsForPasswd()
        {
            bool isOk = true;

            // -ep|-encpass ep     ：暗号化パスワード
            foreach (string key in _keyNameOfEncPassList)
            {
                if (_namedArgs.TryGetValue(key, out string? epVal) && !string.IsNullOrEmpty(epVal))
                {
                    _password = epVal.Trim().Trim('\'', '\"');
                    _isDecodePasswd = true;
                    break;
                }
            }

            // -p|-pass|-password p：パスワード
            foreach (string key in _keyNameOfPasswordList)
            {
                if (_namedArgs.TryGetValue(key, out string? pVal) && !string.IsNullOrEmpty(pVal))
                {
                    _password = pVal.Trim().Trim('\'', '\"');
                    _isDecodePasswd = false;
                    break;
                }
            }

            // -ek|-encenckey ek   ：暗号化暗号鍵
            foreach (string key in _keyNameOfEncEncKeyList)
            {
                if (_namedArgs.TryGetValue(key, out string? ekVal) && !string.IsNullOrEmpty(ekVal))
                {
                    _encKey = ekVal.Trim().Trim('\'', '\"');
                    _isDecodeKey = true;
                    break;
                }
            }

            // -k|-key|-enckey key ：暗号鍵
            foreach (string key in _keyNameOfEncKeyList)
            {
                if (_namedArgs.TryGetValue(key, out string? kVal) && !string.IsNullOrEmpty(kVal))
                {
                    _encKey = kVal.Trim().Trim('\'', '\"');
                    _isDecodeKey = false;
                    break;
                }
            }

            // 暗号鍵のデフォルト値
            if (string.IsNullOrEmpty(_encKey))
            {
                _isDecodeKey = false;
                _isDefaultEncKey = true;
                _encKey = _defaultEncKey;
            }

            // -s|-size|-keysize n ：鍵長
            foreach (string key in _keyNameOfEncKeySizeList)
            {
                if (_namedArgs.TryGetValue(key, out string? sVal) && !string.IsNullOrEmpty(sVal))
                {
                    int tempInt = MdlUtil.ParseInt(sVal, MdlConst.INT_NULL);
                    if (tempInt != MdlConst.INT_NULL)
                    {
                        _keySize = (tempInt == 256) ? 256 : 128;
                    }
                    break;
                }
            }

            // -blocksize num      ：ブロック長
            if (_namedArgs.TryGetValue("blocksize", out string? bsVal) && !string.IsNullOrEmpty(bsVal))
            {
                int tempInt = MdlUtil.ParseInt(bsVal, MdlConst.INT_NULL);
                if (tempInt != MdlConst.INT_NULL) _blockSize = tempInt;
            }

            // -hashalgo algo      ：MD5|SHA1|SHA256|SHA512
            string? algoVal = _namedArgs.GetValueOrDefault("hashalgo");
            if (string.IsNullOrEmpty(algoVal)) algoVal = _namedArgs.GetValueOrDefault("hashalgorithm");
            if (!string.IsNullOrEmpty(algoVal))
            {
                _hashAlgorithm = algoVal.ToUpperInvariant();
            }

            // -iteration num      ：繰返回数
            if (_namedArgs.TryGetValue("iteration", out string? iterVal) && !string.IsNullOrEmpty(iterVal))
            {
                int tempInt = MdlUtil.ParseInt(iterVal, MdlConst.INT_NULL);
                if (tempInt != MdlConst.INT_NULL) _iterationCount = tempInt;
            }

            // 値チェック
            if ("MD5".Equals(_hashAlgorithm, StringComparison.OrdinalIgnoreCase))
            {
                _iterationCount = 0;
            }
            else
            {
                if (_iterationCount < 1) _iterationCount = 1;
            }

            // 暗号鍵複合化
            isOk = DecryptKeyAndPassword();

            // DEBUG
            if (_isDebugAuth) ShowDebugAuth();

            return isOk;
        }

        /// <summary>
        /// 現在保持されている認証デバッグ情報（ユーザー名、暗号鍵、鍵長、ハッシュアルゴリズム等）をログに出力します。
        /// </summary>
        /// <example>
        /// <code>
        /// cmmnArgs.ShowDebugAuth();
        /// </code>
        /// </example>
        public void ShowDebugAuth()
        {
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] Username        : " + _username);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] Password        : " + _password);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] EncKey          : " + _encKey);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] KeySize         : " + _keySize);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] BlockSize       : " + _blockSize);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] HashAlgo        : " + _hashAlgorithm);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] Iteration       : " + _iterationCount);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] IsDecodeKey     : " + _isDecodeKey);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] IsDecodePasswd  : " + _isDecodePasswd);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] EncKeyEnvName   : " + _encKeyEnvName);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] IsSwitchUser    : " + _isSwitchUser);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] IsLogon         : " + _isLogon);
            WriteLine(MdlConst.LVL_DEBUG, "[showDubugAuth] IsLogonAlwaysOk : " + _isLogonAlwaysOk);
        }

        /// <summary>
        /// 認証処理の動作制御フラグ（認証エラー無視、ユーザー切り替えフラグ、ログオンフラグ等）をコマンドライン引数から取得します。
        /// </summary>
        /// <returns>フラグの取得が正常に完了した場合は true、失敗した場合は false</returns>
        /// <example>
        /// <code>
        /// bool ok = cmmnArgs.GetArgsForAuthFlag();
        /// </code>
        /// </example>
        public bool GetArgsForAuthFlag()
        {
            bool isOk = true;

            // -ignore-fail        ：認証エラー無視フラグ
            foreach (string key in new string[] { "ignore-fail" })
            {
                if (MdlArg.ContainsKey(_namedArgs, key))
                {
                    _isLogonAlwaysOk = true;
                    break;
                }
            }

            // -su                 ：ユーザー認証実行フラグ
            foreach (string key in new string[] { "su" })
            {
                if (MdlArg.ContainsKey(_namedArgs, key))
                {
                    _isSwitchUser = true;
                    break;
                }
            }

            // -logon              ：ユーザー認証実行フラグ
            foreach (string key in new string[] { "logon" })
            {
                if (MdlArg.ContainsKey(_namedArgs, key))
                {
                    _isLogon = true;
                    break;
                }
            }

            // DEBUG
            if (_isDebugAuth)
            {
                WriteLine(MdlConst.LVL_DEBUG, "[GetArgsForAuthFlag] IsSwitchUser    : " + _isSwitchUser);
                WriteLine(MdlConst.LVL_DEBUG, "[GetArgsForAuthFlag] IsLogon         : " + _isLogon);
                WriteLine(MdlConst.LVL_DEBUG, "[GetArgsForAuthFlag] IsLogonAlwaysOk : " + _isLogonAlwaysOk);
            }

            return isOk;
        }

        /// <summary>
        /// 保持されている暗号鍵およびパスワードが暗号化されている場合、それぞれの復号処理を行います。
        /// </summary>
        /// <returns>復号処理がすべて成功した場合は true、失敗した場合は false</returns>
        /// <example>
        /// <code>
        /// bool decrypted = cmmnArgs.DecryptKeyAndPassword();
        /// </code>
        /// </example>
        public bool DecryptKeyAndPassword()
        {
            bool isSuccess = true;

            // 暗号鍵複合化
            if (isSuccess && _isDecodeKey)
            {
                _encKey = DecryptPassword(_defaultEncKey, _encKey, _keySize, _blockSize, _hashAlgorithm, _iterationCount) ?? "";
                if (string.IsNullOrEmpty(_encKey))
                {
                    isSuccess = false;
                }
                else
                {
                    _isDecodeKey = false;
                }
            }

            // パスワード複合化
            if (isSuccess && _isDecodePasswd)
            {
                _password = DecryptPassword(_encKey, _password, _keySize, _blockSize, _hashAlgorithm, _iterationCount) ?? "";
                if (string.IsNullOrEmpty(_password))
                {
                    isSuccess = false;
                }
                else
                {
                    _isDecodePasswd = false;
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// 指定された暗号鍵とセキュリティパラメータを用いて、暗号化されたパスワードを復号し、平文文字列を返します。
        /// </summary>
        /// <param name="encKey">復号に使用する暗号鍵</param>
        /// <param name="password">暗号化されたパスワード文字列</param>
        /// <param name="keySize">鍵長（ビット数: 128 または 256）</param>
        /// <param name="blockSize">ブロックサイズ（ビット数）</param>
        /// <param name="hashAlgorithm">使用するハッシュアルゴリズム（例: "AES", "SHA256", "MD5"）</param>
        /// <param name="iterationCount">ストレッチング繰返回数</param>
        /// <returns>復号に成功した場合は平文パスワード、失敗した場合は空文字列</returns>
        /// <example>
        /// <code>
        /// string plainPass = cmmnArgs.DecryptPassword("myKey", "encryptedBase64Str", 128, 128, "SHA256", 10000);
        /// </code>
        /// </example>
        public string DecryptPassword(string encKey, string password, int keySize, int blockSize, string hashAlgorithm, int iterationCount)
        {
            string output = "";
            if (string.IsNullOrEmpty(encKey)) return "";
            if (string.IsNullOrEmpty(password)) return "";
            if (string.IsNullOrEmpty(hashAlgorithm)) hashAlgorithm = _hashAlgorithm;
            ClsCrypt crypt = new ClsCrypt();
            crypt.KeySize = keySize;
            crypt.BlockSize = blockSize;
            crypt.HashAlgorithm = hashAlgorithm;
            crypt.IterationCount = iterationCount;
            crypt.IsVerbose = _isDebugAuth;
            crypt.EncKeyEnvName = _encKeyEnvName;
            if (_isDebugAuth)
            {
                WriteLine(MdlConst.LVL_DEBUG, "[DecodePasswd] crypt.Decrypt(" + encKey + ", " + password + ")");
            }
            if (crypt.Decrypt(encKey, password))
            {
                output = crypt.Result;
            }
            else
            {
                WriteLine(MdlConst.LVL_E, crypt.ErrorMessage);
                WriteLine(MdlConst.LVL_E, crypt.ErrorDump);
            }
            return output;
        }

        /// <summary>
        /// 暗号化されたパスワードを復号します（繰返回数 default=0）。
        /// </summary>
        /// <param name="encKey">暗号鍵</param>
        /// <param name="password">暗号化パスワード</param>
        /// <param name="keySize">鍵長</param>
        /// <param name="blockSize">ブロックサイズ</param>
        /// <param name="hashAlgorithm">ハッシュアルゴリズム</param>
        /// <returns>復号された平文パスワード</returns>
        /// <example>
        /// <code>
        /// string plainPass = cmmnArgs.DecryptPassword("myKey", "encStr", 128, 128, "MD5");
        /// </code>
        /// </example>
        public string DecryptPassword(string encKey, string password, int keySize, int blockSize, string hashAlgorithm)
        {
            return DecryptPassword(encKey, password, keySize, blockSize, hashAlgorithm, 0);
        }

        /// <summary>
        /// 暗号化されたパスワードを復号します（ハッシュアルゴリズム default="MD5"）。
        /// </summary>
        /// <param name="encKey">暗号鍵</param>
        /// <param name="password">暗号化パスワード</param>
        /// <param name="keySize">鍵長</param>
        /// <param name="blockSize">ブロックサイズ</param>
        /// <returns>復号された平文パスワード</returns>
        /// <example>
        /// <code>
        /// string plainPass = cmmnArgs.DecryptPassword("myKey", "encStr", 128, 128);
        /// </code>
        /// </example>
        public string DecryptPassword(string encKey, string password, int keySize, int blockSize)
        {
            return DecryptPassword(encKey, password, keySize, blockSize, "MD5", 0);
        }

        /// <summary>
        /// 暗号化されたパスワードを復号します（ブロックサイズ default=128）。
        /// </summary>
        /// <param name="encKey">暗号鍵</param>
        /// <param name="password">暗号化パスワード</param>
        /// <param name="keySize">鍵長</param>
        /// <returns>復号された平文パスワード</returns>
        /// <example>
        /// <code>
        /// string plainPass = cmmnArgs.DecryptPassword("myKey", "encStr", 128);
        /// </code>
        /// </example>
        public string DecryptPassword(string encKey, string password, int keySize)
        {
            return DecryptPassword(encKey, password, keySize, 128, "MD5", 0);
        }

        /// <summary>
        /// 暗号化されたパスワードを復号します（鍵長 default=128）。
        /// </summary>
        /// <param name="encKey">暗号鍵</param>
        /// <param name="password">暗号化パスワード</param>
        /// <returns>復号された平文パスワード</returns>
        /// <example>
        /// <code>
        /// string plainPass = cmmnArgs.DecryptPassword("myKey", "encStr");
        /// </code>
        /// </example>
        public string DecryptPassword(string encKey, string password)
        {
            return DecryptPassword(encKey, password, 128, 128, "MD5", 0);
        }

        /// <summary>
        /// 指定された引数キーに対応する絶対パス文字列を取得・正規化（辞書置換、末尾セパレータ除去、ディレクトリ生成）します。
        /// </summary>
        /// <param name="key">パスを取得するためのコマンドライン引数キー</param>
        /// <param name="pathType">パス判定タイプ（<see cref="MdlFile.PATH_IS_FILE"/>, <see cref="MdlFile.PATH_IS_DIRECTORY"/>, <see cref="MdlFile.PATH_AUTO_DETECT"/>）</param>
        /// <param name="createDirectory">ディレクトリが存在しない場合に自動生成するかどうか</param>
        /// <returns>取得・正規化されたパス文字列。取得不可または存在チェック失敗時は空文字列</returns>
        /// <example>
        /// <code>
        /// string dirPath = cmmnArgs.GetPathParam("ldir", MdlFile.PATH_IS_DIRECTORY, true);
        /// </code>
        /// </example>
        public string GetPathParam(string key, int pathType, bool createDirectory)
        {
            const string STR_MY_NAME = "[ClsCmmnArgs.GetPathParam()]";
            string result = "";
            result = MdlFile.RemoveTrailingPathSeparator(MdlFile.GetAbsolutePath(_namedArgs.GetValueOrDefault(key, "")));
            if (string.IsNullOrEmpty(MdlFile.GetDirectoryPath(result))) result = result + @"\.";
            result = ReplaceByDictionary(result);
            string directoryToCheck = pathType switch
            {
                MdlFile.PATH_IS_FILE => MdlFile.GetDirectoryPath(result),
                MdlFile.PATH_AUTO_DETECT => MdlFile.GetPathType(result) switch
                {
                    MdlFile.PATH_IS_DIRECTORY => result,
                    MdlFile.PATH_IS_FILE => MdlFile.GetDirectoryPath(result),
                    _ => ""
                },
                _ => result
            };
            if (!string.IsNullOrEmpty(directoryToCheck) && createDirectory)
            {
                MdlFile.CreateDirectory(directoryToCheck);
            }
            if (MdlFile.PATH_IS_DIRECTORY != MdlFile.GetPathType(directoryToCheck))
            {
                result = "";
            }
            if (string.IsNullOrEmpty(result))
            {
                WriteLine(MdlConst.LVL_E, STR_MY_NAME + " PLEASE SPECIFY THE ARGUMENT : -" + key + " = " + _namedArgs.GetValueOrDefault(key, ""));
            }
            return result;
        }

        /// <summary>
        /// 引数で指定した対象文字列中のキーを、置換用辞書 (<see cref="ReplaceDic"/>) に登録された対応値に一括置換して返します。
        /// </summary>
        /// <param name="target">置換対象の文字列</param>
        /// <returns>置換処理後の文字列</returns>
        /// <example>
        /// <code>
        /// string result = cmmnArgs.ReplaceByDictionary(@"C:\Data\__ENV_ID__\file.txt");
        /// </code>
        /// </example>
        public string ReplaceByDictionary(string target)
        {
            const string STR_MY_NAME = "[ClsCmmnArgs.ReplaceByDictionary()]";
            string result = target;
            foreach (KeyValuePair<string, string> pair in _replaceDic)
            {
                result = result.Replace(pair.Key, pair.Value);
                if (_verbose > 5) WriteLine(MdlConst.LVL_DEBUG, STR_MY_NAME + "[" + pair.Key + "⇒" + pair.Value + "] " + target + "⇒" + result);
            }
            return result;
        }

        /// <summary>
        /// ネットワーク共有共有パス（NET USE）に関連するコマンドライン引数（マウントパス、ドライブ名、エラー無視リスト等）を取得・設定します。
        /// </summary>
        /// <returns>処理が正常に終了した場合は true、それ以外は false</returns>
        /// <example>
        /// <code>
        /// bool ok = cmmnArgs.GetNetUseArgs();
        /// </code>
        /// </example>
        public bool GetNetUseArgs()
        {
            bool isOk = true;

            // -mount path         ：マウントフラグ
            if (_namedArgs.TryGetValue("mount", out string? mountVal) && !string.IsNullOrEmpty(mountVal))
            {
                _netSharePath = MdlFile.RemoveTrailingPathSeparator(mountVal);
            }

            // -drive name         ：ドライブ名
            if (_namedArgs.TryGetValue("drive", out string? driveVal) && !string.IsNullOrEmpty(driveVal))
            {
                _driveName = driveVal.Replace(":", "");
            }

            // ネットワーク共有ディレクトリパスが指定されなかった場合
            if (string.IsNullOrEmpty(_netSharePath))
            {
                _isMount = false;
                _isUmount = false;
            }
            // ネットワーク共有ディレクトリパスが指定された場合
            else
            {
                _isMount = !_namedArgs.ContainsKey("no-mount");
                _isUmount = !_namedArgs.ContainsKey("no-umount");
            }

            // -mount-ok-no csv    ：正常と見なすエラー番号リスト
            string? okNoVal = _namedArgs.GetValueOrDefault("mount-ok-no");
            if (string.IsNullOrEmpty(okNoVal)) okNoVal = _namedArgs.GetValueOrDefault("logon-ok-no");
            if (!string.IsNullOrEmpty(okNoVal))
            {
                List<string> netUseOkErrNoListStr = MdlUtil.ParseCsvToList(null, okNoVal);
                foreach (string element in netUseOkErrNoListStr)
                {
                    string strVal = element.Trim();
                    if (!string.IsNullOrEmpty(strVal))
                    {
                        int intVal = MdlUtil.ParseInt(strVal, MdlConst.INT_NULL);
                        if (intVal != MdlConst.INT_NULL) _netUseOkErrNoList.Add(intVal);
                    }
                }
            }

            return isOk;
        }

        /// <summary>
        /// ファイル・ディレクトリの絞り込み/除外フィルタ（-if, -id, -xf, -xd等）のコマンドライン引数を解析し、各種ルールリストに登録します。
        /// </summary>
        /// <returns>取得・設定が成功した場合は true、失敗した場合は false</returns>
        /// <example>
        /// <code>
        /// bool ok = cmmnArgs.GetFilterLists();
        /// </code>
        /// </example>
        public bool GetFilterLists()
        {
            bool isSuccess = true;
            string temp = "";
            // フィルタ設定：対象ファイル
            if (isSuccess)
            {
                temp = _namedArgs.GetValueOrDefault("if", "");
                if (!string.IsNullOrEmpty(temp))
                {
                    if (_replaceDic.Count > 0) temp = ReplaceByDictionary(temp);
                    _incFilesList = MdlUtil.ParseCsvToList(_incFilesList, temp, _splitPattern, _verbose, true);
                }
            }
            // フィルタ設定：対象フォルダ
            if (isSuccess)
            {
                temp = _namedArgs.GetValueOrDefault("idf", "");
                if (!string.IsNullOrEmpty(temp)) _isRegIncBasename = false;
                if (string.IsNullOrEmpty(temp)) temp = _namedArgs.GetValueOrDefault("id", "");
                if (string.IsNullOrEmpty(temp)) temp = _namedArgs.GetValueOrDefault("idb", "");
                if (!string.IsNullOrEmpty(temp))
                {
                    if (_replaceDic.Count > 0) temp = ReplaceByDictionary(temp);
                    _incDirsList = MdlUtil.ParseCsvToList(_incDirsList, temp, _splitPattern, _verbose, true);
                }
            }
            // フィルタ設定：除外ファイル
            if (isSuccess)
            {
                temp = _namedArgs.GetValueOrDefault("xf", "");
                if (!string.IsNullOrEmpty(temp))
                {
                    if (_replaceDic.Count > 0) temp = ReplaceByDictionary(temp);
                    _excFilesList = MdlUtil.ParseCsvToList(_excFilesList, temp, _splitPattern, _verbose, true);
                }
            }
            // フィルタ設定：除外フォルダ
            if (isSuccess)
            {
                temp = _namedArgs.GetValueOrDefault("xdf", "");
                if (!string.IsNullOrEmpty(temp)) _isRegExcBasename = false;
                if (string.IsNullOrEmpty(temp)) temp = _namedArgs.GetValueOrDefault("xd", "");
                if (string.IsNullOrEmpty(temp)) temp = _namedArgs.GetValueOrDefault("xdb", "");
                if (!string.IsNullOrEmpty(temp))
                {
                    if (_replaceDic.Count > 0) temp = ReplaceByDictionary(temp);
                    _excDirsList = MdlUtil.ParseCsvToList(_excDirsList, temp, _splitPattern, _verbose, true);
                }
            }
            // -id結果を階層下に適用するか否か
            if (_namedArgs.ContainsKey("idorxd"))
            {
                _isDirFilterOr = true;
            }
            if (_namedArgs.ContainsKey("no-id-rec"))
            {
                _isIncHitRecursive = false;
            }
            if (_namedArgs.ContainsKey("no-xd-rec"))
            {
                _isExcHitRecursive = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 指定されたエラーレベルとメッセージをロガー経由でログファイルおよびコンソールに出力します。
        /// </summary>
        /// <param name="level">ログメッセージのエラーレベル（例: <see cref="MdlConst.LVL_E"/>, <see cref="MdlConst.LVL_DEBUG"/>）</param>
        /// <param name="message">出力するメッセージ文字列</param>
        /// <example>
        /// <code>
        /// cmmnArgs.Writeln(MdlConst.LVL_DEBUG, "処理を開始しました。");
        /// </code>
        /// </example>
        public void WriteLine(int level, string message)
        {
            try
            {
                _logger.WriteLine(level, message);
            }
            catch
            {
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// 共通コマンドライン引数の利用方法（Usage）および現在の各パラメータ設定値を標準出力に出力します。
        /// </summary>
        /// <example>
        /// <code>
        /// cmmnArgs.Usage();
        /// </code>
        /// </example>
        public void ShowUsage()
        {
            WriteLine(MdlConst.LVL_NONE, "");
            WriteLine(MdlConst.LVL_NONE, "CmmnParams Option：");
            WriteLine(MdlConst.LVL_NONE, "   -arg-def path       ：引数定義INIファイルパス     （現在値=" + _argDefFilePath + "）");
            WriteLine(MdlConst.LVL_NONE, "   -h|-help|-?         ：Usage表示                   （現在値=" + _isUsage + "）");
            WriteLine(MdlConst.LVL_NONE, "   -h hostname         ：ホスト名                    （現在値=" + _host + "）");
            WriteLine(MdlConst.LVL_NONE, "   -force              ：強制実行フラグ              （現在値=" + _isForce + "）");
            WriteLine(MdlConst.LVL_NONE, "CmmnParams Output Option：");
            WriteLine(MdlConst.LVL_NONE, "   -v |-vv|-brief num  ：冗長表示                    （現在値=" + _verbose + "）");
            WriteLine(MdlConst.LVL_NONE, "   -diff               ：差分表示フラグ              （現在値=" + _isDiff + "）");
            WriteLine(MdlConst.LVL_NONE, "   -console mode       ：メッセージ表示 off|stdout|stderr");
            WriteLine(MdlConst.LVL_NONE, "   -stacktrace         ：例外時スタックトレース表示  （現在値=" + _isStackTrace + "）");
            WriteLine(MdlConst.LVL_NONE, "   -stdenc encode      ：標準出力エンコード          （現在値=" + _logger.GetValueByKey(ClsLogger.CONSOLE_ENCODING, "") + "）");
            WriteLine(MdlConst.LVL_NONE, "CmmnParams Job Option：");
            WriteLine(MdlConst.LVL_NONE, "   -ajsjobname name    ：AJSJOBNAME                  （現在値=" + (null != _jp1 ? Jp1.JobName : "") + "）");
            WriteLine(MdlConst.LVL_NONE, "   -nojp1              ：AJSJOBNAME参照フラグ        （現在値=" + _isAjsJob + "）");
            WriteLine(MdlConst.LVL_NONE, "   -envajs str         ：AJSJOBNAME検索プレフィックス（現在値=" + (null != _jp1 ? Jp1.Prefix : "") + "）");
            WriteLine(MdlConst.LVL_NONE, "   -envvar str         ：環境変数検索プレフィックス  （現在値=" + _envPrefix + "）");
            WriteLine(MdlConst.LVL_NONE, "   -envenvid str       ：環境種別キー環境変数名      （現在値=" + _envIdKey + "）");
            WriteLine(MdlConst.LVL_NONE, "CmmnParams Replace Option：");
            WriteLine(MdlConst.LVL_NONE, "   -replace a:b        ：文字列置換CSVリスト         （現在値=" + _namedArgs.GetValueOrDefault("replace", "") + "）");
            WriteLine(MdlConst.LVL_NONE, "   -morereplace b:c    ：文字列再値置換CSVリスト     （現在値=" + _namedArgs.GetValueOrDefault("morereplace", "") + "）");
            WriteLine(MdlConst.LVL_NONE, "   -reservereplace     ：文字列予約語再値置換        （現在値=" + _namedArgs.GetValueOrDefault("reservereplace", "") + "）");
            WriteLine(MdlConst.LVL_NONE, "   -splitby pattern    ：文字列分割デリミタパターン  （現在値=" + _splitPattern + "）");
            WriteLine(MdlConst.LVL_NONE, "   -split-kv-by pattern：key[分割デリミタパターン]Val（現在値=" + _keyValDelimiter + "）");
            WriteLine(MdlConst.LVL_NONE, "CmmnParams Log Option：");
            WriteLine(MdlConst.LVL_NONE, "   -ldir path          ：ログ出力先ディレクトリパス（日付付ファイル名で出力）（現在値=" + _logger.GetValueByKey(ClsLogger.DIR, "") + "）");
            WriteLine(MdlConst.LVL_NONE, "   -log  path          ：ログ出力ファイルパス（-ldirより優先）               （現在値=" + _logger.GetValueByKey(ClsLogger.PATH, "") + "）");
            WriteLine(MdlConst.LVL_NONE, "   -logenc encode      ：ログファイルエンコード      （現在値=" + _logger.GetValueByKey(ClsLogger.FILE_ENCODING, "") + "）");
            WriteLine(MdlConst.LVL_NONE, "CmmnParams Command Option：");
            WriteLine(MdlConst.LVL_NONE, "   -retry num          ：リトライ回数                （現在値=" + _retryMax + "）");
            WriteLine(MdlConst.LVL_NONE, "   -sleep sec          ：リトライ間隔（秒）          （現在値=" + _retrySleep + "）");
            WriteLine(MdlConst.LVL_NONE, "   -timeout sec        ：タイムアウト（秒）          （現在値=" + _timeout + "）");
            WriteLine(MdlConst.LVL_NONE, "CmmnParams Auth Option：");
            WriteLine(MdlConst.LVL_NONE, "   -auth-conf-key key  ：アカウント設定ファイルパス指定引数名 （現在値=" + _argKeyOfUserConf + "）");
            WriteLine(MdlConst.LVL_NONE, "   -domain str         ：ドメイン名                  （現在値=" + _domainName + "）");
            WriteLine(MdlConst.LVL_NONE, "   -lhn <name>         ：ユーザ名 => 指定値|自ホスト名\\ユーザ名");
            WriteLine(MdlConst.LVL_NONE, "   -rhn <name>         ：ユーザ名 => 指定値|接続先ホスト名\\ユーザ名");
            WriteLine(MdlConst.LVL_NONE, "   -u|-user|-username n：ユーザ名                    （現在値=" + _username + "）");
            WriteLine(MdlConst.LVL_NONE, "   -p|-pass|-password p：パスワード                  （現在値=" + _password + "）");
            WriteLine(MdlConst.LVL_NONE, "   -ep|-encpass ep     ：暗号化パスワード");
            WriteLine(MdlConst.LVL_NONE, "   -k|-key|-enckey key ：暗号鍵                      （現在値=" + (_verbose > 4 ? _encKey : "***************") + "）");
            WriteLine(MdlConst.LVL_NONE, "   -ek|-encenckey ek   ：暗号化暗号鍵");
            WriteLine(MdlConst.LVL_NONE, "   -s|-size|-keysize n ：鍵長                        （現在値=" + _keySize + "）");
            WriteLine(MdlConst.LVL_NONE, "   -blocksize num      ：ブロック長                  （現在値=" + _blockSize + "）");
            WriteLine(MdlConst.LVL_NONE, "   -hashalgo algo      ：MD5|SHA1|SHA256|SHA512      （現在値=" + _hashAlgorithm + "）");
            WriteLine(MdlConst.LVL_NONE, "   -iteration num      ：繰返回数                    （現在値=" + _iterationCount + "）");
            WriteLine(MdlConst.LVL_NONE, "   -ignore-fail        ：認証エラー無視フラグ        （現在値=" + _isLogonAlwaysOk + "）");
            WriteLine(MdlConst.LVL_NONE, "   -su                 ：ユーザー認証実行フラグ      （現在値=" + _isSwitchUser + "）");
            WriteLine(MdlConst.LVL_NONE, "   -logon              ：ユーザー認証実行フラグ      （現在値=" + _isLogon + "）");
            WriteLine(MdlConst.LVL_NONE, "   -env-enckey name    ：暗号鍵格納環境変数名        （現在値=" + _encKeyEnvName + "）");
            WriteLine(MdlConst.LVL_NONE, "CmmnParams NetUse Option：");
            WriteLine(MdlConst.LVL_NONE, "   -mount path         ：マウントフラグ              （現在値=" + _isMount + "）");
            WriteLine(MdlConst.LVL_NONE, "   -drive              ：ドライブ名                  （現在値=" + !_isMount + "）");
            WriteLine(MdlConst.LVL_NONE, "   -no-mount           ：非マウントフラグ            （現在値=" + !_isMount + "）");
            WriteLine(MdlConst.LVL_NONE, "   -mount-ok-no csv    ：正常と見なすエラー番号リスト（現在値=" + !_isMount + "）");
            WriteLine(MdlConst.LVL_NONE, "CmmnParams Debug Option：");
            WriteLine(MdlConst.LVL_NONE, "   -dumpargs           ：引数の表示");
            WriteLine(MdlConst.LVL_NONE, "   -dumpreplace        ：置換リストの表示");
            WriteLine(MdlConst.LVL_NONE, "   -debug-auth         ：認証DEBUGフラグ             （現在値=" + _isDebugAuth + "）");
            WriteLine(MdlConst.LVL_NONE, "   -hh|-??             ：CmmnParams Usage");
            WriteLine(MdlConst.LVL_NONE, "");
        }

        /// <summary>
        /// 各種デフォルト引数名キーリストの初期化および環境情報の取得を行います（旧メソッド）。
        /// </summary>
        [Obsolete("代わりに 'InitializeLists()' を使用します。")]
        public void InitList()
        {
            InitializeLists();
        }

        /// <summary>
        /// 保持されている暗号鍵およびパスワードの復号処理を行います（旧メソッド）。
        /// </summary>
        [Obsolete("代わりに 'DecryptKeyAndPassword()' を使用します。")]
        public bool DecryptKeyAndPasswd()
        {
            return DecryptKeyAndPassword();
        }

        /// <summary>
        /// 暗号化されたパスワードを復号します（旧メソッド）。
        /// </summary>
        [Obsolete("代わりに 'DecryptPassword()' を使用します。")]
        public string DecryptPasswd(string encKey, string password, int keySize, int blockSize, string hashAlgorithm, int iterationCount)
            => DecryptPassword(encKey, password, keySize, blockSize, hashAlgorithm, iterationCount);

        /// <summary>
        /// 暗号化されたパスワードを復号します（旧メソッド、繰返回数 default=0）。
        /// </summary>
        [Obsolete("代わりに 'DecryptPassword()' を使用します。")]
        public string DecryptPasswd(string encKey, string password, int keySize, int blockSize, string hashAlgorithm)
            => DecryptPassword(encKey, password, keySize, blockSize, hashAlgorithm);

        /// <summary>
        /// 暗号化されたパスワードを復号します（旧メソッド、ハッシュアルゴリズム default="MD5"）。
        /// </summary>
        [Obsolete("代わりに 'DecryptPassword()' を使用します。")]
        public string DecryptPasswd(string encKey, string password, int keySize, int blockSize)
            => DecryptPassword(encKey, password, keySize, blockSize);

        /// <summary>
        /// 暗号化されたパスワードを復号します（旧メソッド、ブロックサイズ default=128）。
        /// </summary>
        [Obsolete("代わりに 'DecryptPassword()' を使用します。")]
        public string DecryptPasswd(string encKey, string password, int keySize)
            => DecryptPassword(encKey, password, keySize);

        /// <summary>
        /// 暗号化されたパスワードを復号します（旧メソッド、鍵長 default=128）。
        /// </summary>
        [Obsolete("代わりに 'DecryptPassword()' を使用します。")]
        public string DecryptPasswd(string encKey, string password)
            => DecryptPassword(encKey, password);

        /// <summary>
        /// 引数で指定した対象文字列中のキーを置換用辞書で置換して返します（旧メソッド）。
        /// </summary>
        [Obsolete("代わりに 'ReplaceByDictionary()' を使用します。")]
        public string ReplaceByDic(string target)
        {
            return ReplaceByDictionary(target);
        }

        /// <summary>
        /// 現在保持されている認証デバッグ情報をログに出力します（旧メソッド）。
        /// </summary>
        [Obsolete("代わりに 'ShowDebugAuth()' を使用します。")]
        public void ShowDubugAuth()
        {
            ShowDebugAuth();
        }

        /// <summary>
        /// 指定されたエラーレベルとメッセージを出力します（旧メソッド）。
        /// </summary>
        [Obsolete("代わりに 'WriteLine()' を使用します。")]
        public void Writeln(int level, string message)
        {
            WriteLine(level, message);
        }

        /// <summary>
        /// 共通コマンドライン引数の利用方法を出力します（旧メソッド）。
        /// </summary>
        [Obsolete("代わりに 'ShowUsage()' を使用します。")]
        public void Usage()
        {
            ShowUsage();
        }

    }
}
