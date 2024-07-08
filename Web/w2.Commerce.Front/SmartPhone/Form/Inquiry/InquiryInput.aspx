<%--
=========================================================================================================
  Module      : 問合せ入力画面(InquiryInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ page language="C#" masterpagefile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/Inquiry/InquiryInput.aspx.cs" Inherits="Form_Inquiry_InquiryInput" title="問合せ入力ページ" MaintainScrollPositionOnPostBack="true" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<section class="wrap-user inquiry-input">
<div class="user-unit">
	<h2>問合せ情報の入力</h2>
	<dl class="user-form">
		<dt>
			<%-- 問合せ件名 --%>
			件名<span class="require">※</span>
		</dt>
		<dd class="title">
			<p class="attention">
				<asp:CustomValidator runat="Server" ControlToValidate="ddlInquiryTitle" ValidationGroup="Inquiry" ValidateEmptyText="true" SetFocusOnError="true" />
			</p>
			<asp:DropDownList ID="ddlInquiryTitle" runat="server">
				<asp:ListItem Text="選択してください" Value=""></asp:ListItem>
				<asp:ListItem Text="商品について" Value="商品について"></asp:ListItem>
				<asp:ListItem Text="注文・お届けについて" Value="注文・お届けについて"></asp:ListItem>
				<asp:ListItem Text="サイトの利用方法について" Value="サイトの利用方法について"></asp:ListItem>
				<asp:ListItem Text="その他のお問合せ" Value="その他のお問合せ"></asp:ListItem>
			</asp:DropDownList>
			<%= WebSanitizer.HtmlEncode(this.ProductInquiry)%>
			<asp:DropDownList ID="ddlProductVariation" runat="server" Width="300" />
			<asp:HiddenField ID="hfProductTitlePrefix" runat="server" Value="商品名 : " />
		</dd>
		<dt>
			<%-- 問合せ内容 --%>
			内容<span class="require">※</span>
		</dt>
		<dd class="text">
			<p class="attention">
				<asp:CustomValidator runat="Server" ControlToValidate="tbInquiryText" ValidationGroup="Inquiry" ValidateEmptyText="true" SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbInquiryText" runat="server" TextMode="MultiLine" Rows="5" CssClass="inquirytext" Text=""></w2c:ExtendedTextBox>
		</dd>
		<dt>
			<%-- 氏名 --%>
			<%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span>
		</dt>
		<dd class="name">
			<p class="attention">
				<asp:CustomValidator runat="Server" ControlToValidate="tbUserName1" ValidationGroup="Inquiry" ValidateEmptyText="true" SetFocusOnError="true" />
				<asp:CustomValidator runat="Server" ControlToValidate="tbUserName2" ValidationGroup="Inquiry" ValidateEmptyText="true" SetFocusOnError="true" />
			</p>
			<% tbUserName1.Attributes["placeholder"] = ReplaceTag("@@User.name1.name@@"); %>
			<% tbUserName2.Attributes["placeholder"] = ReplaceTag("@@User.name2.name@@"); %>
			<% SetMaxLength(this.WtbUserName1, "@@User.name1.length_max@@"); %>
			<% SetMaxLength(this.WtbUserName2, "@@User.name2.length_max@@"); %>
			<w2c:ExtendedTextBox id="tbUserName1" Runat="server" CssClass="nameFirst"></w2c:ExtendedTextBox>&nbsp;
			<w2c:ExtendedTextBox id="tbUserName2" Runat="server" CssClass="nameLast"></w2c:ExtendedTextBox><br />
		</dd>
		<% if (this.IsJapanese) { %>
		<dt>
			<%-- 氏名（かな） --%>
			<%: ReplaceTag("@@User.name_kana.name@@") %>
			<% if (this.IsJapanese) { %>
			<span class="require">※</span>
			<% } %>
		</dt>
		<dd class="name-kana">
			<p class="attention">
				<asp:CustomValidator runat="Server" ControlToValidate="tbUserNameKana1" ValidationGroup="Inquiry" ValidateEmptyText="true" SetFocusOnError="true" />
				<asp:CustomValidator runat="Server" ControlToValidate="tbUserNameKana2" ValidationGroup="Inquiry" ValidateEmptyText="true" SetFocusOnError="true" />
			</p>
			<% tbUserNameKana1.Attributes["placeholder"] = ReplaceTag("@@User.name_kana1.name@@"); %>
			<% tbUserNameKana2.Attributes["placeholder"] = ReplaceTag("@@User.name_kana2.name@@"); %>
			<% SetMaxLength(this.WtbUserNameKana1, "@@User.name_kana1.length_max@@"); %>
			<% SetMaxLength(this.WtbUserNameKana2, "@@User.name_kana2.length_max@@"); %>
			<w2c:ExtendedTextBox id="tbUserNameKana1" Runat="server" CssClass="nameFirst"></w2c:ExtendedTextBox>&nbsp;
			<w2c:ExtendedTextBox id="tbUserNameKana2" Runat="server" CssClass="nameLast"></w2c:ExtendedTextBox><br />
		</dd>
		<% } %>
		<dt>
			<%-- メールアドレス --%>
			<%: ReplaceTag("@@User.mail_addr.name@@") %>
			<span class="require">※</span>
		</dt>
		<dd class="address">
			<p class="attention">
				<asp:CustomValidator runat="Server" ControlToValidate="tbUserMailAddr" ValidationGroup="Inquiry" ValidateEmptyText="true" SetFocusOnError="true" />
			</p>
			<% tbUserMailAddr.Attributes["placeholder"] = ReplaceTag("@@User.mail_addr.name@@"); %>
			<% SetMaxLength(this.WtbUserMailAddr, "@@User.mail_addr.length_max@@"); %>
			<w2c:ExtendedTextBox id="tbUserMailAddr" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
		</dd>
		<dt>
			<%-- メールアドレス(確認) --%>
			メールアドレス(確認用)<span class="require">※</span>
		</dt>
		<dd class="address">
			<p class="attention">
				<asp:CustomValidator runat="Server" ControlToValidate="tbUserMailAddrConf" ValidationGroup="Inquiry" ValidateEmptyText="true" SetFocusOnError="true" />
			</p>
			<% tbUserMailAddrConf.Attributes["placeholder"] = ReplaceTag("@@User.mail_addr.name@@") + "確認"; %>
			<% SetMaxLength(this.WtbUserMailAddrConf, "@@User.mail_addr.length_max@@"); %>
			<w2c:ExtendedTextBox id="tbUserMailAddrConf" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
		</dd>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@") %>
			<span class="require">※</span>
		</dt>
		<dd class="tel">
			<p class="attention">
				<asp:CustomValidator runat="Server" ControlToValidate="tbTel1" ValidationGroup="Inquiry" ValidateEmptyText="true" SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbTel1" type="tel" style="width:100%;" MaxLength="13" CssClass="shortTel" Runat="server" /><br />
		</dd>
	</dl>
</div>

	<div class="user-footer">
		<div class="button-next">
			<asp:LinkButton ID="lbConfirm" runat="server" ValidationGroup="Inquiry" OnClientClick="return exec_submit();" OnClick="lbConfirm_Click" CssClass="btn">入力内容を確認する</asp:LinkButton>
		</div>
		<div id="spBack" class="button-prev" runat="server" visible="false">
			<a href="<%= WebSanitizer.HtmlEncode(this.ProductPageURL) %>" onclick="return exec_submit();" Class="btn" >戻る</a>
		</div>
	</div>
</section>

<script type="text/javascript">
<!--
	bindEvent();

	<%-- イベントをバインドする --%>
	function bindEvent() {
		bindExecAutoKana();
		bindExecAutoChangeKana();
	}

	<%-- 氏名（姓・名）の自動振り仮名変換のイベントをバインドする --%>
	function bindExecAutoKana() {
		execAutoKanaWithKanaType(
			$("#<%= tbUserName1.ClientID %>"),
			$("#<%= tbUserNameKana1.ClientID %>"),
			$("#<%= tbUserName2.ClientID %>"),
			$("#<%= tbUserNameKana2.ClientID %>"));
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		execAutoChangeKanaWithKanaType(
			$("#<%= tbUserNameKana1.ClientID %>"),
			$("#<%= tbUserNameKana2.ClientID %>"));
	}
//-->
</script>

</asp:Content>