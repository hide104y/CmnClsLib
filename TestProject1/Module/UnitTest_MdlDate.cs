using CmnClsLib.Module;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Module
{
    public class UnitTest_MdlDate
    {
        // --------------------------------------------------------------------
        // ConvertToDateString()
        // --------------------------------------------------------------------
        [Fact]
        public void GetDateEn_引数に2015が指定された場合は2015を返却すること()
        {
            string expected = @"2014";
            string actual = MdlDate.ConvertToDateString("2014");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDateEn_引数に201512が指定された場合は2015SLUSH12を返却すること()
        {
            string expected = @"2014/12";
            string actual = MdlDate.ConvertToDateString("201412");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDateEn_引数に20151227が指定された場合は2015SLUSH12SLUSH27を返却すること()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ConvertToDateString("20141227");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDateEn_引数に2015122719が指定された場合は2015SLUSH12SLUSH27SPACE19を返却すること()
        {
            string expected = @"2014/12/27 19";
            string actual = MdlDate.ConvertToDateString("2014122719");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDateEn_引数に201512271912が指定された場合は2015SLUSH12SLUSH27SPACE19COLON12を返却すること()
        {
            string expected = @"2014/12/27 19:12";
            string actual = MdlDate.ConvertToDateString("201412271912");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDateEn_引数に20151227191235が指定された場合は2015SLUSH12SLUSH27SPACE19COLON12COLON35を返却すること()
        {
            string expected = @"2014/12/27 19:12:35";
            string actual = MdlDate.ConvertToDateString("20141227191235");
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // GetUnixTimeString()
        // --------------------------------------------------------------------
        // irb(main):015:0> Time.local(2014, 12, 27, 0, 0, 0)
        // => 2014-12-27 00:00:00 +0900
        // irb(main):014:0> Time.local(2014, 12, 27, 0, 0, 0).to_i
        // => 1419606000
        // irb(main):015:0>
        // --------------------------------------------------------------------
        [Fact]
        public void StrUnixTime_引数にDATETIME型で20141227000000が指定された場合は文字列1419638400を返却すること()
        {
            string expected = @"1419606000";
            string actual = MdlDate.GetUnixTimeString(DateTime.Parse("2014/12/27 00:00:00"));
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // GetUnixTime()
        // --------------------------------------------------------------------
        [Fact]
        public void LongUnixTime_引数にDATETIME型で20141227000000が指定された場合は1419638400を返却すること()
        {
            long expected = 1419606000;
            long actual = MdlDate.GetUnixTime(DateTime.Parse("2014/12/27 00:00:00"));
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // ConvertUnixTimeToLocalTime()
        // --------------------------------------------------------------------
        [Fact]
        public void LongUnixTime_引数に文字列1419638400が指定された場合はDATETIME型で20141227000000を返却すること()
        {
            DateTime expected = DateTime.Parse("2014/12/27 00:00:00");
            DateTime actual = MdlDate.ConvertUnixTimeToLocalTime("1419606000");
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // GetFormattedDate()
        // --------------------------------------------------------------------
        [Fact]
        public void GetFormatedDate_第１引数にDATETIME型で20141227000000で第2引数に日付書式が指定された場合は2014SLUSH12SLUSH27を返却すること()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.GetFormattedDate(DateTime.Parse("2014/12/27 19:12:35"), "yyyy/MM/dd");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFormatedDate_第１引数にDATETIME型で20141227000000で第2引数に日時書式が指定された場合は2014SLUSH12SLUSH27SPACE19COLON12COLON35を返却すること()
        {
            string expected = @"2014/12/27 19:12:35";
            string actual = MdlDate.GetFormattedDate(DateTime.Parse("2014/12/27 19:12:35"), "yyyy/MM/dd HH:mm:ss");
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // ValidateAndFormatDate()
        // --------------------------------------------------------------------
        [Fact]
        public void GetValidateDate_第１引数にマイナスで日付が指定された場合は2014SLUSH12SLUSH27を返却すること()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ValidateAndFormatDate("2014-12-27");
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // ExtractDateAny(String strArg, Boolean blnIsCheckDate, int intCheckDate)
        // --------------------------------------------------------------------
        [Fact]
        public void ExtractDateAny_その１()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateAny(@"C:\path\88881199\[TEST] 20171310 20141227 20140227 20171227.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateAny_その２()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateAny(@"C:\path\88881199\[TEST] 2017-13-10 2014-12-27 2014-02-27 2017-12-27.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateAny_その３()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateAny(@"C:\path\88881199\[TEST] 2017/13/10 2014/12/27 2014/02/27 2017/12/27.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // ExtractDateStartsWith(String strArg, Boolean blnIsCheckDate, int intCheckDate)
        // --------------------------------------------------------------------
        [Fact]
        public void ExtractDateStartsWith_その１()
        {
            string expected = @"2017/12/10";
            string actual = MdlDate.ExtractDateStartsWith(@"20171210 20141227 20140227 20171227.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateStartsWith_その２()
        {
            string expected = @"2017/12/10";
            string actual = MdlDate.ExtractDateStartsWith(@"2017-12-10 2014-12-27 2014-02-27 2017-12-27.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateStartsWith_その３()
        {
            string expected = @"2017/12/10";
            string actual = MdlDate.ExtractDateStartsWith(@"2017/12/10 2014/12/27 2014/02/27 2017/12/27.txt", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // ExtractDateEndsWith(String strArg, Boolean blnIsCheckDate, int intCheckDate)
        // --------------------------------------------------------------------
        [Fact]
        public void ExtractDateEndsWith_その１()
        {
            string expected = @"2017/12/27";
            string actual = MdlDate.ExtractDateEndsWith(@"20171210 20141227 20140227 20171227", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateEndsWith_その２()
        {
            string expected = @"2017/12/27";
            string actual = MdlDate.ExtractDateEndsWith(@"2017-12-10 2014-12-27 2014-02-27 2017-12-27", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateEndsWith_その３()
        {
            string expected = @"2017/12/27";
            string actual = MdlDate.ExtractDateEndsWith(@"2017/12/10 2014/12/27 2014/02/27 2017/12/27", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // ExtractDateExact(String strArg, Boolean blnIsCheckDate, int intCheckDate)
        // --------------------------------------------------------------------
        [Fact]
        public void ExtractDateExact_その１()
        {
            string expected = @"2017/12/27";
            string actual = MdlDate.ExtractDateExact(@"20171227", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateExact_その２()
        {
            string expected = @"2017/12/27";
            string actual = MdlDate.ExtractDateExact(@"2017-12-27", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateExact_その３()
        {
            string expected = @"2017/12/27";
            string actual = MdlDate.ExtractDateExact(@"2017/12/27", MdlDate.PATTERN_YYYYMMDD, true, 19000001);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // ExtractDateContains(String strArg, Boolean blnIsCheckDate, int intCheckDate)
        // --------------------------------------------------------------------
        [Fact]
        public void ExtractDateContains_その００１()
        {
            string expected = @"2017/12/10";
            string actual = MdlDate.ExtractDateContains(@"AAA 20171210 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その００２()
        {
            string expected = @"2017/12/10";
            string actual = MdlDate.ExtractDateContains(@"AAA 20171210 20141210 20151210 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その００３()
        {
            string expected = @"2017/12/10";
            string actual = MdlDate.ExtractDateContains(@"AAA 2017-12-10 2014-12-10 2015-12-10 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その００４()
        {
            string expected = @"2017/12/10";
            string actual = MdlDate.ExtractDateContains(@"AAA 2017/12/10 2014/12/10 2015/12/10 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その０１１()
        {
            string expected = @"2015/12/10";
            string actual = MdlDate.ExtractDateContains(@"AAA 20171210 20141210 20121210 20151210 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 1, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その０１２()
        {
            string expected = @"2015/12/10";
            string actual = MdlDate.ExtractDateContains(@"AAA 2017-12-10 20141210 20121210 2015-12-10 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 1, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その０１３()
        {
            string expected = @"2015/12/10";
            string actual = MdlDate.ExtractDateContains(@"AAA 2017/12/10 20141210 20121210 2015/12/10 AAA", MdlDate.PATTERN_YYYYMMDD, "DD", true, 1, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その０２１()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateContains(@"C:\path\88881199\[TEST] 20171310 20141227 20140227 20171227.txt", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その０２２()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateContains(@"C:\path\88881199\[TEST] 2017-13-10 2014-12-27 2014-02-27 2017-12-27.txt", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その０２３()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateContains(@"C:\path\88881199\[TEST] 2017/13/10 2014/12/27 2014/02/27 2017/12/27.txt", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その０２４()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateContains(@"[TEST] 2017/09/101 2014/12/27 20140227 2017-12-27.txt", MdlDate.PATTERN_YYYYMMDD, "DD", true, 0, 19000001);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateContains_その０２５()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateContains(@"[TEST] 2017/09/101 20141227003300 20140227 2017-12-27.txt", MdlDate.PATTERN_YYYYMMDDHHMMSS, "SEC", true, 0, 19000001);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // ExtractDateFromPath()
        // --------------------------------------------------------------------
        [Fact]
        public void ExtractDateFromPath_第１引数にフルパスが指定された場合ファイル名から一番最初の日付文字列を抽出すること()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateFromPath(@"C:\path\20151201\[TEST] 20171310 20141227 20140227 20171227.txt");
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateFromPath_第１引数にフルパスが指定された場合ファイル名から一番最初の日付文字列を抽出することその２()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateFromPath(@"C:\path\20151201\[TEST] 201709101 20141227 20140227 20171227.txt");
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateFromPath_第１引数にフルパスが指定された場合ファイル名から一番最初の日付文字列を抽出することその３()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractDateFromPath(@"C:\path\20151201\[TEST] 2017-09-101 2014-12-27 2014-02-27 2017-12-27.txt");
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // ExtractAndFormatDateString()
        // --------------------------------------------------------------------
        [Fact]
        public void ExtractAndFormatDateString_第１引数に日付文字列を含む文字列が指定された場合は最初の日付文字列を抽出すること()
        {
            string expected = @"2015/12/01";
            string actual = MdlDate.ExtractAndFormatDateString(@"C:\path\20151201\[TEST] 20171310 20170327 20141227 20171227");
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractAndFormatDateString_第１引数に日付文字列と微妙に違う文字列が指定された場合は最初の日付文字列を抽出すること()
        {
            string expected = @"2014/12/27";
            string actual = MdlDate.ExtractAndFormatDateString(@"C:\path\20151301\[TEST] 20141227 20131227");
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // ExtractDateFromStringReverse()
        // --------------------------------------------------------------------
        [Fact]
        public void ExtractDateFromStringReverse_第１引数に日付文字列を含む文字列が指定された場合は最後の日付文字列を抽出すること()
        {
            string expected = @"2015/12/01";
            string actual = MdlDate.ExtractDateFromStringReverse(@"C:\path\20171201\20151201\[TEST] 20171310 20170357 20141327 201712279");
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateFromStringReverse_第１引数に日付文字列を含む文字列が指定された場合は最後の日付文字列を抽出することその２()
        {
            string expected = @"2015/12/01";
            string actual = MdlDate.ExtractDateFromStringReverse(@"C:\path\2017-12-01\2015-12-01\[TEST] 2017-13-10 2017-03-57 2014-13-27 201712279");
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ExtractDateFromStringReverse_第１引数に日付文字列を含む文字列が指定された場合は最後の日付文字列を抽出することその３()
        {
            string expected = @"2015/12/01";
            string actual = MdlDate.ExtractDateFromStringReverse(@"C:\path\2017/12/01\2015/12/01\[TEST] 2017/13/10 2017/03/57 2014/13/27 201712279");
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // CompareDatetime()
        // --------------------------------------------------------------------

        // --------------------------------------------------------------------
        // ConvertSecondsToTimeString()
        // --------------------------------------------------------------------
        [Fact]
        public void GetStrSecToTime_第１引数に秒数3662が指定された場合は01_01_02を返却すること()
        {
            string expected = @"01:01:02";
            string actual = MdlDate.ConvertSecondsToTimeString(3662);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void GetStrSecToTime_第１引数に秒数90000が指定された場合は25_00_00を返却すること()
        {
            string expected = @"25:00:00";
            string actual = MdlDate.ConvertSecondsToTimeString(90000);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void GetStrSecToTime_第１引数に秒数360000が指定された場合は100_00_00を返却すること()
        {
            string expected = @"100:00:00";
            string actual = MdlDate.ConvertSecondsToTimeString(360000);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void GetStrSecToTime_第１引数に秒数M3662が指定された場合はM01_01_02を返却すること()
        {
            string expected = @"-01:01:02";
            string actual = MdlDate.ConvertSecondsToTimeString(-3662);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
    }
}
