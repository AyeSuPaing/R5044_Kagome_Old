<%--
=========================================================================================================
  Module      : オペレータ権限設定登録ページ(OperatorAuthorityRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OperatorAuthorityRegister.aspx.cs" Inherits="Form_OperatorAuthority_OperatorAuthorityRegister" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">オペレータ権限情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 入力 ▽-->
	<tr>
		<td>
			<h2 class="cmn-hed-h2">
				<% if ((string)Session[Constants.SESSION_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT){%>
					オペレータ権限設定登録
				<%} %>
				<% if ((string)Session[Constants.SESSION_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE){%>
					オペレータ権限設定編集
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
												<div class="action_part_top">
													<input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnConfirm" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trOperatorId" runat="server" visible="false">
														<td class="edit_title_bg" align="left" width="30%">オペレータ権限ID</td>
														<td class="edit_item_bg" align="left"><asp:Label ID="lbOperatorAuthorityId" runat="server"></asp:Label></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">オペレータ権限名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbOperatorAuthorityName" runat="server" MaxLength="20" Width="240"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">オペレーション</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox runat="server" ID="cbPermitEdit" Text="編集"/>
															<asp:CheckBox runat="server" ID="cbPermitMailSend" Text="メール直接送信"/>
															<asp:CheckBox runat="server" ID="cbPermitApproval" Text="承認"/>
															<asp:CheckBox runat="server" ID="cbPermitUnlock" Text="ロック強制解除"/>
															<asp:CheckBox runat="server" ID="cbPermitPermanentDelete" Text="完全削除"/>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">システム設定</td>
														<td class="edit_item_bg" align="left"><asp:CheckBox runat="server" ID="cbPermitEditSignature" Text="共通署名編集"/></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール通知</td>
														<td class="edit_item_bg" align="left"><asp:CheckBox runat="server" ID="cbReceiveNoAssignWarning" Text="担当未設定警告"/></td>
													</tr>
												</table>
												<br />
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">
														    備考<br />
															・ 編集　　 ・・・メッセージ登録・インシデント更新などのCSオペレーションができるようになります。<br />
															・ メール直接送信 ・・・他のオペレータに承認を受けたり送信代行の依頼をすることなく、作成したメールを直接送信できます。<br />
															　　　　　　　　　　　　　 また、送信代行先リストに表示され、他のオペレータからの送信代行依頼を受け付けることができるようになります。<br />
															・ 承認　　 ・・・承認依頼先リストに表示され、他のオペレータからの承認依頼を受け付けることができるようになります。<br />
															・ ロック強制解除 ・・・自分以外のオペレータによるロックを強制的に解除することができます。<br />
															・ 完全削除　　　・・・ごみ箱にあるインシデント、メッセージを完全削除できるようになります。<br />
															　　　　　　　　　　　 ごみ箱にある承認/送信代行、下書きは"完全削除"権限がなくても完全削除可能です。<br />
															・ 共通署名編集   ・・・全オペレータで共通して利用することができる署名の作成/編集を行うことができます。<br />
															・ 担当未設定警告 ・・・新しく追加された問合せが、一定期間を超えて担当者が設定されないままのときに通知されます。<br />
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnConfirm2" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
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
	<!--△ 入力 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
