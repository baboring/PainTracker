using UnityEngine;

using System;


namespace HC
{
	public class MainStateSplash : MainStateBase
	{
		static MainStateSplash _instance;
		static public MainStateSplash instance { get { return _instance; } }
		MainStateSplash() 
		{
            Logger.Assert(_instance == null);
			_instance = this;
		}

		// 해당 상태를 최초 상태로 초기화 하는곳
		override public void Reset()
		{

		}

		override public void OnEnter()
		{
			// 씬로드에서 안해주니 여기서 완료시 
			Action<bool> complete = (success)=>OnLoadComplete();
			complete(true);
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

            // when splash is finished, call next state
            //SplashScreen.instance.OnFinish = () => GameStateManager.ChangeState(eMainState.Bootup);
		}
	}

}