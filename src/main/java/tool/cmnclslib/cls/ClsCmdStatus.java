package tool.cmnclslib.cls;

import java.util.ArrayList;
import java.util.List;
import tool.cmnclslib.ifc.ICmnLogger;
import tool.cmnclslib.mdl.MdlConst;
import tool.cmnclslib.mdl.MdlUtil;

/**
 * コマンド実行状態の管理および終了コード・出力ログメッセージの判定を行うクラスです。
 */
public class ClsCmdStatus {

    private final ICmnLogger logger;

    private int verbose = 0;
    private int debugLevel = MdlConst.LVL_DEBUG;
    private String okReturnCodeCsv = "0";
    private String warnReturnCodeCsv = "";
    private String errorReturnCodeCsv = "";
    private String okMessageCsv = "";
    private String warnMessageCsv = "";
    private String errorMessageCsv = "";
    private int methodExitStatus = 0;
    private int returnLevel = MdlConst.LVL_I;
    private int errorCode = MdlConst.INT_NULL;
    private int warnCode = MdlConst.INT_NULL;
    private int warnThreshold = MdlConst.INT_NULL;
    private int errorThreshold = MdlConst.INT_NULL;
    private boolean isErrorAtNegative = false;
    private boolean isAlwaysNormal = false;

    private List<String> okMessageList = new ArrayList<>();
    private List<String> warnMessageList = new ArrayList<>();
    private List<String> errorMessageList = new ArrayList<>();
    private List<Integer> okReturnCodeList = new ArrayList<>();
    private List<Integer> warnReturnCodeList = new ArrayList<>();
    private List<Integer> errorReturnCodeList = new ArrayList<>();

    private boolean isOkMessageHit = false;
    private boolean isWarnMessageHit = false;
    private boolean isErrorMessageHit = false;

    /**
     * ロガーを指定して ClsCmdStatus クラスの新しいインスタンスを初期化します。
     *
     * @param logger ログ出力用のロガーインスタンス
     */
    public ClsCmdStatus(ICmnLogger logger) {
        this.logger = logger;
    }

    public int getVerbose() {
        return verbose;
    }

    public void setVerbose(int verbose) {
        this.verbose = verbose;
    }

    public int getDebugLevel() {
        return debugLevel;
    }

    public void setDebugLevel(int debugLevel) {
        this.debugLevel = debugLevel;
    }

    public String getOkReturnCodeCsv() {
        return okReturnCodeCsv;
    }

    public void setOkReturnCodeCsv(String okReturnCodeCsv) {
        this.okReturnCodeCsv = okReturnCodeCsv != null ? okReturnCodeCsv : "";
    }

    public String getWarnReturnCodeCsv() {
        return warnReturnCodeCsv;
    }

    public void setWarnReturnCodeCsv(String warnReturnCodeCsv) {
        this.warnReturnCodeCsv = warnReturnCodeCsv != null ? warnReturnCodeCsv : "";
    }

    public String getErrRetCodeCsv() {
        return errorReturnCodeCsv;
    }

    public void setErrRetCodeCsv(String errorReturnCodeCsv) {
        this.errorReturnCodeCsv = errorReturnCodeCsv != null ? errorReturnCodeCsv : "";
    }

    /**
     * @deprecated {@link #getErrRetCodeCsv()} を使用してください。
     */
    @Deprecated
    public String getErrorReturnCodeCsv() {
        return getErrRetCodeCsv();
    }

    /**
     * @deprecated {@link #setErrRetCodeCsv(String)} を使用してください。
     */
    @Deprecated
    public void setErrorReturnCodeCsv(String errorReturnCodeCsv) {
        setErrRetCodeCsv(errorReturnCodeCsv);
    }

    public String getOkMessageCsv() {
        return okMessageCsv;
    }

    public void setOkMessageCsv(String okMessageCsv) {
        this.okMessageCsv = okMessageCsv != null ? okMessageCsv : "";
    }

    public String getWarnMessageCsv() {
        return warnMessageCsv;
    }

    public void setWarnMessageCsv(String warnMessageCsv) {
        this.warnMessageCsv = warnMessageCsv != null ? warnMessageCsv : "";
    }

    public String getErrorMessageCsv() {
        return errorMessageCsv;
    }

    public void setErrorMessageCsv(String errorMessageCsv) {
        this.errorMessageCsv = errorMessageCsv != null ? errorMessageCsv : "";
    }

    public int getMethodExitStatus() {
        return methodExitStatus;
    }

