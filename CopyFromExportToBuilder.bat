@ECHO OFF

SET "DIR_PROJECT=D:\Workspace\Project.Health\trunk"
SET "DIR_SRC=Export\Health"
SET "DIR_BUILDER=AndroidBuilder"


::dir binary folder
SET DIR_ROOT_SRC=%DIR_PROJECT%\%DIR_SRC%
SET DIR_ROOT_DEST=%DIR_PROJECT%\%DIR_BUILDER%

::binary source
SET "DIR_BIN_SRC=%DIR_ROOT_SRC%\assets\bin"
SET "DIR_BIN_DEST=%DIR_ROOT_DEST%\app\src\main\assets\bin"

:: jin libs
SET "DIR_JIN_SRC=%DIR_ROOT_SRC%\libs\armeabi-v7a"
SET "DIR_JIN_DEST=%DIR_ROOT_DEST%\app\src\main\jniLibs\armeabi-v7a"

::mainActivity java source
SET "DIR_MAIN_SRC=%DIR_ROOT_SRC%\src"
SET "DIR_MAIN_DEST=%DIR_ROOT_DEST%\app\src\main\java"

::Jar files
SET "DIR_LIB_SRC=%DIR_ROOT_SRC%\libs"
SET "DIR_LIB_DEST=%DIR_ROOT_DEST%\app\libs"

:: res (stringtable, )
SET "DIR_RES_SRC=%DIR_ROOT_SRC%\res"
SET "DIR_RES_DEST=%DIR_ROOT_DEST%\app\src\main\res"

ECHO --------------------------------------------
ECHO Copy Android Native Library to assets
ECHO --------------------------------------------

ECHO Project Path=%DIR_PROJECT%
ECHO Project Path=%DIR_BIN_SRC%

::Clean and copy Binary 
echo ### Clean and copy Binary ###
rd %DIR_BIN_DEST% /s /q
xcopy %DIR_BIN_SRC%\*.* %DIR_BIN_DEST%\*.* /s /e /y

::Copy Jin
::echo ### Copy Jin ###
::xcopy %DIR_JIN_SRC%\*.* %DIR_JIN_DEST%\*.* /s /e /y

::Copy Activity Source
::echo ### Copy Activity Source ###
::xcopy %DIR_MAIN_SRC%\*.* %DIR_MAIN_DEST%\*.* /s /e /y

::Copy Libraries
::echo ### Copy Libraries ###
::xcopy %DIR_LIB_SRC%\*.jar %DIR_LIB_DEST%\*.jar /y

::Copy AndroidManifest.xml
::echo ### Copy AndroidManifest.xml ###
::copy %DIR_ROOT_SRC%\AndroidManifest.xml %DIR_ROOT_DEST%\app\src\main\*.* /y

::Clean and copy res 
::echo ### Clean and copy res ###
::rd %DIR_RES_DEST% /s /q
::xcopy %DIR_RES_SRC%\*.* %DIR_RES_DEST%\*.* /s /e /y

ECHO ================================================
ECHO Copy completed - Android Native Library
ECHO ================================================

pause