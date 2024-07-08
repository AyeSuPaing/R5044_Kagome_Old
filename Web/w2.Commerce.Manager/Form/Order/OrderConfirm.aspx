<%--
=========================================================================================================
  Module      : 注文情報確認ページ(OrderConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="uc" TagName="BodyOrderConfirm" Src="~/Form/Common/BodyOrderConfirm.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderConfirm.aspx.cs" Inherits="Form_Order_OrderConfirm" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.App.Common.Input.Order" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.Domain.Order" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
// 注文メール送信画面表示
function open_order_send_mail()
{
	if (document.getElementById('<%= ddlOrderMailId.ClientID %>').value != '')
	{
		var url = '<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_SEND %>'
			+ '?' + '<%= Constants.REQUEST_KEY_ORDER_ID %>' +  '=' + '<%= HttpUtility.UrlEncode(this.OrderInput.OrderId) %>'
			+ '&' + '<%= Constants.REQUEST_KEY_MAIL_TEMPLATE_ID %>' + '=' + encodeURIComponent(document.getElementById('<%= ddlOrderMailId.ClientID %>').value);
		
		open_window(url,'order_send_mail','width=850,height=825,top=120,left=320,status=NO,scrollbars=yes');
	}
	else
	{
		alert('メールテンプレートを選択してください');
	}
}

	function disable_button(button)
	{
		button.hide();
		button.next('.settlementInProcessing').show();
		return true;
	}

// 再注文へ進む確認ボックス表示
function confirm_reorder()
{
	var cooperationIdArray = '<%= (this.UserCreditCard != null ? this.UserCreditCard.CooperationId : "") %>'.split(',');
	if (('<%= this.OrderInput.OrderPaymentKbn %>' == '<%= Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT %>') && (cooperationIdArray[0] == "") && (cooperationIdArray.length == 2))
	{
		return confirm('再注文のため、注文情報登録画面へ遷移致します。\r\nよろしいでしょうか。\r\n※再注文時、現在使用されているクレジットカードを使うことはできません。\r\n別のクレジットカードを選択するか、別の決済を選択する必要があります。');
	}
	else
	{
		return confirm('再注文のため、注文情報登録画面へ遷移致します。\r\nよろしいでしょうか。');
	}
}

// 設定された注文ステータスが選択され更新時の場合のアラート表示
function alert_for_update_orderstatus() {
	var checkedOrderStatus = $("#<%= rblOrderStatus.ClientID %> input:checked");
	var orderStatus = checkedOrderStatus.val();
	if ('<%= Constants.ALERT_FOR_ORDER_STATUS_IN_ORDER_CONFIRM %>'.split(',').indexOf(orderStatus) >= 0) {
		var orderStatusName = checkedOrderStatus.next().text();
		var alertText = "注文ステータスを「" + orderStatusName + "」に更新してもよろしいですか？";
		return confirm(alertText);
	}
	return true;
}

// ページを更新するポップアップを表示
window.onload = function()
{
	var url = new URL(window.location.href);
	var params = url.searchParams;
	if (params.get("<%: Constants.REQUEST_KEY_ORDER_STATUS_UPDATED%>") === "true") {
		alert('他のオペレータもしくはユーザーによって注文ステータスがキャンセルに変更されているのでページを更新しました。');
	}
}

//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<% if (this.IsPopUp == false) { %>
	<tr><td><h1 class="page-title">受注情報</h1></td></tr>
	<tr><td><img height="10" width="100" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
	<% } %>
	<% if (this.IsPopUp) { %>
	<tr><td><h2 class="page-title">受注情報詳細</h2></td></tr>
	<% } else { %>
	<tr><td><h2 class="cmn-hed-h2">受注情報詳細</h2></td></tr>
	<% } %>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<%-- ZEUS仮クレジットカード向け表示 --%>
										<tbody id="tbdyCreditCardInputButtonZeus" Visible="False" runat="server">
										<tr>
										<td>
											<center>
											<asp:Button id="btnRegisterCreditCard" Text="クレジットカードで決済する" runat="server" OnClick="btnRegisterCreditCard_Click"/>
											</center>
											<br/>
										</td>
										</tr>
										</tbody>
										<tbody id="tbdyCreditCardInputUnvisibleMessageZeus" Visible="False" runat="server">
										<tr>
										<td>
											<center class="notice">
											クレジットカード番号は入力できません。<br />
											クレジットカード番号の入力は決済用タブレットにて行ってください。
											</center>
											<br/>
										</td>
										</tr>
										</tbody>
										<%-- 仮クレジットカード登録待ち向け表示 --%>
										<tbody id="tbdyCreditCardRegisterForUnregisterd" Visible="False" runat="server">
										<tr>
										<td>
											<center>
											<table class="detail_table" border="0" cellspacing="1" cellpadding="3" style="min-width: 300px;">
											<tr>
											<td align="left" class="detail_item_bg" style="font-size:11pt">
												<div style="text-align:center">
												<span style="color: red">
												<div id="divCreditCardAuthErrorMessage" Visible="False" runat="server">
													クレジットカード与信が失敗しました。<br/>
													新たにクレジットカード登録から行ってください。<br/>
													<br/>
												</div>
												クレジットカード登録は保留状態です。<br/>
												下記の手順で登録・与信を行ってください。<br/>
												（カード登録ごとに
												<%if (OrderCommon.IsPaymentCardTypeGmo) {%>GMO会員<%} %>
												<%if (OrderCommon.IsPaymentCardTypeYamatoKwc) {%>ヤマト会員<%} %>
												を登録する必要があります。）<br/>
												<br/>
												※注文変更・返品交換の場合、以前取得していた与信は既に取り消し済みです。<br/>
												</span>
												</div>
												<br/>
												<%if (OrderCommon.IsPaymentCardTypeGmo) {%>
												<div style="text-align:left;">
												①決済用タブレットで「会員・カード登録」を開き、<br/>
												　クレジットカード番号と下記情報IDにてカードの登録を行ってください。<br />
												<br />
												<span style="font-size: large; line-height: 18px;text-align:center;">
												　GMO会員ID：<asp:Literal id="lGmoMemberId" runat="server"></asp:Literal><br/>
												</span>
												<br/>
												　※GMO会員IDはハイフンなしで入力してください。<br/>
												<%} %>
												<%if (OrderCommon.IsPaymentCardTypeYamatoKwc) {%>
												<div style="text-align:left;">
												①決済用タブレットにて下記情報で与信を実行してください。<br/>
												<br />
												<span style="font-size: large; line-height: 18px;text-align:center;">
												　受付番号　　：<asp:Literal id="lYamatoKwcOrderNo" runat="server"></asp:Literal><br/>
												　ヤマト会員ID：<asp:Literal id="lYamatoKwcMemberId" runat="server"></asp:Literal><br/>
												　認証キー　　：<asp:Literal id="lYamatoKwcAuthenticationKey" runat="server"></asp:Literal><br/>
												　金額　　　　：1 円<br/>
												</span>
												<br/>
												　※受付番号・ヤマト会員IDはハイフンなしで入力してください。<br/>
												　※１円与信はヤマト会員登録のために利用します。<br/>
												　　ヤマト会員登録後、この1円与信は取り消しされます。<br/>
												<%} %>
												<%if (OrderCommon.IsPaymentCardTypeEScott) {%>
												<div style="text-align:left;">
												①決済用タブレットにて、クレジットカード情報と下記情報で会員登録を実行してください。<br/>
												<br />
												<span style="font-size: large; line-height: 18px;text-align:center;">
												　会員ID　　　　：<asp:Literal id="lEScottKaiinId" runat="server"></asp:Literal><br/>
												</span>
												<br/>
												※会員IDはハイフンなしで入力してください。
												<%} %>
												<br/>
												②登録が完了したらリロードを行い、<br/>
												　画面が切り替わったら与信を実行するボタンをクリックしてください。<br/>
												</div>
												<br/>
												<div style="text-align:center">
												<asp:Button id="btnReloadForRegisterdCreditCard" Text="    リロード    " OnClick="btnReloadForRegisterdCreditCard_Click" runat="server"/><br />
												</div>
												</div>
											</td>
											</tr>
											</table>
											</center>
										</td>
										</tr>
										</tbody>
										<%-- 仮クレジットカード与信待ち向け表示 --%>
										<tbody id="tbdyCreditCardRegisternForUnauthed" Visible="False" runat="server">
										<tr>
										<td style="text-align:center;">
											<span class="notice">
											<%if (OrderCommon.IsPaymentCardTypeYamatoKwc) {%>
											１円与信は正常に取消されました。<br/>
											<%} %>
											注文のクレジットカード与信は保留状態のため、<br/>
											下記のボタンをクリックし、与信を実行してください。<br/>
											</span>
											<br/>
											<table class="edit_table" cellspacing="1" cellpadding="6" width="758" border="0">
											<tr>
												<td class="edit_title_bg" align="left">クレジットカード情報</td>
												<td class="edit_item_bg" align="left"><asp:Literal ID="lCreditCardInfo" runat="server"></asp:Literal> </td>
											</tr>
											<tr>
												<td class="edit_title_bg" align="left">合計金額</td>
												<td class="edit_item_bg" align="left">
													<span style="font-size: large;line-height: 18px">
														<%: this.OrderInput.LastBilledAmount.ToPriceString(true) %></span>
												</td>
											</tr>
											<%-- ▽分割支払い有効の場合は表示▽ --%>
											<tr id="trInstallments" runat="server">
												<td class="edit_title_bg" align="left">支払回数<span class="notice">*</span></td>
												<td class="edit_item_bg" align="left"><asp:DropDownList id="dllCreditInstallments" runat="server"></asp:DropDownList>&nbsp;&nbsp;※AMEX/DINERSは一括のみとなります。</td>
											</tr>
											<%-- △分割支払い有効の場合は表示△ --%>
											<tr class="creditCardItem" style=" height:27px;" id="trRegistCreditCard" runat="server">
												<td align="left" class="edit_title_bg" style="width: 200px;">登録する</td>
												<td class="edit_item_bg"><asp:CheckBox ID="cbRegistCreditCard" runat="server" Text="  登録する" AutoPostBack="True" OnCheckedChanged="cbRegistCreditCard_CheckedChanged"></asp:CheckBox></td>
											</tr>
											<tr class="creditCardItem" style=" height:27px;" id="trCreditCardName" runat="server" Visible="False">
												<td align="left" class="edit_title_bg">クレジットカード登録名 <span class="notice">*</span></td>
												<td class="edit_item_bg"><asp:TextBox ID="tbUserCreditCardName" runat="server" MaxLength="30"></asp:TextBox>&nbsp;&nbsp;※クレジットカードを保存する場合は、以下をご入力ください。</td>
											</tr>
											</table>
											<div class="notice" style="margin: 5px;">
												<asp:Literal ID="lCreditCardRegisternForUnauthed" runat="server" />
											</div>
											<asp:Button id="btnRegisterUnregisterdCreditCardForAuthError" Text="仮クレジットカードを再登録する" Visible="False" runat="server" OnClick="btnRegisterUnregisterdCreditCardForAuthError_Click"/><br/>
											<asp:Button id="btnAuthRegisterdCreditCard" Text="与信を実行する" OnClientClick="this.disabled=true;" runat="server" OnClick="btnAuthRegisterdCreditCard_Click" UseSubmitBehavior="False"/><br/>
										</td>
										</tr>
										</tbody>
										<tr id="trFirstPointAlert" runat="server" visible="false">
											<td>
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3" style="margin-top:<%= this.IsPopUp ? "45px" : "0px" %>">
													<tr>
														<td align="left" class="info_item_bg" style="color:red; font-weight:bold">
														このユーザには今回の注文の他にも初回購入ポイントが割り当てられています。<br />
														ポイントオプションより調整を行う必要があります。
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr id="trSendMailError" runat="server" visible="false">
											<td>
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3" style="margin-top:<%= this.IsPopUp ? "45px" : "0px" %>">
										<tr>
														<td align="left" class="info_item_bg" style="color:red; font-weight:bold">
															<asp:Literal ID="lSendMeilErrorMessage" EnableViewState="false" runat="server" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<div id="divReturnOrderReauthErrormessages" runat="server" class="action_part_top" visible="false">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">
																<span class="notice">
																	<strong><asp:Literal ID="lReturnOrderReauthErrormessages" runat="server" /></strong>
																</span>
															</td>
														</tr>
													</table>
												</div>
												<% if (string.IsNullOrEmpty(lActionCompleteMessage.Text) == false) { %>
													<div class="action_part_top">
														<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
															<tr class="info_item_bg">
																<td align="left">
																	<asp:Literal ID="lActionCompleteMessage" EnableViewState="false" runat="server" />
																</td>
															</tr>
														</table>
													</div>
												<% } %>
												<div class="action_part_top">
													<span class="notice">
														<strong><asp:Literal ID="lMessages" Visible="false" runat="server" /></strong>
													</span>
													<span id="spanUpdateHistoryConfirmTop" runat="server">( <a href="javascript:open_window('<%= UpdateHistoryPage.CreateUpdateHistoryConfirmUrl(Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_ORDER, this.OrderInput.UserId, this.RequestOrderId) %>','updatehistory','width=1100,height=850,top=5,left=600,status=NO,scrollbars=yes,resizable=yes')">履歴参照</a> )</span>
													<asp:Button ID="btnBackListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnBackList_Click" />
													<asp:Button ID="btnReOrderTop" runat="server" Text="  再注文する  " OnClientClick="return confirm_reorder()" OnClick="btnReOrder_Click" />
													<asp:Button ID="btnReturnExchangeTop" runat="server" Text="  返品交換する  " OnClick="btnReturnExchange_Click" OnClientClick="return check_change();" />
													<asp:Button ID="btnEditTop" runat="server" Text="  編集する  " OnClick="btnEdit_Click" OnClientClick="return check_change();" />
												</div>
												<%--▽ 関連注文情報 ▽--%>
												<table cellspacing="0" cellpadding="0" width="758" border="0">
													<tr valign="top">
														<td align="left">
															<table class="detail_table" cellspacing="1" cellpadding="3" width="430" border="0">
																<tr>
																	<td class="detail_title_bg" align="center" colspan="<%= Constants.SUBSCRIPTION_BOX_OPTION_ENABLED ? 4 : 3 %>">関連注文情報</td>
																</tr>
																<%--▽ 定期購入情報 ▽--%>
																<%--▽ 定期購入OP有効 AND 元注文 AND 定期購入IDあり ▽--%>
																<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED && (string.IsNullOrEmpty(this.OrderInput.FixedPurchaseId) == false)) { %>
																<tr>
																	<td class="detail_title_bg" align="left" width="130">定期購入ID</td>
																	<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
																	<td class="detail_title_bg" align="center" width="150">頒布会コースID</td>
																	<%} %>
																	<td class="detail_title_bg" align="center" width="150">定期購入回数(注文時点)</td>
																	<td class="detail_title_bg" align="center" width="150">定期購入回数(出荷時点)</td>
																</tr>
																<tr>
																	<td class="detail_item_bg" align="left"><a href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(FixedPurchasePage.CreateFixedPurchaseDetailUrl(this.OrderInput.FixedPurchaseId, true)) %>','fixedpurchase','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%: this.OrderInput.FixedPurchaseId %></a></td>
																	<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
																	<td class="detail_item_bg" align="left">
																		<% if (this.IsItemsOneSubscriptionBoxCourse) { %>
																		<a href="javascript:open_window('<%:FixedPurchasePage.CreateSubscriptionBoxRegisterUrl(this.OrderInput.ItemSubscriptionBoxCourseIds[0]) %>','fixedpurchase','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																			<%: this.OrderInput.ItemSubscriptionBoxCourseIds[0] %>
																		</a>
																		<% } else { %>
																		<div>
																			<%= this.EncodedSubscriptionBoxCourseIdsForDisplay %>
																		</div>
																		<% } %>
																	</td>
																	<% } %>
																	<td class="detail_item_bg" align="center"><%: (string.IsNullOrEmpty(this.OrderInput.FixedPurchaseOrderCount) == false) ? this.OrderInput.FixedPurchaseOrderCount + " 回目" : "-" %></td>
																	<td class="detail_item_bg" align="center"><%: (string.IsNullOrEmpty(this.OrderInput.FixedPurchaseShippedCount) == false) ? this.OrderInput.FixedPurchaseShippedCount + " 回目" : "-" %></td>
																</tr>
																<% } %>
																<%--△ 定期購入情報 △--%>
																<tr>
																	<td class="detail_title_bg" align="left" width="130">注文ID</td>
																	<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
																	<td class="detail_title_bg" align="center" width="130">頒布会購入回数</td>
																	<%} %>
																	<td class="detail_title_bg" align="center" width="150">登録日時</td>
																	<td class="detail_title_bg" align="center" width="150">返品交換区分</td>
																</tr>
																<asp:Repeater ID="rRelatedOrder" runat="server" ItemType="OrderInput">
																	<ItemTemplate>
																		<tr class='<%# this.OrderInput.OrderId == Item.OrderId ? "list_item_bg_selected" : "detail_item_bg" %>'>
																			<td align="left"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateOrderDetailUrl(Item.OrderId)) %>"><%#: Item.OrderId %></a><%#: (Item.IsLastAuthFlgON) ? " （最終与信）" : string.Empty %></td>
																			<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
																			<td align="center"><%: this.OrderInput.OrderSubscriptionBoxOrderCount == "0" ? "-" : this.OrderInput.OrderSubscriptionBoxOrderCount + "回目" %></td>
																			<%}%>
																			<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																			<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN, Item.ReturnExchangeKbn) %></td>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
															</table>
														</td>
														<% if (Constants.ORDER_COMBINE_OPTION_ENABLED) { %>
														<asp:Repeater ID="rCombinedOrders" runat="server" ItemType="w2.Domain.Order.OrderModel">
														<HeaderTemplate>
															<td align="right">
																<table class="detail_table" cellspacing="1" cellpadding="3" width="315" border="0">
																<tr>
																	<td class="detail_title_bg" align="center" colspan="2">同梱済注文情報</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left" width="130">注文ID</td>
																	<td class="detail_title_bg" align="center" width="150">登録日時</td>
																</tr>
														</HeaderTemplate>
														<ItemTemplate>
																<tr class='detail_item_bg'>
																	<td align="left"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateOrderDetailUrl(Item.OrderId)) %>"><%#: Item.OrderId %></a></td>
																	<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																</tr>
														</ItemTemplate>
														<FooterTemplate>
																</table>
															</td>
														</FooterTemplate>
														</asp:Repeater>
														<% } %>
													</tr>
												</table>
												<%--△ 関連注文情報 △--%>
												<%--▽ 注文ステータス更新 ▽--%>
												<div id="divOrderStatus" runat="server">
												<br />
												<asp:UpdatePanel id="upOrderStatus" runat="server">
												<ContentTemplate>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">注文ステータス更新</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="120">注文ステータス</td>
														<td class="detail_item_bg" colspan="2">
															<asp:RadioButtonList id="rblOrderStatus" OnSelectedIndexChanged="DisplayControl" AutoPostBack="true" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
															<asp:HiddenField ID="hfOrderStatus" runat="server" />

															<span runat="server" id="sAtodenePaymentDeadlineAlert" Visible="False" style="color: red">
																<br/>
																出荷手配済みに変更した場合は、Atodene後払い請求書の支払い期限が確定するのでご注意ください。
															</span>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="120">更新日指定</td>
														<td class="detail_item_bg" align="left">&nbsp;
															<uc:DateTimePickerPeriodInput id="ucUpdateOrderStatusDate" CanShowEndDatePicker="False" UpdatePanelControlId="<%#: upOrderStatus.ID %>" runat="server" IsHideTime="true" />
														</td>
														<td class="detail_item_bg" align="right" width="120" style="white-space: nowrap;">
															<asp:CheckBox ID="cbReauthCancel" Text=" 決済システム連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCreditZeusCancel" Text=" ZEUSキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCreditSBPSCancel" Text=" SBPSクレジットキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCreditYamatoKwcCancel" Text=" ヤマトKWCクレジットキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCreditGMOCancel" Text=" GMOクレジットキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCreditZcomCancel" Text=" Zcomクレジットキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCreditEScottCancel" Text=" e-SCOTTキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCreditPaygentCancel" Text=" ペイジェントキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCareerDocoomKetaiSBPSCancel" Text=" ドコモケータイ払いキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCareerAuKantanSBPSCancel" Text=" auかんたん決済キャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCareerSoftbankKetaiSBPSCancel" Text=" ソフトバンク・ワイモバイルまとめて支払いキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbRecruitSBPSCancel" Text=" リクルートかんたん支払いキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbPaypalSBPSCancel" Text=" PayPal(SBPS)キャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbRakutenIdSBPSCancel" Text=" 楽天ペイ(SBPS)キャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbYamatoKaCancel" Text=" ヤマト後払いキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCvsDefGmoCancel" Text=" GMO後払いキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCvsDefAtodeneCancel" Text=" Atodene後払いキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCvsDefDskCancel" Text=" DSK後払いキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCvsDefAtobaraicomCancel" Text="後払い.com後払いキャンセル連動" Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCvsDefScoreCancel" Text=" スコア後払いキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCvsDefVeritransCancel" Text=" ベリトランス後払いキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbAmazonPayCancel" Text=" Amazon Payキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbAmazonPayCV2Cancel" Text=" Amazon Pay(CV2)キャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbPaypalCancel" Text=" PayPalキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbTriLinkAfterPay" Text=" 後付款(TriLink後払い)キャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbPaidyPayCancel" Text=" Paidy決済キャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbAtonePayCancel" Text=" atone翌月払いキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbAfteePayCancel" Text=" aftee翌月払いキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbLinePayCancel" Text=" LINE Pay決済キャンセル " Visible="false" Font-Size="8pt" runat="server"/>
															<asp:CheckBox ID="cbNPAfterPayCancel" Text=" NP後払いキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbEcPayCancel" Text=" ECPay決済キャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbGooddealShippingCancel" Text=" Gooddeal出荷キャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbNewebPayCancel" Text=" 藍新Pay決済キャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbVeriTransCancel" Text=" ベリトランスクレジットキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbPayPayCancel" Text=" PayPay決済キャンセル " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbCreditRakutenCancel" Text=" 楽天カードのクレジットキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbGmoCancel" Text=" GMO掛け払いキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:RadioButton ID="rbTwInvoiceCancel" Text="電子発票キャンセル"  Visible="false" Font-Size="8pt" GroupName="twInvoiceCancel" runat="server"/>
															<asp:RadioButton ID="rbTwInvoiceRefund" Text="電子発票払い戻し" Visible="false" Font-Size="8pt" GroupName="twInvoiceCancel" runat="server"/>
															<asp:CheckBox ID="cbBokuCancel" Text=" Bokuキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbGmoAtokaraCancel" Text=" アトカラキャンセル連動 " Visible="false" Font-Size="8pt" runat="server" />
															<asp:CheckBox ID="cbRecustomerCooperation" Text=" Recustomer受注連携 " Visible="false" Font-Size="8pt" runat="server" />
															<div id="canselAttentionMessage" Visible="false" style="color:red;font-size:8pt;" runat="server"></div>
															<span style="display:block">
																<asp:Button ID="btnUpdateOrderStatus" Runat="server" Text="  注文ステータス更新  " Width="120" OnClick="btnUpdateOrderStatus_Click" OnClientClick="if (alert_for_update_orderstatus()) return disable_button($(this)); return false" />
																<span class="settlementInProcessing" style="display:none">連携処理中です</span>
															</span>
														</td>
													</tr>
													<tr id="trOrderStatusError" class="list_alert" runat="server" visible="false">
														<td colspan="3" ><asp:label ID="lOrderStatusError" CssClass="notice" style="font-weight:bold;line-height:normal;" runat="server"></asp:label></td>
													</tr>
												</table>
												</ContentTemplate>
												</asp:UpdatePanel>

												</div>
												<%--△ 注文ステータス更新 △--%>
												<%--▽ 店舗受取ステータス更新 ▽--%>
												<div id="dvStorePickupStatus" runat="server">
												<br />
												<asp:UpdatePanel id="upStorePickupStatus" runat="server">
													<ContentTemplate>
														<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tr>
																<td class="detail_title_bg" align="center" colspan="3">店舗受取ステータス更新</td>
															</tr>
															<tr>
																<td class="detail_title_bg" width="120">店舗受取ステータス</td>
																<td class="detail_item_bg">
																	<asp:RadioButtonList id="rblStorePickupStatus" OnSelectedIndexChanged="DisplayControl" AutoPostBack="true" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
																</td>
																<td class="detail_item_bg" width="120" style="white-space: nowrap;">
																	<span style="display:block">
																		<asp:CheckBox ID="cbSendMailStorePickupStatus" Text="  店舗到着メールを送信する  " runat="server" />
																	</span>
																</td>
															</tr>
															<tr>
																<td class="detail_title_bg" width="120">更新日指定</td>
																<td class="detail_item_bg" align="left">&nbsp;
																	<uc:DateTimePickerPeriodInput id="ucUpdateStorePickupStatusDate" CanShowEndDatePicker="False" UpdatePanelControlId="<%#: upStorePickupStatus.ID %>" runat="server" IsHideTime="true" />
																</td>
																<td class="detail_item_bg" align="right" width="120" style="white-space: nowrap;">
																	<span style="display:block">
																		<asp:Button ID="btnUpdateStorePickupStatus" Runat="server" Text="  店舗受取ステータス更新  " Width="120" OnClick="btnUpdateStorePickupStatus_Click" />
																	</span>
																</td>
															</tr>
														</table>
													</ContentTemplate>
												</asp:UpdatePanel>

												</div>
												<%--△ 店舗受取ステータス更新 △--%>
												<%--▽ 実在庫更新 ▽--%>
												<div id="divRealStock" runat="server">
												<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
												<br />
												<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">実在庫更新</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="120">実在庫連動処理</td>
														<td class="detail_item_bg" align="left" width="518">
															<asp:RadioButtonList ID="rblProductRealStockChange" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
														</td>
														<td class="detail_item_bg" align="right" width="120">
															<asp:Button ID="btnUpdateProductRealStock" Runat="server" Text="  実在庫更新  " Width="120" OnClick="btnUpdateProductRealStock_Click" />
														</td>
													</tr>
												</table>
												<br />
												<% } %>
												<%--△ 実在庫利用が有効な場合は表示 △--%>
												</div>
												<%--△ 実在庫更新 △--%>
												<%--▽ 入金ステータス更新 ▽--%>
												<asp:UpdatePanel ID="upOrderPaymentStatus" runat="server">
												<ContentTemplate>
												
												<div id="divOrderPaymentStauts" runat="server">
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">入金ステータス更新</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="120">入金ステータス</td>
														<td class="detail_item_bg" colspan="2">
															<asp:RadioButtonList id="rblOrderPaymentStatus" OnSelectedIndexChanged="DisplayControl" AutoPostBack="true" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">更新日指定</td>
														<td class="detail_item_bg" align="left" width="518">&nbsp;
															<uc:DateTimePickerPeriodInput id="ucUpdateOrderPaymentStatusDate" CanShowEndDatePicker="False" UpdatePanelControlId="<%#: upOrderPaymentStatus.ID %>" runat="server" />
														</td>
														<td class="detail_item_bg" align="right" width="120">
															<asp:Button ID="btnUpdateOrderPaymentStatus" Runat="server" Text="  入金ステータス更新  " Width="120" OnClick="btnUpdateOrderPaymentStatus_Click" />
														</td>
													</tr>
													<tr id="trOrderPaymentStatus" class="list_alert" runat="server" visible="false">
														<td colspan="3" ><asp:label ID="lOrderPaymentStatus" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server"></asp:label></td>
													</tr>
													<%-- GMO後払いの場合のみ表示 --%>
													<% if (CanDisplayGetExternalOrderPaymentStatus(this.OrderInput.OrderPaymentKbn)) { %>
													<tr>
														<td class="detail_title_bg">入金状態</td>
														<td class="detail_item_bg" align="left" width="518">
															<asp:Literal runat="server" ID="lOrderPaymentState" />
															<asp:Literal runat="server" ID="lOrderPaymentDate" />
															<span style="color: red" runat="server" ID="spErrorPaymentStatus" Visible="False">
																<asp:Literal runat="server" ID="lOrderPatmentErrorMessage" /><br/><br/>
																<span style="font-size: 70%">
																	詳細についてはGMOの管理画面を確認してください
																</span>
															</span>
														</td>
														<td class="detail_item_bg" align="right" width="120">
															<asp:Button ID="btnCheckOrderPaymentStatus" runat="server" Text="  状態取得  " Width="120" OnClick="btnCheckOrderPaymentStatus_Click" />
														</td>
													</tr>
													<% } %>
												</table>
												<%--△ 入金ステータス更新 △--%>
												<%--▽ 外部決済連携の処理 ▽--%>
												<% if (GetCanExternalPaymentRealSales(this.OrderInput.OrderPaymentKbn)) { %>
													<br />
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="3">外部決済連携</td>
														</tr>
														<tr>
															<td class="detail_title_bg" width="120">オンライン決済ステータス</td>
															<td class="detail_item_bg" colspan="2"><%: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS, this.OrderInput.OnlinePaymentStatus) %></td>
														</tr>
														<%--▽ 実売上連動処理を利用するの場合 ▽--%>
														<% if (GetCanExternalPaymentCreditRealSales(this.OrderInput.OrderPaymentKbn)) { %>
														<tr>
															<td class="detail_title_bg" width="120">外部決済連携</td>
															<td class="detail_item_bg"  colspan="2">実売上連動処理<br />※クレジット決済の売上確定処理</td>
														</tr>
														<tr>
															<td class="detail_title_bg">処理日指定</td>
															<td class="detail_item_bg" align="left" width="518">&nbsp;
																<uc:DateTimePickerPeriodInput id="ucCardRealSalesDate" CanShowEndDatePicker="False" UpdatePanelControlId="<%#: upOrderPaymentStatus.ID %>" runat="server" />
															</td>
															<td class="detail_item_bg" align="center" width="120">
																<asp:Button ID="btnCardRealSales" Runat="server" Text="  処理実行  " Width="120" OnClientClick="return disable_button($(this))" OnClick="btnCardRealSales_Click" />
																<span class="settlementInProcessing" style="display:none">連携処理中です</span>
															</td>
														</tr>
														<%--△ 実売上連動処理を利用するの場合 △--%>
														<%} else { %>
														<tr>
															<td class="detail_title_bg" width="120">外部決済連携</td>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG) { %>
															<td class="detail_item_bg" width="518">ドコモケータイ払い決済<br />※売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG) { %>
															<td class="detail_item_bg" width="518">S!まとめて支払い<br />※売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS) { %>
															<td class="detail_item_bg" width="518">ソフトバンク・ワイモバイルまとめて支払い<br />※売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS) { %>
															<td class="detail_item_bg" width="518">ドコモケータイ払い決済<br />※売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS) { %>
															<td class="detail_item_bg" width="518">auかんたん決済<br />※売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS) { %>
															<td class="detail_item_bg" width="518">リクルートかんたん支払い<br />※売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS) { %>
															<td class="detail_item_bg" width="518">楽天ペイ<br />※売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) { %>
															<td class="detail_item_bg" width="518">Amazon Pay<br />※売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2) { %>
															<td class="detail_item_bg" width="518">Amazon Pay(CV2)<br />※売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL){ %>
																<td class="detail_item_bg" width="518">PayPal決済<br />
																	<%if (Constants.PAYPAL_PAYMENT_METHOD != w2.App.Common.Constants.PayPalPaymentMethod.AUTH_WITH_SUBMIT) {%>
																	※売上確定処理
																	<% } %>
																</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY) { %>
																<td class="detail_item_bg" width="518">実売上連動処理<br />※Paidy決済の売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE) { %>
																<td class="detail_item_bg" width="518">atone翌月払い<br />※売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE) { %>
																<td class="detail_item_bg" width="518">aftee翌月払い<br />※売上確定処理</td>
															<% } %>
															<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
																<% if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)) { %>
																	<td class="detail_item_bg" width="518">GMO掛け払い<br />※請求確定処理</td>
																<% } %>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY) { %>
															<td class="detail_item_bg" width="518">実売上連動処理<br />※LINE Pay決済の売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) { %>
																<td class="detail_item_bg" width="518">ECPay<br />※決済の売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) { %>
																<td class="detail_item_bg" width="518">藍新Pay<br />※決済の売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY) { %>
																<td class="detail_item_bg" width="518">実売上連動処理<br />※PayPay決済の売上確定処理</td>
															<% } %>
															<% if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU) { %>
																<td class="detail_item_bg" width="518">Boku<br />※決済の売上確定処理</td>
															<% } %>
															<td class="detail_item_bg" align="center" width="120">
																<asp:Button ID="btnExternalPayment" Text="  処理実行  " OnClientClick="return disable_button($(this))" OnClick="btnExternalPayment_Click" Width="120" runat="server" />
																<span class="settlementInProcessing" style="display:none">連携処理中です</span>
															</td>
														</tr>
														<% } %>
														<tr id="trOuterPaymentError" class="list_alert" runat="server" visible="false">
															<td colspan="5" ><asp:label ID="lOuterPaymentError" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server"></asp:label></td>
														</tr>
													</table>
												<% } %>
												<%--△ 外部決済連携の処理 △--%>
												<asp:HiddenField id="hfOrderPaymentKbn" runat="server" />
												</div>
												</ContentTemplate>
												</asp:UpdatePanel>
												<%--▽ 返品交換ステータス ▽--%>
												<asp:UpdatePanel ID="UpdatePanel2" runat="server">
												<ContentTemplate>
												<div id="divOrderReturnExchangeStatus" runat="server">
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">返品交換ステータス更新</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="120">返品交換ステータス</td>
														<td class="detail_item_bg" colspan="2">
															<asp:RadioButtonList id="rblOrderReturnExchangeStatus" OnSelectedIndexChanged="DisplayControl" AutoPostBack="true" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">更新日指定</td>
														<td class="detail_item_bg" align="right"  width="498">&nbsp;
															<uc:DateTimeInput ID="ucOrderReturnExchangeStatusDate" runat="server" YearList="<%# DateTimeUtility.GetBackwardYearListItem() %>" HasTime="False" HasBlankSign="False" HasBlankValue="True" />
														</td>
														<td class="detail_item_bg" align="right" width="140">
															<asp:Button ID="btnUpdateOrderReturnExchangeStatus" Runat="server" Text="  返品交換ステータス更新  " Width="140" OnClick="btnUpdateOrderReturnExchangeStatus_Click" />
														</td>
													</tr>
													<tr id="trOrderReturnExchagneStatus" class="list_alert" runat="server" visible="false">
														<td colspan="5" ><asp:label ID="lOrderReturnExchangeStatus" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server"></asp:label></td>
													</tr>
												</table>
												</div>
												</ContentTemplate>
												</asp:UpdatePanel>
												<%--△ 返品交換ステータス △--%>
												<%--▽ 返金ステータス ▽--%>
												<asp:UpdatePanel ID="UpdatePanel3" runat="server">
												<ContentTemplate>
												<div id="divOrderRepaymentStatus" runat="server">
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">返金ステータス更新</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="120">返金ステータス</td>
														<td class="detail_item_bg" colspan="2">
															<asp:RadioButtonList id="rblOrderRepaymentStatus" OnSelectedIndexChanged="DisplayControl" AutoPostBack="true" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">更新日指定</td>
														<td class="detail_item_bg" align="right" width="498">&nbsp;
															<uc:DateTimeInput ID="ucOrderRepaymentStatusDate" runat="server" YearList="<%# DateTimeUtility.GetBackwardYearListItem() %>" HasTime="False" HasBlankSign="False" HasBlankValue="True" />
														</td>
														<td class="detail_item_bg" align="right" width="140">
															<asp:Button ID="btnUpdateOrderRepaymentStatus" Runat="server" Text="  返金ステータス更新  " Width="140" OnClick="btnUpdateOrderRepaymentStatus_Click" />
														</td>
													</tr>
													<tr id="trOrderRepaymentStatus" class="list_alert" runat="server" visible="false">
														<td colspan="5" ><asp:label ID="lOrderRepaymentStatus" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server"></asp:label></td>
													</tr>
												</table>
												</div>
												</ContentTemplate>
												</asp:UpdatePanel>
												<%--△ 返金ステータス △--%>
												<div runat="server" ID="divCvsDefInvoiceReissue" Visible="false">
													<div id="divReissue" runat="server">
														<br />
														<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tr>
																<td class="detail_title_bg" align="center" colspan="3">後払い請求書再発行</td>
															</tr>
															<tr>
																<td class="detail_title_bg" width="120">請求書再発行</td>
																<td class="detail_item_bg"width="518"></td>
																<td class="detail_item_bg" align="right" width="120">
																	<asp:Button ID="btnCvsDefInvoiceReissue" runat="server" Text="  請求書再発行実行  " OnClick="btnCvsDefInvoiceReissue_Click" />
																</td>
															</tr>
														</table>
													</div>
												</div>
												<%--▽ 外部受注情報連携 ▽--%>
												<asp:UpdatePanel runat="server">
													<ContentTemplate>
														<div id="divOrderDelivertyStatus" runat="server">
															<br />
															<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tr>
																	<td class="detail_title_bg" align="center" colspan="3">外部受注情報連携</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" width="120">オンライン物流ステータス</td>
																	<td class="detail_item_bg" colspan="2"><%: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ONLINE_DELIVERY_STATUS, this.OrderInput.OnlineDeliveryStatus) %></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" width="120">外部受注情報連携</td>
																	<td class="detail_item_bg" width="518">ECPay<br />※ECPayの受注情報処理</td>
																	<td class="detail_item_bg" align="center" width="120">
																		<asp:Button
																			ID="btnExternalOrderInfoAction"
																			Text="  処理実行  "
																			OnClientClick="return disable_button($(this))"
																			OnClick="btnExternalOrderInfoAction_Click"
																			Width="120"
																			runat="server" />
																		<span class="settlementInProcessing" style="display:none">連携処理中です</span>
																	</td>
																	<tr id="trErrorExternalOrderInfo" class="list_alert" runat="server" visible="false">
																		<td colspan="5" >
																			<asp:label ID="lbErrorExternalOrderInfo" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server" />
																		</td>
																	</tr>
																</tr>
															</table>
														</div>
													</ContentTemplate>
												</asp:UpdatePanel>
												<%--△ 外部受注情報連携 △--%>
												<%--▽ 督促ステータス更新 ▽--%>
												<% if (Constants.DEMAND_OPTION_ENABLE) { %>
												<asp:UpdatePanel id="upDemandStatus" runat="server">
												<ContentTemplate>
												<div id="divDemandStatus" runat="server">
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">督促ステータス更新</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="120">督促ステータス</td>
														<td class="detail_item_bg" colspan="2">
															<asp:RadioButtonList
																id="rblDemandStatus"
																OnSelectedIndexChanged="DisplayControl"
																AutoPostBack="true"
																runat="server"
																RepeatDirection="Horizontal"
																RepeatLayout="Flow" />
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">更新日指定</td>
														<td class="detail_item_bg" align="left" width="518">&nbsp;
															<uc:DateTimePickerPeriodInput id="ucUpdateDemandStatusDate" CanShowEndDatePicker="False" UpdatePanelControlId="<%#: upDemandStatus.ID %>" runat="server" />
														</td>
														<td class="detail_item_bg" align="right" width="120">
															<asp:Button ID="btnUpdateDemandStatus" Runat="server" Text="  督促ステータス更新  " Width="120" OnClick="btnUpdateDemandStatus_Click" />
														</td>
													</tr>
													<tr id="trDemandStatus" class="list_alert" runat="server" visible="false">
														<td colspan="3" ><asp:label ID="lDemandStatus" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server"></asp:label></td>
													</tr>
												</table>
												</div>
												</ContentTemplate>
												</asp:UpdatePanel>
												<% } %>
												<%--△ 督促ステータス更新 △--%>
												<%--▽ 拡張ステータス ▽--%>
												<asp:UpdatePanel id="upExtendStatus" runat="server">
												<ContentTemplate>
												<div id="divExtend" runat="server">
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<asp:Repeater ID="rOrderExtendStatusList" runat="server" OnItemCommand="rExtendStatus_ItemCommand" ItemType="System.Data.DataRowView">
														<HeaderTemplate>
															<tr>
																<td class="detail_title_bg" align="center" colspan="5">拡張ステータス更新</td>
															</tr>
														</HeaderTemplate>
														<ItemTemplate>
															<tr>
																<td class="detail_title_bg" width="200">
																	拡張ステータス<asp:Literal ID="lExtendNo" runat="server" />：<br />&nbsp;<asp:Literal ID="lExtendName" runat="server" />
																</td>
																<td class="detail_item_bg">
																	<asp:RadioButtonList id="rblExtend" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio_button_list"></asp:RadioButtonList>
																</td>
																<td class="detail_title_bg" width="80">
																	更新日指定
																</td>
																<td class="detail_item_bg" align="right">
																	&nbsp;
																	<uc:DateTimePickerPeriodInput id="ucExtendStatusDate" CanShowEndDatePicker="False" UpdatePanelControlId="<%#: upExtendStatus.ID %>" runat="server" />
																</td>
																<td class="detail_item_bg" align="right" width="120">
																	<asp:Button ID="btnUpdateExtendStatus" Runat="server" CommandName="update" Text='<%# "ステータス" + Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString() + "更新" %>' Width="120" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trExtendStatus" class="list_alert" runat="server" visible="false">
														<td colspan="5" ><asp:label ID="lExtendStatus" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server"></asp:label></td>
													</tr>
												</table>
												</div>
												</ContentTemplate>
												</asp:UpdatePanel>
												<%--△ 拡張ステータス △--%>
												<br />
												<% if (Constants.URERU_AD_IMPORT_ENABLED) { %>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">外部連携ステータス更新</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="120">外部連携取込ステータス</td>
														<td class="detail_item_bg" align="left" width="445">
															<asp:RadioButtonList ID="rblExternalImportStatus" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"></asp:RadioButtonList>
														</td>
														<td class="detail_item_bg" align="center" width="153">
															<asp:Button ID="btnUpdateExternalImportStatus" OnClick="btnUpdateExternalImportStatus_Click" Text=" 外部連携取込ステータス更新 " Width="153" runat="server" />
														</td>
													</tr>
													<tr id="trExternalOrderImportStatus" class="list_alert" runat="server" visible="false">
														<td colspan="5"><asp:Label ID="lExternalOrderImportStatus" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server"></asp:Label></td>
													</tr>
												</table>
												<br />
												<%} %>

												<%--▽ Amazon連携ステータス ▽--%>
												<% if (this.OrderInput.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON) { %>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">モール連携ステータス更新</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="120">モール連携取込ステータス</td>
														<td class="detail_item_bg" align="left" width="445">
															<asp:RadioButtonList ID="rblMallLinkStatus" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"></asp:RadioButtonList>
														</td>
														<td class="detail_item_bg" align="center" width="153">
															<asp:Button ID="btnUpdateMallLinkStatus" OnClick="btnUpdateMallLinkStatus_Click" Text=" モール連携取込ステータス更新 " Width="153" runat="server" />
														</td>
													</tr>
												</table>
												<br />
												<%} %>
												<% if (Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED) { %>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">Gooddeal連携ステータス</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="85">Gooddeal連携取込ステータス</td>
														<td class="detail_item_bg" align="left" width="320">
															<asp:RadioButtonList ID="rblGooddealDeliveryStatus" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"/>
														</td>
														<td class="detail_item_bg" align="center" width="153">
															<asp:Button ID="btnUpdateGooddealDeliveryStatus" OnClick="btnUpdateGooddealDeliveryStatus_Click" Text="  Gooddeal連携取込  " Width="153" runat="server" />
														</td>
													</tr>
													<tr id="trErrorGooddealDeliveryStatus" class="list_alert" runat="server" visible="false">
														<td colspan="5">
															<span class="notice" style="font-weight:bold; line-height:normal;">
																<asp:Label ID="lErrorGooddealDeliveryStatus" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server" />
															</span>
														</td>
													</tr>
												</table>
												<br />
												<%} %>

												<%--▽ メール送信 ▽--%>
												<a id="toSendMailForm" style="display: block; margin-top: -100px; padding-top: 100px"></a>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">メール送信</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="120">メールテンプレート
														</td>
														<td class="detail_item_bg" align="left" width="445">
															<asp:DropDownList id="ddlOrderMailId" runat="server" Width="100%"></asp:DropDownList>
														</td>
														<td class="detail_item_bg" align="center" width="153">
															<input type="button" style="width:153px" value="  メール送信フォームへ  " onclick="javascript:open_order_send_mail();" />
														</td>
													</tr>
												</table>
												<%--△ メール送信 △--%>
												<br />
												<%--▽ 受注情報表示 ▽--%>
												<uc:BodyOrderConfirm id="ucBodyOrderConfirm" OrderInput='<%# this.OrderInput %>' runat="server"></uc:BodyOrderConfirm>
												<%--△ 受注情報表示 △--%>
												<div class="action_part_bottom" runat="server">
												<asp:Button ID="btnBackListBottom" runat="server" Text="  一覧へ戻る  " onclick="btnBackList_Click" />
												<asp:Button ID="btnReOrderBottom" runat="server" Text="  再注文する  " OnClientClick="return confirm_reorder()" onclick="btnReOrder_Click" />
												<asp:Button id="btnReturnExchangeBottom" runat="server" Text="  返品交換する  " OnClick="btnReturnExchange_Click" OnClientClick="return check_change();"/>
												<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " OnClick="btnEdit_Click" OnClientClick="return check_change();"/>
												</div>
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
	// 編集ボタン押下時
	function check_change() {
		if ('<%= this.OrderInput.OrderPaymentKbn %>' == '<%= Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT %>') {
			var cooperationIdArray = '<%= (this.UserCreditCard != null ? this.UserCreditCard.CooperationId : "") %>'.split(',');

			if ((cooperationIdArray[0] == "") && (cooperationIdArray.length == 2)) {
				return confirm("外部システムから取り込んだ注文のため決済連動できません。\n金額変更がある場合は決済代行会社の管理画面より直接変更する必要があります。");
			}
		}
		return true;
	}
</script>
</asp:Content>
