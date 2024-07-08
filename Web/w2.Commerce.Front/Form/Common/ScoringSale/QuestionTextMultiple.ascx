<%--
=========================================================================================================
  Module      : Question Text Multiple (QuestionTextMultiple.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuestionTextMultiple.ascx.cs" Inherits="Form_Common_ScoringSale_QuestionTextMultiple" %>
<asp:Repeater ID="rScoringSaleQuestionChoice" runat="server" ItemType="ScoringSaleQuestionChoiceInput">
	<HeaderTemplate>
		<h2 class="scoringsale_ttl"><%# ReplaceTag("@@ScoringSaleQuestion.question_symbol_text.name@@") %><%#: this.Question.Question.QuestionNo %><br /></h2>
		<p class="scoringsale_subTtl"><%#: this.Question.Question.QuestionStatement %></p>
		<div class="scoringsale_question">
			<div class="scoringsale_question_item _check">
	</HeaderTemplate>
	<ItemTemplate>
		<asp:CheckBox ID="cbChoiceTextMultiple" runat="server" Checked="<%# Item.IsChosen %>" />
		<label for="<%#: Container.FindControl("cbChoiceTextMultiple").ClientID.ToString() %>">
			<span><%#: Item.QuestionChoiceStatement %></span>
			<asp:HiddenField ID="hfBranchNo" Value="<%# Item.BranchNo %>" runat="server" />
		</label>
	</ItemTemplate>
	<FooterTemplate>
			</div>
		</div>
	</FooterTemplate>
</asp:Repeater>