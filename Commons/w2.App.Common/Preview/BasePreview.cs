/*
=========================================================================================================
  Module      : プレビュー基底クラス(BasePreview.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Xml;
using System.Text;
using System.Data;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using w2.Common.Sql;

namespace w2.App.Common.Preview
{
	public abstract class BasePreview
	{
		/// <summary>
		/// プレビュー情報登録
		/// </summary>
		/// <param name="previewKbn">プレビュー区分</param>
		/// <param name="previewId1">プレビューID1</param>
		/// <param name="previewId2">プレビューID2</param>
		/// <param name="previewId3">プレビューID3</param>
		/// <param name="previewId4">プレビューID4</param>
		/// <param name="previewId5">プレビューID5</param>
		/// <param name="previewData">データ</param>
		public static void InsertPreview(
			string previewKbn,
			string previewId1,
			string previewId2,
			string previewId3,
			string previewId4,
			string previewId5,
			DataTable previewData)
		{
			InsertPreview<DataTable>(
				previewKbn,
				previewId1,
				previewId2,
				previewId3,
				previewId4,
				previewId5,
				previewData);
		}
		/// <summary>
		/// プレビュー情報登録
		/// </summary>
		/// <param name="previewKbn">プレビュー区分</param>
		/// <param name="previewId1">プレビューID1</param>
		/// <param name="previewId2">プレビューID2</param>
		/// <param name="previewId3">プレビューID3</param>
		/// <param name="previewId4">プレビューID4</param>
		/// <param name="previewId5">プレビューID5</param>
		/// <param name="previewData">データ</param>
		public static void InsertPreview<T>(
			string previewKbn,
			string previewId1,
			string previewId2,
			string previewId3,
			string previewId4,
			string previewId5,
			T previewData)
		{
			//------------------------------------------------------
			// シリアライズ化XML作成
			//------------------------------------------------------
			using (var sw = new StringWriter())
			using (var xw = new XmlTextWriter(sw))
			{
				var xs = new DataContractSerializer(typeof(T));
				xs.WriteObject(xw, previewData);

				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();

					//------------------------------------------------------
					// プレビュー情報削除
					//------------------------------------------------------
					using (var statement = new SqlStatement("Preview", "DeletePreview"))
					{
						var htInput = new Hashtable
						{
							{ Constants.FIELD_PREVIEW_PREVIEW_KBN, previewKbn },
							{ Constants.FIELD_PREVIEW_PREVIEW_ID1, previewId1 },
							{ Constants.FIELD_PREVIEW_PREVIEW_ID2, previewId2 },
							{ Constants.FIELD_PREVIEW_PREVIEW_ID3, previewId3 },
							{ Constants.FIELD_PREVIEW_PREVIEW_ID4, previewId4 },
							{ Constants.FIELD_PREVIEW_PREVIEW_ID5, previewId5 },
							{ Constants.FIELD_PREVIEW_DATE_CREATED, DateTime.Now }
						};

						statement.ExecStatement(accessor, htInput);
					}

					//------------------------------------------------------
					// プレビュー情報登録
					//------------------------------------------------------
					using (var statement = new SqlStatement("Preview", "InsertPreview"))
					{
						var htInput = new Hashtable
						{
							{ Constants.FIELD_PREVIEW_PREVIEW_KBN, previewKbn },
							{ Constants.FIELD_PREVIEW_PREVIEW_ID1, previewId1 },
							{ Constants.FIELD_PREVIEW_PREVIEW_ID2, previewId2 },
							{ Constants.FIELD_PREVIEW_PREVIEW_ID3, previewId3 },
							{ Constants.FIELD_PREVIEW_PREVIEW_ID4, previewId4 },
							{ Constants.FIELD_PREVIEW_PREVIEW_ID5, previewId5 },
							{ Constants.FIELD_PREVIEW_PREVIEW_DATA, sw.ToString() }
						};

						statement.ExecStatement(accessor, htInput);
					}

					accessor.CommitTransaction();
				}
			}
		}

		/// <summary>
		/// プレビュー情報取得
		/// </summary>
		/// <param name="previewKbn">プレビュー区分</param>
		/// <param name="previewId1">プレビューID1</param>
		/// <param name="previewId2">プレビューID2</param>
		/// <param name="previewId3">プレビューID3</param>
		/// <param name="previewId4">プレビューID4</param>
		/// <param name="previewId5">プレビューID5</param>
		/// <returns>プレビューデータ</returns>
		public static DataView GetPreview(string previewKbn, string previewId1, string previewId2, string previewId3, string previewId4, string previewId5)
		{
			var dtPreview = GetPreview<DataTable>(
				previewKbn,
				previewId1,
				previewId2,
				previewId3,
				previewId4,
				previewId5);
			var dvResult = dtPreview.DefaultView;
			return dvResult;
		}
		/// <summary>
		/// プレビュー情報取得
		/// </summary>
		/// <typeparam name="T">デシリアライズするクラス</typeparam>
		/// <param name="previewKbn">プレビュー区分</param>
		/// <param name="previewId1">プレビューID1</param>
		/// <param name="previewId2">プレビューID2</param>
		/// <param name="previewId3">プレビューID3</param>
		/// <param name="previewId4">プレビューID4</param>
		/// <param name="previewId5">プレビューID5</param>
		/// <returns></returns>
		public static T GetPreview<T>(
			string previewKbn,
			string previewId1,
			string previewId2,
			string previewId3,
			string previewId4,
			string previewId5)
		{
			var resultPreview = GetPreviews<T>(
				previewKbn,
				previewId1,
				previewId2,
				previewId3,
				previewId4,
				previewId5);

			return resultPreview;
		}

		/// <summary>
		/// プレビュー情報取得
		/// </summary>
		/// <typeparam name="T">デシリアライズするクラス</typeparam>
		/// <param name="previewKbn">プレビュー区分</param>
		/// <param name="previewId1">プレビューID1</param>
		/// <param name="previewId2">プレビューID2</param>
		/// <param name="previewId3">プレビューID3</param>
		/// <param name="previewId4">プレビューID4</param>
		/// <param name="previewId5">プレビューID5</param>
		/// <returns></returns>
		public static T GetPreviews<T>(
			string previewKbn,
			string previewId1,
			string previewId2,
			string previewId3,
			string previewId4,
			string previewId5)
		{
			var resultPreview = default(T);
			//------------------------------------------------------
			// フィールドデータ取得
			//------------------------------------------------------
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Preview", "GetPreview"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_PREVIEW_PREVIEW_KBN, previewKbn },
					{ Constants.FIELD_PREVIEW_PREVIEW_ID1, previewId1 },
					{ Constants.FIELD_PREVIEW_PREVIEW_ID2, previewId2 },
					{ Constants.FIELD_PREVIEW_PREVIEW_ID3, previewId3 },
					{ Constants.FIELD_PREVIEW_PREVIEW_ID4, previewId4 },
					{ Constants.FIELD_PREVIEW_PREVIEW_ID5, previewId5 }
				};

				// プレビュー情報取得
				var dvPreview = statement.SelectSingleStatementWithOC(accessor, htInput);

				// プレビュー情報が存在する？
				if (dvPreview.Count <= 0) return resultPreview;

				// 逆シリアライズ化し、フィールドデータを取得
				var xs = new DataContractSerializer(typeof(T));

				var drvPreview = dvPreview.Cast<DataRowView>().FirstOrDefault();

				resultPreview = (T)xs.ReadObject(
					new MemoryStream(
						Encoding.UTF8.GetBytes(
							(drvPreview == null) ? "" : drvPreview[Constants.FIELD_PREVIEW_PREVIEW_DATA].ToString())));
			}

			return resultPreview;
		}

		/// <summary>
		/// ハッシュ値を生成する
		/// </summary>
		/// <param name="previewKbn">プレビュー区分(※ハッシュ生成元の文字列)</param>
		/// <param name="iHashLength">ハッシュ値文字数</param>
		/// <returns>ハッシュ値</returns>
		public static string CreateHash(string previewKbn, int iHashLength)
		{
			//------------------------------------------------------
			// ハッシュ値を生成する
			//------------------------------------------------------
			// 現在日付（"yyyy/MM/dd"） + プレビュー区分を連結
			var strSeed = DateTime.Now.ToString("yyyy/MM/dd") + previewKbn;

			// 文字列をbyte型配列に変換
			byte[] byteData = System.Text.Encoding.ASCII.GetBytes(strSeed);

			// MD5CryptoServiceProviderオブジェクトを作成
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

			// ハッシュ値を計算する
			byte[] byteHash = md5.ComputeHash(byteData);

			// byte型配列を16進数の文字列に変換
			StringBuilder sbHash = new System.Text.StringBuilder();
			foreach (byte b in byteHash)
			{
				sbHash.Append(b.ToString("x2"));
			}

			// ハッシュ値を設定
			string strResult = sbHash.ToString();

			// 文字数を設定
			if (strResult.Length > iHashLength)
			{
				strResult = strResult.Substring(0, iHashLength);
			}

			return strResult;
		}
	}
}
