<%--
=========================================================================================================
  Module      : LINEミニアプリトップページ(Top.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/MiniApp/Form/Common/MiniApp.master" AutoEventWireup="true" CodeFile="Top.aspx.cs" Inherits="MiniApp_Top" Title="ミニアプリ - トップページ" %>
<%@ Register src="~/MiniApp/Form/Common/BodyMiniAppHeader.ascx" tagPrefix="uc" tagName="BodyMiniAppHeader" %>
<%@ Register Src="~/MiniApp/Form/Common/BodyMiniAppFooter.ascx" TagPrefix="uc" TagName="BodyMiniAppFooter" %>
<%@ Register src="~/SmartPhone/Page/Parts/Parts900FAT_999.ascx" tagPrefix="uc" tagName="FeautureBanner" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>MiniApp/Css/line.css">
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
	<uc:BodyMiniAppHeader runat="server" />

	<main class="l-container-wrap">
		<section class="l-container">
			<div class="m-featureBanner-wrap">
				<div class="m-featureBanner">
					<uc:FeautureBanner runat="server" />
				</div>
			</div>
		</section>
		<section class="l-container">
			<div class="m-menuList">
				<a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE %>" class="m-menuListItem">
					<div class="m-menuListItem__img-wrap">
						<img src="<%= Constants.PATH_ROOT %>MiniApp/Image/miniapp_icon_mypage.png" alt="マイページ" class="m-menuListItem__img">
					</div>
					<p class="m-menuListItem__title">マイページ</p>
				</a>
				<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
				<a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_COUPON_BOX %>" class="m-menuListItem">
					<div class="m-menuListItem__img-wrap">
						<img src="<%= Constants.PATH_ROOT %>MiniApp/Image/miniapp_icon_coupon.png" alt="クーポン一覧" class="m-menuListItem__img">
					</div>
					<p class="m-menuListItem__title">クーポン一覧</p>
				</a>
				<% } %>
				<a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_LIST %>" class="m-menuListItem">
					<div class="m-menuListItem__img-wrap">
						<img src="<%= Constants.PATH_ROOT %>MiniApp/Image/miniapp_icon_order.png" alt="注文履歴" class="m-menuListItem__img">
					</div>
					<p class="m-menuListItem__title">注文履歴</p>
				</a>
				<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
				<a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_LIST %>" class="m-menuListItem">
					<div class="m-menuListItem__img-wrap">
						<img src="<%= Constants.PATH_ROOT %>MiniApp/Image/miniapp_icon_subscription.png" alt="定期購入履歴" class="m-menuListItem__img">
					</div>
					<p class="m-menuListItem__title">定期購入履歴</p>
				</a>
				<% } %>
				<a href="<%= Constants.PATH_ROOT %>" class="m-menuListItem">
					<div class="m-menuListItem__img-wrap">
						<img src="<%= Constants.PATH_ROOT %>MiniApp/Image/miniapp_icon_home.png" alt="サイトトップ" class="m-menuListItem__img">
					</div>
					<p class="m-menuListItem__title">サイトトップ</p>
				</a>
				<a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST %>" class="m-menuListItem">
					<div class="m-menuListItem__img-wrap">
						<img src="<%= Constants.PATH_ROOT %>MiniApp/Image/miniapp_icon_favorite.png" alt="お気に入り商品" class="m-menuListItem__img">
					</div>
					<p class="m-menuListItem__title">お気に入り商品</p>
				</a>
			</div>
		</section>
	</main>

	<uc:BodyMiniAppFooter runat="server" />
</asp:Content>