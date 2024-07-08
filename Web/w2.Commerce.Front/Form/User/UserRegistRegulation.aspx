<%--
=========================================================================================================
  Module      : 会員登録規約画面(UserRegistRegulation.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="UserRegistRegulationMessage" Src="~/Form/User/UserRegistRegulationMessage.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRegistRegulation.aspx.cs" Inherits="Form_User_UserRegistRegulation" Title="会員新規登録規約ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
	<%-- 会員登録系パンくず --%>
	<div id="dvHeaderRegistClumbs">
		<p>
			<img src="../../Contents/ImagesPkg/user/clumbs_regist_1.gif" alt="会員規約" /></p>
	</div>
	<h2>会員規約</h2>
	<div id="dvUserRegistRegulation" class="unit">
		<%-- メッセージ --%>
		<h3>会員規約について</h3>
		<div class="dvContentsInfo">
			<p>「<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>」入会お申込の前に、以下の会員規約・利用規約を必ずお読み下さい。<br />
			ご同意いただける方は、「同意する」をクリックして入会お申込フォームへお進み下さい。
			</p>
		</div>
		
		<div class="dvRegulation">
			<uc:UserRegistRegulationMessage runat="server" />
		</div>
		<div class="dvUserBtnBox">
			<p>
				<span><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>" class="btn btn-large">規約に同意しない</a></span>
				<span><asp:LinkButton ID="lbAgree" runat="server" OnClick="lbAgree_Click" class="btn btn-large btn-inverse">規約に同意する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>
</asp:Content>
