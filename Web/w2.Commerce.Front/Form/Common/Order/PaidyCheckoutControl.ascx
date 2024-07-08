<%--
=========================================================================================================
  Module      : Paidy Checkout Control (PaidyCheckoutControl.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="PaidyCheckoutBaseControl" %>
<% if (Constants.PAYMENT_PAIDY_OPTION_ENABLED) { %>
	<div style="<%= this.DisplayUserControl ? string.Empty : "display: none" %>">
		<a style="text-decoration:none;" href="https://paidy.com/payments/" target="_blank">
			<img src="https://download.paidy.com/2.0/image/banner/checkout_banner_320x100.png" width="90%" alt="Paidy翌月払い(コンビニ/銀行" style="cursor:pointer; padding:0px 0px 10px 10px;" />
		</a>
		<p>
		・メールアドレスと携帯電話番号だけでご利用いただける決済方法です。事前登録・クレジットカードは必要ありません。
		<br />
		・月に何回お買い物をしても、お支払いは翌月にまとめて１回。1ヶ月分のご利用金額は、翌月1日に確定し、メールとSMSでお知らせします。
		<br />
		・下記のお支払い方法がご利用いただけます。
		<br />
		　口座振替(支払手数料：無料)
		<br />
		　コンビニ(支払手数料：356円税込)
		<br />
		　銀行振込(支払手数料：金融機関により異なります)
		</p>
	</div>
<% } %>