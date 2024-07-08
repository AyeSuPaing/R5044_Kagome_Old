<%--
=========================================================================================================
  Module      : かんたん会員登録設定ページ(UserEasyRegisterSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserEasyRegisterSettingList.aspx.cs" Inherits="Form_UserEasyRegisterSetting_UserEasyRegisterSettingList" %>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendRegist" Src="~/Form/Common/User/BodyUserExtendRegist.ascx" %>
<%@ Import Namespace="w2.App.Common.User" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
<%-- Empty --%>
</asp:Content>

<%-- UPDATE PANEL開始 --%>
<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
<table id="tblTtile" cellspacing="0" cellpadding="0" width="791" border="0" runat="server">
	<tr>
		<td><h1 class="page-title">かんたん会員登録設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ 登録編集 ▽-->
	<tr id="trEdit">
		<td><h2 class="cmn-hed-h2">かんたん会員登録設定</h2></td>
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
										<% if (this.mb_flg_result) { %>
										<tr>
											<td>
												<div>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">かんたん会員登録設定を更新しました。</td>
													</tr>
												</table>
												</div>
											</td>
										<tr>
										<% } %>
										<tr>
											<td>
												<div class="action_part_top">
													<asp:Button ID="btnUpdateTop" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" />
												</div>
												<table class="edit_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr class="list_title_bg">
														<td align="center" colspan="2">かんたん会員登録項目</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">
															<%: ReplaceTag("@@User.mail_addr.name@@") %>
														</td>
														<td align="left" class="edit_item_bg"><asp:CheckBox ID="cbUserMailAddr" Text="表示する" Checked="true" Enabled="False" runat="server"/></td>
													</tr>
													<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.login_id.name@@") %></td>
														<td align="left" class="edit_item_bg"><asp:CheckBox ID="cbUserLoginId" Text="表示する" Checked="true" Enabled="False" runat="server"/></td>
													</tr>
													<% } %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.password.name@@") %></td>
														<td align="left" class="edit_item_bg"><asp:CheckBox ID="cbUserPassword" Text="表示する" Checked="true" Enabled="False" runat="server"/></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.name.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserName" Text="表示する" runat="server"/>
														</td>
													</tr>
													<% if (this.IsShippingCountryAvailableJp) { %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserNameKana" Text="表示する" runat="server"/>
														</td>
													</tr>
													<% } %>
													<%--▽ 商品レビュー機能が有効の場合 ▽--%>
													<% if (Constants.PRODUCTREVIEW_ENABLED) { %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.nickname.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserNickName" Text="表示する" runat="server"/>
														</td>
													</tr>
													<% } %>
													<%--△ 商品レビュー機能が有効の場合 △--%>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.birth.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserBirth" Text="表示する" runat="server"/>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.sex.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserSex" Text="表示する" runat="server"/>
														</td>
													</tr>
													<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.mail_addr2.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserMailAddr2" Text="表示する" runat="server"/>
														</td>
													</tr>
													<% } %>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.country.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserCountry" Text="表示する" runat="server"/>
														</td>
													</tr>
													<% } %>
													<% if (Constants.GLOBAL_OPTION_ENABLE == false) { %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.zip.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserZip" Text="表示する" runat="server"/>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.addr1.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserAddr1" Text="表示する" runat="server"/>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.addr2.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserAddr2" Text="表示する" runat="server"/>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.addr3.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserAddr3" Text="表示する" runat="server"/>
														</td>
													</tr>
													<tr style='<%= Constants.DISPLAY_ADDR4_ENABLED ? "" : "display:none;"  %>'>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.addr4.name@@") %> <span class="notice"></span></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserAddr4" Text="表示する" runat="server"/>
														</td>
													</tr>
													<% } %>
													<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.company_name.name@@")%> <span class="notice"></span></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserCompanyName" Text="表示する" runat="server"/>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.company_post_name.name@@")%> <span class="notice"></span></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserCompanyPostName" Text="表示する" runat="server"/>
														</td>
													</tr>
													<%} %>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.tel1.name@@") %></td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserTel1" Text="表示する" runat="server"/>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.tel2.name@@") %> </td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserTel2" Text="表示する" runat="server"/>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">お知らせメールの配信希望</td>
														<td align="left" class="edit_item_bg">
															<asp:CheckBox ID="cbUserMailFlg" Text="表示する" runat="server"/>
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<asp:Button ID="btnUpdateBottom" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" />
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
</table>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>
</asp:Content>