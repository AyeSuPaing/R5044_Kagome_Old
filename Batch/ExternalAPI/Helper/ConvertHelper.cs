/*
=========================================================================================================
  Module      : コンバートヘルパークラス(EncodingHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using ExternalAPI.FreeExport;
using System;
using w2.Common.Util;

namespace ExternalAPI.Helper
{
	public class ConvertHelper
	{
		/// <summary>
		/// DBデータ文字列を文字列変換タイプに応じて変換
		/// </summary>
		/// <param name="data">SQLステートメントデータリーダー 1レコード分のDBデータ内容</param>
		/// <param name="convertType">文字列変換タイプ</param>
		/// <returns>変換後の文字列</returns>
		public static string ApplyConvertByConvertType(string data, ConvertString convertType)
		{
			switch (convertType)
			{
				case ConvertString.None:
					break;

				case ConvertString.ToHankaku:
					data = StringUtility.ToHankaku(data);
					break;

				case ConvertString.ToHankakuKatakana:
					data = StringUtility.ToHankakuKatakana(data);
					break;

				case ConvertString.ToZenkaku:
					data = StringUtility.ToZenkaku(data);
					break;

				case ConvertString.ToZenkakuHiragana:
					data = StringUtility.ToZenkakuHiragana(data);
					break;

				case ConvertString.ToZenkakuKatakana:
					data = StringUtility.ToZenkakuKatakana(data);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(convertType), convertType, null);
			}

			return data;
		}
	}
}
