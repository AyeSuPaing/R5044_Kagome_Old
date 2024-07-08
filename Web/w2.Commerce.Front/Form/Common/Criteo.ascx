<%--
=========================================================================================================
  Module      : Criteoタグ出力コントローラ(Criteo.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Criteo.ascx.cs" Inherits="Form_Common_Criteo" %>

<% if ((Constants.CRITEO_OPTION_ENABLED) && (Constants.SETTING_PRODUCTION_ENVIRONMENT)) { %>
<script type="text/javascript" src="//dynamic.criteo.com/js/ld/ld.js?a=<%= this.Account %>" async="true"></script>
<script type="text/javascript">
window.criteo_q = window.criteo_q || [];
var deviceType = /iPad/.test(navigator.userAgent) ? "t" : /Mobile|iP(hone|od)|Android|BlackBerry|IEMobile|Silk/.test(navigator.userAgent) ? "m" : "d";
window.criteo_q.push(
	{ event: "setAccount", account: <%= this.Account %> },
	{ event: "setEmail", email: "<%= this.HashedEmailSha256 %>", hash_method: "sha256" },
	{ event: "setSiteType", type: deviceType },
	<%= this.MainTag %>
);
</script>
<%} %>
