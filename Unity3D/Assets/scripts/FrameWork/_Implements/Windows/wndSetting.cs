using UnityEngine;
using System.Collections;

namespace HC
{
    public class wndSetting : WindowBase
    {
        public GameObject objExitButton;

        // Use this for initialization
        protected override void Awake()
        {
            eWindowID = WndID.WndSetting;
			eWindowType = WndType.Popup;

			base.Awake();

            ButtonHandler.CreateHandle(0, objExitButton, true, true, (btn) => {
				CloseSelfWindow();
            });
        }

        protected override void OnOpenWindow()
        {
            base.OnOpenWindow();

            // 순차적 진행
        }

		public void OnSelectEngine()
		{
			Debug.Log(UIPopupList.current.value);
			//engineName = nameArray[selected];
			//EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates, UIPopupList.current.value);
		}

	}
}