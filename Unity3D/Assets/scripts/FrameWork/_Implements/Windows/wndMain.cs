using UnityEngine;
using System.Collections;
using CMUPocketSphinx;

namespace HC
{
    public class wndMain : WindowBase {
		public GameObject objRecording;
		public GameObject objListening;
		public GameObject objMenuButton;

		public UILabel lblTextSpeech;
		public UILabel lblHelpMessage;

		public Human objHuman;

		// Use this for initialization
		protected override void Awake() {
            eWindowID = WndID.WndMain;
            base.Awake();

			ButtonHandler.CreateHandle(0, objMenuButton, true, true, (btn) => {
				MainStateManager.ChangeState(eMainState.Menu);
			});
		}

		protected override void OnOpenWindow() {
            base.OnOpenWindow();

            // 순차적 진행
            StartCoroutine(progressInitial());
        }

		IEnumerator progressInitial()
        {
            yield return null;

			if(string.IsNullOrEmpty(PlayerSaveInfo.instance.userName_)) {
				WindowManager.OpenPopup(WndID.WndRegister);
				while(string.IsNullOrEmpty(PlayerSaveInfo.instance.userName_))
					yield return new WaitForFixedUpdate();
			}

			// test 
			TextToSpeechMain.instance.callbackSpeakStart = (msg) => {
				VoiceRecognizerHelper.StopListening();
			};
			TextToSpeechMain.instance.callbackSpeakDone = (msg) => {
				//VoiceRecognizerHelper.StartListening(VoiceRecognizerHelper.KWS_SEARCH);
				VoiceRecognizerHelper.StartListening(VoiceRecognizerHelper.GREET_SEARCH);
			};
			//TextToSpeechMain.Speak(R.Speeches.eKey.Greeting, PlayerSaveInfo.instance.userName_);
			TextToSpeechMain.Speak(R.Speeches.eKey.Welcome, PlayerSaveInfo.instance.userName_);

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
			TextToSpeechMain.Speak(R.Speeches.eKey.WhichPart);
			objHuman.Motion( "shaking_hands_2");
			CameraController.instance.SetZoomInOut(ZOOM_ID.NONE, false);
		}

		public void OnSaySomething(string txt)
		{
			OnVoiceRecordOff();
			TextToSpeechMain.Speak(txt);
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