/*
=========================================================================================================
  Module      : 共通画像選択入力クラス(ImageInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using w2.App.Common;

namespace w2.Cms.Manager.Input
{
	/// <summary>画像タイプ</summary>
	public enum ImageType
	{
		/// <summary>通常</summary>
		Normal,
		/// <summary>特集エリア情報</summary>
		Area,
		/// <summary>特集ページ情報</summary>
		Page,
		/// <summary>OGP画像</summary>
		Ogp,
		/// <summary>画像検索</summary>
		Search
	}

	/// <summary>
	/// 画像選択入力クラス
	/// </summary>
	public class ImageInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImageInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImageInput(ImageType imageType)
		{
			this.ImageType = imageType;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="imageType">商品画像タイプ</param>
		public ImageInput(string baseName, ImageType imageType)
			: this(imageType)
		{
			this.BaseName = baseName;
		}

		/// <summary>アップロードファイル</summary>
		public HttpPostedFileBase UploadFile { get; set; }
		/// <summary>画像ID</summary>
		public long ImageId { get; set; }
		/// <summary>バインド時の名前</summary>
		public string BaseName { get; set; }
		/// <summary>画像タイプ</summary>
		public bool IsFeaturePage { get; set; }
		/// <summary>初期表示用ファイル名</summary>
		public string FileName { get; set; }
		/// <summary>表示用ファイル名</summary>
		public string RealFileName { get; set; }
		/// <summary>ファイルディレクトリ</summary>
		public string FileDir
		{
			get
			{
				var result = string.Empty;
				switch (this.ImageType)
				{
					case ImageType.Normal:
						result = Constants.PATH_ROOT_CMS + Constants.PATH_FEATUREAREA_ICON_IMAGE;
						break;

					case ImageType.Area:
						result = Constants.PATH_ROOT_FRONT_PC + Constants.PATH_FEATURE_IMAGE;
						break;

					case ImageType.Page:
						result = Constants.PATH_ROOT_FRONT_PC + Constants.PATH_FEATUREPAGE_IMAGE;
						break;

					case ImageType.Ogp:
						result = Constants.PATH_ROOT_FRONT_PC + Constants.PATH_FEATURE_IMAGE;
						break;
				}
				return result;
			}
		}
		/// <summary>プレビュー時バイナリデータ</summary>
		public string PreviewBinary { get; set; }
		/// <summary>削除か</summary>
		public bool IsRemove { get; set; }
		/// <summary>商品画像タイプ</summary>
		public ImageType ImageType { get; set; }
	}
}
