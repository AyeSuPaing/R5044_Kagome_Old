<%--
=========================================================================================================
  Module      : エラーポイント蓄積アイコン(ErrorPointIcon.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ErrorPointIcon.ascx.cs" Inherits="Form_Top_ErrorPointIcon" %>

<style type="text/css">
span.tooltip span {
	z-index:10;display:none !important; padding:5px 10px;
	margin-top:-25px; margin-left:5px;
	width:160px; line-height:16px;
}
span.tooltip:hover span{
	display:inline !important; position:absolute; color:#111;
	border:1px solid #DCA; background:#fffAF0;
}
span.tooltip
{
	display: inline !important;
}
</style>



<% if (this.ErrorPoint >= Constants.DISPLAY_ERROR_POINT) { %>

<span class="tooltip"><img src="../../Images/Cs/error_small.png" alt="" /><span>
	エラーポイントは現在 <%= StringUtility.ToNumeric(this.ErrorPoint) %>pt です。</span></span>

<% } %>
