call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\vcvars64.bat"
cd %~dp0
xsd Project.xsd /c /enableDataBinding /n:PicPick.Models /out:..\
pause