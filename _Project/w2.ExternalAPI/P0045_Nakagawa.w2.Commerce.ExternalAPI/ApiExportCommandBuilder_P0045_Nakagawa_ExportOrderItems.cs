/*
=========================================================================================================
  Module      : 中川政七商店 受注商品情報出力クラス(ApiExportCommandBuilder_P0045_Nakagawa_ExportOrderItems.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.App.Common;
using w2.Common.Logger;
using w2.ExternalAPI.Common.Command.ApiCommand.EntireOrder;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;
using w2.ExternalAPI.Common.Ftp;

namespace P0045_Nakagawa.w2.Commerce.ExternalAPI
{
	public class ApiExportCommandBuilder_P0045_Nakagawa_ExportOrderItems : ApiExportCommandBuilder
	{
		#region #Init 初期化処理
		/// <summary>初期化処理</summary>
		public override DataTable GetDataTable()
		{
			// コマンド生成
			GetOrderItems cmd = new GetOrderItems();

			// 引数生成
			GetOrderItemsArg arg = new GetOrderItemsArg()
			{
				CreatedTimeSpan = new PastAbsoluteTimeSpan(0,
															DateTime.Now.AddDays(-int.Parse(Properties["Timespan"])),
															DateTime.Now),
				OrderStatus = Properties["OrderStatus"], // 注文ステータス
				OrderExtendedStatusSpecification =  // 連携フラグがOFF
							OrderExtendedStatusSpecification.GenByString(string.Format("({0}F)", Properties["OutputCsvFlag"]))
			};

			// コマンド実行
			GetOrderItemsResult result = (GetOrderItemsResult)cmd.Do(arg);

			return result.ResultTable;
		}
		#endregion

		#region #Export 出力処理
		/// <summary>出力処理</summary>
		protected override object[] Export(IDataRecord record)
		{
			// 注文情報
			DataTable orderData = GetOrder(record["OrderID"].ToString());
			DataRow order = orderData.Rows[0];
			// 注文者情報
			DataTable orderOwnerData = GetOrderOwner(record["OrderID"].ToString());
			DataRow orderOwner = orderOwnerData.Rows[0];
			// 注文商品情報
			DataTable orderItemData = GetOrderItem(record["OrderID"].ToString());
			var productPriceTax = TaxCalculationUtility.GetTaxPrice(
						(decimal)record["ProductPrice"],
						(decimal)record["TaxRate"],
						(string)record["TaxRoundType"]);

			var priceShippingIncludedTax = TaxCalculationUtility.GetPriceTaxIncluded(
				(decimal)order["order_price_shipping"],
				(decimal)order["order_price_shipping_tax"]);

			var priceExchangeIncludedTax = TaxCalculationUtility.GetPriceTaxIncluded(
				(decimal)order["order_price_exchange"],
				(decimal)order["order_price_exchange_tax"]);

			var result = new List<object>
			{
				record["OrderID"].ToString().Replace("-", ""),	// 1.システム受注番号（楽天受注番号はハイフン除く）
				record["OrderID"],	// 2.サイト受注番号
				GetShoppingMallCode(order),	// 3.ショッピングモールコード
				"",	// 4.ショッピングモール名
				GetStoreCode(order),	// 5.ストアコード
				"",	// 6.ストア名
				order["order_date"],	// 7.受注日時
				order["order_cancel_date"],	// 8.キャンセル日付
				"",	// 9.顧客コード
				orderOwner["owner_name"].ToString(),	// 10.注文者名
				"",	// 11.注文者名カナ
				"",	// 12.注文者郵便番号
				"",	// 13.注文者住所
				"",	// 14.注文者電話番号
				"",	// 15.注文者FAX番号
				"",	// 16.注文者メールアドレス
				"",	// 17.注文備考
				"",	// 18.社内備考
				"",	// 19.倉庫指示
				"",	// 20.警告内容
				"",	// 21.受注進捗コード
				"",	// 22.受注進捗名
				"",	// 23.決済方法コード
				"",	// 24.決済方法名
				"",	// 25.支払回数コード
				"",	// 26.支払回数名
				"",	// 27.モバイル注文フラグ
				GetPoint(order).ToString().Replace("-",""),	// 28.ポイント(マイナス符号除去)
				GetCoupon(order).ToString().Replace("-",""),	// 29.クーポン(マイナス符号除去)
				TaxCalculationUtility.GetPriceTaxExcluded((decimal)order["order_price_subtotal"], (decimal)order["order_price_subtotal_tax"]),	// 30.税抜合計
				order["order_price_subtotal_tax"],	// 31.消費税
				TaxCalculationUtility.GetPriceTaxIncluded((decimal)order["order_price_subtotal"], (decimal)order["order_price_subtotal_tax"]),	// 32.税込合計
				priceShippingIncludedTax,	// 33.配送料
				priceExchangeIncludedTax,	// 34.代引手数料
				order["order_price_total"],	// 35.請求金額
				"",	// 36.同梱フラグ
				"",	// 37.配送番号
				"",	// 38.楽天用ID
				"",	// 39.配送先名
				"",	// 40.配送先名カナ
				"",	// 41.配送先郵便番号
				"",	// 42.配送先住所
				"",	// 43.配送先電話番号
				"",	// 44.配送先FAX番号
				"",	// 45.配送進捗
				"",	// 46.配送進捗名
				"",	// 47.配送方法
				"",	// 48.配送方法名
				"",	// 49.配送時間
				"",	// 50.配送時間名
				"",	// 51.配送料1
				"",	// 52.代引手数料1
				"",	// 53.ギフトフラグ
				"",	// 54.出荷日
				"",	// 55.出荷確定日
				"",	// 56.配送日
				"",	// 57.荷物番号
				"",	// 58.備考
				record["ItemNo"],	// 59.商品番号
				"",	// 60.楽天用ID1
				record["VariationID"],	// 61.商品コード
				"",	// 62.商品名
				"",	// 63.項目選択肢横軸
				"",	// 64.項目選択肢縦軸
				record["ItemQuantity"],	// 65.数量
				TaxCalculationUtility.GetPriceTaxExcluded(
					(decimal)record["ProductPrice"],
					productPriceTax).ToString(),	// 66.税抜単価
				productPriceTax.ToString(),	// 67.単品消費税
				TaxCalculationUtility.GetPriceTaxExcluded((decimal)record["ItemPrice"], (decimal)record["ItemPriceTax"]).ToString(),	 // 68.税抜金額
				record["ItemPriceTax"],	// 69.消費税1
				"",	// 70.税込金額
				"",	// 71.社内備考1
				"",	// 72.出荷備考
				"",	// 73.データ登録日時
				"",	// 74.データ更新日時
				"",	// 75.データ更新者コード
				"",	// 76.データ更新者名
			};

			if (this.Properties["OrderStatus"] != "ODR")
			{
				result.Add((record["CooperationID4"].ToString() != string.Empty) ? record["CooperationID4"] : "0");	// 77.PIC除外フラグ
			}

			return result.ToArray();
		}
		#endregion

		#region
		/// <summary>
		/// 注文情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報</returns>
		private DataTable GetOrder(string orderId)
		{
			// コマンド生成
			GetEntireOrder cmd = new GetEntireOrder();

			// 引数生成
			GetEntireOrderArg arg = new GetEntireOrderArg()
			{
				DataType = OrderDataType.Order,
				ShopId = "0",
				OrderId = orderId
			};

			// コマンド実行
			return ((GetEntireOrderResult)cmd.Do(arg)).ResultTable;
		}
		#endregion

		#region
		/// <summary>
		/// 注文者情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報</returns>
		private DataTable GetOrderOwner(string orderId)
		{
			// コマンド生成
			GetEntireOrder cmd = new GetEntireOrder();

			// 引数生成
			GetEntireOrderArg arg = new GetEntireOrderArg()
			{
				DataType = OrderDataType.OrderOwner,
				ShopId = "0",
				OrderId = orderId
			};

			// コマンド実行
			return ((GetEntireOrderResult)cmd.Do(arg)).ResultTable;
		}
		#endregion

		#region
		/// <summary>
		/// 注文商品情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報</returns>
		private DataTable GetOrderItem(string orderId)
		{
			// コマンド生成
			var cmd = new GetEntireOrder();

			// 引数生成
			var arg = new GetEntireOrderArg
			{
				DataType = OrderDataType.OrderItem,
				ShopId = "0",
				OrderId = orderId
			};

			// コマンド実行
			return ((GetEntireOrderResult)cmd.Do(arg)).ResultTable;
		}
		#endregion

		#region
		/// <summary>
		/// サイト注文ID取得
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>サイト注文ID</returns>
		private string GetSiteOrderId(DataRow order)
		{
			if ((string)order["mall_id"] != "OWN_SITE")
			{
				return (string)order["order_id"];
			}
			else
			{
				return "";
			}
		}
		#endregion

		#region
		/// <summary>
		/// ショッピングモールコード取得
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>ショッピングモールコード</returns>
		private string GetShoppingMallCode(DataRow order)
		{
			if ((string)order["mall_id"] == "rakuten1")
			{
				return "1";
			}
			else if (((string)order["order_kbn"] == "TEL")
				|| ((string)order["order_kbn"] == "FAX"))
			{
				return "99";
			}
			else
			{
				return "2";
			}
		}
		#endregion

		#region
		/// <summary>
		/// ストアコード取得
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>ストアコード</returns>
		private string GetStoreCode(DataRow order)
		{
			if ((string)order["mall_id"] == "rakuten1")
			{
				return "2";
			}
			else if (((string)order["order_kbn"] == "TEL")
				|| ((string)order["order_kbn"] == "FAX"))
			{
				return "3";
			}
			else
			{
				return "1";
			}
		}
		#endregion

		#region
		/// <summary>
		/// ポイント取得
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>ポイント</returns>
		private decimal GetPoint(DataRow order)
		{
			return ((string)order["mall_id"] == "rakuten1") ? GetPointPriceRakuten((string)order["relation_memo"]) : -(decimal)order["order_point_use_yen"];
		}
		#endregion

		#region
		/// <summary>
		/// Get Coupon
		/// </summary>
		/// <param name="order">order</param>
		/// <returns>Price Coupon</returns>
		private decimal GetCoupon(DataRow order)
		{
			// 自社サイトの場合
			// クーポン = 「クーポン割引額」+「セットプロモーション割引額」+「会員ランク割引額」+「調整金額」
			decimal priceCouponOwnerSite = (decimal)order["order_price_regulation"]
											- (decimal)order["order_coupon_use"]
											- (decimal)order["setpromotion_product_discount_amount"]
											- (decimal)order["setpromotion_shipping_charge_discount_amount"]
											- (decimal)order["setpromotion_payment_charge_discount_amount"]
											- (decimal)order["member_rank_discount_price"];

			// 楽天サイトの場合
			// hack:他の割引が始まった場合はクーポン割引の計算方法を見直す必要あり 2014/08 時点
			decimal priceCouponRakuten = ((decimal)order["order_price_regulation"] - GetPointPriceRakuten((string)order["relation_memo"]));

			return ((string)order["mall_id"] == "rakuten1") ? priceCouponRakuten : priceCouponOwnerSite;
		}
		#endregion

		#region
		/// <summary>
		/// Get Point Price Rakuten
		/// </summary>
		/// <param name="relationMemo">Relation Memo</param>
		/// <returns>Point of Rakuten</returns>
		private decimal GetPointPriceRakuten(string relationMemo)
		{
			var pattern = new Regex(@"\[ポイント\] (-\d+)円");
			return pattern.IsMatch(relationMemo) ? decimal.Parse(pattern.Split(relationMemo)[1]) : 0;
		}
		#endregion

		#region #Switch flags
		/// <summary>
		/// 書き込み完了したらフラグをたてる
		/// </summary>
		/// <param name="objects"></param>
		public override void PostExport(object[] objects)
		{
			// 連携フラグをONに
			var cmd = new UpdateOrderExtendedStatus();
			var arg = new UpdateOrderExtendedStatusArg(objects[1].ToString(), int.Parse(Properties["OutputCsvFlag"]), true);
			cmd.Do(arg);
		}
		#endregion

		#region
		/// <summary>
		/// CSVファイルの生成後に実行
		/// </summary>
		/// <param name="filePath">生成したファイルのフルパス</param>
		public override void EndExport(string filePath)
		{
			if (File.Exists(filePath) == false)
			{
				FileLogger.WriteError("FTPアップロード予定のファイルが存在しません ファイル名: " + filePath);
				throw new Exception("FTPアップロード予定のファイルが存在しません ファイル名: " + filePath);
			}

			AddFileHeader(filePath);

			if (FtpFileUpload(filePath))
			{
				FileLogger.WriteInfo("ファイルのFTPアップロードが完了しました ファイル名 : " + filePath);
			}
			else
			{
				FileLogger.WriteError("ファイルのFTPアップロードに失敗しました ファイル名 : " + filePath);
				throw new Exception("ファイルのFTPアップロードに失敗しました ファイル名 : " + filePath);
			}
		}
		#endregion

		#region
		/// <summary>
		/// 指定したファイルの先頭にヘッダーを追加
		/// </summary>
		/// <param name="filePath">ヘッダーを追加するファイルのフルパス</param>
		private void AddFileHeader(string filePath)
		{
			var enc = Encoding.GetEncoding(ExternalAPI.Properties.Settings.Default.ftpUploadFileEncode);
			File.WriteAllText(
				filePath,
				　　((string.IsNullOrEmpty(ExternalAPI.Properties.Settings.Default.header))? "" : ExternalAPI.Properties.Settings.Default.header + "\r\n")
					+ File.ReadAllText(filePath, enc), enc);
		}
		#endregion

		#region
		/// <summary>
		/// 指定したファイルをFTPアップロード
		/// </summary>
		/// <param name="filePath">アップロードするファイルのフルパス</param>
		/// <returns>成功時はtrue,失敗時はfalse</returns>
		private bool FtpFileUpload(string filePath)
		{
			var fluentFtpUtill = new FluentFtpUtility(
				ExternalAPI.Properties.Settings.Default.ftpHost,
				ExternalAPI.Properties.Settings.Default.ftpUserName,
				ExternalAPI.Properties.Settings.Default.ftpUserPassword,
				ExternalAPI.Properties.Settings.Default.ftpUseActive,
				ExternalAPI.Properties.Settings.Default.ftpEnableSsl);

			var uploadDirName = "";
			foreach (string dirName in ExternalAPI.Properties.Settings.Default.ftpUploadDirectory)
			{
				if (filePath.IndexOf(dirName) > 0)
				{
					uploadDirName = dirName;
					break;
				}
			}

			var result = fluentFtpUtill.Upload(
				filePath,
					((string.IsNullOrEmpty(uploadDirName)) ? "" : "/" + uploadDirName + "/")
					+ Path.GetFileNameWithoutExtension((new FileInfo(filePath).Name))
					+ ExternalAPI.Properties.Settings.Default.ftpUploadFileExtension);
			return result;
		}
		#endregion
	}
}
