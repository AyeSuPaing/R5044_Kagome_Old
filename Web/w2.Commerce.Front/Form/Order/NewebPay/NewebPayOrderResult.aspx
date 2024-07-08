<%--
=========================================================================================================
  Module      : Order Result(NewebPayOrderResult.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page MasterPageFile="~/Form/Common/UserPage.master" Language="C#" AutoEventWireup="true" CodeFile="NewebPayOrderResult.aspx.cs" Inherits="Form_Order_NewebPay_NewebPayOrderResult" Title="NewebPay Order Result" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div id="dvUserRegistRegulation" class="unit">
		<div id="divError" class="dvContentsInfo" style="text-align: left" runat="server" visible="false">
			<br />
			<span>
				<asp:Literal runat="server" ID="lError" />。藍新Payでお支払いが失敗しました、お手数ですが、再注文を行ってください。
			</span>
		</div>
		<div id="divButton" class="dvUserBtnBox" runat="server" visible="false">
			<p>
				<a class="btn btn-large btn-inverse" onclick="RedirectTopPage();">OK</a>
			</p>
		</div>
	</div>
	<script type="text/javascript">
		// Redirect top page
		function RedirectTopPage() {
			window.location = '<%= Constants.PATH_ROOT %>';
		}
	</script>
</asp:Content>
