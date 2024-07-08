<%--
=========================================================================================================
  Module      : 海外配送エリア一覧ページ(GlobalShippingAreaList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="GlobalShippingAreaList.aspx.cs" Inherits="Form_Shipping_ShippingList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">配送地域設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">配送エリア一覧</h2></td>
	</tr>
	<tr>
		<td>
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
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td class="action_list_sp"><asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
													</tr>
												</table>
												<!-- ページング--></td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tr class="list_title_bg">
																<td align="center" width="10%">表示順</td>
																<td align="center" width="15%">配送エリアID</td>
																<td align="center" width="60%">配送エリア名</td>
																<td align="center" width="15%">有効フラグ</td>
															</tr>
													<asp:Repeater id="rList" Runat="server" ItemType="w2.Domain.GlobalShipping.Helper.GlobalShippingAreaListSearchResult">
														<HeaderTemplate>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl(Item.GlobalShippingAreaId)) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Item.SortNo) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Item.GlobalShippingAreaId) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Item.GlobalShippingAreaName)%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_GLOBALSHIPPINGAREA, Constants.FIELD_GLOBALSHIPPINGAREA_VALID_FLG, Item.ValidFlg)) %></td>
															</tr>
														</ItemTemplate>
														<AlternatingItemTemplate>
															<tr class="list_item_bg2" onmouseover="listselect_mover(this)" onmouseout="listselect_mout2(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl(Item.GlobalShippingAreaId)) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Item.SortNo) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Item.GlobalShippingAreaId) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Item.GlobalShippingAreaName)%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_GLOBALSHIPPINGAREA, Constants.FIELD_GLOBALSHIPPINGAREA_VALID_FLG, Item.ValidFlg)) %></td>
															</tr>
														</AlternatingItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="4"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnInsertBotttom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

</asp:Content>