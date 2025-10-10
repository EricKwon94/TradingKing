@echo off
setlocal enabledelayedexpansion
echo ===========================================
echo Deleting all 'bin' and 'obj' folders...
echo ===========================================

set deletedCount=0

for /d /r %%i in (bin,obj) do (
    if exist "%%i" (
        echo Deleting folder: %%i
        rmdir /s /q "%%i"
        set /a deletedCount=!deletedCount! + 1
    )
)

if !deletedCount! EQU 0 (
    echo No 'bin' or 'obj' folders found.
) else (
    echo ===========================================
    echo Total folders deleted: !deletedCount!
)

echo ===========================================
pause
