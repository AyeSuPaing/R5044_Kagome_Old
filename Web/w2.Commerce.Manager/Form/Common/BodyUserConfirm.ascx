<%--
=========================================================================================================
  Module      : ユーザー情報出力コントローラ(BodyUserConfirm.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.Domain.FixedPurchase.Helper" %>
<%@ Import Namespace="w2.Domain.Order" %>
<%@ Import Namespace="w2.Domain.UserIntegration" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BodyUserConfirm.ascx.cs" Inherits="Form_Common_BodyUserConfirm" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.App.Common.Global" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Import Namespace="Common" %>
<script type="text/javascript">
<!--
	// 選択された注文情報を設定
	function set_reorder_data(order_id)
	{
		if (window.opener != null)
		{
			window.opener.action_set_reorder_data(order_id);
			window.close();
		}
	}

	// 再注文へ進む確認ボックス表示
	function confirm_reorder()
	{
		if (confirm('再注文のため、注文情報登録画面へ遷移致します。\r\nよろしいでしょうか。') == true) {
			return true;
		} else {
			return false;
		}
	}
	//-->
</script>

<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
	<tr id="trMessages" runat="server" visible="false">
		<td align="center" class="detail_title_bg" colspan="2">メッセージ</td>
	</tr>
	<tr id="trUserIntegrationFlg" runat="server" visible="false">
		<td align="left" class="detail_item_bg" colspan="2">
			<asp:Label ID="lbUserIntegrationFlg" runat="server" ForeColor="Red" />
		</td>
	</tr>
	<tr id="trUserID" runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_ID) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			ユーザーID
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_ID) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_USER_ID) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal id="lUserId" runat="server"></asp:Literal>
			<% if (Constants.CS_OPTION_ENABLED
				&& (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_DETAIL)) { %>
					<a href="#" onclick="javascript:open_window('<%= WebSanitizer.HtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Cs, CreateLinkUserSearchCS(Request[Constants.REQUEST_KEY_USER_ID]))) %>','usersearch','width=1200,height=740,top=5,left=600,status=NO,scrollbars=yes');">
					CS顧客検索</a>
			<% } %>
		</td>
	</tr>
	<%--▽ モール連携オプションが有効の場合 ▽--%>
	<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
	<tr>
		<td align="left" class="detail_title_bg" width="30%">
			サイト
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MALL_ID) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_MALL_ID) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal id="lSiteName" runat="server"></asp:Literal></td>
	</tr>
	<% } %>
	<%--△ モール連携オプションが有効の場合 △--%>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			顧客区分
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal id="lUserKbn" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_NAME1) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.name.name@@") %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_NAME1) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_NAME1) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal id="lUserName" runat="server"></asp:Literal></td>
	</tr>
	<% if (this.IsShippingCountryAvailableJp) { %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_NAME_KANA1) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.name_kana.name@@") %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_NAME_KANA1) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_NAME_KANA1) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserNameKana" runat="server"></asp:Literal></td>
	</tr>
	<% } %>
	<%--▽ 商品レビュー機能が有効の場合 ▽--%>
	<% if (Constants.PRODUCTREVIEW_ENABLED) { %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_NICK_NAME) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.nickname.name@@") %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_NICK_NAME) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_NICK_NAME) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserNickName" runat="server"></asp:Literal></td>
	</tr>
	<% } %>
	<%--△ 商品レビュー機能が有効の場合 △--%>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_SEX) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.sex.name@@") %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_SEX) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_SEX) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserSex" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BIRTH) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.birth.name@@") %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BIRTH) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_BIRTH) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserBirth" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.mail_addr.name@@") %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserMailAddr" runat="server"></asp:Literal>
			<asp:Literal ID="lUserMailAddrErrorPoint" runat="server" visible="false">(エラーポイント：</asp:Literal>
			<asp:Button  ID="btnUserMailAddrErrorPoint" runat="server" Text=" エラーポイントクリア " 
				onclick="btnUserMailAddrErrorPoint_Click" visible="false"/>
			<asp:HiddenField ID="hUserMailAddrErrorPoint" runat="server"/>
		</td>
	</tr>
	<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR2) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.mail_addr2.name@@") %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR2) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR2) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
		<asp:Literal ID="lUserMailAddr2" runat="server"></asp:Literal>
		<asp:Literal ID="lUserMailAddr2ErrorPoint" runat="server" visible="false">(エラーポイント：</asp:Literal>
		<asp:Button  ID="btnUserMailAddr2ErrorPoint" runat="server" Text=" エラーポイントクリア " 
			onclick="btnUserMailAddr2ErrorPoint_Click" visible="false"/>
		<asp:HiddenField ID="hUserMailAddr2ErrorPoint" runat="server"/>
		</td>
	</tr>
	<% } %>
	<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.country.name@@", this.UserAddrCountryIsoCode) %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserAddrCountryName" runat="server"></asp:Literal></td>
	</tr>
	<% } %>
	<% if (this.IsUserAddrJp) { %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ZIP1) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.zip.name@@") %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ZIP1) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ZIP1) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserZip" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR1) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.addr1.name@@") %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR1) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR1) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserAddr1" runat="server"></asp:Literal></td>
	</tr>
	<% } %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR2) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.addr2.name@@", this.UserAddrCountryIsoCode) %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR2) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR2) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserAddr2" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR3) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%:ReplaceTag("@@User.addr3.name@@", this.UserAddrCountryIsoCode) %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR3) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR3) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserAddr3" runat="server"></asp:Literal></td>
	</tr>
	<tr <%= (Constants.DISPLAY_ADDR4_ENABLED || (this.IsUserAddrJp == false)) ? "" : "style=\"display:none;\"" %> visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR4) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.addr4.name@@", this.UserAddrCountryIsoCode) %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR4) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR4) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserAddr4" runat="server"></asp:Literal></td>
	</tr>
	<% if (this.IsUserAddrJp == false) { %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR5) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.addr5.name@@", this.UserAddrCountryIsoCode) %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADDR5) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADDR5) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserAddr5" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ZIP) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.zip.name@@", this.UserAddrCountryIsoCode) %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ZIP) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ZIP) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserZipGlobal" runat="server"></asp:Literal></td>
	</tr>
	<% } %>
	<tr runat="server" visible='<%# (Constants.DISPLAY_CORPORATION_ENABLED && GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_NAME)) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.company_name.name@@")%>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_NAME) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_NAME) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserCompanyName" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# (Constants.DISPLAY_CORPORATION_ENABLED && GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_POST_NAME)) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.company_post_name.name@@")%>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_POST_NAME) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_COMPANY_POST_NAME) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserCompanyPostName" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_TEL1_1) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.tel1.name@@", this.UserAddrCountryIsoCode) %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_TEL1_1) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_TEL1_1) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserTel1" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_TEL2_1) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			<%: ReplaceTag("@@User.tel2.name@@", this.UserAddrCountryIsoCode) %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_TEL2_1) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_TEL2_1) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserTel2" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			お知らせメールの配信希望
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserMailFlg" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			かんたん会員フラグ
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserEasyRegisterFlg" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LOGIN_ID) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			ログインID
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LOGIN_ID) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_LOGIN_ID) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserLoginId" runat="server"></asp:Literal></td>
	</tr>
	<tr style="display:<%= WebSanitizer.HtmlEncode(this.HavePasswordDisplayPower ? "" : "none") %>" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_PASSWORD) %>'>
		<td align="left" class="detail_title_bg" width="30%">パスワード
		<% if (this.ShouldDisplayRemarksToPassword) { %>
			<br />※□は半角スペースです
		<% } %>
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_PASSWORD) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_PASSWORD) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserPassword" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_REMOTE_ADDR) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			リモートIPアドレス
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_REMOTE_ADDR) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_REMOTE_ADDR) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lRemoteAddr" runat="server"></asp:Literal></td>
	</tr>
	<%-- アフィリエイトOPが有効の場合のみ表示する --%>
	<% if (Constants.W2MP_AFFILIATE_OPTION_ENABLED) { %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADVCODE_FIRST) %>'>
		<td align="left" class="detail_title_bg" width="30%">広告コード
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ADVCODE_FIRST) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ADVCODE_FIRST) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserAdvCode" runat="server"></asp:Literal></td>
	</tr>
	<% } %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			ユーザー特記欄
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserMemo" runat="server"></asp:Literal></td>
	</tr>
	<%--▽ 会員ランクOPが有効の場合 ▽--%>
	<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MEMBER_RANK_ID) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			会員ランク
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_MEMBER_RANK_ID) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_MEMBER_RANK_ID) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lMemberRank" runat="server"></asp:Literal></td>
	</tr>
	<%} %>
	<%--△ 会員ランクOPが有効の場合 △--%>
	<%--▽ 会員ランクOPかつ定期OPが有効の場合 ▽--%>
	<% if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
	<tr>
		<td align="left" class="detail_title_bg" width="30%">定期会員/非定期会員</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lFixedPurchaseMember" runat="server"></asp:Literal></td>
	</tr>
	<%} %>
	<%--△ 会員ランクOPかつ定期OPが有効の場合 △--%>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			ユーザー管理レベル
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserManagementLevel" runat="server"></asp:Literal></td>
	</tr>
	<% if (Constants.GLOBAL_OPTION_ENABLE){ %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			アクセス国ISOコード
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lAccessCountryIsoCode" runat="server"></asp:Literal></td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			表示言語ロケールID
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lDispLanguageLocaleId" runat="server"></asp:Literal>
			<asp:Literal ID="lDispLanguageCode" runat="server"></asp:Literal>
		</td>
	</tr>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			表示通貨ロケールID
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lDispCurrencyLocaleId" runat="server"></asp:Literal>
			<asp:Literal ID="lDispCurrencyCode" runat="server"></asp:Literal>
		</td>
	</tr>
	<% }%>
	<% if (Constants.W2MP_POINT_OPTION_ENABLED){ %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			最終誕生日ポイント付与年
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lLastBirthdayPointAddYear" runat="server"></asp:Literal></td>
	</tr>
	<% } %>
	<% if (Constants.W2MP_COUPON_OPTION_ENABLED){ %>
	<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR) %>'>
		<td align="left" class="detail_title_bg" width="30%">
			最終誕生日クーポン付与年
			<span visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR) %>" runat="server">
				<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR) %>
			</span>
		</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal ID="lLastBirthdayCouponPublishYear" runat="server"></asp:Literal></td>
	</tr>
	<% } %>
	<%-- ユーザー拡張項目の表示 --%>
	<asp:Repeater id="rUserExtendList" Runat="server">
	<ItemTemplate>
	<tr>
		<td align="left" class="detail_title_bg" width="30%"><%# WebSanitizer.HtmlEncode(((KeyValuePair<string,string>)Container.DataItem).Key) %></td>
		<td align="left" class="detail_item_bg">
			<%# WebSanitizer.HtmlEncode(((KeyValuePair<string, string>)Container.DataItem).Value) %></td>
	</tr>
	</ItemTemplate>
	</asp:Repeater>
	<tbody id="tbdyUserUpdateInfo" runat="server">
		<tr>
			<td align="left" class="detail_title_bg" width="30%">リアルタイム累計購入回数（注文基準）</td>
			<td align="left" class="detail_item_bg">
				<asp:Literal ID="lUserOrderCountOrderRealtime" runat="server"></asp:Literal>回</td>
		</tr>
		<tr>
			<td align="left" class="detail_title_bg" width="30%">最終ログイン日時</td>
			<td align="left" class="detail_item_bg">
			<asp:Literal ID="lUserDateLastLoggedin" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td align="left" class="detail_title_bg" width="30%">作成日</td>
			<td align="left" class="detail_item_bg">
				<asp:Literal ID="lUserDateCreated" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td align="left" class="detail_title_bg" width="30%">更新日</td>
			<td align="left" class="detail_item_bg">
				<asp:Literal ID="lUserDateChanged" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td align="left" class="detail_title_bg" width="30%">最終更新者</td>
			<td align="left" class="detail_item_bg">
				<asp:Literal ID="lUserLastChanged" runat="server"></asp:Literal></td>
		</tr>
	</tbody>
