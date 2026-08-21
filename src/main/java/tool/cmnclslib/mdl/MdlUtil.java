package tool.cmnclslib.mdl;

import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.text.DecimalFormat;
import java.text.DecimalFormatSymbols;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * 文字列操作、型変換、ファイルサイズフォーマット、CSV解析などの汎用ユーティリティ処理を提供する静的クラスです。
 */
public final class MdlUtil {

    private static final Pattern RIGHT_DIGITS_REGEX = Pattern.compile("\\d+$");
    private static final Pattern DEFAULT_CSV_DELIMITER_REGEX = Pattern.compile("[,/|]");
    private static final String DEFAULT_CSV_PATTERN_STR = "[,/|]";

    private MdlUtil() {
        // インスタンス化防止
    }

    /**
     * 指定された文字列を評価し、論理値（boolean）を返します。
     * "true", "yes", "y", "1" の場合は true を返します。"false", "no", "n", "0" の場合は false を返します。
     * 数値の場合は 0 以外を true と評価し、それ以外の場合はデフォルト値を返します。
     *
     * @param value 評価対象の文字列
     * @param defaultValue 評価できない場合や null の場合に返すデフォルト値
     * @return 評価結果の boolean 値
     */
    public static boolean isTrue(String value, boolean defaultValue) {
        if (value == null || value.trim().isEmpty()) {
            return defaultValue;
        }

        String lower = value.trim().toLowerCase(Locale.ROOT);
        switch (lower) {
            case "true":
            case "yes":
            case "y":
            case "1":
                return true;
            case "false":
            case "no":
            case "n":
            case "0":
                return false;
            default:
                if (isNumeric(value)) {
                    return parseInt(value, defaultValue ? 1 : 0) != 0;
                }
                return defaultValue;
        }
    }

    /**
     * 指定された文字列が数値表現（整数または浮動小数点数）かどうかを判定します。
     *
     * @param target 判定対象の文字列
     * @return 数値としてパース可能な場合は true、それ以外は false
     */
    public static boolean isNumeric(String target) {
        if (target == null || target.trim().isEmpty()) {
            return false;
        }
        String clean = target.replace(",", "").trim();
        try {
            Double.parseDouble(clean);
            return true;
        } catch (NumberFormatException e) {
            return false;
        }
    }

    /**
     * 指定されたオブジェクトが数値かどうかを判定します。
     *
     * @param target 判定対象のオブジェクト
     * @return 数値の場合は true、それ以外は false
     */
    public static boolean isNumeric(Object target) {
        if (target == null) {
            return false;
        }
        if (target instanceof Number) {
            return true;
        }
        return isNumeric(target.toString());
    }

    /**
     * 文字列を 32 ビット符号付き整数に変換します。変換できない場合は指定されたデフォルト値を返します。
     *
     * @param value 変換対象の文字列
     * @param defaultValue 変換失敗時に返すデフォルト値
     * @return 変換後の整数値、またはデフォルト値
     */
    public static int parseInt(String value, int defaultValue) {
        if (value == null || value.trim().isEmpty()) {
            return defaultValue;
        }
        String clean = value.replace(",", "").trim();
        try {
            return Integer.parseInt(clean);
        } catch (NumberFormatException e1) {
            try {
                double dVal = Double.parseDouble(clean);
                return (int) dVal;
            } catch (NumberFormatException e2) {
                return defaultValue;
            }
        }
    }

    /**
     * 文字列を 32 ビット符号なし整数（long型）に変換します。変換できない場合は指定されたデフォルト値を返します。
     *
     * @param value 変換対象の文字列
     * @param defaultValue 変換失敗時に返すデフォルト値
     * @return 変換後の符号なし整数値、またはデフォルト値
     */
    public static long parseUInt(String value, long defaultValue) {
        if (value == null || value.trim().isEmpty()) {
            return defaultValue;
        }
        String clean = value.replace(",", "").trim();
        try {
            long val = Long.parseLong(clean);
            if (val >= 0 && val <= 4294967295L) {
                return val;
            }
            return defaultValue;
        } catch (NumberFormatException e1) {
            try {
                double dVal = Double.parseDouble(clean);
                if (dVal >= 0 && dVal <= 4294967295L) {
                    return (long) dVal;
                }
                return defaultValue;
            } catch (NumberFormatException e2) {
                return defaultValue;
            }
        }
    }

