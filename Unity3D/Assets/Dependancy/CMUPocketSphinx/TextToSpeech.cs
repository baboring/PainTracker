using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace CMUPocketSphinx
{
    public class TextToSpeech : MonoBehaviour
    {
        private const string FUNC_LOG = "_OnLog";

        private string _currentGUID;

        // initialization
        void Awake()
        {
            _currentGUID = System.Guid.NewGuid().ToString();
            gameObject.name = gameObject.name + _currentGUID;
        }

        void Start() {
			TTSHelper.Initialize(gameObject.name, TTSHelper.UnitedKingdom);
		}

		public void _OnLog(string msg)
        {
            Debug.Log(msg);
        }

		public void _OnInitialized(string msg) {
			Debug.Log(msg);
		}
		public void _OnStart(string msg) {
			Debug.Log(msg);
		}
		public void _OnDone(string msg) {
			Debug.Log(msg);
		}

		public void Destroy() {
			TTSHelper.Stop();
        }

    }

}
