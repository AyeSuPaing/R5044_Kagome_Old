<%--
=========================================================================================================
  Module      : インシデント警告アイコン表示切替時間テーブル取得ハンドラ(IncidentWarningIconTermTable.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="IncidentWarningIconTermTable" %>

using System.Linq;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;
using w2.Domain.CsIncidentWarningIcon;

/// <summary>
/// インシデント警告アイコン表示切替時間テーブル取得ハンドラクラス
/// </summary>
public class IncidentWarningIconTermTable : BasePage, IHttpHandler, IRequiresSessionState
{
	/// <summary>
	/// リクエスト処理
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest (HttpContext context)
	{
		if (this.LoginOperatorDeptId == null)
		{
			context.Response.Write(JsonConvert.SerializeObject(false));
		}
		var models = new CsIncidentWarningIconService()
			.GetByOperatorId(this.LoginOperatorDeptId, this.LoginOperatorId);
		context.Response.ContentType = "application/json";

		// 期間テーブルの登録がない場合Falseを返す
		if (models == null)
		{
			context.Response.Write(JsonConvert.SerializeObject(false));
			return;
		}
		var termTable = models
			.OrderBy(i => i.WarningLevel)
			.GroupBy(i => i.IncidentStatus)
			.ToDictionary(i => i.Key, i => i.Select(j => j.Term).ToList());

		context.Response.Write(JsonConvert.SerializeObject(termTable));
	}

	/// <summary>ハンドラインスタンス再利用可否</summary>
	public bool IsReusable
	{
		get { return true; }
	}
}