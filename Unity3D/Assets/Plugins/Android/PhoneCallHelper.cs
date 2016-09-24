using UnityEngine;

public class PhoneCallHelper
{
    private static AndroidJavaClass javaClassTTS;

    public static bool Initialize(string gameObjectName)
    {
        bool result = false;

        if (Application.platform == RuntimePlatform.Android)
        {
            javaClassTTS = new AndroidJavaClass("com.narith.pocketsphinx.PhoneCall");
            DebugTools.Assert(null != javaClassTTS, "ttsHandle is failed to initialize!!");

            return javaClassTTS.CallStatic<bool>("_Create", JavaClassHelper.GetCurrentActivity(), gameObjectName);
        }
        return result;
    }

    public static bool CallTo(string phoneNumber)
    {
        if (Application.platform == RuntimePlatform.Android)
            return javaClassTTS.CallStatic<bool>("_MakeACall", phoneNumber);

		return false;
    }
}