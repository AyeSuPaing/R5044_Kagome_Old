<%--
=========================================================================================================
  Module      : レコメンド設定ボタン画像アップロードページ(RecommendButtonImageFileUpload.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="RecommendButtonImageFileUpload.aspx.cs" Inherits="Form_Recommend_RecommendButtonImageFileUpload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="618" border="0">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="674" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
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
															<td align="left">ボタン画像をアップロードしました。<br />
																<span class="notice">※</span>レコメンド設定を登録・更新後に本ボタン画像が反映されます。</td>
														</tr>
													</table>
												</div>
												<table class="edit_table" width="644" border="0" cellspacing="1" cellpadding="3">
													<%--▽ エラー表示 ▽--%>
													<tr id="trRecommendErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
													</tr>
													<tr id="trRecommendErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:Label ID="lbRecommendErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ エラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">ボタン画像アップロード</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="150">
															<asp:Literal ID="lButtonName" runat="server" /> <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:FileUpload id="fuButtonImage" runat="server" Width="250" accept="image/*" />
														</td>
														
													</tr>
													<tr id="trButtonImagePreview" style="display:none;">
														<td class="edit_title_bg" align="left">
															プレビュー
														</td>
														<td class="edit_item_bg" align="left">
															<img id="imgButtonImage" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<div class="action_part_bottom">
													<asp:Button id="btnButtonImageFileUpload" Runat="server" Text="  ボタン画像アップロード  " OnClick="btnButtonImageFileUpload_Click" />
													<asp:HiddenField ID="hfWindowOpenerPostBack" runat="server" />
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
<script type="text/javascript">
<!--
	// ボタン画像プレビュー表示イベント追加
	$(function()
	{
		$('#<%= fuButtonImage.ClientID %>').change(
			function () {
				if (!this.files.length) {
					return;
				}

				var file = $(this).prop('files')[0];
				var fr = new FileReader();
				fr.onload = function () {
					$('#trButtonImagePreview').css('display', '');
					$('#imgButtonImage').attr('src', fr.result);
				}
				fr.readAsDataURL(file);
			});
	});

	// ページロード処理（ポストバック時にも実行）
	function pageLoad(sender, args) {
		if (window.opener != null) {
			// 親ウィンドウをポストバック
			var windowOpenerPostBack = $('#<%= hfWindowOpenerPostBack.ClientID %>');
			if (windowOpenerPostBack.val() == '1') {
				window.opener.display_buttonimage_preview();
				// 親ウィンドウを2回ポストバックしないようにフラグをOFFにする
				windowOpenerPostBack.val('');
			}
		}
	}
	//-->
</script>
</asp:Content>