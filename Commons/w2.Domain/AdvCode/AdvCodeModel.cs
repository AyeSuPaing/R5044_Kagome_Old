/*
=========================================================================================================
  Module      : 広告コードマスタモデル (AdvCodeModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.AdvCode
{
	/// <summary>
	/// 広告コードマスタモデル
	/// </summary>
	[Serializable]
	public partial class AdvCodeModel : ModelBase<AdvCodeModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AdvCodeModel()
		{
			this.ValidFlg = "1";
			this.AdvertisementDate = null;
			this.MediaCost = null;
			this.PublicationDateFrom = null;
			this.PublicationDateTo = null;
			this.MemberRankIdGrantedAtAccountRegistration = string.Empty;
			this.UserManagementLevelIdGrantedAtAccountRegistration = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AdvCodeModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AdvCodeModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>広告コードNO</summary>
		public long AdvcodeNo
		{
			get { return (long)this.DataSource[Constants.FIELD_ADVCODE_ADVCODE_NO]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_ADVCODE_NO] = value; }
		}
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_DEPT_ID] = value; }
		}
		/// <summary>広告コード</summary>
		public string AdvertisementCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE] = value; }
		}
		/// <summary>媒体名</summary>
		public string MediaName
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_MEDIA_NAME]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_MEDIA_NAME] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ADVCODE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ADVCODE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_LAST_CHANGED] = value; }
		}
		/// <summary>出稿日</summary>
		public DateTime? AdvertisementDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE];
			}
			set { this.DataSource[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE] = value; }
		}
		/// <summary>媒体費</summary>
		public decimal? MediaCost
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ADVCODE_MEDIA_COST] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ADVCODE_MEDIA_COST];
			}
			set { this.DataSource[Constants.FIELD_ADVCODE_MEDIA_COST] = value; }
		}
		/// <summary>備考</summary>
		public string Memo
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_MEMO]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_MEMO] = value; }
		}
		/// <summary>媒体掲載期間(From)</summary>
		public DateTime? PublicationDateFrom
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_FROM] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_FROM];
			}
			set { this.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_FROM] = value; }
		}
		/// <summary>媒体掲載期間(To)</summary>
		public DateTime? PublicationDateTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_TO] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_TO];
			}
			set { this.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_TO] = value; }
		}
		/// <summary>広告媒体区分</summary>
		public string AdvcodeMediaTypeId
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_ADVCODE_MEDIA_TYPE_ID]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_ADVCODE_MEDIA_TYPE_ID] = value; }
		}
		/// <summary>会員登録時紐づけ会員ランクID</summary>
		public string MemberRankIdGrantedAtAccountRegistration
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_MEMBER_RANK_ID_GRANTED_AT_ACCOUNT_REGISTRATION]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_MEMBER_RANK_ID_GRANTED_AT_ACCOUNT_REGISTRATION] = value; }
		}
		/// <summary>会員登録時紐づけユーザー管理レベルID</summary>
		public string UserManagementLevelIdGrantedAtAccountRegistration
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_USER_MANAGEMENT_LEVEL_ID_GRANTED_AT_ACCOUNT_REGISTRATION]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_USER_MANAGEMENT_LEVEL_ID_GRANTED_AT_ACCOUNT_REGISTRATION] = value; }
		}
		#endregion
	}
}
