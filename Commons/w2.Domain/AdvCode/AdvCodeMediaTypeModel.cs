/*
=========================================================================================================
  Module      : 広告媒体区分マスタモデル (AdvCodeMediaTypeModel.cs)
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
	/// 広告媒体区分マスタモデル
	/// </summary>
	[Serializable]
	public partial class AdvCodeMediaTypeModel : ModelBase<AdvCodeMediaTypeModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AdvCodeMediaTypeModel()
		{
			this.DisplayOrder = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AdvCodeMediaTypeModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AdvCodeMediaTypeModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>区分ID</summary>
		public string AdvcodeMediaTypeId
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID] = value; }
		}
		/// <summary>媒体区分名</summary>
		public string AdvcodeMediaTypeName
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DISPLAY_ORDER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
