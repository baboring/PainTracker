using UnityEngine;
using System.Collections;
using PocketSphinx;

public class ShpinxVoiceRecognizeDemo : MonoBehaviour {

    SphinxVoiceRecognizer recognizer;
    // Use this for initialization
    void Start()
    {
        if (SystemConfig.IsDebugOn)
        {
            var console = gameObject.AddComponent<uLinkConsoleGUI>();
            console.showByKey = KeyCode.Menu;
            console.SetVisible(false);
        }

        // attach Voice Recognizer
        recognizer = gameObject.AddComponent<SphinxVoiceRecognizer>();
        recognizer.callback = (msg) => {
            strInput += "\n" + msg;
        };

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    string strInput = "Input : ";
    void OnGUI()
    {
        /*
        var stackTraceLabelStyle = new GUIStyle(GUI.skin.box);
        stackTraceLabelStyle.fontSize = 10 * 2;
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.Label(strInput, stackTraceLabelStyle);
        GUILayout.EndHorizontal();

        if (GUI.Button(new Rect(400, 50, 150, 150), "Call OK!!"))
            recognizer.Notice("Ok...!!!");
        if (GUI.Button(new Rect(560, 50, 150, 150), "Call Dispose"))
            recognizer.Dispose();
        //if (GUI.Button(new Rect(210, 100, 200, 200), "Call Add Nethod"))
            //strInput += ajc.CallStatic<string>("Add",5,9).ToString();
         * */
     }
    public void _OnHypothsisResult(string text) {
        switch (text) {
            case "digits":
            case "turn left":
                Human.instance.OnTurnLeft();
                break;
            case "phones":
            case "turn right":
                Human.instance.OnTurnRight();
                break;
        }
    }
 }
