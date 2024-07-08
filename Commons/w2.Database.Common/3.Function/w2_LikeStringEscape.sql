if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LikeStringEscape]') and OBJECTPROPERTY(id, N'IsScalarFunction') = 1)
drop function [dbo].[w2_LikeStringEscape]
GO
/*
=========================================================================================================
  Module      : like������G�X�P�[�v�֐�(w2_LikeStringEscape.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.

  Desc        : like������̃G�X�P�[�v���s���B�iESCAPE�����Ăяo�����Ŏw�肷��K�v����j
	          : ��F  WHERE name like '%' + dbo.w2_LikeStringEscape( @word ) + '%' + ESCAPE '#'
=========================================================================================================
*/
CREATE FUNCTION w2_LikeStringEscape
	(
	@value [nvarchar] (4000)
	)
RETURNS nvarchar(4000) AS
BEGIN
	------------------------------------------------------
	-- �ϐ��錾��
	------------------------------------------------------
	DECLARE @result nvarchar(4000)

	------------------------------------------------------
	-- ����
	------------------------------------------------------
	SET @result	= @value
	SET @result	= REPLACE(@result, '#', '##')
	SET @result	= REPLACE(@result, '%', '#%')
	SET @result	= REPLACE(@result, '_', '#_')
	SET @result	= REPLACE(@result, '[', '#[')

	RETURN @result
END
