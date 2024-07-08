/*
=========================================================================================================
  Module      : スタッフRegisterビューモデル(RegisterViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Common.Util;
using w2.Domain.RealShop;

namespace w2.Cms.Manager.ViewModels.Staff
{
	/// <summary>
	/// スタッフRegisterビューモデル
	/// </summary>
	[Serializable]
	public class RegisterViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegisterViewModel()
		{
			// 性別ラジオボタンリスト
			this.StaffSexList =
				ValueText.GetValueItemArray(Constants.TABLE_STAFF, Constants.FIELD_STAFF_STAFF_SEX)
					.Select(l =>
						new SelectListItem
						{
							Text = l.Text,
							Value = l.Value
						}).ToList();

			var realShopList = new RealShopService().GetAll();
			if (realShopList != null)
			{
				this.RealShopList =
					new RealShopService().GetAll()
						.Select(l =>
							new SelectListItem
							{
								Text = l.Name,
								Value = l.RealShopId
							}).ToList();
				this.RealShopList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}
			else
			{
				this.RealShopList = new List<SelectListItem>();
				this.RealShopList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}

			this.ModelFlg = Constants.FLG_STAFF_MODEL_FLG_VALID;
		}

		/// <summary>スタッフID</summary>
		public string StaffId { get; set; }
		/// <summary>スタッフ名</summary>
		public string StaffName { get; set; }
		/// <summary>身長</summary>
		public string StaffHeight { get; set; }
		/// <summary>プロフィールテキスト</summary>
		public string StaffProfile { get; set; }
		/// <summary>性別</summary>
		public string StaffSex { get; set; }
		/// <summary>性別リスト</summary>
		public List<SelectListItem> StaffSexList { get; set; }
		/// <summary>オペレータID</summary>
		public string OperatorId { get; set; }
		/// <summary>リアル店舗ID</summary>
		public string RealShopId { get; set; }
		/// <summary>リアルショップリスト</summary>
		public List<SelectListItem> RealShopList { get; set; }
		/// <summary>インスタグラムID</summary>
		public string StaffInstagramId { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>有効か</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_STAFF_VALID_FLG_VALID); }
		}
		/// <summary>モデルフラグ</summary>
		public string ModelFlg { get; set; }
		/// <summary>モデルか</summary>
		public bool IsModel
		{
			get { return (this.ModelFlg == Constants.FLG_STAFF_MODEL_FLG_VALID); }
		}
		/// <summary>ライターフラグ</summary>
		public string WriterFlg { get; set; }
		/// <summary>リアル店舗IDが必須か</summary>
		public bool IsRequiredRealShopId
		{
			get
			{
				var result = Validator.HasNecessary(
					(this.ActionStatus == ActionStatus.Insert)
						? "StaffRegister"
						: (this.ActionStatus == ActionStatus.Update)
							? "StaffModify"
							: "",
					Constants.FIELD_STAFF_REAL_SHOP_ID);
				return result;
			}
		}
	}
}