package tool.cmnclslib.cls;

import java.io.BufferedWriter;
import java.io.FileOutputStream;
import java.io.OutputStreamWriter;
import java.nio.charset.Charset;
import tool.cmnclslib.ifc.ICmnLogger;
import tool.cmnclslib.mdl.MdlApp;
import tool.cmnclslib.mdl.MdlConst;
import tool.cmnclslib.mdl.MdlDate;
import tool.cmnclslib.mdl.MdlFile;
import tool.cmnclslib.mdl.MdlLog;
import tool.cmnclslib.mdl.MdlUtil;

/**
 * コンソールおよびファイルへのログ出力を管理するロガークラスです。
 */
public class ClsLogger implements ICmnLogger {

    public static final String IS_STDOUT = "isStdOut";
    public static final String IS_STDERR = "isStdErr";
    public static final String IS_CONSOLE = "isConsole";
    public static final String IS_FILE = "isFile";
    public static final String IS_APPEND = "isAppend";
    public static final String IS_FLUSH = "isFlush";
    public static final String IS_TRIM_END = "isTrimEnd";
    public static final String IS_TRIM_CONSOLE = "isTrimConsole";
    public static final String IS_CONSOLE_ENCODING = "isConsoleEncoding";
    public static final String DIR = "dir";
    public static final String PATH = "path";
    public static final String BASENAME = "baseName";
    public static final String FILENAME = "fileName";
    public static final String CONSOLE_ENCODING = "consoleEncoding";
    public static final String FILE_ENCODING = "fileEncoding";

    private final Object fileLock = new Object();
    private volatile boolean isStdErr = false;
    private volatile boolean isStdOut = false;
    private volatile boolean isConsole = true;
    private volatile boolean isFile = false;
    private volatile boolean isAppend = true;
    private volatile boolean isFlush = false;
    private volatile boolean isTrimEnd = true;
    private volatile boolean isTrimConsole = true;
    private volatile boolean isConsoleEncoding = false;
    private volatile String dir = "";
    private volatile String path = "";
    private volatile String baseName = "";
    private volatile String fileName = "";
    private volatile Charset consoleEncoding = Charset.defaultCharset();
    private volatile Charset fileEncoding = Charset.defaultCharset();

    /**
     * ClsLogger クラスの新しいインスタンスを初期化します。
     */
    public ClsLogger() {
    }

    /**
     * ログ出力設定のプロパティ値をキー指定で設定します。
     *
     * @param key プロパティキー
     * @param val 設定する値の文字列
     */
    @Override
    public void setValueByKey(String key, String val) {
        if (key == null) {
            return;
        }
        switch (key) {
            case IS_STDOUT:
                isStdOut = MdlUtil.isTrue(val, false);
                break;
            case IS_STDERR:
                isStdErr = MdlUtil.isTrue(val, false);
                break;
            case IS_CONSOLE:
                isConsole = MdlUtil.isTrue(val, true);
                break;
            case IS_FILE:
                isFile = MdlUtil.isTrue(val, false);
                break;
            case IS_APPEND:
                isAppend = MdlUtil.isTrue(val, true);
                break;
            case IS_FLUSH:
                isFlush = MdlUtil.isTrue(val, false);
                break;
            case IS_TRIM_END:
                isTrimEnd = MdlUtil.isTrue(val, true);
                break;
            case IS_TRIM_CONSOLE:
                isTrimConsole = MdlUtil.isTrue(val, true);
                break;
            case IS_CONSOLE_ENCODING:
                isConsoleEncoding = MdlUtil.isTrue(val, false);
                break;
            case DIR:
                dir = val != null ? val : "";
                break;
            case PATH:
                path = val != null ? val : "";
                break;
            case BASENAME:
                baseName = val != null ? val : "";
                break;
            case FILENAME:
                fileName = val != null ? val : "";
                break;
            case CONSOLE_ENCODING:
                consoleEncoding = MdlUtil.getEncoding(val);
                break;
            case FILE_ENCODING:
                fileEncoding = MdlUtil.getEncoding(val);
                break;
            default:
                break;
        }
    }

    /**
     * キーに対応するプロパティ値（文字列）を取得します。
     *
     * @param key プロパティキー
     * @param defaultValue デフォルト値
     * @return プロパティの文字列値、またはデフォルト値
     */
    @Override
    public String getValueByKey(String key, String defaultValue) {
        if (key == null) {
            return defaultValue;
        }
        String value = defaultValue;
        switch (key) {
            case IS_STDOUT:
            case IS_STDERR:
            case IS_CONSOLE:
            case IS_FILE:
            case IS_APPEND:
            case IS_FLUSH:
            case IS_TRIM_END:
            case IS_TRIM_CONSOLE:
            case IS_CONSOLE_ENCODING:
                value = String.valueOf(getValueByKey(key, MdlUtil.isTrue(defaultValue, false)));
                break;
            case DIR:
                value = dir;
                break;
            case PATH:
                value = path;
                break;
            case BASENAME:
                value = baseName;
                break;
            case FILENAME:
                value = fileName;
                break;
            case CONSOLE_ENCODING:
                value = MdlUtil.getEncodingName(consoleEncoding);
                break;
            case FILE_ENCODING:
                value = MdlUtil.getEncodingName(fileEncoding);
                break;
            default:
                break;
        }
        return value;
    }

