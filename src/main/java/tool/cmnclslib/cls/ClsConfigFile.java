package tool.cmnclslib.cls;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.nio.charset.Charset;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import tool.cmnclslib.ifc.ICmnLogger;
import tool.cmnclslib.mdl.MdlConst;
import tool.cmnclslib.mdl.MdlFile;
import tool.cmnclslib.mdl.MdlUtil;

/**
 * 設定ファイル（Key=Value形式および行リスト形式）を読み込み、解析・保持するためのクラスです。
 */
public class ClsConfigFile {

    private static final Pattern COMMENT_REGEX = Pattern.compile("^\\s*#");
    private static final Pattern EMPTY_LINE_REGEX = Pattern.compile("^\\s*$");
    private static final Pattern INLINE_COMMENT_REGEX = Pattern.compile("#.+");

    private ICmnLogger logger;
    private Map<String, String> configMap = new LinkedHashMap<>();
    private Map<String, List<String>> listMap = new LinkedHashMap<>();
    private List<String> duplicateKeys = new ArrayList<>();
    private List<String> configList = new ArrayList<>();
    private int verbose = 0;
    private String pattern = "^(?<KEY>[^#=]+)=(?<VALUE>.+)$";
    private Pattern cachedPatternRegex;
    private Charset encoding = Charset.defaultCharset();
    private boolean isSkipComment = true;
    private boolean detectEncoding = true;

    /**
     * ClsConfigFile クラスの新しいインスタンスを初期化します。
     *
     * @param logger ログ出力に使用するロガーオブジェクト
     */
    public ClsConfigFile(ICmnLogger logger) {
        this.logger = logger;
        clear();
    }

    public Map<String, String> getConfigMap() {
        return configMap;
    }

    public void setConfigMap(Map<String, String> configMap) {
        this.configMap = configMap != null ? configMap : new LinkedHashMap<>();
    }

    /**
     * @deprecated {@link #getConfigMap()} を使用してください。
     */
    @Deprecated
    public Map<String, String> getConfigDictionary() {
        return getConfigMap();
    }

    /**
     * @deprecated {@link #setConfigMap(Map)} を使用してください。
     */
    @Deprecated
    public void setConfigDictionary(Map<String, String> configDictionary) {
        setConfigMap(configDictionary);
    }

    public Map<String, List<String>> getListMap() {
        return listMap;
    }

    public void setListMap(Map<String, List<String>> listMap) {
        this.listMap = listMap != null ? listMap : new LinkedHashMap<>();
    }

    /**
     * @deprecated {@link #getListMap()} を使用してください。
     */
    @Deprecated
    public Map<String, List<String>> getListDictionary() {
        return getListMap();
    }

    /**
     * @deprecated {@link #setListMap(Map)} を使用してください。
     */
    @Deprecated
    public void setListDictionary(Map<String, List<String>> listDictionary) {
        setListMap(listDictionary);
    }

    public List<String> getConfigList() {
        return configList;
    }

    public void setConfigList(List<String> configList) {
        this.configList = configList != null ? configList : new ArrayList<>();
    }

    public List<String> getDuplicateKeys() {
        return duplicateKeys;
    }

    public void setDuplicateKeys(List<String> duplicateKeys) {
        this.duplicateKeys = duplicateKeys != null ? duplicateKeys : new ArrayList<>();
    }

    public int getVerbose() {
        return verbose;
    }

    public void setVerbose(int verbose) {
        this.verbose = verbose;
    }

    public String getPattern() {
        return pattern;
    }

    public void setPattern(String pattern) {
        this.pattern = pattern;
        this.cachedPatternRegex = null;
    }

    public Charset getEncoding() {
        return encoding;
    }

    public void setEncoding(Charset encoding) {
        this.encoding = encoding;
    }

    public boolean isSkipComment() {
        return isSkipComment;
    }

    public void setSkipComment(boolean skipComment) {
        this.isSkipComment = skipComment;
    }

    public boolean isDetectEncoding() {
        return detectEncoding;
    }

    public void setDetectEncoding(boolean detectEncoding) {
        this.detectEncoding = detectEncoding;
    }

    /**
     * 保持している設定情報（マップ、リスト等）をすべてクリアします。
     */
    public void clear() {
        configMap.clear();
        listMap.clear();
        configList.clear();
        duplicateKeys.clear();
    }

