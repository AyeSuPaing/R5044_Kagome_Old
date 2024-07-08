<%--
=========================================================================================================
  Module      : 会員ランクアップ情報出力コントローラ(MemberRankUpDisplay.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemberRankUpDisplay.ascx.cs" Inherits="Form_Common_MemberRankUpDisplay" %>
<div>
	<% if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.MYPAGE_MEMBERRANKUP_DISPLAY) { %>
		<asp:Repeater ID="rMemberRankUpDisplay" ItemType="DispUntilRankUp" runat="server">
			<HeaderTemplate>
				<div style="font-size:12pt; margin-top: 10px; margin-bottom: 10px;">ランクアップ条件</div>
			</HeaderTemplate>
			<ItemTemplate>

				<div Visible=<%# (Item.DisplayOrFlag == false) %> runat="server" >
					<hr Visible="<%# (Container.ItemIndex != 0) %>" runat="server"/>
					<div style="margin-left: 20px; margin-bottom: 3px;">
						集計期間：&nbsp;<%#: DateTimeUtility.ToStringFromRegion(Item.AggregationPeriodStart, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %>
						&nbsp;～&nbsp;<%#: DateTimeUtility.ToStringFromRegion(Item.AggregationPeriodEnd, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %>
					</div>
					<div Visible="<%# Item.CalcPriceFlag %>" style="margin-left: 20px; margin-bottom: 3px;" runat="server">
						現在の合計購入金額：&nbsp;<%#: CurrencyManager.ToPrice(Item.CurrentTotalPrice) %>
					</div>
					<div Visible="<%# Item.CalcCountFlag %>" style="margin-left: 20px; margin-bottom: 3px;" runat="server">
						現在の合計購入回数：&nbsp;<%#: Item.CurrentTotalCount %>回
					</div>
					<div style="font-size:12pt; margin-left: 20px; margin-bottom: 4px; margin-top: 8px;">
						【<%#: Item.NextMemberRankName %>】&nbsp;まで
					</div>
				</div>

				<div>
					<!-- 表示を集計期間毎でまとめた場合に表示 -->
					<div Visible="<%# Item.DisplayOrFlag %>" style="margin-left: 20px; margin-bottom: 3px;" runat="server">または</div>
					
					<!-- ランクアップまであと○○円 -->
					<div Visible="<%# Item.CalcPriceFlag %>" style="margin-left: 40px; margin-bottom: 3px;" runat="server">
						あと&nbsp;<%#: CurrencyManager.ToPrice(Item.DifferenceTotalPrice) %>

						<!-- 同一条件の"購入金額のみ","購入回数のみ"をまとめた場合に表示 -->
						<span Visible="<%# Item.MergeOnlyFlag %>" style="margin-left: 10px; margin-bottom: 3px;" runat="server">または</span>
					</div>

					<!-- ランクアップまであと〇〇回購入 -->
					<div Visible="<%# Item.CalcCountFlag %>" style="margin-left: 40px; margin-bottom: 3px;" runat="server">
						あと&nbsp;<%#: Item.DifferenceTotalCount %>回購入
					</div>
				</div>

				<div Visible="<%# (Item.RankUpConfirmFlag) %>" style="font-size:12pt; color:blue;margin-left: 20px; margin-bottom: 4px; margin-top: 8px;" runat="server">
					☆次回ランクアップ☆
				</div>
				<div Visible="<%# (Item.RankUpConfirmFlag) %>" style="margin-left: 40px; margin-bottom: 3px;" runat="server">
					ランクアップ予定日：&nbsp;<%#: DateTimeUtility.ToStringFromRegion(Item.ExacScheduleDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %>
				</div>

			</ItemTemplate>

			<FooterTemplate>
				<hr />
			</FooterTemplate>
		</asp:Repeater>
	<% } %>
</div>