using UnityEngine;

using System;


namespace HC
{
	public class MainStateBootup : MainStateBase
	{
		static MainStateBootup _instance;
		static public MainStateBootup instance { get { return _instance; } }
		MainStateBootup() 
		{
			MainStateBootup._instance = this;
		}

		void Awake()
		{
			Logger.InfoFormat("GameStateIntro.Awake !!");
		}

		// 해당 상태를 최초 상태로 초기화 하는곳
		override public void Reset()
		{
			
		}

		override public void OnEnter()
		{

			// 웹뷰 이미 있었으면 지워야지
			//if (WebViewManager.IsInstanced)
			//	WebViewManager.SelfDestroy();

			// MessageBox 삭제
			if (MessageBoxManager.IsInstanced)
				MessageBoxManager.SelfDestroy();

			//GlobalData.Instance.Initialize();

			OnLoadComplete();
		}

		override public void OnLeave()
		{

		}

		override public void OnUpdate()
		{

		}

		override public void OnLoadComplete()
		{
			IsPrearedState = true;

			// Logon 상태로 이동
			if (MainStateManager.CurrentState != eMainState.Patch)
				MainStateManager.ChangeState(eMainState.Patch);

		}
	}

}