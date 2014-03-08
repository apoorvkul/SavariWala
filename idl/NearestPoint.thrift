namespace java com.savariwala
namespace csharp SavariWala
namespace cpp savariwala

struct MapPoint {
  1: double latitude,
  2: double longitude,
  3: string description
}

service MapPointProvider {
  list<MapPoint> getMapPoint(1:bool isSrc, 2:double latitude, 3:double longitude)
}
