<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Global/GlobalChangeIcon.ascx.cs" Inherits="Form_Common_Global_GlobalChangeIcon" %>
<% if (Constants.GLOBAL_OPTION_ENABLE)
   { %>
<li>
	<a href="javascript:void(0);" id="toggle-global" style="background-image: url(<%: this.UserNationanFlagPath %>); background-repeat: no-repeat; background-size: contain; background-position: center;">&nbsp;</a>
</li>
<% } %>