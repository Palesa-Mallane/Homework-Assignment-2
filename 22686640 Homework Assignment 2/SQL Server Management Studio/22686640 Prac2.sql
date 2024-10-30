use DBSocialHire
go

Set Statistics Time ON


--SCRIPT 1

select p.Province_Name, COUNT(i.Item_ID) as TotalItems
from item i 
inner join Suburb s on i.Suburb_ID = s.Suburb_ID 
inner join City c on s.City_ID = c.City_ID 
inner join Province p on c.Province_ID = p.Province_ID 
group by Province_Name 
order by TotalItems desc 
Go

--create indexes 
drop index idx_city on City
drop index idx_province on Province
create index idx_city on City (City_ID)
create index idx_province on Province (Province_ID) 


select /*+ ALL_ROWS */ p.Province_Name, COUNT(i.Item_ID) as TotalItems  -- include optimiser hint to decrease overall execution time
from item i 
inner join Suburb s on i.Suburb_ID = s.Suburb_ID 
inner join City c on s.City_ID = c.City_ID 
inner join Province p on c.Province_ID = p.Province_ID 
group by Province_Name 
order by TotalItems desc 
Go

--SQL SERVER PARSE AND COMPILE TIMES
--WITHOUT ENHANCEMENTS: 9MS
--WITH ENHANCEMENTS: 5MS

--SCRIPT 2
select Item_Name, Suburb_Name, City_name, Province_Name
from Item i 
inner join Suburb s on i.Suburb_ID = s.Suburb_ID 
inner join City c on s.City_ID = c.City_ID 
inner join Province p on p.Province_ID = c.Province_ID 
order by c.City_ID, p.Province_ID -- Remove p.Province_ID in order by statement
Go

--create indexes
drop index idx_province on Province 
drop index idx_city on City

create index idx_province on Province (Province_ID)
create index idx_city on City(City_ID)


select Item_Name, Suburb_Name, City_name, Province_Name
from Item i 
inner join Suburb s on i.Suburb_ID = s.Suburb_ID 
inner join City c on s.City_ID = c.City_ID 
inner join Province p on p.Province_ID = c.Province_ID 
order by c.City_ID -- Remove p.Province_ID in order by statement
Go

--PARSE AND COMPILE
--WITHOUT ENHANCEMENTS: 5MS
--WITH ENHANCEMENTS: 4MS


--SCRIPT 3
--create indexes
drop index idx_province on Province 
drop index idx_city on City

create index idx_province on Province (Province_ID)
create index idx_city on City(City_ID)

SELECT i.Item_Name, CONCAT(i.Item_Address, ', ', s.Suburb_Name, ', ', c.City_Name, ', ', p.Province_Name) AS Address_Line -- add concat function
FROM Item i 
INNER JOIN Suburb s ON i.Suburb_ID = s.Suburb_ID 
INNER JOIN City c ON s.City_ID = c.City_ID 
INNER JOIN Province p ON c.Province_ID = p.Province_ID;
GO

--PARSE AND COMPILE TIME
--Without: 8ms
--With concat: 5ms
--with index: 3ms