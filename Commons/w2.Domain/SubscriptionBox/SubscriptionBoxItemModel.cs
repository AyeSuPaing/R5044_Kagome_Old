/*
=========================================================================================================
  Module      : SubscriptionBox Item Model (SubscriptionBoxItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.SubscriptionBox
{
	/// <summary>
	/// Subscription Box Item Model
	/// </summary>
	[Serializable]
	public partial class SubscriptionBoxItemModel : ModelBase<SubscriptionBoxItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SubscriptionBoxItemModel()
		{
			this.SubscriptionBoxCourseId = string.Empty;
			this.BranchNo = 0;
			this.ProductId = string.Empty;
			this.ShopId = string.Empty;
			this.VariationId = string.Empty;
			this.SelectableSince = DateTime.MinValue;
			this.SelectableUntil = DateTime.MinValue;
			this.ShippingType = string.Empty;
			this.Name = string.Empty;
			this.VariationName1 = string.Empty;
			this.VariationName2 = string.Empty;
			this.VariationName3 = string.Empty;
			this.CampaignPrice = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SubscriptionBoxItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SubscriptionBoxItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 基準日時が期間の範囲内か
		/// </summary>
		/// <param name="criteria">基準日時</param>
		/// <returns>True:基準日時が期間の範囲内、False:基準日時が期間の範囲外</returns>
		public bool IsInTerm(DateTime criteria)
		{
			var isInTermSince = ((this.SelectableSince.HasValue == false) || (this.SelectableSince.Value <= criteria));
			var isInTermUntil = ((this.SelectableUntil.HasValue == false) || (criteria <= this.SelectableUntil.Value));
			var result = (isInTermSince && isInTermUntil);
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>枝番</summary>
		public int BranchNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_BRANCH_NO] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_PRODUCT_ID] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_VARIATION_ID] = value; }
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
		/// <summary>キャンペーン期間開始</summary>
		public DateTime? CampaignSince
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_SINCE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_SINCE];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_SINCE] = value; }
		}
		/// <summary>キャンペーン期間終了</summary>
		public DateTime? CampaignUntil
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_UNTIL] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_UNTIL];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_UNTIL] = value; }
		}
		/// <summary>キャンペーン期間価格</summary>
		public decimal? CampaignPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_PRICE];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_PRICE] = value; }
		}
		#endregion
	}
}
