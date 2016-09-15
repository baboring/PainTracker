using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.IO;

namespace HC
{
	static class SoundTable
	{
		static public string GetString(int id)
		{
			return "";
		}
	}
	public class ResourceManager : ManualSingletonMB<ResourceManager>
	{
		Coroutine initCoroutine;

		// 다운로드 완료 되면 알려주는 콜백
		public Action callback_complete;

		public bool IsLoadComplete { get; set; }
		public bool IsManualConfig { get; set; }	// 환경파일을 수동으로 처리 읽게 하자

		static public void CreateInstance()
		{
			Logger.DebugFormat("ResourceManager.CreateInstance");
			// 초기화는 한번만 해야지...
			if (null != instance)
				return;

			if (instance == null) {
				instance = new GameObject("Asset.ResourceManager").AddComponent<ResourceManager>();
				DontDestroyOnLoad(instance.gameObject);
			}
		}

		void Awake()
		{
			Logger.Info("ResourceManager : prepare !!!");
		}

		// Use this for initialization
		public void Initialize(Action<string, float, int, int> callback_progress)
		{
			if (null == initCoroutine ){
				/// 수동으로 해야 할때~
				if (IsManualConfig)
                    DownloadManager.Instance.InitialConfigManual();

				if (!IsLoadComplete) {
					Logger.Debug("ResourceManager : Initialize !!!");
					initCoroutine = StartCoroutine(IDownloadAssetBundles(callback_progress));
				}
			}
		}


		// Asset bundle pool 에 있는 놈들만 로드 한다.
		IEnumerator IDownloadAssetBundles(Action<string, float, int, int> callback_progress)
		{
            Logger.Debug("ResourceManager : DownloadAssetundles Start");

			// 초기화 될때까지 기다려..
			while (!DownloadManager.Instance.ConfigLoaded)
				yield return null;

			// 번들 업데이트 처리 스킵이면 그냥 나가자..
			if (!IsLoadComplete) {
				string szUrl, bundle_file;
				List<string> downloadList = new List<string>();

				// 번들에서 다운로드 할것들 체크
				var bundleList = DownloadManager.Instance.BuiltBundles;
				foreach (var bundle in bundleList) {
					bundle_file = bundle.name + ".assetBundle";
					Logger.Info("[AssetBundle] Check version " + bundle_file);
					szUrl = DownloadManager.Instance.formatUrl(bundle_file);
					//if (DownloadManager.Instance.ProgressOfBundles(new string[] { szUrl }) == 1.0f)
					//    continue;

					WWW wwwUrl = DownloadManager.Instance.GetWWW(szUrl);
					if (null != wwwUrl && null != wwwUrl.assetBundle)
						continue;
					downloadList.Add(bundle.name);
				}

				// download
				int curr = 0;
				float fProgressRatio;
				foreach (string bundle_name in downloadList) {
					bundle_file = bundle_name + ".assetBundle";
					Logger.InfoFormat("[AssetBundle] Download {0}",bundle_file);
					szUrl = DownloadManager.Instance.formatUrl(bundle_file);

					DownloadManager.Instance.StartDownload(szUrl);
					while (DownloadManager.Instance.isInWaitingList(szUrl))
						yield return null;
					curr++;
					while ((fProgressRatio = DownloadManager.Instance.ProgressOfBundles(new string[] { szUrl })) < 1) {
						if (null != callback_progress)
							callback_progress(szUrl, fProgressRatio, curr, bundleList.Length);
						yield return null;
					}

					// 다 받았는지 확인하고 가야지
					while (null == DownloadManager.Instance.GetWWW(szUrl))
						yield return null;

				}
			}
			Logger.Info("ResourceManager : DownloadAssetundles complete");

			// 완료 되었슈~
			if (null != callback_complete)
				callback_complete();

			IsLoadComplete = true;
			initCoroutine = null;

		}

