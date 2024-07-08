<%--
=========================================================================================================
  Module      : 定期配送パターン設定ページ(FixedPurchaseShippingPattern.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master"  AutoEventWireup="true" CodeFile="FixedPurchaseShippingPattern.aspx.cs" Inherits="Form_Common_FixedPurchaseShippingPattern" %>
<%@ Import Namespace="System.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="528" border="0">
	<tr>
		<td>
			<h1 class="cmn-hed-h2">定期配送パターン設定</h1>
		</td>
	</tr>
	<tr>
		<td>
			<table class="edit_table" cellspacing="1" cellpadding="3" border="0">
				<%--▽ 配送パターンエラー表示 ▽--%>
				<tr id="trFixedPurchasePatternErrorMessagesTitle" runat="server" visible="false">
					<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
				</tr>
				<tr id="trFixedPurchasePatternErrorMessages" runat="server" visible="false">
					<td class="edit_item_bg" align="left" colspan="4">
						<asp:Label ID="lbFixedPurchasePatternErrorMessages" runat="server" ForeColor="red" />
					</td>
				</tr>
				<%--△ 配送パターンエラー表示 △--%>
				<tr>
					<td class="edit_title_bg" align="center" colspan="3">配送パターン</td>
				</tr>
				<tr>
					<td class="edit_title_bg" align="left" width="160">配送パターン</td>
					<td class="edit_item_bg" align="left" >
						<asp:UpdatePanel UpdateMode="Conditional" runat="server">
						<ContentTemplate>
						<div id="divAlertMessage" runat="server" style="margin-bottom:10px;" visible="false">
							<asp:Label ID="lbFixedPurchasePatternAlertMessages" runat="server" ForeColor="red" />
						</div>
						<dl>
							<dt id="dtMonthlyDate" runat="server">
								<asp:RadioButton ID="rbFixedPurchaseDays" Text="月間隔日付指定" GroupName="FixedPurchaseShippingPattern" runat="server" />
							</dt>
							<dd id="ddMonthlyDate" style="padding-left:25px" runat="server">
								<asp:DropDownList ID="ddlMonth" runat="server"></asp:DropDownList>
								ヶ月ごと
								<asp:DropDownList ID="ddlMonthlyDate" runat="server" ></asp:DropDownList>
								日に届ける
								<br />
							</dd>
							<dt id="dtWeekAndDay" runat="server">
								<asp:RadioButton ID="rbFixedPurchaseWeekAndDay" Text="月間隔・週・曜日指定" GroupName="FixedPurchaseShippingPattern" runat="server" />
							</dt>
							<dd id="ddWeekAndDay" style="padding-left:25px" runat="server">
								<asp:DropDownList ID="ddlIntervalMonths" runat="server" />
								ヶ月ごと
								<asp:DropDownList ID="ddlWeekOfMonth" runat="server"></asp:DropDownList>
								<asp:DropDownList ID="ddlDayOfWeek" runat="server"></asp:DropDownList>
								に届ける
								<br />
							</dd>
							<dt id="dtIntervalDays" runat="server">
								<asp:RadioButton ID="rbFixedPurchaseIntervalDays" Text="配送日間隔指定" GroupName="FixedPurchaseShippingPattern" runat="server" />
							</dt>
							<dd id="ddIntervalDays" style="padding-left:25px" runat="server">
								<asp:DropDownList ID="ddlIntervalDays" runat="server"></asp:DropDownList>
								日ごとに届ける
								<br />
							</dd>
							<dt id="dtEveryNWeek" runat="server">
								<asp:RadioButton ID="rbFixedPurchaseEveryNWeek" Text="週間隔・曜日指定" GroupName="FixedPurchaseShippingPattern" runat="server" />
							</dt>
							<dd id="ddEveryNWeek" style="padding-left:25px" runat="server">
								<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week" runat="server"></asp:DropDownList>
								週間ごとの
								<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek" runat="server"></asp:DropDownList>
								に届ける
								<br />
							</dd>
						</dl>
						</ContentTemplate>
						</asp:UpdatePanel>
					</td>
					<td class="edit_item_bg" align="right" width="180">
						<input type="button" class="action_popup_bottom" onclick="javascript:set_fixed_purchase_shipping_pattern();" value="  この設定を使用する  " />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<script type="text/javascript">
<!--
	// 配送パターンを設定
	function set_fixed_purchase_shipping_pattern() {
		// 親ウィンドウが存在する場合
		if (window.opener != null) {
			// 選択された商品情報を設定
			var fixedPurchaseKbn = "";
			var fixedPurchaseSetting1 = "";

			if ($("#<%: rbFixedPurchaseDays.ClientID %>").is(":checked")){
				fixedPurchaseKbn = "<%: Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE %>";
				fixedPurchaseSetting1 = $("#<%: ddlMonth.ClientID %>").val() + "," + $("#<%: ddlMonthlyDate.ClientID %>").val();
			}
			else if ($("#<%: rbFixedPurchaseWeekAndDay.ClientID %>").is(":checked")){
				fixedPurchaseKbn = "<%: Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY %>";
				fixedPurchaseSetting1 = $("#<%: ddlIntervalMonths.ClientID %>").val() + "," + $("#<%: ddlWeekOfMonth.ClientID %>").val() + "," + $("#<%: ddlDayOfWeek.ClientID %>").val();
			}
			else if ($("#<%: rbFixedPurchaseIntervalDays.ClientID %>").is(":checked")){
				fixedPurchaseKbn = "<%: Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS %>";
				fixedPurchaseSetting1 = $("#<%: ddlIntervalDays.ClientID %>").val();
			}
			else if ($("#<%: rbFixedPurchaseEveryNWeek.ClientID %>").is(":checked")) {
				fixedPurchaseKbn = "<%: Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY %>";
				fixedPurchaseSetting1 = $("#<%: ddlFixedPurchaseEveryNWeek_Week.ClientID %>").val() + "," + $("#<%: ddlFixedPurchaseEveryNWeek_DayOfWeek.ClientID %>").val();
			}

			if ((fixedPurchaseKbn.length == 0) || (fixedPurchaseSetting1.length == 0) || (fixedPurchaseSetting1.startsWith(",")) || (fixedPurchaseSetting1.endsWith(","))) return;

			window.opener.set_modify_fixedpurchase_setting(fixedPurchaseKbn, fixedPurchaseSetting1);
			// ウィンドウ閉じる(ユーザビリティのため)
			window.close();
		}
	}
	//-->
</script>
</asp:Content>