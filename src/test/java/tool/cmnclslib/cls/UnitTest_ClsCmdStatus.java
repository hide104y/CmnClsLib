package tool.cmnclslib.cls;

import org.junit.Before;
import org.junit.Test;
import tool.cmnclslib.mdl.MdlConst;
import static org.junit.Assert.*;

/**
 * ClsCmdStatus の単体テストクラスです。
 */
public class UnitTest_ClsCmdStatus {

    private ClsLogger logger = new ClsLogger();
    private ClsCmdStatus objCmdStatus;

    @Before
    public void setUp() {
        objCmdStatus = new ClsCmdStatus(logger);
    }

    @Test
    public void カンマ区切り文字列に値を設定してInitを実行するとそれぞれの値がList型に分解されること() {
        objCmdStatus.setOkReturnCodeCsv("0,5,10");
        objCmdStatus.setWarnReturnCodeCsv("0,5,10");
        objCmdStatus.setErrorReturnCodeCsv("0,5,10");
        objCmdStatus.setOkMessageCsv("正常,いけてる,完璧");
        objCmdStatus.setWarnMessageCsv("警告,いけてるないかも,完璧でなかった");
        objCmdStatus.setErrorMessageCsv("異常,いけてない,失敗");
        objCmdStatus.initialize();

        assertEquals(3, objCmdStatus.getOkReturnCodeList().size());
        assertTrue(objCmdStatus.getOkReturnCodeList().contains(0));
        assertTrue(objCmdStatus.getOkReturnCodeList().contains(5));
        assertTrue(objCmdStatus.getOkReturnCodeList().contains(10));

        assertEquals(3, objCmdStatus.getWarnReturnCodeList().size());
        assertTrue(objCmdStatus.getWarnReturnCodeList().contains(0));
        assertTrue(objCmdStatus.getWarnReturnCodeList().contains(5));
        assertTrue(objCmdStatus.getWarnReturnCodeList().contains(10));

        assertEquals(3, objCmdStatus.getErrorReturnCodeList().size());
        assertTrue(objCmdStatus.getErrorReturnCodeList().contains(0));
        assertTrue(objCmdStatus.getErrorReturnCodeList().contains(5));
        assertTrue(objCmdStatus.getErrorReturnCodeList().contains(10));

        assertEquals(3, objCmdStatus.getOkMessageList().size());
        assertTrue(objCmdStatus.getOkMessageList().contains("正常"));
        assertTrue(objCmdStatus.getOkMessageList().contains("いけてる"));
        assertTrue(objCmdStatus.getOkMessageList().contains("完璧"));

        assertEquals(3, objCmdStatus.getWarnMessageList().size());
        assertTrue(objCmdStatus.getWarnMessageList().contains("警告"));
        assertTrue(objCmdStatus.getWarnMessageList().contains("いけてるないかも"));
        assertTrue(objCmdStatus.getWarnMessageList().contains("完璧でなかった"));

        assertEquals(3, objCmdStatus.getErrorMessageList().size());
        assertTrue(objCmdStatus.getErrorMessageList().contains("異常"));
        assertTrue(objCmdStatus.getErrorMessageList().contains("いけてない"));
        assertTrue(objCmdStatus.getErrorMessageList().contains("失敗"));
    }

    @Test
    public void チェックワードリストが空の場合はfalseを返却すること() {
        objCmdStatus.setOkMessageCsv("");
        objCmdStatus.setWarnMessageCsv("");
        objCmdStatus.setErrorMessageCsv("");
        objCmdStatus.initialize();
        assertFalse(objCmdStatus.shouldCheckMessage());
    }

    @Test
    public void チェックワードリストのどれかが空でない場合はtrueを返却すること() {
        objCmdStatus.setOkMessageCsv("");
        objCmdStatus.setWarnMessageCsv("警告,いけてるないかも,完璧でなかった");
        objCmdStatus.setErrorMessageCsv("");
        objCmdStatus.initialize();
        assertTrue(objCmdStatus.shouldCheckMessage());
    }

    @Test
    public void initFlagsを実行するとフラグは全てfalseを返却すること() {
        objCmdStatus.setOkMessageHit(true);
        objCmdStatus.setWarnMessageHit(true);
        objCmdStatus.setErrorMessageHit(true);
        objCmdStatus.resetFlags();
        assertFalse(objCmdStatus.isOkMessageHit());
        assertFalse(objCmdStatus.isWarnMessageHit());
        assertFalse(objCmdStatus.isErrorMessageHit());
    }

    @Test
    public void checkMessageLineで正常終了しましたを評価すると正常該当フラグだけがtrueとなること() {
        objCmdStatus.setOkMessageCsv("正常,いけてる,完璧");
        objCmdStatus.setWarnMessageCsv("警告,いけてるないかも,完璧でなかった");
        objCmdStatus.setErrorMessageCsv("異常,いけてない,失敗");
        objCmdStatus.initialize();
        objCmdStatus.resetFlags();
        objCmdStatus.checkMessageLine("正常終了しました");
        assertTrue(objCmdStatus.isOkMessageHit());
        assertFalse(objCmdStatus.isWarnMessageHit());
        assertFalse(objCmdStatus.isErrorMessageHit());
    }

