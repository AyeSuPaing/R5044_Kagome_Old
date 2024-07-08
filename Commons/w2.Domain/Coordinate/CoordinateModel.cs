/*
=========================================================================================================
  Module      : コーディネートマスタモデル (CoordinateModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Coordinate
{
	/// <summary>
	/// コーディネートマスタモデル
	/// </summary>
	[Serializable]
	public partial class CoordinateModel : ModelBase<CoordinateModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CoordinateModel()
		{
			this.CoordinateId = "";
			this.CoordinateTitle = "";
			this.CoordinateUrl = "";
			this.CoordinateSummary = "";
			this.InternalMemo = "";
			this.HtmlTitle = "";
			this.MetadataKeywords = "";
			this.MetadataDesc = "";
			this.StaffId = "";
			this.RealShopId = "";
			this.DisplayKbn = Constants.FLG_COORDINATE_DISPLAY_KBN_DRAFT;
			this.DisplayDate = null;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CoordinateModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CoordinateModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>コーディネートID</summary>
		public string CoordinateId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_ID] = value; }
		}
		/// <summary>コーディネートタイトル</summary>
		public string CoordinateTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_TITLE]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_TITLE] = value; }
		}
		/// <summary>コーディネートURL</summary>
		public string CoordinateUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_URL]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_URL] = value; }
		}
		/// <summary>コーディネート概要</summary>
		public string CoordinateSummary
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_SUMMARY]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_SUMMARY] = value; }
		}
		/// <summary>内部用メモ</summary>
		public string InternalMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_INTERNAL_MEMO]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_INTERNAL_MEMO] = value; }
		}
		/// <summary>スタッフID</summary>
		public string StaffId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_STAFF_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_STAFF_ID] = value; }
		}
		/// <summary>リアル店舗ID</summary>
		public string RealShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_REAL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_REAL_SHOP_ID] = value; }
		}
		/// <summary>タイトル</summary>
		public string HtmlTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_HTML_TITLE]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_HTML_TITLE] = value; }
		}
		/// <summary>キーワード</summary>
		public string MetadataKeywords
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_METADATA_KEYWORDS]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_METADATA_KEYWORDS] = value; }
		}
		/// <summary>ディスクリプション</summary>
		public string MetadataDesc
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_METADATA_DESC]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_METADATA_DESC] = value; }
		}
		/// <summary>表示区分</summary>
		public string DisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_KBN] = value; }
		}
		/// <summary>表示用更新日</summary>
		public DateTime? DisplayDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_DATE];
			}
			set { this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_DATE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COORDINATE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COORDINATE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
