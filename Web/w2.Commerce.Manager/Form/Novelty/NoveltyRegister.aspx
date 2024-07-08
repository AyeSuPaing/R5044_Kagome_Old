<%--
=========================================================================================================
  Module      : ノベルティ設定登録ページ(NoveltyRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="NoveltyRegister.aspx.cs" Inherits="Form_Novelty_NoveltyRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr><td><h1 class="page-title">ノベルティ設定</h1></td></tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 登録/更新 ▽-->
	<tr id="trRegsit" runat="server" Visible="False">
		<td>
			<h1 class="cmn-hed-h2">ノベルティ設定登録</h1>
		</td>
	</tr>
	<tr id="trEdit" runat="server" Visible="False">
		<td>
			<h1 class="cmn-hed-h2">ノベルティ設定編集</h1>
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
												<div id="divComp" runat="server" class="action_part_top" visible="false">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">ノベルティ設定を登録/更新しました。</td>
														</tr>
													</table>
												</div>
												<div class="action_part_top">
													<asp:Button ID="btnToListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " OnClick="btnInsertUpdate_Click" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ 基本情報エラー表示 ▽--%>
													<tr id="trNoveltyErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
													</tr>
													<tr id="trNoveltyErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:Label ID="lbNoveltyErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ 基本情報エラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr id="trDisplayNoveltyId" runat="server">
														<td class="edit_title_bg" align="left" width="145">ノベルティID</td>
														<td class="edit_item_bg" align="left">
															<asp:Literal id="lNoveltyId" runat="server" />
														</td>
													</tr>
													<tr  id="trInputNoveltyId" runat="server">
														<td class="edit_title_bg" align="left" width="145">ノベルティID <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbNoveltyId" runat="server" Width="300" MaxLength="30" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">ノベルティ名（表示） <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbNoveltyDisplayName" runat="server" Width="300" MaxLength="100" />
														</td>
													</tr>
													<%-- ノベルティ名(表示)翻訳設定情報 --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater ID="rTranslationNoveltyDispName" runat="server"
														 ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel"
														 DataSource="<%# this.NoveltyTranslationData %>">
													<ItemTemplate>
													<tr>
														<td class="edit_title_bg" align="left">　言語コード:<%#: Item.LanguageCode %><br />　言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="edit_item_bg" align="left"><%#: Item.AfterTranslationalName %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">ノベルティ名（管理用） <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbNoveltyName" runat="server" Width="300" MaxLength="100" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">説明（管理用）</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbDiscription" runat="server" TextMode="MultiLine" Width="470" Height="80" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">開始日時 <span class="notice">*</span> - 終了日時</td>
														<td class="edit_item_bg" align="left">
															<uc:DateTimePickerPeriodInput id="ucDateTimePickerPeriod" IsNullEndDateTime="true" runat="server" />
															<span class="search_btn_sub">(<a href="Javascript:SetToday();">今日</a>)</span>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">自動付与フラグ</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox id="cbAutoAdditionalFlg" runat="server" Text="有効" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">有効フラグ</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox id="cbValidFlg" runat="server" Text="有効" />
														</td>
													</tr>
													<tr id="trDateCreated" runat="server" visible="false">
														<td class="edit_title_bg" align="left">作成日</td>
														<td class="edit_item_bg" align="left">
															<asp:Literal ID="lDateCreated" runat="server" />
														</td>
													</tr>
													<tr id="trDateChanged" runat="server" visible="false">
														<td class="edit_title_bg" align="left">更新日</td>
														<td class="edit_item_bg" align="left">
															<asp:Label ID="lDateChanged" runat="server" />
														</td>
													</tr>
													<tr id="trLastChanged" runat="server" visible="false">
														<td class="edit_title_bg" align="left">最終更新者</td>
														<td class="edit_item_bg" align="left">
															<asp:Label ID="lLastChanged" runat="server" />
														</td>
													</tr>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center">適用対象設定</td>
													</tr>
													<tr>
														<td class="edit_item_bg">
															<div style="margin-bottom:5px"></div>
															<asp:Repeater ID="rTargetItemList" OnItemCommand="rTargetItemList_ItemCommand" runat="server">
																<ItemTemplate>
																	<table class="edit_table" cellspacing="1" cellpadding="0" border="0" width="740">
																		<%--▽ 適用対象設定エラー表示 ▽--%>
																		<tr runat="server" visible="<%# ((NoveltyTargetItemInput)Container.DataItem).ErrorMessage.Length != 0 %>">
																			<td class="edit_title_bg" align="center" colspan="3">エラーメッセージ</td>
																		</tr>
																		<tr runat="server" visible="<%# ((NoveltyTargetItemInput)Container.DataItem).ErrorMessage.Length != 0 %>">
																			<td class="edit_item_bg" align="left" colspan="3">
																				<asp:Label ID="lbErrorMessage" runat="server" ForeColor="red" Text='<%# ((NoveltyTargetItemInput)Container.DataItem).ErrorMessage %>' />
																			</td>
																		</tr>
																		<%--△ 適用対象設定エラー常時 △--%>
																		<tr>
																			<td class="edit_title_bg" align="left" colspan="3">
																				<asp:RadioButtonList ID="rblTarget" AutoPostBack="true" runat="server" OnSelectedIndexChanged="rblTarget_SelectedIndexChanged" RepeatDirection="Horizontal" SelectedValue='<%# ((NoveltyTargetItemInput)Container.DataItem).IsItemTypeAll ? Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_ALL : "" %>' RepeatLayout="Flow">
																					<asp:ListItem Value="">条件指定</asp:ListItem>
																					<asp:ListItem Value="ALL">全商品</asp:ListItem>
																				</asp:RadioButtonList>
																			</td>
																		</tr>
																		<%--▽ ブランドOPが有効の場合 ▽--%>
																		<% if (Constants.PRODUCT_BRAND_ENABLED) { %>
																		<tr id="trBrandId" runat="server">
																			<td class="edit_title_bg" align="left" width="122">ブランドID</td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox ID="tbBrandId" runat="server" Width="98%" TextMode="MultiLine" Rows="<%# ((NoveltyTargetItemInput)Container.DataItem).BrandIdRows %>" Text="<%# ((NoveltyTargetItemInput)Container.DataItem).BrandId %>"/>
																				<div style="margin-top:5px; margin-right:10px; float:right;">
																					<asp:LinkButton ID="lbBrandResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbBrandResize" Text="拡大する" CommandArgument="<%# Container.ItemIndex %>"  Visible="<%# ((NoveltyTargetItemInput)Container.DataItem).IsBrandIdResizeNormal %>" />
																					<asp:LinkButton ID="lbBrandResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbBrandResize" Text="縮小する" CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyTargetItemInput)Container.DataItem).IsBrandIdResizeNormal == false %>" />
																				</div>
																				<span>
																					<input type="button" value="  検索  " onclick="javascript:open_poppup_search('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_BRAND_SEARCH) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','BRAND', this);" />
																				</span>
																			</td>
																			<td class="edit_item_bg" align="center" width="50" rowspan="4">
																				<asp:Button ID="btnDeleteTargetProductBrand" runat="server" CommandName="delete" Text="  削除  " CommandArgument="<%# Container.ItemIndex %>"  Visible="<%# ((NoveltyTargetItemInput[])((Repeater)Container.Parent).DataSource).Length > 1 %>" />
																			</td>
																		</tr>
																		<% } %>
																		<%--△ ブランドOPが有効の場合 △--%>
																		<%--▽ 商品カテゴリOPが有効の場合 ▽--%>
																		<% if (Constants.PRODUCT_CTEGORY_OPTION_ENABLE) { %>
																		<tr id="trCategoryId" runat="server">
																			<td class="edit_title_bg" align="left" width="122">カテゴリID</td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox ID="tbCategoryId" runat="server" Width="98%" TextMode="MultiLine" Rows="<%# ((NoveltyTargetItemInput)Container.DataItem).CategoryIdRows %>" Text="<%# ((NoveltyTargetItemInput)Container.DataItem).CategoryId %>" />
																				<div style="margin-top:5px; margin-right:10px; float:right;">
																					<asp:LinkButton ID="lbCategoryResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbCategoryResize" Text="拡大する" CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyTargetItemInput)Container.DataItem).IsCategoryIdResizeNormal %>" />
																					<asp:LinkButton ID="lbCategoryResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbCategoryResize" Text="縮小する" CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyTargetItemInput)Container.DataItem).IsCategoryIdResizeNormal == false %>" />
																				</div>
																				<span id="spSearchCategory">
																					<input type="button" value="  検索  " onclick="javascript:open_poppup_search('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_CATEGORY_SEARCH) %>','product_search','width=850,height=670,top=120,left=420,status=NO,scrollbars=yes','CATEGORY', this);" />
																				</span>
																			</td>
																			<td class="edit_item_bg" align="center" width="50" rowspan="3" visible="<%# (Constants.PRODUCT_BRAND_ENABLED == false) %>">
																				<asp:Button ID="btnDeleteTargetProduct" runat="server" CommandName="delete" Text="  削除  " CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyTargetItemInput[])((Repeater)Container.Parent).DataSource).Length > 1 %>" />
																			</td>
																		</tr>
																		<% } %>
																		<%--△ 商品カテゴリOPが有効の場合 △--%>
																		<tr id="trProductId" runat="server">
																			<td class="edit_title_bg" align="left">商品ID</td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox ID="tbProductId" runat="server" Width="98%" TextMode="MultiLine" Rows="<%# ((NoveltyTargetItemInput)Container.DataItem).ProductIdRows %>" Text="<%# ((NoveltyTargetItemInput)Container.DataItem).ProductId %>" />
																				<div style="margin-top:5px; margin-right:10px; float:right;">
																					<asp:LinkButton ID="lbProductResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbProductResize" Text="拡大する" CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyTargetItemInput)Container.DataItem).IsProductIdResizeNormal %>" />
																					<asp:LinkButton ID="lbProductResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbProductResize" Text="縮小する" CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyTargetItemInput)Container.DataItem).IsProductIdResizeNormal == false %>" />
																				</div>
																				<span id="spSearchProduct" runat="server">
																					<input type="button" value="  検索  " onclick="javascript:open_poppup_search('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_PRODUCT + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','PRODUCT', this);" />
																				</span>
																			</td>
																		</tr>
																		<tr id="trVariationId" runat="server">
																			<td class="edit_title_bg" align="left">バリエーションID</td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox ID="tbProductVariationId" runat="server" Width="98%" TextMode="MultiLine" Rows="<%# ((NoveltyTargetItemInput)Container.DataItem).VariationIdRows %>" Text="<%# ((NoveltyTargetItemInput)Container.DataItem).VariationId %>" />
																				<div style="margin-top:5px; margin-right:10px; float:right;">
																					<asp:LinkButton ID="lbProductVariationResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbProductVariationResize" Text="拡大する" CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyTargetItemInput)Container.DataItem).IsVariationIdResizeNormal %>" />
																					<asp:LinkButton ID="lbProductVariationResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbProductVariationResize" Text="縮小する" CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyTargetItemInput)Container.DataItem).IsVariationIdResizeNormal == false %>" />
																				</div>
																				<span id="spSearchProductVariation" runat="server">
																					<input type="button" value="  検索  " onclick="javascript:open_poppup_search('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','VARIATION', this);" />
																				</span>
																			</td>
																		</tr>
																	</table>
																	<div style="margin:10px 5px 10px 1px; float:left;">を満たす商品が１つカートに存在する
																		&nbsp;<strong runat="server" visible="<%# ((Container.ItemIndex + 1) != ((NoveltyTargetItemInput[])((Repeater)Container.Parent).DataSource).Length) %>">かつ（AND）</strong>
																		<span runat="server" visible="<%# ((Container.ItemIndex + 1) == ((NoveltyTargetItemInput[])((Repeater)Container.Parent).DataSource).Length) %>">場合に付与する。</span>
																	</div>
																</ItemTemplate>
															</asp:Repeater>
															<div style="float:right;margin:5px 15px 5px 0px"><asp:Button  ID="btnAddTargetItem" runat="server"  Text="  追加  " onclick="btnAddTargetItem_Click" /><br/></div>
														</td>
													</tr>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center">付与ノベルティ設定</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left">
															<div style="margin-bottom:5px"></div>
															<table class="edit_table" cellspacing="1" cellpadding="0" border="0" width="740">
																<tr>
																	<td class="edit_title_bg" align="center" width="90">対象金額（以上） <span class="notice">*</span></td>
																	<td class="edit_title_bg" align="center" width="90">対象金額（以下）</td>
																	<td class="edit_title_bg" align="center" rowspan="2">商品ID <span class="notice">*</span></td>
																	<td class="edit_title_bg" rowspan="2" width="51"></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center" colspan="2">適用対象会員</td>
																</tr>
															<asp:Repeater ID="rGrantConditionsList" OnItemCommand="rGrantConditionsList_ItemCommand" runat="server">
															<ItemTemplate>
																	<%--▽ 付与ノベルティ設定エラー表示 ▽--%>
																	<tr runat="server" visible="<%# ((NoveltyGrantConditionsInput)Container.DataItem).ErrorMessage.Length != 0 %>">
																		<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
																	</tr>
																	<tr runat="server" visible="<%# ((NoveltyGrantConditionsInput)Container.DataItem).ErrorMessage.Length != 0 %>">
																		<td class="edit_item_bg" align="left" colspan="4">
																			<asp:Label ID="lbErrorMessage" runat="server" ForeColor="red" Text="<%# ((NoveltyGrantConditionsInput)Container.DataItem).ErrorMessage %>" />
																		</td>
																	</tr>
																	<%--△ 付与ノベルティ設定エラー表示 △--%>
																	<tr>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox id="tbAmountBegin" runat="server" Width="70" MaxLength="18" Text="<%# ((NoveltyGrantConditionsInput)Container.DataItem).AmountBegin.ToPriceString() %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox id="tbAmountEnd" runat="server" Width="70" MaxLength="18" Text="<%# ((NoveltyGrantConditionsInput)Container.DataItem).AmountEnd.ToPriceString() %>" />
																		</td>
																		<td class="edit_item_bg" align="left" rowspan="2">
																			<asp:TextBox ID="tbNoveltyItems" runat="server" Width="98%" TextMode="MultiLine" Rows="<%# ((NoveltyGrantConditionsInput)Container.DataItem).ProductIdRows %>" Text="<%# ((NoveltyGrantConditionsInput)Container.DataItem).ProductId %>" />
																			<div style="margin-top:5px; margin-right:10px; float:right;">
																				<asp:LinkButton ID="lbResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbResize" Text="拡大する" CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyGrantConditionsInput)Container.DataItem).IsProductIdResizeNormal %>" />
																				<asp:LinkButton ID="lbResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbResize" Text="縮小する" CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyGrantConditionsInput)Container.DataItem).IsProductIdResizeNormal == false %>" />
																			</div>
																			<span>
																				<input type="button" value="  検索  " onclick="javascript:open_poppup_search('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_PRODUCT + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','NOVELTYITEMS', this);" />
																			</span>
																		</td>
																		<td class="edit_item_bg" align="center" rowspan="2">
																			<asp:Button ID="btnDeleteItem" runat="server" CommandName="delete" Text="  削除  " CommandArgument="<%# Container.ItemIndex %>" Visible="<%# ((NoveltyGrantConditionsInput[])((Repeater)Container.Parent).DataSource).Length > 1%>"/>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="left" colspan="2">
																			<asp:DropDownList ID="ddlUserRank" runat="server" Width="200" />
																		</td>
																	</tr>
															</ItemTemplate>
															</asp:Repeater>
															</table>
															<div style="float:right;margin:5px 15px 5px 0px">
																<asp:Button  ID="btnAddItem" 	runat="server"  Text="  追加  " onclick="btnAddItem_Click"/>
															</div>
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<asp:Button ID="btnToListBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " OnClick="btnInsertUpdate_Click" />
												</div>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody><tr class="info_item_bg">
														<td align="left">備考<br/>
															・適用対象設定は<strong>かつ（AND）条件</strong>になります。<br/>
															・適用対象設定 > 条件指定 > 各IDは<strong>改行区切り</strong>で複数登録（<strong>または（OR）条件</strong>）できます。<br/>
															・適用対象設定 > 条件指定 > バリエーションIDは、商品IDとバリエーションIDをカンマ区切りで登録してください。 (「商品ID,バリエーションID」の形式)<br/>
															・付与ノベルティ設定 > 対象金額と比較する金額は<strong>「適用対象設定」に該当する商品の合計金額（税込）</strong>（※1）になります。<br/>
															・付与ノベルティ設定 > 適用対象会員が同一の場合、対象金額範囲を<strong>重複しないように登録</strong>してください。<br/>
															・付与ノベルティ設定 > 商品IDは<strong>改行区切り</strong>で複数登録できます。<br/>
															<br/>
															※1.合計金額 = ( 商品価格（会員ランク価格 ＞ セール価格 ＞ 特別価格 ＞ 通常価格） * 数量 ) - セットプロモーション割引金額 - 会員ランク割引金額
														</td>
													</tr>
												</tbody></table>
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
	<!--△ 登録/更新 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<asp:HiddenField ID="hfCurentPopup" runat="server" />
