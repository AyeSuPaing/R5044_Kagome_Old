<%--
=========================================================================================================
  Module      : スマートフォン用カスタムページテンプレート画面(CustomPageTemplate.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Title="返品について" Language="C#" Inherits="ContentsPage" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" %>
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
<section class="wrap-custom-page guide03">
<h2>返品について</h2>
<div class="unit">
	<h4>【返品・交換について】</h4>
	商品の発送には万全を期しておりますが、万一お届けいたしました商品に破損、汚損、欠陥、間違いなどがございましたら、商品を交換いたします。<br />
	商品到着後10日以内に下記「お問い合わせ先」までご連絡ください。<br />商品到着後10日を過ぎた場合には、返品・交換・キャンセルには応じかねますのでご了承ください。<br />
	<br />
	※外装パッケージ（箱）等の破損は、商品の破損と見なしておりません。ご了承ください。
</div>
<div class="unit">
	<h4>【サポートセンター】</h4>
	フリーダイヤル：0120-000-000<br />
	＜受付時間：平日　9:00～18:00＞<br />
	土日祝日・年末年始・夏季などの特別休業日を除く<br />
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