    @Test
    public void checkMessageLineで警告が発生しましたがなんとか終了しましたを評価すると警告該当フラグだけがtrueとなること() {
        objCmdStatus.setOkMessageCsv("正常,いけてる,完璧");
        objCmdStatus.setWarnMessageCsv("警告,いけてるないかも,完璧でなかった");
        objCmdStatus.setErrorMessageCsv("異常,いけてない,失敗");
        objCmdStatus.initialize();
        objCmdStatus.resetFlags();
        objCmdStatus.checkMessageLine("警告が発生しましたがなんとか終了しました");
        assertFalse(objCmdStatus.isOkMessageHit());
        assertTrue(objCmdStatus.isWarnMessageHit());
        assertFalse(objCmdStatus.isErrorMessageHit());
    }

    @Test
    public void checkMessageLineで異常終了しましたを評価すると異常該当フラグだけがtrueとなること() {
        objCmdStatus.setOkMessageCsv("正常,いけてる,完璧");
        objCmdStatus.setWarnMessageCsv("警告,いけてるないかも,完璧でなかった");
        objCmdStatus.setErrorMessageCsv("異常,いけてない,失敗");
        objCmdStatus.initialize();
        objCmdStatus.resetFlags();
        objCmdStatus.checkMessageLine("異常終了しました");
        assertFalse(objCmdStatus.isOkMessageHit());
        assertFalse(objCmdStatus.isWarnMessageHit());
        assertTrue(objCmdStatus.isErrorMessageHit());
    }

    @Test
    public void checkMessageLineで警告が発生しましたが正常に終了しましたを評価すると正常と警告該当フラグがtrueとなること() {
        objCmdStatus.setOkMessageCsv("正常,いけてる,完璧");
        objCmdStatus.setWarnMessageCsv("警告,いけてるないかも,完璧でなかった");
        objCmdStatus.setErrorMessageCsv("異常,いけてない,失敗");
        objCmdStatus.initialize();
        objCmdStatus.resetFlags();
        objCmdStatus.checkMessageLine("警告が発生しましたが正常に終了しました");
        assertTrue(objCmdStatus.isOkMessageHit());
        assertTrue(objCmdStatus.isWarnMessageHit());
        assertFalse(objCmdStatus.isErrorMessageHit());
    }

    @Test
    public void isAlwaysNormalが正の場合はCheckCmdExitCodeの評価で結果がLVL_Iになること() {
        objCmdStatus.setAlwaysNormal(true);
        objCmdStatus.checkCommandExitCode(9999);
        assertEquals(MdlConst.LVL_I, objCmdStatus.getMethodExitStatus());
        assertEquals(MdlConst.LVL_I, objCmdStatus.getReturnLevel());
    }

    @Test
    public void 警告閾値と異常閾値が設定されていない場合にCheckCmdExitCodeの評価で0の結果がLVL_Iになること() {
        int intCmdRetcode = 0;
        objCmdStatus.setWarnThreshold(MdlConst.INT_NULL);
        objCmdStatus.setErrorThreshold(MdlConst.INT_NULL);
        objCmdStatus.checkCommandExitCode(intCmdRetcode);
        assertEquals(MdlConst.LVL_I, objCmdStatus.getMethodExitStatus());
        assertEquals(MdlConst.LVL_I, objCmdStatus.getReturnLevel());
    }

    @Test
    public void 警告閾値と異常閾値が設定されていない場合にCheckCmdExitCodeの評価で1の結果レベルがLVL_Eになること() {
        int intCmdRetcode = 1;
        objCmdStatus.setWarnThreshold(MdlConst.INT_NULL);
        objCmdStatus.setErrorThreshold(MdlConst.INT_NULL);
        objCmdStatus.checkCommandExitCode(intCmdRetcode);
        assertEquals(intCmdRetcode, objCmdStatus.getMethodExitStatus());
        assertEquals(MdlConst.LVL_E, objCmdStatus.getReturnLevel());
    }

    @Test
    public void 警告閾値と異常閾値が設定されていない場合にCheckCmdExitCodeの評価で異常時終了コードが20の場合1の戻り値は20になること() {
        int intCmdRetcode = 1;
        objCmdStatus.setWarnThreshold(MdlConst.INT_NULL);
        objCmdStatus.setErrorThreshold(MdlConst.INT_NULL);
        objCmdStatus.setErrorCode(20);
        objCmdStatus.checkCommandExitCode(intCmdRetcode);
        assertEquals(20, objCmdStatus.getMethodExitStatus());
        assertEquals(MdlConst.LVL_E, objCmdStatus.getReturnLevel());
    }

    @Test
    public void 新設短縮メソッド_getErrRetCodeCsvおよびisErrAtNegativeが正しく動作すること() {
        objCmdStatus.setErrRetCodeCsv("1,2,3");
        assertEquals("1,2,3", objCmdStatus.getErrRetCodeCsv());

        objCmdStatus.setErrAtNegative(true);
        assertTrue(objCmdStatus.isErrAtNegative());

        objCmdStatus.setWarnRetCodeList(java.util.List.of(10, 20));
        assertEquals(2, objCmdStatus.getWarnRetCodeList().size());

        objCmdStatus.setErrRetCodeList(java.util.List.of(30, 40));
        assertEquals(2, objCmdStatus.getErrRetCodeList().size());
    }
}