<asp:HiddenField ID="hfCurentTargetItem" runat="server" />
<script type="text/javascript">
<!--
	var hfCurentPopup = "<%=hfCurentPopup.ClientID %>";
	var hfCurentTargetItem = "<%=hfCurentTargetItem.ClientID %>";

	// 検索フォーム表示
	function open_poppup_search(link_file, window_name, window_type, popup_type, element) {
		targetId = $(element).parent().parent().parent().find("textarea").eq(0).attr("id");

		$("#" + hfCurentPopup).val(popup_type);
		$("#" + hfCurentTargetItem).val(targetId);

		open_window(link_file, window_name, window_type);
	}

	// ブランド一覧で選択されたブランドIDをセット
	function set_brandinfo(brand_id) {
		var targetId = $("#" + hfCurentTargetItem).val();
		set_textarea_data(targetId, brand_id);
	}

	// カテゴリ一覧で選択されたカテゴリIDをセット
	function set_categoryinfo(category_id) {
		var targetId = $("#" + hfCurentTargetItem).val();
		set_textarea_data(targetId, category_id);
	}

	// 商品一覧で選択された商品ID，商品バリエーションIDをセット
	function set_productinfo(product_id, supplier_id, v_id, product_name, display_price, display_special_price, product_price, sale_id, fixed_purchase_id) {
		var curentPopup = $("#" + hfCurentPopup).val();
		var targetId = $("#" + hfCurentTargetItem).val();

		if (curentPopup == "VARIATION") {
			set_textarea_data(targetId, product_id + "," + product_id + v_id);
		}
		else {
			set_textarea_data(targetId, product_id);
		}
	}

	// 入力域に値をセット
	function set_textarea_data(targetElementId, data) {
		var targetId = $("#" + hfCurentTargetItem).val();
		if ($("#" + targetId).length == 0) return;

		var currentItems = $("#" + targetId).val();
		if (check_duplicate(currentItems, data)) {
			$("#" + targetId).val(currentItems + ((currentItems.trim().length < 1) ? "" : "\r\n") + data);
		}
	}

	// 重複IDチェック
	function check_duplicate(currentItems, item) {
		currentItems = currentItems.replace("\r\n", "\n").split('\n');
		for (i = 0; i < currentItems.length; i++) {
			if (currentItems[i].trim() == item) {
				return false;
			}
		}
		return true;
	}

	// 今日設定
	function SetToday() {
		document.getElementById('<%= ucDateTimePickerPeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
		document.getElementById('<%= ucDateTimePickerPeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucDateTimePickerPeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
		document.getElementById('<%= ucDateTimePickerPeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucDateTimePickerPeriod.ClientID %>');
	}
//-->
</script>
</asp:Content>