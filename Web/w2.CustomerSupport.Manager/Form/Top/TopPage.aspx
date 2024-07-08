<%--
=========================================================================================================
  Module      : トップページ(TopPage.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import namespace="w2.App.Common.Cs.IncidentCategory" %>
<%@ Import namespace="w2.App.Common.Cs.ShareInfo" %>
<%@ Import namespace="w2.App.Common.Cs.Incident" %>
<%@ Import namespace="w2.App.Common.Cs.Message" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="TopPage.aspx.cs" Inherits="Form_Top_TopPage" Title="" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.App.Common.Web" %>

<%@ Register Src="~/Form/Top/IncidentAndMessageParts.ascx" TagPrefix="uc1" TagName="IncidentAndMessageParts" %>
<%@ Register Src="~/Form/Top/ListPager.ascx" TagPrefix="uc1" TagName="ListPager" %>
<%@ Register Src="~/Form/Top/SearchParts.ascx" TagPrefix="uc1" TagName="SearchParts" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">

<!-- 共有情報の未読件数表示用スタイル -->
<style type="text/css">
<!--
	.unread 
	{
		display: inline-block;
		height: 9px;
		line-height: 8px;
		border: 2px solid #FFFFFF;
		padding: 4px 4px 2px 4px;
		vertical-align: baseline;
		border-radius: 10px;
		margin-right: 3px;
		
		background-color: #FF3333;
		background-image: -ms-linear-gradient(#ff8135, #DC143C);
		background-image: linear-gradient(#FF3333, #DC143C);
	}
	.unread_text
	{
		color: #FFFFFF;
		font-size: 11px;
		font-weight: bold;
		font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, 'ＭＳ Ｐゴシック', sans-serif;
	}
	.select2-container a:first-child
	{
		Width:150px;
		border-top-left-radius: 0px;
		border-bottom-left-radius: 0px;
		padding:1px;
	}
-->
</style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">

<asp:LinkButton ID="lbRefresh" runat="server" OnClick="lbRefresh_Click"></asp:LinkButton>
<asp:LinkButton ID="lbRefreshIncidentAndMessageBottom" runat="server" OnClick="lbRefreshIncidentAndMessageBottom_Click"></asp:LinkButton>

<table border="0" cellpadding="0" cellspacing="0" width="100%">
<tbody>
	<tr>
	<td></td>
	</tr>
	<tr>
	<td align="center">
		<asp:UpdatePanel ID="up1" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
		<ContentTemplate>

			<asp:LinkButton ID="lbRefreshShareInfoCount" runat="server" OnClick="lbRefreshShareInfoCount_Click" />

			<table width="100%" border="0" cellspacing="0" cellpadding="0">

				<tr>
					<td valign="top"><img height="1" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
					<td width="10" rowspan="4" valign="top"><img alt="" src="../../Images/Common/sp.gif" border="0" height="10" width="5" /></td>
				</tr>

				<!-- タイトル -->
				<tr>
					<td valign="top" style="position: relative;">
						<span class="tab_title_left top_title">
							<%string helpMessage = ""; %>
							<%if (this.TopPageKbn == TopPageKbn.Top) { %>
								<%if (this.IncidentCategoryId == "") { %>
									<% helpMessage = "下記抽出条件にヒットしたインシデントを表示"; %>
									トップ
								<%} else {%>
									<% helpMessage = "該当カテゴリに属し、下記抽出条件にヒットしたインシデントを表示"; %>
									<span style="font-size: smaller">
										<asp:Repeater ID="rCategoryNavigation" runat="server">
										<ItemTemplate>
											<%#: ((CsIncidentCategoryModel)Container.DataItem).CategoryName %>
										</ItemTemplate>
										<SeparatorTemplate>&gt;</SeparatorTemplate>
										</asp:Repeater>
									</span>
								<%} %>
							<%} else if (this.TopPageKbn == TopPageKbn.Approval) {%>
								<% helpMessage = "自分に対しての承認依頼待ちのメッセージを表示"; %>
								承認
							<%} else if (this.TopPageKbn == TopPageKbn.ApprovalRequest) {%>
								<% helpMessage = "自分が承認依頼をしているもの（依頼取り下げを含む）のメッセージを表示"; %>
								承認依頼中
							<%} else if (this.TopPageKbn == TopPageKbn.ApprovalResult) {%>
								<% helpMessage = "自分が承認依頼をしたもので結果が戻ってきたもののメッセージを表示"; %>
								承認依頼 結果返却済み
							<%} else if (this.TopPageKbn == TopPageKbn.Draft) {%>
								<% helpMessage = "自分が下書き保存したメッセージを表示"; %>
								下書き
							<%} else if (this.TopPageKbn == TopPageKbn.Reply) {%>
								<% helpMessage = "自分が回答したメッセージを表示"; %>
								回答済み
							<%} else if (this.TopPageKbn == TopPageKbn.Send) {%>
								<% helpMessage = "自分に対しての送信代行待ちのメッセージを表示"; %>
								送信代行
							<%} else if (this.TopPageKbn == TopPageKbn.SendRequest) {%>
								<% helpMessage = "自分が送信代行依頼をしているもの（依頼取り下げを含む）のメッセージを表示"; %>
								送信依頼中
							<%} else if (this.TopPageKbn == TopPageKbn.SendResult) {%>
								<% helpMessage = "自分が送信代行依頼をしたもので結果が戻ってきたもののメッセージを表示"; %>
								送信依頼 結果返却済み
							<%} else if (this.TopPageKbn == TopPageKbn.SearchIncident) {%>
								<% helpMessage = "検索にヒットしたインシデントを表示"; %>
								検索 - インシデント
							<%} else if (this.TopPageKbn == TopPageKbn.SearchMessage) {%>
								<% helpMessage = "検索にヒットしたメッセージを表示"; %>
								検索 - メッセージ
							<%} else if (this.TopPageKbn == TopPageKbn.TrashIncident) {%>
								<% helpMessage = "削除されているインシデントを表示"; %>
								ゴミ箱 - インシデント
							<%} else if (this.TopPageKbn == TopPageKbn.TrashMessage) {%>
								<% helpMessage = "削除されている全てのメッセージを表示"; %>
								ゴミ箱 - メッセージ
							<%} else if (this.TopPageKbn == TopPageKbn.TrashRequest) {%>
								<% helpMessage = "自分が承認/送信代行依頼したなかで削除されているメッセージを表示"; %>
								ゴミ箱 - 承認/送信代行
							<%} else if (this.TopPageKbn == TopPageKbn.TrashDraft) {%>
								<% helpMessage = "自分が下書き保存したなかで削除されているメッセージを表示"; %>
								ゴミ箱 - 下書き
							<%} %>
							<span data-popover-message="<%= helpMessage %>" class="title-help">
								<span class="icon-help"></span>
							</span>
							</span>
						</span>
						<!-- 共有情報の未読件数バッジ表示 -->
						<asp:UpdatePanel ID="upShareInfoCount" runat="server">
						<ContentTemplate>
							<% if (this.ShareInfoCountString != null) { %>
							<div class="unread" style="position: absolute; right: -12px; top: -9px; z-index: 100;">
								<span class="unread_text"><%= this.ShareInfoCountString %></span>
							</div>
							<% } %>
						</ContentTemplate>
						</asp:UpdatePanel>

						<!-- 上部３ボタンメニュー -->
						<div class="btn-group-top-right">
							<span class="tab_title_right"><a href="javascript:open_window('<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFO_LIST %>','shareinfo','width=1000,height=540,toolbar=1,resizable=1,status=1,scrollbars=1');"><img src="../../Images/Cs/topmenu_shareinfo.png" alt="共有情報" /></a></span>
							<span class="tab_title_right"><a href='Javascript:open_window("<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_USER_SEARCH %>", "user_search","width=1200,height=770,toolbar=1,resizable=1,status=1,scrollbars=1");'><img src="../../Images/Cs/topmenu_phone.png" alt="電話問合せ" /></a></span>
							<span class="tab_title_right"><asp:LinkButton ID="lbReceiveMail" runat="server" OnClick="lbReceiveMail_Click"><img src="../../Images/Cs/topmenu_recievemail.png" alt="メール受信" /></asp:LinkButton></span>
							<%if (this.LoginOperatorCsInfo.EX_PermitEditFlg) {%>
							<span class="tab_title_right"><a href='Javascript:open_window("<%= WebSanitizer.UrlAttrHtmlEncode(
								new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_MESSAGE_INPUT)
									.AddParam(Constants.REQUEST_KEY_MESSAGE_MEDIA_MODE, Constants.KBN_MESSAGE_MEDIA_MODE_MAIL)
									.AddParam(Constants.REQUEST_KEY_MESSAGE_EDIT_MODE, Constants.KBN_MESSAGE_EDIT_MODE_NEW).CreateUrl()) %>", "message","width=1200,height=770,toolbar=1,resizable=1,status=1,scrollbars=1");'><img src="../../Images/Cs/topmenu_newmail.png" alt="メール作成" /></a></span>
							<%} %>
						</div>
						<div style="clear: both; height: 25px;"></div>
						<div class="btn-group-top-right" style="height: 40px;" runat="server" id="divSearchTop">
							<table>
								<tr>
									<td>
										<asp:TextBox ID="tbKeywordTop" Width="200" runat="server" style="font-size: 11px;" height="15px" MaxLength="100" placeholder="検索ワードを入力してください" />
										<asp:Button ID="btnSearchTop" Text="  検索  " runat="server" OnClick="btnSearchTop_Click" Height="28px" Width="60px" /><br />
									</td>
								</tr>
							</table>
						</div>
						<asp:UpdatePanel ID="up3" runat="server">
						<ContentTemplate>
							<br />
							<span class="tab_title_right" style="font-size:8pt;color:#666666"><asp:Label ID="receivedCount" runat="server"></asp:Label></span>
						</ContentTemplate>
						</asp:UpdatePanel>
					</td>
				</tr>

				<!-- 個人/グループタスク一覧 -->
				<tr id="taskList" runat="server">
					<td valign="top">
					<img height="3" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />

					<!--▽ ページング ▽-->
					<% if (ucListPager.Visible) { %>
					<table cellspacing="0" cellpadding="0" border="0" style="<%= (this.TopPageKbn != TopPageKbn.Top) ? "margin-bottom: -30px;" : string.Empty %>">
						<tr>
							<td width="1000px"><uc1:ListPager ID="ucListPager" runat="server" /></td>
						</tr>
					</table>
					<% } %>
					<!--△ ページング △-->
					<img height="2" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />

					<!-- 個人/グループタスク一覧 -->
					<div id="divTasks" runat="server">
						<%if (this.TopPageKbn == TopPageKbn.Top) { %>
						<!-- タスク上部の絞り込みリンク -->
						<div class="text_top btns-filter">
							<span class="btn-group">
								<asp:LinkButton ID="lbRefineTaskUncomplete" CommandArgument="Uncomplete" runat="server" OnClick="lbRefineTaskStatus_Click">未完了</asp:LinkButton>
							</span>
							<span class="btn-group">
								<asp:LinkButton ID="lbRefineTaskNone" CommandArgument="None" runat="server" OnClick="lbRefineTaskStatus_Click">未対応</asp:LinkButton>
								<asp:LinkButton ID="lbRefineTaskActive" CommandArgument="Active" runat="server" OnClick="lbRefineTaskStatus_Click">対応中</asp:LinkButton>
								<asp:LinkButton ID="lbRefineTaskSuspend" CommandArgument="Suspend" runat="server" OnClick="lbRefineTaskStatus_Click">保留</asp:LinkButton>
								<asp:LinkButton ID="lbRefineTaskUrgent" CommandArgument="Urgent" runat="server" OnClick="lbRefineTaskStatus_Click">至急</asp:LinkButton>
							</span>
							<span class="btn-group">
								<asp:LinkButton ID="lbRefineTaskComplete" CommandArgument="Complete" runat="server" OnClick="lbRefineTaskStatus_Click">完了</asp:LinkButton>
							</span>
							<span class="btn-group">
								<asp:LinkButton ID="lbRefineTaskAll" CommandArgument="All" runat="server" OnClick="lbRefineTaskStatus_Click">すべて</asp:LinkButton>
							</span>
							<span class="btn-group">
								<span class="btn-group-title">
									担当
								</span>
								<span style="display:flex;width: 130;">
								<div class="btn-group" style="display: flex;align-items: center;">
									<asp:LinkButton ID="lbChangeTaskTargetGroup" runat="server" CommandArgument="Group" OnClick="lbChangeTaskTarget_Click" title="選択しているグループが設定されているものを表示" Width="74.09">グループ</asp:LinkButton>
									<div class="select2-containe">
										<asp:DropDownList ID="ddlGroupList" runat="server" CssClass="select2-selectTop" Enabled="True" AutoPostBack="True" DataTextField="Key" DataValueField="Value" OnSelectedIndexChanged="ddlGroupList_SelectedIndexChanged">
										</asp:DropDownList>
									</div>
								</div>
								<div class="btn-group" style="display: flex;align-items: center;">
									<asp:LinkButton ID="lbChangeTaskTargetPersonal" runat="server" CommandArgument="Personal" OnClick="lbChangeTaskTarget_Click" title="選択しているオペレータが設定されているものを表示" Width="50.8">個人</asp:LinkButton>
									<div class="select2-containe">
										<asp:DropDownList ID="ddlOperatorList" runat="server" CssClass="select2-selectTop" Enabled="False" AutoPostBack="True" DataTextField="Key" DataValueField="Value" OnSelectedIndexChanged="ddlOperatorList_SelectedIndexChanged">
										</asp:DropDownList>
									</div>
								</div>
								<asp:LinkButton ID="lbChangeTaskTargetAll" runat="server" CommandArgument="All" OnClick="lbChangeTaskTarget_Click" title="担当関係なく全て表示">すべて</asp:LinkButton>
								<asp:LinkButton ID="lbChangeTaskTargetUnassign" runat="server" CommandArgument="Unassign" OnClick="lbChangeTaskTarget_Click" title="インシデントの担当オペレータに設定されていないものを表示">未割当</asp:LinkButton>
								</span>
							</span>
						</div>
						<%} %>

						<div class="tab_title_right">
							<%if (ManagerMenuCache.Instance.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_INCIDENT_MESSAGE_DL))
							{ %>
							<%if (this.TopPageKbn == TopPageKbn.SearchIncident) { %><asp:LinkButton id="lbExportIncident" Runat="server" OnClick="lbExport_Click" CommandName="product" CssClass="cmn-btn-sub-action">エクスポート</asp:LinkButton><% } %>
							<%if (this.TopPageKbn == TopPageKbn.SearchMessage) { %><asp:LinkButton id="lbExportMessage" Runat="server" OnClick="lbExport_Click" CommandName="product" CssClass="cmn-btn-sub-action">エクスポート</asp:LinkButton><% } %>
							<%} %>
							<asp:LinkButton ID="lbUpdateAllIncident" runat="server" OnClientClick="disp_list_update_confirm()" CssClass="cmn-btn-sub-action">一括更新</asp:LinkButton>
							<% if (this.CanDeleteOnList) {%>
								<% if (this.IsTrashPageForNotOfficiallySentMessageOrIsAuthorizedOperatorToPermanentlyDelete) { %>
									<asp:LinkButton ID="lbDeleteIncidents" runat="server" OnClientClick="return disp_list_delete_confirm('チェック項目を全て完全削除します。よろしいですか？')" OnClick="lbDeleteIncidents_Click" CssClass="cmn-btn-sub-action">一括削除</asp:LinkButton>
								<% } %>
							<% } else if (this.CanTrashOnList) { %>
								<% if (this.LoginOperatorCsInfo.EX_PermitEditFlg) { %>
									<asp:LinkButton ID="lbTrashIncidents" runat="server" OnClientClick="return disp_list_delete_confirm('チェック項目を全てゴミ箱へ移動します。よろしいですか？\r\n（ロック中のものは移動されません）')" OnClick="lbTrashIncidents_Click" CssClass="cmn-btn-sub-action">一括削除</asp:LinkButton>
								<% } else { %>
									<a class="cmn-btn-sub-action">一括削除</a>
								<% } %>
							<% } %>
						</div>
						<br class="clr" />


						<!-- リスト -->
						<div class="dataresult larger">
							<!-- リストタイトル -->
							<div>
								<table class="list_table">
								<thead>
								<tr class="list_title_bg">
									<th>
										<%if (this.TopPageKbn == TopPageKbn.Top){ %>
											<%if (this.TaskTargetMode == TaskTargetMode.Personal) {%>
											個人タスク
											<%} else if (this.TaskTargetMode == TaskTargetMode.Group) {%>
											グループタスク
											<%} else if (this.TaskTargetMode == TaskTargetMode.All) {%>
											すべて
											<%} else if (this.TaskTargetMode == TaskTargetMode.Unassign) {%>
											未割当
											<%} %>
										<%} else if (this.TopPageKbn == TopPageKbn.ApprovalRequest){ %>
										承認依頼メッセージ（依頼）
										<%} else if (this.TopPageKbn == TopPageKbn.ApprovalResult){ %>
										承認依頼メッセージ（結果返却済み）
										<%} else if (this.TopPageKbn == TopPageKbn.Approval){ %>
										承認メッセージ（承認）
										<%} else if (this.TopPageKbn == TopPageKbn.SendRequest){ %>
										送信依頼メッセージ（依頼）
										<%} else if (this.TopPageKbn == TopPageKbn.SendResult){ %>
										送信依頼メッセージ（結果返却済み）
										<%} else if (this.TopPageKbn == TopPageKbn.Send){ %>
										送信代行メッセージ（送信代行）
										<%} else if (this.TopPageKbn == TopPageKbn.Draft){ %>
										下書き
										<%} else if (this.TopPageKbn == TopPageKbn.Reply){ %>
										回答済み
										<%} else if (this.TopPageKbn == TopPageKbn.SearchIncident){ %>
										インシデント検索
										<%} else if (this.TopPageKbn == TopPageKbn.SearchMessage){ %>
										メッセージ検索
										<%} else if (this.TopPageKbn == TopPageKbn.TrashIncident){ %>
										ゴミ箱-インシデント
										<%} else if (this.TopPageKbn == TopPageKbn.TrashMessage){ %>
										ゴミ箱-メッセージ
										<%} else if (this.TopPageKbn == TopPageKbn.TrashRequest){ %>
										ゴミ箱-承認/送信代行
										<%} else if (this.TopPageKbn == TopPageKbn.TrashDraft){ %>
										ゴミ箱-下書き
										<%} %>
										（該当 <asp:Literal ID="lListCount" runat="server"></asp:Literal> 件）
									</th>
								</tr>
								</thead>
								</table>
							</div>

							<div class="y_scrollable div_table_header">
								<table class="list_table_min tableHeader check_header_incident" style="<%= this.divIncidentList.Visible ? "" : "display:none" %>">
									<tr class="alt">
										<%if (this.CanDeleteOnList || this.CanTrashOnList) {%>
										<td align="center" width="31">
											<input id="iptChecktAllList" type="checkbox" onclick="check_all_list(this)">
										</td>
										<%} %>
										<td align="center" width="120" style="white-space: nowrap">
											<asp:LinkButton runat="server" ID="lbIncidentId" OnClick="IncidentListSort_Click" CommandArgument="IncidentId" Width="100%" Font-Underline="false">インシデントID
												<span style="float:right"><asp:Label runat="server" ID="lIncidentIdIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<td align="left" width="0" style="white-space: nowrap">
											<asp:LinkButton runat="server" ID="lbIncidentTitle" OnClick="IncidentListSort_Click" CommandArgument="IncidentTitle" Width="100%" Font-Underline="false">タイトル
												<span style="float:right"><asp:Label runat="server" ID="lIncidentTitleIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<td align="left" width="120" style="white-space: nowrap">
											<asp:LinkButton runat="server" ID="lbInquirySource" OnClick="IncidentListSort_Click" CommandArgument="EX_InquirySource" Width="100%" Font-Underline="false">問合せ元
												<span style="float:right"><asp:Label runat="server" ID="lInquirySourceIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<td align="left" width="120" style="white-space: nowrap">
											<asp:LinkButton runat="server" ID="lbIncidentCategoryName" OnClick="IncidentListSort_Click" CommandArgument="EX_IncidentCategoryName" Width="100%" Font-Underline="false">カテゴリ
												<span style="float:right"><asp:Label runat="server" ID="lIncidentCategoryNameIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<td align="center" width="90" style="white-space: nowrap">
											<asp:LinkButton runat="server" ID="lbStatusText" OnClick="IncidentListSort_Click" CommandArgument="EX_StatusText" Width="100%" Font-Underline="false">ステータス
												<span style="float:right"><asp:Label runat="server" ID="lStatusTextIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<td align="left" width="120" style="white-space: nowrap">
											<asp:LinkButton runat="server" ID="lbCsGroupNameText" OnClick="IncidentListSort_Click" CommandArgument="EX_CsGroupName" Width="100%" Font-Underline="false">グループ
												<span style="float:right"><asp:Label runat="server" ID="lCsGroupNameIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<td align="left" width="90" style="white-space: nowrap">
											<asp:LinkButton runat="server" ID="lbCsOperatorName" OnClick="IncidentListSort_Click" CommandArgument="EX_CsOperatorName" Width="100%" Font-Underline="false">オペレータ
												<span style="float:right"><asp:Label runat="server" ID="lCsOperatorNameIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<%if (this.TopPageKbn == TopPageKbn.TrashIncident) { %>
										<td align="center" width="120" style="white-space: nowrap">
											<asp:LinkButton runat="server" ID="lbDateChanged" OnClick="IncidentListSort_Click" CommandArgument="DateChanged" Width="100%" Font-Underline="false">更新日時
												<span style="float:right"><asp:Label runat="server" ID="lDateChangedIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<%} else {%>
										<td align="center" width="120" style="white-space: nowrap">
											<asp:LinkButton runat="server" ID="lbDateLastReceived" OnClick="IncidentListSort_Click" CommandArgument="DateLastReceived" Width="100%" Font-Underline="false">受付日時
												<span style="float:right"><asp:Label runat="server" ID="lDateLastReceivedIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<td align="center" width="120" style="white-space: nowrap">
											<asp:LinkButton runat="server" ID="lbDateMessageLastSendReceived" OnClick="IncidentListSort_Click" CommandArgument="DateMessageLastSendReceived" Width="100%" Font-Underline="false">送受信日時
												<span style="float:right"><asp:Label runat="server" ID="lDateMessageLastSendReceivedIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<%} %>
										<td width="12"></td>
									</tr>
								</table>

								<table style="<%= this.divMessageList.Visible ? "" : "display:none" %>">
									<tr class="alt">
											<%if (this.CanDeleteOnList || this.CanTrashOnList) {%>
											<td align="center" width="31"><input id="Checkbox1" type="checkbox" onclick="check_all_list(this)"></td>
											<%} %>
											<td align="center" width="120">
												<asp:LinkButton runat="server" ID="lbIncidentIdMsg" OnClick="MessageListSort_Click" CommandArgument="IncidentId" Width="100%" Font-Underline="false">インシデントID
													<span style="float:right"><asp:Label runat="server" ID="lIncidentIdMessageIconSort"></asp:Label></span>
												</asp:LinkButton>
											</td>
											<td align="left" width="0">
												<asp:LinkButton runat="server" ID="lbInquiryTitle" OnClick="MessageListSort_Click" CommandArgument="InquiryTitle" Width="100%" Font-Underline="false">件名
													<span style="float:right"><asp:Label runat="server" ID="lInquiryTitleIconSort"></asp:Label></span>
												</asp:LinkButton>
											</td>
											<td align="left" width="120">
												<asp:LinkButton runat="server" ID="lbSender" OnClick="MessageListSort_Click" CommandArgument="EX_Sender" Width="100%" Font-Underline="false">差出人
													<span style="float:right"><asp:Label runat="server" ID="lSenderIconSort"></asp:Label></span>
												</asp:LinkButton>
											</td>
											<td align="center" width="90">
												<%if ((this.TopPageKbn == TopPageKbn.Approval) || (this.TopPageKbn == TopPageKbn.ApprovalRequest) || (this.TopPageKbn == TopPageKbn.ApprovalResult)
													|| (this.TopPageKbn == TopPageKbn.Send) || (this.TopPageKbn == TopPageKbn.SendRequest) || (this.TopPageKbn == TopPageKbn.SendResult)) { %>
												<asp:LinkButton runat="server" ID="lbReplyOperatorName" OnClick="MessageListSort_Click" CommandArgument="EX_NameOperator" Width="100%" Font-Underline="false">依頼者
													<span style="float:right"><asp:Label runat="server" ID="lReplyOperatorNameIconSort"></asp:Label></span>
												</asp:LinkButton>
												<%} else {%>
												<asp:LinkButton runat="server" ID="lbOperatorName" OnClick="MessageListSort_Click" CommandArgument="EX_NameOperator" Width="100%" Font-Underline="false">対応者/<br />作成者
													<span style="float:right"><asp:Label runat="server" ID="lOperatorNameIconSort"></asp:Label></span>
												</asp:LinkButton>
												<%} %>
											</td>
											<td align="center" width="140">
												<asp:LinkButton runat="server" ID="lbMessageStatusName" OnClick="MessageListSort_Click" CommandArgument="EX_MessageStatusName" Width="100%" Font-Underline="false">メッセージステータス
													<span style="float:right"><asp:Label runat="server" ID="lMessageStatusNameIconSort"></asp:Label></span>
												</asp:LinkButton>
											</td>
											<td align="center" width="120">
												<%if ((this.TopPageKbn == TopPageKbn.Draft)
													|| (this.TopPageKbn == TopPageKbn.TrashMessage)
													|| (this.TopPageKbn == TopPageKbn.TrashRequest)
													|| (this.TopPageKbn == TopPageKbn.TrashDraft)) { %>
												<asp:LinkButton runat="server" ID="lbDateChangedMsg" OnClick="MessageListSort_Click" CommandArgument="DateChanged" Width="100%" Font-Underline="false">更新日時
													<span style="float:right"><asp:Label runat="server" ID="lDateChangedMessageIconSort"></asp:Label></span>
												</asp:LinkButton>
												
												<%} else if (this.TopPageKbn == TopPageKbn.Reply) { %>
												<asp:LinkButton runat="server" ID="lbInquiryReplyDate" OnClick="MessageListSort_Click" CommandArgument="InquiryReplyDate" Width="100%" Font-Underline="false">回答日時
													<span style="float:right"><asp:Label runat="server" ID="lInquiryReplyDateIconSort"></asp:Label></span>
												</asp:LinkButton>
												<%} else if ((this.TopPageKbn == TopPageKbn.Approval) || (this.TopPageKbn == TopPageKbn.ApprovalRequest) || (this.TopPageKbn == TopPageKbn.ApprovalResult)
													|| (this.TopPageKbn == TopPageKbn.Send) || (this.TopPageKbn == TopPageKbn.SendRequest) || (this.TopPageKbn == TopPageKbn.SendResult)) { %>
												<asp:LinkButton runat="server" ID="lbRequestDateCreated" OnClick="MessageListSort_Click" CommandArgument="EX_Request" Width="100%" Font-Underline="false">依頼日時
													<span style="float:right"><asp:Label runat="server" ID="lRequestDateCreatedIconSort"></asp:Label></span>
												</asp:LinkButton>
												<%} else {%>
												<asp:LinkButton runat="server" ID="lbDateCreated" OnClick="MessageListSort_Click" CommandArgument="DateCreated" Width="100%" Font-Underline="false">作成日時
													<span style="float:right"><asp:Label runat="server" ID="lDateCreatedIconSort"></asp:Label></span>
												</asp:LinkButton>
												<%} %>
											</td>
											<td width="12"></td>
									</tr>
								</table>
							</div>
							<div class="y_scrollable div_table_data" style="HEIGHT: 160px;">
								<!-- リスト（インシデント） -->
								<div id="divIncidentList" class="dataresult larger check_data_incident" runat="server">
									<div class="dataresult">
									<table id="incident_list" class="list_table_min tableData" style="<%= this.divIncidentList.Visible ? "" : "display:none" %>">
										<asp:Repeater ID="rIncidentList" ItemType="w2.App.Common.Cs.Incident.CsIncidentModel" runat="server">
										<HeaderTemplate>
										</HeaderTemplate>
										<ItemTemplate>
											<tr id="toplist_tr<%#: Item.IncidentId %>" class="<%# "oplist_item_bg" + (Container.ItemIndex % 2 + 1) %>" onmouseover="listselect_mover_cs(this)" onmouseout="listselect_mout_cs(this, <%# Container.ItemIndex % 2 %>)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick_cs(this, <%# Container.ItemIndex % 2 %>, '<%#: Item.IncidentId %>')">
												<%if (this.CanDeleteOnList || this.CanTrashOnList) {%>
												<td width="31" align="center">
													<asp:CheckBox id="cbCheck" CssClass="list_checkbox" runat="server" />
													<span style="display:none" class="list_update" id="lockFlg_<%#: Item.IncidentId %>"><%#: StyleListUpdateLockFlg(Item.LockStatus)%></span>
													<asp:HiddenField ID="hfIncidentId" Value="<%#: Item.IncidentId %>" runat="server" />
												</td>
												<%} %>
												<td width="120" align="center">
													<%#: Item.IncidentId %>
													<a id="toplist_a<%#: Item.IncidentId %>" href="#"></a>
												</td>
												<td runat="server" width="0" align="left" title="<%#: ((CsIncidentModel)Container.DataItem).IncidentTitle %>" style="white-space: nowrap">
													<span class="incident-warning-icon" 
														data-incident-status="<%#: Item.Status %>"
														data-is-need-to-display="<%#: IsNeedToDisplayWarningIcon(Item, Item.LastMessage) %>"
														data-incident-last-received="<%#: Item.DateMessageLastSendReceived %>">！</span>
													<%#: AbbreviateString(((CsIncidentModel)Container.DataItem).IncidentTitle, 50) %>
													<img id="imgLockTop_<%#: ((CsIncidentModel)Container.DataItem).IncidentId %>" style="display:<%#: (((CsIncidentModel)Container.DataItem).LockStatus != Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE) ? "inline" : "none"%>" src="../../Images/Cs/icon_lock.png" alt="ロック" width="14" height="14" />
												</td>
												<td width="120" align="left" title="<%#: GetFromString(Item, true) %>" style="white-space: nowrap">
													<%#: AbbreviateString(GetFromString(Item), 7) %>
												</td>
												<td width="120" align="left"  title="<%#: Item.EX_IncidentCategoryName %>" style="white-space: nowrap">
													<%#: AbbreviateString(Item.EX_IncidentCategoryName, 7) %>
												</td>	
												<td width="90" align="center"
													title="<%#: Item.IsExistLastMessage() ? Item.LastMessage.GetMessageComment() : string.Empty %>"
													style="white-space: nowrap">
													<%#: Item.EX_StatusText %>
													<img src="../../Images/Cs/<%#: Item.IsExistLastMessage() ? Item.LastMessage.GetMessageIcon() : string.Empty %>"
														style="display:<%#: Item.IsExistLastMessage() ? "inline" : "none"%>"
														alt="mail"/>
												</td>
												<td width="120" align="left" style="white-space: nowrap">
													<%#: Item.EX_CsGroupName %>
												</td>
												<td width="90" align="left" title="<%#: (Item.EX_CsGroupName + "\r\n" + Item.EX_CsOperatorName).Trim() %>" style="white-space: nowrap">
													<%#: Item.EX_CsOperatorName %>
												</td>
												
												<%if (this.TopPageKbn == TopPageKbn.TrashIncident) { %>
												<td width="120" align="center" style="white-space: nowrap">
													<%#: DateTimeUtility.ToStringForManager(Item.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>
													</td>
													<%} else { %>
													<td width="120" align="center" style="white-space: nowrap">
													<%#: DateTimeUtility.ToStringForManager(Item.DateLastReceived, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>
													</td>
													<td width="120" align="center" style="white-space: nowrap">
														<%#: DateTimeUtility.ToStringForManager(Item.DateMessageLastSendReceived, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>
													</td>
													<%} %>
											</tr>
										</ItemTemplate>
										</asp:Repeater>
										<tr id="trIncidentListNoData" visible="false" class="oplist_item_bg1" runat="server">
											<td colspan="8" align="center">該当データが存在しません</td>
										</tr>
									</table>
									</div>
								</div>

								<!-- リスト（メッセージ） -->
								<div id="divMessageList" class="dataresult larger" runat="server">
									<div class="dataresult larger">
									<table id="message_list" class="list_table_min">
										<asp:Repeater ID="rMessageList" ItemType="w2.App.Common.Cs.Message.CsMessageModel" runat="server">
										<HeaderTemplate>										
										</HeaderTemplate>
										<ItemTemplate>
											<tr id="toplist_tr<%#: Item.IncidentId %>_<%#: Item.MessageNo %>" class="<%# "oplist_item_bg" + (Container.ItemIndex % 2 + 1) %>" onmouseover="listselect_mover_cs(this)" onmouseout="listselect_mout_cs(this, <%# Container.ItemIndex % 2 %>)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick_cs(this, <%# Container.ItemIndex % 2 %>, '<%#: Item.IncidentId %>', '<%#: Item.MessageNo %>')">
												<%if (this.CanDeleteOnList || this.CanTrashOnList) {%>
												<td width="31" align="center">
													<asp:CheckBox id="cbCheck" CssClass="list_checkbox" runat="server" />
													<span style="display:none" class="list_update"><%#: (IsLockStatus(Item) == false) %></span>
													<asp:HiddenField ID="hfIncidentId" Value="<%#: Item.IncidentId %>" runat="server" />
													<asp:HiddenField ID="hfMessageNo" Value="<%#: Item.MessageNo %>" runat="server" />
												</td>
												<%} %>
												<td width="120" align="center">
													<%#: Item.IncidentId %>
													<!--(<%#: Item.MessageNo %>)-->
													<a id="toplist_a<%#: Item.IncidentId %>_<%#: Item.MessageNo %>" href="#"></a>
												</td>
												<td  runat="server" width="0" align="left" style="white-space: nowrap">
													<span class="img_align"><img src="../../Images/Cs/<%#: Item.GetMessageIcon() %>" alt="mail" /></span>
													<%#: (Item.InquiryTitle)%>
													<span id="spUrgent" visible="<%# Item.EX_UrgencyFlg == Constants.FLG_CSMESSAGEREQUEST_URGENCY_URGENT %>" class="notice" runat="server">*</span>
													<img id="imgLockTop_<%#: Item.IncidentId %>" style="display:<%# IsLockStatus(Item) ? "inline" : "none"%>" src="../../Images/Cs/icon_lock.png" alt="ロック" width="14" height="14" />
												</td>
												<td width="120" align="left" title="<%#: GetFromString(Item) %>" style="white-space: nowrap">
													<%#: AbbreviateString(GetFromString(Item).Replace("\r\n", "\n").Split('\n')[0], 7) %>
												</td>
												<td width="90" align="center" style="white-space: nowrap">
													<%#: (Item.EX_ReplyOperatorName != "") ? Item.EX_ReplyOperatorName : Item.EX_OperatorName%>
												</td>
												<td width="140" align="center" style="white-space: nowrap">
													<%#: Item.EX_MessageStatusName %>
												</td>
												<td width="120" align="center" style="white-space: nowrap">
													<%if ((this.TopPageKbn == TopPageKbn.Draft)
														|| (this.TopPageKbn == TopPageKbn.TrashMessage)
														|| (this.TopPageKbn == TopPageKbn.TrashRequest)
														|| (this.TopPageKbn == TopPageKbn.TrashDraft)) { %>
														<%#: DateTimeUtility.ToStringForManager(Item.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>
													<%} else if (this.TopPageKbn == TopPageKbn.Reply) { %>
														<%#: DateTimeUtility.ToStringForManager(Item.InquiryReplyDate, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>
													<%} else if ((this.TopPageKbn == TopPageKbn.Approval) || (this.TopPageKbn == TopPageKbn.ApprovalRequest) || (this.TopPageKbn == TopPageKbn.ApprovalResult)
														|| (this.TopPageKbn == TopPageKbn.Send) || (this.TopPageKbn == TopPageKbn.SendRequest) || (this.TopPageKbn == TopPageKbn.SendResult)) { %>
														<%#: (Item.EX_Request != null) ? DateTimeUtility.ToStringForManager(Item.EX_Request.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) : "" %>
													<%} else {%>
														<%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>
													<%} %>
												</td>
											</tr>
										</ItemTemplate>
										</asp:Repeater>
										<tr id="trMessageListNoData" visible="false" class="oplist_item_bg1" runat="server">
											<td colspan="7" align="center">該当データが存在しません</td>
										</tr>
									</table>
									</div>
								</div>
							</div>
						<div id="win-size-grip1"><img src ="../../Images/Cs/hsizegrip.png" ></div>
						</div>
					</div>
					</td>
				</tr>
			</table>
			<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />

			<!-- メッセージ＆インシデント下部エリア -->
			<asp:UpdatePanel ID="up2" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
			<ContentTemplate>

				<%-- インシデントメッセージパーツリンク選択ボタン --%>
				<asp:LinkButton ID="lbSelectList" runat="server" OnClick="lbSelectList_Click"></asp:LinkButton>
				<asp:HiddenField id="hfIncidentId" runat="server" />
				<asp:HiddenField id="hfMessageNo" runat="server" />

				<%-- 下部メッセージ＆インシデントユーザーコントロール --%>
				<uc1:IncidentAndMessageParts ID="ucIncidentAndMessageParts" PageKbn="Top" Display="false" runat="server" />

			</ContentTemplate>	
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="lbSelectList" EventName="Click" />
			</Triggers>
			</asp:UpdatePanel>

		</ContentTemplate>
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="ucListPager" EventName="PageChanged" />
			<asp:AsyncPostBackTrigger ControlID="lbRefineTaskUncomplete" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbRefineTaskNone" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbRefineTaskActive" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbRefineTaskSuspend" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbRefineTaskUrgent" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbRefineTaskComplete" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbRefineTaskAll" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbChangeTaskTargetGroup" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbChangeTaskTargetPersonal" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbChangeTaskTargetAll" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbChangeTaskTargetUnassign" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbRefreshIncidentAndMessageBottom" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbTrashIncidents" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbDeleteIncidents" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbReceiveMail" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbIncidentId" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbIncidentTitle" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbInquirySource" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbIncidentCategoryName" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbStatusText" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbCsOperatorName" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbDateLastReceived" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbDateMessageLastSendReceived" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbDateChanged" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbIncidentIdMsg" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbInquiryTitle" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbSender" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbReplyOperatorName" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbOperatorName" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbMessageStatusName" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbDateChangedMsg" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbInquiryReplyDate" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbRequestDateCreated" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbDateCreated" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="lbCsGroupNameText" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="ddlGroupList" EventName="SelectedIndexChanged" />
			<asp:AsyncPostBackTrigger ControlID="ddlOperatorList" EventName="SelectedIndexChanged" />
			<asp:PostBackTrigger ControlID="lbExportIncident" />
			<asp:PostBackTrigger ControlID="lbExportMessage" />

		</Triggers>
		</asp:UpdatePanel>

	</td>
	</tr>

	<!-- ★検索パーツ★ -->
	<tr><td><uc1:SearchParts ID="ucSearchParts" runat="server" Visible="false" /></td></tr>

	<tr>
	<td><img alt="" src="../../Images/Common/sp.gif" border="0" height="10" width="100%" /></td>
	</tr>
</tbody>
</table>


<!-- リスト自動初期選択用JS -->
<script type="text/javascript">
	//----------------------------------------------------
	// リスト選択列保持用（一行選択時、該当列が選択済の場合は色を変えない）
	//----------------------------------------------------
	class_bgcolover = 'oplist_item_bg_over'; // マウスオーバー時の色
	class_bgcolout1 = 'oplist_item_bg1'; // マウスアウトしたときの色１
	class_bgcolout2 = 'oplist_item_bg2'; // マウスアウトしたときの色２
	class_bgcolout3 = 'oplist_item_bg3'; // マウスアウトしたときの色３
	var class_bgcolouts = new Array(class_bgcolout1, class_bgcolout2, class_bgcolout3);
	class_bgcolclck = 'oplist_item_bg_click'; // マウスクリックしたときの色

	var noClickCheck = false;
	var selected_tr = "";
	var selected_before_style = "";
	var single_select = false;
	function listselect_mover_cs(obj)
	{
		if ((single_select == false) && (obj != selected_tr)) listselect_mover(obj);
	}
	function listselect_mout_cs(obj, lineIdx)
	{
		if ((single_select == false) && (obj != selected_tr)) listselect_mout(obj, lineIdx);
	}
	function listselect_mclick_cs(obj, lineIdx, incidentId, messageNo)
	{
		listselect_mclick_cs_inner(obj, incidentId, messageNo, class_bgcolouts[lineIdx]);
	}
	function listselect_mclick_cs_inner(obj, incidentId, messageNo, class_bgcolout)
	{
		// クリック失敗なら抜ける
		if (listselect_mclick(obj, null, noClickCheck) == false) return;
		noClickCheck = false;

		// 以前の選択列の色を戻す
		if (obj != selected_tr) {
			selected_tr.className = selected_before_style;
			selected_tr = obj;
			selected_before_style = class_bgcolout;
		}

		// イベント実行
		if (set_message_list_focus_on_refresh_incident) {
			set_message_list_focus_on_refresh_incident(true);
		}

		document.getElementById('<%= hfIncidentId.ClientID %>').value = incidentId;
		document.getElementById('<%= hfMessageNo.ClientID %>').value = (messageNo) ? messageNo : "";

		setTimeout('__doPostBack("<%= lbSelectList.UniqueID %>", "");', 100);	// ポストバックを終わらせてから実行したい
	}

	// 先頭選択JS
	function listselect_first()
	{
		// クリックチェック解除
		noClickCheck = true;

		// リスト選択
		<%if (rIncidentList.Items.Count > 0) { %>
		var select_tr = $("table#incident_list tr:first");
		select_tr.trigger("click");
		<%} %>
		<%if (rMessageList.Items.Count > 0) { %>
		var select_tr = $("table#message_list tr:first");
		select_tr.trigger("click");
		<%} %>
	}

	// リスト選択JS
	function listselect(incidentId, messageNo)
	{
		// リスト選択
		var idString = incidentId;
		if (messageNo) idString += "_" + messageNo;

		var select_tr = $("tr#toplist_tr" + idString);
		var select_a = $("a#toplist_a" + idString);

		if (select_tr.length != 0) {
			// クリックチェック解除
			noClickCheck = true;
			select_tr.trigger("click");
			select_a.focus();
		}
		else {
			listselect_first();
		}
	}

	// 一覧チェックボックスチェックスクリプト（インシデント・メッセージ共用）
	function check_all_list(cb)
	{
		if (cb.checked) $('span.list_checkbox input').attr('checked', 'checked');
		else $('span.list_checkbox input').removeAttr('checked');
	}

	// 一覧チェックボックスチェックカウント（インシデント・メッセージ共用）
	function disp_list_delete_confirm(message)
	{
		if ($("span.list_checkbox input:checked").length > 0) {
			return confirm(message);
		}
		else {
			alert('削除対象がありません。');
			return false;
		}
	}

	// リフレッシュJS
	function refresh(incidentId, messageNo)
	{
		if (incidentId) document.getElementById('<%= hfIncidentId.ClientID %>').value = incidentId;
		if (messageNo) document.getElementById('<%= hfMessageNo.ClientID %>').value = messageNo;

		__doPostBack("<%= lbRefresh.UniqueID %>", "");
	}
	function refresh_incident_and_imessage_parts()
	{
		__doPostBack("<%= lbRefreshIncidentAndMessageBottom.UniqueID %>", "");
	}

	// 共有情報カウント更新
	function refresh_share_info_count()
	{
		__doPostBack("<%= lbRefreshShareInfoCount.UniqueID %>", "");
	}

	<%-- UpdataPanelの更新時のみ処理を行う --%>
	function bodyPageLoad()
	{
		ddlSelect();

		if (Sys.WebForms == null) return;
		$('.select2-selectTop').select2({
			width: '90%',
			placeholder: '選択してください',
			allowClear: true
		});
		$.extend($.fn.select2.defaults, {
			formatNoMatches: function () { return "該当データがありません"; },
			formatSearching: function () { return "検索中…"; }
		});

		$('.y_scrollable.div_table_data').resizable2({
			handleSelector: "#win-size-grip1",
			resizeWidth: false,
			onDragStart: function (e, $el, opt) {
				$el.css({ 'height': $el.height(), 'max-height': '<%= Constants.VARIABLE_MAXIMUM_SIZE %>px' });
			},
			onDragEnd: function (e, $el, opt) {
				setCookie("<%=Constants.COOKIE_KEY_TOPPAGE_INCIDENT_LIST_HEIGHT%>", $el.height(), { expires: 1000 });
			}
		});
		$('.datagrid.larger .y_scrollable2').resizable2({
			handleSelector: "#win-size-grip2",
			resizeWidth: false,
			onDragStart: function (e, $el, opt) {
				$el.css({ 'height': $el.height(), 'max-height': '<%= Constants.VARIABLE_MAXIMUM_SIZE %>px' });
			},
			onDragEnd: function (e, $el, opt) {
				setCookie("<%=Constants.COOKIE_KEY_TOPPAGE_MESSAGE_HEIGHT%>", $el.height(), { expires: 1000 });
			}
		});
		$('.tab-contents-right.incident-and-message > .y_scrollable').resizable2({
			handleSelector: "#win-size-grip3",
			resizeWidth: false,
			onDragStart: function (e, $el, opt) {
				$el.css({ 'height': $el.height(), 'max-height': '<%= Constants.VARIABLE_MAXIMUM_SIZE %>px' });
			},
			onDragEnd: function (e, $el, opt) {
				setCookie("<%=Constants.COOKIE_KEY_TOPPAGE_INCIDENT_HEIGHT%>", $el.height(), { expires: 1000 });
			}
		});

		$('.y_scrollable.div_table_data').css({ 'height': 'auto', 'max-height': '<%= (CookieManager.GetValue(Constants.COOKIE_KEY_TOPPAGE_INCIDENT_LIST_HEIGHT) ?? Constants.TOPPAGE_INCIDENT_LIST_DEFAULT_HEIGHT_SIZE)%>px' });
		$('.datagrid.larger .y_scrollable2').css({ 'height': 'auto', 'max-height': '<%= (CookieManager.GetValue(Constants.COOKIE_KEY_TOPPAGE_MESSAGE_HEIGHT) ?? Constants.TOPPAGE_MESSAGE_DEFAULT_HEIGHT_SIZE)%>px' });
		$('.tab-contents-right.incident-and-message > .y_scrollable').css({ 'height': 'auto', 'max-height': '<%= (CookieManager.GetValue(Constants.COOKIE_KEY_TOPPAGE_INCIDENT_HEIGHT) ?? Constants.TOPPAGE_INCIDENT_DEFAULT_HEIGHT_SIZE)%>px' });

		redrawIncidentWarningIcon();
	}

	// ページ読込完了時の処理
	$(window).bind('load', function () {
		// CTI連携用：電話番号をGET値として持っていた場合、顧客検索画面をポップアップする。
		if ("<%= Request[Constants.REQUEST_KEY_CUSTOMER_TELNO] %>" != "") {
			open_window("<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_USER_SEARCH + "?" + Constants.REQUEST_KEY_CUSTOMER_TELNO + "=" + Request[Constants.REQUEST_KEY_CUSTOMER_TELNO] %>", "message", "width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");
		}
		// インシデント警告アイコン描画（15秒ごとに更新）
		setInterval(redrawIncidentWarningIcon, 15000);
	});

	// Event update incident
	function disp_list_update_confirm(message) {
		var incidentIdsNotUpdate = '';
		var incidentIdsUpdate = '';
		if ($("span.list_checkbox input:checked").length == 0) {
			alert('更新対象がありません。');
		}
		else {
			$(".dataresult span.list_update").each(function () {
				var checked = (($(this).prev("span.list_checkbox")).children("input:checked").length != 0);
				incidentId = $(this).next("input").val();
				if (checked) {
					if ($(this).html() == "False") {
						incidentIdsNotUpdate += incidentId + ',';
					}
					else {
						incidentIdsUpdate += $(this).next("input").val() + ',';
					}
				}
			});

			if (incidentIdsNotUpdate != '') {
				alert('ロック中の項目を選択しているため、更新できません');
			}
			else {
				open_window('<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENT_INCIDENT_MODIFY_INPUT + "?" + Constants.REQUEST_KEY_INCIDENT_ID%>'
					+ '=' + incidentIdsUpdate.substring(0, incidentIdsUpdate.length - 1), 'UpdateAllIncident', 'width=828,height=540,toolbar=1,resizable=1,status=1,scrollbars=1');
			}
		}

		return false;
	}

	// インシデント警告アイコン描画
	function redrawIncidentWarningIcon() {
		// セッションストレージに期間テーブルがない場合、非同期で取得。
		var incidentWarningIconTermTable = JSON.parse(sessionStorage.getItem("term_table"));
		if (incidentWarningIconTermTable == null) {
			$.ajax({
				url: "IncidentWarningIconTermTable.ashx",
				type: 'POST',
				cache: false,
				dataType: 'json'
			}).done(function(response) {
				var jsonTermTable = JSON.stringify(response);
				sessionStorage.setItem("term_table", jsonTermTable);
				redrawIncidentWarningIcon();
			});
			return;
		}
		var currentDate = new Date;
		if (incidentWarningIconTermTable) {
			$(".incident-warning-icon").each(function () {
				// 設定が無効なら表示しない
				if (incidentWarningIconTermTable[$(this).data("incident-status")] === undefined) return;
				// 最終メッセージのステータス判定
				if ($(this).data("is-need-to-display") === "True") {
					var lastReceivedDate = new Date($(this).data("incident-last-received"));
					var difference = (currentDate - lastReceivedDate) / (1000 * 60);
					var termTable = incidentWarningIconTermTable[$(this).data("incident-status")];
					for (var i = (termTable.length - 1) ; i >= 0; i--) {
						if (termTable[i] === null) continue;
						if (termTable[i] <= difference) {
							$(this).addClass("incident-warning-icon-" + i);
							break;
						}
					}

					if ((difference / 60) < 1) {
						$(this).attr("title", Math.floor(difference) + "分経過");
					} else if ((difference / (60 * 24)) < 1) {
						$(this).attr("title", Math.floor(difference / 60) + "時間経過");
					} else {
						$(this).attr("title", Math.floor(difference / (60 * 24)) + "日経過");
					}

					$(this).css("display", "inline-block");
				} else {
					$(this).css("display", "none");
				}
			});
		}
	}
</script>

<%-- セッションタイムアウト防止 --%>
<script type="text/javascript" src="<%: Constants.PATH_ROOT %>Js/UpdateSessionTimeout.aspx?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>

</asp:Content>
