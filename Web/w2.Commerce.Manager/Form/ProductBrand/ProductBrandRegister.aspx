<%--
=========================================================================================================
  Module      : 商品ブランド設定登録ページ(ProductBrandRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductBrandRegister.aspx.cs" Inherits="Form_ProductBrand_ProductBrandRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">商品ブランド設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trEdit" runat="server">
		<td><h2 class="cmn-hed-h2">商品ブランド設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server">
		<td><h2 class="cmn-hed-h2">商品ブランド設定登録</h2></td>
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
												<div class="action_part_top">
													<asp:Button id="btnBackTop" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
												</div>
												
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr id="trBrandIdEdit" runat="server" Visible="False">
														<td class="edit_title_bg" align="left" width="25%">ブランドID<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:Label id="lbBrandId" Text="<%# WebSanitizer.HtmlEncode(GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_BRAND_ID)) %>" runat="server"></asp:Label>
														</td>
													</tr>
													<tr id="trBrandIdRegister" runat="server" Visible="False">
														<td class="edit_title_bg" align="left" width="25%">ブランドID<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbBrandId" runat="server" Width="200" MaxLength="30" Text="<%# GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_BRAND_ID) %>"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">ブランド名</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbBrandName" runat="server" Width="480" MaxLength="100" Text="<%# GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_BRAND_NAME) %>"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">ブランドタイトル</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbBrandTitle" runat="server" Width="480" MaxLength="100" Text="<%# GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_BRAND_TITLE) %>"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">SEOキーワード</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbSeoKeyword" runat="server" Width="480" MaxLength="200" TextMode="MultiLine" Columns="80" Rows="3" Text="<%# GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_SEO_KEYWORD) %>"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">追加デザインタグ情報</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbMetaInfo" runat="server" Width="480" TextMode="MultiLine" Columns="80" Rows="6" Text="<%# GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_ADDITIONAL_DESIGN_TAG) %>"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">有効フラグ</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbValidFlg" runat="server" Checked="<%# (string)GetKeyValue(this.ProductBrandMaster, Constants.FIELD_PRODUCTBRAND_VALID_FLG) != Constants.FLG_PRODUCTBRAND_VALID_FLG_INVALID %>" Text="有効" /></td>
													</tr>
												</table>
												<br />

												<div class="action_part_bottom">
													<asp:Button id="btnBackBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>