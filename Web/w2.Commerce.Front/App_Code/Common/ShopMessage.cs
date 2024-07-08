/*
=========================================================================================================
  Module      : ショップメッセージクラス(ShopMessage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.ShopMessage;

///*********************************************************************************************
/// <summary>
/// ショップメッセージクラス
/// </summary>
///*********************************************************************************************
public class ShopMessage
{
	/// <summary>
	/// メッセージ取得
	/// </summary>
	/// <param name="messageType">メッセージタイプ</param>
	/// <returns>メッセージ</returns>
	public static string GetMessage(string messageType)
	{
		var message = ShopMessageUtil.GetMessage(messageType);
		return message;
	}

	/// <summary>
	/// 改行をbrタグにしてメッセージ取得
	/// </summary>
	/// <param name="messageType">メッセージタイプ</param>
	/// <returns>メッセージ</returns>
	public static string GetMessageHtmlEncodeChangeToBr(string messageType)
	{
		var message = WebSanitizer.HtmlEncodeChangeToBr(GetMessage(messageType));
		return message;
	}

	/// <summary>
	/// 決済種別に応じてメッセージ取得
	/// </summary>
	/// <param name="paymentId">決済種別</param>
	/// <returns>メッセージ</returns>
	public static string GetMessageByPaymentId(string paymentId = "")
	{
		var message = ShopMessageUtil.GetMessageByPaymentId(paymentId);
		return message;
	}
}
