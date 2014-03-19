namespace java com.savariwala
namespace csharp SavariWala
namespace cpp savariwala

enum ErrorCode {
  Unspecified = 0
  UserNotFound = 1
  InvalidArg = 2
  Interrupted = 3
}

exception ServerError {
  1: string what,
  2: ErrorCode err
}

