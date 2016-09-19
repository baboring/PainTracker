using UnityEngine;
using System;

namespace HC {
    public class TextToSpeechMain : MonoBehaviour, OnTTSListener {

		public Action<string> callbackSpeakStart;
		public Action<string> callbackSpeakDone;

		public static TextToSpeechMain instance;
		public static void CreateInstance() {
			if (null == instance) {
				string _currentGUID = System.Guid.NewGuid().ToString();
				GameObject obj = new GameObject("TextToSpeechMain(" + _currentGUID + ")");
				instance = obj.AddComponent<TextToSpeechMain>();
			}
		}
		public bool IsInstalled { get; private set; }

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
			Logger.Assert(instance.IsInstalled, "Not Initialized TTS Engine");
			TTSHelper.SpeechFlush(text);
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
			if (null != callbackSpeakStart)
				callbackSpeakStart(msg);
		}

		public void _OnDone(string msg) {
			Logger.Debug(msg);
			if (null != callbackSpeakDone)
				callbackSpeakDone(msg);
		}

		public void _OnError(string msg) {
			Logger.Debug(msg);
		}

		public void Destroy() {
			TTSHelper.Stop();
        }

    }

}
