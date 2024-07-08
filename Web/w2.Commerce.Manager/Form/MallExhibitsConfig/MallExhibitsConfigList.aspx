<%--
=========================================================================================================
  Module      : モール出品設定リストページ(MallExhibitsConfigList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MallExhibitsConfigList.aspx.cs" Inherits="Form_MallExhibitsConfig_MallExhibitsConfigList" %>
<%@ Import Namespace="System.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%-- 
テキストボックス内でEnterキーを押してSubmit（一番上に配置されているTextBoxのSubmit）送信しようとすると、
IEのバグでテキストボックスが画面上に一つのみ配置されている場合にSubmit送信されない不具合の対応として、
ダミーのTextBoxを非表示で配置している。
--%>
<asp:TextBox id="tbDummy" runat="server" style="visibility:hidden;display:none;"></asp:TextBox>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">モール出品設定</h1>
		</td>
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
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />検索項目</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSearchKey" runat="server"/>
														</td>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />検索値</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbSearchWord" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />並び順</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="0" Selected="True">商品ID/昇順</asp:ListItem>
																<asp:ListItem Value="1">商品ID/降順</asp:ListItem>
																<asp:ListItem Value="2">商品名/昇順</asp:ListItem>
																<asp:ListItem Value="3">商品名/降順</asp:ListItem>
																<asp:ListItem Value="4">フリガナ/昇順</asp:ListItem>
																<asp:ListItem Value="5">フリガナ/降順</asp:ListItem>
																<asp:ListItem Value="6">作成日/昇順</asp:ListItem>
																<asp:ListItem Value="7">作成日/降順</asp:ListItem>
																<asp:ListItem Value="8">更新日/昇順</asp:ListItem>
																<asp:ListItem Value="9">更新日/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="3">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_EXHIBITS_CONFIG_LIST%>">クリア</a></div>
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
	<!--▽ モール出品設定ファイル登録出力 ▽-->
	<tr id="tr1">
		<td><h2 class="cmn-hed-h2">アップロード・ダウンロード</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" border="0" width="100%">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="list_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0">ファイルパス<span class="notice">*</span></td>
														<td class="list_item_bg1">
															<input id="fFile" contentEditable="false" style="WIDTH: 500px;" size="90" type="file" name="fFile" runat="server">
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<% if (lbResultMessage.Text != ""){ %>
														<td colspan="2">
															<table class="info_table" width="756" border="0" cellspacing="1" cellpadding="3">
																<tr>
																	<td class="info_item_bg">
																		<div class="list_detail">
																			<asp:Label ID="lbResultMessage" runat="server"></asp:Label>
																		</div>
																	</td>
																</tr>
															</table>
														</td>
														<% } %>
													</tr>
													<tr>
														<td align="center"><div ><asp:Button id="btnExport" Runat="server" Text="  更新用全件CSVダウンロード  " OnClick="btnExport_Click" /></div></td>
														<td align="center"><div ><asp:Button id="btnImport" Runat="server" Text="  更新用全件CSVアップロード  " OnClientClick="return exec_submit()" OnClick="btnImport_Click" /></div></td>
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
	<!--△ モール出品設定ファイル登録出力 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr id="trList" runat="server">
		<td><h2 class="cmn-hed-h2">モール出品設定一覧</h2></td>
	</tr>
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">モール出品設定編集</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div id="divStockEdit" runat="server">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="508"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<!--▽ 一覧表示 ▽-->
												<td id="tdDispList" runat="server">
													<div class="x_scrollable" style="WIDTH: 100%">
													<table class="list_table" cellspacing="1" cellpadding="3" width="<%# 428 + m_htMallExhibitsConfigName.Count * 110 %>" border="0">
														<tr class="list_title_bg">
															<td align="center" width="150">商品ID</td>
															<td align="center" width="278">商品名</td>
															<td id="Td1" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1]%></td>
															<td id="Td2" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2]%></td>
															<td id="Td3" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3]%></td>
															<td id="Td4" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4]%></td>
															<td id="Td5" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5]%></td>
															<td id="Td6" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6]%></td>
															<td id="Td7" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7]%></td>
															<td id="Td8" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8]%></td>
															<td id="Td9" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9]%></td>
															<td id="Td10" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10]%></td>
															<td id="Td11" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11]%></td>
															<td id="Td12" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12]%></td>
															<td id="Td13" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13]%></td>
															<td id="Td14" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14]%></td>
															<td id="Td15" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15]%></td>
															<td id="Td16" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16]%></td>
															<td id="Td17" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17]%></td>
															<td id="Td18" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18]%></td>
															<td id="Td19" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19]%></td>
															<td id="Td20" align="center" width="110" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20) %>" ><%# WebSanitizer.HtmlEncode((string)m_htMallExhibitsConfigName[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20])%><br/><%: (string)m_htMallExhibitsConfigColumn[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20]%></td>
														</tr>
														<asp:Repeater id="rList" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																	<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_PRODUCT_ID))%></td>
																	<td align="left">
																		<%if (ManagerMenuCache.Instance.HasOperatorMenuAuthority(Constants.PATH_ROOT + Constants.MENU_PATH_LARGE_PRODUCT)) {%>
																			<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((String)Eval(Constants.FIELD_PRODUCT_PRODUCT_ID))) %>"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME))%></a>
																		<% } else { %>
																			<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME))%>
																		<% } %>
																	</td>
																	<td id="Td21" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td22" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td23" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td24" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td25" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td26" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td27" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td28" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td29" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td30" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td31" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td32" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td33" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td34" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td35" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td36" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td37" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td38" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td39" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																	<td id="Td40" align="center" runat="server" visible="<%# m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20) %>"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLEXHIBITSCONFIG, Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20, (Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20) != DBNull.Value ? Eval(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20) : Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)))%></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" Visible="false">
															<td id="tdListErrorMessage" runat="server" colspan="2"></td>
														</tr>
													</table>
													</div>
												</td>
												<!--△ 一覧表示 △-->
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="508"></td>
															<td width="250" class="action_list_sp"></td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
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
<script language="javascript">
<!--
var exec_submit_flg = 0;
function exec_submit()
{
	if (exec_submit_flg == 0)
	{
		exec_submit_flg = 1;
		return true;
	}
	else
	{
		return false;
	}
}
//-->
</script>
</asp:Content>