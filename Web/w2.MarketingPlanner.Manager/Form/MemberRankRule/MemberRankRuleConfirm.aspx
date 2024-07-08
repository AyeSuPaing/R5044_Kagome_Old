<%--
=========================================================================================================
  Module      : 会員ランク変動ルール確認ページ(MemberRankRuleConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MemberRankRuleConfirm.aspx.cs" Inherits="Form_MemberRankRule_MemberRankRuleConfirm" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<%@ Import Namespace="w2.App.Common.FacebookConversion" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">会員ランク変動ルール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<tr>
		<% if (m_strActionStatus == Constants.ACTION_STATUS_DETAIL){%>
		<td><h2 class="cmn-hed-h2">会員ランク変動ルール設定詳細</h2></td>
		<%} %>
		<% if (m_strActionStatus == Constants.ACTION_STATUS_CONFIRM){%>
		<td><h2 class="cmn-hed-h2">会員ランク変動ルール設定確認</h2></td>
		<%} %>
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
												<div class="action_part_top"><input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnEdit" runat="server" Text="  編集する  " OnClick="btnEdit_Click"></asp:Button>
													<asp:Button id="btnCopyInsert" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click"></asp:Button>
													<asp:Button id="btnDelete" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？')"></asp:Button>
													<asp:Button id="btnInsert" runat="server" Text="  登録する  " OnClick="btnInsert_Click"></asp:Button>
													<asp:Button id="btnUpdate" runat="server" Text="  更新する  " OnClick="btnUpdate_Click"></asp:Button>
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trId" runat="server" visible="false">
														<td class="detail_title_bg" align="left" width="30%" colspan="2">会員ランク変動ルールID</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lMemberRankRuleId" runat="server">
																<%: this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID] %>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%" colspan="2">会員ランク変動ルール名</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lMemberRankRuleName" runat="server">
																<%: this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME] %>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="15%" rowspan="<%= (Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE) ? 5 : 4 %>">抽出条件</td>
														<td class="detail_title_bg" width="15%">集計期間</td>
														<td class="detail_item_bg">
															<asp:Label ID="lTargetExtractType" runat="server">
																<%:
																	((string)this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TYPE] == Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DURING)
																		? string.Format("{0}～{1}",
																			(string.IsNullOrEmpty(StringUtility.ToEmpty(this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START])) == false)
																				? DateTime.Parse(
																					StringUtility.ToEmpty(this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START])).ToString("yyyy/MM/dd HH:mm:ss")
																				: string.Empty,
																			(string.IsNullOrEmpty(StringUtility.ToEmpty(this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END])) == false)
																				? DateTime.Parse(
																					StringUtility.ToEmpty(this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END])).ToString("yyyy/MM/dd HH:mm:ss")
																				: string.Empty)
																		: string.Format("{0} 日前 ～ 当日",
																			this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO])
																%>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="15%">集計期間内の<br />合計購入金額</td>
														<td class="detail_item_bg">
															<asp:Label ID="lTargetExtractTotalPrice" runat="server">
																<%:
																	(string.IsNullOrEmpty(this.MemberRankRuleTargetExtractTotalPriceFrom)
																		&& string.IsNullOrEmpty(this.MemberRankRuleTargetExtractTotalPriceTo))
																			? "-"
																			: string.Format("{0}～{1}",
																				(string.IsNullOrEmpty(this.MemberRankRuleTargetExtractTotalPriceFrom) == false)
																					? string.Format("{0} {1}以上",
																						ChangePriceForDisplay(this.MemberRankRuleTargetExtractTotalPriceFrom),
																						CurrencyManager.IsJapanKeyCurrencyCode ? "円" : string.Empty)
																					: string.Empty,
																				(string.IsNullOrEmpty(this.MemberRankRuleTargetExtractTotalPriceTo) == false)
																					? string.Format("{0} {1}以下",
																						ChangePriceForDisplay(this.MemberRankRuleTargetExtractTotalPriceTo),
																						CurrencyManager.IsJapanKeyCurrencyCode ? "円" : string.Empty)
																					: string.Empty)
																%>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="15%">集計期間内の<br />合計購入回数</td>
														<td class="detail_item_bg">
															<asp:Label ID="lTargetExtractTotalCount" runat="server">
																<%:
																	(string.IsNullOrEmpty(this.MemberRankRuleTargetExtractTotalCountFrom)
																		 && string.IsNullOrEmpty(this.MemberRankRuleTargetExtractTotalCountTo))
																			? "-"
																			: string.Format("{0}～{1}",
																				(string.IsNullOrEmpty(this.MemberRankRuleTargetExtractTotalCountFrom) == false)
																					? string.Format("{0} 回以上", this.MemberRankRuleTargetExtractTotalCountFrom)
																					: string.Empty,
																				(string.IsNullOrEmpty(this.MemberRankRuleTargetExtractTotalCountTo) == false)
																					? string.Format("{0} 回以下", this.MemberRankRuleTargetExtractTotalCountTo)
																					: string.Empty)
																%>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" width="15%">旧会員ランク時の<br />(注文)情報も抽出</td>
														<td class="detail_item_bg">
															<asp:Label ID="lTargetExtractOldRankFlg" runat="server">
																<%:
																	ValueText.GetValueText(
																		Constants.TABLE_MEMBERRANKRULE,
																		Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG,
																		this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG])
																%>
															</asp:Label>
														</td>
													</tr>
													<% if (Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE) { %>
													<tr>
														<td class="detail_title_bg" align="left" width="15%">ターゲット</td>
														<td class="detail_item_bg" align="left">
															<asp:Repeater ID="rTargetLists" runat="server">
																<ItemTemplate>
																	・<%#: ((Hashtable)Container.DataItem)["target_name"] %>
																	<%#: ((string)((Hashtable)Container.DataItem)["target_extract_flg"] == Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON) ? "[適用時抽出]" : "" %>
																	<br />
																</ItemTemplate>
															</asp:Repeater>
														</td> 
													</tr>
													<% } %>
													<tr>
														<td class="detail_title_bg" align="left" width="30%" colspan="2">ランク付与方法</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lRankChangeType" runat="server">
																<%:
																	ValueText.GetValueText(Constants.TABLE_MEMBERRANKRULE,
																		Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE,
																		this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE])
																%>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%" colspan="2">指定付与ランク</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lRankChangeRank" runat="server">
																<%: MemberRankOptionUtility.GetMemberRankName((string)this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID]) %>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%" colspan="2">メールテンプレート</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lMailTemp" runat="server">
																<%:
																	(this.MailTemplate.Count != 0)
																		? this.MailTemplate[0][Constants.FIELD_MAILTEMPLATE_MAIL_NAME]
																		: "-"
																%>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" colspan="2">実行タイミング</td>
														<td class="detail_item_bg">
															<asp:Label ID="lScheduleString" runat="server">
																<%: GetScheduleString(this.MemberRankRuleSet) %>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%" colspan="2">有効フラグ</td>
														<td class="detail_item_bg" align="left">
															<b>
																<asp:Label ID="lValidFlg" runat="server">
																	<%:
																		ValueText.GetValueText(Constants.TABLE_MEMBERRANKRULE,
																			Constants.FIELD_MEMBERRANKRULE_VALID_FLG,
																			(string)this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_VALID_FLG])
																	%>
																</asp:Label>
															</b>
														</td>
													</tr>
												</table>
												<div id="dvMemberRankRuleStatusInfo" runat="server">
													<br />
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="left" width="30%">ステータス</td>
															<td class="detail_item_bg" align="left">
																<asp:Label ID="lStatus" runat="server">
																	<%:
																		ValueText.GetValueText(Constants.TABLE_MEMBERRANKRULE,
																			Constants.FIELD_MEMBERRANKRULE_STATUS,
																			(string)this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_STATUS])
																	%>
																</asp:Label>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">最終付与人数</td>
															<td class="detail_item_bg" align="left">
																<asp:Label ID="lLastCount" runat="server">
																	<%:
																		string.IsNullOrEmpty(this.MemberRankRuleSet[Constants.FIELD_TASKSCHEDULE_PROGRESS].ToString())
																			? "-"
																			: this.MemberRankRuleSet[Constants.FIELD_TASKSCHEDULE_PROGRESS].ToString()
																	%>
																</asp:Label>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg">
																最終付与日時</td>
															<td class="detail_item_bg">
																<asp:Label ID="lLastExecDate" runat="server">
																	<%:
																		DateTimeUtility.ToStringForManager(
																			this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_LAST_EXEC_DATE],
																			DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
																			"-")
																	%>
																</asp:Label>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%" style="height: 28px">アクション</td>
															<td class="detail_item_bg" align="left" style="height: 28px">
																<input type= "button" onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_EXECUTE + "?" + Constants.REQUEST_KEY_MEMBERRANKRULE_ID + "=" + this.MemberRankRuleId) %>','contact','width=850,height=580,top=120,left=420,status=NO,scrollbars=yes');" value="アクションウィンドウを開く" />
															</td>
														</tr>
													</table>
													<script type="text/javascript">
													<!--
														// 別ウィンドウ表示
														function open_window(link_file, window_name, window_type) {
															var new_win = window.open(link_file, window_name, window_type);
															new_win.focus();
														}
													//-->
													</script>
												</div>
												<div class="action_part_bottom">
													<input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnEdit2" runat="server" Text="  編集する  " OnClick="btnEdit_Click"></asp:Button>
													<asp:Button id="btnCopyInsert2" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click"></asp:Button>
													<asp:Button id="btnDelete2" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？')"></asp:Button>
													<asp:Button id="btnInsert2" runat="server" Text="  登録する  " OnClick="btnInsert_Click"></asp:Button>
													<asp:Button id="btnUpdate2" runat="server" Text="  更新する  " OnClick="btnUpdate_Click"></asp:Button>
												</div>
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
