<%--
=========================================================================================================
  Module      : Barcode(Barcode.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="Barcode.aspx.cs" Inherits="Form_Order_EcPay_Barcode" Title="バーコード"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<style type="text/css">
.drawRectangle {
		margin: auto;
		border: 1px solid grey;
		padding: 10px;
	}
.centerImage {
		margin-left: auto;
		margin-right: auto;
		display: block
	}
</style>
<script type="text/javascript" src="../../../Js/JsBarcode.all.min.js"></script>
<div class="drawRectangle" runat="server">
	<img id="barcode1" class="centerImage"/>
	<img id="barcode2" class="centerImage"/>
	<img id="barcode3" class="centerImage"/>
</div>
<script type="text/javascript">
	var params = new URLSearchParams(window.location.search);
	var code1 = params.get('code1');
	var code2 = params.get('code2');
	var code3 = params.get('code3');
	if (code1 != null) {
		JsBarcode("#barcode1", code1,
			{
				width: 1,
				height: 60,
				format: "CODE39",
				fontSize: 12,
			});
	}
	if (code2 != null) {
		JsBarcode("#barcode2", code2,
			{
				width: 1,
				height: 60,
				format: "CODE39",
				fontSize: 12,
			});
	}
	if (code3 != null) {
		JsBarcode("#barcode3", code3,
			{
				width: 1,
				height: 60,
				format: "CODE39",
				fontSize: 12,
			});
	}
</script>
</asp:Content>