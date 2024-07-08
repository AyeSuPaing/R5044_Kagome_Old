<%--
=========================================================================================================
  Module      : 注文情報編集ページ(OrderModifyInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderModifyInput.aspx.cs" Inherits="Form_Order_OrderModifyInput"  maintainScrollPositionOnPostBack="true" %>
<%@ Register TagPrefix="uc" TagName="FieldMemoSetting" Src="~/Form/Common/FieldMemoSetting/BodyFieldMemoSetting.ascx" %>
<%@ Register Src="~/Form/Common/CreditToken.ascx" TagPrefix="uc" TagName="CreditToken" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Input.Order" %>
<%@ Import Namespace="w2.Domain.Payment" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<asp:Button ID="btnTooltipInfo" runat="server" style="display:none;" UseSubmitBehavior="false"/>
<%
var loopCount = 0;
var iShippingLoop = 0;
%>
<script type="text/javascript">
	<!--
	// 選択商品
	var selected_shipping_index = 0;
	var selected_product_index = 0;

	// 商品一覧画面表示
	function open_product_list(link_file, window_name, window_type, shipping_index, product_index) {
		// 選択商品を格納
		selected_shipping_index = shipping_index;
		selected_product_index = product_index;	
		// ウィンドウ表示
		open_window(link_file,window_name,window_type);
	}

	// 商品一覧で選択された商品情報を設定
	function set_productinfo(product_id, supplier_id, variation_id, product_name, product_display_price, product_special_price, product_price, sale_id, fixed_purchase_id, limitedfixedpurchasekbn1setting, limitedfixedpurchasekbn3setting, limitedfixedpurchasekbn4setting, tax_rate) {
		<%
	iShippingLoop = 0;
	// 注文商品数分ループ
	foreach (RepeaterItem riShipping in rShippingList.Items)
	{
		loopCount = 0;
%>
		if (selected_shipping_index == <%= iShippingLoop %>) {
			<%
			foreach (RepeaterItem ri in ((Repeater)riShipping.FindControl("rItemList")).Items)
			{
%>
			if (selected_product_index == <%= loopCount %>) {
				document.getElementById('<%= ((TextBox)ri.FindControl("tbProductId")).ClientID %>').value = product_id;
				document.getElementById('<%= ((TextBox)ri.FindControl("tbVariationId")).ClientID %>').value = variation_id;
		document.getElementById('<%= ((TextBox)ri.FindControl("tbProductName")).ClientID %>').value = product_name;
		document.getElementById('<%= ((TextBox)ri.FindControl("tbItemQuantity")).ClientID %>').value = 1;
		document.getElementById('<%= ((HiddenField)ri.FindControl("hfProductTaxRate")).ClientID %>').value = tax_rate;

				<% if (Constants.PRODUCT_SALE_OPTION_ENABLED){ %>
		document.getElementById('<%= ((TextBox)ri.FindControl("tbProductSaleId")).ClientID %>').value = sale_id;
		<% } %>
				<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
		document.getElementById('<%= ((CheckBox)ri.FindControl("cbFixedPurchaseProduct")).ClientID %>').checked = "";
		<% } %>
				<% if (this.OrderInput.IsSubscriptionBoxFixedAmount == false){ %>
		document.getElementById('<%= ((HiddenField)ri.FindControl("hfSupplierId")).ClientID %>').value = supplier_id;
		document.getElementById('<%= ((TextBox)ri.FindControl("tbProductPrice")).ClientID %>').value = product_price;
				<% } %>
		document.getElementById("<%= btnReCalculate.ClientID %>").click();
	}
	<%
				loopCount++;
			}
%>
	}
	<%
		iShippingLoop++;
	}
%>
	}
	//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<%if (this.IsPopUp == false) {%>
	<tr><td><h1 class="page-title">受注情報</h1></td></tr>
	<tr><td><img height="10" width="100" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
	<%} %>
	<!--▽ 登録 ▽-->
	<%if (this.IsPopUp) {%>
	<tr><td><h1 class="page-title">受注情報編集</h1></td></tr>
	<%} else {%>
	<tr><td><h1 class="cmn-hed-h2">受注情報編集</h1></td></tr>
	<%}%>
	<tr><td><h2 class ="notice"><asp:Literal ID="lbTopErrorMessages" runat="server"></asp:Literal></h2></td></tr>
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
													<table width="758" border="0" cellpadding="0" cellspacing="0">
														<tr>
															<td>
																<table class="info_table" cellspacing="1" cellpadding="3" width="480" border="0" runat="server" id="tbPaymentNoticeMessage" Visible="false">
																	<tbody>
																		<tr class="info_item_bg">
																			<td align="left">
																				<p><asp:Label ID="lbPaymentUserManagementLevelMessage" runat="server" ForeColor="red"></asp:Label></p>
																				<p><asp:Label ID="lbPaymentOrderOwnerKbnMessage" runat="server" ForeColor="red"></asp:Label></p>
																				<p><asp:Label ID="lbPaymentLimitedMessage" runat="server" ForeColor="red"></asp:Label></p>
																			</td>
																		</tr>
																	</tbody>
																</table>
															</td>
															<td align="right">
																<asp:Button ID="btnBackDetailTop" runat="server" Text="  詳細へ戻る  " onclick="btnBackDetail_Click" UseSubmitBehavior="false" />
																<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" OnClientClick="if(CheckStoreExist() == false) {return;} doPostbackEvenIfCardAuthFailed=false;" UseSubmitBehavior="false" />
																<div style="padding:5px 0px 0px 5px;" runat="server" visible="<%# Constants.PAYMENT_REAUTH_ENABLED && (this.IsUpdateShippingConvenience == false) %>">
																	<asp:Label id="lbOldExternalPayment" runat="server" CssClass="external_payment" />
																	<asp:DropDownList ID="ddlOldExecuteType" runat="server" Width="150" />&nbsp;
																	<asp:Label id="lbNewExternalPayment" runat="server" CssClass="external_payment" />
																	<asp:DropDownList ID="ddlNewExecuteType" runat="server" Width="150" />
																</div>
																<asp:Label ID="lbExternalPaymentAlertMessage" runat="server" ForeColor="red" />
															</td>
														</tr>
													</table>
												</div>
												<div style="<%# this.IsUpdateShippingConvenience ? "display:none" : string.Empty %>">
												<!-- ▽受注情報▽ -->
												<table width="758" border="0" cellspacing="0" cellpadding="0">
													<tr valign="top">
														<td>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="480" border="0">
																<tr id="trOrderErrorMessagesTitle" runat="server" visible="false">
																	<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
																</tr>
																<tr id="trOrderErrorMessages" runat="server" visible="false">
																	<td class="edit_item_bg" align="left" colspan="2">
																		<asp:Label ID="lbOrderErrorMessages" runat="server" ForeColor="red"></asp:Label>
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="40%">注文ID</td>
																	<td class="edit_item_bg" align="left" width="60%">
																		<%#: this.OrderInput.OrderId %></td>
																</tr>
																<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
																<tr>
																	<td class="edit_title_bg" align="left" width="40%">頒布会コースID</td>
																	<td class="edit_item_bg" align="left" width="60%">
																		<%# this.EncodedSubscriptionBoxCourseIdForDisplay %>
																	</td>
																</tr>
																<%} %>
																<%--▽ モール連携オプションまたは外部連携注文取込が有効の場合 ▽--%>
																<% if (Constants.MALLCOOPERATION_OPTION_ENABLED || Constants.URERU_AD_IMPORT_ENABLED) { %>
																<tr>
																	<td class="edit_title_bg" align="left">サイト</td>
																	<td class="edit_item_bg" align="left">
																		<%#: CreateSiteNameForDetail(StringUtility.ToEmpty(this.OrderInput.MallId), StringUtility.ToEmpty(this.OrderInput.MallName)) %></td>
																</tr>
																<% } %>
																<%--△ モール連携オプションまたは外部連携注文取込が有効の場合 △--%>																
																<tr>
																	<td class="edit_title_bg" align="left">注文区分</td>
																	<td class="edit_item_bg" align="left">
																		<asp:DropDownList ID="ddlOrderKbn" runat="server"></asp:DropDownList></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">注文ステータス</td>
																	<td class="edit_item_bg" align="left">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, this.OrderInput.OrderStatus) %></td>
																</tr>
																<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
																<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
																<tr>
																	<td class="edit_title_bg" align="left">引当状況</td>
																	<td class="edit_item_bg" align="left">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS, this.OrderInput.OrderStockreservedStatus) %></td>
																</tr>
																<% } %>
																<%--△ 実在庫利用が有効な場合は表示 △--%>
																<tr>
																	<td class="edit_title_bg" align="left">入金ステータス</td>
																	<td class="edit_item_bg" align="left">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, this.OrderInput.OrderPaymentStatus) %></td>
																</tr>
																<tr id="trDemandStatus" Visible="false" runat="server">
																	<td class="edit_title_bg" align="left">督促ステータス</td>
																	<td class="edit_item_bg" align="left">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_DEMAND_STATUS, this.OrderInput.DemandStatus)  %></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">外部決済ステータス</td>
																	<td class="detail_item_bg" align="left">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, this.OrderInput.ExternalPaymentStatus) %>
																	</td>
																</tr>
																<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
																	<tr>
																		<td class="detail_title_bg" align="left">与信状況</td>
																		<td class="detail_item_bg" align="left">
																			<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, this.OrderInput.ExternalPaymentStatus) %>
																		</td>
																	</tr>
																<% } %>
																<tr runat="server" visible="<%# this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_INV_ERROR %>">
																	<td class="detail_title_bg" align="left">外部決済エラーメッセージ</td>
																	<td class="detail_item_bg" align="left">
																		<%#: this.OrderInput.ExternalPaymentErrorMessage %></td>
																</tr>
																<asp:Repeater ID="rOrderExtendStatusList" runat="server" ItemType="System.Data.DataRowView">
																<ItemTemplate>
																<tr>
																	<td class="edit_title_bg" align="left">拡張ステータス<%#: Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO] %>：<br />
																		&nbsp;<%#: Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME] %></td>
																	<td class="edit_item_bg" align="left">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, OrderPage.FIELD_ORDER_EXTEND_STATUS, this.OrderInput.ExtendStatus[(int)Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO] - 1].Value) %></td>
																</tr>
																</ItemTemplate>
																</asp:Repeater>
																<% if (Constants.URERU_AD_IMPORT_ENABLED) { %>
																<tr>
																	<td class="edit_title_bg" align="left">外部連携受注ID</td>
																	<td class="edit_item_bg" align="left"><asp:TextBox ID="tbExternalOrderId" MaxLength="50" Width="95%" runat="server"></asp:TextBox></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">外部連携取込ステータス</td>
																	<td class="edit_item_bg" align="left"><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS, this.OrderInput.ExternalImportStatus) %></td>
																</tr>
																<%} %>
																<tr>
																	<td class="edit_title_bg" align="left">配送種別</td>
																	<td class="edit_item_bg" align="left" runat="server" visible="<%# (this.CanSelectShipping == false) %>">
																		[<%#: this.OrderInput.ShippingId %>]
																		<%#: (this.ShopShipping != null) ? this.ShopShipping.ShopShippingName : string.Empty %></td>
																	<td class="edit_item_bg" align="left" runat="server" visible="<%# this.CanSelectShipping %>">
																		<asp:DropDownList ID="ddlShipping" Width="100%" OnSelectedIndexChanged="ddlShipping_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">出荷後変更区分</td>
																	<td class="edit_item_bg" align="left">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN, this.OrderInput.ShippedChangedKbn) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">リモートIPアドレス</td>
																	<td class="edit_item_bg" align="left">
																		<%#: this.OrderInput.RemoteAddr %></td>
																</tr>
															</table>
															<%--▽ 返品交換情報 ▽--%>
															<br />
															<table class="edit_table" cellspacing="1" cellpadding="3" width="480" border="0">
																<tr>
																	<td class="edit_title_bg" align="center" colspan="2">返品交換情報</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="40%">元注文ID</td>
																	<td class="edit_item_bg" align="left" width="60%">
																		<%#: this.OrderInput.OrderIdOrg  %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">返品交換ステータス</td>
																	<td class="edit_item_bg" align="left">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS, this.OrderInput.OrderReturnExchangeStatus) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">返金ステータス</td>
																	<td class="edit_item_bg" align="left">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS, this.OrderInput.OrderRepaymentStatus) %></td>
																</tr>				
																<tr>
																	<td class="edit_title_bg" align="left">返品交換区分</td>
																	<td class="edit_item_bg" align="left">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN, this.OrderInput.ReturnExchangeKbn) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">返品交換都合区分</td>
																	<td class="edit_item_bg" align="left">
																		<asp:RadioButtonList ID="rblReturnExchangeReasonKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" DataTextField="Text" DataValueField="Value"></asp:RadioButtonList></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">返品交換理由メモ</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox id="tbReturnExchangeReasonMemo" Text='<%# this.OrderInput.ReturnExchangeReasonMemo %>' runat="server" TextMode="MultiLine" Rows="4" Width="95%"></asp:TextBox></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">返金額</td>
																	<td class="edit_item_bg" align="left">
																		<%#: this.OrderInput.OrderPriceRepayment.ToPriceString(true) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">返金メモ</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox id="tbRepaymentMemo" Text='<%# this.OrderInput.RepaymentMemo %>' runat="server" TextMode="MultiLine" Rows="3" Width="95%"></asp:TextBox></td>
																</tr>
															</table>
															<%--△ 返品交換情報 △--%>
														</td>
														<td width="20"></td>
														<td>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="258" border="0">
																<tr>
																	<td class="edit_title_bg" align="center" colspan="2">更新日</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="40%">注文日時</td>
																	<td class="edit_item_bg" align="left" width="60%">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">受注承認日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderRecognitionDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
																<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
																<tr>
																	<td class="edit_title_bg" align="left">在庫引当日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderStockreservedDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<% } %>
																<%--△ 実在庫利用が有効な場合は表示 △--%>
																<tr>
																	<td class="edit_title_bg" align="left">出荷手配日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderShippingDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">出荷完了日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderShippedDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<%if (Constants.REALSHOP_OPTION_ENABLED && Constants.STORE_PICKUP_OPTION_ENABLED){ %>
																<tr>
																	<td class="edit_title_bg" align="left">店舗到着日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.StorePickupStoreArrivedDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">返送日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.StorePickupReturnDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">引渡し完了日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.StorePickupDeliveredCompleteDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<% } %>
																<tr>
																	<td class="edit_title_bg" align="left">配送完了日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderDeliveringDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">キャンセル日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderCancelDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">入金日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderPaymentDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<tr id="trDemandDay" visible="false" runat="server">
																	<td class="edit_title_bg" align="left">督促日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.DemandDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">外部決済与信日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.ExternalPaymentAuthDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																</tr>
																<asp:Repeater ID="rOrderExtendStatusDates" runat="server" ItemType="System.Data.DataRowView">
																<ItemTemplate>
																	<tr>
																		<td class="edit_title_bg" align="left">拡張ステータス<%#: Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO] %>更新日</td>
																		<td class="edit_item_bg" align="left">
																			<%#: DateTimeUtility.ToStringForManager(this.OrderInput.ExtendStatus[(int)Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO]-1].Date, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																	</tr>
																</ItemTemplate>
																</asp:Repeater>
															</table>
															<%--▽ 返品交換更新日 ▽--%>
															<br />
															<table class="edit_table" cellspacing="1" cellpadding="3" width="258" border="0">
																<tr>
																	<td class="edit_title_bg" align="center" colspan="2">返品交換更新日</td>
																</tr>				
																<tr>
																	<td class="edit_title_bg" align="left" width="40%">返品交換受付日</td>
																	<td class="edit_item_bg" align="left" width="60%">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderReturnExchangeReceiptDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">返品交換商品到着日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderReturnExchangeArrivalDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">返品交換完了日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderReturnExchangeCompleteDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">返金日</td>
																	<td class="edit_item_bg" align="left">
																		<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderRepaymentDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
															</table>
															<%--△ 返品交換更新日 △--%>
														</td>
													</tr>
												</table>
												<!-- △受注情報△ -->
												<br />
												<!-- ▽注文者情報▽ -->
												<table class="edit_table owner-input" cellspacing="1" cellpadding="6" width="758" border="0">
													<tbody>
														<tr id="trOrderOwnerErrorMessagesTitle" runat="server" visible="false">
															<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
														</tr>
														<tr id="trOrderOwnerErrorMessages" runat="server" visible="false">
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:Label ID="lbOrderOwnerErrorMessages" runat="server" ForeColor="red"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">注文者情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">ユーザーID</td>
															<td class="edit_item_bg" align="left" width="25%">
																<%#: this.OrderInput.UserId %></td>
															<td class="edit_title_bg" align="left" width="25%">注文者区分</td>
															<td class="edit_item_bg" align="left" width="25%">
																<%#: ValueText.GetValueText(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_KBN, this.OrderInput.Owner.OwnerKbn) %></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name.name@@") %></td>
															<td class="edit_item_bg" align="left" width="25%" colspan="<%= this.IsOwnerAddrJp ? 1 : 3 %>">姓：<asp:TextBox id="tbOwnerName1" Text='<%# this.OrderInput.Owner.OwnerName1 %>' runat="server" Width="70" MaxLength="10"></asp:TextBox> 名：<asp:TextBox id="tbOwnerName2" Text='<%# this.OrderInput.Owner.OwnerName2 %>' runat="server" Width="70" MaxLength="10"></asp:TextBox></td>
															<% if (this.IsOwnerAddrJp) { %>
															<td class="edit_title_bg" align="left" width="25%" ><%: ReplaceTag("@@User.name_kana.name@@") %></td>
															<td class="edit_item_bg" width="25%">姓：<asp:TextBox id="tbOwnerNameKana1" Text='<%# this.OrderInput.Owner.OwnerNameKana1 %>' runat="server" Width="70" MaxLength="20"></asp:TextBox> 名：<asp:TextBox id="tbOwnerNameKana2" Text='<%# this.OrderInput.Owner.OwnerNameKana2 %>' runat="server" Width="70" MaxLength="20"></asp:TextBox></td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">メールアドレス</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwnerMailAddr" Text='<%# this.OrderInput.Owner.OwnerMailAddr %>' runat="server" Width="300" MaxLength="256"></asp:TextBox></td>
														</tr>
														<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">モバイルメールアドレス</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwnerMailAddr2" Text='<%# this.OrderInput.Owner.OwnerMailAddr2 %>' runat="server" Width="300" MaxLength="256"></asp:TextBox></td>
														</tr>
														<% } %>
														<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">
																<%: ReplaceTag("@@User.country.name@@", this.OwnerAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:DropDownList id="ddlOwnerCountry" runat="server" AutoPostBack="true"></asp:DropDownList>
															</td>
														</tr>
														<% } %>
														<% if (this.IsOwnerAddrJp) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.zip.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwnerZip1_1" Text='<%# this.OrderInput.Owner.OwnerZip1 %>' runat="server" MaxLength="3" Width="50"></asp:TextBox>-&nbsp;
																<asp:TextBox id="tbOwnerZip1_2" Text='<%# this.OrderInput.Owner.OwnerZip2 %>' runat="server" MaxLength="4" Width="70"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.addr1.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:DropDownList id="ddlOwnerAddr1" runat="server"></asp:DropDownList>
															</td>
														</tr>
														<% } %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">
																<%: ReplaceTag("@@User.addr2.name@@", this.OwnerAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwnerAddr2" Text='<%# this.OrderInput.Owner.OwnerAddr2 %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">
																<%: ReplaceTag("@@User.addr3.name@@", this.OwnerAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwnerAddr3" Text='<%# this.OrderInput.Owner.OwnerAddr3 %>' runat="server" MaxLength="50" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr <%= (Constants.DISPLAY_ADDR4_ENABLED || (this.IsOwnerAddrJp == false)) ? "" : "style=\"display:none;\""  %>>
															<td class="edit_title_bg" align="left" width="25%">
																<%: ReplaceTag("@@User.addr4.name@@", this.OwnerAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwnerAddr4" Text='<%# this.OrderInput.Owner.OwnerAddr4 %>' runat="server" MaxLength="50" Width="300"></asp:TextBox>
															</td>
														</tr>
														<% if (this.IsOwnerAddrJp == false){ %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.addr5.name@@", this.OwnerAddrCountryIsoCode) %></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<% if (this.IsOwnerAddrUs) { %>
																<asp:DropDownList runat="server" ID="ddlOwnerAddr5"></asp:DropDownList>
																<% } else { %>
																<asp:TextBox id="tbOwnerAddr5" Text='<%# this.OrderInput.Owner.OwnerAddr5 %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
																<% } %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.zip.name@@", this.OwnerAddrCountryIsoCode) %></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwnerZipGlobal" Text='<%# this.OrderInput.Owner.OwnerZip %>' runat="server" MaxLength="20" Width="150" />
																<asp:LinkButton
																	ID="lbSearchAddrFromOwnerZipGlobal"
																	OnClick="lbSearchAddrFromOwnerZipGlobal_Click"
																	Style="display:none;"
																	runat="server" />
															</td>
														</tr>
														<% } %>
														<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.company_name.name@@")%></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwneCompanyName" Text='<%# this.OrderInput.Owner.OwnerCompanyName %>' runat="server" MaxLength="50" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.company_post_name.name@@")%></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwnerCompanyPostName" Text='<%# this.OrderInput.Owner.OwnerCompanyPostName %>' runat="server" MaxLength="50" Width="300"></asp:TextBox>
															</td>
														</tr>
														<%} %>
														<tr>
															<% if (this.IsOwnerAddrJp) { %>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.tel1.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwnerTel1_1" Text='<%# this.OrderInput.Owner.OwnerTel1_1 %>' runat="server" MaxLength="6" Width="40"></asp:TextBox>-
																<asp:TextBox id="tbOwnerTel1_2" Text='<%# this.OrderInput.Owner.OwnerTel1_2 %>' runat="server" MaxLength="4" Width="40"></asp:TextBox>-
																<asp:TextBox id="tbOwnerTel1_3" Text='<%# this.OrderInput.Owner.OwnerTel1_3 %>' runat="server" MaxLength="4" Width="40"></asp:TextBox>
																<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
																	<%#: WebMessages.GetMessages(WebMessages.ERRMSG_INPUT_GMO_KB_MOBILE_PHONE)%>
																<% } %>
															</td>
															<% } else { %>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.tel1.name@@", this.OwnerAddrCountryIsoCode) %></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOwnerTel1Global" Text='<%# this.OrderInput.Owner.OwnerTel1 %>' runat="server" MaxLength="30" Width="150"></asp:TextBox>
															</td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.sex.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:RadioButtonList ID="rblOwnerSex" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.birth.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<uc:DateTimeInput ID="ucOwnerBirth" runat="server" YearList="<%# DateTimeUtility.GetBirthYearListItem() %>" HasTime="False" HasBlankSign="False" HasBlankValue="True" />
															</td>
														</tr>
														<% if (Constants.GLOBAL_OPTION_ENABLE){ %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">アクセス国ISOコード</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:DropDownList id="ddlAccessCountryIsoCode" runat="server"></asp:DropDownList>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">表示言語ロケールID</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:DropDownList id="ddlDispLanguageLocaleId" runat="server" OnSelectedIndexChanged="ddlDispLanguageLocaleId_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
																言語コード( <asp:Literal ID="lLanguageCode" runat="server"></asp:Literal> )
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">表示通貨ロケールID</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:DropDownList id="ddlDispCurrencyLocaleId" runat="server" OnSelectedIndexChanged="ddlDispCurrencyLocaleId_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
																通貨コード( <asp:Literal ID="lCurrencyCode" runat="server"></asp:Literal> )
															</td>
														</tr>
														<% } %>
													</tbody>
												</table>
												<!-- △注文者情報△ -->
												<br />
												<!-- ▽ユーザー情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="6" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">ユーザー情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">ユーザー特記欄</td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbUserMemo" runat="server" TextMode="MultiLine" Rows="2" Width="555"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">ユーザー管理レベル</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList id="ddlUserManagementLevel" OnSelectedIndexChanged="ddlUserManagementLevel_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
															</td>
														</tr>
													</tbody>
													<tbody id="tbdySelectPaymentUserManagementLevelMessage" runat="server" visible="false">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">注意喚起</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:Label ID="lbSelectPaymentUserManagementLevelMessage" runat="server" ForeColor="red"></asp:Label>
														</td>
													</tr>
													</tbody>
												</table>
												<!-- △ユーザー情報△ -->
												<!-- ▽領収書情報▽ -->
												<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody id="tbdyReceiptErrorMessages" runat="server" visible="false">
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="2">
																<asp:Label ID="lbReceiptErrorMessages" runat="server" ForeColor="red"></asp:Label>
															</td>
														</tr>
													</tbody>
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">領収書情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">領収書希望</td>
															<td class="edit_item_bg" align="left" width="75%">
																<asp:RadioButtonList id="rblReceiptFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" OnSelectedIndexChanged="rblReceiptFlg_SelectedIndexChanged" AutoPostBack="True"/>
															</td>
														</tr>
													<tr>
															<td class="edit_title_bg" align="left">領収書出力</td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButtonList id="rblReceiptOutputFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"  Enabled="<%# rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON %>"/>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">宛名
																<% if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON){ %><span class="notice">*</span><% } %>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbReceiptAddress" Text="<%# this.OrderInput.ReceiptAddress %>" Width="600" runat="server" MaxLength="100" Enabled="<%# rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON %>"/>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">但し書き
																<% if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON){ %><span class="notice">*</span><% } %>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbReceiptProviso" Text="<%# this.OrderInput.ReceiptProviso %>" Width="600" runat="server" MaxLength="100" Enabled="<%# rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON %>"/>
															</td>
														</tr>
													</tbody>
												</table>
												<% } %>
												</div>
												<!-- △領収書情報△ -->
												<br />
												<asp:HiddenField ID="hfSelectedShopId" Value="<%# this.OrderInput.Shippings[0].ShippingReceivingStoreId %>" runat="server" />
												<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# this.OrderInput.Shippings[0].ShippingReceivingStoreId %>" />
												<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# this.OrderInput.Shippings[0].ShippingName %>" />
												<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# this.OrderInput.Shippings[0].ShippingAddr4 %>" />
												<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# this.OrderInput.Shippings[0].ShippingTel1 %>" />
												<asp:Repeater ID="rShippingList" DataSource="<%# this.OrderInput.Shippings %>" OnItemCommand="rShippingList_ItemCommand" runat="server" ItemType="OrderShippingInput">
												<ItemTemplate>
												<%-- ▽配送情報▽ --%>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<% if ((Constants.GIFTORDER_OPTION_ENABLED) && (this.OrderInput.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)) { %>
														<tr>
															<td class="edit_title_bg" align="center" colspan="<%# this.CanDeleteShipping ? 3 : 4 %>">【　配送情報<%#: Item.OrderShippingNo %>　】</td>
															<td class="edit_title_bg" align="center" visible="<%# this.CanDeleteShipping %>" runat="server">
																<asp:Button ID="btnDeleteShipping" Text="　削除　" CommandName="delete_shipping" CommandArgument="<%# Item.OrderShippingNo %>" runat="server" UseSubmitBehavior="false" />
															</td>
														</tr>
														<% } %>
														<tr id="trOrderShippingErrorMessagesTitle" runat="server" visible="false">
															<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
														</tr>
														<tr id="trOrderShippingErrorMessages" runat="server" visible="false">
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:Label ID="lbOrderShippingErrorMessages" runat="server" ForeColor="red"></asp:Label>
															</td>
														</tr>
														<% if ((Constants.GIFTORDER_OPTION_ENABLED) && (this.OrderInput.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)) { %>
														<%-- ▽送り主情報（ギフト注文の場合のみ表示）▽--%>
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">送り主情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name.name@@") %></td>
															<td class="edit_item_bg" align="left">
																姓：<asp:TextBox id="tbSenderName1" Text='<%# Item.SenderName1 %>' runat="server" MaxLength="10" Width="70"></asp:TextBox>名：<asp:TextBox id="tbSenderName2" Text='<%# Item.SenderName2 %>' runat="server" MaxLength="10" Width="70"></asp:TextBox></td>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
															<td class="edit_item_bg" align="left" width="25%">
																<div <%# Item.IsSenderAddrJp ? "" : "style=\"display:none\"" %>>
																	姓：<asp:TextBox id="tbSenderNameKana1" Text='<%# Item.SenderNameKana1 %>' runat="server" MaxLength="20" Width="70"></asp:TextBox>名：<asp:TextBox id="tbSenderNameKana2" Text='<%# Item.SenderNameKana2 %>' runat="server" MaxLength="20" Width="70"></asp:TextBox>
																</div>
															</td>
														</tr>
														<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.country.name@@", Item.SenderCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">	
																<asp:DropDownList id="ddlSenderCountry" DataSource="<%# this.UserCountryDisplayList %>" SelectedValue="<%#: Constants.GLOBAL_OPTION_ENABLE ? Item.SenderCountryName : string.Empty %>" runat="server"
																	 OnSelectedIndexChanged="ddlShippingCountrySenderCountry_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
															</td>
														</tr>
														<% } %>
														<tr visible='<%# (Item.IsSenderAddrJp) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.zip.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbSenderZip_1" Text='<%# Item.SenderZip1 %>' runat="server" MaxLength="3" Width="50"></asp:TextBox>-&nbsp;
																<asp:TextBox id="tbSenderZip_2" Text='<%# Item.SenderZip2 %>' runat="server" MaxLength="4" Width="70"></asp:TextBox>
															</td>
														</tr>
														<tr visible='<%# (Item.IsSenderAddrJp) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.addr1.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3">	
																<asp:DropDownList id="ddlSenderAddr1" DataSource="<%# this.PrefectureList %>" SelectedValue="<%# Item.SenderAddr1 %>" runat="server"></asp:DropDownList>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.addr2.name@@", Item.SenderCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbSenderAddr2" Text='<%# Item.SenderAddr2 %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.addr3.name@@", Item.SenderCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbSenderAddr3" Text='<%# Item.SenderAddr3 %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr <%# (Constants.DISPLAY_ADDR4_ENABLED || (Item.IsSenderAddrJp == false)) ? "" : "style=\"display:none;\""  %>>
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.addr4.name@@", Item.SenderCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbSenderAddr4" Text='<%# Item.SenderAddr4 %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr visible='<%# (Item.IsSenderAddrJp == false) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.addr5.name@@", Item.SenderCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3" visible='<%# Item.IsSenderAddrUs %>' runat="server">
																
																<asp:DropDownList ID="ddlSenderAddr5" runat="server" DataSource="<%# this.UsStateList %>"></asp:DropDownList>
															</td>
															<td class="edit_item_bg" align="left" colspan="3" visible='<%# (Item.IsSenderAddrUs == false) %>' runat="server">
																<asp:TextBox id="tbSenderAddr5" Text='<%# Item.SenderAddr5 %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr visible='<%# (Item.IsSenderAddrJp == false) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.zip.name@@", Item.SenderCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbSenderZipGlobal" Text='<%# Item.SenderZip %>' runat="server" MaxLength="20" Width="150" />
																<asp:LinkButton
																	ID="lbSearchAddrFromSenderZipGlobal"
																	OnClick="lbSearchAddrFromSenderZipGlobal_Click"
																	Style="display:none;"
																	runat="server" />
															</td>
														</tr>
														<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
														<tr>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.company_name.name@@")%></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbSenderCompanyName" Text='<%# Item.SenderCompanyName %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.company_post_name.name@@")%></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbSenderCompanyPostName" Text='<%# Item.SenderCompanyPostName %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<%} %>
														<tr visible='<%# (Item.IsSenderAddrJp) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.tel1.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3" width="25%">
																<asp:TextBox id="tbSenderTel1_1" Text='<%# Item.SenderTel1_1 %>' runat="server" MaxLength="6" Width="40"></asp:TextBox>-
																<asp:TextBox id="tbSenderTel1_2" Text='<%# Item.SenderTel1_2 %>' runat="server" MaxLength="4" Width="40"></asp:TextBox>-
																<asp:TextBox id="tbSenderTel1_3" Text='<%# Item.SenderTel1_3 %>' runat="server" MaxLength="4" Width="40"></asp:TextBox>
															</td>
														</tr>
														<tr visible='<%# (Item.IsSenderAddrJp == false) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.tel1.name@@", Item.SenderCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3" width="25%">
																<asp:TextBox id="tbSenderTel1Global" Text='<%# Item.SenderTel1 %>' runat="server" MaxLength="30" Width="150"></asp:TextBox>
															</td>
														</tr>
														<%-- △送り主情報（ギフト注文の場合のみ表示）△--%>
														<% } %>
														<%-- ▽配送先情報▽ --%>
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">配送先情報</td>
														</tr>
														<tr>
															<td class="edit_item_bg" colspan="4">
																<asp:DropDownList
																	ID="ddlUserShipping"
																	CssClass="UserShipping"
																	SelectedValue='<%# ((Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) && CheckItemRelateWithServiceConvenienceStore(ShopShipping))
																		? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE
																		: (string.IsNullOrEmpty(Item.StorePickupRealShopId) == false)
																			&& this.CanUseStorePickup
																				? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP
																				: CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW %>'
																	DataTextField="Text"
																	DataValueField="Value"
																	runat="server"
																	DataSource="<%# GetItemShippingSelect() %>"
																	OnSelectedIndexChanged="ddlUserShipping_SelectedIndexChanged"
																	AutoPostBack="true"></asp:DropDownList>
																<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
																<asp:DropDownList
																	ID="ddlShippingReceivingStoreType" 
																	SelectedValue='<%# (string.IsNullOrEmpty(Item.ShippingReceivingStoreType)) ? Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART : Item.ShippingReceivingStoreType %>'
																	CssClass="ShippingReceivingStoreType"
																	runat="server"
																	OnSelectedIndexChanged="ddlShippingReceivingStoreType_SelectedIndexChanged"
																	DataTextField="Text"
																	DataValueField="Value"
																	DataSource="<%# GetItemShippingECPaySelect() %>"
																	AutoPostBack="true" />
																<asp:Button CssClass="CvsSearch" ID="btnOpenConvenienceStoreMapEcPay" runat="server" OnClick="btnOpenConvenienceStoreMapEcPay_Click" Text="  検索  "/>
																<% } else { %>
																	<input class="CvsSearch" onclick="javascript:openConvenienceStoreMapPopup();" type="button" value="検索" />
																<% } %>
																<div id="dvErrorShippingConvenience" style="display:none;">
																	<span style="color:red;"><%= WebMessages.GetMessages(WebMessages.ERRMSG_CONVENIENCE_STORE_NOT_VALID) %></span>
																</div>
															</td>
														</tr>
														<% if (this.IsStorePickup) { %>
															<tr>
																<td class="edit_title_bg">
																	受取店舗
																</td>
																<td class="edit_item_bg" colspan="3">
																	<asp:DropDownList id="ddlRealStore"
																		DataSource="<%# this.RealShopDataSource %>"
																		DataTextField="Text"
																		DataValueField="Value"
																		SelectedValue='<%# this.RealShopDataSource.Any(rs => rs.Value == Item.StorePickupRealShopId) ? Item.StorePickupRealShopId : string.Empty %>'
																		AutoPostBack="true"
																		OnSelectedIndexChanged="ddlRealStore_SelectedIndexChanged"
																		Width="150"
																		runat="server" />
																</td>
															</tr>
															<tr>
																<td class="edit_title_bg">
																	店舗住所
																</td>
																<td class="edit_item_bg" colspan="3">
																	<asp:Label ID="lbStoreAddress" Text='<%# this.GetRealShopValue(Item.StorePickupRealShopId, "Address") %>' runat="server" />
																</td>
															</tr>
															<tr>
																<td class="edit_title_bg">
																	営業時間
																</td>
																<td class="edit_item_bg" colspan="3">
																	<asp:Label ID="lbStoreOpeningHours" Text='<%# this.GetRealShopValue(Item.StorePickupRealShopId, "OpeningHours") %>' runat="server" />
																</td>
															</tr>
															<tr>
																<td class="edit_title_bg">
																	店舗電話番号
																</td>
																<td class="edit_item_bg" colspan="3">
																	<asp:Label ID="lbStoreTel" Text='<%# this.GetRealShopValue(Item.StorePickupRealShopId, "Tel") %>' runat="server" />
																</td>
															</tr>
														<% } %>
														<tbody style="<%# (Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF && string.IsNullOrEmpty(Item.StorePickupRealShopId)) ? string.Empty : "display:none;" %>" class="ShippingAddress INPUT_NEW">
														<tr>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name.name@@") %></td>
															<td class="edit_item_bg" align="left" width="25%">
																姓：<asp:TextBox id="tbShippingName1" Text='<%# Item.ShippingName1 %>' runat="server" MaxLength="10" Width="70"></asp:TextBox>名：<asp:TextBox id="tbShippingName2" Text='<%# Item.ShippingName2 %>' runat="server" MaxLength="10" Width="70"></asp:TextBox></td>
															<td class="edit_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
															<td class="edit_item_bg" align="left" width="25%">
																<div <%# Item.IsShippingAddrJp ? "" : "style=\"display:none\"" %>>
																	姓：<asp:TextBox id="tbShippingNameKana1" Text='<%# Item.ShippingNameKana1 %>' runat="server" MaxLength="20" Width="70"></asp:TextBox>名：<asp:TextBox id="tbShippingNameKana2" Text='<%# Item.ShippingNameKana2 %>' runat="server" MaxLength="20" Width="70"></asp:TextBox>
																</div>
															</td>
														</tr>
														<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.country.name@@", Item.ShippingCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">	
																<asp:DropDownList id="ddlShippingCountry" DataSource="<%# this.ShippingAvailableCountryDisplayList %>" SelectedValue ="<%#: Constants.GLOBAL_OPTION_ENABLE ? Item.ShippingCountryName : string.Empty %>" runat="server" 
																	OnSelectedIndexChanged="ddlShippingCountrySenderCountry_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
															</td>
														</tr>
														<% } %>
														<tr visible='<%# (Item.IsShippingAddrJp) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.zip.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbShippingZip1_1" Text='<%# Item.ShippingZip1 %>' runat="server" MaxLength="3" Width="50"></asp:TextBox>-&nbsp;
																<asp:TextBox id="tbShippingZip1_2" Text='<%# Item.ShippingZip2 %>' runat="server" MaxLength="4" Width="70"></asp:TextBox>
															</td>
														</tr>
														<tr visible='<%# (Item.IsShippingAddrJp) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.addr1.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3">	
																<asp:DropDownList id="ddlShippingAddr1" DataSource="<%# this.PrefectureList %>" SelectedValue="<%# Item.ShippingAddr1 %>" runat="server"></asp:DropDownList>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.addr2.name@@", Item.ShippingCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbShippingAddr2" Text='<%# Item.ShippingAddr2 %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>														
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.addr3.name@@", Item.ShippingCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbShippingAddr3" Text='<%# Item.ShippingAddr3 %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>														
														<tr <%# (Constants.DISPLAY_ADDR4_ENABLED || (Item.IsShippingAddrJp == false)) ? "" : "style=\"display:none;\""  %>>
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.addr4.name@@", Item.ShippingCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbShippingAddr4" Text='<%# Item.ShippingAddr4 %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr visible='<%# (Item.IsShippingAddrJp == false) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.addr5.name@@", Item.ShippingCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3" 
																visible='<%# Item.IsShippingAddrUs %>' runat="server">
																<asp:DropDownList ID="ddlShippingAddr5" runat="server" DataSource="<%# this.UsStateList %>" ></asp:DropDownList>
															</td>
															<td class="edit_item_bg" align="left" colspan="3"
																visible='<%# (Item.IsShippingAddrUs == false) %>' runat="server">
																<asp:TextBox id="tbShippingAddr5" Text='<%# Item.ShippingAddr5 %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr visible='<%# (Item.IsShippingAddrJp == false) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.zip.name@@", Item.ShippingCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbShippingZipGlobal" Text='<%# Item.ShippingZip %>' runat="server" MaxLength="20" Width="150" />
																<asp:LinkButton
																	ID="lbSearchAddrFromShippingZipGlobal"
																	OnClick="lbSearchAddrFromShippingZipGlobal_Click"
																	Style="display:none;"
																	runat="server" />
															</td>
														</tr>
														<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
														<tr>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.company_name.name@@")%></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbShippingCompanyName" Text='<%# Item.ShippingCompanyName %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.company_post_name.name@@")%></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbShippingCompanyPostName" Text='<%# Item.ShippingCompanyPostName %>' runat="server" MaxLength="40" Width="300"></asp:TextBox>
															</td>
														</tr>
														<%} %>
														<tr visible='<%# (Item.IsShippingAddrJp) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.tel1.name@@") %></td>
															<td class="edit_item_bg" align="left" colspan="3" width="25%">
																<asp:TextBox id="tbShippingTel1_1" Text='<%# Item.ShippingTel1_1 %>' runat="server" MaxLength="6" Width="40"></asp:TextBox>-
																<asp:TextBox id="tbShippingTel1_2" Text='<%# Item.ShippingTel1_2 %>' runat="server" MaxLength="4" Width="40"></asp:TextBox>-
																<asp:TextBox id="tbShippingTel1_3" Text='<%# Item.ShippingTel1_3 %>' runat="server" MaxLength="4" Width="40"></asp:TextBox>
																<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
																	<%#: WebMessages.GetMessages(WebMessages.ERRMSG_INPUT_GMO_KB_MOBILE_PHONE)%>
																<% } %>
															</td>
														</tr>
														<tr visible='<%# (Item.IsShippingAddrJp == false) %>' runat="server">
															<td class="edit_title_bg" align="left" width="120">
																<%#: ReplaceTag("@@User.tel1.name@@", Item.ShippingCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3" width="25%">
																<asp:TextBox id="tbShippingTel1Global" Text='<%# Item.ShippingTel1 %>' runat="server" MaxLength="30" Width="150"></asp:TextBox>
															</td>
														</tr>
														</tbody>
														<tbody style="<%# (Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) ? string.Empty : "display:none;" %>" class="ShippingAddress CONVENIENCE">
															<tr style="height:45px;">
																<td class="edit_title_bg" align="left" width="25%">店舗ID</td>
																<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopId">
																	<span>
																		<asp:Literal ID="lCvsShopId" Text="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON ? Item.ShippingReceivingStoreId : string.Empty %>" runat="server" />
																	</span>
																</td>
															</tr>
															<tr style="height:45px;">
																<td class="edit_title_bg" align="left" width="25%">店舗名称</td>
																<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopName">
																	<span>
																		<asp:Literal ID="lCvsShopName" Text="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON ? Item.ShippingName : string.Empty %>" runat="server" />
																	</span>
																</td>
															</tr>
															<tr style="height:45px;">
																<td class="edit_title_bg" align="left" width="25%">店舗住所</td>
																<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopAddress">
																	<span>
																		<asp:Literal ID="lCvsShopAddress" Text="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON ? Item.ShippingAddr4 : string.Empty %>" runat="server" />
																	</span>
																</td>
															</tr>
															<tr style="height:45px;">
																<td class="edit_title_bg" align="left" width="25%">店舗電話番号</td>
																<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopTel">
																	<span>
																		<asp:Literal ID="lCvsShopTel" Text="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON ? Item.ShippingTel1 : string.Empty %>" runat="server" />
																	</span>
																</td>
															</tr>
														</tbody>
														<tbody class="ShippingAddress OWNER">
															<tr>
																<td class="edit_title_bg" align="left" width="20%">
																	<%: ReplaceTag("@@User.name.name@@") %>
																</td>
																<td id="tdShippingName" class="edit_item_bg" align="left" colspan="3" visible="<%# IsCountryJp(this.OwnerAddrCountryIsoCode) %>" runat="server">
																	<%#: this.OrderInput.Owner.OwnerName %> 
																		（<%#: this.OrderInput.Owner.OwnerNameKana %>）
																</td>
																<td id="tdShippingNameGlobal" class="edit_item_bg" align="left" colspan="3" visible="<%# (IsCountryJp(this.OwnerAddrCountryIsoCode) == false) %>" runat="server">
																	<%#: this.OrderInput.Owner.OwnerName %>
																</td>
															</tr>
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<tr>
																<td class="edit_title_bg" align="left">
																	<%#: ReplaceTag("@@User.country.name@@", this.OwnerAddrCountryIsoCode) %>
																</td>
																<td id="tdShippingCountryName" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerAddrCountryName %>
																</td>
															</tr>
															<% } %>
															<tr visible="<%# IsCountryJp(this.OwnerAddrCountryIsoCode) %>" runat="server">
																<td class="edit_title_bg" align="left" width="20%">
																	<%: ReplaceTag("@@User.zip.name@@") %>
																</td>
																<td id="tdShippingZip" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerZip %>
																</td>
															</tr>
															<tr visible="<%# IsCountryJp(this.OwnerAddrCountryIsoCode) %>" runat="server">
																<td class="edit_title_bg" align="left" width="20%">
																	<%: ReplaceTag("@@User.addr1.name@@") %>
																</td>
																<td id="tdShippingAddr1" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerAddr1 %>
																</td>
															</tr>
															<tr>
																<td class="edit_title_bg" align="left" width="20%">
																	<%#: ReplaceTag("@@User.addr2.name@@", this.OwnerAddrCountryIsoCode) %>
																</td>
																<td id="tdShippingAddr2" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerAddr2 %>
																</td>
															</tr>
															<tr>
																<td class="edit_title_bg" align="left" width="20%" nowrap>
																	<%#: ReplaceTag("@@User.addr3.name@@", this.OwnerAddrCountryIsoCode) %>
																</td>
																<td id="tdShippingAddr3" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerAddr3 %>
																</td>
															</tr>
															<tr <%= Constants.DISPLAY_ADDR4_ENABLED ? "" : "style=\"display:none;\""  %>>
																<td class="edit_title_bg" align="left" width="20%" nowrap>
																	<%#: ReplaceTag("@@User.addr4.name@@", this.OwnerAddrCountryIsoCode) %>
																</td>
																<td id="tdShippingAddr4" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerAddr4 %>
																</td>
															</tr>
															<tr visible="<%# (IsCountryJp(this.OwnerAddrCountryIsoCode) == false) %>" runat="server">
																<td class="edit_title_bg" align="left">
																	<%#: ReplaceTag("@@User.addr5.name@@", this.OwnerAddrCountryIsoCode) %>
																</td>
																<td id="tdShippingAddr5" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerAddr5 %>
																</td>
															</tr>
															<tr visible="<%# (IsCountryJp(this.OwnerAddrCountryIsoCode) == false) %>" runat="server">
																<td class="edit_title_bg" align="left">
																	<%#: ReplaceTag("@@User.zip.name@@", this.OwnerAddrCountryIsoCode) %>
																</td>
																<td id="tdShippingZipGlobal" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerZip %>
																</td>
															</tr>
															<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
															<tr>
																<td class="edit_title_bg" align="left" width="20%" nowrap>
																	<%: ReplaceTag("@@User.company_name.name@@")%>
																</td>
																<td id="tdShippingCompanyName" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerCompanyName %>
																</td>
															</tr>
															<tr>
																<td class="edit_title_bg" align="left" width="20%" nowrap>
																	<%: ReplaceTag("@@User.company_post_name.name@@")%>
																</td>
																<td id="tdShippingCompanyPostName" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerCompanyPostName %>
																</td>
															</tr>
															<%} %>
															<tr>
																<td class="edit_title_bg" align="left" width="20%">
																	<%#: ReplaceTag("@@User.tel1.name@@", this.OwnerAddrCountryIsoCode) %>
																</td>
																<td id="tdShippingTel1" class="edit_item_bg" align="left" colspan="3">
																	<%#: this.OrderInput.Owner.OwnerTel1 %>
																</td>
															</tr>
														</tbody>
														<asp:Repeater ID="rpAddressBook" DataSource="<%# this.UserShippingAddress %>" runat="server" ItemType="w2.Domain.UserShipping.UserShippingModel">
															<ItemTemplate>
																<tbody runat="server" visible="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>" class='<%# string.Format("ShippingAddress shipping_no_{0}", Item.ShippingNo) %>'>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">
																		<%: ReplaceTag("@@User.name.name@@") %>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3" Visible = "<%# IsCountryJp(Item.ShippingCountryIsoCode) %>" runat="server">
																		<%#: Item.ShippingName %> 
																		（<%#: Item.ShippingNameKana %>）
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3" Visible = "<%# (IsCountryJp(Item.ShippingCountryIsoCode) == false) %>" runat="server">
																		<%#: Item.ShippingName %>
																	</td>
																</tr>
																<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																	<tr>
																		<td class="edit_title_bg" align="left">
																			<%#: ReplaceTag("@@User.country.name@@", Item.ShippingCountryIsoCode) %>
																		</td>
																		<td class="edit_item_bg" align="left" colspan="3">
																			<%#: Item.ShippingCountryName %>
																		</td>
																	</tr>
																<% } %>
																<tr Visible = "<%# IsCountryJp(Item.ShippingCountryIsoCode) %>" runat="server">
																	<td class="edit_title_bg" align="left" width="20%">
																		<%: ReplaceTag("@@User.zip.name@@") %>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<%#: Item.ShippingZip %>
																	</td>
																</tr>
																<tr Visible = "<%# IsCountryJp(Item.ShippingCountryIsoCode) %>" runat="server">
																	<td class="edit_title_bg" align="left" width="20%">
																		<%: ReplaceTag("@@User.addr1.name@@") %>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<%#: Item.ShippingAddr1 %>
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">
																		<%#: ReplaceTag("@@User.addr2.name@@", Item.ShippingCountryIsoCode) %>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<%#: Item.ShippingAddr2 %>
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%" nowrap>
																		<%#: ReplaceTag("@@User.addr3.name@@", Item.ShippingCountryIsoCode) %>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<%#: Item.ShippingAddr3 %>
																	</td>
																</tr>
																<tr <%= Constants.DISPLAY_ADDR4_ENABLED ? "" : "style=\"display:none;\""  %>>
																	<td class="edit_title_bg" align="left" width="20%" nowrap>
																		<%#: ReplaceTag("@@User.addr4.name@@", Item.ShippingCountryIsoCode) %>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<%#: Item.ShippingAddr4 %>
																	</td>
																</tr>
																<tr Visible = "<%# (IsCountryJp(Item.ShippingCountryIsoCode) == false) %>" runat="server">
																	<td class="edit_title_bg" align="left">
																		<%#: ReplaceTag("@@User.addr5.name@@", Item.ShippingCountryIsoCode) %>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<%#: Item.ShippingAddr5 %>
																	</td>
																</tr>
																<tr Visible = "<%# (IsCountryJp(Item.ShippingCountryIsoCode) == false) %>" runat="server">
																	<td class="edit_title_bg" align="left">
																		<%#: ReplaceTag("@@User.zip.name@@", Item.ShippingCountryIsoCode) %>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<%#: Item.ShippingZip %>
																	</td>
																</tr>
																<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%" nowrap>
																		<%: ReplaceTag("@@User.company_name.name@@")%>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<%#: Item.ShippingCompanyName %>
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%" nowrap>
																		<%: ReplaceTag("@@User.company_post_name.name@@")%>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<%#: Item.ShippingCompanyPostName %>
																	</td>
																</tr>
																<%} %>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">
																		<%#: ReplaceTag("@@User.tel1.name@@", Item.ShippingCountryIsoCode) %>
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<%#: Item.ShippingTel1 %>
																	</td>
																</tr>
																</tbody>
																<tbody runat="server" visible="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON %>" class='<%# string.Format("ShippingAddress shipping_no_{0}", Item.ShippingNo) %>'>
																<tr style="height: 45px;">
																	<td class="edit_title_bg" align="left" width="25%">店舗ID</td>
																	<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopId">
																		<span>
																			<asp:Literal ID="lCvsShopId" Text="<%# Item.ShippingReceivingStoreId %>" runat="server" />
																		</span>
																		<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# Item.ShippingReceivingStoreId %>" />
																	</td>
																</tr>
																<tr style="height: 45px;">
																	<td class="edit_title_bg" align="left" width="25%">店舗名称</td>
																	<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopName">
																		<span>
																			<asp:Literal ID="lCvsShopName" Text="<%# Item.ShippingName %>" runat="server" />
																		</span>
																		<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# Item.ShippingName %>" />
																	</td>
																</tr>
																<tr style="height: 45px;">
																	<td class="edit_title_bg" align="left" width="25%">店舗住所</td>
																	<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopAddress">
																		<span>
																			<asp:Literal ID="lCvsShopAddress" Text="<%# Item.ShippingAddr4 %>" runat="server" />
																		</span>
																		<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# Item.ShippingAddr4 %>" />
																	</td>
																</tr>
																<tr style="height: 45px;">
																	<td class="edit_title_bg" align="left" width="25%">店舗電話番号</td>
																	<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopTel">
																		<span>
																			<asp:Literal ID="lCvsShopTel" Text="<%# Item.ShippingTel1 %>" runat="server" />
																		</span>
																		<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# Item.ShippingTel1 %>" />
																	</td>
																</tr>
																</tbody>
															</ItemTemplate>
														</asp:Repeater>
														<% if (this.IsStorePickup == false) { %>
															<tr>
																<td class="edit_title_bg" align="left" width="120">配送方法</td>
																<td class="edit_item_bg" align="left">
																	<asp:DropDownList id="ddlShippingMethod" DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD) %>" DataTextField="Text" DataValueField="Value"
																		SelectedValue='<%# (string.IsNullOrEmpty(Item.ShippingMethod) == false) ? Item.ShippingMethod : Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS %>' runat="server" OnSelectedIndexChanged="ddlShippingMethod_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
																	<asp:Button ID="btnSetShippingMethod" runat="server" Text="  配送方法自動判定  " OnClick="btnSetShippingMethod_Click" />
																</td>
																<td class="edit_title_bg" align="left" width="120">配送サービス</td>
																<td class="edit_item_bg" align="left">
																	<asp:DropDownList id="ddlDeliveryCompany" runat="server" DataSource="<%# GetDeliveryCompanyItemList(Item.ShippingMethod, Item.ShippingReceivingStoreFlg) %>" DataTextField="Text" DataValueField="Value" 
																		SelectedValue='<%# CheckSelectDeliveryCompany(Item.DeliveryCompanyId, Item.OrderShippingNo) %>' OnSelectedIndexChanged="ddlDeliveryCompany_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
																	<div id="divChangeShippingPriceMessage" runat="server" visible="false" class ="notice">
																		<asp:Literal ID="lChangeShippingPriceMessage" runat="server"/>
																	</div>
																</td>
															</tr>
															<%--▽ 出荷予定日オプションが有効な場合は表示 ▽--%>
															<% if (this.UseLeadTime){ %>
															<tr>
																<td class="edit_title_bg" align="left" width="120">出荷予定日</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<uc:DateTimePickerPeriodInput id="ucScheduledShippingDate" CanShowEndDatePicker="False" IsNullStartDateTime="true" IsHideTime="True" runat="server" />
																	<asp:Button id="lbAutoCalculateScheduledShippingDate" Text="  自動計算  " CommandArgument="<%# Container.ItemIndex %>" CommandName="CalculateScheduledShippingDate" runat="server"></asp:Button>
																</td>
															</tr>
															<% } %>
															<%--△ 出荷予定日オプションが有効な場合は表示 △--%>
															<tr id="trShippingDate1" visible="<%# this.IsShippingDateUsable(Container.ItemIndex) %>" runat="server">
																<td class="edit_title_bg" align="left" width="120">配送希望日</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<uc:DateTimePickerPeriodInput id="ucShippingDate" CanShowEndDatePicker="False" IsNullStartDateTime="true" IsHideTime="True" runat="server" />
																</td>
															</tr>
															<tr id="trShippingDate2" visible="<%# this.IsShippingDateUsable(Container.ItemIndex) == false %>" runat="server">
																<td class="edit_title_bg" align="left" width="120">配送希望日</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<%= ReplaceTag("@@DispText.shipping_date_list.none@@") %>&nbsp;<%= (Constants.GLOBAL_OPTION_ENABLE == false) ? "(メール便または配送希望日設定可能フラグ無効のため編集不可)" : "(海外配送のため編集不可)"%></td>
															</tr>
															<tr id="trShippingTime1" class="trShippingTime1" visible="<%# this.IsShippingTimeUsable(Container.ItemIndex) %>" runat="server">
																<td class="edit_title_bg" align="left" width="120">配送希望時間帯</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList ID="ddlShippingTime" runat="server" DataSource="<%# GetShippingTimeList(Container.ItemIndex, Item.ShippingTime) %>" DataTextField="Text" DataValueField="Value" SelectedValue='<%# Item.ShippingTime %>' ></asp:DropDownList></td>
															</tr>
															<tr id="trShippingTime2" class="trShippingTime2" visible="<%# this.IsShippingTimeUsable(Container.ItemIndex) == false %>" runat="server">
																<td class="edit_title_bg" align="left" width="120">配送希望時間帯</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<%= ReplaceTag("@@DispText.shipping_time_list.none@@") %>&nbsp;(メール便または配送希望時間帯設定可能フラグ無効のため編集不可)</td>
															</tr>
															<tr runat="server">
																<td class="edit_title_bg" align="left" width="120">配送伝票番号</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:TextBox ID="tbShippingCheckNo" Text="<%# Item.ShippingCheckNo %>" MaxLength="50" runat="server"></asp:TextBox>
																	<%if (OrderCommon.CanShipmentEntry(this.OrderInput.OrderPaymentKbn)) {%>
																		<asp:CheckBox id="cbExecExternalShipmentEntry" Text="出荷情報登録連携" Checked="false" runat="server"/>
																	<%} %>
																	<asp:HiddenField ID="hfOldShippingCheckNo" runat="server" Value="<%# Item.ShippingCheckNo %>"/>
																</td>
															</tr>
															<% if (Constants.TWPELICAN_COOPERATION_EXTEND_ENABLED) { %>
															<tr runat="server">
																<td class="edit_title_bg" align="left" width="120">データ生成日</td>
																<td class="edit_item_bg" align="left">
																	<%#: this.OrderInput.Shippings[0].ShippingStatusUpdateDate %>
																</td>
																<td class="edit_title_bg" align="left" width="120">完了状態コード</td>
																<td class="edit_item_bg" align="left">
																	<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE, this.OrderInput.Shippings[0].ShippingStatusCode) %>
																</td>
															</tr>
															<tr runat="server">
																<td class="edit_title_bg" align="left" width="120">配送状態ID</td>
																<td class="edit_item_bg" align="left">
																	<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS, this.OrderInput.Shippings[0].ShippingStatus) %>
																</td>
																<td class="edit_title_bg" align="left" width="120">営業所略称</td>
																<td class="edit_item_bg" align="left">
																	<%#: this.OrderInput.Shippings[0].ShippingOfficeName %>
																</td>
															</tr>
															<tr runat="server">
																<td class="edit_title_bg" align="left" width="120">Handy操作時間</td>
																<td class="edit_item_bg" align="left">
																	<%#: this.OrderInput.Shippings[0].ShippingHandyTime %>
																</td>
																<td class="edit_title_bg" align="left" width="120">現在の状態</td>
																<td class="edit_item_bg" align="left">
																	<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS, this.OrderInput.Shippings[0].ShippingCurrentStatus) %>
																</td>
															</tr>
															<tr id="Tr1" runat="server">
																<td class="edit_title_bg" align="left" width="120"></td>
																<td class="edit_item_bg" align="left"></td>
																<td class="edit_title_bg" align="left" width="120">状態説明</td>
																<td class="edit_item_bg" align="left">
																	<%#: this.OrderInput.Shippings[0].ShippingStatusDetail %>
																</td>
															</tr>
															<% } %>
															<%-- △配送先情報△--%>
															<%-- ▽のし・包装情報（ギフト注文の場合のみ表示）▽ --%>
															<% if ((Constants.GIFTORDER_OPTION_ENABLED) && (this.OrderInput.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)) { %>
															<tr>
																<td class="edit_title_bg" align="left">のしの種類</td>
																<td id="Td1" class="edit_item_bg" align="left" visible="<%# this.IsWrappingPaperTypeUsable %>" runat="server">
																	<asp:DropDownList ID="ddlWrappingPaperType" DataSource="<%# GetWrappingPaperTypeList(Item.WrappingPaperType) %>" SelectedValue='<%# (this.ShopShipping != null) ? Item.WrappingPaperType : "" %>' runat="server"></asp:DropDownList></td>
																<td id="Td2" class="edit_item_bg" align="left" visible="<%# this.IsWrappingPaperTypeUsable == false %>" runat="server">
																	<%#: Item.WrappingPaperType %>&nbsp;(のし設定可能フラグ無効のため編集不可)</td>
																<td class="edit_title_bg" align="left">のし差出人</td>
																<td class="edit_item_bg" align="left">
																	<asp:TextBox ID="tbWrappingPaperName" Text='<%# Item.WrappingPaperName %>' runat="server" MaxLength="50" Width="120"></asp:TextBox></td>
															</tr>
															<tr>
																<td class="edit_title_bg" align="left" width="120">包装の種類</td>
																<td id="Td3" class="edit_item_bg" align="left" colspan="3" visible="<%# this.IsWrappingBagTypeUsable %>" runat="server">
																	<asp:DropDownList ID="ddlWrappingBagType" DataSource="<%# GetWrappingBagTypeList(Item.WrappingBagType) %>" SelectedValue='<%# (this.ShopShipping != null) ? Item.WrappingBagType : "" %>' runat="server"></asp:DropDownList></td>
																<td id="Td4" class="edit_item_bg" align="left" colspan="3" visible="<%# this.IsWrappingBagTypeUsable == false %>" runat="server">
																	<%#: Item.WrappingBagType %>&nbsp;(包装設定可能フラグ無効のため編集不可)</td>
															</tr>
															<% } %>
															<%-- △のし・包装情報（ギフト注文の場合のみ表示）△--%>
														<% } %>
													</tbody>
												</table>
												<div style="<%# this.IsUpdateShippingConvenience ? "display:none" : string.Empty %>">
												<%-- △配送情報△--%>
												<% if ((Constants.GIFTORDER_OPTION_ENABLED == false) || (this.OrderInput.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_OFF)) { %>
												<br />
												<% } %>
												<!-- ▽商品情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trOrderItemErrorMessagesTitle" runat="server" visible="false">
														<td id="tdOrderItemErrorMessagesTitle" class="edit_title_bg" align="center" colspan='<%# (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 4 : 7) + this.AddColumnCountForItemTable %>'>エラーメッセージ</td>
													</tr>
													<tr id="trOrderItemErrorMessages" runat="server" visible="false">
														<td id="tdOrderItemErrorMessages" class="edit_item_bg" align="left" colspan='<%# (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 4 : 7) + this.AddColumnCountForItemTable %>'>
															<asp:Label ID="lbOrderItemErrorMessages" runat="server" ForeColor="red"></asp:Label>
														</td>
													</tr>
													<tr>
														<td id="tdOrderItems" runat="server" class="edit_title_bg" align="center" colspan='<%# (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 3 : 6) + this.AddColumnCountForItemTable %>'>商品情報</td>
														<td class="edit_title_bg" align="center">
															<asp:Button ID="btnAddProduct" runat="server" Text="  追加  " CommandName="add_product" CommandArgument="<%# Item.OrderShippingNo %>" UseSubmitBehavior="false" /><br />
															<asp:Button ID="btnReCalculate" runat="server" Text="  再計算  " onclick="btnReCalculate_Click" UseSubmitBehavior="false"/>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="center" width="10%" rowspan="2">
															選択/削除
														</td>
														<%--▽ セールOPが有効の場合 ▽--%>
														<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
														<td class="edit_title_bg" align="center" width="8%" rowspan="2">
															セールID
														</td>
														<%} %>
														<%--△ セールOPが有効の場合 △--%>
														<%--▽ ノベルティOPが有効の場合 ▽--%>
														<% if (Constants.NOVELTY_OPTION_ENABLED) { %>
														<td class="edit_title_bg" align="center" width="10%" rowspan="<%= Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>">
															ノベルティID
														</td>
														<%} %>
														<%--▽ レコメンド設定OPが有効の場合 ▽--%>
														<% if (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) { %>
														<td class="edit_title_bg" align="center" width="10%" rowspan="2">
															レコメンドID
														</td>
														<%} %>
														<%--△ レコメンド設定OPが有効の場合 △--%>
														<%--△ ノベルティOPが有効の場合 △--%>
														<%--▽ 商品同梱設定OPが有効の場合 ▽--%>
														<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED) { %>
														<td class="edit_title_bg" align="center" width="10%" rowspan="2">
															商品同梱ID
														</td>
														<% } %>
														<%--△ 商品同梱設定OPが有効の場合 △--%>
														<td class="edit_title_bg" align="center" width="10%" rowspan="2" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" runat="server">
															頒布会コースID
														</td>
														<%--▽ 定期購入OPが有効の場合 ▽--%>
														<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
														<td class="edit_title_bg" align="center" width="5%" rowspan="2">
															定期商品
														</td>
														<% } %>
														<%--△ 定期購入OPが有効の場合 △--%>
														<td class="edit_title_bg" align="center" width="15%">
															商品ID
														</td>
														<td class="edit_title_bg" align="center" rowspan="2" width="<%= 15 + (Constants.PRODUCT_SALE_OPTION_ENABLED ? 0 : 8) +  (Constants.NOVELTY_OPTION_ENABLED ? 0 : 10) + (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 0 : 10) + (this.DisplayItemSubscriptionBoxCourseIdArea ? 0 : 10) %>%">
															商品名
														</td>
														<td class="edit_title_bg" align="center" width="8%" rowspan="2" Visible="<%# this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
															単価（<%: this.ProductPriceTextPrefix %>）
														</td>
														<td class="edit_title_bg" align="center" width="3%" rowspan="2" colspan="<%# (this.OrderInput.HasSetProduct ? 2 : 1) %>">
															数量
														</td>
														<td id="tdFixedPurchaseItemCount" class="edit_title_bg" align="center" width="10%" colspan="2" rowspan="1" Visible="<%# this.IsFixedPurchaseCountAreaShow %>" runat="server">
															定期購入回数
														</td>
														<td class="edit_title_bg" align="center" width="5%" rowspan="2" style="height: 31px" Visible="<%# this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
															消費税率
														</td>
														<td class="edit_title_bg" align="center" width="9%" rowspan="2" Visible="<%# this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
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
														<td class="edit_title_bg" align="center">
															バリエーションID
														</td>
														<td id="tdFixedPurchaseItemOrderCount" class="edit_title_bg" align="center"  Visible="<%# this.IsFixedPurchaseCountAreaShow %>" runat="server">
															注文基準
														</td>
														<td id="tdFixedPurchaseItemShippedCount" class="edit_title_bg" align="center"  Visible="<%# this.IsFixedPurchaseCountAreaShow %>" runat="server">
															出荷基準
														</td>
													</tr>
													<asp:Repeater id="rItemList" DataSource="<%# Item.Items %>" OnItemCommand="rItemList_ItemCommand" runat="server" ItemType="w2.App.Common.Input.Order.OrderItemInput" OnItemDataBound="rItemList_OnItemDataBound">
													<ItemTemplate>
														<%--アイテムインデックス ▽--%>
														<asp:HiddenField id="hfItemIndex" runat="server" Value="<%# Item.ItemIndex %>" />
														<asp:HiddenField id="hfProductTaxRate" runat="server" Value="<%# Item.ProductTaxRate %>" />
														<%--▽ セット商品 ▽--%>
														<tr id="trSetItem" class='<%# (Item.DeleteTargetSet == false) ? "edit_item_bg" : "mobile_item_bg" %>' visible='<%# Item.IsProductSet %>' runat="server">
															<td align="left" runat="server" visible="<%# IsProductSetItemTop(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) && (IsSetEditable(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) == false) %>" rowspan='<%# GetProductSetRowspan(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>'>
																<span style="font-size:xx-small">在庫引当が行われいるため編集できません。（在庫引当戻しを実行後可）</span>
															</td>
															<td align="center" runat="server" visible="<%# IsProductSetItemTop(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) && (IsSetEditable(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource)) %>" rowspan='<%# GetProductSetRowspan(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>'>
																<asp:HiddenField ID="hfDeleteProductSet" runat="server" Value="<%# ((OrderItemInput)Container.DataItem).DeleteTargetSet %>" />
																<asp:Button ID="btnDeleteProductSet" Visible="<%# Item.CanDelete %>" Text="セット削除" runat="server" CommandName="delete_set" CommandArgument="<%# Container.ItemIndex %>"/>
															</td>
															<td align="center" runat="server" visible="<%# Constants.PRODUCT_SALE_OPTION_ENABLED %>" rowspan="2">
																-
															</td>
															<td align="center" runat="server" visible="<%# Constants.NOVELTY_OPTION_ENABLED %>" rowspan="<%# Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>">
																<%#: Item.NoveltyId != "" ? Item.NoveltyId : "-" %>
															</td>
															<td align="center" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) %>" rowspan="2">
																<%#: Item.RecommendId != "" ? Item.RecommendId : "-" %>
															</td>
															<td align="center" visible="<%# Constants.PRODUCTBUNDLE_OPTION_ENABLED %>" rowspan="2" runat="server">
																<%#: (string.IsNullOrEmpty(Item.ProductBundleId) == false) ? Item.ProductBundleId : "-" %>
															</td>
															<td align="center" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" rowspan="2" runat="server">
																<%#: Item.IsSubscriptionBox ? Item.SubscriptionBoxCourseId : "-" %>
															</td>
															<td align="center" runat="server" visible="<%# (Constants.FIXEDPURCHASE_OPTION_ENABLED) %>" rowspan='2'>
																<%#: ValueText.GetValueText(Constants.TABLE_ORDERITEM, Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG, Item.FixedPurchaseProductFlg) %>
															</td>
															<td align="center">
																<%#: Item.ProductId %>
															</td>
															<td align="left" rowspan="2">
																<%#: Item.ProductName %>
															</td>
															<td align="center" rowspan="2" Visible="<%# this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																<%#: Item.IsSubscriptionBoxFixedAmount == false ? Item.ProductPrice.ToPriceString(true) : "-" %>
															</td>
															<td align="center" rowspan="2">
																<%#: Item.ItemQuantitySingle %>
															</td>
															<td align="center" rowspan='<%# GetProductSetRowspan(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' Visible='<%# (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) && IsProductSetItemTop(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' runat="server">
																<%#: Item.IsSubscriptionBoxFixedAmount == false ? string.Format("x {0}", Item.ProductSetCount) : "-" %>
															</td>
															<td align="center" rowspan="2" Visible="<%# this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																<%#: Item.IsSubscriptionBoxFixedAmount == false ? string.Format("{0}%", TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate)) : "-" %>
															</td>
															<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan='<%# GetProductSetRowspan(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' Visible='<%# (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) && IsProductSetItemTop(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' runat="server">
																<%#: Item.IsSubscriptionBoxFixedAmount == false
																	     ? CreateSetPriceSubtotal(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource).ToPriceString(true)
																	     : string.Format("定額{0}", Item.SubscriptionBoxFixedAmount.ToPriceString(true)) %>
															</td>
														</tr>
														<tr id="trSetItem1" class='<%# (Item.DeleteTargetSet == false) ? "edit_item_bg" : "mobile_item_bg" %>' visible='<%# Item.IsProductSet %>' runat="server">
															<td align="center" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) %>">
																<%#: Item.RecommendId != "" ? Item.RecommendId : "-" %>
															</td>
															<td align="center">
																<%#: Item.VId %>
															</td>
														</tr>
														<%--△ セット商品 △--%>
														<%--▽ 通常商品 ▽--%>
														<tr id="trItem" class="edit_item_bg" visible='<%# (Item.IsProductSet == false) %>' runat="server">
															<td align="left" runat="server" rowspan="2" visible="<%# (Item.IsReturnItem == false) && Item.IsRealStockReserved %>">
																<span style="font-size:xx-small">在庫引当が行われいるため編集できません。（在庫引当戻しを実行後可）</span>
															</td>
															<td align="center" runat="server" rowspan="2" visible="<%# ((Item.IsReturnItem == false) && (Item.IsRealStockReserved == false)) %>">
																<input id="inputSelectProduct" type="button" value="選択" onclick="javascript:open_product_list('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT + "&" + Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE + "=" + HttpUtility.UrlEncode(this.OrderInput.ShippingId) + "&" + Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID + "=" + HttpUtility.UrlEncode(this.OrderInput.MemberRankId) + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>	','contact','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%# ((RepeaterItem)Container.Parent.Parent).ItemIndex %>','<%# Container.ItemIndex %>');" /><br />
																<asp:HiddenField ID="hfDeleteProduct" runat="server" Value="<%# Item.DeleteTarget %>" />
																<asp:Button ID="btnDeleteProduct" Visible="<%# Item.CanDelete %>" Text="削除" runat="server" CommandName="delete_item" CommandArgument="<%# Container.ItemIndex %>"/>
															</td>
															<td align="center" runat="server" rowspan="2" visible="<%# Item.IsReturnItem %>">
																<span style="font-size:xx-small">返品商品のためため編集できません。</span>
															</td>
															<%--▽ 通常、交換商品の場合 ▽--%>
															<td align="center" runat="server" rowspan="2" visible="<%# Constants.PRODUCT_SALE_OPTION_ENABLED && (Item.IsReturnItem == false) %>">
																<asp:TextBox ID="tbProductSaleId" runat="server" Text="<%# Item.ProductsaleId %>" Width="50" MaxLength="8" Enabled="<%# (Item.IsRealStockReserved == false) %>"></asp:TextBox>
															</td>
															<td align="center" runat="server" rowspan="<%# Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>" visible="<%# (Constants.NOVELTY_OPTION_ENABLED) && (Item.IsReturnItem == false) %>">
																<asp:TextBox ID="tbNoveltyId" runat="server" Text="<%# Item.NoveltyId %>" Width="50" MaxLength="30" Enabled="<%# (Item.IsRealStockReserved == false) %>"></asp:TextBox>
															</td>
															<td align="center" runat="server" rowspan="2" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) && (Item.IsReturnItem == false) %>">
																<asp:TextBox ID="tbRecommendId" runat="server" Text="<%# Item.RecommendId %>" Width="50" MaxLength="30" Enabled="<%# (Item.IsRealStockReserved == false) %>"></asp:TextBox>
															</td>
															<td align="center" rowspan="2" visible="<%# (Constants.PRODUCTBUNDLE_OPTION_ENABLED && (Item.ItemReturnExchangeKbn != Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN)) %>" runat="server">
																<asp:TextBox ID="tbProductBundleId" Text="<%# Item.ProductBundleId %>" Width="55" MaxLength="30" Enabled="<%# (Item.IsRealStockReserved ==false) %>" runat="server"></asp:TextBox>
															</td>
															<td align="center" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea && (Item.IsReturnItem == false) %>" rowspan="2" runat="server">
																<%#: Item.IsSubscriptionBox ? Item.SubscriptionBoxCourseId : "-" %>
															</td>
															<td align="center" runat="server" rowspan="2" visible="<%# (Constants.FIXEDPURCHASE_OPTION_ENABLED && (Item.IsReturnItem == false)) %>">
																<asp:CheckBox ID="cbFixedPurchaseProduct" OnCheckedChanged="cbFixedPurchaseProduct_CheckedChanged" AutoPostBack="true" Checked='<%# (Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON) %>' runat="server" />
															</td>
															<td align="left" runat="server" visible="<%# (Item.IsReturnItem == false) %>">
																<asp:TextBox ID="tbProductId" runat="server" Text="<%# Item.ProductId %>" Width="90" MaxLength="30" Enabled="<%# (Item.IsRealStockReserved == false) %>"></asp:TextBox>
															</td>
															<td align="left" runat="server" rowspan="2" visible="<%# (Item.IsReturnItem == false) %>">
																<asp:TextBox ID="tbProductName" runat="server" Text="<%# Item.ProductName %>" Width="<%# 115 + (Constants.PRODUCT_SALE_OPTION_ENABLED ? 0 : 50) +  (Constants.NOVELTY_OPTION_ENABLED ? 0 : 50) + (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 0 : 55) %>" MaxLength="200" Enabled="<%# (Item.IsRealStockReserved == false) %>"></asp:TextBox><br />
																<asp:DropDownList ID="ddlOrderSetPromotion" DataSource="<%# this.OrderSetPromotionList %>" SelectedValue="<%# (Item.IsReturnItem == false) ? Item.OrderSetpromotionNo : null %>" DataTextField="Text" DataValueField="Value" Visible="<%# this.OrderInput.SetPromotions.Length != 0 %>" Width="<%# 115 + (Constants.PRODUCT_SALE_OPTION_ENABLED ? 0 : 50) +  (Constants.NOVELTY_OPTION_ENABLED ? 0 : 50) + (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 0 : 55) %>" runat="server"></asp:DropDownList>
																<asp:HiddenField ID="hfOrderSetPromotionNo" Value="<%# Item.OrderSetpromotionNo %>" runat="server" />
															</td>
															<td align="center" runat="server" rowspan="2" Visible="<%# (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) && (Item.IsReturnItem == false) %>">
																<asp:TextBox id="tbProductPrice" runat="server" Text="<%# Item.ProductPrice.ToPriceString() %>" Width="100" Enabled="<%# Item.IsRealStockReserved == false %>" Visible="<%# Item.IsSubscriptionBoxFixedAmount == false %>" />
																<div Visible="<%# Item.IsSubscriptionBoxFixedAmount %>" runat="server">-</div>
															</td>
															<td align="center" runat="server" rowspan="2" colspan="<%# (this.OrderInput.HasSetProduct ? 2 : 1) %>" visible="<%# (Item.IsReturnItem == false) %>">
																<asp:TextBox id="tbItemQuantity" runat="server" Text="<%# Item.ItemQuantity %>" Width="30" MaxLength="3" Enabled="<%# (Item.IsRealStockReserved == false) %>"></asp:TextBox>
															</td>
															<td id="tdFixedPurchaseItemOrderCount" align="center" runat="server" rowspan="2" visible="<%# this.IsFixedPurchaseCountAreaShow && Item.IsReturnItem == false %>">
																<asp:TextBox id="tbFixedPurchaseItemOrderCount" runat="server" Width="33" MaxLength="3" Text='<%# Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON ? (string.IsNullOrEmpty(Item.FixedPurchaseItemOrderCount) ? "0" : Item.FixedPurchaseItemOrderCount) : string.Empty %>' Visible="<%# Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON %>"></asp:TextBox>
																<asp:Label id="lbFixedPurchaseItemOrderCount" Text='<%# Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON ? " 回" : "-" %>' runat="server"></asp:Label>
															</td>
															<td id="tdFixedPurchaseItemShippedCount"  align="center" runat="server" rowspan="2" visible="<%# this.IsFixedPurchaseCountAreaShow && Item.IsReturnItem == false %>">
																<asp:TextBox id="tbFixedPurchaseItemShippedCount" runat="server" Width="33" MaxLength="3" Text='<%# Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON ? (string.IsNullOrEmpty(Item.FixedPurchaseItemShippedCount) ? "0" : Item.FixedPurchaseItemShippedCount) : string.Empty %>' Visible="<%# Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON %>"></asp:TextBox>
																<asp:Label id="lbFixedPurchaseItemShippedCount" Text='<%# Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON ? " 回" : "-" %>' runat="server"></asp:Label>
															</td>
															<%--△ 通常、交換商品の場合 △--%>
															<%--▽ 返品商品の場合 ▽--%>
															<td align="center" runat="server" rowspan="2" visible="<%# (Constants.PRODUCT_SALE_OPTION_ENABLED && Item.IsReturnItem) %>">
																<%#: Item.ProductsaleId != "" ? Item.ProductsaleId : "-" %>
																<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# Item.ProductsaleId %>" />
															</td>
															<td align="center" runat="server" rowspan="<%# Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>" visible="<%# (Constants.NOVELTY_OPTION_ENABLED && Item.IsReturnItem) %>">
																<%#: Item.NoveltyId != "" ? Item.NoveltyId : "-" %>
															</td>
															<td align="center" runat="server" rowspan="2" visible="<%# ((Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) && Item.IsReturnItem) %>">
																<%#: Item.RecommendId != "" ? Item.RecommendId : "-" %>
															</td>
															<td align="center" runat="server" rowspan="2" visible="<%# (Constants.FIXEDPURCHASE_OPTION_ENABLED && Item.IsReturnItem) %>">
																<%#: ValueText.GetValueText(Constants.TABLE_ORDERITEM, Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG, Item.FixedPurchaseProductFlg) %>
															</td>
															<td align="center" runat="server" rowspan="2" visible="<%# (Constants.PRODUCTBUNDLE_OPTION_ENABLED && (Item.ItemReturnExchangeKbn == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN)) %>">
																<%#: (string.IsNullOrEmpty(Item.ProductBundleId) == false) ? Item.ProductBundleId : "-" %>
															</td>
															<td align="center" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea && Item.IsReturnItem %>" rowspan="2" runat="server">
																<%#: Item.IsSubscriptionBox ? Item.SubscriptionBoxCourseId : "-" %>
															</td>
															<td align="center" runat="server" visible="<%# Item.IsReturnItem %>">
																<%#: Item.ProductId %>
																<asp:HiddenField ID="hfProductId" runat="server" Value="<%# Item.ProductId %>" />
															</td>
															<td align="left" runat="server" rowspan="2" visible="<%# Item.IsReturnItem %>">
																<%#: Item.ProductName %>
																<asp:HiddenField ID="hfProductName" runat="server" Value="<%#: Item.ProductName %>" /><br />
																<span visible='<%# (string.IsNullOrEmpty(Item.OrderSetpromotionNo) == false) %>' runat="server">
																	[<%#: Item.OrderSetpromotionNo %>： <%#: this.OrderInput.GetOrderSetPromotionName(Item.OrderSetpromotionNo) %>]
																</span>
															</td>
															<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' runat="server" rowspan="2" Visible="<%# (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) && Item.IsReturnItem %>">
																<%#: Item.IsSubscriptionBoxFixedAmount == false ? Item.ProductPrice.ToPriceString(true) : "-" %>
																<asp:HiddenField ID="hfProductPrice" runat="server" Value="<%# Item.ProductPrice %>" />
															</td>
															<td align="center" runat="server" rowspan="2" colspan="<%# (this.OrderInput.HasSetProduct ? 2 : 1) %>" Visible="<%# (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) && Item.IsReturnItem %>">
																<div Visible="<%# Item.IsSubscriptionBoxFixedAmount == false %>" runat="server"><%# GetMinusNumberNoticeHtml(Item.ItemQuantity, false) %></div>
																<div Visible="<%# Item.IsSubscriptionBoxFixedAmount %>" runat="server">-</div>
																<asp:HiddenField ID="hfItemQuantity" runat="server" Value="<%# Item.ItemQuantity %>" />
															</td>
															<td align="center" runat="server" rowspan="2" Visible="<%# this.IsFixedPurchaseCountAreaShow && Item.IsReturnItem %>">
																<asp:HiddenField ID="hfFixedPurchaseItemOrderCount" runat="server" Value='<%#: Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON ? string.Format("{0} 回", string.IsNullOrEmpty(Item.FixedPurchaseItemOrderCount) ? "0" : Item.FixedPurchaseItemOrderCount) : "-" %>' />
															</td>
															<td align="center" runat="server" rowspan="2" Visible="<%# this.IsFixedPurchaseCountAreaShow && Item.IsReturnItem %>">
																<asp:HiddenField ID="hfFixedPurchaseItemShippedCount" runat="server" Value='<%#: Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON ? string.Format("{0} 回", string.IsNullOrEmpty(Item.FixedPurchaseItemShippedCount) ? "0" : Item.FixedPurchaseItemShippedCount) : "-" %>' />
															</td>
															<%--△  返品商品の場合 △--%>
															<td align="center" rowspan="2" Visible="<%# this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																<%#: Item.IsSubscriptionBoxFixedAmount == false ? string.Format("{0}%", TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate)) : "-" %>
															</td>
															<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																<span ID="sItemPrice" Visible="<%# Item.IsSubscriptionBoxFixedAmount == false %>" runat="server">
																	<%# GetMinusNumberNoticeHtml(Item.ItemPrice, true) %>
																</span>
																<span Visible="<%# Item.IsSubscriptionBoxFixedAmount %>" runat="server">
																	定額(<%#: Item.SubscriptionBoxFixedAmount.ToPriceString(true) %>)
																</span>
																<%--▽隠しタグ サプライヤID、商品名かな、商品価格、実在庫引当済み商品数保持▽--%>
																<asp:HiddenField ID="hfSupplierId" runat="server" Value="<%# Item.SupplierId %>" />
																<asp:HiddenField ID="hfProductNameKana" runat="server" Value="<%# Item.ProductNameKana %>" />
																<asp:HiddenField ID="hfItemRealStockReserved" runat="server" Value="<%# Item.ItemRealstockReserved %>" />
															<%--△隠しタグ サプライヤID、商品名かな、商品価格、実在庫引当済み商品数保持△--%>

																<%--▽商品セット用追加タグ▽--%>
																<asp:HiddenField ID="hfProductSetId" runat="server" Value="<%# Item.ProductSetId %>"/>
																<asp:HiddenField ID="hfProductSetNo" runat="server" Value="<%# Item.ProductSetNo %>"/>
																<%--△商品セット用追加タグ△--%>
																
																<%--▽返品交換追加タグ▽--%>
																<asp:HiddenField ID="hfItemReturnExchangeKbn" runat="server" Value="<%# Item.ItemReturnExchangeKbn %>" />
																<%--△返品交換追加タグ△--%>
															</td>
														</tr>
														<tr id="trItem1" class="edit_item_bg" visible='<%# (Item.IsProductSet == false) %>' runat="server">
															<td align="center" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) && (Item.IsReturnItem == false) %>">
																<asp:TextBox ID="tbRecommendId2" runat="server" Text="<%# Item.RecommendId %>" Width="50" MaxLength="30" Enabled="<%# (Item.IsRealStockReserved == false) %>"></asp:TextBox>
															</td>
															<td align="center" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) && Item.IsReturnItem %>">
																<%#: (Item.RecommendId != "") ? Item.RecommendId : "-"%>
															</td>
															<td align="left" runat="server" visible="<%# (Item.IsReturnItem == false) %>">
																<asp:TextBox ID="tbVariationId" runat="server" Text="<%# Item.VId %>" Width="60" MaxLength="60" Enabled="<%# (Item.IsRealStockReserved == false) %>"></asp:TextBox>
																<asp:Button ID="btnGetProductData" CommandArgument="<%# ((OrderShippingInput)(((RepeaterItem)Container.Parent.Parent)).DataItem).OrderShippingNo %>" CommandName="get" Text="取得" runat="server" visible="<%# ((Item.IsReturnItem == false) && (Item.IsRealStockReserved == false)) %>" UseSubmitBehavior="false" />
															</td>
															<td align="center" runat="server" visible="<%# Item.IsReturnItem %>">
																<%#: (Item.VId != "") ? Item.VId : "-"%>
																<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# Item.VId %>" />
															</td>
														</tr>
														<%--△ 通常商品 △--%>
														<tr visible="<%# (Item.IsProductSet == false) %>" runat="server">
															<td align="center" class="edit_title_bg">付帯情報</td>
															<td id="tdItem1" runat="server" class="edit_item_bg" colspan='<%# (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 3 : 6) + this.AddColumnCountForItemTable %>'>
																<%-- 通常商品時 --%>
																<div id="dvProductOptionValue" visible='<%# Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED %>' style="margin: 10px 0" runat="server">
																	<!-- ▽ 付帯情報編集欄 ▽ -->
																	<asp:Repeater ID="rProductOptionValueSettings" ItemType="w2.App.Common.Product.ProductOptionSetting" DataSource="<%# Item.ProductOptionSettingsWithSelectedValues %>" runat="server">
																		<ItemTemplate>
																			<div style="margin-top: 6px;">
																				<strong style="display: inline-block; padding: 2px 1px;"><%#: Item.ValueName %><span class="notice" runat="server" visible="<%# Item.IsNecessary %>">*</span></strong><br />
																				<span style="display: inline-block">
																					<asp:DropDownList runat="server" ID="ddlProductOptionValueSetting" DataSource="<%# Item.SettingValuesListItemCollection %>" Visible="<%# Item.IsSelectMenu || Item.IsDropDownPrice %>" SelectedValue="<%# Item.GetDisplayProductOptionSettingSelectedValue() %>" />
																					<asp:TextBox ID="tbProductOptionValueSetting" Text="<%# Item.SelectedSettingValueForTextBox %>" Visible="<%# Item.IsTextBox %>" runat="server" />
																					<asp:Repeater ID="rCblProductOptionValueSetting" DataSource="<%# Item.SettingValuesListItemCollection %>" ItemType="System.Web.UI.WebControls.ListItem" Visible="<%# Item.IsCheckBox || Item.IsCheckBoxPrice %>" runat="server">
																						<ItemTemplate>
																							<span title="<%# Eval("Text") %>">
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
																<% if(Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false){ %>
																<asp:TextBox ID="tbProductOptionSettingSelectedTexts" Text='<%# Item.ProductOptionTexts %>' Width="650" runat="server" Visible="<%# (Item.IsReturnItem == false) %>" />
																<% } %>
																<%-- 返品商品時 --%>
																<%#: Item.IsReturnItem ? Item.ProductOptionTexts : "" %>
															</td>
														</tr>
													</ItemTemplate>
													</asp:Repeater>
												</table>
												<%-- △注文商品情報△--%>
												<br />
												</ItemTemplate>
												</asp:Repeater>

												<% if ((Constants.GIFTORDER_OPTION_ENABLED) && (this.OrderInput.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)) { %>
												<%--▽配送先追加ボタン▽--%>
												<asp:Button ID="btnAddShipping" Text="  配送先追加  " onclick="btnAddShipping_Click" runat="server" UseSubmitBehavior="false" />
												<%--△配送先追加ボタン△--%>
												<br />
												<% } %>

												<%-- ▽自動計算適用▽ --%>
												<br/>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center"><asp:CheckBox ID="cbApplyAutoCalculation" runat="server" Checked="true" CssClass="checkBox" Text="自動計算適用" OnCheckedChanged="cbApplyAutoCalculation_CheckedChanged" AutoPostBack="true" /></td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left">
															<% if (cbApplyAutoCalculation.Enabled){ %>
																再計算時に下記が自動で適用されます。<br/>
																<%= Constants.SETPROMOTION_OPTION_ENABLED ? "[セットプロモーション割引]" + "<br/>" : "" %>
																<%= Constants.MEMBER_RANK_OPTION_ENABLED ? "[会員ランク割引]" + "<br/>" : "" %>
																<%= Constants.W2MP_COUPON_OPTION_ENABLED ? "[クーポン割引額]" + "<br/>" : "" %>
																<%= Constants.FIXEDPURCHASE_OPTION_ENABLED ? "[定期会員割引]" + "<br/>" + "[定期購入割引]" + "<br/>" : "" %>
																<%= Constants.W2MP_POINT_OPTION_ENABLED ? "[付与ポイント]" + "<br/>" : "" %>
																手動変更を行う場合はチェックを外してから入力を行ってください。
															<% } else { %>
																<span style="color:red;">引当て済み商品が存在する場合、自動計算適用できません。引当て戻し後に利用してください。</span>
															<% } %>
														</td>
													</tr>
												</table>
												<br/>
												<%-- △自動計算適用△ --%>

												<%-- ▽合計金額▽ --%>
												<div id="dvRecalculateArea" runat="server">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trOrderPriceAlertMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="2">アラートメッセージ</td>
													</tr>
													<tr id="trOrderPriceAlertMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:Label ID="lbOrderPriceAlertMessages" runat="server" ForeColor="red"></asp:Label>
														</td>
													</tr>
													<tr id="trOrderPriceErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
													</tr>
													<tr id="trOrderPriceErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:Label ID="lbOrderPriceErrorMessages" runat="server" ForeColor="red"></asp:Label>
														</td>
													</tr>
													<tr id="trSubscriptionBoxOrderPriceErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:Label ID="lbSubscriptionBoxOrderPriceErrorMessages" runat="server" ForeColor="red"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="center" width="90%">合計金額</td>
														<td class="edit_title_bg" align="center" width="10%"><asp:Button ID="btnReCalculate" runat="server" Text="  再計算  " onclick="btnReCalculate_Click" UseSubmitBehavior="false"/></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="right">商品合計（<%: this.ProductPriceTextPrefix %>）</td>
														<td class="edit_item_bg" align="right">
															<%# GetMinusNumberNoticeHtml(this.OrderInput.OrderPriceSubtotal, true) %></td>
													</tr>
													<%if (this.ProductIncludedTaxFlg == false) { %>
														<tr>
															<td class="edit_title_bg" align="right">消費税額
															</td>
															<td class="edit_item_bg" align="right">
																<%#: this.OrderInput.OrderPriceSubtotalTax.ToPriceString() %></td>
														</tr>
													<%} %>
													<asp:Repeater ID="rSetPromotionProductDiscount" DataSource="<%# this.OrderInput.SetPromotions %>" Visible="<%# this.OrderInput.IsAllItemsSubscriptionBoxFixedAmount == false %>" runat="server" ItemType="OrderSetPromotionInput">
														<ItemTemplate>
															<tr visible="<%# Item.IsDiscountTypeProductDiscount %>" runat="server">
																<td class="edit_title_bg" align="right">
																	<asp:Literal ID="lOrderSetPromotionNo" Text="<%#: Item.OrderSetpromotionNo %>" runat="server"></asp:Literal>：
																	<%#: Item.SetpromotionName %>割引額<br />
																	(ID:<%#: Item.SetpromotionId %>, 商品割引分)
																</td>
																<td class="edit_title_bg" align="right">
																	<asp:TextBox ID="tbSetPromotionProductDiscount" runat="server" Width="60" Text="<%# Item.ProductDiscountAmount.ToPriceString() %>" Enabled="<%# (cbApplyAutoCalculation.Checked == false) %>"></asp:TextBox>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<%-- 会員ランクOPが有効な場合 --%>
													<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="right">
															会員ランク割引<br />
															（現在の設定：<asp:Literal ID="lMemberRankDiscountDisp" runat="server" />）
														</td>
														<td class="edit_item_bg" align="right">
															<asp:TextBox ID="tbMemberRankDiscount" Text='<%# this.OrderInput.MemberRankDiscountPrice.ToPriceString() %>' runat="server" Width="60" Enabled="<%# (cbApplyAutoCalculation.Checked == false) %>"></asp:TextBox></td>
													</tr>
													<%} %>
													<%-- 会員ランクOPかつ定期購入OPが有効な場合 --%>
													<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="right">
															定期会員割引<br />
															（現在の設定：<asp:Literal ID="lFixedPurchaseMemberDiscount" runat="server" />）
														</td>
														<td class="edit_item_bg" align="right">
															<asp:TextBox ID="tbFixedPurchaseMemberDiscount" Text='<%# this.OrderInput.FixedPurchaseMemberDiscountAmount.ToPriceString() %>' runat="server" Width="60" Enabled="<%# (cbApplyAutoCalculation.Checked == false) %>"></asp:TextBox></td>
													</tr>
													<%} %>
													<%-- クーポンオプションが有効の場合 --%>
													<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="right">クーポン割引額</td>
														<td class="edit_item_bg" align="right">
															<span class='<%# decimal.Parse(this.OrderInput.OrderCouponUse) > 0 ? "notice" : "" %>'>
																<%# decimal.Parse(this.OrderInput.OrderCouponUse) > 0 ? "-" : "" %><%#: this.OrderInput.OrderCouponUse.ToPriceString(true) %>
															</span>
														</td>
													</tr>
													<%} %>
													<%-- ポイントオプションが有効の場合 --%>
													<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="right">ポイント利用額</td>
														<td class="edit_item_bg" align="right">
															<span class='<%# decimal.Parse(this.OrderInput.OrderPointUseYen) > 0 ? "notice" : "" %>'>
																<%# decimal.Parse(this.OrderInput.OrderPointUseYen) > 0 ? "-" : "" %><%#: this.OrderInput.OrderPointUseYen.ToPriceString(true) %>
															</span> 
														</td>
													</tr>
													<%} %>
													<%-- 定期購入オプションが有効の場合 --%>
													<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="right">定期購入割引額</td>
														<td class="edit_item_bg" align="right">
															<asp:TextBox ID="tbFixedPurchaseDiscountPrice" Text='<%# this.OrderInput.FixedPurchaseDiscountPrice.ToPriceString() %>' runat="server" Width="60" Enabled="<%# (cbApplyAutoCalculation.Checked == false) %>"></asp:TextBox></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="right">配送料
														<!-- 配送料別途見積もり表示する場合 -->
														<% if (this.OrderInput.IsValidShippingPriceSeparateEstimatesFlg){ %>
															<br /><asp:CheckBox ID="cbShippingPriceSeparateEstimateFlg" runat="server" Checked="<%# this.OrderInput.IsValidShippingPriceSeparateEstimatesFlg %>" OnCheckedChanged="cbShippingPriceSeparateEstimateFlg_OnCheckedChanged" AutoPostBack="true" />
															<%#: (this.ShopShipping != null)
																 ? (string.IsNullOrEmpty(this.ShopShipping.ShippingPriceSeparateEstimatesMessage) == false) ? this.ShopShipping.ShippingPriceSeparateEstimatesMessage : this.ShopShipping.ShippingPriceSeparateEstimatesMessageMobile
																 : string.Empty %>
														<%} %>
														</td>
														<td class="edit_item_bg" align="right">
															<asp:TextBox id="tbOrderPriceShipping" Text='<%# this.OrderInput.OrderPriceShipping.ToPriceString() %>' runat="server" Width="60"></asp:TextBox></td>
													</tr>
													<asp:Repeater ID="rSetPromotionShippingChargeDiscount" DataSource="<%# this.OrderInput.SetPromotions %>" runat="server" ItemType="OrderSetPromotionInput">
														<ItemTemplate>
															<tr visible="<%# Item.IsDiscountTypeShippingChargeFree %>" runat="server">
																<td class="edit_title_bg" align="right">
																	<asp:Literal ID="lOrderSetPromotionNo" Text="<%#: Item.OrderSetpromotionNo %>" runat="server"></asp:Literal>：
																	<%#: Item.SetpromotionName %>割引額<br />
																	(ID:<%#: Item.SetpromotionId %>, 配送料割引分)
																</td>
																<td class="edit_title_bg" align="right">
																	<asp:TextBox ID="tbSetPromotionShippingChargeDiscount" runat="server" Width="60" Text="<%# Item.ShippingChargeDiscountAmount.ToPriceString() %>" Enabled="<%# (cbApplyAutoCalculation.Checked == false) %>"></asp:TextBox>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr>
														<td class="edit_title_bg" align="right">決済手数料</td>
														<td class="edit_item_bg" align="right">
															<asp:TextBox id="tbOrderPriceExchange"
																Text='<%# this.OrderInput.OrderPriceExchange.ToPriceString() %>'
																ForeColor='<%# GetPriceDisplayColor(this.OrderInput.OrderPriceExchange) %>' runat="server" Width="60"></asp:TextBox></td>
													</tr>
													<asp:Repeater ID="rSetPromotionPaymentChargeDiscount" DataSource="<%# this.OrderInput.SetPromotions %>" runat="server" ItemType="OrderSetPromotionInput">
														<ItemTemplate>
															<tr visible="<%# Item.IsDiscountTypePaymentChargeFree %>" runat="server">
																<td class="edit_title_bg" align="right">
																	<asp:Literal ID="lOrderSetPromotionNo" Text="<%#: Item.OrderSetpromotionNo %>" runat="server"></asp:Literal>：
																	<%#: Item.SetpromotionName %>割引額<br />
																	(ID:<%#: Item.SetpromotionId %>, 決済手数料割引分)
																</td>
																<td class="edit_title_bg" align="right">
																	<asp:TextBox ID="tbSetPromotionPaymentChargeDiscount" runat="server" Width="60" Text="<%# Item.PaymentChargeDiscountAmount.ToPriceString() %>" Enabled="<%# (cbApplyAutoCalculation.Checked == false) %>"></asp:TextBox>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr>
														<td class="edit_title_bg" align="right">調整金額</td>
														<td class="edit_item_bg" align="right">
															<asp:TextBox id="tbOrderPriceRegulation"
																Text='<%# this.OrderInput.OrderPriceRegulation.ToPriceString() %>'
																ForeColor='<%# GetPriceDisplayColor(this.OrderInput.OrderPriceRegulation) %>' runat="server" Width="60" Enabled='<%# (this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN) %>'></asp:TextBox></td>
													</tr>
													<asp:Repeater ID="rPriceCorrection" ItemType="OrderPriceByTaxRateInput" DataSource="<%# this.OrderInput.OrderPriceByTaxRates %>" runat="server"  Visible="<%# (this.OrderInput.ReturnExchangeKbn != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN) %>">
														<ItemTemplate>
															<tr>
																	<asp:HiddenField ID="hfTaxRate" runat="server" value="<%#: Item.KeyTaxRate%>" />
																<td class="edit_title_bg" align="right">
																	返品用金額補正(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate) %>%)
																</td>
																<td class="edit_item_bg" align="right">
																	<asp:TextBox ID="tbPriceCorrection" runat="server" Width="60" Text="<%#: Item.PriceCorrectionByRate.ToPriceString() %>"></asp:TextBox>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater ID="rTotalPriceByTaxRate" ItemType="OrderPriceByTaxRateInput" DataSource="<%# this.OrderInput.OrderPriceByTaxRates %>" runat="server">
														<ItemTemplate>
															<tr runat="server">
																<td class="edit_title_bg" align="right">
																	合計金額内訳(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate)%>%)
																</td>
																<td class="edit_item_bg" align="right">
																	<%# GetMinusNumberNoticeHtml(Item.PriceTotalByRate, true) %>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr>
														<td class="edit_title_bg" align="right">金額合計
														</td>
														<td class="edit_item_bg" align="right">
															<%# GetMinusNumberNoticeHtml(this.OrderInput.OrderPriceTotal, true) %></td>
													</tr>
													<tr style="border-width:0 !important; background-color:#fff"><td colspan="2"></td></tr>
													<tr>
														<td class="edit_title_bg" align="right">前回の請求金額
														</td>
														<td class="edit_item_bg" align="right">
															<%# GetMinusNumberNoticeHtml(this.OrderOld.LastBilledAmount, true) %></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="right">今回の請求金額
														</td>
														<td class="edit_item_bg" align="right">
															<%# GetMinusNumberNoticeHtml(this.OrderInput.LastBilledAmount, true) %></td>
													</tr>
													<tr id="trSettlementAmount" runat="server">
														<td class="edit_title_bg" align="right">決済金額
														</td>
														<td class="edit_item_bg" align="right">
															<asp:Literal ID="lSettlementAmount" runat="server"></asp:Literal>
														</td>
													</tr>
												</table>
												</div>
												<%-- △合計金額△--%>
												<br />
												<%--▽ 調整金メモ ▽--%>
												<table cellspacing="0" cellpadding="0" width="758" border="0">
													<tbody>
														<tr valign="top">
															<td align="right" >
																<table class="edit_table" cellspacing="1" cellpadding="3" width="250" border="0">
																	<tr>
																		<td class="edit_title_bg" align="center">
																			調整金額メモ
																			<uc:FieldMemoSetting runat="server" Title="調整金額メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_REGULATION_MEMO %>" />
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="center">
																			<asp:TextBox ID="tbRegulationMemo" Text='<%# this.OrderInput.RegulationMemo %>' runat="server" TextMode="MultiLine" Width="99%" Rows="3"></asp:TextBox></td>
																	</tr>
																</table>
															</td>
														</tr>
													</tbody>
												</table>
												<%--△ 調整金メモ △--%>

												<!-- ▽会員ランク機能▽ -->
												<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
												<div id="divRank" runat="server">
													<br />
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tbody>
															<tr>
																<td class="edit_title_bg" align="center" colspan="4">会員ランク情報</td>
															</tr>
															<tr id="trOrderRank" runat="server">
																<td class="edit_title_bg" align="left" width="25%">注文時会員ランク</td>
																<td class="edit_item_bg" align="left" colspan="3"><%#: this.OrderInput.MemberRankName %></td>
															</tr>
														</tbody>
													</table>
												</div>
												<%} %>
												<!-- △会員ランク機能△ -->

												<%-- ポイントオプションが有効の場合--%>
												<% if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
												<div id="divPoint" runat="server">
												<br />
												<!-- ▽ポイント情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr id="trOrderPointErrorMessagesTitle" runat="server">
															<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
														</tr>
														<tr id="trOrderPointErrorMessages" runat="server">
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:Label ID="lbOrderPointErrorMessages" runat="server" ForeColor="red"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">ポイント情報</td>
														</tr>
														<%-- 会員ユーザの場合表示--%>
														<tr id="trOrderPointUse" runat="server">
															<td class="edit_title_bg" align="left" width="25%">利用ポイント</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox ID="tbOrderPointUse" runat="server" Width="120"></asp:TextBox>pt&nbsp;&nbsp;(現在の利用可能ポイント：<asp:Literal ID="lUserPoint" runat="server" />pt)
															</td>
														</tr>
														<tr id="trLastOrderPointUseBefore" runat="server">
															<td class="edit_title_bg" align="left" width="25%">変更前最終利用ポイント</td>
															<td class="edit_item_bg" align="left" colspan="3"><asp:Literal ID="lLastOrderPointUseBefore" runat="server"></asp:Literal>pt</td>
														</tr>
														<%-- 通常本ポイント付与時 --%>
														<tr id="trOrderPointAddBasePoint" runat="server">
															<td class="edit_title_bg" align="left" width="25%">
																付与通常本ポイント<br/>（現在の設定：<%#: this.OrderPointAdd %>pt）
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox id="tbOrderPointAddBaseComp" runat="server" Width="120" Enabled="<%# (cbApplyAutoCalculation.Checked == false) %>"></asp:TextBox>pt&nbsp;&nbsp;(本ポイント)
																<asp:HiddenField ID="hfBasePointKbnNo" runat="server" Value="<%# this.UserBasePointCompPointKbnNo %>" />
															</td>
														</tr>
														<%-- 仮ポイント/期間限定ポイント付与時 --%>
														<asp:Repeater id="rOrderPointAddTempOrLimitedTermComp" DataSource="<%# (this.UserPointRelatedThisOrder != null) ? this.UserPointRelatedThisOrder.Items : null %>" runat="server" ItemType="w2.Domain.Point.UserPointModel">
															<ItemTemplate>
																<tr>
																	<td class="edit_title_bg" align="left" width="25%">
																		付与ポイント
																		(<%#: ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_INC_KBN, Item.PointIncKbn)%>)
																		<br/>（現在の設定：<%#: this.UserPointRelatedThisOrderOld.Items[Container.ItemIndex].Point %>pt）
																	</td>
																	<td class="edit_item_bg" align="left" colspan="3">
																		<asp:TextBox id="tbOrderPointAdd" runat="server" Text="<%# Item.Point %>" Width="120" Enabled="<%# (cbApplyAutoCalculation.Checked == false) %>" ></asp:TextBox>pt&nbsp;&nbsp;
																		(<%#: ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_TYPE, Item.PointType) %>&nbsp;/
																		<%#: ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_KBN, Item.PointKbn) %>)
																	</td>
																</tr>
																<%-- 枝番 --%>
																<tr style="display:none">
																	<asp:HiddenField ID="hfPointKbnNo" runat="server" Value="<%# Item.PointKbnNo %>" />
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<%-- 会員ユーザの場合表示--%>
														<tr id="trReCalculatePoint" runat="server">
															<td class="edit_item_bg" align="right" colspan="4">
																<asp:Button ID="btnReCalculatePoint" runat="server" Width="150" Text="  利用ポイントを反映する  " onclick="btnReCalculate_Click" UseSubmitBehavior="false"/>
															</td>
														</tr>
														<%-- 非会員ユーザの場合表示--%>
														<tr visible="<%# this.IsUser == false %>" runat="server">
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:Label ID="lbNotUsePoint" runat="server" Text="会員ユーザではないため、ポイントはご利用できません。" ForeColor="red"></asp:Label>
															</td>
														</tr>
													</tbody>
												</table>
	
												<!-- △ポイント情報△ -->
												</div>
												<%} %>
												
												<%-- クーポンオプションが有効の場合--%>
												<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
												<div id="divCoupon" runat="server">
												<br />
												<!-- ▽クーポン情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr id="trOrderCouponErrorMessagesTitle" runat="server" visible="false">
															<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
														</tr>
														<tr id="trOrderCouponErrorMessages" runat="server" visible="false">
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:Label ID="lbOrderCouponErrorMessages" runat="server" ForeColor="red"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="center" width="718" colspan="4">クーポン情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">クーポンコード</td>
															<td class="edit_item_bg" align="left" width="25%">
																<asp:TextBox ID="tbOrderCouponCode" Text='<%# (this.OrderInput.Coupon != null) ? this.OrderInput.Coupon.CouponCode : "" %>' runat="server" Width="150"></asp:TextBox></td>
															<td class="edit_title_bg" align="left" width="25%">クーポン割引額</td>
															<td class="edit_item_bg" align="left" width="25%">
																<asp:TextBox ID="tbOrderCouponUse" Text='<%# this.OrderInput.OrderCouponUse .ToPriceString() %>' runat="server" Width="150" Enabled="<%# (cbApplyAutoCalculation.Checked == false) %>"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">クーポン名(ユーザ表示用)</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbOrderCouponDispName" Text='<%# (this.OrderInput.Coupon != null) ? this.OrderInput.Coupon.CouponDispName : "" %>' runat="server" Width="150"></asp:TextBox></td>
															<td class="edit_title_bg" align="left">クーポン名(管理用)</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbOrderCouponName" Text='<%# (this.OrderInput.Coupon != null) ? this.OrderInput.Coupon.CouponName : "" %>' runat="server" Width="150"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="right" colspan="4">
																<asp:Button ID="btnReCalculateCoupon" runat="server" Text="  適用する  " onclick="btnReCalculate_Click" UseSubmitBehavior="false"/></td>
														</tr>
													</tbody>
												</table>
												<!-- △クーポン情報△ -->
												</div>
												<%} %>
												
												<!-- ▽決済情報▽ -->
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="6" width="758" border="0">

													<tbody id="tbdyPaymentErrorMessages" visible="false" runat="server">
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="2">
																<asp:Label ID="lbPaymentErrorMessages" runat="server" ForeColor="red"></asp:Label>
															</td>
														</tr>
													</tbody>
													<tbody id="tbdyPaymentNoticeMessage" runat="server" visible="false">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">注意喚起</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left" colspan="2">
															<p><asp:Label ID="lbPaymentAlertMessage" runat="server" ForeColor="red" /></p>
															<p><asp:Label ID="lbPaymentUserManagementLevelAlertMessage" runat="server" ForeColor="red" /></p>
															<p><asp:Label ID="lbPaymentOrderOwnerKbnAlertMessage" runat="server" ForeColor="red" /></p>
														</td>
													</tr>
													</tbody>
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">決済情報</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">決済種別</td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList ID="ddlOrderPaymentKbn" runat="server" OnSelectedIndexChanged="ddlOrderPaymentKbn_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><br/>
															<asp:Literal id="lOrderPaymentInfo" runat="server"></asp:Literal>
															<%--▼▼ クレジット Token保持用 ▼▼--%>
															<asp:HiddenField ID="hfCreditToken" runat="server" />
															<asp:HiddenField ID="hfLastFourDigit" runat="server" />
															<%--▲▲ クレジット Token保持用 ▲▲--%>
															<%--▼▼ PayTg 端末状態保持用 ▼▼--%>
															<asp:HiddenField ID="hfCanUseDevice" runat="server" />
															<asp:HiddenField ID="hfStateMessage" runat="server" />
															<%--▲▲ PayTg 端末状態保持用 ▲▲--%>
														</td>
														
													</tr>
													<%-- ▽クレジット決済の場合は表示▽ --%>
													<tbody id="tbdyPaymentKbnCredit" runat="server">
													<tr>
														<td class="edit_title_bg" style="width: 129px;">利用クレジットカード</td>
														<td class="edit_item_bg"><asp:DropDownList ID="ddlUserCreditCard" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlUserCreditCard_SelectedIndexChanged"></asp:DropDownList></td>
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
													</div>
													<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

													<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
													<div id="divCreditCardForTokenAcquired" visible="false" runat="server">
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
														<td class="edit_item_bg" align="left"><asp:DropDownList id="dllCreditInstallments" runat="server" OnSelectedIndexChanged="dllCreditInstallments_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>&nbsp;&nbsp;※AMEX/DINERSは一括のみとなります。</td>
													</tr>
													<%-- △クレジット決済の場合は表示△ --%>
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
													<%-- △Gmo convenience type△ --%>
													<tbody runat="server" id="tbdGmoCvsType">
													<tr>
														<td class="edit_title_bg">支払いコンビニ選択</td>
														<td class="edit_item_bg"><asp:DropDownList ID="ddlGmoCvsType" runat="server"></asp:DropDownList></td>
													</tr>
													</tbody>
													<tr>
														<td class="edit_title_bg" align="left">決済取引ID</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbOrderCardTranId" runat="server" Text='<%# WebSanitizer.HtmlEncodeChangeToBr(this.OrderInput.CardTranId) %>' TextMode="MultiLine" Width="280" Rows="3" Enabled="<%# (Constants.CARDTRANIDOPTION_ENABLED) %>"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">決済注文ID</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbPaymentOrderId" runat="server" Text='<%# WebSanitizer.HtmlEncodeChangeToBr(this.OrderInput.PaymentOrderId) %>' MaxLength="50" Width="280"></asp:TextBox>
														</td>
													</tr>
												</table>
												<!-- △決済情報△ -->

												<%-- Taiwan Order Invoice--%>
												<% if (OrderCommon.DisplayTwInvoiceInfo(this.OrderInput.Shippings[0].ShippingCountryIsoCode)) { %>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody id="tbdyInvoiceErrorMessages" visible="false" runat="server">
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:Label ID="lbInvoiceErrorMessages" runat="server" ForeColor="red"></asp:Label>
															</td>
														</tr>
													</tbody>
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">電子発票</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">發票番号</td>
															<td class="edit_item_bg" align="left" width="75%" colspan="3">
																<%#: (this.TwOrderInvoiceInput != null) ? this.TwOrderInvoiceInput.TwInvoiceNo : string.Empty %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">ステータス</td>
															<td class="edit_item_bg" align="left" width="75%" colspan="3">
																<%#: (this.TwOrderInvoiceInput != null) ? ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS, this.TwOrderInvoiceInput.TwInvoiceStatus) : string.Empty %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">發票種類</td>
															<td class="edit_item_bg" align="left" width="75%" colspan="3">
																<asp:DropDownList ID="ddlUniformInvoice" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUniformInvoice_SelectedIndexChanged" ></asp:DropDownList>
																<asp:DropDownList
																	ID="ddlUniformInvoiceOrCarryTypeOption" runat="server"
																	DataTextField="text"
																	DataValueField="value" AutoPostBack="true" OnSelectedIndexChanged="ddlUniformInvoiceOrCarryTypeOption_SelectedIndexChanged" Visible="false">
																</asp:DropDownList>
															</td>
														</tr>
														<% if (this.IsPersonal) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">共通性載具</td>
															<td class="edit_item_bg" align="left" width="75%" colspan="3">
																<asp:DropDownList ID="ddlCarryType" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlCarryType_SelectedIndexChanged" ></asp:DropDownList>
																<asp:TextBox ID="tbCarryTypeOption1" runat="server" Visible="false" placeholder="例:/AB201+9(限8個字)" MaxLength="8"></asp:TextBox>
																<asp:TextBox ID="tbCarryTypeOption2" runat="server" Visible="false" placeholder="例:TP03000001234567(限16個字)" MaxLength="16"></asp:TextBox>
															</td>
															
														</tr>
														<% } %>
														<% if (this.IsCompany) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">統一編号</td>
															<td class="edit_item_bg" align="left" width="75%" colspan="3">
																<asp:TextBox ID="tbCompanyOption1" runat="server" MaxLength="8" placeholder="例:12345678"></asp:TextBox>
																<asp:Label ID="lbCompanyOption1" Visible="false" runat="server"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">会社名</td>
															<td class="edit_item_bg" align="left" width="75%" colspan="3">
																<asp:TextBox ID="tbCompanyOption2" runat="server" placeholder="例:○○有限股份公司" MaxLength="20"></asp:TextBox>
																<asp:Label ID="lbCompanyOption2" Visible="false" runat="server"></asp:Label>
															</td>
														</tr>
														<% } %>
														<% if (this.IsDonate) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">寄付先コード</td>
															<td class="edit_item_bg" align="left" width="75%" colspan="3">
																<asp:TextBox ID="tbDonateOption1" runat="server" MaxLength="7" Width="15%"></asp:TextBox>
																<asp:Label ID="lbDonateOption1" Visible="false" runat="server"></asp:Label>
															</td>
														</tr>
														<% } %>
													</tbody>
												</table>
												<% } %>

												<%-- アフィリエイトオプションが有効の場合--%>
												<%if (Constants.W2MP_AFFILIATE_OPTION_ENABLED) { %>
												<div id="divAffiliateOP" runat="server">
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr id="trOrderAffiliateErrorMessagesTitle" runat="server" visible="false">
															<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
														</tr>
														<tr id="trOrderAffiliateErrorMessages" runat="server" visible="false">
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:Label ID="lbOrderAffiliateErrorMessages" runat="server" ForeColor="red"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">アフィリエイト情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">広告コード（初回分）</td>
															<td class="edit_item_bg" align="left" width="75%">
																<asp:TextBox id="tbAdvCodeFirst" Text='<%# this.OrderInput.AdvcodeFirst %>' runat="server" MaxLength="30" Width="150"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">広告コード（最新分）</td>
															<td class="edit_item_bg" align="left" width="75%">
																<asp:TextBox id="tbAdvCodeNew" Text='<%# this.OrderInput.AdvcodeNew %>' runat="server" MaxLength="30" Width="150"></asp:TextBox></td>
														</tr>
													</tbody>
												</table>
												</div>
												<%} %>
												<br />
												<!-- ▽コンバージョン情報▽ -->
												<div>
													<br />
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">コンバージョン情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">流入コンテンツタイプ</td>
															<td class="edit_item_bg" align="left" width="75%">
																<asp:DropDownList ID="ddlInflowContentsType" runat="server"></asp:DropDownList></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">流入コンテンツID</td>
															<td class="edit_item_bg" align="left" width="75%">
																<asp:TextBox id="tbInflowContentsId" Text='<%# this.OrderInput.InflowContentsId %>' runat="server" MaxLength="30" Width="150"></asp:TextBox></td>
														</tr>
														</tbody>
													</table>
												</div>
												<br />
												<!-- △コンバージョン情報△ -->
												<%--▽ 注文拡張項目 ▽--%>
												<% if (Constants.ORDER_EXTEND_OPTION_ENABLED) { %>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">注文拡張項目</td>
													</tr>
													<tr id="trOrderExtendErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="3">エラーメッセージ</td>
													</tr>
													<tr id="trOrderExtendErrorMessages" runat="server">
														<td class="edit_item_bg" align="left" colspan="3">
															<asp:Label ID="lbOrderExtendErrorMessages" runat="server" ForeColor="red"></asp:Label>
														</td>
													</tr>
													<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server">
														<ItemTemplate>
															<tr>
																<td class="detail_title_bg" width="120">
																<%-- 項目名 --%>
																<%#: Item.SettingModel.SettingName %>
																</td>
																	<%-- TEXT --%>
																<td class="detail_item_bg" align="left" width="518" runat="server" visible="<%# Item.SettingModel.IsInputTypeText%>" >
																	<asp:TextBox runat="server" ID="tbSelect" Width="250px" MaxLength="100"></asp:TextBox>
																</td>
																<%-- DDL --%>
																<td class="detail_item_bg" align="left" width="518" runat="server" visible="<%# Item.SettingModel.IsInputTypeDropDown %>">
																	<asp:DropDownList runat="server" ID="ddlSelect"></asp:DropDownList>
																</td>
																<%-- RADIO --%>
																<td class="detail_item_bg" align="left" width="518" runat="server" visible="<%# Item.SettingModel.IsInputTypeRadio %>">
																	<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
																</td>
																<%-- CHECK --%>
																<td class="detail_item_bg" align="left" width="518" runat="server" visible="<%# Item.SettingModel.IsInputTypeCheckBox %>">
																	<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="checkBox"></asp:CheckBoxList>
																</td>
															</tr>
															<%-- 検証文言 --%>
															<asp:Label runat="server" ID="lbErrMessage" CssClass="error_inline"></asp:Label>
															<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingModel.SettingId %>" />
															<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.SettingModel.InputType %>" />
														</ItemTemplate>
													</asp:Repeater>
												</table>
												<br />
												<% } %>
												<%--△ 注文拡張項目 △--%>
												<!-- ▽注文メモ情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">
																注文メモ
																<uc:FieldMemoSetting runat="server" Title="注文メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_MEMO %>" />
															</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:TextBox id="tbMemo" runat="server" Text='<%# this.OrderInput.Memo %>' TextMode="MultiLine" Rows="8" Width="99%"></asp:TextBox>
															</td>
														</tr>
													</tbody>
												</table>
												<!-- △注文メモ情報△ -->
												<br />
												<!-- ▽決済連携メモ情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">
																決済連携メモ
																<uc:FieldMemoSetting runat="server" Title="決済連携メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_PAYMENT_MEMO %>" />
															</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:TextBox id="tbPaymentMemo" Text='<%# this.OrderInput.PaymentMemo %>' runat="server" TextMode="MultiLine" Rows="8" Width="99%"></asp:TextBox>
															</td>
														</tr>
													</tbody>
												</table>
												<!-- △決済連携メモ情報△ -->
												<br />
												<!-- ▽管理メモ情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">
																管理メモ
																<uc:FieldMemoSetting runat="server" Title="管理メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_MANAGEMENT_MEMO %>" />
															</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:TextBox id="tbManagementMemo" Text='<%# this.OrderInput.ManagementMemo %>' runat="server" TextMode="MultiLine" Rows="8" Width="99%"></asp:TextBox>
															</td>
														</tr>
													</tbody>
												</table>
												<!-- △管理メモ情報△ -->
												<br />
												<!-- ▽配送メモ情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
													<tr>
														<td class="edit_title_bg" align="center" colspan="4">
															配送メモ
															<uc:FieldMemoSetting runat="server" Title="配送メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_SHIPPING_MEMO %>" />
														</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left" colspan="4">
															<asp:TextBox id="tbShippingMemo" Text='<%# this.OrderInput.ShippingMemo %>' runat="server" TextMode="MultiLine" Rows="8" Width="99%"></asp:TextBox>
														</td>
													</tr>
													</tbody>
												</table>
												<!-- △配送メモ情報△ -->
												<br />
												<!-- ▽外部連携メモ情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">
																外部連携メモ
																<uc:FieldMemoSetting runat="server" Title="外部連携メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_RELATION_MEMO %>" />
															</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="4">
																<asp:TextBox id="tbRelationMemo" Text='<%# this.OrderInput.RelationMemo %>' runat="server" TextMode="MultiLine" Rows="8" Width="99%"></asp:TextBox>
															</td>
														</tr>
													</tbody>
												</table>
												<!-- △外部連携メモ情報△ -->
												<div class="action_part_bottom">
													<asp:HiddenField ID="hfPayTgSendId" runat="server" />
													<asp:HiddenField ID="hfPayTgPostData" runat="server" />
													<asp:HiddenField ID="hfPayTgResponse" runat="server" />
													<asp:Button ID="btnProcessPayTgResponse" runat="server" style="display: none;" OnClick="btnProcessPayTgResponse_Click" />
													<asp:Button ID="btnBackDetailBottom" runat="server" Text="  詳細へ戻る  " onclick="btnBackDetail_Click" UseSubmitBehavior="false" />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" OnClientClick="if(CheckStoreExist() == false) {return;} doPostbackEvenIfCardAuthFailed=false;" UseSubmitBehavior="false" />
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
	<!--△ 登録 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<%--Make ViewState not effect--%>
