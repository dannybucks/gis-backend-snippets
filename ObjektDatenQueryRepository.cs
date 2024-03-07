using System.Data;

using Dapper;

using GVB.MyFeuerwehr.Application;
using GVB.MyFeuerwehr.Application.Geo;
using GVB.MyFeuerwehr.Domain;
using GVB.MyFeuerwehr.Domain.File;
using GVB.MyFeuerwehr.Domain.Geo;
using GVB.MyFeuerwehr.Domain.Heissausbildung;
using GVB.MyFeuerwehr.Domain.Mandant;

using NetTopologySuite.Geometries;

namespace GVB.MyFeuerwehr.Infrastructure.AppDb;

public class ObjektDatenQueryRepository : IObjektDatenQueryRepository
{
    private readonly IAppDbConnectionFactory _appDbConnectionFactory;

    public ObjektDatenQueryRepository(IAppDbConnectionFactory appDbConnectionFactory)
    {
        _appDbConnectionFactory = appDbConnectionFactory;
    }

    static ObjektDatenQueryRepository()
    {
        SqlMapper.AddTypeHandler(new GeometryHandler<Geometry>(geography: true));
    }
    
    public async Task<IList<ObjektDatenFeature>> GetObjektdatenForGemeinde(string gemeindeName)
    {
        var query = @"
                SELECT od.[KeyObjektDaten],
                       od.[ElementType],
                       od.[Art],
                       od.[Adresse],
                       od.[Plz],
                       od.[Ort],
                       od.[Gemeinde],
                       od.[Telefon],
                       od.[Laengengrad],
                       od.[Breitengrad],
                       od.[MeterUeberMeer],
                       od.[GeoLocation].Serialize() AS GeoLocation
                FROM data.ObjektDaten 
                    INNER JOIN Gemeinde gem ON od.GeoLocation.STIntersects(gem.GemeindeGeom) = 1
                WHERE gem.Name = @gemeindeName";

        var parameter = new { gemeindeName };

        using var sqlConnection = await _appDbConnectionFactory.CreateConnection();
        var objekte = await sqlConnection
            .QueryAsync<ObjektDatenFeature>(query, parameter)
            .ToListAsync();

        return objekte;
    }
}