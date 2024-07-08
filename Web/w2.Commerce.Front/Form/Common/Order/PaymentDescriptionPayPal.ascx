<%--
=========================================================================================================
  Module      : PayPal注意書きユーザーコントロール(PaymentDescriptionPayPal.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="BaseUserControl" %>

<%-- ペイパル向け注意書き表示 --%>
<% if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) { %>
<!-- PayPal Logo -->
<table border="0" cellpadding="10" cellspacing="0" align="center">
	<tr>
	<td align="center">
		<a href="#" onclick="javascript:window.open('https://www.paypal.com/jp/webapps/mpp/logo/about','olcwhatispaypal','toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, width=900, height=700');">
			<img src="https://www.paypalobjects.com/digitalassets/c/website/marketing/apac/jp/developer/203x80_c.png" border="0" alt="ペイパル｜新規登録無料、カードのポイント貯まる｜VISA,Mastercard,JCB,American Express,UnionPay,銀行" width="280">
		</a>
	</td>
	</tr>
</table><!-- PayPal Logo -->
<div>
	カードでも銀行口座からでも、一度設定すれば IDとパスワードでかんたん・安全にお支払い。新規登録は無料。銀行口座からのお支払いでも、振込手数料は無料です。<br/>
	※ご利用可能な銀行は、みずほ銀行、三井住友銀行、三菱UFJ銀行、ゆうちょ銀行、りそな銀行・埼玉りそな銀行です。
</div>
<% } %>