</table>
<%--GMO--%>
<%if ((Constants.PAYMENT_GMO_POST_ENABLED) && (this.IsBusinessOwner)) { %>
	<div id="dGmoPost" runat="server">
	<br />
		<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
			<tr class="list_title_bg">
				<td align="center" colspan="4">GMO枠保証審査情報</td>
			</tr>
			<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME1) %>'>
				<td align="left" class="detail_title_bg" width="30%">
					<%: ReplaceTag("@@User.OwnerName1.name@@") %>
					<span id="Span1" visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME1) %>" runat="server">
						<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME1) %>
					</span>
				</td>
				<td align="left" class="detail_item_bg">
					<asp:Literal id="lpresidentNameFamily" runat="server"></asp:Literal></td>
			</tr>
			<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME2) %>'>
				<td align="left" class="detail_title_bg" width="30%">
					<%: ReplaceTag("@@User.OwnerName2.name@@") %>
					<span id="Span2" visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME2) %>" runat="server">
						<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME2) %>
					</span>
				</td>
				<td align="left" class="detail_item_bg">
					<asp:Literal id="lpresidentName" runat="server"></asp:Literal></td>
			</tr>
			<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1) %>'>
				<td align="left" class="detail_title_bg" width="30%">
					<%: ReplaceTag("@@User.OwnerNameKana1.name@@") %>
					<span id="Span3" visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1) %>" runat="server">
						<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1) %>
					</span>
				</td>
				<td align="left" class="detail_item_bg">
					<asp:Literal id="lpresidentNameFamilyKana" runat="server"></asp:Literal></td>
			</tr>
			<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2) %>'>
				<td align="left" class="detail_title_bg" width="30%">
					<%: ReplaceTag("@@User.OwnerNameKana2.name@@") %>
					<span id="Span4" visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2) %>" runat="server">
						<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2) %>
					</span>
				</td>
				<td align="left" class="detail_item_bg">
					<asp:Literal id="lpresidentNameKana" runat="server"></asp:Literal></td>
			</tr>
			<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_BIRTH) %>'>
				<td align="left" class="detail_title_bg" width="30%">
					<%: ReplaceTag("@@User.OwnerBirth.name@@") %>
					<span id="Span6" visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_BIRTH) %>" runat="server">
						<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_BIRTH) %>
					</span>
				</td>
				<td align="left" class="detail_item_bg">
					<asp:Literal id="lOwnerBirth" runat="server"></asp:Literal></td>
			</tr>

			<tr runat="server" visible='<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET) %>'>
				<td align="left" class="detail_title_bg" width="30%">
					<%: ReplaceTag("@@User.RequestBudget.name@@") %>
					<span id="Span5" visible="<%# GetDefaultSettingDisplayField(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET) %>" runat="server">
						<br /><%#: GetDefaultSettingCommentForDisplay(Constants.TABLE_USER, Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET) %>
					</span>
				</td>
				<td align="left" class="detail_item_bg">
					<asp:Literal id="lreqUpperLimit" runat="server"></asp:Literal></td>
			</tr>
		</table>
	</div>
