/*
=========================================================================================================
  Module      : 住所一括更新クラス (UpdateAddressOfUserAndFixedPurchase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.FixedPurchase;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.User.UpdateAddressOfUserandFixedPurchase
{
	/// <summary>
	/// 住所一括更新クラス
	/// </summary>
	public class UpdateAddressOfUserAndFixedPurchase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAnciton">更新履歴アクション</param>
		/// <param name="doUpdate">更新処理を実行する</param>
		/// <param name="isShippingAddrJp">配送先住所が日本か</param>
		public UpdateAddressOfUserAndFixedPurchase(
			string lastChanged,
			UpdateHistoryAction updateHistoryAnciton,
			bool doUpdate,
			bool isShippingAddrJp)
			: this(lastChanged, updateHistoryAnciton, doUpdate, isShippingAddrJp, new ExceptId[0])
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAnciton">更新履歴アクション</param>
		/// <param name="doUpdate">更新処理を実行する</param>
		/// <param name="isShippingAddrJp">配送先住所が日本か</param>
		/// <param name="exceptIds">除外ID情報</param>
		public UpdateAddressOfUserAndFixedPurchase(
			string lastChanged,
			UpdateHistoryAction updateHistoryAnciton,
			bool doUpdate,
			bool isShippingAddrJp,
			params ExceptId[] exceptIds)
		{
			this.LastChanged = lastChanged;
			this.UpdateHistoryAnciton = updateHistoryAnciton;
			this.DoUpdate = doUpdate;
			this.IsShippingAddrJp = isShippingAddrJp;
			this.ExceptIds = exceptIds;
		}

		/// <summary>
		/// <para>住所情報（氏名、電話番号含む）一括更新処理</para>
		/// <para>編集中の定期台帳を起点として、他の定期台帳やユーザ情報に持つ住所を一括で更新する</para>
		/// <para>更新対象は、編集中の定期台帳と同じ住所情報を持つ、定期台帳とユーザ情報</para>
		/// </summary>
		/// <param name="addressRefrectedPattern">更新パターン</param>
		/// <param name="currentFixedPurchaseUpdated">編集中の定期台帳</param>
		/// <returns>同時更新対象リスト</returns>
		public IEnumerable<IUpdated> AddressMassUpdate(
			string[] addressRefrectedPattern,
			FixedPurchaseModel currentFixedPurchaseUpdated)
		{
			// 返却用同時更新対象リスト
			var updatedIdsForReturn = new List<IUpdated>();
			var fixedPurchaseService = new FixedPurchaseService();

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var currentFixedPurchaseShippingOrigin =
					fixedPurchaseService.Get(currentFixedPurchaseUpdated.FixedPurchaseId, accessor).Shippings[0];

				// ユーザ情報更新
				if (addressRefrectedPattern.Contains(Constants.ADDRESS_UPDATE_PATTERN_USER_TOO))
				{
					AddIdToList(
						UpdateUserAddress(
							currentFixedPurchaseUpdated.UserId,
							currentFixedPurchaseShippingOrigin,
							currentFixedPurchaseUpdated.Shippings[0],
							accessor),
						updatedIdsForReturn);
				}
				// その他の定期台帳更新
				if (addressRefrectedPattern.Contains(Constants.ADDRESS_UPDATE_PATTERN_OTHER_FIXED_PURCASES_TOO))
				{
					var updatedFixedPurchase = UpdateOtherFixedPurchase(
						currentFixedPurchaseShippingOrigin,
						currentFixedPurchaseUpdated,
						accessor);
					foreach (var fixedPurchase in updatedFixedPurchase)
					{
						AddIdToList(fixedPurchase, updatedIdsForReturn);
					}
				}

				// 編集中の定期台帳を更新する
				if (this.DoUpdate)
				{
					fixedPurchaseService.UpdateShipping(
						currentFixedPurchaseUpdated.Shippings[0],
						this.LastChanged,
						this.UpdateHistoryAnciton,
						accessor);
				}
				accessor.CommitTransaction();
			}

			// 更新対象リストを返却する
			return updatedIdsForReturn;
		}

		/// <summary>
		/// その他の定期台帳を更新する
		/// </summary>
		/// <param name="currentFixedPurchaseShippingOrigin">編集中の定期台帳が持つ配送情報（編集前）</param>
		/// <param name="currentFixedPurchaseUpdated">編集中の定期台帳（編集後）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>同時更新対象リスト</returns>
		private IEnumerable<IUpdated> UpdateOtherFixedPurchase(
			FixedPurchaseShippingModel currentFixedPurchaseShippingOrigin,
			FixedPurchaseModel currentFixedPurchaseUpdated,
			SqlAccessor accessor)
		{
			// 編集中の定期台帳を持つユーザーが持っているほかの定期情報を取得する
			var otherFixedPurchases = new FixedPurchaseService().GetFixedPurchasesByUserId(currentFixedPurchaseUpdated.UserId, accessor);
			// その他の定期台帳を更新する
			var updatedIdList = new List<IUpdated>();
			foreach (var otherFixedPurchase in otherFixedPurchases)
			{
				var updated = UpdateOtherFixedPurchaseInLoop(
					otherFixedPurchase,
					currentFixedPurchaseShippingOrigin,
					currentFixedPurchaseUpdated.Shippings[0],
					accessor);
				updatedIdList.Add(updated);
			}
			return updatedIdList;
		}

		/// <summary>
		/// <para>その他の定期台帳を更新する</para>
		/// <para>同じ配送情報を持つ定期台帳のみ更新する</para>
		/// </summary>
		/// <param name="otherFixedPurchase">その他の定期台帳情報</param>
		/// <param name="currentFixedPurchaseShipping">編集中の定期台帳が持つ編集前の配送情報</param>
		/// <param name="currentFixedPurchaseUpdatedShipping">編集中の定期台帳が持つ編集後の配送情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>同時更新対象</returns>
		private IUpdated UpdateOtherFixedPurchaseInLoop(
			FixedPurchaseModel otherFixedPurchase,
			FixedPurchaseShippingModel currentFixedPurchaseShipping,
			FixedPurchaseShippingModel currentFixedPurchaseUpdatedShipping,
			SqlAccessor accessor)
		{
			var otherFixedPurchaseShipping = otherFixedPurchase.Shippings[0];
			if (otherFixedPurchaseShipping.IsSameAddress(currentFixedPurchaseShipping, this.IsShippingAddrJp)
				&& (otherFixedPurchaseShipping.FixedPurchaseId != currentFixedPurchaseShipping.FixedPurchaseId))
			{
				// 除外対象の定期購入IDは更新しない
				if (this.DoUpdate)
				{
					if (this.ExceptIds.Any(exceptId
							=> ((exceptId.UpdatedKbn == UpdatedKbn.OtherFixedPurchase)
								&& (exceptId.Ids.Contains(otherFixedPurchaseShipping.FixedPurchaseId)))))
						return null;

					// 配送情報をコピーし、更新する
					otherFixedPurchaseShipping.SetAddress(currentFixedPurchaseUpdatedShipping);
					new FixedPurchaseService().UpdateShipping(otherFixedPurchaseShipping, this.LastChanged, this.UpdateHistoryAnciton, accessor);
				}

				var updatedFixedPurchase = new UpdatedFixedPurchase
				{
					Id = otherFixedPurchaseShipping.FixedPurchaseId,
					FixedPurchase = otherFixedPurchase
				};
				return updatedFixedPurchase;
			}
			return null;
		}

		/// <summary>
		/// <para>ユーザー情報の住所を更新する</para>
		/// <para>配送情報が持つ住所等と同じ内容をユーザー情報に持つ場合のみ更新する</para>
		/// </summary>
		/// <param name="userId">対象ユーザーID</param>
		/// <param name="currentFixedPurchaseShipping">編集中の定期台帳が持つ編集前の配送情報</param>
		/// <param name="currentFixedPurchaseUpdatedShipping">編集中の定期台帳が持つ編集後の配送情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>同時更新対象ID</returns>
		private IUpdated UpdateUserAddress(
			string userId,
			FixedPurchaseShippingModel currentFixedPurchaseShipping,
			FixedPurchaseShippingModel currentFixedPurchaseUpdatedShipping,
			SqlAccessor accessor)
		{
			var user = new UserService().Get(userId);
			var updateUser = new UpdatedUser();
			if (user.IsSameAddress(currentFixedPurchaseShipping, this.IsShippingAddrJp))
			{
				if (this.DoUpdate)
				{
					// 除外対象のユーザーIDは更新しない
					if (this.ExceptIds.Any(
						exceptId => ((exceptId.UpdatedKbn == UpdatedKbn.User) && (exceptId.Ids.Contains(userId)))))
						return null;

					// ユーザ情報に配送情報が持つ住所をコピーし、更新する
					user.SetAddress(currentFixedPurchaseUpdatedShipping);
					user.LastChanged = this.LastChanged;
					new UserService().Update(user, this.UpdateHistoryAnciton, accessor);
				}
				return new UpdatedUser { Id = userId, User = user };
				
			}
			return null;
		}

		/// <summary>
		/// <para>更新対象リスト追加</para>
		/// <para>重複するIDを除外する</para>
		/// </summary>
		/// <param name="id">追加対象</param>
		/// <param name="updateIds">同時更新対象IDリスト</param>
		private void AddIdToList(IUpdated id, List<IUpdated> updateIds)
		{
			if ((id != null) && (updateIds.Find(existingId => id.Id == existingId.Id) == null)) updateIds.Add(id);
		}

		#region プロパティ
		/// <summary>最終更新者</summary>
		private string LastChanged { get; set; }
		/// <summary>更新履歴アクション</summary>
		private UpdateHistoryAction UpdateHistoryAnciton { get; set; }
		/// <summary>更新処理を実行する</summary>
		private bool DoUpdate { get; set; }
		/// <summary>配送先住所が日本か</summary>
		private bool IsShippingAddrJp { get; set; }
		/// <summary>除外ID情報</summary>
		private ExceptId[] ExceptIds { get; set; }
		#endregion

		#region 内部クラス
		/// <summary>
		/// 除外ID情報
		/// </summary>
		public class ExceptId
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="updatedKbn">更新対象区分</param>
			/// <param name="ids">更新対象ID配列</param>
			public ExceptId(UpdatedKbn updatedKbn, string[] ids)
			{
				this.UpdatedKbn = updatedKbn;
				this.Ids = ids;
			}

			#region プロパティ
			/// <summary>更新対象区分</summary>
			public UpdatedKbn UpdatedKbn { get; set; }
			/// <summary>除外ID</summary>
			public string[] Ids { get; set; }
			#endregion
		}
		#endregion
	}
}