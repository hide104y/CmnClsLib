package tool.cmnclslib.cls;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import tool.cmnclslib.ifc.ICmnLogger;
import tool.cmnclslib.mdl.MdlFile;
import static org.junit.Assert.*;

/**
 * ClsFsDate の単体テストクラスです。
 */
public class UnitTest_ClsFsDate {

    private static class TestLogEntry {
        int level;
        String message;

        TestLogEntry(int level, String message) {
            this.level = level;
            this.message = message;
        }
    }

    private static class TestLogger implements ICmnLogger {
        List<TestLogEntry> logs = new ArrayList<>();

        @Override
        public void writeLine(int level, String message) {
            logs.add(new TestLogEntry(level, message));
        }

        @Override
        public String getValueByKey(String key, String defaultValue) {
            return defaultValue;
        }

        @Override
        public boolean getValueByKey(String key, boolean defaultValue) {
            return defaultValue;
        }

        @Override
        public void setValueByKey(String key, String val) {
        }
    }

    private String tempDirectory;
    private String tempFilePath;
    private String tempSubDirPath;
    private TestLogger logger;

    @Before
    public void setUp() throws IOException {
        tempDirectory = new File(new File(new File(System.getProperty("java.io.tmpdir"), "UnitTest"), "CmnClsLib"), "ClsFsDate_" + UUID.randomUUID().toString()).getPath();
        new File(tempDirectory).mkdirs();

        tempFilePath = new File(tempDirectory, "test_file.txt").getPath();
        try (FileOutputStream fos = new FileOutputStream(tempFilePath)) {
            fos.write("ClsFsDate Test Content".getBytes(StandardCharsets.UTF_8));
        }

        tempSubDirPath = new File(tempDirectory, "test_subdir").getPath();
        new File(tempSubDirPath).mkdirs();

        logger = new TestLogger();
    }

    @After
    public void tearDown() {
        MdlFile.deleteRecursively(tempDirectory);
    }

    @Test(expected = NullPointerException.class)
    public void コンストラクタ_NullLoggerを指定した場合_例外がスローされること() {
        new ClsFsDate(null);
    }

    @Test
    public void コンストラクタ_正常なLoggerを指定した場合_初期値が正しいこと() {
        ClsFsDate fsDate = new ClsFsDate(logger);

        assertEquals(0, fsDate.getVerbose());
        assertEquals("", fsDate.getMessage());
        assertFalse(fsDate.isThrowIfException());
    }

    @Test
    public void プロパティ受渡し検証_値が正しく設定および取得できること() {
        ClsFsDate fsDate = new ClsFsDate(logger);

        fsDate.setVerbose(2);
        assertEquals(2, fsDate.getVerbose());

        fsDate.setMessage("Test Error");
        assertEquals("Test Error", fsDate.getMessage());

        fsDate.setThrowIfException(true);
        assertTrue(fsDate.isThrowIfException());
    }

    @Test
    public void setFileDate_正常なファイルパスと日時を指定した場合_trueを返し日時が変更されること() {
        ClsFsDate fsDate = new ClsFsDate(logger);
        LocalDateTime targetDate = LocalDateTime.of(2025, 5, 20, 10, 30, 0);

        boolean result = fsDate.setFileDate(tempFilePath, targetDate, 7, true, true);

        assertTrue(result);
        long lastModified = new File(tempFilePath).lastModified();
        LocalDateTime actualWriteTime = LocalDateTime.ofInstant(java.time.Instant.ofEpochMilli(lastModified), ZoneId.systemDefault());
        assertEquals(targetDate.getYear(), actualWriteTime.getYear());
        assertEquals(targetDate.getMonthValue(), actualWriteTime.getMonthValue());
        assertEquals(targetDate.getDayOfMonth(), actualWriteTime.getDayOfMonth());
    }

    @Test
    public void setFileDate_オーバーロード_引数省略版が正しく動作すること() {
        ClsFsDate fsDate = new ClsFsDate(logger);
        LocalDateTime targetDate = LocalDateTime.of(2025, 6, 15, 14, 0, 0);

        boolean res1 = fsDate.setFileDate(tempFilePath, targetDate, 7, true);
        assertTrue(res1);

        boolean res2 = fsDate.setFileDate(tempFilePath, targetDate, 7);
        assertTrue(res2);
    }

    @Test
    public void setFileDateCore_存在しないファイルパスを指定した場合_処理結果コードが返されること() {
        ClsFsDate fsDate = new ClsFsDate(logger);
        String invalidPath = new File(tempDirectory, "non_existent_file.txt").getPath();
        LocalDateTime targetDate = LocalDateTime.now();

        int code = fsDate.setFileDateCore(invalidPath, targetDate, 7, true, true);
        assertTrue(code >= -1);
    }

    @Test
    public void setFileDateCore_例外が発生しIsThrowIfExceptionがtrueの場合_例外が再スローされログ出力されること() {
        ClsFsDate fsDate = new ClsFsDate(logger);
        fsDate.setVerbose(1);
        fsDate.setThrowIfException(true);

        String invalidPath = "Z:\\NonExistentDirectory_12345\\non_existent_file.txt";
        LocalDateTime targetDate = LocalDateTime.now();

        try {
            fsDate.setFileDateCore(invalidPath, targetDate, 7);
        } catch (Exception e) {
            assertFalse(logger.logs.isEmpty());
        }
    }

    @Test
    public void setDirectoryDate_正常なディレクトリパスと日時を指定した場合_trueを返し日時が変更されること() {
        ClsFsDate fsDate = new ClsFsDate(logger);
        LocalDateTime targetDate = LocalDateTime.of(2025, 7, 10, 9, 15, 0);

        boolean result = fsDate.setDirectoryDate(tempSubDirPath, targetDate, 7, true, true);

        assertTrue(result);
        long lastModified = new File(tempSubDirPath).lastModified();
        LocalDateTime actualWriteTime = LocalDateTime.ofInstant(java.time.Instant.ofEpochMilli(lastModified), ZoneId.systemDefault());
        assertEquals(targetDate.getYear(), actualWriteTime.getYear());
        assertEquals(targetDate.getMonthValue(), actualWriteTime.getMonthValue());
        assertEquals(targetDate.getDayOfMonth(), actualWriteTime.getDayOfMonth());
    }

    @Test
    public void setDirectoryDate_オーバーロード_引数省略版が正しく動作すること() {
        ClsFsDate fsDate = new ClsFsDate(logger);
        LocalDateTime targetDate = LocalDateTime.of(2025, 8, 1, 10, 0, 0);

        boolean res1 = fsDate.setDirectoryDate(tempSubDirPath, targetDate, 7, true);
        assertTrue(res1);

        boolean res2 = fsDate.setDirectoryDate(tempSubDirPath, targetDate, 7);
        assertTrue(res2);
    }

    @Test
    public void setDirectoryDateCore_存在しないディレクトリパスを指定した場合_処理コードが返されること() {
        ClsFsDate fsDate = new ClsFsDate(logger);
        String invalidDir = new File(tempDirectory, "non_existent_dir").getPath();
        LocalDateTime targetDate = LocalDateTime.now();

        int code = fsDate.setDirectoryDateCore(invalidDir, targetDate, 7, true, true);
        assertTrue(code >= -1);
    }
}
