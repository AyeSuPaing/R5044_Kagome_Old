<%--
=========================================================================================================
  Module      : ユーザポイント履歴詳細ページ(UserPointHistoryDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="UserPointHistoryDetail.aspx.cs" Inherits="Form_UserPoint_UserPointHistoryDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
	<table width="605">
		<tr>
			<td><h1 class="page-title">ユーザーポイント情報</h1></td>
		</tr>
		<tr>
			<td><h2 class="cmn-hed-h2">ユーザーポイント履歴詳細</h2></td>
		</tr>
		<tr><td><br /></td></tr>
		<%-- ▼基本情報▼ --%>
		<tr>
			<td>
				<table class="detail_table">
					<tr>
						<td class="ta-center detail_title_bg" colspan="2">基本情報</td>
					</tr>
					<tr>
						<td width="25%" class="detail_title_bg">ユーザー名</td>
						<td width="75%" class="detail_item_bg">
							<%#: this.Summary.UserName %>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg">ポイント加算区分</td>
						<td class="detail_item_bg">
							<%# this.Summary.PointIncKbn %>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg">ポイント数</td>
						<td class="detail_item_bg">
							<%#: this.Summary.PointTotal %>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg">注文ID</td>
						<td class="detail_item_bg">
							<%#: this.Summary.OrderId %>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg">定期購入ID</td>
						<td class="detail_item_bg">
							<%#: this.Summary.FixedPurchaseId %>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg">履歴作成日時</td>
						<td class="detail_item_bg">
							<%#: this.Summary.DateCreated %>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg">更新者</td>
						<td class="detail_item_bg">
							<%#: this.Summary.LastChanged %>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr><td>　</td></tr>
		<%-- ▲基本情報▲ --%>
		<%-- ▼履歴詳細▼ --%>
		<tr>
			<td>
				<table class="list_table">
					<asp:Repeater ID="rHistoryList" ItemType="Form_UserPoint_UserPointHistoryDetail.ListItemForDisplay" runat="server">
						<HeaderTemplate>
							<tr class="list_title_bg">
								<td class="ta-center" rowspan="2" width="15%">ポイント区分</td>
								<td class="ta-center" rowspan="2" width="15%">ルール設定</td>
								<td class="ta-center" rowspan="2" width="15%">ポイント数</td>
								<%if (Constants.CROSS_POINT_OPTION_ENABLED == false) { %>
								<td class="ta-center" rowspan="2" width="15%">通常ポイント<br/>有効期限延長</td>
								<% } %>
								<td class="ta-center" colspan="2" width="40%">利用可能期間</td>
							</tr>
							<tr class="list_title_bg">
								<td class="ta-center" width="20%">開始</td>
								<td class="ta-center" width="20%">終了</td>
							</tr>
						</HeaderTemplate>
						<ItemTemplate>
							<tr class="list_item_bg<%# ((Container.ItemIndex % 2) + 1) %>" ID="UserPointHistoryDetail_HistoryListItem">
								<td class="ta-center">
									<%#: Item.PointKbn %>
								</td>
								<td class="ta-center">
									<%#: Item.PointRuleKbn ?? "-" %>
								</td>
								<td class="ta-center point_num <%#: Item.PointInc.Contains("-") ? "negative_point" : "" %>">
									<%#: Item.PointInc %>
								</td>
								<%if (Constants.CROSS_POINT_OPTION_ENABLED == false) { %>
								<td class="ta-center">
									<%#: Item.ShouldShowPointExpExtend ? "" : "-" %>
									<%#: Item.ShouldShowPointExpExtend ? Item.PointExpExtendSign : "" %>
									<%#: (Item.ShouldShowPointExpExtend && Item.ShouldShowPointExpExtendYear) ? (Item.PointExpExtendYear + "年") : "" %>
									<%#: Item.ShouldShowPointExpExtend ? (Item.PointExpExtendMonth + "ヶ月") : "" %>
								</td>
								<% } %>
								<td class="ta-center">
									<%#: Item.EffectiveDate ?? "-" %>
								</td>
								<td class="ta-center">
									<%#: Item.PointExp ?? "-" %>
								</td>
							</tr>
						</ItemTemplate>
					</asp:Repeater>
				</table>
			</td>
		</tr>
		<%-- ▲履歴詳細▲ --%>
		<tr><td><br /></td></tr>
	</table>
</asp:Content>