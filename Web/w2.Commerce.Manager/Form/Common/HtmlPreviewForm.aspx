<%--
=========================================================================================================
  Module      : HTMLプレビュー用ページ(HtmlPreviewForm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HtmlPreviewForm.aspx.cs" Inherits="Form_Common_HtmlPreviewForm" %>
<link rel="stylesheet" href="<%: this.ResolveClientUrl("~/Css/w2style.css?") + Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>" media="screen,print" type="text/css" />
<link rel="stylesheet" href="<%: this.ResolveClientUrl("~/Css/w2style_ec_v2.css?") + Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>" media="screen,print" type="text/css" />
<link rel="stylesheet" href="../../Images/Icon/icomoon/style.css" media="screen,print" type="text/css" />
<body class="detail_item_bg">
	<%= this.HtmlForPreview %>
</body>
