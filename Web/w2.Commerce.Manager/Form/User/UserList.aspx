<%--
=========================================================================================================
  Module      : ユーザー情報一覧ページ(UserList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserList.aspx.cs" Inherits="Form_User_UserList" %>
<%@ Import Namespace="System.Globalization" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<link rel="stylesheet" type="text/css" href="../../Css/hide-field-button-style.css">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">ユーザー情報</h1>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border cmn-section" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0" class="wide-content">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_box_bg">
												<table class="search_table cmn-form" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															ユーザーID</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbUserId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															顧客区分</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlUserKbn" runat="server" CssClass="search_item_width"></asp:DropDownList></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															並び順</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width">
																<asp:ListItem Value="8">ユーザーID/昇順</asp:ListItem>
																<asp:ListItem Value="9">ユーザーID/降順</asp:ListItem>
																<asp:ListItem Value="0">氏名/昇順</asp:ListItem>
																<asp:ListItem Value="1">氏名/降順</asp:ListItem>
																<asp:ListItem Value="2">氏名（かな）/昇順</asp:ListItem>
																<asp:ListItem Value="3">氏名（かな）/降順</asp:ListItem>
																<asp:ListItem Value="4">作成日/昇順</asp:ListItem>
																<asp:ListItem Value="5">作成日/降順</asp:ListItem>
																<asp:ListItem Value="6">更新日/昇順</asp:ListItem>
																<asp:ListItem Value="7">更新日/降順</asp:ListItem>
															</asp:DropDownList></td>
														<td class="search_btn_bg" width="83" rowspan="<%= 10 + (Constants.DISPLAY_CORPORATION_ENABLED  ? 1 : 0) + (Constants.MALLCOOPERATION_OPTION_ENABLED ? 1 : 0) + (Constants.USERINTEGRATION_OPTION_ENABLED ? 1 : 0) + (Constants.GLOBAL_OPTION_ENABLE ? 1 : 0) %>">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_LIST %>">クリア</a>&nbsp;
																<a href="javascript:Reset();">リセット</a>
																<asp:LinkButton id="lbDeleteEScottKaiin" Runat="server" OnClick="lbDeleteEScottKaiin_Click">e-SCOTT削除会員</asp:LinkButton>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															<%: ReplaceTag("@@User.name.name@@") %></td>
														<td class="search_item_bg" width="130" colspan="<%: this.IsShippingCountryAvailableJp ? 1 : 3 %>">
															<asp:TextBox id="tbName" runat="server" Width="125"></asp:TextBox></td>

														<% if (this.IsShippingCountryAvailableJp) { %>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															<%: ReplaceTag("@@User.name_kana.name@@") %></td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbNameKana" runat="server" Width="125"></asp:TextBox></td>
														<% } %>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															メールアドレス</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbMailAddr" runat="server" Width="125"></asp:TextBox></td>
													</tr>
													<tr id="hide-field_Tel_MailFlg_DelFlg" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															<%: ReplaceTag("@@User.tel1.name@@") %></td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbTel" runat="server" Width="125"></asp:TextBox></td>

														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															メール希望</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlMailFlg" runat="server" CssClass="search_item_width"></asp:DropDownList></td>

															<td class="search_title_bg" width="95">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																退会者</td>
															<td class="search_item_bg" width="130">
																<asp:CheckBox ID="cbDelFlg" runat="server" Text="退会者を含む" Checked="false"></asp:CheckBox></td>
													</tr>
													<tr id="hide-field_Zip_Addr_UserEasyRegisterFlag" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															<%: ReplaceTag("@@User.zip.name@@") %></td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbZip" runat="server" Width="125"></asp:TextBox></td>

														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															住所</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbAddr" runat="server" width="125"></asp:TextBox></td>

														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															かんたん会員</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlUserEasyRegisterFlg" runat="server" width="125" CssClass="search_item_width"></asp:DropDownList>
														</td>
													</tr>

													<%--▽ グローバル対応オプションが有効の場合 ▽--%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<tr id="hide-field_CountryIsoCode" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															国ISOコード</td>
														<td class="search_item_bg" width="130" colspan="5">
															<asp:DropDownList id="ddlCountryIsoCode" runat="server" CssClass="search_item_width"></asp:DropDownList></td>
													</tr>
													<% } %>
													<%-- △ グローバル対応オプションが有効の場合 △--%>
													<!-- 企業名・部署名 -->
													<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
													<tr id="hide-field_CompanyName" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" /><%: ReplaceTag("@@User.company_name.name@@")%></td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="thCompanyName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" /><%: ReplaceTag("@@User.company_post_name.name@@")%></td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="thCompanyPostName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95"></td>
														<td class="search_item_bg" width="130"></td>
													</tr>
													<%} %>

													<%--▽ モール連携オプションが有効の場合 ▽--%>
													<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
													<tr id="hide-field_SiteName" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															サイト</td>
														<td class="search_item_bg" width="130" colspan="5">
															<asp:DropDownList id="ddlSiteName" runat="server" CssClass="search_item_width"></asp:DropDownList></td>
													</tr>
													<% } %>
													<%--△ モール連携オプションが有効の場合 △--%>
													<%--▽ 会員ランクオプションかつ定期オプションが有効の場合 ▽--%>
													<% if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
													<tr id="hide-field_FixedPurchaseMemberFlg" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															定期/非定期</td>
														<td class="search_item_bg" width="130" colspan="5">
															<asp:DropDownList id="ddlFixedPurchaseMemberFlg" runat="server" CssClass="search_item_width"></asp:DropDownList></td>
													</tr>
													<% } %>
													<%--△ 会員ランクオプションかつ定期オプションが有効の場合 △--%>
													<tr id="hide-field_UserManagementSection" style="display: none;">
													<%--▽ メルアドログインの場合ログインID検索窓非表示 ▽--%>
													<% if (!(Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)){ %>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															ユーザー<br />&nbsp;&nbsp;&nbsp;ログインID</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbUserLoginId" runat="server" Width="125"></asp:TextBox></td>
													<% } %>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															ユーザー管理レベル</td>
													<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED){ %>
														<td class="search_item_bg" width="580" colspan="5">
													<% }else{ %>
														<td class="search_item_bg" width="130" colspan="3">
													<% } %>
															<asp:DropDownList id="ddlUserManagementLevelId" runat="server" CssClass="search_item_width"></asp:DropDownList>
															<asp:CheckBox ID="cbUserManagementLevelExclude" runat="server" Text="除外する" Checked="false"></asp:CheckBox>
														</td>
													</tr>
													<%--△ メルアドログインの場合ログインID検索窓非表示 △--%>
													<tr height="24px" id="hide-field_UpUserMemo" style="display: none;">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ユーザー特記欄</td>
														<td class="search_item_bg" colspan="5" valign="middle">
																<asp:DropDownList id="ddlUserMemoFlg" runat="server"></asp:DropDownList>
																<asp:TextBox ID="tbUserMemo" runat="server" Width="425px"></asp:TextBox>
														</td>
													</tr>
													<tr height="24px" id="hide-field_UserExtendSection" style="display: none;">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ユーザー拡張項目</td>
														<td class="search_item_bg" colspan="5" valign="middle">
																<asp:DropDownList id="ddlUserExtendName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUserExtend_SelectedIndexChanged"></asp:DropDownList>
																<asp:DropDownList id="ddlUserExtendFlg" Visible="false" runat="server"></asp:DropDownList>
																<asp:TextBox ID="tbUserExtendText" Visible="false" runat="server" Width="425px"></asp:TextBox>
																<asp:DropDownList id="ddlUserExtendText" Visible="false" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<%--▽ ユーザー統合オプションが有効の場合 ▽--%>
													<% if (Constants.USERINTEGRATION_OPTION_ENABLED) { %>
													<tr id="hide-field_UserIntegrationFlg" style="display: none;">
														<td class="search_title_bg" width="95" height="24px">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															統合ユーザー</td>
														<td class="search_item_bg" colspan="5">
															<asp:CheckBox ID="cbUserIntegrationFlg" runat="server" Text="統合された非代表ユーザーを含む" Checked="false"></asp:CheckBox></td>
													</tr>
													<% } %>
													<%--△ ユーザー統合オプションが有効の場合 △--%>
													<tr id="hide-field_UserDateCreated" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															作成日</td>
														<td class="search_item_bg" colspan="5">
															<div id="userDateCreated">
																<uc:DateTimePickerPeriodInput id="ucUserDateCreated" runat="server" IsNullStartDateTime="true" />
																<span class="search_btn_sub">(<a href="Javascript:SetToday('date_created');">今日</a>｜<a href="Javascript:SetThisMonth('date_created');">今月</a>)</span>
															</div>
														</td>
													</tr>
													<tr id="hide-field_UserDateChanged" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															更新日</td>
														<td class="search_item_bg" colspan="5">
															<div id="userDateChanged">
																<uc:DateTimePickerPeriodInput id="ucUserDateChanged" runat="server" IsNullStartDateTime="true" />
																<span class="search_btn_sub">(<a href="Javascript:SetToday('date_changed');">今日</a>｜<a href="Javascript:SetThisMonth('date_changed');">今月</a>)</span>
															</div>
														</td>
													</tr>
												</table>

													<div id="user-hide-search-field-slide-toggle" style="text-align: center;">
														<span id="check-toggle-text-user">全ての検索項目を表示</span>
														<span id="check-toggle-open">
															<span class="toggle-state-icon icon-arrow-down"/>
														</span>
													</div>
											<br/>
											</td>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="User" TableWidth="758" />
											</td>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr id="userListSearchResult">
		<td><h2 class="cmn-hed-h2" >ユーザー情報一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border cmn-section" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0" class="wide-content">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td style="height: 22px"><asp:Label id="lbPager1" Runat="server" OnClick="pagingBtn"></asp:Label></td>
														<td class="action_list_sp">
															<asp:Button ID="btnDefaultSettingTop" runat="server" Text="  初期設定  " OnClick="btnDefaultSettingTop_Click" />
															<%if (Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE){%>
																<% btnImportTargetList.OnClientClick = "javascript:open_window('" + ImportTargetListUrlCreator.Create(Constants.KBN_WINDOW_POPUP) + "','Import','width=850,height=370,top=120,left=420,status=no,scrollbars=yes');return false;"; %>
																<asp:Button ID="btnImportTargetList" runat="server" Text="  ターゲットリスト作成  " Enabled="false" EnableViewState="False" UseSubmitBehavior="False"/>
															<%} %>
															<asp:Button ID="btnInsert" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
														</td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<% if (this.IsNotSearchDefault) { %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_alert">
														<td><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<% } else { %>
										<tr>
											<td>
												<table class="list_table wide-content" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="100">ユーザーID</td>
														<%--▽ モール連携オプションが有効の場合 ▽--%>
														<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
														<td align="center" width="100">サイト</td>
														<% } %>
														<%--△ モール連携オプションが有効の場合 △--%>
														<td align="center" width="90">顧客区分</td>
														<td align="center" width="268">氏名</td>
														<td align="center" width="100">ユーザー<br />管理レベル</td>
														<td align="center" width="100">作成日</td>
														<td align="center" width="100">更新日</td>
													</tr>
													<asp:Repeater id="rList" ItemType="w2.Domain.User.Helper.UserSearchResult" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl(Item.UserId)) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Item.UserId) %></td>
																<%--▽ モール連携オプションが有効の場合 ▽--%>
																<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td align="center"><%# WebSanitizer.HtmlEncode(CreateSiteNameOnly(StringUtility.ToEmpty(Item.MallId), StringUtility.ToEmpty(Item.MallName)))%></td>
																<% } %>
																<%--△ モール連携オプションが有効の場合 △--%>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, Item.UserKbn)) %></td>
																<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(Item.Name) %>&nbsp;<%# WebSanitizer.HtmlEncode(Item.Symbol) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(GetUserManagementLevelName(Item.UserManagermentLevelId)) %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateChanged, DateTimeUtility.FormatType.ShortDate2Letter)%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" colspan="7" runat="server">
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<% } %>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															・氏名の後ろに「<%= Constants.USERSYMBOL_REPEATER %>」付のユーザーは注文を２回以上しているリピーターユーザー、「<%= Constants.USERSYMBOL_HAS_NOTE %>」付のユーザーは特記事項のある特記ユーザーを表します。<br />
															・ユーザー管理レベルを検索する際に、「除外する」にチェックを入れて検索すると、選択した管理レベル以外のユーザーを一覧に表示できます。
														</td>
													</tr>
												</table>
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