<%} %>
<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_DETAIL){ %>
<br />
<% if (Constants.USER_ATTRIBUTE_OPTION_ENABLE){ %>
<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
	<tr class="list_title_bg">
		<td align="center" colspan="4">
			ユーザー属性<br />
			（集計日：<asp:Literal id="lAttributeCalculateDate" runat="server"></asp:Literal>）
		</td>
	</tr>
	<tbody id="tbdyUserAttribute" runat="server">
	<tr>
		<td align="left" class="detail_title_bg" width="30%">最終購入日</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal id="lLastOrderDate" runat="server"></asp:Literal>
		</td>
		<td align="left" class="detail_title_bg" width="30%">離脱期間<br/>
			（最終購入から経った期間）
		</td>
		<td align="left" class="detail_item_bg" width="20%" style="text-align:right;">
			<asp:Literal id="lAwayDays" runat="server"></asp:Literal> 日
		</td>
	</tr>
	<tr>
		<td align="left" class="detail_title_bg" width="30%">２回目購入日</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal id="lSecondOrderDate" runat="server"></asp:Literal>
		</td>
		<td align="left" class="detail_title_bg" width="30%" rowspan="2">在籍期間<br/>
			（初回購入から最終購入までの期間）
		</td>
		<td align="left" class="detail_item_bg" width="20%" style="text-align:right;" rowspan="2">
			<asp:Literal id="lEnrollmentDays" runat="server"></asp:Literal> 日
		</td>
	</tr>
	<tr>
		<td align="left" class="detail_title_bg" width="30%">初回購入日</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal id="lFirstOrderDate" runat="server"></asp:Literal>
		</td>
	</tr>
	<tr>
		<td align="left" class="detail_title_bg" width="30%">累計購入金額（注文基準・全体）</td>
		<td align="left" class="detail_item_bg" style="text-align:right;">
			<asp:Literal id="lOrderAmountOrderAll" runat="server"></asp:Literal>
		</td>
		<td align="left" class="detail_title_bg" width="30%">累計購入金額（出荷基準・全体）</td>
		<td align="left" class="detail_item_bg" style="text-align:right;">
			<asp:Literal id="lOrderAmountShipAll" runat="server"></asp:Literal>
		</td>
	</tr>
	<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
	<tr>
		<td align="left" class="detail_title_bg" width="30%">累計購入金額（注文基準・定期のみ）</td>
		<td align="left" class="detail_item_bg" style="text-align:right;">
			<asp:Literal id="lOrderAmountOrderFp" runat="server"></asp:Literal>
		</td>
		<td align="left" class="detail_title_bg" width="30%">累計購入金額（出荷基準・定期のみ）</td>
		<td align="left" class="detail_item_bg" style="text-align:right;">
			<asp:Literal id="lOrderAmountShipFp" runat="server"></asp:Literal>
		</td>
	</tr>
	<%} %>
	<tr>
		<td align="left" class="detail_title_bg" width="30%">累計購入回数（注文基準・全体）</td>
		<td align="left" class="detail_item_bg" style="text-align:right;">
			<asp:Literal id="lOrderCountOrderAll" runat="server"></asp:Literal> 回
		</td>
		<td align="left" class="detail_title_bg" width="30%">累計購入回数（出荷基準・全体）</td>
		<td align="left" class="detail_item_bg" style="text-align:right;">
			<asp:Literal id="lOrderCountShipAll" runat="server"></asp:Literal> 回
		</td>
	</tr>
	<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
	<tr>
		<td align="left" class="detail_title_bg" width="30%">累計購入回数（注文基準・定期のみ）</td>
		<td align="left" class="detail_item_bg" style="text-align:right;">
			<asp:Literal id="lOrderCountOrderFp" runat="server"></asp:Literal> 回
		</td>
		<td align="left" class="detail_title_bg" width="30%">累計購入回数（出荷基準・定期のみ）</td>
		<td align="left" class="detail_item_bg" style="text-align:right;">
			<asp:Literal id="lOrderCountShipFp" runat="server"></asp:Literal> 回
		</td>
	</tr>
	<%} %>
	<%if (Constants.CPM_OPTION_ENABLED) {%>
	<tr>
		<td align="left" class="detail_title_bg" width="30%">CPMクラスタ</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal id="lCpmCluster" runat="server"></asp:Literal><br/>
			<asp:Literal id="lCpmClusterBefore" runat="server"></asp:Literal>
		</td>
		<td align="left" class="detail_title_bg" width="30%">CPMクラスタ変更日</td>
		<td align="left" class="detail_item_bg">
			<asp:Literal id="lCpmClusterChangedDate" runat="server"></asp:Literal><br/>
		</td>
	</tr>
	<%} %>
	<tr>
		<td align="left" class="detail_item_bg" colspan="4">
			備考：<br />
			・データは集計日時点のものを表示しています。<br />
			・キャンセル注文は一律除外して算出しています。<br />
			・各購入日・在籍期間・離脱期間は、返品・交換注文を除外して算出しています。<br />
			・各購入金額は、返品交換分の調整金額も合算して算出しています。<br />
			<%if (Constants.CPM_OPTION_ENABLED) {%>
			・CPMクラスタとは、顧客ポートフォリオ・マネジメント理論によって導き出されたユーザーの区分のことです。<br />
			<%} %>
		</td>
	</tr>
	</tbody>
