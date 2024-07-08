<%--
=========================================================================================================
  Module      : スマートフォン用ポイント履歴一覧画面(UserPointHistoryList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserPointHistoryList.aspx.cs" Inherits="Form_User_UserPointHistoryList" Title="ポイント履歴一覧ページ" %>
<%@ Import Namespace="w2.Domain.Point.Helper" %>
<asp:Content ContentPlaceHolderID="head" Runat="Server">
<link href="<%: Constants.PATH_ROOT + "SmartPhone/Css/user.css" %>" rel="stylesheet" type="text/css" media="all" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<section class="wrap-order point-history-list">

<ContentTemplate>

<div class="point-unit">

	<h2>ポイント履歴一覧</h2>

	<%-- ポイント履歴一覧 --%>
	<asp:Repeater ID="rList" Runat="server">
		<HeaderTemplate>
			<div class="msg">
				<table class="total">
					<tr>
						<th>利用可能ポイント</th>
						<td><%: GetNumeric(this.LoginUserPointUsable) %>pt</td>
					</tr>
					<tr>
						<th>仮ポイント</th>
						<td><%: GetNumeric(this.LoginUserPointTemp) %>pt</td>
					</tr>
					<% if (this.LoginUserPointExpiry.HasValue) { %>
					<tr>
						<th>有効期限</th>
						<td><%: DateTimeUtility.ToStringFromRegion(this.LoginUserPointExpiry, DateTimeUtility.FormatType.LongDate1Letter) %></td>
					</tr>
					<% } %>
				</table>
				※ご注文内容の詳細をご覧になるには【ご注文番号】のリンクを押してください。
			</div>
			<%-- ページャ --%>
			<div class="pager-wrap above"><%= this.PagerHtml %></div>
			<div class="content">
			<ul>
		</HeaderTemplate>
		<ItemTemplate>
			<li>
				<h3><%#: DateTimeUtility.ToStringFromRegion(Eval(POINT_CREATE_DATE), DateTimeUtility.FormatType.ShortDate2Letter) %></h3>
				<h4 class="<%# int.Parse(Eval(Constants.FIELD_USERPOINT_POINT).ToString()) < 0 ? "point-get" : "point-use" %>">
					<%--仮ポイントの場合 --%>
					<span visible="<%# Eval(Constants.FIELD_USERPOINT_POINT_TYPE) as string == Constants.FLG_USERPOINT_POINT_TYPE_TEMP %>" runat="server">
						( <%#:  (int.Parse(Eval(Constants.FIELD_USERPOINT_POINT).ToString()) < 0 ? "" : "+") + GetNumeric(Eval(Constants.FIELD_USERPOINT_POINT)) %> )
					</span>
					<%--本ポイントの場合 --%>
					<span visible="<%# Eval(Constants.FIELD_USERPOINT_POINT_TYPE) as string == Constants.FLG_USERPOINT_POINT_TYPE_COMP %>" runat="server">
						<%#: (int.Parse(Eval(Constants.FIELD_USERPOINT_POINT).ToString()) < 0 ? "" : "+") + GetNumeric(Eval(Constants.FIELD_USERPOINT_POINT)) %>
					</span>
				</h4>
				<dl class="point-form">
					<dt>内容</dt>
					<dd>
						<%# GetPointIncText(Container.DataItem).Replace("<br />", "") %>
						<%#: GetReturnExchangeMessage(Container.DataItem, "(返品含む)", "(交換含む)", "(返品交換含む)") %>
						<span visible="<%# Eval(Constants.FIELD_USERPOINT_POINT_TYPE) as string == Constants.FLG_USERPOINT_POINT_TYPE_TEMP%>" runat="server">
							<br>(仮ポイント
							<%--本ポイント自動付与の場合 --%>
							<%--
								：出荷後に利用可能予定
							--%>
							<%--本ポイントバッチ付与かつ出荷前の場合 --%>
							<span visible="<%# ExistTempToUsablePointDate(Container.DataItem) == false%>" runat="server">
								：出荷<%#: GetTempToCompDays(Container.DataItem) %>日後に利用可能予定
							</span>
							<%--本ポイントバッチ付与かつ出荷後の場合 --%>
							<span visible="<%# ExistTempToUsablePointDate(Container.DataItem) %>" runat="server">
								：<%#: DateTimeUtility.ToStringFromRegion(GetTempToUsablePointDate(Container.DataItem), DateTimeUtility.FormatType.ShortDate2Letter) %>に利用可能予定
							</span>)
						</span>
					</dd>
					<span visible='<%# (StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_ORDER_ID)) != "") || (StringUtility.ToEmpty(Eval(POINT_FIXED_PURCHASE_ID)) != "") %>' runat="server">
						<dt>備考</dt>
						<dd>
							<span visible='<%# StringUtility.ToEmpty(Eval(POINT_FIXED_PURCHASE_ID)) != "" %>' runat="server">
								<a href="<%#: PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(StringUtility.ToEmpty(Eval(POINT_FIXED_PURCHASE_ID))) %>">
									定期購入ID：<%#: Eval(POINT_FIXED_PURCHASE_ID) %>
								</a>
								<span visible='<%# StringUtility.ToEmpty(Eval(POINT_ORDER_ID)) != "" %>' runat="server"><br />→</span>
							</span>
							<span visible='<%# StringUtility.ToEmpty(Eval(POINT_ORDER_ID)) != "" %>' runat="server">
								<a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode(Eval(POINT_ORDER_ID) as string) %>'>
									ご注文番号：<%#: Eval(POINT_ORDER_ID) %>
								</a>
							</span>
							<span visible="<%# StringUtility.ToEmpty(Eval(Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN)) == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURN_NOT_USE_POINT %>" runat="server">
								<br />に利用されなかったポイント分
							</span>
						</dd>
					</span>
				</dl>
			</li>
		</ItemTemplate>
		<FooterTemplate>
			</ul>
			</div>
		</FooterTemplate>
	</asp:Repeater>
	<%-- 購入履歴なし--%>
	<% if(StringUtility.ToEmpty(this.AlertMessage) != "") {%>
	<div class="msg-alert">
		<%= this.AlertMessage %>
	</div>
	<%} %>

	<%-- ページャ--%>
	<div class="pager-wrap below"><%= this.PagerHtml %></div>
</div>

<div class="user-footer">
	<div class="button-next">
		<a href="<%: this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE %>" class="btn">マイページトップへ</a>
	</div>
</div>

</ContentTemplate>

</section>
</asp:Content>
