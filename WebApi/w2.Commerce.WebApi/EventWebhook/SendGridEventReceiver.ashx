<%--
=========================================================================================================
  Module      : SendGridエラーイベント受取ハンドラ(SendGridEventReceiver.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="SendGridEventReceiver" %>

using System.IO;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using w2.Common.Logger;
using w2.Domain.MailErrorAddr;

/// <summary>
/// SendGridエラーイベント受取ハンドラクラス
/// </summary>
public class SendGridEventReceiver : IHttpHandler 
{
	/// <summary>
	/// リクエスト処理
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest (HttpContext context) {
		var response = GetResponse(context);

		var eventDatas = JsonConvert.DeserializeObject<SendGridEventDataModel[]>(response);
		if (eventDatas == null) return;
		foreach (var eventData in eventDatas)
		{
			var point = 0;
			var error = new StringBuilder();
			switch (eventData.Event)
			{
				// メールサーバによる受信拒否（以後必ずdropped）
				case "bounce":
				// SendGridによるメール破棄
				case "dropped":
					point = 5;
					error.AppendFormat("{0} {1} {2}pt {3} ",eventData.Email, eventData.Event,point ,eventData.SgEventId);
					break;

				default:
					point = 0;
					break;
			}

			if (point == 0) continue;

			// エラーポイント追加
			new MailErrorAddrService().AddErrorPoint(eventData.Email.Replace("\"", ""), point);
			FileLogger.Write("SendGridEvent", error.ToString());
		}
	}

	/// <summary>
	/// レスポンス取得
	/// </summary>
	/// <param name="context"></param>
	/// <returns></returns>
	private string GetResponse(HttpContext context)
	{
		using (var reader = new StreamReader(context.Request.GetBufferedInputStream()))
		{
			var response = reader.ReadToEnd();
			return response;
		}
	}

	/// <summary>
	/// ハンドラインスタンス再利用可否
	/// </summary>
	public bool IsReusable {
		get {
			return false;
		}
	}
}