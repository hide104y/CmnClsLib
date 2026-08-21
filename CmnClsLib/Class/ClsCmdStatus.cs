using CmnClsLib.Module;
using CmnClsLib.Interface;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Class
{
    /// <summary>
    /// コマンド実行状態の管理および終了コード・出力ログメッセージの判定を行うクラス。
    /// </summary>
    /// <param name="logger">ログ出力用のロガーインスタンス</param>
    /// <example>
    /// <code>
    /// var logger = new CmnLogger();
    /// var cmdStatus = new ClsCmdStatus(logger)
    /// {
    ///     OkReturnCodeCsv = "0",
    ///     WarnReturnCodeCsv = "1",
    ///     ErrorReturnCodeCsv = "2,3"
    /// };
    /// cmdStatus.Initialize();
    /// cmdStatus.CheckCommandExitCode(0);
    /// </code>
    /// </example>
    public class ClsCmdStatus(ICmnLogger logger)
    {
        private readonly ICmnLogger _logger = logger;

        /// <summary>
        /// 冗長出力レベルを取得または設定します。
        /// </summary>
        public int Verbose { get; set; } = 0;

        /// <summary>
        /// DEBUGログの出力レベルを取得または設定します。
        /// </summary>
        public int DebugLevel { get; set; } = MdlConst.LVL_DEBUG;

        /// <summary>
        /// 正常終了コード判定用のCSV文字列を取得または設定します。
        /// </summary>
        public string OkReturnCodeCsv { get; set; } = "0";

        /// <summary>
        /// 警告終了コード判定用のCSV文字列を取得または設定します。
        /// </summary>
        public string WarnReturnCodeCsv { get; set; } = "";

        /// <summary>
        /// 異常終了コード判定用のCSV文字列を取得または設定します。
        /// </summary>
        public string ErrorReturnCodeCsv { get; set; } = "";

        /// <summary>
        /// 正常終了判定文字列のCSV文字列を取得または設定します。
        /// </summary>
        public string OkMessageCsv { get; set; } = "";

        /// <summary>
        /// 警告終了判定文字列のCSV文字列を取得または設定します。
        /// </summary>
        public string WarnMessageCsv { get; set; } = "";

        /// <summary>
        /// 異常終了判定文字列のCSV文字列を取得または設定します。
        /// </summary>
        public string ErrorMessageCsv { get; set; } = "";

        /// <summary>
        /// メソッドの評価終了ステータスコードを取得または設定します。
        /// </summary>
        public int MethodExitStatus { get; set; } = 0;

        /// <summary>
        /// エラーレベルを取得または設定します。
        /// </summary>
        public int ReturnLevel { get; set; } = MdlConst.LVL_I;

        /// <summary>
        /// エラー時の戻り値コードを取得または設定します。
        /// </summary>
        public int ErrorCode { get; set; } = MdlConst.INT_NULL;

        /// <summary>
        /// 警告時の戻り値コードを取得または設定します。
        /// </summary>
        public int WarnCode { get; set; } = MdlConst.INT_NULL;

        /// <summary>
        /// 警告判定の閾値を取得または設定します。
        /// </summary>
        public int WarnThreshold { get; set; } = MdlConst.INT_NULL;

        /// <summary>
        /// 異常判定の閾値を取得または設定します。
        /// </summary>
        public int ErrorThreshold { get; set; } = MdlConst.INT_NULL;

        /// <summary>
        /// 負の戻り値が返ってきた場合にエラー判定とするかどうかのフラグを取得または設定します。
        /// </summary>
        public bool IsErrorAtNegativeValue { get; set; } = false;

        /// <summary>
        /// 常に正常終了判定とするかどうかのフラグを取得または設定します。
        /// </summary>
        public bool IsAlwaysNormal { get; set; } = false;

        // 単体テストで確認用 ==============================
        /// <summary>
        /// 正常終了判定文字列のリストを取得または設定します。
        /// </summary>
        public List<string> OkMessageList { get; set; } = [];

        /// <summary>
        /// 警告終了判定文字列のリストを取得または設定します。
        /// </summary>
        public List<string> WarnMessageList { get; set; } = [];

        /// <summary>
        /// 異常終了判定文字列のリストを取得または設定します。
        /// </summary>
        public List<string> ErrorMessageList { get; set; } = [];

        /// <summary>
        /// 正常終了判定コードのリストを取得または設定します。
        /// </summary>
        public List<int> OkReturnCodeList { get; set; } = [];

        /// <summary>
        /// 警告終了判定コードのリストを取得または設定します。
        /// </summary>
        public List<int> WarnReturnCodeList { get; set; } = [];

        /// <summary>
        /// 異常終了判定コードのリストを取得または設定します。
        /// </summary>
        public List<int> ErrorReturnCodeList { get; set; } = [];

        /// <summary>
        /// 正常判定文字列にヒットしたかどうかのフラグを取得または設定します。
        /// </summary>
        public bool IsOkMessageHit { get; set; } = false;

        /// <summary>
        /// 警告判定文字列にヒットしたかどうかのフラグを取得または設定します。
        /// </summary>
        public bool IsWarnMessageHit { get; set; } = false;

        /// <summary>
        /// 異常判定文字列にヒットしたかどうかのフラグを取得または設定します。
        /// </summary>
        public bool IsErrorMessageHit { get; set; } = false;

        /// <summary>
        /// 設定されたCSV文字列を解析し、各判定用コードリストおよびメッセージリストを生成して初期化します。
        /// </summary>
        /// <example>
        /// <code>
        /// cmdStatus.Initialize();
        /// </code>
        /// </example>
        public void Initialize()
        {
            OkReturnCodeList.Clear();
            WarnReturnCodeList.Clear();
            ErrorReturnCodeList.Clear();
            OkMessageList.Clear();
            WarnMessageList.Clear();
            ErrorMessageList.Clear();

            if (!string.IsNullOrEmpty(OkReturnCodeCsv)) OkReturnCodeList = MdlUtil.ParseCsvToIntList(null, OkReturnCodeCsv);
            if (!string.IsNullOrEmpty(WarnReturnCodeCsv)) WarnReturnCodeList = MdlUtil.ParseCsvToIntList(null, WarnReturnCodeCsv);
            if (!string.IsNullOrEmpty(ErrorReturnCodeCsv)) ErrorReturnCodeList = MdlUtil.ParseCsvToIntList(null, ErrorReturnCodeCsv);
            if (!string.IsNullOrEmpty(OkMessageCsv)) OkMessageList = MdlUtil.ParseCsvToList(null, OkMessageCsv);
            if (!string.IsNullOrEmpty(WarnMessageCsv)) WarnMessageList = MdlUtil.ParseCsvToList(null, WarnMessageCsv);
            if (!string.IsNullOrEmpty(ErrorMessageCsv)) ErrorMessageList = MdlUtil.ParseCsvToList(null, ErrorMessageCsv);
        }

        /// <summary>
        /// 【旧形式】初期化を行います。（代わりに <see cref="Initialize"/> を使用してください）
        /// </summary>
        /// <example>
        /// <code>
        /// cmdStatus.Init();
        /// </code>
        /// </example>
        [Obsolete("代わりに 'Initialize()' を使用します。")]
        public void Init() => Initialize();

        /// <summary>
        /// 出力文字列のチェック判定が必要であるか（判定用メッセージリストが1つ以上設定されているか）を確認します。
        /// </summary>
        /// <returns>メッセージの判定が必要な場合は <c>true</c>。それ以外は <c>false</c>。</returns>
        /// <example>
        /// <code>
        /// if (cmdStatus.ShouldCheckMessage())
        /// {
        ///     cmdStatus.CheckMessageLine(line);
        /// }
        /// </code>
        /// </example>
        public bool ShouldCheckMessage()
        {
            return OkMessageList.Count > 0 || WarnMessageList.Count > 0 || ErrorMessageList.Count > 0;
        }

        /// <summary>
        /// 【旧形式】出力文字列のチェック必要有無を確認します。（代わりに <see cref="ShouldCheckMessage"/> を使用してください）
        /// </summary>
        /// <returns>メッセージ判定が必要な場合は <c>true</c>。それ以外は <c>false</c>。</returns>
        /// <example>
        /// <code>
        /// bool needCheck = cmdStatus.IsCheckMessage();
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ShouldCheckMessage()' を使用します。")]
        public bool IsCheckMessage() => ShouldCheckMessage();

        /// <summary>
        /// メッセージのマッチ判定フラグ（IsOkMessageHit, IsWarnMessageHit, IsErrorMessageHit）を初期化（クリア）します。
        /// </summary>
        /// <example>
        /// <code>
        /// cmdStatus.ResetFlags();
        /// </code>
        /// </example>
        public void ResetFlags()
        {
            IsOkMessageHit = false;
            IsWarnMessageHit = false;
            IsErrorMessageHit = false;
        }

        /// <summary>
        /// 【旧形式】フラグの初期化を行います。（代わりに <see cref="ResetFlags"/> を使用してください）
        /// </summary>
        /// <example>
        /// <code>
        /// cmdStatus.InitFlags();
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ResetFlags()' を使用します。")]
        public void InitFlags() => ResetFlags();

        /// <summary>
        /// コマンドからの出力ログ文字列（1行）を検査し、設定された正常・警告・異常メッセージとマッチするか判定します。
        /// </summary>
        /// <param name="line">チェック対象のログ出力文字列</param>
        /// <example>
        /// <code>
        /// cmdStatus.CheckMessageLine("INFO: Operation completed successfully.");
        /// </code>
        /// </example>
        public void CheckMessageLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return;

            if (!IsOkMessageHit)
            {
                foreach (string pattern in OkMessageList)
                {
                    if (line.Contains(pattern))
                    {
                        if (Verbose > 4) _logger.WriteLine(DebugLevel, $"[HIT] OkStr : [{pattern}] in [{line}]");
                        IsOkMessageHit = true;
                        break;
                    }
                    else
                    {
                        if (Verbose > 6) _logger.WriteLine(DebugLevel, $"[NOHIT] OkStr : [{pattern}] in [{line}]");
                    }
                }
            }

            if (!IsWarnMessageHit)
            {
                foreach (string pattern in WarnMessageList)
                {
                    if (line.Contains(pattern))
                    {
                        if (Verbose > 4) _logger.WriteLine(DebugLevel, $"[HIT] WarnStr : [{pattern}] in [{line}]");
                        IsWarnMessageHit = true;
                        break;
                    }
                    else
                    {
                        if (Verbose > 6) _logger.WriteLine(DebugLevel, $"[NOHIT] WarnStr : [{pattern}] in [{line}]");
                    }
                }
            }

            if (!IsErrorMessageHit)
            {
                foreach (string pattern in ErrorMessageList)
                {
                    if (line.Contains(pattern))
                    {
                        if (Verbose > 4) _logger.WriteLine(DebugLevel, $"[HIT] NgStr : [{pattern}] in [{line}]");
                        IsErrorMessageHit = true;
                        break;
                    }
                    else
                    {
                        if (Verbose > 6) _logger.WriteLine(DebugLevel, $"[NOHIT] NgStr : [{pattern}] in [{line}]");
                    }
                }
            }
        }

        /// <summary>
        /// コマンドの終了コードを検証し、閾値や設定されたリストに基づき終了ステータス (<see cref="MethodExitStatus"/>) およびエラーレベル (<see cref="ReturnLevel"/>) を評価・設定します。
        /// </summary>
        /// <param name="exitCode">コマンドの実行完了コード</param>
        /// <example>
        /// <code>
        /// cmdStatus.CheckCommandExitCode(0);
        /// </code>
        /// </example>
        public void CheckCommandExitCode(int exitCode)
        {
            // 終了コード判定：その１：閾値が設定されていない場合
            if (WarnThreshold == MdlConst.INT_NULL && ErrorThreshold == MdlConst.INT_NULL && !IsAlwaysNormal)
            {
                if (exitCode == 0)
                {
                    MethodExitStatus = MdlConst.LVL_I;
                    ReturnLevel = MdlConst.LVL_I;
                }
                else
                {
                    MethodExitStatus = (ErrorCode == MdlConst.INT_NULL ? exitCode : ErrorCode);
                    ReturnLevel = MdlConst.LVL_E;
                }
            }
            // 終了コード判定：その１：閾値が設定されている場合
            else
            {
                MethodExitStatus = MdlConst.LVL_I;
                ReturnLevel = MdlConst.LVL_I;

                if (IsAlwaysNormal)
                {
                    WarnThreshold = MdlConst.INT_NULL;
                    ErrorThreshold = MdlConst.INT_NULL;
                }
                if (IsErrorAtNegativeValue && exitCode < 0)
                {
                    MethodExitStatus = MdlConst.LVL_E;
                    ReturnLevel = MdlConst.LVL_E;
                }
            }

            // 終了コード判定：その２：警告閾値が設定されている場合
            if (WarnThreshold != MdlConst.INT_NULL && exitCode > WarnThreshold)
            {
                MethodExitStatus = MdlConst.LVL_W;
                ReturnLevel = MdlConst.LVL_W;
            }

            // 終了コード判定：その２：異常閾値が設定されている場合
            if (ErrorThreshold != MdlConst.INT_NULL && exitCode > ErrorThreshold)
            {
                MethodExitStatus = MdlConst.LVL_E;
                ReturnLevel = MdlConst.LVL_E;
            }

            // 終了コード判定：その３：正常閾値リストを確認
            foreach (int check in OkReturnCodeList)
            {
                if (check == exitCode)
                {
                    if (Verbose > 4) _logger.WriteLine(DebugLevel, $"[HIT] OkRetCd : {check}");
                    MethodExitStatus = MdlConst.LVL_I;
                    ReturnLevel = MdlConst.LVL_I;
                    break;
                }
                else
                {
                    if (Verbose > 6) _logger.WriteLine(DebugLevel, $"[NOHIT] OkRetCd : {check}");
                }
            }

            // 終了コード判定：その３：警告閾値リストを確認
            foreach (int check in WarnReturnCodeList)
            {
                if (check == exitCode)
                {
                    if (Verbose > 4) _logger.WriteLine(DebugLevel, $"[HIT] WarnRetCd : {check}");
                    MethodExitStatus = (WarnCode == MdlConst.INT_NULL ? exitCode : WarnCode);
                    ReturnLevel = MdlConst.LVL_W;
                    break;
                }
                else
                {
                    if (Verbose > 6) _logger.WriteLine(DebugLevel, $"[NOHIT] WarnRetCd : {check}");
                }
            }

            // 終了コード判定：その３：異常閾値リストを確認
            foreach (int check in ErrorReturnCodeList)
            {
                if (check == exitCode)
                {
                    if (Verbose > 4) _logger.WriteLine(DebugLevel, $"[HIT] NgRetCd : {check}");
                    MethodExitStatus = (ErrorCode == MdlConst.INT_NULL ? exitCode : ErrorCode);
                    ReturnLevel = MdlConst.LVL_E;
                    break;
                }
                else
                {
                    if (Verbose > 6) _logger.WriteLine(DebugLevel, $"[NOHIT] NgRetCd : {check}");
                }
            }
        }

        /// <summary>
        /// 【旧形式】コマンド終了コードをチェックします。（代わりに <see cref="CheckCommandExitCode(int)"/> を使用してください）
        /// </summary>
        /// <param name="intCmdExitCode">コマンドの実行完了コード</param>
        /// <example>
        /// <code>
        /// cmdStatus.CheckCmdExitCode(0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'CheckCommandExitCode(int exitCode)' を使用します。")]
        public void CheckCmdExitCode(int intCmdExitCode) => CheckCommandExitCode(intCmdExitCode);

        /// <summary>
        /// 出力ログメッセージの判定ヒット状況に基づき、最終的な終了ステータス (<see cref="MethodExitStatus"/>) およびエラーレベル (<see cref="ReturnLevel"/>) を評価・決定します。
        /// </summary>
        /// <example>
        /// <code>
        /// cmdStatus.Evaluate();
        /// </code>
        /// </example>
        public void Evaluate()
        {
            if (OkMessageList.Count > 0)
            {
                if (IsOkMessageHit)
                {
                    MethodExitStatus = MdlConst.LVL_I;
                    ReturnLevel = MdlConst.LVL_I;
                }
                else
                {
                    if (ErrorCode != MdlConst.INT_NULL) MethodExitStatus = ErrorCode;
                    if (MethodExitStatus == MdlConst.LVL_I) MethodExitStatus = MdlConst.LVL_E;
                    ReturnLevel = MdlConst.LVL_E;
                }
            }
            if (WarnMessageList.Count > 0 && IsWarnMessageHit)
            {
                if (WarnCode != MdlConst.INT_NULL) MethodExitStatus = WarnCode;
                if (MethodExitStatus == MdlConst.LVL_I) MethodExitStatus = MdlConst.LVL_W;
                ReturnLevel = MdlConst.LVL_W;
            }
            if (ErrorMessageList.Count > 0 && IsErrorMessageHit)
            {
                if (ErrorCode != MdlConst.INT_NULL) MethodExitStatus = ErrorCode;
                if (MethodExitStatus == MdlConst.LVL_I) MethodExitStatus = MdlConst.LVL_E;
                ReturnLevel = MdlConst.LVL_E;
            }
        }
    }
}