</table>
<% } %>

<div>
	<br />
	<table class="list_table detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr class="list_title_bg">
			<td align="center" colspan="3">アドレス帳一覧 &nbsp;
			<div ID="btnAddShipping" runat="server" >
				<input id="btnAddUserShipping"
					onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(CreateShippingInsertUrl()) %>','shipping_popup','width=850,height=800,top=120,left=320,status=NO,scrollbars=yes');"
					value="  アドレス帳の追加  " type="button" visible="true" 
					/>
			</div>
			</td>
		</tr>
		<tr class="list_title_bg">
			<td align="center">配送先名</td>
			<td align="center" colspan="2">お届け先</td>
		</tr>
		<tr id="trShippingListMessage" runat="server" class="list_alert">
			<td align="center" colspan="3" id="tdShippingListMessageError" runat="server"></td>
		</tr>
		<asp:Repeater ID="rUserShippingList" runat="server" ItemType="w2.Domain.UserShipping.UserShippingModel" OnItemCommand="rUserShippingList_ItemCommand">
			<ItemTemplate>
				<tr class="list_item_bg1">
					<td class="shippingName">
						<%#: Item.Name %>
					</td>
					<td class="shippingAddr">
						<%# IsCountryJp(Item.ShippingCountryIsoCode) ? WebSanitizer.HtmlEncode("〒" + Item.ShippingZip) + "<br />" : "" %>
						<%#: Item.ShippingAddr1 %>&nbsp;
						<%#: Item.ShippingAddr2 %>&nbsp;
						<%#: Item.ShippingAddr3 %>&nbsp;
						<%#: Item.ShippingAddr4 %>&nbsp;
						<%#: Item.ShippingAddr5 %>&nbsp;
						<%#: (IsCountryJp(Item.ShippingCountryIsoCode) == false) ? Item.ShippingZip : "" %>
						<%#: Item.ShippingCountryName %><br />
						<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
							<%#: Item.ShippingCompanyName %><br />
							<%#: Item.ShippingCompanyPostName %><br />
						<% } %>
						<%#: Item.ShippingName %>&nbsp;様
						<%#: IsCountryJp(Item.ShippingCountryIsoCode) ? "（" + Item.ShippingNameKana + "さま）" : "" %>
					</td>
					<td align="right" width="135" style='display:<%# (Request[Constants.REQUEST_KEY_WINDOW_KBN] != Constants.KBN_WINDOW_POPUP)?"":"none" %>'>
						<div runat="server" visible="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>">
							<input onclick="javascript:open_window('<%# CreateShippingDetailUrl(Item.UserId, Item.ShippingNo) %>','shipping_popup','width=850,height=780,top=120,left=320,status=NO,scrollbars=yes');" value="  編集する  " type="button"/>
						</div>
						<asp:Button ID="btnDelete" runat="server" Text="  削除する  " OnClientClick="return confirm('削除しますか？');" CommandName="Delete" CommandArgument="<%# Item.ShippingNo %>" />
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>
</div>
<%} %>
<span id="anchorUserInvoice"></span>
<div runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo() %>">
	<br />
	<table class="list_table detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr class="list_title_bg">
			<td align="center" colspan="4">電子発票管理一覧 &nbsp;
			<% if (this.IsPopUp == false){ %>
			<div ID="btnAddUserInvoice" runat="server" >
				<input id="btnAddTaiWanUserInvoice"
					onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(CreateUserInvoiceInsertUrl()) %>	','shipping_popup','width=850,height=800,top=120,left=320,status=NO,scrollbars=yes');"
					value="  電子発票管理追加  " type="button" visible="true" 
					/>
			</div>
			<%} %>
			</td>
		</tr>
		<tr class="list_title_bg">
			<td align="center" style="width:230px">電子発票情報名</td>
			<td align="center" style="width:230px">電子発票種別</td>
			<td align="center" colspan ="2">コード</td>
		</tr>
		<tr id="trUserInvoiceListMessage" runat="server" visible ="false" class="list_alert">
			<td align="center" colspan="4" id="tdUserInvoiceListMessageError" runat="server"></td>
		</tr>
		<asp:Repeater ID="rUserInvoiceList" runat="server" ItemType="w2.Domain.TwUserInvoice.TwUserInvoiceModel" OnItemCommand="rUserInvoiceList_ItemCommand">
		<ItemTemplate>
		<tr class="list_item_bg1">
		<td class="twInvoiceName">
			<%#:Item.TwInvoiceName %>
		</td>
		<td class="twUniformInvoice">
			<%#: ValueText.GetValueText(
				Constants.TABLE_TWUSERINVOICE,
				Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE,
				Item.TwUniformInvoice) %>
		</td>
		<td class="twCarryType">
			<%# GetDisplayCode(Item) %>
		</td>
			<td align="right" width="135" style='display:<%# (Request[Constants.REQUEST_KEY_WINDOW_KBN] != Constants.KBN_WINDOW_POPUP)?"":"none" %>'>
				<input onclick="javascript:open_window('<%# CreateUserInvoiceDetailUrl(Item.UserId, Item.TwInvoiceNo) %>	','shipping_popup','width=850,height=780,top=120,left=320,status=NO,scrollbars=yes');" value="  編集する  " type="button"/>
				<asp:Button ID="btnDelete" runat="server" Text="  削除する  " OnClientClick="return confirm('削除しますか？');" CommandName="Delete" CommandArgument="<%# Item.TwInvoiceNo %>" />
			</td>
		</tr>
		</ItemTemplate>
		</asp:Repeater>
	</table>
