using UnityEngine;
using System;

namespace HC {

	public enum SpeechState {
		None = 0,
		TrySpeech,
		Start,
		Done,
	}

	public class TextToSpeechMain : MonoBehaviour, OnTTSListener {

		public static Action<string> callbackSpeakStart;

		public static SpeechState speechSt { get; set; }
		public static bool IsInstalled { get; private set; }


		public static TextToSpeechMain instance;
		public static void CreateInstance() {
			if (null == instance) {
				string _currentGUID = System.Guid.NewGuid().ToString();
				GameObject obj = new GameObject("TextToSpeechMain(" + _currentGUID + ")");
				instance = obj.AddComponent<TextToSpeechMain>();
				DontDestroyOnLoad(obj);

			}
		}

		// initialization
		void Awake() {
			TTSHelper.Initialize(gameObject.name, TTSHelper.UnitedKingdom);
		}

		void Start() {
			// 임의로 설정 된것으로 처리 하자
			if (Application.platform != RuntimePlatform.Android)
				IsInstalled = true;
		}

		// Utter Speak
		public static void Speak(string text) {
			speechSt = SpeechState.TrySpeech;
			Logger.Assert(IsInstalled, "Not Initialized TTS Engine");
			TTSHelper.SpeechFlush(text);
			if (Application.platform == RuntimePlatform.WindowsEditor) {
				Timer.SetTimer(500, (id) => instance._OnStart(text));
				Timer.SetTimer(1000, (id) => instance._OnDone(text));
			}
		}
		// Utter Speak
		public static void SpeakFormat(string szFormat, params object[] p) {
			Speak(string.Format(szFormat, p));
		}
		// Utter Speak
		public static void Speak(R.Speeches.eKey key, params object[] p) {
			Speak(string.Format(R.GetSpeeches(key).StrValue, p));
		}


		public void _OnLog(string msg) {
            Logger.Debug(msg);
        }

		public void _OnInitialized(string msg) {
			IsInstalled = true;
			Logger.Debug(msg);
		}

		public void _OnStart(string msg) {
			Logger.Debug(msg);
			speechSt = SpeechState.Start;
			if (null != callbackSpeakStart)
				callbackSpeakStart(msg);
		}

		public void _OnDone(string msg) {
			speechSt = SpeechState.Done;
		}

		public void _OnError(string msg) {
			Logger.Debug(msg);
		}

		public void Destroy() {
			TTSHelper.Stop();
        }

    }

}
