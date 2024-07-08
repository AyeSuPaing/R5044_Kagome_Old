<%--
=========================================================================================================
  Module      : スマートフォン用共通フッタ出力コントローラ(SmartPhoneFooterMain.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="AccessLogTrackerScript" Src="~/Form/Common/AccessLogTrackerScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryTree" Src="~/SmartPhone/Form/Common/Product/BodyProductCategoryTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductColorSearchBox" Src="~/SmartPhone/Form/Common/Product/ProductColorSearchBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateList" Src="~/SmartPhone/Form/Common/Coordinate/BodyCoordinateList.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyFooterMain.ascx.cs" Inherits="Form_Common_BodyFooterMain" %>
<%@ Import Namespace="w2.Domain.Coordinate" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<%-- ▽編集可能領域：フッタ領域▽ --%>

<%--▼ TOP、商品系ページ表示 ▼--%>
<% if (this.IsTopAndProductPage) { %>
<div class="footer-wrap">
	<%--▼ ブランドOP有効 ▼--%>
　<% if (Constants.PRODUCT_BRAND_ENABLED){ %>
  <section class="search-brand unit">
  <h3 class="title">ブランドから探す</h3>
  <nav>
    <ul class="global-nav-1">
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "DefaultBrandTop.aspx?bid=brand1") %>">brandname1</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "DefaultBrandTop.aspx?bid=brand2") %>">brandname2</a></li>
    </ul>
  </nav>
  </section>
　<% } %>
　<%--▲ ブランドOP有効 ▲--%>
  <uc:ProductColorSearchBox runat="server" />
  <uc:BodyProductCategoryTree runat="server" />

  <div class="all-products unit">
    <a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Form/Product/ProductList.aspx") %>" class="btn">全ての商品を見る</a>
  </div>

  <section class="infomation unit">
  <h3 class="title">インフォメーション</h3>
    <ul class="global-nav-2 clearfix">
      <%--▼ ログイン前 ▼--%>
      <%if (this.IsLoggedIn == false) { %>
      <li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION) %>">新規会員登録</a></li>
      <%} %>
      <%--▲ ログイン前 ▲--%>
      <%--▼ ログイン中 ▼--%>
      <%if (this.IsLoggedIn) { %>
      <li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>">マイページ</a></li>
      <%} %>
       <%--▲ ログイン中 ▲--%>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/first.aspx") %>">はじめての方へ</a></li>
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
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST) %>">カート</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Form/User/MailMagazineRegistInput.aspx") %>">メルマガ登録・解除</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST) %>">お気に入り</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/guide01.aspx") %>">送料について</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/guide02.aspx") %>">返品について</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/guide03.aspx") %>">支払い方法について</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/guide04.aspx") %>">ヘルプ</a></li>
      <% if (Constants.REALSHOP_OPTION_ENABLED) { %>
      <li><a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_SHOP_LIST %>">店舗一覧</a></li>
      <% } %>
    </ul>
  </section>

  <div class="page-top">
    <a href="#page-top" class="btn">ページトップへ</a>
  </div>

  <div class="footer-menu">
    <ul>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/privacy.aspx") %>">個人情報保護方針</a></li>
      <li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/termofuse.aspx") %>">特定商取引法に基づく表示</a></li>
    </ul>
  </div>

</div>
<% } %>
<%--▲ TOP、商品系ページ表示 ▲--%>
<%--▼ TOP、商品系ページ以外表示 ▼--%>
<% if (this.IsTopAndProductPage == false) { %>
<div class="page-top">
  <a href="#page-top" class="btn">ページトップ</a>
</div>
<% } %>
<%--▲ TOP、商品系ページ以外表示 ▲--%>

<footer class="footer clearfix">
  <div class="inner">
  <div class="contact-form">
    <a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Form/inquiry/InquiryInput.aspx") %>" class="btn">お問い合わせ</a>
  </div>
  <div class="contact-info">
    <dl>
      <dt>受付時間</dt>
      <dd>10:00 - 17:00（平日のみ）</dd>
      <dt>電話</dt>
      <dd><a href="tel:0300000000">03-0000-0000</a></dd>
      <dt>メール</dt>
      <dd>address@address.com</dd>
    </dl>
  </div>
  </div>
  <small>© XXXXXX All Rights Reserved.</small>
  <nav class="switch-view">
    <a href="<%= WebSanitizer.UrlAttrHtmlEncode(this.ChangeToPcSiteUrl) %>">PCサイト</a> | スマフォサイト
  </nav>
</footer>
<%-- △編集可能領域△ --%>

<%-- w2アクセスログトラッカー出力 --%>
<uc:AccessLogTrackerScript id="AccessLogTrackerScript1" runat="server" />