/*
=========================================================================================================
  Module      : 基底ページヘルパ(BasePageHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using w2.Common.Logger;

/// <summary>
/// 基底ページヘルパ
/// </summary>
public class BasePageHelper
{
	/// <summary>クレジットカード番号フォーム入力可能か（クレジットカード番号フォーム入力可能か（トークン決済or永久トークン決済））</summary>
	public static bool CanUseCreditCardNoForm
	{
		get
		{
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
			{
				return SessionManager.UsePaymentTabletZeus;
			}
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS) return true;
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo) return false;
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc) return false;
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott) return false;
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent) return false;
			return true;	// 他に影響を与えないようにする
		}
	}

	/// <summary>
	/// Convert object to json string
	/// </summary>
	/// <param name="data">Data</param>
	/// <returns>Json string</returns>
	public static string ConvertObjectToJsonString(object data)
	{
		try
		{
			var result = JsonConvert.SerializeObject(data);
			return result;
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);
			return string.Empty;
		}
	}

	/// <summary>
	/// Deserialize JSON object
	/// <typeparam name="TResult">A type of JSON object</typeparam>
	/// <param name="data">Data</param>
	/// <returns>An JSON object</returns>
	public static TResult DeserializeJsonObject<TResult>(string data)
		where TResult : new()
	{
		try
		{
			return JsonConvert.DeserializeObject<TResult>(data);
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);
			return new TResult();
		}
	}
}
