using UnityEngine;

public class JavaClassHelper {

	public static AndroidJavaObject GetCurrentActivity()
	{
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		return jc.GetStatic<AndroidJavaObject>("currentActivity");
	}
}
