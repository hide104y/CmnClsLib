using Xunit;
using Assert = Xunit.Assert;
using CmnClsLib.Class;
using CmnClsLib.Interface;
using CmnClsLib.Module;

namespace TestProject1.Class
{
    public class UnitTest_ClsCmmnArgs
    {
        private readonly string _tempDir = Path.Combine(Path.GetTempPath(), @"UnitTest", "CmnClsLib", "ClsCmmnArgs");

        private ClsCmmnArgs CreateArgs()
        {
            ICmnLogger logger = new ClsLogger();
            return new ClsCmmnArgs(logger);
        }

        [Fact]
        public void Test_InitializeLists()
        {
            var cmmnArgs = CreateArgs();
            cmmnArgs.InitializeLists();
            Assert.Contains("username", cmmnArgs.KeyNameOfUsernameList);
            Assert.Contains("password", cmmnArgs.KeyNameOfPasswordList);
        }

        [Fact]
        public void Test_GetModuleInfo()
        {
            var cmmnArgs = CreateArgs();
            bool result = cmmnArgs.GetModuleInfo();
            Assert.True(result);
            Assert.NotEmpty(cmmnArgs.ExeBaseName);
            Assert.True(cmmnArgs.Pid > 0);
        }

        [Fact]
        public void Test_SplitUserAndDomain()
        {
            var cmmnArgs = CreateArgs();
            cmmnArgs.Username = @"TESTDOMAIN\TestUser";
            cmmnArgs.SplitUserAndDomain();

            Assert.Equal("TESTDOMAIN", cmmnArgs.DomainName);
            Assert.Equal("TestUser", cmmnArgs.UsernameWithoutDomain);
        }

        [Fact]
        public void Test_GetArgsForUser()
        {
            var cmmnArgs = CreateArgs();
            cmmnArgs.NamedArgs = new Dictionary<string, string>
            {
                { "u", "AdminUser" },
                { "domain", "CORPDOMAIN" }
            };

            bool ok = cmmnArgs.GetArgsForUser();
            Assert.True(ok);
            Assert.Equal("AdminUser", cmmnArgs.UsernameWithoutDomain);
            Assert.Equal("CORPDOMAIN", cmmnArgs.DomainName);
        }

        [Fact]
        public void Test_ReplaceByDictionary_And_ObsoleteReplaceByDic()
        {
            var cmmnArgs = CreateArgs();
            cmmnArgs.ReplaceDic["__ENV_ID__"] = "PRODUCTION";

            string replaced1 = cmmnArgs.ReplaceByDictionary(@"C:\Data\__ENV_ID__\log.txt");
            Assert.Equal(@"C:\Data\PRODUCTION\log.txt", replaced1);
        }

        [Fact]
        public void Test_GetPathParam()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }

            var cmmnArgs = CreateArgs();
            cmmnArgs.NamedArgs["ldir"] = _tempDir;

            string path = cmmnArgs.GetPathParam("ldir", MdlFile.PATH_IS_DIRECTORY, true);
            Assert.Equal(_tempDir, path);
            Assert.True(Directory.Exists(_tempDir));

            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }

        [Fact]
        public void Test_GetCommonArgs()
        {
            var cmmnArgs = CreateArgs();
            cmmnArgs.NamedArgs = new Dictionary<string, string>
            {
                { "v", "3" },
                { "force", "" },
                { "diff", "2" }
            };

            bool ok = cmmnArgs.GetCommonArgs();
            Assert.True(ok);
            Assert.Equal(3, cmmnArgs.Verbose);
            Assert.True(cmmnArgs.IsForce);
            Assert.True(cmmnArgs.IsDiff);
            Assert.Equal(2, cmmnArgs.DiffLevel);
        }

        [Fact]
        public void Test_DecryptKeyAndPassword_And_ObsoleteWrapper()
        {
            var cmmnArgs = CreateArgs();
            cmmnArgs.IsDecodeKey = false;
            cmmnArgs.IsDecodePasswd = false;

            bool ok1 = cmmnArgs.DecryptKeyAndPassword();
            Assert.True(ok1);
        }

        [Fact]
        public void Test_WriteLine_And_ObsoleteWriteln()
        {
            var cmmnArgs = CreateArgs();
            cmmnArgs.WriteLine(MdlConst.LVL_NONE, "Test message");
        }

        [Fact]
        public void Test_ShowUsage_And_ObsoleteUsage()
        {
            var cmmnArgs = CreateArgs();
            cmmnArgs.ShowUsage();
        }
    }
}
