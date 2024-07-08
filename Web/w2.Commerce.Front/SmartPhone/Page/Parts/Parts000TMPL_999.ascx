<%--
=========================================================================================================
  Module      : スマートフォン用カスタムパーツテンプレート画面(CustomPartsTemplate.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
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
<link rel="stylesheet" type="text/css" media="all" href="<%= Constants.PATH_ROOT %>/Css/top.css">
<div class="main-area top" style="padding-top:20px;margin-bottom:0px;">
<div class="sp">
<div class="main-inner">
<div class="block-topics">
	<div class="topics-wrapper">
		<div class="box-topics">
			<h2 class="ttl-topics">LANDING PAGE</h2>
			<a href="<%= Constants.PATH_ROOT %>/Landing/Formlp/new_page.aspx">
				<div class="img">
					<img src="<%= Constants.PATH_ROOT %>/Contents/ImagesPkg/landingpage_top.jpg" >
				</div>
			</a>                   
		</div>
		<div class="box-topics">
			<h2 class="ttl-topics">LANDING PAGE</h2>
			<a href="<%= Constants.PATH_ROOT %>/Landing/Formlp/new_page.aspx">
				<div class="img">
					<img src="<%= Constants.PATH_ROOT %>/Contents/ImagesPkg/topic_top.jpg">
				</div>
			</a>
		</div>
	</div>
</div>
</div>
</div>
</div>
<%-- △編集可能領域△ --%>
