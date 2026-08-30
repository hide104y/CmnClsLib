package tool.cmnclslib.cls;

import java.io.BufferedReader;
import java.io.File;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.nio.charset.Charset;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;
import tool.cmnclslib.ifc.ICmnLogger;
import tool.cmnclslib.mdl.MdlApp;
import tool.cmnclslib.mdl.MdlConst;
import tool.cmnclslib.mdl.MdlFile;
import tool.cmnclslib.mdl.MdlUtil;

/**
 * 外部コマンドやプロセスの実行およびログ管理、終了ステータス判定機能を提供するクラスです。
 */
public class ClsCmdExec {

    private static final StringBuilder STRING_BUILDER = new StringBuilder();
    private static final Object LOCK_STRING_BUILDER = new Object();

    private final ICmnLogger logger;
    private final ClsCmdStatus cmdStatus;

    private Thread thread = null;
    private Process process = null;
    private Map<String, String> processEnvs = new LinkedHashMap<>();

    private boolean isRunning = false;
    private boolean isShowCmd = false;
    private boolean isShowEmptyLine = true;
    private boolean isShowOutput = false;
    private boolean isNotShowOutput = false;
    private boolean isShowExitCode = false;
    private boolean isNotShowExitCode = false;
    private boolean isInfoPrefix = true;
    private boolean isStdoutPrefix = false;
    private boolean isErrorDialog = false;
    private boolean isUseShellExecute = false;
    private boolean isCreateNoWindow = true;
    private boolean isLoadUserProfile = false;
    private boolean isRunAs = false;
    private boolean isNoRedirect = false;
    private boolean isSu = false;
    private boolean isStackTrace = false;
    private boolean isSilent = false;
    private boolean isShowEnvMap = false;
    private boolean isClearOutput = true;

    private String errorMessage = "";
    private String cmdPath = "";
    private String workDir = "";
    private String cmdArgs = "";
    private String prefix = "0";
    private String encoding = "";
    private String domainName = "";
    private String username = "";
    private String password = "";
    private String processEnvCsv = "";
    private String stdIn = "";

    private int defaultErrLogLevel = MdlConst.LVL_E;
    private int verbose = 0;
    private int debugLevel = MdlConst.LVL_NONE;
    private int stdoutLevel = MdlConst.LVL_NONE;
    private int timeout = 86400;
    private int cmdExitStatus = 0;

    /**
     * ロガーを指定して ClsCmdExec クラスの新しいインスタンスを初期化します。
     *
     * @param logger ログ出力を行うロガーインスタンス
     */
    public ClsCmdExec(ICmnLogger logger) {
        this.logger = logger;
        this.cmdStatus = new ClsCmdStatus(logger);
    }

    /**
     * プロセス実行時に渡す環境変数のマップを取得します。
     *
     * @return 環境変数マップ
     */
    public Map<String, String> getProcessEnvs() {
        return processEnvs;
    }

    /**
     * プロセス実行時に渡す環境変数のマップを設定します。
     *
     * @param processEnvs 環境変数マップ
     */
    public void setProcessEnvs(Map<String, String> processEnvs) {
        this.processEnvs = processEnvs != null ? processEnvs : new LinkedHashMap<>();
    }

    /**
     * コマンド実行時の標準出力・標準エラー出力を蓄積する StringBuilder を取得します。
     *
     * @return 出力蓄積用 StringBuilder
     */
    public StringBuilder getStringBuilder() {
        return STRING_BUILDER;
    }

    /**
     * プロセス実行時の作業ディレクトリを取得します。
     *
     * @return 作業ディレクトリパス
     */
    public String getWorkDir() {
        return workDir;
    }

    /**
     * プロセス実行時の作業ディレクトリを設定します。
     *
     * @param workDir 作業ディレクトリパス
     */
    public void setWorkDir(String workDir) {
        this.workDir = workDir != null ? workDir : "";
    }

    /**
     * 実行対象のコマンドパスを取得します。
     *
     * @return コマンドパス
     */
    public String getCmdPath() {
        return cmdPath;
    }

    /**
     * 実行対象のコマンドパスを設定します。
     *
     * @param cmdPath コマンドパス
     */
    public void setCmdPath(String cmdPath) {
        this.cmdPath = cmdPath != null ? cmdPath : "";
    }

    /**
     * コマンドライン引数文字列を取得します。
     *
     * @return コマンドライン引数文字列
     */
    public String getCmdArgs() {
        return cmdArgs;
    }

    /**
     * コマンドライン引数文字列を設定します。
     *
     * @param cmdArgs コマンドライン引数文字列
     */
    public void setCmdArgs(String cmdArgs) {
        this.cmdArgs = cmdArgs != null ? cmdArgs : "";
    }

