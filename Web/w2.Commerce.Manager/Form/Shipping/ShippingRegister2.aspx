<%--
=========================================================================================================
  Module      : 配送料情報登録ページ(ShippingRegister2.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.DeliveryCompany" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShippingRegister2.aspx.cs" Inherits="Form_Shipping_ShippingRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.Domain.ShopShipping" %>
<%@ Register TagPrefix="w2cm" Assembly="w2.App.Common" Namespace="w2.App.Common.Web.WebCustomControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
// 配送料全国一律更新時のエラーアラート
function alertMessage() {
	alert("送料一律は0以上の半角数値で入力して下さい。");
}
//-->
</script>
<asp:UpdatePanel runat="server">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" width="797" border="0">
	<tr>
		<td><h1 class="page-title">配送種別設定</h1></td>
	</tr>
	<tr>
		<td style="width: 797px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEditTop" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送種別設定編集【配送料金設定編集】</h2></td>
	</tr>
	<tr id="trRegisterTop" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送種別設定登録【配送料金設定登録】</h2></td>
	</tr>
	<tr>
		<td style="width: 797px; height: 10px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td style="width: 797px">
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
													<asp:Button ID="btnBackTop" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
												</div>
												<asp:Repeater runat="server" DataSource="<%# (ShippingDeliveryPostageModel[])m_param[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO] %>" ItemType="w2.Domain.ShopShipping.ShippingDeliveryPostageModel" visible="<%# (this.SelectedDeliveryCompanyCount > 1) %>">
													<ItemTemplate>
														<div><a href="#<%#: Item.DeliveryCompanyId %>"><%#: GetDeliveryCompanyName(Item.DeliveryCompanyId) %></a></div>
													</ItemTemplate>
												</asp:Repeater>
												<br />
												<table ID="tblDeliveryCompanyErrorMessages" class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server" Visible="false">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">下記配送サービスの更新でエラーが発生しています。</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="2">
																<asp:Label ID="lbDeliveryCompanyErrorMessages" runat="server" ForeColor="red"/>
															</td>
														</tr>
													</tbody>
												</table>
												<asp:Repeater ID="rShippingPostage" runat="server" DataSource="<%# (ShippingDeliveryPostageModel[])m_param[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO] %>"
													ItemType="w2.Domain.ShopShipping.ShippingDeliveryPostageModel" OnItemCommand="rShippingPostage_ItemCommand">
												<ItemTemplate>
													<asp:HiddenField ID="hfDeliveryCompanyId" runat="server" Value="<%# Item.DeliveryCompanyId %>"/>
													<div runat="server" visible="<%# (this.SelectedDeliveryCompanyCount > 1) %>">
														<span id="<%#: Item.DeliveryCompanyId %>"></span><span id="a_idx_<%#:Container.ItemIndex %>"></span>
														<br />
													</div>
													<span runat="server" visible="<%# (Container.ItemIndex != 0) %>"><img height="15" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></span>
													<h2 class="cmn-hed-h2">
														■配送サービス：<%#: GetDeliveryCompanyName(Item.DeliveryCompanyId) %>
														　<span runat="server" visible="<%# Container.ItemIndex < this.SelectedDeliveryCompanyCount - 1 %>"><a href='#a_idx_<%#Container.ItemIndex + 1 %>'>↓</a></span>
														　<span runat="server" visible="<%# Container.ItemIndex > 0 %>"><a href='#a_idx_<%#Container.ItemIndex - 1 %>'>↑</a></span>
													</h2>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tbody>
															<tr id="trShippingPostageErrorMessagesTitle" runat="server" visible="false">
																<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
															</tr>
															<tr id="trShippingPostageErrorMessages" runat="server" visible="false">
																<td class="edit_item_bg" align="left" colspan="2">
																	<asp:Label ID="lbShippingPostageErrorMessages" runat="server" ForeColor="red"/>
																</td>
															</tr>
															<tr>
																<td class="edit_title_bg" align="center" colspan="2">配送料情報</td>
															</tr>
															<tr>
																<td class="edit_item_bg" align="center" colspan="2">
																	配送料設定を行う設定項目の切り替えを行います。各設定項目は下に表示されます。
																</td>
															</tr>
															<tr>
																<td class="edit_title_bg" align="left" width="30%">配送料設定</td>
																<td class="edit_item_bg" align="left">
																	<asp:RadioButton ID="rbShippingKbn0" runat="server" Text="配送料無しの場合" GroupName="ShippingKbn" OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="True" /><br />
																	<asp:RadioButton ID="rbShippingKbn1" runat="server" Text="配送料が全国一律の場合" GroupName="ShippingKbn" OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="True" /><br />
																	<asp:RadioButton ID="rbShippingKbn2" runat="server" Text="配送料を地域別に設定する場合" GroupName="ShippingKbn" OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="True" /><br />
																</td>
															</tr>
														</tbody>
													</table>
													<div id="dvShippingFreePrice" style="display: inline" runat="server">
														<br />
														<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tbody>
															<tr>
																	<td class="edit_title_bg" align="center" colspan="2">配送料無料購入金額情報（<span class="edit_btn_sub"><a href="#note">備考</a></span>）</td>
																</tr>
																<tr>
																	<td class="edit_item_bg" align="center" colspan="2">購入金額による配送料無料に関する設定です。特別配送先の場合は適用されません。</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="30%">無料購入金額設定の利用の有無</td>
																	<td class="edit_item_bg" align="left">
																		<asp:CheckBox id="cbShippingFreePriceFlg" Checked="<%# Item.ShippingFreePriceFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID %>"
																			OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="true" Runat="server"/></td>
																</tr>
																<tr id="trShippingFreePrice" runat="server">
																	<td class="edit_title_bg" align="left" width="30%">無料購入金額設定</td>
																	<td class="edit_item_bg" align="left">
																		１配送先の購入金額<%= (Constants.MANAGEMENT_INCLUDED_TAX_FLAG) ? "（税込総額）" : "（税抜総額）" %>が<asp:TextBox id="tbShippingFreePrice" runat="server" Text="<%# Item.ShippingFreePrice.ToPriceString() %>" Width="50" MaxLength="7"/>
																		<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %>以上の場合は送料を無料にする（特別配送先を除く）。
																	</td>
																</tr>
																<tr id="trAnnounceFreeShippingFlg" runat="server">
																	<td class="edit_title_bg" align="left" width="30%">配送料無料案内表示の有無</td>
																	<td class="edit_item_bg" align="left">
																		<asp:CheckBox id="cbAnnounceShippingFreeFlg" Checked="<%# Item.AnnounceFreeShippingFlg == Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_VALID %>" runat="server" />
																	</td>
																</tr>
																<% if (Constants.FREE_SHIPPING_FEE_OPTION_ENABLED) { %>
																<tr id="trUseFreeShippingFee" runat="server">
																	<td class="edit_title_bg" align="left" width="30%">配送料無料時の請求料金設定</td>
																	<td class="edit_item_bg" align="left">
																		送料無料時に
																		<asp:TextBox
																			id="tbUseFreeShippingFee"
																			runat="server"
																			Width="50"
																			Text='<%# string.IsNullOrEmpty(Item.FreeShippingFee.ToPriceString()) ? "0" : Item.FreeShippingFee.ToPriceString() %>'
																			MaxLength="7" />
																		<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : string.Empty %>の送料を請求する(特別配送先を除く)
																		<br />※商品情報の「配送料無料適用外」 が有効の場合に適用されます。
																	</td>
																</tr>
																<% } %>
															</tbody>
														</table>
													</div>
													<% if (Constants.STORE_PICKUP_OPTION_ENABLED) { %>
													<div id="dvStorePickupFreePriceFlg" style="display: inline" runat="server">
														<br />
														<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tbody>
																<tr>
																	<td class="edit_title_bg" align="center" colspan="2">
																		店舗受取時配送料（<span class="edit_btn_sub"><a href="#note">備考</a></span>）
																	</td>
																</tr>
																<tr>
																	<td class="edit_item_bg" align="center" colspan="2">店舗受取時の配送料を無料にするかを設定します。</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="30%">店舗受取時配送料無料設定</td>
																	<td class="edit_item_bg" align="left">
																		<asp:CheckBox
																			id="cbStorePickupFreePriceFlg"
																			Runat="server"
																			Checked="<%# Item.StorePickupFreePriceFlg == Constants.FLG_ON %>" />
																	</td>
																</tr>
															</tbody>
														</table>
													</div>
													<% } %>
													<div id="dvCalculationPlural" style="display: inline" runat="server">
														<br />
														<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tbody>
															<tr>
																	<td class="edit_title_bg" align="center" colspan="2">複数商品計算方法情報（<span class="edit_btn_sub"><a href="#note">備考</a></span>）</td>
																</tr>
																<tr>
																	<td class="edit_item_bg" align="center" colspan="2">複数商品購入時による複数商品計算方法を設定します。未設定の場合は商品１個ずつに送料がかかります。</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="30%">複数商品計算方法の利用の有無</td>
																	<td class="edit_item_bg" align="left">
																		<asp:CheckBox id="cbCalculationPluralKbn" Checked="<%# Item.CalculationPluralKbn == Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_HIGHEST_SHIPPING_PRICE_PLUS_PLURAL_PRICE %>"
																			OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="true" Runat="server"/>
																	</td>
																</tr>
																<tr id="trCalculationPluralKbn" runat="server">
																	<td class="edit_title_bg" align="left" width="30%">複数商品計算方法</td>
																	<td class="edit_item_bg" align="left">
																		最も<% if (Constants.SHIPPINGPRIORITY_SETTING == Constants.ShippingPriority.HIGH) { %>高い<%} else {%>低い<%}%>送料１点＋
																		<asp:TextBox id="tbPluralShippingPrice" runat="server" Text="<%# Item.PluralShippingPrice.ToPriceString() %>" Width="50" MaxLength="7"/>
																		<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %>／個の送料を設定する。</td>
																</tr>
															</tbody>
														</table>
													</div>
													<div id="dvShippingZone" style="display:inline" runat="server">
														<br />
														<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tbody>
																<tr>
																	<td class="edit_title_bg" align="center" colspan="10">各エリア配送料情報</td>
																</tr>
																<tr>
																	<td class="edit_item_bg" align="center" colspan="10">各エリア･商品サイズ/重量毎の配送料を設定します。特別配送先の追加は『配送先情報』から行います。</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left">配送料一律設定</td>
																	<td class="edit_item_bg" align="left" colspan="10">一律
																		<asp:TextBox id="tbAllShippingPrice" runat="server" Width="50" MaxLength="7"/>
																		<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %>（特別配送先は除く）
																		<asp:Button ID="btnChangePrice" runat="server" CommandName="ChangePrice" Text ="  更新する  "/>
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">エリア／サイズ重量区分</td>
																	<td class="edit_title_bg" align="center" width="8%">メール</td>
																	<td class="edit_title_bg" align="center" width="8%">XXS</td>
																	<td class="edit_title_bg" align="center" width="8%">XS</td>
																	<td class="edit_title_bg" align="center" width="8%">S</td>
																	<td class="edit_title_bg" align="center" width="8%">M</td>
																	<td class="edit_title_bg" align="center" width="8%">L</td>
																	<td class="edit_title_bg" align="center" width="8%">XL</td>
																	<td class="edit_title_bg" align="center" width="8%">XXL</td>
																	<td class="edit_title_bg" align="center" width="16%">配送料条件（サイズ量に関わらず一律設定）</td>
																</tr>
																<tr id="trZoneAll" runat="server">
																	<td class="edit_title_bg" align="left" width="20%">全国一律</td>
																	<td class="edit_item_bg" align="left" width="8%"><asp:TextBox id="tbSizeMailShippingPriceAll" runat="server" Width="60" MaxLength="7"></asp:TextBox></td>
																	<td class="edit_item_bg" align="left" width="8%"><asp:TextBox id="tbSizeXxsShippingPriceAll" runat="server" Width="60" MaxLength="7"></asp:TextBox></td>
																	<td class="edit_item_bg" align="left" width="8%"><asp:TextBox id="tbSizeXsShippingPriceAll" runat="server" Width="60" MaxLength="7"></asp:TextBox></td>
																	<td class="edit_item_bg" align="left" width="8%"><asp:TextBox id="tbSizeSShippingPriceAll" runat="server" Width="60" MaxLength="7"></asp:TextBox></td>
																	<td class="edit_item_bg" align="left" width="8%"><asp:TextBox id="tbSizeMShippingPriceAll" runat="server" Width="60" MaxLength="7"></asp:TextBox></td>
																	<td class="edit_item_bg" align="left" width="8%"><asp:TextBox id="tbSizeLShippingPriceAll" runat="server" Width="60" MaxLength="7"></asp:TextBox></td>
																	<td class="edit_item_bg" align="left" width="8%"><asp:TextBox id="tbSizeXlShippingPriceAll" runat="server" Width="60" MaxLength="7"></asp:TextBox></td>
																	<td class="edit_item_bg" align="left" width="8%"><asp:TextBox id="tbSizeXxlShippingPriceAll" runat="server" Width="60" MaxLength="7"></asp:TextBox></td>
																	<td class="edit_item_bg" align="left" width="16%">
																		<asp:CheckBox runat="server" ID="chkConditionalShippingPriceFlgAll" Text="条件追加" Width="120" Enabled="False"/>
																	</td>
																</tr>
																<asp:Repeater id="rShippingZone" Runat="server" ItemType="w2.Domain.ShopShipping.ShopShippingZoneModel"
																		DataSource="<%# GetShippingZonesByDeliveryCompany((ShopShippingZoneModel[])m_param[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO], Item.DeliveryCompanyId) %>" OnItemDataBound="rShippingZoneOnItemDataBound">
																	<ItemTemplate>
																		<asp:HiddenField ID="hfShippingZoneNo" runat="server" Value="<%# Item.ShippingZoneNo %>"/>
																		<asp:HiddenField ID="hfShippingZoneName" runat="server" Value="<%#: Item.ShippingZoneName %>"/>
																		<tr id="trZone" runat="server">
																			<td class="edit_title_bg" align="left" width="20%"><%#: Item.ShippingZoneName %></td>
																			<td class="edit_item_bg" runat="server" align="center" colspan="9" Visible='<%# Item.IsUnavailableShippingAreaFlg %>'>配送不可</td>
																			<td class="edit_item_bg" align="left" width="8%" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'><asp:TextBox id="tbSizeMailShippingPrice" runat="server" Text='<%# Item.SizeMailShippingPrice.ToPriceString() %>' Width="60" MaxLength="7"/></td>
																			<td class="edit_item_bg" align="left" width="8%" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'><asp:TextBox id="tbSizeXxsShippingPrice" runat="server" Text='<%# Item.SizeXxsShippingPrice.ToPriceString() %>' Width="60" MaxLength="7"/></td>
																			<td class="edit_item_bg" align="left" width="8%" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'><asp:TextBox id="tbSizeXsShippingPrice" runat="server" Text='<%# Item.SizeXsShippingPrice.ToPriceString() %>' Width="60" MaxLength="7"/></td>
																			<td class="edit_item_bg" align="left" width="8%" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'><asp:TextBox id="tbSizeSShippingPrice" runat="server" Text='<%# Item.SizeSShippingPrice.ToPriceString() %>' Width="60" MaxLength="7"/></td>
																			<td class="edit_item_bg" align="left" width="8%" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'><asp:TextBox id="tbSizeMShippingPrice" runat="server" Text='<%# Item.SizeMShippingPrice.ToPriceString() %>' Width="60" MaxLength="7"/></td>
																			<td class="edit_item_bg" align="left" width="8%" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'><asp:TextBox id="tbSizeLShippingPrice" runat="server" Text='<%# Item.SizeLShippingPrice.ToPriceString() %>' Width="60" MaxLength="7"/></td>
																			<td class="edit_item_bg" align="left" width="8%" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'><asp:TextBox id="tbSizeXlShippingPrice" runat="server" Text='<%# Item.SizeXlShippingPrice.ToPriceString() %>' Width="60" MaxLength="7"/></td>
																			<td class="edit_item_bg" align="left" width="8%" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'><asp:TextBox id="tbSizeXxlShippingPrice" runat="server" Text='<%# Item.SizeXxlShippingPrice.ToPriceString() %>' Width="60" MaxLength="7"/></td>
																			<td class="edit_item_bg" align="left" width="16%" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'>
																				<asp:CheckBox runat="server" ID="chkConditionalShippingPriceFlg" Text="条件追加" Width="120" OnCheckedChanged="chkConditionalShippingPriceFlgOnCheckedChanged" AutoPostBack="True"/>
																				<asp:PlaceHolder runat="server" ID="plConditionalShippingPrice" Visible="False">
																					<asp:TextBox runat="server" ID="tbConditionalShippingPriceThreshold" Text="<%# Item.ConditionalShippingPriceThreshold == null ? string.Empty : Item.ConditionalShippingPriceThreshold.ToPriceString() %>" Width="120" MaxLength="7"></asp:TextBox>
																					<br/>円以上の購入で
																					<br/><asp:TextBox runat="server" ID="tbConditionalShippingPrice" Text="<%# Item.ConditionalShippingPrice == null ? string.Empty : Item.ConditionalShippingPrice.ToPriceString() %>" Width="120" MaxLength="7"></asp:TextBox>
																					<br/>円の配送料
																				</asp:PlaceHolder>
																			</td>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
															</tbody>
														</table>
													</div>
												</ItemTemplate>
												</asp:Repeater>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td id="note" align="left" class="info_item_bg" colspan="2">備考（各項目の説明）<br />
															■配送料無料購入金額情報<br />
																配送料無料となる対象金額は、ポイント・クーポンの利用有無問わず「商品の合計金額」となります。<br />
																<br />
															■複数商品計算方法情報<br />
																1.本設定を無効にした場合、購入した商品１点ずつに送料がかかります。<br />
																2.本設定を有効にした場合、購入した商品の中で最も<% if (Constants.SHIPPINGPRIORITY_SETTING == Constants.ShippingPriority.HIGH) { %>高い<%} else {%>低い<%}%>送料 + （その他商品数 * 設定値）が送料となります。<br />
																3.本設定を有効かつ、商品情報の「配送料複数個無料」を有効とした場合、商品情報の設定を優先します。<br />
																<br />
																＜例＞<br />
																商品A(送料300円)、商品B(送料500円)を２点、商品C(送料1000円)を購入したとき<br />
																<br />
																・「1.」の場合<br />
																⇒商品A(送料300円) + (商品B(送料500円) * 2) + 商品C(送料1000円) = 送料2300円<br />
																<br />
																・「2.」の場合で設定値を100円にした場合<br />
																<% if (Constants.SHIPPINGPRIORITY_SETTING == Constants.ShippingPriority.HIGH) { %>
																⇒商品C(送料1000円) + (3商品 * 設定値100円) = 送料1300円<br />
																<%} else {%>
																⇒商品A(送料300円) + (3商品 * 設定値100円) = 送料600円<br />
																<%}%>
																<br />
																・「3.」の場合で商品Bの「配送料複数個無料」を有効とした場合<br />
																⇒商品A(送料300円) + 商品B(送料500円) + 商品C(送料1000円) = 送料1800円<br />
																<br />
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<asp:Button ID="btnBackBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
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
	<!--△ 登録 △-->
	<tr>
		<td style="width: 797px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
