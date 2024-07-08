<%--
=========================================================================================================
  Module      : 外部レコメンド連携商品レコメンドリスト画面(BodyProductRecommendByRecommendEngine.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductRecommendByRecommendEngine.aspx.cs" Inherits="Form_Common_Product_BodyProductRecommendByRecommendEngine" %>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<form id="form1" runat="server">
<div id="divData">
<uc:BodyProductRecommendByRecommendEngine id="ucProductRecommend" runat="server" IsAsync="false" />
</div>
<%-- △編集可能領域△ --%>
</form>
</body>
</html>

