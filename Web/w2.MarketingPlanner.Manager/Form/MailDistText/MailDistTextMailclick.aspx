	<%--
=========================================================================================================
  Module      : メール配信文章メールクリック設定ページ(MailDistTextMailclick.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MailDistTextMailclick.aspx.cs" Inherits="Form_MailDistText_MailDistTextMailclick" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%
	// チェックボックス一覧作成
	System.Collections.Generic.List<string> lCheckBoxIDs = new System.Collections.Generic.List<string>();

	System.Collections.Generic.List<Repeater> lRepeaters = new System.Collections.Generic.List<Repeater>();
	lRepeaters.Add(rMailTextBody);
	lRepeaters.Add(rMailTextHtml);
	lRepeaters.Add(rMailTextBodyMobile);

	lRepeaters.ForEach((Repeater repeater) =>
	{
		foreach (RepeaterItem ri in repeater.Items)
		{
			CheckBox cbMailClickUrl = ((CheckBox)ri.FindControl("cbMailClickUrl"));
			if (cbMailClickUrl.Visible)
			{
				lCheckBoxIDs.Add(((CheckBox)ri.FindControl("cbMailClickUrl")).ClientID);
			}
		}
	});

	// チェックボックスイベントセット
	cbChangeChecked.Attributes["onClick"] = "ChangeChecks();";
%>
<script type="text/javascript">
<!--
function ChangeChecks()
{
<% foreach (string strCheckBoxId in lCheckBoxIDs) {%>
	if (document.getElementById('<%= strCheckBoxId %>') != null)
	{
		document.getElementById('<%= strCheckBoxId %>').checked = document.getElementById('<%= cbChangeChecked.ClientID %>').checked
	}
<% } %>
}
//-->
</script>
<style type="text/css">
	.dvPreview img
	{
		vertical-align : text-bottom;
		max-width : 240px;
		_width:expression(this.clientWidth > 240 ? "240px" : this.clientWidth);	/* ie6でmax-widthを実現する */
	}
	
	.dvPreview
	{
		width : 240px;
		
		color : black;
		background-color : White;
		border : solid 1px black ;
		
		font-size : 20px;
		font-family : 'ＭＳ ゴシック';
		line-height : normal;
		word-wrap : break-word;
	}
</style>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">メール配信文章設定</h1></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top">
													<asp:CheckBox ID="cbChangeChecked" Text="全てのチェックをON/OFF" runat="server" />
													<asp:Button id="btnUpdate2" runat="server" Text="  メールクリック設定更新  " onclick="btnUpdate_Click" OnClientClick="return confirm('設定を更新してもよろしいですか？');"></asp:Button>
												</div>
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr id="trMailtextId" runat="server">
														<td align="left" class="detail_title_bg" width="30%">メール文章ID</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextId" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">メール文章名</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextName" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg">メールFROM名</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailFromName" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg">メールFROM</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailFrom" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg">メールタイトル</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextSubject" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg">メール文章テキスト</td>
														<td align="left" class="detail_item_bg">
															<asp:Repeater ID="rMailTextBody" runat="server">
																<ItemTemplate>
																	<asp:Literal ID="lMailTextLine" Text="<%# Container.DataItem %>" runat="server"></asp:Literal>
																	<asp:CheckBox ID="cbMailClickUrl" Visible="false" runat="server" /><br />
																</ItemTemplate>
															</asp:Repeater>
														</td>
													</tr>
													<tr >
														<td align="left" class="detail_title_bg">メール文章HTML</td>
														<td align="left" class="detail_item_bg">
															<asp:Repeater ID="rMailTextHtml" runat="server">
																<ItemTemplate>
																	<asp:Literal ID="lMailTextLine" Text="<%# Container.DataItem %>" runat="server"></asp:Literal>
																	<asp:CheckBox ID="cbMailClickUrl" Visible="false" runat="server" /><br />
																</ItemTemplate>
															</asp:Repeater>
														</td>
													</tr>
													<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
													<tr>
														<td align="left" class="detail_title_bg">モバイルメールタイトル</td>
														<td align="left" class="detail_item_bg">
															<div id="dvMailtextSubjectMobile" runat="server" class="dvPreview">
																<asp:Literal id="lMailtextSubjectMobile" runat="server"></asp:Literal></div></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg">モバイルメール文章</td>
														<td align="left" class="detail_item_bg">
															<div id="dvMailtextBodyMobile" runat="server" class="dvPreview">
																<asp:Repeater ID="rMailTextBodyMobile" runat="server">
																	<ItemTemplate>
																		<asp:Literal ID="lMailTextLine" Text="<%# Container.DataItem %>" runat="server"></asp:Literal>
																		<asp:CheckBox ID="cbMailClickUrl" Visible="false" runat="server" />
																		<%# (Container.ItemIndex < ((IList)rMailTextBodyMobile.DataSource).Count - 1) ? "<br />" : ""%>
																	</ItemTemplate>
																</asp:Repeater>
															</div>
													</tr>
													<% } %>
													<tr >
														<td align="left" class="detail_title_bg">作成日</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextDateCreated" runat="server"></asp:Literal></td>
													</tr>
													<tr >
														<td align="left" class="detail_title_bg">更新日</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextDateChanged" runat="server"></asp:Literal></td>
													</tr>
													<tr >
														<td align="left" class="detail_title_bg">最終更新者</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextLastChanged" runat="server"></asp:Literal></td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<asp:Button id="btnUpdate" runat="server" Text="  メールクリック設定更新  " onclick="btnUpdate_Click" OnClientClick="return confirm('設定を更新してもよろしいですか？');"></asp:Button>
												</div>
											</td>
										</tr>
									</table>
									<br />
									<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
										<tr class="info_item_bg">
											<td align="left">
												備考<br />
											　	・メールクリックレポートは、メール配信文章毎に集計されます。<br/>
											　	　メールクリックを別に集計したい場合は、メール配信文章を別に用意してください。<br/>
											　	・動的タグ「<@@user:xxx@@>」を含むメールクリック設定は行えません。
											</td>
										</tr>
									</table>
									<br />
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ 詳細 △-->
</table>
</asp:Content>