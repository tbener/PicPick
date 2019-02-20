call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsDevCmd.bat"
cd %~dp0
xsd Project.xsd /c /enableDataBinding /n:PicPick.Project /out:..\
pause