<%--
=========================================================================================================
  Module      : 入荷通知メール登録画面(ProductArrivalMailRegist.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="~/Form/User/ProductArrivalMailRegist.aspx.cs" Inherits="Form_User_ProductArrivalMailRegist" Title="入荷通知メール登録ページ" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
	<link href="../../Css/common.css" rel="stylesheet" type="text/css" media="all" />
	<link href="../../Css/product.css" rel="stylesheet" type="text/css" media="all" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="divProductArrivalMail">
	<div id="divInput" runat="server">
		<h1>入荷通知メール登録</h1>
		<h2>商品名 ： <%#: ProductCommon.CreateProductJointName(this.ProductName, this.VariationName1, this.VariationName2, this.VariationName3) %></h2>
		<div>
			<span>こちらのアイテムが入荷した際、メールにてお知らせします。（通知期限：<%#: DateTimeUtility.ToStringFromRegion(this.ExpiredDate, DateTimeUtility.FormatType.EndOfYearMonth1Letter) %>）</span>
			<br />
			<span visible="<%# this.IsLoggedIn == false %>" runat="server" style="color: Red;">※会員の方はログインしてから登録すると、登録状況を後で確認できます。</span>
			<table>
				<tr visible="<%# this.HasVariation %>" runat="server">
					<th>表示名1 / 表示名2</th>
					<td><%# WebSanitizer.HtmlEncode(this.VariationName1) %> / <%# WebSanitizer.HtmlEncode(this.VariationName2) %></td>
				</tr>
				<tr>
					<th rowspan="4">通知先アドレス<span class="necessary">*</span></th>
				</tr>
				<tr visible="<%# this.IsLoggedIn && this.HasPcAddr %>" runat="server">
					<td>
						<asp:CheckBox id="cbUserPcAddr" runat="server" Checked="<%# this.IsPcAddrRegistered || this.HasPcAddr %>" Enabled="<%# this.IsPcAddrRegistered == false %>" Text='<%#: string.Format("{0} ({1})", ReplaceTag("@@User.mail_addr.name@@"), this.PcAddr) %>' />
					</td>
				</tr>
				<tr visible="<%# Constants.MOBILEOPTION_ENABLED && this.IsLoggedIn && this.HasMbAddr %>" runat="server">
					<td>
						<asp:CheckBox id="cbUserMobileAddr" runat="server" Checked="<%# Constants.MOBILEOPTION_ENABLED && (this.IsMbAddrRegistered || (this.HasPcAddr == false)) %>" Enabled="<%# this.IsMbAddrRegistered == false %>" Text='<%#:string.Format("{0} ({1})", ReplaceTag("@@User.mail_addr2.name@@"), this.MbAddr) %>' />
					</td>
				</tr>
				<tr>
					<td>
						<script type="text/javascript" language="javascript">
						<!--
							function setCheckbox() {
								document.getElementById("<%= cbMailAddr.ClientID %>").checked = true;
							}
						//-->
						</script>
						<asp:CheckBox id="cbMailAddr" runat="server" Checked="<%# this.IsLoggedIn == false %>" Text="その他" />
						<asp:TextBox ID="tbMailAddr" runat="server" CssClass="mailAddr" MaxLength="256" Width="200" onfocus="setCheckbox()" />
						<% if (StringUtility.ToEmpty(this.ErrorMessage) != "") {%>
							<br /><span class="error_inline"><%: this.ErrorMessage %></span>
						<%} %>
					</td>
				</tr>
			</table>
			<p class="btnClose">
				<asp:LinkButton OnClick="lbRegister_Click" runat="server" class="btn btn-large btn-inverse">登録する</asp:LinkButton>
			</p>
		</div>
	</div>
	<div id="divComplete" runat="server">
		<h1>入荷通知メール登録 完了</h1>
		<div>
			<span>入荷お知らせメールのご登録を受け付けました。<br />こちらの商品が入荷次第、ご登録のメールアドレスにお知らせします。</span>
			<br /><br />
			<span>※受け付け完了後、折り返しメールにてご連絡いたします。<br />　メールが届かない場合、お申し込みのメールアドレスに誤りがある可能性がございます。</span>
		</div>
		<p class="btnClose">
			<a href="Javascript:window.close();" class="btn btn-large btn-inverse">閉じる</a>
		</p>
		<script type="text/javascript" language="javascript"> 
		<!--
			setTimeout("self.close();window.opener.location.reload();", 3000) 
		//--> 
		</script>
	</div>
</div>
</asp:Content>