/*
=========================================================================================================
  Module      : e-SCOTT 与信マスター電文レスポンス(Master1AuthResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Order.Payment.EScott.Helper;

namespace w2.App.Common.Order.Payment.EScott.DataSchema
{
	/// <summary>
	/// 与信マスター電文レスポンス
	/// </summary>
	public class Master1AuthResponse : EScottResponseBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="response">レスポンス</param>
		/// <param name="kaiinId">会員ID</param>
		/// <param name="kaiinPass">会員パスワード</param>
		public Master1AuthResponse(Dictionary<string, string> response, string kaiinId, string kaiinPass)
		{
			TransactionId = response.GetValueOrEmpty(EScottConstants.TRANSACTION_ID);
			TransactionDate = response.GetValueOrEmpty(EScottConstants.TRANSACTION_DATE);
			OperateId = response.GetValueOrEmpty(EScottConstants.OPERATE_ID);
			MerchantFree1 = response.GetValueOrEmpty(EScottConstants.MERCHANT_FREE1);
			MerchantFree2 = response.GetValueOrEmpty(EScottConstants.MERCHANT_FREE2);
			MerchantFree3 = response.GetValueOrEmpty(EScottConstants.MERCHANT_FREE3);
			ProcessId = response.GetValueOrEmpty(EScottConstants.PROCESS_ID);
			ProcessPass = response.GetValueOrEmpty(EScottConstants.PROCESS_PASS);
			ResponseCd = response.GetValueOrEmpty(EScottConstants.RESPONSE_CD);
			CompanyCd = response.GetValueOrEmpty(EScottConstants.COMPANY_CD);
			ApproveNo = response.GetValueOrEmpty(EScottConstants.APPROVE_NO);
			McSecCd = response.GetValueOrEmpty(EScottConstants.MC_SEC_CD);

			CardTranId = GenerateTransactionId(this.ProcessId, this.ProcessPass, kaiinId, kaiinPass);
		}

		/// <summary>処理連番</summary>
		public string TransactionId { get; private set; }
		/// <summary>取引日付</summary>
		public string TransactionDate { get; private set; }
		/// <summary>処理区分</summary>
		public string OperateId { get; private set; }
		/// <summary>貴社自由領域1</summary>
		public string MerchantFree1 { get; private set; }
		/// <summary>貴社自由領域2</summary>
		public string MerchantFree2 { get; private set; }
		/// <summary>貴社自由領域3</summary>
		public string MerchantFree3 { get; private set; }
		/// <summary>プロセスID</summary>
		public string ProcessId { get; private set; }
		/// <summary>プロセスパスワード</summary>
		public string ProcessPass { get; private set; }
		/// <summary>処理結果コード</summary>
		public string ResponseCd { get; private set; }
		/// <summary>カード会社コード</summary>
		public string CompanyCd { get; private set; }
		/// <summary>承認番号</summary>
		public string ApproveNo { get; private set; }
		/// <summary>認証結果1(セキュリティコード認証)</summary>
		public string McSecCd { get; private set; }
		/// <summary>カード番号</summary>
		public string CardTranId { get; private set; }
		/// <summary>成功かどうか</summary>
		public bool IsSuccess
		{
			get { return IsSuccessRequest(this.ResponseCd); }
		}
		/// <summary>レスポンスメッセージ</summary>
		public string ResponseMessage
		{
			get { return GetEScottResponseCodeDetails(ResponseCd); }
		}

	}
}
