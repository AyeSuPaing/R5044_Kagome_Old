/*
=========================================================================================================
  Module      : エンコードヘルパークラス(EncodingHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Text;
using w2.App.Common;

namespace ExternalAPI.Helper
{
	public class EncodingHelper
	{
		/// <summary>
		/// エンコード取得（utf-8nが指定された時はutf-8 BOMなしにする）
		/// </summary>
		/// <param name="encodingType">エンコードタイプ</param>
		/// <returns>エンコード</returns>
		public static Encoding GetEncoding(string encodingType)
		{
			var encoding = encodingType.ToLower() == Constants.CONST_FREEEXPORT_ENCODING_UTF8N
				? new UTF8Encoding(false)
				: Encoding.GetEncoding(encodingType);

			return encoding;
		}
	}
}
