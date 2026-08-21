package tool.cmnclslib.mdl;

import org.junit.Test;
import static org.junit.Assert.*;

/**
 * MdlLog の単体テストクラスです。
 */
public class UnitTest_MdlLog {

    @Test
    public void parseLogLevel_引数がnoneの場合は定数値LVL_NONEを返却すること() {
        int expected = MdlConst.LVL_NONE;
        int actual = MdlLog.parseLogLevel("none");
        assertEquals(expected, actual);
    }

    @Test
    public void parseLogLevel_引数がdebugの場合は定数値LVL_DEBUGを返却すること() {
        int expected = MdlConst.LVL_DEBUG;
        int actual = MdlLog.parseLogLevel("debug");
        assertEquals(expected, actual);
    }

    @Test
    public void parseLogLevel_引数がinfoの場合は定数値LVL_Iを返却すること() {
        int expected = MdlConst.LVL_I;
        int actual = MdlLog.parseLogLevel("info");
        assertEquals(expected, actual);
    }

    @Test
    public void parseLogLevel_引数がwarnの場合は定数値LVL_Wを返却すること() {
        int expected = MdlConst.LVL_W;
        int actual = MdlLog.parseLogLevel("warn");
        assertEquals(expected, actual);
    }

    @Test
    public void parseLogLevel_引数がerrorの場合は定数値LVL_Eを返却すること() {
        int expected = MdlConst.LVL_E;
        int actual = MdlLog.parseLogLevel("error");
        assertEquals(expected, actual);
    }

    @Test
    public void logLevelToString_引数が定数値LVL_NONEの場合はを文字列none返却すること() {
        String expected = "none";
        String actual = MdlLog.logLevelToString(MdlConst.LVL_NONE);
        assertEquals(expected, actual);
    }

    @Test
    public void logLevelToString_引数が定数値LVL_DEBUGの場合はを文字列debug返却すること() {
        String expected = "debug";
        String actual = MdlLog.logLevelToString(MdlConst.LVL_DEBUG);
        assertEquals(expected, actual);
    }

    @Test
    public void logLevelToString_引数が定数値LVL_Iの場合はを文字列info返却すること() {
        String expected = "info";
        String actual = MdlLog.logLevelToString(MdlConst.LVL_I);
        assertEquals(expected, actual);
    }

    @Test
    public void logLevelToString_引数が定数値LVL_Wの場合はを文字列warn返却すること() {
        String expected = "warn";
        String actual = MdlLog.logLevelToString(MdlConst.LVL_W);
        assertEquals(expected, actual);
    }

    @Test
    public void logLevelToString_引数が定数値LVL_Eの場合はを文字列error返却すること() {
        String expected = "error";
        String actual = MdlLog.logLevelToString(MdlConst.LVL_E);
        assertEquals(expected, actual);
    }

    @Test
    public void getLogLevelPrefix_引数が定数値LVL_NONEの場合はを空文字列返却すること() {
        String expected = "";
        String actual = MdlLog.getLogLevelPrefix(MdlConst.LVL_NONE);
        assertEquals(expected, actual);
    }

    @Test
    public void getLogLevelPrefix_引数が定数値LVL_DEBUGの場合はを文字列DEBUG空白を返却すること() {
        String expected = "DEBUG ";
        String actual = MdlLog.getLogLevelPrefix(MdlConst.LVL_DEBUG);
        assertEquals(expected, actual);
    }

    @Test
    public void getLogLevelPrefix_引数が定数値LVL_Iの場合はを文字列INFO空白を返却すること() {
        String expected = "INFO ";
        String actual = MdlLog.getLogLevelPrefix(MdlConst.LVL_I);
        assertEquals(expected, actual);
    }

    @Test
    public void getLogLevelPrefix_引数が定数値LVL_Wの場合はを文字列WARN空白を返却すること() {
        String expected = "WARN ";
        String actual = MdlLog.getLogLevelPrefix(MdlConst.LVL_W);
        assertEquals(expected, actual);
    }

    @Test
    public void getLogLevelPrefix_引数が定数値LVL_Eの場合はを文字列ERROR空白を返却すること() {
        String expected = "ERROR ";
        String actual = MdlLog.getLogLevelPrefix(MdlConst.LVL_E);
        assertEquals(expected, actual);
    }
}
