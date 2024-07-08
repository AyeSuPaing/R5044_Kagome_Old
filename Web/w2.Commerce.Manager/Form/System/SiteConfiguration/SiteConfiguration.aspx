<%--
=========================================================================================================
  Module      : 設定情報ページ(SiteConfiguration.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Global.Config" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SiteConfiguration.aspx.cs" Inherits="Form_Configuration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
	<script type="text/javascript" >
		$(document).ready(function () {
			navigateToTab();

			searchFunction();

			// ラジオボタンの切り替えで、hf要素に値を格納
			$('.rbValueTrue, .rbValueFalse').change(function () {
				var $row = $(this).closest('tr');
				var $hfConfigurationValue = $row.find('.hfConfigurationValue');
				if ($(this).hasClass('rbValueTrue')) {
					$hfConfigurationValue.val("TRUE");
				} else {
					$hfConfigurationValue.val("FALSE");
				}
			});

			// 各設定値の入力が変更されたら、hf要素に値を格納
			$('.tbValue').change(function () {
				var $row = $(this).closest('tr');
				var $hfConfigurationValue = $row.find('.hfConfigurationValue');
				$hfConfigurationValue.val($(this).val());
			});

			// 検索ボタンのクリックイベントまたはエンターキーでの発火
			$(document).on('click', '#searchbutton', searchFunction);
			$('#<%= tbSearchText.ClientID %>').keydown(function (e) {
				if (e.which === 13) { // エンター
					e.preventDefault();
					searchFunction();
				}
			});

			$(document).ready(function () {
				$(".error-link").click(function (e) {
					// デフォルトのリンクの動作を停止
					e.preventDefault();

					// データ属性からターゲットのクラスを取得
					var targetClass = $(this).data("target");

					// ターゲットのエレメントを取得
					var targetElement = $(targetClass);

					// ターゲットのエレメントが存在する場合
					if (targetElement.length) {
						$('html, body').animate({
							scrollTop: targetElement.offset().top -100
						}, 500); // 0.5秒かけてスクロール
					}
				});
			});
		});

		function navigateToTab() {

			// クライアントIDを使用してドロップダウンリストの要素を取得
			var $ddlTabs = $('#<%= ddlTabs.ClientID %>');

			var tabValue = $ddlTabs.find('option:selected').val();
			// すべてのタブ要素を非表示
			var tabs = document.querySelectorAll('[id^="tabs-"]');
			tabs.forEach(function (tab) {
				tab.style.display = 'none';
			});

			// 選択されたタブだけを表示
			var selectedTab = document.querySelector(tabValue);
			if (selectedTab) {
				selectedTab.style.display = 'block';
			}
			else {
				// すべてのタブ要素を表示
				var tabs = document.querySelectorAll('[id^="tabs-"]');
				tabs.forEach(function (tab) {
					tab.style.display = 'block';
				});
			}
		}

		function searchFunction(e) {
			// フォームのサブミットを防ぐ
			if (e) e.preventDefault();
			// 入力された検索テキストを取得
			var searchText = $('#<%= tbSearchText.ClientID %>').val();

			if (searchText) {
				// すべての行を非表示にする
				$('[class^="line-"]').hide();

				// 部分一致の検索
				$('[class^="line-"]').each(function () {
					var className = this.className;
					if (className.indexOf(searchText) !== -1) {
						$(this).show();
					}
				});
			} else {
				// すべての行を表示
				$('[class^="line-"]').show();
			}
		}

		// コンフィグ読み込み確認
		function confirmLoadingConfig() {
			return confirm("<%: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CONFIG_LOAD_ALERT) %>");
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td><img height="10" alt="" src="../Images/Common/sp.gif" width="100%" border="0" /></td>

		</tr>
		<tr>
			<td><h1 class="page-title">サイト設定<br /><br />サイト設定編集</h1></td>
		</tr>
		<tr>
			<td>
				<asp:DropDownList ID="ddlTabs" runat="server" onchange="navigateToTab(this.value)" />
				<input type="text" runat="server" id="tbSearchText" class="search_text2" placeholder="設定キーを入力（例：Setting_Production_Environment）" value="" />
				<input type="submit" id="searchbutton" value="検索" class="btn_main" />
			</td>
		</tr>
	</table>
	<table class="list_table" runat="server" id="tbErrorMessage" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr>
			<td class="edit_title_bg" align="center">エラーメッセージ</td>
		</tr>
		<tr>
			<td class="error_message">
				<asp:Label runat="server" ID="lbErrorMessage" ForeColor="red" />
			</td>
		</tr>
	</table>
	
	<div id="divComp" runat="server" class="action_part_top" Visible="FALSE">
		<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
			<tr class="info_item_bg">
				<td align="left">サイト設定を更新しました。
				</td>
			</tr>
		</table>
	</div>
	<table cellspacing="0" cellpadding="0" border="0">
		<tr>
			<td>
				<asp:Button ID="btnReloadTop" runat="server" Text="設定再読み込み" OnClick="btnReload_Click" CssClass="btn_main2" />

			</td>
			<td align="right">
				<asp:Button ID="btnConfirmTop" Visible="false" runat="server" Text="確認する" CssClass="btn_main3" OnClick="btnConfirm_Click" />
				<asp:Button ID="btnModifyTop" Visible="false" runat="server" Text="編集する" CssClass="btn_main3" OnClick="btnModify_Click" OnClientClick="return confirmLoadingConfig();" />
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<div id="tabs">
					<asp:Repeater ID="rReadKbn" runat="server" DataSource="<%# this.ReadKbnList %>" >
						<ItemTemplate>
							<div id="tabs-<%# Container.ItemIndex + 1 %>">
								<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
									<tbody>
										<tr>
											<td class="edit_title_bg" align="center" width="40%" rowspan="3">キー</td>
											<td class="edit_title_bg" align="center" width="60%">説明</td>
										</tr>
										<tr>
											<td class="edit_title_bg" align="center">値<br/></td>
										</tr>
										<tr>
											<td class="edit_title_bg" align="center">AppAll.Configの値<br/></td>
										</tr>
										<asp:Repeater id="rSettings" runat="server" DataSource="<%# GetConfigSettings((string)Container.DataItem) %>" ItemType="w2.App.Common.SettingNode">
											<ItemTemplate>
												<tr runat="server" visible='<%# IsDisplayReadKbn(Item.ReadKbn)%>'>
													<td class="edit_title_bg " align="left" colspan="2">
														<h2><%#: Item.ReadKbn %></h2>
													</td>
												</tr>
												<tr class="line-<%# Item.Key %>">
													<td class="edit_title_bg" align="left" style="word-break:break-all" rowspan="3">
														<span data-popover-message="読取区分「<%#: Item.ReadKbn %>」"><%#: Item.Key %></span>
													</td>
													<td class="edit_item_bg" align="left" style="word-break:break-all"><%#: Item.Comment %></td>
												</tr>
												<tr class="line-<%# Item.Key %>">
													<td class="edit_item_bg" align="left" style="word-break:break-all">
														<span runat="server" Visible="<%# this.IsUseRadioButton(Item) %>">
															TRUE<asp:RadioButton ID="rbValueTrue" CssClass="rbValueTrue" Checked='<%# Item.Value == "TRUE" %>' GroupName="ConfigSettings" runat="server" Enabled="<%# this.IsDisplayOnly == false %>" />
															FALSE<asp:RadioButton ID="rbValueFalse" CssClass="rbValueFalse" Checked='<%# Item.Value == "FALSE" %>' GroupName="ConfigSettings" runat="server" Enabled="<%# this.IsDisplayOnly == false %>" />
														</span>
														<asp:TextBox ID="tbValue" CssClass="tbValue" runat="server" Text='<%# Item.Value %>' Visible="<%# this.IsUseRadioButton(Item) == false %>" BorderWidth="1px" Width="650" Enabled="<%# this.IsDisplayOnly == false %>" />
														<input type="hidden" id ="hfConfigurationReadKbn" class="hfConfigurationReadKbn" value='<%# Item.ReadKbn %>' runat="server" />
														<input type="hidden" id="hfConfigurationKey" class="hfConfigurationKey" value='<%# Item.Key %>' runat="server" />
														<input type="hidden" id="hfConfigurationValue" class="hfConfigurationValue" value='<%# Item.Value %>' runat="server" />
														<asp:Label runat="server" Visible="false" ID="lbErrorMessageLine" ForeColor="red" />
													</td>
												</tr>
												<tr class="line-<%# Item.Key %>">
													<td class="edit_item_bg" align="left" style="word-break:break-all">
														<%#: string.IsNullOrEmpty(Item.AppAllValue) ? "（値なし）" : Item.AppAllValue %>
													</td>
												</tr>
											</ItemTemplate>
										</asp:Repeater>
									</tbody>
								</table>
							</div>
						</ItemTemplate>
					</asp:Repeater>
				</div>
			</td>
		</tr>
		<tr>
			<td>
				<asp:Button ID="btnReloadBottom" runat="server" Text="設定再読み込み" OnClick="btnReload_Click" CssClass="btn_main2" />

			</td>
			<td align="right">
				<asp:Button ID="btnConfirmBottom" Visible="false" runat="server" Text="確認する" CssClass="btn_main3" OnClick="btnConfirm_Click" />
				<asp:Button ID="btnModifyBottom" Visible="false" runat="server" Text="編集する" CssClass="btn_main3" OnClick="btnModify_Click" OnClientClick="return confirmLoadingConfig();" />
			</td>
		</tr>
	</table>
</asp:Content>
