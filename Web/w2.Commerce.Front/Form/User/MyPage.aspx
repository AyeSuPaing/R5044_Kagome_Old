<%--
=========================================================================================================
  Module      : マイページ画面(MyPage.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="MemberRankUpDisplay" Src="~/Form/Common/MemberRankUpDisplay.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyUserProductRecommend" Src="~/Form/Common/Product/BodyUserProductRecommend.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyBarcode" Src="~/Form/Common/BodyBarcode.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MyPage.aspx.cs" Inherits="Form_User_MyPage" Title="マイページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<div id="dvUserFltContents">
		<h2>マイページ</h2>
		<div id="dvMyPage" class="unit">
			<uc:BodyBarcode runat="server" />
			<% if (this.LastLoggedinDate != "") { %>
				<h4 style="margin-bottom: 30px">
					前回ログイン日時：<%: DateTimeUtility.ToStringFromRegion(this.LastLoggedinDate, DateTimeUtility.FormatType.LongDateHourMinute1Letter) %>
				</h4>
			<% } %>
			<%-- ▽ポイントオプション利用時▽ --%>
			<% if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
				<table style="font-size: medium" width="100%">
					<tr style="font-size: 125%">
						<td width="45%">利用可能ポイント</td>
						<td width="55%" align="left">
							<%: GetNumeric(this.LoginUserPointUsable) %>pt
						</td>
					</tr>
					<tr>
					<td>通常ポイント</td>
						<td align="left"><%: GetNumeric(this.LoginUserBasePoint) %>pt</td>
					</tr>
					<% if (this.LoginUserPointExpiry.HasValue) { %>
						<tr>
							<td >通常ポイント有効期限</td>
							<td align="left"><%: DateTimeUtility.ToStringFromRegion(this.LoginUserPointExpiry, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime) %></td>
						</tr>
					<% } %>
					<% if (this.IsLimitedTermPointUsable) { %>
					<tr>
						<td style="vertical-align: top;">
							期間限定ポイント
							<span style="font-size: 0.8em;"><a id="aDetails" style="cursor: pointer; display: <%: this.HasLimitedTermPoint ? "inline" : "none" %>;">内訳を表示</a></span>
						</td>
						<td align="left"><%: GetNumeric(this.LoginUserLimitedTermPointTotal) %>pt</td>
					</tr>
					<% } %>
					<tr style="display: <%: (this.LoginUserLimitedTermPointUsableTotal > 0) ? "" : "none" %>">
						<td></td>
						<td>
							<%: string.Format("(有効期間中：{0}pt)", GetNumeric(this.LoginUserLimitedTermPointUsableTotal)) %>
						</td>
					</tr>

					<asp:Repeater
						ID="rUsableLimitedTermPoint"
						ItemType="w2.App.Common.Option.PointOption.LimitedTermPoint"
						runat="server">
						<ItemTemplate>
							<tr id="usableLimitedTermPoint<%#: Container.ItemIndex %>">
								<td></td>
								<td>&nbsp;
									<%#:
										string.Format(
											"{0}pt {1}から{2}まで有効",
											GetNumeric(Item.Point),
											DateTimeUtility.ToStringFromRegion(Item.EffectiveDate, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime),
											DateTimeUtility.ToStringFromRegion(Item.ExpiryDate, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime))
									%>
								</td>
							</tr>
						</ItemTemplate>
					</asp:Repeater>

					<tr style="display: <%: (this.LoginUserLimitedTermPointUnusableTotal > 0) ? "" : "none" %>">
						<td></td>
						<td>
							<%: string.Format("(有効期間前：{0}pt)", GetNumeric(this.LoginUserLimitedTermPointUnusableTotal)) %>
						</td>
					</tr>

					<asp:Repeater
						ID="rUnusableLimitedTermPoint"
						ItemType="w2.App.Common.Option.PointOption.LimitedTermPoint"
						runat="server">
						<ItemTemplate>
							<tr id="unusableLimitedTermPoint<%#: Container.ItemIndex %>">
								<td></td>
								<td>&nbsp;
									<%#:
										string.Format(
											"{0}pt {1}から{2}まで有効",
											GetNumeric(Item.Point),
											DateTimeUtility.ToStringFromRegion(Item.EffectiveDate, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime),
											DateTimeUtility.ToStringFromRegion(Item.ExpiryDate, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime))
									%>
								</td>
							</tr>
						</ItemTemplate>
					</asp:Repeater>

					<tr>
						<td>(仮ポイント)</td>
						<td align="left">(<%: GetNumeric(this.LoginUserPoint.PointTemp) %>pt)</td>
					</tr>
				</table>
				<br/>
			<% } %>
			<% if (HasLoginMemberRank())
			   { %>
				<h4>現在の会員ランク：<%: this.MemberRankName %></h4>
				<% if (MemberRankUtility.IsBenefitOrderDiscount(this.LoginMemberRankInfo))
				   { %>
					<div style="margin-left: 20px; margin-bottom: 3px;">注文金額割引：<%: MemberRankUtility.GetBenefitOrderDiscount(this.LoginMemberRankInfo, " {0} 以上お買い上げ時 {1}") %></div>
				<% } %>
				<% if (MemberRankUtility.IsBenefitPointAdd(this.LoginMemberRankInfo))
				   { %>
					<div style="margin-left: 20px; margin-bottom: 3px;">ポイント加算：<%: MemberRankUtility.GetBenefitPointAdd(this.LoginMemberRankInfo) %></div>
				<% } %>
				<% if (MemberRankUtility.IsBenefitShippingDiscount(this.LoginMemberRankInfo))
				   { %>
					<div style="margin-left: 20px; margin-bottom: 3px;">配送料割引：<%: MemberRankUtility.GetBenefitShippingDiscount(this.LoginMemberRankInfo) %></div>
				<% } %>
				<% if (MemberRankUtility.IsBenefitFixedFuchaseDiscountRate(this.LoginMemberRankInfo))
				   { %>
					<div style="margin-left: 20px; margin-bottom: 3px;">定期会員割引：<%: MemberRankUtility.GetBenefitFixedPurchaseDiscountRate(this.LoginMemberRankInfo, " {0} % 割引") %></div>
				<% } %>
				<br />
			<% } %>
			<div id="divMemberRankUp">
				<dl>
					<%-- 会員ランクアップ情報 --%>
					<%-- DispAllHigherRankFlagは表示する会員ランクの範囲を変更する（全上位ランクを表示：True、直近ランクのみ表示：False） --%>
					<%-- DispStandardは画面表示の並び順を変更する（ランク順位降順：0、集計期間開始日昇順：1） --%>
					<uc:MemberRankUpDisplay runat="server" DispAllHigherRankFlag="True" DispStandard="0" />
				</dl>
				<br />
			</div>
			<div id="dvUpSell">
				<dl>
					<%-- ユーザおすすめ商品 --%>
					<uc:BodyProductRecommendByRecommendEngine runat="server" RecommendCode="pc611" RecommendTitle="おすすめ商品一覧" MaxDispCount="4" DispCategoryId="" NotDispCategoryId="" NotDispRecommendProductId="" />
				</dl>
			</div>
		</div>
	</div>
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
	<script type="text/javascript">
		var detailsDisplayFlag = false;
		$(function () {
			$(document).ready(function() {
				$('[id^=usableLimitedTermPoint]').hide();
				$('[id^=unusableLimitedTermPoint]').hide();
			});

			$('#aDetails').on('click', function () {
				detailsDisplayFlag = (detailsDisplayFlag === false);
				$('#aDetails').html(detailsDisplayFlag ? "隠す" : "内訳を表示");

				if (detailsDisplayFlag) {
					$('[id^=usableLimitedTermPoint]').show();
					$('[id^=unusableLimitedTermPoint]').show();
				} else {
					$('[id^=usableLimitedTermPoint]').hide();
					$('[id^=unusableLimitedTermPoint]').hide();
				}
			});
		});
	</script>
</asp:Content>