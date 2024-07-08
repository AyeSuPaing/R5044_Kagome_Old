<%--
=========================================================================================================
  Module      : シリアルキー情報確認ページ(SerialKeyConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SerialKeyConfirm.aspx.cs" Inherits="Form_SerialKey_SerialKeyConfirm" %>

<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
<meta http-equiv="Pragma" content="no-cache" />
<meta http-equiv="cache-control" content="no-cache" />
<meta http-equiv="expires" content="0" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">シリアルキー情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">シリアルキー情報詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">シリアルキー情報入力確認</h2></td>
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
													<tr class="info_item_bg">
														<td align="left"><asp:Label ID="lMessage" runat="server"></asp:Label>
														</td>
													</tr>
												</table>
												</div>
												<div class="action_part_top">
													<asp:Button id="btnBackTop" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button ID="btnBackListTop" runat="server" Text="  一覧へ戻る  " Visible="False" OnClick="btnBackList_Click" />
													<asp:Button id="btnEditTop" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" OnClick="btnInsertUpdate_Click" />
												</div>
												<!-- ▽シリアルキー▽ -->
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">シリアルキー情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="250">シリアルキー</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(SerialKeyUtility.DecryptSerialKey((string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_SERIAL_KEY)))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="250">商品ID</td>
														<td class="detail_item_bg" align="left">
															<a href="javascript:open_window('<%# HttpUtility.UrlEncode(CreateProductDetailUrl((string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_PRODUCT_ID))) %>','productcontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																<%# WebSanitizer.HtmlEncode(GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_PRODUCT_ID)) %>
															</a>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="250">バリエーションID</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode((string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_PRODUCTVARIATION_V_ID) != "" ? "商品ID + " + GetKeyValue(this.SerialKeyMaster, Constants.FIELD_PRODUCTVARIATION_V_ID) : "-")%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="250">状態</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SERIALKEY, Constants.FIELD_SERIALKEY_STATUS, GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_STATUS)))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="250">注文ID</td>
														<td class="detail_item_bg" align="left">
															<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateOrderDetailUrl((string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_ORDER_ID))) %>','ordercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																<%# WebSanitizer.HtmlEncode(GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_ORDER_ID))%>
															</a>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="250">注文商品枝番</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_ORDER_ITEM_NO))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="250">ユーザーID</td>
														<td class="detail_item_bg" align="left">
															<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl((string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_USER_ID))) %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																<%# WebSanitizer.HtmlEncode((string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_USER_ID))%>
															</a>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="250">引渡日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_DATE_DELIVERED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="250">有効フラグ</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SERIALKEY, Constants.FIELD_SERIALKEY_VALID_FLG, (string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_VALID_FLG)))%></td>
													</tr>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="250">作成日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_DATE_CREATED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="250">更新日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_DATE_CHANGED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="250">最終更新者</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode((string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_LAST_CHANGED))%></td>
													</tr>
												</table>
												<!-- △シリアルキー△ -->
												<div class="action_part_bottom">
													<asp:Button id="btnBackBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button ID="btnBackListBottom" runat="server" Text="  一覧へ戻る  " Visible="False" OnClick="btnBackList_Click" />
													<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" OnClick="btnInsertUpdate_Click" />
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
