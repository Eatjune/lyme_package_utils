namespace LymeGame.Utils.Package {
# if UNITY_EDITOR
	using UnityEngine;
	using UnityEditor;

	public static partial class PackageSettingsProvider {
		[SettingsProvider]
		public static SettingsProvider PackageProvider() {
			var packageInfo = PackageUtil.PackageInfo;

			var provider = new SettingsProvider($"Project/LymeGame/{packageInfo.DisplayName}", SettingsScope.Project) {
				label = $"{packageInfo.DisplayName}",
				guiHandler = PackageGUIHandler.GetGUIHandler
			};

			return provider;
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