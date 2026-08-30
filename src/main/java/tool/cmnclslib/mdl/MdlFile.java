package tool.cmnclslib.mdl;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.RandomAccessFile;
import java.nio.channels.FileLock;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.attribute.BasicFileAttributes;
import java.nio.file.attribute.FileTime;
import java.security.MessageDigest;
import java.time.Instant;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Comparator;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * ファイルおよびディレクトリに関する操作や判定機能を提供するユーティリティクラスです。
 */
public final class MdlFile {

    // パス種別
    public static final int PATH_IS_NULL = -1;
    public static final int PATH_NOT_FOUND = 0;
    public static final int PATH_IS_DIRECTORY = 1;
    public static final int PATH_IS_FILE = 2;
    public static final int PATH_AUTO_DETECT = 9;

    // ディレクトリ作成ステータス
    public static final int OK_MKDIR_CREATE = 0;
    public static final int OK_MKDIR_ALREADY_EXIST = 1;
    public static final int OK_MKDIR_HANTEI = 9;
    public static final int NG_MKDIR = 11;
    public static final int NG_MKDIR_WRONG_ARG = 12;
    public static final int NG_MKDIR_FILE_EXIST = 13;

    // ファイル作成ステータス
    public static final int OK_TOUCH_CREATE = 0;
    public static final int OK_TOUCH_ALREADY_EXIST = 1;
    public static final int OK_TOUCH_HANTEI = 9;
    public static final int NG_TOUCH = 11;
    public static final int NG_TOUCH_WRONG_ARG = 12;
    public static final int NG_TOUCH_DIR_EXIST = 13;

    // ソート種別
    public static final int SORT_BY_NONE = 0;
    public static final int SORT_BY_NAME = 1;
    public static final int SORT_BY_CTIME = 2;
    public static final int SORT_BY_MTIME = 3;

    private static final Pattern KEY_VAL_REGEX = Pattern.compile("^\\s*(?<KEY>[^#=]*)\\s*=\\s*(?<VAL>.*)\\s*$");

    private MdlFile() {
        // インスタンス化防止
    }

    /**
     * 指定されたファイルパスから親ディレクトリのパスを取得します。
     *
     * @param filePath 対象のファイルパス
     * @return 親ディレクトリのパス。取得できない場合は空文字列
     */
    public static String getDirectoryPath(String filePath) {
        if (filePath == null || filePath.isEmpty()) {
            return "";
        }
        File file = new File(filePath);
        String parent = file.getParent();
        return parent != null ? parent : "";
    }

    /**
     * 指定されたファイルパスから拡張子を除いたファイル名を取得します。
     *
     * @param filePath 対象のファイルパス
     * @return 拡張子を除いたファイル名。指定パスが空の場合は空文字列
     */
    public static String getBaseName(String filePath) {
        if (filePath == null || filePath.isEmpty()) {
            return "";
        }
        String fileName = getFileName(filePath);
        int dotIndex = fileName.lastIndexOf('.');
        return dotIndex > 0 ? fileName.substring(0, dotIndex) : fileName;
    }

    /**
     * @deprecated {@link #getBaseName(String)} を使用してください。
     */
    @Deprecated
    public static String getFileNameWithoutExtension(String filePath) {
        return getBaseName(filePath);
    }

    /**
     * 指定されたファイルパスからファイル名（拡張子含む）を取得します。
     *
     * @param filePath 対象のファイルパス
     * @return ファイル名。指定パスが空の場合は空文字列
     */
    public static String getFileName(String filePath) {
        if (filePath == null || filePath.isEmpty()) {
            return "";
        }
        return new File(filePath).getName();
    }

    /**
     * 指定されたファイルパスから先頭のドットを除いた拡張子を取得します。
     *
     * @param filePath 対象のファイルパス
     * @return 拡張子（例: "txt"）。指定パスが空または拡張子がない場合は空文字列
     */
    public static String getFileExtension(String filePath) {
        if (filePath == null || filePath.isEmpty()) {
            return "";
        }
        String fileName = getFileName(filePath);
        int dotIndex = fileName.lastIndexOf('.');
        return (dotIndex >= 0 && dotIndex < fileName.length() - 1) ? fileName.substring(dotIndex + 1) : "";
    }

    /**
     * 指定されたパスの存在種別を取得します。
     *
     * @param path 確認対象のパス
     * @return パスの状態を示す整数値 (-1: NULL/空, 0: 存在しない, 1: ディレクトリ, 2: ファイル)
     */
    public static int getPathType(String path) {
        if (path == null || path.isEmpty()) {
            return PATH_IS_NULL;
        }
        File file = new File(path);
        if (!file.exists()) {
            return PATH_NOT_FOUND;
        }
        if (file.isDirectory()) {
            return PATH_IS_DIRECTORY;
        }
        if (file.isFile()) {
            return PATH_IS_FILE;
        }
        return PATH_NOT_FOUND;
    }

    /**
     * 指定されたパスがファイルまたはディレクトリとして存在するかどうかを判定します。
     *
     * @param path 確認対象のパス
     * @return ファイルまたはディレクトリが存在する場合は true、それ以外は false
     */
    public static boolean pathExists(String path) {
        int pathType = getPathType(path);
        return pathType == PATH_IS_DIRECTORY || pathType == PATH_IS_FILE;
    }

    /**
     * 指定されたパスが隠しファイルまたは隠しディレクトリかどうかを確認します。
     *
     * @param path 対象のパス
     * @return 隠しファイルまたは隠しディレクトリの場合は true、それ以外は false
     */
    public static boolean isHidden(String path) {
        if (path == null || path.isEmpty()) {
            return false;
        }
        File file = new File(path);
        return file.exists() && file.isHidden();
    }

    /**
     * 指定されたパスがシンボリックリンクかどうかを確認します。
     *
     * @param path 対象のパス
     * @return シンボリックリンクの場合は true、それ以外は false
     */
    public static boolean isSymlink(String path) {
        if (path == null || path.isEmpty()) {
            return false;
        }
        return Files.isSymbolicLink(Paths.get(path));
    }

    /**
     * 指定されたファイルが他のプロセスによってロックされているか判定します。
     *
     * @param filePath 対象のファイルパス
     * @return ロックされている場合は true、利用可能な場合は false
     */
    public static boolean isFileLocked(String filePath) {
        if (filePath == null || !new File(filePath).exists()) {
            return false;
        }
        File file = new File(filePath);
        try (RandomAccessFile raf = new RandomAccessFile(file, "rw");
             FileLock lock = raf.getChannel().tryLock()) {
            return lock == null;
        } catch (Exception e) {
            return true;
        }
    }

    /**
     * 指定されたパスの末尾のディレクトリ区切り文字を削除します。
     *
     * @param path 対象のパス
     * @return 末尾の区切り文字を除去したパス
     */
    public static String trimPathSeparator(String path) {
        if (path == null || path.isEmpty()) {
            return path;
        }
        if (MdlApp.isWindows()) {
            String normalized = path.replace('/', '\\');
            while (normalized.length() > 1 && (normalized.endsWith("\\") || normalized.endsWith("/"))) {
                // "C:\" などのルートドライブ末尾は残す考慮
                if (normalized.length() == 3 && normalized.charAt(1) == ':') {
                    break;
                }
                normalized = normalized.substring(0, normalized.length() - 1);
            }
            return normalized;
        } else {
            String normalized = path;
            while (normalized.length() > 1 && normalized.endsWith("/")) {
                normalized = normalized.substring(0, normalized.length() - 1);
            }
            return normalized;
        }
    }

    /**
     * @deprecated {@link #trimPathSeparator(String)} を使用してください。
     */
    @Deprecated
    public static String removeTrailingPathSeparator(String path) {
        return trimPathSeparator(path);
    }

    /**
     * 指定されたパスの絶対パスを取得します。
     *
     * @param path 対象のパス
     * @return 絶対パス。取得に失敗した場合は元のパスまたは空文字列
     */
    public static String getAbsolutePath(String path) {
        if (path == null || path.isEmpty()) {
            return "";
        }
        try {
            return new File(path).getAbsolutePath();
        } catch (Exception e) {
            return path;
        }
    }

    /**
     * 基準パスからのターゲットパスへの相対パスを取得します。
     *
     * @param basePath 基準となるパス
     * @param targetPath ターゲットとなるパス
     * @return 相対パス。計算できない場合は空文字列
     */
    public static String getRelativePath(String basePath, String targetPath) {
        if (basePath == null || targetPath == null || basePath.isEmpty() || targetPath.isEmpty()) {
            return "";
        }
        try {
            Path base = Paths.get(getAbsolutePath(basePath)).normalize();
            Path target = Paths.get(getAbsolutePath(targetPath)).normalize();
            return base.relativize(target).toString();
        } catch (Exception e) {
            return "";
        }
    }

