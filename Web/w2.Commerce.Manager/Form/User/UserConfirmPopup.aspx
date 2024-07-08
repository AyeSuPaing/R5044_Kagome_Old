<%--
=========================================================================================================
  Module      : ユーザー情報ポップアップ確認ページ(UserConfirmPopup.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyUserConfirm" Src="~/Form/Common/BodyUserConfirm.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="UserConfirmPopup.aspx.cs" Inherits="Form_User_UserConfirmPopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h2 class="cmn-hed-h2">ユーザー情報詳細</h2></td>
		<span id="spanUpdateHistoryConfirmTop" runat="server">( <a href="javascript:open_window('<%= UpdateHistoryPage.CreateUpdateHistoryConfirmUrl(Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_USER, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_ID]), "") %>','updatehistory','width=1200,height=850,top=5,left=600,status=NO,scrollbars=yes,resizable=yes')">履歴参照</a> )</span>
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
												<%--▽ ユーザ情報表示 ▽--%>
												<uc:BodyUserConfirm id="ucBodyUserConfirm" runat="server"></uc:BodyUserConfirm>
												<%--△ ユーザ情報表示 △--%>
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>