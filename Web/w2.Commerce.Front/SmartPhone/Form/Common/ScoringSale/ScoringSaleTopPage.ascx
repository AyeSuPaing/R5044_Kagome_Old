<%--
=========================================================================================================
  Module      : Scoring Sale Top Page (ScoringSaleTopPage.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/ScoringSale/ScoringSaleTopPage.ascx.cs" Inherits="Form_Common_ScoringSale_ScoringSaleTopPage" %>
<%@ Register TagPrefix="uc" TagName="AccessLogTrackerScript" Src="~/Form/Common/AccessLogTrackerScript.ascx" %>
<div class="scoringsale_inner">
	<div class="scoringsale_item">
		<h2 class="scoringsale_ttl"><%#: this.TopPageTitle %><br /></h2>
		<p class="scoringsale_subTtl"><%#: this.TopPageSubTitle %><br /></p>
		<div class="scoringsale_img">
			<img src="<%: this.TopPageImgPath %>" />
		</div>
		<p class="scoringsale_info"><%#: this.TopPageBody %><br /></p>
		<div class="scoringsale_btnArea">
			<asp:LinkButton ID="lbStart" OnClick="lbStart_Click" runat="server" CssClass="scoringsale_btn _go _start" style="padding: 0 40px 0 15px;" />
		</div>
		<p id="pErrorSessionExpired" visible="false" runat="server" class="scoringsale_product error_inline" style="background: none; font-size: 14px;"></p>
	</div>
</div>
<%-- w2 access log tracker output --%>
<uc:AccessLogTrackerScript id="AccessLogTrackerScript1" runat="server" />
<script type="text/javascript">
	$(function () {
		// Set property of css
		var color = '<%= this.ScoringSale.ThemeColor %>';
		document.body.style.setProperty('--color-common', color);
	});
</script>