    /**
     * ディレクトリの基本情報をフォーマット済み文字列として取得します。
     *
     * @param path 対象ディレクトリのパス
     * @param verbosity 詳細出力レベル (0以上で更新日時を含む)
     * @param encloseInQuotes パスをダブルクォーテーションで囲むかどうか
     * @return フォーマットされたディレクトリ情報文字列
     */
    public static String getDirInfoStr(String path, int verbosity, boolean encloseInQuotes) {
        String tempPath = encloseInQuotes ? "\"" + path + "\"" : path;
        if (verbosity < 0) {
            return tempPath;
        }

        String dateStr = "";
        try {
            File file = new File(path);
            if (file.exists()) {
                LocalDateTime mtime = LocalDateTime.ofInstant(Instant.ofEpochMilli(file.lastModified()), ZoneId.systemDefault());
                dateStr = "[" + MdlDate.getFormattedDate(mtime, "yyyy/MM/dd HH:mm:ss") + "]";
            }
        } catch (Exception e) {
            // 無視
        }

        return "[D]" + dateStr + " " + tempPath;
    }

    /**
     * @deprecated {@link #getDirInfoStr(String, int, boolean)} を使用してください。
     */
    @Deprecated
    public static String getDirectoryInfoString(String path, int verbosity, boolean encloseInQuotes) {
        return getDirInfoStr(path, verbosity, encloseInQuotes);
    }

    /**
     * 指定された日時が、指定された前後日時の範囲内に収まっているかを判定します。
     *
     * @param targetDateTime 判定対象の日時
     * @param checkBefore 指定以前かの判定を行うフラグ
     * @param beforeDateTime 以前の比較対象日時
     * @param checkAfter 指定以降かの判定を行うフラグ
     * @param afterDateTime 以降の比較対象日時
     * @return 有効な範囲内であれば true、それ以外は false
     */
    public static boolean isValidDateTime(LocalDateTime targetDateTime, boolean checkBefore, LocalDateTime beforeDateTime, boolean checkAfter, LocalDateTime afterDateTime) {
        if (targetDateTime == null) {
            return false;
        }
        if (checkBefore && beforeDateTime != null && targetDateTime.isAfter(beforeDateTime)) {
            return false;
        }
        if (checkAfter && afterDateTime != null && targetDateTime.isBefore(afterDateTime)) {
            return false;
        }
        return true;
    }

    /**
     * 指定されたパスのファイルまたはディレクトリの更新日時が、指定範囲内かを判定します。
     *
     * @param path 対象のパス
     * @param checkBefore 指定以前かの判定を行うフラグ
     * @param beforeDateTime 以前の比較対象日時
     * @param checkAfter 指定以降かの判定を行うフラグ
     * @param afterDateTime 以降の比較対象日時
     * @return 有効な範囲内であれば true、それ以外は false
     */
    public static boolean isValidDateTime(String path, boolean checkBefore, LocalDateTime beforeDateTime, boolean checkAfter, LocalDateTime afterDateTime) {
        if (path == null) {
            return false;
        }
        File file = new File(path);
        if (!file.exists()) {
            return false;
        }
        LocalDateTime lastModified = LocalDateTime.ofInstant(Instant.ofEpochMilli(file.lastModified()), ZoneId.systemDefault());
        return isValidDateTime(lastModified, checkBefore, beforeDateTime, checkAfter, afterDateTime);
    }

    /**
     * ディレクトリの更新日時が指定範囲内かを判定します。
     *
     * @param path 対象ディレクトリのパス
     * @param checkBefore 指定以前かの判定を行うフラグ
     * @param beforeDateTime 以前の比較対象日時
     * @param checkAfter 指定以降かの判定を行うフラグ
     * @param afterDateTime 以降の比較対象日時
     * @return 有効な範囲内であれば true、それ以外は false
     */
    public static boolean isValidDirDateTime(String path, boolean checkBefore, LocalDateTime beforeDateTime, boolean checkAfter, LocalDateTime afterDateTime) {
        return isValidDateTime(path, checkBefore, beforeDateTime, checkAfter, afterDateTime);
    }

    /**
     * @deprecated {@link #isValidDirDateTime(String, boolean, LocalDateTime, boolean, LocalDateTime)} を使用してください。
     */
    @Deprecated
    public static boolean isValidDirectoryDateTime(String path, boolean checkBefore, LocalDateTime beforeDateTime, boolean checkAfter, LocalDateTime afterDateTime) {
        return isValidDirDateTime(path, checkBefore, beforeDateTime, checkAfter, afterDateTime);
    }

    /**
     * ファイルの更新日時が指定範囲内かを判定します。
     *
     * @param path 対象ファイルのパス
     * @param checkBefore 指定以前かの判定を行うフラグ
     * @param beforeDateTime 以前の比較対象日時
     * @param checkAfter 指定以降かの判定を行うフラグ
     * @param afterDateTime 以降の比較対象日時
     * @return 有効な範囲内であれば true、それ以外は false
     */
    public static boolean isValidFileDateTime(String path, boolean checkBefore, LocalDateTime beforeDateTime, boolean checkAfter, LocalDateTime afterDateTime) {
        return isValidDateTime(path, checkBefore, beforeDateTime, checkAfter, afterDateTime);
    }

    /**
     * ファイルの基本情報をフォーマット済み文字列として取得します。
     *
     * @param path 対象ファイルのパス
     * @param verbosity 詳細出力レベル (1以上で日時、2以上でファイルサイズを追加)
     * @param encloseInQuotes パスをダブルクォーテーションで囲むかどうか
     * @return フォーマットされたファイル情報文字列
     */
    public static String getFileInfoString(String path, int verbosity, boolean encloseInQuotes) {
        String tempPath = encloseInQuotes ? "\"" + path + "\"" : path;
        if (verbosity < 0) {
            return tempPath;
        }

        String line = "[F]";
        try {
            File file = new File(path);
            if (file.exists()) {
                LocalDateTime mtime = LocalDateTime.ofInstant(Instant.ofEpochMilli(file.lastModified()), ZoneId.systemDefault());
                line += "[" + MdlDate.getFormattedDate(mtime, "yyyy/MM/dd HH:mm:ss") + "]";
                if (verbosity > 1) {
                    line += "[" + MdlUtil.formatByteSizeRight(file.length()) + "]";
                }
            }
        } catch (Exception e) {
            // 無視
        }

        return line + " " + tempPath;
    }

    /**
     * 外部コマンド実行用に、プレースホルダーを含む文字列を置換します。
     *
     * @param target 置換対象テンプレート文字列
     * @param fullPath 対象ファイルのフルパス
     * @param basePath ベースディレクトリパス
     * @param relativePath 相対パス
     * @param encloseInQuotes 結果全体をダブルクォーテーションで囲むかどうか
     * @param verbosity 詳細レベル
     * @param currentDateTime 日時プレースホルダー置換用の基準日時
     * @return 置換完了後のコマンド文字列
     */
    public static String replacePathForCmd(String target, String fullPath, String basePath, String relativePath, boolean encloseInQuotes, int verbosity, LocalDateTime currentDateTime) {
        if (target == null) {
            return "";
        }
        String relPath = (relativePath == null || relativePath.isEmpty()) ? "." : relativePath;
        String relDir = getDirectoryPath(relPath);
        String tempPath = target;

        // ファイルパス
        tempPath = tempPath.replace("{}", fullPath != null ? fullPath : "")
                .replace("_PATH_", fullPath != null ? fullPath : "")
                .replace("_RELPATH_", relPath)
                .replace("_RELFLAT_", relPath.replace("\\", "_").replace("/", "_"));
        // ベースディレクトリパス
        tempPath = tempPath.replace("_BASEDIR_", basePath != null ? basePath : "");
        // 親ディレクトリパス
        String parentDir = fullPath != null ? getDirectoryPath(fullPath) : "";
        tempPath = tempPath.replace("_DIR_", parentDir)
                .replace("_RELDIR_", (relDir == null || relDir.isEmpty()) ? "." : relDir)
                .replace("_RELDIRFLAT_", (relDir != null) ? relDir.replace("\\", "_").replace("/", "_") : ".");
        // ファイル名
        String fileName = fullPath != null ? getFileName(fullPath) : "";
        String baseName = fullPath != null ? getBaseName(fullPath) : "";
        tempPath = tempPath.replace("_FILENAME_", fileName)
                .replace("_BASENAME_", baseName);
        // 環境変数
        String userDomain = System.getenv("USERDOMAIN") != null ? System.getenv("USERDOMAIN") : "";
        String computerName = System.getenv("COMPUTERNAME");
        if (computerName == null || computerName.isEmpty()) {
            computerName = System.getenv("HOSTNAME");
        }
        if (computerName == null || computerName.isEmpty()) {
            computerName = System.getenv("HOST");
        }
        if (computerName == null || computerName.isEmpty()) {
            try {
                computerName = java.net.InetAddress.getLocalHost().getHostName();
            } catch (Exception e) {
                computerName = "localhost";
            }
        }
        String userName = System.getProperty("user.name", "");
        tempPath = tempPath.replace("_USERDOMAIN_", userDomain)
                .replace("_COMPUTERNAME_", computerName != null ? computerName : "")
                .replace("_USERNAME_", userName);
        // その他
        tempPath = tempPath.replace("%%", "%");
        tempPath = MdlDate.replaceWithDateTime(tempPath, currentDateTime != null ? currentDateTime : LocalDateTime.now());

        return encloseInQuotes ? " \"" + tempPath + "\"" : tempPath;
    }

