namespace java com.savariwala
namespace csharp SavariWala

struct  User {
  1: string fbUserId,
  2: string userName,
  3: bool isPassenger
}

service UsersManager {
  User getUser(1:string fbUserId),
  void addUser(1:User user)
}
