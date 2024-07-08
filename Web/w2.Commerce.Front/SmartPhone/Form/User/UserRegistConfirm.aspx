<%--
=========================================================================================================
  Module      : スマートフォン用会員登録確認画面(UserRegistConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendRegist" Src="~/SmartPhone/Form/Common/User/BodyUserExtendRegist.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRegistConfirm.aspx.cs" Inherits="Form_User_UserRegistConfirm" Title="会員新規登録確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-order user-regist-comfirm">
<div class="order-unit">
	<h2>会員登録情報の確認</h2>
	<p class="msg">お客様の入力された内容は以下の通りでよろしいでしょうか？<br />よろしければ「登録する」ボタンを押して下さい。</p>
	<dl class="order-form">
		<dt>
			<%: ReplaceTag("@@User.name.name@@") %>
		</dt>
		<dd><%: this.UserInput.Name1 %> <%: this.UserInput.Name2 %></dd>
		<% if (this.IsUserAddrJp) { %>
		<dt>
			<%: ReplaceTag("@@User.name_kana.name@@") %>
		</dt>
		<dd><%: this.UserInput.NameKana1 %> <%: this.UserInput.NameKana2 %></dd>
		<% } %>
		<%if (Constants.PRODUCTREVIEW_ENABLED) { %>
		<dt>
			<%: ReplaceTag("@@User.nickname.name@@") %>
		</dt>
		<dd><%: this.UserInput.NickName %></dd>
		<%} %>
		<dt>
			<%: ReplaceTag("@@User.birth.name@@") %>
		</dt>
		<dd><%: this.UserInput.BirthYear %>年<%: this.UserInput.BirthMonth %>月<%: this.UserInput.BirthDay %>日</dd>
		<dt>
			<%: ReplaceTag("@@User.sex.name@@") %>
		</dt>
		<dd><%: this.UserInput.SexValueText %></dd>
		<dt>
			<%: ReplaceTag("@@User.mail_addr.name@@") %>
		</dt>
		<dd><%: this.UserInput.MailAddr %>
			<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
			<p class="msg">※ログイン時に利用します</p>
			<%} %>
		</dd>
		<dt><%: ReplaceTag("@@User.addr.name@@") %></dt>
		<dd>
			<% if (this.IsUserAddrJp) { %>〒<%: this.UserInput.Zip %><br /><% } %>
			<%: this.UserInput.Addr1 %><%: this.UserInput.Addr2 %><%: this.UserInput.Addr3 %><br />
			<%: this.UserInput.Addr4 %> <%: this.UserInput.Addr5 %> <%: (this.IsUserAddrJp == false) ? this.UserInput.Zip : "" %> <%: this.UserInput.AddrCountryName %>
		</dd>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
		<dt>
			<%: ReplaceTag("@@User.company_name.name@@") %>
		</dt>
		<dd><%: this.UserInput.CompanyName %></dd>
		<dt>
			<%: ReplaceTag("@@User.company_post_name.name@@") %>
		</dt>
		<dd><%: this.UserInput.CompanyPostName %></dd>
		<%} %>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@") %>
		</dt>
		<dd><%: this.UserInput.Tel1 %></dd>
		<dt><%: ReplaceTag("@@User.tel2.name@@") %></dt>
		<dd><%: WebSanitizer.HtmlEncode(this.UserInput.Tel2) %></dd>
		<dt><%: ReplaceTag("@@User.mail_flg.name@@") %></dt>
		<dd><%: this.UserInput.MailFlgValueText %></dd>
		<dt>アンケート</dt>
		<dd class="extend">
			<uc:BodyUserExtendRegist ID="ucBodyUserExtendRegist" runat="server" HasInput="false" HasRegist="true" />
		</dd>
	</dl>
</div>
<%if ((Constants.PAYMENT_GMO_POST_ENABLED) && (this.UserInput.BusinessOwner != null)) { %>
	<div class="order-unit">
		<h2>GMO</h2>
		<dl class="order-form">
			<dt>
				<%: ReplaceTag("@@User.OwnerName1.name@@") %>
			</dt>
			<dd>
				<%: this.UserInput.BusinessOwner.OwnerName1 %>
			</dd>
			<dt>
				<%: ReplaceTag("@@User.OwnerName2.name@@") %>
			</dt>
			<dd>
				<%: this.UserInput.BusinessOwner.OwnerName2 %>
			</dd>
			<dt>
				<%: ReplaceTag("@@User.OwnerNameKana1.name@@") %>
			</dt>
			<dd>
				<%: this.UserInput.BusinessOwner.OwnerNameKana1 %>
			</dd>
			<dt>
				<%: ReplaceTag("@@User.OwnerNameKana2.name@@") %>
			</dt>
			<dd>
				<%: this.UserInput.BusinessOwner.OwnerNameKana2 %>
			</dd>
			<dt>
					<%: ReplaceTag("@@User.OwnerBirth.name@@") %>
			</dt>
			<dd>
				<%if ((this.UserInput.BusinessOwner.BirthYear + this.UserInput.BusinessOwner.BirthMonth + this.UserInput.BusinessOwner.BirthDay).Length != 0){%>
					<%: this.UserInput.BusinessOwner.BirthYear %>年
					<%: this.UserInput.BusinessOwner.BirthMonth %>月
					<%: this.UserInput.BusinessOwner.BirthDay %>日
				<%} %>
			</dd>
			<dt>
				<%: ReplaceTag("@@User.RequestBudget.name@@") %>
			</dt>
			<dd>
				<%: CurrencyManager.ToPrice(this.UserInput.BusinessOwner.RequestBudget) %> 
			</dd>
		</dl>
	</div>
<% } %>
<%if ((Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) || this.IsVisible_UserPassword) { %>
<div class="order-unit">
	<h2>ログイン情報</h2>
	<dl class="order-form">
	<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
		<dt>
			<%: ReplaceTag("@@User.login_id.name@@") %>
		</dt>
		<dd><%: this.UserInput.LoginId %></dd>
	<%} %>
		<%-- ソーシャルログイン連携されている場合はパスワードスキップ --%>
		<%if (this.IsVisible_UserPassword){ %>
		<dt>
			<%: ReplaceTag("@@User.password.name@@") %>
		</dt>
		<dd><%: StringUtility.ChangeToAster(this.UserInput.Password) %></dd>
		<% } %>
	</dl>
</div>
<% } %>

<div class="order-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbSend" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn">送信</asp:LinkButton>
	</div>
	<div class="button-prev">
		<asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn">戻る</asp:LinkButton>
	</div>
</div>

</section>
</asp:Content>