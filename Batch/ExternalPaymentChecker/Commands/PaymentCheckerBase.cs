/*
=========================================================================================================
  Module      : 入金チェッカー基底クラス(PaymentCheckerBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Sql;

namespace w2.Commerce.Batch.ExternalPaymentChecker.Commands
{
	/// <summary>
	/// 入金チェッカー基底クラス
	/// </summary>
	public abstract class PaymentCheckerBase
	{
		/// <summary>
		/// 実行
		/// </summary>
		public abstract void Exec();

		/// <summary>
		/// 未入金注文取得
		/// </summary>
		/// <param name="orderPaymentKbn">注文支払い区分</param>
		/// <param name="orderDateBegin">注文日（チェック開始日時）</param>
		/// <returns></returns>
		protected DataView GetNonPaymentOrders(string orderPaymentKbn, DateTime orderDateBegin)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Order", "GetNonPaymentOrders"))
			{
				var ht = new Hashtable
				{
					{Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, orderPaymentKbn},
					{"order_date_begin", orderDateBegin}
				};

				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
	}
}
