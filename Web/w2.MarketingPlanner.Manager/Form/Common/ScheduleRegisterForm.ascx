<%--
=========================================================================================================
  Module      : スケジュール登録ユーザコントロール(ScheduleRegisterForm.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ScheduleRegisterForm.ascx.cs" Inherits="Form_Common_ScheduleRegisterForm" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%
	rbExecBySchedule.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
	rbExecByManual.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
	
	rbScheRepeatDay.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
	rbScheRepeatWeek.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
	rbScheRepeatMonth.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
	rbScheRepeatOnce.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
%>
<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
	<tr>
		<td class="edit_title_bg" height="20">
			抽出タイミング<span class="notice">*</span></td>
	</tr>
	<tr>
		<td class="search_item_bg">
			<asp:RadioButton id="rbExecBySchedule" GroupName="ExecType" runat="server" />
			<asp:RadioButton id="rbExecByManual" GroupName="ExecType" runat="server" /><br />
			<div id="dvScheduleDetail">
				<div style="padding-left:20px;padding-bottom:10px">
					<asp:RadioButton ID="rbScheRepeatDay" GroupName="ScheType" runat="server" />
					<asp:RadioButton ID="rbScheRepeatWeek" GroupName="ScheType" runat="server" />
					<asp:RadioButton ID="rbScheRepeatMonth" GroupName="ScheType" runat="server" />
					<asp:RadioButton ID="rbScheRepeatOnce" GroupName="ScheType" runat="server" />
				</div>
				<div style="padding-left:20px">
					<span id="spScheTopString" style="padding:5px"></span>
					<span id="spScheDayOfWeek" style="padding-right:10px">
						<asp:RadioButtonList ID="rblScheDayOfWeek" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
					</span>
					<uc:DateTimeInput ID="ucScheDateTime" runat="server" YearList="<%# DateTimeUtility.GetBackwardYearListItem() %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
				</div>
			</div>
		</td>
	</tr>
</table>
<script type="text/javascript">
<!--
function RefleshTargetListSchedule()
{
	// 詳細スケジュール設定？
	if (document.getElementById('<%= rbExecBySchedule.ClientID %>').checked)
	{
		document.getElementById('dvScheduleDetail').style.display = 'inline';
		
		// 日単位
		if (document.getElementById('<%= rbScheRepeatDay.ClientID %>').checked)
		{
			document.getElementById('spScheTopString').innerText = '毎日';
			document.getElementById('spScheDayOfWeek').style.display = 'none';

			document.getElementById('<%= ucScheDateTime.DdlYear.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlMonth.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlDay.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.MonthYearSeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DaySeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DayString.ClientID %>').style.display = 'none';
		}
		// 週単位
		else if (document.getElementById('<%= rbScheRepeatWeek.ClientID %>').checked)
		{
			document.getElementById('spScheTopString').innerText = '毎週';
			document.getElementById('spScheDayOfWeek').style.display = 'inline';

			document.getElementById('<%= ucScheDateTime.DdlYear.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlMonth.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlDay.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.MonthYearSeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DaySeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DayString.ClientID %>').style.display = 'none';
		}
		// 月単位
		else if (document.getElementById('<%= rbScheRepeatMonth.ClientID %>').checked)
		{
			document.getElementById('spScheTopString').innerText = '毎月';
			document.getElementById('spScheDayOfWeek').style.display = 'none';

			document.getElementById('<%= ucScheDateTime.DdlYear.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlMonth.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlDay.ClientID %>').style.display = 'inline';
			document.getElementById('<%= ucScheDateTime.MonthYearSeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DaySeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DayString.ClientID %>').style.display = 'inline';
		}
		// 一回のみ
		else if (document.getElementById('<%= rbScheRepeatOnce.ClientID %>').checked)
		{
			document.getElementById('spScheTopString').innerText = '';
			document.getElementById('spScheDayOfWeek').style.display = 'none';

			document.getElementById('<%= ucScheDateTime.DdlYear.ClientID %>').style.display = 'inline';
			document.getElementById('<%= ucScheDateTime.DdlMonth.ClientID %>').style.display = 'inline';
			document.getElementById('<%= ucScheDateTime.DdlDay.ClientID %>').style.display = 'inline';
			document.getElementById('<%= ucScheDateTime.MonthYearSeparator.ClientID %>').style.display = 'inline';
			document.getElementById('<%= ucScheDateTime.DaySeparator.ClientID %>').style.display = 'inline';
			document.getElementById('<%= ucScheDateTime.DayString.ClientID %>').style.display = 'none';
		}
	}
	else
	{
		document.getElementById('dvScheduleDetail').style.display = 'none';

	}
}
//-->
</script>