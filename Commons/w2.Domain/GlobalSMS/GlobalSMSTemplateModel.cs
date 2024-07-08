/*
=========================================================================================================
  Module      : SMSテンプレートモデル (GlobalSMSTemplateModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.GlobalSMS
{
	/// <summary>
	/// SMSテンプレートモデル
	/// </summary>
	[Serializable]
	public partial class GlobalSMSTemplateModel : ModelBase<GlobalSMSTemplateModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public GlobalSMSTemplateModel()
		{
			this.ShopId = "";
			this.MailId = "";
			this.PhoneCarrier = "";
			this.SmsText = "";
			this.LastChanged = "";
			this.DateChanged = DateTime.Now;
			this.DateCreated = DateTime.Now;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalSMSTemplateModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalSMSTemplateModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_SHOP_ID] = value; }
		}
		/// <summary>メールテンプレートID</summary>
		public string MailId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_MAIL_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_MAIL_ID] = value; }
		}
		/// <summary>キャリア</summary>
		public string PhoneCarrier
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_PHONE_CARRIER]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_PHONE_CARRIER] = value; }
		}
		/// <summary>SMS本文</summary>
		public string SmsText
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_SMS_TEXT]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_SMS_TEXT] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
