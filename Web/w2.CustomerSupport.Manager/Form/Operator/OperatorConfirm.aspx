<%--
=========================================================================================================
  Module      : オペレータ情報確認ページ(OperatorConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OperatorConfirm.aspx.cs" Inherits="Form_Operator_OperatorConfirm" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">オペレータ情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<tr>
		<td>
			<h2 class="cmn-hed-h2">
				<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_DETAIL){%>
					オペレータ情報詳細
				<%} %>
				<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE){%>
					オペレータ情報入力確認
				<%} %>
				<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT){%>
					オペレータ情報入力確認
				<%} %>
			</h2>
		</td>
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
												<div class="action_part_top"><a class="btn-back" href="<%= HtmlSanitizer.UrlAttrHtmlEncode(GetBackButtonUrl()) %>">  戻る  </a>
												<asp:Button id="btnEditTop" runat="server" Visible="False" Text="  編集する  " OnClick="btnEdit_Click" />
												<asp:Button ID="btnDeleteTop" Runat="server" visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return delete_confirm_check();" />
												<asp:Button id="btnUpdateTop" runat="server" Visible="False" Text="  更新する  " OnClick="btnUpdate_Click" />
												<asp:Button id="btnInsertTop" runat="server" Visible="False" Text="  登録する  " OnClick="btnInsert_Click" /></div>
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr id="trOperatorId" runat="server" visible="false">
														<td align="left" class="detail_title_bg" width="30%">オペレータID</td>
														<td align="left" class="detail_item_bg"><%#: this.ShopOperatorModel.OperatorId%></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">オペレータ名</td>
														<td align="left" class="detail_item_bg"><%#: this.ShopOperatorModel.Name%></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">メニュー権限</td>
														<td align="left" class="detail_item_bg"><%#: this.IsCsOperator ? this.CsOperatorModel.EX_MenuAuthorityName : Constants.STRING_UNACCESSABLEUSER_NAME %></td>
													</tr>
													<% if (Constants.TWO_STEP_AUTHENTICATION_OPTION_ENABLED) { %>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">メールアドレス</td>
														<td align="left" class="detail_item_bg"><%#: this.ShopOperatorModel.MailAddr %></td>
													</tr>
													<% } %>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">ログインＩＤ</td>
														<td align="left" class="detail_item_bg"><%#: this.ShopOperatorModel.LoginId%></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">パスワード</td>
														<td align="left" class="detail_item_bg"><%#: (this.ShopOperatorModel.Password != "") ? StringUtility.ChangeToAster(this.ShopOperatorModel.Password) : "（パスワード変更なし）" %></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">有効フラグ</td>
														<td align="left" class="detail_item_bg"><%#: ValueText.GetValueText(Constants.TABLE_SHOPOPERATOR, Constants.FIELD_SHOPOPERATOR_VALID_FLG, this.ShopOperatorModel.ValidFlg) %></td>
													</tr>
													<tr id="trDateCreated" runat="server" visible="false">
														<td align="left" class="detail_title_bg" width="30%">作成日</td>
														<td align="left" class="detail_item_bg"><%#: (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_DETAIL) ? DateTimeUtility.ToStringForManager(this.ShopOperatorModel.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) : string.Empty %></td>
													</tr>
													<tr id="trDateChanged" runat="server" visible="false">
														<td align="left" class="detail_title_bg" width="30%">更新日</td>
														<td align="left" class="detail_item_bg"><%#: Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_DETAIL ? DateTimeUtility.ToStringForManager(this.ShopOperatorModel.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) : string.Empty%></td>
													</tr>
													<tr id="trLastChanged" runat="server" visible="false">
														<td align="left" class="detail_title_bg" width="30%">最終更新者</td>
														<td align="left" class="detail_item_bg"><%#: this.ShopOperatorModel.LastChanged %></td>
													</tr>
												</table>
												<br />
												<!--▽ CSオペレータ欄 ▽-->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server" ID="tabCsOperator" Visible="<%# this.IsCsOperator %>">
													<tr>
														<td class="edit_title_bg" align="left" width="30%" colspan="2">CSオペレータ情報</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">オペレータ権限の選択</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(this.IsCsOperator ? this.CsOperatorModel.EX_OperatorAuthorityName ?? Constants.STRING_UNACCESSABLEUSER_NAME : string.Empty)%></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール送信元の選択</td>
														<td class="edit_item_bg" align="left"><%# WebSanitizer.HtmlEncode(this.IsCsOperator ? this.CsOperatorModel.EX_MailFromDisplayName : string.Empty) %></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール受信設定</td>
														<td class="edit_item_bg" align="left">
															通知メール：<%# this.IsCsOperator ? this.CsOperatorModel.EX_NotifyInfoFlgText : string.Empty %>
															警告メール：<%# this.IsCsOperator ? this.CsOperatorModel.EX_NotifyWarnFlgText : string.Empty %>
															&nbsp;
															メールアドレス：<%# this.IsCsOperator ? this.CsOperatorModel.MailAddr : string.Empty %>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">インシデント滞留警告アイコン表示設定（未対応）</td>
														<td class="edit_item_bg" align="left">
															有効：<asp:Literal ID="lIncidentWarningIconNoneValid" runat="server">×</asp:Literal>&nbsp;
															橙色アイコン切替時間：<asp:Literal ID="lIncidentWarningIconNoneOrange" runat="server">－</asp:Literal>&nbsp;
															赤色アイコン切替時間：<asp:Literal ID="lIncidentWarningIconNoneRed" runat="server">－</asp:Literal>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">インシデント滞留警告アイコン表示設定（対応中）</td>
														<td class="edit_item_bg" align="left">
															有効：<asp:Literal ID="lIncidentWarningIconActiveValid" runat="server">×</asp:Literal>&nbsp;
															橙色アイコン切替時間：<asp:Literal ID="lIncidentWarningIconActiveOrange" runat="server">－</asp:Literal>&nbsp;
															赤色アイコン切替時間：<asp:Literal ID="lIncidentWarningIconActiveRed" runat="server">－</asp:Literal>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">インシデント滞留警告アイコン表示設定（保留）</td>
														<td class="edit_item_bg" align="left">
															有効：<asp:Literal ID="lIncidentWarningIconSuspendValid" runat="server">×</asp:Literal>&nbsp;
															橙色アイコン切替時間：<asp:Literal ID="lIncidentWarningIconSuspendOrange" runat="server">－</asp:Literal>&nbsp;
															赤色アイコン切替時間：<asp:Literal ID="lIncidentWarningIconSuspendRed" runat="server">－</asp:Literal>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">インシデント滞留警告アイコン表示設定（至急）</td>
														<td class="edit_item_bg" align="left">
															有効：<asp:Literal ID="lIncidentWarningIconUrgentValid" runat="server">×</asp:Literal>&nbsp;
															橙色アイコン切替時間：<asp:Literal ID="lIncidentWarningIconUrgentOrange" runat="server">－</asp:Literal>&nbsp;
															赤色アイコン切替時間：<asp:Literal ID="lIncidentWarningIconUrgentRed" runat="server">－</asp:Literal>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">表示順</td>
														<td class="edit_item_bg" align="left"><%# this.IsCsOperator ? this.CsOperatorModel.DisplayOrder.ToString() : string.Empty %></td>
													</tr>
												</table>
												<!--△ CSオペレータ欄 △-->
												<div class="action_part_bottom"><a class="btn-back" href="<%= HtmlSanitizer.UrlAttrHtmlEncode(GetBackButtonUrl()) %>">  戻る  </a>
												<asp:Button id="btnEditBottom" runat="server" Visible="False" Text="  編集する  " OnClick="btnEdit_Click" />
												<asp:Button ID="btnDeleteBottom" Runat="server" visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return delete_confirm_check();" />
												<asp:Button id="btnUpdateBottom" runat="server" Visible="False" Text="  更新する  " OnClick="btnUpdate_Click" />
												<asp:Button id="btnInsertBottom" runat="server" Visible="False" Text="  登録する  " OnClick="btnInsert_Click" /></div>
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
	var strLoginOperatorId = '<%# this.LoginOperatorId %>';
	var strDeleteOperatorId = '<%#: this.ShopOperatorModel.OperatorId %>';
	function delete_confirm_check() {
		if (strLoginOperatorId == strDeleteOperatorId){
			alert('ログインしているオペレータIDは削除できません。');
			return false;
		} else {
			return confirm('情報を削除してもよろしいですか？'); ;
		}
	}

	$(function() {
		sessionStorage.removeItem("term_table");
	});
//-->
</script>
</asp:Content>
