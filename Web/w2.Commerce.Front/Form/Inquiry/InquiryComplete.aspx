<%--
=========================================================================================================
  Module      : 問合せ完了画面(InquiryComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/Inquiry/InquiryComplete.aspx.cs" Inherits="Form_Inquiry_InquiryComplete" Title="問合せ完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
	<%-- 問合せ入力系パンくず --%>
	<div id="dvHeaderRegistClumbs">
		<p><img src="../../Contents/ImagesPkg/inquiry/clumbs_inquiry_3.gif" alt="受付完了" /></p>
	</div>
		<h2>受付完了</h2>

	<div id="dvUserRegistComplete" class="unit">
		<%-- メッセージ --%>
		<p class="completeInfo">
			<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>にお問合せ頂きありがとうございます。<br />

			今後とも、<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>をどうぞ宜しくお願い申し上げます。<br />
		</p>
		<p class="receptionInfo">
			<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
		</p>
		<div class="dvUserBtnBox">
			<p>
				<span id="spShopping" runat="server" Visible="false">
					<asp:LinkButton ID="lbKeepShopping" runat="server" OnClick="lbKeepShopping_Click" class="btn btn-large btn-inverse">買い物を続ける</asp:LinkButton></span>
				<span><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>" class="btn btn-large btn-inverse">トップページへ</a></span>
			</p>
		</div>
	</div>
</div>
</asp:Content>