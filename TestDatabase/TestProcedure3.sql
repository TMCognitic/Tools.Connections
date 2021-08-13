CREATE PROCEDURE [dbo].[TestProcedure3]
	@x int,
	@y int,
	@result int output
AS
Begin
	Set @result = @x + @y;
	select @x as x, @y as y, @result as addition;
End
