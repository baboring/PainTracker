#if !UNITY_EDITOR
using UnityEngine;
using System.Diagnostics;

public static class Debug
{
	public static bool isDebugBuild
	{
		get { return UnityEngine.Debug.isDebugBuild; }
	}

	public static void Break()
	{
		UnityEngine.Debug.Break();
	}
	
	public static void ClearDeveloperConsole()
	{
		UnityEngine.Debug.ClearDeveloperConsole();
	}
	
	public static void DebugBreak()
	{
		UnityEngine.Debug.DebugBreak();
	}

	//DEV_BUILD || CONDITIONAL_LOG
	[Conditional("DEV_BUILD")]
	[Conditional("CONDITIONAL_LOG")]
	public static void Log(object message)
	{
		UnityEngine.Debug.Log(message);
	}

	[Conditional("DEV_BUILD")]
	[Conditional("CONDITIONAL_LOG")]
	public static void Log(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.Log(message, context);
	}

	[Conditional("DEV_BUILD")]
	[Conditional("CONDITIONAL_LOG")]
	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError(message);
	}

	[Conditional("DEV_BUILD")]
	[Conditional("CONDITIONAL_LOG")]
	public static void LogError(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogError(message, context);
	}

	[Conditional("DEV_BUILD")]
	[Conditional("CONDITIONAL_LOG")]
	public static void LogWarning(object message)
	{
		UnityEngine.Debug.LogWarning(message.ToString());
	}

	[Conditional("DEV_BUILD")]
	[Conditional("CONDITIONAL_LOG")]
	public static void LogWarning(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogWarning(message.ToString(), context);
	}

	[Conditional("DEV_BUILD")]
	[Conditional("CONDITIONAL_LOG")]
	public static void LogException(System.Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}

	[Conditional("DEV_BUILD")]
	[Conditional("CONDITIONAL_LOG")]
	public static void LogException(System.Exception exception, Object context)
	{
		UnityEngine.Debug.LogException(exception, context);
	}

	[Conditional("DEV_BUILD")]
	[Conditional("CONDITIONAL_LOG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	[Conditional("DEV_BUILD")]
	[Conditional("CONDITIONAL_LOG")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}
}
#endif