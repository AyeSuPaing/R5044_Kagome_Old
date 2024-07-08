/*
=========================================================================================================
  Module      : ペイジェントクレカ3DS結果処理クラス (PaygentCreditCard3DSecureResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Security.Cryptography;
using System.Text;
using w2.Common.Helper;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.Paygent
{
	public class PaygentCreditCard3DSecureResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">ペイジェントクレカ3Dセキュア応答電文</param>
		public PaygentCreditCard3DSecureResult(PaygentCreditCard3DSecureAuthApiResult result)
		{
			this.Result = result.Result;
			this.Auth3dsId = result.Auth3dsId;
			this.CardBrand = result.CardBrand;
			this.IssurClass = result.IssurClass;
			this.AcqName = result.AcqName;
			this.AcqId = result.AcqId;
			this.IssurName = result.IssurName;
			this.IssurId = result.IssurId;
			this.Hc = result.Hc;
			this.Fingerprint = result.Fingerprint;
			this.MaskedCardNumber = result.MaskedCardNumber;
			this.Requestor3DsecureErrorCode = result.Requestor3DsecureErrorCode;
			this.Server3DsecureErrorCode = result.Server3DsecureErrorCode;
			this.AttemptKbn = result.AttemptKbn;
			this.ResponseCode = result.ResponseCode;
			this.ResponseDetail = result.ResponseDetail;
		}

		/// <summary>
		/// ハッシュ値改竄チェック
		/// </summary>
		/// <returns>改竄されていなければtrue</returns>
		public bool VerifyChecksum()
		{
			// 結果を元にハッシュ値を作成して、結果の中のハッシュ値と一致すればOK
			var joinnedValues = this.Result + this.Auth3dsId + this.AttemptKbn + Constants.PAYMENT_PAYGENT_CREDIT_RECEIVE_3DSECURERESULT_HASHKEY;
			var bytes = Encoding.UTF8.GetBytes(joinnedValues);
			var numArray = new SHA256CryptoServiceProvider().ComputeHash(bytes);

			var hashedText = new StringBuilder();
			foreach (var num in numArray)
			{
				hashedText.AppendFormat("{0:X2}", num);
			}
			var lowerHashedText = hashedText.ToString().ToLower();

			var result = (this.Hc == lowerHashedText);
			// 失敗時はエラーログを落とす
			if (result == false)
			{
				FileLogger.WriteError("ペイジェントクレジット3Dセキュア2.0認証エラー：パラメータ改ざんエラー");
			}
			return result;
		}

		/// <summary>
		/// 3Dセキュア2.0認証結果をもとに認証する
		/// </summary>
		/// <returns>与信可否 or タイムアウト</returns>
		public string Authorize()
		{
			var resultCode = GetAuthErrorCode();
			var result = IsAuthorized(resultCode);
			return result;
		}

		/// <summary>
		/// レスポンス値を元に認証結果を表す認証エラーコード(クラス内)列挙型を取得
		/// </summary>
		/// <returns>認証エラーコード(クラス内)列挙型</returns>
		private AuthErrorCode GetAuthErrorCode()
		{
			// タイムアウトの場合
			if (IsTimeoutError()) return AuthErrorCode.RejectedDueToTimeoutError;

			// attempt=nullの場合（いずれのセキュリティレベルでもOKとする）
			if (IsSuccessful()) return AuthErrorCode.Authorized;
			// attempt=0の場合（セキュリティ低・中でOKとする）
			if (IsAuthorizedWithLowRisk()) return AuthErrorCode.AuthorizedWithLowRisk;
			// attempt=1の場合（セキュリティ低でOKとする）
			if (IsAuthorizedWithHighRisk()) return AuthErrorCode.AuthorizedWithHighRisk;

			return AuthErrorCode.Rejected;
		}

		/// <summary>
		/// ペイジェントにより認証されたかどうか
		/// </summary>
		/// <returns>ペイジェントにより認証されたかどうか</returns>
		private bool IsSuccessful()
		{
			// 正常終了している場合のみtrue、それ以外はfalseを返す
			if ((this.Result == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS) && string.IsNullOrEmpty(this.AttemptKbn)) return true;

			// Attempt制御が発生した場合、resultが"0"でなく、responseCodeが空となるケースがある
			// その場合、純粋な成功ではないため、ひとまず失敗とみなす
			if (string.IsNullOrEmpty(this.ResponseCode)) return false;

			//レスポンスコードを列挙体へパース
			if (EnumHelper.TryParseToEnum<PaygentConstants.PaygentResponseErrorCode>(this.ResponseCode, out var codeParsedIntoEnum)
				== false)
			{
				// 想定外のレスポンスコードである場合は、その旨をログに残して失敗とみなす。
				FileLogger.WriteWarn($"ペイジェントクレジットカード決済で想定外のレスポンスコードが返されました。responseCode={this.ResponseCode}");
				return false;
			}

			return false;
		}

		/// <summary>
		/// 3Dセキュア2.0認証タイムアウトか
		/// </summary>
		/// <returns>3Dセキュア2.0認証タイムアウトか</returns>
		private bool IsTimeoutError()
		{
			var result = (this.Result == PaygentConstants.PAYGENT_RESPONSE_STATUS_FAILURE)
				&& (this.ResponseCode == PaygentConstants.PaygentResponseErrorCode.N31013.ToText());
			return result;
		}

		/// <summary>
		/// 3Dセキュア認証の結果と設定値をもとに後続処理を実行するか
		/// </summary>
		/// <param name="authErrorCode">認証エラーコード(クラス内)列挙型</param>
		/// <returns>与信可否 or タイムアウト</returns>
		private string IsAuthorized(AuthErrorCode authErrorCode)
		{
			switch (authErrorCode)
			{
				case AuthErrorCode.Authorized:
				case AuthErrorCode.AuthorizedWithLowRisk:
				case AuthErrorCode.AuthorizedWithHighRisk:
					return PaygentConstants.PAYGENT_3DSECURE_RESULT_OK;

				case AuthErrorCode.Rejected:
				case AuthErrorCode.RejectedDueToHighRiskTx:
					return PaygentConstants.PAYGENT_3DSECURE_RESULT_NG;

				case AuthErrorCode.RejectedDueToTimeoutError:
					return PaygentConstants.PAYGENT_3DSECURE_RESULT_TIMEOUT;

				default:
					throw new ArgumentOutOfRangeException("authErrorCode", authErrorCode, null);
			}
		}

		/// <summary>
		/// Attemptが発生した、かつ、ペイジェントにより正常と判断されたかどうか
		/// </summary>
		/// <returns>Attemptが発生した、かつ、ペイジェントにより正常と判断されたかどうか</returns>
		/// <remarks>
		/// カード会社がチャージバックリスク負担<dr />
		/// チャージバックリスク: 売上未回収と商品損失という二重の損失を受けるリスク
		/// </remarks>
		private bool IsAuthorizedWithLowRisk()
		{
			// 正常終了 かつ attemptが0 かつ 設定値が「high」以外で処理実行
			var result = (this.Result == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS)
				&& (this.AttemptKbn == "0")
				&& (Constants.PAYMENT_PAYGENT_CREDIT_AUTHORIZATION_CONTROLKBN != PaygentConstants.AUTHORIZATION_CONTROLKBN_HIGH);
			return result;
		}

		/// <summary>
		/// Attemptが発生した、かつ、ペイジェントにより注意と判断されたかどうか
		/// </summary>
		/// <returns>Attemptが発生した、かつ、ペイジェントにより注意されたかどうか</returns>
		/// <remarks>
		/// 加盟店がチャージバックリスク負担<dr />
		/// チャージバックリスク: 売上未回収と商品損失という二重の損失を受けるリスク
		/// </remarks>
		private bool IsAuthorizedWithHighRisk()
		{
			// 正常終了 かつ attemptが1 かつ 設定値が「low」の場合処理実行
			var result = (this.Result == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS)
				&& (this.AttemptKbn == "1")
				&& (Constants.PAYMENT_PAYGENT_CREDIT_AUTHORIZATION_CONTROLKBN == PaygentConstants.AUTHORIZATION_CONTROLKBN_LOW);
			return result;
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

		/// <summary>
		/// 認証エラーコード(クラス内)列挙型
		/// </summary>
		private enum AuthErrorCode
		{
			/// <summary>認証成功</summary>
			Authorized,
			/// <summary>認証成功 - 3Dセキュア2.0認証警告：Attempt制御発生警告(チャージバックリスク: カード会社負担)</summary>
			AuthorizedWithLowRisk,
			/// <summary>認証成功 - 3Dセキュア2.0認証エラー：3Dセキュアサーバエラーもしくは、チャレンジ認証未実施(チャージバックリスク: 加盟店負担)</summary>
			AuthorizedWithHighRisk,
			/// <summary>認証失敗</summary>
			Rejected,
			/// <summary>認証失敗 - 3Dセキュア2.0認証エラー：3Dセキュアサーバエラーもしくは、チャレンジ認証未実施(チャージバックリスク: 加盟店負担)</summary>
			RejectedDueToHighRiskTx,
			/// <summary>認証失敗 - 認証タイムアウト</summary>
			RejectedDueToTimeoutError,
		}

	}
}
