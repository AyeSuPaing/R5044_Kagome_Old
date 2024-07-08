<%--
=========================================================================================================
  Module      : CS顧客検索画面(UserSearch.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="UserSearch.aspx.cs" Inherits="Form_Message_UserSearch" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.App.Common.Cs.UserHistory" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Import Namespace="w2.Domain.DmShippingHistory" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.Domain.ExternalLink" %>
<%@ Register TagPrefix="uc" TagName="ErrorPoint" Src="~/Form/Top/ErrorPointIcon.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<style>
	.left {
		float:left;
		margin-right:5px
	}
</style>
<!-- 顧客検索フォーム -->
<table border="0" cellpadding="0" cellspacing="0" width="100%">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
			<div style="WIDTH:99%;float:left;margin:2px 2px 2px 4px">
			
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
			<tr class="alt">
			<td>
<%
	tbSearchUserName.Attributes["onkeypress"]
		= tbSearchTel.Attributes["onkeypress"]
		= tbSearchMailAddr.Attributes["onkeypress"]
		= tbSearchUserId.Attributes["onkeypress"]
		= tbSearchOrderId.Attributes["onkeypress"]
		= "if (event.keyCode == 13) { __doPostBack('" + btnSearchUser.UniqueID + "',''); return false; }";
%>
				<table width="100%" cellpadding="0" cellspacing="0" class="cssearch_table">
				<tr>
				<td class="cssearch_title_bg" nowrap="nowrap" width="80"><img src="../../Images/Common/arrow_01.gif" alt="" border="0" />氏名 </td>
				<td class="cssearch_item_bg"><asp:TextBox id="tbSearchUserName" Width="120" runat="server" /></td>
				<td class="cssearch_title_bg" nowrap="nowrap" width="80"><img src="../../Images/Common/arrow_01.gif" alt="" border="0" />電話番号</td>
				<td class="cssearch_item_bg"><asp:TextBox id="tbSearchTel" Width="120" runat="server" /></td>
				<td class="cssearch_title_bg" nowrap="nowrap" width="80"><img src="../../Images/Common/arrow_01.gif" alt="" />メールアドレス</td>
				<td class="cssearch_item_bg"><asp:TextBox id="tbSearchMailAddr" Width="120" runat="server"></asp:TextBox></td>
				<td class="cssearch_title_bg" nowrap="nowrap" width="80"><img src="../../Images/Common/arrow_01.gif" alt="" height="5" />ユーザーID</td>
				<td class="cssearch_item_bg"><asp:TextBox id="tbSearchUserId" Width="120" runat="server"></asp:TextBox></td>
				<td class="cssearch_title_bg" nowrap="nowrap" width="80"><img src="../../Images/Common/arrow_01.gif" alt="" border="0" />注文ID</td>
				<td class="search_item_bg"><asp:TextBox id="tbSearchOrderId" Width="120" runat="server"></asp:TextBox></td>
				</tr>
				</table>
			</td>
			</tr>
			</table>
			<asp:Button ID="btnSearchUser" Text="　顧客検索　" runat="server" OnClick="btnSearchUser_Click" CssClass="left" />
			<div class="left">
				<asp:UpdatePanel ID="upButtons" runat="server">
					<ContentTemplate>
						<input value="  新規顧客登録  "
							onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(
								MenuAuthorityHelper.ManagerSiteType.Ec,
								Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_REGISTER + "?"
									+ Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT
									+ "&" + Constants.REQUEST_KEY_USER_KBN + "=" + Constants.FLG_USER_USER_KBN_CS
									+ "&" + Constants.REQUEST_KEY_WINDOW_KBN + "=" + Constants.KBN_WINDOW_POPUP
									+ "&" + Constants.REQUEST_KEY_HIDE_BACK_BUTTON + "=" + Constants.KBN_BACK_BUTTON_HIDDEN
									+ "&" + Constants.REQUEST_KEY_MANAGER_POPUP_CLOSE_CONFIRM + "=" + WebMessages.CFMMSG_USERREGISTER_POPUP_CLOSE_CONFIRM
									+ ((this.IsAddParamTelNo)
										? "&" + Constants.REQUEST_KEY_TEL_NO + "=" + HttpUtility.UrlEncode(StringUtility.ToHankaku(tbSearchTel.Text))
										: ""))) %>','userregister','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');"
							type="button"
							<%= this.LoginOperatorCsInfo.EX_PermitEditFlg ? string.Empty : "disabled='disabled'" %> />
						<input value="  注文登録  "
							onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(
								MenuAuthorityHelper.ManagerSiteType.Ec,
								Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_REGIST_INPUT + "?"
									+ ((string.IsNullOrEmpty(hfSelectedUserId.Value) == false)
										? (Constants.REQUEST_KEY_USER_ID + "=" + HttpUtility.UrlEncode(hfSelectedUserId.Value) + "&")
										: string.Empty)
									+ Constants.REQUEST_KEY_WINDOW_KBN + "=" + Constants.KBN_WINDOW_POPUP
									+ "&" + Constants.REQUEST_KEY_MANAGER_POPUP_CLOSE_CONFIRM + "=" + WebMessages.CFMMSG_ORDERREGISTINPUT_POPUP_CLOSE_CONFIRM
									+ ((this.IsAddParamTelNo)
										? "&" + Constants.REQUEST_KEY_TEL_NO + "=" + HttpUtility.UrlEncode(StringUtility.ToHankaku(tbSearchTel.Text))
										: ""))) %>','orderregister','width=850,height=600,top=110,left=380,status=NO,scrollbars=yes','');"
							type="button"
							<%= this.LoginOperatorCsInfo.EX_PermitEditFlg ? string.Empty : "disabled='disabled'" %> />
						<input value="  問合せ追加  " style="overflow: visible;"
							onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(
								new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_MESSAGE_INPUT)
									.AddParam(Constants.REQUEST_KEY_MESSAGE_MEDIA_MODE, Constants.KBN_MESSAGE_MEDIA_MODE_TEL)
									.AddParam(Constants.REQUEST_KEY_MESSAGE_EDIT_MODE, Constants.KBN_MESSAGE_EDIT_MODE_NEW)
									.AddParam(Constants.REQUEST_KEY_USER_ID, hfSelectedUserId.Value).CreateUrl())%>','messageInput','width=1200,height=740,top=110,left=380,status=NO,scrollbars=yes');"
							type="button"
							<%= this.LoginOperatorCsInfo.EX_PermitEditFlg ? string.Empty : "disabled='disabled'" %> />
					</ContentTemplate>
				</asp:UpdatePanel>
			</div>
			<div style="margin-top:8px;">
				<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_USER_SEARCH %>">クリア</a>&nbsp;&nbsp;
				<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
			</div>
			</div>
		</td>
	</tr>
