<%--
=========================================================================================================
  Module      : 注文返品交換登録ページ(OrderReturnExchangeInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderReturnExchangeInput.aspx.cs" Inherits="Form_Order_OrderReturnExchangeInput" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.Domain.Payment" %>
<%@ Import Namespace="w2.App.Common.Global" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<%@ Import Namespace="w2.Domain.Order" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Register Src="~/Form/Common/CreditToken.ascx" TagPrefix="uc" TagName="CreditToken" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
// 選択商品
var selected_product_index = 0;

// 商品一覧画面表示B
function open_product_list(link_file,window_name,window_type,index) {
	// 選択商品を格納
	selected_product_index = index;	
	// ウィンドウ表示
	open_window(link_file,window_name,window_type);
}

	// 商品一覧で選択された商品情報を設定
	function set_productinfo(product_id, supplier_id, variation_id, product_name, display_price, display_special_price, product_price, sale_id, fixed_purchase_id, limitedfixedpurchasekbn1setting, limitedfixedpurchasekbn3setting, tax_rate) {
<%
	// 注文商品数分ループ
	int iLoop = 0;
	foreach (RepeaterItem ri in rExchangeOrderItem.Items)
	{
%>
	if (selected_product_index == <%= iLoop %>) {
		document.getElementById('<%= ((TextBox)ri.FindControl("tbProductId")).ClientID %>').value = product_id;
	document.getElementById('<%= ((HiddenField)ri.FindControl("hfSupplierId")).ClientID %>').value = supplier_id;
	document.getElementById('<%= ((TextBox)ri.FindControl("tbVId")).ClientID %>').value = variation_id;
	document.getElementById('<%= ((TextBox)ri.FindControl("tbProductName")).ClientID %>').value = product_name;
	document.getElementById('<%= ((TextBox)ri.FindControl("tbProductPrice")).ClientID %>').value = product_price;
	document.getElementById('<%= ((HiddenField)ri.FindControl("hfProductTaxRate")).ClientID %>').value = tax_rate;
	document.getElementById('<%= ((TextBox)ri.FindControl("tbItemQuantity")).ClientID %>').value = 1;
		<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
		document.getElementById('<%= ((CheckBox)ri.FindControl("cbFixedPurchase")).ClientID %>').checked = "";
		<% } %>
		<% if (Constants.PRODUCT_SALE_OPTION_ENABLED){ %>
		document.getElementById('<%= ((TextBox)ri.FindControl("tbProductSaleId")).ClientID %>').value = sale_id;
		<% } %>
		if (product_id != '') {
			document.getElementById('<%= btnReCalculate.ClientID %>').click();
		}

	}
<%
		   iLoop++;
	}
%>
}
	//-->
</script>

