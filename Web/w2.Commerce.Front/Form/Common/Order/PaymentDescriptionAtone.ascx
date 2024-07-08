<%--
=========================================================================================================
  Module      : Payment Description Atone(PaymentDescriptionAtone.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="BaseUserControl" %>

<%-- Atone Payment Description --%>
<p>
	<a href="https://atone.be" target="_blank">
		<img src="https://atone.be/start/banner/bn_320x100.png" alt="atone（アトネ）スマホ決済"
			width="<%# this.IsSmartPhone ? "320" : "300" %>"
			style="margin-bottom:5px;margin-top:<%# this.IsSmartPhone ? "5px" : "0px"%>"/>
	</a>
	<br />
		■atone（アトネ）とは？
	<br />
		今すぐ使える、翌月払いです。お支払いに必要な請求書は、翌月初旬に届きます。
	<br />
		コンビニで20日までにお支払いください。
	<br />
		200円で1ポイント、atone のお買い物に使えるポイントが貯まります。
	<br />
		サービス詳細は
	<a href="https://atone.be/" target="_blank">
		atone（アトネ）公式ページ
	</a>
		をご覧ください。
	<br /> <br />
		■利用方法
	<br />
		書類手続きは不要です。お支払い方法選択時に atone を選び、情報を入力してお買い物を進めてください。
	<br /> <br />
		■支払い方法
	<br />
		はがきの請求書を使って、コンビニの店頭でお支払いください。
	<br />
		atone のスマホアプリで、次の支払い方法に変更することができます。
	<br /> <br />
		・Loppi / マルチコピー機（コンビニ）
	<br />
		・自動引き落とし（口座振替）
	<br /> <br />
		※注意事項
	<br />
		・決済手数料は無料です
	<br />
		・ご利用月のみ請求費用99円 (税込) が発生いたします
	<br />
		・代金は株式会社ネットプロテクションズの
	<a href="https://atone.be/terms/" target="_blank">
		会員規約
	</a>
		に基づき指定の方法で請求いたします。
</p>
