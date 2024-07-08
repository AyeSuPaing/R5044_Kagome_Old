<%--
=========================================================================================================
  Module      : 定期購入情報解約入力画面(FixedPurchaseCancelInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FixedPurchase/FixedPurchaseCancelInput.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseCancelInput" Title="定期購入キャンセルの入力" %>
<%@ Import Namespace="w2.App.Common.Global" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<%-- UpdatePanel開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" UpdateMode ="Conditional" runat="server">
<ContentTemplate>
<div id="dvUserFltContents">
	<h2>定期購入情報詳細</h2>
	<div id="dvFixedPurchaseDetail" class="unit">
		<%-- 解約理由 --%>
		<% if (this.IsCancel) { %>
		<div class="dvFixedPurchaseDetail">
			<h3>購入解約</h3>
			<table cellspacing="0">
				<% if (ddlCancelReason.Items.Count != 0) { %>
				<tr>
					<th>解約理由<span class="necessary">*</span></th>
					<td>
						<asp:DropDownList ID="ddlCancelReason" runat="server" Width="500">
						</asp:DropDownList>
						<asp:CustomValidator ID="cvCancelReason" runat="Server"
							ControlToValidate="ddlCancelReason"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<tr>
					<th >解約メモ<span class="necessary">*</span></th>
					<td>
						<asp:TextBox ID="tbCancelMemo" runat="server" TextMode="MultiLine" Rows="5" CssClass="inquirytext" Text="" Width="500"></asp:TextBox>
						<asp:CustomValidator ID="cvCancelMemo" runat="Server"
							ControlToValidate="tbCancelMemo"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
			</table>
		</div>
		<div class="dvUserBtnBox dvFixedPurchase">
			<p>
				<a href="javascript:history.back();" class="btn btn-large">戻る</a>
				<asp:LinkButton Text="確認する" runat="server" ValidationGroup="FixedPurchaseModifyInput" OnClick="lbConfirm_Click" class="btn btn-large btn-inverse"></asp:LinkButton>
			</p>
		</div>
		<% } %>

		<%-- 一時休止理由 --%>
		<% if (this.IsSuspend) { %>
		<div id="dvFixedPurchaseSuspendDetail" class="dvFixedPurchaseDetail" runat="server">
			<h3>購入一時休止</h3>
			<table cellspacing="0">
				<tr>
					<th>再開予定日</th>
					<td>
						<asp:DropDownList ID="ddlResumeDateYear" runat="server" CssClass="year"></asp:DropDownList>&nbsp;/
						<asp:DropDownList ID="ddlResumeDateMonth" runat="server" CssClass="month"></asp:DropDownList>&nbsp;/
						<asp:DropDownList ID="ddlResumeDateDay" runat="server" CssClass="date"></asp:DropDownList>&nbsp;
						<asp:Button Text="クリア" ID="btnResumeDateClear" runat="server"  OnClick="btnResumeDateClear_Click"/>
						<br/><span class="notes">※日付を指定しない場合は、無期限で登録します。</span>
						<asp:CustomValidator id="cvResumeDateYear" runat="Server"
							ControlToValidate="ddlResumeDateYear"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator id="cvResumeDateMonth" runat="Server"
							ControlToValidate="ddlResumeDateMonth"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator id="cvResumeDateDay" runat="Server"
							ControlToValidate="ddlResumeDateDay"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th>次回配送日</th>
					<td>
						<asp:DropDownList ID="ddlNextShippingDateYear" runat="server" CssClass="year"></asp:DropDownList>&nbsp;/
						<asp:DropDownList ID="ddlNextShippingDateMonth" runat="server" CssClass="month"></asp:DropDownList>&nbsp;/
						<asp:DropDownList ID="ddlNextShippingDateDay" runat="server" CssClass="date"></asp:DropDownList>&nbsp;
						<br/><span class="notes">※日付を指定しない場合は、更新しません。</span>
						<br/><span class="notes">※「再開予定日」以降に定期注文の出荷準備を進めてまいります。
						その為、設定によっては「次回配送日」の指定日にお届けできない場合もございますので、予めご了承ください。</span>
						<asp:CustomValidator id="cvNextShippingDateYear" runat="Server"
							ControlToValidate="ddlNextShippingDateYear"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator id="cvNextShippingDateMonth" runat="Server"
							ControlToValidate="ddlNextShippingDateMonth"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator id="cvNextShippingDateDay" runat="Server"
							ControlToValidate="ddlNextShippingDateDay"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th>休止理由<span class="necessary">*</span></th>
					<td>
						<asp:TextBox ID="tbSuspendReason" runat="server" TextMode="MultiLine" Rows="5" CssClass="inquirytext" Text="" Width="500"></asp:TextBox>
						<asp:CustomValidator ID="cvSuspendReason" runat="Server"
							ControlToValidate="tbSuspendReason"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
			</table>
		</div>
		<div class="dvUserBtnBox dvFixedPurchase">
			<p>
				<a href="javascript:history.back();" class="btn btn-large">戻る</a>
				<asp:LinkButton Text="確認する" runat="server" ValidationGroup="FixedPurchaseModifyInput" OnClick="lbSuspendConfirm_Click" class="btn btn-large btn-inverse"></asp:LinkButton>
			</p>
		</div>
		<% } %>

		<%-- 定期購入情報 --%>
		<div class="dvFixedPurchaseDetail">
			<h3>定期購入情報</h3>
			<table cellspacing="0">
				<tr>
					<th>定期購入ID</th>
					<td>
						<%: this.FixedPurchaseContainer.FixedPurchaseId%>
					</td>
				</tr>
				<tr>
					<th>定期購入設定</th>
					<td>
						<%: OrderCommon.CreateFixedPurchaseSettingMessage(this.FixedPurchaseContainer)%>
					</td>
				</tr>
				<tr>
					<th>最終購入日</th>
					<td>
						<%: DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.LastOrderDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter)%>
					</td>
				</tr>
				<tr>
					<th>購入回数</th>
					<td>
						<%: this.FixedPurchaseContainer.OrderCount%>
					</td>
				</tr>
				<tr>
					<th>お支払い方法</th>
					<td>
						<%: this.Payment.PaymentName %>
						<%if (string.IsNullOrEmpty(this.FixedPurchaseContainer.CardInstallmentsCode) == false) { %>
						（<%: ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, this.FixedPurchaseContainer.CardInstallmentsCode)%>払い）
						<% } %>
					</td>
				</tr>
				<% if ((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (this.UserCreditCardInfo != null)){ %>
				<tr>
					<th>利用クレジットカード情報</th>
					<td>
						<% if (this.UserCreditCardInfo.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_ON) { %>
						カード登録名: <%:this.UserCreditCardInfo.CardDispName %><%: this.UserCreditCardInfo.DispFlag ? "" : " (削除済)" %><br />
						<% } %>
						<%if (OrderCommon.CreditCompanySelectable && (this.UserCreditCardInfo.CompanyName != string.Empty)) {%>
						カード会社: <%: this.UserCreditCardInfo.CompanyName %><br />
						<%} %>
						カード番号: XXXXXXXXXXXX<%: this.UserCreditCardInfo.LastFourDigit %><br />
						有効期限: <%: this.UserCreditCardInfo.ExpirationMonth + "/" + this.UserCreditCardInfo.ExpirationYear + " (月/年)" %><br />
						カード名義人: <%: this.UserCreditCardInfo.AuthorName %>
					</td>
				</tr>
				<%} %>
				<tr>
					<th>定期購入ステータス</th>
					<td>
						<span id="spFixedPurchaseStatus" runat="server">
							<%: this.FixedPurchaseContainer.FixedPurchaseStatusText%></span>
					</td>
				</tr>
				<tr>
					<th>決済ステータス</th>
					<td>
						<span id="spPaymentStatus" runat="server">
							<%: this.FixedPurchaseContainer.PaymentStatusText%></span>
					</td>
				</tr>
				<tr>
					<th>次回配送日</th>
					<td>
						<%: this.CanCancelFixedPurchase ? DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.NextShippingDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) : "-"%>
					</td>
				</tr>
				<tr>
					<th>次々回配送日</th>
					<td>
						<%: this.CanCancelFixedPurchase ? DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.NextNextShippingDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) : "-"%>
					</td>
				</tr>
			</table>
		</div>

		<div class="dvFixedPurchaseDetail">
			<%-- お届け先情報 --%>
			<h3>お届け先情報</h3>
			<table cellspacing="0">
				<tr>
					<th>住所</th>
					<td>
						<% if (this.IsShippingAddrCountryJp) { %>〒<%: this.FixedPurchaseShippingContainer.ShippingZip%><br /><% } %>
						<%: this.FixedPurchaseShippingContainer.ShippingAddr1%><%: this.FixedPurchaseShippingContainer.ShippingAddr2%><%: this.FixedPurchaseShippingContainer.ShippingAddr3%><%: this.FixedPurchaseShippingContainer.ShippingAddr4%><br />
						<%: this.FixedPurchaseShippingContainer.ShippingAddr5 %> <% if (this.IsShippingAddrCountryJp == false) { %><%: this.FixedPurchaseShippingContainer.ShippingZip%><br /><% } %>
						<%: this.FixedPurchaseShippingContainer.ShippingCountryName %>
					</td>
				</tr>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
				<tr>
					<%-- 企業名・部署名 --%>
					<th><%: ReplaceTag("@@User.company_name.name@@") %>・
						<%: ReplaceTag("@@User.company_post_name.name@@") %></th>
					<td>
						<%: this.FixedPurchaseShippingContainer.ShippingCompanyName%><br />
						<%: this.FixedPurchaseShippingContainer.ShippingCompanyPostName%>
					</td>
				</tr>
				<% } %>
				<tr>
					<%-- 氏名 --%>
					<th><%: ReplaceTag("@@User.name.name@@") %></th>
					<td>
						<%: this.FixedPurchaseShippingContainer.ShippingName1%><%: this.FixedPurchaseShippingContainer.ShippingName2%>&nbsp;様
						<% if (GlobalAddressUtil.IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode)) { %>
						（<%: this.FixedPurchaseShippingContainer.ShippingNameKana1%><%: this.FixedPurchaseShippingContainer.ShippingNameKana2%>&nbsp;さま）
						<% } %>
					</td>
				</tr>
				<tr>
					<%-- 電話番号 --%>
					<th><%: ReplaceTag("@@User.tel1.name@@") %></th>
					<td>
						<%: this.FixedPurchaseShippingContainer.ShippingTel1%>
					</td>
				</tr>
				<% if (this.DeliveryCompany.IsValidShippingTimeSetFlg) { %>
				<tr>
					<th>配送希望時間帯</th>
					<td>
						<%: this.DeliveryCompany.GetShippingTimeMessage(this.FixedPurchaseShippingContainer.ShippingTime) != "" ? this.DeliveryCompany.GetShippingTimeMessage(this.FixedPurchaseShippingContainer.ShippingTime) : ReplaceTag("@@DispText.shipping_time_list.none@@") %>
					</td>
				</tr>
				<% } %>
			</table>
		</div>
		<br />
		<%-- 購入商品一覧 --%>
		<div class="dvFixedPurchaseItem">
			<table cellspacing="0">
				<asp:Repeater ID="rItem" runat="server">
					<HeaderTemplate>
						<tr>
							<th class="productName">
								商品名
							</th>
							<th class="productPrice">
								単価（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<%if (IsDisplayOptionPrice()){ %>
							<th class="productPrice">
								オプション価格（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<%} %>
							<th class="orderCount">
								注文数
							</th>
							<th class="orderSubtotal">
								小計（<%#: this.ProductPriceTextPrefix %>）
							</th>
						</tr>
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td class="productName">
								<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).CreateProductJointName()) %>
								<br />
								[<span class="productId"><%# WebSanitizer.HtmlEncode((((FixedPurchaseItemInput)Container.DataItem).ProductId == ((FixedPurchaseItemInput)Container.DataItem).VariationId) ? ((FixedPurchaseItemInput)Container.DataItem).ProductId : ((FixedPurchaseItemInput)Container.DataItem).VariationId)%></span>]
								<span visible='<%# ((FixedPurchaseItemInput)Container.DataItem).ProductOptionTexts != "" %>' runat="server">
									<br />
									<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).GetDisplayProductOptionTexts()).Replace("　", "<br />")%>
								</span>
							</td>
							<td class="productPrice">
								<%#: CurrencyManager.ToPrice(((FixedPurchaseItemInput)Container.DataItem).GetValidPrice()) %>
							</td>
							<% if (IsDisplayOptionPrice()){ %>
							<td class="productPrice">
								<%#: ((FixedPurchaseItemInput)Container.DataItem).ProductOptionSettingsWithSelectedValues.HasOptionPrice == false ? "―" : CurrencyManager.ToPrice(((FixedPurchaseItemInput)Container.DataItem).OptionPrice) %>
							</td>
							<% } %>
							<td class="orderCount">
								<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((FixedPurchaseItemInput)Container.DataItem).ItemQuantity))%>
							</td>
							<td class="orderSubtotal">
								<%#: CurrencyManager.ToPrice(((FixedPurchaseItemInput)Container.DataItem).ItemPriceIncludedOptionPrice) %>
							</td>
						</tr>
					</ItemTemplate>
				</asp:Repeater>
			</table>
		</div>
	</div>
</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UpdatePanel終了 --%>
</asp:Content>