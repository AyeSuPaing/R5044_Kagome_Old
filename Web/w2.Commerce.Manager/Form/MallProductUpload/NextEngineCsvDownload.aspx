<%--
=========================================================================================================
  Module      : モール連携 ネクストエンジン モール商品CSVダウンロードページ(NextEngineCsvDownload.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="NextEngineCsvDownload.aspx.cs" Inherits="Form_MallProductUpload_NextEngineCsvDownload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
			</td>
		</tr>
		<tr>
			<td>
				<h2 class="cmn-hed-h2">ネクストエンジン モール商品CSVダウンロード</h2>
			</td>
		</tr>
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
					<tr>
						<td class="list_box_bg" align="center">
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<table>
														<tr>
															<td>
																<img height="10px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
															</td>
														</tr>
													</table>
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td>
																<div id="divNextEngineCsv" style="display: none;">
																	<asp:Button ID="btnNextEngineCsv" Text="モール商品CSV" runat="server" OnClick="btnNextEngineCsv_OnClick"/>
																</div>
															</td>
														</tr>
													</table>
													<table>
														<tr>
															<td>
																<img height="10px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
															</td>
														</tr>
													</table>
													<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="info_item_bg">
															<td align="left">
																備考<br/>
																・モール商品CSVは自動的にダウンロードされます。もしダウンロードが開始されていない場合は「モール商品CSV」ボタンをクリックしてください。<br/>
															</td>
														</tr>
													</table>
													<table>
														<tr>
															<td>
																<img height="10px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
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
				</table>
			</td>
		</tr>
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
			</td>
		</tr>
	</table>
	<script type="text/javascript">
		// ウィンドウロード時実行イベント処理（OnLoadEventメソッドは、popup.masterでbody要素のonloadに紐づけられている。）
		OnLoadEvent = function() {
			window.opener.InsertClick();
			$('#<%= btnNextEngineCsv.ClientID %>').click();
			$('#divNextEngineCsv').show();
		}
	</script>
</asp:Content>