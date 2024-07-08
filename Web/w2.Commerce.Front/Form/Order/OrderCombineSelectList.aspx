<%--
=========================================================================================================
  Module      : 同梱注文選択画面(OrderCombineSelectList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderCombineSelectList.aspx.cs" Inherits="Form_Order_OrderCombineSelectList" Title="同梱注文選択ページ" %>
<%@ Import Namespace="w2.Domain.Order" %>
<%@ Import Namespace="w2.App.Common.Order.OrderCombine" %>
<%@ Import Namespace="w2.App.Common.Order.FixedPurchaseCombine" %>
<%@ Import Namespace="w2.Common.Web" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
<script type="text/javascript">
	function bodyPageLoad() {
		$('input[name=ParentOrder]:checked').closest('table').css('background', '#f1f1f1');

		$('.OrderCombineSelectlist table:not(:first)').click(function (e) {
			$(this).find('input[type=radio]').click(function (e) {
				e.stopPropagation();
			}).click();
		});
	}
</script>
<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<p id="CartFlow"><img src="../../Contents/ImagesPkg/order/cart_step01.gif" alt="お届け先情報入力" width="781" height="58" /></p>

<%-- 次へイベント用リンクボタン --%>
    <asp:LinkButton ID="lbNext" OnClick="lbNext_Click" ValidationGroup="OrderShipping" runat="server"></asp:LinkButton>
<%-- 戻るイベント用リンクボタン --%>
<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server"></asp:LinkButton>
<div class="btmbtn above cartstep">
	<h2 class="ttlB">同梱注文選択</h2>
	<ul>
		<li><a href="<%: this.NextEvent %>" class="btn btn-success" id="hgcNext1"><%=this.NextButtonText %></a></li>
	</ul>
</div>
<div>
	<p>同梱する注文を選択してください<br />
		<span style="color: red;">※お届け先／支払方法は選択した注文と同じになります。</span><br />
		<span style="color: red;">※同梱後の合計金額が支払方法の上限額を超えている場合はお支払方法選択画面に移動します。</span><br />
		<span style="color: red;"><asp:Label Visible="false" runat="server" ID="lbErrorOwnerAddress"></asp:Label></span><br />
		<span style="color: red;"><%= HtmlSanitizer.HtmlEncodeChangeToBr(this.ErrorMessageForFixedAmountCourse) %></span><br />
	</p>
</div>
<div id="hgcOrderCombineSelectlist" runat="server" class="OrderCombineSelectlist">
	<p class="title">同梱対象注文</p>
	<table>
				<tr>
					<td class="radio">
				<w2c:RadioButtonGroup GroupName="ParentOrder" checked="true" AutoPostBack="true" runat="server" value="None" />
					</td>
					<td></td>
			<td>同梱しない</td>
				</tr>
				</table>
	<asp:Repeater ID="rCombineParentOrder" runat="server" ItemType="OrderModel">
		<ItemTemplate>
			<asp:HiddenField runat="server" ID="hfParentPaymentId" value="<%#Item.OrderPaymentKbn %>"/>
			<asp:HiddenField runat="server" ID="hfParentOrderId" value="<%#Item.OrderId %>"/>
			<table>
				<tr>
					<td rowspan="<%#: 7 + (IsSelectedParentOrderWithError(Item.OrderId) ? 1 : 0) %>" class="radio">
						<w2c:RadioButtonGroup ID="rbgCombineParentOrder" GroupName="ParentOrder" OnCheckedChanged="rbgCombineParentOrder_OnCheckedChanged" AutoPostBack="true" Checked="<%# IsSelectedParentOrder(Item.OrderId) %>" runat="server" />
					</td>
					<th>注文ID：</th>
					<td><%#: Item.OrderId %></td>
				</tr>
				<tr>
					<th >商品情報：	</th>
					<td>
						<asp:Repeater ID="rOrderCombineParentItem" DataSource="<%# Item.Shippings[0].Items %>" ItemType="OrderItemModel" runat="server">
							<ItemTemplate>
									<%#: Item.ProductId %>&nbsp;<%#: Item.ProductName %><br />
							</ItemTemplate>
						</asp:Repeater>
					</td>
				</tr>
				<tr>
					<th>
						<label for='<%#: "hgcOrderCombineParentId" + Item.OrderId %>'>お支払額合計：</label>
					</th>
					<td>
						<label for='<%#: "hgcOrderCombineParentId" + Item.OrderId %>'><%#: CurrencyManager.ToPrice(StringUtility.ToNumeric(Item.OrderPriceTotal)) %></label>
					</td>
				</tr>
				<tr>
				<th>お支払方法：</th>
				<td><%#: Item.PaymentName %></td>
				</tr>
				<tr>
				<th>お届け先：</th>
					<td>
						<label for='<%#: "hgcOrderCombineParentId" + Item.OrderId %>'>
							<%#: IsCountryJp(Item.Shippings[0].ShippingCountryIsoCode) ? "〒" + Item.Shippings[0].ShippingZip : "" %>
							<%#: Item.Shippings[0].ShippingAddr1 %>
							<%#: Item.Shippings[0].ShippingAddr2 %>
							<%#: Item.Shippings[0].ShippingAddr3 %>
							<%#: Item.Shippings[0].ShippingAddr4 %>
							<%#: Item.Shippings[0].ShippingAddr5 %>
							<%#: (IsCountryJp(Item.Shippings[0].ShippingCountryIsoCode) == false) ? Item.Shippings[0].ShippingZip : "" %>
							<%#: Item.Shippings[0].ShippingCountryName %><br />
							<%#: Item.Shippings[0].ShippingName %>
						</label>
					</td>
				</tr>
				<tr>
					<th>
						<label for='<%#: "hgcOrderCombineParentId" + Item.OrderId %>'>お届け予定日：</label>
					</th>
					<td>
						<label for='<%#: "hgcOrderCombineParentId" + Item.OrderId %>'><%#: DateTimeUtility.ToStringFromRegion(Item.Shippings[0].ShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay2Letter, "―") %></label>
					</td>
				</tr>
				<tr>
					<th>お届け周期：</th>
					<td>
							<%#: (Item.FixedPurchaseId == "")
								? "―"
								: FixedPurchaseCombineUtility.GetFixedPachasePatternSettingMessage(Item.FixedPurchaseId) %>
					</td>
				</tr>
				<tr Visible="<%# IsSelectedParentOrderWithError(Item.OrderId) %>" runat="server">
					<th></th>
					<td style="color: red;">
						<%# HtmlSanitizer.HtmlEncodeChangeToBr(this.ErrorMessageForSelectedParentOrder.Value) %>
					</td>
				</tr>
				</table>
			</ItemTemplate>
		</asp:Repeater>
</div>
<div class="btmbtn below cartstep">
	<ul>
		<li><a href="<%: this.BackEvent %>" class="btn btn-large btn-org-gry">前のページに戻る</a></li>
		<li><a href="<%: this.NextEvent %>" class="btn btn-large btn-success" id="hgcNext2"><%=this.NextButtonText %></a></li>
	</ul>
</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>
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
</asp:Content>
