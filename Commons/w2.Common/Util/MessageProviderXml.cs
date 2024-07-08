/*
=========================================================================================================
  Module      : ���b�Z�[�W�񋟃N���X�iXML�t�@�C������擾�j(MessageProviderXml.cs)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace w2.Common.Util
{
	/// <summary>
	/// ���b�Z�[�W�񋟃N���X�iXML�t�@�C������擾�j
	/// </summary>
	/// <remarks>
	/// GetMessages�ŃG���[���b�Z�[�W�擾�B
	/// GetMessages���Ăяo�����Ƃ�XML�t�@�C���̍X�V���t���`�F�b�N���A
	/// �ύX������΍ēǂݍ��݂��s���B
	/// </remarks>
	public class MessageProviderXml : IMessageProvider
	{
		/// <summary>�G���[�t�@�C���ŏI�X�V��</summary>
		private static DateTime s_dtFileLastUpdate = new DateTime(0);
		/// <summary>�G���[���b�Z�[�W�i�[�f�B�N�V���i��</summary>
		private static Dictionary<string, string> s_dicMessages = new Dictionary<string, string>();
		/// <summary>ReaderWriterLockSlim�I�u�W�F�N�g</summary>
		private static System.Threading.ReaderWriterLockSlim s_lock = new System.Threading.ReaderWriterLockSlim();

		/// <summary>
		/// �G���[���b�Z�[�W�擾
		/// </summary>
		/// <param name="messageKey">���b�Z�[�W�L�[</param>
		/// <param name="replaces">�u���p�����[�^</param>
		/// <returns>�G���[���b�Z�[�W</returns>
		public string GetMessages(string messageKey, params string[] replaces)
		{
			// �ǂݍ��݃��b�N�������ď��� �r������ �C���t���������ʑΉ��iPRODUCT_BASE-1024�j
			s_lock.EnterReadLock();
			List<string> errorMessageXmls;
			try
			{
				errorMessageXmls = Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS.Select(string.Copy).ToList();
			}
			finally
			{
				s_lock.ExitReadLock();
			}

			// XML���X�V����Ă���ΐV�K�ɓǂݍ���
			foreach (string strXmlFilePath in errorMessageXmls)
			{
				if (File.Exists(strXmlFilePath))
				{
					if (s_dtFileLastUpdate < File.GetLastWriteTime(strXmlFilePath))
					{
						s_dtFileLastUpdate = File.GetLastWriteTime(strXmlFilePath);

						ReadMessagesXml(Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS);
						break;
					}
				}
			}

			// �ǂݍ��݃��b�N�������ď���
			s_lock.EnterReadLock();
			try
			{
				var message = string.Empty;
				//------------------------------------------------------
				// �G���[���b�Z�[�W�擾
				//------------------------------------------------------
				if (s_dicMessages.ContainsKey(messageKey))
				{
					message = s_dicMessages[messageKey];
				}
				else if (s_dicMessages.ContainsKey(MessageManager.ERRMSG_SYSTEM_ERROR))
				{
					message = s_dicMessages[MessageManager.ERRMSG_SYSTEM_ERROR];
				}

				if (replaces.Any())
				{
					message = replaces
						.Select(
							(value, index) => new
							{
								Value = value,
								Number = index + 1,
							})
						.Aggregate(
							message,
							(before, replace) =>
							{
								return before.Replace(string.Format("@@ {0} @@", replace.Number), replace.Value);
							});
				}

				return message;
			}
			finally
			{
				s_lock.ExitReadLock();
			}
		}

		/// <summary>
		/// ���b�Z�[�W��XML���擾���A�f�B�N�V���i���֐ݒ�
		/// </summary>
		/// <param name="lXmlFilePaths">�ǂݍ���XML�����p�X�̃��X�g</param>
		/// <returns>�擾���b�Z�[�W</returns>
		private void ReadMessagesXml(List<string> lXmlFilePaths)
		{
			// �������݃��b�N��������Dictionary�X�V
			s_lock.EnterWriteLock();
			try
			{
				var xdErrorMessages = new XmlDocument();
				foreach (string strXmlPath in lXmlFilePaths)
				{
					xdErrorMessages.Load(strXmlPath);
					foreach (XmlNode xnErrorMessage in xdErrorMessages.SelectSingleNode("ErrorMessages").ChildNodes)
					{
						if (xnErrorMessage.NodeType == XmlNodeType.Comment)
						{
							continue;
						}

						s_dicMessages[xnErrorMessage.Name] = xnErrorMessage.InnerText;
					}
				}
			}
			finally
			{
				s_lock.ExitWriteLock();
			}
		}
	}
}
