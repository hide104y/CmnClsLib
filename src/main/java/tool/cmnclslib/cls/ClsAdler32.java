package tool.cmnclslib.cls;

import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;

/**
 * Adler-32 チェックサム計算機能を提供するクラスです。
 */
public class ClsAdler32 {

    private static final long ADLER_BASE = 65521L;
    private static final int MAX_UNREDUCED_BYTES = 5552;

    private String errorMessage = "";
    private long length = 0;
    private int bufferSize = 8192;

    /**
     * バイト配列から直接 Adler-32 チェックサムを計算します。
     *
     * @param data チェックサムを計算するバイト配列
     * @param offset 開始オフセット
     * @param length 計算対象の長さ
     * @param initialAdler 初期 Adler-32 値（デフォルトは 1）
     * @return 符号なし32ビット整数値（long）形式の Adler-32 チェックサム値
     */
    public static long computeChecksum(byte[] data, int offset, int length, long initialAdler) {
        if (data == null || length <= 0) {
            return initialAdler;
        }

        long s1 = initialAdler & 0xffffL;
        long s2 = (initialAdler >> 16) & 0xffffL;

        int remaining = length;
        int index = offset;

        while (remaining > 0) {
            int chunkSize = Math.min(remaining, MAX_UNREDUCED_BYTES);
            remaining -= chunkSize;

            for (int i = 0; i < chunkSize; i++) {
                s1 += (data[index++] & 0xff);
                s2 += s1;
            }

            s1 %= ADLER_BASE;
            s2 %= ADLER_BASE;
        }

        return ((s2 << 16) | s1) & 0xffffffffL;
    }

    /**
     * バイト配列全体から Adler-32 チェックサムを計算します。
     *
     * @param data チェックサムを計算するバイト配列
     * @return 符号なし32ビット整数値（long）形式の Adler-32 チェックサム値
     */
    public static long computeChecksum(byte[] data) {
        return computeChecksum(data, 0, data != null ? data.length : 0, 1L);
    }

    /**
     * ClsAdler32 クラスの新しいインスタンスを初期化します。
     */
    public ClsAdler32() {
    }

    /**
     * 最後に発生したエラーメッセージを取得します。
     *
     * @return エラーメッセージ文字列。エラーがない場合は空文字列
     */
    public String getErrorMessage() {
        return errorMessage;
    }

    /**
     * 最後に発生したエラーメッセージを設定します。
     *
     * @param errorMessage エラーメッセージ文字列
     */
    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage != null ? errorMessage : "";
    }

    /**
     * 最後にチェックサムを計算した対象データのバイト長を取得します。
     *
     * @return データ長（バイト単位）
     */
    public long getLength() {
        return length;
    }

    /**
     * 指定されたファイルパスのファイルから Adler-32 チェックサム文字列を計算します。
     *
     * @param filePath チェックサムを計算するファイルのパス
     * @return 計算された Adler-32 チェックサム（10進数文字列）。処理に失敗した場合は空文字列
     */
    public String computeChecksum(String filePath) {
        String checksum = "";
        try {
            java.nio.file.Path path = java.nio.file.Paths.get(filePath);
            try (InputStream stream = java.nio.file.Files.newInputStream(path)) {
                checksum = computeChecksum(stream);
                this.length = java.nio.file.Files.size(path);
            }
        } catch (Exception ex) {
            this.errorMessage = "[Adler32.ComputeChecksum(" + filePath + ")] " + ex.getMessage();
        }
        return checksum;
    }

    /**
     * 指定されたストリームの全データから Adler-32 チェックサム文字列を計算します。
     *
     * @param stream チェックサムの計算対象となる入力ストリーム
     * @return 計算された Adler-32 チェックサム（10進数文字列）。処理中に例外が発生した場合は空文字列
     */
    public String computeChecksum(InputStream stream) {
        String checksum = "";
        this.errorMessage = "";
        this.length = 0;

        byte[] buffer = new byte[bufferSize];
        try {
            long adler = 1L;
            int bytesRead;
            long totalBytes = 0;

            while ((bytesRead = stream.read(buffer, 0, bufferSize)) > 0) {
                adler = computeChecksum(buffer, 0, bytesRead, adler);
                totalBytes += bytesRead;
            }

            checksum = Long.toString(adler);
            this.length = totalBytes;
        } catch (Exception ex) {
            this.errorMessage = "[Adler32.ComputeChecksum(InputStream)] " + ex.getMessage();
        }

        return checksum;
    }
}
