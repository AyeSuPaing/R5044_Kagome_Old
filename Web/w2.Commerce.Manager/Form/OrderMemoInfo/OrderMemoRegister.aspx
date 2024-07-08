<%--
=========================================================================================================
  Module      : 注文メモ情報登録／編集ページ(OrderMemoRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderMemoRegister.aspx.cs" Inherits="Form_OrderMemoInfo_OrderMemoRegister" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr><td><h1 class="page-title">注文メモ設定</h1></td></tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td>
			<h1 class="cmn-hed-h2">注文メモ設定編集</h1>
		</td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td>
			<h1 class="cmn-hed-h2">注文メモ設定登録</h1>
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
												<div class="action_part_top"><input onclick="Javascript:history.back();" type="button" value="  戻る  "/>
													<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr id="trOrderMemoRegist" runat="server" visible="false">
															<td class="edit_title_bg" align="left" width="20%">注文メモID<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="1"><asp:TextBox id="tbOrderMemoId" runat="server" Text="<%# GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID) %>" MaxLength="10" Width="70"></asp:TextBox></td>
														</tr>
														<tr id="trOrderMemoEdit" runat="server" visible="false">
															<td class="edit_title_bg" align="left" width="20%">注文メモID</td>
															<td class="edit_item_bg" align="left" colspan="1"><asp:Literal id="lOrderMemoId" runat="server" Text="<%# WebSanitizer.HtmlEncode(GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID)) %>" ></asp:Literal></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">注文メモ名称<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="1"><asp:TextBox id="tbOrderMemoName" runat="server" Text="<%# GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_NAME) %>" MaxLength="100" Width="250"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">入力項目縦幅</td>
															<td class="edit_item_bg" align="left" colspan="1"><asp:TextBox id="tbHeight" runat="server" Text="<%# GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_HEIGHT) %>" MaxLength="9" Width="50"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">入力項目横幅</td>
															<td class="edit_item_bg" align="left" colspan="1"><asp:TextBox id="tbWidth" runat="server" Text="<%# GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_WIDTH) %>" MaxLength="9" Width="50"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">Css&nbsp;Class</td>
															<td class="edit_item_bg" align="left" colspan="1"><asp:TextBox id="tbCssClass" runat="server" Text="<%# GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_CSS_CLASS) %>" MaxLength="100" Width="250"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">最大入力可能文字数<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="1"><asp:TextBox id="tbMaxLength" runat="server" Text="<%# GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_MAX_LENGTH) %>" MaxLength="9" Width="50"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">デフォルトテキスト</td>
															<td class="edit_item_bg" align="left" colspan="1"><asp:TextBox id="tbDefaultText" runat="server" Text="<%# GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_DEFAULT_TEXT) %>" Width="590" Height="320" TextMode="MultiLine"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">有効期間<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="1">
																<uc:DateTimePickerPeriodInput id="ucDisplayPeriod" runat="server" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">表示順<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="1">
																<asp:TextBox ID="tbDisplayOrder" TextMode="SingleLine" Rows="3" runat="server" Text="<%# GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_DISPLAY_ORDER) %>" Width="50" MaxLength="2"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">有効フラグ</td>
															<td class="edit_item_bg" align="left" colspan="1">
																<asp:CheckBox ID="cbValidFlg" runat="server" Checked="<%# (string)GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_VALID_FLG) != Constants.FLG_ORDER_MEMO_SETTING_VALID_FLG_INVALID %>"></asp:CheckBox></td>
														</tr>
													</tbody>
												</table>
												<div class="action_part_bottom"><input onclick="Javascript:history.back();" type="button" value="  戻る  "/>
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
	</tr>
</table>
</asp:Content>