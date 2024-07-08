/*
=========================================================================================================
  Module      : WebApi用のヘルパ(WebApiHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace w2.App.Common.LohacoCreatorWebApi
{
	/// <summary>
	/// WebApi用のヘルパ
	/// </summary>
	public static class WebApiHelper
	{
		#region +DeserializeXml デシリアライズ(XML ⇒ オブジェクト)
		/// <summary>
		/// デシリアライズ(XML ⇒ オブジェクト)
		/// </summary>
		/// <typeparam name="T">変換するオブジェクトの型</typeparam>
		/// <param name="xml">対象とするXMLの文字列</param>
		/// <returns>変換したオブジェクト</returns>
		public static T DeserializeXml<T>(string xml)
		{
			var reader = XmlReader.Create(new StringReader(xml), null);
			var serializer = new XmlSerializer(typeof(T));
			var result = (T)serializer.Deserialize(reader);
			return result;
		}
		#endregion

		#region +SerializeXml シリアライズ （オブジェクト ⇒ XML）
		/// <summary>
		/// シリアライズ （オブジェクト ⇒ XML）
		/// </summary>
		/// <param name="target">変換するオブジェクト</param>
		/// <returns>変換したXML</returns>
		public static string SerializeXml<T>(T target)
		{
			var settings = new XmlWriterSettings
			{
				Indent = true,
				OmitXmlDeclaration = true,
			};
			using (var stream = new StringWriter())
			using (var writer = XmlWriter.Create(stream, settings))
			{
				var serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(writer, target);
				// <Req>タグにあるネームスペース定義の置換
				var xml = Regex.Replace(stream.ToString(), @"^<Req[^>]*>", "<Req>");
				return xml;
			}
		}
		#endregion

		#region +DeserializeJson デシリアライズ(JSON ⇒ オブジェクト)
		/// <summary>
		/// デシリアライズ(JSON ⇒ オブジェクト)
		/// </summary>
		/// <typeparam name="T">変換するオブジェクトの型</typeparam>
		/// <param name="json">対象とするJSONの文字列</param>
		/// <returns>変換したオブジェクト</returns>
		public static T DeserializeJson<T>(string json)
		{
			var result = JsonConvert.DeserializeObject<T>(json);
			return result;
		}
		#endregion

		#region +SerializeJson シリアライズ （オブジェクト ⇒ JSON）
		/// <summary>
		/// シリアライズ （オブジェクト ⇒ JSON）
		/// </summary>
		/// <param name="target">変換するオブジェクト</param>
		/// <param name="hasIndent">インデントするか</param>
		/// <param name="isSorted">Json名の順にソートするか</param>
		/// <param name="isAscendingSort">キー名の昇順にソートするかどうか</param>
		/// <returns>変換したJSON</returns>
		public static string SerializeJson<T>(T target, bool hasIndent = false, bool isSorted = true, bool isAscendingSort = true)
		{
			string result = null;
			if (isSorted)
			{
				var sortedObj = Sort((JObject)JToken.FromObject(target), isAscendingSort);

				result = JsonConvert.SerializeObject(
					sortedObj,
					(hasIndent) ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None,
					new PrimitiveToStringConverter());
			}
			else
			{
				result = JsonConvert.SerializeObject(
					target,
					(hasIndent) ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None,
					new PrimitiveToStringConverter());
			}
			return result;
		}
		#endregion

		#region +Sort JSONオブジェクトをキー名の順にソート
		/// <summary>
		/// JSONオブジェクトをキー名の順にソート
		/// </summary>
		/// <param name="target">ソートしたいオブジェクト</param>
		/// <param name="isAscendingSort">true：キー名の昇順にソートする、false：キー名の降順にソートする</param>
		public static JObject Sort(JObject target, bool isAscendingSort = true)
		{
			var props = target.Properties().ToList();
			foreach (var prop in props)
			{
				target.Remove(prop.Name);
			}
			var list = (isAscendingSort) ? props.OrderBy(p => p.Name) : props.OrderByDescending(p => p.Name);
			foreach (var prop in list)
			{
				if (prop.Value is JObject)
				{
					var obj = Sort((JObject)prop.Value, isAscendingSort);
					target.Add(prop.Name, obj);
				}
				else if (prop.Value is JArray)
				{
					var count = ((JArray)prop.Value).Count;
					var itemList = new JArray();
					for (int index = 0; index < count; index++)
					{
						var item = Sort((JObject)prop.Value[index], isAscendingSort);
						itemList.Add(item);
					}
					var property = new JProperty(prop.Name, itemList);
					target.Add(property);
				}
				else
				{
					target.Add(prop);
				}
			}
			return target;
		}
		#endregion

		#region CreateRequestSignature ロハコリクエストから認証キー生成
		/// <summary>
		/// ロハコリクエストから認証キー生成
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="sellerId">ストアアカウント</param>
		/// <param name="privateKey">秘密鍵</param>
		/// <returns>認証キー</returns>
		public static string CreateRequestSignature(BaseRequest request, string sellerId, string privateKey)
		{
			// JsonのReqルートオブジェクトの作成
			var obj = new { Req = request };
			var result = CreateRequestSignature(string.Format("{0}{1}", SerializeJson(obj), sellerId), privateKey);
			return result;
		}
		/// <summary>
		/// データから認証キー生成
		/// </summary>
		/// <param name="data">生成元のデータ</param>
		/// <param name="privateKey">秘密鍵</param>
		/// <returns>認証キー</returns>
		public static string CreateRequestSignature(string data, string privateKey)
		{
			var privateParameters = CreatePrivateKeyParameters(privateKey);
			var signedText = SignData(privateParameters, data, CryptoConfig.MapNameToOID("SHA256"));
			return signedText;
		}
		#endregion

		#region +SignData ハッシュ化
		/// <summary>
		/// ハッシュ化
		/// </summary>
		/// <param name="parameters">RSA暗号パラメータ</param>
		/// <param name="data">ハッシュしたいテキスト</param>
		/// <param name="algorithm">ハッシュアルゴリズム</param>
		/// <returns>ハッシュしたテキスト</returns>
		public static string SignData(RSAParameters parameters, string data, object algorithm)
		{
			using (var rsa = new RSACryptoServiceProvider())
			{
				rsa.ImportParameters(parameters);
				var dataBytes = Encoding.UTF8.GetBytes(data);
				var signBytes = rsa.SignData(dataBytes, algorithm);
				var result = Convert.ToBase64String(signBytes);
				return result;
			}
		}
		#endregion

		#region +Encrypt 暗号化
		/// <summary>
		/// 暗号化
		/// </summary>
		/// <param name="publicKeyParameters">RSA暗号パラメータ</param>
		/// <param name="plainText">プレーンテキスト</param>
		/// <returns>暗号したテキスト</returns>
		public static string Encrypt(RSAParameters publicKeyParameters, string plainText)
		{
			using (var rsa = new RSACryptoServiceProvider())
			{
				rsa.ImportParameters(publicKeyParameters);
				var plainBytes = Encoding.UTF8.GetBytes(plainText);
				var cipherBytes = rsa.Encrypt(plainBytes, true);
				var result = Convert.ToBase64String(cipherBytes);
				return result;
			}
		}
		#endregion

		#region +Decrypt 復号化
		/// <summary>
		/// 復号化
		/// </summary>
		/// <param name="privateKeyParameters">RSA暗号パラメータ</param>
		/// <param name="cipherText">暗号したテキスト</param>
		/// <returns>プレーンテキスト</returns>
		public static string Decrypt(RSAParameters privateKeyParameters, string cipherText)
		{
			using (var rsa = new RSACryptoServiceProvider())
			{
				rsa.ImportParameters(privateKeyParameters);
				var cipherBytes = Convert.FromBase64String(cipherText);
				var plainBytes = rsa.Decrypt(cipherBytes, true);
				var result = Encoding.UTF8.GetString(plainBytes);
				return result;
			}
		}
		#endregion

		#region +CreatePublicKeyParameters RSA暗号の公開鍵から各種パラメータへ変換
		/// <summary>
		/// RSA暗号の公開鍵から各種パラメータへ変換
		/// </summary>
		/// <param name="publicKey">RSA暗号の公開鍵</param>
		/// <returns>RSAの各種パラメータ</returns>
		public static RSAParameters CreatePublicKeyParameters(string publicKey)
		{
			using (var reader = new StringReader(publicKey))
			{
				var pem = new PemReader(reader).ReadPemObject();
				var parameters = PublicKeyFactory.CreateKey(pem.Content) as RsaKeyParameters;

				if (parameters == null) throw new CryptographicException("公開鍵からRSA暗号のパラメータへ変換に失敗しました。");

				var result = new RSAParameters
				{
					Exponent = Adjustment(parameters.Exponent.ToByteArray()),
					Modulus = Adjustment(parameters.Modulus.ToByteArray()),
				};
				return result;
			}
		}
		#endregion

		#region +CreatePrivateKeyParameters RSA暗号の秘密鍵から各種パラメータへ変換
		/// <summary>
		/// RSA暗号の秘密鍵から各種パラメータへ変換
		/// </summary>
		/// <param name="privateKey"> RSA暗号の秘密鍵</param>
		/// <returns>RSAの各種パラメータ</returns>
		public static RSAParameters CreatePrivateKeyParameters(string privateKey)
		{
			using (var reader = new StringReader(privateKey))
			{
				var pem = new PemReader(reader);
				var keyPair = pem.ReadObject() as AsymmetricCipherKeyPair;

				if (keyPair == null) throw new CryptographicException("秘密鍵からRSA暗号のパラメータへ変換に失敗しました。");
				var parameters = keyPair.Private as RsaPrivateCrtKeyParameters;

				var result = new RSAParameters
				{
					D = Adjustment(parameters.Exponent.ToByteArray()),
					DP = Adjustment(parameters.DP.ToByteArray()),
					DQ = Adjustment(parameters.DQ.ToByteArray()),
					Exponent = Adjustment(parameters.PublicExponent.ToByteArray()),
					InverseQ = Adjustment(parameters.QInv.ToByteArray()),
					Modulus = Adjustment(parameters.Modulus.ToByteArray()),
					P = Adjustment(parameters.P.ToByteArray()),
					Q = Adjustment(parameters.Q.ToByteArray()),
				};
				return result;
			}
		}
		#endregion

		#region +Adjustment RSA暗号鍵の各種パラメータの頭1バイトが0x00だったら除外する
		/// <summary>
		/// RSA暗号鍵の各種パラメータの頭1バイトが0x00だったら除外する
		/// 「https://qiita.com/y_yoda/items/f4bd3ec56ac7591c5804」をご参照
		/// </summary>
		/// <param name="bytes">RSA暗号鍵の各種パラメータ</param>
		/// <returns>調整したパラメータ</returns>
		public static byte[] Adjustment(byte[] bytes)
		{
			if ((bytes.Length > 0) && (bytes[0] == 0x00))
			{
				var offset = 1;
				var size = bytes.Length - offset;
				var buffer = new byte[size];
				Buffer.BlockCopy(bytes, offset, buffer, 0, size);
				return buffer;
			}

			return bytes;
		}
		#endregion

		#region +DeepClone オブジェクトのディープコピー
		/// <summary>
		/// オブジェクトのディープコピー
		/// </summary>
		/// <typeparam name="T">対象オブジェクトタイプ</typeparam>
		/// <param name="obj">対象オブジェクト</param>
		/// <returns>コピーしたオブジェクト</returns>
		public static T DeepClone<T>(T obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;
				var result = (T)formatter.Deserialize(ms);
				return result;
			}
		}
		#endregion

		#region PrimitiveToStringConverter 内部クラス
		/// <summary>
		/// JSONの基礎オブジェクトをシリアライズ時、文字列形式に出力できるようにコンバータ
		/// </summary>
		public class PrimitiveToStringConverter : JsonConverter
		{
			/// <summary>
			/// 変換対象オブジェクトのチェック
			/// </summary>
			/// <param name="objectType">オブジェクトタイプ</param>
			/// <returns>true：変換可能、false：変換不可</returns>
			public override bool CanConvert(Type objectType)
			{
				return objectType.IsPrimitive;
			}

			/// <summary>
			/// 文字列からオブジェクトに変換可能かどうか
			/// </summary>
			/// <returns>true</returns>
			public override bool CanWrite
			{
				get { return true; }
			}

			/// <summary>
			/// JSONオブジェクトから文字列に変換
			/// </summary>
			/// <param name="writer">JSONライター</param>
			/// <param name="value">対象オブジェクト</param>
			/// <param name="serializer">JSONシリアライズ</param>
			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				writer.WriteValue(value.ToString().ToLower());
			}

			/// <summary>
			/// 文字列からオブジェクトに変換可能かどうか
			/// </summary>
			/// <returns>false</returns>
			public override bool CanRead
			{
				get { return false; }
			}

			/// <summary>
			/// 文字列からオブジェクトに変換
			/// </summary>
			/// <param name="reader">JSONリーダー</param>
			/// <param name="objectType">オブジェクトタイプ</param>
			/// <param name="value">対象オブジェクト</param>
			/// <param name="serializer">JSONシリアライズ</param>
			/// <returns>例外</returns>
			public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
			{
				JToken token = JToken.Load(reader);
				var result = token.ToObject(objectType);
				return result;
			}
		}
		#endregion

		#region SingleOrArrayConverter 内部クラス
		/// <summary>
		/// 1つ・複数小オブジェクトの配列をシリアライズ時、オブジェクトと配列を出力できるようにコンバータ
		/// </summary>
		public class SingleOrArrayConverter<T> : JsonConverter
		{
			/// <summary>
			/// 変換対象オブジェクトのチェック
			/// </summary>
			/// <param name="objectType">オブジェクトタイプ</param>
			/// <returns>true：変換可能、false：変換不可</returns>
			public override bool CanConvert(Type objectType)
			{
				return (objectType == typeof(List<T>));
			}

			/// <summary>
			/// 文字列からオブジェクトに変換
			/// </summary>
			/// <param name="reader">JSONリーダー</param>
			/// <param name="objectType">オブジェクトタイプ</param>
			/// <param name="value">対象オブジェクト</param>
			/// <param name="serializer">JSONシリアライズ</param>
			/// <returns>JSOｎオブジェクト</returns>
			public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
			{
				var token = JToken.Load(reader);
				object result = null;
				result = (token.Type == JTokenType.Array) ? token.ToObject<List<T>>() : new List<T> { token.ToObject<T>() };
				return result;
			}

			/// <summary>
			/// 文字列からオブジェクトに変換可能かどうか
			/// </summary>
			/// <returns>true</returns>
			public override bool CanWrite
			{
				get { return true; }
			}

			/// <summary>
			/// JSONオブジェクトから文字列に変換
			/// </summary>
			/// <param name="writer">JSONライター</param>
			/// <param name="value">対象オブジェクト</param>
			/// <param name="serializer">JSONシリアライズ</param>
			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				var list = (List<T>)value;
				if (list.Count == 1)
				{
					value = list[0];
				}
				serializer.Serialize(writer, value);
			}
		}
		#endregion
	}
}
