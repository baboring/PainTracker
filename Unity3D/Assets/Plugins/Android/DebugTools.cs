#if (DEV_BUILD || DEBUG_ON)
#define CONDITIONAL_LOG     // 데브 빌드는 로그 보여줘
#endif

using System.Diagnostics;

public class DebugTools {

	[Conditional("CONDITIONAL_LOG")]
	public static void Assert(bool comparison, string messageFmt, params System.Object[] args) {
        if (!comparison)
            throw new System.Exception();
        // also used on logging
    }

	[Conditional("CONDITIONAL_LOG")]
	public static void Warning(string logString) {
		UnityEngine.Debug.Log(logString);
	}
}


