/*
=========================================================================================================
  Module      : GMO決済モジュールインターフェース(IPaymentGmo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO
{
	/// <summary>
	/// GMO決済モジュールインターフェース
	/// </summary>
	public interface IPaymentGmo : IService
	{
		#region Property
		/// <summary>サーバURL</summary>
		string ServerUrl { get; set; }
		/// <summary>サイトID</summary>
		string SiteId { get; set; }
		/// <summary>サイトパスワード</summary>
		string SitePass { get; set; }
		/// <summary>ショップID</summary>
		string ShopId { get; set; }
		/// <summary>ショップパスワード</summary>
		string ShopPass { get; set; }
		/// <summary>店舗名</summary>
		string ShopName { get; set; }
		/// <summary>3DS2.0未対応時取り扱い</summary>
		string Tds2Type { get; set; }
		/// <summary>取引ID</summary>
		string AccessId { get; set; }
		/// <summary>取引パスワード</summary>
		string AccessPass { get; set; }
		/// <summary>決済方法</summary>
		string JobCd { get; set; }
		/// <summary>Status</summary>
		string Status { get; set; }
		/// <summary>Conf no</summary>
		string ConfNo { get; set; }
		/// <summary>Receipt no</summary>
		string ReceiptNo { get; set; }
		/// <summary>Payment term</summary>
		string PaymentTerm { get; set; }
		/// <summary>Receipt url</summary>
		string ReceiptUrl { get; set; }
		/// <summary>ACS呼出判定</summary>
		string Acs { get; set; }
		/// <summary>3DSサーバーへのリダイレクトURL</summary>
		string RedirectUrl { get; set; }
		/// <summary>本人認証パスワード入力画面URL</summary>
		string AcsUrl { get; set; }
		/// <summary>本人認証要求電文</summary>
		string PaReq { get; set; }
		/// <summary>取引ID(3Dセキュア用)</summary>
		string Md { get; set; }
		/// <summary>結果メッセージHTML</summary>
		string ResultMessageCsvHtml { get; set; }
		/// <summary>結果メッセージTEXT</summary>
		string ResultMessageCsvText { get; set; }
		/// <summary>結果</summary>
		ResponseResult Result { get; set; }
		/// <summary>エラーメッセージ </summary>
		string ErrorMessages { get; set; }
		/// <summary>エラー種別コード(3~5桁目)</summary>
		string ErrorTypeCode { get; set; }
		/// <summary>通常オーソリ</summary>
		bool IsAuth { get; }
		/// <summary>3Dセキュア1.0</summary>
		bool IsTds1 { get; }
		/// <summary>3Dセキュア2.0</summary>
		bool IsTds2 { get; }
		#endregion
	}
}
