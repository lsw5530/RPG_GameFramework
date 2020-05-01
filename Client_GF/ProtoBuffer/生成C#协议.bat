@echo off
set des=./to

if exist "%des%" rd /s /q "%des%"
md "%des%"

for %%i in (./*.proto) do (
    protoc  --csharp_out="%des%" %%i
    rem 从这里往下都是注释，可忽略
    echo From %%i To %%~ni.cs Successfully!  
)
pause