<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr id="trTitleOrderTop" runat="server">
	</tr>
	<tr id="trTitleOrderMiddle" runat="server">
		<td><h1 class="page-title">受注情報詳細</h1></td>
	</tr>
	<tr id="trTitleOrderBottom" runat="server">
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><h2 class="cmn-hed-h2">受注情報返品交換登録</h2></td>
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
												<div class="action_part_top">
													<asp:Button ID="btnBackDetailTop" runat="server" Text="  詳細へ戻る  " onclick="btnBackDetail_Click" UseSubmitBehavior="False" />
													<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" UseSubmitBehavior="False" OnClientClick="doPostbackEvenIfCardAuthFailed=false;" />
													<div style="padding:5px 0px 0px 5px" runat="server" visible="<%# Constants.PAYMENT_REAUTH_ENABLED %>">
														<asp:Label id="lbExternalPayment" runat="server" CssClass="external_payment" />
														<asp:DropDownList ID="ddlExecuteType" runat="server" Width="150" />
													</div>
													<asp:Label ID="lbPaygentErrorMessages" runat="server" ForeColor="red" visible="false" />
												</div>
												<%--▽ 基本情報 ▽--%>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<p><asp:Label ID="lbReturnHalfAlertMessage" runat="server" ForeColor="red" /></p>
													<tbody id="tbdyErrorMessages" runat="server" visible="false">
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="2">
																<asp:Label ID="lbErrorMessages" runat="server" ForeColor="red"></asp:Label>
															</td>
														</tr>
													</tbody>
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">元注文ID</td>
															<td class="edit_item_bg" align="left">
																<asp:Literal ID="lOrderIdOrg" runat="server"></asp:Literal>
																<asp:HiddenField ID="hfCvsShopId" runat="server" /></td>
														</tr>
														<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">頒布会コースID</td>
															<td class="edit_item_bg" align="left">
																<asp:Literal ID="lSubscriptionBoxCourseId" runat="server"></asp:Literal></td>
														</tr>
														<% } %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">返品交換区分</td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButtonList ID="rblReturnExchangeKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="True" OnSelectedIndexChanged="rblReturnExchangeKbn_OnSelectedIndexChanged"></asp:RadioButtonList>
																<% if ((this.ReturnExchangeOrderOrg.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.ReturnExchangeOrderOrg.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)) { %>
																	<span style="padding-left:10px; color:red;"><%= WebMessages.GetMessages(WebMessages.ERRMSG_GMO_KB_PAYMENT_EXCHANGE) %></span>
																<% } %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">返品交換受付日</td>
															<td class="edit_item_bg" align="left">
																<uc:DateTimeInput ID="ucOrderReturnExchangeReceiptDate" runat="server" YearList="<%# DateTimeUtility.GetBackwardYearListItem() %>" HasTime="False" HasBlankSign="True" HasBlankValue="True" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">返品交換理由</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbReturnExchangeReasonMemo" runat="server" Width="500" Rows="4" TextMode="MultiLine"></asp:TextBox>
																<div>
																<asp:RadioButtonList ID="rblReturnExchangeReasonKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio_button_list"></asp:RadioButtonList>
																</div>
															</td>
														</tr>
														<tr id="trOrderPaymentKbn" runat="server">
															<td class="edit_title_bg" align="left" width="25%">決済種別<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList ID="ddlOrderPaymentKbn" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlOrderPaymentKbn_OnSelectedIndexChanged"></asp:DropDownList>
																<p><asp:Label ID="lbOrderPaymentInfo" runat="server" /></p>
																<%--▼▼ クレジット Token保持用 ▼▼--%>
																<asp:HiddenField ID="hfCreditToken" runat="server" />
																<asp:HiddenField ID="hfLastFourDigit" runat="server" />
																<%--▲▲ クレジット Token保持用 ▲▲--%>
																<%-- 支払い制限アラート --%>
																<p><asp:Label ID="lbPaymentLimitedMessage" runat="server" ForeColor="red" /></p>
																<p><asp:Label ID="lbPaymentAlertMessage" runat="server" ForeColor="red" /></p>
																<p><asp:Label ID="lbPaymentUserManagementLevelMessage" runat="server" ForeColor="red" /></p>
																<p><asp:Label ID="lbPaymentOrderOwnerKbnMessage" runat="server" ForeColor="red" /></p>
															</td>
														</tr>
														<%-- ▽クレジット決済の場合は表示▽ --%>
														<tbody id="tbdyPaymentKbnCredit" runat="server">
														<tr>
															<td class="edit_title_bg">利用クレジットカード</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList ID="ddlUserCreditCard" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlUserCreditCard_SelectedIndexChanged"></asp:DropDownList><br />
																<%--▼▼ カード情報取得用 ▼▼--%>
																<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%= CreateGetCardInfoJsScriptForCreditToken() %>" />
																<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
																<span id="spanErrorMessageForPayTg" style="color: red; display: none" runat="server"></span>
																<%--▲▲ カード情報取得用 ▲▲--%>
																<%--▼▼ PayTg 端末状態保持用 ▼▼--%>
																<asp:HiddenField ID="hfCanUseDevice" runat="server" />
																<asp:HiddenField ID="hfStateMessage" runat="server" />
																<%--▲▲ PayTg 端末状態保持用 ▲▲--%>
															</td>
														</tr>
														<div id="divUserCreditCard" runat="server">
															<%if (OrderCommon.CreditCompanySelectable) {%>
															<tr class="creditCardItem">
																<td align="left" class="edit_title_bg">カード会社</td>
																<td class="edit_item_bg"><asp:Literal ID="lCreditCompany" runat="server"></asp:Literal></td>
															</tr>
															<%} %>
															<tr class="creditCardItem">
																<td align="left" class="edit_title_bg">カード番号</td>
																<td class="edit_item_bg">************<asp:Literal ID="lCreditLastFourDigit" runat="server"></asp:Literal></td>
															</tr>
															<tr class="creditCardItem">
																<td align="left" class="edit_title_bg">有効期限</td>
																<td class="edit_item_bg"><asp:Literal ID="lCreditExpirationMonth" runat="server"></asp:Literal>/<asp:Literal ID="lCreditExpirationYear" runat="server"></asp:Literal>(月/年)</td>
															</tr>
															<tr class="creditCardItem">
																<td align="left" class="edit_title_bg">カード名義人</td>
																<td class="edit_item_bg"><asp:Literal ID="lCreditAuthorName" runat="server"></asp:Literal></td>
															</tr>
														</div>
														<div id="divCreditCardInputNew" runat="server">
														<%--▼▼▼ カード情報入力フォーム表示 ▼▼▼--%>
														<%if (this.CanUseCreditCardNoForm) {%>
														<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
														<div id="divCreditCardNoToken" runat="server">
														<%if (OrderCommon.CreditCompanySelectable) { %>
														<tr class="creditCardItem NEW">
															<td class="edit_title_bg" align="left">カード会社<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlCreditCardCompany" runat="server"></asp:DropDownList></td>
														</tr>
														<%} %>
														<asp:PlaceHolder ID="phCreditCardNotRakuten" runat="server">
														<tr class="creditCardItem NEW">
															<td class="edit_title_bg" align="left">
																<%if (this.CreditTokenizedPanUse) {%>永久トークン<%}else{%>カード番号<%} %><span class="notice">*</span>
															</td>
															<td id="tdCreditNumber" class="edit_item_bg" align="left" runat="server">
																<asp:TextBox id="tbCreditCardNo1" pattern="[0-9]*" Width="160" MaxLength="16" autocomplete="off" runat="server"></asp:TextBox>
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
														</div>
														<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

														<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
														<div id="divCreditCardForTokenAcquired" runat="server">
														<%if (OrderCommon.CreditCompanySelectable) {%>
														<tr class="creditCardItem NEW">
															<td class="edit_title_bg" align="left">カード会社</td>
															<td class="edit_item_bg" align="left"><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" runat="server"></asp:Literal></td>
														</tr>
														<%} %>
														<tr class="creditCardItem NEW">
															<td class="edit_title_bg" align="left">カード番号</td>
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
														<%--▲▲▲ カード情報入力フォーム表示 ▲▲▲--%>
														<%} else {%>
														<%--▼▼▼ カード情報入力フォーム非表示 ▼▼▼--%>
														<tr class="creditCardItem NEW">
															<td class="edit_title_bg" align="left"></td>
															<td class="edit_item_bg" align="left">
																<span class="notice">
																クレジットカード番号は入力できません。<br />
																<%if (this.NeedsRegisterProvisionalCreditCardCardKbnZeus) {%>
																	クレジットカード番号の入力は決済用タブレットにて行ってください。
																<%} %>
																<%if (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus) {%>
																	登録すると「<%: new PaymentService().Get(this.LoginOperatorShopId, Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID).PaymentName %>」として登録されます。
																<%} %>
																</span>
															</td>
														</tr>
														<%} %>
														<%--▲▲▲ カード情報入力フォーム非表示 ▲▲▲--%>
														</div>

														<%--▼▼▼ カード情報入力フォーム表示 ▼▼▼--%>
														<%-- ▽分割支払い有効の場合は表示▽ --%>
														<tr id="trInstallments" runat="server">
															<td class="edit_title_bg" align="left">支払回数<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:DropDownList id="dllCreditInstallments" runat="server" AutoPostBack="true"></asp:DropDownList>&nbsp;&nbsp;※AMEX/DINERSは一括のみとなります。</td>
														</tr>
														<%-- △分割支払い有効の場合は表示△ --%>
														<%if (this.CanUseCreditCardNoForm) {%>
														<tr class="creditCardItem NEW" style=" height:27px;" id="trRegistCreditCard" runat="server">
																<td align="left" class="edit_title_bg" style="width:129px;">登録する</td>
																<td class="edit_item_bg"><asp:CheckBox ID="cbRegistCreditCard" runat="server" Text="  登録する" AutoPostBack="True" OnCheckedChanged="cbRegistCreditCard_CheckedChanged"></asp:CheckBox></td>
														</tr>
														<tr class="creditCardItem NEW" style=" height:27px;" id="trCreditCardName" runat="server">
																<td align="left" class="edit_title_bg" style="width:129px;">クレジットカード登録名 <span class="notice">*</span></td>
																<td class="edit_item_bg"><asp:TextBox ID="tbUserCreditCardName" runat="server" MaxLength="30"></asp:TextBox>&nbsp;&nbsp;※クレジットカードを保存する場合は、ご入力ください。</td>
														</tr>
														<%} %>
														<%--▲▲▲ カード情報入力フォーム表示 ▲▲▲--%>
														</tbody>
														<%-- △クレジット決済の場合は表示△ --%>
													</tbody>
												</table>
												<%--△ 基本情報 △--%>
												<%--▽ 返品商品情報 ▽--%>
												<br />
												<%--▽ 返品商品情報 - エラーメッセージ ▽--%>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody id="tbdyReturnOrderItemErrorMessages" runat="server" visible="false">
														<tr>
															<td class="edit_title_bg" align="center" colspan="7">エラーメッセージ</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="7">
																<asp:Label ID="lbReturnOrderItemErrorMessages" runat="server" ForeColor="red"></asp:Label>
															</td>
														</tr>
													</tbody>
												</table>
												<%--△ 返品商品情報 - エラーメッセージ△--%>
												<%--▽▽▽▽▽ ReturnOrderItem で繰り返し ▽▽▽▽▽--%>
												<asp:Repeater ID="rReturnOrderItem" runat="server" ItemType="w2.App.Common.Order.ReturnOrderItem" >
												<ItemTemplate>
												<br />
												<% if ((Constants.GIFTORDER_OPTION_ENABLED) && (this.ReturnExchangeOrderOrg.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)) { %>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="4">【　配送情報<%#: Item.OrderShippingNo %>　】</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="center" colspan="4">配送先情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name.name@@") %></td>
														<td class="detail_item_bg" align="left" width="25%">
															<%#: Item.ShippingName %></td>
														<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
														<td class="detail_item_bg" align="left">
															<%#: Item.ShippingNameKana %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="25%">住所</td>
														<td class="detail_item_bg" align="left" colspan="3">
															<span visible='<%# (string.IsNullOrEmpty(Item.ShippingZip) == false) %>' runat="server">
															<%#: IsCountryJp(Item.ShippingCountryIsoCode)
																? "〒" + Item.ShippingZip
																: ""%>
															</span>
															<%#: Item.ShippingAddr1 %>
															<%#: Item.ShippingAddr2 %>
															<%#: Item.ShippingAddr3 %>
															<%#: Item.ShippingAddr4 %>
															<%#: Item.ShippingAddr5 %>
															<span id="Span1" visible='<%# (string.IsNullOrEmpty(Item.ShippingZip) == false) %>' runat="server">
															<%#:  IsCountryJp(Item.ShippingCountryIsoCode)
																? Item.ShippingZip
																: ""%>
															</span>
															<%#: Item.ShippingCountryName %>
														</td>
													</tr>
													<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
													<tr>
														<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.company_name.name@@")%>・<%: ReplaceTag("@@User.company_post_name.name@@")%></td>
														<td class="detail_item_bg" align="left" colspan="3">
															<%#: Item.ShippingCompanyName %>&nbsp<%#: Item.ShippingCompanyPostName%>
													</tr>
													<%} %>
													<tr>
														<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.tel1.name@@") %></td>
														<td class="detail_item_bg" align="left" colspan="3">
															<%#: Item.ShippingTel1 %></td>
													</tr>
												</table>
												<% } %>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<p><asp:Label ID="lbReturnHalfAlertMessage" runat="server" ForeColor="red" /></p>
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="<%: 7 + this.AddColumnCountForItemTable %>">返品商品選択</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="center" width="8%" rowspan="2">
																<asp:CheckBox ID="cbReturnProductAll" runat="server" AutoPostBack="true" Visible="<%# (this.OrderItems.Count(item => (item.ReturnStatus != Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE)) > 0) %>" OnCheckedChanged="cbReturnProductAll_CheckedChanged" />
															</td>
															<%--▽ セールOPが有効の場合 ▽--%>
															<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
															<td class="edit_title_bg" align="center" width="8%" rowspan="2">セールID</td>
															<%} %>
															<%--△ セールOPが有効の場合 △--%>
															<%--▽ ノベルティOPが有効の場合 ▽--%>
															<% if (Constants.NOVELTY_OPTION_ENABLED) { %>
															<td class="edit_title_bg" align="center" width="10%" rowspan="<%= Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>">ノベルティID</td>
															<%} %>
															<%--△ ノベルティOPが有効の場合 △--%>
															<%--▽ レコメンド設定OPが有効の場合 ▽--%>
															<% if (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) { %>
															<td class="edit_title_bg" align="center" width="10%" rowspan="2">
																レコメンドID
															</td>
															<%} %>
															<%--△ レコメンド設定OPが有効の場合 △--%>
															<%--▽ 商品同梱設定OPが有効の場合 ▽--%>
															<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED) { %>
															<td class="edit_title_bg" align="center" width="10%" rowspan="2">
																商品同梱ID
															</td>
															<%} %>
															<%--△ 商品同梱設定OPが有効の場合 △--%>
															<td class="edit_title_bg" align="center" width="8%" rowspan="2" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" runat="server">
																頒布会コースID
															</td>
															<td class="edit_title_bg" align="center" width="14%">商品ID</td>
															<td class="edit_title_bg" align="center" width="<%= 20 + (Constants.PRODUCT_SALE_OPTION_ENABLED ? 0 :8) + ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 0 :10) + (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 0 : 10) + (this.DisplayItemSubscriptionBoxCourseIdArea ? 0 : 8) %>%" rowspan="2">
																商品名
															</td>
															<td class="edit_title_bg" align="center" width="6%" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																単価（<%: this.ProductPriceTextPrefix %>）
															</td>
															<td class="edit_title_bg" align="center" width="4%" rowspan="2">数量</td>
															<td class="edit_title_bg" align="center" width="3%" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																消費税率
															</td>
															<td class="edit_title_bg" align="center" width="8%" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																小計（<%: this.ProductPriceTextPrefix %>）
															</td>
														</tr>
														<tr>
															<%--▽ レコメンド設定OPが有効の場合 ▽--%>
															<% if (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) { %>
															<td class="edit_title_bg" align="center">
																レコメンドID
															</td>
															<%} %>
															<td class="edit_title_bg" align="center">バリエーションID</td>
														</tr>
														<asp:Repeater ID="rReturnOrderItem2" runat="server" DataSource="<%# this.OrderItems.Where(orderItem => (orderItem.OrderShippingNo == Item.OrderShippingNo)) %>" ItemType="w2.App.Common.Order.ReturnOrderItem">
														<ItemTemplate>
															<tr id="trItem" runat="server" class="mobile_item_bg">
																<td align="center" rowspan="2">
																	<%--▽ 返品済み商品の場合、チェックボックスは非表示にする ▽--%>
																	<%#: (Item.ReturnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE) ? "返品済み" : "" %>
																	<asp:CheckBox ID="cbReturnProduct" runat="server" Visible="<%# (Item.ReturnStatus != Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE) %>" AutoPostBack="true" OnCheckedChanged="cbReturnProduct_CheckedChanged" />
																	<%--△ 返品済み商品の場合、チェックボックスは非表示にする △--%>
																	<%--▽ 隠しタグ ▽--%>
																	<asp:HiddenField ID="hfReturnStatus" runat="server" Value="<%# Item.ReturnStatus %>" />
																	<asp:HiddenField ID="hfProductId" runat="server" Value="<%# Item.ProductId %>" />
																	<asp:HiddenField ID="hfVId" runat="server" Value="<%# Item.VId %>" />
																	<asp:HiddenField ID="hfSupplierId" runat="server" Value="<%# Item.SupplierId %>" />
																	<asp:HiddenField ID="hfProductName" runat="server" Value="<%# Item.ProductName %>" />
																	<asp:HiddenField ID="hfProductNameKana" runat="server" Value="<%# Item.ProductNameKana %>" />
																	<asp:HiddenField ID="hfProductPrice" runat="server" Value="<%# Item.ProductPrice %>" />
																	<asp:HiddenField ID="hfItemQuantity" runat="server" Value="<%# Item.ItemQuantity %>" />
																	<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# Item.ProductSaleId %>" />
																	<asp:HiddenField ID="hfNoveltyId" runat="server" Value="<%# Item.NoveltyId %>" />
																	<asp:HiddenField ID="hfRecommendId" runat="server" Value="<%# Item.RecommendId %>" />
																	<asp:HiddenField ID="hfProductBundleId" runat="server" Value="<%# Item.ProductBundleId %>" />
																	<asp:HiddenField ID="hfBundleItemDisplayType" runat="server" Value="<%# Item.BundleItemDisplayType %>" />
																	<asp:HiddenField ID="hfBrandId" runat="server" Value="<%# Item.BrandId %>" />
																	<asp:HiddenField ID="hfDownloadUrl" runat="server" Value="<%# Item.DownloadUrl %>" />
																	<asp:HiddenField ID="hfCooperationId1" runat="server" Value="<%# Item.CooperationId1 %>" />
																	<asp:HiddenField ID="hfCooperationId2" runat="server" Value="<%# Item.CooperationId2 %>" />
																	<asp:HiddenField ID="hfCooperationId3" runat="server" Value="<%# Item.CooperationId3 %>" />
																	<asp:HiddenField ID="hfCooperationId4" runat="server" Value="<%# Item.CooperationId4 %>" />
																	<asp:HiddenField ID="hfCooperationId5" runat="server" Value="<%# Item.CooperationId5 %>" />
																	<asp:HiddenField ID="hfCooperationId6" runat="server" Value="<%# Item.CooperationId6 %>" />
																	<asp:HiddenField ID="hfCooperationId7" runat="server" Value="<%# Item.CooperationId7 %>" />
																	<asp:HiddenField ID="hfCooperationId8" runat="server" Value="<%# Item.CooperationId8 %>" />
																	<asp:HiddenField ID="hfCooperationId9" runat="server" Value="<%# Item.CooperationId9 %>" />
																	<asp:HiddenField ID="hfCooperationId10" runat="server" Value="<%# Item.CooperationId10 %>" />
																	<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# Item.ProductOptionValue %>' />
																	<asp:HiddenField ID="hfOrderShippingNo" runat="server" Value='<%# Item.OrderShippingNo %>' />
																	<asp:HiddenField ID="hfShippingName" runat="server" Value='<%# Item.ShippingName %>' />
																	<asp:HiddenField ID="hfShippingNameKana" runat="server" Value='<%# Item.ShippingNameKana %>' />
																	<asp:HiddenField ID="hfShippingTel1" runat="server" Value='<%# Item.ShippingTel1 %>' />
																	<asp:HiddenField ID="hfShippingCountryIsoCode" runat="server" Value='<%# Item.ShippingCountryIsoCode %>' />
																	<asp:HiddenField ID="hfShippingCountryName" runat="server" Value='<%# Item.ShippingCountryName %>' />
																	<asp:HiddenField ID="hfShippingZip" runat="server" Value='<%# Item.ShippingZip %>' />
																	<asp:HiddenField ID="hfShippingAddr1" runat="server" Value='<%# Item.ShippingAddr1 %>' />
																	<asp:HiddenField ID="hfShippingAddr2" runat="server" Value='<%# Item.ShippingAddr2 %>' />
																	<asp:HiddenField ID="hfShippingAddr3" runat="server" Value='<%# Item.ShippingAddr3 %>' />
																	<asp:HiddenField ID="hfShippingAddr4" runat="server" Value='<%# Item.ShippingAddr4 %>' />
																	<asp:HiddenField ID="hfShippingAddr5" runat="server" Value='<%# Item.ShippingAddr5 %>' />
																	<asp:HiddenField ID="hfShippingCompanyName" runat="server" Value='<%# Item.ShippingCompanyName %>' />
																	<asp:HiddenField ID="hfShippingCompanyPostName" runat="server" Value='<%# Item.ShippingCompanyPostName %>' />
																	<asp:HiddenField ID="hfProductPricePretax" runat="server" Value='<%# Item.ProductPricePretax %>' />
																	<asp:HiddenField ID="hfProductTaxIncludedFlg" runat="server" Value='<%# Item.ProductTaxIncludedFlg %>' />
																	<asp:HiddenField ID="hfProductTaxRate" runat="server" Value='<%# Item.ProductTaxRate %>' />
																	<asp:HiddenField ID="hfProductTaxRoundType" runat="server" Value='<%# Item.ProductTaxRoundType %>' />
																	<asp:HiddenField ID="hfOrderSetPromotionNo" runat="server" Value='<%# Item.OrderSetPromotionNo %>' />
																	<asp:HiddenField ID="hfFixedPurchaseProductFlg" runat="server" Value='<%# Item.FixedPurchaseProductFlg %>' />
																	<asp:HiddenField ID="hfDiscountedPrice" runat="server" Value='<%# Item.DiscountedPrice %>' />
																	<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value="<%# Item.SubscriptionBoxCourseId %>" />
																	<asp:HiddenField ID="hfSubscriptionBoxFixedAmount" runat="server" Value="<%# Item.SubscriptionBoxFixedAmount %>" />
																	<%--△ 隠しタグ △--%>
																</td>
																<td align="center" rowspan="2" runat="server" visible='<%# Constants.PRODUCT_SALE_OPTION_ENABLED %>'><%#: (Item.ProductSaleId != "") ? Item.ProductSaleId : "-" %></td>
																<td align="center" rowspan="<%# Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>" runat="server" visible='<%# Constants.NOVELTY_OPTION_ENABLED %>'>
																	<%#: (Item.NoveltyId != "") ? Item.NoveltyId : "-" %>
																</td>
																<td align="center" runat="server" rowspan="2" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) %>">
																	<%#: (Item.RecommendId != "") ? Item.RecommendId : "-" %>
																</td>
																<td align="center" runat="server" rowspan="2" visible="<%# (Constants.PRODUCTBUNDLE_OPTION_ENABLED) %>">
																	<%# OrderPage.GetProductBundleIdDisplayValue(Item.ProductBundleId, this.LoginShopOperator) %>
																</td>
																<td align="center" rowspan="2" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" runat="server">
																	<div Visible="<%# Item.IsSubscriptionBox %>" runat="server">
																		<a href="javascript:open_window('<%#: FixedPurchasePage.CreateSubscriptionBoxRegisterUrl(Item.SubscriptionBoxCourseId) %>','fixedpurchase','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																			<%#: Item.SubscriptionBoxCourseId %>
																		</a>
																	</div>
																	<div Visible="<%# Item.IsSubscriptionBox == false %>" runat="server">-</div>
																</td>
																<td align="center"><%#: Item.ProductId %></td>
																<td align="left" rowspan="2">
																	<%#: Item.ProductName %><br />
																	<span visible='<%# Item.OrderSetPromotionNo != "" %>' runat="server">
																		[<%#: Item.OrderSetPromotionNo %> ： <%#: this.ReturnExchangeOrderOrg.GetOrderSetPromotionName(Item.OrderSetPromotionNo) %>]
																	</span>
																</td>
																<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																	<%#: Item.IsSubscriptionBoxFixedAmount == false ? Item.ProductPrice.ToPriceString(true) : "-" %>
																</td>
																<td align="center" rowspan="2">
																	<span id="spanProductPrice" runat="server">
																		<span id="spanMinusProductPrice" runat="server" /><%#: StringUtility.ToNumeric(Item.ItemQuantity) %>
																	</span>
																</td>
																<td align="center" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																	<span id="spanProductTaxRate" Visible="<%# Item.IsSubscriptionBoxFixedAmount == false %>" runat="server">
																		<span id="spanMinusProductTaxRate" runat="server" /><%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate) %>%
																	</span>
																	<div Visible="<%# Item.IsSubscriptionBoxFixedAmount %>" runat="server">-</div>
																</td>
																<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																	<span id="spanItemQuantity" Visible="<%# Item.IsSubscriptionBoxFixedAmount == false %>" runat="server">
																		<span id="spanMinusItemQuantity" runat="server"></span><%#: Item.OptionIncludedProductPrice.ToPriceString(true) %>
																	</span>
																	<span id="spanFixedAmount" Visible="<%# Item.IsSubscriptionBoxFixedAmount %>" runat="server">
																		定額<br />
																		(<span id="spanFixedAmountMinus" runat="server"></span><%#: Item.SubscriptionBoxFixedAmount.ToPriceString(true) %>)
																	</span>
																</td>
															</tr>
															<tr id="trItem1" runat="server" class="mobile_item_bg">
																<td align="center" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) %>">
																	<%#: (Item.RecommendId != "") ? Item.RecommendId : "-" %>
																</td>
																<td align="center"><%# (Item.VId != "") ? "商品ID + " + Item.VId : "-" %></td>
															</tr>
															<tr visible='<%# (Item.ProductOptionValue != "") %>' runat="server">
																<td align="center" class="edit_title_bg">付帯情報</td>
																<td id="tdItem1" align="left" class="mobile_item_bg" colspan="<%# 6 + this.AddColumnCountForItemTable %>">
																	<%#: ProductOptionSettingHelper.GetDisplayProductOptionTexts(Item.ProductOptionValue) %>
																</td>
															</tr>
														</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>
												</ItemTemplate></asp:Repeater>
												<%--△△△△△ ReturnOrderItem で繰り返し △△△△△--%>
												<%--▽ 返品商品情報 - 返品商品合計 ▽--%>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="right">返品商品合計（<%: this.ProductPriceTextPrefix %>）</td>
															<td class="edit_item_bg" align="right" width="8%"><asp:Label ID="lbReturnOrderPriceSubTotal" runat="server"></asp:Label></td>
														</tr>														
													</tbody>
												</table>
												<%--△ 返品商品情報 - 返品商品合計 △--%>
												<%--△ 返品商品情報 △--%>
												<br />
												<table width="758" border="0" cellspacing="0" cellpadding="0">
													<tbody>
														<tr align="right" valign="top">
															<%-- ポイントオプションが有効の場合--%>
															<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
															<td id="tdOrderPoint" width="350" align ="left" runat="server">
																<%--▽ 付与ポイント情報 ▽--%>
																<div id="divOrderPointAdd" runat="server">
																<table class="edit_table" cellspacing="1" cellpadding="3" width="400" border="0">
																	<tbody id="tbdyReturnOrderAddPointErrorMessages" runat="server" visible="false"  align ="left">
																		<tr>
																			<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
																		</tr>
																		<tr>
																			<td class="edit_item_bg" align="left" colspan="4">
																				<asp:Label ID="lbReturnOrderAddPointErrorMessages" runat="server" ForeColor="red"></asp:Label>
																			</td>
																		</tr>
																	</tbody>
																	<tbody align ="left">
																		<tr style="height: 21px">
																			<td class="edit_title_bg" align="center" colspan="2">
																				付与ポイント情報
																				(<a href="#" onclick="javascript:open_window('<%= WebSanitizer.HtmlEncode(w2.App.Common.Manager.SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Mp, UserPage.CreateUserPointHistoryUrl(this.ReturnExchangeOrderOrg.UserId, true))) %>','userpointhistory','width=1028,height=600,top=110,left=380,status=NO,scrollbars=yes');">履歴</a>)
																			</td>
																			<td class="edit_title_bg" align="center" colspan="2">
																				<small><asp:CheckBox ID="cbOrderPointAddReCalculate" runat="server" CssClass="checkBox" AutoPostBack="true" OnCheckedChanged="cbOrderPointAddReCalculate_CheckedChanged" Text="付与ポイント自動計算" /></small>
																			</td>
																		</tr>
																		<tr style="height: 21px">
																			<td class="edit_title_bg" align="center" colspan="2">付与済み<span class="<%= trOrderBasePointAddComp.Visible ? "notice" : "" %>"><%= trOrderBasePointAddComp.Visible ? "本ポイント" : "仮ポイント" %></span></td>
																			<td class="edit_title_bg" align="center" colspan="1">調整ポイント</td>
																			<td class="edit_title_bg" align="center" colspan="1">調整後ポイント</td>
																		</tr>
																		<%-- ユーザポイント(仮ポイント)が存在する --%>
																		<asp:Repeater ID="rOrderPointAddTemp" runat="server" ItemType="w2.Domain.Point.UserPointModel">
																			<ItemTemplate>
																			<tr>
																				<td class="detail_title_bg" align="left" width="130">
																					<%#: (Item.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE)
																						? "通常 付与ポイント"
																						: "期間限定 付与ポイント"%>
																					(<%#: ValueText.GetValueText(
																						Constants.TABLE_USERPOINT,
																						Constants.FIELD_USERPOINT_POINT_INC_KBN, 
																						Item.PointIncKbn) %>)
																				</td>
																				<td class="detail_item_bg" align="left" width="50"><%#: StringUtility.ToNumeric(Item.Point) %> pt</td>
																				<td class="detail_item_bg" align="left" width="50"><asp:Literal ID="lOrderPointAddAdjustment" runat="server" Text="0" /> pt</td>
																				<td class="detail_item_bg" align="left" width="70"><asp:TextBox ID="tbOrderPointAdd" runat="server" Width="50" Text="<%#: Item.Point %>"></asp:TextBox> pt</td>
																				<asp:HiddenField id="hfPointIncKbn" Value="<%# Item.PointIncKbn %>" runat="server"/>
																				<asp:HiddenField id="hfPointKbnNo" Value="<%# Item.PointKbnNo %>" runat="server"/>
																				<asp:HiddenField id="hfOrderPointAdd" Value="<%# Item.Point %>" runat="server"/>
																				<asp:HiddenField id="hfPointKbn" Value="<%# Item.PointKbn %>" runat="server"/>
																			</tr>
																			</ItemTemplate>
																		</asp:Repeater>
																		<%-- 本ポイントが存在する場合 --%>
																		<tr id="trOrderBasePointAddComp" runat="server">
																			<td class="detail_title_bg" align="left" width="130">通常 付与ポイント</td>
																			<td class="detail_item_bg" align="left" width="50">
																				<asp:Literal ID="lOrderBasePointAdd" runat="server"></asp:Literal> pt
																				<asp:HiddenField id="hfOrderBasePointAdd" runat="server"/>
																			</td>
																			<td class="detail_item_bg" align="left" width="50"><asp:Literal ID="lOrderBasePointAddAdjustment" runat="server" Text="0"/> pt</td>
																			<td class="detail_item_bg" align="left" width="70"><asp:TextBox ID="tbOrderBasePointAdd" runat="server" Width="50"></asp:TextBox> pt</td>
																		</tr>
																		<tr id="trOrderLimitPointAddComp" runat="server">
																			<td class="detail_title_bg" align="left" width="130">期間限定 付与ポイント</td>
																			<td class="detail_item_bg" align="left" width="50">
																				<asp:Literal ID="lOrderLimitPointAdd" runat="server"></asp:Literal> pt
																				<asp:HiddenField id="hfOrderLimitPointAdd" runat="server"/>
																			</td>
																			<td class="detail_item_bg" align="left" width="50"><asp:Literal ID="lOrderLimitPointAddAdjustment" runat="server" Text="0"/> pt</td>
																		<td class="detail_item_bg" align="left" width="70"><asp:TextBox ID="tbOrderLimitPointAdd" runat="server" Width="50"></asp:TextBox> pt</td>
																	</tr>
																	</tbody>
																</table>
																<br/>
																</div>
																<%--△ 付与ポイント情報 △--%>
																<%--▽ 利用ポイント情報 ▽--%>
																<div id="divOrderPointUse" runat="server">
																<table class="edit_table" cellspacing="1" cellpadding="3" width="400" border="0">
																	<tbody id="tbdyReturnOrderUsePointErrorMessages" runat="server" visible="false"  align ="left">
																		<tr>
																			<td class="edit_title_bg" align="center" colspan="3">エラーメッセージ</td>
																		</tr>
																		<tr>
																			<td class="edit_item_bg" align="left" colspan="3">
																				<asp:Label ID="lbReturnOrderUsePointErrorMessages" runat="server" ForeColor="red"></asp:Label>
																			</td>
																		</tr>
																	</tbody>
																	<tbody align ="left">
																		<tr style="height: 21px">
																			<td class="edit_title_bg" align="center" colspan="3">
																				利用ポイント情報
																				(<a href="#" onclick="javascript:open_window('<%= WebSanitizer.HtmlEncode(w2.App.Common.Manager.SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Mp, UserPage.CreateUserPointHistoryUrl(this.ReturnExchangeOrderOrg.UserId, true))) %>','userpointhistory','width=1028,height=600,top=110,left=380,status=NO,scrollbars=yes');">履歴</a>)
																			</td>
																		</tr>
																		<tr style="height: 21px">
																			<td class="edit_title_bg" align="center">変更前最終利用ポイント</td>
																			<td class="edit_title_bg" align="center">調整ポイント</td>
																			<td class="edit_title_bg" align="center">調整後最終利用ポイント</td>
																		</tr>
																		<tr style="height: 21px">
																			<td class="detail_item_bg" align="left" width="90">
																				<asp:Literal ID="lLastOrderPointUseBefore" runat="server"></asp:Literal> pt
																				<asp:HiddenField id="hfLastOrderPointUse" runat="server"/>
																			</td>
																			<td class="detail_item_bg" align="left" width="90"><asp:Literal ID="lOrderPointUseAdjustment" runat="server" Text="0"/> pt</td>
																			<td class="detail_item_bg" align="left" width="120"><asp:TextBox ID="tbLastOrderPointUse" runat="server" Width="50"></asp:TextBox> pt</td>
																		</tr>
																	</tbody>
																</table>
																</div>
																<%--△ 利用ポイント情報 △--%>
															</td>
															<%} %>
															<%--▽ 元注文合計情報 ▽--%>
															<td width="350" align ="right">
																<table class="edit_table" cellspacing="1" cellpadding="3" width="350" border="0">
																	<tbody>
																		<tr style="height: 21px">
																			<td class="edit_title_bg" align="center" colspan="2">元注文合計情報</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="right" width="263">商品合計（<%: this.ProductPriceTextPrefix %>）</td>
																			<td class="edit_item_bg" align="right"><asp:Literal ID="lOrderPriceSubTotal" runat="server"></asp:Literal></td>																	
																		</tr>
																		<%if (this.ProductIncludedTaxFlg == false) { %>
																			<tr>
																				<td class="edit_title_bg" align="right">消費税額</td>
																				<td class="edit_item_bg" align="right"><asp:Literal ID="lOrderPriceTax" runat="server"></asp:Literal></td>
																			</tr>
																		<%} %>
																		<asp:Repeater Visible="<%# this.ReturnExchangeOrderOrg.IsAllItemsSubscriptionBoxFixedAmount == false %>" ID="rSetPromotionProductDiscount" runat="server" ItemType="w2.App.Common.Order.OrderSetPromotion">
																		<ItemTemplate>
																			<tr visible="<%# Item.IsDiscountTypeProductDiscount %>" runat="server">
																				<td class="edit_title_bg" align="right">
																					<%#: Item.OrderSetPromotionNo %>：
																					<%#: Item.SetPromotionName %>割引額<br />
																					(ID:<%#: Item.SetPromotionId %>, 商品割引分)
																				</td>
																				<td class="edit_item_bg" align="right">
																					<span class='<%# Item.ProductDiscountAmount > 0 ? "notice" : "" %>'>
																						<%# Item.ProductDiscountAmount > 0 ? "-" : "" %><%#: Item.ProductDiscountAmount.ToPriceString(true) %>
																					</span>
																				</td>
																			</tr>
																		</ItemTemplate>
																		</asp:Repeater>
																		<%-- 会員ランクオプションが有効の場合--%>
																		<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
																		<tr>
																			<td class="edit_title_bg" align="right">会員ランク割引額</td>
																			<td class="edit_item_bg" align="right"><asp:Label ID="lMemberRankDiscount" runat="server"></asp:Label></td>
																		</tr>
																		<%} %>
																		<%-- 会員ランクオプションかつ定期オプションが有効の場合--%>
																		<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
																		<tr>
																			<td class="edit_title_bg" align="right">定期会員割引額</td>
																			<td class="edit_item_bg" align="right"><asp:Label ID="lFixedPurchaseMemberDiscountAmount" runat="server"></asp:Label></td>
																		</tr>
																		<%} %>
																		<%-- クーポンオプションが有効の場合--%>
																		<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
																		<tr>
																			<td class="edit_title_bg" align="right">クーポン割引額</td>
																			<td class="edit_item_bg" align="right"><asp:Label ID="lOrderCouponUse" runat="server"></asp:Label></td>
																		</tr>
																		<%} %>
																		<%-- ポイントオプションが有効の場合--%>
																		<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
																		<tr>
																			<td class="edit_title_bg" align="right">ポイント利用額</td>
																			<td class="edit_item_bg" align="right"><asp:Label ID="lOrderPointUseYen" runat="server"></asp:Label></td>
																		</tr>
																		<%} %>
																		<%-- 定期購入オプションが有効の場合 --%>
																		<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
																		<tr>
																			<td class="edit_title_bg" align="right">定期購入割引額</td>
																			<td class="edit_item_bg" align="right"><asp:Label ID="lFixedPurchaseDiscountPrice" runat="server">\0</asp:Label></td>
																		</tr>
																		<% } %>
																		<tr>
																			<td class="edit_title_bg" align="right">配送料</td>
																			<td class="edit_item_bg" align="right"><asp:Literal ID="lOrderPriceShipping" runat="server"></asp:Literal></td>
																		</tr>
																		<asp:Repeater ID="rSetPromotionShippingChargeDiscount" runat="server" ItemType="w2.App.Common.Order.OrderSetPromotion">
																		<ItemTemplate>
																			<tr visible="<%# Item.IsDiscountTypeShippingChargeFree %>" runat="server">
																				<td class="edit_title_bg" align="right">
																					<%#: Item.OrderSetPromotionNo %>：
																					<%#: Item.SetPromotionName %>割引額<br />
																					(ID:<%#: Item.SetPromotionId %>, 配送料割引分)
																				</td>
																				<td class="edit_item_bg" align="right">
																					<span class='<%# Item.ShippingChargeDiscountAmount > 0 ? "notice" : "" %>'>
																						<%# Item.ShippingChargeDiscountAmount > 0 ? "-" : ""%><%#: Item.ShippingChargeDiscountAmount.ToPriceString(true) %>
																					</span>
																				</td>
																			</tr>
																		</ItemTemplate>
																		</asp:Repeater>
																		<tr>
																			<td class="edit_title_bg" align="right">決済手数料</td>
																			<td class="edit_item_bg" align="right"><asp:Literal ID="lOrderPriceExchange" runat="server"></asp:Literal></td>
																		</tr>
																		<asp:Repeater ID="rSetPromotionPaymentChargeDiscount" runat="server" ItemType="w2.App.Common.Order.OrderSetPromotion">
																		<ItemTemplate>
																			<tr visible="<%# Item.IsDiscountTypePaymentChargeFree %>" runat="server">
																				<td class="edit_title_bg" align="right">
																					<%#: Item.OrderSetPromotionNo %>：
																					<%#: Item.SetPromotionName %>割引額<br />
																					(ID:<%#: Item.SetPromotionId %>, 決済手数料割引分)
																				</td>
																				<td class="edit_item_bg" align="right">
																					<span class='<%# Item.PaymentChargeDiscountAmount > 0 ? "notice" : "" %>'>
																						<%# Item.PaymentChargeDiscountAmount > 0 ? "-" : ""%><%#: Item.PaymentChargeDiscountAmount.ToPriceString(true) %>
																					</span>
																				</td>
																			</tr>
																		</ItemTemplate>
																		</asp:Repeater>
																		<tr>
																			<td class="edit_title_bg" align="right">調整金額</td>
																			<td class="edit_item_bg" align="right"><asp:Literal ID="lOrderPriceRegulation" runat="server"></asp:Literal></td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="right">合計金額</td>
																			<td class="edit_item_bg" align="right"><asp:Literal ID="lOrderPriceTotal" runat="server"></asp:Literal></td>
																		</tr>
																	</tbody>
																</table>
															</td>
															<%--△ 元注文合計情報 △--%>
														</tr>
													</tbody>
												</table>
												<br />

												<%--▽ 交換済み注文の返品商品情報　★★ここを交換注文分くりかえす！！★★ ▽--%>
												<asp:Repeater ID="rExchangedOrder" runat="server" ItemType="w2.App.Common.Order.ReturnOrder">
													<ItemTemplate>
													<div runat="server" Visible="<%# Item.VisibleExchangedOrder %>">
														<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tbody>
																<tr>
																	<td class="edit_title_bg" align="center" colspan="<%: 6 +this.AddColumnCountForItemTable %>">交換済み注文</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" colspan="2">注文ID</td>
																	<td class="edit_item_bg" align="left" colspan="<%# 4 + this.AddColumnCountForItemTable %>"><%#: Item.OrderId %></td>
																	<asp:HiddenField ID="hfOrderId" runat="server" Value="<%# (string)(Item.OrderId)%>" />
																</tr>
																<% if ((Constants.GIFTORDER_OPTION_ENABLED) && (this.ReturnExchangeOrderOrg.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)) { %>
																<tr>
																	<td class="edit_title_bg" align="left" colspan="2">配送先</td>
																	<td class="edit_item_bg" align="left" colspan="<%# 4 + this.AddColumnCountForItemTable %>">配送情報<%#: Item.Shippings[0].OrderShippingNo %></td>
																</tr>
																<% } %>
																<tr>
																	<td class="edit_title_bg" align="center" colspan="<%: 7 + this.AddColumnCountForItemTable %>">返品商品選択</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center" width="10%" rowspan="2"></td>
																	<%--▽ セールOPが有効の場合 ▽--%>
																	<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
																	<td class="edit_title_bg" align="center" width="9%" rowspan="2">セールID</td>
																	<%} %>
																	<%--△ セールOPが有効の場合 △--%>
																	<%--▽ ノベルティOPが有効の場合 ▽--%>
																	<% if (Constants.NOVELTY_OPTION_ENABLED) { %>
																	<td class="edit_title_bg" align="center" width="10%" rowspan="<%= Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>">ノベルティID</td>
																	<%} %>
																	<%--△ ノベルティOPが有効の場合 △--%>
																	<%--▽ レコメンド設定OPが有効の場合 ▽--%>
																	<% if (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) { %>
																	<td class="edit_title_bg" align="center" width="10%" rowspan="2">
																		レコメンドID
																	</td>
																	<%} %>
																	<%--△ レコメンド設定OPが有効の場合 △--%>
																	<%--▽ 商品同梱設定OPが有効の場合 ▽--%>
																	<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED) { %>
																	<td class="edit_title_bg" align="center" width="10%" rowspan="2">
																		商品同梱ID
																	</td>
																	<%} %>
																	<%--△ 商品同梱設定OPが有効の場合 △--%>
																	<td class="edit_title_bg" align="center" width="10%" rowspan="2" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" runat="server">
																		頒布会コースID
																	</td>
																	<td class="edit_title_bg" align="center" width="15%">商品ID</td>
																	<td class="edit_title_bg" align="center" width="<%= 20 + (Constants.PRODUCT_SALE_OPTION_ENABLED ? 0 : 9) + ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 0 :10) + ((Constants.PRODUCTBUNDLE_OPTION_ENABLED) ? 0 : 10) + (this.DisplayItemSubscriptionBoxCourseIdArea ? 0 : 10) %>%" rowspan="2">
																		商品名
																	</td>
																	<td class="edit_title_bg" align="center" width="7%" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																		単価（<%: this.ProductPriceTextPrefix %>）
																	</td>
																	<td class="edit_title_bg" align="center" width="6%" rowspan="2">数量</td>
																	<td class="edit_title_bg" align="center" width="5%" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																		消費税率
																	</td>
																	<td class="edit_title_bg" align="center" width="8%" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																		小計（<%: this.ProductPriceTextPrefix %>）
																	</td>
																</tr>
																<tr>
																	<%--▽ レコメンド設定OPが有効の場合 ▽--%>
																	<% if (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) { %>
																	<td class="edit_title_bg" align="center">
																		レコメンドID
																	</td>
																	<%} %>
																	<%--△ レコメンド設定OPが有効の場合 △--%>
																	<td class="edit_title_bg" align="center">バリエーションID</td>
																</tr>
																<asp:Repeater ID="rReturnOrderItemExchanged" runat="server" DataSource='<%# Item.Items %>' ItemType="w2.App.Common.Order.ReturnOrderItem">
																	<ItemTemplate>
																		<tr id="trItem" runat="server" class="mobile_item_bg">
																			<td align="center" rowspan="2">
																				<%--▽ 返品済み商品の場合、チェックボックスは非表示にする ▽--%>
																				<%#: (Item.ReturnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE) ? "返品済み" : "" %>
																				<%#: (Item.ReturnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_EXCHANGED) ? "交換元" : "" %>
																				<asp:CheckBox ID="cbReturnProduct" runat="server" Visible="<%# (Item.ReturnStatus != Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE ) && (Item.ReturnStatus != Constants.FLG_ORDER_RETURN_STATUS_RETURN_EXCHANGED)%>" AutoPostBack="true" OnCheckedChanged="cbReturnProduct_CheckedChangedExchange" />
																				<%--△ 返品済み商品の場合、チェックボックスは非表示にする △--%>
																				<%--▽ 隠しタグ ▽--%>
																				<asp:HiddenField ID="hfReturnStatus" runat="server" Value="<%# Item.ReturnStatus %>" />
																				<asp:HiddenField ID="hfProductId" runat="server" Value="<%# Item.ProductId %>" />
																				<asp:HiddenField ID="hfVId" runat="server" Value="<%# Item.VId %>" />
																				<asp:HiddenField ID="hfSupplierId" runat="server" Value="<%# Item.SupplierId %>" />
																				<asp:HiddenField ID="hfProductName" runat="server" Value="<%# Item.ProductName %>" />
																				<asp:HiddenField ID="hfProductNameKana" runat="server" Value="<%# Item.ProductNameKana %>" />
																				<asp:HiddenField ID="hfProductPrice" runat="server" Value="<%# Item.ProductPrice %>" />
																				<asp:HiddenField ID="hfItemQuantity" runat="server" Value="<%# Item.ItemQuantity %>" />
																				<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# Item.ProductSaleId %>" />
																				<asp:HiddenField ID="hfNoveltyId" runat="server" Value="<%# Item.NoveltyId %>" />
																				<asp:HiddenField ID="hfRecommendId" runat="server" Value="<%# Item.RecommendId %>" />
																				<asp:HiddenField ID="hfProductBundleId" runat="server" Value="<%# Item.ProductBundleId %>" />
																				<asp:HiddenField ID="hfBundleItemDisplayType" runat="server" Value="<%# Item.BundleItemDisplayType %>" />
																				<asp:HiddenField ID="hfBrandId" runat="server" Value="<%# Item.BrandId %>" />
																				<asp:HiddenField ID="hfDownloadUrl" runat="server" Value="<%# Item.DownloadUrl %>" />
																				<asp:HiddenField ID="hfCooperationId1" runat="server" Value="<%# Item.CooperationId1 %>" />
																				<asp:HiddenField ID="hfCooperationId2" runat="server" Value="<%# Item.CooperationId2 %>" />
																				<asp:HiddenField ID="hfCooperationId3" runat="server" Value="<%# Item.CooperationId3 %>" />
																				<asp:HiddenField ID="hfCooperationId4" runat="server" Value="<%# Item.CooperationId4 %>" />
																				<asp:HiddenField ID="hfCooperationId5" runat="server" Value="<%# Item.CooperationId5 %>" />
																				<asp:HiddenField ID="hfCooperationId6" runat="server" Value="<%# Item.CooperationId6 %>" />
																				<asp:HiddenField ID="hfCooperationId7" runat="server" Value="<%# Item.CooperationId7 %>" />
																				<asp:HiddenField ID="hfCooperationId8" runat="server" Value="<%# Item.CooperationId8 %>" />
																				<asp:HiddenField ID="hfCooperationId9" runat="server" Value="<%# Item.CooperationId9 %>" />
																				<asp:HiddenField ID="hfCooperationId10" runat="server" Value="<%# Item.CooperationId10 %>" />
																				<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# Item.ProductOptionValue %>' />
																				<asp:HiddenField ID="hfOrderShippingNo" runat="server" Value='<%# Item.OrderShippingNo %>' />
																				<asp:HiddenField ID="hfShippingName" runat="server" Value='<%# Item.ShippingName %>' />
																				<asp:HiddenField ID="hfShippingNameKana" runat="server" Value='<%# Item.ShippingNameKana %>' />
																				<asp:HiddenField ID="hfShippingTel1" runat="server" Value='<%# Item.ShippingTel1 %>' />
																				<asp:HiddenField ID="hfShippingCountryIsoCode" runat="server" Value='<%# Item.ShippingCountryIsoCode %>' />
																				<asp:HiddenField ID="hfShippingCountryName" runat="server" Value='<%# Item.ShippingCountryName %>' />
																				<asp:HiddenField ID="hfShippingZip" runat="server" Value='<%# Item.ShippingZip %>' />
																				<asp:HiddenField ID="hfShippingAddr1" runat="server" Value='<%# Item.ShippingAddr1 %>' />
																				<asp:HiddenField ID="hfShippingAddr2" runat="server" Value='<%# Item.ShippingAddr2 %>' />
																				<asp:HiddenField ID="hfShippingAddr3" runat="server" Value='<%# Item.ShippingAddr3 %>' />
																				<asp:HiddenField ID="hfShippingAddr4" runat="server" Value='<%# Item.ShippingAddr4 %>' />
																				<asp:HiddenField ID="hfShippingAddr5" runat="server" Value='<%# Item.ShippingAddr5 %>' />
																				<asp:HiddenField ID="hfShippingCompanyName" runat="server" Value='<%# Item.ShippingCompanyName %>' />
																				<asp:HiddenField ID="hfShippingCompanyPostName" runat="server" Value='<%# Item.ShippingCompanyPostName %>' />
																				<asp:HiddenField ID="hfProductPricePretax" runat="server" Value='<%# Item.ProductPricePretax %>' />
																				<asp:HiddenField ID="hfProductTaxIncludedFlg" runat="server" Value='<%# Item.ProductTaxIncludedFlg %>' />
																				<asp:HiddenField ID="hfProductTaxRate" runat="server" Value='<%# TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate) %>' />
																				<asp:HiddenField ID="hfProductTaxRoundType" runat="server" Value='<%# Item.ProductTaxRoundType %>' />
																				<asp:HiddenField ID="hfOrderSetPromotionNo" runat="server" Value='<%# Item.OrderSetPromotionNo %>' />
																				<asp:HiddenField ID="hfFixedPurchaseProductFlg" runat="server" Value='<%# Item.FixedPurchaseProductFlg %>' />
																				<asp:HiddenField ID="hfSubscriptionBoxCourseId" Value="<%# Item.SubscriptionBoxCourseId %>" runat="server" />
																				<asp:HiddenField ID="hfSubscriptionBoxFixedAmount" Value="<%# Item.SubscriptionBoxFixedAmount %>" runat="server" />
																				<%--△ 隠しタグ △--%>
																			</td>
																			<td align="center" rowspan="2" runat="server" visible='<%# Constants.PRODUCT_SALE_OPTION_ENABLED %>'><%#: (Item.ProductSaleId != "") ? Item.ProductSaleId : "-" %></td>
																			<td align="center" rowspan="<%# Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>" runat="server" visible='<%# Constants.NOVELTY_OPTION_ENABLED %>'>
																				<%#: (Item.NoveltyId != "") ? Item.NoveltyId : "-" %>
																			</td>
																			<td align="center" rowspan="2" runat="server" visible='<%# (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) %>'>
																				<%#: (Item.RecommendId != "") ? Item.RecommendId : "-" %>
																			</td>
																			<td align="center" rowspan="2" runat="server" visible='<%# Constants.PRODUCTBUNDLE_OPTION_ENABLED %>'>
																				<%# OrderPage.GetProductBundleIdDisplayValue(Item.ProductBundleId, this.LoginShopOperator) %>
																			</td>
																			<td align="center" rowspan="2" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" runat="server">
																				<div Visible="<%# Item.IsSubscriptionBox %>" runat="server">
																					<a href="javascript:open_window('<%#: FixedPurchasePage.CreateSubscriptionBoxRegisterUrl(Item.SubscriptionBoxCourseId) %>','fixedpurchase','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																						<%#: Item.SubscriptionBoxCourseId %>
																					</a>
																				</div>
																				<div Visible="<%# Item.IsSubscriptionBox == false %>" runat="server">-</div>
																			</td>
																			<td align="center" ><%#: Item.ProductId %></td>
																			<td align="left" rowspan="2">
																				<%#: Item.ProductName %><br />
																				<span visible='<%# (Item.OrderSetPromotionNo != "") %>' runat="server">
																					[<%#: Item.OrderSetPromotionNo %> ： <%#: this.ReturnExchangeOrderOrg.GetOrderSetPromotionName(Item.OrderSetPromotionNo) %>]
																				</span>
																			</td>
																			<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																				<%#: Item.IsSubscriptionBoxFixedAmount == false ? Item.ProductPrice.ToPriceString(true) : "-" %>
																			</td>
																			<td align="center" rowspan="2">
																				<span id="spanProductPrice" runat="server">
																					<span id="spanMinusProductPrice" runat="server" /><%#: StringUtility.ToNumeric(Item.ItemQuantity) %>
																				</span>
																			</td>
																			<td align="center" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																				<span id="spanProductTaxRate" Visible="<%# Item.IsSubscriptionBoxFixedAmount == false %>" runat="server">
																					<span id="spanMinusProductTaxRate" runat="server" /><%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate) %>%
																				</span>
																				<div Visible="<%# Item.IsSubscriptionBoxFixedAmount %>" runat="server">-</div>
																			</td>
																			<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																				<span id="spanItemQuantity" Visible="<%# Item.IsSubscriptionBoxFixedAmount == false %>" runat="server">
																					<span id="spanMinusItemQuantity" runat="server" /><%#: (Item.ReturnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_EXCHANGED) ? "-" : ""%><%#: Item.OptionIncludedProductPrice.ToPriceString(true) %>
																				</span>
																				<div Visible="<%# Item.IsSubscriptionBoxFixedAmount %>" runat="server">
																					定額<br />
																					(<%#: string.Format("{0}{1}", this.ReturnExchangeOrderOrg.AllReturnedFixedAmountCourseIds.Contains(Item.SubscriptionBoxCourseId) ? "-" : "", Item.SubscriptionBoxFixedAmount.ToPriceString(true)) %>)
																				</div>
																			</td>
																		</tr>
																		<tr id="trItem1" runat="server" class="mobile_item_bg">
																			<td align="center" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) %>">
																				<%#: (Item.RecommendId != "") ? (string)(Item.RecommendId) : "-" %>
																			</td>
																			<td align="center"><%# (Item.VId != "") ? "商品ID + " + Item.VId : "-" %></td>
																		</tr>
																		<tr class="mobile_item_bg" visible='<%# (Item.ProductOptionValue != "") %>' runat="server">
																			<td align="center" class="edit_title_bg">付帯情報</td>
																			<td id="tdItem1" align="left" colspan="<%# 4 + this.AddColumnCountForItemTable %>">
																				<%#: Item.ProductOptionValue %>
																			</td>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
																<tr>
																	<td class="edit_title_bg" align="right" colspan="<%# 5 + this.AddColumnCountForItemTable %>">返品商品合計</td>
																	<td class="edit_item_bg" align="right"><asp:Label ID="lbReturnOrderPriceSubTotal" Text ='<%# "0".ToPriceString(true) %>' runat="server"></asp:Label></td>
																</tr>
															</tbody>
														</table>
														<br />
														<table width="758" border="0" cellspacing="0" cellpadding="0">
															<tbody>
																<tr align="right" valign="top">
																	<%--▽ 交換済み注文合計情報 ▽--%>
																	<td width="250" align="right">
																		<table class="edit_table" cellspacing="1" cellpadding="3" width="250" border="0">
																			<tbody>
																				<tr style="height: 21px">
																					<td class="edit_title_bg" align="center" colspan="2">交換済み注文合計情報</td>
																				</tr>
																				<tr>
																					<td class="edit_title_bg" align="right" width="60%">商品合計</td>
																					<td class="edit_item_bg" align="right"><%#: (Item.OrderSummaryInfo)["OrderPriceSubtotal"].ToPriceString(true) %> </td>
																				</tr>
																				<tr>
																					<td class="edit_title_bg" align="right">配送料</td>
																					<td class="edit_item_bg" align="right"><%#: (Item.OrderSummaryInfo)["OrderPriceShipping"].ToPriceString(true) %> </td>
																				</tr>
																				<%-- ポイントオプションが有効の場合--%>
																				<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
																				<tr>
																					<td class="edit_title_bg" align="right">ポイント利用額</td>
																					<td class="edit_item_bg" align="right">
																						<span class='<%# decimal.Parse((Item.OrderSummaryInfo)["OrderPointUseYen"]) < 0 ? "notice" : "" %>'>
																							<%#: (Item.OrderSummaryInfo)["OrderPointUseYen"].ToPriceString(true) %>
																						</span>
																					</td>
																				</tr>
																				<%} %>
																				<%-- クーポンオプションが有効の場合--%>
																				<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
																				<tr>
																					<td class="edit_title_bg" align="right">クーポン割引額</td>
																					<td class="edit_item_bg" align="right">
																						<span class='<%# decimal.Parse((Item.OrderSummaryInfo)["OrderCouponUse"]) < 0 ? "notice" : "" %>'>
																							<%#: (Item.OrderSummaryInfo)["OrderCouponUse"].ToPriceString(true) %>
																						</span>
																					</td>
																				</tr>
																				<%} %>
																				<tr>
																					<td class="edit_title_bg" align="right">決済手数料</td>
																					<td class="edit_item_bg" align="right"><%#: (Item.OrderSummaryInfo)["OrderPriceExchange"].ToPriceString(true) %> </td>
																				</tr>
																				<tr>
																					<td class="edit_title_bg" align="right">調整金額</td>
																					<td class="edit_item_bg" align="right"><%#: (Item.OrderSummaryInfo)["OrderPriceRegulation"].ToPriceString(true) %> </td>
																				</tr>
																				<tr>
																					<td class="edit_title_bg" align="right">合計金額</td>
																					<td class="edit_item_bg" align="right"><%#: (Item.OrderSummaryInfo)["OrderPriceTotal"].ToPriceString(true) %> </td>
																				</tr>
																			</tbody>
																		</table>
																	</td>
																	<%--△ 交換注文合計情報 △--%>
																</tr>
														</table>
														<br />
													</div>
													</ItemTemplate>
												</asp:Repeater>
												<%--△ 交換済み注文の返品商品情報  ★☆ここまで繰り返し★★△--%>

												<%--▽ 交換商品情報 ▽--%>
												<div id="divExchangeOrderItem" runat="server">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody id="tbdyExchangeOrderItemErrorMessages" runat="server" visible="false">
													    <tr>
													        <td class="edit_title_bg" align="center" colspan='<%: 7 + this.AddColumnCountForItemTable %>'>エラーメッセージ</td>
													    </tr>
													    <tr>
													        <td class="edit_item_bg" align="left" colspan='<%: 7 + this.AddColumnCountForItemTable %>'>
													            <asp:Label ID="lbExchangeOrderItemErrorMessages" runat="server" ForeColor="red"></asp:Label>
													        </td>
													    </tr>
													</tbody>
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan='<%: (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 4 : 6) + (this.DisplayItemSubscriptionBoxCourseIdArea ? -1 : 0) + this.AddColumnCountForItemTable %>'>
																交換商品注文登録
															</td>
															<td class="edit_title_bg" align="center"><asp:Button ID="btnAddProduct" runat="server" Text="追加" onclick="btnAddProduct_Click" UseSubmitBehavior="False" /></td>
														</tr>
													    <tr>
														    <td class="edit_title_bg" align="center" width="12%" rowspan="2">検索/削除</td>
															<%--▽ セールOPが有効の場合 ▽--%>
															<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
															<td class="edit_title_bg" align="center" width="8%" rowspan="2">セールID</td>
															<%} %>
															<%--△ セールOPが有効の場合 △--%>
															<%--▽ ノベルティOPが有効の場合 ▽--%>
															<% if (Constants.NOVELTY_OPTION_ENABLED) { %>
															<td class="edit_title_bg" align="center" width="10%" rowspan="<%= Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>">ノベルティID</td>
															<%} %>
															<%--△ ノベルティOPが有効の場合 △--%>
															<%--▽ レコメンド設定OPが有効の場合 ▽--%>
															<% if (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) { %>
															<td class="edit_title_bg" align="center" width="10%" rowspan="2">
																レコメンドID
															</td>
															<%} %>
															<%--△ レコメンド設定OPが有効の場合 △--%>
															<%--▽ 商品同梱設定OPが有効の場合 ▽--%>
															<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED) { %>
															<td class="edit_title_bg" align="center" width="10%" rowspan="2">
																商品同梱ID
															</td>
															<%} %>
															<%--△ 商品同梱設定OPが有効の場合 △--%>
															<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
															<td class="edit_title_bg" align="center" width="5%" rowspan="2">
																定期
															</td>
															<%} %>
															<%--△ 定期OPが有効の場合 △--%>
															<td class="edit_title_bg" align="center" width="15%">商品ID</td>
															<td class="edit_title_bg" align="center" width="<%= 13 + (Constants.PRODUCT_SALE_OPTION_ENABLED ? 0 :8) + ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 0 :10) + (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 0 : 10 ) %>%" rowspan="2">商品名</td>
															<% if (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
															<td class="edit_title_bg" align="center" width="7%" rowspan="2">単価（<%: this.ProductPriceTextPrefix %>）</td>
															<% } %>
															<% if (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()) { %>
															<td class="edit_title_bg" align="center" width="5%" rowspan="2" colspan="2">数量</td>
															<% } %>
															<% if (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
															<td class="edit_title_bg" align="center" width="5%" rowspan="2">数量</td>
															<td class="edit_title_bg" align="center" width="5%" rowspan="2">消費税率</td>
															<td class="edit_title_bg" align="center" width="10%" rowspan="2">小計（<%: this.ProductPriceTextPrefix %>）</td>
															<% } %>
														</tr>
														<tr>
															<%--▽ レコメンド設定OPが有効の場合 ▽--%>
															<% if (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) { %>
															<td class="edit_title_bg" align="center">
																レコメンドID
															</td>
															<%} %>
															<td class="edit_title_bg" align="center" nowrap="nowrap">バリエーションID</td>
														</tr>
														<asp:Repeater ID="rExchangeOrderItem" runat="server" OnItemCommand="rExchangeOrderItem_ItemCommand" ItemType="w2.App.Common.Order.ReturnOrderItem">
															<ItemTemplate>
																<tr class="edit_item_bg" style="height: 19px">
																	<td align="center" rowspan="2" nowrap="nowrap">
																		<asp:HiddenField ID="hfProductTaxRate" runat="server" Value='<%# TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate) %>' />
																		<input id="inputSearchProduct" type="button" value="検索" onclick="javascript:open_product_list('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT + "&" + Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE + "=" + HttpUtility.UrlEncode(this.ReturnExchangeOrderOrg.ShippingId) + "&" + Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID + "=" + HttpUtility.UrlEncode(this.ReturnExchangeOrderOrg.MemberRankId) + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%# Container.ItemIndex %>');" />
																		<span visible='<%# (Container.ItemIndex != 0) %>' runat="server">
																			<asp:Button ID="btnDeleteProduct" CommandName="delete" CommandArgument="<%# Container.ItemIndex %>" Text="削除" runat="server" UseSubmitBehavior="False" />
																		</span>
																	</td>
																	<%--▽ セールOPが有効の場合 ▽--%>
																	<% if (Constants.PRODUCT_SALE_OPTION_ENABLED){ %>
																	<td align="center" rowspan="2">
																		<asp:TextBox ID="tbProductSaleId" runat="server" Text="<%# Item.ProductSaleId %>" Width="45" MaxLength="8"></asp:TextBox>
																	</td>
																	<%} %>
																	<%--△ セールOPが有効の場合 △--%>
																	<td align="center" rowspan="<%# Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>" runat="server" visible="<%# Constants.NOVELTY_OPTION_ENABLED %>">
																		<asp:TextBox ID="tbNoveltyId" runat="server" Text="<%# Item.NoveltyId %>" Width="45" MaxLength="30"></asp:TextBox>
																	</td>
																	<td align="center" rowspan="2" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) %>">
																		<asp:TextBox ID="tbRecommendId" runat="server" Text="<%# Item.RecommendId %>" Width="50" MaxLength="30"></asp:TextBox>
																	</td>
																	<td align="center" rowspan="2" runat="server" visible="<%# Constants.PRODUCTBUNDLE_OPTION_ENABLED %>">
																		<asp:TextBox ID="tbProductBundleId" Text="<%# Item.ProductBundleId %>" MaxLength="30" Width="50" runat="server"></asp:TextBox>
																	</td>
																	<%--▽ 定期OPが有効の場合 ▽--%>
																	<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
																	<td align="center" rowspan="2">
																		<asp:CheckBox ID="cbFixedPurchase" OnCheckedChanged="cbFixedPurchaseProductFlg_CheckedChanged" AutoPostBack="true" Checked='<%# (StringUtility.ToEmpty(Item.FixedPurchaseProductFlg) == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON) %>' runat="server" />
																	</td>
																	<%} %>
																	<%--△ 定期OPが有効の場合 △--%>
																	<td align="left">
																		<asp:TextBox ID="tbProductId" runat="server" Text="<%# Item.ProductId %>" Width="95" MaxLength="30"></asp:TextBox>
																		<asp:HiddenField ID="hfSupplierId" runat="server" Value="<%# Item.SupplierId %>" />
																		<asp:HiddenField ID="hfProductId" runat="server" Value="<%# Item.ProductId %>" />
																		<asp:HiddenField ID="hfSubscriptionBoxCourseId" Value="<%# this.ReturnExchangeOrderOrg.SubscriptionBoxCourseId %>" runat="server" />
																		<asp:HiddenField ID="hfSubscriptionBoxFixedAmount" Value="<%# this.ReturnExchangeOrderOrg.SubscriptionBoxFixedAmount %>" runat="server" />
																	</td>
																	<td align="left" rowspan="2">
																		<asp:TextBox ID="tbProductName" runat="server" Text="<%# Item.ProductName %>" Width="130" MaxLength="200"></asp:TextBox><br />
																		<asp:DropDownList ID="ddlOrderSetPromotion" DataSource="<%# this.OrderSetPromotionList %>" DataTextField="Text" DataValueField="Value" Visible="<%# this.ReturnExchangeOrderOrg.SetPromotions.Count != 0 %>" SelectedValue="<%# Item.OrderSetPromotionNo %>" runat="server"></asp:DropDownList>
																	</td>
																	<td align="center" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																		<asp:TextBox id="tbProductPrice" runat="server" Text="<%# Item.ProductPrice.ToPriceString() %>" Width="45" Visible="<%# Item.IsSubscriptionBoxFixedAmount == false %>" />
																		<div Visible="<%# Item.IsSubscriptionBoxFixedAmount %>" runat="server">-</div>
																	</td>
																	<td align="center" rowspan="2" colspan="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 2 : 1 %>">
																		<asp:TextBox id="tbItemQuantity" runat="server" Text="<%# Item.ItemQuantity %>" Width="25" MaxLength="3" />
																	</td>
																	<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																		<%#: Item.IsSubscriptionBoxFixedAmount == false ? string.Format("{0}%", TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate)) : "-" %>
																	</td>
																	<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																		<%#: Item.IsSubscriptionBoxFixedAmount == false
																			     ? Item.ItemPrice.ToPriceString(true)
																			     : string.Format("定額({0})", Item.SubscriptionBoxFixedAmount.ToPriceString(true)) %>
																	</td>
																</tr>
																<tr class="edit_item_bg" style="height: 19px">
																	<td align="center" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) %>">
																		<asp:TextBox ID="tbRecommendId2" runat="server" Text="<%# Item.RecommendId %>" Width="45" MaxLength="30"></asp:TextBox>
																	</td>
																	<td align="left">
																		<asp:TextBox ID="tbVId" runat="server" Text="<%# Item.VId %>" Width="60" MaxLength="30"></asp:TextBox>
																		<asp:Button ID="btnGetProductData" CommandArgument="<%# Container.ItemIndex %>" CommandName="get" Text="取得" runat="server" UseSubmitBehavior="False" />
																		<asp:HiddenField ID="hfItemIndex" runat="server" Value="<%# Container.ItemIndex %>" />
																	</td>
																</tr>
																<tr style="height: 19px">
																	<td align="center" class="edit_title_bg">付帯情報</td>
																	<td id="tdItem1" align="left" class="edit_item_bg" colspan='<%#: (this.IsSubscriptionBoxFixedAmount ? 4 : 6) + this.AddColumnCountForItemTable %>'>
																		<div id="dvProductOptionValue" visible='<%# Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED%>' style="margin: 10px 0" runat="server">
																			<!-- ▽ 付帯情報編集欄 ▽ -->
																			<asp:Repeater ID="rProductOptionValueSettings" ItemType="w2.App.Common.Product.ProductOptionSetting" DataSource="<%# Item.GetProductOptionSettingList() %>" runat="server">
																				<ItemTemplate>
																					<div style="margin-top: 6px;">
																						<strong style="display: inline-block; padding: 2px 1px;"><%#: Item.ValueName %><span class="notice" runat="server" visible="<%# Item.IsNecessary %>">*</span></strong><br />
																						<span style="display: inline-block">
																							<asp:DropDownList runat="server" ID="ddlProductOptionValueSetting" DataSource="<%# Item.SettingValuesListItemCollection %>" Visible="<%# Item.IsSelectMenu || Item.IsDropDownPrice%>" SelectedValue="<%# Item.GetDisplayProductOptionSettingSelectedValue() %>" />
																							<asp:TextBox ID="tbProductOptionValueSetting" Text="<%# Item.SelectedSettingValueForTextBox %>" Visible="<%# Item.IsTextBox %>" runat="server" />
																							<asp:Repeater ID="rCblProductOptionValueSetting" ItemType="System.Web.UI.WebControls.ListItem" DataSource="<%# Item.SettingValuesListItemCollection %>" Visible="<%# Item.IsCheckBox || Item.IsCheckBoxPrice %>" runat="server">
																								<ItemTemplate>
																									<span title="<%# Item.Text %>">
																										<asp:CheckBox ID="cbProductOptionValueSetting" Text='<%# Item.Text %>' Checked='<%# Item.Selected %>' runat="server" />
																									</span>
																								</ItemTemplate>
																							</asp:Repeater>
																						</span>
																					</div>
																				</ItemTemplate>
																			</asp:Repeater>
																			<!-- △ 付帯情報編集欄 △ -->
																		</div>
																		
																		<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false){ %>
																		<asp:TextBox ID="tbProductOptionValue" Text='<%# Item.ProductOptionValue %>' Width="650" runat="server"></asp:TextBox>
																		<% } %>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr>
															<td class="edit_title_bg" align="right" colspan='<%: (this.IsSubscriptionBoxFixedAmount ? 4 : 6) + this.AddColumnCountForItemTable %>'>交換商品合計（<%: this.ProductPriceTextPrefix %>）</td>
															<td class="edit_item_bg" align="right"><asp:Label ID="lbExchangeOrderPriceSubTotal" runat="server"></asp:Label></td>
														</tr>
														<%-- ポイントオプションが有効の場合--%>
														<%if (Constants.W2MP_POINT_OPTION_ENABLED){ %>
														<tr>
															<td class="edit_title_bg" align="right" colspan='<%: (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 4 : 6) + (this.DisplayItemSubscriptionBoxCourseIdArea ? -1 : 0) + this.AddColumnCountForItemTable %>'>交換時ポイント発行</td>
															<td class="edit_item_bg" align="right"><asp:TextBox ID="tbExchangeAddPoint" runat="server" Width="45" Text="0"></asp:TextBox>pt</td>
														</tr>
														<%} %>
													</tbody>
												</table>
												<br />
												</div>
												<%--△ 交換商品情報 △--%>
												
												<%--▽ 返品交換合計情報 ▽--%>
												<table width="758" border="0" cellspacing="0" cellpadding="0" >
													<tbody>
														<tr valign="top">
															<td align="right" width="450">
																<table class="edit_table" cellspacing="1" cellpadding="3" width="250" border="0">
																	<tbody>
																	<tr>
																			<td class="edit_title_bg" align="center" colspan="3">返金情報</td>
																	</tr>																				
																	<tr>
																			<td class="edit_title_bg" align="right" width="61%" colspan="2">返金金額</td>
																		<td class="edit_item_bg" align="center"><asp:TextBox ID="tbOrderPriceRepayment" runat="server" Width="58" Text="0"></asp:TextBox></td>
																	</tr>
																	<tr>
																			<td class="edit_title_bg" align="center" colspan="3">返金先情報</td>
																		</tr>
																		<% if(Constants.TWPELICAN_COOPERATION_EXTEND_ENABLED){ %>
																		<tr>
																			<td class="edit_title_bg" align="left" width="20%">入力タイプ</td>
																			<td class="edit_item_bg" align="left" colspan="2">
																				<asp:RadioButtonList ID="rblRepaymentMemoType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="True" OnSelectedIndexChanged="rblRepaymentMemoType_OnSelectedIndexChanged"></asp:RadioButtonList>
																			</td>
																		</tr>
																		<% } %>
																		<tr id="trRepaymentMemoFreeText" runat="server">
																			<td class="edit_item_bg" align="center" colspan="3"><asp:TextBox ID="tbRepaymentMemo" runat="server" Columns="50" Rows="8" TextMode="MultiLine"></asp:TextBox></td>
																		</tr>
																	</tbody>
																	<tbody id="tbodyRepaymentBankText" runat="server">
																		<tr>
																			<td class="edit_title_bg" align="left" width="20%">銀行コード</td>
																			<td class="edit_item_bg" align="left" colspan="2">
																				<asp:TextBox ID="tbRepaymentBankCode" runat="server" Width="10%" MaxLength="3"></asp:TextBox>
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="20%">銀行名</td>
																			<td class="edit_item_bg" align="left" colspan="2">
																				<asp:TextBox ID="tbRepaymentBankName" runat="server" Width="40%" MaxLength="50"></asp:TextBox>
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="20%">支店名</td>
																			<td class="edit_item_bg" align="left" colspan="2">
																				<asp:TextBox ID="tbRepaymentBankBranch" runat="server" Width="40%" MaxLength="50"></asp:TextBox>
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="20%">口座番号</td>
																			<td class="edit_item_bg" align="left" colspan="2">
																				<asp:TextBox ID="tbRepaymentBankAccountNo" runat="server" Width="40%" MaxLength="50"></asp:TextBox>
																			</td>
																	</tr>															
																	<tr>
																			<td class="edit_title_bg" align="left" width="20%">口座名</td>
																			<td class="edit_item_bg" align="left" colspan="2">
																				<asp:TextBox ID="tbRepaymentBankAccountName" runat="server" Width="40%" MaxLength="50"></asp:TextBox>
																			</td>
																	</tr>																				
																	</tbody>
																</table>															
															</td>
															<td align="right" width="250">
																 <table class="edit_table" cellspacing="1" cellpadding="3" width="250" border="0">
																	<tbody id="tbdyReturnExchangeErrorMessages" runat="server" visible="false">
																		<tr>
																			<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
																		</tr>
																		<tr>
																			<td class="edit_item_bg" align="left" colspan="2">
																			<asp:Label ID="lbReturnExchangeErrorMessages" runat="server" ForeColor="red"></asp:Label>
																			</td>
																		</tr>
																	</tbody>
																	<tbody>
																	<tr>
																		<td class="edit_title_bg" align="center" width="61%">返品交換合計情報</td>
																		<td class="edit_title_bg" align="center">
																		<asp:Button ID="btnReCalculate" runat="server" Text="再計算" onclick="btnRecalculate_Click" UseSubmitBehavior="False" />
																		<br/>
																		<small><asp:CheckBox ID="cbOrderPriceCorrectionReCalculate" runat="server" CssClass="checkBox" AutoPostBack="true" OnCheckedChanged="cbOrderPriceCorrectionReCalculate_CheckedChanged" Text="金額補正の自動計算" /></small>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="right">商品合計（<%: this.ProductPriceTextPrefix %>）</td>
																		<td class="edit_item_bg" align="right"><asp:Label ID="lbReturnExchangeOrderPriceSubTotal" runat="server"></asp:Label></td>
																	</tr>
																	<%if (this.ProductIncludedTaxFlg == false) { %>
																		<tr>
																			<td class="edit_title_bg" align="right">消費税</td>
																			<td class="edit_item_bg" align="right"><asp:Label ID="lbReturnExchangeOrderPriceTax" runat="server"></asp:Label></td>
																		</tr>
																	<%} %>
																	<asp:Repeater ID="rPriceCorrection" ItemType="w2.Domain.Order.OrderPriceByTaxRateModel" runat="server">
																		<ItemTemplate>
																	<tr>
																				<asp:HiddenField ID="hfTaxRate" runat="server" value="<%# Item.KeyTaxRate %>" />
																		<td class="edit_title_bg" align="right">
																					返品用金額補正(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate) %>%)
																		</td>
																				<td class="edit_item_bg" align="right">
																					<asp:TextBox ID="tbPriceCorrection" runat="server" Width="60" Text="<%#: Item.ReturnPriceCorrectionByRate.ToPriceString() %>"></asp:TextBox>
																				</td>
																	</tr>
																		</ItemTemplate>
																	</asp:Repeater>
																	<asp:Repeater ID="rReturnExchangePriceByTaxRate" ItemType="w2.Domain.Order.OrderPriceByTaxRateModel" runat="server">
																		<ItemTemplate>
																			<tr runat="server">
																				<td class="edit_title_bg" align="right">
																					最終合計金額内訳(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate)%>%)
																				</td>
																				<td class="edit_item_bg" align="right">
																					<asp:Label ID="lbReturnExchangeOrderPriceTotalByTaxRate" runat="server" style='<%#: (Item.PriceTotalByRate < 0) ? "color: red;" : ""%>'>
																						<%#: Item.PriceTotalByRate.ToPriceString(true) %>
																					</asp:Label>
																				</td>
																			</tr>
																		</ItemTemplate>
																	</asp:Repeater>
																	<tr>
																		<td class="edit_title_bg" align="right">最終合計金額</td>
																		<td class="edit_item_bg" align="right"><asp:Label ID="lbReturnExchangeOrderPriceTotal" runat="server"></asp:Label></td>
																	</tr>
																	</tbody>
																</table>
																<br />
																<table class="edit_table" cellspacing="1" cellpadding="3" width="250" border="0">
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">調整金額メモ</td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="center" colspan="2"><asp:TextBox ID="tbRegulationMemo" runat="server" TextMode="MultiLine" Columns="25" Rows="3"></asp:TextBox></td>
																	</tr>
																</table>
															</td>
														</tr>
													</tbody>
												</table>
												<%--▽ 請求金額情報 ▽--%>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<asp:Repeater ID="rReturnExchangeLastBilledAmountBeforeByTaxRate" runat="server" ItemType="w2.Domain.Order.OrderPriceByTaxRateModel">
															<ItemTemplate>
																<tr  visible="<%# Container.ItemIndex == 0 %>"  runat="server">
																	<td class="detail_title_bg" align="right" rowspan="<%# ((List<OrderPriceByTaxRateModel>)rReturnExchangeLastBilledAmountBeforeByTaxRate.DataSource).Count + 1 %>" width="60%" runat="server">前回の請求金額</td>
																	<td class="detail_item_bg" align="right" rowspan="<%# ((List<OrderPriceByTaxRateModel>)rReturnExchangeLastBilledAmountBeforeByTaxRate.DataSource).Count + 1 %>" width="10%" runat="server">
																		<%# GetMinusNumberNoticeHtml(this.ReturnExchangeOrderOrg.RelatedOrderLastBilledAmount, true) %>
																	</td>
																</tr>
																<tr runat="server">
																	<td class="edit_title_bg" width="20%" align="right">
																		内訳(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate)%>%)
																	</td>
																	<td class="edit_item_bg" width="10%" align="right">
																		<%# GetMinusNumberNoticeHtml(Item.PriceTotalByRate, true) %>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													<asp:Repeater ID="rReturnExchangeLastBilledAmountByTaxRate" runat="server" ItemType="w2.Domain.Order.OrderPriceByTaxRateModel">
														<ItemTemplate>
															<tr  visible="<%# Container.ItemIndex == 0 %>"  runat="server">
																<td class="detail_title_bg" align="right" rowspan="<%# ((List<OrderPriceByTaxRateModel>)rReturnExchangeLastBilledAmountByTaxRate.DataSource).Count + 1 %>" width="60%" runat="server">今回の請求金額</td>
																<td class="detail_item_bg" align="right" rowspan="<%# ((List<OrderPriceByTaxRateModel>)rReturnExchangeLastBilledAmountByTaxRate.DataSource).Count + 1 %>" width="10%" runat="server">
																	<asp:Label ID="lbReturnExchangeLastBilledAmount" runat="server"></asp:Label>
																</td>
														</tr>
															<tr runat="server">
																<td class="edit_title_bg" width="20%" align="right">
																	内訳(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate)%>%)
																</td>
																<td class="edit_item_bg" width="10%" align="right">
																	<%# GetMinusNumberNoticeHtml(Item.PriceTotalByRate, true) %>
																</td>
														</tr>
														</ItemTemplate>
													</asp:Repeater>
													</tbody>
												</table>
												<%--△ 請求金額情報 △--%>
												<%--△ 返品交換合計情報 △--%>
												<div class="action_part_bottom">
													<asp:HiddenField ID="hfPayTgSendId" runat="server" />
													<asp:HiddenField ID="hfPayTgPostData" runat="server" />
													<asp:HiddenField ID="hfPayTgResponse" runat="server" />
													<asp:Button ID="btnProcessPayTgResponse" runat="server" style="display: none;" OnClick="btnProcessPayTgResponse_Click" />
													<asp:Button ID="btnBackDetailBottom" runat="server" Text="  詳細へ戻る  " onclick="btnBackDetail_Click" UseSubmitBehavior="False" />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" UseSubmitBehavior="False" OnClientClick="doPostbackEvenIfCardAuthFailed=false;" />
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<%-- 戻る遷移のとき、テキストボックスがマスクされていたらポストバックさせる --%>
<%if (OrderCommon.CreditTokenUse) {%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";

	// ページロード処理
	function pageLoad(sender, args) {

		// 戻るボタンで戻ってきたとき、クレジットカード番号がマスキングされたままになるので再計算ボタンイベントを実行する
		var cis = GetCardInfo();
		if (cis && cis[0] && (cis[0].CardNo.indexOf("<%= Constants.CHAR_MASKING_FOR_TOKEN %>") != -1)
			&& $("#" + cis[0].TokenHiddenID).val()) {
			__doPostBack('ctl00$ContentPlaceHolderBody$btnReCalculate', '');
		}
	}
</script>

<uc:CreditToken runat="server" ID="CreditToken" />
<%} %>
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>

<script type="text/javascript">
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
				alert('端末に接続できませんでした。端末をご確認ください。');
			}
		}, 100);
	}

	// PayTg：カード登録実行
	function execCardRegistration(url) {
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

		requestCheckDevice.done(function(data) {
			if (data["canUseDevice"] === true) {
				registerCreditCard(url);
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
	function registerCreditCard(url) {
		var postData = JSON.parse($('#<%= hfPayTgPostData.ClientID %>').val());
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
			data: JSON.stringify(cleanedData),
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

	// Show confirm button area
	function showConfirmButtonArea() {
		$('.action_part_bottom').show();
	}

	// Hide confirm button area
	function hideConfirmButtonArea() {
		$('.action_part_bottom').hide();
	}
</script>
</asp:Content>
