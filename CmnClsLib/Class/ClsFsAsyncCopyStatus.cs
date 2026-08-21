using CmnClsLib.Module;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Class
{
    /// <summary>
    /// ファイルの非同期コピー状態およびストリームを管理するクラスです。
    /// </summary>
    /// <example>
    /// <code>
    /// using var copyStatus = new ClsFsAsyncCopyStatus("source.dat", "dest.dat", true);
    /// if (copyStatus.IsOk)
    /// {
    ///     copyStatus.ShowProgress();
    /// }
    /// </code>
    /// </example>
    public class ClsFsAsyncCopyStatus : IDisposable, IAsyncDisposable
    {
        private FileStream? _sourceStream;
        private FileStream? _destinationStream;
        private byte[] _buffer = new byte[0x1000];
        private DateTime _startTime;
        private long _checkCount = 256;
        private long _currentCount = 0;
        private long _fileSize = 0;
        private string _progressLine = "";
        private string _message = "";
        private string _stackTrace = "";
        private bool _showProgress = false;
        private bool _isDone = false;
        private bool _isOk = true;
        private FileShare _fileShare = FileShare.ReadWrite;
        private bool _disposedValue;

        /// <summary>
        /// <see cref="ClsFsAsyncCopyStatus"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="sourcePath">コピー元ファイルのパス。</param>
        /// <param name="destinationPath">コピー先ファイルのパス。</param>
        /// <param name="isAsync">非同期 I/O を使用してコピーを開く場合は <c>true</c>。それ以外は <c>false</c>。</param>
        /// <param name="fileShare">ファイル共有モード。</param>
        /// <example>
        /// <code>
        /// var status = new ClsFsAsyncCopyStatus(@"C:\src.txt", @"C:\dst.txt", true, FileShare.Read);
        /// </code>
        /// </example>
        public ClsFsAsyncCopyStatus(string sourcePath, string destinationPath, bool isAsync, FileShare fileShare)
        {
            Initialize(sourcePath, destinationPath, isAsync, fileShare);
        }

        /// <summary>
        /// <see cref="ClsFsAsyncCopyStatus"/> クラスの新しいインスタンスを初期化します（共有モード: ReadWrite）。
        /// </summary>
        /// <param name="sourcePath">コピー元ファイルのパス。</param>
        /// <param name="destinationPath">コピー先ファイルのパス。</param>
        /// <param name="isAsync">非同期 I/O を使用してコピーを開く場合は <c>true</c>。それ以外は <c>false</c>。</param>
        /// <example>
        /// <code>
        /// var status = new ClsFsAsyncCopyStatus(@"C:\src.txt", @"C:\dst.txt", true);
        /// </code>
        /// </example>
        public ClsFsAsyncCopyStatus(string sourcePath, string destinationPath, bool isAsync)
        {
            Initialize(sourcePath, destinationPath, isAsync, FileShare.ReadWrite);
        }

        /// <summary>
        /// コピー元のファイルストリームを取得または設定します。
        /// </summary>
        public FileStream? SourceStream { get => _sourceStream; set => _sourceStream = value; }

        /// <summary>
        /// コピー先のファイルストリームを取得または設定します。
        /// </summary>
        public FileStream? DestinationStream { get => _destinationStream; set => _destinationStream = value; }

        /// <summary>
        /// データ転送用バッファーを取得または設定します。
        /// </summary>
        public byte[] Buffer { get => _buffer; set => _buffer = value; }

        /// <summary>
        /// コピー処理が完了しているかどうかを示す値を取得または設定します。
        /// </summary>
        public bool IsDone { get => _isDone; set => _isDone = value; }

        /// <summary>
        /// コピー開始日時を取得または設定します。
        /// </summary>
        public DateTime StartTime { get => _startTime; set => _startTime = value; }

        /// <summary>
        /// 進捗チェックの間隔（I/O回数単位）を取得または設定します。
        /// </summary>
        public long CheckCount { get => _checkCount; set => _checkCount = value; }

        /// <summary>
        /// 現在のループカウント数を取得または設定します。
        /// </summary>
        public long CurrentCount { get => _currentCount; set => _currentCount = value; }

        /// <summary>
        /// 現在のループカウント数を取得または設定します。
        /// </summary>
        [Obsolete("代わりに 'CurrentCount' を使用します。")]
        public long CurCount { get => CurrentCount; set => CurrentCount = value; }

        /// <summary>
        /// コピー対象ファイルのサイズ（バイト単位）を取得または設定します。
        /// </summary>
        public long FileSize { get => _fileSize; set => _fileSize = value; }

        /// <summary>
        /// 進捗画面を表示するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool IsShowProgress { get => _showProgress; set => _showProgress = value; }


        /// <summary>
        /// ストリームオープンおよび初期化が正常に成功したかどうかを示す値を取得または設定します。
        /// </summary>
        public bool IsOk { get => _isOk; set => _isOk = value; }

        /// <summary>
        /// 最新の進捗表示文字列を取得または設定します。
        /// </summary>
        public string ProgressLine { get => _progressLine; set => _progressLine = value; }

        /// <summary>
        /// エラー発生時のエラーメッセージを取得または設定します。
        /// </summary>
        public string Message { get => _message; set => _message = value; }

        /// <summary>
        /// エラー発生時のスタックトレース文字列を取得または設定します。
        /// </summary>
        public string StackTrace { get => _stackTrace; set => _stackTrace = value; }

        /// <summary>
        /// ファイル共有モードを取得または設定します。
        /// </summary>
        public FileShare FileShare { get => _fileShare; set => _fileShare = value; }

        /// <summary>
        /// コピー元およびコピー先のファイルストリームを初期化し、非同期コピーの準備を行います。
        /// </summary>
        /// <param name="sourcePath">コピー元ファイルのパス。</param>
        /// <param name="destinationPath">コピー先ファイルのパス。</param>
        /// <param name="isAsync">非同期 I/O モードで開く場合は <c>true</c>。それ以外は <c>false</c>。</param>
        /// <param name="fileShare">ファイル共有モード。</param>
        /// <returns>初期化が成功した場合は <c>true</c>。失敗した場合は <c>false</c>。</returns>
        /// <example>
        /// <code>
        /// var status = new ClsFsAsyncCopyStatus(@"C:\src.txt", @"C:\dst.txt", true);
        /// bool success = status.Initialize(@"C:\src.txt", @"C:\dst.txt", true, FileShare.Read);
        /// </code>
        /// </example>
        public bool Initialize(string sourcePath, string destinationPath, bool isAsync, FileShare fileShare)
        {
            try
            {
                _fileShare = fileShare;
                _isOk = OpenSourceFile(sourcePath, isAsync);
                if (_isOk) _isOk = OpenDestinationFile(destinationPath, isAsync);
                if (_isOk && _sourceStream is not null)
                {
                    // ファイルサイズ
                    _fileSize = _sourceStream.Length;
                    // 開始時刻
                    _startTime = DateTime.Now;
                    // 進捗表示間隔を取得
                    if (_fileSize > 0) _checkCount = _fileSize / _buffer.Length / 100;
                    // 1%あたりのI/O回数の上限を制限
                    _checkCount = Math.Min(_checkCount, 5000);
                }
            }
            catch
            {
                _isOk = false;
            }
            return _isOk;
        }

        /// <summary>
        /// [旧型式] コピー元およびコピー先のファイルストリームを初期化します。
        /// </summary>
        /// <param name="sourcePath">コピー元ファイルのパス。</param>
        /// <param name="destinationPath">コピー先ファイルのパス。</param>
        /// <param name="isAsync">非同期コピーを行うかどうか。</param>
        /// <param name="fileShare">ファイル共有モード。</param>
        /// <returns>初期化が成功した場合は <c>true</c>。失敗した場合は <c>false</c>。</returns>
        [Obsolete("代わりに 'Initialize(sourcePath, destinationPath, isAsync, fileShare)' を使用します。")]
        public bool Init(string sourcePath, string destinationPath, bool isAsync, FileShare fileShare)
        {
            return Initialize(sourcePath, destinationPath, isAsync, fileShare);
        }

        /// <summary>
        /// コピー元のファイルを開きます。
        /// </summary>
        /// <param name="sourcePath">コピー元ファイルのパス。</param>
        /// <param name="isAsync">非同期で開く場合は <c>true</c>。それ以外は <c>false</c>。</param>
        /// <returns>正常に開けた場合は <c>true</c>。失敗した場合は <c>false</c>。</returns>
        /// <example>
        /// <code>
        /// bool isOpened = status.OpenSourceFile(@"C:\source.dat", true);
        /// </code>
        /// </example>
        public bool OpenSourceFile(string sourcePath, bool isAsync)
        {
            bool isOk = true;
            if (string.IsNullOrEmpty(sourcePath)) return false;
            try
            {
                if (isAsync)
                {
                    _sourceStream = new FileStream(
                                    sourcePath,         // パス
                                    FileMode.Open,      // 作成モード
                                    FileAccess.Read,    // 読み取り/書き込みアクセス許可
                                    _fileShare,         // 共有アクセス許可
                                    0x1000,             // バッファー サイズ：4096
                                    useAsync: true);    // 非同期フラグ
                }
                else
                {
                    _sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, _fileShare);
                }
            }
            catch (Exception ex)
            {
                isOk = false;
                _message = $"[ClsFsAsyncCopyStatus.OpenSourceFile({sourcePath})] {ex.Message}";
                _stackTrace = ex.StackTrace ?? "";
            }
            return isOk;
        }

        /// <summary>
        /// [旧型式] コピー元のファイルを開きます。
        /// </summary>
        /// <param name="sourcePath">コピー元ファイルのパス。</param>
        /// <param name="isAsync">非同期で開くかどうか。</param>
        /// <returns>正常に開けた場合は <c>true</c>。失敗した場合は <c>false</c>。</returns>
        [Obsolete("代わりに 'OpenSourceFile(sourcePath, isAsync)' を使用します。")]
        public bool FileOpenFr(string sourcePath, bool isAsync)
        {
            return OpenSourceFile(sourcePath, isAsync);
        }

        /// <summary>
        /// コピー先のファイルを作成・開きます。
        /// </summary>
        /// <param name="destinationPath">コピー先ファイルのパス。</param>
        /// <param name="isAsync">非同期で開く場合は <c>true</c>。それ以外は <c>false</c>。</param>
        /// <returns>正常に作成・開けた場合は <c>true</c>。失敗した場合は <c>false</c>。</returns>
        /// <example>
        /// <code>
        /// bool isOpened = status.OpenDestinationFile(@"C:\dest.dat", true);
        /// </code>
        /// </example>
        public bool OpenDestinationFile(string destinationPath, bool isAsync)
        {
            bool isOk = true;
            if (string.IsNullOrEmpty(destinationPath)) return false;
            try
            {
                if (isAsync)
                {
                    _destinationStream = new FileStream(
                                    destinationPath,        // パス
                                    FileMode.Create,        // 作成モード
                                    FileAccess.ReadWrite,   // 読み取り/書き込みアクセス許可
                                    _fileShare,             // 共有アクセス許可
                                    0x1000,                 // バッファー サイズ：4096
                                    useAsync: true);        // 非同期フラグ
                }
                else
                {
                    _destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite, _fileShare);
                }
            }
            catch (Exception ex)
            {
                isOk = false;
                _message = $"[ClsFsAsyncCopyStatus.OpenDestinationFile({destinationPath})] {ex.Message}";
                _stackTrace = ex.StackTrace ?? "";
            }
            return isOk;
        }

        /// <summary>
        /// [旧型式] コピー先のファイルを開きます。
        /// </summary>
        /// <param name="destinationPath">コピー先ファイルのパス。</param>
        /// <param name="isAsync">非同期で開くかどうか。</param>
        /// <returns>正常に開けた場合は <c>true</c>。失敗した場合は <c>false</c>。</returns>
        [Obsolete("代わりに 'OpenDestinationFile(destinationPath, isAsync)' を使用します。")]
        public bool FileOpenTo(string destinationPath, bool isAsync)
        {
            return OpenDestinationFile(destinationPath, isAsync);
        }

        /// <summary>
        /// オープンしているストリームをリソース解放して閉じます。
        /// </summary>
        /// <example>
        /// <code>
        /// status.Close();
        /// </code>
        /// </example>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// コンソール画面に進捗状況を出力します（<see cref="ShowProgress"/> が <c>true</c> の場合）。
        /// </summary>
        /// <example>
        /// <code>
        /// status.ShowProgress = true;
        /// status.ShowProgress();
        /// </code>
        /// </example>
        public void ShowProgress()
        {
            try
            {
                if (_showProgress && _fileSize > 0)
                {
                    // ループ回数カウンターの初期化
                    _currentCount = 0;
                    // コピー済容量(Bytes)
                    long copiedSize = _destinationStream?.Position ?? 0;
                    // コピー残容量(Bytes)
                    long remainBytes = _fileSize - copiedSize;
                    // 経過時間
                    int elapsedTime = (int)(DateTime.Now - _startTime).TotalSeconds;
                    // 完了率
                    double ratio = ((double)copiedSize / _fileSize) * 100.0;
                    // 転送速度
                    double bytesPerSec = elapsedTime > 0 ? (double)copiedSize / elapsedTime : 0.0;
                    // 残時間(秒)
                    int remainSec = bytesPerSec > 0.0 ? (int)Math.Ceiling(remainBytes / bytesPerSec) : 0;
                    // 表示文字列の構築
                    _progressLine = $"=> {ratio:000.00}% - {MdlUtil.GetHumanReadableBytes(copiedSize, 2, "0,000.00")}/{MdlUtil.GetHumanReadableBytes(_fileSize, 2, "0,000.00")} - {MdlUtil.GetHumanReadableBytes(bytesPerSec, 2, "0,000.00")}/s - Elaps={MdlDate.ConvertSecondsToTimeString(elapsedTime)}/ Remain={MdlDate.ConvertSecondsToTimeString(remainSec)}";
                    // コンソールの幅を考慮
                    if (_progressLine.Length > Console.WindowWidth) _progressLine = _progressLine[..Console.WindowWidth];
                    // 表示
                    Console.Write(_progressLine);
                    // カーソル位置を戻す
                    Console.SetCursorPosition(0, Console.CursorTop);
                }
            }
            catch { }
        }

        /// <summary>
        /// アンマネージド リソースの解放およびマネージド リソースの破棄を行います。
        /// </summary>
        /// <param name="disposing">マネージド リソースとアンマネージド リソースの両方を解放する場合は <c>true</c>。アンマネージド リソースだけを解放する場合は <c>false</c>。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _isDone = true;
                    _sourceStream?.Dispose();
                    _sourceStream = null;
                    _destinationStream?.Dispose();
                    _destinationStream = null;
                }
                _disposedValue = true;
            }
        }

        /// <summary>
        /// <see cref="ClsFsAsyncCopyStatus"/> によって使用されているすべてのリソースを解放します。
        /// </summary>
        /// <example>
        /// <code>
        /// status.Dispose();
        /// </code>
        /// </example>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 非同期にすべてのリソースを解放します。
        /// </summary>
        /// <returns>非同期の破棄操作を表す <see cref="ValueTask"/>。</returns>
        /// <example>
        /// <code>
        /// await status.DisposeAsync();
        /// </code>
        /// </example>
        public async ValueTask DisposeAsync()
        {
            if (!_disposedValue)
            {
                _isDone = true;
                if (_sourceStream is not null)
                {
                    await _sourceStream.DisposeAsync();
                    _sourceStream = null;
                }
                if (_destinationStream is not null)
                {
                    await _destinationStream.DisposeAsync();
                    _destinationStream = null;
                }
                _disposedValue = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
