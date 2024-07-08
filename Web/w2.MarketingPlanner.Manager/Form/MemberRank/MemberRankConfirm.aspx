<%--
=========================================================================================================
  Module      : 会員ランク情報確認ページ(MemberRankConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MemberRankConfirm.aspx.cs" Inherits="Form_MemberRank_MemberRankConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr id="trTitleUserMiddle" runat="server">
		<td><h1 class="page-title">会員ランク設定</h1></td>
	</tr>
	<tr id="trTitleUserBottom" runat="server">
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">会員ランク設定詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">会員ランク設定確認</h2></td>
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
													<input id="btnHistoryBackTop" value="  戻る  " onclick="javascript:history.back();" type="button" runat="server" />
													<asp:Button ID="btnBackListTop" Text="  一覧へ戻る  " runat="server" Visible="False" OnClick="btnBack_Click" />
													<asp:Button ID="btnEditTop" Text="  編集する  " runat="server" Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnDeleteTop" Text="  削除する  " runat="server" Visible="False" onclick="btnDeleteTop_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？')" />
													<asp:Button ID="btnUpdateTop" Text="  更新する  " runat="server" Visible="False" OnClick="btnUpdate_Click" />
													<asp:Button ID="btnInsertTop" Text="  登録する  " runat="server" Visible="False" OnClick="btnInsert_Click" />
												</div>
												
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">ランクID</td>
														<td align="left" class="edit_item_bg">
															<asp:Literal ID="lRankId" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">ランク名</td>
														<td align="left" class="edit_item_bg">
															<asp:Literal id="lRankName" runat="server"></asp:Literal></td>
													</tr>											
													
													<%-- ランク名翻訳設定情報 --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater runat="server" id="rTranslationRankName"
														 ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel"
														 DataSource="<%# this.MemberRankTranslationData %>">
													<ItemTemplate>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">　言語コード:<%# Item.LanguageCode %> 言語ロケールID:<%# Item.LanguageLocaleId %></td>
														<td align="left" class="edit_item_bg"><%# Item.AfterTranslationalName %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>

													<tr>
														<td align="left" class="edit_title_bg" width="30%">ランク順位</td>
														<td align="left" class="edit_item_bg">
															<asp:Literal id="lRankOrder" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">注文割引指定</td>
														<td align="left" class="edit_item_bg">
															<asp:Literal id="lOrderDiscountType" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">注文金額割引き閾値</td>
														<td align="left" class="edit_item_bg">
															<asp:Literal ID="lOrderDiscountThresholdPrice" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">ポイント加算指定</td>
														<td align="left" class="edit_item_bg">
															<asp:Literal ID="lPointAddType" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">配送料割引指定</td>
														<td align="left" class="edit_item_bg">
															<asp:Literal ID="lShippingDiscountType" runat="server"></asp:Literal></td>
													</tr>
													<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">定期会員割引率</td>
														<td align="left" class="edit_item_bg">
															<asp:Literal ID="lFixedPurchaseDiscountRate" runat="server"></asp:Literal></td>
													</tr>
													<% } %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">ランクメモ</td>
														<td align="left" class="edit_item_bg">
															<asp:Literal ID="lRankMemo" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">有効フラグ</td>
														<td align="left" class="edit_item_bg">
															<asp:Literal ID="lValidFlg" runat="server"></asp:Literal></td>
													</tr>
												</table>

												<div class="action_part_bottom">
													<input id="btnHistoryBackBottom" value="  戻る  " onclick="javascript:history.back();" type="button" runat="server" />
													<asp:Button ID="btnBackListBottom" Text="  一覧へ戻る  " runat="server" Visible="False" OnClick="btnBack_Click" />
													<asp:Button ID="btnEditBottom" Text="  編集する  " runat="server" Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnDeleteBottom" Text="  削除する  " runat="server" Visible="False" onclick="btnDeleteTop_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？')" />
													<asp:Button ID="btnUpdateBottom" Text="  更新する  " runat="server" Visible="False" OnClick="btnUpdate_Click" />
													<asp:Button ID="btnInsertBottom" Text="  登録する  " runat="server" Visible="False" OnClick="btnInsert_Click" />
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
