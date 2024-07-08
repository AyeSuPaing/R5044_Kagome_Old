<%--
=========================================================================================================
  Module      : 名称翻訳設定ダウンロードページ(NameTranslationSettingExport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="NameTranslationSettingDownLoad.aspx.cs" Inherits="Form_NameTranslationSettingDownLoad_NameTranslationSettingDownLoad" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<h1 class="page-title">名称翻訳設定ダウンロード</h1>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td colspan="2">
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="list_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															マスタ種別
															<div class="search_btn_sub">
																<a href="javascript:AllCheck();">全選択</a>
																<a href="javascript:AllNoCheck();">全解除</a>
															</div>
														</td>
														<td class="list_item_bg1" colspan="5">
															<asp:CheckBoxList id="cblDataKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="4" cssclass="chkBoxList" ></asp:CheckBoxList>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td>
									<div class="action_part_bottom"><asp:Button id="btnDownLoad" Runat="server" Text="  データダウンロード  " OnClick="btnDownLoad_Click" /></div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ 検索 △-->
	<tr>
		<td colspan="2"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<script type="text/javascript">
	function AllCheck() {
		$(':checkbox').attr("checked", true);
	}

	function AllNoCheck() {
		$(':checkbox').attr("checked", false);
	}
</script>

</asp:Content>
