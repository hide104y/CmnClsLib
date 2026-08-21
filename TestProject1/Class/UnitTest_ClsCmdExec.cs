using CmnClsLib.Module;
using CmnClsLib.Interface;
using CmnClsLib.Class;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Class
{
    public class UnitTest_ClsCmdExec
    {
        private static ClsLogger _logger = new();
        private ClsCmdExec _cmdExec = new(_logger);

        // --------------------------------------------------------------------
        // ExecuteThread()
        // --------------------------------------------------------------------
        [Fact]
        public void Execute_ipconfigが正常終了すること()
        {
            _cmdExec.CmdPath = "" + System.Environment.GetEnvironmentVariable("ComSpec");
            _cmdExec.CmdArgs = "/c ipconfig /all";
            Assert.Equal(0, _cmdExec.ExecuteThread(3));
        }
        [Fact]
        public void Execute_存在しないコマンドを実行した場合異常終了すること()
        {
            _cmdExec.CmdPath = "" + System.Environment.GetEnvironmentVariable("ComSpec");
            _cmdExec.CmdArgs = "/c ifconfig /all";
            Assert.Equal(1, _cmdExec.ExecuteThread(3));
        }
    }
}
