/*
=========================================================================================================
  Module      : サイト情報Modifyビューモデル(ModifyViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using w2.App.Common;
using w2.App.Common.DataCacheController;
using w2.Cms.Manager.Input;

namespace w2.Cms.Manager.ViewModels.SiteInformation
{
	/// <summary>
	/// サイト情報Modifyビューモデル
	/// </summary>
	[Serializable]
	public class ModifyViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ModifyViewModel()
		{
			var physicalFilePathSiteInformationXml = Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Codes.Constants.FILE_XML_SHOP_MESSAGE);

			var xmlDocument = new XmlDocument();
			xmlDocument.Load(physicalFilePathSiteInformationXml);

			this.PaymentReplaceTags = new PaymentCacheController().GetValidAllWithPrice()
				.Select(m => new ListItem
				{
					Value = "<@@OrderPaymentKbn:" + m.PaymentId + "@@></@@OrderPaymentKbn:" + m.PaymentId + "@@>",
					Text = m.PaymentName
				}).ToArray();

			this.Input = new SiteInformationInput(xmlDocument);
		}

		/// <summary>サイト情報入力</summary>
		public SiteInformationInput Input { get; set; }
		/// <summary>決済種別置換タグリスト</summary>
		public ListItem[] PaymentReplaceTags { get; set; }
	}

}