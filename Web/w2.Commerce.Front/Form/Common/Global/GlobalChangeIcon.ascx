<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GlobalChangeIcon.ascx.cs" Inherits="Form_Common_Global_GlobalChangeIcon" %>
<% if (Constants.GLOBAL_OPTION_ENABLE)
   { %>
<a href="javascript:void(0);">
	<img src="<%: this.UserNationanFlagPath %>">
</a>
<% } %>