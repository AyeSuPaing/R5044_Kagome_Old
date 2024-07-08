<%--
=========================================================================================================
  Module      : 楽天IDConnect連携モック画面(AuthRakutenIDConnectMock.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AuthRakutenIDConnectMock.aspx.cs" Inherits="Auth_Mock_AuthRakutenIDConnectMock" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title>楽天 IDConnect モック</title>
	<link rel="stylesheet" type="text/css" href="./Css/checkout_login.css"/>
</head>
<body>
楽天 IDConnect モック
<div id="wrapper">
	<div class="container">
		<div class="inner">
			<div class="sec-box">
				<p>「OK」をクリックすると、</p>
				<p class="alliance">ｗ２ソリューション株式会社</p>
				<p>は以下の楽天会員情報を受け取ります。</p>

				<dl class="m-info">
					<dt>名前</dt>
					<dd><%: this.FamilyName %> <%: this.GivenName %></dd>

					<dt>フリガナ</dt>
					<dd><%: this.FamilyNameKana %> <%: this.GivenNameKana %></dd>

					<dt>ニックネーム</dt>
					<dd><%: this.Nickname %></dd>

					<dt>性別</dt>
					<dd><%: (this.Gender == "male") ? "男性" : "女性" %></dd>

					<dt>生年月日</dt>
					<dd><%: (this.BirthDate.Split('-').Length > 0) ? this.BirthDate.Split('-')[0] : "" %>年
						<%: (this.BirthDate.Split('-').Length > 1) ? this.BirthDate.Split('-')[1] : "" %>月
						<%: (this.BirthDate.Split('-').Length > 2) ? this.BirthDate.Split('-')[2] : "" %>日</dd>

					<dt>メールアドレス</dt>
					<dd><%: this.Email %></dd>

					<dt>電話番号</dt>
					<dd><%: this.PhoneNumber %></dd>

					<dt>住所</dt>
					<dd><%: this.Formatted %></dd>
				</dl>

				<div style="text-align: center;">
					<form class="btn-list" runat="server">
						<div class="no"><asp:Button  ID="lbCancel" runat="server" OnClick="lbCancel_OnClick" CssClass="no" Text="キャンセル"></asp:Button></div>
						<div class="yes"><asp:Button ID="lbOk" runat="server" OnClick="lbOk_OnClick" CssClass="yes" Text="OK"></asp:Button></div>
					</form>
				</div>

				<div class="attain-box">
					<dl class="slide-menu">
						<dt>▼ 免責事項</dt>
						<dd>
							<ul>
								<li>このページは楽天IDConnectのページを模して作成しています。</li>
							</ul>
						</dd>
					</dl>
				</div><!-- /attain-box -->
			</div><!-- sec-box -->
		</div><!-- /inner -->
	</div><!-- /container -->
</div><!-- /wrapper -->
</body>
</html>