</table>

<img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />

<!-- ユーザー一覧 -->
<div id="divUserList" visible="false" runat="server">
	<table border="0" cellpadding="0" cellspacing="0" width="100%">
	<tr>
	<td align="center">
		<div style="WIDTH:99%;float:left;margin:2px 2px 2px 4px">
			<table border="0" cellpadding="1" cellspacing="1" width="100%">
			<tr>
			<td>
			<div class="dataresult">
				<div style="WIDTH:100%;float:left;margin-bottom:1px">
					<table>
					<thead>
					<tr>
						<th>
							<span style="float:left">
							顧客検索結果 （該当：<asp:Literal ID="lUserSearchCount" runat="server" />）
							</span>
						</th>
					</tr>
					</thead>
					</table>
				</div>
				<div id="divUserListInner">
					<div class="y_scrollable div_table_header">
						<table class="list_table_min">
							<tr class="alt">
								<td width="9%">
									<asp:LinkButton runat="server" ID="lbUserIdSort" OnClick="lbUserSearchSort_Click" CommandArgument="UserId" Width="100%" Font-Underline="false">ユーザーID
										<span style="float:right"><asp:Label runat="server" ID="lUserIdIconSort"></asp:Label></span>
									</asp:LinkButton>
								</td>
								<td width="9%" nowrap="nowrap">
									<asp:LinkButton runat="server" ID="lbClassificationSort" OnClick="lbUserSearchSort_Click" CommandArgument="Classification" Width="100%" Font-Underline="false">
										<span style="float:left;">区分</span>
										<span style="float:right"><asp:Label runat="server" ID="lClassificationIconSort"></asp:Label></span>
									</asp:LinkButton>
								</td>
								<td width="11%" nowrap="nowrap">
									<asp:LinkButton runat="server" ID="lbUserNameSort" OnClick="lbUserSearchSort_Click" CommandArgument="UserName" Width="100%" Font-Underline="false">
										<span style="float:left;">氏名</span>
										<span style="float:right"><asp:Label runat="server" ID="lUserNameIconSort"></asp:Label></span>
									</asp:LinkButton>
								</td>
								<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
									<td width="11%" nowrap="nowrap">
										<asp:LinkButton runat="server" ID="lbCompanyNameSort" OnClick="lbUserSearchSort_Click" CommandArgument="CompanyName" Width="100%" Font-Underline="false">
											<span style="float:left;"><%: ReplaceTag("@@User.company_name.name@@")%></span>
											<span style="float:right"><asp:Label runat="server" ID="lCompanyNameIconSort"></asp:Label></span>
										</asp:LinkButton>
									</td>
									<td width="11%" nowrap="nowrap">
										<asp:LinkButton runat="server" ID="lbCompanyPostNameSort" OnClick="lbUserSearchSort_Click" CommandArgument="CompanyPostName" Width="100%" Font-Underline="false">
											<span style="float:left;"><%: ReplaceTag("@@User.company_post_name.name@@")%></span>
											<span style="float:right"><asp:Label runat="server" ID="lCompanyPostNameIconSort"></asp:Label></span>
										</asp:LinkButton>
									</td>
								<% } %>
								<td width="18%" nowrap="nowrap">
									<asp:LinkButton runat="server" ID="lbMailAddressSort" OnClick="lbUserSearchSort_Click" CommandArgument="MailAddress" Width="100%" Font-Underline="false">
										<span style="float:left;">メールアドレス</span>
										<span style="float:right"><asp:Label runat="server" ID="lMailAddressIconSort"></asp:Label></span>
									</asp:LinkButton>
								</td>
								<td width="11%" nowrap="nowrap">
									<asp:LinkButton runat="server" ID="lbPhoneNumberSort" OnClick="lbUserSearchSort_Click" CommandArgument="PhoneNumber" Width="100%" Font-Underline="false">
										<span style="float:left;">電話番号</span>
										<span style="float:right"><asp:Label runat="server" ID="lPhoneNumberIconSort"></asp:Label></span>
									</asp:LinkButton>
								</td>
								<td width="11%" nowrap="nowrap">
									<asp:LinkButton runat="server" ID="lbStreetAddressSort" OnClick="lbUserSearchSort_Click" CommandArgument="StreetAddress" Width="100%" Font-Underline="false">
										<span style="float:left;">住所</span>
										<span style="float:right"><asp:Label runat="server" ID="lStreetAddressIconSort"></asp:Label></span>
									</asp:LinkButton>
								</td>
								<td width="9%" nowrap="nowrap">
									<asp:LinkButton runat="server" ID="lbManagementLevelSort" OnClick="lbUserSearchSort_Click" CommandArgument="ManagementLevel" Width="100%" Font-Underline="false">
										<span style="float:left;">管理レベル</span>
										<span style="float:right"><asp:Label runat="server" ID="lManagementLevelIconSort"></asp:Label></span>
									</asp:LinkButton>
								</td>
								<td width="11%" nowrap="nowrap">
									<asp:LinkButton runat="server" ID="lbRegisteredDateSort" OnClick="lbUserSearchSort_Click" CommandArgument="RegisteredDate" Width="100%" Font-Underline="false">
										<span style="float:left;">登録日時</span>
										<span style="float:right"><asp:Label runat="server" ID="lRegisteredDateIconSort"></asp:Label></span>
									</asp:LinkButton>
								</td>
							</tr>
						</table>
					</div>
					<div class="y_scrollable div_table_data" style="HEIGHT:160px; background-color:#CCC">
						<table id="user_list" class="list_table_min">
							<asp:Repeater ID="rUserSearchResult" runat="server">
							<HeaderTemplate>
							</HeaderTemplate>
							<ItemTemplate>
								<tr class="<%# "oplist_item_bg" + (Container.ItemIndex % 2 + 1) %>" onmouseover="listselect_mover_userlist(this)" onmouseout="listselect_mout_userlist(this, <%# Container.ItemIndex % 2 %>)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick_userlist(this, <%# Container.ItemIndex % 2 %>, '<%# Eval(Constants.FIELD_USER_USER_ID)%>')">
									<td width="9%"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_USER_ID)) %></td>
									<td width="9%" nowrap="nowrap"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_USER_KBN)) %></td>
									<td width="11%" title="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_NAME)) %>"><%# AbbreviateString(WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_NAME)), 10) %></td>
									<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
										<td width="11%" title="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_COMPANY_NAME)) %>"><%# AbbreviateString(WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_COMPANY_NAME)), 10) %></td>
										<td width="11%" title="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_COMPANY_POST_NAME)) %>"><%# AbbreviateString(WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_COMPANY_POST_NAME)), 10) %></td>
									<% } %>
									<td width="18%" title="<%# WebSanitizer.HtmlEncode((Eval(Constants.FIELD_USER_MAIL_ADDR) + "\r\n" + Eval(Constants.FIELD_USER_MAIL_ADDR2)).Trim()) %>"><%# WebSanitizer.HtmlEncode(AbbreviateString((Eval(Constants.FIELD_USER_MAIL_ADDR) + " " + Eval(Constants.FIELD_USER_MAIL_ADDR2)).Split(' ')[0].Trim(), 20)) %></td>
									<td width="11%" title="<%# WebSanitizer.HtmlEncode((Eval(Constants.FIELD_USER_TEL1)  + "\r\n" + Eval(Constants.FIELD_USER_TEL2))).Trim() %>"><%# WebSanitizer.HtmlEncode((Eval(Constants.FIELD_USER_TEL1) + " " + Eval(Constants.FIELD_USER_TEL2)).Split(' ')[0].Trim()) %></td>
									<td width="11%" title="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_ADDR)) %>"><%# AbbreviateString(WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_ADDR)), 10) %></td>
									<td width="9%" title="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME)) %>"><%# AbbreviateString(WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME)), 10) %></td>
									<td width="11%"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_USER_DATE_CREATED), DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %></td>
								</tr>
							</ItemTemplate>
							</asp:Repeater>
						</table>
					</div>
				</div>
				
				<script type="text/javascript">
					// Get width of scroll div header
					function GetWidthHeaderScroll() {
						var widthIncludeScroll = $(".div_table_data").width();
						var widthWithoutScroll = $(".div_table_data .list_table_min").width();

						var widthScroll = (widthIncludeScroll - widthWithoutScroll);
						switch(widthScroll)
						{
						case 6:
						case 7:
						case 10:
						case 12.5:
						case 17.5:
							widthScroll -= 0.5;
							break;

						case 8:
						case 9:
							widthScroll += 0.5;
							break;

						case 3: widthScroll = 3.3;
							break;

						case 4: widthScroll = 4.2;
							break;

						case 12: widthScroll -= 1;
							break;
						}

						if ((widthScroll > 20) && (widthScroll < 21)) widthScroll = 19;
						if ((widthScroll > 15) && (widthScroll < 16)) widthScroll += 0.5;

						return widthScroll;
					}

					jQuery(function ($) {
						$(".div_table_header").attr("style", "padding-right:" + GetWidthHeaderScroll() + "px");

						$(window).resize(function () {
							$(".div_table_header").attr("style", "padding-right:" + GetWidthHeaderScroll() + "px");
						});
					});
			</script>

				<div id="divUserListOpener" style="margin-top:2px">
					<table>
					<thead>
					<tr>
						<td id="tdUserListOpener" class="toggle" style="text-align:center">
							<img id="imgUserListOpen" src="../../Images/Cs/arrow_down.png" alt="開く" />
							<img id="imgUserListClose" src="../../Images/Cs/arrow_up.png" alt="閉じる" />
						</td>
					</tr>
					</thead>
					</table>
				</div>
			</div>
			</td>
			</tr>
			</table>
		</div>
	</td>
	</tr>
	</table>
	<div id="win-size-grip6"><img src ="../../Images/Cs/hsizegrip.png" ></div>
