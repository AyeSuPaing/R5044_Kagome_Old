<%--
=========================================================================================================
  Module      : セッションタイムアウト更新ページ(UpdateSessionTimeout.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" EnableSessionState="False" ContentType="application/javascript" %>
<%-- 定期的に自分自身を取得してセッションを維持する --%>
setInterval(function () {
	var http = new XMLHttpRequest();
	http.open("GET", "<%= Request.Url.AbsolutePath %>");
	http.send(null);
}, 5 * 60 * 1000);
<%--
	EnableSessionState では、
		True: 排他取得/期限更新
		ReadOnly: 読み取り専用取得/期限更新
		False: 期限更新
	が行われるため、期限更新をする場合はFalseでよい
--%>
