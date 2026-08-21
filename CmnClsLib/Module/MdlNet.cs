using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Module
{
    /// <summary>
    /// ネットワーク関連処理（ICMP Ping、TCP Ping、DNS/IP名前解決、ネットワークインターフェース情報の取得）を提供するモジュールクラスです。
    /// </summary>
    public static class MdlNet
    {
        #region ICMP Ping

        /// <summary>
        /// 指定されたホスト名またはIPアドレスに対してICMP Pingを実行します。
        /// </summary>
        /// <param name="host">Ping送信先のホスト名またはIPアドレス文字列。</param>
        /// <param name="verbosity">詳細出力レベル（0:出力なし、1:エラー出力、2以上:詳細応答出力）。</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）。</param>
        /// <returns>Ping応答が正常に受信された場合は <c>true</c>。失敗した場合は <c>false</c>。</returns>
        /// <example>
        /// <code>
        /// bool isAlive = MdlNet.IcmpPing("127.0.0.1", verbosity: 1, timeout: 1000);
        /// </code>
        /// </example>
        public static bool IcmpPing(string host, int verbosity, int timeout)
        {
            try
            {
                using var ping = new Ping();
                PingReply reply = ping.Send(host, timeout);
                if (reply.Status == IPStatus.Success)
                {
                    if (verbosity > 1)
                    {
                        Console.WriteLine("Reply from {0}: bytes={1} time={2}ms TTL={3}",
                                reply.Address,
                                reply.Buffer.Length,
                                reply.RoundtripTime,
                                reply.Options?.Ttl);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (verbosity > 0)
                {
                    string prefix = verbosity > 1 ? "[MdlNet.IcmpPing()] " : "";
                    Console.WriteLine($"{prefix}EXCEPTION : {host} {ex.Message}");
                }
            }
            return false;
        }

        /// <summary>
        /// 指定されたホスト名またはIPアドレスに対して非同期でICMP Pingを実行します。
        /// </summary>
        /// <param name="host">Ping送信先のホスト名またはIPアドレス文字列。</param>
        /// <param name="verbosity">詳細出力レベル（0:出力なし、1:エラー出力、2以上:詳細応答出力）。</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）。</param>
        /// <param name="cancellationToken">操作のキャンセル通知を監視するキャンセルトークン。</param>
        /// <returns>Ping応答が正常に受信された場合は <c>true</c>。失敗した場合は <c>false</c> を表すタスク。</returns>
        /// <example>
        /// <code>
        /// bool isAlive = await MdlNet.IcmpPingAsync("localhost", verbosity: 0, timeout: 1000);
        /// </code>
        /// </example>
        public static async Task<bool> IcmpPingAsync(string host, int verbosity, int timeout, CancellationToken cancellationToken = default)
        {
            try
            {
                using var ping = new Ping();
                PingReply reply = await ping.SendPingAsync(host, timeout).WaitAsync(cancellationToken);
                if (reply.Status == IPStatus.Success)
                {
                    if (verbosity > 1)
                    {
                        Console.WriteLine("Reply from {0}: bytes={1} time={2}ms TTL={3}",
                                reply.Address,
                                reply.Buffer.Length,
                                reply.RoundtripTime,
                                reply.Options?.Ttl);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (verbosity > 0)
                {
                    string prefix = verbosity > 1 ? "[MdlNet.IcmpPingAsync()] " : "";
                    Console.WriteLine($"{prefix}EXCEPTION : {host} {ex.Message}");
                }
            }
            return false;
        }

        /// <summary>
        /// 指定されたIPアドレスに対してICMP Pingを実行します。
        /// </summary>
        /// <param name="ipAddress">Ping送信先のIPアドレス。</param>
        /// <param name="verbosity">詳細出力レベル（0:出力なし、1:エラー出力、2以上:詳細応答出力）。</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）。</param>
        /// <returns>Ping応答が正常に受信された場合は <c>true</c>。失敗した場合は <c>false</c>。</returns>
        /// <example>
        /// <code>
        /// IPAddress ip = IPAddress.Parse("127.0.0.1");
        /// bool isAlive = MdlNet.IcmpPing(ip, verbosity: 1, timeout: 1000);
        /// </code>
        /// </example>
        public static bool IcmpPing(IPAddress ipAddress, int verbosity, int timeout)
        {
            try
            {
                using var ping = new Ping();
                PingReply reply = ping.Send(ipAddress, timeout);
                if (reply.Status == IPStatus.Success)
                {
                    if (verbosity > 1)
                    {
                        Console.WriteLine("Reply from {0}: bytes={1} time={2}ms TTL={3}",
                                reply.Address,
                                reply.Buffer.Length,
                                reply.RoundtripTime,
                                reply.Options?.Ttl);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (verbosity > 0)
                {
                    string prefix = verbosity > 1 ? "[MdlNet.IcmpPing()] " : "";
                    Console.WriteLine($"{prefix}EXCEPTION : {ipAddress} {ex.Message}");
                }
            }
            return false;
        }

        /// <summary>
        /// 指定されたIPアドレスに対して非同期でICMP Pingを実行します。
        /// </summary>
        /// <param name="ipAddress">Ping送信先のIPアドレス。</param>
        /// <param name="verbosity">詳細出力レベル（0:出力なし、1:エラー出力、2以上:詳細応答出力）。</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）。</param>
        /// <param name="cancellationToken">操作のキャンセル通知を監視するキャンセルトークン。</param>
        /// <returns>Ping応答が正常に受信された場合は <c>true</c>。失敗した場合は <c>false</c> を表すタスク。</returns>
        /// <example>
        /// <code>
        /// IPAddress ip = IPAddress.Loopback;
        /// bool isAlive = await MdlNet.IcmpPingAsync(ip, verbosity: 0, timeout: 1000);
        /// </code>
        /// </example>
        public static async Task<bool> IcmpPingAsync(IPAddress ipAddress, int verbosity, int timeout, CancellationToken cancellationToken = default)
        {
            try
            {
                using var ping = new Ping();
                PingReply reply = await ping.SendPingAsync(ipAddress, timeout).WaitAsync(cancellationToken);
                if (reply.Status == IPStatus.Success)
                {
                    if (verbosity > 1)
                    {
                        Console.WriteLine("Reply from {0}: bytes={1} time={2}ms TTL={3}",
                                reply.Address,
                                reply.Buffer.Length,
                                reply.RoundtripTime,
                                reply.Options?.Ttl);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (verbosity > 0)
                {
                    string prefix = verbosity > 1 ? "[MdlNet.IcmpPingAsync()] " : "";
                    Console.WriteLine($"{prefix}EXCEPTION : {ipAddress} {ex.Message}");
                }
            }
            return false;
        }

        #endregion

        #region TCP Ping

        /// <summary>
        /// 指定されたホスト名またはIPアドレスおよびポート番号に対してTCP接続によるPing試行を行います。
        /// </summary>
        /// <param name="host">接続対象のホスト名またはIPアドレス文字列。</param>
        /// <param name="port">接続対象のTCPポート番号。</param>
        /// <param name="verbosity">詳細出力レベル（0:出力なし、1以上:エラー出力あり）。</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）。</param>
        /// <returns>TCP接続が正常に確立した場合は <c>true</c>。失敗した場合は <c>false</c>。</returns>
        /// <example>
        /// <code>
        /// bool isPortOpen = MdlNet.TcpPing("127.0.0.1", port: 80, verbosity: 1, timeout: 2000);
        /// </code>
        /// </example>
        public static bool TcpPing(string host, int port, int verbosity, int timeout)
        {
            try
            {
                using var tcpClient = new TcpClient();
                using var cts = new CancellationTokenSource(timeout);
                tcpClient.ConnectAsync(host, port, cts.Token).AsTask().GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                if (verbosity > 0) Console.WriteLine("[MdlNet.TcpPing()] EXCEPTION : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 指定されたホスト名またはIPアドレスおよびポート番号に対して非同期でTCP接続によるPing試行を行います。
        /// </summary>
        /// <param name="host">接続対象のホスト名またはIPアドレス文字列。</param>
        /// <param name="port">接続対象のTCPポート番号。</param>
        /// <param name="verbosity">詳細出力レベル（0:出力なし、1以上:エラー出力あり）。</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）。</param>
        /// <param name="cancellationToken">操作のキャンセル通知を監視するキャンセルトークン。</param>
        /// <returns>TCP接続が正常に確立した場合は <c>true</c>。失敗した場合は <c>false</c> を表すタスク。</returns>
        /// <example>
        /// <code>
        /// bool isPortOpen = await MdlNet.TcpPingAsync("localhost", port: 443, verbosity: 0, timeout: 2000);
        /// </code>
        /// </example>
        public static async Task<bool> TcpPingAsync(string host, int port, int verbosity, int timeout, CancellationToken cancellationToken = default)
        {
            try
            {
                using var tcpClient = new TcpClient();
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(timeout);
                await tcpClient.ConnectAsync(host, port, cts.Token);
                return true;
            }
            catch (Exception ex)
            {
                if (verbosity > 0) Console.WriteLine("[MdlNet.TcpPingAsync()] EXCEPTION : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 指定されたIPアドレスおよびポート番号に対してTCP接続によるPing試行を行います。
        /// </summary>
        /// <param name="ipAddress">接続対象のIPアドレス。</param>
        /// <param name="port">接続対象のTCPポート番号。</param>
        /// <param name="verbosity">詳細出力レベル（0:出力なし、1以上:エラー出力あり）。</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）。</param>
        /// <returns>TCP接続が正常に確立した場合は <c>true</c>。失敗した場合は <c>false</c>。</returns>
        /// <example>
        /// <code>
        /// IPAddress ip = IPAddress.Parse("192.168.1.1");
        /// bool isPortOpen = MdlNet.TcpPing(ip, port: 80, verbosity: 1, timeout: 2000);
        /// </code>
        /// </example>
        public static bool TcpPing(IPAddress ipAddress, int port, int verbosity, int timeout)
        {
            try
            {
                using var tcpClient = new TcpClient();
                using var cts = new CancellationTokenSource(timeout);
                tcpClient.ConnectAsync(ipAddress, port, cts.Token).AsTask().GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                if (verbosity > 0) Console.WriteLine("[MdlNet.TcpPing()] EXCEPTION : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 指定されたIPアドレスおよびポート番号に対して非同期でTCP接続によるPing試行を行います。
        /// </summary>
        /// <param name="ipAddress">接続対象のIPアドレス。</param>
        /// <param name="port">接続対象のTCPポート番号。</param>
        /// <param name="verbosity">詳細出力レベル（0:出力なし、1以上:エラー出力あり）。</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）。</param>
        /// <param name="cancellationToken">操作のキャンセル通知を監視するキャンセルトークン。</param>
        /// <returns>TCP接続が正常に確立した場合は <c>true</c>。失敗した場合は <c>false</c> を表すタスク。</returns>
        /// <example>
        /// <code>
        /// IPAddress ip = IPAddress.Loopback;
        /// bool isPortOpen = await MdlNet.TcpPingAsync(ip, port: 8080, verbosity: 0, timeout: 2000);
        /// </code>
        /// </example>
        public static async Task<bool> TcpPingAsync(IPAddress ipAddress, int port, int verbosity, int timeout, CancellationToken cancellationToken = default)
        {
            try
            {
                using var tcpClient = new TcpClient();
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(timeout);
                await tcpClient.ConnectAsync(ipAddress, port, cts.Token);
                return true;
            }
            catch (Exception ex)
            {
                if (verbosity > 0) Console.WriteLine("[MdlNet.TcpPingAsync()] EXCEPTION : " + ex.Message);
                return false;
            }
        }

        #endregion

        #region DNS / IP Name Resolution

        /// <summary>
        /// 指定されたIPアドレス文字列から逆引きDNS解決を行ってホスト名を取得します。
        /// </summary>
        /// <param name="ipAddress">逆引き対象のIPアドレス文字列。</param>
        /// <returns>取得されたホスト名。名前解決に失敗した場合は元の <paramref name="ipAddress"/> 文字列。</returns>
        /// <example>
        /// <code>
        /// string hostname = MdlNet.GetHostnameFromIpAddress("127.0.0.1");
        /// </code>
        /// </example>
        public static string GetHostnameFromIpAddress(string ipAddress)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
                return hostEntry.HostName;
            }
            catch
            {
                return ipAddress;
            }
        }

        /// <summary>
        /// 指定されたIPアドレス文字列から非同期で逆引きDNS解決を行ってホスト名を取得します。
        /// </summary>
        /// <param name="ipAddress">逆引き対象のIPアドレス文字列。</param>
        /// <param name="cancellationToken">操作のキャンセル通知を監視するキャンセルトークン。</param>
        /// <returns>取得されたホスト名。名前解決に失敗した場合は元の <paramref name="ipAddress"/> 文字列を表すタスク。</returns>
        /// <example>
        /// <code>
        /// string hostname = await MdlNet.GetHostnameFromIpAddressAsync("8.8.8.8");
        /// </code>
        /// </example>
        public static async Task<string> GetHostnameFromIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                IPHostEntry hostEntry = await Dns.GetHostEntryAsync(ipAddress, cancellationToken);
                return hostEntry.HostName;
            }
            catch
            {
                return ipAddress;
            }
        }

        /// <summary>
        /// 指定されたホスト名から正引きDNS解決を行って <see cref="IPAddress"/> オブジェクトを取得します。
        /// </summary>
        /// <param name="hostname">正引き対象のホスト名。</param>
        /// <param name="isIpv6">IPv6アドレスを取得する場合は <c>true</c>、IPv4を取得する場合は <c>false</c>。</param>
        /// <returns>条件に合致する最初に見つかった <see cref="IPAddress"/>。見つからない場合や失敗時は <c>null</c>。</returns>
        /// <example>
        /// <code>
        /// IPAddress? ip = MdlNet.GetIpAddressFromHostname("localhost", isIpv6: false);
        /// </code>
        /// </example>
        public static IPAddress? GetIpAddressFromHostname(string hostname, bool isIpv6)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
                AddressFamily targetFamily = isIpv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;
                return hostEntry.AddressList.FirstOrDefault(address => address.AddressFamily == targetFamily);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 指定されたホスト名から非同期で正引きDNS解決を行って <see cref="IPAddress"/> オブジェクトを取得します。
        /// </summary>
        /// <param name="hostname">正引き対象のホスト名。</param>
        /// <param name="isIpv6">IPv6アドレスを取得する場合は <c>true</c>、IPv4を取得する場合は <c>false</c>。</param>
        /// <param name="cancellationToken">操作のキャンセル通知を監視するキャンセルトークン。</param>
        /// <returns>条件に合致する最初に見つかった <see cref="IPAddress"/>。見つからない場合や失敗時は <c>null</c> を表すタスク。</returns>
        /// <example>
        /// <code>
        /// IPAddress? ip = await MdlNet.GetIpAddressFromHostnameAsync("example.com", isIpv6: true);
        /// </code>
        /// </example>
        public static async Task<IPAddress?> GetIpAddressFromHostnameAsync(string hostname, bool isIpv6, CancellationToken cancellationToken = default)
        {
            try
            {
                IPHostEntry hostEntry = await Dns.GetHostEntryAsync(hostname, cancellationToken);
                AddressFamily targetFamily = isIpv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;
                return hostEntry.AddressList.FirstOrDefault(address => address.AddressFamily == targetFamily);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 指定されたホスト名から正引きDNS解決を行って IPアドレスの文字列表現を取得します。
        /// </summary>
        /// <param name="hostname">正引き対象のホスト名。</param>
        /// <param name="isIpv6">IPv6アドレスを取得する場合は <c>true</c>、IPv4を取得する場合は <c>false</c>。</param>
        /// <returns>IPアドレスの文字列表現。取得できなかった場合は <c>null</c>。</returns>
        /// <example>
        /// <code>
        /// string? ipStr = MdlNet.GetIpStringFromHostname("localhost", isIpv6: false);
        /// </code>
        /// </example>
        public static string? GetIpStringFromHostname(string hostname, bool isIpv6)
        {
            if (string.IsNullOrEmpty(hostname)) return null;
            return GetIpAddressFromHostname(hostname, isIpv6)?.ToString();
        }

        /// <summary>
        /// 指定されたホスト名から非同期で正引きDNS解決を行って IPアドレスの文字列表現を取得します。
        /// </summary>
        /// <param name="hostname">正引き対象のホスト名。</param>
        /// <param name="isIpv6">IPv6アドレスを取得する場合は <c>true</c>、IPv4を取得する場合は <c>false</c>。</param>
        /// <param name="cancellationToken">操作のキャンセル通知を監視するキャンセルトークン。</param>
        /// <returns>IPアドレスの文字列表現。取得できなかった場合は <c>null</c> を表すタスク。</returns>
        /// <example>
        /// <code>
        /// string? ipStr = await MdlNet.GetIpStringFromHostnameAsync("localhost", isIpv6: false);
        /// </code>
        /// </example>
        public static async Task<string?> GetIpStringFromHostnameAsync(string hostname, bool isIpv6, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(hostname)) return null;
            IPAddress? ipAddress = await GetIpAddressFromHostnameAsync(hostname, isIpv6, cancellationToken);
            return ipAddress?.ToString();
        }

        #endregion

        #region Network Interface Information

        /// <summary>
        /// 端末上のネットワークインターフェースから割り当てられている IPv4 アドレスの一覧を取得します。
        /// </summary>
        /// <param name="includeDownInterfaces">非アクティブ（ダウン）なインターフェースのアドレスも含める場合は <c>true</c>。</param>
        /// <returns>取得された IPv4 アドレス文字列のリスト。</returns>
        /// <example>
        /// <code>
        /// List&lt;string&gt; ips = MdlNet.GetIpv4Addresses(includeDownInterfaces: false);
        /// </code>
        /// </example>
        public static List<string> GetIpv4Addresses(bool includeDownInterfaces)
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => includeDownInterfaces || ni.OperationalStatus == OperationalStatus.Up)
                .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
                .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(u => u.Address.ToString())
                .ToList();
        }

        #endregion

        #region Obsolete Methods (Backward Compatibility)

        /// <summary>
        /// IPアドレスをホスト名に変換します。（旧式メソッド。代わりに <see cref="GetHostnameFromIpAddress(string)"/> を使用してください）
        /// </summary>
        [Obsolete("代わりに 'GetHostnameFromIpAddress(ipAddress)' を使用します。")]
        public static string IPAddressToHostname(string ipAddress)
        {
            return GetHostnameFromIpAddress(ipAddress);
        }

        /// <summary>
        /// ホスト名をIPアドレスに変換します。（旧式メソッド。代わりに <see cref="GetIpAddressFromHostname(string, bool)"/> を使用してください）
        /// </summary>
        [Obsolete("代わりに 'GetIpAddressFromHostname(hostname, isIpv6)' を使用します。")]
        public static IPAddress? HostnameToIPAddr(string hostname, bool isIPv6)
        {
            return GetIpAddressFromHostname(hostname, isIPv6);
        }

        /// <summary>
        /// ホスト名をIPアドレス文字列に変換します。（旧式メソッド。代わりに <see cref="GetIpStringFromHostname(string, bool)"/> を使用してください）
        /// </summary>
        [Obsolete("代わりに 'GetIpStringFromHostname(hostname, isIpv6)' を使用します。")]
        public static string? HostnameToIP(string hostname, bool isIPv6)
        {
            return GetIpStringFromHostname(hostname, isIPv6);
        }

        /// <summary>
        /// IPv4アドレスのリストを取得します。（旧式メソッド。代わりに <see cref="GetIpv4Addresses(bool)"/> を使用してください）
        /// </summary>
        [Obsolete("代わりに 'GetIpv4Addresses(includeDownInterfaces)' を使用します。")]
        public static List<string> GetIpv4List(bool includeDownInterfaces)
        {
            return GetIpv4Addresses(includeDownInterfaces);
        }

        #endregion
    }
}




