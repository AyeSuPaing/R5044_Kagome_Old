/*
=========================================================================================================
  Module      : ���b�Z�[�W�񋟃N���X�i�_�~�[�j(MessageProviderDummy.cs)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Common.Util
{
	/// <summary>
	/// ���b�Z�[�W�񋟃N���X�i�_�~�[�j
	/// </summary>
	/// <remarks>
	/// GetMessages�ŁA�����̃��b�Z�[�W�L�[�����̂܂ܕԋp����
	/// </remarks>
	public class MessageProviderDummy : IMessageProvider
	{
		/// <summary>
		/// �G���[���b�Z�[�W�擾
		/// </summary>
		/// <param name="messageKey">���b�Z�[�W�L�[</param>
		/// <param name="replaces">�u���p�����[�^</param>
		/// <returns>�G���[���b�Z�[�W</returns>
		public string GetMessages(string messageKey, params string[] replaces)
		{
			return messageKey;
		}
	}
}
