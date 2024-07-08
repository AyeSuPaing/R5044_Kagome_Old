/*
=========================================================================================================
  Module      : ヤフー受注データ追加取込処理(ImportAddYahooOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.Mall.Yahoo.YahooMallOrders;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.Commerce.Batch.ExternalFileImport.Imports
{
	class ImportAddYahooOrder : ImportBase
	{
		///// <summary>店舗ID</summary>
		//protected string m_strShopId = null;

		///// <summary>データタイプ</summary>
		//protected string m_strFileType = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		public ImportAddYahooOrder(string strShopId)
			: base(strShopId, "AddYahooOrder")
		{
		}

		/// <summary>
		/// ファイル取込
		/// </summary>
		/// <param name="strActiveFilePath">取り込みファイルパス</param>
		/// <returns>取込件数</returns>
		public override int Import(string strActiveFilePath)
		{
			var iImportCount = 0;
			var userService = new UserService();

			using (var fs = new FileStream(strActiveFilePath, FileMode.Open))
			using (var sr = new StreamReader(fs, Encoding.GetEncoding(Constants.CONST_ENCODING_DEFAULT)))
			{
				var alHeaders = new ArrayList(StringUtility.SplitCsvLine(sr.ReadLine()));

				//------------------------------------------------------
				// トランザクション
				//------------------------------------------------------
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				{
					sqlAccessor.OpenConnection();
					sqlAccessor.BeginTransaction();

					var strOrderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED;	// ★注文ステータスは注文済みとする
					string strOrderId = null;
					var iItemIndex = 1;

					while (sr.EndOfStream == false)
					{
						// フィールドに改行が含まれる場合を考慮して行を結合（CSVの行に改行がある場合、「"」は奇数個のはず）
						var strLineBuffer = sr.ReadLine();
						while ((((strLineBuffer.Length - strLineBuffer.Replace("\"", "").Length) % 2) != 0)
							&& (sr.EndOfStream == false))
						{
							strLineBuffer += "\r\n" + sr.ReadLine();
						}

						// １行をCSV分割・フィールド数がヘッダの数と合っているかチェック
						var strDatas = StringUtility.SplitCsvLine(strLineBuffer);
						if (alHeaders.Count != strDatas.Length)
						{
							FileLogger.WriteError("ヘッダのフィールド数とフィールド数が一致しません(" + (iImportCount + 1) + "行目)");
							continue;
						}

						// キーブレイク判定
						var strOrderIdTemp = strDatas[alHeaders.IndexOf("OrderId")];
						if (strOrderId != strOrderIdTemp)
						{
							var existUser = false;
							var user = new UserModel();
							var htOrder = new Hashtable();
							var htOrderOwner = new Hashtable();
							var htOrderShipping = new Hashtable();

							var userId = string.Empty;
							var tempUserId = string.Empty;

							strOrderId = strOrderIdTemp;
							iItemIndex = 1;

							try
							{
								// Validate data
								ValidateData(alHeaders, strDatas);

								//------------------------------------------------------
								// 注文情報存在チェック
								//------------------------------------------------------
								DataView dvOrder;
								using (SqlStatement sqlStatement = new SqlStatement(m_strFileType, "CheckOrderId"))
								{
									Hashtable htInput = new Hashtable
									{
										{ Constants.FIELD_ORDER_SHOP_ID, m_strShopId },
										{ Constants.FIELD_ORDER_ORDER_ID, strOrderId },
									};

									dvOrder = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
								}

								if (dvOrder.Count == 0)
								{
									FileLogger.WriteError("注文IDが存在しません。この注文情報はスキップされます。(order_id:" + strOrderId + ")");
									continue;
								}

								// 仮注文情報のユーザーID取得
								tempUserId = StringUtility.ToEmpty(dvOrder[0][Constants.FIELD_ORDER_USER_ID]);

								// モールID & メールアドレスからユーザーID取得
								userId = StringUtility.ToEmpty(
									userService.GetUserId(
										StringUtility.ToEmpty(dvOrder[0][Constants.FIELD_ORDER_MALL_ID]),
										StringUtility.ToEmpty(strDatas[alHeaders.IndexOf("BillMailAddress")]),
									sqlAccessor));

								// ユーザーが存在しない場合、ユーザーID発行
								existUser = (string.IsNullOrEmpty(userId) == false);
								if (existUser == false)
								{
									userId = tempUserId;
								}

								user.UserId = userId;
								// ユーザ情報設定
								SetUserData(user, dvOrder, alHeaders, strDatas, strOrderId);

								//------------------------------------------------------
								// 注文情報取得
								//------------------------------------------------------
								htOrder.Add(Constants.FIELD_ORDER_SHOP_ID, m_strShopId);
								htOrder.Add(Constants.FIELD_ORDER_ORDER_ID, strOrderId);
								htOrder.Add(Constants.FIELD_ORDER_USER_ID, userId);
								htOrder.Add(Constants.FIELD_ORDER_ORDER_STATUS, strOrderStatus);

								FileLogger.WriteDebug("配送方法取得完了");

								htOrder.Add(Constants.FIELD_ORDER_RELATION_MEMO, "\r\n－－お届け方法－－\r\n" + strDatas[alHeaders.IndexOf("ShipMethodName")]); // 追加

								var strPaymentMethod = strDatas[alHeaders.IndexOf("PayMethod")];
								var strPaymentKbn = ConvertValue("OrderPaymentKbnAndPaymentStatusSettings", strPaymentMethod, "payment_kbn").Trim();
								var strPaymentStatus = ConvertValue("OrderPaymentKbnAndPaymentStatusSettings", strPaymentMethod, "payment_status").Trim();
								if (string.IsNullOrEmpty(strPaymentKbn) || string.IsNullOrEmpty(strPaymentStatus))
								{
									throw new ApplicationException("決済種別が取得できません。(order_id:" + strOrderId + ")");
								}

								htOrder.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, strPaymentKbn); // 決済種別ID
								htOrder.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, strPaymentStatus); // 入金ステータス

								var buyerComments = (string.IsNullOrEmpty(strDatas[alHeaders.IndexOf("BuyerComments")]) == false)
									? "－－コメント－－\r\n" + strDatas[alHeaders.IndexOf("BuyerComments")]
									: string.Empty;
								var giftWrapMessage = (string.IsNullOrEmpty(strDatas[alHeaders.IndexOf("GiftWrapMessage")]) == false)
									? "\r\n－－ギフトメッセージ－－\r\n" + strDatas[alHeaders.IndexOf("GiftWrapMessage")]
									: string.Empty;
								htOrder.Add(Constants.FIELD_ORDER_MEMO, buyerComments + giftWrapMessage);
								htOrder.Add(Constants.FIELD_ORDER_CARD_INSTRUMENTS, strDatas[alHeaders.IndexOf("CardPayCount")]);

								// 決済
								var orderPayment = new YahooMallOrderPayment(
									strPaymentMethod,
									strDatas[alHeaders.IndexOf("CombinedPayMethod")],
									strDatas[alHeaders.IndexOf("PayMethodAmount")],
									strDatas[alHeaders.IndexOf("CombinedPayMethodAmount")],
									strDatas[alHeaders.IndexOf("Total")],
									strDatas[alHeaders.IndexOf("UsePoint")],
									strDatas[alHeaders.IndexOf("TotalMallCouponDiscount")],
									strDatas[alHeaders.IndexOf("GiftWrapCharge")],
									new YahooMallOrderPaymentMapper());
								htOrder.Add(Constants.FIELD_ORDER_REGULATION_MEMO, orderPayment.RegulationMemo);
								htOrder.Add(Constants.FIELD_ORDER_ORDER_PRICE_REGULATION, orderPayment.OrderPriceRegulation);
								htOrder.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, Convert.ToDecimal(strDatas[alHeaders.IndexOf("Total")]));
								FileLogger.WriteDebug("注文情報取得完了");

								//------------------------------------------------------
								// 注文者情報取得
								//------------------------------------------------------
								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_ORDER_ID, strOrderId);
								switch (strDatas[alHeaders.IndexOf("DeviceType")])
								{
									case "1":
										htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN, Constants.FLG_ORDEROWNER_OWNER_KBN_PC_USER);
										break;

									case "2":
										htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN, Constants.FLG_ORDEROWNER_OWNER_KBN_MOBILE_USER);
										break;

									case "3":
										htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN, Constants.FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_USER);
										break;

									case "4":
										htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN, Constants.FLG_ORDEROWNER_OWNER_KBN_PC_USER);
										break;

									default:
										throw new ApplicationException("注文者区分が取得できません。(order_id:" + strOrderId + ")");
								}

								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR, strDatas[alHeaders.IndexOf("BillMailAddress")]);
								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME, user.Name);
								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME1, user.Name1);
								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME2, user.Name2);
								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ZIP, user.Zip);
								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR1, strDatas[alHeaders.IndexOf("BillPrefecture")]);
								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR2, StringUtility.ToZenkaku(strDatas[alHeaders.IndexOf("BillCity")]));
								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR3, StringUtility.ToZenkaku(strDatas[alHeaders.IndexOf("BillAddress1")]));
								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR4, StringUtility.ToZenkaku(strDatas[alHeaders.IndexOf("BillAddress2")]));
								htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL1, strDatas[alHeaders.IndexOf("BillPhoneNumber")]);
								FileLogger.WriteDebug("注文者情報取得完了");

								//------------------------------------------------------
								// 注文配送先情報取得
								//------------------------------------------------------
								var shippingName = strDatas[alHeaders.IndexOf("ShipName")].Split(' ');
								var shippingName1 = StringUtility.ToZenkaku((shippingName.Length == 2) ? shippingName[0] : string.Join("", shippingName));
								var shippingName2 = StringUtility.ToZenkaku((shippingName.Length == 2) ? shippingName[1] : "");

								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_ORDER_ID, strOrderId);
								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME, shippingName1 + shippingName2);
								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1, shippingName1);
								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2, shippingName2);

								var strShipZip = ProcessingZip(strOrderId, strDatas[alHeaders.IndexOf("ShipZipCode")]);
								if (string.IsNullOrEmpty(strShipZip))
								{
									throw new ApplicationException("郵便番号が正しくありません。(order_id:" + strOrderId + ")");
								}
								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP, strShipZip);

								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1, strDatas[alHeaders.IndexOf("ShipPrefecture")]);
								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2, StringUtility.ToZenkaku(strDatas[alHeaders.IndexOf("ShipCity")]));
								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3, StringUtility.ToZenkaku(strDatas[alHeaders.IndexOf("ShipAddress1")]));
								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4, StringUtility.ToZenkaku(strDatas[alHeaders.IndexOf("ShipAddress2")]));
								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1, strDatas[alHeaders.IndexOf("ShipPhoneNumber")]);

								bool isAnotherShippingFlagValid = ((StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME1]) != (StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1])))
									|| (StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME2]) != (StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2])))
									|| (StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ZIP]) != (StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP])))
									|| (StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR1]) != (StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1])))
									|| (StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR2]) != (StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2])))
									|| (StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR3]) != (StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3])))
									|| (StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR4]) != (StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4])))
									|| (StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL1]) != (StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1]))));

								htOrderShipping.Add(
									Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG,
									isAnotherShippingFlagValid
										? Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID
										: Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID);

								var strShipPhones = ProcessingTel(strOrderId, strDatas[alHeaders.IndexOf("ShipPhoneNumber")]);
								if (strShipPhones == null)
								{
									throw new ApplicationException("電話番号が正しくありません。(order_id:" + strOrderId + ")");
								}

								string strShippingReqDate = null;
								if ((strDatas[alHeaders.IndexOf("ShipRequestDate")] != "お届け日指定なし")
									&& (string.IsNullOrEmpty(strDatas[alHeaders.IndexOf("ShipRequestDate")]) == false))
								{
									strShippingReqDate = ProcessingDateTime(strDatas[alHeaders.IndexOf("ShipRequestDate")]);
								}
								htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, strShippingReqDate);

								// 配送希望時間帯をチェックし、配送種別IDを登録する
								if (string.IsNullOrEmpty(strDatas[alHeaders.IndexOf("ShipRequestTime")]) == false)
								{
									DataView dvShopShipping;
									using (SqlStatement sqlStatement = new SqlStatement(m_strFileType, "GetShippingFromOrderId"))
									{
										Hashtable htInput = new Hashtable
										{
											{ Constants.FIELD_ORDER_SHOP_ID, m_strShopId },
											{ Constants.FIELD_ORDER_ORDER_ID, strOrderId }
										};
										dvShopShipping = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
									}

									for (var iLoop = 1; iLoop <= 10; iLoop++)
									{
										if (strDatas[alHeaders.IndexOf("ShipRequestTime")].Trim().TrimStart('0').Replace("-", "～") == StringUtility.ToEmpty(dvShopShipping[0]["shipping_time_message" + iLoop]).Trim().TrimStart('0').Replace("-", "～"))
										{
											// 配送種別IDを取得する
											htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME, dvShopShipping[0]["shipping_time_id" + iLoop]);
											break;
										}
									}
									if (htOrderShipping.Contains(Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME) == false)
									{
										throw new ApplicationException("配送種別IDを取得できません。配送種別に配送希望時間帯を登録してください。(" + m_strFileType + " order_id:" + strOrderId + " shipping_time_message:" + strDatas[alHeaders.IndexOf("ShipRequestTime")] + ")");
									}
								}
								else
								{
									// 配送希望時間帯の指定がない場合は、空を登録する
									htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME, "");
								}
								FileLogger.WriteDebug("配送先情報取得完了");
							}
							catch (Exception ex)
							{
								string errorMessage = "値チェック処理で例外エラーが発生しました。(order_id:" + strOrderId + ")\r\n" + ex.ToString();
								FileLogger.WriteError(errorMessage);

								// モール連携監視ログ出力
								w2.App.Common.MallCooperation.MallWatchingLogManager mallWatchingLogManager = new App.Common.MallCooperation.MallWatchingLogManager();
								mallWatchingLogManager.Insert(
									Constants.FLG_MALLWATCHINGLOG_BATCH_ID_EXTERNALFILEIMPORT,
									"",
									Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
									errorMessage);

								// エラーの場合は、次のレコードへ
								continue;
							}

							//------------------------------------------------------
							// データベース更新
							//------------------------------------------------------
							try
							{
								// ユーザー情報登録
								var updateUser = userService.Get(user.UserId);

								updateUser.UserKbn = user.UserKbn;
								updateUser.MailAddr = user.MailAddr;
								updateUser.MailAddr2 = user.MailAddr2;
								updateUser.Name = user.Name;
								updateUser.Name1 = user.Name1;
								updateUser.Name2 = user.Name2;
								updateUser.NameKana = string.Empty;
								updateUser.NameKana1 = string.Empty;
								updateUser.NameKana2 = string.Empty;
								updateUser.Zip = user.Zip;
								updateUser.Zip1 = user.Zip1;
								updateUser.Zip2 = user.Zip2;
								updateUser.Addr = user.Addr;
								updateUser.Addr1 = user.Addr1;
								updateUser.Addr2 = user.Addr2;
								updateUser.Addr3 = user.Addr3;
								updateUser.Addr4 = user.Addr4;
								updateUser.Tel1 = user.Tel1;
								updateUser.Tel1_1 = user.Tel1_1;
								updateUser.Tel1_2 = user.Tel1_2;
								updateUser.Tel1_3 = user.Tel1_3;
								updateUser.LastChanged = user.LastChanged;

								userService.UpdateWithUserExtend(updateUser, UpdateHistoryAction.DoNotInsert, sqlAccessor);

								// 仮注文時に登録したユーザー削除
								if (existUser)
								{
									userService.Delete(
										tempUserId,
										Constants.FLG_LASTCHANGED_BATCH,
										UpdateHistoryAction.DoNotInsert,
										sqlAccessor);
								}

								// 注文情報
								var orderSqlStatementName = ((string)htOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.PAYMENT_METHOD_PAYPAY)
									? "AddOrderForPayPay"
									: "AddOrder";
								using (SqlStatement sqlStatement = new SqlStatement(m_strFileType, orderSqlStatementName))
								{
									if (sqlStatement.ExecStatement(sqlAccessor, htOrder) != 1)
									{
										throw new ApplicationException("注文情報が更新できません。(" + m_strFileType + " order_id:" + strOrderId + ")");
									}
								}

								// 注文者情報
								using (SqlStatement sqlStatement = new SqlStatement(m_strFileType, "AddOrderOwner"))
								{
									sqlStatement.ExecStatement(sqlAccessor, htOrderOwner);
								}

								// 注文配送先情報
								using (SqlStatement sqlStatement = new SqlStatement(m_strFileType, "AddOrderShipping"))
								{
									sqlStatement.ExecStatement(sqlAccessor, htOrderShipping);
								}

								// 更新履歴登録
								var updateHistoryService = new UpdateHistoryService();
								updateHistoryService.InsertForOrder(strOrderId, Constants.FLG_LASTCHANGED_BATCH, sqlAccessor);
								updateHistoryService.InsertForUser(user.UserId, Constants.FLG_LASTCHANGED_BATCH, sqlAccessor);

								// トランザクションコミット
								sqlAccessor.CommitTransaction();
							}
							catch (Exception ex)
							{
								// トランザクションロールバック
								sqlAccessor.RollbackTransaction();

								// エラーの場合は、次のレコードへ
								FileLogger.WriteError("DB更新処理で例外エラーが発生しました。(order_id:" + strOrderId + ")\r\n" + ex.Message);
								continue;
							}

						} // キーブレイク

						iItemIndex++;
						iImportCount++;
					}
				}
			}

			return iImportCount;
		}

		/// <summary>
		/// ユーザ情報作成する
		/// </summary>
		/// <param name="user">ユーザ情報</param>
		/// <param name="dvOrder">注文情報</param>
		/// <param name="alHeaders">ヘッダ情報</param>
		/// <param name="strDatas">データ情報</param>
		/// <param name="strOrderId">注文ID</param>
		private void SetUserData(UserModel user, DataView dvOrder, ArrayList alHeaders, string[] strDatas, string strOrderId)
		{
			user.MallId = StringUtility.ToEmpty(dvOrder[0][Constants.FIELD_ORDER_MALL_ID]);

			string mailAddress = strDatas[alHeaders.IndexOf("BillMailAddress")];
			switch (strDatas[alHeaders.IndexOf("DeviceType")])
			{
				case "1":
					user.UserKbn = Constants.FLG_USER_USER_KBN_PC_USER;
					user.MailAddr = mailAddress;
					user.MailAddr2 = string.Empty;
					break;

				case "2":
					user.UserKbn = Constants.FLG_USER_USER_KBN_MOBILE_USER;
					user.MailAddr = string.Empty;
					user.MailAddr2 = mailAddress;
					break;

				case "3":
					user.UserKbn = Constants.FLG_USER_USER_KBN_SMARTPHONE_USER;
					user.MailAddr = mailAddress;
					user.MailAddr2 = string.Empty;
					break;

				case "4":
					user.UserKbn = Constants.FLG_USER_USER_KBN_PC_USER;
					user.MailAddr = mailAddress;
					user.MailAddr2 = string.Empty;
					break;

				default:
					throw new ApplicationException("ユーザ区分が取得できません。(order_id:" + strOrderId + ")");
			}

			string[] name = strDatas[alHeaders.IndexOf("BillName")].Split(' ');
			string name1 = StringUtility.ToZenkaku((name.Length == 2) ? name[0] : string.Join("", name));
			string name2 = StringUtility.ToZenkaku((name.Length == 2) ? name[1] : "");
			user.Name = name1 + name2;
			user.Name1 = name1;
			user.Name2 = name2;
			string strBillZip = ProcessingZip(strOrderId, strDatas[alHeaders.IndexOf("BillZipCode")]);
			if (strBillZip == null)
			{
				throw new ApplicationException("郵便番号が正しくありません。(order_id:" + strOrderId + ")");
			}
			user.Zip = strBillZip;
			user.Zip1 = strBillZip.Substring(0, 3);
			user.Zip2 = strBillZip.Substring(4);
			user.Addr = StringUtility.ToZenkaku(strDatas[alHeaders.IndexOf("BillPrefecture")] + "　" 
											  + strDatas[alHeaders.IndexOf("BillCity")] + "　" 
											  + strDatas[alHeaders.IndexOf("BillAddress1")] + "　" 
											  + strDatas[alHeaders.IndexOf("BillAddress2")]);
			user.Addr1 = strDatas[alHeaders.IndexOf("BillPrefecture")];
			user.Addr2 = StringUtility.ToZenkaku(strDatas[alHeaders.IndexOf("BillCity")]);
			user.Addr3 = StringUtility.ToZenkaku(strDatas[alHeaders.IndexOf("BillAddress1")]);
			user.Addr4 = StringUtility.ToZenkaku(strDatas[alHeaders.IndexOf("BillAddress2")]);
			user.Tel1 = ProcessingTelFormated(strOrderId, strDatas[alHeaders.IndexOf("BillPhoneNumber")]);
			string[] strBillPhones = ProcessingTel(strOrderId, strDatas[alHeaders.IndexOf("BillPhoneNumber")]);
			if (strBillPhones == null)
			{
				throw new ApplicationException("電話番号が正しくありません。(order_id:" + strOrderId + ")");
			}
			user.Tel1_1 = strBillPhones[0];
			user.Tel1_2 = strBillPhones[1];
			user.Tel1_3 = strBillPhones[2];
			user.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
		}

		/// <summary>
		/// Validates the data.
		/// </summary>
		/// <param name="headers">The csv line headers.</param>
		/// <param name="datas">The csv line datas.</param>
		private void ValidateData(ArrayList headers, string[] datas)
		{
			if (Constants.STR_PREFECTURES_LIST.Contains(datas[headers.IndexOf("ShipPrefecture")]) == false
				|| Constants.STR_PREFECTURES_LIST.Contains(datas[headers.IndexOf("BillPrefecture")]) == false)
			{
				throw new Exception("都道府県が正しくありません。");
			}
		}
	}
}
