--------------------------------------------------------------------------------
-- <copyright file="Program.cs" company="SQLProj.com">
--         Copyright © 2013 SQLProj.com - All rights reserved.
-- </copyright>
--------------------------------------------------------------------------------
create proc olderr
as
begin
	set nocount on
	if (@a = 1)
	begin
		-- lets add some comments 1a
		raiserror 302653 'Amended Agreement does not exist'
		-- lets add some comments 1b
	end
	else
	begin
		-- lets add some comments 2a
		raiserror 302654 'Amended Agreement still does not exist'
		-- lets add some comments 2b
	end
	-- lets add some comments 3a
	raiserror 302655 'Amended Agreement does not exist, giving up'
	-- lets add some comments 3b
end
go
