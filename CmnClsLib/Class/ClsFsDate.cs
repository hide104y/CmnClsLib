using System;
using CmnClsLib.Interface;
using CmnClsLib.Module;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Class;

/// <summary>
/// ファイルシステムの日付操作を行うクラス
/// </summary>
/// <param name="logger">ログ出力用ロガーのインスタンス。</param>
/// <example>
/// <code>
/// ICmnLogger logger = new ConsoleLogger();
/// var dateManager = new ClsFsDate(logger);
/// </code>
/// </example>
public class ClsFsDate(ICmnLogger logger)
{
    private readonly ICmnLogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// ログの出力レベル（詳細度）を取得または設定します。
    /// </summary>
    /// <example>
    /// <code>
    /// dateManager.Verbose = 1;
    /// </code>
    /// </example>
    public int Verbose { get; set; }

    /// <summary>
    /// 直近の処理で発生したエラーメッセージを取得または設定します。
    /// </summary>
    /// <example>
    /// <code>
    /// string err = dateManager.Message;
    /// </code>
    /// </example>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 例外発生時に例外を再スローするかどうかを取得または設定します。
    /// </summary>
    /// <example>
    /// <code>
    /// dateManager.IsThrowIfException = true;
    /// </code>
    /// </example>
    public bool IsThrowIfException { get; set; }

    /// <summary>
    /// 例外発生時に例外を再スローするかどうかを取得または設定します。（旧プロパティ）
    /// </summary>
    /// <example>
    /// <code>
    /// dateManager.IsThrowIfExcptn = true;
    /// </code>
    /// </example>
    [Obsolete("代わりに 'IsThrowIfException' を使用します。")]
    public bool IsThrowIfExcptn
    {
        get => IsThrowIfException;
        set => IsThrowIfException = value;
    }

