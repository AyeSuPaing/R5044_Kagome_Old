<%--
=========================================================================================================
  Module      : 商品セット設定確認ページ(ProductSetConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductSetConfirm.aspx.cs" Inherits="Form_ProductSet_ProductSetConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
	// HTML文書表示用フレームのウィンドウ幅調整
	(function (window, $) {
		$(window).load(function () {
			$('iframe.HtmlPreview').each(function () {
				var doc = $(this).get(0).contentWindow.document;
				var innerHeight = Math.max(
					doc.body.scrollHeight, doc.documentElement.scrollHeight,
					doc.body.offsetHeight, doc.documentElement.offsetHeight,
					doc.body.clientHeight, doc.documentElement.clientHeight);
				$(this).removeAttr("height").css('height', innerHeight + 'px');
			});
		});
	})(window, jQuery);
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">商品セット設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<tr>
		<td>
			<% if (m_strActionStatus == Constants.ACTION_STATUS_DETAIL){%>
				<h2 class="cmn-hed-h2">商品セット設定詳細</h2>
			<%} %>
			<% if ((m_strActionStatus == Constants.ACTION_STATUS_INSERT) || (m_strActionStatus == Constants.ACTION_STATUS_UPDATE)){%>
				<h2 class="cmn-hed-h2">商品セット設定確認</h2>
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
													<asp:Button id="btnEditTop" runat="server" Text="  編集する  " OnClick="btnEdit_Click"></asp:Button>
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="false" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('設定を削除します。よろしいですか？')"></asp:Button>
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " OnClick="btnInsert_Click"></asp:Button>
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " OnClick="btnUpdate_Click"></asp:Button>
												</div>
												<%-- ▽商品エラーメッセージ表示▽ --%>
												<table id="tblProductErrorMessages" class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server" visible="False">
													<tbody>
													<tr>
														<td class="edit_title_bg" align="center">エラーメッセージ</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left" style="border-bottom: none;">
															<asp:Label ID="lbProductErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													</tbody>
												</table>
												<%-- △商品エラーメッセージ表示△ --%>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trId" runat="server" visible="false">
														<td class="detail_title_bg" align="left">商品セットID</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lProductSetId" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="25%">商品セット名</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lProductSetName" runat="server"></asp:Label>
														</td>
													</tr>
													<%-- 商品セット名翻訳設定情報 --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater id="rTranslationProductSetName" runat="server"
														 DataSource="<%# this.ProductSetTranslationData.Where(d => d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTSET_PRODUCT_SET_NAME) %>"
														 ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
													<ItemTemplate>
													<tr>
														<td class="detail_title_bg" align="left" width="25%">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="detail_item_bg" align="left">
															<%# Item.AfterTranslationalName %>
														</td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<tr>
														<td class="detail_title_bg">親商品数(上下限)</td>
														<td class="detail_item_bg">
															<asp:Label ID="lParentMaxMin" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">子商品数(上下限)</td>
														<td class="detail_item_bg">
															<asp:Label ID="lChildMaxMin" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">表示用文言 HTML区分</td>
														<td class="detail_item_bg">
															<asp:Label ID="lDescriptionKbn" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">表示用文言</td>
														<td class="detail_item_bg">
															<div runat="server" visible="<%# (string)m_htProductSet[Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN] != Constants.FLG_PRODUCTSET_DESCRIPTION_KBN_HTML %>">
																<%# WebSanitizer.HtmlEncodeChangeToBr(m_htProductSet[Constants.FIELD_PRODUCTSET_DESCRIPTION]) %>
															</div>
															<div runat="server" visible="<%# (string)m_htProductSet[Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN] == Constants.FLG_PRODUCTSET_DESCRIPTION_KBN_HTML %>">
																<iframe class="HtmlPreview" src="<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HTML_PREVIEW_FORM).AddParam(Constants.HTML_PREVIEW_NO, "1").CreateUrl() %>" style="width: 100%; height: 100%;" frameborder="0" scrolling="no">
																</iframe>
															</div>
														</td>
													</tr>
													<%-- 表示用文言翻訳設定情報 --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater id="rTranslationDescription" runat="server"
														 DataSource="<%# this.ProductSetTranslationData.Where(d => d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTSET_DESCRIPTION) %>"
														 ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
													<ItemTemplate>
													<tr>
														<td class="detail_title_bg" align="left" width="25%" rowspan="2">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="detail_item_bg" align="left">
															<%# ValueText.GetValueText(Constants.TABLE_PRODUCTSET, Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN, Item.DisplayKbn) %>
														</td>
													</tr>
													<tr>
														<td class="detail_item_bg" align="left">
															<%# Item.AfterTranslationalName %>
														</td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<tr>
														<td class="detail_title_bg">1注文購入限度数</td>
														<td class="detail_item_bg">
															<asp:Label ID="lMaxSellQuantity" runat="server"></asp:Label>
														</td>
													</tr>
												
													<tr>
														<td class="detail_title_bg" align="left">有効フラグ</td>
														<td class="detail_item_bg" align="left">
															<b>
																<asp:Label ID="lValidFlg" runat="server"></asp:Label>
															</b>
														</td>
													</tr>
													
													<tr id="trUrl" runat="server" visible="false">
														<td class="detail_title_bg" align="left">URL</td>
														<td class="detail_item_bg" align="left">
															<table>
																<tr><td>PC</td></tr>
																<tr><td class="url_link_border"><a href="<%= this.PcUrl %>" target="_blank"><%= this.PcUrl %></a></td></tr>
															</table>
														</td>
													</tr>
													
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" width="70">商品ID</td>
														<td class="edit_title_bg" align="center" width="70">バリエーションID</td>
														<td class="edit_title_bg" align="left" width="268">商品名</td>
														<td class="edit_title_bg" align="center" width="80">商品価格（<%: this.ProductPriceTextPrefix %>）</td>
														<td class="edit_title_bg" align="center" width="80">セット<br />商品価格（<%: this.ProductPriceTextPrefix %>）</td>
														<td class="edit_title_bg" align="center" width="70">個数制限</td>
														<td class="edit_title_bg" align="center" width="70">親子フラグ</td>
														<td class="edit_title_bg" align="center" width="50">表示順</td>
													</tr>
													<asp:Repeater ID="rProductList" runat="server">
														<ItemTemplate>
														<tr>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).ProductId)%></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).VId)%></td>
															<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).Name)%></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).Price.ToPriceString(true))%></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).SetItemPrice.ToPriceString(true))%></td>
															<td class="edit_item_bg" align="center"><%# CreateMinMaxString(((ProductSetItem)Container.DataItem).CountMin, ((ProductSetItem)Container.DataItem).CountMax)%></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTSETITEM, Constants.FIELD_PRODUCTSETITEM_FAMILY_FLG, ((ProductSetItem)Container.DataItem).FamilyFlg)) %></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(((ProductSetItem)Container.DataItem).DisplayOrder)%></td>
														</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="8"></td>
													</tr>
												</table>
												
												<div class="action_part_bottom">
													<input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " OnClick="btnEdit_Click"></asp:Button>
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="false" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('設定を削除します。よろしいですか？')"></asp:Button>
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " OnClick="btnInsert_Click"></asp:Button>
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " OnClick="btnUpdate_Click"></asp:Button>
												</div>
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
</asp:Content>

