using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace CMUPocketSphinx
{
    public class TextToSpeech : MonoBehaviour, OnTTSListener {

		public bool IsInstalled { get; private set; }

		// initialization
		void Awake() {
			string _currentGUID = System.Guid.NewGuid().ToString();
            gameObject.name = gameObject.name + _currentGUID;
			TTSHelper.Initialize(gameObject.name, TTSHelper.UnitedKingdom);
		}

		void Start() {
		}

		public void _OnLog(string msg) {
            Debug.Log(msg);
        }

		public void _OnInitialized(string msg) {
			IsInstalled = true;
			Debug.Log(msg);
		}

		public void _OnStart(string msg) {
			Debug.Log(msg);
		}

		public void _OnDone(string msg) {
			Debug.Log(msg);
		}

		public void _OnError(string msg) {
			Debug.Log(msg);
		}

		public void Destroy() {
			TTSHelper.Stop();
        }

    }

}
