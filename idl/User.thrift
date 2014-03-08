include "Exception.thrift"

namespace java com.savariwala
namespace csharp SavariWala
namespace cpp savariwala

struct  User {
  1: string fbUserId,
  2: string userName,
  3: bool isPassenger
}

service UsersManager {
  User getUser(1:string fbUserId) throws (1:Exception.ServerError err),
  void addUser(1:User user)  throws (1:Exception.ServerError err)
}
