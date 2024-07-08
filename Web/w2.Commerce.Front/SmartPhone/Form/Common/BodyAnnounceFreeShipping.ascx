<%--
=========================================================================================================
  Module      : 送料無料案内出力コントローラ(BodyAnnounceFreeShipping.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyAnnounceFreeShipping.ascx.cs" Inherits="Form_Common_BodyAnnounceFreeShipping" %>
<%--

下記は保持用のダミー情報です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<div class="dvAnnounceFreeShipping"><span runat="server" visible="<%# this.DifferenceFreeShippingPrice != 0 %>">あと<span class="defPrice"><%#: CurrencyManager.ToPrice(this.DifferenceFreeShippingPrice) %></span>以上ご購入されますと<span class="defPrice freeShipping">送料無料</span>となります。<br /></span>
	<span runat="server" visible="<%# this.IsUseFreeShippingFee %>">※配送料無料適用外商品が含まれています。</span>
</div>
<%-- △編集可能領域△ --%>
