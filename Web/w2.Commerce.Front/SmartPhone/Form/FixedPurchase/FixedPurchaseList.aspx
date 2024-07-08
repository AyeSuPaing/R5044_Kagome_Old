<%--
=========================================================================================================
  Module      : スマートフォン用定期購入情報一覧画面(FixedPurchaseList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FixedPurchase/FixedPurchaseList.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseList" Title="定期購入情報一覧ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user fixed-purchase-list">
<div class="user-unit">
	<h2>定期購入情報一覧</h2>
	<p id="pInfo" class="msg" runat="server">※詳細をご覧になるには、IDリンクをタッチしてください。</p>
	<div id="sortBox" class="clearFix">
		<div class="box clearFix" style="width: 90%;margin: 0 auto;max-width: 500px;">
			<h2>定期購入ステータス</h2>
			<div style="display: flex; flex-wrap: wrap;">
			<asp:Repeater ID="rFixedPurchaseStatusList" runat="server">
				<ItemTemplate>
					<div style="width: 33%; line-height: 30px">
						<asp:CheckBox ID="cbFixedPurchaseStatusList" runat="server" Checked='<%# CheckParam(DataBinder.Eval(Container.DataItem,"value").ToString()) %>' Text="<%#: Container.DataItem %>" AutoPostBack="True" OnCheckedChanged="cbFixedPurchaseStatusList_OnCheckedChanged" />
					</div>
				</ItemTemplate>
			</asp:Repeater>
			</div>
		</div>
	</div>
	<asp:LinkButton ID="lbContinuing" Text="継続中のみ表示" OnClick="lbContinuing_OnClick" runat="server" CssClass="btn" />
	<%-- ページャ --%>
	<div class="pager-wrap above"><%= this.PagerHtml %></div>

	<%-- 定期購入情報一覧 --%>
	<asp:Repeater ID="rList" ItemType="w2.Domain.FixedPurchase.Helper.UserFixedPurchaseListSearchResult" Runat="server">
		<HeaderTemplate>
		<div class="content">
		<ul>
		</HeaderTemplate>
		<ItemTemplate>
		<li>
			<h3>定期購入ID</h3>
			<h4 class="fixed-id"><a href='<%#: PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId) %>'>
			<%#: Item.FixedPurchaseId %>
				<span runat="server" Visible="<%# string.IsNullOrEmpty(Item.SubscriptionBoxCourseId) == false %>" style="text-align: center">
					<br />
					<%# Item.SubscriptionBoxDisplayName %>
				</span>
			</a></h4>
			<dl class="user-form">
				<dt>
					定期購入情報
				</dt>
				<dd>
					定期購入ステータス:
					<span class="fixedPurchaseStatus_<%# Item.FixedPurchaseStatus %>">
						<%#: Item.FixedPurchaseStatusText %>
					</span>
					<br />
					
					定期購入設定:
					<%#: OrderCommon.CreateFixedPurchaseSettingMessage(Item) %>
					<br />

					未出荷注文配送希望日:
					<%#: GetFixedPurchaseShippingDateNearestValue(Item.FixedPurchaseShippingDateNearest) %>
					<br />

					次回配送日:
					<%#: ((Item.IsCancelFixedPurchaseStatus == false) && (Item.IsCompleteStatus == false)) ? DateTimeUtility.ToStringFromRegion(Item.NextShippingDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) : "-" %>
					<br />

					購入回数:
					<%#: StringUtility.ToNumeric(Item.OrderCount) %>
				</dd>
			</dl>
			<div class="button">
				<a href='<%#: PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId) %>' class="btn">
				詳細はこちら</a>
			</div>
		</li>
		</ItemTemplate>
		<FooterTemplate>
		</ul>
		</div>
		</FooterTemplate>
	</asp:Repeater>

	<%-- 購入履歴なし --%>
	<% if (StringUtility.ToEmpty(this.AlertMessage) != ""){ %>
		<div class="msg-alert"><%= this.AlertMessage%></div>
	<%} %>
	
	<%-- ページャ --%>
	<div class="pager-wrap below"><%= this.PagerHtml %></div>
</div>

<div class="user-footer">
	<div class="button-next">
		<a href="<%: this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE %>" class="btn">マイページトップへ</a>
	</div>
</div>

</section>
</asp:Content>