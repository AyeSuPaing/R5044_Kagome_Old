/*
=========================================================================================================
  Module      : マスタインポートワーカーサービス(MasterImportWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ViewModels.MasterImport;
using w2.Common.Logger;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// マスタインポートワーカーサービス
	/// </summary>
	public class MasterImportWorkerService : BaseWorkerService
	{
		/// <summary>店舗ディレクトリ</summary>
		private static string m_shopDirectory = null;

		/// <summary>マスタアップロードディレクトリ</summary>
		private static string m_masterUploadDirectory = null;

		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterType">マスタ種別</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(string shopId, string masterType)
		{
			// ショップのディレクトリ決定
			m_shopDirectory = Path.Combine(Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR, shopId);

			var vm = new ListViewModel();

			// 選択中マスタでのファイルの実アップロードディレクトリ決定（ドロップダウンデフォルト選択処理の後におく必要がある）
			m_masterUploadDirectory = Path.Combine(
				m_shopDirectory,
				string.IsNullOrEmpty(masterType) ? vm.MasterItems[0].Value : masterType,
				Constants.DIRNAME_MASTERIMPORT_UPLOAD);

			// 存在するファイル名取得＆各種表示設定
			if (Directory.Exists(m_masterUploadDirectory))
			{
				vm.UploadFiles = Directory.GetFiles(m_masterUploadDirectory);
				if (vm.UploadFiles.Length != 0)
				{
					vm.UploadFileNames = new string[vm.UploadFiles.Length];
					for (var count = 0; count < vm.UploadFiles.Length; count++)
					{
						vm.UploadFileNames[count] = Path.GetFileName(vm.UploadFiles[count]);
					}

					// アップロード不能に
					vm.UploadEnabled = false;
				}
				else
				{
					// アップロード可能に
					vm.UploadEnabled = true;
				}
			}
			else
			{
				// アップロード可能に
				vm.UploadEnabled = true;
			}
			return vm;
		}

		/// <summary>
		/// アップロード
		/// </summary>
		/// <param name="uploadFile">アップロードファイル</param>
		/// <returns>ビューモデル</returns>
		public string Upload(HttpPostedFileWrapper uploadFile)
		{
			// ファイル指定あり？
			if (uploadFile == null)
			{
				return WebMessages.MasterUploadFileUnselected;
			}

			// ディレクトリが存在しなければ作成
			if (Directory.Exists(m_masterUploadDirectory) == false)
			{
				Directory.CreateDirectory(m_masterUploadDirectory);
			}

			// CSVファイルじゃなければエラー
			if (uploadFile.FileName.EndsWith(".csv") == false)
			{
				return WebMessages.MasterUploadFileNotCsv;
			}

			// ファイル存在チェック
			var filePath = Path.Combine(m_masterUploadDirectory, Path.GetFileName(uploadFile.FileName));
			if (File.Exists(filePath))
			{
				// ファイルが既に存在していたらエラーページへ
				return WebMessages.MasterUploadAlreadyExists;
			}
			if (uploadFile.InputStream.Length == 0)
			{
				// ファイルなしエラー
				return WebMessages.MasterUploadFileUnfind;
			}

			// ファイルアップロード実行
			try
			{
				uploadFile.SaveAs(filePath);
				return "";
			}
			catch (System.UnauthorizedAccessException ex)
			{
				// ファイルアップロード権限エラー（ログにも記録）
				AppLogger.WriteError(ex);
				return WebMessages.MasterUploadUnloadError;
			}
		}

		/// <summary>
		/// ファイル取込
		/// </summary>
		/// <param name="fileName">ファイルの名前</param>
		/// <param name="masterType">マスタ種別</param>
		public void ImportFile(string fileName, string masterType)
		{
			// 処理ファイルディレクトリ作成
			var activeDirectory = Path.Combine(m_shopDirectory, masterType, Constants.DIRNAME_MASTERIMPORT_ACTIVE);
			if (Directory.Exists(activeDirectory) == false)
			{
				Directory.CreateDirectory(activeDirectory);
			}

			// 処理ファイルパス
			var uploadFilePath = Path.Combine(m_masterUploadDirectory, fileName);
			var activeFilePath = Path.Combine(activeDirectory, fileName);

			if (File.Exists(uploadFilePath))
			{
				// ファイル移動（プロセス生成に時間がかかることがあるため、移動後のファイルをバッチへ渡す。）
				File.Move(uploadFilePath, activeFilePath);

				// プロセス実行（移動後ファイルのフルパスを引数として渡す。）
				Process.Start(Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE, "\"" + activeFilePath + "\"");	// スペースが含まれても処理されるように「"」)でくくる
			}
		}

		/// <summary>
		/// ファイル削除
		/// </summary>
		/// <param name="fileName">ファイルの名前</param>
		public void DeleteFile(string fileName)
		{
			// 処理ファイルパス
			var uploadFilePath = Path.Combine(m_masterUploadDirectory, fileName);

			if (File.Exists(uploadFilePath))
			{
				File.Delete(uploadFilePath);
			}
		}

		/// <summary>
		/// マスター種別をXMLから読み込んで返却する
		/// </summary>
		/// <param name="xmlPath">xmlパス</param>
		/// <returns>マスタ種別リスト</returns>
		public List<SelectListItem> GetMasterItemFromXml(string xmlPath)
		{
			var xdoc = XDocument.Load(xmlPath);

			var infoList = xdoc.Descendants("Master")
				.Select(xe => new SelectListItem{ Text = xe.Element("Name").Value, Value =xe.Element("Directory").Value});
			var result = new List<SelectListItem>();
			foreach (var info in infoList)
			{
				// マスタの種別によって判断する
				switch (info.Value)
				{
					default:
						result.Add(new SelectListItem { Text = info.Text, Value = info.Value });
						break;
				}
			}
			return result;
		}
	}
}