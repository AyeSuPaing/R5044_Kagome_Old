<%--
=========================================================================================================
  Module      : 広告コード閲覧権限ページ(AdvertisementCodeAuthorityRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AdvertisementCodeAuthorityRegister.aspx.cs" Inherits="Form_AdvertisementCodeAuthority_AdvertisementCodeAuthorityRegister" %>
<%@ Import Namespace="w2.Domain.AdvCode.Helper" %>
<%@ Import Namespace="w2.Domain.ShopOperator" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td>
				<h1 class="page-title">広告コード閲覧権限</h1>
			</td>
		</tr>
		<tr>
			<td>
				<h2 class="cmn-hed-h2">広告コード閲覧権限設定</h2>
			</td>
		</tr>
		<tr>
			<td>
				<div id="divComp" runat="server" class="action_part_top" Visible="False">
					<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
						<tr class="info_item_bg">
							<td align="left">閲覧可能な広告コードを更新しました。
							</td>
						</tr>
					</table>
				</div>
			</td>
		</tr>
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
					<tr>
						<td class="list_box_bg">
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="675">
																<asp:Label ID="lbPager" runat="server"></asp:Label></td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<tr>
												<td>
													<img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table cellspacing="1" cellpadding="3" width="758" class="list_table" border="0">
														<tr class="list_title_bg">
															<td align="center" width="15%"></td>
															<td align="center" width="15%">オペレータID</td>
															<td align="center" width="50%">オペレータ名</td>
														</tr>
														<asp:Repeater ID="rOperatorList" runat="server">
															<ItemTemplate>
																<tr class="list_item_bg1">
																	<td align="center">
																		<asp:RadioButton ID="rbOperator" AutoPostBack="true" runat="server" OnCheckedChanged="rbOperator_CheckedChanged" />
																		<asp:HiddenField ID="hfOperatorId" Value='<%#: ((ShopOperatorModel)Container.DataItem).OperatorId%>' runat="server" />
																		<asp:HiddenField ID="hfOperatorName" Value='<%#: ((ShopOperatorModel)Container.DataItem).Name%>' runat="server" />
																	</td>
																	<td align="center"><%#: ((ShopOperatorModel)Container.DataItem).OperatorId %></td>
																	<td align="left">&nbsp;<%#: ((ShopOperatorModel)Container.DataItem).Name %></td>
																</tr>
															</ItemTemplate>
															<AlternatingItemTemplate>
																<tr class="list_item_bg2">
																	<td align="center">
																		<asp:RadioButton ID="rbOperator" AutoPostBack="true" runat="server" OnCheckedChanged="rbOperator_CheckedChanged" />
																		<asp:HiddenField ID="hfOperatorId" Value='<%#: ((ShopOperatorModel)Container.DataItem).OperatorId%>' runat="server" />
																		<asp:HiddenField ID="hfOperatorName" Value='<%#: ((ShopOperatorModel)Container.DataItem).Name%>' runat="server" />
																	</td>
																	<td align="center"><%#: ((ShopOperatorModel)Container.DataItem).OperatorId %></td>
																	<td align="left">&nbsp;<%#: ((ShopOperatorModel)Container.DataItem).Name %></td>
																</tr>
															</AlternatingItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" visible="false">
															<td id="tdErrorMessage" runat="server" colspan="4"></td>
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
				</table>
			</td>
		</tr>
	<!--△ 検索 △-->
		<tr>
			<td colspan="2">
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
												<td>
													<img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
												<td>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
														<tr>
															<td>
																オペレータ名：<asp:Literal ID="lOperatorName" runat="server"></asp:Literal><asp:HiddenField ID="hfOperatorId" runat="server"/>
															</td>
															<td width="83" class="action_list_sp">
																<asp:Button ID="btnUpdate" runat="server" Text="  更新する  " OnClick="btnUpdate_OnClick"></asp:Button></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">閲覧可能な広告コード<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbAdAuthorityFields" runat="server" TextMode="MultiLine" Width="240" Height="680"></asp:TextBox></td>
														</tr>
													</table>
												</td>
												<td>
													<img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
											</tr>
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</td>
									<td valign="top" align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
												<td>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
														<tr>
															<td class="search_title_bg" width="30%" align="center">広告コード</td>
															<td class="search_item_bg" width="70%" align="center">
																<asp:TextBox ID="tbSearchAdvertisementCode" Width="300" runat="server"></asp:TextBox>
																<asp:Button ID="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click"></asp:Button>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" colspan="3">広告コード一覧</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="3">
																<table class="edit_table" cellspacing="0" cellpadding="0" border="0" width="360">
																	<tr>
																		<td class="edit_item_bg" align="right">
																			<input type="button" value="  全ての広告コードを一括選択  " onclick="javascript: set_field_all();" class="cmn-btn-sub-action" />
																		</td>
																	</tr>
																</table>
																<div style="height: 590px; overflow-y: scroll;">
																	<table>
																		<asp:Repeater ID="rAdvCodeList" ItemType="w2.Domain.AdvCode.Helper.AdvCodeListSearchResult" runat="server">
																			<ItemTemplate>
																				<tr>
																					<td class="edit_item_bg" align="left">
																						<asp:HiddenField ID="hfAdvertisementCode" runat="server" Value='<%#: ((AdvCodeListSearchResult)Container.DataItem).AdvertisementCode %>' />
																						<a href="javascript:add_field('<%#: ((AdvCodeListSearchResult)Container.DataItem).AdvertisementCode %>');">←&nbsp;
																							<%#: ((AdvCodeListSearchResult)Container.DataItem).AdvertisementCode %></a>&nbsp;
																						(<%#: ((AdvCodeListSearchResult)Container.DataItem).MediaName %>)
																					</td>
																				</tr>
																			</ItemTemplate>
																		</asp:Repeater>
																		<tr id="trAdvCodeListError" class="list_alert" runat="server" Visible="False">
																			<td id="tdAdvCodeErrorMessage" colspan="8" runat="server"></td>
																		</tr>
																	</table>
																</div>
															</td>
														</tr>
													</table>
												</td>
												<td>
													<img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
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
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<tr>
			<td>
				<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
					<tr class="info_item_bg">
						<td align="left">備考<br />
							・閲覧可能な広告コードが設定されていないオペレータはすべての広告コードを閲覧することができます。
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
	</table>
<script type="text/javascript">
<!--
	// 広告コード追加
	function add_field(fld) {
		var flds = document.getElementById('<%= tbAdAuthorityFields.ClientID %>').value;

	if ((flds.charAt(flds.length - 1) !== ',') && (flds.length > 0)) {
		flds += ',\n' + fld;
	}else {
		flds += fld;
	}

	document.getElementById('<%= tbAdAuthorityFields.ClientID %>').value = flds;
}

	// 全て広告コード追加
	function set_field_all() {
		var flds = "";
	<%
		foreach (RepeaterItem ri in rAdvCodeList.Items)
		{
	%>
		var hfSettingName = document.getElementById('<%= ((HiddenField)ri.FindControl("hfAdvertisementCode")).ClientID %>');
		flds += '<%= (ri.ItemIndex != 0) ? ",\\n" : "" %>';
		flds += hfSettingName.value;
	<%
		}
	%>

		document.getElementById('<%= tbAdAuthorityFields.ClientID %>').value = flds;
	}
//-->
</script>
</asp:Content>
