<%--
=========================================================================================================
  Module      : 定期商品変更設定登録/編集画面(FixedPurchaseProductChangeSettingRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Form/Common/DefaultPage.master" CodeFile="FixedPurchaseProductChangeSettingRegister.aspx.cs" Inherits="Form_FixedPurchaseProductChangeSetting_FixedPurchaseProductChangeSettingRegister" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">定期商品変更設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr id="trRegistTitle" runat="server">
		<td><h2 class="cmn-hed-h2">定期商品変更設定登録</h2></td>
	</tr>
	<tr id="trEditTitle" runat="server">
		<td><h2 class="cmn-hed-h2">定期商品変更設定編集</h2></td>
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
											<td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<div class="action_part_top">
													<input type="button" value="  一覧へ戻る  " onclick="javascript: backListPage()" />
													<asp:Button id="btnToConfirmTop" Enabled="<%# this.IsEnabledConfirmButton %>" Text="  確認する  " OnClick="btnToConfirm_Click" runat="server" />
												</div>
												<!-- ▽ヘッダ情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ エラー表示 ▽--%>
													<tr id="trErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
													</tr>
													<tr id="trErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:Label ID="lErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ エラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">定期商品変更ID<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<% if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) { %>
																<%# WebSanitizer.HtmlEncode(this.Input.FixedPurchaseProductChangeId) %>
															<% } else { %>
																<asp:TextBox ID="tbFixedPurchaseProductChangeId" MaxLength="50" Text="<%# this.Input.FixedPurchaseProductChangeId %>" runat="server"></asp:TextBox>
															<% } %>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">定期商品変更設定名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbFixedPurchaseProductChangeName" MaxLength="100" Text="<%# this.Input.FixedPurchaseProductChangeName %>" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">適用優先順<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbPriority" Text="<%# this.Input.Priority %>" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">有効フラグ<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbValidFlg" Checked="<%# this.Input.ValidFlg == Constants.FLG_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID %>" runat="server"/>有効
														</td>
													</tr>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center">変更元商品設定</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left">
															<table class="edit_table" cellspacing="1" cellpadding="3" width="743" border="0">
																<tr>
																	<td rowspan="2" class="edit_title_bg" align="center" width="75">
																		選択/削除
																	</td>
																	<td rowspan="1" class="edit_title_bg" align="center" width="175">
																		商品ID
																	</td>
																	<td rowspan="2" class="edit_title_bg" width="290">
																		商品名
																	</td>
																	<td rowspan="2" class="edit_title_bg" align="center" width="110">
																		指定単位
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center" width="175">
																		バリエーションID
																	</td>
																</tr>
																<asp:Repeater ID="rBeforeChangeItems" ItemType="FixedPurchaseBeforeChangeItemInput" runat="server">
																	<ItemTemplate>
																		<tr>
																			<td rowspan="2" class="edit_item_bg" align="center" width="75">
																				<input type="button" onclick="javascript:openPoppupSearch('<%# GetProductListUrl(Item.IsVariationItemUnitType, isSelectProduct: true) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%= CONST_PRODUCT_SET_EVENT_TYPE_BEFORE_ITEM %>', '<%= Constants.FLG_FIXEDPURCHASEBEFOREPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_VARIATION %>', '<%# Container.ItemIndex %>');" value="  選択  " />
																				<asp:Button runat="server" ID="btnDeleteBeforeChangeItem" OnClick="btnDeleteBeforeChangeItem_Click" CommandArgument="<%# Container.ItemIndex %>" Text="  削除  " />
																			</td>
																			<td rowspan="1" class="edit_item_bg" align="center" width="175">
																				<%# WebSanitizer.HtmlEncode(Item.ProductId) %>
																			</td>
																			<td rowspan="2" class="edit_item_bg" width="290">
																				<%# WebSanitizer.HtmlEncode(Item.ProductName) %>
																			</td>
																			<td rowspan="2" class="edit_item_bg" align="center" width="110">
																				<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASEBEFORECHANGEITEM, Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_ITEM_UNIT_TYPE, Item.ItemUnitType)) %>
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_item_bg" align="center" width="175">
																				<%# WebSanitizer.HtmlEncode(Item.IsVariationItemUnitType ? Item.VariationId : "-") %>
																			</td>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
															</table>
															<br />
															<div style="text-align: right;">
																<input type="button" onclick="javascript:openPoppupSearch('<%= GetProductListUrl(isAddTypeVariation: true) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%= CONST_PRODUCT_SET_EVENT_TYPE_BEFORE_ITEM %>', '<%= Constants.FLG_FIXEDPURCHASEBEFOREPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_VARIATION %>', '');" value="  バリエーション単位で追加  " />
																<input type="button" onclick="javascript:openPoppupSearch('<%= GetProductListUrl(isAddTypeVariation: false) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%= CONST_PRODUCT_SET_EVENT_TYPE_BEFORE_ITEM %>', '<%= Constants.FLG_FIXEDPURCHASEBEFOREPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_PRODUCT %>', '');" value="  商品単位で追加  " />
															</div>
															<%-- 隠しボタン：変更元商品セット --%>
															<asp:LinkButton id="lbSetBeforeChangeItem" OnClick="lbSetBeforeChangeItem_Click" runat="server" />
														</td>
													</tr>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="4">変更後商品設定</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left">
															<table>
																<tr>
																	<td rowspan="2" class="edit_title_bg" align="center" width="75">
																		選択/削除
																	</td>
																	<td rowspan="1" class="edit_title_bg" align="center" width="175">
																		商品ID
																	</td>
																	<td rowspan="2" class="edit_title_bg" width="290">
																		商品名
																	</td>
																	<td rowspan="2" class="edit_title_bg" align="center" width="110">
																		指定単位
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center" width="175">
																		バリエーションID
																	</td>
																</tr>
																<asp:Repeater ID="rAfterChangeItems" ItemType="FixedPurchaseAfterChangeItemInput" runat="server">
																	<ItemTemplate>
																		<tr>
																			<td rowspan="2" class="edit_item_bg" align="center" width="75">
																				<input type="button" onclick="javascript:openPoppupSearch('<%# GetProductListUrl(Item.IsVariationItemUnitType, isSelectProduct: true) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%= CONST_PRODUCT_SET_EVENT_TYPE_AFTER_ITEM %>', '<%= Constants.FLG_FIXEDPURCHASEBEFOREPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_VARIATION %>', '<%# Container.ItemIndex %>');" value="  選択  " />
																				<asp:Button runat="server" ID="btnDeleteBeforeChangeItem" OnClick="btnDeleteAfterChangeItem_Click" CommandArgument="<%# Container.ItemIndex %>" Text="削除" />
																			</td>
																			<td rowspan="1" class="edit_item_bg" align="center" width="175">
																				<%# WebSanitizer.HtmlEncode(Item.ProductId) %>
																			</td>
																			<td rowspan="2" class="edit_item_bg" width="290">
																				<%# WebSanitizer.HtmlEncode(Item.ProductName) %>
																			</td>
																			<td rowspan="2" class="edit_item_bg" align="center" width="110">
																				<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASEAFTERCHANGEITEM, Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_ITEM_UNIT_TYPE, Item.ItemUnitType)) %>
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_item_bg" align="center" width="175">
																				<%# WebSanitizer.HtmlEncode(Item.IsVariationItemUnitType ? Item.VariationId : "-") %>
																			</td>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
															</table>
															<br/>
															<div style="text-align: right;">
																<input type="button" onclick="javascript:openPoppupSearch('<%= GetProductListUrl(isAddTypeVariation: true) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%= CONST_PRODUCT_SET_EVENT_TYPE_AFTER_ITEM %>', '<%= Constants.FLG_FIXEDPURCHASEBEFOREPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_VARIATION %>', '');" value="  バリエーション単位で追加  " />
																<input type="button" onclick="javascript:openPoppupSearch('<%= GetProductListUrl(isAddTypeVariation: false) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%= CONST_PRODUCT_SET_EVENT_TYPE_AFTER_ITEM %>', '<%= Constants.FLG_FIXEDPURCHASEBEFOREPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_PRODUCT %>', '');" value="  商品単位で追加  " />
															</div>
															<%-- 隠しボタン：変更後商品セット --%>
															<asp:LinkButton id="lbSetAfterChangeItem" OnClick="lbSetAfterChangeItem_Click" CommandArgument="<%# CONST_PRODUCT_SET_EVENT_TYPE_AFTER_ITEM %>" runat="server" />
														</td>
													</tr>
												</table>
												<br />
												<div class="action_part_bottom">
													<input type="button" value="  一覧へ戻る  " onclick="javascript: backListPage()" />
													<asp:Button id="btnToConfirmBottom" Enabled="<%# this.IsEnabledConfirmButton %>" Text="  確認する  " OnClick="btnToConfirm_Click" runat="server" />
												</div>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr class="info_item_bg">
															<td align="left">備考<br/>
																・マイページで定期購入商品を変更する際に変更可能な商品を指定できます。<br/>
																・変更元商品設定は、<strong>または（OR）条件</strong>になります。<br/>
																・変更元商品設定は、<strong>同一配送種別</strong>の商品/バリエーションのみ設定可能です。<br/>
																・変更後商品設定は、変更元商品設定にて設定されている<strong>同一配送種別</strong>の商品/バリエーションのみ設定可能です。<br />
																・変更元商品設定/変更後商品設定は、<strong>各10商品</strong>/バリエーションまで設定可能です。<br />
															</td>
														</tr>
													</tbody>
												</table>
											</td>
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
</table>
<asp:HiddenField ID="hfCurentPopup" runat="server" />
<asp:HiddenField ID="hfProductId" runat="server"/>
<asp:HiddenField ID="hfVariationId" runat="server"/>
<asp:HiddenField ID="hfUnitType" runat="server"/>
<asp:HiddenField ID="hfChangeItemIndex" runat="server"/>

