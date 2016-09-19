#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Xml;

using JsonFx.Json;

public class BuildWizard : ScriptableWizard
{
	#region VARIABLES
	public enum ESSENTIAL_DEFINE
	{
		DEV_BUILD, SHIPPING_BUILD, BARUNSON_QA_BUILD,
	};
	
	public enum PROVISIONING
	{
		ADHOC, DISTRIBUTION,
	};

	public ESSENTIAL_DEFINE essentialBuildDefine;

	public string bundleVersion = "1.0.0";

#if UNITY_IPHONE//ios 선 구글빌드번호 표시 안되게, pc에선 안드로이드 빌드번호 따라간다.
	[HideInInspector]
#endif
	public int buildNumberAndroid = 5;

#if UNITY_ANDROID || UNITY_STANDALONE //안드로이드, pc 환경에선 애플빌드번호 표시 안되게
	[HideInInspector]
#endif
	public int buildNumberIOS = 8;

	public int revNumber = 11200; //12.16

	public BuildOptions options = BuildOptions.None;

	public bool definePlatformKakao = false;
	public bool defineUseLocalPparam = false;

#if UNITY_IPHONE || UNITY_STANDALONE //ios, pc 환경에선 구글결제여부 표시 안되게
	[HideInInspector]
#endif
	public bool defineIapGoogle = false;

#if UNITY_ANDROID || UNITY_STANDALONE //안드로이드, pc 환경에선 애플결제여부 표시 안되게
	[HideInInspector]
#endif
	public bool defineIapApple = false;

	public bool defineAdbrix = true;
	public bool defineTnkad = true;

	public bool defineDebugOn = false;
	public bool defineShowFPS = false;
	public bool definePushOff = false;

#if UNITY_IPHONE || UNITY_STANDALONE //ios,pc 환경에선 블루스택여부 표시 안되게
	[HideInInspector]
#endif
	public bool defineBlueStackTest = false;
	public bool adaptAppSealing = false;

	string buildDefines = ""; //빌드에 적용할 최종 디파인 조합

	BuildTargetGroup buildTarget;

#if UNITY_ANDROID || UNITY_STANDALONE //안드로이드,pc 환경에선 프로비저닝 표시 안되게
	[HideInInspector]
#endif
	public PROVISIONING provisioning;
	#endregion

	#region CONST STRING
	const string PROJECTS = "Health";
#if UNITY_IPHONE
	const string SETTING_FILE = "./SettingWizard/BuildSettings.txt";
#else
	const string SETTING_FILE = "SettingWizard\\BuildSettings.txt";
#endif

	const string JSON_ESSENTIAL_DEF = "essential_define";
	const string JSON_BUNDLE_VER = "bundle_version";
	const string JSON_BUILD_NUM_ANDROID = "build_number_android";
	const string JSON_BUILD_NUM_IOS = "build_number_ios";
	const string JSON_REV_NUM = "rev_number";
	const string JSON_KAKAO = "define_platformkakao";
	const string JSON_LOCAL_PPARAM = "define_localpparam";
	const string JSON_IAP_G = "define_iap_google";
	const string JSON_IAP_A = "define_iap_apple";
	const string JSON_ADBRIX = "define_adbrix";
	const string JSON_TNKAD = "define_tnkad";
	const string JSON_DEBUGON = "define_debugon";
	const string JSON_SHOWFPS = "define_showfps";
	const string JSON_PUSHOFF = "define_pushoff";
	const string JSON_BLUESTACK = "define_bluestack";
#endregion

#region CONST STRING FOR XCODE
#if INCLUDE_IL2CPP
	const string XCODE_PRJ_EXPORT_PATH = "/Users/utplus/SVN/ProjectS/Build64";
#else
	const string XCODE_PRJ_EXPORT_PATH = "/Users/utplus/SVN/ProjectS/Build";
#endif
#endregion

