<%--
=========================================================================================================
  Module      : ログインページ(Default.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" Title="ログインページ" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
		<title>
			<%if (this.IsW2C) {%><%: Constants.APPLICATION_NAME_DISP %><%} %>
			<%else if (this.IsRepeatPlus) {%>W2Repeat<%} %>
			<%else if (this.IsRepeatFood) {%>W2Repeat Food<%} %>
			<%else {%><%: Constants.MANAGER_DESIGN_SETTING %><%} %>
			&nbsp;
			<%if (string.IsNullOrEmpty(this.Page.Title) == false) {%><%: this.Title %><%} %>
			<%else {%><%: MenuUtility.GetTitle(Request.FilePath) %><%} %>
		</title>
		<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
		<link rel="stylesheet" href="<%: this.ResolveClientUrl("~/Css/w2style.css?") + Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>" media="screen,print" type="text/css" />
		<link rel="stylesheet" href="<%: this.ResolveClientUrl("~/Css/w2style_ec_v2.css?") + Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>" media="screen,print" type="text/css" />
		<link rel="stylesheet" href="Images/Icon/icomoon/style.css" media="screen,print" type="text/css" />
		<link rel="stylesheet" href="~/Css/w2style_responsive.css" media="screen,print" type="text/css" />
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<style type="text/css">
			html,body {
				overflow: hidden;
			}
			@media screen and (max-width: 1023px) and (min-width: 331px) {
				.container .form-group {
					font-size: 25px;
					width: 800px !important;
				}

				.container .form-group input {
					height: 60px !important;
					font-size: 25px !important;
				}

				#btnExit {
					width: 200px !important;
				}

				#loader {
					width:6% !important;
					display: block;
					left:47% !important;
					top: 47% !important;
				}

				.modal-content-wrapper {
					width: 90% !important;
				}
			}

			.container .form-group {
				margin-top:10px;
			}

			.container {
				padding: 20px;
			}

			.modal-content {
				border-radius: 5px;
			}

			.modal-option-inner {
				margin: 20px;
			}

			.modal.modal-size-s .modal-content-wrapper {
				width: 500px;
			}

			.container .form-group input {
				margin: 0px;
			}

			label {
				cursor: default;
			}

			#btnConfirmCode {
				background: #3a6ba6;
				color: #fff;
				border-radius: 5px;
				padding: 6px 20px;
				line-height: 1.1;
				border: 1px solid #3a6ba6;
				outline: none;
				cursor: pointer;
				font-size: 14px;
				box-shadow: 0px 0px 3px 0px;
				vertical-align: middle;
				width: auto !important;
			}

			#btnConfirmCode:hover {
				opacity: 0.8;
			}

			#authenticationCode {
				border: 1px solid #999;
				padding: 5px 3px;
				height: 32px;
				line-height: 32px;
				border-radius: 5px;
				cursor: pointer;
				font-size: 14px;
				vertical-align: middle;
				box-sizing: border-box;
			}

			#btnExit {
				background: #184377;
				background-color: #989494;
				border-color: #989494;
				border-radius: 5px;
				font-size: 14px;
				margin-left: auto;
				display: block;
				width: 126px;
			}

			#btnExit:hover {
				opacity: 0.8;
			}

			#loader {
				width:2%;
				display: block;
				left:49%;
				top: 49%;
			}

			#background {
				width: 100%;
				height: 100%;
				z-index: 1;
				opacity:0;
				display: none;
			}
		
		</style>
	</head>
	<body onload="window.document.LoginForm.<%: Constants.REQUEST_KEY_MANAGER_LOGIN_ID %>.focus()" class="login <%: this.SiteCssClassName %>">
		<img class="login_ec" src="<%= Constants.PATH_ROOT %>Images/Login/login_EC.svg" alt="logo"/>
		<div id="background" style="position: absolute"></div>
		<div id="loader" style="position: absolute">
			<img src="<%= Constants.PATH_ROOT %>Images/Common/loading.gif" alt="Loading" style="width: 100%" />
		</div>
		<div class="login-box">	
			<div class="login-package-title">
				<h1 class="package-logo">
					<%if (this.IsW2C) {%>
					<img src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/W2_Unified_logo.svg" alt="logo"/>
					<%} else if (this.IsRepeatPlus) {%>
					<img src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/W2_Repeat_logo.svg" alt="logo"/>
					<%} else if (this.IsRepeatFood) {%>
					<img src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/W2_RepeatFood_logo.svg" alt="logo"/>
					<%} else {%>
					<img src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/logo_login.png" alt="logo"/>
					<%} %>
					<div class="login-site-label">
						<%if (string.IsNullOrEmpty(Constants.MANAGER_DESIGN_DECORATE_ICON_FILENAME) == false) {%><span class="icon"><img src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/<%: Constants.MANAGER_DESIGN_DECORATE_ICON_FILENAME %>" alt="" /></span><%} %>
						<%if (string.IsNullOrEmpty(Constants.MANAGER_DESIGN_DECORATE_MESSAGE) == false) {%><span class="label"><%: Constants.MANAGER_DESIGN_DECORATE_MESSAGE %></span><%} %>
					</div>
				</h1>
			</div>
			<div class="login-input-box">
					<img class="login_foam" src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/W2_login_left_01.svg" alt="logo"/>
					<img class="login_foam" style="float:right;" src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/W2_login_left_02.svg" alt="logo"/>	
				<h2 class="login-hed">管理画面ログイン</h2>
				<form id="loginForm" name="LoginForm" method="post" action="<%: Constants.PROTOCOL_HTTPS + Request.Url.Authority + Constants.PATH_ROOT + Constants.PAGE_MANAGER_LOGIN %>">
					<div class="error-message-wrapper">
						 <span class="notice" id="spErrorMessage" runat="server"></span>
					</div>
					<div class="login-form-input">
						<label>
							<img class="login_icon" src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/W2_login_id.svg" alt="logo"/>
							<input type="text" id="<%: Constants.REQUEST_KEY_MANAGER_LOGIN_ID %>" name="<%: Constants.REQUEST_KEY_MANAGER_LOGIN_ID %>" value="<%: Request[Constants.REQUEST_KEY_MANAGER_LOGIN_ID] %>" placeholder="ログインID" />
						</label>
						<label>
							<img  class="login_icon" src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/W2_login_password.svg" alt="logo"/>
							<input type="password" id="<%: Constants.REQUEST_KEY_MANAGER_PASSWORD %>" name="<%: Constants.REQUEST_KEY_MANAGER_PASSWORD %>" placeholder="パスワード" />
						</label>
					</div>
					<div class="login-form-btn">
						<input type="button" id="btnSubmit" value="ログイン" class='<%= this.IsW2C  ? "Unified" : "Repeat" %>' />
					</div>
					<input type="hidden" name="<%: Constants.REQUEST_KEY_MANAGER_LOGIN_FLG %>" value="1" />
					<input type="hidden" name="<%: Constants.REQUEST_KEY_MANAGER_NEXTURL %>" value="<%: Request[Constants.REQUEST_KEY_MANAGER_NEXTURL] %>" />
					<input type="hidden" name="<%: Constants.REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG %>" value="<%: Request[Constants.REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG] %>" />
				</form>
					<img class="login_foam" src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/W2_login_right_01.svg" alt="logo"/>
					<img class="login_foam" style="float:right;" src="<%= Constants.PATH_ROOT %>Images/Common/<%: this.ManagerDesingSettingDirName %>/W2_login_right_02.svg" alt="logo"/>
			</div>
			<%if (this.IsW2Product) {%>
			<p class="copyright">&copy; W2 Co.,Ltd.</p>
			<%} %>
		</div>
		<!-- 2-step authentication modal -->
		<div class="modal-content hide">
			<div id="2fa-modal">
				<div class="modal-option">
					<div class="modal-option-inner modal-option-form-added">
						<div class="container">
							<div class="form-group">
								<label class="col-form-label-lg">オペレータ情報に登録されたメールアドレスに送信しました</label>
							</div>
							<div class="form-group">
								<label>認証メールに表示されている認証コードを入力してください</label>
							</div>
							<div class="form-group">
								<label class="form-check-label">
									認証コード
								</label>
							</div>
							<div class="form-group">
								<input id="authenticationCode" type="text" maxlength="8" style="width: 100% !important;" />
							</div>
							<div class="form-group">
								<span id="spModalErrorMessage" class="notice"></span>
							</div>
							<div class="form-group">
								<input id="btnConfirmCode" type="button" value="ログイン" style="background-color: #045cee; border-color: #045cee; border-radius: 5px; width: 100% !important;" />
							</div>
							<div class="form-group">
								<a id="btnResendCode" href="#" class="pull-left item">認証メールを再送信</a>
							</div>
							<div class="form-group">
								<input id="btnExit" type="button" onclick="$('.modal').hide();" value="サイトに戻る" />
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		<!-- 2-step authentication modal -->
		<script type="text/javascript" charset="Shift_JIS" src="<%= this.ResolveClientUrl("~/Js/jquery-3.3.1.min.js") %>"></script>
		<script type="text/javascript" charset="Shift_JIS" src="<%= this.ResolveClientUrl("~/Js/cookie-utils.js") %>"></script>
		<script type="text/javascript" charset="Shift_JIS" src="<%: this.ResolveClientUrl("~/Js/common.js?") + Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
		<script type="text/javascript" charset="Shift_JIS" src="<%= this.ResolveClientUrl("~/Js/fixed_midashi.js") %>"></script>
		<script type="text/javascript" src="Js/Manager.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
		<script type="text/javascript">
			var isSubmitting = false;
			var isAuthenticating = false;
			var isResendingCode = false;

			$("#<%: Constants.REQUEST_KEY_MANAGER_LOGIN_ID %>").keypress(function (e) {
				var key = e.which;
				if (key == "<%: Constants.CONST_ENTER_KEYCODE %>") {
					login();
					return false;
				}
			});

			$("#<%: Constants.REQUEST_KEY_MANAGER_PASSWORD %>").keypress(function (e) {
				var key = e.which;
				if (key == "<%: Constants.CONST_ENTER_KEYCODE %>") {
					login();
					return false;
				}
			});

			$("#authenticationCode").keypress(function (e) {
				var key = e.which;
				if (key == "<%: Constants.CONST_ENTER_KEYCODE %>") {
					$("#btnConfirmCode").click();
					return false;
				}
			});

			$("#btnSubmit").click(function (e) {
				if (isSubmitting) return;

				$.ajax({
					type: "POST",
					url: "Default.aspx/Login",
					contentType: "application/json; charset=utf-8",
					data: JSON.stringify({
						loginId: $("#" + "<%: Constants.REQUEST_KEY_MANAGER_LOGIN_ID %>").val(),
						password: $("#" + "<%: Constants.REQUEST_KEY_MANAGER_PASSWORD %>").val()
					}),
					dataType: "json",
					beforeSend: function () {
						isSubmitting = true;
						showLoader();
					},
					success: function (result) {
						hideLoader();

						switch (result.d) {
							case "<%: ShopOperatorLoginManager.GetLoginResult(ShopOperatorLoginManager.LoginStage.Failed) %>":
							case "<%: ShopOperatorLoginManager.GetLoginResult(ShopOperatorLoginManager.LoginStage.Normal) %>":
								submitForm();
								break;

							case "<%: ShopOperatorLoginManager.GetLoginResult(ShopOperatorLoginManager.LoginStage.With2StepAuthentication) %>":
								$("#spModalErrorMessage").load(" #spModalErrorMessage");
								$("#authenticationCode").val('');
								var size = innerWidth > 980 ? "modal-size-s" : "modal-size-m";
								modal.open("#2fa-modal", size);
								break;

							default:
								$("#spErrorMessage").html(result.d);
								break;
						}
					},
					error: pageReload,
					complete: function () {
						isSubmitting = false;
					}
				});
			});

			$("#btnConfirmCode").click(function (e) {
				if (isAuthenticating) return;

				$.ajax({
					type: "POST",
					url: "Default.aspx/Authentication",
					contentType: "application/json; charset=utf-8",
					data: JSON.stringify({
						loginId: $("#" + "<%: Constants.REQUEST_KEY_MANAGER_LOGIN_ID %>").val(),
						authenticationCode: $("#authenticationCode").val()
					}),
					dataType: "json",
					beforeSend: function () {
						isAuthenticating = true;
					},
					success: function (result) {
						switch (result.d) {
							case "<%: ShopOperatorLoginManager.GetLoginResult(ShopOperatorLoginManager.LoginStage.With2StepAuthentication) %>":
								submitForm();
								break;

							default:
								$("#spModalErrorMessage").html(result.d);
								break;
						}
					},
					error: pageReload,
					complete: function () {
						isAuthenticating = false;
					}
				});
			});

			$("#btnResendCode").click(function () {
				if (isResendingCode) return;

				$.ajax({
					type: "POST",
					url: "Default.aspx/ResendCode",
					contentType: "application/json; charset=utf-8",
					data: JSON.stringify({
						loginId: $("#" + "<%: Constants.REQUEST_KEY_MANAGER_LOGIN_ID %>").val()
					}),
					dataType: "json",
					beforeSend: function () {
						isResendingCode = true;
					},
					success: function (result) {
						if (result.d) {
							$("#spModalErrorMessage").html(result.d);
						} else {
							$("#spModalErrorMessage").html("<%= WebMessages.GetMessages(WebMessages.ERRMSG_RESEND_AUTHENTICATION_CODE_SUCCESS) %>");
						}
					},
					error: pageReload,
					complete: function () {
						isResendingCode = false;
					}
				});
			});

			function submitForm() {
				$("#loginForm").submit();
			}

			function login() {
				$("#btnSubmit").click();
			}

			function hideLoader() {
				$("#loader").hide();
				$(".login-box").css("opacity", "1");
				$("#background").css("display", "none");
			}

			function showLoader() {
				$("#loader").show();
				$(".login-box").css("opacity", "0.4");
				$("#background").css("display", "block");
			}

			$(window).ready(hideLoader);

			// Page reload
			function pageReload(xmlHttpRequest, status, error) {
				if (xmlHttpRequest.status == 401) {
					window.location.reload();
				}
			}
		</script>
	</body>
</html>
