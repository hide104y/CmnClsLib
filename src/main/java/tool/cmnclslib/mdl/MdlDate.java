package tool.cmnclslib.mdl;

import java.io.File;
import java.lang.management.ManagementFactory;
import java.time.Instant;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.time.chrono.JapaneseChronology;
import java.time.chrono.JapaneseDate;
import java.time.format.DateTimeFormatter;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * 日時・日付文字列の検証、フォーマット変換、UNIX時間変換、正規表現抽出などの日付関連ユーティリティ機能を提供するクラスです。
 */
public final class MdlDate {

    /**
     * 年月日（YYYY/MM/DD）判定用の正規表現パターン文字列です。
     */
    public static final String PATTERN_YYYYMMDD = "(?<YYYY>[0-9]{4})[-/]*(?<MM>[0-1][0-9])[-/]*(?<DD>[0-3][0-9])";

    /**
     * 年月日時分秒（YYYYMMDDHHMMSS）判定用の正規表現パターン文字列です。
     */
    public static final String PATTERN_YYYYMMDDHHMMSS = "(?<YYYY>[0-9]{4})(?<MM>[0-1][0-9])(?<DD>[0-3][0-9])(?<HH>[0-2][0-9])(?<MIN>[0-5][0-9])(?<SEC>[0-5][0-9])";

    private static final Pattern NON_DIGIT_REGEX = Pattern.compile("\\D");
    private static final Pattern WHITESPACE_REGEX = Pattern.compile("\\s+");
    private static final Pattern YYYY_MM_DD_REGEX = Pattern.compile(PATTERN_YYYYMMDD);
    private static final Pattern YYYY_MM_DD_HH_MM_SS_REGEX = Pattern.compile(PATTERN_YYYYMMDDHHMMSS);

    private MdlDate() {
        // インスタンス化防止
    }

    /**
     * 指定された文字列が有効な日付形式であるか、および指定した閾値（YYYYMMDD形式の整数）を超えているかを検証します。
     *
     * @param dateString 検証対象の日付文字列
     * @param checkDate 日付の閾値（例: 20200101）。0 を指定した場合は閾値チェックを行いません
     * @return 日付として有効であり、かつ閾値を超えている場合は true、それ以外は false
     */
    public static boolean isValidDate(String dateString, int checkDate) {
        if (!isValidDate(dateString)) {
            return false;
        }
        if (checkDate == 0) {
            return true;
        }

        String digitsOnly = NON_DIGIT_REGEX.matcher(dateString).replaceAll("");
        if (digitsOnly.length() >= 8) {
            int dateVal = MdlUtil.parseInt(digitsOnly.substring(0, 8), 0);
            return dateVal > checkDate;
        }
        return false;
    }

    /**
     * 指定された文字列が有効な日付形式かを検証します。
     *
     * @param dateString 検証対象の日付文字列
     * @return 有効な日付の場合は true、それ以外は false
     */
    public static boolean isValidDate(String dateString) {
        if (dateString == null || dateString.trim().isEmpty()) {
            return false;
        }

        String trimmed = dateString.trim().replace('-', '/');
        String[] parts = trimmed.split("\\s+");
        String datePart = parts[0];

        String[] dParts = datePart.split("/");
        if (dParts.length == 3) {
            int year = MdlUtil.parseInt(dParts[0], -1);
            int month = MdlUtil.parseInt(dParts[1], -1);
            int day = MdlUtil.parseInt(dParts[2], -1);
            if (year < 1 || month < 1 || month > 12 || day < 1 || day > 31) {
                return false;
            }
            try {
                LocalDate date = LocalDate.of(year, month, day);
                if (parts.length > 1) {
                    String timePart = parts[1];
                    String[] tParts = timePart.split(":");
                    int hour = (tParts.length > 0) ? MdlUtil.parseInt(tParts[0], -1) : 0;
                    int minute = (tParts.length > 1) ? MdlUtil.parseInt(tParts[1], -1) : 0;
                    int second = (tParts.length > 2) ? MdlUtil.parseInt(tParts[2], -1) : 0;
                    if (hour < 0 || hour > 23 || minute < 0 || minute > 59 || second < 0 || second > 59) {
                        return false;
                    }
                }
                return true;
            } catch (Exception e) {
                return false;
            }
        }

        String digitsOnly = NON_DIGIT_REGEX.matcher(trimmed).replaceAll("");
        if (digitsOnly.length() == 8 || digitsOnly.length() == 14) {
            int year = MdlUtil.parseInt(digitsOnly.substring(0, 4), -1);
            int month = MdlUtil.parseInt(digitsOnly.substring(4, 6), -1);
            int day = MdlUtil.parseInt(digitsOnly.substring(6, 8), -1);
            if (year < 1 || month < 1 || month > 12 || day < 1 || day > 31) {
                return false;
            }
            try {
                LocalDate.of(year, month, day);
                if (digitsOnly.length() == 14) {
                    int hour = MdlUtil.parseInt(digitsOnly.substring(8, 10), -1);
                    int minute = MdlUtil.parseInt(digitsOnly.substring(10, 12), -1);
                    int second = MdlUtil.parseInt(digitsOnly.substring(12, 14), -1);
                    if (hour < 0 || hour > 23 || minute < 0 || minute > 59 || second < 0 || second > 59) {
                        return false;
                    }
                }
                return true;
            } catch (Exception e) {
                return false;
            }
        }

        return false;
    }

