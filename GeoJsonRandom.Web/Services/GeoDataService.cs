﻿using GeoJsonRandom.Core;
using GeoJsonRandom.Models;
using Newtonsoft.Json;
using System.Text;

namespace GeoJsonRandom.Services
{
    public class GeoDataService : IGeoDataService
    {
        private readonly IGeoJsonRandomPointGenerator _geoJsonRandomPointGenerator;

        public GeoDataService(IGeoJsonRandomPointGenerator geoJsonRandomPointGenerator)
        {
            _geoJsonRandomPointGenerator = geoJsonRandomPointGenerator;
        }

        /// <summary> 取得縣市 </summary>
        public IEnumerable<string> GetCounties()
        {
            return _geoJsonRandomPointGenerator.GetDirectAdminNames([]).OrderBy(x => x);
        }

        /// <summary> 取得鄉鎮 </summary>
        public IEnumerable<string> GetTowns(string? county)
        {
            if (string.IsNullOrEmpty(county))
                return Enumerable.Empty<string>();
            return _geoJsonRandomPointGenerator.GetDirectAdminNames([county]).OrderBy(x => x);
        }

        /// <summary> 取得村里 </summary>
        public IEnumerable<string> GetVillages(string? county, string? town)
        {
            if (string.IsNullOrEmpty(county) || string.IsNullOrEmpty(town))
                return Enumerable.Empty<string>();
            return _geoJsonRandomPointGenerator.GetDirectAdminNames([county, town]).OrderBy(x => x);
        }

        /// <summary> 產生隨機點位 </summary>
        public IEnumerable<GeoDataResultDto> GenerateRandomPoints(GeoDataConditionDto dto)
        {
            dto.TakeCount = Math.Min(dto.TakeCount, 1000);
            return _geoJsonRandomPointGenerator.GenerateRandomPoints(dto);
        }

        /// <summary> 產生隨機點位JsonFile </summary>
        public Stream GenerateRandomPointsJsonFile(GeoDataConditionDto dto)
        {
            var result = GenerateRandomPoints(dto);
            MemoryStream stream = new MemoryStream();
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
            JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.Indented };
            serializer.Serialize(writer, result);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary> 產生隨機點位CsvFile </summary>
        public Stream GenerateRandomPointsCsvFile(GeoDataConditionDto dto)
        {
            var result = GenerateRandomPoints(dto);
            MemoryStream stream = new MemoryStream();
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
            writer.WriteLine("縣市,鄉鎮,村里,緯度,經度");
            foreach (var item in result)
                writer.WriteLine($"{item.County},{item.Town},{item.Village},{item.Latitude},{item.Longitude}");
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}