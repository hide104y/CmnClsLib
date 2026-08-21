package tool.cmnclslib.cls;

import java.io.File;
import org.junit.Before;
import org.junit.Test;
import tool.cmnclslib.mdl.MdlConst;
import tool.cmnclslib.mdl.MdlFile;
import static org.junit.Assert.*;

/**
 * ClsLogger の単体テストクラスです。
 */
public class UnitTest_ClsLogger {

    private ClsLogger logger = new ClsLogger();
    private String logPath;

    @Before
    public void setUp() {
        java.nio.file.Path tempDirPath = java.nio.file.Paths.get(System.getProperty("java.io.tmpdir"), "UnitTest", "CmnClsLib", "ClsLogger");
        MdlFile.createDirectory(tempDirPath.toString());
        String tempDir = tempDirPath.toString();
        logPath = new File(tempDir, "ClsLog.log").getPath();
        logger.setValueByKey(ClsLogger.IS_FILE, "true");
        logger.setValueByKey(ClsLogger.DIR, tempDir);
        logger.setValueByKey(ClsLogger.PATH, logPath);
    }

    @Test
    public void writeLine_引数の内容が出力されること() {
        if (MdlFile.getPathType(logger.getValueByKey(ClsLogger.PATH, "")) == MdlFile.PATH_IS_FILE) {
            new File(logger.getValueByKey(ClsLogger.PATH, "")).delete();
        }

        logger.writeLine(MdlConst.LVL_NONE, "OK");

        assertEquals("OK", MdlFile.readFile(logger.getValueByKey(ClsLogger.PATH, ""), -1).trim());

        if (MdlFile.getPathType(logger.getValueByKey(ClsLogger.PATH, "")) == MdlFile.PATH_IS_FILE) {
            new File(logger.getValueByKey(ClsLogger.PATH, "")).delete();
        }
    }
}
