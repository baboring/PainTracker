using UnityEngine;

using System;
using System.Collections.Generic;

namespace HC
{
	// state history 관리용 클래스 나중에 리펙토링 하는데 사용하자...
	public abstract class HistoryManager<T> where T : class, new()
	{
		public List<T> Histroy = new List<T>();
	}

	// state base
	public abstract class AppStateBase : MonoBehaviour
	{
		protected AsyncOperation async_operation_;
		protected WndID[] eInitialWindows;					// window 초기 리스트 목록
		public WndID[] eLoadedWindows = new WndID[0];		// window 로딩 체크용이다.

		public bool IsPrearedState = false;				// 각 스테이트별로 완료상태
		protected bool IsDoenLoadWindows { get; private set; }		// 읽을 것들 모두 읽었다

		// 해당 상태를 최초 상태로 초기화 하는곳
		abstract public void Reset();
		virtual public void OnPrepare()		// 준비... Enter 이전에 호출됨
		{
			IsPrearedState = false;
			IsDoenLoadWindows = false;
			// ----------------------------------------------
			// 공통이 되겠네...
			// ----------------------------------------------
			if (null != eInitialWindows) {
                //Logger.Assert(null != eInitialWindows);
                if (eLoadedWindows.Length < 1)
                    eLoadedWindows = new WndID[eInitialWindows.Length];
                Array.Copy(eInitialWindows, eLoadedWindows, eInitialWindows.Length);
            }
            // ----------------------------------------------

		}
		abstract public void OnEnter();
		abstract public void OnLeave();

		abstract public void OnUpdate();

		// 상태 전환이 일어나면 무조건 하는것
		virtual public void OnClearAll()
		{
			//WindowManager.instance.ClearAll();
		}

		// 창 로드 처리 
		virtual public void OnLoadedWindow(WindowBase wndBase)
		{
            Logger.DebugFormat("{0}:LoadedWindow : {1}",this,wndBase.GetID());
			Logger.Assert(null != wndBase, "Window is null !!");
            //Logger.Assert(eLoadedWindows.Length > 0, "mismatch window {0}", wndBase.GetID());
			if (eLoadedWindows.Length < 1)
				return;
			// 찾자..
			var find = Array.FindIndex<WndID>(eLoadedWindows, va => va == wndBase.GetID());
            Logger.Assert(find > -1, wndBase.GetID() + " not found window list!!");
			if (find > -1)
				eLoadedWindows[find] = WndID.Max;

			// 로딩이 다된 상태인지를 알고 싶다.
			var findRest = Array.FindIndex<WndID>(eLoadedWindows, va => va != WndID.Max);

			// 더이상 로드 할것이 없다면 load complete
			if (findRest < 0)
				IsDoenLoadWindows = true;

		}
	}

}