using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace CMUPocketSphinx
{

	public class VoiceRecognizer : MonoBehaviour, OnVoiceRecognizeListener {

		public OnVoiceRecognizeListener _interrupt;

		public void Initialize(OnVoiceRecognizeListener listener) {
			_interrupt = listener;
			StartCoroutine(coInitial());
		}
		// initialization
		void Awake()
        {
			string _currentGUID = System.Guid.NewGuid().ToString();
			gameObject.name = gameObject.name + "("+ _currentGUID + ")";
		}

		void Start() {
			if(null == _interrupt)
				StartCoroutine(coInitial());
		}

		// initial
		IEnumerator coInitial() {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (Application.HasUserAuthorization(UserAuthorization.Microphone)) {
				VoiceRecognizerHelper.Initialize(gameObject.name);
			}
			else {

            }
        }

        public void _OnLog(string msg)
        {
            Debug.Log(msg);
        }
		
		public void _DoInBackground(string msg) {
			if (null != _interrupt) {
				_interrupt._DoInBackground(msg);
				return;
			}

		}
		public void _OnPostExecute(string msg) {
			if (null != _interrupt) {
				_interrupt._OnPostExecute(msg);
				return;
			}
		}

		// override 
		public void _OnWakeup(string msg) {
			if (null != _interrupt) {
				_interrupt._OnWakeup(msg);
				return;
			}
		}

		//void AnswerAndRestart(string text) {
		//	VoiceRecognizerHelper.StopListening();
		//	if (null != callbackSay)
		//		callbackSay(text);

		//	//SetTimer(0, 1200, () => {
		//	//    CMUSphinxAndroid.StartListening(CMUSphinxAndroid.KWS_SEARCH);
		//	//    //if (text.Equals(SphinxPluginAndroid.BODY_SEARCH))
		//	//    //    SphinxPluginAndroid._StartListening(text, 10000);
		//	//    //else if (text.Equals(SphinxPluginAndroid.DIGITS_SEARCH))
		//	//    //    SphinxPluginAndroid._StartListening(text, 10000);
		//	//    //else if (text.Equals(SphinxPluginAndroid.PHONE_SEARCH))
		//	//    //    SphinxPluginAndroid._StartListening(text, 10000);
		//	//    //else if (text.Equals(SphinxPluginAndroid.FORECAST_SEARCH))
		//	//    //    SphinxPluginAndroid._StartListening(text, 10000);
		//	//});
		//}


		public void _OnHypothsisPartial(string text) {
			if (null != _interrupt) {
				_interrupt._OnHypothsisPartial(text);
				return;
			}

			switch (VoiceRecognizerHelper.CurrentSearchName()) {
                case VoiceRecognizerHelper.KWS_SEARCH:
                    if (text.Equals(VoiceRecognizerHelper.KEYPHRASE))
						VoiceRecognizerHelper.SwitchSearch(VoiceRecognizerHelper.MENU_SEARCH);
                    break;
                // select menu
                case VoiceRecognizerHelper.MENU_SEARCH:
                    
                    if (text.Equals(VoiceRecognizerHelper.BODY_SEARCH))
                        VoiceRecognizerHelper.SwitchSearch(VoiceRecognizerHelper.BODY_SEARCH);
                    else if (text.Equals(VoiceRecognizerHelper.DIGITS_SEARCH))
                        VoiceRecognizerHelper.SwitchSearch(VoiceRecognizerHelper.DIGITS_SEARCH);
                    else if (text.Equals(VoiceRecognizerHelper.FORECAST_SEARCH))
                        VoiceRecognizerHelper.SwitchSearch(VoiceRecognizerHelper.FORECAST_SEARCH);
                    else if (text.Equals(VoiceRecognizerHelper.GREET_SEARCH))
                        VoiceRecognizerHelper.SwitchSearch(VoiceRecognizerHelper.GREET_SEARCH);
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
                case VoiceRecognizerHelper.BODY_SEARCH:
                    VoiceRecognizerHelper.StopListening();
                    break;
                default:
                    break;
            }
        }


        public void _OnHypothsisFinal(string text) {
			if (null != _interrupt) {
				_interrupt._OnHypothsisFinal(text);
				return;
			}
			Debug.Log("_OnHypothsisResult::" + text);

            switch (VoiceRecognizerHelper.CurrentSearchName()) {
                case VoiceRecognizerHelper.BODY_SEARCH:
                    //SphinxPluginAndroid._SwitchSearch("_");
                    //AnswerAndRestart(text);
                    break;
                default:
                    break;
            }

        }
        public void _OnBeginningOfSpeech(string msg) {
			if (null != _interrupt) {
				_interrupt._OnBeginningOfSpeech(msg);
				return;
			}
		}
		public void _OnEndOfSpeech(string msg) {
			if (null != _interrupt) {
				_interrupt._OnEndOfSpeech(msg);
				return;
			}
		}
		public void _OnStopLisnening(string msg) {
			if (null != _interrupt) {
				_interrupt._OnStopLisnening(msg);
				return;
			}
		}
		public void _OnStartListening(string msg) {
			if (null != _interrupt) {
				_interrupt._OnStartListening(msg);
				return;
			}
		}
		public void _OnTimeout(string msg) {
			if (null != _interrupt) {
				_interrupt._OnTimeout(msg);
				return;
			}

			VoiceRecognizerHelper.SwitchSearch(VoiceRecognizerHelper.KWS_SEARCH);
        }

        public void Destroy() {
            VoiceRecognizerHelper.Dispose();
        }

    }

}
