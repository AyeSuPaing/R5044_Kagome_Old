<%--
=========================================================================================================
  Module      : 会員ランク情報登録ページ(MemberRankRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MemberRankRegister.aspx.cs" Inherits="Form_MemberRank_MemberRankRegister" MaintainScrollPositionOnPostback="true"%>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr id="trTitleUserMiddle" runat="server">
		<td><h1 class="page-title">会員ランク設定</h1></td>
	</tr>
	<tr id="trTitleUserBottom" runat="server">
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録編集 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">会員ランク設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">会員ランク設定登録</h2></td>
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
												<br />
												<div class="action_part_top">
													<input id="btnHistoryBackTop" value="  戻る  " onclick="javascript:history.back();" type="button" />
													<asp:Button ID="btnConfirmTop" Text="  確認する  " runat="server" onclick="btnConfirmTop_Click"/>
												</div>

												<table class="edit_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr id="trRankIdRegister" runat="server" visible="false">
														<td align="left" class="edit_title_bg" width="30%">ランクID<span class="notice">*</span></td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbRankId" runat="server" MaxLength="30"></asp:TextBox></td>
													</tr>
													<tr id="trRankIdEdit" runat="server" visible="false">
														<td align="left" class="edit_title_bg" width="30%">ランクID</td>
														<td align="left" class="edit_item_bg">
															<asp:Label ID="lRankId" runat="server"></asp:Label></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">ランク名<span class="notice">*</span></td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbRankName" runat="server" MaxLength="100"></asp:TextBox></td>
													</tr>											
													<tr>
														<td align="left" class="edit_title_bg" width="30%">ランク順位<span class="notice">*</span></td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbRankOrder" runat="server" MaxLength="3"></asp:TextBox></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">注文割引指定<span class="notice">*</span></td>
														<td align="left" class="edit_item_bg">
															<asp:RadioButton ID="rbOrderDiscountTypeNone" GroupName="OrderDiscountType" runat="server" Text=" 割引しない" /><br />
															<asp:RadioButton ID="rbOrderDiscountTypeRate" GroupName="OrderDiscountType" runat="server" Text=" 割引率" />&nbsp;&nbsp;&nbsp;
															<asp:TextBox ID="tbOrderDiscountRateValue" runat="server" MaxLength="3"></asp:TextBox>&nbsp;%<br />
															<asp:RadioButton ID="rbOrderDiscountTypeFixed" GroupName="OrderDiscountType" runat="server" Text=" 割引金額" />
															<asp:TextBox ID="tbOrderDiscountFixedValue" runat="server" MaxLength="5"></asp:TextBox>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %><br />
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">注文金額割引き閾値</td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbOrderDiscountThresholdPrice" runat="server" MaxLength="5"></asp:TextBox>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">ポイント加算指定<span class="notice">*</span></td>
														<td align="left" class="edit_item_bg">
															<asp:RadioButton ID="rbPointAddTypeNone" GroupName="PointAddType" runat="server" Text=" 加算しない" /><br />
															<asp:RadioButton ID="rbPointAddTypeRate" GroupName="PointAddType" runat="server" Text=" 加算率" />&nbsp;&nbsp;&nbsp;
															<asp:TextBox ID="tbPointAddRateValue" runat="server" MaxLength="3"></asp:TextBox>&nbsp;%<br />
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">配送料割引指定<span class="notice">*</span></td>
														<td align="left" class="edit_item_bg">
															<asp:RadioButton ID="rbShippingDiscountTypeNone" GroupName="ShippingDiscountType" runat="server" Text=" 割引しない" /><br />
															<asp:RadioButton ID="rbShippingDiscountTypeFixed" GroupName="ShippingDiscountType" runat="server" Text=" 割引金額" />
															<asp:TextBox ID="tbShippingDiscountFixedValue" runat="server" MaxLength="5"></asp:TextBox>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %><br />
															<asp:RadioButton ID="rbShippingDiscountTypeFree" GroupName="ShippingDiscountType" runat="server" Text=" 配送料無料" /><br />
															<asp:RadioButton ID="rbShippingDiscountTypeFreeShippingThreshold" GroupName="ShippingDiscountType" runat="server" />
															<asp:TextBox ID="tbShippingDiscountFreeShippingThresholdValue" runat="server" MaxLength="7"></asp:TextBox>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %>以上購入で配送料無料<br />
														</td>
													</tr>
													<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">定期会員割引率<span class="notice">*</span></td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbFixedPurchaseDiscountRate" runat="server" MaxLength="3"></asp:TextBox>&nbsp;%</td>
													</tr>
													<% } %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">ランクメモ</td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbRankMemo" runat="server" TextMode="MultiLine" Rows="8" Width="500"></asp:TextBox></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">有効フラグ</td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbValidFlg" runat="server" Text=" 有効" /></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考（各項目の説明）<br />
															■会員ランク設定<br />
															・注文割引指定 ・・・購入時に割引きされる金額の計算方法(割引しない/割引率/割引金額)を指定<br />
															・注文金額割引き閾値 ・・・購入時に「注文割引指定」を適用させるための最少購入金額(<%: this.ProductPriceTextPrefix %>)<br />
															・ポイント加算指定 ・・・購入時に加算されるポイントの計算方法(加算しない/加算率)を指定<br />
															・配送料割引指定 ・・・購入時に割引きされる配送料の計算方法(割引しない/割引金額/無料)を指定<br />
															
															<br />
															※1. 「注文金額割引き閾値」と比較する金額は、<b>会員ランク割引対象商品の合計<%= Constants.SETPROMOTION_OPTION_ENABLED ? "(セットプロモーション割引後の金額)" : "" %></b>となります。<br />
															※2. 「注文金額割引き閾値」は、注文割引指定が<b>割引金額</b>の際に設定可能となります。
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<input id="btnHistoryBackBotton" value="  戻る  " onclick="javascript:history.back();" type="button" />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " onclick="btnConfirmTop_Click"></asp:Button>
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
	<!--△ 登録編集 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
