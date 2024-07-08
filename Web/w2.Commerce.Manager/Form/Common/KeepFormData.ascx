<%--
=========================================================================================================
  Module      : Keep Form Data When Back(KeepFormData.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="KeepFormData.ascx.cs" Inherits="Form_Common_KeepFormData" %>
<script type="text/javascript">
	<%if (this.AutoRestoreFormData){ %>
	keepFormUtil = {
		Restore: function() {
			if (typeof(<%=CLIENT_VARIABLE_NAME%>) == "undefined") return;

			for (var i = 0; i < <%=CLIENT_VARIABLE_NAME%>.length; i++) {
				var target = $("#" + <%=CLIENT_VARIABLE_NAME%>[i].Key);
				
				if (target.size() == 0) continue;
				
				switch (target.attr("type")) {
					case "checkbox":
						if (<%=CLIENT_VARIABLE_NAME%>[i].Value == "on") {
							target.click();
							target.prop("checked", "true");
						}
						break;

					default:
						target.val(<%=CLIENT_VARIABLE_NAME%>[i].Value);
						break;
				}

				// process for radio
				if (typeof(target.attr("type")) == "undefined") {
					$('input:radio[name="' + <%=CLIENT_VARIABLE_NAME%>[i].Key.replace(/_/g, "$") + '"][value="' + <%=CLIENT_VARIABLE_NAME%>[i].Value + '"]').attr('checked', true).click();
					
				}
			}
		}
	};

	$(document).ready(function() {
		// Call Javascript Async
			setTimeout(function() {
				keepFormUtil.Restore();
		}, 0);
	});
	<%} %>
</script>