    /// <summary>
    /// ファイルまたはディレクトリの日付を設定します。
    /// </summary>
    /// <param name="path">対象のファイルまたはディレクトリのパス。</param>
    /// <param name="dateString">設定する日付文字列。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="pathKind">パスの種類（ファイル/ディレクトリ等）。</param>
    /// <param name="isValidateDate">日付フォーマットの検証を行うかどうか。</param>
    /// <param name="isForce">属性の変更など強制的に設定を行うかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDate(@"C:\test.txt", "2026/08/02 10:00:00", 1, 0, true, true, true);
    /// </code>
    /// </example>
    public bool SetDate(string path, string dateString, int mode, int pathKind, bool isValidateDate, bool isForce, bool isExec)
    {
        try
        {
            return SetDateCore(path, dateString, mode, pathKind, isValidateDate, isForce, isExec) > -1;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ファイルまたはディレクトリの日付を設定します。（実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象のファイルまたはディレクトリのパス。</param>
    /// <param name="dateString">設定する日付文字列。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="pathKind">パスの種類。</param>
    /// <param name="isValidateDate">日付の検証を行うかどうか。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDate(@"C:\test.txt", "2026/08/02", 1, 0, true, true);
    /// </code>
    /// </example>
    public bool SetDate(string path, string dateString, int mode, int pathKind, bool isValidateDate, bool isForce)
        => SetDate(path, dateString, mode, pathKind, isValidateDate, isForce, true);

    /// <summary>
    /// ファイルまたはディレクトリの日付を設定します。（強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象のファイルまたはディレクトリのパス。</param>
    /// <param name="dateString">設定する日付文字列。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="pathKind">パスの種類。</param>
    /// <param name="isValidateDate">日付の検証を行うかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDate(@"C:\test.txt", "2026/08/02", 1, 0, true);
    /// </code>
    /// </example>
    public bool SetDate(string path, string dateString, int mode, int pathKind, bool isValidateDate)
        => SetDate(path, dateString, mode, pathKind, isValidateDate, true, true);

    /// <summary>
    /// ファイルまたはディレクトリの日付を設定するメイン処理を実行します。
    /// </summary>
    /// <param name="path">対象のファイルまたはディレクトリのパス。</param>
    /// <param name="dateString">設定する日付文字列。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="pathKind">パスの種類。</param>
    /// <param name="isValidateDate">日付の検証を行うかどうか。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>処理結果ステータスコード（0以上で成功、-1で失敗）。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateCore(@"C:\test.txt", "2026/08/02", 1, 0, true, true, true);
    /// </code>
    /// </example>
    public int SetDateCore(string path, string dateString, int mode, int pathKind, bool isValidateDate, bool isForce, bool isExec)
    {
        Message = string.Empty;
        try
        {
            return MdlFile.SetDateMain(path, dateString, mode, pathKind, isValidateDate, isForce, isExec);
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            if (Verbose > 0) _logger.WriteLine(MdlConst.LVL_E, $"[ClsFsDate.SetDate()] EXCEPTION : {ex.Message}");
            if (IsThrowIfException) throw;
            return -1;
        }
    }

    /// <summary>
    /// ファイルまたはディレクトリの日付を設定するメイン処理を実行します。（実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象のファイルまたはディレクトリのパス。</param>
    /// <param name="dateString">設定する日付文字列。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="pathKind">パスの種類。</param>
    /// <param name="isValidateDate">日付の検証を行うかどうか。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateCore(@"C:\test.txt", "2026/08/02", 1, 0, true, true);
    /// </code>
    /// </example>
    public int SetDateCore(string path, string dateString, int mode, int pathKind, bool isValidateDate, bool isForce)
        => SetDateCore(path, dateString, mode, pathKind, isValidateDate, isForce, true);

    /// <summary>
    /// ファイルまたはディレクトリの日付を設定するメイン処理を実行します。（強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象のファイルまたはディレクトリのパス。</param>
    /// <param name="dateString">設定する日付文字列。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="pathKind">パスの種類。</param>
    /// <param name="isValidateDate">日付の検証を行うかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateCore(@"C:\test.txt", "2026/08/02", 1, 0, true);
    /// </code>
    /// </example>
    public int SetDateCore(string path, string dateString, int mode, int pathKind, bool isValidateDate)
        => SetDateCore(path, dateString, mode, pathKind, isValidateDate, true, true);

    /// <summary>
    /// ファイルまたはディレクトリの日付を設定するメイン処理を実行します。（旧方式メソッド）
    /// </summary>
    /// <param name="path">対象のファイルまたはディレクトリのパス。</param>
    /// <param name="strDate">設定する日付文字列。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="kindOfPath">パスの種類。</param>
    /// <param name="isValidateDate">日付の検証を行うかどうか。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateMain(@"C:\test.txt", "2026/08/02", 1, 0, true, true, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetDateCore()' を使用します。")]
    public int SetDateMain(string path, string strDate, int mode, int kindOfPath, bool isValidateDate, bool isForce, bool isExec)
        => SetDateCore(path, strDate, mode, kindOfPath, isValidateDate, isForce, isExec);

    /// <summary>
    /// ファイルまたはディレクトリの日付を設定するメイン処理を実行します。（旧方式メソッド・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象のファイルまたはディレクトリのパス。</param>
    /// <param name="strDate">設定する日付文字列。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="kindOfPath">パスの種類。</param>
    /// <param name="isValidateDate">日付の検証を行うかどうか。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateMain(@"C:\test.txt", "2026/08/02", 1, 0, true, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetDateCore()' を使用します。")]
    public int SetDateMain(string path, string strDate, int mode, int kindOfPath, bool isValidateDate, bool isForce)
        => SetDateCore(path, strDate, mode, kindOfPath, isValidateDate, isForce);

    /// <summary>
    /// ファイルまたはディレクトリの日付を設定するメイン処理を実行します。（旧方式メソッド・強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象のファイルまたはディレクトリのパス。</param>
    /// <param name="strDate">設定する日付文字列。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="kindOfPath">パスの種類。</param>
    /// <param name="isValidateDate">日付の検証を行うかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateMain(@"C:\test.txt", "2026/08/02", 1, 0, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetDateCore()' を使用します。")]
    public int SetDateMain(string path, string strDate, int mode, int kindOfPath, bool isValidateDate)
        => SetDateCore(path, strDate, mode, kindOfPath, isValidateDate);

    /// <summary>
    /// ディレクトリの日付を設定します。
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDirectoryDate(@"C:\myDir", DateTime.Now, 1, true, true);
    /// </code>
    /// </example>
    public bool SetDirectoryDate(string path, DateTime date, int mode, bool isForce, bool isExec)
    {
        try
        {
            return SetDirectoryDateCore(path, date, mode, isForce, isExec) > -1;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ディレクトリの日付を設定します。（実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDirectoryDate(@"C:\myDir", DateTime.Now, 1, true);
    /// </code>
    /// </example>
    public bool SetDirectoryDate(string path, DateTime date, int mode, bool isForce)
        => SetDirectoryDate(path, date, mode, isForce, true);

    /// <summary>
    /// ディレクトリの日付を設定します。（強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDirectoryDate(@"C:\myDir", DateTime.Now, 1);
    /// </code>
    /// </example>
    public bool SetDirectoryDate(string path, DateTime date, int mode)
        => SetDirectoryDate(path, date, mode, true);

    /// <summary>
    /// ディレクトリの日付を設定します。（旧方式メソッド）
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDateToDir(@"C:\myDir", DateTime.Now, 1, true, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetDirectoryDate()' を使用します。")]
    public bool SetDateToDir(string path, DateTime date, int mode, bool isForce, bool isExec)
        => SetDirectoryDate(path, date, mode, isForce, isExec);

    /// <summary>
    /// ディレクトリの日付を設定します。（旧方式メソッド・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDateToDir(@"C:\myDir", DateTime.Now, 1, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetDirectoryDate()' を使用します。")]
    public bool SetDateToDir(string path, DateTime date, int mode, bool isForce)
        => SetDirectoryDate(path, date, mode, isForce);

    /// <summary>
    /// ディレクトリの日付を設定します。（旧方式メソッド・強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDateToDir(@"C:\myDir", DateTime.Now, 1);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetDirectoryDate()' を使用します。")]
    public bool SetDateToDir(string path, DateTime date, int mode)
        => SetDirectoryDate(path, date, mode);

    /// <summary>
    /// ディレクトリの日付を設定するメイン処理を実行します。
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>処理結果ステータスコード（0以上で成功、-1で失敗）。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDirectoryDateCore(@"C:\myDir", DateTime.Now, 1, true, true);
    /// </code>
    /// </example>
    public int SetDirectoryDateCore(string path, DateTime date, int mode, bool isForce, bool isExec)
    {
        Message = string.Empty;
        try
        {
            return MdlFile.SetDateToDirMain(path, date, mode, isForce, isExec);
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            if (Verbose > 0) _logger.WriteLine(MdlConst.LVL_E, $"[ClsFsDate.SetDirectoryDate()] EXCEPTION : {ex.Message}");
            if (IsThrowIfException) throw;
            return -1;
        }
    }

    /// <summary>
    /// ディレクトリの日付を設定するメイン処理を実行します。（実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDirectoryDateCore(@"C:\myDir", DateTime.Now, 1, true);
    /// </code>
    /// </example>
    public int SetDirectoryDateCore(string path, DateTime date, int mode, bool isForce)
        => SetDirectoryDateCore(path, date, mode, isForce, true);

    /// <summary>
    /// ディレクトリの日付を設定するメイン処理を実行します。（強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDirectoryDateCore(@"C:\myDir", DateTime.Now, 1);
    /// </code>
    /// </example>
    public int SetDirectoryDateCore(string path, DateTime date, int mode)
        => SetDirectoryDateCore(path, date, mode, true, true);

    /// <summary>
    /// ディレクトリの日付を設定するメイン処理を実行します。（旧方式メソッド）
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateToDirMain(@"C:\myDir", DateTime.Now, 1, true, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetDirectoryDateCore()' を使用します。")]
    public int SetDateToDirMain(string path, DateTime date, int mode, bool isForce, bool isExec)
        => SetDirectoryDateCore(path, date, mode, isForce, isExec);

    /// <summary>
    /// ディレクトリの日付を設定するメイン処理を実行します。（旧方式メソッド・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateToDirMain(@"C:\myDir", DateTime.Now, 1, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetDirectoryDateCore()' を使用します。")]
    public int SetDateToDirMain(string path, DateTime date, int mode, bool isForce)
        => SetDirectoryDateCore(path, date, mode, isForce);

    /// <summary>
    /// ディレクトリの日付を設定するメイン処理を実行します。（旧方式メソッド・強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ディレクトリのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateToDirMain(@"C:\myDir", DateTime.Now, 1);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetDirectoryDateCore()' を使用します。")]
    public int SetDateToDirMain(string path, DateTime date, int mode)
        => SetDirectoryDateCore(path, date, mode);

    /// <summary>
    /// ファイルの日付を設定します。
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetFileDate(@"C:\myFile.txt", DateTime.Now, 1, true, true);
    /// </code>
    /// </example>
    public bool SetFileDate(string path, DateTime date, int mode, bool isForce, bool isExec)
    {
        try
        {
            return SetFileDateCore(path, date, mode, isForce, isExec) > -1;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ファイルの日付を設定します。（実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetFileDate(@"C:\myFile.txt", DateTime.Now, 1, true);
    /// </code>
    /// </example>
    public bool SetFileDate(string path, DateTime date, int mode, bool isForce)
        => SetFileDate(path, date, mode, isForce, true);

    /// <summary>
    /// ファイルの日付を設定します。（強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetFileDate(@"C:\myFile.txt", DateTime.Now, 1);
    /// </code>
    /// </example>
    public bool SetFileDate(string path, DateTime date, int mode)
        => SetFileDate(path, date, mode, true, true);

    /// <summary>
    /// ファイルの日付を設定します。（旧方式メソッド）
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDateToFile(@"C:\myFile.txt", DateTime.Now, 1, true, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetFileDate()' を使用します。")]
    public bool SetDateToFile(string path, DateTime date, int mode, bool isForce, bool isExec)
        => SetFileDate(path, date, mode, isForce, isExec);

    /// <summary>
    /// ファイルの日付を設定します。（旧方式メソッド・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDateToFile(@"C:\myFile.txt", DateTime.Now, 1, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetFileDate()' を使用します。")]
    public bool SetDateToFile(string path, DateTime date, int mode, bool isForce)
        => SetFileDate(path, date, mode, isForce);

    /// <summary>
    /// ファイルの日付を設定します。（旧方式メソッド・強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <returns>設定が成功した場合は true、失敗した場合は false。</returns>
    /// <example>
    /// <code>
    /// bool isOk = fsDate.SetDateToFile(@"C:\myFile.txt", DateTime.Now, 1);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetFileDate()' を使用します。")]
    public bool SetDateToFile(string path, DateTime date, int mode)
        => SetFileDate(path, date, mode);

    /// <summary>
    /// ファイルの日付を設定するメイン処理を実行します。
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>処理結果ステータスコード（0以上で成功、-1で失敗）。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetFileDateCore(@"C:\myFile.txt", DateTime.Now, 1, true, true);
    /// </code>
    /// </example>
    public int SetFileDateCore(string path, DateTime date, int mode, bool isForce, bool isExec)
    {
        Message = string.Empty;
        try
        {
            return MdlFile.SetDateToFileMain(path, date, mode, isForce, isExec);
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            if (Verbose > 0) _logger.WriteLine(MdlConst.LVL_E, $"[ClsFsDate.SetFileDate()] EXCEPTION : {ex.Message}");
            if (IsThrowIfException) throw;
            return -1;
        }
    }

    /// <summary>
    /// ファイルの日付を設定するメイン処理を実行します。（実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetFileDateCore(@"C:\myFile.txt", DateTime.Now, 1, true);
    /// </code>
    /// </example>
    public int SetFileDateCore(string path, DateTime date, int mode, bool isForce)
        => SetFileDateCore(path, date, mode, isForce, true);

    /// <summary>
    /// ファイルの日付を設定するメイン処理を実行します。（強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetFileDateCore(@"C:\myFile.txt", DateTime.Now, 1);
    /// </code>
    /// </example>
    public int SetFileDateCore(string path, DateTime date, int mode)
        => SetFileDateCore(path, date, mode, true, true);

    /// <summary>
    /// ファイルの日付を設定するメイン処理を実行します。（旧方式メソッド）
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <param name="isExec">実際に実行するかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateToFileMain(@"C:\myFile.txt", DateTime.Now, 1, true, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetFileDateCore()' を使用します。")]
    public int SetDateToFileMain(string path, DateTime date, int mode, bool isForce, bool isExec)
        => SetFileDateCore(path, date, mode, isForce, isExec);

    /// <summary>
    /// ファイルの日付を設定するメイン処理を実行します。（旧方式メソッド・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <param name="isForce">強制的に設定するかどうか。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateToFileMain(@"C:\myFile.txt", DateTime.Now, 1, true);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetFileDateCore()' を使用します。")]
    public int SetDateToFileMain(string path, DateTime date, int mode, bool isForce)
        => SetFileDateCore(path, date, mode, isForce);

    /// <summary>
    /// ファイルの日付を設定するメイン処理を実行します。（旧方式メソッド・強制・実行フラグ省略版）
    /// </summary>
    /// <param name="path">対象ファイルのパス。</param>
    /// <param name="date">設定する DateTime 日時。</param>
    /// <param name="mode">処理モード。</param>
    /// <returns>処理結果ステータスコード。</returns>
    /// <example>
    /// <code>
    /// int code = fsDate.SetDateToFileMain(@"C:\myFile.txt", DateTime.Now, 1);
    /// </code>
    /// </example>
    [Obsolete("代わりに 'SetFileDateCore()' を使用します。")]
    public int SetDateToFileMain(string path, DateTime date, int mode)
        => SetFileDateCore(path, date, mode);
}
