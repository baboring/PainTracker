using UnityEngine;
using System;
using System.Collections;

namespace HC
{

	public class MessageBoxWindow : MonoBehaviour
	{
		public delegate void listener(ButtonHandler buttonHandle);
		public delegate void closed(MessageBoxWindow msgbox);

		public event listener OnButtonClick = null;			// 버튼 클릭시 알림
		public event closed OnClosed = null;				// 창이 닫힐때 알림

		public bool IsActived { get; private set; }			// 창이 Start루틴을 실행 할때 비로소 Active 된거라고 봐야 한다.

		public GameObject objectWindow = null;              // 전체 윈도우
		public UISprite sprite_popupWindow = null;      // 윈도우 sprite가 붙은 오브젝트
		public UISprite sprite_titleBar = null;         // 타이틀바 sprite 가 붙은 오브젝트
		public UILabel objectTitle = null;				   // 제목 오브젝트
		public UILabel objectDesc = null;					 // 설명 오브젝트
		public GameObject objectCancelButton = null;        // Cancel 버튼
		public UILabel objectCancelLabel = null;		    // Cancel 버튼 문자열 스프라이트
		public GameObject objectOkButton = null;            // OK 버튼
		public UILabel objectOkLabel = null;				// OK 버튼 문자열 스프라이트

		public GameObject objectCustomButton = null;        // custom 버튼
		public UILabel objectCustomLabel = null;			// custom 버튼 문자열 스프라이트

		//Animation animationPlay = null;

		//static string szResAniPopupOpen = "UI/Animations/Popup_Open";
		//static string szResAniPopupClose = "UI/Animations/Popup_Close";
		TweenScale tweenScale;
		Vector3 originalScale;

		// 다이얼로그 등장시 ClickLock을 기억 해서 종료시 풀어 주기 위한 변수
		bool isLocked = false;

		void Awake()
		{
			tweenScale = objectWindow.GetComponent<TweenScale>();
			if (null == tweenScale)
				tweenScale = objectWindow.AddComponent<TweenScale>();


            UIPanel panel = this.GetComponent<UIPanel>();
            if(null == panel)
                panel = objectWindow.GetComponent<UIPanel>();
            if(null != panel)
                panel.depth = MessageBoxManager.GetActivedMessageBoxCnt() + 20;
            //tweenScale.enabled = false;

            //animationPlay = objectWindow.GetComponent<Animation>();
            //if( null == animationPlay)
            //    animationPlay = objectWindow.AddComponent<Animation>();

            //animationPlay.playAutomatically = false;
            //animationPlay.AddClip(Resources.Load(szResAniPopupOpen) as AnimationClip ,"Open");
            //animationPlay.AddClip(Resources.Load(szResAniPopupClose) as AnimationClip, "Close");
        }


		///////////////////////////////////////////////
		// Splash time out close 처리
		float self_close_time = 0;
		public void SetSelfCloseTime(float fTime)
		{
			self_close_time = fTime;
		}
		IEnumerator IWaitTimeClose(float wait_time)
		{
			UILabel msg = objectDesc.GetComponent<UILabel>();
			string text = msg.text;
			while (wait_time > 0) {
				msg.text = text + string.Format("\n{0} 초..", (int)wait_time);
				yield return new WaitForSeconds(1);
				wait_time -= 1;
			}
			handleButtonClick(null);
		}
		///////////////////////////////////////////////


		void Start()
		{
			ResizeWindowBox();
			originalScale = objectWindow.transform.localScale;

			//SetClickLock(true);	// 창 뜨는 동안에는 버튼 클릭 안되게
			//if (null != animationPlay && animationPlay.Play("Open"))
			//    while (animationPlay.isPlaying)
			//        yield return null;

			//SetClickLock(false);	// 창 뜨는 동안에는 버튼 클릭 안되게 해제

			windowAnimation(true);

			// 활성화 단계 인식용
			IsActived = true;

			///////////////////////////////////
			// Splash time out close
			if (self_close_time > 0) {
				StartCoroutine(IWaitTimeClose(self_close_time));
			}
		}

		// Click Process check ( 로컬에서 동작중에 코루틴으로 종료되었을때 Lock 카운트 오류 발생 해제용 )
		void SetClickLock(bool bFlag)
		{
			Logger.Assert(isLocked != bFlag, "Duplicate click lock !!");

			if (isLocked != bFlag) {
				isLocked = bFlag;
				if (bFlag)
					ButtonHandler.ClickLock();
				else
					ButtonHandler.ClickUnlock();
			}
		}

		void windowAnimation(bool isOpen)
		{
			if (null == tweenScale)
				return;
			// 열고 있는 중에 닫으면?
			if (isLocked)		// 이미 동작중이면 기존것 삭제 시키자 ( 다시 설정 할 것이니 )
			{
				tweenScale.onFinished = null;
				SetClickLock(false);
			}
			SetClickLock(true);

			tweenScale.Reset();
			tweenScale.duration = 0.15f;

			if (isOpen) {
				tweenScale.from = new Vector3(0.0001f, 0.0001f, originalScale.z);
				tweenScale.to = originalScale;
				tweenScale.method = UITweener.Method.EaseIn;
			}
			else {
				tweenScale.from = originalScale;
				tweenScale.to = new Vector3(0.0001f, 0.0001f, originalScale.z);
				tweenScale.method = UITweener.Method.EaseOut;
			}

			tweenScale.SetOnFinished(() => {
				SetClickLock(false);

				if (isOpen) {
					if (self_close_time > 0)
						StartCoroutine(IWaitTimeClose(self_close_time));
				}
				else {
					// 닫힘을 알림
					MessageBoxManager.Remove(this);
					Destroy();
				}
			});

			tweenScale.enabled = true;
		}

