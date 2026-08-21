package tool.cmnclslib.cls;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.channels.FileChannel;
import java.time.Duration;
import java.time.LocalDateTime;
import tool.cmnclslib.mdl.MdlDate;
import tool.cmnclslib.mdl.MdlUtil;

/**
 * ファイルの非同期コピー状態およびストリームを管理するクラスです。
 */
public class ClsFsAsyncCopyStatus implements AutoCloseable {

    private FileInputStream sourceStream;
    private FileOutputStream destinationStream;
    private byte[] buffer = new byte[0x1000];
    private LocalDateTime startTime = LocalDateTime.now();
    private long checkCount = 256;
    private long currentCount = 0;
    private long fileSize = 0;
    private String progressLine = "";
    private String message = "";
    private String stackTrace = "";
    private boolean showProgress = false;
    private boolean isDone = false;
    private boolean isOk = true;
    private boolean disposed = false;

    /**
     * ClsFsAsyncCopyStatus クラスの新しいインスタンスを初期化します。
     *
     * @param sourcePath コピー元ファイルのパス
     * @param destinationPath コピー先ファイルのパス
     * @param isAsync 非同期モードフラグ
     */
    public ClsFsAsyncCopyStatus(String sourcePath, String destinationPath, boolean isAsync) {
        initialize(sourcePath, destinationPath, isAsync);
    }

    /**
     * ClsFsAsyncCopyStatus クラスの新しいインスタンスを初期化します（ファイル共有モード指定付き）。
     *
     * @param sourcePath コピー元ファイルのパス
     * @param destinationPath コピー先ファイルのパス
     * @param isAsync 非同期モードフラグ
     * @param fileShare ファイル共有モード
     */
    public ClsFsAsyncCopyStatus(String sourcePath, String destinationPath, boolean isAsync, int fileShare) {
        initialize(sourcePath, destinationPath, isAsync);
    }

    public FileInputStream getSourceStream() {
        return sourceStream;
    }

    public void setSourceStream(FileInputStream sourceStream) {
        this.sourceStream = sourceStream;
    }

    public FileOutputStream getDestinationStream() {
        return destinationStream;
    }

    public void setDestinationStream(FileOutputStream destinationStream) {
        this.destinationStream = destinationStream;
    }

    public byte[] getBuffer() {
        return buffer;
    }

    public void setBuffer(byte[] buffer) {
        this.buffer = buffer != null ? buffer : new byte[0x1000];
    }

    public boolean isDone() {
        return isDone;
    }

    public void setDone(boolean done) {
        isDone = done;
    }

    public LocalDateTime getStartTime() {
        return startTime;
    }

    public void setStartTime(LocalDateTime startTime) {
        this.startTime = startTime != null ? startTime : LocalDateTime.now();
    }

    public long getCheckCount() {
        return checkCount;
    }

    public void setCheckCount(long checkCount) {
        this.checkCount = checkCount;
    }

    public long getCurrentCount() {
        return currentCount;
    }

    public void setCurrentCount(long currentCount) {
        this.currentCount = currentCount;
    }

    public long getFileSize() {
        return fileSize;
    }

    public void setFileSize(long fileSize) {
        this.fileSize = fileSize;
    }

    public boolean isShowProgress() {
        return showProgress;
    }

    public void setShowProgress(boolean showProgress) {
        this.showProgress = showProgress;
    }

    public boolean isOk() {
        return isOk;
    }

    public void setOk(boolean ok) {
        isOk = ok;
    }

    public String getProgressLine() {
        return progressLine;
    }

    public void setProgressLine(String progressLine) {
        this.progressLine = progressLine != null ? progressLine : "";
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message != null ? message : "";
    }

    public String getStackTrace() {
        return stackTrace;
    }

    public void setStackTrace(String stackTrace) {
        this.stackTrace = stackTrace != null ? stackTrace : "";
    }

