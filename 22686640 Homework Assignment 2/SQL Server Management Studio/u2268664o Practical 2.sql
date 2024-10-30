use DBSocialHire
GO

Set Statistics Time ON

--SCRIPT 1

--Create indexes on tables used in joins
DROP INDEX idx_item_suburb_id on Item
DROP INDEX idx_suburb_city_id on Suburb

CREATE INDEX idx_item_suburb_id ON Item (Suburb_ID);
CREATE INDEX idx_suburb_city_id ON Suburb (City_ID);



SELECT /*+ INDEX(i idx_item_suburb) INDEX(s idx_suburb_city) */  --Use optimiser hint
       p.Province_Name,
       COUNT(i.Item_ID) as TotalItems
FROM item i
INNER JOIN Suburb s ON i.Suburb_ID = s.Suburb_ID
INNER JOIN City c ON s.City_ID = c.City_ID
INNER JOIN Province p ON c.Province_ID = p.Province_ID
GROUP BY p.Province_Name
ORDER BY TotalItems DESC;


--SCRIPT 2

--Create indexes on tables in join statements
DROP INDEX idx_item_suburb_id ON Item 
DROP INDEX idx_suburb_city_id ON Suburb

CREATE INDEX idx_item_suburb_id ON Item (Suburb_ID);
CREATE INDEX idx_suburb_city_id ON Suburb (City_ID);

SELECT i.Item_Name, s.Suburb_Name, c.City_Name, p.Province_Name
FROM Item i
INNER JOIN Suburb s ON i.Suburb_ID = s.Suburb_ID
INNER JOIN City c ON s.City_ID = c.City_ID
INNER JOIN Province p ON p.Province_ID = c.Province_ID
ORDER BY p.Province_ID -- Remove City_ID in order by statement
GO



--SCRIPT 3
--Create indexes on tables used in join
DROP INDEX idx_item_suburb_id ON Item 
DROP INDEX idx_suburb_city_id ON Suburb

CREATE INDEX idx_item_suburb_id ON Item (Suburb_ID);
CREATE INDEX idx_suburb_city_id ON Suburb (City_ID);


SELECT i.Item_Name, CONCAT(i.Item_Address, ', ', s.Suburb_Name, ', ', c.City_Name, ', ', p.Province_Name) AS Address_Line
FROM Item i
INNER JOIN Suburb s ON i.Suburb_ID = s.Suburb_ID
INNER JOIN City c ON s.City_ID = c.City_ID
INNER JOIN Province p ON c.Province_ID = p.Province_ID;
GO

