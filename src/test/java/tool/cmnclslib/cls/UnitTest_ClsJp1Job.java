package tool.cmnclslib.cls;

import org.junit.Before;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 * ClsJp1Job の単体テストクラスです。
 */
public class UnitTest_ClsJp1Job {

    private ClsLogger logger = new ClsLogger();
    private ClsJp1Job jp1;

    @Before
    public void setUp() {
        jp1 = new ClsJp1Job(logger);
        jp1.setJobName("/ENV.PROD/JOB_SQLF.list_schema.sql__ID_A.10__ID_B.20.sq__ID_C.30.dq/I52.ADサーバ再起動/I22.再起動.RHOST.WEB-SV01..RTARGET.WEB-AP01/AAA/JOB_SQLF.replace.sql__BBB");
    }

    @Test
    public void 環境変数をセットできること() {
        assertTrue(jp1.setEnvironmentVariable());
    }

    @Test
    public void convertStringFromEnvironmentでENVを指定した場合値はPRODであること() {
        jp1.setEnvironmentVariable();
        assertEquals("PROD", jp1.convertStringFromEnvironment("AJSENV.ENV"));
    }

    @Test
    public void convertStringFromEnvironmentでID_Aを指定した場合値は10であること() {
        jp1.setEnvironmentVariable();
        assertEquals("10", jp1.convertStringFromEnvironment("AJSENV.ID_A"));
    }

    @Test
    public void convertStringFromEnvironmentでID_Bを指定した場合値は20であること() {
        jp1.setEnvironmentVariable();
        assertEquals("'20'", jp1.convertStringFromEnvironment("AJSENV.ID_B"));
    }

    @Test
    public void convertStringFromEnvironmentでID_Cを指定した場合値は30であること() {
        jp1.setEnvironmentVariable();
        assertEquals("\"30\"", jp1.convertStringFromEnvironment("AJSENV.ID_C"));
    }

    @Test
    public void convertStringFromEnvironmentでID_Bを指定した場合値はシングルクォーテーションなし20でないこと() {
        jp1.setEnvironmentVariable();
        assertNotEquals("20", jp1.convertStringFromEnvironment("AJSENV.ID_B"));
    }

    @Test
    public void convertStringFromEnvironmentでID_Cを指定した場合値はダブルクォーテーションなし30でないこと() {
        jp1.setEnvironmentVariable();
        assertNotEquals("30", jp1.convertStringFromEnvironment("AJSENV.ID_C"));
    }

    @Test
    public void convertStringFromEnvironmentでRHOSTを指定した場合値はWEBSV01であること() {
        jp1.setEnvironmentVariable();
        assertEquals("WEB-SV01", jp1.convertStringFromEnvironment("AJSENV.RHOST"));
    }

    @Test
    public void convertStringFromEnvironmentでRTARGETを指定した場合値はWEBAP01であること() {
        jp1.setEnvironmentVariable();
        assertEquals("WEB-AP01", jp1.convertStringFromEnvironment("AJSENV.RTARGET"));
    }

    @Test
    public void convertStringFromEnvironmentでJOB_SQLFを指定した場合値はreplacesqlであること() {
        jp1.setEnvironmentVariable();
        assertEquals("replace.sql", jp1.convertStringFromEnvironment("AJSENV.JOB_SQLF"));
    }

    @Test
    public void convertFromEnvで新設メソッドが正しく動作すること() {
        assertTrue(jp1.setEnvVariable());
        assertEquals("PROD", jp1.convertFromEnv("AJSENV.ENV"));
        assertEquals("10", jp1.convertFromEnv("AJSENV.ID_A"));
        assertEquals("'20'", jp1.convertFromEnv("AJSENV.ID_B"));
        assertEquals("\"30\"", jp1.convertFromEnv("AJSENV.ID_C"));
    }
}
