using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using CmnClsLib.Module;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject1.Module
{
    public class UnitTest_MdlApp
    {
        // --------------------------------------------------------------------
        // IsWindows()
        // --------------------------------------------------------------------
        [Fact]
        public void IsWindows_OS環境の判定結果を返すこと()
        {
            bool expected = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool actual = MdlApp.IsWindows();
            Assert.Equal(expected, actual);
        }

        // --------------------------------------------------------------------
        // GetOsName()
        // --------------------------------------------------------------------
        [Fact]
        public void GetOsName_OS名を返すこと()
        {
            string actual = MdlApp.GetOsName();
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.Contains(actual, new[] { "Windows", "Linux", "OSX", "FreeBSD", "UNKNOWN" });
        }

        // --------------------------------------------------------------------
        // GetProcessArchitecture()
        // --------------------------------------------------------------------
        [Fact]
        public void GetProcessArchitecture_プロセスのアーキテクチャを返すこと()
        {
            string actual = MdlApp.GetProcessArchitecture();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        // --------------------------------------------------------------------
        // GetOsArchitecture()
        // --------------------------------------------------------------------
        [Fact]
        public void GetOsArchitecture_OSのアーキテクチャを返すこと()
        {
            string actual = MdlApp.GetOsArchitecture();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        // --------------------------------------------------------------------
        // GetOsDescription()
        // --------------------------------------------------------------------
        [Fact]
        public void GetOsDescription_OSの詳細説明文字列を返すこと()
        {
            string actual = MdlApp.GetOsDescription();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        // --------------------------------------------------------------------
        // GetOsVersion()
        // --------------------------------------------------------------------
        [Fact]
        public void GetOsVersion_OSのバージョン文字列を返すこと()
        {
            string actual = MdlApp.GetOsVersion();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        // --------------------------------------------------------------------
        // GetOsMajorVersion()
        // --------------------------------------------------------------------
        [Fact]
        public void GetOsMajorVersion_OSのメジャーバージョンを返すこと()
        {
            int actual = MdlApp.GetOsMajorVersion();
            Assert.True(actual > 0);
        }

        // --------------------------------------------------------------------
        // GetPlatform()
        // --------------------------------------------------------------------
        [Fact]
        public void GetPlatform_プラットフォーム識別子を返すこと()
        {
            string actual = MdlApp.GetPlatform();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        // --------------------------------------------------------------------
        // GetReferencedAssembliesDictionary() / GetReferencedAssembliesDict()
        // --------------------------------------------------------------------
        [Fact]
        public void GetReferencedAssembliesDictionary_Nullアセンブリの場合は空の辞書を返すこと()
        {
            var actual = MdlApp.GetReferencedAssembliesDictionary(null!, "ALL");
            Assert.Empty(actual);
        }

        [Fact]
        public void GetReferencedAssembliesDictionary_ALL指定で参照アセンブリを取得できること()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var actual = MdlApp.GetReferencedAssembliesDictionary(asm, "ALL");
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void GetReferencedAssembliesDictionary_Nullまたは空文字キー指定でフィルタリング処理されること()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var actual = MdlApp.GetReferencedAssembliesDictionary(asm, "");
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetReferencedAssembliesDictionary_特定キー指定でフィルタリング処理されること()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var actual = MdlApp.GetReferencedAssembliesDictionary(asm, "b77a5c561934e089");
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetReferencedAssembliesDict_非推奨メソッドが同等の結果を返すこと()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var expected = MdlApp.GetReferencedAssembliesDictionary(asm, "ALL");
#pragma warning disable CS0618
            var actual = MdlApp.GetReferencedAssembliesDict(asm, "ALL");
#pragma warning restore CS0618
            Assert.Equal(expected.Count, actual.Count);
        }

        // --------------------------------------------------------------------
        // ランタイム / CLR 情報
        // --------------------------------------------------------------------
        [Fact]
        public void GetFrameworkDescription_フレームワークの説明文字列を返すこと()
        {
            string actual = MdlApp.GetFrameworkDescription();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        [Fact]
        public void GetRuntimeClrVersion_ランタイムCLRバージョンを返すこと()
        {
            string actual = MdlApp.GetRuntimeClrVersion();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        [Fact]
        public void GetBuildClrVersion_ビルド時CLRバージョンを返すこと()
        {
            string actual = MdlApp.GetBuildClrVersion();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        // --------------------------------------------------------------------
        // プロセス / アセンブリ 情報
        // --------------------------------------------------------------------
        [Fact]
        public void GetExeFilePath_実行ファイルのパスを返すこと()
        {
            string actual = MdlApp.GetExeFilePath();
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetExeFileVersion_実行ファイルのバージョンを返すこと()
        {
            string actual = MdlApp.GetExeFileVersion();
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetExeName_実行ファイル名を返すこと()
        {
            string actual = MdlApp.GetExeName();
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetAssemblyLocation_アセンブリのロケーションを返すこと()
        {
            string actual = MdlApp.GetAssemblyLocation();
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetAssemblyName_アセンブリ名を返すこと()
        {
            string actual = MdlApp.GetAssemblyName();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        [Fact]
        public void GetAssemblyVersion_アセンブリバージョンを返すこと()
        {
            string actual = MdlApp.GetAssemblyVersion();
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetAppName_アプリケーション名を返すこと()
        {
            string actual = MdlApp.GetAppName();
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetAppNameWithHostName_アプリ名と小文字のホスト名を結合した文字列を返すこと()
        {
            string actual = MdlApp.GetAppNameWithHostName();
            string expectedAppName = MdlApp.GetAppName();
            string expectedHostName = Environment.MachineName.ToLower();
            Assert.Equal($"{expectedAppName}_{expectedHostName}", actual);
        }

        [Fact]
        public void GetAppName_HostName_非推奨メソッドが同等の結果を返すこと()
        {
#pragma warning disable CS0618
            string actual = MdlApp.GetAppName_HostName();
#pragma warning restore CS0618
            Assert.Equal(MdlApp.GetAppNameWithHostName(), actual);
        }

        // --------------------------------------------------------------------
        // ProcessPriorityClass 関連
        // --------------------------------------------------------------------
        [Fact]
        public void GetPriorityName_優先度クラス名文字列を正しく変換すること()
        {
            Assert.Equal("BelowNormal", MdlApp.GetPriorityName(ProcessPriorityClass.BelowNormal));
            Assert.Equal("High", MdlApp.GetPriorityName(ProcessPriorityClass.High));
            Assert.Equal("Idle", MdlApp.GetPriorityName(ProcessPriorityClass.Idle));
            Assert.Equal("Normal", MdlApp.GetPriorityName(ProcessPriorityClass.Normal));
            Assert.Equal("RealTime", MdlApp.GetPriorityName(ProcessPriorityClass.RealTime));
            Assert.Equal("AboveNormal", MdlApp.GetPriorityName(ProcessPriorityClass.AboveNormal));
            Assert.Equal("AboveNormal", MdlApp.GetPriorityName((ProcessPriorityClass)999));
        }

        [Fact]
        public void GetPriorityClassFromString_文字列から優先度クラスへ正しく変換すること()
        {
            Assert.Equal(ProcessPriorityClass.Normal, MdlApp.GetPriorityClassFromString(null!));
            Assert.Equal(ProcessPriorityClass.Normal, MdlApp.GetPriorityClassFromString(""));
            Assert.Equal(ProcessPriorityClass.AboveNormal, MdlApp.GetPriorityClassFromString("abovenormal"));
            Assert.Equal(ProcessPriorityClass.AboveNormal, MdlApp.GetPriorityClassFromString("ABOVENORMAL"));
            Assert.Equal(ProcessPriorityClass.Idle, MdlApp.GetPriorityClassFromString("idle"));
            Assert.Equal(ProcessPriorityClass.High, MdlApp.GetPriorityClassFromString("high"));
            Assert.Equal(ProcessPriorityClass.RealTime, MdlApp.GetPriorityClassFromString("realtime"));
            Assert.Equal(ProcessPriorityClass.BelowNormal, MdlApp.GetPriorityClassFromString("belownormal"));
            Assert.Equal(ProcessPriorityClass.Normal, MdlApp.GetPriorityClassFromString("invalid_value"));
        }

        [Fact]
        public void GetPriorityClassFromStr_非推奨メソッドが同等の結果を返すこと()
        {
#pragma warning disable CS0618
            Assert.Equal(MdlApp.GetPriorityClassFromString("high"), MdlApp.GetPriorityClassFromStr("high"));
#pragma warning restore CS0618
        }
    }
}
