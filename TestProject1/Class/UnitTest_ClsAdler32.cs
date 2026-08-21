using CmnClsLib.Module;
using CmnClsLib.Class;
using CmnClsLib.Interface;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Class
{
    public class UnitTest_ClsAdler32
    {
        public const String SampleDocument = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const String AdlerValue = "376443647";

        private ClsAdler32 _objAdler32 = new();

        // --------------------------------------------------------------------
        // public String ComputeChecksum(String strPathFFile)
        // --------------------------------------------------------------------
        [Fact]
        public void abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZのadler32値が376443647であること()
        {
            // 第1引数：期待する値
            // 第2引数：計算結果
            Assert.Equal(AdlerValue, _objAdler32.ComputeChecksum(new MemoryStream(Encoding.ASCII.GetBytes(SampleDocument))));
        }
    }
}
