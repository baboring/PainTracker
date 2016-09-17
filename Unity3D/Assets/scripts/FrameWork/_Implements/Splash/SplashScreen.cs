/********************************************************************
	created:	2016/09/04
	filename:	SplashScreen.cs
	author:		Benjamin
	purpose:	[]
*********************************************************************/
using UnityEngine;
using System.Collections;
using System;

namespace HC {

    public class SplashScreen : ManualSingletonMB<SplashScreen> {

        public GameObject[] splash;

        public Action OnFinish;

        private int current = -1;
        void Awake() {
            instance = this;
            foreach(var o in splash)
                if(null != o ) 
                    o.SetActive(false);
        }
        // Use this for initialization
        IEnumerator Start() {
            yield return new WaitForSeconds(0.1f);
            ChangeNext();
        }
        void TurnOff(int num) {
            if (num > -1 && num < splash.Length && null != splash[num])
                splash[num].SetActive(false);
        }
        void TurnOn(int num) {
            if (num > -1 && num < splash.Length && null != splash[num])
                splash[num].SetActive(true);
        }

        public void ChangeNext() {
            TurnOff(current);
            if (++current >= splash.Length) {
                Finish();
                return;
            }
            TurnOn(current);
        }

        void Finish() {
            if (null != OnFinish) {
                OnFinish();
                OnFinish = null;
            }
        }

        void OnApplicationPause(bool pause) {
            //동영상 플레이시(Handheld.PlayFullScreenMovie) 호출하면 유니티는 pause 상태로 들어감
            //동영상이 끝나면 유니티는 Resume 상태가 됨.
            if (pause == false)
                ChangeNext();
        }
        // Update is called once per frame
        void Update() {
            if (Input.GetMouseButtonDown(0))
                Finish();
        }
    }
}
