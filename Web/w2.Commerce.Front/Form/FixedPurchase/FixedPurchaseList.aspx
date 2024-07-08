<%--
=========================================================================================================
  Module      : 定期購入情報一覧画面(FixedPurchaseList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FixedPurchase/FixedPurchaseList.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseList" Title="定期購入情報一覧ページ" %>
<%@ Import Namespace="w2.App.Common.LohacoCreatorWebApi.OrderInfo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<div id="dvUserFltContents">
		<h2>定期購入情報一覧</h2>
		<div id="dvFixedPurchaseList" class="unit">
			<div id="sortBox" class="clearFix">
				<asp:Repeater ID="rFixedPurchaseStatusList" EnableViewState="True" runat="server">
					<HeaderTemplate>
						<div class="box clearFix">
						<p class="title">定期購入ステータス</p>
						<ul class="nav clearFix">
					</HeaderTemplate>
					<ItemTemplate>
						<li>
							<a href="<%# CreateUrl( DataBinder.Eval(Container.DataItem, "value").ToString()) %>"  class='<%# CheckParam(DataBinder.Eval(Container.DataItem,"value").ToString()) ? "select" : "" %>' ><%#: Container.DataItem %></a>
						</li>
					</ItemTemplate>
					<FooterTemplate>
						</ul>
						<asp:LinkButton ID="lbContinuing" class="btn btn-mini" OnClick="lbContinuing_OnClick" Text="継続中のみ表示" runat="server" style="margin-left: 20px;" /></div>
					</FooterTemplate>
				</asp:Repeater>
			</div>
			<%-- ページャ --%>
			<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
			<div class="dvFixedPurchaseList">
				
				<%-- 定期購入情報一覧 --%>
				<asp:Repeater ID="rList" ItemType="w2.Domain.FixedPurchase.Helper.UserFixedPurchaseListSearchResult" Runat="server">
					<HeaderTemplate>
					</HeaderTemplate>
					<ItemTemplate>
						<div class="orderBtr">
							<table>
								<tr class="orderBtr">
									<th class="fixedPurchaseId">
										<a href='<%#: PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId) %>'></a>
										定期購入ID
										<p runat="server" Visible="<%# string.IsNullOrEmpty(Item.SubscriptionBoxCourseId) == false %>" style="text-align: center">
											頒布会名
										</p>
									</th>
									<th class="fixedPurchaseStatus">
										定期購入ステータス</th>
									<th class="fixedPurchaseTerm">
										定期購入設定</th>
								</tr>
								<tbody class="orderContents">
									<tr class="orderBtr">
										<td class="fixedPurchaseId">
											<a href='<%#: PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId) %>'>
												<%#: Item.FixedPurchaseId %>
												<p runat="server" Visible="<%# string.IsNullOrEmpty(Item.SubscriptionBoxCourseId) == false %>" style="text-align: center">
													<%#: Item.SubscriptionBoxDisplayName %>
												</p>
											</a>
										</td>
										<td class="fixedPurchaseStatus">
											<span class="fixedPurchaseStatus_<%# Item.FixedPurchaseStatus %>">
												<%#: Item.FixedPurchaseStatusText %>
											</span>
										</td>
										<td class="fixedPurchaseTerm">
											<%#: OrderCommon.CreateFixedPurchaseSettingMessage(Item) %>
										</td>
									</tr>
								</tbody>
								<tr class="orderBtr">
									<th class="lastOrderDate">
										未出荷注文配送希望日</th>
									<th class="nextShippingDate">
										次回配送日</th>
									<th class="fixedPurchaseCount">
										購入回数</th>
								</tr>
								<tbody class="orderContents">
									<tr class="orderBtr">
										<td class="lastOrderDate">
											<%#: GetFixedPurchaseShippingDateNearestValue(Item.FixedPurchaseShippingDateNearest) %>
										</td>
										<td class="nextShippingDate">
											<%#: ((Item.IsCancelFixedPurchaseStatus == false) && (Item.IsCompleteStatus == false)) ? DateTimeUtility.ToStringFromRegion(Item.NextShippingDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) : "-" %>
										</td>
										<td class="fixedPurchaseCount">
											<%#: StringUtility.ToNumeric(Item.OrderCount) %>
										</td>
									</tr>
								</tbody>
							</table>
							<table class="dvFixedPurcharseContain">
								<tr class="orderBtr">
									<th class="orderName">
										<a href='<%#: PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId) %>'></a>
										商品名</th>
									<th class="emptyColumn"></th>
									<th class="itemCount">注文数</th>
								</tr>
								<asp:Repeater ID="rFixedPurchaseItemList" DataSource="<%# GetFixedPurchaseItemContainerList(Item.FixedPurchaseId) %>" ItemType="w2.Domain.FixedPurchase.Helper.FixedPurchaseItemContainer" Runat="server">
									<ItemTemplate>
										<tr class="orderBtr">
											<td class="itemImage">
												<a href='<%#: PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId) %>'></a>
												<div class="itemArea">
													<div class="itemImage">
														<w2c:ProductImage ID="ProductImage1" ImageSize="S" IsVariation="<%# String.IsNullOrEmpty(Item.VariationId) == false %>" ProductMaster="<%# Item.DataSource %>" runat="server" Visible="true" />
													</div>
													<td class="itemTitle">
														<div class="itemTitle">
															<p><%#: Item.Name %></p>
														</div>
													</td>
												</div>
											</td>
											<td class="itemCount">
												<%#: Item.ItemQuantity %>
											</td>
										</tr>
									</ItemTemplate>
								</asp:Repeater>
							</table>
						</div>
				</ItemTemplate>
					<FooterTemplate>
					</FooterTemplate>
			</asp:Repeater>
		</div>
			
			<%--購入履歴なし --%>
			<% if (StringUtility.ToEmpty(this.AlertMessage) != ""){ %>
				<%= this.AlertMessage%>
			<%} %>
			
			<%--ページャ --%>
			<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
		</div>
	</div>
</asp:Content>