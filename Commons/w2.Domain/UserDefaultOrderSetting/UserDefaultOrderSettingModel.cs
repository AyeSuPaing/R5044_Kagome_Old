/*
=========================================================================================================
  Module      : デフォルト注文方法モデル (UserDefaultOrderSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.UserDefaultOrderSetting
{
	/// <summary>
	/// デフォルト注文方法モデル
	/// </summary>
	[Serializable]
	public partial class UserDefaultOrderSettingModel : ModelBase<UserDefaultOrderSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserDefaultOrderSettingModel()
		{
			this.PaymentId = null;
			this.CreditBranchNo = null;
			this.UserShippingNo = null;
			this.UserInvoiceNo = null;
			this.RakutenCvsType = null;
			this.ZeusCvsType = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserDefaultOrderSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserDefaultOrderSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_ID] = value; }
		}
		/// <summary>決済種別ID</summary>
		public string PaymentId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_PAYMENT_ID] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_PAYMENT_ID];
			}
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_PAYMENT_ID] = value; }
		}
		/// <summary>クレジットカード枝番</summary>
		public int? CreditBranchNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_CREDIT_BRANCH_NO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_CREDIT_BRANCH_NO];
			}
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_CREDIT_BRANCH_NO] = value; }
		}
		/// <summary>配送先枝番</summary>
		public int? UserShippingNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_SHIPPING_NO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_SHIPPING_NO];
			}
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_SHIPPING_NO] = value; }
		}
		/// <summary>User Invoice No</summary>
		public int? UserInvoiceNo
		{
			get
			{
				return ((this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_INVOICE_NO] == DBNull.Value)
					? null
					: (int?)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_INVOICE_NO]);
			}
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_INVOICE_NO] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>楽天コンビニ前払い支払いコンビニ</summary>
		public string RakutenCvsType
		{
			get { return (string)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_RAKUTEN_CVS_TYPE]; }
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_RAKUTEN_CVS_TYPE] = value; }
		}
		/// <summary>Zeusコンビニ前払い支払いコンビニ</summary>
		public string ZeusCvsType
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_ZEUS_CVS_TYPE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_ZEUS_CVS_TYPE];
			}
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_USER_ZEUS_CVS_TYPE] = value; }
		}
		/// <summary>Paygent convenience store type</summary>
		public string PaygentCvsType
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_PAYGENT_CVS_TYPE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_PAYGENT_CVS_TYPE];
			}
			set { this.DataSource[Constants.FIELD_USERDEFAULTORDERSETTING_PAYGENT_CVS_TYPE] = value; }
		}
		#endregion
	}
}
