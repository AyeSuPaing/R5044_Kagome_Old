<%--
=========================================================================================================
  Module      : Body Barcode (BodyBarcode.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyBarcode.ascx.cs" Inherits="Form_Common_BodyBarcode" %>
<script type="text/javascript" src="<%= Constants.PATH_ROOT %>Js/JsBarcode.all.min.js"></script>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<% if (Constants.CROSS_POINT_OPTION_ENABLED
	&& this.IsLoggedIn
	&& (string.IsNullOrEmpty(this.LoginUser.UserExtend.UserExtendDataValue.CrossPointShopCardNo) == false)) { %>
<div style="margin-bottom: 3rem;">
	<h4>会員バーコード</h4>
	<img id="imgBarCodeShopCardNo" />
	<p>会員ID: <%#: string.Format("*{0}*", this.LoginUser.UserExtend.UserExtendDataValue.CrossPointShopCardNo) %></p>
</div>
<% } %>
<%-- △編集可能領域△ --%>

<%-- ▽編集可能領域：プロパティ設定▽ --%>
<script type="text/javascript">
	var shopCardNo = '<%#: this.IsLoggedIn
		? this.LoginUser.UserExtend.UserExtendDataValue.CrossPointShopCardNo
		: string.Empty %>';

	JsBarcode(
		"#imgBarCodeShopCardNo",
		shopCardNo,
		{
			width: 1,
			height: 60,
			format: "CODE39",
			fontSize: 12,
			displayValue: false,
		}
	);
</script>
<%-- △編集可能領域△ --%>