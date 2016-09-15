/********************************************************************
	created:	2014/04/07
	filename:	PlayerSaveInfo.cs
	author:		Benjamin
	purpose:	[]
*********************************************************************/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HC
{
    // 계정 저장 정보
    public class PlayerSaveInfo : ManualSingletonMB<PlayerSaveInfo>
    {
        ///////////////////////////////////////////////////////////

        const string key_FirstAppRun = "FirstAppRun";
        const string key_SaveAccount = "SaveAccount";
        const string key_AutoLogin = "AutoLogin";
        const string key_Agreements = "Agreements";
        const string key_NextOpenDays = "NextOpenDays";
        const string key_Mail = "Mail";
        const string key_Pwd = "Pwd";
        const string key_Nick = "Name";
        const string keySoundBGM = "SoundBGM";
        const string keySoundEffect = "SoundEffect";
        const string key_ShuffleCard = "ShuffleCard";
        const string key_AutoPlay = "AutoPlay";
        const string key_Switch = "Switch";
        const string key_Repeat = "Repeat";
        const string key_WaitTime = "WaitTime";

        const string key_GuestAccount = "GuestAccount"; // 게스트 접속용 계정키
        const string PUSH_FIRST_REG = "GCMFirstReg";    // 푸시 등록
        const string PUSH_REG_ID = "GCMRegID";      // 푸시 등록 ID
        const string key_LastOpenBgID = "LastOpenBgID"; //마지막 저장된 로비 백그라운드
        const string key_LastDay = "LastDay";       //마지막 접속 날짜
        const string key_AutoAccrueSK = "AutoAccrueSK"; //누적스킬리스트 자동켜기
                                                        ///////////////////////////////////////////////////////////
        public string guest_Account_ = string.Empty;    // Guest Account

        public string userId = string.Empty;        // Email 계정
        public string pwd_ = string.Empty;      // password
        public string name_ = string.Empty;     // 이름(NickName)

        public bool IsSaveAccount = true;       // 계정 저장 On/Off
        public bool IsAutoLogin = true;         // 자동 로그인
        public bool IsAgreements = false;       // 이용약관 동의

        int tutorialCompleteFlag = 0;   //현재 상태

        public bool IsNotUseProfile { get; set; }   // 개인 정보 사용 안함 ( 기본값 사용 ) - 봇으로 테스트 할때 부하요소 제거용

        // 싱글톤 인스턴스 생성
        public static void CreateInstance()
        {
            if (instance == null)
            {
                instance = new GameObject("HC.PlayerSaveInfo").AddComponent<PlayerSaveInfo>();
                DontDestroyOnLoad(instance.gameObject);
            }
            //instance.Load();
        }

        public static void DestroyInstance()
        {
            if (null != instance)
                DestroyImmediate(instance.gameObject);
        }

        void Awake()
        {
        }

        // 불어오기
        public bool Load()
        {
            if (IsNotUseProfile)    // bot은 읽지 하지 않는다.
                return true;

            // 게스트 계정
            guest_Account_ = EncryptedPlayerPrefs.GetString(key_GuestAccount);

            // 옵션
            IsSaveAccount = (EncryptedPlayerPrefs.GetInt(key_SaveAccount, 1) > 0);
            IsAutoLogin = (EncryptedPlayerPrefs.GetInt(key_AutoLogin, 1) > 0);
            IsAgreements = (EncryptedPlayerPrefs.GetInt(key_Agreements, 0) > 0);

            // 옵션에 따른 로딩
            if (IsSaveAccount)
            {
                userId = EncryptedPlayerPrefs.GetString(key_Mail);
                pwd_ = EncryptedPlayerPrefs.GetString(key_Pwd);
            }
            name_ = EncryptedPlayerPrefs.GetString(key_Nick);

            Logger.InfoFormat("Load PlayerSaveInfo : {0}", userId);

            return true;
        }

        // 저장
        public bool Save()
        {
            if (IsNotUseProfile)    // bot은 저장 하지 않는다.
                return true;

            Logger.DebugFormat("Save PlayerSaveInfo : {0}", userId);

            if (userId.Length < 1)
            {
                Logger.Warning("userID length is 0");
                //return false;
            }

            // 게스트 계정
            EncryptedPlayerPrefs.SetString(key_GuestAccount, guest_Account_);

            // 옵션 저장
            EncryptedPlayerPrefs.SetInt(key_SaveAccount, (IsSaveAccount) ? 1 : 0);
            EncryptedPlayerPrefs.SetInt(key_AutoLogin, (IsAutoLogin) ? 1 : 0);
            EncryptedPlayerPrefs.SetInt(key_Agreements, (IsAgreements) ? 1 : 0);

            // 옵션에 따른 저장
            EncryptedPlayerPrefs.SetString(key_Mail, (IsSaveAccount) ? userId : "");
            EncryptedPlayerPrefs.SetString(key_Pwd, (IsSaveAccount) ? pwd_ : "");
            EncryptedPlayerPrefs.SetString(key_Nick, name_);

            return true;
        }

        // 게스트 계정을 사용 하는지
        public bool IsExistGusetAccount
        {
            get { return !string.IsNullOrEmpty(guest_Account_); }
        }

        // Bgm 저장
        public bool IsEnableSoundBgm()
        {
            if (IsNotUseProfile)    // bot은 하지 않는다.
                return false;
            return (EncryptedPlayerPrefs.GetInt(keySoundBGM, 1) != 0);
        }

        public void SaveEnableSoundBgm(bool bEnable)
        {
            if (IsNotUseProfile)    // bot은 하지 않는다.
                return;
            EncryptedPlayerPrefs.SetInt(keySoundBGM, (bEnable) ? 1 : 0);
        }

        // Effect 저장
        public bool IsEnableSoundEffect()
        {
            if (IsNotUseProfile)    // bot은 하지 않는다.
                return false;
            return (EncryptedPlayerPrefs.GetInt(keySoundEffect, 1) != 0);
        }

        public void SaveEnableSoundEffect(bool bEnable)
        {
            if (IsNotUseProfile)    // bot은 하지 않는다.
                return;
            EncryptedPlayerPrefs.SetInt(keySoundEffect, (bEnable) ? 1 : 0);
        }

        // Option / Shuffle Card
        public bool IsShuffleCard
        {
            get
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return false;
                return (EncryptedPlayerPrefs.GetInt(key_ShuffleCard, 0) != 0);
            }

            set
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return;
                EncryptedPlayerPrefs.SetInt(key_ShuffleCard, (value) ? 1 : 0);
            }
        }
        // Option / AutoPlay
        public bool IsAutoPlay
        {
            get
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return false;
                return (EncryptedPlayerPrefs.GetInt(key_AutoPlay, 0) != 0);
            }

            set
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return;
                EncryptedPlayerPrefs.SetInt(key_AutoPlay, (value) ? 1 : 0);
            }
        }
        // Option / IsSwitchUpDown
        public bool IsSwitchUpDown
        {
            get
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return false;
                return (EncryptedPlayerPrefs.GetInt(key_Switch, 0) != 0);
            }

            set
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return;
                EncryptedPlayerPrefs.SetInt(key_Switch, (value) ? 1 : 0);
            }
        }
        // Option / Repeat
        public bool IsRepeat
        {
            get
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return false;
                return (EncryptedPlayerPrefs.GetInt(key_Repeat, 0) != 0);
            }

            set
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return;
                EncryptedPlayerPrefs.SetInt(key_Repeat, (value) ? 1 : 0);
            }
        }

        // 
        public float WaitTime
        {
            get
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return 0;
                return EncryptedPlayerPrefs.GetFloat(key_WaitTime, 6);
            }

            set
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return;
                EncryptedPlayerPrefs.SetFloat(key_WaitTime, value);
            }
        }

        // 회원 가입된 상태인지 아닌지 구분..
        public bool IsRegisterd
        {
            get
            {
                if (userId.Trim().Length < 1 || IsAgreements == false)
                    return false;
                // 비번 검색은 안함..
                //if (pwd.Trim().Length < 1)
                //    return false;
                return true;
            }
        }

        // 클리어
        public void Clear()
        {
            // 게스트 account는 삭제 하지 않는다.( 어플 삭제 하면 삭제 되도록 )
            IsSaveAccount = true;
            IsAutoLogin = true;
            IsAgreements = false;

            userId = string.Empty;
            pwd_ = string.Empty;
            name_ = string.Empty;

            Save();
        }

        // Push 등록 상태 
        static public bool IsPushReg
        {
            get
            {
                if (instance.IsNotUseProfile)   // bot은 하지 않는다.
                    return true;

                return (EncryptedPlayerPrefs.GetInt(PUSH_FIRST_REG, 0) == 1);
            }
        }

        static public void SavePushReg(string deviceToken)
        {
            if (instance.IsNotUseProfile)   // bot은 하지 않는다.
                return;
            EncryptedPlayerPrefs.SetString(PUSH_REG_ID, deviceToken);
            EncryptedPlayerPrefs.SetInt(PUSH_FIRST_REG, 1);
        }

        // 등록된 Push RegID
        static public string GetPushRegID()
        {
            if (instance.IsNotUseProfile)   // bot은 하지 않는다.
                return "==ok==";

            return EncryptedPlayerPrefs.GetString(PUSH_REG_ID);
        }

        // 앱 최초 실행 처리
        static public bool IsAppFirstRun
        {
            get
            {
                if (instance.IsNotUseProfile)   // bot은 하지 않는다.
                    return false;
                return (EncryptedPlayerPrefs.GetInt(key_FirstAppRun, 0) == 1);
            }
        }

        static public void SaveAppFirstRun()
        {
            if (instance.IsNotUseProfile)   // bot은 하지 않는다.
                return;
            EncryptedPlayerPrefs.SetInt(key_FirstAppRun, 1);
        }

        //마지막 오픈된 백그라운드
        public int Shared_LastOpenBgID
        {
            get
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return 5;

                //현재 5번이 첫번째맵이라.. Defualt값으로 설정함
                return EncryptedPlayerPrefs.GetInt(key_LastOpenBgID, 5);
            }
            set
            {
                if (IsNotUseProfile)    // bot은 하지 않는다.
                    return;
                EncryptedPlayerPrefs.SetInt(key_LastOpenBgID, value);
            }
        }

        //마지막에 게임한 날짜 저장하고 불러옴
        static public void SaveLastDay()
        {
            if (instance.IsNotUseProfile)   // bot은 하지 않는다.
                return;

            string today = string.Format("{0}{1:0#}{2:0#}", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

            EncryptedPlayerPrefs.SetString(instance.userId + key_LastDay, today);
        }

        static public string LoadLastDay()
        {
            if (instance.IsNotUseProfile)   // bot은 하지 않는다.
                return "20141117";

            return EncryptedPlayerPrefs.GetString(instance.userId + key_LastDay, "20141117");
        }

        //스킬목록 자동보기 저장 및 로드
        public void SaveAutoAccrueSkill(bool enable)
        {
            if (IsNotUseProfile)    // bot은 하지 않는다.
                return;

            EncryptedPlayerPrefs.SetInt(key_AutoAccrueSK, (enable) ? 1 : 0);
        }

        public bool LoadAutoAccrueSkill()
        {
            if (IsNotUseProfile)    // bot은 하지 않는다.
                return false;

            if (EncryptedPlayerPrefs.GetInt(key_AutoAccrueSK, 0) > 0)
                return true;
            return false;
        }
    }//class
}