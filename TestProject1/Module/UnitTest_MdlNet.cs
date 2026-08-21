using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CmnClsLib.Module;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Module
{
    public class UnitTest_MdlNet
    {
        #region ICMP Ping Tests

        [Theory]
        [InlineData("127.0.0.1", 0)]
        [InlineData("127.0.0.1", 1)]
        [InlineData("127.0.0.1", 2)]
        public void IcmpPing_HostString_Loopback(string host, int verbosity)
        {
            bool result = MdlNet.IcmpPing(host, verbosity, timeout: 2000);
            Assert.True(result);
        }

        [Fact]
        public void IcmpPing_HostString_InvalidHost_ReturnsFalse()
        {
            bool result = MdlNet.IcmpPing("invalid_host_name_999999", verbosity: 1, timeout: 500);
            Assert.False(result);
        }

        [Fact]
        public async Task IcmpPingAsync_HostString_Loopback()
        {
            bool result = await MdlNet.IcmpPingAsync("127.0.0.1", verbosity: 2, timeout: 2000);
            Assert.True(result);
        }

        [Fact]
        public async Task IcmpPingAsync_HostString_InvalidHost_ReturnsFalse()
        {
            bool result = await MdlNet.IcmpPingAsync("invalid_host_name_999999", verbosity: 0, timeout: 500);
            Assert.False(result);
        }

        [Fact]
        public void IcmpPing_IPAddress_Loopback()
        {
            bool result = MdlNet.IcmpPing(IPAddress.Loopback, verbosity: 1, timeout: 2000);
            Assert.True(result);
        }

        [Fact]
        public async Task IcmpPingAsync_IPAddress_Loopback()
        {
            bool result = await MdlNet.IcmpPingAsync(IPAddress.Loopback, verbosity: 1, timeout: 2000);
            Assert.True(result);
        }

        #endregion

        #region TCP Ping Tests

        [Fact]
        public void TcpPing_HostString_SuccessAndFailure()
        {
            // 有効なローカルTCPリスナーを起動してテスト
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            try
            {
                bool success = MdlNet.TcpPing("127.0.0.1", port, verbosity: 1, timeout: 2000);
                Assert.True(success);
            }
            finally
            {
                listener.Stop();
            }

            // 停止後のポートへ接続（失敗するはず）
            bool fail = MdlNet.TcpPing("127.0.0.1", port, verbosity: 1, timeout: 500);
            Assert.False(fail);
        }

        [Fact]
        public async Task TcpPingAsync_HostString_SuccessAndFailure()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            try
            {
                bool success = await MdlNet.TcpPingAsync("127.0.0.1", port, verbosity: 0, timeout: 2000);
                Assert.True(success);
            }
            finally
            {
                listener.Stop();
            }

            bool fail = await MdlNet.TcpPingAsync("127.0.0.1", port, verbosity: 1, timeout: 500);
            Assert.False(fail);
        }

        [Fact]
        public void TcpPing_IPAddress_SuccessAndFailure()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            try
            {
                bool success = MdlNet.TcpPing(IPAddress.Loopback, port, verbosity: 1, timeout: 2000);
                Assert.True(success);
            }
            finally
            {
                listener.Stop();
            }

            bool fail = MdlNet.TcpPing(IPAddress.Loopback, port, verbosity: 1, timeout: 500);
            Assert.False(fail);
        }

        [Fact]
        public async Task TcpPingAsync_IPAddress_SuccessAndFailure()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            try
            {
                bool success = await MdlNet.TcpPingAsync(IPAddress.Loopback, port, verbosity: 1, timeout: 2000);
                Assert.True(success);
            }
            finally
            {
                listener.Stop();
            }

            bool fail = await MdlNet.TcpPingAsync(IPAddress.Loopback, port, verbosity: 1, timeout: 500);
            Assert.False(fail);
        }

        #endregion

        #region DNS / IP Name Resolution Tests

        [Fact]
        public void GetHostnameFromIpAddress_Loopback_ReturnsHostnameOrIp()
        {
            string hostname = MdlNet.GetHostnameFromIpAddress("127.0.0.1");
            Assert.False(string.IsNullOrEmpty(hostname));
        }

        [Fact]
        public void GetHostnameFromIpAddress_InvalidIp_ReturnsOriginalString()
        {
            string invalidIp = "999.999.999.999";
            string hostname = MdlNet.GetHostnameFromIpAddress(invalidIp);
            Assert.Equal(invalidIp, hostname);
        }

        [Fact]
        public async Task GetHostnameFromIpAddressAsync_Loopback_ReturnsHostnameOrIp()
        {
            string hostname = await MdlNet.GetHostnameFromIpAddressAsync("127.0.0.1");
            Assert.False(string.IsNullOrEmpty(hostname));
        }

        [Fact]
        public async Task GetHostnameFromIpAddressAsync_InvalidIp_ReturnsOriginalString()
        {
            string invalidIp = "999.999.999.999";
            string hostname = await MdlNet.GetHostnameFromIpAddressAsync(invalidIp);
            Assert.Equal(invalidIp, hostname);
        }

        [Fact]
        public void GetIpAddressFromHostname_Localhost_IPv4()
        {
            IPAddress? ip = MdlNet.GetIpAddressFromHostname("localhost", isIpv6: false);
            Assert.NotNull(ip);
            Assert.Equal(AddressFamily.InterNetwork, ip.AddressFamily);
        }

        [Fact]
        public void GetIpAddressFromHostname_InvalidHost_ReturnsNull()
        {
            IPAddress? ip = MdlNet.GetIpAddressFromHostname("invalid_host_name_999999", isIpv6: false);
            Assert.Null(ip);
        }

        [Fact]
        public async Task GetIpAddressFromHostnameAsync_Localhost_IPv4()
        {
            IPAddress? ip = await MdlNet.GetIpAddressFromHostnameAsync("localhost", isIpv6: false);
            Assert.NotNull(ip);
            Assert.Equal(AddressFamily.InterNetwork, ip.AddressFamily);
        }

        [Fact]
        public async Task GetIpAddressFromHostnameAsync_InvalidHost_ReturnsNull()
        {
            IPAddress? ip = await MdlNet.GetIpAddressFromHostnameAsync("invalid_host_name_999999", isIpv6: false);
            Assert.Null(ip);
        }

        [Fact]
        public void GetIpStringFromHostname_Localhost()
        {
            string? ipStr = MdlNet.GetIpStringFromHostname("localhost", isIpv6: false);
            Assert.NotNull(ipStr);
            Assert.True(IPAddress.TryParse(ipStr, out _));
        }

        [Fact]
        public void GetIpStringFromHostname_NullOrEmptyHost_ReturnsNull()
        {
            string? ipStr1 = MdlNet.GetIpStringFromHostname(null!, isIpv6: false);
            string? ipStr2 = MdlNet.GetIpStringFromHostname("", isIpv6: false);
            Assert.Null(ipStr1);
            Assert.Null(ipStr2);
        }

        [Fact]
        public async Task GetIpStringFromHostnameAsync_Localhost()
        {
            string? ipStr = await MdlNet.GetIpStringFromHostnameAsync("localhost", isIpv6: false);
            Assert.NotNull(ipStr);
            Assert.True(IPAddress.TryParse(ipStr, out _));
        }

        [Fact]
        public async Task GetIpStringFromHostnameAsync_NullOrEmptyHost_ReturnsNull()
        {
            string? ipStr1 = await MdlNet.GetIpStringFromHostnameAsync(null!, isIpv6: false);
            string? ipStr2 = await MdlNet.GetIpStringFromHostnameAsync("", isIpv6: false);
            Assert.Null(ipStr1);
            Assert.Null(ipStr2);
        }

        #endregion

        #region Network Interface Information Tests

        [Fact]
        public void GetIpv4Addresses_ReturnsList()
        {
            List<string> addressesIncludeDown = MdlNet.GetIpv4Addresses(includeDownInterfaces: true);
            List<string> addressesActiveOnly = MdlNet.GetIpv4Addresses(includeDownInterfaces: false);

            Assert.NotNull(addressesIncludeDown);
            Assert.NotNull(addressesActiveOnly);

            foreach (var ipStr in addressesActiveOnly)
            {
                Assert.True(IPAddress.TryParse(ipStr, out var ip));
                Assert.Equal(AddressFamily.InterNetwork, ip.AddressFamily);
            }
        }

        #endregion

        #region Obsolete Methods Tests

#pragma warning disable CS0618

        [Fact]
        public void Obsolete_IPAddressToHostname()
        {
            string hostname = MdlNet.IPAddressToHostname("127.0.0.1");
            Assert.False(string.IsNullOrEmpty(hostname));
        }

        [Fact]
        public void Obsolete_HostnameToIPAddr()
        {
            IPAddress? ip = MdlNet.HostnameToIPAddr("localhost", isIPv6: false);
            Assert.NotNull(ip);
        }

        [Fact]
        public void Obsolete_HostnameToIP()
        {
            string? ipStr = MdlNet.HostnameToIP("localhost", isIPv6: false);
            Assert.NotNull(ipStr);
        }

        [Fact]
        public void Obsolete_GetIpv4List()
        {
            List<string> list = MdlNet.GetIpv4List(includeDownInterfaces: true);
            Assert.NotNull(list);
        }

#pragma warning restore CS0618

        #endregion
    }
}
