using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Module
{
    /// <summary>
    /// ファイルおよびディレクトリに関する操作や判定機能を提供するユーティリティクラス。
    /// </summary>
    public static class MdlFile
    {
        // パス種別
        /// <summary>指定されない (Nullまたは空)</summary>
        public const int PATH_IS_NULL = -1;
        /// <summary>存在しない</summary>
        public const int PATH_NOT_FOUND = 0;
        /// <summary>ディレクトリ</summary>
        public const int PATH_IS_DIRECTORY = 1;
        /// <summary>ファイル</summary>
        public const int PATH_IS_FILE = 2;
        /// <summary>パス種別の自動判別指示</summary>
        public const int PATH_AUTO_DETECT = 9;

        // ディレクトリ作成
        /// <summary>作成成功</summary>
        public const int OK_MKDIR_CREATE = 0;
        /// <summary>既に存在（成功）</summary>
        public const int OK_MKDIR_ALREADY_EXIST = 1;
        /// <summary>判定成功</summary>
        public const int OK_MKDIR_HANTEI = 9;
        /// <summary>作成失敗</summary>
        public const int NG_MKDIR = 11;
        /// <summary>不正な引数（失敗）</summary>
        public const int NG_MKDIR_WRONG_ARG = 12;
        /// <summary>同名ファイル存在（失敗）</summary>
        public const int NG_MKDIR_FILE_EXIST = 13;

        // ファイル作成
        /// <summary>作成成功</summary>
        public const int OK_TOUCH_CREATE = 0;
        /// <summary>既に存在（成功）</summary>
        public const int OK_TOUCH_ALREADY_EXIST = 1;
        /// <summary>判定成功</summary>
        public const int OK_TOUCH_HANTEI = 9;
        /// <summary>作成失敗</summary>
        public const int NG_TOUCH = 11;
        /// <summary>不正な引数（失敗）</summary>
        public const int NG_TOUCH_WRONG_ARG = 12;
        /// <summary>同名ディレクトリ存在（失敗）</summary>
        public const int NG_TOUCH_DIR_EXIST = 13;

        // ソート
        /// <summary>ソートなし</summary>
        public const int SORT_BY_NONE = 0;
        /// <summary>名前でソート</summary>
        public const int SORT_BY_NAME = 1;
        /// <summary>作成日でソート</summary>
        public const int SORT_BY_CTIME = 2;
        /// <summary>更新日でソート</summary>
        public const int SORT_BY_MTIME = 3;

        #region Obsolete Legacy Compatibility Methods

        /// <summary>
        /// ディレクトリ情報を文字列形式で取得します。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="verbosity">詳細出力レベル。</param>
        /// <param name="encloseInQuotes">パスを引用符で囲むかどうか。</param>
        /// <returns>ディレクトリ情報の文字列。</returns>
        /// <example>
        /// <code>
        /// string info = MdlFile.GetDirectoryInfoStrLine(@"C:\data", 1, true);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetDirectoryInfoString()' を使用します。")]
        public static string GetDirectoryInfoStrLine(string path, int verbosity, bool encloseInQuotes)
        {
            return GetDirectoryInfoString(path, verbosity, encloseInQuotes);
        }

        /// <summary>
        /// 指定された日時が有効かどうかを確認します。
        /// </summary>
        /// <param name="path">対象の日時。</param>
        /// <param name="checkBefore">指定日以前かどうかを確認するか。</param>
        /// <param name="beforeDateTime">指定日以前の日時。</param>
        /// <param name="checkAfter">指定日以降かどうかを確認するか。</param>
        /// <param name="afterDateTime">指定日以降の日時。</param>
        /// <returns>有効な場合は true、それ以外の場合は false。</returns>
        /// <example>
        /// <code>
        /// bool valid = MdlFile.CheckValidDateTime(DateTime.Now, true, DateTime.Now.AddDays(1), false, DateTime.MinValue);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsValidDateTime()' を使用します。")]
        public static bool CheckValidDateTime(DateTime path, bool checkBefore, DateTime beforeDateTime, bool checkAfter, DateTime afterDateTime)
        {
            return IsValidDateTime(path, checkBefore, beforeDateTime, checkAfter, afterDateTime);
        }

        /// <summary>
        /// 指定されたパスの更新日時が有効かどうかを確認します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="checkBefore">指定日以前かどうかを確認するか。</param>
        /// <param name="beforeDateTime">指定日以前の日時。</param>
        /// <param name="checkAfter">指定日以降かどうかを確認するか。</param>
        /// <param name="afterDateTime">指定日以降の日時。</param>
        /// <returns>有効な場合は true、それ以外の場合は false。</returns>
        /// <example>
        /// <code>
        /// bool valid = MdlFile.CheckValidDateTime(@"C:\data\file.txt", true, DateTime.Now, false, DateTime.MinValue);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsValidDateTime()' を使用します。")]
        public static bool CheckValidDateTime(string path, bool checkBefore, DateTime beforeDateTime, bool checkAfter, DateTime afterDateTime)
        {
            return IsValidDateTime(path, checkBefore, beforeDateTime, checkAfter, afterDateTime);
        }

        /// <summary>
        /// ディレクトリの更新日時が有効かどうかを確認します。
        /// </summary>
        /// <param name="path">ディレクトリのパス。</param>
        /// <param name="checkBefore">指定日以前かどうかを確認するか。</param>
        /// <param name="beforeDateTime">指定日以前の日時。</param>
        /// <param name="checkAfter">指定日以降かどうかを確認するか。</param>
        /// <param name="afterDateTime">指定日以降の日時。</param>
        /// <returns>有効な場合は true、それ以外の場合は false。</returns>
        /// <example>
        /// <code>
        /// bool valid = MdlFile.CheckValidDateTimeOfDir(@"C:\data", true, DateTime.Now, false, DateTime.MinValue);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsValidDirectoryDateTime()' を使用します。")]
        public static bool CheckValidDateTimeOfDir(string path, bool checkBefore, DateTime beforeDateTime, bool checkAfter, DateTime afterDateTime)
        {
            return IsValidDirectoryDateTime(path, checkBefore, beforeDateTime, checkAfter, afterDateTime);
        }

        /// <summary>
        /// ファイルの更新日時が有効かどうかを確認します。
        /// </summary>
        /// <param name="path">ファイルのパス。</param>
        /// <param name="checkBefore">指定日以前かどうかを確認するか。</param>
        /// <param name="beforeDateTime">指定日以前の日時。</param>
        /// <param name="checkAfter">指定日以降かどうかを確認するか。</param>
        /// <param name="afterDateTime">指定日以降の日時。</param>
        /// <returns>有効な場合は true、それ以外の場合は false。</returns>
        /// <example>
        /// <code>
        /// bool valid = MdlFile.CheckValidDateTimeOfFile(@"C:\data\file.txt", true, DateTime.Now, false, DateTime.MinValue);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsValidFileDateTime()' を使用します。")]
        public static bool CheckValidDateTimeOfFile(string path, bool checkBefore, DateTime beforeDateTime, bool checkAfter, DateTime afterDateTime)
        {
            return IsValidFileDateTime(path, checkBefore, beforeDateTime, checkAfter, afterDateTime);
        }

        /// <summary>
        /// ファイル情報を文字列形式で取得します。
        /// </summary>
        /// <param name="path">ファイルのパス。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <param name="encloseInQuotes">パスを引用符で囲むかどうか。</param>
        /// <returns>ファイル情報の文字列。</returns>
        /// <example>
        /// <code>
        /// string info = MdlFile.GetFileInfoStrLine(@"C:\data\file.txt", 1, true);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetFileInfoString()' を使用します。")]
        public static string GetFileInfoStrLine(string path, int verbosity, bool encloseInQuotes)
        {
            return GetFileInfoString(path, verbosity, encloseInQuotes);
        }

        /// <summary>
        /// ファイル名から特定の不要文字を削除または置換します。
        /// </summary>
        /// <param name="originalFileName">元のファイル名。</param>
        /// <returns>サニタイズ後のファイル名。</returns>
        /// <example>
        /// <code>
        /// string safe = MdlFile.RemoveCharFromFilename("sample?.txt");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'SanitizeFileName()' を使用します。")]
        public static string RemoveCharFromFilename(string originalFileName)
        {
            return SanitizeFileName(originalFileName);
        }

        /// <summary>
        /// 指定されたファイルパスからディレクトリパスを取得します。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <returns>ディレクトリパス。取得できない場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string dir = MdlFile.GetDirectoryPathOfTargetFile(@"C:\folder\file.txt");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetDirectoryPath()' を使用します。")]
        public static string GetDirectoryPathOfTargetFile(string filePath)
        {
            return GetDirectoryPath(filePath);
        }

        /// <summary>
        /// 指定されたファイルパスからファイル名（拡張子を含む）を取得します。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <returns>ファイル名。取得できない場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string name = MdlFile.GetFileNameWithExtension(@"C:\folder\file.txt");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetFileName()' を使用します。")]
        public static string GetFileNameWithExtension(string filePath)
        {
            return GetFileName(filePath);
        }

        /// <summary>
        /// 指定されたパスの存在状態を確認します。
        /// </summary>
        /// <param name="path">確認対象のパス。</param>
        /// <returns>パスの存在状態を示す整数値。</returns>
        /// <example>
        /// <code>
        /// int type = MdlFile.CheckExistPath(@"C:\folder");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetPathType()' を使用します。")]
        public static int CheckExistPath(string? path)
        {
            return GetPathType(path);
        }

        /// <summary>
        /// 指定されたパスが存在するかどうかを確認します。
        /// </summary>
        /// <param name="path">確認対象のパス。</param>
        /// <returns>パスが存在する場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool exists = MdlFile.IsExistPath(@"C:\folder\file.txt");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'PathExists()' を使用します。")]
        public static bool IsExistPath(string path)
        {
            return PathExists(path);
        }

        /// <summary>
        /// 指定されたパスの末尾のパス区切り文字を削除します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <returns>末尾の区切り文字が除去されたパス。</returns>
        /// <example>
        /// <code>
        /// string clean = MdlFile.RmLastPathSeparator(@"C:\folder\");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'RemoveTrailingPathSeparator()' を使用します。")]
        public static string RmLastPathSeparator(string path)
        {
            return RemoveTrailingPathSeparator(path);
        }

        /// <summary>
        /// 指定されたパスにファイルを作成（タッチ）します。
        /// </summary>
        /// <param name="path">作成するファイルのパス。</param>
        /// <returns>操作結果のステータスコード。</returns>
        /// <example>
        /// <code>
        /// int res = MdlFile.Touch(@"C:\folder\newfile.txt");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'CreateEmptyFile()' を使用します。")]
        public static int Touch(string path)
        {
            return CreateEmptyFile(path);
        }

        /// <summary>
        /// 指定されたパスにディレクトリを作成します。
        /// </summary>
        /// <param name="path">作成するディレクトリのパス。</param>
        /// <returns>操作結果のステータスコード。</returns>
        /// <example>
        /// <code>
        /// int res = MdlFile.Mkdir(@"C:\folder\newdir");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'CreateDirectory()' を使用します。")]
        public static int Mkdir(string path)
        {
            return CreateDirectory(path);
        }

        /// <summary>
        /// 指定されたパスのファイルまたはディレクトリを再帰的に削除します。
        /// </summary>
        /// <param name="path">削除対象のパス。</param>
        /// <returns>成功した場合は true、失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.Rm_rf(@"C:\folder\temp");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'DeleteRecursively()' を使用します。")]
        public static bool Rm_rf(string path)
        {
            return DeleteRecursively(path, 0);
        }

        /// <summary>
        /// 指定されたディレクトリを再帰的に削除します。
        /// </summary>
        /// <param name="directoryInfo">削除対象のディレクトリ情報。</param>
        /// <returns>成功した場合は true、失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.Rm_rf(new DirectoryInfo(@"C:\folder\temp"));
        /// </code>
        /// </example>
        [Obsolete("代わりに 'DeleteRecursively()' を使用します。")]
        public static bool Rm_rf(DirectoryInfo directoryInfo)
        {
            return DeleteRecursively(directoryInfo, 0);
        }

        /// <summary>
        /// 指定されたパスのファイルまたはディレクトリを再帰的に削除します。
        /// </summary>
        /// <param name="path">削除対象のパス。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>成功した場合は true、失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.Rm_rf(@"C:\folder\temp", 1);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'DeleteRecursively()' を使用します。")]
        public static bool Rm_rf(string path, int verbosity)
        {
            return DeleteRecursively(path, verbosity);
        }

        /// <summary>
        /// 指定されたディレクトリを再帰的に削除します。
        /// </summary>
        /// <param name="directoryInfo">削除対象のディレクトリ情報。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>成功した場合は true、失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.Rm_rf(new DirectoryInfo(@"C:\folder\temp"), 1);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'DeleteRecursively()' を使用します。")]
        public static bool Rm_rf(DirectoryInfo directoryInfo, int verbosity)
        {
            return DeleteRecursively(directoryInfo, verbosity);
        }

        /// <summary>
        /// 指定されたディレクトリを削除します（シンボリックリンク処理考慮）。
        /// </summary>
        /// <param name="directoryInfo">対象ディレクトリ情報。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>成功した場合は true、失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.RmDirs(new DirectoryInfo(@"C:\folder\temp"), 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'DeleteDirectory()' を使用します。")]
        public static bool RmDirs(DirectoryInfo directoryInfo, int verbosity)
        {
            return DeleteDirectory(directoryInfo, verbosity);
        }

        /// <summary>
        /// 指定されたファイルを削除します。
        /// </summary>
        /// <param name="fileInfo">対象ファイル情報。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>成功した場合は true、失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.RmFile(new FileInfo(@"C:\folder\file.txt"), 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'DeleteFile()' を使用します。")]
        public static bool RmFile(FileInfo fileInfo, int verbosity)
        {
            return DeleteFile(fileInfo, verbosity);
        }

        /// <summary>
        /// 指定されたパス内の空のディレクトリを再帰的に削除します。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>成功した場合は true、失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.RmEmptyDirectory(@"C:\folder", 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'DeleteEmptyDirectories()' を使用します。")]
        public static bool RmEmptyDirectory(string path, int verbosity)
        {
            return DeleteEmptyDirectories(path, verbosity);
        }

        /// <summary>
        /// 指定された '=' 区切りテキストファイルを読み込み、辞書オブジェクトに格納します。
        /// </summary>
        /// <param name="filePath">ファイルパス。</param>
        /// <param name="encoding">文字エンコーディング。</param>
        /// <returns>キーと値のペア。</returns>
        /// <example>
        /// <code>
        /// var dic = MdlFile.FileToDicByEqual(@"C:\config.txt", Encoding.UTF8);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ReadFileToDictionary()' を使用します。")]
        public static Dictionary<string, string> FileToDicByEqual(string filePath, Encoding encoding)
        {
            return ReadFileToDictionary(filePath, encoding);
        }

        /// <summary>
        /// 指定された '=' 区切りテキストファイルを自動エンコーディング検出で読み込み、辞書オブジェクトに格納します。
        /// </summary>
        /// <param name="filePath">ファイルパス。</param>
        /// <returns>キーと値のペア。</returns>
        /// <example>
        /// <code>
        /// var dic = MdlFile.FileToDicByEqual(@"C:\config.txt");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ReadFileToDictionary()' を使用します。")]
        public static Dictionary<string, string> FileToDicByEqual(string filePath)
        {
            return ReadFileToDictionary(filePath);
        }

        /// <summary>
        /// ディレクトリのタイムスタンプが異なるかどうかを確認します。
        /// </summary>
        /// <param name="sourceDir">ソースディレクトリ。</param>
        /// <param name="targetDir">ターゲットディレクトリ。</param>
        /// <param name="timeRange">許容秒数。</param>
        /// <param name="mode">判定モード。</param>
        /// <returns>差分がある場合は true。</returns>
        /// <example>
        /// <code>
        /// bool diff = MdlFile.IsDiffTimestampOfDir(dir1, dir2, 1.0, 2);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsDirectoryTimestampDifferent()' を使用します。")]
        public static bool IsDiffTimestampOfDir(DirectoryInfo sourceDir, DirectoryInfo targetDir, double timeRange, int mode)
        {
            return IsDirectoryTimestampDifferent(sourceDir, targetDir, timeRange, mode);
        }

        /// <summary>
        /// ファイルのタイムスタンプが異なるかどうかを確認します。
        /// </summary>
        /// <param name="sourceFile">ソースファイル。</param>
        /// <param name="targetFile">ターゲットファイル。</param>
        /// <param name="timeRange">許容秒数。</param>
        /// <param name="mode">判定モード。</param>
        /// <returns>差分がある場合は true。</returns>
        /// <example>
        /// <code>
        /// bool diff = MdlFile.IsDiffTimestampOfFile(file1, file2, 1.0, 2);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsFileTimestampDifferent()' を使用します。")]
        public static bool IsDiffTimestampOfFile(FileInfo sourceFile, FileInfo targetFile, double timeRange, int mode)
        {
            return IsFileTimestampDifferent(sourceFile, targetFile, timeRange, mode);
        }

        /// <summary>
        /// ディレクトリの属性・モードを変更します。
        /// </summary>
        /// <param name="path">ディレクトリパス。</param>
        /// <param name="mode">属性モード文字列。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.ChModeDir(@"C:\folder", "W");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ChangeDirectoryAttributes()' を使用します。")]
        public static bool ChModeDir(string path, string mode)
        {
            return ChangeDirectoryAttributes(path, mode);
        }

        /// <summary>
        /// ディレクトリの属性・モードを変更します。
        /// </summary>
        /// <param name="directoryInfo">ディレクトリ情報。</param>
        /// <param name="mode">属性モード文字列。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.ChModeDir(new DirectoryInfo(@"C:\folder"), "W");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ChangeDirectoryAttributes()' を使用します。")]
        public static bool ChModeDir(DirectoryInfo directoryInfo, string mode)
        {
            return ChangeDirectoryAttributes(directoryInfo, mode);
        }

        /// <summary>
        /// ファイルの属性・モードを変更します。
        /// </summary>
        /// <param name="path">ファイルパス。</param>
        /// <param name="mode">属性モード文字列。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.ChModeFile(@"C:\file.txt", "W");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ChangeFileAttributes()' を使用します。")]
        public static bool ChModeFile(string path, string mode)
        {
            return ChangeFileAttributes(path, mode);
        }

        /// <summary>
        /// ファイルの属性・モードを変更します。
        /// </summary>
        /// <param name="fileInfo">ファイル情報。</param>
        /// <param name="mode">属性モード文字列。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.ChModeFile(new FileInfo(@"C:\file.txt"), "W");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ChangeFileAttributes()' を使用します。")]
        public static bool ChModeFile(FileInfo fileInfo, string mode)
        {
            return ChangeFileAttributes(fileInfo, mode);
        }

        /// <summary>
        /// ファイルのSHA1ハッシュを取得します。
        /// </summary>
        /// <param name="path">ファイルパス。</param>
        /// <returns>ハッシュ文字列。</returns>
        /// <example>
        /// <code>
        /// string hash = MdlFile.GetSha1Hash(@"C:\file.bin");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ComputeSha1Hash()' を使用します。")]
        public static string GetSha1Hash(string path)
        {
            return ComputeSha1Hash(path);
        }

        /// <summary>
        /// パスが有効かどうかを判定し、状態コードを返します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="includeBaseName">包含判定時にファイル名のみを使用するか。</param>
        /// <param name="excludeBaseName">除外判定時にファイル名のみを使用するか。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <param name="isOrCondition">包含条件をOR評価するか。</param>
        /// <param name="debugLevel">デバッグ出力レベル。</param>
        /// <returns>評価コード（1: 適合、2: 除外対象、0: 未該当）。</returns>
        /// <example>
        /// <code>
        /// int code = MdlFile.IntPathEffective(@"C:\data\file.txt", true, true, new List&lt;string&gt;(), new List&lt;string&gt;(), false, 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'EvaluatePathFilterCode()' を使用します。")]
        public static int IntPathEffective(string path, bool includeBaseName, bool excludeBaseName, List<string> includePatterns, List<string> excludePatterns, bool isOrCondition, int debugLevel)
        {
            return EvaluatePathFilterCode(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, isOrCondition, debugLevel);
        }

        /// <summary>
        /// パスがフィルタに適合するか判定します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="includeBaseName">包含判定時にファイル名のみを使用するか。</param>
        /// <param name="excludeBaseName">除外判定時にファイル名のみを使用するか。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <param name="isOrCondition">包含条件をOR評価するか。</param>
        /// <param name="debugLevel">デバッグ出力レベル。</param>
        /// <returns>適合する場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool matched = MdlFile.IsPathEffective(@"C:\data\file.txt", true, true, new List&lt;string&gt;(), new List&lt;string&gt;(), false, 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsPathFilterMatched()' を使用します。")]
        public static bool IsPathEffective(string path, bool includeBaseName, bool excludeBaseName, List<string> includePatterns, List<string> excludePatterns, bool isOrCondition, int debugLevel)
        {
            return IsPathFilterMatched(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, isOrCondition, debugLevel);
        }

        /// <summary>
        /// パスがフィルタに適合するか判定します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="includeBaseName">包含判定時にファイル名のみを使用するか。</param>
        /// <param name="excludeBaseName">除外判定時にファイル名のみを使用するか。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <param name="debugLevel">デバッグ出力レベル。</param>
        /// <returns>適合する場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool matched = MdlFile.IsPathEffective(@"C:\data\file.txt", true, true, new List&lt;string&gt;(), new List&lt;string&gt;(), 0);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsPathFilterMatched()' を使用します。")]
        public static bool IsPathEffective(string path, bool includeBaseName, bool excludeBaseName, List<string> includePatterns, List<string> excludePatterns, int debugLevel)
        {
            return IsPathFilterMatched(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, debugLevel);
        }

        /// <summary>
        /// パスがフィルタに適合するか判定します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="includeBaseName">包含判定時にファイル名のみを使用するか。</param>
        /// <param name="excludeBaseName">除外判定時にファイル名のみを使用するか。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <param name="isOrCondition">包含条件をOR評価するか。</param>
        /// <returns>適合する場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool matched = MdlFile.IsPathEffective(@"C:\data\file.txt", true, true, new List&lt;string&gt;(), new List&lt;string&gt;(), false);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsPathFilterMatched()' を使用します。")]
        public static bool IsPathEffective(string path, bool includeBaseName, bool excludeBaseName, List<string> includePatterns, List<string> excludePatterns, bool isOrCondition)
        {
            return IsPathFilterMatched(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, isOrCondition);
        }

        /// <summary>
        /// パスがフィルタに適合するか判定します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="includeBaseName">包含判定時にファイル名のみを使用するか。</param>
        /// <param name="excludeBaseName">除外判定時にファイル名のみを使用するか。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <returns>適合する場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool matched = MdlFile.IsPathEffective(@"C:\data\file.txt", true, true, new List&lt;string&gt;(), new List&lt;string&gt;());
        /// </code>
        /// </example>
        [Obsolete("代わりに 'IsPathFilterMatched()' を使用します。")]
        public static bool IsPathEffective(string path, bool includeBaseName, bool excludeBaseName, List<string> includePatterns, List<string> excludePatterns)
        {
            return IsPathFilterMatched(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns);
        }

        /// <summary>
        /// フィルターフラグを統合して取得します。
        /// </summary>
        /// <param name="previousEffective">親階層の判定結果フラグ。</param>
        /// <param name="currentEffective">自階層の判定結果フラグ。</param>
        /// <param name="isOrCondition">包含条件をOR評価するか。</param>
        /// <param name="isIncludeHitRecursive">包含ヒットを再帰継承するか。</param>
        /// <param name="isExcludeHitRecursive">除外ヒットを再帰継承するか。</param>
        /// <returns>統合されたフィルターフラグ値。</returns>
        /// <example>
        /// <code>
        /// int flag = MdlFile.GetEffectiveFlag(1, 1, false, true, true);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'CombineFilterFlags()' を使用します。")]
        public static int GetEffectiveFlag(int previousEffective, int currentEffective, bool isOrCondition, bool isIncludeHitRecursive, bool isExcludeHitRecursive)
        {
            return CombineFilterFlags(previousEffective, currentEffective, isOrCondition, isIncludeHitRecursive, isExcludeHitRecursive);
        }

        #endregion

        #region Core Path & File Info Utilities

        /// <summary>
        /// 指定されたファイルパスから親ディレクトリのパスを取得します。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <returns>親ディレクトリのパス。取得できない場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string dirPath = MdlFile.GetDirectoryPath(@"C:\data\sample.txt");
        /// // dirPath は "C:\data"
        /// </code>
        /// </example>
        public static string GetDirectoryPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "";
            return Path.GetDirectoryName(filePath) ?? "";
        }

        /// <summary>
        /// 指定されたファイルパスから拡張子を除いたファイル名を取得します。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <returns>拡張子を除いたファイル名。指定パスが空の場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string fileName = MdlFile.GetFileNameWithoutExtension(@"C:\data\sample.txt");
        /// // fileName は "sample"
        /// </code>
        /// </example>
        public static string GetFileNameWithoutExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "";
            return Path.GetFileNameWithoutExtension(filePath);
        }

        /// <summary>
        /// 指定されたファイルパスからファイル名（拡張子含む）を取得します。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <returns>ファイル名。指定パスが空の場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string fileName = MdlFile.GetFileName(@"C:\data\sample.txt");
        /// // fileName は "sample.txt"
        /// </code>
        /// </example>
        public static string GetFileName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "";
            return Path.GetFileName(filePath);
        }

        /// <summary>
        /// 指定されたファイルパスから先頭のドットを除いた拡張子を取得します。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <returns>拡張子（例: "txt"）。指定パスが空の場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string ext = MdlFile.GetFileExtension(@"C:\data\sample.txt");
        /// // ext は "txt"
        /// </code>
        /// </example>
        public static string GetFileExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "";
            return Path.GetExtension(filePath).TrimStart('.');
        }

        /// <summary>
        /// 指定されたパスの存在種別を取得します。
        /// </summary>
        /// <param name="path">確認対象のパス。</param>
        /// <returns>
        /// パスの状態を示す整数値。
        /// <list type="bullet">
        ///   <item><see cref="PATH_IS_NULL"/> (-1): パスが null または空文字列</item>
        ///   <item><see cref="PATH_NOT_FOUND"/> (0): パスが存在しない</item>
        ///   <item><see cref="PATH_IS_DIRECTORY"/> (1): ディレクトリとして存在</item>
        ///   <item><see cref="PATH_IS_FILE"/> (2): ファイルとして存在</item>
        /// </list>
        /// </returns>
        /// <example>
        /// <code>
        /// int status = MdlFile.GetPathType(@"C:\Windows");
        /// if (status == MdlFile.PATH_IS_DIRECTORY)
        /// {
        ///     Console.WriteLine("ディレクトリが存在します。");
        /// }
        /// </code>
        /// </example>
        public static int GetPathType(string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return PATH_IS_NULL;
            }
            if (Directory.Exists(path))
            {
                return PATH_IS_DIRECTORY;
            }
            if (File.Exists(path))
            {
                return PATH_IS_FILE;
            }
            return PATH_NOT_FOUND;
        }

        /// <summary>
        /// 指定されたパスがファイルまたはディレクトリとして存在するかどうかを判定します。
        /// </summary>
        /// <param name="path">確認対象のパス。</param>
        /// <returns>ファイルまたはディレクトリが存在する場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool exists = MdlFile.PathExists(@"C:\Windows\System32");
        /// </code>
        /// </example>
        public static bool PathExists(string path)
        {
            int pathType = GetPathType(path);
            return pathType == PATH_IS_DIRECTORY || pathType == PATH_IS_FILE;
        }

        /// <summary>
        /// 指定されたパスが隠しファイルまたは隠しディレクトリかどうかを確認します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <returns>隠しファイルまたは隠しディレクトリの場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool isHidden = MdlFile.IsHidden(@"C:\Users\User\AppData");
        /// </code>
        /// </example>
        public static bool IsHidden(string path)
        {
            return GetPathType(path) switch
            {
                PATH_IS_DIRECTORY => (new DirectoryInfo(path).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden,
                PATH_IS_FILE => (new FileInfo(path).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden,
                _ => false,
            };
        }

        /// <summary>
        /// 指定されたパスがシンボリックリンク（再解析ポイント）かどうかを確認します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <returns>シンボリックリンクの場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool isSymlink = MdlFile.IsSymlink(@"C:\path\to\link");
        /// </code>
        /// </example>
        public static bool IsSymlink(string path)
        {
            return GetPathType(path) switch
            {
                PATH_IS_DIRECTORY => (new DirectoryInfo(path).Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint,
                PATH_IS_FILE => (new FileInfo(path).Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint,
                _ => false,
            };
        }

        /// <summary>
        /// 指定されたファイルが他のプロセスによってロックされている（独占開かれている）か判定します。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <returns>ロックされている場合は true、利用可能な場合は false。</returns>
        /// <example>
        /// <code>
        /// if (MdlFile.IsFileLocked(@"C:\data\log.txt"))
        /// {
        ///     Console.WriteLine("ファイルは使用中です。");
        /// }
        /// </code>
        /// </example>
        public static bool IsFileLocked(string filePath)
        {
            if (!File.Exists(filePath)) return false;
            try
            {
                using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return false;
            }
            catch (IOException)
            {
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 指定されたパスの末尾のディレクトリ区切り文字を削除します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <returns>末尾の区切り文字を除去したパス。</returns>
        /// <example>
        /// <code>
        /// string normalized = MdlFile.RemoveTrailingPathSeparator(@"C:\folder\subfolder\");
        /// // normalized は "C:\folder\subfolder"
        /// </code>
        /// </example>
        public static string RemoveTrailingPathSeparator(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            string normalized = path.Replace('/', '\\');
            return Path.TrimEndingDirectorySeparator(normalized);
        }

        /// <summary>
        /// 指定されたパスの絶対パスを取得します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <returns>絶対パス。取得に失敗した場合は元のパスまたは空文字列。</returns>
        /// <example>
        /// <code>
        /// string absPath = MdlFile.GetAbsolutePath(@".\relative\path.txt");
        /// </code>
        /// </example>
        public static string GetAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            try
            {
                return Path.GetFullPath(path);
            }
            catch
            {
                return path;
            }
        }

        /// <summary>
        /// 基準パスからのターゲットパスへの相対パスを取得します。
        /// </summary>
        /// <param name="basePath">基準となるパス。</param>
        /// <param name="targetPath">ターゲットとなるパス。</param>
        /// <returns>相対パス。計算できない場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string relPath = MdlFile.GetRelativePath(@"C:\base\dir", @"C:\base\dir\sub\file.txt");
        /// // relPath は "sub\file.txt"
        /// </code>
        /// </example>
        public static string GetRelativePath(string basePath, string targetPath)
        {
            string absBasePath = GetAbsolutePath(basePath);
            string absTargetPath = GetAbsolutePath(targetPath);

            if (string.IsNullOrEmpty(absBasePath) || string.IsNullOrEmpty(absTargetPath)) return "";
            if (Regex.IsMatch(targetPath, @"^\\")) return targetPath;

            try
            {
                absBasePath = absBasePath.Replace("%", "%25");
                absTargetPath = absTargetPath.Replace("%", "%25");

                Uri baseUri = new Uri(absBasePath);
                Uri targetUri = new Uri(absTargetPath);
                Uri relativeUri = baseUri.MakeRelativeUri(targetUri);

                string relativePath = Uri.UnescapeDataString(relativeUri.ToString());
                relativePath = relativePath.Replace("%25", "%").Replace('/', '\\');
                return relativePath;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region Information Formatting & Date Validation

        /// <summary>
        /// ディレクトリの基本情報をフォーマット済み文字列として取得します。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="verbosity">詳細出力レベル (0以上で更新日時を含む)。</param>
        /// <param name="encloseInQuotes">パスをダブルクォーテーションで囲むかどうか。</param>
        /// <returns>フォーマットされたディレクトリ情報文字列。</returns>
        /// <example>
        /// <code>
        /// string info = MdlFile.GetDirectoryInfoString(@"C:\data", 1, true);
        /// // 出力例: "[D][2026/08/01 12:00:00] \"C:\data\""
        /// </code>
        /// </example>
        public static string GetDirectoryInfoString(string path, int verbosity, bool encloseInQuotes)
        {
            string tempPath = encloseInQuotes ? $"\"{path}\"" : path;
            if (verbosity < 0) return tempPath;

            string dateStr = "";
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                dateStr = $"[{MdlDate.GetFormattedDate(fileInfo.LastWriteTime, "yyyy/MM/dd HH:mm:ss")}]";
            }
            catch { }

            return $"[D]{dateStr} {tempPath}";
        }

        /// <summary>
        /// 指定された日時が、指定された前後日時の範囲内に収まっているかを判定します。
        /// </summary>
        /// <param name="targetDateTime">判定対象の日時。</param>
        /// <param name="checkBefore">指定以前かの判定を行うフラグ。</param>
        /// <param name="beforeDateTime">以前の比較対象日時。</param>
        /// <param name="checkAfter">指定以降かの判定を行うフラグ。</param>
        /// <param name="afterDateTime">以降の比較対象日時。</param>
        /// <returns>有効な範囲内であれば true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool isValid = MdlFile.IsValidDateTime(DateTime.Now, true, DateTime.Now.AddDays(1), true, DateTime.Now.AddDays(-1));
        /// </code>
        /// </example>
        public static bool IsValidDateTime(DateTime targetDateTime, bool checkBefore, DateTime beforeDateTime, bool checkAfter, DateTime afterDateTime)
        {
            if (checkBefore && MdlDate.CompareDateTime(targetDateTime, beforeDateTime, 0) > 0)
            {
                return false;
            }
            if (checkAfter && MdlDate.CompareDateTime(targetDateTime, afterDateTime, 0) < 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 指定されたパスのファイルまたはディレクトリの更新日時が、指定範囲内かを判定します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="checkBefore">指定以前かの判定を行うフラグ。</param>
        /// <param name="beforeDateTime">以前の比較対象日時。</param>
        /// <param name="checkAfter">指定以降かの判定を行うフラグ。</param>
        /// <param name="afterDateTime">以降の比較対象日時。</param>
        /// <returns>有効な範囲内であれば true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool valid = MdlFile.IsValidDateTime(@"C:\data\log.txt", false, DateTime.Now, true, DateTime.Today);
        /// </code>
        /// </example>
        public static bool IsValidDateTime(string path, bool checkBefore, DateTime beforeDateTime, bool checkAfter, DateTime afterDateTime)
        {
            try
            {
                return GetPathType(path) switch
                {
                    PATH_IS_DIRECTORY => IsValidDateTime(new DirectoryInfo(path).LastWriteTime, checkBefore, beforeDateTime, checkAfter, afterDateTime),
                    PATH_IS_FILE => IsValidDateTime(new FileInfo(path).LastWriteTime, checkBefore, beforeDateTime, checkAfter, afterDateTime),
                    _ => false,
                };
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ディレクトリの更新日時が指定範囲内かを判定します。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="checkBefore">指定以前かの判定を行うフラグ。</param>
        /// <param name="beforeDateTime">以前の比較対象日時。</param>
        /// <param name="checkAfter">指定以降かの判定を行うフラグ。</param>
        /// <param name="afterDateTime">以降の比較対象日時。</param>
        /// <returns>有効な範囲内であれば true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool valid = MdlFile.IsValidDirectoryDateTime(@"C:\data", false, DateTime.Now, true, DateTime.Today);
        /// </code>
        /// </example>
        public static bool IsValidDirectoryDateTime(string path, bool checkBefore, DateTime beforeDateTime, bool checkAfter, DateTime afterDateTime)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                return IsValidDateTime(dirInfo.LastWriteTime, checkBefore, beforeDateTime, checkAfter, afterDateTime);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ファイルの更新日時が指定範囲内かを判定します。
        /// </summary>
        /// <param name="path">対象ファイルのパス。</param>
        /// <param name="checkBefore">指定以前かの判定を行うフラグ。</param>
        /// <param name="beforeDateTime">以前の比較対象日時。</param>
        /// <param name="checkAfter">指定以降かの判定を行うフラグ。</param>
        /// <param name="afterDateTime">以降の比較対象日時。</param>
        /// <returns>有効な範囲内であれば true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool valid = MdlFile.IsValidFileDateTime(@"C:\data\file.txt", false, DateTime.Now, true, DateTime.Today);
        /// </code>
        /// </example>
        public static bool IsValidFileDateTime(string path, bool checkBefore, DateTime beforeDateTime, bool checkAfter, DateTime afterDateTime)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                return IsValidDateTime(fileInfo.LastWriteTime, checkBefore, beforeDateTime, checkAfter, afterDateTime);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ファイルの基本情報をフォーマット済み文字列として取得します。
        /// </summary>
        /// <param name="path">対象ファイルのパス。</param>
        /// <param name="verbosity">詳細出力レベル (1以上で日時、2以上でファイルサイズを追加)。</param>
        /// <param name="encloseInQuotes">パスをダブルクォーテーションで囲むかどうか。</param>
        /// <returns>フォーマットされたファイル情報文字列。</returns>
        /// <example>
        /// <code>
        /// string info = MdlFile.GetFileInfoString(@"C:\data\sample.txt", 2, true);
        /// // 出力例: "[F][2026/08/01 12:00:00][10.5 KB] \"C:\data\sample.txt\""
        /// </code>
        /// </example>
        public static string GetFileInfoString(string path, int verbosity, bool encloseInQuotes)
        {
            string tempPath = encloseInQuotes ? $"\"{path}\"" : path;
            if (verbosity < 0) return tempPath;

            string line = "[F]";
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                line += $"[{MdlDate.GetFormattedDate(fileInfo.LastWriteTime, "yyyy/MM/dd HH:mm:ss")}]";
                if (verbosity > 1)
                {
                    line += $"[{MdlUtil.GetHumanReadableBytesRight(fileInfo.Length)}]";
                }
            }
            catch { }

            return $"{line} {tempPath}";
        }

        #endregion

        #region Command Exec Path Replacement

        /// <summary>
        /// 外部コマンド実行用に、プレースホルダーを含む文字列を置換します。
        /// </summary>
        /// <param name="target">置換対象テンプレート文字列。</param>
        /// <param name="fullPath">対象ファイルのフルパス。</param>
        /// <param name="basePath">ベースディレクトリパス。</param>
        /// <param name="relativePath">相対パス。</param>
        /// <param name="encloseInQuotes">結果全体をダブルクォーテーションで囲むかどうか。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <param name="currentDateTime">日時プレースホルダー置換用の基準日時。</param>
        /// <returns>置換完了後のコマンド文字列。</returns>
        /// <example>
        /// <code>
        /// string cmd = MdlFile.ReplacePathForCmdExec("echo {} _FILENAME_", @"C:\data\test.txt", @"C:\data", "test.txt", false, 0, DateTime.Now);
        /// </code>
        /// </example>
        public static string ReplacePathForCmdExec(string target, string fullPath, string basePath, string relativePath, bool encloseInQuotes, int verbosity, DateTime currentDateTime)
        {
            string relPath = string.IsNullOrEmpty(relativePath) ? @"." : relativePath;
            string relDir = GetDirectoryPath(relPath);
            string tempPath = target;

            // ファイルパス
            tempPath = tempPath.Replace("{}", fullPath)
                               .Replace("_PATH_", fullPath)
                               .Replace("_RELPATH_", relPath)
                               .Replace("_RELFLAT_", relPath.Replace(@"\", "_").Replace(@"/", "_"));
            // ベースディレクトリパス
            tempPath = tempPath.Replace("_BASEDIR_", basePath);
            // 親ディレクトリパス
            tempPath = tempPath.Replace("_DIR_", GetDirectoryPath(fullPath))
                               .Replace("_RELDIR_", string.IsNullOrEmpty(relDir) ? @"." : relDir)
                               .Replace("_RELDIRFLAT_", relDir.Replace(@"\", "_").Replace(@"/", "_"));
            // ファイル名
            tempPath = tempPath.Replace("_FILENAME_", GetFileName(fullPath))
                               .Replace("_BASENAME_", GetFileNameWithoutExtension(fullPath));
            // 環境変数
            tempPath = tempPath.Replace("_USERDOMAIN_", Environment.GetEnvironmentVariable("USERDOMAIN"))
                               .Replace("_COMPUTERNAME_", Environment.MachineName)
                               .Replace("_USERNAME_", Environment.UserName);
            // その他
            tempPath = tempPath.Replace(@"%%", @"%");
            tempPath = MdlDate.ReplaceStringWithDateTime(tempPath, currentDateTime);

            return encloseInQuotes ? $" \"{tempPath}\"" : tempPath;
        }

        /// <summary>
        /// 外部コマンド実行用にプレースホルダーを含む文字列を置換します（現在日時を使用）。
        /// </summary>
        /// <param name="target">置換対象テンプレート文字列。</param>
        /// <param name="fullPath">対象ファイルのフルパス。</param>
        /// <param name="basePath">ベースディレクトリパス。</param>
        /// <param name="relativePath">相対パス。</param>
        /// <param name="encloseInQuotes">結果全体をダブルクォーテーションで囲むかどうか。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>置換完了後のコマンド文字列。</returns>
        /// <example>
        /// <code>
        /// string cmd = MdlFile.ReplacePathForCmdExec("echo {} _FILENAME_", @"C:\data\test.txt", @"C:\data", "test.txt", false, 0);
        /// </code>
        /// </example>
        public static string ReplacePathForCmdExec(string target, string fullPath, string basePath, string relativePath, bool encloseInQuotes, int verbosity)
        {
            return ReplacePathForCmdExec(target, fullPath, basePath, relativePath, encloseInQuotes, verbosity, DateTime.Now);
        }

        /// <summary>
        /// 外部コマンド実行用にプレースホルダーを含む文字列を置換します（空のベースパスを使用）。
        /// </summary>
        /// <param name="target">置換対象テンプレート文字列。</param>
        /// <param name="fullPath">対象ファイルのフルパス。</param>
        /// <param name="relativePath">相対パス。</param>
        /// <param name="encloseInQuotes">結果全体をダブルクォーテーションで囲むかどうか。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <param name="currentDateTime">日時プレースホルダー置換用の基準日時。</param>
        /// <returns>置換完了後のコマンド文字列。</returns>
        /// <example>
        /// <code>
        /// string cmd = MdlFile.ReplacePathForCmdExec("echo {}", @"C:\data\test.txt", "test.txt", false, 0, DateTime.Now);
        /// </code>
        /// </example>
        public static string ReplacePathForCmdExec(string target, string fullPath, string relativePath, bool encloseInQuotes, int verbosity, DateTime currentDateTime)
        {
            return ReplacePathForCmdExec(target, fullPath, "", relativePath, encloseInQuotes, verbosity, currentDateTime);
        }

        /// <summary>
        /// 外部コマンド実行用にプレースホルダーを含む文字列を置換します（空のベースパス、現在日時を使用）。
        /// </summary>
        /// <param name="target">置換対象テンプレート文字列。</param>
        /// <param name="fullPath">対象ファイルのフルパス。</param>
        /// <param name="relativePath">相対パス。</param>
        /// <param name="encloseInQuotes">結果全体をダブルクォーテーションで囲むかどうか。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>置換完了後のコマンド文字列。</returns>
        /// <example>
        /// <code>
        /// string cmd = MdlFile.ReplacePathForCmdExec("echo {}", @"C:\data\test.txt", "test.txt", false, 0);
        /// </code>
        /// </example>
        public static string ReplacePathForCmdExec(string target, string fullPath, string relativePath, bool encloseInQuotes, int verbosity)
        {
            return ReplacePathForCmdExec(target, fullPath, "", relativePath, encloseInQuotes, verbosity, DateTime.Now);
        }

        #endregion

        #region File & Directory Creation / Deletion

        /// <summary>
        /// 指定されたパスに空ファイルを作成（タッチ）します。必要な親ディレクトリは自動作成されます。
        /// </summary>
        /// <param name="path">作成するファイルのパス。</param>
        /// <returns>操作結果のステータスコード（<see cref="OK_TOUCH_CREATE"/>, <see cref="OK_TOUCH_ALREADY_EXIST"/>, <see cref="NG_TOUCH_DIR_EXIST"/> など）。</returns>
        /// <example>
        /// <code>
        /// int status = MdlFile.CreateEmptyFile(@"C:\temp\newfile.txt");
        /// if (status == MdlFile.OK_TOUCH_CREATE)
        /// {
        ///     Console.WriteLine("ファイルが新規作成されました。");
        /// }
        /// </code>
        /// </example>
        public static int CreateEmptyFile(string path)
        {
            int result = OK_TOUCH_CREATE;
            path = GetAbsolutePath(path);

            switch (GetPathType(path))
            {
                case PATH_IS_DIRECTORY:
                    return NG_TOUCH_DIR_EXIST;
                case PATH_IS_FILE:
                    return OK_TOUCH_ALREADY_EXIST;
                case PATH_IS_NULL:
                    return NG_TOUCH_WRONG_ARG;
            }

            string directoryPath = GetDirectoryPath(path);
            switch (GetPathType(directoryPath))
            {
                case PATH_IS_FILE:
                    return NG_TOUCH;
                case PATH_NOT_FOUND:
                    CreateDirectory(directoryPath);
                    break;
            }

            try
            {
                File.Create(path).Close();
            }
            catch
            {
                result = NG_TOUCH;
            }

            return result;
        }

        /// <summary>
        /// 指定されたパスにディレクトリを作成します。
        /// </summary>
        /// <param name="path">作成するディレクトリのパス。</param>
        /// <returns>操作結果のステータスコード（<see cref="OK_MKDIR_CREATE"/>, <see cref="OK_MKDIR_ALREADY_EXIST"/>, <see cref="NG_MKDIR_FILE_EXIST"/> など）。</returns>
        /// <example>
        /// <code>
        /// int status = MdlFile.CreateDirectory(@"C:\temp\new_folder");
        /// </code>
        /// </example>
        public static int CreateDirectory(string path)
        {
            path = GetAbsolutePath(path);
            path = RemoveTrailingPathSeparator(path);

            switch (GetPathType(path))
            {
                case PATH_IS_NULL:
                    return NG_MKDIR_WRONG_ARG;
                case PATH_IS_DIRECTORY:
                    return OK_MKDIR_ALREADY_EXIST;
                case PATH_IS_FILE:
                    return NG_MKDIR_FILE_EXIST;
            }

            try
            {
                Directory.CreateDirectory(path);
            }
            catch { }

            return GetPathType(path) == PATH_IS_DIRECTORY ? OK_MKDIR_CREATE : NG_MKDIR;
        }

        /// <summary>
        /// 指定されたパスのファイルまたはディレクトリを再帰的に削除します。
        /// </summary>
        /// <param name="path">削除対象のパス。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>削除が成功した場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool success = MdlFile.DeleteRecursively(@"C:\temp\work_dir", 1);
        /// </code>
        /// </example>
        public static bool DeleteRecursively(string path, int verbosity = 0)
        {
            return GetPathType(path) switch
            {
                PATH_IS_DIRECTORY => DeleteDirectory(new DirectoryInfo(path), verbosity),
                PATH_IS_FILE => DeleteFile(new FileInfo(path), verbosity),
                _ => true,
            };
        }

        /// <summary>
        /// 指定されたディレクトリを再帰的に削除します。
        /// </summary>
        /// <param name="directoryInfo">削除対象のディレクトリ情報。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>削除が成功した場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// DirectoryInfo dirInfo = new DirectoryInfo(@"C:\temp\work");
        /// bool success = MdlFile.DeleteRecursively(dirInfo, 0);
        /// </code>
        /// </example>
        public static bool DeleteRecursively(DirectoryInfo directoryInfo, int verbosity = 0)
        {
            bool isSuccess = true;

            // 全ファイルの読み取り専用属性を解除
            foreach (FileInfo fileInfo in directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                ChangeFileAttributes(fileInfo, "W");
            }
            // サブディレクトリの属性解除・削除
            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (!DeleteDirectory(subDirectoryInfo, verbosity))
                {
                    isSuccess = false;
                }
            }
            // 自身の属性解除
            ChangeDirectoryAttributes(directoryInfo, "W");

            try
            {
                directoryInfo.Delete(true);
            }
            catch (Exception ex)
            {
                if (verbosity > 0) Console.WriteLine($" => FAILED TO DELETE DIRECTORY({directoryInfo.FullName})：EXCEPTION：{ex.Message}");
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 指定されたディレクトリを削除します（シンボリックリンクの場合は階層下を削除せずリンク本体のみ削除）。
        /// </summary>
        /// <param name="directoryInfo">対象のディレクトリ情報。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>成功した場合は true、失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool success = MdlFile.DeleteDirectory(new DirectoryInfo(@"C:\temp\folder"), 0);
        /// </code>
        /// </example>
        public static bool DeleteDirectory(DirectoryInfo directoryInfo, int verbosity = 0)
        {
            if (IsSymlink(directoryInfo.FullName))
            {
                try
                {
                    ChangeDirectoryAttributes(directoryInfo, "W");
                    directoryInfo.Delete(false);
                    return true;
                }
                catch (Exception ex)
                {
                    if (verbosity > 0) Console.WriteLine($" => FAILED TO DELETE SYMLINK({directoryInfo.FullName})：EXCEPTION：{ex.Message}");
                    return false;
                }
            }
            return DeleteRecursively(directoryInfo, verbosity);
        }

        /// <summary>
        /// 指定されたファイルを削除します（読み取り専用属性を自動で解除して削除）。
        /// </summary>
        /// <param name="fileInfo">対象のファイル情報。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>成功した場合は true、失敗した場合は false。</returns>
        /// <example>
        /// <code>
        /// bool success = MdlFile.DeleteFile(new FileInfo(@"C:\temp\file.txt"), 0);
        /// </code>
        /// </example>
        public static bool DeleteFile(FileInfo fileInfo, int verbosity = 0)
        {
            ChangeFileAttributes(fileInfo, "W");
            try
            {
                fileInfo.Delete();
                return true;
            }
            catch (Exception ex)
            {
                if (verbosity > 0) Console.WriteLine($" => FAILED TO DELETE FILE({fileInfo.FullName})：EXCEPTION：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 指定されたパス配下の空のディレクトリを再帰的に検索・削除します。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="verbosity">詳細レベル。</param>
        /// <returns>すべての空ディレクトリ削除が成功した場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool success = MdlFile.DeleteEmptyDirectories(@"C:\temp\parent", 0);
        /// </code>
        /// </example>
        public static bool DeleteEmptyDirectories(string path, int verbosity = 0)
        {
            bool isSuccess = true;
            try
            {
                if (!Directory.Exists(path)) return true;
                if (IsEmptyDirectory(path)) return DeleteRecursively(path, verbosity);

                foreach (string subDirPath in Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly))
                {
                    if (!DeleteEmptyDirectories(subDirPath, verbosity)) isSuccess = false;
                    if (!Directory.Exists(subDirPath)) continue;
                    if (IsEmptyDirectory(subDirPath))
                    {
                        if (!DeleteRecursively(subDirPath, verbosity)) isSuccess = false;
                    }
                    if (!Directory.Exists(path)) return true;
                }
            }
            catch { }
            return isSuccess;
        }

        /// <summary>
        /// 指定されたディレクトリが空（ファイルもサブディレクトリも含まない）かどうか判定します。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <returns>空の場合は true、それ以外（存在しない場合も含む）は false。</returns>
        /// <example>
        /// <code>
        /// bool empty = MdlFile.IsEmptyDirectory(@"C:\temp\empty_dir");
        /// </code>
        /// </example>
        public static bool IsEmptyDirectory(string path)
        {
            if (!Directory.Exists(path)) return false;
            try
            {
                return !Directory.EnumerateFileSystemEntries(path).Any();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ファイル名として不適切な記号（'/', '\', ':', ';', '|', ',', '*', '?', '&lt;', '&gt;', '"'）を除去または置換して安全なファイル名を生成します。
        /// </summary>
        /// <param name="originalFileName">元のファイル名。</param>
        /// <returns>サニタイズ後のファイル名。</returns>
        /// <example>
        /// <code>
        /// string safeName = MdlFile.SanitizeFileName("report:2026/08/01?.txt");
        /// // safeName は "report2026_08_01.txt"
        /// </code>
        /// </example>
        public static string SanitizeFileName(string originalFileName)
        {
            if (string.IsNullOrEmpty(originalFileName)) return "";
            string sanitized = originalFileName.Replace(" ", "_")
                                               .Replace("\\", "_")
                                               .Replace("/", "_");
            char[] removeChars = [':', ';', '|', ',', '*', '?', '<', '>', '"'];
            foreach (char c in removeChars)
            {
                sanitized = sanitized.Replace(c.ToString(), "");
            }
            return sanitized;
        }

        #endregion

        #region File Read & Write Operations

        /// <summary>
        /// 指定されたファイルにメッセージ文字列を書き込みます（MS932等のエンコーディング対応）。
        /// </summary>
        /// <param name="filePath">書き込み先のファイルパス。</param>
        /// <param name="message">書き込むメッセージ文字列。</param>
        /// <param name="fileMode">ファイルのオープンモード。</param>
        /// <param name="fileAccess">アクセス権限。</param>
        /// <param name="fileShare">共有モード。</param>
        /// <returns>なし。</returns>
        /// <example>
        /// <code>
        /// MdlFile.WriteFile(@"C:\log.txt", "Hello World", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        /// </code>
        /// </example>
        public static void WriteFile(string filePath, string message, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using FileStream stream = new FileStream(filePath, fileMode, fileAccess, fileShare);
            using StreamWriter writer = new StreamWriter(stream, Encoding.GetEncoding(932));
            writer.AutoFlush = true;
            writer.WriteLine(message);
            writer.Flush();
        }

        /// <summary>
        /// 指定されたファイルに複数行のメッセージを書き込みます。
        /// </summary>
        /// <param name="filePath">書き込み先のファイルパス。</param>
        /// <param name="message">書き込むメッセージ行のリスト。</param>
        /// <param name="fileMode">ファイルのオープンモード。</param>
        /// <param name="fileAccess">アクセス権限。</param>
        /// <param name="fileShare">共有モード。</param>
        /// <returns>なし。</returns>
        /// <example>
        /// <code>
        /// MdlFile.WriteFile(@"C:\log.txt", new List&lt;string&gt; { "Line 1", "Line 2" }, FileMode.Create, FileAccess.Write, FileShare.Read);
        /// </code>
        /// </example>
        public static void WriteFile(string filePath, List<string> message, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using FileStream stream = new FileStream(filePath, fileMode, fileAccess, fileShare);
            using StreamWriter writer = new StreamWriter(stream, Encoding.GetEncoding(932));
            writer.AutoFlush = true;
            foreach (string line in message)
            {
                writer.WriteLine(line);
            }
            writer.Flush();
        }

        /// <summary>
        /// 指定されたファイルにメッセージを書き込みます（ファイルアクセス権限は ReadWrite）。
        /// </summary>
        /// <param name="filePath">書き込み先のファイルパス。</param>
        /// <param name="message">書き込むメッセージ文字列。</param>
        /// <param name="fileMode">ファイルのオープンモード。</param>
        /// <param name="fileShare">共有モード。</param>
        /// <returns>なし。</returns>
        /// <example>
        /// <code>
        /// MdlFile.WriteFile(@"C:\log.txt", "Hello World", FileMode.Create, FileShare.ReadWrite);
        /// </code>
        /// </example>
        public static void WriteFile(string filePath, string message, FileMode fileMode, FileShare fileShare)
        {
            WriteFile(filePath, message, fileMode, FileAccess.ReadWrite, fileShare);
        }

        /// <summary>
        /// 指定されたファイルにメッセージを追記書き込みします（FileMode.Append）。
        /// </summary>
        /// <param name="filePath">書き込み先のファイルパス。</param>
        /// <param name="message">書き込むメッセージ文字列。</param>
        /// <returns>なし。</returns>
        /// <example>
        /// <code>
        /// MdlFile.WriteFile(@"C:\log.txt", "Log entry");
        /// </code>
        /// </example>
        public static void WriteFile(string filePath, string message)
        {
            WriteFile(filePath, message, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }

        /// <summary>
        /// 指定されたファイルから最大バイト数制限付きで文字列を読み込みます。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <param name="maxBytes">読み込む最大バイト数（0以下の場合は無制限）。</param>
        /// <param name="encoding">使用する文字エンコーディング。</param>
        /// <returns>読み込んだファイル内容文字列。</returns>
        /// <example>
        /// <code>
        /// string text = MdlFile.ReadFile(@"C:\data.txt", 1024, Encoding.UTF8);
        /// </code>
        /// </example>
        public static string ReadFile(string filePath, int maxBytes, Encoding encoding)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StringBuilder output = new StringBuilder();
            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader reader = new StreamReader(stream, encoding ?? Encoding.Default);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (maxBytes > 0 && output.Length + line.Length > maxBytes)
                {
                    break;
                }
                output.AppendLine(line);
            }
            return output.ToString().Trim();
        }

        /// <summary>
        /// 指定されたファイルから自動エンコーディング検出で文字列を読み込みます。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <param name="maxBytes">読み込む最大バイト数（0以下の場合は無制限）。</param>
        /// <returns>読み込んだファイル内容文字列。</returns>
        /// <example>
        /// <code>
        /// string text = MdlFile.ReadFile(@"C:\data.txt", 0);
        /// </code>
        /// </example>
        public static string ReadFile(string filePath, int maxBytes)
        {
            return ReadFile(filePath, maxBytes, DetectFileEncoding(filePath) ?? Encoding.Default);
        }

        /// <summary>
        /// Key=Value 形式のテキストファイルを読み込み、Dictionary に展開します。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <param name="encoding">文字エンコーディング。</param>
        /// <returns>Key と Value の辞書オブジェクト。</returns>
        /// <example>
        /// <code>
        /// Dictionary&lt;string, string&gt; settings = MdlFile.ReadFileToDictionary(@"C:\config.ini", Encoding.UTF8);
        /// string server = settings["ServerName"];
        /// </code>
        /// </example>
        public static Dictionary<string, string> ReadFileToDictionary(string filePath, Encoding encoding)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            Regex regex = new Regex(@"^\s*(?<KEY>[^#=]*)\s*=\s*(?<VAL>.*)\s*$");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader reader = new StreamReader(stream, encoding ?? Encoding.Default);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                Match match = regex.Match(line);
                if (match.Success)
                {
                    string key = MdlUtil.TrimQuotes(match.Groups["KEY"].Value);
                    string value = MdlUtil.TrimQuotes(match.Groups["VAL"].Value);
                    dictionary[key] = value;
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Key=Value 形式のテキストファイルを自動エンコーディング検出で読み込み、Dictionary に展開します。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <returns>Key と Value の辞書オブジェクト。</returns>
        /// <example>
        /// <code>
        /// var settings = MdlFile.ReadFileToDictionary(@"C:\config.ini");
        /// </code>
        /// </example>
        public static Dictionary<string, string> ReadFileToDictionary(string filePath)
        {
            return ReadFileToDictionary(filePath, DetectFileEncoding(filePath) ?? Encoding.Default);
        }

        #endregion

        #region File & Directory Date Operations

        /// <summary>
        /// 指定されたパスのファイルまたはディレクトリの日時を一括設定します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="date">設定する日時文字列。</param>
        /// <param name="mode">設定モード（1: 作成日, 2: 更新日, 4: アクセス日などの組み合わせ）。</param>
        /// <param name="pathType">パス種別（自動判別の場合は <see cref="PATH_AUTO_DETECT"/>）。</param>
        /// <param name="validateDate">日時文字列の検証を行うかどうか。</param>
        /// <param name="force">変更の有無に関わらず強制設定するか。</param>
        /// <param name="execute">実際に処理を実行するか。</param>
        /// <returns>処理が成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.SetDate(@"C:\data\file.txt", "2026-08-01 10:00:00", 7, MdlFile.PATH_AUTO_DETECT, true, true, true);
        /// </code>
        /// </example>
        public static bool SetDate(string path, string date, int mode, int pathType, bool validateDate, bool force, bool execute)
        {
            SetDateMain(path, date, mode, pathType, validateDate, force, execute);
            return true;
        }

        /// <summary>
        /// 指定されたパスの日時を設定します（execute=true）。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="date">設定する日時文字列。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="pathType">パス種別。</param>
        /// <param name="validateDate">日時文字列の検証を行うかどうか。</param>
        /// <param name="force">変更の有無に関わらず強制設定するか。</param>
        /// <returns>処理が成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.SetDate(@"C:\data\file.txt", "2026-08-01 10:00:00", 7, MdlFile.PATH_AUTO_DETECT, true, true);
        /// </code>
        /// </example>
        public static bool SetDate(string path, string date, int mode, int pathType, bool validateDate, bool force)
        {
            return SetDate(path, date, mode, pathType, validateDate, force, true);
        }

        /// <summary>
        /// 指定されたパスの日時を設定します（force=true, execute=true）。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="date">設定する日時文字列。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="pathType">パス種別。</param>
        /// <param name="validateDate">日時文字列の検証を行うかどうか。</param>
        /// <returns>処理が成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.SetDate(@"C:\data\file.txt", "2026-08-01 10:00:00", 7, MdlFile.PATH_AUTO_DETECT, true);
        /// </code>
        /// </example>
        public static bool SetDate(string path, string date, int mode, int pathType, bool validateDate)
        {
            return SetDate(path, date, mode, pathType, validateDate, true, true);
        }

        /// <summary>
        /// 指定されたパスのファイルまたはディレクトリの日時設定のメイン処理です。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="date">設定する日時文字列。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="pathType">パス種別。</param>
        /// <param name="validateDate">日時文字列の検証を行うかどうか。</param>
        /// <param name="force">変更の有無に関わらず強制設定するか。</param>
        /// <param name="execute">実際に処理を実行するか。</param>
        /// <returns>処理結果ステータスコード。</returns>
        /// <example>
        /// <code>
        /// int code = MdlFile.SetDateMain(@"C:\data\file.txt", "2026-08-01 10:00:00", 7, MdlFile.PATH_AUTO_DETECT, true, true, true);
        /// </code>
        /// </example>
        public static int SetDateMain(string path, string date, int mode, int pathType, bool validateDate, bool force, bool execute)
        {
            DateTime dateTime;

            if (PATH_AUTO_DETECT == pathType)
            {
                pathType = GetPathType(path);
                if (PATH_NOT_FOUND == pathType || PATH_IS_NULL == pathType) return -1;
            }

            if (validateDate)
            {
                string? validDate = MdlDate.ValidateAndFormatDate(date);
                if (string.IsNullOrEmpty(validDate)) return -1;
                dateTime = DateTime.Parse(validDate);
            }
            else
            {
                dateTime = DateTime.Parse(date);
            }

            return pathType switch
            {
                PATH_IS_FILE => SetDateToFileMainInternal(path, dateTime, mode, force, execute),
                PATH_IS_DIRECTORY => SetDateToDirMainInternal(path, dateTime, mode, force, execute),
                _ => 0,
            };
        }

        /// <summary>
        /// ファイルの日時設定を実行するための内部メソッドです。
        /// </summary>
        /// <param name="path">ファイルパス。</param>
        /// <param name="dateTime">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="force">強制フラグ。</param>
        /// <param name="execute">実行フラグ。</param>
        /// <returns>処理結果コード。</returns>
        /// <example>
        /// <code>
        /// // 内部利用のため直接呼び出しは避けてください
        /// </code>
        /// </example>
        private static int SetDateToFileMainInternal(string path, DateTime dateTime, int mode, bool force, bool execute)
        {
            if (execute) ChangeFileAttributes(path, "W");
            return SetDateToFileMain(path, dateTime, mode, force, execute);
        }

        /// <summary>
        /// ディレクトリの日時設定を実行するための内部メソッドです。
        /// </summary>
        /// <param name="path">ディレクトリパス。</param>
        /// <param name="dateTime">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="force">強制フラグ。</param>
        /// <param name="execute">実行フラグ。</param>
        /// <returns>処理結果コード。</returns>
        /// <example>
        /// <code>
        /// // 内部利用のため直接呼び出しは避けてください
        /// </code>
        /// </example>
        private static int SetDateToDirMainInternal(string path, DateTime dateTime, int mode, bool force, bool execute)
        {
            if (execute) ChangeDirectoryAttributes(path, "W");
            return SetDateToDirMain(path, dateTime, mode, force, execute);
        }

        /// <summary>
        /// 指定されたパスの日時を設定します（execute=true）。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="date">設定する日時文字列。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="pathType">パス種別。</param>
        /// <param name="validateDate">日時文字列の検証を行うかどうか。</param>
        /// <param name="force">変更の有無に関わらず強制設定するか。</param>
        /// <returns>処理結果ステータスコード。</returns>
        /// <example>
        /// <code>
        /// int code = MdlFile.SetDateMain(@"C:\data\file.txt", "2026-08-01 10:00:00", 7, MdlFile.PATH_AUTO_DETECT, true, true);
        /// </code>
        /// </example>
        public static int SetDateMain(string path, string date, int mode, int pathType, bool validateDate, bool force)
        {
            return SetDateMain(path, date, mode, pathType, validateDate, force, true);
        }

        /// <summary>
        /// ディレクトリの日時（作成日・更新日・アクセス日）を設定します。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="date">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="force">強制設定フラグ。</param>
        /// <param name="execute">実行フラグ。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.SetDateToDir(@"C:\data", DateTime.Now, 7, true, true);
        /// </code>
        /// </example>
        public static bool SetDateToDir(string path, DateTime date, int mode, bool force, bool execute)
        {
            SetDateToDirMain(path, date, mode, force, execute);
            return true;
        }

        /// <summary>
        /// ディレクトリの日時を設定します（execute=true）。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="date">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="force">強制設定フラグ。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.SetDateToDir(@"C:\data", DateTime.Now, 7, true);
        /// </code>
        /// </example>
        public static bool SetDateToDir(string path, DateTime date, int mode, bool force)
        {
            return SetDateToDir(path, date, mode, force, true);
        }

        /// <summary>
        /// ディレクトリの日時を設定します（force=true, execute=true）。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="date">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.SetDateToDir(@"C:\data", DateTime.Now, 7);
        /// </code>
        /// </example>
        public static bool SetDateToDir(string path, DateTime date, int mode)
        {
            return SetDateToDir(path, date, mode, true, true);
        }

        /// <summary>
        /// ディレクトリの日時設定のメイン処理です。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="date">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="force">強制設定フラグ。</param>
        /// <param name="execute">実行フラグ。</param>
        /// <returns>処理結果コード。</returns>
        /// <example>
        /// <code>
        /// int code = MdlFile.SetDateToDirMain(@"C:\data", DateTime.Now, 7, true, true);
        /// </code>
        /// </example>
        public static int SetDateToDirMain(string path, DateTime date, int mode, bool force, bool execute)
        {
            bool setCreate = mode is 1 or 3 or 5 or 7;
            bool setModify = mode is 2 or 3 or 6 or 7;
            bool setAccess = mode is 4 or 5 or 6 or 7;
            if (mode is not (1 or 2 or 3 or 4 or 5 or 6))
            {
                setCreate = setModify = setAccess = true;
            }

            int resultCode = 0;

            if (setCreate && (force || ShouldSetDate(date, Directory.GetCreationTime(path))))
            {
                if (execute) Directory.SetCreationTime(path, date);
                resultCode += 100;
            }

            if (setModify && (force || ShouldSetDate(date, Directory.GetLastWriteTime(path))))
            {
                if (execute) Directory.SetLastWriteTime(path, date);
                resultCode += 10;
            }

            if (setAccess && (force || ShouldSetDate(date, Directory.GetLastAccessTime(path))))
            {
                if (execute) Directory.SetLastAccessTime(path, date);
                resultCode += 1;
            }

            return resultCode;
        }

        /// <summary>
        /// ディレクトリの日時を設定します（execute=true）。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="date">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="force">強制設定フラグ。</param>
        /// <returns>処理結果コード。</returns>
        /// <example>
        /// <code>
        /// int code = MdlFile.SetDateToDirMain(@"C:\data", DateTime.Now, 7, true);
        /// </code>
        /// </example>
        public static int SetDateToDirMain(string path, DateTime date, int mode, bool force)
        {
            return SetDateToDirMain(path, date, mode, force, true);
        }

        /// <summary>
        /// 日時設定が必要かどうか判定する内部補助メソッドです。
        /// </summary>
        /// <param name="newDate">変更後の日時。</param>
        /// <param name="currentDate">現在の対象日時。</param>
        /// <returns>設定変更が必要な場合は true。</returns>
        /// <example>
        /// <code>
        /// // 内部利用のため直接呼び出しは避けてください
        /// </code>
        /// </example>
        private static bool ShouldSetDate(DateTime newDate, DateTime currentDate)
        {
            try
            {
                return newDate.CompareTo(currentDate) != 0;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// ファイルの日時（作成日・更新日・アクセス日）を設定します。
        /// </summary>
        /// <param name="path">対象ファイルのパス。</param>
        /// <param name="date">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="force">強制設定フラグ。</param>
        /// <param name="execute">実行フラグ。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.SetDateToFile(@"C:\data\file.txt", DateTime.Now, 7, true, true);
        /// </code>
        /// </example>
        public static bool SetDateToFile(string path, DateTime date, int mode, bool force, bool execute)
        {
            SetDateToFileMain(path, date, mode, force, execute);
            return true;
        }

        /// <summary>
        /// ファイルの日時を設定します（execute=true）。
        /// </summary>
        /// <param name="path">対象ファイルのパス。</param>
        /// <param name="date">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="force">強制設定フラグ。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.SetDateToFile(@"C:\data\file.txt", DateTime.Now, 7, true);
        /// </code>
        /// </example>
        public static bool SetDateToFile(string path, DateTime date, int mode, bool force)
        {
            return SetDateToFile(path, date, mode, force, true);
        }

        /// <summary>
        /// ファイルの日時を設定します（force=true, execute=true）。
        /// </summary>
        /// <param name="path">対象ファイルのパス。</param>
        /// <param name="date">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.SetDateToFile(@"C:\data\file.txt", DateTime.Now, 7);
        /// </code>
        /// </example>
        public static bool SetDateToFile(string path, DateTime date, int mode)
        {
            return SetDateToFile(path, date, mode, true, true);
        }

        /// <summary>
        /// ファイルの日時設定のメイン処理です。
        /// </summary>
        /// <param name="path">対象ファイルのパス。</param>
        /// <param name="date">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="force">強制設定フラグ。</param>
        /// <param name="execute">実行フラグ。</param>
        /// <returns>処理結果コード。</returns>
        /// <example>
        /// <code>
        /// int code = MdlFile.SetDateToFileMain(@"C:\data\file.txt", DateTime.Now, 7, true, true);
        /// </code>
        /// </example>
        public static int SetDateToFileMain(string path, DateTime date, int mode, bool force, bool execute)
        {
            bool setCreate = mode is 1 or 3 or 5 or 7;
            bool setModify = mode is 2 or 3 or 6 or 7;
            bool setAccess = mode is 4 or 5 or 6 or 7;
            if (mode is not (1 or 2 or 3 or 4 or 5 or 6))
            {
                setCreate = setModify = setAccess = true;
            }

            int resultCode = 0;

            if (setCreate && (force || ShouldSetDate(date, File.GetCreationTime(path))))
            {
                if (execute) File.SetCreationTime(path, date);
                resultCode += 100;
            }

            if (setModify && (force || ShouldSetDate(date, File.GetLastWriteTime(path))))
            {
                if (execute) File.SetLastWriteTime(path, date);
                resultCode += 10;
            }

            if (setAccess && (force || ShouldSetDate(date, File.GetLastAccessTime(path))))
            {
                if (execute) File.SetLastAccessTime(path, date);
                resultCode += 1;
            }

            return resultCode;
        }

        /// <summary>
        /// ファイルの日時を設定します（execute=true）。
        /// </summary>
        /// <param name="path">対象ファイルのパス。</param>
        /// <param name="date">設定日時。</param>
        /// <param name="mode">設定モード。</param>
        /// <param name="force">強制設定フラグ。</param>
        /// <returns>処理結果コード。</returns>
        /// <example>
        /// <code>
        /// int code = MdlFile.SetDateToFileMain(@"C:\data\file.txt", DateTime.Now, 7, true);
        /// </code>
        /// </example>
        public static int SetDateToFileMain(string path, DateTime date, int mode, bool force)
        {
            return SetDateToFileMain(path, date, mode, force, true);
        }

        /// <summary>
        /// 2つのディレクトリのタイムスタンプに指定許容範囲（秒）以上の差異があるか確認します。
        /// </summary>
        /// <param name="sourceDir">比較元のディレクトリ情報。</param>
        /// <param name="targetDir">比較先のディレクトリ情報。</param>
        /// <param name="timeRange">許容誤差（秒）。</param>
        /// <param name="mode">1: 作成日, 2: 更新日, 3: 作成日または更新日。</param>
        /// <returns>タイムスタンプが異なる場合は true、一致していれば false。</returns>
        /// <example>
        /// <code>
        /// bool diff = MdlFile.IsDirectoryTimestampDifferent(dir1, dir2, 2.0, 2);
        /// </code>
        /// </example>
        public static bool IsDirectoryTimestampDifferent(DirectoryInfo sourceDir, DirectoryInfo targetDir, double timeRange, int mode)
        {
            return mode switch
            {
                1 => MdlDate.CompareDateTime(sourceDir.CreationTime, targetDir.CreationTime, timeRange) != 0,
                2 => MdlDate.CompareDateTime(sourceDir.LastWriteTime, targetDir.LastWriteTime, timeRange) != 0,
                3 => MdlDate.CompareDateTime(sourceDir.LastWriteTime, targetDir.LastWriteTime, timeRange) != 0
                  || MdlDate.CompareDateTime(sourceDir.CreationTime, targetDir.CreationTime, timeRange) != 0,
                _ => false,
            };
        }

        /// <summary>
        /// 2つのファイルのタイムスタンプに指定許容範囲（秒）以上の差異があるか確認します。
        /// </summary>
        /// <param name="sourceFile">比較元のファイル情報。</param>
        /// <param name="targetFile">比較先のファイル情報。</param>
        /// <param name="timeRange">許容誤差（秒）。</param>
        /// <param name="mode">1: 作成日, 2: 更新日, 3: 作成日または更新日。</param>
        /// <returns>タイムスタンプが異なる場合は true、一致していれば false。</returns>
        /// <example>
        /// <code>
        /// bool diff = MdlFile.IsFileTimestampDifferent(file1, file2, 1.0, 2);
        /// </code>
        /// </example>
        public static bool IsFileTimestampDifferent(FileInfo sourceFile, FileInfo targetFile, double timeRange, int mode)
        {
            return mode switch
            {
                1 => MdlDate.CompareDateTime(sourceFile.CreationTime, targetFile.CreationTime, timeRange) != 0,
                2 => MdlDate.CompareDateTime(sourceFile.LastWriteTime, targetFile.LastWriteTime, timeRange) != 0,
                3 => MdlDate.CompareDateTime(sourceFile.LastWriteTime, targetFile.LastWriteTime, timeRange) != 0
                  || MdlDate.CompareDateTime(sourceFile.CreationTime, targetFile.CreationTime, timeRange) != 0,
                _ => false,
            };
        }

        #endregion

        #region File & Directory Attribute Management

        /// <summary>
        /// 指定されたパスのモード（読み取り専用、隠し属性等）を変更します。
        /// </summary>
        /// <param name="path">対象のパス。</param>
        /// <param name="mode">属性モード（"W", "R", "-R", "H", "-H" など）。</param>
        /// <returns>成功した場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool success = MdlFile.ChangeMode(@"C:\data\file.txt", "W");
        /// </code>
        /// </example>
        public static bool ChangeMode(string path, string mode)
        {
            return GetPathType(path) switch
            {
                PATH_IS_DIRECTORY => ChangeDirectoryAttributes(path, mode),
                PATH_IS_FILE => ChangeFileAttributes(path, mode),
                _ => true,
            };
        }

        /// <summary>
        /// 指定されたディレクトリの属性を変更します。
        /// </summary>
        /// <param name="path">対象ディレクトリのパス。</param>
        /// <param name="mode">属性モード文字列。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.ChangeDirectoryAttributes(@"C:\data", "W");
        /// </code>
        /// </example>
        public static bool ChangeDirectoryAttributes(string path, string mode)
        {
            return ChangeDirectoryAttributes(new DirectoryInfo(path), mode);
        }

        /// <summary>
        /// 指定されたディレクトリ情報の属性を変更します。
        /// </summary>
        /// <param name="directoryInfo">対象ディレクトリ情報。</param>
        /// <param name="mode">属性モード文字列。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.ChangeDirectoryAttributes(new DirectoryInfo(@"C:\data"), "W");
        /// </code>
        /// </example>
        public static bool ChangeDirectoryAttributes(DirectoryInfo directoryInfo, string mode)
        {
            bool isSuccess = true;
            string upperMode = mode.ToUpper();
            bool addReadOnly = false, removeReadOnly = false;
            bool addHidden = false, removeHidden = false;

            if (upperMode.Contains('R'))
            {
                if (upperMode.Contains("-R")) removeReadOnly = true;
                else addReadOnly = true;
            }
            if (upperMode.Contains('W'))
            {
                if (upperMode.Contains("-W"))
                {
                    addReadOnly = true;
                    removeReadOnly = false;
                }
                else
                {
                    removeReadOnly = true;
                    removeHidden = true;
                    addReadOnly = false;
                    addHidden = false;
                }
            }
            if (upperMode.Contains('H'))
            {
                if (upperMode.Contains("-H")) removeHidden = true;
                else addHidden = true;
            }

            try
            {
                if (addReadOnly && (directoryInfo.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
                    directoryInfo.Attributes |= FileAttributes.ReadOnly;

                if (removeReadOnly && (directoryInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    directoryInfo.Attributes &= ~FileAttributes.ReadOnly;

                if (addHidden && (directoryInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    directoryInfo.Attributes |= FileAttributes.Hidden;

                if (removeHidden && (directoryInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    directoryInfo.Attributes &= ~FileAttributes.Hidden;
            }
            catch
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定されたファイルの属性を変更します。
        /// </summary>
        /// <param name="path">対象ファイルのパス。</param>
        /// <param name="mode">属性モード文字列。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.ChangeFileAttributes(@"C:\data\file.txt", "W");
        /// </code>
        /// </example>
        public static bool ChangeFileAttributes(string path, string mode)
        {
            return ChangeFileAttributes(new FileInfo(path), mode);
        }

        /// <summary>
        /// 指定されたファイル情報の属性を変更します。
        /// </summary>
        /// <param name="fileInfo">対象ファイル情報。</param>
        /// <param name="mode">属性モード文字列。</param>
        /// <returns>成功した場合は true。</returns>
        /// <example>
        /// <code>
        /// bool ok = MdlFile.ChangeFileAttributes(new FileInfo(@"C:\data\file.txt"), "W");
        /// </code>
        /// </example>
        public static bool ChangeFileAttributes(FileInfo fileInfo, string mode)
        {
            bool isSuccess = true;
            string upperMode = mode.ToUpper();
            bool setNormal = upperMode.Contains("FN");
            bool setWritable = false;
            bool addReadOnly = false, removeReadOnly = false;
            bool addArchive = false, removeArchive = false;
            bool addHidden = false, removeHidden = false;

            if (upperMode.Contains('R'))
            {
                if (upperMode.Contains("-R")) removeReadOnly = true;
                else addReadOnly = true;
            }
            if (upperMode.Contains('W'))
            {
                if (upperMode.Contains("-W"))
                {
                    addReadOnly = true;
                    removeReadOnly = false;
                }
                else if (upperMode.Contains("+W"))
                {
                    removeReadOnly = true;
                    addReadOnly = false;
                }
                else
                {
                    setWritable = true;
                    removeHidden = true;
                    removeReadOnly = false;
                    addReadOnly = false;
                }
            }
            if (upperMode.Contains('A'))
            {
                if (upperMode.Contains("-A")) removeArchive = true;
                else addArchive = true;
            }
            if (upperMode.Contains('H'))
            {
                if (upperMode.Contains("-H")) removeHidden = true;
                else addHidden = true;
            }

            try
            {
                if (setNormal) fileInfo.Attributes = FileAttributes.Normal;
                if (addReadOnly) fileInfo.Attributes = AddAttribute(fileInfo.Attributes, FileAttributes.ReadOnly);
                if (removeReadOnly) fileInfo.Attributes = RemoveAttribute(fileInfo.Attributes, FileAttributes.ReadOnly);

                if (setWritable && (fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                }

                if (addArchive) fileInfo.Attributes = AddAttribute(fileInfo.Attributes, FileAttributes.Archive);
                if (removeArchive) fileInfo.Attributes = RemoveAttribute(fileInfo.Attributes, FileAttributes.Archive);
                if (addHidden) fileInfo.Attributes = AddAttribute(fileInfo.Attributes, FileAttributes.Hidden);
                if (removeHidden) fileInfo.Attributes = RemoveAttribute(fileInfo.Attributes, FileAttributes.Hidden);
            }
            catch
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// ファイル属性フラグを追加します。
        /// </summary>
        /// <param name="attributes">現在のファイル属性。</param>
        /// <param name="attributesToAdd">追加するファイル属性。</param>
        /// <returns>追加後のファイル属性。</returns>
        /// <example>
        /// <code>
        /// FileAttributes attr = MdlFile.AddAttribute(FileAttributes.Normal, FileAttributes.ReadOnly);
        /// </code>
        /// </example>
        public static FileAttributes AddAttribute(FileAttributes attributes, FileAttributes attributesToAdd)
        {
            if ((attributes & attributesToAdd) != attributesToAdd)
            {
                attributes |= attributesToAdd;
            }
            return attributes;
        }

        /// <summary>
        /// ファイル属性フラグを削除します。
        /// </summary>
        /// <param name="attributes">現在のファイル属性。</param>
        /// <param name="attributesToRemove">削除するファイル属性。</param>
        /// <returns>削除後のファイル属性。</returns>
        /// <example>
        /// <code>
        /// FileAttributes attr = MdlFile.RemoveAttribute(FileAttributes.ReadOnly, FileAttributes.ReadOnly);
        /// </code>
        /// </example>
        public static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            if ((attributes & attributesToRemove) == attributesToRemove)
            {
                attributes &= ~attributesToRemove;
            }
            return attributes;
        }

        #endregion

        #region Hash & Encoding Calculations

        /// <summary>
        /// 指定されたファイルの SHA-1 ハッシュ値を計算し、小文字の16進数文字列で返します（.NET 10最適化）。
        /// </summary>
        /// <param name="path">対象のファイルパス。</param>
        /// <returns>SHA-1 ハッシュ文字列。</returns>
        /// <example>
        /// <code>
        /// string hash = MdlFile.ComputeSha1Hash(@"C:\data\file.bin");
        /// </code>
        /// </example>
        public static string ComputeSha1Hash(string path)
        {
            using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] hashBytes = SHA1.HashData(fileStream);
            return Convert.ToHexStringLower(hashBytes);
        }

        /// <summary>
        /// ファイルのエンコーディングを自動検出します。
        /// </summary>
        /// <param name="filePath">対象のファイルパス。</param>
        /// <returns>検出された Encoding オブジェクト。</returns>
        /// <example>
        /// <code>
        /// Encoding enc = MdlFile.DetectFileEncoding(@"C:\data\japanese.txt");
        /// </code>
        /// </example>
        public static Encoding DetectFileEncoding(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.ReadExactly(bytes);
            return DetectEncoding(bytes) ?? Encoding.Default;
        }

        /// <summary>
        /// バイト配列のエンコーディングを自動判定します。
        /// </summary>
        /// <param name="bytes">対象のバイト配列。</param>
        /// <returns>検出された Encoding オブジェクト。バイナリの場合は null。</returns>
        /// <example>
        /// <code>
        /// byte[] data = File.ReadAllBytes(@"C:\data.txt");
        /// Encoding? enc = MdlFile.DetectEncoding(data);
        /// </code>
        /// </example>
        public static Encoding? DetectEncoding(byte[] bytes)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            const byte bEscape = 0x1B;
            const byte bAt = 0x40;
            const byte bDollar = 0x24;
            const byte bAnd = 0x26;
            const byte bOpen = 0x28;
            const byte bB = 0x42;
            const byte bD = 0x44;
            const byte bJ = 0x4A;
            const byte bI = 0x49;

            int len = bytes.Length;

            bool isBinary = false;
            for (int i = 0; i < len; i++)
            {
                byte b1 = bytes[i];
                if (b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF)
                {
                    isBinary = true;
                    if (b1 == 0x00 && i < len - 1 && bytes[i + 1] <= 0x7F)
                    {
                        return Encoding.Unicode;
                    }
                }
            }
            if (isBinary) return null;

            bool notJapanese = true;
            for (int i = 0; i < len; i++)
            {
                byte b1 = bytes[i];
                if (b1 == bEscape || 0x80 <= b1)
                {
                    notJapanese = false;
                    break;
                }
            }
            if (notJapanese) return Encoding.ASCII;

            for (int i = 0; i < len - 2; i++)
            {
                byte b1 = bytes[i];
                byte b2 = bytes[i + 1];
                byte b3 = bytes[i + 2];

                if (b1 == bEscape)
                {
                    if ((b2 == bDollar && b3 == bAt) || (b2 == bDollar && b3 == bB) ||
                        (b2 == bOpen && (b3 == bB || b3 == bJ)) || (b2 == bOpen && b3 == bI))
                    {
                        return Encoding.GetEncoding(50220);
                    }
                    if (i < len - 3)
                    {
                        byte b4 = bytes[i + 3];
                        if (b2 == bDollar && b3 == bOpen && b4 == bD)
                        {
                            return Encoding.GetEncoding(50220);
                        }
                        if (i < len - 5 && b2 == bAnd && b3 == bAt && b4 == bEscape &&
                            bytes[i + 4] == bDollar && bytes[i + 5] == bB)
                        {
                            return Encoding.GetEncoding(50220);
                        }
                    }
                }
            }

            int sjis = 0, euc = 0, utf8 = 0;
            for (int i = 0; i < len - 1; i++)
            {
                byte b1 = bytes[i];
                byte b2 = bytes[i + 1];
                if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
                    ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC)))
                {
                    sjis += 2;
                    i++;
                }
            }
            for (int i = 0; i < len - 1; i++)
            {
                byte b1 = bytes[i];
                byte b2 = bytes[i + 1];
                if (((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) ||
                    (b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF)))
                {
                    euc += 2;
                    i++;
                }
                else if (i < len - 2)
                {
                    byte b3 = bytes[i + 2];
                    if (b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) && (0xA1 <= b3 && b3 <= 0xFE))
                    {
                        euc += 3;
                        i += 2;
                    }
                }
            }
            for (int i = 0; i < len - 1; i++)
            {
                byte b1 = bytes[i];
                byte b2 = bytes[i + 1];
                if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF))
                {
                    utf8 += 2;
                    i++;
                }
                else if (i < len - 2)
                {
                    byte b3 = bytes[i + 2];
                    if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) && (0x80 <= b3 && b3 <= 0xBF))
                    {
                        utf8 += 3;
                        i += 2;
                    }
                }
            }

            if (euc > sjis && euc > utf8) return Encoding.GetEncoding(51932);
            if (sjis > euc && sjis > utf8) return Encoding.GetEncoding(932);
            if (utf8 > euc && utf8 > sjis) return Encoding.UTF8;

            return null;
        }

        #endregion

        #region Path Filtering & Evaluation

        /// <summary>
        /// パスが含めるパターン・除外するパターンに一致するか評価しコード値を返します。
        /// </summary>
        /// <param name="path">評価対象のパス。</param>
        /// <param name="includeBaseName">包含判定時にファイル名のみを使用するか。</param>
        /// <param name="excludeBaseName">除外判定時にファイル名のみを使用するか。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <param name="isOrCondition">包含条件をOR評価するか。</param>
        /// <param name="debugLevel">デバッグ出力レベル。</param>
        /// <returns>評価コード（1: 適合、2: 除外対象、0: 未該当）。</returns>
        /// <example>
        /// <code>
        /// int code = MdlFile.EvaluatePathFilterCode(@"C:\data\file.txt", true, true, new List&lt;string&gt;(), new List&lt;string&gt;(), false, 0);
        /// </code>
        /// </example>
        public static int EvaluatePathFilterCode(string path, bool includeBaseName, bool excludeBaseName, List<string> includePatterns, List<string> excludePatterns, bool isOrCondition, int debugLevel)
        {
            string target = includeBaseName ? Path.GetFileName(path) : path;
            int result = 1;

            if (includePatterns.Count > 0)
            {
                bool isHit = false;
                result = 0;
                foreach (string pattern in includePatterns)
                {
                    if (Regex.IsMatch(target, pattern, RegexOptions.IgnoreCase))
                    {
                        isHit = true;
                        if (debugLevel > 7) Console.WriteLine($"[MdlFile.EvaluatePathFilterCode()][INC][{includeBaseName}] HIT : {pattern} -> {target}");
                        break;
                    }
                    else if (debugLevel > 10)
                    {
                        Console.WriteLine($"[MdlFile.EvaluatePathFilterCode()][INC][{includeBaseName}] NO HIT : {pattern} -> {target}");
                    }
                }
                if (isHit)
                {
                    result = 1;
                    if (isOrCondition) return result;
                }
            }

            target = excludeBaseName ? Path.GetFileName(path) : path;
            if (excludePatterns.Count > 0)
            {
                foreach (string pattern in excludePatterns)
                {
                    if (Regex.IsMatch(target, pattern, RegexOptions.IgnoreCase))
                    {
                        if (debugLevel > 7) Console.WriteLine($"[MdlFile.EvaluatePathFilterCode()][EXC][{excludeBaseName}] HIT : {pattern} -> {target}");
                        return 2;
                    }
                    else if (debugLevel > 10)
                    {
                        Console.WriteLine($"[MdlFile.EvaluatePathFilterCode()][EXC][{includeBaseName}] NO HIT : {pattern} -> {target}");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// パスが指定されたフィルターパターンに一致するか判定します。
        /// </summary>
        /// <param name="path">評価対象のパス。</param>
        /// <param name="includeBaseName">包含判定時にファイル名のみを使用するか。</param>
        /// <param name="excludeBaseName">除外判定時にファイル名のみを使用するか。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <param name="isOrCondition">包含条件をOR評価するか。</param>
        /// <param name="debugLevel">デバッグ出力レベル。</param>
        /// <returns>適合する場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool matched = MdlFile.IsPathFilterMatched(@"C:\data\file.txt", true, true, new List&lt;string&gt;(), new List&lt;string&gt;(), false, 0);
        /// </code>
        /// </example>
        public static bool IsPathFilterMatched(string path, bool includeBaseName, bool excludeBaseName, List<string> includePatterns, List<string> excludePatterns, bool isOrCondition, int debugLevel)
        {
            return EvaluatePathFilterCode(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, isOrCondition, debugLevel) == 1;
        }

        /// <summary>
        /// パスが指定されたフィルターパターンに一致するか判定します（isOrCondition=false）。
        /// </summary>
        /// <param name="path">評価対象のパス。</param>
        /// <param name="includeBaseName">包含判定時にファイル名のみを使用するか。</param>
        /// <param name="excludeBaseName">除外判定時にファイル名のみを使用するか。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <param name="debugLevel">デバッグ出力レベル。</param>
        /// <returns>適合する場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool matched = MdlFile.IsPathFilterMatched(@"C:\data\file.txt", true, true, new List&lt;string&gt;(), new List&lt;string&gt;(), 0);
        /// </code>
        /// </example>
        public static bool IsPathFilterMatched(string path, bool includeBaseName, bool excludeBaseName, List<string> includePatterns, List<string> excludePatterns, int debugLevel)
        {
            return IsPathFilterMatched(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, false, debugLevel);
        }

        /// <summary>
        /// パスが指定されたフィルターパターンに一致するか判定します（debugLevel=0）。
        /// </summary>
        /// <param name="path">評価対象のパス。</param>
        /// <param name="includeBaseName">包含判定時にファイル名のみを使用するか。</param>
        /// <param name="excludeBaseName">除外判定時にファイル名のみを使用するか。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <param name="isOrCondition">包含条件をOR評価するか。</param>
        /// <returns>適合する場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool matched = MdlFile.IsPathFilterMatched(@"C:\data\file.txt", true, true, new List&lt;string&gt;(), new List&lt;string&gt;(), false);
        /// </code>
        /// </example>
        public static bool IsPathFilterMatched(string path, bool includeBaseName, bool excludeBaseName, List<string> includePatterns, List<string> excludePatterns, bool isOrCondition)
        {
            return IsPathFilterMatched(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, isOrCondition, 0);
        }

        /// <summary>
        /// パスが指定されたフィルターパターンに一致するか判定します（isOrCondition=false, debugLevel=0）。
        /// </summary>
        /// <param name="path">評価対象のパス。</param>
        /// <param name="includeBaseName">包含判定時にファイル名のみを使用するか。</param>
        /// <param name="excludeBaseName">除外判定時にファイル名のみを使用するか。</param>
        /// <param name="includePatterns">包含する正規表現パターンのリスト。</param>
        /// <param name="excludePatterns">除外する正規表現パターンのリスト。</param>
        /// <returns>適合する場合は true、それ以外は false。</returns>
        /// <example>
        /// <code>
        /// bool matched = MdlFile.IsPathFilterMatched(@"C:\data\file.txt", true, true, new List&lt;string&gt;(), new List&lt;string&gt;());
        /// </code>
        /// </example>
        public static bool IsPathFilterMatched(string path, bool includeBaseName, bool excludeBaseName, List<string> includePatterns, List<string> excludePatterns)
        {
            return IsPathFilterMatched(path, includeBaseName, excludeBaseName, includePatterns, excludePatterns, false, 0);
        }

        /// <summary>
        /// フィルターフラグを階層再帰ルールに従って結合計算します。
        /// </summary>
        /// <param name="previousEffective">親階層の判定結果フラグ。</param>
        /// <param name="currentEffective">自階層の判定結果フラグ。</param>
        /// <param name="isOrCondition">包含条件をOR評価するか。</param>
        /// <param name="isIncludeHitRecursive">包含ヒットを再帰継承するか。</param>
        /// <param name="isExcludeHitRecursive">除外ヒットを再帰継承するか。</param>
        /// <returns>統合されたフィルターフラグ値。</returns>
        /// <example>
        /// <code>
        /// int flag = MdlFile.CombineFilterFlags(1, 1, false, true, true);
        /// </code>
        /// </example>
        public static int CombineFilterFlags(int previousEffective, int currentEffective, bool isOrCondition, bool isIncludeHitRecursive, bool isExcludeHitRecursive)
        {
            int result = currentEffective;

            switch (previousEffective)
            {
                case 0:
                    result = currentEffective;
                    break;
                case 1:
                    if (isIncludeHitRecursive)
                    {
                        result = currentEffective == 2 ? 3 : 1;
                    }
                    break;
                case 2:
                    if (isExcludeHitRecursive)
                    {
                        result = (currentEffective == 1 && isOrCondition) ? 1 : 2;
                    }
                    break;
                case 3:
                    if (isExcludeHitRecursive)
                    {
                        result = (currentEffective == 1 && isOrCondition) ? 1 : 3;
                    }
                    break;
            }
            return result;
        }

        #endregion

        #region Directory Size & Permissions

        /// <summary>
        /// ディレクトリ内のすべてのファイルサイズの合計（バイト）を取得します。
        /// </summary>
        /// <param name="directoryPath">対象ディレクトリのパス。</param>
        /// <param name="includeSymLinks">シンボリックリンクを含めるかどうか。</param>
        /// <param name="showExceptions">例外メッセージを表示するかどうか。</param>
        /// <returns>合計ファイルサイズ（バイト）。</returns>
        /// <example>
        /// <code>
        /// long size = MdlFile.GetDirectoryFileSize(@"C:\data", false, true);
        /// </code>
        /// </example>
        public static long GetDirectoryFileSize(string directoryPath, bool includeSymLinks, bool showExceptions)
        {
            return GetDirectoryFileSize(new DirectoryInfo(directoryPath), includeSymLinks, showExceptions);
        }

        /// <summary>
        /// ディレクトリ内のすべてのファイルサイズの合計（バイト）を取得します。
        /// </summary>
        /// <param name="directoryInfo">対象ディレクトリ情報。</param>
        /// <param name="includeSymLinks">シンボリックリンクを含めるかどうか。</param>
        /// <param name="showExceptions">例外メッセージを表示するかどうか。</param>
        /// <returns>合計ファイルサイズ（バイト）。</returns>
        /// <example>
        /// <code>
        /// long size = MdlFile.GetDirectoryFileSize(new DirectoryInfo(@"C:\data"), false, true);
        /// </code>
        /// </example>
        public static long GetDirectoryFileSize(DirectoryInfo directoryInfo, bool includeSymLinks, bool showExceptions)
        {
            long totalSize = 0;
            try
            {
                if (includeSymLinks && IsSymlink(directoryInfo.FullName))
                {
                    return 0;
                }

                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    try
                    {
                        if (!includeSymLinks || !IsSymlink(fileInfo.FullName))
                        {
                            totalSize += fileInfo.Length;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (showExceptions) Console.WriteLine($" => FAILED TO GET FILE SIZE({fileInfo.FullName})：EXCEPTION：{ex.Message}");
                    }
                }

                foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    totalSize += GetDirectoryFileSize(subDirectoryInfo, includeSymLinks, showExceptions);
                }
            }
            catch (Exception ex)
            {
                if (showExceptions) Console.WriteLine($" => FAILED TO GET DIR SIZE({directoryInfo.FullName})：EXCEPTION：{ex.Message}");
            }
            return totalSize;
        }

        /// <summary>
        /// 指定されたディレクトリのアクセス権限を取得しコンソールに表示します（Windows環境）。
        /// </summary>
        /// <param name="directoryPath">対象ディレクトリのパス。</param>
        /// <param name="showExceptions">例外メッセージを表示するかどうか。</param>
        /// <returns>なし。</returns>
        /// <example>
        /// <code>
        /// MdlFile.DisplayDirectoryPermissions(@"C:\data", true);
        /// </code>
        /// </example>
        public static void DisplayDirectoryPermissions(string? directoryPath, bool showExceptions)
        {
            Console.WriteLine("AccessControlType,AccountName,FileSystemRights");
            if (string.IsNullOrEmpty(directoryPath)) return;
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                    System.Security.AccessControl.DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
                    foreach (System.Security.AccessControl.FileSystemAccessRule accessRule in directorySecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
                    {
                        string strLine = $"{accessRule.AccessControlType},{(accessRule.IdentityReference as System.Security.Principal.NTAccount).Value},{accessRule.FileSystemRights}";
                        Console.WriteLine(strLine);
                    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
                }
            }
            catch (Exception ex)
            {
                if (showExceptions) Console.WriteLine($" => FAILED TO GET PERMISSION({directoryPath})：EXCEPTION：{ex.Message}");
            }
        }

        #endregion

        #region File & Directory Sorting Utilities

        /// <summary>
        /// 指定ディレクトリ内のファイルパス一覧を条件に従ってソートして返します。
        /// </summary>
        /// <param name="path">検索対象ディレクトリのパス。</param>
        /// <param name="searchPattern">検索パターン（例: "*.txt"）。</param>
        /// <param name="searchOption">サブディレクトリも含めるかの検索オプション。</param>
        /// <param name="sortType">ソート種別（1: 名前, 2: 作成日, 3: 更新日）。</param>
        /// <param name="isAscending">昇順でソートする場合は true。</param>
        /// <param name="isShowFileList">ファイル一覧をコンソール出力するかどうか。</param>
        /// <returns>ソートされたファイルパスの配列。</returns>
        /// <example>
        /// <code>
        /// string[] files = MdlFile.GetSortedFiles(@"C:\data", "*.txt", SearchOption.TopDirectoryOnly, MdlFile.SORT_BY_NAME, true, false);
        /// </code>
        /// </example>
        public static string[] GetSortedFiles(string path, string searchPattern, SearchOption searchOption, int sortType, bool isAscending, bool isShowFileList)
        {
            string[] result = sortType > 0
                ? GetSortedFilesInfo(path, searchPattern, searchOption, sortType, isAscending).Select(f => f.FullName).ToArray()
                : Directory.Exists(path) ? Directory.GetFiles(path, searchPattern, searchOption) : [];

            if (isShowFileList)
            {
                foreach (string curPath in result)
                {
                    Console.WriteLine($"[MdlFile.GetSortedFiles({path})] {curPath}");
                }
            }

            return result;
        }

        /// <summary>
        /// 指定ディレクトリ内の FileInfo 配列を条件に従ってソートして返します。
        /// </summary>
        /// <param name="path">検索対象ディレクトリのパス。</param>
        /// <param name="searchPattern">検索パターン。</param>
        /// <param name="searchOption">検索オプション。</param>
        /// <param name="sortType">ソート種別。</param>
        /// <param name="isAscending">昇順でソートする場合は true。</param>
        /// <returns>ソートされた FileInfo の配列。</returns>
        /// <example>
        /// <code>
        /// FileInfo[] files = MdlFile.GetSortedFilesInfo(@"C:\data", "*.txt", SearchOption.TopDirectoryOnly, MdlFile.SORT_BY_NAME, true);
        /// </code>
        /// </example>
        public static FileInfo[] GetSortedFilesInfo(string path, string searchPattern, SearchOption searchOption, int sortType, bool isAscending)
        {
            if (!Directory.Exists(path)) return [];

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles(searchPattern, searchOption);

            if (sortType > 0)
            {
                Array.Sort(files, (f1, f2) =>
                {
                    int comparison = sortType switch
                    {
                        SORT_BY_NAME => string.Compare(f1.Name, f2.Name, StringComparison.OrdinalIgnoreCase),
                        SORT_BY_CTIME => DateTime.Compare(f1.CreationTime, f2.CreationTime),
                        SORT_BY_MTIME => DateTime.Compare(f1.LastWriteTime, f2.LastWriteTime),
                        _ => 0,
                    };
                    return isAscending ? comparison : -comparison;
                });
            }

            return files;
        }

        /// <summary>
        /// 指定ディレクトリ内のサブディレクトリパス一覧を条件に従ってソートして返します。
        /// </summary>
        /// <param name="path">検索対象ディレクトリのパス。</param>
        /// <param name="searchPattern">検索パターン。</param>
        /// <param name="searchOption">検索オプション。</param>
        /// <param name="sortType">ソート種別。</param>
        /// <param name="isAscending">昇順でソートする場合は true。</param>
        /// <param name="isShowDirList">ディレクトリ一覧をコンソール出力するかどうか。</param>
        /// <returns>ソートされたディレクトリパスの配列。</returns>
        /// <example>
        /// <code>
        /// string[] dirs = MdlFile.GetSortedDirectories(@"C:\data", "*", SearchOption.TopDirectoryOnly, MdlFile.SORT_BY_NAME, true, false);
        /// </code>
        /// </example>
        public static string[] GetSortedDirectories(string path, string searchPattern, SearchOption searchOption, int sortType, bool isAscending, bool isShowDirList)
        {
            string[] result = sortType > 0
                ? GetSortedDirectoriesInfo(path, searchPattern, searchOption, sortType, isAscending).Select(d => d.FullName).ToArray()
                : Directory.Exists(path) ? Directory.GetDirectories(path, searchPattern, searchOption) : [];

            if (isShowDirList)
            {
                foreach (string curPath in result)
                {
                    Console.WriteLine($"[MdlFile.GetSortedDirectories({path})] {curPath}");
                }
            }

            return result;
        }

        /// <summary>
        /// 指定ディレクトリ内の DirectoryInfo 配列を条件に従ってソートして返します。
        /// </summary>
        /// <param name="path">検索対象ディレクトリのパス。</param>
        /// <param name="searchPattern">検索パターン。</param>
        /// <param name="searchOption">検索オプション。</param>
        /// <param name="sortType">ソート種別。</param>
        /// <param name="isAscending">昇順でソートする場合は true。</param>
        /// <returns>ソートされた DirectoryInfo の配列。</returns>
        /// <example>
        /// <code>
        /// DirectoryInfo[] dirs = MdlFile.GetSortedDirectoriesInfo(@"C:\data", "*", SearchOption.TopDirectoryOnly, MdlFile.SORT_BY_NAME, true);
        /// </code>
        /// </example>
        public static DirectoryInfo[] GetSortedDirectoriesInfo(string path, string searchPattern, SearchOption searchOption, int sortType, bool isAscending)
        {
            if (!Directory.Exists(path)) return [];

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            DirectoryInfo[] directories = dirInfo.GetDirectories(searchPattern, searchOption);

            if (sortType > 0)
            {
                Array.Sort(directories, (d1, d2) =>
                {
                    int comparison = sortType switch
                    {
                        SORT_BY_NAME => string.Compare(d1.Name, d2.Name, StringComparison.OrdinalIgnoreCase),
                        SORT_BY_CTIME => DateTime.Compare(d1.CreationTime, d2.CreationTime),
                        SORT_BY_MTIME => DateTime.Compare(d1.LastWriteTime, d2.LastWriteTime),
                        _ => 0,
                    };
                    return isAscending ? comparison : -comparison;
                });
            }

            return directories;
        }

        /// <summary>
        /// ソートタイプ番号に対応する識別文字列を取得します。
        /// </summary>
        /// <param name="sortType">ソートタイプ番号。</param>
        /// <returns>ソートタイプ名（"name", "ctime", "mtime", "none"）。</returns>
        /// <example>
        /// <code>
        /// string name = MdlFile.GetSortTypeName(MdlFile.SORT_BY_NAME); // "name"
        /// </code>
        /// </example>
        public static string GetSortTypeName(int sortType)
        {
            return sortType switch
            {
                SORT_BY_NAME => "name",
                SORT_BY_CTIME => "ctime",
                SORT_BY_MTIME => "mtime",
                _ => "none",
            };
        }

        /// <summary>
        /// ソートタイプ識別文字列に対応するソートタイプ番号を取得します。
        /// </summary>
        /// <param name="name">ソートタイプ識別文字列。</param>
        /// <returns>ソートタイプ番号（<see cref="SORT_BY_NAME"/>, <see cref="SORT_BY_CTIME"/>, <see cref="SORT_BY_MTIME"/>, <see cref="SORT_BY_NONE"/>）。</returns>
        /// <example>
        /// <code>
        /// int type = MdlFile.GetSortTypeNum("name"); // SORT_BY_NAME
        /// </code>
        /// </example>
        public static int GetSortTypeNum(string name)
        {
            if (string.IsNullOrEmpty(name)) return SORT_BY_NONE;
            return name.ToLower() switch
            {
                "name" => SORT_BY_NAME,
                "ctime" => SORT_BY_CTIME,
                "mtime" => SORT_BY_MTIME,
                _ => SORT_BY_NONE,
            };
        }

        #endregion
    }
}
