/*
=========================================================================================================
  Module      :更新データ基底クラス (UpdateDataBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using w2.Common.Util;
using w2.Domain.UserCreditCard;

namespace w2.Domain.UpdateHistory.Helper.UpdateData
{
	/// <summary>
	/// 更新データ基底クラス
	/// </summary>
	public abstract class UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected UpdateDataBase()
		{
			this.KeyValues = new UpdateDataKeyValue[0];
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 更新データXML変換（モデル⇒バイナリ）
		/// </summary>
		/// <param name="historyExludeFields">履歴に格納しないフィールド</param>
		/// <param name="hashExludeFields">ハッシュに含めないフィールド</param>
		/// <returns>シリアライズ化された更新履歴、ハッシュ文字列</returns>
		protected Tuple<byte[], string> SerializeAndCreateHashString(string[] historyExludeFields, string[] hashExludeFields)
		{
			var xmlBytes = UpdateDataConverter.CreateXml(this);
			var xmlString = Encoding.UTF8.GetString(xmlBytes);

			// 履歴データを作成
			var xmlStringForHistory = ExludeFields(xmlString, historyExludeFields);
			var xmlBytesForHistory = Encoding.UTF8.GetBytes(xmlStringForHistory);
			var serialized = UpdateDataConverter.Compress(xmlBytesForHistory);

			// 比較用ハッシュ作成
			var xmlStringForHash = ExludeFields(xmlString, hashExludeFields);
			var xmlBytesForHash = Encoding.UTF8.GetBytes(xmlStringForHash);
			var hashString = UpdateHistoryHashCreator.CreateSha256HashString(xmlBytesForHash);

			return new Tuple<byte[], string>(serialized, hashString);
		}
		#endregion

		/// <summary>
		/// ソースのXMLからフィールドを除外する
		/// </summary>
		/// <param name="source">ソースXML文字列</param>
		/// <param name="exludeFields">除外フィールド</param>
		/// <returns></returns>
		private string ExludeFields(string source, string[] exludeFields)
		{
			if (exludeFields.Length == 0) return source;

			var result = Regex.Replace(source, string.Format("^.* key=\"({0})\".*\n", string.Join("|", exludeFields)), "", RegexOptions.Multiline);
			return result;
		}

		/// <summary>
		/// Hashtableへ変換
		/// </summary>
		/// <returns></returns>
		public Hashtable ToHashtable()
		{
			var result = new Hashtable(this.KeyValues.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
			return result;
		}

		#region プロパティ
		/// <summary>
		/// インデクサ
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns>値</returns>
		public string this[string key]
		{
			get
			{
				var keyValue = this.KeyValues.FirstOrDefault(kv => kv.Key == key);
				return (keyValue != null) ? keyValue.Value : "";
			}
		}

		/// <summary>キー値リスト</summary>
		[XmlArray("KeyValues")]
		[XmlArrayItem("KeyValue")]
		public UpdateDataKeyValue[] KeyValues { get; set; }
		#endregion

		/// <summary>
		/// 更新データクラス（ユーザークレジットカード）
		/// </summary>
		public class UpdateDataUserCreditCard : UpdateDataBase
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public UpdateDataUserCreditCard()
				: base()
			{
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="model">ユーザークレジットカード情報モデル</param>
			public UpdateDataUserCreditCard(UserCreditCardModel model)
				: this()
			{
				this.KeyValues = model.CreateUpdateDataList();
			}
			#endregion
		}
	}

	/// <summary>
	/// 更新データキー値クラス
	/// </summary>
	public class UpdateDataKeyValue
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataKeyValue()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">値</param>
		public UpdateDataKeyValue(string key, object value)
		{
			this.Key = key;
			this.Value = StringUtility.ToEmpty(value);
		}
		#endregion

		#region プロパティ
		/// <summary>キー</summary>
		[XmlAttribute("key")]
		public string Key { get; set; }
		/// <summary>値</summary>
		[XmlAttribute("value")]
		public string Value { get; set; }
		#endregion
	}
}