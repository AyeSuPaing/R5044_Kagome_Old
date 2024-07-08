<%--
=========================================================================================================
  Module      : 自動翻訳設定登録ページ(AutoTranslationWordRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AutoTranslationWordRegister.aspx.cs" Inherits="Form_AutoTranslationWord_AutoTranslationWordRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">自動翻訳設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trEditTitle" runat="server">
		<td><h2 class="cmn-hed-h2">自動翻訳設定編集</h2></td>
	</tr>
	<tr id="trRegistTitle" runat="server">
		<td><h2 class="cmn-hed-h2">自動翻訳設定登録</h2></td>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<div id="divComp" runat="server" class="action_part_top" Visible="False">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">自動翻訳設定を登録/更新しました。
															</td>
														</tr>
													</table>
												</div>
												<div class="action_part_top">
													<asp:Button ID="btnToList" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click"  />
													<asp:Button id="btnInsert" runat="server" Text="  登録する  " OnClick="btnInsert_Click"  />
													<asp:Button id="btnUpdate" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" />
												</div>
												
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">自動翻訳情報</td>
													</tr>
													<div id="divDispInsert" runat="server" Visible="False">
													<tr>
														<td class="detail_title_bg" align="left">翻訳元ワード <span class="notice">*</span></td>
														<td class="detail_item_bg" align="left">
															<asp:TextBox ID="tbWordBefore" runat="server" Width="500" Height="200" TextMode="MultiLine"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">言語コード <span class="notice">*</span></td>
														<td class="detail_item_bg" align="left">
															<asp:TextBox ID="tbLanguageCode" runat="server" MaxLength="10" Width="50"></asp:TextBox>
													</tr>
													</div>
													<div id="divDispUpdate" runat="server" Visible="False">
													<tr>
														<td class="detail_title_bg" align="left" Width="150">翻訳元ワード </td>
														<td class="detail_item_bg" align="left">
														<asp:Literal ID="lWordBefore" runat="server"></asp:Literal>
														<%-- 改行コードを変換していない文字列を格納 --%>
														<asp:HiddenField ID="hfWordBefore" runat="server"></asp:HiddenField>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">言語コード </td>
														<td class="detail_item_bg" align="left">
														<asp:Literal ID="lLanguageCode" runat="server"></asp:Literal>
													</tr>
													</div>
													<tr runat="server">
														<td class="detail_title_bg" align="left">翻訳後ワード</td>
														<td class="detail_item_bg" align="left">
															<asp:TextBox ID="tbWordAfter" runat="server" Width="500" Height="200" TextMode="MultiLine"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">クリアフラグ</td>
														<td class="detail_item_bg" align="left">
															<asp:CheckBox ID="cbClearFlg" runat="server" Checked="True" /></td>
													</tr>
													<tbody id="tbdyDispUpdateInfo" runat="server">
														<tr>
															<td class="detail_title_bg" align="left">最終利用日時</td>
															<td class="detail_item_bg url_link" align="left">
																<asp:Literal ID="lDateUsed" runat="server"></asp:Literal></td>
														</tr>
													</tbody>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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

