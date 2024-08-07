/*
=========================================================================================================
  Module      : 決済手数料マスタモデル (PaymentPriceModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Payment
{
	/// <summary>
	/// 決済手数料マスタモデル
	/// </summary>
	[Serializable]
	public partial class PaymentPriceModel : ModelBase<PaymentPriceModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PaymentPriceModel()
		{
			this.PaymentGroupId = "";
			this.PaymentPriceNo = 1;
			this.TgtPriceBgn = 0;
			this.TgtPriceEnd = 0;
			this.PaymentPrice = 0;
			this.DelFlg = "0";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PaymentPriceModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PaymentPriceModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENTPRICE_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_SHOP_ID] = value; }
		}
		/// <summary>決済種別グループID</summary>
		public string PaymentGroupId
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENTPRICE_PAYMENT_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_PAYMENT_GROUP_ID] = value; }
		}
		/// <summary>決済種別ID</summary>
		public string PaymentId
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENTPRICE_PAYMENT_ID]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_PAYMENT_ID] = value; }
		}
		/// <summary>枝番</summary>
		public int PaymentPriceNo
		{
			get { return (int)this.DataSource[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO] = value; }
		}
		/// <summary>対象購入金額（以上）</summary>
		public decimal TgtPriceBgn
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PAYMENTPRICE_TGT_PRICE_BGN]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_TGT_PRICE_BGN] = value; }
		}
		/// <summary>対象購入金額（以下）</summary>
		public decimal TgtPriceEnd
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END] = value; }
		}
		/// <summary>決済手数料</summary>
		public decimal PaymentPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENTPRICE_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PAYMENTPRICE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PAYMENTPRICE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENTPRICE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PAYMENTPRICE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
