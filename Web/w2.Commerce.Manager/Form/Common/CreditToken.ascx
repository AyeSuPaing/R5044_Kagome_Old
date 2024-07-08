<%--
=========================================================================================================
  Module      : クレジットTokenユーザーコントロール(CreditToken.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="w2.App.Common.Order" %>

<%if (OrderCommon.CreditTokenUse) {%>
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/w2.credit.token.aspx?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
<%} %>

<%-- ▼▼ZEUSトークン取得用のスクリプト▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Zeus) { %>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/CredtTokenZeus.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
<script type="text/javascript">
	zeusClientIp = '<%= SessionManager.UsePaymentTabletZeus ? Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP_OFFLINE : Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP %>';
</script>
<% } %>
<%-- ▲▲ZEUSトークン取得用のスクリプト▲▲ --%>
	