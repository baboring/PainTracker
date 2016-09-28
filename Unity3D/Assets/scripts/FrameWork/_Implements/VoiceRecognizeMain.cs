using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CMUPocketSphinx;

namespace HC {

	public enum ListenState {
		Stop = 0,
		Start,
		Changing,
	}

	public class VoiceRecognizeMain : MonoBehaviour, OnVoiceRecognizeListener {

		public static List<OnVoiceRecognizeListener> lstListener = new List<OnVoiceRecognizeListener>();

		public static VoiceRecognizeMain instance;
		public static void CreateInstance() {
			if (null == instance) {
				string _currentGUID = System.Guid.NewGuid().ToString();
				GameObject obj = new GameObject("VoiceRecognizeMain("+ _currentGUID+")");
				instance = obj.AddComponent<VoiceRecognizeMain>();
				DontDestroyOnLoad(obj);
			}
		}

		public static ListenState listenSt { get; set; }
		public static bool IsInstalled { get; private set; }
		// Use this for initialization
		IEnumerator Start() {
			// 임의로 설정 된것으로 처리 하자
			if (Application.platform != RuntimePlatform.Android) {
				VoiceRecognizerHelper.Initialize(this.gameObject);
				yield break;
			}
			Logger.Debug("VoiceRecognizeMain Initial!");
			yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
			if (Application.HasUserAuthorization(UserAuthorization.Microphone)) {
				VoiceRecognizerHelper.Initialize(this.gameObject);
			}
		}

		public void Destroy() {
			VoiceRecognizerHelper.Dispose();
		}

		public void _DoInBackground(string msg) {

		}

		public void _OnPostExecute(string msg) {
			Logger.Debug("_OnPostExecute : " + msg );
			if(msg.Equals("success"))
				IsInstalled = true;
		}

		public void _OnHypothsisPartial(string text) {
			foreach(var va in lstListener)
				va._OnHypothsisPartial(text);

			// 윈도우 에디터 전용
			if (Application.platform == RuntimePlatform.WindowsEditor) {
				// wakeup 처리
				switch (VoiceRecognizerHelper.CurrentSearchName()) {
					case VoiceRecognizerHelper.SEARCH_KWS:
						if (text.Equals(VoiceRecognizerHelper.KEYPHRASE)) {
							_OnWakeup(text);
							VoiceRecognizerHelper.StopListening();
						}
						break;
				}
			}
		}

        public void _OnHypothsisFinal(string text) {
			Logger.Debug("_OnHypothsisFinal : " + text);
			foreach (var va in lstListener)
				va._OnHypothsisFinal(text);
        }

        public void _OnWakeup(string msg) {
			Logger.Debug("_OnWakeup: " + msg);
			foreach (var va in lstListener)
				va._OnWakeup(msg);
        }

        public void _OnStopListening(string mode) {
			listenSt = ListenState.Stop;
			foreach (var va in lstListener)
				va._OnStopListening(mode);
        }
        public void _OnStartListening(string search) {
			listenSt = ListenState.Start;
			foreach (var va in lstListener)
				va._OnStartListening(search);
        }
		public void _OnCancelListening(string mode) {
			listenSt = ListenState.Stop;
			foreach (var va in lstListener)
				va._OnCancelListening(mode);
		}
		public void _OnSwitchSearch(string mode) {
			foreach (var va in lstListener)
				va._OnSwitchSearch(mode);
		}

		public void _OnBeginningOfSpeech(string msg) {
			foreach (var va in lstListener)
				va._OnBeginningOfSpeech(msg);
        }
        public void _OnEndOfSpeech(string msg) {
			listenSt = ListenState.Stop;
			foreach (var va in lstListener)
				va._OnEndOfSpeech(msg);
        }

        public void _OnTimeout(string msg) {
        }


		public void _OnLog(string msg) {
		}

	}

}