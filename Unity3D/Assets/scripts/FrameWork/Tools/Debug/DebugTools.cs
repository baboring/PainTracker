using UnityEngine;
using System.Collections;

public class DebugTools {
#if !DEBUG
[System.Diagnostics.Conditional("UNITY_EDITOR")]
#endif
    public static void Assert(bool comparison, string messageFmt, params System.Object[] args) {
        if (!comparison)
            throw new System.Exception();
        // also used on logging
    }
}


