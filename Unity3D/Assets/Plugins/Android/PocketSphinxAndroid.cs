using UnityEngine;

namespace CMUPocketSphinx
{
    static public class CMUSphinxAndroid
    {
        public const string KEYPHRASE = "hello pretty aden";
        public const string KWS_SEARCH = "wakeup";
        public const string MENU_SEARCH = "menu";
        public const string FORECAST_SEARCH = "forecast";
        public const string DIGITS_SEARCH = "digits";
        public const string BODY_SEARCH = "body";
        public const string GREET_SEARCH = "greet";

		private static AndroidJavaClass sphinxHandle;

		private static AndroidJavaObject GetCurrentActivity()
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return jc.GetStatic<AndroidJavaObject>("currentActivity");
        }

        public static bool Init(string gameObjectName, string mode = "")
        {
            bool result = false;

            if (Application.platform == RuntimePlatform.Android)
            {
                sphinxHandle = new AndroidJavaClass("com.narith.pocketsphinx.RecognizerForUnity3d");
                DebugTools.Assert(null == sphinxHandle, "sphinxHandle is fail to initialize!!");
                result = sphinxHandle.CallStatic<bool>("_Init", gameObjectName);
				if(result) {
					DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
					result = sphinxHandle.CallStatic<bool>("_Create", GetCurrentActivity(), mode);
				}
            }
            return result;
        }
		
        public static void _DispatchMessage(string func, string msg)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            sphinxHandle.CallStatic("_DispatchMessage", func, msg);
        }

        // Show 
        public static void _ToastShow(string msg)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            GetCurrentActivity().Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                sphinxHandle.CallStatic("_ToastShow", msg);
            }));
        }

        public static void _Dispose()
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            sphinxHandle.CallStatic("_Dispose");
        }

        public static void _SwitchSearch(string searchName)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            sphinxHandle.CallStatic("_SwitchSearch", searchName);
        }

        public static string _CurrentSearchName()
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            return sphinxHandle.CallStatic<string>("_CurrentSearchName");
        }

        public static void _StopListening()
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            sphinxHandle.CallStatic("_StopListening");
        }

        public static bool _StartListening(string searchName)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            return sphinxHandle.CallStatic<bool>("_StartListening", searchName);
        }

        public static bool _StartListening(string searchName, int timeout)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            return sphinxHandle.CallStatic<bool>("_StartListening", searchName, timeout);
        }
    }

}