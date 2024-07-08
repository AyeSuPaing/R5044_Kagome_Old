<%--
=========================================================================================================
  Module      : カテゴリ検索ページ(ProductCategorySearch.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="ProductCategorySearch.aspx.cs" Inherits="Form_Common_ProductCategorySearch" Title="商品カテゴリ情報一覧" %>
<%@ Import Namespace="System.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="528" border="0">
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="528" border="0">
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
												<table cellspacing="1" cellpadding="2" width="504" border="0">
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															カテゴリID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbCategoryId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															カテゴリ名</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_btn_bg" width="83">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_CATEGORY_SEARCH %>">クリア</a>
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
			<h1 class="cmn-hed-h2">商品カテゴリ情報一覧</h1>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="528" border="0">
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
												<table class="list_pager" cellspacing="0" cellpadding="0" width="504" border="0">
													<tr>
														<td width="504" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
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
												<table class="list_table" cellspacing="1" cellpadding="3" width="504" border="0">
													<tr class="list_title_bg">
														<td align="center" width="30%" style="height: 17px">カテゴリID</td>
														<td align="center" width="70%" style="height: 17px">カテゴリ名</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="javascript:set_categoryinfo(<%# WebSanitizer.HtmlEncode(CreateJavaScriptSetCategoryInfo((DataRowView)Container.DataItem)) %>)">
																<td align="left">
																	<div style="margin-left:<%# (((string)Eval(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID)).Length/Constants.CONST_CATEGORY_ID_LENGTH -1)*10 %>px;">
																		<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID)) %>
																	</div>
																</td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTCATEGORY_NAME)) %></td>
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
	// 選択された商品カテゴリ情報を設定
	function set_categoryinfo(category_id, name) {
		// 親ウィンドウが存在する場合
		if (window.opener != null) {
			// 選択された商品カテゴリ情報を設定    
			window.opener.set_categoryinfo(category_id, name);
			// 商品カテゴリ一覧ウィンドウ閉じる(ユーザビリティのため)
			window.close();
		}
	}
//-->
</script>
</asp:Content>