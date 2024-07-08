<%--
=========================================================================================================
  Module      : 回答例文章設定一覧ページ(AnswerTemplateList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.AnswerTemplate" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AnswerTemplateList.aspx.cs" Inherits="Form_AnswerTemplate_AnswerTemplateList" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">回答例文章設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border" cellspacing="0" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table cellspacing="1" cellpadding="0" width="100%" border="0">
							<tr>
								<td class="search_box_bg">
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td align="center">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td class="search_box">
															<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
																<tr>
																	<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />回答例カテゴリ</td>
																	<td class="search_item_bg"><asp:dropdownlist id="ddlCategory" CssClass="select2-select" runat="server" Width="80%"></asp:dropdownlist></td>
																	<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />回答例件名</td>
																	<td class="search_item_bg"><asp:Textbox ID="tbAnswerMailTitle" runat="server" Width="160" /></td>
																	<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />有効フラグ</td>
																	<td class="search_item_bg"><asp:DropDownList id="ddlValidFlg" runat="server" ></asp:DropDownList></td>
																	<td class="search_btn_bg" width="83" rowspan="2">
																	    <div class="search_btn_main"><asp:button id="btnSearch" runat="server" Text="　検索　" onclick="btnSearch_Click"></asp:button></div>
																	    <div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATE_LIST %>">クリア</a></div>
																	</td>
																</tr>
																<tr>
																	<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />回答例タイトル</td>
																	<td class="search_item_bg"><asp:textbox id="tbAnswerTitle" runat="server" Width="160"></asp:textbox></td>
																	<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />回答例本文</td>
																	<td class="search_item_bg" colspan="3"><asp:Textbox ID="tbAnswerText" runat="server" Width="320" /></td>
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
			</table>
		</td>
	</tr>
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">回答例文章設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="0" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table cellspacing="1" cellpadding="0" width="100%" border="0">
							<tr>
								<td>
									<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td align="center">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<!--▽ ページング ▽-->
															<table class="list_pager" cellspacing="0" cellpadding="0" border="0" width="758">
																<tr>
																	<td width="675"><asp:label id="lbPager1" Runat="server"></asp:label></td>
																	<td width="83" class="action_list_sp" style="height: 22px"><asp:button id="btnInsertTop" runat="server" Text="　新規登録　" onclick="btnInsert_Click" /></td>
																</tr>
															</table>
															<!--△ ページング △--></td>
													</tr>
													<tr>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tr class="list_title_bg">
																	<td align="center" width="100">回答例カテゴリ</td>
																	<td align="center" width="160">回答例タイトル</td>
																	<td align="center" width="160">回答例件名</td>
																	<td align="center" width="358">回答例本文</td>
																	<td align="center" width="60">表示順</td>
																	<td align="center" width="80">有効フラグ</td>
																</tr>
																<asp:repeater id="rList" Runat="server">
																	<ItemTemplate>
																		<tr class="list_item_bg<%# (Container.ItemIndex % 2) + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# (Container.ItemIndex % 2) + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateDetailUrl((((CsAnswerTemplateModel)Container.DataItem).AnswerId)) %>')">
																			<td align="center"><%# WebSanitizer.HtmlEncode(((CsAnswerTemplateModel)Container.DataItem).EX_CategoryName).TrimStart() %></td>
																			<td align="left" title="<%# WebSanitizer.HtmlEncode(((CsAnswerTemplateModel)Container.DataItem).AnswerTitle) %>">&nbsp;<%# WebSanitizer.HtmlEncode(AbbreviateString(((CsAnswerTemplateModel)Container.DataItem).AnswerTitle, 12)) %></td>
																			<td align="left" title="<%#: ((CsAnswerTemplateModel)Container.DataItem).AnswerMailTitle %>">
																				&nbsp;<%#: AbbreviateString(((CsAnswerTemplateModel)Container.DataItem).AnswerMailTitle, 12) %></td>
																			<td align="left" title="<%# WebSanitizer.HtmlEncode(((CsAnswerTemplateModel)Container.DataItem).AnswerText) %>"><%# WebSanitizer.HtmlEncode(AbbreviateString(((CsAnswerTemplateModel)Container.DataItem).AnswerText, 32)) %></td>
																			<td align="center"><%# WebSanitizer.HtmlEncode(((CsAnswerTemplateModel)Container.DataItem).DisplayOrder) %></td>
																			<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_CSANSWERTEMPLATE, Constants.FIELD_CSANSWERTEMPLATE_VALID_FLG, ((CsAnswerTemplateModel)Container.DataItem).ValidFlg)) %></td>
																		</tr>
																	</ItemTemplate>
																</asp:repeater>
																<tr id="trListError" class="list_alert" runat="server" Visible="False">
																	<td id="tdErrorMessage" colspan="6" runat="server"></td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td class="action_part_bottom"><asp:Button id="btnInsertBotttom" runat="server" Text="　新規登録　" onclick="btnInsert_Click"></asp:Button></td>
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
		$(function () {
			ddlSelect();
		});
	</script>
</asp:Content>