    /**
     * 文字列を 64 ビット符号付き整数（長整数）に変換します。変換できない場合は指定されたデフォルト値を返します。
     *
     * @param value 変換対象の文字列
     * @param defaultValue 変換失敗時に返すデフォルト値
     * @return 変換後の長整数値、またはデフォルト値
     */
    public static long parseLong(String value, long defaultValue) {
        if (value == null || value.trim().isEmpty()) {
            return defaultValue;
        }
        String clean = value.replace(",", "").trim();
        try {
            return Long.parseLong(clean);
        } catch (NumberFormatException e1) {
            try {
                double dVal = Double.parseDouble(clean);
                return (long) dVal;
            } catch (NumberFormatException e2) {
                return defaultValue;
            }
        }
    }

    /**
     * 文字列を倍精度浮動小数点数（double）に変換します。変換できない場合は指定されたデフォルト値を返します。
     *
     * @param value 変換対象の文字列
     * @param defaultValue 変換失敗時に返すデフォルト値
     * @return 変換後の double 値、またはデフォルト値
     */
    public static double parseDouble(String value, double defaultValue) {
        if (value == null || value.trim().isEmpty()) {
            return defaultValue;
        }
        String clean = value.replace(",", "").trim();
        try {
            return Double.parseDouble(clean);
        } catch (NumberFormatException e) {
            return defaultValue;
        }
    }

    /**
     * 文字列の前後の空白、シングルクォーテーション、ダブルクォーテーションをトリムして返します。
     * 全角スペースや引用符で囲まれた前後の余白も取り除きます。
     *
     * @param target 対象の文字列
     * @return クォーテーションを取り除いた文字列。null の場合は空文字
     */
    public static String trimQuotes(String target) {
        if (target == null || target.isEmpty()) {
            return "";
        }
        String result = trimWhitespace(target);
        if (result.startsWith("\"") && result.endsWith("\"") && result.length() >= 2) {
            result = result.substring(1, result.length() - 1);
            result = trimWhitespace(result);
        } else if (result.startsWith("'") && result.endsWith("'") && result.length() >= 2) {
            result = result.substring(1, result.length() - 1);
            result = trimWhitespace(result);
        }
        return result;
    }

    private static String trimWhitespace(String str) {
        int start = 0;
        int end = str.length();
        while (start < end && isWhitespaceChar(str.charAt(start))) {
            start++;
        }
        while (end > start && isWhitespaceChar(str.charAt(end - 1))) {
            end--;
        }
        return str.substring(start, end);
    }

    private static boolean isWhitespaceChar(char c) {
        return Character.isWhitespace(c) || c == '\u3000' || c == '\u00A0';
    }

    /**
     * 文字列を指定された長さに左揃え（末尾パディング）でフォーマットします。
     *
     * @param str 対象の文字列
     * @param length 揃える長さ
     * @return 左揃えされた文字列
     */
    public static String formatStringLeft(String str, int length) {
        if (str == null || str.isEmpty()) {
            return "";
        }
        if (str.length() >= length) {
            return str;
        }
        return str + " ".repeat(length - str.length());
    }

    /**
     * 文字列を指定された長さに右揃え（先頭パディング）でフォーマットします。
     *
     * @param str 対象の文字列
     * @param length 揃える長さ
     * @return 右揃えされた文字列
     */
    public static String formatStringRight(String str, int length) {
        if (str == null || str.isEmpty()) {
            return "";
        }
        if (str.length() >= length) {
            return str;
        }
        return " ".repeat(length - str.length()) + str;
    }

    /**
     * 文字列が "false"（大文字小文字を問わない）または null/空文字でない場合に "true" を返し、それ以外は null を返します。
     *
     * @param value 判定対象の文字列
     * @return "true" または null
     */
    public static String toBoolStringOrNull(String value) {
        if (value == null || value.isEmpty()) {
            return null;
        }
        return "false".equalsIgnoreCase(value.trim()) ? null : "true";
    }

    /**
     * @deprecated {@link #toBoolStringOrNull(String)} を使用してください。
     */
    @Deprecated
    public static String toBooleanStringOrNull(String value) {
        return toBoolStringOrNull(value);
    }

    /**
     * 文字列の末尾に存在する連続した数字を削除します。
     *
     * @param input 対象の文字列
     * @return 末尾の数字が削除された文字列
     */
    public static String trimNumberRight(String input) {
        if (input == null || input.isEmpty()) {
            return "";
        }
        return RIGHT_DIGITS_REGEX.matcher(input.trim()).replaceAll("");
    }

