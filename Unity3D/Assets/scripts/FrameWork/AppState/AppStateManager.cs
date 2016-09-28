using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HC
{
	public enum eAppState
	{
		None = 0,
		Splash,
		Bootup,
		Patch,
		Logon,
		Main,
		Setting,
		Menu
	}

	public delegate void NotifyRestart();		// 리부팅 알림

    public class AppStateManager : SingletonMB<AppStateManager>
	{
		public eAppState _currentState = eAppState.None;
		public eAppState _previousState = eAppState.None;

		AppStateBase currGameState;

		public static NotifyRestart OnNotifyRestart;	// 리부팅 알림
		

		Dictionary<eAppState, AppStateBase> lstGameState = new Dictionary<eAppState,AppStateBase>();

		static public eAppState PreviousState
		{
			get { return instance._previousState; }
			private set {instance._previousState = value;}
		}
		static public eAppState CurrentState
		{
			get { return instance._currentState; }
			private set
			{
				Logger.InfoFormat(ColorType.magenta,"[GSM] change state ({0})",value);
				instance._previousState = instance._currentState;
				instance._currentState = value;
			}
		}

		// 인스턴스 생성하자.
		public static void CreateInstance() {
			instance.Initial();
		}

		public override void Initial()
		{
			base.Initial();
			Logger.InfoFormat("GSM Initialization !!");
		}

		protected override void Awake()
		{
			base.Awake();
			Logger.InfoFormat("Initialization !!");
			// table resource create
			R.Create();

			// windowManager 초기화를 여기서 먼저 해주자.
			MessageBoxManager.instance.Initial();
			WindowManager.CreateInstance();

			SceneLoader.CreateInstance();
			//PushManager.CreateInstance();

			// 개인 정보 생성
			PlayerSaveInfo.CreateInstance();
			PlayerSaveInfo.instance.Load();

			// seq manager
			SeqManager.instance.Initial();



			AddGameState(eAppState.None, this.gameObject.AddComponent<AppStateNone>());
			AddGameState(eAppState.Splash, this.gameObject.AddComponent<AppStateSplash>());
			AddGameState(eAppState.Bootup, this.gameObject.AddComponent<AppStateBootup>());
            AddGameState(eAppState.Main, this.gameObject.AddComponent<AppStateMain>());
			AddGameState(eAppState.Menu, this.gameObject.AddComponent<AppStateMenu>());
		}

		private void Initialization()
		{
		}

		private void Reset()
		{

			PlayerSaveInfo.DestroyInstance();
		}

		// 해당 상태를 최초 상태로 초기화 하는곳
		static public void Restart() {

			Logger.Warning("<<<<<<<<<<< App Restart !! >>>>>>>>>>>");

			if (IsInstanced)
				instance.Reset();

			// 리스타트 되었다.
			if (null != OnNotifyRestart)
				OnNotifyRestart();

			// 초기상태에서 다시 리셋 하라고 하면 안되지
			if (CurrentState == eAppState.None) {
				Logger.Warning("Duplicate Restart !!");
				return;
			}

			if (IsInstanced) {
				foreach (var state in instance.lstGameState.Values) {
					if (null != state)
						state.Reset();
				}

				instance.Initialization();

				// 동작중인 State가 있으면 나와
				ChangeState(eAppState.None);
			}

			// 앱 재시작을 요청하면 앱 버전 체크부터 다시 해야한다.
			Main.instance.OnStartup();
		}

		// 현재 스테이트 준비 완료
		static public bool IsPrepareStateDone
		{
			get {
				if (null == instance.currGameState)
					return false;
				return instance.currGameState.IsPrearedState;
			}
			set {
				if (null == instance.currGameState)
					return;
				Logger.Info(ColorType.magenta, "PrepareStateDone : " + CurrentState + " : " + value);
				instance.currGameState.IsPrearedState = value;
			}
		}

		// Scene 내부의 윈도우가 초기화 되었을때 여기를 호출해서 완료된것을 알려주자
		static public void OnLoadedWindow(WindowBase wndBase)
		{
			if (null == instance.currGameState)
				return;
			instance.currGameState.OnLoadedWindow(wndBase);
		}

		// 상태 변경
		static public void ChangeState(eAppState _eGameState)
		{
			// state check
			if (CurrentState == _eGameState) {
				Logger.Warning("duplicate state : " + _eGameState);
				return;
			}

			AppStateBase prevState;
			if (instance.lstGameState.TryGetValue(CurrentState, out prevState)) {
				if (null != prevState) {
					prevState.OnClearAll();
					prevState.OnLeave();
				}
			}

			CurrentState = _eGameState;

            if (instance.lstGameState.TryGetValue(_eGameState, out instance.currGameState)) {
                if (null != instance.currGameState) {
                    instance.currGameState.OnPrepare();
                    instance.currGameState.OnEnter();
				}
			}

		}

		void Update()
		{
			if (null != currGameState)
				currGameState.OnUpdate();
		}

		static public T Get<T>() where T : AppStateBase
		{
			var result = instance.lstGameState.Values.Where(p => (null != p && p.GetType() == typeof(T)));
			if (result.Count() < 1)
				return default(T);
			return (T)result.First();
		}

		void AddGameState(eAppState estate, AppStateBase cmpStateBase)
		{
			cmpStateBase.transform.parent = gameObject.transform;
			lstGameState.Add(estate, cmpStateBase);
		}
	}

}