    public void setMethodExitStatus(int methodExitStatus) {
        this.methodExitStatus = methodExitStatus;
    }

    public int getReturnLevel() {
        return returnLevel;
    }

    public void setReturnLevel(int returnLevel) {
        this.returnLevel = returnLevel;
    }

    public int getErrorCode() {
        return errorCode;
    }

    public void setErrorCode(int errorCode) {
        this.errorCode = errorCode;
    }

    public int getWarnCode() {
        return warnCode;
    }

    public void setWarnCode(int warnCode) {
        this.warnCode = warnCode;
    }

    public int getWarnThreshold() {
        return warnThreshold;
    }

    public void setWarnThreshold(int warnThreshold) {
        this.warnThreshold = warnThreshold;
    }

    public int getErrorThreshold() {
        return errorThreshold;
    }

    public void setErrorThreshold(int errorThreshold) {
        this.errorThreshold = errorThreshold;
    }

    public boolean isErrAtNegative() {
        return isErrorAtNegative;
    }

    public void setErrAtNegative(boolean errorAtNegative) {
        this.isErrorAtNegative = errorAtNegative;
    }

    /**
     * @deprecated {@link #isErrAtNegative()} を使用してください。
     */
    @Deprecated
    public boolean isErrorAtNegativeValue() {
        return isErrAtNegative();
    }

    /**
     * @deprecated {@link #setErrAtNegative(boolean)} を使用してください。
     */
    @Deprecated
    public void setErrorAtNegativeValue(boolean errorAtNegativeValue) {
        setErrAtNegative(errorAtNegativeValue);
    }

    public boolean isAlwaysNormal() {
        return isAlwaysNormal;
    }

    public void setAlwaysNormal(boolean alwaysNormal) {
        this.isAlwaysNormal = alwaysNormal;
    }

    public List<String> getOkMessageList() {
        return okMessageList;
    }

    public void setOkMessageList(List<String> okMessageList) {
        this.okMessageList = okMessageList != null ? okMessageList : new ArrayList<>();
    }

    public List<String> getWarnMessageList() {
        return warnMessageList;
    }

    public void setWarnMessageList(List<String> warnMessageList) {
        this.warnMessageList = warnMessageList != null ? warnMessageList : new ArrayList<>();
    }

    public List<String> getErrorMessageList() {
        return errorMessageList;
    }

    public void setErrorMessageList(List<String> errorMessageList) {
        this.errorMessageList = errorMessageList != null ? errorMessageList : new ArrayList<>();
    }

    public List<Integer> getOkReturnCodeList() {
        return okReturnCodeList;
    }

    public void setOkReturnCodeList(List<Integer> okReturnCodeList) {
        this.okReturnCodeList = okReturnCodeList != null ? okReturnCodeList : new ArrayList<>();
    }

    public List<Integer> getWarnRetCodeList() {
        return warnReturnCodeList;
    }

    public void setWarnRetCodeList(List<Integer> warnReturnCodeList) {
        this.warnReturnCodeList = warnReturnCodeList != null ? warnReturnCodeList : new ArrayList<>();
    }

    /**
     * @deprecated {@link #getWarnRetCodeList()} を使用してください。
     */
    @Deprecated
    public List<Integer> getWarnReturnCodeList() {
        return getWarnRetCodeList();
    }

    /**
     * @deprecated {@link #setWarnRetCodeList(List)} を使用してください。
     */
    @Deprecated
    public void setWarnReturnCodeList(List<Integer> warnReturnCodeList) {
        setWarnRetCodeList(warnReturnCodeList);
    }

    public List<Integer> getErrRetCodeList() {
        return errorReturnCodeList;
    }

    public void setErrRetCodeList(List<Integer> errorReturnCodeList) {
        this.errorReturnCodeList = errorReturnCodeList != null ? errorReturnCodeList : new ArrayList<>();
    }

    /**
     * @deprecated {@link #getErrRetCodeList()} を使用してください。
     */
    @Deprecated
    public List<Integer> getErrorReturnCodeList() {
        return getErrRetCodeList();
    }

    /**
     * @deprecated {@link #setErrRetCodeList(List)} を使用してください。
     */
    @Deprecated
    public void setErrorReturnCodeList(List<Integer> errorReturnCodeList) {
        setErrRetCodeList(errorReturnCodeList);
    }

    public boolean isOkMessageHit() {
        return isOkMessageHit;
    }

    public void setOkMessageHit(boolean okMessageHit) {
        isOkMessageHit = okMessageHit;
    }

