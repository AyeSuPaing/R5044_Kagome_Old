<%--
=========================================================================================================
  Module      : 定期商品変更設定一覧画面(FixedPurchaseProductChangeSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Form/Common/DefaultPage.master" CodeFile="FixedPurchaseProductChangeSettingList.aspx.cs" Inherits="Form_FixedPurchaseProductChangeSetting_FixedPurchaseProductChangeSettingList" %>
<%@ Import Namespace="w2.Domain.FixedPurchaseProductChangeSetting.Helper" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr><td><h1 class="page-title">定期商品変更設定</h1></td></tr>
	<tr>
		<td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table class="box_border cmn-section" cellspacing="1" cellpadding="3" width="784" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0" class="wide-content">
										<tr><td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<tr>
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															定期商品変更設定ID
														</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox runat="server" ID="tbFixedPurchaseProductChangeId" />
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															定期商品変更設定名
														</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox runat="server" ID="tbFixedPurchaseProductChangeName" />
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順
														</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width" />
														</td>
														<td class="search_btn_bg" width="83" rowspan="2">
															<div class="search_btn_main"><asp:Button runat="server" ID="btnSearch" Text="　検索　" OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_LIST %>">クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															変更元商品ID
														</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox runat="server" ID="tbBeforeChangeProductId" />
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															変更後商品ID
														</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox runat="server" ID="tbAfterChangeProductId" />
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															有効フラグ
														</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlValidFlg" runat="server" CssClass="search_item_width" />
														</td>
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
			</table>
		</td>
	</tr>
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">定期商品変更設定一覧</h2></td>
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
											<td><img height="6" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675" style="height: 22px"><asp:Label ID="lbPager1" runat="server" /></td>
														<td class="action_list_sp"><asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="120">定期商品変更設定ID</td>
														<td align="center" width="258">定期商品変更設定名</td>
														<td align="center" width="120">適用優先順</td>
														<td align="center" width="70">有効フラグ</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateDetailUrl(((FixedPurchaseProductChangeSettingListSearchResult)Container.DataItem).FixedPurchaseProductChangeId) %>')">
																<td align="center"><%#: ((FixedPurchaseProductChangeSettingListSearchResult)Container.DataItem).FixedPurchaseProductChangeId %></td>
																<td align="left"><%#: ((FixedPurchaseProductChangeSettingListSearchResult)Container.DataItem).FixedPurchaseProductChangeName %></td>
																<td align="center"><%#: ((FixedPurchaseProductChangeSettingListSearchResult)Container.DataItem).Priority %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASEPRODUCTCHANGESETTING, Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG, ((FixedPurchaseProductChangeSettingListSearchResult)Container.DataItem).ValidFlg)) %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="6"></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
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
		<td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
