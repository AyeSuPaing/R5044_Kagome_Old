<%--
=========================================================================================================
  Module      : シングルサインオンテスト用モック（P0078_Mackenyu用）クラス(P0078_Mackenyu.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P0078_Mackenyu.aspx.cs" Inherits="P0078_Mackenyu" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <div>
		<form name="form1" action="https://localhost/R5044_Kagome.Develop/Web/w2.Commerce.Front/Form/SingleSignOn.aspx" method="post">
			<div>
				<label>遷移先URL</label>
				<input id="nurl" name="nurl" value="<%= this.NextUrl %>" style="width: 600px;">
			</div>
			<div>
				<label>メンバID</label>
				<input id="mid" name="mid" value="<%= this.MId %>" style="width: 600px;">
			</div>
			<div>
				<label>ログイン時のタイムスタンプ</label>
				<input id="timestamp" name="timestamp" value="<%= this.TimeStamp %>" style="width: 600px;">
			</div>
			<div>
				<label>チェックコード</label>
				<input id="checkcode" name="checkcode" value="<%= this.CheckCode %>" style="width: 600px;"/>
			</div>
			<div>
				<label>ニックネーム</label>
				<input id="nickname" name="nickname" value="<%= this.Nickname %>" style="width: 600px;">
			</div>
			<div>
				<label>誕生年</label>
				<input id="byear" name="byear" value="<%= this.BirthYear %>" style="width: 600px;">
			</div>
			<div>
				<label>誕生月</label>
				<input id="bmonth" name="bmonth" value="<%= this.BirthMonth %>" style="width: 600px;">
			</div>
			<div>
				<label>誕生日</label>
				<input id="bday" name="bday" value="<%= this.BirthDay %>" style="width: 600px;">
			</div>
			<div>
				<label>性別</label>
				<input id="sex" name="sex" value="<%= this.Sex %>" style="width: 600px;">
			</div>
			<div>
				<label>都道府県</label>
				<input id="pref" name="pref" value="<%= this.Pref %>" style="width: 600px;">
			</div>
			<div>
				<label>メールアドレス</label>
				<input id="mailaddr" name="mailaddr" value="<%= this.MailAddr %>" style="width: 600px;">
			</div>
			<div>
				<label>電話番号1</label>
				<input id="tel1" name="tel1" value="<%= this.Tel1 %>" style="width: 600px;">
			</div>
			<div>
				<label>電話番号2</label>
				<input id="tel2" name="tel2" value="<%= this.Tel2 %>" style="width: 600px;">
			</div>
			<div>
				<label>電話番号3</label>
				<input id="tel3" name="tel3" value="<%= this.Tel3 %>" style="width: 600px;">
			</div>
			<div>
				<a href="javascript:form1.submit()">コールバック</a>
			</div>
		</form>
    </div>
</body>
<script>
</script>
</html>
