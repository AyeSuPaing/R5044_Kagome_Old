<%--
=========================================================================================================
  Module      : 広告コード検索ページ(AdvertisementCodeSearch.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.Domain.AdvCode.Helper" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="AdvertisementCodeSearch.aspx.cs" Inherits="Form_Common_AdvertisementCodeSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
	// 選択された広告コードを設定
	function setAdvCode(advcode) {
		// 親ウィンドウが存在する場合
		if (window.opener != null) {
			// 選択された広告コードを設定
			window.opener.setAdvertisementCode(advcode);

			// アラートを表示する
			autoAlert(advcode + 'の広告コードが追加されました！', 3000);
		}
	}

	// 自動的に閉じるアラートを設定
	function autoAlert(message, duration) {
		var element = document.createElement("div");
		element.setAttribute("style", "position:fixed;top:50%;left:45%;background-color:#FFFFFF;border:1px #898589 solid;padding:5px;font-size:14px;");
		element.innerHTML = message;
		setTimeout(function () { element.parentNode.removeChild(element); }, duration);
		document.body.appendChild(element);
	}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
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
														<td class="search_item_bg" ><asp:TextBox id="tbSearchMediaName" runat="server" Width="100"></asp:TextBox></td>
														<td class="search_btn_bg" width="80" rowspan="2">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " onclick="btnSearch_Click"></asp:Button></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISEMENT_CODE_SEARCH %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
															</div>
														</td>
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
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!--▽ ページング ▽--> 
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td><asp:label id="lbPager" Runat="server"></asp:label></td>
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
														<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="javascript:setAdvCode(<%#: CreateJavaScriptSetAdvertisementCode(Item.AdvertisementCode) %>)">
															<td align="center"><%#: Item.AdvcodeNo %></td>
															<td align="left" title="<%#: Item.AdvcodeMediaTypeName %>"><%#: AbbreviateString(Item.AdvcodeMediaTypeName, 10) %></td>
															<td align="left" title="<%#: Item.MediaName %>">
																<%#: Item.AdvertisementCode %><hr><%#: AbbreviateString(Item.MediaName, 30) %></td>
															<td align="right"><%#: Item.MediaCost.ToPriceString(true) %></td>
															<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.AdvertisementDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
															<td align="center"><%#: GetPublicationDateString(Item.PublicationDateFrom, Item.PublicationDateTo) %></td>
															<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_ADVCODE, Constants.FIELD_ADVCODE_VALID_FLG, Item.ValidFlg) %></td>
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
											<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>