	[MenuItem("Build Wizard/Open Wizard")]
	static void MenuEntryCall()
	{
		BuildWizard buildWiz = BuildWizard.DisplayWizard("Build Settings", typeof(BuildWizard), "Start Build", "Apply And Save") as BuildWizard;

		buildWiz.ReadJsonSettings();
		buildWiz.OnWizardUpdate();
		buildWiz.ApplyDefine();
	}

	[MenuItem("Build Wizard/Open Export")]
	static void MenuExportCall() {
		BuildWizard buildWiz = BuildWizard.DisplayWizard("Build Settings", typeof(BuildWizard), "Start Export", "Apply And Save") as BuildWizard;
		buildWiz.options = BuildOptions.InstallInBuildFolder | BuildOptions.AcceptExternalModificationsToPlayer;
		buildWiz.ReadJsonSettings();
		buildWiz.OnWizardUpdate();
		buildWiz.ApplyDefine();
	}

	/// 뭔가를 갱신하면 호출
	void OnWizardUpdate()
	{
		UpdateDefineRelation();
		UpdateDefine();
	}

	void UpdateDefineRelation()
	{
		//카카오가 들어가면 결제는 항상 붙어야 한다.
		if (definePlatformKakao)
		{
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
			{
				defineIapGoogle = true;
			}
			else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
			{
				defineIapApple = true;
			}
		}
	}

	/// Start Build 버튼 누르면 호출
	void OnWizardCreate()
	{
		//if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
		//    return;

		if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
		{
			const string APPSEALING_ASSETPATH = "Assets/SecureScript";
			AssetDatabase.DeleteAsset(APPSEALING_ASSETPATH);
#if SHIPPING_BUILD || BARUNSON_QA_BUILD
			AssetDatabase.DeleteAsset("Assets/Resources/PParam/ProjectS.bytes");
#endif
		}

		ApplyDefine();			//디파인 적용
		APPVersionModify();		//버전정보, 빌드번호 적용
		SaveJsonSettings();		//셋팅 저장

		bool result = RunUnityBuild("Build");		//유니티로 빌드

		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
		{
			if (result)		//ios는 xcode로 빌드를 한번버 거쳐야 한다.
			{
				CopyXCodeSettings();		//xcode 상에서 설정된 플러그인들 , 몇몇 수정된 소스파일들을 유니티가 빌드해 낸 곳으로 복사
				UpdatePList();				//xcode 가 읽어들이는 메타파일 내에 버전정보, 빌드번호 수정
				RunXCodeBuild();			//mac console command 를 이용하여 xcode 빌드
			}
		}
		else
		{

		}
	}

	void OnWizardOtherButton()
	{
		ApplyDefine();			//디파인 적용
		APPVersionModify();		//버전정보, 빌드번호 적용
		SaveJsonSettings();		//셋팅 저장
	}

	public void ReadJsonSettings()
	{
		Dictionary<string, System.Object> json_decode = JsonReader.Deserialize(File.ReadAllText(SETTING_FILE))
			as Dictionary<string, System.Object>;
		if (json_decode != null)
		{
			essentialBuildDefine = (ESSENTIAL_DEFINE)json_decode[JSON_ESSENTIAL_DEF];
			bundleVersion = (string)json_decode[JSON_BUNDLE_VER];
			buildNumberAndroid = (int)json_decode[JSON_BUILD_NUM_ANDROID];
			buildNumberIOS = (int)json_decode[JSON_BUILD_NUM_IOS];
			revNumber = (int)json_decode[JSON_REV_NUM];

			definePlatformKakao = Convert.ToBoolean((int)json_decode[JSON_KAKAO]);
			defineUseLocalPparam = Convert.ToBoolean((int)json_decode[JSON_LOCAL_PPARAM]);

			defineIapGoogle = Convert.ToBoolean((int)json_decode[JSON_IAP_G]);
			defineIapApple = Convert.ToBoolean((int)json_decode[JSON_IAP_A]);

			defineAdbrix = Convert.ToBoolean((int)json_decode[JSON_ADBRIX]);
			defineTnkad = Convert.ToBoolean((int)json_decode[JSON_TNKAD]);

			defineDebugOn = Convert.ToBoolean((int)json_decode[JSON_DEBUGON]);
			defineShowFPS = Convert.ToBoolean((int)json_decode[JSON_SHOWFPS]);
			definePushOff = Convert.ToBoolean((int)json_decode[JSON_PUSHOFF]);

			defineBlueStackTest = Convert.ToBoolean((int)json_decode[JSON_BLUESTACK]);

			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
				defineTnkad = false;
		}
	}