    /**
     * バイト数を人間が読みやすい単位表記（KB, MB, GB など）に変換します。
     *
     * @param bytes バイト数
     * @param unit 単位の基数（例: 1024 または 1000）
     * @param digits 小数点以下の桁数（予約用パラメータ）
     * @param format 数値のフォーマット文字列（例: "#,##0.##"）
     * @param unitFormat 単位文字列のフォーマット（例: "{0}{1}B"）
     * @param byteSuffix バイト接頭辞
     * @return フォーマットされたバイト数文字列
     */
    public static String formatByteSize(double bytes, int unit, int digits, String format, String unitFormat, String byteSuffix) {
        String[] suffixes = new String[] {byteSuffix, "K", "M", "G", "T", "P", "E", "Z", "Y"};
        int index = 0;
        double currentBytes = bytes;
        while (currentBytes >= unit && index < suffixes.length - 1) {
            currentBytes /= unit;
            index++;
        }
        String suffix = index < suffixes.length ? suffixes[index] : byteSuffix;

        DecimalFormatSymbols symbols = new DecimalFormatSymbols(Locale.US);
        DecimalFormat df = new DecimalFormat(format, symbols);
        String formattedNum = df.format(currentBytes);

        return unitFormat.replace("{0}", formattedNum).replace("{1}", suffix);
    }

    /**
     * @deprecated {@link #formatByteSize(double, int, int, String, String, String)} を使用してください。
     */
    @Deprecated
    public static String getHumanReadableBytes(double bytes, int unit, int digits, String format, String unitFormat, String byteSuffix) {
        return formatByteSize(bytes, unit, digits, format, unitFormat, byteSuffix);
    }

    /**
     * バイト数を人間が読みやすい形式に変換します。
     *
     * @param bytes バイト数
     * @param unit 単位の基数（例: 1024 または 1000）
     * @param digits 小数点以下の桁数
     * @param format 数値のフォーマット文字列
     * @param unitFormat 単位文字列のフォーマット
     * @return フォーマットされたバイト数文字列
     */
    public static String formatByteSize(double bytes, int unit, int digits, String format, String unitFormat) {
        return formatByteSize(bytes, unit, digits, format, unitFormat, "");
    }

    /**
     * @deprecated {@link #formatByteSize(double, int, int, String, String)} を使用してください。
     */
    @Deprecated
    public static String getHumanReadableBytes(double bytes, int unit, int digits, String format, String unitFormat) {
        return formatByteSize(bytes, unit, digits, format, unitFormat);
    }

    /**
     * バイト数を人間が読みやすい形式に変換します。
     *
     * @param bytes バイト数
     * @param unit 単位の基数（例: 1024 または 1000）
     * @param digits 小数点以下の桁数
     * @param format 数値のフォーマット文字列
     * @return フォーマットされたバイト数文字列
     */
    public static String formatByteSize(double bytes, int unit, int digits, String format) {
        return formatByteSize(bytes, unit, digits, format, "{0}{1}B", "");
    }

    /**
     * @deprecated {@link #formatByteSize(double, int, int, String)} を使用してください。
     */
    @Deprecated
    public static String getHumanReadableBytes(double bytes, int unit, int digits, String format) {
        return formatByteSize(bytes, unit, digits, format);
    }

    /**
     * バイト数を人間が読みやすい形式に変換します。
     *
     * @param bytes バイト数
     * @param digits 小数点以下の桁数
     * @param format 数値のフォーマット文字列
     * @return フォーマットされたバイト数文字列
     */
    public static String formatByteSize(double bytes, int digits, String format) {
        return formatByteSize(bytes, 1024, digits, format, "{0}{1}B", "");
    }

    /**
     * @deprecated {@link #formatByteSize(double, int, String)} を使用してください。
     */
    @Deprecated
    public static String getHumanReadableBytes(double bytes, int digits, String format) {
        return formatByteSize(bytes, digits, format);
    }

    /**
     * バイト数を人間が読みやすい標準的な形式（1024バイト単位、小数点2桁）に変換します。
     *
     * @param bytes バイト数
     * @return フォーマットされたバイト数文字列
     */
    public static String formatByteSize(double bytes) {
        return formatByteSize(bytes, 1024, 2, "#,##0.##", "{0}{1}B", "");
    }

