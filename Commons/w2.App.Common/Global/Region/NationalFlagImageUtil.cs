/*
=========================================================================================================
  Module      : 国旗画像ユーティリティクラス(NationalFlagImageUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.IO;

namespace w2.App.Common.Global.Region
{
	public class NationalFlagImageUtil
	{
		/// <summary>
		/// 国旗画像のフルパス取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>ファイルのフルパス</returns>
		public static string GetNationalFlagPath(string fileName)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return string.Empty;

			var fileFullName = fileName + ".png";

			if (File.Exists(Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, Constants.PATH_NATIONAL_FLAG, fileFullName)) == false) return string.Empty;

			string webFilePath = Path.Combine(Constants.PATH_ROOT, Constants.PATH_NATIONAL_FLAG, fileFullName);
			return webFilePath;
		}
	}
}
