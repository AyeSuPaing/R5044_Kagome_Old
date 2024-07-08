/*
=========================================================================================================
  Module      : メッセージメール添付ファイルモデルのパーシャルクラス(CsMessageMailAttachmentModel_EX.cs)
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
using System.Web;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージメール添付ファイルモデルのパーシャルクラス
	/// </summary>
	public partial class CsMessageMailAttachmentModel : ModelBase<CsMessageMailAttachmentModel>
	{
		#region プロパティ
		/// <summary>ファイルNO（Before）</summary>
		public int EX_FileNoBefore
		{
			get { return (int)this.DataSource["file_no_before"]; }
			set { this.DataSource["file_no_before"] = value; }
		}
		#endregion

		/// <summary>
		/// ファイルダウンロードURLの作成
		/// </summary>
		/// <param name="requestKeyMailId">リクエストキー：メールID</param>
		/// <param name="requestKeyFileNo">リクエストキー：ファイルNO</param>
		/// <returns>ダウンロードURL</returns>
		public string EX_CreateFileDownloadUrl(string requestKeyMailId, string requestKeyFileNo)
		{
			return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_MAILATTACHMENTDOWNLOADER
				+ "?" + requestKeyMailId + "=" + HttpUtility.UrlEncode(this.MailId) + "&" + requestKeyFileNo + "=" + this.FileNo;
		}
	}
}
