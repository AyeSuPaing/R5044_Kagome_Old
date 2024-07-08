<%--
=========================================================================================================
  Module      : トラッカー出力ユーザコントローラ(AccessLogTrackerScript.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/AccessLogTrackerScript.ascx.cs" Inherits="Form_Common_AccessLogTrackerScript" %>

<!-- w2tracker -->
<%-- トラッカー読み込み --%>
<div id="divTracker" runat="server" visible="false">
<script type='text/javascript'>
<!--
	var w2accesslog_account_id = "<%= Constants.W2MP_ACCESSLOG_ACCOUNT_ID %>";
	var w2accesslog_target_domain = "<%= Constants.W2MP_ACCESSLOG_TARGET_DOMAIN %>";
	var w2accesslog_cookie_root = "<%= Constants.PATH_ROOT %>";
	var w2accesslog_getlog_path = "<%= Constants.PATH_ROOT + Constants.W2MP_ACCESSLOG_GETLOG_PATH %>";

	<%-- XMLパーサが特殊文字（「<」「>」）をタグとして認識しないようエスケープしておく（XHTMLに対応） --%>
	document.write(unescape("%3Csc" + "ript type='text/javascript' src='" + (("https:" == document.location.protocol) ? "https:" : "http:") + "//<%= Constants.SITE_DOMAIN + Constants.PATH_ROOT + Constants.W2MP_ACCESSLOG_TRACKER_PATH %>'%3E%3C/sc" + "ript%3E"));
// -->
</script>
</div>

<%-- シルバーエッグリクエスト用タグ --%>
<% if ((Constants.RECOMMEND_OPTION_ENABLED) && (Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg)){ %>
	<script>
		(function () {
			var s = document.createElement('script');
			s.type = 'text/javascript';
			s.async = true;
			s.src = '//<%= Constants.RECOMMEND_SILVEREGG_API_DOMAIN %>/suite/page?m=<%= Constants.RECOMMEND_SILVEREGG_MERCHANT_ID %>&p=<%= SilvereggAigentRecommend.GetRecommendPageId() %>&cookie=<%= w2.App.Common.Util.UserCookieManager.UniqueUserId %>&t=' + (new Date()).getTime() + '&r=' + escape(document.referrer);
			var e = document.getElementsByTagName('script')[0];
			e.parentNode.insertBefore(s, e);
		})();
	</script>
<%} %>

<%-- GoogleAnaytics解析タグ --%>
<% if ((Constants.GOOGLEANALYTICS_ENABLED) && (Constants.SETTING_PRODUCTION_ENVIRONMENT)
	&& (Request.Url.AbsolutePath.ToLower().Contains((Constants.PAGE_FRONT_ORDER_COMPLETE).ToLower()) == false)) { %>
	<%-- GA4用 --%>
	<script async src="https://www.googletagmanager.com/gtag/js?id=<%= (string.IsNullOrEmpty(Constants.GOOGLEANALYTICS_PROFILE_ID) ? Constants.GOOGLEANALYTICS_MEASUREMENT_ID : Constants.GOOGLEANALYTICS_PROFILE_ID) %>"></script>
	<script type="text/javascript">
		window.dataLayer = window.dataLayer || [];
		function gtag() { dataLayer.push(arguments); }
		gtag('js', new Date());
		<% if (string.IsNullOrEmpty(Constants.GOOGLEANALYTICS_PROFILE_ID) == false) { %>
			gtag('config', '<%= Constants.GOOGLEANALYTICS_PROFILE_ID %>');
		<% } %>
		gtag('config', '<%= Constants.GOOGLEANALYTICS_MEASUREMENT_ID %>');
	</script>
<% } %>

<%-- ログを取得 --%>
<div id="divGetLog" runat="server" visible="false">
<script type='text/javascript'>
<!--
	getlog();
// -->
</script>
</div>
<%-- ログを取得（ログイン用）--%>
<div id="divGetLogForLogin" runat="server" visible="false">
<script type='text/javascript'>
<!--
	getlog_for_login("<%= this.LoginUserId %>");
// -->
</script>
</div>
<%-- ログを取得（退会用）--%>
<div id="divGetLogForLeave" runat="server" visible="false">
<script type='text/javascript'>
<!--
	getlog_for_leave("<%= Session[Constants.SESSION_KEY_W2MP_ACCESSLOG_LOGIN_USER_ID] %>");
// -->
</script>
</div>
