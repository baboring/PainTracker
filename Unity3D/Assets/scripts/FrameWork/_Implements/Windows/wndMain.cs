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

			EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates);

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
	}
}