/********************************************************************
	created:	2013/11/25
	filename:	WindowIDs.cs
	author:		Benjamin
	purpose:	[]
*********************************************************************/

using UnityEngine;
using System;

namespace HC
{
	public enum WndID
	{
        WndMain = 0,					// Lobby 진정한 메인이다.
        WndPlay,
        WndSetting,
		WndMenu,						// main menu
		WndRegister,					// register name or so on data.
        Max
	}

}