package tool.cmnclslib.mdl;

import java.io.File;
import java.util.LinkedHashMap;
import java.util.Locale;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * コマンドライン引数を解析するモジュールクラスです。
 */
public final class MdlArg {

    private static final Pattern KEY_REGEX = Pattern.compile("^-{1,2}(?<KEY>[^-].*)$");
    private static final Pattern ESCAPED_DASH_REGEX = Pattern.compile("^\\\\-");

    private MdlArg() {
        // インスタンス化防止
    }

    /**
     * コマンドライン引数の配列を解析し、名前付き引数のキーと値のマップを取得します（大文字・小文字を区別します）。
     *
     * @param args コマンドライン引数の文字列配列
     * @return 解析された名前付き引数のマップ
     */
    public static Map<String, String> getNamedArgs(String[] args) {
        return getNamedArgs(args, false);
    }

    /**
     * コマンドライン引数の配列を解析し、名前付き引数のキーと値のマップを取得します。
     *
     * @param args コマンドライン引数の文字列配列
     * @param ignoreCase キー解析時に小文字化して大文字小文字を区別しない場合は true
     * @return 解析された名前付き引数のマップ
     */
    public static Map<String, String> getNamedArgs(String[] args, boolean ignoreCase) {
        Map<String, String> namedArgs = new LinkedHashMap<>();
        if (args == null || args.length == 0) {
            return namedArgs;
        }

        for (int i = 0; i < args.length; i++) {
            String key = "";
            String value = "";
            String arg = (ignoreCase && args[i] != null) ? args[i].toLowerCase(Locale.ROOT) : args[i];
            if (arg == null) {
                continue;
            }
            boolean isMatch = false;

            Matcher matchForKey = KEY_REGEX.matcher(arg);
            if (matchForKey.matches()) {
                key = MdlUtil.trimQuotes(matchForKey.group("KEY"));
                isMatch = true;
                // マイナス数字（-1 -1.0）の場合は除外
                if (MdlUtil.isNumeric(key)) {
                    isMatch = false;
                }
            }

            if (isMatch) {
                if (i < args.length - 1 && args[i + 1] != null) {
                    value = args[i + 1];
                    // 最初の文字が「-」|「--」で始まっていて、数字でない場合は無視
                    Matcher matchForValue = KEY_REGEX.matcher(value);
                    if (matchForValue.matches() && !MdlUtil.isNumeric(value)) {
                        value = "";
                    }
                }

                if ("h".equals(key) && (value == null || value.isEmpty())) {
                    key = "help";
                }

                String unescapedValue = (value != null) ? ESCAPED_DASH_REGEX.matcher(value).replaceAll("-") : "";
                namedArgs.put(key, unescapedValue);
            }
        }

        return namedArgs;
    }

    /**
     * 指定されたキーが解析済み引数マップ内に存在するか判定します。
     *
     * @param namedArgs 名前付き引数のマップ
     * @param key 判定対象のキー文字列
     * @return キーが存在する場合は true、それ以外は false
     */
    public static boolean containsKey(Map<String, String> namedArgs, String key) {
        return key != null && !key.isEmpty() && namedArgs != null && namedArgs.containsKey(key);
    }

    /**
     * 指定されたキーに対応する引数値を取得します。
     *
     * @param namedArgs 名前付き引数のマップ
     * @param key 取得するキー文字列
     * @return キーに対応する値。キーが存在しないまたは無効な場合は空文字列 ("")
     */
    public static String getValue(Map<String, String> namedArgs, String key) {
        if (key != null && !key.isEmpty() && namedArgs != null && namedArgs.containsKey(key)) {
            String val = namedArgs.get(key);
            return val != null ? val : "";
        }
        return "";
    }

    /**
     * 指定されたキーに対応する値から絶対パスを取得します。
     *
     * @param namedArgs 名前付き引数のマップ
     * @param key 取得するキー文字列
     * @return 絶対パス文字列。パスが無効またはキーが存在しない場合は空文字列 ("")
     */
    public static String getFullPath(Map<String, String> namedArgs, String key) {
        String val = getValue(namedArgs, key);
        if (val != null && !val.isEmpty()) {
            try {
                return new File(val).getAbsolutePath();
            } catch (Exception e) {
                return "";
            }
        }
        return "";
    }
}
