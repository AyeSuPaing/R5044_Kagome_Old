/*
=========================================================================================================
  Module      : SubscriptionBox Model (SubscriptionBoxModel.cs)
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
	/// Subscription Box Model
	/// </summary>
	[Serializable]
	public partial class SubscriptionBoxModel : ModelBase<SubscriptionBoxModel>
	{
		/// <summary>日付フォーマット</summary>
		public const string DATE_FORMAT = "yyyy-MM-dd";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SubscriptionBoxModel()
		{
			this.CourseId = string.Empty;
			this.ManagementName = string.Empty;
			this.DisplayName = string.Empty;
			this.MinimumPurchaseQuantity = null;
			this.MaximumPurchaseQuantity = null;
			this.MinimumNumberOfProducts = null;
			this.MaximumNumberOfProducts = null;
			this.AutoRenewal = Constants.FLG_SUBSCRIPTIONBOX_AUTO_RENEWAL_FALSE;
			this.ItemsChangeableByUser = Constants.FLG_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER_FALSE;
			this.OrderItemDeterminationType = Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME;
			this.ValidFlg = Constants.FLG_SUBSCRIPTIONBOX_VALID_TRUE;
			this.LastChanged = string.Empty;
			this.DisplayPriority = 0;
			this.DateChanged = null;
			this.DateCreated = null;
			this.FixedAmountFlg = Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_FALSE;
			this.FixedAmount = null;
			this.TaxCategoryId = string.Empty;
			this.SelectableProducts = new SubscriptionBoxItemModel[0];
			this.DefaultOrderProducts = new SubscriptionBoxDefaultItemModel[0];
			this.MinimumAmount = null;
			this.MaximumAmount = null;
			this.FirstSelectableFlg = Constants.FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_FALSE;
			this.IndefinitePeriod = Constants.FLG_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_FALSE;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SubscriptionBoxModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SubscriptionBoxModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>頒布会コースID</summary>
		public string CourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID];  }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>頒布会コース管理名</summary>
		public string ManagementName
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MANAGEMENT_NAME]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MANAGEMENT_NAME] = value; }
		}
		/// <summary>頒布会コース表示名</summary>
		public string DisplayName
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_NAME]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_NAME] = value; }
		}
		/// <summary>自動繰り返し設定</summary>
		public string AutoRenewal
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_AUTO_RENEWAL]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_AUTO_RENEWAL] = value; }
		}
		/// <summary>マイページでの商品変更可否</summary>
		public string ItemsChangeableByUser
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER] = value; }
		}
		/// <summary>商品決定方法</summary>
		public string OrderItemDeterminationType
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE] = value; }
		}
		/// <summary>最低購入数量</summary>
		public int? MinimumPurchaseQuantity
		{
			get 
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_PURCHASE_QUANTITY] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_PURCHASE_QUANTITY];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_PURCHASE_QUANTITY] = value; }
		}
		/// <summary>最大購入数量</summary>
		public int? MaximumPurchaseQuantity
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_PURCHASE_QUANTITY] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_PURCHASE_QUANTITY];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_PURCHASE_QUANTITY] = value; }
		}
		/// <summary>最低購入数量</summary>
		public int? MinimumNumberOfProducts
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_NUMBER_OF_PRODUCTS] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_NUMBER_OF_PRODUCTS];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_NUMBER_OF_PRODUCTS] = value; }
		}
		/// <summary>最大購入数量</summary>
		public int? MaximumNumberOfProducts
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_NUMBER_OF_PRODUCTS] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_NUMBER_OF_PRODUCTS];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_NUMBER_OF_PRODUCTS] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime? DateCreated
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DATE_CREATED] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DATE_CREATED];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime? DateChanged
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DATE_CHANGED] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DATE_CHANGED];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_LAST_CHANGED] = value; }
		}
		/// <summary>定額設定</summary>
		public string FixedAmountFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT_FLG]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT_FLG] = value; }
		}
		/// <summary>定額価格</summary>
		public decimal? FixedAmount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT] = value; }
		}
		/// <summary>税率カテゴリID</summary>
		public string TaxCategoryId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_TAX_CATEGORY_ID] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_TAX_CATEGORY_ID];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_TAX_CATEGORY_ID] = value; }
		}
		/// <summary>表示優先順</summary>
		public int DisplayPriority
		{
			get { return (int)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_PRIORITY]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_PRIORITY] = value; }
		}
		/// <summary>商品合計金額下限（税込）</summary>
		public decimal? MinimumAmount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_AMOUNT] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_AMOUNT];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MINIMUM_AMOUNT] = value; }
		}
		/// <summary>商品合計金額上限（税込）</summary>
		public decimal? MaximumAmount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_AMOUNT] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_AMOUNT];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_MAXIMUM_AMOUNT] = value; }
		}
		/// <summary>頒布会初回選択画面フラグ</summary>
		public string FirstSelectableFlg
		{
			get{ return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIRST_SELECTABLE_FLG]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_FIRST_SELECTABLE_FLG] = value; }
		}
		/// <summary>無期限設定フラグ</summary>
		public string IndefinitePeriod
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_FLG]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_FLG] = value; }
		}
		#endregion
	}
}