    /**
     * @deprecated {@link #formatByteSize(double)} を使用してください。
     */
    @Deprecated
    public static String getHumanReadableBytes(double bytes) {
        return formatByteSize(bytes);
    }

    /**
     * バイト数をフォーマットし、指定幅で右揃えにします。
     *
     * @param bytes バイト数
     * @param width 右揃え用の文字列幅
     * @return 右揃えされたバイト数文字列
     */
    public static String formatByteSizeRight(double bytes, int width) {
        return formatStringRight(formatByteSize(bytes, 1024, 2, "#,##0.00", "{0} ({1}B)", " "), width);
    }

    /**
     * @deprecated {@link #formatByteSizeRight(double, int)} を使用してください。
     */
    @Deprecated
    public static String getHumanReadableBytesRight(double bytes, int width) {
        return formatByteSizeRight(bytes, width);
    }

    /**
     * バイト数をフォーマットし、標準幅（13桁）で右揃えにします。
     *
     * @param bytes バイト数
     * @return 13桁に右揃えされたバイト数文字列
     */
    public static String formatByteSizeRight(double bytes) {
        return formatByteSizeRight(bytes, 13);
    }

    /**
     * @deprecated {@link #formatByteSizeRight(double)} を使用してください。
     */
    @Deprecated
    public static String getHumanReadableBytesRight(double bytes) {
        return formatByteSizeRight(bytes);
    }

    /**
     * 区切り文字（正規表現）で区切られた CSV 文字列をパースし、文字列のリストとして返します。
     *
     * @param list 追加先のリスト（null の場合は新規作成されます）
     * @param csv パース対象の CSV 文字列
     * @param pattern 区切りパターンの正規表現（null/空文字の場合は [,/|]）
     * @param debugLevel デバッグログレベル
     * @param isUnique 重複要素を除外する場合は true
     * @param isRegexTest 正規表現テストを行うフラグ
     * @return パース結果の文字列リスト
     */
    public static List<String> parseCsvToList(List<String> list, String csv, String pattern, int debugLevel, boolean isUnique, boolean isRegexTest) {
        List<String> targetList = list != null ? list : new ArrayList<>();
        if (csv == null || csv.isEmpty()) {
            return targetList;
        }
        String effectivePattern = (pattern == null || pattern.isEmpty()) ? DEFAULT_CSV_PATTERN_STR : pattern;

        if (debugLevel > 6) {
            System.out.println("[MdlUtil.parseCsvToList()] ARG1 : list.size = " + targetList.size() + " / csv = " + csv + " / pattern = " + effectivePattern);
        }

        String[] elements = csv.split(effectivePattern);
        for (String element : elements) {
            String temp = element.trim();
            if (temp.isEmpty()) {
                continue;
            }
            if (temp.startsWith("*")) {
                temp = "." + temp;
            }
            try {
                if (isRegexTest) {
                    Pattern.compile(temp);
                }
                if (isUnique) {
                    if (!targetList.contains(temp)) {
                        if (debugLevel > 5) {
                            System.out.println("[MdlUtil.parseCsvToList()] list.add(" + temp + ")");
                        }
                        targetList.add(temp);
                    } else if (debugLevel > 5) {
                        System.out.println("[MdlUtil.parseCsvToList()] NOT UNIQ => SKIP list.add(" + temp + ")");
                    }
                } else {
                    if (debugLevel > 5) {
                        System.out.println("[MdlUtil.parseCsvToList()] list.add(" + temp + ")");
                    }
                    targetList.add(temp);
                }
            } catch (Exception ex) {
                if (debugLevel > 5) {
                    System.out.println("[MdlUtil.parseCsvToList()] EXCEPTION : Pattern.compile(" + temp + ") : " + ex.getMessage());
                }
            }
        }
        return targetList;
    }

    public static List<String> parseCsvToList(List<String> list, String csv, String pattern, int debugLevel, boolean isUnique) {
        return parseCsvToList(list, csv, pattern, debugLevel, isUnique, true);
    }

    /**
     * CSV 文字列をパースして文字列リストを取得します（デフォルト設定）。
     *
     * @param list 追加先のリスト
     * @param csv パース対象の CSV 文字列
     * @return パース結果の文字列リスト
     */
    public static List<String> parseCsvToList(List<String> list, String csv) {
        return parseCsvToList(list, csv, DEFAULT_CSV_PATTERN_STR, 0, true, true);
    }

