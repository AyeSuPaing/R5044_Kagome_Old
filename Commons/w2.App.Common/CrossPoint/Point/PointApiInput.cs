/*
=========================================================================================================
  Module      : Point Api Input (PointApiInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// ポイント情報入力クラス
	/// </summary>
	public class PointApiInput
	{
		/// <summary>No point id flag</summary>
		private const string NO_POINT_ID_FLG = "1";

		/// <summary>リクエスト種別</summary>
		public enum RequestType
		{
			Get,
			Register,
			Modify,
			Delete,
			Grant,
			Update,
			GetPointHisList,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointApiInput()
		{
			this.MemberId = string.Empty;
			this.OrderDate = null;
			this.PosNo = string.Empty;
			this.OrderId = string.Empty;
			this.PointId = string.Empty;
			this.BaseGrantPoint = 0m;
			this.SpecialGrantPoint = 0m;
			this.UsePoint = 0m;
			this.PriceTotalInTax = 0m;
			this.PriceTotalNoTax = 0m;
			this.IsNoPointId = false;
			this.UserCode = string.Empty;
			this.UpdatePoint = 0;
			this.StartNo = 0;
			this.EndNo = 0;
			this.Sort = string.Empty;
			this.ReasonId = string.Empty;
		}

		/// <summary>
		/// パラメータ取得
		/// </summary>
		/// <param name="type">リクエスト種別</param>
		/// <returns>パラメータ</returns>
		public Dictionary<string, string> GetParam(RequestType type)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_POINT_MEMBER_ID, this.MemberId },
			};

			switch (type)
			{
				case RequestType.Get:
					param.Add(Constants.CROSS_POINT_PARAM_POINT_USE_POINT, this.UsePoint.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_AMOUNT_TOTAL_IN_TAX, this.PriceTotalInTax.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_AMOUNT_TOTAL_NO_TAX, this.PriceTotalNoTax.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_DETAIL_COUNT, this.Items.Length.ToString());

					foreach (var item in this.Items.Select((detail, index) => new { detail, serial = index + 1 }))
					{
						param.Add(Constants.CROSS_POINT_PARAM_POINT_ITEM_CODE + item.serial, item.detail.ProductId);
						param.Add(Constants.CROSS_POINT_PARAM_POINT_UNIT_PRICE + item.serial, item.detail.Price.ToString("F0"));
						param.Add(Constants.CROSS_POINT_PARAM_POINT_SALES_UNIT_PRICE + item.serial, item.detail.SalesPrice.ToString("F0"));
						param.Add(Constants.CROSS_POINT_PARAM_POINT_QUANTITY + item.serial, item.detail.Quantity.ToString());
						param.Add(Constants.CROSS_POINT_PARAM_POINT_TAX + item.serial, item.detail.Tax.ToString("F0"));
					}

					if (this.IsNoPointId) param.Add(Constants.CROSS_POINT_PARAM_POINT_NO_POINT_ID_FLG, NO_POINT_ID_FLG);
					break;

				case RequestType.Register:
					var orderDate = this.OrderDate.Value.Millisecond == 999 ? this.OrderDate.Value.AddSeconds(1) : this.OrderDate;
					param.Add(Constants.CROSS_POINT_PARAM_POINT_SLIP_DATE, StringUtility.ToDateString(orderDate, "yyyyMMddHHmmss"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_POS_NO, this.PosNo);
					param.Add(Constants.CROSS_POINT_PARAM_POINT_SLIP_NO, this.OrderId);
					param.Add(Constants.CROSS_POINT_PARAM_POINT_POINT_ID, this.PointId);
					param.Add(Constants.CROSS_POINT_PARAM_POINT_BASE_GRANT_POINT, this.BaseGrantPoint.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_SPECIAL_GRANT_POINT, this.SpecialGrantPoint.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_USE_POINT, this.UsePoint.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_AMOUNT_TOTAL_IN_TAX, this.PriceTotalInTax.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_AMOUNT_TOTAL_NO_TAX, this.PriceTotalNoTax.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_DETAIL_COUNT, this.Items.Length.ToString());

					foreach (var item in this.Items.Select((detail, index) => new { detail, serial = index + 1 }))
					{
						param.Add(Constants.CROSS_POINT_PARAM_POINT_ITEM_CODE + item.serial, item.detail.ProductId);
						param.Add(Constants.CROSS_POINT_PARAM_POINT_JAN_CODE + item.serial, item.detail.JanCode);
						param.Add(Constants.CROSS_POINT_PARAM_POINT_ITEM_NAME + item.serial, item.detail.ProductName);
						param.Add(Constants.CROSS_POINT_PARAM_POINT_UNIT_PRICE + item.serial, item.detail.Price.ToString("F0"));
						param.Add(Constants.CROSS_POINT_PARAM_POINT_SALES_UNIT_PRICE + item.serial, item.detail.SalesPrice.ToString("F0"));
						param.Add(Constants.CROSS_POINT_PARAM_POINT_QUANTITY + item.serial, item.detail.Quantity.ToString());
						param.Add(Constants.CROSS_POINT_PARAM_POINT_TAX + item.serial, item.detail.Tax.ToString("F0"));
						param.Add(Constants.CROSS_POINT_PARAM_POINT_ITEM_KBN + item.serial, item.detail.ItemSalesKbn);
					}
					break;

				case RequestType.Modify:
					param.Add(Constants.CROSS_POINT_PARAM_POINT_SLIP_DATE, StringUtility.ToDateString(this.OrderDate, "yyyyMMddHHmmss"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_POS_NO, this.PosNo);
					param.Add(Constants.CROSS_POINT_PARAM_POINT_SLIP_NO, this.OrderId);
					param.Add(Constants.CROSS_POINT_PARAM_POINT_BASE_GRANT_POINT, this.BaseGrantPoint.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_SPECIAL_GRANT_POINT, this.SpecialGrantPoint.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_USE_POINT, this.UsePoint.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_AMOUNT_TOTAL_IN_TAX, this.PriceTotalInTax.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_AMOUNT_TOTAL_NO_TAX, this.PriceTotalNoTax.ToString("F0"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_DETAIL_COUNT, this.Items.Length.ToString());

					foreach (var item in this.Items.Select((detail, index) => new { detail, serial = index + 1 }))
					{
						param.Add(Constants.CROSS_POINT_PARAM_POINT_ITEM_CODE + item.serial, item.detail.ProductId);
						param.Add(Constants.CROSS_POINT_PARAM_POINT_JAN_CODE + item.serial, item.detail.JanCode);
						param.Add(Constants.CROSS_POINT_PARAM_POINT_ITEM_NAME + item.serial, item.detail.ProductName);
						param.Add(Constants.CROSS_POINT_PARAM_POINT_UNIT_PRICE + item.serial, item.detail.Price.ToString("F0"));
						param.Add(Constants.CROSS_POINT_PARAM_POINT_SALES_UNIT_PRICE + item.serial, item.detail.SalesPrice.ToString("F0"));
						param.Add(Constants.CROSS_POINT_PARAM_POINT_QUANTITY + item.serial, item.detail.Quantity.ToString());
						param.Add(Constants.CROSS_POINT_PARAM_POINT_TAX + item.serial, item.detail.Tax.ToString("F0"));
						param.Add(Constants.CROSS_POINT_PARAM_POINT_ITEM_KBN + item.serial, item.detail.ItemSalesKbn);
					}
					break;

				case RequestType.Delete:
					param.Add(Constants.CROSS_POINT_PARAM_POINT_SLIP_DATE, StringUtility.ToDateString(this.OrderDate, "yyyyMMddHHmmss"));
					param.Add(Constants.CROSS_POINT_PARAM_POINT_POS_NO, this.PosNo);
					param.Add(Constants.CROSS_POINT_PARAM_POINT_SLIP_NO, this.OrderId);
					break;

				case RequestType.Grant:
					param.Add(Constants.CROSS_POINT_PARAM_POINT_SLIP_NO, this.OrderId);
					param.Add(Constants.CROSS_POINT_PARAM_POINT_USER_CODE, this.UserCode);
					break;

				case RequestType.Update:
					param.Add(Constants.CROSS_POINT_UPDATE_POINT, this.UpdatePoint.ToString());
					param.Add(Constants.CROSS_POINT_PARAM_POINT_REASON, this.ReasonId);
					break;

				case RequestType.GetPointHisList:
					param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_START_NO, this.StartNo.ToString());
					param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_END_NO, this.EndNo.ToString());
					break;
			}

			return param;
		}

		/// <summary>会員ID</summary>
		public string MemberId { get; set; }
		/// <summary>伝票日付</summary>
		public DateTime? OrderDate { get; set; }
		/// <summary>POS番号</summary>
		public string PosNo { get; set; }
		/// <summary>伝票番号</summary>
		public string OrderId { get; set; }
		/// <summary>ポイントID</summary>
		public string PointId { get; set; }
		/// <summary>基本付与ポイント</summary>
		public decimal BaseGrantPoint { get; set; }
		/// <summary>特別付与ポイント</summary>
		public decimal SpecialGrantPoint { get; set; }
		/// <summary>利用ポイント</summary>
		public decimal UsePoint { get; set; }
		/// <summary>購入金額合計（税込）</summary>
		public decimal PriceTotalInTax { get; set; }
		/// <summary>購入金額合計（税抜）</summary>
		public decimal PriceTotalNoTax { get; set; }
		/// <summary>明細</summary>
		public OrderDetail[] Items { get; set; }
		/// <summary>担当者コード</summary>
		public string UserCode { get; set; }
		/// <summary>PointID不要</summary>
		public bool IsNoPointId { get; set; }
		/// <summary>更新ポイント</summary>
		public int UpdatePoint { get; set; }
		/// <summary>取得開始番号</summary>
		public int StartNo { get; set; }
		/// <summary>取得最終番号</summary>
		public int EndNo { get; set; }
		/// <summary>順序</summary>
		public string Sort { get; set; }
		/// <summary> 理由ID </summary>
		public string ReasonId { get; set; }
	}
}
