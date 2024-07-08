/*
=========================================================================================================
  Module      : データタグファクトリ変換クラス(ConvertDataTagFactory.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Text;
using w2.Common;

namespace w2.App.Common.Mobile.Converter
{
	public static class ConvertDataTagFactory
	{
		/// <summary>
		/// ConvertDataTagBaseのインスタンスを生成
		/// </summary>
		/// <param name="className">クラス名</param>
		/// <summary>HTML文字列</summary>
		/// <param name="data">データ</param>
		/// <returns>ConvertDataTagBaseのインスタンス</returns>
		public static ConvertDataTagBase CreateInstance(string className, StringBuilder html,  Hashtable data)
		{	
			switch (className)
			{
				// データフォーマットタグ
				case "ConvertDataFormatTag":
					return new ConvertDataFormatTag(html, data);

				// 上記以外
				default:
					throw new w2Exception("インスタンス「" + className + "」は生成できません。");
			}
		}
	}
}
