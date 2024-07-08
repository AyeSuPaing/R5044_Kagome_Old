<%--
=========================================================================================================
  Module      : レコメンド設定登録ページ(RecommendRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Recommend" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="RecommendRegister.aspx.cs" Inherits="Form_Recommend_RecommendRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">レコメンド設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 登録/更新 ▽-->
	<tr>
		<td>
		<% if (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT || this.ActionStatus == Constants.ACTION_STATUS_INSERT){%>
			<h2 class="cmn-hed-h2">レコメンド設定登録</h2>
		<%} %>
		<% if(this.ActionStatus == Constants.ACTION_STATUS_UPDATE) {%>
			<h2 class="cmn-hed-h2">レコメンド設定編集</h2>
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
												<div id="divComp" runat="server" class="action_part_top" visible="false">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">レコメンド設定を登録/更新しました。</td>
														</tr>
													</table>
												</div>
												<div>
													<span class="notice">
														<strong><asp:Literal ID="lMessages" Visible="false" runat="server" /></strong>
													</span>
												</div>
												<div class="action_part_top">
													<asp:Button ID="btnToReportTop" runat="server" Text="  レポートを見る  " OnClick="btnToReport_Click" />
													<asp:Button ID="btnToListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " OnClick="btnInsertUpdate_Click" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ 基本情報エラー表示 ▽--%>
													<tr id="trRecommendErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
													</tr>
													<tr id="trRecommendErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:Label ID="lbRecommendErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ 基本情報エラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr id="trDisplayRecommendId" runat="server">
														<td class="edit_title_bg" align="left" width="150">レコメンドID</td>
														<td class="edit_item_bg" align="left">
															<asp:Literal id="lRecommendId" runat="server" />
														</td>
													</tr>
													<tr  id="trInputRecommendId" runat="server">
														<td class="edit_title_bg" align="left" width="150">レコメンドID <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbRecommendId" runat="server" Width="300" MaxLength="30" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">レコメンド名（管理用） <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbRecommendName" runat="server" Width="300" MaxLength="100" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">説明（管理用）</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbDiscription" runat="server" TextMode="MultiLine" Width="470" Height="80" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">表示ページ <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:RadioButtonList ID="rblRecommendDisplayPage" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio_button_list"></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">レコメンド区分 <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:RadioButtonList ID="rblRecommendKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio_button_list" OnSelectedIndexChanged="rblRecommendKbn_SelectedIndexChanged" AutoPostBack="true"></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">ワンタイム表示フラグ <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbOnetimeFlg" runat="server" Text="有効" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">開始日時-終了日時 <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<uc:DateTimePickerPeriodInput id="ucDisplayPeriod" runat="server" IsNullEndDateTime="true" />
															<span class="search_btn_sub">(<a href="Javascript:SetToday();">今日</a>)</span>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">適用優先順 <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbPriority" runat="server" Width="50" MaxLength="7" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">有効フラグ <span class="notice">*</span></td>
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
													<% if (Constants.BOTCHAN_OPTION) { %>
														<tr>
															<td class="edit_title_bg" align="left">チャットボットでも同様にレコメンドする</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBox
																	id="cbChatbotUseFlg"
																	runat="server"
																	Text="有効"
																	AutoPostBack="true"
																	OnCheckedChanged="cbChatbotUseFlg_OnCheckedChanged"/>
															</td>
														</tr>
													<% } %>
												</table>
												<br/>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ 適用条件アイテムエラー表示 ▽--%>
													<tr id="trApplyConditionItemErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center">エラーメッセージ</td>
													</tr>
													<tr id="trApplyConditionItemErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left">
															<asp:Label ID="lbApplyConditionItemErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ 適用条件アイテムエラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center">適用条件設定</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left">
															<%--▽ 適用条件アイテム（購入している） ▽--%>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="743" border="0">
																<tr>
																	<td class="edit_title_bg" align="center" colspan="5">
																		下記商品のいずれかを<strong>購入している</strong>&nbsp;（過去注文もしくはカート）
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center" width="75" rowspan="2">
																		選択/削除
																	</td>
																	<td class="edit_title_bg" align="center" width="100" rowspan="2">
																		<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
																				&& (rblRecommendKbn.SelectedValue != Constants.FLG_RECOMMEND_RECOMMEND_KBN_CROSS_SELL)) { %>
																			通常/定期/頒布会商品
																		<% } else { %>
																		通常/定期商品
																		<% } %>
																	</td>
																	<td class="edit_title_bg" align="center" width="175">
																		商品ID
																	</td>
																	<td class="edit_title_bg" align="left" width="290" rowspan="2">
																		商品名
																	<td class="edit_title_bg" align="center" width="110" rowspan="2">
																		指定単位
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center">
																		バリエーションID
																	</td>
																</tr>
																<asp:Repeater ID="rApplyConditionItemsBuy" runat="server" OnItemCommand="rApplyConditionItemsBuy_ItemCommand" ItemType="RecommendApplyConditionItemInput">
																	<ItemTemplate>
																		<tr>
																			<td class="edit_item_bg" align="left" rowspan="2">
																				<input type="button" onclick="javascript:open_poppup_search_for_apply_condition_item_for_buy('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + (Item.IsRecommendApplyConditionItemUnitTypeProduct ? Constants.KBN_PRODUCT_SEARCH_PRODUCT : Constants.KBN_PRODUCT_SEARCH_VARIATION) + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','APPLY_CONDITION_ITEM', '<%# Container.ItemIndex %>');" value="選択" />
																				<asp:Button ID="btnDelete" runat="server" Text="削除" CommandArgument="<%# Container.ItemIndex %>" />
																			</td>
																			<td class="edit_item_bg" align="center" rowspan="2">
																				<asp:DropDownList ID="ddlRecommendApplyConditionItemType" runat="server" Width="100" DataSource='<%# this.ProductTypeList %>' DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlUpsellTargetItemType_SelectedIndexChanged" AutoPostBack="true" SelectedValue=" <%# ValidateSelectedProductType(Item.RecommendApplyConditionItemType) %>" />
																			</td>
																			<td class="edit_item_bg" align="center">
																				<%#: Item.RecommendApplyConditionItemProductId %>
																			</td>
																			<td class="edit_item_bg" align="left" rowspan="2">
																				<%#: Item.CreateProductJointName() %>
																			</td>
																			<td class="edit_item_bg" align="center" rowspan="2">
																				<%#: Item.RecommendApplyConditionItemUnitTypeText %>
																				<asp:HiddenField ID="hfRecommendApplyConditionItemUnitType" runat="server" Value="<%# Item.RecommendApplyConditionItemUnitType %>" />
																				<asp:HiddenField ID="hfRecommendApplyConditionItemProductId" runat="server" Value="<%# Item.RecommendApplyConditionItemProductId %>" />
																				<asp:HiddenField ID="hfRecommendApplyConditionItemVariationId" runat="server" Value="<%# Item.RecommendApplyConditionItemVariationId %>" />
																				<asp:HiddenField ID="hfProductName" runat="server" Value="<%# Item.ProductName %>" />
																				<asp:HiddenField ID="hfVariationName1" runat="server" Value="<%# Item.VariationName1 %>" />
																				<asp:HiddenField ID="hfVariationName2" runat="server" Value="<%# Item.VariationName2 %>" />
																				<asp:HiddenField ID="hfVariationName3" runat="server" Value="<%# Item.VariationName3 %>" />
																				<asp:HiddenField ID="hfFixedPurchaseFlg" runat="server"  Value="<%# Item.FixedPurchaseFlg %>" />
																				<asp:HiddenField ID="hfSubscriptionBoxFlg" runat="server"  Value="<%# Item.SubscriptionBoxFlg %>" />
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_item_bg" align="center">
																				<%#: Item.IsRecommendApplyConditionItemUnitTypeVariation ?  Item.RecommendApplyConditionItemVariationId : "-" %>
																			</td>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
															</table>
															<div style="float:right;margin:5px 5px 5px 0px">
																<input type="button" onclick="javascript:open_poppup_search_for_apply_condition_item_for_buy('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','APPLY_CONDITION_ITEM', '');" value="  バリエーション単位で追加  " />
																<input type="button" onclick="javascript:open_poppup_search_for_apply_condition_item_for_buy('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_PRODUCT + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','APPLY_CONDITION_ITEM', '');" value="  商品単位で追加  " />
															</div>
															<%--△ 適用条件アイテム（購入している） △--%>
															<div style="margin-top:40px;"></div>
															<%--▽ 適用条件アイテム（購入していない） ▽--%>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="743" border="0">
																<tr>
																	<td class="edit_title_bg" align="center" colspan="5">
																		下記商品のいずれかを<strong>購入していない</strong>&nbsp;（過去注文もしくはカート）
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center" width="75" rowspan="2">
																		選択/削除
																	</td>
																	<td class="edit_title_bg" align="center" width="100" rowspan="2">
																		<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
																				&& (rblRecommendKbn.SelectedValue != Constants.FLG_RECOMMEND_RECOMMEND_KBN_CROSS_SELL)) { %>
																			通常/定期/頒布会商品
																		<% } else { %>
																		通常/定期商品
																		<% } %>
																	</td>
																	<td class="edit_title_bg" align="center" width="175">
																		商品ID
																	</td>
																	<td class="edit_title_bg" align="left" width="290" rowspan="2">
																		商品名
																	<td class="edit_title_bg" align="center" width="110" rowspan="2">
																		指定単位
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center">
																		バリエーションID
																	</td>
																</tr>
																<asp:Repeater ID="rApplyConditionItemsNotBuy" runat="server" OnItemCommand="rApplyConditionItemsNotBuy_ItemCommand" ItemType="RecommendApplyConditionItemInput">
																	<ItemTemplate>
																		<tr>
																			<td class="edit_item_bg" align="left" rowspan="2">
																				<input type="button" onclick="javascript:open_poppup_search_for_apply_condition_item_for_not_buy('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + (Item.IsRecommendApplyConditionItemUnitTypeProduct ? Constants.KBN_PRODUCT_SEARCH_PRODUCT : Constants.KBN_PRODUCT_SEARCH_VARIATION) + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','APPLY_CONDITION_ITEM', '<%# Container.ItemIndex %>');" value="選択" />
																				<asp:Button ID="btnDelete" runat="server" Text="削除" CommandArgument="<%# Container.ItemIndex %>" />
																			</td>
																			<td class="edit_item_bg" align="center" rowspan="2">
																				<asp:DropDownList ID="ddlRecommendApplyConditionItemType" runat="server" Width="100" DataSource='<%# this.ProductTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue="<%# ValidateSelectedProductType(Item.RecommendApplyConditionItemType) %>" />
																			</td>
																			<td class="edit_item_bg" align="center">
																				<%#: Item.RecommendApplyConditionItemProductId %>
																			</td>
																			<td class="edit_item_bg" align="left" rowspan="2">
																				<%#: Item.CreateProductJointName() %>
																			</td>
																			<td class="edit_item_bg" align="center" rowspan="2">
																				<%#: Item.RecommendApplyConditionItemUnitTypeText %>
																				<asp:HiddenField ID="hfRecommendApplyConditionItemUnitType" runat="server" Value="<%# Item.RecommendApplyConditionItemUnitType %>" />
																				<asp:HiddenField ID="hfRecommendApplyConditionItemProductId" runat="server" Value="<%# Item.RecommendApplyConditionItemProductId %>" />
																				<asp:HiddenField ID="hfRecommendApplyConditionItemVariationId" runat="server" Value="<%# Item.RecommendApplyConditionItemVariationId %>" />
																				<asp:HiddenField ID="hfProductName" runat="server" Value="<%# Item.ProductName %>" />
																				<asp:HiddenField ID="hfVariationName1" runat="server" Value="<%# Item.VariationName1 %>" />
																				<asp:HiddenField ID="hfVariationName2" runat="server" Value="<%# Item.VariationName2 %>" />
																				<asp:HiddenField ID="hfVariationName3" runat="server" Value="<%# Item.VariationName3 %>" />
																				<asp:HiddenField ID="hfFixedPurchaseFlg" runat="server"  Value="<%# Item.FixedPurchaseFlg %>" />
																				<asp:HiddenField ID="hfSubscriptionBoxFlg" runat="server"  Value="<%# Item.SubscriptionBoxFlg %>" />
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_item_bg" align="center">
																				<%#: Item.IsRecommendApplyConditionItemUnitTypeVariation ?  Item.RecommendApplyConditionItemVariationId : "-" %>
																			</td>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
															</table>
															<div style="float:right;margin:5px 5px 5px 0px">
																<input type="button" onclick="javascript:open_poppup_search_for_apply_condition_item_for_not_buy('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','APPLY_CONDITION_ITEM', '');" value="  バリエーション単位で追加  " />
																<input type="button" onclick="javascript:open_poppup_search_for_apply_condition_item_for_not_buy('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_PRODUCT + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','APPLY_CONDITION_ITEM', '');" value="  商品単位で追加  " />
															</div>
															<%--△ 適用条件アイテム（購入していない） △--%>
															<%-- 隠しボタン：適用条件商品セット --%>
															<asp:LinkButton id="lbSetApplyConditionItem" runat="server" OnClick="lbSetApplyConditionItem_Click"></asp:LinkButton>
														</td>
													</tr>
												</table>
												<br />
												<div id="divRecommendItem" runat="server">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ レコメンドアイテムエラー表示 ▽--%>
													<tr id="trRecommendItemErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center">エラーメッセージ</td>
													</tr>
													<tr id="trRecommendItemErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left">
															<asp:Label ID="lbRecommendItemErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ レコメンドアイテムエラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center">レコメンド商品設定</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left">
															<div style="margin-bottom:5px"></div>
															<div id="divUpSell" runat="server">
															カート内の
																<asp:DropDownList ID="ddlUpsellTargetItemType" runat="server" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlUpsellTargetItemType_SelectedIndexChanged" AutoPostBack="true" Width="100" />
															「<a href="javascript:void(0)" onclick="javascript:open_poppup_search_for_recommend_item('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','UP_SELL_TARGET_ITEM', '');">
																<asp:Literal ID="lUpsellTargetItem" runat="server" Text="選択する" /></a>」
																<asp:HiddenField ID="hfRecommendUpsellTargetItemProductId" runat="server" />
																<asp:HiddenField ID="hfRecommendUpsellTargetItemVariationId" runat="server" />
																<asp:HiddenField ID="hfRecommendUpsellTargetItemProductName" runat="server" />
																<asp:HiddenField ID="hfRecommendUpsellTargetItemVariationName1" runat="server" />
																<asp:HiddenField ID="hfRecommendUpsellTargetItemVariationName2" runat="server" />
																<asp:HiddenField ID="hfRecommendUpsellTargetItemVariationName3" runat="server" />
																<asp:HiddenField ID="hfRecommendUpsellTargetItemShippingType" runat="server"/>
																<asp:HiddenField ID="hfRecommendUpsellTargetItemFixedPurchaseFlg" runat="server" />
																<asp:HiddenField ID="hfRecommendUpsellTargetItemSubscriptionBoxFlg" runat="server" />
															</div>
															<div id="divCrossSell" runat="server">
																カートに
															</div>
															<div style="margin-bottom:10px"></div>
															<%-- 隠しボタン：アップセル対象商品＆レコメンド商品&配送パターン設定セット --%>
															<asp:LinkButton id="lbSetUpsellTargetItem" runat="server" OnClick="lbSetUpsellTargetItem_Click"></asp:LinkButton>
															<asp:LinkButton id="lbSetRecommendItem" runat="server" OnClick="lbSetRecommendItem_Click"></asp:LinkButton>
															<asp:LinkButton ID="lbSetFixedPurchaseShippingPattern" runat="server" OnClick="lbSetFixedPurchaseShippingPattern_Click"></asp:LinkButton>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="743" border="0">
																<tr>
																	<td class="edit_title_bg" align="center" colspan="5">
																		レコメンド商品
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center" width="75" rowspan="2">
																		選択/削除
																	</td>
																	<td class="edit_title_bg" align="center" width="100" rowspan="2">
																		<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
																			通常/定期/頒布会商品
																		<% } else { %>
																		通常/定期商品
																		<% } %>
																	</td>
																	<td class="edit_title_bg" align="center" width="175">
																		商品ID
																	</td>
																	<td class="edit_title_bg" align="left" width="290" rowspan="2">
																		商品名
																	</td>
																	<td class="edit_title_bg" align="center" width="110" rowspan="2">
																		数量
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="center">
																		バリエーションID
																	</td>
																</tr>
																<asp:Repeater ID="rRecommendItems" runat="server" OnItemCommand="rRecommendItems_ItemCommand" ItemType="RecommendItemInput">
																	<ItemTemplate>
																		<tr>
																			<td class="edit_item_bg" align="left" rowspan="2">
																				<input type="button" onclick="javascript:open_poppup_search_for_recommend_item('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>	','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','RECOMMEND_ITEM', '<%# Container.ItemIndex %>	');" value="選択" />
																				<asp:Button ID="btnDelete" runat="server" Text="削除" CommandArgument="<%# Container.ItemIndex %>" />
																			</td>
																			<td class="edit_item_bg" align="center" rowspan="2">
																				<asp:DropDownList ID="ddlRecommendItemType" runat="server" Width="100" DataSource='<%# this.ProductTypeList %>' DataTextField="Text" DataValueField="Value" AutoPostBack="true" OnSelectedIndexChanged="ddlRecommendItemType_SelectedIndexChanged" SelectedValue="<%# ValidateSelectedProductType(Item.RecommendItemType) %>" />
																				<span runat="server" visible="<%# DisplayFixedPurchaseShippingPattern(Item) %>">
																					<br />
																					定期配送パターン：<a href="javascript:void(0);" onclick="javascript:open_popup_modify_fixedpurchase_setting('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXEDPURCHASE_SHIPPING_PATTERN + "?" 
																																																		+ Constants.REQUEST_KEY_SHOP_ID + "=" + Item.ShopId
																																																		+ "&" + Constants.REQUEST_KEY_PRODUCT_ID + "=" + HttpUtility.UrlEncode(Item.RecommendItemProductId)
																																																		+ "&" + Constants.REQUEST_KEY_VARIATION_ID + "=" + HttpUtility.UrlEncode(Item.RecommendItemVariationId)
																																																		+ "&" + Constants.REQUEST_KEY_RECOMMENDITEM_FIXED_PURCHASE_KBN + "=" + Item.FixedPurchaseKbn
																																																		+ "&" + Constants.REQUEST_KEY_RECOMMENDITEM_FIXED_PURCHASE_SETTING1 + "=" + Item.FixedPurchaseSetting1) %>'
																																																, 'shipping_pattern'
																																																, 'width=800,height=400,top=320,left=420,status=NO,scrollbars=yes'
																																																, '<%# Container.ItemIndex %>');">
																						<asp:Literal ID="lFixedPurchaseShippingPattern" Text='<%# string.IsNullOrEmpty(Item.CreateFixedPurchaseSettingMessage()) ? "設定する" : Item.CreateFixedPurchaseSettingMessage() %>' runat="server" /></a>
																					<asp:HiddenField ID="hfFixedPurchaseKbn" runat="server" Value="<%# Item.FixedPurchaseKbn %>" />
																					<asp:HiddenField ID="hfFixedPurchaseSetting1" runat="server" Value="<%# Item.FixedPurchaseSetting1 %>" />
																					<asp:HiddenField ID="hfIsValidateFixedPurchaseShippingPattern" runat="server" Value="<%# DisplayFixedPurchaseShippingPattern(Item) %>" />
																				</span>
																			</td>
																			<td class="edit_item_bg" align="center">
																				<%#: Item.RecommendItemProductId %>
																			</td>
																			<td class="edit_item_bg" align="left" rowspan="2">
																				<%#: Item.CreateProductJointName() %>
																			</td>
																			<td class="edit_item_bg" align="center" rowspan="2">
																				<% if (rblRecommendKbn.SelectedValue == Constants.FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL) { %>
																				<span style="font-size:xx-small">
																					<asp:CheckBox ID="cbRecommendItemAddQuantityType" runat="server" Checked='<%# Item.RecommendItemAddQuantityType == "SAME_QUANTITY" %>' Text="変更商品と同じ数量" OnCheckedChanged="cbRecommendItemAddQuantityType_CheckedChanged" AutoPostBack="true" />
																				</span>
																				<br/>
																				<% } %>
																				<asp:TextBox id="tbRecommendItemAddQuantity" runat="server" Width="50" Text="<%# Item.RecommendItemAddQuantity %>" Enabled='<%# Item.RecommendItemAddQuantityType != "SAME_QUANTITY" %>' MaxLength="7" />
																				<asp:HiddenField ID="hfRecommendItemProductId" runat="server" Value="<%# Item.RecommendItemProductId %>" />
																				<asp:HiddenField ID="hfRecommendItemVariationId" runat="server" Value="<%# Item.RecommendItemVariationId %>" />
																				<asp:HiddenField ID="hfProductName" runat="server" Value="<%# Item.ProductName %>" />
																				<asp:HiddenField ID="hfVariationName1" runat="server" Value="<%# Item.VariationName1 %>" />
																				<asp:HiddenField ID="hfVariationName2" runat="server" Value="<%# Item.VariationName2 %>" />
																				<asp:HiddenField ID="hfVariationName3" runat="server" Value="<%# Item.VariationName3 %>" />
																				<asp:HiddenField ID="hfShippingType" runat="server" Value="<%# Item.ShippingType %>"/>
																				<asp:HiddenField ID="hfFixedPurchaseFlg" runat="server"  Value="<%# Item.FixedPurchaseFlg %>" />
																				<asp:HiddenField ID="hfSubscriptionBoxFlg" runat="server"  Value="<%# Item.SubscriptionBoxFlg %>" />
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_item_bg" align="center">
																				<%#: (Item.RecommendItemProductId != Item.RecommendItemVariationId) ? Item.RecommendItemVariationId : "-" %>
																			</td>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
															</table>
															<div style="float:right;margin:5px 5px 5px 0px">
																<input type="button" onclick="javascript:open_poppup_search_for_recommend_item('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>	','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','RECOMMEND_ITEM', '');" value="  追加  " />
															</div>
															<div style="margin-bottom:10px"></div>
															<%= divUpSell.Visible ? "に変更する。" : "を追加する。" %>
														</td>
													</tr>
												</table>
												<br />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="3">レコメンドHTML設定</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left" colspan="3">
															レコメンド商品を表示するためのHTMLを設定します。<br />
															各ボタン画像パス及びリンクを利用する場合、下記のタグを設定してください。<br />
															・レコメンド商品投入ボタン画像URLタグ：<strong><%= RecommendDisplayConverter.TAG_ADD_ITEM_BUTTONIMAGE_URL %></strong><br />
															・レコメンド商品投入ボタンリンクタグ：<strong><%= RecommendDisplayConverter.TAG_ADD_ITEM_LINK %></strong><br />
															<br />
															<strong>[例]</strong><br />
															<strong><%: string.Format("<a href=\"{0}\"><img src=\"{1}\" /></a>", RecommendDisplayConverter.TAG_ADD_ITEM_LINK, RecommendDisplayConverter.TAG_ADD_ITEM_BUTTONIMAGE_URL) %></strong>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="130" rowspan="2">PCサイト用</td>
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:RadioButtonList ID="rblRecommendDisplayKbnPc" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio_button_list"></asp:RadioButtonList>
															<input type= "button" onclick="javascript:open_wysiwyg('<%= tbRecommendDisplayPc.ClientID %>','<%= rblRecommendDisplayKbnPc.ClientID %>');" value="  HTMLエディタ  " />
															<div style="margin-bottom:5px"></div>
															<asp:TextBox ID="tbRecommendDisplayPc" runat="server" Width="600" Rows="10" TextMode="MultiLine"/>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="130">レコメンド商品投入ボタン</td>
														<td class="edit_item_bg" align="left">
															<input type="button" value="  ファイルアップロード  " onclick="javascript:open_poppup_buttonimage_fileaupload('<%= WebSanitizer.UrlAttrHtmlEncode(CreateRecommendButtonImageFileUploadUrl(this.TempRecommendId, ButtonImageType.AddItemPc)) %>');" />
															<div style="margin-bottom:3px"></div>
															<a id="aButtonImageAddItemPc" runat="server" visible="false" href="javascript:void(0)" class="popup_img">プレビュー<span><img id="imgButtonImageAddItemPc" runat="server" /></span></a>
															<asp:LinkButton ID="lbDisplayButtonImagePreview" runat="server" OnClick="lbDisplayButtonImagePreview_Click" />
															<div style="margin-bottom:3px"></div>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="130" rowspan="2">スマートフォンサイト用</td>
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:RadioButtonList ID="rblRecommendDisplayKbnSp" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio_button_list"></asp:RadioButtonList>
															<input type="button" onclick="javascript:open_wysiwyg('<%= tbRecommendDisplaySp.ClientID %>','<%= rblRecommendDisplayKbnSp.ClientID %>');" value="  HTMLエディタ  " />
															<div style="margin-bottom:5px"></div>
															<asp:TextBox ID="tbRecommendDisplaySp" runat="server" Width="600" Rows="10" TextMode="MultiLine"/>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="130">レコメンド商品投入ボタン</td>
														<td class="edit_item_bg" align="left">
															<input type="button" value="  ファイルアップロード  " onclick="javascript:open_poppup_buttonimage_fileaupload('<%= WebSanitizer.UrlAttrHtmlEncode(CreateRecommendButtonImageFileUploadUrl(this.TempRecommendId, ButtonImageType.AddItemSp)) %>');" />
															<div style="margin-bottom:3px"></div>
															<a id="aButtonImageAddItemSp" runat="server" visible="false" href="javascript:void(0)" class="popup_img">プレビュー<span><img id="imgButtonImageAddItemSp" runat="server" /></span></a>
															<div style="margin-bottom:3px"></div>
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<asp:Button ID="btnToReportBottom" runat="server" Text="  レポートを見る  " OnClick="btnToReport_Click" />
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
													<tbody>
														<tr class="info_item_bg">
															<td align="left">備考<br/>
																・適用条件設定は<strong>かつ（AND）条件</strong>になります。<br/>
																・アップセルの場合、レコメンド商品設定 > アップセル対象商品は<strong>必須項目</strong>になります。<br/>
																・アップセル・クロスセルの場合、レコメンド商品設定 > レコメンド対象商品は<strong>必須項目</strong>になります。<br/>
																・表示ページが注文完了ページの場合、全てのレコメンド商品が注文時の決済方法で購入可能なレコメンド設定が適用されます。<br />
																・表示ページが注文完了ページの場合、決済方法が<strong><%: string.Join("または", Constants.RECOMMENDOPTION_APPLICABLE_PAYMENTIDS_FOR_ORDER_COMPLETE.Where(payment => (payment != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)).Select(pay => ValueText.GetValueText(Constants.TABLE_PAYMENT, "payment_type", pay)).ToArray()) %>の場合のみ</strong>レコメンド設定を適用します。<br />
																<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
																・頒布会商品はレコメンド区分がアップセル・レコメンドHTMLの場合のみ設定可能です。<br/>
																・頒布会商品は頒布会商品同士の場合のみレコメンド可能です。<br/>
																<% } %>
															</td>
														</tr>
													</tbody>
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
	<!--△ 登録/更新 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<asp:HiddenField ID="hfCurentPopup" runat="server" />
