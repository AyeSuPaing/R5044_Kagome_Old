<%--
=========================================================================================================
  Module      : ポイント基本ルール一覧ページ(PointRuleList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PointRuleList.aspx.cs" Inherits="Form_PointRule_PointRuleList" %>
<%@ Import Namespace="w2.App.Common.Extensions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="820" border="0">
	<tr>
		<td><h1 class="page-title">基本ルール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">基本ルール設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="813" border="0">
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
												<table class="list_pager" cellspacing="0" cellpadding="0" width="813" border="0">
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
												<table class="list_table" cellspacing="1" cellpadding="3" width="813" border="0">
													<tr class="list_title_bg">
														<td align="center" width="220">ルール名</td>
														<td align="center" width="115">ポイント区分</td>
														<td align="center" width="140">ポイント加算区分</td>
														<td align="center" width="128">ポイント加算ルール</td>
														<td align="center" width="140">ルール有効期間</td>
														<td align="center" width="70">有効フラグ</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<HeaderTemplate>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreatePointRuleDetailUrl(Container.ToModel<WrappedSearchResult>().PointRuleId)) %>')">
																<td align="left">&nbsp;
																	<%#: Container.ToModel<WrappedSearchResult>().PointRuleName %>
																	<%# Container.ToModel<WrappedSearchResult>().CampaignValidFlg == "1" ? "<span class=\"notice\">*</span>" : ""%></td>
																<td align="center">
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_KBN, Container.ToModel<WrappedSearchResult>().PointKbn) %></td>
																<td align="left">&nbsp;
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_INC_KBN, Container.ToModel<WrappedSearchResult>().PointIncKbn) %></td>
																<td align="left">&nbsp;
																<span runat="server" Visible="<%# (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM) || (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE) %>">通常：</span>
																	<asp:Literal runat="server" ID ="lPointRule" Text='<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().IncType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().IncNum) + "pt" : Container.ToModel<WrappedSearchResult>().IncType == Constants.FLG_POINTRULE_INC_TYPE_RATE ? StringUtility.ToEmpty(Container.ToModel<WrappedSearchResult>().IncRate) + "%" : "商品毎に設定")%>'></asp:Literal>
																	<span id="sFixedPurchasePoint" runat="server" Visible="<%# (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM) || (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE) %>"><br />&nbsp;
																	定期：<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseNum) + "pt" : Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE ? StringUtility.ToEmpty(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseRate) + "%" : "商品毎に設定") %></td>
																	</span>
																<td align="center">
																	<%#: DateTimeUtility.ToStringForManager(Container.ToModel<WrappedSearchResult>().ExpBgn, DateTimeUtility.FormatType.ShortDate2Letter)%>
																	<%#: (Container.ToModel<WrappedSearchResult>().ExpBgn.HasValue && Container.ToModel<WrappedSearchResult>().ExpBgn.HasValue) ? "～" : "" %>
																	<%#: DateTimeUtility.ToStringForManager(Container.ToModel<WrappedSearchResult>().ExpEnd, DateTimeUtility.FormatType.ShortDate2Letter)%></td>
																<td align="center">
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_VALID_FLG, Container.ToModel<WrappedSearchResult>().ValidFlg) %></td>
															</tr>
														</ItemTemplate>
														<AlternatingItemTemplate>
															<tr class="list_item_bg2" onmouseover="listselect_mover(this)" onmouseout="listselect_mout2(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreatePointRuleDetailUrl(Container.ToModel<WrappedSearchResult>().PointRuleId)) %>')">
																<td align="left">&nbsp;
																	<%#: Container.ToModel<WrappedSearchResult>().PointRuleName %>
																	<%# Container.ToModel<WrappedSearchResult>().CampaignValidFlg == "1" ? "<span class=\"notice\">*</span>" : ""%></td>
																<td align="center">
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_KBN, Container.ToModel<WrappedSearchResult>().PointKbn) %></td>
																<td align="left">&nbsp;
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_INC_KBN, Container.ToModel<WrappedSearchResult>().PointIncKbn) %></td>
																<td align="left">&nbsp;
																<span runat="server" Visible="<%# (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM) || (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE) %>">通常：</span>
																	<asp:Literal runat="server" ID ="lPointRule" Text='<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().IncType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().IncNum) + "pt" : Container.ToModel<WrappedSearchResult>().IncType == Constants.FLG_POINTRULE_INC_TYPE_RATE ? StringUtility.ToEmpty(Container.ToModel<WrappedSearchResult>().IncRate) + "%" : "商品毎に設定")%>'></asp:Literal>
																	<span id="sFixedPurchasePoint" runat="server" Visible="<%# (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM) || (Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE) %>"><br />&nbsp;
																	定期：<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseNum) + "pt" : Container.ToModel<WrappedSearchResult>().IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE ? StringUtility.ToEmpty(Container.ToModel<WrappedSearchResult>().IncFixedPurchaseRate) + "%" : "商品毎に設定") %></td>
																	</span>
																<td align="center">
																	<%#: DateTimeUtility.ToStringForManager(Container.ToModel<WrappedSearchResult>().ExpBgn, DateTimeUtility.FormatType.ShortDate2Letter)%>
																	<%#: (Container.ToModel<WrappedSearchResult>().ExpBgn.HasValue && Container.ToModel<WrappedSearchResult>().ExpBgn.HasValue) ? "～" : "" %>
																	<%#: DateTimeUtility.ToStringForManager(Container.ToModel<WrappedSearchResult>().ExpEnd, DateTimeUtility.FormatType.ShortDate2Letter)%></td>
																<td align="center">
																	<%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_VALID_FLG, Container.ToModel<WrappedSearchResult>().ValidFlg) %></td>
															</tr>
														</AlternatingItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="False">
														<td id="tdErrorMessage" colspan="6" runat="server"></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															ルール名の後に<span class="notice">*</span>があるルールは、キャンペーン設定で同じポイント加算区分のポイントルールが設定されています。<br />
															キャンペーン設定の「基本ルールとの二重適用を許可」がオフの場合、キャンペーン設定が優先して適用されます。
														</td>
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
