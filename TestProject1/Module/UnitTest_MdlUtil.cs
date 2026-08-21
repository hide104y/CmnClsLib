using CmnClsLib.Module;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Module
{
    public class UnitTest_MdlUtil
    {
        // --------------------------------------------------------------------
        // IsNumeric()
        // --------------------------------------------------------------------
        [Fact]
        public void IsNumeric_引数が正の整数文字列の場合はTRUEを返却すること()
        {
            Assert.True(MdlUtil.IsNumeric("123"));
        }

        [Fact]
        public void IsNumeric_引数がPLUS整数文字列の場合はTRUEを返却すること()
        {
            Assert.True(MdlUtil.IsNumeric("+123"));
        }

        [Fact]
        public void IsNumeric_引数が負の整数文字列の場合はTRUEを返却すること()
        {
            Assert.True(MdlUtil.IsNumeric("-123"));
        }

        [Fact]
        public void IsNumeric_引数が正の浮動小数文字列の場合はTRUEを返却すること()
        {
            Assert.True(MdlUtil.IsNumeric("123.23"));
        }

        [Fact]
        public void IsNumeric_引数が負の浮動小数文字列の場合はTRUEを返却すること()
        {
            Assert.True(MdlUtil.IsNumeric("-123.23"));
        }

        [Fact]
        public void IsNumeric_引数の正の整数文字列にカンマが含まれる場合はTRUEを返却すること()
        {
            Assert.True(MdlUtil.IsNumeric("123,323"));
        }

        [Fact]
        public void IsNumeric_引数の負の整数文字列にカンマが含まれる場合はTRUEを返却すること()
        {
            Assert.True(MdlUtil.IsNumeric("-123,323"));
        }

        [Fact]
        public void IsNumeric_引数がNULLの場合FLAS返却すること()
        {
            Assert.False(MdlUtil.IsNumeric(null));
        }

        [Fact]
        public void IsNumeric_引数がから文字列の場合FLAS返却すること()
        {
            Assert.False(MdlUtil.IsNumeric(""));
        }

        [Fact]
        public void IsNumeric_引数が数字でない文字列の場合はFALSEを返却すること()
        {
            Assert.False(MdlUtil.IsNumeric("abc000"));
        }

        [Fact]
        public void IsNumeric_引数がオブジェクトの場合はFALSEを返却すること()
        {
            Object objTmp = new object();
            Assert.False(MdlUtil.IsNumeric(objTmp));
        }

        // --------------------------------------------------------------------
        // ParseInt()
        // --------------------------------------------------------------------
        [Fact]
        public void ParseInt_引数が正の整数文字列の場合はその値を整数で返却すること()
        {
            int expected = 123;
            Assert.Equal(expected, MdlUtil.ParseInt(expected.ToString(), MdlConst.INT_NULL));
        }

        [Fact]
        public void ParseInt_引数がPLUS付正の整数文字列の場合はその値を整数で返却すること()
        {
            int expected = 123;
            Assert.Equal(expected, MdlUtil.ParseInt("+" + expected.ToString(), MdlConst.INT_NULL));
        }

        [Fact]
        public void ParseInt_引数が負の整数文字列の場合はその値を整数で返却すること()
        {
            int expected = -123;
            Assert.Equal(expected, MdlUtil.ParseInt(expected.ToString(), MdlConst.INT_NULL));
        }

        [Fact]
        public void ParseInt_引数が正の浮動小数文字列の場合はその値の小数点以下切り捨ての整数で返却すること()
        {
            Assert.Equal(123, MdlUtil.ParseInt("123.93", MdlConst.INT_NULL));
        }

        [Fact]
        public void ParseInt_引数が負の浮動小数文字列の場合はその値を整数で返却すること()
        {
            Assert.Equal(-123, MdlUtil.ParseInt("-123.93", MdlConst.INT_NULL));
        }

        [Fact]
        public void ParseInt_引数の正の整数文字列にカンマが含まれる場合はその値の小数点以下切り捨ての整数で返却すること()
        {
            Assert.Equal(123323, MdlUtil.ParseInt("123,323", MdlConst.INT_NULL));
        }

        [Fact]
        public void ParseInt_引数の負の整数文字列にカンマが含まれる場合はその値を整数で返却すること()
        {
            Assert.Equal(-123323, MdlUtil.ParseInt("-123,323", MdlConst.INT_NULL));
        }

        [Fact]
        public void ParseInt_引数がNULLの場合は定数INT_NULLの値を返却すること()
        {
            Assert.Equal(MdlConst.INT_NULL, MdlUtil.ParseInt(null, MdlConst.INT_NULL));
        }

        [Fact]
        public void ParseInt_引数がから文字列の場合は定数INT_NULLの値を返却すること()
        {
            Assert.Equal(MdlConst.INT_NULL, MdlUtil.ParseInt("", MdlConst.INT_NULL));
        }

        [Fact]
        public void ParseInt_引数が数字でない文字列の場合は定数INT_NULLの値を返却すること()
        {
            Assert.Equal(MdlConst.INT_NULL, MdlUtil.ParseInt("abc000", MdlConst.INT_NULL));
        }

        // --------------------------------------------------------------------
        // ParseLong()
        // --------------------------------------------------------------------
        [Fact]
        public void ParseLong_引数が正の整数文字列の場合はその値を整数で返却すること()
        {
            int expected = 123;
            Assert.Equal((long)expected, MdlUtil.ParseLong(expected.ToString(), MdlConst.LNG_NULL));
        }

        [Fact]
        public void ParseLong_引数がPLUS付正の整数文字列の場合はその値を整数で返却すること()
        {
            int expected = 123;
            Assert.Equal((long)expected, MdlUtil.ParseLong("+" + expected.ToString(), MdlConst.LNG_NULL));
        }

        [Fact]
        public void ParseLong_引数が負の整数文字列の場合はその値を整数で返却すること()
        {
            int expected = -123;
            Assert.Equal((long)expected, MdlUtil.ParseLong(expected.ToString(), MdlConst.LNG_NULL));
        }

        [Fact]
        public void ParseLong_引数が正の浮動小数文字列の場合はその値の小数点以下切り捨ての整数で返却すること()
        {
            Assert.Equal(123, MdlUtil.ParseLong("123.93", MdlConst.LNG_NULL));
        }

        [Fact]
        public void ParseLong_引数が負の浮動小数文字列の場合はその値を整数で返却すること()
        {
            Assert.Equal(-123, MdlUtil.ParseLong("-123.93", MdlConst.LNG_NULL));
        }

        [Fact]
        public void ParseLong_引数の正の整数文字列にカンマが含まれる場合はその値の小数点以下切り捨ての整数で返却すること()
        {
            Assert.Equal(123323, MdlUtil.ParseLong("123,323", MdlConst.LNG_NULL));
        }

        [Fact]
        public void ParseLong_引数の負の整数文字列にカンマが含まれる場合はその値を整数で返却すること()
        {
            Assert.Equal(-123323, MdlUtil.ParseLong("-123,323", MdlConst.LNG_NULL));
        }

        [Fact]
        public void ParseLong_引数がNULLの場合は定数LNG_NULLの値を返却すること()
        {
            Assert.Equal(MdlConst.LNG_NULL, MdlUtil.ParseLong(null, MdlConst.LNG_NULL));
        }

        [Fact]
        public void ParseLong_引数がから文字列の場合は定数LNG_NULLの値を返却すること()
        {
            Assert.Equal(MdlConst.LNG_NULL, MdlUtil.ParseLong("", MdlConst.LNG_NULL));
        }

        [Fact]
        public void ParseLong_引数が数字でない文字列の場合は定数LNG_NULLの値を返却すること()
        {
            Assert.Equal(MdlConst.LNG_NULL, MdlUtil.ParseLong("abc000", MdlConst.LNG_NULL));
        }

        // --------------------------------------------------------------------
        // ParseDouble()
        // --------------------------------------------------------------------
        [Fact]
        public void ParseDouble_引数が正の整数文字列の場合はその値をDOUBLで返却すること()
        {
            Assert.Equal(123.0, MdlUtil.ParseDouble("123", MdlConst.DBL_NULL));
        }

        [Fact]
        public void ParseDouble_引数がPLUS付正の整数文字列の場合はその値をDOUBLで返却すること()
        {
            Assert.Equal(123.0, MdlUtil.ParseDouble("+123", MdlConst.DBL_NULL));
        }

        [Fact]
        public void ParseDouble_引数が負の整数文字列の場合はその値をDOUBLで返却すること()
        {
            Assert.Equal(-123.0, MdlUtil.ParseDouble("-123", MdlConst.DBL_NULL));
        }

        [Fact]
        public void ParseDouble_引数が正の浮動小数文字列の場合はその値をDOUBLで返却すること()
        {
            Assert.Equal(123.93, MdlUtil.ParseDouble("123.93", MdlConst.DBL_NULL));
        }

        [Fact]
        public void ParseDouble_引数が負の浮動小数文字列の場合はその値をDOUBLで返却すること()
        {
            Assert.Equal(-123.93, MdlUtil.ParseDouble("-123.93", MdlConst.DBL_NULL));
        }

        [Fact]
        public void ParseDouble_引数の正の整数文字列にカンマが含まれる場合はその値をDOUBLで返却すること()
        {
            Assert.Equal(123323.0, MdlUtil.ParseDouble("123,323", MdlConst.DBL_NULL));
        }

        [Fact]
        public void ParseDouble_引数の負の整数文字列にカンマが含まれる場合はその値をDOUBLで返却すること()
        {
            Assert.Equal(-123323.0, MdlUtil.ParseDouble("-123,323", MdlConst.DBL_NULL));
        }

        [Fact]
        public void ParseDouble_引数がNULLの場合は定数DBL_NULLの値を返却すること()
        {
            Assert.Equal(MdlConst.DBL_NULL, MdlUtil.ParseDouble(null, MdlConst.DBL_NULL));
        }

        [Fact]
        public void ParseDouble_引数がから文字列の場合は定数DBL_NULLの値を返却すること()
        {
            Assert.Equal(MdlConst.DBL_NULL, MdlUtil.ParseDouble("", MdlConst.DBL_NULL));
        }

        [Fact]
        public void ParseDouble_引数が数字でない文字列の場合は定数DBL_NULLの値を返却すること()
        {
            Assert.Equal(MdlConst.DBL_NULL, MdlUtil.ParseDouble("abc000", MdlConst.DBL_NULL));
        }

        // --------------------------------------------------------------------
        // TrimQuotes()
        // --------------------------------------------------------------------
        [Fact]
        public void TrimQuotes_前後に複数の半角空白＿全角空白がある場合はそれを除去した文字列を返却すること()
        {
            string expected = "abcDef000##";
            Assert.Equal(expected, MdlUtil.TrimQuotes("  　　 " + expected + "  　　 "));
        }

        [Fact]
        public void TrimQuotes_ダブルクォートで囲まれた文字列の場合はそれを除去した文字列を返却すること()
        {
            string expected = "abcDef000##";
            Assert.Equal(expected, MdlUtil.TrimQuotes("　  \"  　{0}  　　 \"".Replace("{0}", expected)));
        }

        [Fact]
        public void TrimQuotes_シングルクォートで囲まれた文字列の場合はそれを除去した文字列を返却すること()
        {
            string expected = "abcDef000##";
            Assert.Equal(expected, MdlUtil.TrimQuotes("　  '  　　 " + expected + "  　　 '"));
        }

        // --------------------------------------------------------------------
        // ToBooleanStringOrNull()
        // --------------------------------------------------------------------
        [Fact]
        public void ToBooleanStringOrNull_引数が小文字の文字列falseの場合はnullを返却すること()
        {
            Assert.Null(MdlUtil.ToBooleanStringOrNull("false"));
        }

        [Fact]
        public void ToBooleanStringOrNull_引数が大文字の文字列FALSEの場合はnullを返却すること()
        {
            Assert.Null(MdlUtil.ToBooleanStringOrNull("FALSE"));
        }

        [Fact]
        public void ToBooleanStringOrNull_引数が文字列FALSEとfalse以外の場合は文字列trueを返却すること()
        {
            Assert.Equal("true", MdlUtil.ToBooleanStringOrNull("FALSE以外の文字列"));
        }

        // --------------------------------------------------------------------
        // GetRegexTarget()
        // --------------------------------------------------------------------
        [Fact]
        public void GetRegexTarget_指定した環境変数を取り出す正規表現パターンで文字列から文字列を抽出できること()
        {
            string target = "ENV.RUN_ENV";
            string prefix = @"ENV\.";
            string expected = "RUN_ENV";
            string pattern = @"^" + prefix + @"(?<TARGET>.+)$";
            string actual = MdlUtil.GetRegexTarget(target, pattern);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void GetRegexTarget_指定したAJSJOBNAMEから取り出す正規表現パターンで文字列から文字列を抽出できること()
        {
            string target = "AJSENV.ENV";
            string prefix = @"AJSENV\.";
            string expected = "ENV";
            string pattern = @"^" + prefix + @"(?<TARGET>[aA-zZ0-9_-]+)$";
            string actual = MdlUtil.GetRegexTarget(target, pattern);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
    }
}