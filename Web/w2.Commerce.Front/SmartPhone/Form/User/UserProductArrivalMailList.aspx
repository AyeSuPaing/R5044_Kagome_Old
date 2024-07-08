<%--
=========================================================================================================
  Module      : スマートフォン用入荷通知メール情報一覧画面(UserProductArrivalMailList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserProductArrivalMailList.aspx.cs" Inherits="Form_User_UserProductArrivalMailList" Title="入荷お知らせメールリスト" %>
<%@ Import Namespace="w2.App.Common.UserProductArrivalMail" %>
<%@ Import Namespace="System.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user user-product-arrival-mail-list">
<div class="user-unit">
	<h2>入荷お知らせメール情報</h2>
	<div class="msg">
	<%-- 登録完了メッセージ --%>
	<%if (this.blRequestComp){ %>
		<p class="attention">リクエストを受け付けました。</p>
	<%} %>
	<p>入荷お知らせメールをお申し込みいただいている商品の一覧です。</p>
	<%-- メール配信希望アラート --%>
	<% if (CheckUserMailFlg() == false){ %>
		<p class="attention">※入荷通知メールはお知らせメールの配信が無効でも配信されます。</p>
	<% } %>
	</div>

	<div class="pager-wrap above"><%= this.PagerHtml %></div>

	<!-- 入荷通知メール情報一覧 -->
		<asp:Repeater id="rUserProductArrivalMailList" runat="server">
		<HeaderTemplate>
		</HeaderTemplate>
		<ItemTemplate>
		<table class="cart-table">
		<tbody>
			<tr class="cart-unit-product">
				<td class="product-image">
					<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailVariationUrl(Container.DataItem)) %>">
						<w2c:ProductImage ID="ProductImage1" ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="true" runat="server" />
					</a>
				</td>
				<td class="product-info">
					<ul>
						<li class="product-name">
						<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailVariationUrl(Container.DataItem)) %>">
						<asp:Literal ID="lProductname" Text='<%# WebSanitizer.HtmlEncode(CreateProductJointName(Container.DataItem)) + "&nbsp;[" + WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID)) + "]"%>' runat="server" ></asp:Literal></a>
						</li>
					</ul>
				</td>
			</tr>
			<tr class="cart-unit-product">
				<td class="date">
					<w2c:ExtendedTextBox id="tbDateExpiredYearYear" type="tel" Runat="server" MaxLength="4" Text="<%# WebSanitizer.HtmlEncode(((DateTime)Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED)).Year) %>"></w2c:ExtendedTextBox>
					年<br />
					<w2c:ExtendedTextBox id="tbDateExpiredYearMonth" type="tel" Runat="server" MaxLength="2" Text="<%# WebSanitizer.HtmlEncode(((DateTime)Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED)).Month) %>"></w2c:ExtendedTextBox>
					月末日
				</td>
				<td class="cancel">
					<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USERPRODUCTARRIVALMAIL, Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN, Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN)))%><br />
					<%# (StringUtility.ToEmpty(Eval("guest_mail_addr")) == "") ? StringUtility.ToHankaku(UserProductArrivalMailCommon.CreateMailAddressNameFromTagReplacer((string)Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN))) : "" %>
					<%# (StringUtility.ToEmpty(Eval("guest_mail_addr")) != "") ? WebSanitizer.HtmlEncode((string)Eval("guest_mail_addr")) : "" %>
					<%# (StringUtility.ToEmpty(Eval("guest_mail_addr")) != "") ? "(" + StringUtility.ToHankaku(UserProductArrivalMailCommon.CreateMailAddressKbnNameFromTagReplacer((string)Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN))) + ")" : ""%>
					<!-- 入荷通知メール情報削除 -->
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateDeleteUserProductArrivalMailUrl(Container.DataItem)) %>' class="btn">キャンセル</a>
					<asp:HiddenField ID="hfArrivalMailNo" runat="server" Value="<%# Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_MAIL_NO) %>" />
				</td>
			</tr>
		</tbody>
		</table>
		</ItemTemplate>
		<FooterTemplate>
		</FooterTemplate>
		</asp:Repeater>
		<%-- エラーメッセージ --%>
		<% if (StringUtility.ToEmpty(this.ErrorMessage) != ""){ %>
			<div class="msg-alert"><%= this.ErrorMessage %></div>
		<% } %>

	<!-- ページャ -->
	<div class="pager-wrap below"><%= this.PagerHtml %></div>

	<% if (btnUpdate.Visible){ %>
	<div class="button">
		<div class="button-next">
			<asp:LinkButton ID="btnUpdate" OnClick="btnUpdate_Click" OnClientClick="javascript:return confirm('内容を更新します。よろしいですか？')" Text="内容を更新する" CssClass="btn" runat="server" ></asp:LinkButton>
		</div>
		<%-- 買い物を続けるボタン --%>
		<%if (this.blRequestComp){ %>
			<div class="button-prev">
				<a href='<%= WebSanitizer.UrlAttrHtmlEncode(this.BeforeProductUrl) %>' class="btn">お買い物を続ける</a>
			</div>
		<%} %>
	</div>
	<% } %>

	<uc:BodyProductRecommendByRecommendEngine runat="server" RecommendCode="sp915" RecommendTitle="おすすめ商品一覧" MaxDispCount="6" DispCategoryId="" NotDispCategoryId="" NotDispRecommendProductId="" />

</div>

<div class="user-footer">
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn">マイページトップへ</a>
	</div>
</div>

</section>
</asp:Content>