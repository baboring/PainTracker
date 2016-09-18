using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace CMUPocketSphinx
{
    public class TextToSpeech : MonoBehaviour, OnTTSListener {

		private string _currentGUID;

		public static OnVoiceRecognizeListener listener;

		// initialization
		void Awake() {
            _currentGUID = System.Guid.NewGuid().ToString();
            gameObject.name = gameObject.name + _currentGUID;
			TTSHelper.Initialize(gameObject.name, TTSHelper.UnitedKingdom);
		}

		void Start() {
		}

		public void _OnLog(string msg) {
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
