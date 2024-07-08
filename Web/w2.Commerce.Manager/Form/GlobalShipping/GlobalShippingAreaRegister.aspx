<%--
=========================================================================================================
  Module      : 海外配送エリア登録ページ(GlobalShippingAreaRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.DeliveryCompany" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="GlobalShippingAreaRegister.aspx.cs" Inherits="Form_Shipping_ShippingRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="w2cm" Assembly="w2.App.Common" Namespace="w2.App.Common.Web.WebCustomControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">配送地域設定</h1></td>
	</tr>
	<tr>
		<td style="width: 797px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEditTop" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送地域設定編集</h2></td>
	</tr>
	<tr id="trRegisterTop" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送地域設定登録</h2></td>
	</tr>
	<tr>
		<td style="width: 797px">
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top"><input onclick="Javascript:history.back();" type="button" value="  戻る  " />
												<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" /></div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">配送エリアID<span class="notice">*</span></td>
															<td id="tdGlobalShippingAreaIdEdit" class="edit_item_bg" align="left" runat="server" visible="false">
																<asp:TextBox ID="tbId" runat="server" MaxLength="10"></asp:TextBox>
															</td>
															<td id="tdShippingIdView" class="edit_item_bg" align="left" runat="server" visible="false">
																<asp:Literal ID="litId" runat="server" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">配送エリア名<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbName" runat="server" Width="250" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">表示順<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbSortNo" runat="server" Width="60" MaxLength="3"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">有効フラグ<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:CheckBox runat="server" ID="chkValidFlg"></asp:CheckBox></td>
														</tr>
													</tbody>
												</table>
												<br />
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
													</tr>
												</table>
												<div class="action_part_bottom"></div>
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
	<tr>
		<td style="width: 797px; height: 10px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ 登録 △-->
	<tr>
		<td style="width: 797px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
