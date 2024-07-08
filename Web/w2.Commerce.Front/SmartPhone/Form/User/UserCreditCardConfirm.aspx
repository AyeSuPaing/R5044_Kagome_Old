<%--
=========================================================================================================
  Module      : スマートフォン用ユーザクレジットカード確認画面(UserCreditCardConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserCreditCardConfirm.aspx.cs" Inherits="Form_User_UserCreditCardConfirm" Title="登録クレジットカード確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-order user-credit-card-comfirm">
<div class="user-unit">
	<h2>入力内容の確認</h2>
	<div class="msg">登録するクレジットカードに間違いがなければ、「登録する」ボタンを押してください。</div>
	<dl class="credit-card-list">
		<dt>登録名</dt>
		<dd><asp:Literal ID="lCardDispName" runat="server"></asp:Literal></dd>
		<div id="divCreditCardNoToken" runat="server">
		<%if (OrderCommon.CreditCompanySelectable) {%>
		<dt>カード会社</dt>
		<dd><asp:Literal ID="lCardCompanyName" runat="server"></asp:Literal></dd>
		<%} %>
		<dt>カード番号</dt>
		<dd>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigit" runat="server"></asp:Literal></dd>
		<dt>有効期限</dt>
		<dd><asp:Literal ID="lExpirationMonth" runat="server"></asp:Literal>/<asp:Literal ID="lExpirationYear" runat="server"></asp:Literal> (月/年)</dd>
		<dt>カード名義人</dt>
		<dd><asp:Literal ID="lAuthorName" runat="server"></asp:Literal></dd>
		</div>
	</dl>
</div>

<div class="user-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbSend" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" CssClass="btn">登録する</asp:LinkButton>
		<asp:LinkButton ID="lbUpdate" runat="server" OnClientClick="return exec_submit()" OnClick="lbUpdate_Click" CssClass="btn">更新する</asp:LinkButton>
	</div>
	<div class="button-prev">
		<asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" CssClass="btn">戻る</asp:LinkButton>
	</div>
</div>
</section>
</asp:Content>