<script>
	var hfCurentPopup = "<%=hfCurentPopup.ClientID %>";

	// 検索フォーム表示（適用条件アイテム用）
	function openPoppupSearch(linkFile, windowName, windowType, popupType, unitType, changeItemIndex) {
		$("#" + hfCurentPopup).val(popupType);
		$("#<%= hfUnitType.ClientID %>").val(unitType);
		$("#<%= hfChangeItemIndex.ClientID %>").val(changeItemIndex);
		open_window(linkFile, windowName, windowType);
	}

	// 商品一覧で選択された商品ID，商品バリエーションIDをセット
	function set_productinfo(product_id, supplier_id, v_id, product_name, display_price, display_special_price, product_price, sale_id, fixed_purchase_id) {
		var curentPopup = $("#" + hfCurentPopup).val();
		$("#<%= hfProductId.ClientID %>").val(product_id);
		$("#<%= hfVariationId.ClientID %>").val(product_id + v_id);
		switch (curentPopup) {
			case "<%= CONST_PRODUCT_SET_EVENT_TYPE_BEFORE_ITEM %>":
				__doPostBack("<%= lbSetBeforeChangeItem.UniqueID %>", "");
				break;

			case "<%= CONST_PRODUCT_SET_EVENT_TYPE_AFTER_ITEM %>":
				__doPostBack("<%= lbSetAfterChangeItem.UniqueID %>", "");
				break;
		}
	}

	// 一覧へ戻る
	function backListPage() {
		var url = "<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_LIST %>";
		window.location.href = url;
	}
</script>
</asp:Content>
