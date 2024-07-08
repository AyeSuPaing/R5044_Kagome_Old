/*
=========================================================================================================
  Module      : シリアルキー情報モデル (SerialKeyModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.SerialKey
{
	/// <summary>
	/// シリアルキー情報モデル
	/// </summary>
	public partial class SerialKeyModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>商品名</summary>
		public string ProductName
		{
			get { return (string)this.DataSource["product_name"]; }
			set { this.DataSource["product_name"] = value; }
		}
		/// <summary>ダウンロードURL</summary>
		public string DownloadUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DOWNLOAD_URL]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DOWNLOAD_URL] = value; }
		}
		/// <summary>商品IDを含まないバリエーションID</summary>
		public string VId
		{
			get { return this.VariationId.Substring(this.ProductId.Length); }
		}
		#endregion
	}
}
