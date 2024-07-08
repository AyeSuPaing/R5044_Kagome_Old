<%--
=========================================================================================================
  Module      : スマートフォン用注文決済画面(OrderSettlement.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderSettlement.aspx.cs" Inherits="Form_Order_OrderSettlement" Title="注文決済ページ" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScriptForPaygent.ascx" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paygent.Paidy.Checkout" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<p style="padding-top: 4px">
<span style="color:Red">
別途認証が必要な注文があります。<br />
「決済する」ボタンをクリックして決済を行ってください。
</span>
</p>

<asp:HiddenField ID="hfPaidyPaymentId" runat="server" />
<asp:HiddenField ID="hfPaidySelect" runat="server" />
<asp:HiddenField ID="hfPaidyStatus" runat="server" />

<asp:Repeater ID="rOrder" OnItemCommand="rOrder_ItemCommand" runat="server">
<ItemTemplate>
	<div class="panel CartArea" style="padding-top: 4px">
	<h2>注文番号：<asp:Literal ID="lOrderId" Text="<%# ((CartObject)Container.DataItem).OrderId %>" runat="server"></asp:Literal></h2>
		<div style="padding-top: 3px;">
		<fieldset>
			<div class="row">
				<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).Items %>" runat="server">
				<ItemTemplate>
					<%-- 通常商品 --%>
					<div visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false %>" runat="server">
						<label><%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductName) %></label>
						<small>数量：<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((CartProduct)Container.DataItem).Count))%></small>
					</div>
					<%-- セット商品 --%>
					<div visible="<%# (((CartProduct)Container.DataItem).IsSetItem) && (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
						<asp:Repeater DataSource="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items : null %>" runat="server">
						<HeaderTemplate>
							<label>
						</HeaderTemplate>
						<ItemTemplate>
							<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %> x <%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((CartProduct)Container.DataItem).CountSingle)) %><br />
						</ItemTemplate>
						<FooterTemplate>
							</label>
						</FooterTemplate>
						</asp:Repeater>
						<small>数量：<%# WebSanitizer.HtmlEncode(GetProductSetCount((CartProduct)Container.DataItem))%></small>
					</div>

				</ItemTemplate>
				</asp:Repeater>
			</div>
			<div class="row">
				合計金額：<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceTotal) %>
			</div>
			<div class="cart-footer">
				<div visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) != "under_examination_yamato_ka_sms" %>' runat="server"><span  style="color: red"><%= WebSanitizer.HtmlEncodeChangeToBr(this.SmsAuthErrorMessage) %></span><br/></div>
				<div visible='<%# GetCanSettlement(((CartObject)Container.DataItem).OrderId) %>' runat="server" class="button-next order">
					<asp:HiddenField ID="hfCartIndex" runat="server" Value="<%# Container.ItemIndex %>" />
					<div style="display: none;">
						<asp:LinkButton ID="lbPaidySettlement" runat="server" Text="決済する" OnClick="lbPaidySettlement_Click" />
					</div>
					<asp:LinkButton id="lbSettlement" runat="server" CommandName="settlement" CommandArgument="<%# Container.ItemIndex %>" class="btn" OnClientClick="<%# GeneratePaidyPaygentScript(((CartObject)Container.DataItem), Container) %>">決済する</asp:LinkButton>
				</div>
				<div class="status" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "success" %>' runat="server">注文完了</div>
				<div class="status" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "incomplete_docomo" %>' runat="server">注文完了</div>
				<div class="status" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "failure" %>' runat="server">注文失敗</div>
				<div class="status" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "cancel" %>' runat="server">キャンセル済</div>
				<div style="padding: 5px 30px 0px 30px;" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "under_examination_yamato_ka_sms" %>' runat="server">
					電話番号<%#: GetTelNumForSmsSend((CartObject)Container.DataItem) %>宛てに<br/>SMSを送信いたしました。<br/>3分以内にSMSに記載の4桁の認証コードを入力してください。<br/>
					SMSが届かない場合は<asp:LinkButton ID="LinkButton1" runat="server" style="color:#0000EE" Text="こちら" CommandName="transittelnuminput" CommandArgument="<%# Container.ItemIndex %>" />をクリックし、SMS受信可能な携帯電話番号を設定してください。<br/>
					<span style="color: red"><%= WebSanitizer.HtmlEncodeChangeToBr(this.SmsAuthErrorMessage) %></span><br/>
					<div style="text-align: center"><asp:TextBox TextMode="Number" style="width:6em" placeholder="認証コード" min="3" MaxLength="4" Columns="6" ID="tbAuthCode" runat="server"/><br/><br/>
						<div class="button-next order"><asp:LinkButton id="lbSmsAuth" runat="server" CommandName="smsauth" CommandArgument="<%# Container.ItemIndex %>" class="btn">認証する</asp:LinkButton></div>
					</div>
				</div>
				<div style="padding: 5px 30px 0px 30px;" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "under_input_telnum_yamato_ka_sms" %>' runat="server">
					4桁の認証コードが記載されたSMSを送信いたします。<br/>
					SMSが受信可能な携帯電話番号をハイフンなしで入力してください。<br/><br/>
					<span style="color: red"><%= WebSanitizer.HtmlEncodeChangeToBr(this.SmsAuthErrorMessage) %></span><br/>
					<div style="text-align: center"><asp:TextBox ID="tbCellPhoneNumber" TextMode="Phone" placeholder="携帯電話番号" Columns="8" MaxLength="11" runat="server"/></div>
					<div class="button-next order"><asp:LinkButton id="LinkButton2" Text="SMSを送信する" class="btn" runat="server" CommandName="smssend" CommandArgument="<%# Container.ItemIndex %>" /></div>
				</div>
			</div>
		</fieldset>
		</div>
	</div>

</ItemTemplate>
</asp:Repeater>
<div class="cart-footer">
	<div class="button-prev">
		<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" Text="戻る" class="btn" runat="server" visible='<%# CartList.Items[0].Payment.IsPaymentYamatoKaSms %>'/>
	</div>
</div>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<%--▼▼ Paidy用スクリプト ▼▼--%>
<script type="text/javascript">
	var hfPaidyPaymentIdControlId = "<%= (this.WhfPaidyPaymentId.ClientID) %>";
	var hfPaidyStatusControlId = "<%= (this.WhfPaidyStatus.ClientID) %>";
	var isHistoryPage = false;
	var body;
	var customBody;
</script>
<uc:PaidyCheckoutScript runat="server" />
</asp:Content>
