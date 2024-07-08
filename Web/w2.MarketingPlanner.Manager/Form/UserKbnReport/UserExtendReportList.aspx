<%--
=========================================================================================================
Module      : ユーザー区分レポート一覧ページ(UserExtendReportList.aspx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserExtendReportList.aspx.cs" Inherits="Form_UserKbnReport_UserExtendReportList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
<tr>
<td><h1 class="page-title">ユーザー区分レポート</h1></td>
</tr>
<tr>
<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
</tr>
<!--▽ 一覧 ▽-->
<tr>
<td>
<div class="tabs-wrapper">
<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USER_KBN_REPORT_LIST  %>">レポート表示</a>
<a href="#" class="current">
ユーザー拡張項目レポート
</a>
</div>
</td>
</tr>
<tr>
<td class="tab-contents">
<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
<tr>
<td>
<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
<tr>
<td align="center">
<table cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td><img height="10" alt="" src="Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
			<div class="search_btn_sub_rt" >
				<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton></div>
		</td>
	</tr>
	<tr>
		<td>
			<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
				<tr class="info_item_bg">
					<td align="left">データ作成日時：<%: DateTimeUtility.ToStringForManager(DateTime.Now, DateTimeUtility.FormatType.LongDateHourMinute2Letter) %></td>
				</tr>
			</table>
		</td>
	</tr>
	<!--▽ 分析結果 ▽-->
	<asp:Repeater ID="rHeadResults" Runat="server">
	<ItemTemplate>
		<tr>
			<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<tr>
			<td>
				<table class="list_table" cellspacing="1" cellpadding="3" width="700" border="0">
					<tr class="list_title_bg">
						<td align="center" width="30%" colspan="4"><asp:Literal ID="lUserHeaderTitle" runat="server" Text="<%# WebSanitizer.HtmlEncode(((KbnAnalysisUtility.KbnAnalysisTable)Container.DataItem).Title) %>" /></td>
					</tr>
					<asp:Repeater id="HeadResult2" Runat="server" DataSource='<%# ((KbnAnalysisUtility.KbnAnalysisTable)Container.DataItem).Rows %>'>
					<ItemTemplate>
						<tr>
							<td class="list_item_bg2" align="center" width="20%"><asp:Literal ID="lUserHeaderName" runat="server" Text="<%# WebSanitizer.HtmlEncode(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Name)%>" /></td>
							<td class="list_item_bg1" align="left" width="60%"><img height="12" alt='<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count)) %>人' src="../../Images/graph_bar01.gif" width='<%# KbnAnalysisUtility.GetRateImgWidth(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count ,((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Total) %>%' border="0" /></td>
							<td class="list_item_bg1" align="right" width="10%"><asp:Literal ID="lUserHeaderRate" runat="server" Text="<%# WebSanitizer.HtmlEncode(KbnAnalysisUtility.GetRateString(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count, ((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Total, 1))%>" />%
							</td>
							<td class="list_item_bg1" align="right" width="10%"><asp:Literal ID="lUserHeaderCount" runat="server" Text="<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count))%>" />人</td>
						</tr>
					</ItemTemplate>
					</asp:Repeater>
				</table>
			</td>
		</tr>
	</ItemTemplate>
	</asp:Repeater>
	<!--△ 分析結果 △-->
	<tr>
		<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
			<table class="search_table" cellspacing="1" cellpadding="3" width="700" border="0">
				<tr>
					<td class="search_title_bg" width="20%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
							border="0" />顧客区分選択</td>
					<td class="search_item_bg">
						<asp:RadioButtonList id="rlUserKbn" runat="server" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list">
							<asp:ListItem Text="全体" Value="" />
						</asp:RadioButtonList>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--▽ 分析結果 ▽-->
	<asp:Repeater ID="rResults" Runat="server">
	<ItemTemplate>
		<tr>
			<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<tr>
			<td>
				<table class="list_table" cellspacing="1" cellpadding="3" width="700" border="0">
					<tr class="list_title_bg">
						<td align="center" width="30%" colspan="4"><asp:Literal ID="lUserTitle" runat="server" Text="<%# WebSanitizer.HtmlEncode(((KbnAnalysisUtility.KbnAnalysisTable)Container.DataItem).Title)%>" /></td>
					</tr>
					<asp:Repeater id="Result2" Runat="server" DataSource='<%# ((KbnAnalysisUtility.KbnAnalysisTable)Container.DataItem).Rows %>'>
						<ItemTemplate>
							<tr>
								<td class="list_item_bg2" align="center" width="20%"><asp:Literal ID="lUserName" runat="server" Text="<%# WebSanitizer.HtmlEncode(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Name)%>" /></td>
								<td class="list_item_bg1" align="left" width="60%"><img height="12" alt='<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count)) %>人' src="../../Images/graph_bar01.gif" width='<%# KbnAnalysisUtility.GetRateImgWidth(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count ,((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Total) %>%' border="0" /></td>
								<td class="list_item_bg1" align="right" width="10%"><asp:Literal ID="lUserRate" runat="server" Text="<%# WebSanitizer.HtmlEncode(KbnAnalysisUtility.GetRateString(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count, ((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Total, 1))%>" />%
								</td>
								<td class="list_item_bg1" align="right" width="10%"><asp:Literal ID="lUserCount" runat="server" Text="<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count))%>" />人</td>
							</tr>
						</ItemTemplate>
					</asp:Repeater>
				</table>

			</td>
		</tr>
	</ItemTemplate>
	</asp:Repeater>
	<!--△ 分析結果 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
	<td>
		<img height="30" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
		<!-- 備考 -->
		<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
			<tr class="info_item_bg">
				<td align="left">
					<table class="no-border no-padding">
						<tr>
							<td colspan="3">■対象</td>
						</tr>
						<tr>
							<td colspan="3">本レポートは、自社サイト会員のみが対象となります。</td>
						</tr>
						<tr>
							<td colspan="3">有効ユーザーはＰＣ会員<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>、モバイル会員<% } %>（退会ユーザーは除く）</td>
						</tr>
						<tr>
							<td colspan="3">入力方法「テキストボックス」を除くユーザー拡張項目設定が集計対象となります。</td>
						</tr>
						<!--<tr>
							<td colspan="3"></td>
						</tr>-->
					</table>
					<br />
					<table class="no-border no-padding">
						<tr>
							<td colspan="3">■集計方法</td>
						</tr>
						<tr>
							<td colspan="3">・ユーザー拡張項目で設定中の各要素（選択肢）の「値」を元に集計が行われます。</td>
						</tr>
						<tr>
							<td colspan="3">・各要素（選択肢）の「値」を変更または削除した場合など、<br />　データベース内の「値」が各要素（選択肢）の「値」のいずれにも一致しなくなった場合は、「上記以外」として集計されます。</td>
						</tr>
						<tr>
							<td colspan="3">・有効ユーザー数が１００％となるよう集計します。（入力方式がチェックボックス以外）</td>
						</tr>
						<tr>
							<td colspan="3">・入力方法がチェックボックスの場合、複数選択が行える仕組み上、<br />　ユーザーの選択項目数（回答数）が有効ユーザー数を超えることになるが、その際には選択項目数を１００％として集計します。</td>
						</tr>
					</table>
					<br />
				</td>
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
