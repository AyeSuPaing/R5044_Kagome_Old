<%--
=========================================================================================================
  Module      : 商品同梱設定一覧ページ(ProductBundleList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductBundleList.aspx.cs" Inherits="Form_ProductBundle_ProductBundleList" %>
<%@ Import Namespace="w2.Domain.ProductBundle.Helper" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ Title ▽-->
	<tr>
		<td><h1 class="page-title">商品同梱設定</h1></td>
	</tr>
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ End title △-->
	<!--▽ Search Box ▽-->
	<tr>
	<td>
	<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
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
				<td class="search_box_bg">
				<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
					<tr>
						<td class="search_title_bg" width="95">
							<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品同梱ID
						</td>
						<td class="search_item_bg" width="130">
							<asp:TextBox id="tbBundleId" runat="server" Width="125"/>
						</td>
						<td class="search_title_bg" width="95">
							<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品同梱名
						</td>
						<td class="search_item_bg" width="130">
							<asp:TextBox id="tbBundleName" runat="server" Width="125"/>
						</td>
						<td class="search_title_bg" width="95">
							<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />並び順
						</td>
						<td class="search_item_bg" width="130">
							<asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width">
								<asp:ListItem Value="0" Text="商品同梱ID/昇順" />
								<asp:ListItem Value="1" Text="商品同梱ID/降順" />
								<asp:ListItem Value="2" Text="商品同梱名/昇順" />
								<asp:ListItem Value="3" Text="商品同梱名/降順" />
								<asp:ListItem Value="4" Text="開始日時/昇順" />
								<asp:ListItem Value="5" Text="開始日時/降順" />
								<asp:ListItem Value="6" Text="終了日時/昇順" />
								<asp:ListItem Value="7" Text="終了日時/降順" />
								<asp:ListItem Value="8" Text="適用優先順/昇順" />
								<asp:ListItem Value="9" Text="適用優先順/降順" />
							</asp:DropDownList>
						</td>
						<td class="search_btn_bg" width="83" rowspan="2">
							<div class="search_btn_main">
								<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" />
							</div>
							<div class="search_btn_sub">
								<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTBUNDLE_LIST %>">クリア</a>&nbsp;
								<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
							</div>
						</td>
					</tr>
					<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
					<tr>
						<td class="search_title_bg" width="95">
							<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />対象注文種別
						</td>
						<td class="search_item_bg" width="130">
							<asp:DropDownList id="ddlTargetOrderType" runat="server" CssClass="search_item_width"></asp:DropDownList>
						</td>
						<td class="search_title_bg" width="95">
							<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />対象購入商品ID
						</td>
						<td class="search_item_bg" width="130">
							<asp:TextBox id="tbTargetProductId" runat="server" Width="125"/>
						</td>
						<td class="search_title_bg" width="95">
							<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />同梱商品ID
						</td>
						<td class="search_item_bg" width="130">
							<asp:TextBox id="tbBundleProductId" runat="server" Width="125"/>
						</td>
					</tr>
					<% } %>
				</table>
				</td>
			</tr>
			<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
		</table>
		</td>
		</tr>
	</table>
	</td>
	</tr>
	</table>
	</td>
	</tr>
	<!--△ End Search Box △-->

	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>

	<!--▽ List ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">商品同梱設定一覧</h2></td>
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
								<td width="83" class="action_list_sp" style="height: 22px">
									<asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
								</td>
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
								<td align="center" width="15%">商品同梱ID</td>
								<td align="center" width="43%">商品同梱名</td>
								<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
								<td align="center" width="12%">対象注文種別</td>
								<%} %>
								<td align="center" width="15%">開始日時<br />終了日時</td>
								<td align="center" width="8%">適用<br />優先順</td>
								<td align="center" width="8%">有効<br />フラグ</td>
							</tr>
							<asp:Repeater id="rProductBundleList" ItemType="w2.Domain.ProductBundle.Helper.ProductBundleListSearchResult" Runat="server">
								<ItemTemplate>
									<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>"
										onmouseover="listselect_mover(this)"
										onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)"
										onmousedown="listselect_mdown(this)"
										onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductBundleRegister(Constants.ACTION_STATUS_UPDATE, Item.ProductBundleId)) %>')">
										<td align="center"><%#: Item.ProductBundleId %></td>
										<td align="left"><%#: Item.ProductBundleName %></td>
										<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
										<td align="center">
											<%#: ValueText.GetValueText(Constants.TABLE_PRODUCTBUNDLE, Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE, Item.TargetOrderType) %>
										</td>
										<%} %>
										<td align="center">
											<%#: DateTimeUtility.ToStringForManager(Item.StartDatetime, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %><br />
											<%#: DateTimeUtility.ToStringForManager(Item.EndDatetime, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter, "-") %>
										</td>
										<td align="center">
											<%#: Item.ApplyOrder %>
										</td>
										<td align="center">
											<%#: ValueText.GetValueText(Constants.TABLE_PRODUCTBUNDLE, Constants.FIELD_PRODUCTBUNDLE_VALID_FLG, Item.ValidFlg) %>
										</td>
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