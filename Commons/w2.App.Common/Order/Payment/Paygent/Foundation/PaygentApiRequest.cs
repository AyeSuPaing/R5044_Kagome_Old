/*
=========================================================================================================
  Module      : Paygent API 共通リクエストクラス(PaygentApiRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using jp.co.ks.merchanttool.connectmodule.system;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.Paygent.Foundation
{
	/// <summary>
	/// Paygent API 共通リクエストクラス
	/// </summary>
	internal class PaygentApiRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiName">API名</param>
		/// <param name="parameters">リクエストパラメータ</param>
		public PaygentApiRequest(
			string apiName,
			Dictionary<string, string> parameters)
		{
			this.ApiName = apiName;
			this.Parameters = parameters;
			this.PaygentConnModule = new PaygentB2BModule();
			foreach (var pair in parameters)
			{
				this.PaygentConnModule.reqPut(pair.Key, pair.Value);
			}
		}

		/// <summary>
		/// POSTレスポンス取得
		/// </summary>
		/// <returns>レスポンス</returns>
		public Dictionary<string, string> Post()
		{
			try
			{
				this.PaygentConnModule.post();
				var responseDataset = (Hashtable)this.PaygentConnModule.resNext();
				var result = AppendResultValues(responseDataset);
				return result;
			}
			catch (Exception ex)
			{
				var logMessage = string.Format(
					"{0} : 送信パラメータ : {1}",
					this.ApiName,
					JsonConvert.SerializeObject(this.Parameters));
				FileLogger.WriteError(logMessage, ex);
				throw new PaygentException(ex.Message, ex);
			}
		}

		/// <summary>
		/// ペイジェント接続モジュールの結果を表す値をレスポンスデータセットに追加する。
		/// </summary>
		/// <param name="responseDataset">ペイジェント接続モジュールレスポンスデータセット</param>
		/// <returns>ペイジェント処理の結果データセット</returns>
		/// <remarks>ペイジェント接続モジュールの構造上、結果を表す値をこのクラスのPostメソッドが返却する辞書型値に含める必要がある。</remarks>
		private Dictionary<string, string> AppendResultValues(Hashtable responseDataset)
		{
			var result = responseDataset.Cast<DictionaryEntry>().ToDictionary(
				entry => StringUtility.ToEmpty(entry.Key),
				entry => StringUtility.ToEmpty(entry.Value));
			result.Add(PaygentConstants.PAYGENT_API_RESPONSE_RESULT, this.PaygentConnModule.getResultStatus());
			result.Add(PaygentConstants.PAYGENT_API_RESPONSE_CODE, this.PaygentConnModule.getResponseCode());
			result.Add(PaygentConstants.PAYGENT_API_RESPONSE_DETAIL, this.PaygentConnModule.getResponseDetail());
			return result;
		}

		/// <summary>API名</summary>
		public string ApiName { get; private set; }
		/// <summary>リクエストパラメータ</summary>
		public Dictionary<string, string> Parameters { get; private set; }
		/// <summary>ペイジェント接続モジュール</summary>
		private PaygentB2BModule PaygentConnModule { get; set; }
	}
}
