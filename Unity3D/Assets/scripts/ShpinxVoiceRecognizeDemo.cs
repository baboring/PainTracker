using UnityEngine;
using System.Collections;
using CMUPocketSphinx;

namespace HC {
    public class ShpinxVoiceRecognizeDemo : MonoBehaviour {

        VoiceRecognizer recognizer;
        // Use this for initialization
        void Start() {
            if (SystemConfig.IsDebugOn) {
                var console = gameObject.AddComponent<uLinkConsoleGUI>();
                console.showByKey = KeyCode.Menu;
                console.SetVisible(false);
            }

            // attach Voice Recognizer
            recognizer = gameObject.AddComponent<VoiceRecognizer>();

            recognizer.callbackSay = OnSaySomething;

            OnVoiceRecordOff();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKey(KeyCode.Escape))
                Application.Quit();
        }
        void OnSaySomething(string txt) {
            OnVoiceRecordOff();
            EasyTTSUtil.SpeechAdd(txt);
            Debug.Log("OnSaySomething : " + txt);
			WindowManager.GetWindow<wndMain>().objHuman.filterAnimation(txt);

            switch (txt) {
                case "head" :
                    Main.instance.SetZoomInOut(ZOOM_ID.HEAD, true);
                    break;
                case "thigh":
                    Main.instance.SetZoomInOut(ZOOM_ID.THIGH, true);
                    break;
                case "stomach":
                    Main.instance.SetZoomInOut(ZOOM_ID.STOMACK, true);
                    break;
                default:
                    Main.instance.SetZoomInOut(ZOOM_ID.NONE, false);
                    break;
            }
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
            CMUSphinxAndroid._ToastShow(text);
        }

        public void _OnWakeup(string msg) {
            OnVoiceRecordOff();
            EasyTTSUtil.SpeechAdd(string.Format("Hello, {0}. Which part do you have pain?", Main.instance.myName));
            Main.instance.SetZoomInOut(ZOOM_ID.NONE, false);
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