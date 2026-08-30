package tool.cmnclslib.cls;

import org.junit.Before;
import org.junit.Test;
import tool.cmnclslib.mdl.MdlApp;
import static org.junit.Assert.*;

/**
 * ClsCmdExec の単体テストクラスです。
 */
public class UnitTest_ClsCmdExec {

    private ClsLogger logger = new ClsLogger();
    private ClsCmdExec cmdExec;

    @Before
    public void setUp() {
        cmdExec = new ClsCmdExec(logger);
    }

    @Test
    public void execute_標準コマンドが正常終了すること() {
        if (MdlApp.isWindows()) {
            String comSpec = System.getenv("ComSpec");
            cmdExec.setCmdPath(comSpec != null && !comSpec.isEmpty() ? comSpec : "cmd.exe");
            cmdExec.setCmdArgs("/c echo test");
        } else {
            cmdExec.setCmdPath("sh");
            cmdExec.setCmdArgs("-c echo test");
        }
        assertEquals(0, cmdExec.executeThread(3));
    }

    @Test
    public void execute_存在しないコマンドを実行した場合異常終了すること() {
        if (MdlApp.isWindows()) {
            String comSpec = System.getenv("ComSpec");
            cmdExec.setCmdPath(comSpec != null && !comSpec.isEmpty() ? comSpec : "cmd.exe");
            cmdExec.setCmdArgs("/c non_existent_command_xyz_12345");
            assertNotEquals(0, cmdExec.executeThread(3));
        } else {
            cmdExec.setCmdPath("sh");
            cmdExec.setCmdArgs("-c non_existent_command_xyz_12345");
            assertNotEquals(0, cmdExec.executeThread(3));
        }
    }
}
