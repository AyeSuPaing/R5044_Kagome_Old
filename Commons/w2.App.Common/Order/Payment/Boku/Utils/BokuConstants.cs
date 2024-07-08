/*
=========================================================================================================
  Module      : Boku Constants(BokuConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.Boku.Utils
{
	/// <summary>
	/// Boku Constants
	/// </summary>
	public class BokuConstants
	{
		/// <summary>Const: Boku charge status success</summary>
		public const string CONST_BOKU_CHARGE_STATUS_SUCCESS = "success";
		/// <summary>Const: Boku charge status fail</summary>
		public const string CONST_BOKU_CHARGE_STATUS_FAILED = "failed";
		/// <summary>Const: Boku charge status in-progress</summary>
		public const string CONST_BOKU_CHARGE_STATUS_IN_PROGRESS = "in-progress";

		/// <summary>Const: Boku charge status success</summary>
		public const string CONST_BOKU_REFUND_STATUS_SUCCESS = "success";
		/// <summary>Const: Boku charge status fail</summary>
		public const string CONST_BOKU_REFUND_STATUS_FAILED = "failed";
		/// <summary>Const: Boku charge status in-progress</summary>
		public const string CONST_BOKU_REFUND_STATUS_IN_PROGRESS = "in-progress";

		/// <summary>Const: Boku charge status pending-validate</summary>
		public const string CONST_BOKU_OPTIN_STATUS_PENDING_VALIDATE = "pending-validate";
		/// <summary>Const: Boku optin status pending-confirm</summary>
		public const string CONST_BOKU_OPTIN_STATUS_PENDING_CONFIRM = "pending-confirm";
		/// <summary>Const: Boku optin status active</summary>
		public const string CONST_BOKU_OPTIN_STATUS_ACTIVE = "active";
		/// <summary>Const: Boku optin status closed</summary>
		public const string CONST_BOKU_OPTIN_STATUS_CLOSE = "closed";

		/// <summary>Const: Boku optin type opt</summary>
		public const string CONST_BOKU_OPTIN_TYPE_OPT = "opt";
		/// <summary>Const: Boku optin type hosted</summary>
		public const string CONST_BOKU_OPTIN_TYPE_HOSTED = "hosted";

		/// <summary>Const: Boku charge subcription period unit day</summary>
		public const string CONST_BOKU_CHARGE_SUBCRIPTION_PERIOD_UNIT_DAY = "day";
		/// <summary>Const: Boku charge subcription period unit week</summary>
		public const string CONST_BOKU_CHARGE_SUBCRIPTION_PERIOD_UNIT_WEEK = "week";
		/// <summary>Const: Boku charge subcription period unit month</summary>
		public const string CONST_BOKU_CHARGE_SUBCRIPTION_PERIOD_UNIT_MONTH = "month";
		/// <summary>Const: Boku charge subcription period unit year</summary>
		public const string CONST_BOKU_CHARGE_SUBCRIPTION_PERIOD_UNIT_YEAR = "year";

		/// <summary>Const: Boku optin terms optin purpose standing approval</summary>
		public const string CONST_BOKU_OPTIN_TERMS_OPTIN_PURPOSE_STANDING_APPROVAL = "standing-approval";
		/// <summary>Const: Boku optin terms optin purpose single transaction</summary>
		public const string CONST_BOKU_OPTIN_TERMS_OPTIN_PURPOSE_SINGLE_TRANSACTION = "single-transaction";
		/// <summary>Const: Boku optin terms optin purpose subcription</summary>
		public const string CONST_BOKU_OPTIN_TERMS_OPTIN_PURPOSE_SUBCRIPTION = "subcription";

		/// <summary>Const: Boku tax mode inctax</summary>
		public const string CONST_BOKU_TAX_MODE_INCTAX = "inctax";
		/// <summary>Const: Boku tax mode extax</summary>
		public const string CONST_BOKU_TAX_MODE_EXTAX = "extax";

		/// <summary>Const: Boku reason code unauthorized use minor</summary>
		public const int CONST_BOKU_REASON_CODE_UNAUTHORIZED_USE_MINOR = 3;
		/// <summary>Const: Boku reason code non fulfillment</summary>
		public const int CONST_BOKU_REASON_CODE_NON_FULFILLMENT = 8;
		/// <summary>Const: Boku reason code unauthorized use known</summary>
		public const int CONST_BOKU_REASON_CODE_UNAUTHORIZED_USE_KNOWN = 19;
		/// <summary>Const: Boku reason code fraud</summary>
		public const int CONST_BOKU_REASON_CODE_FRAUD = 20;
		/// <summary>Const: Boku reason code unauthorized use unknown</summary>
		public const int CONST_BOKU_REASON_CODE_UNAUTHORIZED_USE_UNKNOWN = 22;
		/// <summary>Const: Boku reason code good will</summary>
		public const int CONST_BOKU_REASON_CODE_GOOD_WILL = 33;

		/// <summary>Const: Boku payment method carrierbilling</summary>
		public const string CONST_BOKU_PAYMENT_METHOD_CARRIERBILLING = "carrierbilling";

		/// <summary>Const: Boku default currency code</summary>
		public static string CONST_BOKU_DEFAULT_CURRENCY_CODE = "JPY";
	}
}
