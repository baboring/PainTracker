using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	public GameObject hideMenu;
	public UIPopupList popupList;
	public UIInput uiInput;

	private string engineName = "";
	private string enginePkg ="";

	string[] nameArray;
	string[] pkgArray;
	// Use this for initialization
	void Start () {
		EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates);

		nameArray = EasyTTSUtil.GetEngineNameArray();
		pkgArray = EasyTTSUtil.GetEnginePkgArray();

		if (null != pkgArray)
			foreach (var item in pkgArray) {
			popupList.AddItem(item);
		}

		EasyTTSUtil.SpeechAdd("Welcome");
	}
	public void OpenSetting()
	{
		EasyTTSUtil.OpenTTSSetting();
	}
	public void OnSelectEngine()
	{
		Debug.Log(UIPopupList.current.value);
		//engineName = nameArray[selected];
		EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates, UIPopupList.current.value);
	}
	
	public void QuickProgram() {
		Application.Quit();
	}

	public void OnSubmit()
	{
		EasyTTSUtil.SpeechAdd(uiInput.value);
	}
	public void OnSay()
	{
		//string saySomething = string.Format("My name is %s.", uiInput.value);
		if (uiInput.value.Length > 0)
			Human.instance.OnSay("My name is " + uiInput.value, "shaking_hands_2");
	}
	// Update is called once per frame
	void Update () {
		// 종료 버튼 ( Back key )
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Application.platform == RuntimePlatform.Android) {
				Application.Quit();
			}
			else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
				
			}
		}
		if (Input.GetKeyDown(KeyCode.Menu) || Input.GetKeyDown(KeyCode.Tab)) {
			hideMenu.SetActive(!hideMenu.activeSelf);
		}
	}
	void OnApplicationQuit()
	{
		EasyTTSUtil.Stop();
	}
}