	void SaveJsonSettings()
	{
		Dictionary<string, System.Object> json_encode = new Dictionary<string, System.Object>();
		json_encode.Add(JSON_ESSENTIAL_DEF, (int)essentialBuildDefine);
		json_encode.Add(JSON_BUNDLE_VER, bundleVersion);
		json_encode.Add(JSON_BUILD_NUM_ANDROID, buildNumberAndroid);
		json_encode.Add(JSON_BUILD_NUM_IOS, buildNumberIOS);
		json_encode.Add(JSON_REV_NUM, revNumber);
		json_encode.Add(JSON_KAKAO, Convert.ToInt32(definePlatformKakao));
		json_encode.Add(JSON_LOCAL_PPARAM, Convert.ToInt32(defineUseLocalPparam));
		json_encode.Add(JSON_IAP_G, Convert.ToInt32(defineIapGoogle));
		json_encode.Add(JSON_IAP_A, Convert.ToInt32(defineIapApple));
		json_encode.Add(JSON_ADBRIX, Convert.ToInt32(defineAdbrix));
		json_encode.Add(JSON_TNKAD, Convert.ToInt32(defineTnkad));
		json_encode.Add(JSON_DEBUGON, Convert.ToInt32(defineDebugOn));
		json_encode.Add(JSON_SHOWFPS, Convert.ToInt32(defineShowFPS));
		json_encode.Add(JSON_PUSHOFF, Convert.ToInt32(definePushOff));
		json_encode.Add(JSON_BLUESTACK, Convert.ToInt32(defineBlueStackTest));

		JsonWriter wr = new JsonWriter(SETTING_FILE);
		wr.PrettyPrint = true;

		wr.Write(json_encode);
		wr.TextWriter.Flush();	
		wr.TextWriter.Close();

		json_encode.Clear();
		json_encode = null;
	}

	public void UpdateDefine()
	{
		if (essentialBuildDefine == ESSENTIAL_DEFINE.DEV_BUILD)
			buildDefines = "DEV_BUILD";
		else if (essentialBuildDefine == ESSENTIAL_DEFINE.SHIPPING_BUILD)
			buildDefines = "SHIPPING_BUILD";
		else if (essentialBuildDefine == ESSENTIAL_DEFINE.BARUNSON_QA_BUILD)
			buildDefines = "BARUNSON_QA_BUILD";
		
		DefineConcat(definePlatformKakao, "PLATFORM_KAKAO");
		DefineConcat(defineUseLocalPparam, "USE_LOCAL_PPARAM");
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
			DefineConcat(defineIapGoogle, "IAP_GOOGLE");
		else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
			DefineConcat(defineIapApple, "IAP_APPLE");
		DefineConcat(defineAdbrix, "ADBRIX");
		DefineConcat(defineTnkad, "TNKAD");
		DefineConcat(defineDebugOn, "DEBUG_ON");
		DefineConcat(defineShowFPS, "SHOW_FPS");
		DefineConcat(definePushOff, "PUSH_OFF");
		DefineConcat(defineBlueStackTest, "BLUESTACK_TEST");
		DefineConcat(adaptAppSealing, "APP_SEALING");
	}

	public void ApplyDefine()
	{
		BuildTargetGroup btg;
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
			btg = BuildTargetGroup.Android;
		else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
			btg = BuildTargetGroup.iPhone;
		else
			btg = BuildTargetGroup.Standalone;
		PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, buildDefines);

