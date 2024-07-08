/*
=========================================================================================================
  Module      : データ入力ユーティリティモジュール(DataInputUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.App.Common.Util
{
	/// <summary>
	/// データ入力ユーティリティモジュール
	/// </summary>
	public class DataInputUtility
	{
		/// <summary>
		/// 設定を元に全角変換
		/// </summary>
		/// <param name="obj">変換対象オブジェクト</param>
		/// <param name="isJpAddress">日本住所か</param>
		/// <returns>変換後の値</returns>
		public static string ConvertToFullWidthBySetting(object obj, bool isJpAddress = true)
		{
			if (Constants.HALFWIDTH_CHARACTER_INPUTABLE && (isJpAddress == false))
			{
				return StringUtility.ToEmpty(obj);
			}
			return StringUtility.ToZenkaku(obj);
		}

		/// <summary>
		/// 設定を元に全角かな/カナ変換
		/// </summary>
		/// <param name="obj">変換対象オブジェクト</param>
		/// <param name="isJpAddress">日本住所か</param>
		/// <returns>変換後の値</returns>
		public static string ConvertToFullWidthKanaBySetting(object obj, bool isJpAddress = true)
		{
			if (Constants.HALFWIDTH_CHARACTER_INPUTABLE && (isJpAddress == false))
			{
				return StringUtility.ToEmpty(obj);
			}
			var type = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana.type@@");
			switch (type)
			{
				case Validator.STRTYPE_FULLWIDTH_HIRAGANA:
					return StringUtility.ToZenkakuHiragana(obj);
				case Validator.STRTYPE_FULLWIDTH_KATAKANA:
					return StringUtility.ToZenkakuKatakana(obj);
			}
			return StringUtility.ToZenkaku(obj);
		}
	}
}
