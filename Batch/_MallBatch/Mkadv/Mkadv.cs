/*
=========================================================================================================
  Module      : 商品コンバータメインクラス(Mkadv.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.Extensions.Currency;
using w2.Commerce.MallBatch.Mkadv.Common;
using w2.Commerce.MallBatch.Mkadv.Common.Convert;
using w2.Commerce.MallBatch.Mkadv.Common.Format;
using w2.Commerce.MallBatch.Mkadv.Common.ProductInfo;
using w2.Common.Logger;
using w2.Common.Sql;

namespace w2.Commerce.MallBatch.Mkadv
{
	///**************************************************************************************
	/// <summary>
	/// 商品コンバータメインクラス
	/// </summary>
	///**************************************************************************************
	class Mkadv
	{
		// 一時テーブル指定モードフラグ
		static int m_iTmpTableUse = -1;
		static string m_strMkadv_FilenameReplace2IdTarget = null;
		static string m_strMkadv_MsgCommandLineAlert = null;

		/// <summary>
		/// アプリケーションのメイン エントリ ポイント
		/// </summary>
		/// <param name="strArgs">引数</param>
		[STAThread]
		static void Main(string[] strArgs)
		{
			Mkadv mkadv = null;

			try
			{
				mkadv = new Mkadv();
			}
			catch (Exception ex)
			{
				Console.WriteLine("MKADV初期化中にエラー発生");
				Console.WriteLine(ex.ToString());
				Environment.Exit(-1);
			}

			try
			{
				AppLogger.WriteInfo("起動");

				// 商品コンバータ実行
				mkadv.Execute(strArgs);

				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Mkadv()
		{
			try
			{
				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				// アプリケーション名設定
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定			
				w2.App.Common.ConfigurationSetting csSetting
					= new w2.App.Common.ConfigurationSetting(
						Properties.Settings.Default.ConfigFileDirPath,
						w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C100_BatchCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C300_Mkadv);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				m_strMkadv_FilenameReplace2IdTarget = csSetting.GetAppStringSetting("Mkadv_FilenameReplace2IdTarget");
				m_strMkadv_MsgCommandLineAlert = csSetting.GetAppStringSetting("Mkadv_MsgCommandLineAlert");

			}
			catch (Exception ex)
			{
				throw new System.ApplicationException("config.xmlファイルの読み込みに失敗しました。\r\n" + ex.ToString());
			}
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="strArgs">引数</param>
		public void Execute(string[] strArgs)
		{
			string strArg5 = null;
			string strArg6 = null;
			if (strArgs.Length != 3)
			{
				if (strArgs.Length >= 4)
				{
					// 一時テーブル指定モード、グローバル一時テーブルの##を除くテーブル名指定
					m_iTmpTableUse = int.Parse(strArgs[3]);

					// モールID指定
					strArg5 = (strArgs.Length >= 5) ? strArgs[4] : "";

					// 楽天用SKU管理番号出力フォーマット
					strArg6 = (strArgs.Length >= 6) ? strArgs[5] : "";
				}
				else
				{
					// 引数の数が不正
					FileLogger.WriteWarn(m_strMkadv_MsgCommandLineAlert);
					return;
				}
			}

			// 引数の処理
			// 第一引数　出力先ID
			// そのまま

			// 第二引数　出力先パス（frontSiteRootからの相対パス）
			// 日付指定を処理
			string strArg2 = strArgs[1];
			try
			{
				Regex regex = new Regex("\\[[^\\[\\]]+\\]");
				Match match = regex.Match(strArg2);
				while (match.Success)
				{
					string strDateTimeString = DateTime.Now.ToString(match.Value);
					strDateTimeString = strDateTimeString.Substring(1, strDateTimeString.Length - 2);
					if (match.Value.Equals(m_strMkadv_FilenameReplace2IdTarget) == false)
					{
						strArg2 = regex.Replace(strArg2, strDateTimeString, 1, match.Index);
					}
					match = match.NextMatch();
				}
			}
			catch (FormatException fe)
			{
				FileLogger.WriteWarn("ファイル名の日付書式が不適切:" + fe.Message);
				strArg2 = strArgs[1];
			}
			// 第三引数　出力対象
			// そのまま

			// 第四引数　モールID
			// そのまま

			// 第五引数　楽天用SKU管理番号出力フォーマット
			// そのまま

			// 店舗ID別、出力先ID別に商品コンバータデータを作成する
			MakeAdvertise(int.Parse(strArgs[0]), strArg2, strArgs[2], strArg5, strArg6);
		}

		/// <summary>
		/// ファイル出力
		/// </summary>
		/// <param name="iTargetId">出力先ID</param>
		/// <param name="strOutputPathEx">出力先パス</param>
		/// <param name="strTargetName">データソース対象SQL名</param>
		/// <param name="strMallId">モールID</param>
		/// <param name="skuManagementIdOutputFormat"></param>
		/// <remarks>
		/// データベースからデータを取得し、出力先のフォーマットに従ってファイルに出力する。
		/// </remarks>
		private void MakeAdvertise(int iTargetId, string strOutputPathEx, string strTargetName, string strMallId, string skuManagementIdOutputFormat)
		{
			// モール監視ログ登録（バッチ起動）
			MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MKADV, strMallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, "CSVファイルの出力処理を開始しました。");

			// 出力先フルパスを作成
			string strOutputPath = Constants.PHYSICALDIRPATH_CONTENTS_ROOT + strOutputPathEx;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				//------------------------------------------------------
				// 初期処理
				//------------------------------------------------------
				// コンパイル済み出力フォーマットを取得する（共通）
				ProductConvertFormat productConvertFormat = new ProductConvertFormat(sqlAccessor, iTargetId);

				// 文字列変換テーブルを取得する
				ProductConverter productConverter = new ProductConverter(sqlAccessor, iTargetId);

				// 店舗情報を取得する（全レコード取得）
				Shop shop = new Shop(sqlAccessor, iTargetId);

				string strPath = Path.GetTempFileName(); // 一時ファイル名
				int iOutputCount = 0;

				// グローバル一時テーブル指定実行対応
				bool blIsiTmpTableUse = false;
				string strId = null;
				if (m_iTmpTableUse == -1)
				{
					strId = shop.ShopIds[0];
				}
				else
				{
					strId = m_iTmpTableUse.ToString();
					blIsiTmpTableUse = true;
				}

				//------------------------------------------------------
				// ファイル作成
				//------------------------------------------------------
				using (StreamWriter streamWriter = new StreamWriter(File.Open(@strPath, FileMode.Create), Encoding.GetEncoding(productConvertFormat.CharacterCodeType)))
				{
					// ヘッダ出力
					if (productConvertFormat.GetHeaderString() != "")
					{
						streamWriter.Write(productConvertFormat.GetHeaderString());
						streamWriter.Write(productConvertFormat.NewLineType);
					}

					// 商品情報を取得する
					Product product = new Product(
						sqlAccessor,
						strId,
						strTargetName,
						productConvertFormat.ExtractionPatternId,
						productConvertFormat.IsNeedYhoVariationInfo,
						productConvertFormat.IsNeedRtnCategoryInfo,
						blIsiTmpTableUse,
						strMallId);

					// 現在描画中の商品IDを保持
					var tempProductId = string.Empty;

					// 商品情報数分繰り返す
					foreach (Hashtable htProduct in product.Products)
					{
						// 金額に関連するフィールドのみ決済通貨による小数点以下桁数の補正を行う
						if (htProduct.ContainsKey(Constants.FIELD_PRODUCT_DISPLAY_PRICE)) htProduct[Constants.FIELD_PRODUCT_DISPLAY_PRICE] = htProduct[Constants.FIELD_PRODUCT_DISPLAY_PRICE].ToPriceDecimal();
						if (htProduct.ContainsKey(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE)) htProduct[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] = htProduct[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE].ToPriceDecimal();
						if (htProduct.ContainsKey(Constants.FIELD_PRODUCTVARIATION_PRICE)) htProduct[Constants.FIELD_PRODUCTVARIATION_PRICE] = htProduct[Constants.FIELD_PRODUCTVARIATION_PRICE].ToPriceDecimal();
						if (htProduct.ContainsKey(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)) htProduct[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] = htProduct[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE].ToPriceDecimal();

						StringBuilder sbBuilder = new StringBuilder();
						try
						{
							// コンパイル済みフォーマット（出力フォーマット）に沿って商品レコードをファイル出力する
							for (int iLoop = 0; iLoop < productConvertFormat.FormatCompile.Count; iLoop++)
							{
								string strValue = null;
								var productId = (string)htProduct[Constants.FIELD_PRODUCT_PRODUCT_ID];

								// 区切り記号
								if (iLoop != 0)
								{
									sbBuilder.Append(productConvertFormat.Separater);
								}

								//ヤフーバリエーションあり
								if (productConvertFormat.IsNeedYhoVariationInfo)
								{
									// 変換後文字列を取得する（変換前→変換後）
									strValue = productConverter.Convert(
										productConvertFormat.ColumnIds[iLoop],
										productConvertFormat.FormatCompile[iLoop].GetFormatedString(
											htProduct,
											(YahooVariation)product.HtYhoProductVariationInfo[productId],
											strMallId));
								}
								// 上記以外（通常）
								else
								{
									// 楽天SKU管理の「SKU管理番号」列かつ、SKU管理番号出力フィールドが指定されている場合、その内容を取得する
									if (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION
										&& (iLoop == productConvertFormat.SkuControlNumber + 1)
										&& string.IsNullOrEmpty(skuManagementIdOutputFormat) == false)
									{
										var format = new FormatCompile(skuManagementIdOutputFormat);
										strValue = productConverter.Convert(
											productConvertFormat.ColumnIds[iLoop],
											format.GetFormatedString(
												htProduct,
												new YahooVariation(),
												strMallId));
									}
									else
									{
										// ファイル出力値を取得する
										strValue = productConverter.Convert(
											productConvertFormat.ColumnIds[iLoop],
											productConvertFormat.FormatCompile[iLoop].GetFormatedString(
												htProduct,
												new YahooVariation(),
												strMallId));
									}
								}

								// 楽天SKUマイグレーション対象かつ、商品マスタ行が描画済みの場合、商品マスタ列は空文字とする
								// ただし1列目だけは出力する
								// また、1列目は「SKU管理番号」ではない前提とする
								strValue = Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION
									&& (tempProductId == productId)
									&& (iLoop <= productConvertFormat.SkuControlNumber)
									&& (iLoop != 0)
										? string.Empty
										: strValue;

								// 区切り文字あり？
								if (productConvertFormat.IsQuote)
								{
									sbBuilder.Append(productConvertFormat.Quote).Append(strValue.Replace(productConvertFormat.Quote.ToString(), productConvertFormat.Quote + productConvertFormat.Quote.ToString())).Append(productConvertFormat.Quote);
								}
								else
								{
									sbBuilder.Append(strValue);
								}

								// 楽天SKUマイグレーション対象かつ、商品マスタ行が描画されておらず、SKU管理番号の列の1つ手前の場合
								if (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION
									&& (iLoop == productConvertFormat.SkuControlNumber)
									&& (tempProductId != productId))
								{
									// 今書いている行は商品マスタ行とし、残り全ては空欄にしてSKU行に移動する
									for (var j = productConvertFormat.SkuControlNumber + 1; j < productConvertFormat.FormatCompile.Count; j++)
									{
										sbBuilder.Append(productConvertFormat.Separater);
									}
									sbBuilder.Append(productConvertFormat.NewLineType);

									// 1列目に1列目に入れたものと同じ値を入れる
									// 1列目は「SKU管理番号」ではない前提とする
									var column1Value = productConverter.Convert(
										productConvertFormat.ColumnIds[0],
										productConvertFormat.FormatCompile[0].GetFormatedString(
											htProduct,
											new YahooVariation(),
											strMallId));
									if (productConvertFormat.IsQuote)
									{
										sbBuilder.Append(productConvertFormat.Quote).Append(column1Value.Replace(productConvertFormat.Quote.ToString(), productConvertFormat.Quote + productConvertFormat.Quote.ToString())).Append(productConvertFormat.Quote);
									}
									else
									{
										sbBuilder.Append(column1Value);
									}

									// SKU列の最初までスキップ
									for (var i = 0; i < productConvertFormat.SkuControlNumber; i++)
									{
										sbBuilder.Append(productConvertFormat.Separater);
									}

									// 商品マスタ行が描画済みであることを示す
									tempProductId = productId;
								}
							}
							sbBuilder.Append(productConvertFormat.NewLineType);
							streamWriter.Write(sbBuilder.ToString());
							iOutputCount++;
						}
						catch (Exception ex)
						{
							// 出力失敗を破棄
							FileLogger.WriteWarn("行出力エラー:" + ex.ToString());
						}
					}
				}

				//------------------------------------------------------
				// ファイルを上書きコピー
				//------------------------------------------------------
				try
				{
					// ファイル名とshop_idを置換
					string strOutputPathSono2 = strOutputPath.Replace(m_strMkadv_FilenameReplace2IdTarget, strId);

					// ディレクトリが見つからなければ作る
					if (Directory.Exists(Path.GetDirectoryName(strOutputPathSono2)) == false)
					{
						Directory.CreateDirectory(Path.GetDirectoryName(strOutputPathSono2));
					}

					// コピーする(上書きON）
					File.Copy(strPath, strOutputPathSono2, true);

					// 相対パスを作る
					string strRelativePath = strOutputPathEx.Replace(m_strMkadv_FilenameReplace2IdTarget, strId).Replace(@"\", "/");

					// コピーする(上書きON）
					File.Copy(strPath, strOutputPathSono2, true);

					// パスをDBに投入
					using (SqlStatement sqlStatement = new SqlStatement("AdFiles", "InsertItAdFiles"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_MALLPRDCNVFILES_ADTO_ID, iTargetId);
						htInput.Add(Constants.FIELD_MALLPRDCNVFILES_PATH, strRelativePath);

						sqlStatement.ExecStatement(sqlAccessor, htInput);
					}

					// 一時ファイルを削除する
					File.Delete(strPath);

					// モール監視ログ登録（ファイル出力成功）
					MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MKADV, strMallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, "ファイルの出力に成功しました。\r\nファイル名[" + strRelativePath + "]\r\nレコード数[" + iOutputCount.ToString() + "]\r\n商品コンバータID[" + iTargetId + "]");
				}
				catch (IOException ioe)
				{
					// ファイル出力のエラーメッセージ(これはthrowしない)
					FileLogger.WriteError(ioe);
					iOutputCount = -1;

					// モール監視ログ登録（ファイル出力失敗）
					MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MKADV, strMallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, "ファイル出力エラー：ファイルの出力に失敗しました。\r\n商品コンバータID[" + iTargetId + "]\r\nエラー詳細[" + ioe.Message + "]");
				}

				// 出力行数を記録
				if (iOutputCount >= 0)
				{
					using (SqlStatement sqlStatement = new SqlStatement("Adto", "UpdateCreatedRecordCount"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_MALLPRDCNV_ADTO_ID, iTargetId);
						htInput.Add(Constants.FIELD_MALLPRDCNV_CREATEDRECORDCOUNT, iOutputCount);

						sqlStatement.ExecStatement(sqlAccessor, htInput);
					}
				}
			}
		}

		/// <summary>
		/// モール監視ログマネージャ
		/// </summary>
		public static w2.App.Common.MallCooperation.MallWatchingLogManager MallWatchingLogManager
		{
			get { return m_mallWatchingLogManager; }
		}
		public static w2.App.Common.MallCooperation.MallWatchingLogManager m_mallWatchingLogManager = new App.Common.MallCooperation.MallWatchingLogManager();
	}
}
