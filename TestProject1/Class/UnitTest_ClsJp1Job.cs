using CmnClsLib.Module;
using CmnClsLib.Class;
using CmnClsLib.Interface;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Class
{
    public class UnitTest_ClsJp1Job
    {
        private static ClsLogger _logger = new();
        private ClsJp1Job _jp1 = new(_logger);

        public UnitTest_ClsJp1Job()
        {
            _jp1.JobName = "/ENV.PROD/JOB_SQLF.list_schema.sql__ID_A.10__ID_B.20.sq__ID_C.30.dq/I52.ADサーバ再起動/I22.再起動.RHOST.WEB-SV01..RTARGET.WEB-AP01/AAA/JOB_SQLF.replace.sql__BBB";
        }

        // --------------------------------------------------------------------
        // SetEnvironmentVariable()
        // --------------------------------------------------------------------
        [Fact]
        public void 環境変数をセットできること()
        {
            Assert.True(_jp1.SetEnvironmentVariable());
        }

        // --------------------------------------------------------------------
        // ConvertStringFromEnvironment()
        // --------------------------------------------------------------------
        [Fact]
        public void ConvertStringFromEnvironmentでENVを指定した場合値はPRODであること()
        {
            Assert.Equal("PROD", _jp1.ConvertStringFromEnvironment("AJSENV.ENV"));
        }
        [Fact]
        public void ConvertStringFromEnvironmentでID_Aを指定した場合値は10であること()
        {
            Assert.Equal("10", _jp1.ConvertStringFromEnvironment("AJSENV.ID_A"));
        }
        [Fact]
        public void ConvertStringFromEnvironmentでID_Bを指定した場合値は20であること()
        {
            Assert.Equal("'20'", _jp1.ConvertStringFromEnvironment("AJSENV.ID_B"));
        }
        [Fact]
        public void ConvertStringFromEnvironmentでID_Cを指定した場合値は30であること()
        {
            Assert.Equal("\"30\"", _jp1.ConvertStringFromEnvironment("AJSENV.ID_C"));
        }
        [Fact]
        public void ConvertStringFromEnvironmentでID_Bを指定した場合値はシングルクォーテーションなし20でないこと()
        {
            Assert.NotEqual("20", _jp1.ConvertStringFromEnvironment("AJSENV.ID_B"));
        }
        [Fact]
        public void ConvertStringFromEnvironmentでID_Cを指定した場合値はダブルクォーテーションなし30でないこと()
        {
            Assert.NotEqual("30", _jp1.ConvertStringFromEnvironment("AJSENV.ID_C"));
        }
        [Fact]
        public void ConvertStringFromEnvironmentでRHOSTを指定した場合値はWEBSV01であること()
        {
            Assert.Equal("WEB-SV01", _jp1.ConvertStringFromEnvironment("AJSENV.RHOST"));
        }
        [Fact]
        public void ConvertStringFromEnvironmentでRTARGETを指定した場合値はWEBAP01であること()
        {
            Assert.Equal("WEB-AP01", _jp1.ConvertStringFromEnvironment("AJSENV.RTARGET"));
        }
        [Fact]
        public void ConvertStringFromEnvironmentでJOB_SQLFを指定した場合値はreplacesqlであること()
        {
            Assert.Equal("replace.sql", _jp1.ConvertStringFromEnvironment("AJSENV.JOB_SQLF"));
        }
    }
}
