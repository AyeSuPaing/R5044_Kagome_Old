<%--
=========================================================================================================
  Module      : 配送会社情報一覧ページ(DeliveryCompanyList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.DeliveryCompany" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="DeliveryCompanyList.aspx.cs" Inherits="Form_DeliveryCompany_DeliveryCompanyList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ Title ▽-->
	<tr>
		<td><h1 class="page-title">配送サービス設定</h1></td>
	</tr>
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ End title △-->

	<!--▽ List ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">配送サービス設定一覧</h2></td>
	</tr>
	<tr>
		<td style="width: 792px">
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!--▽ paging ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675" style="height: 22px"><asp:Label id="lbPager" Runat="server"></asp:Label></td>
														<td width="83" class="action_list_sp" style="height: 22px"><asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
													</tr>
												</table>
												<!-- paging -->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="15%">配送サービスID</td>
														<td align="center" width="70%">配送サービス名</td>
														<td align="center" width="15%">配送希望時間帯設定</td>
													</tr>
													<asp:Repeater id="rDeliveryCompanyList" ItemType="DeliveryCompanyModel" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>"
																onmouseover="listselect_mover(this)"
																onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)"
																onmousedown="listselect_mdown(this)"
																onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDeliveryCompanyUrl(Item.DeliveryCompanyId, Constants.PAGE_MANAGER_DELIVERY_COMPANY_CONFIRM, Constants.ACTION_STATUS_DETAIL)) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Item.DeliveryCompanyId) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Item.DeliveryCompanyName) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG, Item.ShippingTimeSetFlg)) %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="6"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ List △-->
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>