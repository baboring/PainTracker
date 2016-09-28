using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMUPocketSphinx;

namespace HC {

	using System.Linq;

	public class SeqManager : SingletonMB<SeqManager> {
		public enum SeqFlow : int {
			None = 0,
			Speech,
			Response,
			Feedback,
		}


		private R.Seq.eState currentSt;

		// 흐름의 단뒤 데이터
		private SeqData currSeqData;

		private Dictionary<R.Seq.eState, SeqData> dicState = new Dictionary<R.Seq.eState, SeqData>();

		private Dictionary<SeqFlow, IEnumerable> seqTask;

		private InterruptableCoroutine iCoroutine;

		bool bWakeup = false;

		public override void Initial() {
			base.Initial();

		}

		// Use this for initialization
		protected override void Awake() {
			foreach (var val in R.AllSeq.Values) {
				dicState.Add(val.state, new SeqData(val));
			}

			currentSt = R.Seq.eState.Sleep;
			// initial state machine
			seqTask = new Dictionary<SeqFlow, IEnumerable>() {
				{SeqFlow.Speech,  taskFlowSpeakTTS()},
				{SeqFlow.Response,  taskFlowResponse()},
				{SeqFlow.Feedback,  taskFlowFeedback()},
			};

		}

		public void OnStartup() {
			bWakeup = true;
			Logger.Debug("Start Routine");
		}

		public void RunMainRoutine() {
			// test 
			TextToSpeechMain.callbackSpeakStart = (msg) => {
				//WindowManager.GetWindow<wndMain>().SetHelpMessage(msg);
				Human.instance.filterAnimation(msg);
			};

			// start coroutine
			iCoroutine = new InterruptableCoroutine(this);
			iCoroutine.Call(DoRourtine());
		}

		public string fakeSpeech {
			get {
				switch (currSeqData.state) {
					case R.Seq.eState.Sleep:
						return "hello benjamin";
					case R.Seq.eState.AskDoForYou:
						return "a headache";
					case R.Seq.eState.AskHowDensity:
						return "five";
					case R.Seq.eState.AskConfirm:
						return "that's it";
					case R.Seq.eState.AskCallup:
						return "yes";

				}
				return "";
			}
		}

		IEnumerable DoRourtine() {

			// wait for initial

			bool exit = false;
			while (!exit) {

				bWakeup = false;
				do {
					currentSt = R.Seq.eState.Sleep;
					currSeqData = dicState[currentSt];
					VoiceRecognizerHelper.SwitchSearch(currSeqData.SearchName);
					// 멈춤신호 올때 까지 대기
					while (VoiceRecognizeMain.listenSt != ListenState.Start)
						yield return null;

					// 시작 했으니 끝날때 까지 대기
					while (VoiceRecognizeMain.listenSt != ListenState.Stop)
						yield return null;

					if (!bWakeup) {
						string text = string.Format(currSeqData.GetSpeak(), PlayerSaveInfo.instance.userName_);
						TextToSpeechMain.Speak(text);
						while (TextToSpeechMain.speechSt != SpeechState.Done)
							yield return null;
					}
				} while (!bWakeup);

				// 시작은 인사 부터 하자
				currentSt = R.Seq.eState.Greet;
				yield return null;

				while (currentSt != R.Seq.eState.Sleep) {

					// 루틴 처리 하자...
					currSeqData = dicState[currentSt];
					currSeqData.Reset();

					// 단계별 처리 하자.
					StoppableCoroutine co_rtn = null;
					co_rtn = ChangeFlow(SeqFlow.Speech);
					while (co_rtn != null && !co_rtn.IsExit)
						yield return null;

					// 응답 단계
					co_rtn = ChangeFlow(SeqFlow.Response);
					while (co_rtn != null && !co_rtn.IsExit)
						yield return null;

					// 결과로 어떻게 할지 결정
					co_rtn = ChangeFlow(SeqFlow.Feedback);
					while (co_rtn != null && !co_rtn.IsExit)
						yield return null;
				}

				Logger.Debug("End of Sequence!!");
				// 마무리... 카메라 복귀
				yield return null;
				CameraController.instance.SetLookatPart(null);
			}
		}

