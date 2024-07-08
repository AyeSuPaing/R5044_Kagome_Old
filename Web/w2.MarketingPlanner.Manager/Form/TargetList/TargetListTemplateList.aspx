<%--
=========================================================================================================
  Module      : ターゲットリストテンプレート一覧ページ(TargetListTemplateList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="TargetListTemplateList.aspx.cs" Inherits="Form_TargetList_TargetListTemplateList" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="w2.App.Common.TargetList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
// 選択されたテンプレートIDを親ページに連携
function setTemplateId(templateId)
{
	if (window.opener != null)
	{
		window.opener.applyTargetListTemplate(templateId);
		window.close();
	}
}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="605" border="0">
	<tr>
		<td><h1 class="page-title">ターゲットリストテンプレート情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />カテゴリ</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSearchCategory" runat="server">
																<asp:ListItem Selected="True"></asp:ListItem>
																<asp:ListItem>ステップメール</asp:ListItem>
																<asp:ListItem>誕生日施策</asp:ListItem>
																<asp:ListItem>新規獲得</asp:ListItem>
																<asp:ListItem>CPM施策</asp:ListItem>
																<asp:ListItem>RFM施策</asp:ListItem>
																<asp:ListItem>掘り起こし施策</asp:ListItem>
																<asp:ListItem>カゴ落ち施策</asp:ListItem>
																<asp:ListItem>よくある設定</asp:ListItem>
															</asp:DropDownList></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															テンプレート名</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbSearchTemplateName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="1" Selected="True">テンプレートID/昇順</asp:ListItem>
																<asp:ListItem Value="2">テンプレートID/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " onclick="btnSearch_Click"></asp:Button></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TARGETLIST_TEMPLATELIST %>">クリア</a></div>
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
	</tr>
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">ターゲットリストテンプレート一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="100%" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" border="0" width="100%">
							<tr>
								<td align="center" style="width: 600px">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="550" border="0">
													<tr class="list_title_bg">
														<td align="center">ID</td>
														<td align="center">テンプレート名</td>
														<td align="center">カテゴリ</td>
													</tr>
													<asp:Repeater ID="rTargetListTemplates" runat="server" ItemType="TargetListTemplate">
													<ItemTemplate>
													<tr class="list_item_bg<%#: Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%#: Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="javascript:setTemplateId('<%#: StringUtility.ToEmpty(Item.TemplateId) %>')">
														<td align="center"><%#: Item.TemplateId %></td>
														<td align="left"><%#: Item.Name %></td>
														<td align="left"><%#: Item.Category %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" colspan="3" runat="server"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="7" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
