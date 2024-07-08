/*
=========================================================================================================
  Module      : FLAPSバッチ連携 (FlapsIntegrationAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Xml.Linq;
using w2.App.Common;
using w2.App.Common.Flaps;
using w2.App.Common.Flaps.Order;
using w2.App.Common.Flaps.ProductStock;
using w2.Common.Logger;

namespace w2.Commerce.Batch.FlapsIntegration
{
	/// <summary>
	/// FLAPSバッチ連携
	/// </summary>
	public class FlapsIntegrationAction
	{
		/// <summary>
		///商品同期処理
		/// </summary>
		public void CaptureChangedProducts()
		{
			// 実行できるプロセスを一つに制限する
			var locker = new FlapsIntegrationLocker(Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT);
			if (locker.Lock())
			{
				new FlapsIntegrationFacade().CaptureChangedProducts();
			}
			else
			{
				var msg = "別のプロセスで商品同期処理を行っているため、もうしばらくお待ちください。";
				FileLogger.WriteError(msg);
				Console.WriteLine(msg);
				return;
			}

			locker.Unlock();
		}

		/// <summary>
		/// 商品在庫同期処理
		/// </summary>
		public void CaptureChangedProductStocks()
		{
			// 実行できるプロセスを一つに制限する
			var locker = new FlapsIntegrationLocker(Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCTSTOCK);
			if (locker.Lock())
			{
				new FlapsIntegrationFacade().CaptureChangedProductStocks();
			}
			else
			{
				var msg = "別のプロセスで商品同期処理を行っているため、もうしばらくお待ちください。";
				FileLogger.WriteError(msg);
				Console.WriteLine(msg);
				return;
			}

			locker.Unlock();
		}
	}
}