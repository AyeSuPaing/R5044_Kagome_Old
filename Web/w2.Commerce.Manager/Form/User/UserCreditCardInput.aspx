
<%--
=========================================================================================================
  Module      : ユーザクレジットカード入力画面(UserCreditCardInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true"
	CodeFile="~/Form/User/UserCreditCardInput.aspx.cs" Inherits="Form_User_UserCreditCardInput"
	Title="ユーザークレジットカード入力" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<script runat="server">
	protected new void Page_Load(object sender, EventArgs e)
	{
		base.Page_Load(sender, e);
		if (this.ProcessMode == ProcessModeType.OrderCreditCardAuth) this.Title = "注文クレジットカード決済";
	}
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
<tr>
	<td>
		<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
	</td>
</tr>
<tr>
	<td>
		<%if (this.ProcessMode == ProcessModeType.UserCreditCartdRegister) {%>
		<%if (Request[Constants.REQUEST_KEY_CREDITCARD_NO] == null) {%>
		<h2 class="cmn-hed-h2"><%if (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus) {%>仮<%} %>クレジットカード情報登録</h2>
		<% } else { %>
		<h2 class="cmn-hed-h2">クレジットカード情報変更</h2>
		<% } %>
		<% } %>
		<%if (this.ProcessMode == ProcessModeType.OrderCreditCardAuth) {%>
		<h2 class="cmn-hed-h2">注文クレジットカード決済</h2>
		<% } %>
	</td>
</tr>
<tr>
	<td>
		<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
		<tr>
		<td>
			<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr>
			<td align="center">
				<table cellspacing="0" cellpadding="0" border="0">
				<tr>
					<td>
						<img height="15" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
					</td>
				</tr>
				<tr>
				<td>
					<div id="dvUserFltContents">
					<div id="dvUserCreditCardInput">
					<div class="dvUserCreditCardInfo">
						<%-- UPDATE PANELここから --%>
						<asp:UpdatePanel ID="upUpdatePanel" runat="server">
						<ContentTemplate>
							<div id="divErrorMessage" runat="server" visible="false">
								<table class="edit_table" width="758" border="0" cellspacing="1" cellpadding="3">
									<tr class="edit_item_bg">
										<td><asp:Label ID="lbErrorMessage" runat="server" ForeColor="Red" ></asp:Label></td>
									</tr>
								</table>
								<br />
							</div>
							<%if (this.ProcessMode == ProcessModeType.OrderCreditCardAuth) {%>
							<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
							<tr>
								<td align="left" class="edit_title_bg">注文情報</td>
									<td class="edit_item_bg">
									<a href="<%: OrderPage.CreateOrderDetailUrl(this.OrderId, true, false, null) %>" target="_blank">
										<asp:Literal id="lOrderId" runat="server"></asp:Literal>
									</a><br/>
									合計金額：<span style="font-weight: bold">
										<asp:Literal id="lOrderPriceTotal" runat="server"></asp:Literal></span>
								</td>
							</tr>
							<tr>
								<td align="left" class="edit_title_bg">注文者情報</td>
								<td class="edit_item_bg">
									<asp:Literal id="lOrderOwnerOwnerName" runat="server"></asp:Literal>（<asp:Literal id="lOrderOwnerOwnerNameKana" runat="server"></asp:Literal>）<br/>
									<asp:Literal id="lOrderOwnerOwnerTel1" runat="server"></asp:Literal>
								</td>
							</tr>
							</table>
							<br/>
							<%} %>
							<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
								<%if (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus == false) {%>
								<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
								<tbody id="divCreditCardNoToken" runat="server">
								<%if (OrderCommon.CreditCompanySelectable) {%>
								<tr>
									<td align="left" class="edit_title_bg">
										カード会社<span class="notice">*</span>
									</td>
									<td class="edit_item_bg">
										<asp:DropDownList ID="ddlCreditCardCompany" runat="server"></asp:DropDownList>
									</td>
								</tr>
								<%} %>
												<asp:PlaceHolder ID="phCreditCardNotRakuten" runat="server">
													<tr class="creditCardItem NEW">
														<td class="edit_title_bg" align="left">
															<%if (this.CreditTokenizedPanUse) {%>永久トークン<%}else{%>カード番号<%} %><span class="notice">*</span>
														</td>
														<td id="tdCreditNumber" class="edit_item_bg" align="left" runat="server">
															<asp:TextBox id="tbCreditCardNo1" pattern="[0-9]*" Width="160" MaxLength="16" autocomplete="off" runat="server"></asp:TextBox>
															<%--▼▼ カード情報取得用 ▼▼--%>
															<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%= CreateGetCardInfoJsScriptForCreditToken() %>" />
															<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
															<%--▲▲ カード情報取得用 ▲▲--%>
														</td>
														<td id="tdGetCardInfo" class="edit_item_bg" runat="server"><asp:Button id="btnGetCreditCardInfo" Text="  決済端末と接続  " onClick="btnGetCardInfo_Click" runat="server"/>※決済端末と接続ボタンを押下したあと、決済端末でカード番号を入力してください。</td>
														<div id="payTgModal" class="payTgModal">
															<div class="payTgModalOuter">
																<div class="payTgModalMargin">
																</div>
																<div class="payTgModalContents">
																	<h1 style="font-size: 16px;">PayTG決済結果待機中・・・</h1><br/>
																	<h1 style="font-size: 16px;">テンキー端末の操作を完了してください。</h1>
																</div>
															</div>
														</div>
													</tr>
													<tr id="trCreditExpire" runat="server" class="creditCardItem NEW">
														<td class="edit_title_bg" align="left">有効期限<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlCreditExpireMonth" runat="server"></asp:DropDownList>/<asp:DropDownList id="ddlCreditExpireYear" runat="server"></asp:DropDownList> (月/年)</td>
													</tr>
													<tr class="creditCardItem NEW">
														<td class="edit_title_bg" align="left">カード名義人 <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbCreditAuthorName" runat="server" Width="180" autocomplete="off"></asp:TextBox></td>
													</tr>
													<tr id="trSecurityCode" runat="server" class="creditCardItem NEW">
														<td class="edit_title_bg" align="left">セキュリティコード</td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbCreditSecurityCode" runat="server" Width="60" MaxLength="4" autocomplete="off"></asp:TextBox></td>
													</tr>
													</asp:PlaceHolder>
								</tbody>
								<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>
								<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
								<div id="divCreditCardForTokenAcquired" Visible="False" runat="server">
								<%if (OrderCommon.CreditCompanySelectable) {%>
								<tr class="creditCardItem NEW">
									<td class="edit_title_bg" align="left">カード会社</td>
									<td class="edit_item_bg" align="left"><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" runat="server"></asp:Literal></td>
								</tr>
								<%} %>
								<tr class="creditCardItem NEW">
									<td class="edit_title_bg" align="left" style="width: 190px;">カード番号</td>
									<td class="edit_item_bg" align="left">
										************<asp:Literal ID="lLastFourDigitForTokenAcquired" Text="" runat="server"></asp:Literal>
										<asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
									</td>
								</tr>
								<tr class="creditCardItem NEW">
									<td class="edit_title_bg" align="left">有効期限</td>
									<td class="edit_item_bg" align="left">
										<asp:Literal ID="lExpirationMonthForTokenAcquired" Text="" runat="server"></asp:Literal> / <asp:Literal ID="lExpirationYearForTokenAcquired" Text="" runat="server"></asp:Literal> (月/年)
									</td>
								</tr>
								<tr class="creditCardItem NEW">
									<td class="edit_title_bg" align="left">カード名義人</td>
									<td class="edit_item_bg" align="left">
										<asp:Literal ID="lCreditAuthorNameForTokenAcquired" Text="" runat="server"></asp:Literal>
									</td>
								</tr>
								</div>
								<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>
								<tr id="trInstallments" runat="server">
									<td class="edit_title_bg" align="left">支払回数<span class="notice">*</span></td>
									<td class="edit_item_bg" align="left"><asp:DropDownList id="dllCreditInstallments" runat="server"></asp:DropDownList>&nbsp;&nbsp;※AMEX/DINERSは一括のみとなります。</td>
								</tr>
								<tr id="trRegistCreditCard" runat="server">
									<td align="left" class="edit_title_bg"  style="width:200px;">登録する</td>
									<td class="edit_item_bg">
										<asp:CheckBox ID="cbRegistCreditCard" runat="server" Text="  登録する" AutoPostBack="True" OnCheckedChanged="cbRegistCreditCard_CheckedChanged"></asp:CheckBox>
									</td>
								</tr>
								<%} %>
								<tr id="trCreditCardName" runat="server">
									<td align="left" class="edit_title_bg">
										クレジットカード登録名 <span class="notice">*</span>
									</td>
									<td class="edit_item_bg">
										<asp:TextBox ID="tbUserCreditCardName" runat="server" MaxLength="30"></asp:TextBox>
									</td>
								</tr>
							</table>
							<%--▼▼ クレジット Token保持用 ▼▼--%>
							<asp:HiddenField ID="hfCreditToken" Value="" runat="server" />
							<%--▲▲ クレジット Token保持用 ▲▲--%>
							<%--▼▼ PayTg 端末状態保持用 ▼▼--%>
							<asp:HiddenField ID="hfCanUseDevice" runat="server" />
							<asp:HiddenField ID="hfStateMessage" runat="server" />
							<%--▲▲ PayTg 端末状態保持用 ▲▲--%>
						</ContentTemplate>
						<Triggers>
							<asp:AsyncPostBackTrigger ControlID="btnConfirm" EventName="Click" />
						</Triggers>
						</asp:UpdatePanel>
						<%-- UPDATE PANELここまで --%>
					</div>
					<br />
					<div style="text-align: right;">
						<%if (SessionManager.UsePaymentTabletZeus) {%>
						<input onclick="Javascript: history.back();" type="button" value="  戻る  " />
						<% } %>
						<%if (this.ProcessMode == ProcessModeType.OrderCreditCardAuth) { btnConfirm.Text = @"  登録する  "; } %>
						<div class="fixed-bottom-area">
						<asp:HiddenField ID="hfPayTgSendId" runat="server" />
						<asp:HiddenField ID="hfPayTgPostData" runat="server" />
						<asp:HiddenField ID="hfPayTgResponse" runat="server" />
						<asp:Button ID="btnProcessPayTgResponse" runat="server" Style="display: none;" OnClick="btnProcessPayTgResponse_Click" />
						<asp:Button ID="btnConfirm" Text="  確認する  " UseSubmitBehavior="false" OnClientClick="doPostbackEvenIfCardAuthFailed=false;" ValidationGroup="UserCreditCardRegist" runat="server" OnClick="btnConfirm_Click"></asp:Button>
						</div>
					</div>
					</div>
					</div>
				</td>
				</tr>
				<tr>
				<td>
					<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
				</td>
				</tr>
				</table>
			</td>
			</tr>
			</table>
		</td>
		</tr>
		</table>
	</td>
</tr>
<tr>
	<td>
		<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
	</td>
</tr>
</table>
<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";

	// Show confirm button area
	function showConfirmButtonArea() {
		$('.fixed-bottom-area').show();
	}

	// Hide confirm button area
	function hideConfirmButtonArea() {
		$('.fixed-bottom-area').hide();
	}

	// PayTg：PayTg端末状態確認
	function execGetPayTgDeviceStatus(apiUrl) {
		<% if(Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED) { %>
		mockWindow = window.open(apiUrl, 'CheckDeviceStatusPayTgMock', 'width=500,height=300,top=120,left=420,status=NO,scrollbars=no');
		<% } else { %>
		var requestCheckDeviceStatus = $.ajax({
			url: apiUrl,
			type: "GET",
			dataType: "json",
			cache: false
		});

		requestCheckDeviceStatus.done(function (data) {
			document.getElementById('<%= hfCanUseDevice.ClientID%>').value = data["canUseDevice"];
			document.getElementById('<%= hfStateMessage.ClientID%>').value = data["stateMessage"];
		})
		<% } %>
	}

	// PayTg：端末状態確認モックのレスポンス取得
	function getResponseFromCheckDeviceStatusMock(result) {
		// モック画面閉じる
		mockWindow.close();
		setTimeout(function () {
			var jsonRes = JSON.parse(result);
			document.getElementById('<%= hfCanUseDevice.ClientID%>').value = jsonRes["canUseDevice"];
			document.getElementById('<%= hfStateMessage.ClientID%>').value = jsonRes["stateMessage"];
			if (jsonRes["canUseDevice"] === "false" || jsonRes["stateMessage"] === "未接続") {
				alert('決済端末に接続できません。再度お試しください。');
			}
		}, 100);
	}

	// PayTg：カード登録実行
	function execCardRegistration(url, hfPayTgPostData) {
		lockScreen();
		hideConfirmButtonArea();
		<% if (Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED) { %>
		mockWindow = window.open(url, 'RegisterCardMock', 'width=750,height=550,top=120,left=420');
		mockWindow.onbeforeunload = function () {
		};
		<% } else { %>
			// PayTG専用端末の状態チェック
			var requestCheckDevice = $.ajax({
				url: "<%= Constants.PAYMENT_SETTING_PAYTG_DEVICE_STATUS_CHECK_URL %>",
				type: "GET",
				dataType: "json",
				cache: false
		});
		console.log(requestCheckDevice)
		requestCheckDevice.done(function(data) {
			if (data["canUseDevice"] === true) {
				registerCreditCard(url, hfPayTgPostData);
			} else {
				unlockScreen(false, false);
				showConfirmButtonArea();
			}
		});

		requestCheckDevice.fail(function(error) {
			unlockScreen(false, false);
			console.log(error);
			showConfirmButtonArea();
		});
		<% } %>
		return false;
	}

	// PayTg：クレジットカード登録
	function registerCreditCard(url, hfPayTgPostData) {
		var postData = JSON.parse(hfPayTgPostData || "null");
		// null値を含まないようにデータを整形する
		var cleanedData = {};
		for (var key in postData) {
			if (postData[key] !== null) {
				cleanedData[key] = postData[key];
			}
		}
		var requestRegisterCard = $.ajax({
			url: url,
			type: "POST",
			contentType: 'application/json',
			data: JSON.stringify(postData),
			cache: false
		});
		requestRegisterCard.done(function (result) {
			// PayTG連携のレスポンスはHiddenFieldに保持する
			$('#<%= hfPayTgResponse.ClientID %>').val(JSON.stringify(result));
			// サーバー側でPayTG連携のレスポンス処理を行う
			$('#<%= btnProcessPayTgResponse.ClientID %>').click();
			// ロック画面を解除
			unlockScreen((result["mstatus"] === "success"), true);
		});
		requestRegisterCard.fail(function (error) {
			console.log(error);
			unlockScreen(false, false);
		});

		return false;
	}

	// PayTg：カード登録モックのレスポンス取得
	function getResponseFromMock(result) {
		// モック画面閉じる
		mockWindow.close();
		setTimeout(function () {
			// ロック画面を解除
			var jsonRes = JSON.parse(result);
			unlockScreen((jsonRes["mstatus"] === "success"), true);
			showConfirmButtonArea();

			// レスポンスはHiddenFieldに保持する
			$('#<%= hfPayTgResponse.ClientID %>').val(result);
			// サーバー側でPayTG連携のレスポンス処理を行う
			$('#<%= btnProcessPayTgResponse.ClientID %>').click();
		}, 100);
	}
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>
<asp:HiddenField ID="hfCreditBincode" runat="server" />
</asp:Content>
