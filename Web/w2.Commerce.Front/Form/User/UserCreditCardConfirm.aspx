<%--
=========================================================================================================
  Module      : ユーザクレジットカード確認画面(UserCreditCardConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserCreditCardConfirm.aspx.cs" Inherits="Form_User_UserCreditCardConfirm" Title="登録クレジットカード確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<div id="dvHeaderUserCreditCardClumbs">
	<p>
		<img src="../../Contents/ImagesPkg/user/clumbs_usercreditcard_2.gif" alt="入力内容の確認" /></p>
	</div>

		<h2>入力内容の確認</h2>

	<div id="dvUserCreditCardInput" class="unit">
		<div class="dvContentsInfo">
			<div id="divRegisterDisplay" runat="server">登録するクレジットカードに間違いがなければ、「登録する」ボタンを押下してください。</div>
			<div id="divUpdateDisplay" runat="server">更新するクレジットカードに間違いがなければ、「更新する」ボタンを押下してください。</div>
		</div>
		<div class="dvUserCreditCardInfo">
			<h3>クレジットカード情報</h3>
			<table cellspacing="0">
				<tr>
					<th>クレジットカード登録名</th>
					<td><asp:Literal ID="lCardDispName" runat="server"></asp:Literal></td>
				</tr>
				<tbody id="divCreditCardNoToken" runat="server">
				<%if (OrderCommon.CreditCompanySelectable) {%>
				<tr>
					<th>カード会社</th>
					<td><asp:Literal ID="lCardCompanyName" runat="server"></asp:Literal></td>
				</tr>
				<%} %>
				<tr>
					<th>カード番号</th>
					<td>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigit" runat="server"></asp:Literal></td>
				</tr>
				<tr>
					<th>有効期限</th>
					<td>
						<asp:Literal ID="lExpirationMonth" runat="server"></asp:Literal>/<asp:Literal ID="lExpirationYear" runat="server"></asp:Literal> (月/年)
					</td>
				</tr>
				<tr>
					<th>カード名義人</th>
					<td><asp:Literal ID="lAuthorName" runat="server"></asp:Literal></td>
				</tr>
				</tbody>
			</table>
		</div>
		<div class="dvUserBtnBox">
			<p>
				<span><asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn btn-large">戻る</asp:LinkButton></span>
				<span><asp:LinkButton ID="lbUpdate" runat="server" OnClientClick="return exec_submit()" OnClick="lbUpdate_Click" class="btn btn-large btn-inverse">更新する</asp:LinkButton></span>
				<span><asp:LinkButton ID="lbSend" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn btn-large btn-inverse">登録する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>
</asp:Content>