</div>

<% if (w2.Domain.User.UserService.IsUser(this.UserKbn) == true && Constants.MAX_NUM_REGIST_CREDITCARD > 0 && Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_DETAIL && OrderCommon.CreditCardRegistable){ %>
<div>
	<br />
	<a name="for_disp_creditcard"></a>
	<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr class="list_title_bg" align="center" id="trInsertCreditCard" runat="server" >
			<td colspan="3">登録クレジットカード一覧 &nbsp;
				<div id="divUserCreditCardRegisterable" Visible="False" runat="server">
				<%if (this.CanCreditCardInput || this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus) {%>
					<% if (SessionManager.UsePaymentTabletZeus) btnInsertCreditCard.OnClientClick = null; %>
					<asp:Button ID="btnInsertCreditCard" runat="server" Text="  クレジットカードの登録  " UseSubmitBehavior="False" OnClientClick="openPopupInsertCreditCard();return false;" OnClick="btnInsertCreditCard_Click" />
				<% } else {%>
					<br/><span style="font-weight: normal">（クレジットカードの登録は決済用タブレットにて行ってください）</span>
				<% } %>
				</div>
			</td>
		</tr>
		<tr class="list_title_bg">
			<td align="center" width="34%">クレジットカード登録名</td>
			<td align="center">登録カード詳細</td>
			<td align="center" width="6%">&nbsp;&nbsp;</td>
		</tr>
		<asp:Repeater ID="rUserCreditCardList" ItemType="w2.App.Common.Order.UserCreditCard" runat="server" OnItemCommand="rUserCreditCardList_ItemCommand">
			<ItemTemplate>
				<tr class="list_item_bg1">
					<td align="left"><div style="padding-left:7px;"><%#: Item.CardDispName %></div></td>
					<td>
						<div Visible="<%# Item.IsRegisterdStatusNormal %>" runat="server">
							<%# UserCreditCardHelper.CreateCreditCardInfoHtml(Item, false) %>
						</div>
						<div Visible="<%# Item.IsRegisterdStatusUnregisterd %>" runat="server">
							<%if (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus) {%>
							<span style="color: red">
							クレジットカード登録は保留状態です。<br/>
							下記の手順で与信を行ってください。<br/>
							</span>
							<br/>
							<%if (OrderCommon.IsPaymentCardTypeGmo) {%>
							①決済用タブレット「会員・カード登録」を開き、<br/>
							クレジットカード番号と下記IDにてカードの登録を行ってください。<br />
							<br />
							<span style="font-size: large;line-height: 18px">
							GMO会員ID：<%#: string.Join("-", StringUtility.SplitByLength(Item.CooperationId, 4))  %><br />
							</span>
							<br/>
							※GMO会員IDはハイフンなしで入力してください。<br/>
							<%} %>
							<%if (OrderCommon.IsPaymentCardTypeYamatoKwc) {%>
							①決済用タブレットにて下記情報とともに1円与信を実行してください。<br/>
							<span style="font-size: large; line-height: 18px;text-align:center;">
							　受付番号　　：<%#: string.Join("-", StringUtility.SplitByLength(DateTime.Now.ToString("yyMMddHHmmss"), 4)) %><br/>
							　ヤマト会員ID：<%#: string.Join("-", StringUtility.SplitByLength(Item.CooperationId, 4)) %><br/>
							　認証キー　　：<%#: Item.CooperationId2 %><br/>
							　金額　　　　：1 円<br/>
							</span>
							<br/>
							　※受付番号・ヤマト会員IDはハイフンなしで入力してください。<br/>
							　※１円与信はヤマト会員登録のために利用します。<br/>
							　　ヤマト会員登録後、この1円与信は取り消しされます。<br/>
							<%} %>
							<%if (OrderCommon.IsPaymentCardTypeEScott) {%>
							①決済用タブレットにて、クレジットカード情報と下記情報で会員登録を実行してください。<br/>
							<br />
							<span style="font-size: large; line-height: 18px;text-align:center;">
							　会員ID　　　　：<%#: string.Join("-", StringUtility.SplitByLength(Item.CooperationId, 4)) %><br/>
							</span>
							<br/>
							※会員IDはハイフンなしで入力してください。
							<%} %>
							<br/>
							②登録が完了したらリロードを行い、正常に登録されていることを確認してください。<br/>
							<br/>
							<asp:Button id="btnReloadForRegisterdCreditCard" Text="    リロード    " OnClick="btnReloadForRegisterdCreditCard_Click" runat="server"/>
							<%} %>	
						</div>
					</td>
					<td align="center">
						<div style="padding:7px;">
							<input onclick="javascript:open_window('<%# CreateCreditCardEdit(Item.UserId, Item.BranchNo) %>	','shipping_popup','width=850,height=780,top=120,left=320,status=NO,scrollbars=yes');" value="  編集する  " type="button" />
							<asp:Button ID="lbDelete" runat="server" CommandName="Delete" CommandArgument='<%#: Item.BranchNo %>' OnClientClick="return confirm('削除しますか？');" Text="  削除する  "></asp:Button>
						</div>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr id="trCreditCardErrorMessage" runat="server" visible="false" class="list_item_bg1">
			<td colspan="3" align="center"><%=WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERCREDITCARD_NO_CARD)%></td>
		</tr>
	</table>
