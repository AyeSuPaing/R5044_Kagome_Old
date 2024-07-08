/*
=========================================================================================================
  Module      : 商品見つからない例外(NoMatchItemException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Import
{
	/// <summary>
	/// 商品見つからない例外
	/// </summary>
	public class NoMatchItemException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private NoMatchItemException()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">エラーメッセージ</param>
		/// <param name="innerException">内包例外</param>
		private NoMatchItemException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="productId">見つからなかった商品のID</param>
		public NoMatchItemException(string productId)
			: base(string.Format("商品ID'{0}'は存在しません。", productId))
		{
			this.NoMatchProductId = productId;
		}

		/// <summary>見つからなかった商品のID</summary>
		public string NoMatchProductId { get; private set; }
	}
}
