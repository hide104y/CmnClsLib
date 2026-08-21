using CmnClsLib.Module;
using CmnClsLib.Class;
using CmnClsLib.Interface;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Class
{
    public class UnitTest_ClsCrypt
    {
        private ClsCrypt _crypt = new();

        // --------------------------------------------------------------------
        // public Boolean Encrypt(String strKey, String strPlain)
        // --------------------------------------------------------------------
        [Fact]
        public void MD5で文字列を暗号化および復号化できること()
        {
            _crypt.HashAlgorithm = "MD5";
            _crypt.KeySize = MdlConst.CRYPT_KEYSIZE;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = 0;
            Assert.True(_crypt.Encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, _crypt.Result));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }
        [Fact]
        public void SHA1で文字列を暗号化および復号化できること()
        {
            _crypt.HashAlgorithm = "SHA1";
            _crypt.KeySize = MdlConst.CRYPT_KEYSIZE;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = MdlConst.CRYPT_ITERATIONCOUNT;
            Assert.True(_crypt.Encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, _crypt.Result));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }
        [Fact]
        public void SHA256サイズ128で文字列を暗号化および復号化できること()
        {
            _crypt.HashAlgorithm = "SHA256";
            _crypt.KeySize = MdlConst.CRYPT_KEYSIZE;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = MdlConst.CRYPT_ITERATIONCOUNT;
            Assert.True(_crypt.Encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, _crypt.Result));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }
        [Fact]
        public void SHA256サイズ256で文字列を暗号化および復号化できること()
        {
            _crypt.HashAlgorithm = "SHA256";
            _crypt.KeySize = 256;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = MdlConst.CRYPT_ITERATIONCOUNT;
            Assert.True(_crypt.Encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, _crypt.Result));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }
        [Fact]
        public void SHA512サイズ128で文字列を暗号化および復号化できること()
        {
            _crypt.HashAlgorithm = "SHA512";
            _crypt.KeySize = MdlConst.CRYPT_KEYSIZE;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = MdlConst.CRYPT_ITERATIONCOUNT;
            Assert.True(_crypt.Encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, _crypt.Result));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }
        [Fact]
        public void SHA512サイズ256で文字列を暗号化および復号化できること()
        {
            _crypt.HashAlgorithm = "SHA512";
            _crypt.KeySize = 256;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = MdlConst.CRYPT_ITERATIONCOUNT;
            Assert.True(_crypt.Encrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.CRYPT_TEST_PWD));
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, _crypt.Result));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }

        // --------------------------------------------------------------------
        // public Boolean Decrypt(String strKey, String strPlain)
        // --------------------------------------------------------------------
        [Fact]
        public void MD5でOpenSSL互換暗号化文字列を復号化できること()
        {
            _crypt.HashAlgorithm = "MD5";
            _crypt.KeySize = MdlConst.CRYPT_KEYSIZE;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = 0;
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_MD5_S128_B128_C0));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }
        [Fact]
        public void SHA1でOpenSSL互換暗号化文字列を復号化できること()
        {
            _crypt.HashAlgorithm = "SHA1";
            _crypt.KeySize = MdlConst.CRYPT_KEYSIZE;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = MdlConst.CRYPT_ITERATIONCOUNT;
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_SHA1_S128_B128_C10000));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }
        [Fact]
        public void SHA256サイズ128でOpenSSL互換暗号化文字列を復号化できること()
        {
            _crypt.HashAlgorithm = "SHA256";
            _crypt.KeySize = MdlConst.CRYPT_KEYSIZE;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = MdlConst.CRYPT_ITERATIONCOUNT;
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_SHA256_S128_B128_C10000));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }
        [Fact]
        public void SHA256サイズ256でOpenSSL互換暗号化文字列を復号化できること()
        {
            _crypt.HashAlgorithm = "SHA256";
            _crypt.KeySize = 256;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = MdlConst.CRYPT_ITERATIONCOUNT;
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_SHA256_S256_B128_C10000));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }
        [Fact]
        public void SHA512サイズ128でOpenSSL互換暗号化文字列を復号化できること()
        {
            _crypt.HashAlgorithm = "SHA512";
            _crypt.KeySize = ClsCrypt.DEFAULT_KEY_SIZE;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = MdlConst.CRYPT_ITERATIONCOUNT;
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_SHA512_S128_B128_C10000));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }
        [Fact]
        public void SHA512サイズ256でOpenSSL互換暗号化文字列を復号化できること()
        {
            _crypt.HashAlgorithm = "SHA512";
            _crypt.KeySize = 256;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = MdlConst.CRYPT_ITERATIONCOUNT;
            Assert.True(_crypt.Decrypt(MdlConst.CRYPT_TEST_KEY, MdlConst.ENC_PASS_SHA512_S256_B128_C10000));
            Assert.Equal(MdlConst.CRYPT_TEST_PWD, _crypt.Result);
        }

        // --------------------------------------------------------------------
        // private Boolean DeriveOpenSslKeyPbkdf2(byte[] baKey, byte[] baSalt)
        // --------------------------------------------------------------------
        [Fact]
        public void DeriveOpenSslKeyPbkdf2でIterationCountが0の場合は1に変更されること()
        {
            byte[] baKey = Encoding.UTF8.GetBytes(MdlConst.CRYPT_TEST_KEY);
            byte[] baSalt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            _crypt.HashAlgorithm = "SHA512";
            _crypt.KeySize = MdlConst.CRYPT_KEYSIZE;
            _crypt.BlockSize = MdlConst.CRYPT_BLOCKSIZE;
            _crypt.IterationCount = 0;
            Assert.True(_crypt.DeriveOpenSslKeyPbkdf2(baKey, baSalt));
            Assert.Equal(1, _crypt.IterationCount);
        }

        // --------------------------------------------------------------------
    }
}
