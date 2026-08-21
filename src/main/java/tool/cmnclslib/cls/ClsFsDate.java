package tool.cmnclslib.cls;

import java.time.LocalDateTime;
import java.util.Objects;
import tool.cmnclslib.ifc.ICmnLogger;
import tool.cmnclslib.mdl.MdlConst;
import tool.cmnclslib.mdl.MdlFile;

/**
 * ファイルシステムの日付操作を行うクラスです。
 */
public class ClsFsDate {

    private final ICmnLogger logger;
    private int verbose = 0;
    private String message = "";
    private boolean isThrowIfException = false;

    /**
     * ロガーを指定して ClsFsDate クラスの新しいインスタンスを初期化します。
     *
     * @param logger ログ出力用ロガーのインスタンス
     */
    public ClsFsDate(ICmnLogger logger) {
        this.logger = Objects.requireNonNull(logger, "logger must not be null");
    }

    public int getVerbose() {
        return verbose;
    }

    public void setVerbose(int verbose) {
        this.verbose = verbose;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message != null ? message : "";
    }

    public boolean isThrowIfException() {
        return isThrowIfException;
    }

    public void setThrowIfException(boolean throwIfException) {
        isThrowIfException = throwIfException;
    }

    /**
     * ファイルまたはディレクトリの日付を設定します。
     *
     * @param path 対象のファイルまたはディレクトリのパス
     * @param dateString 設定する日付文字列
     * @param mode 処理モード
     * @param pathKind パスの種類
     * @param isValidateDate 日付フォーマットの検証を行うかどうか
     * @param isForce 強制的に設定を行うかどうか
     * @param isExec 実際に実行するかどうか
     * @return 設定が成功した場合は true、失敗した場合は false
     */
    public boolean setDate(String path, String dateString, int mode, int pathKind, boolean isValidateDate, boolean isForce, boolean isExec) {
        try {
            return setDateCore(path, dateString, mode, pathKind, isValidateDate, isForce, isExec) > -1;
        } catch (Exception e) {
            return false;
        }
    }

    public boolean setDate(String path, String dateString, int mode, int pathKind, boolean isValidateDate, boolean isForce) {
        return setDate(path, dateString, mode, pathKind, isValidateDate, isForce, true);
    }

    public boolean setDate(String path, String dateString, int mode, int pathKind, boolean isValidateDate) {
        return setDate(path, dateString, mode, pathKind, isValidateDate, true, true);
    }

    /**
     * ファイルまたはディレクトリの日付を設定するメイン処理を実行します。
     *
     * @param path 対象のファイルまたはディレクトリのパス
     * @param dateString 設定する日付文字列
     * @param mode 処理モード
     * @param pathKind パスの種類
     * @param isValidateDate 日付の検証を行うかどうか
     * @param isForce 強制的に設定するかどうか
     * @param isExec 実際に実行するかどうか
     * @return 処理結果ステータスコード
     */
    public int setDateCore(String path, String dateString, int mode, int pathKind, boolean isValidateDate, boolean isForce, boolean isExec) {
        message = "";
        try {
            return MdlFile.setDateMain(path, dateString, mode, pathKind, isValidateDate, isForce, isExec);
        } catch (Exception ex) {
            message = ex.getMessage();
            if (verbose > 0 && logger != null) {
                logger.writeLine(MdlConst.LVL_E, "[ClsFsDate.SetDate()] EXCEPTION : " + ex.getMessage());
            }
            if (isThrowIfException) {
                throw new RuntimeException(ex);
            }
            return -1;
        }
    }

    public int setDateCore(String path, String dateString, int mode, int pathKind, boolean isValidateDate, boolean isForce) {
        return setDateCore(path, dateString, mode, pathKind, isValidateDate, isForce, true);
    }

    public int setDateCore(String path, String dateString, int mode, int pathKind, boolean isValidateDate) {
        return setDateCore(path, dateString, mode, pathKind, isValidateDate, true, true);
    }

