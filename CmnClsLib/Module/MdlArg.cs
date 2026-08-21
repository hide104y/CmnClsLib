using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Module
{
    /// <summary>
    /// コマンドライン引数を解析するモジュールクラスです。
    /// </summary>
    public partial class MdlArg
    {
        [GeneratedRegex(@"^-{1,2}(?<KEY>[^-].*)$")]
        private static partial Regex KeyRegex();

        [GeneratedRegex(@"^\\-")]
        private static partial Regex EscapedDashRegex();

        /// <summary>
        /// コマンドライン引数の配列を解析し、名前付き引数のキーと値の辞書を取得します（大文字・小文字を区別します）。
        /// </summary>
        /// <param name="args">コマンドライン引数の文字列配列。</param>
        /// <returns>解析された名前付き引数の辞書。</returns>
        /// <example>
        /// <code>
        /// string[] args = new[] { "-file", "data.txt", "--verbose" };
        /// var namedArgs = MdlArg.GetNamedArgs(args);
        /// // namedArgs["file"] -> "data.txt"
        /// </code>
        /// </example>
        public static Dictionary<string, string> GetNamedArgs(string[] args)
        {
            return GetNamedArgs(args, false);
        }

        /// <summary>
        /// コマンドライン引数の配列を解析し、名前付き引数のキーと値の辞書を取得します。
        /// </summary>
        /// <param name="args">コマンドライン引数の文字列配列。</param>
        /// <param name="ignoreCase">キー解析時に小文字化して大文字小文字を区別しない場合は true。</param>
        /// <returns>解析された名前付き引数の辞書。</returns>
        /// <example>
        /// <code>
        /// string[] args = new[] { "-FILE", "data.txt" };
        /// var namedArgs = MdlArg.GetNamedArgs(args, ignoreCase: true);
        /// // namedArgs["file"] -> "data.txt"
        /// </code>
        /// </example>
        public static Dictionary<string, string> GetNamedArgs(string[] args, bool ignoreCase)
        {
            Dictionary<string, string> namedArgs = new();
            Regex regex = KeyRegex();

            for (int i = 0; i < args.Length; i++)
            {
                string key = "";
                string value = "";
                string arg = ignoreCase ? args[i].ToLower() : args[i];
                bool isMatch = false;

                Match matchForKey = regex.Match(arg);
                if (matchForKey.Success)
                {
                    key = MdlUtil.TrimQuotes(matchForKey.Groups["KEY"].Value);
                    isMatch = true;
                    // マイナス数字（-1 -1.0）の場合は除外
                    if (MdlUtil.IsNumeric(key)) isMatch = false;
                }

                if (isMatch)
                {
                    if (i < args.Length - 1)
                    {
                        value = args[i + 1];
                        // 最初の文字が「-」|「--」で始まっていて、数字でない場合は無視
                        Match matchForValue = regex.Match(value);
                        if (matchForValue.Success && !MdlUtil.IsNumeric(value))
                        {
                            value = "";
                        }
                    }

                    if (key is "h" && string.IsNullOrEmpty(value))
                    {
                        key = "help";
                    }

                    namedArgs[key] = EscapedDashRegex().Replace(value, "-");
                }
            }

            return namedArgs;
        }

        /// <summary>
        /// 指定されたキーが解析済み引数辞書内に存在するか判定します。
        /// </summary>
        /// <param name="namedArgs">名前付き引数の辞書。</param>
        /// <param name="key">判定対象のキー文字列。</param>
        /// <returns>キーが存在する場合は true。それ以外は false。</returns>
        /// <example>
        /// <code>
        /// if (MdlArg.ContainsKey(namedArgs, "help"))
        /// {
        ///     ShowHelp();
        /// }
        /// </code>
        /// </example>
        public static bool ContainsKey(Dictionary<string, string> namedArgs, string? key)
        {
            return !string.IsNullOrEmpty(key) && namedArgs?.ContainsKey(key) == true;
        }

        /// <summary>
        /// 指定されたキーが解析済み引数辞書内に存在するか判定します。
        /// </summary>
        /// <param name="namedArgs">名前付き引数の辞書。</param>
        /// <param name="key">判定対象のキー文字列。</param>
        /// <returns>キーが存在する場合は true。それ以外は false。</returns>
        /// <example>
        /// <code>
        /// if (MdlArg.IsExistParam(namedArgs, "help"))
        /// {
        ///     ShowHelp();
        /// }
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ContainsKey(namedArgs, key)' を使用します。")]
        public static bool IsExistParam(Dictionary<string, string> namedArgs, string? key)
        {
            return ContainsKey(namedArgs, key);
        }

        /// <summary>
        /// 指定されたキーに対応する引数値を取得します。
        /// </summary>
        /// <param name="namedArgs">名前付き引数の辞書。</param>
        /// <param name="key">取得するキー文字列。</param>
        /// <returns>キーに対応する値。キーが存在しないまたは無効な場合は空文字列 ("")。</returns>
        /// <example>
        /// <code>
        /// string mode = MdlArg.GetValue(namedArgs, "mode");
        /// </code>
        /// </example>
        public static string GetValue(Dictionary<string, string> namedArgs, string key)
        {
            if (!string.IsNullOrEmpty(key) && namedArgs != null && namedArgs.TryGetValue(key, out string? val))
            {
                return val;
            }
            return "";
        }

        /// <summary>
        /// 指定されたキーに対応する引数値を取得します。
        /// </summary>
        /// <param name="namedArgs">名前付き引数の辞書。</param>
        /// <param name="key">取得するキー文字列。</param>
        /// <returns>キーに対応する値。キーが存在しないまたは無効な場合は空文字列 ("")。</returns>
        /// <example>
        /// <code>
        /// string mode = MdlArg.GetValByKey(namedArgs, "mode");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetValue(namedArgs, key)' を使用します。")]
        public static string GetValByKey(Dictionary<string, string> namedArgs, string key)
        {
            return GetValue(namedArgs, key);
        }

        /// <summary>
        /// 指定されたキーに対応する値から絶対パスを取得します。
        /// </summary>
        /// <param name="namedArgs">名前付き引数の辞書。</param>
        /// <param name="key">取得するキー文字列。</param>
        /// <returns>絶対パス文字列。パスが無効またはキーが存在しない場合は空文字列 ("")。</returns>
        /// <example>
        /// <code>
        /// string fullPath = MdlArg.GetFullPath(namedArgs, "input");
        /// </code>
        /// </example>
        public static string GetFullPath(Dictionary<string, string> namedArgs, string key)
        {
            string val = GetValue(namedArgs, key);
            if (!string.IsNullOrEmpty(val))
            {
                try
                {
                    return Path.GetFullPath(val);
                }
                catch
                {
                    return "";
                }
            }
            return "";
        }

        /// <summary>
        /// 指定されたキーに対応する値から絶対パスを取得します。
        /// </summary>
        /// <param name="namedArgs">名前付き引数の辞書。</param>
        /// <param name="key">取得するキー文字列。</param>
        /// <returns>絶対パス文字列。パスが無効またはキーが存在しない場合は空文字列 ("")。</returns>
        /// <example>
        /// <code>
        /// string fullPath = MdlArg.GetPathParam(namedArgs, "input");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetFullPath(namedArgs, key)' を使用します。")]
        public static string GetPathParam(Dictionary<string, string> namedArgs, string key)
        {
            return GetFullPath(namedArgs, key);
        }
    }
}
