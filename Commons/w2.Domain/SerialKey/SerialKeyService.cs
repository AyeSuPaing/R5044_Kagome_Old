/*
=========================================================================================================
  Module      : シリアルキー情報サービス (SerialKeyService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Transactions;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;

namespace w2.Domain.SerialKey
{
	/// <summary>
	/// シリアルキー情報サービス
	/// </summary>
	public class SerialKeyService : ServiceBase
	{
		#region +Get シリアルキー情報取得
		/// <summary>
		/// シリアルキー情報取得
		/// </summary>
		/// <param name="serialKey">シリアルキー</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー情報</returns>
		public SerialKeyModel Get(string serialKey, string productId, SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.Get(productId, serialKey);
				return result;
			}
		}
		#endregion

		#region +Delete シリアルキー削除
		/// <summary>
		/// シリアルキー削除
		/// </summary>
		/// <param name="serialKey">シリアルキー</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー削除件数</returns>
		public int Delete(string serialKey, string productId, SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.Delete(serialKey, productId);
				return result;
			}
		}
		#endregion

		#region +Insert シリアルキー登録
		/// <summary>
		/// シリアルキー登録
		/// </summary>
		/// <param name="model">シリアルキー情報モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー登録件数</returns>
		public int Insert(SerialKeyModel model, SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.Insert(model);
				return result;
			}
		}
		#endregion

		#region +Update シリアルキー更新
		/// <summary>
		/// シリアルキー更新
		/// </summary>
		/// <param name="model">シリアルキー情報モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー更新件数</returns>
		public int Update(SerialKeyModel model, SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +GetSerialKeyCount シリアルキー該当件数取得
		/// <summary>
		/// シリアルキー該当件数取得
		/// </summary>
		/// <param name="serialKey">シリアルキー</param>
		/// <param name="productId">商品ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="status">状態</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="sortKbn">ソート区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー該当件数</returns>
		public int GetSerialKeyCount(
			string serialKey,
			string productId,
			string userId,
			string orderId,
			string status,
			string validFlg,
			string sortKbn,
			SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.GetSerialKeyCount(
					serialKey,
					productId,
					userId,
					orderId,
					status,
					validFlg,
					sortKbn);
				return result;
			}
		}
		#endregion

		#region +CheckForReserveSerialKey シリアルキー引当前チェック処理
		/// <summary>
		/// シリアルキー引当前チェック処理
		/// </summary>
		/// <param name="productCount">商品数</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー配列</returns>
		public string[] CheckForReserveSerialKey(
			int productCount,
			string productId,
			string variationId,
			SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.CheckForReserveSerialKey(productCount, productId, variationId);
				return result;
			}
		}
		#endregion

		#region +Authenticate シリアルキー認証
		/// <summary>
		/// シリアルキー認証
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="serialKey">シリアルキー</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー戻しされたか</returns>
		public SerialKeyModel Authenticate(
			string orderId,
			string productId,
			string serialKey,
			SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.Authenticate(orderId, productId, serialKey);
				return result;
			}
		}
		#endregion

		#region +GetSerialKeyList シリアルキー一覧情報取得
		/// <summary>
		/// シリアルキー一覧情報取得
		/// </summary>
		/// <param name="serialKey">シリアルキー</param>
		/// <param name="productId">商品ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="status">ステータス</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="sortKbn">ソート区分</param>
		/// <param name="beginNum">取得開始番号</param>
		/// <param name="endNum">取得終了番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns></returns>
		public SerialKeyModel[] GetSerialKeyList(
			string serialKey,
			string productId,
			string userId,
			string orderId,
			string status,
			string validFlg,
			string sortKbn,
			int beginNum,
			int endNum,
			SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.GetSerialKeyList(
					serialKey,
					productId,
					userId,
					orderId,
					status,
					validFlg,
					sortKbn,
					beginNum,
					endNum);
				return result;
			}
		}
		#endregion

		#region +GetSerialKeyFromOrder 注文からシリアルキー情報取得
		/// <summary>
		/// 注文からシリアルキー情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderItemNo">注文商品枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー情報</returns>
		public SerialKeyModel[] GetSerialKeyFromOrder(string orderId, int orderItemNo, SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.GetSerialKeyFromOrder(orderId, orderItemNo);
				return result;
			}
		}
		#endregion

		#region +GetDeliveredSerialKeys 購入毎のシリアルキー一覧を取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 購入毎のシリアルキー一覧を取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderItemNo">注文商品枝番</param>
		/// <param name="periodDays">有効期限</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー情報</returns>
		public DataView GetDeliveredSerialKeysInDataView(
			string orderId,
			int orderItemNo,
			int periodDays,
			SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.GetDeliveredSerialKeysInDataView(orderId, orderItemNo, periodDays);
				return result;
			}
		}
		#endregion

		#region +UnreserveSerialKey シリアルキー引当戻し処理
		/// <summary>
		/// シリアルキー引当戻し処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー引当戻し件数</returns>
		public int UnreserveSerialKey(string orderId, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.UnreserveSerialKey(orderId, lastChanged);
				return result;
			}
		}
		#endregion

		#region +ReserveSerialKey シリアルキー引当処理
		/// <summary>
		/// シリアルキー引当処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderItemNo">注文商品枝番</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productCount">商品数量（ギフト考慮）</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー引当件数</returns>
		public int ReserveSerialKey(
			string orderId,
			int orderItemNo,
			string userId,
			int productCount,
			string productId,
			string variationId,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.ReserveSerialKey(
					orderId,
					orderItemNo,
					userId,
					productCount,
					productId,
					variationId,
					lastChanged);
				return result;
			}
		}
		#endregion

		#region +DeliverSerialKey シリアルキー引渡処理
		/// <summary>
		/// シリアルキー引渡処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー引渡件数</returns>
		public int DeliverSerialKey(string orderId, string lastChanged, SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.DeliverSerialKey(orderId, lastChanged);
				return result;
			}
		}
		#endregion

		#region +UpdateByCancelOrder 注文シリアルキー戻し
		/// <summary>
		/// 注文シリアルキー戻し
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー戻しされたか</returns>
		public int UpdateByCancelOrder(string orderId, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.UpdateByCancelOrder(orderId, lastChanged);
				return result;
			}
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// CSVへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か</returns>
		public bool ExportToCsv(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new SerialKeyRepository(accessor))
			using (var reader = repository.GetMasterWithReader(
				input,
				new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames)))
			{
				new MasterExportCsv().Exec(
					setting,
					reader,
					outputStream,
					formatDate,
					digitsByKeyCurrency,
					replacePrice);
			}

			return true;
		}

		/// <summary>
		/// Excelへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="excelTemplateSetting">Excelテンプレート設定</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か（件数エラーの場合は失敗）</returns>
		public bool ExportToExcel(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			ExcelTemplateSetting excelTemplateSetting,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			using (var repository = new SerialKeyRepository())
			{
				var dv = repository.GetMaster(input, new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				if (dv.Count >= 20000) return false;

				new MasterExportExcel().Exec(
					setting,
					excelTemplateSetting,
					dv,
					outputStream,
					formatDate,
					digitsByKeyCurrency,
					replacePrice);
				return true;
			}
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <returns>チェックOKか</returns>
		public bool CheckFieldsForGetMaster(string sqlFieldNames)
		{
			try
			{
				using (var repository = new SerialKeyRepository())
				{
					repository.CheckSerialKeyFieldsForGetMaster(
						new Hashtable(),
						new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteWarn(ex);
				return false;
			}

			return true;
		}
		#endregion

		#region +CountSerialKeyForDuplicationCheck シリアルキー件数取得（重複チェック用）
		/// <summary>
		/// シリアルキー件数取得（重複チェック用）
		/// </summary>
		/// <param name="serialKey">シリアルキー</param>
		/// <param name="productId">商品ID</param>
		/// <param name="productIdOld">旧商品ID（更新チェック時に指定）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>シリアルキー該当件数</returns>
		public int CountSerialKeyForDuplicationCheck(
			string serialKey,
			string productId,
			string productIdOld = null,
			SqlAccessor accessor = null)
		{
			using (var repository = new SerialKeyRepository(accessor))
			{
				var result = repository.CountSerialKeyForDuplicationCheck(serialKey, productId, productIdOld);
				return result;
			}
		}
		#endregion
	}
}