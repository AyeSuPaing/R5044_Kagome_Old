<%--
=========================================================================================================
  Module      : ポイント基本ルール確認ページ(PointRuleConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PointRuleConfirm.aspx.cs" Inherits="Form_PointRule_PointRuleConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">基本ルール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">基本ルール設定詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">基本ルール設定確認</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top"><input type="button" onclick="Javascript:history.back();" value="  戻る  " />
													<asp:Button id="btnEditTop" runat="server" Text="  編集する  " Visible="False" onclick="btnEditTop_Click"></asp:Button>
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" onclick="btnCopyInsertTop_Click"></asp:Button>
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " Visible="False" onclick="btnDeleteTop_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？')"></asp:Button>
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" onclick="btnInsertTop_Click"></asp:Button>
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" onclick="btnUpdateTop_Click"></asp:Button></div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr id="trPointRuleId" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">ルールID</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(Input.PointRuleId)%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">ルール名</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(Input.PointRuleName)%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">ポイント加算区分</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_INC_KBN, Input.PointIncKbn))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">ポイント区分</td>
														<td class="detail_item_bg" align="left"><%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_KBN, this.Input.PointKbn) %></td>
													</tr>
													<tr id="trIncType" runat="server">
														<td class="detail_title_bg" align="left" width="30%">ポイント計算方法</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(Input.IncType == Constants.FLG_POINTRULE_INC_TYPE_PRODUCT ? "商品毎" : "購入金額毎")%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">ポイント加算ルール</td>
														<td class="detail_item_bg" align="left"><span runat="server" Visible="<%# (this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM) || (this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE) %>">通常：</span><asp:Literal runat="server" id ="lPointRule" Text='<%# WebSanitizer.HtmlEncode(this.Input.IncType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? StringUtility.ToNumeric(Input.IncNum) + "pt" : this.Input.IncType == Constants.FLG_POINTRULE_INC_TYPE_RATE ? StringUtility.ToEmpty(this.Input.IncRate) + "%" : "商品情報のポイント数を使用")%>'></asp:Literal><br/>
															<span runat="server" Visible="<%# (this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM) || (this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE) %>">
															定期：<%# WebSanitizer.HtmlEncode(this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM ? StringUtility.ToNumeric(this.Input.IncFixedPurchaseNum) + "pt" : this.Input.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE ? StringUtility.ToEmpty(this.Input.IncFixedPurchaseRate) + "%" : lPointRule.Text)%>
															</span>
														</td>
													</tr>
													<tr Visible="<%# this.Input.IsLimitedTermPoint %>" runat="server">
														<td class="detail_title_bg" align="left" width="30%">期間限定ポイント有効期限</td>
														<td class="detail_item_bg" align="left">
															<% if (string.IsNullOrEmpty(this.Input.EffectiveOffset) == false) { %>
																<% if (int.Parse(this.Input.EffectiveOffset) > 0) { %>
																	発行の<%#: this.Input.EffectiveOffset %><%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET_TYPE, this.Input.EffectiveOffsetType) %>から
																<% } else { %>
																	発行から
																<% } %>
																<%#: this.Input.Term %><%#: ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_TERM_TYPE, this.Input.TermType) %>有効
															<% } else { %>
																<%#: DateTimeUtility.ToStringForManager(Input.PeriodBegin, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) + "～" + DateTimeUtility.ToStringForManager(Input.PeriodEnd, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
															<% } %>
														</td>
													</tr>
													<% if (Constants.CROSS_POINT_OPTION_ENABLED == false) { %>
													<tr id="trPointExpEntend" runat="server">
														<td class="detail_title_bg" align="left" width="30%">ポイント有効期限の延長設定</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Input.PointExpExtendMonth != "00" ? StringUtility.ToEmpty(int.Parse(Input.PointExpExtendMonth)) + "ヶ月" : "-"))%></td>
													</tr>
													<% } %>
													<tr id="trPointExpire" runat="server">
														<td class="detail_title_bg" align="left" width="30%">ルール有効期間</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(Input.ExpBgn, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) + "～" + DateTimeUtility.ToStringForManager(Input.ExpEnd, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">優先順位</td>
														<td class="detail_item_bg" align="left">100</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">有効フラグ</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_VALID_FLG, Input.ValidFlg))%></td>
													</tr>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">作成日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(Input.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
													</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">更新日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(Input.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">最終更新者</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(Server.HtmlEncode(Input.LastChanged)) %></td>
													</tr>
													<tr>
														<tr id="trUrl" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">URL</td>
														<td class="detail_item_bg" align="left"><a href="<%= this.Url %>" target="_blank"><%: this.Url %></a></td>
													</tr>
												</table>
												<div class="action_part_bottom"><input type="button" onclick="Javascript:history.back();" value="  戻る  " />
													<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " Visible="False" onclick="btnEditTop_Click"></asp:Button>
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" onclick="btnCopyInsertTop_Click"></asp:Button>
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " Visible="False" onclick="btnDeleteTop_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？')"></asp:Button>
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" onclick="btnInsertTop_Click"></asp:Button>
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" onclick="btnUpdateTop_Click"></asp:Button></div>
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>