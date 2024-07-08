<%--
=========================================================================================================
  Module      : ポイントルール登録ページ(PointRuleRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PointRuleRegister.aspx.cs" Inherits="Form_PointRule_PointRuleRegister" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">基本ルール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">基本ルール設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">基本ルール設定登録</h2></td>
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
													<input type="button" onclick="Javascript: history.back();" value="  戻る  " />
													<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " onclick="btnConfirmTop_Click"></asp:Button>
												</div>
												<asp:UpdatePanel ID="UpdatePanel1" runat="server">
												<ContentTemplate>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr id="trPointRuleId" runat="server" Visible="False">
															<td class="edit_title_bg" align="left" width="30%">ルールID</td>
															<td class="edit_item_bg" align="left">
																<%# WebSanitizer.HtmlEncode(Input.PointRuleId) %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">ルール名<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbPointRuleName" runat="server" Text="<%# Input.PointRuleName %>" Width="200" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">ポイント加算区分<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList id="ddlPointIncKbn" runat="server" SelectedValue="<%# Input.PointIncKbn %>" OnSelectedIndexChanged="ddlPointIncKbn_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
																<% if (ddlPointIncKbn.SelectedValue == Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK) {%>
																	<br />特定URLクリック時に発行されるポイントの設定です
																<% }else if(ddlPointIncKbn.SelectedValue == Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE){ %>
																	<br />ポイントルールスケジュールでご利用いただけるポイントの設定です
																<% }else if(ddlPointIncKbn.SelectedValue == Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT){ %>
																	<br />ポイントルールスケジュールでご利用いただけるポイントの設定です
																	<br />※1年に1度しか付与されないようになっています
																<%} %>
															</td>
														</tr>
														<tr id="trPointKbn" runat="server">
															<td class="edit_title_bg" align="left" width="30%">ポイント区分<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButton ID="rbPointKbnNormalPoint" GroupName="PointKbn" Text="通常ポイント" AutoPostBack="True" OnCheckedChanged="PointKbn_OnCheckedChanged" Runat="server" Checked="<%# this.Input.IsLimitedTermPoint == false %>"></asp:RadioButton>
																<% if (Constants.CROSS_POINT_OPTION_ENABLED == false) { %>
																<asp:RadioButton ID="rbPointKbnLimitedTermPoint" GroupName="PointKbn" Text="期間限定ポイント" AutoPostBack="True" OnCheckedChanged="PointKbn_OnCheckedChanged" Runat="server" Checked="<%# this.Input.IsLimitedTermPoint %>"></asp:RadioButton>
																<% } %>
															</td>
														</tr>
														<tr id="trBuyIncKbn" runat="server">
															<td class="edit_title_bg" align="left" width="30%">ポイント計算方法<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButton ID="rbBuyIncKbnPrice" runat="server" CssClass="radio_button_list" Text="購入金額毎" GroupName="rblBuyIncKbnPrice" Checked="<%# Input.IncType != Constants.FLG_POINTRULE_INC_TYPE_PRODUCT %>" OnCheckedChanged="rbBuyIncKbn_CheckedChanged" AutoPostBack="True" />
																<asp:RadioButton ID="rbBuyIncKbnProduct" runat="server" CssClass="radio_button_list" Text="商品毎" GroupName="rblBuyIncKbnPrice" Checked="<%# Input.IncType == Constants.FLG_POINTRULE_INC_TYPE_PRODUCT %>" OnCheckedChanged="rbBuyIncKbn_CheckedChanged" AutoPostBack="True" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">ポイント加算ルール<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbIncPoint" runat="server" Text='<%# (this.Input.IncType == Constants.FLG_POINTRULE_INC_TYPE_NUM) ? this.Input.IncNum : ((this.Input.IncType == Constants.FLG_POINTRULE_INC_TYPE_RATE) ? this.Input.IncRate : "") %>' Width="100" MaxLength="7"></asp:TextBox>
																<asp:DropDownList id="ddlPointIncType" runat="server"/>
																<span id="dvFixedPurchaseSetting" runat="server"><br />
																<asp:CheckBox id="cbShowFixedPurchasePointSetting" runat="server"  Checked="<%# (this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM) || (this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE) %>" AutoPostBack="true" Text="定期ポイント加算ルール設定する" /><br />
																<% if (cbShowFixedPurchasePointSetting.Checked) { %>
																	<asp:TextBox id="tbIncFixedPurchasePoint" runat="server" Text='<%# this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? this.Input.IncFixedPurchaseNum : this.Input.IncFixedPurchaseRate %>' Width="100" MaxLength="7"></asp:TextBox>
																	<asp:DropDownList id="ddlFixedPurchasePointIncType" runat="server"></asp:DropDownList>
																<% } %>
																</span>
															</td>
														</tr>
														<tr id="trLimitedTermPointExpiration" runat="server" style="display: none;">
															<td class="edit_title_bg" align="left" width="30%">期間限定ポイント有効期限<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButton ID="rbLimitedTermPointExpirationDay" CssClass="radio_button_list" GroupName="ExpireKbn" Text="発行日から期限を指定する" AutoPostBack="True" Checked="<%# string.IsNullOrEmpty(Input.PeriodBegin) %>" OnCheckedChanged="ExpireKbn_CheckedChanged" Runat="server" ></asp:RadioButton><br/>
																<p style="margin: 0 15px;" ID="pLimitedTermPointExpirationDay" runat="server">
																	発行の<asp:TextBox ID="tbEffectiveOffset" Width="40" Text="<%# this.Input.EffectiveOffset %>" runat="server" />
																	<asp:DropDownList ID="ddlEffectiveOffsetType" SelectedValue="<%# string.IsNullOrEmpty(this.Input.EffectiveOffsetType) ? Constants.FLG_POINTRULE_EFFECTIVE_OFFSET_TYPE_DAY : this.Input.EffectiveOffsetType %>" runat="server"/>から
																	<asp:TextBox ID="tbTerm" Width="40" Text="<%# this.Input.Term %>" runat="server" />
																	<asp:DropDownList ID="ddlTermType" SelectedValue="<%# string.IsNullOrEmpty(this.Input.TermType) ? Constants.FLG_POINTRULE_TERM_TYPE_DAY : this.Input.TermType %>" runat="server"/>有効
																</p>
																<asp:RadioButton ID="rbLimitedTermPointPeriod" CssClass="radio_button_list" GroupName="ExpireKbn" Text="有効期間を指定する" AutoPostBack="True" Checked="<%# string.IsNullOrEmpty(Input.PeriodBegin) == false %>" OnCheckedChanged="ExpireKbn_CheckedChanged" Runat="server" ></asp:RadioButton><br/>
																<span style="margin: 0 15px;" ID="spLimitedTermPointPeriod" runat="server">
																	<uc:DateTimePickerPeriodInput ID="ucPeriod" runat="server" />
																</span>
															</td>
														</tr>
														<tr id="trPointExpEntend" runat="server">
															<td class="edit_title_bg" align="left" width="30%">ポイント有効期限の延長設定<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<% if (Constants.CROSS_POINT_OPTION_ENABLED == false) { %>
																<div>
																	<asp:DropDownList id="ddlPointExpEntend" runat="server" SelectedValue='<%# (string.IsNullOrEmpty(StringUtility.ToEmpty(Input.PointExpExtend)) == false) ? Input.PointExpExtendMonth : string.Empty %>' />ヶ月延長
																</div>
																<% } else { %>
																<asp:Label runat="server" class="notice" Text="<%#: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CHECK_THE_EXPIRATION_DATE_ON_THE_CROSSPOINT_MANAGEMENT_SCREEN) %>" />
																<% } %>
															</td>
														</tr>
														<tr id="trPointExpire" runat="server">
															<td class="edit_title_bg" align="left" width="30%">ルール有効期間<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<uc:DateTimePickerPeriodInput ID="ucExpireDatePeriod" runat="server" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">優先順位</td>
															<td class="edit_item_bg" align="left">
																100
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBox ID="cbValidFlg" Runat="server" Checked='<%# StringUtility.ToEmpty(Input.ValidFlg) == "" ? false : Input.ValidFlg == Constants.FLG_POINTRULE_VALID_FLG_VALID ? true : false %>' Text="有効"></asp:CheckBox>
															</td>
														</tr>
													</tbody>
												</table>
												<br />
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考（各項目の説明）<br />
															■ポイント基本ルール情報<br />
															・ルール名<br />
															　ポイントルールの名称を設定します。<br />
															<br />
															・ポイント加算区分<br />
															　ポイント付与条件を指定します。
															<br />
															<br />
															・ポイント区分<br />
															　ポイント区分を指定します。<br />
															<br />
															・ポイント計算方法<br />
															　購入時に加算されるポイントの計算方法を指定します。<br />
															　「商品毎」を指定した場合、商品管理にてポイント設定が必要です。<br />
															　※ポイント加算区分が「購入時ポイント発行」選択時のみ表示します。<br />
															<br />
															・ポイント加算ルール<br />
															　加算するポイントのポイント数/率を選択し、付与数/率を設定します。<br />
															　※加算率はポイント加算区分が「購入時ポイント発行」および「初回購入時ポイント発行」選択時のみ有効です。<br />
															　※小数点以下のポイントは切り捨てされます。<br />
															<br />
															・期間限定ポイント有効期限<br />
															　発行する期間限定ポイントの有効期限を設定します。<br />
															　例）発行したその月のみ使用できるポイントを発行する場合、「発行の0ヶ月後の月初から1ヶ月間有効」と設定してください。<br/>
															　ポイント加算区分が「初回購入ポイント発行」「購入時ポイント発行」の場合、<br />
															　「有効期間を指定する」を利用することはできません。
															<br />
															<br />
															・ポイント有効期限の延長設定<br />
															　ポイント発行時のユーザーポイント有効期限を指定します。（0～36カ月）<br />
															<br />
															・ルール有効期間<br />
															　ルールを有効にする期間を指定します。<br />
															　例）<%#: DateTime.Now.Year %>年中の有効なルールを指定する場合は、<%#: DateTimeUtility.ToStringForManager(new DateTime(DateTime.Now.Year, 1, 1), DateTimeUtility.FormatType.LongDate1Letter) %>
															～<%#: DateTimeUtility.ToStringForManager(new DateTime(DateTime.Now.Year, 12, 31), DateTimeUtility.FormatType.LongDate1Letter) %>を設定してください。<br />
															<br />
															・優先順位<br />
															　適用されるルールの優先順位を設定します。<br />
															　※ポイント基本ルール設定の場合は「100」固定です。<br />
															<br />
															・有効フラグ<br />
															　ポイントルールの有効/無効を指定します。<br />
															<br />
															■補足<br />
															<% if (Constants.MARKETINGPLANNER_USE_TEMPORARY_POINT) { %>
																・購入時に期間限定ポイントを発行する場合、購入時点で有効期限は計算されず、本ポイントへの移行時に計算されます。<br />
															<% } %>
															・「初回購入ポイント発行」と「購入時ポイント発行」は重複してポイント付与されます。<br />
															・「ポイント有効期限の延長設定」は通常本ポイントに対して適用されます。<br />
															　「ポイント区分」が期間限定ポイントの場合でも、通常ポイントの期間延長を行うことができます。<br />
															・通常ポイントの有効期限は適用される最長の日付となります。<br />
															　例1)<br />
															　　現在の有効期限が<%#: DateTimeUtility.ToStringForManager(DateTime.Now.AddMonths(2), DateTimeUtility.FormatType.LongDate2Letter) %>、購入時1カ月延長で<%#: DateTimeUtility.ToStringForManager(DateTime.Now.AddMonths(1), DateTimeUtility.FormatType.LongDate2Letter) %>となる場合、<br />
															　　有効期限は変わらず<%#: DateTimeUtility.ToStringForManager(DateTime.Now.AddMonths(2), DateTimeUtility.FormatType.LongDate2Letter) %>となります。<br />
															　例2)<br />
															　　現在の有効期限が<%#: DateTimeUtility.ToStringForManager(DateTime.Now.AddMonths(2), DateTimeUtility.FormatType.LongDate2Letter) %>、購入時12カ月延長で<%#: DateTimeUtility.ToStringForManager(DateTime.Now.AddMonths(12), DateTimeUtility.FormatType.LongDate2Letter) %>となる場合、<br />
															　　有効期限は延長後の<%#: DateTimeUtility.ToStringForManager(DateTime.Now.AddMonths(12), DateTimeUtility.FormatType.LongDate2Letter) %>となります。<br />
															　例3)<br />
															　　「初回購入ポイント発行」購入時2カ月延長で<%#: DateTimeUtility.ToStringForManager(DateTime.Now.AddMonths(2), DateTimeUtility.FormatType.LongDate2Letter) %>、<br />
															　　「購入時ポイント発行」購入時6カ月延長で<%#: DateTimeUtility.ToStringForManager(DateTime.Now.AddMonths(6), DateTimeUtility.FormatType.LongDate2Letter) %>となる場合、<br />
															　　より長い有効期限の<%#: DateTimeUtility.ToStringForManager(DateTime.Now.AddMonths(6), DateTimeUtility.FormatType.LongDate2Letter) %>となります。<br />
															　　※ただし、現在の有効期限のほうが長い場合は有効期限は変わりません。(例1参照)
															<br />
														</td>
													</tr>
												</table>
												</ContentTemplate>
												</asp:UpdatePanel>
												<div class="action_part_bottom">
													<input type="button" onclick="Javascript: history.back();" value="  戻る  " />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " onclick="btnConfirmTop_Click"></asp:Button>
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
	<!--△ 登録 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>