/*
=========================================================================================================
  Module      : Upload Image Input (UploadImageInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.App.Common.Input;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.ProductSubImageSetting;
using w2.Domain.ShopOperator;

namespace w2.App.Common.Input
{
	/// <summary>
	/// Upload image input
	/// </summary>
	[Serializable]
	public class UploadImageInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UploadImageInput()
		{
			this.ProductSubImageSettings
				= DomainFacade.Instance.ProductSubImageSettingService.GetProductSubImageSettings(
					((ShopOperatorModel)SessionManager.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]).ShopId,
					Constants.PRODUCTSUBIMAGE_MAXCOUNT)
					?? new ProductSubImageSettingModel[0];
		}

		/// <summary>
		/// Get variation image
		/// </summary>
		/// <param name="variationImageHead">Variation image head</param>
		/// <returns>Variation image</returns>
		public UploadProductVariationImageInput GetVariationImage(string variationImageHead)
		{
			return this.VariationImages.FirstOrDefault(image => (image.ImageHead == variationImageHead));
		}

		/// <summary>
		/// Get sub image
		/// </summary>
		/// <param name="imageNo">Image no</param>
		/// <returns>Sub image</returns>
		public UploadProductSubImageInput GetSubImage(int imageNo)
		{
			return this.SubImages.FirstOrDefault(image => (image.ImageNo == imageNo));
		}

		/// <summary>
		/// Initialize image size settings
		/// </summary>
		/// <param name="isSubImage">Is Sub-image</param>
		private void InitializeImageSizeSettings(bool isSubImage)
		{
			var imageSizeS = new List<string>(
				isSubImage
					? new[] { "0", "0" }
					: Constants.PRODUCTIMAGE_SIZE_S.Split(','));
			var imageSizeM = new List<string>(
				isSubImage
					? Constants.PRODUCTSUBIMAGE_SIZE_M.Split(',')
					: Constants.PRODUCTIMAGE_SIZE_M.Split(','));
			var imageSizeL = new List<string>(
				isSubImage
					? Constants.PRODUCTSUBIMAGE_SIZE_L.Split(',')
					: Constants.PRODUCTIMAGE_SIZE_L.Split(','));
			var imageSizeLL = new List<string>(
				isSubImage
					? Constants.PRODUCTSUBIMAGE_SIZE_LL.Split(',')
					: Constants.PRODUCTIMAGE_SIZE_LL.Split(','));

			// 横幅設定
			this.ImageSizeWidthS = imageSizeS[0].Trim();
			this.ImageSizeWidthM = imageSizeM[0].Trim();
			this.ImageSizeWidthL = imageSizeL[0].Trim();
			this.ImageSizeWidthLL = imageSizeLL[0].Trim();

			// 縦幅設定
			this.ImageSizeHeightS = (imageSizeS.Count == 1) ? imageSizeS[0].Trim() : imageSizeS[1].Trim();
			this.ImageSizeHeightM = (imageSizeM.Count == 1) ? imageSizeM[0].Trim() : imageSizeM[1].Trim();
			this.ImageSizeHeightL = (imageSizeL.Count == 1) ? imageSizeL[0].Trim() : imageSizeL[1].Trim();
			this.ImageSizeHeightLL = (imageSizeLL.Count == 1) ? imageSizeLL[0].Trim() : imageSizeLL[1].Trim();
		}

		/// <summary>
		/// Resize image
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <param name="isSubImage">Is sub image</param>
		public void ResizeImage(string filePath, bool isSubImage = false)
		{
			InitializeImageSizeSettings(isSubImage);

			if (isSubImage == false) ResizeImageToSizeS(filePath);

			ResizeImageToSizeM(filePath);
			ResizeImageToSizeL(filePath);
			ResizeImageToSizeLL(filePath);
		}

		/// <summary>
		/// Resize image to size S
		/// </summary>
		/// <param name="filePath">File path</param>
		private void ResizeImageToSizeS(string filePath)
		{
			ImageConvert.ResizeImage(
				filePath,
				filePath.Replace(Constants.PRODUCTIMAGE_FOOTER_LL, Constants.PRODUCTIMAGE_FOOTER_S),
				ObjectUtility.TryParseInt(this.ImageSizeWidthS),
				ObjectUtility.TryParseInt(this.ImageSizeHeightS));
		}

		/// <summary>
		/// Resize image to size M
		/// </summary>
		/// <param name="filePath">File path</param>
		private void ResizeImageToSizeM(string filePath)
		{
			ImageConvert.ResizeImage(
				filePath,
				filePath.Replace(Constants.PRODUCTIMAGE_FOOTER_LL, Constants.PRODUCTIMAGE_FOOTER_M),
				ObjectUtility.TryParseInt(this.ImageSizeWidthM),
				ObjectUtility.TryParseInt(this.ImageSizeHeightM));
		}

		/// <summary>
		/// Resize image to size L
		/// </summary>
		/// <param name="filePath">File path</param>
		private void ResizeImageToSizeL(string filePath)
		{
			ImageConvert.ResizeImage(
				filePath,
				filePath.Replace(Constants.PRODUCTIMAGE_FOOTER_LL, Constants.PRODUCTIMAGE_FOOTER_L),
				ObjectUtility.TryParseInt(this.ImageSizeWidthL),
				ObjectUtility.TryParseInt(this.ImageSizeHeightL));
		}

		/// <summary>
		/// Resize image to size LL
		/// </summary>
		/// <param name="filePath">File path</param>
		private void ResizeImageToSizeLL(string filePath)
		{
			// Rename the original image to a temporary image, this procedure use to resize the image.
			var filePathBase = filePath.Substring(0, filePath.LastIndexOf('.'));
			var tempFilePath = filePathBase + "temp_" + Constants.PRODUCTIMAGE_FOOTER_LL;
			File.Copy(filePath, tempFilePath, true);

			ImageConvert.ResizeImage(
				tempFilePath,
				filePath,
				ObjectUtility.TryParseInt(this.ImageSizeWidthLL),
				ObjectUtility.TryParseInt(this.ImageSizeHeightLL));

			File.Delete(tempFilePath);
		}

		/// <summary>Main image</summary>
		public UploadProductImageInput MainImage { get; set; }
		/// <summary>Sub images</summary>
		public UploadProductSubImageInput[] SubImages { get; set; }
		/// <summary>Variation images</summary>
		public UploadProductVariationImageInput[] VariationImages { get; set; }
		/// <summary>Image head</summary>
		public string ImageHead { get; set; }
		/// <summary>PCサイト商品画像自動リサイズ</summary>
		public bool AutoResize { get; set; }
		/// <summary>PCサイト商品画像自動リサイズS横幅</summary>
		public string ImageSizeWidthS { get; set; }
		/// <summary>PCサイト商品画像自動リサイズM横幅</summary>
		public string ImageSizeWidthM { get; set; }
		/// <summary>PCサイト商品画像自動リサイズL横幅</summary>
		public string ImageSizeWidthL { get; set; }
		/// <summary>PCサイト商品画像自動リサイズLL横幅</summary>
		public string ImageSizeWidthLL { get; set; }
		/// <summary>PCサイト商品画像自動リサイズS縦</summary>
		public string ImageSizeHeightS { get; set; }
		/// <summary>PCサイト商品画像自動リサイズM縦</summary>
		public string ImageSizeHeightM { get; set; }
		/// <summary>PCサイト商品画像自動リサイズL縦</summary>
		public string ImageSizeHeightL { get; set; }
		/// <summary>PCサイト商品画像自動リサイズLL縦</summary>
		public string ImageSizeHeightLL { get; set; }
		/// <summary>Guid</summary>
		public string Guid { get; set; }
		/// <summary>File index</summary>
		public int FileIndex { get; set; }
		/// <summary>Del flag</summary>
		public bool DelFlg { get; set; }
		/// <summary>Absolute path</summary>
		public string FileName { get; set; }
		/// <summary>Is new file</summary>
		public bool IsNewFile
		{
			get { return (this.FileIndex != -1); }
		}
		/// <summary>Has image</summary>
		public bool HasImage
		{
			get { return (string.IsNullOrEmpty(this.FileName) == false); }
		}
		/// <summary>商品サブ画像設定マスタモデル一覧</summary>
		public ProductSubImageSettingModel[] ProductSubImageSettings { get; private set; }
	}
}

/// <summary>
/// Upload product image input
/// </summary>
[Serializable]
public class UploadProductImageInput : UploadImageInput
{
	/// <summary>
	/// Constructor
	/// </summary>
	public UploadProductImageInput()
	{
		this.FileName = string.Empty;
	}

	/// <summary>Image no</summary>
	public int ImageNo { get; set; }
	/// <summary>Source index</summary>
	public int SourceIndex { get; set; }
}

/// <summary>
/// Upload product image input
/// </summary>
[Serializable]
public class UploadProductSubImageInput : UploadImageInput
{
	/// <summary>
	/// Constructor
	/// </summary>
	public UploadProductSubImageInput()
	{
		this.FileName = string.Empty;
	}

	/// <summary>画像番号</summary>
	public int ImageNo { get; set; }
	/// <summary>Source index</summary>
	public int SourceIndex { get; set; }
}

/// <summary>
/// Upload product variation image input
/// </summary>
[Serializable]
public class UploadProductVariationImageInput : UploadImageInput
{
	/// <summary>
	/// Constructor
	/// </summary>
	public UploadProductVariationImageInput()
	{
		this.FileName = string.Empty;
	}

	/// <summary>Reference variation ID</summary>
	public string RefVariationId { get; set; }
}
