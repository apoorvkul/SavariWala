#include <thrift/protocol/TBinaryProtocol.h>
#include <thrift/server/TSimpleServer.h>
#include <thrift/transport/TServerSocket.h>
#include <thrift/transport/TTransportUtils.h>
#include <cppdb/frontend.h>
#include <gen-cpp/MapPointProvider.h>
#include <glog/logging.h>
#include <gflags/gflags.h>
#include <boost/make_shared.hpp>
#include <iostream>
#include <memory>
#include <cmath>
#include <vector>
#include <map>

namespace savariwala {
class MapPointProviderSvc : public MapPointProviderIf
{
    std::vector<std::shared_ptr<MapPoint>> pts_;
    std::vector<std::shared_ptr<MapPoint>> srcPts_;
    const double kmPerDegLat =  111.2;

public:

    void addPt(bool isSrc, const std::shared_ptr<MapPoint>& pt) 
    { 
        auto& selPts = isSrc ? srcPts_ : pts_;
        selPts.push_back(pt); 
    }

    // TODO: Use geohash to pin down the adjacent points first or else use Spatial Indexing from boost.geometry
    virtual void getMapPoint(std::vector<MapPoint> & _return, const bool isSrc, const double latitude, const double longitude)
    {
        std::multimap<double, std::shared_ptr<MapPoint>, std::greater<double>> top10;
        const double kmPerDegLng = kmPerDegLat * std::cos(latitude);
        const auto& selPts = isSrc ? srcPts_ : pts_;

        for(const auto& pt : selPts)
        {
           auto dist = kmPerDegLat * std::abs(pt->latitude - latitude) + kmPerDegLng * std::abs(pt->longitude - longitude); 
           auto first = top10.begin();
           if(dist < first->first)
           {
               top10.insert(std::make_pair(dist, pt));
               top10.erase(first);
           }
        }
        for(auto it = top10.crbegin(); it != top10.crend(); ++it) _return.push_back(*it->second);
    }

    virtual ~MapPointProviderSvc() {}
};
}

using namespace savariwala;

void loadPoint(MapPointProviderSvc& svc, cppdb::session& sql, bool isSrc)
{
    const std::string& query = std::string("SELECT latitude, longitude, description FROM ") +
        (isSrc ? "src_map_points" : "map_points");
    cppdb::result res = sql << query;

    while(res.next()) {
        std::shared_ptr<MapPoint> pt(new MapPoint());
        res >> pt->latitude >> pt->longitude >> pt->description;
        svc.addPt(isSrc, pt);
        LOG(INFO) << pt->latitude << ' ' << pt->longitude << ' ' << pt->description;
        std::cerr << pt->latitude << ' ' << pt->longitude << ' ' << pt->description << std::endl;
    }
}

//template <typename Svc>
void runThriftServer(const boost::shared_ptr<MapPointProviderSvc>& svc)
{
    using namespace apache::thrift;
    using namespace apache::thrift::protocol;
    using namespace apache::thrift::transport;
    using namespace apache::thrift::server;

    boost::shared_ptr<TProtocolFactory> protocolFactory(new TBinaryProtocolFactory());
    boost::shared_ptr<TProcessor> processor(new MapPointProviderProcessor(svc));
    boost::shared_ptr<TServerTransport> serverTransport(new TServerSocket(9090));
    boost::shared_ptr<TTransportFactory> transportFactory(new TBufferedTransportFactory());

    TSimpleServer server(processor, serverTransport, transportFactory, protocolFactory);

    LOG(INFO) << "Starting the server...";
    server.serve();
    LOG(INFO) << "Stopping the server...";
}

DEFINE_int32(port, 9091, "Server Port");
DEFINE_string(connection_string,"postgresql:dbname=rideshare;port=5432;user=rider;password=rider" , "Database connection string");

int main(int argc, char **argv)
{
    google::ParseCommandLineFlags(&argc, &argv, true);
    google::InitGoogleLogging(argv[0]);

    try 
    {
        auto svc = boost::make_shared<MapPointProviderSvc>(); 

        {
            cppdb::session sql(FLAGS_connection_string);
            loadPoint(*svc, sql, true);
            loadPoint(*svc, sql, false);
        }

        runThriftServer(svc);
        return 0;
    }
    catch(std::exception const &e) {
        LOG(ERROR) << e.what();
        return 1;
    }
}
