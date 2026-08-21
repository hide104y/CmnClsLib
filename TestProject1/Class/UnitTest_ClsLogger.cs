using CmnClsLib.Module;
using CmnClsLib.Class;
using CmnClsLib.Interface;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Class
{
    public class UnitTest_ClsLogger
    {
        private ClsLogger _logger = new();

        public UnitTest_ClsLogger()
        {
            _logger.SetValueByKey(ClsLogger.IS_FILE, "true");
            _logger.SetValueByKey(ClsLogger.DIR, System.IO.Path.GetTempPath());
            _logger.SetValueByKey(ClsLogger.PATH, System.IO.Path.GetTempPath() + @"\ClsLog.log");
        }

        // --------------------------------------------------------------------
        // WriteLine()
        // --------------------------------------------------------------------
        [Fact]
        public void WriteLine_引数の内容が出力されること()
        {
            // ファイルが存在する場合は削除
            if (MdlFile.GetPathType(_logger.GetValueByKey(ClsLogger.PATH, "")) == MdlFile.PATH_IS_FILE)
            {
                System.IO.File.Delete(_logger.GetValueByKey(ClsLogger.PATH, ""));
            }
            // 出力
            _logger.WriteLine(MdlConst.LVL_NONE, "OK");
            // テスト
            Assert.Equal("OK", MdlFile.ReadFile(_logger.GetValueByKey(ClsLogger.PATH, ""), -1));
            // 事後作業
            if (MdlFile.GetPathType(_logger.GetValueByKey(ClsLogger.PATH, "")) == MdlFile.PATH_IS_FILE)
            {
                System.IO.File.Delete(_logger.GetValueByKey(ClsLogger.PATH, ""));
            }
        }
    }
}
