<%--
=========================================================================================================
  Module      : Ec Pay Script(EcPayScript.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EcPayScript.ascx.cs" Inherits="Form_Common_Order_EcPay_EcPayScript" %>
<script type="text/javascript">
	function NextPageSelectReceivingStore(params) {
		var url = '<%= Constants.RECEIVINGSTORE_TWECPAY_APIURL + "Express/map" %>';
		var form = document.createElement("form");
		form.setAttribute("method", "post");
		form.setAttribute("action", url);
		form.setAttribute("target", "");

		for (var item in params) {
			if (params.hasOwnProperty(item)) {
				var input = document.createElement('input');
				input.type = 'hidden';
				input.name = item;
				input.value = params[item];
				form.appendChild(input);
			}
		}
		document.body.appendChild(form);
		form.submit();
	}
</script>