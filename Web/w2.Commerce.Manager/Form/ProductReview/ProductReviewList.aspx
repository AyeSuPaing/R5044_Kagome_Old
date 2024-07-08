<%--
=========================================================================================================
  Module      : レビュー一覧ページ(ProductReviewList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductReviewList.aspx.cs" Inherits="Form_ProductReview_ProductReviewList" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
// 全ての選択・解除ボタンクリック
function selected_target_all()
{
	var chk = document.getElementById("cbCheckAll").checked;
	for (i = 0; i < document.getElementsByTagName("input").length; i++) 
	{
		if (document.getElementsByTagName("input")[i].type == "checkbox")
		{
			// リピータ以外でチェックボックスを利用される場合、
			// Case文で処理対象外とする制御を追加すること
			document.getElementsByTagName("input")[i].checked = chk;
		}
	}
}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">レビュー管理</h1></td>
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
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />公開</td>
														<td class="search_item_bg" colspan="3">
															<asp:RadioButtonList ID="rblOpenFlg" runat="server" RepeatLayout="flow" RepeatDirection="Horizontal" RepeatColumns="8">
																<asp:ListItem Value="" Selected="True">全て</asp:ListItem>
																<asp:ListItem Value="1">公開</asp:ListItem>
																<asp:ListItem Value="0">非公開</asp:ListItem>
															</asp:RadioButtonList>
														</td>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />並び順</td>
														<td class="search_item_bg" width="110">
															<asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="0">投稿日/昇順</asp:ListItem>
																<asp:ListItem Value="1" Selected="True">投稿日/降順</asp:ListItem>
																<asp:ListItem Value="2">公開日/昇順</asp:ListItem>
																<asp:ListItem Value="3">公開日/降順</asp:ListItem>
																<asp:ListItem Value="4">チェック日/昇順</asp:ListItem>
																<asp:ListItem Value="5">チェック日/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="10">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTREVIEW_LIST %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />チェック</td>
														<td class="search_item_bg" colspan="3">
															<asp:RadioButtonList ID="rblCheckFlg" runat="server" RepeatLayout="flow" RepeatDirection="Horizontal" RepeatColumns="8">
																<asp:ListItem Value="" Selected="True">全て</asp:ListItem>
																<asp:ListItem Value="1">チェック済み</asp:ListItem>
																<asp:ListItem Value="0">未チェック</asp:ListItem>
															</asp:RadioButtonList>
														</td>
														<td class="search_title_bg" width="130"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />ニックネーム</td>
														<td class="search_item_bg" width="120"><asp:TextBox id="tbNickname" runat="server" Width="105"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />タイトル</td>
														<td class="search_item_bg" width="210" colspan="2"><asp:TextBox id="tbReviewTitle" runat="server" Width="200"></asp:TextBox></td>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />商品ID</td>
														<td class="search_item_bg" width="210" colspan="2"><asp:TextBox id="tbReviewProductId" runat="server" Width="200"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />コメント</td>
														<td class="search_item_bg" width="210" colspan="2"><asp:TextBox id="tbReviewComment" runat="server" Width="200"></asp:TextBox></td>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />商品名</td>
														<td class="search_item_bg" width="210" colspan="2"><asp:TextBox id="tbProductName" runat="server" Width="200"></asp:TextBox></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<%-- マスタ出力 --%>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="ProductReview" TableWidth="758" />
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
	<tr id="trListTitle" runat="server" visible="true">
		<td><h2 class="cmn-hed-h2">レビュー一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<!--▽ レビュー一覧 ▽-->
									<div id="divProductReview" runat="server" visible="true">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td align="left">
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														</tr>
													</table>
													<!--△ ページング △-->
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<div>
													<!-- ▽ テーブルヘッダ ▽ -->
													<div class="tbl_header">                                      
														<table class="list_table" cellspacing="1" cellpadding="3" width="1020">
															<tr class="list_title_bg">
																<td align="center" width="25" rowspan="2"><input id="cbCheckAll" name="cbCheckAll" type="checkbox" onclick="javascript: selected_target_all();" /></td>
																<td align="center" width="50" rowspan="2">公開</td>
																<td align="center" width="50" rowspan="2">チェック</td>
																<td align="center" width="120" rowspan="2">商品名(商品ID)</td>
																<td align="left" width="90" rowspan="2">ニックネーム</td>
																<td align="center" width="70" rowspan="2">評価</td>
																<td align="left" width="0">タイトル</td>
																<td align="center" width="50"rowspan="2">レビュー<br />詳細</td>
																<td align="center" width="75" rowspan="2">投稿日</td>
																<td align="center" width="75" rowspan="2">公開日</td>
																<td align="center" width="75" rowspan="2">チェック日</td>
															</tr>
															<tr class="list_title_bg">
																<td align="left">コメント</td>
															</tr>
														</table>
													</div>
													<!-- △ テーブルヘッダ △ -->

													<!-- ▽ テーブルデータ ▽ -->
													<div class="tbl_data">
														<table class="list_table" cellspacing="1" cellpadding="3" width="1120">
															<asp:Repeater id="rReviewList" Runat="server">
																<ItemTemplate>
																	<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																		<%--▽ 更新用 ▽--%>
																		<td style="display:none">
																			<asp:HiddenField ID="hfProductReviewUserId" runat="server" value="<%# Eval(Constants.FIELD_PRODUCTREVIEW_USER_ID) %>"/>
																			<asp:HiddenField ID="hfProductReviewProductId" runat="server" value="<%# Eval(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID) %>" />
																			<asp:HiddenField ID="hfProductReviewReviewNo" runat="server" value="<%# Eval(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO)  %>" />
																		</td>
																		<%--△ 更新用 △--%>
																		<td align="center" width="25" style="word-break:break-all"><asp:CheckBox id="cbCheckedList" runat="server"></asp:CheckBox></td>
																		<td align="center" width="50" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_OPEN_FLG, (string)Eval(Constants.FIELD_PRODUCTREVIEW_OPEN_FLG)))%></td>
																		<td align="center" width="50" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG, (string)Eval(Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG)))%></td>
																		<td align="left" width="120" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME))%>(<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID))%>)</td>
																		<%--▽ ユーザID有無で切り替え ▽--%>
																		<td align="left" width="90" style="word-break:break-all" runat="server" Visible=<%# (bool)((string)Eval(Constants.FIELD_PRODUCTREVIEW_USER_ID) == "") %>>
																			<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTREVIEW_NICK_NAME))%>
																		</td>
																		<td align="left" width="90" style="word-break:break-all" runat="server" Visible=<%# (bool)((string)Eval(Constants.FIELD_PRODUCTREVIEW_USER_ID) != "") %>>
																			<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl((string)Eval(Constants.FIELD_PRODUCTREVIEW_USER_ID))) %>','contact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																				<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTREVIEW_NICK_NAME))%>
																			</a>
																		</td>
																		<%--△ ユーザID有無で切り替え △--%>
																		<td align="center" width="70" style="word-break:break-all;"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING, Eval(Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING)))%></td>
																		<td align="center" style="padding:0px" valign="top" width="0">
																			<table cellpadding="0" cellspacing="0" width="0" height="100%">
																				<tr>
																					<td align="left" style="word-break:break-all" class="item_bottom_line"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE)) %></td>
																				</tr>
																				<tr>
																					<td align="left" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT)) %></td>
																				</tr>
																			</table>
																		</td>
																		<td align="center" width="50" style="word-break:break-all"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductReviewDetailUrl((string)Eval(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID),(Eval(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO)).ToString())) %>">参照</a></td>
																		<td align="center" width="75" style="white-space:pre-wrap"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_PRODUCTREVIEW_DATE_CREATED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																		<td align="center" width="75" style="white-space:pre-wrap"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_PRODUCTREVIEW_DATE_OPENED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																		<td align="center" width="75" style="white-space:pre-wrap"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
																	</tr>
																</ItemTemplate>
															</asp:Repeater>
															<tr id="trListError" class="list_alert" runat="server" Visible="False">
																<td id="tdErrorMessage" colspan="11" runat="server"></td>
															</tr>
														</table>
													</div>
													<!-- △ テーブルデータ △ -->
													</div>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td align="right">
													<table cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td>
																<table class="list_table" cellspacing="1" cellpadding="3" border="0" style="height:30px">
																	<tr>
																		<td class="list_title_bg" width="130" align="center">
																			公開</td>
																		<td class="list_item_bg1">&nbsp;
																			<asp:DropDownList id="ddlUpdateOpenFlg" runat="server">
																				<asp:ListItem Value="" Selected="True">選択してください</asp:ListItem>
																				<asp:ListItem Value="1" >公開</asp:ListItem>
																				<asp:ListItem Value="0" >非公開</asp:ListItem>
																			</asp:DropDownList>
																		</td>
																	</tr>
																</table>
															</td>
														</tr>
														<tr>
															<td>
																<table class="list_table" cellspacing="1" cellpadding="3" border="0" style="height:30px">
																	<tr>
																		<td class="list_title_bg" width="130" align="center">
																			チェック</td>
																		<td class="list_item_bg1">&nbsp;
																			<asp:DropDownList id="ddlUpdateCheckFlg" runat="server">
																				<asp:ListItem Value="" Selected="True">選択してください</asp:ListItem>
																				<asp:ListItem Value="1" >チェック済</asp:ListItem>
																				<asp:ListItem Value="0" >未チェック</asp:ListItem>
																			</asp:DropDownList>
																		</td>
																	</tr>
																</table>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td align="right">
													<asp:Button ID="btnDeleteReview" Text="  選択行を削除  " runat="server" OnClientClick="return confirm('選択された商品レビューは削除されます。よろしいですか？');" OnClick="btnDeleteReview_Click" CssClass="cmn-btn-sub-action" />
													<asp:Button ID="btnUpdateStatus" Text="  選択した処理を実行  " runat="server" OnClick="btnUpdateStatus_Click" CssClass="cmn-btn-sub-action" />
												</td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</div>
									<!--△ レビュー一覧 △-->
									<!--▽ 状態更新結果一覧 ▽-->
									<div id="divProductReviewComplete" runat="server" visible="false">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="list_pager" cellspacing="0" cellpadding="0" border="0" width="100%">
														<tr>
															<td width="638">以下のレビューを更新いたしました｡</td>
															<td align="right"><asp:Button id="btnRedirectReviewTop" Runat="server" Text="  一覧へ戻る  " onclick="btnRedirectReview_Click" CssClass="cmn-btn-sub-action" ></asp:Button></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="750" border="0">
														<tr class="list_title_bg">
															<td align="center" width="50" rowspan="2">公開</td>
															<td align="center" width="50" rowspan="2">チェック</td>
															<td align="center" width="120" rowspan="2">商品名(商品ID)</td>
															<td align="left" width="90" rowspan="2">ニックネーム</td>
															<td align="center" width="70" rowspan="2">評価</td>
															<td align="left" width="283">タイトル</td>
															<td align="center" width="50"rowspan="2">結果</td>
														</tr>
														<tr class="list_title_bg">
															<td align="left">コメント</td>
														</tr>
														<asp:Repeater id="rReviewListComplete" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																	<td align="center" width="50" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_OPEN_FLG, Eval(Constants.FIELD_PRODUCTREVIEW_OPEN_FLG))) %></td>
																	<td align="center" width="50" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG, Eval(Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG)))%></td>
																	<td align="left" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME))%>(<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID))%>)</td>
																	<td align="left" width="90" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTREVIEW_NICK_NAME))%></td>
																	<td align="center" width="70" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING, Eval(Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING)))%></td>
																	<td align="center" style="padding:0px" valign="top">
																		<table cellpadding="0" cellspacing="0" width="100%" height="100%">
																			<tr>
																				<td align="left" width="283" style="word-break:break-all" class="item_bottom_line"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE)) %></td>
																			</tr>
																			<tr>
																				<td align="left" width="283" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT)) %></td>
																			</tr>
																		</table>
																	</td>
																	<td align="center" width="50" style="word-break:break-all"><%# GetUpdateStatusResult((string)Eval(Constants.FIELD_PRODUCTREVIEW_SHOP_ID), (string)Eval(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID),  (int)Eval(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO))%></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td align="right"><asp:Button id="btnRedirectReviewBottom" Runat="server" Text="  一覧へ戻る  " onclick="btnRedirectReview_Click" CssClass="cmn-btn-sub-action" ></asp:Button></td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</div>
									<!--△ 状態更新結果一覧 △-->
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
		scrollLeftTwoTable("tbl_header", "tbl_data");
	});
</script>
</asp:Content>
