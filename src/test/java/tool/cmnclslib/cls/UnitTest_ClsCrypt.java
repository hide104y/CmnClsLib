package tool.cmnclslib.cls;

import java.nio.charset.StandardCharsets;
import org.junit.Test;
import static org.junit.Assert.*;
import tool.cmnclslib.mdl.MdlConst;

/**
 * ClsCrypt の単体テストクラスです。
 */
public class UnitTest_ClsCrypt {

    private ClsCrypt crypt = new ClsCrypt();

    @Test
    public void testMd5EncryptDecrypt() {
        crypt.setHashAlgorithm("MD5");
        crypt.setKeySize(MdlConst.CRYPT_KEYSIZE);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(0);
        assertTrue(crypt.encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, crypt.getResult()));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testSha1EncryptDecrypt() {
        crypt.setHashAlgorithm("SHA1");
        crypt.setKeySize(MdlConst.CRYPT_KEYSIZE);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(MdlConst.CRYPT_ITERATIONCOUNT);
        assertTrue(crypt.encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, crypt.getResult()));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testSha256Size128EncryptDecrypt() {
        crypt.setHashAlgorithm("SHA256");
        crypt.setKeySize(MdlConst.CRYPT_KEYSIZE);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(MdlConst.CRYPT_ITERATIONCOUNT);
        assertTrue(crypt.encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, crypt.getResult()));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testSha256Size256EncryptDecrypt() {
        crypt.setHashAlgorithm("SHA256");
        crypt.setKeySize(256);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(MdlConst.CRYPT_ITERATIONCOUNT);
        assertTrue(crypt.encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, crypt.getResult()));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testSha512Size128EncryptDecrypt() {
        crypt.setHashAlgorithm("SHA512");
        crypt.setKeySize(MdlConst.CRYPT_KEYSIZE);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(MdlConst.CRYPT_ITERATIONCOUNT);
        assertTrue(crypt.encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, crypt.getResult()));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testSha512Size256EncryptDecrypt() {
        crypt.setHashAlgorithm("SHA512");
        crypt.setKeySize(256);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(MdlConst.CRYPT_ITERATIONCOUNT);
        assertTrue(crypt.encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, crypt.getResult()));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testMd5DecryptOpenSsl() {
        crypt.setHashAlgorithm("MD5");
        crypt.setKeySize(MdlConst.CRYPT_KEYSIZE);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(0);
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_MD5_S128_B128_C0));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testSha1DecryptOpenSsl() {
        crypt.setHashAlgorithm("SHA1");
        crypt.setKeySize(MdlConst.CRYPT_KEYSIZE);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(MdlConst.CRYPT_ITERATIONCOUNT);
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_SHA1_S128_B128_C10000));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testSha256Size128DecryptOpenSsl() {
        crypt.setHashAlgorithm("SHA256");
        crypt.setKeySize(MdlConst.CRYPT_KEYSIZE);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(MdlConst.CRYPT_ITERATIONCOUNT);
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_SHA256_S128_B128_C10000));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testSha256Size256DecryptOpenSsl() {
        crypt.setHashAlgorithm("SHA256");
        crypt.setKeySize(256);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(MdlConst.CRYPT_ITERATIONCOUNT);
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_SHA256_S256_B128_C10000));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testSha512Size128DecryptOpenSsl() {
        crypt.setHashAlgorithm("SHA512");
        crypt.setKeySize(MdlConst.CRYPT_KEYSIZE);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(MdlConst.CRYPT_ITERATIONCOUNT);
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_SHA512_S128_B128_C10000));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testSha512Size256DecryptOpenSsl() {
        crypt.setHashAlgorithm("SHA512");
        crypt.setKeySize(256);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(MdlConst.CRYPT_ITERATIONCOUNT);
        assertTrue(crypt.decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_SHA512_S256_B128_C10000));
        assertEquals(MdlConst.CRYPT_TEST_PWD, crypt.getResult());
    }

    @Test
    public void testDeriveOpenSslKeyPbkdf2IterationZero() {
        byte[] baKey = MdlConst.CRYPT_TEST_KEY.getBytes(StandardCharsets.UTF_8);
        byte[] baSalt = new byte[] {1, 2, 3, 4, 5, 6, 7, 8};
        crypt.setHashAlgorithm("SHA512");
        crypt.setKeySize(MdlConst.CRYPT_KEYSIZE);
        crypt.setBlockSize(MdlConst.CRYPT_BLOCKSIZE);
        crypt.setIterationCount(0);
        assertTrue(crypt.deriveOpenSslKeyPbkdf2(baKey, baSalt));
        assertEquals(1, crypt.getIterationCount());
    }

    @Test
    public void testDeriveKeyPbkdf2() {
        byte[] baKey = MdlConst.CRYPT_TEST_KEY.getBytes(StandardCharsets.UTF_8);
        byte[] baSalt = new byte[] {1, 2, 3, 4, 5, 6, 7, 8};
        crypt.setHashAlgorithm("SHA256");
        crypt.setKeySize(256);
        crypt.setBlockSize(128);
        crypt.setIterationCount(1000);
        assertTrue(crypt.deriveKeyPbkdf2(baKey, baSalt));
    }
}
