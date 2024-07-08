<%--
=========================================================================================================
  Module      : 認証コード送信用モーダル(SendAuthenticationCodeModal.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SendAuthenticationCodeModal.ascx.cs" Inherits="Form_Common_Order_SendAuthenticationCodeModal" %>
<div id="modal">
	<div id="modal_content">
		<asp:HiddenField ID="hfUserId" runat="server" />

		<%-- UPDATE PANEL開始 --%>
		<asp:UpdatePanel ID="upUpdatePanel" runat="server">
			<ContentTemplate>
				<div id="dvSelectAuthenticationReceiver">
					<p class="title">認証方法の選択</p>
					<div class="modal_detail modal_text">
						<asp:RadioButton ID="rbSendMail" GroupName="selectAuthentication" Text="メールアドレス認証" Checked="true" runat="server" /><br />
						<% if (Constants.GLOBAL_SMS_OPTION_ENABLED) { %>
						<asp:RadioButton ID="rbSendSms" GroupName="selectAuthentication" Text="SMS認証" runat="server" /><br />
						<% } %>
					</div>
					<div id="dSendAuthenticationCodeErrorMessage" class="fred"></div>
					<div class="modal_detail"><a href="javascript:clickSendAuthenticationCode();" class="btn-org btn-large btn-org-blk">認証コードを送信する</a></div>
				</div>
				<div id="dvInputAuthenticationCode" style="display: none">
					<p class="title">認証コード入力</p>
					<div class="modal_detail modal_text">
						認証コードを送信しました。<br />
						受け取った認証コードを入力してください。<br />
						<p id="pAuthenticationCodeReceiver"></p>
					</div>
					<asp:TextBox ID="tbAuthenticationCode" Style="width: 120px" placeholder="6桁のコードを入力" MaxLength="6" runat="server" />
					<div id="dCheckAuthenticationCodeErrorMessage" class="fred"></div>
					<div class="modal_detail"><a href="javascript:clickCheckAuthenticationCode();" class="btn-org btn-large btn-org-blk">認証する</a></div>
					<div><a href="javascript:clickResendAuthenticationCode();">認証コードを再送</a></div>
					<p id="pResendMessage" class="toolTip">
						<span>認証コードを再送信しました。</span>
					</p>
				</div>
			</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで --%>

		<div class="btn_close_wrap"><a href="javascript:void(0);" class="btn_close">キャンセル</a></div>
	</div>
</div>
<script type="text/javascript">
	$(function () {
		var modal = $('#modal'),
			modalContent = $('#modal_content'),
			btnClose = $(".btn_close");

		$(modal).on('click', function (event) {
			if ((!($(event.target).closest(modalContent).length) || ($(event.target).closest(btnClose).length))) {
				location.reload();
			}
		});
	});

	<%-- モーダルウィンドウ表示 --%>
	function openWindowSendAuthenticationCodeModal(userId) {
		openAuthenticationFlg = 1;
		$('#<%= this.WhfUserId.ClientID %>').val(userId);
		var modal = $('#modal');
		modal.show();
	}

	<%-- 表示場所切り替え --%>
	function showSelect(isSelect) {
		$('#dSendAuthenticationCodeErrorMessage').html('');
		$('#dCheckAuthenticationCodeErrorMessage').html('');
		var select = $('#dvSelectAuthenticationReceiver');
		var input = $('#dvInputAuthenticationCode');
		if (isSelect) {
			select.show();
			input.hide();
		}
		else {
			select.hide();
			input.show();
		}
	}

	<%-- 認証コード送信クリック --%>
	function clickSendAuthenticationCode() {
		sendAuthenticationCode(false);
	}

	<%-- 認証コード再送信クリック --%>
	function clickResendAuthenticationCode() {
		sendAuthenticationCode(true);
	}

	<%-- 認証コード送信 --%>
	function sendAuthenticationCode(isResend) {
		var isSendMail = document.getElementById('<%= this.WrbSendMail.ClientID %>').checked;
		var userId = $('#<%= this.WhfUserId.ClientID %>').val();

		$.ajax({
			type: "POST",
			url: webMethodUrl + "/SendAuthenticationCode",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				userId: userId,
				isSendMail: isSendMail
			}),
			async: false,
			success: function (response) {
				var data = JSON.parse(response.d);
				if (data.Result) {
					$('#pAuthenticationCodeReceiver').html('認証コード送信先：' + data.AuthenticationCodeReceiver);
					showSelect(false);
					if (isResend) {
						showTooltipMessage();
					}
				} else {
					var errorMessageControlId = (isResend) ? '#dCheckAuthenticationCodeErrorMessage' : '#dSendAuthenticationCodeErrorMessage';
					$(errorMessageControlId).html(data.Message);
				}
			}
		});
	}

	function showTooltipMessage() {
		showTooltip();
	}

	<%-- 認証コードチェック --%>
	function clickCheckAuthenticationCode() {
		var authenticationCode = $('#<%= this.WtbAuthenticationCode.ClientID %>').val();
		var userId = $('#<%= this.WhfUserId.ClientID %>').val();

		$('#dCheckAuthenticationCodeErrorMessage').html('');

		$.ajax({
			type: "POST",
			url: webMethodUrl + "/CheckAuthenticationCode",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				userId: userId,
				authenticationCode: authenticationCode
			}),
			async: false,
			success: function (response) {
				var data = JSON.parse(response.d);
				if (data.Result) {
					var modal = $('#modal');
					modal.hide();
					openAuthenticationFlg = 0;
					authenticationOk = true;
				} else {
					$('#dCheckAuthenticationCodeErrorMessage').html(data.Message);
				}
			}
		});
	}
</script>
