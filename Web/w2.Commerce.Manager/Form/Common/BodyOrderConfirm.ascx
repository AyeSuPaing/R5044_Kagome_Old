<%--
=========================================================================================================
  Module      : 受注情報出力コントローラ(BodyOrderConfirm.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BodyOrderConfirm.ascx.cs" Inherits="Form_Common_BodyOrderConfirm" %>
<%@ Import Namespace="w2.App.Common.Input.Order" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Global.Config" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.Domain.Order" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.ECPay" %>
<%@ Import Namespace="w2.Domain.InvoiceDskDeferred" %>
<%@ Import Namespace="w2.Domain.Score" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Import Namespace="w2.Domain.InvoiceVeritrans" %>
<%@ Register TagPrefix="uc" TagName="FieldMemoSetting" Src="~/Form/Common/FieldMemoSetting/BodyFieldMemoSetting.ascx" %>
<asp:Button ID="btnTooltipInfo" runat="server" style="display:none;"/>
<table width="758" border="0" cellspacing="0" cellpadding="0">
	<tr valign="top">
		<td>
			<%--▽ 基本情報 ▽--%>
			<div id="divOrder" runat="server">
			<table class="detail_table" cellspacing="1" cellpadding="3" width="480" border="0">
				<tr>
					<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left" width="40%">注文ID</td>
					<td class="detail_item_bg" align="left" width="60%">
						<%#: this.OrderInput.OrderId %></td>
				</tr>
				<%--▽ モール連携オプションまたは外部連携注文取込が有効の場合 ▽--%>
				<% if (Constants.MALLCOOPERATION_OPTION_ENABLED || Constants.URERU_AD_IMPORT_ENABLED) { %>
				<tr>
					<td class="detail_title_bg" align="left">サイト</td>
					<td class="detail_item_bg" align="left">
						<%#: BasePage.CreateSiteNameForDetail(this.OrderInput.MallId, this.OrderInput.MallName) %></td>
				</tr>
				<% } %>
				<%--△ モール連携オプションまたは外部連携注文取込が有効の場合 △--%>
				<tr>
					<td class="detail_title_bg" align="left">注文区分</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, this.OrderInput.OrderKbn) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">注文ステータス</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, this.OrderInput.OrderStatus) %><br />
					</td>
				</tr>
				<tr runat="server" visible="<%# IsStorePickUpOrder %>">
					<td class="detail_title_bg" align="left">店舗受取ステータス</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_STOREPICKUP_STATUS, this.OrderInput.StorePickupStatus) %><br />
					</td>
				</tr>
				<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
				<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
				<tr>
					<td class="detail_title_bg" align="left">引当状況</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS, this.OrderInput.OrderStockreservedStatus) %></td>
				</tr>
				<% } %>
				<%--△ 実在庫利用が有効な場合は表示 △--%>
				<tr>
					<td class="detail_title_bg" align="left">入金ステータス</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, this.OrderInput.OrderPaymentStatus) %></td>
				</tr>
				<tr id="trDemandStatus" visible="false" runat="server">
						<td class="detail_title_bg" align="left">督促ステータス</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_DEMAND_STATUS, this.OrderInput.DemandStatus) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">外部決済ステータス</td>
					<td class="detail_item_bg" align="left">
						<span style="margin-right: 25px;"><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, this.OrderInput.ExternalPaymentStatus) %></span>
						<asp:Button ID="btnGetAuth" Text="  与信情報取得  " runat="server" OnClick="btnGetAuth_Click" Visible="<%# (this.IsAuthResultHoldOrder || this.IsAuthResultPendOrder) %>"/>
					</td>
				</tr>
				<asp:Repeater ID="rOrderExtendStatusList" runat="server" ItemType="System.Data.DataRowView">
				<ItemTemplate>
					<tr>
						<td class="detail_title_bg" align="left">拡張ステータス<%#: Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO] %>：<br>
							&nbsp;<%#: Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME] %></td>
						<td class="detail_item_bg" align="left">
							<%# ValueText.GetValueText(Constants.TABLE_ORDER, OrderPage.FIELD_ORDER_EXTEND_STATUS, this.OrderInput.ExtendStatus[(int)Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO] - 1].Value) %></td>
					</tr>
				</ItemTemplate>
				</asp:Repeater>
				<% if (Constants.URERU_AD_IMPORT_ENABLED) { %>
				<tr>
					<td class="detail_title_bg" align="left">外部連携受注ID</td>
					<td class="detail_item_bg" align="left"><%#: this.OrderInput.ExternalOrderId %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">外部連携取込ステータス</td>
					<td class="detail_item_bg" align="left"><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS, this.OrderInput.ExternalImportStatus) %></td>
				</tr>
				<%} %>
				<tr>
					<td class="detail_title_bg" align="left">決済種別</td>
					<td class="detail_item_bg" align="left">
						<%#: this.OrderInput.PaymentName %>
						<span visible="<%# (string.IsNullOrEmpty(this.OrderInput.CardInstruments) == false) && (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) %>" runat="server">
						(<%#: this.OrderInput.CardInstruments %>)
						</span>
					</td>
				</tr>
				<% if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (this.UserCreditCardInfo != null)) { %>
				<tr>
					<td class="detail_title_bg" align="left">利用クレジットカード情報</td>
					<td class="detail_item_bg" align="left">
						<%= UserCreditCardHelper.CreateCreditCardInfoHtml(this.UserCreditCardInfo) %>
					</td>
				</tr>
				<%} %>
				<tr>
					<td class="detail_title_bg" align="left">決済取引ID</td>
					<td class="detail_item_bg" align="left">
						<%#: this.OrderInput.CardTranId %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">決済注文ID</td>
					<td class="detail_item_bg" align="left">
						<%#: this.OrderInput.PaymentOrderId %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">配送種別</td>
					<td class="detail_item_bg" align="left">
						[<%#: this.OrderInput.ShippingId %>]
						<%#: (this.ShopShipping != null) ? this.ShopShipping.ShopShippingName : string.Empty %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">出荷後変更区分</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN, this.OrderInput.ShippedChangedKbn) %></td>
				</tr>				
				<tr>
					<td class="detail_title_bg" align="left">リモートIPアドレス</td>
					<td class="detail_item_bg" align="left">
						<%#: this.OrderInput.RemoteAddr %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left" width="25%">購入回数（注文基準）</td>
					<td class="detail_item_bg" align="left">
						<%: (string.IsNullOrEmpty(this.OrderInput.OrderCountOrder) == false) ? this.OrderInput.OrderCountOrder + " 回目" : "" %>
					</td>
				</tr>
			</table>
			</div>
			<%--△ 基本情報 △--%>
			<%--▽ 返品交換情報 ▽--%>
			<div id="divReturnExchange" runat="server">
			<br />
			<table class="detail_table" cellspacing="1" cellpadding="3" width="480" border="0">
				<tr>
					<td class="detail_title_bg" align="center" colspan="2">返品交換情報</td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left" width="40%">元注文ID</td>
					<td class="detail_item_bg" align="left" width="60%">
						<%#: this.OrderInput.OrderIdOrg %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">返品ステータス</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS, this.OrderInput.OrderReturnExchangeStatus) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">返金ステータス</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS, this.OrderInput.OrderRepaymentStatus) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">返品交換区分</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN, this.OrderInput.ReturnExchangeKbn) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">返品交換都合区分</td>
					<td class="detail_item_bg" align="left">
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN, this.OrderInput.ReturnExchangeReasonKbn) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">返品交換理由メモ</td>
					<td class="detail_item_bg" align="left">
						<%# WebSanitizer.HtmlEncodeChangeToBr(this.OrderInput.ReturnExchangeReasonMemo) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">返金額</td>
					<td class="detail_item_bg" align="left">
						<%#: this.OrderInput.OrderPriceRepayment.ToPriceString(true) %></td>
				</tr>
				<tr id="trRepaymentMemo" runat="server">
					<td class="detail_title_bg" align="left">返金メモ</td>
					<td class="detail_item_bg" align="left">
						<%# WebSanitizer.HtmlEncodeChangeToBr(this.OrderInput.RepaymentMemo) %></td>
				</tr>
				<tbody id="tbodyRepaymentBank" visible="false" runat="server">
					<tr>
						<td class="detail_title_bg" align="center" colspan="2">返金先情報</td>
					</tr>
					<tr>
						<td class="detail_title_bg" align="left" width="40%">銀行コード</td>
						<td class="detail_item_bg" align="left" width="60%">
							<%#: this.RepaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_CODE] %>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg" align="left">銀行名</td>
						<td class="detail_item_bg" align="left">
							<%#: this.RepaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_NAME] %>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg" align="left">支店名</td>
						<td class="detail_item_bg" align="left">
							<%#: this.RepaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_BRANCH] %>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg" align="left">口座番号</td>
						<td class="detail_item_bg" align="left">
							<%#: this.RepaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NO] %>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg" align="left">口座名</td>
						<td class="detail_item_bg" align="left">
							<%#: this.RepaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NAME] %>
						</td>
					</tr>
				</tbody>
			</table>
			</div>
			<%--△ 返品交換情報 △--%>
		</td>
		<td width="20"></td>
		<td>
			<%--▽ 更新日 ▽--%>
			<div id="divOrderDate" runat="server">
			<table class="detail_table" cellspacing="1" cellpadding="3" width="258" border="0">
				<tr>
					<td class="detail_title_bg" align="center" colspan="2">更新日</td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left" width="40%">注文日時</td>
					<td class="detail_item_bg" align="left" width="60%">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">受注承認日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderRecognitionDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
				<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
				<tr>
					<td class="detail_title_bg" align="left">在庫引当日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderStockreservedDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<% } %>
				<%--△ 実在庫利用が有効な場合は表示 △--%>
				<tr>
					<td class="detail_title_bg" align="left">出荷手配日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderShippingDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">出荷完了日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderShippedDate, DateTimeUtility.FormatType.ShortDate2Letter) %>
					</td>
				</tr>
				<tr runat="server" visible="<%# IsStorePickUpOrder %>">
					<td class="detail_title_bg" align="left">店舗到着日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.StorePickupStoreArrivedDate, DateTimeUtility.FormatType.ShortDate2Letter) %>
					</td>
				</tr>
				<tr runat="server" visible="<%# IsStorePickUpOrder %>">
					<td class="detail_title_bg" align="left">返送日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.StorePickupReturnDate, DateTimeUtility.FormatType.ShortDate2Letter) %>
					</td>
				</tr>
				<tr runat="server" visible="<%# IsStorePickUpOrder %>">
					<td class="detail_title_bg" align="left">引渡し完了日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.StorePickupDeliveredCompleteDate, DateTimeUtility.FormatType.ShortDate2Letter) %>
					</td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">配送完了日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderDeliveringDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">キャンセル日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderCancelDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">入金日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderPaymentDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<tr id="trDemandDay" visible="false" runat="server">
					<td class="detail_title_bg" align="left">督促日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.DemandDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">外部決済与信日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.ExternalPaymentAuthDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
				</tr>
				<asp:Repeater ID="rOrderExtendStatusDates" runat="server" ItemType="System.Data.DataRowView">
				<ItemTemplate>
					<tr>
						<td class="edit_title_bg" align="left">拡張ステータス<%#: Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO] %>更新日</td>
						<td class="edit_item_bg" align="left">
							<%#: DateTimeUtility.ToStringForManager(this.OrderInput.ExtendStatus[(int)Item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO] - 1].Date, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
					</tr>
				</ItemTemplate>
				</asp:Repeater>
			</table>
			</div>
			<%--△ 更新日 △--%>
			<%--▽ 返品交換更新日 ▽--%>
			<div id="divReturnExchangeDate" runat="server">
			<br />
			<table class="detail_table" cellspacing="1" cellpadding="3" width="258" border="0">
				<tr>
					<td class="detail_title_bg" align="center" colspan="2">返品交換更新日</td>
				</tr>				
				<tr>
					<td class="detail_title_bg" align="left" width="40%">返品交換受付日</td>
					<td class="detail_item_bg" align="left" width="60%">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderReturnExchangeReceiptDate,  DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<tr runat="server" visible="<%# IsStorePickUpOrder && (this.OrderInput.IsReturnOrder || this.OrderInput.IsExchangeOrder) %>">
					<td class="detail_title_bg" align="left" width="40%">商品返送日</td>
					<td class="detail_item_bg" align="left" width="60%">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.StorePickupReturnDate,  DateTimeUtility.FormatType.ShortDate2Letter) %>
					</td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">返品交換商品到着日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderReturnExchangeArrivalDate,  DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left">返品交換処理完了日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderReturnExchangeCompleteDate,  DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="center" colspan="2">返金更新日</td>
				</tr>				
				<tr>
					<td class="detail_title_bg" align="left">返金日</td>
					<td class="detail_item_bg" align="left">
						<%#: DateTimeUtility.ToStringForManager(this.OrderInput.OrderRepaymentDate,  DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
			</table>
			</div>
			<%--△ 返品交換更新日 △--%>
		</td>
	</tr>
