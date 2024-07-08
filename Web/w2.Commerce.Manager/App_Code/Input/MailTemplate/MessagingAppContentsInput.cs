/*
=========================================================================================================
  Module      : メッセージアプリ向けコンテンツ入力クラス (MessagingAppContentsInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.MessagingAppContents;

namespace Input.MailTemplate
{
	/// <summary>
	/// メッセージアプリ向けコンテンツ入力クラス
	/// </summary>
	[Serializable]
	public class MessagingAppContentsInput : InputBase<MessagingAppContentsModel>
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MessagingAppContentsInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public MessagingAppContentsInput(MessagingAppContentsModel model)
			: this()
		{
			this.DeptId = model.DeptId;
			this.MasterKbn = model.MasterKbn;
			this.MasterId = model.MasterId;
			this.MessagingAppKbn = model.MessagingAppKbn;
			this.BranchNo = model.BranchNo;
			this.MediaType = model.MediaType;
			this.Contents = model.Contents;
			this.LastChanged = model.LastChanged;
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns></returns>
		public override MessagingAppContentsModel CreateModel()
		{
			var model = new MessagingAppContentsModel
			{
				DeptId = this.DeptId,
				MasterKbn = this.MasterKbn,
				MasterId = this.MasterId,
				MessagingAppKbn = this.MessagingAppKbn,
				BranchNo = this.BranchNo,
				MediaType = this.MediaType,
				Contents = this.Contents,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				LastChanged = this.LastChanged
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			// メッセージアプリ区分別にチェックできるように
			this.DataSource[this.MessagingAppKbn + "_" + Constants.FIELD_MESSAGINGAPPCONTENTS_CONTENTS] = this.Contents;
			var errorMessage = Validator.Validate("MessagingAppContents", this.DataSource);
			errorMessage = errorMessage.Replace(Constants.FIELD_MESSAGINGAPPCONTENTS_BRANCH_NO, this.BranchNo);
			return errorMessage;
		}

		/// <summary>
		/// 識別ID
		/// </summary>
		public string DeptId
		{
			get { return (string)this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_DEPT_ID]; }
			set { this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_DEPT_ID] = value; }
		}
		/// <summary>
		/// マスタ区分
		/// </summary>
		public string MasterKbn
		{
			get { return (string)this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_KBN]; }
			set { this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_KBN] = value; }
		}
		/// <summary>
		/// マスタID
		/// </summary>
		public string MasterId
		{
			get { return (string)this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_ID]; }
			set { this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_ID] = value; }
		}
		/// <summary>
		/// メッセージアプリ区分
		/// </summary>
		public string MessagingAppKbn
		{
			get { return (string)this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_MESSAGING_APP_KBN]; }
			set { this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_MESSAGING_APP_KBN] = value; }
		}
		/// <summary>
		/// 枝番
		/// </summary>
		public string BranchNo
		{
			get { return (string)this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_BRANCH_NO]; }
			set { this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_BRANCH_NO] = value; }
		}
		/// <summary>
		/// メディアタイプ
		/// </summary>
		public string MediaType
		{
			get { return (string)this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_MEDIA_TYPE]; }
			set { this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_MEDIA_TYPE] = value; }
		}
		/// <summary>
		/// コンテンツ
		/// </summary>
		public string Contents
		{
			get { return (string)this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_CONTENTS]; }
			set { this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_CONTENTS] = value; }
		}
		/// <summary>
		/// 最終更新者
		/// </summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_LAST_CHANGED]; }
			set { this.DataSource[w2.Domain.Constants.FIELD_MESSAGINGAPPCONTENTS_LAST_CHANGED] = value; }
		}
	}
}