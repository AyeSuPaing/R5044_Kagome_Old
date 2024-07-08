<%--
=========================================================================================================
  Module	  : CPMレポート一覧ページ(CpmReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CpmReportList.aspx.cs" Inherits="Form_CpmReport_CpmReportList" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
<style type="text/css">
<!--
.setting_table td {padding-top: 0; padding-bottom: 0;padding-left: 0;padding-right: 0;}
--> 
</style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">CPMレポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">レポート表示</h2></td>
	</tr>
	<tr>
	<td>
	<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
	<tr>
	<td>
	<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
	<td align="center">
		
	<table cellspacing="0" cellpadding="0" border="0">
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<tr>
			<td>
				<div class="search_btn_sub_rt" >
					<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton>
				</div>

				<table class="search_table" width="700" border="0" cellspacing="1" cellpadding="3">
					<tr class="search_title_bg">
						<td>
							<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
							対象年月
						</td>
					</tr>
					<tr class="search_item_bg">
						<td align="center">
							<table width="500">
							<tr>
							<td>
								<!-- ▽カレンダ▽ -->
								<asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
								<!-- △カレンダ△ -->
							</td>
							<td>
								<asp:RadioButtonList id="rblTargetType" Runat="server" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list" OnSelectedIndexChanged="rblTargetType_SelectedIndexChanged" RepeatLayout="Flow">
									<asp:ListItem Text="月別レポート" Value="Monthly"></asp:ListItem>
									<asp:ListItem Text="日別レポート" Value="Daily"></asp:ListItem>
								</asp:RadioButtonList>
							</td>
							</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<tr>
		<td>
			<asp:UpdatePanel runat="server">
			<ContentTemplate>

			<div class="x_scrollable" style="WIDTH:100%">
			<%
				var tableWidth = 1200;
                var columWidthFirst = 100;
                var columWidth = (tableWidth - columWidthFirst) / (Constants.CPM_CLUSTER_SETTINGS.Settings1.Length * Constants.CPM_CLUSTER_SETTINGS.Settings2.Length);
			%>
			<table class="list_table" cellspacing="1" cellpadding="3" border="0" style="width: <%= tableWidth %>px">
				<tr>
					<td class="list_item_bg2" style="white-space: nowrap; width: <%= columWidthFirst %>px;"></td>
					<% foreach (var setting2 in Constants.CPM_CLUSTER_SETTINGS.Settings2) { %>
					<% foreach (var setting1 in Constants.CPM_CLUSTER_SETTINGS.Settings1) { %>
						<td  class="<%: (Constants.CPM_CLUSTER_SETTINGS.Settings2[0] == setting2)  ? "list_item_bg3" : "list_item_bg2" %>" style="white-space:nowrap" align="center" width="<%= columWidth %>px">
							<%: setting1.Name %>
						</td>
					<% } %>
					<% } %>
				</tr>
				<tr valign="top">
					<td class="list_item_bg2" style="white-space:nowrap" align="center"></td>
					<% foreach (var setting2 in Constants.CPM_CLUSTER_SETTINGS.Settings2) { %>
						<td class="<%: (Constants.CPM_CLUSTER_SETTINGS.Settings2[0] == setting2)  ? "list_item_bg3" : "list_item_bg2" %>" colspan="<%: Constants.CPM_CLUSTER_SETTINGS.Settings1.Length %>" align="center" style="padding:0px"  width="<%= columWidth * Constants.CPM_CLUSTER_SETTINGS.Settings1.Length %>">
							<%: setting2.Name %>
						</td>
					<% } %>
				</tr>
				<tr valign="top">
					<td class="list_item_bg5_dark" colspan="2" style="padding:0"></td>
				</tr>
				<asp:Repeater id="rReport" ItemType="w2.Domain.User.Helper.CpmClusterReport" runat="server">
				<ItemTemplate>
					<tr>
						<td class="list_title_bg" style="text-align: center; white-space: nowrap;">																
							<%--▽ 日月表示 ▽--%>
							<%#: this.IsDailyReport
								? DateTimeUtility.ToStringForManager(new DateTime(this.CurrentYear, this.CurrentMonth, Item.No), DateTimeUtility.FormatType.LongDate2Letter)
								: DateTimeUtility.ToStringForManager(new DateTime(this.CurrentYear, Item.No, 1), DateTimeUtility.FormatType.LongYearMonth)
							%>
						</td>
						<asp:Repeater id="rLine" DataSource="<%# Item.Items %>" ItemType="w2.Domain.User.Helper.CpmClusterReportItem"  Runat="server">
						<ItemTemplate>
							<td Visible="<%# Item.Count.HasValue %>" class="list_item_bg1" align="right" runat="server">
								<p style="text-align: right; width: 37px; float: right">
									&nbsp;(<%#: StringUtility.ToNumeric(Item.Percentage) %>%)&nbsp;
								</p>
								<p style="text-align: right; float: right">
									<%#: StringUtility.ToNumeric(Item.Count) %> 人
								</p>
								<br/>
                                <p Visible="<%# Item.GrowthCount.HasValue %>" style="text-align: center" runat="server">
								<%# (Item.GrowthCount.HasValue && Item.GrowthCount >= 0) ? "+" : "-" %><%#: Item.GrowthCount.HasValue ? StringUtility.ToNumeric(Math.Abs(Item.GrowthCount.Value)) : "" %> 人
								（<%# (Item.GrowthCount.HasValue && Item.GrowthCount >= 0) ? "+" : "-" %><%#: Item.GrowthRate.HasValue ? StringUtility.ToNumeric(Math.Abs(Item.GrowthRate.Value)) : "∞" %> %）
								</p>
							</td>
							<td Visible="<%# Item.Count.HasValue == false%>" class="list_item_bg1" align="right" runat="server"> - </td>
						</ItemTemplate>
						</asp:Repeater>
					</tr>
				</ItemTemplate>
				</asp:Repeater>
			</table>
			</div>
			</ContentTemplate>
			</asp:UpdatePanel>
		</td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<tr>
		<td>
			<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
				<tr class="info_item_bg">
					<td align="left">
						備考<br />
						・セル内上段 ： 括弧内の数字は該当クラスタの割合<br/>
						・セル内下段 ： そのクラスタでの伸び数・括弧内は伸び率<br/>
						　（クラスタ未割り当てのユーザーは表には含まれません。）<br/>
						<br/>
						各クラスタの設定は以下の通りです。<br/>
						<table class="setting_table no-border no-padding">
						<% foreach (var setting in Constants.CPM_CLUSTER_SETTINGS.Settings1) { %>
							<% var dispSeparater = false; %>
							<tr>
							<td>
							・<%: setting.Name %> ：
							</td>
							<td>
							<%if (setting.BuyCount.HasValue) {%>
								購入回数が <%: StringUtility.ToNumeric(setting.BuyCount) %> 回以上<% dispSeparater = true; %>
							<% } %>
							<%if (setting.BuyAmount.HasValue) {%>
								<%= dispSeparater ? "、" : "" %>
								購入金額が <%: setting.BuyAmount.ToPriceString(true) %> 以上<% dispSeparater = true; %>
							<%} %>
							<%if (setting.EnrollmentDays.HasValue) {%>
								<%= dispSeparater ? "、" : "" %>
								在籍期間が <%: StringUtility.ToNumeric(setting.EnrollmentDays) %> 日以上<% dispSeparater = true; %>
							<%} %>
							</td>
							</tr>
						<%} %>
						<tr>
						<td>&nbsp;</td>
						</tr>
						<% foreach (var setting in Constants.CPM_CLUSTER_SETTINGS.Settings2) { %>
							<tr>
							<td>
							・<%: setting.Name %> ：
							</td>
							<td>
							離脱期間が <%if (setting.AwayDays.HasValue) {%><%: StringUtility.ToNumeric(setting.AwayDays) %> 日未満<%} else {%>それ以上<%} %><br/>
							</td>
							</tr>
						<%} %>
						</table>
				</tr>
			</table>
		</td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
	</table>
	</td>
	</tr>
	</table>
	</td>
	</tr>
	</table>
	</td>
	</tr> <!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>