<%--
=========================================================================================================
  Module      : 商品ブランド設定一覧ページ(ProductBrandList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductBrandList.aspx.cs" Inherits="Form_ProductBrand_ProductBrandList" %>
<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
<meta http-equiv="Pragma" content="no-cache">
<meta http-equiv="cache-control" content="no-cache">
<meta http-equiv="expires" content="0">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">商品ブランド設定</h1></td>
	</tr>
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">商品ブランド設定一覧</h2></td>
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
												<!-- ▽ページング▽ -->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="350" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td align="right">
															<table width="400">
																<tr>
																	<td style="text-align:right">
																		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																		<asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_OnClick">翻訳設定出力</asp:LinkButton>
																		<% } %>
																		<asp:Button ID="btnDefaultResetTop" runat="server" Text="  デフォルト設定をはずす  " onclick="btnDefaultReset_Click"/>
																		<asp:Button ID="btnDefaultUpdateTop" runat="server" Text="  デフォルト設定更新  " onclick="btnDefaultUpdate_Click"/>
																		<asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
												<!-- △ページング△ -->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="7%" style="height: 17px">デフォルト設定</td>
														<td align="center" width="15%" style="height: 17px">ブランドID</td>
														<td align="center" width="20%" style="height: 17px">ブランド名</td>
														<td align="center" width="50%" style="height: 17px">タイトル</td>
														<td align="center" width="8%" style="height: 17px">有効</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr id="trItem<%# Container.ItemIndex %>" class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" >
																<td align="center" onclick="">
																	<input type="radio" name="rbDefaultBrand" value='<%# Eval(Constants.FIELD_PRODUCTBRAND_BRAND_ID) %>' <%# ((string)Eval(Constants.FIELD_PRODUCTBRAND_BRAND_ID) == this.DefaultBrandId) ? "checked=\"checked\"" : "" %> /></td>
																<td align="center" onclick="listselect_mclick(document.getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductBrandDetailUrl((string)Eval(Constants.FIELD_PRODUCTBRAND_BRAND_ID))) %>')">
																	<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTBRAND_BRAND_ID)) %></td>
																<td align="center" onclick="listselect_mclick(document.getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductBrandDetailUrl((string)Eval(Constants.FIELD_PRODUCTBRAND_BRAND_ID))) %>')">
																	<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTBRAND_BRAND_NAME)) %></td>
																<td align="center" onclick="listselect_mclick(document.getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductBrandDetailUrl((string)Eval(Constants.FIELD_PRODUCTBRAND_BRAND_ID))) %>')">
																	<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTBRAND_BRAND_TITLE)) %></td>
																<td align="center" onclick="listselect_mclick(document.getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductBrandDetailUrl((string)Eval(Constants.FIELD_PRODUCTBRAND_BRAND_ID))) %>')">
																	<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTBRAND, Constants.FIELD_PRODUCTBRAND_VALID_FLG, Eval(Constants.FIELD_PRODUCTBRAND_VALID_FLG)))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="8"></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
														　ブランド設定の反映には５分程かかります。<br />
														　デフォルトブランドとは、ブランドを指定しない場合に遷移するブランドです。<br />
														　ブランドを有効にする前に、必ず商品にブランドを設定する必要があります。<br />
														　（ブランド未設定の商品が不整合データとして扱われます。）
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="550" style="height: 22px"></td>
														<td align="right">
															<table width="208">
																<tr>
																	<td style="text-align:right">
																		<asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
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
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ 一覧 △-->
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>