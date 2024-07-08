<%--
=========================================================================================================
  Module      : クレジットTokenユーザーコントロール(CreditToken.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" %>

<%if (OrderCommon.CreditTokenUse) {%>
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/w2.credit.token.aspx?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
<%} %>

<%-- ▼▼GMOトークン取得用のスクリプト▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Gmo) { %>
<script type="text/javascript" charset="UTF-8" src="<%=Constants.PAYMENT_CREDIT_GMO_GETTOKEN_JS %>"></script>
<% } %>
<%-- ▲▲GMOトークン取得用のスクリプト▲▲ --%>
<%-- ▼▼SBPSトークン取得用のスクリプト▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.SBPS) { %>
<script type="text/javascript" charset="UTF-8" src="<%=Constants.PAYMENT_SETTING_SBPS_CREDIT_GETTOKEN_JS_URL %>"></script>
<% } %>
<%-- ▲▲SBPSトークン取得用のスクリプト▲▲ --%>
<%-- ▼▼ZEUSトークン取得用のスクリプト▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Zeus) { %>
	<% if (Constants.PAYMENT_SETTING_ZEUS_3DSECURE2) { %>
		<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/CredtTokenZeus2.0.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
	<% }else{ %>
		<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/CredtTokenZeus.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
	<% } %>
<% } %>
<%-- ▲▲ZEUSトークン取得用のスクリプト▲▲ --%>
<%-- ▼▼ヤマトKWCトークン取得用のスクリプト▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.YamatoKwc) { %>
<script type="text/javascript" charset="UTF-8" src="<%=Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_GETTOKEN_JS_URL %>"></script>
<% } %>
<%-- ▲▲ヤマトKWCトークン取得用のスクリプト▲▲ --%>
<%-- ▼▼ソニーペイメントe-SCOTTトークン取得用のスクリプト▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.EScott) { %>
<script type="text/javascript"
	src="<%=Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_GETTOKEN_JS_URL %>?k_TokenNinsyoCode=<%=Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TOKENPAYMENTAUTHCODE %>"
	callBackFunc = "setToken"
	class = "spsvToken">
</script>
<% } %>
<%-- ▲▲ソニーペイメントe-SCOTT取得用のスクリプト▲▲ --%>
<%-- ▼▼ベリトランストークン取得用のスクリプト▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.VeriTrans) { %>
	<script type="text/javascript" charset="UTF-8" src="<%=Constants.PAYMENT_CREDIT_VERITRANS4G_GETTOKEN %>"></script>
<% } %>
<%-- ▲▲ベリトランストークン取得用のスクリプト▲▲ --%>
<%-- ▼▼楽天カードトークン取得用のスクリプト▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Rakuten) { %>
	<script type="text/javascript" charset="UTF-8" src="<%=Constants.PAYMENT_RAKUTEN_CREDIT_GET_TOKEN_JS_URL %>"></script>
<% } %>
<%-- ▲▲楽天カードトークン取得用のスクリプト▲▲ --%>
<%-- ▼▼ペイジェントクレカトークン取得用のスクリプト▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Paygent) { %>
	<script type="text/javascript" charset="UTF-8" src="<%=Constants.PAYMENT_PAYGENT_CREDIT_GETTOKENJSURL %>"></script>
<% } %>
<%-- ▲▲ペイジェントクレカトークン取得用のスクリプト▲▲ --%>
