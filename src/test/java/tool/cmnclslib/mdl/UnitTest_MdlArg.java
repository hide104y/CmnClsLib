package tool.cmnclslib.mdl;

import java.util.Map;
import org.junit.Before;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 * MdlArg の単体テストクラスです。
 */
public class UnitTest_MdlArg {

    private Map<String, String> namedArgs;
    private String[] aryArgs = new String[] {"-path", "C:\\Tool\\Log", "-v", "--level", "3", "--etc", "---three", "--minus", "-3.4"};

    @Before
    public void setUp() {
        namedArgs = MdlArg.getNamedArgs(aryArgs);
    }

    @Test
    public void getNamedArgs_引数の配列をディクショナリーへの変換数が正しいこと() {
        int expected = 5;
        assertEquals(expected, namedArgs.size());
    }

    @Test
    public void isExistParam_引数にpathを指定した場合はtrueが返却されること() {
        boolean actual = MdlArg.containsKey(namedArgs, "path");
        assertTrue(actual);
    }

    @Test
    public void isExistParam_引数にvを指定した場合はtrueが返却されること() {
        boolean actual = MdlArg.containsKey(namedArgs, "v");
        assertTrue(actual);
    }

    @Test
    public void isExistParam_引数にハイフンlevelを指定した場合はtrueが返却されること() {
        boolean actual = MdlArg.containsKey(namedArgs, "level");
        assertTrue(actual);
    }

    @Test
    public void isExistParam_引数にハイフンnotexistを指定した場合はfalseが返却されること() {
        boolean actual = MdlArg.containsKey(namedArgs, "notexist");
        assertFalse(actual);
    }

    @Test
    public void getValByKey_引数にpathを指定した場合は指定のパスが返却されること() {
        String expected = "C:\\Tool\\Log";
        String actual = MdlArg.getValue(namedArgs, "path");
        assertEquals(expected, actual);
    }

    @Test
    public void getValByKey_引数にvを指定した場合は空白が返却されること() {
        String expected = "";
        String actual = MdlArg.getValue(namedArgs, "v");
        assertEquals(expected, actual);
    }

    @Test
    public void getValByKey_引数にハイフンlevelを指定した場合は3が返却されること() {
        String expected = "3";
        String actual = MdlArg.getValue(namedArgs, "level");
        assertEquals(expected, actual);
    }

    @Test
    public void getValByKey_引数にハイフンetcを指定した場合はハイフンハイフンハイフンthreeが返却されること() {
        String expected = "---three";
        String actual = MdlArg.getValue(namedArgs, "etc");
        assertEquals(expected, actual);
    }

    @Test
    public void getValByKey_引数にハイフンminusを指定した場合はハイフン3dot4が返却されること() {
        String expected = "-3.4";
        String actual = MdlArg.getValue(namedArgs, "minus");
        assertEquals(expected, actual);
    }

    @Test
    public void getValByKey_引数にハイフンnotexistを指定した場合はEmptyが返却されること() {
        String expected = "";
        String actual = MdlArg.getValue(namedArgs, "notexist");
        assertEquals(expected, actual);
    }

    @Test
    public void getFullPath_引数にpathを指定した場合は指定のパスが返却されること() {
        String actual = MdlArg.getFullPath(namedArgs, "path");
        assertNotNull(actual);
        assertTrue(actual.contains("Log") || actual.contains("Tool"));
    }
}
