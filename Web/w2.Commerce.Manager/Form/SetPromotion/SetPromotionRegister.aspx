<%--
=========================================================================================================
  Module      : セットプロモーション設定登録ページ(SetPromotionRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Diagnostics.Eventing.Reader" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SetPromotionRegister.aspx.cs" Inherits="Form_SetPromotion_SetPromotionRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">セットプロモーション設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr id="trRegistTitle" runat="server">
		<td><h2 class="cmn-hed-h2">セットプロモーション設定登録</h2></td>
	</tr>
	<tr id="trEditTitle" runat="server">
		<td><h2 class="cmn-hed-h2">セットプロモーション設定編集</h2></td>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<div id="divComp" runat="server" class="action_part_top" Visible="False">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">セットプロモーション設定を登録/更新しました。
															</td>
														</tr>
													</table>
												</div>
												<div class="action_part_top">
													<asp:Button ID="btnToListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" />
												</div>
												<!-- ▽ヘッダ情報▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trInputSetPromotionId" runat="server">
														<td class="detail_title_bg" align="left" width="25%">セットプロモーションID<span class="notice">*</span></td>
														<td class="detail_item_bg" align="left">
															<asp:TextBox ID="tbSetPromotionId" MaxLength="10" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr id="trDispSetPromotionId" runat="server">
														<td class="detail_title_bg" align="left" width="25%">セットプロモーションID</td>
														<td class="detail_item_bg" align="left">
															<asp:Literal ID="lSetPromotionId" runat="server"></asp:Literal>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">セットプロモーション名(管理用)<span class="notice">*</span></td>
														<td class="detail_item_bg" align="left">
															<asp:TextBox ID="tbSetPromotionName" MaxLength="30" runat="server" Width="300"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="25%">セットプロモーション名(サイト表示用)<span class="notice">*</span></td>
														<td class="detail_item_bg" align="left">
															<asp:TextBox ID="tbSetPromotionDispName" MaxLength="30" runat="server" Width="300"></asp:TextBox>
														</td>
													</tr>
													<%-- セットプロモーション名(サイト表示用)翻訳設定情報 --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater ID="rTranslationSetPromotionDispName" runat="server"
														DataSource="<%# this.SetPromotionTranslationData.Where(s => s.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_SETPROMOTION_DISP_NAME) %>"
														ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
													<ItemTemplate>
													<tr>
														<td class="detail_title_bg" align="left" width="25%">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="detail_item_bg" align="left"><%#: Item.AfterTranslationalName %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<tr>
														<td class="detail_title_bg">プロモーション種別<span class="notice">*</span></td>
														<td class="detail_item_bg">
															<asp:CheckBox ID="cbProductDiscountFlg" runat="server" Text=" 商品金額割引" AutoPostBack="true" OnCheckedChanged="cbProductDiscountFlg_OnCheckedChanged" /><br />
															<div id="divProductDiscountSetting" runat="server">
															&nbsp;&nbsp;<asp:RadioButton ID="rbProductDiscountKbnDiscountedPrice" runat="server" GroupName="product_discount_kbn" />
															<asp:TextBox ID="tbDiscountedPrice" runat="server" Width="100" MaxLength="7"></asp:TextBox>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : CurrencyManager.KeyCurrencyUnit %><br />
															&nbsp;&nbsp;<asp:RadioButton ID="rbProductDiscountKbnDiscountPrice" runat="server" GroupName="product_discount_kbn" />
															<asp:TextBox ID="tbDiscountPrice" runat="server" Width="100" MaxLength="7"></asp:TextBox>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : CurrencyManager.KeyCurrencyUnit %>引き<br />
															&nbsp;&nbsp;<asp:RadioButton ID="rbProductDiscountKbnDiscountRate" runat="server" GroupName="product_discount_kbn" />
															<asp:TextBox ID="tbDiscountRate" runat="server" Width="100" MaxLength="7"></asp:TextBox>&nbsp;%割引き<br />
															</div>
															<asp:CheckBox ID="cbShippingChargeFreeFlg" runat="server" Text=" 配送料無料" /><br />
															<asp:CheckBox ID="cbPaymentChargeFreeFlg" runat="server" Text=" 決済手数料無料" /><br />
														</td>
													</tr>
													<% if (this.IsApplyOrderSettingEnable) { %>
													<tr>
														<td class="edit_title_bg" align="left">適用優先順 <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbApplyOrder" runat="server" Width="50" MaxLength="7" />
															<br />
															商品が複数のセットプロモーションに該当する場合、適用優先順にセットプロモーションが適用されます。<br />
															※適用優先順が同じ場合はセットプロモーションIDの昇順に優先されます。
														</td>
													</tr>
													<% } %>
                                                    <tr>
														<td class="edit_title_bg" align="left">表示文言</td>
														<td class="edit_item_bg" align="left">
															<asp:RadioButtonList ID="rblDescriptionKbn" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server">
																<asp:ListItem Text="TEXT" Value="0" Selected="True"></asp:ListItem>
																<asp:ListItem Text="HTML" Value="1"></asp:ListItem>
															</asp:RadioButtonList>
															<input type= "button" onclick="javascript:open_wysiwyg('<%= tbDescription.ClientID %>', '<%= rblDescriptionKbn.ClientID %>');" value="  HTMLエディタ  " /><br />
															<asp:TextBox ID="tbDescription" runat="server" TextMode="MultiLine" Width="500" Height="50"></asp:TextBox></td>
													</tr>
													<%-- 表示文言翻訳設定情報 --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater ID="rTranslationDescription" runat="server"
														DataSource="<%# this.SetPromotionTranslationData.Where(s => s.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_DESCRIPTION) %>"
														ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
													<ItemTemplate>
													<tr>
														<td class="edit_title_bg" align="left" rowspan="2">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="detail_item_bg" align="left">
															<%#: ValueText.GetValueText(Constants.TABLE_SETPROMOTION, Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN, Item.DisplayKbn) %></td>
													</tr>
													<tr>
														<td class="detail_item_bg" align="left"><%#: Item.AfterTranslationalName %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<tr>
														<td class="detail_title_bg">開始日時-終了日時 <span class="notice">*</span></td>
														<td class="detail_item_bg">
															<uc:DateTimePickerPeriodInput id="ucDisplayPeriod" runat="server" IsNullEndDateTime="true" />
														</td>
													</tr>
													<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
													<tr>
														<td class="detail_title_bg">適用会員ランク</td>
														<td class="detail_item_bg">
															<asp:DropDownList ID="ddlTargetMemberRank" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="detail_title_bg">適用注文区分<span class="notice">*</span></td>
														<td class="detail_item_bg">
															<asp:CheckBoxList ID="cblTargetOrderKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">紹介URL</td>
														<td class="detail_item_bg">
															<asp:TextBox ID="tbUrl" runat="server" Width="500"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">表示優先順<span class="notice">*</span></td>
														<td class="detail_item_bg">
															<asp:TextBox ID="tbDisplayOrder" MaxLength="5" runat="server" Width="50"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">ターゲットリストID<span class="notice"></span></td>
														<td class="detail_item_bg">
															<asp:TextBox id="tbTargetList" runat="server" Width="480" TextMode="MultiLine" Columns="80" Rows="3"></asp:TextBox>
															<input id="inputSearchTargetList" type="button" value="  検索  " onclick="javascript:open_product_list(
																																				'<%= WebSanitizer.HtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Mp, (Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST_POPUP))) %>	',
																																				'SetTargetList',
																																				'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes',
																																				'0');" />
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">有効フラグ</td>
														<td class="detail_item_bg" align="left">
															<asp:CheckBox id="cbValidFlg" runat="server" Text="有効" />
														</td>
													</tr>
												</table>
												<!-- △ヘッダ情報△ -->
												<!-- ▽アイテム情報▽ -->
												<div class="action_part_middle">
													<asp:Label ID="lTest" runat="server"></asp:Label>
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" width="5%">No</td>
														<td class="detail_title_bg" colspan="2" align="center" width="75%">対象商品<span class="notice">*</span></td>
														<td class="detail_title_bg" align="center" width="10%">数量</td>
														<td class="detail_title_bg" align="center" width="10%"><asp:Button ID="btnAddItem" runat="server" Text="  追加  " onclick="btnAddItem_Click"/></td>
													</tr>
													<asp:Repeater ID="rItemList" OnItemCommand="rItemList_ItemCommand" runat="server">
														<ItemTemplate>
															<tr>
																<td class="edit_item_bg" align="center">
																	<%# Container.ItemIndex + 1 %>
																</td>
																<td class="edit_item_bg" width="25%">
																	<asp:RadioButtonList ID="rblItemKbn" DataSource="<%#this.SetPromotionItemKbn %>" DataTextField="Text" DataValueField="Value" AutoPostBack="true" runat="server" OnSelectedIndexChanged="rblItemKbn_SelectedIndexChanged" RepeatLayout="Flow"></asp:RadioButtonList>
																</td>
																<td class="edit_item_bg" width="60%">
																	<div id="divInputItemArea" runat="server">
																		<asp:TextBox ID="tbItems" Text='<%# Eval("SetpromotionItems") %>' runat="server" Width="95%" TextMode="MultiLine" Rows="<%# TEXT_BOX_ROWS_NORMAL %>"></asp:TextBox><br/>
																		<div style="margin-top:5px; margin-right:10px; float:right;">
																			<asp:LinkButton ID="lbResizeLarge" CommandArgument="<%# Container.ItemIndex %>" OnClick="lbResize_Click" runat="server">拡大する</asp:LinkButton>
																			<asp:LinkButton ID="lbResizeNormal" CommandArgument="<%# Container.ItemIndex %>" OnClick="lbResize_Click" Visible="false" runat="server">縮小する</asp:LinkButton>
																		</div>
																			<span id="spSearchProduct" runat="server">
																				<input type="button" value="  検索  " onclick="javascript:open_product_list('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_PRODUCT + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>	','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%# Container.ItemIndex %>	');" />
																			</span>
																			<span id="spSearchProductVariation" runat="server">
																				<input type="button" value="  検索  " onclick="javascript:open_product_list('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>	','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%# Container.ItemIndex %>	');" />
																			</span>
																			<span id="spSearchCategory" runat="server">
																				<input type="button" value="  検索  " onclick="javascript:open_category_list('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_CATEGORY_SEARCH) %>	','product_search','width=850,height=670,top=120,left=420,status=NO,scrollbars=yes','<%# Container.ItemIndex %>	');" />
																			</span>
																	</div>
																</td>
																<td class="edit_item_bg" align="center">
																	<asp:TextBox ID="tbItemCount" Text='<%# Eval("SetpromotionItemQuantity") %>' MaxLength="3" runat="server" Width="30"></asp:TextBox><br />
																	<asp:CheckBox ID="cbItemQuantityMoreFlg" runat="server" Checked='<%# (StringUtility.ToEmpty(Eval("SetpromotionItemQuantityMoreFlg")) == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG_VALID) %>' Text="以上" />
																</td>
																<td class="edit_item_bg" align="center">
																	<asp:Button ID="btnDeleteItem" CommandName="delete" CommandArgument="<%# Container.ItemIndex %>" Text="  削除  " runat="server" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												<!-- △アイテム情報△ -->
												<div class="action_part_bottom">
													<asp:Button ID="btnToListBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" />
												</div>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															■フロントでのセットプロモーション動作について<br />
															・商品一覧・商品詳細などでは、商品に適用可能なセットプロモーションの一覧を表示します。（表示優先順の数が小さい順に表示）<br />
															・表示されている適用可能なセットプロモーションのうち1つだけが適用されます。<br />