</table>
<br />
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">注文者情報</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">ユーザーID</td>
		<td class="detail_item_bg" align="left" width="25%">
			<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
				<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl(this.OrderInput.UserId)) %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
					<%#: this.OrderInput.UserId %></a>
			<% } else { %>
				<%#: this.OrderInput.UserId %>
			<% } %>
		</td>
		<td class="detail_title_bg" align="left" width="25%">注文者区分</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: ValueText.GetValueText(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_KBN, this.OrderInput.Owner.OwnerKbn) %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name.name@@") %></td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: this.OrderInput.Owner.OwnerName %>&nbsp;<%#: BasePage.GetUserSymbol(this.OrderInput.UserId) %>
		<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
		<td class="detail_item_bg" width="25%">
			<%#: this.OrderInput.Owner.OwnerNameKana %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">メールアドレス</td>
		<td class="detail_item_bg" align="left" colspan="3">
			<%#: this.OrderInput.Owner.OwnerMailAddr %></td>
	</tr>
	<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">モバイルメールアドレス</td>
		<td class="detail_item_bg" align="left" colspan="3">
			<%#: this.OrderInput.Owner.OwnerMailAddr2 %></td>
	</tr>
	<% } %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">住所</td>
		<td class="detail_item_bg" align="left" colspan="3">
			<span id="Span1" visible='<%# (this.OrderInput.Owner.OwnerZip != "") %>' runat="server">
				<%#: this.OrderInput.Owner.IsAddrJp ? "〒" + this.OrderInput.Owner.OwnerZip : "" %>
			</span>
			<%#: this.OrderInput.Owner.OwnerAddr1 %>
			<%#: this.OrderInput.Owner.OwnerAddr2 %>&nbsp;
			<%#: this.OrderInput.Owner.OwnerAddr3 %>&nbsp;
			<%#: this.OrderInput.Owner.OwnerAddr4 %>&nbsp;
			<%#: this.OrderInput.Owner.OwnerAddr5 %>&nbsp;
			<span visible='<%# (this.OrderInput.Owner.OwnerZip != "") %>' runat="server">
				<%#: (this.OrderInput.Owner.IsAddrJp == false) ? this.OrderInput.Owner.OwnerZip : "" %>
			</span>
			<%#: this.OrderInput.Owner.OwnerAddrCountryName %>
		</td>
	</tr>
	<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.company_name.name@@")%>・<%: ReplaceTag("@@User.company_post_name.name@@")%></td>
		<td class="detail_item_bg" align="left" colspan="3">
			<%#: this.OrderInput.Owner.OwnerCompanyName %>&nbsp<%#: this.OrderInput.Owner.OwnerCompanyPostName %></td>
	</tr>
	<%} %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.tel1.name@@") %></td>
		<td class="detail_item_bg" align="left" colspan="3">
			<%#: this.OrderInput.Owner.OwnerTel1 %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.sex.name@@") %></td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: ValueText.GetValueText(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_SEX, this.OrderInput.Owner.OwnerSex) %></td>
		<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.birth.name@@") %></td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: DateTimeUtility.ToStringForManager(this.OrderInput.Owner.OwnerBirth, DateTimeUtility.FormatType.LongDate2Letter) %></td>
	</tr>
	<% if (Constants.GLOBAL_OPTION_ENABLE){ %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">アクセス国ISOコード</td>
		<td class="detail_item_bg" align="left" colspan="3"><%#: this.OrderInput.Owner.OwnerAddrCountryIsoCode %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">表示言語コード</td>
		<td class="detail_item_bg" align="left" width="25%"><%#: this.OrderInput.Owner.DispLanguageCode %></td>
		<td class="detail_title_bg" align="left" width="25%">表示言語ロケールID</td>
		<td class="detail_item_bg" align="left" width="25%"><%#: GlobalConfigUtil.LanguageLocaleIdDisplayFormat(this.OrderInput.Owner.DispLanguageLocaleId) %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">表示通貨コード</td>
		<td class="detail_item_bg" align="left" width="25%"><%#: this.OrderInput.Owner.DispCurrencyCode %></td>
		<td class="detail_title_bg" align="left" width="25%">表示通貨ロケールID</td>
		<td class="detail_item_bg" align="left" width="25%"><%#: GlobalConfigUtil.CurrencyLocaleIdDisplayFormat(this.OrderInput.Owner.DispCurrencyLocaleId) %></td>
	</tr>
	<%} %>
</table>
<br />
<%-- ▽ユーザー情報▽--%>
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="3">ユーザー情報</td>
	</tr>
	<tr>
		<td class="edit_title_bg" align="left" width="120">ユーザー特記欄</td>
		<td class="edit_item_bg" align="left" colspan="2">
			<asp:TextBox id="tbUserMemo" runat="server" TextMode="MultiLine" Rows="2" Width="100%"></asp:TextBox><br />
		</td>
	</tr>
	<tr>
		<td class="edit_title_bg" align="left" width="120">ユーザー管理レベル</td>
		<td class="edit_item_bg" align="left" width="518">
			<asp:DropDownList id="ddlUserManagementLevel" runat="server" Enabled="<%# (this.IsStorePickUpOrder == false) %>" />
		</td>
		<td class="edit_item_bg" align="right" width="120">
			<asp:Button ID="btnUpdateUserInfo" Text="  ユーザー情報更新  " width="120" runat="server" OnClick="btnUpdateUserInfo_Click" />
		</td>
	</tr>
</table>
<br />
<%-- △ユーザー情報△--%>

