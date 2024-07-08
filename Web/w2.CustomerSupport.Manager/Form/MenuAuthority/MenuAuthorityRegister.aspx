<%--
=========================================================================================================
  Module      : メニュー権限設定登録ページ(MenuAuthorityRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MenuAuthorityRegister.aspx.cs" Inherits="Form_MenuAuthority_MenuAuthorityRegister" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Import Namespace="w2.Domain.MenuAuthority" %>
<%@ Import Namespace="w2.App.Common.Manager.Menu" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">メニュー権限設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 入力 ▽-->
	<tr>
		<td>
			<h2 class="cmn-hed-h2">
				<% if (this.ActionStatus == Constants.ACTION_STATUS_INSERT){%>メニュー権限設定登録<%} %>
				<% if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE){%>メニュー権限設定編集<%} %>
			</h2>
		</td>
	</tr>
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
												<div class="action_part_top"><input onclick="Javascript:history.back()" type="button" value="  戻る  " />
												<asp:Button id="btnConfirm" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" /></div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" width="30%">メニュー権限名
														</td>
														<td class="edit_item_bg" align="left" width="70%">
															<asp:TextBox id="tbMenuAuthorityName" Runat="server" Width="400" MaxLength="20"></asp:TextBox></td>
													</tr>
												</table>
												<div class="multi_check"><input type="checkbox" name="checkAll" id="checkAll" value="checkAll"> 一括選択</div>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<asp:Repeater id="rLargeMenu" ItemType="w2.App.Common.Manager.Menu.MenuLarge" Runat="server">
													<ItemTemplate>
														<tr>
															<td align="left" class="edit_title_bg" colspan="3">
																<%#: Item.Name %>
															</td>
														</tr>
														<asp:Repeater id="rSmallMenu" Runat="server" DataSource='<%# Item.SmallMenus %>' ItemType="w2.App.Common.Manager.Menu.MenuSmall">
														<ItemTemplate>
															<tr>
																<td align="center" class="edit_title_bg" width="10%">
																	<asp:CheckBox ID="cbMenuCheck" Runat="server" Checked='<%# m_menuAuthorityInfoList.ContainsKey(Item.MenuPath) %>'></asp:CheckBox>
																	<input type="radio" name="rbDefaultPage" value='<%# Item.MenuPath %>' <%# m_menuAuthorityInfoList.ContainsKey(Item.MenuPath) ? ((m_menuAuthorityInfoList[Item.MenuPath]).IsDefaultDispOn ? "checked" : "") : "" %> />
																	<asp:HiddenField id="hfMenuPath" Value='<%# Item.MenuPath %>' Runat="server" />
																</td>
																<td align="left" class="edit_item_bg" width="20%"><%#: Item.Name %></td>
																<td align="left" class="edit_item_bg" width="70%">
																	<asp:Repeater ID="rFunction" runat="server" DataSource='<%# Item.Functions %>' ItemType="w2.App.Common.Manager.Menu.MenuFunction">
																	<ItemTemplate>
																		<asp:CheckBox ID="cbCheck" runat="server" Text="<%# Item.Name %>" Checked='<%# m_menuAuthorityInfoList.ContainsKey(((MenuSmall)((RepeaterItem)((Repeater)Container.Parent).Parent).DataItem).MenuPath) && (((int.Parse((m_menuAuthorityInfoList[((MenuSmall)((RepeaterItem)((Repeater)Container.Parent).Parent).DataItem).MenuPath]).FunctionLevel.ToString()) & Item.Level) != 0)) %>' />
																		<asp:HiddenField ID="hfCheck" runat="server" Value="<%# Item.Level %>" />
																	</ItemTemplate>
																	</asp:Repeater>
																</td>
															</tr>
														</ItemTemplate>
														</asp:Repeater>
													</ItemTemplate>
													</asp:Repeater>
												</table>
												<br />
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">
														    備考<br />
															　権限用有効/無効はチェックボックスで、ログイン後デフォルト表示するページはラジオボタンで指定してください。
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnConfirm2" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
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
	<!--△ 入力 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
@section JavaScript
{
<script type="text/javascript">
	// 一括選択
	const checkAll = document.getElementById('checkAll');
	checkAll.addEventListener('click', toggle);
	function toggle(){
		const isChecked = checkAll.checked;
		Array.from(document.getElementsByTagName('input')).forEach(element =>{
			element.checked = isChecked; 
		});
		var radios = document.querySelectorAll('input[type="radio"]');
		if(isChecked) {
			radios[0].checked = isChecked;
		}
	}
</script>
}
</asp:Content>
