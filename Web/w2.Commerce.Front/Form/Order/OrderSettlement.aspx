<%--
=========================================================================================================
Module      : 注文決済画面(OrderSettlement.aspx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderSettlement.aspx.cs" Inherits="Form_Order_OrderSettlement" Title="注文決済ページ" %>
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
<table id="tblLayout">
<tr>
<td>
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<h2 class="ttlC">外部決済</h2>
<p class="pdg_bottomA">
<span style="color:Red;">
別途認証が必要な注文があります。<br />
「決済する」ボタンをクリックして決済を行ってください。
</span>
</p>

<asp:HiddenField ID="hfPaidyPaymentId" runat="server" />
<asp:HiddenField ID="hfPaidyStatus" runat="server" />

<asp:Repeater ID="rOrder" OnItemCommand="rOrder_ItemCommand" runat="server">
<ItemTemplate>
	<div class="orderSettlement">
	<div class="background">
	<div class="bottom">
	<h3>ご注文明細</h3>
	
	<div class="orderDetail">
	<div class="suborderDetail">
	<em>注文番号：&nbsp;<asp:Literal ID="lOrderId" Text="<%# ((CartObject)Container.DataItem).OrderId %>" runat="server"></asp:Literal></em>
	<div class="productList">
	<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).Items %>" runat="server">
	<ItemTemplate>
		<%-- 通常商品 --%>
		<div visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false %>" runat="server">
		<div>
			<table cellpadding="0" cellspacing="0" width="100%">
			<tr>
			<td class="name"><%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductName) %></td>
			<td class="quantity">数量&nbsp;&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((CartProduct)Container.DataItem).Count)) %></td>
			</tr>
			</table>
		</div>
		<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</div>
		<%-- セット商品 --%>
		<div visible="<%# (((CartProduct)Container.DataItem).IsSetItem) && (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
		<div>
		<asp:Repeater DataSource="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items : null %>" runat="server">
		<HeaderTemplate>
			<table cellpadding="0" cellspacing="0" width="100%">
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
			<td class="name"><%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %> x <%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((CartProduct)Container.DataItem).CountSingle)) %></td>
			<td rowspan="<%# ((CartProduct)Container.DataItem).ProductSet.Items.Count %>" visible="<%# ((CartProduct)Container.DataItem).ProductSetItemNo == 1 %>" class="quantity" runat="server">
				数量&nbsp;&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(GetProductSetCount((CartProduct)Container.DataItem))%></td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
		</asp:Repeater>
		</div>
		<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</div>
	</ItemTemplate>
	</asp:Repeater>
	</div>

	<div class="exec">
	合計金額：&nbsp;<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceTotal) %><br />
	<div class="status" visible='<%# GetCanSettlement(((CartObject)Container.DataItem).OrderId) %>' runat="server">
		<asp:HiddenField ID="hfCartIndex" runat="server" Value="<%# Container.ItemIndex %>" />
		<div style="display: none;">
			<asp:LinkButton ID="lbPaidySettlement" runat="server" Text="決済する" OnClick="lbPaidySettlement_Click" />
		</div>
		<asp:LinkButton id="lbSettlement" class="btn btn-success" runat="server" CommandName="settlement" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="<%# GeneratePaidyPaygentScript(((CartObject)Container.DataItem), Container) %>">決済する</asp:LinkButton>
	</div>
	<div class="status" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "success" %>' runat="server">注文完了</div>
	<div class="status" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "incomplete_docomo" %>' runat="server">注文完了</div>
	<div class="status" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "failure" %>' runat="server">注文失敗</div>
	<div class="status" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "cancel" %>' runat="server">キャンセル済</div>
	</div>
	<br class="clr" />
	</div>
	</div><!--orderDetail-->
	<div style="padding: 5px 30px 10px 30px;text-align: center" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "under_examination_yamato_ka_sms" %>' runat="server">
		電話番号<%#: GetTelNumForSmsSend((CartObject)Container.DataItem) %>宛てにSMSを送信いたしました。<br/>3分以内にSMSに記載の4桁の認証コードを入力してください。<br/>
		SMSが届かない場合は<asp:LinkButton runat="server" style="color:#0000EE" Text="こちら" CommandName="transittelnuminput" CommandArgument="<%# Container.ItemIndex %>" />をクリックし、SMS受信可能な携帯電話番号を設定してください。<br/>
		<span style="color: red"><%= WebSanitizer.HtmlEncodeChangeToBr(this.SmsAuthErrorMessage) %></span><br/>
		<div style="text-align: center"><asp:TextBox TextMode="Number" style="width:6em" placeholder="認証コード" min="0" MaxLength="4" Columns="3" ID="tbAuthCode" runat="server"/></div><br/>
		<div style="padding-top:5px"><asp:LinkButton id="lbSmsAuth" Text="認証する" class="btn btn-success" style="text-align: right;" runat="server" CommandName="smsauth" CommandArgument="<%# Container.ItemIndex %>" /></div>
	</div>
	<div style="padding: 5px 30px 10px 30px;text-align: center" visible='<%# GetSettlementStatus(((CartObject)Container.DataItem).OrderId) == "under_input_telnum_yamato_ka_sms" %>' runat="server">
		4桁の認証コードが記載されたSMSを送信いたします。<br/>
		SMSが受信可能な携帯電話番号をハイフンなしで入力してください。<br/><br/>
		<span style="color: red"><%= WebSanitizer.HtmlEncodeChangeToBr(this.SmsAuthErrorMessage) %><br/></span><br/>
		<asp:TextBox ID="tbCellPhoneNumber" TextMode="Phone" placeholder="携帯電話番号" Columns="8" MaxLength="11" runat="server"/>
		<br/><br/>
		<div style="padding-top:5px"><asp:LinkButton id="LinkButton1" Text="SMSを送信する" class="btn btn-success" style="text-align: right;" runat="server" CommandName="smssend" CommandArgument="<%# Container.ItemIndex %>" /></div>
	</div>
	<div style="text-align: center" visible='<%# (GetSettlementStatus(((CartObject)Container.DataItem).OrderId) != "under_examination_yamato_ka_sms") && (GetSettlementStatus(((CartObject)Container.DataItem).OrderId) != "under_input_telnum_yamato_ka_sms") %>' runat="server"><span style="color: red"><%= WebSanitizer.HtmlEncodeChangeToBr(this.SmsAuthErrorMessage) %></span></div><br/>
	</div><!--bottom-->
	</div><!--background-->
	</div><!--orderSettlement-->

</ItemTemplate>
</asp:Repeater>
<div class="btmbtn below">
	<ul>
		<li><asp:LinkButton ID="lbBack" Text="前のページに戻る" OnClick="lbBack_Click" runat="server" class="btn btn-large btn-org-gry" visible='<%# CartList.Items[0].Payment.IsPaymentYamatoKaSms %>'/></li>
	</ul>
</div>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
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
