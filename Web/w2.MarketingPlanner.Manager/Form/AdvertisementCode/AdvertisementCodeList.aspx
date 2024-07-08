<%--
=========================================================================================================
  Module      : 広告コード一覧ページ(AdvertisementCodeList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AdvertisementCodeList.aspx.cs" Inherits="Form_AdvertisementCode_AdvertisementCodeList" %>
<%@ Import Namespace="w2.Domain.AdvCode.Helper" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">広告コード設定</h1></td>
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
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="80">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />広告媒体区分
														</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlAdvCodeMediaType" runat="server" Width="130"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="80"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />広告コード</td>
														<td class="search_item_bg" width="100">
															<asp:TextBox ID="tbSearchAdvertisementCode" Width="100" runat="server"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="80"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />媒体名</td>
														<td class="search_item_bg" width="100"><asp:TextBox id="tbSearchMediaName" runat="server" Width="100"></asp:TextBox></td>
														<td class="search_btn_bg" width="80" rowspan="3">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " onclick="btnSearch_Click"></asp:Button></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_LIST %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
															</div>
														</td>
													</tr>
													<tr>
														<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
														<td class="search_title_bg" width="100"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />会員登録時<br />紐づけ会員ランク</td>
														<td class="search_item_bg" width="100">
															<asp:DropDownList id="ddlMemberRank" runat="server" Width="130" ></asp:DropDownList>
														</td>
														<% } %>
														<%if (Constants.USERMANAGEMENTLEVELSETTING_OPTION_ENABLED) { %>
														<td class="search_title_bg" width="100"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />会員登録時<br />紐づけユーザー管理レベル</td>
														<td class="search_item_bg" width="100">
															<asp:DropDownList id="ddlUserManagementLevel" runat="server" Width="130" ></asp:DropDownList>
														</td>
														<% } %>
													</tr>
													<tr>
														<td class="search_title_bg" width="100"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />並び順</td>
														<td class="search_item_bg" width="100" colspan="5">
															<asp:DropDownList id="ddlSearchSortKbn" runat="server" Width="130" ></asp:DropDownList>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
											<%-- マスタ出力 --%>
											<tr>
												<td class="search_table">
													<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="AdvCode" TableWidth="758" />
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
		<td><h2 class="cmn-hed-h2">広告コード設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div id="divDetail" runat="server">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<!--▽ ページング ▽--> 
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="60%"><asp:label id="lbPager" Runat="server"></asp:label></td>
															<td width="40%" class="action_list_sp">
																<input type="button" id="btnShowAdvCodeMediaTypePopupTop" value="  広告媒体区分  " onclick="OpenWindow('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_MEDIA_TYPE) %>','AdvCodeMediaType','width=900,height=750,top=120,left=420,status=NO,scrollbars=yes');" />
																<asp:Button ID="btnInsertTop" runat="server" Text="  新規登録  " onclick="btnInsert_Click"/>
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
															<td align="center" width="38">NO</td>
															<td align="center" width="130">広告媒体区分</td>
															<td align="center" width="300">広告コード<hr>媒体名</td>
															<td align="center" width="70">媒体費</td>
															<td align="center" width="70">出稿日</td>
															<td align="center" width="170">媒体掲載期間</td>
															<td align="center" width="40">有効<br />フラグ</td>
														</tr>
														<asp:Repeater id="rAdvCodeList" ItemType="w2.Domain.AdvCode.Helper.AdvCodeListSearchResult" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateAdvCodeDetailUrl(((AdvCodeListSearchResult)Container.DataItem).AdvertisementCode)) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).AdvcodeNo)%></td>
																<td align="left" title="<%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).AdvcodeMediaTypeName) %>"><%# WebSanitizer.HtmlEncode(AbbreviateString(((AdvCodeListSearchResult)Container.DataItem).AdvcodeMediaTypeName, 10))%></td>
																<td align="left" title="<%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).MediaName) %>">
																	<%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).AdvertisementCode) %><hr><%# WebSanitizer.HtmlEncode(AbbreviateString(((AdvCodeListSearchResult)Container.DataItem).MediaName, 30))%></td>
																<td align="right"><%# WebSanitizer.HtmlEncode(((AdvCodeListSearchResult)Container.DataItem).MediaCost.ToPriceString(true)) %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.AdvertisementDate, DateTimeUtility.FormatType.ShortDate2Letter)%></td>
																<td align="center"><%#: GetPublicationDateString(Item.PublicationDateFrom, Item.PublicationDateTo) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ADVCODE, Constants.FIELD_ADVCODE_VALID_FLG, ((AdvCodeListSearchResult)Container.DataItem).ValidFlg))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="False">
														<td id="tdErrorMessage" colspan="8" runat="server"></td>
													</tr>
													</table>
													<table cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td class="action_list_sp">
													<input type="button" id="btnShowAdvCodeMediaTypePopupBottom" value="  広告媒体区分  " Visible="" onclick="OpenWindow('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_MEDIA_TYPE) %>','AdvCodeMediaType','width=900,height=750,top=120,left=420,status=NO,scrollbars=yes');" />
													<asp:Button ID="btnInsertBottom" runat="server" Text="  新規登録  " onclick="btnInsert_Click"/>
												</td>
											</tr>
											<tr>
												<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="info_table" width="100%" border="0" cellspacing="1" cellpadding="3">
														<tr class="info_item_bg">
															<td align="left">
																備考<br />
																・広告コードを利用する際は サイトアドレスへパラメータ「advc」を付加し、利用する広告コードを設定してください。<br />
														　		  例：広告コード「1234」を利用する場合、<br />
														　　		<%= Constants.URL_FRONT_PC + Constants.PAGE_FRONT_DEFAULT%>?<strong>advc=1234</strong>　または<br />
																	&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%= Constants.URL_FRONT_PC + Constants.PAGE_FRONT_PRODUCT_DETAIL%>?shop=0&pid=sample&<strong>advc=1234</strong>　となります。
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<script type="text/javascript">
<!--
	// ポップアップ ウィンドウを開く
	function OpenWindow(url, name, style) {
		var newWindow = window.open(url, name, style);
		newWindow.focus();
	}

//-->
</script>

</asp:Content>
