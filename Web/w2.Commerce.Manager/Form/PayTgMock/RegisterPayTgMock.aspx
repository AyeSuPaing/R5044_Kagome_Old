<%--
=========================================================================================================
  Module      : PayTg：クレジットカード登録モックページ(RegisterPayTgMock.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RegisterPayTgMock.aspx.cs" Inherits="Form_PayTgMock_RegisterPayTgMock"  maintainScrollPositionOnPostBack="true" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.PayTg" %>
<%@ Import Namespace="w2.App.Common.Order" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title>クレジットカード登録モック</title>
	<script type="text/javascript" charset="Shift_JIS" src="<%= this.ResolveClientUrl("~/Js/jquery-1.6.4.min.js") %>"></script>
</head>
<body>
	<form id="form1" runat="server">
		<tbody id="divRakutenCredit" runat="server">
		<tr>
			<th>クレジットカード情報</th>
			<td>
				<div class="container" style ="display:none">
					<h5>カード番号</h5>
					<div id="my-card-mount">
						<iframe src="https://payvault.global.rakuten.com/static/payvault/V7/card-number.html" style="border: none; height: 34px; width: 100%"></iframe>
					</div>
					<h5>有効期限</h5>
					<div id="my-expiration-month-mount">
						<iframe src="https://payvault.global.rakuten.com/static/payvault/V7/expiration-month.html" style="border: none; height: 34px; width: 100%"></iframe>
					</div>
					<span>月</span>
					<div id="my-expiration-year-mount">
						<iframe src="https://payvault.global.rakuten.com/static/payvault/V7/expiration-year.html" style="border: none; height: 34px; width: 100%"></iframe>
					</div>
					<span>年</span>
					<h5>セキュリティコード</h5>
					<div id="my-cvv-mount">
						<iframe src="https://payvault.global.rakuten.com/static/payvault/V7/cvv.html" style="border: none; height: 34px; width: 100%"></iframe>
					</div>
				<div id="my-form">
					<input type="hidden" name="timestamp" value="">
					<input type="hidden" name="signature" value="">
					<input type="hidden" name="keyVersion" value="">
					<input type="hidden" name="cardToken" value="">
					<input type="hidden" name="iin" value="">
					<input type="hidden" name="last4digits" value="">
					<input type="hidden" name="brandCode" value="">
					<input type="hidden" name="expirationMonth" value="">
					<input type="hidden" name="expirationYear" value="">
				</div>
				</div>

			</td>
		</tr>
		</tbody>
		
		<div>
			<asp:Literal ID="lMessage" Text="該当のボタンをクリックしてください" runat="server" /><br />
			<asp:Button ID="btnSendAuthOk" runat="server" Text="  Card OK  " OnClick="btnSendAuthOk_Click" />
			<asp:Button ID="btnSendAuthNg" runat="server" Text="  Card NG  " OnClick="btnSendAuthNg_Click" />
		</div>
		<script type="text/javascript" src="https://payvault-stg.global.rakuten.com/static/payvault/V7/payvault.js"></script>
		<script type="text/javascript">
			function returnResponseRakuten(authResult,resultCase) {
					try {
						if (window.opener != null) {
							var res = {
								dataType: '2',
								recordType: '11',
								category: '104',
								procNo: '00001',
								comResult: '000',
								mstatus: 'success',
								vResultCode: 'xxxx',
								merrMsg: '',
								gwErrCd: 'nnn',
								gwErrMsg: '',
								requestDate: '<%= DateTime.Now.ToString("yyyyMMddHHmmss") %>',
								responseDate: '<%= DateTime.Now.ToString("yyyyMMddHHmmss") %>',
								orderDate: '<%= DateTime.Now.ToString("YYYY-MM-DD hh:mm:ss.SSS") %>',
								line1: 'xxxxxx',
								mcSecCd: '1',
								token: '240415220013MMuzX8NYZrepCOdy1111',
								top6: '240415',
								last4: '1111',
								mcAcntNo1: '01',
								expire: '2034',
								brand: 'Visa',
								issurName: 'saison',
								kanaCardName: 'walmart',
								processPass: 'NwLjI74jRUW5XOFtCzfTBOlq1JQ9eOohj5gtpArzQJgzXqHHchE8KhnBpo1wWqsX',
								processId: '123456',
								customerId: '123456',
								orderId: 'TEST-<%= DateTime.Now.ToString("yyyyMMddHHmmss") %>',
								acqName: 'rakutencard',
								resAuthCode: 'xxxxxxxxxxxxx',
								tranDate: '<%= DateTime.Now.ToString("YYYY-MM-DD hh:mm:ss.SSS") %>',
								line2: 'NwLjI74jRUW5XOFtCzfTBOlq1JQ9eOohj5gtpArzQJgzXqHHchE8KhnBpo1wWqsX',
								payTimes: '2',
								nameKanji: 'true',
								nameKana: 'false',
								forward: 'true'
							};

							switch (authResult) {
								case "success":
									res.mstatus = 'success';
									res.merrMsg = '';
									res.category = '104';
									break;
								case "failure":
									res.mstatus = 'failure';
									res.merrMsg = '認証に失敗しました';
									res.category = '104';
									res.vResultCode = 'invalid_request_parameter'
									break;
								case "other":
									res.mstatus = 'success';
									res.category = '104';
									break;
							}

							var result = JSON.stringify(res);
							window.opener.getResponseFromMock(result);
						}
					} catch (error) {
						alert(error);
					}
				}
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