</div>

<asp:UpdatePanel ID="upUserList" runat="server">
<ContentTemplate>
<asp:LinkButton ID="lbSelectUser" runat="server" OnClick="lbSelectUser_Click"></asp:LinkButton>
<asp:HiddenField ID="hfSelectedUserId" runat="server" />

<!-- ユーザー詳細 -->
<div id="divUserDetail" visible="false" runat="server">
	<table style="WIDTH:99%;float:left;margin:2px 2px 2px 4px" border="0" cellspacing="0" cellpadding="0">
	<tr valign="top">
		<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr valign="top">
	<td>
		<!-- ユーザー詳細 -->
		<div class="datagrid">
		<table>
		<thead>
		<tr>
		<th colspan="8">顧客情報</th>
		</tr>
		</thead>
		<tbody>
		<tr>
			<td class="alt" align="left" width="8%">ユーザーID</td>
			<td align="left" width="10%">
				<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
					<a href="#" onclick="javascript:open_window('<%= WebSanitizer.HtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateUserDetailUrl(hfSelectedUserId.Value))) %>','userdetail','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
						
						<asp:Literal ID="lUserId1" runat="server"></asp:Literal></a>
				<% } else { %>
					<asp:Literal ID="lUserId2" runat="server" />
				<% } %>

			</td>
			<td class="alt" align="left" width="8%">会員ランク</td>
			<td align="left" width="27%"><asp:Literal ID="lMemberRank" runat="server"></asp:Literal></td>
			<td class="alt" align="left" width="10%">サイト</td>
			<td align="left" width="17%"><asp:Literal ID="lSiteName" runat="server"></asp:Literal></td>
			<td class="alt" align="left" width="8%">顧客区分</td>
			<td align="left" width="12%"><asp:Literal ID="lUserKbn" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td class="alt" align="left" rowspan="2">氏名</td>
			<td align="left" style="border-bottom-style: dotted;"><asp:Literal ID="lUserNameKana" runat="server"></asp:Literal></td>
			<td class="alt" align="left" rowspan="2">住所</td>
			<td align="left" style="border-bottom-style: dotted;"><asp:Literal ID="lUserAddrKana" runat="server"></asp:Literal></td>
			<td class="alt" align="left" rowspan="2">メールアドレス</td>
			<td align="left"rowspan="2"><asp:Literal ID="lUserMails" runat="server"></asp:Literal><uc:ErrorPoint ID="ucErrorPoint" runat="server" /></td>
			<td class="alt" align="left" rowspan="2">電話番号</td>
			<td align="left" rowspan="2"><asp:Literal ID="lUserTel" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td align="left"><asp:Literal ID="lUserName" runat="server"></asp:Literal></td>
			<td align="left"><asp:Literal ID="lUserAddr" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td class="alt" align="left">特記欄</td>
			<td colspan="3" align="left"><asp:Literal ID="lUserMemo" runat="server"></asp:Literal></td>
			<td class="alt" align="left">ユーザー管理レベル</td>
			<td align="left"><asp:Literal ID="lUserManagementLevel" runat="server"></asp:Literal></td>
			<td class="alt" align="left">生年月日</td>
			<td align="left"><asp:Literal ID="lUserBirth" runat="server"></asp:Literal><asp:Literal ID="lUserAge" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td class="alt" align="left">利用不可な決済情報</td>
			<td colspan="7" align="left"><asp:Literal ID="lPaymentUserManagementLevelNotUse" runat="server"></asp:Literal></td>
		</tr>
		<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
		<tr>
			<td class="alt" align="left">アクセス<br/>国ISOコード</td>
			<td colspan="7" align="left"><asp:Literal ID="lAccessCountryIsoCode" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td class="alt" align="left">表示<br/>言語コード</td>
			<td colspan="3" align="left"><asp:Literal ID="lDispLanguageCode" runat="server"></asp:Literal></td>
			<td class="alt" align="left">表示<br/>言語ロケールID</td>
			<td colspan="3" align="left"><asp:Literal ID="lDispLanguageLocalId" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td class="alt" align="left">表示<br/>通貨コード</td>
			<td colspan="3" align="left"><asp:Literal ID="lDispCurrencyCode" runat="server"></asp:Literal></td>
			<td class="alt" align="left">表示<br/>通貨ロケールID</td>
			<td colspan="3" align="left"><asp:Literal ID="lDispCurrencyLocalId" runat="server"></asp:Literal></td>
		</tr>
		<% } %>
		</tbody>
		</table>
		</div>
		<img height="2" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
	
	<!-- ユーザーサブ情報エリア -->
		<div>
			<div class="tab_title tab_title_left" style="overflow: visible">
				<% lbDispUserRelation.CssClass = (this.UserSubInfoDispMode == UserSubInfoDispModeType.UserRelation) ? "active" : ""; %>
				<% lbDispUserExtends.CssClass = (this.UserSubInfoDispMode == UserSubInfoDispModeType.UserExtends) ? "active" : ""; %>
				<% lbDispUserAttribute.CssClass = (this.UserSubInfoDispMode == UserSubInfoDispModeType.UserAttribute) ? "active" : ""; %>
				<ol class="toc" style="overflow: visible">
					<li><asp:LinkButton ID="lbDispUserRelation" runat="server" OnClick="lbDispUserRelation_Click"><span>ユーザーリレーション情報</span></asp:LinkButton></li>
					<li><asp:LinkButton ID="lbDispUserExtends" runat="server" OnClick="lbDispUserExtends_Click"><span>ユーザー拡張項目情報</span></asp:LinkButton></li>
					<% if (Constants.USER_ATTRIBUTE_OPTION_ENABLE) { %>
					<li><asp:LinkButton ID="lbDispUserAttribute" runat="server" OnClick="lbDispUserAttribute_Click"><span>ユーザー属性情報 <small>（集計日：<asp:Literal ID="lAttributeCalculateDate" runat="server"></asp:Literal>）</small></span></asp:LinkButton></li>
					<% } %>
					<li>
						<div style="margin-left: 7px">
						<table>
							<asp:Repeater ID="rExternalLinkList" runat="server">
								<ItemTemplate>
									<tr>
										<div id="ExternalLink" class="externallink">
											<input type="button" class="externallink-title"
												value="<%# WebSanitizer.HtmlEncode((((CsExternalLinkModel)Container.DataItem).LinkTitle.Length >= 8) ?
													AbbreviateString(((CsExternalLinkModel)Container.DataItem).LinkTitle,6) : ((CsExternalLinkModel)Container.DataItem).LinkTitle).TrimStart() %>"
												onclick="javascript:window.open('<%#: WebSanitizer.HtmlEncode(GetReplacedLink(((CsExternalLinkModel)Container.DataItem).LinkUrl , hfSelectedUserId.Value.Trim())) %>')" />
											<div class="externallink-text">
												<div class="externallink-text-memo">
													<h3><%# WebSanitizer.HtmlEncode(((CsExternalLinkModel)Container.DataItem).LinkTitle) %></h3>
													<hr />
													<%# WebSanitizer.HtmlEncode(((CsExternalLinkModel)Container.DataItem).LinkMemo) %>
												</div>
											</div>
										</div>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
						</table>
						</div>
					</li>
					<li>
						<div class="popup" id="divExterExternalLink" runat="server" visible="False">
							<div id="open">
								<div><img src="../../Images/Cs/arrow_down.png" /></div>
								<div class="popup-body">
									<div class="popup-wrap">
										<table>
											<asp:Repeater ID="rExtraExternalLinkList" runat="server">
												<ItemTemplate>
													<%# (((Container.ItemIndex + Constants.CONST_DISP_EXTERNAL_LINK_BUTTON) % Constants.CONST_DISP_EXTERNAL_LINK_BUTTON) == 0) ? "<tr>" : string.Empty %>
													<td>
														<div class="externallink">
															<input type="button" class="externallink-title"
																value="<%#  WebSanitizer.HtmlEncode((((CsExternalLinkModel)Container.DataItem).LinkTitle.Length >= 8) ?
																	AbbreviateString(((CsExternalLinkModel)Container.DataItem).LinkTitle,6) : ((CsExternalLinkModel)Container.DataItem).LinkTitle).TrimStart() %>"
																onclick="javascript:window.open('<%#: WebSanitizer.HtmlEncode(GetReplacedLink(((CsExternalLinkModel)Container.DataItem).LinkUrl , hfSelectedUserId.Value.Trim())) %>')" />
															<div class="externallink-text">
																<div class="externallink-text-memo">
																	<h3><%# WebSanitizer.HtmlEncode(((CsExternalLinkModel)Container.DataItem).LinkTitle) %></h3>
																	<hr />
																	<%# WebSanitizer.HtmlEncode(((CsExternalLinkModel)Container.DataItem).LinkMemo) %>
																</div>
															</div>
														</div>
													</td>
													<%# (((Container.ItemIndex + Constants.CONST_DISP_EXTERNAL_LINK_BUTTON) % Constants.CONST_DISP_EXTERNAL_LINK_BUTTON) == 6) ? "</tr>" : string.Empty %>
												</ItemTemplate>
											</asp:Repeater>
										</table>
									</div>
								</div>
							</div>
						</div>
					</li>
				</ol>
			</div>
		</div>
		<!-- ユーザーリレーション情報 -->
		<% divUserDetailRelationInfo.Visible = (this.UserSubInfoDispMode == UserSubInfoDispModeType.UserRelation); %>
		<div id="divUserDetailRelationInfo" class="datagrid" runat="server">
		<table>
		<tr>
			<td width="15%" class="alt">登録日時</td>
			<td width="18%"><asp:Literal ID="lUserDateCreated" runat="server"></asp:Literal></td>
			<td width="15%" class="alt">広告コード（登録時）</td>
			<td width="18%"><asp:Literal ID="lUserAdvCodeFirst" runat="server"></asp:Literal></td>
			<td width="15%" class="alt">お知らせメールの配信希望</td>
			<td width="18%"><asp:Literal ID="lUserMailFlg" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td class="alt">初回購入日</td>
			<td><asp:Literal ID="lUserFirstOrderDate" runat="server"></asp:Literal>
				<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
					（定期：<asp:Literal ID="lUserFirstOrderDateFixedPurchase" runat="server"></asp:Literal>）
				<%} %>
			</td>
			<td class="alt">購入回数</td>
			<td>
				<asp:Literal ID="lUserOrderCount" runat="server"></asp:Literal>回
				<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
					（定期：<asp:Literal ID="lUserOrderCountFixedPurchase" runat="server"></asp:Literal>回）
				<%} %>
			</td>
			<td class="alt">退会日</td>
			<td>
				<asp:Literal ID="lUserWithdrawaledDate" runat="server"></asp:Literal>
			</td>
		</tr>
		<tr>
			<td class="alt">最終購入日</td>
			<td><asp:Literal ID="lUserLastOrderDate" runat="server"></asp:Literal>
				<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
					（定期：<asp:Literal ID="lUserLastOrderDateFixedPurchase" runat="server"></asp:Literal>）
				<%} %>
			</td>
			<td class="alt">最終購入金額</td>
			<td><asp:Literal ID="lUserLastOrderPrice" runat="server"></asp:Literal>
				<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
					（定期：<asp:Literal ID="lUserLastOrderPriceFixedPurchase" runat="server"></asp:Literal>）
				<%} %>
			</td>
			<td class="alt">年間累計購買金額</td>
			<td><asp:Literal ID="lUserOrderPriceYearTotal" runat="server"></asp:Literal>
				<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
					（定期：<asp:Literal ID="lUserOrderPriceYearTotalFixedPurchase" runat="server"></asp:Literal>）
				<%} %>
			</td>
		</tr>
		</table>
		</div>

		<!-- ユーザー拡張項目情報 -->
		<% divUserDetailExtendsInfo.Visible = (this.UserSubInfoDispMode == UserSubInfoDispModeType.UserExtends); %>
		<div id="divUserDetailExtendsInfo" class="datagrid" runat="server">
		<table>
			<asp:Repeater id="rUserExtendList" Runat="server">
			<HeaderTemplate>
				<%# ((List<KeyValuePair<string, string>>)((Repeater)Container.Parent).DataSource).Count == 0 ? "<tr><td colspan=\"4\">拡張項目情報はありません。</td></tr>" : ""%>
			</HeaderTemplate>
			<ItemTemplate>
				<%# Container.ItemIndex%4 == 0 ? "<tr>" : ""%>
					<td width="12%" class="alt"><%# WebSanitizer.HtmlEncode(((KeyValuePair<string, string>)Container.DataItem).Key) %></td>
					<td width="13%"><%# WebSanitizer.HtmlEncode(((KeyValuePair<string, string>)Container.DataItem).Value) %></td>
				<%# Container.ItemIndex%4 == 3 ? "</tr>" : ""%>
			</ItemTemplate>
			</asp:Repeater>
		</table>
		</div>
		
		<!-- ユーザー属性情報 -->
		<% if (Constants.USER_ATTRIBUTE_OPTION_ENABLE) { %>
		<% divUserDetailAttributeInfo.Visible = (this.UserSubInfoDispMode == UserSubInfoDispModeType.UserAttribute); %>
		<div id="divUserDetailAttributeInfo" class="datagrid" runat="server">
		<div id="divUserDetailAttributeInfoInner" runat="server">
		<table>
		<tr>
			<td class="alt">最終購入日</td>
			<td><asp:Literal ID="lLastOrderDate" runat="server"></asp:Literal></td>
			<td class="alt">２回目購入日</td>
			<td><asp:Literal ID="lSecondOrderDate" runat="server"></asp:Literal></td>
			<td class="alt">初回購入日</td>
			<td><asp:Literal ID="lFirstOrderDate" runat="server"></asp:Literal></td>
			<td colspan="2"></td>
		</tr>
		<tr>
			<%if (Constants.CPM_OPTION_ENABLED) { %>
			<td class="alt">CPMクラスタ</td>
			<td><asp:Literal ID="lCpmCluster" runat="server"></asp:Literal><br/>
				<asp:Literal ID="lCpmClusterBefore" runat="server"></asp:Literal></td>
			<td class="alt">CPMクラスタ変更日</td>
			<td><asp:Literal ID="lCpmClusterChangedDate" runat="server"></asp:Literal></td>
			<%} %>
			<td class="alt">離脱期間(日)<br /><small>（最終購入から経った期間）</small></td>
			<td><asp:Literal ID="lAwayDays" runat="server"></asp:Literal> 日</td>
			<td class="alt">在籍期間(日)<br /><small>（初回購入から最終購入までの期間）</small></td>
			<td><asp:Literal ID="lEnrollmentDays" runat="server"></asp:Literal> 日</td>
			<%if (Constants.CPM_OPTION_ENABLED == false) { %>
			<td colspan="4"></td>
			<%} %>
		</tr>
		<tr>
			<td width="14%" class="alt">累計購入金額)<br />（注文基準・全体）</td>
			<td width="11%"><asp:Literal ID="lOrderAmountOrderAll" runat="server"></asp:Literal></td>
			<td class="alt">累計購入回数)<br />（注文基準・全体）</td>
			<td><asp:Literal ID="lOrderCountOrderAll" runat="server"></asp:Literal> 回</td>
			<td width="14%" class="alt">累計購入金額)<br />（出荷基準・全体）</td>
			<td width="11%"><asp:Literal ID="lOrderAmountShipAll" runat="server"></asp:Literal></td>
			<td width="14%" class="alt">累計購入回数)<br />（出荷基準・全体）</td>
			<td width="11%"><asp:Literal ID="lOrderCountShipAll" runat="server"></asp:Literal> 回</td>
		</tr>
		<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
		<tr>
			<td width="14%" class="alt">累計購入金額)<br />（注文基準・定期のみ）</td>
			<td width="11%"><asp:Literal ID="lOrderAmountOrderFp" runat="server"></asp:Literal></td>
			<td class="alt">累計購入回数)<br />（注文基準・定期のみ）</td>
			<td><asp:Literal ID="lOrderCountOrderFp" runat="server"></asp:Literal> 回</td>
			<td width="14%" class="alt">累計購入金額)<br />（出荷基準・定期のみ）</td>
			<td width="11%"><asp:Literal ID="lOrderAmountShipFp" runat="server"></asp:Literal></td>
			<td class="alt">累計購入回数)<br />（出荷基準・定期のみ）</td>
			<td><asp:Literal ID="lOrderCountShipFp" runat="server"></asp:Literal> 回</td>
		</tr>
		<%} %>
		</table>
		</div>
			<div id="divUserDetailAttributeNoInfoInner" class="datagrid" runat="server">
				<table>
					<tr>
						<td>未集計のため表示できません。</td>
					</tr>
				</table>
			</div>
		</div>
		<% } %>
	</td>
	</tr>
	</table>