<!-- ▽領収書情報▽ -->
<%if (Constants.RECEIPT_OPTION_ENABLED) { %>
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tbody id="tbdyReceiptErrorMessages" runat="server" visible="false">
	<tr>
		<td class="edit_title_bg" align="center" colspan="3">エラーメッセージ</td>
	</tr>
	<tr>
		<td class="edit_item_bg" align="left" colspan="3">
			<asp:Label ID="lbReceiptErrorMessages" runat="server" ForeColor="red" />
		</td>
	</tr>
	</tbody>
	<tbody>
	<tr>
		<td class="detail_title_bg" align="center" colspan="3">領収書情報</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">領収書希望</td>
		<td class="detail_item_bg" align="left" width="60%">
			<asp:RadioButtonList
				id="rblReceiptFlg"
				runat="server"
				RepeatDirection="Horizontal"
				RepeatLayout="Flow"
				OnSelectedIndexChanged="rblReceiptFlg_SelectedIndexChanged"
				AutoPostBack="True"
				Enabled="<%# (this.IsStorePickUpOrder == false) %>" />
		</td>
		<td class="detail_item_bg" align="center" rowspan="4">
			<asp:Button
				ID="btnUpdateReceipt"
				Runat="server"
				Text="  領収書情報更新  "
				OnClick="btnUpdateReceipt_Click"
				Enabled="<%# (this.IsStorePickUpOrder == false) %>" />
			<div>
				<asp:LinkButton
					ID="lbReceiptExport"
					Runat="server"
					OnClick="lbReceiptExport_Click"
					Text="  領収書出力  "
					Enabled="<%# (this.IsStorePickUpOrder == false) %>" />
			</div>
		</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left">領収書出力</td>
		<td class="detail_item_bg" align="left">
			<asp:RadioButtonList
				ID="rblReceiptOutputFlg"
				runat="server"
				RepeatDirection="Horizontal"
				RepeatLayout="Flow"
				Enabled="<%# (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON) && (this.IsStorePickUpOrder == false) %>" />
		</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left">宛名
			<% if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON){ %><span class="notice">*</span><% } %>
		</td>
		<td class="detail_item_bg" align="left">
			<asp:TextBox
				ID="tbReceiptAddress"
				Text="<%# this.OrderInput.ReceiptAddress %>"
				Width="100%"
				runat="server"
				MaxLength="100"
				Enabled="<%# (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON) && (this.IsStorePickUpOrder == false) %>" />
		</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left">但し書き
			<% if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON){ %><span class="notice">*</span><% } %>
		</td>
		<td class="detail_item_bg" align="left">
			<asp:TextBox
				ID="tbReceiptProviso"
				Text="<%# this.OrderInput.ReceiptProviso %>"
				Width="100%"
				runat="server"
				MaxLength="100"
				Enabled="<%# (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON) && (this.IsStorePickUpOrder == false) %>" />
		</td>
	</tr>
	</tbody>
</table>
<br />
<% } %>
<!-- △領収書情報△ -->

<asp:Repeater ID="rShippingList" DataSource="<%# this.OrderInput.Shippings %>" OnItemCommand="rShippingList_ItemCommand" runat="server" ItemType="OrderShippingInput">
<ItemTemplate>
<%-- ▽配送情報▽ --%>
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<% if ((Constants.GIFTORDER_OPTION_ENABLED) && (this.OrderInput.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)) { %>
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">【　配送情報<%#: Item.OrderShippingNo %>　】</td>
	</tr>
	<%-- ▽送り主情報（ギフト注文の場合のみ表示）▽ --%>
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">送り主情報</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name.name@@")%></td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: Item.SenderName %></td>
		<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name_kana.name@@")%></td>
		<td class="detail_item_bg" align="left">
			<%#: Item.SenderNameKana %></td>
	</tr>	<tr>
		<td class="detail_title_bg" align="left">住所</td>
		<td class="detail_item_bg" align="left" colspan="3">
			<span visible='<%# (string.IsNullOrEmpty(Item.SenderZip) == false) %>' runat="server">
			<%#: Item.IsSenderAddrJp ? "〒" + Item.SenderZip : "" %>
			</span>
			<%#: Item.SenderAddr1 %>
			<%#: Item.SenderAddr2 %>&nbsp;
			<%#: Item.SenderAddr3 %>&nbsp;
			<%#: Item.SenderAddr4 %>&nbsp;
			<%#: Item.SenderAddr5 %>&nbsp;
			<%#: (Item.IsSenderAddrJp == false) ? Item.SenderZip : "" %>
			<%#: Item.SenderCountryName %>
		</td>
	</tr>
	<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
	<tr>
		<td class="detail_title_bg" align="left"><%: ReplaceTag("@@User.company_name.name@@")%>・<%: ReplaceTag("@@User.company_post_name.name@@")%></td>
		<td class="detail_item_bg" align="left" colspan="3">
			<%#: Item.SenderCompanyName %>&nbsp<%#: Item.SenderCompanyPostName %></td>
	</tr>
	<%} %>
	<tr>
		<td class="detail_title_bg" align="left"><%: ReplaceTag("@@User.tel1.name@@")%></td>
		<td class="detail_item_bg" align="left" colspan="3">
			<%#: Item.SenderTel1 %></td>
	</tr>
	<%-- △送り主情報（ギフト注文の場合のみ表示）△ --%>
	<% } %>
	<%-- ▽配送先情報▽ --%>
	<div style="text-align:right; margin:0px 0px 5px 0px;">
		<asp:Button
			ID="btnEdit"
			Visible="<%# (this.OrderInput.IsReturnOrder
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& this.IsShippingConvenience) %>"
			OnClick="btnEdit_Click"
			Text="  編集する  "
			runat="server" />
	</div>
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">配送先情報</td>
	</tr>
	<tbody runat="server" visible="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>">
	<tr>
		<td class="detail_title_bg" align="left" width="25%">
			<%#: string.IsNullOrEmpty(Item.StorePickupRealShopId)
				? ReplaceTag("@@User.name.name@@")
				: "店舗名" %>
		</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: Item.ShippingName %></td>
		<td class="detail_title_bg" align="left" width="25%">
			<%#: string.IsNullOrEmpty(Item.StorePickupRealShopId)
				? ReplaceTag("@@User.name_kana.name@@")
				: "店舗名(かな)" %>
		</td>
		<td class="detail_item_bg" align="left">
			<%#: Item.ShippingNameKana %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">住所</td>
		<td class="detail_item_bg" align="left" colspan="3">
			<span visible='<%# (string.IsNullOrEmpty(Item.ShippingZip) == false) %>' runat="server">
			<%#: Item.IsShippingAddrJp ? "〒" + Item.ShippingZip : "" %>
			</span>
			<%#: Item.ShippingAddr1 %>
			<%#: Item.ShippingAddr2 %>&nbsp;
			<%#: Item.ShippingAddr3 %>&nbsp;
			<%#: Item.ShippingAddr4 %>&nbsp;
			<%#: Item.ShippingAddr5 %>&nbsp;
			<span visible='<%# (string.IsNullOrEmpty(Item.ShippingZip) == false) %>' runat="server">
			<%#: (Item.IsShippingAddrJp == false) ? Item.ShippingZip : "" %>
			</span>
			<%#: Item.ShippingCountryName %>
		</td>
	</tr>
	<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.company_name.name@@")%>・<%: ReplaceTag("@@User.company_post_name.name@@")%></td>
		<td class="detail_item_bg" align="left" colspan="3">
			<%#: Item.ShippingCompanyName %>&nbsp<%#: Item.ShippingCompanyPostName %></td>
	</tr>
	<%} %>
	<tr>
		<td class="detail_title_bg" width="200"><%: ReplaceTag("@@User.tel1.name@@") %></td>
		<td class="detail_item_bg" colspan="3">
			<%#: Item.ShippingTel1 %></td>
	</tr>
	</tbody>
	<tbody runat="server" visible="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON %>">
	<tr>
		<td class="detail_title_bg" align="left" width="25%">店舗ID</td>
		<td class="detail_item_bg" align="left" colspan="3"><%#: Item.ShippingReceivingStoreId %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">店舗名称</td>
		<td class="detail_item_bg" align="left" colspan="3"><%#: Item.ShippingName %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">店舗住所</td>
		<td class="detail_item_bg" align="left" colspan="3"><%#: Item.ShippingAddr4 %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" width="200">店舗電話番号</td>
		<td class="detail_item_bg" colspan="3"><%#: Item.ShippingTel1 %></td>
	</tr>
	</tbody>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">配送方法</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: (string.IsNullOrEmpty(Item.StorePickupRealShopId) == false)
				? ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FLG_ORDER_SHIPPING_KBN_LIST, CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
				: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, Item.ShippingMethod) %>
		<td class="detail_title_bg" align="left" width="25%">配送サービス</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: GetDeliveryCompanyName(Item.DeliveryCompanyId) %></td>
	</tr>
	<tbody runat="server" Visible="<%# (OrderCommon.IsLeadTimeFlgOff(Item.DeliveryCompanyId) == false) %>">
	<tr>
		<td class="detail_title_bg" align="left" width="25%">配送リードタイム</td>
		<td class="detail_item_bg" align="left" <%: this.UseLeadTime ? "" : "colspan=3" %>>
			<%#: OrderCommon.GetTotalLeadTime(this.OrderInput.ShopId, Item.DeliveryCompanyId, Item.ShippingAddr1, Item.ShippingZip) %>
		</td>
		<% if (this.UseLeadTime){ %>
			<td class="detail_title_bg" align="left" width="25%">出荷予定日</td>
			<td class="detail_item_bg" align="left" width="25%"><%#: GetShippingDateText(Item.ScheduledShippingDate) %></td>
		<%} %>
	</tr>
	</tbody>
	<tbody runat="server" Visible="<%# (OrderCommon.IsLeadTimeFlgOff(Item.DeliveryCompanyId)) %>">
	<tr>
		<% if (this.UseLeadTime){ %>
			<td class="detail_title_bg" align="left" width="25%">出荷予定日</td>
			<td class="detail_item_bg" align="left" width="25%" colspan="3"><%#: GetShippingDateText(Item.ScheduledShippingDate) %></td>
		<%} %>
	</tr>
	</tbody>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">配送希望日</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: GetShippingDateText(Item.ShippingDate) %></td>
		<td class="detail_title_bg" align="left" width="25%">配送希望時間帯</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: GetShippingTimeText(Item.DeliveryCompanyId, Item.ShippingTime) %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" width="200">配送伝票番号</td>
		<td class="detail_item_bg" colspan="3">
			<asp:TextBox
				ID="tbShippingCheckNo"
				Text="<%# Item.ShippingCheckNo %>"
				Width="300"
				runat="server"
				MaxLength="50"
				Enabled="<%# (this.IsStorePickUpOrder == false) %>" />

			<asp:Button
				ID="btnUpdateShippingCheckNo"
				Runat="server"
				Text="  配送伝票番号更新  "
				Width="120"
				CommandName="update_shipping_check_no"
				CommandArgument='<%# Container.ItemIndex + "," + Item.OrderShippingNo + "," + Item.ShippingCheckNo %>'
				Enabled="<%# (this.IsStorePickUpOrder == false) %>" />

			<%if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& (Constants.RECEIVINGSTORE_TWECPAY_CVS_LOGISTIC_METHOD == ECPayConstants.CONST_CVS_LOGISTIC_METHOD_C2C)
				&& this.IsCVSAndHaveShippingCheckNo) { %>
				<asp:Button
					ID="btnCVSPaymentSlipPrinting"
					Runat="server"
					Text="  コンビニ支払伝票印刷  "
					Width="120"
					OnClick="btnCVSPaymentSlipPrinting_Click"
					Enabled="<%# (this.IsStorePickUpOrder == false) %>" />
			<% if (this.IsCVS7Eleven) { %>
				<asp:Button
					ID="btnCVSCooperation"
					Runat="server"
					Text="  コンビニ連携  "
					Width="120"
					OnClick="btnCVSCooperation_Click"
					Enabled="<%# (this.IsStorePickUpOrder == false) %>" />
			<% } %>
			<% } %>
			<%if (OrderCommon.CanShipmentEntry(this.OrderInput.OrderPaymentKbn)) {%>
				<asp:CheckBox
					id="cbExecExternalShipmentEntry"
					Text="出荷情報登録連携"
					Checked="true"
					runat="server"
					Enabled="<%# (this.IsStorePickUpOrder == false) %>" />
			<%} %>
		</td>
	</tr>
	<% if (Constants.TWPELICAN_COOPERATION_EXTEND_ENABLED) { %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">データ生成日</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: this.OrderInput.Shippings[0].ShippingStatusUpdateDate %>
		</td>
		<td class="detail_title_bg" align="left" width="25%">完了状態コード</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE, this.OrderInput.Shippings[0].ShippingStatusCode) %>
		</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">配送状態ID</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS, this.OrderInput.Shippings[0].ShippingStatus) %>
		</td>
		<td class="detail_title_bg" align="left" width="25%">営業所略称</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: this.OrderInput.Shippings[0].ShippingOfficeName %>
		</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">Handy操作時間</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: this.OrderInput.Shippings[0].ShippingHandyTime %>
		</td>
		<td class="detail_title_bg" align="left" width="25%">現在の状態</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS, this.OrderInput.Shippings[0].ShippingCurrentStatus) %>
		</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%"></td>
		<td class="detail_item_bg" align="left" width="25%"></td>
		<td class="detail_title_bg" align="left" width="25%">状態説明</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: this.OrderInput.Shippings[0].ShippingStatusDetail %>
		</td>
	</tr>
	<% } %>
