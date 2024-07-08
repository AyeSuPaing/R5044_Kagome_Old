<%--
=========================================================================================================
  Module      : シングルサインオン画面(SingleSignOn.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/MiniApp/Form/Common/MiniApp.master" AutoEventWireup="true" CodeFile="SingleSignOn.aspx.cs" Inherits="MiniApp_SingleSignOn" Title="シングルサインオン画面" %>
<%@ Import Namespace="w2.Common.Logger" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
	<asp:HiddenField ID="hfLineUserId" runat="server" />
	
	<%-- ▽隠しボタン▽ --%>
	<div style="display: none;">
		<asp:Button ID="btnLogin" OnClick="btnLogin_OnClick" runat="server" />
		<asp:Button ID="btnError" OnClick="btnError_OnClick" runat="server" />
	</div>
	<%-- △隠しボタン△ --%>

	<% if (this.IsDevelop == false) { %>
	<script>
		liff.init({
			liffId: '<%= this.LineLiffId %>',
			withLoginOnExternalBrowser: true,
		}).then(() => {
			$.post({
				url: "https://api.line.me/oauth2/v2.1/verify",
				timeout: 10000,
				headers: {
					'Content-Type': 'application/x-www-form-urlencoded',
				},
				dataType: 'json',
				data: { 'id_token': liff.getIDToken(), 'client_id': '<%= this.LineClientId %>' },
			}).done(function (response) {
				$('#<%= hfLineUserId.ClientID %>').val(response.sub);
				$('#<%= btnLogin.ClientID %>').click();
			}).fail(function (xhr) {
				var body = $.parseJSON(xhr.responseText);
				$.post({
					url: '<%= Constants.PATH_ROOT + Constants.HANDLER_LINE_MINIAPP_LOG_EXPORTER %>',
					timeout: 10000,
					headers: {
						'Content-Type': 'application/json',
					},
					dataType: 'json',
					data: JSON.stringify({
						'log_type': '<%= FileLogger.LOGKBN_WARN %>',
						'message': '[LINEユーザー情報取得] ' + body['error_description'],
					})
				}).always(function () {
					$('#<%= btnError.ClientID %>').click();
				});
			});
		}).catch((error) => {
			$.post({
				url: '<%= Constants.PATH_ROOT + Constants.HANDLER_LINE_MINIAPP_LOG_EXPORTER %>',
				timeout: 10000,
				headers: {
					'Content-Type': 'application/json',
				},
				dataType: 'json',
				data: JSON.stringify({
					'log_type': '<%= FileLogger.LOGKBN_WARN %>',
					'message': '[LIFF初期化] ' + error,
				})
			}).always(function () {
				$('#<%= btnError.ClientID %>').click();
			});
		});
	</script>
	<% } %>

	<%-- 開発環境の場合のみデバッグフローで実行 --%>
	<% if (this.IsDevelop) { %>
	<script>
		$('#<%= btnLogin.ClientID %>').click();
	</script>
	<% } %>
</asp:Content>