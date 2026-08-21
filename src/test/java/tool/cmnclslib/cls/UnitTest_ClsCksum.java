package tool.cmnclslib.cls;

import java.io.ByteArrayInputStream;
import java.nio.charset.StandardCharsets;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 * ClsCksum の単体テストクラスです。
 */
public class UnitTest_ClsCksum {

    public static final String SAMPLE_DOCUMENT = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static final String CKSUM_VALUE = "1886645594";
    public static final String ADLER_VALUE = "376443647";
    public static final String MD5_VALUE = "2ad372c377013baa4ee32ab6649d2449";
    public static final String SHA1_VALUE = "db16441c4b330570a9ac83b0e0b006fcd74cc32b";
    public static final String SHA256_VALUE = "3964294b664613798d1a477eb8ad02118b48d0c5738c427613202f2ed123b5f1";
    public static final String SHA512_VALUE = "b8afc1e7b4e4d6a99a6d514a4450431dc189a5628a6777e785c0cd1540045c0eb60274c7d0a951357d2bc4c9407f212e80231eb3c12c877eb7cffda4081587ae";

    private ClsCksum cksum = new ClsCksum();
    public long sampleSize = SAMPLE_DOCUMENT.length();

    @Test
    public void testCksumDefault() {
        assertEquals(CKSUM_VALUE, cksum.getChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII))));
    }

    @Test
    public void testChecksumAlgorithms() {
        assertEquals(CKSUM_VALUE, cksum.getChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII)), "DEFAULT"));
        assertEquals(MD5_VALUE, cksum.getChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII)), "MD5"));
        assertEquals(SHA1_VALUE, cksum.getChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII)), "SHA-1"));
        assertEquals(SHA256_VALUE, cksum.getChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII)), "SHA256"));
        assertEquals(SHA512_VALUE, cksum.getChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII)), "SHA512"));
        assertEquals(ADLER_VALUE, cksum.getChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII)), "ADLER32"));
    }

    @Test
    public void testChecksumLength() {
        cksum.getChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII)), "MD5");
        assertEquals(sampleSize, cksum.getLength());

        cksum.getChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII)), "ADLER32");
        assertEquals(sampleSize, cksum.getLength());

        cksum.getChecksum(new ByteArrayInputStream(SAMPLE_DOCUMENT.getBytes(StandardCharsets.US_ASCII)), "DEFAULT");
        assertEquals(sampleSize, cksum.getLength());
    }
}
