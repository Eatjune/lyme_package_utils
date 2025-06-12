@echo off
setlocal

set "excludeFile=.git\info\exclude"
if not exist "%excludeFile%" (
    echo ❌ .git/info/exclude 文件不存在，确保你在 Git 仓库根目录下运行。
    exit /b 1
)

echo 添加本地忽略规则到 .git/info/exclude...

>> "%excludeFile%" (
    echo.
    echo # 本地忽略 Editor/Config 配置文件
    echo Editor/Config/*.json
    echo Editor/Config/*.meta
    echo Editor/Config/*.cs
)

echo ✅ 已添加本地忽略规则，不会影响 Git 提交。
