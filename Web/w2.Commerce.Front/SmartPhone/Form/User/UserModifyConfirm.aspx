<%--
=========================================================================================================
  Module      : 会員登録変更確認画面(UserModifyConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendModify" Src="~/SmartPhone/Form/Common/User/BodyUserExtendModify.ascx" %>
<%@ page language="C#" masterpagefile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserModifyConfirm.aspx.cs" Inherits="Form_User_UserModifyConfirm" title="登録情報変更確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user user-mofify-comfirm">
<div class="user-unit">
	<h2>入力内容の確認</h2>
	<p class="msg">お客様の入力された内容は以下の通りでよろしいでしょうか？よろしければ「更新する」ボタンを押して下さい。</p>
	<dl class="user-form">
		<dt>
			<%-- 氏名 --%>
			<%: ReplaceTag("@@User.name.name@@") %>
		</dt>
		<dd>
			<%: this.UserInput.Name1 %>&nbsp;<%: this.UserInput.Name2 %>
			<% if (this.IsUserAddrJp) { %>
			&nbsp;（<%: this.UserInput.NameKana1 %>&nbsp;<%: this.UserInput.NameKana2 %>）
			<% } %>
		</dd>
		<%if (Constants.PRODUCTREVIEW_ENABLED) { %>
		<dt>
			<%-- ニックネーム --%>
			<%: ReplaceTag("@@User.nickname.name@@") %>
		</dt>
		<dd>
			<%: this.UserInput.NickName %>
		</dd>
		<%} %>
		<%if (this.UserInput.Birth != null) {%>
		<dt>
			<%-- 生年月日 --%>
			<%: ReplaceTag("@@User.birth.name@@") %>
		</dt>
		<dd>
			<%: this.UserInput.BirthYear %>年&nbsp;<%: this.UserInput.BirthMonth %>月&nbsp;<%: this.UserInput.BirthDay %>日
		</dd>
		<%} %>
		<dt>
			<%-- 性別 --%>
			<%: ReplaceTag("@@User.sex.name@@") %>
		</dt>
		<dd>
			<%: this.UserInput.SexValueText %>
		</dd>
		<dt>
			<%-- ＰＣメールアドレス --%>
			<%: ReplaceTag("@@User.mail_addr.name@@") %>
		</dt>
		<dd>
			<%: this.UserInput.MailAddr %>
			<%if (this.IsPcSiteOrOfflineUser && Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
			<p class="msg">※ログイン時に利用します</p>
			<%} %>
		</dd>
		<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
		<dt>
			<%-- モバイルメールアドレス --%>
			<%: ReplaceTag("@@User.mail_addr2.name@@") %>
		</dt>
		<dd>
			<%: this.UserInput.MailAddr2 %>
			<%if ((this.IsPcSiteOrOfflineUser == false) && Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
			<p class="msg">※ログイン時に利用します</p>
			<%} %>
		</dd>
		<%} %>
		<dt>
			<%: ReplaceTag("@@User.addr.name@@") %>
		</dt>
		<dd>
			<%: this.IsUserAddrJp ? "〒" + this.UserInput.Zip : "" %>
			<%: this.UserInput.Addr1 %><%: this.UserInput.Addr2 %><%: this.UserInput.Addr3 %><br />
			<%: this.UserInput.Addr4 %><%: this.UserInput.Addr5 %><%: (this.IsUserAddrJp == false) ? this.UserInput.Zip : "" %><%: this.UserInput.AddrCountryName %>
		</dd>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
			<dt>
				<%-- 企業名 --%>
				<%: ReplaceTag("@@User.company_name.name@@")%>
			</dt>
			<dd>
				<%: this.UserInput.CompanyName %>
			</dd>

			<dt>
				<%-- 部署名 --%>
				<%: ReplaceTag("@@User.company_post_name.name@@")%>
			</dt>
			<dd>
				<%: this.UserInput.CompanyPostName %>
			</dd>
		<%} %>
		<dt>
			<%-- 電話番号 --%>
			<%: ReplaceTag("@@User.tel1.name@@", this.UserAddrCountryIsoCode) %>
		</dt>
		<dd>
			<%: this.UserInput.Tel1 %>
		</dd>
		<dt>
			<%: ReplaceTag("@@User.tel2.name@@", this.UserAddrCountryIsoCode) %>
		</dt>
		<dd>
			<%: this.UserInput.Tel2 %>
		</dd>
		<dt>
			<%: ReplaceTag("@@User.mail_flg.name@@") %>
		</dt>
		<dd>
			<%: this.UserInput.MailFlgValueText %>
		</dd>
		<% if (this.ExistsUserExtend) { %>
		<dt>
			アンケート
		</dt>
		<dd class="extend">
			<%-- ユーザ拡張項目 --%>
			<uc:BodyUserExtendModify ID="ucBodyUserExtendModify" runat="server" HasInput="false" HasRegist="false" />
		</dd>
		<% } %>
	</dl>
</div>

<div class="user-unit">
	<h2>ログイン情報</h2>
	<dl class="user-form">
		<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
		<dt>
			<%-- ログインID --%>
			<%: ReplaceTag("@@User.login_id.name@@") %>
		</dt>
		<dd>
			<%: this.UserInput.LoginId %>
		</dd>
		<%} %>
		<dt>
			<%-- パスワード --%>
			<%: ReplaceTag("@@User.password.name@@") %>
		</dt>
		<dd>
			<%if (StringUtility.ToEmpty(this.UserInput.Password) != "") { %>
				<%: StringUtility.ChangeToAster(this.UserInput.Password) %>
			<%} else {%>
				（変更なし）
			<%} %>
		</dd>
	</dl>
</div>

<div class="user-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbModity" runat="server" OnClick="lbModity_Click" class="btn">更新する</asp:LinkButton>
	</div>
	<div class="button-prev">
		<asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn">戻る</asp:LinkButton>
	</div>
</div>

</section>
</asp:Content>