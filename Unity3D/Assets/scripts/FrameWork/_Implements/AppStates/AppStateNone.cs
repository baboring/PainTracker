using UnityEngine;

using System;


namespace HC
{
	public class AppStateNone : AppStateBase
	{
		static AppStateNone _instance;
		static public AppStateNone instance { get { return _instance; } }
		AppStateNone()
		{
            Logger.Assert(_instance == null);
			_instance = this;
		}

		void Awake()
		{
			Logger.InfoFormat("MainStateNone.Awake !!");

		}

		// 해당 상태를 최초 상태로 초기화 하는곳
		override public void Reset()
		{

		}

		override public void OnEnter()
		{
			// Window Load가 없으니 바로 완료
			Action<bool> complete = (success) => {
				IsPrearedState = true;
			};
			complete(true);
		}

		override public void OnLeave() { }

		override public void OnUpdate() { }

	}

}