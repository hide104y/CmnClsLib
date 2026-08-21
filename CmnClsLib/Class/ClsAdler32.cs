using System;
using System.Buffers;
using System.IO;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Class
{
    /// <summary>
    /// Adler-32 チェックサム計算機能を提供するクラス。
    /// </summary>
    /// <example>
    /// <code>
    /// // インスタンスを使用したファイルチェックサムの計算例
    /// var adler = new ClsAdler32();
    /// string checksum = adler.ComputeChecksum("sample.txt");
    /// Console.WriteLine($"Adler-32: {checksum}");
    ///
    /// // 静的メソッドを使用した Span からの高速計算例
    /// byte[] data = System.Text.Encoding.UTF8.GetBytes("Wikipedia");
    /// uint result = ClsAdler32.ComputeChecksum(data);
    /// Console.WriteLine($"Adler-32 (uint): {result}");
    /// </code>
    /// </example>
    public class ClsAdler32
    {
        // 定数
        private const uint AdlerBase = 65521;
        private const int MaxUnreducedBytes = 5552;

        // 変数
        private string _errorMessage = "";
        private long _length = 0;
        private int _bufferSize = 8192;

        /// <summary>
        /// <see cref="ClsAdler32"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <example>
        /// <code>
        /// var adler32 = new ClsAdler32();
        /// </code>
        /// </example>
        public ClsAdler32()
        {
        }

        /// <summary>
        /// 最後に発生したエラーメッセージを取得または設定します。
        /// </summary>
        /// <value>エラーメッセージ文字列。エラーがない場合は空文字列。</value>
        /// <example>
        /// <code>
        /// var adler = new ClsAdler32();
        /// string checksum = adler.ComputeChecksum("invalid_path.txt");
        /// if (!string.IsNullOrEmpty(adler.ErrorMessage))
        /// {
        ///     Console.WriteLine($"エラー: {adler.ErrorMessage}");
        /// }
        /// </code>
        /// </example>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => _errorMessage = value;
        }

        /// <summary>
        /// 最後にチェックサムを計算した対象データのバイト長を取得します。
        /// </summary>
        /// <value>データ長（バイト単位）。</value>
        /// <example>
        /// <code>
        /// var adler = new ClsAdler32();
        /// string checksum = adler.ComputeChecksum("sample.txt");
        /// Console.WriteLine($"処理データ長: {adler.Length} bytes");
        /// </code>
        /// </example>
        public long Length => _length;

        /// <summary>
        /// 指定されたファイルパスのファイルから Adler-32 チェックサム文字列を取得します。
        /// （互換性のために残されています。新規コードでは <see cref="ComputeChecksum(string)"/> を使用してください）
        /// </summary>
        /// <param name="filePath">チェックサムを計算するファイルの完全パスまたは相対パス。</param>
        /// <returns>10進数表現の Adler-32 チェックサム文字列。エラー時は空文字列。</returns>
        /// <example>
        /// <code>
        /// var adler = new ClsAdler32();
        /// string checksum = adler.GetChecksum("test.dat");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ComputeChecksum(string filePath)' を使用します。")]
        public string GetChecksum(string filePath)
        {
            return ComputeChecksum(filePath);
        }

        /// <summary>
        /// 指定されたストリームから Adler-32 チェックサム文字列を取得します。
        /// （互換性のために残されています。新規コードでは <see cref="ComputeChecksum(Stream)"/> を使用してください）
        /// </summary>
        /// <param name="stream">チェックサムを計算する入力ストリーム。</param>
        /// <returns>10進数表現の Adler-32 チェックサム文字列。エラー時は空文字列。</returns>
        /// <example>
        /// <code>
        /// using var ms = new MemoryStream(Encoding.UTF8.GetBytes("Hello World"));
        /// var adler = new ClsAdler32();
        /// string checksum = adler.GetChecksum(ms);
        /// </code>
        /// </example>
        [Obsolete("代わりに 'ComputeChecksum(Stream stream)' を使用します。")]
        public string GetChecksum(Stream stream)
        {
            return ComputeChecksum(stream);
        }

        /// <summary>
        /// 指定されたファイルパスのファイルから Adler-32 チェックサム文字列を計算します。
        /// </summary>
        /// <param name="filePath">チェックサムを計算するファイルのパス。</param>
        /// <returns>計算された Adler-32 チェックサム（10進数文字列）。処理に失敗した場合は空文字列が返り、<see cref="ErrorMessage"/> に詳細が設定されます。</returns>
        /// <example>
        /// <code>
        /// var adler = new ClsAdler32();
        /// string checksum = adler.ComputeChecksum(@"C:\data\input.bin");
        /// Console.WriteLine($"Checksum: {checksum}");
        /// </code>
        /// </example>
        public string ComputeChecksum(string filePath)
        {
            string checksum = "";

            try
            {
                using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                checksum = ComputeChecksum(stream);
            }
            catch (Exception ex)
            {
                _errorMessage = $"[Adler32.ComputeChecksum({filePath})] {ex.Message}";
            }

            return checksum;
        }

        /// <summary>
        /// 指定されたストリームの全データから Adler-32 チェックサム文字列を計算します。
        /// </summary>
        /// <param name="stream">チェックサムの計算対象となる入力ストリーム。</param>
        /// <returns>計算された Adler-32 チェックサム（10進数文字列）。処理中に例外が発生した場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// var adler = new ClsAdler32();
        /// using var fileStream = File.OpenRead("data.csv");
        /// string checksum = adler.ComputeChecksum(fileStream);
        /// </code>
        /// </example>
        public string ComputeChecksum(Stream stream)
        {
            string checksum = "";
            _errorMessage = "";

            byte[] buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);
            try
            {
                uint adler = 1;
                int bytesRead;

                while ((bytesRead = stream.Read(buffer.AsSpan(0, _bufferSize))) > 0)
                {
                    adler = ComputeChecksum(buffer.AsSpan(0, bytesRead), adler);
                }

                checksum = adler.ToString();
                _length = stream.Length;
            }
            catch (Exception ex)
            {
                _errorMessage = $"[Adler32.ComputeChecksum(Stream)] {ex.Message}";
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return checksum;
        }

        /// <summary>
        /// バイトスパンデータから直接 Adler-32 チェックサムを高速に計算します。
        /// </summary>
        /// <param name="data">チェックサムを計算するバイトデータスパン。</param>
        /// <param name="initialAdler">計算に使用する初期 Adler-32 値（デフォルトは 1）。継続計算時に以前の値を指定します。</param>
        /// <returns>32 ビット符号なし整数（<see cref="uint"/>）形式の Adler-32 チェックサム値。</returns>
        /// <example>
        /// <code>
        /// byte[] data = Encoding.UTF8.GetBytes("123456789");
        /// uint checksum = ClsAdler32.ComputeChecksum(data);
        /// // 123456789 の Adler-32 値は 0x091E01DE (152961502)
        /// Console.WriteLine($"Checksum (hex): {checksum:X8}");
        /// </code>
        /// </example>
        public static uint ComputeChecksum(ReadOnlySpan<byte> data, uint initialAdler = 1)
        {
            uint s1 = initialAdler & 0xffff;
            uint s2 = (initialAdler >> 16) & 0xffff;

            int remaining = data.Length;
            int index = 0;

            while (remaining > 0)
            {
                int chunkSize = Math.Min(remaining, MaxUnreducedBytes);
                remaining -= chunkSize;

                ReadOnlySpan<byte> chunk = data.Slice(index, chunkSize);
                index += chunkSize;

                foreach (byte b in chunk)
                {
                    s1 += b;
                    s2 += s1;
                }

                s1 %= AdlerBase;
                s2 %= AdlerBase;
            }

            return (s2 << 16) | s1;
        }
    }
}
