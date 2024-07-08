/*
=========================================================================================================
  Module      : アフィリエイトモジュール基底クラス(AffiliateModuleBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;

namespace w2.Commerce.Batch.AffiliateReporter
{
	public class ReportModuleBase
	{
		public ReportModuleBase()
		{
			// テンポラリディレクトリ作成
			if (Directory.Exists(Constants.PHYSICALDIRPATH_TEMPDIR) == false)
			{
				Directory.CreateDirectory(Constants.PHYSICALDIRPATH_TEMPDIR);
			}
		}

		/// <summary>
		/// バイトで切り詰める
		/// </summary>
		/// <param name="strSrc"></param>
		/// <param name="iByteMax"></param>
		/// <returns></returns>
		public static string TrimByBytes(string strSrc, int iByteMax)
		{
			return TrimByBytes(strSrc, iByteMax, Encoding.GetEncoding("Shift_JIS"));
		}
		/// <summary>
		/// バイトで切り詰める
		/// </summary>
		/// <param name="strSrc"></param>
		/// <param name="iByteMax"></param>
		/// <param name="eEncoding"></param>
		/// <returns></returns>
		public static string TrimByBytes(string strSrc, int iByteMax, Encoding eEncoding)
		{
			// 元々小さい場合はそのまま返す
			if (eEncoding.GetBytes(strSrc).Length <= iByteMax)
			{
				return strSrc;
			}

			// 1文字ずつ追加していき調べていく
			StringBuilder sbResult = new StringBuilder();
			for (int iIndex = 0; iIndex < strSrc.Length; iIndex++)
			{
				if (eEncoding.GetBytes(sbResult.ToString()).Length > iByteMax)
				{
					break;
				}
				sbResult.Append(strSrc[iIndex]);
			}

			return sbResult.ToString();
		}
	}
}
