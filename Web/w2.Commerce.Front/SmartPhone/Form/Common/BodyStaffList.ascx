<%--
=========================================================================================================
  Module      : スタッフ一覧出力コントローラ(BodyStaffList.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyStaffList.ascx.cs" Inherits="Form_Common_BodyStaffList" %>
<%@ Import Namespace="w2.Domain.Staff" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="" %>
<%@ FileInfo LastChanged="" %>

--%>

<%-- ▽編集可能領域：プロパティ設定▽ --%>
<script runat="server">
	public new void Page_Init(Object sender, EventArgs e)
	{
		base.Page_Init(sender, e);

		this.StaffId = "";
		this.StaffName = "";
		this.HeightLowerLimit = "";
		this.HeightUpperLimit = "";
		this.MaxDispCount = 2;
	}
</script>
<%-- △編集可能領域△ --%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<% if (this.StaffCount > 0) { %>
<div class="story_wrap">
<div class="right_blk">
<section id="top_recommend_curetor">
<h3 class="recommend_curetor_ttl"><span class="ttl_white">COORDINATE</span>STAFF<span class="ttl_small">スタッフ一覧</span></h3>
<div class="recommend_curator_inner">
	<asp:Repeater ID="rTopStaffList" runat="server" ItemType="w2.Domain.Staff.StaffModel">
		<ItemTemplate>
			<div class="curator_blk">
				<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_STAFF_ID, ((StaffModel)Container.DataItem).StaffId)) %>">
					<span class="curator_img bg_orange gradient_mask">
						<img src="<%# CoordinatePage.GetStaffImagePath(((StaffModel)Container.DataItem).StaffId) %>">
					</span>
					<span class="curator_name">
						<%#: Eval("StaffName") %>
					</span>
				</a>
			</div>
		</ItemTemplate>
	</asp:Repeater>
</div>
</section>
</div>
</div>
<% } %>
<%-- △編集可能領域△ --%>