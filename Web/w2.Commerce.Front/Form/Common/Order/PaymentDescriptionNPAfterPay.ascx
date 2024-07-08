<%--
=========================================================================================================
  Module      : Payment Description NP After Pay(PaymentDescriptionNPAfterPay.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="Form_Common_Order_PaymentDescriptionNPAfterPay" CodeFile="~/Form/Common/Order/PaymentDescriptionNPAfterPay.ascx.cs" %>

<%-- NP After Pay Payment Description --%>
<p>
	<a href="http://np-atobarai.jp/about/mall.html/" target="_blank">
		<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/np_atobarai_banner.gif" alt="NP After Pay（NP後払い）スマホ決済"
			style="width:300px;height:120px;margin-bottom:10px"/>
	</a>
	<br />
		<strong>○このお支払方法の詳細</strong>
	<br />
		商品の到着を確認してから、「コンビニ」「郵便局」「銀行」「LINE Pay」で後払いできる安心・簡単な決済方法です。
	<br />
		請求書は、商品とは別に郵送されますので、発行から14日以内にお支払いをお願いします。
	<br />
		<strong>○ご注意</strong>
	<br />
		後払い手数料：<%: this.PaymentTypeFee %>
	<br />
		後払いのご注文には、株式会社ネットプロテクションズの後払いサービスが適用され、同社へ代金債権を譲渡します。
	<br />
		<a href="https://np-atobarai.jp/terms/atobarai-buyer.html">NP後払い利用規約及び同社のプライバシーポリシー</a>に同意して、後払いサービスをご選択ください。
	<br />
		ご利用限度額は累計残高で<%: this.PaymentTypeMaxAmount %>（税込）迄です。
	<br />
		詳細はバナーをクリックしてご確認下さい。
	<br />
		ご利用者が未成年の場合、法定代理人の利用同意を得てご利用ください。
</p>
