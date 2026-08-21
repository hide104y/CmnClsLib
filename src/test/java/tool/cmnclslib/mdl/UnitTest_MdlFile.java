package tool.cmnclslib.mdl;

import java.io.File;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 * MdlFile の単体テストクラスです。
 */
public class UnitTest_MdlFile {

    private String tempDir = new File(new File(new File(System.getProperty("java.io.tmpdir"), "UnitTest"), "CmnClsLib"), "MdlFile").getPath();

    @Test
    public void getDirectoryPath_引数で指定したファイルのディレクトリパスを返却すること() {
        String target = tempDir + File.separator + "test.txt";
        String expected = tempDir;
        assertEquals(expected, MdlFile.getDirectoryPath(target));
    }

    @Test
    public void getFileNameWithoutExtension_引数で指定したファイルの拡張子無しファイル名を返却すること() {
        String target = tempDir + File.separator + "test.txt";
        String expected = "test";
        assertEquals(expected, MdlFile.getFileNameWithoutExtension(target));
    }

    @Test
    public void getFileName_引数で指定したファイルの拡張子有しファイル名を返却すること() {
        String target = tempDir + File.separator + "test.txt";
        String expected = "test.txt";
        assertEquals(expected, MdlFile.getFileName(target));
    }

    @Test
    public void getFileExtension_引数で指定したファイルの拡張子を返却すること() {
        String target = tempDir + File.separator + "test.txt";
        String expected = "txt";
        assertEquals(expected, MdlFile.getFileExtension(target));
    }

    @Test
    public void createDirectory_引数が空文字列の場合は定数値NG_MKDIR_WRONG_ARGを返却すること() {
        String target = "";
        int expected = MdlFile.NG_MKDIR_WRONG_ARG;
        int actual = MdlFile.createDirectory(target);
        assertEquals(expected, actual);
    }

    @Test
    public void createDirectory_引数に指定したディレクトリの作成に成功した場合は定数値OK_MKDIR_CREATE_0を返却すること() {
        String target = tempDir + File.separator + "CreateDirectory" + File.separator + "002";
        int expected = MdlFile.OK_MKDIR_CREATE;
        MdlFile.deleteRecursively(target);
        int actual = MdlFile.createDirectory(target);
        assertEquals(expected, actual);
    }

    @Test
    public void createDirectory_引数に既に存在するディレクトリを指定した場合は定数値OK_MKDIR_ALREADY_EXIT_1を返却すること() {
        String target = tempDir + File.separator + "CreateDirectory" + File.separator + "003";
        int expected = MdlFile.OK_MKDIR_ALREADY_EXIST;
        MdlFile.createDirectory(target);
        int actual = MdlFile.createDirectory(target);
        assertEquals(expected, actual);
    }

    @Test
    public void createDirectory_引数に既に存在するファイルを指定した場合は定数値NG_MKDIR_FILE_EXIST_13を返却すること() {
        String target = tempDir + File.separator + "CreateDirectory" + File.separator + "AlreadyExist.txt";
        int expected = MdlFile.NG_MKDIR_FILE_EXIST;
        MdlFile.deleteRecursively(target);
        MdlFile.createEmptyFile(target);
        int actual = MdlFile.createDirectory(target);
        assertEquals(expected, actual);
    }

    @Test
    public void createEmptyFile_引数で指定されたファイルの作成に成功した場合OK_TOUCH_CREATE_0を返却すること() {
        String target = tempDir + File.separator + "CreateEmptyFile" + File.separator + "ok.txt";
        int expected = MdlFile.OK_TOUCH_CREATE;
        MdlFile.deleteRecursively(target);
        int actual = MdlFile.createEmptyFile(target);
        assertEquals(expected, actual);
    }

    @Test
    public void createEmptyFile_引数で指定されたファイルが既に存在する場合OK_TOUCH_ALREADY_EXIST_1を返却すること() {
        String target = tempDir + File.separator + "CreateEmptyFile" + File.separator + "alreadyexist.txt";
        int expected = MdlFile.OK_TOUCH_ALREADY_EXIST;
        MdlFile.deleteRecursively(target);
        MdlFile.createEmptyFile(target);
        int actual = MdlFile.createEmptyFile(target);
        assertEquals(expected, actual);
    }

    @Test
    public void createEmptyFile_引数で指定されたファイルがディレクトリとして既に存在する場合NG_TOUCH_DIR_EXIST_13を返却すること() {
        String target = tempDir + File.separator + "CreateEmptyFile" + File.separator + "AlreadyExsitFolder";
        int expected = MdlFile.NG_TOUCH_DIR_EXIST;
        MdlFile.deleteRecursively(target);
        MdlFile.createDirectory(target);
        int actual = MdlFile.createEmptyFile(target);
        assertEquals(expected, actual);
    }

    @Test
    public void getPathType_引数が存在するディレクトリの場合は定数値PATH_IS_DIRECTORY_1を返却すること() {
        String target = tempDir + File.separator + "GetPathType" + File.separator + "001";
        int expected = MdlFile.PATH_IS_DIRECTORY;
        MdlFile.deleteRecursively(target);
        MdlFile.createDirectory(target);
        int actual = MdlFile.getPathType(target);
        assertEquals(expected, actual);
    }

    @Test
    public void getPathType_引数が存在するファイルの場合は定数値PATH_IS_FILEを返却すること() {
        String target = tempDir + File.separator + "GetPathType" + File.separator + "002.txt";
        int expected = MdlFile.PATH_IS_FILE;
        MdlFile.deleteRecursively(target);
        MdlFile.createEmptyFile(target);
        int actual = MdlFile.getPathType(target);
        assertEquals(expected, actual);
    }