<script type="text/javascript">
<!--
	// 今日設定
	function SetToday(set_date) {
		// 作成日
		if (set_date == 'date_created') {
			document.getElementById('<%= ucUserDateCreated.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date) %>';
			document.getElementById('<%= ucUserDateCreated.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucUserDateCreated.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date) %>';
			document.getElementById('<%= ucUserDateCreated.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucUserDateCreated.ClientID %>');
		}
		// 更新日
		else if (set_date == 'date_changed') {
			document.getElementById('<%= ucUserDateChanged.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date) %>';
			document.getElementById('<%= ucUserDateChanged.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucUserDateChanged.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date) %>';
			document.getElementById('<%= ucUserDateChanged.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucUserDateChanged.ClientID %>');
		}
	}

	// 今月設定
	function SetThisMonth(set_date) {
		// 作成日
		if (set_date == 'date_created') {
			document.getElementById('<%= ucUserDateCreated.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucUserDateCreated.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucUserDateCreated.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucUserDateCreated.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucUserDateCreated.ClientID %>');
		}
		// 更新日
		else if (set_date == 'date_changed') {
			document.getElementById('<%= ucUserDateChanged.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucUserDateChanged.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucUserDateChanged.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucUserDateChanged.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucUserDateChanged.ClientID %>');
		}
	}

	//表示と非表示のテキスト ボックス
	$(document).ready(function ()
	{
		SetDisplayControl($("#<%=ddlUserMemoFlg.ClientID %>"), $("#<%=tbUserMemo.ClientID %>"));
		$("#<%=ddlUserMemoFlg.ClientID %>").change(function () { SetDisplayControl($("#<%=ddlUserMemoFlg.ClientID %>"), $("#<%=tbUserMemo.ClientID %>")); });

		SetDisplayControl($("#<%=ddlUserExtendFlg.ClientID %>"), $("#<%=tbUserExtendText.ClientID %>"));
		$("#<%=ddlUserExtendFlg.ClientID %>").change(function () { SetDisplayControl($("#<%=ddlUserExtendFlg.ClientID %>"), $("#<%=tbUserExtendText.ClientID %>")); });
		SetDisplayControl($("#<%=ddlUserExtendFlg.ClientID %>"), $("#<%=ddlUserExtendText.ClientID %>"));
		$("#<%=ddlUserExtendFlg.ClientID %>").change(function () { SetDisplayControl($("#<%=ddlUserExtendFlg.ClientID %>"), $("#<%=ddlUserExtendText.ClientID %>")); });
	});

	// Form Reset
	function Reset()
	{
		document.<%= this.Form.ClientID %>.reset();
		SetDisplayControl($("#<%=ddlUserMemoFlg.ClientID %>"), $("#<%=tbUserMemo.ClientID %>"));
		SetDisplayControl($("#<%=ddlUserExtendFlg.ClientID %>"), $("#<%=tbUserExtendText.ClientID %>"));
		SetDisplayControl($("#<%=ddlUserExtendFlg.ClientID %>"), $("#<%=ddlUserExtendText.ClientID %>"));

		document.getElementById('<%= ucUserDateChanged.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucUserDateChanged.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucUserDateChanged.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucUserDateChanged.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucUserDateChanged.ClientID %>');

		document.getElementById('<%= ucUserDateCreated.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucUserDateCreated.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucUserDateCreated.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucUserDateCreated.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucUserDateCreated.ClientID %>');
	}

	// Enable/Disable control
	function SetDisplayControl($ddlEle, $tbEle)
	{
		if ($ddlEle.val() != 1)
		{
			$tbEle.val("");
			$tbEle.attr("disabled", "disabled");
		}
		else
		{
			$tbEle.removeAttr("disabled");
		}
	}
//-->
</script>
<%--// 検索時の非表示--%>
<script type="text/javascript" src="<%= ResolveUrl("~/Js/hide-show_search_field.js") %>"></script>

</asp:Content>
