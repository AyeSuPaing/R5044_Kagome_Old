<%--
=========================================================================================================
  Module      : ユーザー情報確認ページ(UserConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Reference Page="~/Form/User/UserList.aspx" %>
<%@ Register TagPrefix="uc" TagName="BodyUserConfirm" Src="~/Form/Common/BodyUserConfirm.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserConfirm.aspx.cs" Inherits="Form_User_UserConfirm" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<%if (this.IsPopUp == false) {%>
	<tr><td><h1 class="page-title">ユーザー情報</h1></td></tr>
	<%} %>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="false"><td><h1 class="cmn-hed-h2">ユーザー情報詳細</h1></td></tr>
	<tr id="trConfirm" runat="server" Visible="false"><td><h1 class="cmn-hed-h2">ユーザー情報確認</h1></td></tr>
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
														<strong><asp:Literal ID="lMessages" Visible="false" runat="server"></asp:Literal></strong>
													</span>
												</center>
												<div class="action_part_top">
													<span id="spanUpdateHistoryConfirmTop" runat="server">( <a href="javascript:open_window('<%= UpdateHistoryPage.CreateUpdateHistoryConfirmUrl(Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_USER, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_ID]), "") %>','updatehistory','width=1200,height=850,top=5,left=600,status=NO,scrollbars=yes,resizable=yes')">履歴参照</a> )</span>
													<asp:Button ID="btnHistoryBackTop" Text="戻る" runat="server" OnClick="btnBuckHistoryBackTop_OnClick" />
													<asp:Button ID="btnBackListTop" Text="  一覧へ戻る  " runat="server" OnClick="btnBack_Click" />
													<asp:Button ID="btnEditTop" Text="  編集する  " runat="server" OnClick="btnEdit_Click" />
													<asp:Button ID="btnUpdateTop" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" />
													<asp:Button ID="btnRegistTop" Text="  登録する  " runat="server" OnClick="btnRegist_Click" />
												</div>
												<div id="divAnchor" visible="false" runat="server" style="margin: 5px 0;">
													<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %><a href="#anchorFixedPurchaseHistory">&nbsp;▼定期購入&nbsp;</a>&nbsp;<% } %>
													<a href="#anchorOrderHistory">&nbsp;▼過去の受注履歴&nbsp;</a>&nbsp;
													<% if (Constants.MEMBER_RANK_OPTION_ENABLED){ %><a href="#anchorMemberRankHistory">&nbsp;▼会員ランク更新履歴&nbsp;</a><% } %>
												</div>
												<%--▽ ユーザ情報表示 ▽--%>
												<uc:BodyUserConfirm id="ucBodyUserConfirm" runat="server"></uc:BodyUserConfirm>
												<%--△ ユーザ情報表示 △--%>
												<div class="action_part_bottom">
													<asp:Button ID="btnHistoryBackBottom" Text="戻る" runat="server" OnClick="btnBuckHistoryBackTop_OnClick" />
													<asp:Button ID="btnBackListBottom" Text="  一覧へ戻る  " runat="server" OnClick="btnBack_Click" />
													<asp:Button ID="btnEditBottom" Text="  編集する  " runat="server" OnClick="btnEdit_Click" />
													<asp:Button ID="btnUpdateBottom" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" />
													<asp:Button ID="btnRegistBottom" Text="  登録する  " runat="server" OnClick="btnRegist_Click" />
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