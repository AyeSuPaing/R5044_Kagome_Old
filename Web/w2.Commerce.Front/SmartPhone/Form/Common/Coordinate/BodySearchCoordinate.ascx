<%--
=========================================================================================================
  Module      : スマートフォン用コーディネート検索出力コントローラ(BodySearchCoordinate.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Coordinate/BodySearchCoordinate.ascx.cs" Inherits="Form_Common_Coordinate_BodySearchCoordinate" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="dvProductAdvancedSearch" runat="server">
<h3>検索条件の入力</h3>
<table>
<tbody>
<tr>
	<th>フリーワード</th>
	<td class="sort-word">
		<asp:TextBox ID="tbSearchCoordinate" type="search" runat="server" MaxLength="250" placeholder="検索キーワード"></asp:TextBox>
	</td>
</tr>
</tbody>
</table>
<div class="button">
	<asp:LinkButton ID="lbSearch" runat="server" OnClick="lbSearch_Click" CssClass="btn">絞り込む</asp:LinkButton>
</div>
</div>
<%-- △編集可能領域△ --%>