		StoppableCoroutine ChangeFlow(SeqFlow newFlow) {
			Logger.Debug(newFlow.ToString() + "<" + currSeqData.state.ToString() +">");

			IEnumerable func;
			StoppableCoroutine co_rtn = null;
			if (seqTask.TryGetValue(newFlow,out func)) {
				co_rtn = func.MakeStoppable();
				iCoroutine.Call(co_rtn);
				//DoCoroutine(func);
			}

			if (newFlow == SeqFlow.Speech) {
				switch (currSeqData.state) {
					case R.Seq.eState.Greet:
						CameraController.instance.SetCameraView(CAMERA_ID.HEAD);
						break;
					case R.Seq.eState.AskWhich:
						CameraController.instance.SetCameraView(CAMERA_ID.NONE);
						break;
				}
			}

			//while (co_rtn != null && !co_rtn.IsExit)
			//	yield return null;

			return co_rtn;
		}
#region Flow Task
		IEnumerable taskFlowSpeakTTS() {
			yield return null;
			VoiceRecognizerHelper.StopListening();
			string text = string.Format(currSeqData.GetSpeak(), PlayerSaveInfo.instance.userName_);
			WindowManager.GetWindow<wndMain>().HelpMessage = text;
			TextToSpeechMain.Speak(text);
			// wait
			while (TextToSpeechMain.speechSt != SpeechState.Done)
				yield return null;
		}

		IEnumerable taskFlowResponse() {
			yield return null;
			if (!string.IsNullOrEmpty(currSeqData.SearchName)) {
				if (VoiceRecognizerHelper.StartListening(currSeqData.SearchName))
					VoiceRecognizeMain.listenSt = ListenState.Start;
				// wait
				while (VoiceRecognizeMain.listenSt != ListenState.Stop)
					yield return null;
			}

			// 어디인지 물어 보는 곳인데 여기서 잠시 대기 
			StoppableCoroutine co_rtn = null;
			switch (currSeqData.state) {
				case R.Seq.eState.Greet:
					currSeqData.isDone = true;
					break;
				case R.Seq.eState.AskDoForYou:
					co_rtn = task_AskCommand().MakeStoppable();
					break;
				case R.Seq.eState.AskWhich:
					co_rtn = taskSelectPartOfBody().MakeStoppable();
					break;
				case R.Seq.eState.AskHowDensity:
					co_rtn = task_Empty().MakeStoppable();
					break;
				case R.Seq.eState.CallupEmergency:
					co_rtn = task_Callup().MakeStoppable();
					break;
				case R.Seq.eState.AskConfirm:
				case R.Seq.eState.Finish:
					currSeqData.isDone = true;
					break;
			}

			// awaiting
			if (null != co_rtn) {
				//yield return DoCoroutine(co_rtn);
				iCoroutine.Call(co_rtn);
				while (!co_rtn.IsExit)
					yield return null;
			}
			Logger.Debug("End of Response!!");

		}
			// 마지막 부분
		IEnumerable taskFlowFeedback() {
			yield return null;
			// 확실하면 다음으로 그렇지 않으면?
			if (currSeqData.isDone) {
				currentSt = currSeqData.next;
				yield break;
			}

			// 여기서 알려 줘야지 다시 하라고..
			string text = "Say again, Please";
			TextToSpeechMain.Speak(text);
			// wait
			while (TextToSpeechMain.speechSt != SpeechState.Done)
				yield return null;
		}
		#endregion
		// 전화 하기
		IEnumerable task_Callup() {
			yield return null;
			string text = "Calling! " + PlayerSaveInfo.instance.userPhone_;
			WindowManager.GetWindow<wndMain>().HelpMessage = text;
			TextToSpeechMain.Speak(text);
			while (TextToSpeechMain.speechSt != SpeechState.Done)
				yield return null;

			PhoneCallHelper.CallTo(PlayerSaveInfo.instance.userPhone_);
			yield return null;

			currSeqData.isDone = true;
		}

		// 각 파트별로 하자고
		IEnumerable taskSelectPartOfBody() {
			yield return null;
			TouchPart.Reset();
			Human.instance.colliderSpin.enabled = false;
			while (TouchPart.touchedObj == null)
				yield return null;

			CameraController.instance.SetLookatPart(TouchPart.touchedObj);
			yield return new WaitForSeconds(0.5f);

			// 숨길것은 숨기자
			Human.instance.Focus(TouchPart.touchedObj);
			bool isConfirm = false;
			WindowManager.GetWindow<wndMain>().SetConfirmButtons(true,(btn) => {

				if(btn == eMessageButton.ID_YES) {
					currSeqData.isDone = true;
				}
				else {

				}
				WindowManager.GetWindow<wndMain>().SetConfirmButtons(false);
				isConfirm = true;
			});
			while(!isConfirm)
				yield return null;

			Human.instance.Focus(null);
			Human.instance.Reset();
			//CameraController.instance.SetCameraView(CAMERA_ID.HEAD);
			// end of routine
		}

