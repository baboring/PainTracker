/********************************************************************
	created:	2013/11/25
	filename:	WindowBase.cs
	author:		Benjamin
	purpose:	[]
*********************************************************************/

using UnityEngine;
using System;
using System.Collections;

namespace HC
{
	public enum WndType
	{
		Normal = 0,
		Popup
	}

	public enum WndVisible
	{
		None = 0,							// 현상태 유지
		Hide,							// 감추기
		Show,							// 보이기
	}

	// 닫는 형태 구분 enum
	public enum WndCloseType
	{
		None = 0,	// 그냥 닫기
		Cancel,		// 취소로 닫기
		Ok			// 확인으로 닫기
	}


	public enum WndOption : int
	{
		None		= 0,
		TopMost		= 0x01000000,	// Top Most
	}

	public class WindowHelper
	{
		// 이동 처리 ( 일단 임시 처리 )
		static public void MoveTo(GameObject obj, float fDuration, Vector3 vPos) {
			TweenPosition proc = obj.GetComponent<TweenPosition>();
			if (null == proc) {
				proc = obj.AddComponent<TweenPosition>();
			}

			if (null != proc) {
				TweenPosition.Begin(obj, fDuration, vPos);
			}
		}
		// 현재 위치에서 delta 만큰
		static public void Move(GameObject obj, float fDuration, Vector3 vDelta) {
			TweenPosition proc = obj.GetComponent<TweenPosition>();
			if (null == proc) {
				proc = obj.AddComponent<TweenPosition>();
			}

			if (null != proc) {
				
				vDelta.x = proc.value.x + vDelta.x;
				vDelta.y = proc.value.y + vDelta.y;
				vDelta.z = proc.value.z + vDelta.z;
				TweenPosition.Begin(obj, fDuration, vDelta);
			}
		}
	}

	// Window 관리용 중간 베이스 클래스
	public class WindowBase : MonoBehaviour
	{
		[HideInInspector]
		protected WndID eWindowID = WndID.Max;
		protected WndType eWindowType = WndType.Normal;
		protected int eWindowOption = 0;				// 윈도우 옵션이다. ( TopMost 를 위한것... )

		protected Action<WindowBase> callback_closed = null;
		public GameObject objAnimation = null;

		enum AniState
		{
			None = 0,
			Openning,
			Closing,
		}
		// 초기화 이후 닫는것을 본인이 알아서 할것임...( 다른 창을 참조하고 알아서 끄기 위함 )
		protected bool _IsInitialSelfClose = false;
		public bool IsInitialSelfClose		{ get { return _IsInitialSelfClose; } }

		// 해당 윈도우를 독점으로 사용할 것이니 WindowManager가 닫지 못하게 처리시..
		protected bool _IsExclusiveLock = false;
		public bool IsExclusiveLock			{ get { return _IsExclusiveLock; } }

		// 창 종료시 Default Cancel 처리용 Invoke Call 참조용
		protected ButtonHandler _InvokeCancelHandler;	 // 창 열때마다 처리... 본인이 닫으면 처리 안되도록..

		// 애니 종료 전에 꺼질 경우  ClickLock을 풀어야 해~~
		bool _bProcessed = false;

		// animation 상태를 알기 위한것임...
		//AniState _eAniState = AniState.None;

		// animation imp
		//
		//Animation animationPlay = null;
		TweenScale tweenScale;
		Vector3 originalScale;

		//static string szResAniPopupOpen = "UI/Animations/Popup_Open";
		//static string szResAniPopupClose = "UI/Animations/Popup_Close";
		
		// 디버깅용 타이머 로딩시 시간소요 타임워치
		public System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

