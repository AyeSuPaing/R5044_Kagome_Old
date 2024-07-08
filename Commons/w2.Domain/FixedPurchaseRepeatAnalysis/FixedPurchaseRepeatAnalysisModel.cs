/*
=========================================================================================================
  Module      : 定期購入継続分析テーブルモデル (FixedPurchaseRepeatAnalysisModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FixedPurchaseRepeatAnalysis
{
	/// <summary>
	/// 定期購入継続分析テーブルモデル
	/// </summary>
	[Serializable]
	public partial class FixedPurchaseRepeatAnalysisModel : ModelBase<FixedPurchaseRepeatAnalysisModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseRepeatAnalysisModel()
		{
			this.Count = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseRepeatAnalysisModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseRepeatAnalysisModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>データ番号</summary>
		public long DataNo
		{
			get { return (long)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_DATA_NO]; }
		}
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_USER_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_USER_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_PRODUCT_ID] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_VARIATION_ID] = value; }
		}
		/// <summary>回数</summary>
		public int Count
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_COUNT]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_COUNT] = value; }
		}
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_ORDER_ID] = value; }
		}
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_FIXED_PURCHASE_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_FIXED_PURCHASE_ID] = value; }
		}
		/// <summary>ステータス</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_STATUS]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_STATUS] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
