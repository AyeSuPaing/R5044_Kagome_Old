<%--
=========================================================================================================
  Module      : 回答例文章設定確認ページ(AnswerTemplateConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AnswerTemplateConfirm.aspx.cs" Inherits="Form_AnswerTemplate_AnswerTemplateConfirm" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">回答例文章設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">回答例文章設定詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">回答例文章設定確認</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="0" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table cellspacing="1" cellpadding="0" width="100%" border="0">
							<tr>
								<td>
									<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td align="center">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<div class="action_part_top">
																<input type="button" onclick="Javascript:history.back();" value =" 　戻る 　" />
																<asp:button id="btnEditTop" runat="server" Text="　編集する　" Visible="False" onclick="btnEdit_Click"></asp:button>
																<asp:button id="btnCopyInsertTop" runat="server" Text=" コピー新規登録する " Visible="False" onclick="btnCopyInsert_Click"></asp:button>
																<asp:button id="btnDeleteTop" runat="server" Text="　削除する　" Visible="False" onclick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');"></asp:button>
																<asp:button id="btnInsertTop" runat="server" Text="　登録する　" Visible="False" onclick="btnInsert_Click"></asp:button>
																<asp:button id="btnUpdateTop" runat="server" Text="　更新する　" Visible="False" Width="72px" onclick="btnUpdate_Click"></asp:button>
															</div>
															<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tr>
																	<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left" width="30%">回答例カテゴリ</td>
																	<td class="detail_item_bg" align="left"><%: this.AnswerTemplateInfo.EX_CategoryName %></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left" width="30%">回答例タイトル</td>
																	<td class="detail_item_bg" align="left"><%: this.AnswerTemplateInfo.AnswerTitle %></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left" width="30%">回答例件名</td>
																	<td class="detail_item_bg" align="left"><%: this.AnswerTemplateInfo.AnswerMailTitle %></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left" width="30%">回答例本文</td>
																	<td class="detail_item_bg" align="left"><%= WebSanitizer.HtmlEncodeChangeToBr(this.AnswerTemplateInfo.AnswerText) %></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left" width="30%">表示順</td>
																	<td class="detail_item_bg" align="left"><%: this.AnswerTemplateInfo.DisplayOrder %></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left" width="30%">有効フラグ</td>
																	<td class="detail_item_bg" align="left"><%: ValueText.GetValueText(Constants.TABLE_CSANSWERTEMPLATE, Constants.FIELD_CSANSWERTEMPLATE_VALID_FLG, this.AnswerTemplateInfo.ValidFlg) %></td>
																</tr>
																<tr id="trDateCreated" runat="server" Visible="False">
																	<td class="detail_title_bg" align="left" width="30%">作成日</td>
																	<td class="detail_item_bg" align="left"><%: DateTimeUtility.ToStringForManager(this.AnswerTemplateInfo.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																</tr>
																<tr id="trDateChanged" runat="server" Visible="False">
																	<td class="detail_title_bg" align="left" width="30%">更新日</td>
																	<td class="detail_item_bg" align="left"><%: DateTimeUtility.ToStringForManager(this.AnswerTemplateInfo.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																</tr>
																<tr id="trLastChanged" runat="server" Visible="False">
																	<td class="detail_title_bg" align="left" width="30%">最終更新者</td>
																	<td class="detail_item_bg" align="left"><%: this.AnswerTemplateInfo.LastChanged %></td>
																</tr>
															</table>
															<div class="action_part_bottom">
																<input type="button" onclick="Javascript:history.back();" value=" 　戻る 　" />
																<asp:button id="btnEditBottom" runat="server" Text="　編集する　" Visible="False" onclick="btnEdit_Click"></asp:button>
																<asp:button id="btnCopyInsertBottom" runat="server" Text=" コピー新規登録する " Visible="False" onclick="btnCopyInsert_Click"></asp:button>
																<asp:button id="btnDeleteBottom" runat="server" Text="　削除する　" Visible="False" onclick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');"></asp:button>
																<asp:button id="btnInsertBottom" runat="server" Text="　登録する　" Visible="False" onclick="btnInsert_Click"></asp:button>
																<asp:button id="btnUpdateBottom" runat="server" Text="　更新する　" Visible="False" Width="72px" onclick="btnUpdate_Click"></asp:button>
															</div>
														</td>
													</tr>
													<tr>
														<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
			</table>
		</td>
	</tr>
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>