</div>

<asp:LinkButton ID="lbLoadHistories" runat="server" OnClick="lbLoadHistories_Click"></asp:LinkButton>
<asp:HiddenField ID="hfLoadHistoryFlg" runat="server" Value="0" />
<!-- ユーザー履歴 -->
<div id="divUserHistory" visible="false" runat="server">
	<table  style="WIDTH:99%;float:left;margin:2px 2px 2px 4px" border="0" cellspacing="0" cellpadding="0">
	<tr valign="top">
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr valign="top">
		<td>
			<div class="tab_title tab_title_left">
			<% lbDispUserHistoryAll.CssClass = (this.UserHistoryDispMode == UserHistoryModeType.All) ? "active" : ""; %>
			<% lbDispUserHistoryOrder.CssClass = (this.UserHistoryDispMode == UserHistoryModeType.Order) ? "active" : ""; %>
			<% lbDispUserHistoryFixedPurchase.CssClass = (this.UserHistoryDispMode == UserHistoryModeType.FixedPurchase) ? "active" : ""; %>
			<% lbDispUserHistoryMessage.CssClass = (this.UserHistoryDispMode == UserHistoryModeType.Message) ? "active" : ""; %>
			<% lbDispMailSendLog.CssClass = (this.UserHistoryDispMode == UserHistoryModeType.MailSendLog) ? "active" : ""; %>
			<% if (Constants.DM_SHIPPING_HISTORY_OPTION_ENABLED) { %>
				<% lbDispDmShippingHistory.CssClass = (this.UserHistoryDispMode == UserHistoryModeType.DmShippingHistory) ? "active" : ""; %>
			<% } %>
			<ol class="toc">
			<li><asp:LinkButton ID="lbDispUserHistoryAll" runat="server" OnClick="lbDispUserHistoryAll_Click" ><span>履歴情報（全体）</span></asp:LinkButton></li>
			<li><asp:LinkButton ID="lbDispUserHistoryOrder" runat="server" OnClick="lbDispUserHistoryOrder_Click"><span>履歴情報（注文のみ）</span></asp:LinkButton></li>
			<li><asp:LinkButton ID="lbDispUserHistoryFixedPurchase" runat="server" OnClick="lbDispUserHistoryFixedPurchase_Click"><span>履歴情報（定期情報のみ）</span></asp:LinkButton></li>
			<li><asp:LinkButton ID="lbDispUserHistoryMessage" runat="server" OnClick="lbDispUserHistoryMessage_Click"><span>履歴情報（メッセージのみ）</span></asp:LinkButton></li>
			<li><asp:LinkButton ID="lbDispMailSendLog" runat="server" OnClick="lbDispMailSendLog_Click"><span>履歴情報（送信済みメールのみ）</span></asp:LinkButton></li>
			<% if (Constants.DM_SHIPPING_HISTORY_OPTION_ENABLED) { %>
				<li><asp:LinkButton ID="lbDispDmShippingHistory" runat="server" OnClick="lbDispDmShippingHistory_Click"><span>DM発送履歴</span></asp:LinkButton></li>
			<% } %>
			</ol>
			</div>

			<div class="dataresult">
				<div id="divHistoryHead" runat="server">
					<table>
					<thead>
					<tr>
						<th>履歴一覧 （該当<asp:Literal ID="lUserHistoryCount" runat="server"></asp:Literal>件）</th>
					</tr>
					</thead>
					</table>
				</div>
				<div style="WIDTH:100%;float:left;margin:0px 0px 0px 0px">
				<div class="y_scrollable div_table_header">
				<table class="list_table_min" border="0" cellpadding="2" cellspacing="1" width="100%">
						<tr class="alt">
							<td width="110">日付</td>
							<td width="250">分類</td>
							<td width="0">内容</td>
							<td width="12"></td>
						</tr>
				</table>
				</div>
				<div class="y_scrollable div_table_data" style="HEIGHT:160px; background-color:#FFF">
				<img id="imgLoading" src="../../Images/Common/loading.gif" alt="Loading" runat="server" />
				<div id="divHistoryBody" runat="server">
					<table class="list_table_min" border="0" cellpadding="2" cellspacing="1" width="100%">
						<tbody>
						<asp:Repeater ID="rUserHistoryInfoAll" ItemType="w2.App.Common.Cs.UserHistory.UserHistoryBase" runat="server">
						<ItemTemplate>
							<tr title="<%#: GetHistoryDetail(((UserHistoryBase)Container.DataItem)) %>" class="<%# "oplist_item_bg" + (Container.ItemIndex % 2 + 1) %>" onmouseover="listselect_mover_userlist(this)" onmouseout="listselect_mout_userlist(this, <%# Container.ItemIndex % 2 %>)" onmousedown="listselect_mdown(this)" onclick="onclick_history('<%# Item.Url %>', <%# (((UserHistoryBase)Container.DataItem) is UserHistoryMessage) ? "true" : "false" %>)">
								<td width="110"><%#: (((UserHistoryBase)Container.DataItem) is UserHistoryDmShippingHistory) ? DateTimeUtility.ToStringFromRegion(Item.DateTime, DateTimeUtility.FormatType.ShortDate2Letter) : DateTimeUtility.ToStringFromRegion(Item.DateTime, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
								<td width="250"><%#: Constants.SUBSCRIPTION_BOX_OPTION_ENABLED ? (IsSubscriptionBoxOrder(((UserHistoryBase)Container.DataItem)) == false ? Item.KbnString : "定期情報(頒布会)") : Item.KbnString %></td>
								<td width="0">
									<span class="tooltipDetail">
										<span class="img_align">
											<%# GetHistoryIcon((UserHistoryBase)Container.DataItem) %>
										</span>
										<span class="img_align" title="<%#: GetReadFlg((UserHistoryBase)Container.DataItem) %>" visible="<%# IsMailSendLog((UserHistoryBase)Container.DataItem) %>" runat="server">
											<asp:CheckBox ID="checkBoxReadFlg" runat="server" Enabled="false" CssClass="checkBoxZoomNormal" Checked="<%# IsCheckedReadFlg((UserHistoryBase)Container.DataItem) %>" />
										</span>
										<%#: GetHistoryContetns((UserHistoryBase)Container.DataItem) %>
										<span id="spUrgent" visible='<%# ((UserHistoryBase)Container.DataItem).MessageUrgencyFlg == Constants.FLG_CSMESSAGEREQUEST_URGENCY_URGENT %>' class="notice" runat="server">*</span>
									</span>
								</td>
							</tr>
						</ItemTemplate>
						</asp:Repeater>
						</tbody>
					</table>
				</div>
				</div>
				</div>
			</div>
		</td>
	</tr>
	</table>
