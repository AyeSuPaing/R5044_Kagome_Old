<%--
=========================================================================================================
  Module      : Advertisement Code Register(AdvertisementCodeRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AdvertisementCodeRegister.aspx.cs" Inherits="Form_AdvertisementCode_AdvertisementCodeRegister" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">広告コード設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">広告コード設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">広告コード設定登録</h2></td>
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
											<td align="center">
												<div id="divComp" runat="server" class="action_part_top" Visible="False">
												<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
													<tr class="info_item_bg">
														<td align="left">広告コード設定を登録/更新しました。
														</td>
													</tr>
												</table>
												</div>
												<div class="action_part_top">
													<input type="button" value='  一覧へ戻る  ' onclick="Javascript:location.href='<%= CreateAdvCodeListUrl() %>';"/>
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" onclick="btnCopyInsert_Click"></asp:Button>
													<asp:Button ID="btnDeleteTop" runat="server" Text="  削除する  " OnClientClick="return confirm('削除します。よろしいですか？');" Visible="false" OnClick="btnDelete_Click" />
													<asp:Button id="btnInsertUpdateTopRegist" runat="server" Visible="true" Text="  登録する  " OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnInsertUpdateTopUpdate" runat="server" Visible="false" Text="  更新する  " OnClick="btnInsertUpdate_Click" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">広告媒体区分</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList id="ddlAdvCodeMediaType" runat="server" Width="130"></asp:DropDownList>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">広告コード<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:HiddenField ID="hfAdvCodeNo" runat="server" Value="<%# m_inputParam[Constants.FIELD_ADVCODE_ADVCODE_NO] %>"></asp:HiddenField>
																<asp:TextBox id="tbAdvertisementCode" runat="server" Text="<%# m_inputParam[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE] %>" Width="200" MaxLength="30"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">媒体名<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbMediaName" runat="server" Text="<%# m_inputParam[Constants.FIELD_ADVCODE_MEDIA_NAME] %>" Width="200" MaxLength="50"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">媒体費</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbMediaCost" runat="server" Text="<%# m_inputParam[Constants.FIELD_ADVCODE_MEDIA_COST].ToPriceString()%>" Width="200" MaxLength="30"></asp:TextBox> <%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">出稿日</td>
															<td class="edit_item_bg" align="left">
																<uc:DateTimePickerPeriodInput id="ucAdvCodeDate" runat="server" CanShowEndDatePicker="False" IsNullStartDateTime="True" IsNullEndDateTime="True" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">媒体掲載期間</td>
															<td class="edit_item_bg" align="left">
																<p>
																	<uc:DateTimePickerPeriodInput id="ucPublishDatePeriod" runat="server"  IsNullEndDateTime="True" />
																</p>
															</td>
														</tr>
														<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">会員登録時紐づけ会員ランク</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList id="ddlMemberRank" runat="server" Width="130"></asp:DropDownList>
															</td>
														</tr>
														<% } %>
														<%if (Constants.USERMANAGEMENTLEVELSETTING_OPTION_ENABLED) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">会員登録時紐づけユーザー管理レベル</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList id="ddlUserManagementLevel" runat="server" Width="130"></asp:DropDownList>
															</td>
														</tr>
														<% } %>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBox ID="cbValidFlg" Runat="server" Checked='<%# StringUtility.ToEmpty(m_inputParam[Constants.FIELD_ADVCODE_VALID_FLG]) == "" ? false : (string)m_inputParam[Constants.FIELD_ADVCODE_VALID_FLG] == Constants.FLG_ADVCODE_VALID_FLG_VALID ? true : false %>' Text="有効"></asp:CheckBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">備考</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbMemo" runat="server" Text="<%# m_inputParam[Constants.FIELD_ADVCODE_MEMO] %>" Width="300" TextMode="MultiLine" Rows="3"></asp:TextBox></td>
														</tr>
													</tbody>
												</table>
												<div class="action_part_bottom">
													<input type="button" value='  一覧へ戻る  ' onclick="Javascript:location.href='<%= CreateAdvCodeListUrl() %>';"/>
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" onclick="btnCopyInsert_Click"></asp:Button>
													<asp:Button ID="btnDeleteBottom" runat="server" Text="  削除する  " OnClientClick="return confirm('削除します。よろしいですか？');" Visible="false" OnClick="btnDelete_Click" />
													<asp:Button id="btnInsertUpdateBottomRegist" runat="server" Visible="true" Text="  登録する  " OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnInsertUpdateBottomUpdate" runat="server" Visible="false" Text="  更新する  " OnClick="btnInsertUpdate_Click" />
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
	<!--△ 登録 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>