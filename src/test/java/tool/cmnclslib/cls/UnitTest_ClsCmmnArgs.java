package tool.cmnclslib.cls;

import java.io.File;
import java.util.HashMap;
import java.util.Map;
import org.junit.Test;
import tool.cmnclslib.ifc.ICmnLogger;
import tool.cmnclslib.mdl.MdlConst;
import tool.cmnclslib.mdl.MdlFile;
import static org.junit.Assert.*;

/**
 * ClsCmmnArgs の単体テストクラスです。
 */
public class UnitTest_ClsCmmnArgs {

    private final String tempDir = new File(new File(new File(System.getProperty("java.io.tmpdir"), "UnitTest"), "CmnClsLib"), "ClsCmmnArgs").getPath();

    private ClsCmmnArgs createArgs() {
        ICmnLogger logger = new ClsLogger();
        return new ClsCmmnArgs(logger);
    }

    @Test
    public void testInitializeLists() {
        ClsCmmnArgs cmmnArgs = createArgs();
        cmmnArgs.initializeLists();
        assertTrue(cmmnArgs.getKeyNameOfUsernameList().contains("username"));
        assertTrue(cmmnArgs.getKeyNameOfPasswordList().contains("password"));
    }

    @Test
    public void testGetModuleInfo() {
        ClsCmmnArgs cmmnArgs = createArgs();
        boolean result = cmmnArgs.getModuleInfo();
        assertTrue(result);
        assertFalse(cmmnArgs.getExeBaseName().isEmpty());
        assertTrue(cmmnArgs.getPid() > 0);
    }

    @Test
    public void testSplitUserAndDomain() {
        ClsCmmnArgs cmmnArgs = createArgs();
        cmmnArgs.setUsername("TESTDOMAIN\\TestUser");
        cmmnArgs.splitUserAndDomain();

        assertEquals("TESTDOMAIN", cmmnArgs.getDomainName());
        assertEquals("TestUser", cmmnArgs.getUsernameWithoutDomain());
    }

    @Test
    public void testGetArgsForUser() {
        ClsCmmnArgs cmmnArgs = createArgs();
        Map<String, String> map = new HashMap<>();
        map.put("u", "AdminUser");
        map.put("domain", "CORPDOMAIN");
        cmmnArgs.setNamedArgs(map);

        boolean ok = cmmnArgs.getArgsForUser();
        assertTrue(ok);
        assertEquals("AdminUser", cmmnArgs.getUsernameWithoutDomain());
        assertEquals("CORPDOMAIN", cmmnArgs.getDomainName());
    }

    @Test
    public void testReplaceByDictionary() {
        ClsCmmnArgs cmmnArgs = createArgs();
        cmmnArgs.getReplaceDictionary().put("__ENV_ID__", "PRODUCTION");

        String replaced = cmmnArgs.replaceByDictionary("C:\\Data\\__ENV_ID__\\log.txt");
        assertEquals("C:\\Data\\PRODUCTION\\log.txt", replaced);
    }

    @Test
    public void testGetPathParam() {
        MdlFile.deleteRecursively(tempDir);

        ClsCmmnArgs cmmnArgs = createArgs();
        cmmnArgs.getNamedArgs().put("ldir", tempDir);

        String path = cmmnArgs.getPathParam("ldir", MdlFile.PATH_IS_DIRECTORY, true);
        assertEquals(tempDir, path);
        assertTrue(new File(tempDir).exists());

        MdlFile.deleteRecursively(tempDir);
    }

    @Test
    public void testGetCommonArgs() {
        ClsCmmnArgs cmmnArgs = createArgs();
        Map<String, String> map = new HashMap<>();
        map.put("v", "3");
        map.put("force", "");
        map.put("diff", "2");
        cmmnArgs.setNamedArgs(map);

        boolean ok = cmmnArgs.getCommonArgs();
        assertTrue(ok);
        assertEquals(3, cmmnArgs.getVerbose());
        assertTrue(cmmnArgs.isForce());
        assertTrue(cmmnArgs.isDiff());
        assertEquals(2, cmmnArgs.getDiffLevel());
    }

    @Test
    public void testDecryptKeyAndPassword() {
        ClsCmmnArgs cmmnArgs = createArgs();
        cmmnArgs.setDecodeKey(false);
        cmmnArgs.setDecodePasswd(false);

        boolean ok = cmmnArgs.decryptKeyAndPassword();
        assertTrue(ok);
    }

    @Test
    public void testWriteLine() {
        ClsCmmnArgs cmmnArgs = createArgs();
        cmmnArgs.writeLine(MdlConst.LVL_NONE, "Test message");
    }

    @Test
    public void testShowUsage() {
        ClsCmmnArgs cmmnArgs = createArgs();
        cmmnArgs.showUsage();
    }

    @Test
    public void testNewModernMethods() {
        ClsCmmnArgs cmmnArgs = createArgs();
        cmmnArgs.setUserNoDomain("testuser");
        assertEquals("testuser", cmmnArgs.getUserNoDomain());

        cmmnArgs.getUserKeyNames().add("custom_user");
        assertTrue(cmmnArgs.getUserKeyNames().contains("custom_user"));

        cmmnArgs.getPassKeyNames().add("custom_pass");
        assertTrue(cmmnArgs.getPassKeyNames().contains("custom_pass"));

        cmmnArgs.getEncPassKeyNames().add("custom_ep");
        assertTrue(cmmnArgs.getEncPassKeyNames().contains("custom_ep"));

        cmmnArgs.getEncKeyNames().add("custom_key");
        assertTrue(cmmnArgs.getEncKeyNames().contains("custom_key"));

        cmmnArgs.getEncEncKeyNames().add("custom_ek");
        assertTrue(cmmnArgs.getEncEncKeyNames().contains("custom_ek"));

        cmmnArgs.getEncKeySizeNames().add("custom_s");
        assertTrue(cmmnArgs.getEncKeySizeNames().contains("custom_s"));

        cmmnArgs.getReplaceMap().put("__TEST__", "REPLACED");
        assertEquals("REPLACED", cmmnArgs.replaceByMap("__TEST__"));

        cmmnArgs.getShortMap().put("custom", "CUSTOM");
        assertEquals("CUSTOM", cmmnArgs.getShortMap().get("custom"));

        cmmnArgs.getAuthDefMap().put("key", "val");
        assertEquals("val", cmmnArgs.getAuthDefMap().get("key"));

        assertTrue(cmmnArgs.decryptKeyAndPass());
    }
}
