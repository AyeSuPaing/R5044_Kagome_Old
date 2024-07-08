<%--
=========================================================================================================
  Module      : Question Text Single (QuestionTextSingle.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/ScoringSale/QuestionTextSingle.ascx.cs" Inherits="Form_Common_ScoringSale_QuestionTextSingle" %>
<asp:Repeater ID="rScoringSaleQuestionChoice" runat="server" ItemType="ScoringSaleQuestionChoiceInput">
	<HeaderTemplate>
		<h2 class="scoringsale_ttl">Q<%#: this.Question.Question.QuestionNo %><br /></h2>
		<p class="scoringsale_subTtl"><%#: this.Question.Question.QuestionStatement %></p>
		<div class="scoringsale_question">
			<div class="scoringsale_question_item _radio radioBtn">
	</HeaderTemplate>
	<ItemTemplate>
		<w2c:RadioButtonGroup ID="rbgChoiceTextSingle" GroupName="<%# this.Question.BranchNo %>" runat="server" Checked="<%# Item.IsChosen %>" />
		<label for="<%#: Container.FindControl("rbgChoiceTextSingle").ClientID.ToString() %>">
			<%# Item.QuestionChoiceStatement %>
			<asp:HiddenField ID="hfBranchNo" runat="server" Value="<%# Item.BranchNo %>" />
		</label>
	</ItemTemplate>
	<FooterTemplate>
			</div>
		</div>
	</FooterTemplate>
</asp:Repeater>