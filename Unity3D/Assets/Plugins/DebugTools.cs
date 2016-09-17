#if (DEV_BUILD || DEBUG_ON)
#define CONDITIONAL_LOG     // 데브 빌드는 로그 보여줘
#endif

#if (UNITY_EDITOR || UNITY_STANDALONE)
#define FILE_LOG            // 컴파일 옵션에서 조절 하게 하자.
#endif

using UnityEngine;
using System;
using System.Diagnostics;

public class DebugTools {

	[Conditional("CONDITIONAL_LOG")]
	public static void Assert(bool comparison, string messageFmt, params System.Object[] args) {
        if (!comparison)
            throw new System.Exception();
        // also used on logging
    }
}


