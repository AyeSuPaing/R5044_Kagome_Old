<%--
=========================================================================================================
  Module      : スマートフォン用アドレス帳確認画面(UserShippingConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserShippingConfirm.aspx.cs" Inherits="Form_User_UserShippingConfirm" Title="アドレス帳確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user user-shipping-list">
<div class="user-unit">
	<h2>入力内容の確認</h2>
	<div class="msg">
		<p>登録する住所に間違いがなければ、「登録する」ボタンを押してください。</p>
	</div>
	<dl class="user-form">
		<dt>配送先名</dt>
		<dd>
			<%: this.UserShipping.Name %>
		</dd>
		<dt>
			<%-- 氏名 --%>
			<%: ReplaceTag("@@User.name.name@@") %>
		</dt>
		<dd>
			<%: this.UserShipping.ShippingName1 %><%: this.UserShipping.ShippingName2 %>&nbsp;様<br />
			<% if (this.IsShippingAddrJp) { %>
			（<%: this.UserShipping.ShippingNameKana1 %><%: this.UserShipping.ShippingNameKana2 %>&nbsp;さま）
			<% } %>
		</dd>
		<dt>
			<%: ReplaceTag("@@User.addr.name@@") %>
		</dt>
		<dd>
			<%if (this.IsShippingAddrJp) {%>〒<%: this.UserShipping.ShippingZip %><br /><%} %>
			<%: this.UserShipping.ShippingAddr1 %>
			<%: this.UserShipping.ShippingAddr2 %>
			<%: this.UserShipping.ShippingAddr3 %><br />
			<%: this.UserShipping.ShippingAddr4 %>
			<%: this.UserShipping.ShippingAddr5 %><br />
			<%if (this.IsShippingAddrJp == false) {%><%: this.UserShipping.ShippingZip %><br /><%} %>
			<%: this.UserShipping.ShippingCountryName %>
		</dd>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
			<dt>
				<%: ReplaceTag("@@User.company_name.name@@")%>
			</dt>
			<dd>
				<%: this.UserShipping.ShippingCompanyName %>
			</dd>
			<dt>
				<%: ReplaceTag("@@User.company_post_name.name@@")%>
			</dt>
			<dd>
				<%: this.UserShipping.ShippingCompanyPostName %>
			</dd>
		<%} %>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@") %>
		</dt>
		<dd>
			<%: this.UserShipping.ShippingTel1 %>
		</dd>
	</dl>
</div>

<div class="user-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbRegist" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn">登録する</asp:LinkButton>
		<asp:LinkButton ID="lbModify" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn">更新する</asp:LinkButton>
	</div>
	<%--
	<div class="button-prev">
		<asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn">戻る</asp:LinkButton>
	</div>
	--%>
</div>
</section>
</asp:Content>