using UnityEngine;
using System.Collections;
using CMUPocketSphinx;

namespace HC {
    public class ShpinxVoiceRecognizeDemo : MonoBehaviour {

        VoiceRecognizer recognizer;
        // Use this for initialization
        void Start() {
            // attach Voice Recognizer
            recognizer = gameObject.AddComponent<VoiceRecognizer>();

            recognizer.callbackSay = WindowManager.GetWindow<wndMain>().OnSaySomething;

            OnVoiceRecordOff();
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

		public void _OnPostExecute(string msg) {
            OnVoiceRecordOn();
        }

        public void _OnHypothsisPartialResult(string text) {

			WindowManager.GetWindow<wndMain>().SetHelpMessage("");
			WindowManager.GetWindow<wndMain>().SetSpeechText(text);
        }

        public void _OnHypothsisResult(string text) {
			WindowManager.GetWindow<wndMain>().SetSpeechText("");
            CMUSphinxAndroid.ToastShow(text);
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
    }

}