<%-- 隠しタグ：適用対象アイテムセット用 --%>
<asp:HiddenField ID="hfApplyConditionItem" runat="server" />
<asp:HiddenField ID="hfApplyConditionItemIndex" runat="server" />
<asp:HiddenField ID="hfRecommendApplyConditionType" runat="server" />
<%-- 隠しタグ：アップセル対象アイテムセット用 --%>
<asp:HiddenField ID="hfUpsellTargetItem" runat="server" />
<%-- 隠しタグ：レコメンドアイテムセット用 --%>
<asp:HiddenField ID="hfRecommendItem" runat="server" />
<asp:HiddenField ID="hfRecommendItemIndex" runat="server" />
<%-- 隠しタグ：配送パターン設定用 --%>
<asp:HiddenField ID="hfFixedPurchaseShippingPattern" runat="server" />
<asp:HiddenField ID="hfFixedPurchaseShippingPatternIndex" runat="server" />
<script type="text/javascript">
<!--
	var hfCurentPopup = "<%=hfCurentPopup.ClientID %>";

	// 検索フォーム表示（適用条件アイテム用）
	function open_poppup_search_for_apply_condition_item_for_buy(link_file, window_name, window_type, popup_type, apply_condition_item_index) {
		$("#<%= hfRecommendApplyConditionType.ClientID %>").val("<%= Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY %>");
		open_poppup_search_for_apply_condition_item(link_file, window_name, window_type, popup_type, apply_condition_item_index);
	}
	function open_poppup_search_for_apply_condition_item_for_not_buy(link_file, window_name, window_type, popup_type, apply_condition_item_index) {
		$("#<%= hfRecommendApplyConditionType.ClientID %>").val("<%= Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_NOT_BUY %>");
		open_poppup_search_for_apply_condition_item(link_file, window_name, window_type, popup_type, apply_condition_item_index);
	}
	function open_poppup_search_for_apply_condition_item(link_file, window_name, window_type, popup_type, apply_condition_item_index) {
		$("#" + hfCurentPopup).val(popup_type);
		$("#<%= hfApplyConditionItemIndex.ClientID %>").val(apply_condition_item_index);

		open_window(link_file, window_name, window_type);
	}

	// 検索フォーム表示（レコメンドアイテム用）
	function open_poppup_search_for_recommend_item(link_file, window_name, window_type, popup_type, recommend_product_item_index) {
		$("#" + hfCurentPopup).val(popup_type);
		$("#<%= hfRecommendItemIndex.ClientID %>").val(recommend_product_item_index);

		open_window(link_file, window_name, window_type);
	}

	// 商品一覧で選択された商品ID，商品バリエーションIDをセット
	function set_productinfo(product_id, supplier_id, v_id, product_name, display_price, display_special_price, product_price, sale_id, fixed_purchase_id) {
		var curentPopup = $("#" + hfCurentPopup).val();

		if (curentPopup == "APPLY_CONDITION_ITEM") {
			$("#<%= hfApplyConditionItem.ClientID %>").val(product_id + "," + product_id + v_id);
			__doPostBack("<%= lbSetApplyConditionItem.UniqueID %>", "");
		}else if (curentPopup == "UP_SELL_TARGET_ITEM") {
			$("#<%= hfUpsellTargetItem.ClientID %>").val(product_id + "," + product_id + v_id);
			__doPostBack("<%= lbSetUpsellTargetItem.UniqueID %>", "");
		}else if (curentPopup == "RECOMMEND_ITEM") {
			$("#<%= hfRecommendItem.ClientID %>").val(product_id + "," + product_id + v_id);
			__doPostBack("<%= lbSetRecommendItem.UniqueID %>", "");
		}
	}

	// 配送パターン設定フォーム表示
	function open_popup_modify_fixedpurchase_setting(link_file, window_name, window_type, recommend_product_item_index)
	{
		$("#<%= hfFixedPurchaseShippingPatternIndex.ClientID %>").val(recommend_product_item_index);

		open_window(link_file, window_name, window_type);
	}

	// 設定した配送パターンをセット
	function set_modify_fixedpurchase_setting(fixed_purchase_kbn, fixed_purchase_setting1)
	{
		$("#<%= hfFixedPurchaseShippingPattern.ClientID %>").val(fixed_purchase_kbn + "," + fixed_purchase_setting1);
		__doPostBack("<%= lbSetFixedPurchaseShippingPattern.UniqueID %>", "");
	}

	// 今日設定
	function SetToday() {
		document.getElementById('<%= ucDisplayPeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
		document.getElementById('<%= ucDisplayPeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucDisplayPeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
		document.getElementById('<%= ucDisplayPeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucDisplayPeriod.ClientID %>');
	}

	// ボタン画像アップロード画面表示
	function open_poppup_buttonimage_fileaupload(link_file) {
		open_window(link_file, 'buttonimage_fileaupload', 'width=1000,height=450,top=120,left=420,status=NO,scrollbars=yes');
	}

	// ボタン画像表示プレビュー（ボタン画像アップロード画面から実行）
	function display_buttonimage_preview()
	{
		__doPostBack("<%= lbDisplayButtonImagePreview.UniqueID %>", "");
	}
	//-->
</script>
</asp:Content>
