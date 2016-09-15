using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HC
{
    //---------------------------------------------------------------------------------
    // 메시지박스 타입(모달)
    public enum eMessageBox
    {
        MB_OK,
        //MB_OKCANCEL,
        //MB_ABORTRETRYIGNORE,
        //MB_YESNOCANCEL,
        MB_YESNO,
        //MB_RETRYCANCEL,
        //MB_HELP,
        MB_CUSTOM_3BTN, //ID_YES , ID_NO , ID_CUSTOM_1
        MAX
    }

    public enum eMessageButton
    {
        None = -1,
        //ID_ERROR,
        ID_YES,
        ID_OK,
        ID_NO,
        //ID_CANCEL,
        ID_CUSTOM_1,
        MaxCount
    }

    public enum eMessageButtonPosition
    {
        LEFT = -1,
        CENTER,
        RIGHT
    }

    public class MessageBoxInfo
    {
        public string szTitle;
        public string szDescription;
        public eMessageBox eMsgBoxType = eMessageBox.MB_OK;

        public MessageBoxInfo(string title, string desc, eMessageBox eType)
        {
            szTitle = title;
            szDescription = desc;
            eMsgBoxType = eType;
        }
        public MessageBoxInfo(string title, string desc)
        {
            szTitle = title;
            szDescription = desc;
        }
    }

    public class CustomMessageBoxInfo : MessageBoxInfo
    {
        public string okBtnText = "";
        public string cancelBtnText = "";
        public string customBtnText = "";

        public eMessageButtonPosition okBtnPos = eMessageButtonPosition.RIGHT;
        public eMessageButtonPosition cancelBtnPos = eMessageButtonPosition.CENTER;
        public eMessageButtonPosition customBtnPos = eMessageButtonPosition.LEFT;

        public CustomMessageBoxInfo(string title, string desc, eMessageBox eType) : base(title, desc, eType)
        {
        }
    }

    public class MessageBoxManager : SingletonMB<MessageBoxManager>
    {
        // 종료 시점 확인
        bool IsApplicationQuit;
        GameObject prefabMessageBox = null;
        GameObject objRoot = null;

        List<MessageBoxWindow> StackMessageBoxs = new List<MessageBoxWindow>();

        static void Prepare()
        {
            instance.Initial();
        }

        public override void Initial()
        {
            // UIRoot 생성
            if (objRoot == null)
            {
                objRoot = this.gameObject;

                UIRoot uiRoot = objRoot.AddComponent<UIRoot>();
                uiRoot.scalingStyle = UIRoot.Scaling.ConstrainedOnMobiles;
            }
            //objRoot = GameObject.Find("Root");
            if (prefabMessageBox == null)
                prefabMessageBox = Resources.Load("UI/Prefab/MessageBox") as GameObject;
        }
        //----------------------------------------------------------------------------------------
        public MessageBoxWindow GetTop()
        {
            if (StackMessageBoxs.Count < 1)
                return null;
            return StackMessageBoxs[StackMessageBoxs.Count - 1];
        }

        // Active count
        static public int ActiveCount
        {
            get
            {
                int cnt = 0;

                foreach (var msgBox in instance.StackMessageBoxs)
                {
                    if (msgBox.IsActived)
                        ++cnt;
                }
                return cnt;
            }
        }

        // 최상위 박스를 취소 처리 한다.
        public static bool CloseMessageBox(WndCloseType eType = WndCloseType.Cancel)
        {
            var msgBox = MessageBoxManager.instance.GetTop();

            if (null == msgBox || !msgBox.IsActived)
                return false;
            if (eType == WndCloseType.Ok)
                msgBox.Ok();
            if (eType == WndCloseType.Cancel)
                msgBox.Cancel();
            else
                msgBox.Cancel();
            return true;
        }

        // 열려 있는 모든 메시지 창 싹제..
        public static void CloseAll(WndCloseType eType = WndCloseType.None)
        {
            if (!IsInstanced)
                return;

            var tmpList = instance.StackMessageBoxs.ToArray();

            foreach (var msgBox in tmpList)
            {
                if (eType == WndCloseType.Ok)
                    msgBox.Ok();
                if (eType == WndCloseType.Cancel)
                    msgBox.Cancel();
                else
                    msgBox.Cancel();

                // 바로 삭제 해버려야 겠다.
                if (Remove(msgBox))
                    DestroyImmediate(msgBox.gameObject);
            }
        }

        // 열려 있는 모든 메시지 창 싹제..
        public static void DestroyAll()
        {
            if (!IsInstanced)
                return;
            for (int i = 0; i < instance.StackMessageBoxs.Count; ++i)
                DestroyImmediate(instance.StackMessageBoxs[i].gameObject);

            instance.StackMessageBoxs.Clear();
            ButtonHandler._LockedClickCount = 0;
        }

        // 강제로 닫아 부러
        public static bool Remove(MessageBoxWindow msgBox)
        {
            return instance._Remove(msgBox);
        }

        // 닫기 처리
        bool _Remove(MessageBoxWindow msgBox)
        {
            if (null == msgBox)
                return false;

            var findIdx = StackMessageBoxs.FindIndex(va => va == msgBox);

            if (findIdx > -1)
            {
                Logger.DebugFormat(ColorType.cyan, "close MsgBox : {0}", findIdx);
                msgBox.InvokeClose();
                StackMessageBoxs.RemoveAt(findIdx);
                return true;
            }
            return false;
        }
        //----------------------------------------------------------------------------------------
        // 메시지박스 생성 
        public static MessageBoxWindow Show(string strTitle, string strDesc, eMessageBox msgBoxType = eMessageBox.MB_OK)
        {
            return Show(new MessageBoxInfo(strTitle, strDesc, msgBoxType));
        }

        // 메시지박스 생성 
        public static MessageBoxWindow Show(MessageBoxInfo msgInfo)
        {
            MessageBoxManager.Prepare();
            return instance._Show(msgInfo.szTitle, msgInfo.szDescription, msgInfo.eMsgBoxType);
        }
        // CUSTOM 버튼 메시지박스 생성 
        public static MessageBoxWindow Show(CustomMessageBoxInfo msgInfo)
        {
            MessageBoxManager.Prepare();
            return instance._Show(msgInfo);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        MessageBoxWindow _Show(string strTitle, string strDesc, eMessageBox msgBoxType)
        {
            GameObject messageBoxObject = null;

            Logger.DebugFormat(ColorType.cyan, "ShowMsg : {0}/{1}", strTitle, strDesc);

            messageBoxObject = GameObject.Instantiate(prefabMessageBox) as GameObject;

            // group root 
            if (messageBoxObject == null)
                return null;
            else {
                messageBoxObject.transform.parent = objRoot.transform;
                messageBoxObject.transform.localScale = new Vector3(1, 1, 1);
                messageBoxObject.transform.localPosition = new Vector3(0, 0, (SystemConfig.DEPTH_BASE_MESSAGE_BOX + (float)(StackMessageBoxs.Count) * -5.0f));
            }

            MessageBoxWindow dlgMessageBox = messageBoxObject.GetComponent<MessageBoxWindow>();

            if (dlgMessageBox == null)
            {
                GameObject.DestroyObject(messageBoxObject);
                return null;
            }

            // Stack에 추가
            StackMessageBoxs.Add(dlgMessageBox);

            //-------------------------------------------------------------------
            // 타이틀 설정
            if (dlgMessageBox.objectTitle != null)
            {
                UILabel labelTitle = dlgMessageBox.objectTitle.GetComponent<UILabel>();

                if (labelTitle != null)
                {
                    if (string.IsNullOrEmpty(strTitle))
                    {
                        //labelTitle.text = StringTable.GetString(StringTable.SYSTEM_STRING.Notice);
                        labelTitle.text = "Notice";
                        //// Title 내용이 없으면 메시지 내용을 위로 올려서 출력해야지.
                        //Vector3 vPos = dlgMessageBox.objectDesc.transform.localPosition;
                        //vPos.y += 25;
                        //dlgMessageBox.objectDesc.transform.localPosition = vPos;
                    }
                    else {
                        //labelTitle.text = strTitle + "(" + StackMessageBoxs.Count + ")";
                        labelTitle.text = strTitle;
                    }
                }
                messageBoxObject.name = "MessageBox (" + StackMessageBoxs.Count + ")";
            }

            // 내용 설정
            if (dlgMessageBox.objectDesc != null)
            {
                UILabel labelDesc = dlgMessageBox.objectDesc.GetComponent<UILabel>();

                if (labelDesc != null)
                    labelDesc.text = strDesc;
            }

            // 스프라이트 설정
            switch (msgBoxType)
            {
                case eMessageBox.MB_YESNO:
                case eMessageBox.MB_CUSTOM_3BTN:        //ID_YES , ID_NO , ID_CUSTOM_1
                    {
                        dlgMessageBox.objectOkLabel.GetComponent<UILabel>().text = R.GetSystemMsg(R.SystemMsg.eKey.Yes).StrValue;   // 예
                        ButtonHandler.CreateHandle(0, dlgMessageBox.objectOkButton.gameObject, true, true, dlgMessageBox.handleButtonClick)
                            .eButton = eMessageButton.ID_YES;
                    }
                    {
                        dlgMessageBox.objectCancelLabel.GetComponent<UILabel>().text = R.GetSystemMsg(R.SystemMsg.eKey.No).StrValue;    // 아니오
                        ButtonHandler.CreateHandle(0, dlgMessageBox.objectCancelButton.gameObject, true, true, dlgMessageBox.handleButtonClick)
                            .eButton = eMessageButton.ID_NO;
                    }
                    break;
                case eMessageBox.MB_OK:
                    {
                        dlgMessageBox.objectOkLabel.GetComponent<UILabel>().text = R.GetSystemMsg(R.SystemMsg.eKey.Confirmation).StrValue;  // 확인

                        Vector3 vPos = dlgMessageBox.objectOkButton.transform.position;
                        vPos.x = 0;
                        dlgMessageBox.objectOkButton.transform.position = vPos;
                        ButtonHandler.CreateHandle(0, dlgMessageBox.objectOkButton.gameObject, true, true, dlgMessageBox.handleButtonClick)
                            .eButton = eMessageButton.ID_OK;
                    }
                    dlgMessageBox.objectCancelButton.gameObject.SetActive(false);

                    break;
            }

            messageBoxObject.SetActive(true);

            return dlgMessageBox;
        }

        MessageBoxWindow _Show(CustomMessageBoxInfo customBoxInfo)
        {
            const float RELATIVE_POSX = 220f;
            //기본내용 설정 후에
            MessageBoxWindow dlgMessageBox = _Show(customBoxInfo.szTitle, customBoxInfo.szDescription, customBoxInfo.eMsgBoxType);

            //커스텀 내용 설정하자
            {
                //바꾸고자 하는 버튼 텍스트가 있으면 변경
                if (string.IsNullOrEmpty(customBoxInfo.okBtnText) == false)
                    dlgMessageBox.objectOkLabel.GetComponent<UILabel>().text = customBoxInfo.okBtnText;

                //버튼 위치 변경
                Vector3 vPos = dlgMessageBox.objectOkButton.transform.localPosition;
                vPos.x = (int)customBoxInfo.okBtnPos * RELATIVE_POSX;
                dlgMessageBox.objectOkButton.transform.localPosition = vPos;
            }
            {
                if (string.IsNullOrEmpty(customBoxInfo.cancelBtnText) == false)
                    dlgMessageBox.objectCancelLabel.GetComponent<UILabel>().text = customBoxInfo.cancelBtnText;

                Vector3 vPos = dlgMessageBox.objectCancelButton.transform.localPosition;
                vPos.x = (int)customBoxInfo.cancelBtnPos * RELATIVE_POSX;
                dlgMessageBox.objectCancelButton.transform.localPosition = vPos;
            }
            {
                dlgMessageBox.objectCustomLabel.GetComponent<UILabel>().text = customBoxInfo.customBtnText;
                ButtonHandler.CreateHandle(0, dlgMessageBox.objectCustomButton.gameObject, true, true, dlgMessageBox.handleButtonClick)
                    .eButton = eMessageButton.ID_CUSTOM_1;

                Vector3 vPos = dlgMessageBox.objectCustomButton.transform.localPosition;
                vPos.x = (int)customBoxInfo.customBtnPos * RELATIVE_POSX;
                dlgMessageBox.objectCustomButton.transform.localPosition = vPos;

                dlgMessageBox.objectCustomButton.gameObject.SetActive(true);        //커스텀 버튼은 인스펙터에서 off 이니 on
            }

            return dlgMessageBox;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Splash Message
        public static MessageBoxWindow Splash(string strDesc, float WaitTime)
        {
            MessageBoxManager.Prepare();
            return MessageBoxManager.instance._Splash(strDesc, WaitTime);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        MessageBoxWindow _Splash(string strDesc, float WaitTime)
        {
            GameObject messageBoxObject = null;

            messageBoxObject = GameObject.Instantiate(prefabMessageBox) as GameObject;

            // group root 
            if (messageBoxObject == null)
                return null;
            else {
                messageBoxObject.transform.parent = objRoot.transform;
                messageBoxObject.transform.localScale = new Vector3(1, 1, 1);
                messageBoxObject.transform.localPosition = new Vector3(0, 0, (SystemConfig.DEPTH_BASE_MESSAGE_BOX - (float)(StackMessageBoxs.Count) * 5.0f));
            }

            MessageBoxWindow dlgMessageBox = messageBoxObject.GetComponent<MessageBoxWindow>();

            if (dlgMessageBox == null)
            {
                GameObject.DestroyObject(messageBoxObject);
                return null;
            }

            // Stack에 추가
            StackMessageBoxs.Add(dlgMessageBox);

            //-------------------------------------------------------------------
            // 타이틀 설정
            if (dlgMessageBox.objectTitle != null)
            {
                UILabel labelTitle = dlgMessageBox.objectTitle.GetComponent<UILabel>();

                if (labelTitle != null)
                    labelTitle.text = R.GetSystemMsg(R.SystemMsg.eKey.TitleNotice).StrValue;    // 알림
                messageBoxObject.name = "SplashBox (" + StackMessageBoxs.Count + ")";
            }

            // 내용 설정
            if (dlgMessageBox.objectDesc != null)
            {
                UILabel labelDesc = dlgMessageBox.objectDesc.GetComponent<UILabel>();

                if (labelDesc != null)
                    labelDesc.text = strDesc;
            }

            dlgMessageBox.objectOkButton.gameObject.SetActive(false);
            dlgMessageBox.objectCancelButton.gameObject.SetActive(false);
            // 대기후 종료할 시간을 주자
            dlgMessageBox.SetSelfCloseTime(WaitTime);

            messageBoxObject.SetActive(true);

            return dlgMessageBox;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public static int GetActivedMessageBoxCnt()
        {
            if (MessageBoxManager.instance.StackMessageBoxs != null)
                return MessageBoxManager.instance.StackMessageBoxs.Count;
            return 0;
        }

        void OnApplicationQuit()
        {
            IsApplicationQuit = true;
        }
    }
}