package tool.cmnclslib.ifc;

/**
 * 共通ロガーの基本操作を提供するインターフェースです。
 */
public interface ICmnLogger {

    /**
     * 指定されたキーに対応するプロパティ値（文字列）を取得します。
     *
     * @param key 取得対象のプロパティキー
     * @param defaultValue キーが存在しない場合や無効な場合に返却されるデフォルト文字列
     * @return プロパティの値、またはデフォルト文字列
     */
    String getValueByKey(String key, String defaultValue);

    /**
     * 指定されたキーに対応するプロパティ値（真偽値）を取得します。
     *
     * @param key 取得対象のプロパティキー
     * @param defaultValue キーが存在しない場合や無効な場合に返却されるデフォルトの真偽値
     * @return プロパティの真偽値、またはデフォルト値
     */
    boolean getValueByKey(String key, boolean defaultValue);

    /**
     * 指定されたキーにプロパティ値を設定します。
     *
     * @param key 設定対象のプロパティキー
     * @param val 設定する値の文字列
     */
    void setValueByKey(String key, String val);

    /**
     * 指定されたログレベルでメッセージを書き込みます。
     *
     * @param level ログレベル
     * @param message 出力するログメッセージ
     */
    void writeLine(int level, String message);
}
