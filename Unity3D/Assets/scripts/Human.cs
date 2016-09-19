using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

	public Animator ani;

    void Awake() {
		if(null == ani)
	       ani = GetComponent<Animator>();
    }
	// Use this for initialization
	
	void Start () {
        
	}

    public void OnPlay() {

    }
	public void OnTouch() {
		string[] anyList = {
			"idle",
			"breathing_idle",
			"waving",
			"fist_pump",
			"standing_greeting",
			"sad_idle",
			"closing",
			"relieved_sigh",
			"yelling",
			"searching_pockets",
			"yawn",
			"shaking_hands_2"
		};
		System.Random Ramdom = new System.Random();
		string aniName = anyList[Ramdom.Next(0,anyList.Length-1)];
		//aniName = "dance";
		Debug.Log(aniName);
		ani.CrossFade(aniName,0.1f);
		//EasyTTSUtil.SpeechAdd(aniName);

	}
	
    public void filterAnimation(string text) {
        switch (text) {
            case "hi":
            case "hello":
                ani.CrossFade("standing_greeting", 0.05f);
                break;
            case "head":
            case "arm":
                ani.CrossFade("sad_idle", 0.05f);
                break;
            case "thigh":
                ani.CrossFade("yelling", 0.05f);
                break;
            case "foot":
                ani.CrossFade("relieved_sigh", 0.05f);
                break;
        }
    }

	public void Motion(string animation = "")
	{
		if (animation.Length > 0)
			ani.CrossFade(animation, 0.03f);
	}
	
}