    /**
     * @deprecated {@link #replacePathForCmd(String, String, String, String, boolean, int, LocalDateTime)} を使用してください。
     */
    @Deprecated
    public static String replacePathForCmdExec(String target, String fullPath, String basePath, String relativePath, boolean encloseInQuotes, int verbosity, LocalDateTime currentDateTime) {
        return replacePathForCmd(target, fullPath, basePath, relativePath, encloseInQuotes, verbosity, currentDateTime);
    }

    /**
     * 外部コマンド実行用にプレースホルダーを含む文字列を置換します（現在日時を使用）。
     *
     * @param target 置換対象テンプレート文字列
     * @param fullPath 対象ファイルのフルパス
     * @param basePath ベースディレクトリパス
     * @param relativePath 相対パス
     * @param encloseInQuotes 結果全体をダブルクォーテーションで囲むかどうか
     * @param verbosity 詳細レベル
     * @return 置換完了後のコマンド文字列
     */
    public static String replacePathForCmd(String target, String fullPath, String basePath, String relativePath, boolean encloseInQuotes, int verbosity) {
        return replacePathForCmd(target, fullPath, basePath, relativePath, encloseInQuotes, verbosity, LocalDateTime.now());
    }

    /**
     * @deprecated {@link #replacePathForCmd(String, String, String, String, boolean, int)} を使用してください。
     */
    @Deprecated
    public static String replacePathForCmdExec(String target, String fullPath, String basePath, String relativePath, boolean encloseInQuotes, int verbosity) {
        return replacePathForCmd(target, fullPath, basePath, relativePath, encloseInQuotes, verbosity);
    }

    /**
     * 外部コマンド実行用にプレースホルダーを含む文字列を置換します（空のベースパスを使用）。
     *
     * @param target 置換対象テンプレート文字列
     * @param fullPath 対象ファイルのフルパス
     * @param relativePath 相対パス
     * @param encloseInQuotes 結果全体をダブルクォーテーションで囲むかどうか
     * @param verbosity 詳細レベル
     * @param currentDateTime 日時プレースホルダー置換用の基準日時
     * @return 置換完了後のコマンド文字列
     */
    public static String replacePathForCmd(String target, String fullPath, String relativePath, boolean encloseInQuotes, int verbosity, LocalDateTime currentDateTime) {
        return replacePathForCmd(target, fullPath, "", relativePath, encloseInQuotes, verbosity, currentDateTime);
    }

    /**
     * @deprecated {@link #replacePathForCmd(String, String, String, boolean, int, LocalDateTime)} を使用してください。
     */
    @Deprecated
    public static String replacePathForCmdExec(String target, String fullPath, String relativePath, boolean encloseInQuotes, int verbosity, LocalDateTime currentDateTime) {
        return replacePathForCmd(target, fullPath, relativePath, encloseInQuotes, verbosity, currentDateTime);
    }

    /**
     * 外部コマンド実行用にプレースホルダーを含む文字列を置換します（空のベースパス、現在日時を使用）。
     *
     * @param target 置換対象テンプレート文字列
     * @param fullPath 対象ファイルのフルパス
     * @param relativePath 相対パス
     * @param encloseInQuotes 結果全体をダブルクォーテーションで囲むかどうか
     * @param verbosity 詳細レベル
     * @return 置換完了後のコマンド文字列
     */
    public static String replacePathForCmd(String target, String fullPath, String relativePath, boolean encloseInQuotes, int verbosity) {
        return replacePathForCmd(target, fullPath, "", relativePath, encloseInQuotes, verbosity, LocalDateTime.now());
    }

    /**
     * @deprecated {@link #replacePathForCmd(String, String, String, boolean, int)} を使用してください。
     */
    @Deprecated
    public static String replacePathForCmdExec(String target, String fullPath, String relativePath, boolean encloseInQuotes, int verbosity) {
        return replacePathForCmd(target, fullPath, relativePath, encloseInQuotes, verbosity);
    }

    /**
     * 指定されたパスに空ファイルを作成（タッチ）します。必要な親ディレクトリは自動作成されます。
     *
     * @param path 作成するファイルのパス
     * @return 操作結果のステータスコード
     */
    public static int createEmptyFile(String path) {
        if (path == null || path.isEmpty()) {
            return NG_TOUCH_WRONG_ARG;
        }
        String absPath = getAbsolutePath(path);
        int pathType = getPathType(absPath);

        switch (pathType) {
            case PATH_IS_DIRECTORY:
                return NG_TOUCH_DIR_EXIST;
            case PATH_IS_FILE:
                return OK_TOUCH_ALREADY_EXIST;
            case PATH_IS_NULL:
                return NG_TOUCH_WRONG_ARG;
            default:
                break;
        }

        String directoryPath = getDirectoryPath(absPath);
        if (!directoryPath.isEmpty()) {
            int dirType = getPathType(directoryPath);
            if (dirType == PATH_IS_FILE) {
                return NG_TOUCH;
            } else if (dirType == PATH_NOT_FOUND) {
                createDirectory(directoryPath);
            }
        }

        try {
            File file = new File(absPath);
            if (file.createNewFile()) {
                return OK_TOUCH_CREATE;
            }
            return OK_TOUCH_ALREADY_EXIST;
        } catch (Exception e) {
            return NG_TOUCH;
        }
    }

    /**
     * 指定されたパスにディレクトリを作成します。
     *
     * @param path 作成するディレクトリのパス
     * @return 操作結果のステータスコード
     */
    public static int createDirectory(String path) {
        if (path == null || path.isEmpty()) {
            return NG_MKDIR_WRONG_ARG;
        }
        String absPath = getAbsolutePath(path);
        absPath = removeTrailingPathSeparator(absPath);

        int pathType = getPathType(absPath);
        switch (pathType) {
            case PATH_IS_NULL:
                return NG_MKDIR_WRONG_ARG;
            case PATH_IS_DIRECTORY:
                return OK_MKDIR_ALREADY_EXIST;
            case PATH_IS_FILE:
                return NG_MKDIR_FILE_EXIST;
            default:
                break;
        }

        try {
            File dir = new File(absPath);
            if (dir.mkdirs()) {
                return OK_MKDIR_CREATE;
            }
            return dir.exists() ? OK_MKDIR_ALREADY_EXIST : NG_MKDIR;
        } catch (Exception e) {
            return NG_MKDIR;
        }
    }

    /**
     * 指定されたパスのファイルまたはディレクトリを再帰的に削除します。
     *
     * @param path 削除対象のパス
     * @param verbosity 詳細レベル
     * @return 削除が成功した場合は true、それ以外は false
     */
    public static boolean deleteRecursively(String path, int verbosity) {
        if (path == null || path.isEmpty()) {
            return true;
        }
        File file = new File(path);
        if (!file.exists()) {
            return true;
        }
        if (file.isDirectory()) {
            return deleteDirectory(file, verbosity);
        }
        return deleteFile(file, verbosity);
    }

    /**
     * 指定されたパスのファイルまたはディレクトリを再帰的に削除します（デフォルトレベル）。
     *
     * @param path 削除対象のパス
     * @return 削除が成功した場合は true、それ以外は false
     */
    public static boolean deleteRecursively(String path) {
        return deleteRecursively(path, 0);
    }

