package tool.cmnclslib.mdl;

import org.junit.Test;
import static org.junit.Assert.*;

/**
 * MdlConst の単体テストクラスです。
 */
public class UnitTest_MdlConst {

    @Test
    public void testConstants() {
        assertEquals(1000, MdlConst.LVL_NONE);
        assertEquals(30, MdlConst.LVL_F);
        assertEquals(20, MdlConst.LVL_E);
        assertEquals(10, MdlConst.LVL_W);
        assertEquals(0, MdlConst.LVL_I);
        assertEquals(-1, MdlConst.LVL_DEBUG);
    }
}
