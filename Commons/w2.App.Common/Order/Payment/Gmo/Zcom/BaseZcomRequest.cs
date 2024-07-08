/*
=========================================================================================================
  Module      : Zcom基底リクエストクラス (BaseZcomRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.GMO.Zcom
{
	/// <summary>
	/// Zcom基底リクエストクラス
	/// </summary>
	public abstract class BaseZcomRequest : IHttpApiRequestData
	{
		/// <summary>保持データ</summary>
		protected IDictionary<string, string> m_data = new Dictionary<string, string>();

		/// <summary>
		/// マスク文字取得
		/// </summary>
		/// <returns>マスク文字</returns>
		protected virtual char GetMaskCharacter()
		{
			return '*';
		}

		/// <summary>
		/// マスクしたいキー取得
		/// </summary>
		/// <returns>マスクしたいデータのキー</returns>
		protected virtual string[] GetMaskedKeys()
		{
			return new[] { "card_number", "security_code" };
		}

		/// <summary>
		/// リクエスト値取得
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> GetRequestData()
		{
			return m_data;
		}

		/// <summary>
		/// POST用文字列生
		/// </summary>
		/// <returns>POST用の文字列</returns>
		public string CreatePostString()
		{
			return string.Join("&", this.m_data.Select(kvp => string.Concat(kvp.Key, "=", HttpUtility.UrlEncode(kvp.Value))).ToArray());
		}

		/// <summary>
		/// マスクしたPOST文字列生成
		/// </summary>
		/// <returns>マスクしたPOST用の文字列</returns>
		public string CreateMaskedPostString()
		{
			// マスク対象のキーを持つものはマスクして返す
			return string.Join("&", this.m_data.Select(kvp => string.Concat(kvp.Key, "=", HttpUtility.UrlEncode(GetMaskedValue(kvp)))).ToArray());
		}

		/// <summary>
		/// マスクした値を取得
		/// </summary>
		/// <param name="kvp">マスクしたいデータ</param>
		/// <returns>マスクしたいキーのもののみマスクした値、マスク対象外の場合は値そのまま</returns>
		private string GetMaskedValue(KeyValuePair<string, string> kvp)
		{
			if (this.GetMaskedKeys().Contains(kvp.Key))
			{
				return new String(this.GetMaskCharacter(), kvp.Value.Length);
			}
			return kvp.Value;
		}
	}
}
