/*
=========================================================================================================
  Module      : オペレータ情報確認ページ処理(OperatorConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.DataCacheController;
using w2.Domain;
using w2.Domain.MenuAuthority;
using w2.Domain.OperatorAuthority;
using w2.Domain.RealShop;
using w2.Domain.ShopOperator;

public partial class Form_Operator_OperatorConfirm : BasePage
{
	protected Hashtable m_htOperator = new Hashtable();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, strActionStatus);

			string strOperatorId = Request[Constants.REQUEST_KEY_OPERATOR_ID];
			ViewState.Add(Constants.REQUEST_KEY_OPERATOR_ID, strOperatorId);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(strActionStatus);

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 登録・画面確認？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT
				|| strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				//------------------------------------------------------
				// 処理区分チェック
				//------------------------------------------------------
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				m_htOperator = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];
			}
			// 詳細表示？
			else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				var shopOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, strOperatorId);
				if (shopOperator == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				m_htOperator = shopOperator.DataSource;

				// 「スーパーユーザー」名設定
				if (shopOperator.IsSuperUser(Constants.ManagerSiteType))
				{
					m_htOperator[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG + "_name"] = Constants.STRING_SUPERUSER_NAME;
				}
				// 「権限なしユーザ」名設定
				else if (shopOperator.IsInaccessibleUser(Constants.ManagerSiteType))
				{
					m_htOperator[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG + "_name"] = Constants.STRING_UNACCESSABLEUSER_NAME;
				}
				else
				{
					var menuAuthorities = new MenuAuthorityService().Get(
						shopOperator.ShopId,
						Constants.ManagerSiteType,
						shopOperator.GetMenuAccessLevel(Constants.ManagerSiteType).Value);
					if (menuAuthorities.Length != 0)
					{
						m_htOperator[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG + "_name"] = menuAuthorities[0].MenuAuthorityName;
					}
				}

				if (Constants.REALSHOP_OPTION_ENABLED)
				{
					var operatorAuthority = DomainFacade.Instance.OperatorAuthorityService.Get(
						shopOperator.ShopId,
						shopOperator.OperatorId);

					if (operatorAuthority != null)
					{
						var realShopSelect = new StringBuilder();
						var realShopIds = operatorAuthority.Select(opere => opere.ConditionValue).ToList();
						var areas = DataCacheControllerFacade
							.GetRealShopAreaCacheController()
							.GetRealShopAreaList();

						var realShop = new RealShopService().GetAll();
						if (realShop != null)
						{
							var realShopGroup = realShop
								.Where(shop => realShopIds.Contains(shop.RealShopId))
								.GroupBy(shop => shop.AreaId)
								.ToList();

							foreach (var realShopAreas in realShopGroup)
							{
								var area = areas.FirstOrDefault(are => are.AreaId == realShopAreas.Key);
								realShopSelect.AppendFormat("{0}: ", area.AreaName);
								var realShopNameList = realShopAreas.Select(shop => shop.Name);
								realShopSelect.AppendFormat("{0}<br />", string.Join(", ", realShopNameList));
							}
						}

						m_htOperator.Add(Constants.FLG_AREA_REAL_SHOP_NAME, realShopSelect.ToString());
					}
				}
			}
			// 該当なし？
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// オペレータ情報にパスワード情報も含まれているため
			// ViewStateではなく、Sessionに格納しておく
			Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = m_htOperator;

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 詳細表示？
		if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnEditBottom.Visible = true;
			btnEditTop.Visible = true;

			// 作成日・更新日・最終更新者表示
			trOperatorId.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;

			// 削除ボタン表示
			btnDeleteBottom.Visible = true;
			btnDeleteTop.Visible = true;
		}
		else
		{
			if (Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] == null)
			{
				// オペーレータ登録の後にブラウザバックを行うとシステムエラーになるため、
				// オペーレータ一覧ページに遷移させる
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST);
			}

			// 新規登録？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				btnInsertBottom.Visible = true;
				btnInsertTop.Visible = true;
			}
			// 編集確認？
			else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				trOperatorId.Visible = true;
				btnUpdateBottom.Visible = true;
				btnUpdateTop.Visible = true;
			}
		}
	}

	/// <summary>
	/// 編集ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// 更新ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		var input = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];

		// パスワード変更を行わない場合はNullを設定
		if (string.IsNullOrEmpty((string)input[Constants.FIELD_SHOPOPERATOR_PASSWORD]))
		{
			input[Constants.FIELD_SHOPOPERATOR_PASSWORD] = DBNull.Value;
		}

		// 更新
		using (var sqlAccessor = new SqlAccessor())
		using (var sqlStatement = new SqlStatement("ShopOperator", "UpdateOperator"))
		{
			var result = sqlStatement.ExecStatementWithOC(sqlAccessor, input);
		}

		// Insert or update operator authority
		InsertUpdateOperatorAuthority(input);

		Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = null;

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST);
	}

	/// <summary>
	/// 登録ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// オペレータ情報取得
		var input = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];
		input[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID] = NumberingUtility.CreateKeyId(
			this.LoginOperatorShopId,
			Constants.NUMBER_KEY_SHOP_OPERATOR_ID,
			Constants.CONST_SHOPOPERATOR_ID_LENGTH);

		// 登録
		using (var sqlAccessor = new SqlAccessor())
		{
			using (var sqlStatement = new SqlStatement("ShopOperator", "InsertOperator"))
			{
				// 登録
				var result = sqlStatement.ExecStatementWithOC(sqlAccessor, input);
			}
		}

		// Insert or update operator authority
		InsertUpdateOperatorAuthority(input);

		Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = null;

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		var htInput = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];

		var service = new CsOperatorService(new CsOperatorRepository());
		if (service.DeleteCheck(this.LoginOperatorDeptId, (string)htInput[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CSOPERATOR_DELETE_IMPOSSIBLE_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 削除
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ShopOperator", "DeleteOperator"))
		{
			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		// Delete operator authority
		DomainFacade.Instance.OperatorAuthorityService.Delete(
			this.LoginOperatorDeptId,
			(string)htInput[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]);

		Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = null;

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST);
	}

	/// <summary>
	/// Insert update operator authority
	/// </summary>
	/// <param name="input">Input</param>
	private void InsertUpdateOperatorAuthority(Hashtable input)
	{
		if (Constants.REALSHOP_OPTION_ENABLED
			&& input.ContainsKey(Constants.FLG_LIST_REAL_SHOP_ID))
		{
			var realShopSelect = (List<string>)input[Constants.FLG_LIST_REAL_SHOP_ID];
			var shopId = (string)input[Constants.FIELD_SHOPOPERATOR_SHOP_ID];
			var operatorId = (string)input[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID];
			DomainFacade.Instance.OperatorAuthorityService.Delete(shopId, operatorId);

			foreach (var realShop in realShopSelect)
			{
				var inputAuthority = new OperatorAuthorityModel
				{
					ShopId = shopId,
					OperatorId = operatorId,
					ConditionValue = realShop,
				};

				DomainFacade.Instance.OperatorAuthorityService.Insert(inputAuthority);
			}
		}
	}
}
