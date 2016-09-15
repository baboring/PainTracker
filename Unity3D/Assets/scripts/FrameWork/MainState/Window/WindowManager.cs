/********************************************************************
	created:	2013/11/25
	filename:	WindowManager.cs
	author:		Benjamin
	purpose:	[]
*********************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HC
{
	// UI 전용
    public class WindowManager : SingletonMB<WindowManager>
	{

		//public WindowBase[] _objWindow = new WindowBase[(int)WndID.Max];

		public GameObject _objHide = null;
		public GameObject _objRoot = null;

		private Dictionary<WndID, WindowBase> _mapPopup = new Dictionary<WndID, WindowBase>();
		private Dictionary<WndID, WindowBase> _mapWindows = new Dictionary<WndID, WindowBase>();

		// popup
		private List<WndID> _HistroyPopupWindow = new List<WndID>();

        private List<InitialWindows> _listInitialWindows = new List<InitialWindows>();

		const int DEPTH_STEP = -40;

		new void Awake()
		{
			var uiRoot = gameObject.AddComponent<UIRoot>();
			uiRoot.scalingStyle = UIRoot.Scaling.ConstrainedOnMobiles;
			uiRoot.manualWidth = 640;
			uiRoot.manualHeight = 1136;
			gameObject.layer = LayerMask.NameToLayer("UI");
			gameObject.AddComponent<UIPanel>().depth = 2;

			ClearAll();

			base.Awake();
		}

		// 인스턴스 생성하자.
		public static void CreateInstance()
		{
            Logger.Info("WindowManager.CreateInstance");

			instance.Initial();
		}

		// 등록된 윈도우 갯수를 알고 싶다고..
		static public int Count {
			get { return instance._mapWindows.Count; }
		}

		// 현재 열려있 는 윈도우 갯수
		static public int CountOpenPopups {
			get { return instance._HistroyPopupWindow.Count; }
		}

        public static void AddInitialWindows(InitialWindows v) {
            instance._listInitialWindows.Add(v);
        }
		// 등록된 모든 창을 끈다.
		public void ClearAll()
		{
			_mapPopup.Clear();
			_HistroyPopupWindow.Clear();

			foreach (var objWin in _mapWindows.Values) {
				//GameObject.DestroyObject(objWin.gameObject);
				DestroyImmediate(objWin.gameObject);
			}
			_mapWindows.Clear();

            // Initial Window 객체 삭제 (Root)
            foreach (var iw in _listInitialWindows)
                DestroyImmediate(iw.gameObject);
            _listInitialWindows.Clear();

			// 팝업 오픈시 Hierachy구조를 위해 Root 생성
			if (null != _objRoot)
				DestroyImmediate(_objRoot);
			_objRoot = new GameObject();
			_objRoot.name = "Root";
			_objRoot.transform.parent = gameObject.transform;
			_objRoot.transform.localScale = new Vector3(1, 1, 1);
			_objRoot.transform.localPosition = new Vector3(0, 0, DEPTH_STEP);
			_objRoot.layer = LayerMask.NameToLayer("UI");

			// 팝업으로 등록된 Window를 담아둘 Object
			if (null != _objHide)
				DestroyImmediate(_objHide);
			_objHide = new GameObject();
			_objHide.name = "Hide";
			_objHide.transform.parent = gameObject.transform;
			_objHide.transform.localScale = new Vector3(1, 1, 1);
			_objHide.transform.localPosition = new Vector3(0, 0, DEPTH_STEP);
			_objHide.layer = LayerMask.NameToLayer("UI");
		}

		// =============================================================================
		#region Facade Functions
		static public bool RegisterWindow(WndID eID, WindowBase proc, WndType eWinType) { return instance._RegisterWindow(eID, proc, eWinType); }
 		static public bool IsRegisterWindow(WndID eID) { return instance._IsRegisterWindow(eID);  }
		static public bool IsRegisterWindow(GameObject go) { if (!WindowManager.IsInstanced) return false; return instance._IsRegisterWindow(go); }
		static public bool OpenPopup(WndID wndID, Action<WindowBase> callback_closed = null) { return instance._OpenPopup(wndID, callback_closed); }
		static public bool ClosePopup(WndID wndID, WndCloseType eType = WndCloseType.None) { return instance._ClosePopup(wndID, eType); }
		static public bool ClosePopupTop(WndCloseType eType = WndCloseType.None) { return instance._ClosePopup(instance.ePopupTopID, eType); }
		static public void ClosePopupAll(WndCloseType eType = WndCloseType.None) { instance._ClosePopupAll(eType); }
		static public void SetActiveWindow(WndID eID, bool bActive) { instance._ShowWindow(eID, (bActive) ? WndVisible.Show : WndVisible.Hide); }
		static public bool IsActiveWindow(WndID eID) { return instance._IsActiveWindow(eID); }
		static public GameObject GetWindowObj(WndID eID) { return instance._GetWindowObj(eID); }
		static public WindowBase GetWindowProc(WndID eID) { return instance._GetWindowProc(eID); }	// 내부에서만 사용하도록 (deprecate)
		static public T GetWindow<T>() where T : WindowBase
		{
			//Logger.Log("Find : " + typeof(T));
			//var result = instance._objWindow.Where(va => null != va && va.GetType() == typeof(T));
			var result = instance._mapWindows.Values.Where(va => null != va && va.GetType() == typeof(T));
            Logger.Assert(result.Count() > 0, " Window not found type : {0}", typeof(T));

            return (T)result.First();
		}

		void OnLoadedWindow(WindowBase wndBase)
		{
			foreach (var objWin in instance._mapWindows.Values) {
				if (null != objWin )
					objWin.OnLoadedWindow(wndBase);
			}

			// 여기서 해줘야 된다.
			MainStateManager.OnLoadedWindow(wndBase);
		}
	
		#endregion
		// =============================================================================
		// =============================================================================
		#region Normal Window

		// 각각의 창들이 자신의 오브젝트를 등록 시켜서 창 관리를 할 수 있게 한다.
		public bool _RegisterWindow(WndID eID, WindowBase proc, WndType eWinType)
		{
			//Logger.Log("Set Window " + eID.ToString());

			if (WndID.Max == eID)
			{
				Logger.Error("WindowID is not asigned !! ( {0} )", eID);
				return false;
			}
			if (null == proc)
			{
				Logger.Error("Window is null !! ( {0} )", eID);
				return false;
			}
			if (_mapWindows.ContainsKey(eID)) {
				if (proc == _mapWindows[eID]) {
                    Logger.Error("Window set Duplicated !! ( {0} : {1} )", eID, proc.gameObject.name);
					return false;
				}
                Logger.Error("Window replaced was another GameObject !! ( {0} : {1} )", eID, proc.gameObject.name);
				return false;
			}
			//Logger.Log("Register Window !! ( " + (int)eID + " ) " + eID + " : " + obj.name);

			_mapWindows.Add(eID, proc);



			// popup window라면 이리로 땡겨 온다.
			if (eWinType == WndType.Popup)
			{
				AttachGameObject(_objHide, proc.gameObject, DEPTH_STEP);
				_mapPopup.Add(eID, proc); // popup용 으로 보관
			}

			// 꺼져 있는 오브젝트의 경우 켰다가 끄게 한다.
			if (!proc.IsInitialSelfClose)
				StartCoroutine(ObjectActiveDeactive(eID, proc));
			else
				OnLoadedWindow(proc);	// Notify other windows


			return true;

		}


		GameObject _GetWindowObj(WndID eID)
		{
			return _GetWindowProc(eID).gameObject;
		}

		WindowBase _GetWindowProc(WndID eID)
		{
			if (!_IsRegisterWindow(eID))
                Logger.Error("Window is not ready !!! ( {0} ) : \n{1}", eID, UnityEngine.StackTraceUtility.ExtractStackTrace());
			return _mapWindows[eID];
		}

		T _GetWindow<T>() where T : WindowBase
		{
			//Logger.Log("Ref : " + typeof(T));
			var result = _mapWindows.Values.Where(va => null != va && va.GetType() == typeof(T));
			Logger.Assert(result.Count() > 0, "not found window type : " + typeof(T));
			if (result.Count() < 1) {
				return default(T);
			}
			return (T)result.First();
		}

		bool _IsRegisterWindow(WndID eID)
		{
			if (_mapWindows.ContainsKey(eID))
				return true;
			return false;
		}

		bool _IsRegisterWindow(GameObject go)
		{
			if (null == go)
				return false;
			if (_mapWindows.Values.Where(pa => (null != pa && pa.gameObject == go)).Count() > 0)
				return true;

			return false;
		}

		bool _IsActiveWindow(WndID eID)
		{
			if (!_IsRegisterWindow(eID)) {
                Logger.Error("Window is not ready !!! ( {0} ) : \n{1}", eID, UnityEngine.StackTraceUtility.ExtractStackTrace());
				return false;
			}

			return _mapWindows[eID].gameObject.activeSelf;
		}

		// 윈도우 switch
		static public void SwitchWindow(WndID from, WndID to)
		{
			instance._SwitchWindow(GetWindowProc(from), GetWindowProc(to));
		}
		static public void SwitchWindow(WindowBase from, WindowBase to)
		{
			instance._SwitchWindow(from, to);
		}
		void _SwitchWindow(WindowBase from, WindowBase to)
		{
			StartCoroutine(_ISwitchWindow(from, to));
		}
		IEnumerator _ISwitchWindow(WindowBase from, WindowBase to)
		{
			if (null != to)
				to.ShowWindow(WndVisible.Show);
			yield return new WaitForEndOfFrame();
			if (null != from)
				from.ShowWindow(WndVisible.Hide);
		}

		// Active window
		void _ShowWindow(WndID eID, WndVisible eShowWindow)
		{
			if (!_IsRegisterWindow(eID)) {
                Logger.Error("Window is not ready !!! ( {0} ){1} : \n{2}", eID, eShowWindow,UnityEngine.StackTraceUtility.ExtractStackTrace());
				return;
			}

			if (eShowWindow == WndVisible.Show && _mapWindows[eID].gameObject.activeSelf)
				return;

			_mapWindows[eID].ShowWindow(eShowWindow);

		}

		// 같은 프레임에서 Active 후 Deactive 하면 Awake는 동작하나 Start는 다음 프레임에서 동작하는지 호출되지 않는다.
		IEnumerator ObjectActiveDeactive(WndID eID, WindowBase wndBase)
		{
			if (null == wndBase)
				yield break;
			// 개별적으로 등록 된것들은 한번씩 켰다가 끈다.
			if (wndBase.gameObject.activeSelf == false)
			{
				wndBase.gameObject.SetActive(true);
				//Logger.Log("Active Window !! ( " + eID.ToString() + " : " + go.name + " )");
			}
			yield return null;
			if (null == wndBase)
				yield break;
			wndBase.gameObject.SetActive(false);
			wndBase.stopwatch.Stop();

            Logger.Debug("Initial Window " + eID.ToString() + " , time : " + wndBase.stopwatch.ElapsedMilliseconds);
            OnLoadedWindow(wndBase);	// Notify other windows
		}
		#endregion
		// =============================================================================
		// =============================================================================
		#region Popuop Window
		// open popup window
		public WndID ePopupTopID
		{
			get
			{
				if (_HistroyPopupWindow.Count < 1)
					return WndID.Max;
				return _HistroyPopupWindow[_HistroyPopupWindow.Count - 1];
			}
		}
		// open popup
		bool _OpenPopup(WndID wndID, Action<WindowBase> callback_closed)
		{
			if (ePopupTopID == wndID)
				return false;

			// whether winId is available popup type.
			if (!_mapPopup.ContainsKey(wndID))
			{
                string title = R.GetSystemMsg(R.SystemMsg.eKey.TitleWarning).StrValue;
                string msg = R.GetSystemMsg(R.SystemMsg.eKey.MsgNotRegisterPopup).StrValue;
				MessageBoxManager.Show(title, msg);
				//MessageBoxManager.Show("Warning !!", "팝업으로 등록된 윈도우가 아닙니다.", eMessageBox.MB_OK);
				return false;
			}

			// whether winId is available popup type.
			if (FindHistoryPopup(wndID) > -1)
			{
                string title = R.GetSystemMsg(R.SystemMsg.eKey.TitleWarning).StrValue;
                string msg = R.GetSystemMsg(R.SystemMsg.eKey.CantUseDuplicatingPopup).StrValue;
				MessageBoxManager.Show(title, msg);
				//MessageBoxManager.Show("Warning !!", "팝업 윈도우는 중복해서 사용 할 수 없습니다.", eMessageBox.MB_OK);
				return false;
			}

			// Preprocessor Popup
			List<WndID> lstClosePopup = new List<WndID>();

			// TopMost 옵션이 있는 놈이 있으면 그 아래로 붙여야 한다.
			WndID wndTopMost = WndID.Max;
			foreach (var find in _HistroyPopupWindow) {
				if (_mapPopup[find].IsOption(WndOption.TopMost)) {
					wndTopMost = find;
					break;
				}
			}
			// TopMost가 있다면 그 앞을 최상위로
			int nInsertIndex = -1;
			if (WndID.Max != wndTopMost) {
				nInsertIndex = _HistroyPopupWindow.FindIndex(va => va == wndTopMost);
			}

			// tail game object choice
			GameObject objTail = _objRoot;  // root 의 끝
			WndID eTailWnd = WndID.Max;
			// TopMost가 있다면
			if (WndID.Max != wndTopMost)
				eTailWnd = (0 < nInsertIndex) ? _HistroyPopupWindow[nInsertIndex] : eTailWnd;
 			else
				eTailWnd = ePopupTopID;		// TopMost 앞 또는 맨뒤

			// 붙을 윈도우가 존재 한다면 그놈을 쓰자.
			if (eTailWnd != WndID.Max)
				objTail = _mapPopup[eTailWnd].gameObject;

			// if available type then move to hierachy tree.
			AttachGameObject(objTail, _mapPopup[wndID].gameObject, DEPTH_STEP);
			
			// TopMost 윈도우가 있다면 뒤로 다시 붙이자
			if (WndID.Max != wndTopMost) {
				AttachGameObject(_mapPopup[wndID].gameObject, _mapPopup[wndTopMost].gameObject, DEPTH_STEP);
				_HistroyPopupWindow.Insert(nInsertIndex, wndID);
			}
			else {
				_HistroyPopupWindow.Add(wndID);
			}

			// Attach listener object.
			_mapPopup[wndID].SetCallbackClosed(callback_closed);

			// 자신이 열리고 있다는것을 알려준다.
			_mapPopup[wndID].ShowWindow(WndVisible.Show);

			

			// 닫아줄 팝업들
			foreach (WndID closeId in lstClosePopup)
				_ClosePopup(closeId,WndCloseType.Cancel);     // 바로 닫으면 깜박거림

			return true;
		}

		// close all popup window
		void _ClosePopupAll(WndCloseType eCloseType)
		{
			Array.ForEach<WndID>(_HistroyPopupWindow.ToArray(), p => {
				if (_mapPopup.ContainsKey(p))
					_ClosePopup(p, eCloseType);
			});
		}

		// close popup window
		bool _InvokeCancel(WndID wndID)
		{
			// whether winId is available popup type.
			if (!_mapPopup.ContainsKey(wndID))
				return false;

			// remove id in history.
			int nFindIdx = FindHistoryPopup(wndID);
			if (nFindIdx < 0)
				return false;

			// 독점 모드이면 할 수 없다..
			if (_mapPopup[wndID].IsExclusiveLock)
				return false;

			// 먼저 cancel invoke 후 닫혀지지 않은 창은 닫아준다.
			return _mapPopup[wndID].InvokeCancel();
		}

		// close popup window
		bool _ClosePopup(WndID wndID, WndCloseType eCloseType)
		{
			//Logger.LogFormat(ColorType.yellow, "ClosePopup : {0},{1}", wndID, eCloseType);

			// whether winId is available popup type.
			if (!_mapPopup.ContainsKey(wndID))
				return false;

			// Cancel 처리 먼저 해본다. 
			// - 취소 처리가 없으면 바로 닫기 있으면 그것 처리하고 거기서 닫기를 처리 할테니 대기..
			if (eCloseType == WndCloseType.Cancel) {
				if (_InvokeCancel(wndID)) {
					return true;
				}
			}

			// remove id in history.
			int nFindIdx = FindHistoryPopup(wndID);
			if (nFindIdx < 0)
				return false;

			// 독점 모드이면 할 수 없다..
			if (_mapPopup[wndID].IsExclusiveLock)
				return false;

			GameObject parentObj = null;
			if (ePopupTopID != wndID)
			{
				// 첫번째라면 Root와 다음을 연결 시켜줌
				if (nFindIdx == 0)
					parentObj = _objRoot;
				else // 바로위 윈도우와 연결되게 한다.
					parentObj = _mapPopup[_HistroyPopupWindow[nFindIdx - 1]].gameObject;
			}

			//  부모를 옮겨감
			if (ePopupTopID != wndID)
				AttachGameObject(parentObj, _mapPopup[_HistroyPopupWindow[nFindIdx + 1]].gameObject, DEPTH_STEP);

			// 내껀 이제 빠져야지
			//AttachGameObject(_objHide, _mapPopup[wndID].gameObject, DEPTH_STEP);
			// Hide 때는 Depth가 문제가 된다...현재 Depth를 유지 하면서 hide로 이동 하자..
			_mapPopup[wndID].gameObject.transform.parent = _objHide.transform;

			// 자신이 닫힌다는것을 알려준다.
			_mapPopup[wndID].ShowWindow(WndVisible.Hide);

			// Dettach listener object.
			_mapPopup[wndID].SetCallbackClosed(null);
			_HistroyPopupWindow.RemoveAt(nFindIdx);

			return true;
		}

		// Search popupIs in history 
		public int FindHistoryPopup(WndID ePopupId)
		{
			return _HistroyPopupWindow.FindIndex(p => p == ePopupId);
		}
		// search parent gameobject by argument id
		public GameObject GetParentObject(WndID ePopupId)
		{
			int nIdx = FindHistoryPopup(ePopupId);

			// Root 오브젝트는 부모가 없다.
			if (nIdx > 0)
				return _mapPopup[_HistroyPopupWindow[nIdx - 1]].gameObject;

			return null;
		}

		bool AttachGameObject(GameObject objParent, GameObject objChild, float depth)
		{
			objChild.transform.parent = objParent.transform;

			// adjust position and scale
			objChild.transform.localScale = new Vector3(1, 1, 1);
			objChild.transform.localPosition = new Vector3(0, 0, depth);

			return true;
		}


		#endregion
		// =============================================================================
	}

}