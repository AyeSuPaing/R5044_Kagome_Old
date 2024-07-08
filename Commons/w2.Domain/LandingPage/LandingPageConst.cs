/*
=========================================================================================================
  Module      : LP定数クラス(LandingPageConst.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Helper.Attribute;

namespace w2.Domain.LandingPage
{
	/// <summary>
	/// LP定数クラス
	/// </summary>
	public static class LandingPageConst
	{
		/// <summary>デザインタイプ：PC</summary>
		public const string PAGE_DESIGN_TYPE_PC = "PC";
		/// <summary>デザインタイプ：SP</summary>
		public const string PAGE_DESIGN_TYPE_SP = "SP";
		/// <summary>デザインタイプ：公開</summary>
		public const string PUBLIC_STATUS_PUBLISHED = "PUBLISHED";
		/// <summary>デザインタイプ：未公開</summary>
		public const string PUBLIC_STATUS_UNPUBLISHED = "UNPUBLISHED";
		/// <summary>会員登録タイプ：自動</summary>
		public const string USER_REGISTRATION_TYPE_AUTO = "AUTO";
		/// <summary>会員登録タイプ：手動</summary>
		public const string USER_REGISTRATION_TYPE_MANUAL = "MANUAL";
		/// <summary>会員登録タイプ：無効</summary>
		public const string USER_REGISTRATION_TYPE_DISABLE = "DISABLE";
		/// <summary>ログインフォームタイプ：表示</summary>
		public const string LOGIN_FORM_TYPE_VISIBLE = "VISIBLE";
		/// <summary>ログインフォームタイプ：非表示</summary>
		public const string LOGIN_FORM_TYPE_HYDE = "HYDE";
		/// <summary>商品セット 購入タイプ：通常</summary>
		public const string BUY_TYPE_NORMAL = "NORMAL";
		/// <summary>商品セット 購入タイプ：定期</summary>
		public const string BUY_TYPE_FIXEDPURCHASE = "FIXEDPURCHASE";
		/// <summary>商品選択タイプ：選択不可</summary>
		public const string PRODUCT_CHOOSE_TYPE_DONOTCHOOSE = "DONOTCHOOSE";
		/// <summary>商品選択タイプ：チェックボックス</summary>
		public const string PRODUCT_CHOOSE_TYPE_CHECKBOX = "CHECKBOX";
		/// <summary>商品選択タイプ：ドロップダウン</summary>
		public const string PRODUCT_CHOOSE_TYPE_DROPDOWNLIST = "DROPDOWNLIST";
		/// <summary>利用するソーシャルログインタイプ：利用可能なソーシャルログインボタン全て表示</summary>
		public const string SOCIAL_LOGIN_USE_TYPE_ALL = "ALL";
		/// <summary>利用するソーシャルログインタイプ：social_login_listカラムで選択されたもののみ利用</summary>
		public const string SOCIAL_LOGIN_USE_TYPE_ONLY = "ONLY";
		/// <summary>ソーシャルログインリスト選択肢</summary>
		public enum SocialLoginType
		{
			/// <summary>Apple</summary>
			[EnumTextName("APPLE")]
			Apple,
			/// <summary>FaceBook</summary>
			[EnumTextName("FB")]
			FaceBook,
			/// <summary>Line</summary>
			[EnumTextName("LINE")]
			Line,
			/// <summary>Twitter</summary>
			[EnumTextName("TWITTER")]
			Twitter,
			/// <summary>Yahoo</summary>
			[EnumTextName("YAHOO")]
			Yahoo,
			/// <summary>Google</summary>
			[EnumTextName("GPLUS")]
			Gplus,
			/// <summary>PayPal</summary>
			[EnumTextName("PAYPAL")]
			PayPal,
			/// <summary>楽天IDログイン</summary>
			[EnumTextName("RAKUTEN")]
			Rakuten
		}
		/// <summary>EFO CUBE利用：利用する</summary>
		public const string EFO_CUBE_USE_FLG_ON = "ON";
		/// <summary>EFO CUBE利用：利用しない</summary>
		public const string EFO_CUBE_USE_FLG_OFF = "OFF";
		/// <summary>確認画面スキップ：スキップする</summary>
		public const string ORDER_CONFIRM_PAGE_SKIP_FLG_ON = "ON";
		/// <summary>確認画面スキップ：スキップしない</summary>
		public const string ORDER_CONFIRM_PAGE_SKIP_FLG_OFF = "OFF";
		/// <summary>メールアドレス確認フォーム利用：表示する</summary>
		public const string MAIL_ADDRESS_CONFIRM_FORM_USE_FLG_ON = "ON";
		/// <summary>メールアドレス確認フォーム利用：表示しない</summary>
		public const string MAIL_ADDRESS_CONFIRM_FORM_USE_FLG_OFF = "OFF";
		/// <summary>商品セット有効フラグ：有効</summary>
		public const string PRODUCT_SET_VALID_FLG_VALID = "1";
		/// <summary>商品セット有効フラグ：無効</summary>
		public const string PRODUCT_SET_VALID_FLG_INVALID = "0";
		/// <summary>ノベルティ利用：表示する</summary>
		public const string NOVELTY_USE_FLG_ON = "ON";
		/// <summary>ノベルティ利用：表示しない</summary>
		public const string NOVELTY_USE_FLG_OFF = "OFF";
		/// <summary>Personal authentication use flag: ON</summary>
		public const string PERSONAL_AUTHENTICATION_USE_FLG_ON = "ON";
		/// <summary>Personal authentication use flag: OFF</summary>
		public const string PERSONAL_AUTHENTICATION_USE_FLG_OFF = "OFF";
		/// <summary>SNSの名前</summary>
		public static string SNS_NAME_TEXT = "sns_Name";
		/// <summary>Twitter文字列</summary>
		public static string SNS_NAME_TWITTER_TEXT = "twitter";
		/// <summary>TwitterのValue</summary>
		public static string SNS_NAME_TWITTER_VALUE = "TWITTER";
	}
}
