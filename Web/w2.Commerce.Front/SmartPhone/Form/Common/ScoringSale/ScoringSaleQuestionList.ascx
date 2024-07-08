<%--
=========================================================================================================
  Module      : Scoring Sale Question List (ScoringSaleQuestionList.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/ScoringSale/ScoringSaleQuestionList.ascx.cs" Inherits="Form_Common_ScoringSale_ScoringSaleQuestionList" %>
<%@ Register TagPrefix="uc" TagName="ScoringSaleQuestion" Src="~/SmartPhone/Form/Common/ScoringSale/ScoringSaleQuestion.ascx" %>
<asp:Repeater ID="rQuestion" runat="server" ItemType="ScoringSaleQuestionPageItemInput">
	<ItemTemplate>
		<div class="scoringsale_questionWrap">
			<div class="scoringsale_item">
				<uc:ScoringSaleQuestion ID="ucScoringSaleQuestion" runat="server" BranchNo="<%# Item.BranchNo %>" />
			</div>
		</div>
	</ItemTemplate>
</asp:Repeater>