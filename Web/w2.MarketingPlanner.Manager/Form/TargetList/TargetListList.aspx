<%--
=========================================================================================================
  Module      : ターゲットリスト設定一覧ページ(TargetListList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="TargetListList.aspx.cs" Inherits="Form_TargetList_TargetListList" %>
<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
	<meta http-equiv="Pragma" content="no-cache"/>
	<meta http-equiv="cache-control" content="no-cache"/>
	<meta http-equiv="expires" content="0"/>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ターゲットリスト情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_box_bg">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="100">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ターゲットID</td>
														<td class="search_item_bg" width="200">
															<asp:TextBox id="tbTargeId" runat="server"></asp:TextBox></td>
														<td class="search_title_bg" width="100">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ターゲット名</td>
														<td class="search_item_bg" width="200">
															<asp:TextBox ID="tbTargetName" runat="server"></asp:TextBox></td>
														<td class="search_btn_bg" width="83" rowspan="2">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Request.Url.AbsolutePath %>">クリア</a>&nbsp;
																
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
												</table>
											</td>
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
	<!--△ 検索 △-->
	<tr>
		<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">ターゲットリスト情報一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td align="left">
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="446"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td width="300" class="action_list_sp">
															<asp:Button id="btnUpload" Runat="server" Text="  CSVアップロード登録  " onclick="btnUpload_Click"></asp:Button>
															<img alt="" src="../../Images/Common/sp.gif" width="2" border="0" />
															<asp:Button id="btnNew" Runat="server" Text="  新規登録  " onclick="btnNew_Click"></asp:Button>
														</td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="100">ターゲットID</td>
														<td align="center" width="368">ターゲット名</td>
														<td align="center" width="80">件数</td>
														<td align="center" width="110">抽出タイミング</td>
														<td align="center" width="100">更新日</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl((String)Eval(Constants.FIELD_TARGETLIST_TARGET_ID))) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_TARGETLIST_TARGET_ID)) %></td>
																<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_TARGETLIST_TARGET_NAME)) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode((Eval(Constants.FIELD_TARGETLIST_DATA_COUNT) != DBNull.Value) ? Eval(Constants.FIELD_TARGETLIST_DATA_COUNT) : "-")%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(CreateExecTimingString((System.Data.DataRowView)Container.DataItem)) %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_USER_DATE_CHANGED), DateTimeUtility.FormatType.ShortDate2Letter) %></td>
															</tr>
														</ItemTemplate>
														<AlternatingItemTemplate>
															<tr class="list_item_bg2" onmouseover="listselect_mover(this)" onmouseout="listselect_mout2(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl((String)Eval(Constants.FIELD_TARGETLIST_TARGET_ID))) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_TARGETLIST_TARGET_ID))%></td>
																<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_TARGETLIST_TARGET_NAME))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode((Eval(Constants.FIELD_TARGETLIST_DATA_COUNT) != DBNull.Value) ? Eval(Constants.FIELD_TARGETLIST_DATA_COUNT) : "-")%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(CreateExecTimingString((System.Data.DataRowView)Container.DataItem)) %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_USER_DATE_CHANGED), DateTimeUtility.FormatType.ShortDate2Letter) %></td>
															</tr>
														</AlternatingItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" colspan="5" runat="server">
														</td>
													</tr>
												</table>
											</td>
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
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
