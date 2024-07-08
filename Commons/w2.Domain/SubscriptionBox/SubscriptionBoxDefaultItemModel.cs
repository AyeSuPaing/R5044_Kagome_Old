/*
=========================================================================================================
  Module      : SubscriptionBox Default Item Model (SubscriptionBoxDefaultItemModel.cs)
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
	/// Subscription Box Default Item Model
	/// </summary>
	[Serializable]
	public partial class SubscriptionBoxDefaultItemModel : ModelBase<SubscriptionBoxDefaultItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SubscriptionBoxDefaultItemModel()
		{
			this.SubscriptionBoxCourseId = string.Empty;
			this.Count = null;
			this.TermSince = null;
			this.TermUntil = null;
			this.ShopId = string.Empty;
			this.ProductId = string.Empty;
			this.ItemQuantity = 0;
			this.VariationId = string.Empty;
			this.BranchNo = 0;
			this.NecessaryProductFlg = Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SubscriptionBoxDefaultItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SubscriptionBoxDefaultItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>枝番</summary>
		public int BranchNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_BRANCH_NO] = value; }
		}
		/// <summary>回数</summary>
		public int? Count
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_COUNT];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_COUNT] = value; }
		}
		/// <summary>期間開始</summary>
		public DateTime? TermSince
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_SINCE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_SINCE];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_SINCE] = value; }
		}
		/// <summary>期間終了</summary>
		public DateTime? TermUntil
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_UNTIL] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_UNTIL];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_UNTIL] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SHOP_ID] == DBNull.Value) return string.Empty;
				return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SHOP_ID];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_PRODUCT_ID] == DBNull.Value) return string.Empty;
				return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_PRODUCT_ID];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_PRODUCT_ID] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_VARIATION_ID] == DBNull.Value) return string.Empty;
				return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_VARIATION_ID];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_VARIATION_ID] = value; }
		}
		/// <summary>数量</summary>
		public int ItemQuantity
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY] == DBNull.Value) return 0;
				return (int)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY];
			}
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY] = value; }
		}
		/// <summary>必須商品フラグ</summary>
		public string NecessaryProductFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_NECESSARY_PRODUCT_FLG]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_NECESSARY_PRODUCT_FLG] = value; }
		}
		#endregion
	}
}
