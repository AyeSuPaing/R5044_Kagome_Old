/*
=========================================================================================================
  Module      : メール注文取得／注文メール／ヤフー (YahooMail.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UpdateHistory;
using w2.Commerce.MallBatch.MailOrderGetter.Process.Base;
using w2.Domain.Order;

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Yahoo
{
	///**************************************************************************************
	/// <summary>
	/// ヤフー注文メール解析クラス
	/// </summary>
	///**************************************************************************************
	class YahooOrder : BaseProcess
	{
		/// <summary>日本国内の決済通貨コード</summary>
		private const string CURRENCY_CODE_JPY = "JPY";
		string m_strMailMessage = null;
		string m_strFileName = null;

		/// <summary>取り込み可能メール件名</summary>
		private static string[] m_successSubjects = { "【Yahoo!ショッピング】注文確認：" };
		/// <summary>取り込み不要メール件名</summary>
		private static string[] m_errorSubjects = { };

		#region 注文メール抽出用文言
		/// <summary>KEY：注文ID</summary>
		const string ORDER_ID = "注文ID　：";

		/// <summary>KEY：注文日時</summary>
		const string ORDER_DATE = "注文日時：";

		/// <summary>KEY：￣￣￣￣￣￣￣￣￣￣</summary>
		const string SEPARATOR = "￣￣￣￣￣￣￣￣￣￣\n";

		/// <summary>KEY：===============";</summary>
		const string SEPARATOR2 = "\n===============";

		/// <summary>KEY：商品の合計金額</summary>
		const string SUB_TOTAL = "商品の合計金額 ： ";

		/// <summary>KEY：手数料</summary>
		const string COMMISSION = "手数料：";

		/// <summary>KEY：送料</summary>
		const string SHIPPING_CHARGE = "送料：";

		/// <summary>KEY：ギフト包装料</summary>
		const string GIFT_WRAPPING = "ギフト包装料：";

		/// <summary>KEY：ポイント利用分</summary>
		const string USE_POINT = "ポイント利用分：";

		/// <summary>KEY：クーポン利用分</summary>
		const string USE_COUPON = "クーポン利用分：";

		/// <summary>KEY：合計金額</summary>
		const string TOTAL = "合計金額 ：";

		/// <summary>KEY：付与ポイント</summary>
		const string RELATION_MEMO = "付与ポイント：";

		/// <summary>KEY：商品情報</summary>
		const string PRODUCT_INFO = "商品情報";

		/// <summary>KEY：円</summary>
		const string YEN = "円";

		/// <summary>KEY：ポイント</summary>
		const string POINT = "ポイント";

		/// <summary>KEY：改行1</summary>
		const string NEW_LINE = "\n";

		#endregion

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public YahooOrder()
		{
			// プロパティ初期化
			this.OrderItems = new List<YahooOrderItem>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strMailMessage">メール文章</param>
		/// <param name="strFileName">対象のファイル名（エラー表示用）</param>
		/// <param name="shopId">店舗ID</param>
		/// <remarks>ヤフー注文メールの解析を実装する</remarks>
		public YahooOrder(string strMailMessage, string strFileName, string shopId)
			: this()
		{
			this.ShopId = shopId;
			InitYahooOrder(strMailMessage, strFileName);
		}

		/// <summary>
		/// 受信した注文メール本文を元にYahooOrderを構成する
		/// </summary>
		/// <param name="strMailMessage">メール文章</param>
		/// <param name="strFileName">対象のファイル名（エラー表示用）</param>
		private void InitYahooOrder(string strMailMessage, string strFileName)
		{
			//------------------------------------------------------
			// メール本文、ファイル名をメンバ変数にセット
			//------------------------------------------------------
			m_strFileName = strFileName;
			m_strMailMessage = strMailMessage.Replace("\r\n", "\n").Replace("\r", "\n");

			//------------------------------------------------------
			// メール解析 ※各項目の解析順序は、ヤフーメールの項目順序と同じとすること！！
			//------------------------------------------------------
			int iBgnIndex = 0;
			int iEndIndex = 0;

			// 注文ID設定
			if (m_strMailMessage.IndexOf(ORDER_ID, iBgnIndex) != -1)
			{
				iBgnIndex = m_strMailMessage.IndexOf(ORDER_ID, iBgnIndex) + ORDER_ID.Length;
				iEndIndex = m_strMailMessage.IndexOf(NEW_LINE, iBgnIndex) - iBgnIndex;
				setProperty(ORDER_ID, m_strMailMessage.Substring(iBgnIndex, iEndIndex));
			}
			else
			{
				throw new Exception(m_strFileName + ":注文IDを取得できません。");
			}
			// 注文日設定
			if (m_strMailMessage.IndexOf(ORDER_DATE, iBgnIndex) != -1)
			{
				iBgnIndex = m_strMailMessage.IndexOf(ORDER_DATE, iBgnIndex) + ORDER_DATE.Length;
				iEndIndex = m_strMailMessage.IndexOf(NEW_LINE, iBgnIndex) - iBgnIndex;
				setProperty(ORDER_DATE, m_strMailMessage.Substring(iBgnIndex, iEndIndex));
			}
			else
			{
				throw new Exception(m_strFileName + ":注文日を取得できません。");
			}
			// 商品情報設定
			if ((m_strMailMessage.IndexOf(SEPARATOR, iBgnIndex) != -1)
				&& (m_strMailMessage.IndexOf(SEPARATOR2, iBgnIndex) != -1))
			{
				iBgnIndex = m_strMailMessage.IndexOf(SEPARATOR, iBgnIndex) + SEPARATOR.Length;
				iEndIndex = m_strMailMessage.IndexOf(SEPARATOR2, iBgnIndex) - iBgnIndex;

				try
				{
					// 商品情報セット
					setProperty(PRODUCT_INFO, m_strMailMessage.Substring(iBgnIndex, iEndIndex));
				}
				catch
				{
					throw new Exception(m_strFileName + ":商品情報を取得できません。");
				}
			}
			else
			{
				throw new Exception(m_strFileName + ":商品情報を取得できません。");
			}
			// 小計設定（小計を取得できなかった場合、後の処理で小計に合計をセットする。）
			bool blSubTotalExistsFlg = true;
			if (m_strMailMessage.IndexOf(SUB_TOTAL, iBgnIndex) != -1)
			{
				iBgnIndex = m_strMailMessage.IndexOf(SUB_TOTAL, iBgnIndex) + SUB_TOTAL.Length;
				iEndIndex = m_strMailMessage.IndexOf(YEN, iBgnIndex) - iBgnIndex;
				setProperty(SUB_TOTAL, m_strMailMessage.Substring(iBgnIndex, iEndIndex));
			}
			else
			{
				// 「小計」に「合計」をセットする（明細金額が「合計」のみのパターンに対応）
				blSubTotalExistsFlg = false;
			}
			// 手数料設定
			if (m_strMailMessage.IndexOf(COMMISSION, iBgnIndex) != -1)
			{
				iBgnIndex = m_strMailMessage.IndexOf(COMMISSION, iBgnIndex) + COMMISSION.Length;
				iEndIndex = m_strMailMessage.IndexOf(YEN, iBgnIndex) - iBgnIndex;
				setProperty(COMMISSION, m_strMailMessage.Substring(iBgnIndex, iEndIndex));
			}
			// 送料設定
			if (m_strMailMessage.IndexOf(SHIPPING_CHARGE, iBgnIndex) != -1)
			{
				iBgnIndex = m_strMailMessage.IndexOf(SHIPPING_CHARGE, iBgnIndex) + SHIPPING_CHARGE.Length;
				iEndIndex = m_strMailMessage.IndexOf(YEN, iBgnIndex) - iBgnIndex;
				setProperty(SHIPPING_CHARGE, m_strMailMessage.Substring(iBgnIndex, iEndIndex));
			}
			// ギフト包装設定
			if (m_strMailMessage.IndexOf(GIFT_WRAPPING, iBgnIndex) != -1)
			{
				iBgnIndex = m_strMailMessage.IndexOf(GIFT_WRAPPING, iBgnIndex) + GIFT_WRAPPING.Length;
				iEndIndex = m_strMailMessage.IndexOf(YEN, iBgnIndex) - iBgnIndex;
				setProperty(GIFT_WRAPPING, m_strMailMessage.Substring(iBgnIndex, iEndIndex));
			}
			// 利用ポイント設定
			if (m_strMailMessage.IndexOf(USE_POINT, iBgnIndex) != -1)
			{
				iBgnIndex = m_strMailMessage.IndexOf(USE_POINT, iBgnIndex) + USE_POINT.Length;
				iEndIndex = m_strMailMessage.IndexOf(POINT, iBgnIndex) - iBgnIndex;
				setProperty(USE_POINT, m_strMailMessage.Substring(iBgnIndex, iEndIndex));
			}
			// クーポン利用
			if (m_strMailMessage.IndexOf(USE_COUPON, iBgnIndex) != -1)
			{
				iBgnIndex = m_strMailMessage.IndexOf(USE_COUPON, iBgnIndex) + USE_COUPON.Length;
				iEndIndex = m_strMailMessage.IndexOf(YEN, iBgnIndex) - iBgnIndex;
				setProperty(USE_COUPON, m_strMailMessage.Substring(iBgnIndex, iEndIndex));
			}

			// 合計設定
			if (m_strMailMessage.IndexOf(TOTAL, iBgnIndex) != -1)
			{
				iBgnIndex = m_strMailMessage.IndexOf(TOTAL, iBgnIndex) + TOTAL.Length;
				iEndIndex = m_strMailMessage.IndexOf(YEN, iBgnIndex) - iBgnIndex;
				setProperty(TOTAL, m_strMailMessage.Substring(iBgnIndex, iEndIndex));

				// 小計が存在しない場合は、小計に合計を入れる
				if (blSubTotalExistsFlg == false)
				{
					setProperty(SUB_TOTAL, this.Total.ToString());
				}
			}
			else
			{
				throw new Exception(m_strFileName + ":合計を取得できません。");
			}
			// メモ設定
			if (m_strMailMessage.IndexOf(RELATION_MEMO, iBgnIndex) != -1)
			{
				iBgnIndex = m_strMailMessage.IndexOf(RELATION_MEMO, iBgnIndex);
				iEndIndex = m_strMailMessage.IndexOf(NEW_LINE, iBgnIndex) - iBgnIndex;
				setProperty(RELATION_MEMO, m_strMailMessage.Substring(iBgnIndex, iEndIndex));
			}
			else
			{
				setProperty(RELATION_MEMO, "");
			}

			this.ShippingTaxRate = Constants.CONST_SHIPPING_TAXRATE;
			this.PaymentTaxRate = Constants.CONST_PAYMENT_TAXRATE;
			this.OrderPriceByTaxRate = GetOrderPriceByTaxRate(this.OrderItems);
		}

		/// <summary>
		/// プロパティをセットする
		/// </summary>
		/// <param name="strKey">キー</param>
		/// <param name="strValue">値</param>
		void setProperty(string strKey, string strValue)
		{
			//------------------------------------------------------
			// パラメータをプロパティにセット
			//------------------------------------------------------
			try
			{
				switch (strKey)
				{
					case ORDER_DATE:
						this.OrderDate = GetOrderDate(strValue);
						break;

					case ORDER_ID:
						var valueList = strValue.Split('-');
						var orderId = Constants.YAHOO_MALL_ORDERID_FORMAT
							? Constants.MALL_ID + "-" + valueList[valueList.Length - 1]
							: valueList[valueList.Length - 1];
						this.OrderId = orderId;
						break;

					case PRODUCT_INFO:
						this.Products = strValue;
						this.OrderItems = GetYahooOrderItems(strValue);
						break;

					case SUB_TOTAL:
						this.SubTotal = int.Parse(strValue.Trim().Replace(",", ""));
						break;

					case COMMISSION:
						this.Commission = int.Parse(strValue.Trim().Replace(",", ""));
						break;

					case SHIPPING_CHARGE:
						this.ShippingCharge = int.Parse(strValue.Trim().Replace(",", ""));
						break;

					case USE_POINT:
						// メール表記上はマイナス符号なしのため、マイナス変換する
						this.UsePoint = int.Parse(strValue.Trim().Replace(",", "")) * -1;
						break;

					case USE_COUPON:
						// メール表記上はマイナス符号ありのため、マイナス変換しない
						this.UseCoupon = int.Parse(strValue.Trim().Replace(",", ""));
						break;

					case GIFT_WRAPPING:
						this.GiftWrapping = int.Parse(strValue.Trim().Replace(",", ""));
						break;

					case TOTAL:
						this.Total = int.Parse(strValue.Trim().Replace(",", ""));
						break;

					case RELATION_MEMO:
						this.RelationMemo = strValue.Trim();
						break;

					default:
						break;
				}
			}
			catch (FormatException)
			{
				throw new FormatException(m_strFileName + "\n[" + strKey + "]パラメータの数値変換に失敗しました。");

			}
			catch (ArgumentNullException)
			{
				throw new ArgumentNullException(m_strFileName + "\n[" + strKey + "]パラメータの値にNullは設定できません。");
			}
		}

		/// <summary>
		/// 注文日をフォーマットにマッチングする
		/// </summary>
		/// <param name="strOrderDate">注文日</param>
		/// <returns>フォーマット変換済み注文日</returns>
		private string GetOrderDate(string strOrderDate)
		{
			return Regex.Match(strOrderDate, "([0-9]+年)+.*[0-9]+日").ToString().Replace("年", "/").Replace("月", "/").Replace("日", "") + " " + Regex.Match(strOrderDate, "([0-9]+時)+.*秒").ToString().Replace("時", ":").Replace("分", ":").Replace("秒", "");
		}

		/// <summary>
		/// Yahoo!受注商品取得
		/// </summary>
		/// <param name="strYahooOrderProduct">受注商品</param>
		/// <returns>受注商品一覧</returns>
		private List<YahooOrderItem> GetYahooOrderItems(string strYahooOrderProduct)
		{
			//------------------------------------------------------
			// 商品情報解析
			//------------------------------------------------------
			List<YahooOrderItem> lYahooItems = new List<YahooOrderItem>();
			int beginIndex = 0;

			// 1商品ごとに分割してリストに追加（次の商品の先頭を探してそこまでで区切る）
			for (int i = 2; strYahooOrderProduct.IndexOf(NEW_LINE + "（" + i.ToString() + "）") != -1; i++)
			{
				int nextIndex = strYahooOrderProduct.IndexOf(NEW_LINE + "（" + i.ToString() + "）") + NEW_LINE.Length;
				int length = nextIndex - beginIndex;
				lYahooItems.Add(new YahooOrderItem(strYahooOrderProduct.Substring(beginIndex, length), this.ShopId));
				beginIndex = nextIndex;
			}

			// 最後の商品を追加
			lYahooItems.Add(new YahooOrderItem(strYahooOrderProduct.Substring(beginIndex), this.ShopId));

			return lYahooItems;
		}

		/// <summary>
		/// オプション名を連結する
		/// </summary>
		/// <param name="lOptions">オプション一覧</param>
		/// <returns>オプション名連結文字列</returns>
		private string CreateOptionNames(List<string> lOptions)
		{
			StringBuilder sbOptionName = new StringBuilder();
			foreach (string strOption in lOptions)
			{
				if (sbOptionName.ToString() != "")
				{
					sbOptionName.Append("、");
				}
				sbOptionName.Append(strOption);
			}
			return sbOptionName.ToString();
		}

		/// <summary>
		/// 取込可能メールを判断する
		/// </summary>
		/// <param name="subject">メール件名</param>
		/// <param name="filePath">対象のファイルパス</param>
		/// <returns>判定結果</returns>
		/// <remarks>不要メールフォルダへの移動も行う</remarks>
		public static bool CheckMailImportPossible(string subject, string filePath)
		{
			return JudgmentGetMail(subject, m_errorSubjects, m_successSubjects, filePath);
		}

		/// <summary>
		/// 税率毎価格情報取得
		/// </summary>
		/// <param name="items">受注商品リスト</param>
		/// <returns>税率毎価格情報</returns>
		private List<OrderPriceByTaxRateModel> GetOrderPriceByTaxRate(List<YahooOrderItem> items)
		{
			var stackedDiscountAmount = 0m;
			var priceTotal = items.Sum(item => item.PricePreTax * item.Quantity);
			// 調整金額適用対象金額取得
			var paymentPrice = this.ShippingCharge;
			var shippingPrice = this.Commission;
			var totalRegulation = this.UsePoint + this.GiftWrapping + this.UseCoupon;
			priceTotal += paymentPrice;
			priceTotal += shippingPrice;
			if (priceTotal != 0)
			{
				items.ForEach(
					item =>
					{
						item.ItemPriceRegulation = (item.PricePreTax * item.Quantity) / priceTotal * totalRegulation;
						stackedDiscountAmount += item.ItemPriceRegulation;
					});
			}
			var shippingRegulationPrice = Math.Floor(shippingPrice / priceTotal * totalRegulation);
			stackedDiscountAmount += shippingRegulationPrice;

			var paymentRegulationPrice = Math.Floor(paymentPrice / priceTotal * totalRegulation);
			stackedDiscountAmount += paymentRegulationPrice;

			var fractionAmount = totalRegulation - stackedDiscountAmount;
			if (fractionAmount != 0)
			{
				var weightItem =
					items.FirstOrDefault(item => (item.PricePreTax * item.Quantity) > 0);
				if (weightItem != null)
				{
					weightItem.ItemPriceRegulation += fractionAmount;
				}
				else if (shippingPrice != 0)
				{
					shippingRegulationPrice += fractionAmount;
				}
				else
				{
					paymentRegulationPrice += fractionAmount;
				}
			}

			var priceInfo = new List<Hashtable>();
			// 税率毎の購入金額を算出する
			priceInfo.AddRange(items
				.Select(item => new Hashtable
				{
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , item.TaxRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , (item.PricePreTax * item.Quantity) + item.ItemPriceRegulation },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
				}).ToList());

			priceInfo.Add(new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , this.ShippingTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , shippingPrice + shippingRegulationPrice },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
			});
			priceInfo.Add(new Hashtable
			{ 
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , this.PaymentTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , paymentPrice + paymentRegulationPrice },
			});

			var groupedItem = priceInfo.GroupBy(item => new
			{
				taxRate = (decimal)item[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]
			});
				var priceByTaxRate = groupedItem.Select(
				item => new OrderPriceByTaxRateModel
				{
					KeyTaxRate = item.Key.taxRate,
					PriceSubtotalByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]),
					PriceShippingByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]),
					PricePaymentByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]),
					PriceTotalByRate = item.Sum(itemKey =>
						((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
					TaxPriceByRate = TaxCalculationUtility.GetTaxPrice(item.Sum(itemKey =>
							((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
						(decimal)item.Key.taxRate,
						Constants.TAX_ROUNDTYPE,
						true)
				}).ToList();
			foreach (var orderPriceByTaxRateModel in priceByTaxRate)
			{
				orderPriceByTaxRateModel.OrderId = this.OrderId;
			}
			return priceByTaxRate;
		}

		/// <summary>
		/// ヤフー注文をDBへ投入する
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strMallId">モールID</param>
		/// <param name="strMailFilePath">操作中のメールパス</param>
		/// <param name="strBaseMailDetail">注文メール詳細情報</param>
		public void InsertOrder(
			SqlAccessor sqlAccessor,
			string strMallId,
			string strMailFilePath,
			string strBaseMailDetail)
		{
			// ユーザIDを発番する
			string strUserId = GetNewUserId();


			//------------------------------------------------------
			// 注文情報
			//------------------------------------------------------
			{
				FileLogger.WriteInfo("[SQL START]w2_Order");
				using (SqlStatement sqlStatement = new SqlStatement("Order", "InsertOrder"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_ORDER_ORDER_ID, this.OrderId);
					htInput.Add(Constants.FIELD_ORDER_ORDER_GROUP_ID, "");
					htInput.Add(Constants.FIELD_ORDER_ORDER_NO, "");
					htInput.Add(Constants.FIELD_ORDER_USER_ID, strUserId);
					htInput.Add(Constants.FIELD_ORDER_SHOP_ID, this.ShopId);
					htInput.Add(Constants.FIELD_ORDER_ORDER_KBN, Constants.FLG_ORDER_ORDER_KBN_PC); // PC会員として取り込む（CSVファイル連携時、更新される）
					htInput.Add(Constants.FIELD_ORDER_ORDER_STATUS, Constants.FLG_ORDER_ORDER_STATUS_TEMP); // 仮注文
					htInput.Add(Constants.FIELD_ORDER_MALL_ID, strMallId);
					htInput.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, "K20"); // （仮値）代引き固定
					DateTime dtOrderDateTime;
					if (DateTime.TryParse(this.OrderDate.Trim(), out dtOrderDateTime))
					{
						htInput.Add(Constants.FIELD_ORDER_ORDER_DATE, dtOrderDateTime);
					}
					else
					{
						htInput.Add(Constants.FIELD_ORDER_ORDER_DATE, DateTime.Now);
						FileLogger.WriteError("[[日時] を正しく取得できませんでした。現在の時刻を設定します。" + this.OrderDate.Trim());
					}

					// 注文商品数を計算する
					int iRecordCount = 0;
					decimal dItemCount = 0;
					if (this.OrderItems != null)
					{
						foreach (YahooOrderItem oiOrderItem in this.OrderItems)
						{
							dItemCount += oiOrderItem.Quantity;
							iRecordCount++;
						}
					}
					else
					{
						throw new MailParsingException("注文商品が取込できませんでした。");
					}
					var itemSubtotalPriceTax = this.OrderItems.Sum(item => item.TaxSubTotal);
					htInput.Add(Constants.FIELD_ORDER_ORDER_ITEM_COUNT, iRecordCount);
					htInput.Add(Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT, dItemCount);
					htInput.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL, this.OrderItems.Sum(item => item.SubTotal));
					htInput.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX,itemSubtotalPriceTax);
					htInput.Add(Constants.FIELD_ORDER_ORDER_PRICE_TAX, this.OrderPriceByTaxRate.Sum(priceByTaxRate => priceByTaxRate.TaxPriceByRate));
					htInput.Add(Constants.FIELD_ORDER_ORDER_PRICE_PACK, new decimal(0));
					htInput.Add(Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING, this.ShippingCharge);
					htInput.Add(Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE, this.Commission);
					// ポイント利用分 + ギフト包装料 + クーポン利用分
					htInput.Add(Constants.FIELD_ORDER_ORDER_PRICE_REGULATION, this.UsePoint + this.GiftWrapping + this.UseCoupon);
					htInput.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, this.Total);
					htInput.Add(Constants.FIELD_ORDER_CARD_KBN, "");
					htInput.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, "");
					htInput.Add(Constants.FIELD_ORDER_SHIPPING_ID, Constants.MALL_DEFAULT_SHIPPING_ID);
					htInput.Add(Constants.FIELD_ORDER_CARD_INSTRUMENTS, "");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDER_CAREER_ID, "");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDER_MEMO, "");
					htInput.Add(Constants.FIELD_ORDER_WRAPPING_MEMO, "");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDER_PAYMENT_MEMO, "");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDER_LAST_BILLED_AMOUNT, this.Total);
					htInput.Add(Constants.FIELD_ORDER_SETTLEMENT_AMOUNT, this.Total);
					htInput.Add(Constants.FIELD_ORDER_SETTLEMENT_CURRENCY, CURRENCY_CODE_JPY); // Global未対応のためJPY固定

					// [ポイント利用方法] 全てヘッダ含めて全ての文言を入力
					htInput.Add(Constants.FIELD_ORDER_RELATION_MEMO, this.RelationMemo);

					// 調整金額メモ（ポイント利用分 + ギフト包装料 + クーポン利用分）
					StringBuilder sbRegulationMemo = new StringBuilder();
					if (this.GiftWrapping != 0)
					{
						sbRegulationMemo.Append("－－ギフト手数料－－\r\n");
						sbRegulationMemo.Append(StringUtility.ToPrice(this.GiftWrapping));
						sbRegulationMemo.Append("円");
					}
					if (this.UsePoint != 0)
					{
						if (sbRegulationMemo.ToString() != "")
						{
							sbRegulationMemo.Append("\r\n");
						}
						sbRegulationMemo.Append("－－ポイント利用分－－\r\n");
						sbRegulationMemo.Append(StringUtility.ToPrice(this.UsePoint));
						sbRegulationMemo.Append("円");
					}
					if (this.UseCoupon != 0)
					{
						if (sbRegulationMemo.ToString() != "")
						{
							sbRegulationMemo.Append("\r\n");
						}
						sbRegulationMemo.Append("－－クーポン利用分－－\r\n");
						sbRegulationMemo.Append(StringUtility.ToPrice(this.UseCoupon));
						sbRegulationMemo.Append("円");
					}
					htInput.Add(Constants.FIELD_ORDER_REGULATION_MEMO, sbRegulationMemo.ToString());

					// 税関係
					htInput.Add(Constants.FIELD_ORDER_ORDER_TAX_INCLUDED_FLG, Constants.FLG_PRODUCT_TAX_INCLUDED_PRETAX);
					htInput.Add(Constants.FIELD_ORDER_ORDER_TAX_RATE, "0");
					htInput.Add(Constants.FIELD_ORDER_ORDER_TAX_ROUND_TYPE, Constants.TAX_ROUNDTYPE);
					htInput.Add(Constants.FIELD_ORDER_SHIPPING_TAX_RATE, this.ShippingTaxRate);
					htInput.Add(Constants.FIELD_ORDER_PAYMENT_TAX_RATE, this.PaymentTaxRate);

					// リアルタイム累計購入回数取得
					var user = new UserService().Get(strUserId);
					var orderCount = ((user == null) ? 0 : user.OrderCountOrderRealtime);
					htInput.Add(Constants.FIELD_ORDER_ORDER_COUNT_ORDER, orderCount + 1);

					htInput.Add(Constants.FIELD_ORDER_ORDER_CANCEL_DATE, null);

					int iUpdated = sqlStatement.ExecStatement(sqlAccessor, htInput);
					if (iUpdated <= 0)
					{
						throw new ApplicationException("In w2_Order, Insert error.");
					}
					FileLogger.WriteInfo("[SQL END]w2_Order");
				}
			}

			//------------------------------------------------------
			// 注文者情報
			//------------------------------------------------------
			{
				FileLogger.WriteInfo("[SQL START]w2_OrderOwner");
				using (SqlStatement sqlStatement = new SqlStatement("Order", "InsertOrderOwner"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_ORDEROWNER_ORDER_ID, this.OrderId);
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN, Constants.FLG_ORDEROWNER_OWNER_KBN_PC_USER); // PC会員として取り込む（CSVファイル連携時、更新される）
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME, "Yahoo仮登録");
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME1, "Yahoo仮登録");
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME2, "");
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA, "かりせいめい");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1, "かりせいめい");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2, "");
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR, "");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2, String.Empty);	// Owner Mail Addr 2
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_ZIP, "000-0000");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR1, "東京都");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR2, "仮市区町村番地");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR3, "");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR4, "");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL1, "00-0000-0000");// Yahoo規定値

					int iUpdated = sqlStatement.ExecStatement(sqlAccessor, htInput);
					if (iUpdated <= 0)
					{
						throw new ApplicationException("In w2_OrderOwner, Insert error.");
					}
				}
				FileLogger.WriteInfo("[SQL END]w2_OrderOwner");
			}

			//------------------------------------------------------
			// 配送先情報
			//------------------------------------------------------
			{
				FileLogger.WriteInfo("[SQL START]w2_OrderShipping");
				using (SqlStatement sqlStatement = new SqlStatement("Order", "InsertOrderShipping"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_ORDERSHIPPING_ORDER_ID, this.OrderId);
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME, "仮の配送先氏名");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1, "仮の配送先氏名");// Yahoo規定値
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2, "");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA, "");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1, "");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2, "");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP, "000-0000");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1, "東京都");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2, "仮市区町村番地");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3, "");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4, "");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1, "00-0000-0000");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, DBNull.Value);
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME, "");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG, Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID);
					htInput.Add(Constants.FIELD_ORDERSHIPPING_EXTERNAL_SHIPPING_COOPERATION_ID, "");
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE, null);

					// 配送会社は指定された配送種別のデフォルト配送会社を設定
					var defaultShippingCompany = new ShopShippingService().GetDefaultCompany(
						Constants.MALL_DEFAULT_SHIPPING_ID,
						Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);
					htInput.Add(Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID, defaultShippingCompany.DeliveryCompanyId);
					htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);

					int iUpdated = sqlStatement.ExecStatement(sqlAccessor, htInput);
					if (iUpdated <= 0)
					{
						throw new ApplicationException("In w2_OrderShipping, Insert error.");
					}
				}
				FileLogger.WriteInfo("[SQL END]w2_OrderShipping");
			}
			//------------------------------------------------------
			// 税率毎価格情報登録
			//------------------------------------------------------
			var orderPriceByTaxRateService = new OrderPriceByTaxRateService();
			FileLogger.WriteInfo("[SQL START]w2_OrderShipping");
			foreach (var orderPriceByTaxRateModel in this.OrderPriceByTaxRate)
			{
				orderPriceByTaxRateModel.DateCreated = DateTime.Now;
				orderPriceByTaxRateModel.DateChanged = DateTime.Now;
				orderPriceByTaxRateService.Insert(orderPriceByTaxRateModel, sqlAccessor);
			}
			FileLogger.WriteInfo("[SQL END]w2_OrderShipping");

			//------------------------------------------------------
			// 注文商品情報
			//------------------------------------------------------
			List<YahooOrderItem> lYahooOrderItems = this.OrderItems;
			StringBuilder sbErrorMessage = new StringBuilder();
			int iItemCount = 1;
			bool blStockError = false;
			if (lYahooOrderItems != null)
			{
				foreach (YahooOrderItem yoiYahooOrderItem in lYahooOrderItems)
				{
					FileLogger.WriteInfo("[SQL START]w2_OrderItems");
					sbErrorMessage.Remove(0, sbErrorMessage.Length);
					string strProductId = "";

					// 商品マスタ（w2_product）とマッチング
					strProductId = yoiYahooOrderItem.ProductId;
					if (strProductId == "")
					{
						blStockError = true;
					}

					//------------------------------------------------------
					// 注文商品登録
					//------------------------------------------------------
					using (SqlStatement sqlStatement = new SqlStatement("Order", "InsertOrderItem"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_ORDERITEM_SHOP_ID, this.ShopId);
						htInput.Add(Constants.FIELD_ORDERITEM_ORDER_ID, this.OrderId);
						htInput.Add(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO, iItemCount);
						htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, strProductId);
						htInput.Add(Constants.FIELD_ORDERITEM_VARIATION_ID, yoiYahooOrderItem.VariationId);
						htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_NAME, yoiYahooOrderItem.ProductName);
						htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE, yoiYahooOrderItem.Price);
						htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_POINT, new decimal(0));
						htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG, TaxCalculationUtility.GetPrescribedProductTaxIncludedFlag());
						htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, yoiYahooOrderItem.TaxRate);
						htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE, Constants.TAX_ROUNDTYPE);
						// 税込み価格
						htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX, yoiYahooOrderItem.PricePreTax);
						htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_SHIP, decimal.Zero);
						htInput.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY, yoiYahooOrderItem.Quantity);
						htInput.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE, yoiYahooOrderItem.Quantity);
						htInput.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE, yoiYahooOrderItem.SubTotal);
						htInput.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX, yoiYahooOrderItem.TaxSubTotal);
						htInput.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE, yoiYahooOrderItem.SubTotal);

						int iUpdated = sqlStatement.ExecStatement(sqlAccessor, htInput);
						if (iUpdated <= 0)
						{
							throw new ApplicationException("In w2_OrderItem, Insert error.");
						}
					}
					iItemCount++;
					FileLogger.WriteInfo("[SQL END]w2_OrderItems");

					//------------------------------------------------------
					// 在庫更新
					//------------------------------------------------------
					FileLogger.WriteInfo("[SQL START]w2_ProductStock");
					DataView dvProductSrockManagementKbn = null;
					using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProductSrockManagementKbn"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.ShopId);
						htInput.Add(Constants.FIELD_PRODUCT_PRODUCT_ID, strProductId);

						dvProductSrockManagementKbn = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
					}

					// 商品マスタに商品が存在しない場合、在庫更新エラーとする
					if (dvProductSrockManagementKbn.Count == 0)
					{
						FileLogger.WriteError("商品バリエーションID" + yoiYahooOrderItem.VariationId + "の商品情報を取得できませんでした。");
						blStockError = true;
						sbErrorMessage.Append("注文ID[").Append(this.OrderId).Append("] 商品バリエーションID[").Append(StringUtility.ToEmpty(yoiYahooOrderItem.VariationId)).Append("]の商品情報をご確認ください。");

						// モール監視ログ登録（在庫更新エラー）
						Program.MallWatchingLogManager.Insert(sqlAccessor, Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER, strMallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING, "在庫更新エラー：商品バリエーションID[" + StringUtility.ToEmpty(yoiYahooOrderItem.VariationId) + "]の商品を取得できませんでした。商品をご登録ください。\r\n注文メール(詳細1)は受注情報としてシステムに取り込まれまれています。\r\n注文ID[" + this.OrderId + "]", strBaseMailDetail);
					}
					// 該当商品の在庫管理方法が「在庫管理しない」以外の場合、在庫更新を行う（Not Transaction）
					else if ((string)dvProductSrockManagementKbn[0][Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
					{
						using (SqlStatement sqlStatement = new SqlStatement("Product", "UpdateProductStock"))
						using (SqlStatement sqlStatement2 = new SqlStatement("Product", "InsertProductStockHistory"))
						{
							//------------------------------------------------------
							// 商品在庫マスタ更新
							//------------------------------------------------------
							Hashtable htInput = new Hashtable();
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, this.ShopId);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, strProductId);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, yoiYahooOrderItem.VariationId);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, yoiYahooOrderItem.Quantity);

							int iUpdated = sqlStatement.ExecStatement(sqlAccessor, htInput);
							if (iUpdated <= 0)
							{
								FileLogger.WriteError("商品バリエーションID" + yoiYahooOrderItem.VariationId + "の在庫を更新できませんでした。商品番号が誤っているか、在庫数が取得できなかった可能性があります。");
								blStockError = true;
							}

							//------------------------------------------------------
							// 商品在庫履歴マスタ更新
							//------------------------------------------------------
							Hashtable htInput2 = new Hashtable();
							htInput2.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, this.ShopId);
							htInput2.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, this.OrderId);
							htInput2.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, strProductId);
							htInput2.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, yoiYahooOrderItem.VariationId);
							htInput2.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, yoiYahooOrderItem.Quantity * -1);

							int iUpdate2 = sqlStatement2.ExecStatement(sqlAccessor, htInput2);
							if (iUpdate2 <= 0)
							{
								FileLogger.WriteError("商品バリエーションID" + yoiYahooOrderItem.VariationId + "の在庫履歴作成に失敗しました。商品番号が誤っているか、在庫数が取得できなかった可能性があります。");
								blStockError = true;
							}

							//------------------------------------------------------
							// エラーメール文言
							//------------------------------------------------------
							if (blStockError)
							{
								sbErrorMessage.Append("注文ID[").Append(this.OrderId).Append("] 商品バリエーションID[").Append(StringUtility.ToEmpty(yoiYahooOrderItem.VariationId)).Append("]の在庫情報をご確認ください。");

								// モール監視ログ登録（在庫更新エラー）
								Program.MallWatchingLogManager.Insert(sqlAccessor, Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER, strMallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING, "在庫更新エラー：商品バリエーションID[" + StringUtility.ToEmpty(yoiYahooOrderItem.VariationId) + "]が間違っているか、在庫数が取得出来なかった可能性があります。在庫情報をご確認ください。\r\n注文メール(詳細1)は受注情報としてシステムに取り込まれまれています。\r\n注文ID[" + this.OrderId + "]", strBaseMailDetail);
							}
						}
					}
					FileLogger.WriteInfo("[SQL END]w2_ProductStock");
				}

				//------------------------------------------------------
				// ユーザ登録
				//------------------------------------------------------
				FileLogger.WriteInfo("[SQL START]Yahoo仮ユーザ登録");
				var user = new UserModel()
				{
					UserId = strUserId,
					UserKbn = Constants.FLG_USER_USER_KBN_PC_USER, // PC会員として取り込む
					MallId = strMallId,
					Name = "Yahoo仮登録",
					Name1 = "Yahoo仮登録",
					Name2 = "",
					NameKana = "かりせいめい",
					NameKana1 = "かりせいめい",
					NameKana2 = "",
					MailAddr = "",
					MailAddr2 = "",
					Zip = "000-0000",
					Zip1 = "000",
					Zip2 = "0000",
					Addr1 = "東京都",
					Addr2 = "仮市区町村番地",
					Addr3 = "",
					Addr4 = "",
					Addr = "東京都仮市区町村番地",
					Tel1 = "00-0000-0000",
					Tel1_1 = "00",
					Tel1_2 = "0000",
					Tel1_3 = "0000",
					Sex = Constants.FLG_USER_SEX_UNKNOWN,
					Birth = null,
					BirthYear = "",
					BirthMonth = "",
					BirthDay = "",
					LoginId = "",
					Password = "",
					MailFlg = Constants.FLG_USER_MAILFLG_UNKNOWN,
					LastChanged = Constants.BATCH_USER,
					MemberRankId = "",
					RecommendUid = "",
					RemoteAddr = "",
					CompanyName = "",
					CompanyPostName = "",
					NickName = "",
				};

				bool insertResult = new UserService().InsertWithUserExtend(
					user,
					Constants.FLG_LASTCHANGED_BATCH,
					UpdateHistoryAction.DoNotInsert,
					sqlAccessor);
				if (insertResult == false)
				{
					throw new ApplicationException("In w2_User, Insert error.");
				}
				FileLogger.WriteInfo("[SQL END]Yahoo仮ユーザ登録");

				// リアルタイム累計購入回数更新処理
				var order = new Hashtable
				{
					{Constants.FIELD_ORDER_USER_ID, strUserId},
					{Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME,  new UserService().Get(strUserId, sqlAccessor).OrderCountOrderRealtime},
				};
				OrderCommon.UpdateRealTimeOrderCount(order, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_ORDER, sqlAccessor);

				// 更新履歴登録
				new UpdateHistoryService().InsertForOrder(this.OrderId, Constants.FLG_LASTCHANGED_BATCH, sqlAccessor);
				new UpdateHistoryService().InsertForUser(user.UserId, Constants.FLG_LASTCHANGED_BATCH, sqlAccessor);

				//------------------------------------------------------
				// 在庫エラー処理
				//------------------------------------------------------
				if (blStockError)
				{
					// 在庫エラーフォルダへ移動、エラーメール送信
					File.Move(strMailFilePath, Constants.PATH_STOCK_ERROR + @"\" + Path.GetFileName(strMailFilePath));
					ErrorMailSender("在庫の更新時にエラーが発生しました。\r\n" + sbErrorMessage.ToString() + "\r\nファイル名：" + Constants.PATH_STOCK_ERROR + @"\" + Path.GetFileName(strMailFilePath));

					return;
				}

				//------------------------------------------------------
				// 成功フォルダへ移動
				//------------------------------------------------------
				File.Move(strMailFilePath, Constants.PATH_SUCCESS + @"\" + Path.GetFileName(strMailFilePath));

				// モール監視ログ登録（注文メール取込成功）
				Program.MallWatchingLogManager.Insert(sqlAccessor, Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER, strMallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, "注文メールの取込を1件完了しました。\r\n取込済みの注文メールは詳細1にてご確認頂けます。\r\n注文ID[" + this.OrderId + "]", strBaseMailDetail);
			}
			else
			{
				throw new MailParsingException("[商品]が見つかりませんでした。");
			}
		}

		/// <summary>注文日</summary>
		public string OrderDate { get; private set; }
		/// <summary>注文ID</summary>
		public string OrderId { get; private set; }
		/// <summary>商品</summary>
		public string Products { get; private set; }
		/// <summary>注文明細</summary>
		public List<YahooOrderItem> OrderItems { get; private set; }
		/// <summary>税率毎価格情報</summary>
		public List<OrderPriceByTaxRateModel> OrderPriceByTaxRate { get; private set; }
		/// <summary>小計</summary>
		public decimal SubTotal { get; private set; }
		/// <summary>手数料</summary>
		public decimal Commission { get; private set; }
		/// <summary>送料</summary>
		public decimal ShippingCharge { get; private set; }
		/// <summary>ポイント利用分</summary>
		public decimal UsePoint { get; private set; }
		/// <summary>クーポン利用分</summary>
		public decimal UseCoupon { get; private set; }
		/// <summary>ギフト包装</summary>
		public decimal GiftWrapping { get; private set; }
		/// <summary>合計</summary>
		public decimal Total { get; private set; }
		/// <summary>外部連携メモ</summary>
		public string RelationMemo { get; private set; }
		/// <summary>注文者氏名（姓）</summary>
		public string OwnerName1 { get; protected set; }
		/// <summary>注文者氏名（名）</summary>
		public string OwnerName2 { get; protected set; }
		/// <summary>注文者メールアドレス</summary>
		public string To { get; private set; }
		/// <summary>ショップID</summary>
		public string ShopId { get; private set; }
		/// <summary>配送料税率</summary>
		public decimal ShippingTaxRate { get; private set; }
		/// <summary>決済手数料税率</summary>
		public decimal PaymentTaxRate { get; private set; }
	}
}
