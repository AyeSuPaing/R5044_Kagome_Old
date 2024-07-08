/*
=========================================================================================================
  Module      : パーツ管理 実ファイルとDBの整合性調整 (PageDesign.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using w2.App.Common;
using w2.App.Common.Design;
using w2.Domain.PartsDesign;

namespace w2.Commerce.Batch.PageDesignConsistency
{
	/// <summary>
	/// パーツ管理 実ファイルとDBの整合性調整
	/// </summary>
	public class PartsDesign
	{
		/// <summary>
		/// パーツ管理 パーツ整合性調整
		/// </summary>
		public void PartsConsistency()
		{
			PartsDesignCommon.CreatePartsDirectory();

			// カスタムパーツ取得
			this.PcRealPartsList = PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Pc);
			this.SpRealPartsList = (DesignCommon.UseSmartPhone)
				? PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Sp)
				: new List<RealParts>();

			var pcSandardParts = ExcludeStandardParts(
				PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Pc));
			this.PcRealPartsList.AddRange(pcSandardParts);
			var spSandardParts = ExcludeStandardParts(
				PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Sp));
			this.SpRealPartsList.AddRange(spSandardParts);

			// 全パーツリスト
			var partsAllList = this.PcRealPartsList.Select(
				p => new PartsPairingModel
				{
					PcRealParts = p
				}).ToList();

			// SPパーツの設定
			foreach (var spList in this.SpRealPartsList)
			{
				// PCに存在する場合はペアリング
				// 存在しない場合はPCページは存在しないものとして全ページリストへ登録
				if (partsAllList.Any(p => (p.PcRealParts != null) && (p.PcRealParts.FileName == spList.FileName)))
				{
					var val = partsAllList.FirstOrDefault(p => p.PcRealParts.FileName == spList.FileName);
					if (val == null) continue;

					var index = partsAllList.IndexOf(val);
					partsAllList.RemoveAt(index);
					val.SpRealParts = spList;
					partsAllList.Insert(index, val);
				}
				else
				{
					partsAllList.Add(
						new PartsPairingModel
						{
							SpRealParts = spList,
							PcRealParts = new RealParts(
								spList.PageTitle,
								DesignCommon.PhysicalDirPathTargetSitePc,
								spList.PageDirPath,
								spList.FileName,
								spList.PageType,
								spList.TemplateTitle,
								spList.TemplateFileName,
								spList.TemplateFilePath)
							{
								LastChange = spList.LastChange
							}
						});
				}

				Thread.Sleep(10);
			}

			// DBよりパーツ情報取得
			// 実ファイルに存在する場合はグループ・順序を設定
			var partsAllModel = new PartsDesignService().GetAllParts();
			foreach (var partsPairingModel in partsAllList)
			{
				var model = partsAllModel.FirstOrDefault(pm => (pm.FileName == partsPairingModel.PcRealParts.FileName));

				var tempUseType =
					((partsPairingModel.PcRealParts.Existence == RealPage.ExistStatus.Exist)
						&& (partsPairingModel.SpRealParts.Existence == RealPage.ExistStatus.Exist))
						?
						Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP
						: ((partsPairingModel.PcRealParts.Existence == RealPage.ExistStatus.NotExist)
							&& (partsPairingModel.SpRealParts.Existence == RealPage.ExistStatus.Exist))
							? Constants.FLG_PAGEDESIGN_USE_TYPE_SP
							: Constants.FLG_PAGEDESIGN_USE_TYPE_PC;

				if (model != null) continue;

				var managementTitle = "";
				if (partsPairingModel.PcRealParts.PageType != Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM)
				{
					managementTitle = (tempUseType == Constants.FLG_PAGEDESIGN_USE_TYPE_SP)
						? partsPairingModel.SpRealParts.PageTitle
						: partsPairingModel.PcRealParts.PageTitle;
				}
				else
				{
					managementTitle = (tempUseType == Constants.FLG_PARTSDESIGN_USE_TYPE_SP)
						? DesignCommon.GetAspxTitle(
							DesignCommon.GetFileTextAll(partsPairingModel.SpRealParts.PhysicalFullPath))
						: DesignCommon.GetAspxTitle(
							DesignCommon.GetFileTextAll(partsPairingModel.PcRealParts.PhysicalFullPath));
				}

				// 存在しなければパーツテーブルに登録
				var insertModel = new PartsDesignModel
				{
					FileName = partsPairingModel.PcRealParts.FileName,
					FileDirPath = partsPairingModel.PcRealParts.PageDirPath,
					PartsType = partsPairingModel.PcRealParts.PageType,
					// 実ファイルの存在状況より各デバイスページの利用状況を確定
					UseType = tempUseType,
					// 管理用タイトルは各ページのタイトルから引用
					ManagementTitle = managementTitle,
					LastChanged = Constants.FLG_LASTCHANGED_BATCH
				};
				new PartsDesignService().InsertParts(insertModel);

				Thread.Sleep(10);
			}
		}

		/// <summary>
		/// パーツ管理 パーツグループ整合性調整
		/// </summary>
		public void GroupConsistency()
		{
			var service = new PartsDesignService();
			var groupIds = service.NotExistGroupIds();
			foreach (var groupId in groupIds)
			{
				service.UpdatePartsMoveOtherGroup(groupId, Constants.FLG_LASTCHANGED_BATCH);
			}
		}

		/// <summary>
		/// オプションによる対象外の標準パーツを除外
		/// </summary>
		/// <param name="parts">標準パーツリスト</param>
		/// <returns>除外結果</returns>
		private IEnumerable<RealParts> ExcludeStandardParts(List<RealParts> parts)
		{
			var result = parts;
			if (Constants.REPEATPLUSONE_OPTION_ENABLED)
			{
				result = parts.Where(
					p => (Constants.REPEATPLUSONE_CONFIGS.RepeatPlusOneSettings.PartsManager.Any(
						rc => p.FileName.Contains(rc.FileName)) == false)).ToList();
			}
			return result;
		}

		/// <summary>PC 実ファイルリスト</summary>
		private List<RealParts> PcRealPartsList { get; set; }
		/// <summary>SP 実ファイルリスト</summary>
		private List<RealParts> SpRealPartsList { get; set; }
	}

	/// <summary>
	/// パーツ ペアリングクラス
	/// </summary>
	public class PartsPairingModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PartsPairingModel()
		{
			this.PcRealParts = new RealParts();
			this.SpRealParts = new RealParts();
		}

		/// <summary>PC 実ファイル</summary>
		public RealParts SpRealParts { get; set; }
		/// <summary>SP 実ファイル</summary>
		public RealParts PcRealParts { get; set; }
	}
}