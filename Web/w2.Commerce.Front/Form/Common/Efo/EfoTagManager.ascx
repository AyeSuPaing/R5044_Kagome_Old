<%--
=========================================================================================================
  Module      : EFOタグマネージャー(EfoTagManager.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EfoTagManager.ascx.cs" Inherits="Form_Common_Efo_EfoTagManager" %>
<% if (this.IsEfoOptionEnabled){ %>
<!-- GeeeN Tag Manager Start -->
<script type="text/javascript">
	(function(i, g, m, a, h) {
		i[a] = i[a] || [];
		i[a].push({ "geeen_tag_manger.start": new Date().getTime(), event: "js" });
		var k = g.getElementsByTagName(m)[0],
			f = g.createElement(m),
			b = a != "GeeeNData" ? "&l=" + a : "",
			j = encodeURIComponent(window.location.href);
		f.async = true;
		f.src = "https://gntm.geeen.co.jp/Onetag/?id=" + h + "&u=" + j + b;
		k.parentNode.insertBefore(f, k)
	})(window, document, "script", "GeeeNData", '<%= int.Parse(w2.App.Common.Constants.EFO_OPTION_PROJECT_NO) %>');
</script>
<!-- GeeeN Tag Manager End -->
<% } %>
