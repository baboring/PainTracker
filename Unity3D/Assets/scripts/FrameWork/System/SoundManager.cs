//yarny
//사운드 사용시 추적이 가능한 관리자 역할 + 유틸기능
using System;
using UnityEngine;

namespace HC
{

	public enum eSND
	{
		Snd_Click,			//Click Sound
		Snd_TradeGold,		//골드거래
		LB_TradeDia,		//다이아거래
		LB_ItemEquip,		//장비교체
		LB_SkinReinforce,	//스킨강화
		LB_BuyPotion,		//물약구매
		LB_Celebration,		//축하음
		LB_Celebration2,	//축하음2
		LB_SK_Lightning,	//스테츄 번개
		LB_SK_Ice,			//스테츄 냉기
		LB_SK_Dark,			//스테츄 암흑
		LB_SK_Fire,			//스테츄 화염
	}
	public class SoundManager : ManualSingletonMB<SoundManager>
	{
		const int NONE = -1;



		static public void CreateInstance()
		{
			Logger.InfoFormat("SoundManager.CreateInstance");

			if (null != instance)
				return;

            if (instance == null) {
                instance = new GameObject("Asset.SoundManager").AddComponent<SoundManager>();
                DontDestroyOnLoad(instance.gameObject);
			}
		}

		string getStringID(eSND sound)
		{
			switch (sound) {
			default: break;
			}
			return string.Empty;
		}

		public AudioClip GetAudioClip(eSND sound)
		{
			switch (sound) {
				case eSND.Snd_Click:
					return this.GetAudioClip("Sound/Click");
			}
			return null;
		}

		public AudioClip GetAudioClip(string resAsset)
		{
			return Resources.Load(resAsset) as AudioClip;
		}

		public AudioSource GetAudioSource(string szName)
		{
			GameObject soundObj = ResourceManager.GetRes_SoundObj(szName);
			Logger.Assert(null != soundObj, szName + " : this sound object is null");
			if (soundObj == null)
				return null;

			AudioSource audio = soundObj.GetComponent<AudioSource>();

			return audio;
		}

		AudioSource begin(eSND sound, float fadeOutTime = 0)
		{
			GameObject soundObj;

			soundObj = ResourceManager.GetRes_SoundObj(getStringID(sound));


			Logger.Assert(null != soundObj, sound + " : this sound object is null");
			if (soundObj == null)
				return null;

			AudioSource audio = soundObj.GetComponent<AudioSource>();

			//시작시 꺼져있는 사운드도 존재한다
			if (!audio.playOnAwake)
				audio.Play();

			//사운드 소리르 짧게 줄이고 싶다면..
			if (fadeOutTime > 0)
				TweenVolume.Begin(soundObj, fadeOutTime, 0);

			return audio;
		}

		static public AudioSource Begin(eSND sound, float fadeOutTime = 0)
		{
            if (null == instance) {
				return null;
			}
            return instance.begin(sound, fadeOutTime);
		}
	}
}