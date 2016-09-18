using UnityEngine;

using System;
using System.Collections;

namespace HC
{
	public class MainStateMain : MainStateBase
	{
		static MainStateMain _instance;
		static public MainStateMain instance { get { return _instance; } }
		MainStateMain()
		{
            Logger.Assert(_instance == null);
			_instance = this;
			eInitialWindows = new WndID[] {
				WndID.WndMain,
				WndID.WndSetting,
				WndID.WndMenu,
			};
		}

		void Awake()
		{
			Logger.InfoFormat("MainStateNone.Awake !!");

			gameObject.AddComponent<CMUPocketSphinx.TextToSpeech>();
		}

		// 해당 상태를 최초 상태로 초기화 하는곳
		override public void Reset()
		{

		}

		override public void OnEnter()
		{
			// Window Load가 없으니 바로 완료
			if(MainStateManager.PreviousState == eMainState.Splash) {
				StartCoroutine(LoadSceneProcess(()=>OnLoadComplete()));
				return;
			}
			Action<bool> complete = (success) => OnLoadComplete();
			complete(true);
		}

		IEnumerator LoadSceneProcess(Action done) {

			yield return null;
			SCENE_IDX[] lstIdx = new SCENE_IDX[] {
				SCENE_IDX.SYSTEM,
			};

			int loadCount = 0;

			foreach (int idxScene in lstIdx) {
				LoadingScreen.instance.SetProress(loadCount / lstIdx.Length);
				loadCount++;

#if DEV_BUILD && UNITY_EDITOR
				Logger.DebugFormat(ColorType.yellow, "Begin Async Load scene : ( {0} )\n {1}", idxScene, UnityEditor.EditorBuildSettings.scenes[idxScene].path);
#endif
				InitialWindows.IsLoadComplete = false;
				AsyncOperation aoScene = Application.LoadLevelAdditiveAsync(idxScene);

				// go 활성화 모두 시켰으면 고고...
				while (!InitialWindows.IsLoadComplete)
					yield return null;
				Logger.DebugFormat("End Async Load Sceen Done : {0}", idxScene);
				yield return null;
				LoadingScreen.instance.Show(false);
				done();
			}
		}

		override public void OnLeave() { }

		override public void OnUpdate() { }

		override public void OnLoadComplete()
		{
			IsPrearedState = true;
			WindowManager.SetActiveWindow(WndID.WndMain,true);
		}
	}

}