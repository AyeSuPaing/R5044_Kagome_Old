/*
=========================================================================================================
  Module      : Lpページデザインモデル (LandingPageDesignModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.LandingPage
{
	/// <summary>
	/// Lpページデザインモデル
	/// </summary>
	[Serializable]
	public partial class LandingPageDesignModel : ModelBase<LandingPageDesignModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public LandingPageDesignModel()
		{
			this.PageId = "";
			this.PageTitle = "";
			this.PageFileName = "";
			this.PublicStatus = LandingPageConst.PUBLIC_STATUS_PUBLISHED;
			this.PublicStartDatetime = null;
			this.PublicEndDatetime = null;
			this.UserRegistrationType = LandingPageConst.USER_REGISTRATION_TYPE_AUTO;
			this.ProductChooseType = LandingPageConst.PRODUCT_CHOOSE_TYPE_DONOTCHOOSE;
			this.LoginFormType = LandingPageConst.LOGIN_FORM_TYPE_VISIBLE;
			this.LastChanged = "";
			this.SocialLoginUseType = LandingPageConst.SOCIAL_LOGIN_USE_TYPE_ALL;
			this.SocialLoginList = "";
			this.TagSettingList = "";
			this.EfoCubeUseFlg = LandingPageConst.EFO_CUBE_USE_FLG_ON;
			this.OrderConfirmPageSkipFlg = LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_OFF;
			this.MailAddressConfirmFormUseFlg = LandingPageConst.MAIL_ADDRESS_CONFIRM_FORM_USE_FLG_ON;
			this.UnpermittedPaymentIds = "";
			this.ProductSets = new LandingPageProductSetModel[]{};
			this.PaymentChooseType = "";
			this.DefaultPaymentId = "";
			this.NoveltyUseFlg = LandingPageConst.NOVELTY_USE_FLG_OFF;
			this.DesignMode = Constants.FLG_LANDINGPAGEDESIGN_DESIGN_MODE_DEFAULT;
			this.PersonalAuthenticationUseFlg = LandingPageConst.PERSONAL_AUTHENTICATION_USE_FLG_OFF;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LandingPageDesignModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LandingPageDesignModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ページID</summary>
		public string PageId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_ID] = value; }
		}
		/// <summary>ページタイトル</summary>
		public string PageTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_TITLE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_TITLE] = value; }
		}
		/// <summary>ページファイル名</summary>
		public string PageFileName
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_FILE_NAME]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_FILE_NAME] = value; }
		}
		/// <summary>公開状態</summary>
		public string PublicStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_STATUS]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_STATUS] = value; }
		}
		/// <summary>公開開始日時</summary>
		public DateTime? PublicStartDatetime
		{
			get
			{
				if (this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_START_DATETIME] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_START_DATETIME];
			}
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_START_DATETIME] = value; }
		}
		/// <summary>公開終了日時</summary>
		public DateTime? PublicEndDatetime
		{
			get
			{
				if (this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_END_DATETIME] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_END_DATETIME];
			}
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_END_DATETIME] = value; }
		}
		/// <summary>商品選択タイプ</summary>
		public string ProductChooseType
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PRODUCT_CHOOSE_TYPE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PRODUCT_CHOOSE_TYPE] = value; }
		}
		/// <summary>会員登録タイプ</summary>
		public string UserRegistrationType
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_USER_REGISTRATION_TYPE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_USER_REGISTRATION_TYPE] = value; }
		}
		/// <summary>ログインフォームタイプ</summary>
		public string LoginFormType
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_LOGIN_FORM_TYPE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_LOGIN_FORM_TYPE] = value; }
		}
		/// <summary>SEOディスクリプション</summary>
		public string MetadataDesc
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_METADATA_DESC]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_METADATA_DESC] = value; }
		}
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_MANAGEMENT_TITLE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_MANAGEMENT_TITLE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_LAST_CHANGED] = value; }
		}
		/// <summary>利用するソーシャルログインタイプ</summary>
		public string SocialLoginUseType
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_SOCIAL_LOGIN_USE_TYPE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_SOCIAL_LOGIN_USE_TYPE] = value; }
		}
		/// <summary>ソーシャルログインリスト</summary>
		public string SocialLoginList
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_SOCIAL_LOGIN_LIST]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_SOCIAL_LOGIN_LIST] = value; }
		}
		/// <summary>タグ設定リスト</summary>
		public string TagSettingList
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_TAG_SETTING_LIST]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_TAG_SETTING_LIST] = value; }
		}
		/// <summary>EFO CUBE利用</summary>
		public string EfoCubeUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_EFO_CUBE_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_EFO_CUBE_USE_FLG] = value; }
		}
		/// <summary>確認画面スキップ</summary>
		public string OrderConfirmPageSkipFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_ORDER_CONFIRM_PAGE_SKIP_FLG]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_ORDER_CONFIRM_PAGE_SKIP_FLG] = value; }
		}
		/// <summary>メールアドレス確認フォーム利用フラグ</summary>
		public string MailAddressConfirmFormUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_MAIL_ADDRESS_CONFIRM_FORM_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_MAIL_ADDRESS_CONFIRM_FORM_USE_FLG] = value; }
		}
		/// <summary>除外する決済種別IDリスト</summary>
		public string UnpermittedPaymentIds
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_UNPERMITTED_PAYMENT_IDS]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_UNPERMITTED_PAYMENT_IDS] = value; }
		}
		/// <summary>デフォルト決済種別</summary>
		public string DefaultPaymentId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DEFAULT_PAYMENT_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DEFAULT_PAYMENT_ID] = value; }
		}
		/// <summary>ノベルティ利用フラグ</summary>
		public string NoveltyUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_NOVELTY_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_NOVELTY_USE_FLG] = value; }
		}
		/// <summary>デザインモード</summary>
		public string DesignMode
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DESIGN_MODE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DESIGN_MODE] = value; }
		}
		/// <summary>決済種別選択タイプ</summary>
		public string PaymentChooseType
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAYMENT_CHOOSE_TYPE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAYMENT_CHOOSE_TYPE] = value; }
		}
		/// <summary>Personal authentication use flag</summary>
		public string PersonalAuthenticationUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PERSONAL_AUTHENTICATION_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PERSONAL_AUTHENTICATION_USE_FLG] = value; }
		}
		#endregion
	}
}
