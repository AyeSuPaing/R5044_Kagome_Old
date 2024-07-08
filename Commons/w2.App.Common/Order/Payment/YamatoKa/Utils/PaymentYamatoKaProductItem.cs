/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 商品アイテムクラス(PaymentYamatoKaProductItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 商品アイテムクラス
	/// </summary>
	public class PaymentYamatoKaProductItem
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentYamatoKaProductItem()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="itemName">購入商品名称</param>
		/// <param name="itemCount">購入商品数量</param>
		/// <param name="unitPrice">購入商品単価</param>
		/// <param name="subTotal">購入商品小計</param>
		public PaymentYamatoKaProductItem(string itemName, int? itemCount, decimal? unitPrice, decimal? subTotal)
		{
			this.ItemName = itemName;
			this.ItemCount = itemCount;
			this.UnitPrice = unitPrice;
			this.SubTotal = subTotal;
		}
		/// <summary>購入商品名称</summary>
		public string ItemName;
		/// <summary>購入商品数量</summary>
		public int? ItemCount;
		/// <summary>購入商品単価</summary>
		public decimal? UnitPrice;
		/// <summary>購入商品小計</summary>
		public decimal? SubTotal;
	}
}