    /**
     * ディレクトリの日付を設定します。
     *
     * @param path 対象ディレクトリのパス
     * @param date 設定する日時
     * @param mode 処理モード
     * @param isForce 強制的に設定するかどうか
     * @param isExec 実際に実行するかどうか
     * @return 設定が成功した場合は true、失敗した場合は false
     */
    public boolean setDirectoryDate(String path, LocalDateTime date, int mode, boolean isForce, boolean isExec) {
        try {
            return setDirectoryDateCore(path, date, mode, isForce, isExec) > -1;
        } catch (Exception e) {
            return false;
        }
    }

    public boolean setDirectoryDate(String path, LocalDateTime date, int mode, boolean isForce) {
        return setDirectoryDate(path, date, mode, isForce, true);
    }

    public boolean setDirectoryDate(String path, LocalDateTime date, int mode) {
        return setDirectoryDate(path, date, mode, true);
    }

    /**
     * ディレクトリの日付を設定するメイン処理を実行します。
     *
     * @param path 対象ディレクトリのパス
     * @param date 設定する日時
     * @param mode 処理モード
     * @param isForce 強制的に設定するかどうか
     * @param isExec 実際に実行するかどうか
     * @return 処理結果ステータスコード
     */
    public int setDirectoryDateCore(String path, LocalDateTime date, int mode, boolean isForce, boolean isExec) {
        message = "";
        try {
            return MdlFile.setDateToDirMain(path, date, mode, isForce, isExec);
        } catch (Exception ex) {
            message = ex.getMessage();
            if (verbose > 0 && logger != null) {
                logger.writeLine(MdlConst.LVL_E, "[ClsFsDate.SetDirectoryDate()] EXCEPTION : " + ex.getMessage());
            }
            if (isThrowIfException) {
                throw new RuntimeException(ex);
            }
            return -1;
        }
    }

    public int setDirectoryDateCore(String path, LocalDateTime date, int mode, boolean isForce) {
        return setDirectoryDateCore(path, date, mode, isForce, true);
    }

    public int setDirectoryDateCore(String path, LocalDateTime date, int mode) {
        return setDirectoryDateCore(path, date, mode, true, true);
    }

    /**
     * ファイルの日付を設定します。
     *
     * @param path 対象ファイルのパス
     * @param date 設定する日時
     * @param mode 処理モード
     * @param isForce 強制的に設定するかどうか
     * @param isExec 実際に実行するかどうか
     * @return 設定が成功した場合は true、失敗した場合は false
     */
    public boolean setFileDate(String path, LocalDateTime date, int mode, boolean isForce, boolean isExec) {
        try {
            return setFileDateCore(path, date, mode, isForce, isExec) > -1;
        } catch (Exception e) {
            return false;
        }
    }

    public boolean setFileDate(String path, LocalDateTime date, int mode, boolean isForce) {
        return setFileDate(path, date, mode, isForce, true);
    }

    public boolean setFileDate(String path, LocalDateTime date, int mode) {
        return setFileDate(path, date, mode, true, true);
    }

    /**
     * ファイルの日付を設定するメイン処理を実行します。
     *
     * @param path 対象ファイルのパス
     * @param date 設定する日時
     * @param mode 処理モード
     * @param isForce 強制的に設定するかどうか
     * @param isExec 実際に実行するかどうか
     * @return 処理結果ステータスコード
     */
    public int setFileDateCore(String path, LocalDateTime date, int mode, boolean isForce, boolean isExec) {
        message = "";
        try {
            return MdlFile.setDateToFileMain(path, date, mode, isForce, isExec);
        } catch (Exception ex) {
            message = ex.getMessage();
            if (verbose > 0 && logger != null) {
                logger.writeLine(MdlConst.LVL_E, "[ClsFsDate.SetFileDate()] EXCEPTION : " + ex.getMessage());
            }
            if (isThrowIfException) {
                throw new RuntimeException(ex);
            }
            return -1;
        }
    }

    public int setFileDateCore(String path, LocalDateTime date, int mode, boolean isForce) {
        return setFileDateCore(path, date, mode, isForce, true);
    }

    public int setFileDateCore(String path, LocalDateTime date, int mode) {
        return setFileDateCore(path, date, mode, true, true);
    }
}
