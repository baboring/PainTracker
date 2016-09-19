using UnityEngine;
using System.Collections;

namespace HC
{
    public class wndRegister : WindowBase
    {
		public UIInput uiName;
		public UIInput uiPhone;

		public GameObject obnBtnSubmit;
		public GameObject obnBtnCancel;

		// Use this for initialization
		protected override void Awake()
        {
            eWindowID = WndID.WndRegister;
            eWindowType = WndType.Popup;

            base.Awake();
			uiName.value = PlayerSaveInfo.instance.userName_;
			uiPhone.value = PlayerSaveInfo.instance.userPhone_;

			ButtonHandler.CreateHandle(0, obnBtnSubmit, true, true, (btn) => {
				PlayerSaveInfo.instance.userName_ = uiName.value;
				PlayerSaveInfo.instance.userPhone_ = uiPhone.value;
				PlayerSaveInfo.instance.Save();
				CloseSelfWindow();
			});
			ButtonHandler.CreateHandle(0, obnBtnCancel, true, true, (btn) => {
				if(string.IsNullOrEmpty(PlayerSaveInfo.instance.userName_)) {
					MessageBoxManager.Show("Register","You should input your name!");
					return;
				}
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