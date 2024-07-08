/*
=========================================================================================================
  Module      : 商品タグ出力クラス(ActionExportProductTag.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Product;
using w2.Domain.ProductTag;

namespace w2.Commerce.Batch.CustomerRingsExport.Action
{
	/// <summary>
	/// 出力クラス
	/// </summary>
	public class ActionExportProductTag : ActionExportBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ActionExportProductTag(FileDefines.FileDefine define)
			: base(define)
		{
			this.ExportFieldString = "";

			var list = ProductTagUtility.GetProductTagSetting();
			foreach (ProductTagSettingModel model in list)
			{
				this.ExportFieldString += "," + model.TagId;
			}
		}
	}
}
