/*
=========================================================================================================
  Module      : 定期購入履歴一覧検索のためのヘルパクラス (FixedPurchaseHistoryListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Common.Sql;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.FixedPurchase.Helper
{
	#region +定期購入履歴一覧検索条件クラス
	/// <summary>
	/// 定期購入履歴一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class FixedPurchaseHistoryListSearchCondition : BaseDbMapModel
	{
		/// <summary>定期購入ID</summary>
		[DbMapName("fixed_purchase_id")]
		public string FixedPurchaseId { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int? BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int? EndRowNumber { get; set; }
	}
	#endregion

	#region +定期購入履歴情報一覧結果クラス
	/// <summary>
	/// 定期購入履歴情報一覧結果クラス
	/// ※FixedPurchaseHistoryModelを拡張
	/// </summary>
	[Serializable]
	public class FixedPurchaseHistoryListSearchResult : FixedPurchaseHistoryModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseHistoryListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>支払金額合計</summary>
		public decimal? OrderPriceTotal
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL];
			}
		}
		/// <summary>支払金額合計表示用</summary>
		public string OrderPriceTotalDisplay
		{
			get
			{
				// 定期購入履歴区分が「定期購入成功」の場合、支払金額合計を返す
				return (this.FixedPurchaseHistoryKbn == Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS)
					? StringUtility.ToPrice(this.OrderPriceTotal) : "-";
			}
		}
		/// <summary>
		/// 購入回数(注文基準)更新表示用
		/// </summary>
		public string UpdateOrderCountDisplay
		{
			get
			{
				if (this.UpdateOrderCount.HasValue == false) return "-";

				// 0以上場合、頭に"+"を付ける
				return ((this.UpdateOrderCount.Value >= 0) ? "+" : "") + this.UpdateOrderCount.Value.ToString();
			}
		}
		/// <summary>
		/// 購入回数(出荷基準)更新表示用
		/// </summary>
		public string UpdateShippedCountDisplay
		{
			get
			{
				if (this.UpdateShippedCount.HasValue == false) return "-";

				// 0以上場合、頭に"+"を付ける
				return ((this.UpdateShippedCount.Value >= 0) ? "+" : "") + this.UpdateShippedCount.Value.ToString();
			}
		}
		/// <summary>
		/// 購入回数(注文基準)更新結果表示用
		/// </summary>
		public string UpdateOrderCountResultDisplay
		{
			get
			{
				if (this.UpdateOrderCountResult.HasValue == false) return "";
				return "(" + this.UpdateOrderCountResult.Value.ToString() + " 回)";
			}
		}
		/// <summary>
		/// 購入回数(出荷基準)更新結果表示用
		/// </summary>
		public string UpdateShippedCountResultDisplay
		{
			get
			{
				if (this.UpdateShippedCountResult.HasValue == false) return "";
				return "(" + this.UpdateShippedCountResult.Value.ToString() + " 回)";
			}
		}
		/// <summary>
		/// 返品済みか
		/// </summary>
		public bool IsReturned
		{
			get { return (this.FixedPurchaseHistoryKbn == Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_RETURN); }
		}
		#endregion
	}
	#endregion
}
