using CmnClsLib.Module;
using CmnClsLib.Class;
using CmnClsLib.Interface;
using System.Text;
using System.Linq;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Class
{
    public class UnitTest_ClsCmdStatus
    {
        private static ClsLogger _logger = new();
        private ClsCmdStatus _objCmdStatus = new(_logger);

        // --------------------------------------------------------------------
        // public void Initialize()
        // --------------------------------------------------------------------
        [Fact]
        public void カンマ区切り文字列に値を設定してInitを実行するとそれぞれの値がList型に分解されること()
        {
            _objCmdStatus.OkReturnCodeCsv = "0,5,10";
            _objCmdStatus.WarnReturnCodeCsv = "0,5,10";
            _objCmdStatus.ErrorReturnCodeCsv = "0,5,10";
            _objCmdStatus.OkMessageCsv = "正常,いけてる,完璧";
            _objCmdStatus.WarnMessageCsv = "警告,いけてるないかも,完璧でなかった";
            _objCmdStatus.ErrorMessageCsv = "異常,いけてない,失敗";
            _objCmdStatus.Initialize();
            Assert.Equal(3, _objCmdStatus.OkReturnCodeList.Count);
            Assert.Contains(0, _objCmdStatus.OkReturnCodeList);
            Assert.Contains(5, _objCmdStatus.OkReturnCodeList);
            Assert.Contains(10, _objCmdStatus.OkReturnCodeList);
            Assert.Equal(3, _objCmdStatus.WarnReturnCodeList.Count);
            Assert.Contains(0, _objCmdStatus.WarnReturnCodeList);
            Assert.Contains(5, _objCmdStatus.WarnReturnCodeList);
            Assert.Contains(10, _objCmdStatus.WarnReturnCodeList);
            Assert.Equal(3, _objCmdStatus.ErrorReturnCodeList.Count);
            Assert.Contains(0, _objCmdStatus.ErrorReturnCodeList);
            Assert.Contains(5, _objCmdStatus.ErrorReturnCodeList);
            Assert.Contains(10, _objCmdStatus.ErrorReturnCodeList);
            Assert.Equal(3, _objCmdStatus.OkMessageList.Count);
            Assert.Contains("正常", _objCmdStatus.OkMessageList);
            Assert.Contains("いけてる", _objCmdStatus.OkMessageList);
            Assert.Contains("完璧", _objCmdStatus.OkMessageList);
            Assert.Equal(3, _objCmdStatus.WarnMessageList.Count);
            Assert.Contains("警告", _objCmdStatus.WarnMessageList);
            Assert.Contains("いけてるないかも", _objCmdStatus.WarnMessageList);
            Assert.Contains("完璧でなかった", _objCmdStatus.WarnMessageList);
            Assert.Equal(3, _objCmdStatus.ErrorMessageList.Count);
            Assert.Contains("異常", _objCmdStatus.ErrorMessageList);
            Assert.Contains("いけてない", _objCmdStatus.ErrorMessageList);
            Assert.Contains("失敗", _objCmdStatus.ErrorMessageList);
        }

        // --------------------------------------------------------------------
        // public Boolean ShouldCheckMessage()
        // --------------------------------------------------------------------
        [Fact]
        public void チェックワードリストが空の場合はfalseを返却すること()
        {
            _objCmdStatus.OkMessageCsv = "";
            _objCmdStatus.WarnMessageCsv = "";
            _objCmdStatus.ErrorMessageCsv = "";
            _objCmdStatus.Initialize();
            Assert.False(_objCmdStatus.ShouldCheckMessage());
        }
        [Fact]
        public void チェックワードリストのどれかが空でない場合はtrueを返却すること()
        {
            _objCmdStatus.OkMessageCsv = "";
            _objCmdStatus.WarnMessageCsv = "警告,いけてるないかも,完璧でなかった";
            _objCmdStatus.ErrorMessageCsv = "";
            _objCmdStatus.Initialize();
            Assert.True(_objCmdStatus.ShouldCheckMessage());
        }

        // --------------------------------------------------------------------
        // public Boolean InitFlags()
        // --------------------------------------------------------------------
        [Fact]
        public void InitFlagsを実行するとフラグは全てfalseを返却すること()
        {
            _objCmdStatus.IsOkMessageHit = true;
            _objCmdStatus.IsWarnMessageHit = true;
            _objCmdStatus.IsErrorMessageHit = true;
            _objCmdStatus.ResetFlags();
            Assert.False(_objCmdStatus.IsOkMessageHit);
            Assert.False(_objCmdStatus.IsWarnMessageHit);
            Assert.False(_objCmdStatus.IsErrorMessageHit);
        }

        // --------------------------------------------------------------------
        // public void CheckMessageLine(String strLine)
        // --------------------------------------------------------------------
        [Fact]
        public void CheckMessageLineで正常終了しましたを評価すると正常該当フラグだけがtrueとなること()
        {
            _objCmdStatus.OkMessageCsv = "正常,いけてる,完璧";
            _objCmdStatus.WarnMessageCsv = "警告,いけてるないかも,完璧でなかった";
            _objCmdStatus.ErrorMessageCsv = "異常,いけてない,失敗";
            _objCmdStatus.Initialize();
            _objCmdStatus.ResetFlags();
            _objCmdStatus.CheckMessageLine("正常終了しました");
            Assert.True(_objCmdStatus.IsOkMessageHit);
            Assert.False(_objCmdStatus.IsWarnMessageHit);
            Assert.False(_objCmdStatus.IsErrorMessageHit);
        }
        [Fact]
        public void CheckMessageLineで警告が発生しましたがなんとか終了しましたを評価すると警告該当フラグだけがtrueとなること()
        {
            _objCmdStatus.OkMessageCsv = "正常,いけてる,完璧";
            _objCmdStatus.WarnMessageCsv = "警告,いけてるないかも,完璧でなかった";
            _objCmdStatus.ErrorMessageCsv = "異常,いけてない,失敗";
            _objCmdStatus.Initialize();
            _objCmdStatus.ResetFlags();
            _objCmdStatus.CheckMessageLine("警告が発生しましたがなんとか終了しました");
            Assert.False(_objCmdStatus.IsOkMessageHit);
            Assert.True(_objCmdStatus.IsWarnMessageHit);
            Assert.False(_objCmdStatus.IsErrorMessageHit);
        }
        [Fact]
        public void CheckMessageLineで異常終了しましたを評価すると異常該当フラグだけがtrueとなること()
        {
            _objCmdStatus.OkMessageCsv = "正常,いけてる,完璧";
            _objCmdStatus.WarnMessageCsv = "警告,いけてるないかも,完璧でなかった";
            _objCmdStatus.ErrorMessageCsv = "異常,いけてない,失敗";
            _objCmdStatus.Initialize();
            _objCmdStatus.ResetFlags();
            _objCmdStatus.CheckMessageLine("異常終了しました");
            Assert.False(_objCmdStatus.IsOkMessageHit);
            Assert.False(_objCmdStatus.IsWarnMessageHit);
            Assert.True(_objCmdStatus.IsErrorMessageHit);
        }
        [Fact]
        public void CheckMessageLineで警告が発生しましたが正常に終了しましたを評価すると正常と警告該当フラグがtrueとなること()
        {
            _objCmdStatus.OkMessageCsv = "正常,いけてる,完璧";
            _objCmdStatus.WarnMessageCsv = "警告,いけてるないかも,完璧でなかった";
            _objCmdStatus.ErrorMessageCsv = "異常,いけてない,失敗";
            _objCmdStatus.Initialize();
            _objCmdStatus.ResetFlags();
            _objCmdStatus.CheckMessageLine("警告が発生しましたが正常に終了しました");
            Assert.True(_objCmdStatus.IsOkMessageHit);
            Assert.True(_objCmdStatus.IsWarnMessageHit);
            Assert.False(_objCmdStatus.IsErrorMessageHit);
        }

        // --------------------------------------------------------------------
        // public void CheckCmdExitCode(int intCmdExitCode)
        // --------------------------------------------------------------------
        [Fact]
        public void BlnIsAlwaysNormalが正の場合はCheckCmdExitCodeの評価で結果がLVL_Iになること()
        {
            _objCmdStatus.IsAlwaysNormal = true;                                // 常に正常終了フラグON
            _objCmdStatus.CheckCommandExitCode(9999);
            Assert.Equal(MdlConst.LVL_I, _objCmdStatus.MethodExitStatus);    // 0
            Assert.Equal(MdlConst.LVL_I, _objCmdStatus.ReturnLevel);         // 正常
        }
        [Fact]
        public void 警告閾値と異常閾値が設定されていない場合にCheckCmdExitCodeの評価で0の結果がLVL_Iになること()
        {
            int intCmdRetcode = 0;                                              // コマンド戻り値
            _objCmdStatus.WarnThreshold = MdlConst.INT_NULL;                    // 未設定
            _objCmdStatus.ErrorThreshold = MdlConst.INT_NULL;                   // 未設定
            _objCmdStatus.CheckCommandExitCode(intCmdRetcode);
            Assert.Equal(MdlConst.LVL_I, _objCmdStatus.MethodExitStatus);    // 0
            Assert.Equal(MdlConst.LVL_I, _objCmdStatus.ReturnLevel);         // 正常
        }
        [Fact]
        public void 警告閾値と異常閾値が設定されていない場合にCheckCmdExitCodeの評価で1の結果レベルがLVL_Eになること()
        {
            int intCmdRetcode = 1;                                              // コマンド戻り値
            _objCmdStatus.WarnThreshold = MdlConst.INT_NULL;                    // 未設定
            _objCmdStatus.ErrorThreshold = MdlConst.INT_NULL;                   // 未設定
            _objCmdStatus.CheckCommandExitCode(intCmdRetcode);
            Assert.Equal(intCmdRetcode, _objCmdStatus.MethodExitStatus);     // コマンドの戻値
            Assert.Equal(MdlConst.LVL_E, _objCmdStatus.ReturnLevel);         // 異常
        }
        [Fact]
        public void 警告閾値と異常閾値が設定されていない場合にCheckCmdExitCodeの評価で異常時終了コードが20の場合1の戻り値は20になること()
        {
            int intCmdRetcode = 1;                                              // コマンド戻り値
            _objCmdStatus.WarnThreshold = MdlConst.INT_NULL;                    // 未設定
            _objCmdStatus.ErrorThreshold = MdlConst.INT_NULL;                   // 未設定
            _objCmdStatus.ErrorCode = 20;                                       // 異常時終了コード
            _objCmdStatus.CheckCommandExitCode(intCmdRetcode);
            Assert.Equal(20, _objCmdStatus.MethodExitStatus);                // 指定された異常時終了コード
            Assert.Equal(MdlConst.LVL_E, _objCmdStatus.ReturnLevel);         // 異常
        }

    }
}
