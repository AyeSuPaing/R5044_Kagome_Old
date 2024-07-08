<%--
=========================================================================================================
  Module      : メールドメイン(MailDomains.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MailDomains.ascx.cs" Inherits="Form.Common.FormCommonMailDomains" %>
<%-- メールアドレス入力時にドメイン補完用 --%>
<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>Css/autoComplete.css?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"/>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/mailcomplete.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
<script type="text/javascript">
	var mailDomains = getMailDomains("<%= Constants.PATH_ROOT %>Contents/MailDomains.xml");
	
	$(function () {
		$('.mail-domain-suggest').each(function (index, element) {
			var $mailSuggestContainer = $("<ul></ul>",
				{
					id: "mailList" + index.toString(),
					class: "ul-mail-domains-list"
				});

			// ulをTextBoxの直下に配置
			var $mailTextBox = $("#" + element.id);
			$mailTextBox.after($mailSuggestContainer);

			$(this).mailcomplete(
				{
					textBoxId : element.id,
					mailDomainList : $mailSuggestContainer.attr("id")
				});
		});
	});
</script>