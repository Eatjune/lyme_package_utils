namespace LymeGame.Utils.Package {
#if UNITY_EDITOR
	using System;
	using System.IO;
	using System.Runtime.CompilerServices;
	using UnityEditor;
	using UnityEngine;

	public static partial class PackageUtil {
		/// <summary>
		/// 插件配置目录,供用户使用
		/// </summary>
		public static string PackageConfigPath = "lyme_package_utils/Config/package.dependencies.json";

		/// <summary>
		/// 所在包信息
		/// </summary>
		public static PackageUtils_LocationInfo PackageInfo {
			get {
				if (m_packageInfo == null) {
					m_packageInfo = new PackageUtils_LocationInfo();
				}

				return m_packageInfo;
			}
		}

		private static PackageUtils_LocationInfo m_packageInfo;

		[InitializeOnLoad]
		public static class PackageUtil_Initialize {
			static PackageUtil_Initialize() {
			}
		}

		/// <summary>
		/// 是否在assets目录下，仅开发模式
		/// </summary>
		public static bool InAssets() {
			return PackageInfo.PackageJson == null;
		}

		public static bool HasInit() {
			var packageInfo = PackageInfo;
			if (!EditorPrefs.GetBool(packageInfo.PackageName, false)) {
				EditorPrefs.SetBool(packageInfo.PackageName, true);
				Debug.Log($"{packageInfo.DisplayName} : {packageInfo.PackageName} 初始化成功...");
				return false;
			}

			return true;
		}

		[Serializable]
		public class PackageUtils_LocationInfo {
			public SimplePackageJson PackageJson;
			public string PackageName;
			public string DisplayName;
			public string FilePath;

			public PackageUtils_LocationInfo() {
				PackageJson = GetPackageJson();
				if (PackageJson == null) return;
				PackageName = PackageJson.name;
				DisplayName = PackageJson.displayName;
			}

			public SimplePackageJson GetPackageJson([CallerFilePath] string callerPath = "") {
				var fullPath = Path.GetFullPath(callerPath);
				var dir = new DirectoryInfo(Path.GetDirectoryName(fullPath));

				while (dir != null) {
					var packageJsonPath = Path.Combine(dir.FullName, "package.json");
					if (File.Exists(packageJsonPath)) {
						var json = File.ReadAllText(packageJsonPath);
						var package = JsonUtility.FromJson<SimplePackageJson>(json);
						FilePath = dir.FullName;
						return package;
					}

					dir = dir.Parent;
				}

				Debug.LogWarning("未找到 package.json");
				return null;
			}

			[Serializable]
			public class SimplePackageJson {
				public string name;
				public string displayName;
			}
		}
	}
}
#endif