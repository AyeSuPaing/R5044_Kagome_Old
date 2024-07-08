<%--
=========================================================================================================
  Module      : カスタムパーツテンプレート画面(CustomPartsTemplate.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" Inherits="BaseUserControl" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="トップ用トピックパーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<link rel="stylesheet" type="text/css" media="all" href="<%= Constants.PATH_ROOT %>Css/top.css">
<div class="main-area top" style="padding-top:20px;width: 1000px;">
<div class="main-inner">
<div class="block-topics">
	<div class="topics-wrapper">
		<div class="box-topics">
			<h2 class="ttl-topics">LANDING PAGE</h2>
			<a href="<%= Constants.PATH_ROOT %>/Landing/Formlp/new_page.aspx">
				<div class="img">
					<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/landingpage_top.jpg" >
				</div>
				<div class="box-txt">
					<p class="category" style="text-align: center">HEALTH</p>
					<p class="ttl" style="text-align: center">HEALTH PAPRIKA SET</p>
					<p class="link" style="text-align: center">READ AND SHOP NOW ▶</p>
				</div>
			</a>                   
			<p class="more"><a href="<%= Constants.PATH_ROOT %>/Landing/Formlp/new_page.aspx">VIEW MORE ▶</a></p>
			<!--FEATURE view more/-->
		</div>
		<div class="box-topics">
			<h2 class="ttl-topics">LANDING PAGE</h2>
			<a href="<%= Constants.PATH_ROOT %>/Landing/Formlp/new_page.aspx">
				<div class="img">
					<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/topic_top.jpg" >
				</div>
				<div class="box-txt">
					<p class="category" style="text-align: center">PHILOSOPHY</p>
					<p class="ttl" style="text-align: center">KNIT DRESS</p>
					<p class="link" style="text-align: center">READ AND SHOP NOW ▶</p>
				</div>
			</a>
			<p class="more"><a href="<%= Constants.PATH_ROOT %>/Landing/Formlp/new_page.aspx">VIEW MORE ▶</a></p>
		</div>
		<div class="box-topics">
			<h2 class="ttl-topics">COORDINATE</h2>
			<a href="<%= Constants.PATH_ROOT %>Form/Coordinate/CoordinateTop.aspx">
				<div class="img">
					<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/coordinate_top.jpg" >
				</div>
				<div class="box-txt">
					<p class="category" style="text-align: center">WINTER</p>
					<p class="ttl" style="text-align: center">AUTUMN WINTER</p>
					<p class="link" style="text-align: center">READ AND SHOP NOW ▶</p>
				</div>
			</a>
			<p class="more"><a href="<%= Constants.PATH_ROOT %>Form/Coordinate/CoordinateTop.aspx">VIEW MORE ▶</a></p>
		</div>
	</div>
</div>
</div>
</div>
<%-- △編集可能領域△ --%>
