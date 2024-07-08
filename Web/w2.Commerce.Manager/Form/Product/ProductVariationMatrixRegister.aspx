<%--
=========================================================================================================
  Module      : 商品バリエーション情報一括設定ページ(ProductVariationMatrixRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Common.Util" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master"AutoEventWireup="true" CodeFile="ProductVariationMatrixRegister.aspx.cs" Inherits="Form_Product_ProductVariationMatrixRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="618" border="0">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">商品バリエーション一括登録</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="698" border="0">				
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tbody id="tbdyVariationMatrixErrorMessages" visible="false" runat="server">
											<tr>
												<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
											</tr>
											<tr>
												<td class="edit_item_bg" align="left" colspan="4">
													<asp:Label ID="lbVariationMatrixErrorMessages" runat="server" ForeColor="red"></asp:Label>
												</td>
											</tr>
										</tbody>
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td align="left">
												<asp:RadioButtonList id="rblVariationType" AutoPostBack="true" 
													RepeatDirection="Horizontal" runat="server" 
													onselectedindexchanged="rblVariationType_SelectedIndexChanged">
													<asp:ListItem Text="楽天 i" Value="rakuten_i" Selected="True"></asp:ListItem>
													<asp:ListItem Text="楽天 s" Value="rakuten_s"></asp:ListItem>
													<asp:ListItem Text="楽天 c" Value="rakuten_c"></asp:ListItem>
												</asp:RadioButtonList>
											</td>
											<td></td>
											<td align="right">
												<asp:RadioButtonList id="rblReplaceOrAdd" RepeatDirection="Horizontal" runat="server">
													<asp:ListItem Text="入れ替え" Value="replace" Selected="True"></asp:ListItem>
													<asp:ListItem Text="  追加  " Value="add"></asp:ListItem>
												</asp:RadioButtonList>
											</td>
										</tr>
										<%-- バリエーションタイプ「i」用2列表示 --%>
										<tr id="trVariationTypeI" runat="server">
											<td>
												<asp:Repeater id="rRakutenTypeI_Vertical" runat="server">
												<HeaderTemplate>
													<table class="detail_table" cellspacing="1" cellpadding="3" width="325" border="0">
														<tr>
															<td class="detail_title_bg" align="center" width="100%" colspan="2">横軸</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="center" width="50%">商品ID1</td>
															<td class="detail_title_bg" align="center" width="50%">表示名1</td>
														</tr>
												</HeaderTemplate>
												<ItemTemplate>
													<tr>
														<td class="detail_item_bg" align="center" width="50%">
															<asp:TextBox ID="tbMallVariationId1" runat="server"></asp:TextBox>
														</td>
														<td class="detail_item_bg" align="center" width="50%">
															<asp:TextBox ID="tbVariationName1" runat="server"></asp:TextBox>
														</td>
													</tr>
												</ItemTemplate>
												<FooterTemplate>
													</table>
												</FooterTemplate>
												</asp:Repeater>
											</td>
											<td>
												<img alt="" src="../../Images/Common/sp.gif" width="5" border="0" />
											</td>
											<td>
												<asp:Repeater id="rRakutenTypeI_Horizonal" runat="server">
												<HeaderTemplate>
													<table class="detail_table" cellspacing="1" cellpadding="3" width="325" border="0">
														<tr>
															<td class="detail_title_bg" align="center" width="100%" colspan="2">縦軸</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="center" width="50%">商品ID2</td>
															<td class="detail_title_bg" align="center" width="50%">表示名2</td>
														</tr>
												</HeaderTemplate>
												<ItemTemplate>
													<tr>
														<td class="detail_item_bg" align="center" width="50%">
															<asp:TextBox ID="tbMallVariationId2" runat="server"></asp:TextBox>
														</td>
														<td class="detail_item_bg" align="center" width="50%">
															<asp:TextBox ID="tbVariationName2" runat="server"></asp:TextBox>
														</td>
													</tr>
												</ItemTemplate>
												<FooterTemplate>
													</table>
												</FooterTemplate>
												</asp:Repeater>
											</td>
										</tr>
										<%-- バリエーションタイプ「s」「c」用2列表示 --%>
										<tr id="trVariationTypeSC" runat="server">
											<td>
												<asp:Repeater id="rRakutenTypeSC1" runat="server">
												<HeaderTemplate>
													<table class="detail_table" cellspacing="1" cellpadding="3" width="325" border="0">
														<tr>
															<td class="detail_title_bg" align="center" width="50%">商品ID1</td>
															<td class="detail_title_bg" align="center" width="50%">表示名1</td>
														</tr>
												</HeaderTemplate>
												<ItemTemplate>
													<tr>
														<td class="detail_item_bg" align="center" width="50%">
															<asp:TextBox ID="tbMallVariationId1" runat="server"></asp:TextBox>
														</td>
														<td class="detail_item_bg" align="center" width="50%">
															<asp:TextBox ID="tbVariationName1" runat="server"></asp:TextBox>
														</td>
													</tr>
												</ItemTemplate>
												<FooterTemplate>
													</table>
												</FooterTemplate>
												</asp:Repeater>
											</td>
											<td>
												<img alt="" src="../../Images/Common/sp.gif" width="5" border="0" />
											</td>
											<td>
												<asp:Repeater id="rRakutenTypeSC2" runat="server">
												<HeaderTemplate>
													<table class="detail_table" cellspacing="1" cellpadding="3" width="325" border="0">
														<tr>
															<td class="detail_item_bg" align="center" width="50%">（商品ID1 続き）</td>
															<td class="detail_item_bg" align="center" width="50%">（表示名1 続き）</td>
														</tr>
												</HeaderTemplate>
												<ItemTemplate>
													<tr>
														<td class="detail_item_bg" align="center" width="50%">
															<asp:TextBox ID="tbMallVariationId1" runat="server"></asp:TextBox>
														</td>
														<td class="detail_item_bg" align="center" width="50%">
															<asp:TextBox ID="tbMallVariationId2" runat="server"></asp:TextBox>
														</td>
													</tr>
												</ItemTemplate>
												<FooterTemplate>
													</table>
												</FooterTemplate>
												</asp:Repeater>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td colspan="3">
												<table class="detail_table" cellspacing="1" cellpadding="3" width="650" border="0">
													<tr>
														<td class="detail_title_bg" align="center" width="25%">販売価格</td>
														<td class="detail_item_bg" align="left" width="25%"><asp:TextBox ID="tbPrice" runat="server"></asp:TextBox></td>
														<td class="detail_title_bg" align="center" width="25%">特別価格</td>
														<td class="detail_item_bg" align="left" width="25%"><asp:TextBox ID="tbSpecialPrice" runat="server"></asp:TextBox></td>
													</tr>
												</table>												
												<div class="action_part_bottom">
													<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_VARIATION_MATRIX_REGISTER %>">初期値に戻す</a>　
													<asp:Button ID="btnConfirmBottom" Text="  設定する  " runat="server" OnClick="btnConfirm_Click" />
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
	<!--△ 一覧 △-->
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<div id="divExecScript" visible="false" runat="server">
<%
	// バリエーション情報のカンマ区切りの文字列（パラメタ列）の作成
	StringBuilder sbVariationInfos = new StringBuilder();
	foreach (string strInfo in this.OutputParams)
	{
		if (sbVariationInfos.Length != 0)
		{
			sbVariationInfos.Append(", ");
		}
		sbVariationInfos.Append("'").Append(strInfo.Replace(@"\", @"\\").Replace("'", @"\'")).Append("'");
	}
%>
<script type="text/javascript">
// メッセージ作成
var strMessage = null;
<%
	if (rblReplaceOrAdd.SelectedValue == "replace")
	{
		%>strMessage = '既定のバリエーション情報は上書きされます。よろしいですか？';<%
	}
	else
	{
		%>strMessage = '既定のバリエーション情報に追加されます。よろしいですか？';<%
	}
%>

// 命令作成
if (confirm(strMessage))
{
	if (window.opener != null)
	{
		window.opener.action_variation_setting(<%= sbVariationInfos.ToString() %>);
		window.close();
	}
}
</script>
</div>
</asp:Content>
