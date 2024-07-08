<%--
=========================================================================================================
  Module      : オペレータ情報登録ページ(OperatorRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OperatorRegister.aspx.cs" Inherits="Form_Operator_OperatorRegister" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
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
				<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT){%>
					オペレータ情報登録
				<%} %>
				<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE){%>
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
												<div class="error-message-wrapper">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">
																<asp:Label id="lErrorMessage" runat="server" />
															</td>
														</tr>
													</table>
												</div>
												<div class="action_part_top">
													<a class="btn-back" href="<%= HtmlSanitizer.UrlAttrHtmlEncode(this.BackButtonUrl) %>">  戻る  </a>
													<asp:Button id="btnConfirm" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trOperatorId" runat="server" visible="false">
														<td class="edit_title_bg" align="left" width="30%">オペレータID</td>
														<td class="edit_item_bg" align="left"><asp:Label ID="lbOperatorId" runat="server"></asp:Label></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">オペレータ名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbOperatorName" runat="server" MaxLength="20" Width="180"></asp:TextBox></td>
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
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbLoginId" runat="server" MaxLength="16" Width="150"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">パスワード<span class="notice">*</span><br /><%= WebSanitizer.HtmlEncode(trOperatorId.Visible ? "（変更する場合のみ入力してください）" : "") %></td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbPassWord" runat="server" MaxLength="16" Width="150" TextMode="Password"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbValid" runat="server" Checked="true" Text="有効" />
														</td>
													</tr>
												</table>
												<br/>
												<br/>
												<!--▽ CSオペレータ欄 ▽-->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<thead>
													<tr>
														<td class="edit_title_bg" align="center" width="30%" colspan="2">CSオペレータ情報</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%" colspan="2">
															<asp:CheckBox runat="server" ID="chkIsCsOperator" Text=" CSオペレータとして登録する" />
														</td>
													</tr>
													</thead>
													<tbody id="tbdyCsOperator" runat="server">
													<tr>
														<td class="edit_title_bg" align="left" width="30%">オペレータ権限の選択<span class="notice"></span></td>
														<td class="edit_item_bg" align="left"><asp:DropDownList runat="server" ID="ddlOperatorAuthority" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール送信元の選択<span class="notice"></span></td>
														<td class="edit_item_bg" align="left"><asp:DropDownList CssClass="select2-select" Width="60%" runat="server" ID="ddlMailFrom" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール受信設定</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox runat="server" ID="chkNoticeMail" Text="通知メール"/>
															<asp:CheckBox runat="server" ID="chkWarningMail" Text="警告メール"/>
															&nbsp;
															メールアドレス <asp:TextBox runat="server" ID="tbMailAddr" Width="250" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">インシデント滞留警告アイコン表示設定（未対応）</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox class="cb-incident-warning-icon" runat="server" ID="cbNone" Text="有効" />
															&nbsp;
															橙色アイコン切替時間
															<asp:TextBox class="tb-incident-warning-icon-hours" runat="server" ID="tbNoneOrangeHours" TextMode="Number" min="0" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />時間
															<asp:TextBox class="tb-incident-warning-icon-minutes" runat="server" ID="tbNoneOrangeMinutes" TextMode="Number" min="0" max="59" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />分
															&nbsp;
															赤色アイコン切替時間
															<asp:TextBox class="tb-incident-warning-icon-hours" runat="server" ID="tbNoneRedHours" TextMode="Number" min="0" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />時間
															<asp:TextBox class="tb-incident-warning-icon-minutes" runat="server" ID="tbNoneRedMinutes" TextMode="Number" min="0" max="59" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />分
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">インシデント滞留警告アイコン表示設定（対応中）</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox class="cb-incident-warning-icon" runat="server" ID="cbActive" Text="有効" />
															&nbsp;
															橙色アイコン切替時間
															<asp:TextBox class="tb-incident-warning-icon-hours" runat="server" ID="tbActiveOrangeHours" TextMode="Number" min="0" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />時間
															<asp:TextBox class="tb-incident-warning-icon-minutes" runat="server" ID="tbActiveOrangeMinutes" TextMode="Number" min="0" max="59" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />分
															&nbsp;
															赤色アイコン切替時間
															<asp:TextBox class="tb-incident-warning-icon-hours" runat="server" ID="tbActiveRedHours" TextMode="Number" min="0" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />時間
															<asp:TextBox class="tb-incident-warning-icon-minutes" runat="server" ID="tbActiveRedMinutes" TextMode="Number" min="0" max="59" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />分
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">インシデント滞留警告アイコン表示設定（保留）</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox class="cb-incident-warning-icon" runat="server" ID="cbSuspend" Text="有効" />
															&nbsp;
															橙色アイコン切替時間
															<asp:TextBox class="tb-incident-warning-icon-hours" runat="server" ID="tbSuspendOrangeHours" TextMode="Number" min="0" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />時間
															<asp:TextBox class="tb-incident-warning-icon-minutes" runat="server" ID="tbSuspendOrangeMinutes" TextMode="Number" min="0" max="59" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />分
															&nbsp;
															赤色アイコン切替時間
															<asp:TextBox class="tb-incident-warning-icon-hours" runat="server" ID="tbSuspendRedHours" TextMode="Number" min="0" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />時間
															<asp:TextBox class="tb-incident-warning-icon-minutes" runat="server" ID="tbSuspendRedMinutes" TextMode="Number" min="0" max="59" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />分
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">インシデント滞留警告アイコン表示設定（至急）</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox class="cb-incident-warning-icon" runat="server" ID="cbUrgent" Text="有効" />
															&nbsp;
															橙色アイコン切替時間
															<asp:TextBox class="tb-incident-warning-icon-hours" runat="server" ID="tbUrgentOrangeHours" TextMode="Number" min="0" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />時間
															<asp:TextBox class="tb-incident-warning-icon-minutes" runat="server" ID="tbUrgentOrangeMinutes" TextMode="Number" min="0" max="59" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />分
															&nbsp;
															赤色アイコン切替時間
															<asp:TextBox class="tb-incident-warning-icon-hours" runat="server" ID="tbUrgentRedHours" TextMode="Number" min="0" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />時間
															<asp:TextBox class="tb-incident-warning-icon-minutes" runat="server" ID="tbUrgentRedMinutes" TextMode="Number" min="0" max="59" Enabled="False" onkeydown="return isNumberKey(event.keyCode)" />分
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">表示順<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:DropDownList runat="server" ID="ddlDisplayOrder" /></td>
													</tr>
													</tbody>
												</table>
												<br />
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">
														    備考<br />
															■通知メールについて<br />
															　- 担当設定通知 ・・・インシデントの担当者に設定されたことを通知します。<br />
															　- 依頼通知 ・・・承認・送信代行の依頼が届いたこと・結果が返却されたことを通知します。<br />
															■警告メールについて<br />
															　- 担当滞留警告 ・・・自分が担当するインシデントで、未対応のまま一定日数を超えて更新がないものを通知します。<br />
															　- グループ滞留警告 ・・・自分が所属するグループ担当のインシデントで、未対応のまま一定日数を超えて更新がないものを通知します。<br />
															　- 担当未設定警告 ・・・担当未設定のまま一定日数を超えたインシデントを通知します。（オペレータ権限で設定されているオペレータのみ）<br />
														</td>
													</tr>
												</table>
												<!--△ CSオペレータ欄 △-->
												<div class="action_part_bottom">
													<a class="btn-back" href="<%= HtmlSanitizer.UrlAttrHtmlEncode(this.BackButtonUrl) %>">  戻る  </a>
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

	 $(function() {
			if ($("#<%=this.chkIsCsOperator.ClientID%>").is(':checked')) {
				$("#<%=this.tbdyCsOperator.ClientID%>").show()
			} else {
				$("#<%=this.tbdyCsOperator.ClientID%>").hide()
			}
	  });

	  $(function() {
			$("#<%=this.chkIsCsOperator.ClientID%>").click(function() {
				if ($("#<%=this.chkIsCsOperator.ClientID%>").is(':checked')) {
					$("#<%=this.tbdyCsOperator.ClientID%>").show()
				} else {
					$("#<%=this.tbdyCsOperator.ClientID%>").hide()
				}
			});
	  });

		$(function () {
			if ($("#<%=this.chkNoticeMail.ClientID%>").is(':checked') || $("#<%=this.chkWarningMail.ClientID%>").is(':checked')) {
				$("#<%=this.tbMailAddr.ClientID%>").removeAttr("disabled")
			} else {
				$("#<%=this.tbMailAddr.ClientID%>").attr("disabled", "disabled")
			}
		});

		$(function () {
			$("#<%=this.chkNoticeMail.ClientID%>").click(function () {
				if ($("#<%=this.chkNoticeMail.ClientID%>").is(':checked') || $("#<%=this.chkWarningMail.ClientID%>").is(':checked')) {
					$("#<%=this.tbMailAddr.ClientID%>").removeAttr("disabled")
				} else {
					$("#<%=this.tbMailAddr.ClientID%>").attr("disabled", "disabled")
				}
			});
		});

		$(function () {
			$("#<%=this.chkWarningMail.ClientID%>").click(function () {
				if ($("#<%=this.chkNoticeMail.ClientID%>").is(':checked') || $("#<%=this.chkWarningMail.ClientID%>").is(':checked')) {
					$("#<%=this.tbMailAddr.ClientID%>").removeAttr("disabled")
				} else {
					$("#<%=this.tbMailAddr.ClientID%>").attr("disabled", "disabled")
				}
			});
		});

	$(function () {
		// インシデント警告アイコン入力欄表示切替
		$(".cb-incident-warning-icon").each(function () {
			var isInvalid = ($(this).children("input").prop("checked") === false);
			$(this).siblings().attr("disabled", isInvalid);
		});
		$(".cb-incident-warning-icon").change(function() {
			var isInvalid = ($(this).children("input").prop("checked") === false);
			$(this).siblings().attr("disabled", isInvalid);
		});
		// インシデント警告アイコン入力欄チェック
		$(".tb-incident-warning-icon-minutes").change(function () {
			if ($(this).val() < 0 || $(this).val().length > 7) {
				$(this).val("0");
			} else if ($(this).val() > 59) {
				var currentHourVal = parseInt($(this).prev(".tb-incident-warning-icon-hours").val());
				if (isNaN(currentHourVal)) currentHourVal = 0;
				var newHourVal = Math.floor(currentHourVal + $(this).val() / 60);
				$(this).prev(".tb-incident-warning-icon-hours").val(newHourVal.toString());
				$(this).val(($(this).val() % 60).toString());
			}
		});
		$(".tb-incident-warning-icon-hours").change(function () {
			if ($(this).val() < 0 || $(this).val().length > 3) {
				$(this).val("0");
			}
		});

		// エラーメッセージテーブル表示
		$("#<%= lErrorMessage.ClientID %>").text() ? $(".error-message-wrapper").show() : $(".error-message-wrapper").hide();
	});

	// [+-.e]のキーコード判定
	function isNumberKey(keyCode) {
		var excludeKeyCode = [69, 107, 109, 110, 187, 189, 190];
		return (excludeKeyCode.includes(keyCode) === false);
	}

	$(function () {
		ddlSelect();
	});
</script>
</asp:Content>
