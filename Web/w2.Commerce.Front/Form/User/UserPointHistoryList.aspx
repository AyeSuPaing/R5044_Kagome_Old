<%--
=========================================================================================================
  Module      : ポイント履歴一覧画面(UserPointHistoryList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserPointHistoryList.aspx.cs" Inherits="Form_User_UserPointHistoryList" Title="ポイント履歴一覧ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">

	<h2>ポイント履歴一覧</h2>

	<div id="dvUserPointHistoryList" class="unit">
		<p style="font-size:15px;line-height:1.5;margin-bottom:5px;">
			利用可能ポイント ：<%: GetNumeric(this.LoginUserPointUsable) %>pt<br/>
			仮ポイント ：( <%: GetNumeric(this.LoginUserPointTemp) %>pt )<br/>
			<% if (this.LoginUserPointExpiry.HasValue) { %>
			有効期限 ：<%: DateTimeUtility.ToStringFromRegion(this.LoginUserPointExpiry, DateTimeUtility.FormatType.LongDate1Letter) %>
			<% } %>
		</p>
		
		<%-- ページャ --%>
		<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
		<div class="dvUserPointHistoryList">
		
		<%-- ポイント履歴一覧 --%>
			<asp:Repeater ID="rList" Runat="server">
				<HeaderTemplate>
				<table cellspacing="0">
					<tr>
						<th class="userPointDate">
							獲得・利用日</th>
						<th class="userPointContent">
							内容</th>
						<th class="point">
							ポイント数</th>
						<th class="orderNum">
							備考</th>
					</tr>
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td class="userPointDate">
							<%#: DateTimeUtility.ToStringFromRegion(Eval(POINT_CREATE_DATE), DateTimeUtility.FormatType.ShortDate2Letter) %>
						</td>
						<td class="userPointContent">
							<%#: GetPointIncText(Container.DataItem) %>
							<%#: GetReturnExchangeMessage(Container.DataItem, "(返品含む)", "(交換含む)", "(返品交換含む)") %>
							<span visible="<%# Eval(Constants.FIELD_USERPOINT_POINT_TYPE) as string == Constants.FLG_USERPOINT_POINT_TYPE_TEMP%>" runat="server">
								<br>(仮ポイント
								<%--本ポイント自動付与の場合 --%>
								<%--
								    ：出荷後に利用可能予定
								--%>
								<%--本ポイントバッチ付与かつ出荷前の場合 --%>
								<span visible="<%# ExistTempToUsablePointDate(Container.DataItem) == false %>" runat="server">
									：出荷<%#: GetTempToCompDays(Container.DataItem) %>日後に利用可能予定
								</span>
								<%--本ポイントバッチ付与かつ出荷後の場合 --%>
								<span visible="<%# ExistTempToUsablePointDate(Container.DataItem) %>" runat="server">
									：<%#: DateTimeUtility.ToStringFromRegion(GetTempToUsablePointDate(Container.DataItem), DateTimeUtility.FormatType.ShortDate2Letter) %>に利用可能予定
								</span>)
							</span>
						</td>
						<td class="point">
							<%--仮ポイントの場合 --%>
							<span visible="<%# Eval(Constants.FIELD_USERPOINT_POINT_TYPE) as string == Constants.FLG_USERPOINT_POINT_TYPE_TEMP %>" runat="server">
								( <%#: (int.Parse(Eval(Constants.FIELD_USERPOINT_POINT).ToString()) < 0 ? "" : "+") + GetNumeric(Eval(Constants.FIELD_USERPOINT_POINT)) %> )
							</span>
							<%--本ポイントの場合 --%>
							<span visible="<%# Eval(Constants.FIELD_USERPOINT_POINT_TYPE) as string == Constants.FLG_USERPOINT_POINT_TYPE_COMP %>" runat="server">
								<%#: (int.Parse(Eval(Constants.FIELD_USERPOINT_POINT).ToString()) < 0 ? "" : "+") + GetNumeric(Eval(Constants.FIELD_USERPOINT_POINT)) %>
							</span>
						</td>
						<td class="orderNum">
							<span visible='<%# StringUtility.ToEmpty(Eval(POINT_FIXED_PURCHASE_ID)) != "" %>' runat="server">
								<a href="<%#: PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(StringUtility.ToEmpty(Eval(POINT_FIXED_PURCHASE_ID))) %>">
									定期購入ID：<%#: Eval(POINT_FIXED_PURCHASE_ID) %>
								</a>
								<span visible='<%# StringUtility.ToEmpty(Eval(POINT_ORDER_ID)) != "" %>' runat="server"><br />→</span>
							</span>
							<span visible='<%# StringUtility.ToEmpty(Eval(POINT_ORDER_ID)) != "" %>' runat="server">
								<a href='<%#: string.Format("{0}{1}?{2}={3}", Constants.PATH_ROOT, Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL, Constants.REQUEST_KEY_ORDER_ID, Eval(POINT_ORDER_ID)) %>'>
									ご注文番号：<%#: Eval(POINT_ORDER_ID) %>
								</a>
							</span>
							<span visible="<%# StringUtility.ToEmpty(Eval(Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN)) == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURN_NOT_USE_POINT %>" runat="server">
								<br />に利用されなかったポイント分
							</span>
						</td>
					</tr>
				</ItemTemplate>
				<FooterTemplate>
				</table>
				</FooterTemplate>
			</asp:Repeater>
		</div>

		<%--ポイント履歴なし --%>
		<% if (StringUtility.ToEmpty(this.AlertMessage) != ""){ %>
			<%= this.AlertMessage%>
		<%} %>

		<%--ページャ --%>
		<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
	</div>
</div>
</asp:Content>