    /**
     * YYYYMMDDHHMMSS 形式などの連続した数字列を、区切り文字付きの日時文字列（YYYY/MM/DD HH:mm:ss）に整形します。
     *
     * @param dateTimeString 変換する数字列（例: "20241110123045"）
     * @return 区切り文字（/ や :）が挿入された日時文字列
     */
    public static String convertToDateString(String dateTimeString) {
        if (dateTimeString == null || dateTimeString.isEmpty()) {
            return "";
        }

        StringBuilder result = new StringBuilder();
        if (dateTimeString.length() >= 4) {
            result.append(dateTimeString, 0, 4);
        }
        if (dateTimeString.length() >= 6) {
            result.append('/').append(dateTimeString, 4, 6);
        }
        if (dateTimeString.length() >= 8) {
            result.append('/').append(dateTimeString, 6, 8);
        }
        if (dateTimeString.length() >= 10) {
            result.append(' ').append(dateTimeString, 8, 10);
        }
        if (dateTimeString.length() >= 12) {
            result.append(':').append(dateTimeString, 10, 12);
        }
        if (dateTimeString.length() >= 14) {
            result.append(':').append(dateTimeString, 12, 14);
        }

        return result.toString();
    }

    /**
     * 現在のシステム日時を UNIX 時間（1970年1月1日からの通算秒数）の文字列形式で取得します。
     *
     * @return 現在の UNIX 時間の文字列表現
     */
    public static String getUnixTimeString() {
        return Long.toString(getUnixTime(LocalDateTime.now()));
    }

    /**
     * 指定された日時を UNIX 時間（1970年1月1日からの通算秒数）の文字列形式で取得します。
     *
     * @param targetTime 対象の LocalDateTime
     * @return 指定した日時の UNIX 時間の文字列表現
     */
    public static String getUnixTimeString(LocalDateTime targetTime) {
        return Long.toString(getUnixTime(targetTime));
    }

    /**
     * 現在のシステム日時を UNIX 時間（1970年1月1日からの通算秒数）で取得します。
     *
     * @return 現在の UNIX 時間（秒）
     */
    public static long getUnixTime() {
        return getUnixTime(LocalDateTime.now());
    }

    /**
     * 指定された日時を UNIX 時間（1970年1月1日からの通算秒数）で取得します。
     *
     * @param targetTime 対象の LocalDateTime
     * @return 指定した日時の UNIX 時間（秒）
     */
    public static long getUnixTime(LocalDateTime targetTime) {
        if (targetTime == null) {
            return 0L;
        }
        return targetTime.atZone(ZoneId.systemDefault()).toEpochSecond();
    }

    /**
     * UNIX 時間の文字列をローカル日時に変換します。
     *
     * @param unixTimeString UNIX 時間を表す文字列（秒）
     * @return ローカルタイムゾーンの LocalDateTime
     */
    public static LocalDateTime fromUnixTime(String unixTimeString) {
        return fromUnixTime(MdlUtil.parseLong(unixTimeString, 0L));
    }

    /**
     * @deprecated {@link #fromUnixTime(String)} を使用してください。
     */
    @Deprecated
    public static LocalDateTime convertUnixTimeToLocalTime(String unixTimeString) {
        return fromUnixTime(unixTimeString);
    }

    /**
     * UNIX 時間をローカル日時に変換します。
     *
     * @param unixTime UNIX 時間（秒）
     * @return ローカルタイムゾーンの LocalDateTime
     */
    public static LocalDateTime fromUnixTime(long unixTime) {
        return LocalDateTime.ofInstant(Instant.ofEpochSecond(unixTime), ZoneId.systemDefault());
    }

    /**
     * @deprecated {@link #fromUnixTime(long)} を使用してください。
     */
    @Deprecated
    public static LocalDateTime convertUnixTimeToLocalTime(long unixTime) {
        return fromUnixTime(unixTime);
    }

    /**
     * 現在のシステム日時を指定したフォーマット文字列で整形して取得します。
     *
     * @param format 日付フォーマット文字列（例: "yyyy/MM/dd"）
     * @return フォーマットされた日付文字列
     */
    public static String getFormattedDate(String format) {
        return getFormattedDate(LocalDateTime.now(), format, false);
    }

