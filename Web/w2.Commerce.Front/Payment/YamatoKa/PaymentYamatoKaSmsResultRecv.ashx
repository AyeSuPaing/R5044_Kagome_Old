<%--
=========================================================================================================
  Module      : ヤマト決済(後払い)SMS認証結果POST通知取得ハンドラ(PaymentYamatoKaSmsResultRecv.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="PaymentYamatoKaSmsResultRecv" %>
using w2.App.Common.Order.Payment.YamatoKa;

using System.Web;

public class PaymentYamatoKaSmsResultRecv : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";

		(new PaymentYamatoKaSmsResultReceiver(context.Request)).Exec();
	}
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}