using UnityEngine;
using System;
using System.Collections;
using CMUPocketSphinx;

namespace HC {
    public class VoiceRecognizeMain : MonoBehaviour, OnVoiceRecognizeListener {

		public Action<string> callbackHypothis;


		public static VoiceRecognizeMain instance;
		public static void CreateInstance() {
			if (null == instance) {
				string _currentGUID = System.Guid.NewGuid().ToString();
				GameObject obj = new GameObject("VoiceRecognizeMain("+ _currentGUID+")");
				instance = obj.AddComponent<VoiceRecognizeMain>();
			}
		}

		public bool IsInstalled { get; private set; }
		// Use this for initialization
		IEnumerator Start() {
			// 임의로 설정 된것으로 처리 하자
			if (Application.platform != RuntimePlatform.Android) {
				IsInstalled = true;
				yield break;
			}

			yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
			if (Application.HasUserAuthorization(UserAuthorization.Microphone)) {
				VoiceRecognizerHelper.Initialize(gameObject.name);
			}
		}

		public void Destroy() {
			VoiceRecognizerHelper.Dispose();
		}


		void OnVoiceRecordOn() {
			WindowManager.GetWindow<wndMain>().OnVoiceRecordOn();
        }
        void OnVoiceRecordOff() {
			WindowManager.GetWindow<wndMain>().OnVoiceRecordOff();
		}
		void OnVoiceRecording() {
			WindowManager.GetWindow<wndMain>().OnVoiceRecordOn();
		}

		public void _DoInBackground(string msg) {

		}

		public void _OnPostExecute(string msg) {
			Logger.Debug("_OnPostExecute : " + msg );
			if(msg.Equals("success"))
				IsInstalled = true;
		}

		public void _OnHypothsisPartial(string text) {
			Logger.Debug("_OnHypothsisPartial : " + text);
			WindowManager.GetWindow<wndMain>().SetHelpMessage("");
			WindowManager.GetWindow<wndMain>().SetSpeechText(text);
        }

        public void _OnHypothsisFinal(string text) {
			Logger.Debug("_OnHypothsisFinal : " + text);
			WindowManager.GetWindow<wndMain>().SetSpeechText("");
            VoiceRecognizerHelper.ToastShow(text);
        }

        public void _OnWakeup(string msg) {
			WindowManager.GetWindow<wndMain>().OnWakeup(msg);
        }

        public void _OnStopLisnening(string msg) {
            OnVoiceRecordOff();
        }
        public void _OnStartListening(string mode) {
            OnVoiceRecordOn();
			WindowManager.GetWindow<wndMain>().OnStartListening(mode);
        }


        public void _OnBeginningOfSpeech(string msg) {
			WindowManager.GetWindow<wndMain>().ShowWave(true, msg);
        }
        public void _OnEndOfSpeech(string msg) {
			WindowManager.GetWindow<wndMain>().ShowWave(false, msg);
        }

        public void _OnTimeout(string msg) {
        }


		public void _OnLog(string msg) {
		}

	}

}