    /**
     * 指定されたディレクトリを再帰的に削除します。
     *
     * @param directoryInfo 削除対象のディレクトリ
     * @param verbosity 詳細レベル
     * @return 成功した場合は true、失敗した場合は false
     */
    public static boolean deleteRecursively(File directoryInfo, int verbosity) {
        if (directoryInfo == null || !directoryInfo.exists()) {
            return true;
        }
        boolean isSuccess = true;
        File[] files = directoryInfo.listFiles();
        if (files != null) {
            for (File file : files) {
                if (file.isDirectory()) {
                    if (!deleteDirectory(file, verbosity)) {
                        isSuccess = false;
                    }
                } else {
                    if (!deleteFile(file, verbosity)) {
                        isSuccess = false;
                    }
                }
            }
        }
        changeDirectoryAttributes(directoryInfo.getAbsolutePath(), "W");
        try {
            if (!directoryInfo.delete()) {
                isSuccess = false;
            }
        } catch (Exception ex) {
            if (verbosity > 0) {
                System.out.println(" => FAILED TO DELETE DIRECTORY(" + directoryInfo.getAbsolutePath() + ")：EXCEPTION：" + ex.getMessage());
            }
            isSuccess = false;
        }
        return isSuccess;
    }

    /**
     * 指定されたディレクトリを削除します（シンボリックリンク対応）。
     *
     * @param directoryInfo 対象のディレクトリ
     * @param verbosity 詳細レベル
     * @return 成功した場合は true、失敗した場合は false
     */
    public static boolean deleteDirectory(File directoryInfo, int verbosity) {
        if (directoryInfo == null || !directoryInfo.exists()) {
            return true;
        }
        if (isSymlink(directoryInfo.getAbsolutePath())) {
            try {
                changeDirectoryAttributes(directoryInfo.getAbsolutePath(), "W");
                return directoryInfo.delete();
            } catch (Exception ex) {
                if (verbosity > 0) {
                    System.out.println(" => FAILED TO DELETE SYMLINK(" + directoryInfo.getAbsolutePath() + ")：EXCEPTION：" + ex.getMessage());
                }
                return false;
            }
        }
        return deleteRecursively(directoryInfo, verbosity);
    }

    /**
     * 指定されたファイルを削除します。
     *
     * @param fileInfo 対象のファイル
     * @param verbosity 詳細レベル
     * @return 成功した場合は true、失敗した場合は false
     */
    public static boolean deleteFile(File fileInfo, int verbosity) {
        if (fileInfo == null || !fileInfo.exists()) {
            return true;
        }
        changeFileAttributes(fileInfo.getAbsolutePath(), "W");
        try {
            return fileInfo.delete();
        } catch (Exception ex) {
            if (verbosity > 0) {
                System.out.println(" => FAILED TO DELETE FILE(" + fileInfo.getAbsolutePath() + ")：EXCEPTION：" + ex.getMessage());
            }
            return false;
        }
    }

    /**
     * 指定されたパス配下の空のディレクトリを再帰的に検索・削除します。
     *
     * @param path 対象ディレクトリのパス
     * @param verbosity 詳細レベル
     * @return すべての空ディレクトリ削除が成功した場合は true、それ以外は false
     */
    public static boolean deleteEmptyDirs(String path, int verbosity) {
        if (path == null || !new File(path).exists()) {
            return true;
        }
        boolean isSuccess = true;
        try {
            File dir = new File(path);
            if (isEmptyDirectory(path)) {
                return deleteRecursively(path, verbosity);
            }
            File[] subDirs = dir.listFiles(File::isDirectory);
            if (subDirs != null) {
                for (File subDir : subDirs) {
                    if (!deleteEmptyDirs(subDir.getAbsolutePath(), verbosity)) {
                        isSuccess = false;
                    }
                    if (subDir.exists() && isEmptyDirectory(subDir.getAbsolutePath())) {
                        if (!deleteRecursively(subDir.getAbsolutePath(), verbosity)) {
                            isSuccess = false;
                        }
                    }
                }
            }
            if (dir.exists() && isEmptyDirectory(path)) {
                return deleteRecursively(path, verbosity);
            }
        } catch (Exception e) {
            isSuccess = false;
        }
        return isSuccess;
    }

    /**
     * @deprecated {@link #deleteEmptyDirs(String, int)} を使用してください。
     */
    @Deprecated
    public static boolean deleteEmptyDirectories(String path, int verbosity) {
        return deleteEmptyDirs(path, verbosity);
    }

    /**
     * 指定されたディレクトリが空かどうか判定します。
     *
     * @param path 対象ディレクトリのパス
     * @return 空の場合は true、それ以外は false
     */
    public static boolean isEmptyDirectory(String path) {
        if (path == null) {
            return false;
        }
        File dir = new File(path);
        if (!dir.exists() || !dir.isDirectory()) {
            return false;
        }
        String[] entries = dir.list();
        return entries != null && entries.length == 0;
    }

    /**
     * ファイル名として不適切な記号を除去または置換して安全なファイル名を生成します。
     *
     * @param originalFileName 元のファイル名
     * @return サニタイズ後のファイル名
     */
    public static String sanitizeFileName(String originalFileName) {
        if (originalFileName == null || originalFileName.isEmpty()) {
            return "";
        }
        String sanitized = originalFileName.replace(" ", "_")
                .replace("\\", "_")
                .replace("/", "_");
        char[] removeChars = new char[] {':', ';', '|', ',', '*', '?', '<', '>', '"'};
        for (char c : removeChars) {
            sanitized = sanitized.replace(Character.toString(c), "");
        }
        return sanitized;
    }

    /**
     * 指定されたファイルにメッセージ文字列を書き込みます。
     *
     * @param filePath 書き込み先のファイルパス
     * @param message 書き込むメッセージ文字列
     * @param append 追記する場合は true
     * @param encoding 文字エンコーディング
     */
    public static void writeFile(String filePath, String message, boolean append, Charset encoding) {
        if (filePath == null || filePath.isEmpty()) {
            return;
        }
        Charset enc = encoding != null ? encoding : StandardCharsets.UTF_8;
        java.nio.file.Path path = java.nio.file.Paths.get(filePath);
        java.nio.file.Path parent = path.getParent();
        if (parent != null && !java.nio.file.Files.exists(parent)) {
            createDirectory(parent.toString());
        }
        java.nio.file.OpenOption[] options = append
                ? new java.nio.file.OpenOption[] {java.nio.file.StandardOpenOption.CREATE, java.nio.file.StandardOpenOption.APPEND}
                : new java.nio.file.OpenOption[] {java.nio.file.StandardOpenOption.CREATE, java.nio.file.StandardOpenOption.TRUNCATE_EXISTING};
        try (BufferedWriter writer = java.nio.file.Files.newBufferedWriter(path, enc, options)) {
            writer.write(message != null ? message : "");
            writer.newLine();
            writer.flush();
        } catch (Exception e) {
            // 書込エラー
        }
    }

    /**
     * 指定されたファイルに複数行のメッセージを書き込みます。
     *
     * @param filePath 書き込み先のファイルパス
     * @param message 書き込むメッセージ行のリスト
     * @param append 追記する場合は true
     * @param encoding 文字エンコーディング
     */
    public static void writeFile(String filePath, List<String> message, boolean append, Charset encoding) {
        if (filePath == null || filePath.isEmpty()) {
            return;
        }
        Charset enc = encoding != null ? encoding : StandardCharsets.UTF_8;
        java.nio.file.Path path = java.nio.file.Paths.get(filePath);
        java.nio.file.Path parent = path.getParent();
        if (parent != null && !java.nio.file.Files.exists(parent)) {
            createDirectory(parent.toString());
        }
        java.nio.file.OpenOption[] options = append
                ? new java.nio.file.OpenOption[] {java.nio.file.StandardOpenOption.CREATE, java.nio.file.StandardOpenOption.APPEND}
                : new java.nio.file.OpenOption[] {java.nio.file.StandardOpenOption.CREATE, java.nio.file.StandardOpenOption.TRUNCATE_EXISTING};
        try (BufferedWriter writer = java.nio.file.Files.newBufferedWriter(path, enc, options)) {
            if (message != null) {
                for (String line : message) {
                    writer.write(line);
                    writer.newLine();
                }
            }
            writer.flush();
        } catch (Exception e) {
            // 書込エラー
        }
    }

    /**
     * 指定されたファイルにメッセージを追記書き込みします。
     *
     * @param filePath 書き込み先のファイルパス
     * @param message 書き込むメッセージ文字列
     */
    public static void writeFile(String filePath, String message) {
        writeFile(filePath, message, true, StandardCharsets.UTF_8);
    }

