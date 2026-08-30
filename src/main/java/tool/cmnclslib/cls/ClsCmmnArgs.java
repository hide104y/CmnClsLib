package tool.cmnclslib.cls;

import java.io.File;
import java.nio.charset.Charset;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.regex.Pattern;
import tool.cmnclslib.ifc.ICmnLogger;
import tool.cmnclslib.mdl.MdlArg;
import tool.cmnclslib.mdl.MdlConst;
import tool.cmnclslib.mdl.MdlDate;
import tool.cmnclslib.mdl.MdlFile;
import tool.cmnclslib.mdl.MdlUtil;

/**
 * 共通コマンドライン引数の解析および環境パラメータの管理を行うクラスです。
 */
public class ClsCmmnArgs {

    private ICmnLogger logger;
    private ClsJp1Job jp1;
    private Map<String, String> namedArgs = new LinkedHashMap<>();
    private String exePath = "";
    private String exeDir = "";
    private String exeBaseName = "";
    private String machineName = "";
    private String argDefFilePath = "";
    private String envPrefix = "ENV\\.";
    private int verbose = 0;
    private long pid = 0;
    private boolean isUsage = false;
    private boolean isStackTrace = false;
    private boolean isAjsJob = false;

    // 認証
    private String authDefFilePath = "";
    private String domainName = "";
    private String username = "";
    private String usernameWithoutDomain = "";
    private String password = "";
    private String defaultEncKey = MdlConst.CRYPT_KEY_ALIAS_DEFAULT;
    private String encKey = "";
    private String encKeyEnvName = "";
    private String argKeyOfUserConf = "def";
    private String hashAlgorithm = ClsCrypt.DEFAULT_HASH_ALGORITHM;
    private int keySize = ClsCrypt.DEFAULT_KEY_SIZE;
    private int blockSize = ClsCrypt.DEFAULT_BLOCK_SIZE;
    private int iterationCount = ClsCrypt.DEFAULT_ITERATION_COUNT;
    private boolean isSwitchUser = false;
    private boolean isLogon = false;
    private boolean isLogonAlwaysOk = false;
    private boolean isDecodePasswd = false;
    private boolean isDecodeKey = false;
    private boolean isDebugAuth = false;
    private boolean isDefaultEncKey = false;

    // NET USE
    private String netSharePath = "";
    private String driveName = "";
    private boolean isMount = false;
    private boolean isUmount = false;
    private List<Integer> netUseOkErrNoList = new ArrayList<>();

    // ETC
    private String host = "";
    private String errorMessage = "";
    private String envIdKey = "ENV_ID";
    private String envId = "";
    private String replaceEnvIdKey = "__ENV_ID__";
    private boolean isForce = false;
    private boolean isDiff = false;
    private boolean isGetEnvId = true;
    private int diffLevel = 0;
    private int timeout = 86400;

    // リトライ
    private int retryMax = 0;
    private int retrySleep = 5;

    // 文字列分割パターン
    private String splitPattern = "[,\\/|]";
    private String keyValDelimiter = "[:]";

    // フィルター
    private boolean isRegIncBasename = true;
    private boolean isRegExcBasename = true;
    private boolean isIncHitRecursive = true;
    private boolean isExcHitRecursive = true;
    private boolean isDirFilterOr = false;

    // 引数名リスト
    private List<String> keyNameOfUsernameList = new ArrayList<>();
    private List<String> keyNameOfPasswordList = new ArrayList<>();
    private List<String> keyNameOfEncPassList = new ArrayList<>();
    private List<String> keyNameOfEncKeyList = new ArrayList<>();
    private List<String> keyNameOfEncEncKeyList = new ArrayList<>();
    private List<String> keyNameOfEncKeySizeList = new ArrayList<>();
    private List<String> incFilesList = new ArrayList<>();
    private List<String> excFilesList = new ArrayList<>();
    private List<String> incDirsList = new ArrayList<>();
    private List<String> excDirsList = new ArrayList<>();

    // 置換辞書
    private Map<String, String> replaceDic = new LinkedHashMap<>();
    private Map<String, String> shortDic = new LinkedHashMap<>();
    private Map<String, String> authDefDic = new LinkedHashMap<>();

    /**
     * ClsCmmnArgs クラスの新しいインスタンスを初期化します。
     *
     * @param logger ログ出力用のロガーインスタンス
     */
    public ClsCmmnArgs(ICmnLogger logger) {
        this.logger = logger;
        this.jp1 = new ClsJp1Job(logger);
        initializeLists();
    }

    public Map<String, String> getNamedArgs() {
        return namedArgs;
    }

    public void setNamedArgs(Map<String, String> namedArgs) {
        this.namedArgs = namedArgs != null ? namedArgs : new LinkedHashMap<>();
    }

    public Map<String, String> getAuthDefMap() {
        return authDefDic;
    }

    public void setAuthDefMap(Map<String, String> authDefDic) {
        this.authDefDic = authDefDic != null ? authDefDic : new LinkedHashMap<>();
    }

    /**
     * @deprecated {@link #getAuthDefMap()} を使用してください。
     */
    @Deprecated
    public Map<String, String> getDicAuthDef() {
        return getAuthDefMap();
    }

    /**
     * @deprecated {@link #setAuthDefMap(Map)} を使用してください。
     */
    @Deprecated
    public void setDicAuthDef(Map<String, String> authDefDic) {
        setAuthDefMap(authDefDic);
    }

    public ClsJp1Job getJp1() {
        return jp1;
    }

    public void setJp1(ClsJp1Job jp1) {
        this.jp1 = jp1;
    }

    public String getExeBaseName() {
        return exeBaseName;
    }

    public void setExeBaseName(String exeBaseName) {
        this.exeBaseName = exeBaseName != null ? exeBaseName : "";
    }

    public String getExePath() {
        return exePath;
    }

    public void setExePath(String exePath) {
        this.exePath = exePath != null ? exePath : "";
    }

    public String getExeDir() {
        return exeDir;
    }

    public void setExeDir(String exeDir) {
        this.exeDir = exeDir != null ? exeDir : "";
    }

    public String getMachineName() {
        return machineName;
    }

    public void setMachineName(String machineName) {
        this.machineName = machineName != null ? machineName : "";
    }

    public long getPid() {
        return pid;
    }

    public void setPid(long pid) {
        this.pid = pid;
    }

    public boolean isUsage() {
        return isUsage;
    }

    public void setUsage(boolean usage) {
        isUsage = usage;
    }

    public int getVerbose() {
        return verbose;
    }

    public void setVerbose(int verbose) {
        this.verbose = verbose;
    }

    public boolean isStackTrace() {
        return isStackTrace;
    }

    public void setStackTrace(boolean stackTrace) {
        isStackTrace = stackTrace;
    }

    public boolean isAjsJob() {
        return isAjsJob;
    }

    public void setAjsJob(boolean ajsJob) {
        isAjsJob = ajsJob;
    }

    public String getAuthDefFilePath() {
        return authDefFilePath;
    }

    public void setAuthDefFilePath(String authDefFilePath) {
        this.authDefFilePath = authDefFilePath != null ? authDefFilePath : "";
    }

    public String getArgKeyOfUserConf() {
        return argKeyOfUserConf;
    }

    public void setArgKeyOfUserConf(String argKeyOfUserConf) {
        this.argKeyOfUserConf = argKeyOfUserConf != null ? argKeyOfUserConf : "def";
    }

    public String getDomainName() {
        return domainName;
    }

    public void setDomainName(String domainName) {
        this.domainName = domainName != null ? domainName : "";
    }

    public String getUsername() {
        return username;
    }

    public void setUsername(String username) {
        this.username = username != null ? username : "";
    }

    public String getUserNoDomain() {
        return usernameWithoutDomain;
    }

    public void setUserNoDomain(String usernameWithoutDomain) {
        this.usernameWithoutDomain = usernameWithoutDomain != null ? usernameWithoutDomain : "";
    }

    /**
     * @deprecated {@link #getUserNoDomain()} を使用してください。
     */
    @Deprecated
    public String getUsernameWithoutDomain() {
        return getUserNoDomain();
    }

    /**
     * @deprecated {@link #setUserNoDomain(String)} を使用してください。
     */
    @Deprecated
    public void setUsernameWithoutDomain(String usernameWithoutDomain) {
        setUserNoDomain(usernameWithoutDomain);
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password != null ? password : "";
    }

    public String getEncKey() {
        return encKey;
    }

    public void setEncKey(String encKey) {
        this.encKey = encKey != null ? encKey : "";
    }

    public String getEncKeyEnvName() {
        return encKeyEnvName;
    }

    public void setEncKeyEnvName(String encKeyEnvName) {
        this.encKeyEnvName = encKeyEnvName != null ? encKeyEnvName : "";
    }

    public String getDefaultEncKey() {
        return defaultEncKey;
    }

    public void setDefaultEncKey(String defaultEncKey) {
        this.defaultEncKey = defaultEncKey != null ? defaultEncKey : MdlConst.CRYPT_KEY_ALIAS_DEFAULT;
    }

    public int getKeySize() {
        return keySize;
    }

    public void setKeySize(int keySize) {
        this.keySize = keySize;
    }

