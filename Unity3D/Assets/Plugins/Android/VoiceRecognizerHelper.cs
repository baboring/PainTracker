//#if (DEV_BUILD || DEBUG_ON)
#define CONDITIONAL_LOG     // 데브 빌드는 로그 보여줘
//# endif

using UnityEngine;

namespace CMUPocketSphinx
{
    static public class VoiceRecognizerHelper
    {
        public const string KEYPHRASE = "hello benjamin";
        public const string SEARCH_KWS = "wakeup";
        public const string MENU_SEARCH = "menu";
        public const string FORECAST_SEARCH = "forecast";
        public const string SEARCH_DIGITS = "digits";
        public const string BODY_SEARCH = "body";
        public const string GREET_SEARCH = "greet";
		public const string SEARCH_PAIN = "pain";
		public const string SEARCH_ANSWER = "answer";
		public const string SEARCH_COMMAND = "command";
		public const string SEARCH_NUMBER_ZERO_TO_TEN = "number 0-10";
		public const string SEARCH_PLAYER = "player";


		private static string lastSearchName = "";
		private static AndroidJavaClass javaClass;
		static GameObject receiverObj;

        public static bool Initialize(GameObject obj, string mode = "")
        {
            bool result = false;

			receiverObj = obj;


			if (Application.platform == RuntimePlatform.Android)
            {
                javaClass = new AndroidJavaClass("com.narith.pocketsphinx.RecognizerForUnity3d");
                DebugTools.Assert(null != javaClass, "sphinxHandle is failed to initialize!!");
                result = javaClass.CallStatic<bool>("_Init", obj.name);
				if(result) {
					DebugTools.Assert(null != javaClass, "sphinxHandle is null");
					result = javaClass.CallStatic<bool>("_Create", JavaClassHelper.GetCurrentActivity(), mode);
				}
            }
			else if (Application.platform == RuntimePlatform.WindowsEditor) {
				receiverObj.SendMessage("_OnPostExecute", "success");
			}
			return result;
        }

		public static void DispatchMessage(string func, string msg) {
			if (Application.platform == RuntimePlatform.Android) {
				DebugTools.Assert(null != javaClass, "sphinxHandle is null");
				javaClass.CallStatic("_DispatchMessage", func, msg);
			}
		}

        // Show 
        public static void ToastShow(string msg) {
			if (Application.platform == RuntimePlatform.Android) {
				DebugTools.Assert(null != javaClass, "sphinxHandle is null");
				JavaClassHelper.GetCurrentActivity().Call("runOnUiThread", new AndroidJavaRunnable(() => {
					javaClass.CallStatic("_ToastShow", msg);
				}));
			}
        }

        public static void Dispose() {
			if (Application.platform == RuntimePlatform.Android) {
				DebugTools.Assert(null != javaClass, "sphinxHandle is null");
				javaClass.CallStatic("_Dispose");
			}
        }

		public static void SwitchSearch(string searchName) {
			if (Application.platform == RuntimePlatform.Android) {
				DebugTools.Assert(null != javaClass, "sphinxHandle is null");
				javaClass.CallStatic("_SwitchSearch", searchName);
			}
			else if (Application.platform == RuntimePlatform.WindowsEditor) {
				lastSearchName = searchName;
				receiverObj.SendMessage("_OnStopListening", searchName);
				receiverObj.SendMessage("_OnSwitchSearch", searchName);
				receiverObj.SendMessage("_OnStartListening", searchName);
			}
		}


		public static string CurrentSearchName() {
			if (Application.platform == RuntimePlatform.Android) {
				DebugTools.Assert(null != javaClass, "sphinxHandle is null");
				return javaClass.CallStatic<string>("_CurrentSearchName");
			}
			return lastSearchName;
        }

        public static void StopListening() {
			if (Application.platform == RuntimePlatform.Android) {
				DebugTools.Assert(null != javaClass, "sphinxHandle is null");
				javaClass.CallStatic("_StopListening");
			}
			else if (Application.platform == RuntimePlatform.WindowsEditor) {
				receiverObj.SendMessage("_OnStopListening", "");
			}

		}

		public static bool StartListening(string searchName, int timeout = 10000) {
			if (Application.platform == RuntimePlatform.Android) {
				DebugTools.Assert(null != javaClass, "sphinxHandle is null");
				if(timeout< 1)
					return javaClass.CallStatic<bool>("_StartListening", searchName);
				return javaClass.CallStatic<bool>("_StartListening", searchName, timeout);
			}
			else if (Application.platform == RuntimePlatform.WindowsEditor) {
				lastSearchName = searchName;
				receiverObj.SendMessage("_OnStartListening", searchName);
			}
			return false;
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
		void _OnBeginningOfSpeech(string search);
		void _OnEndOfSpeech(string hypo);
		void _OnStopListening(string search);
		void _OnStartListening(string search);
		void _OnSwitchSearch(string search);
		void _OnCancelListening(string search);
		void _OnTimeout(string msg);
	}


}