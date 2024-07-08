/*
=========================================================================================================
  Module      : 更新データ変換クラス (UpdateDataConverter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;

namespace w2.Domain.UpdateHistory.Helper.UpdateData
{
	/// <summary>
	/// 更新データ変換クラス
	/// </summary>
	public class UpdateDataConverter
	{
		/// <summary>
		/// XMLシリアライズ
		/// </summary>
		/// <param name="updateData">対象データ</param>
		/// <returns>シリアライズ後データ</returns>
		public static byte[] CreateXml(object updateData)
		{
			using (var stream = new MemoryStream())
			{
				var serializer = new XmlSerializer(updateData.GetType());
				serializer.Serialize(stream, updateData);

				var data = stream.ToArray();
				return data;
			}
		}

		/// <summary>
		/// 圧縮
		/// </summary>
		/// <param name="source">対象</param>
		/// <returns>圧縮後データ</returns>
		public static byte[] Compress(byte[] source)
		{
			// gzip圧縮
			using (var outStream = new MemoryStream())
			{
				using (var zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
				{
					zipStream.Write(source, 0, source.Length);
				}
				var result = outStream.ToArray();
				return result;
			}
		}

		/// <summary>
		///  更新データインスタンス変換（バイナリ⇒モデル）
		/// </summary>
		/// <param name="updateData">更新データ</param>
		/// <returns>更新データインスタンス</returns>
		public static T Deserialize<T>(byte[] updateData)
			where T : IUpdateData, new()
		{
			if (updateData == null) return (T)(typeof (T).GetConstructor(new Type[0])).Invoke(new object[0]);
			using (var zipStream = new GZipStream(new MemoryStream(updateData), CompressionMode.Decompress, true))
			{
				const int BUFFER_SIZE = 4096;
				byte[] buffer = new byte[BUFFER_SIZE];
				using (MemoryStream outStream = new MemoryStream())
				{
					// gzip解凍
					var bytesRead = 0;
					while ((bytesRead = zipStream.Read(buffer, 0, BUFFER_SIZE)) > 0)
					{
						outStream.Write(buffer, 0, bytesRead);
					}

					// XMLデシリアライズ
					using (var reader = new StringReader(Encoding.UTF8.GetString(outStream.ToArray())))
					using (var xmlReader = XmlReader.Create(reader))
					{
						var serializer = new XmlSerializer(typeof(T));
						return (T)serializer.Deserialize(xmlReader);
					}
				}
			}
		}
	}
}