    /**
     * キーに対応するプロパティ値（真偽値）を取得します。
     *
     * @param key プロパティキー
     * @param defaultValue デフォルト値
     * @return プロパティの真偽値、またはデフォルト値
     */
    @Override
    public boolean getValueByKey(String key, boolean defaultValue) {
        if (key == null) {
            return defaultValue;
        }
        boolean value = defaultValue;
        switch (key) {
            case IS_STDOUT:
                value = isStdOut;
                break;
            case IS_STDERR:
                value = isStdErr;
                break;
            case IS_CONSOLE:
                value = isConsole;
                break;
            case IS_FILE:
                value = isFile;
                break;
            case IS_APPEND:
                value = isAppend;
                break;
            case IS_FLUSH:
                value = isFlush;
                break;
            case IS_TRIM_END:
                value = isTrimEnd;
                break;
            case IS_TRIM_CONSOLE:
                value = isTrimConsole;
                break;
            case IS_CONSOLE_ENCODING:
                value = isConsoleEncoding;
                break;
            default:
                break;
        }
        return value;
    }

    /**
     * 指定されたログレベルでログメッセージを出力（コンソールおよびファイル）します。
     *
     * @param level ログレベル
     * @param message 出力メッセージ
     */
    @Override
    public void writeLine(int level, String message) {
        boolean useStdErr = isStdErr;
        switch (level) {
            case MdlConst.LVL_W:
            case MdlConst.LVL_E:
            case MdlConst.LVL_F:
                useStdErr = true;
                break;
            default:
                break;
        }

        String outputLine;
        switch (level) {
            case MdlConst.LVL_DEBUG:
            case MdlConst.LVL_I:
            case MdlConst.LVL_W:
            case MdlConst.LVL_E:
                outputLine = MdlDate.getFormattedDate("yyyy/MM/dd HH:mm:ss") + " " + MdlLog.getLogLevelPrefix(level) + message;
                break;
            default:
                outputLine = MdlLog.getLogLevelPrefix(level) + message;
                break;
        }

        String trimmedLine = isTrimEnd ? MdlUtil.trimEnd(outputLine) : outputLine;

        if (isConsole) {
            writeToConsole(!isStdOut && useStdErr, isTrimConsole ? trimmedLine : outputLine);
        }

        writeToFile(trimmedLine);
    }

    private void writeToConsole(boolean toStdErr, String line) {
        try {
            if (toStdErr) {
                System.err.println(line);
            } else {
                System.out.println(line);
            }
        } catch (Exception e) {
            // 無視
        }
    }

    private void writeToFile(String line) {
        if (!isFile) {
            return;
        }
        String currentPath = "";
        if (path == null || path.isEmpty()) {
            if (fileName == null || fileName.isEmpty()) {
                if (baseName == null || baseName.isEmpty()) {
                    baseName = MdlApp.getAppNameWithHost();
                }
                currentPath = MdlFile.combinePath(dir, MdlLog.generateLogFileName(baseName));
            } else {
                currentPath = MdlFile.combinePath(dir, fileName);
            }
        } else {
            currentPath = path;
        }

        MdlFile.createDirectory(MdlFile.getDirectoryPath(currentPath));
        synchronized (fileLock) {
            try {
                java.nio.file.Path targetPath = java.nio.file.Paths.get(currentPath);
                Charset enc = fileEncoding != null ? fileEncoding : Charset.defaultCharset();
                java.nio.file.OpenOption[] options = isAppend
                        ? new java.nio.file.OpenOption[] {java.nio.file.StandardOpenOption.CREATE, java.nio.file.StandardOpenOption.APPEND}
                        : new java.nio.file.OpenOption[] {java.nio.file.StandardOpenOption.CREATE, java.nio.file.StandardOpenOption.TRUNCATE_EXISTING};
                try (BufferedWriter bw = java.nio.file.Files.newBufferedWriter(targetPath, enc, options)) {
                    bw.write(line);
                    bw.newLine();
                    if (isFlush) {
                        bw.flush();
                    }
                }
            } catch (Exception ex) {
                isFile = false;
                writeToConsole(true, "ERROR [Logger.WriteToFile()] EXCEPTION : " + ex.getMessage());
            } finally {
                isAppend = true;
            }
        }
    }
}