<asp:UpdatePanel runat="server">
	<ContentTemplate>
		<asp:HiddenField id="hfMessageConfirm" Value="" runat="server"/>
		<asp:HiddenField id="hfDiscountMessageConfirm" Value="" runat="server"/>
		<asp:LinkButton id="lbOrderConfirm" runat="server" OnClick="lbOrderConfirm_Click"></asp:LinkButton>
	</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
	<!--
	var isCheckStore = false;
	$(document).ready(function () {

		displayMemoPopup();
		if (($('#' + '<%= hfMessageConfirm.ClientID%>').val() == '')
			&&  ($('#' + '<%= hfDiscountMessageConfirm.ClientID%>').val() == '')) return;
		if (ShowConfirm($('#' + '<%= hfMessageConfirm.ClientID%>').val()) &&
			ShowConfirm($('#' + '<%= hfDiscountMessageConfirm.ClientID%>').val())) 
		{
			__doPostBack("<%= lbOrderConfirm.UniqueID %>", "");
		}
		$('#' + '<%= hfMessageConfirm.ClientID%>').val('');
		$('#' + '<%= hfDiscountMessageConfirm.ClientID%>').val('');
	});

	$(window).on('load', function(){
		<% if (this.HasErrorOnPostback) { %>
		setTimeout(function (e) {
			scrollPositionOnError();
		}, 0);
		<% } %>
	});

	// Show confirm message
	function ShowConfirm(message)
	{
		if(message == '') return true;

		message = message.replace(/(&lt;br \/&gt;)/g, "\n");
		return confirm(message);
	}

	// Set Event Change To Text Box Of Class owner-input
	$(".owner-input input[type=text]").change(function () {
		setOrderShippingWithOrderOwner();
	});

	// Set Event Change To Dropdown List Of Class owner-input
	$(".owner-input select").change(function () {
		setOrderShippingWithOrderOwner();
	});

	// Set Order Shipping With Order Owner
	function setOrderShippingWithOrderOwner()
	{
		var userShipping = $(".UserShipping option:selected").val();
		if(userShipping != "OWNER") return;

		var ownerName1 = $("#<%= tbOwnerName1.ClientID %>").val();
		var ownerName2 = $("#<%= tbOwnerName2.ClientID %>").val();
		var ownerNameKana1 = $("#<%= tbOwnerNameKana1.ClientID %>").val();
		var ownerNameKana2 = $("#<%= tbOwnerNameKana2.ClientID %>").val();
		var ownerZip1_1 = $("#<%= tbOwnerZip1_1.ClientID %>").val();
		var ownerZip1_2 = $("#<%= tbOwnerZip1_2.ClientID %>").val();
		var ownerAddr1 = $("#<%= ddlOwnerAddr1.ClientID %>").val();
		var ownerAddr2 = $("#<%= tbOwnerAddr2.ClientID %>").val();
		var ownerAddr3 = $("#<%= tbOwnerAddr3.ClientID %>").val();
		var ownerAddr4 = $("#<%= tbOwnerAddr4.ClientID %>").val();
		var ownerAddr5 = $("#<%= tbOwnerAddr5.ClientID %>").val();
		var ownerTel1_1 = $("#<%= tbOwnerTel1_1.ClientID %>").val();
		var ownerTel1_2 = $("#<%= tbOwnerTel1_2.ClientID %>").val();
		var ownerTel1_3 = $("#<%= tbOwnerTel1_3.ClientID %>").val();
		var ownerTelGlobal = $("#<%= tbOwnerTel1Global.ClientID %>").val();
		var ownerZipGlobal = $("#<%= tbOwnerZipGlobal.ClientID %>").val();
		var ownerCompanyName = $("#<%= tbOwneCompanyName.ClientID %>").val();
		var ownerCompanyPostName = $("#<%= tbOwnerCompanyPostName.ClientID %>").val();
		var ownerCountry = $("#<%= ddlOwnerCountry.ClientID %> option:selected");
		var isCountryJp = (ownerCountry.val() == '<%= Constants.COUNTRY_ISO_CODE_JP %>');

		// Declare Control
		var tdShippingName = (document.querySelector('[id$="tdShippingName"]') != null)
			? document.querySelector('[id$="tdShippingName"]')
			: document.querySelector('[id$="tdShippingNameGlobal"]');
		var tdShippingZip = (document.querySelector('[id$="tdShippingZip"]') != null)
			? document.querySelector('[id$="tdShippingZip"]')
			: document.querySelector('[id$="tdShippingZipGlobal"]');
		var tdShippingAddr1 = document.querySelector('[id$="tdShippingAddr1"]');
		var tdShippingAddr2 = document.querySelector('[id$="tdShippingAddr2"]');
		var tdShippingAddr3 = document.querySelector('[id$="tdShippingAddr3"]');
		var tdShippingAddr4 = document.querySelector('[id$="tdShippingAddr4"]');
		var tdShippingAddr5 = document.querySelector('[id$="tdShippingAddr5"]');
		var tdShippingTel1 = document.querySelector('[id$="tdShippingTel1"]');
		var tdShippingCountryName = document.querySelector('[id$="tdShippingCountryName"]');
		var tdShippingCompanyName = document.querySelector('[id$="tdShippingCompanyName"]');
		var tdShippingCompanyPostName = document.querySelector('[id$="tdShippingCompanyPostName"]');

		// Set values
		var ownerName = ((ownerNameKana1 + ownerNameKana2).length == 0) || (isCountryJp == false)
			? ownerName1 + ownerName2
			: ownerName1 + ownerName2 + "（" + ownerNameKana1 + ownerNameKana2 + "）";
		tdShippingName.innerHTML = ownerName;
		tdShippingAddr2.innerHTML = ownerAddr2;
		tdShippingAddr3.innerHTML = ownerAddr3;
		tdShippingAddr4.innerHTML = ownerAddr4;
		tdShippingTel1.innerHTML = ((ownerTel1_1 + ownerTel1_2 + ownerTel1_3).length > 0)
			? ownerTel1_1 + "-" + ownerTel1_2 + "-" + ownerTel1_3
			: "";
		tdShippingCountryName.innerHTML= ownerCountry.html();
		if((tdShippingCompanyName != null)
			&& (tdShippingCompanyPostName != null))
		{
			tdShippingCompanyName.innerHTML = ownerCompanyName;
			tdShippingCompanyPostName.innerHTML = ownerCompanyPostName;
		}

		if (isCountryJp)
		{
			if(tdShippingAddr1 != null)
			{
				tdShippingAddr1.innerHTML = (ownerAddr1 != undefined)
					? ownerAddr1
					: "";
			}
			tdShippingZip.innerHTML= ((ownerZip1_1 + ownerZip1_2).length > 0)
				? ownerZip1_1 + "-" + ownerZip1_2
				: "";
		}
		else
		{
			if(tdShippingAddr5 != null)
			{
				ownerAddr5 = (ownerAddr5 != undefined)
					? ownerAddr5
					: $("#<%= ddlOwnerAddr5.ClientID %>").val();
				tdShippingAddr5.innerHTML = ownerAddr5;
			}
			tdShippingZip.innerHTML = ownerZipGlobal;
			tdShippingTel1.innerHTML = ownerTelGlobal;
		}
	}

	$(document.getElementsByClassName('UserShipping')).change(function () {
		InitializeOrderShipping();
	});

	InitializeOrderShipping();
	function InitializeOrderShipping()
	{
		var userShippingKbn = $(document.getElementsByClassName('UserShipping')).val();
		$('#dvErrorShippingConvenience').css("display", "none");

		var btnCvsSearch = $(document.getElementsByClassName('CvsSearch'));
		var tbodyUserShippingNormal = $(document.getElementsByClassName('UserShippingNormal'));
		var tbodyConvenienceStore = $(document.getElementsByClassName('CONVENIENCE_STORE'));
		var trShippingTime1 = $(document.getElementsByClassName('trShippingTime1'));
		var trShippingTime2 = $(document.getElementsByClassName('trShippingTime2'));
		var ddlOrderPayment = $("<%= ddlOrderPaymentKbn.ClientID %>");
		var paymentConvenience = "<%= Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE %>";
		var ddlShippingReceivingStoreType = $(document.getElementsByClassName('ShippingReceivingStoreType'));

		var element = document.getElementsByClassName('ShippingAddress');
		$(element).hide();
		switch(userShippingKbn)
		{
			case 'NEW':
				$('.INPUT_NEW').show();
				btnCvsSearch.hide();
				trShippingTime1.show();
				trShippingTime2.show();
				isCheckStore = false;
				$("#<%= ddlOrderPaymentKbn.ClientID %> option[value=" + paymentConvenience + "]").hide();
			<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
			ddlShippingReceivingStoreType.hide();
			<% } %>
			break;

		case 'CONVENIENCE_STORE':
			$('.CONVENIENCE').show();
			trShippingTime1.hide();
			trShippingTime2.hide();
			btnCvsSearch.show();
			$("#<%= ddlOrderPaymentKbn.ClientID %> option[value=" + paymentConvenience + "]").show();
			isCheckStore = true;
			<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
			ddlShippingReceivingStoreType.show();
			<% } %>
			break;

		case 'OWNER':
			$('.OWNER').show();
			trShippingTime1.show();
			trShippingTime2.show();
			btnCvsSearch.hide();
			$("#<%= ddlOrderPaymentKbn.ClientID %> option[value=" + paymentConvenience + "]").hide();
			isCheckStore = false;
			setOrderShippingWithOrderOwner();
			<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
			ddlShippingReceivingStoreType.hide();
			<% } %>
			break;

		default:
			var shippingNoCovenience = '<%= string.Join(",", this.UserShippingAddress.Where(item => (item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)).Select(item => item.ShippingNo)) %>';
			if(shippingNoCovenience.split(",").indexOf(userShippingKbn) != -1) {
				btnCvsSearch.show();
				$("#<%= ddlOrderPaymentKbn.ClientID %> option[value=" + paymentConvenience + "]").show();
				isCheckStore = true;
				$('.trShippingTime1').hide();
				$('.trShippingTime2').hide();
				var elements = document.getElementsByClassName('shipping_no_' + userShippingKbn)[0];
				$("#<%=hfSelectedShopId.ClientID %>").val(elements.querySelector('[id$="hfCvsShopId"]').value);
				<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
				ddlShippingReceivingStoreType.show();
				<% } %>
			}
			else {
				btnCvsSearch.hide();
				$("#<%= ddlOrderPaymentKbn.ClientID %> option[value=" + paymentConvenience + "]").hide();
				isCheckStore = false;
				$('.trShippingTime1').show();
				$('.trShippingTime2').show();
				<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
				ddlShippingReceivingStoreType.hide();
				<% } %>
			}
			var className = 'shipping_no_' + userShippingKbn;
			$('.'+ className).show();
			break;
	}
}

	<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
	<%-- Open convenience store map popup --%>
	function openConvenienceStoreMapPopup() {
		var url = '<%= OrderCommon.CreateConvenienceStoreMapUrl() %>';
		window.open(url, "", "width=1000,height=800");
	}

	<%-- Set convenience store data --%>
	function setConvenienceStoreData(cvsspot, name, addr, tel) {
		var userShippingKbn = $(document.getElementsByClassName('UserShipping')).val();
		var elements = document.getElementsByClassName('CONVENIENCE')[0];
		if(userShippingKbn != 'CONVENIENCE_STORE')
		{
			elements = document.getElementsByClassName('shipping_no_' + userShippingKbn)[0];
		}

		// For display
		if(userShippingKbn == 'CONVENIENCE_STORE')
		{
			elements.querySelector('[id$="tdCvsShopId"] > span').innerHTML = cvsspot;
			elements.querySelector('[id$="tdCvsShopName"] > span').innerHTML = name;
			elements.querySelector('[id$="tdCvsShopAddress"] > span').innerHTML = addr;
			elements.querySelector('[id$="tdCvsShopTel"] > span').innerHTML = tel;

			// For get value
			$("#<%=hfCvsShopId.ClientID %>").val(cvsspot);
			$("#<%=hfCvsShopName.ClientID %>").val(name);
			$("#<%=hfCvsShopAddress.ClientID %>").val(addr);
			$("#<%=hfCvsShopTel.ClientID %>").val(tel);
		}
		else
		{
			elements.querySelector('[id$="tdCvsShopId"] > span').innerHTML = cvsspot;
			elements.querySelector('[id$="tdCvsShopName"] > span').innerHTML = name;
			elements.querySelector('[id$="tdCvsShopAddress"] > span').innerHTML = addr;
			elements.querySelector('[id$="tdCvsShopTel"] > span').innerHTML = tel;

			elements.querySelector('[id$="hfCvsShopId"]').value = cvsspot;
			elements.querySelector('[id$="hfCvsShopName"]').value = name;
			elements.querySelector('[id$="hfCvsShopAddress"]').value = addr;
			elements.querySelector('[id$="hfCvsShopTel"]').value = tel;
		}

		$("#<%=hfSelectedShopId.ClientID %>").val(cvsspot);
	}
	<% } %>

	<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
	<%-- Set convenience store Ec pay data --%>
	function setConvenienceStoreEcPayData(cvsspot, name, addr, tel) {
		if(cvsspot != "")
		{
			$("#<%=hfSelectedShopId.ClientID %>").val(cvsspot);
			var elements = document.getElementsByClassName('CONVENIENCE')[0];

			// For display
			elements.querySelector('[id$="tdCvsShopId"] > span').innerHTML = cvsspot;
			elements.querySelector('[id$="tdCvsShopName"] > span').innerHTML = name;
			elements.querySelector('[id$="tdCvsShopAddress"] > span').innerHTML = addr;
			elements.querySelector('[id$="tdCvsShopTel"] > span').innerHTML = tel;

			// For get value
			$("#<%= hfCvsShopId.ClientID %>").val(cvsspot);
			$("#<%= hfCvsShopName.ClientID %>").val(name);
			$("#<%= hfCvsShopAddress.ClientID %>").val(addr);
			$("#<%= hfCvsShopTel.ClientID %>").val(tel);
		}
	}
	<% } %>

	<%-- Check Store Exist --%>
	function CheckStoreExist()
	{
		if(isCheckStore == false) return true;
		var shopId = $("#<%=hfSelectedShopId.ClientID %>").val();
		var shopValid = true;
		$.ajax({
			type: "POST",
			url: "<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_MODIFY_INPUT %>/CheckStoreIdValid",
			data: JSON.stringify({ storeId: shopId }),
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			cache: false,
			async: false,
			success: function (data) {
				if (data.d) {
					$('#dvErrorShippingConvenience').css("display", "none");
				}
				else {
					shopValid = false;
					$('#dvErrorShippingConvenience').css("display", "");
					$([document.documentElement, document.body]).animate({
						scrollTop: $("#<%=ddlUserManagementLevel.ClientID %>").offset().top
					}, 2000);
				}
			}
		});

		return shopValid;
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
	//-->

	execAutoKanaWithKanaType(
		$("#<%= tbOwnerName1.ClientID %>"),
		$("#<%= tbOwnerNameKana1.ClientID %>"),
		$("#<%= tbOwnerName2.ClientID %>"),
		$("#<%= tbOwnerNameKana2.ClientID %>"));

	<% foreach (RepeaterItem ri in rShippingList.Items) { %>
	execAutoKanaWithKanaType(
		$("#<%= ((TextBox)ri.FindControl("tbShippingName1")).ClientID%>"),
		$("#<%= ((TextBox)ri.FindControl("tbShippingNameKana1")).ClientID%>"),
		$("#<%= ((TextBox)ri.FindControl("tbShippingName2")).ClientID%>"),
		$("#<%= ((TextBox)ri.FindControl("tbShippingNameKana2")).ClientID%>"));

	execAutoKanaWithKanaType(
		$("#<%= ((TextBox)ri.FindControl("tbSenderName1")).ClientID%>"),
		$("#<%= ((TextBox)ri.FindControl("tbSenderNameKana1")).ClientID%>"),
		$("#<%= ((TextBox)ri.FindControl("tbSenderName2")).ClientID%>"),
		$("#<%= ((TextBox)ri.FindControl("tbSenderNameKana2")).ClientID%>"));
	<% } %>

	<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
	// Textbox change search owner zip global
	textboxChangeSearchGlobalZip(
		'<%= tbOwnerZipGlobal.ClientID %>',
		'<%= lbSearchAddrFromOwnerZipGlobal.UniqueID %>');

	// Textbox change search shipping zip global
	textboxChangeSearchGlobalZip(
		document.querySelector('[id$="tbShippingZipGlobal"]').id,
		document.querySelector('[id$="lbSearchAddrFromShippingZipGlobal"]').id.replaceAll('_', '$'));

	// Textbox change search sender zip global
	textboxChangeSearchGlobalZip(
		document.querySelector('[id$="tbSenderZipGlobal"]').id,
		document.querySelector('[id$="lbSearchAddrFromSenderZipGlobal"]').id.replaceAll('_', '$'));
	<% } %>

	<%-- エラー発生時、エラー表示位置にスクロール --%>
	function scrollPositionOnError() {
		var lbOrderItemErrorMessages = document.getElementById('<%= GetControlIdByRepeaterItem("lbOrderItemErrorMessages") %>')
		var lbOrderShippingErrorMessages = document.getElementById('<%= GetControlIdByRepeaterItem("lbOrderShippingErrorMessages") %>')
		if($('#<%= lbOrderErrorMessages.ClientID %>').length > 0)
		{
			$(window).scrollTop(($('#<%= lbOrderErrorMessages.ClientID %>').offset().top - 100));
		}
		else if($('#<%= lbOrderOwnerErrorMessages.ClientID %>').length > 0)
		{
			$(window).scrollTop(($('#<%= lbOrderOwnerErrorMessages.ClientID %>').offset().top - 100));
		}
		else if($('#<%= lbReceiptErrorMessages.ClientID %>').length > 0)
		{
			$(window).scrollTop(($('#<%= lbReceiptErrorMessages.ClientID %>').offset().top - 100));
		}
		else if(lbOrderItemErrorMessages != null)
		{
			$(window).scrollTop(($(lbOrderItemErrorMessages).offset().top - 100));
		}
		else if(lbOrderShippingErrorMessages != null)
		{
			$(window).scrollTop(($(lbOrderShippingErrorMessages).offset().top - 100));
		}
		else if($('#<%= lbOrderPriceErrorMessages.ClientID %>').length > 0)
		{
			$(window).scrollTop(($('#<%= lbOrderPriceErrorMessages.ClientID %>').offset().top - 100));
		}
		else if($('#<%= lbOrderPointErrorMessages.ClientID %>').length > 0)
		{
			$(window).scrollTop(($('#<%= lbOrderPointErrorMessages.ClientID %>').offset().top - 100));
		}
		else if($('#<%= lbPaymentErrorMessages.ClientID %>').length > 0)
		{
			$(window).scrollTop(($('#<%= lbPaymentErrorMessages.ClientID %>').offset().top - 100));
		}
		else if($('#<%= lbInvoiceErrorMessages.ClientID %>').length > 0)
		{
			$(window).scrollTop(($('#<%= lbInvoiceErrorMessages.ClientID %>').offset().top - 100));
		}
		else if($('#<%= lbOrderCouponErrorMessages.ClientID %>').length > 0)
		{
			$(window).scrollTop(($('#<%= lbOrderCouponErrorMessages.ClientID %>').offset().top - 100));
		}
		else if($('#<%= lbOrderAffiliateErrorMessages.ClientID %>').length > 0)
		{
			$(window).scrollTop(($('#<%= lbOrderAffiliateErrorMessages.ClientID %>').offset().top - 100));
		}
		else if($('#<%= lbOrderExtendErrorMessages.ClientID %>').length > 0)
		{
			$(window).scrollTop(($('#<%= lbOrderExtendErrorMessages.ClientID %>').offset().top - 100));
		}
}
</script>

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

</asp:Content>
