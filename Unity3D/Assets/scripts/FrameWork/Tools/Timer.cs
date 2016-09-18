using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class Timer : MonoBehaviour {

	/// <summary>
	/// Timer 
	/// </summary>
	void decreaseTimeRemaining() {
		foreach (var t in lstIimer)
			t.Elapsed(100);
		lstIimer.RemoveAll(item => item.IsTimeOver);
	}

	void Awake() {
		InvokeRepeating("decreaseTimeRemaining", 1, 0.1f);
	}

	class TIMER {
		public int id;
		public Action callback;
		public int remainTime;

		public bool IsTimeOver { get { return (remainTime <= 0); } }
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



	// Use this for initialization
	void Start () {
		//SphinxPluginAndroid._StopListening();
		SetTimer(0, 4000, () => {
			//CMUSphinxAndroid.StartListening(CMUSphinxAndroid.BODY_SEARCH, 10000);
		});

	}

	// Update is called once per frame
	void Update () {
	
	}
}
