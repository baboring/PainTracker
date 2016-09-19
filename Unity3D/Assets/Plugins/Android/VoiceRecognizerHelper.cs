//#if (DEV_BUILD || DEBUG_ON)
#define CONDITIONAL_LOG     // 데브 빌드는 로그 보여줘
//# endif

using UnityEngine;

namespace CMUPocketSphinx
{
    static public class VoiceRecognizerHelper
    {
        public const string KEYPHRASE = "hello pretty aden";
        public const string KWS_SEARCH = "wakeup";
        public const string MENU_SEARCH = "menu";
        public const string FORECAST_SEARCH = "forecast";
        public const string DIGITS_SEARCH = "digits";
        public const string BODY_SEARCH = "body";
        public const string GREET_SEARCH = "greet";

		private static AndroidJavaClass javaClass;

        public static bool Initialize(string gameObjectName, string mode = "")
        {
            bool result = false;

            if (Application.platform == RuntimePlatform.Android)
            {
                javaClass = new AndroidJavaClass("com.narith.pocketsphinx.RecognizerForUnity3d");
                DebugTools.Assert(null != javaClass, "sphinxHandle is failed to initialize!!");
                result = javaClass.CallStatic<bool>("_Init", gameObjectName);
				if(result) {
					DebugTools.Assert(null != javaClass, "sphinxHandle is null");
					result = javaClass.CallStatic<bool>("_Create", JavaClassHelper.GetCurrentActivity(), mode);
				}
            }
            return result;
        }
		
        public static void DispatchMessage(string func, string msg)
        {
            DebugTools.Assert(null != javaClass, "sphinxHandle is null");
            javaClass.CallStatic("_DispatchMessage", func, msg);
        }

        // Show 
        public static void ToastShow(string msg)
        {
            DebugTools.Assert(null != javaClass, "sphinxHandle is null");
			JavaClassHelper.GetCurrentActivity().Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                javaClass.CallStatic("_ToastShow", msg);
            }));
        }

        public static void Dispose()
        {
            DebugTools.Assert(null != javaClass, "sphinxHandle is null");
            javaClass.CallStatic("_Dispose");
        }

        public static void SwitchSearch(string searchName)
        {
            DebugTools.Assert(null != javaClass, "sphinxHandle is null");
            javaClass.CallStatic("_SwitchSearch", searchName);
        }

        public static string CurrentSearchName()
        {
            DebugTools.Assert(null != javaClass, "sphinxHandle is null");
            return javaClass.CallStatic<string>("_CurrentSearchName");
        }

        public static void StopListening()
        {
            DebugTools.Assert(null != javaClass, "sphinxHandle is null");
            javaClass.CallStatic("_StopListening");
        }

        public static bool StartListening(string searchName)
        {
            DebugTools.Assert(null != javaClass, "sphinxHandle is null");
            return javaClass.CallStatic<bool>("_StartListening", searchName);
        }

        public static bool StartListening(string searchName, int timeout)
        {
            DebugTools.Assert(null != javaClass, "sphinxHandle is null");
            return javaClass.CallStatic<bool>("_StartListening", searchName, timeout);
        }
    }

	// Listener
	public interface OnVoiceRecognizeListener {
		void _OnLog(string msg);
		void _OnWakeup(string msg);
		void _OnHypothsisPartial(string text);
		void _OnHypothsisFinal(string text);
		void _DoInBackground(string msg);
		void _OnPostExecute(string msg);
		void _OnBeginningOfSpeech(string msg);
		void _OnEndOfSpeech(string msg);
		void _OnStopLisnening(string msg);
		void _OnStartListening(string msg);
		void _OnTimeout(string msg);
	}


}