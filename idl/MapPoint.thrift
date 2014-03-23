namespace java com.savariwala
namespace csharp SavariWala
namespace cpp savariwala

struct GeoLoc
{
   1:double lat, 
   2:double lng
}

// thrift does not support inheritance for structs
struct MapPoint {
  1: GeoLoc loc,
  2: string name,
  3: string address,
  4: string locality
}

