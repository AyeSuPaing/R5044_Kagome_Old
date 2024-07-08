/*
=========================================================================================================
  Module      : 表示商品作成バッチ処理(CreateDispProduct.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using w2.App.Common.RefreshFileManager;
using w2.App.Common;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Common.Sql;
using w2.Domain.DispProductInfo;
using w2.Domain.ProductRanking;

namespace w2.Commerce.Batch.CreateDispProduct
{
	class CreateDispProduct
	{
		private static int m_iCollectDays = 0;

		/// <summary>
		/// メイン
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			try
			{
				// 実体作成
				CreateDispProduct program = new CreateDispProduct();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				// ランキングIDの指定がある場合は該当情報のみ集計
				if (args.Length > 1)
				{
					for (var i = 1; i < args.Length; i++)
					{
						var productRanking = new ProductRankingService().Get(args[0], args[i]);

						// 商品ランキング情報を取得
						if (productRanking == null)
						{
							// 例外をスローする
							throw new Exception("商品ランキングID[" + args[i] + "]が存在しません。");
						}
						program.Execute(productRanking);
					}
				}
				else
				{
					program.Execute();
				}

				// 統計情報の更新
				new DispProductInfoService().UpdateStatistics();

				// フロント系サイトの表示を最新状態にする
				RefreshFileManagerProvider.GetInstance(RefreshFileType.DisplayProduct).CreateUpdateRefreshFile();

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");

				// メール送信
				SendMail();
			}
			catch (Exception ex)
			{
				string strErrorMessage = ex.ToString();

				try
				{
					// メール送信
					SendMail(ex);
				}
				catch (Exception ex2)
				{
					strErrorMessage += "\r\n" + ex2.ToString();
				}

				// エラーイベントログ出力
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CreateDispProduct()
		{
			// 設定初期化
			Iniitialize();
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private void Iniitialize()
		{
			try
			{
				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				// アプリケーション名設定
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定
				ConfigurationSetting csSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_CreateDispProduct);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// メール送信先設定
				Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");

				// ランキング集計日数設定
				m_iCollectDays = csSetting.GetAppIntSetting("Collect_Days");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// メール送信処理（成功時）
		/// </summary>
		private static void SendMail()
		{
			SendMail(null);
		}
		/// <summary>
		/// メール送信処理（失敗時）
		/// </summary>
		/// <param name="ex">例外（NULLなら成功）</param>
		private static void SendMail(Exception ex)
		{
			using (SmtpMailSender smsMailSender = new SmtpMailSender(Constants.SERVER_SMTP))
			{
				// メール送信デフォルト値設定
				smsMailSender.SetSubject(Constants.MAIL_SUBJECTHEAD);
				smsMailSender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => smsMailSender.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => smsMailSender.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => smsMailSender.AddBcc(mail.Address));

				if (ex == null)
				{
					smsMailSender.SetBody("成功");
				}
				else
				{
					smsMailSender.SetBody(BaseLogger.CreateExceptionMessage(ex));
				}

				// メール送信
				bool blResult = smsMailSender.SendMail();
				if (blResult == false)
				{
					Exception ex2 = smsMailSender.MailSendException;
					FileLogger.WriteError(ex2);
				}
			}
		}

		/// <summary>
		/// データ作成実行
		/// </summary>
		private void Execute()
		{
			ArrayList alStatemntNames = new ArrayList();

			// ステートメントの一覧を取得
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(Constants.PHYSICALDIRPATH_SQL_STATEMENT + "Statements.xml");
			XmlNode xnStatement = xmlDoc.SelectSingleNode("Statements").FirstChild;
			while (xnStatement != null)
			{
				if (xnStatement.NodeType != XmlNodeType.Comment)
				{
					alStatemntNames.Add(xnStatement.Name);
				}

				xnStatement = xnStatement.NextSibling;
			}

			var currentDate = DateTime.Today;

			// ＳＱＬ発行
			foreach (string statementName in alStatemntNames)
			{
				using (var accessor = new SqlAccessor())
				using (var statement = new SqlStatement("Statements", statementName) { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
				{
					var input = new Hashtable
						{
							{ Constants.FIELD_PRODUCT_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
							{ "current_date", currentDate },
							{ "collect_date", currentDate.AddDays(m_iCollectDays * (-1)) },
						};

					statement.ExecStatementWithOC(accessor, input);
				}
			}

			// 商品ランキング情報の集計タイプは「自動」かつ有効フラグが「有効」の商品ランキングIDを取得して集計
			var productRankings = new ProductRankingService().GetAll();
			foreach (var productRanking in productRankings)
			{
				Execute(productRanking);
			}
		}

		/// <summary>
		/// ランキングデータ作成実行
		/// </summary>
		/// <param name="productRanking">商品ランキング設定マスタモデル</param>
		private void Execute(ProductRankingModel productRanking)
		{
			try
			{
				// 商品ランキング情報の設定を基に注文データの集計結果を取得
				var dispProductInfoes = new DispProductInfoService().GetTotalResultOrder(productRanking);

				// 表示用のランキング情報を作成
				var displayRanking = CreateRankingForDisplay(productRanking.Items, dispProductInfoes);

				//表示用のランキング情報をDBへ格納
				StoreRankingForDisplay(productRanking.ShopId, productRanking.RankingId, displayRanking);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// 表示用のランキング情報を作成
		/// </summary>
		/// <param name="rankingItems">商品ランキングアイテムモデル</param>
		/// <param name="dispProductInfoes">商品表示情報マスタモデル</param>
		/// <returns>ランキング表示用Dictionary</returns>
		private static Dictionary<int, string> CreateRankingForDisplay(ProductRankingItemModel[] rankingItems, DispProductInfoModel[] dispProductInfoes)
		{
			// 一時テーブルを作成
			var tmpDisplayRanking = new Dictionary<int, string>();
			var rowCount = dispProductInfoes.Length + ((rankingItems.Length > 0) ? rankingItems[rankingItems.Length - 1].Rank : 0);
			for (var i = 1; i <= rowCount; i++)
			{
				tmpDisplayRanking.Add(i, "");
			}

			// 一時テーブルにランキングアイテム情報の固定フラグが有効な商品IDを格納
			if (rankingItems.Length != 0)
			{
				foreach (var rankingItem in rankingItems)
				{
					if (rankingItem.FixationFlg == Constants.FLG_PRODUCTRANKINGITEM_FIXATION_FLG_ON)
					{
						tmpDisplayRanking[rankingItem.Rank] = rankingItem.ProductId;
					}
				}
			}

			// ランキングアイテム情報と注文データの集計結果を合わせたランキング情報を作成
			var rank = 1;
			foreach (var dispProductInfo in dispProductInfoes)
			{
				var productRepetition = false;

				// 固定フラグが有効な商品IDと重複する場合は固定を優先
				foreach (var rankingItem in rankingItems)
				{
					if ((dispProductInfo.ProductId.ToLower() == rankingItem.ProductId.ToLower())
						&& (rankingItem.FixationFlg == Constants.FLG_PRODUCTRANKINGITEM_FIXATION_FLG_ON))
					{
						productRepetition = true;
						break;
					}
				}

				if (productRepetition == false)
				{
					// 格納先の順位に商品が設定済みの場合繰り下げて格納
					while (rank <= tmpDisplayRanking.Count)
					{
						if ((string)tmpDisplayRanking[rank] == "")
						{
							tmpDisplayRanking[rank++] = dispProductInfo.ProductId;
							break;
						}
						rank++;
					}
				}
			}
			// 可変分追加
			foreach (var rankingItem in rankingItems)
			{
				if (rankingItem.FixationFlg == Constants.FLG_PRODUCTRANKINGITEM_FIXATION_FLG_ON) continue;
				bool addFlag = true;
				foreach (var displayRanking in tmpDisplayRanking)
				{
					if (rankingItem.ProductId == displayRanking.Value)
					{
						addFlag = false;
						break;
					}
				}
				if (addFlag)
				{
					// 格納先の順位に商品が設定済みの場合繰り下げて格納
					while (rank <= tmpDisplayRanking.Count)
					{
						if (tmpDisplayRanking[rank] == "")
						{
							tmpDisplayRanking[rank++] = rankingItem.ProductId;
							break;
						}
						rank++;
					}
				}
			}
			return tmpDisplayRanking;
		}

		/// <summary>
		/// 表示用のランキング情報をDBへ格納
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">ランキングID</param>
		/// <param name="dicDisplayRanking">ランキング表示用Dictionary</param>
		private static void StoreRankingForDisplay(string shopId, string rankingId, Dictionary<int, string> dicDisplayRanking)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				// トランザクション開始
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();
				var dispProductInfoService = new DispProductInfoService();

				// 商品表示情報マスタ削除
				dispProductInfoService.Delete(shopId, rankingId, sqlAccessor);

				// 商品表示情報マスタ登録
				foreach (int rank in dicDisplayRanking.Keys)
				{
					// 空行は追加しない
					if (string.IsNullOrEmpty(dicDisplayRanking[rank]) == false)
					{
						var dispProductInfo = new DispProductInfoModel
						{
							ShopId = shopId,
							DataKbn = rankingId,
							DisplayOrder = rank,
							ProductId = dicDisplayRanking[rank]
						};
						dispProductInfoService.Insert(dispProductInfo, sqlAccessor);
					}
				}

				// トランザクションコミット
				sqlAccessor.CommitTransaction();
			}
		}
	}
}