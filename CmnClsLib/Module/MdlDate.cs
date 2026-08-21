using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Module
{
    /// <summary>
    /// 日時・日付文字列の検証、フォーマット変換、UNIX時間変換、正規表現抽出などの日付関連ユーティリティ機能を提供するクラスです。
    /// </summary>
    public static partial class MdlDate
    {
        /// <summary>
        /// 年月日（YYYY/MM/DD）判定用の正規表現パターン文字列です。
        /// </summary>
        public static string PATTERN_YYYYMMDD = @"(?<YYYY>[0-9]{4})[-\/]*(?<MM>[0-1][0-9])[-\/]*(?<DD>[0-3][0-9])";

        /// <summary>
        /// 年月日時分秒（YYYYMMDDHHMMSS）判定用の正規表現パターン文字列です。
        /// </summary>
        public static string PATTERN_YYYYMMDDHHMMSS = @"(?<YYYY>[0-9]{4})(?<MM>[0-1][0-9])(?<DD>[0-3][0-9])(?<HH>[0-2][0-9])(?<MIN>[0-5][0-9])(?<SEC>[0-5][0-9])";

        #region Generated Regex (.NET Source Generators)

        /// <summary>
        /// 数字以外の文字にマッチする正規表現オブジェクトを取得します。
        /// </summary>
        /// <returns>数字以外の文字にマッチする <see cref="Regex"/> オブジェクト。</returns>
        /// <example>
        /// <code>
        /// string result = NonDigitRegex().Replace("2026/08/01", ""); // "20260801"
        /// </code>
        /// </example>
        [GeneratedRegex(@"\D")]
        private static partial Regex NonDigitRegex();

        /// <summary>
        /// 1文字以上の連続する空白文字にマッチする正規表現オブジェクトを取得します。
        /// </summary>
        /// <returns>連続する空白文字にマッチする <see cref="Regex"/> オブジェクト。</returns>
        /// <example>
        /// <code>
        /// string[] parts = WhitespaceRegex().Split("2026/08/01  12:00:00");
        /// </code>
        /// </example>
        [GeneratedRegex(@"\s+")]
        private static partial Regex WhitespaceRegex();

        /// <summary>
        /// 年月日（YYYY/MM/DD等）にマッチする正規表現オブジェクトを取得します。
        /// </summary>
        /// <returns>年月日パターンにマッチする <see cref="Regex"/> オブジェクト。</returns>
        /// <example>
        /// <code>
        /// Match match = YyyyMmDdRegex().Match("2026/08/01");
        /// </code>
        /// </example>
        [GeneratedRegex(@"(?<YYYY>[0-9]{4})[-\/]*(?<MM>[0-1][0-9])[-\/]*(?<DD>[0-3][0-9])")]
        private static partial Regex YyyyMmDdRegex();

        /// <summary>
        /// 年月日時分秒（YYYYMMDDHHMMSS）にマッチする正規表現オブジェクトを取得します。
        /// </summary>
        /// <returns>年月日時分秒パターンにマッチする <see cref="Regex"/> オブジェクト。</returns>
        /// <example>
        /// <code>
        /// Match match = YyyyMmDdHhMmSsRegex().Match("20260801123045");
        /// </code>
        /// </example>
        [GeneratedRegex(@"(?<YYYY>[0-9]{4})(?<MM>[0-1][0-9])(?<DD>[0-3][0-9])(?<HH>[0-2][0-9])(?<MIN>[0-5][0-9])(?<SEC>[0-5][0-9])")]
        private static partial Regex YyyyMmDdHhMmSsRegex();

        #endregion

        #region Obsolete Compatible Methods

        /// <summary>
        /// 【非推奨】指定された文字列が日付として有効か、および指定した日付閾値を超えているかを検証します。
        /// </summary>
        /// <param name="dateString">検証対象の日付文字列。</param>
        /// <param name="checkDate">日付の閾値（YYYYMMDD形式の数値）。0の場合は閾値チェックを行いません。</param>
        /// <returns>有効な日付であり閾値を超えている場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool isValid = MdlDate.IsDateTime("2026/08/01", 20200101); // true
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsValidDate()' を使用します。")]
        public static bool IsDateTime(string dateString, int checkDate)
        {
            return IsValidDate(dateString, checkDate);
        }

        /// <summary>
        /// 【非推奨】指定された文字列が有効な日付かを検証します。
        /// </summary>
        /// <param name="dateString">検証対象の日付文字列。</param>
        /// <returns>有効な日付の場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool isValid = MdlDate.IsDateTime("2026-08-01"); // true
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsValidDate()' を使用します。")]
        public static bool IsDateTime(string dateString)
        {
            return IsValidDate(dateString);
        }

        /// <summary>
        /// 【非推奨】YYYYMMDDHHMMSS形式の文字列を日付形式の文字列に変換します。
        /// </summary>
        /// <param name="strYyyyMMddhhmmss">変換対象の日時文字列。</param>
        /// <returns>フォーマット済みの日付文字列。</returns>
        /// <example>
        /// <code>
        /// string result = MdlDate.GetDateEn("20260801123045"); // "2026/08/01 12:30:45"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ConvertToDateString()' を使用します。")]
        public static string GetDateEn(string strYyyyMMddhhmmss)
        {
            return ConvertToDateString(strYyyyMMddhhmmss);
        }

        /// <summary>
        /// 【非推奨】引数文字列から日付文字列を抽出し、フォーマットします。
        /// </summary>
        /// <param name="inputString">入力文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="dateCheckType">日付検証の閾値タイプ。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string date = MdlDate.GetDateStrFromArgStr("file_20260801.txt", true, 0); // "2026/08/01"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractAndFormatDateString()' を使用します。")]
        public static string GetDateStrFromArgStr(string inputString, bool checkDate, int dateCheckType)
        {
            return ExtractAndFormatDateString(inputString, checkDate, dateCheckType);
        }

        /// <summary>
        /// 【非推奨】引数文字列から日付文字列を抽出し、フォーマットします。
        /// </summary>
        /// <param name="inputString">入力文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string date = MdlDate.GetDateStrFromArgStr("file_20260801.txt", true); // "2026/08/01"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractAndFormatDateString()' を使用します。")]
        public static string GetDateStrFromArgStr(string inputString, bool checkDate)
        {
            return ExtractAndFormatDateString(inputString, checkDate, 0);
        }

        /// <summary>
        /// 【非推奨】引数文字列から日付文字列を抽出し、フォーマットします。
        /// </summary>
        /// <param name="inputString">入力文字列。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string date = MdlDate.GetDateStrFromArgStr("file_20260801.txt"); // "2026/08/01"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractAndFormatDateString()' を使用します。")]
        public static string GetDateStrFromArgStr(string inputString)
        {
            return ExtractAndFormatDateString(inputString, true, 0);
        }

        /// <summary>
        /// 【非推奨】引数文字列から日付文字列を末尾優先で抽出し、フォーマットします。
        /// </summary>
        /// <param name="inputString">入力文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="dateCheckType">日付検証の閾値タイプ。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string date = MdlDate.GetDateStrFromArgStrReverse("log_20240101_20260801.txt", true, 0); // "2026/08/01"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateFromStringReverse()' を使用します。")]
        public static string GetDateStrFromArgStrReverse(string inputString, bool checkDate, int dateCheckType)
        {
            return ExtractDateFromStringReverse(inputString, checkDate, dateCheckType);
        }

        /// <summary>
        /// 【非推奨】引数文字列から日付文字列を末尾優先で抽出し、フォーマットします。
        /// </summary>
        /// <param name="inputString">入力文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string date = MdlDate.GetDateStrFromArgStrReverse("log_20240101_20260801.txt", true); // "2026/08/01"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateFromStringReverse()' を使用します。")]
        public static string GetDateStrFromArgStrReverse(string inputString, bool checkDate)
        {
            return ExtractDateFromStringReverse(inputString, checkDate, 0);
        }

        /// <summary>
        /// 【非推奨】引数文字列から日付文字列を末尾優先で抽出し、フォーマットします。
        /// </summary>
        /// <param name="inputString">入力文字列。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string date = MdlDate.GetDateStrFromArgStrReverse("log_20240101_20260801.txt"); // "2026/08/01"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateFromStringReverse()' を使用します。")]
        public static string GetDateStrFromArgStrReverse(string inputString)
        {
            return ExtractDateFromStringReverse(inputString, true, 0);
        }

        /// <summary>
        /// 【非推奨】パス文字列からファイル名を取得し、日付文字列を抽出しフォーマットします。
        /// </summary>
        /// <param name="inputString">入力パス文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="dateCheckType">日付検証の閾値タイプ。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string date = MdlDate.GetDateFromStr(@"C:\data\20260801.csv", true, 0); // "2026/08/01"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateFromPath()' を使用します。")]
        public static string GetDateFromStr(string inputString, bool checkDate, int dateCheckType)
        {
            return ExtractDateFromPath(inputString, checkDate, dateCheckType);
        }

        /// <summary>
        /// 【非推奨】パス文字列からファイル名を取得し、日付文字列を抽出しフォーマットします。
        /// </summary>
        /// <param name="inputString">入力パス文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string date = MdlDate.GetDateFromStr(@"C:\data\20260801.csv", true); // "2026/08/01"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateFromPath()' を使用します。")]
        public static string GetDateFromStr(string inputString, bool checkDate)
        {
            return ExtractDateFromPath(inputString, checkDate, 0);
        }

        /// <summary>
        /// 【非推奨】パス文字列からファイル名を取得し、日付文字列を抽出しフォーマットします。
        /// </summary>
        /// <param name="inputString">入力パス文字列。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string date = MdlDate.GetDateFromStr(@"C:\data\20260801.csv"); // "2026/08/01"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateFromPath()' を使用します。")]
        public static string GetDateFromStr(string inputString)
        {
            return ExtractDateFromPath(inputString, true, 0);
        }

        /// <summary>
        /// 【非推奨】日付文字列を検証し、フォーマットされた日付文字列を返します。
        /// </summary>
        /// <param name="dateString">入力日付文字列。</param>
        /// <param name="includeTime">時刻情報を含めるかどうか。</param>
        /// <returns>フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string formatted = MdlDate.GetValidateDate("2026-08-01 12:30", true); // "2026/08/01 12:30:00"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ValidateAndFormatDate()' を使用します。")]
        public static string GetValidateDate(string dateString, bool includeTime)
        {
            return ValidateAndFormatDate(dateString, includeTime);
        }

        /// <summary>
        /// 【非推奨】文字列を <see cref="DateTime"/> に変換します。
        /// </summary>
        /// <param name="dateTimeString">日付時刻の文字列。</param>
        /// <param name="dateTime">変換された <see cref="DateTime"/>。</param>
        /// <returns>変換が成功した場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool success = MdlDate.IsConvStringToDateTime("20260801123045", out DateTime dt);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'TryParseDateTime()' を使用します。")]
        public static bool IsConvStringToDateTime(string dateTimeString, out DateTime dateTime)
        {
            return TryParseDateTime(dateTimeString, out dateTime);
        }

        /// <summary>
        /// 【非推奨】2つの日時を比較し、指定された秒数範囲内であるかを判定します。
        /// </summary>
        /// <param name="firstDateTime">比較する最初の日時。</param>
        /// <param name="secondDateTime">比較する2つ目の日時。</param>
        /// <param name="secondRange">許容する秒数の範囲。</param>
        /// <returns>範囲内であれば0、範囲外で第1引数が大きければ1、小さければ-1。</returns>
        /// <example>
        /// <code>
        /// int res = MdlDate.CompareDatetime(DateTime.Now, DateTime.Now.AddSeconds(-2), 5.0); // 0
        /// </code>
        /// </example>
        [Obsolete("代わりに 'CompareDateTime()' を使用します。")]
        public static int CompareDatetime(DateTime firstDateTime, DateTime secondDateTime, double secondRange)
        {
            return CompareDateTime(firstDateTime, secondDateTime, secondRange);
        }

        /// <summary>
        /// 【非推奨】指定された文字列から任意のパターンで日付文字列を抽出します。
        /// </summary>
        /// <param name="input">入力文字列。</param>
        /// <param name="pattern">正規表現パターン。</param>
        /// <param name="validateDate">日付の妥当性を検証するかどうか。</param>
        /// <param name="dateFormat">日付検証の閾値タイプ。</param>
        /// <returns>抽出された日付文字列（YYYY/MM/DD形式）。</returns>
        /// <example>
        /// <code>
        /// string res = MdlDate.GetDateStrFromArgStr0("sample_20260801.txt", MdlDate.PATTERN_YYYYMMDD, true, 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateAny()' を使用します。")]
        public static string GetDateStrFromArgStr0(string input, string pattern, bool validateDate, int dateFormat)
        {
            return ExtractDateAny(input, pattern, validateDate, dateFormat);
        }

        /// <summary>
        /// 【非推奨】指定された文字列の先頭から日付文字列を抽出します。
        /// </summary>
        /// <param name="input">入力文字列。</param>
        /// <param name="basePattern">基本の正規表現パターン。</param>
        /// <param name="validateDate">日付の妥当性を検証するかどうか。</param>
        /// <param name="dateFormat">日付検証の閾値タイプ。</param>
        /// <returns>抽出された日付文字列（YYYY/MM/DD形式）。</returns>
        /// <example>
        /// <code>
        /// string res = MdlDate.GetDateStrFromArgStrLeft("20260801_file.txt", MdlDate.PATTERN_YYYYMMDD, true, 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateStartsWith()' を使用します。")]
        public static string GetDateStrFromArgStrLeft(string input, string basePattern, bool validateDate, int dateFormat)
        {
            return ExtractDateStartsWith(input, basePattern, validateDate, dateFormat);
        }

        /// <summary>
        /// 【非推奨】指定された文字列の中間から日付文字列を抽出します。
        /// </summary>
        /// <param name="input">入力文字列。</param>
        /// <param name="basePattern">基本の正規表現パターン。</param>
        /// <param name="checkGroup">グループチェック用文字列。</param>
        /// <param name="validateDate">日付の妥当性を検証するかどうか。</param>
        /// <param name="mode">取得モード（0: 最初に見つかった項目、1: 最後に見つかった項目）。</param>
        /// <param name="dateFormat">日付検証の閾値タイプ。</param>
        /// <returns>抽出された日付文字列（YYYY/MM/DD形式）。</returns>
        /// <example>
        /// <code>
        /// string res = MdlDate.GetDateStrFromArgStrMiddle("abc_20260801_def.txt", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateContains()' を使用します。")]
        public static string GetDateStrFromArgStrMiddle(string input, string basePattern, string checkGroup, bool validateDate, int mode, int dateFormat)
        {
            return ExtractDateContains(input, basePattern, checkGroup, validateDate, mode, dateFormat);
        }

        /// <summary>
        /// 【非推奨】指定された文字列の末尾から日付文字列を抽出します。
        /// </summary>
        /// <param name="input">入力文字列。</param>
        /// <param name="basePattern">基本の正規表現パターン。</param>
        /// <param name="validateDate">日付の妥当性を検証するかどうか。</param>
        /// <param name="dateFormat">日付検証の閾値タイプ。</param>
        /// <returns>抽出された日付文字列（YYYY/MM/DD形式）。</returns>
        /// <example>
        /// <code>
        /// string res = MdlDate.GetDateStrFromArgStrRight("file_20260801", MdlDate.PATTERN_YYYYMMDD, true, 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateEndsWith()' を使用します。")]
        public static string GetDateStrFromArgStrRight(string input, string basePattern, bool validateDate, int dateFormat)
        {
            return ExtractDateEndsWith(input, basePattern, validateDate, dateFormat);
        }

        /// <summary>
        /// 【非推奨】指定された文字列全体が完全一致する日付文字列を抽出します。
        /// </summary>
        /// <param name="input">入力文字列。</param>
        /// <param name="basePattern">基本の正規表現パターン。</param>
        /// <param name="validateDate">日付の妥当性を検証するかどうか。</param>
        /// <param name="dateFormat">日付検証の閾値タイプ。</param>
        /// <returns>抽出された日付文字列（YYYY/MM/DD形式）。</returns>
        /// <example>
        /// <code>
        /// string res = MdlDate.GetDateStrFromArgStrOnly("20260801", MdlDate.PATTERN_YYYYMMDD, true, 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ExtractDateExact()' を使用します。")]
        public static string GetDateStrFromArgStrOnly(string input, string basePattern, bool validateDate, int dateFormat)
        {
            return ExtractDateExact(input, basePattern, validateDate, dateFormat);
        }

        #endregion

        /// <summary>
        /// 指定された文字列が有効な日付形式であるか、および指定した閾値（YYYYMMDD形式の整数）を超えているかを検証します。
        /// </summary>
        /// <param name="dateString">検証対象の日付文字列。</param>
        /// <param name="checkDate">日付の閾値（例: 20200101）。0 を指定した場合は閾値チェックを行いません。</param>
        /// <returns>日付として有効であり、かつ閾値を超えている場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool valid1 = MdlDate.IsValidDate("2026/08/01", 20200101); // true
        /// bool valid2 = MdlDate.IsValidDate("2019/12/31", 20200101); // false
        /// </code>
        /// </example>
        public static bool IsValidDate(string dateString, int checkDate)
        {
            if (checkDate == 0)
            {
                return DateTime.TryParse(dateString, out _);
            }
            
            if (DateTime.TryParse(dateString, out _))
            {
                string digitsOnly = NonDigitRegex().Replace(dateString, "");
                if (MdlUtil.ParseInt(digitsOnly, 0) > checkDate) return true;
            }
            return false;
        }

        /// <summary>
        /// 指定された文字列が有効な日付形式かを検証します。
        /// </summary>
        /// <param name="dateString">検証対象の日付文字列。</param>
        /// <returns>有効な日付の場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool valid = MdlDate.IsValidDate("2026-08-01"); // true
        /// bool invalid = MdlDate.IsValidDate("2026-02-30"); // false
        /// </code>
        /// </example>
        public static bool IsValidDate(string dateString)
        {
            return DateTime.TryParse(dateString, out _);
        }

        /// <summary>
        /// YYYYMMDDHHMMSS 形式などの連続した数字列を、区切り文字付きの日時文字列（YYYY/MM/DD HH:mm:ss）に整形します。
        /// </summary>
        /// <param name="dateTimeString">変換する数字列（例: "20241110123045"）。</param>
        /// <returns>区切り文字（/ や :）が挿入された日時文字列。</returns>
        /// <example>
        /// <code>
        /// string dateStr = MdlDate.ConvertToDateString("20260801123045"); // "2026/08/01 12:30:45"
        /// </code>
        /// </example>
        public static string ConvertToDateString(string dateTimeString)
        {
            if (string.IsNullOrEmpty(dateTimeString)) return string.Empty;

            StringBuilder result = new StringBuilder();
            if (dateTimeString.Length > 3) result.Append(dateTimeString[..4]);
            if (dateTimeString.Length > 5) result.Append('/').Append(dateTimeString[4..6]);
            if (dateTimeString.Length > 7) result.Append('/').Append(dateTimeString[6..8]);
            if (dateTimeString.Length > 9) result.Append(' ').Append(dateTimeString[8..10]);
            if (dateTimeString.Length > 11) result.Append(':').Append(dateTimeString[10..12]);
            if (dateTimeString.Length > 13) result.Append(':').Append(dateTimeString[12..14]);

            return result.ToString();
        }

        /// <summary>
        /// 現在のシステム日時を UNIX 時間（1970年1月1日からの通算秒数）の文字列形式で取得します。
        /// </summary>
        /// <returns>現在の UNIX 時間の文字列表現。</returns>
        /// <example>
        /// <code>
        /// string unixStr = MdlDate.GetUnixTimeString(); // 例: "1785596400"
        /// </code>
        /// </example>
        public static string GetUnixTimeString()
        {
            return GetUnixTime(DateTime.Now).ToString();
        }

        /// <summary>
        /// 指定された日時を UNIX 時間（1970年1月1日からの通算秒数）の文字列形式で取得します。
        /// </summary>
        /// <param name="targetTime">対象の <see cref="DateTime"/>。</param>
        /// <returns>指定した日時の UNIX 時間の文字列表現。</returns>
        /// <example>
        /// <code>
        /// string unixStr = MdlDate.GetUnixTimeString(new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)); // "1785542400"
        /// </code>
        /// </example>
        public static string GetUnixTimeString(DateTime targetTime)
        {
            return GetUnixTime(targetTime).ToString();
        }

        /// <summary>
        /// 現在のシステム日時を UNIX 時間（1970年1月1日からの通算秒数）で取得します。
        /// </summary>
        /// <returns>現在の UNIX 時間（秒）。</returns>
        /// <example>
        /// <code>
        /// long unixTime = MdlDate.GetUnixTime();
        /// </code>
        /// </example>
        public static long GetUnixTime()
        {
            return GetUnixTime(DateTime.Now);
        }

        /// <summary>
        /// 指定された日時を UNIX 時間（1970年1月1日からの通算秒数）で取得します。
        /// </summary>
        /// <param name="targetTime">対象の <see cref="DateTime"/>。</param>
        /// <returns>指定した日時の UNIX 時間（秒）。</returns>
        /// <example>
        /// <code>
        /// long unixTime = MdlDate.GetUnixTime(new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)); // 1785542400
        /// </code>
        /// </example>
        public static long GetUnixTime(DateTime targetTime)
        {
            return new DateTimeOffset(targetTime).ToUnixTimeSeconds();
        }

        /// <summary>
        /// UNIX 時間の文字列をローカル日時に変換します。
        /// </summary>
        /// <param name="unixTimeString">UNIX 時間を表す文字列（秒）。</param>
        /// <returns>ローカルタイムゾーンの <see cref="DateTime"/>。</returns>
        /// <example>
        /// <code>
        /// DateTime localTime = MdlDate.ConvertUnixTimeToLocalTime("1785542400");
        /// </code>
        /// </example>
        public static DateTime ConvertUnixTimeToLocalTime(string unixTimeString)
        {
            return ConvertUnixTimeToLocalTime(MdlUtil.ParseLong(unixTimeString, 0));
        }

        /// <summary>
        /// UNIX 時間をローカル日時に変換します。
        /// </summary>
        /// <param name="unixTime">UNIX 時間（秒）。</param>
        /// <returns>ローカルタイムゾーンの <see cref="DateTime"/>。</returns>
        /// <example>
        /// <code>
        /// DateTime localTime = MdlDate.ConvertUnixTimeToLocalTime(1785542400L);
        /// </code>
        /// </example>
        public static DateTime ConvertUnixTimeToLocalTime(long unixTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
        }

        /// <summary>
        /// 現在のシステム日時を指定したフォーマット文字列で整形して取得します。
        /// </summary>
        /// <param name="format">日付フォーマット文字列（例: "yyyy/MM/dd"）。</param>
        /// <returns>フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string today = MdlDate.GetFormattedDate("yyyy/MM/dd"); // 例: "2026/08/01"
        /// </code>
        /// </example>
        public static string GetFormattedDate(string format)
        {
            return GetFormattedDate(DateTime.Now, format, false);
        }

        /// <summary>
        /// 指定した日時を指定したフォーマット文字列で整形して取得します。
        /// </summary>
        /// <param name="date">対象の <see cref="DateTime"/>。</param>
        /// <param name="format">日付フォーマット文字列（例: "yyyy/MM/dd"）。</param>
        /// <returns>フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string dateStr = MdlDate.GetFormattedDate(new DateTime(2026, 8, 1), "yyyy年MM月dd日"); // "2026年08月01日"
        /// </code>
        /// </example>
        public static string GetFormattedDate(DateTime date, string format)
        {
            return GetFormattedDate(date, format, false);
        }

        /// <summary>
        /// 指定した日時を指定したフォーマット文字列および和暦（JapaneseCalendar）オプションを使用して整形取得します。
        /// </summary>
        /// <param name="date">対象の <see cref="DateTime"/>。</param>
        /// <param name="format">日付フォーマット文字列。</param>
        /// <param name="isCulture">true の場合は ja-JP カルチャと和暦（JapaneseCalendar）を使用して整形します。</param>
        /// <returns>フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string gengoDate = MdlDate.GetFormattedDate(new DateTime(2026, 8, 1), "ggyy年MM月dd日", true); // "令和08年08月01日"
        /// </code>
        /// </example>
        public static string GetFormattedDate(DateTime date, string format, bool isCulture)
        {
            CultureInfo cultureInfo = new CultureInfo("ja-JP");
            cultureInfo.DateTimeFormat.Calendar = new JapaneseCalendar();
            return isCulture ? date.ToString(format, cultureInfo) : date.ToString(format);
        }

        /// <summary>
        /// 数字列（8桁・9桁・10桁・12桁・14桁）を解釈し、<see cref="DateTime"/> 構造体に変換を試みます。
        /// </summary>
        /// <param name="dateTimeString">変換対象の数字列文字列。</param>
        /// <param name="dateTime">変換に成功した場合の <see cref="DateTime"/>。失敗時は現在日時が格納されます。</param>
        /// <returns>変換に成功した場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// if (MdlDate.TryParseDateTime("20260801123045", out DateTime dt))
        /// {
        ///     Console.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss")); // "2026-08-01 12:30:45"
        /// }
        /// </code>
        /// </example>
        public static bool TryParseDateTime(string dateTimeString, out DateTime dateTime)
        {
            dateTime = DateTime.Now;
            if (string.IsNullOrEmpty(dateTimeString)) return false;

            string digitsOnly = NonDigitRegex().Replace(dateTimeString, "");
            if (!MdlUtil.IsNumeric(digitsOnly)) return false;

            string? pattern = digitsOnly.Length switch
            {
                8 => "yyyy/MM/dd",
                9 or 10 or 12 or 14 => "yyyy/MM/dd HH:mm:ss",
                _ => null
            };

            if (pattern is null) return false;

            string formattedString = digitsOnly.Length switch
            {
                8 => $"{digitsOnly[..4]}/{digitsOnly[4..6]}/{digitsOnly[6..8]}",
                9 => $"{digitsOnly[..4]}/{digitsOnly[4..6]}/{digitsOnly[6..8]} 0{digitsOnly[8..9]}:00:00",
                10 => $"{digitsOnly[..4]}/{digitsOnly[4..6]}/{digitsOnly[6..8]} {digitsOnly[8..10]}:00:00",
                12 => $"{digitsOnly[..4]}/{digitsOnly[4..6]}/{digitsOnly[6..8]} {digitsOnly[8..10]}:{digitsOnly[10..12]}:00",
                14 => $"{digitsOnly[..4]}/{digitsOnly[4..6]}/{digitsOnly[6..8]} {digitsOnly[8..10]}:{digitsOnly[10..12]}:{digitsOnly[12..14]}",
                _ => string.Empty
            };

            if (IsValidDate(formattedString))
            {
                try
                {
                    CultureInfo cultureInfo = new CultureInfo("ja-JP");
                    dateTime = DateTime.ParseExact(formattedString, pattern, cultureInfo, DateTimeStyles.AssumeLocal);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 不定形式の日付文字列を解析し、標準的な形式（YYYY/MM/DD または YYYY/MM/DD HH:mm:ss）に変換・検証します。
        /// </summary>
        /// <param name="dateString">入力日付文字列。</param>
        /// <param name="includeTime">時刻情報（HH:mm:ss）の補正・出力を含めるかどうか。</param>
        /// <returns>正規化された日付文字列。無効な日付の場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string fmt1 = MdlDate.ValidateAndFormatDate("2026-8-1", false); // "2026/08/01"
        /// string fmt2 = MdlDate.ValidateAndFormatDate("20260801 1230", true); // "2026/08/01 12:30:00"
        /// </code>
        /// </example>
        public static string ValidateAndFormatDate(string dateString, bool includeTime)
        {
            if (string.IsNullOrWhiteSpace(dateString)) return string.Empty;

            string formattedDate = "";
            string[] dateTimeParts = WhitespaceRegex().Split(dateString);
            string tempDate = dateTimeParts[0].Replace("-", "/").Replace(" ", "");

            if (!MdlUtil.IsNumeric(tempDate.Replace("/", "")))
            {
                return formattedDate;
            }

            string[] dateParts = tempDate.Split('/');
            switch (dateParts.Length)
            {
                case 3:
                    formattedDate = string.Format("{0,4:0000}/{1,2:00}/{2,2:00}", dateParts[0], dateParts[1], dateParts[2]).Replace(" ", "0");
                    break;
                case 1:
                    switch (tempDate.Length)
                    {
                        case 8:
                            formattedDate = $"{tempDate[..4]}/{tempDate[4..6]}/{tempDate[6..8]}";
                            break;
                        case 10 when includeTime:
                            formattedDate = $"{tempDate[..4]}/{tempDate[4..6]}/{tempDate[6..8]} {tempDate[8..10]}:00:00";
                            break;
                        case 12 when includeTime:
                            formattedDate = $"{tempDate[..4]}/{tempDate[4..6]}/{tempDate[6..8]} {tempDate[8..10]}:{tempDate[10..12]}:00";
                            break;
                        case 14 when includeTime:
                            formattedDate = $"{tempDate[..4]}/{tempDate[4..6]}/{tempDate[6..8]} {tempDate[8..10]}:{tempDate[10..12]}:{tempDate[12..14]}";
                            break;
                    }
                    break;
            }

            if (includeTime && dateTimeParts.Length > 1 && formattedDate.Length == 10)
            {
                string tempTime = dateTimeParts[1].Replace(" ", "");
                string[] timeParts = tempTime.Split(':');
                if (!MdlUtil.IsNumeric(tempTime.Replace(":", "")))
                {
                    return formattedDate;
                }

                switch (timeParts.Length)
                {
                    case 3:
                        formattedDate += " " + string.Format("{0,2:00}:{1,2:00}:{2,2:00}", timeParts[0], timeParts[1], timeParts[2]).Replace(" ", "0");
                        break;
                    case 2:
                        formattedDate += " " + string.Format("{0,2:00}:{1,2:00}", timeParts[0], timeParts[1]).Replace(" ", "0") + ":00";
                        break;
                    case 1:
                        formattedDate += tempTime.Length switch
                        {
                            2 => $" {tempTime[..2]}:00:00",
                            4 => $" {tempTime[..2]}:{tempTime[2..4]}:00",
                            6 => $" {tempTime[..2]}:{tempTime[2..4]}:{tempTime[4..6]}",
                            _ => ""
                        };
                        break;
                }
            }
            return IsValidDate(formattedDate) ? formattedDate : string.Empty;
        }

        /// <summary>
        /// 不定形式の日付文字列を解析し、標準的な日付形式（YYYY/MM/DD）に変換・検証します。
        /// </summary>
        /// <param name="dateString">入力日付文字列。</param>
        /// <returns>正規化された日付文字列。無効な日付の場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string formatted = MdlDate.ValidateAndFormatDate("2026-8-1"); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ValidateAndFormatDate(string dateString)
        {
            return ValidateAndFormatDate(dateString, false);
        }

        /// <summary>
        /// 任意文字列から正規表現を用いて日付パターン（YYYYMMDD または YYYYMMDDHHMMSS）を抽出し、YYYY/MM/DD 形式に整形します。
        /// </summary>
        /// <param name="inputString">入力文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="dateCheckType">日付検証の閾値タイプ。</param>
        /// <returns>抽出・フォーマットされた日付文字列。見つからない場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractAndFormatDateString("backup_20260801_data.zip", true, 0); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractAndFormatDateString(string inputString, bool checkDate, int dateCheckType)
        {
            string basePattern1 = PATTERN_YYYYMMDD;
            string basePattern2 = PATTERN_YYYYMMDDHHMMSS;

            string result1 = ExtractDateAny(inputString, basePattern1, checkDate, dateCheckType);
            bool isPattern1Matched = !string.IsNullOrEmpty(result1);

            string result2 = ExtractDateAny(inputString, basePattern2, checkDate, dateCheckType);
            bool isPattern2Matched = !string.IsNullOrEmpty(result2);

            if (!isPattern1Matched && !isPattern2Matched) return string.Empty;

            if (isPattern1Matched)
            {
                string res = ExtractDateStartsWith(inputString, basePattern1, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            if (isPattern2Matched)
            {
                string res = ExtractDateStartsWith(inputString, basePattern2, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }

            if (isPattern1Matched)
            {
                string res = ExtractDateContains(inputString, basePattern1, "DD", checkDate, 0, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            if (isPattern2Matched)
            {
                string res = ExtractDateContains(inputString, basePattern2, "SEC", checkDate, 0, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }

            if (isPattern1Matched)
            {
                string res = ExtractDateEndsWith(inputString, basePattern1, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            if (isPattern2Matched)
            {
                string res = ExtractDateEndsWith(inputString, basePattern2, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }

            if (isPattern1Matched)
            {
                string res = ExtractDateExact(inputString, basePattern1, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            if (isPattern2Matched)
            {
                string res = ExtractDateExact(inputString, basePattern2, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            return string.Empty;
        }

        /// <summary>
        /// ファイルパスからファイル名を取り出し、日付文字列を抽出・フォーマットします。
        /// </summary>
        /// <param name="path">入力パス文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateFromPath(@"C:\logs\20260801.log", true); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractAndFormatDateString(string path, bool checkDate)
        {
            return ExtractAndFormatDateString(path, checkDate, 0);
        }

        /// <summary>
        /// ファイルパスからファイル名を取り出し、日付文字列を抽出・フォーマットします。
        /// </summary>
        /// <param name="path">入力パス文字列。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateFromPath(@"C:\logs\20260801.log"); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractAndFormatDateString(string path)
        {
            return ExtractAndFormatDateString(path, true, 0);
        }

        /// <summary>
        /// ファイルパスからファイル名を取り出し、日付文字列を抽出・フォーマットします。
        /// </summary>
        /// <param name="path">入力パス文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="dateCheckType">日付検証の閾値タイプ。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateFromPath(@"C:\logs\20260801.log", true, 0); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateFromPath(string path, bool checkDate, int dateCheckType)
        {
            return ExtractAndFormatDateString(Path.GetFileName(path), checkDate, dateCheckType);
        }

        /// <summary>
        /// ファイルパスからファイル名を取り出し、日付文字列を抽出・フォーマットします。
        /// </summary>
        /// <param name="path">入力パス文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateFromPath(@"C:\logs\20260801.log", true); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateFromPath(string path, bool checkDate)
        {
            return ExtractAndFormatDateString(Path.GetFileName(path), checkDate, 0);
        }

        /// <summary>
        /// ファイルパスからファイル名を取り出し、日付文字列を抽出・フォーマットします。
        /// </summary>
        /// <param name="path">入力パス文字列。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateFromPath(@"C:\logs\20260801.log"); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateFromPath(string path)
        {
            return ExtractAndFormatDateString(Path.GetFileName(path), true, 0);
        }

        /// <summary>
        /// 入力文字列から末尾側優先で日付文字列を検索・抽出し、YYYY/MM/DD 形式に整形します。
        /// </summary>
        /// <param name="inputString">入力文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="dateCheckType">日付検証の閾値タイプ。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateFromStringReverse("report_20240101_20260801.pdf", true, 0); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateFromStringReverse(string inputString, bool checkDate, int dateCheckType)
        {
            string basePattern1 = PATTERN_YYYYMMDD;
            string basePattern2 = PATTERN_YYYYMMDDHHMMSS;

            string result1 = ExtractDateAny(inputString, basePattern1, checkDate, dateCheckType);
            bool isPattern1Matched = !string.IsNullOrEmpty(result1);

            string result2 = ExtractDateAny(inputString, basePattern2, checkDate, dateCheckType);
            bool isPattern2Matched = !string.IsNullOrEmpty(result2);

            if (!isPattern1Matched && !isPattern2Matched) return string.Empty;

            if (isPattern1Matched)
            {
                string res = ExtractDateEndsWith(inputString, basePattern1, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            if (isPattern2Matched)
            {
                string res = ExtractDateEndsWith(inputString, basePattern2, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }

            if (isPattern1Matched)
            {
                string res = ExtractDateContains(inputString, basePattern1, "DD", checkDate, 1, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            if (isPattern2Matched)
            {
                string res = ExtractDateContains(inputString, basePattern2, "SEC", checkDate, 1, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }

            if (isPattern1Matched)
            {
                string res = ExtractDateStartsWith(inputString, basePattern1, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            if (isPattern2Matched)
            {
                string res = ExtractDateStartsWith(inputString, basePattern2, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }

            if (isPattern1Matched)
            {
                string res = ExtractDateExact(inputString, basePattern1, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            if (isPattern2Matched)
            {
                string res = ExtractDateExact(inputString, basePattern2, checkDate, dateCheckType);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            return string.Empty;
        }

        /// <summary>
        /// 入力文字列から末尾側優先で日付文字列を検索・抽出し、YYYY/MM/DD 形式に整形します。
        /// </summary>
        /// <param name="inputString">入力文字列。</param>
        /// <param name="checkDate">日付の妥当性をチェックするかどうか。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateFromStringReverse("report_20240101_20260801.pdf", true); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateFromStringReverse(string inputString, bool checkDate)
        {
            return ExtractDateFromStringReverse(inputString, checkDate, 0);
        }

        /// <summary>
        /// 入力文字列から末尾側優先で日付文字列を検索・抽出し、YYYY/MM/DD 形式に整形します。
        /// </summary>
        /// <param name="inputString">入力文字列。</param>
        /// <returns>抽出・フォーマットされた日付文字列。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateFromStringReverse("report_20240101_20260801.pdf"); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateFromStringReverse(string inputString)
        {
            return ExtractDateFromStringReverse(inputString, true, 0);
        }

        /// <summary>
        /// 指定された正規表現パターンにマッチする最初の日付部分文字列を抽出します。
        /// </summary>
        /// <param name="input">入力文字列。</param>
        /// <param name="pattern">検索に使用する正規表現パターン。</param>
        /// <param name="validateDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="dateFormat">日付検証の閾値タイプ。</param>
        /// <returns>抽出された日付文字列（YYYY/MM/DD形式）。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateAny("text_20260801_data", MdlDate.PATTERN_YYYYMMDD, true, 0); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateAny(string input, string pattern, bool validateDate, int dateFormat)
        {
            Regex regex = pattern == PATTERN_YYYYMMDD ? YyyyMmDdRegex() :
                          pattern == PATTERN_YYYYMMDDHHMMSS ? YyyyMmDdHhMmSsRegex() :
                          new Regex(pattern);

            MatchCollection matches = regex.Matches(input);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string dateString = $"{match.Groups["YYYY"].Value}/{match.Groups["MM"].Value}/{match.Groups["DD"].Value}";
                    if (!validateDate || IsValidDate(dateString, dateFormat))
                    {
                        return dateString;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 文字列の先頭位置（^パターン）から始まる日付部分文字列を抽出します。
        /// </summary>
        /// <param name="input">入力文字列。</param>
        /// <param name="basePattern">基本となる正規表現パターン。</param>
        /// <param name="validateDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="dateFormat">日付検証の閾値タイプ。</param>
        /// <returns>抽出された日付文字列（YYYY/MM/DD形式）。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateStartsWith("20260801_text", MdlDate.PATTERN_YYYYMMDD, true, 0); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateStartsWith(string input, string basePattern, bool validateDate, int dateFormat)
        {
            string pattern = @"^" + basePattern + @"[^0-9]+";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string dateString = $"{match.Groups["YYYY"].Value}/{match.Groups["MM"].Value}/{match.Groups["DD"].Value}";
                    if (!validateDate || IsValidDate(dateString, dateFormat))
                    {
                        return dateString;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 文字列の中間位置から数字以外で挟まれた日付部分文字列を抽出します。
        /// </summary>
        /// <param name="input">入力文字列。</param>
        /// <param name="basePattern">基本となる正規表現パターン。</param>
        /// <param name="checkGroup">グループチェック名（例: "DD", "SEC"）。</param>
        /// <param name="validateDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="mode">0 の場合は最初に見つかった項目、1 の場合は最後に見つかった項目を返します。</param>
        /// <param name="dateFormat">日付検証の閾値タイプ。</param>
        /// <returns>抽出された日付文字列（YYYY/MM/DD形式）。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateContains("file_20260801_v1", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 0); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateContains(string input, string basePattern, string checkGroup, bool validateDate, int mode, int dateFormat)
        {
            string pattern = @"[^0-9]+" + basePattern;
            List<string> matchesList = [];
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);
            foreach (Match match in matches)
            {
                int checkPos = match.Groups[checkGroup].Index + 2;
                if (match.Success && input.Length > checkPos && !MdlUtil.IsNumeric(input.Substring(checkPos, 1)))
                {
                    string dateString = $"{match.Groups["YYYY"].Value}/{match.Groups["MM"].Value}/{match.Groups["DD"].Value}";
                    bool isValid = !validateDate || IsValidDate(dateString, dateFormat);
                    if (isValid)
                    {
                        if (mode == 0) return dateString;
                        matchesList.Add(dateString);
                    }
                }
            }
            return matchesList.Count > 0 ? matchesList[^1] : string.Empty;
        }

        /// <summary>
        /// 文字列の末尾位置（パターン$）で終わる日付部分文字列を抽出します。
        /// </summary>
        /// <param name="input">入力文字列。</param>
        /// <param name="basePattern">基本となる正規表現パターン。</param>
        /// <param name="validateDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="dateFormat">日付検証の閾値タイプ。</param>
        /// <returns>抽出された日付文字列（YYYY/MM/DD形式）。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateEndsWith("data_20260801", MdlDate.PATTERN_YYYYMMDD, true, 0); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateEndsWith(string input, string basePattern, bool validateDate, int dateFormat)
        {
            string pattern = @"[^0-9]+" + basePattern + @"$";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string dateString = $"{match.Groups["YYYY"].Value}/{match.Groups["MM"].Value}/{match.Groups["DD"].Value}";
                    if (!validateDate || IsValidDate(dateString, dateFormat))
                    {
                        return dateString;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 文字列全体がパターンと完全一致（^パターン$）する日付部分文字列を抽出します。
        /// </summary>
        /// <param name="input">入力文字列。</param>
        /// <param name="basePattern">基本となる正規表現パターン。</param>
        /// <param name="validateDate">日付の妥当性をチェックするかどうか。</param>
        /// <param name="dateFormat">日付検証の閾値タイプ。</param>
        /// <returns>抽出された日付文字列（YYYY/MM/DD形式）。</returns>
        /// <example>
        /// <code>
        /// string extracted = MdlDate.ExtractDateExact("20260801", MdlDate.PATTERN_YYYYMMDD, true, 0); // "2026/08/01"
        /// </code>
        /// </example>
        public static string ExtractDateExact(string input, string basePattern, bool validateDate, int dateFormat)
        {
            string pattern = @"^" + basePattern + @"$";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string dateString = $"{match.Groups["YYYY"].Value}/{match.Groups["MM"].Value}/{match.Groups["DD"].Value}";
                    if (!validateDate || IsValidDate(dateString, dateFormat))
                    {
                        return dateString;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 2つの <see cref="DateTime"/> を比較し、差分が指定した許容秒数範囲内であるかを判定します。
        /// </summary>
        /// <param name="first">比較する最初の日時。</param>
        /// <param name="second">比較する2つ目の日時。</param>
        /// <param name="secondRange">許容する秒数の範囲。</param>
        /// <returns>差分が範囲内の場合は 0。範囲外の場合で first が大きければ 1、小さければ -1。</returns>
        /// <example>
        /// <code>
        /// DateTime t1 = DateTime.Now;
        /// DateTime t2 = t1.AddSeconds(3);
        /// int cmp = MdlDate.CompareDateTime(t1, t2, 5.0); // 0 (5秒以内)
        /// </code>
        /// </example>
        public static int CompareDateTime(DateTime first, DateTime second, double secondRange)
        {
            int result = 0;
            try
            {
                int comparisonResult = first.CompareTo(second);
                if (comparisonResult > 0)
                {
                    if (Math.Abs((first - second).TotalSeconds) >= secondRange) result = 1;
                }
                else if (comparisonResult < 0)
                {
                    if (Math.Abs((second - first).TotalSeconds) >= secondRange) result = -1;
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 秒数を "HH:mm:ss" または "24時間以上の時間:mm:ss" 形式の時間文字列に変換します。
        /// </summary>
        /// <param name="seconds">変換する秒数。</param>
        /// <returns>整形された時間文字列。</returns>
        /// <example>
        /// <code>
        /// string time1 = MdlDate.ConvertSecondsToTimeString(3665); // "01:01:05"
        /// string time2 = MdlDate.ConvertSecondsToTimeString(90000); // "25:00:00"
        /// </code>
        /// </example>
        public static string ConvertSecondsToTimeString(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            string timeString;
            if (timeSpan.TotalDays > 1.0)
            {
                timeString = $"{(int)timeSpan.TotalHours}{timeSpan:\\:mm\\:ss}";
            }
            else
            {
                timeString = timeSpan.ToString(@"hh\:mm\:ss");
            }
            return timeSpan < TimeSpan.Zero ? "-" + timeString : timeString;
        }

        /// <summary>
        /// 長整数型の秒数を "HH:mm:ss" または "24時間以上の時間:mm:ss" 形式の時間文字列に変換します。
        /// </summary>
        /// <param name="seconds">変換する秒数（long型）。</param>
        /// <returns>整形された時間文字列。</returns>
        /// <example>
        /// <code>
        /// string time = MdlDate.ConvertSecondsToTimeString(3600L); // "01:00:00"
        /// </code>
        /// </example>
        public static string ConvertSecondsToTimeString(long seconds)
        {
            return ConvertSecondsToTimeString((int)seconds);
        }

        /// <summary>
        /// テンプレート文字列内の指定フォーマットプレースホルダー（%Y, %m, %d, %H, %M, %S, %w, %pid）を日時の値で置換します。
        /// </summary>
        /// <param name="target">置換対象のテンプレート文字列。</param>
        /// <param name="currentDateTime">置換に使用する日時。</param>
        /// <returns>置換後の文字列。</returns>
        /// <example>
        /// <code>
        /// string path = MdlDate.ReplaceStringWithDateTime("log_%Y%m%d_%H%M%S.txt", DateTime.Now);
        /// </code>
        /// </example>
        public static string ReplaceStringWithDateTime(string target, DateTime currentDateTime)
        {
            Dictionary<string, string> formatDictionary = new()
            {
                ["%y"] = "yy",
                ["%Y"] = "yyyy",
                ["%m"] = "MM",
                ["%d"] = "dd",
                ["%H"] = "HH",
                ["%M"] = "mm",
                ["%S"] = "ss"
            };

            foreach (var (key, value) in formatDictionary)
            {
                if (target.Contains(key))
                {
                    target = target.Replace(key, currentDateTime.ToString(value));
                }
            }

            if (target.Contains("%w"))
            {
                target = target.Replace("%w", ((int)currentDateTime.DayOfWeek).ToString());
            }

            if (target.Contains("%pid"))
            {
                target = target.Replace("%pid", System.Diagnostics.Process.GetCurrentProcess().Id.ToString());
            }

            return target;
        }

        /// <summary>
        /// テンプレート文字列内の指定フォーマットプレースホルダーを現在日時で置換します。
        /// </summary>
        /// <param name="target">置換対象のテンプレート文字列。</param>
        /// <returns>置換後の文字列。</returns>
        /// <example>
        /// <code>
        /// string path = MdlDate.ReplaceStringWithDateTime("output_%Y%m%d.csv");
        /// </code>
        /// </example>
        public static string ReplaceStringWithDateTime(string target)
        {
            return ReplaceStringWithDateTime(target, DateTime.Now);
        }

        /// <summary>
        /// 指定した日時でテンプレート文字列内の日付プレースホルダー（%Y, %m, %d, %H, %M, %S, %w）を置換します。
        /// </summary>
        /// <param name="date">置換に使用する日時。</param>
        /// <param name="target">フォーマット対象の文字列。</param>
        /// <returns>置換後の文字列。</returns>
        /// <example>
        /// <code>
        /// string str = MdlDate.ConvertFormattedDate(new DateTime(2026, 8, 1), "Date: %Y-%m-%d"); // "Date: 2026-08-01"
        /// </code>
        /// </example>
        public static string ConvertFormattedDate(DateTime date, string target)
        {
            bool isCulture = false;
            target = target.Replace("%Y", GetFormattedDate(date, "yyyy", isCulture));
            target = target.Replace("%m", GetFormattedDate(date, "MM", isCulture));
            target = target.Replace("%d", GetFormattedDate(date, "dd", isCulture));
            target = target.Replace("%H", GetFormattedDate(date, "HH", isCulture));
            target = target.Replace("%M", GetFormattedDate(date, "mm", isCulture));
            target = target.Replace("%S", GetFormattedDate(date, "ss", isCulture));
            target = target.Replace("%w", ((int)date.DayOfWeek).ToString());
            return target;
        }
    }
}
