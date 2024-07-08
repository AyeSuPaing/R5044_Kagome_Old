<%--
=========================================================================================================
  Module      : 商品情報登録ページ(ProductRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductRegister.aspx.cs" Inherits="Form_Product_ProductRegister" %>

<%@ Import Namespace="w2.App.Common.Option" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderHead" runat="Server">
	<meta http-equiv="Pragma" content="no-cache" />
	<meta http-equiv="cache-control" content="no-cache" />
	<meta http-equiv="expires" content="0" />
<style type="text/css">
		.default_setting {
	}

		.default_setting_item {
			width: 10%;
		}

		.default_setting_noaction {
			display: none;
	}

		table {
			table-layout: unset;
	}

		.notice {
			font-size: 14px;
		}
	</style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
	<asp:HiddenField ID="hfScrollPosition" runat="server" />
<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr id="trTitleProductTop" runat="server"></tr>
	<tr id="trTitleProductMiddle" runat="server">
			<td>
				<h1 class="page-title">商品情報</h1>
			</td>
	</tr>
	<tr id="trTitleProductBottom" runat="server">
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
	</tr>
		<!-- ▽登録▽ -->
		<tr>
			<td>
				<h2 class="cmn-hed-h2">商品初期設定</h2>
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
													<div id="divDisplayCompleteMessage" runat="server" class="action_part_top" visible="False">
												<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
													<tr class="info_item_bg">
																<td align="left">商品画面の初期設定情報を登録/更新しました。</td>
													</tr>
												</table>
												</div>
												<div class="action_part_top">
													<asp:Button id="btnBackListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnBackList_Click" />
													<asp:Button id="btnUpdateDefaultSettingTop" runat="server" Text="  更新する  " OnClick="btnUpdateDefaultSetting_Click" />
												</div>
													<div style="margin-top: 10px; float: left;">
													<a href="#anchorVariation">&nbsp;▼商品バリエーション&nbsp;</a>&nbsp;
												</div>
											</td>
										</tr>
										<tr>
											<td>
													<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">設定項目説明
															<table class="info_table" cellspacing="1" cellpadding="3" width="742" border="0">
																<tr class="info_item_bg">
																	<td class="edit_title_bg" align="left" width="50">表示</td>
																	<td align="left">
																		商品項目を非表示にする場合はチェックを外します。
																	</td>
																</tr>
																<tr class="info_item_bg">
																	<td class="edit_title_bg" align="left">項目メモ</td>
																	<td align="left">
																		商品項目の説明を設定することが出来ます。
																	</td>
																</tr>
																<tr class="info_item_bg">
																	<td class="edit_title_bg" align="left">初期値</td>
																	<td align="left">
																		新規登録（マスタアップロード含む）する際に既定値として設定される値を設定します。<br />
																		※初期値を利用する場合はチェックを付けます。
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
												<td colspan="6">
													<img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" />
												</td>
										</tr>
										<tr>
											<td>
													<asp:UpdatePanel ID="upProductMain" runat="server">
														<ContentTemplate>
												<%-- ▽商品エラーメッセージ表示▽ --%>
												<asp:UpdatePanel ID="upProductErrorMessages" runat="server">
													<ContentTemplate>
														<table id="tblProductErrorMessages" class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server" visible="false">
															<tbody>
															<tr>
																				<td class="edit_title_bg" align="center" colspan="5">エラーメッセージ</td>
															</tr>
															<tr>
																				<td class="edit_item_bg" align="left" colspan="5" style="border-bottom: none;">
																	<asp:Label ID="lbProductErrorMessages" runat="server" ForeColor="red" />
																</td>
															</tr>
															</tbody>
														</table>
													</ContentTemplate>
												</asp:UpdatePanel>
												<%-- △商品エラーメッセージ表示△ --%>
												<!-- ▽商品▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
																		<td class="edit_title_bg" align="center" colspan="5">基本情報</td>
														</tr>
														<tr class="default_setting">
															<td class="edit_title_bg default_setting" align="center" width="8%">表示</td>
															<td class="edit_title_bg default_setting" align="center" width="21%">項目名</td>
															<td class="edit_title_bg default_setting" align="center" width="11%">項目メモ</td>
																		<td class="edit_title_bg default_setting" align="center" width="50%" colspan="2">初期値</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbProductIdDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PRODUCT_ID) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品ID<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbProductIdDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_ID) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbProductIdHasDefault" runat="server" Checked="false" Enabled="false" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbProductId" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_PRODUCT_ID) %>" Width="200" MaxLength="30" Enabled="false" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center" style="min-width: 30px;">
																			<asp:CheckBox ID="cbNoteDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_NOTE) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">備考</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbNoteDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_NOTE) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center" style="min-width: 30px;">
																			<asp:CheckBox ID="cbNoteHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_NOTE) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbNote" TextMode="MultiLine" Width="420" Rows="2" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_NOTE) %>" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSupplierIdDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SUPPLIER_ID) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">サプライヤID</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbSupplierIdDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SUPPLIER_ID) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSupplierIdHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SUPPLIER_ID) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbSupplierId" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SUPPLIER_ID) %>" Width="200" MaxLength="20" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId1DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID1) %>" /></td>
																		<td class="edit_title_bg" align="left" width="200">商品連携ID1</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCooperationId1DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID1) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId1HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID1) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCooperationId1" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID1) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId2DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID2) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品連携ID2</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCooperationId2DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID2) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId2HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID2) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCooperationId2" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID2) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId3DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID3) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品連携ID3</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCooperationId3DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID3) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId3HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID3) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCooperationId3" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID3) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId4DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID4) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品連携ID4</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCooperationId4DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID4) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId4HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID4) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCooperationId4" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID4) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId5DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID5) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品連携ID5</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCooperationId5DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID5) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId5HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID5) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCooperationId5" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID5) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId6DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID6) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品連携ID6</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCooperationId6DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID6) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId6HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID6) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCooperationId6" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID6) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId7DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID7) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品連携ID7</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCooperationId7DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID7) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId7HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID7) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCooperationId7" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID7) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId8DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID8) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品連携ID8</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCooperationId8DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID8) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId8HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID8) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCooperationId8" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID8) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId9DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID9) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品連携ID9</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCooperationId9DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID9) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId9HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID9) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCooperationId9" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID9) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId10DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID10) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品連携ID10</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCooperationId10DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID10) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCooperationId10HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID10) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCooperationId10" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_COOPERATION_ID10) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbMallProductIdDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">モール拡張商品ID</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbMallProductIdDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbMallProductIdHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbMallProductId" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID) %>" Width="200" MaxLength="30" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cblMallExhibitsConfigDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">モール出品設定</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tblMallExhibitsConfigDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG) %>" />
																		</td>
															<td class="edit_item_bg default_setting" align="center"></td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBoxList ID="cblMallExhibitsConfig" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" Enabled="false" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbAndMallReservationFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">＆mall連携予約商品フラグ</td>
															<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbAndMallReservationFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbAndMallReservationFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbAndMallReservationFlg" runat="server" Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG) == Constants.FLG_PRODUCT_ANDMALL_RESERVATION_FLG_RESERVATION) %>" Text="有効" />
															</td>
														</tr>
														<% } %>
																	<% if (Constants.GOOGLESHOPPING_COOPERATION_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbGoogleShoppingFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">Googleショッピング連携</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbGoogleShoppingFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbGoogleShoppingFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbGoogleShoppingFlg" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG) != Constants.FLG_PRODUCT_GOOGLE_SHOPPING_FLG_INVALID %>" Text="有効" />
																			<br />
																			※Googleショッピングへ登録する商品の商品IDは、半角英数で登録して下さい。
																		</td>
														</tr>
														<% } %>
																	<% if (Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbUseRecommendFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">外部レコメンド利用</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbUseRecommendFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbUseRecommendFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbUseRecommendFlg" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG) != Constants.FLG_PRODUCT_USE_RECOMMEND_FLG_INVALID %>" Text="有効" />
																		</td>
														</tr>
														<% } %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbNameDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_NAME) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品名<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbNameDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_NAME) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbNameHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_NAME) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbName" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_NAME) %>" Width="300" MaxLength="100" />
																		</td>
														</tr>
																	<% if (this.IsOperationalCountryJp) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbNameKanaDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_NAME_KANA) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品名(カナ)</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbNameKanaDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_NAME_KANA) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbNameKanaHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_NAME_KANA) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbNameKana" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_NAME_KANA) %>" Width="300" MaxLength="100" />
																		</td>
														</tr>
														<% } %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSeoKeywordsDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SEO_KEYWORDS) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">SEOキーワード</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbSeoKeywordsDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SEO_KEYWORDS) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSeoKeywordsHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SEO_KEYWORDS) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbSeoKeywords" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SEO_KEYWORDS) %>" Width="300" MaxLength="200" />
																			<br />
																			デフォルトでは商品名/カテゴリがキーワードに設定されています。それ以外のキーワードを追加する場合に登録します。（","カンマ区切りで複数指定可。）
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCatchcopyDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATCHCOPY) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">キャッチコピー</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCatchcopyDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATCHCOPY) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCatchcopyHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATCHCOPY) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbCatchcopy" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATCHCOPY) %>" Width="300" MaxLength="60" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSearchKeywordDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SEARCH_KEYWORD) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">検索キーワード</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbSearchKeywordDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SEARCH_KEYWORD) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSearchKeywordHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SEARCH_KEYWORD) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbSearchKeyword" runat="server" TextMode="MultiLine" Rows="8" Width="420" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SEARCH_KEYWORD) %>" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbOutlineDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_OUTLINE) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品概要</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbOutlineDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_OUTLINE) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbOutlineHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_OUTLINE) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:RadioButtonList ID="rbOutlineFlg" runat="server" Width="150" RepeatDirection="Horizontal" RepeatLayout="Flow" SelectedValue='<%# GetEditModeByDefaultSettingFieldValue(Constants.FIELD_PRODUCT_OUTLINE_KBN) %>' CssClass="radio_button_list">
																	<asp:ListItem Value="0">TEXT</asp:ListItem>
																	<asp:ListItem Value="1">HTML</asp:ListItem>
																</asp:RadioButtonList>
																			<input type="button" onclick="javascript:open_wysiwyg('<%= tbOutline.ClientID %>', '<%= rbOutlineFlg.ClientID %>');" value="  HTMLエディタ  " />
																			<br />
																			<asp:TextBox ID="tbOutline" TextMode="MultiLine" Rows="4" Width="420" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_OUTLINE) %>" runat="server" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDescDetail1DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL1) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品詳細説明1</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDescDetail1DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL1) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDescDetail1HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL1) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:RadioButtonList ID="rbDescDetailFlg1" runat="server" Width="150" RepeatDirection="Horizontal" RepeatLayout="Flow" SelectedValue='<%# GetEditModeByDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1) %>' CssClass="radio_button_list">
																	<asp:ListItem Value="0">TEXT</asp:ListItem>
																	<asp:ListItem Value="1">HTML</asp:ListItem>
																</asp:RadioButtonList>
																			<input type="button" onclick="javascript:open_wysiwyg('<%= tbDescDetail1.ClientID %>', '<%= rbDescDetailFlg1.ClientID %>');" value="  HTMLエディタ  " />
																			<br />
																			<asp:TextBox ID="tbDescDetail1" TextMode="MultiLine" Rows="8" Width="420" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL1) %>" runat="server" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDescDetail2DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL2) %>" /></td>
																		<td class="edit_title_bg" align="left" width="200">商品詳細説明2</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDescDetail2DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL2) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDescDetail2HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL2) %>" /></td>
															<td class="edit_item_bg" align="left">
																			<asp:RadioButtonList ID="rbDescDetailFlg2" runat="server" Width="150" RepeatDirection="Horizontal" RepeatLayout="Flow" SelectedValue='<%# GetEditModeByDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2) %>' CssClass="radio_button_list">
																	<asp:ListItem Value="0">TEXT</asp:ListItem>
																	<asp:ListItem Value="1">HTML</asp:ListItem>
																</asp:RadioButtonList>
																			<input type="button" onclick="javascript:open_wysiwyg('<%= tbDescDetail2.ClientID %>', '<%= rbDescDetailFlg2.ClientID %>');" value="  HTMLエディタ  " /><br />
																			<asp:TextBox ID="tbDescDetail2" TextMode="MultiLine" Rows="8" Width="420" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL2) %>" runat="server" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDescDetail3DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL3) %>" /></td>
																		<td class="edit_title_bg" align="left" width="200">商品詳細説明3</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDescDetail3DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL3) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDescDetail3HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL3) %>" /></td>
															<td class="edit_item_bg" align="left">
																			<asp:RadioButtonList ID="rbDescDetailFlg3" runat="server" Width="150" RepeatDirection="Horizontal" RepeatLayout="Flow" SelectedValue='<%# GetEditModeByDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3) %>' CssClass="radio_button_list">
																	<asp:ListItem Value="0">TEXT</asp:ListItem>
																	<asp:ListItem Value="1">HTML</asp:ListItem>
																</asp:RadioButtonList>
																			<input type="button" onclick="javascript:open_wysiwyg('<%= tbDescDetail3.ClientID %>', '<%= rbDescDetailFlg3.ClientID %>');" value="  HTMLエディタ  " /><br />
																			<asp:TextBox ID="tbDescDetail3" TextMode="MultiLine" Rows="8" Width="420" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL3) %>" runat="server" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDescDetail4DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL4) %>" /></td>
																		<td class="edit_title_bg" align="left" width="200">商品詳細説明4</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDescDetail4DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL4) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDescDetail4HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL4) %>" /></td>
															<td class="edit_item_bg" align="left">
																			<asp:RadioButtonList ID="rbDescDetailFlg4" runat="server" Width="150" RepeatDirection="Horizontal" RepeatLayout="Flow" SelectedValue='<%# GetEditModeByDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4) %>' CssClass="radio_button_list">
																	<asp:ListItem Value="0">TEXT</asp:ListItem>
																	<asp:ListItem Value="1">HTML</asp:ListItem>
																</asp:RadioButtonList>
																			<input type="button" onclick="javascript:open_wysiwyg('<%= tbDescDetail4.ClientID %>', '<%= rbDescDetailFlg4.ClientID %>');" value="  HTMLエディタ  " /><br />
																			<asp:TextBox ID="tbDescDetail4" TextMode="MultiLine" Rows="8" Width="420" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DESC_DETAIL4) %>" runat="server" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbReturnExchangeMessageDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE) %>" /></td>
																		<td class="edit_title_bg" align="left" width="200">返品・交換・解約説明(TEXT)</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbReturnExchangeMessageDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbReturnExchangeMessageHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE) %>" /></td>
															<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbReturnExchangeMessage" TextMode="MultiLine" Rows="3" Width="420" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE) %>" runat="server" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbUrlDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_URL) %>" /></td>
																		<td class="edit_title_bg" align="left" width="200">紹介URL</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbUrlDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_URL) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbUrlHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_URL) %>" /></td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbUrl" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_URL) %>" Width="300" MaxLength="256" /></td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbInquiteEmailDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_INQUIRE_EMAIL) %>" /></td>
																		<td class="edit_title_bg" align="left" width="200">問い合わせ用メールアドレス</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbInquiteEmailDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_INQUIRE_EMAIL) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbInquiteEmailHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_INQUIRE_EMAIL) %>" /></td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbInquiteEmail" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_INQUIRE_EMAIL) %>" Width="300" MaxLength="256" /></td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbInquiteTelDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_INQUIRE_TEL) %>" /></td>
																		<td class="edit_title_bg" align="left" width="200">問い合わせ用電話番号</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbInquiteTelDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_INQUIRE_TEL) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbInquiteTelHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_INQUIRE_TEL) %>" /></td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbInquiteTel" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_INQUIRE_TEL) %>" Width="150" MaxLength="30" /></td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbTaxIncludeDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG) %>" /></td>
																		<td class="edit_title_bg" align="left" width="200">税区分</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbTaxIncludeDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbTaxIncludeHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG) %>" /></td>
																		<td class="edit_item_bg" align="left"><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG, TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag()) %></td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbTaxCategoryDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID) %>" Enabled="False" /></td>
																		<td class="edit_title_bg" align="left" width="200">商品税率カテゴリ<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbTaxCategoryDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbTaxCategoryHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID) %>" /></td>
															<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlTaxCategory" runat="server" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbPriceDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_PRICE) %>" Enabled="False" /></td>
																		<td class="edit_title_bg" align="left" width="200">商品表示価格<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbPriceDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_PRICE) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbPriceHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_PRICE) %>" /></td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbPrice" runat="server" MaxLength="7" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_PRICE).ToPriceString() %>" Width="100" /></td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSpecialPriceDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE) %>" /></td>
																		<td class="edit_title_bg" align="left" width="200">商品表示特別価格</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbSpecialPriceDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSpecialPriceHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE) %>" /></td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbSpecialPrice" runat="server" MaxLength="7" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE).ToPriceString() %>" Width="100" /></td>
														</tr>
														<% if (Constants.PRODUCT_ORDER_LIMIT_ENABLED) { %>
																	<tr>
															<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCheckProductOrderLimitDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG) %>" />
															</td>
																		<td class="edit_title_bg" align="left" width="200">通常商品重複購入制限フラグ</td>
															<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCheckProductOrderLimitDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG) %>" />
															</td>
															<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCheckProductOrderLimitHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG) %>" />
															</td>
															<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbCheckProductOrderLimit" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG) == Constants.FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_VALID %>" Text="有効" />
															</td>
														</tr>
														<% } %>
																	<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED && Constants.PRODUCT_ORDER_LIMIT_ENABLED) { %>
																	<tr>
															<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCheckFixedProductOrderLimitDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG) %>" />
															</td>
																		<td class="edit_title_bg" align="left" width="200">定期商品重複購入制限フラグ</td>
															<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbCheckFixedProductOrderLimitDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG) %>" />
															</td>
															<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCheckFixedProductOrderLimitHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG) %>" />
															</td>
															<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbCheckFixedProductOrderLimit" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG) == Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_VALID %>" Text="有効" />
															</td>
														</tr>
														<% } %>
																	<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
																	<tr>
															<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchaseFirsttimePriceDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE) %>" />
															</td>
																		<td class="edit_title_bg" align="left" width="200">定期購入初回価格</td>
															<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbFixedPurchaseFirsttimePriceDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE) %>" />
															</td>
															<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchaseFirsttimePriceHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE) %>" />
															</td>
															<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbFixedPurchaseFirsttimePrice" runat="server" MaxLength="7" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE).ToPriceString() %>" Width="100" />
															</td>
														</tr>
																	<tr>
															<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchasePriceDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE) %>" />
															</td>
																		<td class="edit_title_bg" align="left" width="200">定期購入価格</td>
															<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbFixedPurchasePriceDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE) %>" />
															</td>
															<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchasePriceHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE) %>" />
															</td>
															<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbFixedPurchasePrice" runat="server" MaxLength="7" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE).ToPriceString() %>" Width="100" />
															</td>
														</tr>
														<% } %>
																	<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbMemberRankPriceDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="300px">商品会員ランク価格</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbMemberRankPriceDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE) %>" />
																		</td>
															<td class="edit_item_bg default_setting" align="center"></td>
																		<td class="edit_item_bg" align="left"></td>
																	</tr>
																	<% } %>
																	<tr id="trPoint" runat="server" visible="<%# Constants.W2MP_POINT_OPTION_ENABLED %>">
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbPointDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_POINT1) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">ポイント<span class="notice">*</span>
																			<br />
																			（基本ルール購入時商品毎発行）
																		</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbPointDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_POINT1) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbPointHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_POINT1) %>" />
															</td>
															<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbPoint" runat="server" Text='<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_POINT1) %>' Width="100" MaxLength="3" />
																			<asp:DropDownList ID="ddlPointKbn" runat="server" />
																			<br />
																			<% if(Constants.MEMBER_RANK_OPTION_ENABLED) { %>
																			<asp:CheckBox ID="cbMemberRankPointExcludeDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG) == Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_VALID %>" AutoPostBack="true" Text="会員ランクのポイント加算を適用しない" />
																			<br />
																			<% } %>
																			<asp:CheckBox ID="cbShowFixedPurchasePointSetting" runat="server" Checked="<%# IsShowFixedPurchasePointSetting(Constants.FIELD_PRODUCT_POINT_KBN2) %>" AutoPostBack="true" Text="定期購入ポイント設定する" />
																			<br />
																<% if (cbShowFixedPurchasePointSetting.Checked) { %>
																			<asp:TextBox ID="tbIncFixedPurchasePoint" runat="server" Text='<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_POINT2) %>' Width="100" MaxLength="3" />
																			<asp:DropDownList ID="ddlFixedPurchasePointKbn" runat="server" />
																<% } %>
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbShippingTypeDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SHIPPING_TYPE) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">配送種別<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbShippingTypeDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SHIPPING_TYPE) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbShippingTypeHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SHIPPING_TYPE) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlShippingType" runat="server" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbShippingSizeKbnDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">配送サイズ区分<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbShippingSizeKbnDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbShippingSizeKbnHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlShippingSizeKbn" runat="server" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCProductSizeFactorDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR) %>" Enabled="false" /></td>
																		<td class="edit_title_bg" align="left" width="200">商品サイズ係数<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbProductSizeFactorDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR) %>" /></td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbCProductSizeFactorHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR) %>" /></td>
																<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbProductSizeFactor" runat="server" MaxLength="9" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR) %>" />
															</td>
														</tr>
																	<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbProductWeightGramDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品重量（g）<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbProductWeightGramDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbProductWeightGramHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbProductWeightGram" runat="server" MaxLength="7" Text='<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM) %>' />
															</td>
														</tr>
														<% } %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbColorImageDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">カラーイメージ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbColorImageDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbColorImageHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:DropDownList runat="server" ID="ddlColorImage" DataSource='<%# GetProductColorListItem() %>' SelectedValue="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID) %>" DataTextField="text" DataValueField="value" />
																			<img id="iColorImage" src='<%: ProductColorUtility.GetColorImageUrl((string)ddlColorImage.SelectedValue) %>' style="vertical-align: middle;" height="25" width="25" alt="" />
																			<br />
																※カラーを追加・変更したい場合は「<%: Constants.PATH_IMAGES_COLOR %>」に画像ファイルを設置したうえで、「<%: Constants.FILEPATH_XML_COLORS %>」に設定を追加してください。<br />
																			フォーマット：&lt;ProductColor id="重複しない任意のID" filename="カラーの画像ファイル名" dispname="カラーの表示名"&gt;&lt;/ProductColor&gt;
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbPluralShippingPriceFreeDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">配送料複数個無料</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbPluralShippingPriceFreeDeflultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbPluralShippingPriceFreeHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbPluralShippingPriceFree" runat="server" Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG) != Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_INVALID) %>" Text="有効" />
																		</td>
														</tr>
																	<% if (Constants.FREE_SHIPPING_FEE_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox
																			ID="cbExcludeFreeShippingDefaultSetting"
																			runat="server"
																			Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">配送料無料適用外</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox
																				ID="tbExcludeFreeShippingDefaultSetting"
																				runat="server"
																				Width="100"
																				MaxLength="50"
																				Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox
																				ID="cbExcludeFreeShippingHasDefault"
																				runat="server"
																				Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox
																				ID="cbExcludeFreeShipping" runat="server"
																				Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG)
																					!= Constants.FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_INVALID) %>"
																				Text="有効" />
																		</td>
																	</tr>
																	<% } %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbLimitedPaymentDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">利用不可決済</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbLimitedPaymentDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox runat="server" ID="cbLimitedPaymentHasDefault" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS) %>" />
																		</td>
															<td class="edit_item_bg">
																			<asp:CheckBoxList ID="cblLimitedPayment" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" DataTextField="Key" DataValueField="Value" RepeatColumns="4" />
																<div class="edit_item_bg">
																				<br />
																				利用不可決済種別は管理画面上、決済種別情報にて有効な決済種別が表示されます。
																				<br>
																				実際のフロントサイトへの表示制限は配送種別情報での決済種別設定、および、決済種別情報の利用不可ユーザー管理レベルでの制限を考慮した上で表示されますのでご注意ください。
																</div>
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDisplayDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_TO) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">表示期間<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDisplayDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_TO) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDisplayHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_TO) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																<uc:DateTimePickerPeriodInput ID="ucDisplay" runat="server" IsNullEndDateTime="true" />
																	&nbsp;(<a href="Javascript:dateEndClear('displayToDate');">終了日時を指定しない</a>)
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSellDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SELL_TO) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">販売期間<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbSellDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELL_TO) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSellHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SELL_TO) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<uc:DateTimePickerPeriodInput ID="ucSell" runat="server" IsNullEndDateTime="true" />
																	&nbsp;(<a href="Javascript:dateEndClear('sellToDate');">終了日時を指定しない</a>)
																			<br />
																			<asp:CheckBox ID="cbDisplaySellFlg" runat="server" Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG) == Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_DISP) %>" Text="販売期間をフロントサイトに表示する" />
																</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDisplayPriorityDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">表示優先順<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbisplayPriorityDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDisplayPriorityHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbDisplayPriority" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY) %>" Width="100" />
																			<br />
																			フロントサイトの新着順並び替えの際、表示優先度順降順＋登録日（商品情報作成日）降順で並び替えされます。
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbMaxSellQuantityDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">1注文購入限度数<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbMaxSellQuantityDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbMaxSellQuantityHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbMaxSellQuantity" runat="server" MaxLength="3" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY) %>" Width="50" />
															</td>
														</tr>
																	<tr id="trProductStockMaagementKbn" runat="server" visible="<%# Constants.PRODUCT_STOCK_OPTION_ENABLE %>">
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbStockManagementKbnDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">在庫管理方法</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbStockManagementKbnDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbStockManagementKbnHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlStockManagementKbn" runat="server" SelectedValue='<%# GetStockManagementKbnSetting(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) %>' />
															</td>
														</tr>
																	<tr id="trProductStockMessage" runat="server" visible="<%# Constants.PRODUCT_STOCK_OPTION_ENABLE %>">
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbStockMessageIdDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品在庫文言</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbStockMessageIdDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbStockMessageIdHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlStockMessageId" runat="server" />
															</td>
														</tr>
																	<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
																	<tr>
															<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchaseFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">定期購入フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbFixedPurchaseFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchaseFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlFixedPurchaseFlg" runat="server"
																				SelectedValue='<%# GetFixedPurchaseFlgSetting(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) %>' />
															</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbLimitedFixedPurchaseKbn1SettingDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING) %>" />
															</td>
																		<td class="edit_title_bg" align="left" width="200">定期購入配送間隔月利用不可</td>
															<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbLimitedFixedPurchaseKbn1SettingDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING) %>" />
															</td>
															<td class="edit_item_bg default_setting" align="center"></td>
															<td class="edit_item_bg">
																			<asp:CheckBoxList ID="cblLimitedFixedPurchaseKbn1Setting" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" DataTextField="Key" DataValueField="Value" />
															</td>
														</tr>
																	<tr>
															<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox id="cbLimitedFixedPurchaseKbn4SettingDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING) %>" />
															</td>
																		<td class="edit_title_bg" align="left" width="200">定期購入配送間隔週利用不可</td>
															<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox id="tbLimitedFixedPurchaseKbn4SettingDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%#: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING) %>" />
															</td>
															<td class="edit_item_bg default_setting" align="center"></td>
															<td class="edit_item_bg">
																<asp:CheckBoxList ID="cblLimitedFixedPurchaseKbn4Setting" runat="server"  RepeatDirection="Horizontal" RepeatLayout="Flow" DataTextField="Key" DataValueField="Value" />
															</td>
														</tr>
																	<tr>
															<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbLimitedFixedPurchaseKbn3SettingDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING) %>" />
															</td>
																		<td class="edit_title_bg" align="left" width="200">定期購入配送間隔利用不可</td>
															<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbLimitedFixedPurchaseKbn3SettingDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING) %>" />
															</td>
															<td class="edit_item_bg default_setting" align="center"></td>
															<td class="edit_item_bg">
																			<asp:CheckBoxList ID="cblLimitedFixedPurchaseKbn3Setting" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" DataTextField="Key" DataValueField="Value" />
															</td>
														</tr>
																	<tr>
															<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchaseCancelableCountDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">定期購入解約可能回数（出荷基準）</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbFixedPurchaseCancelableCountDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchaseCancelableCountHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbFixedPurchaseCancelableCount" runat="server" MaxLength="2" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT) %>" Width="30" />
																			回
															</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchaseLimitedSkippedCountDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT) %>" />
															</td>
																		<td class="edit_title_bg" align="left" width="200">定期購入スキップ制限回数</td>
															<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbFixedPurchaseLimitedSkippedCountDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT) %>" />
															</td>
															<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchaseLimitedSkippedCountHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT) %>" />
															</td>
															<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbFixedPurchaseLimitedSkippedCount" runat="server" MaxLength="3" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT) %>" Width="30" />
																			回
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbLimitedUserLevelDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">定期購入利用不可ユーザー管理レベル</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbLimitedUserLevelDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox runat="server" ID="cbLimitedUserLevelHasDefault" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS) %>" />
																		</td>
																		<td class="edit_item_bg">
																			<asp:CheckBoxList ID="cblLimitedUserLevel" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
															</td>
														</tr>
															<!--▽定期購入割引▽ -->
																	<% if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbFixedPurchaseNextShippingSettingDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">定期購入2回目以降商品</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbFixedPurchaseNextShippingSettingDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING) %>" />
																		</td>
															<td class="edit_item_bg default_setting" align="center"></td>
																		<td class="edit_item_bg"></td>
																	</tr>
																	<% } %>
																	<!-- △定期購入割引△ -->
																	<!-- ▽頒布会フラグ▽ -->
																	<%if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDistributionFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">頒布会フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDistributionFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDistributionFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:DropDownList
																				ID="ddlSubscriptionBoxFlg"
																				runat="server"
																				SelectedValue='<%# GetSubscriptionBoxFlgSetting(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG) %>' />
																		</td>
																</tr>
																	<% } %>
																	<!-- △頒布会フラグ△ -->
																	<% } %>
																	<% if (Constants.GIFTORDER_OPTION_ENABLED) { %>
																			<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbGiftFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_GIFT_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">ギフト購入フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbGiftFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_GIFT_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbGiftFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_GIFT_FLG) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlGiftFlg" runat="server" SelectedValue='<%# GetGiftFlgSetting(Constants.FIELD_PRODUCT_GIFT_FLG) %>' />
																					</td>
																			</tr>
																	<% } %>
																	<% if (Constants.USERPRODUCTARRIVALMAIL_OPTION_ENABLED) { %>
																			<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbArrivalMailValidFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">再入荷通知メール有効フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbArrivalMailValidFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG) %>" />
																				</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbArrivalMailValidFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG) %>" />
																				</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbArrivalMailValidFlg" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG) == Constants.FLG_PRODUCT_ARRIVAL_MAIL_VALID_FLG_VALID %>" Text="有効" />
																				</td>
																			</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbReleaseMailValidFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">販売開始通知メール有効フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbReleaseMailValidFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG) %>" />
																				</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbReleaseMailValidFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG) %>" />
																				</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbReleaseMailValidFlg" runat="server" Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG) == Constants.FLG_PRODUCT_RELEASE_MAIL_VALID_FLG_VALID) %>" Text="有効" />
															</td>
														</tr>
														<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbResaleMailValidFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">再販売通知メール有効フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbResaleMailValidFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbResaleMailValidFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbResaleMailValidFlg" runat="server" Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG) == Constants.FLG_PRODUCT_RESALE_MAIL_VALID_FLG_VALID) %>" Text="有効" />
														</td>
														</tr>
																	<% } %>
																	<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
																	<tr>
															<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbMemberRankDiscountFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">会員ランク割引対象</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbMemberRankDiscountFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG) %>" />
															</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbMemberRankDiscountFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG) %>" />
															</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbMemberRankDiscountFlg" runat="server" Text="有効" />
															</td>
																				</tr>
																				<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDisplayMemberRankDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">閲覧可能会員ランク</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDisplayMemberRankDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDisplayMemberRankHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlDisplayMemberRank" runat="server" Width="300" />
																					</td>
																				</tr>
																		<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDisplayFixedPurchaseMemberLimitDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG) %>" />
																			</td>
																		<td class="edit_title_bg" align="left" width="200">定期会員限定フラグ（閲覧）</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDisplayFixedPurchaseMemberLimitDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG) %>" />
																			</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDisplayFixedPurchaseMemberLimitHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG) %>" />
																			</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbDisplayFixedPurchaseMemberLimit" runat="server" Text="有効" />
																			</td>
																		</tr>
																		<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbBuyableMemberRankDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">販売可能会員ランク</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbBuyableMemberRankDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK) %>" />
																			</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbBuyableMemberRankHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK) %>" />
																			</td>
																		<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlBuyableMemberRank" runat="server" Width="300" />
																			</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbBuyableFixedPurchaseMemberLimitDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG) %>" />
																			</td>
																		<td class="edit_title_bg" align="left" width="200">定期会員限定フラグ（購入）</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbBuyableFixedPurchaseMemberLimitDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG) %>" />
																			</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbBuyableFixedPurchaseMemberLimitHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG) %>" />
															</td>
															<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbBuyableFixedPurchaseMemberLimit" runat="server" Text="有効" />
															</td>
														</tr>
														<% } %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSelectVariationKbnDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品バリエーション選択方法</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbSelectVariationKbnDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbSelectVariationKbnHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlSelectVariationKbn" runat="server" SelectedValue='<%# GetSelectVariationKbnSetting(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN) %>' />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbAgeLimitFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">年齢制限フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbAgeLimitFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbAgeLimitFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbAgeLimitFlg" runat="server" Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG) == Constants.FLG_PRODUCT_AGE_LIMIT_FLG_VALID) %>" Text="有効" />
															</td>
														</tr>
																	<% if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDigitalContentsFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">デジタルコンテンツ商品フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDigitalContentsFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDigitalContentsFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbDigitalContentsFlg" runat="server" Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG) == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID) %>" Text="有効" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDownloadUrlDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DOWNLOAD_URL) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">ダウンロードURL</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDownloadUrlDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DOWNLOAD_URL) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDownloadUrlHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DOWNLOAD_URL) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbDownloadUrl" runat="server" MaxLength="1000" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DOWNLOAD_URL) %>" Width="300" />
															</td>
														</tr>
																	<% } %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbAddCartUrlLimitFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">カート投入URL制限フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbAddCartUrlLimitFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbAddCartUrlLimitFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbAddCartUrlLimitFlg" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG) == Constants.FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_VALID %>" Text="有効" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbValidFlgDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_VALID_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">有効フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbValidFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_VALID_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbValidFlgHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_VALID_FLG) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbValidFlg" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_VALID_FLG) != Constants.FLG_PRODUCT_VALID_FLG_INVALID %>" Text="有効" />
															</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDisplayKbnDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_KBN) %>" />
															</td>
																		<td class="edit_title_bg" align="left" width="200">商品表示区分</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbDisplayKbnDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_KBN) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbDisplayKbnHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_KBN) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																<table border="1">
																	<tr align="center">
																		<td width="50">区分</td>
																		<td width="80">商品一覧</td>
																		<td width="80">商品詳細</td>
																	</tr>
																	<tr align="center">
																					<td>
																						<asp:RadioButton ID="rbDisplayKbn1" runat="server" GroupName="rbDisplayKbn" Checked='<%# IsAllDisplayKbnSetting(Constants.FIELD_PRODUCT_DISPLAY_KBN) %>' />
																					</td>
																		<td>○</td>
																		<td>○</td>
																	</tr>
																	<tr align="center">
																					<td>
																						<asp:RadioButton ID="rbDisplayKbn2" runat="server" GroupName="rbDisplayKbn" Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_KBN) == Constants.FLG_PRODUCT_DISPLAY_DISP_ONLY_DETAIL) %>" />
																					</td>
																		<td>×</td>
																		<td>○</td>
																	</tr>
																	<tr align="center">
																					<td>
																						<asp:RadioButton ID="rbDisplayKbn3" runat="server" GroupName="rbDisplayKbn" Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_KBN) == Constants.FLG_PRODUCT_DISPLAY_UNDISP_ALL) %>" />
																					</td>
																		<td>×</td>
																		<td>×</td>
																	</tr>
																</table>
															</td>
														</tr>
														<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbBundleItemDisplayTypeDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">同梱商品明細表示フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbBundleItemDisplayTypeDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbBundleItemDisplayTypeHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbBundleItemDisplayType" runat="server" Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE) != Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_INVALID) %>" Text="有効" />
																		</td>
														</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbProductTypeDefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PRODUCT_TYPE) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">商品区分</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbProductTypeDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_TYPE) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbProductTypeHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_PRODUCT_TYPE) %>" />
																		</td>
															<td class="edit_item_bg" align="left">
																			<asp:RadioButtonList ID="rblProductType" runat="server" Width="200" RepeatDirection="Horizontal" RepeatLayout="Flow"
																				SelectedValue='<%# GetProductTypeSetting(Constants.FIELD_PRODUCT_PRODUCT_TYPE) %>'
																				CssClass="radio_button_list" />
															</td>
														</tr>
														<% } %>
														<% if (Constants.STORE_PICKUP_OPTION_ENABLED) { %>
														<tr>
															<td class="edit_title_bg default_setting" align="center">
																<asp:CheckBox
																	ID="cbStorePickupFlgDefaultSetting"
																	runat="server"
																	Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_STOREPICKUP_FLG) %>" />
															</td>
															<td class="edit_title_bg" align="left" width="200">店舗受取可能フラグ</td>
															<td class="edit_item_bg default_setting" align="left">
																<asp:TextBox
																	ID="tbStorePickupFlgDefaultSetting"
																	runat="server"
																	Width="100"
																	MaxLength="50"
																	Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOREPICKUP_FLG) %>" />
															</td>
															<td class="edit_item_bg default_setting" align="center">
																<asp:CheckBox
																	ID="cbStorePickupFlgHasDefault"
																	runat="server"
																	Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_STOREPICKUP_FLG) %>" />
															</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBox
																	ID="cbStorePickupFlg"
																	runat="server"
																	Text="有効"
																	Checked="<%# (GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_STOREPICKUP_FLG) != Constants.FLG_OFF) %>" />
															</td>
														</tr>
														<% } %>
													</tbody>
												</table>
												<!-- △商品△ -->
												<br />
												<%--- ▽商品付帯情報▽ ---%>
												<%--- 商品付帯情報（初期設定） ---%>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="5">商品付帯情報</td>
													</tr>
													<tr class="default_setting">
														<td class="edit_title_bg default_setting" align="center" width="42">表示</td>
														<td class="edit_title_bg default_setting" align="center" width="122">項目名</td>
														<td class="edit_title_bg default_setting" align="center" width="106">項目メモ</td>
														<td class="edit_title_bg default_setting" align="center" colspan="2" width="392">初期値</td>
													</tr>
													<asp:Repeater ID="rProductOptionValueDefaultSetting" OnItemDataBound="rProductOptionValueDefaultSetting_OnItemDataBound" runat="server">
														<ItemTemplate>
															<tr class="default_setting">
																<td class="edit_title_bg default_setting" align="center" width="42"><asp:CheckBox id="cbProductOptionValueDefaultSetting" runat="server" /></td>
																<td class="edit_title_bg" align="left" width="122">付帯情報&nbsp;<%# Container.ItemIndex + 1 %></td>
																<td class="edit_item_bg default_setting" align="left" width="106"><asp:TextBox id="tbProductOptionValueDefaultSetting" Width="100" MaxLength="50" runat="server" /></td>
																<td class="edit_item_bg default_setting" width="13" align="center"><asp:CheckBox id="cbProductOptionValueHasDefault" runat="server" /></td>
																<td class="edit_item_bg" align="left" width="389"><asp:TextBox id="tbProductOptionValueDefaultValue" Text="<%# Container.DataItem %>" Width="400" runat="server" /></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												<br />
															<%--- △商品付帯情報△ ---%>
															<%-- ▽商品に関連する情報エラーメッセージ表示▽ --%>
															<asp:UpdatePanel ID="upProductRelationErrorMessages" runat="server">
																<ContentTemplate>
																	<table id="tblProductRelationErrorMessages" class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server" visible="false">
																		<tbody>
													<tr>
																				<td class="edit_title_bg" align="center" colspan="5">エラーメッセージ</td>
													</tr>
																<tr>
																				<td class="edit_item_bg" align="left" colspan="5" style="border-bottom: none;">
																					<asp:Label ID="lbProductRelationErrorMessages" runat="server" ForeColor="red" />
																	</td>
																</tr>
																		</tbody>
																	</table>
																</ContentTemplate>
															</asp:UpdatePanel>
															<%-- △商品に関連する情報エラーメッセージ表示△ --%>
															<!-- ▽カテゴリ▽ -->
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<% if (Constants.PRODUCT_CTEGORY_OPTION_ENABLE) { %>
																<tr id="trCategory" runat="server" visible="true">
																	<td class="edit_title_bg" align="center" colspan="5">カテゴリ</td>
																			</tr>
																<tr class="default_setting">
																	<td class="edit_title_bg default_setting" align="center" width="8%">表示</td>
																	<td class="edit_title_bg default_setting" align="center" width="26%">項目名</td>
																	<td class="edit_title_bg default_setting" align="center" width="16%">項目メモ</td>
																	<td class="edit_title_bg default_setting" align="center" width="50%" colspan="2">初期値</td>
																</tr>
																<tr id="trCategoryId1" runat="server" visible="true">
																	<td class="edit_title_bg default_setting" align="center" style="min-width: 30px;">
																		<asp:CheckBox ID="cbCategoryId1DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID1) %>" />
																				</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">カテゴリ1</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbCategoryId1DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID1) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center" style="min-width: 30px;">
																		<asp:CheckBox ID="cbCategoryId1HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATEGORY_ID1) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox ID="tbCategoryId1" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATEGORY_ID1) %>" Width="200" MaxLength="30" />
																	</td>
																			</tr>
																<tr id="trCategoryId2" runat="server" visible="true">
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbCategoryId2DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID2) %>" />
																				</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">カテゴリ2</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbCategoryId2DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID2) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbCategoryId2HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATEGORY_ID2) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox ID="tbCategoryId2" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATEGORY_ID2) %>" Width="200" MaxLength="30" />
																	</td>
																			</tr>
																<tr id="trCategoryId3" runat="server" visible="true">
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbCategoryId3DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID3) %>" />
																					</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">カテゴリ3</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbCategoryId3DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID3) %>" />
																					</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbCategoryId3HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATEGORY_ID3) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox ID="tbCategoryId3" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATEGORY_ID3) %>" Width="200" MaxLength="30" />
																	</td>
																				</tr>
																<tr id="trCategoryId4" runat="server" visible="true">
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbCategoryId4DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID4) %>" />
																					</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">カテゴリ4</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbCategoryId4DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID4) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbCategoryId4HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATEGORY_ID4) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox ID="tbCategoryId4" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATEGORY_ID4) %>" Width="200" MaxLength="30" />
																	</td>
																				</tr>
																<tr id="trCategoryId5" runat="server" visible="true">
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbCategoryId5DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID5) %>" />
																	</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">カテゴリ5</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbCategoryId5DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID5) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbCategoryId5HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATEGORY_ID5) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox ID="tbCategoryId5" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_CATEGORY_ID5) %>" Width="200" MaxLength="30" />
																	</td>
																</tr>
													<% } %>
																<% if (Constants.PRODUCT_BRAND_ENABLED) { %>
														<tr>
																	<td class="edit_title_bg" align="center" colspan="5">ブランド</td>
																	</tr>
																<tr>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbBrandId1DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID1) %>" Enabled="false" />
																		</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">ブランドID1<span class="notice">*</span></td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbBrandId1DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID1) %>" />
																		</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbBrandId1HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BRAND_ID1) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																		<asp:DropDownList ID="ddlBrandId1" runat="server" Width="300" />
																		</td>
																	</tr>
																<tr>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbBrandId2DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID2) %>" />
																		</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">ブランドID2</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbBrandId2DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID2) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbBrandId2HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BRAND_ID2) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:DropDownList ID="ddlBrandId2" runat="server" Width="300" />
																	</td>
																	</tr>
																<tr>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbBrandId3DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID3) %>" />
																		</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">ブランドID3</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbBrandId3DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID3) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbBrandId3HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BRAND_ID3) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:DropDownList ID="ddlBrandId3" runat="server" Width="300" />
																	</td>
																	</tr>
																<tr>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbBrandId4DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID4) %>" />
																		</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">ブランドID4</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbBrandId4DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID4) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbBrandId4HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BRAND_ID4) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:DropDownList ID="ddlBrandId4" runat="server" Width="300" />
																	</td>
																	</tr>
																<tr>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbBrandId5DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID5) %>" />
															</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">ブランドID5</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbBrandId5DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID5) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbBrandId5HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BRAND_ID5) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:DropDownList ID="ddlBrandId5" runat="server" Width="300" />
																	</td>
														</tr>
													<% } %>
														<tr>
																	<td class="edit_title_bg" align="center" colspan="5">関連する商品の設定（クロスセル）</td>
														</tr>
														<tr>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbRelatedProductIdCs1DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1) %>" />
															</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">関連商品ID</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbRelatedProductIdCs1DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbRelatedProductIdCsHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox ID="tbRelatedProductIdCs1" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1) %>" Width="150" MaxLength="30" />
																		<br />
																		<asp:TextBox ID="tbRelatedProductIdCs2" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2) %>" Width="150" MaxLength="30" />
																		<asp:TextBox ID="tbRelatedProductIdCs3" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3) %>" Width="150" MaxLength="30" />
																		<br />
																		<asp:TextBox ID="tbRelatedProductIdCs4" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4) %>" Width="150" MaxLength="30" />
																		<asp:TextBox ID="tbRelatedProductIdCs5" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5) %>" Width="150" MaxLength="30" />
																	</td>
														</tr>
																<tr>
																	<td class="edit_title_bg" align="center" colspan="5">関連する商品の設定（アップセル）</td>
													</tr>
																<tr>
																	<td class="edit_title_bg default_setting" align="center">
																		<asp:CheckBox ID="cbRelatedProductIdUs1DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1) %>" />
																	</td>
																	<td class="edit_title_bg default_setting_item" align="left" width="200">関連商品ID</td>
																	<td class="edit_item_bg default_setting" align="left">
																		<asp:TextBox ID="tbRelatedProductIdUs1DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																		<asp:CheckBox ID="cbRelatedProductIdUsHasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox ID="tbRelatedProductIdUs1" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1) %>" Width="150" MaxLength="30" />
																		<br />
																		<asp:TextBox ID="tbRelatedProductIdUs2" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2) %>" Width="150" MaxLength="30" />
																		<asp:TextBox ID="tbRelatedProductIdUs3" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3) %>" Width="150" MaxLength="30" />
																		<br />
																		<asp:TextBox ID="tbRelatedProductIdUs4" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4) %>" Width="150" MaxLength="30" />
																		<asp:TextBox ID="tbRelatedProductIdUs5" runat="server" Text="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5) %>" Width="150" MaxLength="30" />
																	</td>
													</tr>
												</table>
												<br />
												<%-- ▽キャンペーンアイコン エラーメッセージ表示▽ --%>
												<asp:UpdatePanel ID="upIconErrorMessages" runat="server">
													<ContentTemplate>
														<table id="tblIconErrorMessages" class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server" visible="false">
															<tbody>
															<tr>
																				<td class="edit_title_bg" align="center" colspan="5">エラーメッセージ</td>
															</tr>
															<tr>
																				<td class="edit_item_bg" align="left" colspan="5" style="border-bottom: none;">
																	<asp:Label ID="lbIconErrorMessages" runat="server" ForeColor="red" />
																</td>
															</tr>
															</tbody>
														</table>
													</ContentTemplate>
												</asp:UpdatePanel>
												<%-- △キャンペーンアイコン エラーメッセージ表示△ --%>
															<asp:UpdatePanel ID="UpdatePanel2" runat="server">
												<ContentTemplate>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																		<tr>
																			<td class="edit_title_bg" align="center" colspan="5">キャンペーンアイコン</td>
														</tr>
																		<tr>
																			<td class="edit_title_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg1DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG1) %>" />
																			</td>
																			<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン1</td>
																			<td class="edit_item_bg default_setting" align="left" rowspan="2">
																				<asp:TextBox ID="tbIconFlg1DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG1) %>" />
																			</td>
																			<td class="edit_item_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg1HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG1) %>" />
																			</td>
																			<td class="edit_item_bg" align="left">
																				<asp:CheckBox ID="cbIconFlg1" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG1) == Constants.FLG_PRODUCT_ICON_ON %>" />
																			</td>
														</tr>
																		<tr>
															<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン1有効期限</td>
															<td class="edit_item_bg" align="left">
																				<p>
																					～
																	<uc:DateTimeInput ID="ucIconFlgTermEnd1" runat="server" YearList="<%# DateTimeUtility.GetLongRangeYearListItem() %>" SelectedDate="<%# GetIconFlgTermEnd(1) %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
																					<asp:Button runat="server" Text="  リセット  " ID="btnResetIconFlgTermEnd1" OnClick="btnResetIconFlgTermEnd_OnClick" CommandName="ucIconFlgTermEnd1" CommandArgument="1" />
																</p>
															</td>
														</tr>
																		<tr>
																			<td class="edit_title_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg2DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG2) %>" />
																			</td>
																			<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン2</td>
																			<td class="edit_item_bg default_setting" align="left" rowspan="2">
																				<asp:TextBox ID="tbIconFlg2DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG2) %>" />
																			</td>
																			<td class="edit_item_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg2HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG2) %>" />
																			</td>
																			<td class="edit_item_bg" align="left">
																				<asp:CheckBox ID="cbIconFlg2" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG2) == Constants.FLG_PRODUCT_ICON_ON %>" />
																			</td>
														</tr>
																		<tr>
															<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン2有効期限</td>
															<td class="edit_item_bg" align="left">
																				<p>
																					～
																	<uc:DateTimeInput ID="ucIconFlgTermEnd2" runat="server" YearList="<%# DateTimeUtility.GetLongRangeYearListItem() %>" SelectedDate="<%# GetIconFlgTermEnd(2) %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
																	<asp:Button runat="server" Text="  リセット  " ID="btnResetIconFlgTermEnd2" OnClick="btnResetIconFlgTermEnd_OnClick" CommandName="ucIconFlgTermEnd2" CommandArgument="2" />
																</p>
															</td>
														</tr>
																		<tr>
																			<td class="edit_title_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg3DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG3) %>" />
																			</td>
																			<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン3</td>
																			<td class="edit_item_bg default_setting" align="left" rowspan="2">
																				<asp:TextBox ID="tbIconFlg3DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG3) %>" />
																			</td>
																			<td class="edit_item_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg3HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG3) %>" />
																			</td>
																			<td class="edit_item_bg" align="left">
																				<asp:CheckBox ID="cbIconFlg3" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG3) == Constants.FLG_PRODUCT_ICON_ON %>" />
																			</td>
														</tr>
																		<tr>
															<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン3有効期限</td>
															<td class="edit_item_bg" align="left">
																				<p>
																					～
																	<uc:DateTimeInput ID="ucIconFlgTermEnd3" runat="server" YearList="<%# DateTimeUtility.GetLongRangeYearListItem() %>" SelectedDate="<%# GetIconFlgTermEnd(3) %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
																					<asp:Button runat="server" Text="  リセット  " ID="btnResetIconFlgTermEnd3" OnClick="btnResetIconFlgTermEnd_OnClick" CommandName="ucIconFlgTermEnd3" CommandArgument="3" />
																</p>
															</td>
														</tr>
																		<tr>
																			<td class="edit_title_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg4DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG4) %>" />
																			</td>
																			<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン4</td>
																			<td class="edit_item_bg default_setting" align="left" rowspan="2">
																				<asp:TextBox ID="tbIconFlg4DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG4) %>" />
																			</td>
																			<td class="edit_item_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg4HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG4) %>" />
																			</td>
																			<td class="edit_item_bg" align="left">
																				<asp:CheckBox ID="cbIconFlg4" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG4) == Constants.FLG_PRODUCT_ICON_ON %>" />
																			</td>
														</tr>
																		<tr>
															<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン4有効期限</td>
															<td class="edit_item_bg" align="left">
																				<p>
																					～
																	<uc:DateTimeInput ID="ucIconFlgTermEnd4" runat="server" YearList="<%# DateTimeUtility.GetLongRangeYearListItem() %>" SelectedDate="<%# GetIconFlgTermEnd(4) %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
																					<asp:Button runat="server" Text="  リセット  " ID="btnResetIconFlgTermEnd4" OnClick="btnResetIconFlgTermEnd_OnClick" CommandName="ucIconFlgTermEnd4" CommandArgument="4" />
																</p>
															</td>
														</tr>
																		<tr>
																			<td class="edit_title_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg5DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG5) %>" />
																			</td>
																			<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン5</td>
																			<td class="edit_item_bg default_setting" align="left" rowspan="2">
																				<asp:TextBox ID="tbIconFlg5DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG5) %>" />
																			</td>
																			<td class="edit_item_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg5HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG5) %>" />
																			</td>
																			<td class="edit_item_bg" align="left">
																				<asp:CheckBox ID="cbIconFlg5" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG5) == Constants.FLG_PRODUCT_ICON_ON %>" />
																			</td>
														</tr>
																		<tr>
															<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン5有効期限</td>
															<td class="edit_item_bg" align="left">
																				<p>
																					～
																	<uc:DateTimeInput ID="ucIconFlgTermEnd5" runat="server" YearList="<%# DateTimeUtility.GetLongRangeYearListItem() %>" SelectedDate="<%# GetIconFlgTermEnd(5) %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
																					<asp:Button runat="server" Text="  リセット  " ID="btnResetIconFlgTermEnd5" OnClick="btnResetIconFlgTermEnd_OnClick" CommandName="ucIconFlgTermEnd5" CommandArgument="5" />
																</p>
															</td>
														</tr>
																		<tr>
																			<td class="edit_title_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg6DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG6) %>" />
																			</td>
																			<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン6</td>
																			<td class="edit_item_bg default_setting" align="left" rowspan="2">
																				<asp:TextBox ID="tbIconFlg6DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG6) %>" />
																			</td>
																			<td class="edit_item_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg6HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG6) %>" />
																			</td>
																			<td class="edit_item_bg" align="left">
																				<asp:CheckBox ID="cbIconFlg6" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG6) == Constants.FLG_PRODUCT_ICON_ON %>" />
																			</td>
														</tr>
																		<tr>
															<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン6有効期限</td>
															<td class="edit_item_bg" align="left">
																				<p>
																					～
																	<uc:DateTimeInput ID="ucIconFlgTermEnd6" runat="server" YearList="<%# DateTimeUtility.GetLongRangeYearListItem() %>" SelectedDate="<%# GetIconFlgTermEnd(6) %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
																					<asp:Button runat="server" Text="  リセット  " ID="btnResetIconFlgTermEnd6" OnClick="btnResetIconFlgTermEnd_OnClick" CommandName="ucIconFlgTermEnd6" CommandArgument="6" />
																</p>
															</td>
														</tr>
																		<tr>
																			<td class="edit_title_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg7DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG7) %>" />
																			</td>
																			<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン7</td>
																			<td class="edit_item_bg default_setting" align="left" rowspan="2">
																				<asp:TextBox ID="tbIconFlg7DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG7) %>" />
																			</td>
																			<td class="edit_item_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg7HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG7) %>" />
																			</td>
																			<td class="edit_item_bg" align="left">
																				<asp:CheckBox ID="cbIconFlg7" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG7) == Constants.FLG_PRODUCT_ICON_ON %>" />
																			</td>
														</tr>
																		<tr>
															<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン7有効期限</td>
															<td class="edit_item_bg" align="left">
																				<p>
																					～
																	<uc:DateTimeInput ID="ucIconFlgTermEnd7" runat="server" YearList="<%# DateTimeUtility.GetLongRangeYearListItem() %>" SelectedDate="<%# GetIconFlgTermEnd(7) %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
																					<asp:Button runat="server" Text="  リセット  " ID="btnResetIconFlgTermEnd7" OnClick="btnResetIconFlgTermEnd_OnClick" CommandName="ucIconFlgTermEnd7" CommandArgument="7" />
																</p>
															</td>
														</tr>
																		<tr>
																			<td class="edit_title_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg8DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG8) %>" />
																			</td>
																			<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン8</td>
																			<td class="edit_item_bg default_setting" align="left" rowspan="2">
																				<asp:TextBox ID="tbIconFlg8DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG8) %>" />
																			</td>
																			<td class="edit_item_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg8HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG8) %>" />
																			</td>
																			<td class="edit_item_bg" align="left">
																				<asp:CheckBox ID="cbIconFlg8" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG8) == Constants.FLG_PRODUCT_ICON_ON %>" />
																			</td>
														</tr>
																		<tr>
															<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン8有効期限</td>
															<td class="edit_item_bg" align="left">
																				<p>
																					～
																	<uc:DateTimeInput ID="ucIconFlgTermEnd8" runat="server" YearList="<%# DateTimeUtility.GetLongRangeYearListItem() %>" SelectedDate="<%# GetIconFlgTermEnd(8) %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
																					<asp:Button runat="server" Text="  リセット  " ID="btnResetIconFlgTermEnd8" OnClick="btnResetIconFlgTermEnd_OnClick" CommandName="ucIconFlgTermEnd8" CommandArgument="8" />
																</p>
															</td>
														</tr>
																		<tr>
																			<td class="edit_title_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg9DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG9) %>" />
																			</td>
																			<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン9</td>
																			<td class="edit_item_bg default_setting" align="left" rowspan="2">
																				<asp:TextBox ID="tbIconFlg9DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG9) %>" />
																			</td>
																			<td class="edit_item_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg9HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG9) %>" />
																			</td>
																			<td class="edit_item_bg" align="left">
																				<asp:CheckBox ID="cbIconFlg9" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG9) == Constants.FLG_PRODUCT_ICON_ON %>" />
																			</td>
														</tr>
																		<tr>
															<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン9有効期限</td>
															<td class="edit_item_bg" align="left">
																				<p>
																					～
																	<uc:DateTimeInput ID="ucIconFlgTermEnd9" runat="server" YearList="<%# DateTimeUtility.GetLongRangeYearListItem() %>" SelectedDate="<%# GetIconFlgTermEnd(9) %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
																					<asp:Button runat="server" Text="  リセット  " ID="btnResetIconFlgTermEnd9" OnClick="btnResetIconFlgTermEnd_OnClick" CommandName="ucIconFlgTermEnd9" CommandArgument="9" />
																</p>
															</td>
														</tr>													
																		<tr>
																			<td class="edit_title_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg10DefaultSetting" runat="server" Checked="<%# GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG10) %>" />
																			</td>
																			<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン10</td>
																			<td class="edit_item_bg default_setting" align="left" rowspan="2">
																				<asp:TextBox ID="tbIconFlg10DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG10) %>" />
																			</td>
																			<td class="edit_item_bg default_setting" align="center" rowspan="2">
																				<asp:CheckBox ID="cbIconFlg10HasDefault" runat="server" Checked="<%# HasProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG10) %>" />
																			</td>
																			<td class="edit_item_bg" align="left">
																				<asp:CheckBox ID="cbIconFlg10" runat="server" Checked="<%# GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_ICON_FLG10) == Constants.FLG_PRODUCT_ICON_ON %>" />
																			</td>
														</tr>
																		<tr>
															<td class="edit_title_bg default_setting_item" align="left" width="200">アイコン10有効期限</td>
															<td class="edit_item_bg" align="left">
																				<p>
																					～
																	<uc:DateTimeInput ID="ucIconFlgTermEnd10" runat="server" YearList="<%# DateTimeUtility.GetLongRangeYearListItem() %>" SelectedDate="<%# GetIconFlgTermEnd(10) %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" />
																					<asp:Button runat="server" Text="  リセット  " ID="btnResetIconFlgTermEnd10" OnClick="btnResetIconFlgTermEnd_OnClick" CommandName="ucIconFlgTermEnd10" CommandArgument="10" />
																</p>
															</td>
														</tr>
													</table>
												<br />
												</ContentTemplate>
												</asp:UpdatePanel>
												<!-- △カテゴリ△ -->
															<p id="anchorVariation" align="right" style="margin-bottom: 5px;"><a href="#top">ページトップ</a></p>
												<%-- ▽商品バリエーション エラーメッセージ表示▽ --%>
												<asp:UpdatePanel ID="upVariationErrorMessages" runat="server">
													<ContentTemplate>
														<table id="tblVariationErrorMessages" class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server" visible="false">
															<tbody>
															<tr>
																				<td class="edit_title_bg" align="center" colspan="5">エラーメッセージ</td>
															</tr>
															<tr>
																				<td class="edit_item_bg" align="left" colspan="5" style="border-bottom: none;">
																	<asp:Label ID="lbVariationErrorMessages" runat="server" ForeColor="red" />
																</td>
															</tr>
															</tbody>
														</table>
													</ContentTemplate>
												</asp:UpdatePanel>
												<%-- △商品バリエーション エラーメッセージ表示△ --%>
												<!-- ▽商品バリエーション▽ -->
												<table class="edit_table" cellspacing="1" cellpadding="0" width="758" border="0">
																<tr>
																	<td class="edit_title_bg" colspan="7" align="center">商品バリエーション</td>
													</tr>
																<tr>
														<td class="edit_title_bg" colspan="2"></td>
																	<td class="edit_title_bg default_setting" align="center">表示</td>
																	<td class="edit_title_bg default_setting" align="center">項目名</td>
																	<td class="edit_title_bg default_setting" align="center">項目メモ</td>
														<td class="edit_title_bg default_setting" align="center" colspan="2">初期値</td>
													</tr>
															<tbody id="tbdyVariationItem" runat="server">
																<tr>
																		<td class="edit_title_bg" align="center" width="20" rowspan="26">商品バリエーションID<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left" width="60" rowspan="26">
																			<div class="default_setting">
																				<asp:CheckBox ID="cbVIdHasDefault" runat="server" Checked="true" Enabled="false" />
																			</div>
																			商品ID＋<br />
																			<asp:TextBox ID="tbVId" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_V_ID) %>" Width="50" MaxLength="30" Enabled="false" />
																		</td>
																		<td class="edit_title_bg default_setting" align="center" width="5%" style="min-width: 30px;">
																			<asp:CheckBox ID="cbDisplayOrderDefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER) %>" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">表示順<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left" width="15%">
																			<asp:TextBox ID="tbDisplayOrderDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER) %>" />
																	</td>
																		<td class="edit_item_bg default_setting" align="center" width="20" style="min-width: 30px;">
																			<asp:CheckBox ID="cbDisplayOrderHasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER) %>" />
																	</td>
																		<td class="edit_item_bg" align="left" width="200">
																			<asp:TextBox ID="tbDisplayOrder" Text='<%# StringUtility.ToValue(GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER), 1) %>' MaxLength="3" Width="25" runat="server" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationName1DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1) %>" Enabled="false" />
																		</td>
																		<td class="edit_title_bg" align="left">表示名1<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationName1DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationName1HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationName1" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1) %>" Width="100" MaxLength="30" />&nbsp;
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationName2DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">表示名2</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationName2DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationName2HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationName2" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2) %>" Width="100" MaxLength="30" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationName3DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">表示名3</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationName3DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationName3HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationName3" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %>" Width="100" MaxLength="30" />
																	</td>
																</tr>
																<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationWeightGramDefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM) %>" Enabled="false" />
																		</td>
																		<td class="edit_title_bg" align="left">重量（g）<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationWeightGramDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationWeightGramHasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationWeightGram" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM) %>" Width="100" MaxLength="28" />
																	</td>
																</tr>
																<% } %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationColorImageDefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">カラーイメージ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationColorImageDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationColorImageHasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:DropDownList ID="ddlVariationColorImage" runat="server" DataSource='<%# GetProductColorListItem() %>' SelectedValue="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID) %>" DataValueField="value" DataTextField="text" />
																			<img id='<%# "iVariationColorImage0" %>' name="iVariationColorImage" src='<%: ProductColorUtility.GetColorImageUrl(ddlVariationColorImage.SelectedValue) %>' alt="" style="vertical-align: middle;" width="25" height="25" />
																		<br />
																		※カラーを追加・変更したい場合は「<%: Constants.PATH_IMAGES_COLOR %>」に画像ファイルを設置したうえで、<br />
																		「<%: Constants.FILEPATH_XML_COLORS %>」に設定を追加してください。<br />
																			フォーマット：<br />
																			&lt;ProductColor id="重複しない任意のID" filename="カラーの画像ファイル名" dispname="カラーの表示名"&gt;&lt;/ProductColor&gt;
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationPriceDefaultSetting" runat="server" Checked="true" Enabled="False" />
																		</td>
																		<td class="edit_title_bg" align="left">販売価格<span class="notice">*</span></td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationPriceDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_PRICE) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationPriceHasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_PRICE) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationPrice" runat="server" MaxLength="7" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_PRICE).ToPriceString() %>" Width="50" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationSpecialPriceDefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">特別価格</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationSpecialPriceDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationSpecialPriceHasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationSpecialPrice" runat="server" MaxLength="7" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE).ToPriceString() %>" Width="50" />
																	</td>
																</tr>
																	<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
																	<tr>
																	<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationFixedPurchaseFirsttimePriceDefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">定期購入初回価格</td>
																	<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationFixedPurchaseFirsttimePriceDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE) %>" />
																		</td>
																	<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationFixedPurchaseFirsttimePriceHasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationFixedPurchaseFirsttimePrice" runat="server" MaxLength="7" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE).ToPriceString() %>" Width="100" />
																		</td>
																</tr>
																	<tr>
																	<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationFixedPurchasePriceDefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE) %>" />
																	</td>
																		<td class="edit_title_bg" align="left">定期購入価格</td>
																	<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationFixedPurchasePriceDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE) %>" />
																	</td>
																	<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationFixedPurchasePriceHasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE) %>" />
																	</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationFixedPurchasePrice" runat="server" MaxLength="7" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE).ToPriceString() %>" Width="100" />
																	</td>
																</tr>
																<% } %>
																	<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationMemberRankPriceDefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">会員ランク価格</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationMemberRankPriceDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE) %>" />
																		</td>
																	<td class="edit_item_bg default_setting" align="center"></td>
																		<td class="edit_item_bg" align="left"></td>
																	</tr>
																	<% } %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId1DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">商品バリエーション連携ID1</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationCooperationId1DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId1HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationCooperationId1" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId2DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">商品バリエーション連携ID2</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationCooperationId2DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId2HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationCooperationId2" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId3DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">商品バリエーション連携ID3</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationCooperationId3DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId3HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationCooperationId3" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId4DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4) %>" /></td>
																		<td class="edit_title_bg" align="left">商品バリエーション連携ID4</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationCooperationId4DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId4HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationCooperationId4" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId5DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">商品バリエーション連携ID5</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationCooperationId5DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId5HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationCooperationId5" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId6DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">商品バリエーション連携ID6</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationCooperationId6DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId6HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationCooperationId6" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId7DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">商品バリエーション連携ID7</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationCooperationId7DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId7HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationCooperationId7" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId8DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">商品バリエーション連携ID8</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationCooperationId8DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId8HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationCooperationId8" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId9DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">商品バリエーション連携ID9</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationCooperationId9DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId9HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationCooperationId9" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId10DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">商品バリエーション連携ID10</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationCooperationId10DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationCooperationId10HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationCooperationId10" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationAndMallReservationFlgDefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">＆mall連携予約商品フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationAndMallReservationFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationAndMallReservationFlgHasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbVariationAndMallReservationFlg" runat="server" Checked="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG) == Constants.FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_RESERVATION %>" Text="有効" />
																	</td>
																</tr>
																	<% } %>
																	<% if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationDownloadUrlDefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL) %>" />
																		</td>
																		<td class="edit_title_bg" align="left" width="200">ダウンロードURL</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbVariationDownloadUrlDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbVariationDownloadUrlHasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:TextBox ID="tbVariationDownloadUrl" runat="server" MaxLength="1000" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL) %>" Width="300" />
																	</td>
																</tr>
																	<% } %>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbMallVariationId1DefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">モールバリエーション</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbMallVariationId1DefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbMallVariationId1HasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1) %>" />
																		</td>
																		<td class="edit_item_bg" align="left">ID1:
																			<asp:TextBox ID="tbMallVariationId1" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1) %>" Width="100" MaxLength="200" />
																			<br />
																			ID2:
																			<asp:TextBox ID="tbMallVariationId2" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2) %>" Width="100" MaxLength="200" />
																			<br />
																			種別:
																			<asp:TextBox ID="tbMallVariationType" runat="server" Text="<%# GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE) %>" Width="100" MaxLength="200" />
																	</td>
																</tr>
																	<tr>
																		<td class="edit_title_bg default_setting" align="center">
																			<asp:CheckBox ID="cbValiationAddCartUrlLimitFlgDefaultSetting" runat="server" Checked="<%# GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG) %>" />
																		</td>
																		<td class="edit_title_bg" align="left">カート投入URL制限フラグ</td>
																		<td class="edit_item_bg default_setting" align="left">
																			<asp:TextBox ID="tbValiationAddCartUrlLimitFlgDefaultSetting" runat="server" Width="100" MaxLength="50" Text="<%# GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG) %>" />
																		</td>
																		<td class="edit_item_bg default_setting" align="center">
																			<asp:CheckBox ID="cbValiationAddCartUrlLimitFlgHasDefault" runat="server" Checked="<%# HasVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG) %>" />
																		</td>
																	<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbValiationAddCartUrlLimitFlg" runat="server" Checked="<%# (GetVariationDefaultSettingFieldValue(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG) == Constants.FLG_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG_VALID) %>" Text="有効" />
																	</td>
																</tr>
															</tbody>
												</table>
												<!-- △商品バリエーション△ -->
												</ContentTemplate>
												</asp:UpdatePanel>
												<br />
												<!-- ▽BOTTOM▽ -->
												<div class="action_part_bottom">
													<asp:Button id="btnBackListBottom" runat="server" Text="  一覧へ戻る  " Visible="False" OnClick="btnBackList_Click" />
													<asp:Button id="btnUpdateDefaultSettingBottom" runat="server" Visible="false" Text="  更新する  " OnClick="btnUpdateDefaultSetting_Click" />
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
		<!-- △登録△ -->
	<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
	</tr>
