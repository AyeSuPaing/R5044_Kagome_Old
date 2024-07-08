/*
=========================================================================================================
  Module      : Directories (Directories.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;

namespace w2.Commerce.Batch.CrossPointCooperation
{
	public class Directories
	{
		/// <summary>Activeファイルパス</summary>
		public static string ActiveFilePath
		{
			get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Active"); }
		}
		/// <summary>Completeファイルパス</summary>
		public static string CompleteFilePath
		{
			get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Complete"); }
		}
		/// <summary>Errorファイルパス</summary>
		public static string ErrorFilePath
		{
			get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Error"); }
		}
		/// <summary>Tmpファイルパス</summary>
		public static string TmpFilePath
		{
			get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tmp"); }
		}
	}
}