<%--
=========================================================================================================
	Module      : ターゲットリストポップアップ一覧ページ(TargetListPopup.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
	Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="TargetListPopup.aspx.cs" Inherits="Form_TargetListMerge_TargetListPopup" Title="ターゲットリスト一覧" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<!--▽ 検索 ▽-->
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
					<tr>
						<td class="search_box_bg">
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
											<tr>
												<td class="search_box_bg">
													<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
														<tr>
															<td class="search_title_bg" width="100">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
																ターゲットID
															</td>
															<td class="search_item_bg">
																<asp:TextBox ID="tbTargeId" runat="server"></asp:TextBox>
															</td>
															<td class="search_title_bg" width="100">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
																ターゲット名
															</td>
															<td class="search_item_bg">
																<asp:TextBox ID="tbTargetName" runat="server"></asp:TextBox>
															</td>
															<td class="search_btn_bg" width="83" rowspan="2">
																<div class="search_btn_main">
																	<asp:Button ID="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
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
		<!--△ 検索 △-->
		<tr>
			<td>
				<img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<!--▽ 一覧 ▽-->
		<tr>
			<td><h2 class="cmn-hed-h2">ターゲットリスト一覧</h2></td>
		</tr>
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
					<tr>
						<td>
							<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td>
										<img height="12" alt="" src="../../Images/Common/sp.gif" border="0" />
									</td>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
											<tr>
												<td align="left">
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="675">
																<asp:Label ID="lbPager" runat="server"></asp:Label>
															</td>
															<td width="83" class="action_list_sp">
															</td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<tr>
												<td>
													<img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_title_bg">
															<td align="center" width="60">
																ターゲットID
															</td>
															<td align="center" width="368">
																ターゲット名
															</td>
															<td align="center" width="50">
																件数
															</td>
														</tr>
														<asp:Repeater ID="rList" runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="CallBack('<%#Eval(Constants.FIELD_TARGETLIST_TARGET_NAME)%>','<%#Eval(Constants.FIELD_TARGETLIST_TARGET_ID)%>', '<%# WebSanitizer.HtmlEncode(StringUtility.ToValue(Eval(Constants.FIELD_TARGETLIST_DATA_COUNT), "0"))%>',<%#(new List<string>{Constants.FLG_TARGETLIST_TARGET_TYPE_CSV,Constants.FLG_TARGETLIST_TARGET_TYPE_MERGE,Constants.FLG_TARGETLIST_TARGET_TYPE_USER_LIST,Constants.FLG_TARGETLIST_TARGET_TYPE_ORDER_LIST,Constants.FLG_TARGETLIST_TARGET_TYPE_ORDERWORKFLOW_LIST}).Contains(Eval(Constants.FIELD_TARGETLIST_TARGET_TYPE)).ToString().ToLower() %>);">
																	<td align="center">
																		<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_TARGETLIST_TARGET_ID)) %>
																	</td>
																	<td align="left">
																		&nbsp;<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_TARGETLIST_TARGET_NAME)) %></td>
																	<td align="center">
																		<%# WebSanitizer.HtmlEncode(StringUtility.ToValue(Eval(Constants.FIELD_TARGETLIST_DATA_COUNT), "0"))%>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" visible="false">
															<td id="tdErrorMessage" colspan="5" runat="server">
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
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
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
	</table>
	<script type="text/javascript">
		var fnCallBack;
		var refection = "var fnCallBack = window.opener." + window.name + ";";
		eval(refection);
		function CallBack(text, value, data_count, disable_checkbox) {
			if (typeof (fnCallBack) != 'undefined') 
			{
				fnCallBack("【" + value + "】" + text, data_count, value, disable_checkbox);
				window.close();
			}
		}
	</script>
</asp:Content>
