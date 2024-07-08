<%--
=========================================================================================================
  Module      : スマートフォン用共通ヘッダ出力コントローラ(SmartPhoneHeaderMain.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyProductSearchBox" Src="~/SmartPhone/Form/Common/Product/BodyProductSearchBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyMiniCart" Src="~/SmartPhone/Form/Common/BodyMiniCart.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryTree" Src="~/SmartPhone/Form/Common/Product/BodyProductCategoryTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="GlobalChangeList" Src="~/SmartPhone/Form/Common/Global/GlobalChangeList.ascx" %>
<%@ Register TagPrefix="uc" TagName="GlobalChangeIcon" Src="~/SmartPhone/Form/Common/Global/GlobalChangeIcon.ascx" %>
<%@ Control Language="c#" AutoEventWireup="true" Inherits="Form_Common_BodyHeaderMain"  CodeFile="~/Form/Common/BodyHeaderMain.ascx.cs" %>
<%--

下記は保持用のダミー情報です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<% this.Reload(); %>

<%-- ▽編集可能領域：共通ヘッダ領域▽ --%>

<%--▼ カート一覧、注文系ページ表示 ▼--%>
<% if (this.IsCartListAndOrderPage) { %>
<a id="page-top" name="page-top"></a>
<!-- header start -->
<header class="header clearfix">
<div class="logo-main"><h1><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>">w2Commerce DEMO SITE</a></h1></div>
</header>
<!-- header end -->
<% } %>
<%--▲ カート一覧、注文系ページ表示 ▲--%>

<%--▼ カート一覧、注文系ページ以外表示(1) ▼--%>
<% if (this.IsCartListAndOrderPage == false) { %>
<a id="page-top" name="page-top"></a>
<!-- header start -->
<header class="header clearfix">
<div class="logo-main"><h1><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>">w2Commerce DEMO SITE</a></h1></div>
<nav>
<ul class="header-global-nav">
  <%--▼ グローバル:切り替えタグ ▼--%>
  <uc:GlobalChangeIcon runat="server" />
  <%--▲ グローバル:切り替えタグ ▲--%>
  <li>
    <a href="javascript:void(0);" id="toggle-search">検索</a>
  </li><!--
  --><li>
    <a href="javascript:void(0);" id="toggle-global-menu">メニュー</a>
  </li>
</ul>
</nav>
</header>
<!-- header end -->

<section class="header-toggle toggle-global-menu">
  <div class="unit">
    <ul class="global-nav-2 clearfix">
      <%--▼ ログイン前 ▼--%>
      <%if (this.IsLoggedIn == false) { %>
      <li>
		<asp:LinkButton ID="lbUserRegist" runat="server" OnClick="lbUserRegist_Click" Text ="新規会員登録"/>
      </li>
      <%if (Constants.USEREAZYREGISTERSETTING_OPTION_ENABLED) {%>
      <li>
		<asp:LinkButton ID="lbUserEasyRegist" runat="server" OnClick="lbUserEasyRegist_Click" Text ="かんたん会員登録"/>
      </li>
      <%} %>
      <%} %>
      <%--▲ ログイン前 ▲--%>
      <%--▼ ログイン中 ▼--%>
      <%if (this.IsLoggedIn) { %>
      <li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>">マイページ</a></li>
      <%} %>
       <%--▲ ログイン中 ▲--%>
      <%--▼ ログイン前 ▼--%>
      <%if (this.IsLoggedIn == false) { %>
      <li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN + "?" + Constants.REQUEST_KEY_NEXT_URL + "=" + HttpUtility.UrlEncode(this.NextUrl)) %>">ログイン</a></li>
      <%} %>
      <%--▲ ログイン前 ▲--%>
      <%--▼ ログイン中 ▼--%>
      <%if (this.IsLoggedIn) { %>
      <li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGOFF) %>">ログアウト</a></li>
      <%} %>
       <%--▲ ログイン中 ▲--%>
      <li><a href="<%: this.CartListPageUrl %>">カート</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Form/User/MailMagazineRegistInput.aspx") %>">メルマガ登録・解除</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST) %>">お気に入り</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/guide04.aspx") %>">ヘルプ</a></li>
      <% if (Constants.REALSHOP_OPTION_ENABLED) { %>
      <li><a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_SHOP_LIST %>">店舗一覧</a></li>
      <% } %>
    </ul>
</div>
<% } %>
<%--▲ カート一覧、注文系ページ以外表示(1) ▲--%>

<%--▼ カート一覧、注文系ページ以外表示(2) ▼--%>
<%-- ミニカートパーツタグが未出力の場合、UpdatePanelエラーが発生するため、「display:none;」で非表示 --%>
<% divBodyMiniCart.Attributes["style"] = (this.IsCartListAndOrderPage ? "display:none;" : ""); %>
  <div id="divBodyMiniCart" runat="server" class="unit">
     <uc:BodyMiniCart runat="server" />
  </div>
<%--▲ カート一覧、注文系ページ以外表示(2) ▲--%>

<%--▼ カート一覧、注文系ページ以外表示(3) ▼--%>
<% if (this.IsCartListAndOrderPage == false) { %>
  <div class="unit">
    <h3 class="title">キーワードから探す</h3>
     <uc:BodyProductSearchBox runat="server" />
  </div>

  <div class="unit">
    <uc:BodyProductCategoryTree runat="server" />
  </div>

  <div class="button">
    <a href="javascript:void(0);" class="close btn">閉じる</a>
  </div>
</section>

<section class="header-toggle toggle-search">
  <uc:BodyProductSearchBox runat="server" />
  <div class="button">
    <a href="javascript:void(0);" class="close btn">閉じる</a>
  </div>
</section>

<%--▼ グローバル:切り替えタグ ▼--%>
<uc:GlobalChangeList runat="server" />
<%--▲ グローバル:切り替えタグ ▲--%>

<div class="header-user-nav">
  <ul>
  <%--▼ ログイン前 ▼--%>
  <%if (this.IsLoggedIn == false) { %>
	<!--
    <li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION) %>" class="btn">会員登録</a></li>
    <li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN + "?" + Constants.REQUEST_KEY_NEXT_URL + "=" + HttpUtility.UrlEncode(this.NextUrl)) %>" class="btn">
        ログイン
      </a>
    </li>
	-->
	<li style="font-size: 2px;">&nbsp;</li>
  <%} %>
  <%--▲ ログイン前 ▲--%>
  <%--▼ ログイン中 ▼--%>
  <%if (this.IsLoggedIn) { %>
    <li class="is-login">こんにちは、<%= WebSanitizer.HtmlEncode(this.LoginUserName) %> 様</li>
    <%-- ポイントオプションが有効な場合 --%>
    <% if (Constants.W2MP_POINT_OPTION_ENABLED) {%>
    <li class="is-login point">保有ポイント <%= WebSanitizer.HtmlEncode(GetNumeric(this.LoginUserPointUsable)) %>PT</li>
    <%} %>
  <%} %>
  <%--▲ ログイン中 ▲--%>
  </ul>
</div>
<% } %>
<%--▲ カート一覧、注文系ページ以外表示(3) ▲--%>

<%-- △編集可能領域△ --%>
</ContentTemplate>
</asp:UpdatePanel>

<%if (this.IsOrderPage == false) {%>
<%--▼ 注文系以外のページ ▼--%>
<%-- ▽編集可能領域：注文ページ以外で表示する領域▽ --%>

<%-- △編集可能領域△ --%>
<%--▲ 注文系以外のページ ▲--%>
<%} %>