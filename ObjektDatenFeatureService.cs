using System.Globalization;

using GVB.MyFeuerwehr.Application.File;
using GVB.MyFeuerwehr.Domain;
using GVB.MyFeuerwehr.Domain.File;
using GVB.MyFeuerwehr.Domain.Geo;

using NetTopologySuite.Features;

using Newtonsoft.Json.Serialization;

namespace GVB.MyFeuerwehr.Application.Geo;

public class ObjektDatenFeatureService : IObjektDatenFeatureService
{
    private readonly IObjektDatenQueryRepository _objektDatenQueryRepository;
    private readonly FileMetaDtoMapper _fileMetaDtoMapper;
    private readonly IFileRepository _fileRepository;

    public ObjektDatenFeatureService(
        IObjektDatenQueryRepository objektDatenQueryRepository, FileMetaDtoMapper fileMetaDtoMapper,
        IFileRepository fileRepository)
    {
        _objektDatenQueryRepository = objektDatenQueryRepository;
        _fileMetaDtoMapper = fileMetaDtoMapper;
        _fileRepository = fileRepository;
    }

    public async Task<string> LoadFeatures(string gemeindeName)
    {
        var elements = await _objektDatenQueryRepository.GetObjektdatenForGemeinde(gemeindeName);
        
        var featureCollectionResult = new FeatureCollection();
        foreach (var objekt in elements)
        {
            var attributes = new AttributesTable
            {
                { "id", objekt.KeyObjektDaten }, 
                { nameof(objekt.Gemeinde).FirstLetterLowerCase(), objekt.Gemeinde },
                { nameof(objekt.Art).FirstLetterLowerCase(), objekt.Art },
                { nameof(objekt.Adresse).FirstLetterLowerCase(), objekt.Adresse },
                { nameof(objekt.Ort).FirstLetterLowerCase(), objekt.Ort },
                { nameof(objekt.Plz).FirstLetterLowerCase(), objekt.Plz },
                { nameof(objekt.Telefon).FirstLetterLowerCase(), objekt.Telefon }
            };
            var feature = new Feature(objekt.GeoLocation, attributes);
            featureCollectionResult.Add(feature);
        }

        return ConvertFeatureCollectionToJsonString(featureCollectionResult);
    }
    
    private static string ConvertFeatureCollectionToJsonString(FeatureCollection features)
    {
        var geoJsonWriter = new NetTopologySuite.IO.GeoJsonWriter();
        geoJsonWriter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        return geoJsonWriter.Write(features);
    }
}