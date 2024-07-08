<%--
=========================================================================================================
  Module      : 特集エリアパーツテンプレート画面(PartsBannerTemplate.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="FeatureAreaUserControl" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="slider(デモ用・削除しないでください)" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
base.Page_Init(sender, e);

<%-- ▽編集可能領域：プロパティ設定▽ --%>
this.FeatureAreaId = "9999999999";
<%-- △編集可能領域△ --%>
}
</script>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<asp:Repeater id="rFeatureArea" ItemType="FeatureAreaUserControl.FeatureAreaBannerInput" Runat="server">
<HeaderTemplate>
	<%-- スライダーを表示す領域を明示的に指定する（指定しない場合、表示が崩れる可能性あり） --%>
<ul class="slider clearFix" style="height:auto;margin:0px 25px 25px 25px">
</HeaderTemplate>
<ItemTemplate>
	    <li style="margin: 10px 0px; float: left; width:100%">
		<a href="<%#: Item.DisplayLinkUrl %>" target='<%#: Item.IsAnotherWindow ? "_blank" : "" %>'>
		    <div>
        	    <%-- スライダーを表示す領域を明示的に指定する（指定しない場合、表示が崩れる可能性あり） --%>
	    		<img src="<%#: Item.ImageFilePath %>" alt="<%#: Item.AltText %>" style="width:100%" >
		    </div>
		    <div>
    			<label><%#: Item.Text %></label>
		    </div>
		</a>
	</li>
</ItemTemplate>
<FooterTemplate>
	</ul>
</FooterTemplate>
</asp:Repeater>
<script>
	<!--
	$(document).on('ready',
	function() {
		$('.slider').slick({
			autoplay: <%= this.SliderScrollAuto %>,
			autoplaySpeed: <%= int.Parse(this.SliderScrollInterval) * 1000 %>,
			dots: <%= this.SliderDot %>,
			arrows: <%= this.SliderArrow %>,
			slidesToShow: <%= this.SliderCount %>,
			slidesToScroll: <%= this.SliderScrollCount %>
		});
	})
	// -->
</script>
<%-- △編集可能領域△ --%>