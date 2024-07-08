<%--
=========================================================================================================
  Module      : ヤマト後払いSMS認証連携注意書きユーザーコントロール(PaymentDescriptionSmsDef.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="BaseUserControl" %>

<%-- ヤマト後払いSMS認証連携注意書き表示 --%>
<%if (Constants.PAYMENT_SMS_DEF_KBN == Constants.PaymentSmsDef.YamatoKa) { %>
	<strong>◆『クロネコ代金後払いサービス スマホ後払い』の詳細</strong>
	<div>
		ご注文商品の配達完了を基にヤマトクレジットファイナンス株式会社から購入者様へSMSとEメールにてお支払い手続きメールをお届けします。<br/>
		メールの記載事項に従って発行日から14日以内に、主要なコンビニエンスストアにてお支払い下さい。
	</div>
	<strong>◆ご注意</strong>
	<div>代金後払いのご注文には、ヤマトクレジットファイナンス株式会社の提供するクロネコ代金後払いサービス規約が適用され、サービスの範囲内で個人情報を提供し、立替払い契約を行います。<br/>
		ご利用限度額は累計残高で5万円（税別）迄です。<br/>
		詳細はクロネコ代金後払いサービスをクリックしてご確認ください。<br/>
		※SMSを受信可能な電話番号が必要です。
	</div>
	<br />
	<div class="txtc" style="text-align: center">
		<a href="https://business.kuronekoyamato.co.jp/service/lineup/payment/sp-postpaid/" target="_blank">
			<img src="https://business.kuronekoyamato.co.jp/service/lineup/payment/banner/images/afterpayment/ban_sp_250_80_6.png" alt="クロネコ代金後払いサービス スマホタイプ" width="250" height="80" border="0">
		</a>
	</div>
<% } %>