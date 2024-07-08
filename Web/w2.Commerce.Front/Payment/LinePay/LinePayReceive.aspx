<%--
=========================================================================================================
  Module      : Line Pay Receive(LinePayReceive.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page MasterPageFile="~/Form/Common/PopupPage.master" Language="C#" AutoEventWireup="true" CodeFile="LinePayReceive.aspx.cs" Inherits="Payment_LinePay_LinePayReceive" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="divError" style="display: none; text-align: center;">
	<label style="color: red;">決済に失敗しました。再度やり直してください。</label><br />
	<a class="button" href="javascript:window.close()">OK</a>
</div>
<style>
	.button {
		text-decoration: none;
		display: inline-block;
		background-color: #efefef;
		padding: 5px 15px;
		margin: 10px 150px;
	}
</style>
	
</asp:Content>