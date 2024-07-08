<%--
=========================================================================================================
  Module      : 名称翻訳設定一覧ページ(NameTranslationSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="NameTranslationSettingList.aspx.cs" Inherits="Form_NameTranslationSetting_NameTranslationSettingList" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">名称翻訳設定</h1></td>
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
											<td>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															対象データ区分
															<br />
															翻訳対象項目
														</td>
														<td class="search_item_bg">
															<asp:UpdatePanel runat="server">
															<ContentTemplate>
															<asp:DropDownList ID="ddlConditionDataKbn" DataValueField="Value" DataTextField="Text" AutoPostBack="True" OnSelectedIndexChanged="ddlConditionDataKbn_OnSelectedIndexChanged" runat="server" />
															<br />
															<asp:DropDownList ID="ddlConditionTranslationTargetColumn" Width="300px" DataValueField="Value" DataTextField="Text" runat="server" />
															</ContentTemplate>
															</asp:UpdatePanel>
														</td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															マスタID
														</td>
														<td class="search_item_bg">
															<div>
																<span>ID1:</span>
																<asp:TextBox ID="tbConditionMasterId1" Width="125" runat="server" />
															</div>
															<div>
																<span>ID2:</span>
																<asp:TextBox ID="tbConditionMasterId2" Width="125" runat="server" />
															</div>
														</td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															言語コード(ロケールID)
														</td>
														<td class="search_item_bg">
															<asp:DropDownList ID="ddlConditionLanguages" DataValueField="Value" DataTextField="Text" runat="server" />
														</td>
														<td class="search_btn_bg" width="83" rowspan="4">
															<div class="search_btn_main"><asp:Button runat="server" ID="btnSearch" Text="　検索　" OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%: Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_LIST %>">クリア</a>
																<a href="javascript:document.<%: this.Form.ClientID %>.reset();">リセット</a>
															</div>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
	</tr>
	<tr>
		<td><h2 class="cmn-hed-h2">名称翻訳設定一覧</h2></td>
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
										<% if (divComp.Visible == false) { %>
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<% } %>
										<tr>
											<td>
												<div id="divComp" runat="server" class="action_part_top" visible="false">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">適用優先順を更新しました。</td>
														</tr>
													</table>
												</div>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="587" style="height: 22px">
															<asp:Label ID="lbPager" runat="server"></asp:Label>
														</td>
														<td class="action_list_sp">
															<asp:Button ID="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
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
														<td align="center" width="160px">対象データ区分</td>
														<td align="center" width="230px">翻訳対象項目</td>
														<td align="center" width="150px">マスタID</td>
														<td align="center" width="auto">翻訳名称</td>
													</tr>
													<asp:Repeater id="rNameTranslationSettingList" ItemType="w2.Domain.NameTranslationSetting.Helper.NameTranslationSettingContainer" OnItemDataBound="rNameTranslationSettingList_OnItemDataBound" runat="server">
													<ItemTemplate>
													<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)"  onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRegisterUrl(Constants.ACTION_STATUS_UPDATE, Item.DataKbn, Item.TranslationTargetColumn, Item.MasterId1, Item.MasterId2)) %>')">
														<td><%#: Item.DataKbnName %></td>
														<td><%#: Item.TranslationTargetColumnName %></td>
														<td>
															<div Visible="<%# (string.IsNullOrEmpty(Item.MasterId1) == false) %>" runat="server">
																<span>ID1:</span>
																<%#: Item.MasterId1 %>
															</div>
															<div Visible="<%# (string.IsNullOrEmpty(Item.MasterId2) == false) %>" runat="server">
																<span>ID2:</span>
																<%#: Item.MasterId2 %>
															</div>
														</td>
														<td>
															<table class="list_table" cellspacing="1" cellpadding="3" border="0">
																<tr>
																	<td class="list_title_bg" style="width: 85px">翻訳前名称</td>
																	<td class="list_item_bg1" style="width: auto; word-break: break-all; background-color: #FFF2EB; ">
																		<span style="color:red"><%# (Item.BeforeTranslationalName == null) ? "該当データがマスタに存在しません" : "" %></span>
																		<pre><%# WebSanitizer.HtmlEncodeChangeToBr(Item.BeforeTranslationalName) %></pre>
																	</td>
																</tr>
															</table>
															<br />
															<table class="list_table" cellspacing="1" cellpadding="3" border="0">
																<asp:Repeater ID="rLanguages" ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel" runat="server">
																<ItemTemplate>
																<tr>
																	<td class="list_title_bg" style="width: 85px"><%#: Item.LanguageCode %>(<%#: Item.LanguageLocaleId %>)</td>
																	<td class="list_item_bg1" style="width: auto"><pre><%# WebSanitizer.HtmlEncodeChangeToBr(Item.AfterTranslationalName) %></pre></td>
																</tr>
																</ItemTemplate>
																</asp:Repeater>
															</table>
														</td>
													</tr>
													</ItemTemplate>
												</asp:Repeater>
												<tr id="trListError" class="list_alert" runat="server" Visible="false">
													<td id="tdErrorMessage" runat="server" colspan="6"></td>
												</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp">
												<asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
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
</table>

</asp:Content>