    /**
     * 直近のエラーメッセージを取得します。
     *
     * @return エラーメッセージ文字列
     */
    public String getErrorMessage() {
        return errorMessage;
    }

    /**
     * 直近のエラーメッセージを設定します。
     *
     * @param errorMessage エラーメッセージ文字列
     */
    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage != null ? errorMessage : "";
    }

    /**
     * ログ出力時のプレフィックス文字列を取得します。
     *
     * @return プレフィックス文字列
     */
    public String getPrefix() {
        return prefix;
    }

    /**
     * ログ出力時のプレフィックス文字列を設定します。
     *
     * @param prefix プレフィックス文字列
     */
    public void setPrefix(String prefix) {
        this.prefix = prefix != null ? prefix : "0";
    }

    /**
     * 標準入出力のエンコーディングを取得します。
     *
     * @return エンコーディング名
     */
    public String getEncoding() {
        return encoding;
    }

    /**
     * 標準入出力のエンコーディングを設定します。
     *
     * @param encoding エンコーディング名
     */
    public void setEncoding(String encoding) {
        this.encoding = encoding != null ? encoding : "";
    }

    /**
     * プロセス実行ユーザーのドメイン名を取得します。
     *
     * @return ドメイン名
     */
    public String getDomainName() {
        return domainName;
    }

    /**
     * プロセス実行ユーザーのドメイン名を設定します。
     *
     * @param domainName ドメイン名
     */
    public void setDomainName(String domainName) {
        this.domainName = domainName != null ? domainName : "";
    }

    /**
     * プロセス実行ユーザー名を取得します。
     *
     * @return ユーザー名
     */
    public String getUsername() {
        return username;
    }

    /**
     * プロセス実行ユーザー名を設定します。
     *
     * @param username ユーザー名
     */
    public void setUsername(String username) {
        this.username = username != null ? username : "";
    }

    /**
     * プロセス実行ユーザーのパスワードを取得します。
     *
     * @return パスワード
     */
    public String getPassword() {
        return password;
    }

    /**
     * プロセス実行ユーザーのパスワードを設定します。
     *
     * @param password パスワード
     */
    public void setPassword(String password) {
        this.password = password != null ? password : "";
    }

    /**
     * CSV形式のプロセス環境変数定義文字列を取得します。
     *
     * @return 環境変数CSV文字列
     */
    public String getProcessEnvCsv() {
        return processEnvCsv;
    }

    /**
     * CSV形式のプロセス環境変数定義文字列を設定します。
     *
     * @param processEnvCsv 環境変数CSV文字列
     */
    public void setProcessEnvCsv(String processEnvCsv) {
        this.processEnvCsv = processEnvCsv != null ? processEnvCsv : "";
    }

    /**
     * 標準入力に流し込む文字列を取得します。
     *
     * @return 標準入力文字列
     */
    public String getStdIn() {
        return stdIn;
    }

    /**
     * 標準入力に流し込む文字列を設定します。
     *
     * @param stdIn 標準入力文字列
     */
    public void setStdIn(String stdIn) {
        this.stdIn = stdIn != null ? stdIn : "";
    }

    /**
     * コマンドが実行中かどうかを取得します。
     *
     * @return 実行中の場合は true、それ以外は false
     */
    public boolean isRunning() {
        return isRunning;
    }

    /**
     * コマンドが実行中かどうかのフラグを設定します。
     *
     * @param running 実行中フラグ
     */
    public void setRunning(boolean running) {
        isRunning = running;
    }

    /**
     * 実行コマンドを表示するかどうかを取得します。
     *
     * @return 表示する場合は true、それ以外は false
     */
    public boolean isShowCmd() {
        return isShowCmd;
    }

    /**
     * 実行コマンドを表示するかどうかのフラグを設定します。
     *
     * @param showCmd コマンド表示フラグ
     */
    public void setShowCmd(boolean showCmd) {
        isShowCmd = showCmd;
    }

    /**
     * コマンド出力を表示するかどうかを取得します。
     *
     * @return 出力を表示する場合は true、それ以外は false
     */
    public boolean isShowOutput() {
        return isShowOutput;
    }

    /**
     * コマンド出力を表示するかどうかのフラグを設定します。
     *
     * @param showOutput 出力表示フラグ
     */
    public void setShowOutput(boolean showOutput) {
        isShowOutput = showOutput;
    }

    /**
     * コマンド出力を非表示にするかどうかを取得します。
     *
     * @return 非表示にする場合は true、それ以外は false
     */
    public boolean isNotShowOutput() {
        return isNotShowOutput;
    }

    /**
     * コマンド出力を非表示にするかどうかのフラグを設定します。
     *
     * @param notShowOutput 非表示フラグ
     */
    public void setNotShowOutput(boolean notShowOutput) {
        isNotShowOutput = notShowOutput;
    }

    /**
     * 終了コードを表示するかどうかを取得します。
     *
     * @return 終了コードを表示する場合は true、それ以外は false
     */
    public boolean isShowExitCode() {
        return isShowExitCode;
    }

    /**
     * 終了コードを表示するかどうかのフラグを設定します。
     *
     * @param showExitCode 終了コード表示フラグ
     */
    public void setShowExitCode(boolean showExitCode) {
        isShowExitCode = showExitCode;
    }

    /**
     * 終了コードを非表示にするかどうかを取得します。
     *
     * @return 終了コードを非表示にする場合は true、それ以外は false
     */
    public boolean isNotShowExitCode() {
        return isNotShowExitCode;
    }

    /**
     * 終了コードを非表示にするかどうかのフラグを設定します。
     *
     * @param notShowExitCode 終了コード非表示フラグ
     */
    public void setNotShowExitCode(boolean notShowExitCode) {
        isNotShowExitCode = notShowExitCode;
    }

    /**
     * 情報ログにプレフィックスを付与するかどうかを取得します。
     *
     * @return プレフィックスを付与する場合は true、それ以外は false
     */
    public boolean isInfoPrefix() {
        return isInfoPrefix;
    }

    /**
     * 情報ログにプレフィックスを付与するかどうかのフラグを設定します。
     *
     * @param infoPrefix プレフィックス付与フラグ
     */
    public void setInfoPrefix(boolean infoPrefix) {
        isInfoPrefix = infoPrefix;
    }

    /**
     * 標準出力にプレフィックスを付与するかどうかを取得します。
     *
     * @return プレフィックスを付与する場合は true、それ以外は false
     */
    public boolean isStdoutPrefix() {
        return isStdoutPrefix;
    }

    /**
     * 標準出力にプレフィックスを付与するかどうかのフラグを設定します。
     *
     * @param stdoutPrefix プレフィックス付与フラグ
     */
    public void setStdoutPrefix(boolean stdoutPrefix) {
        isStdoutPrefix = stdoutPrefix;
    }

    /**
     * エラーダイアログを表示するかどうかを取得します。
     *
     * @return エラーダイアログを表示する場合は true、それ以外は false
     */
    public boolean isErrorDialog() {
        return isErrorDialog;
    }

    /**
     * エラーダイアログを表示するかどうかを設定します。
     *
     * @param errorDialog エラーダイアログ表示フラグ
     */
    public void setErrorDialog(boolean errorDialog) {
        isErrorDialog = errorDialog;
    }

    /**
     * 別ユーザー権限（su / ユーザー切り替え）で実行するかどうかを取得します。
     *
     * @return su 実行フラグ
     */
    public boolean isSu() {
        return isSu;
    }

    /**
     * 別ユーザー権限（su / ユーザー切り替え）で実行するかどうかを設定します。
     *
     * @param su su 実行フラグ
     */
    public void setSu(boolean su) {
        isSu = su;
    }

    /**
     * シェル実行を使用するかどうかを取得します。
     *
     * @return シェル実行フラグ
     */
    public boolean isUseShellExecute() {
        return isUseShellExecute;
    }

    /**
     * シェル実行を使用するかどうかを設定します。
     *
     * @param useShellExecute シェル実行フラグ
     */
    public void setUseShellExecute(boolean useShellExecute) {
        isUseShellExecute = useShellExecute;
    }

    /**
     * ウィンドウを作成せずに実行するかどうかを取得します。
     *
     * @return ウィンドウ非作成フラグ
     */
    public boolean isCreateNoWindow() {
        return isCreateNoWindow;
    }

    /**
     * ウィンドウを作成せずに実行するかどうかを設定します。
     *
     * @param createNoWindow ウィンドウ非作成フラグ
     */
    public void setCreateNoWindow(boolean createNoWindow) {
        isCreateNoWindow = createNoWindow;
    }

    /**
     * ユーザープロファイルを読み込むかどうかを取得します。
     *
     * @return ユーザープロファイル読み込みフラグ
     */
    public boolean isLoadUserProfile() {
        return isLoadUserProfile;
    }

    /**
     * ユーザープロファイルを読み込むかどうかを設定します。
     *
     * @param loadUserProfile ユーザープロファイル読み込みフラグ
     */
    public void setLoadUserProfile(boolean loadUserProfile) {
        isLoadUserProfile = loadUserProfile;
    }

    /**
     * 昇格権限（管理者として実行 / RunAs）で実行するかどうかを取得します。
     *
     * @return RunAs 実行フラグ
     */
    public boolean isRunAs() {
        return isRunAs;
    }

    /**
     * 昇格権限（管理者として実行 / RunAs）で実行するかどうかを設定します。
     *
     * @param runAs RunAs 実行フラグ
     */
    public void setRunAs(boolean runAs) {
        isRunAs = runAs;
    }

    /**
     * リダイレクトを行わないかどうかを取得します。
     *
     * @return リダイレクト無効フラグ
     */
    public boolean isNoRedirect() {
        return isNoRedirect;
    }

    /**
     * リダイレクトを行わないかどうかを設定します。
     *
     * @param noRedirect リダイレクト無効フラグ
     */
    public void setNoRedirect(boolean noRedirect) {
        isNoRedirect = noRedirect;
    }

    /**
     * スタックトレースを出力するかどうかを取得します。
     *
     * @return スタックトレース出力フラグ
     */
    public boolean isStackTrace() {
        return isStackTrace;
    }

    /**
     * スタックトレースを出力するかどうかを設定します。
     *
     * @param stackTrace スタックトレース出力フラグ
     */
    public void setStackTrace(boolean stackTrace) {
        isStackTrace = stackTrace;
    }

    /**
     * 空行を表示するかどうかを取得します。
     *
     * @return 空行表示フラグ
     */
    public boolean isShowEmptyLine() {
        return isShowEmptyLine;
    }

    /**
     * 空行を表示するかどうかを設定します。
     *
     * @param showEmptyLine 空行表示フラグ
     */
    public void setShowEmptyLine(boolean showEmptyLine) {
        isShowEmptyLine = showEmptyLine;
    }

    /**
     * サイレントモード（一切の出力を抑制）かどうかを取得します。
     *
     * @return サイレントフラグ
     */
    public boolean isSilent() {
        return isSilent;
    }

    /**
     * サイレントモード（一切の出力を抑制）かどうかを設定します。
     *
     * @param silent サイレントフラグ
     */
    public void setSilent(boolean silent) {
        isSilent = silent;
    }

    /**
     * 環境変数マップを表示するかどうかを取得します。
     *
     * @return 環境変数マップ表示フラグ
     */
    public boolean isShowEnvMap() {
        return isShowEnvMap;
    }

    /**
     * 環境変数マップを表示するかどうかを設定します。
     *
     * @param showEnvMap 環境変数マップ表示フラグ
     */
    public void setShowEnvMap(boolean showEnvMap) {
        isShowEnvMap = showEnvMap;
    }

    /**
     * @deprecated {@link #isShowEnvMap()} を使用してください。
     */
    @Deprecated
    public boolean isShowEnvDic() {
        return isShowEnvMap();
    }

    /**
     * @deprecated {@link #setShowEnvMap(boolean)} を使用してください。
     */
    @Deprecated
    public void setShowEnvDic(boolean showEnvDic) {
        setShowEnvMap(showEnvDic);
    }

    /**
     * 実行前に出力バッファをクリアするかどうかを取得します。
     *
     * @return 出力クリアフラグ
     */
    public boolean isClearOutput() {
        return isClearOutput;
    }

    /**
     * 実行前に出力バッファをクリアするかどうかを設定します。
     *
     * @param clearOutput 出力クリアフラグ
     */
    public void setClearOutput(boolean clearOutput) {
        this.isClearOutput = clearOutput;
    }

    /**
     * @deprecated {@link #isClearOutput()} を使用してください。
     */
    @Deprecated
    public boolean isClearStringBuilder() {
        return isClearOutput();
    }

    /**
     * @deprecated {@link #setClearOutput(boolean)} を使用してください。
     */
    @Deprecated
    public void setClearStringBuilder(boolean clearStringBuilder) {
        setClearOutput(clearStringBuilder);
    }

    /**
     * デフォルトエラーログレベルを取得します。
     *
     * @return デフォルトエラーログレベル
     */
    public int getDefaultErrLevel() {
        return defaultErrLogLevel;
    }

    /**
     * デフォルトエラーログレベルを設定します。
     *
     * @param defaultErrorLogLevel デフォルトエラーログレベル
     */
    public void setDefaultErrLevel(int defaultErrorLogLevel) {
        this.defaultErrLogLevel = defaultErrorLogLevel;
    }

    /**
     * @deprecated {@link #getDefaultErrLevel()} を使用してください。
     */
    @Deprecated
    public int getDefaultErrorLogLevel() {
        return getDefaultErrLevel();
    }

    /**
     * @deprecated {@link #setDefaultErrLevel(int)} を使用してください。
     */
    @Deprecated
    public void setDefaultErrorLogLevel(int defaultErrorLogLevel) {
        setDefaultErrLevel(defaultErrorLogLevel);
    }

    /**
     * ログ詳細レベル (verbose) を取得します。
     *
     * @return 詳細レベル
     */
    public int getVerbose() {
        return verbose;
    }

    /**
     * ログ詳細レベル (verbose) を設定します。
     *
     * @param verbose 詳細レベル
     */
    public void setVerbose(int verbose) {
        this.verbose = verbose;
    }

    /**
     * デバッグレベルを取得します。
     *
     * @return デバッグレベル
     */
    public int getDebugLevel() {
        return debugLevel;
    }

    /**
     * デバッグレベルを設定します。
     *
     * @param debugLevel デバッグレベル
     */
    public void setDebugLevel(int debugLevel) {
        this.debugLevel = debugLevel;
    }

    /**
     * 標準出力のログレベルを取得します。
     *
     * @return 標準出力ログレベル
     */
    public int getStdoutLevel() {
        return stdoutLevel;
    }

    /**
     * 標準出力のログレベルを設定します。
     *
     * @param stdoutLevel 標準出力ログレベル
     */
    public void setStdoutLevel(int stdoutLevel) {
        this.stdoutLevel = stdoutLevel;
    }

    /**
     * コマンド実行タイムアウト（秒）を取得します。
     *
     * @return タイムアウト秒数
     */
    public int getTimeout() {
        return timeout;
    }

    /**
     * コマンド実行タイムアウト（秒）を設定します。
     *
     * @param timeout タイムアウト秒数
     */
    public void setTimeout(int timeout) {
        this.timeout = timeout;
    }

    /**
     * 正常終了コードのCSV文字列を取得します。
     *
     * @return 正常終了コードCSV文字列
     */
    public String getOkReturnCodeCsv() {
        return cmdStatus.getOkReturnCodeCsv();
    }

    /**
     * 正常終了コードのCSV文字列を設定します。
     *
     * @param okReturnCodeCsv 正常終了コードCSV文字列
     */
    public void setOkReturnCodeCsv(String okReturnCodeCsv) {
        cmdStatus.setOkReturnCodeCsv(okReturnCodeCsv);
    }

    /**
     * 警告終了コードのCSV文字列を取得します。
     *
     * @return 警告終了コードCSV文字列
     */
    public String getWarnReturnCodeCsv() {
        return cmdStatus.getWarnReturnCodeCsv();
    }

    /**
     * 警告終了コードのCSV文字列を設定します。
     *
     * @param warnReturnCodeCsv 警告終了コードCSV文字列
     */
    public void setWarnReturnCodeCsv(String warnReturnCodeCsv) {
        cmdStatus.setWarnReturnCodeCsv(warnReturnCodeCsv);
    }

    /**
     * エラー終了コードのCSV文字列を取得します。
     *
     * @return エラー終了コードCSV文字列
     */
    public String getErrRetCodeCsv() {
        return cmdStatus.getErrRetCodeCsv();
    }

    /**
     * エラー終了コードのCSV文字列を設定します。
     *
     * @param errorReturnCodeCsv エラー終了コードCSV文字列
     */
    public void setErrRetCodeCsv(String errorReturnCodeCsv) {
        cmdStatus.setErrRetCodeCsv(errorReturnCodeCsv);
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

    /**
     * 正常と判定するメッセージパターンのCSV文字列を取得します。
     *
     * @return 正常メッセージCSV文字列
     */
    public String getOkMessageCsv() {
        return cmdStatus.getOkMessageCsv();
    }

    /**
     * 正常と判定するメッセージパターンのCSV文字列を設定します。
     *
     * @param okMessageCsv 正常メッセージCSV文字列
     */
    public void setOkMessageCsv(String okMessageCsv) {
        cmdStatus.setOkMessageCsv(okMessageCsv);
    }

    /**
     * 警告と判定するメッセージパターンのCSV文字列を取得します。
     *
     * @return 警告メッセージCSV文字列
     */
    public String getWarnMessageCsv() {
        return cmdStatus.getWarnMessageCsv();
    }

    /**
     * 警告と判定するメッセージパターンのCSV文字列を設定します。
     *
     * @param warnMessageCsv 警告メッセージCSV文字列
     */
    public void setWarnMessageCsv(String warnMessageCsv) {
        cmdStatus.setWarnMessageCsv(warnMessageCsv);
    }

    /**
     * エラーと判定するメッセージパターンのCSV文字列を取得します。
     *
     * @return エラーメッセージCSV文字列
     */
    public String getErrorMessageCsv() {
        return cmdStatus.getErrorMessageCsv();
    }

    /**
     * エラーと判定するメッセージパターンのCSV文字列を設定します。
     *
     * @param errorMessageCsv エラーメッセージCSV文字列
     */
    public void setErrorMessageCsv(String errorMessageCsv) {
        cmdStatus.setErrorMessageCsv(errorMessageCsv);
    }

    /**
     * 警告判定しきい値終了コードを取得します。
     *
     * @return 警告しきい値
     */
    public int getWarnThreshold() {
        return cmdStatus.getWarnThreshold();
    }

    /**
     * 警告判定しきい値終了コードを設定します。
     *
     * @param warnThreshold 警告しきい値
     */
    public void setWarnThreshold(int warnThreshold) {
        cmdStatus.setWarnThreshold(warnThreshold);
    }

    /**
     * エラー判定しきい値終了コードを取得します。
     *
     * @return エラーしきい値
     */
    public int getErrorThreshold() {
        return cmdStatus.getErrorThreshold();
    }

    /**
     * エラー判定しきい値終了コードを設定します。
     *
     * @param errorThreshold エラーしきい値
     */
    public void setErrorThreshold(int errorThreshold) {
        cmdStatus.setErrorThreshold(errorThreshold);
    }

    /**
     * 終了コードが負の値の場合にエラーとするかどうかを取得します。
     *
     * @return 負の値でエラーとする場合は true、それ以外は false
     */
    public boolean isErrAtNegative() {
        return cmdStatus.isErrAtNegative();
    }

    /**
     * 終了コードが負の値の場合にエラーとするかどうかのフラグを設定します。
     *
     * @param errorAtNegativeValue 負の値エラー判定フラグ
     */
    public void setErrAtNegative(boolean errorAtNegativeValue) {
        cmdStatus.setErrAtNegative(errorAtNegativeValue);
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

    /**
     * 終了コードによらず常に正常終了扱いとするかどうかを取得します。
     *
     * @return 常に正常とする場合は true、それ以外は false
     */
    public boolean isAlwaysNormal() {
        return cmdStatus.isAlwaysNormal();
    }

    /**
     * 終了コードによらず常に正常終了扱いとするかどうかのフラグを設定します。
     *
     * @param alwaysNormal 常に正常フラグ
     */
    public void setAlwaysNormal(boolean alwaysNormal) {
        cmdStatus.setAlwaysNormal(alwaysNormal);
    }

    /**
     * エラー時の終了コードを取得します。
     *
     * @return エラー終了コード
     */
    public int getErrorCode() {
        return cmdStatus.getErrorCode();
    }

    /**
     * エラー時の終了コードを設定します。
     *
     * @param errorCode エラー終了コード
     */
    public void setErrorCode(int errorCode) {
        cmdStatus.setErrorCode(errorCode);
    }

    /**
     * 警告時の終了コードを取得します。
     *
     * @return 警告終了コード
     */
    public int getWarnCode() {
        return cmdStatus.getWarnCode();
    }

    /**
     * 警告時の終了コードを設定します。
     *
     * @param warnCode 警告終了コード
     */
    public void setWarnCode(int warnCode) {
        cmdStatus.setWarnCode(warnCode);
    }

    /**
     * プロセスの終了コードを取得します。
     *
     * @return プロセス終了コード
     */
    public int getCmdExitStatus() {
        return cmdExitStatus;
    }

    /**
     * プロセスの終了コードを設定します。
     *
     * @param cmdExitStatus プロセス終了コード
     */
    public void setCmdExitStatus(int cmdExitStatus) {
        this.cmdExitStatus = cmdExitStatus;
    }

    /**
     * 評価後のメソッド終了ステータスを取得します。
     *
     * @return メソッド終了ステータス
     */
    public int getMethodExitStatus() {
        return cmdStatus.getMethodExitStatus();
    }

    /**
     * 評価後のメソッド終了ステータスを設定します。
     *
     * @param methodExitStatus メソッド終了ステータス
     */
    public void setMethodExitStatus(int methodExitStatus) {
        cmdStatus.setMethodExitStatus(methodExitStatus);
    }

    /**
     * 評価後のリターンレベルを取得します。
     *
     * @return リターンレベル
     */
    public int getReturnLevel() {
        return cmdStatus.getReturnLevel();
    }

    /**
     * 評価後のリターンレベルを設定します。
     *
     * @param returnLevel リターンレベル
     */
    public void setReturnLevel(int returnLevel) {
        cmdStatus.setReturnLevel(returnLevel);
    }

    /**
     * 内部状態およびコマンドステータス評価クラスの初期化を行います。
     */
    public void initialize() {
        cmdStatus.setVerbose(verbose);
        cmdStatus.setDebugLevel(debugLevel);
    }

    /**
     * 指定された優先度で別スレッドを起動し、コマンドを実行します。
     *
     * @param priority スレッドの優先度設定
     * @return コマンド評価後のメソッド終了ステータス
     */
    public int executeThread(Object priority) {
        String methodName = "[ClsCmdExec.doThread()][" + prefix + "]";
        if (encoding == null || encoding.isEmpty()) {
            encoding = MdlApp.isWindows() ? "Shift_JIS" : "UTF-8";
        }
        cmdExitStatus = -1;
        cmdStatus.setMethodExitStatus(cmdStatus.getErrorCode() == MdlConst.INT_NULL ? MdlConst.LVL_E : cmdStatus.getErrorCode());
        cmdStatus.setReturnLevel(MdlConst.LVL_E);

        if (isRunning) {
            if (logger != null) {
                logger.writeLine(defaultErrLogLevel, methodName + "[中止] 他の処理が実行中です。");
            }
        } else {
            isRunning = true;
            thread = new Thread(() -> executeThreadWrapper(priority));
            thread.start();
            try {
                thread.join();
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
            }
            isRunning = false;
        }
        return cmdStatus.getMethodExitStatus();
    }

    /**
     * 実行中のプロセスおよびスレッドを強制終了・キャンセルします。
     */
    public void cancel() {
        if (process != null && process.isAlive()) {
            process.destroyForcibly();
        }
        if (thread != null && thread.isAlive()) {
            thread.interrupt();
        }
        isRunning = false;
    }

    /**
     * 内部で出力結果保持に使用しているバッファの内容をクリアします。
     */
    public void clearOutput() {
        STRING_BUILDER.setLength(0);
    }

    /**
     * @deprecated {@link #clearOutput()} を使用してください。
     */
    @Deprecated
    public void clearStringBuilder() {
        clearOutput();
    }

    /**
     * スレッド同期ロックを確立した上でバッファの内容を安全にクリアします。
     */
    public void clearOutputWithLock() {
        synchronized (LOCK_STRING_BUILDER) {
            clearOutput();
        }
    }

    /**
     * @deprecated {@link #clearOutputWithLock()} を使用してください。
     */
    @Deprecated
    public void clearStringBuilderWithLock() {
        clearOutputWithLock();
    }

    private void executeThreadWrapper(Object priority) {
        try {
            executeCore(priority != null ? priority : 3);
        } catch (Exception e) {
            // 中断
        }
    }

    private boolean executeCore(Object priority) {
        String methodName = "[ClsCmdExec.execute()][" + prefix + "]";
        boolean isSuccess = true;
        if (verbose > 4) {
            isShowOutput = true;
        }
        if (verbose > 3) {
            isShowExitCode = true;
        }
        cmdStatus.setMethodExitStatus(cmdStatus.getErrorCode() == MdlConst.INT_NULL ? MdlConst.LVL_E : cmdStatus.getErrorCode());
        cmdStatus.setReturnLevel(MdlConst.LVL_I);
        cmdStatus.initialize();
        cmdStatus.resetFlags();
        boolean hasStandardInput = stdIn != null && !stdIn.isEmpty();

        String output = cmdPath + " " + cmdArgs;
        if (isInfoPrefix) {
            output = methodName + " " + output;
        }
        if ((isShowCmd || verbose > 4) && logger != null) {
            logger.writeLine(debugLevel, output);
        }

        try {
            List<String> commandList = new ArrayList<>();
            commandList.add(cmdPath);
            if (cmdArgs != null && !cmdArgs.isEmpty()) {
                commandList.addAll(Arrays.asList(cmdArgs.split("\\s+")));
            }

            ProcessBuilder pb = new ProcessBuilder(commandList);
            if (workDir != null && !workDir.isEmpty()) {
                pb.directory(new File(workDir));
            }

            if (processEnvCsv != null && !processEnvCsv.isEmpty()) {
                processEnvs = MdlUtil.parseCsvToMap(processEnvs, processEnvCsv, "[,|]", "=", verbose, true, false);
            }
            if (!processEnvs.isEmpty()) {
                Map<String, String> env = pb.environment();
                for (Map.Entry<String, String> entry : processEnvs.entrySet()) {
                    String key = entry.getKey();
                    String value = entry.getValue();
                    if ("+PATH".equalsIgnoreCase(key)) {
                        String currentPath = env.get("PATH");
                        String newPath = value + File.pathSeparator + (currentPath != null ? currentPath : "");
                        if (isShowEnvMap && logger != null) {
                            logger.writeLine(debugLevel, "[SETENV] PATH = " + newPath);
                        }
                        env.put("PATH", newPath);
                    } else {
                        if (isShowEnvMap && logger != null) {
                            logger.writeLine(debugLevel, "[SETENV] " + key + " = " + value);
                        }
                        env.put(key, value);
                    }
                }
            }

            pb.redirectErrorStream(true);
            process = pb.start();

            Charset charset;
            try {
                charset = Charset.forName(encoding);
            } catch (Exception e) {
                charset = Charset.defaultCharset();
            }

            if (hasStandardInput) {
                try (OutputStreamWriter writer = new OutputStreamWriter(process.getOutputStream(), charset)) {
                    writer.write(stdIn);
                    writer.flush();
                }
            }

            try (BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream(), charset))) {
                String line;
                while ((line = reader.readLine()) != null) {
                    synchronized (LOCK_STRING_BUILDER) {
                        STRING_BUILDER.append(line).append(System.lineSeparator());
                    }
                }
            }

            boolean finished = process.waitFor(timeout, TimeUnit.SECONDS);
            if (!finished) {
                cmdStatus.setMethodExitStatus(cmdStatus.getErrorCode() == MdlConst.INT_NULL ? MdlConst.LVL_E : cmdStatus.getErrorCode());
                cmdStatus.setReturnLevel(MdlConst.LVL_E);
                isSuccess = false;
                errorMessage = methodName + " TIMEOUT : " + timeout + "秒 => KILL()";
                if (logger != null) {
                    logger.writeLine(defaultErrLogLevel, errorMessage);
                }
                process.destroyForcibly();
            }

            cmdExitStatus = process.exitValue();

            if (cmdStatus.shouldCheckMessage()) {
                String[] lines = STRING_BUILDER.toString().split("\\r?\\n");
                for (String line : lines) {
                    cmdStatus.checkMessageLine(line);
                }
            }
            cmdStatus.checkCommandExitCode(cmdExitStatus);
            cmdStatus.evaluate();

            if (isShowOutput || (!isNotShowOutput && 0 != cmdExitStatus)) {
                String outStr = STRING_BUILDER.toString().trim();
                for (String line : outStr.split("\\r?\\n")) {
                    String show = isStdoutPrefix ? methodName + " " + line : line;
                    if (isShowEmptyLine || !line.isEmpty()) {
                        if (logger != null) {
                            logger.writeLine(stdoutLevel, show);
                        }
                    }
                }
            }

            if (isShowExitCode || (!isNotShowExitCode && 0 != cmdExitStatus)) {
                output = "コマンド終了コード = " + cmdExitStatus + " => メソッド終了コード = " + cmdStatus.getMethodExitStatus();
                if (isInfoPrefix) {
                    output = methodName + " " + output;
                }
                if (logger != null) {
                    logger.writeLine(debugLevel, output);
                }
            }
        } catch (Exception ex) {
            cmdStatus.setMethodExitStatus(cmdStatus.getErrorCode() == MdlConst.INT_NULL ? MdlConst.LVL_E : cmdStatus.getErrorCode());
            cmdStatus.setReturnLevel(MdlConst.LVL_E);
            isSuccess = false;
            errorMessage = methodName + " EXCEPTION : " + ex.getMessage();
            if (logger != null) {
                logger.writeLine(defaultErrLogLevel, errorMessage);
                if (isStackTrace) {
                    logger.writeLine(MdlConst.LVL_NONE, "");
                    for (StackTraceElement ste : ex.getStackTrace()) {
                        logger.writeLine(MdlConst.LVL_NONE, ste.toString());
                    }
                    logger.writeLine(MdlConst.LVL_NONE, "");
                }
            }
        } finally {
            process = null;
            synchronized (LOCK_STRING_BUILDER) {
                if (isClearOutput) {
                    clearOutput();
                }
            }
        }
        return isSuccess;
    }

    /**
     * 指定されたパスの環境変数定義ファイルを読み込み、環境変数辞書に登録します。
     *
     * @param filePath 環境変数定義ファイルのパス
     * @return ファイルの読み込みおよび登録に成功した場合は true。ファイルが存在しない場合は false
     */
    public boolean readEnvDefFile(String filePath) {
        String methodName = "[ClsCmdExec.ReadEnvDefFile()][" + prefix + "]";
        boolean isSuccess = true;
        String absoluteFilePath = MdlFile.getAbsolutePath(filePath != null ? filePath.trim() : "");
        if (MdlFile.pathExists(absoluteFilePath)) {
            ClsConfigFile configFile = new ClsConfigFile(logger);
            configFile.setConfigMap(processEnvs);
            configFile.setVerbose(verbose);
            configFile.setPattern("^(?<KEY>[^#=]+)=(?<VALUE>.+)$");
            configFile.loadToMap(absoluteFilePath);
        } else {
            isSuccess = false;
            errorMessage = methodName + " NO SUCH A FILE : " + absoluteFilePath;
            if (logger != null) {
                logger.writeLine(defaultErrLogLevel, errorMessage);
            }
        }
        return isSuccess;
    }

    /**
     * @deprecated {@link #readEnvDefFile(String)} を使用してください。
     */
    @Deprecated
    public boolean readEnvironmentDefinitionFile(String filePath) {
        return readEnvDefFile(filePath);
    }
}
