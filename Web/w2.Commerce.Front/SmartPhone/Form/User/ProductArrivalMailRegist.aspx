<%--
=========================================================================================================
  Module      : スマートフォン用入荷通知メール登録画面(ProductArrivalMailRegist.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" masterpagefile="~/SmartPhone/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="~/Form/User/ProductArrivalMailRegist.aspx.cs" Inherits="Form_User_ProductArrivalMailRegist" title="入荷通知メール登録ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user user-product-arrival-mail-regist">

<div id="divInput" class="user-unit" runat="server">
	<h2>入荷お知らせメール申込</h2>

	<div class="msg">
	こちらのアイテムが入荷した際、メールにてお知らせします。（通知期限：<%#: DateTimeUtility.ToStringFromRegion(this.ExpiredDate, DateTimeUtility.FormatType.EndOfYearMonth1Letter) %>）</span>
	<br />
	<span class="attention" visible="<%# this.IsLoggedIn == false %>" runat="server">
		※会員の方はログインしてから登録すると、登録状況を後で確認できます。
	</span>
	</div>

	<script type="text/javascript" language="javascript">
		<!--
		function setCheckbox() {
			if (document.getElementById("<%= cbMailAddr.ClientID %>") != null) {
				document.getElementById("<%= cbMailAddr.ClientID %>").checked = true;
			}
		}
		//-->
	</script>

	<dl class="user-form">
		<dt>商品名</dt>
		<dd><%#: ProductCommon.CreateProductJointName(this.ProductName, this.VariationName1, this.VariationName2, this.VariationName3) %></dd>
		<dd visible="<%# this.HasVariation %>" runat="server">
			表示名1 / 表示名2：<%# WebSanitizer.HtmlEncode(this.VariationName1) %> / <%# WebSanitizer.HtmlEncode(this.VariationName2) %>
		</dd>
		<dt>通知先アドレス</dt>
		<dd class="mail" visible="<%# this.IsLoggedIn && this.HasPcAddr %>" runat="server">
			<asp:CheckBox id="cbUserPcAddr" runat="server" Checked="<%# this.IsPcAddrRegistered || this.HasPcAddr %>" Enabled="<%# this.IsPcAddrRegistered == false %>" Text='<%#: ReplaceTag("@@User.mail_addr.name@@") + " (" +this.PcAddr + ")" %>' />
		</dd>
		<dd class="mobile" visible="<%# Constants.MOBILEOPTION_ENABLED && this.IsLoggedIn && this.HasMbAddr %>" runat="server">
			<asp:CheckBox id="cbUserMobileAddr" runat="server" Checked="<%# Constants.MOBILEOPTION_ENABLED && (this.IsMbAddrRegistered || (this.HasPcAddr == false)) %>" Enabled="<%# this.IsMbAddrRegistered == false %>" Text='<%#: ReplaceTag("@@User.mail_addr2.name@@") + " (" + this.MbAddr + ")" %>' />
		</dd>
		<dd class="mail">
			<% if (StringUtility.ToEmpty(this.ErrorMessage) != "") {%><p class="attention"><%: this.ErrorMessage %></p><%} %>
			<asp:CheckBox id="cbMailAddr" runat="server" Checked="<%# this.IsLoggedIn == false %>" Text="その他アドレス" />
			<w2c:ExtendedTextBox ID="tbMailAddr" runat="server" MaxLength="256" onfocus="setCheckbox()" />
		</dd>
	</dl>

	<div class="user-footer">
		<div class="button-next">
			<asp:LinkButton OnClick="lbRegister_Click" runat="server" CssClass="btn">登録する</asp:LinkButton>
		</div>
	</div>
</div>

<div id="divComplete" class="user-unit" runat="server">
	<h2>入荷通知メール登録 完了</h2>
	<div class="msg">
		再入荷お知らせメールのご登録を受け付けました。こちらの商品が入荷次第、ご登録のメールアドレスにお知らせします。<br />
		※受け付け完了後、折り返しメールにてご連絡いたします。メールが届かない場合、お申し込みのメールアドレスに誤りがある可能性がございます。
	</div>

	<div class="user-footer">
		<div class="button-next">
			<a href="Javascript:window.close();" class="btn">閉じる</a>
		</div>
	</div>

	<script type="text/javascript" language="javascript"> 
	<!--
		setTimeout("self.close();window.opener.location.reload();", 3000) 
	//--> 
	</script>

</div>

</section>
</asp:Content>