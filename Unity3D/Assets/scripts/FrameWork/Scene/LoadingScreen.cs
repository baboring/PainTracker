using UnityEngine;
using System.Collections;

public class LoadingScreen : ManualSingletonMB<LoadingScreen> {

	void Awake() {
		instance = this;
		DontDestroyOnLoad(this);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	public void Show(bool bFlag) {
		gameObject.SetActive(bFlag);
	}

	public void ShowLoadingProress(float percent) {

	}
}
