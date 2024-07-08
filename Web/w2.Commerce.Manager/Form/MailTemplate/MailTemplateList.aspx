<%--
=========================================================================================================
  Module      : メールテンプレート設定一覧ページ(MailTemplateList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.MailTemplate" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailTemplateList.aspx.cs" Inherits="Form_MailTemplate_MailTemplateList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%-- 
テキストボックス内でEnterキーを押してSubmit（一番上に配置されているTextBoxのSubmit）送信しようとすると、
IEのバグでテキストボックスが画面上に一つのみ配置されている場合にSubmit送信されない不具合の対応として、
ダミーのTextBoxを非表示で配置している。
--%>
<asp:TextBox id="tbDummy" runat="server" style="visibility:hidden;display:none;"></asp:TextBox>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">メールテンプレート設定</h1></td>
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
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlSearchKey" runat="server">
																<asp:ListItem Value="0" Selected="True">メールテンプレートID</asp:ListItem>
																<asp:ListItem Value="1">メールテンプレート名</asp:ListItem>
															</asp:DropDownList></td>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />検索値</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbSearchWord" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />並び順</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="0">メールテンプレートID/昇順</asp:ListItem>
																<asp:ListItem Value="1">メールテンプレートID/降順</asp:ListItem>
																<asp:ListItem Value="2">メールテンプレート名/昇順</asp:ListItem>
																<asp:ListItem Value="3">メールテンプレート名/降順</asp:ListItem>
																<asp:ListItem Value="4">作成日/昇順</asp:ListItem>
																<asp:ListItem Value="5">作成日/降順</asp:ListItem>
																<asp:ListItem Value="6">更新日/昇順</asp:ListItem>
																<asp:ListItem Value="7">更新日/降順</asp:ListItem>
															</asp:DropDownList></td>
														<td class="search_btn_bg" width="83">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_LIST %>">クリア</a></div>
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
	<tr>
		<td><h2 class="cmn-hed-h2">メールテンプレート設定一覧</h2></td>
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
														<td width="675" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td class="action_list_sp"><asp:Button id="btnInsertTop" Runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
														<td align="center" width="15%">メールテンプレートID</td>
														<td align="center" width="40%">メールテンプレート名</td>
														<td align="center" width="18%">作成日</td>
														<td align="center" width="18%">更新日</td>
														<td align="center" width="9%">自動送信<br />フラグ</td>
													</tr>
													<asp:Repeater id="rList" Runat="server" ItemType="w2.Domain.MailTemplate.MailTemplateModel">
														<ItemTemplate>
															<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMailTemplateDetailUrl((String)Item.MailId)) %>')">
																<td align="center"><%#: Item.MailId %></td>
																<td align="left"><%#: Item.MailName %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																<td align="center"><%#: Item.AutoSendFlg.ToString().Replace("1", "〇").Replace("0", "-") %></td>
															</tr>
														</ItemTemplate>
														<AlternatingItemTemplate>
															<tr class="list_item_bg2" onmouseover="listselect_mover(this)" onmouseout="listselect_mout2(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMailTemplateDetailUrl((String)Item.MailId)) %>')">
																<td align="center"><%#: Item.MailId %></td>
																<td align="left"><%#: Item.MailName %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																<td align="center"><%#: Item.AutoSendFlg.ToString().Replace("1", "〇").Replace("0", "-") %></td>
															</tr>
														</AlternatingItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="5"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnInsertBottom" Runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															自動送信フラグが〇/-のメールテンプレートは、自動送信処理を編集画面で変更できます。<br/>
															自動送信フラグが空欄のメールテンプレートは、自動送信処理に対応していません。
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
