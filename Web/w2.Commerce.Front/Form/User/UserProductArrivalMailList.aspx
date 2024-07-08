<%--
=========================================================================================================
  Module      : 入荷通知メール情報一覧画面(UserProductArrivalMailList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserProductArrivalMailList.aspx.cs" Inherits="Form_User_UserProductArrivalMailList" Title="入荷お知らせメールリスト" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
		<h2>入荷お知らせメール情報</h2>
	<div id="dvFavoriteList" class="unit">
		<%-- 登録完了メッセージ --%>
		<%if (this.blRequestComp){ %>
			<p>リクエストを受け付けました。</p>
		<%} %>
		<h4>入荷お知らせメールをお申し込みいただいている商品の一覧です。</h4>
		<%-- メール配信希望アラート --%>
		<% if (CheckUserMailFlg() == false){ %>
			<p>※入荷通知メールはお知らせメールの配信が無効でも配信されます。<br />
			</p><hr />
		<% } %>
		<!-- ///// ページャ ///// -->
		<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
		<!-- ///// 入荷通知メール情報一覧 ///// -->
			<asp:Repeater id="rUserProductArrivalMailList" runat="server">
				<HeaderTemplate>
					<table cellspacing="0">
						<tr>
							<th></th>
							<th>商品名</th>
							<th>メール区分</th>
							<th>配信先</th>
							<th>通知期限</th>
							<th>&nbsp;</th>
						</tr>
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
					<td width="15%">
							<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailVariationUrl(Container.DataItem)) %>">
							<w2c:ProductImage ID="ProductImage1" ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="true" runat="server" /></a>
					</td>
					<td width="25%">
							<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailVariationUrl(Container.DataItem)) %>">
								<asp:Literal ID="lProductname" Text='<%# WebSanitizer.HtmlEncode(CreateProductJointName(Container.DataItem)) + "&nbsp;[" + WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID)) + "]"%>' runat="server" ></asp:Literal></a>
						</td>
						<td width="14%">
							<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USERPRODUCTARRIVALMAIL, Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN, Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN)))%>
						</td>
						<td width="14%">
							<%# (StringUtility.ToEmpty(Eval("guest_mail_addr")) == "") ? StringUtility.ToHankaku(UserProductArrivalMailCommon.CreateMailAddressNameFromTagReplacer((string)Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN))) : "" %>
							<%# (StringUtility.ToEmpty(Eval("guest_mail_addr")) != "") ? WebSanitizer.HtmlEncode((string)Eval("guest_mail_addr")) : "" %>
							<%# (StringUtility.ToEmpty(Eval("guest_mail_addr")) != "") ? "(" + StringUtility.ToHankaku(UserProductArrivalMailCommon.CreateMailAddressKbnNameFromTagReplacer((string)Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN))) + ")" : ""%>
						</td>
						<td width="22%">
							<asp:TextBox ID="tbDateExpiredYearYear" Width="35" MaxLength="4" Text="<%# WebSanitizer.HtmlEncode(((DateTime)Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED)).Year) %>" runat="server"></asp:TextBox>
							年
							<asp:TextBox ID="tbDateExpiredYearMonth" Width="20" MaxLength="2" Text="<%# WebSanitizer.HtmlEncode(((DateTime)Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED)).Month) %>" runat="server"></asp:TextBox>
							月末日
						</td>
						<td width="10%">
							<!-- ///// 入荷通知メール情報削除 ///// -->
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateDeleteUserProductArrivalMailUrl(Container.DataItem)) %>' class="btn btn-mini">
								削除</a>
						</td>
						<asp:HiddenField ID="hfArrivalMailNo" runat="server" Value="<%# Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_MAIL_NO) %>" />					
					</tr>
				</ItemTemplate>
				<FooterTemplate>
					</table>
				</FooterTemplate>
			</asp:Repeater>
		<%-- エラーメッセージ --%>
		<% if (StringUtility.ToEmpty(this.ErrorMessage) != ""){ %>
			<p><%= this.ErrorMessage %></p>
		<% } %>

		<!-- ///// ページャ ///// -->
		<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
		
		<div align="center">
			<asp:LinkButton ID="btnUpdate" OnClick="btnUpdate_Click" OnClientClick="javascript:return confirm('内容を更新します。よろしいですか？')" Text="内容を更新する" class="btn btn-large btn-inverse" runat="server" >
				内容を更新する</asp:LinkButton>
		</div>
		<br />

		<!-- 買い物を続けるボタン -->
		<%if (this.blRequestComp){ %>
			<a href='<%= WebSanitizer.UrlAttrHtmlEncode(this.BeforeProductUrl) %>' class="btn btn-large">
				お買い物を続ける</a>
		<%} %>

		<uc:BodyProductRecommendByRecommendEngine runat="server" RecommendCode="pc915" RecommendTitle="おすすめ商品一覧" MaxDispCount="4" DispCategoryId="" NotDispCategoryId="" NotDispRecommendProductId="" />
	</div>
</div>
</asp:Content>