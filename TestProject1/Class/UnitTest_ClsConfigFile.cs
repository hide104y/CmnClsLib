using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CmnClsLib.Class;
using CmnClsLib.Interface;
using CmnClsLib.Module;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Class
{
    public class UnitTest_ClsConfigFile
    {
        private class TestLogger : ICmnLogger
        {
            public List<(int Level, string Message)> Logs { get; } = new();

            public void WriteLine(int level, string message)
            {
                Logs.Add((level, message));
            }

            public string GetValueByKey(string key, string defaultValue) => defaultValue;
            public bool GetValueByKey(string key, bool defaultValue) => defaultValue;
            public void SetValueByKey(string key, string val) { }
            
#pragma warning disable CS0618 // 旧型の型またはメンバーが追跡されています
            public string GetValByKey(string key, string defaultValue) => defaultValue;
            public bool GetValByKey(string key, bool defaultValue) => defaultValue;
            public void SetValByKey(string key, string val) { }
            public void Writeln(int level, string msg) => WriteLine(level, msg);
#pragma warning restore CS0618
        }

        private string CreateTempFile(string content, Encoding? encoding = null)
        {
            string tempPath = Path.GetTempFileName();
            if (encoding != null)
            {
                File.WriteAllText(tempPath, content, encoding);
            }
            else
            {
                File.WriteAllText(tempPath, content);
            }
            return tempPath;
        }

        private void DeleteTempFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public void Constructor_InitializesEmptyCollections()
        {
            var logger = new TestLogger();
            var configFile = new ClsConfigFile(logger);

            Assert.NotNull(configFile.ConfigDictionary);
            Assert.Empty(configFile.ConfigDictionary);
            Assert.NotNull(configFile.ListDictionary);
            Assert.Empty(configFile.ListDictionary);
            Assert.NotNull(configFile.ConfigList);
            Assert.Empty(configFile.ConfigList);
            Assert.NotNull(configFile.DuplicateKeys);
            Assert.Empty(configFile.DuplicateKeys);
        }

        [Fact]
        public void Clear_ClearsAllCollections()
        {
            var logger = new TestLogger();
            var configFile = new ClsConfigFile(logger);

            configFile.ConfigDictionary["Key1"] = "Value1";
            configFile.ListDictionary["DupKey"] = new List<string> { "Val1", "Val2" };
            configFile.ConfigList.Add("Line1");
            configFile.DuplicateKeys.Add("DupKey");

            configFile.Clear();

            Assert.Empty(configFile.ConfigDictionary);
            Assert.Empty(configFile.ListDictionary);
            Assert.Empty(configFile.ConfigList);
            Assert.Empty(configFile.DuplicateKeys);
        }

        [Fact]
        public void Properties_GetAndSet_WorksCorrectly()
        {
            var logger = new TestLogger();
            var configFile = new ClsConfigFile(logger);

            configFile.Verbose = 5;
            Assert.Equal(5, configFile.Verbose);

            string customPattern = @"^(?<KEY>[^:]+):(?<VALUE>.+)$";
            configFile.Pattern = customPattern;
            Assert.Equal(customPattern, configFile.Pattern);

            configFile.Encoding = Encoding.UTF8;
            Assert.Equal(Encoding.UTF8, configFile.Encoding);

            configFile.IsSkipComment = false;
            Assert.False(configFile.IsSkipComment);
        }

#pragma warning disable CS0618 // 旧型の型またはメンバーが追跡されています
        [Fact]
        public void ObsoleteProperties_GetAndSet_WorksCorrectly()
        {
            var logger = new TestLogger();
            var configFile = new ClsConfigFile(logger);

            var dict = new Dictionary<string, string> { { "K", "V" } };
            configFile.ConfigDic = dict;
            Assert.Same(dict, configFile.ConfigDictionary);

            var listDict = new Dictionary<string, List<string>> { { "K", new List<string> { "V" } } };
            configFile.ListDic = listDict;
            Assert.Same(listDict, configFile.ListDictionary);

            var dupKeys = new List<string> { "Key1" };
            configFile.DuplicateKeyList = dupKeys;
            Assert.Same(dupKeys, configFile.DuplicateKeys);
        }
#pragma warning restore CS0618

        [Fact]
        public void LoadToDictionary_ValidFile_ParsesKeyValuePairs()
        {
            string content = "Server=Localhost\nPort=8080\nTitle=\"My Application\"";
            string tempFile = CreateTempFile(content, Encoding.UTF8);

            try
            {
                var logger = new TestLogger();
                var configFile = new ClsConfigFile(logger);

                int count = configFile.LoadToDictionary(tempFile);

                Assert.Equal(3, count);
                Assert.Equal("Localhost", configFile.ConfigDictionary["Server"]);
                Assert.Equal("8080", configFile.ConfigDictionary["Port"]);
                Assert.Equal("My Application", configFile.ConfigDictionary["Title"]);
            }
            finally
            {
                DeleteTempFile(tempFile);
            }
        }

        [Fact]
        public void LoadToDictionary_WithCommentsAndEmptyLines_SkipsCorrectly()
        {
            string content = "# This is a comment\n\nKey1=Val1 # inline comment\n\n# Another comment\nKey2=Val2";
            string tempFile = CreateTempFile(content, Encoding.UTF8);

            try
            {
                var logger = new TestLogger();
                var configFile = new ClsConfigFile(logger);
                configFile.IsSkipComment = true;

                int count = configFile.LoadToDictionary(tempFile);

                Assert.Equal(2, count);
                Assert.Equal("Val1", configFile.ConfigDictionary["Key1"]);
                Assert.Equal("Val2", configFile.ConfigDictionary["Key2"]);
            }
            finally
            {
                DeleteTempFile(tempFile);
            }
        }

        [Fact]
        public void LoadToDictionary_DuplicateKeys_StoresMultipleValuesInListDictionary()
        {
            string content = "Item=Apple\nItem=Banana\nOther=Single";
            string tempFile = CreateTempFile(content, Encoding.UTF8);

            try
            {
                var logger = new TestLogger();
                var configFile = new ClsConfigFile(logger);
                configFile.DuplicateKeys.Add("Item");

                int count = configFile.LoadToDictionary(tempFile);

                Assert.Equal(2, count);
                Assert.Equal("Banana", configFile.ConfigDictionary["Item"]);
                Assert.True(configFile.ListDictionary.ContainsKey("Item"));
                Assert.Equal(new[] { "Apple", "Banana" }, configFile.ListDictionary["Item"]);
            }
            finally
            {
                DeleteTempFile(tempFile);
            }
        }

        [Fact]
        public void LoadToDictionary_CustomPattern_ParsesWithCustomRegex()
        {
            string content = "Key1:Val1\nKey2:Val2";
            string tempFile = CreateTempFile(content, Encoding.UTF8);

            try
            {
                var logger = new TestLogger();
                var configFile = new ClsConfigFile(logger);
                configFile.Pattern = "^(?<KEY>[^:]+):(?<VALUE>.+)$";

                int count = configFile.LoadToDictionary(tempFile);

                Assert.Equal(2, count);
                Assert.Equal("Val1", configFile.ConfigDictionary["Key1"]);
                Assert.Equal("Val2", configFile.ConfigDictionary["Key2"]);
            }
            finally
            {
                DeleteTempFile(tempFile);
            }
        }

        [Fact]
        public void LoadToDictionary_FileNotFound_ReturnsMinusOne()
        {
            var logger = new TestLogger();
            var configFile = new ClsConfigFile(logger);

            int result = configFile.LoadToDictionary(@"C:\non_existent_file_123456.conf");

            Assert.Equal(-1, result);
            Assert.NotEmpty(logger.Logs);
        }

        [Fact]
        public void LoadToList_UniqueFalse_ReadsAllLines()
        {
            string content = "Line1\nLine2\nLine1\nLine3";
            string tempFile = CreateTempFile(content, Encoding.UTF8);

            try
            {
                var logger = new TestLogger();
                var configFile = new ClsConfigFile(logger);

                int count = configFile.LoadToList(tempFile, unique: false);

                Assert.Equal(4, count);
                Assert.Equal(new[] { "Line1", "Line2", "Line1", "Line3" }, configFile.ConfigList);
            }
            finally
            {
                DeleteTempFile(tempFile);
            }
        }

        [Fact]
        public void LoadToList_UniqueTrue_RemovesDuplicates()
        {
            string content = "Line1\nLine2\nLine1\nLine3";
            string tempFile = CreateTempFile(content, Encoding.UTF8);

            try
            {
                var logger = new TestLogger();
                var configFile = new ClsConfigFile(logger);

                int count = configFile.LoadToList(tempFile, unique: true);

                Assert.Equal(3, count);
                Assert.Equal(new[] { "Line1", "Line2", "Line3" }, configFile.ConfigList);
            }
            finally
            {
                DeleteTempFile(tempFile);
            }
        }

        [Fact]
        public void LoadToList_FileNotFound_ReturnsMinusOne()
        {
            var logger = new TestLogger();
            var configFile = new ClsConfigFile(logger);

            int result = configFile.LoadToList(@"C:\non_existent_file_123456.conf", unique: false);

            Assert.Equal(-1, result);
            Assert.NotEmpty(logger.Logs);
        }

#pragma warning disable CS0618 // 旧型の型またはメンバーが追跡されています
        [Fact]
        public void ObsoleteMethods_DelegateCorrectly()
        {
            string content = "Key1=Val1\nKey2=Val2";
            string tempFile = CreateTempFile(content, Encoding.UTF8);

            try
            {
                var logger = new TestLogger();
                var configFile = new ClsConfigFile(logger);

                int dictCount = configFile.ReadFile(tempFile);
                Assert.Equal(2, dictCount);

                configFile.Clear();

                int listCount = configFile.ReadFileToList(tempFile, unique: false);
                Assert.Equal(2, listCount);

                configFile.Writeln(1, "Test log message");
                Assert.Contains(logger.Logs, log => log.Level == 1 && log.Message == "Test log message");
            }
            finally
            {
                DeleteTempFile(tempFile);
            }
        }
#pragma warning restore CS0618

        [Fact]
        public void WriteLog_WithLogger_CallsLoggerWriteLine()
        {
            var logger = new TestLogger();
            var configFile = new ClsConfigFile(logger);

            configFile.WriteLog(MdlConst.LVL_DEBUG, "Test Debug Message");

            Assert.Single(logger.Logs);
            Assert.Equal(MdlConst.LVL_DEBUG, logger.Logs[0].Level);
            Assert.Equal("Test Debug Message", logger.Logs[0].Message);
        }

        [Fact]
        public void WriteLog_NullLogger_OutputsToConsoleWithoutException()
        {
            // Null logger constructed using reflection or internal state if possible,
            // or pass ICmnLogger via null!
            var configFile = new ClsConfigFile(null!);

            var exception = Record.Exception(() =>configFile.WriteLog(MdlConst.LVL_DEBUG, "Test Console Log"));

            Assert.Null(exception);
        }
    }
}
