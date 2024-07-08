<%--
=========================================================================================================
  Module      : 会員証バーコードコントロール(BodyMemberCardBarcode.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BodyMemberCardBarcode.ascx.cs" Inherits="MiniApp_Form_Common_BodyMemberCardBarcode" %>
<script type="text/javascript" src="<%= Constants.PATH_ROOT %>Js/JsBarcode.all.min.js"></script>

<div class="m-barcodeContent__barcode-wrap">
	<div class="m-barcodeContent__barcode">
		<img id="imgBarCodeShopCardNo" />
	</div>
	<p class="m-barcodeContent__mamberId"><%: this.IsTempLoggedIn ? "仮" : "" %>会員番号：<%#: GetCrossPointShopCardNo() %></p>
</div>

<script type="text/javascript">
	var shopCardNo = '<%#: GetCrossPointShopCardNo() %>'

	JsBarcode(
		"#imgBarCodeShopCardNo",
		shopCardNo,
		{
			width: 1.5,
			height: 60,
			format: "CODE39",
			fontSize: 12,
			displayValue: false,
		}
	);
</script>