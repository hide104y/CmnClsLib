package tool.cmnclslib.mdl;

import java.io.IOException;
import java.net.Inet4Address;
import java.net.Inet6Address;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.NetworkInterface;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Enumeration;
import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * ネットワーク関連処理（ICMP Ping、TCP Ping、DNS/IP名前解決、ネットワークインターフェース情報の取得）を提供するモジュールクラスです。
 */
public final class MdlNet {

    private MdlNet() {
        // インスタンス化防止
    }

    /**
     * 指定されたホスト名またはIPアドレスに対してICMP Pingを実行します。
     *
     * @param host Ping送信先のホスト名またはIPアドレス文字列
     * @param verbosity 詳細出力レベル（0:出力なし、1:エラー出力、2以上:詳細応答出力）
     * @param timeout タイムアウト時間（ミリ秒）
     * @return Ping応答が正常に受信された場合は true、失敗した場合は false
     */
    public static boolean icmpPing(String host, int verbosity, int timeout) {
        try {
            InetAddress address = InetAddress.getByName(host);
            boolean reachable = address.isReachable(timeout);
            if (reachable && verbosity > 1) {
                System.out.println("Reply from " + address.getHostAddress() + " isReachable=true");
            }
            return reachable;
        } catch (Exception ex) {
            if (verbosity > 0) {
                String prefix = verbosity > 1 ? "[MdlNet.icmpPing()] " : "";
                System.out.println(prefix + "EXCEPTION : " + host + " " + ex.getMessage());
            }
            return false;
        }
    }

    /**
     * 指定されたホスト名またはIPアドレスに対して非同期でICMP Pingを実行します。
     *
     * @param host Ping送信先のホスト名またはIPアドレス文字列
     * @param verbosity 詳細出力レベル
     * @param timeout タイムアウト時間（ミリ秒）
     * @return Ping結果の CompletableFuture
     */
    public static CompletableFuture<Boolean> icmpPingAsync(String host, int verbosity, int timeout) {
        return CompletableFuture.supplyAsync(() -> icmpPing(host, verbosity, timeout));
    }

    /**
     * 指定されたIPアドレスに対してICMP Pingを実行します。
     *
     * @param ipAddress Ping送信先の InetAddress
     * @param verbosity 詳細出力レベル
     * @param timeout タイムアウト時間（ミリ秒）
     * @return Ping応答が正常に受信された場合は true、失敗した場合は false
     */
    public static boolean icmpPing(InetAddress ipAddress, int verbosity, int timeout) {
        if (ipAddress == null) {
            return false;
        }
        try {
            boolean reachable = ipAddress.isReachable(timeout);
            if (reachable && verbosity > 1) {
                System.out.println("Reply from " + ipAddress.getHostAddress() + " isReachable=true");
            }
            return reachable;
        } catch (Exception ex) {
            if (verbosity > 0) {
                String prefix = verbosity > 1 ? "[MdlNet.icmpPing()] " : "";
                System.out.println(prefix + "EXCEPTION : " + ipAddress + " " + ex.getMessage());
            }
            return false;
        }
    }

    /**
     * 指定されたIPアドレスに対して非同期でICMP Pingを実行します。
     *
     * @param ipAddress Ping送信先の InetAddress
     * @param verbosity 詳細出力レベル
     * @param timeout タイムアウト時間（ミリ秒）
     * @return Ping結果の CompletableFuture
     */
    public static CompletableFuture<Boolean> icmpPingAsync(InetAddress ipAddress, int verbosity, int timeout) {
        return CompletableFuture.supplyAsync(() -> icmpPing(ipAddress, verbosity, timeout));
    }

    /**
     * 指定されたホスト名またはIPアドレスおよびポート番号に対してTCP接続によるPing試行を行います。
     *
     * @param host 接続対象のホスト名またはIPアドレス文字列
     * @param port 接続対象のTCPポート番号
     * @param verbosity 詳細出力レベル（0:出力なし、1以上:エラー出力あり）
     * @param timeout タイムアウト時間（ミリ秒）
     * @return TCP接続が正常に確立した場合は true、失敗した場合は false
     */
    public static boolean tcpPing(String host, int port, int verbosity, int timeout) {
        try (Socket socket = new Socket()) {
            socket.connect(new InetSocketAddress(host, port), timeout);
            return true;
        } catch (Exception ex) {
            if (verbosity > 0) {
                System.out.println("[MdlNet.tcpPing()] EXCEPTION : " + ex.getMessage());
            }
            return false;
        }
    }

