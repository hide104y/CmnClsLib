package tool.cmnclslib.mdl;

import java.io.File;
import java.net.InetAddress;
import java.util.Locale;
import java.util.SortedMap;
import java.util.TreeMap;

/**
 * アプリケーション実行環境、OS、プロセス情報等を取得するモジュールクラスです。
 */
public final class MdlApp {

    /**
     * プロセスの優先度を表す列挙型です。
     */
    public enum ProcessPriorityClass {
        NORMAL,
        IDLE,
        HIGH,
        REAL_TIME,
        BELOW_NORMAL,
        ABOVE_NORMAL
    }

    private MdlApp() {
        // インスタンス化防止
    }

    /**
     * 現在の実行環境のOSが Windows であるかどうかを判定します。
     *
     * @return Windows の場合は true、それ以外は false
     */
    public static boolean isWindows() {
        String os = System.getProperty("os.name", "");
        return os.toLowerCase(Locale.ROOT).contains("win");
    }

    /**
     * 現在実行中のオペレーティングシステム（OS）の名前を取得します。
     *
     * @return OS名（"Windows", "Linux", "OSX", "FreeBSD", または "UNKNOWN"）
     */
    public static String getOsName() {
        String os = System.getProperty("os.name", "").toLowerCase(Locale.ROOT);
        if (os.contains("win")) {
            return "Windows";
        }
        if (os.contains("linux")) {
            return "Linux";
        }
        if (os.contains("mac") || os.contains("darwin") || os.contains("osx")) {
            return "OSX";
        }
        if (os.contains("freebsd")) {
            return "FreeBSD";
        }
        return "UNKNOWN";
    }

    /**
     * 現在実行中のプロセスのアーキテクチャを取得します。
     *
     * @return プロセスのアーキテクチャ名（例: "X64", "Arm64"）
     */
    public static String getProcessArch() {
        String arch = System.getProperty("os.arch", "");
        if (arch.equalsIgnoreCase("amd64") || arch.equalsIgnoreCase("x86_64")) {
            return "X64";
        }
        if (arch.equalsIgnoreCase("x86") || arch.equalsIgnoreCase("i386")) {
            return "X86";
        }
        if (arch.equalsIgnoreCase("aarch64") || arch.equalsIgnoreCase("arm64")) {
            return "Arm64";
        }
        return arch;
    }

    /**
     * @deprecated {@link #getProcessArch()} を使用してください。
     */
    @Deprecated
    public static String getProcessArchitecture() {
        return getProcessArch();
    }

    /**
     * オペレーティングシステム（OS）のアーキテクチャを取得します。
     *
     * @deprecated {@link #getProcessArch()} を使用してください。
     * @return OSのアーキテクチャ名（例: "X64", "Arm64"）
     */
    @Deprecated
    public static String getOsArchitecture() {
        return getProcessArch();
    }

    /**
     * オペレーティングシステム（OS）の詳細な説明文字列を取得します。
     *
     * @return OSの詳細説明文字列（例: "Microsoft Windows 10.0"）
     */
    public static String getOsDescription() {
        return System.getProperty("os.name", "") + " " + System.getProperty("os.version", "");
    }

    /**
     * オペレーティングシステム（OS）のバージョン文字列を取得します。
     *
     * @return OSのバージョン文字列
     */
    public static String getOsVersion() {
        return System.getProperty("os.version", "");
    }

    /**
     * オペレーティングシステム（OS）のメジャーバージョン番号を取得します。
     *
     * @return OSのメジャーバージョン番号
     */
    public static int getOsMajorVersion() {
        String ver = System.getProperty("os.version", "0");
        String majorStr = ver.split("[^0-9]")[0];
        return MdlUtil.parseInt(majorStr, 1);
    }

    /**
     * 現在のプラットフォーム識別子を取得します。
     *
     * @return プラットフォーム識別文字列（例: "Win32NT", "Windows", "Linux"）
     */
    public static String getPlatform() {
        if (isWindows()) {
            return "Win32NT";
        }
        return getOsName();
    }

