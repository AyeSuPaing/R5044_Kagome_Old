/*
=========================================================================================================
  Module      : 注文商品情報保持クラス／注文データ抽象定義 (OrderItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Base
{
	///**************************************************************************************
	/// <summary>
	/// 注文商品情報保持クラス
	/// </summary>
	///**************************************************************************************
	public abstract class OrderItem : BaseProcess
	{
		/// <summary>商品名</summary>
		public string ProductName { get; protected set; }

		/// <summary>商品ID</summary>
		public string ProductId { get; protected set; }

		/// <summary>バリエーションID</summary>
		public string VariationId { get; protected set; }

		/// <summary>単価</summary>
		public decimal Price { get; protected set; }

		/// <summary>単価消費税</summary>
		public decimal PriceTax { get; protected set; }

		/// <summary>税率</summary>
		public decimal TaxRate { get; protected set; }

		/// <summary>個数</summary>
		public decimal Quantity { get; protected set; }

		/// <summary>小計</summary>
		public decimal Subtotal { get; protected set; }

		/// <summary>小計消費税</summary>
		public decimal TaxSubtotal { get; protected set; }

		/// <summary>税込みフラグ（税込み時true）</summary>
		public bool TaxFlag { get; protected set; }

		/// <summary>送料込みフラグ（送料込み時true）</summary>
		public bool ShippingFlag { get; protected set; }
	}
}
