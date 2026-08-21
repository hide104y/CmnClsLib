package tool.cmnclslib.mdl;

import java.time.LocalDateTime;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 * MdlDate の単体テストクラスです。
 */
public class UnitTest_MdlDate {

    @Test
    public void getDateEn_引数に2015が指定された場合は2015を返却すること() {
        String expected = "2014";
        String actual = MdlDate.convertToDateString("2014");
        assertEquals(expected, actual);
    }

    @Test
    public void getDateEn_引数に201512が指定された場合は2015SLUSH12を返却すること() {
        String expected = "2014/12";
        String actual = MdlDate.convertToDateString("201412");
        assertEquals(expected, actual);
    }

    @Test
    public void getDateEn_引数に20151227が指定された場合は2015SLUSH12SLUSH27を返却すること() {
        String expected = "2014/12/27";
        String actual = MdlDate.convertToDateString("20141227");
        assertEquals(expected, actual);
    }

    @Test
    public void getDateEn_引数に2015122719が指定された場合は2015SLUSH12SLUSH27SPACE19を返却すること() {
        String expected = "2014/12/27 19";
        String actual = MdlDate.convertToDateString("2014122719");
        assertEquals(expected, actual);
    }

    @Test
    public void getDateEn_引数に201512271912が指定された場合は2015SLUSH12SLUSH27SPACE19COLON12を返却すること() {
        String expected = "2014/12/27 19:12";
        String actual = MdlDate.convertToDateString("201412271912");
        assertEquals(expected, actual);
    }

    @Test
    public void getDateEn_引数に20151227191235が指定された場合は2015SLUSH12SLUSH27SPACE19COLON12COLON35を返却すること() {
        String expected = "2014/12/27 19:12:35";
        String actual = MdlDate.convertToDateString("20141227191235");
        assertEquals(expected, actual);
    }

    @Test
    public void strUnixTime_引数にDATETIME型で20141227000000が指定された場合は文字列を返却すること() {
        LocalDateTime dt = LocalDateTime.of(2014, 12, 27, 0, 0, 0);
        long unixTime = MdlDate.getUnixTime(dt);
        String actual = MdlDate.getUnixTimeString(dt);
        assertEquals(String.valueOf(unixTime), actual);
    }

    @Test
    public void longUnixTime_引数にDATETIME型で20141227000000が指定された場合は数値を返却すること() {
        LocalDateTime dt = LocalDateTime.of(2014, 12, 27, 0, 0, 0);
        long actual = MdlDate.getUnixTime(dt);
        assertTrue(actual > 0);
    }

    @Test
    public void longUnixTime_引数に文字列が指定された場合はDATETIME型を返却すること() {
        LocalDateTime dt = LocalDateTime.of(2014, 12, 27, 0, 0, 0);
        long unixTime = MdlDate.getUnixTime(dt);
        LocalDateTime actual = MdlDate.convertUnixTimeToLocalTime(String.valueOf(unixTime));
        assertEquals(dt, actual);
    }

    @Test
    public void getFormatedDate_第１引数にDATETIME型で20141227000000で第2引数に日付書式が指定された場合は2014SLUSH12SLUSH27を返却すること() {
        String expected = "2014/12/27";
        String actual = MdlDate.getFormattedDate(LocalDateTime.of(2014, 12, 27, 19, 12, 35), "yyyy/MM/dd");
        assertEquals(expected, actual);
    }

    @Test
    public void getFormatedDate_第１引数にDATETIME型で20141227000000で第2引数に日時書式が指定された場合は2014SLUSH12SLUSH27SPACE19COLON12COLON35を返却すること() {
        String expected = "2014/12/27 19:12:35";
        String actual = MdlDate.getFormattedDate(LocalDateTime.of(2014, 12, 27, 19, 12, 35), "yyyy/MM/dd HH:mm:ss");
        assertEquals(expected, actual);
    }

