using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

// 2026/08/08 Gemini 3.6 Flash (High) Review & Modified

namespace CmnClsLib.Module
{
    public static class MdlApp
    {
        /// <summary>
        /// 現在の実行環境のOSが Windows であるかどうかを判定します。
        /// </summary>
        /// <returns>Windows の場合は <c>true</c>。それ以外の場合は <c>false</c>。</returns>
        /// <example>
        /// <code>
        /// bool isWindows = MdlApp.IsWindows();
        /// // 戻り値: true (Windows環境の場合)
        /// </code>
        /// </example>
        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        /// <summary>
        /// 現在実行中のオペレーティングシステム（OS）の名前を取得します。
        /// </summary>
        /// <returns>OS名（"Windows", "Linux", "OSX", "FreeBSD", または "UNKNOWN"）。</returns>
        /// <example>
        /// <code>
        /// string osName = MdlApp.GetOsName();
        /// // 戻り値例: "Windows"
        /// </code>
        /// </example>
        public static string GetOsName()
        {
            if (OperatingSystem.IsWindows()) return "Windows";
            if (OperatingSystem.IsLinux()) return "Linux";
            if (OperatingSystem.IsMacOS()) return "OSX";
            if (OperatingSystem.IsFreeBSD()) return "FreeBSD";
            return "UNKNOWN";
        }

        /// <summary>
        /// 現在実行中のプロセスのアーキテクチャを取得します。
        /// </summary>
        /// <returns>プロセスのアーキテクチャ名（例: "X64", "Arm64"）。</returns>
        /// <example>
        /// <code>
        /// string processArch = MdlApp.GetProcessArchitecture();
        /// // 戻り値例: "X64"
        /// </code>
        /// </example>
        public static string GetProcessArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture.ToString();
        }

        /// <summary>
        /// オペレーティングシステム（OS）のアーキテクチャを取得します。
        /// </summary>
        /// <returns>OSのアーキテクチャ名（例: "X64", "Arm64"）。</returns>
        /// <example>
        /// <code>
        /// string osArch = MdlApp.GetOsArchitecture();
        /// // 戻り値例: "X64"
        /// </code>
        /// </example>
        public static string GetOsArchitecture()
        {
            return RuntimeInformation.OSArchitecture.ToString();
        }

        /// <summary>
        /// オペレーティングシステム（OS）の詳細な説明文字列を取得します。
        /// </summary>
        /// <returns>OSの詳細説明文字列（例: "Microsoft Windows 10.0.22621"）。</returns>
        /// <example>
        /// <code>
        /// string osDesc = MdlApp.GetOsDescription();
        /// // 戻り値例: "Microsoft Windows 10.0.22621"
        /// </code>
        /// </example>
        public static string GetOsDescription()
        {
            return RuntimeInformation.OSDescription;
        }

        /// <summary>
        /// オペレーティングシステム（OS）のバージョン文字列を取得します。
        /// </summary>
        /// <returns>OSのバージョン文字列（例: "Microsoft Windows NT 10.0.22621.0"）。</returns>
        /// <example>
        /// <code>
        /// string osVersion = MdlApp.GetOsVersion();
        /// // 戻り値例: "Microsoft Windows NT 10.0.22621.0"
        /// </code>
        /// </example>
        public static string GetOsVersion()
        {
            return Environment.OSVersion.VersionString;
        }

        /// <summary>
        /// オペレーティングシステム（OS）のメジャーバージョン番号を取得します。
        /// </summary>
        /// <returns>OSのメジャーバージョン番号（例: 10）。</returns>
        /// <example>
        /// <code>
        /// int majorVer = MdlApp.GetOsMajorVersion();
        /// // 戻り値例: 10
        /// </code>
        /// </example>
        public static int GetOsMajorVersion()
        {
            return Environment.OSVersion.Version.Major;
        }

        /// <summary>
        /// 現在のプラットフォーム識別子を取得します。
        /// </summary>
        /// <returns>プラットフォーム識別文字列（例: "Win32NT"）。</returns>
        /// <example>
        /// <code>
        /// string platform = MdlApp.GetPlatform();
        /// // 戻り値例: "Win32NT"
        /// </code>
        /// </example>
        public static string GetPlatform()
        {
            return Environment.OSVersion.Platform.ToString();
        }