    /**
     * 文字列の末尾の空白文字を取り除きます。
     *
     * @param str 対象文字列
     * @return 末尾の空白が削除された文字列
     */
    public static String trimEnd(String str) {
        if (str == null || str.isEmpty()) {
            return "";
        }
        int end = str.length();
        while (end > 0 && isWhitespaceChar(str.charAt(end - 1))) {
            end--;
        }
        return str.substring(0, end);
    }

    /**
     * 区切り文字で区切られた CSV 文字列を整数リストにパースします。
     *
     * @param list 追加先のリスト（null の場合は新規作成されます）
     * @param csv パース対象の CSV 文字列
     * @param pattern 区切りパターンの正規表現（null/空文字の場合は [,/|]）
     * @param debugLevel デバッグログレベル
     * @param isUnique 重複要素を除外する場合は true
     * @return パース結果の整数リスト
     */
    public static List<Integer> parseCsvToInts(List<Integer> list, String csv, String pattern, int debugLevel, boolean isUnique) {
        List<Integer> targetList = list != null ? list : new ArrayList<>();
        if (csv == null || csv.isEmpty()) {
            return targetList;
        }
        String effectivePattern = (pattern == null || pattern.isEmpty()) ? DEFAULT_CSV_PATTERN_STR : pattern;

        if (debugLevel > 6) {
            System.out.println("[MdlUtil.parseCsvToInts()] ARG1 : list.size = " + targetList.size() + " / csv = " + csv + " / pattern = " + effectivePattern);
        }

        String[] elements = csv.split(effectivePattern);
        for (String element : elements) {
            String temp = element.trim();
            int intTemp = parseInt(temp, MdlConst.INT_NULL);
            if (intTemp != MdlConst.INT_NULL) {
                if (isUnique) {
                    if (!targetList.contains(intTemp)) {
                        if (debugLevel > 5) {
                            System.out.println("[MdlUtil.parseCsvToInts()] list.add(" + intTemp + ")");
                        }
                        targetList.add(intTemp);
                    } else if (debugLevel > 5) {
                        System.out.println("[MdlUtil.parseCsvToInts()] NOT UNIQ => SKIP list.add(" + intTemp + ")");
                    }
                } else {
                    if (debugLevel > 5) {
                        System.out.println("[MdlUtil.parseCsvToInts()] list.add(" + intTemp + ")");
                    }
                    targetList.add(intTemp);
                }
            }
        }
        return targetList;
    }

    /**
     * @deprecated {@link #parseCsvToInts(List, String, String, int, boolean)} を使用してください。
     */
    @Deprecated
    public static List<Integer> parseCsvToIntList(List<Integer> list, String csv, String pattern, int debugLevel, boolean isUnique) {
        return parseCsvToInts(list, csv, pattern, debugLevel, isUnique);
    }

    /**
     * CSV 文字列を整数リストにパースします（デフォルト設定）。
     *
     * @param list 追加先のリスト
     * @param csv パース対象の CSV 文字列
     * @return パース結果の整数リスト
     */
    public static List<Integer> parseCsvToInts(List<Integer> list, String csv) {
        return parseCsvToInts(list, csv, DEFAULT_CSV_PATTERN_STR, 0, true);
    }

    /**
     * @deprecated {@link #parseCsvToInts(List, String)} を使用してください。
     */
    @Deprecated
    public static List<Integer> parseCsvToIntList(List<Integer> list, String csv) {
        return parseCsvToInts(list, csv);
    }

