/*
=========================================================================================================
  Module      : Yahoo注文取込メソッド(ImportYahooOrderMethod.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using w2.Commerce.MallBatch.MallOrderImporter.Utils;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Logger;
using w2.App.Common.Option;
using w2.Commerce.MallBatch.MallOrderImporter.HtmlObjects;

namespace w2.Commerce.MallBatch.MallOrderImporter.Method
{
	class ImportYahooOrderMethod : BaseMethod
	{
		/// <summary>ヤフートップURL（クッキー取得用）</summary>
		private const string URL_YAHOO_TOP = "http://www.yahoo.co.jp";
		/// <summary>ヤフーログインURL</summary>
		private const string URL_YAHOO_LOGIN = "https://login.yahoo.co.jp/config/login";
		/// <summary>ヤフーストアプロのURL</summary>
		private const string URL_YAHOO_STORE_PRO = "https://pro.store.yahoo.co.jp";
		/// <summary>ページエラー時のエラーメッセージ</summary>
		private readonly static string[] PAGEERROR_MESSAGE =
		{
			"しばらくしてから、もう一度実行してください。",
			"しばらく時間をおいてから再度お試しください。"
		};

		/// <summary>
		/// ヤフーモール注文情報取得
		/// </summary>
		/// <returns>成功/失敗　※失敗はメール送信</returns>
		public void Import()
		{
			try
			{
				//------------------------------------------------------
				this.TransactionName = "０．設定読み込み";
				Console.WriteLine(this.TransactionName);
				//------------------------------------------------------
				var yahooCooperationYahooSettings = GetMallCooperationYahooSettings();
				if (yahooCooperationYahooSettings.Count == 0) WriteWarnAndThrow("モール連携設定が取得できませんでした。");
				this.MallId = (string)yahooCooperationYahooSettings[0][Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID];

				var yahooSetting = new MallOrderImportSettingYahoo((string)yahooCooperationYahooSettings[0][Constants.FIELD_MALLCOOPERATIONSETTING_ORDER_IMPORT_SETTING]);
				if (yahooSetting.YahooStoreAccountName == "") WriteWarnAndThrow("Yahoo注文取込設定が見つかりませんでした。");

				// 下記テスト用。各値を書き換えてください。
				//var yahooSetting = new MallOrderImportSettingYahoo("");	// ここは空のままでOK
				//yahooSetting.YahooStoreAccountName = "";
				//yahooSetting.YahooStoreLoginId = "";
				//yahooSetting.YahooStoreLoginPassword = "";
				//yahooSetting.YahooBusinessManagerLoginId = "";
				//yahooSetting.YahooBusinessManagerLoginPassword = "";
				//yahooSetting.YahooOrderCsvLinkName = "【w2Commerce連携用】注文情報CSV";
				//yahooSetting.YahooCustomFieldCsvLinkName = "【w2Commerce連携用】カスタムCSV";
				var siteTopUrl = URL_YAHOO_STORE_PRO + "/pro." + yahooSetting.YahooStoreAccountName;

				//------------------------------------------------------
				this.TransactionName = "１．文字認証用クッキー取得";
				Console.WriteLine(this.TransactionName);
				//------------------------------------------------------
				{
					// Yahooトップページへ遷移してクッキーを取得する（前もってクッキーを取得しておかないとログイン時に画像文字列認証を求められてしまう）
					var roYahooTopPage = ExecGetRequest(URL_YAHOO_TOP, Encoding.GetEncoding("EUC-JP"));
					if (roYahooTopPage.Html.Contains("ただいまシステムメンテナンス中です。"))
					{
						WriteWarnAndThrow("Yahooがメンテナンス中です。しばらくした後また試してください。");
						return;
					}
				}

				//------------------------------------------------------
				this.TransactionName = "２．Yahooストアマネージャーログイン";
				Console.WriteLine(this.TransactionName);
				//------------------------------------------------------
				{
					// ヤフーログイン
					var yahooLoginResponse = TryYahooLogin(URL_YAHOO_LOGIN, yahooSetting);
					if (yahooLoginResponse == null) WriteWarnAndThrow("Yahooログインに失敗しました。");

					// ストアマネージャProログイン
					yahooLoginResponse = TryYahooLogin(siteTopUrl, yahooSetting);
					if (yahooLoginResponse == null) WriteWarnAndThrow("Yahooログインに失敗しました。");
					var responseHtml = yahooLoginResponse.Html;

					// 2015/02/18 このメッセージが出たら再ログインが必要
					if (responseHtml.Contains("お客様のYahoo! JAPANビジネスIDはすでにYahoo! JAPAN IDと連携されています。"))
					{
						yahooLoginResponse = ExecGetRequest(siteTopUrl, Encoding.UTF8); ;
						responseHtml = yahooLoginResponse.Html;
					}

					// ログイン再試行？
					if (responseHtml.Contains("パスワードの再確認")
						|| (responseHtml.Contains("ログインしてください")))
					{
						var yahooLoginResponse2 = TryYahooLogin(yahooLoginResponse, yahooSetting);
						responseHtml = yahooLoginResponse2.Html;
					}

					// リダイレクト先URL取得
					foreach (Match match in Regex.Matches(responseHtml, @"window\.location\.replace\(.*?\)"))
					{
						var nextUrl = match.Value.Replace("window.location.replace(\"", "").Replace("\")", "");
						var redirectPage = ExecGetRequest(nextUrl, Encoding.GetEncoding("EUC-JP"));
						responseHtml = redirectPage.Html;
						break;
					}
					// ログイン試行の中（2014/08/19から対応のUTF8取得）でこのhtmlが取得できることもある。
					foreach (Match match2 in Regex.Matches(responseHtml, @"CONTENT=""0; URL=(.*?)"">"))
					{
						if (match2.Groups.Count >= 2) this.SiteUrlTop = match2.Groups[1].Value;
						break;
					}
					//// リンクがあることもある。（2015/02/13）
					//foreach (Match match3 in Regex.Matches(redirectPageHtml, @"<a href=""(.*?)"">ご利用中のサービスに戻る</a>"))
					//{
					//	if (match3.Groups.Count >= 2) this.SiteUrlTop = match3.Groups[1].Value;
					//	break;
					//}

					if (this.SiteUrlTop == null)
					{
						//AppLogger.WriteFatal("サイトトップURLが取得できませんでした：\r\n" + responseHtml);
						this.SiteUrlTop = siteTopUrl;
					}
				}

				//------------------------------------------------------
				this.TransactionName = "３．Yahooストアマネージャトップから注文管理メニューへ";
				Console.WriteLine(this.TransactionName);
				//------------------------------------------------------
				string orderSearchUrl = null;
				{
					// 管理メニュートップページ取得
					var topPage = ExecGetRequest(this.SiteUrlTop, Encoding.UTF8);
					// リフレッシュページに遷移することがあるのでもう一回アクセス
					if (topPage.Html.Contains(string.Format("URL={0}\"", this.SiteUrlTop)))
					{
						topPage = ExecGetRequest(this.SiteUrlTop, Encoding.UTF8);
					}

					// 選択画面だったら直に遷移
					if (topPage.Html.Contains("ストアアカウント選択"))
					{
						var refreshPage = ExecGetRequest(siteTopUrl, Encoding.UTF8);
						foreach (Match match2 in Regex.Matches(refreshPage.Html, @"CONTENT=""0; URL=(.*?)"">"))
						{
							if (match2.Groups.Count >= 2) this.SiteUrlTop = match2.Groups[1].Value;
							break;
						}
						topPage = ExecGetRequest(this.SiteUrlTop, Encoding.UTF8);
					}

					// ページエラーであれば抜ける
					CheckPageError(topPage.Html);

					// 「ログアウト」リンク取得
					const string LINK_NAME_LOGOUT = "ログアウト";
					if (topPage.Html.Contains(LINK_NAME_LOGOUT))
					{
						this.LogoutUrl = GenarateNextUrl(topPage.Url, GetAnchorUrl(topPage.Html, LINK_NAME_LOGOUT));
					}

					// キーワードが見つからない場合は同意画面を表示している可能性がある 　※Proではまだ未確認
					const string LINK_NAME_ORDER_SEARCH = "注文検索";
					if (topPage.Html.Contains(LINK_NAME_ORDER_SEARCH) == false)
					{
						var regulationForm = GetFormInfo(topPage.Html, new Regex("fF[0-9]+"));
						if (regulationForm == null) WriteWarnAndThrow(this.TransactionName + "でフォームが取得できませんでした。", topPage.Html);
						topPage = ExecPostRequest(topPage.Url, regulationForm.Action, regulationForm.Params, Encoding.UTF8);
					}

					// 「注文検索」リンク取得
					if (topPage.Html.Contains(LINK_NAME_ORDER_SEARCH))
					{
						orderSearchUrl = GenarateNextUrl(topPage.Url, GetAnchorUrl(topPage.Html, LINK_NAME_ORDER_SEARCH));
					}
					// 権限無しなどでリンクが取得できない場合は抜ける
					if (orderSearchUrl == null) WriteWarnAndThrow(this.TransactionName + "で「" + LINK_NAME_ORDER_SEARCH + "」リンクが取得できませんでした。アカウントの権限を確認してください。");
				}

				//------------------------------------------------------
				this.TransactionName = "４．CSVダウンロード画面取得";
				Console.WriteLine(this.TransactionName);
				//------------------------------------------------------
				ResponseObject csvDownloadPage = null;
				{
					// 注文検索ページ取得
					var orderSearchPage = ExecGetRequest(orderSearchUrl, Encoding.UTF8);

					// ページエラーであれば抜ける
					CheckPageError(orderSearchPage.Html);

					// フォーム情報取得（「検索」のsubmit分を削除）
					var orderSearchForm = GetFormInfo(orderSearchPage.Html, new Regex("f"));
					if (orderSearchForm == null) WriteWarnAndThrow(this.TransactionName + "でフォームが取得できませんでした。", orderSearchPage.Html);

					orderSearchForm.Action = this.SiteUrlTop + "/order/manage/index/select-download-file";
					orderSearchForm.Params["view_type"] = "index";
					// ※今のところデフォルトで過去一日分のデータを取得しようとしているので条件はそのままとする
					// 開始日を前日に
					//orderSearchForm.Params["OrderTimeFromDay"] = DateTime.Now.AddDays(-3).ToString("yyyy/MM/dd");
					//orderSearchForm.Params["OrderTimeFromHour"] = "0";
					//orderSearchForm.Params["OrderTimeToDay"] = DateTime.Now.ToString("yyyy/MM/dd");
					//orderSearchForm.Params["OrderTimeToHour"] = "0";
					// 「注文ステータス」を「新規注文」にセット
					orderSearchForm.Params["OrderStatusE"] = "2f";	// 新規注文
					//orderSearchForm.Params["OrderStatusE"] = "2t";	// 処理中

					// CSVダウンロードページ取得（該当なしの場合、トップに遷移している場合は抜ける）
					csvDownloadPage = ExecPostRequest(orderSearchPage.Url, orderSearchForm.Action, orderSearchForm.Params, Encoding.UTF8);
					if (csvDownloadPage.Html.Contains("該当する注文がありません")) return;
					if (csvDownloadPage.Html.Contains("<title>トップ - ストアクリエイターPro</title>")) return;
				}

				//------------------------------------------------------
				this.TransactionName = "５．CSVダウンロード（受注ファイル、カスタムフィールドファイル）";
				Console.WriteLine(this.TransactionName);
				//------------------------------------------------------
				var csvPathInfos = new List<CsvPathInfo>();
				{
					// ページエラーであれば抜ける
					CheckPageError(csvDownloadPage.Html);

					// CSVダウンロードフォーム情報取得
					var csvDownloadForm = GetFormInfo(csvDownloadPage.Html, new Regex("f"));
					if (csvDownloadForm == null) WriteWarnAndThrow(this.TransactionName + "でフォームが取得できませんでした。", csvDownloadPage.Html);

					// 受注ファイルCSVダウンロード
					if (yahooSetting.YahooOrderCsvLinkName != "")
					{
						csvPathInfos.Add(new CsvPathInfo(Constants.MASTER_IMPORT_KBN_ADD_YAHOO_ORDER));
						if (DownloadCsvAndSetFileName(csvDownloadForm, yahooSetting.YahooOrderCsvLinkName, csvPathInfos[0]) == false)
						{
							WriteWarnAndThrow(this.TransactionName + "でリンクが見つかりませんでした[" + yahooSetting.YahooOrderCsvLinkName + "]", csvDownloadPage.Html);
						}
					}

					// カスタムフィールドCSVダウンロード
					if (yahooSetting.YahooCustomFieldCsvLinkName != "")
					{
						csvPathInfos.Add(new CsvPathInfo(Constants.MASTER_IMPORT_KBN_ADD_YAHOO_CUSTOM_FIELDS));
						if (DownloadCsvAndSetFileName(csvDownloadForm, yahooSetting.YahooCustomFieldCsvLinkName, csvPathInfos[1]) == false)
						{
							WriteWarnAndThrow(this.TransactionName + "でリンクが見つかりませんでした[" + yahooSetting.YahooCustomFieldCsvLinkName + "]", csvDownloadPage.Html);
						}
					}
					if (csvPathInfos.Count == 0) WriteWarnAndThrow(this.TransactionName + "でリンクが１つも見つかりませんでした。", csvDownloadPage.Html);
				}

				//------------------------------------------------------
				this.TransactionName = "６．注文チェック（受注ファイルのみチェック対象）";
				Console.WriteLine(this.TransactionName);
				//------------------------------------------------------
				{
					bool hasTarget = false;
					using (var fs = new FileStream(csvPathInfos[0].UploadFilePath, FileMode.Open))
					using (var sr = new StreamReader(fs, Encoding.GetEncoding("Shift_JIS")))
					{
						var headers = StringUtility.SplitCsvLine(sr.ReadLine());
						using (var sqlAccessor = new SqlAccessor())
						{
							sqlAccessor.OpenConnection();

							while (sr.EndOfStream == false)
							{
								// フィールドに改行が含まれる場合を考慮して行を結合（CSVの行に改行がある場合、「"」は奇数個のはず）
								string strLineBuffer = sr.ReadLine();
								while ((((strLineBuffer.Length - strLineBuffer.Replace("\"", "").Length) % 2) != 0) && (sr.EndOfStream == false))
								{
									strLineBuffer += "\r\n" + sr.ReadLine();
								}

								// １行をCSV分割・フィールド数がヘッダの数と合っているかチェック
								var csvLine = StringUtility.SplitCsvLine(strLineBuffer);
								if (headers.Length != csvLine.Length) break;
								string orderId = csvLine[Array.IndexOf(headers, "OrderId")];

								// 注文見つからなければエラーとする
								var order = GetOrder(orderId, sqlAccessor);
								if (order.Count == 0)
								{
									WriteWarnAndThrow("注文IDが存在しません。この注文情報はスキップされます。(order_id:" + orderId + ")");
									continue;
								}

								if (StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_ORDER_STATUS]) == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
								{
									// 仮注文の商品がひとつでもあればbreak
									hasTarget = true;
									break;
								}
							}
						}
					}

					if (hasTarget == false)
					{
						// 受注ファイルに仮注文がなければ全ファイル削除
						foreach (var filePath in csvPathInfos.Select(i => i.UploadFilePath))
						{
							File.Delete(filePath);
						}
					}
				}

				//------------------------------------------------------
				this.TransactionName = "７．CSV取込";
				Console.WriteLine(this.TransactionName);
				//------------------------------------------------------
				{
					foreach (var info in csvPathInfos)
					{
						if (info.UploadFilePath == null) continue;

						// 処理ファイルディレクトリ作成
						if (Directory.Exists(info.ActiveDirectoryPath) == false) Directory.CreateDirectory(info.ActiveDirectoryPath);
						if (File.Exists(info.UploadFilePath) == false) continue;
						{
							// ファイル移動（プロセス生成に時間がかかることがあるため、移動後のファイルをバッチへ渡す。）
							File.Move(info.UploadFilePath, info.ActiveFilePath);

							// プロセス実行（移動後ファイルのフルパスを引数として渡す。）
							var arguments = new StringBuilder().Append(Constants.CONST_DEFAULT_SHOP_ID).Append(" ").Append(info.FileType).Append(" ").Append(info.FileName);

							System.Diagnostics.Process.Start(Constants.PHYSICALDIRPATH_EXTERNAL_EXE, arguments.ToString()).WaitForExit();	// スペースが含まれても処理されるように「"」でくくる
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("「" + this.TransactionName + "」でエラーが発生しました", ex);
			}
			finally
			{
				//------------------------------------------------------
				this.TransactionName = "８．ログアウト（しないとYahooに怒られるらしい）";
				Console.WriteLine(this.TransactionName);
				//------------------------------------------------------
				if (this.LogoutUrl != null)
				{
					ExecGetRequest(this.LogoutUrl, Encoding.GetEncoding("EUC-JP"));
				}
			}
		}

		/// <summary>
		/// 注文情報取得
		/// </summary>
		/// <param name="orderId"></param>
		/// <param name="sqlAccessor"></param>
		/// <returns></returns>
		private static DataView GetOrder(string orderId, SqlAccessor sqlAccessor)
		{
			using (var sqlStatement = new SqlStatement("YahooOrderImport", "GetOrder"))
			{
				var ht = new Hashtable();
				ht.Add(Constants.FIELD_ORDER_ORDER_ID, orderId);

				var dv = sqlStatement.SelectSingleStatement(sqlAccessor, ht);
				return dv;
			}
		}

		/// <summary>
		/// CSVダウンロード
		/// </summary>
		/// <param name="csvDownloadForm">CSVダウンロードフォーム</param>
		/// <param name="csvLinkName">CSVリンク名</param>
		/// <param name="csvPathInfo">CSVパス情報</param>
		/// <returns>成功失敗</returns>
		private bool DownloadCsvAndSetFileName(FormInfo csvDownloadForm, string csvLinkName, CsvPathInfo csvPathInfo)
		{
			// 右記の様なタグを取得：<input type="hidden" name="order_template_list[2][dl_templ_name]" value="【w2Commerce連携用】注文情報CSV">
			var nameParams = csvDownloadForm.Params.Where(p => p.Value == csvLinkName).ToArray();
			if (nameParams.Length == 0) return false;

			// 右記のようなタグから値を取得：<input type="hidden" name="order_template_list[2][dl_templ_id]" value="449550">
			var fileNumber = csvDownloadForm.Params[nameParams[0].Key.Replace("[dl_templ_name]", "[dl_templ_id]")];
			// 遷移用アクションセット
			csvDownloadForm.Params["file_number"] = fileNumber;
			csvDownloadForm.Params["dl_order_list"] = csvDownloadForm.Params2["dl_order_list0"];
			csvDownloadForm.Action = this.SiteUrlTop + "/order/manage/index/download-file";

			// CSV保存
			csvPathInfo.FileName = ExecPostRequestFileDownload(csvDownloadForm.Action, csvDownloadForm.Params, Encoding.GetEncoding("Shift_JIS"), csvPathInfo.UploadDirectoryPath);
			return (csvPathInfo.FileName != null);
		}

		/// <summary>
		/// Yahooログイン試行
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="setting">モール連携設定</param>
		/// <returns>ログイン後レスポンス</returns>
		private ResponseObject TryYahooLogin(string url, MallOrderImportSettingYahoo setting)
		{
			// ログインページ取得
			var loginPage = ExecGetRequest(url, Encoding.UTF8);

			// レスポンス取得
			var response = TryYahooLogin(loginPage, setting);
			return response;
		}
		/// <summary>
		/// Yahooログイン試行
		/// </summary>
		/// <param name="loginPage">ログインページ</param>
		/// <param name="setting">モール連携設定</param>
		/// <returns>ログイン後レスポンス</returns>
		private ResponseObject TryYahooLogin(ResponseObject loginPage, MallOrderImportSettingYahoo setting)
		{
			// フォームセット
			var yahooLoginFormInfo = GetFormInfo(loginPage.Html, "login_form");
			if (yahooLoginFormInfo == null)
			{
				WriteWarnAndThrow(loginPage.Url + "：ログインフォームが取得できませんでした。", loginPage.Html);
				return null;
			}
			if (Regex.IsMatch(loginPage.Html, @"\nYahoo\! JAPANビジネスIDでログイン\n"))
			{
				yahooLoginFormInfo.Params["user_name"] = setting.YahooBusinessManagerLoginId;
				yahooLoginFormInfo.Params["password"] = setting.YahooBusinessManagerLoginPassword;
			}
			else
			{
				yahooLoginFormInfo.Params["login"] = setting.YahooStoreLoginId;
				yahooLoginFormInfo.Params["passwd"] = setting.YahooStoreLoginPassword;
			}

			// 文字認証解除するため、隠しタグ（<hidden name=".albatross">）の値を書き換える
			// ※本隠しタグの値をPOST先でチェックし、文字認証画面を表示させている
			var m = Regex.Match(loginPage.Html, @"document\.getElementsByName\(""\.albatross""\)\[0\].value = ""(?<albatross>.*)"";", RegexOptions.IgnoreCase);
			if (m.Groups.Count != 0) yahooLoginFormInfo.Params[".albatross"] = m.Groups["albatross"].Value;

			// ログイン試行（リダイレクトページ）
			var response = ExecPostRequest(loginPage.Url, yahooLoginFormInfo.Action, yahooLoginFormInfo.Params,  Encoding.UTF8);

			//// Yahooのバグ？ 2014/08/19より、たまにページはEUC-JP指定なのにUTF-8で転送されてくることがあるのでその対応
			//// またその時、二回目の最ログインで Refresh ページが取得されることがあるようだ
			//if ((response.Html.Contains("。") == false)
			//	&& (response.Html.Contains("<META HTTP-EQUIV=\"Refresh\" ") == false))
			//{
			//	response = ExecPostRequest(loginPage.Url, yahooLoginFormInfo.Action, yahooLoginFormInfo.Params, Encoding.GetEncoding("EUC-JP"));
			//	AppLogger.WriteWarn("ログイン試行で「。」が取得できなかったためUTF-8で取得し直します。再取得結果：" + response.Html.Contains("。"));
			//}
			if (response.Html.Contains("<!-- エラー -->\n<div "))
			{
				var match = Regex.Match(response.Html, @"<!-- エラー -->\n[^\n]*\n[^\n]*\n[^\n]*\n", RegexOptions.Singleline);
				if (match.Success)
				{
					throw new Exception("ログイン処理でエラーあり：" + match.Value);
				}
			}
			return response;
		}

		/// <summary>
		/// モール連携設定取得
		/// </summary>
		/// <returns></returns>
		private DataView GetMallCooperationYahooSettings()
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("MallCooperationSetting", "GetMallCooperationSetting"))
			{
				var ht = new Hashtable();
				ht.Add(Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID);
				ht.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN, Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO);
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}

		/// <summary>
		/// ページエラー判定（エラーがあれば例外発生）
		/// </summary>
		/// <param name="html">HTML</param>
		private void CheckPageError(string html)
		{
			foreach (var message in PAGEERROR_MESSAGE.Where(html.Contains))
			{
				WriteWarnAndThrow(string.Format(this.TransactionName + "でページに文言「{0}」が見つかりました。", message));
			}
		}

		/// <summary>サイトごとのURL</summary>
		private string SiteUrlTop { get; set; }
		/// <summary>ログアウトURL</summary>
		private string LogoutUrl { get; set; }
	}
}
