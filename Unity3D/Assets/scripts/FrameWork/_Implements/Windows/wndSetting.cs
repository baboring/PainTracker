using UnityEngine;
using System.Collections;

namespace HC
{
    public class wndSetting : WindowBase
    {
		public UIInput uiName;
		public UIInput uiPhone;

		public GameObject obnBtnSubmit;
		public GameObject obnBtnCancel;

		// Use this for initialization
		protected override void Awake()
        {
            eWindowID = WndID.WndSetting;
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
				CloseSelfWindow();
            });
        }

        protected override void OnOpenWindow()
        {
            base.OnOpenWindow();

			uiName.value = PlayerSaveInfo.instance.userName_;
			uiPhone.value = PlayerSaveInfo.instance.userPhone_;

		}

		public void OnSelectEngine()
		{
			Debug.Log(UIPopupList.current.value);
			//engineName = nameArray[selected];
			//EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates, UIPopupList.current.value);
		}

	}
}