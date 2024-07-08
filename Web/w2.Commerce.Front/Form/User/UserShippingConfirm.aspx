<%--
=========================================================================================================
  Module      : アドレス帳確認画面(UserShippingConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserShippingConfirm.aspx.cs" Inherits="Form_User_UserShippingConfirm" Title="アドレス帳確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<%-- パンくず --%>
	<div id="dvHeaderUserShippingClumbs">
	<p>
		<img src="../../Contents/ImagesPkg/user/clumbs_usershipping_2.gif" alt="入力内容の確認" /></p>
	</div>

		<h2>入力内容の確認</h2>

	<div id="dvUserShippingInput" class="unit">
		<div class="dvContentsInfo">
			<p>登録する住所に間違いがなければ、「<%= (lbRegist.Visible == true) ? "登録" : "更新"%>する」ボタンを押してください。</p>
		</div>
		<div class="dvUserShippingInfo">
			<h3>アドレス帳情報</h3>
			<table cellspacing="0">
				<tr>
					<th>配送先名</th>
					<td><%: this.UserShipping.Name %></td>
				</tr>
				<tr>
					<%-- 氏名 --%>
					<th><%: ReplaceTag("@@User.name.name@@") %></th>
					<td>
						<%: this.UserShipping.ShippingName1 %><%: this.UserShipping.ShippingName2 %>&nbsp;様
						<% if (this.IsShippingAddrJp) { %>
						（<%: this.UserShipping.ShippingNameKana1 %><%: this.UserShipping.ShippingNameKana2 %>&nbsp;さま）
						<% } %>
					</td>
				</tr>
				<tr>
					<%-- 住所 --%>
					<th>
						<%: ReplaceTag("@@User.addr.name@@") %>
					</th>
					<td>
						<% if (this.IsShippingAddrJp){ %>
						〒<%: this.UserShipping.ShippingZip %><br />
						<% } %>
						<%: this.UserShipping.ShippingAddr1 %>
						<%: this.UserShipping.ShippingAddr2 %><br />
						<%: this.UserShipping.ShippingAddr3 %>
						<%: this.UserShipping.ShippingAddr4 %>
						<%: this.UserShipping.ShippingAddr5 %><br />
						<% if (this.IsShippingAddrJp == false) { %>
						<%: this.UserShipping.ShippingZip %><br />
						<% } %>
						<%: this.UserShipping.ShippingCountryName %>
					</td>
				</tr>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.company_name.name@@")%>・
						<%: ReplaceTag("@@User.company_post_name.name@@")%></th>
					<td>
						<%: this.UserShipping.ShippingCompanyName %><br />
						<%: this.UserShipping.ShippingCompanyPostName %>
					</td>
				</tr>
				<%} %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %>
					</th>
					<td>
						<%: this.UserShipping.ShippingTel1 %></td>
				</tr>
			</table>
		</div>
		<div class="dvUserBtnBox">
			<p>
				<asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn btn-large">戻る</asp:LinkButton>
				<asp:LinkButton ID="lbRegist" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn btn-large btn-inverse">登録する</asp:LinkButton>
				<asp:LinkButton ID="lbModify" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn btn-large btn-inverse">更新する</asp:LinkButton>
			</p>
		</div>

	</div>
</div>
</asp:Content>