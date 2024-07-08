<%--
=========================================================================================================
  Module      : レコメンド表示出力コントローラ(BodyRecommend.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyRecommend.ascx.cs" Inherits="Form_Common_BodyRecommend" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<%-- レコメンド商品投入リンク --%>
<asp:LinkButton id="lbAddItem" runat="server" OnClick="lbAddItem_Click" />
<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
	<asp:DropDownList id="ddlSubscriptionCourseId" Visible="False" runat="server" />
<% } %>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- レコメンド表示 --%>
<%= this.RecommendDisplay %>
<%-- △編集可能領域△ --%>