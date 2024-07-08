<%--
=========================================================================================================
  Module      : シリアルキー一覧ページ(SerialKeyList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SerialKeyList.aspx.cs" Inherits="Form_SerialKey_SerialKeyList" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
<meta http-equiv="Pragma" content="no-cache" />
<meta http-equiv="cache-control" content="no-cache" />
<meta http-equiv="expires" content="0" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">シリアルキー情報</h1></td>
	</tr>
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td style="width: 792px">
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
											<td class="search_table">
												<%
													// 各テキストボックスのEnter押下時に検索を走らせるようにする
													tbProductId.Attributes["onkeypress"]
														= tbProductId.Attributes["onkeypress"]
														= tbOrderId.Attributes["onkeypress"]
														= tbUserId.Attributes["onkeypress"]
														= tbSerialKey.Attributes["onkeypress"]
														= "if (event.keyCode==13){__doPostBack('" + btnSearch.UniqueID + "',''); return false;}";
												%>
												<table cellspacing="1" cellpadding="2" width="768" border="0">
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															シリアルキー</td>
														<td class="search_item_bg" width="130" colspan="5">
															<asp:TextBox id="tbSerialKey" runat="server" Width="210"></asp:TextBox></td>
														<td class="search_btn_bg" width="83" rowspan="3">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_SERIALKEY_LIST %>">クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="98">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品ID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbProductId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															注文ID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbOrderId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ユーザーID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbUserId" runat="server" Width="125"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															状態</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlStatus" runat="server"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															有効フラグ</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlValidFlg" runat="server"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="100">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server"></asp:DropDownList>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="SerialKey" TableWidth="768" />
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">シリアルキー情報一覧</h2></td>
	</tr>
	<tr>
		<td style="width: 792px">
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
														<td width="550" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td align="right">
															<table width="200px">
																<tr>
																	<td style="text-align:right">
																		<asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
																	</td>
																</tr>
															</table>
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
														<td align="center" width="24%" style="height: 17px">商品ID（ + バリエーションID）</td>
														<td align="center" width="22%" style="height: 17px">シリアルキー</td>
														<td align="center" width="12%" style="height: 17px">状態</td>
														<td align="center" width="12%" style="height: 17px">注文ID</td>
														<td align="center" width="12%" style="height: 17px">ユーザーID</td>
														<td align="center" width="10%" style="height: 17px">引渡日</td>
														<td align="center" width="8%" style="height: 17px">有効フラグ</td>
													</tr>
													<asp:Repeater id="rList" ItemType="w2.Domain.SerialKey.SerialKeyModel" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateSerialKeyDetailUrl(Item.SerialKey, Item.ProductId)) %>')">
																<td align="center"><%#: Item.ProductId + (Item.VId != "" ? " + " + Item.VId : "") %></td>
																<td align="center"><%#: SerialKeyUtility.DecryptSerialKey(Item.SerialKey) %></td>
																<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_SERIALKEY, Constants.FIELD_SERIALKEY_STATUS, Item.Status) %></td>
																<td align="center"><%#: Item.OrderId %></td>
																<td align="center"><%#: Item.UserId %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateDelivered, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_SERIALKEY, Constants.FIELD_SERIALKEY_VALID_FLG, Item.ValidFlg) %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="7"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															シリアルキーは完全一致で検索します。
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="550" style="height: 22px"></td>
														<td align="right">
															<table width="200px">
																<tr>
																	<td style="text-align:right">
																		<asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