</div>
<% } %>
<%--▽ ポイントOPが有効の場合 ▽--%>
<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsDispPointInfo) { %>
<div id="Div1" runat="server">
	<br />
	<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr class="list_title_bg">
		<td align="center" colspan="4">このユーザーのポイント情報</td>
		</tr>
		<tr>
			<td class="detail_title_bg" align="left" width="30%">利用可能ポイント</td>
			<td class="detail_item_bg" align="left">
			<% if ((this.UserPoint != null) && this.UserPoint.ExistsUserPointRecord && MenuUtility.HasAuthorityMp(this.LoginShopOperator, Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_USERPOINTHISTORY_LIST)) { %>
				<div>
					<a href="#" onclick="javascript:open_window('<%= WebSanitizer.HtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Mp, CreateUserPointHistoryUrl(Request[Constants.REQUEST_KEY_USER_ID]))) %>','userpointhistory','width=1250,height=600,top=110,left=380,status=NO,scrollbars=yes');">
					<%= WebSanitizer.HtmlEncode(GetNumeric(this.UserPointUsable)) %>pt（仮ポイント<%= WebSanitizer.HtmlEncode(GetNumeric(this.UserPointTemp)) %>pt）</a>
				</div>
			 <% } else { %>
				<div>
					 <%= WebSanitizer.HtmlEncode(GetNumeric(this.UserPointUsable)) %>pt（仮ポイント<%= WebSanitizer.HtmlEncode(GetNumeric(this.UserPointTemp)) %>pt）
				</div>
			 <% } %>
			</td>
			<td class="detail_title_bg" align="left">有効期限</td>
			<td class="detail_item_bg" align="left" width="20%">
				<% if (this.UserPointExpiry.HasValue) { %>
				<%: DateTimeUtility.ToStringForManager(this.UserPointExpiry.Value, DateTimeUtility.FormatType.ShortDate2Letter) %> 
				<% } %>
			</td>
		</tr>
	</table>
</div>
<% } %>
<%--△ ポイントOPが有効の場合 △--%>
<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
<div id="dvFixedPurchaseHistory" runat="server">
	<p id="anchorFixedPurchaseHistory" align="right" style="margin-bottom:5px;"><a href="#top">ページトップ</a></p>
	<br />
	<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr class="list_title_bg">
		<td align="center" colspan="7">このユーザーの定期購入
			<asp:Label ID="lbFixedPurchaseHistoryCount" runat="server" Visible="false"/>
		</td>
		</tr>
		<tr class="list_title_bg">
			<td align="center" width="120" rowspan="2">定期購入ID</td>
			<td align="center" width="120" rowspan="2">定期購入区分</td>
			<td align="center" width="100" colspan="2">購入回数</td>
			<td align="center" width="100" rowspan="2">定期購入ステータス</td>
			<td align="center" width="100" rowspan="2">決済ステータス</td>
			<td align="center" width="60" rowspan="2">有効フラグ</td>
		</tr>
		<tr class="list_title_bg">
			<td align="center" width="50">注文基準</td>
			<td align="center" width="50">出荷基準</td>
		</tr>
		<asp:Repeater id="rFixedPurchaseList" Runat="server">
			<ItemTemplate>
				<tr class="list_item_bg1 fixedPurchaseHistory">
					<td align="center">
						<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM)) { %>
							<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(FixedPurchasePage.CreateFixedPurchaseDetailUrl(((UserFixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseId, true)) %>','ordercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: ((UserFixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseId %></a>
						<% } else { %>
							<%#: ((UserFixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseId %>
						<% } %>
					</td>
					<td align="center"><%# WebSanitizer.HtmlEncode(OrderCommon.CreateFixedPurchaseSettingMessage((UserFixedPurchaseListSearchResult)Container.DataItem))%></td>
					<td align="right"><%# WebSanitizer.HtmlEncode(((UserFixedPurchaseListSearchResult)Container.DataItem).OrderCount)%> 回</td>
					<td align="right"><%# WebSanitizer.HtmlEncode(((UserFixedPurchaseListSearchResult)Container.DataItem).ShippedCount)%> 回</td>
					<td align="center">
						<span class="<%# FixedPurchasePage.GetFixedPurchaseStatusCssClass(((UserFixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseStatus) %>">
							<%# WebSanitizer.HtmlEncode(((UserFixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseStatusText)%>
						</span></td>
					<td align="center">
						<span class="<%# FixedPurchasePage.GetPaymentStatusCssClass(((UserFixedPurchaseListSearchResult)Container.DataItem).PaymentStatus) %>">
							<%# WebSanitizer.HtmlEncode(((UserFixedPurchaseListSearchResult)Container.DataItem).PaymentStatusText)%>
						</span></td>
					<td align="center"><%# WebSanitizer.HtmlEncode(((UserFixedPurchaseListSearchResult)Container.DataItem).ValidFlgText)%></td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr id="trFixedPurchaseListError" class="list_alert" runat="server" Visible="false">
			<td id="tdFixedPurchaseListErrorMessage" colspan="7" runat="server">
			</td>
		</tr>
		<tr id="trFixedPurchaseMore" class="list_title_bg" runat="server">
			<td colspan="7">
				<a id="showFixedPurchase" href="#anchorFixedPurchaseHistory" onclick="ShowMore('fixedPurchaseHistory','<%=trFixedPurchaseMore.ClientID %>')" style="width:100%;display:inline-block;text-align:center;text-decoration:none">すべて見る</a>
			</td>
		</tr>
	</table>
</div>
<% } %>
<div id="dvOrderHistory" runat="server">
	<p id="anchorOrderHistory" align="right" style="margin-bottom:5px;"><a href="#top">ページトップ</a></p>
	<br />
	<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr class="list_title_bg">
		<td align="center" colspan="<%# CanUseReOrderFunction() ? 8 : 7 %>">
			このユーザーの過去の受注履歴
			<asp:Label ID="lbOrderHistoryCount" runat="server" Visible="false"/>
			<%if (Constants.ORDERREGIST_OPTION_ENABLED 
		 && this.Page.Master.AppRelativeVirtualPath.ToLower() == "~/Form/Common/DefaultPage.master".ToLower() 
		 && ManagerMenuCache.Instance.HasOperatorMenuAuthority(Constants.PATH_ROOT + Constants.MENU_PATH_LARGE_ORDERREGIST)) {%>
				<asp:Button ID="btnGoToOrderRegist" Text="  注文登録する  " runat="server" OnClick="btnGoToOrderRegist_Click"  visible="true" />
			<%} %>
		</td>
		</tr>
		<tr class="list_title_bg">
			<td align="center" width="150">注文ID</td>
			<td align="center" width="150">注文日時</td>
			<td align="center" width="108">合計金額</td>
			<td align="center" width="100">注文ステータス</td>
			<td align="center" width="100">入金ステータス</td>
			<td align="center" width="190">決済種別</td>
			<td align="center" width="30">メモ</td>
			<td align="center" width="30" style="<%# CanUseReOrderFunction() ? "" : "display:none" %>"></td>
		</tr>
		<asp:Repeater id="rOrderList" ItemType="w2.Domain.Order.OrderModel" Runat="server" OnItemCommand="rOrderList_ItemCommand">
			<ItemTemplate>
				<tr class="list_item_bg1 orderHistory" title="<%# this.GetHistoryOrderItemDetail(((OrderModel)Container.DataItem).Items) %>" style="text-align:left">
					<td align="center">
						<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM)) { %>
							<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateOrderDetailUrl(((OrderModel)Container.DataItem).OrderId)) %>','ordercontact','width=1100,height=800,top=110,left=380,status=NO,scrollbars=yes,resizable=YES');"><%#: ((OrderModel)Container.DataItem).OrderId %></a>
						<% } else { %>
							<%#: ((OrderModel)Container.DataItem).OrderId %>
						<% } %>
					</td>
					<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.OrderDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
					<td align="right"><%#: ((OrderModel)Container.DataItem).OrderPriceTotal.ToPriceString(true) %></td>
					<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, ((OrderModel)Container.DataItem).OrderStatus) %></td>
					<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, ((OrderModel)Container.DataItem).OrderPaymentStatus) %></td>
					<td align="center">
						<%#:
							StringUtility.ToEmpty(((OrderModel)Container.DataItem).PaymentName) + 
							((StringUtility.ToEmpty(((OrderModel)Container.DataItem).CardInstruments) != "") ? ("(" + StringUtility.ToEmpty(((OrderModel)Container.DataItem).CardInstruments) + ")") : "")
						%>
					</td>
					<td align="center"><%# (StringUtility.ToEmpty(((OrderModel)Container.DataItem).Memo).Length == 0) ? "" : "*" %></td>
					<td align="center" style="<%# CanUseReOrderFunction() ? "" : "display:none" %>">
						<asp:Button ID="btnReOrder" Text="  再注文  " runat="server" OnClientClick="return confirm_reorder()" CommandName="ReOrder" CommandArgument="<%# ((OrderModel)Container.DataItem).OrderId %>" Visible="<%#CanReOrder((OrderModel)Container.DataItem) && Request[Constants.REQUEST_KEY_WINDOW_KBN] != Constants.KBN_WINDOW_POPUP%>" />
						<input type="button" value="  再注文  " runat="server" onclick=<%# "javascript:set_reorder_data('" + ((OrderModel)Container.DataItem).OrderId +"')" %> Visible="<%#CanReOrder((OrderModel)Container.DataItem) && (this.IsPopUp)%>" />
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr id="trOrderListError" class="list_alert" runat="server" Visible="false">
			<td id="tdOrderListErrorMessage" colspan="<%# CanUseReOrderFunction() ? 8 : 7 %>" runat="server">
			</td>
		</tr>
		<tr id="trOrderMore" class="list_title_bg" runat="server">
			<td colspan="<%# CanUseReOrderFunction() ? 8 : 7 %>">
				<a id="showMoreOrder" href="#anchorOrderHistory" onclick="ShowMore('orderHistory','<%=trOrderMore.ClientID %>')" style="width:100%;display:inline-block;text-align:center;text-decoration:none">すべて見る</a>
			</td>
		</tr>
		<tr  style="<%# CanUseReOrderFunction() ? "" : "display:none" %>">
			<td align="left" class="detail_item_bg" style="padding: 3px 2px 3px 7px" colspan="<%# CanUseReOrderFunction() ? 8 : 7 %>">
				備考：<br />
				次のいずれかの注文に対しては再注文できません。<br />
				・セット商品を購入した注文<br />
				・返品／交換注文<br />
				・定期購入のみの商品を購入した注文
			</td>
		</tr>
	</table>
