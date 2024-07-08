<%--
=========================================================================================================
  Module      : セール設定登録ページ(ProductSaleRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductSaleRegister.aspx.cs" Inherits="Form_ProductSale_ProductSaleRegister" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">セール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trEditTitle" runat="server">
		<td><h2 class="cmn-hed-h2">セール設定編集</h2></td>
	</tr>
	<tr id="trRegistTitle" runat="server">
		<td><h2 class="cmn-hed-h2">セール設定登録</h2></td>
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
															<td align="left">セール設定を登録/更新しました。
															</td>
														</tr>
													</table>
												</div>
												<div class="action_part_top">
													<asp:Button ID="btnToListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('セール設定を削除します。よろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" OnClientClick="return confirm('表示内容で更新します。よろしいですか？');" />
												</div>

												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">セール情報</td>
													</tr>
													<tr id="trDispProductSaleId" runat="server">
														<td class="detail_title_bg" align="left" width="25%">商品セールID</td>
														<td class="detail_item_bg" align="left">
															<asp:Literal ID="lProductSaleId" runat="server"></asp:Literal></td>
													</tr>
													<tr id="trSelectProductSaleKbn" runat="server">
														<td class="detail_title_bg" align="left">商品セール区分 <span class="notice">*</span></td>
														<td class="detail_item_bg" align="left">
															<asp:RadioButtonList ID="rblProductSaleKbn" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblProductSaleKbn_SelectedIndexChanged"></asp:RadioButtonList></td>
													</tr>
													<tr id="trDispProductSaleKbn" runat="server">
														<td class="detail_title_bg" align="left">商品セール区分</td>
														<td class="detail_item_bg" align="left">
															<asp:Literal ID="lProductSaleKbn" runat="server"></asp:Literal>
															<asp:HiddenField ID="hfProductSaleKbn" runat="server" /></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">商品セール名 <span class="notice">*</span></td>
														<td class="detail_item_bg" align="left">
															<asp:TextBox ID="tbProductSaleName" runat="server" MaxLength="30" Width="300"></asp:TextBox></td>
													</tr>
													<tr id="trClosedMarketPassword" runat="server">
														<td class="detail_title_bg" align="left">闇市パスワード <span class="notice">*</span></td>
														<td class="detail_item_bg" align="left">
															<asp:TextBox ID="tbClosedMarketPassword" runat="server" MaxLength="30" Width="150"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">開始日時 - 終了日時 <span class="notice">*</span></td>
														<td class="detail_item_bg" align="left">
															<uc:DateTimePickerPeriodInput id="ucDisplayPeriod" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">有効フラグ</td>
														<td class="detail_item_bg" align="left">
															<asp:CheckBox ID="cbValidFlg" runat="server" Text="有効" /></td>
													</tr>
													<tbody id="tbdyDispUpdateInfo" runat="server">
														<tr>
															<td class="detail_title_bg" align="left">開催状態</td>
															<td class="detail_item_bg" align="left">
																<strong id="srgSaleOpened" runat="server">
																	<asp:Literal ID="lSaleOpened" runat="server"></asp:Literal>
																</strong></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">URL</td>
															<td class="detail_item_bg url_link" align="left">
																<a href="<%= this.Url %>" target="_blank"><%= this.Url %></a></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">作成日</td>
															<td class="detail_item_bg" align="left">
																<asp:Literal ID="lDateCreated" runat="server"></asp:Literal></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">更新日</td>
															<td class="detail_item_bg" align="left">
																<asp:Literal ID="lDateChanged" runat="server"></asp:Literal></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">最終更新者</td>
															<td class="detail_item_bg" align="left">
																<asp:Literal ID="lLastChanged" runat="server"></asp:Literal></td>
														</tr>
													</tbody>
												</table>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												<% if (this.ShowsCSVDownloadLink){ %>
													<%-- マスタ出力 --%>
													<table>
														<tr>
															<td class="search_table">
																<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="ProductSalePrice" TableWidth="758" />
															</td>
														</tr>
													</table>
													<img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													<table>
														<tr id="trMasterUploadMessage" runat="server" Visible="false">
															<td align="center">
																<span class="info">
																	<a target="_blank" href="<%=this.MasterUploadUrl %>">登録商品が<%: Constants.PRODUCTSALE_REGISTER_UPPER_LIMIT %>件以上の場合は、マスタアップロードをお使いください。</a><br />
																</span>
															</td>
														</tr>
													</table>
													<img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												<% } %>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr ID="trProductSalePrice" runat="server">
														<td class="edit_title_bg" align="center" colspan="7">商品セール価格</td>
													</tr>
													<tr ID="trSaleDiscountRateInput" runat="server">
															<td class="edit_item_bg" align="right" colspan="7">
																商品価格（<%: this.ProductPriceTextPrefix %>）の<asp:TextBox id="tbBatchConvertPercentage" Width="32" height="32" MaxLength="3" runat="server" />%に設定&nbsp;
																<input type="button" value="  一括計算  " onclick="batchconvert_salesprice();" />
															</td>
														</tr>
													<tr ID="trProductSalePriceHead" runat="server">
														<td class="edit_title_bg" align="center" width="40">削除</td>
														<td class="edit_title_bg" align="center" width="118">商品ID</td>
														<td class="edit_title_bg" align="center" width="80">バリエーションID</td>
														<td class="edit_title_bg" align="left" width="300">商品名</td>
														<td class="edit_title_bg" align="center" width="60">商品価格（<%: this.ProductPriceTextPrefix %>）<br />(特別価格)</td>
														<td class="edit_title_bg" align="center" width="120">セール価格（<%: this.ProductPriceTextPrefix %>）<span class="notice">*</span></td>
														<td class="edit_title_bg" align="center" width="50">並び順<span class="notice">*</span></td>
													</tr>
													<asp:Repeater ID="rProductList" runat="server">
														<ItemTemplate>
														<tr>
															<td class="edit_item_bg" align="center"><asp:CheckBox ID="cbProductSelect" runat="server" /></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(((ProductSalePrice)Container.DataItem).ProductId)%></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(((ProductSalePrice)Container.DataItem).VId)%></td>
															<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(((ProductSalePrice)Container.DataItem).Name)%></td>
															<td class="edit_item_bg" align="right">
																<%# WebSanitizer.HtmlEncode(((ProductSalePrice)Container.DataItem).Price.ToPriceString(true))%>
																<asp:HiddenField ID="hfPrice" runat="server" Value='<%# ((ProductSalePrice)Container.DataItem).Price %>' /><br />
																<%# WebSanitizer.HtmlEncode(((ProductSalePrice)Container.DataItem).SpecialPrice == "" ? "" : "(" + ((ProductSalePrice)Container.DataItem).SpecialPrice.ToPriceString(true) + ")")%></td>
															<td class="edit_item_bg" align="center"><asp:TextBox ID="tbProductSalePrice" Width="50" MaxLength="7" runat="server"></asp:TextBox>
																<input type="button" value="  計算  " onclick="convert_salesprice(document.getElementById('<%# Container.FindControl("tbProductSalePrice").ClientID %>'), document.getElementById('<%# Container.FindControl("hfPrice").ClientID %>').value);" /></td>
															<td class="edit_item_bg" align="center"><asp:TextBox ID="tbDisplayOrder" Width="40" MaxLength="3" runat="server"></asp:TextBox></td>
														</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="7"></td>
													</tr>
												</table>
												<div class="action_part_middle" ID="dProductButton" runat="server">
													<input id="inputSearchProduct" type="button" value="  商品選択  " onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID + "&" + Constants.REQUEST_KEY_PRODUCT_SEARCH_CALLER + "=" + Constants.REQUEST_KEY_NAME_PRODUCT_SALE) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes');" />
													<asp:Button ID="btnDelete" Text="  商品削除  " runat="server" OnClick="btnDeleteItem_Click"  OnClientClick="return confirm('選択された商品は商品セール対象リストから削除します。よろしいですか？');"/>
												</div>

												<div class="action_part_bottom">
													<asp:Button ID="btnToListBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('セール設定を削除します。よろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" OnClientClick="return confirm('表示内容で更新します。よろしいですか？');" />
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
															・PCサイトの商品セール一覧画面では、商品マスタの「商品表示区分」に関わらず商品が表示されます。<br/>
															・対象商品の定期購入フラグを「定期のみ」とした場合、セールは適用されません。<br/>
															<% if (this.ShowsCSVDownloadLink){ %>
															・更新ボタンをクリックするまで「商品セール価格マスタ出力」からダウンロードするCSVには反映されません。<br/>
															<% } %>
															<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED){ %>
															・<span class="notice">セール価格は商品単価に対する割引後の金額のため、付帯価格に対しては適用されません。</span>
															<% } %>
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
<asp:HiddenField ID="hfAddProductId" runat="server" />
<asp:HiddenField ID="hfAddVId" runat="server" />
<asp:HiddenField ID="hfAddProductName" runat="server" />
<asp:HiddenField ID="hfAddProductPrice" runat="server" />
<asp:HiddenField ID="hfAddProductSpecialPrice" runat="server" />
<asp:LinkButton id="lbAddProduct" runat="server" OnClick="lbAddProduct_Click"></asp:LinkButton>
<script type="text/javascript">
<!--
// 商品追加処理
function set_productinfo(product_id, supplier_id, v_id, name, display_price, special_price, price, sale_id, fixed_purchase_id)
{
	document.getElementById('<%= hfAddProductId.ClientID %>').value = product_id;
	document.getElementById('<%= hfAddVId.ClientID %>').value = v_id;
	document.getElementById('<%= hfAddProductName.ClientID %>').value = name;
	document.getElementById('<%= hfAddProductPrice.ClientID %>').value = display_price;
	document.getElementById('<%= hfAddProductSpecialPrice.ClientID %>').value = special_price;

	__doPostBack('<%= lbAddProduct.UniqueID %>','');
}
	// 基軸通貨桁数
	var digits = '<%: CurrencyManager.DigitsByKeyCurrency %>';
	// 基軸通貨フォーマット
	var formatter = new Intl.NumberFormat('<%: Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId %>', { minimumFractionDigits : digits, maximumFractionDigits: digits, useGrouping : false, currency : '<%: Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code %>'});

// セール価格一括入力
function batchconvert_salesprice()
{
	if (isNaN( parseFloat(document.getElementById('<%= tbBatchConvertPercentage.ClientID %>').value) ) == false)
	{
<%foreach (RepeaterItem ri in rProductList.Items) {%>
	document.getElementById('<%= ((TextBox)ri.FindControl("tbProductSalePrice")).ClientID %>').value
	 = formatter.format(parseFloat(document.getElementById('<%= tbBatchConvertPercentage.ClientID %>').value) * document.getElementById('<%= ((HiddenField)ri.FindControl("hfPrice")).ClientID %>').value / 100);
<%} %>
	}
	else
	{
		alert("商品価格の割合（％）を正しく設定してください。");
	}
}
// セール価格入力
function convert_salesprice(tbTarget, price)
{
	if (isNaN( parseFloat(document.getElementById('<%= tbBatchConvertPercentage.ClientID %>').value) ) == false)
	{
		tbTarget.value = formatter.format(parseFloat(document.getElementById('<%= tbBatchConvertPercentage.ClientID %>').value) * price / 100);
	}
	else
	{
		alert("商品価格の割合（％）を正しく設定してください。");
	}
}
//-->
</script>
</asp:Content>

