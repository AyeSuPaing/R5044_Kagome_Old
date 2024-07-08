/*
=========================================================================================================
  Module      : レイアウト編集 入力(LayoutEditInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Domain.PartsDesign;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// レイアウト編集 入力
	/// </summary>
	public class LayoutEditInput
	{
		/// <summary>
		/// レイアウトタイプ
		/// </summary>
		public enum LayoutTypeMaster
		{
			/// <summary>デフォルト 両サイドバー</summary>
			Default,
			/// <summary>左バー</summary>
			Left,
			/// <summary>右バー</summary>
			Right,
			/// <summary>両サイドバーなし</summary>
			NoSide,
			/// <summary>白紙ページ ヘッダフッタなし</summary>
			Simple
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LayoutEditInput(bool isFeaturePage = false)
		{
			this.LayoutType = isFeaturePage
				? LayoutTypeMaster.NoSide.ToString()
				: LayoutTypeMaster.Default.ToString();
			this.MovePartsLeft = new MovePartsModel[] { };
			this.MovePartsRight = new MovePartsModel[] { };
			this.MovePartsCenterTop = new MovePartsModel[] { };
			this.MovePartsCenterBottom = new MovePartsModel[] { };
		}

		/// <summary>
		/// バインドで受け取った配置内容をMovePartsModelにセット
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		public void SetMovePartsModel(DesignCommon.DeviceType deviceType)
		{
			var allRealPartsList = PartsDesignCommon.GetCustomPartsList(deviceType);
			allRealPartsList.AddRange(PartsDesignCommon.GetStandardPartsList(deviceType));
			var partsAllModel = new PartsDesignService().GetAllParts();

			this.MovePartsLeft = SetMovePartsList(allRealPartsList, partsAllModel, this.MovePartsLeftIds);
			this.MovePartsRight = SetMovePartsList(allRealPartsList, partsAllModel, this.MovePartsRightIds);
			this.MovePartsCenterTop = SetMovePartsList(allRealPartsList, partsAllModel, this.MovePartsCenterTopIds);
			this.MovePartsCenterBottom = SetMovePartsList(
				allRealPartsList,
				partsAllModel,
				this.MovePartsCenterBottomIds);
		}

		/// <summary>
		/// パーツ文字列をMovePartsModelに変換
		/// </summary>
		/// <param name="allRealPartsList">全ての実ページ</param>
		/// <param name="partsAllModel">全てのパーツモデル</param>
		/// <param name="ids">パーツ文字列</param>
		/// <returns>MovePartsModel配列</returns>
		private MovePartsModel[] SetMovePartsList(
			List<RealParts> allRealPartsList,
			PartsDesignModel[] partsAllModel,
			string[] ids)
		{
			if (ids == null) return new MovePartsModel[] { };

			var result = ids.Select(
				id =>
				{
					var model = partsAllModel.FirstOrDefault(m => m.PartsId == long.Parse(id));
					if (model == null) return new MovePartsModel();

					var realParts = allRealPartsList.FirstOrDefault(p => p.FileName == model.FileName);
					if (realParts == null) return new MovePartsModel();

					return new MovePartsModel
					{
						RealParts = realParts,
						PartsDesignModel = model
					};
				}).ToArray();
			return result;
		}

		/// <summary>レイアウトタイプ</summary>
		public string LayoutType { get; set; }
		/// <summary>左レイアウト配置順(初期表示用)</summary>
		public MovePartsModel[] MovePartsLeft { get; set; }
		/// <summary>右レイアウト配置順(初期表示用)</summary>
		public MovePartsModel[] MovePartsRight { get; set; }
		/// <summary>コンテンツ上レイアウト配置順(初期表示用)</summary>
		public MovePartsModel[] MovePartsCenterTop { get; set; }
		/// <summary>コンテンツ下レイアウト配置順(初期表示用)</summary>
		public MovePartsModel[] MovePartsCenterBottom { get; set; }
		/// <summary>左レイアウト配置順(バインド用)</summary>
		public string[] MovePartsLeftIds { get; set; }
		/// <summary>右レイアウト配置順(バインド用)</summary>
		public string[] MovePartsRightIds { get; set; }
		/// <summary>コンテンツ上レイアウト配置順(バインド用)</summary>
		public string[] MovePartsCenterTopIds { get; set; }
		/// <summary>コンテンツ下レイアウト配置順(バインド用)</summary>
		public string[] MovePartsCenterBottomIds { get; set; }
	}
}