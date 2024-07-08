/*
=========================================================================================================
  Module      : ソフトバンクペイメントユーティリティ(PaymentSBPSUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメントユーティリティ（主にPKG固有の定数などと決済との中間を担う）
	/// </summary>
	public class PaymentSBPSUtil
	{
		/// <summary>
		/// 決済種別IDからソフトバンクペイメントの支払い方法へコンバート
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>ソフトバンクペイメントの支払い方法</returns>
		public static PaymentSBPSTypes.PayMethodTypes? ConvertPaymentIdToPayMethodType(string paymentId)
		{
			switch (paymentId)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS:
					return PaymentSBPSTypes.PayMethodTypes.softbank;

				case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
					return PaymentSBPSTypes.PayMethodTypes.softbank2;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
					return PaymentSBPSTypes.PayMethodTypes.auone;

				case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
					return PaymentSBPSTypes.PayMethodTypes.docomo;

				case Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS:
					return PaymentSBPSTypes.PayMethodTypes.recruit;

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS:
					return PaymentSBPSTypes.PayMethodTypes.paypal;

				case Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS:
					return Constants.PAYMENT_SETTING_SBPS_RAKUTENIDV2_ENABLED ? PaymentSBPSTypes.PayMethodTypes.rakutenv2 : PaymentSBPSTypes.PayMethodTypes.rakuten;

				case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
					return PaymentSBPSTypes.PayMethodTypes.credit3d2;	// クレジットであれば3Dセキュア2.0

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
					return PaymentSBPSTypes.PayMethodTypes.paypay;
			}

			return null;
		}

		/// <summary>
		/// SBPS支払い方法から決済種別IDへコンバート
		/// </summary>
		/// <param name="payMethodType">ソフトバンクペイメントの支払い方法</param>
		/// <returns>決済種別ID（該当無しの場合はnull）</returns>
		public static string ConvertPayMethodTypeToPaymentId(
			PaymentSBPSTypes.PayMethodTypes payMethodType)
		{
			switch (payMethodType)
			{
				case PaymentSBPSTypes.PayMethodTypes.softbank:
					return Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS;

				case PaymentSBPSTypes.PayMethodTypes.softbank2:
					return Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS;

				case PaymentSBPSTypes.PayMethodTypes.docomo:
					return Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS;

				case PaymentSBPSTypes.PayMethodTypes.auone:
					return Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS;

				case PaymentSBPSTypes.PayMethodTypes.recruit:
					return Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS;

				case PaymentSBPSTypes.PayMethodTypes.paypal:
					return Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS;

				case PaymentSBPSTypes.PayMethodTypes.rakuten:
				case PaymentSBPSTypes.PayMethodTypes.rakutenv2:
					return Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS;
				
				case PaymentSBPSTypes.PayMethodTypes.credit:
				case PaymentSBPSTypes.PayMethodTypes.credit3d:
				case PaymentSBPSTypes.PayMethodTypes.credit3d2:
					return Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;

				case PaymentSBPSTypes.PayMethodTypes.paypay:
					return Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY;
			}
			return null;
		}

		/// <summary>
		/// クレジット分割情報取得
		/// </summary>
		/// <param name="installmentsCode">支払い方法（ValueTextの値）、指定なしの場合は一括</param>
		/// <returns>クレジット分割情報</returns>
		public static PaymentSBPSCreditDivideInfo GetCreditDivideInfo(string installmentsCode = null)
		{
			switch (StringUtility.ToEmpty(installmentsCode))
			{
				case "":
				case "1":
					return new PaymentSBPSCreditDivideInfo(PaymentSBPSCreditDivideInfo.DivideTypes.Once);

				case "BONUS1":
					return new PaymentSBPSCreditDivideInfo(PaymentSBPSCreditDivideInfo.DivideTypes.Bonus1);

				case "REVO":
					return new PaymentSBPSCreditDivideInfo(PaymentSBPSCreditDivideInfo.DivideTypes.Revo);

				default:
					int divideTimes = int.Parse(installmentsCode);
					return new PaymentSBPSCreditDivideInfo(PaymentSBPSCreditDivideInfo.DivideTypes.Divide, divideTimes);
			}
		}

		/// <summary>
		/// SBPS Webコンビニタイプからコードへコンバート
		/// </summary>
		/// <param name="webCvsType">SBPS Webコンビニタイプ</param>
		/// <returns>Webコンビニタイプのコード</returns>
		public static string ConvertWebCvsTypeToCode(
			PaymentSBPSTypes.WebCvsTypes webCvsType)
		{
			switch (webCvsType)
			{
				case PaymentSBPSTypes.WebCvsTypes.SevenEleven:
					return "001";

				case PaymentSBPSTypes.WebCvsTypes.Lowson:
					return "002";

				case PaymentSBPSTypes.WebCvsTypes.MiniStop:
					return "005";

				case PaymentSBPSTypes.WebCvsTypes.DailyYamazaki:
					return "010";

				case PaymentSBPSTypes.WebCvsTypes.FamilyMart:
					return "016";

				case PaymentSBPSTypes.WebCvsTypes.Seicomart:
					return "018";
			}
			return null;
		}

		/// <summary>
		/// クレジットトークン期限切れか判定
		/// </summary>
		/// <param name="resErrorCode">エラーコード</param>
		/// <returns>期限切れか</returns>
		public static bool IsCreditTokenExpired(string resErrorCode)
		{
			if (string.IsNullOrEmpty(resErrorCode)) return false;
			return resErrorCode.Substring(3, 2) == "T2";
		}

		/// <summary>
		/// SBPS決済の顧客ID作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>SBPS決済の顧客ID</returns>
		public static string CreateCustCode(string userId)
		{
			return ("reg_sps" + userId);
		}
	}
}
