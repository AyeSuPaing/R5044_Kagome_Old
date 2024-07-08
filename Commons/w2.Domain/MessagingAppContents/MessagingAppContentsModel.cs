/*
=========================================================================================================
  Module      : メッセージアプリ向けコンテンツモデルクラス(MessagingAppContentsModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MessagingAppContents
{
	/// <summary>
	/// メッセージアプリ向けコンテンツモデルクラス
	/// </summary>
	public partial class MessagingAppContentsModel : ModelBase<MessagingAppContentsModel>
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MessagingAppContentsModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MessagingAppContentsModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MessagingAppContentsModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}

		/// <summary>
		/// 識別ID
		/// </summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_DEPT_ID] = value; }
		}
		/// <summary>
		/// マスタ区分
		/// </summary>
		public string MasterKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_KBN]; }
			set { this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_KBN] = value; }
		}
		/// <summary>
		/// マスタID
		/// </summary>
		public string MasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_ID] = value; }
		}
		/// <summary>
		/// メッセージアプリ区分
		/// </summary>
		public string MessagingAppKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_MESSAGING_APP_KBN]; }
			set { this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_MESSAGING_APP_KBN] = value; }
		}
		/// <summary>
		/// 枝番
		/// </summary>
		public string BranchNo
		{
			get { return (string)this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_BRANCH_NO] = value; }
		}
		/// <summary>
		/// メディアタイプ
		/// </summary>
		public string MediaType
		{
			get { return (string)this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_MEDIA_TYPE]; }
			set { this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_MEDIA_TYPE] = value; }
		}
		/// <summary>
		/// コンテンツ
		/// </summary>
		public string Contents
		{
			get { return (string)this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_CONTENTS]; }
			set { this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_CONTENTS] = value; }
		}
		/// <summary>
		/// 作成日
		/// </summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_DATE_CREATED] = value; }
		}
		/// <summary>
		/// 更新日
		/// </summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_DATE_CHANGED] = value; }
		}
		/// <summary>
		/// 最終更新者
		/// </summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MESSAGINGAPPCONTENTS_LAST_CHANGED] = value; }
		}
	}
}
