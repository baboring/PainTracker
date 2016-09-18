using UnityEngine;
using System.Collections;

namespace HC
{
    public class wndRegister : WindowBase
    {
        public GameObject obnBtnSubmit;

		// Use this for initialization
		protected override void Awake()
        {
            eWindowID = WndID.WndRegister;
            eWindowType = WndType.Popup;

            base.Awake();

            ButtonHandler.CreateHandle(0, obnBtnSubmit, true, true, (btn) =>
            {
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