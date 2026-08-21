using CmnClsLib.Module;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Module
{
    public class UnitTest_MdlLog
    {
        // --------------------------------------------------------------------
        // ParseLogLevel()
        // --------------------------------------------------------------------
        [Fact]
        public void ParseLogLevel_引数がnoneの場合は定数値LVL_NONEを返却すること()
        {
            int expected = MdlConst.LVL_NONE;
            int actual = MdlLog.ParseLogLevel("none");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseLogLevel_引数がdebugの場合は定数値LVL_DEBUGを返却すること()
        {
            int expected = MdlConst.LVL_DEBUG;
            int actual = MdlLog.ParseLogLevel("debug");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseLogLevel_引数がinfoの場合は定数値LVL_Iを返却すること()
        {
            int expected = MdlConst.LVL_I;
            int actual = MdlLog.ParseLogLevel("info");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseLogLevel_引数がwarnの場合は定数値LVL_Wを返却すること()
        {
            int expected = MdlConst.LVL_W;
            int actual = MdlLog.ParseLogLevel("warn");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseLogLevel_引数がerrorの場合は定数値LVL_Eを返却すること()
        {
            int expected = MdlConst.LVL_E;
            int actual = MdlLog.ParseLogLevel("error");
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // LogLevelToString()
        // --------------------------------------------------------------------
        [Fact]
        public void LogLevelToString_引数が定数値LVL_NONEの場合はを文字列none返却すること()
        {
            string expected = @"none";
            string actual = MdlLog.LogLevelToString(MdlConst.LVL_NONE);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LogLevelToString_引数が定数値LVL_DEBUGの場合はを文字列debug返却すること()
        {
            string expected = @"debug";
            string actual = MdlLog.LogLevelToString(MdlConst.LVL_DEBUG);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LogLevelToString_引数が定数値LVL_Iの場合はを文字列info返却すること()
        {
            string expected = @"info";
            string actual = MdlLog.LogLevelToString(MdlConst.LVL_I);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LogLevelToString_引数が定数値LVL_Wの場合はを文字列warn返却すること()
        {
            string expected = @"warn";
            string actual = MdlLog.LogLevelToString(MdlConst.LVL_W);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LogLevelToString_引数が定数値LVL_Eの場合はを文字列error返却すること()
        {
            string expected = @"error";
            string actual = MdlLog.LogLevelToString(MdlConst.LVL_E);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // GetLogLevelPrefix()
        // --------------------------------------------------------------------
        [Fact]
        public void GetLogLevelPrefix_引数が定数値LVL_NONEの場合はを空文字列返却すること()
        {
            string expected = @"";
            string actual = MdlLog.GetLogLevelPrefix(MdlConst.LVL_NONE);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetLogLevelPrefix_引数が定数値LVL_DEBUGの場合はを文字列DEBUG空白を返却すること()
        {
            string expected = @"DEBUG ";
            string actual = MdlLog.GetLogLevelPrefix(MdlConst.LVL_DEBUG);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetLogLevelPrefix_引数が定数値LVL_Iの場合はを文字列INFO空白を返却すること()
        {
            string expected = @"INFO ";
            string actual = MdlLog.GetLogLevelPrefix(MdlConst.LVL_I);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetLogLevelPrefix_引数が定数値LVL_Wの場合はを文字列WARN空白を返却すること()
        {
            string expected = @"WARN ";
            string actual = MdlLog.GetLogLevelPrefix(MdlConst.LVL_W);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetLogLevelPrefix_引数が定数値LVL_Eの場合はを文字列ERROR空白を返却すること()
        {
            string expected = @"ERROR ";
            string actual = MdlLog.GetLogLevelPrefix(MdlConst.LVL_E);
            Assert.Equal(expected, actual);
        }
    }
}
