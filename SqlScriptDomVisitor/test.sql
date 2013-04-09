create table dbo.t1
(
	c1 int not null identity(1,1) primary key clustered,
	c2 nvarchar(10) null,
	c3 datetime default getdate()
)
go

create view dbo.v1
as
select c1, c3, c2 from t1 where c3 < getdate()
go

create proc dbo.p1
as
begin
	set nocount on
	select * from v1
end
go
