package tool.cmnclslib.cls;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import org.junit.Test;
import tool.cmnclslib.ifc.ICmnLogger;
import tool.cmnclslib.mdl.MdlConst;
import static org.junit.Assert.*;

/**
 * ClsConfigFile の単体テストクラスです。
 */
public class UnitTest_ClsConfigFile {

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

    private String createTempFile(String content, Charset charset) throws IOException {
        File tempFile = File.createTempFile("ut_cfg_", ".tmp");
        try (FileOutputStream fos = new FileOutputStream(tempFile)) {
            fos.write(content.getBytes(charset != null ? charset : StandardCharsets.UTF_8));
        }
        return tempFile.getAbsolutePath();
    }

    private void deleteTempFile(String filePath) {
        if (filePath != null) {
            new File(filePath).delete();
        }
    }

    @Test
    public void constructor_InitializesEmptyCollections() {
        TestLogger logger = new TestLogger();
        ClsConfigFile configFile = new ClsConfigFile(logger);

        assertNotNull(configFile.getConfigDictionary());
        assertTrue(configFile.getConfigDictionary().isEmpty());
        assertNotNull(configFile.getListDictionary());
        assertTrue(configFile.getListDictionary().isEmpty());
        assertNotNull(configFile.getConfigList());
        assertTrue(configFile.getConfigList().isEmpty());
        assertNotNull(configFile.getDuplicateKeys());
        assertTrue(configFile.getDuplicateKeys().isEmpty());
    }

    @Test
    public void clear_ClearsAllCollections() {
        TestLogger logger = new TestLogger();
        ClsConfigFile configFile = new ClsConfigFile(logger);

        configFile.getConfigDictionary().put("Key1", "Value1");
        configFile.getListDictionary().put("DupKey", new ArrayList<>(Arrays.asList("Val1", "Val2")));
        configFile.getConfigList().add("Line1");
        configFile.getDuplicateKeys().add("DupKey");

        configFile.clear();

        assertTrue(configFile.getConfigDictionary().isEmpty());
        assertTrue(configFile.getListDictionary().isEmpty());
        assertTrue(configFile.getConfigList().isEmpty());
        assertTrue(configFile.getDuplicateKeys().isEmpty());
    }

    @Test
    public void properties_GetAndSet_WorksCorrectly() {
        TestLogger logger = new TestLogger();
        ClsConfigFile configFile = new ClsConfigFile(logger);

        configFile.setVerbose(5);
        assertEquals(5, configFile.getVerbose());

        String customPattern = "^(?<KEY>[^:]+):(?<VALUE>.+)$";
        configFile.setPattern(customPattern);
        assertEquals(customPattern, configFile.getPattern());

        configFile.setEncoding(StandardCharsets.UTF_8);
        assertEquals(StandardCharsets.UTF_8, configFile.getEncoding());

        configFile.setSkipComment(false);
        assertFalse(configFile.isSkipComment());
    }

    @Test
    public void loadToDictionary_ValidFile_ParsesKeyValuePairs() throws IOException {
        String content = "Server=Localhost\nPort=8080\nTitle=\"My Application\"";
        String tempFile = createTempFile(content, StandardCharsets.UTF_8);

        try {
            TestLogger logger = new TestLogger();
            ClsConfigFile configFile = new ClsConfigFile(logger);

            int count = configFile.loadToDictionary(tempFile);

            assertEquals(3, count);
            assertEquals("Localhost", configFile.getConfigDictionary().get("Server"));
            assertEquals("8080", configFile.getConfigDictionary().get("Port"));
            assertEquals("My Application", configFile.getConfigDictionary().get("Title"));
        } finally {
            deleteTempFile(tempFile);
        }
    }

    @Test
    public void loadToDictionary_WithCommentsAndEmptyLines_SkipsCorrectly() throws IOException {
        String content = "# This is a comment\n\nKey1=Val1 # inline comment\n\n# Another comment\nKey2=Val2";
        String tempFile = createTempFile(content, StandardCharsets.UTF_8);

        try {
            TestLogger logger = new TestLogger();
            ClsConfigFile configFile = new ClsConfigFile(logger);
            configFile.setSkipComment(true);

            int count = configFile.loadToDictionary(tempFile);

            assertEquals(2, count);
            assertEquals("Val1", configFile.getConfigDictionary().get("Key1"));
            assertEquals("Val2", configFile.getConfigDictionary().get("Key2"));
        } finally {
            deleteTempFile(tempFile);
        }
    }

