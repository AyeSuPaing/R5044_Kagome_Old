<%--
=========================================================================================================
  Module      : Question Image Text Multiple (QuestionImageTextMultiple.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuestionImageTextMultiple.ascx.cs" Inherits="Form_Common_ScoringSale_QuestionImageTextMultiple" %>
<asp:Repeater ID="rScoringSaleQuestionChoice" runat="server" ItemType="ScoringSaleQuestionChoiceInput">
	<HeaderTemplate>
		<h2 class="scoringsale_ttl"><%# ReplaceTag("@@ScoringSaleQuestion.question_symbol_text.name@@") %><%#: this.Question.Question.QuestionNo %><br /></h2>
		<p class="scoringsale_subTtl"><%#: this.Question.Question.QuestionStatement %></p>
		<div class="scoringsale_question">
			<div class="scoringsale_question_item _select_img _img_check _check">
	</HeaderTemplate>
	<ItemTemplate>
		<div class="scoringsale-question-image-choice">
			<asp:CheckBox runat="server" ID="cbChoiceImageTextMultiple" Checked="<%# Item.IsChosen %>" />
			<label for="<%#: Container.FindControl("cbChoiceImageTextMultiple").ClientID.ToString() %>">
				<div>
					<asp:Image src="<%#: Constants.PATH_ROOT_FRONT_PC + Item.QuestionChoiceStatementImgPath %>" runat="server" />
				</div>
				<span><%#: Item.QuestionChoiceStatement %></span>
				<asp:HiddenField ID="hfBranchNo" runat="server" Value="<%# Item.BranchNo %>" />
			</label>
		</div>
	</ItemTemplate>
	<FooterTemplate>
			</div>
		</div>
	</FooterTemplate>
</asp:Repeater>