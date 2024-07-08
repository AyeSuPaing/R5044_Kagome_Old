<%--
=========================================================================================================
  Module      : Atone LPカート用与信取得ページ(AtoneExecOrderForLandingCart.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AtoneExecOrderForLandingCart.aspx.cs" Inherits="Payment_Atone_AtoneExecOrderForLandingCart" %>
<%@ Register TagPrefix="uc" TagName="HeaderScriptDeclaration" Src="~/Form/Common/HeaderScriptDeclaration.ascx" %>
<%@ Register TagPrefix="uc" TagName="AtonePaymentScript" Src="~/Form/Common/AtonePaymentScript.ascx" %>
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="ja" lang="ja">
	<head id="Head1" runat="server">
		<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
		<title>転送中です</title>
		<meta http-equiv="Content-Script-Type" content="text/javascript" />
		<%-- キャッシュ無効設定：ここから（消さないでください） --%>
		<meta http-equiv="Pragma" content="no-cache" />
		<meta http-equiv="Cache-Control" content="no-cache" />
		<meta http-equiv="Expires" content="-1" />
		<%	
			Response.AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate");
			Response.AddHeader("Pragma", "no-cache");
		%>
		<%-- キャッシュ無効設定：ここまで --%>
		<%-- 各種Js読み込み --%>
		<uc:HeaderScriptDeclaration id="HeaderScriptDeclaration" runat="server"/>
		<link href="../../Css/common.css" rel="stylesheet" type="text/css" media="all" />
	</head>
	<body>
		<form id="form1" onsubmit="return (document.getElementById('__EVENTVALIDATION') != null);" runat="server">
			<script type="text/javascript">
				// クリックジャック攻撃対策
				CheckClickJack('<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_BLANK %>');
			</script>
			<%-- スクリプトマネージャ --%>
			<asp:ScriptManager ID="smScriptManager" runat="server" ScriptMode="Release"></asp:ScriptManager>
			<div>
				転送中です。しばらくお待ちください。
				<p style="margin: 0 auto;">
					<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/loading.gif" alt="転送中です" />
				</p>
				<asp:HiddenField runat="server" ID="hfAtoneToken" />
				<asp:LinkButton runat="server" ID="lbReturnToConfirm" OnClick="lbReturnToConfirm_Click"/>
				<asp:LinkButton runat="server" ID="lbReturnToInput" OnClick="lbReturnToInput_Click" CssClass="btn btn-large btn-org-gry">前のページに戻る</asp:LinkButton>
			</div>
			<script type="text/javascript">
				let isLastItemCart = false;
				let isPageConfirm = false;
				let isMyPage = null;
				$('#<%= hfAtoneToken.ClientID %>').val('<%= this.IsLoggedIn 
					? this.LoginUser.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID]
					: string.Empty %>');

				$(function () {
					GetAtoneAuthority();
				});

				// モーダルから隠しフィールドにトークンをセット
				function SetAtoneTokenFromChildPage(token) {
					$('#<%= hfAtoneToken.ClientID %>').val(token);
				}

				// 現在のトークンを取得
				function GetCurrentAtoneToken() {
					return $('#<%= hfAtoneToken.ClientID %>').val();
				}

				// Atone決済を利用するカートのIndexを取得
				function GetIndexCartHavingPaymentAtoneOrAftee(isAtone, callBack) {
					$.ajax({
						type: "POST",
						url: "<%= string.Format("{0}{1}",
							Constants.PATH_ROOT,
							string.Format("{0}{1}", this.IsSmartPhone
								? "SmartPhone/"
								: string.Empty, Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)) %>/GetIndexCartHavingPaymentAtoneOrAftee",
						contentType: "application/json; charset=utf-8",
						dataType: "json",
						data: JSON.stringify({ isAtone: isAtone }),
						async: false,
						success: callBack
					});
				}

				// Atone与信取得
				function GetAtoneAuthority() {
					GetIndexCartHavingPaymentAtoneOrAftee(true, function (response) {
						const data = JSON.parse(response.d);
						if (data.indexs.length > 0) {
							for (let index = 0; index < data.indexs.length; index++) {
								AtoneAuthories(data.indexs[index]);
								isLastItemCart = (index == (data.indexs.length - 1));
								break;
							}
						}
					});
				}

				// 注文実行
				function ExecuteOrder() {
					const buttonComplete = document.getElementById("<%= lbReturnToConfirm.ClientID %>");
					buttonComplete.click();
				}
			</script>
			<% ucAtonePaymentScript.CurrentUrl = string.Format("{0}{1}",
				Constants.PATH_ROOT,
				string.Format("{0}{1}", this.IsSmartPhone
					? "SmartPhone/"
					: string.Empty, Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)); %>
			<uc:AtonePaymentScript ID="ucAtonePaymentScript" runat="server"/>
		</form>
	</body>
</html>

