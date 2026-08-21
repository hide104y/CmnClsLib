package tool.cmnclslib.cls;

import java.io.ByteArrayInputStream;
import java.nio.charset.StandardCharsets;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 * ClsAdler32 の単体テストクラスです。
 */
public class UnitTest_ClsAdler32 {

    public static final String SAMPLE_DOCUMENT = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static final String ADLER_VALUE = "376443647";

    private ClsAdler32 objAdler32 = new ClsAdler32();

    @Test
    public void testAdler32Calculation() {
        assertEquals(ADLER_VALUE, objAdler32.computeChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII))));
    }
}
