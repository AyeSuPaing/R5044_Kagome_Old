
<%--
=========================================================================================================
  Module      : 更新履歴情報情報確認ページ(UpdateHistoryConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.UpdateHistory.Helper" %>
<%@ Import Namespace="w2.Domain.UpdateHistory.Setting" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="UpdateHistoryConfirm.aspx.cs" Inherits="Form_UpdateHistory_UpdateHistoryConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="860" border="0">
	<!--▽ 詳細 ▽-->
	<tr><td><h1 class="cmn-hed-h2">更新履歴一覧</h1></td></tr>
	<tr>
		<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
	<td>
		<table class="box_border" cellspacing="1" cellpadding="3" width="860" border="0">
		<tr>
		<td>
			<!--▽ 更新履歴検索 ▽-->
			<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr>
			<td align="center" valign="top">
				<div style="margin:0px 0px 5px 0px;"></div>
				<table cellspacing="0" cellpadding="0" border="0">
				<tr valign="top">
				<td>
					<table class="list_table" cellspacing="1" cellpadding="3" border="0" width="840">
						<tr class="list_title_bg">
							<td align="center" colspan="5">更新履歴検索</td>
						</tr>
						<tr>
						<td class="list_title_bg" align="left" width="130">
							<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />マスタ</td>
						<td class="list_item_bg1" align="left" width="250">
							<asp:CheckBoxList ID="cblMaster" runat="server" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="checkBox" />
						</td>
						<td class="list_title_bg" align="left" width="130">
							<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />マスタID</td>
						<td class="list_item_bg1" align="left" width="250">
							<asp:TextBox ID="tbMasterId" runat="server" Width="200"></asp:TextBox>
						</td>
						<td class="list_title_bg" align="center" width="75">
							<div class="search_btn_main"><asp:Button ID="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
							<div class="search_btn_sub">
								<a href="<%= CreateUpdateHistoryConfirmUrl("", this.RequestUserId, "") %>">クリア</a>
								<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
							</div>
						</td>
						</tr>
						<tr class="list_item_bg1">
						<td colspan="5">
							<div style="height:<%= (rSearchUpdateHistoryList.Items.Count <= 4) ? 70 + (rSearchUpdateHistoryList.Items.Count * 30) : 190 %>px; width: 100%; overflow-y:scroll; float: left;">
							<asp:UpdatePanel runat="server">
							<ContentTemplate>
							<table class="list_table" cellspacing="1" cellpadding="0" border="0">
								<tr class="list_title_bg">
									<td align="center" width="60">詳細表示</td>
									<td align="center" width="180">更新日時</td>
									<td align="center" width="250">更新区分</td>
									<td align="center" width="150">マスタID</td>
									<td align="center" width="150">更新者</td>
								</tr>
								<tr id="trDisplayUpdateHistoryListError" class="list_alert" runat="server" Visible="true">
									<td id="tdDisplayUpdateHistoryListErrorMessage" runat="server" colspan="5">
										<%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %>
									</td>
								</tr>
								<asp:Repeater ID="rSearchUpdateHistoryList" runat="server" ItemType="UpdateHistorySearchInput">
								<ItemTemplate>
									<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
										<td align="center">
											<asp:CheckBox ID="cbDisplayDetailUpdateHistory" OnCheckedChanged="cbDisplayDetailUpdateHistory_CheckedChanged" AutoPostBack="true" runat="server" Text="　"  />
											<%-- 隠しフィールド --%>
											<asp:HiddenField ID="hfUpdateHhistoryNo" runat="server" Value="<%# Item.UpdateHistoryNo %>" />
											<asp:HiddenField ID="hfUpdateHistoryKbn" runat="server" Value="<%# Item.UpdateHistoryKbn %>" />
											<asp:HiddenField ID="hfUserId" runat="server" Value="<%# Item.UserId %>" />
											<asp:HiddenField ID="hfMasterId" runat="server" Value="<%# Item.MasterId %>" />
										</td>
										<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
										<td align="left"><%#: Item.UpdateHistoryAction %></td>
										<td align="left"><%#: Item.MasterId %></td>
										<td align="left"><%#: Item.LastChanged %></td>
									</tr>
								</ItemTemplate>
								</asp:Repeater>
							</table>
							</ContentTemplate>
							</asp:UpdatePanel>
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
			<!--△ 更新履歴検索 △-->
			<asp:UpdatePanel runat="server">
			<ContentTemplate>
			<!--▽ 各マスタ詳細 ▽-->
			<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr>
			<td align="center" valign="top">
				<div style="margin:0px 0px 5px 0px;"></div>
				<table cellspacing="0" cellpadding="0" border="0">
				<tr valign="top">
				<td>
					<table class="detail_table" cellspacing="1" cellpadding="3" width="840" border="0">
						<tr>
							<td class="detail_title_bg" align="center" colspan="6">ユーザー情報</td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left" width="117">ユーザーID</td>
							<td class="detail_item_bg" align="left" width="160">
								<a href="javascript:open_window('<%: this.UserDetailUrl %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes,resizable=YES');">
								<asp:Literal ID="lUserId" runat="server" />
								</a>
							</td>
							<td class="detail_title_bg" align="left" width="117"><%: ReplaceTag("@@User.name.name@@") %></td>
							<td class="detail_item_bg" align="left" width="160"><asp:Literal ID="lName" runat="server" /></td>
							<td class="detail_title_bg" align="left" width="117"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
							<td class="detail_item_bg" align="left" width="160"><asp:Literal ID="lNameKana" runat="server" /></td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left">住所</td>
							<td class="detail_item_bg" align="left" colspan="5"><asp:Literal ID="lZip" runat="server" /> <asp:Literal ID="lAddr" runat="server" /></td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left"><%: ReplaceTag("@@User.tel1.name@@") %></td>
							<td class="detail_item_bg" align="left"><asp:Literal ID="lTel1" runat="server" /></td>
							<td class="detail_title_bg" align="left"><%: ReplaceTag("@@User.tel2.name@@") %></td>
							<td class="detail_item_bg" align="left" colspan="3"><asp:Literal ID="lTel2" runat="server" /></td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left"><%: ReplaceTag("@@User.mail_addr.name@@") %></td>
							<td class="detail_item_bg" align="left"><asp:Literal ID="lMailAddr" runat="server" /></td>
							<td class="detail_title_bg" align="left"><%: ReplaceTag("@@User.mail_addr2.name@@") %></td>
							<td class="detail_item_bg" align="left" colspan="3"><asp:Literal ID="lMailAddr2" runat="server" /></td>
						</tr>
					</table>
					<% if (this.IsUpdateHistoryKbnOrder) { %>
					<div style="margin:0px 0px 5px 0px;"></div>
					<table class="detail_table" cellspacing="1" cellpadding="3" width="840" border="0">
						<tr>
							<td class="detail_title_bg" align="center" colspan="6">注文情報</td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left" width="117">注文ID</td>
							<td class="detail_item_bg" align="left" width="160">
								<a href="javascript:open_window('<%: this.OrderDetailUrl %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes,resizable=YES');">
								<asp:Literal ID="lOrderId" runat="server" />
								</a>
							</td>
							<td class="detail_title_bg" align="left"width="117">注文者区分</td>
							<td class="detail_item_bg" align="left" width="160"><asp:Literal ID="lOwnerKbn" runat="server" /></td>
							<td class="detail_title_bg" align="left" width="117">注文区分</td>
							<td class="detail_item_bg" align="left" width="160"><asp:Literal ID="lOrderKbn" runat="server" /></td>
						</tr>
						<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
							<tr>
								<td class="detail_title_bg" align="left" width="117">頒布会コースID</td>
								<td class="detail_item_bg" align="left" colspan="5">
									<asp:Literal ID="lSubscriptionBoxCourseIdOrder" runat="server" />
								</td>
							</tr>
						<%} %>
						<tr>
							<td class="detail_title_bg" align="left">決済種別</td>
							<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderPaymentKbn" runat="server" /></td>
							<td class="detail_title_bg" align="left">配送種別</td>
							<td class="detail_item_bg" align="left"><asp:Literal ID="lShippingId" runat="server" /></td>
							<td class="detail_title_bg" align="left">合計金額</td>
							<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderPriceTotal" runat="server" /></td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left">注文ステータス</td>
							<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderStatus" runat="server" /></td>
							<td class="detail_title_bg" align="left">注文日時</td>
							<td class="detail_item_bg" align="left" colspan="3"><asp:Literal ID="lOrderDate" runat="server" /></td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left">入金ステータス</td>
							<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderPaymentStatus" runat="server" /></td>
							<td class="detail_title_bg" align="left">入金日時</td>
							<td class="detail_item_bg" align="left" colspan="3"><asp:Literal ID="lOrderPaymentDate" runat="server" /></td>
						</tr>
					</table>
					<% } %>
					<% if (this.IsUpdateHistoryKbnFixedPurchase) { %>
					<div style="margin:0px 0px 5px 0px;"></div>
					<table class="detail_table" cellspacing="1" cellpadding="3" width="840" border="0">
						<tr>
							<td class="detail_title_bg" align="center" colspan="6">定期購入情報</td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left" width="117">定期購入ID</td>
							<td class="detail_item_bg" align="left" colspan="5">
								<a href="javascript:open_window('<%: this.FixedPurchaseDetailUrl %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes,resizable=YES');">
								<asp:Literal ID="lFixedPurchaseId" runat="server" />
								</a>
							</td>
						</tr>
						<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
							<tr>
								<td class="detail_title_bg" align="left" width="117">頒布会コースID</td>
								<td class="detail_item_bg" align="left" colspan="5">
									<asp:Literal ID="lSubscriptionBoxCourseId" runat="server" />
								</td>
							</tr>
						<%} %>
						<tr>
							<td class="detail_title_bg" align="left" width="117">定期購入設定</td>
							<td class="detail_item_bg" align="left" width="160"><asp:Literal ID="lFixedPurchaseSetting1" runat="server" /></td>
							<td class="detail_title_bg" align="left" width="117">定期購入開始日時</td>
							<td class="detail_item_bg" align="left" width="160"><asp:Literal id="lFixedPurchaseDate" runat="server" /></td>
							<td class="detail_title_bg" align="left" width="117">最終購入日</td>
							<td class="detail_item_bg" align="left" width="160"><asp:Literal ID="lFixedPurchaseLastOrderDate" runat="server" /></td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left">購入回数(注文基準)</td>
							<td class="detail_item_bg" align="left"><asp:Literal ID="lFixedPurchaseOrderCount" runat="server" /></td>
							<td class="detail_title_bg" align="left">購入回数(出荷基準)</td>
							<td class="detail_item_bg" align="left" colspan="3"><asp:Literal ID="lFixedPurchaseShippedCount" runat="server" /></td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left">定期購入ステータス</td>
							<td class="detail_item_bg" align="left"><asp:Literal ID="lFixedPurchaseStatus" runat="server" /></td>
							<td class="detail_title_bg" align="left">決済ステータス</td>
							<td class="detail_item_bg" align="left" colspan="3"><asp:Literal ID="lPaymentStatus" runat="server" /></td>
						</tr>
						<tr>
							<td class="detail_title_bg" align="left">次回配送日</td>
							<td class="detail_item_bg" align="left"><asp:Literal ID="lNextShippingDate" runat="server" /></td>
							<td class="detail_title_bg" align="left">次々回配送日</td>
							<td class="detail_item_bg" align="left" colspan="3"><asp:Literal ID="lNextNextShippingDate" runat="server" /></td>
						</tr>
					</table>
					<% } %>
				</td>
				</tr>
				</table>
				<div style="margin:0px 0px 5px 0px;"></div>
			</td>
			</tr>
			</table>
			<!--△ 各マスタ詳細 △-->
			</ContentTemplate>
			</asp:UpdatePanel>
		</td>
		</tr>
		</table>
	</td>
	</tr>
	<!--△ 詳細 △-->
	<tr><td><h1 class="cmn-hed-h2">更新履歴詳細</h1></td></tr>
	<!--▽ 一覧 ▽-->
	<tr>
	<td>
		<table class="box_border" cellspacing="1" cellpadding="3" width="860" border="0">
		<tr>
		<td>
			<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr>
			<td align="center">
				<asp:UpdatePanel runat="server">
				<ContentTemplate>

				<table class="detail_table" cellspacing="0" cellpadding="0" border="0">
				<tr id="trListError" class="list_alert" runat="server" Visible="true">
					<td id="tdErrorMessage" runat="server" style="background-color:#ffffff">
						<%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %>
					</td>
				</tr>
				<asp:Repeater ID="rDetailUpdateHistoryList" runat="server" OnItemCommand="rDetailUpdateHistoryList_ItemCommand" ItemType="UpdateHistoryInput">
					<ItemTemplate>
					<tr>
						<td class="list_item_bg1">
							<div style="margin:10px 0px 10px 0px;"></div>
							<table class="list_table" cellspacing="1" width="840" cellpadding="0" border="0">
							<tr class="list_title_bg">
								<td align="left" colspan="3">更新日時：<%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %> 更新区分：<%#: Item.UpdateHistoryAction %> 更新者：<%#: Item.LastChanged %><div style="float:right"> <asp:LinkButton id="lDisplayChange" runat="server" CommandArgument="<%# Item.UpdateHistoryNo %>"><%# Item.IsOpenAllField ? "差分表示" : "全表示" %></asp:LinkButton></a>&nbsp;&nbsp;</div></td>
							</tr>
							<tr class="list_title_bg">
								<td align="left" width="200">項目名</td>
								<td align="left" width="320">更新前</td>
								<td align="left" width="320">更新後</td>
							</tr>
							<asp:Repeater ID="rFieldList" runat="server" DataSource="<%# Item.BeforeAndAfterUpdateDataList %>" ItemType="BeforeAndAfterUpdateData">
							<ItemTemplate>
							<tr runat="server" visible="<%# (((UpdateHistoryInput)((RepeaterItem)Container.Parent.Parent).DataItem).IsOpenAllField || ((((UpdateHistoryInput)((RepeaterItem)Container.Parent.Parent).DataItem).IsOpenAllField == false) && (Item.IsDifferent()))) %>">
								<td align="left" class="list_title_bg" style="overflow-wrap:break-word;word-break:break-all;vertical-align:middle;"><%# WebSanitizer.HtmlEncodeChangeToBr(Item.FieldJName) %></td>
								<td align="left" class="list_item_bg1" style="overflow-wrap:break-word;word-break:break-all;vertical-align:top;">
									<span class='<%# Item.IsDifferent()  ? "marker_y" : "" %>'>
									<%# WebSanitizer.HtmlEncodeChangeToBr(DisplayValue(Item).Before) %></span></td>
								<td align="left" class="list_item_bg1" style="overflow-wrap:break-word;word-break:break-all;vertical-align:top;">
									<span class='<%# Item.IsDifferent() ? "marker_y" : "" %>'>
									<%# WebSanitizer.HtmlEncodeChangeToBr(Item.After) %></span></td>
							</tr>
							</ItemTemplate>
							</asp:Repeater>
							</table>
							<div style="margin:10px 0px 10px 0px;"></div>
						</td>
					</tr>
					</ItemTemplate>
				</asp:Repeater>
				</table>
				</ContentTemplate>
				</asp:UpdatePanel>
			</td>
			</tr>
			</table>
		</td>
		</tr>
		</table>
	</td>
	</tr>
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>