		//IEnumerator Close()
		//{
		//    SetClickLock(true);	// 창 뜨는 동안에는 버튼 클릭 안되게
		//    //if (null != animationPlay && animationPlay.Play("Close"))
		//    //    while ( animationPlay.isPlaying)
		//    //        yield return null;
		//    while (tweenScale.enabled)
		//        yield return null;

		//    SetClickLock(false);	// 창 뜨는 동안에는 버튼 클릭 안되게 해제

		//    // 닫힘을 알림
		//    MessageBoxManager.Close(this);
		//    if (null != OnClosed)
		//        OnClosed(this);

		//    Destroy();

		//}

		// 삭제 하자
		void Destroy()
		{
			if (null != this && null != this.gameObject)
				GameObject.DestroyObject(this.gameObject);
		}

		// Ok 처리용
		public void Ok()
		{
			if (null == objectOkButton)
				return;

			// ok 버튼에서
			if (null != objectOkButton.GetComponent<ButtonHandler>())
				objectOkButton.GetComponent<ButtonHandler>().InvokeClick();
		}

		// cancel 처리용
		public void Cancel()
		{
			if (null == objectCancelButton || objectCancelButton.gameObject.activeSelf == false) {

				if (null == objectOkButton)
					return;
				// cancel 버튼이 없으니 ok 버튼에서
				if (null != objectOkButton.GetComponent<ButtonHandler>())
					objectOkButton.GetComponent<ButtonHandler>().InvokeClick();
			}
			else if (null != objectCancelButton && objectCancelButton.gameObject.activeSelf == true) {
				if (null != objectCancelButton.GetComponent<ButtonHandler>())
					objectCancelButton.GetComponent<ButtonHandler>().InvokeClick();
			}
		}

		// 흐미 강제로 닫는걸 넣어?
		public void InvokeClose()
		{
			if (null != OnClosed)
				OnClosed(this);
			OnClosed = null;
		}
		// --------------------------------------------------
		// 버튼 핸들러 처리 한다.
		public void handleButtonClick(ButtonHandler buttonHandle)
		{
			if (null != OnButtonClick)
				OnButtonClick(buttonHandle);

			// 애니메이션 종료후 처리를 위해서 코루틴 사용.
			//MessageBoxManager.CloseMessageBox(this);
			//GameObject.DestroyObject(this.gameObject);


			if (null == this)
				return;

			// 종료문을 코루틴으로 애니메이션 종료 되면 처리하게 하자.
			//StartCoroutine(Close());

			windowAnimation(false);
		}

		void ResizeWindowBox()
		{
			return;
			const int DEFAULT_LINE = 3;  //메세지박스의 기본 크기가 3줄에 맞춰져 있다.
			UILabel msg = objectDesc.GetComponent<UILabel>();
			if (null == msg || null == msg.bitmapFont)
				return;
			Vector2 emptyLineSize = NGUIText.CalculatePrintedSize("");
			Vector2 textSize = NGUIText.CalculatePrintedSize(msg.processedText);

			//Vector2 emptyLineSize = msg.font.CalculatePrintedSize("", msg.supportEncoding, msg.symbolStyle);
			//Vector2 textSize = msg.font.CalculatePrintedSize(msg.processedText, msg.supportEncoding, msg.symbolStyle);
			int lineCnt = (int)(textSize.y / emptyLineSize.y);
			float charHeight = msg.transform.localScale.y;

			if (DEFAULT_LINE < lineCnt) {
				int addedLineCnt = lineCnt - DEFAULT_LINE;
				float moveDelta = addedLineCnt * (charHeight / 2);

				//팝업의 윈도우BG 크기조절
				sprite_popupWindow.transform.localScale = new Vector3(
 					sprite_popupWindow.transform.localScale.x,
 					sprite_popupWindow.transform.localScale.y + addedLineCnt * charHeight,
 					sprite_popupWindow.transform.localScale.z);

				//타이틀바 위치 조절(위로)
				sprite_titleBar.transform.localPosition = new Vector3(
 					sprite_titleBar.transform.localPosition.x,
 					sprite_titleBar.transform.localPosition.y + moveDelta,
 					sprite_titleBar.transform.localPosition.z);

				//타이틀 텍스트 위치 조절(위로)
				objectTitle.transform.localPosition = new Vector3(
 					objectTitle.transform.localPosition.x,
 					objectTitle.transform.localPosition.y + moveDelta,
 					objectTitle.transform.localPosition.z);

				//취소버튼 위치 조절(아래로)
				objectCancelButton.transform.localPosition = new Vector3(
 					objectCancelButton.transform.localPosition.x,
 					objectCancelButton.transform.localPosition.y - moveDelta,
 					objectCancelButton.transform.localPosition.z);

				//확인 버튼 위치 조절(아래로)
				objectOkButton.transform.localPosition = new Vector3(
 					objectOkButton.transform.localPosition.x,
 					objectOkButton.transform.localPosition.y - moveDelta,
 					objectOkButton.transform.localPosition.z);
			}
		}

		// 파괴 될때... 클릭 상태 복구 해야문제가 없다.
		void OnDestroy()
		{
			if (isLocked)
				SetClickLock(false);
		}
	}

}