/*
=========================================================================================================
  Module      : Gooddeal constants (GooddealConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Helper.Attribute;

namespace w2.App.Common.Gooddeal
{
	/// <summary>
	/// Gooddeal constants
	/// </summary>
	public class GooddealConstants
	{
		/// <summary>Gooddeal status success</summary>
		public const string GOODDEAL_STATUS_SUCCESS = "Success";
		/// <summary>Gooddeal status failure</summary>
		public const string GOODDEAL_STATUS_FAILURE = "Fail";

		/// <summary>Shipping gooddeal register error code success</summary>
		public const string SHIPPING_GOODDEAL_REGISTER_ERROR_CODE_SUCCESS = "000";
		/// <summary>Shipping gooddeal cancel error code success</summary>
		public const string SHIPPING_GOODDEAL_CANCEL_ERROR_CODE_SUCCESS = "014";

		/// <summary>Taiwan prefix tel number</summary>
		public const string CONST_TW_PREFIX_TEL_NO = "09";

		/// <summary>Gooddeal mail tag replace: Time begin</summary>
		public const string GOODDEAL_MAIL_TAG_REPLACE_KEY_TIME_BEGIN = "time_begin";
		/// <summary>Gooddeal mail tag replace: Time end</summary>
		public const string GOODDEAL_MAIL_TAG_REPLACE_KEY_TIME_END = "time_end";
		/// <summary>Gooddeal mail tag replace: Execute count</summary>
		public const string GOODDEAL_MAIL_TAG_REPLACE_KEY_EXECUTE_COUNT = "execute_count";
		/// <summary>Gooddeal mail tag replace: Success count</summary>
		public const string GOODDEAL_MAIL_TAG_REPLACE_KEY_SUCCESS_COUNT = "success_count";
		/// <summary>Gooddeal mail tag replace: Failure count</summary>
		public const string GOODDEAL_MAIL_TAG_REPLACE_KEY_FAILURE_COUNT = "failure_count";
		/// <summary>Gooddeal mail tag replace: Message</summary>
		public const string GOODDEAL_MAIL_TAG_REPLACE_KEY_MESSAGE = "message";

		/// <summary>Long date format</summary>
		public const string CONST_LONGDATE_FORMAT = "yyyy/MM/dd HH:mm:ss";

		/// <summary>配送時間帯指定無し</summary>
		public const string CONST_SHIPPING_TIME_NOT_SPECIFY = "4";

		/// <summary>
		/// 配送サービスが越境の番号
		/// コード 32= 越境_SF Express
		/// コード 33= 越境_好馬吉（ハマジ）
		/// コード 34= 越境_郵便局
		/// コード 35= 越境_その他
		/// コード 36= 越境_宇迅（Yuxun）
		/// </summary>
		public static string[] TRANSNATIONNAL_NUMBER = { "32", "33", "34", "35", "36" };

		/// <summary>支払い区分</summary>
		public enum PostPayType
		{
			/// <summary>着払い</summary>
			[EnumTextName("到付")]
			CashOnDelivery,
			/// <summary>差出人の支払い</summary>
			[EnumTextName("寄付")]
			SenderPayment
		}
	}
}
