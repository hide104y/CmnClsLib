using CmnClsLib.Module;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Module
{
    public class UnitTest_MdlFile
    {

        // ls ${Env:USERPROFILE}\AppData\Local\Temp\UnitTest
        private string _tempdir = Path.Combine(System.IO.Path.GetTempPath(), @"UnitTest", @"CmnClsLib", @"MdlFile");

        // --------------------------------------------------------------------
        // GetDirectoryPath()
        // --------------------------------------------------------------------
        [Fact]
        public void GetDirectoryPath_引数で指定したファイルのディレクトリパスを返却すること()
        {
            string target = Path.Combine(_tempdir, @"test.txt");
            string expected = _tempdir;
            Assert.Equal(expected, MdlFile.GetDirectoryPath(target));
        }

        // --------------------------------------------------------------------
        // GetFileNameWithoutExtension()
        // --------------------------------------------------------------------
        [Fact]
        public void GetFileNameWithoutExtension_引数で指定したファイルの拡張子無しファイル名を返却すること()
        {
            string target = Path.Combine(_tempdir, @"test.txt");
            string expected = @"test";
            Assert.Equal(expected, MdlFile.GetFileNameWithoutExtension(target));
        }

        // --------------------------------------------------------------------
        // GetFileName()
        // --------------------------------------------------------------------
        [Fact]
        public void GetFileName_引数で指定したファイルの拡張子有しファイル名を返却すること()
        {
            string target = Path.Combine(_tempdir, @"test.txt");
            string expected = @"test.txt";
            Assert.Equal(expected, MdlFile.GetFileName(target));
        }

        // --------------------------------------------------------------------
        // GetFileExtension()
        // --------------------------------------------------------------------
        [Fact]
        public void GetFileExtension_引数で指定したファイルの拡張子を返却すること()
        {
            string target = Path.Combine(_tempdir, @"test.txt");
            string expected = @"txt";
            Assert.Equal(expected, MdlFile.GetFileExtension(target));
        }

        // --------------------------------------------------------------------
        // CreateDirectory()
        // --------------------------------------------------------------------
        [Fact]
        public void CreateDirectory_引数が空文字列の場合は定数値NG_MKDIR_WRONG_ARGを返却すること()
        {
            string target = @"";
            int expected = MdlFile.NG_MKDIR_WRONG_ARG;
            int actual = MdlFile.CreateDirectory(target);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateDirectory_引数に指定したディレクトリの作成に成功した場合は定数値OK_MKDIR_CREATE_0を返却すること()
        {
            string target = Path.Combine(_tempdir, @"CreateDirectory\002");
            int expected = MdlFile.OK_MKDIR_CREATE;
            int actual = -1;
            bool isOk = false;
            isOk = MdlFile.DeleteRecursively(target);
            actual = MdlFile.CreateDirectory(target);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateDirectory_引数に既に存在するディレクトリを指定した場合は定数値OK_MKDIR_ALREADY_EXIT_1を返却すること()
        {
            string target = Path.Combine(_tempdir, @"CreateDirectory\003");
            int actual = -1;
            int expected = MdlFile.OK_MKDIR_ALREADY_EXIST;
            actual = MdlFile.CreateDirectory(target);
            actual = MdlFile.CreateDirectory(target);
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void CreateDirectory_引数に既に存在するファイルを指定した場合は定数値NG_MKDIR_FILE_EXIST_13を返却すること()
        {
            bool isOk = false;
            string target = Path.Combine(_tempdir, @"CreateDirectory\AlreadyExist.txt");
            int actual = -1;
            int expected = MdlFile.NG_MKDIR_FILE_EXIST;
            isOk = MdlFile.DeleteRecursively(target);
            actual = MdlFile.CreateEmptyFile(target);
            actual = MdlFile.CreateDirectory(target);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateDirectory_引数に指定したディレクトリの作成に失敗した場合は定数値NG_MKDIR_11を返却すること()
        {
            int actual = -1;
            string target = @"A:\NO_DRIVE";
            int expected = MdlFile.NG_MKDIR;
            actual = MdlFile.CreateDirectory(target);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // CreateEmptyFile()
        // --------------------------------------------------------------------
        [Fact]
        public void Touch_引数で指定されたファイルの作成に成功した場合OK_TOUCH_CREATE_0を返却すること()
        {
            int actual = -1;
            bool isOk = false;
            string target = Path.Combine(_tempdir, @"CreateEmptyFile\ok.txt");
            int expected = MdlFile.OK_TOUCH_CREATE;
            isOk = MdlFile.DeleteRecursively(target);
            actual = MdlFile.CreateEmptyFile(target);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Touch_引数で指定されたファイルが既に存在する場合OK_TOUCH_ALREADY_EXIST_1を返却すること()
        {
            int actual = -1;
            bool isOk = false;
            string target = Path.Combine(_tempdir, @"CreateEmptyFile\alreadyexist.txt");
            int expected = MdlFile.OK_TOUCH_ALREADY_EXIST;
            isOk = MdlFile.DeleteRecursively(target);
            actual = MdlFile.CreateEmptyFile(target);
            actual = MdlFile.CreateEmptyFile(target);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Touch_引数で指定されたファイルがディレクトリとして既に存在する場合NG_TOUCH_DIR_EXIST_13を返却すること()
        {
            int actual = -1;
            bool isOk = false;
            string target = Path.Combine(_tempdir, @"CreateEmptyFile\AlreadyExsitFolder");
            int expected = MdlFile.NG_TOUCH_DIR_EXIST;
            isOk = MdlFile.DeleteRecursively(target);
            actual = MdlFile.CreateDirectory(target);
            actual = MdlFile.CreateEmptyFile(target);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // GetPathType()
        // --------------------------------------------------------------------
        [Fact]
        public void GetPathType_引数が存在するディレクトリの場合は定数値PATH_IS_DIRECTORY_1を返却すること()
        {
            string target = Path.Combine(_tempdir, @"GetPathType\001");
            int actual = -1;
            bool isOk = false;
            int expected = MdlFile.PATH_IS_DIRECTORY;
            isOk = MdlFile.DeleteRecursively(target);
            actual = MdlFile.CreateDirectory(target);
            actual = MdlFile.GetPathType(target);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPathType_引数が存在するファイルの場合は定数値PATH_IS_FILEを返却すること()
        {
            int actual = -1;
            bool isOk = false;
            string target = Path.Combine(_tempdir, @"GetPathType\002.txt");
            int expected = MdlFile.PATH_IS_FILE;
            isOk = MdlFile.DeleteRecursively(target);
            actual = MdlFile.CreateEmptyFile(target);
            actual = MdlFile.GetPathType(target);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPathType_引数が空文字列の場合は定数値PATH_IS_NULLを返却すること()
        {
            int actual = -1;
            string target = @"";
            int expected = MdlFile.PATH_IS_NULL;
            actual = MdlFile.GetPathType(target);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPathType_引数が存在しないパスの場合は定数値PATH_NOT_FOUNDを返却すること()
        {
            int actual = -1;
            string target = @"C:\NOT_EXIT";
            int expected = MdlFile.PATH_NOT_FOUND;
            actual = MdlFile.GetPathType(target);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // PathExists()
        // --------------------------------------------------------------------
        [Fact]
        public void PathExists_引数が存在するディレクトリの場合はtrueを返却すること()
        {
            string target = Path.Combine(_tempdir, @"PathExists\001");
            MdlFile.CreateDirectory(target);
            bool actual = MdlFile.PathExists(target);
            Assert.True(actual);
        }

        [Fact]
        public void PathExists_引数が存在するファイルの場合はtrueを返却すること()
        {
            string target = Path.Combine(_tempdir, @"PathExists\002.txt");
            MdlFile.CreateEmptyFile(target);
            bool actual = MdlFile.PathExists(target);
            Assert.True(actual);
        }

        [Fact]
        public void PathExists_引数が空文字列の場合はfalseを返却すること()
        {
            string target = @"";
            bool actual = MdlFile.PathExists(target);
            Assert.False(actual);
        }

        [Fact]
        public void PathExists_引数が存在しないパスの場合はfalseを返却すること()
        {
            string target = @"C:\NOT_EXIT";
            bool actual = MdlFile.PathExists(target);
            Assert.False(actual);
        }

        // --------------------------------------------------------------------
        // RemoveTrailingPathSeparator()
        // --------------------------------------------------------------------
        [Fact]
        public void RemoveTrailingPathSeparator_引数の文字列の最後に円マークが指定された場合は除去して返却すること()
        {
            string target = @"C:\DIR1\DIR2\";
            string expected = @"C:\DIR1\DIR2";
            string actual = MdlFile.RemoveTrailingPathSeparator(target);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveTrailingPathSeparator_引数の文字列の最後にスラッシュが指定された場合は除去して返却すること()
        {
            string target = @"C:/DIR1/DIR2/";
            string expected = @"C:\DIR1\DIR2";
            string actual = MdlFile.RemoveTrailingPathSeparator(target);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // GetAbsolutePath()
        // --------------------------------------------------------------------
        [Fact]
        public void GetAbsolutePath_引数が相対パスの場合に絶対パスを返却すること()
        {
            // 先頭が一致すればドライブ文字が含まれる？
            string target = @"..\";
            string expected = System.IO.Directory.GetCurrentDirectory().Substring(0, 3);
            string actual = MdlFile.GetAbsolutePath(target).Substring(0, 3);
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // SanitizeFileName()
        // --------------------------------------------------------------------
        [Fact]
        public void SanitizeFileName_引数に空白が含まれる場合はアンダースコアに置換して返却すること()
        {
            string expected = @"a_b";
            string actual = MdlFile.SanitizeFileName("a b");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数に円マークが含まれる場合はアンダースコアに置換して返却すること()
        {
            string expected = @"a_b";
            string actual = MdlFile.SanitizeFileName(@"a\b");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数にスラッシュが含まれる場合はアンダースコアに置換して返却すること()
        {
            string expected = @"a_b";
            string actual = MdlFile.SanitizeFileName(@"a/b");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数にコロンが含まれる場合は除去して返却すること()
        {
            string expected = @"abc";
            string actual = MdlFile.SanitizeFileName(@"a:b:c");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数にセミコロンが含まれる場合は除去して返却すること()
        {
            string expected = @"abc";
            string actual = MdlFile.SanitizeFileName(@"a;b;c");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数にパイプが含まれる場合は除去して返却すること()
        {
            string expected = @"abc";
            string actual = MdlFile.SanitizeFileName(@"a|b|c");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数にカンマが含まれる場合は除去して返却すること()
        {
            string expected = @"abc";
            string actual = MdlFile.SanitizeFileName(@"a,b,c");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数にアスタリスクが含まれる場合は除去して返却すること()
        {
            string expected = @"abc";
            string actual = MdlFile.SanitizeFileName(@"a*b*c");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数にクエスチョンマークが含まれる場合は除去して返却すること()
        {
            string expected = @"abc";
            string actual = MdlFile.SanitizeFileName(@"a?b?c");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数に小なりが含まれる場合は除去して返却すること()
        {
            string expected = @"abc";
            string actual = MdlFile.SanitizeFileName(@"a<b<c");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数に大なりが含まれる場合は除去して返却すること()
        {
            string expected = @"abc";
            string actual = MdlFile.SanitizeFileName(@"a>b>c");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SanitizeFileName_引数にダブルクォーテーションが含まれる場合は除去して返却すること()
        {
            string expected = @"abc";
            string actual = MdlFile.SanitizeFileName("a\"b\"c");
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // WriteFile() && ReadFile
        // --------------------------------------------------------------------
        [Fact]
        public void WriteFileAndReadFile_ファイル書込とファイル読込ログファイルができること()
        {
            // 変数初期化
            string logDir = Path.Combine(_tempdir, @"WriteFile_ReadFile");
            string logPath = Path.Combine(logDir, @"test.txt");
            string expected = "TESTTESTTEST";
            // 事前作業
            MdlFile.CreateDirectory(logDir);
            if (MdlFile.GetPathType(logPath) == MdlFile.PATH_IS_FILE)
            {
                System.IO.File.Delete(logPath);
            }
            // 出力
            MdlFile.WriteFile(logPath, expected);
            // テスト
            string actual = MdlFile.ReadFile(logPath, -1);
            Assert.Equal(expected, actual);
            // 事後作業
            if (MdlFile.GetPathType(logPath) == MdlFile.PATH_IS_FILE)
            {
                System.IO.File.Delete(logPath);
            }
        }
        // --------------------------------------------------------------------
        // DeleteRecursively()
        // --------------------------------------------------------------------
        [Fact]
        public void rm_rf_引数で指定したディレクトリの削除に成功した場合はtrueを返却すること()
        {
            string target = @"C:\Tool\UnitTest\MdlFile";
            Assert.True(MdlFile.DeleteRecursively(target));
        }
    }
}
