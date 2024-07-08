<%--
=========================================================================================================
  Module      : LINEミニアプリログイン連携画面(LoginCooperation.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/MiniApp/Form/Common/MiniApp.master" AutoEventWireup="true" CodeFile="LoginCooperation.aspx.cs" Inherits="MiniApp_User_LoginCooperation" Title="ミニアプリ連携画面" %>
<%@ Register Src="~/MiniApp/Form/Common/BodyMiniAppHeader.ascx" TagPrefix="uc" TagName="BodyMiniAppHeader" %>
<%@ Register Src="~/MiniApp/Form/Common/BodyMiniAppFooter.ascx" TagPrefix="uc" TagName="BodyMiniAppFooter" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>MiniApp/Css/line.css">
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
	<uc:BodyMiniAppHeader runat="server" />

	<main class="l-container-wrap">
		<section class="l-container">
			<div class="m-inputForm-wrap">
				<h2 class="m-inputForm-title">アカウント連携</h2>
				<p class="m-inputForm-discription">
					既にアカウントを作成されている方は<br>
					IDとパスワードを入力してください。
				</p>
				<dl class="m-inputForm-inputs">
					<dt class="m-inputForm-head">ログインID</dt>
					<dd class="m-inputForm-input">
						<asp:TextBox ID="tbLoginId" runat="server" />
					</dd>
					<dt class="m-inputForm-head">パスワード</dt>
					<dd class="m-inputForm-input">
						<asp:TextBox ID="tbPassword" TextMode="Password" runat="server" />
					</dd>
				</dl>
				<div id="dvCooperateError" class="m-inputForm-error" visible="false" runat="server">
					<asp:Literal ID="lCooperateError" runat="server" />
				</div>
				<div class="m-inputForm__btn-wrap">
					<asp:LinkButton ID="lbCooperate" Text="連携する" CssClass="m-inputForm__btn" OnClick="lbCooperate_OnClick" runat="server" />
				</div>
			</div>
			<div class="m-inputForm-wrap">
				<h2 class="m-inputForm-title">新規会員登録</h2>
				<p class="m-inputForm-discription">
					アカウントをお持ちでない方は<br>
					こちらから会員登録してください。
				</p>
				<div class="m-inputForm__btn-wrap">
					<asp:LinkButton ID="lbRegist" Text="会員登録する" CssClass="m-inputForm__btn" OnClick="lbRegist_OnClick" runat="server" />
				</div>
			</div>
		</section>
	</main>

	<uc:BodyMiniAppFooter runat="server" />
</asp:Content>