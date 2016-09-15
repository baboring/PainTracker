/********************************************************************
	created:	2013/06/13 
	filename:	Vars.cs
	author:		ehalshbest
	purpose:	[ struct, typedef,,etc..]
*********************************************************************/
using UnityEngine;

using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HC
{
	// 오브젝트와 한 BOOL
	public class BOOL
	{
		public bool flag;

		public BOOL(bool flag = false)	{ this.flag = flag; }
		static public bool FALSE		{ get { return false; } }
		static public bool TRUE			{ get { return true; } }
	}

	// app veriosn
	public class AppVersionInfo
	{
		public string Version = string.Empty;
		public int BuildNo;
		public int Size;

	}

}	// __end namespace SB