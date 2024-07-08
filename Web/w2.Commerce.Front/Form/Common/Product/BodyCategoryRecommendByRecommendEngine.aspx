<%--
=========================================================================================================
  Module      : 外部レコメンド連携カテゴリレコメンドリスト画面(BodyCategoryRecommendByRecommendEngine.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyCategoryRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyCategoryRecommendByRecommendEngine.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyCategoryRecommendByRecommendEngine.aspx.cs" Inherits="Form_Common_Product_BodyCategoryRecommendByRecommendEngine" %>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<form id="form1" runat="server">
<div id="divData">
<uc:BodyCategoryRecommendByRecommendEngine id="ucCategoryRecommend" runat="server" IsAsync="false" />
</div>
<%-- △編集可能領域△ --%>
</form>
</body>
</html>

