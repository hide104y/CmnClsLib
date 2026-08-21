using System;
using System.Text.RegularExpressions;
using CmnClsLib.Interface;
using CmnClsLib.Module;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Class
{
    /// <summary>
    /// JP1/AJS3 ジョブの環境変数取得および文字列変換処理を提供するクラスです。
    /// </summary>
    public class ClsJp1Job
    {
        // 変数
        private readonly ICmnLogger _logger;
        private string _jobName = "";
        private string _prefix = @"AJSENV\.";
        private string _pattern = @"\/|__|\.\.";
        private int _verbose = 0;
        private bool _isAjsJob = false;
        private bool _isSilent = false;

        /// <summary>
        /// <see cref="ClsJp1Job"/> クラスの新しいインスタンスを初期化し、JP1ジョブ環境変数を読み込みます。
        /// </summary>
        /// <param name="logger">ログ出力用のロガーインスタンス</param>
        /// <example>
        /// <code>
        /// ICmnLogger logger = new CmnLogger();
        /// var jp1Job = new ClsJp1Job(logger);
        /// </code>
        /// </example>
        public ClsJp1Job(ICmnLogger logger)
        {
            _logger = logger;
            LoadEnvironmentVariables();
        }

        /// <summary>
        /// 置換キーのプレフィックス文字列を取得または設定します。
        /// </summary>
        /// <example>
        /// <code>
        /// jp1Job.Prefix = @"AJSENV\.";
        /// </code>
        /// </example>
        public string Prefix { get => _prefix; set => _prefix = value; }

        /// <summary>
        /// ジョブ名を分解するための正規表現パターンを取得または設定します。
        /// </summary>
        /// <example>
        /// <code>
        /// jp1Job.Pattern = @"\/|__|\.\.";
        /// </code>
        /// </example>
        public string Pattern { get => _pattern; set => _pattern = value; }

        /// <summary>
        /// JP1ジョブ名を取得または設定します。
        /// </summary>
        /// <example>
        /// <code>
        /// jp1Job.JobName = "JOB_SAMPLE";
        /// </code>
        /// </example>
        public string JobName { get => _jobName; set => _jobName = value; }

        /// <summary>
        /// 詳細ログ出力レベルを取得または設定します。
        /// </summary>
        /// <example>
        /// <code>
        /// jp1Job.Verbose = 5;
        /// </code>
        /// </example>
        public int Verbose { get => _verbose; set => _verbose = value; }

        /// <summary>
        /// JP1/AJS3 ジョブ環境変数が有効かどうかを示す値を取得または設定します。
        /// </summary>
        /// <example>
        /// <code>
        /// bool isAjs = jp1Job.IsAjsJob;
        /// </code>
        /// </example>
        public bool IsAjsJob { get => _isAjsJob; set => _isAjsJob = value; }

        /// <summary>
        /// ログ出力を抑制するかどうかを示す値を取得または設定します。
        /// </summary>
        /// <example>
        /// <code>
        /// jp1Job.IsSilent = true;
        /// </code>
        /// </example>
        public bool IsSilent { get => _isSilent; set => _isSilent = value; }

        /// <summary>
        /// JP1/AJS3 の環境変数 [AJSJOBNAME] の値を取得し、クラス内に保持します。
        /// </summary>
        /// <returns>環境変数が設定されており、ジョブ名が取得できた場合は true。それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool isLoaded = jp1Job.LoadEnvironmentVariables();
        /// </code>
        /// </example>
        public bool LoadEnvironmentVariables()
        {
            string envValue = System.Environment.GetEnvironmentVariable("AJSJOBNAME") ?? "";
            _isAjsJob = false;
            if (!string.IsNullOrEmpty(envValue))
            {
                _jobName = envValue;
                _isAjsJob = true;
            }
            return _isAjsJob;
        }

        /// <summary>
        /// JP1/AJS3 の環境変数 [AJSJOBNAME] を取得します。（旧方式）
        /// </summary>
        /// <returns>環境変数が設定されている場合は true。それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool isSet = jp1Job.GetEnv();
        /// </code>
        /// </example>
        [Obsolete("代わりに 'LoadEnvironmentVariables()' を使用します。")]
        public bool GetEnv()
        {
            return LoadEnvironmentVariables();
        }

        /// <summary>
        /// 指定されたジョブ名を保持し、プロセス環境変数 [AJSJOBNAME] に設定します。
        /// </summary>
        /// <param name="jobName">設定するJP1ジョブ名</param>
        /// <returns>環境変数の設定処理が成功した場合は true。失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool isSuccess = jp1Job.SetEnvironmentVariable("JOB_001");
        /// </code>
        /// </example>
        public bool SetEnvironmentVariable(string jobName)
        {
            if (!string.IsNullOrEmpty(jobName))
            {
                _jobName = jobName;
                _isAjsJob = true;
            }
            return SetEnvironmentVariable();
        }

        /// <summary>
        /// 指定されたジョブ名を保持し、プロセス環境変数 [AJSJOBNAME] に設定します。（旧方式）
        /// </summary>
        /// <param name="jobName">設定するJP1ジョブ名</param>
        /// <returns>環境変数の設定が成功した場合は true。失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool isSuccess = jp1Job.SetEnv("JOB_001");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'SetEnvironmentVariable(string)' を使用します。")]
        public bool SetEnv(string jobName)
        {
            return SetEnvironmentVariable(jobName);
        }

        /// <summary>
        /// 現在保持しているジョブ名をプロセス環境変数 [AJSJOBNAME] に設定します。
        /// </summary>
        /// <returns>環境変数の設定処理が成功した場合は true。失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool isSuccess = jp1Job.SetEnvironmentVariable();
        /// </code>
        /// </example>
        public bool SetEnvironmentVariable()
        {
            bool isSuccess = true;
            try
            {
                if (!string.IsNullOrEmpty(_jobName))
                {
                    System.Environment.SetEnvironmentVariable("AJSJOBNAME", _jobName, EnvironmentVariableTarget.Process);
                }
            }
            catch
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 現在保持しているジョブ名をプロセス環境変数 [AJSJOBNAME] に設定します。（旧方式）
        /// </summary>
        /// <returns>環境変数の設定が成功した場合は true。失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool isSuccess = jp1Job.SetEnv();
        /// </code>
        /// </example>
        [Obsolete("代わりに 'SetEnvironmentVariable()' を使用します。")]
        public bool SetEnv()
        {
            return SetEnvironmentVariable();
        }

        /// <summary>
        /// 置換対象文字列に含まれるJP1環境変数キーを、環境変数名 [AJSJOBNAME] から抽出した値に変換します。
        /// </summary>
        /// <param name="replaceTarget">環境変数キーを含む置換対象文字列</param>
        /// <returns>変換後の文字列（キーが存在しない場合やヒットしなかった場合は元の文字列）</returns>
        /// <example>
        /// <code>
        /// string converted = jp1Job.ConvertStringFromEnvironment("Select * from AJSENV.ID_A");
        /// </code>
        /// </example>
        public string ConvertStringFromEnvironment(string replaceTarget)
        {
            string methodName = "[ClsJp1Job.ConvertStringFromEnvironment()] ";
            string result = replaceTarget;
            string hit = "";
            if (string.IsNullOrEmpty(_jobName)) return result;

            if (_verbose > 4)
            {
                _logger.WriteLine(MdlConst.LVL_NONE, $"{methodName}EXEC GetRegexTarget({replaceTarget},{_prefix}(?<TARGET>[a-zA-Z0-9_-]+))");
            }

            string key = MdlUtil.GetRegexTarget(replaceTarget, $"{_prefix}(?<TARGET>[a-zA-Z0-9_-]+)");

            // キーが見つかった場合
            if (!string.IsNullOrEmpty(key))
            {
                if (_verbose > 4)
                {
                    _logger.WriteLine(MdlConst.LVL_NONE, $"{methodName}KEY FOUND = {key}");
                }

                //  環境変数を「/」または「__」または「..」で分解
                //    /ENV.PROD/JOB_SQLF.list_schema.sql__ID_A.10__ID_B.20.sq__ID_C.30.dq/I52.ADサーバ再起動/I22.再起動.RHOST.WEB-SV01/AAA
                Regex regex = new Regex(_pattern);
                foreach (string element in regex.Split(_jobName))
                {
                    string unit = element.Trim();
                    if (!string.IsNullOrEmpty(unit))
                    {
                        string extractedValue = MdlUtil.GetRegexTarget(unit, $@"^{key}\.(?<TARGET>.+)$");
                        if (string.IsNullOrEmpty(extractedValue))
                        {
                            extractedValue = MdlUtil.GetRegexTarget(unit, $@"\.{key}\.(?<TARGET>.+)$");
                        }
                        if (!string.IsNullOrEmpty(extractedValue))
                        {
                            hit = extractedValue;
                        }
                    }
                }

                // 置換対象文字列が見つかった場合
                if (!string.IsNullOrEmpty(hit))
                {
                    //  クォーテーション
                    if (hit.EndsWith(".sq", StringComparison.OrdinalIgnoreCase))
                    {
                        hit = $"'{hit[..^3]}'";
                    }
                    else if (hit.EndsWith(".dq", StringComparison.OrdinalIgnoreCase))
                    {
                        hit = $"\"{hit[..^3]}\"";
                    }

                    // 置換処理
                    if (_verbose > 0)
                    {
                        _logger.WriteLine(MdlConst.LVL_NONE, $"{methodName}[CONVERT] {_prefix}{key} => {hit}");
                    }
                    else
                    {
                        WriteLine(MdlConst.LVL_NONE, $"{methodName}[CONVERT] {_prefix}{key} => {hit}");
                    }
                    result = hit;
                }
                else
                {
                    if (_verbose > 0)
                    {
                        _logger.WriteLine(MdlConst.LVL_NONE, $"{methodName}[NOHIT] {_prefix}{key}");
                    }
                    else
                    {
                        WriteLine(MdlConst.LVL_NONE, $"{methodName}[NOHIT] {_prefix}{key}");
                    }
                }
            }
            else
            {
                if (_verbose > 4)
                {
                    _logger.WriteLine(MdlConst.LVL_NONE, $"{methodName}KEY NOT FOUND");
                }
            }
            return result;
        }

        /// <summary>
        /// 置換対象文字列に含まれるJP1環境変数キーを、環境変数名 [AJSJOBNAME] から抽出した値に変換します。（旧方式）
        /// </summary>
        /// <param name="replaceTarget">環境変数キーを含む置換対象文字列</param>
        /// <returns>変換後の文字列</returns>
        /// <example>
        /// <code>
        /// string converted = jp1Job.ConvStrFrmEnv("Select * from AJSENV.ID_A");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ConvertStringFromEnvironment(string)' を使用します。")]
        public string ConvStrFrmEnv(string replaceTarget)
        {
            return ConvertStringFromEnvironment(replaceTarget);
        }

        /// <summary>
        /// 指定されたレベルとメッセージでログを出力します（サイレントモード時は出力しません）。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="message">出力メッセージ</param>
        /// <example>
        /// <code>
        /// jp1Job.WriteLine(1, "処理が開始されました。");
        /// </code>
        /// </example>
        public void WriteLine(int level, string message)
        {
            if (_isSilent) return;
            _logger.WriteLine(level, message);
        }

        /// <summary>
        /// 指定されたレベルとメッセージでログを出力します（旧方式）。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="message">出力メッセージ</param>
        /// <example>
        /// <code>
        /// jp1Job.Writeln(1, "処理が開始されました。");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'WriteLine(int, string)' を使用します。")]
        public void Writeln(int level, string message)
        {
            WriteLine(level, message);
        }

    }
}
