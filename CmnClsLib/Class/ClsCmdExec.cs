using System.Text;
using System.Diagnostics;
using System.Security;
using System.Runtime.InteropServices;
using CmnClsLib.Interface;
using CmnClsLib.Module;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Class
{
    /// <summary>
    /// 外部コマンドやプロセスの実行およびログ管理、終了ステータス判定機能を提供するクラスです。
    /// </summary>
    /// <example>
    /// <code>
    /// var logger = new ConsoleLogger();
    /// var cmdExec = new ClsCmdExec(logger)
    /// {
    ///     CmdPath = "ping.exe",
    ///     CmdArgs = "127.0.0.1",
    ///     IsShowOutput = true
    /// };
    /// int result = cmdExec.ExecuteThread(3);
    /// </code>
    /// </example>
    public class ClsCmdExec
    {
        private readonly ICmnLogger _logger;                        // ログ出力
        private Thread? _thread = null;                             // スレッド
        private Process? _process = null;                           // プロセス
        private readonly ClsCmdStatus _cmdStatus;                   // コマンド終了コード判定
        private Dictionary<string, string> _processEnvs = [];       // 環境変数リスト
        private bool _isRunning = false;                            // 実行中フラグ
        private bool _isShowCmd = false;                            // コマンド表示フラグ
        private bool _isShowEmptyLine = true;                       // 空行表示フラグ
        private bool _isShowOutput = false;                         // 標準出力表示フラグ
        private bool _isNotShowOutput = false;                      // 標準出力非表示フラグ
        private bool _isShowExitCode = false;                       // 終了コード表示フラグ
        private bool _isNotShowExitCode = false;                    // 終了コード非表示フラグ
        private bool _isInfoPrefix = true;                          // 出力の先頭に文字列付与フラグ
        private bool _isStdoutPrefix = false;                       // 出力の先頭に文字列付与フラグ
        private bool _isErrorDialog = false;                        // 起動できなかった時のエラーダイアログ表示フラグ
        private bool _isUseShellExecute = false;                    // コンソールウィンドウ表示フラグ
        private bool _isCreateNoWindow = true;                      // ウィンドウ非作成フラグ
        private bool _isLoadUserProfile = false;                    // ユーザープロファイルの読み込みフラグ
        private bool _isRunAs = false;                              // 管理者として実行フラグ
        private bool _isNoRedirect = false;                         // 非リダイレクトフラグ
        private bool _isSu = false;                                 // SwitchUserフラグ
        private bool _isStackTrace = false;                         // スタックトレースフラグ
        private bool _isSilent = false;                             // ログ非出力フラグ
        private bool _isShowEnvDic = false;                         // 環境変数リスト表示
        private bool _isClearStringBuilder = true;                  // _stringBuilderのクリアフラグ
        private string _errorMessage = "";                          // エラーメッセージ
        private string _cmdPath = "";                               // コマンドパス
        private string _workDir = "";                               // ワーキングディレクトリパス
        private string _cmdArgs = "";                               // コマンド引数
        private string _prefix = "0";                               // ログ出力PREFIX
        private string _encoding = "";                              // 出力文字コード：Shift_JIS|UTF-8
        private string _domainName = "";                            // ドメイン名
        private string _username = "";                              // ユーザ名
        private string _password = "";                              // パスワード
        private string _processEnvCsv = "";                         // 環境変数カンマ区切り文字列
        private string _stdIn = "";                                 // 標準入力文字列
        private int _defaultErrorLogLevel = MdlConst.LVL_E;         // デフォルトエラーレベル
        private int _verbose = 0;                                   // 冗長出力レベル
        private int _debugLevel = MdlConst.LVL_NONE;                // DEBUGログ出力レベル
        private int _stdoutLevel = MdlConst.LVL_NONE;               // コマンド標準出力のログ出力レベル
        private int _timeout = 86400;                               // タイムアウト（秒）
        private static readonly StringBuilder _stringBuilder = new();
        private static readonly Lock _lockStringBuilder = new();    // .NET 9/10 高速同期オブジェクト
        private int _cmdExitStatus = 0;                             // コマンド終了コード
        private System.Text.Encoding _encodingObject = System.Text.Encoding.Default;

        /// <summary>
        /// <see cref="ClsCmdExec"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="logger">ログ出力を行うロガーインスタンス</param>
        /// <example>
        /// <code>
        /// ICmnLogger logger = new CustomLogger();
        /// var cmdExec = new ClsCmdExec(logger);
        /// </code>
        /// </example>
        public ClsCmdExec(ICmnLogger logger)
        {
            _logger = logger;
            _cmdStatus = new(logger);
        }

        /// <summary>プロセスに設定する環境変数の辞書を取得または設定します。</summary>
        public Dictionary<string, string> ProcessEnvs { get => _processEnvs; set => _processEnvs = value; }
        
        /// <summary>コマンド実行時の標準出力・標準エラー出力を保持する StringBuilder を取得または設定します。</summary>
        public StringBuilder StringBuilder { get => _stringBuilder; set { } }
        
        /// <summary>コマンドを実行するワーキングディレクトリのパスを取得または設定します。</summary>
        public string WorkDir { get => _workDir; set => _workDir = value; }
        
        /// <summary>実行するコマンドまたはプログラムのパスを取得または設定します。</summary>
        public string CmdPath { get => _cmdPath; set => _cmdPath = value; }
        
        /// <summary>コマンドに渡す引数文字列を取得または設定します。</summary>
        public string CmdArgs { get => _cmdArgs; set => _cmdArgs = value; }
        
        /// <summary>エラー発生時のエラーメッセージを取得または設定します。</summary>
        public string ErrorMessage { get => _errorMessage; set => _errorMessage = value; }
        
        /// <summary>ログ出力時のプレフィックス文字列を取得または設定します。</summary>
        public string Prefix { get => _prefix; set => _prefix = value; }
        
        /// <summary>標準出力・標準エラー出力のエンコーディング名（デフォルト: "Shift_JIS"）を取得または設定します。</summary>
        public string Encoding { get => _encoding; set => _encoding = value; }
        
        /// <summary>別ユーザーとして実行する場合のドメイン名を取得または設定します。</summary>
        public string DomainName { get => _domainName; set => _domainName = value; }
        
        /// <summary>別ユーザーとして実行する場合のユーザー名を取得または設定します。</summary>
        public string Username { get => _username; set => _username = value; }
        
        /// <summary>別ユーザーとして実行する場合のパスワードを取得または設定します。</summary>
        public string Password { get => _password; set => _password = value; }
        
        /// <summary>カンマ区切りの環境変数設定文字列を取得または設定します。</summary>
        public string ProcessEnvCsv { get => _processEnvCsv; set => _processEnvCsv = value; }
        
        /// <summary>プロセスへ引き渡す標準入力文字列を取得または設定します。</summary>
        public string StdIn { get => _stdIn; set => _stdIn = value; }
        
        /// <summary>現在コマンドが実行中かどうかを示す値を取得または設定します。</summary>
        public bool IsRunning { get => _isRunning; set => _isRunning = value; }
        
        /// <summary>実行するコマンド文字列をログに出力するかどうかを示す値を取得または設定します。</summary>
        public bool IsShowCmd { get => _isShowCmd; set => _isShowCmd = value; }
        
        /// <summary>プロセスの出力をログに出力するかどうかを示す値を取得または設定します。</summary>
        public bool IsShowOutput { get => _isShowOutput; set => _isShowOutput = value; }
        
        /// <summary>プロセスの出力をログに出力しないかどうかを示す値を取得または設定します。</summary>
        public bool IsNotShowOutput { get => _isNotShowOutput; set => _isNotShowOutput = value; }
        
        /// <summary>プロセスの終了コードをログに出力するかどうかを示す値を取得または設定します。</summary>
        public bool IsShowExitCode { get => _isShowExitCode; set => _isShowExitCode = value; }
        
        /// <summary>プロセスの終了コードをログに出力しないかどうかを示す値を取得または設定します。</summary>
        public bool IsNotShowExitCode { get => _isNotShowExitCode; set => _isNotShowExitCode = value; }
        
        /// <summary>ログ出力の先頭にメソッド情報プレフィックスを付与するかどうかを示す値を取得または設定します。</summary>
        public bool IsInfoPrefix { get => _isInfoPrefix; set => _isInfoPrefix = value; }
        
        /// <summary>標準出力の各行の先頭にプレフィックスを付与するかどうかを示す値を取得または設定します。</summary>
        public bool IsStdoutPrefix { get => _isStdoutPrefix; set => _isStdoutPrefix = value; }
        
        /// <summary>プロセス起動失敗時にエラーダイアログを表示するかどうかを示す値を取得または設定します。</summary>
        public bool IsErrorDialog { get => _isErrorDialog; set => _isErrorDialog = value; }
        
        /// <summary>別ユーザーアカウントでプロセスを実行（Switch User）するかどうかを示す値を取得または設定します。</summary>
        public bool IsSu { get => _isSu; set => _isSu = value; }
        
        /// <summary>ShellExecuteを使用してプロセスを起動するかどうかを示す値を取得または設定します。</summary>
        public bool IsUseShellExecute { get => _isUseShellExecute; set => _isUseShellExecute = value; }
        
        /// <summary>新しいウィンドウを作成せずにプロセスを実行するかどうかを示す値を取得または設定します。</summary>
        public bool IsCreateNoWindow { get => _isCreateNoWindow; set => _isCreateNoWindow = value; }
        
        /// <summary>プロセスの起動時にユーザープロファイルを読み込むかどうかを示す値を取得または設定します。</summary>
        public bool IsLoadUserProfile { get => _isLoadUserProfile; set => _isLoadUserProfile = value; }
        
        /// <summary>管理者権限（RunAs）でプロセスを起動するかどうかを示す値を取得または設定します。</summary>
        public bool IsRunAs { get => _isRunAs; set => _isRunAs = value; }
        
        /// <summary>標準出力・標準エラーのリダイレクトを行わないかどうかを示す値を取得または設定します。</summary>
        public bool IsNoRedirect { get => _isNoRedirect; set => _isNoRedirect = value; }
        
        /// <summary>例外発生時にスタックトレースをログ出力するかどうかを示す値を取得または設定します。</summary>
        public bool IsStackTrace { get => _isStackTrace; set => _isStackTrace = value; }
        
        /// <summary>空行をログに出力するかどうかを示す値を取得または設定します。</summary>
        public bool IsShowEmptyLine { get => _isShowEmptyLine; set => _isShowEmptyLine = value; }
        
        /// <summary>ログ出力を抑制するかどうかを示す値を取得または設定します。</summary>
        public bool IsSilent { get => _isSilent; set => _isSilent = value; }
        
        /// <summary>設定された環境変数の一覧をログ出力するかどうかを示す値を取得または設定します。</summary>
        public bool IsShowEnvDic { get => _isShowEnvDic; set => _isShowEnvDic = value; }
        
        /// <summary>処理終了時に StringBuilder を自動クリアするかどうかを示す値を取得または設定します。</summary>
        public bool IsClearStringBuilder { get => _isClearStringBuilder; set => _isClearStringBuilder = value; }
        
        /// <summary>デフォルトのエラーログレベルを取得または設定します。</summary>
        public int DefaultErrorLogLevel { get => _defaultErrorLogLevel; set => _defaultErrorLogLevel = value; }
        
        /// <summary>ログの冗長出力レベルを取得または設定します。</summary>
        public int Verbose { get => _verbose; set => _verbose = value; }
        
        /// <summary>デバッグログの出力レベルを取得または設定します。</summary>
        public int DebugLevel { get => _debugLevel; set => _debugLevel = value; }
        
        /// <summary>標準出力のログレベルを取得または設定します。</summary>
        public int StdoutLevel { get => _stdoutLevel; set => _stdoutLevel = value; }
        
        /// <summary>コマンド実行のタイムアウト時間（秒単位）を取得または設定します。</summary>
        public int Timeout { get => _timeout; set => _timeout = value; }
        
        /// <summary>正常終了と判定する戻り値コードの CSV リストを取得または設定します。</summary>
        public string OkReturnCodeCsv { get => _cmdStatus.OkReturnCodeCsv; set => _cmdStatus.OkReturnCodeCsv = value; }
        
        /// <summary>警告終了と判定する戻り値コードの CSV リストを取得または設定します。</summary>
        public string WarnReturnCodeCsv { get => _cmdStatus.WarnReturnCodeCsv; set => _cmdStatus.WarnReturnCodeCsv = value; }
        
        /// <summary>エラー終了と判定する戻り値コードの CSV リストを取得または設定します。</summary>
        public string ErrorReturnCodeCsv { get => _cmdStatus.ErrorReturnCodeCsv; set => _cmdStatus.ErrorReturnCodeCsv = value; }
        
        /// <summary>正常とみなす出力メッセージの CSV パターンを取得または設定します。</summary>
        public string OkMessageCsv { get => _cmdStatus.OkMessageCsv; set => _cmdStatus.OkMessageCsv = value; }
        
        /// <summary>警告とみなす出力メッセージの CSV パターンを取得または設定します。</summary>
        public string WarnMessageCsv { get => _cmdStatus.WarnMessageCsv; set => _cmdStatus.WarnMessageCsv = value; }
        
        /// <summary>エラーとみなす出力メッセージの CSV パターンを取得または設定します。</summary>
        public string ErrorMessageCsv { get => _cmdStatus.ErrorMessageCsv; set => _cmdStatus.ErrorMessageCsv = value; }
        
        /// <summary>警告と判定する終了コードのしきい値を取得または設定します。</summary>
        public int WarnThreshold { get => _cmdStatus.WarnThreshold; set => _cmdStatus.WarnThreshold = value; }
        
        /// <summary>エラーと判定する終了コードのしきい値を取得または設定します。</summary>
        public int ErrorThreshold { get => _cmdStatus.ErrorThreshold; set => _cmdStatus.ErrorThreshold = value; }
        
        /// <summary>負の終了コードをエラーとして扱うかどうかを示す値を取得または設定します。</summary>
        public bool IsErrorAtNegativeValue { get => _cmdStatus.IsErrorAtNegativeValue; set => _cmdStatus.IsErrorAtNegativeValue = value; }
        
        /// <summary>常に正常終了として評価するかどうかを示す値を取得または設定します。</summary>
        public bool IsAlwaysNormal { get => _cmdStatus.IsAlwaysNormal; set => _cmdStatus.IsAlwaysNormal = value; }
        
        /// <summary>エラー時の戻り値コードを取得または設定します。</summary>
        public int ErrorCode { get => _cmdStatus.ErrorCode; set => _cmdStatus.ErrorCode = value; }
        
        /// <summary>警告時の戻り値コードを取得または設定します。</summary>
        public int WarnCode { get => _cmdStatus.WarnCode; set => _cmdStatus.WarnCode = value; }
        
        /// <summary>実行したコマンドの終了コードを取得または設定します。</summary>
        public int CmdExitStatus { get => _cmdExitStatus; set => _cmdExitStatus = value; }
        
        /// <summary>評価後の最終的なメソッド終了ステータスを取得または設定します。</summary>
        public int MethodExitStatus { get => _cmdStatus.MethodExitStatus; set => _cmdStatus.MethodExitStatus = value; }
        
        /// <summary>ログ出力のレベルを取得または設定します。</summary>
        public int ReturnLevel { get => _cmdStatus.ReturnLevel; set => _cmdStatus.ReturnLevel = value; }

        /// <summary>
        /// 内部状態およびコマンドステータス評価クラスの初期化を行います。
        /// </summary>
        /// <example>
        /// <code>
        /// var cmdExec = new ClsCmdExec(logger);
        /// cmdExec.Verbose = 2;
        /// cmdExec.Initialize();
        /// </code>
        /// </example>
        public void Initialize()
        {
            _cmdStatus.Verbose = _verbose;
            _cmdStatus.DebugLevel = _debugLevel;
        }

        /// <summary>
        /// 内部状態およびコマンドステータス評価クラスの初期化を行います。
        /// </summary>
        /// <remarks>このメソッドは旧仕様との互換性のために残されています。<see cref="Initialize"/> の使用を推奨します。</remarks>
        [Obsolete("代わりに 'Initialize()' を使用します。")]
        public void Init()
        {
            Initialize();
        }

        /// <summary>
        /// 指定された優先度で別スレッドを起動し、コマンドを実行します。
        /// </summary>
        /// <param name="priority">スレッドの優先度設定（0: リアルタイム, 1: 高, 2: 通常以上, 3: 通常, 4: 通常以下, 5: アイドル）</param>
        /// <returns>コマンド評価後のメソッド終了ステータス（0: 正常, それ以外: 警告またはエラー）</returns>
        /// <example>
        /// <code>
        /// var cmdExec = new ClsCmdExec(logger)
        /// {
        ///     CmdPath = "cmd.exe",
        ///     CmdArgs = "/c echo Hello World"
        /// };
        /// int exitCode = cmdExec.ExecuteThread(3);
        /// </code>
        /// </example>
        public int ExecuteThread(object priority)
        {
            string methodName = $"[ClsCmdExec.doThread()][{_prefix}]";
            if (string.IsNullOrEmpty(_encoding))
            {
                switch(_encodingObject.CodePage)
                {
                    case 65001:
                        _encoding = "UTF-8";
                        break;
                    default:
                        _encoding = "Shift_JIS";
                        break;
                }
            }
            _cmdExitStatus = -1;
            _cmdStatus.MethodExitStatus = (_cmdStatus.ErrorCode == MdlConst.INT_NULL ? MdlConst.LVL_E : _cmdStatus.ErrorCode);
            _cmdStatus.ReturnLevel = MdlConst.LVL_E;
            if (_isRunning)
            {
                _logger.WriteLine(_defaultErrorLogLevel, $"{methodName}[中止] 他の処理が実行中です。");
            }
            else
            {
                _isRunning = true;
                _thread = new Thread(ExecuteThreadWrapper);
                _thread.Start(priority);
                // スレッドの終了を待機
                _thread.Join();
                _isRunning = false;
            }
            return _cmdStatus.MethodExitStatus;
        }

        /// <summary>
        /// 指定された優先度で別スレッドを起動し、コマンドを実行します。
        /// </summary>
        /// <param name="priority">スレッドの優先度</param>
        /// <returns>メソッド終了ステータス</returns>
        /// <remarks>このメソッドは旧仕様との互換性のために残されています。<see cref="ExecuteThread(object)"/> の使用を推奨します。</remarks>
        [Obsolete("代わりに 'ExecuteThread(object priority)' を使用します。")]
        public int DoThread(object priority)
        {
            return ExecuteThread(priority);
        }

        /// <summary>
        /// 実行中のプロセスおよびスレッドを安全に強制終了・キャンセルします。
        /// </summary>
        /// <example>
        /// <code>
        /// var cmdExec = new ClsCmdExec(logger);
        /// // 別スレッド等で実行中の処理をキャンセル
        /// cmdExec.Cancel();
        /// </code>
        /// </example>
        public void Cancel()
        {
            if (_process is { HasExited: false } proc)
            {
                proc.Kill();
            }
            if (_thread is { IsAlive: true } th)
            {
                th.Interrupt();
            }
            _isRunning = false;
            _process?.Dispose();
        }

        /// <summary>
        /// 内部で出力結果保持に使用している <see cref="StringBuilder"/> の内容をクリアします。
        /// </summary>
        /// <example>
        /// <code>
        /// cmdExec.ClearStringBuilder();
        /// </code>
        /// </example>
        public void ClearStringBuilder()
        {
            _stringBuilder.Clear();
        }

        /// <summary>
        /// スレッド同期ロックを確立した上で <see cref="StringBuilder"/> の内容を安全にクリアします。
        /// </summary>
        /// <example>
        /// <code>
        /// cmdExec.ClearStringBuilderWithLock();
        /// </code>
        /// </example>
        public void ClearStringBuilderWithLock()
        {
            lock (_lockStringBuilder)
            {
                ClearStringBuilder();
            }
        }

        /// <summary>
        /// スレッド起動時のエントリポイントとなるラッパーメソッドです。
        /// </summary>
        /// <param name="priority">スレッドの優先度パラメータ</param>
        private void ExecuteThreadWrapper(object? priority)
        {
            try
            {
                ExecuteCore(priority ?? 3);
            }
            catch (ThreadInterruptedException)
            {
            }
        }

        /// <summary>
        /// コマンドプロセスを生成して実行し、標準出力・標準エラーのキャプチャおよび評価を行います。
        /// </summary>
        /// <param name="priority">プロセスの優先度指定</param>
        /// <returns>プロセスの起動および実行処理が成功した場合は true。それ以外は false。</returns>
        private bool ExecuteCore(object priority)
        {
            string methodName = $"[ClsCmdExec.execute()][{_prefix}]";
            bool isSuccess = true;
            if (_verbose > 4) _isShowOutput = true;
            if (_verbose > 3) _isShowExitCode = true;
            _cmdStatus.MethodExitStatus = (_cmdStatus.ErrorCode == MdlConst.INT_NULL ? MdlConst.LVL_E : _cmdStatus.ErrorCode);
            _cmdStatus.ReturnLevel = MdlConst.LVL_I;
            _cmdStatus.Initialize();
            _cmdStatus.ResetFlags();
            bool hasStandardInput = !string.IsNullOrEmpty(_stdIn);
            // 表示
            string output = $"{_cmdPath} {_cmdArgs}";
            if (_isInfoPrefix) output = $"{methodName} {output}";
            if (_isShowCmd || _verbose > 4) _logger.WriteLine(_debugLevel, output);
            
            // プロセスのオプション
            using Process process = new();
            _process = process;
            if (!string.IsNullOrEmpty(_workDir)) process.StartInfo.WorkingDirectory = _workDir;
            process.StartInfo.FileName = _cmdPath;
            process.StartInfo.Arguments = _cmdArgs;
            process.StartInfo.CreateNoWindow = _isCreateNoWindow;
            process.StartInfo.UseShellExecute = _isUseShellExecute;
            process.StartInfo.ErrorDialog = _isErrorDialog;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) process.StartInfo.LoadUserProfile = _isLoadUserProfile;
            if (_isNoRedirect)
            {
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;
            }
            else
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.StandardOutputEncoding = System.Text.Encoding.GetEncoding(_encoding);
                process.StartInfo.StandardErrorEncoding = System.Text.Encoding.GetEncoding(_encoding);
                process.OutputDataReceived += OnOutputDataReceived;
                process.ErrorDataReceived += OnErrorDataReceived;
            }
            process.StartInfo.RedirectStandardInput = hasStandardInput;
            
            // プロセス環境変数の設定
            if (!string.IsNullOrEmpty(_processEnvCsv))
            {
                _processEnvs = MdlUtil.ParseCsvToDictionary(_processEnvs, _processEnvCsv, @"[,|]", @"=", _verbose, true, false);
            }
            if (_processEnvs.Count > 0)
            {
                foreach (var (key, value) in _processEnvs)
                {
                    if (key.Equals("+PATH", StringComparison.OrdinalIgnoreCase))
                    {
                        string path = $"{value};{process.StartInfo.EnvironmentVariables["PATH"]}";
                        if (_isShowEnvDic) _logger.WriteLine(_debugLevel, $"[SETENV] PATH = {path}");
                        process.StartInfo.EnvironmentVariables["PATH"] = path;
                    }
                    else
                    {
                        if (_isShowEnvDic) _logger.WriteLine(_debugLevel, $"[SETENV] {key} = {value}");
                        process.StartInfo.EnvironmentVariables[key] = value;
                    }
                }
            }
            // Switch User
            if (_isSu && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SecureString securePasswd = new();
                process.StartInfo.Domain = _domainName;  // ローカルユーザの場合はnullを指定
                process.StartInfo.UserName = _username;
                if (!string.IsNullOrEmpty(_password))
                {
                    foreach (char c in _password)
                    {
                        securePasswd.AppendChar(c);
                    }
                    process.StartInfo.Password = securePasswd;
                }
            }
            // 管理者として実行
            if (_isRunAs) process.StartInfo.Verb = "RunAs";
            
            // プロセス（外部プログラム）起動
            try
            {
                // プロセス開始
                process.Start();
                // 優先度指定
                int prioInt = priority is int p ? p : 3;
                process.PriorityClass = prioInt switch
                {
                    0 => ProcessPriorityClass.RealTime,
                    1 => ProcessPriorityClass.High,
                    2 => ProcessPriorityClass.AboveNormal,
                    3 => ProcessPriorityClass.Normal,
                    4 => ProcessPriorityClass.BelowNormal,
                    5 => ProcessPriorityClass.Idle,
                    _ => ProcessPriorityClass.Normal
                };
                
                // 標準入力の書き込み
                if (hasStandardInput)
                {
                    using StreamWriter sw = process.StandardInput;
                    sw.Write(_stdIn);
                }
                // 出力
                if (!_isNoRedirect)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }
                // プロセスが終了待ち
                if (!process.WaitForExit(_timeout * 1000))
                {
                    _cmdStatus.MethodExitStatus = (_cmdStatus.ErrorCode == MdlConst.INT_NULL ? MdlConst.LVL_E : _cmdStatus.ErrorCode);
                    _cmdStatus.ReturnLevel = MdlConst.LVL_E;
                    isSuccess = false;
                    _errorMessage = $"{methodName} TIMEOUT : {_timeout}秒 => KILL()";
                    _logger.WriteLine(_defaultErrorLogLevel, _errorMessage);
                    process.Kill();
                }
                // コマンド終了コードの値取得
                _cmdExitStatus = process.ExitCode;
                // コマンド終了コードの判定
                if (_cmdStatus.ShouldCheckMessage())
                {
                    foreach (string line in _stringBuilder.ToString().Split([Environment.NewLine], StringSplitOptions.None))
                    {
                        _cmdStatus.CheckMessageLine(line);
                    }
                }
                _cmdStatus.CheckCommandExitCode(_cmdExitStatus);
                _cmdStatus.Evaluate();
                // ログ出力
                if (_isShowOutput || (!_isNotShowOutput && 0 != _cmdExitStatus))
                {
                    output = _stringBuilder.ToString().Trim();
                    foreach (string line in output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
                    {
                        string show = _isStdoutPrefix ? $"{methodName} {line}" : line;
                        if (_isShowEmptyLine || !string.IsNullOrEmpty(line))
                        {
                            _logger.WriteLine(_stdoutLevel, show);
                        }
                    }
                }
                if (_isShowExitCode || (!_isNotShowExitCode && 0 != _cmdExitStatus))
                {
                    output = $"コマンド終了コード = {_cmdExitStatus} => メソッド終了コード = {_cmdStatus.MethodExitStatus}";
                    if (_isInfoPrefix) output = $"{methodName} {output}";
                    _logger.WriteLine(_debugLevel, output);
                }
            }
            catch (Exception ex)
            {
                _cmdStatus.MethodExitStatus = (_cmdStatus.ErrorCode == MdlConst.INT_NULL ? MdlConst.LVL_E : _cmdStatus.ErrorCode);
                _cmdStatus.ReturnLevel = MdlConst.LVL_E;
                isSuccess = false;
                _errorMessage = $"{methodName} EXCEPTION : {ex.Message}";
                _logger.WriteLine(_defaultErrorLogLevel, _errorMessage);
                if (_isStackTrace)
                {
                    _logger.WriteLine(MdlConst.LVL_NONE, "");
                    _logger.WriteLine(MdlConst.LVL_NONE, ex.StackTrace ?? "");
                    _logger.WriteLine(MdlConst.LVL_NONE, "");
                }
            }
            finally
            {
                _process = null;
                lock (_lockStringBuilder)
                {
                    if (_isClearStringBuilder) ClearStringBuilder();
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// 標準出力データ受信イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">受信したデータを含むイベント引数</param>
        private static void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (_lockStringBuilder)
            {
                _stringBuilder.AppendLine(e.Data);
            }
        }

        /// <summary>
        /// 標準エラーデータ受信イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">受信したデータを含むイベント引数</param>
        private static void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (_lockStringBuilder)
            {
                _stringBuilder.AppendLine(e.Data);
            }
        }

        /// <summary>
        /// 指定されたパスの環境変数定義ファイルを読み込み、環境変数辞書に登録します。
        /// </summary>
        /// <param name="filePath">環境変数定義ファイルのパス</param>
        /// <returns>ファイルの読み込みおよび登録に成功した場合は true。ファイルが存在しない場合は false。</returns>
        /// <example>
        /// <code>
        /// var cmdExec = new ClsCmdExec(logger);
        /// bool success = cmdExec.ReadEnvironmentDefinitionFile(@"C:\config\env.txt");
        /// </code>
        /// </example>
        public bool ReadEnvironmentDefinitionFile(string filePath)
        {
            string methodName = $"[ClsCmdExec.ReadEnvDefFile()][{_prefix}]";
            bool isSuccess = true;
            string absoluteFilePath = MdlFile.GetAbsolutePath(filePath.Trim());
            if (MdlFile.PathExists(absoluteFilePath))
            {
                ClsConfigFile configFile = new(_logger)
                {
                    ConfigDictionary = _processEnvs,
                    Verbose = _verbose,
                    Pattern = "^(?<KEY>[^#=]+)=(?<VALUE>.+)$"
                };
                configFile.LoadToDictionary(absoluteFilePath);
            }
            else
            {
                isSuccess = false;
                _errorMessage = $"{methodName} NO SUCH A FILE : {absoluteFilePath}";
                _logger.WriteLine(_defaultErrorLogLevel, _errorMessage);
            }
            return isSuccess;
        }

        /// <summary>
        /// 指定されたパスの環境変数定義ファイルを読み込み、環境変数辞書に登録します。
        /// </summary>
        /// <param name="filePath">環境変数定義ファイルのパス</param>
        /// <returns>読み込み成功時 true</returns>
        /// <remarks>このメソッドは旧仕様との互換性のために残されています。<see cref="ReadEnvironmentDefinitionFile(string)"/> の使用を推奨します。</remarks>
        [Obsolete("代わりに 'ReadEnvironmentDefinitionFile(string filePath)' を使用します。")]
        public bool ReadEnvDefFile(string filePath)
        {
            return ReadEnvironmentDefinitionFile(filePath);
        }

    }
}
