/*
=========================================================================================================
 Module      : OGPタグ設定ワーカーサービス(OspTagSettingWorkerService.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Web;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.OgpTagSetting;
using w2.Common.Logger;
using w2.Database.Common;
using w2.Domain.FeatureImage;
using w2.Domain.OgpTagSetting;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// OGPタグ設定ワーカーサービス
	/// </summary>
	public class OgpTagSettingWorkerService : BaseWorkerService
	{
		/// <summary>フロントのルート物理パス</summary>
		private static readonly string m_frontRootPhysicalPath
			= HttpContext.Current.Server.MapPath(Constants.PATH_ROOT_FRONT_PC);
		/// <summary>フロントのルートURL</summary>
		private static readonly string m_frontRootUrl
			= (Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC);

		/// <summary>
		/// 編集用ビューモデル作成
		/// </summary>
		/// <returns>ビューモデル</returns>
		public ModifyViewModel CreateViewModelForModify()
		{
			var defaultSettingModel = new OgpTagSettingService()
				.Get(Constants.FLG_OGPTAGSETTING_DATA_KBN_DEFAULT_SETTING);
			var vm = new ModifyViewModel
			{
				InputForDefaultSetting = new OgpTagSettingInput(defaultSettingModel)
			};

			vm.InputForDefaultSetting.ImageInput = new ImageInput("InputForDefaultSetting.ImageInput", ImageType.Ogp)
			{
				FileName = vm.InputForDefaultSetting.ImageUrl
					.Replace(
						(m_frontRootUrl + Codes.Constants.PATH_FEATURE_IMAGE),
						string.Empty)
			};

			//本ファイル名を保存する
			vm.InputForDefaultSetting.ImageInput.RealFileName = vm.InputForDefaultSetting.ImageInput.FileName;
			//ファイル名をUrlEncodeする
			vm.InputForDefaultSetting.ImageInput.FileName = HttpUtility.UrlEncode(vm.InputForDefaultSetting.ImageInput.FileName);

			return vm;
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="inputForDefaultSetting">全体設定インプット</param>
		/// <param name="imageUrl">画像URL</param>
		/// <returns>エラーメッセージ</returns>
		public string Update(OgpTagSettingInput inputForDefaultSetting, string imageUrl)
		{
			inputForDefaultSetting.DataKbn = Constants.FLG_OGPTAGSETTING_DATA_KBN_DEFAULT_SETTING;
			inputForDefaultSetting.LastChanged = this.SessionWrapper.LoginOperatorName;

			// 入力チェック
			var errorMessage = inputForDefaultSetting.Validate();
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return errorMessage;
			}

			inputForDefaultSetting.ImageUrl = (inputForDefaultSetting.ImageInput.ImageId == 0)
				? imageUrl
				: GetImageUrl(inputForDefaultSetting.ImageInput.ImageId);

			if (inputForDefaultSetting.ImageInput.IsRemove)
			{
				inputForDefaultSetting.ImageUrl = string.Empty;
			}

			// アップサート
			new OgpTagSettingService().Upsert(inputForDefaultSetting.CreateModel());

			// キャッシュ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.OgpTagSetting).CreateUpdateRefreshFile();

			return string.Empty;
		}

		/// <summary>
		/// ファイルアップロード
		/// </summary>
		/// <param name="file">ファイル</param>
		/// <param name="url">アップロード済URL</param>
		/// <returns>アップロードが成功したか</returns>
		public bool Upload(HttpPostedFileBase file, out string url)
		{
			url = string.Empty;
			if (file == null)
			{
				return true;
			}

			// フォルダ無ければ作る
			if (Directory.Exists(this.ImageDirectoryPath) == false)
			{
				Directory.CreateDirectory(this.ImageDirectoryPath);
			}

			try
			{
				file.SaveAs(Path.Combine(this.ImageDirectoryPath, file.FileName));
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return false;
			}

			// URLにして返す
			url = Constants.PROTOCOL_HTTPS
				+ Constants.SITE_DOMAIN
				+ Constants.PATH_ROOT_FRONT_PC
				+ Codes.Constants.PATH_FEATURE_IMAGE
				+ file.FileName;
			return true;
		}

		/// <summary>
		/// 画僧IDからURL取得
		/// </summary>
		/// <param name="imageId">画像ID</param>
		/// <returns>URL</returns>
		private static string GetImageUrl(long imageId)
		{
			var imageModel = new FeatureImageService().Get(imageId);
			if (imageModel == null) return string.Empty;

			var url = Constants.PROTOCOL_HTTPS
				+ Constants.SITE_DOMAIN
				+ Constants.PATH_ROOT_FRONT_PC
				+ imageModel.FileDirPath
				+ imageModel.FileName;
			url = url.Replace(@"\", "/");

			return url;
		}

		/// <summary>画像ディレクトリパス名</summary>
		private string ImageDirectoryPath
		{
			get
			{
				return Path.Combine(
					m_frontRootPhysicalPath,
					Codes.Constants.PATH_FEATURE_IMAGE).Replace("/", @"\");
			}
		}
	}
}