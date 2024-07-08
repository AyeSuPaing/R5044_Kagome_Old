<%--
=========================================================================================================
  Module      : ユーザー情報登録ページ(UserRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserRegister.aspx.cs" Inherits="Form_User_UserRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendRegist" Src="~/Form/Common/User/BodyUserExtendRegist.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Import Namespace="w2.App.Common.User" %>
<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
<meta http-equiv="Pragma" content="no-cache">
<meta http-equiv="cache-control" content="no-cache">
<meta http-equiv="expires" content="0">
<% if (this.IsDefaultSettingPage) { %>
<style type="text/css">
	.default_setting_item { width : 10%; }
	.default_setting_noaction { display:none; }
</style>
<% } else { %>
<style type="text/css">
	.default_setting { display : none; }
	.default_setting_item { width : 30%; }
	.default_setting_noaction { word-break: break-all; font-size:90%; }
</style>
<% } %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<asp:UpdatePanel  runat="server">
	<ContentTemplate>
		<table cellspacing="0" cellpadding="0" width="791" border="0">
			<%if (this.IsPopUp == false) {%>
			<tr><td><h1 class="page-title">ユーザー情報</h1></td></tr>
			<%} %>
			<!--▽ 登録編集 ▽-->
			<tr id="trEdit" runat="server" Visible="False"><td><h1 class="cmn-hed-h2">ユーザー情報編集</h1></td></tr>
			<tr id="trRegister" runat="server" Visible="False"><td><h1 class="cmn-hed-h2">ユーザー情報登録</h1></td></tr>
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
														<div id="divComp" runat="server" class="action_part_top" Visible="False">
														<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
															<tr class="info_item_bg">
																<td align="left"><%: ReplaceTag("@@UserDefaultSetting.message_upsert.name@@") %>
																</td>
															</tr>
														</table>
														</div>
														<div class="action_part_top">

															<% if (Request[Constants.REQUEST_KEY_HIDE_BACK_BUTTON] != Constants.KBN_BACK_BUTTON_HIDDEN) { %>
															<asp:Button ID="btnHistoryBackTop" Text="戻る" runat="server" OnClick="btnHistoryBackTop_OnClick"/>
															<% } %>
															<asp:Button ID="btnConfirmTop" Text="  確認する  " runat="server" OnClick="btnConfirm_Click" />
															<asp:Button ID="btnUpdateDefaultSettingTop" Text="  更新する  " runat="server" OnClick="btnUpdateDefaultSetting_Click" />
														</div>
														<tr>
															<td>
																<table id="tblMemo" class="info_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server" visible='<%# (IsDefaultSettingPage)%>' >
																	<tr class="info_item_bg">
																		<td align="left"><%: ReplaceTag("@@UserDefaultSetting.setting_item_description_title.name@@") %>
																			<table class="info_table" cellspacing="1" cellpadding="3" width="742" border="0">
																				<tr class="info_item_bg">
																					<td class="edit_title_bg" align="left" width="50"><%: ReplaceTag("@@UserDefaultSetting.display_title.name@@") %></td>
																					<td align="left">
																						<%: ReplaceTag("@@UserDefaultSetting.display_description_title.name@@") %>
																					</td>
																				</tr>
																				<tr class="info_item_bg">
																					<td class="edit_title_bg" align="left"><%: ReplaceTag("@@UserDefaultSetting.item_memo_title.name@@") %></td>
																					<td align="left">
																						<%: ReplaceTag("@@UserDefaultSetting.item_memo_description_title.name@@") %>
																					</td>
																				</tr>
																				<tr class="info_item_bg">
																					<td class="edit_title_bg" align="left"><%: ReplaceTag("@@UserDefaultSetting.initial_value_title.name@@") %></td>
																					<td align="left">
																						<%: ReplaceTag("@@UserDefaultSetting.initial_value_description_title.name@@") %><br />
																						<%: ReplaceTag("@@UserDefaultSetting.message_default_value_title.name@@") %>
																					</td>
																				</tr>
																			</table>
																		</td>
																	</tr>
																</table>
															</td>
														</tr>
														<tr>
															<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td>
														</tr>
														<table class="edit_table" width="758" border="0" cellspacing="1" cellpadding="3">
															<tbody id="tbdyErrorMessages" visible="false" runat="server">
															<tr>
																<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
															</tr>
															<tr>
																<td class="edit_item_bg" align="left" colspan="4">
																	<asp:Label ID="lbErrorMessages" runat="server" ForeColor="red"></asp:Label>
																</td>
															</tr>
															</tbody>
															<tr class="default_setting">
																<td class="edit_title_bg default_setting" align="center" width="8%">表示</td>
																<td class="edit_title_bg default_setting" align="center" width="21%">項目名</td>
																<td class="edit_title_bg default_setting" align="center" width="11%">項目メモ</td>
																<td class="edit_title_bg default_setting" align="center" colspan="2" width="50%">初期値</td>
															</tr>
															<tr id="trInputUserId" visible="false" runat="server" >
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">ユーザーID</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserIdDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_USER_ID) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td align="left" class="edit_item_bg" />
																<td align="left" class="edit_item_bg">
																	<asp:Literal ID="lUserId" runat="server"></asp:Literal></td>
															</tr >
															<%--▽ モール連携オプションが有効の場合 ▽--%>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
															<tr id="trSiteName" runat="server">
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbSiteDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	サイト
																	<br />
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_MALL_ID) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbSiteDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_MALL_ID) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbSiteHasDefault" runat="server" Checked="true" Enabled="false" />
																</td>
																<td align="left" class="edit_item_bg" colspan="1">
																	<asp:HiddenField ID="hfMallId" runat="server" />
																	<asp:HiddenField ID="hfMallName" runat="server" />
																	<asp:Literal ID="lSiteName" runat="server"></asp:Literal></td>
															</tr>
															<% } %>
															<%--△ モール連携オプションが有効の場合 △--%>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN) %>'>
																<td class="edit_title_bg default_setting" align="center">
																<asp:CheckBox id="cbUserKbnDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	顧客区分
																	<span class="notice" runat="server">*</span><br />
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox id="tbUserKbnDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserKbnHasDefault" runat="server" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN) %>" />
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:DropDownList ID="ddlUserKbn" runat="server" />
																</td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_NAME1) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserNameDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.name.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_NAME1) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserNameDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_NAME1) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserNameHasDefault" runat="server" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_NAME1) %>" />
																</td>
																<td align="left" class="edit_item_bg">
																	姓：<asp:TextBox ID="tbUserName1" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_NAME1) %>" MaxLength="20" runat="server" />
																	名：<asp:TextBox ID="tbUserName2" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_NAME2) %>" MaxLength="20" runat="server" />
																</td>
															</tr>
															<% if (this.IsUserAddrJp) { %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_NAME_KANA1) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserNameKanaDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.name_kana.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_NAME_KANA1) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserNameKanaDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_NAME_KANA1) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserNameKanaHasDefault" runat="server" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_NAME_KANA1) %>" />
																</td>
																<td align="left" class="edit_item_bg">
																	姓：<asp:TextBox ID="tbUserNameKana1" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_NAME_KANA1) %>" MaxLength="30" runat="server" />
																	名：<asp:TextBox ID="tbUserNameKana2" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_NAME_KANA2) %>" MaxLength="30" runat="server" />
																</td>
															</tr>
															<% } %>
															<%--▽ 商品レビュー機能が有効の場合 ▽--%>
															<% if (Constants.PRODUCTREVIEW_ENABLED) { %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_NICK_NAME) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserNickNameDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_NICK_NAME) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.nickname.name@@") %>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER,Constants.FIELD_USER_NICK_NAME) %>
																	</span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserNickNameDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_NICK_NAME) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserNickNameHasDefault" runat="server" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_NICK_NAME) %>" />
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserNickName" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_NICK_NAME) %>" MaxLength="20" runat="server" />
																</td>
															</tr>
															<% } %>
															<%--△ 商品レビュー機能が有効の場合 △--%>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_SEX) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserSexDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%"><%: ReplaceTag("@@User.sex.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER,Constants.FIELD_USER_SEX) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox id="tbUserSexDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_SEX) %>" runat="server" Width="100" MaxLength="50" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox id="cbUserSexHasDefault" runat="server" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_SEX) %>" />
																	</td>
																<td align="left" class="edit_item_bg">
																	<asp:RadioButtonList ID="rblUserSex" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"
																		SelectedValue='<%#: (string.IsNullOrEmpty((string)GetKeyValue(this.UserMaster, Constants.FIELD_USER_SEX)) == false) 
																			? (string)GetKeyValue(this.UserMaster, Constants.FIELD_USER_SEX) 
																			: null %>' />
																</td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BIRTH) %>'> 
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbBirthDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.birth.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_BIRTH) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbBirthDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_BIRTH) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbBirthHasDefault" runat="server" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_BIRTH) %>" />
																</td>
																<td align="left" class="edit_item_bg">
																	<uc:DateTimeInput ID="ucUserBirth" runat="server" YearList="<%# DateTimeUtility.GetBirthYearListItem() %>" HasTime="False" HasBlankSign="False" HasBlankValue="True" ZeroSuppress="True"
																		SelectedDate="<%# GetDate(this.UserMaster, Constants.FIELD_USER_BIRTH) %>" />
																	<span style="font-size : x-small">※会員のみ必須</span></td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BIRTH) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserMailAddr1Default" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.mail_addr.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserMailAddr1Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserMailAddr1HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserMailAddr1" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_MAIL_ADDR) %>" Width="300" MaxLength="256" runat="server" />
																	<span runat="server" style="font-size : x-small" visible="<%# Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED %>">※会員のみPCモバイルどちらか必須</span>
																</td>
															</tr>
															<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR2) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserMailAddr2Default" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.mail_addr2.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR2) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserMailAddr2Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR2) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserMailAddr2HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR2) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserMailAddr2" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_MAIL_ADDR2) %>" Width="300" MaxLength="256" runat="server"></asp:TextBox>
																	<span style="font-size : x-small">※会員のみPCモバイルどちらか必須</span>
																</td>
															</tr>
															<% } %>
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserCountryDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.country.name@@", this.UserAddrCountryIsoCode) %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserCountryDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserCountryHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:DropDownList ID="ddlUserCountry" runat="server" AutoPostBack="true" />
																</td>
															</tr>
															<% } %>
															<% if (this.IsUserAddrJp) { %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ZIP1) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserZipDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.zip.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ZIP1) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserZipDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_ZIP1) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserZipHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_ZIP1) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserZip1" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_ZIP1) %>" Width="50" MaxLength="3" runat="server" />-
																	<asp:TextBox ID="tbUserZip2" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_ZIP2) %>" Width="70" MaxLength="4" runat="server" />
																	<asp:Button ID="btnUserZipSearch" Text="  住所検索  " runat="server" OnClick="btnOwnerZipSearch_Click" />
																</td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR1) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAddr1Default" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.addr1.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR1) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserAddr1Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_ADDR1) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAddr1HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_ADDR1) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:DropDownList ID="ddlUserAddr1" SelectedValue="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_ADDR1) %>" runat="server" />
																</td>
															</tr>
															<% } %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR2) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAddr2Default" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.addr2.name@@", this.UserAddrCountryIsoCode) %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR2) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserAddr2Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_ADDR2) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAddr2HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_ADDR2) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserAddr2" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_ADDR2) %>" MaxLength="50" Width="300" runat="server" />
																</td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR3) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAddr3Default" runat="server" Checked="<%# (this.IsUserAddrJp) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR3) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.addr3.name@@", this.UserAddrCountryIsoCode) %>
																	<% if (this.IsUserAddrJp) { %><span class="notice" runat="server">*</span><% } %>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR3) %>
																	</span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserAddr3Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_ADDR3) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAddr3HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_ADDR3) %>" runat="server" />
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserAddr3" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_ADDR3) %>" MaxLength="50" Width="300" runat="server" />
																</td>
															</tr>
															<tr style='<%= (Constants.DISPLAY_ADDR4_ENABLED || (this.IsUserAddrJp == false)) ? "" : "display:none;"  %>' runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR4) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAddr4Default" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR4) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.addr4.name@@", this.UserAddrCountryIsoCode) %>
																	<% if (CheckNecessaryAddress(Constants.FIELD_USER_ADDR4)) { %><span class="notice" runat="server">*</span><% } %>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR4) %>
																	</span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserAddr4Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_ADDR4) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAddr4HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_ADDR4) %>" runat="server" />
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserAddr4" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_ADDR4) %>" MaxLength="50" Width="300" runat="server" />
																</td>
															</tr>
															<% if (this.IsUserAddrJp == false) { %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR5) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAddr5Default" runat="server" Checked="<%# (this.IsUserAddrUs) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR5) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.addr5.name@@", this.UserAddrCountryIsoCode) %> 
																	<% if (this.IsUserAddrUs) { %><span class="notice" runat="server">*</span><% } %>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR5) %>
																	</span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserAddr5Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_ADDR5) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAddr5HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_ADDR5) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<% if (this.IsUserAddrUs) { %>
																	<asp:DropDownList ID="ddlUserAddr5" runat="server" />
																	<% } else { %>
																	<asp:TextBox ID="tbUserAddr5" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_ADDR5) %>" MaxLength="50" Width="300" runat="server" />
																	<% } %>
																</td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ZIP) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserZipGlobalDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ZIP) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.zip.name@@", this.UserAddrCountryIsoCode) %>
																	<% if (this.IsUserAddrZipNecessary) { %><span class="notice" runat="server">*</span><% } %>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ZIP) %>
																	</span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserZipGlobalDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_ZIP) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserZipGlobalHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_ZIP) %>" runat="server" />
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserZipGlobal" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_ZIP) %>" Width="120" MaxLength="20" runat="server" />
																	<asp:LinkButton
																		ID="lbSearchAddrFromZipGlobal"
																		OnClick="lbSearchAddrFromZipGlobal_Click"
																		Style="display:none;"
																		runat="server" />
																</td>
															</tr>
															<% } %>
															<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_NAME) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserCompanyNameDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_NAME) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.company_name.name@@")%>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_NAME) %>
																	</span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserCompanyNameDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_NAME) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserCompanyNameHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_NAME) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserCompanyName" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_COMPANY_NAME) %>" MaxLength="50" Width="300" runat="server" />
																</td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_POST_NAME) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserCompanyPostNameDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_POST_NAME) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.company_post_name.name@@")%>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_POST_NAME) %>
																	</span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserCompanyPostNameDefalutSetting" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_POST_NAME) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserCompanyPostNameHasDefalut" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_POST_NAME) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserCompanyPostName" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_COMPANY_POST_NAME) %>" MaxLength="50" Width="300" runat="server"></asp:TextBox></td>
															</tr>
															<%} %>
															<% if (this.IsUserAddrJp) { %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_TEL1_1) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserTelDefault" runat="server" Checked="true" Enabled="False"/>
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.tel1.name@@") %>
																	<span class="notice" runat="server">*</span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER,Constants.FIELD_USER_TEL1_1) %>
																	</span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserTelDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_TEL1_1) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserTelHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_TEL1_1) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserTel1_1" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_TEL1_1) %>" MaxLength="6" Width="40" runat="server" /> -
																	<asp:TextBox ID="tbUserTel1_2" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_TEL1_2) %>" MaxLength="4" Width="40" runat="server" /> -
																	<asp:TextBox ID="tbUserTel1_3" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_TEL1_3) %>" MaxLength="4" Width="40" runat="server" />
																	<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
																		<span style="font-size : x-small">※<%= WebMessages.GetMessages(WebMessages.ERRMSG_INPUT_GMO_KB_MOBILE_PHONE) %></span>
																	<% } %>
																</td>
															</tr>
															<% } else { %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_TEL1) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserTel1GlobalDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.tel1.name@@", this.UserAddrCountryIsoCode) %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"  ><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_TEL1) %></span>
																</td>
																<td class="edit_item_bg default_setting" align="left">
																	<asp:TextBox id="tbUserTel1GlobalDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_TEL1) %>" runat="server" Width="100" MaxLength="50" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserTel1GlobalHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_TEL1) %>" runat="server" />
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserTel1Global" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_TEL1) %>" MaxLength="30" Width="100" runat="server" />
																</td>
															</tr>
															<% } %>
															<% if (this.IsUserAddrJp) { %>
															<tr runat="server" visible="<%# (this.IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_TEL2_1) %>" >
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserTel2Default" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_TEL2_1) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.tel2.name@@") %>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_TEL2_1) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbUserTel2Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_TEL2_1) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserTel2HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_TEL2_1) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserTel2_1" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_TEL2_1) %>" MaxLength="6" Width="40" runat="server" /> -
																	<asp:TextBox ID="tbUserTel2_2" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_TEL2_2) %>" MaxLength="4" Width="40" runat="server" /> -
																	<asp:TextBox ID="tbUserTel2_3" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_TEL2_3) %>" MaxLength="4" Width="40" runat="server" />
																</td>
															</tr>
															<% } else { %>
															<tr runat="server" visible="<%# (this.IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_TEL2) %>" >
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserTel2GlobalDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_TEL2) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.tel2.name@@", this.UserAddrCountryIsoCode) %>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_TEL2) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbUserTel2GlobalDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_TEL2) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserTel2GlobalHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_TEL2) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserTel2Global" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_TEL2) %>" MaxLength="30" Width="100" runat="server" />
																</td>
															</tr>
															<% } %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserMailFlgDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.mail_flg.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG) %></span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbUserMailFlgDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserMailFlgHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG) %>" runat="server" />
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:RadioButtonList ID="rblUserMailFlg" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"
																		SelectedValue='<%#: (string.IsNullOrEmpty((string)GetKeyValue(this.UserMaster, Constants.FIELD_USER_MAIL_FLG)) == false) 
																			? (string)GetKeyValue(this.UserMaster, Constants.FIELD_USER_MAIL_FLG) 
																			: Constants.FLG_USER_MAILFLG_NG %>' />
																</td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserEasyRegisterFlgDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	かんたん会員フラグ
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br />
																		<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbUserEasyRegisterFlgDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserEasyRegisterFlgHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:RadioButtonList ID="rblUserEasyRegisterFlg" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"
																		SelectedValue='<%#: (string.IsNullOrEmpty((string)GetKeyValue(this.UserMaster, Constants.FIELD_USER_EASY_REGISTER_FLG)) == false) 
																			? (string)GetKeyValue(this.UserMaster, Constants.FIELD_USER_EASY_REGISTER_FLG) 
																			: Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL %>' />
																</td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LOGIN_ID) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserLoginIdDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.login_id.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_LOGIN_ID) %></span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbUserLoginIdDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_LOGIN_ID) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserLoginIdHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_LOGIN_ID) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserLoginId" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_LOGIN_ID) %>" runat="server" />
																	<span style="font-size : x-small">※会員のみ必須</span>
																</td>
															</tr>
															<tr style="display:<%= WebSanitizer.HtmlEncode(this.HavePasswordDisplayPower ? "" : "none") %>" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_PASSWORD) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserPasswordDefault" runat="server" Checked="true" Enabled="False" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	<%: ReplaceTag("@@User.password.name@@") %>
																	<span class="notice" runat="server">*<br /></span>
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_PASSWORD) %></span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbUserPasswordDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_PASSWORD) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserPasswordHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_PASSWORD) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbPassword" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_PASSWORD) %>" runat="server" />
																	<span style="font-size : x-small">※会員のみ必須</span></td>
															</tr>
															<%-- アフィリエイトOPが有効の場合 --%>
															<% if (Constants.W2MP_AFFILIATE_OPTION_ENABLED) { %>
																<tr runat="server" visible="<%# (this.IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADVCODE_FIRST) %>" >
																	<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserAdvCodeDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADVCODE_FIRST) %>" />
																	</td>
																	<td align="left" class="edit_title_bg" width="30%">
																		広告コード（初回分）
																		<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br />
																		<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADVCODE_FIRST) %>
																		</span>
																	</td>
																	<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbUserAdvCodeDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_ADVCODE_FIRST) %>" MaxLength="30" Width="100" runat="server" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox id="cbUserAdvCodeHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_ADVCODE_FIRST) %>" runat="server" />
																	</td>
																	<td align="left" class="edit_item_bg">
																		<asp:TextBox ID="tbUserAdvCode" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_ADVCODE_FIRST) %>" runat="server" />
																	</td>
																</tr>
															<% } %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserMemoDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	ユーザー特記欄
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																	<br />
																	<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																<asp:TextBox ID="tbUserMemoDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserMemoHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbUserMemo" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_USER_MEMO) %>" runat="server" TextMode="MultiLine" Rows="8" Width="500" />
																</td>
															</tr>
															<%--▽ 会員ランクOPが有効の場合 ▽--%>
															<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MEMBER_RANK_ID) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbMemberRankDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MEMBER_RANK_ID) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	会員ランク
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																	<br />
																	<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_MEMBER_RANK_ID) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbMemberRankDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_MEMBER_RANK_ID) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbMemberRankHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_MEMBER_RANK_ID) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:DropDownList ID="ddlMemberRank" runat="server" />
																</td>
															</tr>
															<% } %>
															<%--△ 会員ランクOPが有効の場合 △--%>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserManagementLevelDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	ユーザー管理レベル
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																	<br />
																	<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																<asp:TextBox ID="tbUserManagementLevelDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbUserManagementLevelHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:DropDownList ID="ddlUserManagementLevel" runat="server" />
																</td>
															</tr>
															<%--▽ グローバル対応有効の場合 ▽--%>
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbAccessCountryIsoCodeDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	アクセス国ISOコード
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																	<br />
																	<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																<asp:TextBox ID="tbAccessCountryIsoCodeDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbAccessCountryIsoCodeHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:DropDownList ID="ddlAccessCountryIsoCode" runat="server" />
																</td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbDispLanguageLocaleIdDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	表示言語ロケールID
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																	<br />
																	<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER,Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																<asp:TextBox ID="tbDispLanguageLocaleIdDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID) %>" MaxLength="30" Width="100" runat="server"></asp:TextBox>
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbDispLanguageLocaleIdHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:DropDownList id="ddlDispLanguageLocaleId"
																		runat="server" OnSelectedIndexChanged="ddlDispLanguageLocaleId_SelectedIndexChanged"
																		AutoPostBack="true" />
																	言語コード( <asp:Literal ID="lDispLanguageCode" runat="server"></asp:Literal> )
																</td>
															</tr>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbDispCurrencyLocaleIdDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	表示通貨ロケールID
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																	<br />
																	<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbDispCurrencyLocaleIdDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbDispCurrencyLocaleIdHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:DropDownList id="ddlDispCurrencyLocaleId"
																		SelectedValue="<%# (Constants.GLOBAL_OPTION_ENABLE) ? GetKeyValue(this.UserMaster, Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID) : null %>"
																		runat="server"
																		OnSelectedIndexChanged="ddlDispCurrencyLocaleId_SelectedIndexChanged"
																		AutoPostBack="true" />
																	通貨コード( <asp:Literal ID="lDispCurrencyCode" runat="server"></asp:Literal> )
																</td>
															</tr>
															<% } %>
															<%--△ グローバル対応有効の場合 △--%>
															<% if (Constants.W2MP_POINT_OPTION_ENABLED){ %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbLastBirthdayPointAddYearDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	最終誕生日ポイント付与年
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																	<br />
																	<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																<asp:TextBox ID="tbLastBirthdayPointAddYearDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbLastBirthdayPointAddYearHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR) %>" runat="server"/>
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbLastBirthdayPointAddYear" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR) %>" MaxLength="4" runat="server" />
																</td>
															</tr>
															<% } %>
															<% if (Constants.W2MP_COUPON_OPTION_ENABLED){ %>
															<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR) %>'>
																<td class="edit_title_bg default_setting" align="center">
																	<asp:CheckBox id="cbLastBirthdayCouponPublishYearDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR) %>" />
																</td>
																<td align="left" class="edit_title_bg" width="30%">
																	最終誕生日クーポン付与年
																	<span visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																	<br />
																	<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR) %>
																	</span>
																</td>
																<td align="left" class="edit_item_bg default_setting">
																<asp:TextBox ID="tbLastBirthdayCouponPublishYearDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR) %>" MaxLength="30" Width="100" runat="server" />
																</td>
																<td class="edit_item_bg default_setting" align="center">
																	<asp:CheckBox id="cbLastBirthdayCouponPublishYearHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR) %>" runat="server" />
																</td>
																<td align="left" class="edit_item_bg">
																	<asp:TextBox ID="tbLastBirthdayCouponPublishYear" Text="<%# GetKeyValue(this.UserMaster, Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR) %>" MaxLength="4" runat="server" />
																</td>
															</tr>
															<% } %>
															<%-- ユーザ拡張項目 --%>
															<tbody id="divBodyUserExtendRegist" visible="<%# this.IsDefaultSettingPage == false %>" runat="server">
															<uc:BodyUserExtendRegist ID="ucBodyUserExtendRegist" runat="server" />
															</tbody>
															<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
																<%--GMO--%>
																<% if (this.IsUserAddrJp) { %>
																<tr id="Tr1" runat="server" visible='<%# (!IsDefaultSettingPage) %>'>
																	<td align="left" class="edit_title_bg" width="30%">
																		GMO枠保証を希望する
																	</td>
																	<td align="left" class="edit_item_bg">
																		<asp:CheckBox id="isBusinessOwner" runat="server" OnCheckedChanged="checkBusinessOwnerChangedEvent" AutoPostBack="True"  Checked="true" />
																	</td>
																</tr>
															
																<%if (isBusinessOwner.Checked || IsDefaultSettingPage){%>
																<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME1) %>'>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox id="cbOwnerName1Default" runat="server" Checked="true" Enabled="false" />
																	</td>
																	<td align="left" class="edit_title_bg" width="30%">
																		<%: ReplaceTag("@@User.OwnerName1.name@@") %>
																		<span id="Span12" class="notice" runat="server">*<br /></span>
																		<span id="Span1" visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br />
																		<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME1) %>
																		</span>
																	</td>
																	<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbOwnerName1Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME1) %>" MaxLength="30" Width="100" runat="server" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox id="cbOwnerName1HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME1) %>" runat="server" />
																	</td>
																	<td align="left" class="edit_item_bg">
																		<asp:TextBox ID="tbOwnerName1" Text="<%# GetKeyValue(this.userBusinessOwnerMaster, Constants.FIELD_USER_BUSINESS_OWNER_NAME1) %>" MaxLength="21" runat="server" />
																	</td>
																</tr>
															
																<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME2) %>'>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox id="cbOwnerName2Default" runat="server" Checked="true" Enabled="false" />
																	</td>
																	<td align="left" class="edit_title_bg" width="30%">
																		<%: ReplaceTag("@@User.OwnerName2.name@@") %>
																		<span id="Span11" class="notice" runat="server">*<br /></span>
																		<span id="Span2" visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br />
																		<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME2) %>
																		</span>
																	</td>
																	<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbOwnerName2Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME2) %>" MaxLength="30" Width="100" runat="server" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox id="cbOwnerName2HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME2) %>" runat="server" />
																	</td>
																	<td align="left" class="edit_item_bg">
																		<asp:TextBox ID="tbOwnerName2" Text="<%# GetKeyValue(this.userBusinessOwnerMaster, Constants.FIELD_USER_BUSINESS_OWNER_NAME2) %>" MaxLength="21" runat="server" />
																	</td>
																</tr>

																<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1) %>'>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox id="cbOwnerNameKana1Default" runat="server" Checked="true" Enabled="false" />
																	</td>
																	<td align="left" class="edit_title_bg" width="30%">
																		<%: ReplaceTag("@@User.OwnerNameKana1.name@@") %>
																		<span id="Span10" class="notice" runat="server">*<br /></span>
																		<span id="Span3" visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br />
																		<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1) %>
																		</span>
																	</td>
																	<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbOwnerNameKana1Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1) %>" MaxLength="30" Width="100" runat="server" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox id="cbOwnerNameKana1HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1) %>" runat="server" />
																	</td>
																	<td align="left" class="edit_item_bg">
																		<asp:TextBox ID="tbOwnerNameKana1" Text="<%# GetKeyValue(this.userBusinessOwnerMaster, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1) %>" MaxLength="25" runat="server" />
																	</td>
																</tr>

																<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2) %>'>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox id="cbOwnerNameKana2Default" runat="server" Checked="true" Enabled="false" />
																	</td>
																	<td align="left" class="edit_title_bg" width="30%">
																		<%: ReplaceTag("@@User.OwnerNameKana2.name@@") %>
																		<span id="Span9" class="notice" runat="server">*<br /></span>
																		<span id="Span4" visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br />
																		<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2) %>
																		</span>
																	</td>
																	<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbOwnerNameKana2Default" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2) %>" MaxLength="30" Width="100" runat="server" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox id="cbOwnerNameKana2HasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2) %>" runat="server" />
																	</td>
																	<td align="left" class="edit_item_bg">
																		<asp:TextBox ID="tbOwnerNameKana2" Text="<%# GetKeyValue(this.userBusinessOwnerMaster, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2) %>" MaxLength="25" runat="server" />
																	</td>
																</tr>

																<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_BIRTH) %>'> 
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox id="cbOwnerBirthDefault" runat="server" Checked="true" Enabled="false" />
																	</td>
																	<td align="left" class="edit_title_bg" width="30%">
																		<%: ReplaceTag("@@User.OwnerBirth.name@@") %>
																		<span id="Span5" class="notice" runat="server">*<br /></span>
																		<span id="Span6" visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server"><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_BIRTH) %></span>
																	</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox id="tbOwnerBirthDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_BIRTH) %>" runat="server" Width="100" MaxLength="50" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox id="cbOwnerBirthHasDefault" runat="server" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_BIRTH) %>" />
																	</td>
																	<td align="left" class="edit_item_bg">
																		<uc:DateTimeInput ID="ucOwnerBirth" runat="server" YearList="<%# DateTimeUtility.GetBirthYearListItem() %>" HasTime="False" HasBlankSign="False" HasBlankValue="True" ZeroSuppress="True"
																			SelectedDate="<%# GetDate(this.userBusinessOwnerMaster, Constants.FIELD_USER_BUSINESS_OWNER_BIRTH) %>" />
																	</td>
																</tr>

																<tr runat="server" visible='<%# (IsDefaultSettingPage) ? true : IsDefaultSettingDisplayField(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET) %>'>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox id="cbRequestBudgetDefault" runat="server" Checked="<%# IsDefaultSettingDisplayField(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET) %>" />
																	</td>
																	<td align="left" class="edit_title_bg" width="30%">
																		<%: ReplaceTag("@@User.RequestBudget.name@@") %>
																		<span id="Span7" visible="<%# (this.IsDefaultSettingPage == false) %>" runat="server">
																		<br />
																		<%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET) %>
																		</span>
																	</td>
																	<td align="left" class="edit_item_bg default_setting">
																	<asp:TextBox ID="tbRequestBudgetDefault" Text="<%# GetDefaultSettingComment(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET) %>" MaxLength="30" Width="100" runat="server" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox id="cbRequestBudgetHasDefault" Checked="<%# IsDefaultSettingHasDefault(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET) %>" runat="server" />
																	</td>
																	<td align="left" class="edit_item_bg">
																		<asp:TextBox ID="tbRequestBudget" Text="<%# GetKeyValue(this.userBusinessOwnerMaster, Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET) %>" MaxLength="8" runat="server" />
																		<p style="display: contents;">円</p>
																	</td>
																</tr>
																<% } %>
															<%} %>
															<%} %>
														</table>

														<div class="action_part_bottom">
															<% if (Request[Constants.REQUEST_KEY_HIDE_BACK_BUTTON] != Constants.KBN_BACK_BUTTON_HIDDEN) { %>
															<asp:Button ID="btnBackHistoryBottom1" Text="戻る" runat="server" OnClick="btnHistoryBackTop_OnClick" />
															<% } %>
															<asp:Button ID="btnConfirmBottom" Text="  確認する  " runat="server" OnClick="btnConfirm_Click" />
															<asp:Button ID="btnUpdateDefaultSettingBottom" Text="  更新する  " runat="server" OnClick="btnUpdateDefaultSetting_Click" />
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

<script type="text/javascript">
	execAutoKanaWithKanaType(
		$("#<%= tbUserName1.ClientID %>"),
		$("#<%= tbUserNameKana1.ClientID %>"),
		$("#<%= tbUserName2.ClientID %>"),
		$("#<%= tbUserNameKana2.ClientID %>"));

	<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
	// Textbox change search zip global
	textboxChangeSearchGlobalZip(
		'<%= tbUserZipGlobal.ClientID %>',
		'<%= lbSearchAddrFromZipGlobal.UniqueID %>');

	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(
		function () {
			// Textbox change search zip global
			textboxChangeSearchGlobalZip(
				'<%= tbUserZipGlobal.ClientID %>',
				'<%= lbSearchAddrFromZipGlobal.UniqueID %>');
		});
	<% } %>
</script>

</asp:Content>

