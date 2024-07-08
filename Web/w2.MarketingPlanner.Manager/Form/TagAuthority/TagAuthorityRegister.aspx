<%--
=========================================================================================================
  Module      : タグ閲覧権限ページ(TagAuthorityRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="TagAuthorityRegister.aspx.cs" Inherits="Form_TagAuthority_TagAuthorityRegister" %>
<%@ Import Namespace="w2.Domain.ShopOperator" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td>
				<h1 class="page-title">タグ閲覧権限</h1>
			</td>
		</tr>
		<tr>
			<td>
				<h2 class="cmn-hed-h2">タグ閲覧権限設定</h2>
			</td>
		</tr>
		<tr>
			<td>
				<div id="divComp" runat="server" class="action_part_top" Visible="False">
					<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
						<tr class="info_item_bg">
							<td align="left">
								閲覧可能な<asp:Literal ID="lCompKbnText" runat="server" /> を更新しました。
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
																<asp:Literal ID="lbPager" runat="server" />
															</td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<tr>
												<td>
													<img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
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
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<!--▽ 一覧・詳細 ▽-->
		<tr>
			<td valign="top">
				<asp:HiddenField ID="hfOperatorId" runat="server" />
				<asp:UpdatePanel ID="upAuthorities" runat="server">
					<ContentTemplate>
						<%-- 検索隠しボタン --%>
						<asp:Button ID="btnSearch" Text="  検索  " OnClick="btnSearch_Click" style="display: none;" runat="server" />
						<asp:HiddenField ID="hfDifferential" Value="False" runat="server" />

						<table class="box_border" cellspacing="1" cellpadding="3" width="375" border="0">
							<tr>
								<td>
									<div class="tabs-wrapper">
										<asp:LinkButton
											ID="lbSwitchTagAuthority"
											Text="<%#: Constants.FLG_TAG_AUTHORITY_TEXT_TAG_ID %>"
											CommandArgument="<%# Constants.FLG_TAG_AUTHORITY_KBN_TAG %>"
											OnClick="lbSwitch_OnClick"
											OnClientClick="return alertNoSave();"
											runat="server" />
										<asp:LinkButton
											ID="lbSwitchMediaTypeAuthority"
											Text="<%#: Constants.FLG_TAG_AUTHORITY_TEXT_MEDIA_TYPE %>"
											CommandArgument="<%# Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE %>"
											OnClick="lbSwitch_OnClick"
											OnClientClick="return alertNoSave();"
											runat="server" />
										<asp:LinkButton
											ID="lbSwitchLocationAuthority"
											Text="<%#: Constants.FLG_TAG_AUTHORITY_TEXT_LOCATION %>"
											CommandArgument="<%# Constants.FLG_TAG_AUTHORITY_KBN_LOCATION %>"
											OnClick="lbSwitch_OnClick"
											OnClientClick="return alertNoSave();"
											runat="server" />
									</div>
								</td>
							</tr>
							<tr>
								<td class="tab-contents">
									<table id="tblTagAuthority" class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0" Visible="<%# (this.AuthorityKbn == Constants.FLG_TAG_AUTHORITY_KBN_TAG) %>" runat="server">
										<tr>
											<td valign="top" align="center">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
																<tr>
																	<td>
																		オペレータ名：<%: this.OperatorName %>
																	</td>
																	<td width="83" class="action_list_sp">
																		<asp:Button ID="btnUpdateTagAuthority" Text="  更新する  " OnClick="btnUpdate_OnClick" runat="server" />
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">閲覧可能なタグID<span class="notice">*</span></td>
																	<td class="edit_item_bg" align="left">
																		<table class="edit_table" cellspacing="0" cellpadding="0" border="0" width="360">
																			<tr>
																				<td class="edit_item_bg" align="left" width="500">
																					<asp:TextBox ID="tbSearchTagId" Width="300" placeholder="タグID / タグ名" onkeypress="search()" runat="server" />
																					<input type="button" value="  検索  " onclick="search()" />
																				</td>
																				<td class="edit_item_bg" align="right">
																					<asp:Button
																						ID="btnSelectAllForTagId"
																						Text="  全てのタグIDを一括選択  "
																						CssClass="cmn-btn-sub-action"
																						OnClick="btnAllChange_OnClick"
																						CommandArgument="<%# Constants.FLG_TAG_AUTHORITY_ISCHECKED_VALID %>"
																						runat="server" />
																					<asp:Button
																						ID="btnClearAllForTagId"
																						Text="  全てのタグIDを一括解除  "
																						CssClass="cmn-btn-sub-action"
																						OnClick="btnAllChange_OnClick"
																						CommandArgument="<%# Constants.FLG_TAG_AUTHORITY_ISCHECKED_INVALID %>"
																						runat="server" />
																				</td>
																			</tr>
																		</table>
																		<div style="height: 590px; overflow-y: scroll;">
																			<table>
																				<asp:Repeater ID="rTagIdList" ItemType="w2.Domain.Affiliate.AffiliateTagSettingModel" runat="server">
																					<ItemTemplate>
																						<tr>
																							<td class="edit_item_bg" align="left">
																								<asp:HiddenField ID="hfValue" Value='<%# Item.AffiliateId %>' runat="server" />
																								<asp:CheckBox ID="cbIsSelected" AutoPostBack="True" OnCheckedChanged="cbIsSelected_OnCheckedChanged" runat="server" />
																								<strong><%#: Item.AffiliateId %></strong>
																								&nbsp;
																								(<%#: Item.AffiliateName %>)
																							</td>
																						</tr>
																					</ItemTemplate>
																				</asp:Repeater>
																				<tr id="trTagListError" class="list_alert" runat="server" Visible="False">
																					<td id="tdTagErrorMessage" colspan="8" runat="server"></td>
																				</tr>
																			</table>
																		</div>
																	</td>
																</tr>
															</table>
														</td>
														<td>
															<img height="6" alt="" src="../../Images/Common/sp.gif" border="0" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
									<table id="tblMediaTypeAuthority" class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0" Visible="<%# (this.AuthorityKbn == Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE) %>" runat="server">
										<tr>
											<td valign="top" align="center">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
																<tr>
																	<td>
																		オペレータ名：<%: this.OperatorName %>
																	</td>
																	<td width="83" class="action_list_sp">
																		<asp:Button ID="btnUpdateMediaTypeAuthority" Text="  更新する  " OnClick="btnUpdate_OnClick" runat="server" />
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">閲覧可能な広告媒体区分<span class="notice">*</span></td>
																	<td class="edit_item_bg" align="left">
																		<table class="edit_table" cellspacing="0" cellpadding="0" border="0" width="360">
																			<tr>
																				<td class="edit_item_bg" align="left" width="500">
																					<asp:TextBox ID="tbSearchMediaTypeId" Width="300" placeholder="広告媒体区分ID / 広告媒体区分名" onkeypress="search()" runat="server" />
																					<input type="button" value="  検索  " onclick="search()" />
																				</td>
																				<td class="edit_item_bg" align="right">
																					<asp:Button
																						ID="btnSelectAllForMediaType"
																						Text="  全ての広告媒体区分を一括選択  "
																						CssClass="cmn-btn-sub-action"
																						OnClick="btnAllChange_OnClick"
																						CommandArgument="<%# Constants.FLG_TAG_AUTHORITY_ISCHECKED_VALID %>"
																						runat="server" />
																					<asp:Button
																						ID="btnClearAllForMediaType"
																						Text="  全ての広告媒体区分を一括解除  "
																						CssClass="cmn-btn-sub-action"
																						OnClick="btnAllChange_OnClick"
																						CommandArgument="<%# Constants.FLG_TAG_AUTHORITY_ISCHECKED_INVALID %>"
																						runat="server" />
																				</td>
																			</tr>
																		</table>
																		<div style="height: 590px; overflow-y: scroll;">
																			<table>
																				<asp:Repeater ID="rMediaTypeList" ItemType="w2.Domain.AdvCode.AdvCodeMediaTypeModel" runat="server">
																					<ItemTemplate>
																						<tr>
																							<td class="edit_item_bg" align="left">
																								<asp:HiddenField ID="hfValue" Value='<%# Item.AdvcodeMediaTypeId %>' runat="server" />
																								<asp:CheckBox ID="cbIsSelected" AutoPostBack="True" OnCheckedChanged="cbIsSelected_OnCheckedChanged" runat="server" />
																								<strong><%#: Item.AdvcodeMediaTypeId %></strong>
																								&nbsp;
																								(<%#: Item.AdvcodeMediaTypeName %>)
																							</td>
																						</tr>
																					</ItemTemplate>
																				</asp:Repeater>
																				<tr id="trMediaTypeListError" class="list_alert" runat="server" Visible="False">
																					<td id="tdMediaTypeErrorMessage" colspan="8" runat="server"></td>
																				</tr>
																			</table>
																		</div>
																	</td>
																</tr>
															</table>
														</td>
														<td>
															<img height="6" alt="" src="../../Images/Common/sp.gif" border="0" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
									<table id="tblLocationAuthority" class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0" Visible="<%# (this.AuthorityKbn == Constants.FLG_TAG_AUTHORITY_KBN_LOCATION) %>" runat="server">
										<tr>
											<td valign="top" align="center">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
																<tr>
																	<td>
																		オペレータ名：<%: this.OperatorName %>
																	</td>
																	<td width="83" class="action_list_sp">
																		<asp:Button ID="btnUpdateLocationAuthority" Text="  更新する  " OnClick="btnUpdate_OnClick" runat="server" />
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">閲覧可能な設置箇所<span class="notice">*</span></td>
																	<td class="edit_item_bg" align="left">
																		<table class="edit_table" cellspacing="0" cellpadding="0" border="0" width="360">
																			<tr>
																				<td class="edit_item_bg" align="right">
																					<asp:Button
																						ID="btnSelectAllForLocation"
																						Text="  全ての設置箇所を一括選択  "
																						CssClass="cmn-btn-sub-action"
																						OnClick="btnAllChange_OnClick"
																						CommandArgument="<%# Constants.FLG_TAG_AUTHORITY_ISCHECKED_VALID %>"
																						runat="server" />
																					<asp:Button
																						ID="btnClearAllForLocation"
																						Text="  全ての設置箇所を一括解除  "
																						CssClass="cmn-btn-sub-action"
																						OnClick="btnAllChange_OnClick"
																						CommandArgument="<%# Constants.FLG_TAG_AUTHORITY_ISCHECKED_INVALID %>"
																						runat="server" />
																				</td>
																			</tr>
																		</table>
																		<div style="height: 590px; overflow-y: scroll;">
																			<table>
																				<asp:Repeater ID="rLocationList" ItemType="w2.App.Common.Affiliate.TargetPage" runat="server">
																					<ItemTemplate>
																						<tr>
																							<td class="edit_item_bg" align="left">
																								<asp:HiddenField ID="hfValue" Value='<%# Item.Path %>' runat="server" />
																								<asp:CheckBox ID="cbIsSelected" AutoPostBack="True" OnCheckedChanged="cbIsSelected_OnCheckedChanged" runat="server" />
																								<strong><%#: Item.Path %></strong>
																								&nbsp;
																								(<%#: Item.Name %>)
																							</td>
																						</tr>
																					</ItemTemplate>
																				</asp:Repeater>
																			</table>
																		</div>
																	</td>
																</tr>
															</table>
														</td>
														<td>
															<img height="6" alt="" src="../../Images/Common/sp.gif" border="0" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
		</tr>
		<!--△ 一覧・詳細 △-->
	
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<tr>
			<td>
				<asp:UpdatePanel ID="upRemarks" runat="server">
					<ContentTemplate>
						<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
							<tr class="info_item_bg">
								<td align="left">備考<br />
									<% if (this.AuthorityKbn == Constants.FLG_TAG_AUTHORITY_KBN_TAG) { %>

									・タグIDが空のオペレータは、アフィリエイトレポートのアフィリエイト情報を全て閲覧可能となります。<br/>
									・タグIDが空ではないオペレータは、設定したアフィリエイトレポートのアフィリエイト情報のみ閲覧可能となります。

									<% } else if (this.AuthorityKbn == Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE) { %>

									・広告媒体区分が空のオペレータは、タグIDのタブにて設定した権限に準ずる編集権限となります。<br />
									・広告媒体区分が空でないオペレータは、タグIDのタブにて設定したIDに加えて設定した広告媒体区分が編集可能となります。

									<% } else if (this.AuthorityKbn == Constants.FLG_TAG_AUTHORITY_KBN_LOCATION) { %>

									・設置箇所が空のオペレータは、タグIDのタブにて設定した権限に準ずる編集権限となります。<br />
									・設置箇所が空でないオペレータは、タグIDのタブにて設定したIDに加えて設定した設置箇所が編集可能となります。

									<% } %>
								</td>
							</tr>
						</table>
					</ContentTemplate>
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="lbSwitchTagAuthority" />
						<asp:AsyncPostBackTrigger ControlID="lbSwitchMediaTypeAuthority" />
						<asp:AsyncPostBackTrigger ControlID="lbSwitchLocationAuthority" />
					</Triggers>
				</asp:UpdatePanel>
			</td>
		</tr>
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
	</table>

	<script>
		// 検索隠しボタンイベント
		function search() {
			// キーが押された かつ 押されたキーが「enter」以外の場合は検索しない
			if (window.event.type === 'keypress' && (window.event.keyCode !== 13)){
				return;
			}

			var btnSearch = document.getElementById('<%: btnSearch.ClientID %>');
			btnSearch.click();
		}

		// タブ切り替え時警告
		function alertNoSave() {
			var hfDifferential = document.getElementById('<%: hfDifferential.ClientID %>');
			var isChange = (hfDifferential.value.toLowerCase() === 'true');
			if (isChange === false) return true;

			return confirm('変更内容が保存されていません。\nタブを切り替えてよろしいですか？');
		}
	</script>
</asp:Content>