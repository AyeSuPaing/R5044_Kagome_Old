<%--
=========================================================================================================
  Module      : ブランド検索ページ(ProductBrandSearch.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.ProductBrand" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="ProductBrandSearch.aspx.cs" Inherits="Form_Common_ProductBrandSearch" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="528" border="0">
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="714" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="700" border="0">
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ブランドID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbBrandId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ブランド名</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbBrandName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_btn_bg" width="83">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href='<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_BRAND_SEARCH %>'>クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
												</table>
											</td>
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
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td>
			<h1 class="cmn-hed-h2">商品情報一覧</h1>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="714" border="0">
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
												<!-- ▽ページング▽ -->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="700" border="0">
													<tr>
														<td style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
													</tr>
												</table>
												<!-- △ページング△ -->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="690" border="0">
													
													<tr class="list_title_bg">
														<td align="center" width="20%" style="height: 17px">ブランドID</td>
														<td align="center" width="40%" style="height: 17px">ブランド名</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="javascript:set_brandinfo('<%# WebSanitizer.HtmlEncode(((ProductBrandModel)Container.DataItem).BrandId) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(((ProductBrandModel)Container.DataItem).BrandId) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((ProductBrandModel)Container.DataItem).BrandName) %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="2"></td>
													</tr>
												</table>
											</td>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	// 選択された商品情報を設定
	function set_brandinfo(brand_id) {
		// 親ウィンドウが存在する場合
		if (window.opener != null) {
			// 選択された商品情報を設定    
			window.opener.set_brandinfo(brand_id);
			// 商品一覧ウィンドウ閉じる(ユーザビリティのため)
			window.close();
		}
	}
//-->
</script>
</asp:Content>