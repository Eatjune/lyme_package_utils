namespace LymeGame.Utils.Package {
#if UNITY_EDITOR
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEditor.PackageManager;
	using UnityEditor.PackageManager.Requests;
	using UnityEngine;

	/// <summary>
	/// 读取包内package.dependencies内所有的依赖并询问用户是否加载
	/// </summary>
	public static partial class PackageDependenciesInitialize {
		public static ListRequest UPMCheckRequest;
		private static ListRequest m_UPMCheckRequest;
		private static bool hasChecked = false;
		private static bool force = false;

		public static void ReImport(bool _force = false) {
			force = _force;
			UPMCheckRequest = Client.List(true);
			// 在编辑器更新后执行（等待 Unity 编译完成）
			EditorApplication.update += RunOnce;
		}

		/// <summary>
		/// 发送UPMListRequest 【异步】
		/// </summary>
		public static void RequireUPMCheckRequest() {
			if (UPMCheckRequest == null) {
				UPMCheckRequest = Client.List(true);
			}
		}

		public static List<DependencyItem> GetDependencyItems() {
			var items = new List<DependencyItem>();
			var packageRootPath = PackageUtil.PackageInfo.FilePath;
			var jsonPath = $"{packageRootPath}/{PackageUtil.PackageConfigPath}";
			if (!File.Exists(jsonPath)) {
				Debug.LogWarning($"依赖配置文件未找到：{jsonPath}");
				return items;
			}

			var json = File.ReadAllText(jsonPath);
			var wrapper = JsonUtility.FromJson<DependencyWrapper>(json);
			return wrapper.dependencies;
		}

		private static void RunOnce() {
			if (hasChecked) return;
			if (!UPMCheckRequest.IsCompleted) return;
			// 等待编译完成
			if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;

			// 取消注册，防止每次进入PlayMode都运行
			EditorApplication.update -= RunOnce;
			hasChecked = true;

			var packageRootPath = PackageUtil.PackageInfo.FilePath;
			var jsonPath = $"{packageRootPath}/{PackageUtil.PackageConfigPath}";
			if (!File.Exists(jsonPath)) {
				Debug.LogWarning($"依赖配置文件未找到：{jsonPath}");
				return;
			}

			try {
				var json = File.ReadAllText(jsonPath);
				var wrapper = JsonUtility.FromJson<DependencyWrapper>(json);

				if (wrapper.dependencies.Count > 0) {
					foreach (var dep in wrapper.dependencies) {
						CheckUniTask(dep);
					}
				}
			}
			catch (Exception e) {
				Debug.LogError($"解析依赖配置文件出错: {e.Message}");
			}
		}

		[InitializeOnLoad]
		public static class UniTaskDependencyChecker {
			static UniTaskDependencyChecker() {
				if (PackageUtil.InAssets()) return;
				if (PackageUtil.HasInit()) return;
				ReImport();
			}
		}

		private static void CheckUniTask(DependencyItem item) {
			if (force) {
				Client.Add(item.url);
				Debug.LogError($"正在从 {item.url} 拉取：{item.name},请勿重新编译...");
				return;
			}

			var found = false;
			foreach (var package in UPMCheckRequest.Result) {
				if (package.name == item.packageName) {
					found = true;
					break;
				}
			}

			if (!found) {
				found = HasTypeInNamespace(item.namespaceName, item.typeName);
			}

			if (!found) {
				var userAgreed = EditorUtility.DisplayDialog($"缺少依赖：{item.name}", $"检测到未安装 {item.name}。是否现在通过 UPM 自动安装最新版 {item.name}？", "安装", "取消");

				if (userAgreed) {
					Client.Add(item.url);
					Debug.LogError($"正在从 {item.url} 拉取：{item.name},请勿重新编译...");
				}
			} else {
				Debug.Log($"项目内已包含依赖:{item.name}");
			}
		}

		/// <summary>
		/// 检查当前项目里，是否存在指定命名空间下的某个类型
		/// </summary>
		/// <param name="namespaceName">命名空间，比如 "Cysharp.Threading.Tasks"</param>
		/// <param name="typeName">类型名，比如 "UniTask"</param>
		/// <returns>是否找到该类型</returns>
		public static bool HasTypeInNamespace(string namespaceName, string typeName) {
			// 遍历当前 AppDomain 中所有已加载的程序集
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assembly in assemblies) {
				// 获取该程序集下所有类型
				var types = assembly.GetTypes();

				foreach (var type in types) {
					if (type.Namespace == namespaceName && type.Name == typeName) {
						return true;
					}
				}
			}

			return false;
		}

		[Serializable]
		public class DependencyWrapper {
			public List<DependencyItem> dependencies;
		}

		[Serializable]
		public class DependencyItem {
			public string name;
			public string packageName;
			public string url;
			public string namespaceName;
			public string typeName;
		}
	}
#endif
}