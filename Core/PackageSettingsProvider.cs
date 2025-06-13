namespace LymeGame.Utils.Package {
# if UNITY_EDITOR
	using UnityEngine;
	using UnityEditor;

	public static partial class PackageSettingsProvider {
		[SettingsProvider]
		public static SettingsProvider PackageProvider() {
			var packageInfo = PackageUtil.PackageInfo;
			if (packageInfo.PackageJson == null) {
				return null;
			}

			var provider = new SettingsProvider($"Project/LymeGame/{packageInfo.DisplayName}", SettingsScope.Project) {
				label = $"{packageInfo.DisplayName}",
				guiHandler = OnPackageGUIHandler
			};

			return provider;
		}

		public static void OnPackageGUIHandler(string searchContext) {
			var packageInfo = PackageUtil.PackageInfo;
			if (packageInfo.PackageJson == null) {
				return;
			}

			#region 包基础信息

			GUILayout.Label($"包名 : {packageInfo.PackageName}", EditorStyles.boldLabel);
			GUILayout.Label($"包路径 : {packageInfo.FilePath}", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			var hasInit = EditorPrefs.GetBool(packageInfo.PackageName, false) ? '是' : '否';
			GUILayout.Label($"初始化 : {hasInit}", EditorStyles.boldLabel);

			if (GUILayout.Button("重置初始化", GUILayout.Width(100))) {
				if (EditorPrefs.GetBool(packageInfo.PackageName, false)) {
					EditorPrefs.DeleteKey(packageInfo.PackageName);
				}
			}

			GUILayout.EndHorizontal();

			#endregion

			GUILayout.Space(10);

			#region 依赖管理

			GUILayout.BeginHorizontal();

			GUILayout.Label("依赖管理", EditorStyles.boldLabel);
			if (GUILayout.Button("导入所有依赖", GUILayout.Width(100))) {
				PackageDependenciesInitialize.ReImport(true);
			}

			GUILayout.EndHorizontal();

			PackageDependenciesInitialize.RequireUPMCheckRequest();

			if (PackageDependenciesInitialize.UPMCheckRequest is {IsCompleted: true}) {
				var items = PackageDependenciesInitialize.GetDependencyItems();
				foreach (var dependencyItem in items) {
					var found = false;

					if (!dependencyItem.assetStorePackage) {
						foreach (var package in PackageDependenciesInitialize.UPMCheckRequest.Result) {
							if (package.name == dependencyItem.packageName) {
								found = true;
								break;
							}
						}
					}

					if (!found) {
						found = PackageDependenciesInitialize.HasTypeInNamespace(dependencyItem.namespaceName, dependencyItem.typeName);
					}

					GUILayout.BeginHorizontal();
					var hasImport = found ? "<color=green>已导入</color>" : "<color=red>未导入</color>";
					GUILayout.Label($"{dependencyItem.name}  {hasImport}", new GUIStyle(GUI.skin.label) {richText = true});
					if (GUILayout.Button("导入", GUILayout.Width(100))) {
						PackageDependenciesInitialize.ImportPackageDependency(dependencyItem);
					}

					GUILayout.EndHorizontal();
				}
			}

			#endregion

			//包自定义设置
			PackageGUIHandler.GetGUIHandler(searchContext);
		}

		[SettingsProvider]
		public static SettingsProvider DefaultRootProvider() {
			var rootProvider = new SettingsProvider($"Project/LymeGame", SettingsScope.Project) {
				label = $"LymeGame Studio",
				guiHandler = (searchContext) => {
					GUILayout.Label($"© 2025 [Lyme Game]. All rights reserved.", EditorStyles.boldLabel);
				}
			};

			return rootProvider;
		}
	}
#endif
}