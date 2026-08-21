package tool.cmnclslib.mdl;

import org.junit.Test;
import static org.junit.Assert.*;

/**
 * MdlUtil の単体テストクラスです。
 */
public class UnitTest_MdlUtil {

    @Test
    public void isNumeric_引数が正の整数文字列の場合はTRUEを返却すること() {
        assertTrue(MdlUtil.isNumeric("123"));
    }

    @Test
    public void isNumeric_引数がPLUS整数文字列の場合はTRUEを返却すること() {
        assertTrue(MdlUtil.isNumeric("+123"));
    }

    @Test
    public void isNumeric_引数が負の整数文字列の場合はTRUEを返却すること() {
        assertTrue(MdlUtil.isNumeric("-123"));
    }

    @Test
    public void isNumeric_引数が正の浮動小数文字列の場合はTRUEを返却すること() {
        assertTrue(MdlUtil.isNumeric("123.23"));
    }

    @Test
    public void isNumeric_引数が負の浮動小数文字列の場合はTRUEを返却すること() {
        assertTrue(MdlUtil.isNumeric("-123.23"));
    }

    @Test
    public void isNumeric_引数の正の整数文字列にカンマが含まれる場合はTRUEを返却すること() {
        assertTrue(MdlUtil.isNumeric("123,323"));
    }

    @Test
    public void isNumeric_引数の負の整数文字列にカンマが含まれる場合はTRUEを返却すること() {
        assertTrue(MdlUtil.isNumeric("-123,323"));
    }

    @Test
    public void isNumeric_引数がNULLの場合FLAS返却すること() {
        assertFalse(MdlUtil.isNumeric(null));
    }

    @Test
    public void isNumeric_引数がから文字列の場合FLAS返却すること() {
        assertFalse(MdlUtil.isNumeric(""));
    }

    @Test
    public void isNumeric_引数が数字でない文字列の場合はFALSEを返却すること() {
        assertFalse(MdlUtil.isNumeric("abc000"));
    }

    @Test
    public void isNumeric_引数がオブジェクトの場合はFALSEを返却すること() {
        Object objTmp = new Object();
        assertFalse(MdlUtil.isNumeric(objTmp));
    }

    @Test
    public void parseInt_引数が正の整数文字列の場合はその値を整数で返却すること() {
        int expected = 123;
        assertEquals(expected, MdlUtil.parseInt(String.valueOf(expected), MdlConst.INT_NULL));
    }

    @Test
    public void parseInt_引数がPLUS付正の整数文字列の場合はその値を整数で返却すること() {
        int expected = 123;
        assertEquals(expected, MdlUtil.parseInt("+" + expected, MdlConst.INT_NULL));
    }

    @Test
    public void parseInt_引数が負の整数文字列の場合はその値を整数で返却すること() {
        int expected = -123;
        assertEquals(expected, MdlUtil.parseInt(String.valueOf(expected), MdlConst.INT_NULL));
    }

    @Test
    public void parseInt_引数が正の浮動小数文字列の場合はその値の小数点以下切り捨ての整数で返却すること() {
        assertEquals(123, MdlUtil.parseInt("123.93", MdlConst.INT_NULL));
    }

    @Test
    public void parseInt_引数が負の浮動小数文字列の場合はその値を整数で返却すること() {
        assertEquals(-123, MdlUtil.parseInt("-123.93", MdlConst.INT_NULL));
    }

    @Test
    public void parseInt_引数の正の整数文字列にカンマが含まれる場合はその値の小数点以下切り捨ての整数で返却すること() {
        assertEquals(123323, MdlUtil.parseInt("123,323", MdlConst.INT_NULL));
    }

    @Test
    public void parseInt_引数の負の整数文字列にカンマが含まれる場合はその値を整数で返却すること() {
        assertEquals(-123323, MdlUtil.parseInt("-123,323", MdlConst.INT_NULL));
    }

    @Test
    public void parseInt_引数がNULLの場合は定数INT_NULLの値を返却すること() {
        assertEquals(MdlConst.INT_NULL, MdlUtil.parseInt(null, MdlConst.INT_NULL));
    }

    @Test
    public void parseInt_引数がから文字列の場合は定数INT_NULLの値を返却すること() {
        assertEquals(MdlConst.INT_NULL, MdlUtil.parseInt("", MdlConst.INT_NULL));
    }

