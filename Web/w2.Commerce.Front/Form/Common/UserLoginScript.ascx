<%--
=========================================================================================================
  Module      : ユーザーログインスクリプトユーザーコントロール(UserLoginScript.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" %>

<script type="text/javascript">
	<%-- 変数定義：クライアントID --%>
	var loginIdControlId = "";
	var passwordControlId = "";
	var errorMessageControlId = "";
	var loginButtonControlId = "";
	<%-- モーダルウィンドウ表示フラグ --%>
	var openAuthenticationFlg = 0;
	<%-- 認証OKフラグ --%>
	var authenticationOk = false;

	<%-- クライアントIDセット --%>
	function SetOmotionClientId(argLoginIdControlId, argPasswordControlId, argErrorMessageControlId, argLoginButtonControlId) {
		loginIdControlId = argLoginIdControlId;
		passwordControlId = argPasswordControlId;
		errorMessageControlId = argErrorMessageControlId;
		loginButtonControlId = argLoginButtonControlId;
	}

	<%-- ログインボタンクリック --%>
	function onClientClickLogin() {
<%if (Constants.OMOTION_ENABLED == false) {%>
		return true;
<%} else { %>
		eval(setOmotionClientIdJs);

		if (authenticationOk) {
			return true;
		}

		var ipAddress = '<%= Request.UserHostAddress %>';
		var loginId = $('#' + loginIdControlId).val();
		var password = $('#' + passwordControlId).val();

		$('#omotion_req_id').val(loginId);
		var result = true;

		// w2ログインチェック
		$.ajax({
			type: "POST",
			url: webMethodUrl + "/TryLogin",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				ipAddress: ipAddress,
				loginId: loginId,
				password: password
			}),
			async: false,
			success: function (response) {
				var data = JSON.parse(response.d);
				var message = data.Message;
				var userId = data.UserId;

				$('#' + errorMessageControlId).html(message);
				if (message != "") {
					result = false;
				} else {
					result = false;
					// O-MOTIONチェック
					$.ajax({
						type: "POST",
						url: webMethodUrl + "/TryOmotion",
						contentType: "application/json; charset=utf-8",
						dataType: "json",
						data: JSON.stringify({
							loginId: loginId
						}),
						async: false,
						success: function (data) {
							if (!data.d) {
								result = false;
								openWindowSendAuthenticationCodeModal(userId);
								waitForAuthentication(function () {
									$.ajax({
										type: "POST",
										url: webMethodUrl + "/SendOmotionFeedback",
										contentType: "application/json; charset=utf-8",
										dataType: "json",
										data: JSON.stringify({
											loginId: loginId,
											value: authenticationOk
										}),
										async: false,
										success: function (data) {
											var btnLogin = document.getElementById(loginButtonControlId);
											btnLogin.click();
											result = true;
										}
									});
								},
									1000);
							} else {
								result = true;
							}
						}
					});
				}
			}
		});

		return result;
<%} %>
	}

	<%-- 待ち --%>
	function waitForAuthentication(func, ms) {
		setTimeout(function () {
			if (openAuthenticationFlg === 0) {
				func.call();
			} else {
				waitForAuthentication(func, ms);
			}
		}, ms);
	}
</script>
<%if (Constants.OMOTION_ENABLED) {%>
<%if (string.IsNullOrEmpty(Constants.OMOTION_TEST_LOGINID) == false) {%>
<input type="hidden" id="omotion_req_id" value="<%= Constants.OMOTION_TEST_LOGINID %>">
<%} %>
<script type="text/javascript" src="<%= Constants.OMOTION_JS_PATH %>">
<%} %>
</script>
