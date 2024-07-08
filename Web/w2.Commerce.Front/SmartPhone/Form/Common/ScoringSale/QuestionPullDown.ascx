<%--
=========================================================================================================
  Module      : Question Pull Down (QuestionPullDown.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/ScoringSale/QuestionPullDown.ascx.cs" Inherits="Form_Common_ScoringSale_QuestionPullDown" %>
<div>
	<h2 class="scoringsale_ttl">Q<%#: (this.Question != null) ? this.Question.Question.QuestionNo : 0 %></h2>
	<p class="scoringsale_subTtl"><%#: (this.Question != null) ? this.Question.Question.QuestionStatement : string.Empty %></p>
	<div class="scoringsale_question">
		<div class="scoringsale_question_item _select">
			<div class="divQuestionChoice">
				<asp:DropDownList
					ID="ddlQuestionPullDown"
					CssClass="ddlQuestionPullDown"
					Width="100%"
					runat="server"
					DataSource="<%# this.GetQuestionItems() %>"
					DataTextField="text"
					DataValueField="value"
					SelectedValue="<%# this.BrandNo %>" />
			</div>
		</div>
	</div>
</div>
<script type="text/javascript">
	$(".ddlQuestionPullDown").select2();
	$('.ddlQuestionPullDown').select2({
		minimumResultsForSearch: Infinity
	});
</script>