		public T GetResource<T>(string bundleName, string resourceName)
		{
			string szUrl = bundleName + ".assetBundle";
			WWW wwwUrl = DownloadManager.Instance.GetWWW(szUrl);
			Logger.Assert(null != wwwUrl && null != wwwUrl.assetBundle, "Bundle is null : " + szUrl);
			if (null == wwwUrl || null == wwwUrl.assetBundle)
				return default(T);

			var find = Array.Find(DownloadManager.Instance.BuiltBundles, va => va.name == bundleName);
			if (null != find)
			{
				UnityEngine.Object rscObject = wwwUrl.assetBundle.Load(resourceName, typeof(T));
				Logger.Assert(null != rscObject, resourceName + " not found");
				if (null != rscObject)
					return (T)Convert.ChangeType(rscObject, typeof(T));
			}
			return default(T);
		}

		// 사운드 
		Dictionary<int, GameObject> mapResourceSound = new Dictionary<int, GameObject>();
		//저장해 놓은 리소스가 있으면 저장해 놓은것 사용, 없으면 새로 읽어들임
		static public AudioSource GetRes_Sound(int ResSndId)			{ return instance._GetRes_SoundSource(ResSndId);	}
		//www로부터 새 리소스를 읽어들인다.
		static public GameObject GetRes_SoundObj(int ResSndId)			{ return instance._GetRes_SoundInstance(ResSndId); }
		//www로부터 새 리소스를 읽어들인다.
		static public GameObject GetRes_SoundObj(string prefab_name)	{ return instance._GetRes_SoundInstance(prefab_name); }

		// 사운드를 바로 플레이 하자.
		static public void PlayRes_Sound(int ResSndId) 
		{
            AudioSource res_Snd = instance._GetRes_SoundSource(ResSndId);
			if (null != res_Snd)
				res_Snd.Play();
		}

		// 사운드 리소스 로딩...
		GameObject _GetRes_SoundInstance(string prefab_name)
		{
			if (string.IsNullOrEmpty(prefab_name))
				return null;
			// 어셋 번들을 사용 하지 않을떄는 임시로 로컬에서 사용하도록 하자.

			var assetBundle = DownloadManager.Instance.GetWWW("SoundPrefabs.assetBundle");
			Logger.Assert(null != assetBundle, "SoundPrefabs.asset Bundle is null");
			if (null == assetBundle)
				return null;

			UnityEngine.Object prefabObj = assetBundle.assetBundle.Load(prefab_name);
			Logger.Assert(null != prefabObj, "not found " + prefab_name );
			if (null == prefabObj)
				return null;

			AudioSource audioSrc = (prefabObj as GameObject).GetComponent<AudioSource>();
			if (AudioManager.instance.IsMuteEffect)
				audioSrc.mute = true;

			var obj = Instantiate(prefabObj, new Vector3(0, 0, 0), Quaternion.identity);


			if (obj.GetType() == typeof(AudioClip)) {
				AudioSource AudioObj = new GameObject(prefab_name, typeof(AudioSource)).AddComponent<AudioSource>();
				AudioObj.clip = obj as AudioClip;
				return AudioObj.gameObject;
			}

			return obj as GameObject;
		}

		// 사운드 리소스 로딩...
		GameObject _GetRes_SoundInstance(int ResSndId)
		{
			var res = SoundTable.GetString(ResSndId);
			Logger.Assert(null != res, "Sound Resource not found " + ResSndId);
			if (null == res)
				return null;

			return _GetRes_SoundInstance(res);
		}

		// 사운드 리소스 로딩...
		AudioSource _GetRes_SoundSource(int ResSndId)
		{
			// 이미 사용한 리소스는 재활용 하자.
			if (mapResourceSound.ContainsKey(ResSndId))
				return mapResourceSound[ResSndId].GetComponent<AudioSource>();

			var res = SoundTable.GetString(ResSndId);
			Logger.Assert(null != res, "Sound Resource not found " + ResSndId);
			if (null == res)
				return null;

			GameObject obj = _GetRes_SoundInstance(res);
			Logger.Assert(null != obj,"sound prefab is not found : " + res);
			if (null == obj) {
				return null;
			}
			// 리소스 추가...
			mapResourceSound.Add(ResSndId, obj);
			obj.transform.parent = this.transform;

			return obj.GetComponent<AudioSource>();

		}
	}
}
