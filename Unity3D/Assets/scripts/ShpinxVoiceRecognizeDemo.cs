using UnityEngine;
using System.Collections;
using PocketSphinx;

namespace HC {
    public class ShpinxVoiceRecognizeDemo : MonoBehaviour {

        SphinxVoiceRecognizer recognizer;
        // Use this for initialization
        void Start() {
            if (SystemConfig.IsDebugOn) {
                var console = gameObject.AddComponent<uLinkConsoleGUI>();
                console.showByKey = KeyCode.Menu;
                console.SetVisible(false);
            }

            // attach Voice Recognizer
            recognizer = gameObject.AddComponent<SphinxVoiceRecognizer>();

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
            Human.instance.filterAnimation(txt);

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
            Main.instance.objRecording.SetActive(true);
        }
        void OnVoiceRecordOff() {
            Main.instance.objRecording.SetActive(false);
        }
        void OnVoiceRecording() {
            Main.instance.objRecording.SetActive(true);
        }

        public void _OnPostExecute(string msg) {
            OnVoiceRecordOn();
        }


        public void _OnHypothsisPartialResult(string text) {
            Main.instance.lblHelpMessage.text = "";
            Main.instance.SetSpeechText(text);
            Human.instance.filterAnimation(text);
        }
        public void _OnHypothsisResult(string text) {
            Main.instance.SetSpeechText("");
            SphinxPluginAndroid._ToastShow(text);
        }

        public void _OnWakeup(string msg) {
            OnVoiceRecordOff();
            EasyTTSUtil.SpeechAdd(string.Format("Hello, {0}. Which part do you have pain?", Main.instance.myName));
            Main.instance.SetZoomInOut(ZOOM_ID.NONE, false);
        }


        public void _OnStopLisnening(string msg) {
            OnVoiceRecordOff();
        }
        public void _OnStartListening(string msg) {
            OnVoiceRecordOn();
            switch(msg){
                case "wakeup":
                    Main.instance.lblHelpMessage.text = "(Say) Hello, Pretty Aden!!";
                    break;
                case "body":
                    Main.instance.lblHelpMessage.text = "Which part do you have pain? ( \"Head\", \"Stomach\", \"Thigh\" )";
                    break;
            }
        }


        public void _OnBeginningOfSpeech(string msg) {
            Main.instance.objListening.SetActive(true);
        }
        public void _OnEndOfSpeech(string msg) {
            Main.instance.objListening.SetActive(false);
        }

        public void _OnTimeout(string msg) {
        }
    }

}