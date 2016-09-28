/********************************************************************
	created:	2013/11/26
	filename:	ButtonHandler.cs
	author:		Benjamin
	purpose:	[]
*********************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HC
{
	// WigetsColorToggle
	public class WigetsColorToggle : MonoBehaviour
	{
		bool bIsDark = false;

		Dictionary<UIWidget, Color> _lstChild = null;

		public Color toggleColor = new Color(0.4f, 0.4f, 0.4f);


		public bool IsDark
		{
			get { return bIsDark; }
			set
			{
				bIsDark = value;
				Toggle(!bIsDark);
			}
		}

		void Awake()
		{
			// 오브젝트들을 가지고 있자..
			ChangeRoot(this.gameObject);
		}

		public void ChangeRoot(GameObject obj)
		{
			if(null == _lstChild)
				_lstChild = new Dictionary<UIWidget, Color>();
			_lstChild.Clear();
			Util.FetchChildWigets(_lstChild, obj);
		}

		void Toggle(bool value)
		{
			if (null == _lstChild)
				return;
			foreach (var wig in _lstChild)
				wig.Key.color = (value) ? wig.Value : wig.Value * toggleColor;
		}
	}

	public class ButtonHandler : MonoBehaviour
	{
		public const string DEFAULT_ANIMATION = "UI/Animations/button";

		public int _idx = 0;
		public eMessageButton eButton = eMessageButton.None;

		public delegate void listener( ButtonHandler buttonHandle );
		public event listener OnButtonClick = null;		// 버튼 Click시 이벤트 발생
		public event listener OnButtonHold = null;		// 버튼 Hold시 이벤트 발생 ( 오래 눌렀을때 )

		UIPlaySound _uiButtonSound = null;            // Sound
		//ActiveAnimation _uiActiveAnimation = null;      // Animation
		//UIButtonPlayAnimation _uiButtonActiveAni = null;

		bool _bTriggerUseAnimation = false;
		bool _bClickSound = false;
		bool _bColorGrayIfDiabled = true;
		Dictionary<UIWidget, Color> _lstChild = new Dictionary<UIWidget, Color>();
		const float TIME_HOLD = 1;			// Hold 반응시간
		bool _pressed;
		bool _bHoldProcessed = false;							// hold flag
		bool _bProcessed = false;
		bool _bHoldEnable = true;
		bool _was_locked_when_pressed = false;

		TweenScale tweenScale;
		Vector3 originalScale;

		public bool IsHoldEnable
		{
			get { return _bHoldEnable; }
			set { _bHoldEnable = value; }
		}

		// 중복 클릭을 방지 하기 위한 코드
		static public int _LockedClickCount = 0;
		static public bool IsLockedClick
		{
			get { return ( _LockedClickCount > 0 ); }
			private set { }
		}
		static public void ClickLock()
		{
			++_LockedClickCount;
			//Logger.DebugFormat(ColorType.yellow, "Lock Count {0}", _LockedClickCount);

		}
		static public void ClickUnlock()
		{
			--_LockedClickCount;
			//Logger.DebugFormat(ColorType.yellow, "unLock Count {0}", _LockedClickCount);
			Logger.Assert(_LockedClickCount > -1, "_LockedClickCount has been error");
		}

		void Awake()
		{
			// 오브젝트들을 가지고 있자..
			Util.FetchChildWigets(_lstChild, this.gameObject);
		}

		// 꺼지는데 진행중이면 Lock은 풀어줘야지
		void OnDisable()
		{
			if (_bProcessed) {
				//Debug.Log("<color=red>Button deactive by OnDisable</color>");
				// Animation end callback을 cancel 하기 위해 끈다.
				if (null != tweenScale) {
					//local scale이 변경 되었을테니 초기화 하자
					tweenScale.enabled = false;
					this.transform.localScale = originalScale;
				}

				SetClickLock(false);
			}
			// 홀드 중이였으면 상태 초기화 해야지
			if (_bHoldProcessed)
				_bHoldProcessed = false;
		}

		// facade functon
		// idx default is 0
		static public ButtonHandler CreateHandle(int idx, GameObject obj, bool bClickSound = true, bool bClickAnimation = true, listener eventClick = null)
		{
			Logger.Assert(null != obj,"Object is null");
			//if( null == obj )
			//  return null;
			ButtonHandler handle = obj.GetComponent<ButtonHandler>();
			if( null == handle )
				handle = obj.AddComponent<ButtonHandler>();

			handle._idx = idx;
			handle.IsClickSound = bClickSound;
			handle.isTriggerUseAnimation = bClickAnimation;

			if( null != eventClick )
				handle.OnButtonClick += eventClick;

			return handle;
		}

		// click sound 추가
		void _AddClickSound( AudioClip clickSound )
		{
			if (null == clickSound)
				return;

			// 사운드 컴포넌트가 없으면 등록 하자
			_uiButtonSound = gameObject.GetComponent<UIPlaySound>();
			if (null == _uiButtonSound) {
				_uiButtonSound = gameObject.AddComponent<UIPlaySound>();
				_uiButtonSound.audioClip = clickSound;
			}

			AudioManager.instance.AddSoundButton(_uiButtonSound);
			_uiButtonSound.trigger = UIPlaySound.Trigger.OnTrigger;

		}

		float duration = 0.2f; // 애니메이션의 길이입니다.(시간)
		float startDelay = 0.0f; // 애니메이션 시작 전 딜레이입니다.
		Vector3 scaleTo = new Vector3(1f, 1f, 1f); // 오브젝트의 최종 Scale 입니다.
		// 부풀었다가 줄어드는 효과를 위한 AnimationCurve 입니다.
		AnimationCurve animationCurve = new AnimationCurve(
			new Keyframe(0.9f, 0.9f, 0.9f, 1f), // 0%일때 0의 값에서 시작해서
			new Keyframe(0.7f, 1.2f, 1f, 1f), // 애니메이션 시작후 70% 지점에서 1.2의 사이즈까지 커졌다가
			new Keyframe(1f, 1f, 1f, 0f)
		); // 100%로 애니메이션이 끝날때는 1.0의 사이즈가 됩니다.
		
		void _AddClickAnimation(string resAniName)
		{

// 			this.tweenScale = this.GetComponent<TweenScale>();
// 			if (null == this.tweenScale)
// 				this.tweenScale = gameObject.AddComponent<TweenScale>();

 			this.originalScale = this.transform.localScale;

			tweenScale = TweenScale.Begin(gameObject, duration, scaleTo);
			tweenScale.from = new Vector3(0.5f, 0.5f, 0.5f);
			tweenScale.duration = duration;
			tweenScale.delay = startDelay;
			//tweenScale.method = UITweener.Method.BounceIn; // AnimationCurve 대신 이것도 한번 써보세요.
			tweenScale.animationCurve = animationCurve;

			// 모든 애니매이션이 끝나면 버튼잠김 풀고 이벤트 호출
			this.tweenScale.SetOnFinished(this.OnAnimationEnd);
			this.tweenScale.enabled = false;

			// Animation 을 사용 할떄..
		//	ActiveAnimation uiActiveAni = gameObject.GetComponent<ActiveAnimation>();

		//	// Animation 컴포넌트가 없으면 등록 하자
		//	if (null == uiActiveAni) {
		//		_uiActiveAnimation = gameObject.AddComponent<ActiveAnimation>();
		//		if (null != _uiActiveAnimation) {
		//			Animation ani = gameObject.GetComponent<Animation>();

		//			ani.AddClip(Resources.Load(resAniName) as AnimationClip, "button");
		//			ani.clip = ani.GetClip("button");
		//			ani.playAutomatically = false;
		//			ani.cullingType = AnimationCullingType.BasedOnRenderers;
		//		}
		//	}
		//	else {
		//		_uiActiveAnimation = uiActiveAni;
		//	}

		//	_uiButtonActiveAni = gameObject.GetComponent<UIButtonPlayAnimation>();
		//	if (null == _uiButtonActiveAni) {
		//		_uiButtonActiveAni = gameObject.AddComponent<UIButtonPlayAnimation>();
		//		_uiButtonActiveAni.eventReceiver = this.gameObject;
		//		_uiButtonActiveAni.callWhenFinished = "OnAnimationEnd";
		//		_uiButtonActiveAni.trigger = AnimationOrTween.Trigger.OnTrigger;
		//	}

		}

		/// event message
		void OnPress( bool isPressed )
		{
			if( isPressed )
				_was_locked_when_pressed = ButtonHandler.IsLockedClick;

			if (ButtonHandler.IsLockedClick) {
				_pressed = false;
				return;
			}

			if( isPressed ) {
				if (! _pressed) {
					_pressed = true;
					// hold 기능 On 일때만
					if (_bHoldEnable)
						StartCoroutine(WhilePressed());
				}
			}
			else {
				_pressed = false;
			}
		}

		IEnumerator WhilePressed()
		{
			float elapsed = 0.0f;
			while( elapsed < TIME_HOLD ) {
				if( ! _pressed )
					yield break;
				elapsed += UnityEngine.Time.deltaTime;
				yield return null;
			}
			if (null != OnButtonHold)
				OnButtonHold( this );
			_bHoldProcessed = true;
		}

		// Click Process check
		void SetClickLock(bool bFlag)
		{
			Logger.Assert(_bProcessed != bFlag, "Duplicate click lock !!");

			if (_bProcessed != bFlag) {
				_bProcessed = bFlag;

				if (bFlag)
					ButtonHandler.ClickLock();
				else
					ButtonHandler.ClickUnlock();
			}
		}

		// delegate event clear all
		public void ClearClickEvent()
		{
			OnButtonClick = null;
			OnButtonHold = null;
		}

		// 외부에서 클릭 기능 동작 시킨다.
		public void InvokeClick()
		{
			Logger.Assert(_bProcessed == false, "don't Invoke On Clicked state !!");
			if (null != OnButtonClick)
				OnButtonClick(this);

			_bHoldProcessed = false;
		}

		// ClickSound option
		public bool IsClickSound
		{
			get { return _bClickSound; }
			set
			{
				// 사운드 붙여보자 
				if (value && null != SoundManager.instance)	// 리소스 관리자가 없으면 사운드 못 붙이지...
				{
                    var sndClip = SoundManager.instance.GetAudioClip(eSND.Snd_Click);
					if (null != sndClip)
						_AddClickSound(sndClip);
				}
				_bClickSound = value;
			}
		}       // trigger option

		public bool isTriggerUseAnimation
		{
			get { return _bTriggerUseAnimation; }
			set
			{
				// 애니메이션 붙여 보자
				if (value)
					_AddClickAnimation(ButtonHandler.DEFAULT_ANIMATION);
				_bTriggerUseAnimation = value;
			}
		}

		// collider 를 Enable Disable 시킨다.
		public bool IsEnabled
		{
			get
			{
				Collider col = collider;
				return col && col.enabled;
			}
			set
			{
				Collider col = collider;
				if (!col)
					return;

				if (col.enabled != value) {
					// UIImageButton을 가지고 있을 경우 Disable 버튼 처리를 하게 한다.
					UIImageButton scriptUIButton = gameObject.GetComponent<UIImageButton>();
					if (null != scriptUIButton)
						scriptUIButton.isEnabled = value;
					else
						col.enabled = value;

					// 컬러로 Diable 음영 조절 하려면....
					if (_bColorGrayIfDiabled) {
						foreach (var wig in _lstChild)
							wig.Key.color = (value) ? wig.Value : wig.Value * new Color(0.4f, 0.4f, 0.4f);
					}
				}
			}
		}


		//=======================================
		// 버튼 애니메이션
		//=======================================
		void playAnimation()
		{
			SetClickLock(true);//버튼잠김

			tweenScale.ResetToBeginning();

			tweenScale.enabled = true;
		}

		// 외부에서 강제 클릭 시킨다.
		void OnClick()
		{
			// 중복 클릭 및 다른 버튼 클릭시 무시
			bool clickLock = (ButtonHandler.IsLockedClick || !IsEnabled || _was_locked_when_pressed);
			if (clickLock) {
				return;
			}

			// Hold 상태 이면 무시
			if( _bHoldProcessed ) {
				_bHoldProcessed = false;
				return;
			}

			Logger.Assert(!_bProcessed,"click error");

			if (null == tweenScale)
				this.InvokeClick();//바로 이벤트 호출
			else
				playAnimation();//애니메이션 동작후 이벤트 호출
			
			if (null != _uiButtonSound)
				gameObject.SendMessage("OnTrigger");
		}

		public void OnAnimationEnd()
		{
			// animation Click 의 경우 Unlock
			SetClickLock(false);
			tweenScale.enabled = false;
			this.InvokeClick();
		}



	}
}