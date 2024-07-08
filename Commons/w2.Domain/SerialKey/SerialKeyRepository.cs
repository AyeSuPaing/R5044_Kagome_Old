/*
=========================================================================================================
  Module      : シリアルキー情報リポジトリ (SerialKeyRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.SerialKey.Helper;

namespace w2.Domain.SerialKey
{
	/// <summary>
	/// シリアルキー情報リポジトリ
	/// </summary>
	internal class SerialKeyRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "SerialKey";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal SerialKeyRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal SerialKeyRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get シリアルキー情報取得
		/// <summary>
		/// シリアルキー認証
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="serialKey">シリアルキー</param>
		/// <returns>シリアルキー情報</returns>
		internal SerialKeyModel Get(
			string productId,
			string serialKey)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_SERIAL_KEY, serialKey},
				{ Constants.FIELD_SERIALKEY_PRODUCT_ID, productId }
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new SerialKeyModel(dv[0]);
		}
		#endregion

		#region ~Delete シリアルキー削除
		/// <summary>
		/// シリアルキー削除
		/// </summary>
		/// <param name="serialKey">シリアルキー</param>
		/// <param name="productId">商品ID</param>
		/// <returns>シリアルキー削除件数</returns>
		internal int Delete(string serialKey, string productId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_SERIAL_KEY, serialKey },
				{ Constants.FIELD_SERIALKEY_PRODUCT_ID, productId }
			};
			return Exec(XML_KEY_NAME, "Delete", ht);
		}
		#endregion

		#region ~Insert シリアルキー登録
		/// <summary>
		/// シリアルキー登録
		/// </summary>
		/// <param name="model">シリアルキー情報モデル</param>
		/// <returns>シリアルキー登録件数</returns>
		internal int Insert(SerialKeyModel model)
		{
			return Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update シリアルキー更新
		/// <summary>
		/// シリアルキー更新
		/// </summary>
		/// <param name="model">シリアルキー情報モデル</param>
		/// <returns>シリアルキー更新件数</returns>
		internal int Update(SerialKeyModel model)
		{
			return Exec(XML_KEY_NAME, "Update", model.DataSource);
		}
		#endregion

		#region ~GetSerialKeyCount シリアルキー該当件数取得
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
		/// <returns>シリアルキー該当件数</returns>
		internal int GetSerialKeyCount(
			string serialKey,
			string productId,
			string userId,
			string orderId,
			string status,
			string validFlg,
			string sortKbn)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_SERIAL_KEY + Constants.FLG_LIKE_ESCAPED, serialKey },
				{ Constants.FIELD_SERIALKEY_PRODUCT_ID + Constants.FLG_LIKE_ESCAPED, productId },
				{ Constants.FIELD_SERIALKEY_USER_ID + Constants.FLG_LIKE_ESCAPED, userId },
				{ Constants.FIELD_SERIALKEY_ORDER_ID + Constants.FLG_LIKE_ESCAPED, orderId },
				{ Constants.FIELD_SERIALKEY_STATUS, status },
				{ Constants.FIELD_SERIALKEY_VALID_FLG, validFlg },
				{ "sort_kbn", sortKbn },
			};

			var dv = Get(XML_KEY_NAME, "GetSerialKeyCount", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region ~CheckForReserveSerialKey シリアルキー引当前チェック処理
		/// <summary>
		/// シリアルキー引当前チェック処理
		/// </summary>
		/// <param name="productCount">商品数</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <returns>シリアルキー配列</returns>
		internal string[] CheckForReserveSerialKey(
			int productCount,
			string productId,
			string variationId)
		{
			var ht = new Hashtable
			{
				{ "key_count", productCount },
				{ Constants.FIELD_SERIALKEY_PRODUCT_ID, productId },
				{ Constants.FIELD_SERIALKEY_VARIATION_ID, variationId },
			};
			var dv = Get(XML_KEY_NAME, "CheckForReserveSerialKey", ht);
			return dv.Cast<DataRowView>().Select(drv => (string)drv[Constants.FIELD_SERIALKEY_SERIAL_KEY]).ToArray();
		}
		#endregion

		#region ~Authenticate シリアルキー認証
		/// <summary>
		/// シリアルキー認証
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="serialKey">シリアルキー</param>
		/// <returns>シリアルキー情報</returns>
		internal SerialKeyModel Authenticate(
			string orderId,
			string productId,
			string serialKey)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_ORDER_ID, orderId },
				{ Constants.FIELD_SERIALKEY_PRODUCT_ID, productId },
				{ Constants.FIELD_SERIALKEY_SERIAL_KEY, SerialKeyUtility.EncryptSerialKey(serialKey)}
			};
			var dv = Get(XML_KEY_NAME, "Authenticate", ht);
			if (dv.Count == 0) return null;
			return new SerialKeyModel(dv[0]);
		}
		#endregion

		#region ~GetSerialKeyList シリアルキー一覧情報取得
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
		/// <returns></returns>
		internal SerialKeyModel[] GetSerialKeyList(
			string serialKey,
			string productId,
			string userId,
			string orderId,
			string status,
			string validFlg,
			string sortKbn,
			int beginNum,
			int endNum)
		{
			var ht = new Hashtable()
			{
				{ Constants.FIELD_SERIALKEY_SERIAL_KEY + Constants.FLG_LIKE_ESCAPED, serialKey },
				{ Constants.FIELD_SERIALKEY_PRODUCT_ID + Constants.FLG_LIKE_ESCAPED, productId },
				{ Constants.FIELD_SERIALKEY_USER_ID + Constants.FLG_LIKE_ESCAPED, userId },
				{ Constants.FIELD_SERIALKEY_ORDER_ID + Constants.FLG_LIKE_ESCAPED, orderId },
				{ Constants.FIELD_SERIALKEY_STATUS, status },
				{ Constants.FIELD_SERIALKEY_VALID_FLG, validFlg },
				{ "sort_kbn", sortKbn },
				{ Constants.FIELD_COMMON_BEGIN_NUM, beginNum },
				{ Constants.FIELD_COMMON_END_NUM, endNum }
			};
			var dv = Get(XML_KEY_NAME, "GetSerialKeyList", ht);
			return dv.Cast<DataRowView>().Select(drv => new SerialKeyModel(drv)).ToArray();
		}
		#endregion

		#region ~GetSerialKeyFromOrder 注文からシリアルキー情報取得
		/// <summary>
		/// 注文からシリアルキー情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderItemNo">注文商品枝番</param>
		/// <returns>シリアルキー情報</returns>
		internal SerialKeyModel[] GetSerialKeyFromOrder(
			string orderId,
			int orderItemNo)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_ORDER_ID, orderId },
				{ Constants.FIELD_SERIALKEY_ORDER_ITEM_NO, orderItemNo }
			};
			var dv = Get(XML_KEY_NAME, "GetSerialKeyFromOrder", ht);
			return dv.Cast<DataRowView>().Select(drv => new SerialKeyModel(drv)).ToArray();
		}
		#endregion

		#region ~GetDeliveredSerialKeysInDataView 購入毎のシリアルキー一覧を取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 購入毎のシリアルキー一覧を取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderItemNo">注文商品枝番</param>
		/// <param name="periodDays">有効期限</param>
		/// <returns>シリアルキー情報</returns>
		internal DataView GetDeliveredSerialKeysInDataView(
			string orderId,
			int orderItemNo,
			int periodDays
			)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_ORDER_ID, orderId },
				{ Constants.FIELD_SERIALKEY_ORDER_ITEM_NO, orderItemNo },
				{ "period_days", periodDays }
			};
			var dv = Get(XML_KEY_NAME, "GetDeliveredSerialKeys", ht);
			return dv;
		}
		#endregion

		#region ~UnreserveSerialKey シリアルキー引当戻し処理
		/// <summary>
		/// シリアルキー引当戻し処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>シリアルキー引当戻し件数</returns>
		internal int UnreserveSerialKey(string orderId, string lastChanged)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_ORDER_ID, orderId },
				{ Constants.FIELD_SERIALKEY_LAST_CHANGED, lastChanged }
			};
			return Exec(XML_KEY_NAME, "UnreserveSerialKey", ht);
		}
		#endregion

		#region ~DeliverSerialKey シリアルキー引渡処理
		/// <summary>
		/// シリアルキー引渡処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>シリアルキー引渡件数</returns>
		internal int DeliverSerialKey(string orderId, string lastChanged)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_ORDER_ID, orderId },
				{ Constants.FIELD_SERIALKEY_LAST_CHANGED, lastChanged }
			};
			return Exec(XML_KEY_NAME, "DeliverSerialKey", ht);
		}
		#endregion

		#region ~ReserveSerialKey シリアルキー引当処理
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
		/// <returns>シリアルキー引当件数</returns>
		internal int ReserveSerialKey(
			string orderId,
			int orderItemNo,
			string userId,
			int productCount,
			string productId,
			string variationId,
			string lastChanged)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_ORDER_ID, orderId},
				{ Constants.FIELD_SERIALKEY_ORDER_ITEM_NO, orderItemNo },
				{ Constants.FIELD_SERIALKEY_USER_ID, userId },
				{ "key_count", productCount },
				{ Constants.FIELD_SERIALKEY_PRODUCT_ID, productId },
				{ Constants.FIELD_SERIALKEY_VARIATION_ID, variationId },
				{ Constants.FIELD_SERIALKEY_LAST_CHANGED, lastChanged }
			};

			return Exec(XML_KEY_NAME, "ReserveSerialKey", ht);
		}
		#endregion

		#region ~UpdateByCancelOrder 注文シリアルキー戻し
		/// <summary>
		/// 注文シリアルキー戻し
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>シリアルキー戻しされたか</returns>
		internal int UpdateByCancelOrder(string orderId, string lastChanged)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_ORDER_ID, orderId },
				{ Constants.FIELD_SERIALKEY_LAST_CHANGED, lastChanged }
			};
			return Exec(XML_KEY_NAME, "UpdateByCancelOrder", ht);
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, "GetSerialKeyMaster", input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, "GetSerialKeyMaster", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckSerialKeyFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckSerialKeyFields", input, replaces: replaces);
		}
		#endregion

		#region ~CountSerialKeyForDuplicationCheck シリアルキー件数取得（重複チェック用）
		/// <summary>
		/// シリアルキー件数取得（重複チェック用）
		/// </summary>
		/// <param name="serialKey">シリアルキー</param>
		/// <param name="productId">商品ID</param>
		/// <param name="productIdOld">旧商品ID</param>
		/// <returns>シリアルキー該当件数</returns>
		internal int CountSerialKeyForDuplicationCheck(
			string serialKey,
			string productId,
			string productIdOld)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SERIALKEY_SERIAL_KEY, serialKey },
				{ Constants.FIELD_SERIALKEY_PRODUCT_ID, productId },
				{ Constants.FIELD_SERIALKEY_PRODUCT_ID + "_old", productIdOld },
			};

			var dv = Get(XML_KEY_NAME, "CountSerialKeyForDuplicationCheck", ht);
			return (int)dv[0][0];
		}
		#endregion
	}
}
