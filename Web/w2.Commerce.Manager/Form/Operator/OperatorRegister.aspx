<%--
=========================================================================================================
  Module      : オペレータ情報登録ページ(OperatorRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OperatorRegister.aspx.cs" Inherits="Form_Operator_OperatorRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">オペレータ情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 編集入力 ▽-->
	<tr>
		<td>
			<h2 class="cmn-hed-h2">
				<% if(Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT){%>
					オペレータ情報登録
				<%} %>
				<% if(Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE){%>
					オペレータ情報編集
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
														<td class="edit_title_bg" align="left" width="30%">オペレータID</td>
														<td class="edit_item_bg" align="left"><asp:Label ID="lbOperatorId" runat="server"></asp:Label></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">オペレータ名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbOperatorName" runat="server" MaxLength="20" Width="180" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メニュー権限<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlMenuAccessLevel" Runat="server"></asp:DropDownList></td>
													</tr>
													<% if (Constants.TWO_STEP_AUTHENTICATION_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メールアドレス<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbMailAddress" runat="server" MaxLength="256" Width="150" /></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">ログインＩＤ<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbLoginId" runat="server" MaxLength="16" Width="150" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">パスワード<span class="notice">*</span><br /><%= WebSanitizer.HtmlEncode(trOperatorId.Visible ? "（変更する場合のみ入力してください）" : "") %></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbPassWord" runat="server" MaxLength="16" Width="150" TextMode="Password" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbValid" runat="server" Checked="true" Text="有効" />
														</td>
													</tr>
												</table>
												<% if (Constants.REALSHOP_OPTION_ENABLED) { %>
													<img height="33" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">
																リアル店舗オペレータ情報
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" colspan="2">
																<asp:CheckBox
																	ID="cbRealShopOperatorRegister"
																	Text="リアル店舗オペレータとして登録する"
																	onclick="javascript:change(this);"
																	runat="server" />
															</td>
														</tr>
														<tr id="trRealShopArea">
															<td class="edit_title_bg" align="left" width="30%">
																所属店舗
															</td>
															<td class="edit_item_bg" align="left">
																<asp:Repeater ID="rArea" runat="server" ItemType="w2.App.Common.RealShop.RealShopArea">
																	<ItemTemplate>
																		<div class="edit_title_bg check-toggle-open is-toggle-open">
																			<asp:CheckBox
																				ID="ckArea"
																				Text="<%# Item.AreaName %>"
																				index="<%# Container.ItemIndex %>"
																				runat="server"
																				onclick="javascript:checkAll(this)" />
																			<asp:Label
																				ID="lbAreaName"
																				onclick="javascript:load(this);"
																				index="<%# Container.ItemIndex %>"
																				class="toggle-state-icon icon-arrow-down"
																				runat="server" />
																			<div id="toggle<%# Container.ItemIndex %>" style="padding-left:30px" class="edit_item_bg">
																				<asp:CheckBoxList
																					ID="ckRealShop"
																					runat="server"
																					class="checkBox"
																					index="<%# Container.ItemIndex %>"
																					DataSource="<%# GetRealShopByArea(Item.AreaId) %>"
																					DataTextField="Text"
																					DataValueField="Value"
																					RepeatDirection="Vertical"
																					RepeatLayout="Flow" />
																			</div>
																		</div>
																	</ItemTemplate>
																</asp:Repeater>
															</td>
														</tr>
													</table>
												<% } %>
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
	function change(checkBox) {
		if (checkBox && checkBox.checked) {
			$("#trRealShopArea").slideDown("fast");
		} else {
			$("#trRealShopArea").slideUp("fast");
		}
	}

	function load(item) {
		$("#toggle" + $(item).attr('index')).slideToggle("fast");
		var divToggle = item.parentNode;
		if (divToggle.classList.contains('is-toggle-open')) {
			divToggle.classList.remove('is-toggle-open');
		} else {
			divToggle.classList.add('is-toggle-open');
		}
	}

	function checkAll(item) {
		var index = item.parentElement.getAttribute('index');
		const isChecked = item.checked;
		Array.from($("div#toggle" + index + " input")).forEach(element => {
			element.checked = isChecked;
		});
	}

	$('.checkBox').click(function (item) {
		var ckArea = this.parentNode.parentNode.firstElementChild.firstElementChild;
		var index = this.getAttribute('index');
		ckArea.checked = true;
		Array.from($("div#toggle" + index + " input")).forEach(element => {
			if (element.checked == false) {
				ckArea.checked = false;
			}
		});
	});
</script>
</asp:Content>
