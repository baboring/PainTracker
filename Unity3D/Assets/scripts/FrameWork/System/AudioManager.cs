/********************************************************************
	created:	2013/11/26
	filename:	AudioManager.cs
	author:		Benjamin
	purpose:	[]
*********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HC
{
    public class AudioManager : SingletonMB<AudioManager>
    {
		bool _isMuteEffect = !PlayerSaveInfo.instance.IsEnableSoundEffect();

        // audio Button list
		List<UIPlaySound> m_lstButtonSounds = new List<UIPlaySound>();
        // audio Effect list
        List<AudioSource> m_lstEffectSounds = new List<AudioSource>();

		GameObject objAudioListener = null;


		public bool IsMuteEffect
        {
            get { return _isMuteEffect; }
            set
            {
                _isMuteEffect = value;
				PlayerSaveInfo.instance.SaveEnableSoundEffect(!_isMuteEffect);
				foreach (UIPlaySound uiButtonSound in m_lstButtonSounds)
                {
                    if (null != uiButtonSound)
                        uiButtonSound.enabled = !value;
                }
                foreach (AudioSource effectSound in m_lstEffectSounds)
                {
                    if (null != effectSound)
                        effectSound.mute = value;
                }
            }
        }

		new void Awake()
		{
			base.Awake();
			// audio listener가 없으면 붙여 주자
			AudioListener mListener = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;

			if (mListener == null) {
				objAudioListener = new GameObject("AudioListener");
				objAudioListener.AddComponent<AudioListener>();
				MoveAudioListener(null);
				//GameObject objMainCam = GameObject.FindGameObjectWithTag("MainCamera");
				//Camera cam = (null != objMainCam) ? objMainCam.GetComponent<Camera>() : cam = Camera.main;
				//if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
				//if (cam != null) mListener = cam.gameObject.AddComponent<AudioListener>();
			}
			else {
				objAudioListener = mListener.gameObject;
			}
		}
		void Start()
		{
// 			AudioSource mBGM = GameObject.FindObjectOfType(typeof(AudioSource)) as AudioSource;
// 			if (null == mBGM) {
// 				// 로비 전용이다...필드는 개별로 처리하므로 별도 관리가 필요하다....( Mute 처리를 위해서라도..... )
// 				Logger..Assert(null != prefabLoadingScreen);
// 				GameObject obj = Instantiate(prefabLoadingScreen, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
// 				obj.name = "BGM_Lobby";
// 				obj.transform.parent = this.transform;
// 				mBGM = obj.GetComponent<AudioSource>();
// 				// 수동으로 붙여줘?
// 				//mBGM = this.gameObject.AddComponent<AudioSource>();
// 				//mBGM.loop = true;
// 				//mBGM.clip = Resources.Load(AUDIO_BGM_LOBBY) as AudioClip;
// 			}
// 
// 			AddSoundBGM(mBGM);

		}

        // 버튼 클릭음 관리
		public void AddSoundButton(UIPlaySound uiButtonSound)
        {
            if (null == uiButtonSound)
                return;
            uiButtonSound.enabled = !_isMuteEffect;
			m_lstButtonSounds.Add(uiButtonSound);
        }

        // 버튼 클릭음 관리
		public void RemoveSoundButton(UIPlaySound uiButtonSound)
        {
            if (null == uiButtonSound)
                return;
            for (int i = 0; i < m_lstButtonSounds.Count; ++i)
            {
                if (uiButtonSound == m_lstButtonSounds[i])
                {
                    m_lstButtonSounds.RemoveAt(i);
                    return;
                }
            }
        }

        // 효과음 관리
        public void AddSoundEffect(AudioSource audioSource)
        {
            if (null == audioSource)
                return;
            audioSource.mute = _isMuteEffect;
            m_lstEffectSounds.Add(audioSource);
        }

		static public AudioListener GetAudioListener()
		{
			return (AudioListener)FindObjectOfType(typeof(AudioListener));
		}

		public void MoveAudioListener(Transform parentTr)
		{
			Logger.Assert(null != objAudioListener,"Audio Listener is null");
			if (null == objAudioListener)
				return;
			if (parentTr != null)
			{
				objAudioListener.transform.parent = parentTr;
			}
			else
			{
				objAudioListener.transform.parent = this.transform;
			}
			objAudioListener.transform.localPosition = Vector3.zero;
			
		}

    }

	// fade in / out 처리를 하자.
	public class FadeInOut
	{
		class FadeSound
		{
			public float fade_time = 0.0f;
			public float cur_time = 0.0f;

			public float start_volume = 0.0f;
			public float target_volume = 0.0f;

			public GameObject obj = null;

			AudioSource source = null;
			public bool finish = false;

			public FadeSound(GameObject obj, float start_volume, float target_volume, float fade_time)
			{
				this.fade_time = fade_time;
				this.cur_time = 0.0f;
				this.start_volume = start_volume;
				this.target_volume = target_volume;
				this.finish = false;

				if(null == obj)
					finish = true;
				source = obj.GetComponent<AudioSource>();
			}

			public bool Update(float elpasedTime)
			{
				if (finish || null == obj || obj.activeSelf == false) {
					finish = true;
					return true;
				}

				cur_time += elpasedTime;

				// 시간별 갱신
				if (cur_time > fade_time) {
                    source.volume = (BGM.instance.Mute) ? 0 : target_volume;

					if (target_volume == 0)
						source.Stop();

					finish = true;
				}
				else {
                    source.volume = (BGM.instance.Mute) ? 0 : Mathf.Lerp(start_volume, target_volume, cur_time / fade_time);
				}

				if (source.isPlaying == false) {
					if (source.volume > 0) {
						source.Play();
						source.time = 0.0f;
					}
				}
				else {
					if (source.volume <= 0)
						source.Stop();
				}


				return finish;

			}
		};

		List<FadeSound> fade_list = new List<FadeSound>();


		void Update()
		{
			foreach (FadeSound f in fade_list) {
				f.Update(Time.deltaTime);
			}

			for (int i = fade_list.Count - 1; i >= 0; i--) {
				if (fade_list[i].finish == true)
					fade_list.RemoveAt(i);
			}

		}
	}
}