<% if (OrderCommon.IsInvoiceBundleServiceUsable(this.OrderInput.OrderPaymentKbn)) { %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">後払い請求書</td>
		<td class="detail_item_bg" align="left" width="25%" colspan="<%# CheckDisplayInvoice() ? 1 : 3 %>">
			<%: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG, this.OrderInput.InvoiceBundleFlg) %>
		</td>
		<% if (CheckDisplayInvoice()){ %>
		<td class="detail_title_bg" align="left" width="25%">印字データ取得状況　<asp:Button runat="server" id="btnGetInvoice" Text="　印字データ取得　" CommandName="GetInvoice" Enabled="<%# CheckEnabledInvoiceButton() %>"/>
		</td>
		<td class="detail_item_bg" align="left" width="25%">
			<% if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene) && (GetAtodeneInvoice() != null)) { %>
				<%: string.Format("取得済み（取得日時：{0:yyyy/MM/dd HH:mm:ss}）", GetAtodeneInvoice().DateCreated) %>
			<% } else if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk) && (this.InvoiceData != null)){ %>
				<%: string.Format("取得済み（取得日時：{0:yyyy/MM/dd HH:mm:ss}）", ((InvoiceDskDeferredModel)this.InvoiceData).DateCreated) %>
			<% } else if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score) && (this.InvoiceData != null)){ %>
				<%: string.Format("取得済み（取得日時：{0:yyyy/MM/dd HH:mm:ss}）", ((InvoiceScoreModel)this.InvoiceData).DateCreated) %>
			<% } else if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans) && (this.InvoiceData != null)){ %>
				<%: string.Format("取得済み（取得日時：{0:yyyy/MM/dd HH:mm:ss}）", ((InvoiceVeritransModel)this.InvoiceData).DateChanged) %>
			<% }else { %>
				未取得
			<% } %>
		</td>
		<% } %>
	</tr>
	<%} %>
	<%-- △配送先情報△--%>
	<%-- ▽のし・包装情報（ギフト注文の場合のみ表示）▽ --%>
	<% if ((Constants.GIFTORDER_OPTION_ENABLED) && (this.OrderInput.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)) { %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">のしの種類</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: Item.WrappingPaperType %></td>
		<td class="detail_title_bg" align="left" width="25%">のし差出人</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: Item.WrappingPaperName %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">包装の種類</td>
		<td class="detail_item_bg" align="left" colspan="3">
			<%#: Item.WrappingBagType %></td>
	</tr>
	<% } %>
	<%-- △のし・包装情報（ギフト注文の場合のみ表示）△--%>
