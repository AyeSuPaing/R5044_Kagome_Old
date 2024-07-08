<%--
=========================================================================================================
  Module      : 後付款(TriLink後払い)注意書きユーザーコントロール(PaymentDescriptionTriLinkAfterPay.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="BaseUserControl" %>
<%@ Import Namespace="w2.App.Common.Global.Region" %>

<%-- 後付款(TriLink後払い)注意書き表示 --%>
<% if (RegionManager.GetInstance().Region.LanguageLocaleId != Constants.LANGUAGE_LOCALE_ID_ZHTW) { %>
<pre>
<strong>◆『台湾後払い決済いサービス』の詳細</strong>
台湾唯一の後払い決済サービスです。
ご注文の確定後、台湾全土の四台コンビニ約10,000
店舗及び銀行ATMにてお支払いいただける「請求番
号」が記載されたメールを送信いたします。
メール内に記載の支払い期限までにお支払いいただ
くようお願いいたします。<br />
<strong>◆ご注意</strong>
注文者の住所と配送先が台湾の時のみに利用できる
決済となります。ご利用限度額はご購入者様一人当
たりNTD10,000元です。他のEC店舗でのご利用も合
算した金額となります。<span style="text-decoration: underline">お客様情報が正しく入力さ
れなかった場合、与信審査が通らない場合がござい
ます。</span>詳細は下記「後付款」画像をク
リックしてご確認ください。
<a href="https://www.afterpay.com.tw/customer" target="_blank">
<img src="../../Contents/ImagesPkg/afterpay/banner20200225.jpg" alt="afterpay" width="300" height="75" />
</a>
</pre>
<% } else { %>
<pre>
<strong>■先取貨，後付款（四大超商・銀行ATM）</strong>
<span style="color: red;">「收到請款通知9天內」</span>再前往<span style="color: red;">便利超商</span>或者是<span style="color: red;">ATM付
款</span>的付款方式。繳款代碼會以E-mail等方式寄送，
再至附近的便利超商或ATM繳款即可。<span style="color: red; font-weight: bold">您將可能收到
來自【後付款服務中心】的email，或致電與您確認
訂單資訊,屆時敬請協助配合。</span>

「先取貨後付款」，為「先取貨後付款 」支付服務
提供公司（三環亞洲股份有限公司）所提供的服務。
顧客訂購商品時適用「先取貨後付款」服務，於服
務範圍内提供個人情報並且讓渡商品代金債權。
請點選「後付款」的標誌，確認詳細內容。
<a href="https://www.afterpay.com.tw/customer" target="_blank">
<img src="../../Contents/ImagesPkg/afterpay/banner20200225.jpg" alt="afterpay" width="300" height="75" />
</a>
親愛的顧客請確認所有規約後，開始訂購商品。

<strong>【注意事項】</strong>
・請於收到請款通知後9天以内繳款。
・使用「先取貨後付款」服務的金額上限為，(含複
数店舗)累計10,000元(NTD)。超過10,000元(NTD)時，
請使用其他結帳方式。
・購物時如使用「後付款」服務，需要進行能否利用
此服務之審査。審査結果如為「不可」時，請選擇使
用其他的結帳方式。
・至繳費期限為止，如未完成繳納 ，除了以年率5%
（日息1.370/10000）作為延遲利息外，另需加上延
遲利息的年率20%（日息5.480/10000）作為延遲損害
金，一併請求支付。
</pre>
<% } %>