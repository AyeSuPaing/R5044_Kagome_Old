/*
=========================================================================================================
  Module      : Amazon住所モデル(AmazonAddressModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Order;

namespace w2.App.Common.Amazon.Helper
{
	/// <summary>
	/// Amazon住所モデル
	/// </summary>
	[Serializable]
	public class AmazonAddressModel
	{
		#region プロパティ
		/// <summary>姓名</summary>
		public string Name { get; set; }
		/// <summary>姓</summary>
		public string Name1 { get; set; }
		/// <summary>名</summary>
		public string Name2 { get; set; }
		/// <summary>姓名（かな</summary>
		public string NameKana { get; set; }
		/// <summary>姓（かな）</summary>
		public string NameKana1 { get; set; }
		/// <summary>名（かな）</summary>
		public string NameKana2 { get; set; }
		/// <summary>メールアドレス</summary>
		public string MailAddr { get; set; }
		/// <summary>メールアドレス2</summary>
		public string MailAddr2 { get; set; }
		/// <summary>郵便番号</summary>
		public string Zip { get; set; }
		/// <summary>郵便番号1</summary>
		public string Zip1 { get; set; }
		/// <summary>郵便番号2</summary>
		public string Zip2 { get; set; }
		/// <summary>住所</summary>
		public string Addr { get; set; }
		/// <summary>住所1</summary>
		public string Addr1 { get; set; }
		/// <summary>住所2</summary>
		public string Addr2 { get; set; }
		/// <summary>住所3</summary>
		public string Addr3 { get; set; }
		/// <summary>住所4</summary>
		public string Addr4 { get; set; }
		/// <summary>国名コード</summary>
		public string CountryCode { get; set; }
		/// <summary>電話番号</summary>
		public string Tel { get; set; }
		/// <summary>電話番号1</summary>
		public string Tel1 { get; set; }
		/// <summary>電話番号2</summary>
		public string Tel2 { get; set; }
		/// <summary>電話番号3</summary>
		public string Tel3 { get; set; }
		#endregion
	}
}