</div>
<%--▽ 会員ランクOPが有効の場合 ▽--%>
<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
<div id="dvUserMemberRankHistory" runat="server">
	<p id="anchorMemberRankHistory" align="right" style="margin-bottom:5px;"><a href="#top">ページトップ</a></p>
	<br />
	<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr class="list_title_bg">
		<td align="center" colspan="3">このユーザーの会員ランク更新履歴
			<asp:Label ID="lbMemberRankHistoryCount" runat="server" Visible="false"/>
		</td>
		</tr>
		<tr class="list_title_bg">
			<td align="center" width="34%">更新日時</td>
			<td align="center" width="33%">変更前ランク</td>
			<td align="center" width="33%">変更後ランク</td>
		</tr>
		<asp:Repeater id="rMemberRankList" Runat="server">
			<ItemTemplate>
				<tr class="list_item_bg1 memberRankHistory">
					<td align="center"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_USERMEMBERRANKHISTORY_DATE_CREATED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
					<td align="center"><%# WebSanitizer.HtmlEncode(Eval("before_rank_name"))%></td>
					<td align="center"><%# WebSanitizer.HtmlEncode(Eval("after_rank_name"))%></td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr id="trMemberRankListError" class="list_alert" runat="server" Visible="false">
			<td id="tdMemberRankListErrorMessage" colspan="3" runat="server">
			</td>
		</tr>
		<tr id="trMemberRankMore" class="list_title_bg" runat="server">
			<td colspan="3">
				<a id="showMoreMemberRank" href="#anchorMemberRankHistory" onclick="ShowMore('memberRankHistory','<%=trMemberRankMore.ClientID %>')" style="width:100%;display:inline-block;text-align:center;text-decoration:none">すべて見る</a>
			</td>
		</tr>
	</table>
