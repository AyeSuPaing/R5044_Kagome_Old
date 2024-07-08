/*
=========================================================================================================
  Module      : 定期購入履歴情報モデル (FixedPurchaseHistoryModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入履歴情報モデル
	/// </summary>
	public partial class FixedPurchaseHistoryModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>
		/// 拡張項目_定期購入履歴区分テキスト
		/// </summary>
		public string FixedPurchaseHistoryKbnText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASEHISTORY, Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN, this.FixedPurchaseHistoryKbn);
			}
		}
		/// <summary>
		/// 拡張項目_配送希望日
		/// </summary>
		public DateTime? ShippingDate
		{
			get 
			{
				if (this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE]; 
			}
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE] = value; }
		}
		#endregion
	}
}