    @Test
    public void loadToDictionary_DuplicateKeys_StoresMultipleValuesInListDictionary() throws IOException {
        String content = "Item=Apple\nItem=Banana\nOther=Single";
        String tempFile = createTempFile(content, StandardCharsets.UTF_8);

        try {
            TestLogger logger = new TestLogger();
            ClsConfigFile configFile = new ClsConfigFile(logger);
            configFile.getDuplicateKeys().add("Item");

            int count = configFile.loadToDictionary(tempFile);

            assertEquals(2, count);
            assertEquals("Banana", configFile.getConfigDictionary().get("Item"));
            assertTrue(configFile.getListDictionary().containsKey("Item"));
            assertEquals(Arrays.asList("Apple", "Banana"), configFile.getListDictionary().get("Item"));
        } finally {
            deleteTempFile(tempFile);
        }
    }

    @Test
    public void loadToDictionary_CustomPattern_ParsesWithCustomRegex() throws IOException {
        String content = "Key1:Val1\nKey2:Val2";
        String tempFile = createTempFile(content, StandardCharsets.UTF_8);

        try {
            TestLogger logger = new TestLogger();
            ClsConfigFile configFile = new ClsConfigFile(logger);
            configFile.setPattern("^(?<KEY>[^:]+):(?<VALUE>.+)$");

            int count = configFile.loadToDictionary(tempFile);

            assertEquals(2, count);
            assertEquals("Val1", configFile.getConfigDictionary().get("Key1"));
            assertEquals("Val2", configFile.getConfigDictionary().get("Key2"));
        } finally {
            deleteTempFile(tempFile);
        }
    }

    @Test
    public void loadToDictionary_FileNotFound_ReturnsMinusOne() {
        TestLogger logger = new TestLogger();
        ClsConfigFile configFile = new ClsConfigFile(logger);

        int result = configFile.loadToDictionary("C:\\non_existent_file_123456.conf");

        assertEquals(-1, result);
        assertFalse(logger.logs.isEmpty());
    }

    @Test
    public void loadToList_UniqueFalse_ReadsAllLines() throws IOException {
        String content = "Line1\nLine2\nLine1\nLine3";
        String tempFile = createTempFile(content, StandardCharsets.UTF_8);

        try {
            TestLogger logger = new TestLogger();
            ClsConfigFile configFile = new ClsConfigFile(logger);

            int count = configFile.loadToList(tempFile, false);

            assertEquals(4, count);
            assertEquals(Arrays.asList("Line1", "Line2", "Line1", "Line3"), configFile.getConfigList());
        } finally {
            deleteTempFile(tempFile);
        }
    }

    @Test
    public void loadToList_UniqueTrue_RemovesDuplicates() throws IOException {
        String content = "Line1\nLine2\nLine1\nLine3";
        String tempFile = createTempFile(content, StandardCharsets.UTF_8);

        try {
            TestLogger logger = new TestLogger();
            ClsConfigFile configFile = new ClsConfigFile(logger);

            int count = configFile.loadToList(tempFile, true);

            assertEquals(3, count);
            assertEquals(Arrays.asList("Line1", "Line2", "Line3"), configFile.getConfigList());
        } finally {
            deleteTempFile(tempFile);
        }
    }

    @Test
    public void loadToList_FileNotFound_ReturnsMinusOne() {
        TestLogger logger = new TestLogger();
        ClsConfigFile configFile = new ClsConfigFile(logger);

        int result = configFile.loadToList("C:\\non_existent_file_123456.conf", false);

        assertEquals(-1, result);
        assertFalse(logger.logs.isEmpty());
    }

    @Test
    public void writeLog_WithLogger_CallsLoggerWriteLine() {
        TestLogger logger = new TestLogger();
        ClsConfigFile configFile = new ClsConfigFile(logger);

        configFile.writeLog(MdlConst.LVL_DEBUG, "Test Debug Message");

        assertEquals(1, logger.logs.size());
        assertEquals(MdlConst.LVL_DEBUG, logger.logs.get(0).level);
        assertEquals("Test Debug Message", logger.logs.get(0).message);
    }

    @Test
    public void loadToMap_WithValidFile_PopulatesConfigMapAndListMap() throws IOException {
        String content = "Key1=Val1\nKey2=Val2\nDupKey=First\nDupKey=Second";
        String tempFile = createTempFile(content, StandardCharsets.UTF_8);

        try {
            TestLogger logger = new TestLogger();
            ClsConfigFile configFile = new ClsConfigFile(logger);
            configFile.getDuplicateKeys().add("DupKey");

            int count = configFile.loadToMap(tempFile);

            assertEquals(3, count);
            assertEquals("Val1", configFile.getConfigMap().get("Key1"));
            assertEquals("Val2", configFile.getConfigMap().get("Key2"));
            assertEquals("Second", configFile.getConfigMap().get("DupKey"));
            assertEquals(Arrays.asList("First", "Second"), configFile.getListMap().get("DupKey"));
        } finally {
            deleteTempFile(tempFile);
        }
    }
}
