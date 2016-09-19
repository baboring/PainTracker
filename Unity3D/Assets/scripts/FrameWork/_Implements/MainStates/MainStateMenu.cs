using UnityEngine;

using System;

namespace HC
{
    public class MainStateMenu : MainStateBase
    {
        static MainStateMenu _instance;
        static public MainStateMenu instance { get { return _instance; } }
		MainStateMenu()
        {
            Logger.Assert(_instance == null);
            _instance = this;
        }

        void Awake()
        {
            Logger.InfoFormat("MainStateMenu.Awake !!");
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
				WindowManager.SetActiveWindow(WndID.WndMenu, true);
			};
            complete(true);

        }
        override public void OnLeave() {
			WindowManager.SetActiveWindow(WndID.WndMenu, false);
		}
        override public void OnUpdate() { }



	}

}