    /**
     * 指定されたファイルから最大バイト数制限付きで文字列を読み込みます。
     *
     * @param filePath 対象のファイルパス
     * @param maxBytes 読み込む最大バイト数（0以下の場合は無制限）
     * @param encoding 使用する文字エンコーディング
     * @return 読み込んだファイル内容文字列
     */
    public static String readFile(String filePath, int maxBytes, Charset encoding) {
        if (filePath == null || filePath.isEmpty()) {
            return "";
        }
        java.nio.file.Path path = java.nio.file.Paths.get(filePath);
        if (!java.nio.file.Files.exists(path)) {
            return "";
        }
        Charset enc = encoding != null ? encoding : Charset.defaultCharset();
        StringBuilder output = new StringBuilder();
        try (BufferedReader reader = java.nio.file.Files.newBufferedReader(path, enc)) {
            String line;
            while ((line = reader.readLine()) != null) {
                if (maxBytes > 0 && output.length() + line.length() > maxBytes) {
                    break;
                }
                output.append(line).append(System.lineSeparator());
            }
        } catch (Exception e) {
            // 読込エラー
        }
        return output.toString().trim();
    }

    /**
     * 指定されたファイルから自動エンコーディング検出で文字列を読み込みます。
     *
     * @param filePath 対象のファイルパス
     * @param maxBytes 読み込む最大バイト数（0以下の場合は無制限）
     * @return 読み込んだファイル内容文字列
     */
    public static String readFile(String filePath, int maxBytes) {
        return readFile(filePath, maxBytes, detectFileEncoding(filePath));
    }

    /**
     * Key=Value 形式のテキストファイルを読み込み、Map に展開します。
     *
     * @param filePath 対象のファイルパス
     * @param encoding 文字エンコーディング
     * @return Key と Value のマップ
     */
    public static Map<String, String> readFileToMap(String filePath, Charset encoding) {
        Map<String, String> dictionary = new LinkedHashMap<>();
        if (filePath == null || !new File(filePath).exists()) {
            return dictionary;
        }
        Charset enc = encoding != null ? encoding : Charset.defaultCharset();
        try (BufferedReader reader = new BufferedReader(new InputStreamReader(new FileInputStream(filePath), enc))) {
            String line;
            while ((line = reader.readLine()) != null) {
                Matcher match = KEY_VAL_REGEX.matcher(line);
                if (match.matches()) {
                    String key = MdlUtil.trimQuotes(match.group("KEY"));
                    String value = MdlUtil.trimQuotes(match.group("VAL"));
                    dictionary.put(key, value);
                }
            }
        } catch (Exception e) {
            // エラー時
        }
        return dictionary;
    }

    /**
     * @deprecated {@link #readFileToMap(String, Charset)} を使用してください。
     */
    @Deprecated
    public static Map<String, String> readFileToDictionary(String filePath, Charset encoding) {
        return readFileToMap(filePath, encoding);
    }

    /**
     * Key=Value 形式のテキストファイルを自動エンコーディング検出で読み込み、Map に展開します。
     *
     * @param filePath 対象のファイルパス
     * @return Key と Value のマップ
     */
    public static Map<String, String> readFileToMap(String filePath) {
        return readFileToMap(filePath, detectFileEncoding(filePath));
    }

    /**
     * @deprecated {@link #readFileToMap(String)} を使用してください。
     */
    @Deprecated
    public static Map<String, String> readFileToDictionary(String filePath) {
        return readFileToMap(filePath);
    }

    /**
     * 指定されたパスのファイルまたはディレクトリの日時を一括設定します。
     *
     * @param path 対象のパス
     * @param date 設定する日時文字列
     * @param mode 設定モード
     * @param pathType パス種別
     * @param validateDate 日時文字列の検証を行うかどうか
     * @param force 変更の有無に関わらず強制設定するか
     * @param execute 実際に処理を実行するか
     * @return 処理が成功した場合は true
     */
    public static boolean setDate(String path, String date, int mode, int pathType, boolean validateDate, boolean force, boolean execute) {
        setDateMain(path, date, mode, pathType, validateDate, force, execute);
        return true;
    }

    /**
     * 指定されたパスの日時を設定します（execute=true）。
     *
     * @param path 対象のパス
     * @param date 設定する日時文字列
     * @param mode 設定モード
     * @param pathType パス種別
     * @param validateDate 日時文字列の検証を行うかどうか
     * @param force 強制設定フラグ
     * @return 処理が成功した場合は true
     */
    public static boolean setDate(String path, String date, int mode, int pathType, boolean validateDate, boolean force) {
        return setDate(path, date, mode, pathType, validateDate, force, true);
    }

    /**
     * 指定されたパスの日時を設定します（force=true, execute=true）。
     *
     * @param path 対象のパス
     * @param date 設定する日時文字列
     * @param mode 設定モード
     * @param pathType パス種別
     * @param validateDate 日時文字列の検証を行うかどうか
     * @return 処理が成功した場合は true
     */
    public static boolean setDate(String path, String date, int mode, int pathType, boolean validateDate) {
        return setDate(path, date, mode, pathType, validateDate, true, true);
    }

    /**
     * 指定されたパスのファイルまたはディレクトリの日時設定のメイン処理です。
     *
     * @param path 対象のパス
     * @param date 設定する日時文字列
     * @param mode 設定モード
     * @param pathType パス種別
     * @param validateDate 日時文字列の検証を行うかどうか
     * @param force 強制設定フラグ
     * @param execute 実行フラグ
     * @return 処理結果ステータスコード
     */
    public static int setDateMain(String path, String date, int mode, int pathType, boolean validateDate, boolean force, boolean execute) {
        LocalDateTime dateTime;
        int effectivePathType = pathType;
        if (PATH_AUTO_DETECT == effectivePathType) {
            effectivePathType = getPathType(path);
            if (PATH_NOT_FOUND == effectivePathType || PATH_IS_NULL == effectivePathType) {
                return -1;
            }
        }

        if (validateDate) {
            String validDate = MdlDate.validateAndFormat(date, true);
            if (validDate == null || validDate.isEmpty()) {
                return -1;
            }
            dateTime = MdlDate.parseDateTime(validDate);
        } else {
            dateTime = MdlDate.parseDateTime(date);
        }

        if (dateTime == null) {
            return -1;
        }

        if (effectivePathType == PATH_IS_FILE) {
            return setDateToFileMain(path, dateTime, mode, force, execute);
        } else if (effectivePathType == PATH_IS_DIRECTORY) {
            return setDateToDirMain(path, dateTime, mode, force, execute);
        }
        return 0;
    }

    /**
     * ディレクトリの日時を設定します。
     *
     * @param path 対象ディレクトリのパス
     * @param date 設定日時
     * @param mode 設定モード
     * @param force 強制設定フラグ
     * @param execute 実行フラグ
     * @return 処理結果コード
     */
    public static int setDateToDirMain(String path, LocalDateTime date, int mode, boolean force, boolean execute) {
        return setDateToFileMain(path, date, mode, force, execute);
    }

    /**
     * ファイルの日時を設定します。
     *
     * @param path 対象ファイルのパス
     * @param date 設定日時
     * @param mode 設定モード
     * @param force 強制設定フラグ
     * @param execute 実行フラグ
     * @return 処理結果コード
     */
    public static int setDateToFileMain(String path, LocalDateTime date, int mode, boolean force, boolean execute) {
        if (path == null || date == null) {
            return 0;
        }
        File file = new File(path);
        if (!file.exists()) {
            return 0;
        }

        boolean setCreate = mode == 1 || mode == 3 || mode == 5 || mode == 7;
        boolean setModify = mode == 2 || mode == 3 || mode == 6 || mode == 7;
        boolean setAccess = mode == 4 || mode == 5 || mode == 6 || mode == 7;
        if (mode < 1 || mode > 6) {
            setCreate = setModify = setAccess = true;
        }

        int resultCode = 0;
        long targetMillis = date.atZone(ZoneId.systemDefault()).toInstant().toEpochMilli();

        if (setModify && (force || file.lastModified() != targetMillis)) {
            if (execute) {
                file.setLastModified(targetMillis);
            }
            resultCode += 10;
        }

        if (setCreate) {
            resultCode += 100;
        }
        if (setAccess) {
            resultCode += 1;
        }

        return resultCode;
    }

    /**
     * 2つのディレクトリのタイムスタンプに指定許容範囲（秒）以上の差異があるか確認します。
     *
     * @param sourceDir 比較元のディレクトリ
     * @param targetDir 比較先のディレクトリ
     * @param timeRange 許容誤差（秒）
     * @param mode 判定モード
     * @return タイムスタンプが異なる場合は true
     */
    public static boolean isDirTimeDiff(File sourceDir, File targetDir, double timeRange, int mode) {
        return isFileTimeDiff(sourceDir, targetDir, timeRange, mode);
    }

