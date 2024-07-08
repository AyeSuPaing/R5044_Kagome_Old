/*
=========================================================================================================
  Module      : 決済カード連携マスタモデル (UserCreditCardModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.UserCreditCard
{
	/// <summary>
	/// 決済カード連携マスタモデル
	/// </summary>
	[Serializable]
	public partial class UserCreditCardModel : ModelBase<UserCreditCardModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserCreditCardModel()
		{
			this.BranchNo = 1;
			this.CooperationId = "";
			this.CardDispName = "";
			this.LastFourDigit = "";
			this.ExpirationMonth = "";
			this.ExpirationYear = "";
			this.AuthorName = "";
			this.DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_ON;
			this.LastChanged = "";
			this.CompanyCode = "";
			this.CooperationId2 = "";
			this.RegisterActionKbn = Constants.FLG_USERCREDITCARD_REGISTER_STATUS_NORMAL;
			this.RegisterStatus = Constants.FLG_USERCREDITCARD_REGISTER_STATUS_NORMAL;
			this.RegisterTargetId = "";
			this.BeforeOrderStatus = "";
			this.CooperationType = Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_CREDITCARD;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserCreditCardModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserCreditCardModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		[UpdateData(1, "user_id")]
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_USER_ID] = value; }
		}
		/// <summary>カード枝番</summary>
		[UpdateData(2, "branch_no")]
		public int BranchNo
		{
			get { return (int)this.DataSource[Constants.FIELD_USERCREDITCARD_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_BRANCH_NO] = value; }
		}
		/// <summary>連携ID</summary>
		[UpdateData(3, "cooperation_id")]
		public string CooperationId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_COOPERATION_ID]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_COOPERATION_ID] = value; }
		}
		/// <summary>カード表示名</summary>
		[UpdateData(4, "card_disp_name")]
		public string CardDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_CARD_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_CARD_DISP_NAME] = value; }
		}
		/// <summary>カード番号下４桁</summary>
		[UpdateData(5, "last_four_digit")]
		public string LastFourDigit
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_LAST_FOUR_DIGIT]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_LAST_FOUR_DIGIT] = value; }
		}
		/// <summary>有効期限（月）</summary>
		[UpdateData(6, "expiration_month")]
		public string ExpirationMonth
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_EXPIRATION_MONTH]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_EXPIRATION_MONTH] = value; }
		}
		/// <summary>有効期限（年）</summary>
		[UpdateData(7, "expiration_year")]
		public string ExpirationYear
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_EXPIRATION_YEAR]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_EXPIRATION_YEAR] = value; }
		}
		/// <summary>カード名義人</summary>
		[UpdateData(8, "author_name")]
		public string AuthorName
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_AUTHOR_NAME]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_AUTHOR_NAME] = value; }
		}
		/// <summary>表示フラグ</summary>
		[UpdateData(9, "disp_flg")]
		public string DispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_DISP_FLG] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(10, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERCREDITCARD_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(11, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERCREDITCARD_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		[UpdateData(12, "last_changed")]
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_LAST_CHANGED] = value; }
		}
		/// <summary>カード会社コード</summary>
		[UpdateData(13, "company_code")]
		public string CompanyCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_COMPANY_CODE]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_COMPANY_CODE] = value; }
		}
		/// <summary>連携ID2</summary>
		[UpdateData(14, "cooperation_id2")]
		public string CooperationId2
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_COOPERATION_ID2]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_COOPERATION_ID2] = value; }
		}
		/// <summary>登録アクション区分</summary>
		[UpdateDataAttribute(15, "register_action_kbn")]
		public string RegisterActionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_REGISTER_ACTION_KBN]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_REGISTER_ACTION_KBN] = value; }
		}
		/// <summary>登録ステータス</summary>
		[UpdateDataAttribute(16, "register_status")]
		public string RegisterStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_REGISTER_STATUS]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_REGISTER_STATUS] = value; }
		}
		/// <summary>登録対象ID</summary>
		[UpdateDataAttribute(17, "register_target_id")]
		public string RegisterTargetId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_REGISTER_TARGET_ID]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_REGISTER_TARGET_ID] = value; }
		}
		/// <summary>更新前ステータス</summary>
		[UpdateDataAttribute(18, "before_order_status")]
		public string BeforeOrderStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_BEFORE_ORDER_STATUS]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_BEFORE_ORDER_STATUS] = value; }
		}
		/// <summary>連携種別</summary>
		[UpdateDataAttribute(19, "cooperation_type")]
		public string CooperationType
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_COOPERATION_TYPE]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_COOPERATION_TYPE] = value; }
		}
		#endregion
	}
}
