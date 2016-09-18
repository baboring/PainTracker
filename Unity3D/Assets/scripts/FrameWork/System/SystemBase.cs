/********************************************************************
	created:	2014/12/07
	filename:	SystemBase.cs
	author:		Benjamin
	purpose:	[환경적인 정보 관리용 클래스]
*********************************************************************/
using UnityEngine;
using System.Collections.Generic;

//#if UNITY_STANDALONE_WIN

namespace HC
{
	public enum BuildPlatform
	{
		WebPlayer,
		Standalones,
		IOS,
		Android,
		WP8,
	}

	// 시스템 환경 설정
    public abstract class SystemBase
	{
		public const int DefScreenWidth = 720;
		public const int DefScreenHeight = 1280;

		static int _ScreenWidth = DefScreenWidth;
        static int _ScreenHeight = DefScreenHeight;
        static bool isIntiailization = false;

		public static int ScreenWidth { get { return _ScreenWidth; } private set { _ScreenWidth = value; } }
		public static int ScreenHeight { get { return _ScreenHeight; } private set { _ScreenHeight = value; } }

		static bool FullScreen {get; set;}

		public static int targetFrameRate = 60;

		// Depth order
		public const float DEPTH_BASE_MESSAGE_BOX = -10;
		public const float DEPTH_BASE_SYSTEM = -300;		// (Exit App , Reconnect , WaitResponse )
		public const float DEPTH_BASE_WEBVIEW = -990;		// Web View

		public static AppVersionInfo AppVersion = new AppVersionInfo();		// app version 이다.

        // streaming Asset path
        string GetStreamingAssetsPath() {
            string path;
#if UNITY_EDITOR
            path = "file:" + Application.dataPath + "/StreamingAssets";
#elif UNITY_ANDROID
     path = "jar:file://"+ Application.dataPath + "!/assets/";
#elif UNITY_IOS
     path = "file:" + Application.dataPath + "/Raw";
#else
     //Desktop (Mac OS or Windows)
     path = "file:"+ Application.dataPath + "/StreamingAssets";
#endif

            return path;
        }
		static BuildPlatform getRuntimePlatform()
		{
			if (Application.platform == RuntimePlatform.WindowsPlayer ||
				Application.platform == RuntimePlatform.OSXPlayer ||
				Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor) {
				return BuildPlatform.Standalones;
			}
			else if (Application.platform == RuntimePlatform.OSXWebPlayer ||
					Application.platform == RuntimePlatform.WindowsWebPlayer) {
				return BuildPlatform.WebPlayer;
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				return BuildPlatform.IOS;
			}
			else if (Application.platform == RuntimePlatform.Android) {
				return BuildPlatform.Android;
			}
			else {
				Logger.Error("Platform {0} is not supported by BundleManager.", Application.platform);
				return BuildPlatform.Standalones;
			}
		}

		// 현재 인터넷 환경이 WIFI 인지 체크
		public bool IsWIFI { get { return (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork); } }

		// 푸시 알림 시스템
		static public string PushSystem
		{
			get
			{
				switch (getRuntimePlatform()) {
				case BuildPlatform.Standalones:
					return "";
				case BuildPlatform.WebPlayer:
					return "";
				case BuildPlatform.Android:
					return "G";
				case BuildPlatform.IOS:
					return "A";
				}
				return "";
			}
		}

		// 현재 시스템의 OS 구분
		static public string Platform
		{
			// OS 구분
			get
			{
				switch (getRuntimePlatform()) {
				case BuildPlatform.Standalones:
					return "pc";
				case BuildPlatform.WebPlayer:
					return "web";
				case BuildPlatform.Android:
					return "android";
				case BuildPlatform.IOS:
					return "ios";
				}
				return "";
			}
		}

		// 시스템 uuid
		static public string DeviceUID 
		{
			get { return SystemInfo.deviceUniqueIdentifier; }
		}


		// 현재 시스템의 OS 구분
		static public string OS_Info
		{
			// OS 구분
			get
			{
				var os_Version = SystemInfo.operatingSystem;
				int sdkPos = -1;
				//int iVersionNumber = 0;
				float osVersion = 0;
				switch (getRuntimePlatform()) {
				case BuildPlatform.Standalones:
				case BuildPlatform.WebPlayer: {
						string[] os_info = os_Version.Split(' ');
						return os_info[0];
					}
				case BuildPlatform.Android: {
						float.TryParse(os_Version.Substring(sdkPos + 4, 2), out osVersion);
						string[] os_info = os_Version.Split('/');
						return os_info[0];
					}
				case BuildPlatform.IOS:
					os_Version = os_Version.Replace("iPhone OS ", "");
					float.TryParse(os_Version.Substring(0, 1), out osVersion);
					break;
				}

                Logger.DebugFormat("Version Number: {0}", osVersion);

				return SystemInfo.operatingSystem;
			}
		}
		////////////////////////////////////////
		// Dev 환경
		static readonly public bool IsDevBuild = 
#if DEV_BUILD
		true;
#else
		false;
#endif
		////////////////////////////////////////
		// Debug 환경
		static readonly public bool IsDebugOn =
#if DEBUG_ON
		true;
#else
		false;
#endif


		public enum RatioOption
		{
			None = 0,
			DependWidth,
			DependHeight,
		}

		// 자동 회전 On/Off
		static public bool IsAutoRotate
		{
			get { return Screen.orientation == ScreenOrientation.AutoRotation; }
			set
			{
				if (value) {
					Screen.orientation = ScreenOrientation.AutoRotation;
					Screen.autorotateToLandscapeLeft = false;
					Screen.autorotateToLandscapeRight = false;
					Screen.autorotateToPortrait = true;
					Screen.autorotateToPortraitUpsideDown = true;
				}
				else {
					Screen.orientation = ScreenOrientation.Landscape;
				}
			}
		}

