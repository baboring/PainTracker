@ECHO OFF

SET "DIR_PROJECT=D:\Workspace\Project.Health\trunk"

SET "DIR_BIN_SRC=D:\Workspace\Project.Health\trunk\Export\Health\assets\bin"
SET "DIR_BIN_DEST=D:\Workspace\Project.Health\trunk\AndroidBuilder\app\src\main\assets\bin"

SET "DIR_JIN_SRC=D:\Workspace\Project.Health\trunk\Export\Health\libs\armeabi-v7a"
SET "DIR_JIN_DEST=D:\Workspace\Project.Health\trunk\AndroidBuilder\app\src\main\jniLibs\armeabi-v7a"

ECHO --------------------------------------------
ECHO Copy Android Native Library to assets
ECHO --------------------------------------------

ECHO Project Path=%DIR_PROJECT%
 
rd %DIR_BIN_DEST% /s /q

xcopy %DIR_BIN_SRC%\*.* %DIR_BIN_DEST%\*.* /s /e /y
xcopy %DIR_JIN_SRC%\*.* %DIR_JIN_DEST%\*.* /s /e /y

ECHO Copy completed - Android Native Library

pause