﻿<%--
=========================================================================================================
  Module      : カスタムページテンプレート画面(CustomPageTemplate.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Title="送料について" Language="C#" Inherits="ContentsPage" MasterPageFile="~/Form/Common/DefaultPage.master" %>
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
		<h2>送料について</h2>
		<div class="unit">
			<h4>【送料】</h4>
			1回のご購入金額が○○円未満（税抜）の場合は全国一律○円（税込）となります。1回のご購入金額が○○円（税抜）以上の場合、送料無料といたします。<br />
			また、「定期配送サービス」は送料無料でお届けいたします。さらに、「定期配送サービス」をご注文いただき、その定期商品と同時発送の場合に限り、「定期配送サービス」以外の商品も送料無料となります。<br /><br /><br />
			<h4>【お届けできる地域】</h4>
			日本国外および離島等、一部の地域への配送はお受けできない場合がございます。予めご了承ください。<br />
			商品が欠品等の場合、その旨を表示する場合がございますので予めご了承ください。その他の販売条件について画面上表記する場合がございます。<br /><br /><br />
			<h4>【お届けまでの期間】</h4>
			飲料は、原則としてお申し込みから約２営業日後、佐川急便でのお届けとなります。サプリメントは、ヤマトメール便にてお届けいたしますので、お申し込みから約1週間でのお届けとなります。品切れなどにより納期が大幅に遅れる場合には、 事前にご連絡をいたします。<br />
			※ゴールデンウィーク、夏季、年末年始などの特別休暇および配達地域の状況により、上記日数よりお届けまでお時間がかかる場合がございますのでご了承ください。
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