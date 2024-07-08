/*
=========================================================================================================
  Module      : PaymentGmo(PaymentGmo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.GMO
{
	public class PaymentGmo : IPayment
	{
		#region 定数
		//------------------------------------------------------
		// パラメータ
		//------------------------------------------------------
		/// <summary>GMOから割り当てられたサイトID</summary>
		public const string PARAM_SITEID = "SiteID";
		/// <summary>GMOから割り当てられたサイトパスワード</summary>
		public const string PARAM_SITEPASS = "SitePass";
		/// <summary>GMOから割り当てられたショップID</summary>
		public const string PARAM_SHOPID = "ShopID";
		/// <summary>GMOから割り当てられたショップパスワード</summary>
		public const string PARAM_SHOPPASS = "ShopPass";
		/// <summary>オーダーID</summary>
		public const string PARAM_ORDERID = "OrderID";
		/// <summary>処理区分（有効性チェック、即時売上など）</summary>
		public const string PARAM_JOBCD = "JobCd";
		/// <summary>利用金額</summary>
		public const string PARAM_AMOUNT = "Amount";
		/// <summary>本人認証フラグ</summary>
		public const string PARAM_TDFLAG = "TdFlag";
		/// <summary>店舗名</summary>
		public const string PARAM_TDTENANTNAME = "TdTenantName";
		/// <summary>本人認証フラグ</summary>
		public const string PARAM_TDS2TYPE = "Tds2Type";
		/// <summary>取引ID</summary>
		public const string PARAM_ACCESSID = "AccessID";
		/// <summary>取引パスワード</summary>
		public const string PARAM_ACCESSPASS = "AccessPass";
		/// <summary>支払方法（一括、分割など）</summary>
		public const string PARAM_METHOD = "Method";
		/// <summary>支払回数</summary>
		public const string PARAM_PAYTIMES = "PayTimes";
		/// <summary>カード番号</summary>
		public const string PARAM_CARDNO = "CardNo";
		/// <summary>有効期限（YYMM)</summary>
		public const string PARAM_EXPIRE = "Expire";
		/// <summary>名義人</summary>
		public const string PARAM_HOLDERNAME = "HolderName";
		/// <summary>セキュリティーコード</summary>
		public const string PARAM_SECURITYCODE = "SecurityCode";
		/// <summary>会員ID</summary>
		public const string PARAM_MEMBERID = "MemberID";
		/// <summary>会員名</summary>
		public const string PARAM_MEMBERNAME = "MemberName";
		/// <summary>カード登録連番モード</summary>
		public const string PARAM_SEQMODE = "SeqMode";
		/// <summary>カード登録連番</summary>
		public const string PARAM_CARDSEQ = "CardSeq";
		/// <summary>MD5ハッシュ</summary>
		public const string PARAM_CHECKSTRING = "CheckString";
		/// <summary>エラーコード</summary>
		public const string PARAM_ERRCODE = "ErrCode";
		/// <summary>エラー詳細コード</summary>
		public const string PARAM_ERRINFO = "ErrInfo";
		/// <summary>削除フラグ</summary>
		public const string PARAM_DELETEFLAG = "DeleteFlag";
		/// <summary>トークン</summary>
		public const string PARAM_TOKEN = "Token";
		/// <summary>Convenience code</summary>
		public const string PARAM_CONVENIENCE = "Convenience";
		/// <summary>Customer name</summary>
		public const string PARAM_CUSTOMER_NAME = "CustomerName";
		/// <summary>Customer name Kana</summary>
		public const string PARAM_CUSTOMER_NAME_KANA = "CustomerKana";
		/// <summary>Customer tel no</summary>
		public const string PARAM_CUSTOMER_TEL_NO = "TelNo";
		/// <summary>Payment term day</summary>
		public const string PARAM_PAYMENT_TERM_DAY = "PaymentTermDay";
		/// <summary>Receipts disp 11</summary>
		public const string PARAM_RECEIPTS_DISP_11 = "ReceiptsDisp11";
		/// <summary>Receipts disp 12</summary>
		public const string PARAM_RECEIPTS_DISP_12 = "ReceiptsDisp12";
		/// <summary>Receipts disp 13</summary>
		public const string PARAM_RECEIPTS_DISP_13 = "ReceiptsDisp13";
		/// <summary>Status</summary>
		public const string PARAM_STATUS = "Status";
		/// <summary>Conf no</summary>
		public const string PARAM_CONF_NO = "ConfNo";
		/// <summary>Receipt no</summary>
		public const string PARAM_RECEIPT_NO = "ReceiptNo";
		/// <summary>Payment term</summary>
		public const string PARAM_PAYMENT_TERM = "PaymentTerm";
		/// <summary>Tran date</summary>
		public const string PARAM_TRAN_DATE = "TranDate";
		/// <summary>Receipt url</summary>
		public const string PARAM_RECEIPT_URL = "ReceiptUrl";
		/// <summary>Check string</summary>
		public const string PARAM_CHECK_STRING = "CheckString";
		/// <summary>ACS呼出判定</summary>
		public const string PARAM_ACS = "ACS";
		/// <summary>加盟店戻りURL</summary>
		public const string PARAM_RET_URL = "RetUrl";
		/// <summary>3DS2.0開始 URL</summary>
		public const string PARAM_REDIRECT_URL = "RedirectUrl";
		/// <summary>本人認証パスワード入力画面URL</summary>
		public const string PARAM_ACS_URL = "ACSUrl";
		/// <summary>本人認証要求電文</summary>
		public const string PARAM_PA_REQ = "PaReq";
		/// <summary>本人認証サービス結果</summary>
		public const string PARAM_PA_RES = "PaRes";
		/// <summary>取引ID(本人認証用)</summary>
		public const string PARAM_MD = "MD";

		// ACS定数
		public const string RESPONSE_ACS_AUTH = "0";
		public const string RESPONSE_ACS_TDS1 = "1";
		public const string RESPONSE_ACS_TDS2 = "2";

		/// <summary>キー：戻りURL</summary>
		public const string RETURN_URL = "GMO_3DS_RETURN_URL";
		#endregion

		#region Property
		/// <summary>サーバURL</summary>
		public string ServerUrl { get; set; }
		/// <summary>サイトID</summary>
		public string SiteId { get; set; }
		/// <summary>サイトパスワード</summary>
		public string SitePass { get; set; }
		/// <summary>ショップID</summary>
		public string ShopId { get; set; }
		/// <summary>ショップパスワード</summary>
		public string ShopPass { get; set; }
		/// <summary>店舗名</summary>
		public string ShopName { get; set; }
		/// <summary>3DS2.0未対応時取り扱い</summary>
		public string Tds2Type { get; set; }
		/// <summary>取引ID</summary>
		public string AccessId { get; set; }
		/// <summary>取引パスワード</summary>
		public string AccessPass { get; set; }
		/// <summary>決済方法</summary>
		public string JobCd { get; set; }
		/// <summary>Status</summary>
		public string Status { get; set; }
		/// <summary>Conf no</summary>
		public string ConfNo { get; set; }
		/// <summary>Receipt no</summary>
		public string ReceiptNo { get; set; }
		/// <summary>オンライン決済番号</summary>
		protected string OnlinePaymentId
		{
			get
			{
				return (string.IsNullOrEmpty(this.ReceiptNo) && this.Result.IsSuccess)
					? string.Empty
					: this.ReceiptNo.Insert(4, "-");
			}
		}
		/// <summary>Payment term</summary>
		public string PaymentTerm { get; set; }
		/// <summary>Receipt url</summary>
		public string ReceiptUrl { get; set; }
		/// <summary>ACS呼出判定</summary>
		public string Acs { get; set; }
		/// <summary>3DSサーバーへのリダイレクトURL</summary>
		public string RedirectUrl { get; set; }
		/// <summary>本人認証パスワード入力画面URL</summary>
		public string AcsUrl { get; set; }
		/// <summary>本人認証要求電文</summary>
		public string PaReq { get; set; }
		/// <summary>取引ID(3Dセキュア用)</summary>
		public string Md { get; set; }
		/// <summary>結果メッセージHTML</summary>
		public string ResultMessageCsvHtml { get; set; }
		/// <summary>結果メッセージTEXT</summary>
		public string ResultMessageCsvText { get; set; }
		/// <summary>結果</summary>
		public ResponseResult Result { get; set; }
		/// <summary>エラーメッセージ </summary>
		public string ErrorMessages { get; set; }
		/// <summary>エラー種別コード(3~5桁目)</summary>
		public string ErrorTypeCode { get; set; }

		/// <summary>通常オーソリ</summary>
		public bool IsAuth
		{
			get { return this.Acs == RESPONSE_ACS_AUTH; }
		}
		/// <summary>3Dセキュア1.0</summary>
		public bool IsTds1
		{
			get { return this.Acs == RESPONSE_ACS_TDS1; }
		}
		/// <summary>3Dセキュア2.0</summary>
		public bool IsTds2
		{
			get { return this.Acs == RESPONSE_ACS_TDS2; }
		}
		#endregion
	}
}
