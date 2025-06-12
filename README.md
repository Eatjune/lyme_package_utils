# PackageUtils

## 使用方法

1. 直接复制PackageUtils文件夹,并粘贴至所需使用包的Editor根目录下
2. git clone https://github.com/Eatjune/lyme_package_utils.git

## 包含工具

- PackageDependenciesInitialize
  - 如果包需要依赖其他包，可以在 package.dependencies.json 内增加依赖项目
  - 确保PackageUtils根目录下包含package.dependencies.json文件
  - 依赖json格式样例 ：

    ```json
     {
      "name": "UniTask",
      "packageName": "com.cysharp.unitask",
      "url": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
      "namespaceName": "Cysharp.Threading.Tasks",
      "typeName": "UniTask"
    }
    ```

- PackageSettingsProvider
  - 在project settings内提供当前包的相关设置
  - <b>如需使用，复制 Template/PackageGUIHandler.cs 后修改其中的GetGUIHandler函数</b>

- PackageUtil
  - 提供包的一些工具,包括：包的相关信息等