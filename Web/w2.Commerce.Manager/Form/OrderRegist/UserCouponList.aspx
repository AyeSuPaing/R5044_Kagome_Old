<%--
=========================================================================================================
  Module      : ユーザークーポン選択ページ(UserCouponList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="ユーザークーポン一覧" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="UserCouponList.aspx.cs" Inherits="Form_OrderRegist_UserCouponList" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
	<script type="text/javascript">
	<!--
	// 選択されたクーポン情報を設定
	function set_coupon_info(coupon_code) {
		if (window.opener != null) {
			window.opener.action_set_coupon(coupon_code);
			window.close();
		}
	}
	//-->
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">

	<!-- 検索フォーム --> 
	<table cellspacing="0" cellpadding="0" width="95%" border="0" style="margin:5px;min-width:850px">
	<tr>
	<td>
	<table class="box_border" cellspacing="1" cellpadding="3" width="850" border="0">
	<tr>
	<td class="search_box_bg">
	<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
	<td align="left">
		<table class="search_table" cellspacing="1" cellpadding="3" border="0" width="95%" style="margin:10px">
			<tr>
				<td class="search_title_bg">
					<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
					クーポンコード
				</td>
				<td class="search_item_bg">
					<asp:TextBox ID="tbCouponCode" runat="server"></asp:TextBox>
				</td>
				<td class="search_title_bg">
					<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
					クーポン名
				</td>
				<td class="search_item_bg">
					<asp:TextBox ID="tbCouponName" runat="server"></asp:TextBox>
				</td>
				<td class="search_title_bg">
					<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
					対象
				</td>
				<td class="search_item_bg">
					<asp:RadioButtonList ID="rblSearchType" runat="server" RepeatLayout="Flow">
						<asp:ListItem Text="ユーザー所持クーポン" Value="UserCoupons" Selected="True"></asp:ListItem>
						<asp:ListItem Text="ユーザー利用可能クーポン" Value="UsableCoupons"></asp:ListItem>
					</asp:RadioButtonList>
				</td>
				<td class="search_title_bg" style="width:40px">
					<asp:Button id="btnSearch" Text="　検索　" runat="server" OnClick="btnSearch_Click" />
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

	<!--結果一覧 -->
	<table cellspacing="0" cellpadding="0" width="95%" border="0" style="margin:5px;min-width:850px">
	<tr>
		<td><h2 class="cmn-hed-h2">ユーザークーポン一覧</h2></td>
	</tr>
	<tr>
	<td>
	<table class="box_border" cellspacing="1" cellpadding="3" width="850" border="0">
	<tr>
	<td class="list_box_bg">
	<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
	<td align="center">
	<!-- ▽ページング▽ -->
	<table class="list_pager" cellspacing="0" cellpadding="0" border="0" style="margin:5px 0px 5px 0px">
		<tr>
			<td><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
		</tr>
	</table>
	<!-- △ページング△ -->
	<table class="edit_table" cellspacing="1" cellpadding="3" border="0" width="95%" style="margin:0px 10px 10px 10px">
		<asp:Repeater ID="rUsableCouponList" ItemType="UserCouponDetailInfo" runat="server">
		<HeaderTemplate>
			<tr class="list_title_bg">
				<td align="center" width="120" rowspan="2">クーポンコード</td>
				<td align="center">クーポン名(管理用)</td>
				<td align="center" width="85" rowspan="2">割引額/率</td>
				<td align="center" rowspan="2">有効期限</td>
			</tr>
			<tr class="list_title_bg">
				<td align="center">発行パターン</td>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="javascript:set_coupon_info('<%#: Item.CouponCode %>')">
				<td align="center">
					<%#: Item.CouponCode %>
				</td>
				<td align="center" style="padding:0px">
					<table cellpadding="0" cellspacing="0" width="100%">
						<tr>
							<td align="left" class="item_bottom_line"><%#: Item.CouponName %></td>
						</tr>
						<tr>
							<td align="left"><%#: ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_COUPON_TYPE, Item.CouponType) %></td>
						</tr>
					</table>
				</td>
				<td align="center"><%# WebSanitizer.HtmlEncodeChangeToBr(GetCouponDiscountString(Item)) %></td>
				<td align="center" style="padding:0px">
					<table cellpadding="0" cellspacing="0" width="100%">
						<tr>
							<td align="center"><%#: Item.ExpireEnd %></td>
						</tr>
					</table>
				</td>
			</tr>
		</ItemTemplate>
		</asp:Repeater>
		<tr id="trListError" class="list_alert" runat="server" Visible="false">
			<td id="tdErrorMessage" runat="server" colspan="7">
				該当クーポンはありません。
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

</asp:Content>

