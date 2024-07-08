<%--
=========================================================================================================
  Module      : シリアルキー情報登録ページ(SerialKeyRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SerialKeyRegister.aspx.cs" Inherits="Form_SerialKey_SerialKeyRegister" maintainScrollPositionOnPostBack="true" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>

<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
<meta http-equiv="Pragma" content="no-cache" />
<meta http-equiv="cache-control" content="no-cache" />
<meta http-equiv="expires" content="0" />
</asp:Content>
<asp:Content id="Content1" ContentPlaceHolderid="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">シリアルキー情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">シリアルキー情報編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">シリアルキー情報登録</h2></td>
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
												<div class="action_part_top">
													<asp:Button id="btnBackListTop" runat="server" Text="  一覧へ戻る  " Visible="False" OnClick="btnBackList_Click" />
													<asp:Button id="btnBackTop" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
												</div>
											</td>
										</tr>
										<tr>
											<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!-- ▽シリアルキー▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr id="trSerialKeyEdit" runat="server" Visible="False">
															<td class="edit_title_bg" align="left" width="200">シリアルキー</td>
															<td class="edit_item_bg" align="left"><asp:Label id="lbSerialKey" Text="<%# WebSanitizer.HtmlEncode(SerialKeyUtility.DecryptSerialKey((string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_SERIAL_KEY))) %>" runat="server"></asp:Label></td>
														</tr>
														<tr id="trSerialKeyRegister" runat="server" Visible="False">
															<td class="edit_title_bg" align="left" width="200">シリアルキー<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbSerialKey" runat="server" Text="<%# SerialKeyUtility.DecryptSerialKey((string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_SERIAL_KEY)) %>" Width="200" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="200">商品ID<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbProductId" runat="server" Text="<%# GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_PRODUCT_ID) %>" Width="200" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="200">バリエーションID</td>
															<td class="edit_item_bg" align="left">商品ID + <asp:TextBox id="tbVariationId" runat="server" Text="<%# GetKeyValue(this.SerialKeyMaster, Constants.FIELD_PRODUCTVARIATION_V_ID) %>" Width="200" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="200">状態</td>
															<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlStatus" runat="server" SelectedValue="<%# StringUtility.ToEmpty(GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_STATUS))%>" AutoPostBack="True" OnSelectedIndexChanged="ddlStatus_OnSelectedIndexChanged"></asp:DropDownList>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="200">注文ID</td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbOrderId" runat="server" Text="<%# GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_ORDER_ID) %>" Width="200" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="200">注文商品枝番</td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbOrderItemNo" runat="server" Text="<%# GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_ORDER_ITEM_NO) %>" Width="80" MaxLength="2"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="200">ユーザーID</td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbUserId" runat="server" Text="<%# GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_USER_ID) %>" Width="200" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="200">引渡日</td>
															<td class="edit_item_bg" align="left">
																<uc:DateTimePickerPeriodInput id="ucDisplayPeriod" CanShowEndDatePicker="False" runat="server" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="200">有効フラグ</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox id="cbValidFlg" runat="server" Checked="<%# (string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_VALID_FLG) != Constants.FLG_SERIALKEY_VALID_FLG_INVALID %>" Text="有効"></asp:CheckBox></td>
														</tr>
													</tbody>
												</table>
												<!-- △シリアルキー△ -->
												<!-- ▽BOTTOM▽ -->
												<div class="action_part_bottom">
													<asp:Button id="btnBackListBottom" runat="server" Text="  一覧へ戻る  " Visible="False" OnClick="btnBackList_Click" />
													<asp:Button id="btnBackBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
												</div>
												<!-- △BOTTOM△ -->
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