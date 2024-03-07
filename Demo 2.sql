SELECT * FROM ObjektDaten od 
	INNER JOIN Gemeinde gem ON od.GeoLocation.STIntersects(gem.GemeindeGeom) = 1
WHERE gem.Name = 'Aarberg'