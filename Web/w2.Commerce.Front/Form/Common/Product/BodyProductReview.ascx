<%--
=========================================================================================================
  Module      : 商品レビュー出力コントローラ(BodyProductReview.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductReview.ascx.cs" Inherits="Form_Common_Product_BodyProductReview" %>

<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<div id="dvProductReviewArea">
	<div id="dvProductReviewImage">
		<p class="title"><a name="aProductReview">商品レビュー</a></p>
	</div>

	<!--▽ 商品レビューコメント一覧 ▽-->
	<div id="dvProductReviewComments" runat="server">
		<div id="pagination" class="above clearFix">
		<%= this.PagerHtml %>
		</div>
		<asp:Repeater id="rProductReview" runat="server" >
			<HeaderTemplate>
			<div id="dvReviewComment">
			</HeaderTemplate>
			<ItemTemplate>
				<ul class="ulReviewComment">
					<li>
						<img class="imgReviewRating" src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/product/icon_review_rating<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING)) %>.gif" alt="<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING)) %>">
						<strong><%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE)) %></strong>
						<%#: DateTimeUtility.ToStringFromRegion(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_DATE_OPENED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
					</li>
					<li class="liReviewName">by:<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_NICK_NAME)) %></li>
					<li class="liComment"><%# WebSanitizer.HtmlEncodeChangeToBr(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT)) %></li>
					<hr />
				</ul>
			</ItemTemplate>
			<FooterTemplate>
			</div>
			</FooterTemplate>
		</asp:Repeater>
		<asp:LinkButton id="lbReviewDispPager" runat="server" OnClick="lbReviewDispPager_Click" class="allReview">すべての商品レビューを表示(全<%= this.TotalComment %>件)</asp:LinkButton>
		<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
	</div>
	<!--△ 商品レビューコメント一覧 △-->

	<!--▽ 商品レビュー「コメントを書く」ボタン ▽-->
	<div id="dvProductReviewButtonControls">
		<asp:linkButton id="lbDispCommentForm" OnClick="lbDispCommentForm_Click" runat="server" class="btn">
		コメントを書く</asp:linkButton>
	</div>
	<!--△	商品レビュー「コメントを書く」ボタン △-->

	<!--▽ 商品レビュー記入フォーム ▽-->
	<div id="dvProductReviewInput" runat="server">
		<table class="tblReviewInput">
			<tbody>
				<tr>
					<th>
						<%: ReplaceTag("@@User.nickname.name@@") %>
						<span style="color: Red;">*</span>
					</th>
					<td>
						<asp:TextBox id="tbNickName" MaxLength="20" runat="server"></asp:TextBox>
						<asp:CustomValidator runat="Server"
							ControlToValidate="tbNickName"
							ValidationGroup="ProductReview"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th>評価</th>
					<td>
						<asp:DropDownList ID="ddlReviewRating" runat="server"></asp:DropDownList>&nbsp;星5つが最高、星1つが最低
						<asp:CustomValidator runat="Server"
							ControlToValidate="ddlReviewRating"
							ValidationGroup="ProductReview"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />	
					</td>
				</tr>
				<tr>
					<th>タイトル<span style="color: Red;">*</span></th>
					<td>
						<asp:TextBox id="tbReviewTitle" MaxLength="20" Columns="40" runat="server"></asp:TextBox>
						<asp:CustomValidator runat="Server"
							ControlToValidate="tbReviewTitle"
							ValidationGroup="ProductReview"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />	
					</td>
				</tr>
				<tr>
					<th>コメント<span style="color: Red;">*</span></th>
					<td>
						<asp:TextBox id="tbReviewComment" TextMode="MultiLine" runat="server" Columns="30" Rows="5"></asp:TextBox>
						<asp:CustomValidator runat="Server"
							ControlToValidate="tbReviewComment"
							ValidationGroup="ProductReview"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />	
					</td>
				</tr>
				<tr>
					<th></th>
					<td>
						<asp:LinkButton ID="lbBackToDetail" OnClick="lbBackToDetail_Click" runat="server" class="btn">
							戻る</asp:LinkButton>
						<asp:LinkButton ID="lbReviewConfirm" OnClick="lbReviewConfirm_Click" runat="server" ValidationGroup="ProductReview" class="btn btn-inverse">
							プレビューを確認する</asp:LinkButton>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
	<!--△ 商品レビュー記入フォーム △-->

	<!--▽ 商品レビュープレビュー ▽-->
	<div id="dvProductReviewConfirm" runat="server">
		<span class="spMessage">このように表示されます</span>
		<div id="dvReviewComment">
			<ul class="ulReviewComment">
				<li>
					<span id="sIconImageRating1" visible="false" runat="server">
						<img class="imgReviewRating" src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/product/icon_review_rating1.gif" alt="レート1" />
					</span>
					<span id="sIconImageRating2" visible="false" runat="server">
						<img class="imgReviewRating" src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/product/icon_review_rating2.gif" alt="レート2" />
					</span>
					<span id="sIconImageRating3" visible="false" runat="server">
						<img class="imgReviewRating" src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/product/icon_review_rating3.gif" alt="レート3" />
					</span>
					<span id="sIconImageRating4" visible="false" runat="server">
						<img class="imgReviewRating" src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/product/icon_review_rating4.gif" alt="レート4" />
					</span>
					<span id="sIconImageRating5" visible="false" runat="server">
						<img class="imgReviewRating" src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/product/icon_review_rating5.gif" alt="レート5" />
					</span>

					<strong> <%= this.ReviewTitle %> </strong>
				</li>
				<li class="liReviewName">by:<%= this.NickName %></li>
				<li> <%= this.ReviewComment %> </li>
			</ul>
		</div>
		<hr />
		<br />
		<asp:LinkButton ID="lbBackToInput" OnClick="lbBackToInput_Click" runat="server" class="btn">戻る</asp:LinkButton>
		<asp:LinkButton ID="lbReviewRegist" OnClick="lbProductReviewRegist_Click" runat="server" class="btn btn-inverse">投稿する</asp:LinkButton>
	</div>
	<!--△ 商品レビュープレビュー △-->
	
	<!--▽ 商品レビュー完了 ▽-->
	<div id="dvProductReviewComplete" runat="server">
		ご登録ありがとうございました。<br />
		投稿は管理者承認後反映されます。<br />
		<br />
		<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server">レビュー一覧に戻る</asp:LinkButton>
	</div>
	<!--△ 商品レビュー投稿完了 △-->
</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>