    public int getBlockSize() {
        return blockSize;
    }

    public void setBlockSize(int blockSize) {
        this.blockSize = blockSize;
    }

    public String getHashAlgorithm() {
        return hashAlgorithm;
    }

    public void setHashAlgorithm(String hashAlgorithm) {
        this.hashAlgorithm = hashAlgorithm != null ? hashAlgorithm.toUpperCase(Locale.ROOT) : ClsCrypt.DEFAULT_HASH_ALGORITHM;
    }

    public int getIterationCount() {
        return iterationCount;
    }

    public void setIterationCount(int iterationCount) {
        this.iterationCount = iterationCount;
    }

    public boolean isSwitchUser() {
        return isSwitchUser;
    }

    public void setSwitchUser(boolean switchUser) {
        isSwitchUser = switchUser;
    }

    public boolean isLogon() {
        return isLogon;
    }

    public void setLogon(boolean logon) {
        isLogon = logon;
    }

    public boolean isLogonAlwaysOk() {
        return isLogonAlwaysOk;
    }

    public void setLogonAlwaysOk(boolean logonAlwaysOk) {
        isLogonAlwaysOk = logonAlwaysOk;
    }

    public boolean isDecodePasswd() {
        return isDecodePasswd;
    }

    public void setDecodePasswd(boolean decodePasswd) {
        isDecodePasswd = decodePasswd;
    }

    public boolean isDecodeKey() {
        return isDecodeKey;
    }

    public void setDecodeKey(boolean decodeKey) {
        isDecodeKey = decodeKey;
    }

    public boolean isDefaultEncKey() {
        return isDefaultEncKey;
    }

    public void setDefaultEncKey(boolean defaultEncKey) {
        isDefaultEncKey = defaultEncKey;
    }

    public String getNetSharePath() {
        return netSharePath;
    }

    public void setNetSharePath(String netSharePath) {
        this.netSharePath = netSharePath != null ? netSharePath : "";
    }

    public String getDriveName() {
        return driveName;
    }

    public void setDriveName(String driveName) {
        this.driveName = driveName != null ? driveName : "";
    }

    public boolean isMount() {
        return isMount;
    }

    public void setMount(boolean mount) {
        isMount = mount;
    }

    public boolean isUmount() {
        return isUmount;
    }

    public void setUmount(boolean umount) {
        isUmount = umount;
    }

    public List<Integer> getNetUseOkErrNoList() {
        return netUseOkErrNoList;
    }

    public void setNetUseOkErrNoList(List<Integer> netUseOkErrNoList) {
        this.netUseOkErrNoList = netUseOkErrNoList != null ? netUseOkErrNoList : new ArrayList<>();
    }

    public String getHost() {
        return host;
    }

    public void setHost(String host) {
        this.host = host != null ? host : "";
    }

    public String getEnvIdKey() {
        return envIdKey;
    }

    public void setEnvIdKey(String envIdKey) {
        this.envIdKey = envIdKey != null ? envIdKey : "ENV_ID";
    }

    public String getEnvId() {
        return envId;
    }

    public void setEnvId(String envId) {
        this.envId = envId != null ? envId : "";
    }

    public String getRunEnvKey() {
        return replaceEnvIdKey;
    }

    public void setRunEnvKey(String runEnvKey) {
        this.replaceEnvIdKey = runEnvKey != null ? runEnvKey : "__ENV_ID__";
    }

    public String getRunEnv() {
        return envId;
    }

    public void setRunEnv(String runEnv) {
        this.envId = runEnv != null ? runEnv : "";
    }

    public String getReplaceEnvIdKey() {
        return replaceEnvIdKey;
    }

    public void setReplaceEnvIdKey(String replaceEnvIdKey) {
        this.replaceEnvIdKey = replaceEnvIdKey != null ? replaceEnvIdKey : "__ENV_ID__";
    }

