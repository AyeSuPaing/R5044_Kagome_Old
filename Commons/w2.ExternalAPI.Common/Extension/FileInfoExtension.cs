/*
=========================================================================================================
  Module      : FileInfo拡張クラス(FileInfoExtension.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;

namespace w2.ExternalAPI.Common.Extension
{
	public static class FileInfoExtension
	{
		/// <summary>
		/// 読み取り専用属性を外して強制削除する
		/// </summary>
		/// <param name="file"></param>
		public static void ForceDelete(this FileInfo file)
		{
			if (!file.Exists) return;

			// 読み取り専用であっても、全属性を解除して削除する
			file.Attributes = FileAttributes.Normal;
			file.Delete();
		}
	}
}
