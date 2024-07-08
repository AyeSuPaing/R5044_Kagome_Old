<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RegisterCardVeriTransMock.aspx.cs" Inherits="Form_PayTg_RegisterCardVeriTransMock" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.PayTg" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title>クレジットカード登録モック</title>
	<script type="text/javascript" charset="Shift_JIS" src="<%= this.ResolveClientUrl("~/Js/jquery-1.6.4.min.js") %>"></script>
	<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/CredtTokenZeus.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
</head>
<body>
	<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
	<div>
		<asp:Literal ID="lMessage" Text="まず、クレジットカード情報を入力してトークン採番をお願い致します" runat="server" /><br />
		<asp:HiddenField ID="hfToken" runat="server" />
	</div>
	<br/>
	<table style="border-width: 0; padding: 0; border-collapse: collapse;">
		<tr>
			<td class="ititlecommon">アカウントID</td>
			<td class="ivaluecommon">
				<asp:Label ID="lbAccountId" runat="server"/>
			</td>
		</tr>
		<tr>
			<td class="ititlecommon">クレジットカード番号</td>
			<td class="ivaluecommon">
				<asp:TextBox ID="cardNumber" runat="server" Text="4111111111111111" maxlength="16"/>
			</td>
		</tr>
		<tr>
			<td class="ititlecommon">有効期限</td>
			<td class="ivaluecommon">
				<asp:TextBox ID="tbExp" runat="server" placeholder="MM/YY" Text="01/30" maxlength="5" size="5"/>
				&nbsp;&nbsp;<span style="font-size: small; color: red;">※形式：MM/YY</span>
			</td>
        </tr>
		<tr>
			<td class="ititlecommon">セキュリティコード</td>
			<td class="ivaluecommon">
				<asp:TextBox ID="csc" runat="server" placeholder="123" Text="123" maxlength="4" size="4"/>
				&nbsp;&nbsp;<span style="font-size: small; color: red;">※必要な場合は入力してください。</span>
			</td>
		</tr>
		<tr>
			<td class="thlToken">トークン</td>
			<td class="thlToken"><asp:Button ID="Button1" runat="server" Text=" トークン採番 " OnClick="btnCreateTokenId_Click" /></td>
		</tr>
		<tr>
			<td class="ititlecommon">トークン採番結果</td>
			<td class="ivaluecommon">
				<asp:Label runat="server" ID="tokenId"></asp:Label>
			</td>
		</tr>
	</table>
	<br/>
	<asp:Button ID="btnRegister" runat="server" Text=" クレジットカード登録 " OnClick="RegisterCard_Click" />

	<script type="text/javascript">
	<!--
		function registerCard(cardNo) {
			getTokenVeriTrans(cardNo);
		}

		function getTokenVeriTrans(cardInfo) {
			var cardInfoList = cardInfo.split('#');

			var data = {
				token_api_key: '<%= Constants.PAYMENT_CREDIT_VERITRANS4G_TOKEN_API_KEY %>',
				card_number: cardInfoList[0],
				card_expire: cardInfoList[1],
				security_code: cardInfoList[2],
				lang: "ja"
			};

			var url = '<%= Constants.PAYMENT_CREDIT_VERITRANS4G_GETTOKEN %>';
			var xhr = new XMLHttpRequest();
			xhr.open('POST', url, true);
			xhr.setRequestHeader('Accept', 'application/json');
			xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
			xhr.addEventListener('loadend', function () {
				if (xhr.status === 0) {
					alert("トークンサーバーとの接続に失敗しました");
					return;
				}
				var response = JSON.parse(xhr.response);
				getTokenCallbackVeriTrans(response);

			});
			xhr.send(JSON.stringify(data));
		}

		function getTokenCallbackVeriTrans(result) {
			var res = result.status;
			if (res === "success") {
				document.getElementById('<%= tokenId.ClientID %>').innerText = result.token;
				document.getElementById('<%= hfToken.ClientID %>').value = result.token;
			} else {
				document.getElementById('<%= tokenId.ClientID %>').innerText = "";
				document.getElementById('<%= hfToken.ClientID %>').value = "";
				alert(result.message);
			}
		}

		function returnResponse(isSuccess) {
			try {
				if (window.opener != null) {
					var res = {
						dataType: "2",
						comResult: '000',
						mstatus: (isSuccess === true) ? 'success' : 'failure',
						vResultCode: '',
						expire: document.getElementById('<%= tbExp.ClientID %>').value,
						reqCardNumber: '**************11',
						merrMsg: (isSuccess === true) ? '' : 'リクエストされたカード情報について決済センターとの確認に失敗しました。',
						procNo: '00001',
						requestDate: '<%= DateTime.Now.ToString("yyyyMMddHHmmss") %>',
						responseDate: '<%= DateTime.Now.ToString("yyyyMMddHHmmss") %>',
						orderId: 'TEST-<%= DateTime.Now.ToString("yyyyMMddHHmmss") %>',
						customerId: '<%= Request[PayTgConstants.PARAM_CUSTOMERID] %>'
					};

					var result = JSON.stringify(res);
					window.opener.getResponseFromMock(result);
				}
			} catch (error) {
				alert(error);
			}
		}
	//-->
	</script>
		
	<style type="text/css">
		td.ititlecommon {
			background-color: #e6e6fa;
			border-color: #ccccff;
			border-width: 1px;
			border-style: solid;
			width: 180px;
			padding: 5px;
			margin: 0px;
		}
		td.ivaluecommon {
			border-color: #ccccff;
			border-width: 1px;
			border-style: solid;
			width: 480px;
			padding: 5px;
			margin: 0px;
			word-break: break-all;
		}
		td.thlToken {
			background-color: #f9f9e0;
			border-color: #ccccff;
			border-width: 1px;
			border-style: solid;
			padding: 0.5em 0.5em 0.5em 5px;
			margin: 0px;
		}
	</style>
	</form>
</body>
</html>