		virtual protected void Awake()
		{
			//Debug.Log("Awake : " + eWindowID);

			stopwatch.Start();

			
			//if (null != objAnimationWindow)
			//{
			//    animationPlay = objAnimationWindow.GetComponent<Animation>();
			//    if (null == animationPlay)
			//        animationPlay = objAnimationWindow.AddComponent<Animation>();

			//    animationPlay.playAutomatically = false;
			//    animationPlay.AddClip(Resources.Load(szResAniPopupOpen) as AnimationClip, "Open");
			//    animationPlay.AddClip(Resources.Load(szResAniPopupClose) as AnimationClip, "Close");

			//}

			if (null != objAnimation) {
                tweenScale = objAnimation.GetComponent<TweenScale>();
				if (null == tweenScale)
                    tweenScale = objAnimation.AddComponent<TweenScale>();

				tweenScale.enabled = false;
                originalScale = objAnimation.transform.localScale;

				//animationPlay.playAutomatically = false;
				//animationPlay.AddClip(Resources.Load(szResAniPopupOpen) as AnimationClip, "Open");
				//animationPlay.AddClip(Resources.Load(szResAniPopupClose) as AnimationClip, "Close");
			}

			//WindowManager.RegisterWindow(wndBase.GetID(), wndBase, wndBase.GetWndType());
			WindowManager.RegisterWindow(this.GetID(), this, this.GetWndType());
		}
		public WndID GetID() { return eWindowID; }
		public WndType GetWndType() { return eWindowType; }

		// 윈도우 옵션 추가
		public void AddOption(WndOption wndOption)
		{
			eWindowOption |= (int)wndOption;
		}
		// 윈도우 옵션 체크
		public bool IsOption(WndOption wndOption)
		{
			return ((eWindowOption & (int)wndOption) > 0);
		}

		// 꺼지는데 진행중이면 Lock은 풀어줘야지
		void OnDisable()
		{
			if (_bProcessed) {
				//Debug.Log("<color=red>WindowBase deactive by OnDisable</color>");
				SetClickLock(false);
			}
		}

		// Click Process check
		void SetClickLock(bool bFlag)
		{
			Logger.Assert(_bProcessed != bFlag,"SetClickLock");

			if (_bProcessed != bFlag) {
				_bProcessed = bFlag;

				if (bFlag)
					ButtonHandler.ClickLock();
				else
					ButtonHandler.ClickUnlock();
			}
		}

		
		// open ani
		//protected IEnumerator OpenAni()
		//{
		//    // 창 뜨는 동안에는 버튼 클릭 안되게
		//    SetClickLock(true);

			
		//    _eAniState = AniState.Openning;
		//    if (null != animationPlay && animationPlay.Play("Open"))
		//        while (animationPlay.isPlaying)
		//            yield return null;
			
		//    // 창 뜨는 동안에는 버튼 클릭 안되게 해제
		//    SetClickLock(false);
		//    _eAniState = AniState.None;
		//}

		// close ani
		//protected IEnumerator CloseAni( bool deactivate )
		//{
		//    // 창 닫는동안에는 버튼 클릭 안되게
		//    SetClickLock(true);
		//    _eAniState = AniState.Closing;

		//    // @benjamin 20140116 이걸 왜 넣었었을까...
		//    //gameObject.transform.localPosition = new Vector3( 0, 0, 0 );

		//    if( null != animationPlay && animationPlay.Play( "Close" ) )
		//        while( animationPlay.isPlaying )
		//            yield return null;

		//    // 창 닫는동안에는  버튼 클릭 안되게 해제
		//    SetClickLock(false);
		//    _eAniState = AniState.None;

		//    if( deactivate )
		//        gameObject.SetActive( false );
		//}

		void windowAnimation(bool isOpen) {
			SetClickLock(true);

			tweenScale.Reset();
			tweenScale.duration = 0.15f;

			if (isOpen) {
				tweenScale.from = new Vector3(0.0001f, 0.0001f, originalScale.z);
				tweenScale.to = originalScale;
				tweenScale.method = UITweener.Method.EaseIn;
			} else {
				tweenScale.from = originalScale;
				tweenScale.to = new Vector3(0.0001f, 0.0001f, originalScale.z);
				tweenScale.method = UITweener.Method.EaseOut;
			}

            tweenScale.SetOnFinished(()=>{
				SetClickLock(false);

				if (!isOpen)
					gameObject.SetActive(false);
			});

			tweenScale.enabled = true;
		}

