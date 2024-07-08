<%--
=========================================================================================================
  Module      : 配送料情報一覧ページ(ShippingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShippingList.aspx.cs" Inherits="Form_Shipping_ShippingList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">配送種別設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">配送種別設定一覧</h2></td>
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
																<td align="center" width="10%" rowspan="2">配送種別ID</td>
																<td align="center" width="30%" rowspan="2">配送種別</td>
																<td align="center" width="10%" colspan="1">配送希望設定</td>
																<td align="center" width="10%" visible="<%# Constants.FIXEDPURCHASE_OPTION_ENABLED %>" runat="server" rowspan="2">定期購入設定</td>
																<td align="center" width="20%" visible="<%# Constants.GIFTORDER_OPTION_ENABLED %>" runat="server" colspan="2">ギフト設定</td>
																<td align="center" width="10%" rowspan="2">決済種別設定</td>
															</tr>
															<tr class="list_title_bg">
																<td align="center" width="5%">日付</td>
																<td align="center" width="5%" visible="<%# Constants.GIFTORDER_OPTION_ENABLED %>" runat="server">のし</td>
																<td align="center" width="5%" visible="<%# Constants.GIFTORDER_OPTION_ENABLED %>" runat="server">包装</td>
															</tr>
													<asp:Repeater id="rList" Runat="server">
														<HeaderTemplate>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateShippingDetailUrl((String)Eval(Constants.FIELD_SHOPSHIPPING_SHIPPING_ID))) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_SHOPSHIPPING_SHIPPING_ID)) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG, (string)Eval(Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG))) %></td>
																<td align="center" visible="<%# Constants.FIXEDPURCHASE_OPTION_ENABLED %>" runat="server"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG, (string)Eval(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG)))%></td>
																<td align="center" visible="<%# Constants.GIFTORDER_OPTION_ENABLED %>" runat="server"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG, (string)Eval(Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG)))%></td>
																<td align="center" visible="<%# Constants.GIFTORDER_OPTION_ENABLED %>" runat="server"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG, (string)Eval(Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG)))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG, (string)Eval(Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG)))%></td>
															</tr>
														</ItemTemplate>
														<AlternatingItemTemplate>
															<tr class="list_item_bg2" onmouseover="listselect_mover(this)" onmouseout="listselect_mout2(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateShippingDetailUrl((String)Eval(Constants.FIELD_SHOPSHIPPING_SHIPPING_ID))) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_SHOPSHIPPING_SHIPPING_ID)) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG, (string)Eval(Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG))) %></td>
																<td align="center" visible="<%# Constants.FIXEDPURCHASE_OPTION_ENABLED %>" runat="server"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG, (string)Eval(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG)))%></td>
																<td align="center" visible="<%# Constants.GIFTORDER_OPTION_ENABLED %>" runat="server"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG, (string)Eval(Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG)))%></td>
																<td align="center" visible="<%# Constants.GIFTORDER_OPTION_ENABLED %>" runat="server"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG, (string)Eval(Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG)))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG, (string)Eval(Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG)))%></td>
															</tr>
														</AlternatingItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="7"></td>
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