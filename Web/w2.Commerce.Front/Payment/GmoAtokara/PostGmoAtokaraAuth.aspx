<!--
=========================================================================================================
Module      : GMOアトカラ データ送信ページ処理(PostGmoAtokaraAuth.aspx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
-->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PostGmoAtokaraAuth.aspx.cs" Inherits="Payment_GmoAtokara_PostGmoAtokaraAuth" %>

<html>
<head>
	<meta http-equiv="Content-Type" content="text/html; charset=Windows-31J">
</head>

<body>

<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/jquery-3.6.0.min.js"></script>
<script type="text/javascript" src="<%= Constants.PAYMENT_GMOATOKARA_DEFERRED_URL_ORDERREGISTER %>"></script>
　<script type="text/javascript">
	window.addEventListener('load', function () {
		execOrder();
	});
	// SMS 認証結果
	function gmoSmsAuthResult(data) {
		// レスポンス結果判定処理
		var isFailure = data.result === 'NG';
		$.ajax({
			type: "POST",
			url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_GMO_ATOKARA_POST %>/WriteLog",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				isSuccess: isFailure == false,
				data: data
			})
		}).done(function () {
			if (isFailure) {
				var href = createResultUrl('');
				location.href = href;
			} else {
				var gmoTransactionId = data.transactionResult.gmoTransactionId;
				var href = createResultUrl(gmoTransactionId);
				location.href = href;
			}
		});
	}

	// 注文確定ボタン押下時
	function execOrder() {
		if (typeof gmoAuthOrderDecision == "function") {
			gmoAuthOrderDecision();
		} else {
			// 読み込みエラー発生時の処理
			var href = createResultUrl('');
			location.href = href;
		}
	}

	function createResultUrl(gmoTransactionId) {
		var href = "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_GMO_ATOKARA_RESULT + '?' + Constants.REQUEST_KEY_ORDER_ID + '=' + this.Request[Constants.REQUEST_KEY_ORDER_ID] %>" + '&gmoid=' + gmoTransactionId;
		return href;
	}
 </script>
 
<asp:Repeater id="rPostData" Runat="server">
<ItemTemplate>
	<input type="hidden" class="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" value="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
</ItemTemplate>
</asp:Repeater>

ただいま決済処理中です。<br />画面が切り替わるまでそのままお待ちください . . .

 <!-- デバイス情報 -->
 <input type="hidden" id="fraudbuster" name="fraudbuster" class="gmoDeviceInfo" />
 <script type="text/javascript" src="//fraud-buster.appspot.com/js/fraudbuster.js"></script>
</body>
</html>
