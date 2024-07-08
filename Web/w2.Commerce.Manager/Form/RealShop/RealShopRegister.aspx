<%--
=========================================================================================================
  Module      : リアル店舗情報登録ページ(RealShopRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="RealShopRegister.aspx.cs" Inherits="Form_RealShop_RealShopRegister" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td><h1 class="page-title">リアル店舗情報</h1></td>
		</tr>
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<tr id="trRegistTitle" runat="server" visible="false">
			<td><h2 class="cmn-hed-h2">リアル店舗情報登録</h2></td>
		</tr>
		<tr id="trEditTitle" runat="server" visible="false">
			<td><h2 class="cmn-hed-h2">リアル店舗情報編集</h2></td>
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
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
											<tr>
												<td>
													<div id="divComp" runat="server" class="action_part_top" visible="false">
														<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
															<tr class="info_item_bg">
																<td align="left">
																	リアル店舗情報を登録/更新しました。
																</td>
															</tr>
														</table>
													</div>
													<div class="action_part_top">
														<asp:Button ID="btnBackListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnBackList_Click"/>
														<asp:Button ID="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
														<asp:Button ID="btnDeleteTop" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
														<asp:Button ID="btnInsertTop" runat="server" Text="  登録する  " OnClick="btnInsertUpdate_Click" />
														<asp:Button ID="btnUpdateTop" runat="server" Text="  更新する  " OnClick="btnInsertUpdate_Click" />
													</div>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">
																基本情報
															</td>
														</tr>
														<tr id="trInputRealShopId" runat="server" visible="false">
															<td class="edit_title_bg" align="left" width="25%">
																リアル店舗ID<span class="notice">*</span>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbRealShopId" MaxLength="30" runat="server" Width="200"></asp:TextBox>
															</td>
														</tr>
														<tr id="trDispRealShopId" runat="server" visible="false">
															<td class="edit_title_bg" align="left" width="25%">
																リアル店舗ID
															</td>
															<td class="edit_item_bg" align="left">
																<asp:Literal ID="lRealShopId" runat="server"></asp:Literal>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																店舗名<span class="notice">*</span>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbRealShopName" MaxLength="100" runat="server" Width="500"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																店舗名かな
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbRealShopNameKana" MaxLength="200" runat="server" Width="500"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																地域<span class="notice">*</span>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList ID="ddlSettingArea" AutoPostBack="true" runat="server" />
															</td>
														</tr>
														<% if (Constants.PRODUCT_BRAND_ENABLED) { %>
														<tr>
															<td class="edit_title_bg" align="left">
																ブランド
															</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList ID="ddlSettingBrand" AutoPostBack="true" runat="server" />
															</td>
														</tr>
														<% } %>
														<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
														<tr>
															<td class="edit_title_bg" align="left">
																<%: ReplaceTag("@@User.country.name@@", this.ShopAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList ID="ddlCountry" runat="server" AutoPostBack="true"></asp:DropDownList>
															</td>
														</tr>
														<% } %>
														<% if (this.IsShopAddrJp) { %>
														<tr>
															<td class="edit_title_bg" align="left">
																郵便番号
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbZip1" Width="50" MaxLength="3" runat="server"></asp:TextBox>
																-
																<asp:TextBox ID="tbZip2" Width="70" MaxLength="4" runat="server"></asp:TextBox>
																<asp:Button ID="btnZipSearch" Text="  住所検索  " runat="server" OnClick="btnZipSearch_Click" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																都道府県
															</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList ID="ddlAddr1" runat="server"></asp:DropDownList>
															</td>
														</tr>
														<% } %>
														<tr>
															<td class="edit_title_bg" align="left">
																<%: ReplaceTag("@@User.addr2.name@@", this.ShopAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbAddr2" MaxLength="100" Width="500" runat="server"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																<%: ReplaceTag("@@User.addr3.name@@", this.ShopAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbAddr3" MaxLength="100" Width="500" runat="server"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																<%: ReplaceTag("@@User.addr4.name@@", this.ShopAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbAddr4" MaxLength="100" Width="500" runat="server"></asp:TextBox>
															</td>
														</tr>
														<% if (this.IsShopAddrJp == false) { %>
														<tr>
															<td class="edit_title_bg" align="left">
																<%: ReplaceTag("@@User.addr5.name@@", this.ShopAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left">
																<% if (this.IsShopAddrUs) { %>
																<asp:DropDownList ID="ddlAddr5" runat="server"></asp:DropDownList>
																<% } else { %>
																<asp:TextBox ID="tbAddr5" MaxLength="100" Width="500" runat="server"></asp:TextBox>
																<% } %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																<%: ReplaceTag("@@User.zip.name@@", this.ShopAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbZipGlobal" Width="150" MaxLength="20" runat="server" />
																<asp:LinkButton
																	ID="lbSearchAddrFromZipGlobal"
																	OnClick="lbSearchAddrFromZipGlobal_Click"
																	Style="display:none;"
																	runat="server" />
															</td>
														</tr>
														<% } %>
														<tr>
															<% if (this.IsShopAddrJp) { %>
															<td class="edit_title_bg" align="left">
																電話番号
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbTel1_1" MaxLength="6" Width="40" runat="server"></asp:TextBox>
																-
																<asp:TextBox ID="tbTel1_2" MaxLength="4" Width="40" runat="server"></asp:TextBox>
																-
																<asp:TextBox ID="tbTel1_3" MaxLength="4" Width="40" runat="server"></asp:TextBox>
															</td>
															<% } else { %>
															<td class="edit_title_bg" align="left">
																<%: ReplaceTag("@@User.tel1.name@@", this.ShopAddrCountryIsoCode) %>
															</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbTel1Global" MaxLength="30" Width="150" runat="server"></asp:TextBox>
															</td>
															<% } %>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																FAX
															</td>
															<td class="edit_item_bg" align="left">
																<% if (this.IsShopAddrJp) { %>
																<asp:TextBox ID="tbFax1_1" MaxLength="6" Width="40" runat="server"></asp:TextBox>
																-
																<asp:TextBox ID="tbFax1_2" MaxLength="4" Width="40" runat="server"></asp:TextBox>
																-
																<asp:TextBox ID="tbFax1_3" MaxLength="4" Width="40" runat="server"></asp:TextBox>
																<% } else { %>
																<asp:TextBox ID="tbFax1Global" MaxLength="30" Width="150" runat="server"></asp:TextBox>
																<% } %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																経緯度
															</td>
															<td class="edit_item_bg" align="left">
																<span>緯度</span>
																<asp:TextBox ID="tbLatitude" Width="100" runat="server" />
																<span>経度</span>
																<asp:TextBox ID="tbLongitude" Width="100" runat="server" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg">
																メールアドレス
																<% if (Constants.STORE_PICKUP_OPTION_ENABLED) { %>
																<span class="notice">*</span>
																<% } %>
															</td>
															<td class="edit_item_bg">
																<asp:TextBox ID="tbMailAddr" runat="server" Width="300"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg">
																URL
															</td>
															<td class="edit_item_bg">
																<asp:TextBox ID="tbUrl" MaxLength="256" runat="server" Width="500"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg">
																営業時間
															</td>
															<td class="edit_item_bg">																
																<asp:TextBox ID="tbOpeningHours" runat="server" TextMode="MultiLine" Width="500" Height="50"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg">
																表示順<span class="notice">*</span>
															</td>
															<td class="edit_item_bg">
																<asp:TextBox ID="tbDisplayOrder" MaxLength="5" runat="server" Width="50"></asp:TextBox>
																&nbsp;フロントのリアル店舗を表示する際に、表示順昇順＋リアル店舗ID昇順で表示されます。
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																有効フラグ
															</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBox ID="cbValidFlg" runat="server" Text="有効" />
															</td>
														</tr>
													</table>
													<br />
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">
																説明
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="25%">
																説明1（PC）
															</td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButtonList ID="rblDesc1KbnPC" RepeatLayout="Flow" RepeatDirection="Horizontal"
																	runat="server">
																</asp:RadioButtonList>
																<input type= "button" onclick="javascript:open_wysiwyg('<%= tblDesc1Pc.ClientID %>', '<%= rblDesc1KbnPC.ClientID %>');" value="  HTMLエディタ  " /><br />
																<asp:TextBox ID="tblDesc1Pc" runat="server" TextMode="MultiLine" Width="500" Height="100"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																説明2（PC）
															</td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButtonList ID="rblDesc2KbnPC" RepeatLayout="Flow" RepeatDirection="Horizontal"
																	runat="server">
																</asp:RadioButtonList>
																<input type= "button" onclick="javascript:open_wysiwyg('<%= tblDesc2Pc.ClientID %>', '<%= rblDesc2KbnPC.ClientID %>');" value="  HTMLエディタ  " /><br />
																<asp:TextBox ID="tblDesc2Pc" runat="server" TextMode="MultiLine" Width="500" Height="100"></asp:TextBox>
															</td>
														</tr>
														<% if (Constants.SMARTPHONE_OPTION_ENABLED){  %>
														<tr>
															<td class="edit_title_bg" align="left">
																説明1（スマートフォン）
															</td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButtonList ID="rblDesc1KbnSP" RepeatLayout="Flow" RepeatDirection="Horizontal"
																	runat="server">
																</asp:RadioButtonList>
																<input type= "button" onclick="javascript:open_wysiwyg('<%= tblDesc1Sp.ClientID %>', '<%= rblDesc1KbnSP.ClientID %>');" value="  HTMLエディタ  " /><br />
																<asp:TextBox ID="tblDesc1Sp" runat="server" TextMode="MultiLine" Width="500" Height="100"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">
																説明2（スマートフォン）
															</td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButtonList ID="rblDesc2KbnSP" RepeatLayout="Flow" RepeatDirection="Horizontal"
																	runat="server">
																</asp:RadioButtonList>
																<input type= "button" onclick="javascript:open_wysiwyg('<%= tblDesc2Sp.ClientID %>', '<%= rblDesc2KbnSP.ClientID %>');" value="  HTMLエディタ  " /><br />
																<asp:TextBox ID="tblDesc2Sp" runat="server" TextMode="MultiLine" Width="500" Height="100"></asp:TextBox>
															</td>
														</tr>
														<% }  %>
													</table>
													<div class="action_part_bottom">
														<asp:Button ID="btnBackListBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnBackList_Click" />
														<asp:Button ID="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
														<asp:Button ID="btnDeleteBottom" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
														<asp:Button ID="btnInsertBottom" runat="server" Text="  登録する  " OnClick="btnInsertUpdate_Click" />
														<asp:Button ID="btnUpdateBottom" runat="server" Text="  更新する  " OnClick="btnInsertUpdate_Click" />
													</div>
												</td>
											</tr>
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
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
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
	<script type="text/javascript">
		function disabled_input() {
			var kbn = getParam('window_kbn');
			if (kbn === '1') {
				$(":input").attr("disabled", "");
			};
		}

		/**
		 * Get the URL parameter value
		 *
		 * @param  name {string} パラメータのキー文字列
		 * @return  url {url} 対象のURL文字列（任意）
		 */
		function getParam(name, url) {
			if (!url) url = window.location.href;
			name = name.replace(/[\[\]]/g, "\\$&");
			var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
				results = regex.exec(url);
			if (!results) return null;
			if (!results[2]) return '';
			return decodeURIComponent(results[2].replace(/\+/g, " "));
		}

		disabled_input();

		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		// Textbox change search zip global
		textboxChangeSearchGlobalZip(
			'<%= tbZipGlobal.ClientID %>',
			'<%= lbSearchAddrFromZipGlobal.UniqueID %>');
		<% } %>
	</script>
	</table>
</asp:Content>