    public boolean isWarnMessageHit() {
        return isWarnMessageHit;
    }

    public void setWarnMessageHit(boolean warnMessageHit) {
        isWarnMessageHit = warnMessageHit;
    }

    public boolean isErrorMessageHit() {
        return isErrorMessageHit;
    }

    public void setErrorMessageHit(boolean errorMessageHit) {
        isErrorMessageHit = errorMessageHit;
    }

    /**
     * 設定されたCSV文字列を解析し、各判定用コードリストおよびメッセージリストを生成して初期化します。
     */
    public void initialize() {
        okReturnCodeList.clear();
        warnReturnCodeList.clear();
        errorReturnCodeList.clear();
        okMessageList.clear();
        warnMessageList.clear();
        errorMessageList.clear();

        if (okReturnCodeCsv != null && !okReturnCodeCsv.isEmpty()) {
            okReturnCodeList = MdlUtil.parseCsvToInts(null, okReturnCodeCsv);
        }
        if (warnReturnCodeCsv != null && !warnReturnCodeCsv.isEmpty()) {
            warnReturnCodeList = MdlUtil.parseCsvToInts(null, warnReturnCodeCsv);
        }
        if (errorReturnCodeCsv != null && !errorReturnCodeCsv.isEmpty()) {
            errorReturnCodeList = MdlUtil.parseCsvToInts(null, errorReturnCodeCsv);
        }
        if (okMessageCsv != null && !okMessageCsv.isEmpty()) {
            okMessageList = MdlUtil.parseCsvToList(null, okMessageCsv);
        }
        if (warnMessageCsv != null && !warnMessageCsv.isEmpty()) {
            warnMessageList = MdlUtil.parseCsvToList(null, warnMessageCsv);
        }
        if (errorMessageCsv != null && !errorMessageCsv.isEmpty()) {
            errorMessageList = MdlUtil.parseCsvToList(null, errorMessageCsv);
        }
    }

    /**
     * 出力文字列のチェック判定が必要であるかを確認します。
     *
     * @return メッセージの判定が必要な場合は true、それ以外は false
     */
    public boolean shouldCheckMessage() {
        return !okMessageList.isEmpty() || !warnMessageList.isEmpty() || !errorMessageList.isEmpty();
    }

    /**
     * メッセージのマッチ判定フラグを初期化します。
     */
    public void resetFlags() {
        isOkMessageHit = false;
        isWarnMessageHit = false;
        isErrorMessageHit = false;
    }

    /**
     * コマンドからの出力ログ文字列（1行）を検査し、設定された正常・警告・異常メッセージとマッチするか判定します。
     *
     * @param line チェック対象のログ出力文字列
     */
    public void checkMessageLine(String line) {
        if (line == null || line.isEmpty()) {
            return;
        }

        if (!isOkMessageHit) {
            for (String pattern : okMessageList) {
                if (line.contains(pattern)) {
                    if (verbose > 4 && logger != null) {
                        logger.writeLine(debugLevel, "[HIT] OkStr : [" + pattern + "] in [" + line + "]");
                    }
                    isOkMessageHit = true;
                    break;
                } else if (verbose > 6 && logger != null) {
                    logger.writeLine(debugLevel, "[NOHIT] OkStr : [" + pattern + "] in [" + line + "]");
                }
            }
        }

        if (!isWarnMessageHit) {
            for (String pattern : warnMessageList) {
                if (line.contains(pattern)) {
                    if (verbose > 4 && logger != null) {
                        logger.writeLine(debugLevel, "[HIT] WarnStr : [" + pattern + "] in [" + line + "]");
                    }
                    isWarnMessageHit = true;
                    break;
                } else if (verbose > 6 && logger != null) {
                    logger.writeLine(debugLevel, "[NOHIT] WarnStr : [" + pattern + "] in [" + line + "]");
                }
            }
        }

        if (!isErrorMessageHit) {
            for (String pattern : errorMessageList) {
                if (line.contains(pattern)) {
                    if (verbose > 4 && logger != null) {
                        logger.writeLine(debugLevel, "[HIT] NgStr : [" + pattern + "] in [" + line + "]");
                    }
                    isErrorMessageHit = true;
                    break;
                } else if (verbose > 6 && logger != null) {
                    logger.writeLine(debugLevel, "[NOHIT] NgStr : [" + pattern + "] in [" + line + "]");
                }
            }
        }
    }

