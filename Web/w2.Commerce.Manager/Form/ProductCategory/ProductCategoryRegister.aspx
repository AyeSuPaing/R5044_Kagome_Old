<%--
=========================================================================================================
  Module      : 商品カテゴリ登録ページ(ProductCategoryRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductCategoryRegister.aspx.cs" Inherits="Form_ProductCategory_ProductCategoryRegister" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%-- 
テキストボックス内でEnterキーを押してSubmit（一番上に配置されているTextBoxのSubmit）送信しようとすると、
IEのバグでテキストボックスが画面上に一つのみ配置されている場合にSubmit送信されない不具合の対応として、
ダミーのTextBoxを非表示で配置している。
--%>
<asp:TextBox id="tbDummy" runat="server" style="visibility:hidden;display:none;"></asp:TextBox>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">商品カテゴリ設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td colspan="2">
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
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="90"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品カテゴリ</td>
														<td class="search_item_bg" width="135"><asp:DropDownList id="ddlSearchKey" runat="server" CssClass="search_item_width">
																<asp:ListItem Selected="True"></asp:ListItem>
															</asp:DropDownList></td>
														<td class="search_title_bg" width="90"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />カテゴリID</td>
														<td class="search_item_bg" width="135"><asp:TextBox id="tbSearchWord" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_btn_bg" width="83">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTCATEGORY_REGISTER %>">クリア</a></div>
															<div class="search_btn_sub"><asp:LinkButton id="lbExportProductListUrl" Runat="server" OnClick="lbExportProductListUrl_Click">商品一覧ページURL出力</asp:LinkButton></div>
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<div class="search_btn_sub"><asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_Click" >翻訳設定出力</asp:LinkButton></div>
															<% } %>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<% if ((this.ActionStatus == null) || (this.ActionStatus == Constants.ACTION_STATUS_DETAIL) || (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)) { %>
										<%-- マスタ出力 --%>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="ProductCategory" TableWidth="758" />
											</td>
										</tr>
										<%} %>
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
	<!--△ 検索 △-->
	<tr>
		<td colspan="2"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<!--▽ 一覧・詳細 ▽-->
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h2 class="cmn-hed-h2">商品カテゴリ設定一覧</h2></td>
		<td><h2 id="imgEdit" Visible="false" class="cmn-hed-h2" runat="server">商品カテゴリ情報編集</h2><h2 id="imgRegister" Visible="false" class="cmn-hed-h3" runat="server">商品カテゴリ情報登録</h2><h2 id="imgDetail" Visible="false" class="cmn-hed-h2" runat="server">商品カテゴリ詳細</h2><h2 id="imgConfirm" Visible="false" class="cmn-hed-h2" runat="server">商品カテゴリ入力確認</h2></td>
	</tr>
	<tr>
		<td valign="top">
			<table class="box_border" cellspacing="1" cellpadding="0" width="375" border="0">
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
												<table class="list_pager" cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
													</tr>
												</table>
												<!-- ページング--></td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="375" border="0">
													<tr class="list_title_bg">
														<td align="center">商品カテゴリ名</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductCategoryDetailUrl((string)Eval(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID))) %>')">
																<td align="left">
																	<div style="margin-left:<%# (((string)Eval(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID)).Length/Constants.CONST_CATEGORY_ID_LENGTH -1)*10 %>px;">
																	<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTCATEGORY_NAME)) %>
																</td>
															</tr>
														</ItemTemplate>
														<AlternatingItemTemplate>
															<tr class="list_item_bg2" onmouseover="listselect_mover(this)" onmouseout="listselect_mout2(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductCategoryDetailUrl((string)Eval(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID))) %>')">
																<td align="left">
																	<div style="margin-left:<%# (((string)Eval(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID)).Length/Constants.CONST_CATEGORY_ID_LENGTH -1)*10 %>px;">
																	<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTCATEGORY_NAME)) %>
																</td>
															</tr>
														</AlternatingItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									</table>
								</td>
								<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top">
			<table class="box_border" cellspacing="1" cellpadding="3" width="375" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr id="trNotSelect" runat="server" Visible="True" width="370">
											<td>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
													<tr>
														<td class="edit_item_bg">
															<div id="divNotSelectMessage" runat="server"></div>
														</td>
													</tr>
												</table>
												<div class="action_part_bottom"><asp:Button id="btnCategoryRootInsertTop" runat="server" Text="  最上位カテゴリの登録  " OnClick="btnCategoryRootInsert_Click" /></div>
											</td>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr id="trDetail" runat="server" Visible="False">
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
											<td>
												<div id="divComp" runat="server" class="action_part_middle" Visible="True">
												<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
													<tr class="info_item_bg">
														<td align="left"><asp:Label ID="lMessage" runat="server"></asp:Label>
														</td>
													</tr>
												</table>
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
													<tr>
														<td class="edit_title_bg" align="left" width="35%">親カテゴリ</td>
														<td class="edit_item_bg" align="left">
															<%# (string.IsNullOrEmpty((string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID])) 
																	? "親カテゴリなし"
																	: WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID]) %>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリID</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID])%></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリ名</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_NAME])%></td>
													</tr>
													<%-- カテゴリ名翻訳設定情報 --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater ID="rTranslationProductCategoryName" runat="server"
														DataSource="<%# this.CategoryTranslationData %>"
														ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
													<ItemTemplate>
													<tr>
														<td class="edit_title_bg" align="left">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="edit_item_bg" align="left"><%#: Item.AfterTranslationalName %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<% if (this.IsOperationalCountryJp) { %>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリ名 (フリガナ)</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_NAME_KANA])%></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">表示順</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_DISPLAY_ORDER])%></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">子カテゴリの並び順</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN, (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN])) %></td>
													</tr>
													<%if (Constants.PRODUCT_BRAND_ENABLED){ %>
													<tr>
														<td class="edit_title_bg" align="left">許可ブランドID</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(((string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS] == "") ? "全て" : m_htParam[Constants.FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS])%></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">SEOキーワード</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_SEO_KEYWORDS])%></td>
													</tr>
													<%if (Constants.FRIENDLY_URL_ENABLED){ %>
													<tr>
														<td class="edit_title_bg" align="left">カノニカルタグ用テキスト</td>
														<td class="edit_item_bg" align="left"><%#: m_htParam[Constants.FIELD_PRODUCTCATEGORY_CANONICAL_TEXT]%></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリページURL</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_URL])%><br />
															<span id="spanCategoryPreviewDetail" runat="server" visible='<%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_URL]) != "" %>'>
																<a href="javascript:open_window('<%# CreatePreviewUrl(WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_URL]))%>','preview_productcategory','');">プレビュー</a>
															</span>
														</td>
													</tr>
													<% if (Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg){ %>
													<tr>
														<td class="edit_title_bg" align="left">外部レコメンド利用フラグ</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_USE_RECOMMEND_FLG, (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_USE_RECOMMEND_FLG]))%></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">有効フラグ</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_VALID_FLG, (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_VALID_FLG])) %></td>
													</tr>
													<%-- 会員ランクオプションが有効の場合--%>
													<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
													<%if (this.IsParentCategory == false) { %>
													<tr>
														<td class="edit_title_bg" align="left">会員ランク表示制御</td>
														<td class="edit_item_bg" align="left">
															閲覧可能会員ランク：
															<%# WebSanitizer.HtmlEncode(GetMemberRankName((string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID]))%><br />
															<br />
															閲覧可能ランク未満の場合<br />&nbsp;&nbsp;カテゴリツリーに
															<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG, (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG])) %>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">定期会員限定フラグ</td>
														<td class="edit_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG, (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG])) %>
														</td>
													</tr>
													<%} else if (this.IsParentCategory){ %>
														<tr>
															<td class="edit_title_bg" align="left">会員ランク表示制御</td>
															<td class="edit_item_bg" align="left">
																親カテゴリに準ずる
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">定期会員限定フラグ</td>
															<td class="edit_item_bg" align="left">
																親カテゴリに準ずる
															</td>
														</tr>
													<% } %>
													<% } %>
												</table>
												<div class="action_part_bottom"><asp:Button id="btnCategoryEditBottom" runat="server" Text="  編集する  " OnClick="btnCategoryEdit_Click" CssClass="cmn-btn-sub-action" />
													<asp:Button id="btnCategoryRootInsertBottom" runat="server" Text="  最上位カテゴリの登録  " OnClick="btnCategoryRootInsert_Click" CssClass="cmn-btn-sub-action" /><br />
													<br />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " OnClick="btnDelete_Click" CssClass="cmn-btn-sub-action" OnClientClick="return confirm('情報を削除してもよろしいですか？')" />
													<asp:Button id="btnCategoryChildInsertBottom" runat="server" Text="  子カテゴリの登録  " OnClick="btnCategoryChildInsert_Click" CssClass="cmn-btn-sub-action" /><br />
												</div>
											</td>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr id="trEdit" runat="server" Visible="False">
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
											<td>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
													<tr>
														<td class="edit_title_bg" align="left" width="35%">親カテゴリ</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID])%></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリID<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbParentCategoryId" runat="server" Width="64px" Text="<%# m_htParam[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID] %>" ReadOnly="True"></asp:TextBox>
															<asp:TextBox id="tbCategoryId" runat="server" Text='<%# m_htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_input"] %>' MaxLength="3"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリ名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbName" runat="server" Text="<%# m_htParam[Constants.FIELD_PRODUCTCATEGORY_NAME] %>" MaxLength="40" Width="220"></asp:TextBox></td>
													</tr>
													<% if (this.IsOperationalCountryJp) { %>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリ名 (フリガナ)</td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbNameKana" runat="server" Text="<%# m_htParam[Constants.FIELD_PRODUCTCATEGORY_NAME_KANA] %>" MaxLength="60" Width="220"></asp:TextBox></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">表示順</td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbDisplayOrder" runat="server" Text="<%# m_htParam[Constants.FIELD_PRODUCTCATEGORY_DISPLAY_ORDER] ?? 1 %>" MaxLength="3"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">子カテゴリの並び順</td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList ID="ddlChildCategorySortKbn" runat="server" 
																SelectedValue='<%# (StringUtility.ToEmpty(m_htParam[Constants.FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN]) != "") ? m_htParam[Constants.FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN].ToString() : "0" %>'></asp:DropDownList></td>
													</tr>
													<%if (Constants.PRODUCT_BRAND_ENABLED){ %>
													<tr>
														<td class="edit_title_bg" align="left">許可ブランドID</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox id="cbPermitBrand" runat="server" Text="全て" AutoPostBack="true" /><br />
															<asp:CheckBoxList id="cblPermitBrandIds" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">SEOキーワード</td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbSeoKeywords" runat="server" Text="<%# m_htParam[Constants.FIELD_PRODUCTCATEGORY_SEO_KEYWORDS] %>" MaxLength="200" Width="220"></asp:TextBox></td>
													</tr>
													<%if (Constants.FRIENDLY_URL_ENABLED){ %>
													<tr>
														<td class="edit_title_bg" align="left">カノニカルタグ用テキスト</td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbCanonicalText" runat="server" Text="<%# m_htParam[Constants.FIELD_PRODUCTCATEGORY_CANONICAL_TEXT] %>" MaxLength="100" Width="220"></asp:TextBox></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリページURL</td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbUrl" runat="server" Text="<%# m_htParam[Constants.FIELD_PRODUCTCATEGORY_URL] %>" MaxLength="256" Width="220"></asp:TextBox><br />
														<a href="javascript:open_preview_window_modify(document.getElementById('<%= tbUrl.ClientID %>').value);">プレビュー</a></td>
													</tr>
													<% if (Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg){ %>
													<tr>
														<td class="edit_title_bg" align="left">外部レコメンド利用フラグ</td>
														<td class="edit_item_bg" align="left">
														<asp:CheckBox id="cbUseRecommendFlg" runat="server" Checked="<%# ((string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_USE_RECOMMEND_FLG] != Constants.FLG_PRODUCTCATEGORY_USE_RECOMMEND_FLG_INVALID) %>" Text="有効" /></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">有効フラグ</td>
														<td class="edit_item_bg" align="left">
														<asp:CheckBox id="cbValidFlg" runat="server" Checked="<%# ((string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_VALID_FLG] != Constants.FLG_PRODUCTCATEGORY_VALID_FLG_INVALID) %>" Text="有効" /></td>
													</tr>
													<%-- 会員ランクオプションが有効の場合--%>
													<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
													<%if (this.IsParentCategory == false) { %>
													<tr>
														<td class="edit_title_bg" align="left">会員ランク表示制御</td>
														<td class="edit_item_bg" align="left">
															閲覧可能会員ランク<br />
															<asp:DropDownList ID="ddlDisplayMemberRank" runat="server"></asp:DropDownList><br />
															<asp:CheckBox ID="cbLowerMemberCanDisplayTreeFlg" Text="閲覧可能ランク未満でもカテゴリツリーに表示" runat="server" Checked="<%# ((string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG] != Constants.FLG_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG_INVALID) %>" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">定期会員限定フラグ</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbOnlyFixedPurchaseMemberFlg" runat="server" Checked="<%# ((string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG] == Constants.FLG_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG_VALID) %>"  Text="有効" />
														</td>
													</tr>
													<%} else if (this.IsParentCategory){ %>
														<tr>
															<td class="edit_title_bg" align="left">会員ランク表示制御</td>
															<td class="edit_item_bg" align="left">
																親カテゴリに準ずる
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">定期会員限定フラグ</td>
															<td class="edit_item_bg" align="left">
																親カテゴリに準ずる
															</td>
														</tr>
													<% } %>
													<% } %>
												</table>
												<div class="action_part_bottom">
													<asp:Button id="btnBackDetailBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" CssClass="cmn-btn-sub-action" />
													<asp:Button id="btnCategoryConfirmBottom" runat="server" Text="  確認する  " OnClick="btnCategoryConfirm_Click" CssClass="cmn-btn-sub-action" />
												</div>
											</td>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr id="trConfirm" runat="server" Visible="False">
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
											<td>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
													<tr>
														<td class="edit_title_bg" align="left" width="35%">親カテゴリ</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID])%></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリID</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID])%></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリ名</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_NAME])%></td>
													</tr>
													<% if (this.IsOperationalCountryJp) { %>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリ名 (フリガナ)</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_NAME_KANA])%></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">表示順</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_DISPLAY_ORDER])%></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">子カテゴリの並び順</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN, (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN])) %></td>
													</tr>
													<%if (Constants.PRODUCT_BRAND_ENABLED){ %>
													<tr>
														<td class="edit_title_bg" align="left">許可ブランドID</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(((string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS] == "") ? "全て" : m_htParam[Constants.FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS])%></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">SEOキーワード</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_SEO_KEYWORDS])%></td>
													</tr>
													<%if (Constants.FRIENDLY_URL_ENABLED){ %>
													<tr>
														<td class="edit_title_bg" align="left">カノニカルタグ用テキスト</td>
														<td class="edit_item_bg" align="left"><%#: m_htParam[Constants.FIELD_PRODUCTCATEGORY_CANONICAL_TEXT]%></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">カテゴリページURL</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_URL])%><br />
															<span id="spanCategoryPreviewConfirm" runat="server" visible='<%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_URL]) != "" %>'>
																<a href="javascript:open_window('<%# CreatePreviewUrl(WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTCATEGORY_URL]))%>','preview_productcategory','');">プレビュー</a>
															</span>
														</td>
													</tr>
													<% if (Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg){ %>
													<tr>
														<td class="edit_title_bg" align="left">外部レコメンド利用フラグ</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_USE_RECOMMEND_FLG, (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_USE_RECOMMEND_FLG]))%></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">有効フラグ</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_VALID_FLG, (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_VALID_FLG])) %></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">会員ランク表示制御</td>
													</tr>
													<%-- 会員ランクオプションが有効の場合--%>
													<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
													<%if (this.IsParentCategory == false) { %>
													<tr>
														<td class="edit_title_bg" align="left">会員ランク表示制御</td>
														<td class="edit_item_bg" align="left">
															閲覧可能会員ランク：
															<%# WebSanitizer.HtmlEncode(GetMemberRankName((string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID]))%><br />
															<br />
															閲覧可能ランク未満の場合<br />&nbsp;&nbsp;カテゴリツリーに
															<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG, (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG])) %>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">定期会員限定フラグ</td>
														<td class="edit_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG, (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG])) %>
														</td>
													</tr>
													<%} else if (this.IsParentCategory){ %>
														<tr>
															<td class="edit_title_bg" align="left">会員ランク表示制御</td>
															<td class="edit_item_bg" align="left">
																親カテゴリに準ずる
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">定期会員限定フラグ</td>
															<td class="edit_item_bg" align="left">
																親カテゴリに準ずる
															</td>
														</tr>
													<% } %>
													<% } %>
												</table>
												<div class="action_part_bottom">
													<asp:Button id="btnBackUpdateBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" CssClass="cmn-btn-sub-action" />
													<asp:Button id="btnCategoryRegisterBottom" runat="server" Text="  登録する  " OnClick="btnCategoryRegister_Click" CssClass="cmn-btn-sub-action" /><br />
												</div>
											</td>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
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
<!--△ 一覧・詳細 △-->
</ContentTemplate>
</asp:UpdatePanel>
<table class="info_table" cellspacing="1" cellpadding="3" width="784" border="0">
	<tr class="info_item_bg">
		<td align="left">
			備考<br />
			・「SEOキーワード」<br />
			　	設定したSEOキーワードは、サイト管理＞SEO設定の「@@ seo_keywords @@」タグで出力されます。（","カンマ区切りで複数指定可。）<br />
			・「カテゴリページURL」<br />
			　	例1：PCサイトのルート以下（「<%= Constants.URL_FRONT_PC %>」以下）を入力します。<br />
			　	  　　「<%= Constants.URL_FRONT_PC %>Contents/Category01.aspx」を参照する場合、「Contents/Category01.aspx」と入力します。<br />
			　	例2：URLをそのまま入力します。（「<%= Constants.URL_FRONT_PC %>Category01.html」の指定など）<br />
		</td>
	</tr>
</table>
<script type="text/javascript">
<!--
	// プレビュー画面表示（編集画面用）
	function open_preview_window_modify(url) 
	{
		if (url != "") 
		{
			var openurl = null;
			strurl = new String(url);

			if ((strurl.toLowerCase().indexOf('<%= Constants.PROTOCOL_HTTP %>') == 0)
				|| strurl.toLowerCase().indexOf('<%= Constants.PROTOCOL_HTTPS %>') == 0) 
			{
				openurl = url;
			}
			else 
			{
				openurl = '<%= Constants.URL_FRONT_PC %>' + url;
			}

			open_window(openurl, 'preview_productcategory', '');
		}
	}
-->
</script>
</asp:Content>