</table>
<%-- △配送情報△--%>
<% if ((Constants.GIFTORDER_OPTION_ENABLED == false) || (this.OrderInput.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_OFF)) { %>
<br />
<% } %>
<%-- ▽注文商品情報▽ --%>
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="<%# 6 + this.AddColumnCountForItemTable %>">商品情報</td>
	</tr>
	<tr>
		<% if (this.OrderInput.IsNotReturnExchangeOrder == false) { %>
		<td class="detail_title_bg" align="center" rowspan="2" width="10%">
			在庫戻し<br />
			<asp:Label ID="Label1" class="detail_title_bg" align="center" visible='<%# (Item.IsAllItemStockReturned == false) %>' runat="server">
				<a href="javascript:selected_item_all();">全てチェック </a>
			</asp:Label>
		</td>
		<% } %>
		<%--▽ セールOPが有効の場合 ▽--%>
		<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
		<td class="detail_title_bg" align="center" rowspan="2" width='10%'>
			セールID
		</td>
		<%} %>
		<%--△ セールOPが有効の場合 △--%>
		<%--▽ ノベルティOPが有効の場合 ▽--%>
		<% if (Constants.NOVELTY_OPTION_ENABLED) { %>
		<td class="detail_title_bg" align="center" rowspan="<%= Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>" width="10%">
			ノベルティID
		</td>
		<%} %>
		<%--△ ノベルティOPが有効の場合 △--%>
		<%--▽ レコメンド設定OPが有効の場合 ▽--%>
		<% if (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) { %>
		<td class="detail_title_bg" align="center" rowspan="2" width="10%">
			レコメンドID
		</td>
		<%} %>
		<%--△ レコメンド設定OPが有効の場合 △--%>
		<%--▽ 商品同梱設定OPが有効の場合 ▽--%>
		<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED) { %>
		<td class="detail_title_bg" align="center" rowspan="2" width="10%">
			商品同梱ID
		</td>
		<%} %>
		<%--△ 商品同梱設定OPが有効の場合 △--%>
		<td class="detail_title_bg" align="center" rowspan="2" width="10%" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" runat="server">
			頒布会コースID
		</td>
		<td class="detail_title_bg" align="center" width="13%">
			商品ID
		</td>
		<% if(this.OrderInput.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON) { %>
		<td class="detail_title_bg" align="center" width="13%" rowspan="2">
			モール用項目
		</td>
		<% } %>
		<td class="detail_title_bg" align="center" rowspan="2" width="<%= 10 + ((this.OrderInput.IsNotReturnExchangeOrder == false) ? 0 : 10) + (Constants.PRODUCT_SALE_OPTION_ENABLED ? 0 : 10) + ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 0 : 10) + (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 0 : 8) + (Constants.REALSTOCK_OPTION_ENABLED ? 0 : 8) %>%">
			商品名
		</td>
		<td class="detail_title_bg" align="center" rowspan="2" width="8%" Visible="<%# this.DisplayAllWhenHasFixedAmountItem %>" runat="server">
			単価（<%#: this.ProductPriceTextPrefix %>）
		</td>
		<td class="detail_title_bg" align="center" rowspan="2" width='4%' colspan="<%# this.OrderInput.HasSetProduct ? 2 : 1 %>">
			数量
		</td>
		<td class="detail_title_bg" align="center" width="8%" colspan="2" rowspan="1" Visible="<%# this.IsFixedPurchaseCountAreaShow %>" runat="server">
			定期購入回数
		</td>
		<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
		<td class="detail_title_bg" align="center" rowspan="2" width="4%">
			引当状況
		</td>
		<% } %>
		<td class="edit_title_bg" align="center" width="4%" rowspan="2" style="height: 31px" Visible="<%# this.DisplayAllWhenHasFixedAmountItem %>" runat="server">
			消費税率
		</td>
		<td class="detail_title_bg" align="center" rowspan="2" width="8%" Visible="<%# this.DisplayAllWhenHasFixedAmountItem %>" runat="server">
			小計（<%#: this.ProductPriceTextPrefix %>）
		</td>
	</tr>
	<tr>
		<%--▽ レコメンド設定OPが有効の場合 ▽--%>
		<% if (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) { %>
		<td class="detail_title_bg" align="center">
			レコメンドID
		</td>
		<%} %>
		<%--△ レコメンド設定OPが有効の場合 △--%>
		<td class="detail_title_bg" align="center">
			バリエーションID
		</td>
		<td class="detail_title_bg" align="center" Visible="<%# this.IsFixedPurchaseCountAreaShow %>" runat="server">
			注文基準
		</td>
		<td class="detail_title_bg" align="center" Visible="<%# this.IsFixedPurchaseCountAreaShow %>" runat="server">
			出荷基準
		</td>
	</tr>
	<asp:Repeater id="rItemList" DataSource="<%# Item.Items %>" runat="server" ItemType="w2.App.Common.Input.Order.OrderItemInput">
		<ItemTemplate>
			<%--▽ 隠しタグ ▽--%>
			<asp:HiddenField ID="hfOrderShippingNo" runat="server" Value="<%# Item.OrderShippingNo %>" />
			<asp:HiddenField ID="hfOrderItemNo" runat="server" Value="<%# Item.OrderItemNo %>" />
			<%--△ 隠しタグ △--%>
			<%-- セット商品 --%>
			<tr visible='<%# Item.IsProductSet %>' runat="server">
				<td class="detail_item_bg" align="center" rowspan='<%# OrderPage.GetProductSetRowspan(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' runat="server" visible="<%# (this.OrderInput.IsNotReturnExchangeOrder == false) %>">
					<asp:CheckBox ID="cbItemSet" checked='<%# Item.IsItemStockReturned %>' Enabled='<%# Item.IsAllowReturnStock %>' Visible='<%# Item.IsProductStockManaged %>' runat="server"></asp:CheckBox>
				</td>
				<td class="detail_item_bg" align="center" rowspan='<%# OrderPage.GetProductSetRowspan(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' runat="server" visible="<%# Constants.PRODUCT_SALE_OPTION_ENABLED && OrderPage.IsProductSetItemTop(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>">
					<%#: (string.IsNullOrEmpty(Item.ProductsaleId) == false) ? Item.ProductsaleId : "-" %>
				</td>
				<td class="detail_item_bg" align="center" rowspan='<%# Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>' runat="server" visible="<%# Constants.NOVELTY_OPTION_ENABLED %>">
					<%#: (string.IsNullOrEmpty(Item.NoveltyId) == false) ? Item.NoveltyId : "-" %>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) %>">
					<%#: (string.IsNullOrEmpty(Item.RecommendId) == false) ? Item.RecommendId : "-" %>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2" visible="<%# Constants.PRODUCTBUNDLE_OPTION_ENABLED %>" runat="server">
					<%# OrderPage.GetProductBundleIdDisplayValue(Item.ProductBundleId, this.LoginShopOperator) %>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" runat="server">
					<div Visible="<%# Item.IsSubscriptionBox %>" runat="server">
						<a href="javascript:open_window('<%#: FixedPurchasePage.CreateSubscriptionBoxRegisterUrl(Item.SubscriptionBoxCourseId) %>','fixedpurchase','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
							<%#: Item.SubscriptionBoxCourseId %>
						</a>
					</div>
					<div Visible="<%# Item.IsSubscriptionBox == false %>" runat="server">-</div>
				</td>
				<td class="detail_item_bg" align="center">
					<%# Item.ProductId %>
				</td>
				<td visible='<%# (this.OrderInput.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON) %>' class="detail_item_bg" align="center" rowspan="2">
					<%#: (string.IsNullOrEmpty(Item.ColumnForMallOrder) == false) ? Item.ColumnForMallOrder : "-" %>
				</td>
				<td class="detail_item_bg" align="left" rowspan="2">
					<%#: Item.ProductName %>
				</td>
				<td class="detail_item_bg" align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.DisplayAllWhenHasFixedAmountItem %>" runat="server">
					<%#: Item.IsSubscriptionBoxFixedAmount == false ? Item.ProductPrice.ToPriceString(true) : "-" %>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2">
					<%#: Item.ItemQuantitySingle %>
				</td>
				<td class="detail_item_bg" align="center" rowspan='<%# OrderPage.GetProductSetRowspan(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' Visible='<%# this.DisplayAllWhenHasFixedAmountItem && OrderPage.IsProductSetItemTop(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' runat="server">
					x <%#: Item.ProductSetCount %>
				</td>
				<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
				<td class="detail_item_bg" align="center" rowspan="2" visible='<%# Constants.REALSTOCK_OPTION_ENABLED %>' runat="server">
					<%#: OrderPage.GetRealStockReservedDisplayString(Item) %>
				</td>
				<%--△ 実在庫利用が有効な場合は表示 △--%>
				<td class="detail_item_bg" align="center" rowspan="2" Visible="<%# this.DisplayAllWhenHasFixedAmountItem %>" runat="server">
					<%#: Item.IsSubscriptionBoxFixedAmount == false ? string.Format("{0}%", Item.ProductTaxRate) : "-" %>
				</td>
				<td class="detail_item_bg" align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan='<%# OrderPage.GetProductSetRowspan(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' Visible='<%# this.DisplayAllWhenHasFixedAmountItem && OrderPage.IsProductSetItemTop(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' runat="server">
					<%#: Item.IsSubscriptionBoxFixedAmount == false
						     ? OrderPage.CreateSetPriceSubtotal(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource).ToPriceString(true)
						     : string.Format("定額({0})", Item.SubscriptionBoxFixedAmount.ToPriceString(true)) %>
				</td>
			</tr>
			<tr visible='<%# Item.IsProductSet %>' runat="server">
				<td class="detail_item_bg" align="center" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) %>">
					<%#: (string.IsNullOrEmpty(Item.RecommendId) == false) ? Item.RecommendId : "-" %>
				</td>
				<td class="detail_item_bg" align="center">
					<%#: (string.IsNullOrEmpty(Item.VId) == false) ? "商品ID + " + Item.VId : "-" %>
				</td>
			</tr>
			<%-- 通常商品 --%>
			<tr visible='<%# (Item.IsProductSet == false) %>' runat="server">
				<td class="detail_item_bg" align="center" rowspan='<%# OrderPage.GetProductSetRowspan(Item, (OrderItemInput[])((Repeater)Container.Parent).DataSource) %>' runat="server" visible="<%# (this.OrderInput.IsNotReturnExchangeOrder == false) %>">
					<asp:CheckBox ID="cbItem" checked='<%# Item.IsItemStockReturned %>' Enabled='<%# Item.IsAllowReturnStock %>' Visible='<%# Item.IsProductStockManaged %>' runat="server"></asp:CheckBox>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2" runat="server" visible="<%# Constants.PRODUCT_SALE_OPTION_ENABLED %>">
					<%#: (string.IsNullOrEmpty(Item.ProductsaleId) == false) ? Item.ProductsaleId : "-" %>
				</td>
				<td class="detail_item_bg" align="center" rowspan="<%# Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>" runat="server" visible="<%# Constants.NOVELTY_OPTION_ENABLED %>">
					<%#: (string.IsNullOrEmpty(Item.NoveltyId) == false) ? Item.NoveltyId : "-" %>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) %>">
					<%#: (string.IsNullOrEmpty(Item.RecommendId) == false) ? Item.RecommendId : "-" %>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2" visible="<%# Constants.PRODUCTBUNDLE_OPTION_ENABLED %>" runat="server">
					<%# OrderPage.GetProductBundleIdDisplayValue(Item.ProductBundleId, this.LoginShopOperator) %>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" runat="server">
					<div Visible="<%# Item.IsSubscriptionBox %>" runat="server">
						<a href="javascript:open_window('<%#: FixedPurchasePage.CreateSubscriptionBoxRegisterUrl(Item.SubscriptionBoxCourseId) %>','fixedpurchase','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
							<%#: Item.SubscriptionBoxCourseId %>
						</a>
					</div>
					<div Visible="<%# Item.IsSubscriptionBox == false %>" runat="server">-</div>
				</td>
				<td class="detail_item_bg" align="center">
					<%#: Item.ProductId %>
				</td>
				<td visible='<%# (this.OrderInput.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON) %>' class="detail_item_bg" align="center" rowspan="2">
					<%#: (string.IsNullOrEmpty(Item.ColumnForMallOrder) == false) ? Item.ColumnForMallOrder : "-" %>
				</td>
				<td class="detail_item_bg" align="left" rowspan="2">
					<%#: Item.ProductName %>
					<div runat="server" visible='<%# ((string.IsNullOrEmpty(Item.GiftWrappingId) == false) || (string.IsNullOrEmpty(Item.GiftMessage) == false)) %>'>
						<img src="<%= Constants.PATH_ROOT %>Images/Common/gift.png" 
							title='<%#: string.Format("ギフト包装ID：{0}\r\nギフトメッセージ：{1}", Item.GiftWrappingId, Item.GiftMessage)  %>'
							height="16" width="16" border="0" style="margin-bottom:-3px;margin-left:-3px;"/>
					</div>
					<%# WebSanitizer.HtmlEncodeChangeToBr(GetDeliveredSerialKeyList(Item.OrderItemNo)) %><br />
					<span visible='<%# (string.IsNullOrEmpty(Item.OrderSetpromotionNo) == false) %>' runat="server">
						[<%#: Item.OrderSetpromotionNo %> ： <%#: this.OrderInput.GetOrderSetPromotionName(Item.OrderSetpromotionNo) %>]
					</span>
				</td>
				<td class="detail_item_bg" align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.DisplayAllWhenHasFixedAmountItem %>" runat="server">
					<%#: Item.IsSubscriptionBoxFixedAmount == false ? Item.ProductPrice.ToPriceString(true) : "-" %>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2" colspan="<%# this.OrderInput.HasSetProduct ? 2 : 1 %>">
					<%# OrderPage.GetMinusNumberNoticeHtml(Item.ItemQuantity, false)%>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2" Visible="<%# this.IsFixedPurchaseCountAreaShow %>" runat="server">
					<%#: Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON ? string.Format("{0} 回", string.IsNullOrEmpty(Item.FixedPurchaseItemOrderCount) ? "0" : Item.FixedPurchaseItemOrderCount) : "-" %>
				</td>
				<td class="detail_item_bg" align="center" rowspan="2" Visible="<%# this.IsFixedPurchaseCountAreaShow %>" runat="server">
					<%#: Item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON ? string.Format("{0} 回", string.IsNullOrEmpty(Item.FixedPurchaseItemShippedCount) ? "0" : Item.FixedPurchaseItemShippedCount) : "-" %>
				</td>
				<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
				<td class="detail_item_bg" align="center" rowspan="2" visible='<%# Constants.REALSTOCK_OPTION_ENABLED %>' runat="server">
					<%# OrderPage.GetRealStockReservedDisplayString(Item) %>
				</td>
				<%--△ 実在庫利用が有効な場合は表示 △--%>
				<td class="detail_item_bg" align="center" rowspan="2" Visible="<%# this.DisplayAllWhenHasFixedAmountItem %>" runat="server">
					<%#: Item.IsSubscriptionBoxFixedAmount == false ? string.Format("{0}%", Item.ProductTaxRate) : "-" %>
				</td>
				<td class="detail_item_bg" align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.DisplayAllWhenHasFixedAmountItem %>" runat="server">
					<div Visible="<%# Item.IsSubscriptionBoxFixedAmount == false %>" runat="server">
						<%# OrderPage.GetMinusNumberNoticeHtml(Item.ItemPrice, true) %>
					</div>
					<div Visible="<%# Item.IsSubscriptionBoxFixedAmount %>" class='<%# IsFixedAmountMinusWhenReturn(Item.SubscriptionBoxCourseId) ? "item-notice" : string.Empty %>' runat="server">
						定額<br />(<%#: string.Format("{0}{1}", IsFixedAmountMinusWhenReturn(Item.SubscriptionBoxCourseId) ? "-" : "", Item.SubscriptionBoxFixedAmount.ToPriceString(true)) %>)
					</div>
				</td>
			</tr>
			<tr visible='<%# (Item.IsProductSet == false) %>' runat="server">
				<td class="detail_item_bg" align="center" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) %>">
					<%#: (string.IsNullOrEmpty(Item.RecommendId) == false) ? Item.RecommendId : "-" %>
				</td>
				<td class="detail_item_bg" align="center">
					<%#: (string.IsNullOrEmpty(Item.VId) == false) ? "商品ID + " + Item.VId : "-" %>
				</td>
			</tr>
			<tr visible='<%# ((Item.IsProductSet == false) && (string.IsNullOrEmpty(Item.ProductOptionTexts) == false)) %>' runat="server">
				<td class="detail_title_bg" align="center">付帯情報</td>
				<td class="detail_item_bg" align="left" colspan='<%# 5 + this.AddColumnCountForItemTable %>'>
					<%#: ProductOptionSettingHelper.GetDisplayProductOptionTexts(Item.ProductOptionTexts) %>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
	<% if (this.OrderInput.IsNotReturnExchangeOrder == false) { %>
	<tr>
		<td class="detail_title_bg" align="right" colspan="<%# 6 + this.AddColumnCountForItemTable %>">
			<asp:Button ID="btnReturnStock" Text="  在庫情報を更新  " width="120" runat="server" Enabled='<%# (Item.IsAllItemStockReturned == false) %>' OnClick="btnReturnStock_Click" />
		</td>
	</tr>
	<% } %>
</table>
<%-- △注文商品情報△--%>
<br />
</ItemTemplate>
</asp:Repeater>

<%-- ▽合計金額▽ --%>
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="2">
			合計金額
			<span runat="server" style='<%# ((this.OrderInput.MallId == Constants.FLG_USER_MALL_ID_OWN_SITE) || (this.OrderInput.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)) ? "display:none;" : "color:red; font-size: 8pt;"%>'>
				<br />(※現時点ではモール注文商品価格の税込・税抜が不明なため、商品金額と合計金額計算の整合性が合わない場合がございます。)
			</span>
		</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">商品合計（<%#: this.ProductPriceTextPrefix %>）</td>
		<td class="detail_item_bg" align="right">
			<%# OrderPage.GetMinusNumberNoticeHtml(this.OrderInput.OrderPriceSubtotal, true) %></td>
	</tr>
	<%if (this.ProductIncludedTaxFlg == false){ %>
		<tr>
			<td class="detail_title_bg" align="left">消費税額</td>
			<td class="detail_item_bg" align="right">
				<%# OrderPage.GetMinusNumberNoticeHtml(this.OrderInput.OrderPriceSubtotalTax, true)%></td>
		</tr>
	<%} %>
	<asp:Repeater DataSource="<%# this.OrderInput.SetPromotions %>" Visible="<%# this.OrderInput.IsAllItemsSubscriptionBoxFixedAmount == false %>" runat="server" ItemType="w2.App.Common.Input.Order.OrderSetPromotionInput">
		<ItemTemplate>
			<tr visible="<%# Item.IsDiscountTypeProductDiscount %>" runat="server">
				<td class="detail_title_bg" align="right">
					<%#: Item.OrderSetpromotionNo %> ： <%#: Item.SetpromotionName %>割引額<br />
					(ID:<%#: Item.SetpromotionId %>, 商品割引分)
				</td>
				<td class="detail_item_bg" align="right">
					<span class='<%# decimal.Parse(Item.ProductDiscountAmount) > 0 ? "notice" : "" %>'>
						<%#: decimal.Parse(Item.ProductDiscountAmount) > 0 ? "-" : "" %><%#: Item.ProductDiscountAmount.ToPriceString(true) %>
					</span>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
	<%-- 会員ランクOPが有効な場合 --%>
	<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
	<tr>
		<td class="detail_title_bg" align="left">会員ランク割引</td>
		<td class="detail_item_bg" align="right">
			<span class='<%# decimal.Parse(this.OrderInput.MemberRankDiscountPrice) > 0 ? "notice" : "" %>'>
				<%# decimal.Parse(this.OrderInput.MemberRankDiscountPrice) > 0 ? "-" : "" %><%#: this.OrderInput.MemberRankDiscountPrice.ToPriceString(true) %>
			</span>
		</td>
	</tr>
	<%} %>
	<%-- 会員ランクOP及び定期購入オプションが有効な場合 --%>
	<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
	<tr>
		<td class="detail_title_bg" align="left">定期会員割引</td>
		<td class="detail_item_bg" align="right">
			<span class='<%# decimal.Parse(this.OrderInput.FixedPurchaseMemberDiscountAmount) > 0 ? "notice" : "" %>'>
				<%# decimal.Parse(this.OrderInput.FixedPurchaseMemberDiscountAmount) > 0 ? "-" : "" %><%#: this.OrderInput.FixedPurchaseMemberDiscountAmount.ToPriceString(true) %>
			</span>
		</td>
	</tr>
	<%} %>
	<%-- クーポンオプションが有効の場合--%>
	<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
	<tr>
		<td class="detail_title_bg" align="left">クーポン割引額</td>
		<td class="detail_item_bg" align="right">
			<span class='<%# decimal.Parse(this.OrderInput.OrderCouponUse) > 0 ? "notice" : "" %>'>
				<%# decimal.Parse(this.OrderInput.OrderCouponUse) > 0 ? "-" : ""%><%#: this.OrderInput.OrderCouponUse.ToPriceString(true) %>
			</span>
		</td>
	</tr>
	<%} %>
	<%-- ポイントオプションが有効の場合--%>
	<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
	<tr>
		<td class="detail_title_bg" align="left">ポイント利用額</td>
		<td class="detail_item_bg" align="right">
			<span class='<%# decimal.Parse(this.OrderInput.OrderPointUseYen) > 0 ? "notice" : "" %>'>
				<%# decimal.Parse(this.OrderInput.OrderPointUseYen) > 0 ? "-" : "" %><%#: this.OrderInput.OrderPointUseYen.ToPriceString(true) %>
			</span>
		</td>
	</tr>
	<%} %>
	<%-- 定期購入オプションが有効の場合 --%>
	<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
	<tr>
		<td class="detail_title_bg" align="left">定期購入割引額</td>
		<td class="detail_item_bg" align="right">
			<span class='<%# decimal.Parse(this.OrderInput.FixedPurchaseDiscountPrice) > 0 ? "notice" : "" %>'>
				<%#: decimal.Parse(this.OrderInput.FixedPurchaseDiscountPrice) > 0 ? "-" : ""%><%#: this.OrderInput.FixedPurchaseDiscountPrice.ToPriceString(true) %>
			</span>
		</td>
	</tr>
	<% } %>
	<tr>
		<td class="detail_title_bg" align="left">配送料
			<!-- 配送料別途見積もりする場合 -->
			<% if (this.OrderInput.IsValidShippingPriceSeparateEstimatesFlg){ %>
				<br /><%#: (this.ShopShipping != null) ? this.ShopShipping.ShippingPriceSeparateEstimatesMessage : string.Empty %>
	<% } %>
		</td>
		<td class="detail_item_bg" align="right">
			<%#: this.OrderInput.OrderPriceShipping.ToPriceString(true) %></td>
	</tr>
	<asp:Repeater DataSource="<%# this.OrderInput.SetPromotions %>" runat="server" ItemType="OrderSetPromotionInput">
		<ItemTemplate>
			<tr visible="<%# Item.IsDiscountTypeShippingChargeFree %>" runat="server">
				<td class="detail_title_bg" align="right">
					<%#: Item.OrderSetpromotionNo %> ： <%#: Item.SetpromotionName %>割引額<br />
					(ID:<%#: Item.SetpromotionId %>, 配送料割引分)
				</td>
				<td class="detail_item_bg" align="right">
					<span class='<%# decimal.Parse(Item.ShippingChargeDiscountAmount) > 0 ? "notice" : "" %>'>
						<%#: (decimal.Parse(Item.ShippingChargeDiscountAmount) > 0) ? "-" : ""%><%#: Item.ShippingChargeDiscountAmount.ToPriceString(true) %>
					</span>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
	<tr>
		<td class="detail_title_bg" align="left">決済手数料</td>
		<td class="detail_item_bg" align="right">
			<%#: this.OrderInput.OrderPriceExchange.ToPriceString(true) %></td>
	</tr>
	<asp:Repeater DataSource="<%# this.OrderInput.SetPromotions %>" runat="server" ItemType="OrderSetPromotionInput">
		<ItemTemplate>
			<tr visible="<%# Item.IsDiscountTypePaymentChargeFree %>" runat="server">
				<td class="detail_title_bg" align="right">
					<%#: Item.OrderSetpromotionNo %> ： <%#: Item.SetpromotionName %>割引額<br />
					(ID:<%#: Item.SetpromotionId %>, 決済手数料割引分)
				</td>
				<td class="detail_item_bg" align="right">
					<span class='<%# decimal.Parse(Item.PaymentChargeDiscountAmount) > 0 ? "notice" : "" %>'>
						<%# decimal.Parse(Item.PaymentChargeDiscountAmount) > 0 ? "-" : ""%><%#: Item.PaymentChargeDiscountAmount.ToPriceString(true) %>
					</span>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
	<tr>
		<td class="detail_title_bg" align="left">調整金額</td>
		<td class="detail_item_bg" align="right">
			<%# OrderPage.GetMinusNumberNoticeHtml(this.OrderInput.OrderPriceRegulation, true)%></td>
	</tr>
	<asp:Repeater ID="rPriceCorrection" ItemType="OrderPriceByTaxRateInput" DataSource="<%# this.OrderInput.OrderPriceByTaxRates %>" runat="server"  Visible="<%# (this.OrderInput.ReturnExchangeKbn != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN) %>">
		<ItemTemplate>
			<tr>
				<asp:HiddenField ID="hfTaxCategoryId" runat="server" />
				<asp:HiddenField ID="hfTaxRate" runat="server" />
				<td class="edit_title_bg" align="left">
					返品用金額補正(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate)%>%)
				</td>
				<td class="edit_item_bg" align="right">
					<%# OrderPage.GetMinusNumberNoticeHtml(Item.PriceCorrectionByRate, true) %>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
	<asp:Repeater ID="rPriceTotalByTaxRate" ItemType="OrderPriceByTaxRateInput" DataSource="<%# this.OrderInput.OrderPriceByTaxRates %>" runat="server">
		<ItemTemplate>
			<tr>
				<asp:HiddenField ID="hfTaxCategoryId" runat="server" />
				<asp:HiddenField ID="hfTaxRate" runat="server" />
				<td class="edit_title_bg" align="left">
					合計金額内訳(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate) %>%)
				</td>
				<td class="edit_item_bg" align="right">
					<%# OrderPage.GetMinusNumberNoticeHtml(Item.PriceTotalByRate, true) %>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
	<tr>
		<td class="detail_title_bg" align="left">合計金額</td>
		<td class="detail_item_bg" align="right">
			<%# OrderPage.GetMinusNumberNoticeHtml(this.OrderInput.OrderPriceTotal, true)%></td>
	</tr>
