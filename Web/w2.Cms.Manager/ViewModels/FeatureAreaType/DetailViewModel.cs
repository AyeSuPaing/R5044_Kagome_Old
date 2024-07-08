/*
=========================================================================================================
  Module      : 特集エリアタイプ詳細ビューモデル(DetailViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Common.Util;
using w2.Domain.FeatureArea;

namespace w2.Cms.Manager.ViewModels.FeatureAreaType
{
	/// <summary>
	/// 特集エリアタイプ詳細ビューモデル
	/// </summary>
	public class DetailViewModel : ViewModelBase
	{
		/// <summary>画像共通パーツのバインド時の名前</summary>
		private const string AREA_TYPE_IMAGE = "Input.AreaTypeImage";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public DetailViewModel(FeatureAreaTypeModel model)
		{
			if (model == null)
			{
				this.Input = new FeatureAreaTypeInput();
				this.ActionStatus = ActionStatus.Insert;
				return;
			}

			this.ActionStatus = ActionStatus.Update;
			this.Input = new FeatureAreaTypeInput
			{
				AreaTypeId = model.AreaTypeId,
				AreaTypeName = model.AreaTypeName,
				ActionType = model.ActionType,
				InternalMemo = model.InternalMemo,
				PcTagInput = new FeatureAreaTypeDetailTagInput()
				{
					IsPc = true,
					StartTag = model.PcStartTag,
					RepeatTag = model.PcRepeatTag,
					EndTag = model.PcEndTag,
					ScriptTag = model.PcScriptTag,
				},
				SpTagInput = new FeatureAreaTypeDetailTagInput()
				{
					IsPc = false,
					StartTag = model.SpStartTag,
					RepeatTag = model.SpRepeatTag,
					EndTag = model.SpEndTag,
					ScriptTag = model.SpScriptTag,
				},
			};
		}

		/// <summary> PC/SP </summary>
		public bool IsPc { get; set; }
		/// <summary>入力クラス</summary>
		public FeatureAreaTypeInput Input { get; set; }
		/// <summary>動作タイプ</summary>
		public KeyValuePair<string, string>[] ActionTypes
		{
			get
			{
				return ValueText.GetValueKvpArray(
					Constants.TABLE_FEATUREAREATYPE,
					Constants.FIELD_FEATUREAREATYPE_ACTION_TYPE);
			}
		}
		/// <summary>削除可能か</summary>
		public bool IsDeletable
		{
			get
			{
				return this.IsActionStatusUpdate
					&& (this.IsDefaultAreaType == false);
			}
		}
		/// <summary>デフォルト定義の特集エリアタイプか</summary>
		public bool IsDefaultAreaType
		{
			get
			{
				return ((this.Input.AreaTypeId == Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_SIDE)
					|| (this.Input.AreaTypeId == Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_SLIDER)
					|| (this.Input.AreaTypeId == Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_RANDOM)
					|| (this.Input.AreaTypeId == Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_VERTICAL));
			}
		}
	}
}