    /**
     * 指定されたホスト名またはIPアドレスおよびポート番号に対して非同期でTCP接続によるPing試行を行います。
     *
     * @param host 接続対象のホスト名またはIPアドレス文字列
     * @param port 接続対象のTCPポート番号
     * @param verbosity 詳細出力レベル
     * @param timeout タイムアウト時間（ミリ秒）
     * @return TCP Ping結果の CompletableFuture
     */
    public static CompletableFuture<Boolean> tcpPingAsync(String host, int port, int verbosity, int timeout) {
        return CompletableFuture.supplyAsync(() -> tcpPing(host, port, verbosity, timeout));
    }

    /**
     * 指定されたIPアドレスおよびポート番号に対してTCP接続によるPing試行を行います。
     *
     * @param ipAddress 接続対象のIPアドレス
     * @param port 接続対象のTCPポート番号
     * @param verbosity 詳細出力レベル
     * @param timeout タイムアウト時間（ミリ秒）
     * @return TCP接続が正常に確立した場合は true、失敗した場合は false
     */
    public static boolean tcpPing(InetAddress ipAddress, int port, int verbosity, int timeout) {
        if (ipAddress == null) {
            return false;
        }
        return tcpPing(ipAddress.getHostAddress(), port, verbosity, timeout);
    }

    /**
     * 指定されたIPアドレスおよびポート番号に対して非同期でTCP接続によるPing試行を行います。
     *
     * @param ipAddress 接続対象のIPアドレス
     * @param port 接続対象のTCPポート番号
     * @param verbosity 詳細出力レベル
     * @param timeout タイムアウト時間（ミリ秒）
     * @return TCP Ping結果の CompletableFuture
     */
    public static CompletableFuture<Boolean> tcpPingAsync(InetAddress ipAddress, int port, int verbosity, int timeout) {
        return CompletableFuture.supplyAsync(() -> tcpPing(ipAddress, port, verbosity, timeout));
    }

    /**
     * 指定されたIPアドレス文字列から逆引きDNS解決を行ってホスト名を取得します。
     *
     * @param ipAddress 逆引き対象のIPアドレス文字列
     * @return 取得されたホスト名。名前解決に失敗した場合は元の ipAddress 文字列
     */
    public static String getHostFromIp(String ipAddress) {
        if (ipAddress == null || ipAddress.isEmpty()) {
            return "";
        }
        try {
            InetAddress addr = InetAddress.getByName(ipAddress);
            return addr.getHostName();
        } catch (Exception e) {
            return ipAddress;
        }
    }

    /**
     * @deprecated {@link #getHostFromIp(String)} を使用してください。
     */
    @Deprecated
    public static String getHostnameFromIpAddress(String ipAddress) {
        return getHostFromIp(ipAddress);
    }

    /**
     * 指定されたIPアドレス文字列から非同期で逆引きDNS解決を行ってホスト名を取得します。
     *
     * @param ipAddress 逆引き対象のIPアドレス文字列
     * @return ホスト名取得結果の CompletableFuture
     */
    public static CompletableFuture<String> getHostFromIpAsync(String ipAddress) {
        return CompletableFuture.supplyAsync(() -> getHostFromIp(ipAddress));
    }

    /**
     * @deprecated {@link #getHostFromIpAsync(String)} を使用してください。
     */
    @Deprecated
    public static CompletableFuture<String> getHostnameFromIpAddressAsync(String ipAddress) {
        return getHostFromIpAsync(ipAddress);
    }

    /**
     * 指定されたホスト名から正引きDNS解決を行って InetAddress オブジェクトを取得します。
     *
     * @param hostname 正引き対象のホスト名
     * @param isIpv6 IPv6アドレスを取得する場合は true、IPv4を取得する場合は false
     * @return 条件に合致する最初に見つかった InetAddress。見つからない場合や失敗時は null
     */
    public static InetAddress getIpFromHost(String hostname, boolean isIpv6) {
        if (hostname == null || hostname.isEmpty()) {
            return null;
        }
        try {
            InetAddress[] addresses = InetAddress.getAllByName(hostname);
            for (InetAddress addr : addresses) {
                if (isIpv6 && addr instanceof Inet6Address) {
                    return addr;
                }
                if (!isIpv6 && addr instanceof Inet4Address) {
                    return addr;
                }
            }
        } catch (UnknownHostException e) {
            return null;
        }
        return null;
    }

