<%--
=========================================================================================================
  Module      : 外部リンク設定登録ページ(ExternalLinkPreferenceRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ExternalLinkPreferenceRegister.aspx.cs" Inherits="Form_ExternalLinkPreference_ExternalLinkPreferenceRegister" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">外部リンク設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">外部リンク設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">外部リンク設定登録</h2></td>
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
																<input type="button" onclick="Javascript:history.back();" value="　戻る　" />
																<asp:button id="btnConfirmTop" runat="server" Text="　確認する　" onclick="btnConfirm_Click"></asp:button>
															</div>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tbody>
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">リンク名称<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:textbox id="tbExternalLinkTitle" runat="server" Width="500" MaxLength="50"></asp:textbox></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">URL<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:textbox id="tbExternalLinkUrl" runat="server" Width="500" Height="20" TextMode="MultiLine"></asp:textbox><br />
																		<input type="button" id="btnInsertableReplacementTagList" value="  挿入可能置換タグ一覧  " /></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">管理メモ<span class="notice"></span></td>
																		<td class="edit_item_bg" align="left"><asp:textbox id="tbExternalLinkMemo" runat="server" Width="500" Height="200" TextMode="MultiLine"></asp:textbox></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">表示順<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:textbox id="tbDisplayOrder" runat="server" Width="50" Height="20"  MaxLength="3"></asp:textbox>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
																		<td class="edit_item_bg" align="left"><asp:CheckBox id="cbValidFlg" Runat="server" Checked="true" Text="有効" /></td>
																	</tr>
																</tbody>
															</table>
														</td>
													</tr>
													<tr>
														<td>
															<br/>
															<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tr class="info_item_bg">
																	<td align="left">
																		備考<br />
																		&nbsp;&nbsp;URLにはユーザーマスタ/ユーザー拡張項目マスタの項目を動的に出力可能です。<br/>
																		&nbsp;&nbsp;具体的には、「＠＠」でユーザーマスタ項目名を囲って記述することで、出力が可能です。<br/>
																		&nbsp;&nbsp;例) ユーザーIDを出力したい場合 @@ user_id @@ と記載します。<br/>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td>
															<div class="action_part_bottom">
																<input type="button" onclick="Javascript:history.back();" value="　戻る　" />
																<asp:button id="btnConfirmBottom" runat="server" Text="　確認する　" onclick="btnConfirm_Click"></asp:button>
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
	<!--△ 登録 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
														<!--△ 置換タグ一覧 △-->
			<!--▽ 置換タグ一覧 ▽-->
			<div class="edit_title_bg" id="draggable_floatWindow" style="position:fixed; cursor: move; height: 200px; width: 480px;
				padding-right: 5px; padding-left: 5px; border: 1px solid #000000; border-radius: 10px; bottom: 110px; right: 40px; z-index: 100; display: none">
				<div id="resizable_floatWindow" style="height: 200px; width: 480px; padding-right: 5px; border-radius: 10px;">
					<asp:UpdatePanel runat="server" ChildrenAsTriggers="True">
						<ContentTemplate>
							<div style="text-align: left; float: left; font-size: 17px; font-weight: bold; padding-left: 5px; padding-top: 5px;">置換タグ一覧</div>
							<div style="text-align: right; padding-right: 5px; margin-left: auto;">
								<asp:TextBox runat="server" ID="tbTagSearch" placeholder="検索ワードを入力" />
								<span style="display: inline" class="replacementtag-close">×</span>
							</div>
							<div class="edit_title_bg">
								<table id="replacementtag" class="edit_table" style="table-layout: fixed;">
									<thead style="display: block;" class="edit_item_bg;">
										<tr>
											<th style="height: 20px; width: 220px">内容</th>
											<th style="height: 20px; width: 220px">置換タグ</th>
										</tr>
									</thead>
									<tbody style="display: block; overflow-y: scroll; max-height: 125px;">
										<asp:Repeater runat="server" ID="rReplacrmentTagList">
											<ItemTemplate>
												<tr onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown1(this)" onclick="TableRowClick(this)">
													<td style="height: 20px; width: 220px"><%#: ((Hashtable)Container.DataItem)[Constants.MASTEREXPORTSETTING_XML_J_NAME] %></td>
													<td style="height: 20px; width: 220px">@@&nbsp;<%#: ((Hashtable)Container.DataItem)[Constants.MASTEREXPORTSETTING_XML_NAME] %>&nbsp;@@</td>
												</tr>
											</ItemTemplate>
										</asp:Repeater>
									</tbody>
								</table>
							</div>
						</ContentTemplate>
					</asp:UpdatePanel>
				</div>
			</div>
													<!--△ 置換タグ一覧 △-->
	<script type="text/javascript">
		function TableRowClick(obj) {
			var result;
			result = listselect_mclick_Insert(
				obj,
				'<%= tbExternalLinkUrl.ClientID %>');

		return result;
		}

		$("#draggable_floatWindow").draggable();
		$("#resizable_floatWindow").resizable({
			minHeight: 110,
			minWidth: 400
		});

		function pageLoad() {

			$(".replacementtag-close").click(function () {
				$("#draggable_floatWindow").hide();
				return false;
			});

			$("#btnInsertableReplacementTagList").click(function () {
				$("#draggable_floatWindow").show();
				$("#replacementtag thead th:first-child").css('width', $("#replacementtag tbody td:first-child").width() + 1);
				$("#replacementtag thead th:nth-child(2)").css('width', $("#replacementtag tbody td:nth-child(2)").width() + 1);
				return false;
			});

			//リアルタイム検索
			$('#<%= tbTagSearch.ClientID %>').keyup(function () {
				var re = new RegExp($('#<%= tbTagSearch.ClientID %>').val());
				$('#replacementtag tbody tr').each(function () {
					var txt = $(this).find("td:eq(0)").html() + $(this).find("td:eq(1)").html();
					console.log(txt);
					if (txt.match(re) != null) {
						$(this).show();
					} else {
						$(this).hide();
					}
				});
			});
		}

		//リサイズ
		$(function () {
			$("#resizable_floatWindow").resize(function () {
				$("#draggable_floatWindow").height($("#resizable_floatWindow").height());
				$("#draggable_floatWindow").width($("#resizable_floatWindow").width());
				$("#replacementtag tbody").css('max-height', $("#resizable_floatWindow").height() - 77);
				$("#replacementtag tbody td").css('width', ($("#resizable_floatWindow").width() - 10) / 2);
				$("#replacementtag thead th:first-child").css('width', $("#replacementtag tbody td:first-child").width() + 1);
				$("#replacementtag thead th:nth-child(2)").css('width', $("#replacementtag tbody td:nth-child(2)").width() + 1);
			});
		});

		// Wysiwygエディタを開く
		function open_wysiwyg(textAreaId, htmlTextKbn) {
			global_fullPageFlg = false;
			global_allowedContent = false;
			textAreaWysiwygBinded = document.getElementById(textAreaId);
			open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_WYSIWYG_EDITOR) %>', 'wysiwyg', 'width=900,height=740,top=120,left=420,status=NO,resizable=yes,scrollbars=yes');
			textAreaWysiwygBinded.setAttribute("disabled", "disabled");
			document.getElementById(htmlTextKbn).checked = true;
		}
	</script>
</asp:Content>
