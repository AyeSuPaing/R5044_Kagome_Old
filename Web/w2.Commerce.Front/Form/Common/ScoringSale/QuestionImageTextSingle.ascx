﻿<%--
=========================================================================================================
  Module      : Question Image Text Single (QuestionImageTextSingle.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuestionImageTextSingle.ascx.cs" Inherits="Form_Common_ScoringSale_QuestionImageTextSingle" %>
<asp:Repeater ID="rScoringSaleQuestionChoice" runat="server" ItemType="ScoringSaleQuestionChoiceInput">
	<HeaderTemplate>
		<h2 class="scoringsale_ttl"><%# ReplaceTag("@@ScoringSaleQuestion.question_symbol_text.name@@") %><%#: this.Question.Question.QuestionNo %> <br /></h2>
		<p class="scoringsale_subTtl"><%#: this.Question.Question.QuestionStatement %></p>
		<div class="scoringsale_question">
			<div class="scoringsale_question_item _select_img _img_radio _radio" >
	</HeaderTemplate>
	<ItemTemplate>
		<div class="scoringsale-question-image-choice">
			<w2c:RadioButtonGroup ID="rbgChoiceImageTextSingle" GroupName="<%# this.Question.BranchNo %>" Checked="<%# Item.IsChosen %>" runat="server" />
			<label for='<%#: Container.FindControl("rbgChoiceImageTextSingle").ClientID.ToString() %>'>
				<div>
					<asp:Image ID="Image1" src="<%#: Constants.PATH_ROOT_FRONT_PC + Item.QuestionChoiceStatementImgPath %>" runat="server" />
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