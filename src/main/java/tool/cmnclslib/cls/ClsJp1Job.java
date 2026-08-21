package tool.cmnclslib.cls;

import java.util.regex.Pattern;
import tool.cmnclslib.ifc.ICmnLogger;
import tool.cmnclslib.mdl.MdlConst;
import tool.cmnclslib.mdl.MdlUtil;

/**
 * JP1/AJS3 ジョブの環境変数取得および文字列変換処理を提供するクラスです。
 */
public class ClsJp1Job {

    private final ICmnLogger logger;
    private String jobName = "";
    private String prefix = "AJSENV\\.";
    private String pattern = "\\/|__|\\.\\.";
    private Pattern cachedPattern = Pattern.compile(pattern);
    private int verbose = 0;
    private boolean isAjsJob = false;
    private boolean isSilent = false;

    /**
     * ClsJp1Job クラスの新しいインスタンスを初期化し、JP1ジョブ環境変数を読み込みます。
     *
     * @param logger ログ出力用のロガーインスタンス
     */
    public ClsJp1Job(ICmnLogger logger) {
        this.logger = logger;
        loadEnvVariables();
    }

    public String getPrefix() {
        return prefix;
    }

    public void setPrefix(String prefix) {
        this.prefix = prefix != null ? prefix : "AJSENV\\.";
    }

    public String getPattern() {
        return pattern;
    }

    public void setPattern(String pattern) {
        this.pattern = pattern != null ? pattern : "\\/|__|\\.\\.";
        this.cachedPattern = Pattern.compile(this.pattern);
    }

    public String getJobName() {
        return jobName;
    }

    public void setJobName(String jobName) {
        this.jobName = jobName != null ? jobName : "";
    }

    public int getVerbose() {
        return verbose;
    }

    public void setVerbose(int verbose) {
        this.verbose = verbose;
    }

    public boolean isAjsJob() {
        return isAjsJob;
    }

    public void setAjsJob(boolean ajsJob) {
        this.isAjsJob = ajsJob;
    }

    public boolean isSilent() {
        return isSilent;
    }

    public void setSilent(boolean silent) {
        this.isSilent = silent;
    }

    /**
     * JP1/AJS3 の環境変数 [AJSJOBNAME] の値を取得し、クラス内に保持します。
     *
     * @return 環境変数が設定されており、ジョブ名が取得できた場合は true。それ以外は false
     */
    public boolean loadEnvVariables() {
        String envValue = System.getenv("AJSJOBNAME");
        this.isAjsJob = false;
        if (envValue != null && !envValue.isEmpty()) {
            this.jobName = envValue;
            this.isAjsJob = true;
        }
        return this.isAjsJob;
    }

    /**
     * @deprecated {@link #loadEnvVariables()} を使用してください。
     */
    @Deprecated
    public boolean loadEnvironmentVariables() {
        return loadEnvVariables();
    }

    /**
     * 指定されたジョブ名を保持し、環境変数設定状態とします。
     *
     * @param jobName 設定するJP1ジョブ名
     * @return 処理が成功した場合は true
     */
    public boolean setEnvVariable(String jobName) {
        if (jobName != null && !jobName.isEmpty()) {
            this.jobName = jobName;
            this.isAjsJob = true;
        }
        return setEnvVariable();
    }

    /**
     * @deprecated {@link #setEnvVariable(String)} を使用してください。
     */
    @Deprecated
    public boolean setEnvironmentVariable(String jobName) {
        return setEnvVariable(jobName);
    }

    /**
     * 現在保持しているジョブ名を設定します。
     *
     * @return 処理が成功した場合は true
     */
    public boolean setEnvVariable() {
        return true;
    }

    /**
     * @deprecated {@link #setEnvVariable()} を使用してください。
     */
    @Deprecated
    public boolean setEnvironmentVariable() {
        return setEnvVariable();
    }

    /**
     * 置換対象文字列に含まれるJP1環境変数キーを、環境変数名 [AJSJOBNAME] から抽出した値に変換します。
     *
     * @param replaceTarget 環境変数キーを含む置換対象文字列
     * @return 変換後の文字列
     */
    public String convertFromEnv(String replaceTarget) {
        String methodName = "[ClsJp1Job.ConvertFromEnv()] ";
        String result = replaceTarget;
        String hit = "";
        if (jobName == null || jobName.isEmpty()) {
            return result;
        }

        if (verbose > 4 && logger != null) {
            logger.writeLine(MdlConst.LVL_NONE, methodName + "EXEC GetRegexTarget(" + replaceTarget + "," + prefix + "(?<TARGET>[a-zA-Z0-9_-]+))");
        }

        String key = MdlUtil.getRegexTarget(replaceTarget, prefix + "(?<TARGET>[a-zA-Z0-9_-]+)");

        if (key != null && !key.isEmpty()) {
            if (verbose > 4 && logger != null) {
                logger.writeLine(MdlConst.LVL_NONE, methodName + "KEY FOUND = " + key);
            }

            Pattern regex = cachedPattern != null ? cachedPattern : Pattern.compile(pattern);
            for (String element : regex.split(jobName)) {
                String unit = element != null ? element.trim() : "";
                if (!unit.isEmpty()) {
                    String extractedValue = MdlUtil.getRegexTarget(unit, "^" + key + "\\.(?<TARGET>.+)$");
                    if (extractedValue == null || extractedValue.isEmpty()) {
                        extractedValue = MdlUtil.getRegexTarget(unit, "\\." + key + "\\.(?<TARGET>.+)$");
                    }
                    if (extractedValue != null && !extractedValue.isEmpty()) {
                        hit = extractedValue;
                    }
                }
            }

            if (!hit.isEmpty()) {
                if (hit.toLowerCase().endsWith(".sq")) {
                    hit = "'" + hit.substring(0, hit.length() - 3) + "'";
                } else if (hit.toLowerCase().endsWith(".dq")) {
                    hit = "\"" + hit.substring(0, hit.length() - 3) + "\"";
                }

                if (verbose > 0 && logger != null) {
                    logger.writeLine(MdlConst.LVL_NONE, methodName + "[CONVERT] " + prefix + key + " => " + hit);
                } else {
                    writeLine(MdlConst.LVL_NONE, methodName + "[CONVERT] " + prefix + key + " => " + hit);
                }
                result = hit;
            } else {
                if (verbose > 0 && logger != null) {
                    logger.writeLine(MdlConst.LVL_NONE, methodName + "[NOHIT] " + prefix + key);
                } else {
                    writeLine(MdlConst.LVL_NONE, methodName + "[NOHIT] " + prefix + key);
                }
            }
        } else {
            if (verbose > 4 && logger != null) {
                logger.writeLine(MdlConst.LVL_NONE, methodName + "KEY NOT FOUND");
            }
        }
        return result;
    }

    /**
     * @deprecated {@link #convertFromEnv(String)} を使用してください。
     */
    @Deprecated
    public String convertStringFromEnvironment(String replaceTarget) {
        return convertFromEnv(replaceTarget);
    }

    /**
     * 指定されたレベルとメッセージでログを出力します。
     *
     * @param level ログレベル
     * @param message 出力メッセージ
     */
    public void writeLine(int level, String message) {
        if (isSilent) {
            return;
        }
        if (logger != null) {
            logger.writeLine(level, message);
        } else {
            System.out.println(message);
        }
    }
}
