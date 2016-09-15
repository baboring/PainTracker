//@yarny 우선 inGame만 처리됨
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HC
{
	public enum eBGM
	{
		None,
		MAX
	}
	public class BGM : ManualSingletonMB<BGM>
	{
		class BGMInfo
		{
			public int index;
			public float defaultVolume;
			public GameObject gameObj;
			public AudioSource audioSource;

		};

		BGMInfo[] bgmList = new BGMInfo[(int)eBGM.MAX];
		const float FADE_TIME = 1f;//스무스볼륨 시간
		
		// BGM 볼륨
		float _totalVol;
		public float TotalVolume {
			get { return _totalVol; }
			set {
				_totalVol = value;

				foreach (BGMInfo info in bgmList) {
					if (info == null)
						continue;
					if (info.gameObj)
						info.audioSource.volume = info.defaultVolume * _totalVol;//기본볼륨값 기준
				}
			}
		}

		// BGM 뮤트
		bool _mute;
		public bool Mute{
			get { return _mute; }
			set {
				_mute = value;
				
				foreach (BGMInfo info in bgmList) {
					if (info == null)
						continue;
					if (info.gameObj) 
						info.audioSource.mute = _mute;
				}
			}
		}
		//-----------------------------------------
		// BGM 생성
		//-----------------------------------------

		//인스펙터 링크
		public GameObject ambientLoop;
		public GameObject ambient1;
		public GameObject ambient2;

		void Awake()
		{
			instance = this;
		}

		void Start() {
			// 추가된 BGM 이곳에 등록
			//뮤트 적용 여부
            Mute = !PlayerSaveInfo.instance.IsEnableSoundBgm();
		}

		void setObject(eBGM bgm, int ResSndId) {
			GameObject gameObj;
			if (null == bgmList[(int)bgm])
				gameObj = ResourceManager.GetRes_SoundObj(ResSndId);
			else
				gameObj = bgmList[(int)bgm].gameObj;

			setObject(bgm, gameObj);
		}

		void setObject(eBGM bgm, GameObject gameObj){
			// 예외처리
			if (null == gameObj)
				return;
			bgmList[(int)bgm] = new BGMInfo();
			BGMInfo bInfo = bgmList[(int)bgm];
			bInfo.gameObj = gameObj;

			if (bInfo.gameObj) {
				bInfo.index = (int)bgm;
				bInfo.audioSource = bInfo.gameObj.GetComponent<AudioSource>();
				bInfo.defaultVolume = bInfo.audioSource.volume;
				bInfo.audioSource.playOnAwake = false;
			}
		}

		//-----------------------------------------
		// BGM 재생
		//-----------------------------------------
		static public void PlayBgm(eBGM bgm)
		{
            Logger.Assert(null != instance);
            instance._PlayBgm(bgm);
		}

		void _PlayBgm(eBGM bgm) {
			BGMInfo bgmData = getBgmInfo(bgm);
			if (null == bgmData || null == bgmData.gameObj || null == bgmData.audioSource)
				return;

			//Logger.Log("<color=blue> play:" + bgm + "vol is :" + bgmData.audioSource.enabled+"</color>");

			bgmData.audioSource.enabled = true;
			bgmData.audioSource.volume = bgmData.defaultVolume * 0.1f;
			bgmData.audioSource.Play();
			TweenVolume.Begin(bgmData.gameObj, FADE_TIME, bgmData.defaultVolume);
		}

		//이미 재생중인 BGM은 끄고 플레이
		static public void StopAndPlayBgm(eBGM bgm)
		{
            Logger.Assert(null != instance);
            instance._StopAndPlayBgm(bgm);
		}

		void _StopAndPlayBgm(eBGM bgm) {
			BGMInfo bgmData = getBgmInfo(bgm);
			if (bgmData == null || bgmData.gameObj == null)
				return;

			//Logger.Log("<color=blue> stopAndPlay:" + bgm + "</color>");

			foreach (BGMInfo info in bgmList) {
				if (info == null)
					continue;

				if (bgmData == info)
					PlayBgm(bgm);
				else
				{
					if (bgm != (eBGM)info.index)
						_StopBgm((eBGM)info.index);
				}
			}
		}


		//-----------------------------------------
		// BGM 멈춤
		//-----------------------------------------
		static public void StopBgm(eBGM bgm)
		{
            Logger.Assert(null != instance);

            instance._StopBgm(bgm);
		}
		void _StopBgm(eBGM bgm){
			BGMInfo bgmData = getBgmInfo(bgm);
			if (bgmData == null || bgmData.gameObj == null || bgmData.audioSource == null)
				return;

			if (!bgmData.gameObj.activeSelf)
				return;

			if (!IsPlaying(bgm))
				return;

			//Logger.Log("<color=blue> stop:" + bgm + "</color>");

			TweenVolume.Begin(bgmData.gameObj, FADE_TIME, 0);
		}

		// 모두 끄기
		public static void StopBgmAll() {
			foreach (BGMInfo info in instance.bgmList) {
				if (info == null)
					continue;
				StopBgm((eBGM)info.index);
			}
		}

		//-----------------------------------------
		// BGM 활성화
		//-----------------------------------------

		void ActiveBgm(eBGM bgm, bool active) {
			BGMInfo bgmData = getBgmInfo(bgm);
			if (bgmData == null || bgmData.gameObj == null)
				return;

			bgmData.gameObj.SetActive(active);
			bgmData.audioSource.enabled = active;

			//Logger.Log("<color=blue> active is:" + active + "</color>");

			if (!active) {
				//bgmData.audioSource.volume = bgmData.defaultVolume;

				//static이 해제되면서 리스트에 bgm쌓이니 우선 지우고 봐야함
				Destroy(bgmData.gameObj); 
			}
		}

		static public void ActiveBgmAll(bool active)
		{
            Logger.Assert(null != instance);

			foreach (BGMInfo info in instance.bgmList) {
				if (info == null)
					continue;
				instance.ActiveBgm((eBGM)info.index, active);
			}
		}

		//-----------------------------------------
		// BGM 유틸
		//-----------------------------------------

		//BGM을 확률에 따라 재생할지 말지 결정
		void RandomBgm(eBGM bgm, int percent100) {
			percent100 = Math.Min(percent100, 100);
			if (UnityEngine.Random.Range(0, 100) < percent100)
				PlayBgm(bgm);
		}

		bool IsPlaying(eBGM bgm) {
			BGMInfo bgmData = getBgmInfo(bgm);
			if (null == bgmData || null == bgmData.gameObj || null == bgmData.audioSource)
				return false;

			return bgmData.audioSource.isPlaying;
		}

		//현재 재생중인 BGM갯수
		int PlayingCount() {
			int playCount = 0;
			foreach (BGMInfo info in bgmList) {
				if (info == null)
					continue;
				if (IsPlaying((eBGM)info.index))
					playCount++;
			}

			return playCount;
		}

		BGMInfo getBgmInfo(eBGM bgm) {
			return bgmList[(int)bgm];
		}

		//-----------------------------------------
		// BGM 커스텀
		//-----------------------------------------

		const float AMBIENTL_CHECK_TIME = 2.0f;//2초마다 환경음 재생여부 검사
		const float AMBIENT1_CHECK_TIME = 20.0f;
		const float AMBIENT2_CHECK_TIME = 50.0f;
		float ambientL_timer = 0;
		float ambient1_timer = 0;
		float ambient2_timer = 0;


	}
}
