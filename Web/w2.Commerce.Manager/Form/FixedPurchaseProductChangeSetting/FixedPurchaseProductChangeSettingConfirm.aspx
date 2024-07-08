<%--
=========================================================================================================
  Module      : 定期商品変更設定詳細/確認画面(FixedPurchaseProductChangeSettingConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Form/Common/DefaultPage.master" CodeFile="FixedPurchaseProductChangeSettingConfirm.aspx.cs" Inherits="Form_FixedPurchaseProductChangeSetting_FixedPurchaseProductChangeSettingConfirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
<table>
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">定期商品変更設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<tr>
		<td>
			<% if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL){%>
				<h2 class="cmn-hed-h2">定期商品変更設定詳細</h2>
			<%} %>
			<% if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT) || (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)){%>
				<h2 class="cmn-hed-h2">定期商品変更設定確認</h2>
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
												<div class="action_part_top">
													<asp:Button ID="btnBuckHistoryBackTop" Text="  戻る  " OnClick="btnBuckHistoryBack_Click" runat="server"/>
													<asp:Button ID="btnBackListPageTop" Text="  一覧へ戻る  " OnClick="btnBackListPage_Click" runat="server" />
													<asp:Button ID="btnEditTop" Text="  編集する  " OnClick="btnEdit_Click" runat="server" />
													<asp:Button ID="btnCopyInsertTop" Text="  コピー新規登録する  " Visible="false" OnClick="btnCopyInsert_Click" runat="server" />
													<asp:Button ID="btnDeleteTop" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('設定を削除します。よろしいですか？')" runat="server" />
													<asp:Button ID="btnInsertTop" Text="  登録する  " OnClick="btnInsert_Click" runat="server" />
													<asp:Button ID="btnUpdateTop" Text="  更新する  " OnClick="btnUpdate_Click" runat="server" />
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
													<tr>
														<td class="detail_title_bg" align="left" width="300">定期商品変更設定ID</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lFixedPurchaseProductChangeId" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">定期商品変更設定名</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lFixedPurchaseProductChangeName" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">適用優先順</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lPriority" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">有効フラグ</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lValidFlg" runat="server"></asp:Label>
														</td>
													</tr>
													<tr ID="trDateCreated" runat="server">
														<td class="detail_title_bg" align="left">作成日</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lbDateCreated" runat="server"></asp:Label>
														</td>
													</tr>
													<tr ID="trDateChanged" runat="server">
														<td class="detail_title_bg" align="left">更新日</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lbDateChanged" runat="server"></asp:Label>
														</td>
													</tr>
													<tr ID="trLastChanged" runat="server">
														<td class="detail_title_bg" align="left">最終更新者</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lbLastChanged" runat="server"></asp:Label>
														</td>
													</tr>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" style="table-layout:fixed;">
													<tr>
														<td class="edit_title_bg" align="center" colspan="4">変更元商品設定</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="center">商品ID</td>
														<td class="edit_title_bg" align="center">バリエーションID</td>
														<td class="edit_title_bg" align="center">商品名</td>
														<td class="edit_title_bg" align="center">指定単位</td>
													</tr>
													<asp:Repeater ID="rBeforeChangeItems" ItemType="FixedPurchaseBeforeChangeItemInput" runat="server">
														<ItemTemplate>
														<tr>
															<td class="edit_item_bg" align="center"><%#: Item.ProductId %></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(string.IsNullOrEmpty(Item.VariationId) == false ? Item.VariationId : "-")%></td>
															<td class="edit_item_bg" align="left"><%#: Item.ProductName %></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASEBEFORECHANGEITEM, Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_ITEM_UNIT_TYPE, Item.ItemUnitType))%></td>
														</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" style="table-layout:fixed;">
													<tr>
														<td class="edit_title_bg" align="center" colspan="4">変更後商品設定</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="center">商品ID</td>
														<td class="edit_title_bg" align="center">バリエーションID</td>
														<td class="edit_title_bg" align="center">商品名</td>
														<td class="edit_title_bg" align="center">指定単位</td>
													</tr>
													<asp:Repeater ID="rAfterChangeItems" ItemType="FixedPurchaseAfterChangeItemInput" runat="server">
														<ItemTemplate>
														<tr>
															<td class="edit_item_bg" align="center"><%# Item.ProductId %></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(string.IsNullOrEmpty(Item.VariationId) == false ? Item.VariationId : "-")%></td>
															<td class="edit_item_bg" align="left"><%# Item.ProductName %></td>
															<td class="edit_item_bg" align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASEAFTERCHANGEITEM, Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_ITEM_UNIT_TYPE, Item.ItemUnitType))%></td>
														</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												
												<div class="action_part_bottom">
													<asp:Button ID="btnBuckHistoryBackBottom" OnClick="btnBuckHistoryBack_Click" Text="  戻る  " runat="server"/>
													<asp:Button ID="btnBackListPageBottom" Text="  一覧へ戻る  " OnClick="btnBackListPage_Click" runat="server" />
													<asp:Button ID="btnEditBottom" Text="  編集する  " OnClick="btnEdit_Click" runat="server" />
													<asp:Button ID="btnCopyInsertBottom" Text="  コピー新規登録する  " Visible="false" OnClick="btnCopyInsert_Click" runat="server" />
													<asp:Button ID="btnDeleteBottom" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('設定を削除します。よろしいですか？')" runat="server" />
													<asp:Button ID="btnInsertBottom" Text="  登録する  " OnClick="btnInsert_Click" runat="server" />
													<asp:Button ID="btnUpdateBottom" Text="  更新する  " OnClick="btnUpdate_Click" runat="server" />
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
		<td><img height="10" alt="" src="<%: Constants.PATH_ROOT %>Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
