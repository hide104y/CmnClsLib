using CmnClsLib.Module;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Module
{
    public class UnitTest_MdlArg
    {
        private Dictionary<String, String> _namedArgs = new();
        private String[] _aryArgs = new string[] { "-path", @"C:\Tool\Log", "-v", "--level", "3", "--etc", "---three", "--minus", "-3.4" };

        public UnitTest_MdlArg()
        {
            _namedArgs = MdlArg.GetNamedArgs(_aryArgs);
        }

        // --------------------------------------------------------------------
        // getNamedArgs()
        // --------------------------------------------------------------------
        [Fact]
        public void getNamedArgs_引数の配列をディクショナリーへの変換数が正しいこと()
        {
            int expected = 5;
            Dictionary<String, String> actual = _namedArgs;
            Assert.Equal(expected, actual.Count);
        }

        // --------------------------------------------------------------------
        // ContainsKey()
        // --------------------------------------------------------------------
        [Fact]
        public void IsExistParam_引数にpathを指定した場合はtrueが返却されること()
        {
            bool actual = MdlArg.ContainsKey(_namedArgs, "path");
            Assert.True(actual);
        }

        [Fact]
        public void IsExistParam_引数にvを指定した場合はtrueが返却されること()
        {
            bool actual = MdlArg.ContainsKey(_namedArgs, "v");
            Assert.True(actual);
        }

        [Fact]
        public void IsExistParam_引数にハイフンlevelを指定した場合はtrueが返却されること()
        {
            bool actual = MdlArg.ContainsKey(_namedArgs, "level");
            Assert.True(actual);
        }

        [Fact]
        public void IsExistParam_引数にハイフンnotexistを指定した場合はfalseが返却されること()
        {
            bool actual = MdlArg.ContainsKey(_namedArgs, "notexist");
            Assert.False(actual);
        }

        // --------------------------------------------------------------------
        // GetValue()
        // --------------------------------------------------------------------
        [Fact]
        public void GetValByKey_引数にpathを指定した場合は指定のパスが返却されること()
        {
            string expected = @"C:\Tool\Log";
            string actual = MdlArg.GetValue(_namedArgs, "path");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValByKey_引数にvを指定した場合は空白が返却されること()
        {
            string expected = @"";
            string actual = MdlArg.GetValue(_namedArgs, "v");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValByKey_引数にハイフンlevelを指定した場合は3が返却されること()
        {
            string expected = @"3";
            string actual = MdlArg.GetValue(_namedArgs, "level");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValByKey_引数にハイフンetcを指定した場合はハイフンハイフンハイフンthreeが返却されること()
        {
            string expected = @"---three";
            string actual = MdlArg.GetValue(_namedArgs, "etc");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValByKey_引数にハイフンminusを指定した場合はハイフン3dot4が返却されること()
        {
            string expected = @"-3.4";
            string actual = MdlArg.GetValue(_namedArgs, "minus");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValByKey_引数にハイフンnotexistを指定した場合はEmptyが返却されること()
        {
            string expected = @"";
            string actual = MdlArg.GetValue(_namedArgs, "notexist");
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // GetFullPath()
        // --------------------------------------------------------------------
        [Fact]
        public void GetFullPath_引数にpathを指定した場合は指定のパスが返却されること()
        {
            string expected = @"C:\Tool\Log";
            string actual = MdlArg.GetFullPath(_namedArgs, "path");
            Assert.Equal(expected, actual);
        }
    }
}