    /**
     * CSV形式の文字列をパースし、キーと値のペアを格納したMapを返します。
     *
     * @param dictionary 追加先のMap（null の場合は新規作成されます）
     * @param csv パース対象の CSV 文字列
     * @param delimiterPattern 要素の区切り正規表現（null/空文字時は [,/|]）
     * @param keyValuePattern キーと値の区切り正規表現（null/空文字時は =）
     * @param debugLevel デバッグログレベル
     * @param isUnique 同一キーが存在する場合に上書き（true）するか無視（false）するか
     * @param isRegexTest 正規表現テストを行うフラグ
     * @return パース結果のMap
     */
    public static Map<String, String> parseCsvToMap(
            Map<String, String> dictionary,
            String csv,
            String delimiterPattern,
            String keyValuePattern,
            int debugLevel,
            boolean isUnique,
            boolean isRegexTest) {

        Map<String, String> targetMap = dictionary != null ? dictionary : new LinkedHashMap<>();
        if (csv == null || csv.isEmpty()) {
            return targetMap;
        }
        String effectiveDelimiter = (delimiterPattern == null || delimiterPattern.isEmpty()) ? DEFAULT_CSV_PATTERN_STR : delimiterPattern;
        String effectiveKeyValue = (keyValuePattern == null || keyValuePattern.isEmpty()) ? "=" : keyValuePattern;

        if (debugLevel > 6) {
            System.out.println("[MdlUtil.parseCsvToMap()] ARG1 : size = " + targetMap.size() + " / csv = " + csv
                    + " / delimiter = " + effectiveDelimiter + " / keyValue = " + effectiveKeyValue);
        }

        String[] elements = csv.split(effectiveDelimiter);
        for (String element : elements) {
            String temp = element.trim();
            if (temp.isEmpty()) {
                continue;
            }
            List<String> listElement = parseCsvToList(new ArrayList<>(), temp, effectiveKeyValue, debugLevel + 2, false, isRegexTest);
            if (listElement.size() > 1) {
                String k = listElement.get(0);
                String v = listElement.get(1);
                if (debugLevel > 5) {
                    System.out.println("[MdlUtil.parseCsvToMap()] map[" + k + "] = " + v);
                }
                if (isUnique) {
                    targetMap.put(k, v);
                } else {
                    targetMap.putIfAbsent(k, v);
                }
            }
        }
        return targetMap;
    }

    /**
     * @deprecated {@link #parseCsvToMap(Map, String, String, String, int, boolean, boolean)} を使用してください。
     */
    @Deprecated
    public static Map<String, String> parseCsvToDictionary(
            Map<String, String> dictionary,
            String csv,
            String delimiterPattern,
            String keyValuePattern,
            int debugLevel,
            boolean isUnique,
            boolean isRegexTest) {
        return parseCsvToMap(dictionary, csv, delimiterPattern, keyValuePattern, debugLevel, isUnique, isRegexTest);
    }

    /**
     * CSV形式の文字列をパースし、キーと値のペアを格納したMapを返します（デフォルト設定）。
     *
     * @param dictionary 追加先のMap
     * @param csv パース対象の CSV 文字列
     * @return パース結果のMap
     */
    public static Map<String, String> parseCsvToMap(Map<String, String> dictionary, String csv) {
        return parseCsvToMap(dictionary, csv, DEFAULT_CSV_PATTERN_STR, "=", 0, true, true);
    }

    /**
     * @deprecated {@link #parseCsvToMap(Map, String)} を使用してください。
     */
    @Deprecated
    public static Map<String, String> parseCsvToDictionary(Map<String, String> dictionary, String csv) {
        return parseCsvToMap(dictionary, csv);
    }

    /**
     * 指定された文字列の Shift_JIS エンコーディングでのバイト数を取得します。
     *
     * @param input 対象の文字列
     * @return Shift_JIS におけるバイト数。エラー時は -1
     */
    public static int getShiftJisByteCount(String input) {
        if (input == null) {
            return -1;
        }
        try {
            return input.getBytes(Charset.forName("MS932")).length;
        } catch (Exception e) {
            try {
                return input.getBytes(Charset.forName("Shift_JIS")).length;
            } catch (Exception ex) {
                return -1;
            }
        }
    }

    /**
     * 正規表現パターンで指定された名前付きキャプチャグループ "TARGET" の評価結果を取得します。
     *
     * @param input 対象の文字列
     * @param pattern 正規表現パターン（例: {@code @"(?<TARGET>\d+)"}）
     * @return 一致した TARGET グループの文字列。一致しなかった場合は空文字
     */
    public static String getRegexTarget(String input, String pattern) {
        if (input == null || input.isEmpty() || pattern == null || pattern.isEmpty()) {
            return "";
        }
        try {
            Pattern regex = Pattern.compile(pattern);
            Matcher matcher = regex.matcher(input);
            if (matcher.find()) {
                return matcher.group("TARGET");
            }
        } catch (Exception e) {
            // パターンエラー時は空文字返却
        }
        return "";
    }

