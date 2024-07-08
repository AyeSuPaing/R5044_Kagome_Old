<%--
=========================================================================================================
  Module      : ターゲットリスト設定登録ページ(TargetListRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="TargetListRegister.aspx.cs" Inherits="Form_TargetList_TargetListRegister" MaintainScrollPositionOnPostback="true"%>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Register TagPrefix="uc" TagName="ScheduleRegisterForm" Src="~/Form/Common/ScheduleRegisterForm.ascx" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="w2.App.Common.TargetList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
// 選択されたターゲットリストテンプレートの情報を画面に設定
function applyTargetListTemplate(templateId)
{
	$('#' + '<%= hfTargetListTemplateId.ClientID %>').val(templateId);
	__doPostBack('ctl00$ContentPlaceHolderBody$hfTargetListTemplateId', '');
}
//-->

// グループ化ボタン活性化、非活性化
	$(function() {
		$('input:checkbox').change(function() {
			var cnt = $('.cb_count input:checkbox:checked').length;
			if (cnt <= 1) {
				$('.make_group').prop("disabled", true);
			} else {
				$('.make_group').prop("disabled", false);
			}
		}).trigger('change');
	});
</script>
<asp:HiddenField ID="hfTargetListTemplateId" runat="server" Value="" OnValueChanged="hfTargetListTemplateId_ValueChanged" />
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ターゲットリスト情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr>
		<% if ((m_strActionKbn == Constants.ACTION_STATUS_INSERT) || (m_strActionKbn == Constants.ACTION_STATUS_COPY_INSERT)){%>
		<td><h2 class="cmn-hed-h2">ターゲットリスト情報登録</h2></td>
		<%} %>
		<% if (m_strActionKbn == Constants.ACTION_STATUS_UPDATE){%>
		<td><h2 class="cmn-hed-h2">ターゲットリスト情報編集</h2></td>
		<%} %>
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
                                                    <asp:Button ID="btnBackTop" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<input type="button" value="  テンプレート一覧  " onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TARGETLIST_TEMPLATELIST) %>','contact','width=950,height=600,top=120,left=420,status=NO,scrollbars=yes');"  />
													<asp:Button ID="btnConfirmTop" Text="  確認する  " runat="server" OnClick="btnConfirm_Click" />
												</div>
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr id="trTargetId" runat="server">
														<td align="left" class="search_title_bg" width="18%" style="height: 19px">ターゲットID</td>
														<td align="left" class="search_item_bg" style="height: 19px"><asp:Label Id="lbTargetId" runat="server"></asp:Label></td>
													</tr>
													<tr>
														<td align="left" class="search_title_bg">ターゲットリスト名<span class="notice">*</span></td>
														<td align="left" class="search_item_bg"><asp:TextBox ID="tbTargetListName" Width="300" runat="server"></asp:TextBox></td>
													</tr>
												</table>
												<br />
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="search_title_bg" width="695">条件
															<span class="notice">*</span>
															<asp:Button class="make_group" id="btnMakeGroup" Text="  グループ化  " Disabled="true" runat="server" OnClick="btnMakeGroup_Click" />
														</td>
														<td align="center" class="search_title_bg" width="63"><asp:Button id="btnAdd" Text="  追加  " runat="server" OnClick="btnAdd_Click" /></td>
													</tr>
													<asp:Repeater ID="rConditions" runat="server">
														<ItemTemplate>
													
														<tr>
															<td colspan="2" align="left" class="search_item_bg" runat="server" Visible="<%# Container.ItemIndex != 0 %>">
																<asp:DropDownList Height="30" ID="ddlConditionType" AutoPostBack="true" Visible="<%# Container.ItemIndex == 1 %>" OnSelectedIndexChanged="ddlAllCondition_OnSelectedIndexChanged" runat="server">
																	<asp:ListItem Value="AND">かつ</asp:ListItem>
																	<asp:ListItem Value="OR">または</asp:ListItem>
																</asp:DropDownList>
																<span style="font-size: 125%;">
																<asp:Literal Text='<%#: (((ITargetListCondition)Container.DataItem).GetConditionType((ITargetListCondition)Container.DataItem) == TargetListCondition.CONDITION_TYPE_AND) ? "かつ" : "または" %>' Visible="<%# Container.ItemIndex != 1 %>" runat="server"></asp:Literal>
																</span>
															</td>
														</tr>
														
														<asp:Repeater ID="rGroupConditions" DataSource="<%# ((ITargetListCondition)Container.DataItem).MakeBindData((ITargetListCondition)Container.DataItem) %>" ItemType="w2.App.Common.TargetList.TargetListCondition" runat="server">
														<ItemTemplate>

															<tr>
																<td colspan="2" align="left" class="search_item_bg group_item" runat="server" Visible="<%# Container.ItemIndex != 0 %>">
																	<asp:DropDownList Height="30" ID="ddlGroupConditionType" AutoPostBack="true" Visible="<%# (((List<ITargetListCondition>)((Repeater)Container.Parent.Parent.Parent).DataSource).Count == 1) && (Container.ItemIndex == 1) %>" OnSelectedIndexChanged="ddlGroupConditionType_OnSelectedIndexChanged" runat="server">
																		<asp:ListItem Value="AND">かつ</asp:ListItem>
																		<asp:ListItem Value="OR">または</asp:ListItem>
																	</asp:DropDownList>
																	<span style="font-size: 125%;">
																	<asp:Literal Text='<%#: (Item.GroupConditionType == TargetListCondition.CONDITION_TYPE_AND) ? "かつ" : "または" %>' Visible="<%# (((List<ITargetListCondition>)((Repeater)Container.Parent.Parent.Parent).DataSource).Count != 1) || (Container.ItemIndex != 1) %>" runat="server"></asp:Literal>
																	</span>
																	<span style="padding: 0px 20px 0px 20px;" />
																	<asp:Button id="btnCancelGroup" Text="  グループ解除  " Visible="<%# Container.ItemIndex == 1 %>" OnClick="btnCancelGroup_OnClick" runat="server" />
																</td>
															</tr>
															<tr>
																<td align="left" class="search_item_bg <%# (Item.GroupNo != 0) ? "group_item" : "" %>">
																	<asp:CheckBox class="cb_count" ID="cbGroupItem" Visible='<%# Item.GroupNo == 0 %>' runat="server"/>
																	<asp:HiddenField ID="hfConditionType" Value='<%# Item.ConditionType %>' runat="server"/>
																	<asp:HiddenField ID="hfGroupConditionType" Value='<%# Item.GroupConditionType %>' runat="server"/>
																	<asp:HiddenField ID="hfGroupNo" Value='<%# Item.GroupNo %>' runat="server"/>
																	<asp:DropDownList Height="30" ID="ddlDataKbn" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDataKbn_SelectedIndexChanged"></asp:DropDownList>
																	<span id="spanInputCondition" runat="server">
																		<asp:Literal runat="server" ID="lPrefix"/>
																		<asp:DropDownList Height="30" ID="ddlDataField" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDataField_SelectedIndexChanged"></asp:DropDownList>
																		<asp:HiddenField ID="hfDataType" runat="server" />
																		<span id="spanInputConditionDetail" runat="server">
																			が
																			<asp:DropDownList Height="30" ID="ddlFixedPurchasePattern" runat="server" DataTextField="text" DataValueField="value" Visible="False" AutoPostBack="true" OnSelectedIndexChanged="ddlFixedPurchasePattern_SelectedIndexChanged" />
																			<span id="spanFixedPurchasePattern" runat="server" Visible="False">
																				：<asp:TextBox ID="tbFixedPurchasePattern" Width="50" runat="server" MaxLength="2" Visible="False" />
																				<span runat="server" ID="spanMonth" Visible="<%# Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE %>">
																					ヶ月ごと
																				</span>
																				<span runat="server" ID="spanIntervalMonth" Visible="<%# Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY %>">
																					ヶ月ごと第
																				</span>
																				<span runat="server" ID="spanIntervalWeek" Visible="<%# Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY %>">
																					週間ごと
																				</span>
																				<asp:TextBox ID="tbFixedPurchasePatternDetail" Width="50" runat="server" MaxLength="3" Visible="False" />
																				<asp:DropDownList Height="30" ID="ddlDayOfWeek" runat="server" Visible="False" />
																				<span runat="server" ID="spanDay" Visible="<%# Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE %>">
																					日に届ける
																				</span>
																				<span runat="server" ID="spanIntervalDay" Visible="<%# Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS %>">
																					日ごとに届ける
																				</span>
																				<span runat="server" ID="spanDayOfWeek" Visible="<%# Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY || Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY %>">
																					曜日に届ける
																				</span>
																			</span>
																			<asp:DropDownList Height="30" ID="ddlValue" runat="server"  DataTextField="text" DataValueField="value"></asp:DropDownList>
																			<asp:TextBox ID="tbValue1" Width="300" runat="server"></asp:TextBox>
																			<asp:DropDownList Height="30" ID="ddlEqualSign" Width="120" runat="server"></asp:DropDownList>
																			<asp:DropDownList Height="30" ID="ddlOrderExist" runat="server"></asp:DropDownList>
																			<asp:DropDownList Height="30" ID="ddlFixedPurchaseOrderExist" runat="server"/>
																			<asp:DropDownList Height="30" ID="ddlPointExist" runat="server"/>
																			<asp:DropDownList Height="30" ID="ddlDmShippingHistory" runat="server"/>
																			<asp:DropDownList Height="30" ID="ddlFavoriteExist" runat="server"/>
																		</span>
																	</span>
																	<span id="spanInputSql" runat="server">
																		<br />
																		・・・<br />
																		AND	w2_User.user_id IN (<br />
																		<asp:TextBox ID="tbValue2" Width="500" Height="80" TextMode="MultiLine" runat="server"></asp:TextBox><br />
																		)<br />
																	</span>
																</td>
																<td align="center" class="search_title_bg"><asp:Button id="btnDelete" Text="  削除  " OnClick="btnDelete_Click" runat="server" Visible="<%# (((List<ITargetListCondition>)((Repeater)Container.Parent.Parent.Parent).DataSource).Count > 1) %>"/></td>
															</tr>

														</ItemTemplate>
														</asp:Repeater>

														</ItemTemplate>
													</asp:Repeater>

												</table>
												<br />
												<uc:ScheduleRegisterForm ID="ucScheduleRegisterForm" runat="server" />
												<br />
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考<br />
															■日付入力時に利用できるタグ<br />
															・「today」<br />
															　今日の日付で動的に抽出できます。<br />
															・「today-5d」<br />
															　今日から5日前の日付で動的に抽出できます。<br />
															　例えば今日が「<%: DateTimeUtility.ToStringForManager(DateTime.Today, DateTimeUtility.FormatType.ShortDate2Letter) %>」の場合、「<%: DateTimeUtility.ToStringForManager(DateTime.Today.AddDays(-5), DateTimeUtility.FormatType.ShortDate2Letter) %>」になります。<br />
															・「today+3m」<br />
															　今日から3か月後の日付で動的に抽出できます。<br />
															　例えば今日が「<%: DateTimeUtility.ToStringForManager(DateTime.Today, DateTimeUtility.FormatType.ShortDate2Letter) %>」の場合、「<%: DateTimeUtility.ToStringForManager(DateTime.Today.AddMonths(3), DateTimeUtility.FormatType.ShortDate2Letter) %>」になります。<br />
															・「today-1y」<br />
															　今日から1年前の日付で動的に抽出できます。<br />
															　例えば今日が「<%: DateTimeUtility.ToStringForManager(DateTime.Today, DateTimeUtility.FormatType.ShortDate2Letter) %>」の場合、「<%: DateTimeUtility.ToStringForManager(DateTime.Today.AddYears(-1), DateTimeUtility.FormatType.ShortDate2Letter) %>」になります。<br />
															<br />
															■誕生日入力時に利用できるタグ<br />
															・「today.day」<br />
															　今日の日付の「日」を動的に抽出できます。<br />
															　例えば今日が「<%: DateTimeUtility.ToStringForManager(DateTime.Today, DateTimeUtility.FormatType.ShortDate2Letter) %>」の場合、「<%: DateTime.Today.Day %>」になります。<br />
															・「today.day-1」<br />
															　今日から１日前の日付の「日」を動的に抽出できます。<br />
															　例えば今日が「<%: DateTimeUtility.ToStringForManager(DateTime.Today, DateTimeUtility.FormatType.ShortDate2Letter) %>」の場合、「<%: DateTime.Today.AddDays(-1).Day %>」になります。<br />
															・「today.month」<br />
															　今日の日付の「月」を動的に抽出できます。<br />
															　例えば今日が「<%: DateTimeUtility.ToStringForManager(DateTime.Today, DateTimeUtility.FormatType.ShortDate2Letter) %>」の場合、「<%: DateTime.Today.Month %>」になります。<br />
															・「today.month+1」<br />
															　今日から1か月後の日付の「月」を動的に抽出できます。<br />
															　例えば今日が「<%: DateTimeUtility.ToStringForManager(DateTime.Today, DateTimeUtility.FormatType.ShortDate2Letter) %>」の場合、「<%: DateTime.Today.AddMonths(1).Month %>」になります。<br />
															・「today.year」<br />
															　今日の日付の「年」を動的に抽出できます。<br />
															　例えば今日が「<%: DateTimeUtility.ToStringForManager(DateTime.Today, DateTimeUtility.FormatType.ShortDate2Letter) %>」の場合、「<%: DateTime.Today.Year %>」になります。<br />
															・「today.year-1」<br />
															　今日から1年前の日付の「年」を動的に抽出できます。<br />
															　例えば今日が「<%: DateTimeUtility.ToStringForManager(DateTime.Today, DateTimeUtility.FormatType.ShortDate2Letter) %>」の場合、「<%: DateTime.Today.AddYears(-1).Year %>」になります。<br />
															・「(today+30d).month」<br />
															　今日から30日後の日付の「月」を動的に抽出できます。<br />
															　例えば今日が「<%: DateTimeUtility.ToStringForManager(DateTime.Today, DateTimeUtility.FormatType.ShortDate2Letter) %>」の場合、30日後は「<%: DateTimeUtility.ToStringForManager(DateTime.Today.AddDays(30), DateTimeUtility.FormatType.ShortDate2Letter) %>」となり、「<%: DateTime.Today.AddDays(30).Month %>」になります。<br />
															<br />
															■日付項目入力時のイコールサイン<br />
															<table class="no-border no-padding">
																<tr>
																	<td>と等しい日付</td>
																	<td>：入力された値と同じ日付を抽出</td>
																</tr>
																<tr>
																	<td>より過去</td>
																	<td>：入力された値を<b>含まない</b>過去の日付を抽出</td>
																</tr>
																<tr>
																	<td>より未来</td>
																	<td>：入力された値を<b>含む</b>未来の日付を抽出。</td>
																</tr>
																<tr>
																	<td></td>
																	<td>&emsp;但し「入力された日付 00:00:00」の場合は含みません。</td>
																</tr>
																<tr>
																	<td></td>
																	<td>&emsp;日付のみで管理されている項目の時刻は「00:00:00」となり、条件に含まれなくなるため、抽出したい日の<b>前日</b>を指定して下さい。</td>
																</tr>
																<tr>
																	<td></td>
																	<td>
																		&emsp;該当する条件：生年月日&nbsp;/&nbsp;次回配送日&nbsp;/&nbsp;次々回配送日&nbsp;/&nbsp;配送希望日
																		&nbsp;/&nbsp;出荷予定日&nbsp;/&nbsp;出荷完了日時&nbsp;/&nbsp;入金確認日時&nbsp;/&nbsp;定期再開予定日&nbsp;/&nbsp;DM発送日時&nbsp;/&nbsp;有効期間
																	</td>
																</tr>
																<tr>
																	<td>日前</td>
																	<td>：入力された日数前の日付を抽出</td>
																</tr>
																<tr>
																	<td>日前から過去</td>
																	<td>：入力された日数前の日付を<b>含む</b>過去の日付を抽出</td>
																</tr>
																<tr>
																	<td>日前から未来</td>
																	<td>：入力された日数前の日付を<b>含む</b>未来の日付を抽出</td>
																</tr>
																<tr>
																	<td>日後</td>
																	<td>：入力された日数後の日付を抽出</td>
																</tr>
																<tr>
																	<td>日後から過去</td>
																	<td>：入力された日数後の日付を<b>含む</b>過去の日付を抽出</td>
																</tr>
																<tr>
																	<td>日後から未来</td>
																	<td>：入力された日数後の日付を<b>含む</b>未来の日付を抽出</td>
																</tr>
																<tr>
																	<td>週間前から未来</td>
																	<td>：入力された週数前の日付を<b>含む</b>未来の日付を抽出</td>
																</tr>
																<tr>
																	<td>ヶ月前から未来</td>
																	<td>：入力された月数前の日付を<b>含む</b>未来の日付を抽出</td>
																</tr>
																<tr>
																	<td>年前から未来</td>
																	<td>：入力された年数前の日付を<b>含む</b>未来の日付を抽出</td>
																</tr>
															</table>
															<br />
															■受注情報（集計）について<br />
															<table class="no-border no-padding">
																<tr>
																	<td>仮注文・キャンセル・仮注文キャンセルの注文をカウント対象外にします。 一部返品・交換を考慮しております。</td>
																</tr>
															</table>
															<br />
															■ スケジュール実行について<br />
															<table class="no-border no-padding">
																<tr>
																	<td>月単位</td>
																	<td>指定した日付で毎月実行されます。 対象月に指定した日付が存在しない場合は対象月での実行がスキップされます。</td>
																</tr>
															</table>
															<br />
															■ 定期配送パターンについて<br />
															・月間隔日付指定<br />
															　1ヵ月毎Y日のユーザーと指定し、Yが空欄の場合、月間隔日付指定で1ヵ月ごとに定期購入を行っているユーザーを抽出する。<br />
															　Xヵ月毎10日と指定し、Xが空欄の場合、月間隔日付指定で10日に定期購入を行っているユーザーを抽出する。<br />
															・月間隔・週・曜日指定<br />
															　1ヵ月ごと第X Y曜日のユーザーと指定し、X,Yが空欄の場合、月間隔・週・曜日指定で1ヵ月ごとに定期購入を行っているユーザーを抽出する。
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<asp:Button ID="btnBackBottom" Text="  戻る  "  OnClick="btnBack_Click" runat="server" />
													<asp:Button ID="btnConfirmBottom" Text="  確認する  " runat="server" OnClick="btnConfirm_Click" />
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
