<%--
=========================================================================================================
  Module      : カスタムページテンプレート画面(CustomPageTemplate.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Title="はじめての方へ" Language="C#" Inherits="ContentsPage" MasterPageFile="~/Form/Common/DefaultPage.master" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>

<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table id="tblLayout">
<tr>
<td>
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="dvUserBox">
	<div id="dvUserContents">
		<h2>はじめての方へ</h2>
		<div class="unit">
			<h3>【ようこそ、w2オンラインショップへ】</h3>
			こちらでしか取り扱っていない商品も多数ございますので、ぜひお気軽にご利用ください。<br /><br /><br />
			<h3>【ショップのご案内】</h3>
			<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "?bid=brand1") %>">brandname1</a><br /><br /><br />
			<h3>【ご利用ガイド】</h3>
			初めての方でも安心してご購入いただけるよう、オンラインショップのご利用方法やご利用時の注意点についてご説明しています。<br />
			<br />
			→<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/guide01.aspx") %>">送料について</a><br />
			→<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/guide02.aspx") %>">返品について</a><br />
			→<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/guide03.aspx") %>">支払い方法について</a><br />
			→<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/guide04.aspx") %>">ヘルプ</a><br /><br /><br />
			<h3>【定期配送サービス】</h3>
			メリット1　日本全国送料無料<br />
			メリット2　毎回割引価格でお届け<br />
			メリット3　お届けサイクルを選択可能<br />
			メリット4　他の商品も最大○％割引<br /><br /><br />
			<h3>【お問い合せ】</h3>
			■サポートセンター<br />
			フリーダイヤル：0120-000-000<br />
			＜受付時間：平日　9:00～18:00＞<br />
			土日祝日・年末年始・夏季などの特別休業日を除く
			<br />
			通信販売に関するお問い合わせ専用ダイヤルです。
		</div>
	</div>
</div>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
</asp:Content>