    /**
     * @deprecated {@link #getIpFromHost(String, boolean)} を使用してください。
     */
    @Deprecated
    public static InetAddress getIpAddressFromHostname(String hostname, boolean isIpv6) {
        return getIpFromHost(hostname, isIpv6);
    }

    /**
     * 指定されたホスト名から非同期で正引きDNS解決を行って InetAddress オブジェクトを取得します。
     *
     * @param hostname 正引き対象のホスト名
     * @param isIpv6 IPv6アドレスを取得する場合は true、IPv4を取得する場合は false
     * @return 正引き結果の CompletableFuture
     */
    public static CompletableFuture<InetAddress> getIpFromHostAsync(String hostname, boolean isIpv6) {
        return CompletableFuture.supplyAsync(() -> getIpFromHost(hostname, isIpv6));
    }

    /**
     * @deprecated {@link #getIpFromHostAsync(String, boolean)} を使用してください。
     */
    @Deprecated
    public static CompletableFuture<InetAddress> getIpAddressFromHostnameAsync(String hostname, boolean isIpv6) {
        return getIpFromHostAsync(hostname, isIpv6);
    }

    /**
     * 指定されたホスト名から正引きDNS解決を行って IPアドレスの文字列表現を取得します。
     *
     * @param hostname 正引き対象のホスト名
     * @param isIpv6 IPv6アドレスを取得する場合は true、IPv4を取得する場合は false
     * @return IPアドレスの文字列表現。取得できなかった場合は null
     */
    public static String getIpStrFromHost(String hostname, boolean isIpv6) {
        InetAddress addr = getIpFromHost(hostname, isIpv6);
        return addr != null ? addr.getHostAddress() : null;
    }

    /**
     * @deprecated {@link #getIpStrFromHost(String, boolean)} を使用してください。
     */
    @Deprecated
    public static String getIpStringFromHostname(String hostname, boolean isIpv6) {
        return getIpStrFromHost(hostname, isIpv6);
    }

    /**
     * 指定されたホスト名から非同期で正引きDNS解決を行って IPアドレスの文字列表現を取得します。
     *
     * @param hostname 正引き対象のホスト名
     * @param isIpv6 IPv6アドレスを取得する場合は true、IPv4を取得する場合は false
     * @return IP文字列取得結果の CompletableFuture
     */
    public static CompletableFuture<String> getIpStrHostAsync(String hostname, boolean isIpv6) {
        return CompletableFuture.supplyAsync(() -> getIpStrFromHost(hostname, isIpv6));
    }

    /**
     * @deprecated {@link #getIpStrHostAsync(String, boolean)} を使用してください。
     */
    @Deprecated
    public static CompletableFuture<String> getIpStringFromHostnameAsync(String hostname, boolean isIpv6) {
        return getIpStrHostAsync(hostname, isIpv6);
    }

    /**
     * 端末上のネットワークインターフェースから割り当てられている IPv4 アドレスの一覧を取得します。
     *
     * @param includeDownInterfaces 非アクティブ（ダウン）なインターフェースのアドレスも含める場合は true
     * @return 取得された IPv4 アドレス文字列のリスト
     */
    public static List<String> getIpv4Addresses(boolean includeDownInterfaces) {
        List<String> result = new ArrayList<>();
        try {
            Enumeration<NetworkInterface> interfaces = NetworkInterface.getNetworkInterfaces();
            if (interfaces == null) {
                return result;
            }
            for (NetworkInterface nif : Collections.list(interfaces)) {
                if (includeDownInterfaces || nif.isUp()) {
                    Enumeration<InetAddress> addresses = nif.getInetAddresses();
                    for (InetAddress addr : Collections.list(addresses)) {
                        if (addr instanceof Inet4Address) {
                            result.add(addr.getHostAddress());
                        }
                    }
                }
            }
        } catch (Exception e) {
            // エラー時は取得できた分を返す
        }
        return result;
    }
}
