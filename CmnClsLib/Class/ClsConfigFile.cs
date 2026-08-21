using System;
using System.Text;
using System.Text.RegularExpressions;
using CmnClsLib.Module;
using CmnClsLib.Interface;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Class
{
    /// <summary>
    /// 設定ファイル（Key=Value形式および行リスト形式）を読み込み、解析・保持するためのクラスです。
    /// </summary>
    /// <example>
    /// <code>
    /// var logger = new CmnLogger();
    /// var configFile = new ClsConfigFile(logger);
    /// int loadedCount = configFile.LoadToDictionary(@"C:\config.ini");
    /// string value = configFile.ConfigDictionary["SettingKey"];
    /// </code>
    /// </example>
    public partial class ClsConfigFile
    {
        [GeneratedRegex(@"^\s*#")]
        private static partial Regex GeneratedRegexComment();

        [GeneratedRegex(@"^\s*$")]
        private static partial Regex GeneratedRegexEmpty();

        [GeneratedRegex(@"#.+")]
        private static partial Regex GeneratedRegexInlineComment();

        private ICmnLogger? _logger;
        private Dictionary<string, string> _configDictionary = [];
        private Dictionary<string, List<string>> _listDictionary = [];
        private List<string> _duplicateKeys = [];
        private List<string> _configList = [];
        private int _verbose = 0;
        private string _pattern = "^(?<KEY>[^#=]+)=(?<VALUE>.+)$";
        private Regex? _cachedPatternRegex;
        private Encoding _encoding = Encoding.Default;
        private readonly Regex _commentRegex = GeneratedRegexComment();
        private readonly Regex _emptyLineRegex = GeneratedRegexEmpty();
        private readonly Regex _inlineCommentRegex = GeneratedRegexInlineComment();
        private bool _isSkipComment = true;
        private bool _detectEncoding = true;

        static ClsConfigFile()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// <see cref="ClsConfigFile"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="logger">ログ出力に使用するロガーオブジェクト。</param>
        /// <example>
        /// <code>
        /// var configFile = new ClsConfigFile(logger);
        /// </code>
        /// </example>
        public ClsConfigFile(ICmnLogger logger)
        {
            _logger = logger;
            Clear();
        }

        /// <summary>
        /// 読み込んだ設定キーと値のペアを格納する辞書を取得または設定します。
        /// </summary>
        public Dictionary<string, string> ConfigDictionary { get => _configDictionary; set => _configDictionary = value; }

        /// <summary>
        /// 重複を許可した特定キーに対する複数の値を格納する辞書を取得または設定します。
        /// </summary>
        public Dictionary<string, List<string>> ListDictionary { get => _listDictionary; set => _listDictionary = value; }

        /// <summary>
        /// 読み込んだ設定行の一覧を取得または設定します。
        /// </summary>
        public List<string> ConfigList { get => _configList; set => _configList = value; }

        /// <summary>
        /// 重複値をリスト形式 (<see cref="ListDictionary"/>) で保持することを許可するキーのリストを取得または設定します。
        /// </summary>
        public List<string> DuplicateKeys { get => _duplicateKeys; set => _duplicateKeys = value; }

        /// <summary>
        /// 詳細ログの出力レベルを取得または設定します（数値が大きいほど詳細なログが出力されます）。
        /// </summary>
        public int Verbose { get => _verbose; set => _verbose = value; }

        /// <summary>
        /// Key=Value を抽出するための正規表現パターンを取得または設定します。
        /// </summary>
        public string Pattern
        {
            get => _pattern;
            set
            {
                _pattern = value;
                _cachedPatternRegex = null;
            }
        }

        /// <summary>
        /// ファイル読み込み時に使用する文字エンコーディングを取得または設定します。
        /// </summary>
        public Encoding Encoding { get => _encoding; set => _encoding = value; }

        /// <summary>
        /// コメント行（#で始まる行および行末コメント）をスキップするかどうかを取得または設定します。
        /// </summary>
        public bool IsSkipComment { get => _isSkipComment; set => _isSkipComment = value; }

        #region 旧互換用プロパティ
        /// <summary>
        /// 読み込んだ設定キーと値のペアを格納する辞書を取得または設定します。
        /// </summary>
        [Obsolete("代わりに 'ConfigDictionary' を使用します。")]
        public Dictionary<string, string> ConfigDic { get => ConfigDictionary; set => ConfigDictionary = value; }

        /// <summary>
        /// 重複を許可した特定キーに対する複数の値を格納する辞書を取得または設定します。
        /// </summary>
        [Obsolete("代わりに 'ListDictionary' を使用します。")]
        public Dictionary<string, List<string>> ListDic { get => ListDictionary; set => ListDictionary = value; }

        /// <summary>
        /// 重複値をリスト形式で保持することを許可するキーのリストを取得または設定します。
        /// </summary>
        [Obsolete("代わりに 'DuplicateKeys' を使用します。")]
        public List<string> DuplicateKeyList { get => DuplicateKeys; set => DuplicateKeys = value; }
        #endregion

        /// <summary>
        /// 保持している設定情報（辞書、リスト等）をすべてクリアします。
        /// </summary>
        /// <example>
        /// <code>
        /// configFile.Clear();
        /// </code>
        /// </example>
        public void Clear()
        {
            _configDictionary.Clear();
            _listDictionary.Clear();
            _configList.Clear();
            _duplicateKeys.Clear();
        }

        /// <summary>
        /// 指定された設定ファイルを読み込み、設定内容を Key-Value の辞書 (<see cref="ConfigDictionary"/>) に格納します。
        /// </summary>
        /// <param name="filePath">読み込み対象の設定ファイルパス。</param>
        /// <returns>正常に読み込んで辞書に格納された設定項目数。エラー発生時は -1。</returns>
        /// <example>
        /// <code>
        /// var configFile = new ClsConfigFile(logger);
        /// int count = configFile.LoadToDictionary(@"C:\app.conf");
        /// Console.WriteLine($"Loaded {count} items.");
        /// </code>
        /// </example>
        public int LoadToDictionary(string filePath)
        {
            const string METHOD_NAME = "[ClsConfigFile.LoadToDictionary()]";
            if (_verbose > 3)
            {
                WriteLog(MdlConst.LVL_DEBUG, METHOD_NAME);
                WriteLog(MdlConst.LVL_DEBUG, $"{METHOD_NAME}filePath       = {filePath}");
                WriteLog(MdlConst.LVL_DEBUG, $"{METHOD_NAME}_verbose       = {_verbose}");
                WriteLog(MdlConst.LVL_DEBUG, $"{METHOD_NAME}_isSkipComment = {_isSkipComment}");
                WriteLog(MdlConst.LVL_DEBUG, METHOD_NAME);
            }

            if (_configDictionary is null)
            {
                WriteLog(MdlConst.LVL_E, $"{METHOD_NAME} _configDictionary is null");
                return -1;
            }

            try
            {
                if (_detectEncoding)
                {
                    _encoding = MdlFile.DetectFileEncoding(filePath);
                }

                using var reader = new StreamReader(filePath, _encoding ?? Encoding.Default);
                _cachedPatternRegex ??= new Regex(_pattern, RegexOptions.Compiled);

                string? line;
                while ((line = reader.ReadLine()) is not null)
                {
                    string buffer;
                    string lineType = "NORMAL LINE";
                    bool isContinue = false;

                    // コメント行
                    if (_isSkipComment)
                    {
                        if (_commentRegex.IsMatch(line))
                        {
                            isContinue = true;
                            lineType = "SKIP : COMMENT LINE";
                        }
                        buffer = _inlineCommentRegex.Replace(line, "").Trim();
                    }
                    else
                    {
                        buffer = line.Trim();
                    }

                    // 空行
                    if (_emptyLineRegex.IsMatch(buffer))
                    {
                        isContinue = true;
                        lineType = "SKIP : EMPTY LINE";
                    }

                    if (_verbose > 5)
                    {
                        WriteLog(MdlConst.LVL_DEBUG, $"{METHOD_NAME}CURRENT LINE ({lineType}) : {buffer}");
                    }

                    if (isContinue) continue;

                    Match match = _cachedPatternRegex.Match(buffer);
                    if (match.Success)
                    {
                        string key = MdlUtil.TrimQuotes(match.Groups["KEY"].Value);
                        string value = MdlUtil.TrimQuotes(match.Groups["VALUE"].Value);

                        if (_verbose > 3)
                        {
                            WriteLog(MdlConst.LVL_DEBUG, $"{METHOD_NAME}[{filePath}] configDictionary[{key}] = {value}");
                        }

                        if (_duplicateKeys.Count > 0 && _duplicateKeys.Contains(key))
                        {
                            if (!_listDictionary.TryGetValue(key, out var tempList))
                            {
                                tempList = [];
                                _listDictionary[key] = tempList;
                            }
                            tempList.Add(value);
                        }

                        _configDictionary[key] = value;
                    }
                }
                return _configDictionary.Count;
            }
            catch (Exception ex)
            {
                WriteLog(MdlConst.LVL_E, $"{METHOD_NAME} Exception : {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 設定ファイルを読み込み、辞書に格納します（旧形式互換用メソッド）。
        /// </summary>
        /// <param name="filePath">設定ファイルのパス。</param>
        /// <returns>読み込んだ行数。</returns>
        /// <example>
        /// <code>
        /// int count = configFile.ReadFile(@"C:\app.conf");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'LoadToDictionary()' を使用します。")]
        public int ReadFile(string filePath) => LoadToDictionary(filePath);

        /// <summary>
        /// 指定された設定ファイルを読み込み、各行の文字列をリスト (<see cref="ConfigList"/>) に格納します。
        /// </summary>
        /// <param name="filePath">読み込み対象の設定ファイルパス。</param>
        /// <param name="unique">重複行を除外してユニークな行のみ保持する場合は <c>true</c>。</param>
        /// <returns>正常に読み込んでリストに格納された行数。エラー発生時は -1。</returns>
        /// <example>
        /// <code>
        /// var configFile = new ClsConfigFile(logger);
        /// int lineCount = configFile.LoadToList(@"C:\list.txt", unique: true);
        /// </code>
        /// </example>
        public int LoadToList(string filePath, bool unique)
        {
            const string METHOD_NAME = "[ClsConfigFile.LoadToList()]";
            if (_verbose > 3)
            {
                WriteLog(MdlConst.LVL_DEBUG, METHOD_NAME);
                WriteLog(MdlConst.LVL_DEBUG, $"{METHOD_NAME}filePath              = {filePath}");
                WriteLog(MdlConst.LVL_DEBUG, $"{METHOD_NAME}_verbose              = {_verbose}");
                WriteLog(MdlConst.LVL_DEBUG, $"{METHOD_NAME}_isSkipComment = {_isSkipComment}");
                WriteLog(MdlConst.LVL_DEBUG, METHOD_NAME);
            }

            if (_configList is null)
            {
                WriteLog(MdlConst.LVL_E, $"{METHOD_NAME} _configList is null");
                return -1;
            }

            try
            {
                if (_detectEncoding)
                {
                    _encoding = MdlFile.DetectFileEncoding(filePath);
                }

                using var reader = new StreamReader(filePath, _encoding ?? Encoding.Default);

                string? line;
                while ((line = reader.ReadLine()) is not null)
                {
                    string buffer;
                    string lineType = "NORMAL LINE";
                    bool isContinue = false;

                    // コメント
                    if (_isSkipComment)
                    {
                        if (_commentRegex.IsMatch(line))
                        {
                            isContinue = true;
                            lineType = "SKIP : COMMENT LINE";
                        }
                        buffer = _inlineCommentRegex.Replace(line, "").Trim();
                    }
                    else
                    {
                        buffer = line.Trim();
                    }

                    // 空行
                    if (_emptyLineRegex.IsMatch(buffer))
                    {
                        isContinue = true;
                        lineType = "SKIP : EMPTY LINE";
                    }

                    if (_verbose > 5)
                    {
                        WriteLog(MdlConst.LVL_DEBUG, $"{METHOD_NAME}CURRENT LINE ({lineType}) : {buffer}");
                    }

                    if (isContinue) continue;

                    if (!unique || !_configList.Contains(buffer))
                    {
                        _configList.Add(buffer);
                    }
                }
                return _configList.Count;
            }
            catch (Exception ex)
            {
                WriteLog(MdlConst.LVL_E, $"{METHOD_NAME} Exception : {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 設定ファイルを読み込み、リストに格納します（旧形式互換用メソッド）。
        /// </summary>
        /// <param name="filePath">設定ファイルのパス。</param>
        /// <param name="unique">重複行を除外するかどうか。</param>
        /// <returns>読み込んだ行数。</returns>
        /// <example>
        /// <code>
        /// int count = configFile.ReadFileToList(@"C:\list.txt", true);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'LoadToList()' を使用します。")]
        public int ReadFileToList(string filePath, bool unique) => LoadToList(filePath, unique);

        /// <summary>
        /// 指定されたレベルとメッセージでログを出力します。ロガー未設定の場合は標準出力へ出力します。
        /// </summary>
        /// <param name="level">ログレベル。</param>
        /// <param name="message">出力するメッセージ文字列。</param>
        /// <example>
        /// <code>
        /// configFile.WriteLog(MdlConst.LVL_DEBUG, "処理を開始しました。");
        /// </code>
        /// </example>
        public void WriteLog(int level, string message)
        {
            if (_logger != null)
            {
                _logger.WriteLine(level, message);
            }
            else
            {
                try
                {
                    Console.WriteLine(message);
                }
                catch { }
            }
        }

        /// <summary>
        /// ログメッセージを書き込みます（旧形式互換用メソッド）。
        /// </summary>
        /// <param name="level">エラーレベル。</param>
        /// <param name="message">ログメッセージ。</param>
        /// <example>
        /// <code>
        /// configFile.Writeln(1, "メッセージ");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'WriteLog()' を使用します。")]
        public void Writeln(int level, string message) => WriteLog(level, message);
    }
}
