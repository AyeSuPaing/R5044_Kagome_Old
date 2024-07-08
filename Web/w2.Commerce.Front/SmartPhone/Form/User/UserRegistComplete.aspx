<%--
=========================================================================================================
  Module      : スマートフォン用会員登録完了画面(UserRegistComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRegistComplete.aspx.cs" Inherits="Form_User_UserRegistComplete" Title="会員新規登録完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-order user-regist-complete">
<div class="order-unit">
	<h2>登録完了</h2>
	<%-- メッセージ --%>
	<p>
		<%: ShopMessage.GetMessage("ShopName") %>会員にご登録頂きありがとうございます。<br />
		ご登録内容を、<%: this.UserMailAddr %>にお送りしましたのでご確認下さい。<br /><br />今後とも、<%: ShopMessage.GetMessage("ShopName") %>をどうぞ宜しくお願い申し上げます。<br />※メールが届かない場合は、大変お手数ですが下記までご連絡をお願いします。
	</p>
	<p><%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %></p>
</div>
<div class="order-footer">
	<div class="button-next">
		<div id="spNextUrl" runat="server" Visible="false">
			<asp:LinkButton ID="lbShipping" runat="server" OnClick="lbShipping_Click" CssClass="btn">配送先入力画面へ</asp:LinkButton>
		</div>
		<asp:LinkButton ID="lbTopPage" runat="server" OnClick="lbTopPage_Click" CssClass="btn">トップページへ</asp:LinkButton>
	</div>
</div>
</section>
<w2c:FacebookConversionAPI
	EventName="CompleteRegistration"
	UserId="<%#: this.LoginUserId %>"
	CustomDataContentName="Content name"
	CustomDataValue="500.000"
	CustomDataCurrency="JPY"
	CustomDataStatus="Status"
	runat="server" />
</asp:Content>