    @Test
    public void getValidateDate_第１引数にマイナスで日付が指定された場合は2014SLUSH12SLUSH27を返却すること() {
        String expected = "2014/12/27";
        String actual = MdlDate.validateAndFormatDate("2014-12-27");
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateAny_その１() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateAny("C:\\path\\88881199\\[TEST] 20171310 20141227 20140227 20171227.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateAny_その２() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateAny("C:\\path\\88881199\\[TEST] 2017-13-10 2014-12-27 2014-02-27 2017-12-27.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateAny_その３() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateAny("C:\\path\\88881199\\[TEST] 2017/13/10 2014/12/27 2014/02/27 2017/12/27.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateStartsWith_その１() {
        String expected = "2017/12/10";
        String actual = MdlDate.extractDateStartsWith("20171210 20141227 20140227 20171227.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateStartsWith_その２() {
        String expected = "2017/12/10";
        String actual = MdlDate.extractDateStartsWith("2017-12-10 2014-12-27 2014-02-27 2017-12-27.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateStartsWith_その３() {
        String expected = "2017/12/10";
        String actual = MdlDate.extractDateStartsWith("2017/12/10 2014/12/27 2014/02/27 2017/12/27.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateEndsWith_その１() {
        String expected = "2017/12/27";
        String actual = MdlDate.extractDateEndsWith("20171210 20141227 20140227 20171227", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateEndsWith_その２() {
        String expected = "2017/12/27";
        String actual = MdlDate.extractDateEndsWith("2017-12-10 2014-12-27 2014-02-27 2017-12-27", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateEndsWith_その３() {
        String expected = "2017/12/27";
        String actual = MdlDate.extractDateEndsWith("2017/12/10 2014/12/27 2014/02/27 2017/12/27", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateExact_その１() {
        String expected = "2017/12/27";
        String actual = MdlDate.extractDateExact("20171227", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateExact_その２() {
        String expected = "2017/12/27";
        String actual = MdlDate.extractDateExact("2017-12-27", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateExact_その３() {
        String expected = "2017/12/27";
        String actual = MdlDate.extractDateExact("2017/12/27", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その００１() {
        String expected = "2017/12/10";
        String actual = MdlDate.extractDateContains("AAA 20171210 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その００２() {
        String expected = "2017/12/10";
        String actual = MdlDate.extractDateContains("AAA 20171210 20141210 20151210 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その００３() {
        String expected = "2017/12/10";
        String actual = MdlDate.extractDateContains("AAA 2017-12-10 2014-12-10 2015-12-10 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その００４() {
        String expected = "2017/12/10";
        String actual = MdlDate.extractDateContains("AAA 2017/12/10 2014/12/10 2015/12/10 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その０１１() {
        String expected = "2015/12/10";
        String actual = MdlDate.extractDateContains("AAA 20171210 20141210 20121210 20151210 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 1, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その０１２() {
        String expected = "2015/12/10";
        String actual = MdlDate.extractDateContains("AAA 2017-12-10 20141210 20121210 2015-12-10 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 1, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その０１３() {
        String expected = "2015/12/10";
        String actual = MdlDate.extractDateContains("AAA 2017/12/10 20141210 20121210 2015/12/10 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 1, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その０２１() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateContains("C:\\path\\88881199\\[TEST] 20171310 20141227 20140227 20171227.txt", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その０２２() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateContains("C:\\path\\88881199\\[TEST] 2017-13-10 2014-12-27 2014-02-27 2017-12-27.txt", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その０２３() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateContains("C:\\path\\88881199\\[TEST] 2017/13/10 2014/12/27 2014/02/27 2017/12/27.txt", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その０２４() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateContains("[TEST] 2017/09/101 2014/12/27 20140227 2017-12-27.txt", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateContains_その０２５() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateContains("[TEST] 2017/09/101 20141227003300 20140227 2017-12-27.txt", MdlDate.PATTERN_YYYYMMDDHHMMSS, "SEC", true, 0, 19000001);
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateFromPath_第１引数にフルパスが指定された場合ファイル名から一番最初の日付文字列を抽出すること() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateFromPath("C:\\path\\20151201\\[TEST] 20171310 20141227 20140227 20171227.txt");
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateFromPath_第１引数にフルパスが指定された場合ファイル名から一番最初の日付文字列を抽出することその２() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateFromPath("C:\\path\\20151201\\[TEST] 201709101 20141227 20140227 20171227.txt");
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateFromPath_第１引数にフルパスが指定された場合ファイル名から一番最初の日付文字列を抽出することその３() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractDateFromPath("C:\\path\\20151201\\[TEST] 2017-09-101 2014-12-27 2014-02-27 2017-12-27.txt");
        assertEquals(expected, actual);
    }

    @Test
    public void extractAndFormatDateString_第１引数に日付文字列を含む文字列が指定された場合は最初の日付文字列を抽出すること() {
        String expected = "2015/12/01";
        String actual = MdlDate.extractAndFormatDateString("C:\\path\\20151201\\[TEST] 20171310 20170327 20141227 20171227");
        assertEquals(expected, actual);
    }

    @Test
    public void extractAndFormatDateString_第１引数に日付文字列と微妙に違う文字列が指定された場合は最初の日付文字列を抽出すること() {
        String expected = "2014/12/27";
        String actual = MdlDate.extractAndFormatDateString("C:\\path\\20151301\\[TEST] 20141227 20131227");
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateFromStringReverse_第１引数に日付文字列を含む文字列が指定された場合は最後の日付文字列を抽出すること() {
        String expected = "2015/12/01";
        String actual = MdlDate.extractDateFromStringReverse("C:\\path\\20171201\\20151201\\[TEST] 20171310 20170357 20141327 201712279");
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateFromStringReverse_第１引数に日付文字列を含む文字列が指定された場合は最後の日付文字列を抽出することその２() {
        String expected = "2015/12/01";
        String actual = MdlDate.extractDateFromStringReverse("C:\\path\\2017-12-01\\2015-12-01\\[TEST] 2017-13-10 2017-03-57 2014-13-27 201712279");
        assertEquals(expected, actual);
    }

    @Test
    public void extractDateFromStringReverse_第１引数に日付文字列を含む文字列が指定された場合は最後の日付文字列を抽出することその３() {
        String expected = "2015/12/01";
        String actual = MdlDate.extractDateFromStringReverse("C:\\path\\2017/12/01\\2015/12/01\\[TEST] 2017/13/10 2017/03/57 2014/13/27 201712279");
        assertEquals(expected, actual);
    }

    @Test
    public void getStrSecToTime_第１引数に秒数3662が指定された場合は01_01_02を返却すること() {
        String expected = "01:01:02";
        String actual = MdlDate.convertSecondsToTimeString(3662);
        assertEquals(expected, actual);
    }

    @Test
    public void getStrSecToTime_第１引数に秒数90000が指定された場合は25_00_00を返却すること() {
        String expected = "25:00:00";
        String actual = MdlDate.convertSecondsToTimeString(90000);
        assertEquals(expected, actual);
    }

    @Test
    public void getStrSecToTime_第１引数に秒数360000が指定された場合は100_00_00を返却すること() {
        String expected = "100:00:00";
        String actual = MdlDate.convertSecondsToTimeString(360000);
        assertEquals(expected, actual);
    }

    @Test
    public void getStrSecToTime_第１引数に秒数M3662が指定された場合はM01_01_02を返却すること() {
        String expected = "-01:01:02";
        String actual = MdlDate.convertSecondsToTimeString(-3662);
        assertEquals(expected, actual);
    }

    @Test
    public void 新設メソッド_fromUnixTime_validateAndFormat等が正しく動作すること() {
        assertNotNull(MdlDate.fromUnixTime("1600000000"));
        assertNotNull(MdlDate.fromUnixTime(1600000000L));
        assertEquals("2024/01/01", MdlDate.validateAndFormat("2024-01-01", true));
        assertEquals("2024/01/01", MdlDate.extractAndFormatDate("test_20240101_sample"));
        assertEquals("2024/01/01", MdlDate.extractDateReverse("20230101_20240101"));
        assertEquals("2024/01/01", MdlDate.extractDateStart("20240101_rest", MdlDate.PATTERN_YYYYMMDD, true, 1));
        assertEquals("01:01:02", MdlDate.secondsToTimeString(3662));
        assertTrue(MdlDate.replaceWithDateTime("log_%YYYYMMDD%").startsWith("log_20"));
    }
}