		// 처름에 무조건 해야지..
		public static void Initial()
		{
			if (!isIntiailization) {
				isIntiailization = true;

				// 해상도 체크 해야지 ( Pc 빌드 이외에는 불필요 )
				//if (Screen.resolutions.Length > 0) {
				//	Logger.Log("--------------");
				//	var lstRes = Screen.resolutions;
				//	for (int i = 0; i < lstRes.Length; ++i)
				//		Logger.LogFormat("[{0}] Resolution ( {1} x {2} x {3} )", i, lstRes[i].width, lstRes[i].height, lstRes[i].refreshRate);
				//	Logger.Log("--------------");
				//}

				// Main 에서 하도록 두자..
				//SystemConfig.SetScreen(Screen.width, Screen.height, SystemConfig.RatioOption.DependWidth, true);

				//자동 슬립 모드 방지 ( 실시간 게임일때만 키자 )
				Screen.sleepTimeout = SleepTimeout.NeverSleep;

				SystemConfig.targetFrameRate = 30;
				SystemConfig.IsAutoRotate = true;   // 아이폰 4s LandScape 혹시 몰라 넣었다.

				Application.targetFrameRate = targetFrameRate;



			}

		}

		////////////////////////////////////////
        public enum ScreenMode {
            LandScape,
            Portrait,
        }

        protected static ScreenMode scrMode = ScreenMode.Portrait;

        static public void SetScreenMode(ScreenMode eMode, RatioOption dependRatio) {
            scrMode = eMode;
            SetScreen(Screen.width, Screen.height, dependRatio, true);
        }
		// 해상도 설정
		static public void SetScreen(int width, int height, RatioOption dependRatio, bool bFullScreen)
		{
			switch (dependRatio) {
			case RatioOption.None:
				ScreenWidth = width;
				ScreenHeight = height;
				break;
			case RatioOption.DependWidth:
				ScreenWidth = width;
                if (scrMode == ScreenMode.LandScape)
    				ScreenHeight = width * 9 / 16; // 16 : 9 비율로 설정 하기...( Nexus 테스트 필요 ) (LandScape)
                else
				    ScreenHeight = width * 16 / 9; // 9 : 16 비율로 설정 하기...( Nexus 테스트 필요 )
				break;
			case RatioOption.DependHeight:
                if (scrMode == ScreenMode.LandScape)
				    ScreenWidth = height * 16 / 9; // 16 : 9 비율로 설정 하기...( Nexus 테스트 필요 )
                else
                    ScreenWidth = height * 9 / 16; // 16 : 9 비율로 설정 하기...( Nexus 테스트 필요 )
				ScreenHeight = height;
				break;
			}
			FullScreen = bFullScreen;


			Logger.InfoFormat("Org Screen ( {0} x {1} )", Screen.width ,Screen.height);
			Logger.InfoFormat("Set Screen ( {0} x {1} )", ScreenWidth, ScreenHeight);

			Screen.SetResolution(ScreenWidth, ScreenHeight, FullScreen, targetFrameRate);

			// 변경된 해상도..
			Logger.InfoFormat("Cur Screen ( {0} x {1} ) --", Screen.currentResolution.width, Screen.currentResolution.height);
			
		}

		// 해상도 비율로 크기 조절 하기
		static public void SetScreenRatio(float ratio, RatioOption dependRatio)
		{
			SetScreen((int)(ScreenWidth * ratio), (int)(ScreenHeight * ratio), dependRatio, FullScreen);

		}

		// 기준 해상도별 실제 화면의 값을 알려줘..
		static public float GetToScreenX(float fx) { return fx * (1.0f / (float)DefScreenWidth) * (float)Screen.width; }
		static public float GetToScreenY(float fy) { return fy * (1.0f / (float)DefScreenHeight) * (float)Screen.height; }


		// 카메라 비율을 변경해 주자 ( 매 프레임별로 보면서 변경해 주자 )
		static public void UpdateResolution()
		{
			if (null != Camera.current && 0 != Camera.current.pixelRect.width) {
				// 변경되어야 할 해상도
				float WantedAspectCam = Camera.current.pixelRect.width / Camera.current.pixelRect.height;
				float CurrentAspectCam = Camera.current.aspect;
				if (Mathf.Abs(WantedAspectCam - CurrentAspectCam) > float.Epsilon) {
					Camera.current.aspect = WantedAspectCam;
					Logger.InfoFormat("change camera aspect {0} to {1}", CurrentAspectCam, WantedAspectCam);
				}

			}
		}

		//앱 버전이 서버로부터 받은 버전과 같은가(최신 인가?)
		static public bool IsNeedUpdate()
		{
			// 버전이 다르면 업데이트 하시오... 빌드번호는 Minor한것이다..
			return (!AppVersion.Version.Equals(Version.bundleVersion));
		}

		static public bool IsEqualBuildNo()
		{
			return (AppVersion.BuildNo.Equals(Version.bundleBuildNo));
		}


		// 입력값에서 해당 상태 조건 체크
		static public bool IsExistsArg(string arg)
		{
			// batch check
			arg = arg.ToUpper();
			foreach (var info in System.Environment.GetCommandLineArgs()) {
				var st = info.ToUpper();
				if (st.Equals(arg))
					return true;
			}
			return false;
		}
	}

}
