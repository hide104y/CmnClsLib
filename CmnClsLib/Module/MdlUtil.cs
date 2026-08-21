using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Module
{
    /// <summary>
    /// 文字列操作、型変換、ファイルサイズフォーマット、CSV解析などの汎用ユーティリティ処理を提供する静的クラスです。
    /// </summary>
    public static partial class MdlUtil
    {
        static MdlUtil()
        {
            // 文字コードプロバイダの登録（Shift_JIS、EUC-JP等のエンコーディングを使用可能にする）
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// 末尾の連続した数字にマッチする正規表現オブジェクトを生成します。
        /// </summary>
        /// <returns>末尾の数字にマッチする Regex オブジェクト。</returns>
        /// <example>
        /// <code>
        /// Match m = RightDigitsRegex().Match("abc123"); // "123"
        /// </code>
        /// </example>
        [GeneratedRegex(@"\d+$")]
        private static partial Regex RightDigitsRegex();

        /// <summary>
        /// 指定された文字列を評価し、論理値（bool）を返します。
        /// "true", "yes", "y", "1" の場合は true を返します。"false", "no", "n", "0" の場合は false を返します。
        /// 数値の場合は 0 以外を true と評価し、それ以外の場合はデフォルト値を返します。
        /// </summary>
        /// <param name="value">評価対象の文字列。</param>
        /// <param name="defaultValue">評価できない場合や null の場合に返すデフォルト値。</param>
        /// <returns>評価結果の bool 値。</returns>
        /// <example>
        /// <code>
        /// bool r1 = MdlUtil.IsTrue("yes", false); // true
        /// bool r2 = MdlUtil.IsTrue("0", true);    // false
        /// bool r3 = MdlUtil.IsTrue("invalid", false); // false
        /// </code>
        /// </example>
        public static bool IsTrue(string? value, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;

            return value.Trim().ToLowerInvariant() switch
            {
                "true" or "yes" or "y" or "1" => true,
                "false" or "no" or "n" or "0" => false,
                _ => IsNumeric(value) ? Convert.ToBoolean(ParseInt(value, defaultValue ? 1 : 0)) : defaultValue
            };
        }

        /// <summary>
        /// 指定された文字列が数値表現（整数または浮動小数点数）かどうかを判定します。
        /// </summary>
        /// <param name="target">判定対象の文字列。</param>
        /// <returns>数値としてパース可能な場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool b1 = MdlUtil.IsNumeric("123.45"); // true
        /// bool b2 = MdlUtil.IsNumeric("abc");    // false
        /// </code>
        /// </example>
        public static bool IsNumeric(string? target)
        {
            if (string.IsNullOrWhiteSpace(target)) return false;
            return double.TryParse(target.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        /// <summary>
        /// 指定されたオブジェクトが数値かどうかを判定します。
        /// </summary>
        /// <param name="target">判定対象のオブジェクト。</param>
        /// <returns>数値の場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool b1 = MdlUtil.IsNumeric((object)100);     // true
        /// bool b2 = MdlUtil.IsNumeric((object)"12.34"); // true
        /// </code>
        /// </example>
        public static bool IsNumeric(object? target)
        {
            if (target is null) return false;
            if (target is sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal) return true;
            return IsNumeric(target.ToString());
        }

        /// <summary>
        /// 文字列を 32 ビット符号付き整数に変換します。変換できない場合は指定されたデフォルト値を返します。
        /// </summary>
        /// <param name="value">変換対象の文字列。</param>
        /// <param name="defaultValue">変換失敗時に返すデフォルト値。</param>
        /// <returns>変換後の整数値、またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// int val1 = MdlUtil.ParseInt("1,234", 0); // 1234
        /// int val2 = MdlUtil.ParseInt("12.34", 0); // 12
        /// int val3 = MdlUtil.ParseInt("abc", -1);  // -1
        /// </code>
        /// </example>
        public static int ParseInt(string? value, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            string clean = value.Replace(",", "");
            if (int.TryParse(clean, CultureInfo.InvariantCulture, out int iVal))
            {
                return iVal;
            }
            if (double.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out double dVal))
            {
                return (int)Math.Truncate(dVal);
            }
            return defaultValue;
        }

        /// <summary>
        /// 文字列を 32 ビット符号付き整数に変換します。[非推奨: 代わりに ParseInt を使用してください]
        /// </summary>
        /// <param name="value">変換対象の文字列。</param>
        /// <param name="defaultValue">変換失敗時に返すデフォルト値。</param>
        /// <returns>変換後の整数値、またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// int val = MdlUtil.ParseStrToInt("123", 0); // 123
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseInt()' を使用します。")]
        public static int ParseStrToInt(string? value, int defaultValue)
        {
            return ParseInt(value, defaultValue);
        }

        /// <summary>
        /// 文字列を 32 ビット符号なし整数に変換します。変換できない場合は指定されたデフォルト値を返します。
        /// </summary>
        /// <param name="value">変換対象の文字列。</param>
        /// <param name="defaultValue">変換失敗時に返すデフォルト値。</param>
        /// <returns>変換後の符号なし整数値、またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// uint val1 = MdlUtil.ParseUInt("456", 0U); // 456U
        /// uint val2 = MdlUtil.ParseUInt("abc", 99U); // 99U
        /// </code>
        /// </example>
        public static uint ParseUInt(string? value, uint defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            string clean = value.Replace(",", "");
            if (uint.TryParse(clean, CultureInfo.InvariantCulture, out uint uVal))
            {
                return uVal;
            }
            if (double.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out double dVal) && dVal >= 0)
            {
                return (uint)Math.Truncate(dVal);
            }
            return defaultValue;
        }

        /// <summary>
        /// 文字列を 32 ビット符号なし整数に変換します。[非推奨: 代わりに ParseUInt を使用してください]
        /// </summary>
        /// <param name="value">変換対象の文字列。</param>
        /// <param name="defaultValue">変換失敗時に返すデフォルト値。</param>
        /// <returns>変換後の符号なし整数値、またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// uint val = MdlUtil.ParseStrToUint("456", 0U); // 456U
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseUInt()' を使用します。")]
        public static uint ParseStrToUint(string? value, uint defaultValue)
        {
            return ParseUInt(value, defaultValue);
        }

        /// <summary>
        /// 文字列を 64 ビット符号付き整数（長整数）に変換します。変換できない場合は指定されたデフォルト値を返します。
        /// </summary>
        /// <param name="value">変換対象の文字列。</param>
        /// <param name="defaultValue">変換失敗時に返すデフォルト値。</param>
        /// <returns>変換後の長整数値、またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// long val1 = MdlUtil.ParseLong("9876543210", 0L); // 9876543210L
        /// </code>
        /// </example>
        public static long ParseLong(string? value, long defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            string clean = value.Replace(",", "");
            if (long.TryParse(clean, CultureInfo.InvariantCulture, out long lVal))
            {
                return lVal;
            }
            if (double.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out double dVal))
            {
                return (long)Math.Truncate(dVal);
            }
            return defaultValue;
        }

        /// <summary>
        /// 文字列を 64 ビット符号付き整数に変換します。[非推奨: 代わりに ParseLong を使用してください]
        /// </summary>
        /// <param name="value">変換対象の文字列。</param>
        /// <param name="defaultValue">変換失敗時に返すデフォルト値。</param>
        /// <returns>変換後の長整数値、またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// long val = MdlUtil.ParseStrToLong("9876543210", 0L); // 9876543210L
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseLong()' を使用します。")]
        public static long ParseStrToLong(string? value, long defaultValue)
        {
            return ParseLong(value, defaultValue);
        }

        /// <summary>
        /// 文字列を 64 ビット符号なし整数に変換します。変換できない場合は指定されたデフォルト値を返します。
        /// </summary>
        /// <param name="value">変換対象の文字列。</param>
        /// <param name="defaultValue">変換失敗時に返すデフォルト値。</param>
        /// <returns>変換後の符号なし長整数値、またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// ulong val = MdlUtil.ParseULong("1234567890123456789", 0UL); // 1234567890123456789UL
        /// </code>
        /// </example>
        public static ulong ParseULong(string? value, ulong defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            string clean = value.Replace(",", "");
            if (ulong.TryParse(clean, CultureInfo.InvariantCulture, out ulong ulVal))
            {
                return ulVal;
            }
            if (double.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out double dVal) && dVal >= 0)
            {
                return (ulong)Math.Truncate(dVal);
            }
            return defaultValue;
        }

        /// <summary>
        /// 文字列を 64 ビット符号なし整数に変換します。[非推奨: 代わりに ParseULong を使用してください]
        /// </summary>
        /// <param name="value">変換対象の文字列。</param>
        /// <param name="defaultValue">変換失敗時に返すデフォルト値。</param>
        /// <returns>変換後の符号なし長整数値、またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// ulong val = MdlUtil.ParseStrToUlong("1234567890123456789", 0UL); // 1234567890123456789UL
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseULong()' を使用します。")]
        public static ulong ParseStrToUlong(string? value, ulong defaultValue)
        {
            return ParseULong(value, defaultValue);
        }

        /// <summary>
        /// 文字列を倍精度浮動小数点数（double）に変換します。変換できない場合は指定されたデフォルト値を返します。
        /// </summary>
        /// <param name="value">変換対象の文字列。</param>
        /// <param name="defaultValue">変換失敗時に返すデフォルト値。</param>
        /// <returns>変換後の double 値、またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// double d1 = MdlUtil.ParseDouble("1,000.5", 0.0); // 1000.5
        /// </code>
        /// </example>
        public static double ParseDouble(string? value, double defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            if (double.TryParse(value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out double dVal))
            {
                return dVal;
            }
            return defaultValue;
        }

        /// <summary>
        /// 文字列を倍精度浮動小数点数に変換します。[非推奨: 代わりに ParseDouble を使用してください]
        /// </summary>
        /// <param name="value">変換対象の文字列。</param>
        /// <param name="defaultValue">変換失敗時に返すデフォルト値。</param>
        /// <returns>変換後の double 値、またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// double d = MdlUtil.ParseStrToDbl("100.5", 0.0); // 100.5
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseDouble()' を使用します。")]
        public static double ParseStrToDbl(string? value, double defaultValue)
        {
            return ParseDouble(value, defaultValue);
        }

        /// <summary>
        /// 文字列の前後の空白、シングルクォーテーション、ダブルクォーテーションをトリムして返します。
        /// </summary>
        /// <param name="target">対象の文字列。</param>
        /// <returns>クォーテーションを取り除いた文字列。null の場合は空文字。</returns>
        /// <example>
        /// <code>
        /// string trimmed = MdlUtil.TrimQuotes("  \"hello\"  "); // "hello"
        /// </code>
        /// </example>
        public static string TrimQuotes(string? target)
        {
            if (string.IsNullOrEmpty(target)) return string.Empty;
            return target.AsSpan().Trim().Trim("\"'").Trim().ToString();
        }

        /// <summary>
        /// 文字列の前後のクォーテーションを取り除きます。[非推奨: 代わりに TrimQuotes を使用してください]
        /// </summary>
        /// <param name="target">対象の文字列。</param>
        /// <returns>クォーテーションを取り除いた文字列。null の場合は空文字。</returns>
        /// <example>
        /// <code>
        /// string trimmed = MdlUtil.TrimStr("  'world'  "); // "world"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'TrimQuotes()' を使用します。")]
        public static string TrimStr(string? target)
        {
            return TrimQuotes(target);
        }

        /// <summary>
        /// 文字列を指定された長さに左揃え（末尾パディング）でフォーマットします。
        /// </summary>
        /// <param name="str">対象の文字列。</param>
        /// <param name="length">揃える長さ。</param>
        /// <returns>左揃えされた文字列。</returns>
        /// <example>
        /// <code>
        /// string res = MdlUtil.FormatStringLeft("abc", 5); // "abc  "
        /// </code>
        /// </example>
        public static string FormatStringLeft(string? str, int length)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (str.Length >= length) return str;
            return str.PadRight(length);
        }

        /// <summary>
        /// 文字列を指定された長さに右揃え（先頭パディング）でフォーマットします。
        /// </summary>
        /// <param name="str">対象の文字列。</param>
        /// <param name="length">揃える長さ。</param>
        /// <returns>右揃えされた文字列。</returns>
        /// <example>
        /// <code>
        /// string res = MdlUtil.FormatStringRight("abc", 5); // "  abc"
        /// </code>
        /// </example>
        public static string FormatStringRight(string? str, int length)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (str.Length >= length) return str;
            return str.PadLeft(length);
        }

        /// <summary>
        /// 文字列が "false"（大文字小文字を問わない）または null/空文字でない場合に "true" を返し、それ以外は null を返します。
        /// </summary>
        /// <param name="value">判定対象の文字列。</param>
        /// <returns>"true" または null。</returns>
        /// <example>
        /// <code>
        /// string? s1 = MdlUtil.ToBooleanStringOrNull("anything"); // "true"
        /// string? s2 = MdlUtil.ToBooleanStringOrNull("false");    // null
        /// </code>
        /// </example>
        public static string? ToBooleanStringOrNull(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return string.Equals(value.Trim(), "false", StringComparison.OrdinalIgnoreCase) ? null : "true";
        }

        /// <summary>
        /// 文字列が "false" でない場合に "true" を返します。[非推奨: 代わりに ToBooleanStringOrNull を使用してください]
        /// </summary>
        /// <param name="value">判定対象の文字列。</param>
        /// <returns>"true" または null。</returns>
        /// <example>
        /// <code>
        /// string? res = MdlUtil.GetStrTrueUnlessStrFalse("yes"); // "true"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ToBooleanStringOrNull()' を使用します。")]
        public static string? GetStrTrueUnlessStrFalse(string? value)
        {
            return ToBooleanStringOrNull(value);
        }

        /// <summary>
        /// 文字列の末尾に存在する連続した数字を削除します。
        /// </summary>
        /// <param name="input">対象の文字列。</param>
        /// <returns>末尾の数字が削除された文字列。</returns>
        /// <example>
        /// <code>
        /// string res = MdlUtil.TrimNumberRight("test1234"); // "test"
        /// </code>
        /// </example>
        public static string TrimNumberRight(string? input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return RightDigitsRegex().Replace(input.Trim(), "");
        }

        /// <summary>
        /// バイト数を人間が読みやすい単位表記（KB, MB, GB など）に変換します。
        /// </summary>
        /// <param name="bytes">バイト数。</param>
        /// <param name="unit">単位の基数（例: 1024 または 1000）。</param>
        /// <param name="digits">小数点以下の桁数（予約用パラメータ）。</param>
        /// <param name="format">数値のフォーマット文字列（例: "#,##0.##"）。</param>
        /// <param name="unitFormat">単位文字列のフォーマット（例: "{0}{1}B"）。</param>
        /// <param name="byteSuffix">バイト接頭辞。</param>
        /// <returns>フォーマットされたバイト数文字列。</returns>
        /// <example>
        /// <code>
        /// string s = MdlUtil.GetHumanReadableBytes(1536, 1024, 2, "#,##0.##", "{0}{1}B", ""); // "1.5KB"
        /// </code>
        /// </example>
        public static string GetHumanReadableBytes(double bytes, int unit, int digits, string format, string unitFormat, string byteSuffix)
        {
            string[] suffixes = [byteSuffix, "K", "M", "G", "T", "P", "E", "Z", "Y"];
            int index = 0;
            while (bytes >= unit && index < suffixes.Length - 1)
            {
                bytes /= unit;
                index++;
            }
            string suffix = index < suffixes.Length ? suffixes[index] : byteSuffix;
            return string.Format(CultureInfo.InvariantCulture, unitFormat, bytes.ToString(format, CultureInfo.InvariantCulture), suffix);
        }

        /// <summary>
        /// バイト数を人間が読みやすい形式に変換します。
        /// </summary>
        /// <param name="bytes">バイト数。</param>
        /// <param name="unit">単位の基数（例: 1024 または 1000）。</param>
        /// <param name="digits">小数点以下の桁数（予約用パラメータ）。</param>
        /// <param name="format">数値のフォーマット文字列（例: "#,##0.##"）。</param>
        /// <param name="unitFormat">単位文字列のフォーマット（例: "{0}{1}B"）。</param>
        /// <returns>フォーマットされたバイト数文字列。</returns>
        /// <example>
        /// <code>
        /// string s = MdlUtil.GetHumanReadableBytes(1024000, 1024, 2, "#,##0.##", "{0} {1}B"); // "1,000 KB"
        /// </code>
        /// </example>
        public static string GetHumanReadableBytes(double bytes, int unit, int digits, string format, string unitFormat)
        {
            return GetHumanReadableBytes(bytes, unit, digits, format, unitFormat, "");
        }

        /// <summary>
        /// バイト数を人間が読みやすい形式に変換します。
        /// </summary>
        /// <param name="bytes">バイト数。</param>
        /// <param name="unit">単位の基数（例: 1024 または 1000）。</param>
        /// <param name="digits">小数点以下の桁数（予約用パラメータ）。</param>
        /// <param name="format">数値のフォーマット文字列（例: "#,##0.##"）。</param>
        /// <returns>フォーマットされたバイト数文字列。</returns>
        /// <example>
        /// <code>
        /// string s = MdlUtil.GetHumanReadableBytes(2048, 1024, 2, "#,##0.##"); // "2KB"
        /// </code>
        /// </example>
        public static string GetHumanReadableBytes(double bytes, int unit, int digits, string format)
        {
            return GetHumanReadableBytes(bytes, unit, digits, format, "{0}{1}B", "");
        }

        /// <summary>
        /// バイト数を人間が読みやすい形式に変換します。
        /// </summary>
        /// <param name="bytes">バイト数。</param>
        /// <param name="digits">小数点以下の桁数（予約用パラメータ）。</param>
        /// <param name="format">数値のフォーマット文字列（例: "#,##0.##"）。</param>
        /// <returns>フォーマットされたバイト数文字列。</returns>
        /// <example>
        /// <code>
        /// string s = MdlUtil.GetHumanReadableBytes(5242880, 2, "#,##0.00"); // "5.00MB"
        /// </code>
        /// </example>
        public static string GetHumanReadableBytes(double bytes, int digits, string format)
        {
            return GetHumanReadableBytes(bytes, 1024, digits, format, "{0}{1}B", "");
        }

        /// <summary>
        /// バイト数を人間が読みやすい標準的な形式（1024バイト単位、小数点2桁）に変換します。
        /// </summary>
        /// <param name="bytes">バイト数。</param>
        /// <returns>フォーマットされたバイト数文字列。</returns>
        /// <example>
        /// <code>
        /// string s = MdlUtil.GetHumanReadableBytes(1048576); // "1MB"
        /// </code>
        /// </example>
        public static string GetHumanReadableBytes(double bytes)
        {
            return GetHumanReadableBytes(bytes, 1024, 2, "#,##0.##", "{0}{1}B", "");
        }

        /// <summary>
        /// バイト数をフォーマットし、指定幅で右揃えにします。
        /// </summary>
        /// <param name="bytes">バイト数。</param>
        /// <param name="width">右揃え用の文字列幅。</param>
        /// <returns>右揃えされたバイト数文字列。</returns>
        /// <example>
        /// <code>
        /// string s = MdlUtil.GetHumanReadableBytesRight(1024, 20);
        /// </code>
        /// </example>
        public static string GetHumanReadableBytesRight(double bytes, int width)
        {
            return FormatStringRight(GetHumanReadableBytes(bytes, 1024, 2, "#,##0.00", "{0} ({1}B)", " "), width);
        }

        /// <summary>
        /// バイト数をフォーマットし、標準幅（13桁）で右揃えにします。
        /// </summary>
        /// <param name="bytes">バイト数。</param>
        /// <returns>13桁に右揃えされたバイト数文字列。</returns>
        /// <example>
        /// <code>
        /// string s = MdlUtil.GetHumanReadableBytesRight(1024);
        /// </code>
        /// </example>
        public static string GetHumanReadableBytesRight(double bytes)
        {
            return GetHumanReadableBytesRight(bytes, 13);
        }

        /// <summary>
        /// 区切り文字（正規表現）で区切られた CSV 文字列をパースし、文字列のリストとして返します。
        /// </summary>
        /// <param name="list">追加先のリスト（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <param name="pattern">区切りパターンの正規表現（省略時は [,/|]）。</param>
        /// <param name="debugLevel">デバッグログレベル。</param>
        /// <param name="isUnique">重複要素を除外する場合は true。</param>
        /// <param name="isRegexTest">正規表現テストを行うフラグ。</param>
        /// <returns>パース結果の文字列リスト。</returns>
        /// <example>
        /// <code>
        /// var list = MdlUtil.ParseCsvToList(null, "apple, banana | orange");
        /// </code>
        /// </example>
        public static List<string> ParseCsvToList(List<string>? list, string csv, string? pattern = @"[,\/|]", int debugLevel = 0, bool isUnique = true, bool isRegexTest = true)
        {
            list ??= [];
            if (string.IsNullOrEmpty(csv)) return list;
            string effectivePattern = string.IsNullOrEmpty(pattern) ? @"[,\/|]" : pattern;

            if (debugLevel > 6) Console.WriteLine($"[MdlUtil.ParseCsvToList()] ARG1 : list.Count = {list.Count} / csv = {csv} / pattern = {effectivePattern}");

            string[] elements = Regex.Split(csv, effectivePattern);
            foreach (string element in elements)
            {
                string temp = element.Trim();
                if (string.IsNullOrEmpty(temp)) continue;

                if (temp.StartsWith('*')) temp = "." + temp;
                try
                {
                    if (isRegexTest && Regex.IsMatch("test", temp))
                    {
                        // 正規表現チェック（動作確認）
                    }
                    if (isUnique)
                    {
                        if (!list.Contains(temp))
                        {
                            if (debugLevel > 5) Console.WriteLine($"[MdlUtil.ParseCsvToList()] list.Add({temp})");
                            list.Add(temp);
                        }
                        else if (debugLevel > 5)
                        {
                            Console.WriteLine($"[MdlUtil.ParseCsvToList()] NOT UNIQ => SKIP list.Add({temp})");
                        }
                    }
                    else
                    {
                        if (debugLevel > 5) Console.WriteLine($"[MdlUtil.ParseCsvToList()] list.Add({temp})");
                        list.Add(temp);
                    }
                }
                catch (Exception ex)
                {
                    if (debugLevel > 5) Console.WriteLine($"[MdlUtil.ParseCsvToList()] EXCEPTION : Regex.IsMatch(\"test\", {temp}) : {ex.Message}");
                }
            }
            return list;
        }

        /// <summary>
        /// CSV文字列をリストにパースします。[非推奨: 代わりに ParseCsvToList を使用してください]
        /// </summary>
        /// <param name="list">追加先のリスト（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <param name="pattern">区切りパターンの正規表現。</param>
        /// <param name="debugLevel">デバッグログレベル。</param>
        /// <param name="isUnique">重複要素を除外する場合は true。</param>
        /// <param name="isRegexTest">正規表現テストを行うフラグ。</param>
        /// <returns>パース結果の文字列リスト。</returns>
        /// <example>
        /// <code>
        /// var list = MdlUtil.SetCsvToList(null, "a,b,c", @"[,\/|]", 0, true, true);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseCsvToList()' を使用します。")]
        public static List<string> SetCsvToList(List<string>? list, string csv, string? pattern, int debugLevel, bool isUnique, bool isRegexTest)
        {
            return ParseCsvToList(list, csv, pattern, debugLevel, isUnique, isRegexTest);
        }

        /// <summary>
        /// CSV文字列をリストにパースします。[非推奨: 代わりに ParseCsvToList を使用してください]
        /// </summary>
        /// <param name="list">追加先のリスト（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <param name="pattern">区切りパターンの正規表現。</param>
        /// <param name="debugLevel">デバッグログレベル。</param>
        /// <param name="isUnique">重複要素を除外する場合は true。</param>
        /// <returns>パース結果の文字列リスト。</returns>
        /// <example>
        /// <code>
        /// var list = MdlUtil.SetCsvToList(null, "a,b,c", @"[,\/|]", 0, true);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseCsvToList()' を使用します。")]
        public static List<string> SetCsvToList(List<string>? list, string csv, string pattern, int debugLevel, bool isUnique)
        {
            return ParseCsvToList(list, csv, pattern, debugLevel, isUnique, true);
        }

        /// <summary>
        /// CSV文字列をリストにパースします。[非推奨: 代わりに ParseCsvToList を使用してください]
        /// </summary>
        /// <param name="list">追加先のリスト（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <param name="pattern">区切りパターンの正規表現。</param>
        /// <param name="debugLevel">デバッグログレベル。</param>
        /// <returns>パース結果の文字列リスト。</returns>
        /// <example>
        /// <code>
        /// var list = MdlUtil.SetCsvToList(null, "a,b,c", @"[,\/|]", 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseCsvToList()' を使用します。")]
        public static List<string> SetCsvToList(List<string>? list, string csv, string pattern, int debugLevel)
        {
            return ParseCsvToList(list, csv, pattern, debugLevel, true, true);
        }

        /// <summary>
        /// CSV文字列をリストにパースします。[非推奨: 代わりに ParseCsvToList を使用してください]
        /// </summary>
        /// <param name="list">追加先のリスト（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <returns>パース結果の文字列リスト。</returns>
        /// <example>
        /// <code>
        /// var list = MdlUtil.SetCsvToList(null, "a,b,c");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseCsvToList()' を使用します。")]
        public static List<string> SetCsvToList(List<string>? list, string csv)
        {
            return ParseCsvToList(list, csv, @"[,\/|]", 0, true, true);
        }

        /// <summary>
        /// 区切り文字で区切られた CSV 文字列を整数リストにパースします。
        /// </summary>
        /// <param name="list">追加先のリスト（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <param name="pattern">区切りパターンの正規表現（省略時は [,/|]）。</param>
        /// <param name="debugLevel">デバッグログレベル。</param>
        /// <param name="isUnique">重複要素を除外する場合は true。</param>
        /// <returns>パース結果の整数リスト。</returns>
        /// <example>
        /// <code>
        /// var list = MdlUtil.ParseCsvToIntList(null, "10, 20, 30");
        /// </code>
        /// </example>
        public static List<int> ParseCsvToIntList(List<int>? list, string csv, string? pattern = @"[,\/|]", int debugLevel = 0, bool isUnique = true)
        {
            list ??= [];
            if (string.IsNullOrEmpty(csv)) return list;
            string effectivePattern = string.IsNullOrEmpty(pattern) ? @"[,\/|]" : pattern;

            if (debugLevel > 6) Console.WriteLine($"[MdlUtil.ParseCsvToIntList()] ARG1 : list.Count = {list.Count} / csv = {csv} / pattern = {effectivePattern}");

            string[] elements = Regex.Split(csv, effectivePattern);
            foreach (string element in elements)
            {
                string temp = element.Trim();
                int intTemp = ParseInt(temp, MdlConst.INT_NULL);
                if (MdlConst.INT_NULL != intTemp)
                {
                    if (isUnique)
                    {
                        if (!list.Contains(intTemp))
                        {
                            if (debugLevel > 5) Console.WriteLine($"[MdlUtil.ParseCsvToIntList()] list.Add({intTemp})");
                            list.Add(intTemp);
                        }
                        else if (debugLevel > 5)
                        {
                            Console.WriteLine($"[MdlUtil.ParseCsvToIntList()] NOT UNIQ => SKIP list.Add({intTemp})");
                        }
                    }
                    else
                    {
                        if (debugLevel > 5) Console.WriteLine($"[MdlUtil.ParseCsvToIntList()] list.Add({intTemp})");
                        list.Add(intTemp);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// CSV文字列を整数リストにパースします。[非推奨: 代わりに ParseCsvToIntList を使用してください]
        /// </summary>
        /// <param name="list">追加先のリスト（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <param name="pattern">区切りパターンの正規表現。</param>
        /// <param name="debugLevel">デバッグログレベル。</param>
        /// <param name="isUnique">重複要素を除外する場合は true。</param>
        /// <returns>パース結果の整数リスト。</returns>
        /// <example>
        /// <code>
        /// var list = MdlUtil.SetCsvToIntList(null, "1,2,3", @"[,\/|]", 0, true);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseCsvToIntList()' を使用します。")]
        public static List<int> SetCsvToIntList(List<int>? list, string csv, string? pattern, int debugLevel, bool isUnique)
        {
            return ParseCsvToIntList(list, csv, pattern, debugLevel, isUnique);
        }

        /// <summary>
        /// CSV文字列を整数リストにパースします。[非推奨: 代わりに ParseCsvToIntList を使用してください]
        /// </summary>
        /// <param name="list">追加先のリスト（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <param name="pattern">区切りパターンの正規表現。</param>
        /// <param name="debugLevel">デバッグログレベル。</param>
        /// <returns>パース結果の整数リスト。</returns>
        /// <example>
        /// <code>
        /// var list = MdlUtil.SetCsvToIntList(null, "1,2,3", @"[,\/|]", 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseCsvToIntList()' を使用します。")]
        public static List<int> SetCsvToIntList(List<int>? list, string csv, string pattern, int debugLevel)
        {
            return ParseCsvToIntList(list, csv, pattern, debugLevel, true);
        }

        /// <summary>
        /// CSV文字列を整数リストにパースします。[非推奨: 代わりに ParseCsvToIntList を使用してください]
        /// </summary>
        /// <param name="list">追加先のリスト（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <returns>パース結果の整数リスト。</returns>
        /// <example>
        /// <code>
        /// var list = MdlUtil.SetCsvToIntList(null, "1,2,3");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseCsvToIntList()' を使用します。")]
        public static List<int> SetCsvToIntList(List<int>? list, string csv)
        {
            return ParseCsvToIntList(list, csv, null, 0, true);
        }

        /// <summary>
        /// CSV形式の文字列をパースし、キーと値のペアを格納したディクショナリを返します。
        /// </summary>
        /// <param name="dictionary">追加先のディクショナリ（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <param name="delimiterPattern">要素の区切り正規表現（省略時は [,/|]）。</param>
        /// <param name="keyValuePattern">キーと値の区切り正規表現（省略時は =）。</param>
        /// <param name="debugLevel">デバッグログレベル。</param>
        /// <param name="isUnique">同一キーが存在する場合に上書き（true）するか無視（false）するか。</param>
        /// <param name="isRegexTest">正規表現テストを行うフラグ。</param>
        /// <returns>パース結果のディクショナリ。</returns>
        /// <example>
        /// <code>
        /// var dic = MdlUtil.ParseCsvToDictionary(null, "Key1=Val1, Key2=Val2");
        /// </code>
        /// </example>
        public static Dictionary<string, string> ParseCsvToDictionary(
            Dictionary<string, string>? dictionary,
            string csv,
            string? delimiterPattern = @"[,\/|]",
            string? keyValuePattern = "=",
            int debugLevel = 0,
            bool isUnique = true,
            bool isRegexTest = true)
        {
            dictionary ??= [];
            if (string.IsNullOrEmpty(csv)) return dictionary;
            string effectiveDelimiter = string.IsNullOrEmpty(delimiterPattern) ? @"[,\/|]" : delimiterPattern;
            string effectiveKeyValue = string.IsNullOrEmpty(keyValuePattern) ? "=" : keyValuePattern;

            if (debugLevel > 6) Console.WriteLine($"[MdlUtil.ParseCsvToDictionary()] ARG1 : dictionary.Count = {dictionary.Count} / csv = {csv} / delimiterPattern = {effectiveDelimiter} / keyValuePattern = {effectiveKeyValue}");

            string[] elements = Regex.Split(csv, effectiveDelimiter);
            foreach (string element in elements)
            {
                string temp = element.Trim();
                if (string.IsNullOrEmpty(temp)) continue;

                List<string> listElement = ParseCsvToList([], temp, effectiveKeyValue, debugLevel + 2, false, isRegexTest);
                if (listElement.Count > 1)
                {
                    string k = listElement[0];
                    string v = listElement[1];
                    if (debugLevel > 5) Console.WriteLine($"[MdlUtil.ParseCsvToDictionary()] dictionary[{k}] = {v}");
                    if (isUnique)
                    {
                        dictionary[k] = v;
                    }
                    else
                    {
                        dictionary.TryAdd(k, v);
                    }
                }
            }
            return dictionary;
        }

        /// <summary>
        /// CSV文字列をディクショナリにパースします。[非推奨: 代わりに ParseCsvToDictionary を使用してください]
        /// </summary>
        /// <param name="dictionary">追加先のディクショナリ（null の場合は新規作成されます）。</param>
        /// <param name="csv">パース対象の CSV 文字列。</param>
        /// <param name="delimiterPattern">要素の区切り正規表現。</param>
        /// <param name="keyValuePattern">キーと値の区切り正規表現。</param>
        /// <param name="debugLevel">デバッグログレベル。</param>
        /// <param name="isUnique">同一キーが存在する場合に上書き（true）するか無視（false）するか。</param>
        /// <param name="isRegexTest">正規表現テストを行うフラグ。</param>
        /// <returns>パース結果のディクショナリ。</returns>
        /// <example>
        /// <code>
        /// var dic = MdlUtil.SetCsvToDictionary(null, "k1=v1, k2=v2", @"[,\/|]", "=", 0, true, true);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ParseCsvToDictionary()' を使用します。")]
        public static Dictionary<string, string> SetCsvToDictionary(Dictionary<string, string>? dictionary, string csv, string? delimiterPattern, string? keyValuePattern, int debugLevel, bool isUnique, bool isRegexTest)
        {
            return ParseCsvToDictionary(dictionary, csv, delimiterPattern, keyValuePattern, debugLevel, isUnique, isRegexTest);
        }

        /// <summary>
        /// 指定された文字列の Shift_JIS エンコーディングでのバイト数を取得します。
        /// </summary>
        /// <param name="input">対象の文字列。</param>
        /// <returns>Shift_JIS におけるバイト数。エラー時は -1。</returns>
        /// <example>
        /// <code>
        /// int len = MdlUtil.GetShiftJisByteCount("こんにちは"); // 10
        /// </code>
        /// </example>
        public static int GetShiftJisByteCount(string? input)
        {
            if (input is null) return -1;
            try
            {
                return Encoding.GetEncoding("Shift_JIS").GetByteCount(input);
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 文字列の Shift_JIS でのバイト数を取得します。[非推奨: 代わりに GetShiftJisByteCount を使用してください]
        /// </summary>
        /// <param name="input">対象の文字列。</param>
        /// <returns>Shift_JIS におけるバイト数。エラー時は -1。</returns>
        /// <example>
        /// <code>
        /// int len = MdlUtil.GetSjisStringLength("テスト"); // 6
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetShiftJisByteCount()' を使用します。")]
        public static int GetSjisStringLength(string input)
        {
            return GetShiftJisByteCount(input);
        }

        /// <summary>
        /// 正規表現パターンで指定された名前付きキャプチャグループ "TARGET" の評価結果を取得します。
        /// </summary>
        /// <param name="input">対象の文字列。</param>
        /// <param name="pattern">正規表現パターン（例: @"(?&lt;TARGET&gt;\d+)"）。</param>
        /// <returns>一致した TARGET グループの文字列。一致しなかった場合は空文字。</returns>
        /// <example>
        /// <code>
        /// string num = MdlUtil.GetRegexTarget("ID: 12345", @"ID:\s*(?&lt;TARGET&gt;\d+)"); // "12345"
        /// </code>
        /// </example>
        public static string GetRegexTarget(string? input, string? pattern)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern)) return string.Empty;
            Regex regex = new(pattern);
            Match match = regex.Match(input);
            return match.Success ? match.Groups["TARGET"].Value : string.Empty;
        }

        /// <summary>
        /// 指定された文字列が包含パターンに合致し、かつ除外パターンに合致しないかを判定します。
        /// </summary>
        /// <param name="line">評価対象の文字列。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <param name="isOrCondition">OR 条件で評価する場合は true。</param>
        /// <param name="debugLevel">デバッグログレベル。</param>
        /// <returns>有効な場合は 1、除外された場合は 2、包含条件を満たさなかった場合は 0。</returns>
        /// <example>
        /// <code>
        /// int status = MdlUtil.IsStringEffective("log.txt", new List&lt;string&gt; { @"\.txt$" }, new List&lt;string&gt;(), true, 0); // 1
        /// </code>
        /// </example>
        public static int IsStringEffective(string line, List<string> includePatterns, List<string> excludePatterns, bool isOrCondition, int debugLevel)
        {
            int result = 1;
            if (includePatterns.Count > 0)
            {
                bool isHit = false;
                result = 0;
                foreach (string pattern in includePatterns)
                {
                    if (Regex.IsMatch(line, pattern, RegexOptions.IgnoreCase))
                    {
                        isHit = true;
                        if (debugLevel > 5) Console.WriteLine($"[IsStringEffective()][INC] HIT : {pattern} -> {line}");
                        break;
                    }
                    else if (debugLevel > 10)
                    {
                        Console.WriteLine($"[IsStringEffective()][INC] NOHIT : {pattern} -> {line}");
                    }
                }
                if (isHit)
                {
                    result = 1;
                    if (isOrCondition) return result;
                }
            }
            if (excludePatterns.Count > 0)
            {
                foreach (string pattern in excludePatterns)
                {
                    if (Regex.IsMatch(line, pattern, RegexOptions.IgnoreCase))
                    {
                        if (debugLevel > 5) Console.WriteLine($"[IsStringEffective()][EXC] HIT : {pattern} -> {line}");
                        return 2;
                    }
                    else if (debugLevel > 10)
                    {
                        Console.WriteLine($"[IsStringEffective()][EXC] NOHIT : {pattern} -> {line}");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// エンコーディング名を表す文字列から Encoding オブジェクトを取得します。
        /// </summary>
        /// <param name="encodingName">エンコーディング名（例: "UTF-8", "Shift_JIS", "EUC-JP", "MS932"）。</param>
        /// <returns>対応する Encoding オブジェクト。指定なしや不明な場合は Encoding.Default。</returns>
        /// <example>
        /// <code>
        /// Encoding enc = MdlUtil.GetEncoding("Shift_JIS");
        /// </code>
        /// </example>
        public static Encoding GetEncoding(string? encodingName)
        {
            if (string.IsNullOrWhiteSpace(encodingName)) return Encoding.Default;

            return encodingName.Trim().ToUpperInvariant() switch
            {
                "UTF8" or "UTF-8" => Encoding.UTF8,
                "UNICODE" => Encoding.Unicode,
                "ASCII" => Encoding.ASCII,
                "JIS" => Encoding.GetEncoding(50220),
                "MS932" => Encoding.GetEncoding(932),
                "SJIS" or "SHIFT_JIS" => Encoding.GetEncoding("Shift_JIS"),
                "EUC" or "EUC-JP" => Encoding.GetEncoding(51932),
                "DEFAULT" => Encoding.Default,
                _ => Encoding.GetEncoding(encodingName.Trim())
            };
        }

        /// <summary>
        /// Encoding オブジェクトから対応する標準的なエンコーディング名を取得します。
        /// </summary>
        /// <param name="encoding">Encoding オブジェクト。</param>
        /// <returns>エンコーディング名文字列。</returns>
        /// <example>
        /// <code>
        /// string name = MdlUtil.GetEncodingName(Encoding.UTF8); // "UTF-8"
        /// </code>
        /// </example>
        public static string GetEncodingName(Encoding encoding)
        {
            if (encoding == Encoding.UTF8) return "UTF-8";
            if (encoding == Encoding.Default) return "DEFAULT";
            if (encoding == Encoding.Unicode) return "UNICODE";
            if (encoding == Encoding.ASCII) return "ASCII";
            if (encoding.Equals(Encoding.GetEncoding(50220))) return "JIS";
            if (encoding.Equals(Encoding.GetEncoding(932))) return "MS932";
            if (encoding.Equals(Encoding.GetEncoding("Shift_JIS"))) return "SHIFT_JIS";
            if (encoding.Equals(Encoding.GetEncoding(51932))) return "EUC";

            return encoding.EncodingName;
        }

        /// <summary>
        /// C 言語の sprintf 風にフォーマット文字列と可変長引数を用いて文字列を組み立てます。
        /// </summary>
        /// <param name="format">フォーマット文字列。</param>
        /// <param name="args">フォーマット引数。</param>
        /// <returns>フォーマット後の文字列。</returns>
        /// <example>
        /// <code>
        /// string formatted = MdlUtil.Sprintf("Count: %d, Name: %s", 10, "Test"); // "Count: 10, Name: Test"
        /// </code>
        /// </example>
        public static string Sprintf(string format, params object[] args)
        {
            StringBuilder buffer = new();
            string specifier = string.Empty;

            char[] typeSpecifiers = ['b', 'd', 'u', 'o', 'x', 'X', 'f', 'c', 's', '%'];
            int specifierStart = 0;
            int argIndex = 0;
            bool continueLoop = true;

            while (continueLoop)
            {
                if (specifier.Length > 0) specifierStart += specifier.Length;

                int specifierEnd = format.IndexOf('%', specifierStart);
                if (specifierEnd == -1) continueLoop = false;

                int normalTextSize = (specifierEnd != -1) ? specifierEnd - specifierStart : format.Length - specifierStart;
                buffer.Append(format.AsSpan(specifierStart, normalTextSize));

                if (!continueLoop) continue;

                specifierStart = format.IndexOf('%', specifierStart);
                specifierEnd = format.IndexOfAny(typeSpecifiers, specifierStart + 1) + 1;
                specifier = format[specifierStart..specifierEnd];

                if (specifier.EndsWith('d'))
                {
                    buffer.Append((int)args[argIndex]);
                }
                else if (specifier.EndsWith('c'))
                {
                    buffer.Append((char)args[argIndex]);
                }
                else if (specifier.EndsWith('s'))
                {
                    buffer.Append(args[argIndex]?.ToString());
                }
                else if (specifier.EndsWith('%'))
                {
                    buffer.Append('%');
                }
                ++argIndex;
            }

            return buffer.ToString();
        }

        /// <summary>
        /// 要素のコレクションを指定した区切り文字で連結します。
        /// </summary>
        /// <typeparam name="T">コレクションの要素の型。</typeparam>
        /// <param name="items">連結する要素のシーケンス。</param>
        /// <param name="delimiter">区切り文字列。</param>
        /// <returns>連結された文字列。</returns>
        /// <example>
        /// <code>
        /// string joined = MdlUtil.Join(new[] { 1, 2, 3 }, ", "); // "1, 2, 3"
        /// </code>
        /// </example>
        public static string Join<T>(IEnumerable<T> items, string delimiter)
        {
            return string.Join(delimiter, items);
        }

        /// <summary>
        /// 文字列リストを指定区切り文字で連結します。
        /// </summary>
        /// <param name="list">連結対象の文字列リスト。</param>
        /// <param name="delimiter">区切り文字列。</param>
        /// <returns>連結された文字列。</returns>
        /// <example>
        /// <code>
        /// string res = MdlUtil.Join(new List&lt;string&gt; { "A", "B" }, ","); // "A,B"
        /// </code>
        /// </example>
        public static string Join(List<string> list, string delimiter) => Join<string>(list, delimiter);

        /// <summary>
        /// 文字列配列を指定区切り文字で連結します。
        /// </summary>
        /// <param name="list">連結対象の文字列配列。</param>
        /// <param name="delimiter">区切り文字列。</param>
        /// <returns>連結された文字列。</returns>
        /// <example>
        /// <code>
        /// string res = MdlUtil.Join(new[] { "X", "Y" }, "-"); // "X-Y"
        /// </code>
        /// </example>
        public static string Join(string[] list, string delimiter) => Join<string>(list, delimiter);

        /// <summary>
        /// 整数配列を指定区切り文字で連結します。
        /// </summary>
        /// <param name="list">連結対象の整数配列。</param>
        /// <param name="delimiter">区切り文字列。</param>
        /// <returns>連結された文字列。</returns>
        /// <example>
        /// <code>
        /// string res = MdlUtil.Join(new[] { 1, 2, 3 }, ":"); // "1:2:3"
        /// </code>
        /// </example>
        public static string Join(int[] list, string delimiter) => Join<int>(list, delimiter);

        /// <summary>
        /// 要素リストを結びつけます。[非推奨: 代わりに Join を使用してください]
        /// </summary>
        /// <param name="list">連結対象の文字列リスト。</param>
        /// <param name="delimiter">区切り文字列。</param>
        /// <returns>連結された文字列。</returns>
        /// <example>
        /// <code>
        /// string res = MdlUtil.JoinList(new List&lt;string&gt; { "A", "B" }, ","); // "A,B"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'Join()' を使用します。")]
        public static string JoinList(List<string> list, string delimiter) => Join(list, delimiter);

        /// <summary>
        /// 要素配列を結びつけます。[非推奨: 代わりに Join を使用してください]
        /// </summary>
        /// <param name="list">連結対象の文字列配列。</param>
        /// <param name="delimiter">区切り文字列。</param>
        /// <returns>連結された文字列。</returns>
        /// <example>
        /// <code>
        /// string res = MdlUtil.JoinList(new[] { "X", "Y" }, "-"); // "X-Y"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'Join()' を使用します。")]
        public static string JoinList(string[] list, string delimiter) => Join(list, delimiter);

        /// <summary>
        /// 整数配列を結びつけます。[非推奨: 代わりに Join を使用してください]
        /// </summary>
        /// <param name="list">連結対象の整数配列。</param>
        /// <param name="delimiter">区切り文字列。</param>
        /// <returns>連結された文字列。</returns>
        /// <example>
        /// <code>
        /// string res = MdlUtil.JoinList(new[] { 1, 2, 3 }, ":"); // "1:2:3"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'Join()' を使用します。")]
        public static string JoinList(int[] list, string delimiter) => Join(list, delimiter);

        /// <summary>
        /// 指定したキーに対応する文字列値をディクショナリから取得します。存在しない場合はデフォルト値を返します。
        /// </summary>
        /// <param name="namedArgs">辞書オブジェクト。</param>
        /// <param name="key">検索キー。</param>
        /// <param name="defaultValue">キーが存在しない場合のデフォルト値。</param>
        /// <returns>取得された文字列値またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// string val = MdlUtil.GetValByKey(dict, "key1", "default");
        /// </code>
        /// </example>
        public static string GetValByKey(Dictionary<string, string> namedArgs, string key, string defaultValue)
        {
            if (!string.IsNullOrEmpty(key) && namedArgs.TryGetValue(key, out string? val))
            {
                return val;
            }
            return defaultValue;
        }

        /// <summary>
        /// 指定したキーに対応する論理値をディクショナリから取得します。存在しない場合はデフォルト値を返します。
        /// </summary>
        /// <param name="namedArgs">辞書オブジェクト。</param>
        /// <param name="key">検索キー。</param>
        /// <param name="defaultValue">キーが存在しない場合のデフォルト値。</param>
        /// <returns>取得された bool 値またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// bool val = MdlUtil.GetValByKey(dict, "isEnable", false);
        /// </code>
        /// </example>
        public static bool GetValByKey(Dictionary<string, string> namedArgs, string key, bool defaultValue)
        {
            if (!string.IsNullOrEmpty(key) && namedArgs.TryGetValue(key, out string? val))
            {
                return IsTrue(val, defaultValue);
            }
            return defaultValue;
        }

        /// <summary>
        /// 指定したキーに対応する整数値をディクショナリから取得します。存在しない場合はデフォルト値を返します。
        /// </summary>
        /// <param name="namedArgs">辞書オブジェクト。</param>
        /// <param name="key">検索キー。</param>
        /// <param name="defaultValue">キーが存在しない場合のデフォルト値。</param>
        /// <returns>取得された整数値またはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// int val = MdlUtil.GetValByKey(dict, "count", 0);
        /// </code>
        /// </example>
        public static int GetValByKey(Dictionary<string, string> namedArgs, string key, int defaultValue)
        {
            if (!string.IsNullOrEmpty(key) && namedArgs.TryGetValue(key, out string? val))
            {
                if (IsNumeric(val))
                {
                    return ParseInt(val, defaultValue);
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 指定したキーに対応する Encoding オブジェクトをディクショナリから取得します。存在しない場合はデフォルト値を返します。
        /// </summary>
        /// <param name="namedArgs">辞書オブジェクト。</param>
        /// <param name="key">検索キー。</param>
        /// <param name="defaultValue">キーが存在しない場合のデフォルト値。</param>
        /// <returns>取得された Encoding オブジェクトまたはデフォルト値。</returns>
        /// <example>
        /// <code>
        /// Encoding enc = MdlUtil.GetValByKey(dict, "encoding", Encoding.UTF8);
        /// </code>
        /// </example>
        public static Encoding GetValByKey(Dictionary<string, string> namedArgs, string key, Encoding defaultValue)
        {
            if (!string.IsNullOrEmpty(key) && namedArgs.TryGetValue(key, out string? val))
            {
                return GetEncoding(val);
            }
            return defaultValue;
        }
    }
}
