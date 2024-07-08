if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LogProccessBeginSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_LogProccessBeginSp]
GO
/*
=========================================================================================================
  Module      : ���O���H�J�n�v���V�[�W��(w2_LogProccessBeginSp.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_LogProccessBeginSp] @TARGET_DATE varchar(10) OUTPUT AS

	-- ������
	SET @TARGET_DATE = ''

	-- ���H�\�����擾
	SELECT	@TARGET_DATE = target_date
	  FROM	w2_AccessLogStatus
	 WHERE	day_status = '10'

	-- �擾�ł��Ȃ���Η�O
	IF @TARGET_DATE = ''
	BEGIN
		RAISERROR ('���H�\�ȃ��O�͂���܂���',16,1)
	END
	
	--�����J�n�X�e�[�^�X�֕ύX
	UPDATE	w2_AccessLogStatus
	   SET	day_status = '11'
	 WHERE	target_date = @TARGET_DATE

GO