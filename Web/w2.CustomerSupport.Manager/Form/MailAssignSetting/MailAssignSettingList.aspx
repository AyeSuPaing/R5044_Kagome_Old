<%--
=========================================================================================================
  Module      : 受信時振分けルール設定一覧ページ(MailAssignSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailAssignSettingList.aspx.cs" Inherits="Form_MailAssignSetting_MailAssignSettingList" %>
<%@ Import Namespace="w2.App.Common.MailAssignSetting" %>
<%@ Import Namespace="w2.Common.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><h1 class="page-title">受信時振分けルール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">受信時振分けルール設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td>
						<table cellspacing="1" cellpadding="0" width="100%" border="0">
							<tr>
								<td>
									<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td align="center">
												<table cellspacing="0" cellpadding="0" border="0" width="100%">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<div id ="dvMessageExecSuccess" runat="server" visible="false">
														<tr>
															<td>
																<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
																	<tr class="info_item_bg">
																		<td align="left">メール振分けを実行しました。</td>
																	</tr>
																</table>
															</td>
														</tr>
														<tr>
															<td height="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td>
														</tr>
													</div>
													<tr>
														<td>
															<!--▽ ページング ▽-->
															<table class="list_pager" cellspacing="0" cellpadding="0" border="0" width="758">
																<tr>
																	<td width="675"><asp:label id="lbPager1" Runat="server"></asp:label></td>
																	<td class="action_list_sp" style="height: 22px"><asp:button id="btnRunMailAssignTop" runat="server" Text="　メール振分け　" OnClientClick="return validate_mail_assign()" onclick="btnRunMailAssign_Click" /></td>
																	<td width="83" class="action_list_sp" style="height: 22px"><asp:button id="btnInsertTop" runat="server" Text="　新規登録　" onclick="btnInsert_Click" /></td>
																</tr>
															</table>
															<!--△ ページング △-->
														</td>
													</tr>
													<tr>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<div id="divHeaderList" class="y_scrollable div_table_header" style="border: 0px; ">
																<table class="list_table" cellspacing="1" cellpadding="3" width="100%" border="0" style="min-width:800px">
																	<tr class="list_title_bg">
																		<td align="center" align="center" width="30" style="max-width:30px" rowspan="2">
																			<input id="cbChecktAllList" type="checkbox" onclick="check_all_list(this)">
																		</td>
																		<td align="center" width="100" rowspan="2" class="colPriorityOrder">優先順</td>
																		<td align="center" width="100" rowspan="2" class="colDisName">振分け設定名</td>
																		<td align="center" rowspan="2" class="colDisCondition" style="min-width:100px">振分け条件</td>
																		<td align="center" width="60" rowspan="2" class="colStopSorting">振分け停止</td>
																		<td align="center" colspan="6">振分けアクション</td>
																		<td align="center" width="50" rowspan="2" class="colAutoResponseName">オートレスポンス</td>
																		<td width="9" rowspan="2"></td>
																	</tr>
																	<tr class="list_title_bg">
																		<td align="center" width="60" class="colStatus">ステータス</td>
																		<td align="center" width="60" class="colCategory">カテゴリ</td>
																		<td align="center" width="60" class="colImportance">重要度</td>
																		<td align="center" width="60" class="colGroup">担当<br />グループ</td>
																		<td align="center" width="60" class="colOperator">担当<br />	オペレータ</td>
																		<td align="center" width="40" class="colTrashName">ゴミ箱</td>
																	</tr>
																</table>
															</div>
															<div id="divDataList" class="y_scrollable div_table_data" style="border: 0px; max-height:300px <%= rList.Items.Count > 0 ? string.Empty: "display:none;" %>">
																<table class="list_table" cellspacing="1" cellpadding="3" width="100%" border="0">
																	<asp:Repeater ID="rList" runat="server">
																		<ItemTemplate>
																			<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" height="30">
																				<td align="center" width="30" style="max-width:30px">
																					<asp:CheckBox ID="cbCheck" CssClass="list_checkbox" runat="server" />
																					<asp:HiddenField ID="hfMailAssignId" Value="<%# ((CsMailAssignSettingModel)Container.DataItem).MailAssignId %>" runat="server" />
																				</td>

																				<td width="100" class="colPriorityOrder_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center" style='<%# ((CsMailAssignSettingModel)Container.DataItem).EX_IsMatchOnBind || ((CsMailAssignSettingModel)Container.DataItem).EX_IsMatchAnything ? "display: none": ""%>'>&nbsp;<%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).AssignPriority)%></td>
																				<td width="100" class="colPriorityOrder_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center" style='<%# ((CsMailAssignSettingModel)Container.DataItem).EX_IsMatchUserSetting ? "display: none": ""%>'>&nbsp;<b><%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).AssignPriority)%></b></td>

																				<td width="100" class="colDisName_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center"><%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).MailAssignName)%></td>

																				<td class="colDisCondition_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="left" style='word-break: break-all; <%# ((CsMailAssignSettingModel)Container.DataItem).EX_IsMatchOnBind || ((CsMailAssignSettingModel)Container.DataItem).EX_IsMatchAnything ? "display: none": ""%>'>
																					<%# (((CsMailAssignSettingModel)Container.DataItem).Items.Length >= 2) ? "▼" + ((CsMailAssignSettingModel)Container.DataItem).EX_LogicalOperationName + "<br /><br />" : ""%>
																					<asp:Repeater ID="Repeater1" DataSource="<%# ((CsMailAssignSettingModel)Container.DataItem).Items %>" runat="server">
																						<ItemTemplate>
																							<%# ((CsMailAssignSettingModel)((RepeaterItem)Container.Parent.Parent).DataItem).Items.Length >= 2 ? "" : ""%>
																							&lt;<%# ((CsMailAssignSettingItemModel)Container.DataItem).EX_AssignItemMatchingTargetName %>&gt; が [<%# WebSanitizer.HtmlEncode(((CsMailAssignSettingItemModel)Container.DataItem).MatchingValue) %>] <%# ((CsMailAssignSettingItemModel)Container.DataItem).EX_MatchingTypeName %>
																							<br />
																						</ItemTemplate>
																						<SeparatorTemplate>
																							<%# ((CsMailAssignSettingModel)((RepeaterItem)((Repeater)Container.Parent).Parent).DataItem).EX_LogicalOperationName_Short %><br />
																						</SeparatorTemplate>
																					</asp:Repeater>
																				</td>
																				<td class="colDisCondition_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" style='<%# ((CsMailAssignSettingModel)Container.DataItem).EX_IsMatchOnBind ? "": "display: none" %>'><b>既存インシデントに紐付く受信メール [固定]</b></td>
																				<td class="colDisCondition_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" style='<%# ((CsMailAssignSettingModel)Container.DataItem).EX_IsMatchAnything ? "": "display: none" %>'><b>全ての受信メール [固定]</b></td>
																				<td class="colStopSorting_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center"><%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).EX_StopFiltering)%></td>
																				<td class="colStatus_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center"><%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).EX_AssignStatusName) %></td>
																				<td class="colCategory_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center"><%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).EX_AssignIncidentCategoryName) %></td>
																				<td class="colImportance_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center"><%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).EX_AssignImportanceName) %></td>
																				<td class="colGroup_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center"><%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).EX_AssignCsGroupName) %></td>
																				<td class="colOperator_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center" style='word-break: break-all'><%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).EX_AssignOperatorName) %></td>
																				<td class="colTrashName_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center"><%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).EX_TrashName)%></td>
																				<td class="colAutoResponseName_Data" onclick="listselect_mclick(this, '<%# WebSanitizer.HtmlEncode(CreateDetailUrl(((CsMailAssignSettingModel)Container.DataItem).MailAssignId)) %>')" align="center"><%# WebSanitizer.HtmlEncode(((CsMailAssignSettingModel)Container.DataItem).EX_AutoResponseName) %></td>
																			</tr>
																		</ItemTemplate>
																	</asp:Repeater>
																	<tr id="trListError" class="list_alert" runat="server" visible="False">
																		<td id="tdErrorMessage" colspan="11" runat="server"></td>
																	</tr>
																</table>
															</div>
															<div id="win-size-grip7"><img src ="../../Images/Cs/hsizegrip.png" ></div>
														</td>
													</tr>
													<tr>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<table>
															<tr>
																<td class="action_part_bottom"><asp:Button id="btnRunMailAssignBotttom" runat="server" Text="　メール振分け　" OnClientClick="return validate_mail_assign()" onclick="btnRunMailAssign_Click"></asp:Button></td>
																<td width="83" class="action_part_bottom"><asp:Button id="btnInsertBotttom" runat="server" Text="　新規登録　" onclick="btnInsert_Click"></asp:Button></td>
															</tr>
														</table>
													</tr>
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<script type="text/javascript">

	// Window load
	$(window).bind('load', function () {
		SetWidthColDataForDataList();
	});

	// Window resize
	$(window).resize(function () {
		setInterval(function () {
			SetWidthColDataForDataList();
		}, 500);
	});

	// Check all list
	function check_all_list(checkAll) {
		if (checkAll.checked) $('span.list_checkbox input').attr('checked', 'checked');
		else $('span.list_checkbox input').removeAttr('checked');
	}

	// Check validate mail assign
	function validate_mail_assign(message) {
		if ($("span.list_checkbox input:checked").length == 0) {
			alert('メール振分け対象がありません。');
			return false;
		}

		return true;
	}

	// Set Width Col Data For Data List
	function SetWidthColDataForDataList() {
		SetWidthColDataByHeader('.colPriorityOrder', '.colPriorityOrder_Data');
		SetWidthColDataByHeader('.colDisName', '.colDisName_Data');
		SetWidthColDataByHeader('.colDisCondition', '.colDisCondition_Data');
		SetWidthColDataByHeader('.colStopSorting', '.colStopSorting_Data');
		SetWidthColDataByHeader('.colStatus', '.colStatus_Data');
		SetWidthColDataByHeader('.colCategory', '.colCategory_Data');
		SetWidthColDataByHeader('.colImportance', '.colImportance_Data');
		SetWidthColDataByHeader('.colGroup', '.colGroup_Data');
		SetWidthColDataByHeader('.colOperator', '.colOperator_Data');
		SetWidthColDataByHeader('.colTrashName', '.colTrashName_Data');
	}

	// Set Width For Column Data By Header
	function SetWidthColDataByHeader(headerClassName, dataClassName) {
		//Googleの場合、borderは1.25ｐｘ、左と右のpaddingは１０ｐｘ
		var deviation = 11.25;
		//【Microsoft Edge】【Firefox】【IE バージョン１０以下】の場合、borderは1ｐｘ、左と右のpaddingは１０ｐｘ
		if (navigator.userAgent.search(/(firefox|edg|msie)/i) != -1) {
			deviation = 11;
		}
		//IE バージョン１１以上の場合、borderは1ｐｘ、左と右のpaddingは１０ｐｘ
		if (navigator.userAgent.search(/trident.+rv/i) != -1) {
			deviation = 11;
		}
		//width設定値を取得
		var headerWidth = $('#divHeaderList').find(headerClassName)[0].getBoundingClientRect().width.toFixed(3) - deviation;

		// Reset width
		$('#divDataList').find(dataClassName).css({
			"width": ""
		});

		// Set width column data
		$('#divDataList').find(dataClassName).css({
			"width": (headerWidth) + "px"
		});
	}

	$('#divDataList').resizable2({
		handleSelector: "#win-size-grip7",
		resizeWidth: false,
		onDragStart: function (e, $el, opt) {
			$el.css({ 'height': $el.height(), 'max-height': '<%= Constants.VARIABLE_MAXIMUM_SIZE %>px' });
		},
		onDragEnd: function (e, $el, opt) {
			setCookie("<%= Constants.COOKIE_KEY_MAIL_ASSIGN_SETTING_LIST_HEIGHT %>", $el.height(), { expires: 1000 });
		}
	});

	$('#divDataList').css({ 'height': 'auto', 'max-height': '<%= (CookieManager.GetValue(Constants.COOKIE_KEY_MAIL_ASSIGN_SETTING_LIST_HEIGHT) ?? Constants.MAIL_ASSIGN_SETTING_LIST_DEFAULT_HEIGHT_SIZE) %>px' });

</script>
</asp:Content>
