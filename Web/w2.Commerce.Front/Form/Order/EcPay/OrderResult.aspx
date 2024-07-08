<%--
=========================================================================================================
  Module      : Order Result(OrderResult.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page MasterPageFile="~/Form/Common/UserPage.master" Language="C#" AutoEventWireup="true" CodeFile="OrderResult.aspx.cs" Inherits="EcPay_OrderResult" Title="Order Result"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div id="dvUserRegistRegulation" class="unit">
		<div id="divError" class="dvContentsInfo" style="text-align: left" runat="server" visible="false">
			<br />
			<span>
				注文処理中です。<br />
				注文完了メールが届かない場合、サイト管理者にお問い合わせください。
			</span>
		</div>
		<div id="divButton" class="dvUserBtnBox" runat="server" visible="false">
			<p>
				<a class="btn btn-large btn-inverse" onclick="RedirectTopPage();">OK</a>
			</p>
		</div>
	</div>
	<script type="text/javascript">
		function RedirectTopPage() {
			window.location = '<%= Constants.PATH_ROOT %>';
		}
	</script>
</asp:Content>