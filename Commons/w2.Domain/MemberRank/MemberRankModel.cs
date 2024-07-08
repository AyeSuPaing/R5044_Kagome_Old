/*
=========================================================================================================
  Module      : 会員ランクマスタモデル (MemberRankModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MemberRank
{
	/// <summary>
	/// 会員ランクマスタモデル
	/// </summary>
	[Serializable]
	public partial class MemberRankModel : ModelBase<MemberRankModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MemberRankModel()
		{
			this.OrderDiscountType = Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_NONE;
			this.OrderDiscountValue = null;
			this.OrderDiscountThresholdPrice = null;
			this.PointAddType = Constants.FLG_MEMBERRANK_POINT_ADD_TYPE_NONE;
			this.PointAddValue = null;
			this.ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE;
			this.ShippingDiscountValue = null;
			this.DefaultMemberRankSettingFlg = Constants.FLG_MEMBERRANK_DEFAULT_MEMBER_RANK_SETTING_FLG_OFF;
			this.ValidFlg = Constants.FLG_MEMBERRANK_VALID_FLG_VALID;
			this.FixedPurchaseDiscountRate = 0m;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MemberRankModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MemberRankModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ランクID</summary>
		public string MemberRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID] = value; }
		}
		/// <summary>ランク順位</summary>
		public int MemberRankOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ORDER]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ORDER] = value; }
		}
		/// <summary>ランク名</summary>
		public string MemberRankName
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME] = value; }
		}
		/// <summary>注文割引方法</summary>
		public string OrderDiscountType
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE] = value; }
		}
		/// <summary>注文割引数</summary>
		public decimal? OrderDiscountValue
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_VALUE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_VALUE];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_VALUE] = value; }
		}
		/// <summary>注文金額割引き閾値</summary>
		public decimal? OrderDiscountThresholdPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_THRESHOLD_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_THRESHOLD_PRICE];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_THRESHOLD_PRICE] = value; }
		}
		/// <summary>ポイント加算方法</summary>
		public string PointAddType
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE] = value; }
		}
		/// <summary>ポイント加算数</summary>
		public decimal? PointAddValue
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANK_POINT_ADD_VALUE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_MEMBERRANK_POINT_ADD_VALUE];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANK_POINT_ADD_VALUE] = value; }
		}
		/// <summary>配送料割引方法</summary>
		public string ShippingDiscountType
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE] = value; }
		}
		/// <summary>配送料割引数</summary>
		public decimal? ShippingDiscountValue
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_VALUE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_VALUE];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_VALUE] = value; }
		}
		/// <summary>デフォルト会員ランク設定フラグ</summary>
		public string DefaultMemberRankSettingFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_DEFAULT_MEMBER_RANK_SETTING_FLG]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_DEFAULT_MEMBER_RANK_SETTING_FLG] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MEMBERRANK_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MEMBERRANK_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_LAST_CHANGED] = value; }
		}
		/// <summary>会員ランクメモ</summary>
		public string MemberRankMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_MEMO]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_MEMO] = value; }
		}
		/// <summary>定期会員割引率</summary>
		public decimal FixedPurchaseDiscountRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_MEMBERRANK_FIXED_PURCHASE_DISCOUNT_RATE]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_FIXED_PURCHASE_DISCOUNT_RATE] = value; }
		}
		#endregion
	}
}
