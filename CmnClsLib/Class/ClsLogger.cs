using CmnClsLib.Interface;
using CmnClsLib.Module;
using System;
using System.IO;
using System.Threading;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Class
{
    /// <summary>
    /// コンソールおよびファイルへのログ出力を管理するロガークラスです。
    /// </summary>
    public class ClsLogger : ICmnLogger
    {
        // 定数
        public const string IS_STDOUT = "isStdOut";
        public const string IS_STDERR = "isStdErr";
        public const string IS_CONSOLE = "isConsole";
        public const string IS_FILE = "isFile";
        public const string IS_APPEND = "isAppend";
        public const string IS_FLUSH = "isFlush";
        public const string IS_TRIM_END = "isTrimEnd";
        public const string IS_TRIM_CONSOLE = "isTrimConsole";
        public const string IS_CONSOLE_ENCODING = "isConsoleEncoding";
        public const string DIR = "dir";
        public const string PATH = "path";
        public const string BASENAME = "baseName";
        public const string FILENAME = "fileName";
        public const string CONSOLE_ENCODING = "consoleEncoding";
        public const string FILE_ENCODING = "fileEncoding";

        // ログ出力設定用変数
        private readonly Lock _fileLock = new();
        private volatile bool _isStdErr = false;
        private volatile bool _isStdOut = false;
        private volatile bool _isConsole = true;
        private volatile bool _isFile = false;
        private volatile bool _isAppend = true;
        private volatile bool _isFlush = false;
        private volatile bool _isTrimEnd = true;
        private volatile bool _isTrimConsole = true;
        private volatile bool _isConsoleEncoding = false;
        private volatile string _dir = "";
        private volatile string _path = "";
        private volatile string _baseName = "";
        private volatile string _fileName = "";
        private volatile System.Text.Encoding _consoleEncoding = System.Text.Encoding.Default;
        private volatile System.Text.Encoding _fileEncoding = System.Text.Encoding.Default;

        /// <summary>
        /// <see cref="ClsLogger"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <example>
        /// <code>
        /// var logger = new ClsLogger();
        /// </code>
        /// </example>
        public ClsLogger()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// ログ出力設定のプロパティ値をキー指定で設定します。
        /// </summary>
        /// <param name="key">プロパティキー (例: <see cref="IS_FILE"/>, <see cref="DIR"/>)</param>
        /// <param name="val">設定する値の文字列</param>
        /// <example>
        /// <code>
        /// var logger = new ClsLogger();
        /// logger.SetValueByKey(ClsLogger.IS_FILE, "true");
        /// logger.SetValueByKey(ClsLogger.DIR, @"C:\Logs");
        /// </code>
        /// </example>
        public void SetValueByKey(string key, string val)
        {
            switch (key)
            {
                case ClsLogger.IS_STDOUT:
                    _isStdOut = MdlUtil.IsTrue(val, false);
                    break;
                case ClsLogger.IS_STDERR:
                    _isStdErr = MdlUtil.IsTrue(val, false);
                    break;
                case ClsLogger.IS_CONSOLE:
                    _isConsole = MdlUtil.IsTrue(val, true);
                    break;
                case ClsLogger.IS_FILE:
                    _isFile = MdlUtil.IsTrue(val, false);
                    break;
                case ClsLogger.IS_APPEND:
                    _isAppend = MdlUtil.IsTrue(val, true);
                    break;
                case ClsLogger.IS_FLUSH:
                    _isFlush = MdlUtil.IsTrue(val, false);
                    break;
                case ClsLogger.IS_TRIM_END:
                    _isTrimEnd = MdlUtil.IsTrue(val, true);
                    break;
                case ClsLogger.IS_TRIM_CONSOLE:
                    _isTrimConsole = MdlUtil.IsTrue(val, true);
                    break;
                case ClsLogger.IS_CONSOLE_ENCODING:
                    _isConsoleEncoding = MdlUtil.IsTrue(val, false);
                    break;
                case ClsLogger.DIR:
                    _dir = val;
                    break;
                case ClsLogger.PATH:
                    _path = val;
                    break;
                case ClsLogger.BASENAME:
                    _baseName = val;
                    break;
                case ClsLogger.FILENAME:
                    _fileName = val;
                    break;
                case ClsLogger.CONSOLE_ENCODING:
                    _consoleEncoding = MdlUtil.GetEncoding(val);
                    break;
                case ClsLogger.FILE_ENCODING:
                    _fileEncoding = MdlUtil.GetEncoding(val);
                    break;
            }
        }

        /// <summary>
        /// ログ出力設定のプロパティ値をキー指定で設定します。（旧式）
        /// </summary>
        /// <param name="key">プロパティキー</param>
        /// <param name="val">設定する値の文字列</param>
        /// <example>
        /// <code>
        /// logger.SetValByKey(ClsLogger.IS_FILE, "true");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'SetValueByKey(string, string)' を使用します。")]
        public void SetValByKey(string key, string val)
        {
            SetValueByKey(key, val);
        }

        /// <summary>
        /// キーに対応するプロパティ値（文字列）を取得します。
        /// </summary>
        /// <param name="key">プロパティキー</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>プロパティの文字列値、またはデフォルト値</returns>
        /// <example>
        /// <code>
        /// string dir = logger.GetValueByKey(ClsLogger.DIR, "");
        /// </code>
        /// </example>
        public string GetValueByKey(string key, string defaultValue)
        {
            string value = defaultValue;
            switch (key)
            {
                case ClsLogger.IS_STDOUT:
                case ClsLogger.IS_STDERR:
                case ClsLogger.IS_CONSOLE:
                case ClsLogger.IS_FILE:
                case ClsLogger.IS_APPEND:
                case ClsLogger.IS_FLUSH:
                case ClsLogger.IS_TRIM_END:
                case ClsLogger.IS_TRIM_CONSOLE:
                case ClsLogger.IS_CONSOLE_ENCODING:
                    value = (GetValueByKey(key, MdlUtil.IsTrue(defaultValue, false))).ToString();
                    break;
                case ClsLogger.DIR:
                    value = _dir;
                    break;
                case ClsLogger.PATH:
                    value = _path;
                    break;
                case ClsLogger.BASENAME:
                    value = _baseName;
                    break;
                case ClsLogger.FILENAME:
                    value = _fileName;
                    break;
                case ClsLogger.CONSOLE_ENCODING:
                    value = MdlUtil.GetEncodingName(_consoleEncoding);
                    break;
                case ClsLogger.FILE_ENCODING:
                    value = MdlUtil.GetEncodingName(_fileEncoding);
                    break;
            }
            return value;
        }

        /// <summary>
        /// キーに対応するプロパティ値（文字列）を取得します。（旧式）
        /// </summary>
        /// <param name="key">プロパティキー</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>プロパティの文字列値、またはデフォルト値</returns>
        /// <example>
        /// <code>
        /// string dir = logger.GetValByKey(ClsLogger.DIR, "");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetValueByKey(string, string)' を使用します。")]
        public string GetValByKey(string key, string defaultValue)
        {
            return GetValueByKey(key, defaultValue);
        }

        /// <summary>
        /// キーに対応するプロパティ値（真偽値）を取得します。
        /// </summary>
        /// <param name="key">プロパティキー</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>プロパティの真偽値、またはデフォルト値</returns>
        /// <example>
        /// <code>
        /// bool isFile = logger.GetValueByKey(ClsLogger.IS_FILE, false);
        /// </code>
        /// </example>
        public bool GetValueByKey(string key, bool defaultValue)
        {
            bool value = defaultValue;
            switch (key)
            {
                case ClsLogger.IS_STDOUT:
                    value = _isStdOut;
                    break;
                case ClsLogger.IS_STDERR:
                    value = _isStdErr;
                    break;
                case ClsLogger.IS_CONSOLE:
                    value = _isConsole;
                    break;
                case ClsLogger.IS_FILE:
                    value = _isFile;
                    break;
                case ClsLogger.IS_APPEND:
                    value = _isAppend;
                    break;
                case ClsLogger.IS_FLUSH:
                    value = _isFlush;
                    break;
                case ClsLogger.IS_TRIM_END:
                    value = _isTrimEnd;
                    break;
                case ClsLogger.IS_TRIM_CONSOLE:
                    value = _isTrimConsole;
                    break;
                case ClsLogger.IS_CONSOLE_ENCODING:
                    value = _isConsoleEncoding;
                    break;
            }
            return value;
        }

        /// <summary>
        /// キーに対応するプロパティ値（真偽値）を取得します。（旧式）
        /// </summary>
        /// <param name="key">プロパティキー</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>プロパティの真偽値、またはデフォルト値</returns>
        /// <example>
        /// <code>
        /// bool isFile = logger.GetValByKey(ClsLogger.IS_FILE, false);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetValueByKey(string, bool)' を使用します。")]
        public bool GetValByKey(string key, bool defaultValue)
        {
            return GetValueByKey(key, defaultValue);
        }

        /// <summary>
        /// 指定されたログレベルでログメッセージを出力（コンソールおよびファイル）します。
        /// </summary>
        /// <param name="level">ログレベル (例: <see cref="MdlConst.LVL_I"/>, <see cref="MdlConst.LVL_E"/>)</param>
        /// <param name="message">出力メッセージ</param>
        /// <example>
        /// <code>
        /// var logger = new ClsLogger();
        /// logger.WriteLine(MdlConst.LVL_I, "処理を開始しました。");
        /// </code>
        /// </example>
        public void WriteLine(int level, string message)
        {
            bool isStdErr = _isStdErr;
            switch (level)
            {
                case MdlConst.LVL_W:
                case MdlConst.LVL_E:
                case MdlConst.LVL_F:
                    isStdErr = true;
                    break;
            }
            string outputLine;
            switch (level)
            {
                case MdlConst.LVL_DEBUG:
                case MdlConst.LVL_I:
                case MdlConst.LVL_W:
                case MdlConst.LVL_E:
                    outputLine = MdlDate.GetFormattedDate("yyyy/MM/dd HH:mm:ss") + " " + MdlLog.GetLogLevelPrefix(level) + message;
                    break;
                default:
                    outputLine = MdlLog.GetLogLevelPrefix(level) + message;
                    break;
            }
            // 行末の空白を削除
            string trimmedLine = (_isTrimEnd ? outputLine.TrimEnd() : outputLine);
            // コンソール出力
            if (_isConsole) WriteToConsole((_isStdOut ? false : isStdErr), (_isTrimConsole ? trimmedLine : outputLine));
            // ファイル出力
            WriteToFile(trimmedLine);
        }

        /// <summary>
        /// 指定されたログレベルでログメッセージを出力します。（旧式）
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="msg">出力メッセージ</param>
        /// <example>
        /// <code>
        /// logger.Writeln(MdlConst.LVL_I, "メッセージ");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'WriteLine(int, string)' を使用します。")]
        public void Writeln(int level, string msg)
        {
            WriteLine(level, msg);
        }

        /// <summary>
        /// コンソールにログメッセージを書き込みます。
        /// </summary>
        /// <param name="isStdErr">標準エラー出力に書き込む場合は true、標準出力に書き込む場合は false</param>
        /// <param name="line">書き込むログメッセージ行</param>
        /// <example>
        /// <code>
        /// WriteToConsole(false, "メッセージ");
        /// </code>
        /// </example>
        private void WriteToConsole(bool isStdErr, string line)
        {
            try
            {
                if (_isConsoleEncoding) Console.OutputEncoding = _consoleEncoding;
                if (isStdErr)
                {
                    Console.Error.WriteLine(line);
                }
                else
                {
                    Console.Out.WriteLine(line);
                }
            }
            catch { }
        }

        /// <summary>
        /// ログメッセージをファイルへスレッドセーフに書き込みます。
        /// </summary>
        /// <param name="line">ファイルに書き込むログメッセージ行</param>
        /// <example>
        /// <code>
        /// WriteToFile("2026/08/02 12:00:00 [INFO] テストメッセージ");
        /// </code>
        /// </example>
        private void WriteToFile(string line)
        {
            if (!_isFile) return;
            string currentPath = "";
            if (string.IsNullOrEmpty(_path))
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    if (string.IsNullOrEmpty(_baseName)) _baseName = MdlApp.GetAppNameWithHostName();
                    currentPath = Path.Combine(_dir, MdlLog.GenerateLogFileName(_baseName));
                }
                else
                {
                    currentPath = Path.Combine(_dir, _fileName);
                }
            }
            else
            {
                currentPath = _path;
            }
            MdlFile.CreateDirectory(MdlFile.GetDirectoryPath(currentPath));
            lock (_fileLock)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(currentPath, _isAppend, _fileEncoding))
                    {
                        sw.WriteLine(line);
                        if (_isFlush) sw.Flush();
                    }
                }
                catch (Exception ex)
                {
                    _isFile = false;
                    WriteToConsole(true, "ERROR [Logger.WriteToFile()] EXCEPTION : " + ex.Message);
                }
                finally
                {
                    _isAppend = true;
                }
            }
        }
    }
}