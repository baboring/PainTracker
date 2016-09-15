using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HC
{
	public class Protector
	{
		public static bool IsIllegalAPKSignature()
		{
#if UNITY_ANDROID
		    if (Application.platform == RuntimePlatform.Android){
		        var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		        var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		        var mgr = activity.Call<AndroidJavaObject>("getPackageManager");
		        var name = activity.Call<string>("getPackageName");

		        const int GET_SIGNATURES = 64;
		        var packageInfo = mgr.Call<AndroidJavaObject>("getPackageInfo", name, GET_SIGNATURES);
		        var signatures = packageInfo.Get<AndroidJavaObject[]>("signatures");

                //for (int i = 0; i < signatures.Length; i++) {
                //    Logger.Debug("signature = " + signatures[i].Call<string>("toCharsString"));
                //    Logger.Debug("signature hash = " + signatures[0].Call<int>("hashCode").ToString("X"));
                //}

				//나중에 이 Hash값이 변경될수 있는지는 지켜보자
				//문제가 없다면 이값을 pparam 혹은 서버에 저장에 저장하자
				string signatureHash = R.GetSecure(R.Secure.eKey.SignatureHash).Text;
				return (signatureHash != signatures[0].Call<int>("hashCode").ToString("X"));
		    }
#endif
			return false;
		}

		static List<string> GetInstalledAppIDList()
		{
#if UNITY_ANDROID
			//http://developer.android.com/reference/android/content/pm/PackageManager.html#getInstalledPackages(int)
			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject mgr = activity.Call<AndroidJavaObject>("getPackageManager");
			AndroidJavaObject packageInfoList = mgr.Call<AndroidJavaObject>("getInstalledPackages", 0);

			int listSize = packageInfoList.Call<int>("size");

			List<string> packageNameList = new List<string>(listSize);
			for (int i = 0; i < listSize; i++)
			{
				AndroidJavaObject packageInfo = packageInfoList.Call<AndroidJavaObject>("get", i);
				if (packageInfo != null)
				{
					string name = packageInfo.Get<string>("packageName");
					if (name.Equals("android") == true)
						continue;
					packageNameList.Add(name);
				}
			}

			return packageNameList;
#else
			return null;
#endif
		}

		public static bool IsInstalledIllegalApplication()
		{
			if (Application.platform != RuntimePlatform.Android)
				return false;

			//이 값은 서버에 보관할지 클라에 보관할지 정하자...서버가 제일 좋겠지만
			var illegalAppIDs = R.AllillegalApp;

#if BLUESTACK_TEST
			illegalAppIDs = illegalAppIDs.Replace("com.bluestacks.home", ".");
#endif

			List<string> installedAppIDs = GetInstalledAppIDList();
			if (installedAppIDs != null)
			{
				foreach (string id in installedAppIDs)				
					if (illegalAppIDs.ContainsKey(id) == true)
						return true;
			}
			return false;
		}

		//secret 은 반드시 192bit(24bytes) 일 것
		public static string Encrypt(string valueToEnc, string secret)
		{
			byte[] bytes = UTF8Encoding.UTF8.GetBytes(valueToEnc);

			TripleDES des = new TripleDESCryptoServiceProvider();
			des.Key = UTF8Encoding.UTF8.GetBytes(secret);
			des.Mode = CipherMode.ECB;
			ICryptoTransform xform = des.CreateEncryptor();
			byte[] encrypted = xform.TransformFinalBlock(bytes, 0, bytes.Length);

			return Convert.ToBase64String(encrypted, 0, encrypted.Length);
		}

		//secret 은 반드시 192bit(24bytes) 일 것
		public static string Decrypt(string valueToDec, string secret)
		{
			byte[] bytes = Convert.FromBase64String(valueToDec);

			TripleDES des = new TripleDESCryptoServiceProvider();
			des.Key = UTF8Encoding.UTF8.GetBytes(secret);
			des.Mode = CipherMode.ECB;
			ICryptoTransform xform = des.CreateDecryptor();
			byte[] decrypted = xform.TransformFinalBlock(bytes, 0, bytes.Length);

			return UTF8Encoding.UTF8.GetString(decrypted);
		}

		//http://forum.unity3d.com/threads/playerprefs-encryption.26437/
		static public string MD5(string strToEncrypt)
		{
			UTF8Encoding ue = new UTF8Encoding();
			byte[] bytes = ue.GetBytes(strToEncrypt);

			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] hashBytes = md5.ComputeHash(bytes);

			string hashString = "";
			for (int i = 0; i < hashBytes.Length; i++)
			{
				hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
			}

			return hashString.PadLeft(32, '0');
		}
	}

}

/*
 co.kr.fuckingdetect;org.aqua.gg;com.bluestacks.home;cn.mc.sq;com.android.ggjb;com.android.xxx;mon.blue.warcat340;mon.blue.w
arcat521;mon.blue.warcat520;mon.blue.warcat510;mon.blue.warcat500;mon.blue.warcat400;mon.blue.warcat331;mon.blue.warcat3
10;mon.blue.warcat203;mon.blue.warcat332;com.google.android.xyz;idv.aqua.bulldog;com.google.android.kkk;com.cih.game_cih;c
om.cih.gamecih;cn.maocai.gamekiller;cn.luomao.gamekiller;com.cih.gamecih2co.kr.fuckingdetect;org.aqua.gg;com.bluestacks.hom
e;cn.mc.sq;com.android.ggjb;com.android.xxx;mon.blue.warcat340;mon.blue.warcat521;mon.blue.warcat520;mon.blue.warcat510;m
on.blue.warcat500;mon.blue.warcat400;mon.blue.warcat331;mon.blue.warcat310;mon.blue.warcat203;mon.blue.warcat332;com.goo
gle.android.xyz;idv.aqua.bulldog;com.google.android.kkk;com.cih.game_cih;com.cih.gamecih;cn.maocai.gamekiller;cn.luomao.game
killer;com.cih.gamecih2
 * 
 * 
 * string secret = "abcdefghijklmnopqrstuvwx";	//24bytes
			string encStr = Protector.Encrypt("abd!@#QWEASDvbfb4y5", secret);
			Debug.LogWarning("encstr : " + encStr);

			string decStr = Protector.Decrypt(encStr, secret);
			Debug.LogWarning("decstr : " + decStr);
 */