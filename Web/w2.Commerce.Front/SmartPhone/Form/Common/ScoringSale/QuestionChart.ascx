<%--
=========================================================================================================
  Module      : Question Chart (QuestionChart.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/ScoringSale/QuestionChart.ascx.cs" Inherits="Form_Common_ScoringSale_QuestionChart" %>
<asp:Repeater ID="rScoringSaleQuestionChoice" runat="server" ItemType="ScoringSaleQuestionChoiceInput">
	<HeaderTemplate>
		<h2 class="scoringsale_ttl">Q<%#: this.Question.Question.QuestionNo %><br /></h2>
		<p class="scoringsale_subTtl"><%#: this.Question.Question.QuestionStatement %></p>
		<div class="scoringsale_question">
			<div class="scoringsale_question_item _radio _likert">
	</HeaderTemplate>
	<ItemTemplate>
		<w2c:RadioButtonGroup ID="rbgChoiceChart" GroupName="<%# this.Question.BranchNo %>" Checked="<%# Item.IsChosen %>" runat="server" />
		<label for="<%#: Container.FindControl("rbgChoiceChart").ClientID.ToString() %>">
			<asp:HiddenField ID="hfBranchNo" runat="server" Value="<%# Item.BranchNo %>" />
			<%# Item.QuestionChoiceStatement %>
		</label>
	</ItemTemplate>
	<FooterTemplate>
			</div>
		</div>
	</FooterTemplate>
</asp:Repeater>