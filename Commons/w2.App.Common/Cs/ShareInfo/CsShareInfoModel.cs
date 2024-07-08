/*
=========================================================================================================
  Module      : 共有情報モデル(CsShareInfoModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.ShareInfo
{
	/// <summary>
	/// 共有情報モデル
	/// </summary>
	[Serializable]
	public partial class CsShareInfoModel : ModelBase<CsShareInfoModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsShareInfoModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">共有情報データ</param>
		public CsShareInfoModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">共有情報データ</param>
		public CsShareInfoModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFO_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_DEPT_ID] = value; }
		}
		/// <summary>共有情報NO</summary>
		public long InfoNo
		{
			get { return (long)this.DataSource[Constants.FIELD_CSSHAREINFO_INFO_NO]; }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_INFO_NO] = value; }
		}
		/// <summary>共有情報テキスト区分</summary>
		public string InfoTextKbn
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFO_INFO_TEXT_KBN]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_INFO_TEXT_KBN] = value; }
		}
		/// <summary>共有情報テキスト</summary>
		public string InfoText
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFO_INFO_TEXT]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_INFO_TEXT] = value; }
		}
		/// <summary>共有情報区分</summary>
		public string InfoKbn
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFO_INFO_KBN]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_INFO_KBN] = value; }
		}
		/// <summary>共有情報重要度</summary>
		public int InfoImportance
		{
			get { return (int)this.DataSource[Constants.FIELD_CSSHAREINFO_INFO_IMPORTANCE]; }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_INFO_IMPORTANCE] = value; }
		}
		/// <summary>送信元オペレータID</summary>
		public string Sender
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFO_SENDER]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_SENDER] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFO_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSSHAREINFO_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSSHAREINFO_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFO_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFO_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