    /**
     * 指定した日時を指定したフォーマット文字列で整形して取得します。
     *
     * @param date 対象の LocalDateTime
     * @param format 日付フォーマット文字列（例: "yyyy/MM/dd"）
     * @return フォーマットされた日付文字列
     */
    public static String getFormattedDate(LocalDateTime date, String format) {
        return getFormattedDate(date, format, false);
    }

    /**
     * 指定した日時を指定したフォーマット文字列および和暦オプションを使用して整形取得します。
     *
     * @param date 対象の LocalDateTime
     * @param format 日付フォーマット文字列
     * @param isCulture true の場合は ja-JP カルチャと和暦（JapaneseChronology）を使用して整形します
     * @return フォーマットされた日付文字列
     */
    public static String getFormattedDate(LocalDateTime date, String format, boolean isCulture) {
        if (date == null || format == null) {
            return "";
        }
        if (isCulture) {
            try {
                JapaneseDate jDate = JapaneseDate.from(date);
                DateTimeFormatter formatter = DateTimeFormatter.ofPattern(format, Locale.JAPAN)
                        .withChronology(JapaneseChronology.INSTANCE);
                return formatter.format(jDate);
            } catch (Exception e) {
                // 和暦変換失敗時は西暦フォーマットにフォールバック
            }
        }
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern(format, Locale.ROOT);
        return date.format(formatter);
    }

    /**
     * 数字列（8桁・9桁・10桁・12桁・14桁）を解釈し、LocalDateTime に変換を試みます。
     *
     * @param dateTimeString 変換対象の数字列文字列
     * @return 変換された LocalDateTime（失敗時は null）
     */
    public static LocalDateTime parseDateTime(String dateTimeString) {
        if (dateTimeString == null || dateTimeString.trim().isEmpty()) {
            return null;
        }

        String digitsOnly = NON_DIGIT_REGEX.matcher(dateTimeString).replaceAll("");
        if (!MdlUtil.isNumeric(digitsOnly)) {
            return null;
        }

        String formattedString;
        switch (digitsOnly.length()) {
            case 8:
                formattedString = digitsOnly.substring(0, 4) + "/" + digitsOnly.substring(4, 6) + "/" + digitsOnly.substring(6, 8);
                break;
            case 9:
                formattedString = digitsOnly.substring(0, 4) + "/" + digitsOnly.substring(4, 6) + "/" + digitsOnly.substring(6, 8)
                        + " 0" + digitsOnly.substring(8, 9) + ":00:00";
                break;
            case 10:
                formattedString = digitsOnly.substring(0, 4) + "/" + digitsOnly.substring(4, 6) + "/" + digitsOnly.substring(6, 8)
                        + " " + digitsOnly.substring(8, 10) + ":00:00";
                break;
            case 12:
                formattedString = digitsOnly.substring(0, 4) + "/" + digitsOnly.substring(4, 6) + "/" + digitsOnly.substring(6, 8)
                        + " " + digitsOnly.substring(8, 10) + ":" + digitsOnly.substring(10, 12) + ":00";
                break;
            case 14:
                formattedString = digitsOnly.substring(0, 4) + "/" + digitsOnly.substring(4, 6) + "/" + digitsOnly.substring(6, 8)
                        + " " + digitsOnly.substring(8, 10) + ":" + digitsOnly.substring(10, 12) + ":" + digitsOnly.substring(12, 14);
                break;
            default:
                return null;
        }

        if (isValidDate(formattedString)) {
            try {
                String[] parts = formattedString.split("\\s+");
                String[] dParts = parts[0].split("/");
                int y = Integer.parseInt(dParts[0]);
                int m = Integer.parseInt(dParts[1]);
                int d = Integer.parseInt(dParts[2]);
                int h = 0;
                int min = 0;
                int s = 0;
                if (parts.length > 1) {
                    String[] tParts = parts[1].split(":");
                    h = Integer.parseInt(tParts[0]);
                    min = Integer.parseInt(tParts[1]);
                    s = Integer.parseInt(tParts[2]);
                }
                return LocalDateTime.of(y, m, d, h, min, s);
            } catch (Exception e) {
                return null;
            }
        }
        return null;
    }

