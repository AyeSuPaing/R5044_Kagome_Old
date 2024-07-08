/*
=========================================================================================================
  Module      : 注文配送先情報入力クラス (OrderShippingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Linq;
using w2.Common.Util;
using w2.App.Common.Global;
using w2.App.Common.Input.Order.Helper;
using w2.Domain.Order;
using static w2.Common.Util.Validator;
using w2.Domain;
using w2.App.Common.Order;
using w2.Domain.RealShop;

namespace w2.App.Common.Input.Order
{
	/// <summary>
	/// 注文配送先情報入力クラス（登録、編集で利用）
	/// </summary>
	[Serializable]
	public class OrderShippingInput : InputBase<OrderShippingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderShippingInput()
		{
			this.OrderId = string.Empty;
			this.OrderShippingNo = "0";
			this.ShippingName = string.Empty;
			this.ShippingName1 = string.Empty;
			this.ShippingName2 = string.Empty;
			this.ShippingNameKana = string.Empty;
			this.ShippingNameKana1 = string.Empty;
			this.ShippingNameKana2 = string.Empty;
			this.ShippingAddr1 = string.Empty;
			this.ShippingAddr2 = string.Empty;
			this.ShippingAddr3 = string.Empty;
			this.ShippingAddr4 = string.Empty;
			this.ShippingTel1 = string.Empty;
			this.ShippingTel1_1 = string.Empty;
			this.ShippingTel1_2 = string.Empty;
			this.ShippingTel1_3 = string.Empty;
			this.ShippingTel2 = string.Empty;
			this.ShippingTel3 = string.Empty;
			this.ShippingFax = string.Empty;
			this.ShippingCompany = "0";
			this.ShippingDate = null;
			this.ShippingTime = string.Empty;
			this.ShippingCheckNo = string.Empty;
			this.DelFlg = "0";
			this.DateCreated = DateTime.Now.ToString();
			this.DateChanged = DateTime.Now.ToString();
			this.SenderName = string.Empty;
			this.SenderName1 = string.Empty;
			this.SenderName2 = string.Empty;
			this.SenderNameKana = string.Empty;
			this.SenderNameKana1 = string.Empty;
			this.SenderNameKana2 = string.Empty;
			this.SenderZip = string.Empty;
			this.SenderZip1 = string.Empty;
			this.SenderZip2 = string.Empty;
			this.SenderAddr1 = string.Empty;
			this.SenderAddr2 = string.Empty;
			this.SenderAddr3 = string.Empty;
			this.SenderAddr4 = string.Empty;
			this.SenderTel1 = string.Empty;
			this.SenderTel1_1 = string.Empty;
			this.SenderTel1_2 = string.Empty;
			this.SenderTel1_3 = string.Empty;
			this.WrappingPaperType = string.Empty;
			this.WrappingPaperName = string.Empty;
			this.WrappingBagType = string.Empty;
			this.ShippingCompanyName = string.Empty;
			this.ShippingCompanyPostName = string.Empty;
			this.SenderCompanyName = string.Empty;
			this.SenderCompanyPostName = string.Empty;
			this.AnotherShippingFlg = Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID;
			this.ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
			this.DeliveryCompanyId = string.Empty;
			this.Items = new OrderItemInput[0];
			this.ShippingCountryIsoCode = string.Empty;
			this.ShippingCountryName = string.Empty;
			this.ShippingAddr5 = string.Empty;
			this.SenderCountryIsoCode = string.Empty;
			this.SenderCountryName = string.Empty;
			this.SenderAddr5 = string.Empty;
			this.ScheduledShippingDate = null;
			this.ExternalShipmentEntry = false;
			this.OldShippingCheckNo = string.Empty;
			this.ShippingReceivingStoreFlg
				= Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
			this.ShippingReceivingStoreId = string.Empty;
			this.ShippingExternalDelivertyStatus = string.Empty;
			this.ShippingStatus = string.Empty;
			this.ShippingStatusUpdateDate = null;
			this.ShippingReceivingMailDate = null;
			this.ShippingReceivingStoreType = string.Empty;
			this.Items = new OrderItemInput[0];
			this.ShippingStatusCode = string.Empty;
			this.ShippingOfficeName = string.Empty;
			this.ShippingHandyTime = string.Empty;
			this.ShippingCurrentStatus = string.Empty;
			this.ShippingStatusDetail = string.Empty;
			this.StorePickupRealShopId = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public OrderShippingInput(OrderShippingModel model)
			: this()
		{
			this.OrderId = model.OrderId;
			this.OrderShippingNo = model.OrderShippingNo.ToString();
			this.ShippingName = model.ShippingName;
			this.ShippingNameKana = model.ShippingNameKana;
			this.ShippingZip = model.ShippingZip;
			var shippingZip = this.ShippingZip.Split('-');
			this.ShippingZip1 = (shippingZip.Length > 0) ? shippingZip[0] : string.Empty;
			this.ShippingZip2 = (shippingZip.Length > 1) ? shippingZip[1] : string.Empty;
			this.ShippingAddr1 = model.ShippingAddr1;
			this.ShippingAddr2 = model.ShippingAddr2;
			this.ShippingAddr3 = model.ShippingAddr3;
			this.ShippingAddr4 = model.ShippingAddr4;
			this.ShippingTel1 = model.ShippingTel1;
			var shippingTel1 = this.ShippingTel1.Split('-');
			this.ShippingTel1_1 = (shippingTel1.Length > 0) ? shippingTel1[0] : string.Empty;
			this.ShippingTel1_2 = (shippingTel1.Length > 1) ? shippingTel1[1] : string.Empty;
			this.ShippingTel1_3 = (shippingTel1.Length > 2) ? shippingTel1[2] : string.Empty;
			this.ShippingTel2 = model.ShippingTel2;
			this.ShippingTel3 = model.ShippingTel3;
			this.ShippingFax = model.ShippingFax;
			this.ShippingCompany = model.ShippingCompany;
			this.ShippingDate = (model.ShippingDate != null) ? model.ShippingDate.ToString() : null;
			this.ShippingTime = model.ShippingTime;
			this.ShippingCheckNo = model.ShippingCheckNo;
			this.DelFlg = model.DelFlg;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.ShippingName1 = model.ShippingName1;
			this.ShippingName2 = model.ShippingName2;
			this.ShippingNameKana1 = model.ShippingNameKana1;
			this.ShippingNameKana2 = model.ShippingNameKana2;
			this.SenderName = model.SenderName;
			this.SenderName1 = model.SenderName1;
			this.SenderName2 = model.SenderName2;
			this.SenderNameKana = model.SenderNameKana;
			this.SenderNameKana1 = model.SenderNameKana1;
			this.SenderNameKana2 = model.SenderNameKana2;
			this.SenderZip = model.SenderZip;
			var senderZip = this.SenderZip.Split('-');
			this.SenderZip1 = (senderZip.Length > 0) ? senderZip[0] : string.Empty;
			this.SenderZip2 = (senderZip.Length > 1) ? senderZip[1] : string.Empty;
			this.SenderAddr1 = model.SenderAddr1;
			this.SenderAddr2 = model.SenderAddr2;
			this.SenderAddr3 = model.SenderAddr3;
			this.SenderAddr4 = model.SenderAddr4;
			this.SenderTel1 = model.SenderTel1;
			var senderTel1 = this.SenderTel1.Split('-');
			this.SenderTel1_1 = (senderTel1.Length > 0) ? senderTel1[0] : string.Empty;
			this.SenderTel1_2 = (senderTel1.Length > 1) ? senderTel1[1] : string.Empty;
			this.SenderTel1_3 = (senderTel1.Length > 2) ? senderTel1[2] : string.Empty;
			this.WrappingPaperType = model.WrappingPaperType;
			this.WrappingPaperName = model.WrappingPaperName;
			this.WrappingBagType = model.WrappingBagType;
			this.ShippingCompanyName = model.ShippingCompanyName;
			this.ShippingCompanyPostName = model.ShippingCompanyPostName;
			this.SenderCompanyName = model.SenderCompanyName;
			this.SenderCompanyPostName = model.SenderCompanyPostName;
			this.AnotherShippingFlg = model.AnotherShippingFlg;
			this.ShippingMethod = model.ShippingMethod;
			this.DeliveryCompanyId = model.DeliveryCompanyId;
			this.ShippingCountryIsoCode = model.ShippingCountryIsoCode;
			this.ShippingCountryName = model.ShippingCountryName;
			this.ShippingAddr5 = model.ShippingAddr5;
			this.SenderCountryIsoCode = model.SenderCountryIsoCode;
			this.SenderCountryName = model.SenderCountryName;
			this.SenderAddr5 = model.SenderAddr5;
			this.ScheduledShippingDate = (model.ScheduledShippingDate != null) ? model.ScheduledShippingDate.ToString() : null;
			this.ShippingReceivingStoreFlg = model.ShippingReceivingStoreFlg;
			this.ShippingReceivingStoreId = model.ShippingReceivingStoreId;
			this.ShippingExternalDelivertyStatus = model.ShippingExternalDelivertyStatus;
			this.ShippingStatus = model.ShippingStatus;
			this.ShippingStatusUpdateDate
				= ((model.ShippingStatusUpdateDate != null)
					? model.ShippingStatusUpdateDate.ToString()
					: null);
			this.ShippingReceivingMailDate
				= ((model.ShippingReceivingMailDate != null)
					? model.ShippingReceivingMailDate.ToString()
					: null);
			this.ShippingReceivingStoreType = model.ShippingReceivingStoreType;
			this.ShippingStatusCode = model.ShippingStatusCode;
			this.ShippingOfficeName = model.ShippingOfficeName;
			this.ShippingHandyTime = model.ShippingHandyTime;
			this.ShippingCurrentStatus = model.ShippingCurrentStatus;
			this.ShippingStatusDetail = model.ShippingStatusDetail;
			this.StorePickupRealShopId = model.StorePickupRealShopId;

			// 商品リスト
			var orderItems = model.Items.Select((s, index) => new OrderItemInput(s, index));
			this.Items = OrderInputHelper.SetCanDeletePropertyToOrderItemInput(orderItems);
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override OrderShippingModel CreateModel()
		{
			var model = new OrderShippingModel
			{
				OrderId = this.OrderId,
				OrderShippingNo = int.Parse(this.OrderShippingNo),
				ShippingName = ((string.IsNullOrEmpty(this.ShippingName1 + this.ShippingName2) == false)
					? this.ShippingName1 + this.ShippingName2
					: this.ShippingName),
				ShippingNameKana = this.ShippingNameKana1 + this.ShippingNameKana2,
				ShippingCountryIsoCode = this.ShippingCountryIsoCode,
				ShippingCountryName = this.ShippingCountryName,
				ShippingZip = (string.IsNullOrEmpty(this.ShippingZip1 + this.ShippingZip2) == false)
					? string.Join("-", this.ShippingZip1, this.ShippingZip2)
					: this.ShippingZip,
				ShippingAddr1 = this.ShippingAddr1,
				ShippingAddr2 = this.ShippingAddr2,
				ShippingAddr3 = this.ShippingAddr3,
				ShippingAddr4 = this.ShippingAddr4,
				ShippingAddr5 = this.ShippingAddr5,
				ShippingTel1 = (string.IsNullOrEmpty(this.ShippingTel1_1 + this.ShippingTel1_2 + ShippingTel1_3) == false)
					? string.Join("-", this.ShippingTel1_1, this.ShippingTel1_2, this.ShippingTel1_3)
					: this.ShippingTel1,
				ShippingTel2 = this.ShippingTel2,
				ShippingTel3 = this.ShippingTel3,
				ShippingFax = this.ShippingFax,
				ShippingCompany = this.ShippingCompany,
				ShippingDate = (this.ShippingDate != null) ? DateTime.Parse(this.ShippingDate) : (DateTime?)null,
				ShippingTime = this.ShippingTime,
				ShippingCheckNo = this.ShippingCheckNo,
				DelFlg = this.DelFlg,
				DateCreated = DateTime.Parse(this.DateCreated),
				DateChanged = DateTime.Parse(this.DateChanged),
				ShippingName1 = this.ShippingName1,
				ShippingName2 = this.ShippingName2,
				ShippingNameKana1 = this.ShippingNameKana1,
				ShippingNameKana2 = this.ShippingNameKana2,
				SenderName = this.SenderName1 + this.SenderName2,
				SenderName1 = this.SenderName1,
				SenderName2 = this.SenderName2,
				SenderNameKana = this.SenderNameKana1 + this.SenderNameKana2,
				SenderNameKana1 = this.SenderNameKana1,
				SenderNameKana2 = this.SenderNameKana2,
				SenderCountryIsoCode = this.SenderCountryIsoCode,
				SenderCountryName = this.SenderCountryName,
				SenderZip = (string.IsNullOrEmpty(this.SenderZip1 + this.SenderZip2) == false)
				? string.Join("-", this.SenderZip1, this.SenderZip2)
				: this.SenderZip,
				SenderAddr1 = this.SenderAddr1,
				SenderAddr2 = this.SenderAddr2,
				SenderAddr3 = this.SenderAddr3,
				SenderAddr4 = this.SenderAddr4,
				SenderAddr5 = this.SenderAddr5,
				SenderTel1 = (string.IsNullOrEmpty(this.SenderTel1_1 + this.SenderTel1_2 + this.SenderTel1_3) == false)
				? string.Join("-", this.SenderTel1_1, this.SenderTel1_2, this.SenderTel1_3)
				: this.SenderTel1,
				WrappingPaperType = this.WrappingPaperType,
				WrappingPaperName = this.WrappingPaperName,
				WrappingBagType = this.WrappingBagType,
				ShippingCompanyName = this.ShippingCompanyName,
				ShippingCompanyPostName = this.ShippingCompanyPostName,
				SenderCompanyName = this.SenderCompanyName,
				SenderCompanyPostName = this.SenderCompanyPostName,
				AnotherShippingFlg = this.AnotherShippingFlg,
				ShippingMethod = this.ShippingMethod,
				DeliveryCompanyId = this.DeliveryCompanyId,
				ScheduledShippingDate
					= (string.IsNullOrEmpty(this.ScheduledShippingDate)
						? (DateTime?)null
						: DateTime.Parse(this.ScheduledShippingDate)),
				ShippingReceivingStoreFlg = this.ShippingReceivingStoreFlg,
				ShippingReceivingStoreId = this.ShippingReceivingStoreId,
				ShippingExternalDelivertyStatus = this.ShippingExternalDelivertyStatus,
				ShippingStatus = this.ShippingStatus,
				ShippingStatusUpdateDate
					= (string.IsNullOrEmpty(this.ShippingStatusUpdateDate)
						? (DateTime?)null
						: DateTime.Parse(this.ShippingStatusUpdateDate)),
				ShippingReceivingMailDate
					= (string.IsNullOrEmpty(this.ShippingReceivingMailDate)
						? (DateTime?)null
						: DateTime.Parse(this.ShippingReceivingMailDate)),
				ShippingReceivingStoreType = StringUtility.ToEmpty(this.ShippingReceivingStoreType),
				ShippingStatusCode = this.ShippingStatusCode,
				ShippingOfficeName = this.ShippingOfficeName,
				ShippingHandyTime = this.ShippingHandyTime,
				ShippingCurrentStatus = this.ShippingCurrentStatus,
				ShippingStatusDetail = this.ShippingStatusDetail,
				StorePickupRealShopId = this.StorePickupRealShopId,
			};

			// 注文商品
			model.Items = this.Items.Select(i => i.CreateModel()).ToArray();

			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="giftFlg">ギフト購入フラグ</param>
		/// <param name="orderPaymentKbn">支払区分</param>
		/// <param name="isConvenienceStore">Is Convenience Store</param>
		/// <returns>エラーメッセージ</returns>
		public w2.Common.Util.Validator.ErrorMessageList Validate(string giftFlg, string orderPaymentKbn, bool isConvenienceStore = false)
		{
			var shippingInput = new Hashtable
		{
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1, this.ShippingName1 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2, this.ShippingName2 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1, this.ShippingNameKana1 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2, this.ShippingNameKana2 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_1", this.ShippingZip1 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_2", this.ShippingZip2 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1, this.ShippingAddr1 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2, this.ShippingAddr2 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3, this.ShippingAddr3 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4, this.ShippingAddr4 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME, this.ShippingCompanyName },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME, this.ShippingCompanyPostName },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_1", this.ShippingTel1_1 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_2", this.ShippingTel1_2 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_3", this.ShippingTel1_3 },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, this.ShippingDate },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME, this.ShippingTime },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO, this.ShippingCheckNo },
			{ Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE, this.ScheduledShippingDate}
		};

			if (this.IsShippingAddrJp == false)
			{
				shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5, this.ShippingAddr5);
				shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP, this.ShippingZip);
				shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1, this.ShippingTel1);
				shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE, this.ShippingCountryIsoCode);
			}

			if (isConvenienceStore)
			{
				return new w2.Common.Util.Validator.ErrorMessageList();
			}

			var errorMessages = new ErrorMessageList();

			if (this.ShippingAddressKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
			{
				if (string.IsNullOrEmpty(this.StorePickupRealShopId)
					|| (new RealShopService().Get(this.StorePickupRealShopId) == null))
				{
					errorMessages.Add(Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID, "実店舗 ID が存在しません");
				}
			}
			else
			{
				var shippingValidationName = (this.IsShippingAddrJp)
					? (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
						? "OrderShippingAmazonModifyInput"
						: "OrderShippingModifyInput"
					: "OrderShippingModifyInputGlobal";

				errorMessages = w2.Common.Util.Validator.Validate(
					shippingValidationName,
					shippingInput,
					Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE,
					this.ShippingCountryIsoCode);
			}

			// ギフト注文？
			if ((Constants.GIFTORDER_OPTION_ENABLED) && (giftFlg == Constants.FLG_ORDER_GIFT_FLG_ON))
			{
				var giftInput = new Hashtable
			{
				{ Constants.FIELD_ORDERSHIPPING_SENDER_NAME1, this.SenderName1 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_NAME2, this.SenderName2 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1, this.SenderNameKana1 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2, this.SenderNameKana2 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_ZIP + "_1", this.SenderZip1 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_ZIP + "_2", this.SenderZip2 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1, this.SenderAddr1 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2, this.SenderAddr2 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3, this.SenderAddr3 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4, this.SenderAddr4 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME, this.SenderCompanyName },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME, this.SenderCompanyPostName },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_1", this.SenderTel1_1 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_2", this.SenderTel1_2 },
				{ Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_3", this.SenderTel1_3 },
				{ Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE, this.WrappingPaperType },
				{ Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME, this.WrappingPaperName },
				{ Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE, this.WrappingBagType },
			};

				if (this.IsSenderAddrJp == false)
				{
					giftInput.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5, this.SenderAddr5);
					giftInput.Add(Constants.FIELD_ORDERSHIPPING_SENDER_TEL1, this.SenderTel1);
					giftInput.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ZIP, this.SenderZip);
					giftInput.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE, this.SenderCountryIsoCode);
				}
				var giftValidationName = this.IsSenderAddrJp
					? "OrderShippingModifyInput"
					: "OrderShippingModifyInputGlobal";
				errorMessages.AddRange(
					w2.Common.Util.Validator.Validate(
						giftValidationName,
						giftInput,
						Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE,
						this.SenderCountryIsoCode));
			}

			return errorMessages;
		}

		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_ID] = value; }
		}
		/// <summary>配送先枝番</summary>
		public string OrderShippingNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO] = value; }
		}
		/// <summary>配送先氏名</summary>
		public string ShippingName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME] = value; }
		}
		/// <summary>配送先氏名かな</summary>
		public string ShippingNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA] = value; }
		}
		/// <summary>郵便番号</summary>
		public string ShippingZip
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP] = value; }
		}
		/// <summary>郵便番号（ハイフンなし）</summary>
		public string HyphenlessShippingZip
		{
			get { return this.ShippingZip.Replace("-", ""); }
		}
		/// <summary>郵便番号1</summary>
		public string ShippingZip1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_1"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_1"] = value; }
		}
		/// <summary>郵便番号2</summary>
		public string ShippingZip2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_2"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_2"] = value; }
		}
		/// <summary>住所1</summary>
		public string ShippingAddr1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1] = value; }
		}
		/// <summary>住所2</summary>
		public string ShippingAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2] = value; }
		}
		/// <summary>住所3</summary>
		public string ShippingAddr3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3] = value; }
		}
		/// <summary>住所４</summary>
		public string ShippingAddr4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4] = value; }
		}
		/// <summary>電話番号1</summary>
		public string ShippingTel1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1] = value; }
		}
		/// <summary>電話番号1_1</summary>
		public string ShippingTel1_1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_1"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_1"] = value; }
		}
		/// <summary>電話番号1_2</summary>
		public string ShippingTel1_2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_2"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_2"] = value; }
		}
		/// <summary>電話番号1_3</summary>
		public string ShippingTel1_3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_3"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_3"] = value; }
		}
		/// <summary>電話番号2</summary>
		public string ShippingTel2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL2] = value; }
		}
		/// <summary>電話番号3</summary>
		public string ShippingTel3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL3]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL3] = value; }
		}
		/// <summary>ＦＡＸ</summary>
		public string ShippingFax
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_FAX]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_FAX] = value; }
		}
		/// <summary>配送業者</summary>
		public string ShippingCompany
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY] = value; }
		}
		/// <summary>配送希望日</summary>
		public string ShippingDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE] = value; }
		}
		/// <summary>配送希望時間帯</summary>
		public string ShippingTime
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME] = value; }
		}
		/// <summary>配送希望時間帯テキスト</summary>
		public string ShippingTimeText
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME + "_text"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME + "_text"] = value; }
		}
		/// <summary>配送伝票番号</summary>
		public string ShippingCheckNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_DATE_CHANGED] = value; }
		}
		/// <summary>配送先氏名1</summary>
		public string ShippingName1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1] = value; }
		}
		/// <summary>配送先氏名2</summary>
		public string ShippingName2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2] = value; }
		}
		/// <summary>配送先氏名かな1</summary>
		public string ShippingNameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1] = value; }
		}
		/// <summary>配送先氏名かな2</summary>
		public string ShippingNameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2] = value; }
		}
		/// <summary>送り主氏名</summary>
		public string SenderName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME] = value; }
		}
		/// <summary>送り主氏名1</summary>
		public string SenderName1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME1] = value; }
		}
		/// <summary>送り主氏名2</summary>
		public string SenderName2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME2] = value; }
		}
		/// <summary>送り主氏名かな</summary>
		public string SenderNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA] = value; }
		}
		/// <summary>送り主氏名かな1</summary>
		public string SenderNameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1] = value; }
		}
		/// <summary>送り主氏名かな2</summary>
		public string SenderNameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2] = value; }
		}
		/// <summary>送り主郵便番号</summary>
		public string SenderZip
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP] = value; }
		}
		/// <summary>送り主郵便番号1</summary>
		public string SenderZip1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP + "_1"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP + "_1"] = value; }
		}
		/// <summary>送り主郵便番号2</summary>
		public string SenderZip2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP + "_2"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP + "_2"] = value; }
		}
		/// <summary>送り主住所1</summary>
		public string SenderAddr1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1] = value; }
		}
		/// <summary>送り主住所2</summary>
		public string SenderAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2] = value; }
		}
		/// <summary>送り主住所3</summary>
		public string SenderAddr3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3] = value; }
		}
		/// <summary>送り主住所４</summary>
		public string SenderAddr4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4] = value; }
		}
		/// <summary>送り主電話番号1</summary>
		public string SenderTel1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1] = value; }
		}
		/// <summary>送り主電話番号1_1</summary>
		public string SenderTel1_1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_1"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_1"] = value; }
		}
		/// <summary>送り主電話番号1_2</summary>
		public string SenderTel1_2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_2"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_2"] = value; }
		}
		/// <summary>送り主電話番号1_3</summary>
		public string SenderTel1_3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_3"]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_3"] = value; }
		}
		/// <summary>のし種類</summary>
		public string WrappingPaperType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE] = value; }
		}
		/// <summary>のし差出人</summary>
		public string WrappingPaperName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME] = value; }
		}
		/// <summary>包装種類</summary>
		public string WrappingBagType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE] = value; }
		}
		/// <summary>配送先企業名</summary>
		public string ShippingCompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME] = value; }
		}
		/// <summary>配送先部署名</summary>
		public string ShippingCompanyPostName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME] = value; }
		}
		/// <summary>送り主企業名</summary>
		public string SenderCompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME] = value; }
		}
		/// <summary>送り主部署名</summary>
		public string SenderCompanyPostName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME] = value; }
		}
		/// <summary>別送フラグ</summary>
		public string AnotherShippingFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG] = value; }
		}
		/// <summary>配送方法</summary>
		public string ShippingMethod
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD] = value; }
		}
		/// <summary>配送会社ID</summary>
		public string DeliveryCompanyId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID] = value; }
		}
		/// <summary>配送先国ISOコード</summary>
		public string ShippingCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>配送先国名</summary>
		public string ShippingCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME] = value; }
		}
		/// <summary>住所5</summary>
		public string ShippingAddr5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5] = value; }
		}
		/// <summary>送り主国ISOコード</summary>
		public string SenderCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>送り主国名</summary>
		public string SenderCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME] = value; }
		}
		/// <summary>送り主住所5</summary>
		public string SenderAddr5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5] = value; }
		}
		/// <summary>店舗受取フラグ</summary>
		public string ShippingReceivingStoreFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG] = value; }
		}
		/// <summary>店舗受取店舗ID</summary>
		public string ShippingReceivingStoreId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID] = value; }
		}
		/// <summary>配送のペリカン管理番号</summary>
		public string ShippingExternalDelivertyStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_EXTERNAL_DELIVERTY_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_EXTERNAL_DELIVERTY_STATUS] = value; }
		}
		/// <summary>配送状態</summary>
		public string ShippingStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS] = value; }
		}
		/// <summary>配送状態更新日</summary>
		public string ShippingStatusUpdateDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_UPDATE_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_UPDATE_DATE] = value; }
		}
		/// <summary>店舗受取メール送信日</summary>
		public string ShippingReceivingMailDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_MAIL_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_MAIL_DATE] = value; }
		}
		/// <summary>コンビニ受取：受取方法</summary>
		public string ShippingReceivingStoreType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE] = value; }
		}
		/// <summary>完了状態コード</summary>
		public string ShippingStatusCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE] = value; }
		}
		/// <summary>営業所略称</summary>
		public string ShippingOfficeName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_OFFICE_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_OFFICE_NAME] = value; }
		}
		/// <summary>Handy操作時間</summary>
		public string ShippingHandyTime
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_HANDY_TIME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_HANDY_TIME] = value; }
		}
		/// <summary>現在の状態</summary>
		public string ShippingCurrentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS] = value; }
		}
		/// <summary>状態説明</summary>
		public string ShippingStatusDetail
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_DETAIL]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_DETAIL] = value; }
		}
		/// <summary>商品リスト</summary>
		public OrderItemInput[] Items
		{
			get { return (OrderItemInput[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		/// <summary>すべてのアイテムが在庫戻し済みかどうか</summary>
		public bool IsAllItemStockReturned { get { return this.Items.All(i => (i.IsAllowReturnStock == false)); } }
		/// <summary>配送先の住所は日本か</summary>
		public bool IsShippingAddrJp
		{
			get { return GlobalAddressUtil.IsCountryJp(this.ShippingCountryIsoCode); }
		}
		/// <summary>配送先の住所はアメリカか</summary>
		public bool IsShippingAddrUs
		{
			get { return GlobalAddressUtil.IsCountryUs(this.ShippingCountryIsoCode); }
		}
		/// <summary>送り主は日本の住所か</summary>
		public bool IsSenderAddrJp
		{
			get { return GlobalAddressUtil.IsCountryJp(this.SenderCountryIsoCode); }
		}
		/// <summary>送り主は日本のアメリカか</summary>
		public bool IsSenderAddrUs
		{
			get { return GlobalAddressUtil.IsCountryUs(this.SenderCountryIsoCode); }
		}
		/// <summary>出荷予定日</summary>
		public string ScheduledShippingDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE] = value; }
		}
		/// <summary>出荷情報登録連携</summary>
		public bool ExternalShipmentEntry { get; set; }
		/// <summary>更新前配送伝票番号</summary>
		public string OldShippingCheckNo { get; set; }
		/// <summary>Shipping Address Kbn</summary>
		public string ShippingAddressKbn
		{
			get { return (string)this.DataSource["shipping_address_kbn"]; }
			set { this.DataSource["shipping_address_kbn"] = value; }
		}
		/// <summary>Is Shipping Address Taiwan</summary>
		public bool IsShippingAddrTw
		{
			get { return GlobalAddressUtil.IsCountryTw(this.ShippingCountryIsoCode); }
		}
		/// <summary>宅配便か？</summary>
		public bool IsExpress
		{
			get { return (this.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS); }
		}
		/// <summary>Real store opening hours</summary>
		public string OpeningHours
		{
			get { return (string)this.DataSource["opening_hours"]; }
			set { this.DataSource["opening_hours"] = value; }
		}
		/// <summary>受取店舗</summary>
		public string StorePickupRealShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID] = value; }
		}
		#endregion
	}
}
