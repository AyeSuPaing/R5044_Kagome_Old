<%--
=========================================================================================================
  Module      : Scoring Sale Question (ScoringSaleQuestion.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/ScoringSale/ScoringSaleQuestion.ascx.cs" Inherits="Form_Common_ScoringSale_ScoringSaleQuestion" %>
<%@ Register TagPrefix="uc" TagName="QuestionTextSingle" Src="~/SmartPhone/Form/Common/ScoringSale/QuestionTextSingle.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionTextMultiple" Src="~/SmartPhone/Form/Common/ScoringSale/QuestionTextMultiple.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionImageTextSingle" Src="~/SmartPhone/Form/Common/ScoringSale/QuestionImageTextSingle.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionImageTextMultiple" Src="~/SmartPhone/Form/Common/ScoringSale/QuestionImageTextMultiple.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionPullDown" Src="~/SmartPhone/Form/Common/ScoringSale/QuestionPullDown.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionChart" Src="~/SmartPhone/Form/Common/ScoringSale/QuestionChart.ascx" %>

<% if (this.QuestionPageItem.Question.AnswerType == Constants.FLG_SCORINGSALE_QUESTION_TYPE_TEXT_SINGLE) { %>
	<uc:QuestionTextSingle ID="ucQuestionTextSingle" runat="server" />
<% } else if (this.QuestionPageItem.Question.AnswerType == Constants.FLG_SCORINGSALE_QUESTION_TYPE_TEXT_MULTIPLE) { %>
	<uc:QuestionTextMultiple ID="ucQuestionTextMultiple" runat="server" />
<% } else if (this.QuestionPageItem.Question.AnswerType == Constants.FLG_SCORINGSALE_QUESTION_TYPE_IMAGE_TEXT_SINGLE) { %>
	<uc:QuestionImageTextSingle ID="ucQuestionImageTextSingle" runat="server" />
<% } else if (this.QuestionPageItem.Question.AnswerType == Constants.FLG_SCORINGSALE_QUESTION_TYPE_IMAGE_TEXT_MULTIPLE) { %>
	<uc:QuestionImageTextMultiple ID="ucQuestionImageTextMultiple" runat="server" />
<% } else if (this.QuestionPageItem.Question.AnswerType == Constants.FLG_SCORINGSALE_QUESTION_TYPE_PULLDOWN) { %>
	<uc:QuestionPullDown ID="ucQuestionPullDown" runat="server" />
<% } else if (this.QuestionPageItem.Question.AnswerType == Constants.FLG_SCORINGSALE_QUESTION_TYPE_CHART) { %>
	<uc:QuestionChart ID="ucQuestionChart" runat="server" />
<% } %>