    /**
     * 指定されたクラス/パッケージが参照しているパッケージ情報のマップを取得します。
     *
     * @param targetClass 対象のクラスオブジェクト
     * @param publicKeyToken フィルタリング条件（"ALL" で全件、null または空文字でトークン無し、または指定されたトークン文字列）
     * @return インデックスと参照名のソート済みマップ
     */
    public static SortedMap<Integer, String> getRefPackagesMap(Class<?> targetClass, String publicKeyToken) {
        SortedMap<Integer, String> referencedAssemblies = new TreeMap<>();
        if (targetClass == null) {
            return referencedAssemblies;
        }

        try {
            Package[] packages = Package.getPackages();
            for (int i = 0; i < packages.length; i++) {
                String name = packages[i].getName();
                boolean shouldAdd = false;
                if (publicKeyToken == null || publicKeyToken.isEmpty()) {
                    shouldAdd = true;
                } else if ("ALL".equalsIgnoreCase(publicKeyToken)) {
                    shouldAdd = true;
                } else if (name.contains(publicKeyToken)) {
                    shouldAdd = true;
                }

                if (shouldAdd) {
                    referencedAssemblies.put(i, name);
                }
            }
        } catch (Exception e) {
            // 例外発生時は取得できた範囲を返す
        }
        return referencedAssemblies;
    }

    /**
     * @deprecated {@link #getRefPackagesMap(Class, String)} を使用してください。
     */
    @Deprecated
    public static SortedMap<Integer, String> getReferencedAssembliesDictionary(Class<?> targetClass, String publicKeyToken) {
        return getRefPackagesMap(targetClass, publicKeyToken);
    }

    /**
     * 現在実行中のフレームワークの説明文字列を取得します。
     *
     * @return フレームワークの説明文字列（例: "Java HotSpot(TM) 64-Bit Server VM 11.0.1"）
     */
    public static String getFrameworkDesc() {
        return System.getProperty("java.vm.name", "") + " " + System.getProperty("java.version", "");
    }

    /**
     * @deprecated {@link #getFrameworkDesc()} を使用してください。
     */
    @Deprecated
    public static String getFrameworkDescription() {
        return getFrameworkDesc();
    }

    /**
     * 現在実行中のJavaランタイムバージョンを取得します。
     *
     * @return ランタイムバージョン文字列
     */
    public static String getJavaVersion() {
        return System.getProperty("java.version", "");
    }

    /**
     * @deprecated {@link #getJavaVersion()} を使用してください。
     */
    @Deprecated
    public static String getRuntimeClrVersion() {
        return getJavaVersion();
    }

    /**
     * @deprecated {@link #getJavaVersion()} を使用してください。
     */
    @Deprecated
    public static String getBuildClrVersion() {
        return getJavaVersion();
    }

    /**
     * 現在のプロセスの実行ファイルのフルパスを取得します。
     *
     * @return 実行ファイルのフルパス。取得できない場合は空文字列
     */
    public static String getExeFilePath() {
        try {
            String path = MdlApp.class.getProtectionDomain().getCodeSource().getLocation().toURI().getPath();
            if (path != null) {
                File file = new File(path);
                return file.getAbsolutePath();
            }
        } catch (Exception e) {
            // 取得失敗時
        }
        return System.getProperty("java.home", "") + "\\bin\\java.exe";
    }

    /**
     * 現在のプロセスの実行ファイルのファイルバージョン文字列を取得します。
     *
     * @return ファイルバージョン文字列。取得できない場合は空文字列
     */
    public static String getExeFileVersion() {
        Package pkg = MdlApp.class.getPackage();
        if (pkg != null && pkg.getImplementationVersion() != null) {
            return pkg.getImplementationVersion();
        }
        return System.getProperty("java.version", "1.0.0.0");
    }

