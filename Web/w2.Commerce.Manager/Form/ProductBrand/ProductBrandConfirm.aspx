<%--
=========================================================================================================
  Module      : ブランド情報確認ページ(ProductBrandConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductBrandConfirm.aspx.cs" Inherits="Form_ProductBrand_ProductBrandConfirm" %>

<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
<meta http-equiv="Pragma" content="no-cache">
<meta http-equiv="cache-control" content="no-cache">
<meta http-equiv="expires" content="0">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr id="trTitleProductTop" runat="server">
	</tr>
	<tr id="trTitleProductMiddle" runat="server">
		<td><h1 class="page-title">商品ブランド設定</h1></td>
	</tr>
	<tr id="trTitleProductBottom" runat="server">
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">商品ブランド設定詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">商品ブランド設定入力確認</h2></td>
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
												<div class="action_part_top">
													<asp:Button id="btnBackTop" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button ID="btnBackListTop" runat="server" Text="  一覧へ戻る  " Visible="False" OnClick="btnBack_Click" />
													<asp:Button id="btnEditTop" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" OnClick="btnInsertUpdate_Click" />
												</div>
												
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr id="trDispBrandId" runat="server">
														<td class="detail_title_bg" align="left" width="25%">ブランドID</td>
														<td class="detail_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_BRAND_ID))%>
														</td>
													</tr>
													<tr id="trBrandName" runat="server">
														<td class="detail_title_bg" align="left" width="250">ブランド名</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_BRAND_NAME))%></td>
													</tr>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<%-- ブランド名翻訳設定情報 --%>
													<asp:Repeater ID="rTranslationBrandName" runat="server"
														 DataSource="<%# this.ProductBrandTranslationData %>"
														 ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
													<ItemTemplate>
														<tr>
															<td class="detail_title_bg" align="left" width="250">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
															<td class="detail_item_bg" align="left"><%#: Item.AfterTranslationalName %></td>
														</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<tr id="trBrandTitle" runat="server">
														<td class="detail_title_bg" align="left" width="250">ブランドタイトル</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_BRAND_TITLE))%></td>
													</tr>
													<tr id="trSeoKeyword" runat="server">
														<td class="detail_title_bg" align="left" width="250">SEOキーワード</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_SEO_KEYWORD))%></td>
													</tr>
													<tr id="trMetInfo" runat="server">
														<td class="detail_title_bg" align="left" width="250">追加デザインタグ情報</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_ADDITIONAL_DESIGN_TAG))%></td>
													</tr>
													<tr id="trValidFlg" runat="server">
														<td class="detail_title_bg" align="left" width="250">有効フラグ</td>
														<td class="detail_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTBRAND, Constants.FIELD_PRODUCTBRAND_VALID_FLG, GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_VALID_FLG)))%>
														</td>
													</tr>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="250">作成日</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_DATE_CREATED))%></td>
													</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="250">更新日</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_DATE_CHANGED))%></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="250">最終更新者</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_LAST_CHANGED))%></td>
													</tr>
												</table>
												<br />

												<div class="action_part_bottom">
													<asp:Button id="btnBackBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button ID="btnBackListBottom" runat="server" Text="  一覧へ戻る  " Visible="False" OnClick="btnBack_Click" />
													<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" OnClick="btnInsertUpdate_Click" />
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