		// 일정시간 뒤에 사라지게...
		IEnumerator CloseTimer(float fTime)
		{
			gameObject.transform.localPosition = new Vector3(0, 0, 0);

			yield return new WaitForSeconds(fTime);

			gameObject.SetActive(false);
		}

		// 다음 프레임에서 창 사라지게
		IEnumerator CloseNextFrame()
		{
			gameObject.transform.localPosition = new Vector3(0, 0, 0);

			yield return null;

			gameObject.SetActive(false);
		}
		
		public void SetCallbackClosed( Action<WindowBase> callback_closed )
		{
			this.callback_closed = callback_closed;
		}

		

		protected bool OpenSelfWindow(Action<WindowBase> callback_Closed = null)
		{
			if (WndType.Popup == eWindowType)
				return WindowManager.OpenPopup(eWindowID, callback_Closed);
			else 
			{
				if(null == this.callback_closed)
					SetCallbackClosed(callback_Closed);
				WindowManager.SetActiveWindow(eWindowID, true);
				return true;
			}
		}

		// 창 내부에서 닫기를 처리를 하고 싶을때...
		protected bool CloseSelfWindow()
		{
			if (WndType.Popup == eWindowType)
				return WindowManager.ClosePopup(eWindowID);
			else {
				WindowManager.SetActiveWindow(eWindowID, false);
				return true;
			}
		}

		// cancel 처리로 창을 닥고 싶을때.
		public bool InvokeCancel()
		{
			if (null == _InvokeCancelHandler)
				return false;

			_InvokeCancelHandler.InvokeClick();
			return true;
		}

		// Ani 동작중이였다면 꺼주자
		//void StopAniCoroutine() {
		//    if (_eAniState != AniState.None) {
		//        if (_eAniState == AniState.Openning)
		//            StopCoroutine("OpenAni");
		//        if (_eAniState == AniState.Closing)
		//            StopCoroutine("CloseAni");
		//        //Debug.Log("<color=red>Stop Coroutine : </color> " + _eAniState.ToString());
		//        _eAniState = AniState.None;
		//        if (_bProcessed)
		//            SetClickLock(false);
		//    }
		//}

		// WindowManager만 접근해야 한다.
		public void ShowWindow(WndVisible eShowWindow) {
            Logger.Assert(eShowWindow == WndVisible.Show || eShowWindow == WndVisible.Hide,"unable working !!");
			if (eShowWindow == WndVisible.Show)
				OnOpenWindow();
			else if (eShowWindow == WndVisible.Hide)
				OnCloseWindow();
        }

		virtual protected void OnOpenWindow() {
			gameObject.SetActive(true);

            if (WndType.Popup == eWindowType && null != objAnimation) {
				//StopAniCoroutine();
				//StartCoroutine(OpenAni());
				windowAnimation(true);
			}
		}

		virtual protected void OnCloseWindow()
		{
			// popup 창닫기 처리
			if (WndType.Popup == eWindowType && gameObject.activeSelf)	// Active 상태에서만 Ani 처리가 가능 하다..
			{
				// Animation 있는 경우는 애니 하고..
                if (null != objAnimation) {

					//StopAniCoroutine();
					//StartCoroutine(CloseAni(true));
					windowAnimation(false);
				}
				else // Animation이 없으면 깜박 거리므로 몇초되에 사라지게 해보자
					StartCoroutine(CloseNextFrame());
			}
			else
				gameObject.SetActive(false);

			// 콜백을 돌려주자.
			if (null != callback_closed)
				callback_closed(this);
		}

		// 자기 자신의 부모 객체를 가져온다.
		protected GameObject GetParentObject()
		{
			if (WndType.Popup == eWindowType)
				return WindowManager.instance.GetParentObject(eWindowID);

			return null;
		}

		// 윈도우가 로딩 되었다네....
		virtual public void OnLoadedWindow(WindowBase wndBase)
		{
			//Debug.Log("OnLoadedWindow " + wndBase.name);
		}

	}

}