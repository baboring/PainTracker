using UnityEngine;
using System.Collections;
using CMUPocketSphinx;

namespace HC
{
    public class wndMenu : WindowBase
    {
        public GameObject objBtnClose;
		public GameObject objBtnSetting;
		public GameObject objBtnExtra;
		public GameObject objBtnHelp;

		bool isVisible = false;
		// Use this for initialization
		protected override void Awake()
        {
            eWindowID = WndID.WndMenu;

			base.Awake();

			// Help button
			ButtonHandler.CreateHandle(0, objBtnHelp, true, true, (btn) => {
				MessageBoxManager.Show("Help","This is Help MessageBox");
				isVisible = !isVisible;
				WindowManager.GetWindow<wndMain>().SetVisible(isVisible);
			});

			// open Setting window
			ButtonHandler.CreateHandle(0, objBtnSetting, true, true, (btn) => {
				WindowManager.OpenPopup(WndID.WndSetting);
				//test mode
				//VoiceRecognizeMain.instance.listenSt = ListenState.Stop;
				//TextToSpeechMain.instance.callbackSpeakDone("");
			});

			// extra button
			ButtonHandler.CreateHandle(0, objBtnExtra, true, true, (btn) => {
				int id = (int)CameraController.instance.zoom_id;
				CameraController.instance.SetCameraView((CAMERA_ID)(++id % 4));
			});
			// go to main scene
			ButtonHandler.CreateHandle(0, objBtnClose, true, true, (btn) => {
				AppStateManager.ChangeState(eAppState.Main);
			});

		}

		// override
		protected override void OnOpenWindow()
        {
            base.OnOpenWindow();
			// hide menu button
			WindowManager.GetWindow<wndMain>().objMenuButton.SetActive(false);
		}
		protected override void OnCloseWindow()
		{
			base.OnCloseWindow();
			// show menu button
			WindowManager.GetWindow<wndMain>().objMenuButton.SetActive(true);
		}
	}
}