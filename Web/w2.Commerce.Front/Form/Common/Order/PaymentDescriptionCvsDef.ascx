<%--
=========================================================================================================
  Module      : コンビニ(後払い)注意書きユーザーコントロール(PaymentDescriptionCvsDef.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="BaseUserControl" %>

<%-- コンビニ(後払い)注意書き表示 --%>
<%if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.YamatoKa) { %>
	<strong>◆『クロネコ代金後払いサービス 請求書後払い』の詳細</strong>
	<div>
		ご注文商品の配達完了を基にヤマトクレジットファイナンス株式会社から購入者様へ請求書をお届けいたします。請求書の記載事項に従って発行日から14日以内にお支払い下さい。
		主要なコンビニエンスストア・郵便局のいずれでもお支払いできます。
	</div>
	<strong>◆ご注意</strong>
	<div>
		代金後払いのご注文には、ヤマトクレジットファイナンス株式会社の提供するクロネコ代金後払いサービス規約が適用され、サービスの範囲内で個人情報を提供し、立替払い契約を行います。<br />
		ご利用限度額は累計残高で54,000円（税込）迄です。<span style="text-decoration: underline">短期間の内に繰り返し複数のご注文をされた場合、与信審査が通らない場合がございます。</span><br />
		詳細は下記「クロネコ代金後払いサービス」をクリックしてご確認ください。
	</div>
	<br />
	<div class="txtc" style="text-align: center">
		<a href="https://business.kuronekoyamato.co.jp/service/lineup/payment/banner/afterpayment_03.html" target="_blank">
			<img src="https://business.kuronekoyamato.co.jp/service/lineup/payment/banner/images/afterpayment/ban_ap_01.gif" width="250" height="50" alt="クロネコ代金後払いサービス" border="0" />
		</a>
	</div>
<% } %>

<%-- GMO後払い注意書き表示 --%>
<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo) { %>
	<strong>◆GMO後払い</strong>
<% } %>

<%-- Atodene後払い注意書き表示 --%>
<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene) { %>
	<strong>◆Atodene後払い</strong>
<% } %>

<%-- DSK後払い注意書き表示 --%>
<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk) { %>
	<div>
		<a href="https://www.dsk-atobarai.jp/purchaser/" target="_blank">
			<img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/DskDeferred/dsk_deferred_banner.png" alt="DSK後払い" border="0" alt="_blank" style="display:block; margin: 0 auto;"/></a>
		<p>
		① 「ＤＳＫ後払い」は、電算システムグループの株式会社ＤＳテクノロジーズが提供するサービスです。<br>
			ご利用にあたり、利用規約に同意のうえ、お申し込みください。<br>
			※ＤＳＫ後払いご利用規約は<a href="https://www.dsk-atobarai.jp/purchaser/#terms" target="_blank">こちら</a>をご覧ください。<br>
		② 購入者様の個人情報およびご注文の内容は、株式会社ＤＳテクノロジーズが行う与信審査および請求書発行等の業務に必要な範囲内で株式会社ＤＳテクノロジーズに提供します。<br>
		③ 後払い手数料：■■■円（税込）<br>
		④ ご請求書は、商品●●●●（と同梱して・とは別に「ＤＳＫ後払い」より・と同梱もしくは商品とは別に「ＤＳＫ後払い」より）お送りいたしますので、<br>
			商品のお受取り後、ご請求書に記載のお支払期限までに●●●●（コンビニエンスストア・郵便局・各種スマートフォンアプリ決済）にてお支払いください。<br>
		⑤ 払込金受領証（払込受領書）は、領収書となりますので最低６か月間は大切に保管してください。<br>
		⑥ ご利用は、お買い物累計（未払金額合計）が55,000円（税込）までとなります。<br>
		⑦ ご注文の都度、所定の審査がございます。審査の結果により、他の決済方法のご利用をお願いする場合がございます。<br>
		⑧ 「よくあるお問い合わせ」は<a href="https://www.dsk-atobarai.jp/purchaser/#faq" target="_blank">こちら</a>をご覧ください。<br>
			ＤＳＫ後払いサポートセンター<br>
			メールでのお問い合わせ：support@dsk-atobarai.jp<br>
			お電話でのお問い合わせ：058-279-3474（受付時間： 9:00-17:00 平日のみ）<br>
			※お問い合わせの際は、ご請求書に記載のお問い合わせ番号をお伝えください。<br>
		</p>
	</div>
<% } %>

<%-- 後払い.com注意書き表示 --%>
<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom) { %>
	<strong>◆後払い.com</strong>
<% } %>

<%-- スコア後払い注意書き表示 --%>
<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score) { %>
	<div>
		●後払い手数料：■■■ 円（税込）<br/>
		●利用限度額：55,000 円（税込）<br/>
		●払込票は商品とは別に郵送されます。発行から14 日以内にコンビニでお支払いください。<br/>
		●代金譲渡等株式会社SCORE が提供するサービスの範囲内で個人情報を提供します。<br/>
		与信審査の結果により他の決済方法をご利用していただく場合もございますので同意の上申込ください。<br/>
		提供する目的：後払い決済のための審査及び代金回収や債権管理のため<br/>
		株式会社SCORE よりサービスに関する情報のお知らせのため<br/>
		提供する項目：氏名、電話番号、住所、E MAIL アドレス、購入商品、金額等<br/>
		提供の手段：専用システムにて実施<br/>
		●「スコア後払い決済サービス」では以下の場合サービスをご利用いただけません。予めご了承ください。<br/>
		・郵便局留め・運送会社営業所留め（営業所での引き取り）<br/>
		・商品の転送<br/>
		・「病院」「ホテル」「学校」のご住所でご名義が職員以外の場合<br/>
		・コンビニ店頭での受け渡し<br/>
		バナーをクリックしたリンク先のページで、必ず詳細を確認してください。<br/>
		個人情報の提供に関する問合せ先：▲▲▲ ▲▲▲ ▲▲▲▲<br/>
		ご利用者が未成年の場合は、法定代理人の利用同意を得てご利用ください。<br/>
		●下記<br/>
		スマートフォンアプリからお支払い可能です 。<br/>
		・LINEPay 請求書支払い<br/>
		・楽天銀行コンビニ支払サービス（アプリで払込票支払）<br/>
		・ｄ払い請求書払い<br/>
		・ファミペイ請求書支払い<br/>
		<a href="https://www.scoring.jp/consumer/" target="_blank"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/ScoreDeferred/score_deferred_banner_combini.png" width="290" alt="SCORE後払い"/></a>
		<a href="https://www.scoring.jp/consumer/" target="_blank"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/ScoreDeferred/score_deferred_banner_mobile.png" width="290" alt="SCORE後払い"/></a>
	</div>
<% } %>

<%-- ベリトランス後払い注意書き表示 --%>
<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans) { %>
	<strong>◆ベリトランス後払い</strong>
<% } %>
