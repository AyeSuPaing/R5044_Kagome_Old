<%--
=========================================================================================================
  Module      : キャプチャ認証コントローラ(Captcha.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="CaptchaControl" %>
<% if (string.IsNullOrEmpty(Constants.RECAPTCHA_SITE_KEY) == false){ %>
<style>
.text-xs-center {
	padding-top:10pt;
	text-align: center;
}
.g-recaptcha {
	display: inline-block;
}
</style>
<script type="text/javascript">
<!--
	// キャプチャ認証前はボタン非表示にする
	$(function () {
		$('#<%= this.EnabledControlClientID %>').css('visibility','hidden');
	});

	// キャプチャ認証callbackイベント
	function recaptchaCallback(response)
	{
		if (response != '')
		{
			//alert(response);
			$('#<%= this.EnabledControlClientID %>').css('visibility','');
			if ( __doPostBack ) { __doPostBack('<%= lbSave.UniqueID %>',''); }
		}
	}

	// タイムアウト時の処理
	function recaptchaTimeOutCallback() {
		$('#<%= this.EnabledControlClientID %>').css('visibility', 'hidden');
	}
	//-->
</script>
<%-- UPDATEPANELによりヘッダメニューが動作しないバグ対応 --%>
<script type="text/javascript" language="javascript">
	function bodyPageLoad()
	{
		if ( Sys.WebForms == null ) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if ( isAsyncPostback )
		{
			$( function ()
			{
				$( '#HeadGlobalNavi ul > li.onMenu' ).hover(
					function ()
					{
						$( ".HeadGNaviList:not(:animated)", this ).slideDown( "fast" );
						$( this ).addClass( 'active' );
					},
				function ()
				{
					$( ".HeadGNaviList", this ).slideUp( "fast" );
					$( this ).removeClass( 'active' );
				} );
				$( '#HeadRight .hoverMenu' ).hover(
				function ()
				{
					$( this ).children( '.menu' ).stop().slideDown( "fast" );
				},
				function ()
				{
					$( this ).children( '.menu' ).stop().slideUp( "fast" );
				} );
			} );
		}
	}
</script>
<script src="https://www.google.com/recaptcha/api.js?hl=jp"></script>
<div class="text-xs-center">
<div class="g-recaptcha" data-callback="recaptchaCallback" data-expired-callback="recaptchaTimeOutCallback" data-sitekey="<%= Constants.RECAPTCHA_SITE_KEY %>"></div>
</div>
<%-- 再描画させないため、UpdatePanel利用--%>
<asp:LinkButton ID="lbSave" runat="server" OnClick="lbSave_Click" />
<asp:UpdatePanel ID="upSave" runat="server">
	<ContentTemplate>
	</ContentTemplate>
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="lbSave" EventName="Click" />
	</Triggers>
</asp:UpdatePanel>
<% } %>
