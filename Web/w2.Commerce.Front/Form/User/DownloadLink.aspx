<%--
=========================================================================================================
  Module      : シリアルキー認証完了画面(SerialKeyAuthComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/SerialKeyAuthComplete.aspx.cs" Inherits="Form_User_SerialKeyAuthComplete" Title="ダウンロードリンクポップアップ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
	<div id="dvSerialKeyAuth">
		
		<h2>認証成功</h2>
		<p>下記のURLにアクセスして、デジタルコンテンツをダウンロードしてください。</p>
		<br />

		<div>
			<table cellspacing="0">
				<tr>
					<th>注文番号</th>
					<td><%: this.OrderId %></td>
				</tr>
				<tr>
					<th>商品名</th>
					<td><%: this.ProductName %></td>
				</tr>
				<tr>
					<th>ダウンロードURL</th>
					<td><asp:LinkButton Text="ダウンロード" runat="server" OnClick="lbDownload_Click">ダウンロード</asp:LinkButton></td>
				</tr>
				<tr>
					<th>シリアルキー</th>
					<td><%: this.SerialKeyFormatted %></td>
				</tr>
			</table>
		</div>

		<div class="dvUserBtnBox">
			<p><span><a href="Javascript:window.close();">
				<img src="../../Contents/ImagesPkg/btn_close.gif" alt="閉じる"></a></span></p>
		</div>
	</div>
</div>
</asp:Content>
