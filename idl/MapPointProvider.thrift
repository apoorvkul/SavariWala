include "MapPoint.thrift"

namespace java com.savariwala
namespace csharp SavariWala
namespace cpp savariwala

service MapPointProvider {
  list<MapPoint.MapPoint> getMapPoint(1:bool isSrc, 2:MapPoint.GeoLoc loc)
}