</table>
<script type="text/javascript">
	<%-- カラーイメージ画像変更（バリエーション） --%>
		function bodyPageLoad(sender, args) {
			for (var i = 0; i < $('[name$=ddlVariationColorImage]').length; i++) {
			var ddlVariationColorImageObject = $("#" + $('[name$=ddlVariationColorImage]')[i].id);
			var iVariationColorImageObject = $('#' + $('[name=iVariationColorImage]')[i].id);
			iVariationColorImageObject.css('display', 'none');
			iVariationColorImageObject.attr("src", '');
				<% foreach (var color in this.ProductColors) { %>
				if (('<%: color.Id %>' == ddlVariationColorImageObject.val()) && (ddlVariationColorImageObject.val() != '')) {
				iVariationColorImageObject.css('display', 'inline');
				iVariationColorImageObject.attr("src", decodeURI('<%: ProductColorUtility.GetColorImageUrl(color.Id) %>'));
			}
				<% } %>
				ddlVariationColorImageObject.change({ddlObject: ddlVariationColorImageObject, imageObject:iVariationColorImageObject}, function(e) {
				e.data.imageObject.css('display', 'none');
				e.data.imageObject.attr("src", '');
					<% foreach (var color in this.ProductColors) { %>
					if (('<%: color.Id %>' == e.data.ddlObject.val()) && (ddlVariationColorImageObject.val() != '')) {
					e.data.imageObject.css('display', 'inline');
					e.data.imageObject.attr("src", decodeURI('<%: ProductColorUtility.GetColorImageUrl(color.Id) %>'));
				}
					<% } %>
			});
		}

			var colorImage = $('#<%# ddlColorImage.ClientID %>').val();
			if (colorImage == "") {
				$('#iColorImage').css('display', 'none');
				$('#iColorImage').attr("src", '');
			} else {
			<% foreach (var color in this.ProductColors) { %>
				if ('<%: color.Id %>' == colorImage) {
					$('#iColorImage').css('display', 'inline');
					$('#iColorImage').attr("src", decodeURI('<%: ProductColorUtility.GetColorImageUrl(color.Id) %>'));
				}
			<% } %>
			}

			$('#<%# ddlColorImage.ClientID %>').change(function () {
				$('#iColorImage').css('display', 'none');
				$('#iColorImage').attr("src", '');
				<% foreach (var color in this.ProductColors) { %>
				if (('<%: color.Id %>' == $('#<%# ddlColorImage.ClientID %>').val()) && ('<%: color.Id %>' != "")) {
					$('#iColorImage').css('display', 'inline');
					$('#iColorImage').attr("src", decodeURI('<%= ProductColorUtility.GetColorImageUrl(color.Id) %>'));
				}
				<% } %>
			});
	}

	<%-- カラーイメージ画像変更（商品） --%>
	function changeColorImage(e, imageElement){
			<% foreach (var color in this.ProductColors) { %>
		imageElement.css('display', 'none');
		imageElement.attr("src") = '';
			if ('<%: color.Id %>' == e.srcElement.val()) {
			imageElement.css('display', 'inline');
				imageElement.attr("src") = decodeURI('<%: ProductColorUtility.GetColorImageUrl(color.Id) %>');
		}
			<% } %>
	}
	
		$(document).ready(function () {
			var scrollPosition = (isNaN($('#<%= hfScrollPosition.ClientID %>').val()) ? 0 : $('#<%= hfScrollPosition.ClientID %>').val());
			$(document).scrollTop(scrollPosition);

			<% if (this.HasErrorOnPostback) { %>
			scrollPositionOnError();
			<% } %>
		});

		$(document).scroll(function () {
			$('#<%= hfScrollPosition.ClientID %>').val($(document).scrollTop());
		});

	<%-- エラー発生時、エラー表示位置にスクロール --%>
		function scrollPositionOnError() {
			if ($('#<%= tblProductErrorMessages.ClientID %>').length > 0) {
			$(window).scrollTop(($('#<%= upProductErrorMessages.ClientID %>').offset().top - 100));
			} else if ($('#<%= tblProductRelationErrorMessages.ClientID %>').length > 0) {
			$(window).scrollTop(($('#<%= upProductRelationErrorMessages.ClientID %>').offset().top - 100));
			} else if ($('#<%= tblIconErrorMessages.ClientID %>').length > 0) {
			$(window).scrollTop(($('#<%= upIconErrorMessages.ClientID %>').offset().top - 100));
			} else if ($('#<%= tblVariationErrorMessages.ClientID %>').length > 0) {
			$(window).scrollTop(($('#<%= upVariationErrorMessages.ClientID %>').offset().top - 100));
		}
	}

	<%-- Date end clear --%>
	function dateEndClear(set_date) {
		if (set_date == 'displayToDate') {
			document.getElementById('<%= ucDisplay.HfEndDate.ClientID %>').value = '';
			document.getElementById('<%= ucDisplay.HfEndTime.ClientID %>').value = '';
			reloadDisplayDateTimePeriod('<%= ucDisplay.ClientID %>');
			} else if (set_date == 'sellToDate') {
			document.getElementById('<%= ucSell.HfEndDate.ClientID %>').value = '';
			document.getElementById('<%= ucSell.HfEndTime.ClientID %>').value = '';
			reloadDisplayDateTimePeriod('<%= ucSell.ClientID %>');
		}
	}
</script>
</asp:Content>
