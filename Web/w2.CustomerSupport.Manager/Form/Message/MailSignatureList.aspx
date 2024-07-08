<%--
=========================================================================================================
  Module      : メール署名一覧ページ(MailSignatureList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.MailSignature" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MailSignatureList.aspx.cs" Inherits="Form_Message_MailSignatureList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="528" border="0">
	<tr>
		<td><h2 class="cmn-hed-h2">メール署名一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="728" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="704" border="0">
													<tr class="list_title_bg">
														<td align="center" width="20%" style="height: 17px">署名タイトル</td>
														<td align="left" style="height: 17px">署名本文</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="javascript:set_signature_to_opener('<%# ((CsMailSignatureModel)Container.DataItem).MailSignatureId %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsMailSignatureModel)Container.DataItem).SignatureTitle) %>
																<td align="left"><%# WebSanitizer.HtmlEncodeChangeToBr(((CsMailSignatureModel)Container.DataItem).SignatureText) %>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	function set_signature_to_opener(signature_id)
	{
		if (window.opener && (window.opener.closed == false) && window.opener.set_signature) {
			window.opener.set_signature(signature_id);
			window.close();
		}
	}
//-->
</script>
</asp:Content>