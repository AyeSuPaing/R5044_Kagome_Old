<%--
=========================================================================================================
  Module      : ユーザー拡張項目設定ページ(ProductExtendSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductExtendSettingList.aspx.cs" Inherits="Form_ProductExtendSetting_ProductExtendSettingList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">商品拡張項目設定</h1>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr id="trList" runat="server">
		<td><h2 class="cmn-hed-h2">商品拡張項目設定一覧</h2></td>
	</tr>
	<tr>
		<!--一覧-->
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<!--実際のコンテンツ部分-->
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<!--編集ページ-->
									<div id="divExtendEdit" runat="server">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td height="6" colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td>
											</tr>
											<tr>
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr id="trExtendComplete" runat="server" visible="false">
												<td>
													<table class="info_table" cellspacing="1" cellpadding="3" width="400" border="0">
														<tr class="info_item_bg">
															<td align="left">商品拡張項目を登録/更新しました。
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<!-- 一括更新ボタン -->
												<td align="right"><asp:Button ID="btnAllUpdateTop" runat="server" Text="  一括更新する  " OnClick="btnAllUpdate_Click" OnClientClick="return check_delete_fields_confirm();" /></td>
												<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
											</tr>
											<tr>
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr>
												<td valign="top">
													
													<!-- ***********************拡張項目リスト*********************** -->
													<%
														for (int i = 0; i < rExtendList.Items.Count; i++)
														{
															// 出力フォーマットにonfocusイベントをセット
															((TextBox)(rExtendList.Items[i].FindControl("tbExtendName"))).Attributes.Add("onfocus",
															   "javascript:listselect_mout1(document.getElementById('AdColumnRow' + document.getElementById('" + hdnAdColumnSelected.ClientID + "').value ) );" + 
															   "document.getElementById('" + hdnAdColumnSelected.ClientID + "').value=" + i + ";document.getElementById('" + hdnAdColumnFocus.ClientID + "').value=this.id;"+
															   "listselect_mover(document.getElementById('AdColumnRow" + i + "'));");
															
															((TextBox)(rExtendList.Items[i].FindControl("tbExtendDiscription"))).Attributes.Add("onfocus",
															   "javascript:listselect_mout1(document.getElementById('AdColumnRow' + document.getElementById('" + hdnAdColumnSelected.ClientID + "').value ) );" +
															   "document.getElementById('" + hdnAdColumnSelected.ClientID + "').value=" + i + ";document.getElementById('" + hdnAdColumnFocus.ClientID + "').value=this.id;" +
															   "listselect_mover(document.getElementById('AdColumnRow" + i + "'));");
														}
													 %>
													<asp:HiddenField ID="hdnAdColumnSelected" Value="0" runat="server"/>
													<asp:HiddenField ID="hdnAdColumnFocus" Value="" runat="server"/>
													<asp:Repeater id="rExtendList" Runat="server">
														<ItemTemplate>
															<%-- 任意の行数ごとにヘッダーをつける --%>
															<%-- 任意の行数ごとにヘッダーをつける --%>
															<% this.ExtendListNoTmp++; %>
															<%if ((this.ExtendListNoTmp == 1) || (this.ExtendListNoTmp == 51) || (this.ExtendListNoTmp == 101)) { %>
															<table class="list_table" cellSpacing="1" cellPadding="3" width="758" border="0" align="center">
															<tr class="list_title_bg">
																<td align="center" colspan="3">商品拡張項目(格納可能文字数：<% if (this.ExtendListNoTmp == 1) {%>MAX10文字<%} %><% if (this.ExtendListNoTmp == 51) {%>MAX30文字<%} %><% if (this.ExtendListNoTmp == 101) {%>制限なし<%} %>)</td>
															</tr>
															<tr class="list_title_bg">
																<td align="right" colspan="3"><input type="hidden" value="0" id="hdnExtend<%: this.ExtendListNoTmp %>"><a href="Javascript:DisplayPage('tbodyExtend<%: this.ExtendListNoTmp %>', 'hdnExtend<%: this.ExtendListNoTmp %>')">開く /  閉じる</a>&nbsp;</td>
															</tr>
															<tbody id="tbodyExtend<%: this.ExtendListNoTmp %>">
															<tr class="list_title_bg">
																<td with="50" align="center">No.</td>
																<td with="180" align="left">拡張項目名</td>
																<td with="520" align="left">拡張項目説明</td>
															</tr>
															<%} %>
															<tr class="list_item_bg1" id="AdColumnRow<%# Container.ItemIndex %>">
																<td align="center"><%: this.ExtendListNoTmp %></td>
																<td align="left">
																	<asp:TextBox ID="tbExtendName" runat="server" Width="180" MaxLength="30" Text='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME] %>'></asp:TextBox>
																	<asp:HiddenField ID="hdnExtendNo" runat="server" Value='<%# Container.ItemIndex + 1 %>' />
																	<asp:HiddenField ID="hdnExtendNameBefore" runat="server" Value='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME + "_Escaped"] %>' />
																	<asp:HiddenField ID="hdnExtendDiscriptionBefore" runat="server" Value='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_DISCRIPTION] %>' />
																</td>
																<td align="left"><asp:TextBox ID="tbExtendDiscription" runat="server" TextMode="MultiLine" Width="500" Rows="2" Text='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_DISCRIPTION] %>'></asp:TextBox></td>
															</tr>
															<%-- 任意の行数ごとにフッターをつける --%>
															<%if ((this.ExtendListNoTmp == 50) || (this.ExtendListNoTmp == 100)) { %></tbody></table><br /><%} %>
															<%if ((this.ExtendListNoTmp == 140)) { %></tbody></table><%} %>
														</ItemTemplate>
													</asp:Repeater>
													<!-- **********************拡張項目リスト終わり*********************** -->													
												</td>
												<td><img src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
											</tr>
											<tr>
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr>
												<!-- 一括更新ボタン -->
												<td align="right"><asp:Button ID="btnAllUpdateBottom" runat="server" Text="  一括更新する  " OnClick="btnAllUpdate_Click" OnClientClick="return check_delete_fields_confirm();" /></td>
												<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
											</tr>
											<tr>
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
										</table>
									</div>
								</td>
							</tr>
						</table>
						<!--実際のコンテンツ部分終わり-->
					</td>
				</tr>
			</table>
		</td>
		<!--一覧終わり-->
	</tr>
	<tr>
		<!--スペーサー-->
		<td><img alt="" src="../../Images/Common/sp.gif" width="1" height="10" border="0" /></td>
		<!--スペーサー終わり-->
	</tr>
</table>
<script type="text/javascript">
<!--
//=============================================================================================
// ページ開く/閉じる
//=============================================================================================
function DisplayPage(tagid, hdntagid)
{
	// 開く/閉じる更新状態保持用HIDDEN更新
	document.getElementById(hdntagid).value = (document.getElementById(tagid).style.display == "none") ? "1" : "0";
	
	document.getElementById(tagid).style.display = (document.getElementById(hdntagid).value == "1") ? "" : "none";
}
//=============================================================================================
// 更新確認Confirmダイアログ生成（削除するフィールドがあれば番号を出力する）
//=============================================================================================
function check_delete_fields_confirm()
{
	var strDeleteFields = "";

	<%for (int iLoop = 0; iLoop < PRODUCT_EXTEND_COUNT; iLoop++) { %>
		strDeleteFields += ((document.getElementById('<%= rExtendList.Items[iLoop].FindControl("tbExtendName").ClientID %>').value == '') && (document.getElementById('<%= rExtendList.Items[iLoop].FindControl("hdnExtendNameBefore").ClientID %>').value != '')) ? ((strDeleteFields.length != 0) ? ", " : "") + "<%= iLoop+1 + ":" + ((HiddenField)rExtendList.Items[iLoop].FindControl("hdnExtendNameBefore")).Value %>" : "";
	<%} %>
	
	var strMessage = (strDeleteFields.length != 0) ? 'クリアされたフィールドは実データも削除されます。：' + strDeleteFields : '表示内容で更新します。';
	return confirm(strMessage + "\nよろしいですか？");
}
//-->
</script>
</asp:Content>

