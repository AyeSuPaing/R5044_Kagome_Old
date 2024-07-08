<%--
=========================================================================================================
  Module      : カスタムページテンプレート画面(CustomPageTemplate.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Title="はじめての方へ" Language="C#" Inherits="ContentsPage" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>

<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<section class="wrap-custom-page first">
<h2>はじめての方へ</h2>
<div class="unit">
	<h4>【ようこそ、w2オンラインショップへ】</h4>
	こちらでしか取り扱っていない商品も多数ございますので、ぜひお気軽にご利用ください。
</div>
<div class="unit">
	<h4>【ショップのご案内】</h4>
	<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "?bid=brand1") %>">brandname1</a><br />
</div>
<div class="unit">
	<h4>【ご利用ガイド】</h4>
	初めての方でも安心してご購入いただけるよう、オンラインショップのご利用方法やご利用時の注意点についてご説明しています。<br />
	<br />
	→<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/guide01.aspx") %>">送料について</a><br />
	→<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/guide02.aspx") %>">返品について</a><br />
	→<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/guide03.aspx") %>">支払い方法について</a><br />
	→<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Page/guide04.aspx") %>">ヘルプ</a>
</div>
<div class="unit">
	<h4>【定期配送サービス】</h4>
	メリット1　日本全国送料無料<br />
	メリット2　毎回割引価格でお届け<br />
	メリット3　お届けサイクルを選択可能<br />
	メリット4　他の商品も最大○％割引
</div>
<div class="unit">
	<h4>【お問い合せ】</h4>
	■サポートセンター<br />
	フリーダイヤル：0120-000-000<br />
	＜受付時間：平日　9:00～18:00＞<br />
	土日祝日・年末年始・夏季などの特別休業日を除く
	<br />
	通信販売に関するお問い合わせ専用ダイヤルです。
</div>
</section>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
</asp:Content>