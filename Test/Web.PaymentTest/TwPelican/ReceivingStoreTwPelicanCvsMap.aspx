<%--
=========================================================================================================
  Module      : Receiving Store Tw Pelican Cvs Map (ReceivingStoreTwPelicanCvsMap.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReceivingStoreTwPelicanCvsMap.aspx.cs" Inherits="Payment_TwPelican_ReceivingStoreTwPelicanCvsMap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title></title>
	<style>
		.left {
			text-align: right;
		}

		.right {
			float: left;
		}
	</style>
</head>
<body>
	<!-- Setting in Config:
	<Setting key="ConvenienceStoreMap_Pc_Url" value="http://localhost/V5/Web/w2.Commerce.Front/Payment/TwPelican/ReceivingStoreTwPelicanCvsMap.aspx" />
	<Setting key="ConvenienceStoreMap_Smartphone_Url" value="http://localhost/V5/Web/w2.Commerce.Front/Payment/TwPelican/ReceivingStoreTwPelicanCvsMap.aspx" />
	-->
	<div>
		<strong>ConvenienceStoreMap_Pc_Url:</strong>
		<br />
		<strong>ConvenienceStoreMap_Smartphone_Url:</strong>
		<br />
		http://localhost/V5/Web/w2.Commerce.Front/Payment/TwPelican/ReceivingStoreTwPelicanCvsMap.aspx
	</div>
	<br />
	<form id="Form1" runat="server">
		<table>
			<tr>
				<th class="left">Redirect URL:</th>
				<th class="right">
					<input id="tbRedirectUrl" type="text" value="" style="width: 800px;" />
				</th>
			</tr>
			<tr>
				<th class="left">Shop Id:</th>
				<th class="right">
					<input id="tbShopId" type="text" maxlength="20" value="L004351" />
				</th>
			</tr>
			<tr>
				<th class="left">Shop Name:</th>
				<th class="right">
					<input id="tbShopName" type="text" maxlength="100" value="爾富北市士園店" />
				</th>
			</tr>
			<tr>
				<th class="left">Shop Address:</th>
				<th class="right">
					<input id="tbShopAddress" type="text" maxlength="200" value="台北市士林區中山北路七段191巷21號" />
				</th>
			</tr>
			<tr>
				<th class="left">Shop Tel:</th>
				<th class="right">
					<input id="tbShopTel" type="text" maxlength="16" value="(02)89782026" />
				</th>
			</tr>
			<tr>
				<th class="left"></th>
				<th class="right">
					<input type="button" onclick="javascript: send();" value="Send" />
				</th>
			</tr>
		</table>
	</form>
	<script>
		// Get URL parameter
		function getUrlParameter(name) {
			name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
			var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
			var results = regex.exec(location.search);
			return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
		};

		// Send data
		function send() {
			var redirectUrl = tbRedirectUrl.value + '?'
				+ 'cvsspot=' + tbShopId.value
				+ '&name=' + tbShopName.value
				+ '&addr=' + tbShopAddress.value
				+ '&tel=' + tbShopTel.value;

			window.location.replace(redirectUrl);
		}

		var tbRedirectUrl = document.getElementById('tbRedirectUrl');
		var tbShopId = document.getElementById('tbShopId');
		var tbShopName = document.getElementById('tbShopName');
		var tbShopAddress = document.getElementById('tbShopAddress');
		var tbShopTel = document.getElementById('tbShopTel');

		tbRedirectUrl.value = getUrlParameter('cvstemp');
	</script>
</body>
</html>
