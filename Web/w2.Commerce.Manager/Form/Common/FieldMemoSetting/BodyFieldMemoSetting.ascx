<%--
=========================================================================================================
  Module      : 項目メモ表示コントローラ(BodyFieldMemoSetting.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BodyFieldMemoSetting.ascx.cs" Inherits="Form_Common_FieldMemoSetting_BodyFieldMemoSetting" %>

<a id="linkMemo" class="tip_memo" runat="server">
	<img src="<%= Constants.PATH_ROOT %>Images/Common/info.png" alt="" height="16" width="16" border="0" style="cursor:pointer;margin-bottom:-3px;margin-left:-3px;"/>
</a>

<asp:UpdatePanel runat="server" UpdateMode="Conditional">
   <ContentTemplate>
	<div id="divTitle" visible="false" runat="server">
		<span class="memo_title_display">[<asp:Label id="lblTitle" runat="server"/>]</span>
	</div>
   </ContentTemplate>
   <Triggers>
     <asp:AsyncPostBackTrigger ControlID="btnTooltipInfo" EventName="Click" />
   </Triggers>
 </asp:UpdatePanel>