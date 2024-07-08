/*
=========================================================================================================
  Module      : SubscriptionBox Default Item Model (SubscriptionBoxDefaultItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;

namespace w2.Domain.SubscriptionBox
{
	/// <summary>
	/// Extends of SubscriptionBoxDefaultItemModel
	/// </summary>
	public partial class SubscriptionBoxDefaultItemModel
	{
		#region メソッド
		/// <summary>
		/// 基準日時が期間の範囲内か
		/// </summary>
		/// <param name="criteria">基準日時</param>
		/// <returns>True:基準日時が期間の範囲内、False:基準日時が期間の範囲外</returns>
		public bool IsInTerm(DateTime criteria)
		{
			var isInTermSince = ((this.TermSince.HasValue == false) || (this.TermSince.Value <= criteria));
			var isInTermUntil = ((this.TermUntil.HasValue == false) || (criteria <= this.TermUntil.Value));
			var result = (isInTermSince && isInTermUntil);
			return result;
		}

		/// <summary>
		/// 基準日時が期間終了日を過ぎているか
		/// </summary>
		/// <param name="criteria">基準日時</param>
		/// <returns>True:基準日時が期間終了日を過ぎていない、False:基準日時が期間終了日を過ぎている</returns>
		public bool IsInTermUntil(DateTime criteria)
		{
			var result = (this.TermUntil.HasValue == false) || (criteria <= this.TermUntil.Value);
			return result;
		}

		/// <summary>
		/// 期間の時間を補正
		/// </summary>
		public void CorrectTerm()
		{
			if (this.TermSince.HasValue)
			{
				this.TermSince = new DateTime(
					this.TermSince.Value.Year,
					this.TermSince.Value.Month,
					this.TermSince.Value.Day);
			}

			if (this.TermUntil.HasValue)
			{
				this.TermUntil = new DateTime(
					this.TermUntil.Value.Year,
					this.TermUntil.Value.Month,
					this.TermUntil.Value.Day,
					23,
					59,
					59,
					997);
			}
		}

		/// <summary>
		/// 選択可能時間内かを判定
		/// </summary>
		/// <param name="criteria">基準日時</param>
		/// <returns>選択可能時間内かどうか</returns>
		public bool IsInSelectableTerm(DateTime criteria)
		{
			var isInSelectableSince = ((this.SelectableSince.HasValue == false) || (this.SelectableSince.Value <= criteria));
			var isInSelectableUntil = ((this.SelectableUntil.HasValue == false) || (this.SelectableUntil.Value >= criteria));
			var result = (isInSelectableSince && isInSelectableUntil);
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>商品名</summary>
		public string Name
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_NAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		}
		/// <summary>バリエーション名1</summary>
		public string VariationName1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] = value; }
		}
		/// <summary>バリエーション名2</summary>
		public string VariationName2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] = value; }
		}
		/// <summary>バリエーション名3</summary>
		public string VariationName3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3] = value; }
		}
		/// <summary>単価</summary>
		public decimal? Price
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE] = value; }
		}
		/// <summary>引継ぎ商品の枝番</summary>
		public int TakeOverProductBranchNo
		{
			get { return (int)this.DataSource["TakeOverProductBranchNo"]; }
			set { this.DataSource["TakeOverProductBranchNo"] = value; }
		}
		/// <summary>必須商品か</summary>
		public bool IsNecessary
		{
			get { return ((string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_NECESSARY_PRODUCT_FLG] == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID); }
		}
		/// <summary>選択可能期間開始</summary>
		public DateTime? SelectableSince
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_SINCE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_SINCE];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_SINCE] = value; }
		}
		/// <summary>選択可能期間終了</summary>
		public DateTime? SelectableUntil
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_UNTIL] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_UNTIL];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_UNTIL] = value; }
		}
		/// <summary>配送種別</summary>
		public string ShippingId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_ID] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_ID];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_ID] = value; }
		}
		#endregion
	}
}
