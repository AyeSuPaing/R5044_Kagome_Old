<%--
=========================================================================================================
  Module      : 会員ランク設定一覧ページ(MemberRankList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MemberRankList.aspx.cs" Inherits="Form_MemberRank_MemberRankList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">会員ランク設定</h1></td>
	</tr>
	
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">会員ランク設定一覧</h2></td>
	</tr>
	<tr>
		<td>
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
											<td align="left">
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td><asp:label id="lbPager1" Runat="server"></asp:label></td>
														<td class="action_list_sp" style="height: 22px">
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_OnClick">翻訳データ出力</asp:LinkButton>
															<% } %>
															<asp:Button ID="btnDefaultUpdateTop" runat="server" Text="  デフォルト設定更新  " onclick="btnDefaultUpdate_Click"/>
															<asp:Button ID="btnInsertTop" runat="server" Text="  新規登録  " onclick="btnInsert_Click"/>
														</td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="68" rowspan="2">デフォルト<br />設定選択</td>
														<td align="center" width="70" rowspan="2">ランク順位</td>
														<td align="center" width="80">ランクID</td>
														<td align="center" width="125" rowspan="2">注文割引指定</td>
														<td align="center" width="110" rowspan="2">注文金額<br />割引き閾値</td>
														<td align="center" width="110" rowspan="2">ポイント加算指定</td>
														<td align="center" width="125" rowspan="2">配送料割引指定</td>
														<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
														<td align="center" width="125" rowspan="2">定期会員割引率</td>
														<% } %>
														<td align="center" width="70" rowspan="2">有効フラグ</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center">ランク名</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" id="trItem<%# Container.ItemIndex %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)">
																<td align="center">
																	<input type="radio" name="rbDefaultRank" value='<%# Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID) %>' <%# ((string)Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID) == this.DefaultRankId) ? "checked=\"checked\"" : "" %> />
																</td>
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMemberRankDetailUrl((String)Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID))) %>')">
																	<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ORDER))%>
																</td>
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMemberRankDetailUrl((String)Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID))) %>')">
																	<table class="list_table_childtable" cellpadding="0" cellspacing="0" width="100%" height="100%">
																		<tr>
																			<td align="left" class="item_bottom_line"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID))%></td>
																		</tr>
																		<tr>
																			<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME))%></td>
																		</tr>
																	</table>
																</td>
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMemberRankDetailUrl((String)Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID))) %>')">
																	<%#: GetOrderDiscountType(
																		Eval(Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_VALUE).ToPriceDecimal(),
																		(string)Eval(Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE)) %>
																</td>
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMemberRankDetailUrl((String)Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID))) %>')">
																	<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_THRESHOLD_PRICE).ToPriceString(true))%>
																	<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MEMBERRANK, Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_THRESHOLD_PRICE, Eval(Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_THRESHOLD_PRICE)))%>
																</td>
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMemberRankDetailUrl((String)Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID))) %>')">
																	<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MEMBERRANK_POINT_ADD_VALUE))%>
																	<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MEMBERRANK, Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE, Eval(Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE)))%>
																</td>
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMemberRankDetailUrl((String)Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID))) %>')">
																	<%#: GetShippingDiscountType(
																		Eval(Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_VALUE).ToPriceDecimal(),
																		(string)Eval(Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE)) %>
																</td>
																<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMemberRankDetailUrl((String)Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID))) %>')">
																	<%# WebSanitizer.HtmlEncode(string.Format("{0}%", Eval(Constants.FIELD_MEMBERRANK_FIXED_PURCHASE_DISCOUNT_RATE)))%>
																</td>
																<% } %>
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMemberRankDetailUrl((String)Eval(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID))) %>')">
																	<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MEMBERRANK, Constants.FIELD_MEMBERRANK_VALID_FLG, Eval(Constants.FIELD_MEMBERRANK_VALID_FLG)))%>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" colspan="9" runat="server">
														</td>
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
															・「デフォルト選択設定」で選択したランクが、ユーザーが新規会員登録を行った際にデフォルトで設定されるランクとなります。<br />
															・「ランク順位」は、番号が若いほど上位のランクとなります。
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td align="left">
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td class="action_list_sp" style="height: 22px">
															<asp:Button ID="btnDefaultUpdateBottom" runat="server" Text="  デフォルト設定更新  " onclick="btnDefaultUpdate_Click"/>
															<asp:Button ID="btnInsertBottom" runat="server" Text="  新規登録  " onclick="btnInsert_Click"/>
														</td>
													</tr>
												</table>
												<!-- ページング-->
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
