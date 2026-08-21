using System.Buffers;
using System.Security.Cryptography;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Class
{
    public class ClsCksum
    {
        private string _errorMessage = "";
        private long _length = 0;
        private readonly int _bufferSize = 8192;
        private int _checksumValue = 0;
        private int _checksumLength = 0;
        private static readonly uint[] _crcTable =
        [
            0x00000000,
            0x04C11DB7, 0x09823B6E, 0x0D4326D9, 0x130476DC, 0x17C56B6B,
            0x1A864DB2, 0x1E475005, 0x2608EDB8, 0x22C9F00F, 0x2F8AD6D6,
            0x2B4BCB61, 0x350C9B64, 0x31CD86D3, 0x3C8EA00A, 0x384FBDBD,
            0x4C11DB70, 0x48D0C6C7, 0x4593E01E, 0x4152FDA9, 0x5F15ADAC,
            0x5BD4B01B, 0x569796C2, 0x52568B75, 0x6A1936C8, 0x6ED82B7F,
            0x639B0DA6, 0x675A1011, 0x791D4014, 0x7DDC5DA3, 0x709F7B7A,
            0x745E66CD, 0x9823B6E0, 0x9CE2AB57, 0x91A18D8E, 0x95609039,
            0x8B27C03C, 0x8FE6DD8B, 0x82A5FB52, 0x8664E6E5, 0xBE2B5B58,
            0xBAEA46EF, 0xB7A96036, 0xB3687D81, 0xAD2F2D84, 0xA9EE3033,
            0xA4AD16EA, 0xA06C0B5D, 0xD4326D90, 0xD0F37027, 0xDDB056FE,
            0xD9714B49, 0xC7361B4C, 0xC3F706FB, 0xCEB42022, 0xCA753D95,
            0xF23A8028, 0xF6FB9D9F, 0xFBB8BB46, 0xFF79A6F1, 0xE13EF6F4,
            0xE5FFEB43, 0xE8BCCD9A, 0xEC7DD02D, 0x34867077, 0x30476DC0,
            0x3D044B19, 0x39C556AE, 0x278206AB, 0x23431B1C, 0x2E003DC5,
            0x2AC12072, 0x128E9DCF, 0x164F8078, 0x1B0CA6A1, 0x1FCDBB16,
            0x018AEB13, 0x054BF6A4, 0x0808D07D, 0x0CC9CDCA, 0x7897AB07,
            0x7C56B6B0, 0x71159069, 0x75D48DDE, 0x6B93DDDB, 0x6F52C06C,
            0x6211E6B5, 0x66D0FB02, 0x5E9F46BF, 0x5A5E5B08, 0x571D7DD1,
            0x53DC6066, 0x4D9B3063, 0x495A2DD4, 0x44190B0D, 0x40D816BA,
            0xACA5C697, 0xA864DB20, 0xA527FDF9, 0xA1E6E04E, 0xBFA1B04B,
            0xBB60ADFC, 0xB6238B25, 0xB2E29692, 0x8AAD2B2F, 0x8E6C3698,
            0x832F1041, 0x87EE0DF6, 0x99A95DF3, 0x9D684044, 0x902B669D,
            0x94EA7B2A, 0xE0B41DE7, 0xE4750050, 0xE9362689, 0xEDF73B3E,
            0xF3B06B3B, 0xF771768C, 0xFA325055, 0xFEF34DE2, 0xC6BCF05F,
            0xC27DEDE8, 0xCF3ECB31, 0xCBFFD686, 0xD5B88683, 0xD1799B34,
            0xDC3ABDED, 0xD8FBA05A, 0x690CE0EE, 0x6DCDFD59, 0x608EDB80,
            0x644FC637, 0x7A089632, 0x7EC98B85, 0x738AAD5C, 0x774BB0EB,
            0x4F040D56, 0x4BC510E1, 0x46863638, 0x42472B8F, 0x5C007B8A,
            0x58C1663D, 0x558240E4, 0x51435D53, 0x251D3B9E, 0x21DC2629,
            0x2C9F00F0, 0x285E1D47, 0x36194D42, 0x32D850F5, 0x3F9B762C,
            0x3B5A6B9B, 0x0315D626, 0x07D4CB91, 0x0A97ED48, 0x0E56F0FF,
            0x1011A0FA, 0x14D0BD4D, 0x19939B94, 0x1D528623, 0xF12F560E,
            0xF5EE4BB9, 0xF8AD6D60, 0xFC6C70D7, 0xE22B20D2, 0xE6EA3D65,
            0xEBA91BBC, 0xEF68060B, 0xD727BBB6, 0xD3E6A601, 0xDEA580D8,
            0xDA649D6F, 0xC423CD6A, 0xC0E2D0DD, 0xCDA1F604, 0xC960EBB3,
            0xBD3E8D7E, 0xB9FF90C9, 0xB4BCB610, 0xB07DABA7, 0xAE3AFBA2,
            0xAAFBE615, 0xA7B8C0CC, 0xA379DD7B, 0x9B3660C6, 0x9FF77D71,
            0x92B45BA8, 0x9675461F, 0x8832161A, 0x8CF30BAD, 0x81B02D74,
            0x857130C3, 0x5D8A9099, 0x594B8D2E, 0x5408ABF7, 0x50C9B640,
            0x4E8EE645, 0x4A4FFBF2, 0x470CDD2B, 0x43CDC09C, 0x7B827D21,
            0x7F436096, 0x7200464F, 0x76C15BF8, 0x68860BFD, 0x6C47164A,
            0x61043093, 0x65C52D24, 0x119B4BE9, 0x155A565E, 0x18197087,
            0x1CD86D30, 0x029F3D35, 0x065E2082, 0x0B1D065B, 0x0FDC1BEC,
            0x3793A651, 0x3352BBE6, 0x3E119D3F, 0x3AD08088, 0x2497D08D,
            0x2056CD3A, 0x2D15EBE3, 0x29D4F654, 0xC5A92679, 0xC1683BCE,
            0xCC2B1D17, 0xC8EA00A0, 0xD6AD50A5, 0xD26C4D12, 0xDF2F6BCB,
            0xDBEE767C, 0xE3A1CBC1, 0xE760D676, 0xEA23F0AF, 0xEEE2ED18,
            0xF0A5BD1D, 0xF464A0AA, 0xF9278673, 0xFDE69BC4, 0x89B8FD09,
            0x8D79E0BE, 0x803AC667, 0x84FBDBD0, 0x9ABC8BD5, 0x9E7D9662,
            0x933EB0BB, 0x97FFAD0C, 0xAFB010B1, 0xAB710D06, 0xA6322BDF,
            0xA2F33668, 0xBCB4666D, 0xB8757BDA, 0xB5365D03, 0xB1F740B4
        ];

        /// <summary>
        /// <see cref="ClsCksum"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <example>
        /// <code>
        /// ClsCksum cksum = new ClsCksum();
        /// </code>
        /// </example>
        public ClsCksum()
        {
        }

        /// <summary>
        /// 直近の処理で発生したエラーメッセージを取得または設定します。
        /// </summary>
        /// <value>エラーメッセージ文字列。エラーがない場合は空文字列。</value>
        /// <example>
        /// <code>
        /// if (!string.IsNullOrEmpty(cksum.ErrorMessage))
        /// {
        ///     Console.WriteLine(cksum.ErrorMessage);
        /// }
        /// </code>
        /// </example>
        public string ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; } }

        /// <summary>
        /// 最後に処理したデータのバイト長を取得します。
        /// </summary>
        /// <value>処理されたストリームまたはファイルのバイト数。</value>
        /// <example>
        /// <code>
        /// long bytesProcessed = cksum.Length;
        /// </code>
        /// </example>
        public long Length { get { return _length; } }

        /// <summary>
        /// 指定されたファイルパスのファイルからデフォルトのアルゴリズム (cksum) でチェックサムを取得します。
        /// </summary>
        /// <param name="filePath">チェックサムを取得する対象ファイルのフルパス。</param>
        /// <returns>計算されたチェックサム文字列。エラーが発生した場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// ClsCksum cksum = new ClsCksum();
        /// string result = cksum.GetChecksum(@"C:\path\to\file.txt");
        /// </code>
        /// </example>
        public string GetChecksum(string filePath)
        {
            return GetChecksum(filePath, "cksum");
        }

        /// <summary>
        /// 指定されたファイルパスのファイルとアルゴリズムを使用してチェックサムを取得します。
        /// </summary>
        /// <param name="filePath">チェックサムを取得する対象ファイルのフルパス。</param>
        /// <param name="algorithm">使用するアルゴリズム ("ADLER32", "MD5", "SHA1", "SHA256", "SHA512", "cksum" 等)。</param>
        /// <returns>計算されたチェックサム文字列。エラーが発生した場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// ClsCksum cksum = new ClsCksum();
        /// string md5Hash = cksum.GetChecksum(@"C:\path\to\file.txt", "MD5");
        /// </code>
        /// </example>
        public string GetChecksum(string filePath, string algorithm)
        {
            string checksum = "";
            _errorMessage = "";
            try
            {
                using (FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    switch (algorithm.ToUpperInvariant())
                    {
                        case "ADLER32":
                        case "MD5":
                        case "SHA":
                        case "SHA1":
                        case "SHA-1":
                        case "SHA256":
                        case "SHA512":
                            checksum = GetChecksum(stream, algorithm);
                            break;
                        default:
                            checksum = GetChecksum(stream);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = "[ClsCksum.GetChecksum(" + filePath + "," + algorithm + ")] " + ex.Message;
            }
            return checksum;
        }

        /// <summary>
        /// 指定されたストリームとアルゴリズムを使用してチェックサムを取得します。
        /// </summary>
        /// <param name="stream">チェックサムを取得する読み取り可能な入力ストリーム。</param>
        /// <param name="algorithm">使用するアルゴリズム ("ADLER32", "MD5", "SHA1", "SHA256", "SHA512", "cksum" 等)。</param>
        /// <returns>計算されたチェックサム文字列。エラーが発生した場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// ClsCksum cksum = new ClsCksum();
        /// using var fs = File.OpenRead(@"C:\path\to\file.txt");
        /// string sha256Hash = cksum.GetChecksum(fs, "SHA256");
        /// </code>
        /// </example>
        public string GetChecksum(Stream stream, string algorithm)
        {
            string checksum = "";
            _length = 0;
            try
            {
                switch (algorithm.ToUpperInvariant())
                {
                    case "ADLER32":
                        checksum = (new ClsAdler32()).ComputeChecksum(stream);
                        break;
                    case "MD5":
                        checksum = Convert.ToHexStringLower(MD5.HashData(stream));
                        break;
                    case "SHA":
                    case "SHA1":
                    case "SHA-1":
                        checksum = Convert.ToHexStringLower(SHA1.HashData(stream));
                        break;
                    case "SHA256":
                        checksum = Convert.ToHexStringLower(SHA256.HashData(stream));
                        break;
                    case "SHA512":
                        checksum = Convert.ToHexStringLower(SHA512.HashData(stream));
                        break;
                    default:
                        checksum = GetChecksum(stream);
                        break;
                }
                _length = (0 == _length ? stream.Length : _length);
            }
            catch (Exception ex)
            {
                _errorMessage = "[ClsCksum.GetChecksum(Stream," + algorithm + ")] " + ex.Message;
            }
            return checksum;
        }

        /// <summary>
        /// 指定されたストリームから POSIX cksum (CRC32 互換) アルゴリズムでチェックサムを取得します。
        /// </summary>
        /// <param name="stream">チェックサムを取得する読み取り可能な入力ストリーム。</param>
        /// <returns>計算された POSIX cksum チェックサム数値文字列。エラーが発生した場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// ClsCksum cksum = new ClsCksum();
        /// using var fs = File.OpenRead(@"C:\path\to\file.txt");
        /// string posixCksum = cksum.GetChecksum(fs);
        /// </code>
        /// </example>
        public string GetChecksum(Stream stream)
        {
            string checksum = "";
            _checksumValue = 0;
            _length = 0;
            _errorMessage = "";
            byte[] buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);
            try
            {
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, _bufferSize)) > 0)
                {
                    UpdateCrcBuffer(buffer.AsSpan(0, bytesRead));
                }
                _checksumLength = (int)_length;
                for (; _checksumLength != 0; _checksumLength >>= 8)
                {
                    _checksumValue = (_checksumValue << 8) ^ (int)_crcTable[((_checksumValue >> 24) ^ _checksumLength) & 0xFF];
                }
                checksum = (~_checksumValue & 0xFFFFFFFFL).ToString();
            }
            catch (Exception ex)
            {
                _errorMessage = "[ClsCksum.GetChecksum(Stream)] " + ex.Message;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
            return checksum;
        }

        /// <summary>
        /// バッファのスパンを受け取り、内部 CRC チェックサム値を更新します。
        /// </summary>
        /// <param name="bytes">CRC 計算対象データのバイトスパン。</param>
        /// <example>
        /// <code>
        /// UpdateCrcBuffer(buffer.AsSpan(0, bytesRead));
        /// </code>
        /// </example>
        private void UpdateCrcBuffer(ReadOnlySpan<byte> bytes)
        {
            foreach (byte b in bytes)
            {
                UpdateCrcByte(b);
            }
        }

        /// <summary>
        /// 1 バイトを受け取り、内部 CRC チェックサム値およびデータ長を更新します。
        /// </summary>
        /// <param name="b">更新対象のバイトデータ。</param>
        /// <example>
        /// <code>
        /// UpdateCrcByte(0x41);
        /// </code>
        /// </example>
        private void UpdateCrcByte(byte b)
        {
            _checksumValue = (_checksumValue << 8) ^ (int)_crcTable[((_checksumValue >> 24) ^ b) & 0xFF];
            _length++;
        }
    }
}
