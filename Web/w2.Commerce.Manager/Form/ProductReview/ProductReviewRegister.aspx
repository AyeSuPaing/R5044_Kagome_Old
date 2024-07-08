<%--
=========================================================================================================
  Module      : 商品レビュー登録ページ(ProductReviewRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductReviewRegister.aspx.cs" Inherits="Form_ProductReview_ProductReviewRegister" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
// 公開フラグ選択状態によって日付入力の状態変更
function open_flg_checkbox_status()
{
	setDisableSelectPeriodModal('<%= ucOpenDatePeriod.ClientID %>', document.getElementById('<%= cbOpenFlg.ClientID %>').checked == false);
}

// チェックフラグ選択状態によって日付入力の状態変更
function check_flg_checkbox_status()
{
	setDisableSelectPeriodModal('<%= ucCheckDatePeriod.ClientID %>', document.getElementById('<%= cbCheckFlg.ClientID %>').checked == false);
}

//-->
</script>
<%
	// JavaScirpt設定
	cbOpenFlg.Attributes["onClick"] += "open_flg_checkbox_status()";
	cbCheckFlg.Attributes["onClick"] += "check_flg_checkbox_status()";
%>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">レビュー管理</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 編集 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">レビュー編集</h2></td>
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
													<input id="btnHistoryBackTop" value="  戻る  " onclick="javascript:history.back();" type="button">
													<asp:Button ID="btnConfirmTop" Text="  確認する  " runat="server" OnClick="btnConfirm_Click" />
												</div>
												<table class="edit_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="edit_title_bg" align="left">商品ID</td>
														<td class="edit_item_bg" align="left"><asp:Literal ID="lProductId" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">商品名</td>
														<td class="edit_item_bg" align="left"><asp:Literal ID="lProductName" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">公開/非公開</td>
														<td class="edit_item_bg" align="left"><asp:CheckBox id="cbOpenFlg" runat="server" Text="公開"></asp:CheckBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">公開日</td>
														<td class="edit_item_bg" align="left">
															<uc:DateTimePickerPeriodInput id="ucOpenDatePeriod" CanShowEndDatePicker="False" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">チェック済/未チェック</td>
														<td class="edit_item_bg" align="left"><asp:CheckBox id="cbCheckFlg" runat="server" Text="チェック済"></asp:CheckBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">チェック日</td>
														<td class="edit_item_bg" align="left">
															<uc:DateTimePickerPeriodInput id="ucCheckDatePeriod" CanShowEndDatePicker="False" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">ユーザID</td>
														<td class="edit_item_bg" align="left"><asp:Literal ID="lUserId" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">ニックネーム<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:Textbox ID="tbNickname" runat="server"></asp:Textbox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">評価</td>
														<td class="edit_item_bg" align="left"><asp:DropDownList ID="ddlRating" runat="server"></asp:DropDownList></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">タイトル<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:Textbox ID="tbTitle" Columns="74" runat="server"></asp:Textbox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">コメント<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:Textbox ID="tbComment" TextMode="MultiLine" Rows="4" Width="420" runat="server"></asp:Textbox></td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<input id="btnHistoryBackBotton" value="  戻る  " onclick="javascript:history.back();" type="button">
													<asp:Button ID="btnConfirmBottom" Text="  確認する  " runat="server" OnClick="btnConfirm_Click" />
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
	<!--△ 編集 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
