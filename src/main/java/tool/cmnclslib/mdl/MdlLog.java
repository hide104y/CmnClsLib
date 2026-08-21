package tool.cmnclslib.mdl;

import java.util.Locale;

/**
 * ログ出力処理に関連するユーティリティ機能を提供する静的クラスです。
 */
public final class MdlLog {

    private MdlLog() {
        // インスタンス化防止
    }

    /**
     * ログレベルの文字列（"none", "debug", "info", "warn", "error", "fatal"）を対応する整数値に変換します。
     *
     * @param logLevel 変換対象のログレベル文字列（大文字小文字不問、前後空白許容）
     * @return 対応するログレベルの整数値。無効な文字列または null/空文字の場合はデフォルト値 (MdlConst.LVL_I)
     */
    public static int parseLogLevel(String logLevel) {
        if (logLevel == null || logLevel.trim().isEmpty()) {
            return MdlConst.LVL_I;
        }

        String lower = logLevel.trim().toLowerCase(Locale.ROOT);
        switch (lower) {
            case "none":
                return MdlConst.LVL_NONE;
            case "debug":
                return MdlConst.LVL_DEBUG;
            case "info":
                return MdlConst.LVL_I;
            case "warn":
                return MdlConst.LVL_W;
            case "error":
                return MdlConst.LVL_E;
            case "fatal":
                return MdlConst.LVL_F;
            default:
                return MdlConst.LVL_I;
        }
    }

    /**
     * ログレベルの整数値を対応する文字列（"none", "debug", "info", "warn", "error", "fatal"）に変換します。
     *
     * @param logLevel ログレベルの整数値
     * @return 小文字表記のログレベル文字列。不明な定数の場合は "info"
     */
    public static String logLevelToString(int logLevel) {
        switch (logLevel) {
            case MdlConst.LVL_NONE:
                return "none";
            case MdlConst.LVL_DEBUG:
                return "debug";
            case MdlConst.LVL_I:
                return "info";
            case MdlConst.LVL_W:
                return "warn";
            case MdlConst.LVL_E:
                return "error";
            case MdlConst.LVL_F:
                return "fatal";
            default:
                return "info";
        }
    }

    /**
     * ファイルのベース名に当日の日付（yyyyMMdd）を付加してサニタイズされたログファイル名を生成します。
     *
     * @param baseName ログファイルのベース名
     * @return サニタイズ済みのログファイル名（例: "app_20260801.log"）
     */
    public static String generateLogFileName(String baseName) {
        String dateStr = MdlDate.getFormattedDate("yyyyMMdd");
        return MdlFile.sanitizeFileName(baseName + "_" + dateStr + ".log");
    }

    /**
     * ログレベルに応じた出力用のプレフィックス文字列（大文字＋末尾スペース）を取得します。
     *
     * @param logLevel ログレベルの整数値
     * @return ログプレフィックス文字列（例: "INFO ", "ERROR "）。"none" の場合は空文字列
     */
    public static String getLogLevelPrefix(int logLevel) {
        String prefix = logLevelToString(logLevel);
        return "none".equals(prefix) ? "" : prefix.toUpperCase(Locale.ROOT) + " ";
    }
}
