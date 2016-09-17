using UnityEngine;
using System;
using System.Collections;

namespace HC
{

	public enum SCENE_IDX
	{
        // constant
		MAIN = 0,
		SPLASH,
        // ------------------------
		SYSTEM,
 	}


	public class SceneLoader : ManualSingletonMB<SceneLoader>
	{
		//protected GameObject progress_bar_ = null;
        SCENE_IDX previous_scene_;
        SCENE_IDX current_scene_;

		// 씬이 변경되면 알아서 알려줘요
        public delegate void NotifyChangedScene(SCENE_IDX state);
		public NotifyChangedScene OnChangedScene;

		/////////////////////////////////////////////////////////////////
		static public void CreateInstance()
		{
			// 초기화는 한번만 해야지...
			Logger.Assert(null == instance, "Twice initial instance !!");
			if (null != instance)
				return;

			if (instance == null) {
				instance = new GameObject("SceneLoader").AddComponent<SceneLoader>();
				DontDestroyOnLoad(instance.gameObject);
			}
		}

		static public void Reset()
		{
			if (instance == null)
				return;
            instance.current_scene_ = SCENE_IDX.MAIN;
            instance.previous_scene_ = SCENE_IDX.MAIN;
		}

		// callback
		public void OnLevelWasLoaded( int level )
		{
#if UNITY_EDITOR
			Logger.InfoFormat(ColorType.yellow,"OnLevelWasLoaded : ( {0} ) => {1}", level, UnityEditor.EditorBuildSettings.scenes[level].path);
#else
			Logger.InfoFormat("OnLevelWasLoaded : {0}", level );
#endif
        }

		// 현재 씬 상태
        static public SCENE_IDX GetCurrentScene()
		{
			if (null == instance)
				return 0;
			return instance.current_scene_;
		}

		// 이전 씬 상태
        static public SCENE_IDX GetPreviousScene()
		{
			if (null == instance)
				return 0;
			return instance.previous_scene_;
		}


		// 비동기 로딩 처리
		static public void LoadAsyncIdx(SCENE_IDX sceneIndex, Action<bool> callback)
		{
			Logger.Assert(null != instance);
			instance.StartCoroutine(instance.LoadAsyncSceneIdx(sceneIndex, callback));
		}

		// 비동기 씬 로딩
		IEnumerator LoadAsyncSceneIdx(SCENE_IDX sceneIndex, Action<bool> callback)
		{
			Logger.Debug("LoadAsyncSceneIdx : " + sceneIndex);
			yield return null;

			AsyncOperation async_op = Application.LoadLevelAsync((int)sceneIndex);

            Logger.Assert(null != async_op, "LoadLevelAsync sceneIndex is invaild");
			//if( progress_bar_ != null && async_op != null ) {
			//    UISlider slider = progress_bar_.GetComponent< UISlider >();
			//    slider.sliderValue = async_op.progress;
			//    Logger.LogFormat( "progress : {0}", async_op.progress );

			//    if( async_op.isDone ) {
			//        slider.sliderValue = 1.0f;
			//        async_op = null;
			//    }
			//}
			while (!async_op.isDone)
				yield return null;
			async_op = null;

			if (null != callback)
				callback(true);

			if (null != OnChangedScene)
				OnChangedScene(current_scene_);
		}

	}

}
