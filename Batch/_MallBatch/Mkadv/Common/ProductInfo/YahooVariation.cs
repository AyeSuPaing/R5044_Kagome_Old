/*
=========================================================================================================
  Module      : ヤフーバリエーションクラス(YahooVariation.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.MallBatch.Mkadv.Common.ProductInfo
{
	///**************************************************************************************
	/// <summary>
	/// ヤフーバリエーションクラス
	/// </summary>
	///**************************************************************************************
	public class YahooVariation
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public YahooVariation()
		{
			// プロパティ初期化
			this.VariationNames = new Dictionary<string, List<string>>();
		}

		/// <summary>
		/// オプション設定
		/// </summary>
		/// <param name="strKey">キー</param>
		/// <param name="strValue">値</param>
		public void SetOptions(string strKey, string strValue)
		{
			// YahoonoのOptionsは半角スペース区切りの為、値に含まれる半角スペースは削除する
			strValue = strValue.Replace(" ", ""); 
			if (this.VariationNames.ContainsKey(strKey))
			{
				if (this.VariationNames[strKey].IndexOf(strValue) == -1)
				{
					this.VariationNames[strKey].Add(strValue);
				}
			}
			else
			{
				this.VariationNames.Add(strKey, new List<string>());
				this.VariationNames[strKey].Add(strValue);
			}

			// プロパティへセットする
			this.Options = GetOptions();
		}

		/// <summary>
		/// オプション取得
		/// </summary>
		/// <returns>文字列</returns>
		public string GetOptions()
		{
			StringBuilder sbResult = new StringBuilder();
			string strTmpKey = "";
			foreach (string strKey in this.VariationNames.Keys)
			{
				sbResult.Append((strTmpKey == "") ? "" : "\r\n\r\n").Append(strKey);
				strTmpKey = strKey;

				foreach (string strValue in this.VariationNames[strKey])
				{
					sbResult.Append(" ").Append(strValue);
				}
			}

			return sbResult.ToString();
		}

		/// <summary>ヤフーサブコード</summary>
		public string SubCode { get; set; }

		/// <summary>ヤフーオプション</summary>
		public string Options { get; set; }

		/// <summary>バリエーション名</summary>
		private Dictionary<string, List<string>> VariationNames { get; set; }
	}
}
