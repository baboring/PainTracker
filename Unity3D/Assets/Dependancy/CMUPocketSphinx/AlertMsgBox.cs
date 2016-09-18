using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace CMUPocketSphinx
{
    public class AlertMsgBox : SingletonMB<AlertMsgBox>
    {
        private const string FUNC_LOG = "_OnLog";

        private string _currentGUID;
		private AndroidJavaClass javaClass;

		// initialization
		protected override void Awake()
        {
            _currentGUID = System.Guid.NewGuid().ToString();
            gameObject.name = gameObject.name + _currentGUID;
			if (Application.platform == RuntimePlatform.Android)
				javaClass = new AndroidJavaClass("com.narith.pocketsphinx.AlertDialogBox");
		}

		public void _OnLog(string msg)
        {
            Debug.Log(msg);
        }

		public void _OnYes(string msg) {
			Debug.Log(msg);
		}
		public void _OnNo(string msg) {
			Debug.Log(msg);
		}

    }

}