    @Test
    public void parseInt_引数が数字でない文字列の場合は定数INT_NULLの値を返却すること() {
        assertEquals(MdlConst.INT_NULL, MdlUtil.parseInt("abc000", MdlConst.INT_NULL));
    }

    @Test
    public void parseLong_引数が正の整数文字列の場合はその値を整数で返却すること() {
        int expected = 123;
        assertEquals((long) expected, MdlUtil.parseLong(String.valueOf(expected), MdlConst.LNG_NULL));
    }

    @Test
    public void parseLong_引数がPLUS付正の整数文字列の場合はその値を整数で返却すること() {
        int expected = 123;
        assertEquals((long) expected, MdlUtil.parseLong("+" + expected, MdlConst.LNG_NULL));
    }

    @Test
    public void parseLong_引数が負の整数文字列の場合はその値を整数で返却すること() {
        int expected = -123;
        assertEquals((long) expected, MdlUtil.parseLong(String.valueOf(expected), MdlConst.LNG_NULL));
    }

    @Test
    public void parseLong_引数が正の浮動小数文字列の場合はその値の小数点以下切り捨ての整数で返却すること() {
        assertEquals(123, MdlUtil.parseLong("123.93", MdlConst.LNG_NULL));
    }

    @Test
    public void parseLong_引数が負の浮動小数文字列の場合はその値を整数で返却すること() {
        assertEquals(-123, MdlUtil.parseLong("-123.93", MdlConst.LNG_NULL));
    }

    @Test
    public void parseLong_引数の正の整数文字列にカンマが含まれる場合はその値の小数点以下切り捨ての整数で返却すること() {
        assertEquals(123323, MdlUtil.parseLong("123,323", MdlConst.LNG_NULL));
    }

    @Test
    public void parseLong_引数の負の整数文字列にカンマが含まれる場合はその値を整数で返却すること() {
        assertEquals(-123323, MdlUtil.parseLong("-123,323", MdlConst.LNG_NULL));
    }

    @Test
    public void parseLong_引数がNULLの場合は定数LNG_NULLの値を返却すること() {
        assertEquals(MdlConst.LNG_NULL, MdlUtil.parseLong(null, MdlConst.LNG_NULL));
    }

    @Test
    public void parseLong_引数がから文字列の場合は定数LNG_NULLの値を返却すること() {
        assertEquals(MdlConst.LNG_NULL, MdlUtil.parseLong("", MdlConst.LNG_NULL));
    }

    @Test
    public void parseLong_引数が数字でない文字列の場合は定数LNG_NULLの値を返却すること() {
        assertEquals(MdlConst.LNG_NULL, MdlUtil.parseLong("abc000", MdlConst.LNG_NULL));
    }

    @Test
    public void parseDouble_引数が正の整数文字列の場合はその値をDOUBLで返却すること() {
        assertEquals(123.0, MdlUtil.parseDouble("123", MdlConst.DBL_NULL), 0.0001);
    }

    @Test
    public void parseDouble_引数がPLUS付正の整数文字列の場合はその値をDOUBLで返却すること() {
        assertEquals(123.0, MdlUtil.parseDouble("+123", MdlConst.DBL_NULL), 0.0001);
    }

    @Test
    public void parseDouble_引数が負の整数文字列の場合はその値をDOUBLで返却すること() {
        assertEquals(-123.0, MdlUtil.parseDouble("-123", MdlConst.DBL_NULL), 0.0001);
    }

    @Test
    public void parseDouble_引数が正の浮動小数文字列の場合はその値をDOUBLで返却すること() {
        assertEquals(123.93, MdlUtil.parseDouble("123.93", MdlConst.DBL_NULL), 0.0001);
    }

    @Test
    public void parseDouble_引数が負の浮動小数文字列の場合はその値をDOUBLで返却すること() {
        assertEquals(-123.93, MdlUtil.parseDouble("-123.93", MdlConst.DBL_NULL), 0.0001);
    }

    @Test
    public void parseDouble_引数の正の整数文字列にカンマが含まれる場合はその値をDOUBLで返却すること() {
        assertEquals(123323.0, MdlUtil.parseDouble("123,323", MdlConst.DBL_NULL), 0.0001);
    }

