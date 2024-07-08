<%--
=========================================================================================================
  Module      : 注文ワークフロー設定情報登録ページ(OrderWorkflowSettingRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register src="../Common/KeepFormData.ascx" tagname="KeepFormData" tagprefix="uc" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderWorkflowSettingRegister.aspx.cs" Inherits="Form_OrderWorkflowSetting_OrderWorkflowSettingRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.App.Common.Order.Workflow" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.Common.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">受注ワークフロー設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">受注ワークフロー設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">受注ワークフロー設定登録</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<asp:UpdatePanel ID="UpdatePanel1" runat="server">
									<ContentTemplate>
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
                                                <div id="divComp" runat="server" class="action_part_top" Visible="False">
												<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
													<tr class="info_item_bg">
														<td align="left"><asp:Label ID="lMessage" runat="server"></asp:Label>
														</td>
													</tr>
												</table>
												</div>
												<div class="action_part_top">
                                                    <asp:Button id="btnBack" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" OnClientClick="return canExecInsert();" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdate_Click" /></div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本設定</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="28%">ワークフロー区分<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList ID="ddlWorkflowKbn" Runat="server" SelectedValue="<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN) %>" AutoPostBack="True" OnSelectedIndexChanged="ddlWorkflowKbn_SelectedIndexChanged"></asp:DropDownList>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="28%">ワークフロー名<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbWorkflowName" runat="server" Text="<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME) %>" Width="480" MaxLength="50"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="28%">ワークフロー作業説明</td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbDesc1" runat="server" Text="<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_DESC1) %>" Width="480" MaxLength="60" TextMode="MultiLine" Columns="80" Rows="3"></asp:TextBox></td>
														</tr>
														<tr id="trWorkflowType" runat="server" Visible="<%# Constants.FIXEDPURCHASE_OPTION_ENABLED %>">
															<td class="edit_title_bg" align="left" width="28%">ワークフロー対象</td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButtonList id="rblTargetWorkflowType"
																					runat="server"
																					SelectedValue="<%# this.WorkflowType %>"
																					RepeatDirection="Horizontal"
																					RepeatLayout="Flow"
																					OnSelectedIndexChanged="rblTargetWorkflowType_OnSelectedIndexChanged"
																					AutoPostBack="true">
																</asp:RadioButtonList>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="28%">有効フラグ<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBox ID="cbValidFlg" runat="server" AutoPostBack="false" Checked="<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_VALID_FLG) == Constants.FLG_ORDERWORKFLOWSETTING_VALID_FLG_VALID) %>" Text="有効" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="28%">ワークフロー実行順<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbDisplayOrder" runat="server" Text='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_ORDER, "1") %>' Width="25" MaxLength="3"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="28%">ワークフロー一覧表示件数<p>※表示件数が100件を超えると実行形式が自動で切り替わります。</p></td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList id="ddlDisplayCount" Runat="server" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT, ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT)[0].Value) %>'
																	OnSelectedIndexChanged="ddlDisplayCount_SelectedIndexChanged" AutoPostBack="true" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="28%">ワークフロー実行形式<span class="notice">*</span><p>※実行形式を切り替えるとアクション設定の再設定が必要です。</p></td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButton ID="rbWorkFlowDetailKbnNormal" GroupName="WorkFlowDetailKbn" 
																	runat="server" 
																	Checked=<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN, Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL) == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL %> 
																	Text="<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN, Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL)) %>" 
																	AutoPostBack="true"
																	OnCheckedChanged="rblDisplayKbn_SelectedIndexChanged" /><br />
																&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
																<asp:RadioButtonList id="rblDisplayKbn" Runat="server" RepeatDirection="Horizontal" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN, Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE) %>' RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="rblDisplayKbn_SelectedIndexChanged"></asp:RadioButtonList>
																<br />
																<div id="divReturnOrder" runat="server">
																	<asp:RadioButton ID="rbWorkFlowDetailKbnReturn" GroupName="WorkFlowDetailKbn"
																		runat="server"
																		Checked="<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN) == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_RETURN %> "
																		Text="<%#: ValueText.GetValueText(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN, Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_RETURN) %>" 
																		AutoPostBack="true"
																		OnCheckedChanged="rblDisplayKbn_SelectedIndexChanged" /><br />
																	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
																	<asp:RadioButtonList id="rblDisplayKbnReturn" Runat="server" RepeatDirection="Horizontal" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN, Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE) %>' RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="rblDisplayKbn_SelectedIndexChanged"></asp:RadioButtonList>
																	<br />
																</div>
																<asp:RadioButton ID="rbWorkFlowDetailKbnOrderImportPopUp" GroupName="WorkFlowDetailKbn" AutoPostBack="true" OnCheckedChanged="rblDisplayKbn_SelectedIndexChanged" runat="server" Checked=<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN) == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_ODR_IMP %> Text="<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN, Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_ODR_IMP)) %>" /><br />
															</td>
														</tr>
														<tr id="trAdditionalSearchFlg" runat="server">
															<td class="edit_title_bg" align="left" width="28%">検索ボックスの指定<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButtonList id="rblAdditionalSearchFlg" AutoPostBack="false" Runat="server" RepeatDirection="Vertical" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG, Constants.FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_OFF) %>' RepeatLayout="Flow"></asp:RadioButtonList>
															</td>
														</tr>
													</tbody>
												</table>
												<%--▽ 受注情報抽出検索条件設定 ▽--%>
												<div id="divSearch" runat="server">
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">抽出検索条件設定</td>
													</tr>
													<%--▽ 通常注文 ▽--%>
													<tbody id="tbodyOrderSearch" runat="server">
													<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">注文種別</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrder" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">注文区分</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBoxlist id="cblOrderKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">注文者区分</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBoxlist id="cblOwnerKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">並び順</td>
														<td class="edit_item_bg">
															<asp:DropDownList id="ddlSortKbn" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">注文ステータス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr id="trStorePickupStatusCondition" visible="false" runat="server">
														<td class="edit_title_bg" align="left" width="28%">店舗受取ステータス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist ID="cblStorePickupStatus"
																runat="server"
																RepeatDirection="Horizontal"
																RepeatLayout="Flow" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%" rowspan="2">ステータス更新日</td>
														<td class="edit_item_bg">
															<asp:DropDownList ID="ddlUpdateStatusDate" runat="server"></asp:DropDownList>を
														</td>
													</tr>
													<tr>
														<td class="edit_item_bg">
															<asp:RadioButtonList ID="rblUpdateStatusDate" DataValueField="Value" DataTextField="Text" Runat="server" RepeatDirection="Horizontal" RepeatColumns="6" RepeatLayout="Flow" AutoPostBack="True" OnSelectedIndexChanged="rblUpdateStatusDate_SelectedIndexChanged">
															</asp:RadioButtonList>
															&nbsp;&nbsp;
															<asp:DropDownList ID="ddlUpdateStatusDateFrom" Runat="server" Enabled="false">
															</asp:DropDownList>日前～
															過去に＋<asp:DropDownList ID="ddlUpdateStatusDateTo" Runat="server" Enabled="false">
															</asp:DropDownList>日の間で
															<asp:DropDownList ID="ddlUpdateStatusHourTo" Runat="server" Enabled="false"></asp:DropDownList>時
															<asp:DropDownList ID="ddlUpdateStatusMinuteTo" Runat="server" Enabled="false"></asp:DropDownList>分
															<asp:DropDownList ID="ddlUpdateStatusSecondTo" Runat="server" Enabled="false"></asp:DropDownList>秒&nbsp;以降の注文を対象												
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">商品ID<br>※いずれかの商品が購入されたものを抽出</td>
														<td class="edit_item_bg">
															<asp:TextBox id="tbProductId" runat="server" Width="400"></asp:TextBox>(カンマ区切り)
														</td>
													</tr>
													<% if (Constants.SETPROMOTION_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">セットプロモーションID</td>
														<td class="edit_item_bg">
															<asp:TextBox id="tbOrderSetPromotionId" runat="server" Width="100"></asp:TextBox>
														</td>
													</tr>
													<% } %>
													<%--▽ ノベルティOPが有効の場合 ▽--%>
													<% if (Constants.NOVELTY_OPTION_ENABLED){ %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">ノベルティID</td>
														<td class="edit_item_bg">
															<asp:CheckBox ID="cbNoveltyOn" Text=" あり " runat="server" OnCheckedChanged="cbNoveltyOn_OnCheckedChanged" AutoPostBack="true" />
															<asp:TextBox id="tbNoveltyId" runat="server" Width="250" Enabled="false"/> （カンマ区切り）
															<asp:CheckBox ID="cbNoveltyOff" Text=" なし " runat="server" />
															<br/>
															※テキストボックスが空の場合はノベルティIDがある全ての注文を抽出対象とします。
														</td>
													</tr>
													<% } %>
													<%--△ ノベルティOPが有効の場合 △--%>
													<%--▽ レコメンド設定OPが有効の場合 ▽--%>
													<% if (Constants.RECOMMEND_OPTION_ENABLED){ %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">レコメンドID</td>
														<td class="edit_item_bg">
															<asp:CheckBox ID="cbRecommendOn" Text=" あり " runat="server" OnCheckedChanged="cbRecommendOn_OnCheckedChanged" AutoPostBack="true" />
															<asp:TextBox id="tbRecommendId" runat="server" Width="250" Enabled="false"/> （カンマ区切り）
															<asp:CheckBox ID="cbRecommendOff" Text=" なし " runat="server" />
															<br/>
															※テキストボックスが空の場合はレコメンドIDがある全ての注文を抽出対象とします。
														</td>
													</tr>
													<% } %>
													<%--△ レコメンド設定OPが有効の場合 △--%>
													<%-- ▽頒布会OPが有効の場合▽ --%>
													<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
														<tr>
															<td class="edit_title_bg" width="28%">頒布会コースID</td>
															<td class="edit_item_bg">
																<asp:TextBox id="tbSubscriptionBoxCourseId" runat="server" Width="125"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />頒布会注文回数</td>
															<td class="edit_item_bg" colspan="3">
																<asp:TextBox id="tbSubscriptionBoxOrderCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbSubscriptionBoxOrderCountTo" runat="server" Width="45" />
															</td>
														</tr>
													<%} %>
													<%-- △頒布会OPが有効の場合△ --%>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">合計金額</td>
														<td class="edit_item_bg">
															<asp:TextBox id="tbOrderPriceTotal" runat="server" Width="100" MaxLength="9"></asp:TextBox><%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %>以上
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">入金ステータス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderPaymentStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr id="trDemandStatus" visible="false" runat="server">
														<td  class="edit_title_bg" align="left" width="28%">督促ステータス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblDemandStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<asp:Repeater ID="rOrderExtendStatusForSearch" runat="server">
														<ItemTemplate>
														<tr>
															<td class="edit_title_bg" align="left" width="28%">拡張ステータス<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO)%>：
																<br />&nbsp;<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME)%></td>
															<td class="edit_item_bg">
																<asp:HiddenField id="hfExtendStatusChangeNo1" runat="server" Value="<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO)%>" />
																<asp:CheckBox id="cbExtendStatusOn" runat="server" Checked="<%# CheckExtendStatusSame((int)Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO), Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON, WorkflowSetting.WorkflowTypes.Order) %>" />オン
																<asp:CheckBox id="cbExtendStatusOff" runat="server" Checked="<%# CheckExtendStatusSame((int)Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO), Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF, WorkflowSetting.WorkflowTypes.Order) %>" />オフ
															</td>
														</tr>
														</ItemTemplate>
													</asp:Repeater>
													<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
													<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">引当状況</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderStockRservedStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">出荷状況</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderShippedStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<% } %>
													<%--△ 実在庫利用が有効な場合は表示 △--%>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">決済種別</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderPaymentKbn" runat="server" DataTextField="Key" DataValueField="Value"	RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">外部決済ステータス：クレジットカード</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderExternalPaymentStatusByCard" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">外部決済ステータス：コンビニ（後払い）</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderExternalPaymentStatusByCVS" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">外部決済ステータス：後付款(TriLink後払い)</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderExternalPaymentStatusByTryLinkAfterPay" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">ECPay決済方法</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderExternalPaymentStatusByEcPay" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">藍新Pay決済方法</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderExternalPaymentStatusByNewebPay" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">外部決済ステータス：全決済</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderExternalPaymentStatusForAllPayment" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">最終与信日時</td>
														<td class="edit_item_bg">
															<asp:CheckBox ID="cbLastAuthDateOn" Text="&nbsp;あり&nbsp;&nbsp;" runat="server" OnCheckedChanged="cbLastAuthDateOn_OnCheckedChanged" AutoPostBack="true"></asp:CheckBox>
															<asp:TextBox id="tbLastAuthDateFrom" Enabled="false" runat="server" Width="30" MaxLength="3"></asp:TextBox>日前～
															<asp:TextBox id="tbLastAuthDateTo" Enabled="false" runat="server" Width="30" MaxLength="3"></asp:TextBox>日前まで&nbsp;&nbsp;
															<asp:CheckBox ID="cbLastAuthDateOff" Text="&nbsp;なし" runat="server"></asp:CheckBox>
														</td>
													</tr>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送先：国</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblShippingCountry" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送種別</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderShippingKbn" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<%--▽ 出荷予定日オプションが有効な場合は表示 ▽--%>
													<% if (this.UseLeadTime) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">出荷予定日指定
															<a href="#note">備考</a>
														</td>
														<td class="edit_item_bg">
															<asp:CheckBox ID="cbScheduledShippingDateOn" Text="&nbsp;あり&nbsp;&nbsp;" runat="server" OnCheckedChanged="cbScheduledShippingDateOn_OnCheckedChanged" AutoPostBack="true"></asp:CheckBox>
															本日から<asp:TextBox id="tbcbScheduledShippingDateFrom" runat="server" Width="30" MaxLength="3"></asp:TextBox>営業日以降　～　
															<asp:TextBox id="tbcbScheduledShippingDateTo" runat="server" Width="30" MaxLength="3"></asp:TextBox>営業日の間の注文&nbsp;&nbsp;
															<asp:CheckBox ID="cbScheduledShippingDateOff" Text="&nbsp;なし" runat="server"></asp:CheckBox>
														</td>
													</tr>
													<% } %>
													<%--△ 出荷予定日オプションが有効な場合は表示 △--%>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送希望日指定
															<a href="#note">備考</a>
														</td>
														<td class="edit_item_bg">
															<asp:CheckBox ID="cbShippingDateOn" Text="&nbsp;あり&nbsp;&nbsp;" runat="server" OnCheckedChanged="cbShippingDateOn_OnCheckedChanged" AutoPostBack="true"></asp:CheckBox>
															本日から<asp:TextBox id="tbShippingDateFrom" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以降　～　
															<asp:TextBox id="tbShippingDateTo" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以前の間の注文&nbsp;&nbsp;
															<asp:CheckBox ID="cbShippingDateOff" Text="&nbsp;なし" runat="server"></asp:CheckBox>
														</td>
													</tr>
													<% if (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">送料の別途見積</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblSeparateEstimatesFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送伝票番号</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblShippingCheckNo" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">出荷後変更区分</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblShippedChangedKbn" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">サイト</td>
														<td class="edit_item_bg">
															<asp:CheckBoxList ID="cblSiteName" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"></asp:CheckBoxList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">モール連携ステータス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxList ID="cblMallLinkStatus" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"></asp:CheckBoxList>
														</td>
													</tr>
													<% } %>
													<% if (Constants.MALLCOOPERATION_OPTION_ENABLED || Constants.URERU_AD_IMPORT_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">外部連携ステータス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxList ID="cblExternalImportStatus" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"></asp:CheckBoxList>
														</td>
													</tr>
													<% } %>
													<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">楽天ポイント利用方法<br />※外部連携メモを参照しています。</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblRakutenPointUseType" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<% } %>
													<% if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">デジタルコンテンツ商品</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblDigitalContentsFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
															<asp:RadioButtonList ID="rblDigitalContentsFlg" DataValueField="Value" DataTextField="Text" Runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
														</td>
													</tr>
													<% } %>
													<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">定期購買注文</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblFixedPurchase" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
															<asp:RadioButtonList ID="rblFixedPurchase" DataValueField="Value" DataTextField="Text" Runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">定期購入回数(注文時点)</td>
														<td class="edit_item_bg">
															<asp:TextBox id="tbOrderCountFrom" runat="server" Width="45" />以上&nbsp;～&nbsp;<asp:TextBox id="tbOrderCountTo" runat="server" Width="45" />以下
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">定期購入回数(出荷時点)</td>
														<td class="edit_item_bg">
															<asp:TextBox id="tbShippedCountFrom" runat="server" Width="45" />以上&nbsp;～&nbsp;<asp:TextBox id="tbShippedCountTo" runat="server" Width="45" />以下
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">注文メモ</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderMemoFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
															<br />
															<asp:TextBox ID="tbMemo" runat="server" Text='<%# GetSearchSettingValue(Constants.FIELD_ORDER_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT, WorkflowSetting.WorkflowTypes.Order) %>' width="530" Height="40" TextMode="MultiLine" Visible="<%#Constants.ORDERWORKFLOWSETTING_MEMO_TEXTBOX_ENABLED%>"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">管理メモ</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderManagementMemoFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
															<br />
															<asp:TextBox ID="tbManagementMemo" runat="server" Text='<%# GetSearchSettingValue(Constants.FIELD_ORDER_MANAGEMENT_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT, WorkflowSetting.WorkflowTypes.Order) %>' Width="530" Height="40" TextMode="MultiLine" Visible="<%#Constants.ORDERWORKFLOWSETTING_MEMO_TEXTBOX_ENABLED%>"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送メモ</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblShippingMemoFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
															<br />
															<asp:TextBox ID="tbShippingMemo" runat="server" Text='<%# GetSearchSettingValue(Constants.FIELD_ORDER_SHIPPING_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT, WorkflowSetting.WorkflowTypes.Order) %>' Width="530" Height="40" TextMode="MultiLine" Visible="<%#Constants.ORDERWORKFLOWSETTING_MEMO_TEXTBOX_ENABLED%>" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">決済連携メモ</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderPaymentMemoFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
															<br />
															<asp:TextBox ID="tbPaymentMemo" runat="server" Text='<%# GetSearchSettingValue(Constants.FIELD_ORDER_PAYMENT_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT, WorkflowSetting.WorkflowTypes.Order) %>' width="530" Height="40" TextMode="MultiLine" Visible="<%#Constants.ORDERWORKFLOWSETTING_MEMO_TEXTBOX_ENABLED%>"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">外部連携メモ</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderRelationMemoFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
															<br />
															<asp:TextBox ID="tbRelationMemo" runat="server" Text='<%# GetSearchSettingValue(Constants.FIELD_ORDER_RELATION_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT, WorkflowSetting.WorkflowTypes.Order) %>' Width="530" Height="40" TextMode="MultiLine" Visible="<%#Constants.ORDERWORKFLOWSETTING_MEMO_TEXTBOX_ENABLED%>"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">ユーザー特記欄</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblUserUserMemoFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">商品付帯情報</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblProductOptionFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr runat="server" visible="<%# Constants.W2MP_AFFILIATE_OPTION_ENABLED %>">
														<td class="edit_title_bg" align="left" width="28%">広告コード</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblAdvCodeFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="True" OnSelectedIndexChanged="cbAdvCode_OnCheckedChanged"></asp:CheckBoxlist>
															<asp:TextBox ID="tbAdvCode" runat="server" Text='<%# GetSearchSettingValue(OrderSearchParam.KEY_ORDER_ADVCODE + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT, WorkflowSetting.WorkflowTypes.Order) %>' Width="200" Visible="True" style="margin-left: 20px"></asp:TextBox>
														</td>
													</tr>
													<%if (Constants.GIFTORDER_OPTION_ENABLED){ %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">ギフト購入フラグ</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblGiftFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<%} %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">ユーザー管理レベル</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblUserManagementLevel" runat="server" DataTextField="Key" DataValueField="Value"	RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送先</td>
														<td class="edit_item_bg">
															<asp:DropDownList id="ddlAnotherShippingFlag"
																runat="server"
																AutoPostBack="True"
																OnSelectedIndexChanged="ddlAnotherShippingFlag_SelectedIndexChanged" />
														</td>
													</tr>
													<tr id="trPickupStore" visible="false" runat="server">
														<td class="edit_title_bg" align="left" width="28%">受取店舗</td>
														<td class="edit_item_bg">
															<asp:DropDownList ID="ddlStorePickup" runat="server" />
														</td>
													</tr>
													<% if (this.IsSearchShippingAddr1) { %>
													<tr>
														<td class="edit_title_bg">配送先：都道府県</td>
														<td class="edit_item_bg" colspan="5">
															<span ID="LocalArea1"><asp:CheckBox ID="cbLocalArea1" runat="server" style="font-weight:bold" Text="北海道"></asp:CheckBox></span>
															<span ID="LocalArea2"><asp:CheckBox ID="cbLocalArea2" runat="server" style="font-weight:bold" Text="東北"></asp:CheckBox></span>
															<span ID="LocalArea3"><asp:CheckBox ID="cbLocalArea3" runat="server" style="font-weight:bold" Text="関東"></asp:CheckBox></span>
															<span ID="LocalArea4"><asp:CheckBox ID="cbLocalArea4" runat="server" style="font-weight:bold" Text="中部"></asp:CheckBox></span>
															<span ID="LocalArea5"><asp:CheckBox ID="cbLocalArea5" runat="server" style="font-weight:bold" Text="近畿"></asp:CheckBox></span>
															<span ID="LocalArea6"><asp:CheckBox ID="cbLocalArea6" runat="server" style="font-weight:bold" Text="中国"></asp:CheckBox></span>
															<span ID="LocalArea7"><asp:CheckBox ID="cbLocalArea7" runat="server" style="font-weight:bold" Text="四国"></asp:CheckBox></span>
															<span ID="LocalArea8"><asp:CheckBox ID="cbLocalArea8" runat="server" style="font-weight:bold" Text="九州/沖縄"></asp:CheckBox></span>
															<asp:CheckBoxlist id="cblShippingPrefectures" runat="server" DataTextField="Key" DataValueField="Value"
																			RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="50"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg">市区町村</td>
														<td class="edit_item_bg" colspan="5">
															<asp:TextBox id="tbShippingCity" runat="server" Width="35%" MaxLength="50"/>
														</td>
													</tr>
													<% } %>
													<% if (OrderCommon.CanDisplayInvoiceBundle()) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">請求書同梱フラグ</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblInvoiceBundleFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<% } %>
													<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送状態</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblShippingStatus" runat="server" DataTextField="Key" DataValueField="Value"	RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5">
															</asp:CheckBoxlist>
														</td>
													</tr>
													<% } %>
													<% if (Constants.TWPELICAN_COOPERATION_EXTEND_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">完了状態コード</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblShippingStatusCode" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">現在の状態</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblShippingCurrentStatus" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6"></asp:CheckBoxlist>
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送方法</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblShippingMethod" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送サービス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblDeliveryCompany" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">領収書希望フラグ
														<td class="edit_item_bg">
															<asp:CheckBoxList id="cblOrderReceiptFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">領収書出力フラグ
														<td class="edit_item_bg">
															<asp:CheckBoxList id="cblOrderReceiptOutputFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
														</td>
													</tr>
													<% } %>
													<asp:Repeater ID="rOrderExtend"
																ItemType="w2.Domain.OrderExtendSetting.OrderExtendSettingModel"
																visible="<%# Constants.ORDER_EXTEND_OPTION_ENABLED %>"
																runat="server">
														<ItemTemplate>
															<tr>
																<td class="edit_title_bg" align="left" width="28%">注文拡張項目:<br /> <%#: Item.SettingName %>(<%#: Item.SettingId %>)
																<td class="edit_item_bg">
																	<asp:CheckBoxList id="cblOrderExtendAttribute"  DataTextField="Text" DataValueField="Value" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
																</td>
															</tr>
															<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingId %>" />
														</ItemTemplate>
													</asp:Repeater>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">キャンセル可能時間帯の注文
														<td class="edit_item_bg">
															<asp:CheckBox ID="cbExtractCancelable" Text="キャンセル可能時間帯の注文を除く" runat="server"></asp:CheckBox>
															<br/>
															※注文時刻から<%: Constants.ORDER_HISTORY_DETAIL_ORDER_CANCEL_TIME %>分以内の注文が対象となります。
														</td>
													</tr>
													</tbody>
													<%--△ 通常注文 △--%>
													<%--▽ 返品交換注文 ▽--%>
													<tbody id="tbodyReturnExchangeSearch" runat="server">
													<tr>
														<td class="edit_title_bg" align="left" width="28%">返品交換区分</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblReturnExchangeKbn" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">返品交換都合区分</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblReturnExchangeReasonKbn" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">並び順</td>
														<td class="edit_item_bg">
															<asp:DropDownList id="ddlReturnExchangeSortKbn" runat="server">
															</asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">返品交換ステータス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderReturnExchangeStatus" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">返金ステータス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblOrderRepaymentStatus" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%" rowspan="2">返品交換返金更新日</td>
														<td class="edit_item_bg">
															<asp:DropDownList ID="ddlReturnExchangeUpdateStatusDate" runat="server"></asp:DropDownList>を
														</td>
													</tr>
													<tr>
														<td class="edit_item_bg">
															<asp:RadioButtonList ID="rblReturnExchangeUpdateStatusDate" DataValueField="Value" DataTextField="Text" Runat="server" RepeatDirection="Horizontal" RepeatColumns="6" RepeatLayout="Flow" AutoPostBack="True" OnSelectedIndexChanged="rblReturnExchangeUpdateStatusDate_SelectedIndexChanged">
															</asp:RadioButtonList>
															&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
															<asp:DropDownList ID="ddlReturnExchangeUpdateStatusDateFrom" Runat="server" Enabled="false"></asp:DropDownList>日前～
															過去に＋<asp:DropDownList ID="ddlReturnExchangeUpdateStatusDateTo" Runat="server" Enabled="false"></asp:DropDownList>日の間で
															<asp:DropDownList ID="ddlReturnExchangeUpdateStatusHourTo" Runat="server" Enabled="false"></asp:DropDownList>時
															<asp:DropDownList ID="ddlReturnExchangeUpdateStatusMinuteTo" Runat="server" Enabled="false"></asp:DropDownList>分
															<asp:DropDownList ID="ddlReturnExchangeUpdateStatusSecondTo" Runat="server" Enabled="false"></asp:DropDownList>秒～
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">決済種別</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblReturnOrderPaymentKbn" runat="server" DataTextField="Key" DataValueField="Value"	RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">外部決済ステータス：クレジットカード</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblReturnOrderExternalPaymentStatusByCard" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">外部決済ステータス：コンビニ（後払い）</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblReturnOrderExternalPaymentStatusByCVS" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">外部決済ステータス：台湾後払い</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblReturnOrderExternalPaymentStatusByTryLinkAfterPay" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">外部決済ステータス：全決済</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblReturnOrderExternalPaymentStatusForAllPayment" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													</tbody>
													<%--△ 返品交換注文 △--%>
													<tr id="trInvoiceStatus" runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo() %>">
														<td class="edit_title_bg" align="left" width="28%">発票ステータス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cbInvoiceStatus" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
												</table>
												</div>
												<%--△ 受注情報抽出検索条件設定 △--%>
												<%--▽ 定期台帳抽出検索条件設定 ▽--%>
												<div id="divFixedPurchaseSearch" runat="server">
												<br/>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">定期台帳抽出検索条件設定</td>
													</tr>
													<tbody id="tbodyFixedPurchaseSearch" runat="server">
													<tr>
														<td class="edit_title_bg" align="left" width="28%">注文区分</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBoxlist id="cblFixedPurchaseOrderKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">並び順</td>
														<td class="edit_item_bg">
															<asp:DropDownList id="ddlFixedPurchaseSortKbn" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">定期購入ステータス</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblFixedPurchaseStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">定期再開予定日</td>
														<td class="edit_item_bg">
															<asp:CheckBox ID="cbResumeDateOn" Text="&nbsp;あり&nbsp;&nbsp;" runat="server" OnCheckedChanged="cbResumeDateOn_OnCheckedChanged" AutoPostBack="true"></asp:CheckBox>
															本日から<asp:TextBox id="tbResumeDateFrom" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以降　～　
															<asp:TextBox id="tbResumeDateTo" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以前の間の定期台帳
															<asp:CheckBox ID="cbResumeDateOff" Text="&nbsp;なし" runat="server"></asp:CheckBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">商品ID<br>※いずれかの商品が購入されたものを抽出</td>
														<td class="edit_item_bg">
															<asp:TextBox id="tbfixedPurchaseItemProductId" runat="server" Width="400"></asp:TextBox>(カンマ区切り)
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">定期購入区分</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblFixedPurchaseKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送種別</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblFixedPurchaseShippingKbn" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">決済種別</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblFixedPurchasePaymentKbn" runat="server" DataTextField="Key" DataValueField="Value"	RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">管理メモ</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblFixedPurchaseManagementMemoFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxlist>
															<br />
															<asp:TextBox ID="tbFixedPurchaseManagementMemo" runat="server" Text='<%# GetSearchSettingValue(Constants.FIELD_ORDER_MANAGEMENT_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT, WorkflowSetting.WorkflowTypes.FixedPurchase) %>' Width="530" Height="40" TextMode="MultiLine" Visible="<%#Constants.ORDERWORKFLOWSETTING_MEMO_TEXTBOX_ENABLED%>"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送メモ</td>
														<td class="edit_item_bg">
															<asp:CheckBoxlist id="cblFpShippingMemoFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
															<br />
															<asp:TextBox ID="tbFpShippingMemo" runat="server" Text='<%# GetSearchSettingValue(Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT, WorkflowSetting.WorkflowTypes.FixedPurchase) %>' Width="530" Height="40" TextMode="MultiLine" Visible="<%#Constants.ORDERWORKFLOWSETTING_MEMO_TEXTBOX_ENABLED%>" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />購入回数<br/>　(注文基準)</td>
														<td class="edit_item_bg">
															<asp:TextBox id="tbFixedPurchaseCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbFixedPurchaseCountTo" runat="server" Width="45" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />購入回数<br/>　(出荷基準)</td>
														<td class="edit_item_bg" colspan="3">
															<asp:TextBox id="tbFixedPurchaseShippedCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbFixedPurchaseShippedCountTo" runat="server" Width="45" />
														</td>
													</tr>
													<%-- ▽頒布会OP有効▽ --%>
													<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
														<tr>
															<td class="edit_title_bg" width="28%">頒布会コースID</td>
															<td class="edit_item_bg">
																<asp:TextBox id="tbFixedPurchaseSubscriptionBoxCourseId" runat="server" Width="125"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />頒布会注文回数</td>
															<td class="edit_item_bg" colspan="3">
																<asp:TextBox id="tbFixedPurchaseSubscriptionBoxOrderCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbFixedPurchaseSubscriptionBoxOrderCountTo" runat="server" Width="45" />
															</td>
														</tr>
													<%} %>
													<%--△ 頒布会OPが有効の場合 ▽--%> 
													<tr>
														<td class="edit_title_bg" align="left" width="28%">作成日</td>
														<td class="edit_item_bg" colspan="5">
															本日から
															<asp:TextBox id="tbCreatedDateTo" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以前の定期台帳&nbsp;&nbsp;
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">更新日</td>
														<td class="edit_item_bg" colspan="5">
															本日から
															<asp:TextBox id="tbUpdatedDateTo" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以前の定期台帳&nbsp;&nbsp;
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">最終購入日</td>
														<td class="edit_item_bg" colspan="5">
															本日から
															<asp:TextBox id="tbLastOrderDateTo" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以前の定期台帳&nbsp;&nbsp;
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">購入開始日</td>
														<td class="edit_item_bg" colspan="5">
															本日から
															<asp:TextBox id="tbFixedPurchaseDateTo" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以前の定期台帳&nbsp;&nbsp;
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">次回配送日</td>
														<td class="edit_item_bg" colspan="5">
															本日から<asp:TextBox id="tbNextShippingDateFrom" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以降　～　
															<asp:TextBox id="tbNextShippingDateTo" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以前の間の定期台帳&nbsp;&nbsp;
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">次々回配送日</td>
														<td class="edit_item_bg" colspan="5">
															本日から<asp:TextBox id="tbNextNextShippingDateFrom" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以降　～　
															<asp:TextBox id="tbNextNextShippingDateTo" runat="server" Width="30" MaxLength="3"></asp:TextBox>日以前の間の定期台帳&nbsp;&nbsp;
														</td>
													</tr>
													<asp:Repeater ID="rFixedPurchaseExtendStatusForSearch" runat="server">
														<ItemTemplate>
															<tr>
																<td class="edit_title_bg" align="left" width="28%">拡張ステータス<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO)%>：
																	<br />&nbsp;<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME)%></td>
																<td class="edit_item_bg">
																	<asp:HiddenField id="hfExtendStatusChangeNo1" runat="server" Value="<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO)%>" />
																	<asp:CheckBox id="cbExtendStatusOn" runat="server" Checked="<%# CheckExtendStatusSame((int)Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO), Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON, WorkflowSetting.WorkflowTypes.FixedPurchase) %>" />オン
																	<asp:CheckBox id="cbExtendStatusOff" runat="server" Checked="<%# CheckExtendStatusSame((int)Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO), Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF, WorkflowSetting.WorkflowTypes.FixedPurchase) %>" />オフ
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">領収書希望フラグ
														<td class="edit_item_bg" colspan="5">
															<asp:CheckBoxList id="cblFixedPurchaseReceiptFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
														</td>
													</tr>
													<% } %>
													<asp:Repeater ID="rFixedPurchaseOrderExtend"
																ItemType="w2.Domain.OrderExtendSetting.OrderExtendSettingModel"
																visible="<%# Constants.ORDER_EXTEND_OPTION_ENABLED %>"
																runat="server">
														<ItemTemplate>
															<tr>
																<td class="edit_title_bg" align="left" width="28%">注文拡張項目:<br /> <%#: Item.SettingName %>(<%#: Item.SettingId %>)
																<td class="edit_item_bg">
																	<asp:CheckBoxList id="cblOrderExtendAttribute"  DataTextField="Text" DataValueField="Value" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
																</td>
															</tr>
															<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingId %>" />
														</ItemTemplate>
													</asp:Repeater>
													</tbody>
												</table>
												</div>
												<%--△ 受注情報抽出検索条件設定 △--%>
												<%--▽ アクション設定 ▽--%>
												<div id="divAction" runat="server">
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="6">アクション設定</td>
													</tr>
													<%--▽ 通常設定：一行表示 ▽--%>
													<tbody id="tbodyOrderAction" runat="server">
													<tr id="trReturn" runat="server">
														<td class="edit_title_bg" align="left" width="28%">返品</td>
														<td class="edit_item_bg" colspan="3">
															<asp:RadioButtonList id="rblReturnOrderAction" runat="server" AutoPostBack="true" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION, WorkflowSetting.WorkflowTypes.Order, Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_NOT_CHANGE) %>' OnSelectedIndexChanged="rblReturnOrderAction_SelectedIndexChanged"></asp:RadioButtonList>
															<br />
															<p id="pReturnActionAreaReasonMemo" runat="server">
																&nbsp;&nbsp;&nbsp;&nbsp;返品理由
																<br />
																&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox id="tbReturnActionReasonMemo" runat="server" TextMode="MultiLine" Text="<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO, WorkflowSetting.WorkflowTypes.Order) %>" Columns="80" Rows="6" />
																<br />
																<asp:RadioButtonList id="rblReturnActionReasonKbn" Runat="server" RepeatDirection="Horizontal"
																	SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN, WorkflowSetting.WorkflowTypes.Order, Constants.FLG_ORDERWORKFLOWSETTING_RETURN_REASON_KBN_CUSTOMER_CONVENIENCE) %>'
																	RepeatLayout="Flow" AutoPostBack="true"></asp:RadioButtonList>
															</p>
														</td>
													</tr>
													<tr id="trOrderStatusChange" runat="server">
														<td class="edit_title_bg" align="left" width="28%">注文ステータス</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblOrderStatusChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# SetStatusChange(GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE, WorkflowSetting.WorkflowTypes.Order)) %>'></asp:RadioButtonList>
															<span runat="server" Visible="<%# OrderCommon.IsInvoiceBundleServiceUsable() %>" style="color: red">
																<br/>
																出荷手配済みに変更した場合は、<%# (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk) ? "DSK" : "Atodene" %>後払い請求書の支払い期限が確定するのでご注意ください。
															</span>
														</td>
													</tr>
													<tr id="trStorePickupStatusAction" runat="server" Visible="false">
														<td class="edit_title_bg" align="left" width="28%">店舗受取ステータス</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblStorePickupStatus"
																runat="server"
																RepeatDirection="Vertical"
																RepeatLayout="Flow" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送希望日</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblShippingDateAction" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION, WorkflowSetting.WorkflowTypes.Order, Constants.FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_OFF) %>' OnSelectedIndexChanged="rblEnabledKbn_SelectedIndexChanged" AutoPostBack="True"></asp:RadioButtonList></br>
															<a href="#note" title="">※このアクションを実行する場合は、抽出検索条件の設定が必要です。詳細は備考欄をご確認ください。</a>
															<asp:Literal runat="server" Visible='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION, WorkflowSetting.WorkflowTypes.Order) != Constants.FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_OFF) %>'></asp:Literal>
														</td>
													</tr>
													<%--▽ 出荷予定日オプションが有効な場合は表示 ▽--%>
													<% if (this.UseLeadTime) { %>
													<tr id="trScheduledShippingDateAction" runat="server">
														<td class="edit_title_bg" align="left" width="28%">出荷予定日</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblScheduledShippingDateAction" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION, WorkflowSetting.WorkflowTypes.Order, Constants.FLG_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION_OFF) %>'></asp:RadioButtonList>
														</td>
													</tr>
													<% } %>
													<%--△ 出荷予定日オプションが有効な場合は表示 △--%>
													<tr id="Tr1" runat="server">
														<td class="edit_title_bg" align="left" width="28%">実在庫連動処理</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblProductRealStockChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE, WorkflowSetting.WorkflowTypes.Order) %>'></asp:RadioButtonList>
														</td>
													</tr>
													<tr id="trPaymentStatusChange" runat="server">
														<td class="edit_title_bg" align="left" width="28%">入金ステータス</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblPaymentStatusChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE, WorkflowSetting.WorkflowTypes.Order) %>'></asp:RadioButtonList>
														</td>
													</tr>
													<tr runat="server" id ="trExternalPaymentAction">
														<td class="edit_title_bg" align="left" width="28%">外部決済連携</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButton ID="rbExternalPaymentActionNone" runat="server" GroupName="ExternalPaymentAction" Text="指定しない" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == "") %>' /><br />
															<%-- 実売上処理が有効な場合 --%>
															<% if (Constants.PAYMENT_CARD_REALSALES_ENABLED){ %>
																<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus){ %>
																<asp:RadioButton ID="rbExternalPaymentActionCardRealSales" runat="server" GroupName="ExternalPaymentAction" Text="Zeus決済連携処理" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZEUS_CREDITCARD_PAYMENT) %>' />&nbsp;&nbsp;※クレジット決済の売上確定処理<br />
																<% } %>
																<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS){ %>
																<asp:RadioButton ID="rbExternalPaymentActionSBPSCreditCardSales" runat="server" GroupName="ExternalPaymentAction" Text="SBPSクレジット売上要求" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_CREDITCARD_SALES) %>' />&nbsp;&nbsp;※SBPSクレジット売上請求確定<br />
																<%} %>
																<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo){ %>
																<asp:RadioButton ID="rbExternalPaymentActionGMOCreditCardSales" runat="server" GroupName="ExternalPaymentAction" Text="GMOクレジット売上要求" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CREDITCARD_SALES) %>' />&nbsp;&nbsp;※GMOクレジット売上請求確定<br />
																<%} %>
																<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zcom){ %>
																	<asp:RadioButton ID="rbExternalPaymentActionZCOMCreditCardSales" runat="server" GroupName="ExternalPaymentAction" Text="Zcomクレジット売上要求" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZCOM_CREDITCARD_SALES) %>' />&nbsp;&nbsp;※Zcomクレジット売上請求確定<br />
																<%} %>
																<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott){ %>
																	<asp:RadioButton ID="rbExternalPaymentActionEScottCreditCardSales" runat="server" GroupName="ExternalPaymentAction" Text="e-SCOTTクレジット売上要求" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ESCOTT_CREDITCARD_SALES) %>' />&nbsp;&nbsp;※e-SCOTTクレジット売上請求確定<br />
																<%} %>
																<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans){ %>
																	<asp:RadioButton ID="rbExternalPaymentActionVeriTransCreditCardSales" runat="server" GroupName="ExternalPaymentAction" Text="ベリトランスクレジット売上要求" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_VERITRANS_CREDITCARD_SALES) %>' />&nbsp;&nbsp;※ベリトランスクレジット売上請求確定<br />
																<%} %>
																<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent){ %>
																	<asp:RadioButton ID="rbExternalPaymentActionPaygentCreditCardSales" runat="server" GroupName="ExternalPaymentAction" Text="Paygentクレジット売上要求" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYGENT_CREDITCARD_SALES) %>' />&nbsp;&nbsp;※paygentクレジット売上請求確定<br />
																<%} %>
															<% } %>
															<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
																<asp:RadioButton ID="rbExternalPaymentActionGMOInvoice" runat="server" GroupName="ExternalPaymentAction" Text="GMO掛け払い" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_INVOICE) %>' />&nbsp;&nbsp;※請求確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_SETTING_SBPS_SOFTBANKKETAI_REALSALES_ENABLED){ %>
															<asp:RadioButton ID="rbExternalPaymentActionSBPSSoftbankKetaiPayment" runat="server" GroupName="ExternalPaymentAction" Text="ソフトバンク・ワイモバイルまとめて支払い(SBPS)" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_SOFTBANKKETAI_PAYMENT) %>' />&nbsp;&nbsp;※ソフトバンク・ワイモバイルまとめて支払いの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_SETTING_SBPS_DOCOMOKETAI_REALSALES_ENABLED){ %>
															<asp:RadioButton ID="rbExternalPaymentActionSBPSDocomoKetaiPayment" runat="server" GroupName="ExternalPaymentAction" Text="ドコモケータイ払い(SBPS)" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_DOCOMOKETAI_PAYMENT) %>' />&nbsp;&nbsp;※ドコモケータイ払いの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_SETTING_SBPS_AUKANTAN_REALSALES_ENABLED){ %>
															<asp:RadioButton ID="rbExternalPaymentActionSBPSAuKantanPayment" runat="server" GroupName="ExternalPaymentAction" Text="ａｕかんたん決済(SBPS)" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_AUKANTAN_PAYMENT) %>' />&nbsp;&nbsp;※ａｕかんたん決済の売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_SETTING_DOCOMOKETAI_REALSALES_ENABLED){ %>
															<asp:RadioButton ID="rbExternalPaymentActionDocomoPayment" runat="server" GroupName="ExternalPaymentAction" Text="ドコモケータイ払い" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_DOCOMO_PAYMENT) %>' />&nbsp;&nbsp;※ドコモケータイ払いの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_SETTING_SBPS_RECRUIT_REALSALES_ENABLED){ %>
															<asp:RadioButton ID="rbExternalPaymentActionSBPSRecruitPayment" runat="server" GroupName="ExternalPaymentAction" Text="リクルートかんたん支払い(SBPS)" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_RECRUIT_PAYMENT) %>' />&nbsp;&nbsp;※リクルートかんたん支払いの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_SETTING_SBPS_RAKUTEN_ID_REALSALES_ENABLED){ %>
															<asp:RadioButton ID="rbExternalPaymentActionSBPSRakutenIdPayment" runat="server" GroupName="ExternalPaymentAction" Text="楽天ペイ(SBPS)" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_RAKUTEN_ID_PAYMENT) %>' />&nbsp;&nbsp;※楽天ペイの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo){ %>
															<asp:RadioButton ID="rbCvsDefGmo" runat="server" GroupName="ExternalPaymentAction" Text="Gmo後払い出荷報告" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_SHIP) %>' />&nbsp;&nbsp;※Gmo後払い決済の出荷報告処理<br />
															<% } %>
															<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene){ %>
																<asp:RadioButton ID="rbCvsDefAtodene" runat="server" GroupName="ExternalPaymentAction" Text="Atodene後払い出荷報告" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATODENE_CVS_DEF_SHIP) %>' />&nbsp;&nbsp;※Atodene後払い決済の出荷報告処理<br />
															<% } %>
															<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk){ %>
																<asp:RadioButton ID="rbCvsDefDsk" runat="server" GroupName="ExternalPaymentAction" Text="DSK後払い出荷報告" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_DSK_CVS_DEF_SHIP) %>' />&nbsp;&nbsp;※DSK後払い決済の出荷報告処理<br />
															<% } %>
															<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom) { %>
																<asp:RadioButton
																	ID="rbCvsDefAtobaraicom"
																	GroupName="ExternalPaymentAction"
																	Text="後払い.com出荷報告"
																	Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATOBARAICOM_CVS_DEF_SHIP) %>'
																	runat="server" />
																&nbsp;&nbsp;※後払い.com決済の出荷報告処理
															<br />
															<% } %>
															<% if (Constants.AMAZON_PAYMENT_OPTION_ENABLED && (Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW == false)){ %>
															<asp:RadioButton ID="rbExternalPaymentActionAmazonPayment" runat="server" GroupName="ExternalPaymentAction" Text="Amazon Pay" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_AMAZON_PAYMENT) %>' />&nbsp;&nbsp;※Amazon Payの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYPAL_LOGINPAYMENT_ENABLED && (Constants.PAYPAL_PAYMENT_METHOD != w2.App.Common.Constants.PayPalPaymentMethod.AUTH_WITH_SUBMIT)) { %>
																<asp:RadioButton ID="rbExternalPaymentActionPayPalPayment" runat="server" GroupName="ExternalPaymentAction" Text="PayPal" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAL_PAYMENT) %>' />&nbsp;&nbsp;※PayPalの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_PAIDY_OPTION_ENABLED) { %>
															<asp:RadioButton ID="rbExternalPaymentActionPaidyPayment" runat="server" GroupName="ExternalPaymentAction" Text="Paidy" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAIDY_PAYMENT) %>' />&nbsp;&nbsp;※Paidyの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_ATONEOPTION_ENABLED) { %>
															<asp:RadioButton ID="rbExternalPaymentActionAtonePayment" runat="server"
																GroupName="ExternalPaymentAction" Text="Atone"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATONE_PAYMENT) %>' />
															&nbsp;&nbsp;※Atoneの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_AFTEEOPTION_ENABLED) { %>
															<asp:RadioButton ID="rbExternalPaymentActionAfteePayment" runat="server"
																GroupName="ExternalPaymentAction" Text="Aftee"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_AFTEE_PAYMENT) %>' />
															&nbsp;&nbsp;※Afteeの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_LINEPAY_OPTION_ENABLED) { %>
															<asp:RadioButton ID="rbExternalPaymentActionLinePayment" runat="server"
																GroupName="ExternalPaymentAction" Text="LINE Pay"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_LINE_PAYMENT) %>' />
															&nbsp;&nbsp;※LINE Payの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED) { %>
															<asp:RadioButton ID="rbExternalPaymentActionNPPayment" runat="server"
																GroupName="ExternalPaymentAction" Text="NP後払い出荷報告"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NP_AFTERPAY_SHIP) %>' />
															&nbsp;&nbsp;※NP後払い決済の出荷報告処理<br />
															<% } %>
															<% if (Constants.ECPAY_PAYMENT_OPTION_ENABLED) { %>
															<asp:RadioButton ID="rbExternalPaymentActionEcPayment" runat="server"
																GroupName="ExternalPaymentAction"
																Text="ECPay"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_EC_PAYMENT) %>' />
															&nbsp;&nbsp;※ECPayの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo) { %>
															<asp:RadioButton
																ID="rbRequestCvsDefInvoiceReissueGmo"
																runat="server"
																GroupName="ExternalPaymentAction"
																Text="後払い請求書再発行"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_INVOICE_REISSUE) %>' />
															&nbsp;&nbsp;※後払い決済の請求書再発行処理<br />
															<% } %>
															<% if (Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED) { %>
															<asp:RadioButton ID="rbExternalPaymentActionNewebPayment" runat="server"
																GroupName="ExternalPaymentAction"
																Text="藍新Pay"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NEWEB_PAYMENT) %>' />
															&nbsp;&nbsp;※藍新Payの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_PAYPAYOPTION_ENABLED) { %>
															<asp:RadioButton ID="rbExternalPaymentActionPayPayPayment" runat="server"
																GroupName="ExternalPaymentAction"
																Text="PayPay"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAY_PAYMENT) %>' />
															&nbsp;&nbsp;※PayPayの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) { %>
															<asp:RadioButton ID="rbExternalPaymentActionRakutenCreditCardSales" runat="server"
																GroupName="ExternalPaymentAction"
																Text="楽天カード　"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_RAKUTEN_CREDITCARD_PAYMENT) %>' />
															&nbsp;&nbsp;※クレジットカードの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_BOKU_OPTION_ENABLED) { %>
															<asp:RadioButton ID="rbExternalPaymentActionBokuPayment" runat="server"
																GroupName="ExternalPaymentAction"
																Text="Boku"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_CARRIERBILLING_BOKU) %>' />
															&nbsp;&nbsp;※Bokuの売上確定処理<br />
															<% } %>
															<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score){ %>
																<asp:RadioButton ID="rbCvsDefScore" runat="server" GroupName="ExternalPaymentAction" Text="スコア後払い出荷報告処理" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SCORE_CVS_DEF_SHIP) %>' /><br />
															<% } %>
															<% if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans){ %>
																<asp:RadioButton ID="rbCvsDefVeritrans" runat="server" GroupName="ExternalPaymentAction" Text="ベリトランス後払い出荷報告処理" Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_VERITRANS_CVS_DEF_SHIP) %>' /><br />
															<% } %>
														</td>
													</tr>
													<tr runat="server" id ="trExternalOrderInfoAction">
														<td class="edit_title_bg" align="left" width="28%">外部受注情報連携</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButton
																ID="rbExternalOrderInfoActionNone"
																GroupName="ExternalOrderInfoAction"
																runat="server"
																Text="指定しない"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION, WorkflowSetting.WorkflowTypes.Order) == "") %>'
																OnCheckedChanged="rbExternalOderInfoAction_OnCheckedChanged"
																AutoPostBack="true" />
															<br />
															<asp:RadioButton
																ID="rbExternalOrderInfoActionECPay"
																GroupName="ExternalOrderInfoAction"
																runat="server"
																Text="ECPay   "
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_ECPAY) %>'
																OnCheckedChanged="rbExternalOderInfoAction_OnCheckedChanged"
																AutoPostBack="true" />
															※ECPayの受注情報処理
															<br />
															<asp:RadioButton
																ID="rbExternalorderInfoNextEngine"
																GroupName="ExternalOrderInfoAction"
																runat="server"
																Text="ネクストエンジン受注情報アップロード"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE) %>'
																OnCheckedChanged="rbExternalOderInfoAction_OnCheckedChanged"
																AutoPostBack="true" />
															<br />
															<asp:RadioButton
																ID="rbExternalOrderInfoActionNextEngineImport"
																GroupName="ExternalOrderInfoAction"
																runat="server"
																Text="ネクストエンジン受注ステータス取込"
																Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE_IMPORT) %>'
																OnCheckedChanged="rbExternalOderInfoAction_OnCheckedChanged"
																AutoPostBack="true" />
															<br />
															<a style="display:<%# Constants.NE_OPTION_ENABLED ? "" : "none" %>;" href="javascript:open_window('<%= HtmlSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NE_ACCESS_TOKEN_REGIST) %>','NeAccessToken','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">※ネクストエンジンアクセストークン取得</a>
															<% if (Constants.CROSS_MALL_OPTION_ENABLED) { %>
															<br />
																<asp:RadioButton
																	ID="rbExternalOderInfoActionCrossMallUpdateStatus"
																	GroupName="ExternalOrderInfoAction"
																	runat="server"
																	Text="CROSS MALL 出荷完了ステータス連携"
																	Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_CROSSMALL_UPDATE_STATUS) %>' 
																	OnCheckedChanged="rbExternalOderInfoAction_OnCheckedChanged"
																	AutoPostBack="true" />
																<br />
																<asp:Label ID="lCrossMallOrderUpdateMessages" Visible='<%# this.rbExternalOderInfoActionCrossMallUpdateStatus.Checked %>' runat="server" style="color:red;padding-left:30px;">
																	CROSS MALL連携の実行対象でなかった場合や連携失敗した場合でも、対象注文に対するその他アクションは実行します。<br />
																</asp:Label>
															<% } %>
															<% if (Constants.RECUSTOMER_API_OPTION_ENABLED) { %>
																<asp:RadioButton
																	ID="rbExternalOrderInfoActionRecustomer"
																	GroupName="ExternalOrderInfoAction"
																	runat="server"
																	Text="Recustomer受注連携 ※注文ステータスが出荷完了の受注にのみ連携を行う"
																	Checked='<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_RECUSTOMER) %>' 
																	OnCheckedChanged="rbExternalOderInfoAction_OnCheckedChanged"
																	AutoPostBack="true" />
															<% } %>
														</td>
													</tr>
													<tr id="trDemandStatusChange" visible="false" runat="server" >
														<td class="edit_title_bg" align="left" width="28%">督促ステータス</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblDemandStatusChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE, WorkflowSetting.WorkflowTypes.Order) %>'></asp:RadioButtonList>
														</td>
													</tr>
													<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">領収書出力フラグ</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblOrderReceiptOutputFlgChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue="<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_RECEIPT_OUTPUT_FLG_CHANGE, WorkflowSetting.WorkflowTypes.Order) %>" />
														</td>
													</tr>
													<% } %>
													<asp:Repeater ID="rOrderExtendStatusForAction" runat="server">
													<ItemTemplate>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">拡張ステータス<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) %>：<br />
															&nbsp;<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME)%></td>
														<td class="edit_item_bg" colspan="5">
															<asp:HiddenField ID="hfExtendStatusChangeNo" runat="server" Value="<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) %>" />
															<asp:RadioButtonList id="rblExtendStatusChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" DataTextField="Text" DataValueField="Value" DataSource="<%# m_licExtendStatusChange %>" SelectedValue="<%# GetOrderWorkflowSettingValue(WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[(int)Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) - 1], WorkflowSetting.WorkflowTypes.Order) %>"></asp:RadioButtonList>
														</td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													</tbody>
													<%--△ 通常設定：一行表示 △--%>
													<%--▽ 通常設定：返品交換注文 ▽--%>
													<tbody id="tbodyReturnExchangeAction" runat="server">
													<tr>
														<td class="edit_title_bg" align="left" width="28%">返品交換ステータス</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblReturnExchangeStatusChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE, WorkflowSetting.WorkflowTypes.Order) %>'></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">返金ステータス</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblRepaymentStatusChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE, WorkflowSetting.WorkflowTypes.Order) %>'></asp:RadioButtonList>
														</td>
													</tr>
													</tbody>
													<tbody id="tbodyInvoiceSatatusAction" runat="server" visible="<%# (this.DisplayKbnValue == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE) %>">
													<tr>
														<td class="edit_title_bg" align="left" width="28%" runat="server" visible="<%# Constants.TWINVOICE_ENABLED %>">電子発票連携</td>
														<td class="edit_item_bg" colspan="5" runat="server" visible="<%# Constants.TWINVOICE_ENABLED %>">
															<asp:RadioButtonList id="rblInvoiceStatusApi"
																runat="server"
																RepeatDirection="Vertical"
																RepeatLayout="Flow"
																SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_API, WorkflowSetting.WorkflowTypes.Order) %>'
																CssClass="radio_button_list" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%" runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo() %>">発票ステータス</td>
														<td class="edit_item_bg" runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo() %>">
															<asp:RadioButtonList id="rblInvoiceStatusChange"
																runat="server"
																RepeatDirection="Vertical"
																RepeatLayout="Flow"
																SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_CHANGE, WorkflowSetting.WorkflowTypes.Order) %>'
																CssClass="radio_button_list" />
														</td>
													</tr>
													</tbody>
													<%--△ 通常設定：返品交換注文 △--%>
													<%--▽ 通常設定：メール設定 ▽--%>
													<tr id="tbodyMailId" runat="server">
														<td class="edit_title_bg" align="left" width="28%">メール送信設定</td>
														<td class="edit_item_bg" colspan="5">
															<asp:DropDownList id="ddlMailId" runat="server" DataTextField="Key" DataValueField="Value"></asp:DropDownList>
														</td>
													</tr>
													<%--△ 通常設定：メール設定 △--%>
													<%--▽ 通常設定：カセット表示 ▽--%>
													<tbody id="tbodyOrderActionCassette" runat="server">
													<tr>
														<td class="edit_title_bg" align="left" width="28%">ステータス分類</td>
														<td class="edit_title_bg" align="center" width="8%">初期選択</td>
														<td class="edit_title_bg" align="center" width="27%">アクション選択項目</td>
														<td class="edit_title_bg" align="center" colspan="3">実行時、メール送信設定</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left">共通</td>
														<td class="edit_item_bg" align="center">
															<input type="radio" id="rbCassetteDefaultSelect" name="CassetteDefaultSelect" checked='<%# GetCassetteDefaultSelectValid("", "", WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %>' /></td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbCassetteOrderDoNothingFlg" runat="server" Checked="<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE, WorkflowSetting.WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_ON) %>" Text="何もしない" />
														</td>
														<td class="edit_item_bg" align="center" colspan="3"></td>
													</tr>
													<asp:Repeater id="rUpdateOrderStatusSettingList" Runat="server">
														<ItemTemplate>
															<tr>
																<td id="Td1" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rUpdateOrderStatusSettingList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">注文ステータス更新区分</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectStatusChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassetteOrderStatusChange" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server" SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater id="rUpdateStorePickupStatusSettingList" Runat="server">
														<ItemTemplate>
															<tr>
																<td id="Td1" class="edit_item_bg"
																	align="left"
																	rowspan="<%# ((Dictionary<string, string>)rUpdateStorePickupStatusSettingList.DataSource).Count %>"
																	visible="<%# Container.ItemIndex == 0 %>"
																	runat="server">店舗受取ステータス</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio"
																		id="rbCassetteDefaultSelectStatusChange<%# Container.ItemIndex %>"
																		name="CassetteDefaultSelect"
																		value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE + Container.ItemIndex %>"
																		<%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox
																		ID="cbStorePickupStatusChange"
																		runat="server"
																		Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>"
																		Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue"
																		runat="server"
																		Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList
																		id="ddlCassetteMailId"
																		runat="server"
																		SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>'
																		DataTextField="Text"
																		DataValueField="Value"
																		DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater id="rUpdateProductRealStockSettingList" Runat="server">
														<ItemTemplate>
															<tr id="Tr1" runat="server">
																<td id="Td2" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rUpdateProductRealStockSettingList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">実在庫連動処理</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteEarlyProductRealStockChangeSelection<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassetteProductRealStockChange" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server"　SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater id="rUpdatePaymentStatusSettingList" Runat="server">
														<ItemTemplate>
															<tr>
																<td id="Td3" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rUpdatePaymentStatusSettingList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">入金ステータス</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectPaymentStatusChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassettePaymentStatusChange" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server"　SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater id="rExternalPaymentActionSettingList" Runat="server">
														<ItemTemplate>
															<tr>
																<td id="Td4" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rExternalPaymentActionSettingList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">外部決済連携</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectExternalPaymentAction<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassetteExternalPaymentAction" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server" SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater id="rExternalOrderInfoActionSettingList" Runat="server">
														<ItemTemplate>
															<tr>
																<td class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rExternalOrderInfoActionSettingList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">外部受注情報連携</td>
																<td class="edit_item_bg" align="center">
																	<input
																		type="radio"
																		id="rbCassetteDefaultSelectExternalOrderInfoAction<%# Container.ItemIndex %>"
																		name="CassetteDefaultSelect"
																		value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION + Container.ItemIndex %>"<%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : string.Empty %> />
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox
																		ID="cbCassetteExternalOrderInfoAction"
																		runat="server"
																		Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>"
																		Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList
																		ID="ddlCassetteMailId"
																		runat="server"
																		SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>'
																		DataTextField="Text"
																		DataValueField="Value"
																		DataSource="<%# m_licMailIds %>"
																		Width="290" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater id="rReturnActionSettingList" Runat="server">
														<ItemTemplate>
															<tr>
																<td class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rReturnActionSettingList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">返品</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectReturnAction<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : string.Empty %> />
																</td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassetteReturnAction" runat="server"
																		Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>"
																		Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>"
																		CssClass="CassetteReturnAction"
																		option="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>"
																		onchange="CassetteReturnActionChange(this);" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																	<div id="CassetteReasonMemo<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" align="left"
																		style="display: none;">
																		&nbsp;&nbsp;返品理由<br/>
																		<asp:TextBox
																			Text="<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO, WorkflowSetting.WorkflowTypes.Order) %>"
																			ID="tbCassetteReturnMemo"
																			TextMode="MultiLine"
																			Rows="3"
																			Width="420"
																			runat="server"></asp:TextBox>
																		<br/>
																		<asp:RadioButtonList
																			ID="rblCassetteReturnKbn"
																			runat="server"
																			RepeatDirection="Horizontal"
																			RepeatLayout="Flow"
																			CssClass="radio_button_list"
																			DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN) %>"
																			DataValueField="Value"
																			DataTextField="Text"
																			SelectedValue="<%# GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN, WorkflowSetting.WorkflowTypes.Order, Constants.FLG_ORDERWORKFLOWSETTING_RETURN_REASON_KBN_CUSTOMER_CONVENIENCE) %>">
																		</asp:RadioButtonList>
																	</div>
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server" SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater id="rUpdateDemandStatusSettingList" visible="false" Runat="server">
														<ItemTemplate>
															<tr>
																<td id="Td5" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rUpdateDemandStatusSettingList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">督促ステータス</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectDemandStatusChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE + Container.ItemIndex %>"<%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassetteDemandStatusChange" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server" SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
													<asp:Repeater id="rReceiptOutputFlgSettingList" Runat="server">
														<ItemTemplate>
															<tr>
																<td class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rReceiptOutputFlgSettingList.DataSource).Count %>"
																	visible="<%# Container.ItemIndex == 0 %>" runat="server">領収書出力フラグ</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectReceiptOutputFlgChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect"
																		value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE + Container.ItemIndex %>"
																		<%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassetteReceiptOutputFlgChange" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>"
																			Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server" SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>'
																		DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<asp:Repeater ID="rOrderExtendStatusSettingList" runat="server">
														<ItemTemplate>
															<asp:HiddenField ID="htExtendStatusNo" runat="server" Value="<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO)%>" />
															<asp:Repeater id="rCassetteOrderExtendStatusChildList" Runat="server" DataSource="<%# m_dicCassetteExtendStatus %>">
																<ItemTemplate>
																	<tr>
																		<td class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)m_dicCassetteExtendStatus).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">拡張ステータス<%# DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) %>：
																		<br />&nbsp;<%# DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME)%></td>
																		<td class="edit_item_bg" align="center">
																			<input type="radio" id="rbCassetteDefaultSelectOrderExtendStatus<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[(int)DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) - 1] + "_" + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[(int)DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) - 1], ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %> /></td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbCassetteOrderExtendStatus" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[(int)DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) - 1], ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																			<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																		</td>
																		<td class="edit_item_bg" align="left" colspan="3">
																			<asp:DropDownList id="ddlCassetteMailId" runat="server" SelectedValue='<%# GetCassetteMailId(WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[(int)DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) - 1], ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																		</td>
																	</tr>
																</ItemTemplate>
															</asp:Repeater>
														</ItemTemplate>
													</asp:Repeater>
													<% if (Constants.TWINVOICE_ENABLED) { %>
													<asp:Repeater id="rUpdateOrderInvoiceStatusCallApiSettingList" Runat="server">
														<ItemTemplate>
															<tr>
																<td class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rUpdateOrderInvoiceStatusCallApiSettingList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">電子発票連携</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectStatusChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : string.Empty %> />
																</td>
																		<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassetteInvoiceStatusApi" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																			<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																		</td>
																		<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server" SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<% if (OrderCommon.DisplayTwInvoiceInfo()) { %>
													<asp:Repeater id="rUpdateOrderInvoiceStatusSettingList" Runat="server">
														<ItemTemplate>
															<tr>
																<td id="Td1" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rUpdateOrderInvoiceStatusSettingList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">発票ステータス</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectStatusChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : string.Empty %> />
																</td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassetteInvoiceStatusChange" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server" SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<% } %>
													</tbody>
													<%--△ 通常ワーク：カセット表示 △--%>
													<%--▽ 返品交換ワーク：カセット表示 ▽--%>
													<tbody id="tbodyCassetteReturnExchangeAction" runat="server">
													<tr>
														<td class="edit_title_bg" align="left" width="28%">セレクトボックス設定</td>
														<td class="edit_title_bg" align="center" width="8%">初期設定</td>
														<td class="edit_title_bg" align="center" width="23%">セレクトボックス表示要否設定</td>
														<td class="edit_title_bg" align="center">実行時送信メール選択</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left">共通</td>
														<td class="edit_item_bg" align="center"><input type="radio" id="rbCassetteDefaultSelectNoneSpecified" name="CassetteDefaultSelect" checked='<%# GetCassetteDefaultSelectValid("", "", WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %>' /></td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbCassetteReturnExchangeDoNothingFlg" runat="server" Checked="<%# (GetOrderWorkflowSettingValue(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE) == Constants.FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_ON) %>" Text="何もしない" />
														</td>
														<td class="edit_item_bg" align="center"></td>
													</tr>
													<asp:Repeater id="rCassetteReturnExchangeStatusChangeList" Runat="server">
														<ItemTemplate>
															<tr>
																<td id="Td7" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rCassetteReturnExchangeStatusChangeList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">返品交換ステータス</td>
																<td class="edit_item_bg" align="center"><input type="radio" id="rbCassetteDefaultSelectReturnExchangeStatusChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassetteReturnExchangeStatusChange" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server" SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater id="rCassetteRepaymentStatusChangeList" Runat="server">
														<ItemTemplate>
															<tr>
																<td id="Td8" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rCassetteRepaymentStatusChangeList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">返金ステータス</td>
																<td class="edit_item_bg" align="center"><input type="radio" id="rbCassetteDefaultSelectRepaymentStatusChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left">
																	<asp:CheckBox ID="cbCassetteRepaymentStatusChange" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:DropDownList id="ddlCassetteMailId" runat="server" SelectedValue='<%# GetCassetteMailId(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.Order) %>' DataTextField="Text" DataValueField="Value" DataSource="<%# m_licMailIds %>" Width="290"></asp:DropDownList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													</tbody>
													<%--△ 返品交換ワーク：カセット表示 △--%>
												</table>
												</div>
												<%--△ アクション設定 △--%>
												<%--▽ 定期台帳アクション設定 ▽--%>
												<div id="divFixedPurchaseAction" runat="server">
												<br/>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="6">アクション設定</td>
													</tr>
													<%--▽ 通常設定：一行表示 ▽--%>
													<tbody id="tbodyFixedPurchaseAction" runat="server">
													<tr>
														<td class="edit_title_bg" align="left" width="28%">定期購入状態変更区分</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList
																id="rblFixedPurchaseIsAliveChange"
																class="radio_button_list"
																onchange="SetVisibilityForFixedPurchaseCancelReason();"
																runat="server"
																RepeatDirection="Vertical"
																RepeatLayout="Flow"
																CssClass="FixedPurchaseIsAliveChange"
																SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE, WorkflowSetting.WorkflowTypes.FixedPurchase, Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_NONE) %>'></asp:RadioButtonList>
															<br />
															<table id="tblCancelReason" style="margin-left: -4%; display: none;">
																<tr>
																	<td style="border: 0; text-align: right;">解約理由</td>
																	<td style="border: 0; text-align: left;">
																		<asp:DropDownList id="ddlCancelReason" DataSource="<%# GetCancelReason() %>" DataTextField="Text" DataValueField="Value" runat="server" Width="450"></asp:DropDownList>
																	</td>
																</tr>
																<tr>
																	<td style="border: 0; text-align: right; width:auto">解約メモ</td>
																	<td style="border: 0; text-align: left;">
																		<asp:TextBox id="tbCancelMemo" Width="442" Height="70" runat="server" Text="<%# GetOrderWorkflowSettingValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO, WorkflowSetting.WorkflowTypes.FixedPurchase) %>" TextMode="MultiLine"></asp:TextBox>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">決済ステータス変更区分</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblFixedPurchasePaymentStatusChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE, WorkflowSetting.WorkflowTypes.FixedPurchase, Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE_ACTION_NONE) %>'></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">次回配送日変更区分</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblFixedPurchaseNextShippingDate" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE, WorkflowSetting.WorkflowTypes.FixedPurchase, Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_SHIPPING_DATE_CHANGE_ACTION_OFF) %>' AutoPostBack="true" OnSelectedIndexChanged="rblFixedPurchaseNextShippingDate_SelectedIndexChanged"></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">次々回配送日変更区分</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblFixedPurchaseNextNextShippingDate" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE, WorkflowSetting.WorkflowTypes.FixedPurchase, Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_NEXT_SHIPPING_DATE_CHANGE_ACTION_OFF) %>'></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="28%">配送不可エリア停止変更</td>
														<td class="edit_item_bg" colspan="5">
															<asp:RadioButtonList id="rblFixedPurchaseStopUnavailableShippingAreaChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="radio_button_list" SelectedValue='<%# GetOrderWorkflowSettingValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE, WorkflowSetting.WorkflowTypes.FixedPurchase, Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE_ACTION_NONE) %>'></asp:RadioButtonList>
														</td>
													</tr>
													<asp:Repeater ID="rFixedPurchaseExtendStatusForAction" runat="server">
														<ItemTemplate>
															<tr>
																<td class="edit_title_bg" align="left" width="28%">拡張ステータス<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) %>：<br />
																	&nbsp;<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME)%></td>
																<td class="edit_item_bg" colspan="5">
																	<asp:HiddenField ID="hfExtendStatusChangeNo" runat="server" Value="<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) %>" />
																	<asp:RadioButtonList id="rblExtendStatusChange" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" DataTextField="Text" DataValueField="Value" DataSource="<%# m_licExtendStatusChange %>" SelectedValue="<%# GetOrderWorkflowSettingValue(WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[(int)Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) - 1], WorkflowSetting.WorkflowTypes.FixedPurchase) %>"></asp:RadioButtonList>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													</tbody>
													<%--△ 通常設定：一行表示 △--%>
													<%--▽ 通常設定：カセット表示 ▽--%>
													<tbody id="tbodyFixedPurchaseActionCassette" runat="server">
													<tr>
														<td class="edit_title_bg" align="left" width="28%">ステータス分類</td>
														<td class="edit_title_bg" align="center" width="8%">初期選択</td>
														<td class="edit_title_bg" align="center" width="27%" colspan="4">アクション選択項目</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left">共通</td>
														<td class="edit_item_bg" align="center">
															<input type="radio" id="rbFixedPurchaseCassetteDefaultSelect" name="CassetteDefaultSelect" checked='<%# GetCassetteDefaultSelectValid("", "", WorkflowSetting.WorkflowTypes.FixedPurchase) ? "checked" : "" %>' /></td>
														<td class="edit_item_bg" align="left" colspan="4">
															<asp:CheckBox ID="cbCassetteFixedPurchaseDoNothingFlg" runat="server" Checked="<%# (GetOrderWorkflowSettingValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE) == Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE_ON) %>" Text="何もしない" />
														</td>
													</tr>
													<asp:Repeater id="rUpdateFixedPurchaseIsAliveList" Runat="server" OnItemDataBound="rUpdateFixedPurchaseIsAliveList_ItemDataBound">
														<ItemTemplate>
															<tr>
																<td id="Td1" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rUpdateFixedPurchaseIsAliveList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">定期購入状態変更</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectFixedPurchaseIsAliveChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.FixedPurchase) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left" colspan="4">
																	<asp:CheckBox
																		ID="cbCassetteFixedPurchaseIsAliveChange"
																		runat="server"
																		option="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>"
																		onchange="SetVisibilityForCassetteFixedPurchaseCancelReason(this);"
																		CssClass="CassetteFixedPurchaseIsAliveChange"
																		Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.FixedPurchase) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																	<br />
																	<table id="tblCancelReason" runat="server" class="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" style="display: none;" >
																		<tr>
																			<td style="border: 0; text-align: right;">解約理由</td>
																			<td style="border: 0; text-align: left;">
																				<asp:DropDownList id="ddlCassetteCancelReason" DataSource="<%# GetCancelReason() %>" DataTextField="Text" DataValueField="Value" runat="server" Width="450"></asp:DropDownList>
																			</td>
																		</tr>
																		<tr>
																			<td style="border: 0; text-align: right; width:auto">解約メモ</td>
																			<td style="border: 0; text-align: left;">
																				<asp:TextBox id="tbCancelMemo" Width="442" Height="70" runat="server" Text="<%# GetOrderWorkflowSettingValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO, WorkflowSetting.WorkflowTypes.FixedPurchase) %>" TextMode="MultiLine"></asp:TextBox>
																			</td>
																		</tr>
																	</table>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater id="rUpdatePaymentStatusList" Runat="server">
														<ItemTemplate>
															<tr>
																<td id="Td1" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rUpdatePaymentStatusList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">決済ステータス変更区分</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectPaymentStatusChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.FixedPurchase) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left" colspan="4">
																	<asp:CheckBox ID="cbCassettePaymentStatusChange" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.FixedPurchase) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:Repeater id="rUpdateStopUnavailableShippingAreaList" Runat="server">
														<ItemTemplate>
															<tr>
																<td id="Td1" class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)rUpdateStopUnavailableShippingAreaList.DataSource).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">配送不可エリア停止変更</td>
																<td class="edit_item_bg" align="center">
																	<input type="radio" id="rbCassetteDefaultSelectStopUnavailableShippingAreaChange<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.FixedPurchase) ? "checked" : "" %> /></td>
																<td class="edit_item_bg" align="left" colspan="4">
																	<asp:CheckBox ID="cbCassetteStopUnavailableShippingAreaChange" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE, ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.FixedPurchase) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																	<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
														<asp:Repeater ID="rFixedPurchaseExtendStatusSettingList" runat="server">
														<ItemTemplate>
															<asp:HiddenField ID="htExtendStatusNo" runat="server" Value="<%# Eval(Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO)%>" />
															<asp:Repeater id="rCassetteFixedPurchaseExtendStatusChildList" Runat="server" DataSource="<%# m_dicCassetteExtendStatus %>">
																<ItemTemplate>
																	<tr>
																		<td class="edit_item_bg" align="left" rowspan="<%# ((Dictionary<string, string>)m_dicCassetteExtendStatus).Count %>" visible="<%# Container.ItemIndex == 0 %>" runat="server">拡張ステータス<%# DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) %>：
																		<br />&nbsp;<%# DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME)%></td>
																		<td class="edit_item_bg" align="center">
																			<input type="radio" id="rbCassetteDefaultSelectOrderExtendStatus<%# Container.ItemIndex %>" name="CassetteDefaultSelect" value="<%# WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[(int)DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) - 1] + "_" + Container.ItemIndex %>" <%# GetCassetteDefaultSelectValid(WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[(int)DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) - 1], ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.FixedPurchase) ? "checked" : "" %> /></td>
																		<td class="edit_item_bg" align="left" colspan="4">
																			<asp:CheckBox ID="cbCassetteFixedPurchaseExtendStatus" runat="server" Checked="<%# GetCassetteOrderStatusChangeValidity(WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[(int)DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO) - 1], ((KeyValuePair<string, string>)Container.DataItem).Key, WorkflowSetting.WorkflowTypes.FixedPurchase) %>" Text="<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>" />
																			<asp:HiddenField ID="hfCassetteActionFieldValue" runat="server" Value="<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>" />
																		</td>
																	</tr>
																</ItemTemplate>
															</asp:Repeater>
														</ItemTemplate>
													</asp:Repeater>
													</tbody>
													<%--△ 通常ワーク：カセット表示 △--%>
												</table>
												</div>
												<%--△ 定期台帳アクション設定 △--%>
                                                <%--▽ 「作成日」「更新日」「更新者」 ▽--%>
                                                <% if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
                                                       || (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)) { %>
												<div runat="server">
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody runat="server">
													<tr>
														<td class="edit_title_bg" align="left" width="15%">作成日</td>
														<td class="edit_item_bg">
															<asp:Literal ID="lDateCreated" runat="server"></asp:Literal>
														</td>
                                                        <td class="edit_title_bg" align="left" width="15%">更新日</td>
														<td class="edit_item_bg">
															<asp:Literal ID="lDateChanged" runat="server"></asp:Literal>
														</td>
                                                        <td class="edit_title_bg" align="left" width="15%">更新者</td>
														<td class="edit_item_bg">
															<asp:Literal ID="lLastChanged" runat="server"></asp:Literal>
														</td>
													</tr>
                                                    </tbody>
                                                </table>
                                                </div>
                                                <% } %>
												<%--△ 「作成日」「更新日」「更新者」 △--%>
												<div class="action_part_bottom">
													<asp:Button id="btnBackBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" OnClientClick="return canExecInsert();" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdate_Click" />
												</div>
												<%--▽ 備考欄 ▽--%>
												<table id="note">
													<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
													<tr>
														<td>
															<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tr class="info_item_bg">
																	<td align="left">備考<br /><br />
																		<dl>
																			<dt><%# Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE ? "出荷予定日・" : "" %>配送希望日指定
																				<%# Constants.FIXEDPURCHASE_OPTION_ENABLED ? "・次回配送日・次々回配送日・定期再開予定日" : ""%>：</dt>

																			<dd style="margin-left:10px;">
																				<p>・テキストボックスが空の場合は、指定のある全ての注文を抽出対象とします。</p>
																				<p>・[To]を入力し、[From]を空にした場合は、当日より過去の注文も抽出対象とします。</p>
																				<p>・出荷予定日・定期再開予定日は抽出時に営業日を考慮します。　※パターンA</p>
																				<p>・配送希望日指定・次回配送日・次々回配送日は抽出時に営業日を考慮せずに休日を含みます。　※パターンB</p>
																				<br/>
																				〇設定例<br />
																				<div style="text-indent: 1rem;">
																					<p>・本日から[ 5]日以降～[　]日以前の間の注文　→　本日より5営業日後～</p>
																					<p>・本日から[　]日以降～[ 5]日以前の間の注文　→　～本日より5営業日後</p>
																					<p>・本日から[ 0]日以降～[ 5]日以前の間の注文　→　本日～本日より5営業日後</p>
																					<p>・本日から[ 5]日以降～[ 5]日以前の間の注文　→　本日よりちょうど5営業日後</p>
																					<p>・本日から[ 5]日以降～[10]日以前の間の注文　→　本日より5営業日後～本日より10営業日後</p>
																				</div><br/>
																				<hr/>
																				<br/>
																				<p>※本日は2021/03/01(月)　営業日は月曜日～金曜日とします。</p><br/>
																				〇具体例（パターンA）<br />
																				<div style="text-indent: 1rem;">
																					<p>・本日から[ 5]日以降～[　]日以前の間の注文　→　2021/03/08(月)～</p>
																					<p>・本日から[　]日以降～[ 5]日以前の間の注文　→　～2021/03/08(月)</p>
																					<p>・本日から[ 0]日以降～[ 5]日以前の間の注文　→　2021/03/01(月)～2021/03/08(月)</p>
																					<p>・本日から[ 5]日以降～[ 5]日以前の間の注文　→　2021/03/08(月)</p>
																					<p>・本日から[ 5]日以降～[10]日以前の間の注文　→　2021/03/08(月)～2021/03/15(月)</p>
																				</div><br />
																				〇具体例（パターンB）<br/>
																				<div style="text-indent: 1rem;">
																					<p>・本日から[ 5]日以降～[　]日以前の間の注文　→　2021/03/06(土)～</p>
																					<p>・本日から[ 5]日以降～[　]日以前の間の注文　→　2021/03/06(土)～</p>
																					<p>・本日から[　]日以降～[ 5]日以前の間の注文　→　～2021/03/06(土)</p>
																					<p>・本日から[ 0]日以降～[ 5]日以前の間の注文　→　2021/03/01(月)～2021/03/06(土)</p>
																					<p>・本日から[ 5]日以降～[ 5]日以前の間の注文　→　2021/03/06(土)</p>
																					<p>・本日から[ 5]日以降～[10]日以前の間の注文　→　2021/03/06(土)～2021/03/11(木)</p>
																				</div><br/>
																				<hr/>
																			</dd>
																		</dl><br/>
																		<dl>
																			<dt>アクション設定「配送希望日」：</dt>
																			<dd style="margin-left:10px;">
																				配送希望日を一括変更する場合は、下記の抽出条件(1)～<%# (Constants.GLOBAL_OPTION_ENABLE == false) ? "(2)" : "(3)" %>をすべて設定してください。<br />
																				<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
																				(1)「注文種別」：定期注文にチェックする<br />
																				　または<br />
																				　
																				<% } else { %>
																				(1)
																				<% } %>
																				「配送種別」：配送種別設定の「配送可能日付範囲利用」が有効となっているもののみチェックする<br />
																				(2)「配送方法」：宅配便にチェックする<br />
																				<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																				(3)「配送先：国」：運用する国に設定する<br />
																				<% } %>
																			</dd>
																		</dl>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
													<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
												</table>
												<%--△ 備考欄 △--%>
											</td>
										</tr>
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
	<!--△ 登録 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<!--▽ Keep form data. It use when come back from Error Page ▽-->
