<%--
=========================================================================================================
  Module      : 会員登録完了画面(UserRegistComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRegistComplete.aspx.cs" Inherits="Form_User_UserRegistComplete" Title="会員新規登録完了ページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
	<%-- 会員登録系パンくず --%>
	<div id="dvHeaderRegistClumbs">
		<p>
			<img src="../../Contents/ImagesPkg/user/clumbs_regist_4.gif" alt="登録完了" /></p>
	</div>
	
	<h2>登録完了</h2>

	<div id="dvUserRegistComplete" class="unit">
		<%-- メッセージ --%>
		<p class="completeInfo">
			<%: ShopMessage.GetMessage("ShopName") %>会員にご登録頂きありがとうございます。<br />
			ご登録内容を、<%: (this.UserMailAddr != "") ? this.UserMailAddr : this.UserMailAddr2 %>にお送りしましたのでご確認下さい。<br /><br />今後とも、<%: ShopMessage.GetMessage("ShopName") %>をどうぞ宜しくお願い申し上げます。<br />※メールが届かない場合は、大変お手数ですが下記までご連絡をお願いします。
		</p>
		<p class="receptionInfo">
			<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
		</p>
		<div class="dvUserBtnBox">
			<p>
				<span id="spNextUrl" runat="server" Visible="false"><asp:LinkButton ID="lbShipping" runat="server" OnClick="lbShipping_Click" class="btn btn-large btn-inverse">配送先入力画面へ</asp:LinkButton></span>
				<span id="spCart" runat="server" Visible="false"><asp:LinkButton ID="lbCart" runat="server" OnClick="lbCart_Click" class="btn btn-large btn-inverse">カートへ</asp:LinkButton></span>
				<span id="spUserProductArrivalMailList" runat="server" Visible="false"><asp:LinkButton ID="lbUserProductArrivalMailList" runat="server" OnClick="lbUserProductArrivalMailList_Click" class="btn btn-large btn-inverse">入荷お知らせメール一覧へ</asp:LinkButton></span>
				<span><asp:LinkButton ID="lbTopPage" runat="server" OnClick="lbTopPage_Click" class="btn btn-large btn-inverse">トップページへ</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>
<w2c:FacebookConversionAPI
	EventName="CompleteRegistration"
	UserId="<%#: this.LoginUserId %>"
	CustomDataContentName="Content name"
	CustomDataValue="500.000"
	CustomDataCurrency="JPY"
	CustomDataStatus="Status"
	runat="server" />
</asp:Content>