<%--
=========================================================================================================
  Module      : 商品レビュー出力コントローラ(BodyProductReview.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductReview.ascx.cs" Inherits="Form_Common_Product_BodyProductReview" %>
<%--
下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>
--%>
<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<div class="review unit">
	<h4 class="title">商品レビュー</h4>

	<%--▽ 商品レビューコメント一覧 ▽--%>
	<div id="dvProductReviewComments" runat="server">
		<div class="pager"><%= this.PagerHtml %></div>
		<asp:Repeater id="rProductReview" runat="server">
			<HeaderTemplate>
			<div class="unit">
			</HeaderTemplate>
			<ItemTemplate>
				<ul class="review-list">
					<li><img class="imgReviewRating" src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/product/icon_review_rating<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING)) %>.gif" alt="<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING)) %>"></li>
					<li>
						<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE)) %><br />
						<%#: DateTimeUtility.ToStringFromRegion(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_DATE_OPENED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
					</li>
					<li>by:<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_NICK_NAME)) %></li>
					<li><%# WebSanitizer.HtmlEncodeChangeToBr(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT)) %></li>
				</ul>
			</ItemTemplate>
			<FooterTemplate>
			</div>
			</FooterTemplate>
		</asp:Repeater>
		<div class="">
			<asp:LinkButton id="lbReviewDispPager" runat="server" OnClick="lbReviewDispPager_Click">
				すべての商品レビューを表示(全<%= this.TotalComment %>件)
			</asp:LinkButton>
		</div>
		<div class="pager"><%= this.PagerHtml %></div>

	</div>
	<%--△ 商品レビューコメント一覧 △--%>

		<%--▽ 商品レビュー「コメントを書く」ボタン ▽--%>
		<div id="dvProductReviewButtonControls" class="review-footer">
			<asp:linkButton id="lbDispCommentForm" OnClick="lbDispCommentForm_Click" CssClass="btn" runat="server">
				コメントを書く
			</asp:linkButton>
		</div>
		<%--△	商品レビュー「コメントを書く」ボタン △--%>

	<!--▽ 商品レビュー記入フォーム ▽-->
	<div id="dvProductReviewInput" runat="server">
		<dl>
			<dt>
				<%: ReplaceTag("@@User.nickname.name@@") %>
				<span style="color: Red;">*</span>
			</dt>
			<dd>
				<p class="attention">
				<asp:CustomValidator runat="Server"
					ControlToValidate="tbNickName"
					ValidationGroup="ProductReview"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
				</p>
				<asp:TextBox id="tbNickName" MaxLength="20" runat="server"></asp:TextBox>
			</dd>
			<dt>評価</dt>
			<dd>
				<p class="attention">
				<asp:CustomValidator runat="Server"
					ControlToValidate="ddlReviewRating"
					ValidationGroup="ProductReview"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
				</p>
				<asp:DropDownList ID="ddlReviewRating" runat="server"></asp:DropDownList>
			</dd>
			<dt>タイトル<span style="color: Red;">*</span></dt>
			<dd>
				<p class="attention">
				<asp:CustomValidator runat="Server"
					ControlToValidate="tbReviewTitle"
					ValidationGroup="ProductReview"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
				</p>
				<asp:TextBox id="tbReviewTitle" MaxLength="20" Columns="40" runat="server"></asp:TextBox>
			</dd>
			<dt>コメント<span style="color: Red;">*</span></dt>
			<dd>
				<p class="attention">
				<asp:CustomValidator runat="Server"
					ControlToValidate="tbReviewComment"
					ValidationGroup="ProductReview"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
				</p>
				<asp:TextBox id="tbReviewComment" TextMode="MultiLine" runat="server" Columns="30" Rows="5"></asp:TextBox>
			</dd>
		</dl>
		<div class="review-footer">
			<div class="button-next">
				<asp:LinkButton ID="lbReviewConfirm" OnClick="lbReviewConfirm_Click" CssClass="btn" runat="server" ValidationGroup="ProductReview">
					プレビューを確認する
				</asp:LinkButton>
			</div>
			<div class="button-prev">
				<asp:LinkButton ID="lbBackToDetail" OnClick="lbBackToDetail_Click" CssClass="btn" runat="server" >
					戻る
				</asp:LinkButton>
			</div>
		</div>
	</div>
	<!--△ 商品レビュー記入フォーム △-->

	<!--▽ 商品レビュープレビュー ▽-->
	<div id="dvProductReviewConfirm" runat="server">
		<ul class="review-list unit">
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
			</li>
			<li><%= this.ReviewTitle %></li>
			<li>by:<%= this.NickName %></li>
			<li><%= this.ReviewComment %></li>
		</ul>
		<div class="review-footer">
			<div class="button-next">
				<asp:LinkButton ID="lbReviewRegist" OnClick="lbProductReviewRegist_Click" CssClass="btn" runat="server" >
					投稿する
				</asp:LinkButton>
			</div>
			<div class="button-prev">
				<asp:LinkButton ID="lbBackToInput" OnClick="lbBackToInput_Click" CssClass="btn" runat="server" >
					戻る
				</asp:LinkButton>
			</div>
		</div>
	</div>
	<!--△ 商品レビュープレビュー △-->
	
	<!--▽ 商品レビュー完了 ▽-->
	<div id="dvProductReviewComplete" class="unit" runat="server">
		<p class="msg">
			ご登録ありがとうございました。<br />
			投稿は管理者承認後反映されます。
		</p>
		<div class="review-footer">
			<div class="button-next">
				<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server" CssClass="btn">レビュー一覧に戻る</asp:LinkButton>
			</div>
		</div>
	</div>
	<!--△ 商品レビュー投稿完了 △-->

</div>
<%-- △編集可能領域△ --%>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>