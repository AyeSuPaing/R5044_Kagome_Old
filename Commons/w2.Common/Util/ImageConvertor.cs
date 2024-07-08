/*
=========================================================================================================
  Module      : 画像コンバータ(ImageConvertor.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;

namespace w2.Common.Util
{
	///**************************************************************************************
	/// <summary>
	/// 画像変換を行う
	/// </summary>
	/// <remarks>
	/// 画像の圧縮･補完方法についてはお客様のニーズに合わせて対応すべきです。
	/// 問題になるのはクオリティ・補正タイプです。
	/// HighQualityBicubic は数ある補完方法の中でも特に綺麗に補完されるのですが、
	/// その欠点としてクオリティを落とすと画像にノイズが走ってしまいます。
	/// 
	/// ノイズ対策施策として、補正方法を２種類（簡易、完全）用意してあります。
	/// 簡易補正はノイズが入りやすい外側を切り取る方法、
	/// 完全補正は画像を鏡面反転させたものを並べてノイズの発生を防ぐ対応ですが低速です。
	/// </remarks>
	///**************************************************************************************
	public class ImageConvertor
	{
		/// <summary>補正タイプ</summary>
		public enum SupplementType
		{
			/// <summary>補正なし（高速）</summary>
			None,
			/// <summary>簡易補正（高速）</summary>
			Simple,
			/// <summary>完全補正（低速）</summary>
			Complete
		}

		/// <summary>
		/// バイト列へ画像コンバートする
		/// </summary>
		/// <param name="imgSrc">対象イメージ</param>
		/// <param name="iWidth">変換後幅</param>
		/// <param name="iHeight">変換後高さ</param>
		/// <param name="stSupplementType">補正タイプ</param>
		/// <param name="ifFormat">フォーマット</param>
		/// <param name="iEncodeQuality">圧縮クオリティ(10～100）</param>
		/// <returns>バイト配列</returns>
		public static byte[] ConvertToBytes(
			Image imgSrc,
			int iWidth,
			int iHeight,
			SupplementType stSupplementType,
			ImageFormat ifFormat,
			int iEncodeQuality)
		{
			using (MemoryStream msMemoryStream = new MemoryStream())
			{
				//------------------------------------------------------
				// コンバート（メモリストリームへ書き込み）
				//------------------------------------------------------
				ConvertToStream(
					imgSrc,
					iWidth,
					iHeight,
					stSupplementType,
					ifFormat,
					iEncodeQuality,
					msMemoryStream);

				//------------------------------------------------------
				// byte配列へ書き出し（※Response.OutputStreamに直接保存するとpngやbmp作成に失敗するため）
				//------------------------------------------------------
				byte[] bBuffer = new byte[(int)msMemoryStream.Length];
				msMemoryStream.Seek(0, System.IO.SeekOrigin.Begin);
				msMemoryStream.Read(bBuffer, 0, (int)msMemoryStream.Length);

				return bBuffer;
			}
		}

		/// <summary>
		/// 画像コンバートしてファイルへ保存する
		/// </summary>
		/// <param name="imgSrc">対象イメージ</param>
		/// <param name="iWidth">変換後幅</param>
		/// <param name="iHeight">変換後高さ</param>
		/// <param name="stSupplementType">補正タイプ</param>
		/// <param name="ifFormat">フォーマット</param>
		/// <param name="iEncodeQuality">圧縮クオリティ(10～100）</param>
		/// <param name="strDestFilePath">出力対象ファイルパス</param>
		public static void ConvertToFile(
			Image imgSrc,
			int iWidth,
			int iHeight,
			SupplementType stSupplementType,
			ImageFormat ifFormat,
			int iEncodeQuality,
			string strDestFilePath)
		{
			using (FileStream fsFileStream = new FileStream(strDestFilePath, FileMode.Create, FileAccess.Write))
			{
				//------------------------------------------------------
				// コンバート（ファイルストリームへ書き込み）
				//------------------------------------------------------
				ConvertToStream(
					imgSrc,
					iWidth,
					iHeight,
					stSupplementType,
					ifFormat,
					iEncodeQuality,
					fsFileStream);
			}
		}

		/// <summary>
		/// 画像コンバートしてストリームに出力する
		/// </summary>
		/// <param name="imgSrc">対象イメージ</param>
		/// <param name="iWidth">変換後幅</param>
		/// <param name="iHeight">変換後高さ</param>
		/// <param name="stSupplementType">補正タイプ</param>
		/// <param name="ifFormat">フォーマット</param>
		/// <param name="iEncodeQuality">圧縮クオリティ(10～100）</param>
		/// <param name="sOutputStream">出力ストリーム</param>
		private static void ConvertToStream(
			Image imgSrc,
			int iWidth,
			int iHeight,
			SupplementType stSupplementType,
			ImageFormat ifFormat,
			int iEncodeQuality,
			Stream sOutputStream)
		{
			using (Bitmap bmDest = new Bitmap(iWidth, iHeight))
			using (Graphics grph = Graphics.FromImage(bmDest))
			{
				//------------------------------------------------------
				// 初期設定
				//------------------------------------------------------
				// クオリティは現状Defaultとする
				grph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

				// 高品質で低速なレンダリングを指定します。
				grph.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

				// アンチエイリアス処理されたレタリングを指定します。
				grph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

				// 補完方法指定（ハイクオリティ・バイキュービック方式）
				grph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

				// エンコーダに渡すパラメタ作成（画像形式、クオリティ指定）
				ImageCodecInfo iciEncoder = null;
				foreach (ImageCodecInfo ici in ImageCodecInfo.GetImageEncoders())
				{
					if (ici.FormatID == ifFormat.Guid)
					{
						iciEncoder = ici;
						break;
					}
				}
				EncoderParameters epParams = new EncoderParameters(1);
				epParams.Param[0] = new EncoderParameter(Encoder.Quality, iEncodeQuality);

				//------------------------------------------------------
				// 補完向け補正処理
				//------------------------------------------------------
				// バイキュービック方式で縮小すると左端と上端に薄い線が入るため、補正処理を行う
				switch (stSupplementType)
				{
					// 補正なし
					case SupplementType.None:
						grph.DrawImage(imgSrc, 0, 0, iWidth, iHeight);
						break;

					// 簡易補正（ピクセル調整）
					case SupplementType.Simple:
						grph.DrawImage(imgSrc, -2, -2, iWidth + 3, iHeight + 3);
						break;

					// 完全補正（画像を水平垂直方向に反転し、4*4に並べ、3行3列目に描写された部分を取得）
					case SupplementType.Complete:
						using (Bitmap bmTemp = new Bitmap(imgSrc.Width * 4, imgSrc.Height * 4))
						using (Graphics grphTemp = Graphics.FromImage(bmTemp))
						using (TextureBrush tbBrush = new TextureBrush(imgSrc, System.Drawing.Drawing2D.WrapMode.TileFlipXY))
						{
							grphTemp.FillRectangle(tbBrush, new Rectangle(0, 0, imgSrc.Width * 4, imgSrc.Height * 4));
							grph.DrawImage(bmTemp, iWidth * -2, iHeight * -2, iWidth * 4, iHeight * 4);
						}
						break;
				}

				//------------------------------------------------------
				// 画像を出力
				//------------------------------------------------------
				bmDest.Save(sOutputStream, iciEncoder, epParams);
			}
		}
	}
}
