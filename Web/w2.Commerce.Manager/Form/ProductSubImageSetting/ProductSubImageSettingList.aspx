<%--
=========================================================================================================
  Module      : 商品サブ画像設定ページ(ProductSubImageSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductSubImageSettingList.aspx.cs" Inherits="Form_ProductSubImageSetting_ProductSubImageSettingList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<!-- タイトル -->
		<td><h1 class="page-title">商品サブ画像設定</h1></td>
	</tr>
	<tr>
		<!-- スペース -->
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<!-- タブ -->
		<td><h2 class="cmn-hed-h2">商品サブ画像設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<!--▽コンテンツ部分▽-->
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<!--編集ページ-->
									<div>
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<!-- スペース -->
												<td height="6" colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td>
											</tr>
											<tr>
												<!-- スペース -->
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr>
												<!-- 一括更新ボタン -->
												<td align="right"><asp:Button ID="btnAllUpdateTop" runat="server" Text="  一括更新する  " OnClick="btnAllUpdate_Click" OnClientClick="return confirm('名前のある欄のみ有効になりますがよろしいですか？');" /></td>
												<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
											</tr>
											<tr>
												<!-- スペース -->
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr>
												<td valign="top">
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0" align="center">
													<tbody id="tbodyExtend">
														<tr class="list_title_bg">
															<td align="center" style="width:100px">No.</td>
															<td align="left">商品サブ画像設定名</td>
														</tr>
														<asp:Repeater id="rExtendList" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg1">
																	<td align="center">
																		<%# WebSanitizer.HtmlEncode(Container.ItemIndex + 1) %>
																	</td>
																	<td align="left">
																		<asp:TextBox ID="tbSubImageName" runat="server" Width="380" MaxLength="30"
																			Text='<%# Container.DataItem %>'>
																		</asp:TextBox>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
													</table>
												</td>
												<!-- スペース -->
												<td><img src="../../Images/Common/sp.gif" width="5" height="1" border="0" alt="" /></td>
											</tr>
											<tr>
												<!-- スペース -->
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr>
												<!-- 備考 -->
												<td colspan="6">
													<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="info_item_bg">
															<td align="left">備考<br />
															本機能は商品情報メニューと連携します。<br />
															商品サブ画像設定名の設定数分、商品毎にサブ画像を追加できます。(ファイル名等詳細は商品情報メニューを参照してください)<br />
															商品サブ画像設定名は、商品サブ画像拡大表示画面内で他画像へのリンク用小画像にカーソルを合わせると表示されます。<br />
															サブ画像を設定した後名前を削除した場合、サブ画像は表示されなくなりますがデータは削除されません。
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<!-- スペース -->
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr>
												<!-- 一括更新ボタン -->
												<td align="right"><asp:Button ID="btnAllUpdateBottom" runat="server" Text="  一括更新する  " OnClick="btnAllUpdate_Click" OnClientClick="return confirm('名前のある欄のみ有効になりますがよろしいですか？');" /></td>
												<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
											</tr>
											<tr>
												<!-- スペース -->
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
										</table>
									</div>
								</td>
							</tr>
						</table>
						<!--△コンテンツ部分△-->
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<!-- スペース -->
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>

