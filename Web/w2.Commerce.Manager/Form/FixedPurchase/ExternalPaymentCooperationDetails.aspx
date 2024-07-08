<%--
=========================================================================================================
  Module      : 外部決済連携ログ詳細ページ(ExternalPaymentCooperationDetails.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Form/Common/PopupPage.master" CodeFile="ExternalPaymentCooperationDetails.aspx.cs" Inherits="Form_FixedPurchase_ExternalPaymentCooperationDetails" %>

<%@ Register TagPrefix="uc" TagName="FieldMemoSetting" Src="~/Form/Common/FieldMemoSetting/BodyFieldMemoSetting.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<asp:Button ID="btnTooltipInfo" runat="server" style="display:none;" UseSubmitBehavior="true"/>
	<div id="divExternalPaymentCooperationLog" runat="server">
		<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
			<tr>
				<td class="detail_title_bg" align="center" colspan="4">
					外部決済連携ログ
					<uc:FieldMemoSetting ID="FieldMemoSetting1" runat="server" Title="外部決済連携ログメモ" FieldMemoSettingList="<%# this.FieldMemoSettingData %>" TableName="<%# Constants.TABLE_FIXEDPURCHASEHISTORY %>" FieldName="<%# Constants.FIELD_FIXEDPURCHASEHISTORY_EXTERNAL_PAYMENT_COOPERATION_LOG %>" />
				</td>
			</tr>
			<tr>
				<td class="detail_item_bg" align="left" colspan="4">
					<%= WebSanitizer.HtmlEncodeChangeToBr(this.ExternalPaymentCooperationLog)%>&nbsp;
				</td>
			</tr>
		</table>
		<script type="text/javascript">
			$(document).ready(function () {
				displayMemoPopup();
			});
		</script>
		<br>
	</div>
</asp:Content>
