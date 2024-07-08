<%--
=========================================================================================================
	Module      : ターゲットリストマージページ(TargetListMerge.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
	Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master"
	AutoEventWireup="true" CodeFile="TargetListMerge.aspx.cs" Inherits="Form_TargetListMerge_TargetListMerge" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" runat="Server">
	<style type="text/css">
		.trFileUpload
		{
			padding-top: 5px;
			padding-bottom: 5px;
		}
		.block_name
		{
			border: 0px solid white;
			width: 345px;
			height: 30px;
			background: transparent;
			margin: 0 0;
			display: block;
			vertical-align: middle;
			line-height: 32px;
			overflow: hidden;
		}
		.select_btn
		{
			width: 120px;
			vertical-align: middle;
		}
		.merge_block
		{
			float: left;
			display: block;
			width: 81px;
			border: 4px solid #c7d7e2;
			padding: 4px;
			margin-right: 8px;
			-webkit-border-radius: 3px;
			-moz-border-radius: 3px;
			border-radius: 3px;
		}
		.merge_block_checked
		{
			border: 4px solid #386c2a;
			-moz-box-shadow: 3px 3px 4px #444;
			-webkit-box-shadow: 3px 3px 4px #444;
			box-shadow: 3px 3px 4px #444;
		}
		.mergeKbn.A
		{
			background: url(../../Images/Form/TargetList/M01.png);
		}
		.mergeKbn.B
		{
			background: url(../../Images/Form/TargetList/M02.png);
		}
		.mergeKbn.C
		{
			background: url(../../Images/Form/TargetList/M03.png);
		}
		.mergeKbn.D
		{
			background: url(../../Images/Form/TargetList/M04.png);
		}
		.merge_block_checked .mergeKbn
		{
			background-position: 81px 0;
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
		<!--▽ タイトル ▽-->
		<tr>
			<td>
				<h1 class="page-title">ターゲットリストマージ</h1>
			</td>
		</tr>
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<!--△ タイトル △-->
		<tr>
			<td>
				<table width="758" border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td><h2 class="cmn-hed-h2">ターゲットリストマージ</h2></td>
					</tr>
					<tr>
						<td>
							<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
								<tr>
									<td>
										<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
											<tr>
												<td width="12">
													<img alt="" height="12" src="../../Images/Common/sp.gif" border="0" />
												</td>
												<td align="left">
													<table cellspacing="0" cellpadding="5" border="0" width="100%">
														<tr>
															<td>
																<img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
																<div style="color: red;" id="clientError">
																</div>
																<div id="inproccess" class="action_part_top" style="text-align: center; display: none">
																	<img alt="Loading" src="../../Images/Common/loading.gif" />
																	<div style="font-size: 12px; font-family: メイリオ; color: #356dae; font-weight: 600;">
																		処理中</div>
																</div>
															</td>
														</tr>
														<tr>
															<td align="left">
																<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0"
																	style="padding: 0; margin: 0;">
																	<tr class="list_item_bg1">
																		<td align="left" class="detail_title_bg">
																			ターゲットリスト名<span class="notice"> *</span>
																		</td>
																		<td align="left" colspan="4">
																			<asp:TextBox ID="tbTargetListName" Width="643px" runat="server"></asp:TextBox>
																		</td>
																	</tr>
																	<tr class="list_title_bg">
																		<td colspan="5" align="center">
																			ターゲットリスト情報
																		</td>
																	</tr>
																	<tr class="list_item_bg1" valign="center">
																		<td width="99px" align="center" class="detail_title_bg">
																			ターゲットリスト１<span class="notice"> *</span>
																		</td>
																		<td width="367px" valign="middle">
																			<asp:HiddenField ID="hfTargetListId1" runat="server"></asp:HiddenField>
																			<asp:HiddenField ID="hfTargetListName1" runat="server"></asp:HiddenField>
																			<div id="lbTargetListName1" class="block_name">
																			</div>
																		</td>
																		<td width="50px" align="center">
																			<span id="sDataCount1"></span>
																			<asp:HiddenField ID="hfDataCount1" runat="server"></asp:HiddenField>
																		</td>
																		<td width="90px">
																			<p><asp:CheckBox ID="cbTargetExtract1" runat="server" />  マージ時抽出  </p>
																			<asp:HiddenField ID="hfTargetExtract1" runat="server" Value="true"></asp:HiddenField>
																		</td>
																		<td align="center" width="130px">
																			<input type="button" class="select_btn" value="  ターゲットリスト選択  " onclick="OpenWindow('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST_POPUP) %>','SetTargetList1','width=820,height=750,top=120,left=420,status=NO,scrollbars=yes');" />
																		</td>
																	</tr>
																	<tr class="list_item_bg1" valign="center">
																		<td width="99px" align="center" class="detail_title_bg">
																			ターゲットリスト２<span class="notice"> *</span>
																		</td>
																		<td>
																			<asp:HiddenField ID="hfTargetListId2" runat="server"></asp:HiddenField>
																			<asp:HiddenField ID="hfTargetListName2" runat="server"></asp:HiddenField>
																			<div id="lbTargetListName2" class="block_name">
																			</div>
																		</td>
																		<td width="50px" align="center">
																			<span id="sDataCount2"></span>
																			<asp:HiddenField ID="hfDataCount2" runat="server"></asp:HiddenField>
																		</td>
																		<td>
																			<p><asp:CheckBox ID="cbTargetExtract2" runat="server" />  マージ時抽出  </p>
																			<asp:HiddenField ID="hfTargetExtract2" runat="server" Value="true"></asp:HiddenField>
																		</td>
																		<td align="center">
																			<input type="button" class="select_btn" value="  ターゲットリスト選択  " onclick="OpenWindow('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST_POPUP) %>','SetTargetList2','width=820,height=750,top=120,left=420,status=NO,scrollbars=yes');" />
																		</td>
																	</tr>
																	<tr class="list_item_bg1">
																		<td colspan="5">
																			<br />
																		</td>
																	</tr>
																	<tr class="list_title_bg">
																		<td colspan="5" align="center">
																			マージパターン
																		</td>
																	</tr>
																	<tr class="list_item_bg1">
																		<td colspan="5" style="padding: 4px" align="center">
																			<table>
																				<tr>
																					<td>
																						<div class="merge_block">
																							<div style="font-weight: bold; border-bottom: 1px solid #cfcfcf;">
																								<asp:RadioButton ID="rbMergeKbnA" runat="server" GroupName="rbMergeKbn" Text="パターンA" />
																							</div>
																							<div style="width: 81px; height: 46px;" class="mergeKbn A">
																							</div>
																						</div>
																						<div class="merge_block">
																							<div style="font-weight: bold; border-bottom: 1px solid #cfcfcf;">
																								<asp:RadioButton ID="rbMergeKbnB" runat="server" GroupName="rbMergeKbn" Text="パターンB" />
																							</div>
																							<div style="width: 81px; height: 46px;" class="mergeKbn C">
																							</div>
																						</div>
																						<div class="merge_block">
																							<div style="font-weight: bold; border-bottom: 1px solid #cfcfcf;">
																								<asp:RadioButton ID="rbMergeKbnC" runat="server" GroupName="rbMergeKbn" Text="パターンC" />
																							</div>
																							<div style="width: 81px; height: 46px;" class="mergeKbn D">
																							</div>
																						</div>
																						<div class="merge_block">
																							<div style="font-weight: bold; border-bottom: 1px solid #cfcfcf;">
																								<asp:RadioButton ID="rbMergeKbnD" runat="server" GroupName="rbMergeKbn" Text="パターンD" />
																							</div>
																							<div style="width: 81px; height: 46px;" class="mergeKbn B">
																							</div>
																						</div>
																					</td>
																				</tr>
																			</table>
																		</td>
																	</tr>
																	<tr class="list_item_bg1">
																		<td colspan="5">
																			<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
																				<tbody>
																					<tr class="info_item_bg">
																						<td align="left">
																							説明 ：
																							<br />
																							・パターンA ２つのリストの全てのユーザーが対象
																							<br />
																							・パターンB ２つのリストの重複するユーザーのみ対象
																							<br />
																							・パターンC ターゲットリスト１に選択した方を正として、重複するユーザーを除いた正のリストの対象ユーザーのみ対象
																							<br />
																							・パターンD ２つのリストの重複するユーザーを除いたユーザー
																						</td>
																					</tr>
																				</tbody>
																			</table>
																		</td>
																	</tr>
																</table>
															</td>
														</tr>
														<tr>
															<td>
																<img alt="" height="5" src="../../Images/Common/sp.gif" width="100%" border="0" />
															</td>
														</tr>
														<tr>
															<td class="action_list_sp">
																<input type="button" name="reset" id="btnReset" value="  クリア  " onclick="location.reload();" />
																<asp:Button ID="btnMerge" runat="server" Text="  マージする  " OnClick="btnMerge_Click" OnClientClick="return confirm('マージを行います。よろしいでしょうか?');"/>
															</td>
														</tr>
														<tr>
															<td>
																<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
															</td>
														</tr>
													</table>
												</td>
												<td width="12">
													<img alt="" height="12" src="../../Images/Common/sp.gif" border="0" />
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
							<img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
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
	</table>
<script type="text/javascript">

		$(document).ready(function() {
			BindEventMergeClick();
			// radioボタンに自動復元無効追加
			$(":radio").each(function(index) {
				$(":radio")[index].autocomplete = 'off';
			});
		});

		// イベントの差し込みデータの種類をバインドします。
		function BindEventMergeClick() {
			$(":radio")[0].checked = true;

			$(".merge_block").click(function () {
				$(".merge_block").removeClass("merge_block_checked");
				$(this).find(":radio").attr('checked', 'checked');
				$(this).addClass("merge_block_checked");
			});
			$(".merge_block").removeClass("merge_block_checked");
			$(".merge_block :radio").each(function () {
				if ($(this).attr('checked')) {
					$(this).parent().parent().addClass("merge_block_checked");
				}
			});
			if ($("#<%=hfTargetExtract1.ClientID %>").val() == "true") $("#<%=cbTargetExtract1.ClientID %>").attr("disabled", "disabled");
			if ($("#<%=hfTargetExtract2.ClientID %>").val() == "true") $("#<%=cbTargetExtract2.ClientID %>").attr("disabled", "disabled");
			if ($("#<%=hfDataCount1.ClientID %>").val() != "") $("#sDataCount1").html($("#<%=hfDataCount1.ClientID %>").val());
			if ($("#<%=hfDataCount2.ClientID %>").val() != "") $("#sDataCount2").html($("#<%=hfDataCount2.ClientID %>").val());
			if ($("#<%=hfTargetListName1.ClientID %>").val() != "") $("#lbTargetListName1").html($("#<%=hfTargetListName1.ClientID %>").val());
			if ($("#<%=hfTargetListName2.ClientID %>").val() != "") $("#lbTargetListName2").html($("#<%=hfTargetListName2.ClientID %>").val());
		}

		// データの設定対象リスト 1
		function SetTargetList1(text, data_count, value, disable_checkbox) {
			$("#<%=hfTargetListId1.ClientID %>").val(value);
			$("#lbTargetListName1").html(text);
			$("#<%=hfTargetListName1.ClientID %>").val(text);
			$("#<%=hfTargetExtract1.ClientID %>").val(disable_checkbox);
			$("#<%=hfDataCount1.ClientID %>").val(data_count + " 件");
			$("#sDataCount1").html(data_count + " 件");
			if (disable_checkbox) $("#<%=cbTargetExtract1.ClientID %>").removeAttr("checked").attr("disabled", "disabled");
			else $("#<%=cbTargetExtract1.ClientID %>").removeAttr("disabled");
		}

		// データの設定対象リスト 2
		function SetTargetList2(text, data_count, value, disable_checkbox) {
			$("#<%=hfTargetListId2.ClientID %>").val(value);
			$("#lbTargetListName2").html(text);
			$("#<%=hfTargetListName2.ClientID %>").val(text);
			$("#<%=hfTargetExtract2.ClientID %>").val(disable_checkbox);
			$("#<%=hfDataCount2.ClientID %>").val(data_count + " 件");
			$("#sDataCount2").html(data_count + " 件");
			if (disable_checkbox) $("#<%=cbTargetExtract2.ClientID %>").removeAttr("checked").attr("disabled", "disabled");
			else $("#<%=cbTargetExtract2.ClientID %>").removeAttr("disabled");
		}

		// ポップアップ ウィンドウを開く
		function OpenWindow(url, name, style) {
			var newWindow = window.open(url, name, style);
			newWindow.focus();
		}

</script>
</asp:Content>
