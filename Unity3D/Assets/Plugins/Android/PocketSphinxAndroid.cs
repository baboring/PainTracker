//#if (DEV_BUILD || DEBUG_ON)
#define CONDITIONAL_LOG     // 데브 빌드는 로그 보여줘
//# endif

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







        public static bool Initialize(string gameObjectName, string mode = "")
        {
            bool result = false;

            if (Application.platform == RuntimePlatform.Android)
            {
                sphinxHandle = new AndroidJavaClass("com.narith.pocketsphinx.RecognizerForUnity3d");
                DebugTools.Assert(null != sphinxHandle, "sphinxHandle is failed to initialize!!");
                result = sphinxHandle.CallStatic<bool>("_Init", gameObjectName);
				if(result) {
					DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
					result = sphinxHandle.CallStatic<bool>("_Create", JavaClassHelper.GetCurrentActivity(), mode);
				}
            }
            return result;
        }
		
        public static void DispatchMessage(string func, string msg)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            sphinxHandle.CallStatic("_DispatchMessage", func, msg);
        }

        // Show 
        public static void ToastShow(string msg)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
			JavaClassHelper.GetCurrentActivity().Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                sphinxHandle.CallStatic("_ToastShow", msg);
            }));
        }

        public static void Dispose()
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            sphinxHandle.CallStatic("_Dispose");
        }

        public static void SwitchSearch(string searchName)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            sphinxHandle.CallStatic("_SwitchSearch", searchName);
        }

        public static string CurrentSearchName()
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            return sphinxHandle.CallStatic<string>("_CurrentSearchName");
        }

        public static void StopListening()
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            sphinxHandle.CallStatic("_StopListening");
        }

        public static bool StartListening(string searchName)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            return sphinxHandle.CallStatic<bool>("_StartListening", searchName);
        }

        public static bool StartListening(string searchName, int timeout)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            return sphinxHandle.CallStatic<bool>("_StartListening", searchName, timeout);
        }
    }

	// Listener
	public interface OnVoiceRecognizeListener {
		void _OnLog(string msg);
		void _OnWakeup(string msg);
		void _OnHypothsisPartialResult(string text);
		void _OnHypothsisResult(string text);
		void _DoInBackground(string msg);
		void _OnPostExecute(string msg);
		void _OnBeginningOfSpeech(string msg);
		void _OnEndOfSpeech(string msg);
		void _OnStopLisnening(string msg);
		void _OnStartListening(string msg);
		void _OnTimeout(string msg);
	}


}