</div>

</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
	// ページロード
	function bodyPageLoad(sender, /*Sys.ApplicationLoadEventArgs*/args) {
		// 非同期ポストバック時、履歴読み込みフラグが立っていたら読み込み処理
		if (args.get_isPartialLoad()) {
			if (document.getElementById('<%= hfLoadHistoryFlg.ClientID %>').value == "1") {
				// pageloadの二重押しチェックでキャンセルされないようタイマ処理でポストバック実行
				setTimeout(function() {
					__doPostBack("<%= lbLoadHistories.UniqueID %>", "");
				}, 1);
			}
		}
	}

	//----------------------------------------------------
	// リスト選択列保持用（一行選択時、該当列が選択済の場合は色を変えない）
	//----------------------------------------------------
	class_bgcolover = 'oplist_item_bg_over'; // マウスオーバー時の色
	class_bgcolout1 = 'oplist_item_bg1'; // マウスアウトしたときの色１
	class_bgcolout2 = 'oplist_item_bg2'; // マウスアウトしたときの色２
	class_bgcolout3 = 'oplist_item_bg3'; // マウスアウトしたときの色３
	var class_bgcolouts = new Array(class_bgcolout1, class_bgcolout2, class_bgcolout3);
	class_bgcolclck = 'oplist_item_bg_click'; // マウスクリックしたときの色

	var selected_tr = "";
	var selected_before_style = "";
	var single_select = false;
	function listselect_mover_userlist(obj)
	{
		if ((single_select == false) && (obj != selected_tr)) listselect_mover(obj);
	}
	function listselect_mout_userlist(obj, lineIdx)
	{
		if ((single_select == false) && (obj != selected_tr)) listselect_mout(obj, lineIdx);
	}
	function listselect_mclick_userlist(obj, lineIdx, userId)
	{
		listselect_mclick_userlist_inner(obj, userId, class_bgcolouts[lineIdx]);
	}
	function listselect_mclick_userlist_inner(obj, userId, class_bgcolout)
	{
		// クリック失敗なら抜ける
		if (listselect_mclick(obj) == false) return;
		// 履歴ロード中は抜ける（ポストバックが二重実行されるとフリーズするため）
		if (document.getElementById('<%= hfLoadHistoryFlg.ClientID %>').value == "1") return;
		
		// 選択済み→再度選択済みを選択の場合は選択解除
		if (obj == selected_tr) {
			selected_tr = "";
			obj.className = class_bgcolover;   // ←★この1行を追加
			// イベント実行
			document.getElementById('<%= hfSelectedUserId.ClientID %>').value = "";
			__doPostBack("<%= lbSelectUser.UniqueID %>", "");
			return;
		} 

		// 以前の選択列の色を戻す
		selected_tr.className = selected_before_style;
		selected_tr = obj;
		selected_before_style = class_bgcolout;

		// イベント実行
		document.getElementById('<%= hfSelectedUserId.ClientID %>').value = userId;
		__doPostBack("<%= lbSelectUser.UniqueID %>", "");
	}
	// 履歴クリック処理
	function onclick_history(url, isLarge)
	{
		open_window(url, 'history', (isLarge ? 'width=1200,height=770' : 'width=1000,height=600') + ',top=110,left=380,status=NO,scrollbars=yes');
	}

	// ユーザーリストのアコーディオン
	$(function()
	{
		// マウスイン／アウト処理
		$("#tdUserListOpener").hover(function() {
			$("#tdUserListOpener.toggle").addClass("togglehlight");
		}, function() {
			$("#tdUserListOpener.toggle").removeClass("togglehlight");
		});

		// クリック
		$("#tdUserListOpener").click(function(event) {
			// 開く
			if ($("#divUserListInner").css("display") == "none") {
				$("#divUserListInner").show(300);
				$("#imgUserListOpen").hide();
				$("#imgUserListClose").show();
				$("#divUserListOpener").removeClass("toggleclose");
				$("#divUserListOpener").addClass("toggleopen");
			}
			// 閉じる
			else {
				$("#divUserListInner").hide(300);
				$("#tdUserListOpener").removeClass("togglehlight");
				$("#imgUserListOpen").show();
				$("#imgUserListClose").hide();
				$("#divUserListOpener").addClass("toggleclose");
				$("#divUserListOpener").removeClass("toggleopen");
			}
		});
		$("#imgUserListOpen").hide();
		$("#imgUserListClose").show();
		$("#divUserListOpener").removeClass("toggleclose");
		$("#divUserListOpener").addClass("toggleopen");

	});

	//　ユーザーリストの先頭選択
	function select_user_list_first(userId)
	{
		// Set row select
		var rowSelect = document.getElementById('user_list').rows[0];
		rowSelect.className = class_bgcolclck;
		selected_tr = rowSelect;

		// Call select user click
		document.getElementById('<%= hfSelectedUserId.ClientID %>').value = userId;
		__doPostBack("<%= lbSelectUser.UniqueID %>", "");
	}
	
	$('.y_scrollable.div_table_data').resizable2({
		handleSelector: "#win-size-grip6",
		resizeWidth: false,
		onDragStart: function (e, $el, opt) {
			$el.css({ 'height': $el.height(), 'max-height': '<%= Constants.VARIABLE_MAXIMUM_SIZE %>px' });
		},
		onDragEnd: function (e, $el, opt) {
			setCookie("<%=Constants.COOKIE_KEY_USER_SEARCH_HEIGHT%>", $el.height(), { expires: 1000 });
		}
	});
	$('.y_scrollable.div_table_data').css({ 'height': 'auto', 'max-height': '<%= CookieManager.GetValue(Constants.COOKIE_KEY_USER_SEARCH_HEIGHT) ?? Constants.USER_SEARCH_DEFAULT_HEIGHT_SIZE %>px' });
</script>

</asp:Content>