</table>
<%-- △合計金額△--%>
<%--▽ 請求金額情報 ▽--%>
<br />
<table class="detail_table" width="758" cellspacing="1" cellpadding="3" border="0">
	<tr>
		<td class="detail_title_bg" align="right" width="90%">最終請求金額</td>
		<td class="detail_item_bg" align="right" width="10%">
		<%# OrderPage.GetMinusNumberNoticeHtml(this.OrderInput.LastBilledAmount, true) %></td>
	</tr>
	<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
	<tr>
		<td class="detail_title_bg" align="right" width="90%">決済金額</td>
		<td class="detail_item_bg" align="right" width="10%">
			<%# CurrencyManager.ToSettlementCurrencyNotation(decimal.Parse(this.OrderInput.SettlementAmount), this.OrderInput.SettlementCurrency)%></td>
	</tr>
	<%} %>
</table>
<%-- △ 請求金額情報 △--%>

<%--▽ 調整金メモ ▽--%>
<div id="divRegulationMemo" runat="server">
<br />
<table width="758" border="0" cellspacing="0" cellpadding="0">
	<tr valign="top">
		<td align="right">
			<table class="detail_table" cellspacing="1" cellpadding="3" width="306" border="0">
				<tr>
					<td class="detail_title_bg" align="center">
						調整金額メモ
						<uc:FieldMemoSetting runat="server" Title="調整金額メモ" FieldMemoSettingList="<%# this.FieldMemoSettingData %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_REGULATION_MEMO %>" />
					</td>
				</tr>
				<tr>
					<td class="detail_item_bg" align="left">
						<%# WebSanitizer.HtmlEncodeChangeToBr(this.OrderInput.RegulationMemo) %> &nbsp;</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
