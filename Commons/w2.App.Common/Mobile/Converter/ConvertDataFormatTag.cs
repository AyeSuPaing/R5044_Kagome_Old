/*
=========================================================================================================
  Module      : データフォーマットタグ変換クラス(DataFormatTag.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using w2.Common.Util;

namespace w2.App.Common.Mobile.Converter
{
	public class ConvertDataFormatTag : ConvertDataTagBase
	{
		#region 定数
		// データフォーマットタグ
		private const string TAG_HEAD = "<@@data_format:";
		private const string TAG_FOOT = "@@>";
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="html">変換対象文字列</param>
		/// <param name="data">データ</param>
		public ConvertDataFormatTag(StringBuilder html, Hashtable data)
			: base(html, data)
		{
			// 何もしない
		}

		/// <summary>
		/// データフォーマットタグ変換
		/// </summary>
		protected override void ConvertTag()
		{
			foreach (Match m in GetTagMatches(TAG_HEAD, TAG_FOOT))
			{
				// データフォーマットタグ取得
				string inner = GetTagInner(m.Value, TAG_HEAD, TAG_FOOT);
				string[] inners = inner.Split(':');
				string replace = "";
				if (inners.Length >= 2)
				{
					string key = inners[0];
					string format = inner.Substring((key + ":").Length);
					string value = StringUtility.ToEmpty(this.Data[key]);

					// 指定あり？
					if ((key != "")
						&& (format != "")
						&& (value != ""))
					{
						try
						{
							DateTime dateValue;
							decimal decimalValue;
							int intValue;
							// datetime変換可？
							if (DateTime.TryParse(value, out dateValue))
							{
								replace = dateValue.ToString(format);
							}
							// decimal変換可？
							else if (decimal.TryParse(value, out decimalValue))
							{
								replace = string.Format(format, decimalValue);
							}
							// int変換可？
							else if (int.TryParse(value, out intValue))
							{
								replace = string.Format(format, intValue);
							}
						}
						catch (Exception ex)
						{
							w2.Common.Logger.AppLogger.WriteError("データフォーマットタグ変換エラー：タグ「" + m.Value + "」", ex);
						}
					}
				}
				this.Html.Replace(m.Value, replace);
			}
		}
	}
}
