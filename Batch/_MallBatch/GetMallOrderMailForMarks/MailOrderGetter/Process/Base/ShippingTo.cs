/*
=========================================================================================================
  Module      : 商品送付先情報保持クラス／注文データ抽象定義 (ShippingTo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Base
{
	///**************************************************************************************
	/// <summary>
	/// 商品送付先情報保持クラス
	/// </summary>
	///**************************************************************************************
	public abstract class ShippingTo : BaseProcess
	{
		/// <summary>送付先名（姓）</summary>
		public string ShippingToName1 { get; protected set; }
		/// <summary>送付先名（名）</summary>
		public string ShippingToName2 { get; protected set; }
		/// <summary>送付先名かな（姓）</summary>
		public string ShippingToNameKana1 { get; protected set; }
		/// <summary>送付先名かな（名）</summary>
		public string ShippingToNameKana2 { get; protected set; }
		/// <summary>郵便番号</summary>
		public string Zip { get; protected set; }
		/// <summary>住所１</summary>
		public string Addr1 { get; protected set; }
		/// <summary>住所２</summary>
		public string Addr2 { get; protected set; }
		/// <summary>住所３</summary>
		public string Addr3 { get; protected set; }
		/// <summary>住所４</summary>
		public string Addr4 { get; protected set; }
		/// <summary>電話番号</summary>
		public string Phone { get; protected set; }
		/// <summary>のし</summary>
		public string WrappingPaperType { get; protected set; }
		/// <summary>Is rakuten oversea order address</summary>
		public bool IsOverseaAddress { get; protected set; }
	}
}
