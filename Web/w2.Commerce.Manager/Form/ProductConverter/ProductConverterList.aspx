<%--
=========================================================================================================
  Module      : 商品コンバータ 設定一覧ページ(ProductConverterList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductConverterList.aspx.cs" Inherits="Form_ProductConverter_ProductConverterList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">商品コンバータ設定</h1>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!-- ▽一覧▽ -->
	<tr>
		<td><h2 class="cmn-hed-h2">商品コンバータ設定一覧</h2></td>
	</tr>
	<tr>
		<td style="width: 792px">
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!-- ▽ページング▽ -->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="300px" height="22"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td class="action_list_sp" align="left">
															<asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
														</td>
													</tr>
												</table>
												<!-- △ページング△ -->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="5%" height="17" >ＩＤ</td>
														<td align="center" width="50%" height="17">商品コンバート 設定名</td>
														<td align="center" width="20%" height="17">最終作成日時</td>
														<td align="center" width="10%" height="17">件数</td>
														<td align="center" width="10%" height="17">ファイル</td>
													</tr>
													<!-- ▽商品コンバータ一覧▽ -->
													<asp:Repeater id="rProductConverterList" Runat="server">
														<ItemTemplate>
															<tr id="rProductConverterListItem<%# Container.ItemIndex%>" class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)">
																<asp:HiddenField ID="hfAdtoId" runat="server" value="<%# Eval(Constants.FIELD_MALLPRDCNV_ADTO_ID) %>"/>
																<td align="center" onclick="listselect_mclick(getElementById('rProductConverterListItem<%# Container.ItemIndex%>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductConverterDetailUrl( ((int)Eval(Constants.FIELD_MALLPRDCNV_ADTO_ID)).ToString() )) %>')"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MALLPRDCNV_ADTO_ID))%></td>
																<td align="left" onclick="listselect_mclick(getElementById('rProductConverterListItem<%# Container.ItemIndex%>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductConverterDetailUrl( ((int)Eval(Constants.FIELD_MALLPRDCNV_ADTO_ID)).ToString() )) %>')"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MALLPRDCNV_ADTO_NAME))%></td>
																<td align="center" onclick="listselect_mclick(getElementById('rProductConverterListItem<%# Container.ItemIndex%>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductConverterDetailUrl( ((int)Eval(Constants.FIELD_MALLPRDCNV_ADTO_ID)).ToString() )) %>')"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_MALLPRDCNV_LASTCREATED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
																<td align="center" onclick="listselect_mclick(getElementById('rProductConverterListItem<%# Container.ItemIndex%>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductConverterDetailUrl( ((int)Eval(Constants.FIELD_MALLPRDCNV_ADTO_ID)).ToString() )) %>')"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MALLPRDCNV_CREATEDRECORDCOUNT))%></td>
																<td align="center"><a onclick="javascript:window.open('<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductConverterFilesUrl(  ((int)Eval(Constants.FIELD_MALLPRDCNV_ADTO_ID)).ToString() )) %>','_blank','status=0,resizable=1,toolbar=0,menubar=0,width=800px,height=640px');return false;" href="#">一覧</a></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<!-- △商品コンバータ一覧△ -->
													<tr id="trProductConverterListError" class="list_alert" runat="server" visible="false">
														<td id="tdErrorMessage" runat="server" colspan="8"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp" align="right">
												<asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
											</td>
										</tr>
										<tr>
											<td><img height="10px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<!-- △一覧△ -->
</table>
<script type="text/javascript">
<!--
	var exec_submit_flg = 0;
	function exec_submit() {
		if (exec_submit_flg == 0) {
			exec_submit_flg = 1;
			return true;
		}
		else {
			return false;
		}
	}
//-->
</script>
</asp:Content>