    public String getErrorMessage() {
        return errorMessage;
    }

    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage != null ? errorMessage : "";
    }

    public boolean isForce() {
        return isForce;
    }

    public void setForce(boolean force) {
        isForce = force;
    }

    public boolean isDiff() {
        return isDiff;
    }

    public void setDiff(boolean diff) {
        isDiff = diff;
    }

    public boolean isGetEnvId() {
        return isGetEnvId;
    }

    public void setGetEnvId(boolean getEnvId) {
        isGetEnvId = getEnvId;
    }

    public boolean isDebugAuth() {
        return isDebugAuth;
    }

    public void setDebugAuth(boolean debugAuth) {
        isDebugAuth = debugAuth;
    }

    public int getDiffLevel() {
        return diffLevel;
    }

    public void setDiffLevel(int diffLevel) {
        this.diffLevel = diffLevel;
    }

    public int getTimeout() {
        return timeout;
    }

    public void setTimeout(int timeout) {
        this.timeout = timeout;
    }

    public int getRetryMax() {
        return retryMax;
    }

    public void setRetryMax(int retryMax) {
        this.retryMax = retryMax;
    }

    public int getRetrySleep() {
        return retrySleep;
    }

    public void setRetrySleep(int retrySleep) {
        this.retrySleep = retrySleep;
    }

    public String getSplitPattern() {
        return splitPattern;
    }

    public void setSplitPattern(String splitPattern) {
        this.splitPattern = splitPattern != null ? splitPattern : "[,\\/|]";
    }

    public String getKeyValDelimiter() {
        return keyValDelimiter;
    }

    public void setKeyValDelimiter(String keyValDelimiter) {
        this.keyValDelimiter = keyValDelimiter != null ? keyValDelimiter : "[:]";
    }

    public boolean isRegIncBasename() {
        return isRegIncBasename;
    }

    public void setRegIncBasename(boolean regIncBasename) {
        isRegIncBasename = regIncBasename;
    }

    public boolean isRegExcBasename() {
        return isRegExcBasename;
    }

    public void setRegExcBasename(boolean regExcBasename) {
        isRegExcBasename = regExcBasename;
    }

    public boolean isIncHitRecursive() {
        return isIncHitRecursive;
    }

    public void setIncHitRecursive(boolean incHitRecursive) {
        isIncHitRecursive = incHitRecursive;
    }

    public boolean isExcHitRecursive() {
        return isExcHitRecursive;
    }

    public void setExcHitRecursive(boolean excHitRecursive) {
        isExcHitRecursive = excHitRecursive;
    }

    public boolean isDirFilterOr() {
        return isDirFilterOr;
    }

    public void setDirFilterOr(boolean dirFilterOr) {
        isDirFilterOr = dirFilterOr;
    }

    public List<String> getUserKeyNames() {
        return keyNameOfUsernameList;
    }

    public void setUserKeyNames(List<String> keyNameOfUsernameList) {
        this.keyNameOfUsernameList = keyNameOfUsernameList != null ? keyNameOfUsernameList : new ArrayList<>();
    }

    /**
     * @deprecated {@link #getUserKeyNames()} を使用してください。
     */
    @Deprecated
    public List<String> getKeyNameOfUsernameList() {
        return getUserKeyNames();
    }

    /**
     * @deprecated {@link #setUserKeyNames(List)} を使用してください。
     */
    @Deprecated
    public void setKeyNameOfUsernameList(List<String> keyNameOfUsernameList) {
        setUserKeyNames(keyNameOfUsernameList);
    }

    public List<String> getPassKeyNames() {
        return keyNameOfPasswordList;
    }

    public void setPassKeyNames(List<String> keyNameOfPasswordList) {
        this.keyNameOfPasswordList = keyNameOfPasswordList != null ? keyNameOfPasswordList : new ArrayList<>();
    }

    /**
     * @deprecated {@link #getPassKeyNames()} を使用してください。
     */
    @Deprecated
    public List<String> getKeyNameOfPasswordList() {
        return getPassKeyNames();
    }

    /**
     * @deprecated {@link #setPassKeyNames(List)} を使用してください。
     */
    @Deprecated
    public void setKeyNameOfPasswordList(List<String> keyNameOfPasswordList) {
        setPassKeyNames(keyNameOfPasswordList);
    }

    public List<String> getEncPassKeyNames() {
        return keyNameOfEncPassList;
    }

    public void setEncPassKeyNames(List<String> keyNameOfEncPassList) {
        this.keyNameOfEncPassList = keyNameOfEncPassList != null ? keyNameOfEncPassList : new ArrayList<>();
    }

    /**
     * @deprecated {@link #getEncPassKeyNames()} を使用してください。
     */
    @Deprecated
    public List<String> getKeyNameOfEncPassList() {
        return getEncPassKeyNames();
    }

    /**
     * @deprecated {@link #setEncPassKeyNames(List)} を使用してください。
     */
    @Deprecated
    public void setKeyNameOfEncPassList(List<String> keyNameOfEncPassList) {
        setEncPassKeyNames(keyNameOfEncPassList);
    }

    public List<String> getEncKeyNames() {
        return keyNameOfEncKeyList;
    }

    public void setEncKeyNames(List<String> keyNameOfEncKeyList) {
        this.keyNameOfEncKeyList = keyNameOfEncKeyList != null ? keyNameOfEncKeyList : new ArrayList<>();
    }

    /**
     * @deprecated {@link #getEncKeyNames()} を使用してください。
     */
    @Deprecated
    public List<String> getKeyNameOfEncKeyList() {
        return getEncKeyNames();
    }

    /**
     * @deprecated {@link #setEncKeyNames(List)} を使用してください。
     */
    @Deprecated
    public void setKeyNameOfEncKeyList(List<String> keyNameOfEncKeyList) {
        setEncKeyNames(keyNameOfEncKeyList);
    }

    public List<String> getEncEncKeyNames() {
        return keyNameOfEncEncKeyList;
    }

    public void setEncEncKeyNames(List<String> keyNameOfEncEncKeyList) {
        this.keyNameOfEncEncKeyList = keyNameOfEncEncKeyList != null ? keyNameOfEncEncKeyList : new ArrayList<>();
    }

    /**
     * @deprecated {@link #getEncEncKeyNames()} を使用してください。
     */
    @Deprecated
    public List<String> getKeyNameOfEncEncKeyList() {
        return getEncEncKeyNames();
    }

    /**
     * @deprecated {@link #setEncEncKeyNames(List)} を使用してください。
     */
    @Deprecated
    public void setKeyNameOfEncEncKeyList(List<String> keyNameOfEncEncKeyList) {
        setEncEncKeyNames(keyNameOfEncEncKeyList);
    }

    public List<String> getEncKeySizeNames() {
        return keyNameOfEncKeySizeList;
    }

    public void setEncKeySizeNames(List<String> keyNameOfEncKeySizeList) {
        this.keyNameOfEncKeySizeList = keyNameOfEncKeySizeList != null ? keyNameOfEncKeySizeList : new ArrayList<>();
    }

    /**
     * @deprecated {@link #getEncKeySizeNames()} を使用してください。
     */
    @Deprecated
    public List<String> getKeyNameOfEncKeySizeList() {
        return getEncKeySizeNames();
    }

    /**
     * @deprecated {@link #setEncKeySizeNames(List)} を使用してください。
     */
    @Deprecated
    public void setKeyNameOfEncKeySizeList(List<String> keyNameOfEncKeySizeList) {
        setEncKeySizeNames(keyNameOfEncKeySizeList);
    }

    public List<String> getIncFilesList() {
        return incFilesList;
    }

    public void setIncFilesList(List<String> incFilesList) {
        this.incFilesList = incFilesList != null ? incFilesList : new ArrayList<>();
    }

    public List<String> getExcFilesList() {
        return excFilesList;
    }

    public void setExcFilesList(List<String> excFilesList) {
        this.excFilesList = excFilesList != null ? excFilesList : new ArrayList<>();
    }

    public List<String> getIncDirsList() {
        return incDirsList;
    }

    public void setIncDirsList(List<String> incDirsList) {
        this.incDirsList = incDirsList != null ? incDirsList : new ArrayList<>();
    }

    public List<String> getExcDirsList() {
        return excDirsList;
    }

    public void setExcDirsList(List<String> excDirsList) {
        this.excDirsList = excDirsList != null ? excDirsList : new ArrayList<>();
    }

    public Map<String, String> getReplaceMap() {
        return replaceDic;
    }

    public void setReplaceMap(Map<String, String> replaceDic) {
        this.replaceDic = replaceDic != null ? replaceDic : new LinkedHashMap<>();
    }

    /**
     * @deprecated {@link #getReplaceMap()} を使用してください。
     */
    @Deprecated
    public Map<String, String> getReplaceDic() {
        return getReplaceMap();
    }

    /**
     * @deprecated {@link #setReplaceMap(Map)} を使用してください。
     */
    @Deprecated
    public void setReplaceDic(Map<String, String> replaceDic) {
        setReplaceMap(replaceDic);
    }

    /**
     * @deprecated {@link #getReplaceMap()} を使用してください。
     */
    @Deprecated
    public Map<String, String> getReplaceDictionary() {
        return getReplaceMap();
    }

    /**
     * @deprecated {@link #setReplaceMap(Map)} を使用してください。
     */
    @Deprecated
    public void setReplaceDictionary(Map<String, String> replaceDic) {
        setReplaceMap(replaceDic);
    }

    public Map<String, String> getShortMap() {
        return shortDic;
    }

    public void setShortMap(Map<String, String> shortDic) {
        this.shortDic = shortDic != null ? shortDic : new LinkedHashMap<>();
    }

    /**
     * @deprecated {@link #getShortMap()} を使用してください。
     */
    @Deprecated
    public Map<String, String> getShortDic() {
        return getShortMap();
    }

    /**
     * @deprecated {@link #setShortMap(Map)} を使用してください。
     */
    @Deprecated
    public void setShortDic(Map<String, String> shortDic) {
        setShortMap(shortDic);
    }

    /**
     * 各種デフォルト引数名キーリストの初期化および環境情報の取得を行います。
     */
    public void initializeLists() {
        keyNameOfUsernameList.addAll(Arrays.asList("username", "user", "u"));
        keyNameOfPasswordList.addAll(Arrays.asList("password", "pass", "p"));
        keyNameOfEncPassList.addAll(Arrays.asList("encodedpassword", "encpass", "ep"));
        keyNameOfEncKeyList.addAll(Arrays.asList("enckey", "key", "k"));
        keyNameOfEncEncKeyList.addAll(Arrays.asList("encenckey", "ek"));
        keyNameOfEncKeySizeList.addAll(Arrays.asList("keysize", "size", "s"));

        try {
            machineName = System.getenv("COMPUTERNAME");
            if (machineName == null || machineName.isEmpty()) {
                machineName = java.net.InetAddress.getLocalHost().getHostName();
            }
        } catch (Exception e) {
            machineName = "";
        }
    }

    /**
     * 現在のプロセスのモジュール情報を取得します。
     *
     * @return モジュール情報の取得に成功した場合は true、失敗した場合は false
     */
    public boolean getModuleInfo() {
        return getModuleInfo("");
    }

    /**
     * 指定された実行ファイルパスからモジュール情報を取得します。
     *
     * @param exePath 実行ファイルのフルパス
     * @return モジュール情報の取得に成功した場合は true、失敗した場合は false
     */
    public boolean getModuleInfo(String exePath) {
        try {
            String path = exePath;
            if (path == null || path.isEmpty() || !MdlFile.pathExists(path)) {
                path = System.getProperty("java.class.path", "");
            }
            this.exeDir = MdlFile.getDirectoryPath(path);
            this.exeBaseName = MdlFile.getBaseName(path);
            this.pid = ProcessHandle.current().pid();
            return true;
        } catch (Exception e) {
            return false;
        }
    }

    /**
     * コマンドライン引数辞書から一般的な共通引数を解析して保持します。
     *
     * @return 共通引数の取得・解析が成功した場合は true、それ以外は false
     */
    public boolean getCommonArgs() {
        final String strMyName = "[ClsCmmnArgs.GetCommonArgs()]";
        boolean isOk = true;
        String tempStr = "";

        if (namedArgs.containsKey("arg-def")) {
            argDefFilePath = namedArgs.get("arg-def");
            if (argDefFilePath != null && !argDefFilePath.isEmpty()) {
                try {
                    Map<String, String> dicNamedArg = MdlFile.readFileToMap(argDefFilePath);
                    if (!dicNamedArg.isEmpty()) {
                        for (Map.Entry<String, String> entry : dicNamedArg.entrySet()) {
                            namedArgs.putIfAbsent(entry.getKey(), entry.getValue());
                        }
                    }
                } catch (Exception ex) {
                    writeLine(MdlConst.LVL_E, strMyName + "[-arg-def " + argDefFilePath + "] EXCEPTION : " + ex.getMessage());
                }
            }
        }

        if (namedArgs.containsKey("h")) {
            String hVal = namedArgs.get("h");
            if (hVal != null && !hVal.isEmpty()) {
                host = hVal.trim();
            } else {
                isUsage = true;
            }
        }

        if (namedArgs.containsKey("help") || namedArgs.containsKey("?")) {
            isUsage = true;
        }

        if (MdlArg.containsKey(namedArgs, "force")) {
            isForce = true;
        }

        String[] vKeys = new String[] {"v", "vv", "vvv", "vvvv", "vvvvv", "vvvvvv", "vvvvvvv", "vvvvvvvv", "vvvvvvvvv", "vvvvvvvvvv", "vvvvvvvvvvv", "vvvvvvvvvvvv"};
        for (String key : vKeys) {
            if (namedArgs.containsKey(key)) {
                verbose = key.length();
                String vVal = namedArgs.get(key);
                if (vVal != null && !vVal.isEmpty()) {
                    int tempInt = MdlUtil.parseInt(vVal, MdlConst.INT_NULL);
                    if (tempInt != MdlConst.INT_NULL) {
                        verbose = tempInt;
                    }
                    break;
                }
            }
        }
        if (namedArgs.containsKey("brief")) {
            verbose = -1;
            String briefVal = namedArgs.get("brief");
            if (briefVal != null && !briefVal.isEmpty()) {
                int tempInt = MdlUtil.parseInt(briefVal, MdlConst.INT_NULL);
                if (tempInt != MdlConst.INT_NULL) {
                    verbose = -1 * tempInt;
                }
            }
        }

        if (namedArgs.containsKey("diff")) {
            isDiff = true;
            String diffVal = namedArgs.get("diff");
            if (diffVal != null && !diffVal.isEmpty()) {
                int tempInt = MdlUtil.parseInt(diffVal, MdlConst.INT_NULL);
                if (tempInt != MdlConst.INT_NULL) {
                    diffLevel = tempInt;
                }
            }
        }

        if (namedArgs.containsKey("console")) {
            String consoleVal = namedArgs.get("console");
            if (consoleVal != null && !consoleVal.isEmpty()) {
                String lower = consoleVal.toLowerCase(Locale.ROOT);
                if ("off".equals(lower)) {
                    logger.setValueByKey(ClsLogger.IS_CONSOLE, "false");
                    logger.setValueByKey(ClsLogger.IS_STDOUT, "false");
                    logger.setValueByKey(ClsLogger.IS_STDERR, "false");
                } else if ("stdout".equals(lower)) {
                    logger.setValueByKey(ClsLogger.IS_CONSOLE, "true");
                    logger.setValueByKey(ClsLogger.IS_STDOUT, "true");
                    logger.setValueByKey(ClsLogger.IS_STDERR, "false");
                } else if ("stderr".equals(lower)) {
                    logger.setValueByKey(ClsLogger.IS_CONSOLE, "true");
                    logger.setValueByKey(ClsLogger.IS_STDOUT, "false");
                    logger.setValueByKey(ClsLogger.IS_STDERR, "true");
                }
            }
        }

        if (namedArgs.containsKey("stacktrace")) {
            isStackTrace = true;
        }

        if (namedArgs.containsKey("stdenc")) {
            String stdEncVal = namedArgs.get("stdenc");
            if (stdEncVal != null && !stdEncVal.isEmpty()) {
                logger.setValueByKey(ClsLogger.IS_CONSOLE_ENCODING, "true");
                logger.setValueByKey(ClsLogger.CONSOLE_ENCODING, stdEncVal.trim());
            }
        }

        if (namedArgs.containsKey("env-enckey")) {
            String envEncKeyVal = namedArgs.get("env-enckey");
            if (envEncKeyVal != null && !envEncKeyVal.isEmpty()) {
                encKeyEnvName = envEncKeyVal.trim();
            }
        }

        if (jp1 != null && jp1.isAjsJob()) {
            logger.setValueByKey(ClsLogger.IS_STDERR, "true");
        }

        if (namedArgs.containsKey("ajsjobname")) {
            String ajsJobVal = namedArgs.get("ajsjobname");
            if (ajsJobVal != null && !ajsJobVal.isEmpty() && jp1 != null) {
                jp1.setEnvVariable(ajsJobVal);
            }
        }

        if (namedArgs.containsKey("nojp1") && jp1 != null) {
            jp1.setAjsJob(false);
        }

        if (jp1 != null) {
            isAjsJob = jp1.isAjsJob();
        }

        if (namedArgs.containsKey("envajs")) {
            String envAjsVal = namedArgs.get("envajs");
            if (envAjsVal != null && !envAjsVal.isEmpty() && jp1 != null) {
                jp1.setPrefix(envAjsVal);
            }
        }

        if (namedArgs.containsKey("envvar")) {
            String envVarVal = namedArgs.get("envvar");
            if (envVarVal != null && !envVarVal.isEmpty()) {
                envPrefix = envVarVal;
            }
        }

        if (namedArgs.containsKey("envenvid")) {
            String envEnvIdVal = namedArgs.get("envenvid");
            if (envEnvIdVal != null && !envEnvIdVal.isEmpty()) {
                envIdKey = envEnvIdVal;
            }
        }
        if (isGetEnvId) {
            String envVal = System.getenv(envIdKey);
            envId = envVal != null ? envVal : "";
        }

        if (namedArgs.containsKey("splitby")) {
            String splitByVal = namedArgs.get("splitby");
            if (splitByVal != null && !splitByVal.isEmpty()) {
                splitPattern = splitByVal;
            }
        }

        if (namedArgs.containsKey("split-kv-by")) {
            String splitKvVal = namedArgs.get("split-kv-by");
            if (splitKvVal != null && !splitKvVal.isEmpty()) {
                keyValDelimiter = splitKvVal;
            }
        }

        if (namedArgs.containsKey("replace")) {
            String replaceVal = namedArgs.get("replace");
            if (replaceVal != null && !replaceVal.isEmpty()) {
                for (String pair : MdlUtil.parseCsvToList(null, replaceVal, splitPattern, verbose, true)) {
                    String[] pairParts = pair.split(keyValDelimiter);
                    if (pairParts.length > 1) {
                        String replaceTo = pairParts[1];
                        if (isAjsJob && jp1 != null && Pattern.compile("^" + jp1.getPrefix()).matcher(replaceTo).find()) {
                            replaceTo = jp1.convertFromEnv(replaceTo);
                        }
                        String envName = MdlUtil.getRegexTarget(replaceTo, "^" + envPrefix + "(?<TARGET>.+)$");
                        if (envName != null && !envName.isEmpty()) {
                            String strEnvVal = System.getenv(envName);
                            if (strEnvVal != null) {
                                replaceTo = strEnvVal;
                            }
                        }
                        if (!shortDic.isEmpty()) {
                            for (Map.Entry<String, String> entry : shortDic.entrySet()) {
                                replaceTo = Pattern.compile("^" + entry.getKey() + "$", Pattern.CASE_INSENSITIVE).matcher(replaceTo != null ? replaceTo : "").replaceAll(entry.getValue());
                            }
                        }
                        replaceDic.put(pairParts[0], replaceTo != null ? replaceTo : "");
                    }
                }
            }
        }

        if (namedArgs.containsKey("reservereplace")) {
            shortDic.put("prod", "production");
            shortDic.put("stg", "staging");
            shortDic.put("dev", "development");
        }

        if (namedArgs.containsKey("morereplace")) {
            String moreReplaceVal = namedArgs.get("morereplace");
            if (moreReplaceVal != null && !moreReplaceVal.isEmpty()) {
                for (String pair : MdlUtil.parseCsvToList(null, moreReplaceVal, splitPattern, verbose, true)) {
                    String[] pairParts = pair.split(":");
                    if (pairParts.length > 1) {
                        shortDic.put(pairParts[0], pairParts[1]);
                    }
                }
            }
        }

        if (envId != null && !envId.isEmpty()) {
            replaceDic.putIfAbsent(replaceEnvIdKey, envId);
        }

        if (host != null && !host.isEmpty()) {
            host = replaceByMap(host);
        }

        String[] ldirKeys = new String[] {"ldir", "ldir-n"};
        for (String key : ldirKeys) {
            if (MdlArg.containsKey(namedArgs, key)) {
                tempStr = "";
                if ("ldir".equals(key)) {
                    tempStr = getPathParam("ldir", MdlFile.PATH_IS_DIRECTORY, true);
                } else if ("ldir-n".equals(key)) {
                    logger.setValueByKey(ClsLogger.IS_APPEND, "false");
                    tempStr = getPathParam("ldir-n", MdlFile.PATH_IS_DIRECTORY, true);
                }
                if (!tempStr.isEmpty()) {
                    logger.setValueByKey(ClsLogger.IS_FILE, "true");
                    logger.setValueByKey(ClsLogger.DIR, tempStr);
                    logger.setValueByKey(ClsLogger.PATH, MdlFile.combinePath(tempStr, exeBaseName + "." + MdlDate.getFormattedDate("yyyyMMdd.HHmmss") + "." + pid + ".log"));
                    break;
                }
            }
        }

        String[] logKeys = new String[] {"log", "log-n"};
        for (String key : logKeys) {
            if (MdlArg.containsKey(namedArgs, key)) {
                tempStr = "";
                if ("log".equals(key)) {
                    tempStr = getPathParam("log", MdlFile.PATH_IS_FILE, true);
                } else if ("log-n".equals(key)) {
                    logger.setValueByKey(ClsLogger.IS_APPEND, "false");
                    tempStr = getPathParam("log-n", MdlFile.PATH_IS_FILE, true);
                }
                if (!tempStr.isEmpty()) {
                    logger.setValueByKey(ClsLogger.DIR, MdlFile.getDirectoryPath(tempStr));
                    if (MdlFile.createDirectory(logger.getValueByKey(ClsLogger.DIR, "")) < MdlFile.OK_MKDIR_HANTEI) {
                        logger.setValueByKey(ClsLogger.IS_FILE, "true");
                        if (verbose > 4) {
                            writeLine(MdlConst.LVL_DEBUG, strMyName + " -log : " + tempStr);
                        }
                        logger.setValueByKey(ClsLogger.PATH, MdlDate.replaceWithDateTime(tempStr.replace("%%", "%")));
                        break;
                    } else {
                        isOk = false;
                    }
                }
            }
        }

        if (MdlArg.containsKey(namedArgs, "logenc")) {
            tempStr = MdlArg.getValue(namedArgs, "logenc");
            if (tempStr != null && !tempStr.isEmpty()) {
                logger.setValueByKey(ClsLogger.FILE_ENCODING, tempStr);
            }
        }

        if (logger.getValueByKey(ClsLogger.IS_FILE, false) && verbose > 4) {
            writeLine(MdlConst.LVL_DEBUG, strMyName + " LogDir : " + logger.getValueByKey(ClsLogger.DIR, ""));
            writeLine(MdlConst.LVL_DEBUG, strMyName + " Path : " + logger.getValueByKey(ClsLogger.PATH, ""));
        }

        if (namedArgs.containsKey("retry")) {
            int tempInt = MdlUtil.parseInt(namedArgs.get("retry"), MdlConst.INT_NULL);
            if (tempInt != MdlConst.INT_NULL) {
                retryMax = Math.max(0, tempInt);
            }
        }

        if (namedArgs.containsKey("sleep")) {
            int tempInt = MdlUtil.parseInt(namedArgs.get("sleep"), MdlConst.INT_NULL);
            if (tempInt != MdlConst.INT_NULL) {
                retrySleep = Math.max(1, tempInt);
            }
        }

        if (namedArgs.containsKey("timeout")) {
            int tempInt = MdlUtil.parseInt(namedArgs.get("timeout"), MdlConst.INT_NULL);
            if (tempInt != MdlConst.INT_NULL) {
                timeout = Math.max(1, tempInt);
            }
        }

        if (namedArgs.containsKey("dumpargs")) {
            for (Map.Entry<String, String> entry : namedArgs.entrySet()) {
                writeLine(MdlConst.LVL_DEBUG, "ARG : -" + entry.getKey() + " " + entry.getValue());
            }
        }

        if (namedArgs.containsKey("dumpreplace")) {
            for (Map.Entry<String, String> entry : replaceDic.entrySet()) {
                writeLine(MdlConst.LVL_DEBUG, "[REPLACE] KEY = " + entry.getKey() + " / VAL = " + entry.getValue());
            }
            for (Map.Entry<String, String> entry : shortDic.entrySet()) {
                writeLine(MdlConst.LVL_DEBUG, "[MOREREPLACE] KEY = " + entry.getKey() + " / VAL = " + entry.getValue());
            }
        }

        if (namedArgs.containsKey("debug-auth")) {
            isDebugAuth = true;
        }

        if (namedArgs.containsKey("hh") || namedArgs.containsKey("??")) {
            isUsage = true;
            getArgsForAuth();
            showUsage();
        }

        return isOk;
    }

    /**
     * 認証情報に関連するコマンドライン引数を取得および解析します。
     *
     * @return 認証引数の取得・解析が正常に完了した場合は true、それ以外は false
     */
    public boolean getArgsForAuth() {
        boolean isOk = true;
        if (MdlArg.containsKey(namedArgs, "auth-conf-key")) {
            String tempStr = MdlArg.getValue(namedArgs, "auth-conf-key");
            if (tempStr != null && !tempStr.isEmpty()) {
                argKeyOfUserConf = tempStr;
            }
        }
        if (isOk) {
            isOk = getArgsForUserDefFile();
        }
        if (isOk) {
            isOk = getArgsForUser();
        }
        if (isOk) {
            isOk = getArgsForPasswd();
        }
        if (isOk) {
            isOk = getArgsForAuthFlag();
        }
        if (isDebugAuth) {
            showDebugAuth();
        }
        return isOk;
    }

    /**
     * ユーザー定義の認証設定ファイル引数を取得し、設定ファイルを読み込みます。
     *
     * @return 設定ファイルの読み込みおよび解析が成功した場合は true、失敗した場合は false
     */
    public boolean loadUserDefFileArgs() {
        boolean isOk = true;
        String[] keys = new String[] {argKeyOfUserConf, argKeyOfUserConf + "name"};
        for (String key : keys) {
            if (namedArgs.containsKey(key)) {
                String confVal = namedArgs.get(key);
                if (confVal != null && !confVal.isEmpty()) {
                    if (key.equals(argKeyOfUserConf)) {
                        authDefFilePath = confVal;
                    } else if (key.equals(argKeyOfUserConf + "name")) {
                        authDefFilePath = MdlFile.combinePath(MdlFile.combinePath(MdlConst.CONF_BASE, "passwd"), confVal + "." + replaceEnvIdKey + ".yml");
                    }
                    isOk = readUserDefFile(confVal);
                    break;
                }
            }
        }
        if (isDebugAuth) {
            showDebugAuth();
        }
        return isOk;
    }

    /**
     * @deprecated {@link #loadUserDefFileArgs()} を使用してください。
     */
    @Deprecated
    public boolean getArgsForUserDefFile() {
        return loadUserDefFileArgs();
    }

    /**
     * 指定されたパスのユーザー定義（認証情報）設定ファイルを読み込み、情報を抽出します。
     *
     * @param filePath 読み込む設定ファイルのパス
     * @return 読み込み・解析が成功した場合は true、失敗した場合は false
     */
    public boolean readUserDefFile(String filePath) {
        final String strMyName = "[ClsCmmnArgs.ReadUserDefFile()]";
        boolean isSuccess = true;
        authDefFilePath = MdlFile.getAbsolutePath(filePath != null ? filePath.trim() : "");
        authDefFilePath = replaceByMap(authDefFilePath);
        if (MdlFile.pathExists(authDefFilePath)) {
            ClsConfigFile configFile = new ClsConfigFile(logger);
            authDefDic.clear();
            configFile.setConfigMap(authDefDic);
            configFile.setVerbose(verbose);
            configFile.setPattern("^(?<KEY>[^#:]+):(?<VALUE>.+)$");
            if (configFile.loadToMap(authDefFilePath) > 0) {
                if (authDefDic.containsKey("username") && !authDefDic.get("username").isEmpty()) {
                    username = authDefDic.get("username");
                }
                splitUserAndDomain();
                if (authDefDic.containsKey("domain") && !authDefDic.get("domain").isEmpty()) {
                    domainName = authDefDic.get("domain");
                }
                if (authDefDic.containsKey("password")) {
                    isDecodePasswd = true;
                    password = authDefDic.get("password");
                }
                String[] cryptoKeys = new String[] {"crypto", "encrypted"};
                for (String key : cryptoKeys) {
                    if (authDefDic.containsKey(key) && "false".equalsIgnoreCase(authDefDic.get(key))) {
                        isDecodePasswd = false;
                    }
                }
                if (authDefDic.containsKey("plaintext") && !authDefDic.get("plaintext").isEmpty()) {
                    password = authDefDic.get("plaintext");
                }
                String[] encKeyKeys = new String[] {"key", "enckey", "secret", "encenckey"};
                for (String key : encKeyKeys) {
                    if (authDefDic.containsKey(key) && !authDefDic.get(key).isEmpty()) {
                        encKey = authDefDic.get(key);
                        if ("encenckey".equals(key)) {
                            isDecodeKey = true;
                        } else {
                            isDecodeKey = false;
                        }
                    }
                }
                if (encKey == null || encKey.isEmpty()) {
                    isDecodeKey = false;
                    isDefaultEncKey = true;
                    encKey = defaultEncKey;
                }
                if (authDefDic.containsKey("keysize") && !authDefDic.get("keysize").isEmpty()) {
                    keySize = MdlUtil.parseInt(authDefDic.get("keysize"), 128);
                }
                if (authDefDic.containsKey("blocksize") && !authDefDic.get("blocksize").isEmpty()) {
                    blockSize = MdlUtil.parseInt(authDefDic.get("blocksize"), 128);
                }
                if (authDefDic.containsKey("iteration") && !authDefDic.get("iteration").isEmpty()) {
                    iterationCount = MdlUtil.parseInt(authDefDic.get("iteration"), 10000);
                }
                String[] hashAlgoKeys = new String[] {"hashalgo", "hashalgorithm"};
                for (String key : hashAlgoKeys) {
                    if (authDefDic.containsKey(key) && !authDefDic.get(key).isEmpty()) {
                        hashAlgorithm = authDefDic.get(key).toUpperCase(Locale.ROOT);
                    }
                }
                String[] envEncKeyKeys = new String[] {"env-enckey", "envenckey", "enckeyenvname"};
                for (String key : envEncKeyKeys) {
                    if (authDefDic.containsKey(key) && !authDefDic.get(key).isEmpty()) {
                        encKeyEnvName = authDefDic.get(key);
                    }
                }
                if (authDefDic.containsKey("debug-auth")) {
                    String debugVal = authDefDic.get("debug-auth").toLowerCase(Locale.ROOT);
                    isDebugAuth = "true".equals(debugVal) || "yes".equals(debugVal) || "y".equals(debugVal);
                }
            }
            if ("MD5".equalsIgnoreCase(hashAlgorithm)) {
                iterationCount = 0;
            } else {
                if (iterationCount < 1) {
                    iterationCount = 1;
                }
            }
        } else {
            writeLine(MdlConst.LVL_DEBUG, strMyName + "INVALID ARGUMENT: -" + argKeyOfUserConf + " " + authDefFilePath + " : NO SUCH A FILE");
            isSuccess = false;
        }
        return isSuccess;
    }

    /**
     * 現在設定されているユーザー名文字列から、ドメイン名とドメインなしユーザー名を分離して保持します。
     */
    public void splitUserAndDomain() {
        if (username == null || username.isEmpty()) {
            username = "WORKGROUP\\Administrator";
        }
        String[] fields = username.split("\\\\");
        if (fields.length > 1) {
            domainName = fields[0];
            usernameWithoutDomain = fields[1];
        } else {
            if (domainName == null || domainName.isEmpty()) {
                domainName = "WORKGROUP";
            }
            usernameWithoutDomain = fields[0];
        }
    }

    /**
     * コマンドライン引数からユーザー名、ドメイン名等を取得します。
     *
     * @return 取得処理が成功した場合は true、失敗した場合は false
     */
    public boolean getArgsForUser() {
        boolean isOk = true;

        for (String key : keyNameOfUsernameList) {
            if (namedArgs.containsKey(key)) {
                String uVal = namedArgs.get(key);
                if (uVal != null && !uVal.isEmpty()) {
                    username = MdlUtil.trimQuotes(uVal.trim());
                    break;
                }
            }
        }
        splitUserAndDomain();

        if (namedArgs.containsKey("domain")) {
            String domainVal = namedArgs.get("domain");
            if (domainVal != null && !domainVal.isEmpty()) {
                domainName = MdlUtil.trimQuotes(domainVal.trim());
            }
        }

        if (namedArgs.containsKey("lhn")) {
            String lhnVal = namedArgs.get("lhn");
            if (lhnVal != null && !lhnVal.isEmpty()) {
                username = lhnVal + "\\" + usernameWithoutDomain;
                domainName = lhnVal;
            } else {
                username = machineName + "\\" + usernameWithoutDomain;
                domainName = machineName;
            }
        }

        if (namedArgs.containsKey("rhn")) {
            String rhnVal = namedArgs.get("rhn");
            if (rhnVal != null && !rhnVal.isEmpty()) {
                username = rhnVal + "\\" + usernameWithoutDomain;
                domainName = rhnVal;
            } else {
                username = host + "\\" + usernameWithoutDomain;
                domainName = host;
            }
        }

        if (isDebugAuth) {
            writeLine(MdlConst.LVL_DEBUG, "[GetArgsForUser] Domainname      : " + domainName);
            writeLine(MdlConst.LVL_DEBUG, "[GetArgsForUser] Username        : " + username);
        }
        return isOk;
    }

    /**
     * パスワード、暗号鍵等の引数を取得・解析し、復号処理を実施します。
     *
     * @return 引数の取得および復号が成功した場合は true、失敗した場合は false
     */
    public boolean getArgsForPasswd() {
        boolean isOk = true;

        for (String key : keyNameOfEncPassList) {
            if (namedArgs.containsKey(key)) {
                String epVal = namedArgs.get(key);
                if (epVal != null && !epVal.isEmpty()) {
                    password = MdlUtil.trimQuotes(epVal.trim());
                    isDecodePasswd = true;
                    break;
                }
            }
        }

        for (String key : keyNameOfPasswordList) {
            if (namedArgs.containsKey(key)) {
                String pVal = namedArgs.get(key);
                if (pVal != null && !pVal.isEmpty()) {
                    password = MdlUtil.trimQuotes(pVal.trim());
                    isDecodePasswd = false;
                    break;
                }
            }
        }

        for (String key : keyNameOfEncEncKeyList) {
            if (namedArgs.containsKey(key)) {
                String ekVal = namedArgs.get(key);
                if (ekVal != null && !ekVal.isEmpty()) {
                    encKey = MdlUtil.trimQuotes(ekVal.trim());
                    isDecodeKey = true;
                    break;
                }
            }
        }

        for (String key : keyNameOfEncKeyList) {
            if (namedArgs.containsKey(key)) {
                String kVal = namedArgs.get(key);
                if (kVal != null && !kVal.isEmpty()) {
                    encKey = MdlUtil.trimQuotes(kVal.trim());
                    isDecodeKey = false;
                    break;
                }
            }
        }

        if (encKey == null || encKey.isEmpty()) {
            isDecodeKey = false;
            isDefaultEncKey = true;
            encKey = defaultEncKey;
        }

        for (String key : keyNameOfEncKeySizeList) {
            if (namedArgs.containsKey(key)) {
                String sVal = namedArgs.get(key);
                if (sVal != null && !sVal.isEmpty()) {
                    int tempInt = MdlUtil.parseInt(sVal, MdlConst.INT_NULL);
                    if (tempInt != MdlConst.INT_NULL) {
                        keySize = (tempInt == 256) ? 256 : 128;
                    }
                    break;
                }
            }
        }

        if (namedArgs.containsKey("blocksize")) {
            int tempInt = MdlUtil.parseInt(namedArgs.get("blocksize"), MdlConst.INT_NULL);
            if (tempInt != MdlConst.INT_NULL) {
                blockSize = tempInt;
            }
        }

        String algoVal = namedArgs.get("hashalgo");
        if (algoVal == null || algoVal.isEmpty()) {
            algoVal = namedArgs.get("hashalgorithm");
        }
        if (algoVal != null && !algoVal.isEmpty()) {
            hashAlgorithm = algoVal.toUpperCase(Locale.ROOT);
        }

        if (namedArgs.containsKey("iteration")) {
            int tempInt = MdlUtil.parseInt(namedArgs.get("iteration"), MdlConst.INT_NULL);
            if (tempInt != MdlConst.INT_NULL) {
                iterationCount = tempInt;
            }
        }

        if ("MD5".equalsIgnoreCase(hashAlgorithm)) {
            iterationCount = 0;
        } else {
            if (iterationCount < 1) {
                iterationCount = 1;
            }
        }

        isOk = decryptKeyAndPassword();

        if (isDebugAuth) {
            showDebugAuth();
        }

        return isOk;
    }

    /**
     * 現在保持されている認証デバッグ情報をログに出力します。
     */
    public void showDebugAuth() {
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] Username        : " + username);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] Password        : " + password);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] EncKey          : " + encKey);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] KeySize         : " + keySize);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] BlockSize       : " + blockSize);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] HashAlgo        : " + hashAlgorithm);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] Iteration       : " + iterationCount);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] IsDecodeKey     : " + isDecodeKey);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] IsDecodePasswd  : " + isDecodePasswd);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] EncKeyEnvName   : " + encKeyEnvName);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] IsSwitchUser    : " + isSwitchUser);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] IsLogon         : " + isLogon);
        writeLine(MdlConst.LVL_DEBUG, "[showDubugAuth] IsLogonAlwaysOk : " + isLogonAlwaysOk);
    }

    /**
     * 認証処理の動作制御フラグをコマンドライン引数から取得します。
     *
     * @return フラグの取得が正常に完了した場合は true、失敗した場合は false
     */
    public boolean getArgsForAuthFlag() {
        boolean isOk = true;

        if (MdlArg.containsKey(namedArgs, "ignore-fail")) {
            isLogonAlwaysOk = true;
        }
        if (MdlArg.containsKey(namedArgs, "su")) {
            isSwitchUser = true;
        }
        if (MdlArg.containsKey(namedArgs, "logon")) {
            isLogon = true;
        }

        if (isDebugAuth) {
            writeLine(MdlConst.LVL_DEBUG, "[GetArgsForAuthFlag] IsSwitchUser    : " + isSwitchUser);
            writeLine(MdlConst.LVL_DEBUG, "[GetArgsForAuthFlag] IsLogon         : " + isLogon);
            writeLine(MdlConst.LVL_DEBUG, "[GetArgsForAuthFlag] IsLogonAlwaysOk : " + isLogonAlwaysOk);
        }

        return isOk;
    }

    /**
     * 保持されている暗号鍵およびパスワードが暗号化されている場合、復号処理を行います。
     *
     * @return 復号処理がすべて成功した場合は true、失敗した場合は false
     */
    public boolean decryptKeyAndPass() {
        boolean isSuccess = true;

        if (isSuccess && isDecodeKey) {
            encKey = decryptPassword(defaultEncKey, encKey, keySize, blockSize, hashAlgorithm, iterationCount);
            if (encKey == null || encKey.isEmpty()) {
                isSuccess = false;
            } else {
                isDecodeKey = false;
            }
        }

        if (isSuccess && isDecodePasswd) {
            password = decryptPassword(encKey, password, keySize, blockSize, hashAlgorithm, iterationCount);
            if (password == null || password.isEmpty()) {
                isSuccess = false;
            } else {
                isDecodePasswd = false;
            }
        }
        return isSuccess;
    }

    /**
     * @deprecated {@link #decryptKeyAndPass()} を使用してください。
     */
    @Deprecated
    public boolean decryptKeyAndPassword() {
        return decryptKeyAndPass();
    }

    /**
     * 指定された暗号鍵とセキュリティパラメータを用いて、暗号化されたパスワードを復号し、平文文字列を返します。
     *
     * @param encKey 復号に使用する暗号鍵
     * @param password 暗号化されたパスワード文字列
     * @param keySize 鍵長
     * @param blockSize ブロックサイズ
     * @param hashAlgorithm ハッシュアルゴリズム
     * @param iterationCount ストレッチング繰返回数
     * @return 復号に成功した場合は平文パスワード、失敗した場合は空文字列
     */
    public String decryptPassword(String encKey, String password, int keySize, int blockSize, String hashAlgorithm, int iterationCount) {
        String output = "";
        if (encKey == null || encKey.isEmpty() || password == null || password.isEmpty()) {
            return "";
        }
        String effectiveHashAlgo = (hashAlgorithm != null && !hashAlgorithm.isEmpty()) ? hashAlgorithm : this.hashAlgorithm;
        ClsCrypt crypt = new ClsCrypt();
        crypt.setKeySize(keySize);
        crypt.setBlockSize(blockSize);
        crypt.setHashAlgorithm(effectiveHashAlgo);
        crypt.setIterationCount(iterationCount);
        crypt.setVerbose(isDebugAuth);
        crypt.setEncKeyEnvName(encKeyEnvName);

        if (isDebugAuth) {
            writeLine(MdlConst.LVL_DEBUG, "[DecodePasswd] crypt.Decrypt(" + encKey + ", " + password + ")");
        }
        if (crypt.decrypt(encKey, password)) {
            output = crypt.getResult();
        } else {
            writeLine(MdlConst.LVL_E, crypt.getErrorMessage());
            writeLine(MdlConst.LVL_E, crypt.getErrorDump());
        }
        return output;
    }

    public String decryptPassword(String encKey, String password, int keySize, int blockSize, String hashAlgorithm) {
        return decryptPassword(encKey, password, keySize, blockSize, hashAlgorithm, 0);
    }

    public String decryptPassword(String encKey, String password, int keySize, int blockSize) {
        return decryptPassword(encKey, password, keySize, blockSize, "MD5", 0);
    }

    public String decryptPassword(String encKey, String password, int keySize) {
        return decryptPassword(encKey, password, keySize, 128, "MD5", 0);
    }

    public String decryptPassword(String encKey, String password) {
        return decryptPassword(encKey, password, 128, 128, "MD5", 0);
    }

    /**
     * 指定された引数キーに対応する絶対パス文字列を取得・正規化します。
     *
     * @param key パスを取得するためのコマンドライン引数キー
     * @param pathType パス判定タイプ
     * @param createDirectory ディレクトリが存在しない場合に自動生成するかどうか
     * @return 取得・正規化されたパス文字列
     */
    public String getPathParam(String key, int pathType, boolean createDirectory) {
        final String strMyName = "[ClsCmmnArgs.GetPathParam()]";
        String rawPath = namedArgs.getOrDefault(key, "");
        String result = MdlFile.trimPathSeparator(MdlFile.getAbsolutePath(rawPath));
        if (MdlFile.getDirectoryPath(result).isEmpty()) {
            result = MdlFile.combinePath(result, ".");
        }
        result = replaceByMap(result);

        String directoryToCheck;
        switch (pathType) {
            case MdlFile.PATH_IS_FILE:
                directoryToCheck = MdlFile.getDirectoryPath(result);
                break;
            case MdlFile.PATH_AUTO_DETECT:
                int pType = MdlFile.getPathType(result);
                if (pType == MdlFile.PATH_IS_DIRECTORY) {
                    directoryToCheck = result;
                } else if (pType == MdlFile.PATH_IS_FILE) {
                    directoryToCheck = MdlFile.getDirectoryPath(result);
                } else {
                    directoryToCheck = "";
                }
                break;
            default:
                directoryToCheck = result;
                break;
        }

        if (directoryToCheck != null && !directoryToCheck.isEmpty() && createDirectory) {
            MdlFile.createDirectory(directoryToCheck);
        }

        if (MdlFile.PATH_IS_DIRECTORY != MdlFile.getPathType(directoryToCheck)) {
            result = "";
        }
        if (result.isEmpty()) {
            writeLine(MdlConst.LVL_E, strMyName + " PLEASE SPECIFY THE ARGUMENT : -" + key + " = " + rawPath);
        }
        return result;
    }

    /**
     * 引数で指定した対象文字列中のキーを、置換用マップに登録された対応値に一括置換して返します。
     *
     * @param target 置換対象の文字列
     * @return 置換処理後の文字列
     */
    public String replaceByMap(String target) {
        final String strMyName = "[ClsCmmnArgs.ReplaceByDictionary()]";
        if (target == null) {
            return "";
        }
        String result = target;
        for (Map.Entry<String, String> pair : replaceDic.entrySet()) {
            result = result.replace(pair.getKey(), pair.getValue());
            if (verbose > 5) {
                writeLine(MdlConst.LVL_DEBUG, strMyName + "[" + pair.getKey() + "⇒" + pair.getValue() + "] " + target + "⇒" + result);
            }
        }
        return result;
    }

    /**
     * @deprecated {@link #replaceByMap(String)} を使用してください。
     */
    @Deprecated
    public String replaceByDictionary(String target) {
        return replaceByMap(target);
    }

    /**
     * ネットワーク共有パス（NET USE）に関連するコマンドライン引数を取得・設定します。
     *
     * @return 処理が正常に終了した場合は true、それ以外は false
     */
    public boolean getNetUseArgs() {
        boolean isOk = true;

        if (namedArgs.containsKey("mount")) {
            String mountVal = namedArgs.get("mount");
            if (mountVal != null && !mountVal.isEmpty()) {
                netSharePath = MdlFile.trimPathSeparator(mountVal);
            }
        }

        if (namedArgs.containsKey("drive")) {
            String driveVal = namedArgs.get("drive");
            if (driveVal != null && !driveVal.isEmpty()) {
                driveName = driveVal.replace(":", "");
            }
        }

        if (netSharePath == null || netSharePath.isEmpty()) {
            isMount = false;
            isUmount = false;
        } else {
            isMount = !namedArgs.containsKey("no-mount");
            isUmount = !namedArgs.containsKey("no-umount");
        }

        String okNoVal = namedArgs.get("mount-ok-no");
        if (okNoVal == null || okNoVal.isEmpty()) {
            okNoVal = namedArgs.get("logon-ok-no");
        }
        if (okNoVal != null && !okNoVal.isEmpty()) {
            List<String> netUseOkErrNoListStr = MdlUtil.parseCsvToList(null, okNoVal);
            for (String element : netUseOkErrNoListStr) {
                String strVal = element.trim();
                if (!strVal.isEmpty()) {
                    int intVal = MdlUtil.parseInt(strVal, MdlConst.INT_NULL);
                    if (intVal != MdlConst.INT_NULL) {
                        netUseOkErrNoList.add(intVal);
                    }
                }
            }
        }

        return isOk;
    }

    /**
     * ファイル・ディレクトリの絞り込み/除外フィルタのコマンドライン引数を解析し、各種ルールリストに登録します。
     *
     * @return 取得・設定が成功した場合は true、失敗した場合は false
     */
    public boolean getFilterLists() {
        boolean isSuccess = true;
        String temp = "";

        if (isSuccess) {
            temp = namedArgs.getOrDefault("if", "");
            if (!temp.isEmpty()) {
                if (!replaceDic.isEmpty()) {
                    temp = replaceByDictionary(temp);
                }
                incFilesList = MdlUtil.parseCsvToList(incFilesList, temp, splitPattern, verbose, true);
            }
        }

        if (isSuccess) {
            temp = namedArgs.getOrDefault("idf", "");
            if (!temp.isEmpty()) {
                isRegIncBasename = false;
            }
            if (temp.isEmpty()) {
                temp = namedArgs.getOrDefault("id", "");
            }
            if (temp.isEmpty()) {
                temp = namedArgs.getOrDefault("idb", "");
            }
            if (!temp.isEmpty()) {
                if (!replaceDic.isEmpty()) {
                    temp = replaceByDictionary(temp);
                }
                incDirsList = MdlUtil.parseCsvToList(incDirsList, temp, splitPattern, verbose, true);
            }
        }

        if (isSuccess) {
            temp = namedArgs.getOrDefault("xf", "");
            if (!temp.isEmpty()) {
                if (!replaceDic.isEmpty()) {
                    temp = replaceByDictionary(temp);
                }
                excFilesList = MdlUtil.parseCsvToList(excFilesList, temp, splitPattern, verbose, true);
            }
        }

        if (isSuccess) {
            temp = namedArgs.getOrDefault("xdf", "");
            if (!temp.isEmpty()) {
                isRegExcBasename = false;
            }
            if (temp.isEmpty()) {
                temp = namedArgs.getOrDefault("xd", "");
            }
            if (temp.isEmpty()) {
                temp = namedArgs.getOrDefault("xdb", "");
            }
            if (!temp.isEmpty()) {
                if (!replaceDic.isEmpty()) {
                    temp = replaceByDictionary(temp);
                }
                excDirsList = MdlUtil.parseCsvToList(excDirsList, temp, splitPattern, verbose, true);
            }
        }

        if (namedArgs.containsKey("idorxd")) {
            isDirFilterOr = true;
        }
        if (namedArgs.containsKey("no-id-rec")) {
            isIncHitRecursive = false;
        }
        if (namedArgs.containsKey("no-xd-rec")) {
            isExcHitRecursive = false;
        }
        return isSuccess;
    }

    /**
     * 指定されたエラーレベルとメッセージをロガー経由で出力します。
     *
     * @param level ログメッセージのエラーレベル
     * @param message 出力するメッセージ文字列
     */
    public void writeLine(int level, String message) {
        try {
            if (logger != null) {
                logger.writeLine(level, message);
            } else {
                System.out.println(message);
            }
        } catch (Exception e) {
            System.out.println(message);
        }
    }

    /**
     * 共通コマンドライン引数の利用方法（Usage）を出力します。
     */
    public void showUsage() {
        writeLine(MdlConst.LVL_NONE, "");
        writeLine(MdlConst.LVL_NONE, "CmmnParams Option：");
        writeLine(MdlConst.LVL_NONE, "   -arg-def path       ：引数定義INIファイルパス     （現在値=" + argDefFilePath + "）");
        writeLine(MdlConst.LVL_NONE, "   -h|-help|-?         ：Usage表示                   （現在値=" + isUsage + "）");
        writeLine(MdlConst.LVL_NONE, "   -h hostname         ：ホスト名                    （現在値=" + host + "）");
        writeLine(MdlConst.LVL_NONE, "   -force              ：強制実行フラグ              （現在値=" + isForce + "）");
        writeLine(MdlConst.LVL_NONE, "CmmnParams Output Option：");
        writeLine(MdlConst.LVL_NONE, "   -v |-vv|-brief num  ：冗長表示                    （現在値=" + verbose + "）");
        writeLine(MdlConst.LVL_NONE, "   -diff               ：差分表示フラグ              （現在値=" + isDiff + "）");
        writeLine(MdlConst.LVL_NONE, "   -console mode       ：メッセージ表示 off|stdout|stderr");
        writeLine(MdlConst.LVL_NONE, "   -stacktrace         ：例外時スタックトレース表示  （現在値=" + isStackTrace + "）");
        writeLine(MdlConst.LVL_NONE, "   -stdenc encode      ：標準出力エンコード          （現在値=" + (logger != null ? logger.getValueByKey(ClsLogger.CONSOLE_ENCODING, "") : "") + "）");
        writeLine(MdlConst.LVL_NONE, "CmmnParams Job Option：");
        writeLine(MdlConst.LVL_NONE, "   -ajsjobname name    ：AJSJOBNAME                  （現在値=" + (jp1 != null ? jp1.getJobName() : "") + "）");
        writeLine(MdlConst.LVL_NONE, "   -nojp1              ：AJSJOBNAME参照フラグ        （現在値=" + isAjsJob + "）");
        writeLine(MdlConst.LVL_NONE, "   -envajs str         ：AJSJOBNAME検索プレフィックス（現在値=" + (jp1 != null ? jp1.getPrefix() : "") + "）");
        writeLine(MdlConst.LVL_NONE, "   -envvar str         ：環境変数検索プレフィックス  （現在値=" + envPrefix + "）");
        writeLine(MdlConst.LVL_NONE, "   -envenvid str       ：環境種別キー環境変数名      （現在値=" + envIdKey + "）");
        writeLine(MdlConst.LVL_NONE, "CmmnParams Replace Option：");
        writeLine(MdlConst.LVL_NONE, "   -replace a:b        ：文字列置換CSVリスト         （現在値=" + namedArgs.getOrDefault("replace", "") + "）");
        writeLine(MdlConst.LVL_NONE, "   -morereplace b:c    ：文字列再値置換CSVリスト     （現在値=" + namedArgs.getOrDefault("morereplace", "") + "）");
        writeLine(MdlConst.LVL_NONE, "   -reservereplace     ：文字列予約語再値置換        （現在値=" + namedArgs.getOrDefault("reservereplace", "") + "）");
        writeLine(MdlConst.LVL_NONE, "   -splitby pattern    ：文字列分割デリミタパターン  （現在値=" + splitPattern + "）");
        writeLine(MdlConst.LVL_NONE, "   -split-kv-by pattern：key[分割デリミタパターン]Val（現在値=" + keyValDelimiter + "）");
        writeLine(MdlConst.LVL_NONE, "CmmnParams Log Option：");
        writeLine(MdlConst.LVL_NONE, "   -ldir path          ：ログ出力先ディレクトリパス（日付付ファイル名で出力）（現在値=" + (logger != null ? logger.getValueByKey(ClsLogger.DIR, "") : "") + "）");
        writeLine(MdlConst.LVL_NONE, "   -log  path          ：ログ出力ファイルパス（-ldirより優先）               （現在値=" + (logger != null ? logger.getValueByKey(ClsLogger.PATH, "") : "") + "）");
        writeLine(MdlConst.LVL_NONE, "   -logenc encode      ：ログファイルエンコード      （現在値=" + (logger != null ? logger.getValueByKey(ClsLogger.FILE_ENCODING, "") : "") + "）");
        writeLine(MdlConst.LVL_NONE, "CmmnParams Command Option：");
        writeLine(MdlConst.LVL_NONE, "   -retry num          ：リトライ回数                （現在値=" + retryMax + "）");
        writeLine(MdlConst.LVL_NONE, "   -sleep sec          ：リトライ間隔（秒）          （現在値=" + retrySleep + "）");
        writeLine(MdlConst.LVL_NONE, "   -timeout sec        ：タイムアウト（秒）          （現在値=" + timeout + "）");
        writeLine(MdlConst.LVL_NONE, "CmmnParams Auth Option：");
        writeLine(MdlConst.LVL_NONE, "   -auth-conf-key key  ：アカウント設定ファイルパス指定引数名 （現在値=" + argKeyOfUserConf + "）");
        writeLine(MdlConst.LVL_NONE, "   -domain str         ：ドメイン名                  （現在値=" + domainName + "）");
        writeLine(MdlConst.LVL_NONE, "   -lhn <name>         ：ユーザ名 => 指定値|自ホスト名\\ユーザ名");
        writeLine(MdlConst.LVL_NONE, "   -rhn <name>         ：ユーザ名 => 指定値|接続先ホスト名\\ユーザ名");
        writeLine(MdlConst.LVL_NONE, "   -u|-user|-username n：ユーザ名                    （現在値=" + username + "）");
        writeLine(MdlConst.LVL_NONE, "   -p|-pass|-password p：パスワード                  （現在値=" + password + "）");
        writeLine(MdlConst.LVL_NONE, "   -ep|-encpass ep     ：暗号化パスワード");
        writeLine(MdlConst.LVL_NONE, "   -k|-key|-enckey key ：暗号鍵                      （現在値=" + (verbose > 4 ? encKey : "***************") + "）");
        writeLine(MdlConst.LVL_NONE, "   -ek|-encenckey ek   ：暗号化暗号鍵");
        writeLine(MdlConst.LVL_NONE, "   -s|-size|-keysize n ：鍵長                        （現在値=" + keySize + "）");
        writeLine(MdlConst.LVL_NONE, "   -blocksize num      ：ブロック長                  （現在値=" + blockSize + "）");
        writeLine(MdlConst.LVL_NONE, "   -hashalgo algo      ：MD5|SHA1|SHA256|SHA512      （現在値=" + hashAlgorithm + "）");
        writeLine(MdlConst.LVL_NONE, "   -iteration num      ：繰返回数                    （現在値=" + iterationCount + "）");
        writeLine(MdlConst.LVL_NONE, "   -ignore-fail        ：認証エラー無視フラグ        （現在値=" + isLogonAlwaysOk + "）");
        writeLine(MdlConst.LVL_NONE, "   -su                 ：ユーザー認証実行フラグ      （現在値=" + isSwitchUser + "）");
        writeLine(MdlConst.LVL_NONE, "   -logon              ：ユーザー認証実行フラグ      （現在値=" + isLogon + "）");
        writeLine(MdlConst.LVL_NONE, "   -env-enckey name    ：暗号鍵格納環境変数名        （現在値=" + encKeyEnvName + "）");
        writeLine(MdlConst.LVL_NONE, "CmmnParams NetUse Option：");
        writeLine(MdlConst.LVL_NONE, "   -mount path         ：マウントフラグ              （現在値=" + isMount + "）");
        writeLine(MdlConst.LVL_NONE, "   -drive              ：ドライブ名                  （現在値=" + (!isMount) + "）");
        writeLine(MdlConst.LVL_NONE, "   -no-mount           ：非マウントフラグ            （現在値=" + (!isMount) + "）");
        writeLine(MdlConst.LVL_NONE, "   -mount-ok-no csv    ：正常と見なすエラー番号リスト（現在値=" + (!isMount) + "）");
        writeLine(MdlConst.LVL_NONE, "CmmnParams Debug Option：");
        writeLine(MdlConst.LVL_NONE, "   -dumpargs           ：引数の表示");
        writeLine(MdlConst.LVL_NONE, "   -dumpreplace        ：置換リストの表示");
        writeLine(MdlConst.LVL_NONE, "   -debug-auth         ：認証DEBUGフラグ             （現在値=" + isDebugAuth + "）");
        writeLine(MdlConst.LVL_NONE, "   -hh|-??             ：CmmnParams Usage");
        writeLine(MdlConst.LVL_NONE, "");
    }
}
