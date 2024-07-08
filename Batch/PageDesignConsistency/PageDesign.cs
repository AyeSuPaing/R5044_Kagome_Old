/*
=========================================================================================================
  Module      : ページ管理 実ファイルとDBの整合性調整 (PageDesign.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Design;
using w2.Domain.PageDesign;

namespace w2.Commerce.Batch.PageDesignConsistency
{
	/// <summary>
	/// ページ管理 実ファイルとDBの整合性調整
	/// </summary>
	public class PageDesign
	{
		/// <summary>
		/// ページ管理 ページ整合性調整
		/// </summary>
		public void PageConsistency()
		{
			// カスタムページ取得
			this.PcRealPageList = PageDesignCommon.GetCustomPageList(DesignCommon.DeviceType.Pc);
			this.SpRealPageList = (DesignCommon.UseSmartPhone)
				? PageDesignCommon.GetCustomPageList(DesignCommon.DeviceType.Sp)
				: new List<RealPage>();

			// 標準ページの取得
			this.PcRealPageList.AddRange(PageDesignCommon.GetStandardPageSetting(DesignCommon.DeviceType.Pc));
			this.SpRealPageList.AddRange(PageDesignCommon.GetStandardPageSetting(DesignCommon.DeviceType.Sp));

			// HTMLページの取得
			this.PcRealPageList.AddRange(PageDesignCommon.GetHtmlPageList(DesignCommon.DeviceType.Pc));

			// 全ページリス
			var pageAllList = this.PcRealPageList.Select(
				p => new PagePairingModel
				{
					PcRealPage = p
				}).ToList();
			// SPページの設定
			foreach (var spList in this.SpRealPageList)
			{
				// PCに存在する場合はペアリング
				// 存在しない場合はPCページは存在しないものとして全ページリストへ登録
				if (pageAllList.Any(p => (p.PcRealPage != null) && (p.PcRealPage.FileName == spList.FileName)))
				{
					var val = pageAllList.FirstOrDefault(p => p.PcRealPage.FileName == spList.FileName);
					if (val == null) continue;

					var index = pageAllList.IndexOf(val);
					pageAllList.RemoveAt(index);
					val.SpRealPage = spList;
					pageAllList.Insert(index, val);
				}
				else
				{
					pageAllList.Add(
						new PagePairingModel
						{
							SpRealPage = spList,
							PcRealPage = new RealPage(
								spList.PageTitle,
								DesignCommon.PhysicalDirPathTargetSitePc,
								spList.PageDirPath,
								spList.FileName,
								spList.PageType)
							{
								LastChange = spList.LastChange
							}
						});
				}
			}

			// DBよりページ情報取得
			// 実ファイルに存在する場合はグループ・順序を設定
			var pageAllModel = new PageDesignService().GetAllPage();
			foreach (var pagePairingModel in pageAllList)
			{
				var model = pageAllModel.FirstOrDefault(
					pm => (pm.FileDirPath == pagePairingModel.PcRealPage.PageDirPath)
						&& (pm.FileName == pagePairingModel.PcRealPage.FileName));

				var tempUseType =
					((pagePairingModel.PcRealPage.Existence == RealPage.ExistStatus.Exist)
						&& (pagePairingModel.SpRealPage.Existence == RealPage.ExistStatus.Exist))
						?
						Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP
						: ((pagePairingModel.PcRealPage.Existence == RealPage.ExistStatus.NotExist)
							&& (pagePairingModel.SpRealPage.Existence == RealPage.ExistStatus.Exist))
							? Constants.FLG_PAGEDESIGN_USE_TYPE_SP
							: Constants.FLG_PAGEDESIGN_USE_TYPE_PC;

				if (model != null) continue;

				var managementTitle = "";
				if (pagePairingModel.PcRealPage.PageType != Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM)
				{
					managementTitle = (tempUseType == Constants.FLG_PAGEDESIGN_USE_TYPE_SP)
						? pagePairingModel.SpRealPage.PageTitle
						: pagePairingModel.PcRealPage.PageTitle;
				}
				else
				{
					managementTitle = (tempUseType == Constants.FLG_PAGEDESIGN_USE_TYPE_SP)
						? DesignCommon.GetAspxTitle(
							DesignCommon.GetFileTextAll(pagePairingModel.SpRealPage.PhysicalFullPath))
						: DesignCommon.GetAspxTitle(
							DesignCommon.GetFileTextAll(pagePairingModel.PcRealPage.PhysicalFullPath));
				}

				// 存在しなければページテーブルに登録
				var insertModel = new PageDesignModel
				{
					FileName = pagePairingModel.PcRealPage.FileName,
					FileDirPath = pagePairingModel.PcRealPage.PageDirPath,
					PageType = pagePairingModel.PcRealPage.PageType,
					// 実ファイルの存在状況より各デバイスページの利用状況を確定
					UseType = tempUseType,
					// 管理用タイトルは各ページのタイトルから引用
					ManagementTitle = managementTitle,
					LastChanged = Constants.FLG_LASTCHANGED_BATCH
				};
				new PageDesignService().InsertPage(insertModel);
			}
		}

		/// <summary>
		/// ページ管理 ページグループ整合性調整
		/// </summary>
		public void GroupConsistency()
		{
			var service = new PageDesignService();
			var groupIds = service.NotExistGroupIds();
			foreach (var groupId in groupIds)
			{
				service.UpdatePageMoveOtherGroup(groupId, Constants.FLG_LASTCHANGED_BATCH);
			}
		}

		/// <summary>PC 実ファイルリスト</summary>
		private List<RealPage> PcRealPageList { get; set; }
		/// <summary>PC 実ファイルリスト</summary>
		private List<RealPage> SpRealPageList { get; set; }
	}

	/// <summary>
	/// ページ ペアリングクラス
	/// </summary>
	public class PagePairingModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PagePairingModel()
		{
			this.PcRealPage = new RealPage();
			this.SpRealPage = new RealPage();
		}

		/// <summary>PC 実ファイル</summary>
		public RealPage SpRealPage { get; set; }
		/// <summary>PC 実ファイル</summary>
		public RealPage PcRealPage { get; set; }
	}
}