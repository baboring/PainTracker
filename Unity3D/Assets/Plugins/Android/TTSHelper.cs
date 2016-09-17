using UnityEngine;
using System.Collections;

public class TTSHelper : MonoBehaviour {

	//public static UIPopupList popupList;

	private static AndroidJavaClass ttsHandle;
	private static AndroidJavaObject GetCurrentActivity()
	{
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		return jc.GetStatic<AndroidJavaObject>("currentActivity");
	}

	public static bool Initialize(string gameObjectName, string local = UnitedStates, string enginePkg = null)
	{
		bool result = false;
		if (Application.platform == RuntimePlatform.Android) {
			ttsHandle = new AndroidJavaClass("com.narith.pocketsphinx.SpeechManager");
			DebugTools.Assert(null != ttsHandle, "ttsHandle is failed to initialize!!");

			ttsHandle.CallStatic<bool>("_Init", gameObjectName);
			if (enginePkg == null)
				result = ttsHandle.CallStatic<bool>("_Create", GetCurrentActivity(), local);
			else
				result = ttsHandle.CallStatic<bool>("_Create", GetCurrentActivity(), local, enginePkg);
		}
		return result;
	}
	public static void OpenTTSSetting()
	{
		if (Application.platform == RuntimePlatform.Android)
			ttsHandle.CallStatic("_OpenTTSSetting");
	}
	public static void Stop()
	{
		if (Application.platform == RuntimePlatform.Android)
			ttsHandle.CallStatic("_ShotDown");
	}
	public static string[] GetEnginePkgArray()
	{
		if (Application.platform == RuntimePlatform.Android)
			return ttsHandle.CallStatic<string[]>("_GetEnginePkgArray");
		Debug.LogWarning("GetEnginePkgArray is only for android");
		return null;
	}
	public static string[] GetEngineNameArray()
	{
		if (Application.platform == RuntimePlatform.Android)
			return ttsHandle.CallStatic<string[]>("_GetEngineNameArray");

		Debug.LogWarning("GetEngineNameArray is only for android");
		return null;
	}
	public static void SetEngineByPackageName(string pkg)
	{
		if (Application.platform == RuntimePlatform.Android)
			ttsHandle.CallStatic("_SetEngineByPackageName",pkg);

		Debug.LogError("SetEngineByPackageName is only for android");
	}
	public static string GetDefaultEngineName()
	{
		if (Application.platform == RuntimePlatform.Android)
			return ttsHandle.CallStatic<string>("_GetDefaultEngineName");

		Debug.LogError("GetEngineNameArray is only for android");
		return null;
	}
	public static void SpeechAdd(string text)
	{
		if (string.IsNullOrEmpty(text))
			return;
		if (Application.platform == RuntimePlatform.Android)
			ttsHandle.CallStatic("_SpeechAdd",text);
	}
	public static void SpeechFlush(string text)
	{
		if (string.IsNullOrEmpty(text))
			return;
		if (Application.platform == RuntimePlatform.Android)
			ttsHandle.CallStatic("_SpeechFrush",text);
	}

	public const string SaudiArabia = "ar-SA";
	public const string SouthAfrica = "en-ZA";
	public const string Thailand = "th-TH";
	public const string Belgium = "nl-BE";
	public const string Australia = "en-AU";
	public const string Germany = "de-DE";
	public const string UnitedStates = "en-US";
	public const string Brazil = "pt-BR";
	public const string Poland = "pl-PL";
	public const string Ireland = "en-IE";
	public const string Greece = "el-GR";
	public const string Indonesia = "id-ID";
	public const string Sweden = "sv-SE";
	public const string Turkey = "tr-TR";
	public const string Portugal = "pt-PT";
	public const string Japan = "ja-JP";
	public const string Korea = "ko-KR";
	public const string Hungary = "hu-HU";
	public const string CzechRepublic = "cs-CZ";
	public const string Denmark = "da-DK";
	public const string Mexico = "es-MX";
	public const string Canada = "fr-CA";
	public const string Netherlands = "nl-NL";
	public const string Finland = "fi-FI";
	public const string Spain = "es-ES";
	public const string Italy = "it-IT";
	public const string Romania = "ro-RO";
	public const string Norway = "no-NO";
	public const string HongKong = "zh-HK";
	public const string Taiwan = "zh-TW";
	public const string Slovakia = "sk-SK";
	public const string China = "zh-CN";
	public const string Russia = "ru-RU";
	public const string UnitedKingdom = "en-GB";
	public const string France = "fr-FR";
	public const string India = "hi-IN";
}