    @Test
    public void parseDouble_引数の負の整数文字列にカンマが含まれる場合はその値をDOUBLで返却すること() {
        assertEquals(-123323.0, MdlUtil.parseDouble("-123,323", MdlConst.DBL_NULL), 0.0001);
    }

    @Test
    public void parseDouble_引数がNULLの場合は定数DBL_NULLの値を返却すること() {
        assertEquals(MdlConst.DBL_NULL, MdlUtil.parseDouble(null, MdlConst.DBL_NULL), 0.0001);
    }

    @Test
    public void parseDouble_引数がから文字列の場合は定数DBL_NULLの値を返却すること() {
        assertEquals(MdlConst.DBL_NULL, MdlUtil.parseDouble("", MdlConst.DBL_NULL), 0.0001);
    }

    @Test
    public void parseDouble_引数が数字でない文字列の場合は定数DBL_NULLの値を返却すること() {
        assertEquals(MdlConst.DBL_NULL, MdlUtil.parseDouble("abc000", MdlConst.DBL_NULL), 0.0001);
    }

    @Test
    public void trimQuotes_前後に複数の半角空白＿全角空白がある場合はそれを除去した文字列を返却すること() {
        String expected = "abcDef000##";
        assertEquals(expected, MdlUtil.trimQuotes("  　　 " + expected + "  　　 "));
    }

    @Test
    public void trimQuotes_ダブルクォートで囲まれた文字列の場合はそれを除去した文字列を返却すること() {
        String expected = "abcDef000##";
        assertEquals(expected, MdlUtil.trimQuotes("　  \"  　" + expected + "  　　 \""));
    }

    @Test
    public void trimQuotes_シングルクォートで囲まれた文字列の場合はそれを除去した文字列を返却すること() {
        String expected = "abcDef000##";
        assertEquals(expected, MdlUtil.trimQuotes("　  '  　　 " + expected + "  　　 '"));
    }

    @Test
    public void toBooleanStringOrNull_引数が小文字の文字列falseの場合はnullを返却すること() {
        assertNull(MdlUtil.toBooleanStringOrNull("false"));
    }

    @Test
    public void toBooleanStringOrNull_引数が大文字の文字列FALSEの場合はnullを返却すること() {
        assertNull(MdlUtil.toBooleanStringOrNull("FALSE"));
    }

    @Test
    public void toBooleanStringOrNull_引数が文字列FALSEとfalse以外の場合は文字列trueを返却すること() {
        assertEquals("true", MdlUtil.toBooleanStringOrNull("FALSE以外の文字列"));
    }

    @Test
    public void getRegexTarget_指定した環境変数を取り出す正規表現パターンで文字列から文字列を抽出できること() {
        String target = "ENV.RUN_ENV";
        String prefix = "ENV\\.";
        String expected = "RUN_ENV";
        String pattern = "^" + prefix + "(?<TARGET>.+)$";
        String actual = MdlUtil.getRegexTarget(target, pattern);
        assertEquals(expected, actual);
    }

    @Test
    public void getRegexTarget_指定したAJSJOBNAMEから取り出す正規表現パターンで文字列から文字列を抽出できること() {
        String target = "AJSENV.ENV";
        String prefix = "AJSENV\\.";
        String expected = "ENV";
        String pattern = "^" + prefix + "(?<TARGET>[a-zA-Z0-9_-]+)$";
        String actual = MdlUtil.getRegexTarget(target, pattern);
        assertEquals(expected, actual);
    }

    @Test
    public void 新設メソッド_toBoolStringOrNull_formatByteSize_parseCsvToInts_parseCsvToMapが動作すること() {
        assertNull(MdlUtil.toBoolStringOrNull("false"));
        assertEquals("true", MdlUtil.toBoolStringOrNull("true"));
        assertNotNull(MdlUtil.formatByteSize(1024L, 2, "0,000.00"));
        assertNotNull(MdlUtil.formatByteSizeRight(1024L));
        assertEquals(3, MdlUtil.parseCsvToInts(null, "1,2,3").size());

        java.util.Map<String, String> map = new java.util.LinkedHashMap<>();
        MdlUtil.parseCsvToMap(map, "a=1,b=2", "[,|]", "=", 0, false, false);
        assertEquals("1", map.get("a"));
        assertEquals("2", map.get("b"));
    }
}
