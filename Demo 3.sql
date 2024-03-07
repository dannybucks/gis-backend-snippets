declare @geom GEOGRAPHY = GEOGRAPHY::STGeomFromText('POLYGON((7.26 47.06, 7.28 47.06, 7.28 47.04, 7.26 47.04, 7.26 47.06))', 4326).MakeValid()
select  @geom

SELECT GemeindeGeom.STIntersection (@geom)
FROM [gisdemo].[dbo].[Gemeinde]
WHERE Name = 'Aarberg'