</div>
<%--△ 調整金メモ △--%>
<%-- 会員ランクOPが有効な場合 --%>
<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
<div id="divMemberRank" runat="server">
<br />
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="2">会員ランク情報</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">注文時会員ランク</td>
		<td class="detail_item_bg" align="left" width="75%"><%#: this.OrderInput.MemberRankName %></td>
	</tr>
</table>
</div>
<%} %>
<%-- ポイントオプションが有効の場合--%>
<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
<div id="divPointUse" runat="server">
<br />
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="2">利用ポイント情報</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">利用ポイント</td>
		<td class="detail_item_bg" align="left" width="75%" >
			<%#: StringUtility.ToNumeric(this.OrderInput.OrderPointUse) %>pt</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">
			最終利用ポイント
			<a runat="server" Visible="<%# (this.OrderUsePointDetail.Length > 0) %>" 
				Title="<%#: CreateOrderPointUseDetailToolTipText() %>">(詳細)</a>
		</td>
		<td class="detail_item_bg" align="left" width="75%" >
			<%#: StringUtility.ToNumeric(this.OrderInput.LastOrderPointUse) %>pt
		</td>
	</tr>
</table>
</div>
<div id="divPointAdd" runat="server">
<br />
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="2">付与ポイント情報</td>
	</tr>
	<%-- 本ポイント付与時 --%>
	<tr id="trOrderPointAdd" runat="server">
		<td class="detail_title_bg" align="left" width="25%">購入時付与ポイント</td>
		<td class="detail_item_bg" align="left" width="75%">
			<%#: StringUtility.ToNumeric(decimal.Parse(this.OrderInput.OrderPointAdd) - PointTemporary) %>pt
		</td>
	</tr>
	<%-- 仮ポイント/期間限定ポイント付与時 --%>
	<asp:Repeater id="rOrderPointAddTemp" ItemType="w2.Domain.Point.UserPointModel" runat="server">
		<ItemTemplate>
			<tr>
				<td class="detail_title_bg" align="left" width="25%">付与ポイント(<%#: ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_INC_KBN, Item.PointIncKbn) %>)</td>
				<td class="detail_item_bg" align="left">
					<%#: StringUtility.ToNumeric(Item.Point) %>pt&nbsp;&nbsp;
					(<%#: ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_TYPE, Item.PointType) %>&nbsp;/
					<%#: ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_KBN, Item.PointKbn) %>)
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
</table>
</div>
<%} %>
<%-- クーポンオプションが有効の場合--%>
<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
<div id="divCoupon" runat="server">
<br />
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">クーポン情報</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">クーポンコード</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: ((this.OrderInput.Coupon != null) && (this.OrderInput.Coupon.CouponCode != "")) ? this.OrderInput.Coupon.CouponCode : "指定無し" %></td>
		<td class="detail_title_bg" align="left" width="25%">クーポン割引額</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: ((this.OrderInput.Coupon != null) && (this.OrderInput.Coupon.CouponCode != "")) ? this.OrderInput.OrderCouponUse.ToPriceString(true) : "-"%></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">クーポン名(ユーザ表示用)</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: ((this.OrderInput.Coupon != null) && (this.OrderInput.Coupon.CouponCode != "")) ? this.OrderInput.Coupon.CouponDispName : "-"%></td>
		<td class="detail_title_bg" align="left" width="25%">クーポン名(管理用)</td>
		<td class="detail_item_bg" align="left" width="25%">
			<%#: ((this.OrderInput.Coupon != null) && (this.OrderInput.Coupon.CouponCode != "")) ? this.OrderInput.Coupon.CouponName : "-" %></td>
	</tr>
</table>
</div>
<%} %>
<%-- Taiwan Order Invoice--%>
<%if (OrderCommon.DisplayTwInvoiceInfo(this.OrderInput.Shippings[0].ShippingCountryIsoCode)) { %>
<div runat="server">
<br />
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">電子発票</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">發票番号</td>
		<td class="detail_item_bg" align="left" width="75%" colspan="3">
			<%#: (this.TwOrderInvoiceInput != null) ? this.TwOrderInvoiceInput.TwInvoiceNo : string.Empty %></td>
	</tr>
	<% if (Constants.TWINVOICE_ECPAY_ENABLED) { %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">發票作成時間</td>
		<td class="detail_item_bg" align="left" width="75%" colspan="3">
			<%#: ((this.TwOrderInvoiceInput != null)
				&& (this.TwOrderInvoiceInput.TwInvoiceDate != null))
					? DateTimeUtility.ToString(
						DateTime.Parse(this.TwOrderInvoiceInput.TwInvoiceDate),
						DateTimeUtility.FormatType.LongDateHourMinute2Letter,
						this.OrderInput.Owner.DispLanguageLocaleId)
					: string.Empty %>
		</td>
	</tr>
	<% } %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">ステータス</td>
		<td class="detail_item_bg" align="left" width="75%" colspan="3">
			<%#: (this.TwOrderInvoiceInput != null) ? ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS, this.TwOrderInvoiceInput.TwInvoiceStatus) : string.Empty %>
			<% if(this.TwOrderInvoiceInput != null) { %>>
				<% if (Constants.TWINVOICE_ENABLED && (this.TwOrderInvoiceInput.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED)) { %>
				<asp:Button ID="btnDownloadIssuedInvoice" Text="  発行済電発票ダウンロード  " OnClick="btnDownloadIssuedInvoice_Click" Runat="server" />
				<% } else if (Constants.TWINVOICE_ENABLED && (this.TwOrderInvoiceInput.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED)) { %>
				<asp:Button ID="btnDownloadRefundedInvoice" Text="  払い戻し済電子発票ダウンロード  " OnClick="btnDownloadRefundedInvoice_Click" Runat="server" />
				<% } %>
			<% } %>
			<asp:Label ID="lbDownloadInvoiceErrorMessage" ForeColor="red" Visible="false" Runat="server" />
			<% if (Constants.TWINVOICE_ECPAY_ENABLED) { %>
			<asp:Button ID="btnInvoiceIssue" runat="server" Visible="<%# CheckDisplayIssueButton() %>" Text="  電子発票発行  " OnClick="btnInvoiceIssue_Click" />
			<asp:Button ID="btnInvoiceCancel" runat="server" Visible="<%# CheckDisplayCancelButton() %>" Text="  電子発票キャンセル  " OnClick="btnInvoiceCancel_Click" />
			<asp:Button ID="btnInvoiceRefund" runat="server" Visible="<%# CheckDisplayCancelButton() %>" Text="  電子発票払い戻し  " OnClick="btnInvoiceRefund_Click" />
			<div ID="dvInvoiceErrorMessages" runat="server" Visible="False">
				<asp:Label ID="lbInvoiceErrorMessages" runat="server" ForeColor="red" />
			</div>
			<% } %>
		</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">發票種類</td>
		<td class="detail_item_bg" align="left" width="75%" colspan="3">
			<%#: (this.TwOrderInvoiceInput != null) ? ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, this.TwOrderInvoiceInput.TwUniformInvoice) : string.Empty %></td>
	</tr>
	<% if (this.IsPersonal) { %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">共通性載具</td>
		<td class="detail_item_bg" align="left" width="75%" colspan="3">
			<%#: GetTwCarryTypeOption() %></td>
	</tr>
	<% } %>
	<% if (this.IsCompany) { %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">統一編号</td>
		<td class="detail_item_bg" align="left" width="75%" colspan="3">
			<%#: (this.TwOrderInvoiceInput != null) ? this.TwOrderInvoiceInput.TwUniformInvoiceOption1 : string.Empty %>
		</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">会社名</td>
		<td class="detail_item_bg" align="left" width="75%" colspan="3">
			<%#: (this.TwOrderInvoiceInput != null) ? this.TwOrderInvoiceInput.TwUniformInvoiceOption2 : string.Empty %>
		</td>
	</tr>
	<% } %>
	<% if (this.IsDonate) { %>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">寄付先コード</td>
		<td class="detail_item_bg" align="left" width="75%" colspan="3">
			<%#: (this.TwOrderInvoiceInput != null) ? this.TwOrderInvoiceInput.TwUniformInvoiceOption1 : string.Empty %></td>
	</tr>
	<% } %>