    /**
     * 指定された設定ファイルを読み込み、設定内容を Key-Value のマップに格納します。
     *
     * @param filePath 読み込み対象の設定ファイルパス
     * @return 正常に読み込んでマップに格納された設定項目数。エラー発生時は -1
     */
    public int loadToMap(String filePath) {
        final String methodName = "[ClsConfigFile.LoadToMap()]";
        if (verbose > 3) {
            writeLog(MdlConst.LVL_DEBUG, methodName);
            writeLog(MdlConst.LVL_DEBUG, methodName + "filePath       = " + filePath);
            writeLog(MdlConst.LVL_DEBUG, methodName + "verbose       = " + verbose);
            writeLog(MdlConst.LVL_DEBUG, methodName + "isSkipComment = " + isSkipComment);
            writeLog(MdlConst.LVL_DEBUG, methodName);
        }

        if (configMap == null) {
            writeLog(MdlConst.LVL_E, methodName + " configMap is null");
            return -1;
        }

        try {
            if (detectEncoding) {
                encoding = MdlFile.detectFileEncoding(filePath);
            }

            if (cachedPatternRegex == null) {
                cachedPatternRegex = Pattern.compile(pattern);
            }

            java.nio.file.Path path = java.nio.file.Paths.get(filePath);
            Charset enc = encoding != null ? encoding : Charset.defaultCharset();
            try (BufferedReader reader = java.nio.file.Files.newBufferedReader(path, enc)) {
                String line;
                while ((line = reader.readLine()) != null) {
                    String buffer;
                    String lineType = "NORMAL LINE";
                    boolean isContinue = false;

                    // コメント行
                    if (isSkipComment) {
                        if (COMMENT_REGEX.matcher(line).find()) {
                            isContinue = true;
                            lineType = "SKIP : COMMENT LINE";
                        }
                        buffer = INLINE_COMMENT_REGEX.matcher(line).replaceAll("").trim();
                    } else {
                        buffer = line.trim();
                    }

                    // 空行
                    if (EMPTY_LINE_REGEX.matcher(buffer).find()) {
                        isContinue = true;
                        lineType = "SKIP : EMPTY LINE";
                    }

                    if (verbose > 5) {
                        writeLog(MdlConst.LVL_DEBUG, methodName + "CURRENT LINE (" + lineType + ") : " + buffer);
                    }

                    if (isContinue) {
                        continue;
                    }

                    Matcher match = cachedPatternRegex.matcher(buffer);
                    if (match.find()) {
                        String key = MdlUtil.trimQuotes(match.group("KEY"));
                        String value = MdlUtil.trimQuotes(match.group("VALUE"));

                        if (verbose > 3) {
                            writeLog(MdlConst.LVL_DEBUG, methodName + "[" + filePath + "] configMap[" + key + "] = " + value);
                        }

                        if (!duplicateKeys.isEmpty() && duplicateKeys.contains(key)) {
                            listMap.computeIfAbsent(key, k -> new ArrayList<>()).add(value);
                        }

                        configMap.put(key, value);
                    }
                }
            }
            return configMap.size();
        } catch (Exception ex) {
            writeLog(MdlConst.LVL_E, methodName + " Exception : " + ex.getMessage());
            return -1;
        }
    }

    /**
     * @deprecated {@link #loadToMap(String)} を使用してください。
     */
    @Deprecated
    public int loadToDictionary(String filePath) {
        return loadToMap(filePath);
    }

    /**
     * 指定された設定ファイルを読み込み、各行の文字列をリストに格納します。
     *
     * @param filePath 読み込み対象の設定ファイルパス
     * @param unique 重複行を除外してユニークな行のみ保持する場合は true
     * @return 正常に読み込んでリストに格納された行数。エラー発生時は -1
     */
    public int loadToList(String filePath, boolean unique) {
        final String methodName = "[ClsConfigFile.LoadToList()]";
        if (verbose > 3) {
            writeLog(MdlConst.LVL_DEBUG, methodName);
            writeLog(MdlConst.LVL_DEBUG, methodName + "filePath              = " + filePath);
            writeLog(MdlConst.LVL_DEBUG, methodName + "verbose              = " + verbose);
            writeLog(MdlConst.LVL_DEBUG, methodName + "isSkipComment = " + isSkipComment);
            writeLog(MdlConst.LVL_DEBUG, methodName);
        }

        if (configList == null) {
            writeLog(MdlConst.LVL_E, methodName + " configList is null");
            return -1;
        }

        try {
            if (detectEncoding) {
                encoding = MdlFile.detectFileEncoding(filePath);
            }

            java.nio.file.Path path = java.nio.file.Paths.get(filePath);
            Charset enc = encoding != null ? encoding : Charset.defaultCharset();
            try (BufferedReader reader = java.nio.file.Files.newBufferedReader(path, enc)) {
                String line;
                while ((line = reader.readLine()) != null) {
                    String buffer;
                    String lineType = "NORMAL LINE";
                    boolean isContinue = false;

                    if (isSkipComment) {
                        if (COMMENT_REGEX.matcher(line).find()) {
                            isContinue = true;
                            lineType = "SKIP : COMMENT LINE";
                        }
                        buffer = INLINE_COMMENT_REGEX.matcher(line).replaceAll("").trim();
                    } else {
                        buffer = line.trim();
                    }

                    if (EMPTY_LINE_REGEX.matcher(buffer).find()) {
                        isContinue = true;
                        lineType = "SKIP : EMPTY LINE";
                    }

                    if (verbose > 5) {
                        writeLog(MdlConst.LVL_DEBUG, methodName + "CURRENT LINE (" + lineType + ") : " + buffer);
                    }

                    if (isContinue) {
                        continue;
                    }

                    if (!unique || !configList.contains(buffer)) {
                        configList.add(buffer);
                    }
                }
            }
            return configList.size();
        } catch (Exception ex) {
            writeLog(MdlConst.LVL_E, methodName + " Exception : " + ex.getMessage());
            return -1;
        }
    }

    /**
     * 指定されたレベルとメッセージでログを出力します。
     *
     * @param level ログレベル
     * @param message 出力するメッセージ文字列
     */
    public void writeLog(int level, String message) {
        if (logger != null) {
            logger.writeLine(level, message);
        } else {
            System.out.println(message);
        }
    }
}