    /**
     * 不定形式の日付文字列を解析し、標準的な形式（YYYY/MM/DD または YYYY/MM/DD HH:mm:ss）に変換・検証します。
     *
     * @param dateString 入力日付文字列
     * @param includeTime 時刻情報（HH:mm:ss）の補正・出力を含めるかどうか
     * @return 正規化された日付文字列。無効な日付の場合は空文字列
     */
    public static String validateAndFormat(String dateString, boolean includeTime) {
        if (dateString == null || dateString.trim().isEmpty()) {
            return "";
        }

        String formattedDate = "";
        String[] dateTimeParts = WHITESPACE_REGEX.split(dateString.trim());
        String tempDate = dateTimeParts[0].replace("-", "/").replace(" ", "");

        if (!MdlUtil.isNumeric(tempDate.replace("/", ""))) {
            return formattedDate;
        }

        String[] dateParts = tempDate.split("/");
        switch (dateParts.length) {
            case 3:
                int y = MdlUtil.parseInt(dateParts[0], 0);
                int m = MdlUtil.parseInt(dateParts[1], 0);
                int d = MdlUtil.parseInt(dateParts[2], 0);
                formattedDate = String.format(Locale.ROOT, "%04d/%02d/%02d", y, m, d);
                break;
            case 1:
                switch (tempDate.length()) {
                    case 8:
                        formattedDate = tempDate.substring(0, 4) + "/" + tempDate.substring(4, 6) + "/" + tempDate.substring(6, 8);
                        break;
                    case 10:
                        if (includeTime) {
                            formattedDate = tempDate.substring(0, 4) + "/" + tempDate.substring(4, 6) + "/" + tempDate.substring(6, 8)
                                    + " " + tempDate.substring(8, 10) + ":00:00";
                        }
                        break;
                    case 12:
                        if (includeTime) {
                            formattedDate = tempDate.substring(0, 4) + "/" + tempDate.substring(4, 6) + "/" + tempDate.substring(6, 8)
                                    + " " + tempDate.substring(8, 10) + ":" + tempDate.substring(10, 12) + ":00";
                        }
                        break;
                    case 14:
                        if (includeTime) {
                            formattedDate = tempDate.substring(0, 4) + "/" + tempDate.substring(4, 6) + "/" + tempDate.substring(6, 8)
                                    + " " + tempDate.substring(8, 10) + ":" + tempDate.substring(10, 12) + ":" + tempDate.substring(12, 14);
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        if (includeTime && dateTimeParts.length > 1 && formattedDate.length() == 10) {
            String tempTime = dateTimeParts[1].replace(" ", "");
            String[] timeParts = tempTime.split(":");
            if (!MdlUtil.isNumeric(tempTime.replace(":", ""))) {
                return formattedDate;
            }

            switch (timeParts.length) {
                case 3:
                    int h3 = MdlUtil.parseInt(timeParts[0], 0);
                    int min3 = MdlUtil.parseInt(timeParts[1], 0);
                    int s3 = MdlUtil.parseInt(timeParts[2], 0);
                    formattedDate += " " + String.format(Locale.ROOT, "%02d:%02d:%02d", h3, min3, s3);
                    break;
                case 2:
                    int h2 = MdlUtil.parseInt(timeParts[0], 0);
                    int min2 = MdlUtil.parseInt(timeParts[1], 0);
                    formattedDate += " " + String.format(Locale.ROOT, "%02d:%02d", h2, min2) + ":00";
                    break;
                case 1:
                    switch (tempTime.length()) {
                        case 2:
                            formattedDate += " " + tempTime.substring(0, 2) + ":00:00";
                            break;
                        case 4:
                            formattedDate += " " + tempTime.substring(0, 2) + ":" + tempTime.substring(2, 4) + ":00";
                            break;
                        case 6:
                            formattedDate += " " + tempTime.substring(0, 2) + ":" + tempTime.substring(2, 4) + ":" + tempTime.substring(4, 6);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        return isValidDate(formattedDate) ? formattedDate : "";
    }

    /**
     * @deprecated {@link #validateAndFormat(String, boolean)} を使用してください。
     */
    @Deprecated
    public static String validateAndFormatDate(String dateString, boolean includeTime) {
        return validateAndFormat(dateString, includeTime);
    }

    /**
     * 不定形式の日付文字列を解析し、標準的な日付形式（YYYY/MM/DD）に変換・検証します。
     *
     * @param dateString 入力日付文字列
     * @return 正規化された日付文字列。無効な日付の場合は空文字列
     */
    public static String validateAndFormat(String dateString) {
        return validateAndFormat(dateString, false);
    }

    /**
     * @deprecated {@link #validateAndFormat(String)} を使用してください。
     */
    @Deprecated
    public static String validateAndFormatDate(String dateString) {
        return validateAndFormat(dateString);
    }

    /**
     * 任意文字列から正規表現を用いて日付パターン（YYYYMMDD または YYYYMMDDHHMMSS）を抽出し、YYYY/MM/DD 形式に整形します。
     *
     * @param inputString 入力文字列
     * @param checkDate 日付の妥当性をチェックするかどうか
     * @param dateCheckType 日付検証の閾値タイプ
     * @return 抽出・フォーマットされた日付文字列。見つからない場合は空文字列
     */
    public static String extractAndFormatDate(String inputString, boolean checkDate, int dateCheckType) {
        String basePattern1 = PATTERN_YYYYMMDD;
        String basePattern2 = PATTERN_YYYYMMDDHHMMSS;

        String result1 = extractDateAny(inputString, basePattern1, checkDate, dateCheckType);
        boolean isPattern1Matched = !result1.isEmpty();

        String result2 = extractDateAny(inputString, basePattern2, checkDate, dateCheckType);
        boolean isPattern2Matched = !result2.isEmpty();

        if (!isPattern1Matched && !isPattern2Matched) {
            return "";
        }

        if (isPattern1Matched) {
            String res = extractDateStart(inputString, basePattern1, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }
        if (isPattern2Matched) {
            String res = extractDateStart(inputString, basePattern2, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }

        if (isPattern1Matched) {
            String res = extractDateContains(inputString, basePattern1, "DD", checkDate, 0, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }
        if (isPattern2Matched) {
            String res = extractDateContains(inputString, basePattern2, "SEC", checkDate, 0, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }

        if (isPattern1Matched) {
            String res = extractDateEndsWith(inputString, basePattern1, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }
        if (isPattern2Matched) {
            String res = extractDateEndsWith(inputString, basePattern2, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }

        if (isPattern1Matched) {
            String res = extractDateExact(inputString, basePattern1, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }
        if (isPattern2Matched) {
            String res = extractDateExact(inputString, basePattern2, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }
        return "";
    }

    /**
     * @deprecated {@link #extractAndFormatDate(String, boolean, int)} を使用してください。
     */
    @Deprecated
    public static String extractAndFormatDateString(String inputString, boolean checkDate, int dateCheckType) {
        return extractAndFormatDate(inputString, checkDate, dateCheckType);
    }

    /**
     * ファイルパスからファイル名を取り出し、日付文字列を抽出・フォーマットします。
     *
     * @param path 入力パス文字列
     * @param checkDate 日付の妥当性をチェックするかどうか
     * @return 抽出・フォーマットされた日付文字列
     */
    public static String extractAndFormatDate(String path, boolean checkDate) {
        return extractAndFormatDate(path, checkDate, 0);
    }

    /**
     * @deprecated {@link #extractAndFormatDate(String, boolean)} を使用してください。
     */
    @Deprecated
    public static String extractAndFormatDateString(String path, boolean checkDate) {
        return extractAndFormatDate(path, checkDate);
    }

    /**
     * ファイルパスからファイル名を取り出し、日付文字列を抽出・フォーマットします。
     *
     * @param path 入力パス文字列
     * @return 抽出・フォーマットされた日付文字列
     */
    public static String extractAndFormatDate(String path) {
        return extractAndFormatDate(path, true, 0);
    }

    /**
     * @deprecated {@link #extractAndFormatDate(String)} を使用してください。
     */
    @Deprecated
    public static String extractAndFormatDateString(String path) {
        return extractAndFormatDate(path);
    }

    /**
     * ファイルパスからファイル名を取り出し、日付文字列を抽出・フォーマットします。
     *
     * @param path 入力パス文字列
     * @param checkDate 日付の妥当性をチェックするかどうか
     * @param dateCheckType 日付検証の閾値タイプ
     * @return 抽出・フォーマットされた日付文字列
     */
    public static String extractDateFromPath(String path, boolean checkDate, int dateCheckType) {
        if (path == null) {
            return "";
        }
        File file = new File(path);
        return extractAndFormatDate(file.getName(), checkDate, dateCheckType);
    }

    /**
     * ファイルパスからファイル名を取り出し、日付文字列を抽出・フォーマットします。
     *
     * @param path 入力パス文字列
     * @param checkDate 日付の妥当性をチェックするかどうか
     * @return 抽出・フォーマットされた日付文字列
     */
    public static String extractDateFromPath(String path, boolean checkDate) {
        return extractDateFromPath(path, checkDate, 0);
    }

    /**
     * ファイルパスからファイル名を取り出し、日付文字列を抽出・フォーマットします。
     *
     * @param path 入力パス文字列
     * @return 抽出・フォーマットされた日付文字列
     */
    public static String extractDateFromPath(String path) {
        return extractDateFromPath(path, true, 0);
    }

    /**
     * 入力文字列から末尾側優先で日付文字列を検索・抽出し、YYYY/MM/DD 形式に整形します。
     *
     * @param inputString 入力文字列
     * @param checkDate 日付の妥当性をチェックするかどうか
     * @param dateCheckType 日付検証の閾値タイプ
     * @return 抽出・フォーマットされた日付文字列
     */
    public static String extractDateReverse(String inputString, boolean checkDate, int dateCheckType) {
        String basePattern1 = PATTERN_YYYYMMDD;
        String basePattern2 = PATTERN_YYYYMMDDHHMMSS;

        String result1 = extractDateAny(inputString, basePattern1, checkDate, dateCheckType);
        boolean isPattern1Matched = !result1.isEmpty();

        String result2 = extractDateAny(inputString, basePattern2, checkDate, dateCheckType);
        boolean isPattern2Matched = !result2.isEmpty();

        if (!isPattern1Matched && !isPattern2Matched) {
            return "";
        }

        if (isPattern1Matched) {
            String res = extractDateEndsWith(inputString, basePattern1, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }
        if (isPattern2Matched) {
            String res = extractDateEndsWith(inputString, basePattern2, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }

        if (isPattern1Matched) {
            String res = extractDateContains(inputString, basePattern1, "DD", checkDate, 1, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }
        if (isPattern2Matched) {
            String res = extractDateContains(inputString, basePattern2, "SEC", checkDate, 1, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }

        if (isPattern1Matched) {
            String res = extractDateStart(inputString, basePattern1, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }
        if (isPattern2Matched) {
            String res = extractDateStart(inputString, basePattern2, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }

        if (isPattern1Matched) {
            String res = extractDateExact(inputString, basePattern1, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }
        if (isPattern2Matched) {
            String res = extractDateExact(inputString, basePattern2, checkDate, dateCheckType);
            if (!res.isEmpty()) {
                return res;
            }
        }
        return "";
    }

    /**
     * @deprecated {@link #extractDateReverse(String, boolean, int)} を使用してください。
     */
    @Deprecated
    public static String extractDateFromStringReverse(String inputString, boolean checkDate, int dateCheckType) {
        return extractDateReverse(inputString, checkDate, dateCheckType);
    }

    /**
     * 入力文字列から末尾側優先で日付文字列を検索・抽出し、YYYY/MM/DD 形式に整形します。
     *
     * @param inputString 入力文字列
     * @param checkDate 日付の妥当性をチェックするかどうか
     * @return 抽出・フォーマットされた日付文字列
     */
    public static String extractDateReverse(String inputString, boolean checkDate) {
        return extractDateReverse(inputString, checkDate, 0);
    }

    /**
     * @deprecated {@link #extractDateReverse(String, boolean)} を使用してください。
     */
    @Deprecated
    public static String extractDateFromStringReverse(String inputString, boolean checkDate) {
        return extractDateReverse(inputString, checkDate);
    }

    /**
     * 入力文字列から末尾側優先で日付文字列を検索・抽出し、YYYY/MM/DD 形式に整形します。
     *
     * @param inputString 入力文字列
     * @return 抽出・フォーマットされた日付文字列
     */
    public static String extractDateReverse(String inputString) {
        return extractDateReverse(inputString, true, 0);
    }

    /**
     * @deprecated {@link #extractDateReverse(String)} を使用してください。
     */
    @Deprecated
    public static String extractDateFromStringReverse(String inputString) {
        return extractDateReverse(inputString);
    }

    /**
     * 指定された正規表現パターンにマッチする最初の日付部分文字列を抽出します。
     *
     * @param input 入力文字列
     * @param pattern 検索に使用する正規表現パターン
     * @param validateDate 日付の妥当性をチェックするかどうか
     * @param dateFormat 日付検証の閾値タイプ
     * @return 抽出された日付文字列（YYYY/MM/DD形式）
     */
    public static String extractDateAny(String input, String pattern, boolean validateDate, int dateFormat) {
        if (input == null || pattern == null) {
            return "";
        }
        Pattern regex = pattern.equals(PATTERN_YYYYMMDD) ? YYYY_MM_DD_REGEX :
                pattern.equals(PATTERN_YYYYMMDDHHMMSS) ? YYYY_MM_DD_HH_MM_SS_REGEX :
                Pattern.compile(pattern);

        Matcher matcher = regex.matcher(input);
        while (matcher.find()) {
            String dateString = matcher.group("YYYY") + "/" + matcher.group("MM") + "/" + matcher.group("DD");
            if (!validateDate || isValidDate(dateString, dateFormat)) {
                return dateString;
            }
        }
        return "";
    }

    /**
     * 文字列の先頭位置（^パターン）から始まる日付部分文字列を抽出します。
     *
     * @param input 入力文字列
     * @param basePattern 基本となる正規表現パターン
     * @param validateDate 日付の妥当性をチェックするかどうか
     * @param dateFormat 日付検証の閾値タイプ
     * @return 抽出された日付文字列（YYYY/MM/DD形式）
     */
    public static String extractDateStart(String input, String basePattern, boolean validateDate, int dateFormat) {
        if (input == null || basePattern == null) {
            return "";
        }
        String patternStr = "^" + basePattern + "[^0-9]+";
        try {
            Pattern regex = Pattern.compile(patternStr);
            Matcher matcher = regex.matcher(input);
            while (matcher.find()) {
                String dateString = matcher.group("YYYY") + "/" + matcher.group("MM") + "/" + matcher.group("DD");
                if (!validateDate || isValidDate(dateString, dateFormat)) {
                    return dateString;
                }
            }
        } catch (Exception e) {
            // パターンエラー
        }
        return "";
    }

    /**
     * @deprecated {@link #extractDateStart(String, String, boolean, int)} を使用してください。
     */
    @Deprecated
    public static String extractDateStartsWith(String input, String basePattern, boolean validateDate, int dateFormat) {
        return extractDateStart(input, basePattern, validateDate, dateFormat);
    }

    /**
     * 文字列の中間位置から数字以外で挟まれた日付部分文字列を抽出します。
     *
     * @param input 入力文字列
     * @param basePattern 基本となる正規表現パターン
     * @param checkGroup グループチェック名（例: "DD", "SEC"）
     * @param validateDate 日付の妥当性をチェックするかどうか
     * @param mode 0 の場合は最初に見つかった項目、1 の場合は最後に見つかった項目を返します
     * @param dateFormat 日付検証の閾値タイプ
     * @return 抽出された日付文字列（YYYY/MM/DD形式）
     */
    public static String extractDateContains(String input, String basePattern, String checkGroup, boolean validateDate, int mode, int dateFormat) {
        if (input == null || basePattern == null) {
            return "";
        }
        String patternStr = "[^0-9]+" + basePattern;
        List<String> matchesList = new ArrayList<>();
        try {
            Pattern regex = Pattern.compile(patternStr);
            Matcher matcher = regex.matcher(input);
            while (matcher.find()) {
                int checkPos = matcher.end(checkGroup);
                if (input.length() > checkPos && !MdlUtil.isNumeric(input.substring(checkPos, checkPos + 1))) {
                    String dateString = matcher.group("YYYY") + "/" + matcher.group("MM") + "/" + matcher.group("DD");
                    boolean isValid = !validateDate || isValidDate(dateString, dateFormat);
                    if (isValid) {
                        if (mode == 0) {
                            return dateString;
                        }
                        matchesList.add(dateString);
                    }
                }
            }
        } catch (Exception e) {
            // パターンエラー
        }
        return !matchesList.isEmpty() ? matchesList.get(matchesList.size() - 1) : "";
    }

    /**
     * 文字列の末尾位置（パターン$）で終わる日付部分文字列を抽出します。
     *
     * @param input 入力文字列
     * @param basePattern 基本となる正規表現パターン
     * @param validateDate 日付の妥当性をチェックするかどうか
     * @param dateFormat 日付検証の閾値タイプ
     * @return 抽出された日付文字列（YYYY/MM/DD形式）
     */
    public static String extractDateEndsWith(String input, String basePattern, boolean validateDate, int dateFormat) {
        if (input == null || basePattern == null) {
            return "";
        }
        String patternStr = "[^0-9]+" + basePattern + "$";
        try {
            Pattern regex = Pattern.compile(patternStr);
            Matcher matcher = regex.matcher(input);
            while (matcher.find()) {
                String dateString = matcher.group("YYYY") + "/" + matcher.group("MM") + "/" + matcher.group("DD");
                if (!validateDate || isValidDate(dateString, dateFormat)) {
                    return dateString;
                }
            }
        } catch (Exception e) {
            // パターンエラー
        }
        return "";
    }

    /**
     * 文字列全体がパターンと完全一致（^パターン$）する日付部分文字列を抽出します。
     *
     * @param input 入力文字列
     * @param basePattern 基本となる正規表現パターン
     * @param validateDate 日付の妥当性をチェックするかどうか
     * @param dateFormat 日付検証の閾値タイプ
     * @return 抽出された日付文字列（YYYY/MM/DD形式）
     */
    public static String extractDateExact(String input, String basePattern, boolean validateDate, int dateFormat) {
        if (input == null || basePattern == null) {
            return "";
        }
        String patternStr = "^" + basePattern + "$";
        try {
            Pattern regex = Pattern.compile(patternStr);
            Matcher matcher = regex.matcher(input);
            while (matcher.find()) {
                String dateString = matcher.group("YYYY") + "/" + matcher.group("MM") + "/" + matcher.group("DD");
                if (!validateDate || isValidDate(dateString, dateFormat)) {
                    return dateString;
                }
            }
        } catch (Exception e) {
            // パターンエラー
        }
        return "";
    }

    /**
     * 2つの LocalDateTime を比較し、差分が指定した許容秒数範囲内であるかを判定します。
     *
     * @param first 比較する最初の日時
     * @param second 比較する2つ目の日時
     * @param secondRange 許容する秒数の範囲
     * @return 差分が範囲内の場合は 0。範囲外の場合で first が大きければ 1、小さければ -1
     */
    public static int compareDateTime(LocalDateTime first, LocalDateTime second, double secondRange) {
        if (first == null || second == null) {
            return 0;
        }
        try {
            int comparisonResult = first.compareTo(second);
            long diffSeconds = ChronoUnit.SECONDS.between(second, first);
            if (comparisonResult > 0) {
                if (Math.abs(diffSeconds) >= secondRange) {
                    return 1;
                }
            } else if (comparisonResult < 0) {
                if (Math.abs(diffSeconds) >= secondRange) {
                    return -1;
                }
            }
        } catch (Exception e) {
            // エラー時は0
        }
        return 0;
    }

    /**
     * 秒数を "HH:mm:ss" または "24時間以上の時間:mm:ss" 形式の時間文字列に変換します。
     *
     * @param seconds 変換する秒数
     * @return 整形された時間文字列
     */
    public static String secondsToTimeString(int seconds) {
        boolean isNegative = seconds < 0;
        int absSeconds = Math.abs(seconds);
        int hours = absSeconds / 3600;
        int minutes = (absSeconds % 3600) / 60;
        int secs = absSeconds % 60;

        String timeStr = String.format(Locale.ROOT, "%02d:%02d:%02d", hours, minutes, secs);
        return isNegative ? "-" + timeStr : timeStr;
    }

    /**
     * @deprecated {@link #secondsToTimeString(int)} を使用してください。
     */
    @Deprecated
    public static String convertSecondsToTimeString(int seconds) {
        return secondsToTimeString(seconds);
    }

    /**
     * 長整数型の秒数を "HH:mm:ss" または "24時間以上の時間:mm:ss" 形式の時間文字列に変換します。
     *
     * @param seconds 変換する秒数（long型）
     * @return 整形された時間文字列
     */
    public static String secondsToTimeString(long seconds) {
        return secondsToTimeString((int) seconds);
    }

    /**
     * @deprecated {@link #secondsToTimeString(long)} を使用してください。
     */
    @Deprecated
    public static String convertSecondsToTimeString(long seconds) {
        return secondsToTimeString((int) seconds);
    }

    /**
     * テンプレート文字列内の指定フォーマットプレースホルダー（%Y, %m, %d, %H, %M, %S, %w, %pid）を日時の値で置換します。
     *
     * @param target 置換対象のテンプレート文字列
     * @param currentDateTime 置換に使用する日時
     * @return 置換後の文字列
     */
    public static String replaceWithDateTime(String target, LocalDateTime currentDateTime) {
        if (target == null) {
            return "";
        }
        LocalDateTime dt = currentDateTime != null ? currentDateTime : LocalDateTime.now();

        String result = target;
        result = result.replace("%y", dt.format(DateTimeFormatter.ofPattern("yy", Locale.ROOT)));
        result = result.replace("%Y", dt.format(DateTimeFormatter.ofPattern("yyyy", Locale.ROOT)));
        result = result.replace("%m", dt.format(DateTimeFormatter.ofPattern("MM", Locale.ROOT)));
        result = result.replace("%d", dt.format(DateTimeFormatter.ofPattern("dd", Locale.ROOT)));
        result = result.replace("%H", dt.format(DateTimeFormatter.ofPattern("HH", Locale.ROOT)));
        result = result.replace("%M", dt.format(DateTimeFormatter.ofPattern("mm", Locale.ROOT)));
        result = result.replace("%S", dt.format(DateTimeFormatter.ofPattern("ss", Locale.ROOT)));
        result = result.replace("%w", Integer.toString(dt.getDayOfWeek().getValue() % 7)); // JavaのDayOfWeek: Monday=1...Sunday=7 -> Sunday=0

        if (result.contains("%pid")) {
            String pid = ManagementFactory.getRuntimeMXBean().getName().split("@")[0];
            result = result.replace("%pid", pid);
        }

        return result;
    }

    /**
     * @deprecated {@link #replaceWithDateTime(String, LocalDateTime)} を使用してください。
     */
    @Deprecated
    public static String replaceStringWithDateTime(String target, LocalDateTime currentDateTime) {
        return replaceWithDateTime(target, currentDateTime);
    }

    /**
     * テンプレート文字列内の指定フォーマットプレースホルダーを現在日時で置換します。
     *
     * @param target 置換対象のテンプレート文字列
     * @return 置換後の文字列
     */
    public static String replaceWithDateTime(String target) {
        return replaceWithDateTime(target, LocalDateTime.now());
    }

    /**
     * @deprecated {@link #replaceWithDateTime(String)} を使用してください。
     */
    @Deprecated
    public static String replaceStringWithDateTime(String target) {
        return replaceWithDateTime(target);
    }

    /**
     * 指定した日時でテンプレート文字列内の日付プレースホルダー（%Y, %m, %d, %H, %M, %S, %w）を置換します。
     *
     * @param date 置換に使用する日時
     * @param target フォーマット対象の文字列
     * @return 置換後の文字列
     */
    public static String convertFormattedDate(LocalDateTime date, String target) {
        return replaceWithDateTime(target, date);
    }
}