    /**
     * コマンドの終了コードを検証し、ステータスおよびエラーレベルを評価・設定します。
     *
     * @param exitCode コマンドの実行完了コード
     */
    public void checkCommandExitCode(int exitCode) {
        if (warnThreshold == MdlConst.INT_NULL && errorThreshold == MdlConst.INT_NULL && !isAlwaysNormal) {
            if (exitCode == 0) {
                methodExitStatus = MdlConst.LVL_I;
                returnLevel = MdlConst.LVL_I;
            } else {
                methodExitStatus = (errorCode == MdlConst.INT_NULL ? exitCode : errorCode);
                returnLevel = MdlConst.LVL_E;
            }
        } else {
            methodExitStatus = MdlConst.LVL_I;
            returnLevel = MdlConst.LVL_I;

            if (isAlwaysNormal) {
                warnThreshold = MdlConst.INT_NULL;
                errorThreshold = MdlConst.INT_NULL;
            }
            if (isErrorAtNegative && exitCode < 0) {
                methodExitStatus = MdlConst.LVL_E;
                returnLevel = MdlConst.LVL_E;
            }
        }

        if (warnThreshold != MdlConst.INT_NULL && exitCode > warnThreshold) {
            methodExitStatus = MdlConst.LVL_W;
            returnLevel = MdlConst.LVL_W;
        }

        if (errorThreshold != MdlConst.INT_NULL && exitCode > errorThreshold) {
            methodExitStatus = MdlConst.LVL_E;
            returnLevel = MdlConst.LVL_E;
        }

        for (int check : okReturnCodeList) {
            if (check == exitCode) {
                if (verbose > 4 && logger != null) {
                    logger.writeLine(debugLevel, "[HIT] OkRetCd : " + check);
                }
                methodExitStatus = MdlConst.LVL_I;
                returnLevel = MdlConst.LVL_I;
                break;
            } else if (verbose > 6 && logger != null) {
                logger.writeLine(debugLevel, "[NOHIT] OkRetCd : " + check);
            }
        }

        for (int check : warnReturnCodeList) {
            if (check == exitCode) {
                if (verbose > 4 && logger != null) {
                    logger.writeLine(debugLevel, "[HIT] WarnRetCd : " + check);
                }
                methodExitStatus = (warnCode == MdlConst.INT_NULL ? exitCode : warnCode);
                returnLevel = MdlConst.LVL_W;
                break;
            } else if (verbose > 6 && logger != null) {
                logger.writeLine(debugLevel, "[NOHIT] WarnRetCd : " + check);
            }
        }

        for (int check : errorReturnCodeList) {
            if (check == exitCode) {
                if (verbose > 4 && logger != null) {
                    logger.writeLine(debugLevel, "[HIT] NgRetCd : " + check);
                }
                methodExitStatus = (errorCode == MdlConst.INT_NULL ? exitCode : errorCode);
                returnLevel = MdlConst.LVL_E;
                break;
            } else if (verbose > 6 && logger != null) {
                logger.writeLine(debugLevel, "[NOHIT] NgRetCd : " + check);
            }
        }
    }

    /**
     * 出力ログメッセージの判定ヒット状況に基づき、最終的な終了ステータスおよびエラーレベルを評価・決定します。
     */
    public void evaluate() {
        if (!okMessageList.isEmpty()) {
            if (isOkMessageHit) {
                methodExitStatus = MdlConst.LVL_I;
                returnLevel = MdlConst.LVL_I;
            } else {
                if (errorCode != MdlConst.INT_NULL) {
                    methodExitStatus = errorCode;
                }
                if (methodExitStatus == MdlConst.LVL_I) {
                    methodExitStatus = MdlConst.LVL_E;
                }
                returnLevel = MdlConst.LVL_E;
            }
        }
        if (!warnMessageList.isEmpty() && isWarnMessageHit) {
            if (warnCode != MdlConst.INT_NULL) {
                methodExitStatus = warnCode;
            }
            if (methodExitStatus == MdlConst.LVL_I) {
                methodExitStatus = MdlConst.LVL_W;
            }
            returnLevel = MdlConst.LVL_W;
        }
        if (!errorMessageList.isEmpty() && isErrorMessageHit) {
            if (errorCode != MdlConst.INT_NULL) {
                methodExitStatus = errorCode;
            }
            if (methodExitStatus == MdlConst.LVL_I) {
                methodExitStatus = MdlConst.LVL_E;
            }
            returnLevel = MdlConst.LVL_E;
        }
    }
}
