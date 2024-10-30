use BikeStores
go

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [production].[images] (
    [image_id] INT IDENTITY(1,1) NOT NULL,
    [brand_id] INT NOT NULL,
    [image_path] NVARCHAR(MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([image_id] ASC),
    FOREIGN KEY (brand_id) REFERENCES [production].[brands](brand_id),  
) ON [PRIMARY]
GO


ALTER TABLE [production].[products]
DROP COLUMN image_id;  -- Adjust based on the actual column name if it's duplicated

ALTER TABLE [production].[products]
ADD image_id INT;


UPDATE [production].[products]
SET image_id = (
    SELECT TOP 1 i.image_id
    FROM [production].[images] i
    WHERE i.image_path LIKE '%' + LEFT(production.products.product_name, CHARINDEX('(', production.products.product_name + '(') - 1) + '%'
);

UPDATE [production].[products] 
SET image_id = (
    SELECT TOP 1 i.image_id
    FROM [production].[images] i
    WHERE 
        (CASE 
            WHEN p.brand_id = 1 THEN i.image_path LIKE '%' + 'Electra' + '%'
            WHEN p.brand_id = 2 THEN i.image_path LIKE '%' + 'Haro' + '%'
            WHEN p.brand_id = 3 THEN i.image_path LIKE '%' + 'Heller' + '%'
            WHEN p.brand_id = 4 THEN i.image_path LIKE '%' + 'Pure Cycles' + '%'
            WHEN p.brand_id = 5 THEN i.image_path LIKE '%' + 'Ritchey' + '%'
            WHEN p.brand_id = 6 THEN i.image_path LIKE '%' + 'Strider' + '%'
            WHEN p.brand_id = 7 THEN i.image_path LIKE '%' + 'Sun' + '%'
            WHEN p.brand_id = 8 THEN i.image_path LIKE '%' + 'Surly' + '%'
            WHEN p.brand_id = 9 THEN i.image_path LIKE '%' + 'Trek' + '%'
            ELSE 0
        END)
)



select * from production.products

