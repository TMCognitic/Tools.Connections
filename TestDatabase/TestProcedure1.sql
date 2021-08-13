CREATE PROCEDURE [dbo].[TestProcedure1]
	@x int,
	@y int,
	@result int output
AS
Begin
	Set @result = @x + @y;
End
