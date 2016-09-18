using UnityEngine;
using System.Collections;

public class PrefabManager : ManualSingletonMB<PrefabManager> {

	public GameObject objMessageBox;

	void Awake() {
		instance = this;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
