<%--
=========================================================================================================
  Module      : Scoring Sale Question List (ScoringSaleQuestionList.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ScoringSaleQuestionList.ascx.cs" Inherits="Form_Common_ScoringSale_ScoringSaleQuestionList" %>
<%@ Register TagPrefix="uc" TagName="ScoringSaleQuestion" Src="~/Form/Common/ScoringSale/ScoringSaleQuestion.ascx" %>
<div class="page-border-top"></div>
<asp:Repeater ID="rQuestion" runat="server" ItemType="ScoringSaleQuestionPageItemInput">
	<ItemTemplate>
		<div class="scoringsale_questionWrap">
			<div class="scoringsale_item">
				<uc:ScoringSaleQuestion ID="ucScoringSaleQuestion" runat="server" BranchNo="<%# Item.BranchNo %>" />
			</div>
		</div>
	</ItemTemplate>
</asp:Repeater>