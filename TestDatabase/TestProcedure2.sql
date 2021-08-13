CREATE PROCEDURE [dbo].[TestProcedure2]
	@x int,
	@y int,
	@result int output
AS
Begin
	Set @result = @x + @y;
	select @result;
End
