﻿<%--
=========================================================================================================
  Module      : スマートフォン用カスタムページテンプレート画面(CustomPageTemplate.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Title="特定商取引に関する法律に基づく表記" Language="C#" Inherits="ContentsPage" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" %>
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
<section class="wrap-custom-page privacy">
<h2>特定商取引に関する法律に基づく表記</h2>
<div class="unit">
	販売業者	XXXX株式会社<br />
	運営責任者	○○○○<br />
	住所	〒XXX-XXXX<br />
	XXXX XXXX<br />
	電話番号	03-0000-0000<br />
	メールアドレス	support@-----<br />
	URL	http://----
</div>
<div class="unit">
	<h4>商品以外の必要代金</h4>
	■消費税<br />
	<br />
	■送料<br />
	○○円（北海道・沖縄は○○円）となります。<br />
	※○○円以上のご購入で送料無料となります。<br />
	<br />
	■代引手数料<br />
	1万円未満：○円<br />
	1万円以上～3万円未満：○円<br />
	3万円以上～10万円未満：○円<br />
	10万円以上～50万円まで：○円<br />
	※配送業者はヤマト運輸となります<br />
	注文方法	インターネット販売<br />
	支払方法	代金引換、クレジットカード決済がご利用になれます。<br />
	<br />
	■代金引換の場合<br />
	ご配送時にお荷物と引き換えに商品代金及び諸手数料を担当ドライバーにお支払い下さい。<br />
	<br />
	■クレジットカード決済の場合<br />
	分割払い、リボ払いも対応しております。<br />
	カード会社によっては最低限度額がございます。<br />
	詳しくは、ご利用のカード会社へ直接お問合せください。
</div>
<div class="unit">
	<h4>引渡し時期</h4>
	ご注文から3日以内に出荷させていただきます。店頭に商品のあるものに関しては、最短即日出荷で対応させていただきます。<br />
	予約、入荷待ちの商品に関しては、入荷後の出荷となります。<br />
	※店頭でも販売している関係上、場合によっては時間差で売り切れになってしまう場合がございますがご了承ください。<br />
	返品・交換について	<br />
	商品到着後3営業日以内にご連絡頂き、送料をご負担いただける場合は、お客様都合による商品のサイズ交換、返品をお受け致します。<br />
	<br />
	ご注文商品と異なる商品や商品不良の場合は、お手数ではございますが商品到着後7日以内にメールもしくはお電話にてご連絡下さい。<br />
	未使用のものに限り、不良品の返品・交換をお受けいたします。不良品の返品・交換時に発生する返送料は弊社が負担いたします。
</div>
</section>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
</asp:Content>
