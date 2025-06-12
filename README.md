# PackageUtils

## 使用方法

1. 引入PackageUtils，**请勿修改库名称,且请放入Editor目录下**
   1. 下载本repo，复制粘贴至所需使用包的Editor根目录下
   2. 在Editor根目录下执行命令 *git clone https://github.com/Eatjune/lyme_package_utils.git*
2. 执行RUNME.bat文件（作用：忽略本地config下所有文件的记录，防止冲突）

## 注意事项
- 请勿修改Core内代码，防止冲突

## 工具目录

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
  - <b>如需使用，直接修改 Config/PackageGUIHandler.cs 中的 GetGUIHandler 函数</b>

- PackageUtil
  - 提供包的一些工具,包括：包的相关信息等