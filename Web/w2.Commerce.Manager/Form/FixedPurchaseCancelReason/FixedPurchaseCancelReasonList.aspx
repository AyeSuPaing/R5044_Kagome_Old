<%--
=========================================================================================================
  Module      : 定期解約理由区分設定一覧ページ (FixedPurchaseCancelReasonList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="FixedPurchaseCancelReasonList.aspx.cs" Inherits="Form_FixedPurchaseCancelReason_FixedPurchaseCancelReasonList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">定期解約理由区分設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" width="100" border="0" alt="" src="../../Images/Common/sp.gif" /></td>
	</tr>
	<tr>
		<td><h2 class="cmn-hed-h2">定期解約理由区分設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div>
									<%-- UPDATE PANEL開始 --%>
									<asp:UpdatePanel ID="upUpdatePanel" runat="server">
									<ContentTemplate>
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="5" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr id="trMessages" runat="server" visible="false">
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<br />
														<br />
														<br />
														<td align="left"><asp:Label ID="lMessages" runat="server"></asp:Label></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="5" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr>
											<td align="right">
												<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_OnClick">翻訳設定出力</asp:LinkButton>
												<% } %>
												<asp:Button ID="btnAddTop" runat="server" Text="&nbsp;&nbsp;追加&nbsp;&nbsp;" OnClick="btnAdd_Click" />&nbsp;
												<asp:Button ID="btnAllUpdateTop" runat="server" Text="&nbsp;&nbsp;一括更新&nbsp;&nbsp;" OnClick="btnAllUpdate_Click" OnClientClick="return check_confirm();" />
											</td>
										</tr>
										<tr>
											<td><img height="5" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr>
											<td valign="top">
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0" align="center">
														<tr class="list_title_bg">
															<td align="center" colspan="6">定期解約理由区分設定項目</td>
														</tr>
														<tr class="list_title_bg">
															<td width="38" align="center">No</td>
															<td width="220" align="left">解約理由区分ID</td>
															<td width="255" align="left">解約理由区分名</td>
															<td width="60" align="center">表示順</td>
															<td width="100" align="center">表示範囲</td>
															<td width="60" align="center">削除</td>
														</tr>
														<asp:Repeater id="rCancelReasonList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																<td align="center">
																	<%# Container.ItemIndex + 1 %>
																</td>
																<td align="left">
																	<asp:Label ID="lbCancelReasonId" runat="server" Text="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).CancelReasonId %>" Visible="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).IsNew == false %>" />
																	<div runat="server" Visible="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).IsNew %>">
																		<asp:TextBox ID="tbCancelReasonId" runat="server" Width="200" MaxLength="30" Text="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).CancelReasonId %>" /></div>
																</td>
																<td align="left">
																	<asp:TextBox ID="tbCancelReasonName" runat="server" Width="230" MaxLength="100" Text="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).CancelReasonName %>" />

																	<%-- 解約理由区分名翻訳設定情報 --%>
																	<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																	<asp:Repeater ID="rTranslationCancelReasonName" runat="server"
																		 DataSource="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).CancelReasonNameTranslationData %>"
																		 ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
																	<ItemTemplate>
																		<div><%#: Item.LanguageCode %>(<%#: Item.LanguageLocaleId %>)：<%#: Item.AfterTranslationalName %></div>
																	</ItemTemplate>
																	</asp:Repeater>
																	<% } %>

																</td>
																<td align="left">
																	<asp:TextBox ID="tbDisplayOrder" runat="server" Width="40" MaxLength="7" Text='<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).DisplayOrder %>' />
																</td>
																<td align="left">
																	<asp:CheckBox id="cbPcDisplayFlg" runat="server" Checked="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).IsValidDisplayKbnPc %>" Text="&nbsp;&nbsp;PC/スマフォ" /><br/>
																	<asp:CheckBox id="cbEcDisplayFlg" runat="server" Checked="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).IsValidDisplayKbnEc %>" Text="&nbsp;&nbsp;EC管理" />
																</td>
																<td align="center">
																	<asp:CheckBox id="cbDeleteFlg" runat="server" Checked="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).IsDelete %>" 
																				Visible="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).IsNew == false %>" />
																	<asp:LinkButton ID="btnCancel" runat="server" Text="キャンセル" OnClick="btnCancel_Click" CommandArgument="<%# Container.ItemIndex %>" 
																				Visible="<%# ((FixedPurchaseCancelReasonInput)Container.DataItem).IsNew %>" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="6"></td>
													</tr>
												</table>
											</td>
											<td><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
										</tr>
										<tr>
											<td><img height="5" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr>
											<td align="right">
												<asp:Button ID="btnAddBottom" runat="server" Text="&nbsp;&nbsp;追加&nbsp;&nbsp;" OnClick="btnAdd_Click" />&nbsp;
												<asp:Button ID="btnAllUpdateBottom" runat="server" Text="&nbsp;&nbsp;一括更新&nbsp;&nbsp;" OnClick="btnAllUpdate_Click" OnClientClick="return check_confirm();" />
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															・既に定期購入情報で解約理由区分を利用している場合、削除することはできません。<br />
															　利用しない場合は、表示範囲を全て未チェックにしてください。<br />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									</table>
									</ContentTemplate>
									</asp:UpdatePanel>
									<%-- UPDATE PANELここまで --%>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr><td><img height="10" width="1" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
</table>
<script type="text/javascript">
<!--
	//=============================================================================================
	// 更新確認ダイアログ生成
	//=============================================================================================
	function check_confirm() {
		return confirm("表示内容で更新します。\nよろしいですか？");
	}
//-->
</script>
</asp:Content>