<%--
=========================================================================================================
  Module      : LINEミニアプリ会員証画面(MemberCard.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/MiniApp/Form/Common/MiniApp.master" AutoEventWireup="true" CodeFile="MemberCard.aspx.cs" Inherits="MiniApp_User_MemberCard" Title="会員証画面" %>
<%@ Register Src="~/MiniApp/Form/Common/BodyMiniAppHeader.ascx" TagPrefix="uc" TagName="BodyMiniAppHeader" %>
<%@ Register Src="~/MiniApp/Form/Common/BodyMiniAppFooter.ascx" TagPrefix="uc" TagName="BodyMiniAppFooter" %>
<%@ Register Src="~/MiniApp/Form/Common/BodyMemberCardBarcode.ascx" TagPrefix="uc" TagName="BodyMemberCardBarcode" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>MiniApp/Css/line.css">
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
	<uc:BodyMiniAppHeader runat="server" />

	<main class="l-container-wrap">
		<section class="l-container">
			<div class="m-barcodeContent">
				<h2 class="m-barcodeContent__title">
					<% if (this.IsTempLoggedIn) { %>
					Temporary<br />
					<% } %>
					Member's Card<br />
					<span class="m-barcodeContent__subtitle"><%: this.IsTempLoggedIn ? "仮" : "" %>会員証</span>
				</h2>
				<uc:BodyMemberCardBarcode runat="server" />
			</div>
			<div class="m-barcodeContent__btn-wrap">
				<a href="<%= GetBackUrl() %>" class="m-barcodeContent__btn">閉じる</a>
			</div>
		</section>
	</main>

	<uc:BodyMiniAppFooter runat="server" />
</asp:Content>