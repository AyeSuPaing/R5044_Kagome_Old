/*
=========================================================================================================
  Module      : コンテンツタグモデル (ContentsTagModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ContentsTag
{
	/// <summary>
	/// コンテンツタグモデル
	/// </summary>
	[Serializable]
	public partial class ContentsTagModel : ModelBase<ContentsTagModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ContentsTagModel()
		{
			this.ContentsTagName = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ContentsTagModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ContentsTagModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>コンテンツタグID</summary>
		public long ContentsTagId
		{
			get { return (long)this.DataSource[Constants.FIELD_CONTENTSTAG_CONTENTS_TAG_ID]; }
		}
		/// <summary>コンテンツタグ名</summary>
		public string ContentsTagName
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSTAG_CONTENTS_TAG_NAME]; }
			set { this.DataSource[Constants.FIELD_CONTENTSTAG_CONTENTS_TAG_NAME] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CONTENTSTAG_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CONTENTSTAG_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CONTENTSTAG_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CONTENTSTAG_DATE_CHANGED] = value; }
		}
		#endregion
	}
}
