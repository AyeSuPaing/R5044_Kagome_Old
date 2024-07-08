<%--
=========================================================================================================
  Module      : リアル店舗情報一覧ページ(RealShopList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.RealShop" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="RealShopList.aspx.cs" Inherits="Form_RealShop_RealShopList" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">リアル店舗情報</h1></td>
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
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="90">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															リアル店舗ID
														</td>
														<td class="search_item_bg" width="120">
															<%-- リアル店舗ID選択用（リピータで実装） --%>
															<% 
																tbRealShopId.Attributes.Add("autocomplete", "on");
																tbRealShopId.Attributes.Add("list", "realshopid");
															%>
															<asp:Repeater ID="rShopIdAutoComplete" runat="server">
																<HeaderTemplate>
																	<datalist id="realshopid">
																</HeaderTemplate>
																<ItemTemplate>
																	<option value="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_REAL_SHOP_ID))%>" label="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_NAME))%>"></option>
																</ItemTemplate>
																<FooterTemplate>
																	</datalist>
																</FooterTemplate>
															</asp:Repeater>
															<asp:TextBox id="tbRealShopId" runat="server" Width="125"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="90">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															有効フラグ
														</td>
														<td class="search_item_bg" width="120">
															<asp:DropDownList id="dllValidFlg" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="90">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順
														</td>
														<td class="search_item_bg" width="120">
															<asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="4">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_REALSHOP_LIST %>">クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															店舗名
														</td>
														<td class="search_item_bg">
															<%-- リアル店舗名選択用（リピータで実装） --%>
															<% 
																tbRealShopName.Attributes.Add("autocomplete", "on");
																tbRealShopName.Attributes.Add("list", "realshopname");
															%>
															<asp:Repeater ID="rShopNameAutoComplete" runat="server">
																<HeaderTemplate>
																	<datalist id="realshopname">
																</HeaderTemplate>
																<ItemTemplate>
																	<option value="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_NAME))%>" label="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_NAME))%>"></option>
																</ItemTemplate>
																<FooterTemplate>
																	</datalist>
																</FooterTemplate>
															</asp:Repeater>
															<asp:TextBox id="tbRealShopName" runat="server" Width="125"></asp:TextBox>
														</td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															店舗名かな
														</td>
														<td class="search_item_bg" colspan="3">
															<asp:TextBox id="tbRealShopNameKana" runat="server" Width="125"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															電話番号
														</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbTel" runat="server" Width="125"></asp:TextBox>
														</td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															FAX
														</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbFax" runat="server" Width="125"></asp:TextBox>
														</td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															メールアドレス
														</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbMailAddr" runat="server" Width="125"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															郵便番号
														</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbZip" runat="server" Width="125"></asp:TextBox>
														</td>
														<td class="search_title_bg">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															住所
														</td>
														<td class="search_item_bg" colspan="3">
															<asp:TextBox id="tbAddr" runat="server" Width="357"></asp:TextBox>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="RealShop" TableWidth="758" />
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">リアル店舗情報一覧</h2></td>
	</tr>
	<tr>
		<td style="width:792px">
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
														<td width="675" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label>	
														</td>
														<td class="action_list_sp"><asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
														<td align="center" width="120">リアル店舗ID</td>
														<td align="center" width="259">店舗名</td>
														<td align="center" width="259">店舗名かな</td>
														<td align="center" width="70">表示順</td>
														<td align="center" width="70">有効フラグ</td>
													</tr>
													<%-- △サンプル★△ --%>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRealShopRegistUrl((string)Eval(Constants.FIELD_REALSHOP_REAL_SHOP_ID),Constants.ACTION_STATUS_UPDATE)) %>')">
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_REAL_SHOP_ID))%></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_NAME))%></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_NAME_KANA))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_DISPLAY_ORDER))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_REALSHOP, Constants.FIELD_REALSHOP_VALID_FLG, Eval(Constants.FIELD_REALSHOP_VALID_FLG)))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="5"></td>
													</tr>
												</table>
												<!--
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															・
														</td>
													</tr>
												</table>
												-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									</table>
									<div class="info_item_bg" style="border: 1px; border-style: solid; border-color: #bababa; padding: 5px;">
										<table>
											<tr>
												<td style="border: 1px; border-style: solid; border-color: #bababa; padding: 5px;" colspan="2">
													■ エリア一覧
												</td>
											</tr>
											<tr>
												<td style="border: 1px; border-style: solid; border-color: #bababa; padding: 5px;">
													エリアID
												</td>
												<td style="border: 1px; border-style: solid; border-color: #bababa; padding: 5px;">
													エリア名
												</td>
											</tr>
											<asp:Repeater ID="rArea" runat="server">
												<ItemTemplate>
													<tr>
														<td style="border: 1px; border-style: solid; border-color: #bababa; padding: 5px;">
															<%# ((RealShopArea)Container.DataItem).AreaId %>
														</td>
														<td style="border: 1px; border-style: solid; border-color: #bababa; padding: 5px;">
															<%# ((RealShopArea)Container.DataItem).AreaName %>
														</td>
													</tr>
												</ItemTemplate>
											</asp:Repeater>
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
</asp:Content>