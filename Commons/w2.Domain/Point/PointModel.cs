/*
=========================================================================================================
  Module      : ポイントマスタモデル (PointModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Point
{
	/// <summary>
	/// ポイントマスタモデル
	/// </summary>
	[Serializable]
	public partial class PointModel : ModelBase<PointModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PointModel()
		{
			this.PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE;;
			this.PointExpKbn = Constants.FLG_POINT_POINT_EXP_KBN_VALID;
			this.UsableUnit = 1;
			this.ExchangeRate = 1;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PointModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PointModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINT_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_POINT_DEPT_ID] = value; }
		}
		/// <summary>ポイント区分</summary>
		public string PointKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINT_POINT_KBN]; }
			set { this.DataSource[Constants.FIELD_POINT_POINT_KBN] = value; }
		}
		/// <summary>ポイント有効期限設定</summary>
		public string PointExpKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINT_POINT_EXP_KBN]; }
			set { this.DataSource[Constants.FIELD_POINT_POINT_EXP_KBN] = value; }
		}
		/// <summary>ポイント利用可能単位</summary>
		public long UsableUnit
		{
			get { return (long)this.DataSource[Constants.FIELD_POINT_USABLE_UNIT]; }
			set { this.DataSource[Constants.FIELD_POINT_USABLE_UNIT] = value; }
		}
		/// <summary>ポイント換算率</summary>
		public decimal ExchangeRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_POINT_EXCHANGE_RATE]; }
			set { this.DataSource[Constants.FIELD_POINT_EXCHANGE_RATE] = value; }
		}
		/// <summary>予備区分1</summary>
		public string Kbn1
		{
			get { return (string)this.DataSource[Constants.FIELD_POINT_KBN1]; }
			set { this.DataSource[Constants.FIELD_POINT_KBN1] = value; }
		}
		/// <summary>予備区分2</summary>
		public string Kbn2
		{
			get { return (string)this.DataSource[Constants.FIELD_POINT_KBN2]; }
			set { this.DataSource[Constants.FIELD_POINT_KBN2] = value; }
		}
		/// <summary>予備区分3</summary>
		public string Kbn3
		{
			get { return (string)this.DataSource[Constants.FIELD_POINT_KBN3]; }
			set { this.DataSource[Constants.FIELD_POINT_KBN3] = value; }
		}
		/// <summary>予備区分4</summary>
		public string Kbn4
		{
			get { return (string)this.DataSource[Constants.FIELD_POINT_KBN4]; }
			set { this.DataSource[Constants.FIELD_POINT_KBN4] = value; }
		}
		/// <summary>予備区分5</summary>
		public string Kbn5
		{
			get { return (string)this.DataSource[Constants.FIELD_POINT_KBN5]; }
			set { this.DataSource[Constants.FIELD_POINT_KBN5] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_POINT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_POINT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_POINT_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_POINT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_POINT_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_POINT_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
