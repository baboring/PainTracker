//Md5를 이용한 암호화 로컬저장, PlayerPrefs 대체해 사용하면된다
using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
 
public class EncryptedPlayerPrefs  {
    //for EncryptedPlayerPrefs
    public const string PRIVATE_KEY_FOR_PREFS = "f223kl23fp23lbadsfwel4";
    public const string SECRET_FOR_PREFS_1 = "f2jlf3j23";
    public const string SECRET_FOR_PREFS_2 = "sfljalk13";
    public const string SECRET_FOR_PREFS_3 = "sfalkjskf";
    public const string SECRET_FOR_PREFS_4 = "g243jp36i";
    public const string SECRET_FOR_PREFS_5 = "sfgkj234v";

    // Add some values to this array before using EncryptedPlayerPrefs
	static string[] keys = new string[5] { 
        SECRET_FOR_PREFS_1, 
        SECRET_FOR_PREFS_2, 
        SECRET_FOR_PREFS_3, 
        SECRET_FOR_PREFS_4, 
        SECRET_FOR_PREFS_5 
    };

    public static string Md5(string strToEncrypt) {
        UTF8Encoding ue = new UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
 
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
 
        string hashString = "";
 
        for (int i = 0; i < hashBytes.Length; i++) {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
    
    public static void SaveEncryption(string key, string type, string value) {
        int keyIndex = (int)Mathf.Floor(Random.value * keys.Length);
        string secretKey = keys[keyIndex];

		StringBuilder strToEnc = new StringBuilder(128);
		strToEnc.Append(type);
		strToEnc.Append('_');
		strToEnc.Append(PRIVATE_KEY_FOR_PREFS);
		strToEnc.Append('_');
		strToEnc.Append(secretKey);
		strToEnc.Append('_');
		strToEnc.Append(value);

        string check = Md5(strToEnc.ToString());
        PlayerPrefs.SetString(key + "_encryption_check", check);
        PlayerPrefs.SetInt(key + "_used_key", keyIndex);
    }
    
    public static bool CheckEncryption(string key, string type, string value) {
        int keyIndex = PlayerPrefs.GetInt(key + "_used_key");
        string secretKey = keys[keyIndex];

		StringBuilder strToEnc = new StringBuilder(128);
		strToEnc.Append(type);
		strToEnc.Append('_');
		strToEnc.Append(PRIVATE_KEY_FOR_PREFS);
		strToEnc.Append('_');
		strToEnc.Append(secretKey);
		strToEnc.Append('_');
		strToEnc.Append(value);

        string check = Md5(strToEnc.ToString());
        if(!PlayerPrefs.HasKey(key + "_encryption_check")) 
			return false;

        string storedCheck = PlayerPrefs.GetString(key + "_encryption_check");
        return storedCheck == check;
    }
    
    public static void SetInt(string key, int value) {
        PlayerPrefs.SetInt(key, value);
        SaveEncryption(key, "int", value.ToString());
    }
    
    public static void SetFloat(string key, float value) {
        PlayerPrefs.SetFloat(key, value);
        SaveEncryption(key, "float", Mathf.Floor(value*1000).ToString());
    }
    
    public static void SetString(string key, string value) {
        PlayerPrefs.SetString(key, value);
        SaveEncryption(key, "string", value);
    }
    
    public static int GetInt(string key) {
        return GetInt(key, 0);
    }
    
    public static float GetFloat(string key) {
        return GetFloat(key, 0f);
    }
    
    public static string GetString(string key) {
        return GetString(key, "");
    }
    
    public static int GetInt(string key,int defaultValue) {
        int value = PlayerPrefs.GetInt(key);
        if(!CheckEncryption(key, "int", value.ToString())) return defaultValue;
        return value;
    }
    
    public static float GetFloat(string key, float defaultValue) {
        float value = PlayerPrefs.GetFloat(key);
        if(!CheckEncryption(key, "float", Mathf.Floor(value*1000).ToString())) return defaultValue;
        return value;
    }
    
    public static string GetString(string key, string defaultValue) {
        string value = PlayerPrefs.GetString(key);
        if(!CheckEncryption(key, "string", value)) return defaultValue;
        return value;
    }
    
    public static bool HasKey(string key) {
        return PlayerPrefs.HasKey(key);
    }
    
    public static void DeleteKey(string key) {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey(key + "_encryption_check");
        PlayerPrefs.DeleteKey(key + "_used_key");
    }
    
}
