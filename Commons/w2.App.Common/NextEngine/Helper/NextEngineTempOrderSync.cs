/*
=========================================================================================================
  Module      : ネクストエンジン仮注文情報連携クラス(NextEngineTempOrderSync.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Order;
using w2.App.Common.Order.Register;
using w2.Domain.Order;

namespace w2.App.Common.NextEngine.Helper
{
	/// <summary>
	/// ネクストエンジン仮注文情報連携クラス
	/// </summary>
	[Serializable]
	public class NextEngineTempOrderSync
	{
		/// <summary>
		/// コンストラクター
		/// </summary>
		public NextEngineTempOrderSync()
		{
			this.TempOrderSyncFlg = false;
			this.IsSynchronized = false;
		}

		/// <summary>
		/// ネクストエンジン仮注文連携オブジェクトを設定
		/// </summary>
		/// <param name="order">対象注文情報</param>
		/// <param name="nextEngineTempOrderSync">ネクストエンジン仮注文連携オブジェクト</param>
		public static void SetNextEngineTempOrderSync(Hashtable order, NextEngineTempOrderSync nextEngineTempOrderSync)
		{
			order[Constants.NE_REALATION_TEMP_ORDER_SYNC_OBJECT] = nextEngineTempOrderSync ?? new NextEngineTempOrderSync();
		}

		/// <summary>
		/// ネクストエンジン仮注文連携オブジェクトを取得
		/// </summary>
		/// <param name="order">対象注文情報</param>
		public static NextEngineTempOrderSync GetNextEngineTempOrderSync(Hashtable order)
		{
			if ((order[Constants.NE_REALATION_TEMP_ORDER_SYNC_OBJECT] is NextEngineTempOrderSync) == false)
			{
				// null 返さない
				order[Constants.NE_REALATION_TEMP_ORDER_SYNC_OBJECT] = new NextEngineTempOrderSync();
			}

			var result = (NextEngineTempOrderSync)order[Constants.NE_REALATION_TEMP_ORDER_SYNC_OBJECT];
			return result;
		}

		/// <summary>
		/// 仮注文連携処理
		/// </summary>
		/// <param name="orderRegister">注文登録インスタンス</param>
		/// <param name="order">対象注文情報</param>
		/// <param name="cart">登録注文カート</param>
		public void SyncTempOrder(OrderRegisterBase orderRegister, Hashtable order, CartObject cart)
		{
			if ((IsTempOrderSyncOptionEnabled == false)
				|| this.TempOrderSyncFlg == false)
			{
				return;
			}

			orderRegister.SendOrderMailByNextEngine(order, cart);
			this.IsSynchronized = true;
		}

		/// <summary>
		/// 連携済み仮注文キャンセル処理
		/// </summary>
		/// <param name="targetOrder">連携済み注文情報モデル</param>
		/// <returns>エラーメッセージ</returns>
		public Tuple<string ,bool> CancelSynchronizedTempOrder(OrderModel targetOrder)
		{
			if ((IsTempOrderSyncOptionEnabled == false)
				|| (targetOrder == null)
				|| (targetOrder.IsTempOrder == false)
				|| (this.IsSynchronized == false))
			{
				return Tuple.Create(string.Empty, false);
			}

			var cancelNextEngineOrder = OrderCommon.UpdateNextEngineOrderForCancel(targetOrder);
			return Tuple.Create(cancelNextEngineOrder.Item1, cancelNextEngineOrder.Item2);
		}

		/// <summary>
		/// 仮注文連携フラグを設定
		/// </summary>
		/// <param name="syncFlg">連携フラグ</param>
		public void SetTempOrderSyncFlg(bool syncFlg)
		{
			this.TempOrderSyncFlg = IsTempOrderSyncOptionEnabled && syncFlg;
		}

		/// <summary>仮注文連携オプション有効か</summary>
		public static bool IsTempOrderSyncOptionEnabled
		{
			get { return Constants.NE_OPTION_ENABLED && Constants.NE_REALATION_TEMP_ORDER; }
		}
		/// <summary>仮注文連携するかフラグ</summary>
		public bool TempOrderSyncFlg { get; private set; }
		/// <summary>仮注文連携済み注文か</summary>
		public bool IsSynchronized { get; private set; }
	}
}