　															 購入時、商品の組み合わせから最も合計料金が安くなるセットプロモーションが自動的に選ばれます。<br />
															■対象商品の入力方法<br />
															<% if (Constants.PRODUCT_STOCK_OPTION_ENABLE){ %>
															・[商品ID指定]、[カテゴリID指定]：商品ID、カテゴリIDごとに改行で区切ってください。<br />
															<% }else{ %>
															・[商品ID指定]：商品IDごとに改行で区切ってください。<br />
															<% } %>
															・[バリエーションID指定]：「商品ID,バリエーションID」のようにカンマ区切りで入力し、バリエーションごとに改行で区切ってください。<br />
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<asp:HiddenField ID="hfAddProductId" runat="server" />
<asp:HiddenField ID="hfAddVariationId" runat="server" />
<asp:HiddenField ID="hfAddCategoryId" runat="server" />
<asp:HiddenField ID="hfAddIndex" runat="server" />
<asp:LinkButton id="lbSetItem" runat="server" OnClick="lbSetItem_Click"></asp:LinkButton>
<script type="text/javascript">
<!--
	// 選択商品
	var selected_index = 0;

	// 商品一覧画面表示
	function open_product_list(link_file, window_name, window_type, index) {
		var product_count = 0;
		var product_max_count = 10;
		var shipping_type_product_ids = '';
		var value = '';
		<%
		var itemKbn = "";
		var clientId = "";
		foreach (RepeaterItem ri in rItemList.Items)
		{
			itemKbn = ((RadioButtonList)ri.FindControl("rblItemKbn")).SelectedValue;
			if ((itemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT)
				 || (itemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION))
			{
				clientId = ((TextBox)ri.FindControl("tbItems")).ClientID;
		%>
		<%-- 配送料種別取得用商品ID連結（先頭10件のみ取得　※URLパラメータが長くなるため） --%>
		if (product_count < product_max_count) {
			value = $('#<%= clientId %>').val();
			if (value != '') {
				var product_ids = value.split(/\r\n|\r|\n/);
				for (var i = 0; i < product_ids.length; i++) {
					var product_id = product_ids[0];
					<%
						if (itemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION)
						{
					%>
					product_id = product_id.split(',')[0];
					<%
						}
					%>
					if (product_id != '') {
						if (shipping_type_product_ids != '') shipping_type_product_ids += ','
						shipping_type_product_ids += product_id;
					}
					product_count++;
					if (product_count == product_max_count) break;
				}
			}
		}
		<%
			}
		}
		%>
		link_file += '&<%= Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS %>=' + encodeURIComponent(shipping_type_product_ids);

		// 選択商品を格納
		selected_index = index;
		// ウィンドウ表示
		open_window(link_file, window_name, window_type);
	}

	// カテゴリ一覧画面表示
	function open_category_list(link_file, window_name, window_type, index) {
		// 選択商品を格納
		selected_index = index;
		// ウィンドウ表示
		open_window(link_file, window_name, window_type);
	}

	// 商品一覧で選択された商品情報を設定
	function set_productinfo(product_id, supplier_id, v_id, name, display_price, special_price, price, sale_id) {
		document.getElementById('<%= hfAddProductId.ClientID %>').value = product_id;
		document.getElementById('<%= hfAddVariationId.ClientID %>').value = product_id + v_id;
		document.getElementById('<%= hfAddIndex.ClientID %>').value = selected_index;
		__doPostBack('<%= lbSetItem.UniqueID %>', '');
	}

	// カテゴリ一覧で選択されたカテゴリを設定
	function set_categoryinfo(category_id, name) {
		document.getElementById('<%= hfAddCategoryId.ClientID %>').value = category_id;
		document.getElementById('<%= hfAddIndex.ClientID %>').value = selected_index;
		__doPostBack('<%= lbSetItem.UniqueID %>', '');
	}

	// ターゲットリスト一覧で選択されたターゲットリストIDをセット
	function SetTargetList(text, data_count, value) {
		var targetList = $('#<%= tbTargetList.ClientID %>').val();
		if (targetList.trim().length !== 0) targetList += '\n';
		$('#<%= tbTargetList.ClientID %>').val(targetList + value);
	}

//-->
</script>
</asp:Content>