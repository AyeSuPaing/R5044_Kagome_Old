<%--
=========================================================================================================
Module      : Paidy Checkout Script (PaidyCheckoutScript.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>

<%@  Language="C#" AutoEventWireup="true" %>
<% if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct) && Constants.PAYMENT_PAIDY_OPTION_ENABLED) { %>
<%-- ▼▼Paidy支払▼▼ --%>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PAYMENT_PAIDY_API_URL %>"></script>
<script type="text/javascript">
	// Setting config
	var config = {
		"api_key": "<%= Constants.PAYMENT_PAIDY_API_KEY %>",
		"logo_url": "https://download.paidy.com/2.0/image/logo/paidy-logo.png",
		"closed": function (callbackData) {
			$("#" + hfPaidyTokenIdControlId).val(callbackData.id);
			if (callbackData.id != null) {
				if (isHistoryPage) {
					__doPostBack(updatePaymentUniqueID, '');
				} else {
					$("#" + lbNextProcess)[0].click();
				}
			}
		},
		"token": {
			"wallet_id": "default",
			"type": "recurring",
		}
	};

	// Paidy Pay Process
	function PaidyPayProcess() {
		// Check has selected paidy pay
		var paidyPaySelected = $("#" + hfPaidyPaySelectedControlId).val();
		if (paidyPaySelected == "True") {
			// Check token has value
			var tokenId = $("#" + hfPaidyTokenIdControlId).val();
			if ((tokenId != undefined)
				&& (tokenId != "")) {
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
	function paidyPay() {
		var payload = {
			"store_name": "Paidy sample store",
			"buyer": buyer
		};
		paidyHandler.launch(payload);
	};
</script>
<%-- ▲▲Paidy支払▲▲ --%>
<% } %>