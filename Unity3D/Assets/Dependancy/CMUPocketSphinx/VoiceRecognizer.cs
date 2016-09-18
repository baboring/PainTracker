using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace CMUPocketSphinx
{

	public class VoiceRecognizer : MonoBehaviour, OnVoiceRecognizeListener {
        
        private string _currentGUID;

		public static OnVoiceRecognizeListener listener;
		// initialization
		void Awake()
        {
			_currentGUID = System.Guid.NewGuid().ToString();
            gameObject.name = gameObject.name + _currentGUID;
        }

        IEnumerator Start() {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (Application.HasUserAuthorization(UserAuthorization.Microphone)) {
				CMUSphinxAndroid.Initialize(gameObject.name);
			}
			else {

            }
        }

        public void _OnLog(string msg)
        {
            Debug.Log(msg);
        }



        public Action<string> callbackSay;
        public void _OnWakeup(string msg) {
        }

        public void _OnHypothsisPartialResult(string text) {
            switch (CMUSphinxAndroid.CurrentSearchName()) {
                case CMUSphinxAndroid.KWS_SEARCH:
                    //if (text.Equals(SphinxPluginAndroid.KWS_PHRASE))
                    //SphinxPluginAndroid._SwitchSearch(SphinxPluginAndroid.MENU_SEARCH);
                    break;
                // select menu
                case CMUSphinxAndroid.MENU_SEARCH:
                    
                    if (text.Equals(CMUSphinxAndroid.BODY_SEARCH))
                        CMUSphinxAndroid.SwitchSearch(CMUSphinxAndroid.BODY_SEARCH);
                    else if (text.Equals(CMUSphinxAndroid.DIGITS_SEARCH))
                        CMUSphinxAndroid.SwitchSearch(CMUSphinxAndroid.DIGITS_SEARCH);
                    else if (text.Equals(CMUSphinxAndroid.FORECAST_SEARCH))
                        CMUSphinxAndroid.SwitchSearch(CMUSphinxAndroid.FORECAST_SEARCH);
                    else if (text.Equals(CMUSphinxAndroid.GREET_SEARCH))
                        CMUSphinxAndroid.SwitchSearch(CMUSphinxAndroid.GREET_SEARCH);
                    //switch(text) {
                    //    case SphinxPluginAndroid.BODY_SEARCH:
                    //    case SphinxPluginAndroid.DIGITS_SEARCH:
                    //    case SphinxPluginAndroid.PHONE_SEARCH:
                    //    case SphinxPluginAndroid.FORECAST_SEARCH:
                    //    case SphinxPluginAndroid.GREET_SEARCH:
                    //        AnswerAndStart(text);
                    //        break;
                    //}

                    break;
                case CMUSphinxAndroid.BODY_SEARCH:
                    CMUSphinxAndroid.StopListening();
                    break;
                default:
                    break;
            }
        }

        void AnswerAndRestart(string text) {
            CMUSphinxAndroid.StopListening();
            if (null != callbackSay)
                callbackSay(text);

            //SetTimer(0, 1200, () => {
            //    CMUSphinxAndroid.StartListening(CMUSphinxAndroid.KWS_SEARCH);
            //    //if (text.Equals(SphinxPluginAndroid.BODY_SEARCH))
            //    //    SphinxPluginAndroid._StartListening(text, 10000);
            //    //else if (text.Equals(SphinxPluginAndroid.DIGITS_SEARCH))
            //    //    SphinxPluginAndroid._StartListening(text, 10000);
            //    //else if (text.Equals(SphinxPluginAndroid.PHONE_SEARCH))
            //    //    SphinxPluginAndroid._StartListening(text, 10000);
            //    //else if (text.Equals(SphinxPluginAndroid.FORECAST_SEARCH))
            //    //    SphinxPluginAndroid._StartListening(text, 10000);
            //});
        }

        public void _OnHypothsisResult(string text) {
            Debug.Log("_OnHypothsisResult::" + text);

            switch (CMUSphinxAndroid.CurrentSearchName()) {
                case CMUSphinxAndroid.BODY_SEARCH:
                    //SphinxPluginAndroid._SwitchSearch("_");
                    AnswerAndRestart(text);
                    break;
                default:
                    break;
            }

        }
        public void _DoInBackground(string msg) {

        }
        public void _OnPostExecute(string msg) {

        }
        public void _OnBeginningOfSpeech(string msg) {

        }
        public void _OnEndOfSpeech(string msg) {

        }

        public void _OnStopLisnening(string msg) {

        }
        public void _OnStartListening(string msg) {

        }


        public void _OnTimeout(string msg) {
            CMUSphinxAndroid.SwitchSearch(CMUSphinxAndroid.KWS_SEARCH);
        }

        public void Destroy() {
            CMUSphinxAndroid.Dispose();
        }

    }

}
