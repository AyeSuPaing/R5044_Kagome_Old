<%--
=========================================================================================================
  Module      : 配送会社情報確認ページ(DeliveryCompanyConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="DeliveryCompanyConfirm.aspx.cs" Inherits="Form_DeliveryCompany_DeliveryCompanyConfirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">配送サービス設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->

	<!--▽ 詳細 ▽-->
	<tr id="trDetailTop" runat="server" Visible="True">
		<td><h2 class="cmn-hed-h2">配送サービス設定詳細</h2></td>
	</tr>
	<tr id="trConfirmTop" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送サービス設定確認</h2></td>
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
												<div class="action_part_top"><input onclick="Javascript: history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnEditTop" runat="server" Visible="False" Text="  編集する  " OnClick="btnEdit_Click"></asp:Button>
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="false" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('設定を削除します。よろしいですか？')"></asp:Button>
													<asp:Button id="btnInsertTop" runat="server" Visible="False" Text="  登録する  " OnClick="btnInsert_Click"></asp:Button>
													<asp:Button id="btnUpdateTop" runat="server" Visible="False" Text="  更新する  " OnClick="btnUpdate_Click"></asp:Button>
												</div>

												<!--▽ 基本情報 ▽-->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">配送サービスID</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDeliveryCompanyId"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="150">配送サービス名</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDeliveryCompanyName"></asp:Literal></td>
													</tr>
													<tr runat="server" ID="trDeliveryCompanyTypeCreditcard">
														<td class="detail_title_bg" align="left">出荷連携配送会社<br />(クレジットカード)</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDeliveryCompanyTypeCreditcard"></asp:Literal></td>
													</tr>
													<tr runat="server" ID="trDeliveryCompanyTypePostPayment">
														<td class="detail_title_bg" align="left">出荷連携配送会社<br />(後払い)</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDeliveryCompanyTypePostPayment"></asp:Literal></td>
													</tr>
													<tr runat="server" ID="trDeliveryCompanyTypeNpPostPayment" visible="false">
														<td class="detail_title_bg" align="left">出荷連携配送会社<br />(NP後払い)</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDeliveryCompanyTypeNpPostPayment"></asp:Literal></td>
													</tr>
													<tr runat="server" ID="trDeliveryCompanyTypeGooddeal">
														<td class="detail_title_bg" align="left">出荷連携配送会社<br />(Gooddeal)</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDeliveryCompanyTypeGooddeal"></asp:Literal></td>
													</tr>
													<tr runat="server" ID="trDeliveryCompanyTypeGmoAtokara">
														<td class="detail_title_bg" align="left">出荷連携配送会社<br />(アトカラ)</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDeliveryCompanyTypeGmoAtokara"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">表示順</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDisplayOrder"></asp:Literal></td>
													</tr>
													<% if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED){ %>
													<tr>
														<td class="detail_title_bg" align="left">メール便配送サイズ上限</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDeliveryCompanyMailSizeLimit"></asp:Literal></td>
													</tr>
													<% } %>
													<% if (Constants.TODAY_SHIPPABLE_DEADLINE_TIME){ %>
													<tr>
														<td class="detail_title_bg" align="left">当日出荷締め時間</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDeadlineTime"></asp:Literal></td>
													</tr>
													<% } %>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left">作成日</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDateCreated"></asp:Literal></td>
													</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left">更新日</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lDateChanged"></asp:Literal></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left">最終更新者</td>
														<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lLastChanged"></asp:Literal></td>
													</tr>
												</table>
												<!--△ 基本情報 △-->
												<br />
												<!--▽ 配送時間帯情報 ▽-->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">配送可能時間帯情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="150">時間帯設定の利用の有無</td>
														<td class="detail_item_bg" align="left" colspan="2"><asp:Literal runat="server" ID="lShippingTimeSetFlg"></asp:Literal></td>
													</tr>
													<tbody id="tbShippingTime" runat="server">
													<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
														<tr>
															<td class="detail_title_bg" align="left"></td>
															<td class="detail_title_bg" align="left">自社サイト内表示文言</td>
															<td class="detail_title_bg" align="left">モール取込マッピング文言　※カンマ区切り「,」で複数指定可</td>
														</tr>
													<% } %>
														<tr>
															<td class="detail_title_bg" align="left">時間帯1</td>
															<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageID1"></asp:Literal></td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageMatching1"></asp:Literal></td>
															<% } %>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">時間帯2</td>
															<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageID2"></asp:Literal></td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageMatching2"></asp:Literal></td>
															<% } %>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">時間帯3</td>
															<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageID3"></asp:Literal></td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageMatching3"></asp:Literal></td>
															<% } %>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">時間帯4</td>
															<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageID4"></asp:Literal></td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageMatching4"></asp:Literal></td>
															<% } %>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">時間帯5</td>
															<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageID5"></asp:Literal></td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageMatching5"></asp:Literal></td>
															<% } %>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">時間帯6</td>
															<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageID6"></asp:Literal></td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageMatching6"></asp:Literal></td>
															<% } %>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">時間帯7</td>
															<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageID7"></asp:Literal></td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageMatching7"></asp:Literal></td>
															<% } %>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">時間帯8</td>
															<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageID8"></asp:Literal></td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageMatching8"></asp:Literal></td>
															<% } %>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">時間帯9</td>
															<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageID9"></asp:Literal></td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageMatching9"></asp:Literal></td>
															<% } %>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">時間帯10</td>
															<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageID10"></asp:Literal></td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="detail_item_bg" align="left"><asp:Literal runat="server" ID="lShippingTimeMessageMatching10"></asp:Literal></td>
															<% } %>
														</tr>
													</tbody>
												</table>
												<!--△ 配送時間帯情報 △-->
												<br />
												<%--▽ リードタイム設定情報 ▽--%>
												<% if (this.UseLeadTime){ %>
												<!--▽ リードタイム設定の利用の有無 ▽-->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">配送リードタイム設定情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">リードタイム設定の利用の有無</td>
														<td class="detail_item_bg" align="left" colspan="2">
															<asp:Literal runat="server" ID="lDeliveryLeadTimeZoneSetFlg"></asp:Literal></td>
													</tr>
													<tr id="trShippingLeadTimeDefault" runat="server">
														<td class="detail_title_bg" align="left">基本配送リード（日数）</td>
														<td class="detail_item_bg" align="left" colspan="2">
															<asp:Literal runat="server" ID="lShippingLeadTimeDefault"></asp:Literal></td>
													</tr>
												</table>
												<!--△ リードタイム設定の利用の有無 △-->
												<br />
												<!--▽ 配信リードタイムゾーン情報 ▽-->
												<table class="edit_table">
													<tbody id="tbDeliveryLeadTimeZone" runat="server">
														<asp:Repeater ID="rDeliveryLeadTimeZone" runat="server">
														<HeaderTemplate>
															<tr>
																<td class="detail_title_bg" align="center" colspan="3">通常の配送リードタイム情報</td>
															</tr>
														</HeaderTemplate>
															<ItemTemplate>
																<tr id="trZone" runat="server">
																	<td class="edit_title_bg" align="left" width="20%">
																		<asp:Label ID="lLeadTimeZoneName" Text="<%# StringUtility.ToEmpty(((DeliveryLeadTimeInput)Container.DataItem).LeadTimeZoneName) %>" runat="server"></asp:Label></td>
																	<td class="edit_item_bg" align="left" colspan="2">
																		<asp:Label ID="tbShippingLeadTime" Text="<%# StringUtility.ToEmpty(Int32.Parse(((DeliveryLeadTimeInput)Container.DataItem).ShippingLeadTime)) %>" runat="server"></asp:Label></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
													<tbody id="tbDeliveryLeadTimeZoneSpecial" runat="server" visible="false">
														<tr>
															<td class="detail_title_bg" align="center" colspan="3">特別の配送リードタイム情報</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="center" width="25%">地帯名</td>
															<td class="detail_title_bg" align="center">郵便番号
															</td>
															<td class="detail_title_bg" align="center" width="20%">追加配送リード(日数)</td>
														</tr>
														<asp:Repeater ID="rDeliveryLeadTimeZoneSpecial" runat="server">
															<ItemTemplate>
																<tr id="trZone" runat="server">
																	<td class="edit_title_bg" align="left" width="20%">
																		<asp:Label ID="lLeadTimeZoneName" Text="<%# StringUtility.ToEmpty(((DeliveryLeadTimeInput)Container.DataItem).LeadTimeZoneName) %>" runat="server"></asp:Label></td>
																	<td align="left">
																		<asp:Label ID="tbZip" Style="word-break: break-all;" Width="100%" Text="<%# StringUtility.ToEmpty(((DeliveryLeadTimeInput)Container.DataItem).Zip) %>" runat="server"></asp:Label></td>
																	<td align="left">
																		<asp:Label ID="tbShippingLeadTime" Text="<%# StringUtility.ToEmpty(Int32.Parse(((DeliveryLeadTimeInput)Container.DataItem).ShippingLeadTime)) %>" runat="server"></asp:Label></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>
												<!--△ 配信リードタイムゾーン情報 △-->
												<% } %>
												<%--△ リードタイム設定情報 △--%>
												<div class="action_part_bottom">
													<input onclick="Javascript: history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnEditBottom" runat="server" Visible="False" Text="  編集する  " OnClick="btnEdit_Click"></asp:Button>
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="false" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('設定を削除します。よろしいですか？')"></asp:Button>
													<asp:Button id="btnInsertBottom" runat="server" Visible="False" Text="  登録する  " OnClick="btnInsert_Click"></asp:Button>
													<asp:Button id="btnUpdateBottom" runat="server" Visible="False" Text="  更新する  " OnClick="btnUpdate_Click"></asp:Button>
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
