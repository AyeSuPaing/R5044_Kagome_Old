<%--
=========================================================================================================
  Module      : 配送会社情報登録ページ(DeliveryCompanyRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="DeliveryCompanyRegister.aspx.cs" Inherits="Form_DeliveryCompany_DeliveryCompanyRegister" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<asp:UpdatePanel runat="server">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ Title ▽-->
	<tr>
		<td><h1 class="page-title">配送サービス設定</h1></td>
	</tr>
	<tr>
		<td style="width: 797px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ End title △-->

	<!--▽ Registration ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送サービス設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送サービス設定登録</h2></td>
	</tr>
	<tr>
		<td style="width: 797px">
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top"><input onclick="Javascript: history.back();" type="button" value="  戻る  " />
												<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" /></div>

												<!--▽ 基本情報 ▽-->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="150">配送サービスID<span class="notice">*</span></td>
															<td id="tdShippingIdEdit" class="edit_item_bg" align="left" runat="server" visible="false">
																<asp:TextBox ID="tbDeliveryCompanyId" runat="server" MaxLength="10"></asp:TextBox>
															</td>
															<td id="tdShippingIdView" class="edit_item_bg" align="left" runat="server" visible="false">
																<asp:Literal ID="lDeliveryCompanyId" runat="server"></asp:Literal>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">配送サービス名<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbDeliveryCompanyName" runat="server" Width="250" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr runat="server" id="trDeliveryCompanyTypeCreditcard">
															<td class="edit_title_bg" align="left">出荷連携配送会社<br />(クレジットカード)</td>
															<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlDeliveryCompanyTypeCreditcard" runat="server"></asp:DropDownList></td>
														</tr>
														<tr runat="server" id="trDeliveryCompanyTypePostPayment">
															<td class="edit_title_bg" align="left">出荷連携配送会社<br />(後払い)</td>
															<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlDeliveryCompanyTypePostPayment" runat="server"></asp:DropDownList></td>
														</tr>
														<tr runat="server" id="trDeliveryCompanyTypeNpPostPayment" visible="false">
															<td class="edit_title_bg" align="left">出荷連携配送会社<br />(NP後払い)</td>
															<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlDeliveryCompanyTypeNpPostPayment" runat="server"></asp:DropDownList></td>
														</tr>
														<tr runat="server" id="trDeliveryCompanyTypeGooddeal" visible="false">
															<td class="edit_title_bg" align="left">出荷連携配送会社<br />(Goodeal)</td>
															<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlDeliveryCompanyTypeGooddeal" runat="server"></asp:DropDownList></td>
														</tr>
														<tr runat="server" id="trDeliveryCompanyTypeGmoAtokara" visible="false">
															<td class="edit_title_bg" align="left">出荷連携配送会社<br />(GMOアトカラ)</td>
															<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlDeliveryCompanyTypeGmoAtokara" runat="server"></asp:DropDownList></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">表示順<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbDisplayOrder" runat="server" Width="250" MaxLength="3"></asp:TextBox></td>
														</tr>
														<% if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED){ %>
														<tr>
															<td class="edit_title_bg" align="left">メール便配送サイズ上限</td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbDeliveryCompanyMailSizeLimit" runat="server" Width="90" MaxLength="9"></asp:TextBox><br>※購入商品の商品サイズ係数の合計値が、メール便配送サイズ上限を超えた際に、配送サービスが変更されます。</td>
														</tr>
														<% } %>
													<% if (Constants.TODAY_SHIPPABLE_DEADLINE_TIME){ %>
														<tr>
														<td align="left" class="edit_title_bg" width="30%">当日出荷締め時間&nbsp;<small>[<a href="javascript:void(0)" title="当日出荷締め時間は、何時何分まで、当日出荷を受け付けるかどうかを設定する項目になります。
当日出荷締め時間をご利用の場合、設定時間を入れてください。
当日出荷を利用しない場合は、空欄で設定してください。

＜設定方法＞
・注文時間が、設定時間以前の場合は、出荷予定日は当日になります。
例）当日出荷締め時間が「14:00」の場合、
2021/04/01 13:59 までの注文は、出荷予定日が、当日（2021/04/01）になります。

・注文時間が設定時間以降の場合は、出荷予定日が翌日になります。
例）当日出荷締め時間が「14:00」の場合、
2021/04/01 14:00 以降の注文は、出荷予定日が、翌日（2021/04/02）になります。">？</a>]</small>
														</td>
														<td align="left" class="edit_item_bg">
															<asp:DropDownList ID="ddlTimeHour" runat="server" DataSource="<%# DateTimeUtility.GetHourListItem() %>" DataTextField="Text" DataValueField="Value" />
															：
															<asp:DropDownList ID="ddlTimeMinute" runat="server" DataSource="<%# DateTimeUtility.GetMinuteListItem() %>" DataTextField="Text" DataValueField="Value" />
															(例)13:00　※利用しない場合は空を設定
														</td>
														</tr>
														<% } %>
													</tbody>
												</table>
												<!--△ 基本情報 △-->
												<br />
												<!--▽ 配送時間帯情報 ▽-->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="3">配送可能時間帯情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">時間帯設定の利用の有無</td>
															<td class="edit_item_bg" align="left" colspan="2">
																<asp:CheckBox id="cbShippingTimeSetFlg" Checked="false" OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="true" Runat="server"></asp:CheckBox>
															</td>
														</tr>
													<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
														<tr>
															<td class="edit_title_bg" align="left"></td>
															<td class="edit_title_bg" align="left">自社サイト内表示文言</td>
															<td class="edit_title_bg" align="left">モール取込マッピング文言　※カンマ区切り「,」で複数指定可</td>
														</tr>
													<% } %>
													</tbody>
													<tbody id="tbShippingTime" runat="server">
														<tr>
															<td class="edit_title_bg" align="left" width="150">時間帯1</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbShippingTimeMessage1" runat="server" Width="150" MaxLength="30"></asp:TextBox>
																ID:<asp:TextBox id="tbShippingTimeId1" runat="server" Width="50" MaxLength="5"></asp:TextBox>
															</td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
															    <td class="edit_item_bg" align="left"><asp:TextBox id="tbShippingTimeMessageMatching1" runat="server" Width="400" MaxLength="100"></asp:TextBox></td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="150">時間帯2</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbShippingTimeMessage2" runat="server" Width="150" MaxLength="30"></asp:TextBox>
																ID:<asp:TextBox id="tbShippingTimeId2" runat="server" Width="50" MaxLength="5"></asp:TextBox>
															</td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="edit_item_bg" align="left"><asp:TextBox id="tbShippingTimeMessageMatching2" runat="server" Width="400" MaxLength="100"></asp:TextBox></td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="150">時間帯3</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbShippingTimeMessage3" runat="server" Width="150" MaxLength="30"></asp:TextBox>
																ID:<asp:TextBox id="tbShippingTimeId3" runat="server" Width="50" MaxLength="5"></asp:TextBox>
															</td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="edit_item_bg" align="left"><asp:TextBox id="tbShippingTimeMessageMatching3" runat="server" Width="400" MaxLength="100"></asp:TextBox></td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="150">時間帯4</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbShippingTimeMessage4" runat="server" Width="150" MaxLength="30"></asp:TextBox>
																ID:<asp:TextBox id="tbShippingTimeId4" runat="server" Width="50" MaxLength="5"></asp:TextBox>
															</td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="edit_item_bg" align="left"><asp:TextBox id="tbShippingTimeMessageMatching4" runat="server" Width="400" MaxLength="100"></asp:TextBox></td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="150">時間帯5</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbShippingTimeMessage5" runat="server" Width="150" MaxLength="30"></asp:TextBox>
																ID:<asp:TextBox id="tbShippingTimeId5" runat="server" Width="50" MaxLength="5"></asp:TextBox>
															</td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="edit_item_bg" align="left"><asp:TextBox id="tbShippingTimeMessageMatching5" runat="server" Width="400" MaxLength="100"></asp:TextBox></td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="150">時間帯6</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbShippingTimeMessage6" runat="server" Width="150" MaxLength="30"></asp:TextBox>
																ID:<asp:TextBox id="tbShippingTimeId6" runat="server" Width="50" MaxLength="5"></asp:TextBox>
															</td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="edit_item_bg" align="left"><asp:TextBox id="tbShippingTimeMessageMatching6" runat="server" Width="400" MaxLength="100"></asp:TextBox></td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="150">時間帯7</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbShippingTimeMessage7" runat="server" Width="150" MaxLength="30"></asp:TextBox>
																ID:<asp:TextBox id="tbShippingTimeId7" runat="server" Width="50" MaxLength="5"></asp:TextBox>
															</td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="edit_item_bg" align="left"><asp:TextBox id="tbShippingTimeMessageMatching7" runat="server" Width="400" MaxLength="100"></asp:TextBox></td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="150">時間帯8</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbShippingTimeMessage8" runat="server" Width="150" MaxLength="30"></asp:TextBox>
																ID:<asp:TextBox id="tbShippingTimeId8" runat="server" Width="50" MaxLength="5"></asp:TextBox>
															</td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="edit_item_bg" align="left"><asp:TextBox id="tbShippingTimeMessageMatching8" runat="server" Width="400" MaxLength="100"></asp:TextBox></td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="150">時間帯9</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbShippingTimeMessage9" runat="server" Width="150" MaxLength="30"></asp:TextBox>
																ID:<asp:TextBox id="tbShippingTimeId9" runat="server" Width="50" MaxLength="5"></asp:TextBox>
															</td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="edit_item_bg" align="left"><asp:TextBox id="tbShippingTimeMessageMatching9" runat="server" Width="400" MaxLength="100"></asp:TextBox></td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="150">時間帯10</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbShippingTimeMessage10" runat="server" Width="150" MaxLength="30"></asp:TextBox>
																ID:<asp:TextBox id="tbShippingTimeId10" runat="server" Width="50" MaxLength="5"></asp:TextBox>
															</td>
															<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<td class="edit_item_bg" align="left"><asp:TextBox id="tbShippingTimeMessageMatching10" runat="server" Width="400" MaxLength="100"></asp:TextBox></td>
															<% } %>
														</tr>
													</tbody>
												</table>
												<!--△ 配送時間帯情報 △-->
												<%--▽ リードタイム設定情報 ▽--%>
												<% if (this.UseLeadTime){ %>
												<br />
												<table class="edit_table">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">配送リードタイム設定情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">リードタイム設定の利用の有無</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBox ID="cbDeliveryLeadTimeSetFlg" Checked="false" OnCheckedChanged="RefreshComponentsDeliveryLeadTimeSet_OnCheckedChanged" AutoPostBack="true" runat="server"></asp:CheckBox>
															</td>
														</tr>
														<tr id="trShippingLeadTimeDefault" runat="server" visible="false">
															<td class="edit_title_bg" align="left">基本配送リード（日数）<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="9">
																<asp:TextBox ID="tbShippingLeadTimeDefault" MaxLength="3" runat="server">日数</asp:TextBox></td>
														</tr>
													</tbody>
												</table>
												<br />
												<table class="edit_table">
													<tbody id="tbDeliveryLeadTimeSet" runat="server">
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">通常の配送リードタイム情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="center" width="20%">都道府県</td>
															<td class="edit_title_bg" align="center" >追加配送リード（日数）<span class="notice">*</span></td>
														</tr>
														<asp:Repeater ID="rDeliveryLeadTimeZone" runat="server">
															<ItemTemplate>
																<tr id="trZone" runat="server">
																	<td class="edit_title_bg" align="left" width="20%">
																		<asp:Label ID="lLeadTimeZoneName" Text="<%# StringUtility.ToEmpty(((DeliveryLeadTimeInput)Container.DataItem).LeadTimeZoneName) %>" runat="server"></asp:Label></td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox ID="tbShippingLeadTime" Text="<%# StringUtility.ToEmpty(((DeliveryLeadTimeInput)Container.DataItem).ShippingLeadTime) %>" MaxLength="3" runat="server"></asp:TextBox></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>
												<br />
												<div runat="server" id="dvAddZone">
													<asp:UpdatePanel ID="upDeliveryLeadTimeZoneSpecial" runat="server">
														<ContentTemplate>
															<table class="edit_table">
																<tr>
																	<td class="edit_title_bg" colspan="6" style="text-align: center"><span>特別の配送リードタイム情報</span></td>
																</tr>
																<tr>
																	<td colspan="6" style="text-align: center"><span>対象郵便番号を複数入力する場合はカンマで区切りって入力してください。</span></td>
																</tr>
																<tr>
																	<td>
																		<table class="edit_table">
																			<tr>
																				<td class="edit_title_bg" style="text-align: center; width: 25%;">地帯名<span style="color: red;">*</span> </td>
																				<td class="edit_title_bg" style="text-align: center;" >郵便番号(<span style="color: red;">※</span>「-」は不要)<span style="color: red;">*</span></td>
																				<td class="edit_title_bg" style="text-align: center; width: 20%;" >追加配送リード(日数)<span style="color: red;">*</span></td>
																				<td class="edit_title_bg" style="width: 6%; text-align: center;" ><asp:Button ID="btnAddTop" runat="server" Text="  追加  " OnClick="btnAddDeliveryLeadTimeZoneSpecial_Click" /></td>
																			</tr>
																			<asp:Repeater ID="rDeliveryLeadTimeZoneSpecial" runat="server" OnItemCommand="rDeliveryLeadTimeZoneSpecial_ItemCommand">
																				<ItemTemplate>
																					<tbody id="tbodyAddZoneItem" runat="server">
																						<tr>
																							<td>
																								<asp:TextBox ID="tbZoneName" Text="<%# StringUtility.ToEmpty(((DeliveryLeadTimeInput)Container.DataItem).LeadTimeZoneName) %>" runat="server"></asp:TextBox></td>
																							<td>
																								<asp:TextBox ID="tbZip" TextMode="MultiLine" Rows="3" Width="420" Text="<%# StringUtility.ToEmpty(((DeliveryLeadTimeInput)Container.DataItem).Zip) %>" runat="server"></asp:TextBox></td>
																							<td>
																								<asp:TextBox ID="tbShippingLeadTime" Text="<%# StringUtility.ToEmpty(((DeliveryLeadTimeInput)Container.DataItem).ShippingLeadTime) %>" MaxLength="3" runat="server"></asp:TextBox></td>
																							<td>
																								<asp:Button ID="btnDelete" runat="server" Text="削除" CommandName="DeleteZone" /></td>
																						</tr>
																					</tbody>
																				</ItemTemplate>
																			</asp:Repeater>
																		</table>
																	</td>
																</tr>
															</table>
														</ContentTemplate>
													</asp:UpdatePanel>
												</div>
												<% } %>
												<%--△ リードタイム設定情報 △--%>
												<br />
												<br />
												<br />
												<div class="action_part_bottom">
													<input onclick="Javascript: history.back();" type="button" value="  戻る  " />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
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
	<!--△ End registration △-->
</table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
