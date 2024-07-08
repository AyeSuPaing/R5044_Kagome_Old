/*
=========================================================================================================
  Module      : 外部リンク設定モデル(CsExternalLinkModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.Domain.ExternalLink
{
	/// <summary>
	/// 外部リンクモデル
	/// </summary>
	[Serializable]
	public partial class CsExternalLinkModel : ModelBase<CsExternalLinkModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsExternalLinkModel()
			: base()
        {
        }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">情報</param>
		public CsExternalLinkModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">情報</param>
		public CsExternalLinkModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSEXTERNALLINK_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSEXTERNALLINK_DEPT_ID] = value; }
		}
		/// <summary>リンクID</summary>
		public string LinkId
		{
			get { return (string)(this.DataSource[Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_ID]); }
			set { this.DataSource[Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_ID] = value; }
		}
		/// <summary>リンクタイトル</summary>
		public string LinkTitle
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_TITLE]); }
			set { this.DataSource[Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_TITLE] = value; }
		}
		/// <summary>リンクURL</summary>
		public string LinkUrl
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_URL]); }
			set { this.DataSource[Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_URL] = value; }
		}
		/// <summary>リンクURL</summary>
		public string LinkMemo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_MEMO]); }
			set { this.DataSource[Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_MEMO] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSEXTERNALLINK_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSEXTERNALLINK_VALID_FLG] = value; }
		}
		/// <summary>順位</summary>
		public string DisplayOrder
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSEXTERNALLINK_DISPLAY_ORDER]); }
			set { this.DataSource[Constants.FIELD_CSEXTERNALLINK_DISPLAY_ORDER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSEXTERNALLINK_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSEXTERNALLINK_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSEXTERNALLINK_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSEXTERNALLINK_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSEXTERNALLINK_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSEXTERNALLINK_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
