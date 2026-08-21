package tool.cmnclslib.mdl;

import java.net.InetAddress;
import java.net.ServerSocket;
import java.util.List;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 * MdlNet の単体テストクラスです。
 */
public class UnitTest_MdlNet {

    @Test
    public void icmpPing_HostString_Loopback() {
        boolean result = MdlNet.icmpPing("127.0.0.1", 0, 2000);
        assertTrue(result);
    }

    @Test
    public void icmpPing_HostString_InvalidHost_ReturnsFalse() {
        boolean result = MdlNet.icmpPing("invalid_host_name_999999", 1, 500);
        assertFalse(result);
    }

    @Test
    public void icmpPingAsync_HostString_Loopback() throws Exception {
        boolean result = MdlNet.icmpPingAsync("127.0.0.1", 2, 2000).get();
        assertTrue(result);
    }

    @Test
    public void icmpPingAsync_HostString_InvalidHost_ReturnsFalse() throws Exception {
        boolean result = MdlNet.icmpPingAsync("invalid_host_name_999999", 0, 500).get();
        assertFalse(result);
    }

    @Test
    public void icmpPing_InetAddress_Loopback() throws Exception {
        boolean result = MdlNet.icmpPing(InetAddress.getByName("127.0.0.1"), 1, 2000);
        assertTrue(result);
    }

    @Test
    public void icmpPingAsync_InetAddress_Loopback() throws Exception {
        boolean result = MdlNet.icmpPingAsync(InetAddress.getByName("127.0.0.1"), 1, 2000).get();
        assertTrue(result);
    }

    @Test
    public void tcpPing_HostString_SuccessAndFailure() throws Exception {
        int port;
        try (ServerSocket serverSocket = new ServerSocket(0, 1, InetAddress.getByName("127.0.0.1"))) {
            port = serverSocket.getLocalPort();
            boolean success = MdlNet.tcpPing("127.0.0.1", port, 1, 2000);
            assertTrue(success);
        }

        boolean fail = MdlNet.tcpPing("127.0.0.1", port, 1, 500);
        assertFalse(fail);
    }

    @Test
    public void tcpPingAsync_HostString_SuccessAndFailure() throws Exception {
        int port;
        try (ServerSocket serverSocket = new ServerSocket(0, 1, InetAddress.getByName("127.0.0.1"))) {
            port = serverSocket.getLocalPort();
            boolean success = MdlNet.tcpPingAsync("127.0.0.1", port, 0, 2000).get();
            assertTrue(success);
        }

        boolean fail = MdlNet.tcpPingAsync("127.0.0.1", port, 1, 500).get();
        assertFalse(fail);
    }

    @Test
    public void getHostnameFromIpAddress_Loopback_ReturnsHostnameOrIp() {
        String hostname = MdlNet.getHostnameFromIpAddress("127.0.0.1");
        assertFalse(hostname.isEmpty());
    }

    @Test
    public void getHostnameFromIpAddress_InvalidIp_ReturnsOriginalString() {
        String invalidIp = "999.999.999.999";
        String hostname = MdlNet.getHostnameFromIpAddress(invalidIp);
        assertEquals(invalidIp, hostname);
    }

    @Test
    public void getIpAddressFromHostname_Localhost_IPv4() {
        InetAddress ip = MdlNet.getIpAddressFromHostname("localhost", false);
        assertNotNull(ip);
    }

    @Test
    public void getIpAddressFromHostname_InvalidHost_ReturnsNull() {
        InetAddress ip = MdlNet.getIpAddressFromHostname("invalid_host_name_999999", false);
        assertNull(ip);
    }

    @Test
    public void getIpStringFromHostname_Localhost() {
        String ipStr = MdlNet.getIpStringFromHostname("localhost", false);
        assertNotNull(ipStr);
    }

    @Test
    public void getIpStringFromHostname_NullOrEmptyHost_ReturnsNull() {
        String ipStr1 = MdlNet.getIpStringFromHostname(null, false);
        String ipStr2 = MdlNet.getIpStringFromHostname("", false);
        assertNull(ipStr1);
        assertNull(ipStr2);
    }

    @Test
    public void getIpv4Addresses_ReturnsList() {
        List<String> addressesIncludeDown = MdlNet.getIpv4Addresses(true);
        List<String> addressesActiveOnly = MdlNet.getIpv4Addresses(false);

        assertNotNull(addressesIncludeDown);
        assertNotNull(addressesActiveOnly);
    }

    @Test
    public void 新設メソッド_getHostFromIp_getIpFromHost等が動作すること() {
        assertEquals("127.0.0.1", MdlNet.getHostFromIp("127.0.0.1"));
        assertNotNull(MdlNet.getHostFromIpAsync("127.0.0.1"));
        assertNotNull(MdlNet.getIpFromHost("localhost", false));
        assertNotNull(MdlNet.getIpFromHostAsync("localhost", false));
        assertNotNull(MdlNet.getIpStrFromHost("localhost", false));
        assertNotNull(MdlNet.getIpStrHostAsync("localhost", false));
    }
}