    @Test
    public void getPathType_引数が空文字列の場合は定数値PATH_IS_NULLを返却すること() {
        int expected = MdlFile.PATH_IS_NULL;
        int actual = MdlFile.getPathType("");
        assertEquals(expected, actual);
    }

    @Test
    public void getPathType_引数が存在しないパスの場合は定数値PATH_NOT_FOUNDを返却すること() {
        int expected = MdlFile.PATH_NOT_FOUND;
        int actual = MdlFile.getPathType("C:\\NOT_EXIST_PATH_XYZ_12345");
        assertEquals(expected, actual);
    }

    @Test
    public void pathExists_引数が存在するディレクトリの場合はtrueを返却すること() {
        String target = tempDir + File.separator + "PathExists" + File.separator + "001";
        MdlFile.createDirectory(target);
        assertTrue(MdlFile.pathExists(target));
    }

    @Test
    public void pathExists_引数が存在するファイルの場合はtrueを返却すること() {
        String target = tempDir + File.separator + "PathExists" + File.separator + "002.txt";
        MdlFile.createEmptyFile(target);
        assertTrue(MdlFile.pathExists(target));
    }

    @Test
    public void pathExists_引数が空文字列の場合はfalseを返却すること() {
        assertFalse(MdlFile.pathExists(""));
    }

    @Test
    public void pathExists_引数が存在しないパスの場合はfalseを返却すること() {
        assertFalse(MdlFile.pathExists("C:\\NOT_EXIST_PATH_XYZ_12345"));
    }

    @Test
    public void removeTrailingPathSeparator_引数の文字列の最後に円マークが指定された場合は除去して返却すること() {
        String target = "C:\\DIR1\\DIR2\\";
        String expected = "C:\\DIR1\\DIR2";
        assertEquals(expected, MdlFile.removeTrailingPathSeparator(target));
    }

    @Test
    public void sanitizeFileName_引数に空白が含まれる場合はアンダースコアに置換して返却すること() {
        assertEquals("a_b", MdlFile.sanitizeFileName("a b"));
    }

    @Test
    public void sanitizeFileName_引数に円マークが含まれる場合はアンダースコアに置換して返却すること() {
        assertEquals("a_b", MdlFile.sanitizeFileName("a\\b"));
    }

    @Test
    public void sanitizeFileName_引数にスラッシュが含まれる場合はアンダースコアに置換して返却すること() {
        assertEquals("a_b", MdlFile.sanitizeFileName("a/b"));
    }

    @Test
    public void sanitizeFileName_引数にコロンが含まれる場合は除去して返却すること() {
        assertEquals("abc", MdlFile.sanitizeFileName("a:b:c"));
    }

    @Test
    public void sanitizeFileName_引数にセミコロンが含まれる場合は除去して返却すること() {
        assertEquals("abc", MdlFile.sanitizeFileName("a;b;c"));
    }

    @Test
    public void sanitizeFileName_引数にパイプが含まれる場合は除去して返却すること() {
        assertEquals("abc", MdlFile.sanitizeFileName("a|b|c"));
    }

    @Test
    public void sanitizeFileName_引数にカンマが含まれる場合は除去して返却すること() {
        assertEquals("abc", MdlFile.sanitizeFileName("a,b,c"));
    }

    @Test
    public void sanitizeFileName_引数にアスタリスクが含まれる場合は除去して返却すること() {
        assertEquals("abc", MdlFile.sanitizeFileName("a*b*c"));
    }

    @Test
    public void sanitizeFileName_引数にクエスチョンマークが含まれる場合は除去して返却すること() {
        assertEquals("abc", MdlFile.sanitizeFileName("a?b?c"));
    }

    @Test
    public void sanitizeFileName_引数に小なりが含まれる場合は除去して返却すること() {
        assertEquals("abc", MdlFile.sanitizeFileName("a<b<c"));
    }

    @Test
    public void sanitizeFileName_引数に大なりが含まれる場合は除去して返却すること() {
        assertEquals("abc", MdlFile.sanitizeFileName("a>b>c"));
    }

    @Test
    public void sanitizeFileName_引数にダブルクォーテーションが含まれる場合は除去して返却すること() {
        assertEquals("abc", MdlFile.sanitizeFileName("a\"b\"c"));
    }

    @Test
    public void writeFileAndReadFile_ファイル書込とファイル読込ができること() {
        String logDir = tempDir + File.separator + "WriteFile_ReadFile";
        String logPath = logDir + File.separator + "test.txt";
        String expected = "TESTTESTTEST";

        MdlFile.createDirectory(logDir);
        new File(logPath).delete();

        MdlFile.writeFile(logPath, expected);
        String actual = MdlFile.readFile(logPath, -1);
        assertEquals(expected, actual);

        new File(logPath).delete();
    }

    @Test
    public void 新設メソッド_getBaseName_trimPathSeparator_readFileToMap等が正しく動作すること() {
        assertEquals("test", MdlFile.getBaseName("C:\\dir\\test.txt"));
        assertEquals("C:\\dir", MdlFile.trimPathSeparator("C:\\dir\\"));
        assertTrue(MdlFile.isValidDirDateTime(tempDir, false, null, false, null));
        assertEquals("cmd C:\\file.txt", MdlFile.replacePathForCmd("cmd _PATH_", "C:\\file.txt", "file.txt", false, 0));
        assertNotNull(MdlFile.getSortedDirsInfo(tempDir, "*", false, MdlFile.SORT_BY_NAME, true));
    }
}
