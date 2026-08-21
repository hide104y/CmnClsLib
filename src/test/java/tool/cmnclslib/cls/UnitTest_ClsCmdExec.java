package tool.cmnclslib.cls;

import org.junit.Before;
import org.junit.Test;
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
    public void execute_ipconfigが正常終了すること() {
        String comSpec = System.getenv("ComSpec");
        if (comSpec == null || comSpec.isEmpty()) {
            comSpec = "cmd.exe";
        }
        cmdExec.setCmdPath(comSpec);
        cmdExec.setCmdArgs("/c ipconfig /all");
        assertEquals(0, cmdExec.executeThread(3));
    }

    @Test
    public void execute_存在しないコマンドを実行した場合異常終了すること() {
        String comSpec = System.getenv("ComSpec");
        if (comSpec == null || comSpec.isEmpty()) {
            comSpec = "cmd.exe";
        }
        cmdExec.setCmdPath(comSpec);
        cmdExec.setCmdArgs("/c ifconfig /all");
        assertEquals(1, cmdExec.executeThread(3));
    }
}
