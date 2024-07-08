<%--
=========================================================================================================
  Module      : サポート情報ページ(SupportInformation.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SupportInformation.aspx.cs" Inherits="Form_Support_SupportInformation" %>
<%@ Import Namespace="w2.App.Common.Global.Config" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td>
			<h1 class="page-title">サポートサイト</h1>
		</td>
	</tr>
	<tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<tr>
		<td>
			<table class="box_border" width="784" border="0" cellspacing="1" cellpadding="0">
				<tr>
					<td>
						<table class="list_box_bg" width="100%" border="0" cellspacing="0" cellpadding="0">
							<tr>
								<td>
									<script type="text/javascript">
										// 投稿情報取得URL
										var url = "<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_SUPPORT_INFORMATION_GETTER %>";
										// サポートサイトURL
										var support_url = "<%= Constants.SUPPORT_SITE_URL %>";
										var params = "al=on&rd=on";

										// 検索フォーム初期メッセージ
										$(function () {
											var defalut_value = "検索ワードを入力してください";
											var search_text_id = "#search_text";
											$(search_text_id).attr('placeholder', defalut_value);

											$(search_text_id).focus(function () {
												$(this).css("color", "#4b4b4b");
												$(this).removeAttr('placeholder');
											});
											$(search_text_id).blur(function () {
												$(search_text_id).attr('placeholder', defalut_value);
											});

											// 検索入力ボックス、検索ボタンイベント追加（サポートサイトウィンドウ表示）
											$(search_text_id).keypress(function (e) {
												if ($(search_text_id).val() != "" && e.which == 13) {
													open_supportsite(support_url + "?s=" + encodeURI($(search_text_id).val()) + "&" + params);
												}
											});
											$("#searchbutton").click(function (e) {
												open_supportsite(support_url + "?s=" + encodeURI($(search_text_id).val()) + "&" + params);
												e.preventDefault();
												return false;
											});
										});

										// 1週間前の日付取得　※New表示に利用
										var now = new Date();
										var nowms = now.getTime();
										var newDate = new Date(nowms - (7 * 24 * 60 * 60 * 1000));
										newDate = new Date(newDate.getFullYear(), newDate.getMonth(), newDate.getDate());

										// お知らせ
										var timeStamp = '&timestamp=' + (new Date()).getTime();
										$.getJSON(url + "?j=info" + timeStamp + "&callback=",
										function (data) {
											var id = "#info";
											var dateFormat = '<%= GlobalConfigUtil.GetDateTimeFormatText(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE, DateTimeUtility.FormatType.ShortDate2Letter) %>';
											var postDate;
											var newMessage;
											$(id).append("<dl></dl>");
											$.each(data, function (i, post) {
												if (i > 4) return false; // 5件表示

												// 公開日時取得
												postDate = new Date(post.date.split("T")[0].replace(/-/g, "/"));
												postDate = new Date(postDate.getFullYear(), postDate.getMonth(), postDate.getDate());

												//日付フォーマット
												var formattedDate = dateFormat.replace(/yyyy/, postDate.getFullYear())
													.replace(/MM/, ("0" + (postDate.getMonth() + 1)).slice(-2))
													.replace(/dd/, ("0" + postDate.getDate()).slice(-2));

												// New表示用Html取得
												newMessage = "";
												if (postDate >= newDate) {
													newMessage = "<span style=\"color:red\">New</span>"
												}

												$("<dt></dt>").append(formattedDate + " " + newMessage + "<br/>"
												+ "<a href=\"javascript:open_supportsite('" + encodeURI(post.link) + "/?" + params + "');\">" + post.title.rendered + "</a>").appendTo(id + " dl");
												$("<dd></dd>").append(post.post_excerpt_stackable).appendTo(id + " dl");
											});

											$(id + "_loading").hide();
										});

										// 最新の投稿
										$.getJSON(url + "?j=new" + timeStamp + "&callback=",
										function (data) {
											var id = "#new";
											var dateFormat = '<%= GlobalConfigUtil.GetDateTimeFormatText(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE, DateTimeUtility.FormatType.ShortDate2Letter) %>';
											var postDate;
											var newMessage;
											$(id).append("<dl></dl>");
											$.each(data, function (i, post) {
												if (i > 4) return false; // 5件表示

												// 公開日時取得
												postDate = new Date(post.date.split("T")[0].replace(/-/g, "/"));
												postDate = new Date(postDate.getFullYear(), postDate.getMonth(), postDate.getDate());

												//日付フォーマット
												var formattedDate = dateFormat.replace(/yyyy/, postDate.getFullYear())
													.replace(/MM/, ("0" + (postDate.getMonth() + 1)).slice(-2))
													.replace(/dd/, ("0" + postDate.getDate()).slice(-2));

												// New表示用Html取得
												newMessage = "";
												if (postDate >= newDate) {
													newMessage = "<span style=\"color:red\">New</span>"
												}

												$("<dt></dt>").append(formattedDate + " " + newMessage + "<br/>"
												+ "<a href=\"javascript:open_supportsite('" + encodeURI(post.link) + "/?" + params + "');\">" + post.title.rendered + "</a>").appendTo(id + " dl");
												$("<dd></dd>").append(post.post_excerpt_stackable).appendTo(id + " dl");
											});

											$(id + "_loading").hide();
										});
									</script>
									<div id="support">
										<table id="contents">
											<tr>
												<td colspan="2">
													<!-- 検索フォーム -->
													<div id="searchbox">
														<input type="text" id="search_text" value="" />
														<input type="submit" id="searchbutton" value="検索" class="btn_main">
													</div>
												</td>
											</tr>
											<tr>
												<td valign="top">
													<!-- お知らせ -->
													<fieldset id="info">
														<legend>お知らせ</legend>
														<div id="info_loading" class="loading"><img src="<%= Constants.PATH_ROOT %>Images/Common/loading.gif" alt="Loading" /></div>
													</fieldset>
												</td>
												<td valign="top">
													<!-- 最新の投稿 -->
													<fieldset id="new">
														<legend>最新の投稿</legend>
														<div id="new_loading" class="loading"><img src="<%= Constants.PATH_ROOT %>Images/Common/loading.gif" alt="Loading" /></div>
													</fieldset>
												</td>
											</tr>
										</table>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>