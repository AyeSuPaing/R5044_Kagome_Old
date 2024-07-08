/*
=========================================================================================================
  Module      : ユーザー履歴（注文）クラス(UserHistoryOrder.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w2.Common.Util;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	/// ユーザー履歴（注文）クラス
	/// </summary>
	[Serializable]
	public class UserHistoryOrder : UserHistoryBase
	{
		static string KBN_STRING = "注文";
		/// <summary>Kbn order store pickup string</summary>
		public const string KBN_ORDER_STOREPICKUP_STRING = "注文(店舗受取)";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		internal UserHistoryOrder(DataRowView info)
			: base(info)
		{
			this.Items = new List<UserHistoryOrderItem>();
			this.Shippings = new List<UserHistoryOrderShipping>();
		}

		/// <summary>
		/// 情報セット
		/// </summary>
		protected override void SetInfo()
		{
			this.DateTime = this.OrderDate;

			string returnExchangeKbn = this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN].ToString();

			switch (returnExchangeKbn)
			{
				case Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN:
				case Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE:
					this.KbnString = string.Format("{0}({1})", KBN_STRING, ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN, returnExchangeKbn));
					break;

				default:
					this.KbnString = KBN_STRING;
					break;
			}

			if (Constants.STORE_PICKUP_OPTION_ENABLED
				&& (string.IsNullOrEmpty(
					StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_STATUS])) == false))
			{
				this.KbnString = KBN_ORDER_STOREPICKUP_STRING;
			}
		}

		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDER_ORDER_ID]); }
		}
		/// <summary>注文日時</summary>
		public DateTime OrderDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDER_ORDER_DATE]; }
		}
		/// <summary>注文ステータス</summary>
		public string OrderStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDER_ORDER_STATUS]); }
		}
		/// <summary>注文入金ステータス</summary>
		public string OrderPaymentStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS]); }
		}
		/// <summary>注文アイテム数</summary>
		public int OrderItemCount
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDER_ORDER_ITEM_COUNT]; }
		}
		/// <summary>支払金額合計</summary>
		public decimal OrderPriceTotal
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL]; }
		}
		/// <summary>子要素（商品が複数あったとき用）</summary>
		public List<UserHistoryOrderItem> Items { get; private set; }
		/// <summary>子要素（配送先情報が複数あったとき用）</summary>
		public List<UserHistoryOrderShipping> Shippings { get; private set; }
	}
}