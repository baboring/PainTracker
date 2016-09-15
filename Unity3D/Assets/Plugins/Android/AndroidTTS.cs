using UnityEngine;

public static class AndroidTTS
{
	private static AndroidJavaClass ttsHandle;
	private static AndroidJavaObject GetCurrentActivity()
	{
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		return jc.GetStatic<AndroidJavaObject>("currentActivity");
	}

	public static bool Init(string gameObjectName, string local = UnitedStates, string enginePkg = null)
	{
		bool result = false;
		if (Application.platform == RuntimePlatform.Android) {
			ttsHandle = new AndroidJavaClass("com.narith.pocketsphinx.SpeechManager");
			DebugTools.Assert(null == ttsHandle, "ttsHandle is fail to initialize!!");

			result = ttsHandle.CallStatic<bool>("_Init", gameObjectName);
			if (enginePkg == null) {
				result = ttsHandle.CallStatic<bool>("_Create", GetCurrentActivity(), local);
			}
			else {
				result = ttsHandle.CallStatic<bool>("_Create", GetCurrentActivity(), local, enginePkg);
			}
		}
		return result;
	}

	public const string UnitedStates = "en-US";

}
