/*
=========================================================================================================
  Module      : モール連携更新ログモデル (MallCooperationUpdateLogModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MallCooperationUpdateLog
{
	/// <summary>
	/// モール連携更新ログモデル
	/// </summary>
	[Serializable]
	public partial class MallCooperationUpdateLogModel : ModelBase<MallCooperationUpdateLogModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MallCooperationUpdateLogModel()
		{
			this.ActionStatus = Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_STATUS_INITIAL;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MallCooperationUpdateLogModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MallCooperationUpdateLogModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ログNO</summary>
		public long LogNo
		{
			get { return (long)this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_LOG_NO]; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_SHOP_ID] = value; }
		}
		/// <summary>モールID</summary>
		public string MallId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_VARIATION_ID] = value; }
		}
		/// <summary>マスタ区分</summary>
		public string MasterKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_MASTER_KBN]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_MASTER_KBN] = value; }
		}
		/// <summary>処理区分</summary>
		public string ActionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_ACTION_KBN]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_ACTION_KBN] = value; }
		}
		/// <summary>処理ステータス</summary>
		public string ActionStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_ACTION_STATUS]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_ACTION_STATUS] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONUPDATELOG_DATE_CHANGED] = value; }
		}
		#endregion
	}
}
