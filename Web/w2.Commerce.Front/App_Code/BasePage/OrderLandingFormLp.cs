/*
=========================================================================================================
  Module      : 注文LPフォームページ(OrderLandingFormLp.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.LandingPage.LandingPageDesignData;
using w2.App.Common.Web.Process;
using w2.Domain.LandingPage;

/// <summary>
/// 注文LPフォームページ
/// </summary>
public class OrderLandingFormLp : OrderCartPageLanding
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	/// <summary>
	/// ページデザイン作成
	/// </summary>
	/// <param name="landingPageDesign">ランディングページデザイン</param>
	/// <returns>ページデザインインプット</returns>
	protected PageDesignInput CreatePageDesign(LandingPageDesignModel landingPageDesign)
	{
		var rtn = new PageDesignInput
		{
			PageId = landingPageDesign.PageId,
			BlockSettings = landingPageDesign.Blocks.Select(b => new BlockDesignInput
			{
				BlockClassName = b.BlockClassName,
				BlockIndex = b.BlockIndex.ToString(),
				Elements = b.Elements.Select(e => new BlockElementInput
				{
					ElementIndex = e.ElementIndex.ToString(),
					ElementPlaceHolderName = e.ElementPlaceHolderName,
					Attributes = e.Attributes.Select(a => new BlockKeyValue
					{
						Attribute = a.Attribute,
						Value = a.Value
					}).ToArray()
				}).ToArray()
			}).ToArray()
		};

		return rtn;
	}

	/// <summary>
	/// ページコントロール表示
	/// </summary>
	/// <param name="landingPageDesign">ランディングページデザイン</param>
	public void DisplayControlPage(LandingPageDesignModel landingPageDesign)
	{
		this.Title = landingPageDesign.PageTitle;
		this.MetaDescription = landingPageDesign.MetadataDesc;
	}

	/// <summary>プロセス</summary>
	protected new OrderLandingProcess Process
	{
		get { return (OrderLandingProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new OrderLandingProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
	/// <summary>ページデザイン</summary>
	protected LandingPageDesignModel LpPageDesign { get; set; }
	/// <summary>ページデザインインプット</summary>
	protected PageDesignInput PageDesignInput { get; set; }
}