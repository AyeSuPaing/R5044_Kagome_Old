/*
=========================================================================================================
  Module      : スタッフモデル (StaffModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Staff
{
	/// <summary>
	/// スタッフモデル
	/// </summary>
	[Serializable]
	public partial class StaffModel : ModelBase<StaffModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public StaffModel()
		{
			this.StaffId = "";
			this.StaffName = "";
			this.StaffProfile = "";
			this.StaffHeight = 0;
			this.StaffInstagramId = "";
			this.StaffSex = Constants.FLG_STAFF_SEX_UNKNOWN;
			this.ModelFlg = Constants.FLG_STAFF_MODEL_FLG_VALID;
			this.OperatorId = "";
			this.RealShopId = "";
			this.ValidFlg = Constants.FLG_STAFF_VALID_FLG_VALID;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public StaffModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public StaffModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>スタッフID</summary>
		public string StaffId
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_STAFF_ID]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_ID] = value; }
		}
		/// <summary>氏名</summary>
		public string StaffName
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_STAFF_NAME]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_NAME] = value; }
		}
		/// <summary>プロフィールテキスト</summary>
		public string StaffProfile
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_STAFF_PROFILE]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_PROFILE] = value; }
		}
		/// <summary>身長</summary>
		public int StaffHeight
		{
			get { return (int)this.DataSource[Constants.FIELD_STAFF_STAFF_HEIGHT]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_HEIGHT] = value; }
		}
		/// <summary>インスタグラムID</summary>
		public string StaffInstagramId
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_STAFF_INSTAGRAM_ID]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_INSTAGRAM_ID] = value; }
		}
		/// <summary>性別</summary>
		public string StaffSex
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_STAFF_SEX]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_SEX] = value; }
		}
		/// <summary>モデルフラグ</summary>
		public string ModelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_MODEL_FLG]; }
			set { this.DataSource[Constants.FIELD_STAFF_MODEL_FLG] = value; }
		}
		/// <summary>オペレータID</summary>
		public string OperatorId
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_OPERATOR_ID]; }
			set { this.DataSource[Constants.FIELD_STAFF_OPERATOR_ID] = value; }
		}
		/// <summary>リアル店舗ID</summary>
		public string RealShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_REAL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_STAFF_REAL_SHOP_ID] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_STAFF_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_STAFF_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_STAFF_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_STAFF_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_STAFF_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_STAFF_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
