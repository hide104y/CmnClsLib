using System;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Module;

/// <summary>
/// ログ出力処理に関連するユーティリティ機能を提供する静的クラスです。
/// </summary>
public static class MdlLog
{
    /// <summary>
    /// ログレベルの文字列（"none", "debug", "info", "warn", "error", "fatal"）を対応する整数値に変換します。
    /// </summary>
    /// <param name="logLevel">変換対象のログレベル文字列（大文字小文字不問、前後空白許容）。</param>
    /// <returns>対応するログレベルの整数値。無効な文字列または null/空文字の場合はデフォルト値 (MdlConst.LVL_I) を返します。</returns>
    /// <example>
    /// <code>
    /// int level = MdlLog.ParseLogLevel("error"); // MdlConst.LVL_E が返されます
    /// int defaultLevel = MdlLog.ParseLogLevel(null); // MdlConst.LVL_I が返されます
    /// </code>
    /// </example>
    public static int ParseLogLevel(string logLevel)
    {
        if (string.IsNullOrWhiteSpace(logLevel))
        {
            return MdlConst.LVL_I;
        }

        return logLevel.Trim().ToLowerInvariant() switch
        {
            "none" => MdlConst.LVL_NONE,
            "debug" => MdlConst.LVL_DEBUG,
            "info" => MdlConst.LVL_I,
            "warn" => MdlConst.LVL_W,
            "error" => MdlConst.LVL_E,
            "fatal" => MdlConst.LVL_F,
            _ => MdlConst.LVL_I
        };
    }

    /// <summary>
    /// 【非推奨】ログレベルの文字列を対応する整数値に変換します。代わりに <see cref="ParseLogLevel(string)"/> を使用してください。
    /// </summary>
    /// <param name="logLevel">ログレベルの文字列。</param>
    /// <returns>対応する整数値のログレベル。</returns>
    [Obsolete("代わりに 'ParseLogLevel(logLevel)' を使用します。")]
    public static int LogLvlStrToInt(string logLevel) => ParseLogLevel(logLevel);

    /// <summary>
    /// ログレベルの整数値を対応する文字列（"none", "debug", "info", "warn", "error", "fatal"）に変換します。
    /// </summary>
    /// <param name="logLevel">ログレベルの整数値。</param>
    /// <returns>小文字表記のログレベル文字列。不明な定数の場合は "info" を返します。</returns>
    /// <example>
    /// <code>
    /// string levelStr = MdlLog.LogLevelToString(MdlConst.LVL_E); // "error" が返されます
    /// </code>
    /// </example>
    public static string LogLevelToString(int logLevel)
    {
        return logLevel switch
        {
            MdlConst.LVL_NONE => "none",
            MdlConst.LVL_DEBUG => "debug",
            MdlConst.LVL_I => "info",
            MdlConst.LVL_W => "warn",
            MdlConst.LVL_E => "error",
            MdlConst.LVL_F => "fatal",
            _ => "info"
        };
    }

    /// <summary>
    /// 【非推奨】ログレベルの整数値を対応する文字列に変換します。代わりに <see cref="LogLevelToString(int)"/> を使用してください。
    /// </summary>
    /// <param name="logLevel">ログレベルの整数値。</param>
    /// <returns>対応する文字列のログレベル。</returns>
    [Obsolete("代わりに 'LogLevelToString(logLevel)' を使用します。")]
    public static string LogLvlIntToStr(int logLevel) => LogLevelToString(logLevel);

    /// <summary>
    /// ファイルのベース名に当日の日付（yyyyMMdd）を付加してサニタイズされたログファイル名を生成します。
    /// </summary>
    /// <param name="baseName">ログファイルのベース名。</param>
    /// <returns>サニタイズ済みのログファイル名（例: "app_20260801.log"）。</returns>
    /// <example>
    /// <code>
    /// string fileName = MdlLog.GenerateLogFileName("app"); // "app_20260801.log" などの形式で返されます
    /// </code>
    /// </example>
    public static string GenerateLogFileName(string baseName)
    {
        string dateStr = MdlDate.GetFormattedDate("yyyyMMdd");
        return MdlFile.SanitizeFileName($"{baseName}_{dateStr}.log");
    }

    /// <summary>
    /// 【非推奨】ベース名に日付を付加してファイル名を生成します。代わりに <see cref="GenerateLogFileName(string)"/> を使用してください。
    /// </summary>
    /// <param name="baseName">ベース名。</param>
    /// <returns>生成されたファイル名。</returns>
    [Obsolete("代わりに 'GenerateLogFileName(baseName)' を使用します。")]
    public static string GetFileName(string baseName) => GenerateLogFileName(baseName);

    /// <summary>
    /// ログレベルに応じた出力用のプレフィックス文字列（大文字＋末尾スペース）を取得します。
    /// </summary>
    /// <param name="logLevel">ログレベルの整数値。</param>
    /// <returns>ログプレフィックス文字列（例: "INFO ", "ERROR "）。"none" の場合は空文字列を返します。</returns>
    /// <example>
    /// <code>
    /// string prefix = MdlLog.GetLogLevelPrefix(MdlConst.LVL_E); // "ERROR " が返されます
    /// string nonePrefix = MdlLog.GetLogLevelPrefix(MdlConst.LVL_NONE); // "" が返されます
    /// </code>
    /// </example>
    public static string GetLogLevelPrefix(int logLevel)
    {
        string prefix = LogLevelToString(logLevel);
        return prefix == "none" ? string.Empty : $"{prefix.ToUpperInvariant()} ";
    }

    /// <summary>
    /// 【非推奨】ログレベルに対応するプレフィックスメッセージを取得します。代わりに <see cref="GetLogLevelPrefix(int)"/> を使用してください。
    /// </summary>
    /// <param name="logLevel">ログレベル。</param>
    /// <returns>プレフィックスメッセージ。</returns>
    [Obsolete("代わりに 'GetLogLevelPrefix(logLevel)' を使用します。")]
    public static string GetPrefixMessage(int logLevel) => GetLogLevelPrefix(logLevel);
}
