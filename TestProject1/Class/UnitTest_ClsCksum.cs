using CmnClsLib.Module;
using CmnClsLib.Class;
using CmnClsLib.Interface;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Class
{
    public class UnitTest_ClsCksum
    {
        public const String SampleDocument = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const String CksumValue = "1886645594";
        public const String AdlerValue = "376443647";
        public const String MD5Value = "2ad372c377013baa4ee32ab6649d2449";
        public const String SHA1Value = "db16441c4b330570a9ac83b0e0b006fcd74cc32b";
        public const String SHA256Value = "3964294b664613798d1a477eb8ad02118b48d0c5738c427613202f2ed123b5f1";
        public const String SHA512Value = "b8afc1e7b4e4d6a99a6d514a4450431dc189a5628a6777e785c0cd1540045c0eb60274c7d0a951357d2bc4c9407f212e80231eb3c12c877eb7cffda4081587ae";

        private ClsCksum _cksum = new();
        public long sampleSize = (long)SampleDocument.Length;

        // --------------------------------------------------------------------
        // public String GetChecksum(Stream objStream)
        // --------------------------------------------------------------------
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのcksum値が1886645594であること()
        {
            Assert.Equal(CksumValue, _cksum.GetChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument))));
        }

        // --------------------------------------------------------------------
        // public String GetChecksum(Stream objStream, String strAlgorithm)
        // --------------------------------------------------------------------
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのDEFAULT値が2ad372c377013baa4ee32ab6649d2449であること()
        {
            Assert.Equal(CksumValue, _cksum.GetChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument)), "DEFAULT"));
        }
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのMD5値が2ad372c377013baa4ee32ab6649d2449であること()
        {
            Assert.Equal(MD5Value, _cksum.GetChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument)), "MD5"));
        }
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのSHA1値が2ad372c377013baa4ee32ab6649d2449であること()
        {
            Assert.Equal(SHA1Value, _cksum.GetChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument)), "SHA-1"));
        }
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのSHA256値が2ad372c377013baa4ee32ab6649d2449であること()
        {
            Assert.Equal(SHA256Value, _cksum.GetChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument)), "SHA256"));
        }
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのSHA512値が2ad372c377013baa4ee32ab6649d2449であること()
        {
            Assert.Equal(SHA512Value, _cksum.GetChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument)), "SHA512"));
        }
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのadler32値が376443647であること()
        {
            Assert.Equal(AdlerValue, _cksum.GetChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument)), "ADLER32"));
        }
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのMD5計算時の文字列サイズは52であること()
        {
            _cksum.GetChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument)), "MD5");
            Assert.Equal(sampleSize, _cksum.Length);
        }
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのadler32計算時の文字列サイズは52であること()
        {
            _cksum.GetChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument)), "ADLER32");
            Assert.Equal(sampleSize, _cksum.Length);
        }
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのcksum計算時の文字列サイズは52であること()
        {
            _cksum.GetChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument)), "DEFAULT");
            Assert.Equal(sampleSize, _cksum.Length);
        }

    }
}
