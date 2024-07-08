<%--
=========================================================================================================
  Module      : Scoring Sale Question Page (ScoringSaleQuestionPage.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/ScoringSale/ScoringSaleQuestionPage.ascx.cs" Inherits="Form_Common_ScoringSale_ScoringSaleQuestionPage" %>
<%@ Register TagPrefix="uc" TagName="ScoringSaleQuestionList" Src="~/SmartPhone/Form/Common/ScoringSale/ScoringSaleQuestionList.ascx" %>
<%@ Register TagPrefix="uc" TagName="AccessLogTrackerScript" Src="~/Form/Common/AccessLogTrackerScript.ascx" %>
<asp:UpdatePanel runat="server">
	<ContentTemplate>
		<uc:ScoringSaleQuestionList ID="ucScoringSaleQuestionList" runat="server" />
		<div class="scoringsale_question">
			<div class="scoringsale_btnArea">
				<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" AutoPostback="false" Class="scoringsale_btn _back" runat="server" />
					<p class="scoringsale_pageNum"><asp:Literal ID="lPageNo" runat="server" /></p>
				<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" AutoPostback="false" runat="server" />
			</div>
		</div>
		<p id="pErrorSessionExpired" visible="false" runat="server" class="scoringsale_product error_inline" style="background: none; font-size: 14px;"></p>
	</ContentTemplate>
</asp:UpdatePanel>
<%-- w2 access log tracker output --%>
<uc:AccessLogTrackerScript id="AccessLogTrackerScript1" runat="server" />
<script type="text/javascript">
	$(function () {
		// Set property of css
		var color = '<%= this.ScoringSale.ThemeColor %>';
		document.body.style.setProperty('--color-common', color);
	});
</script>