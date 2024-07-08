<%--
=========================================================================================================
  Module      : ポイントキャンペーンルール登録ページ(PointRuleCampaignRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PointRuleCampaignRegister.aspx.cs" Inherits="Form_PointRuleCampaign_PointRuleCampaignRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<asp:UpdatePanel runat="server">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">キャンペーン設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">キャンペーン設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">キャンペーン設定登録</h2></td>
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
												<div class="action_part_top"><asp:Button id="btnBackTop" OnClick="btnBack_OnClick" runat="server" Text="戻る"></asp:Button>
													<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " onclick="btnConfirmTop_Click"></asp:Button></div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="3">基本情報</td>
														</tr>
														<tr id="trPointRuleId" runat="server" Visible="False">
															<td class="edit_title_bg" align="left" width="30%">ルールID</td>
															<td class="edit_item_bg" align="left" colspan="2">
																<%# WebSanitizer.HtmlEncode(Input.PointRuleId) %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">キャンペーン名<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="2"><asp:TextBox id="tbPointRuleName" runat="server" Text="<%# Input.PointRuleName %>" MaxLength="30" Width="200"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">ポイント加算区分<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="2"><asp:DropDownList id="ddlPointIncKbn" runat="server" SelectedValue="<%# Input.PointIncKbn %>" OnSelectedIndexChanged="ddlPointIncKbn_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList></td>
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
														<tr>
															<td class="edit_title_bg" align="left" width="30%">ポイント加算ルール<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbIncPoint" runat="server" Text='<%# this.Input.IncType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? this.Input.IncNum : this.Input.IncRate %>' Width="100" MaxLength="7"></asp:TextBox>
																<asp:DropDownList id="ddlPointIncType" runat="server"></asp:DropDownList>
																<span id="dvFixedPurchaseSetting" runat="server"><br />
																<asp:CheckBox id="cbShowFixedPurchasePointSetting" runat="server" Checked="<%# (this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM) || (this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE) %>" AutoPostBack="true" Text="定期ポイント加算ルール設定する" /><br />
																<% if (cbShowFixedPurchasePointSetting.Checked)
																{ %>
																	<asp:TextBox id="tbIncFixedPurchasePoint" runat="server" Text='<%# this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? this.Input.IncFixedPurchaseNum : this.Input.IncFixedPurchaseRate %>' Width="100" MaxLength="7"></asp:TextBox>
																	<asp:DropDownList id="ddlFixedPurchasePointIncType" runat="server"></asp:DropDownList>
																<% } %>
																</span>
															</td>
														</tr>
														<tr id="trLimitedTermPointExpiration" runat="server">
															<td class="edit_title_bg" align="left" width="30%">期間限定ポイント有効期限<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButton ID="rbLimitedTermPointExpirationDay" CssClass="radio_button_list" GroupName="ExpireKbn" Text="発行日から期限を指定する" AutoPostBack="True" Checked="<%# string.IsNullOrEmpty(this.Input.PeriodBegin) %>" OnCheckedChanged="ExpireKbn_CheckedChanged" Runat="server" ></asp:RadioButton><br/>
																<p style="margin: 0 15px;" ID="pLimitedTermPointExpirationDay" runat="server">
																	発行の<asp:TextBox ID="tbEffectiveOffset" Width="40" Text="<%# this.Input.EffectiveOffset %>" runat="server" />
																	<asp:DropDownList ID="ddlEffectiveOffsetType" SelectedValue="<%# string.IsNullOrEmpty(this.Input.EffectiveOffsetType) ? Constants.FLG_POINTRULE_EFFECTIVE_OFFSET_TYPE_DAY : this.Input.EffectiveOffsetType %>" runat="server"/>から
																	<asp:TextBox ID="tbTerm" Width="40" Text="<%# this.Input.Term %>" runat="server" />
																	<asp:DropDownList ID="ddlTermType" SelectedValue="<%# string.IsNullOrEmpty(this.Input.TermType) ? Constants.FLG_POINTRULE_TERM_TYPE_DAY : this.Input.TermType %>" runat="server"/>有効
																</p>
																<asp:RadioButton ID="rbLimitedTermPointPeriod" CssClass="radio_button_list" GroupName="ExpireKbn" Text="有効期間を指定する" AutoPostBack="True" Checked="<%# string.IsNullOrEmpty(Input.PeriodBegin) == false %>" OnCheckedChanged="ExpireKbn_CheckedChanged" Runat="server" ></asp:RadioButton><br/>
																<p style="margin: 0 15px;" ID="pLimitedTermPointPeriod" runat="server">
																	<uc:DateTimePickerPeriodInput ID="ucPeriod" runat="server" />
																</p>
															</td>
														</tr>
														<tr id="trPointExpEntend" runat="server">
															<td class="edit_title_bg" align="left" width="30%">ポイント有効期限の延長設定<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="2">
																<% if (Constants.CROSS_POINT_OPTION_ENABLED == false) { %>
																<div>
																	<asp:DropDownList id="ddlPointExpEntend" runat="server" SelectedValue='<%# (string.IsNullOrEmpty(StringUtility.ToEmpty(Input.PointExpExtend)) == false) ? Input.PointExpExtendMonth : "00" %>' />ヶ月延長
																</div>
																<% } else { %>
																<asp:Label runat="server" class="notice" Text="<%#: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CHECK_THE_EXPIRATION_DATE_ON_THE_CROSSPOINT_MANAGEMENT_SCREEN) %>" />
																<% } %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">キャンペーン有効期間<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="2">
																<p>
																	<uc:DateTimePickerPeriodInput ID="ucExpire" runat="server" IsHideTime="true" />
																	&nbsp;&nbsp;
																	<asp:Button id="btnReflect" Text="  反映する  " Runat="server" onclick="btnReflect_Click"></asp:Button>
																</p>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%" rowSpan="2">キャンペーン有効期間詳細設定<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="2"><asp:RadioButton id="rbCampaignTermKbn1" runat="server" GroupName="rbCampaignTermKbn" Checked='<%# StringUtility.ToEmpty(Input.CampaignTermKbn) == "" ? true : false %>' OnCheckedChanged="rbCampaignTermKbn_CheckedChanged" AutoPostBack="True"></asp:RadioButton>全体&nbsp;&nbsp;
																<asp:RadioButton id="rbCampaignTermKbn2" Runat="server" GroupName="rbCampaignTermKbn" Checked='<%# StringUtility.ToEmpty(Input.CampaignTermKbn) == Constants.FLG_POINTRULE_CAMPAIGN_TERM_KBN_MONTH ? true : false %>' OnCheckedChanged="rbCampaignTermKbn_CheckedChanged" AutoPostBack="True"></asp:RadioButton>毎月&nbsp;&nbsp;
																<asp:DropDownList id="ddlCampaignTermValue1" runat="server" SelectedValue='<%# StringUtility.ToEmpty(Input.CampaignTermKbn) == Constants.FLG_POINTRULE_CAMPAIGN_TERM_KBN_MONTH ? StringUtility.ToEmpty(Input.CampaignTermValue) : "" %>'></asp:DropDownList>日
																<asp:RadioButton id="rbCampaignTermKbn3" Runat="server" GroupName="rbCampaignTermKbn" Checked='<%# StringUtility.ToEmpty(Input.CampaignTermKbn) == Constants.FLG_POINTRULE_CAMPAIGN_TERM_WEEK ? true : false %>' OnCheckedChanged="rbCampaignTermKbn_CheckedChanged" AutoPostBack="True"></asp:RadioButton>毎週&nbsp;&nbsp;
																<asp:DropDownList id="ddlCampaignTermValue2" runat="server" SelectedValue='<%# StringUtility.ToEmpty(Input.CampaignTermKbn) == Constants.FLG_POINTRULE_CAMPAIGN_TERM_WEEK ? StringUtility.ToEmpty(Input.CampaignTermValue) : "" %>'></asp:DropDownList>曜日
																<asp:RadioButton id="rbCampaignTermKbn4" Runat="server" GroupName="rbCampaignTermKbn" Checked='<%# StringUtility.ToEmpty(Input.CampaignTermKbn) == Constants.FLG_POINTRULE_CAMPAIGN_TERM_KBN_EVERY_OTHER_WEEK ? true : false %>' OnCheckedChanged="rbCampaignTermKbn_CheckedChanged" AutoPostBack="True"></asp:RadioButton>隔週&nbsp;&nbsp;
																<asp:DropDownList id="ddlCampaignTermValue3" runat="server" SelectedValue='<%# StringUtility.ToEmpty(Input.CampaignTermKbn) == Constants.FLG_POINTRULE_CAMPAIGN_TERM_KBN_EVERY_OTHER_WEEK ? StringUtility.ToEmpty(Input.CampaignTermValue) : "" %>'></asp:DropDownList>曜日&nbsp;&nbsp;
																<asp:Button id="btnReflect2" Text="  反映する  " Runat="server" onclick="btnReflect2_Click"></asp:Button>
															</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="center">
																<table cellspacing="0" cellpadding="0" border="0">
																	<asp:Repeater id="rCampaign" Runat="server" DataSource="<%# m_alCampaign %>">
																		<ItemTemplate>
																			<%# (int)((Hashtable)Container.DataItem)[CAMPAIGN_CALENDAR_DATE_NO] % 2 == 1 ? "<tr>":"" %>
																			<td class="edit_item_bg" align="center">
																				<asp:Calendar id="cldCampaign1" runat="server" Width="224px" OnSelectionChanged="cldCampaign1_SelectionChanged" OnDayRender="cldCampaign1_DayRender" VisibleDate="<%# (DateTime)((Hashtable)Container.DataItem)[CAMPAIGN_CALENDAR_DATE] %>" ShowNextPrevMonth="False" CellPadding="3" CellSpacing="1" ForeColor="#0864AA" BorderColor="#ece9d8" CssClass="calendar_control">
																				<TitleStyle CssClass="calendar_title"></TitleStyle>
																				<DayHeaderStyle CssClass="calendar_dayheader_bg"></DayHeaderStyle>
																				<OtherMonthDayStyle ForeColor="LightGray"></OtherMonthDayStyle>
																				</asp:Calendar>
																			</td>
																			<%# (int)((Hashtable)Container.DataItem)[CAMPAIGN_CALENDAR_DATE_NO] % 2 == 0 ? "</tr>":"" %>
																		</ItemTemplate>
																	</asp:Repeater></table>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">基本ルールとの二重適用</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBox ID="cbAllowDuplicateApply" Text="許可する" Checked="<%# this.Input.AllowDuplicateApplyFlg == Constants.FLG_POINTRULE_DUPLICATE_APPLY_ALLOW %>" runat="server"/>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">優先順位<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="2">
																<asp:TextBox ID="tbPriority" runat="server" Width="25" MaxLength="2" Text='<%# StringUtility.ToEmpty(Input.Priority) != "" ? Input.Priority : "99" %>'></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
															<td class="edit_item_bg" align="left" colspan="2"><asp:checkbox id="cbValidFlg" Checked='<%# StringUtility.ToEmpty(Input.ValidFlg) == "" ? false : Input.ValidFlg == Constants.FLG_POINTRULE_VALID_FLG_VALID ? true : false %>' Runat="server" Text="有効">
																</asp:checkbox></td>
														</tr>
													</tbody>
												</table>
												<br />
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tbody>
														<tr>
															<td align="left" class="info_item_bg" colspan="2">備考（各項目の説明）<br />
																■ポイントキャンペーンルール情報<br />
																・ポイント加算ルール・・・加算するポイント数/率を指定<br />
																　※ただし「購入時ポイント発行」はキャンペーンの場合に、商品毎の設定はできません。商品マスタで管理して下さい。<br />
																・ポイント有効期限延長 ・・・ポイント発行時のユーザーポイント有効期限を指定(1～36ヶ月)<br />
																・ルール有効期間・・・ルールを有効にする期間を指定　設定例）<%: DateTime.Now.Year %>年中の有効なルールを指定する場合は、<%: DateTimeUtility.ToStringForManager(new DateTime(DateTime.Now.Year, 1, 1), DateTimeUtility.FormatType.LongDate1Letter) %>
																～<%: DateTimeUtility.ToStringForManager(new DateTime(DateTime.Now.Year, 12, 31), DateTimeUtility.FormatType.LongDate1Letter) %>を設定する。<br />
																・優先順位 ・・・ポイントルールの優先順位を指定<br />
																<br />
																※1.ポイントの指定は『ポイント加算区分』が『購入時ポイント発行』、『初回購入時ポイント発行』選択時は両方指定可能、<br />
																『新規登録ポイント発行』、『ログイン毎ポイント発行』選択時は『加算数』のみ指定可能です。<br />
																※2.ポイント(加算率)の場合、パーセント指定です｡(例)購入金額の25%をポイントにしたい場合は、『25』と指定。<br />
																また、小数点以下のポイントは切り捨てです。<br />
																※3. 延長設定は、該当ユーザーが現在所持しているポイントの有効期限よりも長い場合に適用されます。<br />
																　　　(例1) 現在所持しているポイントの有効期限が6ヶ月後で、延長設定を1ヶ月とした場合、有効期限に変化はなく、6ヶ月後となります。<br />
																　　　(例2) 現在所持しているポイントの有効期限が6ヶ月後で、延長設定を12ヶ月とした場合、有効期限は現在時点から12ヶ月後となります。<br />
																　　　(例3) 初回購入ポイントで1ヶ月、購入時ポイントで6ヶ月と設定されていて、双方が適用される場合<br />
																　　　　　　 期限が長い方を優先するため、6ヶ月が適用されます。<br />
																<div ID="divReviewPoint" runat="server">
																	<br/>■レビュー投稿時ポイントについて
																	<br/>・レビュー投稿時ポイントは公開タイミングがキャンペーン期間中に適用されます。
																</div>
															</td>
														</tr>
													</tbody>
												</table>
												<div class="action_part_bottom"><asp:Button id="btnBackbottom" OnClick="btnBack_OnClick" runat="server" Text="戻る"></asp:Button>
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " onclick="btnConfirmTop_Click"></asp:Button></div>
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
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>