    /**
     * @deprecated {@link #isDirTimeDiff(File, File, double, int)} を使用してください。
     */
    @Deprecated
    public static boolean isDirectoryTimestampDifferent(File sourceDir, File targetDir, double timeRange, int mode) {
        return isDirTimeDiff(sourceDir, targetDir, timeRange, mode);
    }

    /**
     * 2つのファイルのタイムスタンプに指定許容範囲（秒）以上の差異があるか確認します。
     *
     * @param sourceFile 比較元のファイル
     * @param targetFile 比較先のファイル
     * @param timeRange 許容誤差（秒）
     * @param mode 判定モード
     * @return タイムスタンプが異なる場合は true
     */
    public static boolean isFileTimeDiff(File sourceFile, File targetFile, double timeRange, int mode) {
        if (sourceFile == null || targetFile == null || !sourceFile.exists() || !targetFile.exists()) {
            return true;
        }
        LocalDateTime srcTime = LocalDateTime.ofInstant(Instant.ofEpochMilli(sourceFile.lastModified()), ZoneId.systemDefault());
        LocalDateTime tgtTime = LocalDateTime.ofInstant(Instant.ofEpochMilli(targetFile.lastModified()), ZoneId.systemDefault());
        return MdlDate.compareDateTime(srcTime, tgtTime, timeRange) != 0;
    }

    /**
     * @deprecated {@link #isFileTimeDiff(File, File, double, int)} を使用してください。
     */
    @Deprecated
    public static boolean isFileTimestampDifferent(File sourceFile, File targetFile, double timeRange, int mode) {
        return isFileTimeDiff(sourceFile, targetFile, timeRange, mode);
    }

    /**
     * 指定されたパスのモード（属性）を変更します。
     *
     * @param path 対象のパス
     * @param mode 属性モード（"W", "R", "-R", "H", "-H" など）
     * @return 成功した場合は true
     */
    public static boolean changeMode(String path, String mode) {
        if (path == null) {
            return true;
        }
        File file = new File(path);
        if (!file.exists()) {
            return true;
        }
        return changeFileAttributes(path, mode);
    }

    /**
     * 指定されたディレクトリの属性を変更します。
     *
     * @param path 対象ディレクトリのパス
     * @param mode 属性モード文字列
     * @return 成功した場合は true
     */
    public static boolean changeDirAttributes(String path, String mode) {
        return changeFileAttributes(path, mode);
    }

    /**
     * @deprecated {@link #changeDirAttributes(String, String)} を使用してください。
     */
    @Deprecated
    public static boolean changeDirectoryAttributes(String path, String mode) {
        return changeDirAttributes(path, mode);
    }

    /**
     * 指定されたファイルの属性を変更します。
     *
     * @param path 対象ファイルのパス
     * @param mode 属性モード文字列
     * @return 成功した場合は true
     */
    public static boolean changeFileAttributes(String path, String mode) {
        if (path == null || mode == null) {
            return false;
        }
        File file = new File(path);
        if (!file.exists()) {
            return false;
        }
        String upper = mode.toUpperCase(Locale.ROOT);
        if (upper.contains("W") && !upper.contains("-W")) {
            file.setWritable(true, false);
        } else if (upper.contains("R") || upper.contains("-W")) {
            file.setReadOnly();
        }
        return true;
    }

    /**
     * 指定されたファイルの SHA-1 ハッシュ値を計算し、小文字の16進数文字列で返します。
     *
     * @param path 対象のファイルパス
     * @return SHA-1 ハッシュ文字列
     */
    public static String computeSha1Hash(String path) {
        if (path == null || !new File(path).exists()) {
            return "";
        }
        try {
            MessageDigest md = MessageDigest.getInstance("SHA-1");
            try (FileInputStream fis = new FileInputStream(path)) {
                byte[] buffer = new byte[8192];
                int bytesRead;
                while ((bytesRead = fis.read(buffer)) != -1) {
                    md.update(buffer, 0, bytesRead);
                }
            }
            byte[] digest = md.digest();
            StringBuilder sb = new StringBuilder();
            for (byte b : digest) {
                sb.append(String.format("%02x", b));
            }
            return sb.toString();
        } catch (Exception e) {
            return "";
        }
    }

    /**
     * ファイルのエンコーディングを自動検出します。
     *
     * @param filePath 対象のファイルパス
     * @return 検出された Charset オブジェクト
     */
    public static Charset detectFileEncoding(String filePath) {
        if (filePath == null || !new File(filePath).exists()) {
            return Charset.defaultCharset();
        }
        try {
            byte[] bytes = Files.readAllBytes(Paths.get(filePath));
            Charset detected = detectEncoding(bytes);
            return detected != null ? detected : Charset.defaultCharset();
        } catch (Exception e) {
            return Charset.defaultCharset();
        }
    }