    /**
     * 指定された文字列が包含パターンに合致し、かつ除外パターンに合致しないかを判定します。
     *
     * @param line 評価対象の文字列
     * @param includePatterns 包含する正規表現パターンのリスト
     * @param excludePatterns 除外する正規表現パターンのリスト
     * @param isOrCondition OR 条件で評価する場合は true
     * @param debugLevel デバッグログレベル
     * @return 有効な場合は 1、除外された場合は 2、包含条件を満たさなかった場合は 0
     */
    public static int isStringEffective(String line, List<String> includePatterns, List<String> excludePatterns, boolean isOrCondition, int debugLevel) {
        int result = 1;
        if (includePatterns != null && !includePatterns.isEmpty()) {
            boolean isHit = false;
            result = 0;
            for (String pattern : includePatterns) {
                try {
                    Pattern p = Pattern.compile(pattern, Pattern.CASE_INSENSITIVE);
                    if (p.matcher(line).find()) {
                        isHit = true;
                        if (debugLevel > 5) {
                            System.out.println("[isStringEffective()][INC] HIT : " + pattern + " -> " + line);
                        }
                        break;
                    } else if (debugLevel > 10) {
                        System.out.println("[isStringEffective()][INC] NOHIT : " + pattern + " -> " + line);
                    }
                } catch (Exception e) {
                    // パターン構文エラー時は無視
                }
            }
            if (isHit) {
                result = 1;
                if (isOrCondition) {
                    return result;
                }
            }
        }
        if (excludePatterns != null && !excludePatterns.isEmpty()) {
            for (String pattern : excludePatterns) {
                try {
                    Pattern p = Pattern.compile(pattern, Pattern.CASE_INSENSITIVE);
                    if (p.matcher(line).find()) {
                        if (debugLevel > 5) {
                            System.out.println("[isStringEffective()][EXC] HIT : " + pattern + " -> " + line);
                        }
                        return 2;
                    } else if (debugLevel > 10) {
                        System.out.println("[isStringEffective()][EXC] NOHIT : " + pattern + " -> " + line);
                    }
                } catch (Exception e) {
                    // パターン構文エラー時は無視
                }
            }
        }
        return result;
    }

    /**
     * エンコーディング名を表す文字列から Charset オブジェクトを取得します。
     *
     * @param encodingName エンコーディング名（例: "UTF-8", "Shift_JIS", "EUC-JP", "MS932"）
     * @return 対応する Charset オブジェクト。指定なしや不明な場合は Charset.defaultCharset()
     */
    public static Charset getEncoding(String encodingName) {
        if (encodingName == null || encodingName.trim().isEmpty()) {
            return Charset.defaultCharset();
        }

        String upper = encodingName.trim().toUpperCase(Locale.ROOT);
        switch (upper) {
            case "UTF8":
            case "UTF-8":
                return StandardCharsets.UTF_8;
            case "UNICODE":
            case "UTF-16":
                return StandardCharsets.UTF_16;
            case "ASCII":
            case "US-ASCII":
                return StandardCharsets.US_ASCII;
            case "JIS":
            case "ISO-2022-JP":
                return Charset.forName("ISO-2022-JP");
            case "MS932":
                return Charset.forName("MS932");
            case "SJIS":
            case "SHIFT_JIS":
            case "SHIFT-JIS":
                return Charset.forName("Shift_JIS");
            case "EUC":
            case "EUC-JP":
                return Charset.forName("EUC-JP");
            case "DEFAULT":
                return Charset.defaultCharset();
            default:
                try {
                    return Charset.forName(encodingName.trim());
                } catch (Exception e) {
                    return Charset.defaultCharset();
                }
        }
    }

    /**
     * Charset オブジェクトから対応する標準的なエンコーディング名を取得します。
     *
     * @param charset Charset オブジェクト
     * @return エンコーディング名文字列
     */
    public static String getEncodingName(Charset charset) {
        if (charset == null) {
            return "DEFAULT";
        }
        if (charset.equals(StandardCharsets.UTF_8)) {
            return "UTF-8";
        }
        if (charset.equals(Charset.defaultCharset())) {
            return "DEFAULT";
        }
        if (charset.equals(StandardCharsets.UTF_16) || charset.equals(StandardCharsets.UTF_16LE) || charset.equals(StandardCharsets.UTF_16BE)) {
            return "UNICODE";
        }
        if (charset.equals(StandardCharsets.US_ASCII)) {
            return "ASCII";
        }
        if (charset.name().equalsIgnoreCase("ISO-2022-JP")) {
            return "JIS";
        }
        if (charset.name().equalsIgnoreCase("MS932")) {
            return "MS932";
        }
        if (charset.name().equalsIgnoreCase("Shift_JIS")) {
            return "SHIFT_JIS";
        }
        if (charset.name().equalsIgnoreCase("EUC-JP")) {
            return "EUC";
        }

        return charset.name();
    }

