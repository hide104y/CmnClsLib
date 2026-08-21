using System;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Interface
{
    /// <summary>
    /// 共通ロガーの基本操作を提供するインターフェースです。
    /// </summary>
    public interface ICmnLogger
    {
        /// <summary>
        /// 指定されたキーに対応するプロパティ値（文字列）を取得します。
        /// </summary>
        /// <param name="key">取得対象のプロパティキー</param>
        /// <param name="defaultValue">キーが存在しない場合や無効な場合に返却されるデフォルト文字列</param>
        /// <returns>プロパティの値、またはデフォルト文字列</returns>
        /// <example>
        /// <code>
        /// string logDir = logger.GetValueByKey("dir", @"C:\Logs");
        /// </code>
        /// </example>
        string GetValueByKey(string key, string defaultValue) => GetValueByKey(key, defaultValue);

        /// <summary>
        /// 指定されたキーに対応するプロパティ値（真偽値）を取得します。
        /// </summary>
        /// <param name="key">取得対象のプロパティキー</param>
        /// <param name="defaultValue">キーが存在しない場合や無効な場合に返却されるデフォルトの真偽値</param>
        /// <returns>プロパティの真偽値、またはデフォルト値</returns>
        /// <example>
        /// <code>
        /// bool isFileLog = logger.GetValueByKey("isFile", false);
        /// </code>
        /// </example>
        bool GetValueByKey(string key, bool defaultValue) => GetValueByKey(key, defaultValue);

        /// <summary>
        /// 指定されたキーにプロパティ値を設定します。
        /// </summary>
        /// <param name="key">設定対象のプロパティキー</param>
        /// <param name="val">設定する値の文字列</param>
        /// <example>
        /// <code>
        /// logger.SetValueByKey("isFile", "true");
        /// </code>
        /// </example>
        void SetValueByKey(string key, string val) => SetValueByKey(key, val);

        /// <summary>
        /// 指定されたログレベルでメッセージを書き込みます。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="message">出力するログメッセージ</param>
        /// <example>
        /// <code>
        /// logger.WriteLine(1, "情報ログメッセージ");
        /// </code>
        /// </example>
        void WriteLine(int level, string message) => WriteLine(level, message);

        /// <summary>
        /// 指定されたキーに対応するプロパティ値（文字列）を取得します。（旧式）
        /// </summary>
        /// <param name="key">取得対象のプロパティキー</param>
        /// <param name="defaultValue">デフォルト文字列</param>
        /// <returns>プロパティの値、またはデフォルト文字列</returns>
        [Obsolete("代わりに 'GetValueByKey(string, string)' を使用します。")]
        string GetValByKey(string key, string defaultValue);

        /// <summary>
        /// 指定されたキーに対応するプロパティ値（真偽値）を取得します。（旧式）
        /// </summary>
        /// <param name="key">取得対象のプロパティキー</param>
        /// <param name="defaultValue">デフォルトの真偽値</param>
        /// <returns>プロパティの真偽値、またはデフォルト値</returns>
        [Obsolete("代わりに 'GetValueByKey(string, bool)' を使用します。")]
        bool GetValByKey(string key, bool defaultValue);

        /// <summary>
        /// 指定されたキーにプロパティ値を設定します。（旧式）
        /// </summary>
        /// <param name="key">設定対象のプロパティキー</param>
        /// <param name="val">設定する値の文字列</param>
        [Obsolete("代わりに 'SetValueByKey(string, string)' を使用します。")]
        void SetValByKey(string key, string val);

        /// <summary>
        /// 指定されたログレベルでメッセージを書き込みます。（旧式）
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="msg">出力するログメッセージ</param>
        [Obsolete("代わりに 'WriteLine(int, string)' を使用します。")]
        void Writeln(int level, string msg);
    }
}
