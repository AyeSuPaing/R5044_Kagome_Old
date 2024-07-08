<%--
=========================================================================================================
  Module      : クーポン発行スケジュール確認ページ(CouponScheduleConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CouponScheduleConfirm.aspx.cs" Inherits="Form_CouponSchedule_CouponScheduleConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">クーポン発行スケジュール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<tr>
		<% if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL){%>
		<td><h2 class="cmn-hed-h2">クーポン発行スケジュール設定詳細</h2></td>
		<%} %>
		<% if (this.ActionStatus == Constants.ACTION_STATUS_CONFIRM){%>
		<td><h2 class="cmn-hed-h2">クーポン発行スケジュール設定確認</h2></td>
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
														<td class="detail_title_bg" align="left" width="30%">クーポン発行スケジュールID</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lCouponScheduleId" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">クーポン発行スケジュール名</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lCouponScheduleName" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">ターゲット</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lTarget" runat="server"></asp:Label>
														</td> 
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">クーポン設定</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lCoupon" runat="server"></asp:Label>
														</td>
													</tr>
													<tr runat="server" id="trCouponQuantity">
														<td class="detail_title_bg" align="left" width="30%">クーポン発行枚数</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lPublishQuantity" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">メール文章</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lMailTemp" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg">実行タイミング</td>
														<td class="detail_item_bg">
															<asp:Label ID="lScheduleString" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">有効フラグ</td>
														<td class="detail_item_bg" align="left">
															<b>
																<asp:Label ID="lValidFlg" runat="server"></asp:Label>
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
																<asp:Label ID="lStatus" runat="server"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">最終付与人数</td>
															<td class="detail_item_bg" align="left">
																<asp:Label ID="lLastCount" runat="server"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg">
																最終付与日時</td>
															<td class="detail_item_bg">
																<asp:Label ID="lLastExecDate" runat="server"></asp:Label>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%" style="height: 28px">アクション</td>
															<td class="detail_item_bg" align="left" style="height: 28px">
																<input type= "button" onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ACTIONWINDOW_SCHEDULEEXECUTE + "?" + Constants.REQUEST_KEY_ACTION_KBN + "=" + Constants.FLG_TASKSCHEDULE_ACTION_KBN_PUBLISH_COUPON + "&" + Constants.REQUEST_KEY_MASTER_ID + "=" + this.CouponScheduleId) %>	','contact','width=850,height=580,top=120,left=420,status=NO,scrollbars=yes');" value="アクションウィンドウを開く" />
															</td>
														</tr>
													</table>
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
