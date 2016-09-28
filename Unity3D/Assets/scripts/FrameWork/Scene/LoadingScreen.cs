using UnityEngine;
using System.Collections;

public class LoadingScreen : ManualSingletonMB<LoadingScreen> {

	static GameObject objLoadingScreen;
	public UISpriteAnimation spriteAni;

	public static void Load() {
		if(null == objLoadingScreen) {
			var prefabObj = Resources.Load("Prefab/ScreenLoading") as GameObject;
			objLoadingScreen = GameObject.Instantiate(prefabObj) as GameObject;
		}
	}
	void Awake() {
		instance = this;
		DontDestroyOnLoad(this);
	}
	// Use this for initialization
	void Start () {
		spriteAni.Play();
	}
	
	public void Show(bool bFlag) {
		gameObject.SetActive(bFlag);
		spriteAni.Play();
	}

	public void SetProress(float percent) {

	}
}