<uc:KeepFormData ID="KeepFormData" runat="server" />
<!--△ Keep form data. It use when come back from Error Page △-->
<script type="text/javascript">
	// Set visibility for reason memo
	function SetVisibilityForReasonMemo() {
		$('.CassetteReturnAction').each(function () {
			var element = $(this).find("input[type='checkbox']");
			CassetteReturnActionChange($(element).parent());
		});
	}

	// Cassette return action change
	function CassetteReturnActionChange(element) {
		// If option = accept return and checkbox has checked
		if (($(element).attr('option') == '<%# Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN %>')
			&& ($(element).find("input[type='checkbox']:checked").length > 0)) {
			// Set display cassette reason memo
			$('#CassetteReasonMemo<%# Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN %>').css('display', '');
		}
		else {
			// Set non-display cassette reason memo
			$('#CassetteReasonMemo<%# Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN %>').css('display', 'none');
		}
	}

	// Set visibility for fixedpurchase cancel reason
	function SetVisibilityForFixedPurchaseCancelReason() {
		$('.FixedPurchaseIsAliveChange > input[type="radio"]:checked').each(function (index) {
			var element = $('.FixedPurchaseIsAliveChange > input[type="radio"]:checked')[index];
			if (element.defaultValue == '<%# Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL %>') {
				document.getElementById("tblCancelReason").style.display = "";
			}
			else {
				document.getElementById("tblCancelReason").style.display = "none";
			}
		});

		// Cassette: Set Value
		$('.CassetteFixedPurchaseIsAliveChange > input[type="checkbox"]:checked').each(function (index) {
			var element = $(this)[index];
			SetVisibilityForCassetteFixedPurchaseCancelReason($(element).parent());
		});
	}

	// Cassette: Set visibility for fixedpurchase cancel reason
	function SetVisibilityForCassetteFixedPurchaseCancelReason(element) {
		var option = $(element).attr('option');
		if (option == '<%# Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL %>') {
			var checked = $(element).find("input[type='checkbox']:checked");
			if (checked.length > 0) {
				$('.<%= Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL %>').css('display', '');
			}
			else {
				$('.<%= Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL %>').css('display', 'none');
			}
		}
	}
</script>

<script type="text/javascript">
// 「配送先：都道府県」の整形処理。（ロードで崩れてしまうので下記のようにしてる。）
$(function () {
	var process = function () {
		<% if (this.IsSearchShippingAddr1) { %>
			// SCRIPTタグの生成
			var el = document.createElement("script");

			// SCRIPTタグのSRC属性に読み込みたいファイルを指定
			el.src = "<%= ResolveUrl("~/Js/prefectures.js") %>";

			// BODY要素の最後に追加
			document.body.appendChild(el);
		<% } %>
	};
	process();
	if (Sys && Sys.Application) { Sys.Application.add_load(process); }
});

	var canInsert = true;

	// Check if can execute insert action
	function canExecInsert() {
		if (canInsert == false) {
			$('#<%= btnInsertTop.ClientID %>').attr('disabled', 'disabled');
			$('#<%= btnInsertBottom.ClientID %>').attr('disabled', 'disabled');
			return false;
		}

		canInsert = false;
		return true;
	}
</script>
</asp:Content>
