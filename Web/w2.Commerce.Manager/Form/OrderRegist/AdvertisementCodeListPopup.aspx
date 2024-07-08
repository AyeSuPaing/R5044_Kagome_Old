<%--
=========================================================================================================
  Module      : 広告コード一覧ページ(AdvertisementCodeListPopup.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="AdvertisementCodeListPopup.aspx.cs" Inherits="Form_AdvertisementCode_AdvertisementCodeList" Title="広告コード情報一覧" %>
<%@ Import Namespace="w2.Domain.AdvCode.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellspacing="0" cellpadding="0" border="0">
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="714" border="0">
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
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="700" border="0">
													<tr>
														<td class="search_title_bg" width="80" >
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />広告媒体区分
														</td>
														<td class="search_item_bg" width="130" >
															<asp:DropDownList id="ddlAdvCodeMediaType" runat="server" Width="125"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="70">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />広告コード
														</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbAdvCode" runat="server" Width="125"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="50">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />媒体名
														</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbMediaName" runat="server" Width="125"></asp:TextBox>
														</td>
														<td class="search_btn_bg" rowspan="2" width="50">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" />
															</div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_ADVPOPUP %>">クリア</a>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="100"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />並び順</td>
														<td class="search_item_bg" width="135" colspan="5">
															<asp:DropDownList id="ddlSearchSortKbn" runat="server" Width="130">
																<asp:ListItem Value="0">登録日/昇順</asp:ListItem>
																<asp:ListItem Value="1" Selected="True">登録日/降順</asp:ListItem>
																<asp:ListItem Value="2">広告コード/昇順</asp:ListItem>
															</asp:DropDownList>
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr id="trList" runat="server">
		<td><h2 class="cmn-hed-h2">広告コード情報一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="714" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div id="divEdit" runat="server">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<!--▽ ページング ▽--> 
													<table class="list_pager" cellspacing="0" cellpadding="0" width="700" border="0">
														<tr>
															<td width="80%"><asp:label id="lbPager1" Runat="server"></asp:label></td>
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
													<table class="list_table" cellspacing="1" cellpadding="3" width="690" border="0">
														<tr class="list_title_bg">
															<td align="center" width="38">NO</td>
															<td align="center" width="150">広告媒体区分</td>
															<td align="center" >広告コード<hr />媒体名</td>
															<td align="center" width="160">媒体掲載期間</td>
														</tr>
														<asp:Repeater id="rEditList" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# (Container.ItemIndex % 2) + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="CallBack('<%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).AdvertisementCode) %>','<%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).MediaName) %>')">
																	<td align="center">
																		<%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).AdvcodeNo) %>
																	</td>
																	<td align="left" title="<%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).AdvcodeMediaTypeName) %>">
																		 <%# WebSanitizer.HtmlEncode(AbbreviateString(((AdvCodeListSearchResult)Container.DataItem).AdvcodeMediaTypeName, 10)) %>
																	</td>
																	<td align="left" title="<%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).MediaName) %>">
																		<asp:HiddenField ID="hfAdvCodeNo" Value='<%# ((AdvCodeListSearchResult)Container.DataItem).AdvcodeNo %>' runat="server" />
																		<%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).AdvertisementCode) %><hr /><%# WebSanitizer.HtmlEncode(AbbreviateString(((AdvCodeListSearchResult)Container.DataItem).MediaName, 30))%>
																	<td align="center"><%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).PublicationDateString)%></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" Visible="false">
															<td id="tdErrorMessage" runat="server" style="height: 17px" colspan="5" ></td>
														</tr>
													</table>
													<table cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</div>
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
	var fnCallBack;
	var refection = "var fnCallBack = window.opener." + window.name + ";";
	eval(refection);
	function CallBack(value,text) {
		if (typeof (fnCallBack) != 'undefined') {
			fnCallBack(value,text);
			window.close();
		}
	}
</script>
</asp:Content>
