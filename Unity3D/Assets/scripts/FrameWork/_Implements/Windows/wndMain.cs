using UnityEngine;
using System;
using System.Collections;
using CMUPocketSphinx;

namespace HC
{
    public class wndMain : WindowBase , OnVoiceRecognizeListener {
		public GameObject objRecording;
		public GameObject objListening;
		public GameObject objMenuButton;
		public GameObject objBtnNext;
		public UILabel lblDebug;

		// variables for buttons & labels
		public GameObject objBtnOk;
		public GameObject objBtnBack;

		public UILabel lblTextSpeech;
		public UILabel lblHelpMessage;

		// for debug
		public static string szDebug = "";

		Action<eMessageButton> OnYesNolistener;

		// help text
		public string SpeechText {
			get { return lblTextSpeech.text; }
			set { lblTextSpeech.text = value; }
		}

		public string HelpMessage {
			get { return lblHelpMessage.text; }
			set { lblHelpMessage.text = value; }
		}

		// Use this for initialization
		protected override void Awake() {
            eWindowID = WndID.WndMain;
            base.Awake();

			ButtonHandler.CreateHandle(0, objMenuButton, true, true, (btn) => {
				AppStateManager.ChangeState(eAppState.Menu);
			});

			// stop button
			ButtonHandler.CreateHandle(1, objRecording, true, true, (btn) => {
				if(VoiceRecognizeMain.listenSt == ListenState.Start)
					//&& VoiceRecognizerHelper.CurrentSearchName() != VoiceRecognizerHelper.SEARCH_KWS)
					VoiceRecognizerHelper.StopListening();
			});

			ButtonHandler.CreateHandle(0, objBtnOk, true, true, (btn) => {
				if (null != OnYesNolistener)
					OnYesNolistener(eMessageButton.ID_YES);
			});
			ButtonHandler.CreateHandle(0, objBtnBack, true, true, (btn) => {
				if (null != OnYesNolistener)
					OnYesNolistener(eMessageButton.ID_NO);
			});


			// Test Next
			objBtnNext.SetActive(Application.platform == RuntimePlatform.WindowsEditor);
			ButtonHandler.CreateHandle(0, objBtnNext, true, true, (btn) => {
				VoiceRecognizeMain.instance._OnHypothsisPartial(SeqManager.instance.fakeSpeech);
				VoiceRecognizeMain.instance._OnEndOfSpeech(SeqManager.instance.fakeSpeech);

				//VoiceRecognizerHelper.SwitchSearch(VoiceRecognizerHelper.SEARCH_KWS);
			});

			// initial 
			lblDebug.text = "";
			lblHelpMessage.text = "";
			lblTextSpeech.text = "";

			objBtnOk.SetActive(false);
			objBtnBack.SetActive(false);
		}

		void Start() {
			VoiceRecognizeMain.lstListener.Add(this);
		}

		protected override void OnOpenWindow() {
            base.OnOpenWindow();

            // 순차적 진행
            StartCoroutine(Initialize());
        }

		IEnumerator Initialize()
        {
			SpeechText = "";
			objRecording.SetActive(false);
			objListening.SetActive(false);
			yield return null;

			if(string.IsNullOrEmpty(PlayerSaveInfo.instance.userName_)) {
				WindowManager.OpenPopup(WndID.WndRegister);
				while(string.IsNullOrEmpty(PlayerSaveInfo.instance.userName_))
					yield return new WaitForFixedUpdate();
			}
			Logger.Debug("WindowMain Done!!");

			// display motion
			//Human.instance.Motion("standing_greeting");

			yield return new WaitForSeconds(2);
			SeqManager.instance.RunMainRoutine();

		}

		// confirm button 확인 유무처리
		public void SetConfirmButtons(bool bShow, Action<eMessageButton> listener = null) {
			objBtnOk.SetActive(bShow);
			objBtnBack.SetActive(bShow);
			OnYesNolistener = listener;
		}


		// Display enabled state of speech

		// Display voice wave
		void ShowWave(bool flag, string msg) {
			objListening.SetActive(flag);
		}

		public void _DoInBackground(string msg) { }
		public void _OnPostExecute(string msg) { }
		// callback function
		public void _OnLog(string msg) {

		}
		public void _OnWakeup(string msg) {
			// start routine
			SeqManager.instance.OnStartup();
		}

		int timerID = -1;

		// partial text
		public void _OnHypothsisPartial(string text) {
			SpeechText = text;
			Timer.CancelTimer(timerID);
			timerID = Timer.SetTimer(1000, (id) => SpeechText = "");
			if (SeqManager.instance.CompareCodition(text)) {
				// match case
				VoiceRecognizerHelper.StopListening();
			}
		}
		public void _OnHypothsisFinal(string text) {
			//VoiceRecognizerHelper.ToastShow(text);
		}


		public void _OnBeginningOfSpeech(string msg) {
			ShowWave(true, msg);
		}
		public void _OnEndOfSpeech(string text) {
			ShowWave(false, text);
			//if (SeqManager.instance.CompareCodition(text))
			//	VoiceRecognizerHelper.StopListening();
		}
		// Display Message
		public void _OnStartListening(string search) {
			wndMain.szDebug = search;

			objRecording.SetActive(true);
			switch (search) {
				case "wakeup":
					HelpMessage = "(Say) Hello, Benjamin!!";
					break;
				case "body":
					HelpMessage = "Which part do you have pain? ( \"Head\", \"Stomach\", \"Thigh\" )";
					break;
			}
		}
		public void _OnStopListening(string msg) {
			objRecording.SetActive(false);
		}
		public void _OnCancelListening(string msg) {
			objRecording.SetActive(false);
		}
		public void _OnSwitchSearch(string msg) { }
		public void _OnTimeout(string msg) { }


		[SerializeField]
		private bool _isVisible = false;
		public void SetVisible(bool visibility) {
			if (_isVisible == visibility) return;
			_isVisible = visibility;
			if(!_isVisible)
				lblDebug.text = "";
		}

		public KeyCode showByKey = KeyCode.Tab;
		void Update() {
			if (showByKey != KeyCode.None && Input.GetKeyDown(showByKey)) {
				SetVisible(!_isVisible);
			}
			if(_isVisible)
				lblDebug.text = "debug\n----------\n" + szDebug;
		}
	}
}