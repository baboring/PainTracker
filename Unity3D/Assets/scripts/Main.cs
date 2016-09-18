using UnityEngine;
using System.Collections;

namespace HC {
	public class Main : ManualSingletonMB<Main> {

		// Use this for initialization
		void Awake() {
			instance = this;
			DontDestroyOnLoad(instance);

			if (SystemConfig.IsDebugOn) {
				var console = gameObject.AddComponent<uLinkConsoleGUI>();
				console.showByKey = KeyCode.Menu;
				console.SetVisible(false);
			}

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
			//if(MainStateManager.CurrentState == eMainState.Main) {
			//	if (Application.platform == RuntimePlatform.Android) {
			//		Main.Quit();
			//	}
			//	else if (Application.platform == RuntimePlatform.WindowsPlayer || 
			//		Application.platform == RuntimePlatform.WindowsEditor) {

					// 메시지 박스가 있으면 그것부터 끄자
					bool IsLockWindow = false;
					bool bBackProcess = false;
					if (!IsLockWindow && !bBackProcess && MessageBoxManager.IsInstanced)
						bBackProcess = MessageBoxManager.CloseMessageBox(WndCloseType.Cancel);

					if (!bBackProcess && !MessageBoxManager.IsExist("Alert Message")) {
						MessageBoxManager.Show("Quit Message", "Are you sure quit this?", eMessageBox.MB_YESNO)
							.OnButtonClick += (handle) => {
								if(handle.eButton == eMessageButton.ID_YES)
									Main.Quit();
						};

					}
			//	}
			//}
		}

		// Update is called once per frame
		void Update() {

			// 종료 버튼 ( Back key )
			if (Input.GetKeyDown(KeyCode.Escape)) {
				OnClickBackKey();
			}
		}

		public static void Quit() {
			Logger.Debug(ColorType.red, "Application.Quit");
			Application.Quit();
		}
	}
}