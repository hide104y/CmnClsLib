package tool.cmnclslib.mdl;

import org.junit.Test;
import static org.junit.Assert.*;

/**
 * MdlApp の単体テストクラスです。
 */
public class UnitTest_MdlApp {

    @Test
    public void isWindows_OS環境の判定結果を返すこと() {
        boolean actual = MdlApp.isWindows();
        String os = System.getProperty("os.name").toLowerCase();
        boolean expected = os.contains("win");
        assertEquals(expected, actual);
    }

    @Test
    public void getOsName_OS名を返すこと() {
        String actual = MdlApp.getOsName();
        assertFalse(actual.isEmpty());
        assertTrue(actual.equals("Windows") || actual.equals("Linux") || actual.equals("OSX") || actual.equals("FreeBSD") || actual.equals("UNKNOWN"));
    }

    @Test
    public void getProcessArchitecture_プロセスのアーキテクチャを返すこと() {
        String actual = MdlApp.getProcessArchitecture();
        assertFalse(actual.isEmpty());
    }

    @Test
    public void getOsArchitecture_OSのアーキテクチャを返すこと() {
        String actual = MdlApp.getOsArchitecture();
        assertFalse(actual.isEmpty());
    }

    @Test
    public void getOsDescription_OSの詳細説明文字列を返すこと() {
        String actual = MdlApp.getOsDescription();
        assertFalse(actual.isEmpty());
    }

    @Test
    public void getOsVersion_OSのバージョン文字列を返すこと() {
        String actual = MdlApp.getOsVersion();
        assertFalse(actual.isEmpty());
    }

    @Test
    public void getOsMajorVersion_OSのメジャーバージョンを返すこと() {
        int actual = MdlApp.getOsMajorVersion();
        assertTrue(actual > 0);
    }

    @Test
    public void getPlatform_プラットフォーム識別子を返すこと() {
        String actual = MdlApp.getPlatform();
        assertFalse(actual.isEmpty());
    }

    @Test
    public void getFrameworkDescription_フレームワークの説明文字列を返すこと() {
        String actual = MdlApp.getFrameworkDescription();
        assertFalse(actual.isEmpty());
    }

    @Test
    public void getRuntimeClrVersion_ランタイムバージョンを返すこと() {
        String actual = MdlApp.getRuntimeClrVersion();
        assertFalse(actual.isEmpty());
    }

    @Test
    public void getBuildClrVersion_ビルドバージョンを返すこと() {
        String actual = MdlApp.getBuildClrVersion();
        assertFalse(actual.isEmpty());
    }

    @Test
    public void getExeFilePath_実行ファイルのパスを返すこと() {
        String actual = MdlApp.getExeFilePath();
        assertNotNull(actual);
    }

    @Test
    public void getExeFileVersion_実行ファイルのバージョンを返すこと() {
        String actual = MdlApp.getExeFileVersion();
        assertNotNull(actual);
    }

    @Test
    public void getExeName_実行ファイル名を返すこと() {
        String actual = MdlApp.getExeName();
        assertNotNull(actual);
    }

    @Test
    public void getAppName_アプリケーション名を返すこと() {
        String actual = MdlApp.getAppName();
        assertNotNull(actual);
    }

    @Test
    public void getAppNameWithHostName_アプリ名と小文字のホスト名を結合した文字列を返すこと() {
        String actual = MdlApp.getAppNameWithHostName();
        String expectedAppName = MdlApp.getAppName();
        String host = "";
        try {
            host = java.net.InetAddress.getLocalHost().getHostName().toLowerCase(java.util.Locale.ROOT);
        } catch (Exception e) {
            host = "localhost";
        }
        assertEquals(expectedAppName + "_" + host, actual);
    }

    @Test
    public void getPriorityName_優先度クラス名文字列を正しく変換すること() {
        assertEquals("BelowNormal", MdlApp.getPriorityName(MdlApp.ProcessPriorityClass.BELOW_NORMAL));
        assertEquals("High", MdlApp.getPriorityName(MdlApp.ProcessPriorityClass.HIGH));
        assertEquals("Idle", MdlApp.getPriorityName(MdlApp.ProcessPriorityClass.IDLE));
        assertEquals("Normal", MdlApp.getPriorityName(MdlApp.ProcessPriorityClass.NORMAL));
        assertEquals("RealTime", MdlApp.getPriorityName(MdlApp.ProcessPriorityClass.REAL_TIME));
        assertEquals("AboveNormal", MdlApp.getPriorityName(MdlApp.ProcessPriorityClass.ABOVE_NORMAL));
    }

    @Test
    public void getPriorityClassFromString_文字列から優先度クラスへ正しく変換すること() {
        assertEquals(MdlApp.ProcessPriorityClass.NORMAL, MdlApp.getPriorityClassFromString(null));
        assertEquals(MdlApp.ProcessPriorityClass.NORMAL, MdlApp.getPriorityClassFromString(""));
        assertEquals(MdlApp.ProcessPriorityClass.ABOVE_NORMAL, MdlApp.getPriorityClassFromString("abovenormal"));
        assertEquals(MdlApp.ProcessPriorityClass.ABOVE_NORMAL, MdlApp.getPriorityClassFromString("ABOVENORMAL"));
        assertEquals(MdlApp.ProcessPriorityClass.IDLE, MdlApp.getPriorityClassFromString("idle"));
        assertEquals(MdlApp.ProcessPriorityClass.HIGH, MdlApp.getPriorityClassFromString("high"));
        assertEquals(MdlApp.ProcessPriorityClass.REAL_TIME, MdlApp.getPriorityClassFromString("realtime"));
        assertEquals(MdlApp.ProcessPriorityClass.BELOW_NORMAL, MdlApp.getPriorityClassFromString("belownormal"));
        assertEquals(MdlApp.ProcessPriorityClass.NORMAL, MdlApp.getPriorityClassFromString("invalid_value"));
    }

    @Test
    public void 新設メソッド群が正しく動作すること() {
        assertNotNull(MdlApp.getJavaVersion());
        assertNotNull(MdlApp.getProcessArch());
        assertNotNull(MdlApp.getFrameworkDesc());
        assertNotNull(MdlApp.getAppNameWithHost());
        assertNotNull(MdlApp.getRefPackagesMap(UnitTest_MdlApp.class, ""));
        assertEquals(MdlApp.ProcessPriorityClass.HIGH, MdlApp.parsePriorityClass("high"));
    }
}