    /**
     * C 言語の sprintf 風にフォーマット文字列と可変長引数を用いて文字列を組み立てます。
     *
     * @param format フォーマット文字列
     * @param args フォーマット引数
     * @return フォーマット後の文字列
     */
    public static String sprintf(String format, Object... args) {
        if (format == null) {
            return "";
        }
        return String.format(Locale.ROOT, format, args);
    }

    /**
     * 要素のコレクションを指定した区切り文字で連結します。
     *
     * @param items 連結する要素のシーケンス
     * @param delimiter 区切り文字列
     * @return 連結された文字列
     */
    public static String join(Iterable<?> items, String delimiter) {
        if (items == null) {
            return "";
        }
        String sep = delimiter != null ? delimiter : "";
        StringBuilder sb = new StringBuilder();
        boolean first = true;
        for (Object item : items) {
            if (!first) {
                sb.append(sep);
            }
            if (item != null) {
                sb.append(item);
            }
            first = false;
        }
        return sb.toString();
    }

    /**
     * 文字列配列を指定区切り文字で連結します。
     *
     * @param list 連結対象の文字列配列
     * @param delimiter 区切り文字列
     * @return 連結された文字列
     */
    public static String join(String[] list, String delimiter) {
        if (list == null) {
            return "";
        }
        return String.join(delimiter != null ? delimiter : "", list);
    }

    /**
     * 整数配列を指定区切り文字で連結します。
     *
     * @param list 連結対象の整数配列
     * @param delimiter 区切り文字列
     * @return 連結された文字列
     */
    public static String join(int[] list, String delimiter) {
        if (list == null) {
            return "";
        }
        String sep = delimiter != null ? delimiter : "";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < list.length; i++) {
            if (i > 0) {
                sb.append(sep);
            }
            sb.append(list[i]);
        }
        return sb.toString();
    }

    /**
     * 指定したキーに対応する文字列値をディクショナリから取得します。存在しない場合はデフォルト値を返します。
     *
     * @param namedArgs 辞書オブジェクト
     * @param key 検索キー
     * @param defaultValue キーが存在しない場合のデフォルト値
     * @return 取得された文字列値またはデフォルト値
     */
    public static String getValByKey(Map<String, String> namedArgs, String key, String defaultValue) {
        if (namedArgs != null && key != null && namedArgs.containsKey(key)) {
            String val = namedArgs.get(key);
            return val != null ? val : defaultValue;
        }
        return defaultValue;
    }

    /**
     * 指定したキーに対応する論理値をディクショナリから取得します。存在しない場合はデフォルト値を返します。
     *
     * @param namedArgs 辞書オブジェクト
     * @param key 検索キー
     * @param defaultValue キーが存在しない場合のデフォルト値
     * @return 取得された boolean 値またはデフォルト値
     */
    public static boolean getValByKey(Map<String, String> namedArgs, String key, boolean defaultValue) {
        if (namedArgs != null && key != null && namedArgs.containsKey(key)) {
            String val = namedArgs.get(key);
            return isTrue(val, defaultValue);
        }
        return defaultValue;
    }

    /**
     * 指定したキーに対応する整数値をディクショナリから取得します。存在しない場合はデフォルト値を返します。
     *
     * @param namedArgs 辞書オブジェクト
     * @param key 検索キー
     * @param defaultValue キーが存在しない場合のデフォルト値
     * @return 取得された整数値またはデフォルト値
     */
    public static int getValByKey(Map<String, String> namedArgs, String key, int defaultValue) {
        if (namedArgs != null && key != null && namedArgs.containsKey(key)) {
            String val = namedArgs.get(key);
            if (isNumeric(val)) {
                return parseInt(val, defaultValue);
            }
        }
        return defaultValue;
    }

    /**
     * 指定したキーに対応する Charset オブジェクトをディクショナリから取得します。存在しない場合はデフォルト値を返します。
     *
     * @param namedArgs 辞書オブジェクト
     * @param key 検索キー
     * @param defaultValue キーが存在しない場合のデフォルト値
     * @return 取得された Charset オブジェクトまたはデフォルト値
     */
    public static Charset getValByKey(Map<String, String> namedArgs, String key, Charset defaultValue) {
        if (namedArgs != null && key != null && namedArgs.containsKey(key)) {
            String val = namedArgs.get(key);
            return getEncoding(val);
        }
        return defaultValue;
    }
}
