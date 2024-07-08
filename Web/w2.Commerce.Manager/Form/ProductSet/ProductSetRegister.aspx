<%--
=========================================================================================================
  Module      : 商品セット設定登録ページ(ProductSetRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductSetRegister.aspx.cs" Inherits="Form_ProductSet_ProductSetRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">商品セット設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 編集入力 ▽-->
	<tr>
		<td>
			<% if ((m_strActionStatus == Constants.ACTION_STATUS_INSERT) || (m_strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)){%>
				<h2 class="cmn-hed-h2">商品セット設定登録</h2>
			<%} %>
			<% if (m_strActionStatus == Constants.ACTION_STATUS_UPDATE) {%>
				<h2 class="cmn-hed-h2">商品セット設定編集</h2>
			<%} %>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top"><input onclick="Javascript:history.back()" type="button" value="  戻る  " />
												<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " onclick="btnConfirm_Click"></asp:Button>
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trMailtextId" runat="server" visible="false">
														<td class="edit_title_bg" align="left" width="30%">商品セットID</td>
														<td class="edit_item_bg" align="left">
															<asp:Literal id="lProductSetId" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">商品セット名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbProductSetName" runat="server" MaxLength="30" Width="200"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">親商品数(上下限)</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbParentMin" runat="server" MaxLength="6" Width="50"></asp:TextBox> 以上
															<asp:TextBox id="tbParentMax" runat="server" MaxLength="6" Width="50"></asp:TextBox> 以下</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">子商品数(上下限)</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbChildMin" runat="server" MaxLength="6" Width="50"></asp:TextBox> 以上
															<asp:TextBox id="tbChildMax" runat="server" MaxLength="6" Width="50"></asp:TextBox> 以下</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">表示用文言</td>
														<td class="edit_item_bg" align="left">
															<asp:RadioButtonList ID="rblDescriptionKbn" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server">
																<asp:ListItem Text="TEXT" Value="0" Selected="True"></asp:ListItem>
																<asp:ListItem Text="HTML" Value="1"></asp:ListItem>
															</asp:RadioButtonList>
															<input type= "button" onclick="javascript:open_wysiwyg('<%= tbDescription.ClientID %>', '<%= rblDescriptionKbn.ClientID %>');" value="  HTMLエディタ  " /><br />
															<asp:TextBox id="tbDescription" runat="server" TextMode="MultiLine" Width="450" Height="50"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">1注文購入限度数<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbMaxSellQuantity" runat="server" MaxLength="6" Width="50"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">有効フラグ</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbValidFlg" runat="server" Checked="true" Text="有効" /></td>
													</tr>
												</table>
												
												
												<div class="action_part_middle">
													<input id="inputSearchProduct" type="button" value="  商品選択  " onclick="javascript:open_product_list('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes');" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" width="40"><asp:Button ID="btnDelete" Text="  削除  " runat="server" OnClick="btnDelete_Click" /></td>
														<td class="edit_title_bg" align="center" width="70">商品ID</td>
														<td class="edit_title_bg" align="center" width="70">バリエーションID</td>
														<td class="edit_title_bg" align="left" width="250">商品名</td>
														<td class="edit_title_bg" align="center" width="60">商品価格（<%: this.ProductPriceTextPrefix %>）</td>
														<td class="edit_title_bg" align="center" width="68">セット<br />商品価格（<%: this.ProductPriceTextPrefix %>）<span class="notice">*</span></td>
														<td class="edit_title_bg" align="center" width="80">個数制限</td>
														<td class="edit_title_bg" align="center" width="80">親子<br />フラグ<span class="notice">*</span></td>
														<td class="edit_title_bg" align="center" width="50">表示順<span class="notice">*</span></td>
													</tr>
													<asp:Repeater ID="rProductList" runat="server">
														<ItemTemplate>
														<tr>
															<td class="edit_item_bg" align="center"><asp:CheckBox ID="cbProductSelect" runat="server" /></td>
															<td class="edit_item_bg" align="center">
																<%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).ProductId)%>
																<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((ProductSetItem)Container.DataItem).ProductId %>" />
															</td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).VId)%></td>
															<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).Name)%></td>
															<td class="edit_item_bg" align="right"><%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).Price.ToPriceString(true))%></td>
															<td class="edit_item_bg" align="center"><asp:TextBox ID="tbProductSetItemPrice" Width="50" MaxLength="7" runat="server"></asp:TextBox></td>
															<td class="edit_item_bg" align="center">
																<asp:TextBox ID="tbCountMin" Width="30" MaxLength="3" runat="server"></asp:TextBox>以上<br />
																<asp:TextBox ID="tbCountMax" Width="30" MaxLength="3" runat="server"></asp:TextBox>以下
															</td>
															<td class="edit_item_bg" align="center">
																<asp:RadioButtonList ID="rblFamilyFlg" RepeatLayout="Flow" RepeatDirection="Vertical" runat="server">
																	<asp:ListItem Text="親" Value="1"></asp:ListItem>
																	<asp:ListItem Text="子" Value="2"></asp:ListItem>
																</asp:RadioButtonList>
															</td>
															<td class="edit_item_bg" align="center"><asp:TextBox ID="tbDisplayOrder" Width="25px" MaxLength="3" Text="<%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).DisplayOrder)%>" runat="server"></asp:TextBox> </td>
														</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="9"></td>
													</tr>
												</table>
												
												<div class="action_part_bottom"><input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " onclick="btnConfirm_Click"></asp:Button></div>
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<asp:HiddenField ID="hfAddProductId" runat="server" />
<asp:HiddenField ID="hfAddVId" runat="server" />
<asp:HiddenField ID="hfAddProductName" runat="server" />
<asp:HiddenField ID="hfAddProductPrice" runat="server" />
<asp:LinkButton id="lbAddProduct" runat="server" OnClick="lbAddProduct_Click"></asp:LinkButton>
<script type="text/javascript">
<!--

// 商品一覧画面表示
function open_product_list(link_file, window_name, window_type) {
	var shipping_type_product_ids = '';
	<% foreach(RepeaterItem ri in rProductList.Items){ %>
		<%-- 配送料種別取得用商品ID連結 --%>
		var product_id = $('#<%= ((HiddenField)ri.FindControl("hfProductId")).ClientID %>').val();
		if (product_id != '') {
			if (shipping_type_product_ids != '') shipping_type_product_ids += ','
			shipping_type_product_ids += product_id;
		}
	<% } %>
	link_file += '&<%= Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS %>=' + encodeURIComponent(shipping_type_product_ids);	

	// ウィンドウ表示
	open_window(link_file, window_name, window_type);
}

// 商品追加処理
function set_productinfo(product_id, supplier_id, v_id, name, display_price, special_price, price, sale_id, fixed_purchase_id) {
	document.getElementById('<%= hfAddProductId.ClientID %>').value = product_id;
	document.getElementById('<%= hfAddVId.ClientID %>').value = v_id;
	document.getElementById('<%= hfAddProductName.ClientID %>').value = name;
	document.getElementById('<%= hfAddProductPrice.ClientID %>').value = display_price;
	
	__doPostBack('<%= lbAddProduct.UniqueID %>','');
}
//-->
</script>
</asp:Content>