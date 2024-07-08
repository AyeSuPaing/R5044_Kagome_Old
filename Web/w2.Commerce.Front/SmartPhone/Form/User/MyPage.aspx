<%--
=========================================================================================================
  Module      : マイページ画面(MyPage.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyMyPageMenu" Src="~/SmartPhone/Form/Common/BodyMyPageMenu.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MyPage.aspx.cs" Inherits="Form_User_MyPage" Title="マイページ" %>
<%@ Register TagPrefix="uc" TagName="MemberRankUpDisplay" Src="~/SmartPhone/Form/Common/MemberRankUpDisplay.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyBarcode" Src="~/Form/Common/BodyBarcode.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user mypage">
<div class="user-unit">
<h2>マイページ</h2>
	<div class="msg">
		<p>マイページは会員登録時に入力して頂いた個人情報の確認、変更やご注文、配達の確認を行うページです。</p>
	</div>
	<dl class="user-form">
		<uc:BodyBarcode runat="server" />
		<%if (this.LastLoggedinDate != "") { %>
			<dt>前回ログイン日時</dt>
			<dd><%: DateTimeUtility.ToStringFromRegion(this.LastLoggedinDate, DateTimeUtility.FormatType.LongDateHourMinute1Letter)%></dd>
		<%} %>
		<%-- ▽ポイントオプション利用時▽ --%>
		<%if (Constants.W2MP_POINT_OPTION_ENABLED) {%>
			<dt>利用可能ポイント</dt>
			<dd><%: GetNumeric(this.LoginUserPointUsable) %>pt</dd>
			<dt>通常ポイント</dt>
			<dd>
				<%: GetNumeric(this.LoginUserBasePoint) %>pt
				<% if (this.LoginUserPointExpiry.HasValue) { %>
					（<%: DateTimeUtility.ToStringFromRegion(this.LoginUserPointExpiry, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime) %>まで有効）
				<% } %>
			</dd>
			<% if (this.HasLimitedTermPoint && (Constants.CROSS_POINT_OPTION_ENABLED == false)) { %>
				<dt>期間限定ポイント</dt>
				<dd>
					<%: this.LoginUserLimitedTermPointTotal %>pt
				</dd>
				<dd style="display: <%: this.LoginUserLimitedTermPointUsableTotal > 0 ? "" : "none" %>">
					<%: string.Format("(有効期間中：{0}pt)", GetNumeric(this.LoginUserLimitedTermPointUsableTotal)) %> <br />
					<asp:Repeater
						ID="rUsableLimitedTermPoint"
						ItemType="w2.App.Common.Option.PointOption.LimitedTermPoint"
						runat="server">
						<ItemTemplate>
							&nbsp;<%#:
								string.Format(
									"{0}pt {1}から{2}まで有効",
									GetNumeric(Item.Point),
									DateTimeUtility.ToStringFromRegion(Item.EffectiveDate, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime),
									DateTimeUtility.ToStringFromRegion(Item.ExpiryDate, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime))
							%>
						</ItemTemplate>
					</asp:Repeater>
				</dd>
				<dd style="display: <%: this.LoginUserLimitedTermPointUnusableTotal > 0 ? "" : "none" %>">
					<%: string.Format("(有効期間前：{0}pt)", GetNumeric(this.LoginUserLimitedTermPointUnusableTotal)) %> <br />
					<asp:Repeater
						ID="rUnusableLimitedTermPoint"
						ItemType="w2.App.Common.Option.PointOption.LimitedTermPoint"
						runat="server">
						<ItemTemplate>
							&nbsp;<%#:
								string.Format(
									"{0}pt {1}から{2}まで有効",
									GetNumeric(Item.Point),
									DateTimeUtility.ToStringFromRegion(Item.EffectiveDate, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime),
									DateTimeUtility.ToStringFromRegion(Item.ExpiryDate, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime))
							%>
						</ItemTemplate>
					</asp:Repeater>
				</dd>
			<% } %>
		<%} %>
		<%if (HasLoginMemberRank()) { %>
			<dt>現在の会員ランク</dt>
			<dd><%: this.MemberRankName %>
		<%if (MemberRankUtility.IsBenefitOrderDiscount(this.LoginMemberRankInfo)) { %>
			<p style="margin-left:20px;">注文金額割引：<%: MemberRankUtility.GetBenefitOrderDiscount(this.LoginMemberRankInfo, "{0} 以上お買い上げ時 {1}") %></p>
		<%} %>
		<%if (MemberRankUtility.IsBenefitPointAdd(this.LoginMemberRankInfo)) { %>
			<p style="margin-left:20px;">ポイント加算：<%: MemberRankUtility.GetBenefitPointAdd(this.LoginMemberRankInfo) %></p>
		<%} %>
		<% if (MemberRankUtility.IsBenefitShippingDiscount(this.LoginMemberRankInfo)) { %>
			<p style="margin-left:20px;">配送料割引：<%: MemberRankUtility.GetBenefitShippingDiscount(this.LoginMemberRankInfo) %></p>
		<%} %>
		<% if (MemberRankUtility.IsBenefitFixedFuchaseDiscountRate(this.LoginMemberRankInfo)) { %>
			<p style="margin-left:20px;">定期会員割引：<%: MemberRankUtility.GetBenefitFixedPurchaseDiscountRate(this.LoginMemberRankInfo, " {0} % 割引") %></p>
		<%} %>
			</dd>
		<%} %>

		<%-- 会員ランクアップ情報 --%>
		<dd>
			<%-- DispAllHigherRankFlagは表示する会員ランクの範囲を変更する（全上位ランクを表示：True、直近ランクのみ表示：False） --%>
			<%-- DispStandardは画面表示の並び順を変更する（ランク順位降順：0、集計期間開始日昇順：1） --%>
			<uc:MemberRankUpDisplay runat="server" DispAllHigherRankFlag="True" DispStandard="0" />
		</dd>

	</dl>
	<%-- マイページメニュー（非表示 ※コントロールが存在しないとサーバエラーが発生する） --%>
	<uc:BodyMyPageMenu id="BodyMyPageMenu" runat="server"></uc:BodyMyPageMenu>
	<uc:BodyProductRecommendByRecommendEngine runat="server" RecommendCode="sp611" RecommendTitle="おすすめ商品一覧" MaxDispCount="6" DispCategoryId="" NotDispCategoryId="" NotDispRecommendProductId="" />
</div>
</section>
<w2c:FacebookConversionAPI
	EventName="ViewContent"
	UserId="<%#: this.LoginUserId %>"
	CustomDataContentName="Content name"
	CustomDataValue="500.000"
	CustomDataCurrency="JPY"
	CustomDataContentType="Content Type"
	CustomDataContentIds="Contents IDS"
	CustomDataContentCategory="Content Category"
	runat="server" />
</asp:Content>