    /**
     * 現在のプロセスの実行ファイル名（拡張子なし）を取得します。
     *
     * @return 実行ファイル名（拡張子なし）
     */
    public static String getExeName() {
        String fullPath = getExeFilePath();
        File file = new File(fullPath);
        String name = file.getName();
        int dotIndex = name.lastIndexOf('.');
        return (dotIndex > 0) ? name.substring(0, dotIndex) : name;
    }

    /**
     * 現在実行中のクラス/JARの場所（パス）を取得します。
     *
     * @deprecated {@link #getExeFilePath()} を使用してください。
     * @return ファイルパス。取得できない場合は空文字列
     */
    @Deprecated
    public static String getAssemblyLocation() {
        return getExeFilePath();
    }

    /**
     * 現在実行中のアセンブリ/モジュール名を取得します。
     *
     * @return アセンブリ名
     */
    public static String getAssemblyName() {
        return "CmnClsLib";
    }

    /**
     * 現在実行中のバージョン文字列を取得します。
     *
     * @deprecated {@link #getExeFileVersion()} を使用してください。
     * @return バージョン文字列
     */
    @Deprecated
    public static String getAssemblyVersion() {
        return getExeFileVersion();
    }

    /**
     * アプリケーション名（実行ファイル名から末尾の数字を除去したもの）を取得します。
     *
     * @return 整形されたアプリケーション名
     */
    public static String getAppName() {
        return MdlUtil.trimNumberRight(getExeName());
    }

    /**
     * アプリケーション名と小文字に変換されたホスト名を結合した文字列を取得します。
     *
     * @return "アプリケーション名_ホスト名" の形式の文字列
     */
    public static String getAppNameWithHost() {
        String hostName = "";
        try {
            hostName = InetAddress.getLocalHost().getHostName().toLowerCase(Locale.ROOT);
        } catch (Exception e) {
            hostName = "localhost";
        }
        return getAppName() + "_" + hostName;
    }

    /**
     * @deprecated {@link #getAppNameWithHost()} を使用してください。
     */
    @Deprecated
    public static String getAppNameWithHostName() {
        return getAppNameWithHost();
    }

    /**
     * プロセスの優先度クラスに対応する名前文字列を取得します。
     *
     * @param priorityClass プロセスの優先度クラス
     * @return 優先度クラスの名前文字列（例: "Normal", "High"）
     */
    public static String getPriorityName(ProcessPriorityClass priorityClass) {
        if (priorityClass == null) {
            return "AboveNormal";
        }
        switch (priorityClass) {
            case BELOW_NORMAL:
                return "BelowNormal";
            case HIGH:
                return "High";
            case IDLE:
                return "Idle";
            case NORMAL:
                return "Normal";
            case REAL_TIME:
                return "RealTime";
            case ABOVE_NORMAL:
            default:
                return "AboveNormal";
        }
    }

    /**
     * 文字列からプロセスの優先度クラス列挙型を取得します。
     *
     * @param priority 優先度クラスを表す文字列（例: "normal", "high"）
     * @return 対応する ProcessPriorityClass。該当しない場合は ProcessPriorityClass.NORMAL
     */
    public static ProcessPriorityClass parsePriorityClass(String priority) {
        if (priority == null || priority.trim().isEmpty()) {
            return ProcessPriorityClass.NORMAL;
        }

        String lower = priority.trim().toLowerCase(Locale.ROOT);
        switch (lower) {
            case "abovenormal":
                return ProcessPriorityClass.ABOVE_NORMAL;
            case "idle":
                return ProcessPriorityClass.IDLE;
            case "high":
                return ProcessPriorityClass.HIGH;
            case "realtime":
                return ProcessPriorityClass.REAL_TIME;
            case "belownormal":
                return ProcessPriorityClass.BELOW_NORMAL;
            case "normal":
            default:
                return ProcessPriorityClass.NORMAL;
        }
    }

    /**
     * @deprecated {@link #parsePriorityClass(String)} を使用してください。
     */
    @Deprecated
    public static ProcessPriorityClass getPriorityClassFromString(String priority) {
        return parsePriorityClass(priority);
    }
}
