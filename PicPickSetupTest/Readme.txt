put this in the PostBuildEvent

cscript.exe "$(ProjectDir)ModifyMsiToEnableLaunchApplication.js" "$(BuiltOuputPath)"