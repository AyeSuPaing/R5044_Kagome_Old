
/*
=========================================================================================================
  Module      : FLAPS注文キャンセル連携クラス (FlapsOrderCancellationReplication.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Order;

namespace w2.App.Common.Flaps.Order
{
	/// <summary>
	/// FLAPS注文キャンセル連携クラス
	/// </summary>
	[XmlRoot(ElementName = "FWS")]
	public class FlapsOrderCancellationReplication: FlapsEntity
	{
		internal FlapsOrderCancellationReplication()
		{
			this.Method = "iPTS/DoSubmitOrderReturn";
			this.Request = OrderCancellationApiService.Get;
			this.UserIdCode = Constants.FLAPS_ORDER_USER_ID_CODE;
		}

		/// <summary>
		/// 注文キャンセル実行
		/// </summary>
		/// <param name="accessor">アクセサ</param>
		/// <returns>注文キャンセル結果オブジェクト</returns>
		public OrderCancellationResult Post(SqlAccessor accessor = null)
		{
			if(string.IsNullOrEmpty(this.PosNoSerNo))
			{
				var order = new OrderService().Get(this.Identifier, accessor);
				if (order == null)
				{
					var msg = string.Format("注文が存在しません。DB接続先や注文IDを確かめてください。order_id: {0}", this.Identifier);
					FileLogger.WriteError(msg);
					return null;
				}

				var posNoSerNo = (string)order.DataSource[Constants.FLAPS_ORDEREXTENDSETTING_ATTRNO_FOR_POSNOSERNO];
				if (string.IsNullOrEmpty(posNoSerNo))
				{
					var msg = string.Format(
						"PosNoSerNoが存在しません。DB接続先や注文IDを確かめてください。もしくはこの注文は注文API処理を未実行です。order_id: {0}",
						this.Identifier);
					FileLogger.WriteError(msg);
					return null;
				}

				this.PosNoSerNo = posNoSerNo;
			}
			
			var result = Get<OrderCancellationResult>();
			return result;
		}

		/// <summary>識別キー</summary>
		[XmlElement(ElementName = "Identifier")]
		public string Identifier { get; set; }
		/// <summary>ショップカウンター業績唯一番号</summary>
		[XmlElement(ElementName = "PosNoSerNo")]
		public string PosNoSerNo { get; set; }
		/// <summary>発票日付/返品日付</summary>
		[XmlElement(ElementName = "Date")]
		public string Date { get; set; }
		/// <summary>販売者コード</summary>
		[XmlElement(ElementName = "UserIDCode")]
		public string UserIdCode { get; set; }
		/// <summary>元発票番号</summary>
		[XmlElement(ElementName = "InvoiceNo")]
		public string InvoiceNo { get; set; }
		/// <summary>備考</summary>
		[XmlElement(ElementName = "Remark")]
		public string Remark { get; set; }
		/// <summary>指定返品倉庫コード</summary>
		[XmlElement(ElementName = "DesignateWarehouseCode")]
		public string DesignateWarehouseCode { get; set; }
	}
}
