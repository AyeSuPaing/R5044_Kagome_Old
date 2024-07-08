/*
=========================================================================================================
  Module      : 店舗管理者入力クラス (ShopOperatorInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.ShopOperator;

/// <summary>
/// 店舗管理者マスタ入力クラス
/// </summary>
public class ShopOperatorInput : InputBase<ShopOperatorModel>
{
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ShopOperatorInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ShopOperatorInput(ShopOperatorModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.OperatorId = model.OperatorId;
		this.Name = model.Name;
		this.MailAddr = model.MailAddr;
		this.MenuAccessLevel1 = (model.MenuAccessLevel1 != null) ? model.MenuAccessLevel1.ToString() : null;
		this.MenuAccessLevel2 = (model.MenuAccessLevel2 != null) ? model.MenuAccessLevel2.ToString() : null;
		this.MenuAccessLevel3 = (model.MenuAccessLevel3 != null) ? model.MenuAccessLevel3.ToString() : null;
		this.MenuAccessLevel4 = (model.MenuAccessLevel4 != null) ? model.MenuAccessLevel4.ToString() : null;
		this.MenuAccessLevel5 = (model.MenuAccessLevel5 != null) ? model.MenuAccessLevel5.ToString() : null;
		this.MenuAccessLevel6 = (model.MenuAccessLevel6 != null) ? model.MenuAccessLevel6.ToString() : null;
		this.MenuAccessLevel7 = (model.MenuAccessLevel7 != null) ? model.MenuAccessLevel7.ToString() : null;
		this.MenuAccessLevel8 = (model.MenuAccessLevel8 != null) ? model.MenuAccessLevel8.ToString() : null;
		this.MenuAccessLevel9 = (model.MenuAccessLevel9 != null) ? model.MenuAccessLevel9.ToString() : null;
		this.MenuAccessLevel10 = (model.MenuAccessLevel10 != null) ? model.MenuAccessLevel10.ToString() : null;
		this.LoginId = model.LoginId;
		this.Password = model.Password;
		this.OdbcUserName = model.OdbcUserName;
		this.OdbcPassword = model.OdbcPassword;
		this.ValidFlg = model.ValidFlg;
		this.DelFlg = model.DelFlg;
		this.UsableAdvCodeNosInReport = model.UsableAdvcodeNosInReport ?? string.Empty;
	}

	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ShopOperatorModel CreateModel()
	{
		var model = new ShopOperatorModel
		{
			ShopId = this.ShopId,
			OperatorId = this.OperatorId,
			Name = this.Name,
			MailAddr = this.MailAddr ?? string.Empty,
			MenuAccessLevel1 = (this.MenuAccessLevel1 != null) ? int.Parse(this.MenuAccessLevel1) : (int?)null,
			MenuAccessLevel2 = (this.MenuAccessLevel2 != null) ? int.Parse(this.MenuAccessLevel2) : (int?)null,
			MenuAccessLevel3 = (this.MenuAccessLevel3 != null) ? int.Parse(this.MenuAccessLevel3) : (int?)null,
			MenuAccessLevel4 = (this.MenuAccessLevel4 != null) ? int.Parse(this.MenuAccessLevel4) : (int?)null,
			MenuAccessLevel5 = (this.MenuAccessLevel5 != null) ? int.Parse(this.MenuAccessLevel5) : (int?)null,
			MenuAccessLevel6 = (this.MenuAccessLevel6 != null) ? int.Parse(this.MenuAccessLevel6) : (int?)null,
			MenuAccessLevel7 = (this.MenuAccessLevel7 != null) ? int.Parse(this.MenuAccessLevel7) : (int?)null,
			MenuAccessLevel8 = (this.MenuAccessLevel8 != null) ? int.Parse(this.MenuAccessLevel8) : (int?)null,
			MenuAccessLevel9 = (this.MenuAccessLevel9 != null) ? int.Parse(this.MenuAccessLevel9) : (int?)null,
			MenuAccessLevel10 = (this.MenuAccessLevel10 != null) ? int.Parse(this.MenuAccessLevel10) : (int?)null,
			LoginId = this.LoginId,
			Password = this.Password,
			OdbcUserName = this.OdbcUserName ?? string.Empty,
			OdbcPassword = this.OdbcPassword ?? string.Empty,
			ValidFlg = this.ValidFlg,
			DelFlg = this.DelFlg,
			UsableAdvcodeNosInReport = this.UsableAdvCodeNosInReport,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate(string checkDivision)
	{
		var error = string.Join(
			Environment.NewLine,
			w2.Common.Util.Validator.Validate(checkDivision, this.DataSource)
				.Select(err => err.Value)
				.ToArray());
		return error;
	}

	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_SHOP_ID] = value; }
	}
	/// <summary>オペレータID</summary>
	public string OperatorId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID] = value; }
	}
	/// <summary>オペレータ名</summary>
	public string Name
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME] = value; }
	}
	/// <summary>メールアドレス</summary>
	public string MailAddr
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MAIL_ADDR]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MAIL_ADDR] = value; }
	}
	/// <summary>メニューアクセスレベル1</summary>
	public string MenuAccessLevel1
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1] = value; }
	}
	/// <summary>メニューアクセスレベル2</summary>
	public string MenuAccessLevel2
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL2]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL2] = value; }
	}
	/// <summary>メニューアクセスレベル3</summary>
	public string MenuAccessLevel3
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL3]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL3] = value; }
	}
	/// <summary>メニューアクセスレベル4</summary>
	public string MenuAccessLevel4
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL4]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL4] = value; }
	}
	/// <summary>メニューアクセスレベル5</summary>
	public string MenuAccessLevel5
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL5]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL5] = value; }
	}
	/// <summary>メニューアクセスレベル6</summary>
	public string MenuAccessLevel6
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL6]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL6] = value; }
	}
	/// <summary>メニューアクセスレベル7</summary>
	public string MenuAccessLevel7
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL7]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL7] = value; }
	}
	/// <summary>メニューアクセスレベル8</summary>
	public string MenuAccessLevel8
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL8]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL8] = value; }
	}
	/// <summary>メニューアクセスレベル9</summary>
	public string MenuAccessLevel9
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL9]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL9] = value; }
	}
	/// <summary>メニューアクセスレベル10</summary>
	public string MenuAccessLevel10
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL10]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL10] = value; }
	}
	/// <summary>ログインID</summary>
	public string LoginId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_LOGIN_ID]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_LOGIN_ID] = value; }
	}
	/// <summary>パスワード</summary>
	public string Password
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_PASSWORD]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_PASSWORD] = value; }
	}
	/// <summary>ODBCユーザ名</summary>
	public string OdbcUserName
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_ODBC_USER_NAME]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_ODBC_USER_NAME] = value; }
	}
	/// <summary>ODBCパスワード</summary>
	public string OdbcPassword
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_ODBC_PASSWORD]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_ODBC_PASSWORD] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_VALID_FLG] = value; }
	}
	/// <summary>削除フラグ</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_DEL_FLG] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_LAST_CHANGED] = value; }
	}
	/// <summary>閲覧可能な広告コード</summary>
	public string UsableAdvCodeNosInReport
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_NOS_IN_REPORT]; }
		set { this.DataSource[Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_NOS_IN_REPORT] = value; }
	}
}