		Debug.Log("### Apply Defines : " + PlayerSettings.GetScriptingDefineSymbolsForGroup(btg));
	}

	//checkValue 가 true면 주어진 concatDefine을 concatenation 한다
	void DefineConcat(bool checkValue, string concatDefine)
	{
		if (checkValue)
			buildDefines = buildDefines + ";" + concatDefine;
	}

	public void APPVersionModify()
	{
		PlayerSettings.bundleVersion = bundleVersion;
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
			PlayerSettings.Android.bundleVersionCode = buildNumberAndroid;

		const string codeFilePath = "Assets/Scripts/";
		const string codeFileName = "Version.cs";

		string newCodeFormat =
			"public class Version\n" +
			"{{\n" +
			"\tpublic const int revision = {0};\n" +
			"\tpublic const string bundleVersion = \"{1}\";\n" +
			"\tpublic const int bundleBuildNo = {2};\n" +
			"}}";

		//안드로이드, pc에는 buildNumberAndroid 를 적용
		int buildNum = (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone) ? buildNumberIOS : buildNumberAndroid;
		string newCode = string.Format(newCodeFormat, revNumber, bundleVersion, buildNum);

		File.WriteAllText(codeFilePath + codeFileName, newCode);

		UnityEditor.AssetDatabase.Refresh();
	}

	public bool RunUnityBuild(string destPath = "")
	{
		if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android &&
			EditorUserBuildSettings.activeBuildTarget != BuildTarget.iPhone &&
			EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows)
			return false; //현재 빌드 설정이 안드로이드, 아이폰, pc가 아니면 그냥 나감

		string target = "";
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
		{
			//앱 실링이 MonoDevelop Project 를 사용하니까 한번은 Sync 해 줘야 한다.
			if(adaptAppSealing)
				EditorApplication.ExecuteMenuItem("Assets/Sync MonoDevelop Project");

			//android apk 파일명 만들기
			if (string.IsNullOrEmpty(destPath)) {
				string outFileName = GetPackageFileName();
				var current_d = System.IO.Directory.GetCurrentDirectory();
				target = System.IO.Path.Combine(current_d, outFileName);
			}
			else
				target = string.Format("{0}\\{1}", destPath, GetPackageFileName());
		}
		else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone) {
			//ios 는 xcode 용 프로젝트 파일 export 폴더
			target = XCODE_PRJ_EXPORT_PATH;
		}
		else {
			if (string.IsNullOrEmpty(destPath))
				target = "../../../Build/PC Standalone/Project_S_Dev.exe";
			else
				target = destPath; //pc는 destPath 에 파일명까지 포함되어 있음
		}

		if (!string.IsNullOrEmpty(target)) {
			string[] sceneLevels = EditorBuildSettings.scenes.Where(ss => ss.enabled).Select(s => s.path).ToArray();
			string error = BuildPipeline.BuildPlayer(sceneLevels, target, EditorUserBuildSettings.activeBuildTarget, this.options);
			return (string.IsNullOrEmpty(error) ? true : false);
		}

		Debug.LogError("target Empty!!");
		return false;
	}