		// 마지막 부분
		IEnumerable task_AskCommand() {
			yield return null;

			//currSeqData.isClear = currSeqData.CompareCodition(VoiceRecognizeMain.lastSpeech);

			// 매칭이 됐다..
			if (currSeqData.isDone) {
				foreach(var seq in dicState.Values) {
					// 찾아 보자
					if (seq != currSeqData && seq.IsMatchCases(currSeqData.matchSet)) {
						currSeqData.next = seq.state;
						break;
					}
				}
			}

		}
		IEnumerable task_Empty() {
			yield return null;
		}


		// 코루틴 시작해요
		Coroutine DoCoroutine(IEnumerator _coroutine) {
			var co = StartCoroutine(_coroutine);
			Logger.Assert(null != co, "coroutine is null : " + _coroutine);
			return co;
		}

		// 코루틴 단계별 처리용
		Coroutine DoCoroutine(IEnumerable _coroutine) {
			var co = StartCoroutine(_coroutine.GetEnumerator());
			Logger.Assert(null != co, "coroutine is null : " + _coroutine);
			return co;
		}

		public bool CompareCodition(string msg) {
			if (currSeqData.CompareCodition(msg)) {
				currSeqData.isDone = true;
				return true;
			}
			return false;
		}

		// end of class
	}

	public class SeqData {
		public R.Seq.eState state;
		public string[] cases;
		public string[] speak;
		public string condition;
		public string[] condiset;
		public string elseCase;
		public R.Seq.eState next;
		// ----------------------
		public bool isDone;        // make sure 
		public string matchSet;

		public SeqData(R.Seq val) {
			state = val.state;
			speak = val.speak.Split('|');
			cases = val.cases.Split('|');
			condition = val.condition;
			condiset = val.cond_set.Split('|');
			elseCase = val.elseCase;
			next = val.next;
		}
		public string SearchName {
			get {
				if (state == R.Seq.eState.Greet)
					return "";
				if (state == R.Seq.eState.AskDoForYou)
					return VoiceRecognizerHelper.SEARCH_COMMAND;
				if (state == R.Seq.eState.AskWhich)
					return "";
				if (state == R.Seq.eState.AskHowDensity)
					return VoiceRecognizerHelper.SEARCH_NUMBER_ZERO_TO_TEN;
				if (state == R.Seq.eState.AskConfirm)
					return VoiceRecognizerHelper.SEARCH_ANSWER;
				if (state == R.Seq.eState.Finish)
					return "";
				if (state == R.Seq.eState.Sleep)
					return VoiceRecognizerHelper.SEARCH_KWS;
				if (state == R.Seq.eState.AskCallup)
					return VoiceRecognizerHelper.SEARCH_ANSWER;
				if (state == R.Seq.eState.CallupEmergency)
					return "";
				return "";
			}
		}
		public void Reset() {
			isDone = false;
			matchSet = "";
		}

		// 단계 시작시 말을 할지 말지.
		public string GetSpeak() {
			if (speak.Length < 1)
				return "";

			int rnd = Random.Range(0, speak.Length - 1);
			return speak[rnd];
		}

		public bool CompareCodition(string text) {
			if (string.IsNullOrEmpty(text))
				return false;
			wndMain.szDebug = string.Format("Compare : {0}\n{1}\n", this.condition, text);

			string[] words = text.Split(' ');
			for(int i=0;i<this.condiset.Length;++i)
				wndMain.szDebug += string.Format("{0}:{1}\n",i, condiset[i]);

			switch (this.condition) {
				case "wait_for":
					foreach (var val in this.condiset) {
						if (text.Contains(val.Trim())) {
							matchSet = val.Trim();
							Logger.Debug("Match:" + matchSet);
							wndMain.szDebug += "-------------------\n<match> " + matchSet;
							return true;
						}
					}
					break;
				case "none":
					return true;
			}

			return false;
		}

		// 일치하는 경우를 찾자.
		public bool IsMatchCases(string matchString) {

			return cases.Contains<string>(matchString);
		}

		// end of class
	}


}