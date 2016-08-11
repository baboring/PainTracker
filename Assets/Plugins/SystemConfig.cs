using UnityEngine;
using System.Collections;

public class SystemConfig {

    static public bool IsDebugOn { get {return true; }  }

    string GetStreamingAssetsPath() {
        string path;
#if UNITY_EDITOR
        path = "file:" + Application.dataPath + "/StreamingAssets";
#elif UNITY_ANDROID
     path = "jar:file://"+ Application.dataPath + "!/assets/";
#elif UNITY_IOS
     path = "file:" + Application.dataPath + "/Raw";
#else
     //Desktop (Mac OS or Windows)
     path = "file:"+ Application.dataPath + "/StreamingAssets";
#endif

        return path;
    }
}
