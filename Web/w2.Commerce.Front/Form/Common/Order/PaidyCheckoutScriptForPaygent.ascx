<%--
=========================================================================================================
Module      :Paidy(Paygent)用Checkoutスクリプト (PaidyCheckoutScriptForPaygent.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright w2solution Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
--%>
<%@  Language="C#" AutoEventWireup="true" %>
<% if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent) && Constants.PAYMENT_PAIDY_OPTION_ENABLED) { %>
<%-- ▼▼Paidy支払▼▼ --%>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PAYMENT_PAIDY_API_URL %>"></script>
<script type="text/javascript">
	// Setting config
	var config = {
		"api_key": "<%= Constants.PAYMENT_PAYGENT_PAIDY_API_KEY %>",
		"logo_url": "https://download.paidy.com/2.0/image/logo/paidy-logo.png",
		"closed": function (callbackData) {
			$("#" + hfPaidyStatusControlId).val(callbackData.status);
			if (body === undefined) {
				body = customBody;
			}
			// ログ出力用のPOSTを送信
			$.ajax({
				type: "POST",
				url: "<%=Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_CONFIRM%>/WritePaidyLog",
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				data: JSON.stringify({ request: JSON.stringify(body, null, "\t"), response: JSON.stringify(callbackData, null, "\t") }),
				async: false
			});
			if (callbackData.id != null && callbackData.id != undefined) {
				$("#" + hfPaidyPaymentIdControlId).val(callbackData.id);
				if (isHistoryPage) {
					__doPostBack(updatePaymentUniqueID, '');
				} else {
					$("#" + lbNextProcess)[0].click();
				}
			}
		}
	};

	// Paidy Pay Process
	function PaidyPayProcess(customBody = undefined) {
		if (customBody != undefined) {
			paidyPay(customBody);
			return;
		}
		// Check has selected paidy pay
		var paidyPaySelected = $("#" + hfPaidyPaySelectedControlId).val();
		if (paidyPaySelected == "True") {
			// Check payment has value
			var paymentId = $("#" + hfPaidyPaymentIdControlId).val();
			if ((paymentId != undefined)
				&& (paymentId != "")) {
				if (isHistoryPage) {
					__doPostBack(updatePaymentUniqueID, '');
				} else {
					$("#" + lbNextProcess)[0].click();
				}
			} else {
				paidyPay();
			}
		} else {
			if (isHistoryPage) {
				__doPostBack(updatePaymentUniqueID, '');
			} else {
				$("#" + lbNextProcess)[0].click();
			}
		}
	}

	// Load config
	var paidyHandler = Paidy.configure(config);

	// Paidy Pay
	function paidyPay(customBody = undefined) {
		if (customBody != undefined) {
			var payload = customBody;
			paidyHandler.launch(payload);
		}
		else {
			var payload = body;
			paidyHandler.launch(payload);
		}
	};
</script>
<%-- ▲▲Paidy支払▲▲ --%>
<% } %>
