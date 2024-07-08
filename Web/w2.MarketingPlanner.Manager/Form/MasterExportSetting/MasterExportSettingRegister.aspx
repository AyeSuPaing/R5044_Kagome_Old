<%--
=========================================================================================================
  Module      : マスタ出力定義登録ページ(MasterExportSettingRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MasterExportSettingRegister.aspx.cs" Inherits="Form_MasterExportSetting_MasterExportSettingRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
// CSVフォーマットフィールド追加
function add_field( fld )
{
	// CSVフォーマット入力内容取得
	var flds = document.getElementById('<%= tbFields.ClientID %>').value;
	
	if ( flds.charAt(flds.length -1) != ',' && flds.length > 0)
	{
		flds += ',\n' + fld;
	}
	else
	{
		flds += fld;
	}
	
	// CSVフォーマット入力内容設定
	document.getElementById('<%= tbFields.ClientID %>').value = flds;
}

// CSVフォーマットフィールド設定（全て選択）
function set_field_all()
{
	var flds = "";
<%
	foreach (RepeaterItem ri in rList.Items)
	{
%>
		var hfSettingName = document.getElementById( '<%= ((HiddenField)ri.FindControl("hfSettingName")).ClientID %>' );
		flds += '<%= (ri.ItemIndex != 0) ? ",\\n" : "" %>';
		flds += hfSettingName.value;
<%
	}
%>
	// CSVフォーマット入力内容設定
	document.getElementById('<%= tbFields.ClientID %>').value = flds;
}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">マスタダウンロード設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td colspan="2">
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
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />マスタ種別</td>
														<td class="search_item_bg" colspan="5">
															<asp:RadioButtonList id="rblMasterKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" Width="653" AutoPostBack="True" RepeatColumns="6" OnSelectedIndexChanged="rblMasterKbn_SelectedIndexChanged1"></asp:RadioButtonList></td>
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
		<td colspan="2"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧・詳細 ▽-->
	<tr>
		<td valign="top">
			<table class="box_border" cellspacing="1" cellpadding="3" width="375" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td valign="top" align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
											<td>
												<div id="divComp" runat="server" class="action_part_top" Visible="False">
												<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
													<tr class="info_item_bg">
														<td align="left">マスタ出力定義情報を登録/更新しました。
														</td>
													</tr>
												</table>
												</div>
												<div class="action_part_top">
													<asp:DropDownList id="ddlSelectSetting" AutoPostBack = "true" Width="200px" OnSelectedIndexChanged="ddlSelectSetting_SelectedIndexChanged" runat="server"></asp:DropDownList>
													<asp:Button id="btnDeleteTop" runat="server" Width="80px" Visible="False" Text="  削除する  " OnClick="btnDeleteTop_Click" OnClientClick= "return confirm('情報を削除してもよろしいですか？');"/>
													<asp:Button id="btnUpdateTop" runat="server" Width="80px" Visible="False" Text="  更新する  " OnClick="btnUpdateTop_Click" />
												</div>
												<hr />
												<div class="action_part_top">
													<asp:TextBox id="tbSettingName" align="left" Width="275px" runat="server"></asp:TextBox>
													<asp:Button id="btnRegisterTop" runat= "server" Width="80px" Visible="False" Text="  新規登録  " OnClick="btnRegisterTop_Click" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
													<tr>
														<td class="edit_title_bg" align="left" width="30%">出力ファイル形式<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList id="ddlExportFileType" runat="server"></asp:DropDownList></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">出力フォーマット<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbFields" runat="server" TextMode="MultiLine" Width="240" Height="680"></asp:TextBox></td>
													</tr>
												</table>
											</td>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									</table>
								</td>
								<td valign="top" align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
											<td>
												
												<div class="action_part_top"></div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
													<tr>
														<td class="edit_title_bg" align="left" width="30%">マスタ種別</td>
														<td class="edit_item_bg" align="left"><%= WebSanitizer.HtmlEncode(rblMasterKbn.SelectedItem.Text) %></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" colspan="2">フィールド</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left" colspan="2">
															<table class="edit_table" cellspacing="0" cellpadding="0" border="0" width="360">
																<tr>
																	<td class="edit_item_bg" align="right">
																		<input type="button" value="  全ての項目を一括選択  " onclick="javascript:set_field_all();" />
																	</td>
																</tr>
															</table>
															<div style="height: 743px; overflow-y: scroll;">
																<table>
																	<asp:repeater id="rList" runat="server">
																		<ItemTemplate>
																			<tr>
																				<td class="edit_item_bg" align="left">
																				<asp:HiddenField ID="hfSettingName" runat="server" Value='<%# WebSanitizer.UrlAttrHtmlEncode(((Hashtable)Container.DataItem)[Constants.MASTEREXPORTSETTING_XML_NAME]) %>' />
																				<a href="javascript:add_field('<%# WebSanitizer.UrlAttrHtmlEncode(((Hashtable)Container.DataItem)[Constants.MASTEREXPORTSETTING_XML_NAME]) %>');">
																				←&nbsp;
																				<%#: ((Hashtable)Container.DataItem)[Constants.MASTEREXPORTSETTING_XML_J_NAME] %></a>&nbsp;
																				(<%#: ((Hashtable)Container.DataItem)[Constants.MASTEREXPORTSETTING_XML_NAME] %>)
																				</td>
																			</tr>
																		</ItemTemplate>
																	</asp:repeater>
																</table>
															</div>
														</td>
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
	<!--△ 一覧・詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