</div>
<%} %>
<%--△ 会員ランクOPが有効の場合 △--%>
<%--▽ ユーザー統合OPが有効の場合 ▽--%>
<% if (Constants.USERINTEGRATION_OPTION_ENABLED) { %>
<div id="dvUserIntegration" runat="server" visible="false">
	<br />
	<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr class="list_title_bg">
		<td align="center" colspan="3">このユーザーの統合履歴一覧</td>
		</tr>
		<tr class="list_title_bg">
			<td align="center" width="34%">更新日時</td>
			<td align="center" width="33%">ユーザー統合No</td>
			<td align="center" width="33%">統合したユーザーID</td>
		</tr>
		<asp:Repeater id="rUserIntegrationList" Runat="server">
			<ItemTemplate>
				<tr class="list_item_bg1">
					<td align="center"><%#: DateTimeUtility.ToStringForManager(((UserIntegrationModel)Container.DataItem).DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
					<td align="center">
						<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_INTEGRATION_REGISTER)) { %>
							<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserIntegrationRegisterlUrl(((UserIntegrationModel)Container.DataItem).UserIntegrationNo.ToString())) %>','userintegration','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: ((UserIntegrationModel)Container.DataItem).UserIntegrationNo %></a>
						<% } else { %>
							<%#: ((UserIntegrationModel)Container.DataItem).UserIntegrationNo %>
						<% } %>
					</td>
					<td align="center" style="padding-left: 0px;">
						<asp:Repeater runat="server" DataSource="<%# ((UserIntegrationModel)Container.DataItem).Users.Where(u => u.IsOnRepresentativeFlg == false) %>">
							<ItemTemplate>
								<%# Container.ItemIndex != 0 ? "<br/>" : "" %>
								<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
									<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl(((UserIntegrationUserModel)Container.DataItem).UserId)) %>"><%# WebSanitizer.HtmlEncode(((UserIntegrationUserModel)Container.DataItem).UserId) %></a>
								<% } else { %>
									<%# WebSanitizer.HtmlEncode(((UserIntegrationUserModel)Container.DataItem).UserId) %>
								<% } %>
							</ItemTemplate>
						</asp:Repeater>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>
</div>
<%} %>
<%--△ ユーザー統合OPが有効の場合 △--%>
<%--▽ DM発送履歴OPが有効の場合 ▽--%>
<% if (Constants.DM_SHIPPING_HISTORY_OPTION_ENABLED) { %>
<p id="anchorDmShippingHistory" align="right" style="margin-bottom:5px;"><a href="#top">ページトップ</a></p>
<div id="dvUserDmShippingHistory" runat="server">
	<br />
	<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr class="list_title_bg">
			<td align="center" colspan="4">このユーザーのDM発送履歴
				(<asp:Label ID="lbDmShippingHistoryCount" runat="server" />件)
			</td>
		</tr>
		<tr class="list_title_bg">
			<td align="center" width="18%">DM発送日</td>
			<td align="center" width="18%">DMコード</td>
			<td align="center" width="46%">DM名</td>
			<td align="center" width="18%">有効期間</td>
		</tr>
		<asp:Repeater id="rDmShippingHistoryList" ItemType="w2.Domain.DmShippingHistory.DmShippingHistoryModel" Runat="server">
			<ItemTemplate>
				<tr class="list_item_bg1 dmShippingHistory">
					<td align="center"><%#: DateTimeUtility.ToStringFromRegion(Item.DmShippingDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
					<td align="center"><%#: Item.DmCode %></td>
					<td align="center"><%#: Item.DmName %></td>
					<td align="center"><%#: Item.ValidDate %></td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr id="trDmShippingHistoryListError" class="list_alert" runat="server" Visible="false">
			<td id="tdDmShippingHistoryListErrorMessage" colspan="4" runat="server">
			</td>
		</tr>
		<tr id="trDmShippingHistoryMore" class="list_title_bg" runat="server">
			<td colspan="4">
				<a id="showDmShippingHistory" href="#anchorDmShippingHistory" onclick="ShowMore('dmShippingHistory','<%= trDmShippingHistoryMore.ClientID %>')" style="width:100%;display:inline-block;text-align:center;text-decoration:none">すべて見る</a>
			</td>
		</tr>
	</table>
</div>
<%} %>
<%--△ DM発送履歴OPが有効の場合 △--%>
<%--▽ 会員退会ボタン(退会している場合は非表示) ▽--%>
<div id="dvUserWithdrawal" Visible="true" runat="server">
	<br />
	<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr class="list_title_bg">
		<td align="center" colspan="3">退会</td>
		</tr>
		<tr class="list_item_bg1">
		<td align="center" colspan="3">
			<% if (IsWithdrawalLimit(this.UserId)) { %>
			<div runat="server" Visible="True" style="color:#ff0000; margin-bottom:3px; font-size:15px; line-height: 20px" align="center">
				有効な定期台帳があります。<br />
				定期台帳を削除後、退会処理が行えます。
			</div>
			<% } else { %>
			<asp:Button ID="btnUserWithdrawal" Text="  会員退会  " runat="server" OnClientClick="return confirm('退会処理を行います。よろしいですか？');" OnCommand="btnUserWithdrawal_Click"/>
			<% } %>
			</td>
		</tr>
	</table>
</div>
<%--△ 会員退会ボタン(退会している場合は非表示) △--%>
<script type="text/javascript" language="javascript">
	function openPopupInsertCreditCard()
	{
		var windowStyle = "width=814,height=510,top=120,left=420,status=no,scrollbars=no";
		var url =
			"<%= new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CREDITCARD_INPUT)
					.AddParam(Constants.REQUEST_KEY_USER_ID, Request[Constants.FIELD_USER_USER_ID])
					.AddParam(Constants.REQUEST_KEY_ACTION_STATUS,Constants.ACTION_STATUS_INSERT).CreateUrl() %>";
		var newWindow = window.open(url, "OpenPopupCreditCardInput", windowStyle);
		newWindow.focus();
	}

	$(document).ready(function () {
		ShowLimitItems("fixedPurchaseHistory");
		ShowLimitItems("orderHistory");
		ShowLimitItems("memberRankHistory");
		ShowLimitItems("dmShippingHistory");

		var isScrollToUserInvoice = (getUrlParameter("invoice") == "1");
		if(isScrollToUserInvoice)
		{
			ScrollToUserInvoice();
		}
	});

	function ShowLimitItems(historyItem)
	{
		$('.' + historyItem).hide();
		$('.' + historyItem).each(function(index) {
			if (index >= <%=Constants.ITEMS_HISTORY_FIRST_DISPLAY %>) return false;

			$(this).show();
		});
	}

	function ShowMore(historyItem, showMore)
	{
		$('.' + historyItem).show();
		$('#' + showMore).hide();
	}

	// Get Url Parameter
	var getUrlParameter = function getUrlParameter(sParam) {
		var sPageURL = window.location.search.substring(1),
			sURLVariables = sPageURL.split('&'),
			sParameterName,
			i;

		for (i = 0; i < sURLVariables.length; i++) {
			sParameterName = sURLVariables[i].split('=');

			if (sParameterName[0] === sParam) {
				return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
			}
		}
	};

	// Scroll To User Invoice
	function ScrollToUserInvoice() {
		// Scroll
		$('html,body').animate({
			scrollTop: $("#anchorUserInvoice").offset().top
		}, 0);
	}
</script>
