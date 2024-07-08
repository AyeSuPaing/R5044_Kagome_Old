<%--
=========================================================================================================
  Module      : 会員登録変更確認画面(UserModifyConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendModify" Src="~/Form/Common/User/BodyUserExtendModify.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserModifyConfirm.aspx.cs" Inherits="Form_User_UserModifyConfirm" Title="登録情報変更確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<%-- 会員情報変更系パンくず --%>
	<div id="dvHeaderModifyClumbs">
		<p>
			<img src="../../Contents/ImagesPkg/user/clumbs_modify_2.gif" alt="入力内容の確認" /></p>
	</div>

		<h2>入力内容の確認</h2>

	<div id="dvUserModifyConfirm" class="unit">
		<%-- メッセージ --%>
		<div class="dvContentsInfo">
			<p>お客様の入力された内容は以下の通りでよろしいでしょうか？<br />よろしければ「更新する」ボタンを押して下さい。</p>
		</div>
		
		<div class="dvUserInfo">
			<h3>お客様情報</h3>
			<table cellspacing="0">
				<tr>
					<th>
						<%-- 氏名 --%>
						<%: ReplaceTag("@@User.name.name@@") %>
					</th>
					<td>
						<%: this.UserInput.Name1 %>
						<%: this.UserInput.Name2 %>
						<% if (this.IsUserAddrJp) { %>
						（<%: this.UserInput.NameKana1 %>
						<%: this.UserInput.NameKana2 %>）
						<% } %>
					</td>
				</tr>
				<%if (Constants.PRODUCTREVIEW_ENABLED) { %>
				<tr>
					<th>
						<%-- ニックネーム --%>
						<%: ReplaceTag("@@User.nickname.name@@") %>
					</th>
					<td>
						<%: this.UserInput.NickName %></td>
				</tr>
				<%} %>
				<tr>
					<th>
						<%-- 生年月日 --%>
						<%: ReplaceTag("@@User.birth.name@@") %>
					</th>
					<td>
					<%if (this.UserInput.Birth != null) {%>
						<%: this.UserInput.BirthYear %>年
						<%: this.UserInput.BirthMonth %>月
						<%: this.UserInput.BirthDay %>日
					<%} %></td>
				</tr>
				<tr>
					<th>
						<%-- 性別 --%>
						<%: ReplaceTag("@@User.sex.name@@") %>
					</th>
					<td>
						<%: this.UserInput.SexValueText %></td>
				</tr>
				<tr>
					<th>
						<%-- ＰＣメールアドレス --%>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>
					</th>
					<td>
						<%: this.UserInput.MailAddr %>
						<%if (this.IsPcSiteOrOfflineUser && Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
						<span class="notes">※ログイン時に利用します</span>
						<%} %></td>
				</tr>
				<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
				<tr>
					<th>
						<%-- モバイルメールアドレス --%>
						<%: ReplaceTag("@@User.mail_addr2.name@@") %>
					</th>
					<td>
						<%: this.UserInput.MailAddr2 %>
						<%if ((this.IsPcSiteOrOfflineUser == false) && Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
						<span class="notes">※ログイン時に利用します</span>
						<%} %></td>
				</tr>
				<% } %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.addr.name@@") %>
					</th>
					<td>
						<% if (this.IsUserAddrJp) { %>
						〒<%: this.UserInput.Zip %><br />
						<% } %>
						<%: this.UserInput.Addr1 %> <%: this.UserInput.Addr2 %><br />
						<%: this.UserInput.Addr3 %> <%: this.UserInput.Addr4 %><br />
						<%: this.UserInput.Addr5 %> 
						<% if (this.IsUserAddrJp == false) { %>
						<%: this.UserInput.Zip %><br />
						<% } %>
						<%: this.UserInput.AddrCountryName %>
					</td>
				</tr>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
				<tr>
					<th>
						<!-- 企業名 -->
						<%: ReplaceTag("@@User.company_name.name@@")%>
					</th>
					<td>
						<%: this.UserInput.CompanyName %></td>
				</tr>
				<tr>
					<th>
						<!-- 部署名 -->
						<%: ReplaceTag("@@User.company_post_name.name@@")%>
					</th>
					<td>
						<%: this.UserInput.CompanyPostName %></td>
				</tr>
				<%}%>
				<tr>
					<th>
						<%-- 電話番号 --%>
						<%: ReplaceTag("@@User.tel1.name@@", this.UserAddrCountryIsoCode) %>
					</th>
					<td>
						<%: this.UserInput.Tel1 %></td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.tel2.name@@", this.UserAddrCountryIsoCode) %>
					</th>
					<td><%: WebSanitizer.HtmlEncode(this.UserInput.Tel2) %></td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.mail_flg.name@@") %>
					</th>
					<td>
						<%: this.UserInput.MailFlgValueText %></td>
				</tr>
				<%-- ユーザー拡張項目　HasInput:true(入力画面)/false(確認画面)　HasRegist:true(新規登録)/false(登録編集) --%>
				<uc:BodyUserExtendModify ID="ucBodyUserExtendModify" runat="server" HasInput="false" HasRegist="false" />
			</table>
		</div>
		<div class="dvLoginInfo">
			<h3>ログイン情報</h3>
			<table cellspacing="0">
				<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
				<tr>
					<th>
					<%-- ログインID --%>
					<%: ReplaceTag("@@User.login_id.name@@") %>
					</th>
					<td>
						<%: this.UserInput.LoginId %></td>
				</tr>
				<%} %>
				<tr>
					<th>
					<%-- パスワード --%>
					<%: ReplaceTag("@@User.password.name@@") %>
					</th>
					<td>
						<%if (StringUtility.ToEmpty(this.UserInput.Password) != "") { %>
							<%: StringUtility.ChangeToAster(this.UserInput.Password) %>
						<%} else {%>
							（変更なし）
						<%} %>
					</td>
				</tr>
			</table>
		</div>
		<div class="dvUserBtnBox">
			<p>
				<span><asp:LinkButton ID="lbSend" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn btn-large">
					戻る</asp:LinkButton></span>
				<span><asp:LinkButton ID="lbModity" runat="server" OnClick="lbModity_Click" class="btn btn-large btn-inverse">
					更新する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>
</asp:Content>