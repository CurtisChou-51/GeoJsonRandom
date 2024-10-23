using NetTopologySuite.Geometries;

namespace GeoJsonRandom.Core
{
    public static class RandomHelper
    {
        private static Random _random = new();
        private static GeometryFactory _geometryFactory = new();

        /// <summary> 依照權重由集合內隨機選取 </summary>
        public static T SelectItemByWeight<T>(IEnumerable<T> items, Func<T, double> weightFunc)
        {
            if (!items.Any())
                throw new ArgumentException("Items can not be empty", nameof(items));

            var weightedItems = items.Select(item => (Item: item, Weight: weightFunc(item))).ToList();
            double totalWeight = weightedItems.Sum(x => x.Weight);
            if (totalWeight <= 0)
                throw new ArgumentException("Total weight must be positive", nameof(weightFunc));

            double randomPoint = _random.NextDouble() * totalWeight;
            double accumulatedWeight = 0;
            foreach (var (item, weight) in weightedItems)
            {
                accumulatedWeight += weight;
                if (accumulatedWeight > randomPoint)
                    return item;
            }
            return items.Last();
        }

        /// <summary> 在Geometry內產生隨機點位 </summary>
        public static (decimal lng, decimal lat) GenerateLngLatIn(Geometry geo)
        {
            if (geo is MultiPolygon multiPolygon)
                return GenerateLngLatInMultiPolygon(multiPolygon);
            return GenerateLngLatInNonMultiPolygon(geo);
        }

        /// <summary> 在MultiPolygon內產生隨機點位 </summary>
        private static (decimal lng, decimal lat) GenerateLngLatInMultiPolygon(MultiPolygon multiPolygon)
        {
            // 直接對MultiPolygon取隨機可能因為分佈較破碎的狀況(ex:宜蘭縣頭城鎮大溪里)導致隨機取點未命中機率增加
            // 因此先依照面積隨機選出一個區域再對該區域隨機
            Geometry geo = SelectItemByWeight(multiPolygon.Geometries, x => x.Area);
            return GenerateLngLatIn(geo);
        }

        /// <summary> 在非MultiPolygon內產生隨機點位 </summary>
        private static (decimal lng, decimal lat) GenerateLngLatInNonMultiPolygon(Geometry nonMultiPolygon)
        {
            while (true)
            {
                // 在邊界框內隨機生成一個點，檢查點是否在多邊形內
                Envelope envelope = nonMultiPolygon.EnvelopeInternal;
                double x = Math.Round(_random.NextDouble() * (envelope.MaxX - envelope.MinX) + envelope.MinX, 6);
                double y = Math.Round(_random.NextDouble() * (envelope.MaxY - envelope.MinY) + envelope.MinY, 6);
                Point point = _geometryFactory.CreatePoint(new Coordinate(x, y));
                if (nonMultiPolygon.Contains(point))
                    return (lng: (decimal)x, lat: (decimal)y);
            }
        }
    }
}
