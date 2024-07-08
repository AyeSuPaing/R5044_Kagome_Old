/*
=========================================================================================================
  Module      : 特集画像管理ワーカーサービス(FeatureImageWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeatureImage;
using w2.Cms.Manager.ViewModels.FeatureImage;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.FeatureImage;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 特集画像管理ワーカーサービス
	/// </summary>
	public class FeatureImageWorkerService : BaseWorkerService
	{
		/// <summary>特集画像ルート（実際のアップロード先）</summary>
		public string _featureRoot;
		/// <summary>特集画像ページ検索ルート</summary>
		public string _featurePageImageSearchRoot;
		/// <summary>特集画像エリア検索ルート</summary>
		public string _featureAreaImageSearchRoot;
		/// <summary>特集画像ルート</summary>
		public string _featureImageRoot = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, Constants.PATH_FEATURE_IMAGE);
		/// <summary>特集タイプ画像ルート</summary>
		public string _featureTypeRoot = Path.Combine(Constants.PHYSICALDIRPATH_CMS_MANAGER, Constants.PATH_FEATUREAREA_ICON_IMAGE);
		/// <summary>特集ページ画像ルート</summary>
		public string _featurePageImageRoot = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, Constants.PATH_FEATUREPAGE_IMAGE);

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeatureImageWorkerService() : this(ImageType.Area)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="imageType">画像タイプ</param>
		/// <param name="cache">画像検索キャッシュ登録方法</param>
		public FeatureImageWorkerService(ImageType imageType, ImageSearchCache cache) : this(imageType)
		{
			switch (cache)
			{
				case ImageSearchCache.Register:
					SessionWrapper.FeatureImageList = GetFeatureImageList();
					SessionWrapper.FeatureImageGroupList = new FeatureImageService().GetAllGroup();
					break;

				case ImageSearchCache.Restore:
					SessionWrapper.FeatureImageList = SessionWrapper.FeatureImageList
						?? GetFeatureImageList();
					SessionWrapper.FeatureImageGroupList = SessionWrapper.FeatureImageGroupList
						?? new FeatureImageService().GetAllGroup();
					break;
			}
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="imageType">画像タイプ</param>
		public FeatureImageWorkerService(ImageType imageType)
		{
			this.ImageType = imageType;
			switch (imageType)
			{
				case ImageType.Normal:
					_featureRoot = _featureTypeRoot;
					break;

				case ImageType.Area:
					_featureRoot = _featureImageRoot;
					break;

				case ImageType.Page:
					_featureRoot = _featurePageImageRoot;
					break;

				case ImageType.Ogp:
					_featureRoot = _featureImageRoot;
					break;

				case ImageType.Search:
					_featurePageImageSearchRoot = _featurePageImageRoot;
					_featureAreaImageSearchRoot = _featureImageRoot;
					break;
			}
		}

		/// <summary>
		/// メインビューモデル作成
		/// </summary>
		/// <returns>ビューモデル</returns>
		public MainViewModel CreateMainVm()
		{
			var mainViewModel = new MainViewModel();
			return mainViewModel;
		}

		/// <summary>
		/// 特集画像ファイルパス一覧取得
		/// </summary>
		/// <returns>特集画像ファイルパス一覧</returns>
		public List<ImageViewModel> GetFeatureImageList()
		{
			var imageViewList = GetImageFilePathList();

			var service = new FeatureImageService();
			var imageAllModel = service.GetAllImage();

			var fileDirPath = string.Empty;
			switch (this.ImageType)
			{
				case ImageType.Normal:
					fileDirPath = Constants.PATH_FEATUREAREA_ICON_IMAGE;
					break;

				case ImageType.Area:
					fileDirPath = Constants.PATH_FEATURE_IMAGE;
					break;

				case ImageType.Page:
					fileDirPath = Constants.PATH_FEATUREPAGE_IMAGE;
					break;

				case ImageType.Ogp:
					fileDirPath = Constants.PATH_FEATURE_IMAGE;
					break;
			}

			foreach (var imageView in imageViewList)
			{
				var featureImage = imageAllModel.FirstOrDefault(
					image => (image.FileName == imageView.FileName)
						&& (image.FileDirPath == Path.GetDirectoryName(imageView.ImagePath).Replace('\\', '/') + "/"));

				if (featureImage != null)
				{
					imageView.ImageId = featureImage.ImageId;
					imageView.GroupId = featureImage.GroupId;
					imageView.Sort = featureImage.ImageSortNumber;
					imageView.ImagePath = featureImage.FileDirPath + HttpUtility.HtmlAttributeEncode(featureImage.FileName).Replace("+", "%20");
					imageView.RealFileName = featureImage.FileName;
					imageView.DateCreated = featureImage.DateCreated;
				}
				else
				{
					// 存在しなければ特集画像テーブルに登録
					var insertModel = new FeatureImageModel
					{
						FileName = imageView.FileName,
						FileDirPath = imageView.ImagePath
							.Substring(0, imageView.ImagePath.Length - imageView.FileName.Length)
							.Replace('\\', '/'),
						GroupId = 0,
						ImageSortNumber = 0,
						LastChanged = this.SessionWrapper.LoginOperatorName
					};

					imageView.ImageId = service.Insert(insertModel);
					imageView.ImagePath = imageView.ImagePath.Replace('\\', '/');
					imageView.RealFileName = imageView.FileName;
				}
			}

			return imageViewList;
		}

		/// <summary>
		/// グループビューモデル作成
		/// </summary>
		/// <param name="paramModel">パラメタモデル</param>
		/// <param name="featureImageFilePathList">特集画像リスト</param>
		/// <param name="featureImageGroupList">特集画像グループリスト</param>
		/// <returns>ビューモデル</returns>
		public GroupListViewModel CreateGroupListVm(
			FeatureImageListSearchParamModel paramModel = null,
			List<ImageViewModel> featureImageFilePathList = null,
			FeatureImageGroupModel[] featureImageGroupList = null)
		{
			featureImageFilePathList = featureImageFilePathList ?? GetFeatureImageList();
			featureImageGroupList = featureImageGroupList ?? new FeatureImageService().GetAllGroup();

			// 各グループに振り分け
			var groupListViewModel = new GroupListViewModel
			{
				GroupImageViewModel = featureImageGroupList.Where(g => CheckGroupId(g.GroupId, paramModel)).Select(
					g => new GroupImageModel(g)
					{
						ListImageViewModels = featureImageFilePathList
							.Where(l => (l.GroupId == g.GroupId))
							.Where(l => CheckKeyword(l, paramModel))
							.Where(l => CheckDateCreated(l, paramModel))
							.OrderBy(l => l.Sort).ToList(),
					}).ToList(),
				ImageType = this.ImageType
			};

			if (CheckGroupId(0, paramModel))
			{
				// 「その他」グループの設定
				var otherGroup = new GroupImageModel
				{
					GroupName = ValueText.GetValueText(Constants.TABLE_FEATUREIMAGEGROUP, Constants.FIELD_FEATUREIMAGEGROUP_GROUP_NAME, "0"),
					ListImageViewModels = featureImageFilePathList
						.Where(l => featureImageGroupList.Any(gl => (gl.GroupId == l.GroupId)) == false)
						.Where(l => CheckKeyword(l, paramModel))
						.Where(l => CheckDateCreated(l, paramModel))
						.OrderBy(l => l.Sort).ToList(),
					GroupSortNumber = 9999999,
					DateChanged = DateTime.Now,
					DateCreated = DateTime.Now,
				};

				// 最後尾に「その他」グループを配置
				groupListViewModel.GroupImageViewModel.Add(otherGroup);
			}

			return groupListViewModel;
		}

		/// <summary>
		/// 画像ファイルパス取得
		/// </summary>
		/// <returns>画像モデルリスト</returns>
		private List<ImageViewModel> GetImageFilePathList()
		{
			var list = new List<ImageViewModel>();

			if (string.IsNullOrEmpty(_featureRoot) == false)
			{
				if (Directory.Exists(_featureRoot) == false)
				{
					Directory.CreateDirectory(_featureRoot);
				}

				GetImageList(_featureRoot, list);
			}


			if ((string.IsNullOrEmpty(_featureRoot)
				&& (string.IsNullOrEmpty(_featurePageImageRoot) == false)))
			{
				if (Directory.Exists(_featurePageImageRoot) == false)
				{
					Directory.CreateDirectory(_featurePageImageRoot);
				}

				GetImageList(_featurePageImageRoot, list);
			}

			if ((string.IsNullOrEmpty(_featureRoot)
				&& (string.IsNullOrEmpty(_featureAreaImageSearchRoot) == false)))
			{
				if (Directory.Exists(_featureAreaImageSearchRoot) == false)
				{
					Directory.CreateDirectory(_featureAreaImageSearchRoot);
				}

				GetImageList(_featureAreaImageSearchRoot, list);
			}

			return list;
		}
		/// <summary>
		/// キーワードチェック
		/// </summary>
		/// <param name="l">画像</param>
		/// <param name="paramModel">パラメタモデル</param>
		/// <returns>キーワード検索対象か</returns>
		private bool CheckKeyword(ImageViewModel l, FeatureImageListSearchParamModel paramModel)
		{
			var checkPageKeyWord = (string.IsNullOrEmpty(paramModel.Keyword) || (l.FileName.Contains(paramModel.Keyword)));
			return checkPageKeyWord;
		}

		/// <summary>
		/// グループチェック
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="paramModel">パラメタモデル</param>
		/// <returns>グループ検索対象か</returns>
		private bool CheckGroupId(long groupId, FeatureImageListSearchParamModel paramModel)
		{
			var checkGroupId = (string.IsNullOrEmpty(paramModel.GroupId) || (groupId == long.Parse(paramModel.GroupId)));
			return checkGroupId;
		}

		/// <summary>
		/// アップロード日チェック
		/// </summary>
		/// <param name="l">画像</param>
		/// <param name="paramModel">パラメタモデル</param>
		/// <returns>アップロード日検索対象か</returns>
		private bool CheckDateCreated(ImageViewModel l, FeatureImageListSearchParamModel paramModel)
		{
			if (paramModel.DateCreatedKbn == Constants.DateSelectType.Unselected) return true;

			var compareDateTime = DateTime.Now;
			switch (paramModel.DateCreatedKbn)
			{
				case Constants.DateSelectType.Day:
					compareDateTime = DateTime.Now.AddDays(-1);
					break;

				case Constants.DateSelectType.Week:
					compareDateTime = DateTime.Now.AddDays(-7);
					break;

				case Constants.DateSelectType.Month:
					compareDateTime = DateTime.Now.AddMonths(-1);
					break;

				case Constants.DateSelectType.ThreeMonth:
				case Constants.DateSelectType.AfterThreeMonth:
					compareDateTime = DateTime.Now.AddMonths(-3);
					break;
			}

			var checkCreatedDate = ((paramModel.DateCreatedKbn != Constants.DateSelectType.AfterThreeMonth)
				? ((l.DateCreated > compareDateTime) || (l.DateCreated > compareDateTime))
				: ((l.DateCreated < compareDateTime) && (l.DateCreated < compareDateTime)));
			return checkCreatedDate;
		}

		/// <summary>
		/// グループ順更新
		/// </summary>
		/// <param name="groupIds">グループ順序配列</param>
		public void GroupSortUpdate(long[] groupIds)
		{
			new FeatureImageService().UpdateGroupSort(groupIds);
		}

		/// <summary>
		/// 画像順更新
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="imagePath">画像順序配列</param>
		public void ImageSortUpdate(long groupId, string[] imagePath)
		{
			if (imagePath == null) return;

			new FeatureImageService().InsertUpdateImageSort(groupId, imagePath);
		}

		/// <summary>
		/// 再帰的に画像取得
		/// </summary>
		/// <param name="featureRoot">特集画像ルート</param>
		/// <param name="list">画像モデルリスト</param>
		/// <returns>画像モデルリスト</returns>
		public List<ImageViewModel> GetImageList(string featureRoot, List<ImageViewModel> list)
		{
			foreach (var filePath in Directory.GetFiles(featureRoot))
			{
				var file = new FileInfo(filePath);
				var model = new ImageViewModel
				{
					ImagePath = filePath.Substring(
						(this.ImageType != ImageType.Normal)
							? Constants.PHYSICALDIRPATH_CONTENTS_ROOT.Length
							: HttpContext.Current.Server.MapPath("~/").Length),
					DataChanged = DateTimeUtility.ToStringForManager(
						file.LastWriteTime,
						DateTimeUtility.FormatType.LongDateHourMinuteSecond1Letter),
					FileSize = ConvertFileSize(file.Length),
				};

				switch (Path.GetExtension(filePath).ToLower())
				{
					case ".jpg":
					case ".jpeg":
					case ".png":
					case ".gif":
					case ".bmp":
					case ".tiff":
						try
						{
							using (var img = Image.FromFile(filePath))
							{
								model.ImageSize = img.Width + " x " + img.Height + "px";
							}
						}
						catch (Exception)
						{
							model.ImageSize = "-";
						}
						break;
					case ".svg":
						model.ImageSize = "-";
						break;
				}

				list.Add(model);
			}

			foreach (var dir in Directory.GetDirectories(featureRoot))
			{
				GetImageList(dir + @"\", list);
			}
			return list;
		}

		/// <summary>
		/// ファイルサイズ文字列変換
		/// </summary>
		/// <param name="b">バイト数</param>
		/// <returns>変換後文字列</returns>
		private string ConvertFileSize(long b)
		{
			if (b < 1024)
			{
				return "1KB";
			}

			if (b <= 1048576) return (b / 1000) + "KB";
			var mb = b / 1048576f;
			return mb.ToString("F1") + "MB";
		}

		/// <summary>
		/// グループ追加
		/// </summary>
		/// <param name="input">グループ入力内容</param>
		public void GroupAdd(FeatureImageGroupInput input)
		{
			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;
			new FeatureImageService().InsertGroup(model);
		}

		/// <summary>
		/// 特集画像アップロード
		/// </summary>
		/// <param name="featureImage">画像ファイル</param>
		/// <param name="groupId">グループID</param>
		public string Upload(HttpPostedFileBase featureImage, string groupId)
		{
			if (featureImage == null) return string.Empty;

			try
			{
				var fileName = featureImage.FileName.Contains(@"\")
					? new Regex(@"([^\\]+?)?$").Match(featureImage.FileName).Value
					: featureImage.FileName;

				var uploadPath = Path.Combine(
					_featureRoot,
					fileName);
				uploadPath = CheckAndChangeUploadPath(uploadPath);
				featureImage.SaveAs(uploadPath);

				fileName = Path.GetFileName(uploadPath);
				var fileDirPath = string.Empty;
				switch (this.ImageType)
				{
					case ImageType.Normal:
						fileDirPath = Constants.PATH_FEATUREAREA_ICON_IMAGE;
						break;

					case ImageType.Area:
						fileDirPath = Constants.PATH_FEATURE_IMAGE;
						break;

					case ImageType.Page:
						fileDirPath = Constants.PATH_FEATUREPAGE_IMAGE;
						break;
				}

				new FeatureImageService().UploadFeatureImage(
					fileName,
					fileDirPath,
					groupId,
					this.SessionWrapper.LoginOperatorName);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}

		/// <summary>
		/// アップロードパスを確認して変更する
		/// </summary>
		/// <param name="uploadPath">アップロードパス</param>
		/// <returns>アップロードパス</returns>
		protected string CheckAndChangeUploadPath(string uploadPath)
		{
			var extension = Path.GetExtension(uploadPath);
			var fileNameWithOutExtension = Path.GetFileNameWithoutExtension(uploadPath);
			var count = 2;
			while (File.Exists(uploadPath))
			{
				uploadPath = Path.Combine(
					_featureRoot,
					string.Format(fileNameWithOutExtension + "({0})" + extension, count));
				count++;
			}

			return uploadPath;
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="keyword">キーワード</param>
		/// <returns>ビューモデル</returns>
		public GroupListViewModel Search(string groupId, string keyword)
		{
			var vm = CreateGroupListVm(
				new FeatureImageListSearchParamModel
				{
					GroupId = groupId,
					Keyword = keyword
				});
			return vm;
		}

		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		public void DeleteGroup(long groupId)
		{
			new FeatureImageService().DeleteGroup(groupId);
		}

		/// <summary>
		/// グループ名変更
		/// </summary>
		/// <param name="input">グループ入力内容</param>
		public void GroupNameEdit(FeatureImageGroupInput input)
		{
			var service = new FeatureImageService();
			var model = service.GetGroup(input.GroupId);

			if (model == null) return;

			model.GroupName = input.GroupName;

			service.Update(model);
		}

		/// <summary>
		/// 画像削除
		/// </summary>
		/// <param name="imageId">画像ID</param>
		public void ImageDelete(string imageId)
		{
			var service = new FeatureImageService();
			var model = service.Get(long.Parse(imageId));

			var deleteFilePath = Constants.PHYSICALDIRPATH_CONTENTS_ROOT + model.FileDirPath + model.FileName;

			// ファイル削除
			File.Delete(deleteFilePath);

			service.Delete(model.ImageId);
		}

		/// <summary>
		/// 画像名変更
		/// </summary>
		/// <param name="fileName">更新後ファイル名</param>
		/// <param name="imageId">画像ID</param>
		/// <returns>エラーメッセージ</returns>
		public string ImageNameEdit(string fileName, string imageId)
		{
			var service = new FeatureImageService();
			var model = service.Get(long.Parse(imageId));

			if (model == null) return string.Empty;

			var errorMessage = CheckFileName(fileName);
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			try
			{
				File.Move(
					Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, model.FileDirPath, model.FileName),
					Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, model.FileDirPath, fileName));

				model.FileName = fileName;

				service.Update(model);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}

		/// <summary>
		/// ファイル存在チェック
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>true:あり false:なし</returns>
		public bool CheckFileExist(string fileName)
		{
			var isExist = File.Exists(Path.Combine(_featureRoot, fileName));
			return isExist;
		}

		/// <summary>
		/// 特集エリア画像か
		/// </summary>
		public ImageType ImageType { get; set; }
	}
}
