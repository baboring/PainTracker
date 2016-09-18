using UnityEngine;
using System.Collections;

namespace HC
{
    public class wndMain : WindowBase
    {
		public GameObject objRecording;
		public GameObject objListening;
		public GameObject objMenuButton;

		public UILabel lblTextSpeech;
		public UILabel lblHelpMessage;

		public Human objHuman;

		// Use this for initialization
		protected override void Awake()
        {
            eWindowID = WndID.WndMain;
            base.Awake();

			ButtonHandler.CreateHandle(0, objMenuButton, true, true, (btn) => {
				MainStateManager.ChangeState(eMainState.Menu);
			});

		}

        protected override void OnOpenWindow()
        {
            base.OnOpenWindow();

            // 순차적 진행
            StartCoroutine(progressInitial());
        }

		IEnumerator progressInitial()
        {
            yield return new WaitForFixedUpdate();


			//nameArray = EasyTTSUtil.GetEngineNameArray();
			//pkgArray = EasyTTSUtil.GetEnginePkgArray();

			//if (null != pkgArray)
			//	foreach (var item in pkgArray) {
			//		popupList.AddItem(item);
			//	}

			//// welcome message
			//yield return new WaitForSeconds(2);
			////EasyTTSUtil.SpeechAdd("Welcome");

			lblHelpMessage.text = "Hello, Pretty Aden";
			SetSpeechText("");
			objRecording.SetActive(false);
			objListening.SetActive(false);

			Logger.Debug("WindowMain Done!!");
            yield return new WaitForSeconds(2);
        }

		public void SetSpeechText(string text)
		{
			lblTextSpeech.text = text;
			objHuman.filterAnimation(text);
		}
		public void SetHelpMessage(string text)
		{
			lblHelpMessage.text = text;
		}

		// Display enabled state of speech
		public void OnVoiceRecordOn()
		{
			objRecording.SetActive(true);
		}
		public void OnVoiceRecordOff()
		{
			objRecording.SetActive(false);
		}
		public void OnVoiceRecording()
		{
			objRecording.SetActive(true);
		}

		// Display voice wave
		public void ShowWave(bool flag, string msg)
		{
			objListening.SetActive(flag);
		}

		// Display Message
		public void OnStartListening(string mode)
		{
			switch (mode) {
				case "wakeup":
					SetHelpMessage("(Say) Hello, Pretty Aden!!");
					break;
				case "body":
					SetHelpMessage("Which part do you have pain? ( \"Head\", \"Stomach\", \"Thigh\" )");
					break;
			}
		}

		public void OnWakeup(string msg)
		{
			OnVoiceRecordOff();
			objHuman.OnSay("Which part do you have pain?", "shaking_hands_2");
			CameraController.instance.SetZoomInOut(ZOOM_ID.NONE, false);
		}

		public void OnSaySomething(string txt)
		{
			OnVoiceRecordOff();
			TTSHelper.SpeechFlush(txt);
			Debug.Log("OnSaySomething : " + txt);
			WindowManager.GetWindow<wndMain>().objHuman.filterAnimation(txt);

			switch (txt) {
				case "head":
					CameraController.instance.SetZoomInOut(ZOOM_ID.HEAD, true);
					break;
				case "thigh":
					CameraController.instance.SetZoomInOut(ZOOM_ID.THIGH, true);
					break;
				case "stomach":
					CameraController.instance.SetZoomInOut(ZOOM_ID.STOMACK, true);
					break;
				default:
					CameraController.instance.SetZoomInOut(ZOOM_ID.NONE, false);
					break;
			}
		}
	}
}