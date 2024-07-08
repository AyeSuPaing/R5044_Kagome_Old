<%--
=========================================================================================================
  Module      : ポイントキャンペーンルール一覧ページ(PointRuleCampaignList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PointRuleCampaignList.aspx.cs" Inherits="Form_PointRuleCampaign_PointRuleCampaignList" %>
<%@ Import Namespace="w2.App.Common.Extensions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">キャンペーン設定</h1></td>
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
														<td class="search_title_bg" width="130"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />ポイント加算区分</td>
														<td class="search_item_bg" width="208"><asp:DropDownList id="ddlPointIncKbn" runat="server"></asp:DropDownList></td>
														<td class="search_title_bg" width="130"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />並び順</td>
														<td class="search_item_bg" width="207"><asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="0">優先度/昇順</asp:ListItem>
																<asp:ListItem Value="1">優先度/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " onclick="btnSearch_Click"></asp:Button></div>
															<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_LIST %>">クリア</a></div>
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
		<td><h2 class="cmn-hed-h2">キャンペーン設定一覧</h2></td>
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
														<td width="675"><asp:label id="lbPager1" Runat="server"></asp:label></td>
														<td width="83" class="action_list_sp"><asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " onclick="btnInsertTop_Click"></asp:Button></td>
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
														<td align="center" width="190">キャンペーン名</td>
														<td align="center" width="100">ポイント区分</td>
														<td align="center" width="140">ポイント加算区分</td>
														<td align="center" width="100">ポイント加算ルール</td>
														<td align="center" width="180">キャンペーン有効期間</td>
														<td align="center" width="50">優先度</td>
														<td align="center" width="50">有効フラグ</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<HeaderTemplate>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreatePointRuleCampaignDetailUrl(Container.ToModel<WrappedSearchResult>().PointRuleId)) %>')">
																<td align="left">&nbsp;
																	<%#: Container.ToModel<WrappedSearchResult>().PointRuleName%></td>
																<td align="center">
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_KBN, Container.ToModel<WrappedSearchResult>().PointKbn)%></td>
																<td align="left">&nbsp;
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_INC_KBN, Container.ToModel<WrappedSearchResult>().PointIncKbn)%></td>
																<td align="left">&nbsp;
																	<span runat="server" Visible="<%# (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType != Constants.FLG_POINTRULE_INC_TYPE_PRODUCT) && (string.IsNullOrEmpty(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType) == false) %>">通常：</span>
																	<asp:Literal runat="server" id ="lPointRule" Text='<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().IncType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().IncNum) + "pt" : Container.ToModel<WrappedSearchResult>().IncType == Constants.FLG_POINTRULE_INC_TYPE_RATE ? StringUtility.ToEmpty(Container.ToModel<WrappedSearchResult>().IncRate) + "%" : "商品毎に設定")%>'></asp:Literal>
																	<span id="sFixedPurchasePoint" runat="server" Visible="<%# (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType != Constants.FLG_POINTRULE_INC_TYPE_PRODUCT) && (string.IsNullOrEmpty(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType) == false) %>"><br />&nbsp;
																	定期：<%#: Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseNum) + "pt" : Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE ? StringUtility.ToEmpty(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseRate) + "%" : "商品毎に設定" %></span></td>
																<td align="center">
																	<%#: DateTimeUtility.ToStringForManager(Container.ToModel<WrappedSearchResult>().ExpBgn, DateTimeUtility.FormatType.ShortDate2Letter)%>
																	～
																	<%#: DateTimeUtility.ToStringForManager(Container.ToModel<WrappedSearchResult>().ExpEnd, DateTimeUtility.FormatType.ShortDate2Letter)%></td>
																<td align="center">
																	<%#: Container.ToModel<WrappedSearchResult>().Priority%></td>
																<td align="center">
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_VALID_FLG, Container.ToModel<WrappedSearchResult>().ValidFlg)%></td>
															</tr>
														</ItemTemplate>
														<AlternatingItemTemplate>
															<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreatePointRuleCampaignDetailUrl(Container.ToModel<WrappedSearchResult>().PointRuleId)) %>')">
																<td align="left">&nbsp;
																	<%#: Container.ToModel<WrappedSearchResult>().PointRuleName%></td>
																<td align="center">
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_KBN, Container.ToModel<WrappedSearchResult>().PointKbn)%></td>
																<td align="left">&nbsp;
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_INC_KBN, Container.ToModel<WrappedSearchResult>().PointIncKbn)%></td>
																<td align="left">&nbsp;
																	<span runat="server" Visible="<%# (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType != Constants.FLG_POINTRULE_INC_TYPE_PRODUCT) && (string.IsNullOrEmpty(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType) == false) %>">通常：</span>
																	<asp:Literal runat="server" id ="lPointRule" Text='<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().IncType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().IncNum) + "pt" : Container.ToModel<WrappedSearchResult>().IncType == Constants.FLG_POINTRULE_INC_TYPE_RATE ? StringUtility.ToEmpty(Container.ToModel<WrappedSearchResult>().IncRate) + "%" : "商品毎に設定")%>'></asp:Literal>
																	<span id="sFixedPurchasePoint" runat="server" Visible="<%# (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType != Constants.FLG_POINTRULE_INC_TYPE_PRODUCT) && (string.IsNullOrEmpty(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType) == false) %>"><br />&nbsp;
																	定期：<%#: Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseNum) + "pt" : Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE ? StringUtility.ToEmpty(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseRate) + "%" : "商品毎に設定" %></span></td>
																<td align="center">
																	<%#: DateTimeUtility.ToStringForManager(Container.ToModel<WrappedSearchResult>().ExpBgn, DateTimeUtility.FormatType.ShortDate2Letter)%>
																	～
																	<%#: DateTimeUtility.ToStringForManager(Container.ToModel<WrappedSearchResult>().ExpEnd, DateTimeUtility.FormatType.ShortDate2Letter)%></td>
																<td align="center">
																	<%#: Container.ToModel<WrappedSearchResult>().Priority%></td>
																<td align="center">
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_VALID_FLG, Container.ToModel<WrappedSearchResult>().ValidFlg)%></td>
															</tr>
														</AlternatingItemTemplate>
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
											<td class="action_list_sp"><asp:Button id="btnInsertBotttom" runat="server" Text="  新規登録  " onclick="btnInsertTop_Click"></asp:Button></td>
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