    /**
     * バイト配列のエンコーディングを自動判定します。
     *
     * @param bytes 対象のバイト配列
     * @return 検出された Charset オブジェクト。バイナリの場合は null
     */
    public static Charset detectEncoding(byte[] bytes) {
        if (bytes == null || bytes.length == 0) {
            return StandardCharsets.UTF_8;
        }

        int len = bytes.length;
        boolean isBinary = false;
        for (int i = 0; i < len; i++) {
            byte b1 = bytes[i];
            if (b1 <= 0x06 || b1 == 0x7F || (b1 & 0xFF) == 0xFF) {
                isBinary = true;
                if (b1 == 0x00 && i < len - 1 && (bytes[i + 1] & 0xFF) <= 0x7F) {
                    return StandardCharsets.UTF_16LE;
                }
            }
        }
        if (isBinary) {
            return null;
        }

        boolean notJapanese = true;
        for (byte b1 : bytes) {
            if ((b1 & 0xFF) == 0x1B || (b1 & 0xFF) >= 0x80) {
                notJapanese = false;
                break;
            }
        }
        if (notJapanese) {
            return StandardCharsets.US_ASCII;
        }

        // JIS, SJIS, EUC, UTF-8 簡易判定
        int sjis = 0;
        int euc = 0;
        int utf8 = 0;
        for (int i = 0; i < len - 1; i++) {
            int b1 = bytes[i] & 0xFF;
            int b2 = bytes[i + 1] & 0xFF;
            if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC))
                    && ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC))) {
                sjis += 2;
                i++;
            }
        }
        for (int i = 0; i < len - 1; i++) {
            int b1 = bytes[i] & 0xFF;
            int b2 = bytes[i + 1] & 0xFF;
            if ((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) {
                euc += 2;
                i++;
            }
        }
        for (int i = 0; i < len - 1; i++) {
            int b1 = bytes[i] & 0xFF;
            int b2 = bytes[i + 1] & 0xFF;
            if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF)) {
                utf8 += 2;
                i++;
            } else if (i < len - 2) {
                int b3 = bytes[i + 2] & 0xFF;
                if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) && (0x80 <= b3 && b3 <= 0xBF)) {
                    utf8 += 3;
                    i += 2;
                }
            }
        }

        if (euc > sjis && euc > utf8) {
            return Charset.forName("EUC-JP");
        }
        if (sjis > euc && sjis > utf8) {
            return Charset.forName("MS932");
        }
        if (utf8 > euc && utf8 > sjis) {
            return StandardCharsets.UTF_8;
        }

        return StandardCharsets.UTF_8;
    }

    /**
     * パスが含めるパターン・除外するパターンに一致するか評価しコード値を返します。
     *
     * @param path 評価対象のパス
     * @param includeBaseName 包含判定時にファイル名のみを使用するか
     * @param excludeBaseName 除外判定時にファイル名のみを使用するか
     * @param includePatterns 包含する正規表現パターンのリスト
     * @param excludePatterns 除外する正規表現パターンのリスト
     * @param isOrCondition 包含条件をOR評価するか
     * @param debugLevel デバッグ出力レベル
     * @return 評価コード（1: 適合、2: 除外対象、0: 未該当）
     */
    public static int evalPathFilterCode(String path, boolean includeBaseName, boolean excludeBaseName, List<String> includePatterns, List<String> excludePatterns, boolean isOrCondition, int debugLevel) {
        if (path == null) {
            return 0;
        }
        String target = includeBaseName ? getFileName(path) : path;
        int result = 1;

        if (includePatterns != null && !includePatterns.isEmpty()) {
            boolean isHit = false;
            result = 0;
            for (String pattern : includePatterns) {
                try {
                    Pattern p = Pattern.compile(pattern, Pattern.CASE_INSENSITIVE);
                    if (p.matcher(target).find()) {
                        isHit = true;
                        if (debugLevel > 7) {
                            System.out.println("[MdlFile.evaluatePathFilterCode()][INC][" + includeBaseName + "] HIT : " + pattern + " -> " + target);
                        }
                        break;
                    }
                } catch (Exception e) {
                    // パターンエラー
                }
            }
            if (isHit) {
                result = 1;
                if (isOrCondition) {
                    return result;
                }
            }
        }

        target = excludeBaseName ? getFileName(path) : path;
        if (excludePatterns != null && !excludePatterns.isEmpty()) {
            for (String pattern : excludePatterns) {
                try {
                    Pattern p = Pattern.compile(pattern, Pattern.CASE_INSENSITIVE);
                    if (p.matcher(target).find()) {
                        if (debugLevel > 7) {
                            System.out.println("[MdlFile.evaluatePathFilterCode()][EXC][" + excludeBaseName + "] HIT : " + pattern + " -> " + target);
                        }
                        return 2;
                    }
                } catch (Exception e) {
                    // パターンエラー
                }
            }
        }
        return result;
    }

    /**
     * @deprecated {@link #evalPathFilterCode(String, boolean, boolean, List, List, boolean, int)} を使用してください。
     */
    @Deprecated
    public static int evaluatePathFilterCode(String path, boolean includeBaseName, boolean excludeBaseName, List<String> includePatterns, List<String> excludePatterns, boolean isOrCondition, int debugLevel) {
        return evalPathFilterCode(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, isOrCondition, debugLevel);
    }

    /**
     * パスが指定されたフィルターパターンに一致するか判定します。
     *
     * @param path 評価対象のパス
     * @param includeBaseName 包含判定時にファイル名のみを使用するか
     * @param excludeBaseName 除外判定時にファイル名のみを使用するか
     * @param includePatterns 包含する正規表現パターンのリスト
     * @param excludePatterns 除外する正規表現パターンのリスト
     * @param isOrCondition 包含条件をOR評価するか
     * @param debugLevel デバッグ出力レベル
     * @return 適合する場合は true、それ以外は false
     */
    public static boolean isPathFilterMatched(String path, boolean includeBaseName, boolean excludeBaseName, List<String> includePatterns, List<String> excludePatterns, boolean isOrCondition, int debugLevel) {
        return evalPathFilterCode(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, isOrCondition, debugLevel) == 1;
    }

    /**
     * パスが指定されたフィルターパターンに一致するか判定します（isOrCondition=false）。
     *
     * @param path 評価対象のパス
     * @param includeBaseName 包含判定時にファイル名のみを使用するか
     * @param excludeBaseName 除外判定時にファイル名のみを使用するか
     * @param includePatterns 包含する正規表現パターンのリスト
     * @param excludePatterns 除外する正規表現パターンのリスト
     * @param debugLevel デバッグ出力レベル
     * @return 適合する場合は true、それ以外は false
     */
    public static boolean isPathFilterMatched(String path, boolean includeBaseName, boolean excludeBaseName, List<String> includePatterns, List<String> excludePatterns, int debugLevel) {
        return isPathFilterMatched(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, false, debugLevel);
    }

    /**
     * パスが指定されたフィルターパターンに一致するか判定します（debugLevel=0）。
     *
     * @param path 評価対象のパス
     * @param includeBaseName 包含判定時にファイル名のみを使用するか
     * @param excludeBaseName 除外判定時にファイル名のみを使用するか
     * @param includePatterns 包含する正規表現パターンのリスト
     * @param excludePatterns 除外する正規表現パターンのリスト
     * @param isOrCondition 包含条件をOR評価するか
     * @return 適合する場合は true、それ以外は false
     */
    public static boolean isPathFilterMatched(String path, boolean includeBaseName, boolean excludeBaseName, List<String> includePatterns, List<String> excludePatterns, boolean isOrCondition) {
        return isPathFilterMatched(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, isOrCondition, 0);
    }

    /**
     * パスが指定されたフィルターパターンに一致するか判定します（isOrCondition=false, debugLevel=0）。
     *
     * @param path 評価対象のパス
     * @param includeBaseName 包含判定時にファイル名のみを使用するか
     * @param excludeBaseName 除外判定時にファイル名のみを使用するか
     * @param includePatterns 包含する正規表現パターンのリスト
     * @param excludePatterns 除外する正規表現パターンのリスト
     * @return 適合する場合は true、それ以外は false
     */
    public static boolean isPathFilterMatched(String path, boolean includeBaseName, boolean excludeBaseName, List<String> includePatterns, List<String> excludePatterns) {
        return isPathFilterMatched(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, false, 0);
    }

    /**
     * フィルターフラグを階層再帰ルールに従って結合計算します。
     *
     * @param previousEffective 親階層の判定結果フラグ
     * @param currentEffective 自階層の判定結果フラグ
     * @param isOrCondition 包含条件をOR評価するか
     * @param isIncludeHitRecursive 包含ヒットを再帰継承するか
     * @param isExcludeHitRecursive 除外ヒットを再帰継承するか
     * @return 統合されたフィルターフラグ値
     */
    public static int combineFilterFlags(int previousEffective, int currentEffective, boolean isOrCondition, boolean isIncludeHitRecursive, boolean isExcludeHitRecursive) {
        int result = currentEffective;
        switch (previousEffective) {
            case 0:
                result = currentEffective;
                break;
            case 1:
                if (isIncludeHitRecursive) {
                    result = currentEffective == 2 ? 3 : 1;
                }
                break;
            case 2:
                if (isExcludeHitRecursive) {
                    result = (currentEffective == 1 && isOrCondition) ? 1 : 2;
                }
                break;
            case 3:
                if (isExcludeHitRecursive) {
                    result = (currentEffective == 1 && isOrCondition) ? 1 : 3;
                }
                break;
            default:
                break;
        }
        return result;
    }

    /**
     * ディレクトリ内のすべてのファイルサイズの合計（バイト）を取得します。
     *
     * @param directoryPath 対象ディレクトリのパス
     * @param includeSymLinks シンボリックリンクを含めるかどうか
     * @param showExceptions 例外メッセージを表示するかどうか
     * @return 合計ファイルサイズ（バイト）
     */
    public static long getDirectoryFileSize(String directoryPath, boolean includeSymLinks, boolean showExceptions) {
        if (directoryPath == null) {
            return 0L;
        }
        return getDirectoryFileSize(new File(directoryPath), includeSymLinks, showExceptions);
    }

    /**
     * ディレクトリ内のすべてのファイルサイズの合計（バイト）を取得します。
     *
     * @param directoryInfo 対象ディレクトリ
     * @param includeSymLinks シンボリックリンクを含めるかどうか
     * @param showExceptions 例外メッセージを表示するかどうか
     * @return 合計ファイルサイズ（バイト）
     */
    public static long getDirectoryFileSize(File directoryInfo, boolean includeSymLinks, boolean showExceptions) {
        if (directoryInfo == null || !directoryInfo.exists()) {
            return 0L;
        }
        long totalSize = 0L;
        try {
            if (includeSymLinks && isSymlink(directoryInfo.getAbsolutePath())) {
                return 0L;
            }
            File[] files = directoryInfo.listFiles();
            if (files != null) {
                for (File file : files) {
                    if (file.isDirectory()) {
                        totalSize += getDirectoryFileSize(file, includeSymLinks, showExceptions);
                    } else {
                        if (!includeSymLinks || !isSymlink(file.getAbsolutePath())) {
                            totalSize += file.length();
                        }
                    }
                }
            }
        } catch (Exception ex) {
            if (showExceptions) {
                System.out.println(" => FAILED TO GET DIR SIZE(" + directoryInfo.getAbsolutePath() + ")：EXCEPTION：" + ex.getMessage());
            }
        }
        return totalSize;
    }

    /**
     * 指定されたディレクトリのアクセス権限を取得しコンソールに表示します。
     *
     * @param directoryPath 対象ディレクトリのパス
     * @param showExceptions 例外メッセージを表示するかどうか
     */
    public static void showDirPermissions(String directoryPath, boolean showExceptions) {
        System.out.println("AccessControlType,AccountName,FileSystemRights");
        if (directoryPath == null || directoryPath.isEmpty()) {
            return;
        }
        File dir = new File(directoryPath);
        if (dir.exists()) {
            System.out.println("Allow," + System.getProperty("user.name", "") + ",r=" + dir.canRead() + " w=" + dir.canWrite() + " x=" + dir.canExecute());
        }
    }

    /**
     * @deprecated {@link #showDirPermissions(String, boolean)} を使用してください。
     */
    @Deprecated
    public static void displayDirectoryPermissions(String directoryPath, boolean showExceptions) {
        showDirPermissions(directoryPath, showExceptions);
    }

    /**
     * 指定ディレクトリ内のファイルパス一覧を条件に従ってソートして返します。
     *
     * @param path 検索対象ディレクトリのパス
     * @param searchPattern 検索パターン
     * @param searchAllDirectories サブディレクトリも含めるか
     * @param sortType ソート種別（1: 名前, 2: 作成日, 3: 更新日）
     * @param isAscending 昇順でソートする場合は true
     * @param isShowFileList ファイル一覧をコンソール出力するかどうか
     * @return ソートされたファイルパスの配列
     */
    public static String[] getSortedFiles(String path, String searchPattern, boolean searchAllDirectories, int sortType, boolean isAscending, boolean isShowFileList) {
        File[] files = getSortedFilesInfo(path, searchPattern, searchAllDirectories, sortType, isAscending);
        String[] result = new String[files.length];
        for (int i = 0; i < files.length; i++) {
            result[i] = files[i].getAbsolutePath();
            if (isShowFileList) {
                System.out.println("[MdlFile.getSortedFiles(" + path + ")] " + result[i]);
            }
        }
        return result;
    }

    /**
     * 指定ディレクトリ内の File 配列を条件に従ってソートして返します。
     *
     * @param path 検索対象ディレクトリのパス
     * @param searchPattern 検索パターン（ワイルドカードまたはnull）
     * @param searchAllDirectories サブディレクトリも含めるか
     * @param sortType ソート種別
     * @param isAscending 昇順でソートする場合は true
     * @return ソートされた File の配列
     */
    public static File[] getSortedFilesInfo(String path, String searchPattern, boolean searchAllDirectories, int sortType, boolean isAscending) {
        if (path == null) {
            return new File[0];
        }
        File dir = new File(path);
        if (!dir.exists() || !dir.isDirectory()) {
            return new File[0];
        }

        List<File> fileList = new ArrayList<>();
        collectFiles(dir, searchPattern, searchAllDirectories, fileList);

        File[] files = fileList.toArray(new File[0]);
        if (sortType > 0) {
            Comparator<File> comparator = getFileComparator(sortType);
            if (!isAscending) {
                comparator = comparator.reversed();
            }
            Arrays.sort(files, comparator);
        }

        return files;
    }

    private static void collectFiles(File dir, String pattern, boolean recursive, List<File> result) {
        File[] entries = dir.listFiles();
        if (entries == null) {
            return;
        }
        Pattern regex = null;
        if (pattern != null && !pattern.isEmpty() && !pattern.equals("*") && !pattern.equals("*.*")) {
            String p = pattern.replace(".", "\\.").replace("*", ".*").replace("?", ".");
            regex = Pattern.compile("^" + p + "$", Pattern.CASE_INSENSITIVE);
        }

        for (File entry : entries) {
            if (entry.isFile()) {
                if (regex == null || regex.matcher(entry.getName()).matches()) {
                    result.add(entry);
                }
            } else if (entry.isDirectory() && recursive) {
                collectFiles(entry, pattern, true, result);
            }
        }
    }

    private static Comparator<File> getFileComparator(int sortType) {
        switch (sortType) {
            case SORT_BY_NAME:
                return Comparator.comparing(File::getName, String.CASE_INSENSITIVE_ORDER);
            case SORT_BY_CTIME:
            case SORT_BY_MTIME:
            default:
                return Comparator.comparingLong(File::lastModified);
        }
    }

    /**
     * 指定ディレクトリ内のサブディレクトリパス一覧を条件に従ってソートして返します。
     *
     * @param path 検索対象ディレクトリのパス
     * @param searchPattern 検索パターン
     * @param searchAllDirectories サブディレクトリも含めるか
     * @param sortType ソート種別
     * @param isAscending 昇順でソートする場合は true
     * @param isShowDirList ディレクトリ一覧をコンソール出力するかどうか
     * @return ソートされたディレクトリパスの配列
     */
    public static String[] getSortedDirectories(String path, String searchPattern, boolean searchAllDirectories, int sortType, boolean isAscending, boolean isShowDirList) {
        File[] dirs = getSortedDirsInfo(path, searchPattern, searchAllDirectories, sortType, isAscending);
        String[] result = new String[dirs.length];
        for (int i = 0; i < dirs.length; i++) {
            result[i] = dirs[i].getAbsolutePath();
            if (isShowDirList) {
                System.out.println("[MdlFile.getSortedDirectories(" + path + ")] " + result[i]);
            }
        }
        return result;
    }

    /**
     * 指定ディレクトリ内のサブディレクトリ File 配列を条件に従ってソートして返します。
     *
     * @param path 検索対象ディレクトリのパス
     * @param searchPattern 検索パターン
     * @param searchAllDirectories サブディレクトリも含めるか
     * @param sortType ソート種別
     * @param isAscending 昇順でソートする場合は true
     * @return ソートされたサブディレクトリ File の配列
     */
    public static File[] getSortedDirsInfo(String path, String searchPattern, boolean searchAllDirectories, int sortType, boolean isAscending) {
        if (path == null) {
            return new File[0];
        }
        File dir = new File(path);
        if (!dir.exists() || !dir.isDirectory()) {
            return new File[0];
        }

        List<File> dirList = new ArrayList<>();
        collectDirectories(dir, searchPattern, searchAllDirectories, dirList);

        File[] dirs = dirList.toArray(new File[0]);
        if (sortType > 0) {
            Comparator<File> comparator = getFileComparator(sortType);
            if (!isAscending) {
                comparator = comparator.reversed();
            }
            Arrays.sort(dirs, comparator);
        }

        return dirs;
    }

    /**
     * @deprecated {@link #getSortedDirsInfo(String, String, boolean, int, boolean)} を使用してください。
     */
    @Deprecated
    public static File[] getSortedDirectoriesInfo(String path, String searchPattern, boolean searchAllDirectories, int sortType, boolean isAscending) {
        return getSortedDirsInfo(path, searchPattern, searchAllDirectories, sortType, isAscending);
    }

    private static void collectDirectories(File dir, String pattern, boolean recursive, List<File> result) {
        File[] entries = dir.listFiles(File::isDirectory);
        if (entries == null) {
            return;
        }
        Pattern regex = null;
        if (pattern != null && !pattern.isEmpty() && !pattern.equals("*") && !pattern.equals("*.*")) {
            String p = pattern.replace(".", "\\.").replace("*", ".*").replace("?", ".");
            regex = Pattern.compile("^" + p + "$", Pattern.CASE_INSENSITIVE);
        }

        for (File entry : entries) {
            if (regex == null || regex.matcher(entry.getName()).matches()) {
                result.add(entry);
            }
            if (recursive) {
                collectDirectories(entry, pattern, true, result);
            }
        }
    }

    /**
     * ソートタイプ番号に対応する識別文字列を取得します。
     *
     * @param sortType ソートタイプ番号
     * @return ソートタイプ名（"name", "ctime", "mtime", "none"）
     */
    public static String getSortTypeName(int sortType) {
        switch (sortType) {
            case SORT_BY_NAME:
                return "name";
            case SORT_BY_CTIME:
                return "ctime";
            case SORT_BY_MTIME:
                return "mtime";
            default:
                return "none";
        }
    }

    /**
     * ソートタイプ識別文字列に対応するソートタイプ番号を取得します。
     *
     * @param name ソートタイプ識別文字列
     * @return ソートタイプ番号
     */
    public static int getSortTypeNum(String name) {
        if (name == null || name.isEmpty()) {
            return SORT_BY_NONE;
        }
        String lower = name.toLowerCase(Locale.ROOT);
        switch (lower) {
            case "name":
                return SORT_BY_NAME;
            case "ctime":
                return SORT_BY_CTIME;
            case "mtime":
                return SORT_BY_MTIME;
            default:
                return SORT_BY_NONE;
        }
    }

    /**
     * 複数のパス文字列を結合して 1 つのパスにします。
     *
     * @param path1 1 番目のパス
     * @param path2 2 番目のパス
     * @return 結合されたパス文字列
     */
    public static String combinePath(String path1, String path2) {
        if (path1 == null || path1.isEmpty()) {
            return path2 != null ? path2 : "";
        }
        if (path2 == null || path2.isEmpty()) {
            return path1;
        }
        File f1 = new File(path1);
        return new File(f1, path2).getPath();
    }
}

