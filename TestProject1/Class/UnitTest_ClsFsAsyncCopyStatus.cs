using System;
using System.IO;
using System.Threading.Tasks;
using CmnClsLib.Class;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Class
{
    public class UnitTest_ClsFsAsyncCopyStatus : IDisposable
    {
        private readonly string _tempDirectory;
        private readonly string _tempSourcePath;
        private readonly string _tempDestPath;

        public UnitTest_ClsFsAsyncCopyStatus()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), @"UnitTest", "CmnClsLib", "ClsFsAsyncCopyStatus", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDirectory);
            _tempSourcePath = Path.Combine(_tempDirectory, "source.txt");
            _tempDestPath = Path.Combine(_tempDirectory, "dest.txt");

            // テスト用のソースファイルを生成 (10 KB)
            byte[] dummyData = new byte[10240];
            new Random(42).NextBytes(dummyData);
            File.WriteAllBytes(_tempSourcePath, dummyData);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_tempDirectory))
                {
                    Directory.Delete(_tempDirectory, true);
                }
            }
            catch
            {
                // 一時ディレクトリ削除時の例外は無視
            }
        }

        // --------------------------------------------------------------------
        // コンストラクタ / Initialize (4引数・3引数)
        // --------------------------------------------------------------------
        [Fact]
        public void コンストラクタ4引数_正常なパスを指定した場合_初期化が成功しストリームが開かれること()
        {
            using var status = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: false, FileShare.Read);

            Assert.True(status.IsOk);
            Assert.NotNull(status.SourceStream);
            Assert.NotNull(status.DestinationStream);
            Assert.Equal(10240, status.FileSize);
            Assert.Equal(FileShare.Read, status.FileShare);
            Assert.False(status.IsDone);
        }

        [Fact]
        public void コンストラクタ3引数_正常なパスを指定した場合_デフォルトのFileShareReadWriteで初期化されること()
        {
            using var status = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: true);

            Assert.True(status.IsOk);
            Assert.NotNull(status.SourceStream);
            Assert.NotNull(status.DestinationStream);
            Assert.Equal(FileShare.ReadWrite, status.FileShare);
        }

        [Fact]
        public void コンストラクタ_存在しないソースパスを指定した場合_IsOkがfalseとなりメッセージが設定されること()
        {
            string invalidSource = Path.Combine(_tempDirectory, "non_existent.txt");
            using var status = new ClsFsAsyncCopyStatus(invalidSource, _tempDestPath, isAsync: false);

            Assert.False(status.IsOk);
            Assert.Null(status.SourceStream);
            Assert.False(string.IsNullOrEmpty(status.Message));
        }

        // --------------------------------------------------------------------
        // OpenSourceFile / OpenDestinationFile 単体メソッド
        // --------------------------------------------------------------------
        [Fact]
        public void OpenSourceFile_パスがNullまたは空の場合_falseを返すこと()
        {
            using var status = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: false);
            status.Dispose(); // ストリームを一旦閉じる

            using var status2 = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: false);
            bool resultNull = status2.OpenSourceFile(null!, isAsync: false);
            bool resultEmpty = status2.OpenSourceFile("", isAsync: false);

            Assert.False(resultNull);
            Assert.False(resultEmpty);
        }

        [Fact]
        public void OpenDestinationFile_パスがNullまたは空の場合_falseを返すこと()
        {
            using var status = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: false);

            bool resultNull = status.OpenDestinationFile(null!, isAsync: false);
            bool resultEmpty = status.OpenDestinationFile("", isAsync: false);

            Assert.False(resultNull);
            Assert.False(resultEmpty);
        }

        // --------------------------------------------------------------------
        // Progress / Console 表示関連
        // --------------------------------------------------------------------
        [Fact]
        public void ShowProgress_IsShowProgressがtrueの場合_ProgressLineが正しく更新されること()
        {
            using var status = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: false);
            status.IsShowProgress = true;

            status.ShowProgress();

            Assert.False(string.IsNullOrEmpty(status.ProgressLine));
            Assert.Contains("%", status.ProgressLine);
        }

        [Fact]
        public void ShowProgress_IsShowProgressがfalseの場合_ProgressLineが更新されないこと()
        {
            using var status = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: false);
            status.IsShowProgress = false;
            status.ProgressLine = "Initial";

            status.ShowProgress();

            Assert.Equal("Initial", status.ProgressLine);
        }

        // --------------------------------------------------------------------
        // Dispose / DisposeAsync / Close
        // --------------------------------------------------------------------
        [Fact]
        public void Dispose_実行後にストリームがnullになりIsDoneがtrueになること()
        {
            var status = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: false);
            Assert.NotNull(status.SourceStream);
            Assert.NotNull(status.DestinationStream);

            status.Dispose();

            Assert.True(status.IsDone);
            Assert.Null(status.SourceStream);
            Assert.Null(status.DestinationStream);
        }

        [Fact]
        public async Task DisposeAsync_実行後にストリームがnullになりIsDoneがtrueになること()
        {
            var status = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: true);
            Assert.NotNull(status.SourceStream);
            Assert.NotNull(status.DestinationStream);

            await status.DisposeAsync();

            Assert.True(status.IsDone);
            Assert.Null(status.SourceStream);
            Assert.Null(status.DestinationStream);
        }

        [Fact]
        public void Close_Disposeと同等にリソースが解放されること()
        {
            var status = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: false);
            status.Close();

            Assert.True(status.IsDone);
            Assert.Null(status.SourceStream);
            Assert.Null(status.DestinationStream);
        }

        // --------------------------------------------------------------------
        // Obsolete (非推奨) 互換メソッド・プロパティ
        // --------------------------------------------------------------------
        [Fact]
        public void Obsoleteメンバー_Init_FileOpenFr_FileOpenTo_CurCount_が正しく動作すること()
        {
            string srcPath = Path.Combine(_tempDirectory, "obsolete_src.txt");
            string dstPath = Path.Combine(_tempDirectory, "obsolete_dst.txt");
            File.WriteAllText(srcPath, "Test Content");

            using (var status = new ClsFsAsyncCopyStatus(srcPath, dstPath, isAsync: false))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                status.CurCount = 100;
                Assert.Equal(100, status.CurrentCount);
                Assert.Equal(100, status.CurCount);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            using (var status = new ClsFsAsyncCopyStatus(srcPath, dstPath, isAsync: false))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                bool initResult = status.Init(srcPath, dstPath, isAsync: false, FileShare.ReadWrite);
                Assert.True(initResult, status.Message);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            using (var status = new ClsFsAsyncCopyStatus(srcPath, dstPath, isAsync: false))
            {
                status.Close(); // ストリームをクローズ

#pragma warning disable CS0618 // Type or member is obsolete
                bool frResult = status.FileOpenFr(srcPath, isAsync: false);
                Assert.True(frResult, status.Message);

                status.Close();

                bool toResult = status.FileOpenTo(dstPath, isAsync: false);
                Assert.True(toResult, status.Message);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        // --------------------------------------------------------------------
        // プロパティ Getter / Setter 検証
        // --------------------------------------------------------------------
        [Fact]
        public void プロパティ受渡し検証_値が正しく保持されること()
        {
            using var status = new ClsFsAsyncCopyStatus(_tempSourcePath, _tempDestPath, isAsync: false);

            byte[] newBuffer = new byte[8192];
            status.Buffer = newBuffer;
            Assert.Equal(newBuffer, status.Buffer);

            status.IsDone = true;
            Assert.True(status.IsDone);

            DateTime now = DateTime.Now;
            status.StartTime = now;
            Assert.Equal(now, status.StartTime);

            status.CheckCount = 50;
            Assert.Equal(50, status.CheckCount);

            status.CurrentCount = 20;
            Assert.Equal(20, status.CurrentCount);

            status.FileSize = 2048;
            Assert.Equal(2048, status.FileSize);

            status.IsOk = false;
            Assert.False(status.IsOk);

            status.Message = "TestMessage";
            Assert.Equal("TestMessage", status.Message);

            status.StackTrace = "TestStackTrace";
            Assert.Equal("TestStackTrace", status.StackTrace);

            status.ProgressLine = "TestProgress";
            Assert.Equal("TestProgress", status.ProgressLine);
        }
    }
}
