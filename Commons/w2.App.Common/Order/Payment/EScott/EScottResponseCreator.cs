/*
=========================================================================================================
  Module      : ソニーペイメントe-SCOTTE レスポンス生成(EScottResponseCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Order.Payment.EScott.DataSchema;
using w2.App.Common.Order.Payment.EScott.Helper;

namespace w2.App.Common.Order.Payment.EScott
{
	/// <summary>
	/// e-SCOTTE レスポンス生成クラス
	/// </summary>
	public class EScottResponseCreator
	{
		/// <summary>
		/// APIのレスポンス生成
		/// </summary>
		/// <param name="body">リクエスト本文</param>
		/// <returns>APIレスポンス分</returns>
		public string CreateApiResponse(string body)
		{
			var request = EScottHelper.SplitResponse(body);
			var response = new Dictionary<string, string>();

			switch (request[EScottConstants.OPERATE_ID])
			{
				case EScottConstants.OPERATOR_ID_MASTER_1AUTH:
				case EScottConstants.OPERATOR_ID_MASTER_1GATHERING:
					response = CreateMasterApiResponse(request);
					break;
				case EScottConstants.OPERATOR_ID_PROCESS_1CAPTURE:
				case EScottConstants.OPERATOR_ID_PROCESS_1CHANGE:
				case EScottConstants.OPERATOR_ID_PROCESS_1DELETE:
				case EScottConstants.OPERATOR_ID_PROCESS_1REAUTH:
					response = CreateProcessApiResponse(request);
					break;
				case EScottConstants.OPERATOR_ID_MEMBER_4MEMADD:
					response = CreateMemAddApiResponse(request);
					break;
				case EScottConstants.OPERATOR_ID_MEMBER_4MEMREF:
					response = CreateMemRefApiResponse(request);
					break;
				case EScottConstants.OPERATOR_ID_MEMBER_4MEMDEL:
				case EScottConstants.OPERATOR_ID_MEMBER_4MEMINVAL:
					response = CreateMemDelApiResponse(request);
					break;
				default:
					throw new Exception("オペレータIDが不正です。" + request[EScottConstants.OPERATE_ID]);
			}

			var result = JoinEScottRespose(response);
			return result;
		}

		/// <summary>
		/// マスター電文レスポンス生成
		/// </summary>
		/// <param name="request">リクエスト内容</param>
		/// <returns>レスポンス</returns>
		private Dictionary<string, string> CreateMasterApiResponse(Dictionary<string, string> request)
		{
			var response = new Dictionary<string, string>();
			response.Add(EScottConstants.TRANSACTION_ID, "100");
			response.Add(EScottConstants.TRANSACTION_DATE, DateTime.Now.ToString());
			response.Add(EScottConstants.OPERATE_ID, request[EScottConstants.OPERATE_ID]);
			response.Add(EScottConstants.MERCHANT_FREE1, request[EScottConstants.MERCHANT_FREE1]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE2)) response.Add(EScottConstants.MERCHANT_FREE2, request[EScottConstants.MERCHANT_FREE2]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE3)) response.Add(EScottConstants.MERCHANT_FREE3, request[EScottConstants.MERCHANT_FREE3]);
			response.Add(EScottConstants.PROCESS_ID, "c571e0dc5470bc37b6fbea679885a987");
			response.Add(EScottConstants.PROCESS_PASS, "280cfd9c31de02466db96a8eff04ba5b");
			response.Add(EScottConstants.RESPONSE_CD, EScottConstants.REQUEST_APPROVED);
			response.Add(EScottConstants.COMPANY_CD, string.Empty);
			response.Add(EScottConstants.APPROVE_NO, "1000");
			response.Add(EScottConstants.MC_SEC_CD, EScottConstants.MS_SEC_CD_MATCH);
			return response;
		}

		/// <summary>
		/// 会員登録電文レスポンス生成
		/// </summary>
		/// <param name="request">リクエスト内容</param>
		/// <returns>レスポンス</returns>
		private Dictionary<string, string> CreateMemAddApiResponse(Dictionary<string, string> request)
		{
			var response = new Dictionary<string, string>();
			response.Add(EScottConstants.TRANSACTION_ID, "100");
			response.Add(EScottConstants.TRANSACTION_DATE, DateTime.Now.ToString());
			response.Add(EScottConstants.OPERATE_ID, request[EScottConstants.OPERATE_ID]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE1)) response.Add(EScottConstants.MERCHANT_FREE1, request[EScottConstants.MERCHANT_FREE1]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE2)) response.Add(EScottConstants.MERCHANT_FREE2, request[EScottConstants.MERCHANT_FREE2]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE3)) response.Add(EScottConstants.MERCHANT_FREE3, request[EScottConstants.MERCHANT_FREE3]);
			response.Add(EScottConstants.KAIIN_ID, request[EScottConstants.KAIIN_ID]);
			response.Add(EScottConstants.RESPONSE_CD, EScottConstants.REQUEST_APPROVED);
			response.Add(EScottConstants.MC_SEC_CD, EScottConstants.MS_SEC_CD_MATCH);
			return response;
		}

		/// <summary>
		/// 会員削除電文レスポンス生成
		/// </summary>
		/// <param name="request">リクエスト内容</param>
		/// <returns>レスポンス</returns>
		private Dictionary<string, string> CreateMemDelApiResponse(Dictionary<string, string> request)
		{
			var response = new Dictionary<string, string>();
			response.Add(EScottConstants.TRANSACTION_ID, "100");
			response.Add(EScottConstants.TRANSACTION_DATE, DateTime.Now.ToString());
			response.Add(EScottConstants.OPERATE_ID, request[EScottConstants.OPERATE_ID]);
			response.Add(EScottConstants.MERCHANT_FREE1, request[EScottConstants.MERCHANT_FREE1]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE2)) response.Add(EScottConstants.MERCHANT_FREE2, request[EScottConstants.MERCHANT_FREE2]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE3)) response.Add(EScottConstants.MERCHANT_FREE3, request[EScottConstants.MERCHANT_FREE3]);
			response.Add(EScottConstants.RESPONSE_CD, EScottConstants.REQUEST_APPROVED);
			return response;
		}

		/// <summary>
		/// 会員参照電文レスポンス生成
		/// </summary>
		/// <param name="request">リクエスト内容</param>
		/// <returns>レスポンス</returns>
		private Dictionary<string, string> CreateMemRefApiResponse(Dictionary<string, string> request)
		{
			var response = new Dictionary<string, string>();
			response.Add(EScottConstants.TRANSACTION_ID, "100");
			response.Add(EScottConstants.TRANSACTION_DATE, DateTime.Now.ToString());
			response.Add(EScottConstants.OPERATE_ID, request[EScottConstants.OPERATE_ID]);
			response.Add(EScottConstants.MERCHANT_FREE1, request[EScottConstants.MERCHANT_FREE1]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE2)) response.Add(EScottConstants.MERCHANT_FREE2, request[EScottConstants.MERCHANT_FREE2]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE3)) response.Add(EScottConstants.MERCHANT_FREE3, request[EScottConstants.MERCHANT_FREE3]);
			response.Add(EScottConstants.KAIIN_ID, request[EScottConstants.KAIIN_ID]);
			response.Add(EScottConstants.CARD_NO, "1234***8910");
			response.Add(EScottConstants.CARD_EXP, "25/12");
			return response;
		}

		/// <summary>
		/// プロセス電文レスポンス生成
		/// </summary>
		/// <param name="request">リクエスト内容</param>
		/// <returns>レスポンス</returns>
		private Dictionary<string, string> CreateProcessApiResponse(Dictionary<string, string> request)
		{
			var response = new Dictionary<string, string>();
			response.Add(EScottConstants.TRANSACTION_ID, "100");
			response.Add(EScottConstants.TRANSACTION_DATE, DateTime.Now.ToString());
			response.Add(EScottConstants.OPERATE_ID, request[EScottConstants.OPERATE_ID]);
			response.Add(EScottConstants.MERCHANT_FREE1, request[EScottConstants.MERCHANT_FREE1]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE2)) response.Add(EScottConstants.MERCHANT_FREE2, request[EScottConstants.MERCHANT_FREE2]);
			if (request.ContainsKey(EScottConstants.MERCHANT_FREE3)) response.Add(EScottConstants.MERCHANT_FREE3, request[EScottConstants.MERCHANT_FREE3]);
			response.Add(EScottConstants.PROCESS_ID, "c571e0dc5470bc37b6fbea679885a987");
			response.Add(EScottConstants.PROCESS_PASS, "280cfd9c31de02466db96a8eff04ba5b");
			response.Add(EScottConstants.RESPONSE_CD, EScottConstants.REQUEST_APPROVED);
			return response;
		}

		/// <summary>
		/// レスポンス文生成
		/// </summary>
		/// <param name="response">レスポンス内容内容</param>
		/// <returns>レスポンス</returns>
		private string JoinEScottRespose(Dictionary<string, string> response)
		{
			return string.Join("&", response.Select(str => str.Key + "=" + str.Value));
		}
	}
}
