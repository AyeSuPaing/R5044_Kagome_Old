/*
=========================================================================================================
  Module      : 画像変換ユーティリティ(ImageConvert.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Drawing;
using System.Drawing.Imaging;
using w2.Common.Util;

/// <summary>
/// ImageConvert の概要の説明です
/// </summary>
public class ImageConvert
{
	private const int IMAGE_SIZE_MAX = 2000;

	/// <summary>
	/// 画像リサイズ処理
	/// </summary>
	/// <param name="src">変換元</param>
	/// <param name="dest">変換先</param>
	/// <param name="convertWidth">指定サイズ(横幅)　※nullはサイズ未指定</param>
	/// <param name="convertHeight">指定サイズ(縦幅)　※nullはサイズ未指定</param>
	/// <returns>成功/失敗</returns>
	/// <remarks>画像の縦横比は変えずに、指定サイズを超えない範囲内で拡大/縮小する</remarks>
	public static bool ResizeImage(string src, string dest, int? convertWidth, int? convertHeight)
	{
		// 指定サイズがそれぞれ未指定の場合は、画像変換処理を行わない
		if ((convertWidth == null) && (convertHeight == null)) return false;

		// 指定サイズの横幅・縦幅調整
		if (convertWidth != null)
		{
			convertWidth = (convertWidth > IMAGE_SIZE_MAX) ? IMAGE_SIZE_MAX : convertWidth;
		}
		if (convertHeight != null)
		{
			convertHeight = (convertHeight > IMAGE_SIZE_MAX) ? IMAGE_SIZE_MAX : convertHeight;
		}

		try
		{
			using (Image imgSrc = Image.FromFile(src, true))
			{
				var resizeToWidth = false;

				//------------------------------------------------------
				// 変換後サイズ算出処理
				//------------------------------------------------------
				// 指定サイズの縦横幅どちらかが未指定であれば、指定してある縦横幅に合わせる
				if ((convertWidth == null) ^ (convertHeight == null))
				{
					resizeToWidth = (convertHeight == null);
				}
				else
				{
					// 元サイズと指定サイズの縦横比（縦幅/横幅）が指定サイズの方が大きければ、指定サイズの横幅に合わせる
					// 元サイズと指定サイズの縦横比（縦幅/横幅）が元サイズの方が大きければ、指定サイズの縦幅に合わせる
					resizeToWidth = (((float)imgSrc.Size.Height / imgSrc.Size.Width) < ((float)convertHeight / convertWidth));
				}

				// 変換後サイズ算出（四捨五入）
				int iResizedWidth = (resizeToWidth) ? convertWidth.Value : (int)((imgSrc.Size.Width * ((float)convertHeight / imgSrc.Size.Height)) + 0.5);
				int iResizedHeight = (resizeToWidth == false) ? convertHeight.Value : (int)((imgSrc.Size.Height * ((float)convertWidth / imgSrc.Size.Width)) + 0.5);

				//------------------------------------------------------
				// 画像変換処理
				//------------------------------------------------------
				ImageConvertor.ConvertToFile(
					imgSrc,
					iResizedWidth,
					iResizedHeight,
					ImageConvertor.SupplementType.None,
					ImageFormat.Jpeg,
					Constants.PRODUCTIMAGE_ENCODE_QUALITY,
					dest);
			}
		}
		catch
		{
			// なにもしない
			return false;
		}

		return true;
	}
}
