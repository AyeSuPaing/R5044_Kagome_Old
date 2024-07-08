<%--
=========================================================================================================
  Module      : Incident modify input (IncidentModifyInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="IncidentModifyInput.aspx.cs" Inherits="Form_Incident_IncidentModifyInput" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
	<style type="text/css">
		.label {
			text-align:right;
			padding-right:10%;
		}

		.btn-bottom {
			margin-top:3%;
			margin-bottom:3%;
		}

		.lbId {word-wrap: break-word;}
	</style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<div id="divFadeOutArea">
		<table cellspacing="0" cellpadding="0" width="800" border="0">
			<tr>
				<td>
					<div id="divComp" runat="server" visible="False">
						<table class="info_table" cellspacing="1" cellpadding="3" width="370" border="0">
							<tr class="info_item_bg">
								<td align="left">インシデント情報を更新しました。
								</td>
							</tr>
						</table>
					</div>
				</td>
			</tr>
			<tr>
				<td>
					<br />
				</td>
			</tr>
			<tr>
				<td>
					<div class="datagrid">
						<table cellspacing="1" cellpadding="2" width="758" border="0">
							<tbody>
								<tr>
									<td class="edit_title_bg" width="30%">更新対象インシデントID</td>
									<td class="edit_item_bg">
										<div class="lbId">
											<asp:Label Width="325px" ID="lIncidentId" runat="server"></asp:Label>
										</div>
									</td>
								</tr>
								<tr>
									<td class="edit_title_bg">カテゴリ</td>
									<td class="edit_item_bg">
										<asp:DropDownList Width="70%" ID="ddlIncidentCategory" CssClass="select2-select" runat="server"></asp:DropDownList>
									</td>
								</tr>
								<tr>
									<td class="edit_title_bg">ステータス</td>
									<td class="edit_item_bg">
										<asp:DropDownList Width="70%" ID="ddlIncidentStatus" runat="server"></asp:DropDownList>
									</td>
								</tr>
								<tr>
									<td class="edit_title_bg" rowspan="2">担当</td>
									<td id="tdCsGroups" class="edit_item_bg" runat="server">
										<span style="display: inline-block">グループ：</span>
										<asp:DropDownList Width="34.5%" ID="ddlCsGroups" CssClass="select2-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCsGroups_SelectedIndexChanged" runat="server"></asp:DropDownList>
									</td>
								</tr>
								<tr>
									<td class="edit_item_bg">
										<span style="display: inline-block">オペレータ：</span>
										<asp:DropDownList ID="ddlCsOperators" CssClass="select2-select" Width="34.5%" runat="server"></asp:DropDownList>
										<asp:HiddenField ID="hfCsOperatorBefore" runat="server" />
										<br />
										<asp:Button ID="btnSetOperatorAndGroup" Text="  担当を自分にセット  " OnClick="btnSetOperatorAndGroup_Click" runat="server" /><br/>
										<asp:Label runat="server" ID="lbCsGroupsAlertMessage" style="color: red" Visible="false">現在有効な拠点グループ設定が存在しないため、<br/>担当オペレータを更新する場合は、同時に担当グループが空に更新されます。</asp:Label>
									</td>
								</tr>
							</tbody>
						</table>
					</div>
				</td>
			</tr>
		</table>

		<div class="btn-bottom">
			<asp:Button ID="btnUpdate" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" OnClientClick="return confirm('表示内容で更新します。よろしいですか？');" />
			<input id="btnClose" class="action_popup_bottom" type="button" onclick="fadeout_and_close();" value="  閉じる  " />
		</div>
	</div>

	<script type="text/javascript">
		$(function () {
			ddlSelect();
		});

		// トップページ更新用スクリプト（下書き保存時に利用）
		function refresh_opener() {
			if (window.opener && (window.opener.closed == false) && window.opener.refresh) {
				window.opener.refresh();
			}
		}

		// フェードアウト＆クローズ
		function fadeout_and_close() {
			$('#divFadeOutArea').fadeOut('fast', function () {
				refresh_opener();
				window.close();
			});
		}
	</script>
</asp:Content>
