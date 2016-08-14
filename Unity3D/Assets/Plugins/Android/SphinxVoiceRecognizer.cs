using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace PocketSphinx
{
    public class SphinxVoiceRecognizer : MonoBehaviour
    {
        private const string FUNC_LOG = "_OnLog";

        private string _currentGUID;

        // initialization
        void Awake()
        {
            _currentGUID = System.Guid.NewGuid().ToString();
            gameObject.name = gameObject.name + _currentGUID;
            SphinxPluginAndroid.Init(gameObject.name);
        }

        IEnumerator Start() {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (Application.HasUserAuthorization(UserAuthorization.Microphone)) {
                bool result = SphinxPluginAndroid._Create(false);
            }
            else {

            }
            InvokeRepeating("decreaseTimeRemaining", 1, 0.1f);
        }

        public void _OnLog(string msg)
        {
            Debug.Log(msg);
        }

        /// <summary>
        /// Timer 
        /// </summary>
        void decreaseTimeRemaining() {
          foreach (var t in lstIimer)
                t.Elapsed(100);
            lstIimer.RemoveAll(item => item.IsTimeOver);
        }

        class TIMER {
            public int id;
            public Action callback;
            public int remainTime;

            public bool IsTimeOver {get{return (remainTime <= 0);}}       
            public bool Elapsed(int time) {
                remainTime -= time;
                //Debug.Log(string.Format("time {0} {1}",id,remainTime));
                if (IsTimeOver) {
                    callback();
                    callback = null;
                    return true;
                }
                return false;
            }
        }
        List<TIMER> lstIimer = new List<TIMER>();

        static int time_uid = 0;
        void SetTimer(int id, int ms, Action listener) {
            lstIimer.Add(new TIMER() {
                id = time_uid++,
                remainTime = ms,
                callback = listener
            });
        }
  


        public Action<string> callbackSay;
        public void _OnWakeup(string msg) {
            //SphinxPluginAndroid._StopListening();
            SetTimer(0, 4000, () => {
                SphinxPluginAndroid._StartListening(SphinxPluginAndroid.BODY_SEARCH, 10000);
            });
        }

        public void _OnHypothsisPartialResult(string text) {
            switch (SphinxPluginAndroid._CurrentSearchName()) {
                case SphinxPluginAndroid.KWS_SEARCH:
                    //if (text.Equals(SphinxPluginAndroid.KWS_PHRASE))
                    //SphinxPluginAndroid._SwitchSearch(SphinxPluginAndroid.MENU_SEARCH);
                    break;
                // select menu
                case SphinxPluginAndroid.MENU_SEARCH:
                    
                    if (text.Equals(SphinxPluginAndroid.BODY_SEARCH))
                        SphinxPluginAndroid._SwitchSearch(SphinxPluginAndroid.BODY_SEARCH);
                    else if (text.Equals(SphinxPluginAndroid.DIGITS_SEARCH))
                        SphinxPluginAndroid._SwitchSearch(SphinxPluginAndroid.DIGITS_SEARCH);
                    else if (text.Equals(SphinxPluginAndroid.PHONE_SEARCH))
                        SphinxPluginAndroid._SwitchSearch(SphinxPluginAndroid.PHONE_SEARCH);
                    else if (text.Equals(SphinxPluginAndroid.FORECAST_SEARCH))
                        SphinxPluginAndroid._SwitchSearch(SphinxPluginAndroid.FORECAST_SEARCH);
                    else if (text.Equals(SphinxPluginAndroid.GREET_SEARCH))
                        SphinxPluginAndroid._SwitchSearch(SphinxPluginAndroid.GREET_SEARCH);
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
                case SphinxPluginAndroid.BODY_SEARCH:
                    SphinxPluginAndroid._StopListening();
                    break;
                default:
                    break;
            }
        }

        void AnswerAndRestart(string text) {
            SphinxPluginAndroid._StopListening();
            if (null != callbackSay)
                callbackSay(text);

            SetTimer(0, 1200, () => {
                SphinxPluginAndroid._StartListening(SphinxPluginAndroid.KWS_SEARCH);
                //if (text.Equals(SphinxPluginAndroid.BODY_SEARCH))
                //    SphinxPluginAndroid._StartListening(text, 10000);
                //else if (text.Equals(SphinxPluginAndroid.DIGITS_SEARCH))
                //    SphinxPluginAndroid._StartListening(text, 10000);
                //else if (text.Equals(SphinxPluginAndroid.PHONE_SEARCH))
                //    SphinxPluginAndroid._StartListening(text, 10000);
                //else if (text.Equals(SphinxPluginAndroid.FORECAST_SEARCH))
                //    SphinxPluginAndroid._StartListening(text, 10000);
            });
        }

        public void _OnHypothsisResult(string text) {
            Debug.Log("_OnHypothsisResult::" + text);

            switch (SphinxPluginAndroid._CurrentSearchName()) {
                case SphinxPluginAndroid.BODY_SEARCH:
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
            SphinxPluginAndroid._SwitchSearch(SphinxPluginAndroid.KWS_SEARCH);
        }
        public void Notice(string msg)
        {
            SphinxPluginAndroid._DispatchMessage(FUNC_LOG, msg);
        }

        public void Dispose() {
            SphinxPluginAndroid._Dispose();
        }

    }

}
