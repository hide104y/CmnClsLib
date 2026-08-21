using System;
using System.Collections.Generic;
using System.IO;
using CmnClsLib.Class;
using CmnClsLib.Interface;
using CmnClsLib.Module;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Class
{
    public class UnitTest_ClsFsDate : IDisposable
    {
        private readonly string _tempDirectory;
        private readonly string _tempFilePath;
        private readonly string _tempSubDirPath;
        private readonly TestLogger _logger;

        public UnitTest_ClsFsDate()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), @"UnitTest", "CmnClsLib", "ClsFsDate", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDirectory);

            _tempFilePath = Path.Combine(_tempDirectory, "test_file.txt");
            File.WriteAllText(_tempFilePath, "ClsFsDate Test Content");

            _tempSubDirPath = Path.Combine(_tempDirectory, "test_subdir");
            Directory.CreateDirectory(_tempSubDirPath);

            _logger = new TestLogger();
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
                // テスト用一時ディレクトリ削除時の例外は無視
            }
        }

        // --------------------------------------------------------------------
        // コンストラクタ / プロパティ 検証
        // --------------------------------------------------------------------
        [Fact]
        public void コンストラクタ_NullLoggerを指定した場合_ArgumentNullExceptionがスローされること()
        {
            Assert.Throws<ArgumentNullException>(() => new ClsFsDate(null!));
        }

        [Fact]
        public void コンストラクタ_正常なLoggerを指定した場合_初期値が正しいこと()
        {
            var fsDate = new ClsFsDate(_logger);

            Assert.Equal(0, fsDate.Verbose);
            Assert.Equal(string.Empty, fsDate.Message);
            Assert.False(fsDate.IsThrowIfException);
        }

        [Fact]
        public void プロパティ受渡し検証_値が正しく設定および取得できること()
        {
            var fsDate = new ClsFsDate(_logger);

            fsDate.Verbose = 2;
            Assert.Equal(2, fsDate.Verbose);

            fsDate.Message = "Test Error";
            Assert.Equal("Test Error", fsDate.Message);

            fsDate.IsThrowIfException = true;
            Assert.True(fsDate.IsThrowIfException);
        }

        [Fact]
        public void Obsoleteプロパティ_IsThrowIfExcptn_IsThrowIfExceptionと同期すること()
        {
            var fsDate = new ClsFsDate(_logger);

#pragma warning disable CS0618 // Type or member is obsolete
            fsDate.IsThrowIfExcptn = true;
            Assert.True(fsDate.IsThrowIfException);
            Assert.True(fsDate.IsThrowIfExcptn);

            fsDate.IsThrowIfException = false;
            Assert.False(fsDate.IsThrowIfExcptn);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // --------------------------------------------------------------------
        // SetFileDate / SetFileDateCore / Obsolete SetDateToFile / SetDateToFileMain 検証
        // --------------------------------------------------------------------
        [Fact]
        public void SetFileDate_正常なファイルパスと日時を指定した場合_trueを返し日時が変更されること()
        {
            var fsDate = new ClsFsDate(_logger);
            DateTime targetDate = new DateTime(2025, 5, 20, 10, 30, 0);

            // mode: 7 (全属性: 作成・更新・アクセス日時を設定)
            bool result = fsDate.SetFileDate(_tempFilePath, targetDate, 7, isForce: true, isExec: true);

            Assert.True(result);
            DateTime actualWriteTime = File.GetLastWriteTime(_tempFilePath);
            Assert.Equal(targetDate.Year, actualWriteTime.Year);
            Assert.Equal(targetDate.Month, actualWriteTime.Month);
            Assert.Equal(targetDate.Day, actualWriteTime.Day);
        }

        [Fact]
        public void SetFileDate_オーバーロード_引数省略版が正しく動作すること()
        {
            var fsDate = new ClsFsDate(_logger);
            DateTime targetDate = new DateTime(2025, 6, 15, 14, 0, 0);

            bool res1 = fsDate.SetFileDate(_tempFilePath, targetDate, 7, isForce: true);
            Assert.True(res1);

            bool res2 = fsDate.SetFileDate(_tempFilePath, targetDate, 7);
            Assert.True(res2);
        }

        [Fact]
        public void SetFileDateCore_存在しないファイルパスを指定した場合_処理結果コードが返されること()
        {
            var fsDate = new ClsFsDate(_logger);
            string invalidPath = Path.Combine(_tempDirectory, "non_existent_file.txt");
            DateTime targetDate = DateTime.Now;

            int code = fsDate.SetFileDateCore(invalidPath, targetDate, 7, isForce: true, isExec: true);

            Assert.True(code >= -1);
        }

        [Fact]
        public void SetFileDateCore_例外が発生しIsThrowIfExceptionがtrueの場合_例外が再スローされログ出力されること()
        {
            var fsDate = new ClsFsDate(_logger)
            {
                Verbose = 1,
                IsThrowIfException = true
            };

            string invalidPath = @"Z:\NonExistentDirectory_12345\non_existent_file.txt";
            DateTime targetDate = DateTime.Now;

            Assert.ThrowsAny<Exception>(() => fsDate.SetFileDateCore(invalidPath, targetDate, 7));
            Assert.NotEmpty(_logger.Logs);
        }

        [Fact]
        public void Obsoleteメソッド_SetDateToFile_SetDateToFileMain_正常に動作すること()
        {
            var fsDate = new ClsFsDate(_logger);
            DateTime targetDate = new DateTime(2025, 3, 10, 12, 0, 0);

#pragma warning disable CS0618 // Type or member is obsolete
            bool resBool1 = fsDate.SetDateToFile(_tempFilePath, targetDate, 7, isForce: true, isExec: true);
            Assert.True(resBool1);

            bool resBool2 = fsDate.SetDateToFile(_tempFilePath, targetDate, 7, isForce: true);
            Assert.True(resBool2);

            bool resBool3 = fsDate.SetDateToFile(_tempFilePath, targetDate, 7);
            Assert.True(resBool3);

            int resCode1 = fsDate.SetDateToFileMain(_tempFilePath, targetDate, 7, isForce: true, isExec: true);
            Assert.True(resCode1 >= 0);

            int resCode2 = fsDate.SetDateToFileMain(_tempFilePath, targetDate, 7, isForce: true);
            Assert.True(resCode2 >= 0);

            int resCode3 = fsDate.SetDateToFileMain(_tempFilePath, targetDate, 7);
            Assert.True(resCode3 >= 0);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // --------------------------------------------------------------------
        // SetDirectoryDate / SetDirectoryDateCore / Obsolete SetDateToDir / SetDateToDirMain 検証
        // --------------------------------------------------------------------
        [Fact]
        public void SetDirectoryDate_正常なディレクトリパスと日時を指定した場合_trueを返し日時が変更されること()
        {
            var fsDate = new ClsFsDate(_logger);
            DateTime targetDate = new DateTime(2025, 7, 10, 9, 15, 0);

            bool result = fsDate.SetDirectoryDate(_tempSubDirPath, targetDate, 7, isForce: true, isExec: true);

            Assert.True(result);
            DateTime actualWriteTime = Directory.GetLastWriteTime(_tempSubDirPath);
            Assert.Equal(targetDate.Year, actualWriteTime.Year);
            Assert.Equal(targetDate.Month, actualWriteTime.Month);
            Assert.Equal(targetDate.Day, actualWriteTime.Day);
        }

        [Fact]
        public void SetDirectoryDate_オーバーロード_引数省略版が正しく動作すること()
        {
            var fsDate = new ClsFsDate(_logger);
            DateTime targetDate = new DateTime(2025, 8, 1, 10, 0, 0);

            bool res1 = fsDate.SetDirectoryDate(_tempSubDirPath, targetDate, 7, isForce: true);
            Assert.True(res1);

            bool res2 = fsDate.SetDirectoryDate(_tempSubDirPath, targetDate, 7);
            Assert.True(res2);
        }

        [Fact]
        public void SetDirectoryDateCore_存在しないディレクトリパスを指定した場合_処理コードが返されること()
        {
            var fsDate = new ClsFsDate(_logger);
            string invalidDir = Path.Combine(_tempDirectory, "non_existent_dir");
            DateTime targetDate = DateTime.Now;

            int code = fsDate.SetDirectoryDateCore(invalidDir, targetDate, 7, isForce: true, isExec: true);

            Assert.True(code >= -1);
        }

        [Fact]
        public void SetDirectoryDateCore_例外が発生しIsThrowIfExceptionがtrueの場合_例外が再スローされログ出力されること()
        {
            var fsDate = new ClsFsDate(_logger)
            {
                Verbose = 1,
                IsThrowIfException = true
            };

            string invalidDir = @"Z:\NonExistentDirectory_12345\non_existent_dir";
            DateTime targetDate = DateTime.Now;

            Assert.ThrowsAny<Exception>(() => fsDate.SetDirectoryDateCore(invalidDir, targetDate, 7));
            Assert.NotEmpty(_logger.Logs);
        }

        [Fact]
        public void Obsoleteメソッド_SetDateToDir_SetDateToDirMain_正常に動作すること()
        {
            var fsDate = new ClsFsDate(_logger);
            DateTime targetDate = new DateTime(2025, 4, 5, 11, 0, 0);

#pragma warning disable CS0618 // Type or member is obsolete
            bool resBool1 = fsDate.SetDateToDir(_tempSubDirPath, targetDate, 7, isForce: true, isExec: true);
            Assert.True(resBool1);

            bool resBool2 = fsDate.SetDateToDir(_tempSubDirPath, targetDate, 7, isForce: true);
            Assert.True(resBool2);

            bool resBool3 = fsDate.SetDateToDir(_tempSubDirPath, targetDate, 7);
            Assert.True(resBool3);

            int resCode1 = fsDate.SetDateToDirMain(_tempSubDirPath, targetDate, 7, isForce: true, isExec: true);
            Assert.True(resCode1 >= 0);

            int resCode2 = fsDate.SetDateToDirMain(_tempSubDirPath, targetDate, 7, isForce: true);
            Assert.True(resCode2 >= 0);

            int resCode3 = fsDate.SetDateToDirMain(_tempSubDirPath, targetDate, 7);
            Assert.True(resCode3 >= 0);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // --------------------------------------------------------------------
        // SetDate / SetDateCore / Obsolete SetDateMain (文字列指定版) 検証
        // --------------------------------------------------------------------
        [Fact]
        public void SetDate_正常なファイルパスと日付文字列を指定した場合_trueを返し日時が変更されること()
        {
            var fsDate = new ClsFsDate(_logger);
            string dateStr = "2025/11/15 16:45:00";

            // MdlFile.PATH_AUTO_DETECT (9) を使用
            bool result = fsDate.SetDate(_tempFilePath, dateStr, mode: 7, pathKind: MdlFile.PATH_AUTO_DETECT, isValidateDate: true, isForce: true, isExec: true);

            Assert.True(result);
            DateTime actualWriteTime = File.GetLastWriteTime(_tempFilePath);
            Assert.Equal(2025, actualWriteTime.Year);
            Assert.Equal(11, actualWriteTime.Month);
            Assert.Equal(15, actualWriteTime.Day);
        }

        [Fact]
        public void SetDate_オーバーロード_引数省略版が正しく動作すること()
        {
            var fsDate = new ClsFsDate(_logger);
            string dateStr = "2025/12/01 08:00:00";

            bool res1 = fsDate.SetDate(_tempFilePath, dateStr, mode: 7, pathKind: MdlFile.PATH_AUTO_DETECT, isValidateDate: true, isForce: true);
            Assert.True(res1);

            bool res2 = fsDate.SetDate(_tempFilePath, dateStr, mode: 7, pathKind: MdlFile.PATH_AUTO_DETECT, isValidateDate: true);
            Assert.True(res2);
        }

        [Fact]
        public void SetDateCore_存在しないパスを指定した場合_マイナス1を返すこと()
        {
            var fsDate = new ClsFsDate(_logger);
            string invalidPath = Path.Combine(_tempDirectory, "non_existent_file_date.txt");

            int code = fsDate.SetDateCore(invalidPath, "2025/01/01", mode: 7, pathKind: MdlFile.PATH_AUTO_DETECT, isValidateDate: true, isForce: true, isExec: true);

            Assert.Equal(-1, code);
        }

        [Fact]
        public void SetDateCore_例外が発生しIsThrowIfExceptionがtrueの場合_例外が再スローされログ出力されること()
        {
            var fsDate = new ClsFsDate(_logger)
            {
                Verbose = 1,
                IsThrowIfException = true
            };

            // isValidateDate = false で不適切な日付文字列を渡し、DateTime.Parse で例外を誘発させる
            Assert.ThrowsAny<Exception>(() => fsDate.SetDateCore(_tempFilePath, "invalid_date_format_xyz", mode: 7, pathKind: MdlFile.PATH_AUTO_DETECT, isValidateDate: false));
            Assert.NotEmpty(_logger.Logs);
        }

        [Fact]
        public void Obsoleteメソッド_SetDateMain_正常に動作すること()
        {
            var fsDate = new ClsFsDate(_logger);
            string dateStr = "2025/09/09 09:09:09";

#pragma warning disable CS0618 // Type or member is obsolete
            int res1 = fsDate.SetDateMain(_tempFilePath, dateStr, mode: 7, kindOfPath: MdlFile.PATH_AUTO_DETECT, isValidateDate: true, isForce: true, isExec: true);
            Assert.True(res1 >= 0);

            int res2 = fsDate.SetDateMain(_tempFilePath, dateStr, mode: 7, kindOfPath: MdlFile.PATH_AUTO_DETECT, isValidateDate: true, isForce: true);
            Assert.True(res2 >= 0);

            int res3 = fsDate.SetDateMain(_tempFilePath, dateStr, mode: 7, kindOfPath: MdlFile.PATH_AUTO_DETECT, isValidateDate: true);
            Assert.True(res3 >= 0);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // --------------------------------------------------------------------
        // テスト用ロガークラス
        // --------------------------------------------------------------------
        private class TestLogger : ICmnLogger
        {
            public List<string> Logs { get; } = new List<string>();

            public void WriteLine(int level, string message)
            {
                Logs.Add($"[{level}] {message}");
            }

            public void WriteLine(string message)
            {
                Logs.Add(message);
            }

            public string GetValueByKey(string key, string defaultValue = "")
            {
                return defaultValue;
            }

            public bool GetValueByKey(string key, bool defaultValue)
            {
                return defaultValue;
            }

            public void SetValueByKey(string key, string value)
            {
            }

#pragma warning disable CS0618 // Type or member is obsolete
            public string GetValByKey(string key, string defaultValue) => GetValueByKey(key, defaultValue);
            public bool GetValByKey(string key, bool defaultValue) => GetValueByKey(key, defaultValue);
            public void SetValByKey(string key, string val) => SetValueByKey(key, val);
            public void Writeln(int level, string msg) => WriteLine(level, msg);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
