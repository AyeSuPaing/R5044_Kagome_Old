<%--
=========================================================================================================
  Module      : 商品詳細検索画面(AdvancedSearch.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductAdvancedSearchBox" Src="~/Form/Common/Product/BodyProductAdvancedSearchBox.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Title="詳細検索ページ" Language="C#" Inherits="ContentsPage" MasterPageFile="~/Form/Common/DefaultPage.master" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<%-- ▽編集可能領域：HEAD追加部分▽ --%>
	<%-- △編集可能領域△ --%>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

	<table id="tblLayout">
		<tr>
			<td>
				<div id="divTopArea">
					<%-- ▽レイアウト領域：トップエリア▽ --%>
					<%-- △レイアウト領域△ --%>
				</div>

				<%-- ▽編集可能領域：コンテンツ▽ --%>
				<div class="pageAdvancedSearch">
					<div class="inner">
						<uc:BodyProductAdvancedSearchBox runat="server" />
					</div>
				</div>
				<%-- △編集可能領域△ --%>

				<div id="divBottomArea">
					<%-- ▽レイアウト領域：ボトムエリア▽ --%>
					<%-- △レイアウト領域△ --%>
				</div>

			</td>
			<td>
				<%-- ▽レイアウト領域：ライトエリア▽ --%>
				<%-- △レイアウト領域△ --%>
			</td>
		</tr>
	</table>
</asp:Content>