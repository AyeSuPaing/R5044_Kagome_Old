if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LikeStringEscape]') and OBJECTPROPERTY(id, N'IsScalarFunction') = 1)
drop function [dbo].[w2_LikeStringEscape]
GO
/*
=========================================================================================================
  Module      : like文字列エスケープ関数(w2_LikeStringEscape.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.

  Desc        : like文字列のエスケープを行う。（ESCAPE文を呼び出し元で指定する必要あり）
	          : 例：  WHERE name like '%' + dbo.w2_LikeStringEscape( @word ) + '%' + ESCAPE '#'
=========================================================================================================
*/
CREATE FUNCTION w2_LikeStringEscape
	(
	@value [nvarchar] (4000)
	)
RETURNS nvarchar(4000) AS
BEGIN
	------------------------------------------------------
	-- 変数宣言部
	------------------------------------------------------
	DECLARE @result nvarchar(4000)

	------------------------------------------------------
	-- 処理
	------------------------------------------------------
	SET @result	= @value
	SET @result	= REPLACE(@result, '#', '##')
	SET @result	= REPLACE(@result, '%', '#%')
	SET @result	= REPLACE(@result, '_', '#_')
	SET @result	= REPLACE(@result, '[', '#[')

	RETURN @result
END
