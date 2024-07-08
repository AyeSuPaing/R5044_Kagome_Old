﻿<%--
=========================================================================================================
  Module      : オペレータ情報確認ページ(OperatorConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OperatorConfirm.aspx.cs" Inherits="Form_Operator_OperatorConfirm" %>
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
				<% if(Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_DETAIL){%>
					オペレータ情報詳細
				<%} %>
				<% if(Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT){%>
					オペレータ情報入力確認
				<%} %>
				<% if(Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE){%>
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
												<div class="action_part_top"><input type="button" value="  戻る  " onclick="Javascript:history.back();" />
												<asp:Button id="btnEditTop" runat="server" Visible="False" Text="  編集する  " OnClick="btnEdit_Click" />
												<asp:Button ID="btnDeleteTop" Runat="server" visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return delete_confirm_check();" />
												<asp:Button id="btnUpdateTop" runat="server" Visible="False" Text="  更新する  " OnClick="btnUpdate_Click" />
												<asp:Button id="btnInsertTop" runat="server" Visible="False" Text="  登録する  " OnClick="btnInsert_Click" /></div>
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr id="trOperatorId" runat="server" visible="false">
														<td align="left" class="detail_title_bg" width="30%">オペレータID</td>
														<td align="left" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(m_htOperator[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID])%></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">オペレータ名</td>
														<td align="left" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(m_htOperator[Constants.FIELD_SHOPOPERATOR_NAME])%></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">メニュー権限</td>
														<td align="left" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(m_htOperator[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG + "_name"]) %></td>
													</tr>
													<% if (Constants.TWO_STEP_AUTHENTICATION_OPTION_ENABLED) { %>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">メールアドレス</td>
														<td align="left" class="detail_item_bg"><%#: m_htOperator[Constants.FIELD_SHOPOPERATOR_MAIL_ADDR] %></td>
													</tr>
													<% } %>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">ログインＩＤ</td>
														<td align="left" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(m_htOperator[Constants.FIELD_SHOPOPERATOR_LOGIN_ID])%></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">パスワード</td>
														<td align="left" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(((string)m_htOperator[Constants.FIELD_SHOPOPERATOR_PASSWORD] != "") ? StringUtility.ChangeToAster((string)m_htOperator[Constants.FIELD_SHOPOPERATOR_PASSWORD]) : "（パスワード変更なし）") %></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">有効フラグ</td>
														<td align="left" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPOPERATOR, Constants.FIELD_SHOPOPERATOR_VALID_FLG, m_htOperator[Constants.FIELD_SHOPOPERATOR_VALID_FLG])) %></td>
													</tr>
													<tr id="trDateCreated" runat="server" visible="false">
														<td align="left" class="detail_title_bg" width="30%">作成日</td>
														<td align="left" class="detail_item_bg"><%#: DateTimeUtility.ToStringForManager(m_htOperator[Constants.FIELD_SHOPOPERATOR_DATE_CREATED], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trDateChanged" runat="server" visible="false">
														<td align="left" class="detail_title_bg" width="30%">更新日</td>
														<td align="left" class="detail_item_bg"><%#: DateTimeUtility.ToStringForManager(m_htOperator[Constants.FIELD_SHOPOPERATOR_DATE_CHANGED], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trLastChanged" runat="server" visible="false">
														<td align="left" class="detail_title_bg" width="30%">最終更新者</td>
														<td align="left" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(m_htOperator[Constants.FIELD_SHOPOPERATOR_LAST_CHANGED]) %></td>
													</tr>
												</table>
												<% if (Constants.REALSHOP_OPTION_ENABLED) { %>
													<img height="33" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
														<tr>
															<td align="left" class="detail_title_bg" colspan="2">
																リアル店舗オペレータ情報
															</td>
														</tr>
														<tr>
															<td align="left" class="detail_title_bg" width="30%">
																所属店舗
															</td>
															<td align="left" class="detail_item_bg">
																<%# string.IsNullOrEmpty((string)m_htOperator[Constants.FLG_AREA_REAL_SHOP_NAME])
																	? "-"
																	: (string)m_htOperator[Constants.FLG_AREA_REAL_SHOP_NAME] %>
															</td>
														</tr>
													</table>
												<% } %>
												<div class="action_part_bottom"><input type="button" value="  戻る  " onclick="Javascript:history.back();" />
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
	var strDeleteOperatorId = '<%# WebSanitizer.HtmlEncode(m_htOperator[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]) %>';
	function delete_confirm_check() {
		if (strLoginOperatorId == strDeleteOperatorId){
			alert('ログインしているオペレータIDは削除できません。');
			return false;
		} else {
			return confirm('情報を削除してもよろしいですか？'); ;
		}
	}
//-->
</script>
</asp:Content>