    /**
     * コピー元およびコピー先のファイルストリームを初期化し、コピーの準備を行います。
     *
     * @param sourcePath コピー元ファイルのパス
     * @param destinationPath コピー先ファイルのパス
     * @param isAsync 非同期モードフラグ
     * @return 初期化が成功した場合は true、失敗した場合は false
     */
    public boolean initialize(String sourcePath, String destinationPath, boolean isAsync) {
        try {
            isOk = openSourceFile(sourcePath, isAsync);
            if (isOk) {
                isOk = openDestinationFile(destinationPath, isAsync);
            }
            if (isOk && sourceStream != null) {
                File srcFile = new File(sourcePath);
                fileSize = srcFile.length();
                startTime = LocalDateTime.now();
                if (fileSize > 0) {
                    checkCount = fileSize / buffer.length / 100;
                }
                checkCount = Math.min(checkCount, 5000);
            }
        } catch (Exception e) {
            isOk = false;
        }
        return isOk;
    }

    /**
     * コピー元のファイルを開きます。
     *
     * @param sourcePath コピー元ファイルのパス
     * @param isAsync 非同期モードフラグ
     * @return 正常に開けた場合は true、失敗した場合は false
     */
    public boolean openSourceFile(String sourcePath, boolean isAsync) {
        boolean ok = true;
        if (sourcePath == null || sourcePath.isEmpty()) {
            return false;
        }
        try {
            sourceStream = new FileInputStream(sourcePath);
        } catch (Exception ex) {
            ok = false;
            message = "[ClsFsAsyncCopyStatus.OpenSourceFile(" + sourcePath + ")] " + ex.getMessage();
            stackTrace = getStackTraceStr(ex);
        }
        return ok;
    }

    /**
     * コピー先のファイルを作成・開きます。
     *
     * @param destinationPath コピー先ファイルのパス
     * @param isAsync 非同期モードフラグ
     * @return 正常に作成・開けた場合は true、失敗した場合は false
     */
    public boolean openDestinationFile(String destinationPath, boolean isAsync) {
        boolean ok = true;
        if (destinationPath == null || destinationPath.isEmpty()) {
            return false;
        }
        try {
            destinationStream = new FileOutputStream(destinationPath);
        } catch (Exception ex) {
            ok = false;
            message = "[ClsFsAsyncCopyStatus.OpenDestinationFile(" + destinationPath + ")] " + ex.getMessage();
            stackTrace = getStackTraceStr(ex);
        }
        return ok;
    }

    /**
     * コンソール画面に進捗状況を出力します。
     */
    public void showProgress() {
        try {
            if (showProgress && fileSize > 0 && destinationStream != null) {
                currentCount = 0;
                FileChannel channel = destinationStream.getChannel();
                long copiedSize = channel != null ? channel.position() : 0;
                long remainBytes = fileSize - copiedSize;
                long elapsedTime = Duration.between(startTime, LocalDateTime.now()).getSeconds();
                double ratio = ((double) copiedSize / fileSize) * 100.0;
                double bytesPerSec = elapsedTime > 0 ? (double) copiedSize / elapsedTime : 0.0;
                int remainSec = bytesPerSec > 0.0 ? (int) Math.ceil(remainBytes / bytesPerSec) : 0;

                progressLine = String.format("=> %06.2f%% - %s/%s - %s/s - Elaps=%s/ Remain=%s",
                        ratio,
                        MdlUtil.formatByteSize(copiedSize, 2, "0,000.00"),
                        MdlUtil.formatByteSize(fileSize, 2, "0,000.00"),
                        MdlUtil.formatByteSize((long) bytesPerSec, 2, "0,000.00"),
                        MdlDate.secondsToTimeString((int) elapsedTime),
                        MdlDate.secondsToTimeString(remainSec));

                System.out.print("\r" + progressLine);
            }
        } catch (Exception e) {
            // 無視
        }
    }

    /**
     * オープンしているストリームをリソース解放して閉じます。
     */
    @Override
    public void close() {
        if (!disposed) {
            isDone = true;
            if (sourceStream != null) {
                try {
                    sourceStream.close();
                } catch (IOException e) {
                    // 無視
                }
                sourceStream = null;
            }
            if (destinationStream != null) {
                try {
                    destinationStream.close();
                } catch (IOException e) {
                    // 無視
                }
                destinationStream = null;
            }
            disposed = true;
        }
    }

    /**
     * リソースを解放します。
     *
     * @deprecated {@link #close()} を使用してください。
     */
    @Deprecated
    public void dispose() {
        close();
    }

    private String getStackTraceStr(Throwable t) {
        StringBuilder sb = new StringBuilder();
        for (StackTraceElement ste : t.getStackTrace()) {
            sb.append(ste.toString()).append(System.lineSeparator());
        }
        return sb.toString();
    }
}