        /// <summary>
        /// 指定されたアセンブリが参照しているアセンブリの辞書を取得します。
        /// </summary>
        /// <param name="assembly">対象のアセンブリ。</param>
        /// <param name="publicKeyToken">フィルタリング条件（"ALL" で全件、null または空文字でトークン無し、または指定されたトークン文字列）。</param>
        /// <returns>インデックスと参照アセンブリ名のソート済み辞書。</returns>
        /// <example>
        /// <code>
        /// var assembly = Assembly.GetExecutingAssembly();
        /// var dict = MdlApp.GetReferencedAssembliesDictionary(assembly, "ALL");
        /// </code>
        /// </example>
        public static SortedDictionary<int, AssemblyName> GetReferencedAssembliesDictionary(Assembly assembly, string publicKeyToken)
        {
            SortedDictionary<int, AssemblyName> referencedAssemblies = new SortedDictionary<int, AssemblyName>();
            if (assembly == null) return referencedAssemblies;

            try
            {
                AssemblyName[] assemblies = assembly.GetReferencedAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    bool shouldAdd = false;
                    AssemblyName assemblyName = assemblies[i];

                    if (string.IsNullOrEmpty(publicKeyToken))
                    {
                        if (assemblyName.FullName != null && assemblyName.FullName.Contains("PublicKeyToken=null", StringComparison.OrdinalIgnoreCase))
                        {
                            shouldAdd = true;
                        }
                    }
                    else if (string.Equals(publicKeyToken, "ALL", StringComparison.OrdinalIgnoreCase))
                    {
                        shouldAdd = true;
                    }
                    else
                    {
                        if (assemblyName.FullName != null && assemblyName.FullName.Contains("PublicKeyToken=" + publicKeyToken, StringComparison.OrdinalIgnoreCase))
                        {
                            shouldAdd = true;
                        }
                    }

                    if (shouldAdd)
                    {
                        referencedAssemblies.Add(i, assemblyName);
                    }
                }
            }
            catch
            {
                // 例外発生時は取得できた範囲を返す
            }
            return referencedAssemblies;
        }

        /// <summary>
        /// 指定されたアセンブリが参照しているアセンブリの辞書を取得します。（非推奨）
        /// </summary>
        /// <param name="assembly">対象のアセンブリ。</param>
        /// <param name="publicKeyToken">フィルタリング条件。</param>
        /// <returns>インデックスと参照アセンブリ名のソート済み辞書。</returns>
        /// <example>
        /// <code>
        /// var assembly = Assembly.GetExecutingAssembly();
        /// var dict = MdlApp.GetReferencedAssembliesDict(assembly, "ALL");
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetReferencedAssembliesDictionary(Assembly, string)' を使用します。")]
        public static SortedDictionary<int, AssemblyName> GetReferencedAssembliesDict(Assembly assembly, string publicKeyToken)
        {
            return GetReferencedAssembliesDictionary(assembly, publicKeyToken);
        }

        /// <summary>
        /// 現在実行中の.NETフレームワークの説明文字列を取得します。
        /// </summary>
        /// <returns>フレームワークの説明文字列（例: ".NET 10.0.0"）。</returns>
        /// <example>
        /// <code>
        /// string fwDesc = MdlApp.GetFrameworkDescription();
        /// // 戻り値例: ".NET 10.0.0"
        /// </code>
        /// </example>
        public static string GetFrameworkDescription()
        {
            return RuntimeInformation.FrameworkDescription;
        }

        /// <summary>
        /// 現在実行中のランタイムCLRのバージョンを取得します。
        /// </summary>
        /// <returns>CLRバージョン文字列（例: "v4.0.30319"）。</returns>
        /// <example>
        /// <code>
        /// string clrVer = MdlApp.GetRuntimeClrVersion();
        /// // 戻り値例: "v4.0.30319"
        /// </code>
        /// </example>
        public static string GetRuntimeClrVersion()
        {
            return RuntimeEnvironment.GetSystemVersion();
        }

        /// <summary>
        /// アセンブリのビルド時のCLRバージョンを取得します。
        /// </summary>
        /// <returns>ビルド時CLRバージョン文字列。</returns>
        /// <example>
        /// <code>
        /// string buildClrVer = MdlApp.GetBuildClrVersion();
        /// // 戻り値例: "v4.0.30319"
        /// </code>
        /// </example>
        public static string GetBuildClrVersion()
        {
            return Assembly.GetExecutingAssembly().ImageRuntimeVersion;
        }

        /// <summary>
        /// 現在のプロセスの実行ファイル（.exe）のフルパスを取得します。
        /// </summary>
        /// <returns>実行ファイルのフルパス。取得できない場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string exePath = MdlApp.GetExeFilePath();
        /// // 戻り値例: "C:\App\MyApp.exe"
        /// </code>
        /// </example>
        public static string GetExeFilePath()
        {
            Process currentProcess = Process.GetCurrentProcess();
            return currentProcess?.MainModule?.FileName ?? "";
        }

        /// <summary>
        /// 現在のプロセスの実行ファイルのファイルバージョン文字列を取得します。
        /// </summary>
        /// <returns>ファイルバージョン文字列（例: "1.0.0.0"）。取得できない場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string exeVersion = MdlApp.GetExeFileVersion();
        /// // 戻り値例: "1.0.0.0"
        /// </code>
        /// </example>
        public static string GetExeFileVersion()
        {
            string version = "";
            Process currentProcess = Process.GetCurrentProcess();
            if (currentProcess != null)
            {
                FileVersionInfo? versionInfo = currentProcess.MainModule?.FileVersionInfo;
                if (versionInfo != null) version = versionInfo.FileVersion?.ToString() ?? "";
            }
            return version;
        }

        /// <summary>
        /// 現在のプロセスの実行ファイル名（拡張子なし）を取得します。
        /// </summary>
        /// <returns>実行ファイル名（拡張子なし）。</returns>
        /// <example>
        /// <code>
        /// string exeName = MdlApp.GetExeName();
        /// // 戻り値例: "MyApp"
        /// </code>
        /// </example>
        public static string GetExeName()
        {
            return Path.GetFileNameWithoutExtension(GetExeFilePath());
        }

        /// <summary>
        /// 現在実行中のアセンブリの場所（パス）を取得します。
        /// </summary>
        /// <returns>アセンブリのファイルパス。取得できない場合は空文字列。</returns>
        /// <example>
        /// <code>
        /// string asmLoc = MdlApp.GetAssemblyLocation();
        /// // 戻り値例: "C:\App\CmnClsLib.dll"
        /// </code>
        /// </example>
        public static string GetAssemblyLocation()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly?.Location ?? "";
        }

        /// <summary>
        /// 現在実行中のアセンブリの名前（拡張子なし）を取得します。
        /// </summary>
        /// <returns>アセンブリ名。</returns>
        /// <example>
        /// <code>
        /// string asmName = MdlApp.GetAssemblyName();
        /// // 戻り値例: "CmnClsLib"
        /// </code>
        /// </example>
        public static string GetAssemblyName()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return Path.GetFileNameWithoutExtension(assembly?.GetName()?.Name) ?? "";
        }

        /// <summary>
        /// 現在実行中のアセンブリのバージョン文字列を取得します。
        /// </summary>
        /// <returns>アセンブリのバージョン文字列（例: "1.0.0.0"）。</returns>
        /// <example>
        /// <code>
        /// string asmVer = MdlApp.GetAssemblyVersion();
        /// // 戻り値例: "1.0.0.0"
        /// </code>
        /// </example>
        public static string GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "";
        }

        /// <summary>
        /// アプリケーション名（実行ファイル名から末尾の数字を除去したもの）を取得します。
        /// </summary>
        /// <returns>整形されたアプリケーション名。</returns>
        /// <example>
        /// <code>
        /// string appName = MdlApp.GetAppName();
        /// // 戻り値例: "MyApp"
        /// </code>
        /// </example>
        public static string GetAppName()
        {
            return MdlUtil.TrimNumberRight(MdlApp.GetExeName());
        }

        /// <summary>
        /// アプリケーション名と小文字に変換されたホスト名を結合した文字列を取得します。
        /// </summary>
        /// <returns>"アプリケーション名_ホスト名" の形式の文字列。</returns>
        /// <example>
        /// <code>
        /// string appHostName = MdlApp.GetAppNameWithHostName();
        /// // 戻り値例: "MyApp_mycomputer"
        /// </code>
        /// </example>
        public static string GetAppNameWithHostName()
        {
            return GetAppName() + "_" + Environment.MachineName.ToLower();
        }

        /// <summary>
        /// アプリケーション名とホスト名を結合した文字列を取得します。（非推奨）
        /// </summary>
        /// <returns>"アプリケーション名_ホスト名" の形式の文字列。</returns>
        /// <example>
        /// <code>
        /// string appHostName = MdlApp.GetAppName_HostName();
        /// // 戻り値例: "MyApp_mycomputer"
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetAppNameWithHostName()' を使用します。")]
        public static string GetAppName_HostName()
        {
            return GetAppNameWithHostName();
        }

        /// <summary>
        /// プロセスの優先度クラスに対応する名前文字列を取得します。
        /// </summary>
        /// <param name="priorityClass">プロセスの優先度クラス。</param>
        /// <returns>優先度クラスの名前文字列（例: "Normal", "High"）。</returns>
        /// <example>
        /// <code>
        /// string? name = MdlApp.GetPriorityName(ProcessPriorityClass.High);
        /// // 戻り値: "High"
        /// </code>
        /// </example>
        public static string? GetPriorityName(ProcessPriorityClass priorityClass)
        {
            return priorityClass switch
            {
                ProcessPriorityClass.BelowNormal => "BelowNormal",
                ProcessPriorityClass.High => "High",
                ProcessPriorityClass.Idle => "Idle",
                ProcessPriorityClass.Normal => "Normal",
                ProcessPriorityClass.RealTime => "RealTime",
                _ => "AboveNormal",
            };
        }

        /// <summary>
        /// 文字列からプロセスの優先度クラス列挙型を取得します。
        /// </summary>
        /// <param name="priority">優先度クラスを表す文字列（例: "normal", "high"）。</param>
        /// <returns>対応する <see cref="ProcessPriorityClass"/>。該当しない場合は <see cref="ProcessPriorityClass.Normal"/>。</returns>
        /// <example>
        /// <code>
        /// var priority = MdlApp.GetPriorityClassFromString("high");
        /// // 戻り値: ProcessPriorityClass.High
        /// </code>
        /// </example>
        public static ProcessPriorityClass GetPriorityClassFromString(string priority)
        {
            if (string.IsNullOrWhiteSpace(priority)) return ProcessPriorityClass.Normal;

            return priority.ToLowerInvariant() switch
            {
                "abovenormal" => ProcessPriorityClass.AboveNormal,
                "idle" => ProcessPriorityClass.Idle,
                "high" => ProcessPriorityClass.High,
                "realtime" => ProcessPriorityClass.RealTime,
                "belownormal" => ProcessPriorityClass.BelowNormal,
                _ => ProcessPriorityClass.Normal,
            };
        }

        /// <summary>
        /// 文字列からプロセスの優先度クラス列挙型を取得します。（非推奨）
        /// </summary>
        /// <param name="priority">優先度クラスを表す文字列。</param>
        /// <returns>対応する <see cref="ProcessPriorityClass"/>。</returns>
        /// <example>
        /// <code>
        /// var priority = MdlApp.GetPriorityClassFromStr("high");
        /// // 戻り値: ProcessPriorityClass.High
        /// </code>
        /// </example>
        [Obsolete("代わりに 'GetPriorityClassFromString(string)' を使用します。")]
        public static ProcessPriorityClass GetPriorityClassFromStr(string priority)
        {
            return GetPriorityClassFromString(priority);
        }
    }
}
