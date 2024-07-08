<%--
=========================================================================================================
  Module      : 定期台帳確認ページ(FixedPurchaseConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.User.UpdateAddressOfUserandFixedPurchase" %>
<%@ Import Namespace="w2.Domain.FixedPurchase.Helper" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="FixedPurchaseConfirm.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseConfirm" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="FieldMemoSetting" Src="~/Form/Common/FieldMemoSetting/BodyFieldMemoSetting.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<asp:Button ID="btnTooltipInfo" runat="server" style="display:none;"/>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr id="trTitleFixedpurchaseTop" runat="server">
	</tr>
	<tr id="trTitleFixedpurchaseMiddle" runat="server">
		<td><h1 class="page-title">定期台帳</h1></td>
	</tr>
	<tr id="trTitleFixedpurchaseBottom" runat="server">
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">定期台帳詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">定期台帳確認</h2></td>
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
												<div id="divComp" runat="server" class="action_part_top" Visible="False">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<% if ((this.UpdatedTargetsForDisplay == null) || (this.UpdatedTargetsForDisplay.Count() == 0)) { %>
														<tr class="info_item_bg">
															<td align="left">
																<asp:Literal id="lCompMessages" runat="server" Text="定期台帳を更新しました。" />
																<span style="color:red">
																	<asp:Literal ID="lPaymentContinuousAlert" runat="server" />
																</span>
																
															</td>
														</tr>
														<% } %>
														<% else { %>
														<tr class="info_item_bg">
														<td align="left">
															定期台帳を更新しました。<br />
															次のデータも更新しました。<br />
															<% var i = 0; %>
															<% foreach (var updatedTarget in this.UpdatedTargetsForDisplay) { %>
																<a href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(updatedTarget.CreateUrl()) %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																	<%: updatedTarget.Id %>
																</a>
																<% if (updatedTarget.UpdatedKbn == UpdatedKbn.User) { %>
																	<br />
																<% } else { %>
																	<% if ((i + 1) < this.UpdatedTargetsForDisplay.Count()) { %>
																		&thinsp;,&ensp;
																	<% } %>
																<% } %>
																<% i++; %>
															<% } %>
														</td>
														<% } %>
													</table>
												</div>
												<div id="divOrderComp" runat="server" class="action_part_top" Visible="False">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">
																<asp:Literal id="lOrderCompMessages" runat="server" Text="定期注文の登録を行いました。定期購入履歴一覧から注文情報の確認を行ってください。" />
															</td>
														</tr>
													</table>
												</div>
												<div class="dvPointResetMessages" runat="server" Visible="<%# string.IsNullOrEmpty(this.PointResetMessages) == false %>">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">
																<asp:Literal runat="server" ID="lPointResetMessages" />
															</td>
														</tr>
													</table>
												</div>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<%-- 仮クレジットカード登録待ち向け表示 --%>
										<tbody id="tbdyCreditCardRegisterForUnregisterd" Visible="False" runat="server">
										<tr>
										<td style="text-align:center;">
											<center>
											<table class="detail_table" border="0" cellspacing="1" cellpadding="3" style="min-width: 300px;">
											<tr>
											<td align="left" class="detail_item_bg" style="text-align:left;font-size:11pt">
												<div style="text-align:center">
												<span style="color: red">
												クレジットカード登録は保留状態です。<br/>
												下記の手順で登録・与信を行ってください。<br/>
												（カード登録ごとに
												<%if (OrderCommon.IsPaymentCardTypeGmo) {%>GMO会員<%} %>
												<%if (OrderCommon.IsPaymentCardTypeYamatoKwc) {%>ヤマト会員<%} %>
												を登録する必要があります。）<br/>
												</span>
												</div>
												<br/>
												<%if (OrderCommon.IsPaymentCardTypeGmo) {%>
												<br/>
												①決済用タブレットで「会員・カード登録」を開き、<br/>
													 クレジットカード番号と下記IDにてカードの登録を行ってください。<br />
												<br />
												<span style="font-size: large; line-height: 18px;text-align:center;">
													 GMO会員ID：<asp:Literal id="lGmoMemberId" runat="server"></asp:Literal><br/>
												</span>
												<br/>
													 ※GMO会員IDはハイフンなしで入力してください。<br/>
												<%} %>
												<%if (OrderCommon.IsPaymentCardTypeYamatoKwc) {%>
												<div style="text-align:left;">
												①決済用タブレットにて下記情報とともに1円与信を実行してください。<br/>
												<br />
												<span style="font-size: large; line-height: 18px;text-align:center;">
												 受付番号　　：<asp:Literal id="lYamatoKwcOrderNo" runat="server"></asp:Literal><br/>
												 ヤマト会員ID：<asp:Literal id="lYamatoKwcMemberId" runat="server"></asp:Literal><br/>
												 認証キー　　：<asp:Literal id="lYamatoKwcAuthenticationKey" runat="server"></asp:Literal><br/>
												 金額　　　　：1 円<br/>
												</span>
												<br/>
												 ※受付番号・ヤマト会員IDはハイフンなしで入力してください。<br/>
												 ※１円与信はヤマト会員登録のために利用します。<br/>
												  ヤマト会員登録後、この1円与信は取り消しされます。<br/>
												<br/>
												<%} %>
												<%if (OrderCommon.IsPaymentCardTypeEScott) {%>
												①決済用タブレットにて、クレジットカード情報と下記情報で会員登録を実行してください。<br/>
												<br />
												<span style="font-size: large; line-height: 18px;text-align:center;">
												　会員ID　　　　：<asp:Literal id="lEScottKaiinId" runat="server"></asp:Literal><br/>
												</span>
												<br/>
												※会員IDはハイフンなしで入力してください。

												<%} %>
												<br/>
												②登録が完了したらリロードを行い、正常に登録されていることを確認してください。<br/>
												<br/>
												<div style="text-align:center">
												<asp:Button id="btnReloadForRegisterdCreditCard" Text="    リロード    " OnClick="btnReloadForRegisterdCreditCard_Click" runat="server"/>
												</div>
											</td>
											</tr>
											</table>
											</center>
										</td>
										</tr>
										</tbody>
										<tr>
											<td>
												<div class="action_part_top">
													<span id="spanUpdateHistoryConfirmTop" runat="server">( <a href="javascript:open_window('<%= UpdateHistoryPage.CreateUpdateHistoryConfirmUrl(Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_FIXEDPURCHASE, this.FixedPurchaseContainer.UserId, this.FixedPurchaseContainer.FixedPurchaseId) %>','updatehistory','width=980,height=850,top=5,left=600,status=NO,scrollbars=yes,resizable=yes')">履歴参照</a> )</span>
													<asp:Button id="btnBackTop" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button ID="btnToListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" />
												</div>
												<div id="divAnchor" visible="false" runat="server" style="margin: 5px 0;">
													<a href="#anchorFixedPurchaseHistory">&nbsp;▼定期購入履歴一覧&nbsp;</a>&nbsp;
												</div>
												<%--▽ 基本情報 ▽--%>
												<div id="divFixedPurchase" runat="server">
													<table width="758" border="0" cellspacing="0" cellpadding="0">
														<tr valign="top">
															<td>
																<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<%--▽ 基本情報エラー表示 ▽--%>
																<tr id="trErrorMessagesTitle" runat="server" visible="false">
																	<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
																</tr>
																<tr id="trErrorMessages" runat="server" visible="false">
																	<td class="edit_item_bg" align="left" colspan="2">
																		<asp:Label ID="lbErrorMessages" runat="server" ForeColor="red" />
																	</td>
																</tr>
																<%--△ 基本情報エラー表示 △--%>
																<tr>
																	<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left" width="160">定期購入ID</td>
																	<td class="detail_item_bg" align="left">
																		<% btnFixedPurchaseOrder.OnClientClick = "return confirm('"
																			+ "今すぐ注文を登録します。よろしいですか？\\n\\n"
																			+ "■注意点\\n"
																			+ "・注文情報の配送日は「"
																			+ ReplaceTag("@@DispText.shipping_date_list.none@@")
																			+ "」で登録されます。\\n"
																			+ "・設定された利用ポイント数が本注文に適用されます。\\n"
																			+ "  適用後、定期購入の利用ポイント数が「0」にリセットされます。\\n"
																			+ (Constants.W2MP_COUPON_OPTION_ENABLED ? "・設定された利用クーポンが本注文に適用されます。\\n  適用後、定期購入の利用クーポンが「利用なし」にリセットされます。\\n" : "")
																			+ "・" + (Constants.SEND_FIXEDPURCHASE_MAIL_TO_USER ? "ユーザーに注文メールが送信されます。\\n" : "ユーザーに注文メールが送信されません。\\n　注文メールを送信する際は注文情報詳細画面から行ってください。\\n")
																			+ "・スキップ回数はクリアされます。"
																			+ "');"; %>
																		<% btnResumeFixedPurchase.OnClientClick = "return confirm('"
																			+ ((this.FixedPurchaseContainer.IsCancelTemporaryRegistrationStatus)
																				|| (this.FixedPurchaseContainer.IsTemporaryRegistrationStatus)
																				? ("定期購入を通常に変更してもよろしいでしょうか？\\n\\n")
																				: ("定期購入を再開してもよろしいでしょうか？\\n\\n"))
																			+ "■注意点\\n"
																			+ "・定期台帳の次回・次々回配送日は自動更新されません。\\n"
																			+ "  定期購入再開後に、手動で次回・次々回配送日を設定してください。"
																			+ ((this.FixedPurchaseContainer.PaymentStatus == Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_ERROR)
																				? ("\\n\\n・決済ステータスが「決済失敗」となっています。\\n"
																					+ "  定期購入再開後に、決済ステータスを「通常」へ更新してください。\\n"
																					+ "※決済ステータスが「決済失敗」の場合、定期注文処理から除外されます。")
																				: "")
																			+ ((OrderCommon.IsUsablePaymentContinuous(this.FixedPurchaseContainer.OrderPaymentKbn)
																				&& this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus)
																				? "\\n\\n・定期購入再開後に、支払方法の変更が必要です。"
																				: "")
																			+ "');"; %>
																		<asp:Literal ID="lFixedPurchaseId" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
																		<% if (this.FixedPurchaseContainer.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID) btnFixedPurchaseOrder.Enabled = false; %>
																		<asp:Button ID="btnFixedPurchaseOrder" runat="server" visible='<%# this.CanResumeFixedPurchase %>' Text="  今すぐ注文  " OnClick="btnFixedPurchaseOrder_Click" />
																		<asp:CheckBox ID="cbUpdateNextShippingDate" Text=" 次回配送日を更新する " runat="server" />
																		<asp:Button ID="btnResumeFixedPurchase" runat="server" visible='<%# this.CanResumeFixedPurchase %>' Text="  定期購入再開  " OnClick="btnResumeFixedPurchase_Click" />
																		<span runat="server" Visible="<%# this.IsInvalidResumePaypay %>">
																			<asp:Literal ID="lInvalidResumePaypay" runat="server"/>
																		</span>
																		<div runat="server" Visible="<%# this.IsCancelledByFixedPurchaseCombine %>">
																			※本定期台帳は定期台帳同梱により解約となった定期です。再開する場合は拡張ステータス36をOFFに変更してください。
																		</div>
																	</td>
																</tr>
																<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && this.FixedPurchaseContainer.IsSubsctriptionBox) { %>
																	<tr>
																		<td class="detail_title_bg" align="left">頒布会コースID</td>
																		<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" />
																		<td class="detail_item_bg" align="left"><a href="javascript:open_window('<%: CreateSubscriptionBoxRegisterUrl(hfSubscriptionBoxCourseId.Value) %>','fixedpurchase','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%: hfSubscriptionBoxCourseId.Value %></a>
																	</tr>
																<% } %>
																<tr>
																	<td class="detail_title_bg" align="left">定期購入設定</td>
																	<td class="detail_item_bg" align="left">
																		<asp:Literal ID="lFixedPurchaseSetting1" runat="server" />
																	</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">定期購入開始日時</td>
																	<td class="detail_item_bg" align="left"><asp:Literal id="lFixedPurchaseDate" runat="server" /></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">購入回数(注文基準)</td>
																	<td class="detail_item_bg" align="left">
																		<asp:UpdatePanel ID="upFixedPurchaseOrderCount" runat="server">
																		<ContentTemplate>
																		<p><asp:Literal ID="lFixedPurchaseOrderCount" runat="server"  />
																		<asp:TextBox ID="tbFixedPurchaseOrderCount" runat="server" Width="33" MaxLength="9" /> 回&nbsp;
																		<span id="spanFixedPurchasetOrderCounChange" runat="server" visible="false" class="action_link"><asp:LinkButton ID="lbFixedPurchaseOrderCountChange" runat="server" OnClick="lbFixedPurchaseOrderCountChange_Click">変更</asp:LinkButton></span>
																		<span id="spanFixedPurchaseOrderCountUpdateCancel" runat="server" visible="false" class="action_link">&nbsp;<asp:LinkButton ID="lbFixedPurchaseOrderCountUpdate" runat="server" OnClick="lbFixedPurchaseOrderCountUpdate_Click">更新</asp:LinkButton>&nbsp;&nbsp;/&nbsp;<asp:LinkButton ID="lbFixedPurchaseOrderCountCancel" runat="server" OnClick="lbFixedPurchaseOrderCountCancel_Click">キャンセル</asp:LinkButton></span>
																		</p>
																		<p id="pFixedPurchaseOrderCountError" runat="server" class="notice"></p>
																		</ContentTemplate>
																		</asp:UpdatePanel>
																	</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">購入回数(出荷基準)</td>
																	<td class="detail_item_bg" align="left">
																		<asp:UpdatePanel ID="upFixedPurchaseShippedCount" runat="server">
																		<ContentTemplate>
																		<p><asp:Literal ID="lFixedPurchaseShippedCount" runat="server"  />
																		<asp:TextBox ID="tbFixedPurchaseShippedCount" runat="server" Width="33" MaxLength="9"/> 回&nbsp;
																		<span id="spanFixedPurchaseShippedCountChange" runat="server" visible="false" class="action_link"><asp:LinkButton ID="lbFixedPurchaseShippedCountChange" runat="server" OnClick="lbFixedPurchaseShippedCountChange_Click">変更</asp:LinkButton></span>
																		<span id="spanFixedPurchaseShippedCountUpdateCancel" runat="server" visible="false" class="action_link"><asp:LinkButton ID="lbFixedPurchaseShippedCountUpdate" runat="server" OnClick="lbFixedPurchaseShippedCountUpdate_Click">更新</asp:LinkButton>&nbsp;&nbsp;/&nbsp;&nbsp;<asp:LinkButton ID="lbFixedPurchaseShippedCountCancel" runat="server" OnClick="lbFixedPurchaseShippedCountCancel_Click">キャンセル</asp:LinkButton></span>
																		</p>
																		<p id="pFixedPurchaseShippedCountError" runat="server" class="notice"></p>
																		</ContentTemplate>
																		</asp:UpdatePanel></td>
																</tr>
																<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && this.FixedPurchaseContainer.IsSubsctriptionBox) { %>
																	<tr>
																		<td class="detail_title_bg" align="left">頒布会注文回数</td>
																		<td class="detail_item_bg" align="left"><asp:Literal ID="lSubscriptionBoxCourseCount" runat="server" />回</td>
																	</tr>
																<% } %>
																	<tr>
																		<td class="detail_title_bg" align="left">スキップ回数</td>
																		<td class="detail_item_bg" align="left">
																			<asp:Literal ID="lFixedPurchaseSkippedCount" runat="server" />回
																			<asp:Button ID="btnClearSkippedCount" runat="server" Visible="<%# this.FixedPurchaseContainer.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL %>"
																				OnClick="btnClearSkippedCount_Click" Text="  クリア  "
																				OnClientClick="return confirm('スキップ回数をクリアします。よろしいですか？')" />
																		</td>
																	</tr>
																<tr>
																	<td class="detail_title_bg" align="left">最終購入日</td>
																	<td class="detail_item_bg" align="left"><asp:Literal ID="lFixedPurchaseLastOrderDate" runat="server" />
																	</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">注文区分</td>
																	<td class="detail_item_bg" align="left"><asp:Literal ID="lFixedPurchaseOrderKbn" runat="server" /></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">注文者ユーザーID</td>
																	<td class="detail_item_bg" align="left">
																		<asp:Literal ID="lOwnerUserId" runat="server" />
																	</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">注文者名</td>
																	<td class="detail_item_bg" align="left">
																		<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
																			<a href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl()) %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><asp:Literal ID="lOwnerName1" runat="server" /></a>
																		<% } else { %>
																			<asp:Literal ID="lOwnerName2" runat="server" />
																		<% } %>
																	</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">定期購入ステータス</td>
																	<td class="detail_item_bg" align="left">
																		<span id="spFixedPurchaseStatus" runat="server">
																			<asp:Literal ID="lFixedPurchaseStatus" runat="server" />
																		</span></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">決済ステータス</td>
																	<td class="detail_item_bg" align="left">
																		<asp:RadioButtonList runat="server" ID="rblPaymenStatus" DataValueField="Value" DataTextField="Text" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
																		&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnPaymentStatus" runat="server" OnClick="btnPaymentStatus_Click" Text="  決済ステータス更新  " />
																	</td>
																</tr>
																<%-- ポイントOPオフ時に、中の値がポストされてエラーになるため非表示とする --%>
																<tr Visible=<%# Constants.W2MP_POINT_OPTION_ENABLED %> runat="server">
																	<td class="detail_title_bg" align="left">次回購入の利用ポイント</td>
																	<td class="detail_item_bg" align="left">
																		<asp:UpdatePanel ID="upNextShipingUsePoint" runat="server">
																		<ContentTemplate>
																			<% if ((this.FixedPurchaseContainer.UseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON) && this.CanUseAllPointFlg) { %>
																				<span>全ポイント継続利用</span>
																			<% } else { %>
																				<asp:Literal ID="lNextShippingUsePoint" runat="server" /><%: Constants.CONST_UNIT_POINT_PT %>&nbsp;
																			<% } %>
																			<asp:Button id="btnUpdateNextShippingUsePoint" runat="server" Text="  利用ポイント変更  " OnClick="btnUpdateNextShippingUsePoint_Click" />&nbsp;
																			（利用可能ポイント：<asp:Literal ID="lUserUsablePoint" runat="server" /><%: Constants.CONST_UNIT_POINT_PT %>）<br />
																			<div id="dvNextShippingUsePoint" visible="false" runat="server">
																				<asp:TextBox ID="tbNextShippingUsePoint" runat="server"></asp:TextBox>&nbsp;
																				<asp:LinkButton ID="lbNextShippingUsePointUpdate" runat="server" OnClientClick="return usePointUpdateClick();" OnClick="lbNextShippingUsePointUpdate_Click">更新</asp:LinkButton>&nbsp;&nbsp;/&nbsp;&nbsp;
																				<asp:LinkButton ID="lbNextShippingUsePointCancel" runat="server" OnClick="lbNextShippingUsePointCancel_Click">キャンセル</asp:LinkButton>
																				<% if (this.CanUseAllPointFlg) { %>
																				<br/>
																				<asp:CheckBox ID="cbUseAllPointFlg" Text="全ポイント継続利用 ※以降の注文にも適用されます"
																					OnCheckedChanged="cbUseAllPointFlg_Changed" OnDataBinding="cbUseAllPointFlg_DataBinding"
																					CssClass="checkBox" style="font-size: 12px;" AutoPostBack="True" runat="server" />
																				<% } %>
																				<p id="pNextShippingUsePointError" runat="server" class="notice"></p>
																			</div>
																			<small>
																			※入力した利用ポイントは次回購入時に適用されます。次回配送日の <%# this.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
																			 （一度入力済みの利用ポイントを減らす際、入力済みポイントが有効期限切れの場合には、ポイントが消滅します。）<br />
																			※次回注文生成時、ポイント数が注文時の利用可能ポイント数より大きい場合、適用されなかった分のポイントは戻されます。<br />
																			 （ポイント有効期限切れの場合は、ポイントは戻らず消滅します。）<br />
																			※定期購入に期間限定ポイントを利用することはできません。
																			</small>
																		</ContentTemplate>
																		</asp:UpdatePanel>
																	</td>
																</tr>
																<%-- クーポンOPオフ時に、中の値がポストされてエラーになるため非表示とする --%>
																<tr Visible="<%# Constants.W2MP_COUPON_OPTION_ENABLED %>" runat="server">
																	<td class="detail_title_bg">次回購入の利用クーポン</td>
																	<td class="detail_item_bg">
																		<asp:UpdatePanel ID="upNextShippingUseCoupon" runat="server">
																		<ContentTemplate>
																			<%: (this.FixedPurchaseContainer.NextShippingUseCouponDetail == null) ? "利用なし" : this.FixedPurchaseContainer.NextShippingUseCouponDetail.DisplayName %>
																			<asp:Button id="btnUpdateNextShippingUseCoupon" runat="server" Text="  利用クーポン変更  " OnClick="btnUpdateNextShippingUseCoupon_Click" />&nbsp;
																			<br />
																			<div id="dvNextShippingUseCoupon" visible="false" runat="server">
																				クーポンコード
																				<asp:TextBox ID="tbCouponCode" runat="server" />&nbsp;
																				<asp:LinkButton runat="server" Text="更新" OnClientClick="return confirm('ご利用クーポンを変更します。よろしいですか？');" OnClick="btnUpdateUseCoupon_Click" />&nbsp;&nbsp;/&nbsp;&nbsp;
																				<asp:LinkButton runat="server" Text ="キャンセル" OnClick="lbNextShippingUseCouponCancel_Click" />
																				<br />
																				（<a id="user_coupon_list" href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(CreateUserCouponListUrl("UserCoupons")) %>','coupon_list','width=1000,height=610,top=120,left=320,status=NO,scrollbars=yes');">
																					ユーザー所持クーポンを選択：<%: StringUtility.ToNumeric(GetUsableCouponCount(this.LoginOperatorDeptId, this.FixedPurchaseContainer.UserId, this.FixedPurchaseContainer.OwnerMailAddr, true)) %> 枚</a> ）
																				（<a id="usable_coupon_list" href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(CreateUserCouponListUrl("UsableCoupons")) %>','coupon_list','width=1000,height=610,top=120,left=320,status=NO,scrollbars=yes');">
																					利用可能クーポンを選択： <%: StringUtility.ToNumeric(GetUsableCouponCount(this.LoginOperatorDeptId, this.FixedPurchaseContainer.UserId, this.FixedPurchaseContainer.OwnerMailAddr, false)) %> 枚</a> ）
																					<p id="pNextShippingUseCouponError" runat="server" class="notice"></p>
																			</div>
																			<small>
																			※入力した利用クーポンは次回購入時に適用されます。次回配送日の <%# this.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
																			※次回注文生成時、クーポンが本注文に適用不可となる場合、クーポンが戻されます。
																			</small>
																		</ContentTemplate>
																		</asp:UpdatePanel>
																	</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">有効フラグ</td>
																	<td class="detail_item_bg" align="left"><asp:Literal ID="lFixedPurchaseValidFlg" runat="server" /></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">解約日</td>
																	<td class="detail_item_bg" align="left">
																		<asp:Literal ID="lCancelDate" runat="server" />
																	</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left">再開日</td>
																	<td class="detail_item_bg" align="left">
																		<asp:Literal ID="lRestartDate" runat="server" />
																	</td>
																</tr>
																<%--▽ 更新日 ▽--%>
																<tr id="trFixedPurchaseDate1" runat="server" visible="false">
																	<td class="detail_title_bg" align="left">作成日</td>
																	<td class="detail_item_bg" align="left"><asp:Literal ID="lFixedPurchaseDateCreated" runat="server" /></td>
																</tr>
																<tr id="trFixedPurchaseDate2" runat="server" visible="false">
																	<td class="detail_title_bg" align="left">更新日</td>
																	<td class="detail_item_bg" align="left"><asp:Literal ID="lFixedPurchaseDateChanged" runat="server" /></td>
																</tr>
																<tr id="trFixedPurchaseDate3" runat="server" visible="false">
																	<td class="detail_title_bg" align="left">最終更新者</td>
																	<td class="detail_item_bg" align="left"><asp:Literal ID="lFixedPurchaseLastChanged" runat="server" /></td>
																</tr>
															</table>
															</td>
														</tr>
													</table>
													<br />
												</div>
												<%--△ 基本情報 △--%>
												<%--▽ メール送信 ▽--%>
												<div id="divFixedPurchaseSendMail" runat="server">
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="3">メール送信</td>
														</tr>
														<tr>
															<td class="detail_title_bg" width="120">メールテンプレート
															</td>
															<td class="detail_item_bg" align="left" width="445">
																<asp:DropDownList id="ddlFixedPurchaseMailId" runat="server" Width="400"></asp:DropDownList>
															</td>
															<td class="detail_item_bg" align="center" width="153">
																<input id="btnSendFixedPurchaseMail" runat="server" type="button" style="width:153px" value="  メール送信フォームへ  " onclick="javascript: open_fixed_purchase_send_mail();"/>
															</td>
														</tr>
													</table>
												</div>
												<%--△ メール送信 △--%>
												<br />
												<%--▽ 決済情報 ▽--%>
												<span id="dvAlertErrorMessages" visible="false" runat="server" style="margin: 5px 0;">
													<asp:Label ID="lbAlertErrorMessages" runat="server" ForeColor="red" />
												</span>
												<div id="divOrderPayment" runat="server">
													<div style="text-align:right; margin:0px 0px 5px 0px;">
														<asp:Button id="btnOrderPaymentKbnEdit" runat="server" Text="  編集する  " Enabled="<%# (this.IsItemsExist && this.IsNotPaymentAtConvenienceStoreEcPay) %>" OnClick="btnOrderPaymentKbnEdit_Click" />
													</div>
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">決済情報</td>
														</tr>
														<%-- エラーメッセージ --%>
														<tr id="trModifyCardErrorMessages" visible="false" runat="server">
															<td class="edit_item_bg" align="left" colspan="2">
																<asp:Label ID="lbModifyCardErrorMessages" runat="server" ForeColor="red" />
															</td>
														</tr>
														<%-- 完了メッセージ --%>
														<tr id="trModifyCardFinishMessages" visible="false" runat="server">
															<td class="edit_item_bg" align="left" colspan="2">
																<asp:Label ID="lbModifyCardFinishMessages" runat="server" />
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="160">支払方法</td>
															<td class="detail_item_bg" align="left"><asp:Literal ID="lFixedPurchaseOrderPaymentKbn" runat="server" />
															</td>
														</tr>
														<tr id="trUserCreditCardInfo" runat="server" visible="false">
															<td class="detail_title_bg" align="left">クレジットカード情報</td>
															<td class="detail_item_bg" align="left"><asp:Literal ID="lUserCreditCardInfo" runat="server"></asp:Literal>
															</td>
														</tr>
														<tr id="trUserSmsInfo" runat="server" visible="false">
															<td class="detail_title_bg" align="left">SMS認証済み携帯電話番号</td>
															<td class="detail_item_bg" align="left"><asp:Literal ID="lUserSmsInfo" runat="server"></asp:Literal>
															</td>
														</tr>
													</table>
													<%= (this.ActionStatus == Constants.ACTION_STATUS_DETAIL || this.ActionStatus == Constants.ACTION_STATUS_COMPLETE) ? "<br />" : ""%>
												</div>
												<%--△ 決済情報 △--%>
												<%--▽ 拡張ステータス ▽--%>
												<div id="divFixedPurchaseExtendStatus" runat="server">
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<asp:Repeater ID="rExtendStatus" runat="server" OnItemCommand="rExtendStatus_ItemCommand">
															<HeaderTemplate>
																<tr>
																	<td class="detail_title_bg" align="center" colspan="6">拡張ステータス更新</td>
																</tr>
															</HeaderTemplate>
															<ItemTemplate>
																<tr>
																	<td class="detail_title_bg">
																		拡張ステータス<asp:Literal ID="lExtendNo" runat="server" />：&nbsp;<asp:Literal ID="lExtendName" runat="server" />
																	</td>
																	<td class="detail_item_bg" width="250">
																		<asp:RadioButtonList id="rblExtend" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio_button_list"></asp:RadioButtonList>
																	</td>
																	<td class="detail_title_bg" width="80">
																		更新日指定
																	</td>
																	<td class="detail_item_bg" align="right" width="400">
																		&nbsp;
																		<uc:DateTimePickerPeriodInput id="ucExtendStatusDate" CanShowEndDatePicker="False" runat="server" />
																	</td>
																	<td class="detail_item_bg" align="right" width="120">
																		<asp:Button ID="btnUpdateExtendStatus" Runat="server" CommandName="Update" Text='  <%# "ステータス" + ((ExtendStatus)Container.DataItem).No + "更新" %>  ' Width="120" />
																	</td>
																	<td class="detail_item_bg" style="text-align: center" width="200">
																		<asp:Literal ID="lExtendStatusUpdateDate" runat="server" />
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</table>
													<br/>
												</div>
												<%--△ 拡張ステータス △--%>
												<%--▽ 配送先情報 ▽--%>
												<div id="dvFixedPurchaseShippingAlert" runat="server" visible="false" style="margin: 5px 0;">
													<asp:Label ID="lbFixedPurchaseShippingAlertMessage" runat="server" ForeColor="Red" />
												</div>
												<div id="dvFixedPurchaseShipping" runat="server">
												<div style="text-align:right; margin:0px 0px 5px 0px;">
													<asp:Button id="btnEdit" runat="server" Text="  編集する  " OnClick="btnEdit_Click" Enabled="<%# (this.IsItemsExist) %>" />
												</div>
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">配送先情報</td>
														</tr>
														<tbody id="tbodyUserShipping" runat="server">
															<tr>
																<td class="detail_title_bg" align="left" width="160"><%: ReplaceTag("@@User.name.name@@") %></td>
																<td class="detail_item_bg" align="left" >
																	<asp:Literal ID="lShippingName" runat="server"></asp:Literal>
																	<% if (this.IsShippingAddrJp) { %>
																	（<asp:Literal ID="lShippingNameKana" runat="server" />）
																	<% } %>
																</td>
															</tr>
															<tr>
																<td class="detail_title_bg" align="left">住所</td>
																<td class="detail_item_bg" align="left">
																	<asp:Literal ID="lShippingZip" runat="server" />
																	<asp:Literal ID="lShippingAddr1" runat="server" />&nbsp;
																	<asp:Literal ID="lShippingAddr2" runat="server" />&nbsp;
																	<asp:Literal ID="lShippingAddr3" runat="server" />&nbsp;
																	<asp:Literal ID="lShippingAddr4" runat="server" />&nbsp;
																	<asp:Literal ID="lShippingAddr5" runat="server" />&nbsp;
																	<asp:Literal ID="lShippingZipGlobal" runat="server" />
																	<asp:Literal ID="lShippingCountryName" runat="server" />
																</td>
															</tr>
															<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
															<tr>
																<td class="detail_title_bg" align="left">企業名・部署名</td>
																<td class="detail_item_bg" align="left">
																	<asp:Literal ID="lShippingCompanyName" runat="server" />&nbsp
																	<asp:Literal ID="lShippingCompanyPostName" runat="server" /></td>
															</tr>
															<%} %>
															<tr>
																<td class="detail_title_bg" align="left">電話番号</td>
																<td class="detail_item_bg" align="left"><asp:Literal ID="lShippingTel1" runat="server" /></td>
															</tr>
														</tbody>
														<tbody id="tbodyOrderConvenienceStore" runat="server">
															<tr>
																<td class="detail_title_bg" align="left" width="160">店舗ID</td>
																<td class="detail_item_bg" colspan="3" align="left">
																	<asp:Literal ID="lShippingReceivingStoreId" runat="server" />
																</td>
															</tr>
															<tr>
																<td class="detail_title_bg" align="left" width="160">店舗名称</td>
																<td class="detail_item_bg" colspan="3" align="left">
																	<asp:Literal ID="lShippingNameConvenienceStore" runat="server" />
																</td>
															</tr>
															<tr>
																<td class="detail_title_bg" align="left" width="160">店舗住所</td>
																<td class="detail_item_bg" colspan="3" align="left">
																	<asp:Literal ID="lShippingAddr4ConvenienceStore" runat="server" />
																</td>
															</tr>
															<tr>
																<td class="detail_title_bg" align="left" width="160">店舗電話番号</td>
																<td class="detail_item_bg" colspan="3" align="left">
																	<asp:Literal ID="lShippingTel1ConvenienceStore" runat="server" />
																</td>
															</tr>
														</tbody>
														<tr>
															<td class="detail_title_bg" align="left">配送方法</td>
															<td class="detail_item_bg" align="left"><asp:Literal ID="lShippingMethod" runat="server" /></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">配送サービス</td>
															<td class="detail_item_bg" align="left"><asp:Literal ID="lShippingCompany" runat="server" /></td>
														</tr>
														<% if (this.DeliveryCompany.IsValidShippingTimeSetFlg) { %>
														<tr>
															<td class="detail_title_bg" align="left">配送希望時間帯</td>
															<td class="detail_item_bg" align="left">
																<asp:Literal ID="lShippingTime" runat="server" /></td>
														</tr>
														<%} %>
													</table>
													<% if (this.CanDisplayAddressUpdatePattern) { %>
													<br/>
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">配送情報反映対象</td>
														</tr>
														<% if (this.DoUpdateUser) { %>
														<tr>
															<td class="detail_title_bg" align="left">ユーザー情報</td>
															<td class="detail_item_bg" align="left">
															<% if (this.HasUpdateUser) { %>
																<div id ="dvUserForAddressUpdate" runat="server">
																	<table class="detail_table" cellspacing="0" cellpadding="0">
																	<tr>
																		<td class="detail_title_bg" align="center" width="100px"></td>
																		<td class="detail_title_bg" align="center" width="300px">ユーザーID</td>
																		<td class="detail_title_bg" align="center" width="300px">顧客区分</td>
																	</tr>
																	<asp:Repeater id="rUserListForAddressUpdate" ItemType="w2.Domain.User.UserModel" runat="server">
																	<ItemTemplate>
																		<tr>
																			<td class="detail_item_bg" align="center" width="10"><asp:CheckBox id="cbUpdateExceptUser" runat="server" Checked="True"/></td>
																			<td class="detail_item_bg" align="center" width="30">
																				<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
																					<a href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl()) %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																						<%#: Item.UserId %>
																					</a>
																				<% } else { %>
																					<%#: Item.UserId %>
																				<% } %>
																			</td>
																			<td class="detail_item_bg" align="center" width="30"><%#: ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, Item.UserKbn) %></td>
																			<asp:HiddenField ID="hfUserIdForAddressUpdate" runat="server" Value="<%#: Item.UserId %>" />
																		</tr>
																	</ItemTemplate>
																	</asp:Repeater>
																	</table>
																	<br/>
																</div>
															<% } else { %>
																同じ住所情報を持つデータがありません。
															<% } %>
															</td>
														</tr>
														<% } %>
														<% if (this.DoUpdateOtherFixedPurchaseInfo) { %>
														<tr>
															<td class="detail_title_bg" align="left">その他定期情報</td>
															<td class="detail_item_bg" align="left">
															<% if (this.HasUpdateFixedPurchase) { %>
																<div id ="dvFixedPurchaseForAddressUpdate" runat="server">
																	<table class="detail_table">
																	<tr>
																		<td class="detail_title_bg" align="center" width="100px"></td>
																		<td class="detail_title_bg" align="center" width="300px">定期購入ID</td>
																		<td class="detail_title_bg" align="center" width="300px">定期購入ステータス</td>
																	</tr>
																	<asp:Repeater id="rFixedPurchaseListForAddressUpdate" ItemType="w2.Domain.FixedPurchase.FixedPurchaseModel" Runat="server">
																	<ItemTemplate>
																		<tr>
																			<td class="detail_item_bg" align="center"><asp:CheckBox id="cbUpdateExceptFixedPurchase" runat="server" Checked="True"/></td>
																			<td class="detail_item_bg" align="center">
																				<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM)) { %>
																					<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(FixedPurchasePage.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId, true)) %>','ordercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																						<%#: Item.FixedPurchaseId %>
																					</a>
																				<% } else { %>
																					<%#: Item.FixedPurchaseId %>
																				<% } %>
																			</td>
																			<td class="detail_item_bg" align="center"><%#: ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, Item.FixedPurchaseStatus) %></td>
																			<asp:HiddenField ID="hfFixedPurchaseIdForAddressUpdate" runat="server" Value="<%#: Item.FixedPurchaseId %>" />
																		</tr>
																	</ItemTemplate>
																	</asp:Repeater>
																	</table>
																	<br/>
																</div>
															<% } else { %>
																同じ住所情報を持つデータがありません。
															<% } %>
															</td>
														</tr>
														<% } %>
													</table>
													<%} %>
													<%= (this.ActionStatus == Constants.ACTION_STATUS_DETAIL || this.ActionStatus == Constants.ACTION_STATUS_COMPLETE) ? "<br />" : ""%>
												</div>
												<%--△ 配送先情報 △--%>
												<%--▽ 配送パターン ▽--%>
												<div id="dvFixedPurchasePattern" runat="server">
												<div style="text-align:right; margin:0px 0px 5px 0px;"><asp:Button id="btnPatternEdit" runat="server" Text="  編集する  " OnClick="btnPatternEdit_Click" Enabled="<%# (this.IsItemsExist) %>" /></div>
													<table class="detail_table" cellspacing="1" cellpadding="2" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">配送パターン</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="160">定期購入設定</td>
															<td class="detail_item_bg" align="left">
																<span id="spFixedPurchaseShippingPattern" runat="server">
																	<asp:Literal ID="lFixedPurchaseShippingPattern" runat="server" />
																</span></td>
														</tr>
														<% if (Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE) { %>
															<tr>
																<td class="detail_title_bg" align="left">次回出荷予定日</td>
																<td class="detail_item_bg" align="left">
																	<span id="spNextScheduledShippingDate" runat="server">
																		<asp:Literal ID="lNextScheduledShippingDate" runat="server" />
																	</span>
																</td>
															</tr>
														<% } %>
														<tr>
															<td class="detail_title_bg" align="left">次回配送日</td>
															<td class="detail_item_bg" align="left">
																<span id="spNextShippingDate" runat="server">
																	<asp:Literal ID="lNextShippingDate" runat="server" />
																</span>
															</td>
														</tr>
														<% if (Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE) { %>
															<tr>
																<td class="detail_title_bg" align="left">次々回出荷予定日</td>
																<td class="detail_item_bg" align="left">
																	<span id="spNextNextScheduledShippingDate" runat="server">
																		<asp:Literal ID="lNextNextScheduledShippingDate" runat="server" />
																	</span>
																</td>
															</tr>
														<% } %>
														<tr>
															<td class="detail_title_bg" align="left">次々回配送日</td>
															<td class="detail_item_bg" align="left">
																<span id="spNextNextShippingDate" runat="server">
																	<asp:Literal ID="lNextNextShippingDate" runat="server" />
																</span>
															</td>
														</tr>
														<% if (this.IsLeadTimeFlgOff == false) { %>
														<tr>
															<td class="detail_title_bg" align="left"><asp:Literal ID="lTotalLeadTimeName" Text="配送リードタイム" runat="server" /></td>
															<td class="detail_item_bg" align="left">
																<span id="spTotalLeadTime" runat="server">
																	<asp:Literal ID="lTotalLeadTime" runat="server" />
																</span>
															</td>
														</tr>
														<% } %>
													</table>
													<%= (this.ActionStatus == Constants.ACTION_STATUS_DETAIL || this.ActionStatus == Constants.ACTION_STATUS_COMPLETE) ? "<br />" : ""%>
												</div>
												<%--△ 配送パターン △--%>
												<%--▽ Invoice ▽--%>
												<% if (OrderCommon.DisplayTwInvoiceInfo(this.FixedPurchaseContainer.Shippings[0].ShippingCountryIsoCode)) { %>
												<div id="dvInvoice" runat="server">
												<div style="text-align:right; margin:0px 0px 5px 0px;"><asp:Button id="btnInvoiceEdit" runat="server" Text="  編集する  " OnClick="btnInvoiceEdit_Click"/></div>
													<table class="detail_table" cellspacing="1" cellpadding="2" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">電子発票</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="160">発票種類</td>
															<td class="detail_item_bg" align="left">
																<span id="spInvoiceUniform" runat="server">
																	<asp:Literal ID="lInvoiceUniform" runat="server" />
																</span></td>
														</tr>
														<% if (this.IsPersonal) { %>
														<tr>
															<td class="detail_title_bg" align="left" width="25%">共通性載具</td>
															<td class="detail_item_bg" align="left">
																<span id="spCarryType" runat="server">
																	<asp:Literal ID="lCarryTypeOption" runat="server" />
																</span>
															</td>
														</tr>
														<% } %>
														<% if (this.IsCompany) { %>
														<tr>
															<td class="detail_title_bg" align="left" width="25%">統一編号</td>
															<td class="detail_item_bg" align="left">
																<span id="spCompanyOption1" runat="server">
																	<asp:Literal ID="lCompanyOption1" runat="server" />
																</span>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="25%">会社名</td>
															<td class="detail_item_bg" align="left">
																<span id="spCompanyeOption2" runat="server">
																	<asp:Literal ID="lCompanyOption2" runat="server" />
																</span>
															</td>
														</tr>
														<% } %>
														<% if (this.IsDonate) { %>
														<tr>
															<td class="detail_title_bg" align="left" width="25%">寄付先コード</td>
															<td class="detail_item_bg" align="left">
																<span id="spDonateOption" runat="server">
																	<asp:Literal ID="lDonateOption" runat="server" />
																</span>
															</td>
														<% } %>
														</tr>
													</table>
													<%= (this.ActionStatus == Constants.ACTION_STATUS_DETAIL || this.ActionStatus == Constants.ACTION_STATUS_COMPLETE) ? "<br />" : ""%>
												</div>
												<% } %>
												<%--△ Invoice △--%>
												<%--▽ 次回配送スキップ ▽--%>
												<div id="dvFixedPurchaseSkip" runat="server">
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center">定期購入の次回配送スキップ</td>
														</tr>
														<tr>
															<td class="detail_item_bg" align="center">
																次回配送を中断する場合には、スキップボタンを押下してください。
																<span style="color:red" runat="server" Visible='<%# this.IsCancelable == false %>' ><br/>
																	この定期台帳の商品は、出荷回数が<%: this.FixedPurchaseCancelableCount %>回以上からスキップ可能と設定されているので、スキップができません。
																</span><br/>
																<asp:Button ID="btnFixedPurchaseSkip" runat="server" Text="   スキップ   " OnClick="btnFixedPurchaseSkip_Click" Enabled="<%# (this.IsItemsExist) %>" OnClientClick="return confirm('次回配送をスキップします。よろしいですか？\n\n（※既に自動で登録された注文はキャンセルされません。）');" />
															</td>
														</tr>
													</table>
													<br />
												</div>
												<%--△ 次回配送スキップ △--%>
												<%--▽ 商品情報 ▽--%>
												<div id="dvFixedPurchaseItemList" runat="server">
													<div style="text-align:right; margin:0px 0px 5px 0px;"><asp:Button id="btnItemEdit" runat="server" Text="  編集する  " OnClick="btnItemEdit_Click" /></div>
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<%--▽ 商品変更エラー表示 ▽--%>
														<tr id="trFixedPurchaseItemErrorMessagesTitle" runat="server" visible="false">
															<td class="edit_title_bg" align="center" colspan="8">エラーメッセージ</td>
														</tr>
														<tr id="trFixedPurchaseItemErrorMessages" runat="server" visible="false">
															<td class="edit_item_bg" align="left" colspan="8">
																<asp:Label ID="lbFixedPurchaseItemOrderCountError" runat="server" text ="" Visible ="false" ForeColor="red" />
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="center" colspan="8">商品情報</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="center" width="15%" rowspan="2">
																商品ID
															</td>
															<td class="detail_title_bg" align="center" width="15%" rowspan="2">
																バリエーションID
															</td>
															<td class="detail_title_bg" align="center" width="30%" rowspan="2">
																商品名
															</td>
															<% if ( this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false ) { %>
															<td class="detail_title_bg" align="center" width="8%" rowspan="2">
																単価（<%: this.ProductPriceTextPrefix %>）
															</td>
															<% } %>
															<td class="detail_title_bg" align="center" width="8%" rowspan="2">
																数量
															</td>
															<td class="detail_title_bg" align="center" width="15%" colspan="2">
																購入回数
															</td>
															<% if ( this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false ) { %>
															<td class="detail_title_bg" align="center" width="8%" rowspan="2" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
																小計（<%: this.ProductPriceTextPrefix %>）
															</td>
															<% } %>
														</tr>
														<tr>
															<td class="detail_title_bg" align="center" width="7%">注文基準</td>
															<td class="detail_title_bg" align="center" width="7%">出荷基準</td>
														</tr>
														<asp:Repeater id="rItemList" runat="server">
															<ItemTemplate>
																<tr>
																	<td class="detail_item_bg" align="center">
																		<asp:Literal ID="lProductId" runat="server" text ="<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).ProductId)%>" />
																	</td>
																	<td class="detail_item_bg" align="center">
																		<asp:Literal ID="lVariationId" runat="server" text ='<%# WebSanitizer.HtmlEncode((((FixedPurchaseItemInput)Container.DataItem).ProductId == ((FixedPurchaseItemInput)Container.DataItem).VariationId) ? "-" : "商品ID + " + ((FixedPurchaseItemInput)Container.DataItem).VId)%>' />
																	</td>
																	<td class="detail_item_bg" align="left">
																		<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).CreateProductJointName())%>
																	</td>
																	<td class="detail_item_bg" align="right" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
																		<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).GetValidPrice().ToPriceString(true))%>
																	</td>
																	<td class="detail_item_bg" align="center">
																		<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).ItemQuantity)%>
																	</td>
																	<td class="detail_item_bg" align="center">
																		<p>
																			<asp:Literal ID="lFixedPurchaseItemOrderCount" runat="server" text ="<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).ItemOrderCount)%>" />
																			<asp:TextBox ID="tbFixedPurchaseItemOrderCount" runat="server" Visible="false" Width="33" MaxLength="3" text ="<%#: ((FixedPurchaseItemInput)Container.DataItem).ItemOrderCount %>" CommandArgument="<%# Container.ItemIndex %>"/> 回&nbsp;
																			<span id="spanFixedPurchaseItemOrderCounChange" runat="server" visible="<%# (this.RequestActionKbn != ACTION_KBN_ITEM) %>" class="action_link">
																				<asp:LinkButton ID="lbFixedPurchaseItemOrderCountChange" runat="server" OnClick="lbFixedPurchaseItemOrderCountChange_Click" CommandArgument="<%# Container.ItemIndex %>">変更</asp:LinkButton>
																			</span>
																			<br />
																			<span id="spanFixedPurchaseItemOrderCountUpdateCancel" visible ="false" runat="server" class="action_link">&nbsp;
																				<asp:LinkButton ID="lbFixedPurchaseItemOrderCountUpdate" runat="server" OnClick="lbFixedPurchaseItemOrderCountUpdate_Click" CommandArgument="<%# Container.ItemIndex %>">更新</asp:LinkButton>&nbsp;&nbsp;/&nbsp;
																				<asp:LinkButton ID="lbFixedPurchaseItemOrderCountCancel" runat="server" OnClick="lbFixedPurchaseItemOrderCountCancel_Click" CommandArgument="<%# Container.ItemIndex %>">キャンセル</asp:LinkButton>
																			</span>
																		</p>
																	</td>
																	<td class="detail_item_bg" align="center">
																		<p>
																			<asp:Literal ID="lFixedPurchaseItemShippedCount" runat="server" text ="<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).ItemShippedCount)%>" />
																			<asp:TextBox ID="tbFixedPurchaseItemShippedCount" runat="server" Visible="false" Width="33" MaxLength="3" text ="<%#: ((FixedPurchaseItemInput)Container.DataItem).ItemShippedCount %>" CommandArgument="<%# Container.ItemIndex %>"/> 回&nbsp;
																			<span id="spanFixedPurchaseItemShippedCounChange" runat="server" visible="<%# (this.RequestActionKbn != ACTION_KBN_ITEM) %>" class="action_link">
																				<asp:LinkButton ID="lbFixedPurchaseItemShippedCountChange" runat="server" OnClick="lbFixedPurchaseItemShippedCountChange_Click" CommandArgument="<%# Container.ItemIndex %>">変更</asp:LinkButton>
																			</span>
																			<br />
																			<span id="spanFixedPurchaseItemShippedCountUpdateCancel" visible ="false" runat="server" class="action_link">&nbsp;
																				<asp:LinkButton ID="LinkButton2" runat="server" OnClick="lbFixedPurchaseItemShippedCountUpdate_Click" CommandArgument="<%# Container.ItemIndex %>">更新</asp:LinkButton>&nbsp;&nbsp;/&nbsp;
																				<asp:LinkButton ID="lbFixedPurchaseItemShippedCountCancel" runat="server" OnClick="lbFixedPurchaseItemShippedCountCancel_Click" CommandArgument="<%# Container.ItemIndex %>">キャンセル</asp:LinkButton>
																			</span>
																		</p>
																	</td>
																	<td class="detail_item_bg" align="right" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
																		<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).GetItemPrice().ToPriceString(true))%>
																	</td>
																</tr>
																<tr visible='<%# ((FixedPurchaseItemInput)Container.DataItem).ProductOptionTexts != "" %>' runat="server">
																	<td class="detail_title_bg" align="center">付帯情報</td>
																	<td class="detail_item_bg" align="left" colspan="7">
																		<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).GetDisplayProductOptionTexts())%>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</table>
													<%= (this.ActionStatus == Constants.ACTION_STATUS_DETAIL || this.ActionStatus == Constants.ACTION_STATUS_COMPLETE) ? "<br />" : ""%>
												</div>
												<%--△ 商品情報 △--%>
												<!-- ▽領収書情報▽ -->
												<div id="divReceipt" runat="server" visible="<%# Constants.RECEIPT_OPTION_ENABLED %>">
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody id="tbdyReceiptErrorMessages" runat="server" visible="false">
													<tr>
														<td class="edit_title_bg" align="center" colspan="3">エラーメッセージ</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="left" colspan="3">
															<asp:Label ID="lbReceiptErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													</tbody>
													<tbody>
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">領収書情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="25%">領収書希望</td>
														<td class="detail_item_bg" align="left" width="60%">
															<asp:RadioButtonList id="rblReceiptFlg" runat="server" DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG) %>"
																DataValueField="Value" DataTextField="Text" SelectedValue="<%# this.FixedPurchaseContainer.ReceiptFlg %>" RepeatDirection="Horizontal" RepeatLayout="Flow"
																OnSelectedIndexChanged="rblReceiptFlg_SelectedIndexChanged" AutoPostBack="True"/>
														</td>
														<td class="detail_item_bg" align="center" rowspan="3">
															<asp:Button ID="btnUpdateReceipt" Runat="server" Text="  領収書情報更新  " OnClick="btnUpdateReceipt_Click" />
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">宛名
															<% if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON){ %><span class="notice">*</span><% } %>
														</td>
														<td class="detail_item_bg" align="left">
															<asp:TextBox ID="tbReceiptAddress" Text="<%# this.FixedPurchaseContainer.ReceiptAddress %>" Width="600" runat="server" MaxLength="100" Enabled="<%# rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON %>"/>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">但し書き
															<% if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON){ %><span class="notice">*</span><% } %>
														</td>
														<td class="detail_item_bg" align="left">
															<asp:TextBox ID="tbReceiptProviso" Text="<%# this.FixedPurchaseContainer.ReceiptProviso %>" Width="600" runat="server" MaxLength="100" Enabled="<%# rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON %>"/>
														</td>
													</tr>
													</tbody>
												</table>
												<br />
												</div>
												<!-- △領収書情報△ -->
												<%--▽ 注文拡張項目 ▽--%>
												<div id="dvOrderExtend" runat="server" visible="<%# Constants.ORDER_EXTEND_OPTION_ENABLED %>">
													<br />
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tbody id="tbByOrderExtendErrorMessages" runat="server" visible="false">
															<tr>
																<td class="edit_title_bg" align="center" colspan="3">エラーメッセージ</td>
															</tr>
															<tr>
																<td class="edit_item_bg" align="left" colspan="3">
																	<asp:Label ID="lbOrderExtendErrorMessages" runat="server" ForeColor="red" />
																</td>
															</tr>
														</tbody>
														<tr>
															<td class="detail_title_bg" align="center" colspan="3">注文拡張項目</td>
														</tr>
														<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server">
															<ItemTemplate>
																<tr>
																	<td class="detail_title_bg" width="120">
																	<%-- 項目名 --%>
																	<%#: Item.SettingModel.SettingName %>
																	</td>
																	<%-- TEXT --%>
																	<td class="detail_item_bg" align="left" width="518" runat="server" visible="<%# Item.SettingModel.IsInputTypeText%>" >
																		<asp:TextBox runat="server" ID="tbSelect" Width="250px" MaxLength="100"></asp:TextBox>
																	</td>
																	<%-- DDL --%>
																	<td class="detail_item_bg" align="left" width="518" runat="server" visible="<%# Item.SettingModel.IsInputTypeDropDown %>">
																		<asp:DropDownList runat="server" ID="ddlSelect"></asp:DropDownList>
																	</td>
																	<%-- RADIO --%>
																	<td class="detail_item_bg" align="left" width="518" runat="server" visible="<%# Item.SettingModel.IsInputTypeRadio %>">
																		<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
																	</td>
																	<%-- CHECK --%>
																	<td class="detail_item_bg" align="left" width="518" runat="server" visible="<%# Item.SettingModel.IsInputTypeCheckBox %>">
																		<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="checkBox"></asp:CheckBoxList>
																	</td>
																</tr>
																<%-- 検証文言 --%>
																<asp:Label runat="server" ID="lbErrMessage" CssClass="error_inline"></asp:Label>
																<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingModel.SettingId %>" />
																<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.SettingModel.InputType %>" />
															</ItemTemplate>
														</asp:Repeater>
														<tr>
															<td class="detail_item_bg" align="right" colspan="3">
																<asp:Button ID="btnUpdateOrderExtend" Runat="server" Text="  注文拡張更新  " Width="120" OnClick="btnUpdateOrderExtend_OnClick" />
															</td>
														</tr>
													</table>
													<br />
												</div>
												<%--△ 注文拡張項目 △--%>
												<!-- ▽注文メモ情報▽ -->
												<div id="divMemo" runat="server">
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tbody>
															<tr>
																<td class="edit_title_bg" align="center">
																	注文メモ
																	<uc:FieldMemoSetting ID="FieldMemoSetting2" runat="server" Title="注文メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_FIXEDPURCHASE %>" FieldName="<%# Constants.FIELD_FIXEDPURCHASE_MEMO %>" />
																</td>
															</tr>
															<tr>
																<td class="edit_item_bg" align="right">
																	<asp:TextBox id="tbMemo" runat="server" TextMode="MultiLine" Rows="8" Width="99%"></asp:TextBox>
																	<asp:Button ID="btnUpdateFixedPurchaseMemo" Text="  注文メモ更新  " runat="server" OnClick="btnUpdateFixedPurchaseMemo_Click" />
																</td>
															</tr>
														</tbody>
													</table>
													<br />
												</div>
												<!-- △注文メモ情報△ -->
												<%--▽ 管理メモ ▽--%>
												<div id="divManagementMemo" runat="server">
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center">
																管理メモ
																<uc:FieldMemoSetting ID="FieldMemoSetting1" runat="server" Title="管理メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_FIXEDPURCHASE %>" FieldName="<%# Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO %>" />
															</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="right">
																<asp:TextBox id="tbFixedPurchaseManagementMemo" runat="server" TextMode="MultiLine" Rows="8" Width="99%" /><br />
																<asp:Button ID="btnFixedPurchaseUpdateManagementMemo" Text="  管理メモ更新  " runat="server" OnClick="btnUpdateFixedPurchaseManagementMemo_Click" />
															</td>
														</tr>
													</table>
													<br />
												</div>
												<%--△ 管理メモ △--%>
											<%--▽ 配送メモ ▽--%>
											<div id="divShippingMemo" runat="server">
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center">
															配送メモ
															<uc:FieldMemoSetting runat="server" Title="配送メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_FIXEDPURCHASE %>" FieldName="<%# Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO %>" />
														</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="right">
															<asp:TextBox id="tbShippingMemo" runat="server" TextMode="MultiLine" Rows="8" Width="99%" /><br />
															<asp:Button ID="btnShippingMemo" Text="  配送メモ更新  " runat="server" OnClick="btnUpdateShippingMemo_Click" />
														</td>
													</tr>
												</table>
												<br />
											</div>
											<%--△ 配送メモ △--%>
												<%--▽ 休止 ▽--%>
												<div id="dvFixedPurchaseSuspend" runat="server">
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">定期購入の休止</td>
														</tr>
														<%--▽ 定期購入の休止エラー表示 ▽--%>
														<tr id="trFixedPurchaseSuspendErrorMessages" runat="server" visible="false">
															<td class="edit_item_bg" align="left" colspan="2">
																<asp:Label ID="lbFixedPurchaseSuspendErrorMessages" runat="server" ForeColor="red" />
															</td>
														</tr>
														<%--△ 定期購入の休止エラー表示 △--%>
														<tr id="trFixedPurchaseResumetDate" runat="server">
															<td class="edit_title_bg" align="left" width="160">定期再開予定日</td>
															<td class="edit_item_bg" align="left">
																<uc:DateTimePickerPeriodInput id="ucFixedPurchaseResumetDate" CanShowEndDatePicker="False" IsNullStartDateTime="True" IsHideTime="True" runat="server" />
																<span class="search_btn_sub">(<a href="Javascript:SetDate('onemonthlater');">一か月後</a>｜<a href="Javascript:SetDate('clear');">クリア</a>)</span>
																<br/>※日付を指定しない場合は、無期限で登録します。
																<br/>※日付を指定する場合は、次回配送日を適宜変更してください。
															</td>
														</tr>
														<tr id="trSuspendReason" runat="server">
															<td class="edit_title_bg" align="left" width="160">休止理由</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbSuspendReason" runat="server" Width="99%" TextMode="MultiLine" Rows="5"></asp:TextBox>
															</td>
														</tr>
														<tr id="trFixedPurchaseSuspendReasonButtonDisplay" runat="server">
															<td class="edit_item_bg" align="right" colspan="2">
																<asp:Button ID="btnFixedPurchaseSuspendReason" Text="  休止理由更新  " runat="server" OnClick="btnFixedPurchaseSuspendReason_Click" />
															</td>
														</tr>
														
														<tr id="trFixedPurchaseSuspendButtonDisplay" runat="server">
															<td class="detail_item_bg" align="center" colspan="2">
																この定期購入を一時休止する場合には、休止するボタンを押下してください。
																<span style="color:red" runat="server" Visible="<%# this.IsCancelable == false %>"><br/>
																	この定期台帳の商品は、出荷回数が<%: this.FixedPurchaseCancelableCount %>回以上から一時休止可能と設定されているので、一時休止ができません。
																</span><br/>
																<asp:Button ID="btnFixedPurchaseSuspend" runat="server" Text="    休止する    " OnClick="btnFixedPurchaseSuspend_Click" OnClientClick="return confirm('本当に休止してもよろしいですか？\n（※既に自動で登録された注文はキャンセルされません。）');" />
															</td>
														</tr>
													</table>
													<br />
												</div>
												<%--△ 休止 △--%>
												<%--▽ 解約 ▽--%>
												<div id="dvFixedPurchaseCancel" runat="server">
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">定期購入の解約</td>
														</tr>
														<tr id="trCancelReason" runat="server">
															<td class="edit_title_bg" align="left" width="160">解約理由</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList id="ddlCancelReason" runat="server" Width="575"></asp:DropDownList>
															</td>
														</tr>
														<tr id="trCancelMemo" runat="server">
															<td class="edit_title_bg" align="left" width="160">解約メモ</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbCancelMemo" runat="server" Width="99%" TextMode="MultiLine" Rows="5"></asp:TextBox>
															</td>
														</tr>
														<tr id="trFixedPurchaseCancelReasonButtonDisplay" runat="server">
															<td class="edit_item_bg" align="right" colspan="2">
																<asp:Button ID="btnFixedPurchaseCancelReason" Text="  解約理由更新  " runat="server" OnClick="btnFixedPurchaseCancelReason_Click" />
															</td>
														</tr>
														<tr id="trFixedPurchaseCancelButtonDisplay" runat="server">
															<td class="detail_item_bg" align="center" colspan="2">
																この定期購入を解約する場合には、解約するボタンを押下してください。
																<span style="color:red" runat="server" Visible="<%# this.IsCancelable == false %>"><br/>
																	この定期台帳の商品は、出荷回数が<%: this.FixedPurchaseCancelableCount %>回以上から解約可能と設定されているので、解約ができません。
																</span>
																<br/>
																<span runat="server" Visible='<%# (this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY) && (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus == false) %>'>
																	<asp:Literal ID="lCancelPaypayNotification" runat="server"/>
																</span>
																<br/>
																<asp:Button ID="btnFixedPurchaseCancel" runat="server" Text="    解約する    " OnClick="btnFixedPurchaseCancel_Click" OnClientClick="return confirm('本当に解約してもよろしいですか？\n（※既に自動で登録された注文はキャンセルされません。）');" />
															</td>
														</tr>
														<tr id="trTemporaryRegistrationCancelButtonDisplay" runat="server" Visible="false">
															<td class="detail_item_bg" align="center" colspan="2">
																この定期購入を仮登録キャンセルする場合には、仮登録キャンセルするボタンを押下してください。<br/>
																<asp:Button ID="btnTemporaryRegistrationCancel" runat="server" Text="    仮登録キャンセルする    " OnClick="btnTemporaryRegistrationCancel_Click" OnClientClick="return confirm('本当に仮登録キャンセルしてもよろしいですか？');" />
															</td>
														</tr>
													</table>
													<br />
												</div>
												<%--△ 解約 △--%>
												<%--▽ 定期購入履歴一覧 ▽--%>
												<p id="anchorFixedPurchaseHistory" align="right" style="margin-bottom:5px;"><a href="#top">ページトップ</a></p>
												<div id="dvFixedPurchaseHistory" runat="server">
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="9" id="OrderHistory">
																定期購入履歴一覧
																<asp:Label ID="lbFixedPurchaseHistoryCount" runat="server" Visible="false"/>
															</td>
														</tr>
														<asp:Repeater id="rFixedPurchaseHistory" runat="server">
															<HeaderTemplate>
																<tr>
																	<td class="detail_title_bg" align="center" width="120" rowspan="2">作成日時</td>
																	<td class="detail_title_bg" align="center" width="160" rowspan="2">履歴区分</td>
																	<td class="detail_title_bg" align="center" width="148" colspan="2">購入回数</td>
																	<td class="detail_title_bg" align="center" width="110" rowspan="2">注文ID</td>
																	<td class="detail_title_bg" align="center" width="100" rowspan="2">支払金額合計</td>
																	<td class="detail_title_bg" align="center" width="100" rowspan="2">最終更新者</td>
																	<td class="detail_title_bg" align="center" width="100" rowspan="2">外部決済連携ログ</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="center" width="84">注文基準</td>
																	<td class="detail_title_bg" align="center" width="84">出荷基準</td>
																</tr>
															</HeaderTemplate>
															<ItemTemplate>
																<tr class="fixedPurchaseHistory">
																	<td class="detail_item_bg" align="center" ><%#: DateTimeUtility.ToStringForManager(((FixedPurchaseHistoryListSearchResult)Container.DataItem).DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
																	<td class="detail_item_bg" align="center" >
																		<%# WebSanitizer.HtmlEncode(((FixedPurchaseHistoryListSearchResult)Container.DataItem).FixedPurchaseHistoryKbnText)%>
																	</td>
																	<td class="detail_item_bg" align="center" >
																		<%# WebSanitizer.HtmlEncode(((FixedPurchaseHistoryListSearchResult)Container.DataItem).UpdateOrderCountDisplay)%> <%# WebSanitizer.HtmlEncode(((FixedPurchaseHistoryListSearchResult)Container.DataItem).UpdateOrderCountResultDisplay)%>
																	</td>
																	<td class="detail_item_bg" align="center" >
																		<%# WebSanitizer.HtmlEncode(((FixedPurchaseHistoryListSearchResult)Container.DataItem).UpdateShippedCountDisplay)%> <%# WebSanitizer.HtmlEncode(((FixedPurchaseHistoryListSearchResult)Container.DataItem).UpdateShippedCountResultDisplay)%>
																	</td>
																	<td class="detail_item_bg" align="center" >
																		<%# CreateOrderDetailLink(((FixedPurchaseHistoryListSearchResult)Container.DataItem).OrderId)%>
																	</td>
																	<td class="detail_item_bg" align="center" ><%# WebSanitizer.HtmlEncode((((FixedPurchaseHistoryListSearchResult)Container.DataItem).FixedPurchaseHistoryKbn == Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS) ? ((FixedPurchaseHistoryListSearchResult)Container.DataItem).OrderPriceTotal.ToPriceString(true) : "-" ) %></td>
																	<td class="detail_item_bg" align="center" ><%# WebSanitizer.HtmlEncode(((FixedPurchaseHistoryListSearchResult)Container.DataItem).LastChanged)%></td>
																	<td class="detail_item_bg" align="center">
																			<div runat="server" visible="<%# IsNullOrEmptyExternalPaymentCooperationLog(((FixedPurchaseHistoryListSearchResult)Container.DataItem).ExternalPaymentCooperationLog) %>">-</div>
																			<div runat="server" visible="<%# IsNullOrEmptyExternalPaymentCooperationLog(((FixedPurchaseHistoryListSearchResult)Container.DataItem).ExternalPaymentCooperationLog) == false%>"><a href="<%#: CreateExternalPaymentCooperationHref(((FixedPurchaseHistoryListSearchResult)Container.DataItem).FixedPurchaseId,((FixedPurchaseHistoryListSearchResult)Container.DataItem).FixedPurchaseHistoryNo.ToString()) %>" title="<%#: ((FixedPurchaseHistoryListSearchResult)Container.DataItem).ExternalPaymentCooperationLog%>">詳細</a></div>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr id="trFixedPurchaseHistoryError" class="list_alert" runat="server" Visible="false">
															<td id="tdErrorMessage" colspan="9" runat="server">
															</td>
														</tr>
														<tr id="trFixedPurchaseMore" class="detail_title_bg" runat="server">
															<td colspan="9">
																<a id="showFixedPurchase" href="#anchorFixedPurchaseHistory" onclick="ShowMore('fixedPurchaseHistory','<%=trFixedPurchaseMore.ClientID %>')" style="width:100%;display:inline-block;text-align:center;text-decoration:none">すべて見る</a>
															</td>
														</tr>
													</table>
												</div>
												<%--△ 定期購入履歴一覧 △--%>												
												<div class="action_part_bottom">
													<asp:Button id="btnBackBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button ID="btnToListBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" />
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
<script type="text/javascript">
<!--
	$(document).ready(function () {
		displayMemoPopup();
		ShowLimitItems("fixedPurchaseHistory");
	});

	function ShowLimitItems(historyItem)
	{
		$('.' + historyItem).hide();
		$('.' + historyItem).each(function(index) {
			if (index >= <%=Constants.ITEMS_HISTORY_FIRST_DISPLAY %>) return false;

			$(this).show();
		});
		}

		function ShowMore(historyItem, showMore)
		{
			$('.' + historyItem).show();
			$('#' + showMore).hide();
		}

		// メール送信画面表示
		function open_fixed_purchase_send_mail() {
			if (document.getElementById('<%= ddlFixedPurchaseMailId.ClientID %>').value != '') {
			var url = '<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_SEND %>'
				+ '?' + '<%= Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID %>' + '=' + '<%= HttpUtility.UrlEncode(this.FixedPurchaseContainer.FixedPurchaseId) %>'
				+ '&' + '<%= Constants.REQUEST_KEY_MAIL_TEMPLATE_ID %>' + '=' + encodeURIComponent(document.getElementById('<%= ddlFixedPurchaseMailId.ClientID %>').value);

			open_window(url, 'fixedpurchase_send_mail', 'width=850,height=825,top=120,left=320,status=NO,scrollbars=yes');
		}
		else {
			alert('メールテンプレートを選択してください');
		}
	}

	// 子ウィンドウからのリロード
	function reload_list() {
		<%= Page.ClientScript.GetPostBackEventReference(lbReloadList, "") %>
	}
	//-->

	function SetDate(set_day) {
		if (set_day == 'onemonthlater') {
			document.getElementById('<%= ucFixedPurchaseResumetDate.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.AddMonths(1)) %>';
			document.getElementById('<%= ucFixedPurchaseResumetDate.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucFixedPurchaseResumetDate.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.AddMonths(1)) %>';
			document.getElementById('<%= ucFixedPurchaseResumetDate.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucFixedPurchaseResumetDate.ClientID %>');
		}
		else if (set_day == 'clear') {
			document.getElementById('<%= ucFixedPurchaseResumetDate.HfStartDate.ClientID %>').value = '';
			document.getElementById('<%= ucFixedPurchaseResumetDate.HfStartTime.ClientID %>').value = '';
			document.getElementById('<%= ucFixedPurchaseResumetDate.HfEndDate.ClientID %>').value = '';
			document.getElementById('<%= ucFixedPurchaseResumetDate.HfEndTime.ClientID %>').value = '';
			reloadDisplayDateTimePeriod('<%= ucFixedPurchaseResumetDate.ClientID %>');
		}
	}

	// ユーザークーポンセット
	function action_set_coupon(coupon_code) {
		document.getElementById('<%= tbCouponCode.ClientID %>').value = coupon_code;
	}

	// Use point update click
	function usePointUpdateClick() {
		var messageConfirmUpdate = 'ご利用ポイントを変更します。よろしいですか？';
		<% if (this.CanUsePointForPurchase == false) { %>
			var tbNextShippingUsePoint = document.getElementById('<%= tbNextShippingUsePoint.ClientID %>');
			if (parseInt(tbNextShippingUsePoint.value) > 0)
			{
				var messageCanUsePoint = "<%= GetPriceCanPurchaseUsePoint() %>";
				return confirm(messageCanUsePoint + "\n" + messageConfirmUpdate);
			}
		<% } %>

		return confirm(messageConfirmUpdate);
	}
</script>

<!-- 子ウィンドウからのリロード用ボタン -->
<asp:LinkButton ID="lbReloadList" runat="server" OnClick="lbReloadList_Click"></asp:LinkButton>

</asp:Content>
