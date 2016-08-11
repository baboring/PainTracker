using UnityEngine;

namespace PocketSphinx
{
    static public class SphinxPluginAndroid
    {
        private static AndroidJavaObject GetCurrentActivity() {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return jc.GetStatic<AndroidJavaObject>("currentActivity");
        }

        private static AndroidJavaClass sphinxHandle;
        public static bool Init(string name)
        {
            bool result = false;
            if (Application.platform == RuntimePlatform.Android)  {
                sphinxHandle = new AndroidJavaClass("edu.cmu.pocketsphinx.Plugin.AndroidPlugin");
                DebugTools.Assert(null == sphinxHandle, "sphinxHandle is fail to initialize!!");
                result = sphinxHandle.CallStatic<bool>("_Init", name);
            }
            return result;
        }

        public static bool _Create() {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            return sphinxHandle.CallStatic<bool>("_Create");
        }

        public static void _DispatchMessage(string func,string msg)
        {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            sphinxHandle.CallStatic("_DispatchMessage", func, msg);
        }

        // Show 
        public static void _ToastShow(string msg) {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            GetCurrentActivity().Call("runOnUiThread", new AndroidJavaRunnable(() => {
                sphinxHandle.CallStatic("_ToastShow", msg);
            }));
        }

        public static void _Dispose() {
            DebugTools.Assert(null != sphinxHandle, "sphinxHandle is null");
            sphinxHandle.CallStatic("_Dispose");
        }
    }

}

