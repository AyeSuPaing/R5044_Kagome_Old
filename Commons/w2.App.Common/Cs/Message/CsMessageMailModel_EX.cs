/*
=========================================================================================================
  Module      : メッセージメールモデルのパーシャルクラス(CsMessageMailModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージメールモデルのパーシャルクラス
	/// </summary>
	public partial class CsMessageMailModel : ModelBase<CsMessageMailModel>
	{
		#region プロパティ
		/// <summary>メールReferences</summary>
		public string[] EX_References
		{
			get { return (string[])(this.DataSource["References"] ?? new string[0]); }
			set { this.DataSource["References"] = value; }
		}
		/// <summary>メールデータ</summary>
		public CsMessageMailDataModel EX_MailDataModel
		{
			get { return (CsMessageMailDataModel)this.DataSource["MailDataModel"]; }
			set { this.DataSource["MailDataModel"] = value; }
		}
		/// <summary>メール添付ファイル</summary>
		public CsMessageMailAttachmentModel[] EX_MailAttachments
		{
			get { return (CsMessageMailAttachmentModel[])this.DataSource["MailAttachments"] ?? new CsMessageMailAttachmentModel[0]; }
			set { this.DataSource["MailAttachments"] = value; }
		}
		#endregion
	}
}
