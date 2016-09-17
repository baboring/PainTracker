using UnityEngine;
using System.Collections;

namespace HC {
	public class Main : ManualSingletonMB<Main> {

		// Use this for initialization
		void Awake() {
			instance = this;
			DontDestroyOnLoad(instance);
			MainStateManager.CreateInstance();
		}

		// Use this for initialization
		void Start() {
			OnStartup();
		}

		public void OnStartup() {
			MainStateManager.ChangeState(eMainState.Splash);
		}

		public void OnClickBackKey() {
			if(MainStateManager.CurrentState == eMainState.Main)
				Application.Quit();
		}

		// Update is called once per frame
		void Update() {

			// 종료 버튼 ( Back key )
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (Application.platform == RuntimePlatform.Android) {
					OnClickBackKey();
				}
				else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {

				}
			}
		}
	}
}