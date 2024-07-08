<%--
=========================================================================================================
  Module      : 名称翻訳設定登録ページ(NameTranslationSettingRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="NameTranslationSettingRegister.aspx.cs" Inherits="Form_NameTranslationSetting_NameTranslationSettingRegister" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr><td><h1 class="page-title">名称翻訳設定</h1></td></tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 登録/更新 ▽-->
	<tr id="trRegister" runat="server" Visible="False">
		<td>
			<h1 class="cmn-hed-h2">名称翻訳設定登録</h1>
		</td>
	</tr>
	<tr id="trEdit" runat="server" Visible="False">
		<td>
			<h1 class="cmn-hed-h2">名称翻訳設定編集</h1>
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
												<div id="divComp" runat="server" class="action_part_top" visible="false">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">名称翻訳設定を登録/更新しました。</td>
														</tr>
													</table>
												</div>
												<div class="action_part_top">
													<asp:Button ID="btnToListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " OnClick="btnInsertUpdate_Click" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr id="trDataKbn" runat="server">
														<td class="edit_title_bg" align="left" style="width:250px;">対象データ区分 <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left" style="width:auto;">
															<asp:DropDownList ID="ddlDataKbn" Width="300" MaxLength="30" DataValueField="Value" DataTextField="Text" AutoPostBack="True" OnSelectedIndexChanged="ddlDataKbn_OnSelectedIndexChanged" runat="server" />
														</td>
													</tr>
													<tr id="trTranslationTargetColumn" runat="server">
														<td class="edit_title_bg" align="left">翻訳対象項目 <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList ID="ddlTranslationTargetColumn" Width="300" MaxLength="30" DataValueField="Value" DataTextField="Text" AutoPostBack="True" OnSelectedIndexChanged="ddlTranslationTargetColumn_OnSelectedIndexChanged" runat="server" />
														</td>
													</tr>
													<tr id="trMasterId1" runat="server">
														<td class="edit_title_bg" align="left">マスタID1<asp:Literal ID="lMasterId1" runat="server" /> <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbMasterId1" runat="server" Width="300" MaxLength="100" />
														</td>
													</tr>
													<tr id="trMasterId2" runat="server">
														<td class="edit_title_bg" align="left">マスタID2<asp:Literal ID="lMasterId2" runat="server" /> <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbMasterId2" runat="server" Width="300" MaxLength="100" />
														</td>
													</tr>
												</table>
												<br />
												<table id="tBeforeTranslationalName" class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server">
													<tr>
														<td class="edit_title_bg" style="width: 100px;" align="center">
															翻訳前名称
															<asp:Button ID="btnGetBeforeTranslationalName" runat="server" Text="  取得  " OnClick="btnGetBeforeTranslationalName_Click" Visible="False" />
														</td>
													</tr>
													<tr>
														<td class="edit_item_bg" style="height: 1em;" >
															<span id="sBeforeTranslationalNameNotExists" style="color: red;" Visible="false" runat="server">該当データがマスタに存在しません</span>
															<pre><asp:Literal ID="lBeforeTranslationalName" runat="server"></asp:Literal></pre>
														</td>
													</tr>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" style="width: 100px;" align="center">言語コード<br />(ロケールID)</td>
														<td class="edit_title_bg" style="width: auto;" align="center">翻訳後名称</td>
													</tr>
													<asp:Repeater ID="rLanguages" ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel" OnItemDataBound="rLanguages_OnItemDataBound" runat="server">
													<ItemTemplate>
													<tr>
														<td class="edit_title_bg"><%#: Item.LanguageCode %>(<%#: Item.LanguageLocaleId %>)</td>
														<td class="edit_item_bg" >
															<div style="margin-right: 15px;">
																<div ID="dDisplayKbn" runat="server">
																	<asp:RadioButtonList id="rbDisplayKbn" runat="server" Width="150" RepeatDirection="Horizontal" RepeatLayout="Flow" SelectedValue='<%# Item.ExtendDisplayKbn %>' CssClass="radio_button_list">
																		<asp:ListItem Value="0">TEXT</asp:ListItem>
																		<asp:ListItem Value="1">HTML</asp:ListItem>
																	</asp:RadioButtonList>
																	<input type= "button" onclick="javascript:open_wysiwyg('<%# Container.FindControl("tbAfterTranslationalName").ClientID %>', '<%# Container.FindControl("rbDisplayKbn").ClientID %>');" value="  HTMLエディタ  " /><br />
																</div>
																<asp:TextBox ID="tbAfterTranslationalName" Text="<%# Item.AfterTranslationalName %>" TextMode="MultiLine" Width="100%" Height="150" runat="server" />
															</div>
															<asp:HiddenField ID="hfLanguageCode" Value="<%#: Item.LanguageCode %>" runat="server"/>
															<asp:HiddenField ID="hfLanguageLocaleId" Value="<%#: Item.LanguageLocaleId %>" runat="server"/>
														</td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
												</table>
												<div class="action_part_bottom">
													<asp:Button ID="btnToListBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " OnClick="btnInsertUpdate_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " OnClick="btnInsertUpdate_Click" />
												</div>
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
	<!--△ 登録/更新 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>