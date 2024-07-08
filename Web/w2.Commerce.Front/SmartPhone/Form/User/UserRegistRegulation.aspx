<%--
=========================================================================================================
  Module      : スマートフォン用会員登録規約画面(UserRegistRegulation.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="UserRegistRegulationMessage" Src="~/Form/User/UserRegistRegulationMessage.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Smartphone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRegistRegulation.aspx.cs" Inherits="Form_User_UserRegistRegulation" Title="会員新規登録規約ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-order user-regulation">
<div class="order-unit">
	<h2>会員規約</h2>
	<p class="msg">会員登録の前に必ず「会員規約」をお読み下さい。</p>
	<div class="regulation">
		<uc:UserRegistRegulationMessage runat="server" />
	</div>
</div>
<div class="order-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbAgree2" runat="server" OnClick="lbAgree_Click" class="btn">規約に同意して先に進む</asp:LinkButton>
	</div>
</div>
</section>
</asp:Content>