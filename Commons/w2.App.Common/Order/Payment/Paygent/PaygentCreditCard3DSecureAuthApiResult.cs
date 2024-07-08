/*
=========================================================================================================
  Module      : ペイジェントクレカ3DS応答電文 (PaygentCreditCard3DSecureAuthApiResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.Paygent
{
	/// <summary>
	/// ペイジェントクレカ3DS応答電文
	/// </summary>
	public class PaygentCreditCard3DSecureAuthApiResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">処理結果</param>
		/// <param name="auth3dsId">3Dセキュア認証ID</param>
		/// <param name="cardBrand">クレジットカードブランド</param>
		/// <param name="issurClass">イシュア区分</param>
		/// <param name="acqName">取扱カード会社名</param>
		/// <param name="acqId">取扱カード会社コード</param>
		/// <param name="issurName">カード発行会社名</param>
		/// <param name="issurId">カード発行会社コード</param>
		/// <param name="hash">ハッシュ値</param>
		/// <param name="fingerprint">フィンガープリント</param>
		/// <param name="maskedCardNumber">マスクされたカード番号</param>
		/// <param name="requestor3DsecureErrorCode">3Dセキュアリクエスターエラーコード</param>
		/// <param name="server3DsecureErrorCode">3Dセキュアサーバーエラーコード</param>
		/// <param name="attemptKbn">Attempt区分</param>
		/// <param name="responseCode">レスポンスコード</param>
		/// <param name="responseDetail">処理結果</param>
		public PaygentCreditCard3DSecureAuthApiResult(
			string result,
			string auth3dsId,
			string cardBrand,
			string issurClass,
			string acqName,
			string acqId,
			string issurName,
			string issurId,
			string hash,
			string fingerprint,
			string maskedCardNumber,
			string requestor3DsecureErrorCode,
			string server3DsecureErrorCode,
			string attemptKbn,
			string responseCode,
			string responseDetail)

		{
			this.Result = result;
			this.Auth3dsId = auth3dsId;
			this.CardBrand = cardBrand;
			this.IssurClass = issurClass;
			this.AcqName = acqName;
			this.AcqId = acqId;
			this.IssurName = issurName;
			this.IssurId = issurId;
			this.Hc = hash;
			this.Fingerprint = fingerprint;
			this.MaskedCardNumber = maskedCardNumber;
			this.Requestor3DsecureErrorCode = requestor3DsecureErrorCode;
			this.Server3DsecureErrorCode = server3DsecureErrorCode;
			this.AttemptKbn = attemptKbn;
			this.ResponseCode = responseCode;
			this.ResponseDetail = responseDetail;
		}

		/// <summary>1:処理結果</summary>
		public string Result { get; private set; }
		/// <summary>2:3Dセキュア認証ID</summary>
		public string Auth3dsId { get; private set; }
		/// <summary>3:クレジットカードブランド</summary>
		public string CardBrand { get; private set; }
		/// <summary>4:イシュア区分</summary>
		public string IssurClass { get; private set; }
		/// <summary>5:取扱カード会社名</summary>
		public string AcqName { get; private set; }
		/// <summary>6:取扱カード会社コード</summary>
		public string AcqId { get; private set; }
		/// <summary>7:カード発行会社名</summary>
		public string IssurName { get; private set; }
		/// <summary>8:カード発行会社コード</summary>
		public string IssurId { get; private set; }
		/// <summary>9:ハッシュ値</summary>
		public string Hc { get; private set; }
		/// <summary>10:フィンガープリント</summary>
		public string Fingerprint { get; private set; }
		/// <summary>11:マスクされたカード番号</summary>
		public string MaskedCardNumber { get; private set; }
		/// <summary>12:3Dセキュアリクエスターエラーコード</summary>
		public string Requestor3DsecureErrorCode { get; private set; }
		/// <summary>13:3Dセキュアサーバーエラーコード</summary>
		public string Server3DsecureErrorCode { get; private set; }
		/// <summary>14:Attempt区分</summary>
		public string AttemptKbn { get; private set; }
		/// <summary>15:レスポンスコード</summary>
		public string ResponseCode { get; private set; }
		/// <summary>16:処理結果</summary>
		public string ResponseDetail { get; private set; }
	}
}
