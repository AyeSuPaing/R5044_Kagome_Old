<%--
=========================================================================================================
  Module      : メール配信設定確認ページ(MailDistSettingConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailDistSettingConfirm.aspx.cs" Inherits="Form_MailDistSetting_MailDistSettingConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">メール配信設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<tr>
		<% if (m_strActionStatus == Constants.ACTION_STATUS_DETAIL) {%>
		<td><h2 class="cmn-hed-h2">メール配信設定詳細</h2></td>
		<%} %>
		<% if (m_strActionStatus == Constants.ACTION_STATUS_CONFIRM) {%>
		<td><h2 class="cmn-hed-h2">メール配信設定確認</h2></td>
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
													<asp:Button id="btnDelete" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除するとレポートで名称が取得できないなどの不整合が発生する可能性があります。\n削除してもよろしいですか？')"></asp:Button>
													<asp:Button id="btnInsert" runat="server" Text="  登録する  " OnClick="btnInsert_Click"></asp:Button>
													<asp:Button id="btnUpdate" runat="server" Text="  更新する  " OnClick="btnUpdate_Click"></asp:Button>
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trId" runat="server" visible="false">
														<td class="detail_title_bg" align="left" width="30%">メール配信設定ID</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lMailDistId" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">メール配信設定名</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lMailDistName" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">メール文章</td>
														<td class="detail_item_bg">
															<asp:Label ID="lMailText" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">ターゲット</td>
														<td class="detail_item_bg" align="left">
															<asp:Repeater ID="rTargetLists" runat="server">
																<ItemTemplate>
																	・<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)["target_name"]) %>
																	<%# WebSanitizer.HtmlEncode(((string)((Hashtable)Container.DataItem)["target_extract_flg"] == Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON) ? "[配信時抽出]" : "") %>
																	<br />
																</ItemTemplate>
															</asp:Repeater>
														</td> 
													</tr>
													<tr>
														<td class="detail_title_bg">メール配信エラー除外設定</td>
														<td class="detail_item_bg">
															<asp:Label ID="lExceptErrorPoint" runat="server">
																<%: StringUtility.ToNumeric(this.MailDistSet[Constants.FIELD_MAILDISTSETTING_EXCEPT_ERROR_POINT]) %>
															</asp:Label>pt 以上は除外
														</td>
													</tr>
													<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
													<tr>
														<td class="detail_title_bg">モバイルメール除外設定</td>
														<td class="detail_item_bg">
															<asp:Label ID="lExceptMobileMail" runat="server">
																<%: ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG, (string)this.MailDistSet[Constants.FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG]) %>
															</asp:Label>
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="detail_title_bg">重複配信除外設定</td>
														<td class="detail_item_bg">
															<asp:Label ID="lEnableDeduplication" runat="server">
																<%:
																	ValueText.GetValueText(
																		Constants.TABLE_MAILDISTSETTING,
																		Constants.FIELD_MAILDISTSETTING_ENABLE_DEDUPLICATION,
																		(string)this.MailDistSet[Constants.FIELD_MAILDISTSETTING_ENABLE_DEDUPLICATION])
																%>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">排除リスト</td>
														<td class="detail_item_bg">
															<asp:Label ID="lExceptList" runat="server">
																<%=
																	WebSanitizer.HtmlEncodeChangeToBr(
																		(((this.ExceptLists.Length != 1) || (string.IsNullOrEmpty(this.ExceptLists[0]) == false))
																			? string.Concat(this.ExceptLists
																				.Take(this.DispAddressCount)
																				.Select(data => data + Environment.NewLine))
																			+ ((this.ExceptLists.Length > this.DispAddressCount)
																				? "・・・" + Environment.NewLine
																				: string.Empty)
																			: string.Empty)
																			+ string.Format("(排除アドレス数：{0}件)",
																				(this.ExceptLists[0].Length != 0) ? this.ExceptLists.Length : 0))
																%>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">実行タイミング</td>
														<td class="detail_item_bg">
															<asp:Label ID="lScheduleString" runat="server">
																<%: GetScheduleString(this.MailDistSet) %>
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">有効フラグ</td>
														<td class="detail_item_bg" align="left">
															<b>
																<asp:Label ID="lValidFlg" runat="server">
																	<%:
																		ValueText.GetValueText(
																			Constants.TABLE_MAILDISTSETTING,
																			Constants.FIELD_MAILDISTSETTING_VALID_FLG,
																			(string)this.MailDistSet[Constants.FIELD_MAILDISTSETTING_VALID_FLG])
																	%>
																</asp:Label>
															</b>
														</td>
													</tr>
												</table>
												<div id="dvMailDistStatusInfo" runat="server">
													<br />
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="left" width="30%">ステータス</td>
															<td class="detail_item_bg" align="left">
																<asp:Label ID="lStatus" runat="server"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">最終配信件数</td>
															<td class="detail_item_bg" align="left">
																<asp:Label ID="lLastCount" runat="server"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">最終エラーポイント除外件数</td>
															<td class="detail_item_bg" align="left">
																<asp:Label ID="lLastErrorExceptCount" runat="server"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">最終重複配信除外件数</td>
															<td class="detail_item_bg" align="left">
																<asp:Literal ID="lLastDuplicateExceptCount" runat="server" />
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg">
																最終配信開始日時</td>
															<td class="detail_item_bg">
																<asp:Label ID="lLastDistDate" runat="server"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%" style="height: 28px">アクション</td>
															<td class="detail_item_bg" align="left" style="height: 28px">
																<span onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_EXECUTE + "?" + Constants.REQUEST_KEY_MAILDIST_ID + "=" + this.MailDistId) %>','contact','width=850,height=660,top=120,left=420,status=NO,scrollbars=yes');">
																	<input type= "button" value="アクションウィンドウを開く" />
																</span>
															</td>
														</tr>
													</table>
													<script type="text/javascript">
														<!--
														// 別ウィンドウ表示
														function open_window(link_file, window_name, window_type){
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
													<asp:Button id="btnDelete2" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除するとレポートで名称が取得できないなどの不整合が発生する可能性があります。\n削除してもよろしいですか？')"></asp:Button>
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