</table>
</div>
<% } %>
<% if (this.IsStorePickUpOrder == false) { %>
<%-- アフィリエイトオプションが有効の場合--%>
<%if (Constants.W2MP_AFFILIATE_OPTION_ENABLED) { %>
<div id="divAffiliateOP" runat="server">
<br />
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="2">アフィリエイト情報</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">広告コード（初回分）</td>
		<td class="detail_item_bg" align="left" width="75%">
			<%#: this.OrderInput.AdvcodeFirst %></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="25%">広告コード（最新分）</td>
		<td class="detail_item_bg" align="left" width="75%">
			<%# this.OrderInput.AdvcodeNew %></td>
	</tr>
</table>
</div>
<%} %>
<br />
<div>
	<br />
	<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr>
			<td class="detail_title_bg" align="center" colspan="2">コンバージョン情報</td>
		</tr>
		<tr>
			<td class="detail_title_bg" align="left" width="25%">流入コンテンツタイプ</td>
			<td class="detail_item_bg" align="left" width="75%">
				<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE, this.OrderInput.InflowContentsType) %></td>
		</tr>
		<tr>
			<td class="detail_title_bg" align="left" width="25%">流入コンテンツID</td>
			<td class="detail_item_bg" align="left" width="75%">
				<%# this.OrderInput.InflowContentsId %></td>
		</tr>
	</table>
</div>
<% } %>
<%--▽ 注文拡張項目 ▽--%>
<% if (Constants.ORDER_EXTEND_OPTION_ENABLED) { %>
<br />
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="2">注文拡張項目</td>
	</tr>
	<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server">
		<ItemTemplate>
			<tr>
				<td class="detail_title_bg" align="left" width="25%" >
					<%-- 項目名 --%>
					<%#: Item.SettingModel.SettingName %>
				</td>
				<td class="detail_item_bg" align="left" width="75%" >
					<%#: Item.InputText %>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
</table>
<% } %>
<%--△ 注文拡張項目 △--%>
<% if (this.IsStorePickUpOrder == false) { %>
<br />
<!-- ▽注文メモ情報▽ -->
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">
			注文メモ
			<uc:FieldMemoSetting runat="server" Title="注文メモ" FieldMemoSettingList="<%# this.FieldMemoSettingData %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_MEMO %>" />
		</td>
	</tr>
	<tr>
		<td class="detail_item_bg" align="left" colspan="4">
			<%# WebSanitizer.HtmlEncodeChangeToBr(this.OrderInput.Memo) %>&nbsp;</td>
	</tr>
</table>
<!-- △注文メモ情報△ -->
<br />
<!-- ▽決済連携メモ情報▽ -->
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">
			決済連携メモ
			<uc:FieldMemoSetting runat="server" Title="決済連携メモ" FieldMemoSettingList="<%# this.FieldMemoSettingData %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_PAYMENT_MEMO %>" />
		</td>
	</tr>
	<tr>
		<td class="detail_item_bg" align="left" colspan="4">
			<%# WebSanitizer.HtmlEncodeChangeToBr(this.OrderInput.PaymentMemo) %>&nbsp;</td>
	</tr>
</table>
<!-- △決済連携メモ情報△ -->
<% } %>
<asp:LinkButton ID="lbTwInvoiceECPayFocus" runat="server" />
<br />
<!-- ▽管理メモ情報▽ -->
<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tbody>
		<tr>
			<td class="edit_title_bg" align="center" colspan="4">
				管理メモ
				<uc:FieldMemoSetting runat="server" Title="管理メモ" FieldMemoSettingList="<%# this.FieldMemoSettingData %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_MANAGEMENT_MEMO %>" />
			</td>
		</tr>
		<tr>
			<td class="edit_item_bg" align="right" colspan="4">
				<asp:TextBox id="tbManagementMemo" runat="server" Text='<%# this.OrderInput.ManagementMemo %>' TextMode="MultiLine" Rows="8" Width="99%"></asp:TextBox><br />
				<asp:Button ID="btnUpdateManagementMemo" Text="  管理メモ更新  " runat="server" OnClick="btnUpdateManagementMemo_Click" />
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
			<uc:FieldMemoSetting runat="server" Title="配送メモ" FieldMemoSettingList="<%# this.FieldMemoSettingData %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_SHIPPING_MEMO %>" />
		</td>
	</tr>
	<tr>
		<td class="edit_item_bg" align="right" colspan="4">
			<asp:TextBox id="tbShippingMemo" runat="server" Text='<%# this.OrderInput.ShippingMemo %>' TextMode="MultiLine" Rows="8" Width="99%"></asp:TextBox><br />
			<asp:Button ID="btnUpdateShippingMemo" Text="  配送メモ更新  " runat="server" OnClick="btnUpdateShippingMemo_Click" />
		</td>
	</tr>
	</tbody>
</table>
<!-- △配送メモ情報△ -->
<% if (this.IsStorePickUpOrder == false) { %>
<br />
<!-- ▽外部連携メモ▽ -->
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">
			外部連携メモ
			<uc:FieldMemoSetting runat="server" Title="外部連携メモ" FieldMemoSettingList="<%# this.FieldMemoSettingData %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_RELATION_MEMO %>" />
		</td>
	</tr>
	<tr>
		<td class="detail_item_bg" align="left" colspan="4">
			<%# WebSanitizer.HtmlEncodeChangeToBr(this.OrderInput.RelationMemo)%>&nbsp;</td>
	</tr>
</table>
<!-- △外部連携メモ△ -->
<br />
<!-- ▽外部決済連携ログ -->
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">
			外部決済連携ログ
			<uc:FieldMemoSetting ID="FieldMemoSetting1" runat="server" Title="外部決済連携ログメモ" FieldMemoSettingList="<%# this.FieldMemoSettingData %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_EXTERNAL_PAYMENT_COOPERATION_LOG %>" />
		</td>
	</tr>
	<tr>
		<td class="detail_item_bg" align="left" colspan="4">
			<%# WebSanitizer.HtmlEncodeChangeToBr(this.OrderInput.ExternalPaymentCooperationLog)%>&nbsp;</td>
	</tr>
</table>
<% } %>
<br>
<!-- △外部連携決済エラーログ -->
<!-- ▽最終更新者▽ -->
<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="4">最終更新者</td>
	</tr>
	<tr>
		<td class="detail_item_bg" align="left" colspan="4">
			<%#: this.OrderInput.LastChanged %>&nbsp;</td>
	</tr>
</table>

<script type="text/javascript">
	// 商品全選択・全解除
	function selected_item_all() {
		for (i = 0; i < document.getElementsByTagName("input").length; i++) {
			if ((document.getElementsByTagName("input")[i].type == "checkbox")
			&& (((document.getElementsByTagName("input")[i].name).indexOf("cbItemSet") != -1) || ((document.getElementsByTagName("input")[i].name).indexOf("cbItem") != -1))
			&& (document.getElementsByTagName("input")[i].disabled == false)) {
				document.getElementsByTagName("input")[i].checked = true;
			}
		}
	}

	$(document).ready(function () {
		displayMemoPopup();
	});
</script>
