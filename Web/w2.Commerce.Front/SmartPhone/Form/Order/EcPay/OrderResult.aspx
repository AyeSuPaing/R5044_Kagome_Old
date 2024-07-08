<%--
=========================================================================================================
  Module      : Order Result(OrderResult.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page MasterPageFile="~/Smartphone/Form/Common/OrderPage.master" Language="C#" AutoEventWireup="true" CodeFile="~/Form/Order/EcPay/OrderResult.aspx.cs" Inherits="EcPay_OrderResult" Title="Order Result"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div class="order-unit">
		<div id="divError" style="text-align: left" runat="server" visible="false">
			<span>
				注文処理中です。<br />
				注文完了メールが届かない場合、サイト管理者にお問い合わせください。
			</span>
		</div>
	</div>
	<div class="order-footer">
		<div id="divButton" class="button-next" runat="server" visible="false">
			<a class="btn btn-large btn-inverse" onclick="RedirectTopPage();">OK</a>
		</div>
	</div>
	<script type="text/javascript">
		function RedirectTopPage() {
			window.location = '<%= Constants.PATH_ROOT %>';
		}
	</script>
</asp:Content>