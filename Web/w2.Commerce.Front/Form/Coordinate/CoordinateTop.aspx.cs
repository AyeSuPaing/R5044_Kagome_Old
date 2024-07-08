/*
=========================================================================================================
  Module      : コーディネートトップ画面処理(CoordinateTop.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order;

public partial class Form_Coordinate_CoordinateTop : CoordinatePage
{
	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } }	// Httpアクセス

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
	}
}