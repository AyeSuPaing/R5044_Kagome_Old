<%--
=========================================================================================================
  Module      : LINEミニアプリ共通ヘッダー(BodyMiniAppHeader.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BodyMiniAppHeader.ascx.cs" Inherits="MiniApp_Form_Common_BodyMiniAppHeader" %>

<header class="s-header-wrap">
	<div class="s-header">
		<h1 class="s-header__logo">w2Commerce</h1>

		<% if (this.CanDisplayMemberCard) { %>
		<div class="s-header__memberId">
			<a href="<%= Constants.PATH_ROOT + Constants.PAGE_LINE_MINIAPP_MEMBER_CARD %>" class="s-header__memberIdLink">
				<img src="<%= Constants.PATH_ROOT %>MiniApp/Image/miniapp_icon_barcode.png" alt="会員証" class="s-header__memberIdImg">
			</a>
		</div>
		<% } %>
	</div>
</header>