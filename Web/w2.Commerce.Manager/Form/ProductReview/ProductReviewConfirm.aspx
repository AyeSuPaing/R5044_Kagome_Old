<%--
=========================================================================================================
  Module      : 商品レビュー確認ページ(ProductReviewConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Reference Page="~/Form/ProductReview/ProductReviewList.aspx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductReviewConfirm.aspx.cs" Inherits="Form_ProductReview_ProductReviewConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">レビュー管理</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">レビュー詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">レビュー確認</h2></td>
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
												<center>
													<span class="notice">
														<strong><asp:Literal ID="lMessages" Text="レビューの更新が完了しました。" Visible="false" runat="server"></asp:Literal></strong>
													</span>
												</center>
												<div class="action_part_top">
													<input id="btnHistoryBackTop" value="  戻る  " onclick="javascript:history.back();" type="button" runat="server" />
													<asp:Button ID="btnBackListTop" Text="  一覧へ戻る  " runat="server" OnClick="btnBack_Click" />
													<asp:Button ID="btnEditTop" Text="  編集する  " runat="server" OnClick="btnEdit_Click" />
													<asp:Button ID="btnUpdateTop" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" />
													<asp:Button ID="btnDeleteTop" Text="  削除する  " runat="server" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');"/>
												</div>
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="detail_title_bg" width="30%">公開/非公開</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lOpenFlg" runat="server"></asp:Literal>&nbsp;&nbsp;&nbsp;
															<asp:Button ID="btnOpenValid" text="  公開する  " runat="server" OnClick="btnOpenValid_Click" />
															<asp:Button ID="btnOpenUnValid" text="  非公開にする  " runat="server" OnClick="btnOpenUnValid_Click" />
														</td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">公開日</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lOpenDate" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">チェック済/未チェック</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lCheckFlg" runat="server"></asp:Literal>&nbsp;&nbsp;&nbsp;
															<asp:Button ID="btnCheckValid" text="  チェックする  " runat="server" OnClick="btnCheckValid_Click" />
															<asp:Button ID="btnCheckUnValid" text="  未チェックにする  " runat="server" OnClick="btnCheckUnValid_Click" />
														</td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">チェック日</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lCheckDate" runat="server"></asp:Literal>
														</td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">商品ID</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lProductId" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">商品名</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lProductName" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">ユーザID</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lUserId" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">ニックネーム</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lNickname" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">評価</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lRating" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">タイトル</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lTitle" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">コメント</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal ID="lComment" runat="server"></asp:Literal></td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<input id="btnHistoryBackBottom" value="  戻る  " onclick="javascript:history.back();" type="button" runat="server" />
													<asp:Button ID="btnBackListBottom" Text="  一覧へ戻る  " runat="server" OnClick="btnBack_Click" />
													<asp:Button ID="btnEditBottom" Text="  編集する  " runat="server" OnClick="btnEdit_Click" />
													<asp:Button ID="btnUpdateBottom" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" />
													<asp:Button ID="btnDeleteBottom" Text="  削除する  " runat="server" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');"/>
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
