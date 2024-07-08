<%--
=========================================================================================================
  Module      : 店舗受取注文詳細画面 (StorePickUpOrderDetail.aspx)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="uc" TagName="BodyOrderConfirm" Src="~/Form/Common/BodyOrderConfirm.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage_responsive.master" AutoEventWireup="true" CodeFile="StorePickUpOrderDetail.aspx.cs" Inherits="Form_StorePickUp_StorePickUpOrderDetail" MaintainScrollPositionOnPostback="true" %>

<%@ Import Namespace="w2.App.Common.Input.Order" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.Domain.Order" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<script type="text/javascript">
	<!--
	// 注文メール送信画面表示
	function open_order_send_mail() {
		if (document.getElementById('<%= ddlOrderMailId.ClientID %>').value != '') {
			var url = '<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_SEND %>'
				+ '?' + '<%= Constants.REQUEST_KEY_ORDER_ID %>' + '=' + '<%= HttpUtility.UrlEncode(this.OrderInput.OrderId) %>'
				+ '&' + '<%= Constants.REQUEST_KEY_MAIL_TEMPLATE_ID %>' + '=' + encodeURIComponent(document.getElementById('<%= ddlOrderMailId.ClientID %>').value);

			open_window(url, 'order_send_mail', 'width=850,height=825,top=120,left=320,status=NO,scrollbars=yes');
		}
		else {
			alert('メールテンプレートを選択してください');
		}
	}

	function disable_button(button) {
		button.hide();
		button.next('.settlementInProcessing').show();
		return true;
	}

	// 再注文へ進む確認ボックス表示
	function confirm_reorder() {
		if (confirm('再注文のため、注文情報登録画面へ遷移致します。\r\nよろしいでしょうか。') == true) {
			return true;
		} else {
			return false;
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
	window.onload = function () {
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
		<tr>
			<td><h1 class="page-title">店舗受取注文情報</h1></td>
		</tr>
		<tr>
			<td><img height="10" width="100" border="0" alt="" src="../../Images/Common/sp.gif" /></td>
		</tr>
		<% } %>
		<% if (this.IsPopUp) { %>
		<tr>
			<td><h2 class="page-title">店舗受取注文情報詳細</h2></td>
		</tr>
		<% } else { %>
		<tr>
			<td><h2 class="cmn-hed-h2">店舗受取注文情報詳細</h2></td>
		</tr>
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
											<tbody id="tbdyCreditCardInputButtonZeus" visible="False" runat="server">
												<tr>
													<td>
														<center>
															<asp:Button ID="btnRegisterCreditCard" Text="クレジットカードで決済する" runat="server" />
														</center>
														<br />
													</td>
												</tr>
											</tbody>
											<tbody id="tbdyCreditCardInputUnvisibleMessageZeus" visible="False" runat="server">
												<tr>
													<td>
														<center class="notice">
															クレジットカード番号は入力できません。<br />
															クレジットカード番号の入力は決済用タブレットにて行ってください。
														</center>
														<br />
													</td>
												</tr>
											</tbody>
											<%-- 仮クレジットカード登録待ち向け表示 --%>
											<tbody id="tbdyCreditCardRegisterForUnregisterd" visible="False" runat="server">
												<tr>
													<td>
														<center>
															<table class="detail_table" border="0" cellspacing="1" cellpadding="3" style="min-width: 300px;">
																<tr>
																	<td align="left" class="detail_item_bg" style="font-size: 11pt">
																		<div style="text-align: center">
																			<span style="color: red">
																				<div id="divCreditCardAuthErrorMessage" visible="False" runat="server">
																					クレジットカード与信が失敗しました。<br />
																					新たにクレジットカード登録から行ってください。<br />
																					<br />
																				</div>
																				クレジットカード登録は保留状態です。<br />
																				下記の手順で登録・与信を行ってください。<br />
																				（カード登録ごとに
																				<%if (OrderCommon.IsPaymentCardTypeGmo)
																					{%>GMO会員<%} %>
																				<%if (OrderCommon.IsPaymentCardTypeYamatoKwc)
																					{%>ヤマト会員<%} %>
																				を登録する必要があります。）<br />
																				<br />
																				※注文変更・返品交換の場合、以前取得していた与信は既に取り消し済みです。<br />
																			</span>
																		</div>
																		<br />
																		<%if (OrderCommon.IsPaymentCardTypeGmo) {%>
																		<div style="text-align: left;">
																			①決済用タブレットで「会員・カード登録」を開き、<br />
																			クレジットカード番号と下記情報IDにてカードの登録を行ってください。<br />
																			<br />
																			<span style="font-size: large; line-height: 18px; text-align: center;">GMO会員ID：<asp:Literal ID="lGmoMemberId" runat="server"></asp:Literal><br />
																			</span>
																			<br />
																			※GMO会員IDはハイフンなしで入力してください。<br />
																			<%} %>
																			<%if (OrderCommon.IsPaymentCardTypeYamatoKwc) {%>
																			<div style="text-align: left;">
																				①決済用タブレットにて下記情報で与信を実行してください。<br />
																				<br />
																				<span style="font-size: large; line-height: 18px; text-align: center;">受付番号　　：<asp:Literal ID="lYamatoKwcOrderNo" runat="server"></asp:Literal><br />
																					ヤマト会員ID：<asp:Literal ID="lYamatoKwcMemberId" runat="server"></asp:Literal><br />
																					認証キー　　：<asp:Literal ID="lYamatoKwcAuthenticationKey" runat="server"></asp:Literal><br />
																					金額　　　　：1 円<br />
																				</span>
																				<br />
																				※受付番号・ヤマト会員IDはハイフンなしで入力してください。<br />
																				※１円与信はヤマト会員登録のために利用します。<br />
																				ヤマト会員登録後、この1円与信は取り消しされます。<br />
																				<%} %>
																				<%if (OrderCommon.IsPaymentCardTypeEScott) {%>
																				<div style="text-align: left;">
																					①決済用タブレットにて、クレジットカード情報と下記情報で会員登録を実行してください。<br />
																					<br />
																					<span style="font-size: large; line-height: 18px; text-align: center;">会員ID　　　　：<asp:Literal ID="lEScottKaiinId" runat="server"></asp:Literal><br />
																					</span>
																					<br />
																					※会員IDはハイフンなしで入力してください。
																				<%} %>
																					<br />
																					②登録が完了したらリロードを行い、<br />
																					画面が切り替わったら与信を実行するボタンをクリックしてください。<br />
																				</div>
																				<br />
																				<div style="text-align: center">
																					<asp:Button ID="btnReloadForRegisterdCreditCard" Text="    リロード    " runat="server" /><br />
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
											<tbody id="tbdyCreditCardRegisternForUnauthed" visible="False" runat="server">
												<tr>
													<td style="text-align: center;">
														<span class="notice">
															<%if (OrderCommon.IsPaymentCardTypeYamatoKwc) {%>
																１円与信は正常に取消されました。<br />
															<%} %>
															注文のクレジットカード与信は保留状態のため、<br />
															下記のボタンをクリックし、与信を実行してください。<br />
														</span>
														<br />
														<table class="edit_table" cellspacing="1" cellpadding="6" width="758" border="0">
															<tr>
																<td class="edit_title_bg" align="left">クレジットカード情報</td>
																<td class="edit_item_bg" align="left">
																	<asp:Literal ID="lCreditCardInfo" runat="server" />
																</td>
															</tr>
															<tr>
																<td class="edit_title_bg" align="left">合計金額</td>
																<td class="edit_item_bg" align="left">
																	<span style="font-size: large; line-height: 18px">
																		<%: this.OrderInput.LastBilledAmount.ToPriceString(true) %>
																	</span>
																</td>
															</tr>
															<%-- ▽分割支払い有効の場合は表示▽ --%>
															<tr id="trInstallments" runat="server">
																<td class="edit_title_bg" align="left">支払回数<span class="notice">*</span></td>
																<td class="edit_item_bg" align="left">
																	<asp:DropDownList ID="dllCreditInstallments" runat="server" />&nbsp;&nbsp;※AMEX/DINERSは一括のみとなります。
																</td>
															</tr>
															<%-- △分割支払い有効の場合は表示△ --%>
															<tr class="creditCardItem" style="height: 27px;" id="trRegistCreditCard" runat="server">
																<td align="left" class="edit_title_bg" style="width: 200px;">登録する</td>
																<td class="edit_item_bg">
																	<asp:CheckBox ID="cbRegistCreditCard" runat="server" Text="  登録する" AutoPostBack="True" />
																</td>
															</tr>
															<tr class="creditCardItem" style="height: 27px;" id="trCreditCardName" runat="server" visible="False">
																<td align="left" class="edit_title_bg">クレジットカード登録名 <span class="notice">*</span></td>
																<td class="edit_item_bg">
																	<asp:TextBox ID="tbUserCreditCardName" runat="server" MaxLength="30" />&nbsp;&nbsp;※クレジットカードを保存する場合は、以下をご入力ください。
																</td>
															</tr>
														</table>
														<div class="notice" style="margin: 5px;">
															<asp:Literal ID="lCreditCardRegisternForUnauthed" runat="server" />
														</div>
														<asp:Button
															ID="btnRegisterUnregisterdCreditCardForAuthError"
															Text="仮クレジットカードを再登録する"
															Visible="False"
															runat="server"
															Enabled="false"/><br />

														<asp:Button
															ID="btnAuthRegisterdCreditCard"
															Text="与信を実行する"
															OnClientClick="this.disabled=true;"
															runat="server"
															UseSubmitBehavior="False"
															Enabled="false" /><br />
													</td>
												</tr>
											</tbody>
											<tr id="trFirstPointAlert" runat="server" visible="false">
												<td>
													<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3" style="margin-top: <%= this.IsPopUp ? "45px" : "0px" %>">
														<tr>
															<td align="left" class="info_item_bg" style="color: red; font-weight: bold">このユーザには今回の注文の他にも初回購入ポイントが割り当てられています。<br />
																ポイントオプションより調整を行う必要があります。
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr id="trSendMailError" runat="server" visible="false">
												<td>
													<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3" style="margin-top: <%= this.IsPopUp ? "45px" : "0px" %>">
														<tr>
															<td align="left" class="info_item_bg" style="color: red; font-weight: bold">
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
																		<strong>
																			<asp:Literal ID="lReturnOrderReauthErrormessages" runat="server" />
																		</strong>
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
														<asp:Button ID="btnBackListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnBackList_Click" />
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
																		<td class="detail_item_bg" align="left">
																			<a href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(FixedPurchasePage.CreateFixedPurchaseDetailUrl(this.OrderInput.FixedPurchaseId, true)) %>','fixedpurchase','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%: this.OrderInput.FixedPurchaseId %></a>
																		</td>
																		<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
																		<td class="detail_item_bg" align="left"><a href="javascript:open_window('<%:FixedPurchasePage.CreateSubscriptionBoxRegisterUrl(this.OrderInput.SubscriptionBoxCourseId) %>','fixedpurchase','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%: this.OrderInput.SubscriptionBoxCourseId %></a></td>
																		<%} %>
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
																				<td align="left"><%#: Item.OrderId %><%#: (Item.IsLastAuthFlgON) ? " （最終与信）" : string.Empty %></td>
																				<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
																					{ %>
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
																		<td align="left"><%#: Item.OrderId %></td>
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
														<asp:UpdatePanel ID="upOrderStatus" runat="server">
															<ContentTemplate>
																<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
																	<tr>
																		<td class="detail_title_bg" align="center" colspan="3">注文ステータス更新</td>
																	</tr>
																	<tr>
																		<td class="detail_title_bg" width="120">注文ステータス</td>
																		<td class="detail_item_bg" colspan="2">
																			<asp:RadioButtonList
																				ID="rblOrderStatus"
																				OnSelectedIndexChanged="DisplayControl"
																				AutoPostBack="true"
																				runat="server"
																				RepeatDirection="Horizontal"
																				RepeatLayout="Flow"
																				Enabled="false" />
																			<asp:HiddenField ID="hfOrderStatus" runat="server" />

																			<span runat="server" id="sAtodenePaymentDeadlineAlert" visible="False" style="color: red">
																				<br />
																				出荷手配済みに変更した場合は、Atodene後払い請求書の支払い期限が確定するのでご注意ください。
																			</span>
																		</td>
																	</tr>
																	<tr>
																		<td class="detail_title_bg" width="120">更新日指定</td>
																		<td class="detail_item_bg" align="left">&nbsp;
																			<uc:DateTimePickerPeriodInput
																				ID="ucUpdateOrderStatusDate"
																				CanShowEndDatePicker="False"
																				UpdatePanelControlId="<%#: upOrderStatus.ID %>"
																				runat="server"
																				IsHideTime="true" />
																		</td>
																		<td class="detail_item_bg" align="right" width="120" style="white-space: nowrap;">
																			<asp:CheckBox
																				ID="cbReauthCancel"
																				Text=" 決済システム連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCreditZeusCancel"
																				Text=" ZEUSキャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCreditSBPSCancel"
																				Text=" SBPSクレジットキャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCreditYamatoKwcCancel"
																				Text=" ヤマトKWCクレジットキャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCreditGMOCancel"
																				Text=" GMOクレジットキャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCreditZcomCancel"
																				Text=" Zcomクレジットキャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCreditEScottCancel"
																				Text=" e-SCOTTキャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCareerDocoomKetaiSBPSCancel"
																				Text=" ドコモケータイ払いキャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCareerAuKantanSBPSCancel"
																				Text=" auかんたん決済キャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server" />
																			<asp:CheckBox
																				ID="cbCareerSoftbankKetaiSBPSCancel"
																				Text=" ソフトバンク・ワイモバイルまとめて支払いキャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbRecruitSBPSCancel"
																				Text=" リクルートかんたん支払いキャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbPaypalSBPSCancel"
																				Text=" PayPal(SBPS)キャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbRakutenIdSBPSCancel"
																				Text=" 楽天ペイ(SBPS)キャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbYamatoKaCancel"
																				Text=" ヤマト後払いキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCvsDefGmoCancel"
																				Text=" GMO後払いキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCvsDefAtodeneCancel"
																				Text=" Atodene後払いキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCvsDefDskCancel"
																				Text=" DSK後払いキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCvsDefAtobaraicomCancel"
																				Text="後払い.com後払いキャンセル連動"
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbAmazonPayCancel"
																				Text=" Amazon Payキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbAmazonPayCV2Cancel"
																				Text=" Amazon Pay(CV2)キャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbPaypalCancel"
																				Text=" PayPalキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbTriLinkAfterPay"
																				Text=" 後付款(TriLink後払い)キャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbPaidyPayCancel"
																				Text=" Paidy決済キャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbAtonePayCancel"
																				Text=" atone翌月払いキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbAfteePayCancel"
																				Text=" aftee翌月払いキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbLinePayCancel"
																				Text=" LINE Pay決済キャンセル "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbNPAfterPayCancel"
																				Text=" NP後払いキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbEcPayCancel"
																				Text=" ECPay決済キャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbGooddealShippingCancel"
																				Text=" Gooddeal出荷キャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbNewebPayCancel"
																				Text=" 藍新Pay決済キャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbVeriTransCancel"
																				Text=" ベリトランスクレジットキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbPayPayCancel"
																				Text=" PayPay決済キャンセル "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbCreditRakutenCancel"
																				Text=" 楽天カードのクレジットキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbGmoCancel"
																				Text=" GMO掛け払いキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<asp:RadioButton
																				ID="rbTwInvoiceCancel"
																				Text="電子発票キャンセル"
																				Visible="false"
																				Font-Size="8pt"
																				GroupName="twInvoiceCancel"
																				runat="server"
																				Enabled="false" />
																			<asp:RadioButton
																				ID="rbTwInvoiceRefund"
																				Text="電子発票払い戻し"
																				Visible="false"
																				Font-Size="8pt"
																				GroupName="twInvoiceCancel"
																				runat="server"
																				Enabled="false" />
																			<asp:CheckBox
																				ID="cbBokuCancel"
																				Text=" Bokuキャンセル連動 "
																				Visible="false"
																				Font-Size="8pt"
																				runat="server"
																				Enabled="false" />
																			<div id="canselAttentionMessage" visible="false" style="color: red; font-size: 8pt;" runat="server"></div>
																			<span style="display: block">
																				<asp:Button
																					ID="btnUpdateOrderStatus"
																					runat="server"
																					Text="  注文ステータス更新  "
																					Width="120"
																					Enabled="false" />
																				<span class="settlementInProcessing" style="display: none">連携処理中です</span>
																			</span>
																		</td>
																	</tr>
																	<tr id="trOrderStatusError" class="list_alert" runat="server" visible="false">
																		<td colspan="3">
																			<asp:Label
																				ID="lOrderStatusError"
																				CssClass="notice"
																				Style="font-weight: bold; line-height: normal;"
																				runat="server" />
																		</td>
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
																			<asp:RadioButtonList
																				id="rblStorePickupStatus"
																				OnSelectedIndexChanged="DisplayControl"
																				AutoPostBack="true"
																				runat="server"
																				RepeatDirection="Horizontal"
																				RepeatLayout="Flow" />
																		</td>
																		<td class="detail_item_bg" width="120" style="white-space: nowrap;">
																			<span style="display:block">
																				<asp:CheckBox
																					ID="cbSendMailStorePickupStatus"
																					Text="  店舗到着メールを送信する  "
																					runat="server" />
																			</span>
																		</td>
																	</tr>
																	<tr>
																		<td class="detail_title_bg" width="120">更新日指定</td>
																		<td class="detail_item_bg" align="left">&nbsp;
																			<uc:DateTimePickerPeriodInput
																				id="ucUpdateStorePickupStatusDate"
																				CanShowEndDatePicker="False"
																				UpdatePanelControlId="<%#: upStorePickupStatus.ID %>"
																				runat="server"
																				IsHideTime="true" />
																		</td>
																		<td class="detail_item_bg" align="right" width="120" style="white-space: nowrap;">
																			<span style="display:block">
																				<asp:Button
																					ID="btnUpdateStorePickupStatus"
																					Runat="server"
																					Text="  店舗受取ステータス更新  "
																					Width="120"
																					OnClick="btnUpdateStorePickupStatus_Click" />
																			</span>
																		</td>
																	</tr>
																</table>
															</ContentTemplate>
														</asp:UpdatePanel>
													</div>
													<%--△ 店舗受取ステータス更新 △--%>

													<%--▽ 返品交換ステータス ▽--%>
													<asp:UpdatePanel ID="UpOrderReturnExchangeStatus" runat="server">
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
																			<asp:RadioButtonList
																				ID="rblOrderReturnExchangeStatus"
																				OnSelectedIndexChanged="DisplayControl"
																				AutoPostBack="true"
																				runat="server"
																				RepeatDirection="Horizontal"
																				RepeatLayout="Flow"
																				Enabled="false" />
																		</td>
																	</tr>
																	<tr>
																		<td class="detail_title_bg">更新日指定</td>
																		<td class="detail_item_bg" align="right" width="498">&nbsp;
																			<uc:DateTimeInput
																				ID="ucOrderReturnExchangeStatusDate"
																				runat="server"
																				YearList="<%# DateTimeUtility.GetBackwardYearListItem() %>"
																				HasTime="False"
																				HasBlankSign="False"
																				HasBlankValue="True" />
																		</td>
																		<td class="detail_item_bg" align="right" width="140">
																			<asp:Button
																				ID="btnUpdateOrderReturnExchangeStatus"
																				runat="server"
																				Text="  返品交換ステータス更新  "
																				Width="140"
																				Enabled="false" />
																		</td>
																	</tr>
																	<tr id="trOrderReturnExchagneStatus" class="list_alert" runat="server" visible="false">
																		<td colspan="5">
																			<asp:Label
																				ID="lOrderReturnExchangeStatus"
																				CssClass="notice" Style="font-weight: bold; line-height: normal;"
																				runat="server" />
																		</td>
																	</tr>
																</table>
															</div>
														</ContentTemplate>
													</asp:UpdatePanel>
													<%--△ 返品交換ステータス △--%>
													<%--▽ 返金ステータス ▽--%>
													<asp:UpdatePanel ID="UpOrderRepaymentStatus" runat="server">
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
																			<asp:RadioButtonList
																				ID="rblOrderRepaymentStatus"
																				OnSelectedIndexChanged="DisplayControl"
																				AutoPostBack="true"
																				runat="server"
																				RepeatDirection="Horizontal"
																				RepeatLayout="Flow"
																				Enabled="false" />
																		</td>
																	</tr>
																	<tr>
																		<td class="detail_title_bg">更新日指定</td>
																		<td class="detail_item_bg" align="right" width="498">&nbsp;
																			<uc:DateTimeInput
																				ID="ucOrderRepaymentStatusDate"
																				runat="server"
																				YearList="<%# DateTimeUtility.GetBackwardYearListItem() %>"
																				HasTime="False"
																				HasBlankSign="False"
																				HasBlankValue="True" />
																		</td>
																		<td class="detail_item_bg" align="right" width="140">
																			<asp:Button
																				ID="btnUpdateOrderRepaymentStatus"
																				runat="server"
																				Text="  返金ステータス更新  "
																				Width="140"
																				Enabled="false" />
																		</td>
																	</tr>
																	<tr id="trOrderRepaymentStatus" class="list_alert" runat="server" visible="false">
																		<td colspan="5">
																			<asp:Label
																				ID="lOrderRepaymentStatus"
																				CssClass="notice" Style="font-weight: bold; line-height: normal;"
																				runat="server" />
																		</td>
																	</tr>
																</table>
															</div>
														</ContentTemplate>
													</asp:UpdatePanel>
													<%--△ 返金ステータス △--%>
													<div runat="server" id="divCvsDefInvoiceReissue" visible="false">
														<div id="divReissue" runat="server">
															<br />
															<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tr>
																	<td class="detail_title_bg" align="center" colspan="3">後払い請求書再発行</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" width="120">請求書再発行</td>
																	<td class="detail_item_bg" width="518"></td>
																	<td class="detail_item_bg" align="right" width="120">
																		<asp:Button
																			ID="btnCvsDefInvoiceReissue"
																			runat="server"
																			Text="  請求書再発行実行  "
																			Enabled="false" />
																	</td>
																</tr>
															</table>
														</div>
													</div>
													<%--▽ 外部受注情報連携 ▽--%>
													<asp:UpdatePanel ID="UpOrderDelivertyStatus" runat="server">
														<ContentTemplate>
															<div id="divOrderDelivertyStatus" runat="server">
																<br />
																<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
																	<tr>
																		<td class="detail_title_bg" align="center" colspan="3">外部受注情報連携</td>
																	</tr>
																	<tr>
																		<td class="detail_title_bg" width="120">オンライン物流ステータス</td>
																		<td class="detail_item_bg" colspan="2">
																			<%: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ONLINE_DELIVERY_STATUS, this.OrderInput.OnlineDeliveryStatus) %>
																		</td>
																	</tr>
																	<tr>
																		<td class="detail_title_bg" width="120">外部受注情報連携</td>
																		<td class="detail_item_bg" width="518">ECPay<br />
																			※ECPayの受注情報処理</td>
																		<td class="detail_item_bg" align="center" width="120">
																			<asp:Button
																				ID="btnExternalOrderInfoAction"
																				Text="  処理実行  "
																				Width="120"
																				runat="server"
																				Enabled="false" />
																			<span class="settlementInProcessing" style="display: none">連携処理中です</span>
																		</td>
																	</tr>
																	<tr id="trErrorExternalOrderInfo" class="list_alert" runat="server" visible="false">
																		<td colspan="3">
																			<asp:Label
																				ID="lbErrorExternalOrderInfo"
																				CssClass="notice" Style="font-weight: bold; line-height: normal;"
																				runat="server" />
																		</td>
																	</tr>
																</table>
															</div>
														</ContentTemplate>
													</asp:UpdatePanel>
													<%--△ 外部受注情報連携 △--%>

													<%--▽ Amazon連携ステータス ▽--%>
													<% if (this.OrderInput.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON) { %>
														<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tr>
																<td class="detail_title_bg" align="center" colspan="3">モール連携ステータス更新</td>
															</tr>
															<tr>
																<td class="detail_title_bg" width="120">モール連携取込ステータス</td>
																<td class="detail_item_bg" align="left" width="445">
																	<asp:RadioButtonList
																		ID="rblMallLinkStatus"
																		RepeatDirection="Horizontal"
																		RepeatLayout="Flow"
																		runat="server"
																		Enabled="false" />
																</td>
																<td class="detail_item_bg" align="center" width="153">
																	<asp:Button
																		ID="btnUpdateMallLinkStatus"
																		Text=" モール連携取込ステータス更新 "
																		Width="153"
																		runat="server"
																		Enabled="false" />
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
																	<asp:RadioButtonList
																		ID="rblGooddealDeliveryStatus"
																		RepeatDirection="Horizontal"
																		RepeatLayout="Flow"
																		runat="server"
																		Enabled="false" />
																</td>
																<td class="detail_item_bg" align="center" width="153">
																	<asp:Button
																		ID="btnUpdateGooddealDeliveryStatus"
																		Text="  Gooddeal連携取込  "
																		Width="153"
																		runat="server"
																		Enabled="false" />
																</td>
															</tr>
															<tr id="trErrorGooddealDeliveryStatus" class="list_alert" runat="server" visible="false">
																<td colspan="5">
																	<span class="notice" style="font-weight: bold; line-height: normal;">
																		<asp:Label
																			ID="lErrorGooddealDeliveryStatus"
																			CssClass="notice" Style="font-weight: bold; line-height: normal;"
																			runat="server" />
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
																<asp:DropDownList
																	ID="ddlOrderMailId"
																	runat="server"
																	Width="100%" />
															</td>
															<td class="detail_item_bg" align="center" width="153">
																<input
																	type="button"
																	style="width: 153px"
																	value="  メール送信フォームへ  "
																	onclick="javascript:open_order_send_mail();" />
															</td>
														</tr>
													</table>
													<%--△ メール送信 △--%>
													<br />
													<%--▽ 受注情報表示 ▽--%>
													<uc:BodyOrderConfirm
														ID="ucBodyOrderConfirm"
														OrderInput='<%# this.OrderInput %>'
														runat="server"
														IsStorePickUpOrder="true" />
													<%--△ 受注情報表示 △--%>
													<div class="action_part_bottom" runat="server">
														<asp:Button
															ID="btnBackListBottom"
															runat="server"
															Text="  一覧へ戻る  "
															OnClick="btnBackList_Click" />
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
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
	</table>
</asp:Content>
