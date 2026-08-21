package tool.cmnclslib.cls;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.time.LocalDateTime;
import java.util.Random;
import java.util.UUID;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import tool.cmnclslib.mdl.MdlFile;
import static org.junit.Assert.*;

/**
 * ClsFsAsyncCopyStatus の単体テストクラスです。
 */
public class UnitTest_ClsFsAsyncCopyStatus {

    private String tempDirectory;
    private String tempSourcePath;
    private String tempDestPath;

    @Before
    public void setUp() throws IOException {
        tempDirectory = new File(new File(new File(System.getProperty("java.io.tmpdir"), "UnitTest"), "CmnClsLib"), "ClsFsAsyncCopyStatus_" + UUID.randomUUID().toString()).getPath();
        new File(tempDirectory).mkdirs();
        tempSourcePath = new File(tempDirectory, "source.txt").getPath();
        tempDestPath = new File(tempDirectory, "dest.txt").getPath();

        byte[] dummyData = new byte[10240];
        new Random(42).nextBytes(dummyData);
        try (FileOutputStream fos = new FileOutputStream(tempSourcePath)) {
            fos.write(dummyData);
        }
    }

    @After
    public void tearDown() {
        MdlFile.deleteRecursively(tempDirectory);
    }

    @Test
    public void コンストラクタ4引数_正常なパスを指定した場合_初期化が成功しストリームが開かれること() {
        try (ClsFsAsyncCopyStatus status = new ClsFsAsyncCopyStatus(tempSourcePath, tempDestPath, false, 0)) {
            assertTrue(status.isOk());
            assertNotNull(status.getSourceStream());
            assertNotNull(status.getDestinationStream());
            assertEquals(10240, status.getFileSize());
            assertFalse(status.isDone());
        }
    }

    @Test
    public void コンストラクタ3引数_正常なパスを指定した場合_初期化されること() {
        try (ClsFsAsyncCopyStatus status = new ClsFsAsyncCopyStatus(tempSourcePath, tempDestPath, true)) {
            assertTrue(status.isOk());
            assertNotNull(status.getSourceStream());
            assertNotNull(status.getDestinationStream());
        }
    }

    @Test
    public void コンストラクタ_存在しないソースパスを指定した場合_IsOkがfalseとなりメッセージが設定されること() {
        String invalidSource = new File(tempDirectory, "non_existent.txt").getPath();
        try (ClsFsAsyncCopyStatus status = new ClsFsAsyncCopyStatus(invalidSource, tempDestPath, false)) {
            assertFalse(status.isOk());
            assertNull(status.getSourceStream());
            assertFalse(status.getMessage().isEmpty());
        }
    }

    @Test
    public void openSourceFile_パスがNullまたは空の場合_falseを返すこと() {
        try (ClsFsAsyncCopyStatus status = new ClsFsAsyncCopyStatus(tempSourcePath, tempDestPath, false)) {
            assertFalse(status.openSourceFile(null, false));
            assertFalse(status.openSourceFile("", false));
        }
    }

    @Test
    public void openDestinationFile_パスがNullまたは空の場合_falseを返すこと() {
        try (ClsFsAsyncCopyStatus status = new ClsFsAsyncCopyStatus(tempSourcePath, tempDestPath, false)) {
            assertFalse(status.openDestinationFile(null, false));
            assertFalse(status.openDestinationFile("", false));
        }
    }

    @Test
    public void showProgress_IsShowProgressがtrueの場合_ProgressLineが正しく更新されること() {
        try (ClsFsAsyncCopyStatus status = new ClsFsAsyncCopyStatus(tempSourcePath, tempDestPath, false)) {
            status.setShowProgress(true);
            status.showProgress();
            assertFalse(status.getProgressLine().isEmpty());
            assertTrue(status.getProgressLine().contains("%"));
        }
    }

    @Test
    public void showProgress_IsShowProgressがfalseの場合_ProgressLineが更新されないこと() {
        try (ClsFsAsyncCopyStatus status = new ClsFsAsyncCopyStatus(tempSourcePath, tempDestPath, false)) {
            status.setShowProgress(false);
            status.setProgressLine("Initial");
            status.showProgress();
            assertEquals("Initial", status.getProgressLine());
        }
    }

    @Test
    public void dispose_実行後にストリームがnullになりIsDoneがtrueになること() {
        ClsFsAsyncCopyStatus status = new ClsFsAsyncCopyStatus(tempSourcePath, tempDestPath, false);
        assertNotNull(status.getSourceStream());
        assertNotNull(status.getDestinationStream());

        status.dispose();

        assertTrue(status.isDone());
        assertNull(status.getSourceStream());
        assertNull(status.getDestinationStream());
    }

    @Test
    public void close_Disposeと同等にリソースが解放されること() {
        ClsFsAsyncCopyStatus status = new ClsFsAsyncCopyStatus(tempSourcePath, tempDestPath, false);
        status.close();

        assertTrue(status.isDone());
        assertNull(status.getSourceStream());
        assertNull(status.getDestinationStream());
    }

    @Test
    public void プロパティ受渡し検証_値が正しく保持されること() {
        try (ClsFsAsyncCopyStatus status = new ClsFsAsyncCopyStatus(tempSourcePath, tempDestPath, false)) {
            byte[] newBuffer = new byte[8192];
            status.setBuffer(newBuffer);
            assertArrayEquals(newBuffer, status.getBuffer());

            status.setDone(true);
            assertTrue(status.isDone());

            LocalDateTime now = LocalDateTime.now();
            status.setStartTime(now);
            assertEquals(now, status.getStartTime());

            status.setCheckCount(50);
            assertEquals(50, status.getCheckCount());

            status.setCurrentCount(20);
            assertEquals(20, status.getCurrentCount());

            status.setFileSize(2048);
            assertEquals(2048, status.getFileSize());

            status.setOk(false);
            assertFalse(status.isOk());

            status.setMessage("TestMessage");
            assertEquals("TestMessage", status.getMessage());

            status.setStackTrace("TestStackTrace");
            assertEquals("TestStackTrace", status.getStackTrace());

            status.setProgressLine("TestProgress");
            assertEquals("TestProgress", status.getProgressLine());
        }
    }
}
