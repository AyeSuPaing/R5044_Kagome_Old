<%--
=========================================================================================================
  Module      : 問合せ入力画面(InquiryInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/Inquiry/InquiryInput.aspx.cs" Inherits="Form_Inquiry_InquiryInput" Title="問合せ入力ページ" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<div id="dvUserContents">
	<%-- 問合せ入力系パンくず --%>
	<div id="dvHeaderRegistClumbs">
		<p><img src="../../Contents/ImagesPkg/inquiry/clumbs_inquiry_1.gif" alt="問合せ情報の入力" /></p>
	</div>

		<h2>問合せ情報の入力</h2>

	<div id="dvUserInquiryInput" class="unit">
		
		<%-- 問合せ項目入力フォーム --%>
		<div class="dvUserInfo">
			<h3>問合せ情報</span></h3>

		<%-- 問合せ項目開始 --%>
		<table cellspacing="0">
			<%-- 問合せ件名 --%>
			<tr>
				<th>問合せ件名<span class="necessary">*</span></th>
				<td>
					<asp:DropDownList ID="ddlInquiryTitle" runat="server">
						<asp:ListItem Text="選択してください" Value=""></asp:ListItem>
						<asp:ListItem Text="商品について" Value="商品について"></asp:ListItem>
						<asp:ListItem Text="注文・お届けについて" Value="注文・お届けについて"></asp:ListItem>
						<asp:ListItem Text="サイトの利用方法について" Value="サイトの利用方法について"></asp:ListItem>
						<asp:ListItem Text="その他のお問合せ" Value="その他のお問合せ"></asp:ListItem>
					</asp:DropDownList>
					<asp:CustomValidator runat="Server"
						ControlToValidate="ddlInquiryTitle"
						ValidationGroup="Inquiry"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
					<%= WebSanitizer.HtmlEncode(this.ProductInquiry)%>
					<asp:DropDownList ID="ddlProductVariation" runat="server" Width="400"></asp:DropDownList>
					<asp:HiddenField ID="hfProductTitlePrefix" runat="server" Value="商品名 : " />
				</td>
			</tr>
			<%-- 問合せ内容 --%>
			<tr>
				<th>問合せ内容<span class="necessary">*</span></th>
				<td>
					<asp:TextBox ID="tbInquiryText" runat="server" TextMode="MultiLine" Rows="10" CssClass="inquirytext" Text=""></asp:TextBox>
					<asp:CustomValidator runat="Server"
						ControlToValidate="tbInquiryText"
						ValidationGroup="Inquiry"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
				</td>
			</tr>
			<%-- 氏名 --%>
			<tr>
				<th><%: ReplaceTag("@@User.name.name@@") %><span class="necessary">*</span></th>
				<td>
					<table cellspacing="0">
						<tr>
							<td>
								<% SetMaxLength(WtbUserName1, "@@User.name1.length_max@@"); %>
								<span class="fname">姓</span><asp:TextBox id="tbUserName1" Runat="server" CssClass="nameFirst"></asp:TextBox></td>
							<td>
								<% SetMaxLength(WtbUserName2, "@@User.name2.length_max@@"); %>
								<span class="lname">名</span><asp:TextBox id="tbUserName2" Runat="server" CssClass="nameLast"></asp:TextBox><span class="notes">※全角入力</span></td>
						</tr>
						<tr>
							<td><span class="notes">例：山田</span></td>
							<td><span class="notes">太郎</span></td>
						</tr>
					</table>
					<asp:CustomValidator runat="Server"
						ControlToValidate="tbUserName1"
						ValidationGroup="Inquiry"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
					<asp:CustomValidator runat="Server"
						ControlToValidate="tbUserName2"
						ValidationGroup="Inquiry"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
				</td>
			</tr>
			<%-- 氏名（かな） --%>
			<% if (this.IsJapanese){ %>
			<tr>
				<th><%: ReplaceTag("@@User.name_kana.name@@") %>
					<% if (this.IsJapanese){ %>
					<span class="necessary">*</span>
					<% } %>
				</th>
				<td>
					<table cellspacing="0">
						<tr>
							<td>
								<% SetMaxLength(WtbUserNameKana1, "@@User.name_kana1.length_max@@"); %>
								<span class="fname">姓</span><asp:TextBox id="tbUserNameKana1" Runat="server" CssClass="nameFirst"></asp:TextBox></td>
							<td>
								<% SetMaxLength(WtbUserNameKana2, "@@User.name_kana2.length_max@@"); %>
								<span class="lname">名</span><asp:TextBox id="tbUserNameKana2" Runat="server" CssClass="nameLast"></asp:TextBox><span class="notes">※全角ひらがな入力</span></td>
						</tr>
						<tr>
							<td><span class="notes">例：やまだ</span></td>
							<td><span class="notes">たろう</span></td>
						</tr>
					</table>
					<asp:CustomValidator runat="Server"
						ControlToValidate="tbUserNameKana1"
						ValidationGroup="Inquiry"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
					<asp:CustomValidator runat="Server"
						ControlToValidate="tbUserNameKana2"
						ValidationGroup="Inquiry"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
				</td>
			</tr>
			<% } %>
			<%-- メールアドレス --%>
			<tr>
				<th>
					<%: ReplaceTag("@@User.mail_addr.name@@") %>
					<span class="necessary">*</span>
				</th>
				<td>
					<asp:TextBox id="tbUserMailAddr" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
					<asp:CustomValidator runat="Server"
						ControlToValidate="tbUserMailAddr"
						ValidationGroup="Inquiry"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
				</td>
			</tr>
			<%-- メールアドレス(確認) --%>
			<tr>
				<th>
					<%: ReplaceTag("@@User.mail_addr.name@@") %>(確認用)
					<span class="necessary">*</span>
				</th>
				<td>
					<asp:TextBox id="tbUserMailAddrConf" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
					<asp:CustomValidator runat="Server"
						ControlToValidate="tbUserMailAddrConf"
						ValidationGroup="Inquiry"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
				</td>
			</tr>
			<%-- 電話番号 --%>
			<tr>
				<th>
					<%: ReplaceTag("@@User.tel1.name@@") %>
					<span class="necessary">*</span>
				</th>
				<td>
					<asp:TextBox ID="tbTel1" MaxLength="13" runat="server" CssClass="shortTel" Type="tel"/>
					<asp:CustomValidator runat="Server"
						ControlToValidate="tbTel1"
						ValidationGroup="Inquiry"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
				</td>
			</tr>
		</table>
		<%-- 問合せ項目ここまで --%>

		<div class="dvUserBtnBox">
			<p>
				<span id="spBack" runat="server" visible="false">
					<a href="<%= WebSanitizer.HtmlEncode(this.ProductPageURL) %>" onclick="return exec_submit();" class="btn btn-large">戻る</a></span>
				<span><asp:LinkButton ID="lbConfirm" runat="server" ValidationGroup="Inquiry" OnClientClick="return exec_submit();" OnClick="lbConfirm_Click" class="btn btn-large btn-inverse">
					確認する</asp:LinkButton></span>
			</p>
		</div>
		
	</div>
</div>
</div>

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