#region IOS REALATION
	void CopyXCodeSettings()
	{
		const string COPYFILENAME = "tempShell";

		//xcode setting 을 복사해 주는 shell 스크립트를 만들어서 실행한다.
		string SETTING_PATH = "/Users/utplus/Desktop/settings_projects";
#if INCLUDE_IL2CPP
		SETTING_PATH += "/utplus_kakao_64bit/*";
#else
		SETTING_PATH += "/utplus_kakao/*";
#endif
		//SETTING_PATH += "/jh_dev_kakao_adbrix/*";

		string newCodeFormat =
			"#!/bin/bash\n" +
			"cp -a {0} {1}\n" +
			"exit $?";
		string newCode = string.Format(newCodeFormat, SETTING_PATH, XCODE_PRJ_EXPORT_PATH);
		File.WriteAllText(COPYFILENAME, newCode);

		//위에서 만든 파일은 실행 권한이 없다, 그래서 실행권한을 주는 명령을 별도로 먼저 실행
		System.Diagnostics.Process proc = System.Diagnostics.Process.Start("chmod", "777 " + COPYFILENAME);
		proc.WaitForExit();
		proc.Close();
		Debug.Log("Permission Changed !!!!!!");

		//실행권한을 주었으니 이제 실행
		proc = System.Diagnostics.Process.Start(COPYFILENAME);
		proc.WaitForExit();
		proc.Close();
		Debug.Log("XCode Settings Copied !!!!!!");

		proc.Close();
	}

	//ipa 파일을 얻기 위해 xcode로 빌드
	//command script로 빌드 위해 문자열작업을 하자.....
	void RunXCodeBuild()
	{
		const string BUILDFILENAME = "tempxcodebuild";
		WriteXCodeBuildScript(BUILDFILENAME);

		//위에서 만든 파일은 실행 권한이 없다, 그래서 실행권한을 주는 명령을 별도로 먼저 실행
		System.Diagnostics.Process proc = System.Diagnostics.Process.Start("chmod", "777 " + BUILDFILENAME);
		proc.WaitForExit();
		proc.Close();
		Debug.Log("Permission Changed !!!!!!");

		Debug.Log("XCode Build Start !!!!!!");
		proc = System.Diagnostics.Process.Start(BUILDFILENAME);
		proc.WaitForExit();
		int exitcode = proc.ExitCode;
		proc.Close();

		if(exitcode != 0)
			Debug.LogError("XCode Build Error !!!!!! , ErrorCode: " + exitcode);
		else
			Debug.Log("XCode Build End !!!!!!");
	}

	//ios xcode 메타파일인 plist 파일을 수정한다. 여기에 버전정보, 빌드넘버가 있다.
	void UpdatePList()
	{
		const string PLIST_PATH = XCODE_PRJ_EXPORT_PATH + "/Info.plist";
		//const string PLIST_PATH = "iOSBuild\\Info.plist"; //window 용 test 폴더
		XmlReaderSettings settings = new XmlReaderSettings();
		settings.ProhibitDtd = false;

		XmlReader xmlReader = XmlReader.Create(PLIST_PATH, settings);
		XmlDocument xmlDocu = new XmlDocument();
		xmlDocu.Load(xmlReader);
		xmlReader.Close();

		XmlNode nextNode = xmlDocu.DocumentElement.SelectSingleNode("dict"); //<dict>
		nextNode = nextNode.FirstChild; //<key>~~~</key>

		while(true)
		{
			if (nextNode == null) //없다
				break;
			if (nextNode.InnerText == "CFBundleShortVersionString")
			{
				XmlNode versionNode = nextNode.NextSibling;
				Debug.Log(versionNode.InnerText + " ---> " + bundleVersion);
				versionNode.InnerText = bundleVersion; //버전 변경
			}
			else if (nextNode.InnerText == "CFBundleVersion")
			{
				XmlNode buildNode = nextNode.NextSibling;
				Debug.Log(buildNode.InnerText + " ---> " + buildNumberIOS);
				buildNode.InnerText = buildNumberIOS.ToString(); //빌드 번호 변경
			}
			nextNode = nextNode.NextSibling;
		}
		xmlDocu.Save(PLIST_PATH);

		//xmlDocu.Save("iOSBuild\\InfoOut.plist"); //window test 용 폴더

		//xml 읽을 때 이상하게 <!doctype ~~ 마지막 부분에 '[]' 가 추가된다.
		//'[]' 가 있으면 xcode 가 못 읽으므로 없애는 작업 필요
		//XmlDocument 에서 수정이 안되서 이렇게 함....
		string plist = File.ReadAllText(PLIST_PATH);
		plist = plist.Replace("[]>", ">");
		File.WriteAllText(PLIST_PATH, plist);
	
		Debug.Log("plist modified!!!");
	}

	void WriteXCodeBuildScript(string fileName)
	{
		const string IPA_PATH = "/Users/utplus/SVN/ProjectS/IPA";
		const string ARCHIVE_PATH = "/Users/utplus/Library/Developer/Xcode/Archives"; //xcode로 빌드하면 이 폴더에 아카이브 생성된다. 똑같이 맞추기 위해 이 폴더로 지정

		//지금시간을 월일시분 으로 표시
		string nowTime = DateTime.Now.ToString("MMddHHmm");

		//프로비저닝 적용 전인 아카이브 파일을 export할 경로와 파일 이름
		string archiveFullPath = string.Format("{0}/{1}_{2}.xcarchive", ARCHIVE_PATH, PROJECTS, nowTime);

		//아카이브에 적용할 프로비저닝 이름. !!!xcode에서 확인 할 수 있다.
		string selectProvisioningProfile =
			//(provisioning.Equals(PROVISIONING.ADHOC)) ? "XC Ad Hoc: com.barunsonena.projects" : "Projects_dist_141104"; //옛날꺼
			//(provisioning.Equals(PROVISIONING.ADHOC)) ? "magic kingdom adhoc" : "Projects_dist_150226";  //사내프로비저닝용
			(provisioning.Equals(PROVISIONING.ADHOC)) ? "Projects_adhoc_150226" : "Projects_dist_150226";

		//ipa 파일 이름에 적용할 문자열 결정
		string selectBuildType = (essentialBuildDefine.Equals(ESSENTIAL_DEFINE.DEV_BUILD)) ? "D" : "R";
		string selectProv = (provisioning.Equals(PROVISIONING.ADHOC)) ? "Adhoc" : "Dist";

		//프로비저닝 까지 적용된 최종 패키지 파일인 ipa를 저장할 경로와 파일이름
		string ipaFullPath = string.Format("{0}/{1}", IPA_PATH, GetPackageFileName());

		//bash script 만들어서 파일로 저장
		string newCodeFormat =
			"#!/bin/bash\n" +
			"cd {0}\n" +
			"xcodebuild -scheme Unity-iPhone -archivePath {1} archive\n" +
			"xcodebuild -exportArchive -exportFormat ipa -archivePath {2} -exportPath {3} -exportProvisioningProfile \"{4}\"\n" +
			"exit $?";
		string newCode = string.Format(newCodeFormat, XCODE_PRJ_EXPORT_PATH, archiveFullPath, archiveFullPath, ipaFullPath, selectProvisioningProfile);
		File.WriteAllText(fileName, newCode);
	}
#endregion

	string GetPackageFileName()
	{
		string result = "";
		
		//지금시간을 월일시분 으로 표시
		string nowTime = DateTime.Now.ToString("MMddHHmm");
		string selectBuildType = (essentialBuildDefine.Equals(ESSENTIAL_DEFINE.DEV_BUILD)) ? "D" : "R";

		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
		{
			string buildTypeByPlugin = "";
			if(definePlatformKakao && defineIapGoogle)
				buildTypeByPlugin = ".Kakao";
			else if (defineIapGoogle)
				buildTypeByPlugin = ".QA";

			string blueStackPlugin = "";
			if (defineBlueStackTest)
				blueStackPlugin = "_BS";

			return string.Format("{0}_{1}.{2}{3}.{4}{5}.apk", PROJECTS, selectBuildType, revNumber, buildTypeByPlugin, nowTime, blueStackPlugin);
		}
		else //ios
		{
			//ipa 파일 이름에 적용할 문자열 결정
			string selectProv = (provisioning.Equals(PROVISIONING.ADHOC)) ? "Adhoc" : "Dist";
			return string.Format("{0}_{1}_{2}_{3}.ipa", PROJECTS, selectBuildType, selectProv, nowTime);
		}

		return result;
	}
}
#endif