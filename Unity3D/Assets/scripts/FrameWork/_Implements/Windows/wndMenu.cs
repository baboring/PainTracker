using UnityEngine;
using System.Collections;

namespace HC
{
    public class wndMenu : WindowBase
    {
        public GameObject objBtnClose;
		public GameObject objBtnSetting;
		public GameObject objBtnExtra;
		public GameObject objBtnHelp;

		// Use this for initialization
		protected override void Awake()
        {
            eWindowID = WndID.WndMenu;

			base.Awake();

			// go to main scene
            ButtonHandler.CreateHandle(0, objBtnClose, true, true, (btn) => {
				MainStateManager.ChangeState(eMainState.Main);
			});

			// open Setting window
			ButtonHandler.CreateHandle(0, objBtnSetting, true, true, (btn) => {
				WindowManager.OpenPopup(WndID.WndSetting);
			});
			// Help button
			ButtonHandler.CreateHandle(0, objBtnHelp, true, true, (btn) => {
				
			});
		}

		// override
		protected override void OnOpenWindow()
        {
            base.OnOpenWindow();
			// hide menu button
			WindowManager.GetWindow<wndMain>().objMenuButton.SetActive(false);
		}
		protected override void OnCloseWindow()
		{
			base.OnCloseWindow();
			// show menu button
			WindowManager.GetWindow<wndMain>().objMenuButton.SetActive(true);
		}
	}
}