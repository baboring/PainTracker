using UnityEngine;

using System;
using System.Collections;

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

#if UNITY_ANDROID && !UNITY_EDITOR
                bool result = SphinxPluginAndroid._Create();
#elif UNITY_IPHONE && !UNITY_EDITOR

#endif
            }
            else {

            }
        }

        public Action<string> callback;
        public void _OnLog(string msg)
        {
            Debug.Log(msg);
            if (null != callback)
                callback(msg);
        }

        public void _OnHypothsisPartialResult(string text) {
            _OnLog(text);
        }
        public void _OnHypothsisResult(string text) {
            _OnLog(text);
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
