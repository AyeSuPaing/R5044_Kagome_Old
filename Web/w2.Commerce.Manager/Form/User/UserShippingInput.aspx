<%--
=========================================================================================================
  Module      : ユーザアドレス入力ページ(UserShippingInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="ユーザー配送先情報入力" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="UserShippingInput.aspx.cs" Inherits="Form_User_UserShippingInput" %>
<%@ Import Namespace="Braintree" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" runat="Server">
	<style type="text/css">
		input.tel1, input.tel2, input.tel3 {width: 50px;}
		input.zipFirst {width: 50px;}
		input.zipLast {width: 70px;}
		.error_input{background-color:#ffaaaa!important}
		.error_inline{color: #ff0000;padding: 1px !important;display: block!important;}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<%-- UPDATE PANEL開始 --%>
	<table cellpadding="0" cellspacing="0">
			<tr>
				<td>
				<% if (Request[Constants.REQUEST_KEY_SHIPPING_NO] != null) {%>
					<h2 class="cmn-hed-h2">配送先情報編集</h2>
				<%} else {%>
					<h2 class="cmn-hed-h2">配送先情報登録</h2>
				<%} %>
				</td>
			</tr>
			<tr>
				<td>
					<table class="box_border" cellspacing="1" cellpadding="3" border="0">
						<tbody>
							<tr>
								<td>
									<table class="info_box_bg" cellspacing="0" cellpadding="0" border="0">
										<tbody>
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
											<tr>
												<td>
													<img width="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
												<td align="center">
													<table cellspacing="0" cellpadding="0" border="0">
														<tbody>
															<tr>
																<td>
																	<table class="info_table" cellpadding="3" cellspacing="1" border="0" width="658">
																		<tbody>
																			<tr class="info_item_bg">
																				<td align="left">
																					<p id="pRegistInfo" runat="server" visible="false">
																						アドレス帳に新しいお届け先を登録します。<br/>下のフォームに入力し、「確認する」ボタンを押してください。
																						登録する住所には、「配送先名」を登録<br />する事ができます。（例：「実家」、「お店」など）
																					</p>
																					<p id="pModifyInfo" runat="server" visible="false">
																						アドレス帳に登録されているお届け先を編集します。<br/>下のフォームに入力し、「確認する」ボタンを押してください。
																					</p>
																				</td>
																			</tr>
																		</tbody>
																	</table>
																	<br />
																</td>
															</tr>
															<tr>
																<td>
																	<asp:UpdatePanel ID="upUpdatePanel" runat="server">
																		<ContentTemplate>
																			<div id="divErrorMessage" runat="server" visible="false">
																				<table class="info_table" cellpadding="3" width="658" border="0" cellspacing="1">
																					<tr class="info_item_bg">
																						<td><asp:Label ID="lbErrorMessage" runat="server" ForeColor="Red"></asp:Label></td>
																					</tr>
																				</table>
																				<br />
																			</div>
																			<table cellspacing="1" cellpadding="3" width="658" border="0" class="edit_table">
																				<tr>
																					<td class="edit_title_bg">
																						配送先名<span class="notice">*</span>
																					</td>
																					<td class="edit_item_bg">
																						<asp:TextBox ID="tbName" runat="server" MaxLength="30"></asp:TextBox>
																					</td>
																				</tr>
																				<tr>
																					<%-- 氏名 --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.name.name@@") %><span class="notice">*</span>
																					</td>
																					<td class="edit_item_bg">
																						<table cellspacing="0" class="no-border">
																							<tr>
																								<td>
																									<%-- SetMaxLength(WtbShippingName1, "@@User.name1.length_max@@"); --%>
																									<span>姓</span><asp:TextBox ID="tbShippingName1" runat="server" CssClass="nameFirst"></asp:TextBox>
																								</td>
																								<td>
																									<%-- SetMaxLength(WtbShippingName2, "@@User.name2.length_max@@"); --%>
																									<span>名</span><asp:TextBox ID="tbShippingName2" runat="server" CssClass="nameLast"></asp:TextBox>
																								</td>
																							</tr>
																						</table>
																					</td>
																				</tr>
																				<% if (this.IsShippingAddrJp) { %>
																				<tr>
																					<%-- 氏名（かな） --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.name_kana.name@@") %><span
																							class="notice">*</span>
																					</td>
																					<td class="edit_item_bg">
																						<table cellspacing="0" class="no-border">
																							<tr>
																								<td>
																									<%-- SetMaxLength(WtbShippingNameKana1, "@@User.name_kana1.length_max@@"); --%>
																									<span class="fname">姓</span><asp:TextBox ID="tbShippingNameKana1" runat="server"
																										CssClass="nameFirst"></asp:TextBox>
																								</td>
																								<td>
																									<%-- SetMaxLength(WtbShippingNameKana2, "@@User.name_kana2.length_max@@"); --%>
																									<span class="lname">名</span><asp:TextBox ID="tbShippingNameKana2" runat="server"
																										CssClass="nameLast"></asp:TextBox>
																								</td>
																							</tr>
																						</table>
																					</td>
																				</tr>
																				<% } %>
																				<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																				<tr>
																					<%-- 国 --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.country.name@@", this.ShippingAddrCountryIsoCode) %>
																						<span class="notice">*</span>
																					</td>
																					<td class="edit_item_bg">
																						<asp:DropDownList ID="ddlShippingCountry" runat="server" DataTextField="Text" DataValueField="Value" AutoPostBack="true">
																						</asp:DropDownList>
																					</td>
																				</tr>
																				<% } %>
																				<% if (this.IsShippingAddrJp) { %>
																				<tr>
																					<%-- 郵便番号 --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.zip.name@@") %><span class="notice">*</span>
																					</td>
																					<td class="edit_item_bg">
																						<asp:TextBox ID="tbShippingZip1" MaxLength="3" runat="server" CssClass="zipFirst"></asp:TextBox>-
																						<asp:TextBox ID="tbShippingZip2" MaxLength="4" runat="server" CssClass="zipLast"></asp:TextBox>
																						<asp:Button ID="btnZipSearch" runat="server" OnClick="btnZipSearch_Click" Text="  住所検索  " /><br />
																						<%-- エラーメッセージ --%>
																						<% if (StringUtility.ToEmpty(this.ZipInputErrorMessage) != ""){%>
																						<span style="color: Red">
																							<%: this.ZipInputErrorMessage %></span>
																						<% } %>
																					</td>
																				</tr>
																				<tr>
																					<%-- 都道府県 --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.addr1.name@@") %><span class="notice">*</span>
																					</td>
																					<td class="edit_item_bg">
																						<asp:DropDownList ID="ddlShippingAddr1" runat="server" DataTextField="Text" DataValueField="Value">
																						</asp:DropDownList>
																					</td>
																				</tr>
																				<% } %>
																				<tr>
																					<%-- 市区町村 --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.addr2.name@@", this.ShippingAddrCountryIsoCode) %>
																						<span class="notice">*</span>
																					</td>
																					<td class="edit_item_bg">
																						<%-- SetMaxLength(WtbShippingAddr2, "@@User.addr2.length_max@@"); --%>
																						<asp:TextBox ID="tbShippingAddr2" runat="server" Width="300"></asp:TextBox>
																					</td>
																				</tr>
																				<tr>
																					<%-- 番地 --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.addr3.name@@", this.ShippingAddrCountryIsoCode) %>
																						<% if (this.IsShippingAddrJp) { %><span class="notice">*</span><% } %>
																					</td>
																					<td class="edit_item_bg">
																						<%-- SetMaxLength(WtbShippingAddr3, "@@User.addr3.length_max@@"); --%>
																						<asp:TextBox ID="tbShippingAddr3" runat="server" Width="300"></asp:TextBox>
																					</td>
																				</tr>
																				<tr>
																					<%-- ビル・マンション名 --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.addr4.name@@", this.ShippingAddrCountryIsoCode) %>
																						<% if (this.IsShippingAddrJp == false) { %><span class="notice">*</span><% } %>
																					</td>
																					<td class="edit_item_bg">
																						<asp:TextBox ID="tbShippingAddr4" runat="server" Width="300"></asp:TextBox>
																					</td>
																				</tr>
																				<% if (this.IsShippingAddrJp == false) { %>
																				<tr>
																					<%-- 州 --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.addr5.name@@", this.ShippingAddrCountryIsoCode) %>
																						<% if (this.IsShippingAddrUs) { %><span class="notice">*</span><% } %>
																					</td>
																					<td class="edit_item_bg">
																						<% if (this.IsShippingAddrUs) { %>
																						<asp:DropDownList ID="ddlShippingAddr5" runat="server"></asp:DropDownList>
																						<% } else { %>
																						<asp:TextBox ID="tbShippingAddr5" runat="server" Width="300"></asp:TextBox>
																						<% } %>
																					</td>
																				</tr>
																				<tr>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.zip.name@@", this.ShippingAddrCountryIsoCode) %>
																						<% if (this.IsShippingAddrZipNecessary) { %><span class="notice">*</span><% } %>
																					</td>
																					<td class="edit_item_bg">
																						<asp:TextBox ID="tbShippingZipGlobal" MaxLength="20" runat="server" />
																						<asp:LinkButton
																							ID="lbSearchAddrFromZipGlobal"
																							OnClick="lbSearchAddrFromZipGlobal_Click"
																							Style="display:none;"
																							runat="server" />
																					</td>
																				</tr>
																				<% } %>
																				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
																				<tr>
																					<%-- 企業名 --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.company_name.name@@")%><span
																							class="notice"></span>
																					</td>
																					<td class="edit_item_bg">
																						<asp:TextBox ID="tbShippingCompanyName" runat="server" CssClass="addr2"></asp:TextBox>
																					</td>
																				</tr>
																				<tr>
																					<%-- 部署名 --%>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.company_post_name.name@@")%><span class="notice"></span>
																					</td>
																					<td class="edit_item_bg">
																						<%-- SetMaxLength(WtbShippingCompanyPostName, "@@User.company_post_name.length_max@@"); --%>
																						<asp:TextBox ID="tbShippingCompanyPostName" runat="server" CssClass="addr2"></asp:TextBox>
																					</td>
																				</tr>
																				<%} %>
																				<tr>
																					<%-- 電話番号 --%>
																					<% if (this.IsShippingAddrJp) { %>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.tel1.name@@") %><span class="notice">*</span>
																					</td>
																					<td class="edit_item_bg">
																						<asp:TextBox ID="tbShippingTel1_1" MaxLength="6" runat="server" CssClass="tel1"></asp:TextBox>-
																						<asp:TextBox ID="tbShippingTel1_2" MaxLength="4" runat="server" CssClass="tel2"></asp:TextBox>-
																						<asp:TextBox ID="tbShippingTel1_3" MaxLength="4" runat="server" CssClass="tel3"></asp:TextBox>
																					</td>
																					<% } else { %>
																					<td class="edit_title_bg">
																						<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %>
																						<span class="notice">*</span>
																					</td>
																					<td class="edit_item_bg">
																						<asp:TextBox ID="tbShippingTel1Global" MaxLength="30" runat="server"></asp:TextBox>
																					</td>
																					<% } %>
																				</tr>
																			</table>
																			<table cellspacing="0" cellpadding="5" width="658" border="0">
																				<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
																				<tr>
																					<td align="right">
																						<asp:Button ID="btnReset" runat="server" Text="  リセット  " onclick="btnReset_Click" />
																						<asp:Button ID="btnComfirm" runat="server" Text="  確認する  " onclick="btnComfirm_Click" />
																					</td>
																				</tr>
																				<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
																			</table>
																		</ContentTemplate>
																	</asp:UpdatePanel>
																</td>
															</tr>
														</tbody>
													</table>
												</td>
												<td>
													<img width="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
										</tbody>
									</table>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</table>
	<%-- UPDATE PANELここまで --%>
	<br/>
	<script type="text/javascript">
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		// Textbox change search zip global
		textboxChangeSearchGlobalZip(
			'<%= tbShippingZipGlobal.ClientID %>',
			'<%= lbSearchAddrFromZipGlobal.UniqueID %>');

		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(
			function () {
				// Textbox change search zip global
				textboxChangeSearchGlobalZip(
					'<%= tbShippingZipGlobal.ClientID %>',
					'<%= lbSearchAddrFromZipGlobal.UniqueID %>');
			});
		<% } %>
	</script>
</asp:Content>
