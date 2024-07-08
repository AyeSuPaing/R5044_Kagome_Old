<%--
=========================================================================================================
  Module      : 海外配送エリア確認ページ(GlobalShippingAreaConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="GlobalShippingAreaConfirm.aspx.cs" Inherits="Form_Global_Shipping_Area_Confirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">配送地域設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetailTop" runat="server" Visible="True">
		<td><h2 class="cmn-hed-h2">配送地域設定詳細</h2></td>
	</tr>
	<tr id="trConfirmTop" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送地域設定入力確認</h2></td>
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
												<div class="action_part_top">
													<input runat="server" id="btnHistoryBackTop" onclick="Javascript:history.back();" type="button" value="  戻る  " />
													<asp:Button ID="btnBackToListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnBackToListTop_Click" />
													<asp:Button id="btnEditTop" runat="server" Visible="False" Text="  編集する  " OnClick="btnEdit_Click" />
													<asp:Button id="btnInsertTop" runat="server" Visible="False" Text="  登録する  " OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Visible="False" Text="  更新する  " OnClick="btnUpdate_Click" /></div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送エリアID</td>
														<td class="detail_item_bg" align="left"><%#:this.m_areaData.GlobalShippingAreaId %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送エリア名</td>
														<td class="detail_item_bg" align="left"><%#:this.m_areaData.GlobalShippingAreaName %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">表示順</td>
														<td class="detail_item_bg" align="left"><%#:this.m_areaData.SortNo %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">有効フラグ</td>
														<td class="detail_item_bg" align="left"><%#: ValueText.GetValueText(Constants.TABLE_GLOBALSHIPPINGAREA, Constants.FIELD_GLOBALSHIPPINGAREA_VALID_FLG, this.m_areaData.ValidFlg) %></td>
													</tr>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">作成日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(this.m_areaData.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">更新日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(this.m_areaData.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">最終更新者</td>
														<td class="detail_item_bg" align="left"><%#:this.m_areaData.LastChanged %></td>
													</tr>
												</table>
												<br />
												<div class="action_part_bottom"></div>
											</td>
										</tr>
									</table>
									<br />
									<%if(this.KeepintActionStatus == Constants.ACTION_STATUS_DETAIL) { %>
									<table class="detail_table" cellspacing="0" cellpadding="0" border="0">
										<tr class="detail_title_bg">
											<td  align="center" colspan="<%:USE_EXTEND_COMPONENT_CONDITION ? 7 : 4 %>">配送エリア構成</td>
										</tr>
										<tr>
											<td align="center" width="15%" class="detail_title_bg">国</td>
											<td align="center" width="15%" class="detail_title_bg">住所5(State／Province)</td>
											<td align="center" width="15%" class="detail_title_bg">住所4(Suburb／City)</td>
											<% if (USE_EXTEND_COMPONENT_CONDITION) { %>
											<td align="center" width="15%" class="detail_title_bg">住所3</td>
											<td align="center" width="15%" class="detail_title_bg">住所2</td>
											<td align="center" width="15%" class="detail_title_bg">郵便番号</td>
											<% } %>
											<td align="center" width="10%" class="detail_title_bg">操作</td>
										</tr>
										<%-- 一覧用リピータ --%>
										<asp:Repeater id="repAreaComponent" Runat="server" ItemType="w2.Domain.GlobalShipping.GlobalShippingAreaComponentModel" OnItemCommand="repAreaComponent_ItemCommand">
											<ItemTemplate>
												<tr id="trItem">
													<td align="left" class="detail_item_bg"><%#:Item.CountryIsoCode %></td>
													<td align="left" class="detail_item_bg"><%#:string.IsNullOrEmpty(Item.ConditionAddr5) ? "全て" : Item.ConditionAddr5 %></td>
													<td align="left" class="detail_item_bg"><%#:string.IsNullOrEmpty(Item.ConditionAddr4) ? "全て" : Item.ConditionAddr4 %></td>
													<% if (USE_EXTEND_COMPONENT_CONDITION) { %>
													<td align="left" class="detail_item_bg"><%#:string.IsNullOrEmpty(Item.ConditionAddr3) ? "全て" : Item.ConditionAddr3 %></td>
													<td align="left" class="detail_item_bg"><%#:string.IsNullOrEmpty(Item.ConditionAddr2) ? "全て" : Item.ConditionAddr2 %></td>
													<td align="left" class="detail_item_bg"><%#:string.IsNullOrEmpty(Item.ConditionZip) ? "全て" : Item.ConditionZip %></td>
													<% } %>
													<td align="center" class="detail_item_bg"><asp:Button runat="server" ID="btnDelCondition" Text="削除" OnClientClick="return confirm('削除します。本当によろしいですか？')" CommandArgument="<%#:Item.Seq %>" CommandName="delCondition"/></td>
												</tr>
											</ItemTemplate>
										</asp:Repeater>
									</table>
									<br />
									<br />
									<table class="detail_table"  cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td class="detail_title_bg" align="center" colspan="<%:USE_EXTEND_COMPONENT_CONDITION ? 7 : 4 %>">構成条件の追加</td>
										</tr>
										<tr>
											<td align="center" width="15%" class="detail_title_bg">国</td>
											<td align="center" width="15%" class="detail_title_bg">住所5(State／Province)</td>
											<td align="center" width="15%" class="detail_title_bg">住所4(Suburb／City)</td>
											<% if (USE_EXTEND_COMPONENT_CONDITION) { %>
											<td align="center" width="15%" class="detail_title_bg">住所3</td>
											<td align="center" width="15%" class="detail_title_bg">住所2</td>
											<td align="center" width="15%" class="detail_title_bg">郵郵便番号</td>
											<% } %>
											<td align="center" width="10%" class="detail_title_bg">操作</td>
										</tr>
										<%-- 一覧用リピータ --%>
										<tr id="trItem" >
											<td align="left" class="detail_item_bg"><asp:DropDownList runat="server" ID="ddlConditionCountry" OnSelectedIndexChanged="ddlConditionCountry_SelectedIndexChanged" AutoPostBack="true"/></td>
											<td align="left" class="detail_item_bg"><asp:TextBox runat="server" ID="tbConditionAddr5" Visible="true" /><asp:DropDownList runat="server" ID="ddlUsState" visible="false"/></td>
											<td align="left" class="detail_item_bg"><asp:TextBox runat="server" ID="tbConditionAddr4" /></td>
											<% if (USE_EXTEND_COMPONENT_CONDITION) { %>
											<td align="left" class="detail_item_bg"><asp:TextBox runat="server" ID="tbConditionAddr3" /></td>
											<td align="left" class="detail_item_bg"><asp:TextBox runat="server" ID="tbConditionAddr2" /></td>
											<td align="left" class="detail_item_bg"><asp:TextBox runat="server" ID="tbConditionZip" /></td>
											<% } %>
											<td align="center" class="detail_item_bg"><asp:Button runat="server" ID="btnAddCondition" Text="追加" OnClick="btnAddCondition_Click" /></td>
										</tr>
									</table>
									<br />
									<table id="note" class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
										<tr>
											<td align="left" class="info_item_bg" colspan="2">■ 構成条件の追加について<br />
												空白の場合、全てを対象とします。<br />
												入力した条件はすべて完全一致となります。<br />
												大文字小文字は無視して比較を行います。<br />
												半角スペースは無視して比較を行います。<br />
												例：New Mexico ＝ Newmexico<br />
												住所に対して一致する構成条件が複数ある場合は、後から追加した構成条件に該当するエリアを正とします。<br />
											</td>
										</tr>
									</table>
									<% } %>
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
	
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
