@echo off
setlocal enabledelayedexpansion

set "inputFile=.gitignore"
set "tempFile=.gitignore.tmp"

if not exist "%inputFile%" (
    echo ❌ 未找到 .gitignore 文件！
    exit /b 1
)

> "%tempFile%" (
    for /f "usebackq delims=" %%a in ("%inputFile%") do (
        set "line=%%a"
        rem 判断是否以 "# Editor/Config/" 开头
        echo(!line! | findstr /b /c:"# Editor/Config/" >nul
        if !errorlevel! == 0 (
            rem 去掉前缀 "# "，恢复原规则
            set "line=!line:# Editor/Config/=Editor/Config/!"
        )
        echo(!line!
    )
)

move /y "%tempFile%" "%inputFile%" >nul
echo ✅ .gitignore 已处理完毕，相关注释已解开。
