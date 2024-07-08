<%@ Control Language="C#" ClassName="HeaderScriptDeclaration" Inherits="BaseUserControl" %>

<%-- w2標準js --%>
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/w2.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/w2.textboxevents.js"></script>
<%-- jQuery --%>
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/jquery-3.6.0.min.js"></script>
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/jquery-migrate-3.3.2.min.js"></script>
<%-- etc --%>
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/AC_ActiveX.js"></script>
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/AC_RunActiveContent.js"></script>
<%-- slick --%>
<link href="<%= Constants.PATH_ROOT %>Js/Slick/slick-theme.css" rel="stylesheet" type="text/css">
<link href="<%= Constants.PATH_ROOT %>Js/Slick/slick.css" rel="stylesheet" type="text/css">
<script type="text/javascript" src="<%= Constants.PATH_ROOT %>Js/Slick/slick.min.js"></script>
<%-- クライアント検証用モジュール --%>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %><%= Constants.PAGE_FRONT_VALIDATE_SCRIPT %>"></script>

<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/jquery.bxslider.min.js"></script>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/jquery.biggerlink.min.js"></script>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/jquery.heightLine.js"></script>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/jquery.mousewheel.js"></script>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/jquery.jscrollpane.min.js"></script>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/jquery.ah-placeholder.js"></script>

<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/function.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>

<script type="text/javascript" src="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Js/AutoKana/jquery.autoKana.js") %>"></script>
<script type="text/javascript" src="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Js/AutoKana/jquery.autoChangeKana.js") %>"></script>

<script type="text/javascript">
	<%-- 氏名（姓・名）の自動振り仮名変換を実行する --%>
	function execAutoKanaWithKanaType(firstName, firstNameKana, lastName, lastNameKana) {
		execAutoKana(
			firstName,
			firstNameKana,
			lastName,
			lastNameKana,
			'<%: ReplaceTag("@@User.name_kana.type@@") %>');
	}

	<%-- フリガナ（姓・名）の自動かな←→カナ変換を実行する --%>
	function execAutoChangeKanaWithKanaType(firstNameKana, lastNameKana) {
		execAutoChangeKana(
			firstNameKana,
			lastNameKana,
			'<%: ReplaceTag("@@User.name_kana.type@@") %>');
	}
</script>
<%-- 検索結果レイヤー用スタイル --%>
<link id="lsearchResultLayerCss" rel="stylesheet" href="<%= Constants.PATH_ROOT %>Css/searchResultLayer.css" type="text/css" media="screen" />

<%if (SmartPhoneUtility.CheckSmartPhoneSite(Request.Path) == false) { %>
<%-- SmartPhoneでない場合 --%>
<%} else {%>
<%-- SmartPhoneの場合 --%>
	<%-- iUi --%>
	<link rel="apple-touch-icon" href="<%: SmartPhoneUtility.GetSmartPhoneContentsUrl("Contents/iui/iui-logo-touch-icon.png") %>" />
	<meta name="apple-touch-fullscreen" content="YES" />
	<!--script type="application/x-javascript" src="<%: SmartPhoneUtility.GetSmartPhoneContentsUrl("Contents/iui/iui.js") %>"></script-->
	<link id="liuiCss" rel="stylesheet" href="<%: SmartPhoneUtility.GetSmartPhoneContentsUrl("Contents/iui/iui.css") %>" type="text/css" media="screen" />
	<link id="lspCss" rel="stylesheet" href="<%: SmartPhoneUtility.GetSmartPhoneContentsUrl("Css/sp.css") %>" type="text/css" media="screen" />
<%} %>

<script type="text/javascript">
<!--
	// ページロード処理
	function pageLoad(sender, args) {
		// ページロード共通処理
		pageLoad_common();

		// function.js実行
		if (typeof initializeFunctionJs == "function") initializeFunctionJs();

		// グローバル切り替え実行
		if (typeof switchGlobalFunction == 'function') switchGlobalFunction();

		// PayPal
		if (typeof InitializePaypal == "function") InitializePaypal(sender, args);

		// body側のpageLoad実行
		if (typeof bodyPageLoad == "function") bodyPageLoad();
	}
//-->
</script>