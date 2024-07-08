<%--
=========================================================================================================
  Module      : シリアルキー認証完了画面(SerialKeyAuthComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/SerialKeyAuthComplete.aspx.cs" Inherits="Form_User_SerialKeyAuthComplete" Title="シリアルキー認証完了ページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
	<div id="dvSerialKeyAuth">
		
		<h2>認証成功</h2>
		<p>下記のURLにアクセスして、デジタルコンテンツをダウンロードしてください。<br />
		※ブラウザのポップアップブロックを解除してください。</p>
		<br />
		<!--
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
		-->
		<script type="text/javascript">
		<!--
			// ダウンロードリンクポップアップ
			show_popup_window('<%= Constants.PROTOCOL_HTTP + Request.Url.Authority + Constants.PATH_ROOT + Constants.PAGE_FRONT_DOWNLOAD_LINK %>', 660, 540, false, false, 'Download');
		//-->
		</script>
		<div class="dvUserBtnBox">
			<p><span><asp:LinkButton ID="lbTopPage" runat="server" OnClick="lbTopPage_Click">
				<img src="../../Contents/ImagesPkg/user/btn_go_toppage.gif" alt="トップページへ" /></asp:LinkButton></span>
				<span><a href="Javascript:history.back();">
					<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/btn_go_back.gif" alt="戻る" /></a></span>
			</p>
		</div>
	</div>
</div>
</asp:Content>
